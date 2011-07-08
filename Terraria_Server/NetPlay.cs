
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using Terraria_Server.Messages;
using System.Diagnostics;
namespace Terraria_Server
{
    public class Netplay
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
        public static String serverSIP = "0.0.0.0";
		public static bool disconnect = false;
        public static String password = "";
        public static bool spamCheck = false;
        public static bool ServerUp = false;
        public static bool anyClients = false;
        		
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
			Main.players[Main.myPlayer].hostile = false;
			Main.clientPlayer = (Player)Main.players[Main.myPlayer].clientClone();
			Main.menuMode = 10;
			Main.menuMode = 14;
			if (!Main.autoPass)
			{
				Main.statusText = "Connecting to " + Netplay.serverIP;
			}
			Main.netMode = 1;
			Netplay.disconnect = false;
			Netplay.clientSock = new ClientSock();
			Netplay.clientSock.tcpClient.NoDelay = true;
			Netplay.clientSock.readBuffer = new byte[1024];
			Netplay.clientSock.writeBuffer = new byte[1024];
			bool flag = true;
			while (flag)
			{
				flag = false;
				try
				{
					Netplay.clientSock.tcpClient.Connect(Netplay.serverIP, Netplay.serverPort);
					Netplay.clientSock.networkStream = Netplay.clientSock.tcpClient.GetStream();
					flag = false;
				}
				catch
				{
					if (!Netplay.disconnect && Main.gameMenu)
					{
						flag = true;
					}
				}
			}
			NetMessage.buffer[256].Reset();
			int num = -1;
			while (!Netplay.disconnect)
			{
				if (Netplay.clientSock.tcpClient.Connected)
				{
					if (NetMessage.buffer[256].checkBytes)
					{
						NetMessage.CheckBytes(256);
					}
					Netplay.clientSock.active = true;
					if (Netplay.clientSock.state == 0)
					{
						Main.statusText = "Found server";
						Netplay.clientSock.state = 1;
						NetMessage.SendData(1);
					}
					if (Netplay.clientSock.state == 2 && num != Netplay.clientSock.state)
					{
						Main.statusText = "Sending player data...";
					}
					if (Netplay.clientSock.state == 3 && num != Netplay.clientSock.state)
					{
						Main.statusText = "Requesting world information";
					}
					if (Netplay.clientSock.state == 4)
					{
						WorldGen.worldCleared = false;
						Netplay.clientSock.state = 5;
						WorldGen.clearWorld();
					}
					if (Netplay.clientSock.state == 5 && WorldGen.worldCleared)
					{
						Netplay.clientSock.state = 6;
						Main.players[Main.myPlayer].FindSpawn();
						NetMessage.SendData(8, -1, -1, "", Main.players[Main.myPlayer].SpawnX, (float)Main.players[Main.myPlayer].SpawnY);
					}
					if (Netplay.clientSock.state == 6 && num != Netplay.clientSock.state)
					{
						Main.statusText = "Requesting tile data";
					}
					if (!Netplay.clientSock.locked && !Netplay.disconnect && Netplay.clientSock.networkStream.DataAvailable)
					{
						Netplay.clientSock.locked = true;
						Netplay.clientSock.networkStream.BeginRead(Netplay.clientSock.readBuffer, 0, Netplay.clientSock.readBuffer.Length, new AsyncCallback(Netplay.clientSock.ClientReadCallBack), Netplay.clientSock.networkStream);
					}
					if (Netplay.clientSock.statusMax > 0 && Netplay.clientSock.statusText != "")
					{
						if (Netplay.clientSock.statusCount >= Netplay.clientSock.statusMax)
						{
							Main.statusText = Netplay.clientSock.statusText + ": Complete!";
							Netplay.clientSock.statusText = "";
							Netplay.clientSock.statusMax = 0;
							Netplay.clientSock.statusCount = 0;
						}
						else
						{
							Main.statusText = String.Concat(new object[]
							{
								Netplay.clientSock.statusText, 
								": ", 
								(int)((float)Netplay.clientSock.statusCount / (float)Netplay.clientSock.statusMax * 100f), 
								"%"
							});
						}
					}
					Thread.Sleep(1);
				}
				else
				{
					if (Netplay.clientSock.active)
					{
						Main.statusText = "Lost connection";
						Netplay.disconnect = true;
					}
				}
				num = Netplay.clientSock.state;
			}
			try
			{
				Netplay.clientSock.networkStream.Close();
				Netplay.clientSock.networkStream = Netplay.clientSock.tcpClient.GetStream();
			}
			catch
			{
			}
			if (!Main.gameMenu)
			{
				Main.netMode = 0;
				Player.SavePlayer(Main.players[Main.myPlayer]);
				Main.gameMenu = true;
				Main.menuMode = 14;
			}
			NetMessage.buffer[256].Reset();
			if (Main.menuMode == 15 && Main.statusText == "Lost connection")
			{
				Main.menuMode = 14;
			}
			if (Netplay.clientSock.statusText != "" && Netplay.clientSock.statusText != null)
			{
				Main.statusText = "Lost connection";
			}
			Netplay.clientSock.statusCount = 0;
			Netplay.clientSock.statusMax = 0;
			Netplay.clientSock.statusText = "";
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
			Netplay.serverIP = IPAddress.Parse(serverSIP);
			Netplay.serverListenIP = Netplay.serverIP;
			Main.netMode = 2;
			Netplay.disconnect = false;
			for (int i = 0; i < 256; i++)
			{
				Netplay.serverSock[i] = new ServerSock();
				Netplay.serverSock[i].Reset();
				Netplay.serverSock[i].whoAmI = i;
				Netplay.serverSock[i].tcpClient = new TcpClient();
				Netplay.serverSock[i].tcpClient.NoDelay = true;
				Netplay.serverSock[i].readBuffer = new byte[1024];
				Netplay.serverSock[i].writeBuffer = new byte[1024];
			}
			Netplay.tcpListener = new TcpListener(Netplay.serverListenIP, Netplay.serverPort);
			try
			{
				Netplay.tcpListener.Start();
			}
			catch (Exception arg_11F_0)
			{
				Exception exception = arg_11F_0;
				Main.menuMode = 15;
				Main.statusText = exception.ToString();
				Netplay.disconnect = true;
			}
			if (!Netplay.disconnect)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(Netplay.ListenForClients), 1);
                Program.updateThread.Start();
                Statics.serverStarted = true;
                Program.tConsole.WriteLine("Server started on " + serverSIP + ":" + serverPort.ToString());
                Program.tConsole.WriteLine("Loading Plugins...");
                Program.server.getPluginManager().ReloadPlugins();
			}
			while (!Netplay.disconnect)
			{
				if (Netplay.stopListen)
				{
					int num = -1;
					for (int j = 0; j < 255; j++)
					{
						if (!Netplay.serverSock[j].tcpClient.Connected)
						{
							num = j;
							break;
						}
					}
					if (num >= 0)
					{
                        Netplay.tcpListener.Start();
                        Netplay.stopListen = false;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(Netplay.ListenForClients), 1);
					}
				}
				int num2 = 0;
				for (int k = 0; k < 256; k++)
				{
					if (NetMessage.buffer[k].checkBytes)
					{
						NetMessage.CheckBytes(k);
					}
					if (Netplay.serverSock[k].kill)
					{
						Netplay.serverSock[k].Reset();
						NetMessage.syncPlayers();
					}
					else
					{
                        try
                        {
                            if (Netplay.serverSock[k].tcpClient.Connected)
                            {
                                if (!Netplay.serverSock[k].active)
                                {
                                    Netplay.serverSock[k].state = 0;
                                }
                                Netplay.serverSock[k].active = true;
                                num2++;
                                if (!Netplay.serverSock[k].locked)
                                {
                                    try
                                    {
                                        Netplay.serverSock[k].networkStream = Netplay.serverSock[k].tcpClient.GetStream();
                                        if (Netplay.serverSock[k].networkStream.DataAvailable)
                                        {
                                            Netplay.serverSock[k].locked = true;
                                            //if (Statics.debugMode)
                                            //{
                                                //Netplay.serverSock[k].networkStream.Read(Netplay.serverSock[k].readBuffer, 0, Netplay.serverSock[k].readBuffer.Length);
                                                //NetMessage.RecieveBytes(Netplay.serverSock[k].readBuffer, Netplay.serverSock[k].readBuffer.Length, Netplay.serverSock[k].whoAmI);
                                            //}
                                            //else
                                            //{
                                                Netplay.serverSock[k].networkStream.BeginRead(Netplay.serverSock[k].readBuffer, 0, Netplay.serverSock[k].readBuffer.Length, new AsyncCallback(Netplay.serverSock[k].ServerReadCallBack), Netplay.serverSock[k].networkStream);
                                            //}
                                         }
                                    }
                                    catch
                                    {
                                        Netplay.serverSock[k].kill = true;
                                    }
                                }
                                if (Netplay.serverSock[k].statusMax > 0 && Netplay.serverSock[k].statusText2 != "")
                                {
                                    if (Netplay.serverSock[k].statusCount >= Netplay.serverSock[k].statusMax)
                                    {
                                        Netplay.serverSock[k].statusText = String.Concat(new object[]
									{
										"(", 
										Netplay.serverSock[k].tcpClient.Client.RemoteEndPoint, 
										") ", 
										Netplay.serverSock[k].name, 
										" ", 
										Netplay.serverSock[k].statusText2, 
										": Complete!"
									});
                                        Netplay.serverSock[k].statusText2 = "";
                                        Netplay.serverSock[k].statusMax = 0;
                                        Netplay.serverSock[k].statusCount = 0;
                                    }
                                    else
                                    {
                                        Netplay.serverSock[k].statusText = String.Concat(new object[]
									{
										"(", 
										Netplay.serverSock[k].tcpClient.Client.RemoteEndPoint, 
										") ", 
										Netplay.serverSock[k].name, 
										" ", 
										Netplay.serverSock[k].statusText2, 
										": ", 
										(int)((float)Netplay.serverSock[k].statusCount / (float)Netplay.serverSock[k].statusMax * 100f), 
										"%"
									});
                                    }
                                }
                                else
                                {
                                    if (Netplay.serverSock[k].state == 0)
                                    {
                                        Netplay.serverSock[k].statusText = String.Concat(new object[]
									{
										"(", 
										Netplay.serverSock[k].tcpClient.Client.RemoteEndPoint, 
										") ", 
										Netplay.serverSock[k].name, 
										" is connecting..."
									});

                                    }
                                    else
                                    {
                                        if (Netplay.serverSock[k].state == 1)
                                        {
                                            Netplay.serverSock[k].statusText = String.Concat(new object[]
										{
											"(", 
											Netplay.serverSock[k].tcpClient.Client.RemoteEndPoint, 
											") ", 
											Netplay.serverSock[k].name, 
											" is sending player data..."
										});
                                        }
                                        else
                                        {
                                            if (Netplay.serverSock[k].state == 2)
                                            {
                                                Netplay.serverSock[k].statusText = String.Concat(new object[]
											{
												"(", 
												Netplay.serverSock[k].tcpClient.Client.RemoteEndPoint, 
												") ", 
												Netplay.serverSock[k].name, 
												" requested world information"
											});
                                            }
                                            else
                                            {
                                                if (Netplay.serverSock[k].state != 3 && Netplay.serverSock[k].state == 10)
                                                {
                                                    Netplay.serverSock[k].statusText = String.Concat(new object[]
												{
													"(", 
													Netplay.serverSock[k].tcpClient.Client.RemoteEndPoint, 
													") ", 
													Netplay.serverSock[k].name, 
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
                                if (Netplay.serverSock[k].active)
                                {
                                    Netplay.serverSock[k].kill = true;
                                }
                                else
                                {
                                    Netplay.serverSock[k].statusText2 = "";
                                    if (k < 255)
                                    {
                                        Main.players[k].Active = false;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            try
                            {
                                Netplay.serverSock[k].tcpClient.Client.Close();
                            }
                            catch (Exception)
                            {

                            }
                            try
                            {
                                Netplay.serverSock[k].tcpClient.Close();
                            }
                            catch (Exception)
                            {

                            }
                        }
					}
				}
				Thread.Sleep(10);
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
                if (num2 == 0)
                {
                    Netplay.anyClients = false;
                }
                else
                {
                    Netplay.anyClients = true;
                }
				Netplay.ServerUp = true;
			}
			Netplay.tcpListener.Stop();
			for (int l = 0; l < 256; l++)
			{
				Netplay.serverSock[l].Reset();
			}
			if (Main.menuMode != 15)
			{
				Main.netMode = 0;
				Main.menuMode = 10;
                WorldGen.saveWorld(Program.server.getWorld().SavePath, false);
                while (WorldGen.saveLock)
                {
                }

                Statics.serverStarted = false;
                Statics.IsActive = Statics.keepRunning; //To keep console active & program alive upon restart;
			}
			Main.myPlayer = 0;
		}
		
        public static void ListenForClients(object threadContext)
		{
			while (!Netplay.disconnect && !Netplay.stopListen)
			{
				int num = -1;
				for (int i = 0; i < Main.maxNetplayers; i++)
				{
					if (!Netplay.serverSock[i].tcpClient.Connected)
					{
						num = i;
						break;
					}
				}
				if (num >= 0)
				{
					try
					{
						Netplay.serverSock[num].tcpClient = Netplay.tcpListener.AcceptTcpClient();
						Netplay.serverSock[num].tcpClient.NoDelay = true;
						Program.tConsole.WriteLine(Netplay.serverSock[num].tcpClient.Client.RemoteEndPoint + " is connecting...");
                        Main.players[num].setIPAddress(Netplay.serverSock[num].tcpClient.Client.RemoteEndPoint.ToString());
						continue;
					}
					catch (Exception arg_81_0)
					{
						if (!Netplay.disconnect)
						{
                            Main.menuMode = 15;
                            Program.tConsole.WriteLine("Netplay Exception:");
                            Program.tConsole.WriteLine(arg_81_0.ToString());
							Netplay.disconnect = true;
						}
						continue;
					}
				}
				Netplay.stopListen = true;
				Netplay.tcpListener.Stop();
			}
		}
		
        public static void StartClient()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(Netplay.ClientLoop), 1);
		}
		
        public static void StartServer()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(Netplay.ServerLoop), 1);
		}

        public static void StopServer()
        {
            Statics.IsActive = Statics.keepRunning; //To keep console active & program alive upon restart;
            Program.tConsole.WriteLine("Disabling Plugins");
            Program.server.getPluginManager().DisablePlugins();
            Program.tConsole.WriteLine("Closing Connections...");
            disconnect = true;
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
					Netplay.serverSock[i] = new ServerSock();
					Netplay.serverSock[i].tcpClient.NoDelay = true;
				}
				NetMessage.buffer[i] = new MessageBuffer();
				NetMessage.buffer[i].whoAmI = i;
			}
			Netplay.clientSock.tcpClient.NoDelay = true;
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
