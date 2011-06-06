using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Terraria_Server
{
    public class NetPlay
	{
		public const int bufferSize = 1024;
		public const int maxConnections = 256;
		public static bool stopListen = false;
		public static ServerSock[] serverSock = new ServerSock[256];
		public static ClientSock clientSock = new ClientSock();
		public static TcpListener tcpListener;
		public static IPAddress serverListenIP;
		public static IPAddress serverIP;
		public static int serverPort = 7777;
		public static bool disconnect = false;
		public static string password = "";
		public static bool ServerUp = false;
		
        public static void AddBan(int plr)
		{
			string text = NetPlay.serverSock[plr].tcpClient.Client.RemoteEndPoint.ToString();
			string value = text;
			for (int i = 0; i < text.Length; i++)
			{
				if (text.Substring(i, 1) == ":")
				{
					value = text.Substring(0, i);
				}
			}
			using (StreamWriter streamWriter = new StreamWriter("banlist.txt", true))
			{
				streamWriter.WriteLine("//" + Main.player[plr].name);
				streamWriter.WriteLine(value);
			}
		}
		
        public static bool CheckBan(string ip)
		{
			try
			{
				string b = ip;
				for (int i = 0; i < ip.Length; i++)
				{
					if (ip.Substring(i, 1) == ":")
					{
						b = ip.Substring(0, i);
					}
				}
				if (File.Exists("banlist.txt"))
				{
					using (StreamReader streamReader = new StreamReader("banlist.txt"))
					{
						string a;
						while ((a = streamReader.ReadLine()) != null)
						{
							if (a == b)
							{
								return true;
							}
						}
					}
				}
			}
			catch
			{
			}
			return false;
		}
		
        public static void ClientLoop(object threadContext)
		{
			if (Main.rand == null)
			{
				Main.rand = new Random((int)DateTime.Now.Ticks);
			}
			if (WorldGen.genRand == null)
			{
				WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
			}
			Main.player[Main.myPlayer].hostile = false;
			Main.clientPlayer = (Player)Main.player[Main.myPlayer].clientClone();
			Main.menuMode = 10;
			Main.menuMode = 14;
			if (!Main.autoPass)
			{
				Main.statusText = "Connecting to " + NetPlay.serverIP;
			}
			Main.netMode = 1;
			NetPlay.disconnect = false;
			NetPlay.clientSock = new ClientSock();
			NetPlay.clientSock.tcpClient.NoDelay = true;
			NetPlay.clientSock.readBuffer = new byte[1024];
			NetPlay.clientSock.writeBuffer = new byte[1024];
			bool flag = true;
			while (flag)
			{
				flag = false;
				try
				{
					NetPlay.clientSock.tcpClient.Connect(NetPlay.serverIP, NetPlay.serverPort);
					NetPlay.clientSock.networkStream = NetPlay.clientSock.tcpClient.GetStream();
					flag = false;
				}
				catch
				{
					if (!NetPlay.disconnect && Main.gameMenu)
					{
						flag = true;
					}
				}
			}
			NetMessage.buffer[256].Reset();
			int num = -1;
			while (!NetPlay.disconnect)
			{
				if (NetPlay.clientSock.tcpClient.Connected)
				{
					if (NetMessage.buffer[256].checkBytes)
					{
						NetMessage.CheckBytes(256);
					}
					NetPlay.clientSock.active = true;
					if (NetPlay.clientSock.state == 0)
					{
						Main.statusText = "Found server";
						NetPlay.clientSock.state = 1;
						NetMessage.SendData(1, -1, -1, "", 0, 0f, 0f, 0f);
					}
					if (NetPlay.clientSock.state == 2 && num != NetPlay.clientSock.state)
					{
						Main.statusText = "Sending player data...";
					}
					if (NetPlay.clientSock.state == 3 && num != NetPlay.clientSock.state)
					{
						Main.statusText = "Requesting world information";
					}
					if (NetPlay.clientSock.state == 4)
					{
						WorldGen.worldCleared = false;
						NetPlay.clientSock.state = 5;
						WorldGen.clearWorld();
					}
					if (NetPlay.clientSock.state == 5 && WorldGen.worldCleared)
					{
						NetPlay.clientSock.state = 6;
						Main.player[Main.myPlayer].FindSpawn();
						NetMessage.SendData(8, -1, -1, "", Main.player[Main.myPlayer].SpawnX, (float)Main.player[Main.myPlayer].SpawnY, 0f, 0f);
					}
					if (NetPlay.clientSock.state == 6 && num != NetPlay.clientSock.state)
					{
						Main.statusText = "Requesting tile data";
					}
					if (!NetPlay.clientSock.locked && !NetPlay.disconnect && NetPlay.clientSock.networkStream.DataAvailable)
					{
						NetPlay.clientSock.locked = true;
						NetPlay.clientSock.networkStream.BeginRead(NetPlay.clientSock.readBuffer, 0, NetPlay.clientSock.readBuffer.Length, new AsyncCallback(NetPlay.clientSock.ClientReadCallBack), NetPlay.clientSock.networkStream);
					}
					if (NetPlay.clientSock.statusMax > 0 && NetPlay.clientSock.statusText != "")
					{
						if (NetPlay.clientSock.statusCount >= NetPlay.clientSock.statusMax)
						{
							Main.statusText = NetPlay.clientSock.statusText + ": Complete!";
							NetPlay.clientSock.statusText = "";
							NetPlay.clientSock.statusMax = 0;
							NetPlay.clientSock.statusCount = 0;
						}
						else
						{
							Main.statusText = string.Concat(new object[]
							{
								NetPlay.clientSock.statusText, 
								": ", 
								(int)((float)NetPlay.clientSock.statusCount / (float)NetPlay.clientSock.statusMax * 100f), 
								"%"
							});
						}
					}
					Thread.Sleep(1);
				}
				else
				{
					if (NetPlay.clientSock.active)
					{
						Main.statusText = "Lost connection";
						NetPlay.disconnect = true;
					}
				}
				num = NetPlay.clientSock.state;
			}
			try
			{
				NetPlay.clientSock.networkStream.Close();
				NetPlay.clientSock.networkStream = NetPlay.clientSock.tcpClient.GetStream();
			}
			catch
			{
			}
			if (!Main.gameMenu)
			{
				Main.netMode = 0;
				Player.SavePlayer(Main.player[Main.myPlayer]);
				Main.gameMenu = true;
				Main.menuMode = 14;
			}
			NetMessage.buffer[256].Reset();
			if (Main.menuMode == 15 && Main.statusText == "Lost connection")
			{
				Main.menuMode = 14;
			}
			if (NetPlay.clientSock.statusText != "" && NetPlay.clientSock.statusText != null)
			{
				Main.statusText = "Lost connection";
			}
			NetPlay.clientSock.statusCount = 0;
			NetPlay.clientSock.statusMax = 0;
			NetPlay.clientSock.statusText = "";
			Main.netMode = 0;
		}
		
        public static void ServerLoop(object threadContext)
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
			NetPlay.serverIP = IPAddress.Any;
			NetPlay.serverListenIP = NetPlay.serverIP;
			Main.menuMode = 14;
			Main.statusText = "Starting server...";
			Main.netMode = 2;
			NetPlay.disconnect = false;
			for (int i = 0; i < 256; i++)
			{
				NetPlay.serverSock[i] = new ServerSock();
				NetPlay.serverSock[i].Reset();
				NetPlay.serverSock[i].whoAmI = i;
				NetPlay.serverSock[i].tcpClient = new TcpClient();
				NetPlay.serverSock[i].tcpClient.NoDelay = true;
				NetPlay.serverSock[i].readBuffer = new byte[1024];
				NetPlay.serverSock[i].writeBuffer = new byte[1024];
			}
			NetPlay.tcpListener = new TcpListener(NetPlay.serverListenIP, NetPlay.serverPort);
			try
			{
				NetPlay.tcpListener.Start();
			}
			catch (Exception arg_11F_0)
			{
				Exception exception = arg_11F_0;
				Main.menuMode = 15;
				Main.statusText = exception.ToString();
				NetPlay.disconnect = true;
			}
			if (!NetPlay.disconnect)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(NetPlay.ListenForClients), 1);
                Program.updateThread.Start();
                Statics.serverStarted = true;
                Console.WriteLine("Server started\nLoading Plugins.");
                Program.server.getPluginManager().ReloadPlugins();
			}
			while (!NetPlay.disconnect)
			{
				if (NetPlay.stopListen)
				{
					int num = -1;
					for (int j = 0; j < 255; j++)
					{
						if (!NetPlay.serverSock[j].tcpClient.Connected)
						{
							num = j;
							break;
						}
					}
					if (num >= 0)
					{
                        NetPlay.tcpListener.Start();
                        NetPlay.stopListen = false;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(NetPlay.ListenForClients), 1);
					}
				}
				int num2 = 0;
				for (int k = 0; k < 256; k++)
				{
					if (NetMessage.buffer[k].checkBytes)
					{
						NetMessage.CheckBytes(k);
					}
					if (NetPlay.serverSock[k].kill)
					{
						NetPlay.serverSock[k].Reset();
						NetMessage.syncPlayers();
					}
					else
					{
						if (NetPlay.serverSock[k].tcpClient.Connected)
						{
							if (!NetPlay.serverSock[k].active)
							{
								NetPlay.serverSock[k].state = 0;
							}
							NetPlay.serverSock[k].active = true;
							num2++;
							if (!NetPlay.serverSock[k].locked)
							{
								try
								{
									NetPlay.serverSock[k].networkStream = NetPlay.serverSock[k].tcpClient.GetStream();
									if (NetPlay.serverSock[k].networkStream.DataAvailable)
									{
										NetPlay.serverSock[k].locked = true;
										NetPlay.serverSock[k].networkStream.BeginRead(NetPlay.serverSock[k].readBuffer, 0, NetPlay.serverSock[k].readBuffer.Length, new AsyncCallback(NetPlay.serverSock[k].ServerReadCallBack), NetPlay.serverSock[k].networkStream);
									}
								}
								catch
								{
									NetPlay.serverSock[k].kill = true;
								}
							}
							if (NetPlay.serverSock[k].statusMax > 0 && NetPlay.serverSock[k].statusText2 != "")
							{
								if (NetPlay.serverSock[k].statusCount >= NetPlay.serverSock[k].statusMax)
								{
									NetPlay.serverSock[k].statusText = string.Concat(new object[]
									{
										"(", 
										NetPlay.serverSock[k].tcpClient.Client.RemoteEndPoint, 
										") ", 
										NetPlay.serverSock[k].name, 
										" ", 
										NetPlay.serverSock[k].statusText2, 
										": Complete!"
									});
									NetPlay.serverSock[k].statusText2 = "";
									NetPlay.serverSock[k].statusMax = 0;
									NetPlay.serverSock[k].statusCount = 0;
								}
								else
								{
									NetPlay.serverSock[k].statusText = string.Concat(new object[]
									{
										"(", 
										NetPlay.serverSock[k].tcpClient.Client.RemoteEndPoint, 
										") ", 
										NetPlay.serverSock[k].name, 
										" ", 
										NetPlay.serverSock[k].statusText2, 
										": ", 
										(int)((float)NetPlay.serverSock[k].statusCount / (float)NetPlay.serverSock[k].statusMax * 100f), 
										"%"
									});
								}
							}
							else
							{
								if (NetPlay.serverSock[k].state == 0)
								{
									NetPlay.serverSock[k].statusText = string.Concat(new object[]
									{
										"(", 
										NetPlay.serverSock[k].tcpClient.Client.RemoteEndPoint, 
										") ", 
										NetPlay.serverSock[k].name, 
										" is connecting..."
									});
								}
								else
								{
									if (NetPlay.serverSock[k].state == 1)
									{
										NetPlay.serverSock[k].statusText = string.Concat(new object[]
										{
											"(", 
											NetPlay.serverSock[k].tcpClient.Client.RemoteEndPoint, 
											") ", 
											NetPlay.serverSock[k].name, 
											" is sending player data..."
										});
									}
									else
									{
										if (NetPlay.serverSock[k].state == 2)
										{
											NetPlay.serverSock[k].statusText = string.Concat(new object[]
											{
												"(", 
												NetPlay.serverSock[k].tcpClient.Client.RemoteEndPoint, 
												") ", 
												NetPlay.serverSock[k].name, 
												" requested world information"
											});
										}
										else
										{
											if (NetPlay.serverSock[k].state != 3 && NetPlay.serverSock[k].state == 10)
											{
												NetPlay.serverSock[k].statusText = string.Concat(new object[]
												{
													"(", 
													NetPlay.serverSock[k].tcpClient.Client.RemoteEndPoint, 
													") ", 
													NetPlay.serverSock[k].name, 
													" is playing"
												});
											}
										}
									}
								}
							}
						}
						else
						{
							if (NetPlay.serverSock[k].active)
							{
								NetPlay.serverSock[k].kill = true;
							}
							else
							{
								NetPlay.serverSock[k].statusText2 = "";
								if (k < 255)
								{
									Main.player[k].active = false;
								}
							}
						}
					}
				}
				Thread.Sleep(1);
				if (!WorldGen.saveLock && !Main.dedServ)
				{
					if (num2 == 0)
					{
						Main.statusText = "Waiting for clients...";
					}
					else
					{
						Main.statusText = num2 + " clients connected";
					}
				}
				NetPlay.ServerUp = true;
			}
			NetPlay.tcpListener.Stop();
			for (int l = 0; l < 256; l++)
			{
				NetPlay.serverSock[l].Reset();
			}
			if (Main.menuMode != 15)
			{
				Main.netMode = 0;
				Main.menuMode = 10;
                WorldGen.saveWorld(Program.server.getWorld().getSavePath(), false);
                while (WorldGen.saveLock)
                {
                }
                Statics.serverStarted = false;
			}
			Main.myPlayer = 0;
		}
		
        public static void ListenForClients(object threadContext)
		{
			while (!NetPlay.disconnect && !NetPlay.stopListen)
			{
				int num = -1;
				for (int i = 0; i < Main.maxNetPlayers; i++)
				{
					if (!NetPlay.serverSock[i].tcpClient.Connected)
					{
						num = i;
						break;
					}
				}
				if (num >= 0)
				{
					try
					{
						NetPlay.serverSock[num].tcpClient = NetPlay.tcpListener.AcceptTcpClient();
						NetPlay.serverSock[num].tcpClient.NoDelay = true;
						Console.WriteLine(NetPlay.serverSock[num].tcpClient.Client.RemoteEndPoint + " is connecting...");
						continue;
					}
					catch (Exception arg_81_0)
					{
						Exception exception = arg_81_0;
						if (!NetPlay.disconnect)
						{
							Main.menuMode = 15;
							Main.statusText = exception.ToString();
							NetPlay.disconnect = true;
						}
						continue;
					}
				}
				NetPlay.stopListen = true;
				NetPlay.tcpListener.Stop();
			}
		}
		
        public static void StartClient()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(NetPlay.ClientLoop), 1);
		}
		
        public static void StartServer()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(NetPlay.ServerLoop), 1);
		}

        public static void StopServer()
        {
            Statics.IsActive = false;
            Console.WriteLine("Disabling Plugins");
            Program.server.getPluginManager().DisablePlugins();
            Console.WriteLine("Closing Connections...");
            disconnect = true;
        }

        public static bool SetIP(string newIP)
		{
			bool result;
			try
			{
				NetPlay.serverIP = IPAddress.Parse(newIP);
			}
			catch
			{
				result = false;
				return result;
			}
			return true;
		}
		
        public static bool SetIP2(string newIP)
		{
			bool result;
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(newIP);
				IPAddress[] addressList = hostEntry.AddressList;
				for (int i = 0; i < addressList.Length; i++)
				{
					if (addressList[i].AddressFamily == AddressFamily.InterNetwork)
					{
						NetPlay.serverIP = addressList[i];
						result = true;
						return result;
					}
				}
				result = false;
			}
			catch
			{
				result = false;
			}
			return result;
		}
		
        public static void Init()
		{
			for (int i = 0; i < 257; i++)
			{
				if (i < 256)
				{
					NetPlay.serverSock[i] = new ServerSock();
					NetPlay.serverSock[i].tcpClient.NoDelay = true;
				}
				NetMessage.buffer[i] = new messageBuffer();
				NetMessage.buffer[i].whoAmI = i;
			}
			NetPlay.clientSock.tcpClient.NoDelay = true;
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
