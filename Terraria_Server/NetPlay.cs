using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using System.Diagnostics;

using Terraria_Server.Messages;
using Terraria_Server.Logging;
using Terraria_Server.WorldMod;
using Terraria_Server.Networking;

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
        //internal static Queue<Socket> deadClients;
		
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
			
			SlotManager.Initialize (Main.maxNetplayers);
			
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
						var accepted = AcceptClient (client);
						if (accepted)
						{
							Netplay.anyClients = true;
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
			{
				var conns = Networking.ClientConnection.All.ToArray();

				foreach (var conn in conns)
				{
					conn.Kick ("Server is shutting down.");
				}
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
			
			if (false == WorldIO.saveWorld(Program.server.World.SavePath, true))
			{
				WorldIO.saveWorld(Program.server.World.SavePath, true);
				ProgramLog.Error.Log("Saving failed.  Quitting without saving.");
			}
			
			Statics.serverStarted = false;
		}
		
		static bool AcceptClient (Socket client)
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
					return false;
				}
			}
			catch (Exception e)
			{
				ProgramLog.Debug.Log ("Accepted socket exception ({0})", HandleSocketException (e));
				return false;
			}
			
			try
			{
				var conn = new Networking.ClientConnection (client, -1);
				conn.State = SlotState.CONNECTED;
			}
			catch (SocketException)
			{
				client.SafeClose ();
				ProgramLog.Users.Log ("{0} disconnected.", addr);
			}
			
			return true;
			
//			for (int i = 0; i < 255; i++)
//			{
//				if (slots[i].state == SlotState.VACANT)
//				{
//					ProgramLog.Users.Log ("{0} is connecting on slot {1}...", addr, i);
//					try
//					{
//						AcceptSlot (i, client, addr);
//					}
//					catch (SocketException)
//					{
//						client.SafeClose ();
//			
//						ProgramLog.Users.Log ("{0} disconnected.", addr);
//						
//						return -1;
//					}
//					return i;
//				}
//			}
			
//			client.SafeClose ();
//
//			ProgramLog.Users.Log ("{0} dropped, no slots left.", addr);
//			
//			return -1;
		}
		
		static void AcceptSlot (int id, Socket client, string remoteAddress)
		{
			var slot = slots[id];
			slot.remoteAddress = remoteAddress;
			Main.players[id].IPAddress = remoteAddress;
			slot.conn = new Networking.ClientConnection (client, id);
			slot.state = SlotState.CONNECTED;
			//slot.socket = client;
			//if (slot.readBuffer == null) slot.readBuffer = new byte[1024];
			//if (NetMessage.buffer[id].readBuffer == null) NetMessage.buffer[id].readBuffer = new byte [MessageBuffer.BUFFER_MAX];
			ProgramLog.Debug.Log ("Slot {1} assigned to {0}.", remoteAddress, id);
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
					
				//NetMessage.buffer[i].whoAmI = i;
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
