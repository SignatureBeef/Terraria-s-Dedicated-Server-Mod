//using TDSM.Core.Logging;
//using TDSM.Core.Messages.Out;
//using TDSM.Core.ServerCore;
//using Terraria;

//namespace TDSM.Core.Callbacks
//{
//    public static class Net
//    {
//        public static void SendAnglerQuest()
//        {
//            for (int i = 0; i < 255; i++)
//            {
//                if (TDSM.API.Callbacks.Netplay.slots[i].state == SlotState.PLAYING)
//                {
//                    NewNetMessage.SendData(74, i, -1, Main.player[i].name, Main.anglerQuest, 0f, 0f, 0f, 0);
//                }
//            }
//        }

//        public static void sendWater(int x, int y)
//        {
//            for (int i = 0; i < 256; i++)
//            {
//                if ((/*NewNetMessage.buffer[i].broadcast ||*/ TDSM.API.Callbacks.Netplay.slots[i].state >= SlotState.SENDING_TILES) && TDSM.API.Callbacks.Netplay.slots[i].Connected)
//                {
//                    int num = x / 200;
//                    int num2 = y / 150;
//                    if (TDSM.API.Callbacks.Netplay.slots[i].TileSections[num, num2])
//                    {
//                        NewNetMessage.SendData(48, i, -1, "", x, (float)y, 0f, 0f, 0);
//                    }
//                }
//            }
//        }

