using System;

namespace Terraria_Server.Messages
{
    public class DamageNPCMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.DAMAGE_NPC;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            short npcIndex = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            short damage = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            float knockback = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            int direction = (int)(readBuffer[num] - 1);
            num += 1;
            byte crit = readBuffer[num];

            NPC npc = Main.npcs[(int)npcIndex];
            if (damage >= 0)
            {
                npc.StrikeNPC(Main.players[whoAmI], (int)damage, knockback, direction, crit == 1);
            }
            else
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.Active = false;
            }            

			if (npc.life <= 0)
			{
				NetMessage.SendData(28, -1, whoAmI, String.Empty, (int)npcIndex, (float)damage, knockback, (float)direction, crit);
				NetMessage.SendData(23, -1, -1, String.Empty, (int)npcIndex);
				return;
			}
			
			NetMessage.SendData(28, -1, whoAmI, String.Empty, (int)npcIndex, (float)damage, knockback, (float)direction, crit);
			npc.netUpdate = true;
        }
    }
}
