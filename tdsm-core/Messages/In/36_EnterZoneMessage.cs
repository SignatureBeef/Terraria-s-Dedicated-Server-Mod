using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class EnterZoneMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.ENTER_ZONE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = (int)ReadByte(readBuffer);

			if (playerIndex != whoAmI && Entry.EnableCheatProtection)
            {
                Terraria.Netplay.Clients[whoAmI].Kick("Cheating detected (ENTER_ZONE forgery).");
                return;
            }

            playerIndex = whoAmI;

            Player player9 = Main.player[playerIndex];

            //Todo refactor
            BitsByte bitsByte11 = ReadByte(readBuffer);
            player9.zonw = bitsByte11[0];
            player9.zoneMeteor = bitsByte11[1];
            player9.zoneDungeon = bitsByte11[2];
            player9.zoneJungle = bitsByte11[3];
            player9.zoneHoly = bitsByte11[4];
            player9.zoneSnow = bitsByte11[5];
            player9.zoneBlood = bitsByte11[6];
            player9.zoneCandle = bitsByte11[7];

            NewNetMessage.SendData(36, -1, whoAmI, String.Empty, playerIndex, 0f, 0f, 0f, 0);

        }
    }
}
