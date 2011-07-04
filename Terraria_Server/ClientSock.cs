using System;
using System.Net.Sockets;

namespace Terraria_Server
{
	public class ClientSock
	{
		public TcpClient tcpClient = new TcpClient();
		public NetworkStream networkStream;
		public String statusText;
		public int statusCount;
		public int statusMax;
		public int timeOut;
		public byte[] readBuffer;
		public byte[] writeBuffer;
		public bool active;
		public bool locked;
		public int state;
		public void ClientWriteCallBack(IAsyncResult ar)
		{
			NetMessage.buffer[256].spamCount--;
		}
		public void ClientReadCallBack(IAsyncResult ar)
		{
			if (!Netplay.disconnect)
			{
				int num = this.networkStream.EndRead(ar);
				if (num == 0)
				{
					Netplay.disconnect = true;
                    Program.printData("Lost connection");
				}
				else
				{
					if (Main.ignoreErrors)
					{
						try
						{
							NetMessage.RecieveBytes(this.readBuffer, num, 256);
							goto IL_59;
						}
						catch
						{
							goto IL_59;
						}
					}
					NetMessage.RecieveBytes(this.readBuffer, num, 256);
				}
			}
			IL_59:
			this.locked = false;
		}
	}
}
