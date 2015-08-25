using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;

namespace TDSM.API
{
    public static class SocketExtensions
    {
        #if Full_API
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
        #endif

        public static void SafeClose(this Socket socket)
        {
            if (socket == null)
                return;

            try
            {
                socket.Close();
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public static void SafeShutdown(this Socket socket)
        {
            if (socket == null)
                return;

            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }

    public static class LinqExtensions
    {
        static readonly Random _rand = new Random();

        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            var list = enumerable as IList<T> ?? enumerable.ToList();
            var count = list.Count;
            if (count == 0)
                return default(T);
            return list.ElementAt(_rand.Next(0, count));
        }

        /// <summary>
        /// Shuffle the specified data.
        /// </summary>
        /// <remarks>Based on https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle</remarks>
        /// <param name="data">Data.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> Shuffle<T>(this T[] data)
        {
            var n = data.Length;  
            while (n > 1)
            {  
                n--;  
                var j = _rand.Next(n + 1);
                T value = data[j];  
                data[j] = data[n];  
                data[n] = value;  
            }  

            return data;
        }
    }

    public static class AssemblyExtensions
    {
        public static Type[] GetTypesLoaded(this System.Reflection.Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null).ToArray();
            } 
        }
    }
}

