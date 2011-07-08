using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Threading;
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
        
        private volatile Queue<byte[]> writeQueue;
        private Thread         writeThread;
        private AutoResetEvent writeSignal;
        
        public ServerSock ()
        {
            writeQueue = new Queue<byte[]> ();
            writeSignal = new AutoResetEvent (false);
        }
        
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
            this.writeQueue = new Queue<byte[]> ();

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
        
        public void Send (byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentException ("Data to send cannot be null");
            }
        
            if (writeThread == null)
            {
                writeThread = new Thread (this.WriteThread);
                writeThread.IsBackground = true;
                writeThread.Start ();
            }
            
            lock (writeQueue)
            {
                writeQueue.Enqueue (data);
            }
            writeSignal.Set ();
        }
        
        const int WRITE_THREAD_BATCH_SIZE = 32;
        internal void WriteThread ()
        {
            byte[][] list = new byte[WRITE_THREAD_BATCH_SIZE][];
            while (true)
            {
                try
                {
                    int items = 0;

                    lock (writeQueue)
                    {
                        while (writeQueue.Count > 0)
                        {
                            list[items++] = writeQueue.Dequeue();
                            if (items == WRITE_THREAD_BATCH_SIZE) break;
                        }
                    }

                    if (items == 0)
                    {
                        writeSignal.WaitOne();
                        continue;
                    }

                    try
                    {
                        for (int i = 0; i < items; i++)
                        {
                            networkStream.Write(list[i], 0, list[i].Length);
                            list[i] = null;
                            NetMessage.buffer[this.whoAmI].spamCount--;
                            if (this.statusMax > 0)
                            {
                                this.statusCount++;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                catch (Exception e)
                {
                    Program.tConsole.WriteLine("Exception within WriteThread:Socket");
                    Program.tConsole.WriteLine(e.Message);
                }
            }
        }
    }
}
