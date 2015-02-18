using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using tdsm.core.ServerCore;

namespace tdsm.core.WebInterface
{
    class WebRequest : Connection, IDisposable
    {
        public Dictionary<String, String> Headers { get; private set; }

        public string Method { get; private set; }
        public string RequestUrl { get; private set; }

        public WebRequest(Socket sock)
            : base(sock)
        {
            StartReceiving(new byte[4192]);
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
                RequestUrl = header.Substring(offset, next - offset);
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
        #endregion

        protected override void ProcessRead()
        {
            if (recvBytes < recvBuffer.Length)
            {
                var html = System.Text.Encoding.UTF8.GetString(recvBuffer, 0, recvBytes);

                const String HeaderSeperator = "\r\n\r\n";
                var ix = html.IndexOf(HeaderSeperator);
                if (ix > -1)
                {
                    ParseHeaders(html.Substring(0, ix));
                    HandleRequest(this, html.Remove(0, ix + HeaderSeperator.Length));
                }
            }
        }

        protected override void HandleClosure(SocketError error)
        {
        }

        public static void HandleRequest(WebRequest request, string content)
        {
            var response = "HTTP/1.1 404 Not Found\r\n" +
                            "Content-Type: text/html\r\n" +
                            "Server: TEST\r\n" +
                            "-Powered-By: ASP.NET\r\n" +
                            "Date: Wed, 18 Feb 2015 03:28:26 GMT\r\n" +
                            "Content-Length: 49\r\n\r\n<doctype html><html><title>Title</title>Hi</html>";

            var data = System.Text.Encoding.UTF8.GetBytes(response);
            request.KickAfter(data);
        }

        public void Dispose()
        {
            Headers = null;
            Method = RequestUrl = null;
        }
    }
}
