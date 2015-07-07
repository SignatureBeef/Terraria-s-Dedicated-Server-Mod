using System;
using System.Net.Sockets;

namespace tdsm.api
{
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

        public static void Kick(this Terraria.RemoteClient sock, string reason)
        {
            Terraria.NetMessage.SendData((int)Packet.DISCONNECT, sock.Id, -1, reason);
        }

        public static void SafeClose(this Socket socket)
        {
            if (socket == null) return;

            try
            {
                socket.Close();
            }
            catch (SocketException) { }
            catch (ObjectDisposedException) { }
        }

        public static void SafeShutdown(this Socket socket)
        {
            if (socket == null) return;

            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException) { }
            catch (ObjectDisposedException) { }
        }
    }
}

