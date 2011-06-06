using System;
using System.Net.Sockets;
namespace Terraria_Server
{
	public class ServerSock
	{
		public Socket clientSocket;
		public NetworkStream networkStream;
		public TcpClient tcpClient = new TcpClient();
		public int whoAmI;
		public string statusText2;
		public int statusCount;
		public int statusMax;
		public bool[,] tileSection = new bool[Main.maxTilesX / 200, Main.maxTilesY / 150];
		public string statusText = "";
		public bool active;
		public bool locked;
		public bool kill;
		public int timeOut;
		public bool announced;
		public string name = "Anonymous";
		public string oldName = "";
		public int state;
		public byte[] readBuffer;
		public byte[] writeBuffer;
		public void Reset()
		{
			for (int i = 0; i < Main.maxSectionsX; i++)
			{
				for (int j = 0; j < Main.maxSectionsY; j++)
				{
					this.tileSection[i, j] = false;
				}
			}
			if (this.whoAmI < 255)
			{
				Main.player[this.whoAmI] = new Player();
			}
			this.timeOut = 0;
			this.statusCount = 0;
			this.statusMax = 0;
			this.statusText2 = "";
			this.statusText = "";
			this.name = "Anonymous";
			this.state = 0;
			this.locked = false;
			this.kill = false;
			this.active = false;
			NetMessage.buffer[this.whoAmI].Reset();
			if (this.networkStream != null)
			{
				this.networkStream.Close();
			}
			if (this.tcpClient != null)
			{
				this.tcpClient.Close();
			}
		}
		public void ServerWriteCallBack(IAsyncResult ar)
		{
			NetMessage.buffer[this.whoAmI].spamCount--;
			if (this.statusMax > 0)
			{
				this.statusCount++;
			}
		}
		public void ServerReadCallBack(IAsyncResult ar)
		{
			int num = 0;
			if (!NetPlay.disconnect)
			{
				try
				{
					num = this.networkStream.EndRead(ar);
				}
				catch
				{
				}
				if (num == 0)
				{
					this.kill = true;
				}
				else
				{
					if (Main.ignoreErrors)
					{
						try
						{
							NetMessage.RecieveBytes(this.readBuffer, num, this.whoAmI);
							goto IL_57;
						}
						catch
						{
							goto IL_57;
						}
					}
					NetMessage.RecieveBytes(this.readBuffer, num, this.whoAmI);
				}
			}
			IL_57:
			this.locked = false;
		}
	}
}
