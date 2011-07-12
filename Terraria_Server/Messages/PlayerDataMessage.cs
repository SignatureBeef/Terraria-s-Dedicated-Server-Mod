using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;
using Terraria_Server;
using Terraria_Server.Events;

namespace Terraria_Server.Messages
{
    class PlayerDataMessage : IMessage
    {

        private const int MAX_HAIR_ID = 17;

        public Packet GetPacket()
        {
            return Packet.PLAYER_DATA;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = whoAmI;
            var slot = Netplay.slots[whoAmI];

            if (playerIndex == Main.myPlayer)
            {
                return;
            }

            int hairId = (int)readBuffer[start + 2];
            if (hairId >= MAX_HAIR_ID)
            {
                hairId = 0;
            }

            Player player = Main.players[playerIndex];
            player.hair = hairId;
            player.whoAmi = playerIndex;
            num += 2;

            num = setColor(player.hairColor, num, readBuffer);
            num = setColor(player.skinColor, num, readBuffer);
            num = setColor(player.eyeColor, num, readBuffer);
            num = setColor(player.shirtColor, num, readBuffer);
            num = setColor(player.underShirtColor, num, readBuffer);
            num = setColor(player.pantsColor, num, readBuffer);
            num = setColor(player.shoeColor, num, readBuffer);

            player.hardCore = (readBuffer[num++] != 0);

            player.Name = Encoding.ASCII.GetString(readBuffer, num, length - num + start).Trim();
			
			if (slot.state < SlotState.PLAYING)
			{
				int count = 0;
				foreach(Player otherPlayer in Main.players)
				{
					if (count++ != playerIndex && player.Name.Equals(otherPlayer.Name) && slot.state >= SlotState.CONNECTED)
					{
						slot.Kick (player.Name + " is already on this server.");
						return;
					}
				}
			}

			if (player.Name.Length > 20)
			{
				slot.Kick ("Name is too long.");
				return;
			}

			if (player.Name == "")
			{
				slot.Kick ("Empty name.");
				return;
			}

			Netplay.slots[whoAmI].oldName = player.Name;
			Netplay.slots[whoAmI].name = player.Name;

			var loginEvent = new PlayerLoginEvent();
			loginEvent.Slot = slot;
			loginEvent.Sender = Main.players[whoAmI];
			Program.server.PluginManager.processHook (Plugin.Hooks.PLAYER_AUTH_QUERY, loginEvent);
			
			if (loginEvent.Action == PlayerLoginAction.REJECT)
			{
				if ((slot.state & SlotState.DISCONNECTING) == 0)
					slot.Kick ("Disconnected by server.");
				return;
			}
			else if (loginEvent.Action == PlayerLoginAction.ASK_PASS)
			{
				slot.state = SlotState.PLAYER_AUTH;
				NetMessage.SendData (37, whoAmI, -1, "");
				return;
			}
			
			// PlayerLoginAction.ACCEPT
			NetMessage.SendData (4, -1, whoAmI, player.Name, playerIndex);
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
