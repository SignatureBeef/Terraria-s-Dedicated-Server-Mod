using System;
using System.IO;
using System.Threading;
using tdsm.api.Plugin;


#if Full_API

#endif

namespace tdsm.api.Callbacks
{
    public abstract class IAPISocket
    {
        public int whoAmI;
        public string statusText2;
        public int statusCount;
        public int statusMax;
        public volatile string remoteAddress;
        public bool[,] tileSection;
        public string statusText = String.Empty;
        public bool announced;
        public string name = "Anonymous";
        public string oldName = String.Empty;
        public float spamProjectile;
        public float spamAddBlock;
        public float spamDelBlock;
        public float spamWater;
        public float spamProjectileMax = 100f;
        public float spamAddBlockMax = 100f;
        public float spamDelBlockMax = 500f;
        public float spamWaterMax = 50f;

        public abstract bool IsPlaying();
        public abstract bool CanSendWater();
        public abstract string RemoteAddress();
    }

    public static class NetplayCallback
    {
        public static IAPISocket[] slots;// = new IAPISocket[256];

        public static void StartServer(object state)
        {
#if Full_API
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.StartDefaultServer();
            HookPoints.StartDefaultServer.Invoke(ref ctx, ref args);

            if (ctx.Result != HookResult.IGNORE)
            {
                Console.Write("Starting server...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(Terraria.Netplay.ServerLoop), 1);
                Tools.WriteLine("Ok");
            }
#endif
        }

        public static void SendAnglerQuest()
        {
#if Full_API
            if (slots != null)
                for (int i = 0; i < 255; i++)
                {
                    if (slots[i].IsPlaying())
                    {
                        NetMessageCallback.SendData(74, i, -1, Terraria.Main.player[i].name, Terraria.Main.anglerQuest, 0f, 0f, 0f, 0);
                    }
                }
#endif
        }

        public static void sendWater(int x, int y)
        {
#if Full_API
            if (slots != null)
                for (int i = 0; i < 256; i++)
                {
                    //if ((/*NetMessage.buffer[i].broadcast ||*/ Server.slots[i].state >= SlotState.SENDING_TILES) && Server.slots[i].Connected)
                    if (slots[i].CanSendWater())
                    {
                        int num = x / 200;
                        int num2 = y / 150;
                        if (slots[i].tileSection[num, num2])
                        {
                            NetMessageCallback.SendData(48, i, -1, "", x, (float)y, 0f, 0f, 0);
                        }
                    }
                }
#endif
        }

        public static void syncPlayers()
        {
#if Full_API
            bool flag = false;
            if (slots != null)
                for (int i = 0; i < 255; i++)
                {
                    int num = 0;
                    if (Terraria.Main.player[i].active)
                    {
                        num = 1;
                    }
                    if (slots[i].IsPlaying())
                    {
                        if (Terraria.Main.autoShutdown && !flag)
                        {
                            string text = slots[i].RemoteAddress();
                            string a = text;
                            for (int j = 0; j < text.Length; j++)
                            {
                                if (text.Substring(j, 1) == ":")
                                {
                                    a = text.Substring(0, j);
                                }
                            }
                            if (a == "127.0.0.1")
                            {
                                flag = true;
                            }
                        }
                        NetMessageCallback.SendData(14, -1, i, "", i, (float)num, 0f, 0f, 0);
                        NetMessageCallback.SendData(4, -1, i, Terraria.Main.player[i].name, i, 0f, 0f, 0f, 0);
                        NetMessageCallback.SendData(13, -1, i, "", i, 0f, 0f, 0f, 0);
                        NetMessageCallback.SendData(16, -1, i, "", i, 0f, 0f, 0f, 0);
                        NetMessageCallback.SendData(30, -1, i, "", i, 0f, 0f, 0f, 0);
                        NetMessageCallback.SendData(45, -1, i, "", i, 0f, 0f, 0f, 0);
                        NetMessageCallback.SendData(42, -1, i, "", i, 0f, 0f, 0f, 0);
                        NetMessageCallback.SendData(50, -1, i, "", i, 0f, 0f, 0f, 0);
                        for (int k = 0; k < 59; k++)
                        {
                            NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].inventory[k].name, i, (float)k, (float)Terraria.Main.player[i].inventory[k].prefix, 0f, 0);
                        }
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[0].name, i, 59f, (float)Terraria.Main.player[i].armor[0].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[1].name, i, 60f, (float)Terraria.Main.player[i].armor[1].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[2].name, i, 61f, (float)Terraria.Main.player[i].armor[2].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[3].name, i, 62f, (float)Terraria.Main.player[i].armor[3].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[4].name, i, 63f, (float)Terraria.Main.player[i].armor[4].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[5].name, i, 64f, (float)Terraria.Main.player[i].armor[5].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[6].name, i, 65f, (float)Terraria.Main.player[i].armor[6].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[7].name, i, 66f, (float)Terraria.Main.player[i].armor[7].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[8].name, i, 67f, (float)Terraria.Main.player[i].armor[8].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[9].name, i, 68f, (float)Terraria.Main.player[i].armor[9].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[10].name, i, 69f, (float)Terraria.Main.player[i].armor[10].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[11].name, i, 70f, (float)Terraria.Main.player[i].armor[11].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[12].name, i, 71f, (float)Terraria.Main.player[i].armor[12].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[13].name, i, 72f, (float)Terraria.Main.player[i].armor[13].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[14].name, i, 73f, (float)Terraria.Main.player[i].armor[14].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].armor[15].name, i, 74f, (float)Terraria.Main.player[i].armor[15].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].dye[0].name, i, 75f, (float)Terraria.Main.player[i].dye[0].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].dye[1].name, i, 76f, (float)Terraria.Main.player[i].dye[1].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].dye[2].name, i, 77f, (float)Terraria.Main.player[i].dye[2].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].dye[3].name, i, 78f, (float)Terraria.Main.player[i].dye[3].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].dye[4].name, i, 79f, (float)Terraria.Main.player[i].dye[4].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].dye[5].name, i, 80f, (float)Terraria.Main.player[i].dye[5].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].dye[6].name, i, 81f, (float)Terraria.Main.player[i].dye[6].prefix, 0f, 0);
                        NetMessageCallback.SendData(5, -1, i, Terraria.Main.player[i].dye[7].name, i, 82f, (float)Terraria.Main.player[i].dye[7].prefix, 0f, 0);
                        if (!slots[i].announced)
                        {
                            slots[i].announced = true;
                            NetMessageCallback.SendData(25, -1, i, Terraria.Main.player[i].name + " " + Terraria.Lang.mp[19], 255, 255f, 240f, 20f, 0);
                            if (Terraria.Main.dedServ)
                            {
                                Tools.WriteLine(Terraria.Main.player[i].name + " " + Terraria.Lang.mp[19]);
                            }
                        }
                    }
                    else
                    {
                        num = 0;
                        NetMessageCallback.SendData(14, -1, i, "", i, (float)num, 0f, 0f, 0);
                        if (slots[i].announced)
                        {
                            slots[i].announced = false;
                            NetMessageCallback.SendData(25, -1, i, slots[i].oldName + " " + Terraria.Lang.mp[20], 255, 255f, 240f, 20f, 0);
                            if (Terraria.Main.dedServ)
                            {
                                Tools.WriteLine(slots[i].oldName + " " + Terraria.Lang.mp[20]);
                            }
                        }
                    }
                }
            bool flag2 = false;
            for (int l = 0; l < 200; l++)
            {
                if (Terraria.Main.npc[l].active && Terraria.Main.npc[l].townNPC && Terraria.NPC.TypeToNum(Terraria.Main.npc[l].type) != -1)
                {
                    if (!flag2 && Terraria.Main.npc[l].type == 368)
                    {
                        flag2 = true;
                    }
                    int num2 = 0;
                    if (Terraria.Main.npc[l].homeless)
                    {
                        num2 = 1;
                    }
                    NetMessageCallback.SendData(60, -1, -1, "", l, (float)Terraria.Main.npc[l].homeTileX, (float)Terraria.Main.npc[l].homeTileY, (float)num2, 0);
                }
            }
            if (flag2)
            {
                Terraria.NetMessage.SendTravelShop();
            }
            SendAnglerQuest();
            if (Terraria.Main.autoShutdown && !flag)
            {
                Terraria.WorldFile.saveWorld(false);
                Terraria.Netplay.disconnect = true;
            }
#endif
        }

        public static void AddBan(int plr)
        {
#if Full_API
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.AddBan()
            {
                RemoteAddress = slots[plr].RemoteAddress()
            };

            HookPoints.AddBan.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.DEFAULT)
            {
                string remote = slots[plr].RemoteAddress();
                string ip = remote;
                for (int i = 0; i < remote.Length; i++)
                {
                    if (remote.Substring(i, 1) == ":")
                    {
                        ip = remote.Substring(0, i);
                    }
                }
                using (StreamWriter streamWriter = new StreamWriter(Terraria.Netplay.banFile, true))
                {
                    streamWriter.WriteLine("//" + Terraria.Main.player[plr].name);
                    streamWriter.WriteLine(ip);
                }
            }
#endif
        }
    }
}