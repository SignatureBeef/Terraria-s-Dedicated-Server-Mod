using System;

namespace Terraria_Server.Messages
{
    public class EnterZoneMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.ENTER_ZONE;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = (int)readBuffer[num++];
            
            if (playerIndex != whoAmI)
            {
                Netplay.slots[whoAmI].Kick ("Cheating detected (ENTER_ZONE forgery).");
                return;
            }

            
            playerIndex = whoAmI;

            Player player = Main.players[playerIndex];
            player.zoneEvil = (readBuffer[num++] != 0);
            player.zoneMeteor = (readBuffer[num++] != 0);
            player.zoneDungeon = (readBuffer[num++] != 0);
            player.zoneJungle = (readBuffer[num++] != 0);

            NetMessage.SendData(36, -1, whoAmI, "", playerIndex);
        }
    }
}
