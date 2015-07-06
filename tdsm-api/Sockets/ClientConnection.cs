using System;
using System.Diagnostics;
using Terraria;
using System.Net.Sockets;
using tdsm.api.Plugin;
using System.Threading;
using System.Collections.Generic;
using Terraria.Net.Sockets;
using Terraria.Net;
using System.Net;
using System.Linq;
using System.Collections.Concurrent;

namespace tdsm.api
{
    public class ClientConnection : Connection, ISocket
    {
        private TcpAddress _remoteAddress;
        private TcpListener _listener;

        private bool _isListening;

        //        public SlotState State { get; set; }

        public int SlotId;

        public void Set(int value)
        {
            this.SlotId = value;
        }

        private SocketConnectionAccepted _listenerCallback;

        static ClientConnection()
        {
            var thread = new ProgramThread("TmoL", TimeoutLoop);
            thread.IsBackground = true;
            thread.Start();
        }

        public ClientConnection() 
        {
            
        }

        public ClientConnection(Socket sock)
            : base(sock)
        {
            if (SlotId == 0)
                SlotId = -1;
            
            var remoteEndPoint = (IPEndPoint)sock.RemoteEndPoint;
            _remoteAddress = new TcpAddress(remoteEndPoint.Address, remoteEndPoint.Port);

//            sock.LingerState = new LingerOption(true, 10);

//            var ctx = new HookContext
//            {
//                Connection = this
//            };
//
//            var args = new HookArgs.NewConnection();
//
//            HookPoints.NewConnection.Invoke(ref ctx, ref args);
//
//            if (ctx.CheckForKick())
            //                return;

            StartReading();
        }


        //        public void ResetTimeout()
        //        {
        ////            timeout = 0;
        //        }
        //
        static void TimeoutLoop()
        {
            var clients = new List<RemoteClient>();
//            var msg = NewNetMessage.PrepareThreadInstance();

            try
            {
                while (true)
                {
                    Thread.Sleep(5000);

//                lock (All)
                    clients.AddRange(Netplay.Clients);

                    foreach (var client in clients)
                    {
                        client.TimeOutTimer += 5;

                        if (client.State >= 1) //== SlotState.QUEUED)
                        {
                            if (client.TimeOutTimer >= Main.MaxTimeout / 2)
                            {
//                            msg.Clear();
//                            msg.SendTileLoading(1, SlotManager.WaitingMessage(conn));
//                            client.Send(msg.Output);
                                NetMessage.SendData((int)Packet.SEND_TILE_LOADING, client.Id);
                                client.TimeOutTimer = 0;
                            }
                        }
                        else if (client.TimeOutTimer >= Main.MaxTimeout)
                        {
                            try
                            {
                                client.Kick("Timed out.");
                            }
                            catch (Exception e)
                            {
                                Tools.WriteLine("Exception in timeout thread: {0}", e);
                            }
                        }
                    }

                    clients.Clear();
                }
            }
            catch (Exception e)
            {
                Tools.WriteLine(e);
            }
        }

        ConcurrentQueue<AsyncCallback> readQueue = new ConcurrentQueue<AsyncCallback>();

        class AsyncCallback //: IDisposable
        {
            public SocketReceiveCallback Callback;
            public int Offset;
            public int Size;
            public byte[] Buffer;
            public object State;

            public void Dispose()
            {
                Callback = null;
                Offset = 0;
                Size = 0;
                Buffer = null;
                State = null;
            }
        }

        AsyncCallback test;

        void ISocket.AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state)
        {
            if (test == null)
            {
                test = new AsyncCallback()
                {
                    Callback = callback,
                    Buffer = data,
                    Offset = offset,
                    Size = size,
                    State = state
                };
            }
            else
            {
//                test.Data = data;
                test.Offset = offset;
//                test.Data = data;
            }
//            readQueue.Enqueue(new AsyncCallback()
//                {
//                    Callback = callback,
//                    Buffer = data,
//                    Offset = offset,
//                    Size = size,
//                    State = state
//                });
//            Flush();
        }

