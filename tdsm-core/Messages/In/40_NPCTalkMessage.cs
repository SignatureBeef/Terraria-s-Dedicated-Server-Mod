using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class NPCTalkMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.NPC_TALK;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            ReadByte(readBuffer);
            int playerIndex = whoAmI;

            int talkNPC = (int)ReadInt16(readBuffer);

            Main.player[playerIndex].talkNPC = talkNPC;

            NewNetMessage.SendData(40, -1, whoAmI, String.Empty, playerIndex);
        }
    }
}
