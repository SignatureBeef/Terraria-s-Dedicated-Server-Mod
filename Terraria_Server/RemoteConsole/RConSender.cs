using System;

using Terraria_Server.Commands;

namespace Terraria_Server.RemoteConsole
{
	public class RConSender : ConsoleSender
	{
		public RConClient Client { get; private set; }
		
		public RConSender (RConClient rcon) : base ()
		{
			Op = true;
			Client = rcon;
		}
		
		public override string Name
		{
			get { return "REMOTE CONSOLE"; }
		}
		
		public override void sendMessage (string message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
		{
			Client.WriteLine (message);
		}
	}
}

