using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Threading;
using tdsm.api.Plugin;
using System.Net.Sockets;
using Terraria.Net.Sockets;
using Terraria.Net;
using System.Net;

namespace tdsm.api.Callbacks
{
    //    public abstract class IAPISocket : Terraria.ServerSock
    //    {
    ////        public int Id;
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
    //            return (Terraria.NetMessage.buffer[Id].broadcast || state >= 3) && tcpClient.Connected;
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
        public static bool IsPlaying(this Terraria.RemoteClient sock)
        {
            return sock.State == 10;
        }

        public static bool CanSendWater(this Terraria.RemoteClient sock)
        {
            //return state >= 3;
            return (Terraria.NetMessage.buffer[sock.Id].broadcast || sock.State >= 3) && sock.Socket.IsConnected();
        }

        public static string RemoteAddress(this Terraria.RemoteClient sock)
        {
            return sock.Socket.GetRemoteAddress().ToString();
        }
    }

    public class TemporarySynchSock : ISocket /* Whoever done this, I love you. */
    {

        //
        // Fields
        //
        private TcpClient _connection;

        private TcpListener _listener;

        private SocketConnectionAccepted _listenerCallback;

        private RemoteAddress _remoteAddress;

        private bool _isListening;

        //
        // Constructors
        //
        public TemporarySynchSock(TcpClient tcpClient)
        {
            this._connection = tcpClient;
            this._connection.NoDelay = true;
            IPEndPoint iPEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            this._remoteAddress = new TcpAddress(iPEndPoint.Address, iPEndPoint.Port);
        }

        public TemporarySynchSock()
        {
//            this._connection = new TcpClient ();
//            this._connection.NoDelay = true;
        }

        private void CheckSocket()
        {
            if (this._connection == null)
            {
                this._connection = new TcpClient();
                this._connection.NoDelay = true;
            }
        }

        //
        // Methods
        //
        void ISocket.AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state)
        {
            try
            {
                int len = this._connection.GetStream().Read(data, offset, size);
                if (callback != null)
                    callback(state, len);
            }
            catch (Exception e)
            {
                Tools.WriteLine(e);
            }
//            this._connection.GetStream ().BeginRead (data, offset, size, new AsyncCallback (this.ReadCallback), new Tuple<SocketReceiveCallback, object> (callback, state));
        }

        void ISocket.AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state)
        {
            try
            {
                this._connection.GetStream().Write(data, offset, size);
                if (callback != null)
                    callback(state);
            }
            catch (Exception e)
            {
                Tools.WriteLine(e);
            }
//            this._connection.GetStream ().BeginWrite (data, 0, size, new AsyncCallback (this.SendCallback), new Tuple<SocketSendCallback, object> (callback, state));
        }

        void ISocket.Close()
        {
            this._remoteAddress = null;
            if (this._connection != null)
            {
                this._connection.Close();
            }
        }

        void ISocket.Connect(RemoteAddress address)
        {
            CheckSocket();
            TcpAddress tcpAddress = (TcpAddress)address;
            this._connection.Connect(tcpAddress.Address, tcpAddress.Port);
            this._remoteAddress = address;
        }

        RemoteAddress ISocket.GetRemoteAddress()
        {
            return this._remoteAddress;
        }

        bool ISocket.IsConnected()
        {
            return this._connection != null && this._connection.Client != null && this._connection.Connected;
        }

        bool ISocket.IsDataAvailable()
        {
            return this._connection.GetStream().DataAvailable;
        }

        private void ListenLoop(object unused)
        {
            while (this._isListening && !Terraria.Netplay.disconnect)
            {
                try
                {
                    ISocket socket = new TemporarySynchSock(this._listener.AcceptTcpClient());
                    Tools.WriteLine(socket.GetRemoteAddress() + " is connecting...");
                    this._listenerCallback(socket);
                }
                catch (Exception)
                {
                }
            }
            this._listener.Stop();
        }

        private void ReadCallback(IAsyncResult result)
        {
            Tuple<SocketReceiveCallback, object> tuple = (Tuple<SocketReceiveCallback, object>)result.AsyncState;
            tuple.Item1(tuple.Item2, this._connection.GetStream().EndRead(result));
        }

        private void SendCallback(IAsyncResult result)
        {
            Tuple<SocketSendCallback, object> tuple = (Tuple<SocketSendCallback, object>)result.AsyncState;
            try
            {
                this._connection.GetStream().EndWrite(result);
                tuple.Item1(tuple.Item2);
            }
            catch (Exception)
            {
                ((ISocket)this).Close();
            }
        }

        bool ISocket.StartListening(SocketConnectionAccepted callback)
        {
            this._isListening = true;
            this._listenerCallback = callback;
            if (this._listener == null)
            {
                this._listener = new TcpListener(IPAddress.Any, Terraria.Netplay.ListenPort);
            }
            try
            {
                this._listener.Start();
            }
            catch (Exception)
            {
                return false;
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ListenLoop));
            return true;
        }

        void ISocket.StopListening()
        {
            this._isListening = false;
        }
    }

    public static class NetplayCallback
    {
        //        public static Terraria.ServerSock[] slots;// = new IAPISocket[256];
        #if Full_API
        public static Action<Int32, Vector2, Int32> CheckSectionMethod = Terraria.RemoteClient.CheckSection;
        #endif

        public static void CheckSection(int slot, Vector2 position, int fluff = 1)
        {
#if Full_API
            CheckSectionMethod(slot, position, fluff);
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
                RemoteAddress = Terraria.Netplay.Clients[plr].RemoteAddress()
            };

            HookPoints.AddBan.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.DEFAULT)
            {
                string remote = Terraria.Netplay.Clients[plr].RemoteAddress();
                string ip = remote;
                for (int i = 0; i < remote.Length; i++)
                {
                    if (remote.Substring(i, 1) == ":")
                    {
                        ip = remote.Substring(0, i);
                    }
                }
                using (StreamWriter streamWriter = new StreamWriter(Terraria.Netplay.BanFilePath, true))
                {
                    streamWriter.WriteLine("//" + Terraria.Main.player[plr].name);
                    streamWriter.WriteLine(ip);
                }
            }
#endif
        }

        public static void sendWater(int x, int y)
        {
            if (Terraria.Main.netMode == 1)
            {
                Terraria.NetMessage.SendData(48, -1, -1, "", x, (float)y, 0f, 0f, 0);
                return;
            }
            for (int i = 0; i < 256; i++)
            {
                if ((Terraria.NetMessage.buffer[i] != null &&
                    Terraria.NetMessage.buffer[i].broadcast || Terraria.Netplay.Clients[i].State >= 3) &&
                    Terraria.Netplay.Clients[i] != null &&
                    Terraria.Netplay.Clients[i].Socket != null && Terraria.Netplay.Clients[i].Socket.IsConnected())
                {
                    int num = x / 200;
                    int num2 = y / 150;
                    if (Terraria.Netplay.Clients[i].TileSections[num, num2])
                    {
                        Terraria.NetMessage.SendData(48, i, -1, "", x, (float)y, 0f, 0f, 0);
                    }
                }
            }
        }
    }
}