using System;
using System.Text;
using Terraria_Server.Logging;
using Terraria_Server.Networking;
using Terraria_Server.Plugins;

namespace Terraria_Server.Messages
{
    public class ConnectionRequestMessage : MessageHandler
    {
		public ConnectionRequestMessage ()
		{
			ValidStates = SlotState.CONNECTED;
		}
		
        public override Packet GetPacket()
        {
            return Packet.CONNECTION_REQUEST;
        }

        public override void Process (ClientConnection conn, byte[] readBuffer, int length, int num)
        {
//            ServerSlot slot = Netplay.slots[whoAmI];
//            PlayerLoginEvent loginEvent = new PlayerLoginEvent();
//            loginEvent.Slot = slot;
//            loginEvent.Sender = Main.players[whoAmI];
//            Server.PluginManager.processHook(Plugin.Hooks.PLAYER_PRELOGIN, loginEvent);
//            if ((loginEvent.Cancelled || loginEvent.Action == PlayerLoginAction.REJECT) && (slot.state & SlotState.DISCONNECTING) == 0)
//            {
//                slot.Kick ("Disconnected by server.");
//                return;
//            }

            string clientName = conn.RemoteAddress.Split(':')[0];
//
//            if (Server.BanList.containsException(clientName))
//            {
//                slot.Kick ("You are banned from this Server.");
//                return;
//            }

            if (Program.properties.UseWhiteList && !Server.WhiteList.containsException(clientName))
            {
                conn.Kick ("You are not on the WhiteList.");
                return;
            }

			string version = Networking.StringCache.FindOrMake (new ArraySegment<byte> (readBuffer, num, length - 1));
			
			var ctx = new HookContext
			{
				Connection = conn,
			};
			
			var args = new HookArgs.ConnectionRequestReceived
			{
				Version = version,
			};
			
			HookPoints.ConnectionRequestReceived.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick ())
				return;
			
			if (ctx.Result == HookResult.DEFAULT && !(version == "Terraria" + Statics.CURRENT_TERRARIA_RELEASE))
			{
				if (version.Length > 30) version = version.Substring (0, 30);
				ProgramLog.Debug.Log ("Client version string: {0}", version);
				conn.Kick (string.Concat ("This server requires Terraria ", Statics.VERSION_NUMBER));
				return;
			}
			
			var msg = NetMessage.PrepareThreadInstance ();
			
			if (ctx.Result == HookResult.ASK_PASS || (NetPlay.password != null && NetPlay.password != ""))
			{
				conn.State = SlotState.SERVER_AUTH;
				msg.PasswordRequest ();
				conn.Send (msg.Output);
				return;
			}
			
			conn.State = SlotState.ACCEPTED;
			msg.ConnectionResponse (253 /* arbitrary fake value, true slot assigned later */);
			conn.Send (msg.Output);
        }
    }
}
