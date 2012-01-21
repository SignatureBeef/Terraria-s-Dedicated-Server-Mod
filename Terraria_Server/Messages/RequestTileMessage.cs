using System;

namespace Terraria_Server.Messages
{
    public class RequestTileMessage : SlotMessageHandler
    {
		public RequestTileMessage ()
		{
			IgnoredStates = SlotState.ACCEPTED | SlotState.PLAYER_AUTH;
			ValidStates = SlotState.SENDING_WORLD;
		}

        public override Packet GetPacket()
        {
            return Packet.REQUEST_TILE_BLOCK;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
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

            ServerSlot serverSock = NetPlay.slots[whoAmI];
            if (serverSock.state == SlotState.SENDING_WORLD)
            {
                serverSock.state = SlotState.SENDING_TILES;
            }
            else
                serverSock.Kick ("Invalid operation at this state.");

            NetMessage.SendData(9, whoAmI, -1, "Receiving tile data", num10);
            serverSock.statusText2 = "is receiving tile data";
            serverSock.statusMax += num10;
            int sectionX = NetPlay.GetSectionX(Main.spawnTileX);
            int sectionY = NetPlay.GetSectionY(Main.spawnTileY);

            for (int x = sectionX - 1; x < sectionX + 2; x++)
            {
                for (int y = sectionY - 1; y < sectionY + 2; y++)
                {
                    NetMessage.SendSection(whoAmI, x, y);
                }
            }

            if (flag3)
            {
                num8 = NetPlay.GetSectionX(num8);
                num9 = NetPlay.GetSectionY(num9);
                for (int num11 = num8 - 1; num11 < num8 + 2; num11++)
                {
                    for (int num12 = num9 - 1; num12 < num9 + 2; num12++)
                    {
                        NetMessage.SendSection(whoAmI, num11, num12);
                    }
                }
                NetMessage.SendData(11, whoAmI, -1, "", num8 - 1, (float)(num9 - 1), (float)(num8 + 1), (float)(num9 + 1));
            }

            NetMessage.SendData(11, whoAmI, -1, "", sectionX - 1, (float)(sectionY - 1), (float)(sectionX + 1), (float)(sectionY + 1));

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
					NetMessage.SendData(23, whoAmI, -1, "", npcIndex);
            }

            NetMessage.SendData(49, whoAmI);
			NetMessage.SendData(57, whoAmI);

			//Send NPC Names...

			foreach (var npcId in new int[] { 17, 18, 19, 20, 22, 38, 54, 107, 108, 124 })
				NetMessage.SendData(56, whoAmI, -1, String.Empty, npcId);
        }
    }
}