//        public static void syncPlayers()
//        {
//            bool flag = false;
//            for (int i = 0; i < 255; i++)
//            {
//                int num = 0;
//                if (Main.player[i].active)
//                {
//                    num = 1;
//                }
//                if (TDSM.API.Callbacks.Netplay.slots[i].state == SlotState.PLAYING)
//                {
//                    if (Main.autoShutdown && !flag)
//                    {
//                        string text = TDSM.API.Callbacks.Netplay.slots[i].conn.RemoteAddress;
//                        string a = text;
//                        for (int j = 0; j < text.Length; j++)
//                        {
//                            if (text.Substring(j, 1) == ":")
//                            {
//                                a = text.Substring(0, j);
//                            }
//                        }
//                        if (a == "127.0.0.1")
//                        {
//                            flag = true;
//                        }
//                    }
//                    NewNetMessage.SendData(14, -1, i, "", i, (float)num, 0f, 0f, 0);
//                    NewNetMessage.SendData(4, -1, i, Main.player[i].name, i, 0f, 0f, 0f, 0);
//                    NewNetMessage.SendData(13, -1, i, "", i, 0f, 0f, 0f, 0);
//                    NewNetMessage.SendData(16, -1, i, "", i, 0f, 0f, 0f, 0);
//                    NewNetMessage.SendData(30, -1, i, "", i, 0f, 0f, 0f, 0);
//                    NewNetMessage.SendData(45, -1, i, "", i, 0f, 0f, 0f, 0);
//                    NewNetMessage.SendData(42, -1, i, "", i, 0f, 0f, 0f, 0);
//                    NewNetMessage.SendData(50, -1, i, "", i, 0f, 0f, 0f, 0);
//                    for (int k = 0; k < 59; k++)
//                    {
//                        NewNetMessage.SendData(5, -1, i, Main.player[i].inventory[k].name, i, (float)k, (float)Main.player[i].inventory[k].prefix, 0f, 0);
//                    }
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[0].name, i, 59f, (float)Main.player[i].armor[0].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[1].name, i, 60f, (float)Main.player[i].armor[1].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[2].name, i, 61f, (float)Main.player[i].armor[2].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[3].name, i, 62f, (float)Main.player[i].armor[3].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[4].name, i, 63f, (float)Main.player[i].armor[4].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[5].name, i, 64f, (float)Main.player[i].armor[5].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[6].name, i, 65f, (float)Main.player[i].armor[6].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[7].name, i, 66f, (float)Main.player[i].armor[7].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[8].name, i, 67f, (float)Main.player[i].armor[8].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[9].name, i, 68f, (float)Main.player[i].armor[9].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[10].name, i, 69f, (float)Main.player[i].armor[10].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[11].name, i, 70f, (float)Main.player[i].armor[11].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[12].name, i, 71f, (float)Main.player[i].armor[12].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[13].name, i, 72f, (float)Main.player[i].armor[13].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[14].name, i, 73f, (float)Main.player[i].armor[14].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].armor[15].name, i, 74f, (float)Main.player[i].armor[15].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].dye[0].name, i, 75f, (float)Main.player[i].dye[0].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].dye[1].name, i, 76f, (float)Main.player[i].dye[1].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].dye[2].name, i, 77f, (float)Main.player[i].dye[2].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].dye[3].name, i, 78f, (float)Main.player[i].dye[3].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].dye[4].name, i, 79f, (float)Main.player[i].dye[4].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].dye[5].name, i, 80f, (float)Main.player[i].dye[5].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].dye[6].name, i, 81f, (float)Main.player[i].dye[6].prefix, 0f, 0);
//                    NewNetMessage.SendData(5, -1, i, Main.player[i].dye[7].name, i, 82f, (float)Main.player[i].dye[7].prefix, 0f, 0);
//                    if (!TDSM.API.Callbacks.Netplay.slots[i].announced)
//                    {
//                        TDSM.API.Callbacks.Netplay.slots[i].announced = true;
//                        NewNetMessage.SendData(25, -1, i, Main.player[i].name + " " + Lang.mp[19], 255, 255f, 240f, 20f, 0);
//                        if (Main.dedServ)
//                        {
//                            ProgramLog.Log(Main.player[i].name + " " + Lang.mp[19]);
//                        }
//                    }
//                }
//                else
//                {
//                    num = 0;
//                    NewNetMessage.SendData(14, -1, i, "", i, (float)num, 0f, 0f, 0);
//                    if (TDSM.API.Callbacks.Netplay.slots[i].announced)
//                    {
//                        TDSM.API.Callbacks.Netplay.slots[i].announced = false;
//                        NewNetMessage.SendData(25, -1, i, TDSM.API.Callbacks.Netplay.slots[i].oldName + " " + Lang.mp[20], 255, 255f, 240f, 20f, 0);
//                        if (Main.dedServ)
//                        {
//                            ProgramLog.Log(TDSM.API.Callbacks.Netplay.slots[i].oldName + " " + Lang.mp[20]);
//                        }
//                    }
//                }
//            }
//            bool flag2 = false;
//            for (int l = 0; l < 200; l++)
//            {
//                if (Main.npc[l].active && Main.npc[l].townNPC && NPC.TypeToNum(Main.npc[l].type) != -1)
//                {
//                    if (!flag2 && Main.npc[l].type == 368)
//                    {
//                        flag2 = true;
//                    }
//                    int num2 = 0;
//                    if (Main.npc[l].homeless)
//                    {
//                        num2 = 1;
//                    }
//                    NewNetMessage.SendData(60, -1, -1, "", l, (float)Main.npc[l].homeTileX, (float)Main.npc[l].homeTileY, (float)num2, 0);
//                }
//            }
//            if (flag2)
//            {
//                NetMessage.SendTravelShop();
//            }
//            SendAnglerQuest();
//            if (Main.autoShutdown && !flag)
//            {
//                WorldFile.saveWorld(false);
//                Netplay.disconnect = true;
//            }
//        }

//        public static void AddBan(int plr)
//        {
//            Server.Bans.Add(TDSM.API.Callbacks.Netplay.slots[plr].conn.RemoteAddress);
//            //string text = TDSM.API.Callbacks.Netplay.slots[plr].conn.RemoteAddress;
//            //string value = text;
//            //for (int i = 0; i < text.Length; i++)
//            //{
//            //    if (text.Substring(i, 1) == ":")
//            //    {
//            //        value = text.Substring(0, i);
//            //    }
//            //}
//            //using (StreamWriter streamWriter = new StreamWriter(Netplay.banFile, true))
//            //{
//            //    streamWriter.WriteLine("//" + Main.player[plr].name);
//            //    streamWriter.WriteLine(value);
//            //}
//        }
//    }
//}
