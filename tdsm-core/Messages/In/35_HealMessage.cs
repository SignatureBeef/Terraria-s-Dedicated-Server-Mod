using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class HealMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.HEAL_PLAYER;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = (int)ReadByte(readBuffer);

			if (playerIndex != whoAmI && Entry.EnableCheatProtection)
            {
                Terraria.Netplay.serverSock[whoAmI].Kick("Cheating detected (HEAL_PLAYER forgery).");
                return;
            }

            playerIndex = whoAmI;


            int amount = (int)ReadInt16(readBuffer);
            if (Main.ServerSideCharacter)
            {
                Main.player[playerIndex].HealEffect(amount, true);
            }

            NewNetMessage.SendData(35, -1, whoAmI, String.Empty, playerIndex, (float)amount, 0f, 0f, 0);
        }
    }
}
