using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;
using Terraria_Server;
using Terraria_Server.Logging;
using Terraria_Server.Networking;
using Terraria_Server.Plugins;

namespace Terraria_Server.Messages
{
    class PlayerDataMessage : MessageHandler
    {
		public PlayerDataMessage ()
		{
			//IgnoredStates |= SlotState.ASSIGNING_SLOT;
			ValidStates = SlotState.ACCEPTED | SlotState.ASSIGNING_SLOT |
				/* this so that we can have a custom error message */
				SlotState.SENDING_WORLD | SlotState.SENDING_TILES | SlotState.PLAYING;
		}

        private const int MAX_HAIR_ID = 36;

        public override Packet GetPacket()
        {
            return Packet.PLAYER_DATA;
        }
        
		static string GetName (ClientConnection conn, byte[] readBuffer, int num, int len)
		{
			string name;
			
			try
			{
				name = Encoding.ASCII.GetString (readBuffer, num, len).Trim();
			}
			catch (ArgumentException)
			{
				conn.Kick ("Invalid name: contains non-ASCII characters.");
				return null;
			}
			
			if (name.Length > 20)
			{
				conn.Kick ("Invalid name: longer than 20 characters.");
				return null;
			}

			if (name == "")
			{
				conn.Kick ("Invalid name: whitespace or empty.");
				return null;
			}
			
			foreach (char c in name)
			{
				if (c < 32 || c > 126)
				{
					conn.Kick ("Invalid name: contains non-printable characters.");
					return null;
				}
			}
			
			if (name.Contains (" " + " "))
			{
				conn.Kick ("Invalid name: contains double spaces.");
				return null;
			}
			
			return name;
		}
        
        public override void Process (ClientConnection conn, byte[] readBuffer, int length, int num)
        {
			int start = num - 1;
			
			if (conn.State == SlotState.ASSIGNING_SLOT)
			{
				// TODO: verify that data didn't change.
				int who = conn.SlotIndex;
				NetMessage.SendData (4, -1, who, conn.Player.Name, who);
				return;
			}
			
			if (conn.Player != null)
			{
				conn.Kick ("Player data sent twice.");
				return;
			}
			
			var player = new Player ();
			conn.Player = player;
			player.Connection = conn;
			player.IPAddress = conn.RemoteAddress;
			player.whoAmi = conn.SlotIndex;
			
			var data = new HookArgs.PlayerDataReceived ();
			
			data.Parse (readBuffer, num + 1, length);
            
			if (data.Hair >= MAX_HAIR_ID)
			{
			    data.Hair = 0;
			}
			
			var ctx = new HookContext
			{
				Connection = conn,
				Player = player,
				Sender = player,
			};
			
			HookPoints.PlayerDataReceived.Invoke (ref ctx, ref data);
			
			if (ctx.CheckForKick ())
				return;
			
			if (! data.NameChecked)
			{
				string error;
				if (! data.CheckName (out error))
				{
					conn.Kick (error);
					return;
				}
			}
			
			if (! data.BansChecked)
			{
				string address = conn.RemoteAddress.Split(':')[0];
				
				if (Server.BanList.containsException (address) || Server.BanList.containsException (data.Name))
				{
					ProgramLog.Admin.Log ("Prevented user {0} from accessing the server.", data.Name);
					conn.Kick ("You are banned from this server.");
					return;
				}
			}
			
			data.Apply (player);

			if (ctx.Result == HookResult.ASK_PASS)
			{
				conn.State = SlotState.PLAYER_AUTH;
				
				var msg = NetMessage.PrepareThreadInstance ();
				msg.PasswordRequest ();
				conn.Send (msg.Output);

				return;
			}
			else // HookResult.DEFAULT
			{
				// don't allow replacing connections for guests, but do for registered users
				if (conn.State < SlotState.PLAYING)
				{
					var lname = player.Name.ToLower();

					foreach (var otherPlayer in Main.players)
					{
						var otherSlot = NetPlay.slots[otherPlayer.whoAmi];
						if (otherPlayer.Name != null && lname == otherPlayer.Name.ToLower() && otherSlot.state >= SlotState.CONNECTED)
						{
							conn.Kick ("A \"" + otherPlayer.Name + "\" is already on this server.");
							return;
						}
					}
				}
				
				//conn.Queue = (int)loginEvent.Priority; // actual queueing done on world request message
				
				// and now decide whether to queue the connection
				//SlotManager.Schedule (conn, (int)loginEvent.Priority);
				
				//NetMessage.SendData (4, -1, -1, player.Name, whoAmI);
			}
        }


        private int setColor(Color color, int bufferPos, byte[] readBuffer)
        {
            color.R = readBuffer[bufferPos++];
            color.G = readBuffer[bufferPos++];
            color.B = readBuffer[bufferPos++];
            return bufferPos;
        }

    }
}
