using System;

namespace Terraria_Server.Messages
{
    public class DamageNPCMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.DAMAGE_NPC;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            short npcIndex = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            short damage = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            float knockback = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            int direction = (int)(readBuffer[num] - 1);

            NPC npc = Main.npcs[(int)npcIndex];
            if (damage >= 0)
            {
                npc.StrikeNPC((int)damage, knockback, direction);
            }
            else
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.Active = false;
            }
            
            NetMessage.SendData(28, -1, whoAmI, "", (int)npcIndex, (float)damage, knockback, (float)direction);
            NetMessage.SendData(23, -1, -1, "", (int)npcIndex);
        }
    }
}
