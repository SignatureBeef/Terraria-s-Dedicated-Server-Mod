using System;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PlayerManaUpdateMessage : SlotMessageHandler
    {
        public PlayerManaUpdateMessage()
        {
            IgnoredStates = SlotState.ACCEPTED | SlotState.PLAYER_AUTH;
            ValidStates = SlotState.ASSIGNING_SLOT | SlotState.PLAYING;
        }

        public override Packet GetPacket()
        {
            return Packet.PLAYER_MANA_UPDATE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = (int)ReadByte(readBuffer);

            if (playerIndex != whoAmI)
            {
                tdsm.api.Callbacks.Netplay.slots[whoAmI].Kick("Cheating detected (PLAYER_MANA_UPDATE forgery).");
                return;
            }

            playerIndex = whoAmI;

            int statMana = (int)ReadInt16(readBuffer);
            int statManaMax = (int)ReadInt16(readBuffer);

            Main.player[playerIndex].statMana = statMana;
            Main.player[playerIndex].statManaMax = statManaMax;

            //if (Main.player[playerIndex].HasHackedData())
            //{
            //    Main.player[playerIndex].Kick("No Hacked Mana is allowed.");
            //}

            NewNetMessage.SendData((int)Packet.PLAYER_MANA_UPDATE, -1, whoAmI, String.Empty, playerIndex);
        }
    }
}
