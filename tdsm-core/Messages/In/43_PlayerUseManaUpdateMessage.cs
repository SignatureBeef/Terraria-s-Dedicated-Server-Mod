using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PlayerUseManaUpdateMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_USE_MANA_UPDATE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = (int)ReadByte(readBuffer);

            if (playerIndex != whoAmI)
            {
                tdsm.api.Callbacks.NetplayCallback.slots[whoAmI].Kick("Cheating detected (PLAYER_USE_MANA_UPDATE forgery).");
                return;
            }

            playerIndex = whoAmI;
            num++;

            int manaAmount = (int)ReadInt16(readBuffer);
            num += 2;

            if (playerIndex != Main.myPlayer)
            {
                Main.player[playerIndex].ManaEffect(manaAmount);
            }

            NewNetMessage.SendData(43, -1, whoAmI, String.Empty, playerIndex, (float)manaAmount);
        }
    }
}
