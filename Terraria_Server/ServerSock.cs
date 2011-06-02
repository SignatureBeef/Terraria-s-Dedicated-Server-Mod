using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    using System;
    using System.Net.Sockets;

    public class ServerSock
    {
        public bool active;
        public bool announced;
        public Socket clientSocket;
        public bool kill;
        public bool locked;
        public string name = "Anonymous";
        public NetworkStream networkStream;
        public string oldName = "";
        public byte[] readBuffer;
        public int state;
        public int statusCount;
        public int statusMax;
        public string statusText = "";
        public string statusText2;
        public TcpClient tcpClient = new TcpClient();
        public bool[,] tileSection = new bool[Statics.maxTilesX / 200, Statics.maxTilesY / 150];
        public int timeOut;
        public int whoAmI;
        public byte[] writeBuffer;

        public World world = null;

        public ServerSock(World World)
        {
            world = World;
        }
        
        public void Reset()
        {
            for (int i = 0; i < Statics.maxSectionsX; i++)
            {
                for (int j = 0; j < Statics.maxSectionsY; j++)
                {
                    this.tileSection[i, j] = false;
                }
            }
            if (this.whoAmI < 8)
            {
                world.getPlayerList()[this.whoAmI] = new Player(world);
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
        
        public void ServerReadCallBack(IAsyncResult ar)
        {
            int num = 0;
            if (!world.getServer().getNetPlay().disconnect)
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
                    if (Statics.ignoreErrors)
                    {
                        try
                        {
                            NetMessage.RecieveBytes(this.readBuffer, num, world, this.whoAmI);
                            goto IL_77;
                        }
                        catch
                        {
                            goto IL_77;
                        }
                    }
                    NetMessage.RecieveBytes(this.readBuffer, num, world, this.whoAmI);
                }
            }
        IL_77:
            this.locked = false;
        }
       
        public void ServerWriteCallBack(IAsyncResult ar)
        {
            messageBuffer messageBuffer = NetMessage.buffer[this.whoAmI];
            messageBuffer.spamCount--;
            if (this.statusMax > 0)
            {
                this.statusCount++;
            }
        }
    }
}
