using System;

namespace Terraria_Server.Messages
{
    public class RequestTileMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.REQUEST_TILE_BLOCK;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int num8 = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int num9 = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            bool flag3 = !(num8 == -1 
                || num8 < 10 
                || num8 > Main.maxTilesX - 10 
                || num9 == -1
                || num9 < 10
                || num9 > Main.maxTilesY - 10);
            
            int num10 = 1350;
            if (flag3)
            {
                num10 *= 2;
            }

            ServerSlot serverSock = Netplay.slots[whoAmI];
            if (serverSock.state == SlotState.SENDING_WORLD)
            {
                serverSock.state = SlotState.SENDING_TILES;
            }

            NetMessage.SendData(9, whoAmI, -1, "Receiving tile data", num10);
            serverSock.statusText2 = "is receiving tile data";
            serverSock.statusMax += num10;
            int sectionX = Netplay.GetSectionX(Main.spawnTileX);
            int sectionY = Netplay.GetSectionY(Main.spawnTileY);

            for (int x = sectionX - 2; x < sectionX + 3; x++)
            {
                for (int y = sectionY - 1; y < sectionY + 2; y++)
                {
                    NetMessage.SendSection(whoAmI, x, y);
                }
            }

            if (flag3)
            {
                num8 = Netplay.GetSectionX(num8);
                num9 = Netplay.GetSectionY(num9);
                for (int num11 = num8 - 2; num11 < num8 + 3; num11++)
                {
                    for (int num12 = num9 - 1; num12 < num9 + 2; num12++)
                    {
                        NetMessage.SendSection(whoAmI, num11, num12);
                    }
                }
                NetMessage.SendData(11, whoAmI, -1, "", num8 - 2, (float)(num9 - 1), (float)(num8 + 2), (float)(num9 + 1));
            }

            NetMessage.SendData(11, whoAmI, -1, "", sectionX - 2, (float)(sectionY - 1), (float)(sectionX + 2), (float)(sectionY + 1));

            //Can't switch to a for each because there are 201 items.
            for (int itemIndex = 0; itemIndex < 200; itemIndex++)
            {
                if (Main.item[itemIndex].Active)
                {
                    NetMessage.SendData(21, whoAmI, -1, "", itemIndex);
                    NetMessage.SendData(22, whoAmI, -1, "", itemIndex);
                }
            }
            
            //Can't switch to a for each because there are 1001 NPCs.
            for (int npcIndex = 0; npcIndex < NPC.MAX_NPCS; npcIndex++)
            {
                if (Main.npcs[npcIndex].Active)
                {
                    NetMessage.SendData(23, whoAmI, -1, "", npcIndex);
                }
            }
            NetMessage.SendData(49, whoAmI);
        }
    }
}
