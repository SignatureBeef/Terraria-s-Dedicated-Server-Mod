using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;

namespace Terraria_Server.Messages
{
    class PlayerDataMessage : IMessage
    {

        private const int MAX_HAIR_ID = 17;

        public Packet GetPacket()
        {
            return Packet.PLAYER_DATA;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex;
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }
            else
            {
                playerIndex = (int)readBuffer[start + 1];
            }

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

            if (Main.netMode == 2)
            {
                if (Netplay.slots[whoAmI].state < SlotState.PLAYING)
                {
                    int count = 0;
                    foreach(Player otherPlayer in Main.players)
                    {
                        if (count++ != playerIndex && player.Name.Equals(otherPlayer.Name) && Netplay.slots[count].state >= SlotState.CONNECTED)
                        {
                            Netplay.slots[whoAmI].Kick (player.Name + " is already on this server.");
                            return;
                        }
                    }
                }

                if (player.Name.Length > 20)
                {
                    Netplay.slots[whoAmI].Kick ("Name is too long.");
                    return;
                }

                if (player.Name == "")
                {
                    Netplay.slots[whoAmI].Kick ("Empty name.");
                    return;
                }

                Netplay.slots[whoAmI].oldName = player.Name;
                Netplay.slots[whoAmI].name = player.Name;
                NetMessage.SendData(4, -1, whoAmI, player.Name, playerIndex);
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
