using System;
using System.Text;
using Terraria_Server.Networking;
using Terraria_Server.Logging;
using Terraria_Server.Plugins;

namespace Terraria_Server.Messages
{
	public class DisconnectMessage : MessageHandler
	{
		public DisconnectMessage ()
		{
			ValidStates = SlotState.CONNECTED | SlotState.ACCEPTED | SlotState.SERVER_AUTH;
		}
	
		public override Packet GetPacket()
		{
			return Packet.DISCONNECT;
		}
		
		public override void Process (ClientConnection conn, byte[] readBuffer, int length, int num)
		{
			var data = Encoding.ASCII.GetString (readBuffer, num, length - 1);
			var lines = data.Split ('\n');
			
			foreach (var line in lines)
			{
				if (line == "tdcm1")
				{
					//player.HasClientMod = true;
					ProgramLog.Log ("{0} is a TDCM protocol version 1 client.", conn.RemoteAddress);
				}
				else if (line == "tdsmcomp1")
				{
					conn.CompressionVersion = 1;
					ProgramLog.Log ("{0} supports TDSM compression version 1.", conn.RemoteAddress);
				}
			}
			
			var ctx = new HookContext
			{
				Connection = conn,
			};
			
			var args = new HookArgs.DisconnectReceived
			{
				Content = data,
				Lines   = lines,
			};
			
			HookPoints.DisconnectReceived.Invoke (ref ctx, ref args);
			
			ctx.CheckForKick ();
		}
	}
}

