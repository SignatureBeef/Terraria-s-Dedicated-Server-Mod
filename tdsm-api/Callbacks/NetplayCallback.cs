using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Threading;
using tdsm.api.Plugin;

namespace tdsm.api.Callbacks
{
//    public abstract class IAPISocket : Terraria.ServerSock
//    {
////        public int whoAmI;
////        public string statusText2;
////        public int statusCount;
////        public int statusMax;
//        public volatile string remoteAddress;
////        public bool[,] tileSection;
////        public string statusText = String.Empty;
////        public bool announced;
////        public string name = "Anonymous";
////        public string oldName = String.Empty;
////        public float spamProjectile;
////        public float spamAddBlock;
////        public float spamDelBlock;
////        public float spamWater;
////        public float spamProjectileMax = 100f;
////        public float spamAddBlockMax = 100f;
////        public float spamDelBlockMax = 500f;
////        public float spamWaterMax = 50f;
//
////        public int state;
//
////        //TDSM core doesn't use these - only here for vanilla
////        public System.Net.Sockets.TcpClient tcpClient;
////        public System.Net.Sockets.NetworkStream networkStream;
////        public byte[] readBuffer;
////        public bool kill;
////        public bool active;
////        public bool locked;
////        public int timeOut;
////        public byte[] writeBuffer;
//
//        public virtual bool IsPlaying()
//        {
//            return state == 10;
//        }
//        public virtual bool CanSendWater()
//        {
//            //return state >= 3;
//            return (Terraria.NetMessage.buffer[whoAmI].broadcast || state >= 3) && tcpClient.Connected;
//        }
//        public virtual string RemoteAddress()
//        {
//            return tcpClient.Client.RemoteEndPoint.ToString();
//        }
//
//        //Vanilla fallback
////        public virtual void ServerReadCallBack(IAsyncResult ar) { }
////        public virtual void ServerWriteCallBack(IAsyncResult ar) { }
//
////        public abstract void SpamUpdate();
////        public abstract void SpamClear();
////        public abstract void Reset();
////        public abstract bool SectionRange(int size, int firstX, int firstY);
//
//
//        //public virtual void Debug()
//        //{
//        //    Console.WriteLine("API Socket DEBUG");
//        //}
//    }

    public static class SocketExtensions
    {
        public static bool IsPlaying(this Terraria.ServerSock sock)
        {
            return sock.state == 10;
        }

        public static bool CanSendWater(this Terraria.ServerSock sock)
        {
            //return state >= 3;
            return (Terraria.NetMessage.buffer[sock.whoAmI].broadcast || sock.state >= 3) && sock.tcpClient.Connected;
        }

        public static string RemoteAddress(this Terraria.ServerSock sock)
        {
            return sock.tcpClient.Client.RemoteEndPoint.ToString();
        }
    }

    public static class NetplayCallback
    {
//        public static Terraria.ServerSock[] slots;// = new IAPISocket[256];
#if Full_API
        public static Action<Int32, Vector2> CheckSectionMethod = Terraria.ServerSock.CheckSection;
#endif

        public static void CheckSection(int slot, Vector2 position)
        {
#if Full_API
            CheckSectionMethod(slot, position);
#endif
        }

        public static void StartServer(object state)
        {
#if Full_API
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.StartDefaultServer();
            HookPoints.StartDefaultServer.Invoke(ref ctx, ref args);

            if (ctx.Result != HookResult.IGNORE)
            {
                Console.Write("Starting server...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(Terraria.Netplay.ServerLoop), 1);
                Tools.WriteLine("Ok");
            }
#endif
        }

        public static void AddBan(int plr)
        {
#if Full_API
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.AddBan()
            {
                RemoteAddress = Terraria.Netplay.serverSock[plr].RemoteAddress()
            };

            HookPoints.AddBan.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.DEFAULT)
            {
                string remote = Terraria.Netplay.serverSock[plr].RemoteAddress();
                string ip = remote;
                for (int i = 0; i < remote.Length; i++)
                {
                    if (remote.Substring(i, 1) == ":")
                    {
                        ip = remote.Substring(0, i);
                    }
                }
                using (StreamWriter streamWriter = new StreamWriter(Terraria.Netplay.banFile, true))
                {
                    streamWriter.WriteLine("//" + Terraria.Main.player[plr].name);
                    streamWriter.WriteLine(ip);
                }
            }
#endif
        }

        public static void sendWater (int x, int y)
        {
            if (Terraria.Main.netMode == 1)
            {
                Terraria.NetMessage.SendData (48, -1, -1, "", x, (float)y, 0f, 0f, 0);
                return;
            }
            for (int i = 0; i < 256; i++)
            {
                if ((Terraria.NetMessage.buffer [i] != null && 
                     Terraria.NetMessage.buffer [i].broadcast || Terraria.Netplay.serverSock [i].state >= 3) && 
                    Terraria.Netplay.serverSock [i] != null &&
                    Terraria.Netplay.serverSock [i].tcpClient != null && Terraria.Netplay.serverSock [i].tcpClient.Connected)
                {
                    int num = x / 200;
                    int num2 = y / 150;
                    if (Terraria.Netplay.serverSock [i].tileSection [num, num2])
                    {
                        Terraria.NetMessage.SendData (48, i, -1, "", x, (float)y, 0f, 0f, 0);
                    }
                }
            }
        }
    }
}