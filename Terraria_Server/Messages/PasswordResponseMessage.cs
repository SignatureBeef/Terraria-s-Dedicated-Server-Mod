using System;
using System.Text;
using Terraria_Server;
using Terraria_Server.Events;

namespace Terraria_Server.Messages
{
    public class PasswordResponseMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PASSWORD_RESPONSE;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            String password = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            
			var slot = Netplay.slots[whoAmI];
            
			if (slot.state == SlotState.SERVER_AUTH)
			{
				if (password == Netplay.password)
				{
					slot.state = SlotState.ACCEPTED;
					NetMessage.SendData (3, whoAmI);
					return;
				}
				slot.Kick ("Incorrect server password.");
			}
			else if (slot.state == SlotState.PLAYER_AUTH)
			{
				var name = Main.players[whoAmI].Name ?? "";

				var loginEvent = new PlayerLoginEvent();
				loginEvent.Slot = slot;
				loginEvent.Sender = Main.players[whoAmI];
				Program.server.PluginManager.processHook (Plugin.Hooks.PLAYER_AUTH_REPLY, loginEvent);
				
				if (loginEvent.Action == PlayerLoginAction.REJECT)
				{
					if ((slot.state & SlotState.DISCONNECTING) == 0)
						slot.Kick ("Incorrect password for user: " + (name ?? ""));
				}
				else if (loginEvent.Action == PlayerLoginAction.ASK_PASS)
				{
					NetMessage.SendData (37, whoAmI, -1, "");
				}
				else // PlayerLoginAction.ACCEPT
				{
					slot.state = SlotState.SENDING_WORLD;
					
					NetMessage.SendData (4, -1, whoAmI, name, whoAmI); // broadcast player data now
					
					// replay packets from side buffer
					var buf = NetMessage.buffer[whoAmI];
					NetMessage.CheckBytes (whoAmI, buf.sideBuffer, ref buf.sideBufferBytes, ref buf.sideBufferMsgLen);
					buf.ResetSideBuffer ();
					
					NetMessage.SendData (7, whoAmI); // continue with world data
				}
			}
		}
    }
}
