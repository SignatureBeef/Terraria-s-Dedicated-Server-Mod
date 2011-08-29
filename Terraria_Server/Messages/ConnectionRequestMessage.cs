using System;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Logging;
using Terraria_Server.Networking;

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
//            Program.server.PluginManager.processHook(Plugin.Hooks.PLAYER_PRELOGIN, loginEvent);
//            if ((loginEvent.Cancelled || loginEvent.Action == PlayerLoginAction.REJECT) && (slot.state & SlotState.DISCONNECTING) == 0)
//            {
//                slot.Kick ("Disconnected by server.");
//                return;
//            }

            String clientName = conn.RemoteAddress.Split(':')[0];
//
//            if (Program.server.BanList.containsException(clientName))
//            {
//                slot.Kick ("You are banned from this Server.");
//                return;
//            }

            if (Program.properties.UseWhiteList && !Program.server.WhiteList.containsException(clientName))
            {
                conn.Kick ("You are not on the WhiteList.");
                return;
            }

            if (conn.State == SlotState.CONNECTED)
            {
                String version = Encoding.ASCII.GetString(readBuffer, num, length - 1);
#if TEST_COMPRESSION
                if ((version == "Terraria" + Statics.CURRENT_TERRARIA_RELEASE + "undead"))
                {
                    ProgramLog.Debug.Log ("{0}: Undead's client detected.", conn.RemoteAddress);
                    conn.myClient = true;
                }
                else
#endif
                if (!(version == "Terraria" + Statics.CURRENT_TERRARIA_RELEASE))
                {
                    if (version.Length > 30) version = version.Substring (0, 30);
                    ProgramLog.Debug.Log ("Client version string: {0}", version);
                    conn.Kick (string.Concat ("This server requires Terraria ", Program.VERSION_NUMBER));
                    return;
                }
				
				var msg = NetMessage.PrepareThreadInstance ();
				
                if (Netplay.password == null || Netplay.password == "")
                {
                    conn.State = SlotState.ACCEPTED;
                    
                    msg.ConnectionResponse (253);
                    conn.Send (msg.Output);
                    return;
                }

                conn.State = SlotState.SERVER_AUTH;
                msg.PasswordRequest ();
                conn.Send (msg.Output);
            }
        }
    }
}
