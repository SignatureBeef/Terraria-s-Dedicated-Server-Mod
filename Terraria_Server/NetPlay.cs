using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using Terraria_Server.Messages;
using System.Diagnostics;
namespace Terraria_Server
{
	public static class Netplay
	{
		public const int bufferSize = 1024;
		public const int maxConnections = 256;
		public static bool stopListen = false;
		public static ServerSlot[] slots = new ServerSlot[256];
		public static TcpListener tcpListener;
		public static IPAddress serverListenIP;
		public static IPAddress serverIP;
		public static int serverPort = 7777;
		public static String serverSIP = "0.0.0.0";
		public static bool disconnect = false;
		public static String password = "";
		public static bool spamCheck = false;
		public static bool ServerUp = false;
		public static bool anyClients = false;
		
		public static void SafeClose (this Socket socket)
		{
			if (socket == null) return;
			
			try
			{
				socket.Close ();
			}
			catch (SocketException) {}
			catch (ObjectDisposedException) {}
		}
		
		public static void ServerLoop ()
		{
			if (Main.rand == null)
			{
				Main.rand = new Random((int)DateTime.Now.Ticks);
			}
			if (WorldGen.genRand == null)
			{
				WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
			}
		
			Main.myPlayer = 255;
			Main.players[255].whoAmi = 255;
			Netplay.serverIP = IPAddress.Parse(serverSIP);
			Netplay.serverListenIP = Netplay.serverIP;
			Netplay.disconnect = false;
			
			for (int i = 0; i < 256; i++)
			{
				Netplay.slots[i] = new ServerSlot();
				Netplay.slots[i].Reset();
				Netplay.slots[i].whoAmI = i;
			}
			
			Netplay.tcpListener = new TcpListener(Netplay.serverListenIP, Netplay.serverPort);
			
			try
			{
				Netplay.tcpListener.Start();
			}
            catch (Exception exception)
			{
				Main.menuMode = 15;
				Main.statusText = exception.ToString();
				Netplay.disconnect = true;
			}
			
			if (!Netplay.disconnect)
			{
				Program.updateThread.Start();
				Program.tConsole.WriteLine("Server started on " + serverSIP + ":" + serverPort.ToString());
				Program.tConsole.WriteLine("Loading Plugins...");
				Program.server.PluginManager.LoadPlugins();
				Program.tConsole.WriteLine("Plugins Loaded: " + Program.server.PluginManager.getPluginList().Count.ToString());
				Statics.serverStarted = true;
			}
			else
				return;
			
			var socketToId = new Dictionary<Socket, int> ();
			var readList = new List<Socket> ();
			var errorList = new List<Socket> ();
			var clientList = new List<Socket> ();
			var serverSock = Netplay.tcpListener.Server;
			
			try
			{
				while (!Netplay.disconnect)
				{
					Netplay.anyClients = clientList.Count > 0;
					
					readList.Clear ();
					readList.Add (serverSock);
					readList.AddRange (clientList);
					errorList.Clear ();
					errorList.AddRange (clientList);

					Socket.Select (readList, null, errorList, 500000);
					
					if (Netplay.disconnect) break;

					foreach (var sock in errorList)
					{
						CheckError (sock, socketToId);
						
						if (socketToId.ContainsKey (sock))
							DisposeClient (socketToId[sock]);
						
						clientList.Remove (sock);
						readList.Remove (sock);
					}

					foreach (var sock in readList)
					{
						if (sock == serverSock)
						{
							// Accept new clients
							while (Netplay.tcpListener.Pending())
							{
								var client = Netplay.tcpListener.AcceptSocket ();
								var id = AcceptClient (client);
								if (id >= 0)
								{
									clientList.Add (client);
									Netplay.anyClients = true;
									socketToId[client] = id;
									
									if (clientList.Count > Main.maxNetplayers)
									{
										slots[id].Kick ("Server full, sorry.");
									}
								}
							}
						}
						else
						{
							// Handle existing clients
							bool rem = false;
							int id = -1;
							try
							{
								id = socketToId[sock];
								rem = ! ReadFromClient (id, sock);
							}
							catch (Exception e)
							{
								HandleSocketException (e);
								rem = true;
							}
							
							if (rem)
							{
								sock.SafeClose ();
								
								if (id >= 0)
								{
									DisposeClient (id);
								}
								
								clientList.Remove (sock);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Program.tConsole.WriteLine ("ServerLoop terminated with exception:");
				Program.tConsole.WriteLine (e.ToString());
			}
			
			Netplay.anyClients = false;
			
			try
			{
				tcpListener.Stop ();
			}
			catch (SocketException) {}
			
			for (int i = 0; i < 255; i++)
			{
				try
				{
					slots[i].Kick ("Server restarting, please reconnect");
				}
				catch {}
			}
			
			Thread.Sleep (1000);
			
			for (int i = 0; i < 255; i++)
			{
				try
				{
					slots[i].Reset ();
				}
				catch {}
			}
			
			foreach (var sock in clientList)
			{
				try
				{
					sock.Close ();
				}
				catch {}
			}
			
			Statics.serverStarted = false;
		}
		
		static int lastId = 0;
		static int AcceptClient (Socket client)
		{
			client.NoDelay = true;
			
			string addr;
			try
			{
				var rep = client.RemoteEndPoint;
				if (rep != null)
					addr = rep.ToString();
				else
				{
					Program.tConsole.WriteLine ("Accepted socket disconnected");
					return -1;
				}
			}
			catch (Exception e)
			{
				Program.tConsole.WriteLine ("Accepted socket exception ({1})", HandleSocketException (e));
				return -1;
			}
			
			for (int i = 0; i < 255; i++)
			{
				int k = (lastId + i) % 255;
				if (slots[k].state == SlotState.VACANT)
				{
					lastId = k;
					Program.tConsole.WriteLine ("{0} is connecting on slot {1}...", addr, k);
					try
					{
						AcceptSlot (k, client, addr);
					}
					catch (SocketException)
					{
						client.SafeClose ();
			
						Program.tConsole.WriteLine ("{0} disconnected.", addr);
						
						return -1;
					}
					return k;
				}
			}
			
			client.SafeClose ();

			Program.tConsole.WriteLine ("{0} dropped, no slots left.", addr);
			
			return -1;
		}
		
		static void AcceptSlot (int id, Socket client, string remoteAddress)
		{
			var slot = slots[id];
			slot.remoteAddress = remoteAddress;
			Main.players[id].setIPAddress(remoteAddress);
			slot.state = SlotState.CONNECTED;
			slot.socket = client;
			if (slot.readBuffer == null) slot.readBuffer = new byte[1024];
			if (NetMessage.buffer[id].readBuffer == null) NetMessage.buffer[id].readBuffer = new byte [MessageBuffer.BUFFER_MAX];
			Program.tConsole.WriteLine ("Slot {1} assigned to {0}.", remoteAddress, id);
		}
		
		static bool ReadFromClient (int id, Socket socket)
		{
			var buf = NetMessage.buffer[id].readBuffer;
			var slot = slots[id];
			int recv = -1;
			
			try
			{
				recv =
					socket.Receive (
						buf,
						NetMessage.buffer[id].totalData,
						buf.Length - NetMessage.buffer[id].totalData,
						0);
			}
			catch (Exception e)
			{
				Program.tConsole.WriteLine ("{0} @ {2}: socket exception ({1})", slot.remoteAddress, HandleSocketException (e), id);
			}
			
			if (recv > 0)
			{
				if ((slots[id].state & (SlotState.KICK | SlotState.SHUTDOWN)) == 0)
				{
					NetMessage.buffer[id].totalData += recv;
					NetMessage.CheckBytes (id);
				}
				return true; // don't close connection even if kicking, let the sending thread finish
			}
			else
			{
				Program.tConsole.WriteLine ("{0} @ {1}: connection closed.", slot.remoteAddress, id);
			}
			
			return false;
		}
		
		static byte[] errorBuf = new byte[1];
		static void CheckError (Socket socket, Dictionary<Socket, int> socketToId)
		{
			string addr = "<address lost>";
			int id = -1;
			
			if (socketToId.ContainsKey (socket))
			{
				id = socketToId[socket];
				addr = slots[id].remoteAddress;
			}
			
			try
			{
				addr = socket.RemoteEndPoint.ToString();
			}
			catch (Exception) {}
			
			try
			{
				socket.Receive (errorBuf);
				if (id >= 0)
					Program.tConsole.WriteLine ("{0} @ {1}: connection closed", addr, id);
				else
					Program.tConsole.WriteLine ("{0}: connection closed", addr);
			}
			catch (Exception e)
			{
				HandleSocketException (e);
				if (id >= 0)
					Program.tConsole.WriteLine ("{0} @ {1}: connection closed", addr, id);
				else
					Program.tConsole.WriteLine ("{0}: connection closed", addr);
			}
			
			socket.SafeClose ();
		}
		
		static void DisposeClient (int id)
		{
			Program.tConsole.WriteLine ("Freeing slot {0}", id);
			
			try
			{
				slots[id].Reset ();
			}
			catch (Exception e)
			{
				HandleSocketException (e);
			}
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
		
		private static Thread serverThread;
		
		public static void StartServer()
		{
			if (serverThread == null)
			{
				serverThread = new Thread (Netplay.ServerLoopLoop);
				serverThread.Name = "ServerLoop";
				serverThread.IsBackground = true;
				serverThread.Start();
			}
			disconnect = false;
		}

		public static void StopServer()
		{
			Statics.IsActive = Statics.keepRunning; //To keep console active & program alive upon restart;
			Program.tConsole.WriteLine("Disabling Plugins");
			Program.server.PluginManager.DisablePlugins();
			Program.tConsole.WriteLine("Closing Connections...");
			disconnect = true;
		}
		
		private static void ServerLoopLoop ()
		{
			while (true)
			{
				ServerLoop ();
				while (disconnect) Thread.Sleep (100);
			}
		}

		public static bool SetIP(String newIP)
		{
			try
			{
				Netplay.serverIP = IPAddress.Parse(newIP);
			}
			catch
			{
				return false;
			}
			return true;
		}
		
		public static bool SetIP2(String newIP)
		{
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(newIP);
				IPAddress[] addressList = hostEntry.AddressList;
				for (int i = 0; i < addressList.Length; i++)
				{
					if (addressList[i].AddressFamily == AddressFamily.InterNetwork)
					{
						Netplay.serverIP = addressList[i];
						return true;
					}
				}
			}
			catch
			{
			}
			return false;
		}
		
		public static void Init()
		{
			for (int i = 0; i < 257; i++)
			{
				if (i < 256)
				{
					Netplay.slots[i] = new ServerSlot();
				}
				NetMessage.buffer[i] = new MessageBuffer();
				NetMessage.buffer[i].whoAmI = i;
			}
		}
		
		public static int GetSectionX(int x)
		{
			return x / 200;
		}
		
		public static int GetSectionY(int y)
		{
			return y / 150;
		}
	}
}
