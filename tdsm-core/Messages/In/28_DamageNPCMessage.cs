using System;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class DamageNPCMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.DAMAGE_NPC;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num75 = (int)ReadInt16(readBuffer);
            int num76 = (int)ReadInt16(readBuffer);
            float num77 = ReadSingle(readBuffer);
            int num78 = (int)(ReadByte(readBuffer) - 1);
            byte b5 = ReadByte(readBuffer);

            if (num76 >= 0)
            {
                Main.npc[num75].StrikeNPC(num76, num77, num78, b5 == 1, false);
            }
            else
            {
                Main.npc[num75].life = 0;
                Main.npc[num75].HitEffect(0, 10.0);
                Main.npc[num75].active = false;
            }

            NewNetMessage.SendData(28, -1, whoAmI, String.Empty, num75, (float)num76, num77, (float)num78, (int)b5);
            if (Main.npc[num75].life <= 0)
            {
                NewNetMessage.SendData(23, -1, -1, String.Empty, num75, 0f, 0f, 0f, 0);
                return;
            }
            Main.npc[num75].netUpdate = true;
        }
    }
}
