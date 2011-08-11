using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;
using Terraria_Server;
using Terraria_Server.Events;
using Terraria_Server.Logging;

namespace Terraria_Server.Messages
{
    class PlayerDataMessage : IMessage
    {

        private const int MAX_HAIR_ID = 36;

        public Packet GetPacket()
        {
            return Packet.PLAYER_DATA;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            if (whoAmI == Main.myPlayer)
            {
                return;
            }

            var slot = Netplay.slots[whoAmI];
            var player = Main.players[whoAmI];
            
            bool firstTime = player.Name == null;

            int hairId = (int)readBuffer[start + 2];
            if (hairId >= MAX_HAIR_ID)
            {
                hairId = 0;
            }

            player.whoAmi = whoAmI;
            player.hair = hairId;
            player.Male = readBuffer[start + 3] == 1;
            
            num += 3;

            num = setColor(player.hairColor, num, readBuffer);
            num = setColor(player.skinColor, num, readBuffer);
            num = setColor(player.eyeColor, num, readBuffer);
            num = setColor(player.shirtColor, num, readBuffer);
            num = setColor(player.underShirtColor, num, readBuffer);
            num = setColor(player.pantsColor, num, readBuffer);
            num = setColor(player.shoeColor, num, readBuffer);

            if (firstTime)
                player.Difficulty = readBuffer[num++];
            
            string newName;
            
			try
			{
				newName = Encoding.ASCII.GetString(readBuffer, num, length - num + start).Trim();
			}
			catch (ArgumentException)
			{
				slot.Kick ("Invalid name: contains non-ASCII characters.");
				return;
			}
			
			if (! firstTime)
			{
				if (player.Name != newName)
				{
					slot.Kick ("Attempt to change name during session.");
				}
				return;
					
			}
			
			player.Name = newName;
			
			if (player.Name.Length > 20)
			{
				slot.Kick ("Invalid name: longer than 20 characters.");
				return;
			}

			string address = slot.remoteAddress.Split(':')[0];
			
			if (Program.server.BanList.containsException (address))
			{
				ProgramLog.Admin.Log ("Prevented user {0} from accessing the server.", newName);
				slot.Kick ("You are banned from this server.");
				return;
			}

			if (player.Name == "")
			{
				slot.Kick ("Invalid name: whitespace or empty.");
				return;
			}
			
			foreach (char c in player.Name)
			{
				if (c < 32 || c > 126)
				{
					slot.Kick ("Invalid name: contains non-printable characters.");
					return;
				}
			}
			
			if (player.Name.Contains (" " + " "))
			{
				slot.Kick ("Invalid name: contains double spaces.");
				return;
			}
			
			Netplay.slots[whoAmI].oldName = player.Name;
			Netplay.slots[whoAmI].name = player.Name;

			var loginEvent = new PlayerLoginEvent();
			loginEvent.Slot = slot;
			loginEvent.Sender = player;
			Program.server.PluginManager.processHook (Plugin.Hooks.PLAYER_AUTH_QUERY, loginEvent);
			
			if (loginEvent.Action == PlayerLoginAction.REJECT)
			{
				if ((slot.state & SlotState.DISCONNECTING) == 0)
					slot.Kick ("Rejected by server.");
				return;
			}
			else if (loginEvent.Action == PlayerLoginAction.ASK_PASS)
			{
				slot.state = SlotState.PLAYER_AUTH;
				NetMessage.SendData (37, whoAmI, -1, "");
				return;
			}
			else // PlayerLoginAction.ACCEPT
			{
				// don't allow replacing connections for guests, but do for registered users
				if (slot.state < SlotState.PLAYING)
				{
					var name = player.Name.ToLower();
					int count = 0;
					foreach (var otherPlayer in Main.players)
					{
						var otherSlot = Netplay.slots[otherPlayer.whoAmi];
						if (count++ != whoAmI && otherPlayer.Name != null
							&& name == otherPlayer.Name.ToLower() && otherSlot.state >= SlotState.CONNECTED)
						{
							slot.Kick ("A \"" + otherPlayer.Name + "\" is already on this server.");
							return;
						}
					}
				}
				
				NetMessage.SendData (4, -1, whoAmI, player.Name, whoAmI);
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
