using System;

namespace Terraria_Server.Messages
{
    public class EnterZoneMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.ENTER_ZONE;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            Player player = Main.players[playerIndex];
            player.zoneEvil = (readBuffer[num++] != 0);
            player.zoneMeteor = (readBuffer[num++] != 0);
            player.zoneDungeon = (readBuffer[num++] != 0);
            player.zoneJungle = (readBuffer[num++] != 0);

            if (Main.netMode == 2)
            {
                NetMessage.SendData(36, -1, whoAmI, "", playerIndex);
            }
        }
    }
}
