using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Security.Cryptography;
using System.Text;

using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server.Commands;

namespace Terraria_Server.RemoteConsole
{
	public static class RConServer
	{
		static volatile bool exit = false;
		static Thread thread;
		static List<RConClient> clients = new List<RConClient> ();
		
		public static PropertiesFile LoginDatabase { get; private set; }
		
		public static void Start (string dbPath)
		{
			LoginDatabase = new PropertiesFile (dbPath);
			LoginDatabase.Load ();
			
			if (LoginDatabase.Count == 0)
			{
				var bytes = new byte [8];
				(new Random ((int) DateTime.Now.Ticks)).NextBytes (bytes);
				
				string password = string.Format ("{0:x2}{1:x2}-{2:x2}{3:x2}-{4:x2}{5:x2}-{6:x2}{7:x2}",
					bytes[0], bytes[1], bytes[2], bytes[3], bytes[4], bytes[5], bytes[6], bytes[7]);
				string login = "Owner";
				ProgramLog.Admin.Log ("The rcon login database was empty, a new user \"{1}\" has been created with password: {0}", password, login);
				
				LoginDatabase.setValue (login, Hash (login, password));
			}
			
			LoginDatabase.Save ();
			
			thread = new Thread (RConLoop);
			thread.Start ();
		}
		
		public static void Stop ()
		{
			exit = true;
		}
		
		internal static string Hash (string username, string password)
		{
			var hash = SHA256.Create ();
			var sb = new StringBuilder (64);
			var bytes = hash.ComputeHash (Encoding.ASCII.GetBytes (username + ":rcon:" + password));
			foreach (var b in bytes)
				sb.Append (b.ToString ("x2"));
			return sb.ToString ();
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
			
			ProgramLog.Admin.Log ("Remote console server started on 127.0.0.1:7776.");
			
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
									ProgramLog.Admin.Log ("{0}: remote console closed.", rcon.Id);
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
			
			ProgramLog.Admin.Log ("Remote console server stopped.");
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
					ProgramLog.Admin.Log ("Accepted socket disconnected");
					return null;
				}
				
				ProgramLog.Admin.Log ("New remote console connection from: {0}", addr);
				
				var rcon = new RConClient (client, addr);
				clients.Add (rcon);
				rcon.Greet ();
				
				return rcon;
			}
			catch (Exception e)
			{
				ProgramLog.Error.Log ("Accepted socket exception ({1})", HandleSocketException (e));
				return null;
			}
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
				ProgramLog.Debug.Log ("{0}: socket exception ({1})", rcon.Id, HandleSocketException (e));
			}
			
			if (recv > 0)
			{
				try
				{
					rcon.bytesRead += recv;
					return rcon.ProcessRead ();
				}
				catch (Exception e)
				{
					ProgramLog.Log (e, "Error processing remote console data stream");
				}
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
				ProgramLog.Admin.Log ("{0}: remote console connection closed", rcon.Id);
			}
			catch (Exception e)
			{
				HandleSocketException (e);
				ProgramLog.Admin.Log ("{0}: remote console connection closed", rcon.Id);
			}
			
			socket.SafeClose ();
		}
		
		static void DisposeClient (RConClient rcon)
		{
			clients.Remove (rcon);
			rcon.Close ();
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
		
		internal static void RConCommand (Server dummy, ISender sender, ArgumentList args)
		{
			string name;
			if (args.TryParseOne ("cut", out name))
			{
				if (sender is Player || sender is RConSender)
				{
					sender.sendMessage ("You cannot perform that action.", 255, 238, 130, 238);
					return;
				}

				var lower = name.ToLower();
				foreach (var rcon in clients)
				{
					if (rcon.Name.ToLower() == lower)
						rcon.Close ();
				}
				
				ProgramLog.Admin.Log ("Cut all remote console connections from {0}.", name);
			}
			else if (args.TryParseOne ("ban", out name))
			{
				if (sender is Player || sender is RConSender)
				{
					sender.sendMessage ("You cannot perform that action.", 255, 238, 130, 238);
					return;
				}

				LoginDatabase.setValue (name, null);
				
				var lower = name.ToLower();
				foreach (var rcon in clients)
				{
					if (rcon.Name.ToLower() == lower)
						rcon.Close ();
				}
				
				ProgramLog.Admin.Log ("Cut all remote console connections from {0} and revoked credentials.", name);
			}
			else if (args.Count == 1 && args.GetString(0) == "list")
			{
				foreach (var rcon in clients)
				{
					sender.sendMessage (string.Format ("{0} {1}", rcon.Id, rcon.state));
				}
			}
			else if (args.Count == 1 && args.GetString(0) == "load")
			{
				if (sender is Player || sender is RConSender)
				{
					sender.sendMessage ("You cannot perform that action.", 255, 238, 130, 238);
					return;
				}

				LoginDatabase.Load ();
				ProgramLog.Admin.Log ("Reloaded remote console login database.");
			}
			else
			{
				throw new CommandError ("");
			}
		}
	}
}

