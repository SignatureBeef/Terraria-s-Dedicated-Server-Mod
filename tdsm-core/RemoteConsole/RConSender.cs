using System;
using tdsm.api.Command;

namespace tdsm.core.RemoteConsole
{
    public class RConSender : ConsoleSender
    {
        public RConClient Client { get; private set; }

        public RConSender(RConClient rcon)
            : base()
        {
            Op = true;
            Client = rcon;
        }

        public override void SendMessage(string message, int A = 255, byte R = 255, byte G = 255, byte B = 255)
        {
            Client.WriteLine(message);
        }

        public override string Name
        {
            get { return String.Format("{0}@CONSOLE", Client.Name); }
        }
    }
}

