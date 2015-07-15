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
using tdsm.api.Misc;
using tdsm.api.Logging;

namespace tdsm.api
{
    public class ClientConnection : Connection, ISocket
    {
        private TcpAddress _remoteAddress;
        private TcpListener _listener;

        private bool _isListening;
        private bool _isReceiving;

        //        public SlotState State { get; set; }

        public int SlotId;

        public void Set(int value)
        {
            this.SlotId = value;
        }

        private SocketConnectionAccepted _listenerCallback;

        /*static ClientConnection()
        {
            var thread = new ProgramThread("TmoL", TimeoutLoop);
            thread.IsBackground = true;
            thread.Start();
        }*/

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

            sock.LingerState = new LingerOption(true, 10);

            var ctx = new HookContext
            {
                Connection = this
            };
            
            var args = new HookArgs.NewConnection();
            
            HookPoints.NewConnection.Invoke(ref ctx, ref args);
            
            if (ctx.CheckForKick())
                return;

            _isReceiving = true; //The connection was established, so we can begin reading
        }

        //        public void ResetTimeout()
        //        {
        //            timeout = 0;
        //        }

        /*static void TimeoutLoop()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(5000);

                    if (Netplay.Clients != null)
                    {
                        for (var i = 0; i < Netplay.Clients.Length; i++)
                        {
                            var client = Netplay.Clients[i];
                            if (client != null)
                            {
                                client.TimeOutTimer += 5;

                                if (client.State >= 0) //== SlotState.QUEUED)
                                {
                                    if (client.TimeOutTimer >= Main.MaxTimeout / 2)
                                    {
                                        NetMessage.SendData((int)Packet.SEND_TILE_LOADING, client.Id);
                                        client.TimeOutTimer = 0;
                                    }
                                }
                                else if (client.TimeOutTimer >= Main.MaxTimeout)
                                {
                                    try
                                    {
                                        client.Kick("Timed out.");
                                        client.TimeOutTimer = 0;
                                    }
                                    catch (Exception e)
                                    {
                                        Tools.WriteLine("Exception timing out client @{1}: {0}", e, i);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ProgramLog.Error.Log("Exception in timeout thread: {0}", e);
            }
        }*/

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

        AsyncCallback _callback;

        void ISocket.AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state)
        {
            if (_callback == null)
            {
                _callback = new AsyncCallback()
                {
                    Callback = callback,
                    Buffer = data,
                    Offset = offset,
                    Size = size,
                    State = state
                };
                StartReading();
            }
            else
            {
                _callback.Offset = offset;
            }
        }

        void ISocket.AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state)
        {
            Main.ignoreErrors = false;
            var bt = new byte[size];
            Buffer.BlockCopy(data, offset, bt, 0, size);
            this.Send(bt);
            bt = null;

            if (callback != null)
                callback(state);
            
            this.Flush();
        }

        protected override void ProcessRead()
        {
            var local = new byte[recvBytes];
            Buffer.BlockCopy(recvBuffer, 0, local, 0, recvBytes);

            //Reset read position
            recvBytes = 0;

            DespatchData(local);
        }

        void DespatchData(byte[] buff)
        {
            if (_callback == null)
                throw new InvalidOperationException("No callback to read data to");
            try
            {
                int processed = 0;
                while (processed < buff.Length)
                {
                    var len = buff.Length - processed;
                    if (len > _callback.Size)
                        len = _callback.Size;
                        
                    if (len > 0)
                    {
                        //No point shifting the target buffer as they always read from index 0
                        Buffer.BlockCopy(buff, processed, _callback.Buffer, _callback.Offset, len);
                        _callback.Callback(_callback.State, len);

                        processed += len;
                    }
                }
            }
            catch (Exception e)
            {
                var buffLength = buff == null ? "<null>" : buff.Length.ToString();
                var callbackExists = _callback == null ? "<null>" : "yes";
                var cbBufferExists = _callback == null || _callback.Buffer == null ? "<null>" : "Nope";
                var cbOffset = _callback == null ? "<null>" : _callback.Offset.ToString();
                var cbSize = _callback == null ? "<null>" : _callback.Size.ToString();

                ProgramLog.Error.Log("Read exception caught.");
                ProgramLog.Error.Log("Debug Values: {0},{1},{2},{3},{4}", buffLength, callbackExists, cbBufferExists, cbOffset, cbSize);
                ProgramLog.Error.Log("Error: {0}", e);
            }
        }

        protected override void HandleClosure(SocketError err)
        {
            Tools.WriteLine("{0}: connection closed ({1}).", RemoteAddress, err);
            _isReceiving = false;

            //Issue a 0 byte response, Terraria will close the connection :)
            _callback.Callback(null, 0);
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
            if (_isReceiving)
                Close();
            _isReceiving = false;
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
            return Active || _isReceiving;
        }

        bool ISocket.IsDataAvailable()
        {
            return recvBytes > 0 || _isReceiving;
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
                catch
                {
                }
            }
            this._listener.Stop();
        }
    }
}

