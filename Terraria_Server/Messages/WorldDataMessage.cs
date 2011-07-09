using System;
using System.Text;

namespace Terraria_Server.Messages
{
    public class WorldDataMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.WORLD_DATA;
        }

        public int? GetRequiredNetMode()
        {
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            Main.time = (double)BitConverter.ToInt32(readBuffer, num);
            num += 4;

            Main.dayTime = (readBuffer[num++] == 1);
            Main.moonPhase = (int)readBuffer[num++];
            Main.bloodMoon = ((int)readBuffer[num++] == 1);

            Main.maxTilesX = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            Main.maxTilesY = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            Main.spawnTileX = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            Main.spawnTileY = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            Main.worldSurface = (double)BitConverter.ToInt32(readBuffer, num);
            num += 4;
            Main.rockLayer = (double)BitConverter.ToInt32(readBuffer, num);
            num += 4;
            Main.worldID = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            byte b2 = readBuffer[num++];
            if ((b2 & 1) == 1)
            {
                WorldGen.shadowOrbSmashed = true;
            }
            if ((b2 & 2) == 2)
            {
                NPC.downedBoss1 = true;
            }
            if ((b2 & 4) == 4)
            {
                NPC.downedBoss2 = true;
            }

            if ((b2 & 8) == 8)
            {
                NPC.downedBoss3 = true;
            }

            Main.worldName = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
//            if (Netplay.clientSock.state == 3)
//            {
//                Netplay.clientSock.state = 4;
//            }
        }
    }
}
