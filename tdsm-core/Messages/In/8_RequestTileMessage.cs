using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class RequestTileMessage : SlotMessageHandler
    {
        public RequestTileMessage()
        {
            IgnoredStates = SlotState.ACCEPTED | SlotState.PLAYER_AUTH;
            ValidStates = SlotState.SENDING_WORLD;
        }

        public override Packet GetPacket()
        {
            return Packet.REQUEST_TILE_BLOCK;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num13 = ReadInt32(readBuffer);
            int num14 = ReadInt32(readBuffer);
            bool flag3 = true;
            if (num13 == -1 || num14 == -1)
            {
                flag3 = false;
            }
            else
            {
                if (num13 < 10 || num13 > Main.maxTilesX - 10)
                {
                    flag3 = false;
                }
                else
                {
                    if (num14 < 10 || num14 > Main.maxTilesY - 10)
                    {
                        flag3 = false;
                    }
                }
            }
            int num15 = Netplay.GetSectionX(Main.spawnTileX) - 2;
            int num16 = Netplay.GetSectionY(Main.spawnTileY) - 1;
            int num17 = num15 + 5;
            int num18 = num16 + 3;
            if (num15 < 0)
            {
                num15 = 0;
            }
            if (num17 >= Main.maxSectionsX)
            {
                num17 = Main.maxSectionsX - 1;
            }
            if (num16 < 0)
            {
                num16 = 0;
            }
            if (num18 >= Main.maxSectionsY)
            {
                num18 = Main.maxSectionsY - 1;
            }
            int num19 = (num17 - num15) * (num18 - num16);
            int num20 = -1;
            int num21 = -1;
            if (flag3)
            {
                num13 = Netplay.GetSectionX(num13) - 2;
                num14 = Netplay.GetSectionY(num14) - 1;
                num20 = num13 + 5;
                num21 = num14 + 3;
                if (num13 < 0)
                {
                    num13 = 0;
                }
                if (num20 >= Main.maxSectionsX)
                {
                    num20 = Main.maxSectionsX - 1;
                }
                if (num14 < 0)
                {
                    num14 = 0;
                }
                if (num21 >= Main.maxSectionsY)
                {
                    num21 = Main.maxSectionsY - 1;
                }
                for (int num22 = num13; num22 < num20; num22++)
                {
                    for (int num23 = num14; num23 < num21; num23++)
                    {
                        if (num22 < num15 || num22 >= num17 || num23 < num16 || num23 >= num18)
                        {
                            num19++;
                        }
                    }
                }
            }

            ServerSlot serverSock = Terraria.Netplay.Clients[whoAmI] as ServerSlot;
            if (serverSock.state == SlotState.SENDING_WORLD)
            {
                serverSock.state = SlotState.SENDING_TILES;
            }
            else
                serverSock.Kick("Invalid operation at this state.");

            NewNetMessage.SendData(9, whoAmI, -1, Lang.inter[44], num19, 0f, 0f, 0f, 0);
            Terraria.Netplay.Clients[whoAmI].StatusText2 = "is receiving tile data";
            Terraria.Netplay.Clients[whoAmI].StatusMax += num19;
            for (int num24 = num15; num24 < num17; num24++)
            {
                for (int num25 = num16; num25 < num18; num25++)
                {
                    NewNetMessage.SendSection(whoAmI, num24, num25, false);
                }
            }
            if (flag3)
            {
                for (int num26 = num13; num26 < num20; num26++)
                {
                    for (int num27 = num14; num27 < num21; num27++)
                    {
                        NewNetMessage.SendSection(whoAmI, num26, num27, true);
                    }
                }
                NewNetMessage.SendData(11, whoAmI, -1, String.Empty, num13, (float)num14, (float)(num20 - 1), (float)(num21 - 1), 0);
            }
            NewNetMessage.SendData(11, whoAmI, -1, String.Empty, num15, (float)num16, (float)(num17 - 1), (float)(num18 - 1), 0);
            for (int num28 = 0; num28 < 400; num28++)
            {
                if (Main.item[num28].active)
                {
                    NewNetMessage.SendData(21, whoAmI, -1, String.Empty, num28, 0f, 0f, 0f, 0);
                    NewNetMessage.SendData(22, whoAmI, -1, String.Empty, num28, 0f, 0f, 0f, 0);
                }
            }
            for (int num29 = 0; num29 < 200; num29++)
            {
                if (Main.npc[num29].active)
                {
                    System.Diagnostics.Debug.WriteLine("Sending 23");
                    NewNetMessage.SendData(23, whoAmI, -1, String.Empty, num29, 0f, 0f, 0f, 0);
                }
            }
            for (int num30 = 0; num30 < 1000; num30++)
            {
                if (Main.projectile[num30].active && (Main.projPet[Main.projectile[num30].type] || Main.projectile[num30].netImportant))
                {
                    NewNetMessage.SendData(27, whoAmI, -1, String.Empty, num30, 0f, 0f, 0f, 0);
                }
            }
            NewNetMessage.SendData(49, whoAmI, -1, String.Empty, 0, 0f, 0f, 0f, 0);
            NewNetMessage.SendData(57, whoAmI, -1, String.Empty, 0, 0f, 0f, 0f, 0);
            NewNetMessage.SendData(7, whoAmI, -1, String.Empty, 0, 0f, 0f, 0f, 0);
        }
    }
}
