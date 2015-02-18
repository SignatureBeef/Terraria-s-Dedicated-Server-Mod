using System;
using System.Net;
using System.Net.Sockets;
using tdsm.core.ServerCore;
using tdsm.core.WebInterface.Auth;

namespace tdsm.core.WebInterface
{
    public static class WebServer
    {
        static IHttpAuth Authentication = new HttpDigestAuth();

        public static void Begin(string[] prefixes)
        {
            var server = new TcpListener(IPAddress.Any, 8458);

            server.Start();

            (new System.Threading.Thread(() =>
            {
                System.Threading.Thread.CurrentThread.Name = "Web";

                server.Server.Poll(500000, SelectMode.SelectRead);
                for (; ; )
                {
                    var client = server.AcceptSocket();
                    AcceptClient(client);
                }
            })).Start();
        }

        static void AcceptClient(Socket client)
        {
            client.NoDelay = true;

            string addr;
            try
            {
                var rep = client.RemoteEndPoint;
                if (rep != null)
                    addr = rep.ToString();
                else
                {
                    //return false;
                }
            }
            catch (Exception e)
            {
                //return false;
            }

            try
            {
                var wr = new WebRequest(client);
                wr.Dispose();
            }
            catch (SocketException)
            {
                //client.SafeClose();
            }

            client.SafeClose();

        }

        static string HandleSocketException(Exception e)
        {
            if (e is SocketException)
                return e.Message;
            if (e is System.IO.IOException)
                return e.Message;
            else if (e is ObjectDisposedException)
                return "Socket already disposed";
            else
                throw new Exception("Unexpected exception in socket handling code", e);
        }
    }
}
