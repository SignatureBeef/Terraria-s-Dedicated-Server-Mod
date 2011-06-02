using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Terraria_Server
{
    public class Server //: NetPlay
    {
        private int playerCap = 0;
        public static int processingPlayer = 0;
        private NetPlay netplay = null;
        
        private World world = null;

        public Server(int PlayerCap, World World)
        {
            playerCap = PlayerCap;
            world = World;
            world.setServer(this);
            //this.setWorld(World);
        }

        public World getWorld()
        {
            return netplay.world;
        }

        public NetPlay getNetPlay()
        {
            return netplay;
        }

        public void setWorld(World World)
        {
            netplay.world = World;
        }

        public Server getServer()
        {
            return this;
        }

        protected void getAuth()
        {
            try
            {
                string requestUriString = "";
                StringBuilder stringBuilder = new StringBuilder();
                byte[] numArray = new byte[8192];
                Stream responseStream = WebRequest.Create(requestUriString).GetResponse().GetResponseStream();
                int count;
                do
                {
                    count = responseStream.Read(numArray, 0, numArray.Length);
                    if (count != 0)
                    {
                        string @string = Encoding.ASCII.GetString(numArray, 0, count);
                        stringBuilder.Append(@string);
                    }
                }
                while (count > 0);
                if (((object)stringBuilder).ToString() == "")
                    Statics.webAuth = true;
            }
            catch
            {
                //this.QuitGame();
            }
        }

        public static void setWorldSize()
        {
            Statics.bottomWorld = Statics.maxTilesY * 0x10;
            Statics.rightWorld = Statics.maxTilesX * 0x10;
            Statics.maxSectionsX = Statics.maxTilesX / 200;
            Statics.maxSectionsY = Statics.maxTilesY / 150;
        }

        public void Initialize()
        {
            Statics.netMode = 2;
            setWorldSize();
            if (Statics.webProtect)
            {
                this.getAuth();
                while (!Statics.webAuth)
                {
                    Console.WriteLine("Not Authenticated");
                }
                    //this.Exit();
            }
            if (Statics.rand == null)
                Statics.rand = new Random((int)DateTime.Now.Ticks);
            if (WorldGen.genRand == null)
                WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
            Statics.tileSolid[0] = true;
            Statics.tileBlockLight[0] = true;
            Statics.tileSolid[1] = true;
            Statics.tileBlockLight[1] = true;
            Statics.tileSolid[2] = true;
            Statics.tileBlockLight[2] = true;
            Statics.tileSolid[3] = false;
            Statics.tileNoAttach[3] = true;
            Statics.tileNoFail[3] = true;
            Statics.tileSolid[4] = false;
            Statics.tileNoAttach[4] = true;
            Statics.tileNoFail[4] = true;
            Statics.tileNoFail[24] = true;
            Statics.tileSolid[5] = false;
            Statics.tileSolid[6] = true;
            Statics.tileBlockLight[6] = true;
            Statics.tileSolid[7] = true;
            Statics.tileBlockLight[7] = true;
            Statics.tileSolid[8] = true;
            Statics.tileBlockLight[8] = true;
            Statics.tileSolid[9] = true;
            Statics.tileBlockLight[9] = true;
            Statics.tileBlockLight[10] = true;
            Statics.tileSolid[10] = true;
            Statics.tileNoAttach[10] = true;
            Statics.tileBlockLight[10] = true;
            Statics.tileSolid[11] = false;
            Statics.tileSolidTop[19] = true;
            Statics.tileSolid[19] = true;
            Statics.tileSolid[22] = true;
            Statics.tileSolid[23] = true;
            Statics.tileSolid[25] = true;
            Statics.tileSolid[30] = true;
            Statics.tileNoFail[32] = true;
            Statics.tileBlockLight[32] = true;
            Statics.tileSolid[37] = true;
            Statics.tileBlockLight[37] = true;
            Statics.tileSolid[38] = true;
            Statics.tileBlockLight[38] = true;
            Statics.tileSolid[39] = true;
            Statics.tileBlockLight[39] = true;
            Statics.tileSolid[40] = true;
            Statics.tileBlockLight[40] = true;
            Statics.tileSolid[41] = true;
            Statics.tileBlockLight[41] = true;
            Statics.tileSolid[43] = true;
            Statics.tileBlockLight[43] = true;
            Statics.tileSolid[44] = true;
            Statics.tileBlockLight[44] = true;
            Statics.tileSolid[45] = true;
            Statics.tileBlockLight[45] = true;
            Statics.tileSolid[46] = true;
            Statics.tileBlockLight[46] = true;
            Statics.tileSolid[47] = true;
            Statics.tileBlockLight[47] = true;
            Statics.tileSolid[48] = true;
            Statics.tileBlockLight[48] = true;
            Statics.tileSolid[53] = true;
            Statics.tileBlockLight[53] = true;
            Statics.tileSolid[54] = true;
            Statics.tileBlockLight[52] = true;
            Statics.tileSolid[56] = true;
            Statics.tileBlockLight[56] = true;
            Statics.tileSolid[57] = true;
            Statics.tileBlockLight[57] = true;
            Statics.tileSolid[58] = true;
            Statics.tileBlockLight[58] = true;
            Statics.tileSolid[59] = true;
            Statics.tileBlockLight[59] = true;
            Statics.tileSolid[60] = true;
            Statics.tileBlockLight[60] = true;
            Statics.tileSolid[63] = true;
            Statics.tileBlockLight[63] = true;
            Statics.tileStone[63] = true;
            Statics.tileSolid[64] = true;
            Statics.tileBlockLight[64] = true;
            Statics.tileStone[64] = true;
            Statics.tileSolid[65] = true;
            Statics.tileBlockLight[65] = true;
            Statics.tileStone[65] = true;
            Statics.tileSolid[66] = true;
            Statics.tileBlockLight[66] = true;
            Statics.tileStone[66] = true;
            Statics.tileSolid[67] = true;
            Statics.tileBlockLight[67] = true;
            Statics.tileStone[67] = true;
            Statics.tileSolid[68] = true;
            Statics.tileBlockLight[68] = true;
            Statics.tileStone[68] = true;
            Statics.tileSolid[75] = true;
            Statics.tileBlockLight[75] = true;
            Statics.tileSolid[76] = true;
            Statics.tileBlockLight[76] = true;
            Statics.tileSolid[70] = true;
            Statics.tileBlockLight[70] = true;
            Statics.tileBlockLight[51] = true;
            Statics.tileNoFail[50] = true;
            Statics.tileNoAttach[50] = true;
            Statics.tileDungeon[41] = true;
            Statics.tileDungeon[43] = true;
            Statics.tileDungeon[44] = true;
            Statics.tileBlockLight[30] = true;
            Statics.tileBlockLight[25] = true;
            Statics.tileBlockLight[23] = true;
            Statics.tileBlockLight[22] = true;
            Statics.tileBlockLight[62] = true;
            Statics.tileSolidTop[18] = true;
            Statics.tileSolidTop[14] = true;
            Statics.tileSolidTop[16] = true;
            Statics.tileNoAttach[20] = true;
            Statics.tileNoAttach[19] = true;
            Statics.tileNoAttach[13] = true;
            Statics.tileNoAttach[14] = true;
            Statics.tileNoAttach[15] = true;
            Statics.tileNoAttach[16] = true;
            Statics.tileNoAttach[17] = true;
            Statics.tileNoAttach[18] = true;
            Statics.tileNoAttach[19] = true;
            Statics.tileNoAttach[21] = true;
            Statics.tileNoAttach[27] = true;
            Statics.tileFrameImportant[3] = true;
            Statics.tileFrameImportant[5] = true;
            Statics.tileFrameImportant[10] = true;
            Statics.tileFrameImportant[11] = true;
            Statics.tileFrameImportant[12] = true;
            Statics.tileFrameImportant[13] = true;
            Statics.tileFrameImportant[14] = true;
            Statics.tileFrameImportant[15] = true;
            Statics.tileFrameImportant[16] = true;
            Statics.tileFrameImportant[17] = true;
            Statics.tileFrameImportant[18] = true;
            Statics.tileFrameImportant[20] = true;
            Statics.tileFrameImportant[21] = true;
            Statics.tileFrameImportant[24] = true;
            Statics.tileFrameImportant[26] = true;
            Statics.tileFrameImportant[27] = true;
            Statics.tileFrameImportant[28] = true;
            Statics.tileFrameImportant[29] = true;
            Statics.tileFrameImportant[31] = true;
            Statics.tileFrameImportant[33] = true;
            Statics.tileFrameImportant[34] = true;
            Statics.tileFrameImportant[35] = true;
            Statics.tileFrameImportant[36] = true;
            Statics.tileFrameImportant[42] = true;
            Statics.tileFrameImportant[50] = true;
            Statics.tileFrameImportant[55] = true;
            Statics.tileFrameImportant[61] = true;
            Statics.tileFrameImportant[71] = true;
            Statics.tileFrameImportant[72] = true;
            Statics.tileFrameImportant[73] = true;
            Statics.tileFrameImportant[74] = true;
            Statics.tileFrameImportant[77] = true;
            Statics.tileFrameImportant[78] = true;
            Statics.tileFrameImportant[79] = true;
            Statics.tileTable[14] = true;
            Statics.tileTable[18] = true;
            Statics.tileTable[19] = true;
            Statics.tileWaterDeath[4] = true;
            Statics.tileWaterDeath[51] = true;
            Statics.tileLavaDeath[3] = true;
            Statics.tileLavaDeath[5] = true;
            Statics.tileLavaDeath[10] = true;
            Statics.tileLavaDeath[11] = true;
            Statics.tileLavaDeath[12] = true;
            Statics.tileLavaDeath[13] = true;
            Statics.tileLavaDeath[14] = true;
            Statics.tileLavaDeath[15] = true;
            Statics.tileLavaDeath[16] = true;
            Statics.tileLavaDeath[17] = true;
            Statics.tileLavaDeath[18] = true;
            Statics.tileLavaDeath[19] = true;
            Statics.tileLavaDeath[20] = true;
            Statics.tileLavaDeath[27] = true;
            Statics.tileLavaDeath[28] = true;
            Statics.tileLavaDeath[29] = true;
            Statics.tileLavaDeath[32] = true;
            Statics.tileLavaDeath[33] = true;
            Statics.tileLavaDeath[34] = true;
            Statics.tileLavaDeath[35] = true;
            Statics.tileLavaDeath[36] = true;
            Statics.tileLavaDeath[42] = true;
            Statics.tileLavaDeath[49] = true;
            Statics.tileLavaDeath[50] = true;
            Statics.tileLavaDeath[52] = true;
            Statics.tileLavaDeath[55] = true;
            Statics.tileLavaDeath[61] = true;
            Statics.tileLavaDeath[62] = true;
            Statics.tileLavaDeath[69] = true;
            Statics.tileLavaDeath[71] = true;
            Statics.tileLavaDeath[72] = true;
            Statics.tileLavaDeath[73] = true;
            Statics.tileLavaDeath[74] = true;
            Statics.tileLavaDeath[78] = true;
            Statics.tileLavaDeath[79] = true;
            Statics.wallHouse[1] = true;
            Statics.wallHouse[4] = true;
            Statics.wallHouse[5] = true;
            Statics.wallHouse[6] = true;
            Statics.wallHouse[10] = true;
            Statics.wallHouse[11] = true;
            Statics.wallHouse[12] = true;


            //for (int i = 0; i < Main.maxMenuItems; i++)
            //{
            //    this.menuItemScale[i] = 0.8f;
            //}
            for (int j = 0; j < 2000; j++)
            {
                world.getDust()[j] = new Dust();
            }
            for (int k = 0; k < 201; k++)
            {
                world.getItemList()[k] = new Item();
            }
            for (int l = 0; l < 1001; l++)
            {
                world.getNPCs()[l] = new NPC();
                world.getNPCs()[l].whoAmI = l;
            }
            for (int m = 0; m < 9; m++)
            {
                world.getPlayerList()[m] = new Player(world);
            }
            for (int n = 0; n < 1001; n++)
            {
                world.getProjectile()[n] = new Projectile();
            }
            for (int num2 = 0; num2 < 201; num2++)
            {
                world.getGore()[num2] = new Gore();
            }
            //for (int num3 = 0; num3 < 100; num3++)
            //{
                //Main.cloud[num3] = new Cloud();
            //}
            //for (int num4 = 0; num4 < 100; num4++)
            //{
            //    Main.combatText[num4] = new CombatText();
            //}
            for (int num5 = 0; num5 < Recipe.maxRecipes; num5++)
            {
                Statics.recipe[num5] = new Recipe();
                Statics.availableRecipeY[num5] = (float)(65 * num5);
            }
            Recipe.SetupRecipes();
            //for (int num6 = 0; num6 < Main.numChatLines; num6++)
            //{
            //    Main.chatLine[num6] = new ChatLine();
            //}
            for (int num7 = 0; num7 < Liquid.resLiquid; num7++)
            {
                world.getLiquid()[num7] = new Liquid();
            }
            for (int num8 = 0; num8 < 10000; num8++)
            {
                world.getLiquidBuffer()[num8] = new LiquidBuffer();
            }


            world.getShops()[0] = new Chest();
            world.getShops()[1] = new Chest();
            world.getShops()[1].SetupShop(1);
            world.getShops()[2] = new Chest();
            world.getShops()[2].SetupShop(2);
            world.getShops()[3] = new Chest();
            world.getShops()[3].SetupShop(3);
            world.getShops()[4] = new Chest();
            world.getShops()[4].SetupShop(4);
            Statics.teamColor[0] = new Color(255, 255, 255);
            Statics.teamColor[1] = new Color(230, 40, 20);
            Statics.teamColor[2] = new Color(20, 200, 30);
            Statics.teamColor[3] = new Color(75, 90, 255);
            Statics.teamColor[4] = new Color(200, 180, 0);


                //Main.LoadPlayers();
                //world.getPlayerList()[Main.myPlayer] = (Player)Main.loadPlayer[0].Clone();
                //Main.PlayerPath = Main.loadPlayerPath[0];
            

            netplay = new NetPlay(world);
            netplay.Init();
            WorldGen.EveryTileFrame(world);
           // if (Main.skipMenu)
            //{
               // WorldGen.clearWorld();
                //Main.gameMenu = false;
                //Main.LoadPlayers();
                //world.getPlayerList()[Main.myPlayer] = (Player)Main.loadPlayer[0].Clone();
                //Main.PlayerPath = Main.loadPlayerPath[0];
                //Main.LoadWorlds();
                //WorldGen.generateWorld(-1);
                //WorldGen.EveryTileFrame();
                //world.getPlayerList()[Main.myPlayer].Spawn();
            //}
            //else
            //{
             //   IntPtr systemMenu = Main.GetSystemMenu(this.Window.Handle, false);
             //   int menuItemCount = Main.GetMenuItemCount(systemMenu);
             //   Main.RemoveMenu(systemMenu, menuItemCount - 1, 1024);
            //}
            //base.Initialize();
            //Star.SpawnStars();
        }

        internal void UpdateServer()
        {
            Statics.netPlayCounter++;
            if (Statics.netPlayCounter > 3600)
            {
                NetMessage.SendData(7, world, -1, -1, "", 0, 0f, 0f, 0f);
                NetMessage.syncPlayers(world);
                Statics.netPlayCounter = 0;
            }
            Math.IEEERemainder((double)Statics.netPlayCounter, 60.0);
            if (Math.IEEERemainder((double)Statics.netPlayCounter, 360.0) == 0.0)
            {
                bool flag = true;
                int num = Statics.lastItemUpdate;
                int num2 = 0;
                while (flag)
                {
                    num++;
                    if (num >= 200)
                    {
                        num = 0;
                    }
                    num2++;
                    if (!world.getItemList()[num].active || world.getItemList()[num].owner == 8)
                    {
                        NetMessage.SendData(21, world, -1, -1, "", num, 0f, 0f, 0f);
                    }
                    if (num2 >= Statics.maxItemUpdates || num == Statics.lastItemUpdate)
                    {
                        flag = false;
                    }
                }
                Statics.lastItemUpdate = num;
            }
            for (int i = 0; i < 200; i++)
            {
                if (world.getItemList()[i].active && (world.getItemList()[i].owner == 8 || !world.getPlayerList()[world.getItemList()[i].owner].active))
                {
                    world.getItemList()[i].FindOwner(i, world);
                }
            }
            for (int j = 0; j < 8; j++)
            {
                if (netplay.serverSock[j].active)
                {
                    netplay.serverSock[j].timeOut++;
                    if (!Statics.stopTimeOuts && netplay.serverSock[j].timeOut > 60 * Statics.timeOut)
                    {
                        netplay.serverSock[j].kill = true;
                    }
                }
                if (world.getPlayerList()[j].active)
                {
                    int sectionX = NetPlay.GetSectionX((int)(world.getPlayerList()[j].position.X / 16f));
                    int sectionY = NetPlay.GetSectionY((int)(world.getPlayerList()[j].position.Y / 16f));
                    int num3 = 0;
                    for (int k = sectionX - 1; k < sectionX + 2; k++)
                    {
                        for (int l = sectionY - 1; l < sectionY + 2; l++)
                        {
                            if (k >= 0 && k < Statics.maxSectionsX && l >= 0 && l < Statics.maxSectionsY && !netplay.serverSock[j].tileSection[k, l])
                            {
                                num3++;
                            }
                        }
                    }
                    if (num3 > 0)
                    {
                        int num4 = num3 * 150;
                        NetMessage.SendData(9, world, j, -1, "Recieving tile data", num4, 0f, 0f, 0f);
                        netplay.serverSock[j].statusText2 = "is recieving tile data";
                        netplay.serverSock[j].statusMax += num4;
                        for (int m = sectionX - 1; m < sectionX + 2; m++)
                        {
                            for (int n = sectionY - 1; n < sectionY + 2; n++)
                            {
                                if (m >= 0 && m < Statics.maxSectionsX && n >= 0 && n < Statics.maxSectionsY && !netplay.serverSock[j].tileSection[m, n])
                                {
                                    NetMessage.SendSection(j, m, n, world);
                                    NetMessage.SendData(11, world, j, -1, "", m, (float)n, (float)m, (float)n);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void StartServer()
        {
            netplay.StartServer();
        }

        private static void StartInvasion(World world)
        {
            if (!WorldGen.shadowOrbSmashed)
            {
                return;
            }
            if (world.getInvasionType() == 0 && world.getInvasionDelay() == 0)
            {
                int num = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (world.getPlayerList()[i].active && world.getPlayerList()[i].statLife >= 200)
                    {
                        num++;
                    }
                }
                if (num > 0)
                {
                    world.setInvasionType(1);
                    world.setInvasionSize(100 + 50 * num);
                    world.setInvasionWarn(0);
                    //Main.invasionType = 1;
                    //Main.invasionSize = 100 + 50 * num;
                    //Main.invasionWarn = 0;
                    if (Statics.rand.Next(2) == 0)
                    {
                        world.setInvasionX(0.0);
                        //Main.invasionX = 0.0;
                        return;
                    }
                    //world.getInvasionX() = (double)Statics.maxTilesX;
                    world.setInvasionX((double)Statics.maxTilesX);// = (double)Statics.maxTilesX;
                }
            }
        }

        internal void UpdateTime()
        {
            world.setTime(world.getTime() + 1.0);
            if (!world.isDayTime())
            {
                if (WorldGen.spawnEye && Statics.netMode != 1 && world.getTime() > 4860.0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (world.getPlayerList()[i].active && !world.getPlayerList()[i].dead && (double)world.getPlayerList()[i].position.Y < world.getWorldSurface() * 16.0)
                        {
                            NPC.SpawnOnPlayer(i, 4, world);
                            WorldGen.spawnEye = false;
                            break;
                        }
                    }
                }
                if (world.getTime() > 32400.0)
                {
                    if (world.getInvasionDelay() > 0)
                    {
                        world.setInvasionDelay(world.getInvasionDelay() - 1);
                    }
                    WorldGen.spawnNPC = 0;
                    Statics.checkForSpawns = 0;
                    world.setTime(0.0);
                    world.setBloodMoon(false);
                    world.setDayTime(true);
                    world.setMoonPhase(world.getMoonPhase() +1);
                    if (world.getMoonPhase() >= 8)
                    {
                        world.setMoonPhase(0);
                    }
                    if (Statics.netMode == 2)
                    {
                        NetMessage.SendData(7, world, -1, -1, "", 0, 0f, 0f, 0f);
                        //WorldGen.saveAndPlay();
                    }
                    if (Statics.netMode != 1 && Statics.rand.Next(15) == 0)
                    {
                        StartInvasion(world);
                    }
                }
                if (world.getTime() > 16200.0 && WorldGen.spawnMeteor)
                {
                    WorldGen.spawnMeteor = false;
                    WorldGen.dropMeteor(world);
                    return;
                }
            }
            else
            {
                if (world.getTime() > 54000.0)
                {
                    WorldGen.spawnNPC = 0;
                    Statics.checkForSpawns = 0;
                    if (Statics.rand.Next(50) == 0 && Statics.netMode != 1 && WorldGen.shadowOrbSmashed)
                    {
                        WorldGen.spawnMeteor = true;
                    }
                    if (!NPC.downedBoss1 && Statics.netMode != 1)
                    {
                        bool flag = false;
                        for (int j = 0; j < 8; j++)
                        {
                            if (world.getPlayerList()[j].active && world.getPlayerList()[j].statLifeMax >= 200)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag && Statics.rand.Next(3) == 0)
                        {
                            int num = 0;
                            for (int k = 0; k < 1000; k++)
                            {
                                if (world.getNPCs()[k].active && world.getNPCs()[k].townNPC)
                                {
                                    num++;
                                }
                            }
                            if (num >= 4)
                            {
                                WorldGen.spawnEye = true;
                                if (Statics.netMode == 2)
                                {
                                    NetMessage.SendData(25, world, -1, -1, "You feel an evil presence watching you...", 8, 50f, 255f, 130f);
                                }
                            }
                        }
                    }
                    if (!WorldGen.spawnEye && world.getMoonPhase() != 4 && Statics.rand.Next(7) == 0 && Statics.netMode != 1)
                    {
                        for (int l = 0; l < 8; l++)
                        {
                            if (world.getPlayerList()[l].active && world.getPlayerList()[l].statLifeMax > 100)
                            {
                                world.setBloodMoon(true);
                                break;
                            }
                        }
                        if (world.isBloodMoon())
                        {
                            if (Statics.netMode == 2)
                            {
                                NetMessage.SendData(25, world, -1, -1, "The Blood Moon is rising...", 8, 50f, 255f, 130f);
                            }
                        }
                    }
                    world.setTime(0.0);
                    world.setDayTime(false);
                    if (Statics.netMode == 2)
                    {
                        NetMessage.SendData(7, world, -1, -1, "", 0, 0f, 0f, 0f);
                    }
                }
                if (Statics.netMode != 1)
                {
                    Statics.checkForSpawns++;
                    if (Statics.checkForSpawns >= 7200)
                    {
                        int num2 = 0;
                        for (int m = 0; m < 8; m++)
                        {
                            if (world.getPlayerList()[m].active)
                            {
                                num2++;
                            }
                        }
                        Statics.checkForSpawns = 0;
                        WorldGen.spawnNPC = 0;
                        int num3 = 0;
                        int num4 = 0;
                        int num5 = 0;
                        int num6 = 0;
                        int num7 = 0;
                        int num8 = 0;
                        int num9 = 0;
                        int num10 = 0;
                        for (int n = 0; n < 1000; n++)
                        {
                            if (world.getNPCs()[n].active && world.getNPCs()[n].townNPC)
                            {
                                if (world.getNPCs()[n].type != 37 && !world.getNPCs()[n].homeless)
                                {
                                    WorldGen.QuickFindHome(n, world);
                                }
                                else
                                {
                                    num8++;
                                }
                                if (world.getNPCs()[n].type == 17)
                                {
                                    num3++;
                                }
                                if (world.getNPCs()[n].type == 18)
                                {
                                    num4++;
                                }
                                if (world.getNPCs()[n].type == 19)
                                {
                                    num6++;
                                }
                                if (world.getNPCs()[n].type == 20)
                                {
                                    num5++;
                                }
                                if (world.getNPCs()[n].type == 22)
                                {
                                    num7++;
                                }
                                if (world.getNPCs()[n].type == 38)
                                {
                                    num9++;
                                }
                                num10++;
                            }
                        }
                        if (WorldGen.spawnNPC == 0)
                        {
                            int num11 = 0;
                            bool flag2 = false;
                            int num12 = 0;
                            bool flag3 = false;
                            bool flag4 = false;
                            for (int num13 = 0; num13 < 8; num13++)
                            {
                                if (world.getPlayerList()[num13].active)
                                {
                                    for (int num14 = 0; num14 < 44; num14++)
                                    {
                                        if (world.getPlayerList()[num13].inventory[num14] != null & world.getPlayerList()[num13].inventory[num14].stack > 0)
                                        {
                                            if (world.getPlayerList()[num13].inventory[num14].type == 71)
                                            {
                                                num11 += world.getPlayerList()[num13].inventory[num14].stack;
                                            }
                                            if (world.getPlayerList()[num13].inventory[num14].type == 72)
                                            {
                                                num11 += world.getPlayerList()[num13].inventory[num14].stack * 100;
                                            }
                                            if (world.getPlayerList()[num13].inventory[num14].type == 73)
                                            {
                                                num11 += world.getPlayerList()[num13].inventory[num14].stack * 10000;
                                            }
                                            if (world.getPlayerList()[num13].inventory[num14].type == 74)
                                            {
                                                num11 += world.getPlayerList()[num13].inventory[num14].stack * 1000000;
                                            }
                                            if (world.getPlayerList()[num13].inventory[num14].type == 95 || world.getPlayerList()[num13].inventory[num14].type == 96 || world.getPlayerList()[num13].inventory[num14].type == 97 || world.getPlayerList()[num13].inventory[num14].type == 98 || world.getPlayerList()[num13].inventory[num14].useAmmo == 14)
                                            {
                                                flag3 = true;
                                            }
                                            if (world.getPlayerList()[num13].inventory[num14].type == 166 || world.getPlayerList()[num13].inventory[num14].type == 167)
                                            {
                                                flag4 = true;
                                            }
                                        }
                                    }
                                    int num15 = world.getPlayerList()[num13].statLifeMax / 20;
                                    if (num15 > 5)
                                    {
                                        flag2 = true;
                                    }
                                    num12 += num15;
                                }
                            }
                            if (WorldGen.spawnNPC == 0 && num7 < 1)
                            {
                                WorldGen.spawnNPC = 22;
                            }
                            if (WorldGen.spawnNPC == 0 && (double)num11 > 5000.0 && num3 < 1)
                            {
                                WorldGen.spawnNPC = 17;
                            }
                            if (WorldGen.spawnNPC == 0 && flag2 && num4 < 1)
                            {
                                WorldGen.spawnNPC = 18;
                            }
                            if (WorldGen.spawnNPC == 0 && flag3 && num6 < 1)
                            {
                                WorldGen.spawnNPC = 19;
                            }
                            if (WorldGen.spawnNPC == 0 && (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3) && num5 < 1)
                            {
                                WorldGen.spawnNPC = 20;
                            }
                            if (WorldGen.spawnNPC == 0 && flag4 && num3 > 0 && num9 < 1)
                            {
                                WorldGen.spawnNPC = 38;
                            }
                            if (WorldGen.spawnNPC == 0 && num11 > 100000 && num3 < 2 && num2 > 2)
                            {
                                WorldGen.spawnNPC = 17;
                            }
                            if (WorldGen.spawnNPC == 0 && num12 >= 20 && num4 < 2 && num2 > 2)
                            {
                                WorldGen.spawnNPC = 18;
                            }
                            if (WorldGen.spawnNPC == 0 && num11 > 5000000 && num3 < 3 && num2 > 4)
                            {
                                WorldGen.spawnNPC = 17;
                            }
                            if (!NPC.downedBoss3 && num8 == 0)
                            {
                                int num16 = NPC.NewNPC(world.getDungeonX() * 16 + 8, world.getDungeonY() * 16, world, 37, 0);
                                world.getNPCs()[num16].homeless = false;
                                world.getNPCs()[num16].homeTileX = world.getDungeonX();
                                world.getNPCs()[num16].homeTileY = world.getDungeonY();
                            }
                        }
                    }
                }
            }
        }

        /*
        internal void Update()
        {
            if (Statics.fixedTiming)
            {
                if (Statics.IsActive)
                {
                    Statics.IsFixedTimeStep = false;
                }
                else
                {
                    Statics.IsFixedTimeStep = true;
                }
            }
            else
            {
                Statics.IsFixedTimeStep = true;
            }
            if (Statics.netMode != 2)
            {
                Statics.saveTimer++;
                if (Statics.saveTimer > 18000)
                {
                    Statics.saveTimer = 0;
                    WorldGen.saveToonWhilePlaying(world);
                }
            }
            else
            {
                Statics.saveTimer = 0;
            }
            if (Statics.rand == null || Statics.rand.Next(99999) == 0)
            {
                Statics.rand = new Random((int)DateTime.Now.Ticks);
            }
            Statics.updateTime++;
            if (Statics.updateTime >= 60)
            {
                Statics.updateTime = 0;
                Statics.drawTime = 0;
                if (Liquid.quickSettle)
                {
                    Liquid.maxLiquid = Liquid.resLiquid;
                    Liquid.cycles = 1;
                }
                else
                {
                    
                                    Liquid.maxLiquid = 3000;
                                    Liquid.cycles = 17;
                                
                }
                if (Statics.netMode == 2)
                {
                    Statics.cloudLimit = 0;
                }
            }
            //if (!Main.IsActive)
            //{
            //    Main.hasFocus = false;
            //}
            //else
            //{
            //    Main.hasFocus = true;
            //}
            
            //base.IsMouseVisible = false;
            //if (Main.keyState.IsKeyDown(Keys.F10) && !Main.chatMode && !Main.editSign)
            //{
            //    if (Main.frameRelease)
            //    {
            //        Main.PlaySound(12, -1, -1, 1);
            //        if (Main.showFrameRate)
            //        {
            //            Main.showFrameRate = false;
            //        }
            //        else
            //        {
            //            Main.showFrameRate = true;
            //        }
            //    }
            //    Main.frameRelease = false;
            //}
            //else
            //{
            //    Main.frameRelease = true;
            //}
            //if (Main.keyState.IsKeyDown(Keys.F11))
            //{
            //    if (Main.releaseUI)
            //    {
            //        if (Main.hideUI)
            //        {
            //            Main.hideUI = false;
            //        }
            //        else
            //        {
            //            Main.hideUI = true;
            //        }
            //    }
            //    Main.releaseUI = false;
            //}
            //else
            //{
            //    Main.releaseUI = true;
            //}
            //if ((Main.keyState.IsKeyDown(Keys.LeftAlt) || Main.keyState.IsKeyDown(Keys.RightAlt)) && Main.keyState.IsKeyDown(Keys.Enter))
            //{
            //    if (this.toggleFullscreen)
            //    {
            //        this.graphics.ToggleFullScreen();
            //        Main.chatRelease = false;
            //    }
            //    this.toggleFullscreen = false;
            //}
            //else
            //{
            //    this.toggleFullscreen = true;
            //}
            //Main.oldMouseState = Main.mouseState;
            //Main.mouseState = Mouse.GetState();
            //Main.keyState = Keyboard.GetState();
            //if (Main.editSign)
            //{
            //    Main.chatMode = false;
            //}
            //if (Main.chatMode)
            //{
            //    string a = Main.chatText;
            //    Main.chatText = Main.GetInputText(Main.chatText);
            //    while (Main.fontMouseText.MeasureString(Main.chatText).X > 470f)
            //    {
            //        Main.chatText = Main.chatText.Substring(0, Main.chatText.Length - 1);
            //    }
            //    if (a != Main.chatText)
            //    {
            //        Main.PlaySound(12, -1, -1, 1);
            //    }
            //    if (Main.inputTextEnter && Main.chatRelease)
            //    {
            //        if (Main.chatText != "")
            //        {
            //            NetMessage.SendData(25, -1, -1, Main.chatText, Main.myPlayer, 0f, 0f, 0f);
            //        }
            //        Main.chatText = "";
            //        Main.chatMode = false;
            //        Main.chatRelease = false;
            //        Main.PlaySound(11, -1, -1, 1);
            //    }
            //}
            //if (Main.keyState.IsKeyDown(Keys.Enter) && Main.netMode == 1)
            //{
            //    if (Main.chatRelease && !Main.chatMode && !Main.editSign)
            //    {
            //        Main.PlaySound(10, -1, -1, 1);
            //        Main.chatMode = true;
            //        Main.chatText = "";
            //    }
            //    Main.chatRelease = false;
            //}
            //else
            //{
            //    Main.chatRelease = true;
            //}
            //if (Main.gameMenu)
            //{
            //    Main.UpdateMenu();
            //    if (Main.netMode != 2)
            //    {
            //        return;
            //    }
            //}
            //if (Main.debugMode)
            //{
            //    Main.UpdateDebug();
            //}
            //if (Main.netMode == 1)
            //{
            //    for (int i = 0; i < 44; i++)
            //    {
            //        if (world.getPlayerList()[Main.myPlayer].inventory[i].IsNotTheSameAs(Main.clientPlayer.inventory[i]))
            //        {
            //            NetMessage.SendData(5, -1, -1, world.getPlayerList()[Main.myPlayer].inventory[i].name, Main.myPlayer, (float)i, 0f, 0f);
            //        }
            //    }
            //    if (world.getPlayerList()[Main.myPlayer].armor[0].IsNotTheSameAs(Main.clientPlayer.armor[0]))
            //    {
            //        NetMessage.SendData(5, -1, -1, world.getPlayerList()[Main.myPlayer].armor[0].name, Main.myPlayer, 44f, 0f, 0f);
            //    }
            //    if (world.getPlayerList()[Main.myPlayer].armor[1].IsNotTheSameAs(Main.clientPlayer.armor[1]))
            //    {
            //        NetMessage.SendData(5, -1, -1, world.getPlayerList()[Main.myPlayer].armor[1].name, Main.myPlayer, 45f, 0f, 0f);
            //    }
            //    if (world.getPlayerList()[Main.myPlayer].armor[2].IsNotTheSameAs(Main.clientPlayer.armor[2]))
            //    {
            //        NetMessage.SendData(5, -1, -1, world.getPlayerList()[Main.myPlayer].armor[2].name, Main.myPlayer, 46f, 0f, 0f);
            //    }
            //    if (world.getPlayerList()[Main.myPlayer].armor[3].IsNotTheSameAs(Main.clientPlayer.armor[3]))
            //    {
            //        NetMessage.SendData(5, -1, -1, world.getPlayerList()[Main.myPlayer].armor[3].name, Main.myPlayer, 47f, 0f, 0f);
            //    }
            //    if (world.getPlayerList()[Main.myPlayer].armor[4].IsNotTheSameAs(Main.clientPlayer.armor[4]))
            //    {
            //        NetMessage.SendData(5, -1, -1, world.getPlayerList()[Main.myPlayer].armor[4].name, Main.myPlayer, 48f, 0f, 0f);
            //    }
            //    if (world.getPlayerList()[Main.myPlayer].armor[5].IsNotTheSameAs(Main.clientPlayer.armor[5]))
            //    {
            //        NetMessage.SendData(5, -1, -1, world.getPlayerList()[Main.myPlayer].armor[5].name, Main.myPlayer, 49f, 0f, 0f);
            //    }
            //    if (world.getPlayerList()[Main.myPlayer].chest != Main.clientPlayer.chest)
            //    {
            //        NetMessage.SendData(33, -1, -1, "", world.getPlayerList()[Main.myPlayer].chest, 0f, 0f, 0f);
            //    }
            //    if (world.getPlayerList()[Main.myPlayer].talkNPC != Main.clientPlayer.talkNPC)
            //    {
            //        NetMessage.SendData(40, -1, -1, "", Main.myPlayer, 0f, 0f, 0f);
            //    }
            //    if (world.getPlayerList()[Main.myPlayer].zoneEvil != Main.clientPlayer.zoneEvil)
            //    {
            //        NetMessage.SendData(36, -1, -1, "", Main.myPlayer, 0f, 0f, 0f);
            //    }
            //    if (world.getPlayerList()[Main.myPlayer].zoneMeteor != Main.clientPlayer.zoneMeteor)
            //    {
            //        NetMessage.SendData(36, -1, -1, "", Main.myPlayer, 0f, 0f, 0f);
            //    }
            //    if (world.getPlayerList()[Main.myPlayer].zoneDungeon != Main.clientPlayer.zoneDungeon)
            //    {
            //        NetMessage.SendData(36, -1, -1, "", Main.myPlayer, 0f, 0f, 0f);
            //    }
            //    if (world.getPlayerList()[Main.myPlayer].zoneJungle != Main.clientPlayer.zoneJungle)
            //    {
            //        NetMessage.SendData(36, -1, -1, "", Main.myPlayer, 0f, 0f, 0f);
            //    }
            //}
            //if (Main.netMode == 1)
            //{
            //    Main.clientPlayer = (Player)world.getPlayerList()[Main.myPlayer].clientClone();
            //}
            int j = 0;
            while (j < 8)
            {
                if (Statics.ignoreErrors)
                {
                    try
                    {
                        //world.getPlayerList()[j].UpdatePlayer(j);
                        goto IL_955;
                    }
                    catch
                    {
                        goto IL_955;
                    }
                    goto IL_948;
                }
                goto IL_948;
            IL_955:
                j++;
                continue;
            IL_948:
                //world.getPlayerList()[j].UpdatePlayer(j);
                goto IL_955;
            }
            if (Statics.netMode != 1)
            {
                //NPC.SpawnNPC();
            }
            for (int k = 0; k < 8; k++)
            {
                world.getPlayerList()[k].activeNPCs = 0;
                world.getPlayerList()[k].townNPCs = 0;
            }
            int l = 0;
            while (l < 1000)
            {
                if (Statics.ignoreErrors)
                {
                    try
                    {
                        //world.getNPCs()[l].UpdateNPC(l);
                        goto IL_9CC;
                    }
                    catch
                    {
                        world.getNPCs()[l] = new NPC();
                        goto IL_9CC;
                    }
                    goto IL_9BD;
                }
                goto IL_9BD;
            IL_9CC:
                l++;
                continue;
            IL_9BD:
                //world.getNPCs()[l].UpdateNPC(l);
                goto IL_9CC;
            }
            int m = 0;
            while (m < 200)
            {
                if (Statics.ignoreErrors)
                {
                    try
                    {
                        //world.getGore()[m].Update();
                        goto IL_A13;
                    }
                    catch
                    {
                        //world.getGore()[m] = new Gore();
                        goto IL_A13;
                    }
                    goto IL_A06;
                }
                goto IL_A06;
            IL_A13:
                m++;
                continue;
            IL_A06:
                //world.getGore()[m].Update();
                goto IL_A13;
            }
            int n = 0;
            while (n < 1000)
            {
                if (Statics.ignoreErrors)
                {
                    try
                    {
                        //world.getProjectile()[n].Update(n);
                        goto IL_A5E;
                    }
                    catch
                    {
                        world.getProjectile()[n] = new Projectile();
                        goto IL_A5E;
                    }
                    goto IL_A4F;
                }
                goto IL_A4F;
            IL_A5E:
                n++;
                continue;
            IL_A4F:
                //world.getProjectile()[n].Update(n);
                goto IL_A5E;
            }
            int num = 0;
            while (num < 200)
            {
                if (Statics.ignoreErrors)
                {
                    try
                    {
                        world.getItemList()[num].UpdateItem(num);
                        goto IL_AA9;
                    }
                    catch
                    {
                        world.getItemList()[num] = new Item();
                        goto IL_AA9;
                    }
                    goto IL_A9A;
                }
                goto IL_A9A;
            IL_AA9:
                num++;
                continue;
            IL_A9A:
                world.getItemList()[num].UpdateItem(num);
                goto IL_AA9;
            }
            if (Statics.ignoreErrors)
            {
                try
                {
                    //Dust.UpdateDust();
                    goto IL_AEF;
                }
                catch
                {
                    for (int num2 = 0; num2 < 2000; num2++)
                    {
                        //world.getDust()[num2] = new Dust();
                    }
                    goto IL_AEF;
                }
                goto IL_AEA;
            }
            goto IL_AEA;
        IL_AEF:
            if (Statics.netMode != 2)
            {
                //CombatText.UpdateCombatText();
            }
        if (Statics.ignoreErrors)
            {
                try
                {
                    UpdateTime();
                    goto IL_B18;
                }
                catch
                {
                    Statics.checkForSpawns = 0;
                    goto IL_B18;
                }
                goto IL_B13;
            }
            goto IL_B13;
        IL_B18:
            if (Statics.netMode != 1)
            {
                if (Statics.ignoreErrors)
                {
                    try
                    {
                        //WorldGen.UpdateWorld();
                        //UpdateInvasion();
                        goto IL_B40;
                    }
                    catch
                    {
                        goto IL_B40;
                    }
                }
                //WorldGen.UpdateWorld();
                //UpdateInvasion();
            }
        IL_B40:
        if (Statics.ignoreErrors)
            {
                try
                {
                    if (Statics.netMode == 2)
                    {
                        UpdateServer();
                    }
                    if (Statics.netMode == 1)
                    {
                        //Main.UpdateClient();
                    }
                    goto IL_BB8;
                }
                catch
                {
                    int arg_B6B_0 = Statics.netMode;
                    goto IL_BB8;
                }
                goto IL_B6E;
            }
            goto IL_B6E;
        IL_B13:
            UpdateTime();
            goto IL_B18;
        IL_AEA:
            //Dust.UpdateDust();
            goto IL_AEF;
        IL_B6E:
            if (Statics.netMode == 2)
            {
                UpdateServer();
            }
            if (Statics.netMode == 1)
            {
                
            }
        IL_BB8:
        if (Statics.ignoreErrors)
            {
                try
                {
                    
                    goto IL_C57;
                }
                catch
                {
                    
                    goto IL_C57;
                }
                goto IL_C1E;
            }
            goto IL_C1E;
        IL_C57:
            Update();
            return;
        IL_C1E:
           
            goto IL_C57;
        }
    */

        private static void InvasionWarning(World world)
        {
            if (world.getInvasionType() == 0)
            {
                return;
            }
            string text = "";
            if (world.getInvasionSize() <= 0)
            {
                text = "The goblin army has been defeated!";
            }
            else
            {
                if (world.getInvasionX() < (double)Statics.spawnTileX)
                {
                    text = "A goblin army is approaching from the west!";
                }
                else
                {
                    if (world.getInvasionX() > (double)Statics.spawnTileX)
                    {
                        text = "A goblin army is approaching from the east!";
                    }
                    else
                    {
                        text = "The goblin army has arrived!";
                    }
                }
            }
            if (Statics.netMode == 0)
            {
                //Main.NewText(text, 175, 75, 255);
                return;
            }
            if (Statics.netMode == 2)
            {
                NetMessage.SendData(25, world, -1, -1, text, 8, 175f, 75f, 255f);
            }
        }

        private static void UpdateInvasion(World world)
        {
            if (world.getInvasionType() > 0)
            {
                if (world.getInvasionSize() <= 0)
                {
                    InvasionWarning(world);
                    world.setInvasionType(0);
                    world.setInvasionDelay(7);
                }
                if (world.getInvasionX() == (double)Statics.spawnTileX)
                {
                    return;
                }
                float num = 0.2f;
                if (world.getInvasionX() > (double)Statics.spawnTileX)
                {
                    world.setInvasionX(world.getInvasionX() - (double)num);
                    if (world.getInvasionX() <= (double)Statics.spawnTileX)
                    {
                        world.setInvasionX((double)Statics.spawnTileX);
                        InvasionWarning(world);
                    }
                    else
                    {
                        world.setInvasionWarn(world.getInvasionWarn() - 1);
                    }
                }
                else
                {
                    if (world.getInvasionX() < (double)Statics.spawnTileX)
                    {
                        world.setInvasionX(world.getInvasionX() + (double)num);
                        if (world.getInvasionX() >= (double)Statics.spawnTileX)
                        {
                            world.setInvasionX((double)Statics.spawnTileX);
                            InvasionWarning(world);
                        }
                        else
                        {
                            world.setInvasionWarn(world.getInvasionWarn() - 1);
                        }
                    }
                }
                if (world.getInvasionWarn() <= 0)
                {
                    world.setInvasionWarn(3600);
                    //Statics.invasionWarn = 3600;
                    InvasionWarning(world);
                }
            }
        }

        public void Update()
        {
            bool flag = 1 == 0;
            //int arg_2C_0 = this.asdf;
            //DateTime now = DateTime.Now;
           // this.asdf = arg_2C_0 + now.Subtract(this.tmp).Milliseconds;
            //if (this.asdf >= 1000)
            ////{
            //    this.totalups = 0;
            //    this.asdf = 0;
            //}
            //if (Main.fixedTiming)
            //{
            //}
            //if (!Main.showSplash)
           // {
                if (Statics.netMode != 2)
                {
                    Statics.saveTimer++;
                    if (Statics.saveTimer > 18000)
                    {
                        Statics.saveTimer = 0;
                        WorldGen.saveToonWhilePlaying(world);
                    }
                }
                else
                {
                    Statics.saveTimer = 0;
                }
                if (Statics.rand == null || Statics.rand.Next(99999) == 0)
                {
                    //now = DateTime.Now;
                    Statics.rand = new Random((int)DateTime.Now.Ticks);
                }
                Statics.updateTime++;
                if (Statics.updateTime >= 60)
                {
                    //Lighting.lightPasses = 2;
                    //Lighting.lightSkip = 4;
                    Statics.cloudLimit = 0;
                    //Gore.goreTime = 0;

                    if (Liquid.quickSettle)
                    {
                        Liquid.maxLiquid = Liquid.resLiquid;
                        Liquid.cycles = 1;
                    }
                    else
                    {
                        Liquid.maxLiquid = 3000;
                        Liquid.cycles = 17;
                    }
                    if (Statics.netMode == 2)
                    {
                        Statics.cloudLimit = 0;
                    }
                }
                //Main.hasFocus = false;
                /*if (!Main.chatMode && !Main.editSign)
                {
                    if (Main.frameRelease)
                    {
                        if (Main.showFrameRate)
                        {
                            Main.showFrameRate = false;
                        }
                        else
                        {
                            Main.showFrameRate = true;
                        }
                    }
                    Main.frameRelease = false;
                }
                else
                {
                    Main.frameRelease = true;
                }*/
                //Main.releaseUI = true;
                //if (Main.editSign)
                //{
                //    Main.chatMode = false;
                ///}
                //if (Main.chatMode)
                //{
                //    string a = Main.chatText;
                //    while (Main.fontMouseText.MeasureString(Main.chatText).X > 470f)
                //    {
                //        Main.chatText = Main.chatText.Substring(0, Main.chatText.Length - 1);
                //    }
                //    if (!(a != Main.chatText))
                //    {
                //        goto IL_418;
                //    }
                //IL_418:
                //    if (Main.inputTextEnter && Main.chatRelease)
                //    {
                //        if (Main.chatText != "")
                //        {
                //            NetMessage.SendData(25, -1, -1, Main.chatText, Main.myPlayer, 0f, 0f, 0f);
                //        }
                //        Main.chatText = "";
                //        Main.chatMode = false;
                //        Main.chatRelease = false;
                //    }
                //}
                //Main.chatRelease = true;
                for (int i = 0; i < 8; i++)
                {
                    if (Statics.ignoreErrors)
                    {
                        try
                        {
                            world.getPlayerList()[i].UpdatePlayer(i);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        world.getPlayerList()[i].UpdatePlayer(i);
                    }
                }
                if (Statics.netMode != 1)
                {
                    NPC.SpawnNPC(world);
                }
                for (int j = 0; j < 8; j++)
                {
                    world.getPlayerList()[j].activeNPCs = 0;
                    world.getPlayerList()[j].townNPCs = 0;
                }
                for (int k = 0; k < 1000; k++)
                {
                    if (Statics.ignoreErrors)
                    {
                        try
                        {
                            world.getNPCs()[k].UpdateNPC(k, world);
                        }
                        catch
                        {
                            world.getNPCs()[k] = new NPC();
                        }
                    }
                    else
                    {
                        world.getNPCs()[k].UpdateNPC(k, world);
                    }
                }
                for (int l = 0; l < 200; l++)
                {
                    if (Statics.ignoreErrors)
                    {
                        try
                        {
                            world.getGore()[l].Update();
                        }
                        catch
                        {
                            world.getGore()[l] = new Gore();
                        }
                    }
                    else
                    {
                        world.getGore()[l].Update();
                    }
                }
                for (int m = 0; m < 1000; m++)
                {
                    if (Statics.ignoreErrors)
                    {
                        try
                        {
                            world.getProjectile()[m].Update(m, world);
                        }
                        catch
                        {
                            world.getProjectile()[m] = new Projectile();
                        }
                    }
                    else
                    {
                        world.getProjectile()[m].Update(m, world);
                    }
                }
                for (int n = 0; n < 200; n++)
                {
                    if (Statics.ignoreErrors)
                    {
                        try
                        {
                            world.getItemList()[n].UpdateItem(n, world);
                        }
                        catch
                        {
                            world.getItemList()[n] = new Item();
                        }
                    }
                    else
                    {
                        world.getItemList()[n].UpdateItem(n, world);
                    }
                }
                if (Statics.ignoreErrors)
                {
                    try
                    {
                        Dust.UpdateDust(world);
                    }
                    catch
                    {
                        for (int num = 0; num < 2000; num++)
                        {
                            world.getDust()[num] = new Dust();
                        }
                    }
                }
                else
                {
                    Dust.UpdateDust(world);
                }
                //if (Statics.netMode != 2)
                //{
                //    CombatText.UpdateCombatText();
                //}
                if (Statics.ignoreErrors)
                {
                    try
                    {
                        UpdateTime();
                    }
                    catch
                    {
                        Statics.checkForSpawns = 0;
                    }
                }
                else
                {
                    UpdateTime();
                }
                if (Statics.netMode != 1)
                {
                    if (Statics.ignoreErrors)
                    {
                        try
                        {
                            WorldGen.UpdateWorld(world);
                            UpdateInvasion(world);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        WorldGen.UpdateWorld(world);
                        UpdateInvasion(world);
                    }
                }
                if (Statics.ignoreErrors)
                {
                    try
                    {
                        if (Statics.netMode == 2)
                        {
                            UpdateServer();
                        }
                    }
                    catch
                    {
                        //int num2 = Statics.netMode;
                    }
                }
                else
                {
                    if (Statics.netMode == 2)
                    {
                        UpdateServer();
                    }
                }
                if (Statics.ignoreErrors)
                {
                    try
                    {
                        //for (int num3 = 0; num3 < Main.numChatLines; num3++)
                        //{
                        //    if (Main.chatLine[num3].showTime > 0)
                        //    {
                        //        ChatLine chatLine = Main.chatLine[num3];
                        //        chatLine.showTime--;
                        //    }
                        //}
                    }
                    catch
                    {
                        //for (int num4 = 0; num4 < Main.numChatLines; num4++)
                        //{
                        //    Main.chatLine[num4] = new ChatLine();
                        //}
                    }
                }
                else
                {
                    //for (int num5 = 0; num5 < Main.numChatLines; num5++)
                    //{
                    //    if (Main.chatLine[num5].showTime > 0)
                    //    {
                    //        ChatLine chatLine2 = Main.chatLine[num5];
                    //        chatLine2.showTime--;
                    //    }
                    //}
                }
            //}
            //this.totalups++;
            //this.tmp = DateTime.Now;
        }



        public static bool stopDrops { get; set; }
    }
}