        void ISocket.AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state)
        {
            Main.ignoreErrors = false;
            var bt = new byte[size];
            Buffer.BlockCopy(data, offset, bt, 0, size);
            this.Send(bt);
            //bt = null;

            if (callback != null)
                callback(state);
            
            DespatchData();
        }

        protected override void ProcessRead()
        {
            DespatchData();

        }

        object sync = new object();

        void DespatchData()
        {
            Tools.WriteLine("IsActive: " + Netplay.Clients[tdsm.api.Callbacks.NetplayCallback.LastSlot].IsActive);
            Tools.WriteLine("Active: " + this.Active);
            Tools.WriteLine("receiving: " + receiving);
            Tools.WriteLine("PendingTermination: " + Netplay.Clients[tdsm.api.Callbacks.NetplayCallback.LastSlot].PendingTermination);
            Tools.WriteLine("State: " + Netplay.Clients[tdsm.api.Callbacks.NetplayCallback.LastSlot].State);
            Tools.WriteLine("anyClients: " + Netplay.anyClients);
            Tools.WriteLine("active: " + Main.player[tdsm.api.Callbacks.NetplayCallback.LastSlot].active);

            int processed = 0;
            lock (sync)
            {
                while (recvBytes > 0)
                {
                    Console.WriteLine(SlotId);
//                AsyncCallback cb;
//                if (readQueue.TryDequeue(out cb))
                    if (test != null)
                    {
                        var len = recvBytes;
                        if (recvBytes > test.Size)
                            len = test.Size;
                    
                        if (len > 0)
                        {
                            Buffer.BlockCopy(recvBuffer, processed, test.Buffer, test.Offset, len);
                            test.Callback(test.State, len);

//                    cb.Dispose();

                            processed += len;
                        }
                    }
                    else
                    {
                        break;
                    }

                    if (processed == recvBytes)
                    {
                        recvBytes = 0;
                    }
                    else if (processed > 0)
                    {
                        Buffer.BlockCopy(recvBuffer, processed, recvBuffer, 0, recvBytes - processed);

                        recvBytes -= processed;
                    }
                }
            }
        }

        protected override void HandleClosure(SocketError err)
        {
            Tools.WriteLine("{0}: connection closed ({1}).", RemoteAddress, err);

            if (SlotId > 0 && SlotId < 255)
            {
                var player = Main.player[SlotId];
                if (player != null)
                {
//                    player.Connection = null;
//                    player.active = false;
                }
            }
        }

        public void StartReading()
        {
            #if DEBUG
            Main.ignoreErrors = false;
            #endif
            StartReceiving(new byte[4192]);
            txBuffer = new byte[16384];
        }

        void ISocket.Close()
        {
//            CloseSocket();
        }

        void ISocket.Connect(RemoteAddress address)
        {
            _remoteAddress = address as TcpAddress;
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(_remoteAddress.Address, _remoteAddress.Port);
            SetSocket(sock);
        }

        RemoteAddress ISocket.GetRemoteAddress()
        {
            return _remoteAddress;
        }

        public void Kick(string reason)
        {
            var messageLength = (short)(1 /*PacketId*/ + reason.Length);

            using (var ms = new System.IO.MemoryStream())
            {
                using (var bw = new System.IO.BinaryWriter(ms))
                {
                    bw.Write(messageLength);
                    bw.Write((byte)Packet.DISCONNECT);
                    bw.Write(reason);
                }

                KickAfter(ms.ToArray());
            }
        }

        bool ISocket.StartListening(SocketConnectionAccepted callback)
        {
            this._isListening = true;
            this._listenerCallback = callback;
            if (this._listener == null)
            {
                this._listener = new TcpListener(IPAddress.Any, Netplay.ListenPort);
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

        bool ISocket.IsConnected()
        {
            return Active;
        }

        bool ISocket.IsDataAvailable()
        {
            return receiving;
        }

        private void ListenLoop(object unused)
        {
            while (this._isListening && !Terraria.Netplay.disconnect)
            {
                try
                {
                    ISocket socket = new ClientConnection(this._listener.AcceptSocket());
                    Tools.WriteLine(socket.GetRemoteAddress() + " is connecting...");
                    this._listenerCallback(socket);
                }
                catch (Exception)
                {
                }
            }
            this._listener.Stop();
        }
    }
}

