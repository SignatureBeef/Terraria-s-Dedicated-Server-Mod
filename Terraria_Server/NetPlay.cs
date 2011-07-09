
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
                Program.tConsole.WriteLine("Server started on " + serverSIP + ":" + serverPort.ToString());
                Program.tConsole.WriteLine("Loading Plugins...");
                Program.server.getPluginManager().LoadPlugins();
                Program.tConsole.WriteLine("Plugins Loaded: " + Program.server.getPluginManager().getPluginList().Count.ToString());
                Statics.serverStarted = true;
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
