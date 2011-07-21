using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

using Terraria_Server.Logging;

namespace Terraria_Server.RemoteConsole
{
	public static class RConServer
	{
		static volatile bool exit = false;
		static Thread thread;
		
		public static void Start ()
		{
			thread = new Thread (RConLoop);
			thread.Start ();
		}
		
		public static void Stop ()
		{
			exit = true;
		}
		
		public static void RConLoop ()
		{
			Thread.CurrentThread.Name = "RCon";
			
			var listener = new TcpListener (IPAddress.Parse ("127.0.0.1"), 7776);
			
			try
			{
				listener.Start();
			}
            catch (Exception exception)
			{
				ProgramLog.Log (exception, "Failed to bind to address 127.0.0.1:" + 7776);
				return;
			}
			
			ProgramLog.Log ("Remote console server started on 127.0.0.1:7776.");
			
			var socketToObject = new Dictionary<Socket, RConClient> ();
			var readList = new List<Socket> ();
			var errorList = new List<Socket> ();
			var clientList = new List<Socket> ();
			var serverSock = listener.Server;
			
			try
			{
				while (! exit)
				{
					readList.Clear ();
					readList.Add (serverSock);
					readList.AddRange (clientList);
					errorList.Clear ();
					errorList.AddRange (clientList);

					Socket.Select (readList, null, errorList, 500000);
					
					if (exit) break;

					foreach (var sock in errorList)
					{
						CheckError (sock, socketToObject);
						
						if (socketToObject.ContainsKey (sock))
							DisposeClient (socketToObject[sock]);
						
						clientList.Remove (sock);
						readList.Remove (sock);
					}

					foreach (var sock in readList)
					{
						if (sock == serverSock)
						{
							// Accept new clients
							while (listener.Pending())
							{
								var client = listener.AcceptSocket ();
								var rcon = AcceptClient (client);
								if (rcon != null)
								{
									clientList.Add (client);
									socketToObject[client] = rcon;
								}
							}
						}
						else
						{
							// Handle existing clients
							bool rem = false;
							RConClient rcon = null;
							try
							{
								rcon = socketToObject[sock];
								rem = ! ReadFromClient (rcon, sock);
							}
							catch (Exception e)
							{
								HandleSocketException (e);
								rem = true;
							}
							
							if (rem)
							{
								sock.SafeClose ();
								
								if (rcon != null)
								{
									DisposeClient (rcon);
								}
								
								clientList.Remove (sock);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "RConLoop terminated with exception");
			}
			
			try
			{
				listener.Stop ();
			}
			catch (SocketException) {}
			
			ProgramLog.Log ("Remote console server stopped.");
		}
		
		static RConClient AcceptClient (Socket client)
		{
			client.NoDelay = false;
			
			string addr;
			try
			{
				var rep = client.RemoteEndPoint;
				if (rep != null)
					addr = rep.ToString();
				else
				{
					Program.tConsole.WriteLine ("Accepted socket disconnected");
					return null;
				}
			}
			catch (Exception e)
			{
				Program.tConsole.WriteLine ("Accepted socket exception ({1})", HandleSocketException (e));
				return null;
			}
			
			ProgramLog.Log ("New remote console connection from: {0}", addr);
			return new RConClient (client, addr);
		}
		
		static bool ReadFromClient (RConClient rcon, Socket socket)
		{
			var buf = rcon.readBuffer;
			int recv = -1;
			
			try
			{
				recv =
					socket.Receive (
						buf,
						rcon.bytesRead,
						buf.Length - rcon.bytesRead,
						0);
			}
			catch (Exception e)
			{
				Program.tConsole.WriteLine ("{0}: socket exception ({1})", rcon.remoteAddress, HandleSocketException (e));
			}
			
			if (recv > 0)
			{
				rcon.bytesRead += recv;
				rcon.ProcessRead ();
				return true; // don't close connection even if kicking, let the sending thread finish
			}
			else
			{
				Program.tConsole.WriteLine ("{0}: remote console closed.", rcon.remoteAddress);
			}
			
			return false;
		}
		
		static byte[] errorBuf = new byte[1];
		static void CheckError (Socket socket, Dictionary<Socket, RConClient> socketToObject)
		{
			string addr = "<address lost>";
			RConClient rcon = null;
			
			if (socketToObject.ContainsKey (socket))
			{
				rcon = socketToObject[socket];
				addr = rcon.remoteAddress;
			}
			
			try
			{
				addr = socket.RemoteEndPoint.ToString();
			}
			catch (Exception) {}
			
			try
			{
				socket.Receive (errorBuf);
				Program.tConsole.WriteLine ("{0}: remote console connection closed", addr);
			}
			catch (Exception e)
			{
				HandleSocketException (e);
				Program.tConsole.WriteLine ("{0}: remote console connection closed", addr);
			}
			
			socket.SafeClose ();
		}
		
		static void DisposeClient (RConClient rcon)
		{
			rcon.socket.SafeClose ();
		}
		
		static string HandleSocketException (Exception e)
		{
			if (e is SocketException)
				return e.Message + " @ " + e.StackTrace;
			else if (e is ObjectDisposedException)
				return "Socket already disposed @ " + e.StackTrace;
			else
				throw new Exception ("Unexpected exception in socket handling code", e);
		}

	}
}

