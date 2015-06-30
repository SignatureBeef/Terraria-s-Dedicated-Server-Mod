using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class NPCAddBuffMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.NPC_ADD_BUFF;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            var npcId = ReadInt16(readBuffer);
            var type = ReadByte(readBuffer);
            var time = ReadInt16(readBuffer);

            Main.npc[npcId].AddBuff(type, time, true);

            NewNetMessage.SendData(54, -1, -1, String.Empty, npcId);
        }
    }
}

