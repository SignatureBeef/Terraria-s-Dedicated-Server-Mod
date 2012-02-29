using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using System.Diagnostics;

using Terraria_Server.Plugins;
using Terraria_Server.Messages;
using Terraria_Server.Logging;
using Terraria_Server.WorldMod;
using Terraria_Server.Networking;

namespace Terraria_Server
{
	public static class NetPlay
	{
		public const int bufferSize = 1024;
		public const int maxConnections = 256;
		public static bool stopListen = false;
		public static ServerSlot[] slots = new ServerSlot[256];
		public static TcpListener tcpListener;
		public static IPAddress serverListenIP;
		public static IPAddress serverIP;
		public static int serverPort = 7777;
		public static string serverSIP = "0.0.0.0";
		public static bool disconnect = false;
		public static string password = "";
		public static bool spamCheck = false;
		public static bool ServerUp = false;
		public static bool anyClients = false;
		//internal static Queue<Socket> deadClients;

		public static void SafeClose(this Socket socket)
		{
			if (socket == null) return;

			try
			{
				socket.Close();
			}
			catch (SocketException) { }
			catch (ObjectDisposedException) { }
		}

		public static void SafeShutdown(this Socket socket)
		{
			if (socket == null) return;

			try
			{
				socket.Shutdown(SocketShutdown.Both);
			}
			catch (SocketException) { }
			catch (ObjectDisposedException) { }
		}

		public static void ServerLoop()
		{
			Main.players[255].whoAmi = 255;
			NetPlay.serverIP = IPAddress.Parse(serverSIP);
			NetPlay.serverListenIP = NetPlay.serverIP;
			NetPlay.disconnect = false;

			//			for (int i = 0; i < 256; i++)
			//			{
			////                Netplay.slots[i] = new ServerSlot();
			//                Netplay.slots[i].whoAmI = i;
			//				Netplay.slots[i].Reset();
			//			}
			Init();

			NetPlay.tcpListener = new TcpListener(NetPlay.serverListenIP, NetPlay.serverPort);

			try
			{
				NetPlay.tcpListener.Start();
			}
			catch (Exception exception)
			{
				ProgramLog.Error.Log("Error Starting the Server: {0}", exception);
				NetPlay.disconnect = true;
			}

			if (!NetPlay.disconnect)
			{
				if (!Program.updateThread.IsAlive) Program.updateThread.Start();
				ProgramLog.Admin.Log("{0} {1}:{2}", Language.Languages.ServerStartedOn, serverSIP, serverPort);
				//                ProgramLog.Log("Loading Plugins...");
				//				PluginManager.LoadAllPlugins();
				//                ProgramLog.Log("Plugins Loaded: " + PluginManager.Plugins.Count.ToString());
				//Statics.serverStarted = true;
			}
			else
				return;

			SlotManager.Initialize(Program.properties.MaxPlayers, Program.properties.OverlimitSlots);

			ServerUp = true;
			var serverSock = NetPlay.tcpListener.Server;

			try
			{
				while (!NetPlay.disconnect)
				{
					NetPlay.anyClients = Networking.ClientConnection.All.Count > 0; //clientList.Count > 0;

					serverSock.Poll(500000, SelectMode.SelectRead);

					if (NetPlay.disconnect) break;

					// Accept new clients
					while (NetPlay.tcpListener.Pending())
					{
						var client = NetPlay.tcpListener.AcceptSocket();
						var accepted = AcceptClient(client);
						if (accepted)
						{
							NetPlay.anyClients = true;
						}
					}
				}
			}
			catch (Exception e)
			{
				ProgramLog.Log(e, "ServerLoop terminated with exception");
			}

			NetPlay.anyClients = false;

			try
			{
				tcpListener.Stop();
			}
			catch (SocketException) { }



			lock (Networking.ClientConnection.All)
			{
				var conns = Networking.ClientConnection.All.ToArray();

				foreach (var conn in conns)
				{
					conn.Kick("Server is shutting down.");
				}
			}

			for (int i = 0; i < 255; i++)
			{
				try
				{
					slots[i].Kick("Server is shutting down.");
				}
				catch { }
			}

			Thread.Sleep(1000);

			for (int i = 0; i < 255; i++)
			{
				try
				{
					slots[i].Reset();
				}
				catch { }
			}

			if (!WorldIO.SaveWorld(World.SavePath, true))
				ProgramLog.Error.Log("Saving failed.  Quitting without saving.");

			ServerUp = false;
		}

		static bool AcceptClient(Socket client)
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
					ProgramLog.Debug.Log("Accepted socket disconnected");
					return false;
				}
			}
			catch (Exception e)
			{
				ProgramLog.Debug.Log("Accepted socket exception ({0})", HandleSocketException(e));
				return false;
			}

			try
			{
				ProgramLog.Users.Log("{0} is connecting...", addr);
				var conn = new Networking.ClientConnection(client, -1); //ignore the warning
			}
			catch (SocketException)
			{
				client.SafeClose();
				ProgramLog.Users.Log("{0} disconnected.", addr);
			}

			return true;
		}

		static string HandleSocketException(Exception e)
		{
			if (e is SocketException)
				return e.Message;
			if (e is System.IO.IOException)
				return e.Message;
			else if (e is ObjectDisposedException)
				return "Socket already disposed";
			else
				throw new Exception("Unexpected exception in socket handling code", e);
		}

		private static ProgramThread serverThread;

		public static void StartServer()
		{
			var ctx = new HookContext
			{
				Sender = new ConsoleSender(),
			};

			var args = new HookArgs.ServerStateChange
			{
				ServerChangeState = ServerState.STARTING
			};

			HookPoints.ServerStateChange.Invoke(ref ctx, ref args);

			if (serverThread == null)
			{
				serverThread = new ProgramThread("Serv", NetPlay.ServerLoopLoop);
				serverThread.Start();
			}
			disconnect = false;
		}

		public static void StopServer()
		{
			var ctx = new HookContext
			{
				Sender = new ConsoleSender(),
			};

			var args = new HookArgs.ServerStateChange
			{
				ServerChangeState = ServerState.STOPPING
			};

			HookPoints.ServerStateChange.Invoke(ref ctx, ref args);

			//Statics.IsActive = Statics.keepRunning; //To keep console active & program alive upon restart;
			ProgramLog.Log("Disabling Plugins");
			PluginManager.DisablePlugins();
			ProgramLog.Log("Closing Connections...");
			disconnect = true;
		}

		private static void ServerLoopLoop()
		{
			while (true)
			{
				ServerLoop();
				while (disconnect) Thread.Sleep(100);
			}
		}

		public static void Init()
		{
			for (int i = 0; i < 256; i++)
			{
				if (NetPlay.slots[i] == null)
					NetPlay.slots[i] = new ServerSlot();

				NetPlay.slots[i].whoAmI = i;
				NetPlay.slots[i].Reset();
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
		public static void ResetSections()
		{
			for (int i = 0; i < 256; i++)
			{
				for (int j = 0; j < Main.maxSectionsX; j++)
				{
					for (int k = 0; k < Main.maxSectionsY; k++)
						slots[i].tileSection[j, k] = false;
				}
			}
		}
	}
}
