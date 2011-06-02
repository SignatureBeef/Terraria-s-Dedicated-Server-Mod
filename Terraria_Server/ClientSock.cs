using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Terraria_Server
{
    public class ClientSock
    {
        public bool active;
        public bool locked;
        public NetworkStream networkStream;
        public byte[] readBuffer;
        public int state;
        public int statusCount;
        public int statusMax;
        public string statusText;
        public TcpClient tcpClient = new TcpClient();
        public int timeOut;
        public byte[] writeBuffer;

        public World world = null;

        public ClientSock(World World)
        {
            world = World;
        }

        public void ClientReadCallBack(IAsyncResult ar)
        {
            int streamLength = 0;
            if (!world.getServer().getNetPlay().disconnect)
            {
                streamLength = this.networkStream.EndRead(ar);
                if (streamLength == 0)
                {
                    world.getServer().getNetPlay().disconnect = true;
                    Console.WriteLine("Lost connection");
                }
                else if (Statics.ignoreErrors)
                {
                    try
                    {
                        NetMessage.RecieveBytes(this.readBuffer, streamLength, world, 9);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    NetMessage.RecieveBytes(this.readBuffer, streamLength, world, 9);
                }
            }
            this.locked = false;
        }

        public void ClientWriteCallBack(IAsyncResult ar)
        {
            messageBuffer buffer1 = NetMessage.buffer[9];
            buffer1.spamCount--;
        }
    }
}
