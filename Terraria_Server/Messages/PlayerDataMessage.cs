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

            Player player = Main.player[playerIndex];
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

            player.name = Encoding.ASCII.GetString(readBuffer, num, length - num + start).Trim();

            if (Main.netMode == 2)
            {
                if (Netplay.serverSock[whoAmI].state < 10)
                {
                    int count = 0;
                    foreach(Player otherPlayer in Main.player)
                    {
                        if (count++ != playerIndex && player.name.Equals(otherPlayer.name) && Netplay.serverSock[count].active)
                        {
                            NetMessage.SendData(2, whoAmI, -1, player.name + " is already on this server.");
                            return;
                        }
                    }
                }

                if (player.name.Length > 20)
                {
                    NetMessage.SendData(2, whoAmI, -1, "Name is too long.");
                    return;
                }

                if (player.name == "")
                {
                    NetMessage.SendData(2, whoAmI, -1, "Empty name.");
                    return;
                }

                Netplay.serverSock[whoAmI].oldName = player.name;
                Netplay.serverSock[whoAmI].name = player.name;
                NetMessage.SendData(4, -1, whoAmI, player.name, playerIndex);
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
