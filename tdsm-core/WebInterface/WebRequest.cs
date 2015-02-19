using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using tdsm.core.ServerCore;

namespace tdsm.core.WebInterface
{
    public class WebRequest : /*Connection,*/ IDisposable
    {
        public Dictionary<String, String> Headers { get; private set; }
        public Dictionary<String, String> Request { get; private set; }
        public string[] Segments { get; private set; }

        public string Method { get; private set; }
        public string Url { get; private set; }
        public string RequestUrl { get; private set; }
        public string Path { get; private set; }

        public int StatusCode { get; set; }

        public Socket Client;

        public WebRequest(Socket sock)
        //: base(sock)
        {
            Client = sock;
            StatusCode = 404;
            //StartReceiving(new byte[4192]);
            Request = new Dictionary<String, String>();
        }

        private byte[] buffer;
        public void StartReceiving(byte[] buff)
        {
            //The HTTP request should send and stop, so we dont need to listen
            var read = Client.Receive(buff);
            if (read > 0)
            {
                buffer = buff.Take(read).ToArray();
                ProcessRead();
            }
        }

        #region "Parsing"
        private void ParseMethod(ref string header)
        {
            //    var next = header.IndexOf("\r\n");
            //    if (next > -1)
            //    {
            //        //var methodString = header.Substring(0, next);

            //        header = header.Remove(0, next + 2);
            //    }
            var offset = 0;
            var next = header.IndexOf(" ");
            if (next > 0)
            {
                Method = header.Substring(0, next);
                offset = next + 1;
            }
            next = header.IndexOf(" ", offset);
            if (next > 0)
            {
                Url = header.Substring(offset, next - offset);
                offset = (next - offset) + 1;
            }
            next = header.IndexOf("\r\n", offset);
            if (next > 0)
            {
                //HTTP protocol
                offset = next + 2;
            }

            if (offset > 0) header = header.Remove(0, offset);
        }

        private string[] ParseHeader(string header)
        {
            var next = header.IndexOf(":");
            if (next > -1)
            {
                return new string[]
                {
                    header.Substring(0, next),
                    header.Remove(0, next + 1)
                };
            }
            return null;
        }

        private void ParseHeaders(string header)
        {
            ParseMethod(ref header);
            Headers = header
                .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => ParseHeader(x))
                .Where(x => x != null)
                .ToDictionary(x => x[0], x => x[1]);
        }

        private void SetSegments()
        {
            if (Path != null) Segments = Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void ParseRequest()
        {
            if (Url != null)
            {
                var ix = Url.IndexOf("?");
                if (ix > -1)
                {
                    Path = Url.Substring(0, ix);
                    RequestUrl = Url.Remove(0, ix + 1);
                    Request = RequestUrl.Split('&')
                        .Select(x => x.Split('='))
                        .Where(x => x.Length == 2)
                        .Distinct(RequestComparer.Comparer)
                        .ToDictionary(x => x[0], x => x[1]);
                }
                else
                {
                    Path = Url;
                }
            }
        }

        class RequestComparer : IEqualityComparer<String[]>
        {
            public static readonly RequestComparer Comparer = new RequestComparer();


            public bool Equals(string[] x, string[] y)
            {
                return x.Length == y.Length && x[0] == y[0];
            }

            public int GetHashCode(string[] obj)
            {
                return obj[0].GetHashCode();
            }
        }
        #endregion

        protected void ProcessRead()
        {
            var html = System.Text.Encoding.UTF8.GetString(buffer);

            const String HeaderSeperator = "\r\n\r\n";
            var ix = html.IndexOf(HeaderSeperator);
            if (ix > -1)
            {
                ParseHeaders(html.Substring(0, ix));
                ParseRequest();
                SetSegments();
                WebServer.HandleRequest(this, html.Remove(0, ix + HeaderSeperator.Length));
            }
            Error("File not found");
        }

        protected void HandleClosure(SocketError error)
        {
        }

        //private static string EncapsulatePath(string path)
        //{
        //    path = path.Replace("..", String.Empty).Replace("/", "\\");
        //    while (path.StartsWith("\\")) path = path.Remove(0, 1);
        //    return path;
        //}


        public void Send(byte[] data)
        {
            if (Client.Connected) Client.Send(data);
        }

        public void Send(byte[] data, int length, SocketFlags flags)
        {
            if (Client.Connected) Client.Send(data, length, flags);
        }

        public void KickAfter(byte[] data)
        {
            Send(data);
            Client.SafeClose();
            throw new RequestEndException();
        }

        public void Error(string message)
        {
            Console.WriteLine("404: " + message);
            var html = "<doctype html><html><title>Not found</title><body>" + message + "</body></html>";
            var response = GetHeader(404, "Not Found", "text/html", html.Length);

            var data = System.Text.Encoding.UTF8.GetBytes(response + html);
            KickAfter(data);
        }

        public void RepsondHeader(int statusCode, string status, string contentType, long contentLength)
        {
            var response = GetHeader(statusCode, status, contentType, contentLength);
            var data = System.Text.Encoding.UTF8.GetBytes(response);
            Send(data);
        }

        private string GetHeader(int statusCode, string status, string contentType, long contentLength)
        {
            return "HTTP/1.1 " + statusCode + " " + status + "\r\n" +
                            "Content-Type: " + contentType + "\r\n" +
                            "Server: TEST\r\n" +
                            "-Powered-By: ASP.NET\r\n" +
                            "Date: Wed, 18 Feb 2015 03:28:26 GMT\r\n" +
                            "Content-Length: " + contentLength + "\r\n\r\n";
        }

        public void Dispose()
        {
            Headers = null;
            Method = RequestUrl = null;
            Console.WriteLine("DISPOSING");
        }
    }
}
