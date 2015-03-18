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

        public override void SendMessage(string message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
            Client.WriteLine(message);
        }

        public override string Name
        {
            get { return String.Format("{0}@CONSOLE", Client.Name); }
        }
    }
}

