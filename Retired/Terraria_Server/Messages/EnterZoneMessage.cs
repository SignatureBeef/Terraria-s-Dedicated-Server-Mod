using System;

namespace Terraria_Server.Messages
{
    public class EnterZoneMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.ENTER_ZONE;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = (int)readBuffer[num++];
            
            if (playerIndex != whoAmI)
            {
                NetPlay.slots[whoAmI].Kick ("Cheating detected (ENTER_ZONE forgery).");
                return;
            }

            
            playerIndex = whoAmI;

            Player player = Main.players[playerIndex];
            player.zoneEvil = (readBuffer[num++] != 0);
            player.zoneMeteor = (readBuffer[num++] != 0);
            player.zoneDungeon = (readBuffer[num++] != 0);
            player.zoneJungle = (readBuffer[num++] != 0);
            player.zoneHoly = (readBuffer[num++] != 0);

            NetMessage.SendData(36, -1, whoAmI, "", playerIndex);
        }
    }
}
