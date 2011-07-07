using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class NPCInfoMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.NPC_INFO;
        }

        public int? GetRequiredNetMode()
        {
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            short npcIndex = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            float x = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float y = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float vX = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float vY = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            int target = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int direction = (int)(readBuffer[num++] - 1);
            byte arg_2465_0 = readBuffer[num++];
            int life = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;

            float[] aiInfo = new float[NPC.MAX_AI];
            for (int i = 0; i < NPC.MAX_AI; i++)
            {
                aiInfo[i] = BitConverter.ToSingle(readBuffer, num);
                num += 4;
            }

            String npcName = Encoding.ASCII.GetString(readBuffer, num, length - num + start);

            NPC npc = Main.npcs[(int)npcIndex];
            if (!npc.Active || npc.Name != npcName)
            {
                npc.Active = true;
                npc.SetDefaults(npcName);
            }

            npc.Position.X = x;
            npc.Position.Y = y;
            npc.Velocity.X = vX;
            npc.Velocity.Y = vY;
            npc.target = target;
            npc.direction = direction;
            npc.life = life;

            if (life <= 0)
            {
                npc.Active = false;
            }

            for (int i = 0; i < NPC.MAX_AI; i++)
            {
                npc.ai[i] = aiInfo[i];
            }
        }
    }
}
