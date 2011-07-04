
using System.Net.Sockets;
using System;
namespace Terraria_Server
{
    public class ServerSock
    {
        public Socket clientSocket;
        public NetworkStream networkStream;
        public TcpClient tcpClient = new TcpClient();
        public int whoAmI;
        public String statusText2;
        public int statusCount;
        public int statusMax;
        public bool[,] tileSection = new bool[Main.maxTilesX / 200, Main.maxTilesY / 150];
        public String statusText = "";
        public bool active;
        public bool locked;
        public bool kill;
        public int timeOut;
        public bool announced;
        public String name = "Anonymous";
        public String oldName = "";
        public int state;
        public float spamProjectile;
        public float spamAddBlock;
        public float spamDelBlock;
        public float spamWater;
        public float spamProjectileMax = 100f;
        public float spamAddBlockMax = 100f;
        public float spamDelBlockMax = 500f;
        public float spamWaterMax = 50f;
        public byte[] readBuffer;
        public byte[] writeBuffer;
        
        public void SpamUpdate()
        {
            if (!Netplay.spamCheck)
            {
                this.spamProjectile = 0f;
                this.spamDelBlock = 0f;
                this.spamAddBlock = 0f;
                this.spamWater = 0f;
                return;
            }
            if (this.spamProjectile > this.spamProjectileMax)
            {
                NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Projectile spam");
            }
            if (this.spamAddBlock > this.spamAddBlockMax)
            {
                NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Add tile spam");
            }
            if (this.spamDelBlock > this.spamDelBlockMax)
            {
                NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Remove tile spam");
            }
            if (this.spamWater > this.spamWaterMax)
            {
                NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Liquid spam");
            }
            this.spamProjectile -= 0.4f;
            if (this.spamProjectile < 0f)
            {
                this.spamProjectile = 0f;
            }
            this.spamAddBlock -= 0.3f;
            if (this.spamAddBlock < 0f)
            {
                this.spamAddBlock = 0f;
            }
            this.spamDelBlock -= 5f;
            if (this.spamDelBlock < 0f)
            {
                this.spamDelBlock = 0f;
            }
            this.spamWater -= 0.2f;
            if (this.spamWater < 0f)
            {
                this.spamWater = 0f;
            }
        }
        
        public void SpamClear()
        {
            this.spamProjectile = 0f;
            this.spamAddBlock = 0f;
            this.spamDelBlock = 0f;
            this.spamWater = 0f;
        }
        
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
                Main.players[this.whoAmI] = new Player();
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
            this.SpamClear();
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
            if (!Netplay.disconnect)
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
