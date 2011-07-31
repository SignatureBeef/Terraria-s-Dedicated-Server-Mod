using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using System.Diagnostics;

using Terraria_Server.Messages;
using Terraria_Server.Logging;
using Terraria_Server.WorldMod;

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
		internal static Queue<Socket> deadClients;
		
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

		public static void SafeShutdown (this Socket socket)
		{
			if (socket == null) return;
			
			try
			{
				socket.Shutdown (SocketShutdown.Both);
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
			if (WorldModify.genRand == null)
			{
				WorldModify.genRand = new Random((int)DateTime.Now.Ticks);
			}
		
			Main.myPlayer = 255;
			Main.players[255].whoAmi = 255;
			Netplay.serverIP = IPAddress.Parse(serverSIP);
			Netplay.serverListenIP = Netplay.serverIP;
			Netplay.disconnect = false;
			
//			for (int i = 0; i < 256; i++)
//			{
////                Netplay.slots[i] = new ServerSlot();
//                Netplay.slots[i].whoAmI = i;
//				Netplay.slots[i].Reset();
//			}
			
			Netplay.tcpListener = new TcpListener(Netplay.serverListenIP, Netplay.serverPort);
			
			try
			{
				Netplay.tcpListener.Start();
			}
            catch (Exception exception)
			{
				Main.statusText = exception.ToString();
				Netplay.disconnect = true;
			}
			
			if (!Netplay.disconnect)
			{
				if (! Program.updateThread.IsAlive) Program.updateThread.Start();
				Program.tConsole.WriteLine("Server started on " + serverSIP + ":" + serverPort.ToString());
				Program.tConsole.WriteLine("Loading Plugins...");
				Program.server.PluginManager.LoadPlugins();
				Program.tConsole.WriteLine("Plugins Loaded: " + Program.server.PluginManager.PluginList.Count.ToString());
				Statics.serverStarted = true;
			}
			else
				return;
			
			var serverSock = Netplay.tcpListener.Server;
			
			try // TODO: clean up sometime, error handling too spread out
			{
				while (!Netplay.disconnect)
				{
					Netplay.anyClients = Networking.ClientConnection.All.Count > 0; //clientList.Count > 0;
					
					serverSock.Poll (500000, SelectMode.SelectRead);
					
					if (Netplay.disconnect) break;

					// Accept new clients
					while (Netplay.tcpListener.Pending())
					{
						var client = Netplay.tcpListener.AcceptSocket ();
						var id = AcceptClient (client);
						if (id >= 0)
						{
							//clientList.Add (client);
							Netplay.anyClients = true;
							//socketToId[client] = id;
							
							if (Networking.ClientConnection.All.Count > Main.maxNetplayers)
							{
								slots[id].Kick ("Server full, sorry.");
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				ProgramLog.Log (e, "ServerLoop terminated with exception");
			}
			
			Netplay.anyClients = false;
			
			try
			{
				tcpListener.Stop ();
			}
			catch (SocketException) {}
			
			
			lock (Networking.ClientConnection.All)
			foreach (var conn in Networking.ClientConnection.All)
			{
				conn.Kick ("Server is shutting down.");
			}
			
			for (int i = 0; i < 255; i++)
			{
				try
				{
					slots[i].Kick("Server is shutting down.");
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
			
            WorldIO.saveWorld(Program.server.World.SavePath, true);
			
			Statics.serverStarted = false;
		}
		
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
					ProgramLog.Debug.Log ("Accepted socket disconnected");
					return -1;
				}
			}
			catch (Exception e)
			{
				ProgramLog.Debug.Log ("Accepted socket exception ({0})", HandleSocketException (e));
				return -1;
			}
			
			for (int i = 0; i < 255; i++)
			{
				if (slots[i].state == SlotState.VACANT)
				{
					ProgramLog.Users.Log ("{0} is connecting on slot {1}...", addr, i);
					try
					{
						AcceptSlot (i, client, addr);
					}
					catch (SocketException)
					{
						client.SafeClose ();
			
						ProgramLog.Users.Log ("{0} disconnected.", addr);
						
						return -1;
					}
					return i;
				}
			}
			
			client.SafeClose ();

			ProgramLog.Users.Log ("{0} dropped, no slots left.", addr);
			
			return -1;
		}
		
		static void AcceptSlot (int id, Socket client, string remoteAddress)
		{
			var slot = slots[id];
			slot.remoteAddress = remoteAddress;
			Main.players[id].IPAddress = remoteAddress;
			slot.conn = new Networking.ClientConnection (client);
			slot.conn.SlotIndex = id;
			slot.state = SlotState.CONNECTED;
			//slot.socket = client;
			if (slot.readBuffer == null) slot.readBuffer = new byte[1024];
			if (NetMessage.buffer[id].readBuffer == null) NetMessage.buffer[id].readBuffer = new byte [MessageBuffer.BUFFER_MAX];
			ProgramLog.Debug.Log ("Slot {1} assigned to {0}.", remoteAddress, id);
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
				ProgramLog.Debug.Log ("{0} @ {2}: socket exception ({1})", slot.remoteAddress, HandleSocketException (e), id);
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
				ProgramLog.Users.Log ("{0} @ {1}: connection closed.", slot.remoteAddress, id);
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
					ProgramLog.Users.Log ("{0} @ {1}: connection closed", addr, id);
				else
					ProgramLog.Users.Log ("{0}: connection closed", addr);
			}
			catch (Exception e)
			{
				HandleSocketException (e);
				if (id >= 0)
					ProgramLog.Users.Log ("{0} @ {1}: connection closed", addr, id);
				else
					ProgramLog.Users.Log ("{0}: connection closed", addr);
			}
			
			socket.SafeClose ();
		}
		
		static void DisposeClient (int id)
		{
			ProgramLog.Debug.Log ("Freeing slot {0}", id);
			
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
				return e.Message;
			if (e is System.IO.IOException)
				return e.Message;
			else if (e is ObjectDisposedException)
				return "Socket already disposed";
			else
				throw new Exception ("Unexpected exception in socket handling code", e);
		}
		
		private static ProgramThread serverThread;
		
		public static void StartServer()
		{
			if (serverThread == null)
			{
				serverThread = new ProgramThread ("Serv", Netplay.ServerLoopLoop);
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
			for (int i = 0; i < 256; i++)
			{
				if (NetMessage.buffer[i] == null)
					NetMessage.buffer[i] = new MessageBuffer();
					
				NetMessage.buffer[i].whoAmI = i;
				//NetMessage.buffer[i].Reset (); // slot reset calls that anyway

				if (Netplay.slots[i] == null)
					Netplay.slots[i] = new ServerSlot();
				
				Netplay.slots[i].whoAmI = i;
				Netplay.slots[i].Reset ();
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
