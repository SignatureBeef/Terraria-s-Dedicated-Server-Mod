using System;
using System.Text;
using Terraria_Server;
using Terraria_Server.Plugins;
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
            string password = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            
			if (conn.State == SlotState.SERVER_AUTH)
			{
				var ctx = new HookContext
				{
					Connection = conn,
				};
				
				var args = new HookArgs.ServerPassReceived
				{
					Password = password,
				};
				
				HookPoints.ServerPassReceived.Invoke (ref ctx, ref args);
				
				if (ctx.CheckForKick ())
					return;
				
				if (ctx.Result == HookResult.ASK_PASS)
				{
					var msg = NetMessage.PrepareThreadInstance ();
					msg.PasswordRequest ();
					conn.Send (msg.Output);
				}
				else if (ctx.Result == HookResult.CONTINUE || password == NetPlay.password)
				{
					conn.State = SlotState.ACCEPTED;
					
					var msg = NetMessage.PrepareThreadInstance ();
					msg.ConnectionResponse (253 /* dummy value, real slot assigned later */);
					conn.Send (msg.Output);
					
					return;
				}
				
				conn.Kick ("Incorrect server password.");
			}
			else if (conn.State == SlotState.PLAYER_AUTH)
			{
				var name = conn.Player.Name ?? "";
				
				var ctx = new HookContext
				{
					Connection = conn,
					Player = conn.Player,
					Sender = conn.Player,
				};
				
				var args = new HookArgs.PlayerPassReceived
				{
					Password = password,
				};
				
				HookPoints.PlayerPassReceived.Invoke (ref ctx, ref args);
				
				if (ctx.CheckForKick ())
					return;
				
				if (ctx.Result == HookResult.ASK_PASS)
				{
					var msg = NetMessage.PrepareThreadInstance ();
					msg.PasswordRequest ();
					conn.Send (msg.Output);
				}
				else // HookResult.DEFAULT
				{
					var lower = name.ToLower();
					bool reserved = false;
					
					//conn.Queue = (int)loginEvent.Priority;
					
					foreach (var otherPlayer in Main.players)
					{
						//var otherSlot = Netplay.slots[otherPlayer.whoAmi];
						var otherConn = otherPlayer.Connection;
						if (otherPlayer.Name != null
							&& lower == otherPlayer.Name.ToLower()
							&& otherConn != null
							&& otherConn.State >= SlotState.CONNECTED)
						{
							if (! reserved)
							{
								reserved = SlotManager.HandoverSlot (otherConn, conn);
							}
							otherConn.Kick ("Replaced by new connection.");
						}
					}

					//conn.State = SlotState.SENDING_WORLD;
					
					if (! reserved) // reserved slots get assigned immediately during the kick
					{
						SlotManager.Schedule (conn, conn.DesiredQueue);
					}
					
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
