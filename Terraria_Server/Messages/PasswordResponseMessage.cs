using System;
using System.Text;
using Terraria_Server;
using Terraria_Server.Events;
using Terraria_Server.Networking;

namespace Terraria_Server.Messages
{
    public class PasswordResponseMessage : MessageHandler
    {
		public PasswordResponseMessage ()
		{
			ValidStates = SlotState.SERVER_AUTH | SlotState.PLAYER_AUTH;
		}
    
        public override Packet GetPacket()
        {
            return Packet.PASSWORD_RESPONSE;
        }

        public override void Process (ClientConnection conn, byte[] readBuffer, int length, int num)
        {
            int start = num - 1;
            String password = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            
			if (conn.State == SlotState.SERVER_AUTH)
			{
				if (password == Netplay.password)
				{
					conn.State = SlotState.ACCEPTED;
					
					var msg = NetMessage.PrepareThreadInstance ();
					msg.ConnectionResponse (253);
					conn.Send (msg.Output);
					
					return;
				}
				conn.Kick ("Incorrect server password.");
			}
			else if (conn.State == SlotState.PLAYER_AUTH)
			{
				var name = conn.Player.Name ?? "";

				var loginEvent = new PlayerLoginEvent();
				loginEvent.Sender = conn.Player;
				loginEvent.Password = password;
				Program.server.PluginManager.processHook (Plugin.Hooks.PLAYER_AUTH_REPLY, loginEvent);
				
				if (loginEvent.Action == PlayerLoginAction.REJECT)
				{
					if ((conn.State & SlotState.DISCONNECTING) == 0)
						conn.Kick ("Incorrect password for user: " + (name ?? ""));
				}
				else if (loginEvent.Action == PlayerLoginAction.ASK_PASS)
				{
					var msg = NetMessage.PrepareThreadInstance ();
					msg.PasswordRequest ();
					conn.Send (msg.Output);
				}
				else // PlayerLoginAction.ACCEPT
				{
					var lower = name.ToLower();
					int count = 0;
					foreach (var otherPlayer in Main.players)
					{
						//var otherSlot = Netplay.slots[otherPlayer.whoAmi];
						var otherConn = otherPlayer.Connection;
						if (otherPlayer.Name != null
							&& lower == otherPlayer.Name.ToLower()
							&& otherConn != null
							&& otherConn.State >= SlotState.CONNECTED)
						{
							otherConn.Kick ("Replaced by new connection.");
						}
					}

					//conn.State = SlotState.SENDING_WORLD;
					
					conn.Queue = (int)loginEvent.Priority;
					SlotManager.Schedule (conn, conn.Queue);
					
					//NetMessage.SendData (4, -1, whoAmI, name, whoAmI); // broadcast player data now
					
					// replay packets from side buffer
					//conn.conn.ProcessSideBuffer ();
					//var buf = NetMessage.buffer[whoAmI];
					//NetMessage.CheckBytes (whoAmI, buf.sideBuffer, ref buf.sideBufferBytes, ref buf.sideBufferMsgLen);
					//buf.ResetSideBuffer ();
					
					//NetMessage.SendData (7, whoAmI); // continue with world data
				}
			}
		}
    }
}
