using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class messageBuffer
    {
        public const int readBufferMax = 65535;
        public const int writeBufferMax = 65535;
        public bool broadcast;
        public byte[] readBuffer = new byte[65535];
        public byte[] writeBuffer = new byte[65535];
        public bool writeLocked;
        public int messageLength;
        public int totalData;
        public int whoAmI;
        public int spamCount;
        public int maxSpam;
        public bool checkBytes;

        public void Reset()
        {
            this.writeBuffer = new byte[65535];
            this.writeLocked = false;
            this.messageLength = 0;
            this.totalData = 0;
            this.spamCount = 0;
            this.broadcast = false;
            this.checkBytes = false;
        }

        public void GetData(int start, int length, World world)
        {
            if (this.whoAmI < 9)
            {
                world.getServer().getNetPlay().serverSock[this.whoAmI].timeOut = 0;
            }
            else
            {
                world.getServer().getNetPlay().clientSock.timeOut = 0;
            }
            int num = 0;
            num = start + 1;
            byte b = this.readBuffer[start];
            if (Statics.netMode == 1 && world.getServer().getNetPlay().clientSock.statusMax > 0)
            {
                world.getServer().getNetPlay().clientSock.statusCount++;
            }
            if (Statics.verboseNetplay)
            {
                for (int i = start; i < start + length; i++)
                {
                }
                for (int j = start; j < start + length; j++)
                {
                    byte arg_82_0 = this.readBuffer[j];
                }
            }
            if (Statics.netMode == 2 && b != 38 && world.getServer().getNetPlay().serverSock[this.whoAmI].state == -1)
            {
                NetMessage.SendData(2, world, this.whoAmI, -1, "Incorrect password.", 0, 0f, 0f, 0f);
                return;
            }
            if (b == 1 && Statics.netMode == 2)
            {
                if (world.getServer().getNetPlay().serverSock[this.whoAmI].state == 0)
                {
                    string @string = Encoding.ASCII.GetString(this.readBuffer, start + 1, length - 1);
                    if (!(@string == "Terraria" + Statics.currentRelease))
                    {
                        NetMessage.SendData(2, world, this.whoAmI, -1, "You are not using the same version as this server.", 0, 0f, 0f, 0f);
                        return;
                    }
                    if (world.getServer().getNetPlay().password == null || world.getServer().getNetPlay().password == "")
                    {
                        world.getServer().getNetPlay().serverSock[this.whoAmI].state = 1;
                        NetMessage.SendData(3, world, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
                        return;
                    }
                    world.getServer().getNetPlay().serverSock[this.whoAmI].state = -1;
                    NetMessage.SendData(37, world, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
                    return;
                }
            }
            else
            {
                if (b == 2 && Statics.netMode == 1)
                {
                    world.getServer().getNetPlay().disconnect = true;
                    //Main.statusText = Encoding.ASCII.GetString(this.readBuffer, start + 1, length - 1);
                    return;
                }
                if (b == 3 && Statics.netMode == 1)
                {
                    if (world.getServer().getNetPlay().clientSock.state == 1)
                    {
                        world.getServer().getNetPlay().clientSock.state = 2;
                    }
                    int num2 = (int)this.readBuffer[start + 1];
                    if (num2 != Statics.myPlayer)
                    {
                        world.getPlayerList()[num2] = (Player)world.getPlayerList()[Statics.myPlayer].Clone();
                        world.getPlayerList()[Statics.myPlayer] = new Player(world);
                        world.getPlayerList()[num2].whoAmi = num2;
                        Statics.myPlayer = num2;
                    }
                    NetMessage.SendData(4, world, -1, -1, world.getPlayerList()[Statics.myPlayer].name, Statics.myPlayer, 0f, 0f, 0f);
                    NetMessage.SendData(16, world, -1, -1, "", Statics.myPlayer, 0f, 0f, 0f);
                    NetMessage.SendData(42, world, -1, -1, "", Statics.myPlayer, 0f, 0f, 0f);
                    for (int k = 0; k < 44; k++)
                    {
                        NetMessage.SendData(5, world, -1, -1, world.getPlayerList()[Statics.myPlayer].inventory[k].name, Statics.myPlayer, (float)k, 0f, 0f);
                    }
                    NetMessage.SendData(5, world, -1, -1, world.getPlayerList()[Statics.myPlayer].armor[0].name, Statics.myPlayer, 44f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, -1, world.getPlayerList()[Statics.myPlayer].armor[1].name, Statics.myPlayer, 45f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, -1, world.getPlayerList()[Statics.myPlayer].armor[2].name, Statics.myPlayer, 46f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, -1, world.getPlayerList()[Statics.myPlayer].armor[3].name, Statics.myPlayer, 47f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, -1, world.getPlayerList()[Statics.myPlayer].armor[4].name, Statics.myPlayer, 48f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, -1, world.getPlayerList()[Statics.myPlayer].armor[5].name, Statics.myPlayer, 49f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, -1, world.getPlayerList()[Statics.myPlayer].armor[6].name, Statics.myPlayer, 50f, 0f, 0f);
                    NetMessage.SendData(5, world, -1, -1, world.getPlayerList()[Statics.myPlayer].armor[7].name, Statics.myPlayer, 51f, 0f, 0f);
                    NetMessage.SendData(6, world, -1, -1, "", 0, 0f, 0f, 0f);
                    if (world.getServer().getNetPlay().clientSock.state == 2)
                    {
                        world.getServer().getNetPlay().clientSock.state = 3;
                        return;
                    }
                }
                else
                {
                    if (b == 4)
                    {
                        bool flag = false;
                        int num3 = (int)this.readBuffer[start + 1];
                        int hair = (int)this.readBuffer[start + 2];
                        if (Statics.netMode == 2)
                        {
                            num3 = this.whoAmI;
                        }
                        world.getPlayerList()[num3].hair = hair;
                        world.getPlayerList()[num3].whoAmi = num3;
                        num += 2;
                        world.getPlayerList()[num3].hairColor.R = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].hairColor.G = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].hairColor.B = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].skinColor.R = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].skinColor.G = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].skinColor.B = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].eyeColor.R = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].eyeColor.G = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].eyeColor.B = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].shirtColor.R = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].shirtColor.G = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].shirtColor.B = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].underShirtColor.R = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].underShirtColor.G = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].underShirtColor.B = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].pantsColor.R = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].pantsColor.G = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].pantsColor.B = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].shoeColor.R = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].shoeColor.G = this.readBuffer[num];
                        num++;
                        world.getPlayerList()[num3].shoeColor.B = this.readBuffer[num];
                        num++;
                        string string2 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
                        world.getPlayerList()[num3].name = string2;
                        if (Statics.netMode == 2)
                        {
                            if (world.getServer().getNetPlay().serverSock[this.whoAmI].state < 10)
                            {
                                for (int l = 0; l < 8; l++)
                                {
                                    if (l != num3 && string2 == world.getPlayerList()[l].name && world.getServer().getNetPlay().serverSock[l].active)
                                    {
                                        flag = true;
                                    }
                                }
                            }
                            if (flag)
                            {
                                NetMessage.SendData(2, world, this.whoAmI, -1, string2 + " is already on this server.", 0, 0f, 0f, 0f);
                                return;
                            }
                            world.getServer().getNetPlay().serverSock[this.whoAmI].oldName = string2;
                            world.getServer().getNetPlay().serverSock[this.whoAmI].name = string2;
                            NetMessage.SendData(4, world, -1, this.whoAmI, string2, num3, 0f, 0f, 0f);
                            return;
                        }
                    }
                    else
                    {
                        if (b == 5)
                        {
                            int num4 = (int)this.readBuffer[start + 1];
                            if (Statics.netMode == 2)
                            {
                                num4 = this.whoAmI;
                            }
                            int num5 = (int)this.readBuffer[start + 2];
                            int stack = (int)this.readBuffer[start + 3];
                            string string3 = Encoding.ASCII.GetString(this.readBuffer, start + 4, length - 4);
                            if (num5 < 44)
                            {
                                world.getPlayerList()[num4].inventory[num5] = new Item();
                                world.getPlayerList()[num4].inventory[num5].SetDefaults(string3);
                                world.getPlayerList()[num4].inventory[num5].stack = stack;
                            }
                            else
                            {
                                world.getPlayerList()[num4].armor[num5 - 44] = new Item();
                                world.getPlayerList()[num4].armor[num5 - 44].SetDefaults(string3);
                                world.getPlayerList()[num4].armor[num5 - 44].stack = stack;
                            }
                            if (Statics.netMode == 2 && num4 == this.whoAmI)
                            {
                                NetMessage.SendData(5, world, -1, this.whoAmI, string3, num4, (float)num5, 0f, 0f);
                                return;
                            }
                        }
                        else
                        {
                            if (b == 6)
                            {
                                if (Statics.netMode == 2)
                                {
                                    if (world.getServer().getNetPlay().serverSock[this.whoAmI].state == 1)
                                    {
                                        world.getServer().getNetPlay().serverSock[this.whoAmI].state = 2;
                                    }
                                    NetMessage.SendData(7, world, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
                                    return;
                                }
                            }
                            else
                            {
                                if (b == 7)
                                {
                                    if (Statics.netMode == 1)
                                    {
                                        world.setTime(BitConverter.ToDouble(this.readBuffer, num));
                                        //Main.time = (double)BitConverter.ToInt32(this.readBuffer, num);
                                        num += 4;
                                        world.setDayTime(false);
                                        //Main.dayTime = false;
                                        if (this.readBuffer[num] == 1)
                                        {
                                            world.setDayTime(true);
                                            //Main.dayTime = true;
                                        }
                                        num++;
                                        world.setMoonPhase((int)this.readBuffer[num]);
                                        //Main.moonPhase = (int)this.readBuffer[num];
                                        num++;
                                        int num6 = (int)this.readBuffer[num];
                                        num++;
                                        if (num6 == 1)
                                        {
                                            world.setBloodMoon(true);
                                            //Main.bloodMoon = true;
                                        }
                                        else
                                        {
                                            world.setBloodMoon(false);
                                            //Main.bloodMoon = false;
                                        }
                                        Statics.maxTilesX = BitConverter.ToInt32(this.readBuffer, num);
                                        num += 4;
                                        Statics.maxTilesY = BitConverter.ToInt32(this.readBuffer, num);
                                        num += 4;
                                        Statics.spawnTileX = BitConverter.ToInt32(this.readBuffer, num);
                                        num += 4;
                                        Statics.spawnTileY = BitConverter.ToInt32(this.readBuffer, num);
                                        num += 4;
                                        world.setWorldSurface(BitConverter.ToDouble(this.readBuffer, num));
                                        //Main.worldSurface = (double)BitConverter.ToInt32(this.readBuffer, num);
                                        num += 4;
                                        world.setRockLayer(BitConverter.ToDouble(this.readBuffer, num));
                                        //Main.rockLayer = (double)BitConverter.ToInt32(this.readBuffer, num);
                                        num += 4;
                                        world.setId(BitConverter.ToInt32(this.readBuffer, num));
                                        //Main.worldID = BitConverter.ToInt32(this.readBuffer, num);
                                        num += 4;
                                        world.setName(Encoding.ASCII.GetString(this.readBuffer, num, length - num + start));
                                        //Main.worldName = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
                                        if (world.getServer().getNetPlay().clientSock.state == 3)
                                        {
                                            world.getServer().getNetPlay().clientSock.state = 4;
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    if (b == 8)
                                    {
                                        if (Statics.netMode == 2)
                                        {
                                            int num7 = BitConverter.ToInt32(this.readBuffer, num);
                                            num += 4;
                                            int num8 = BitConverter.ToInt32(this.readBuffer, num);
                                            num += 4;
                                            bool flag2 = true;
                                            if (num7 == -1 || num8 == -1)
                                            {
                                                flag2 = false;
                                            }
                                            else
                                            {
                                                if (num7 < 10 || num7 > Statics.maxTilesX - 10)
                                                {
                                                    flag2 = false;
                                                }
                                                else
                                                {
                                                    if (num8 < 10 || num8 > Statics.maxTilesY - 10)
                                                    {
                                                        flag2 = false;
                                                    }
                                                }
                                            }
                                            int num9 = 1350;
                                            if (flag2)
                                            {
                                                num9 *= 2;
                                            }
                                            if (world.getServer().getNetPlay().serverSock[this.whoAmI].state == 2)
                                            {
                                                world.getServer().getNetPlay().serverSock[this.whoAmI].state = 3;
                                            }
                                            NetMessage.SendData(9, world, this.whoAmI, -1, "Receiving tile data", num9, 0f, 0f, 0f);
                                            world.getServer().getNetPlay().serverSock[this.whoAmI].statusText2 = "is receiving tile data";
                                            world.getServer().getNetPlay().serverSock[this.whoAmI].statusMax += num9;
                                            int sectionX = NetPlay.GetSectionX(Statics.spawnTileX);
                                            int sectionY = NetPlay.GetSectionY(Statics.spawnTileY);
                                            for (int m = sectionX - 2; m < sectionX + 3; m++)
                                            {
                                                for (int n = sectionY - 1; n < sectionY + 2; n++)
                                                {
                                                    NetMessage.SendSection(this.whoAmI, m, n, world);
                                                }
                                            }
                                            if (flag2)
                                            {
                                                num7 = NetPlay.GetSectionX(num7);
                                                num8 = NetPlay.GetSectionY(num8);
                                                for (int num10 = num7 - 2; num10 < num7 + 3; num10++)
                                                {
                                                    for (int num11 = num8 - 1; num11 < num8 + 2; num11++)
                                                    {
                                                        NetMessage.SendSection(this.whoAmI, num10, num11, world);
                                                    }
                                                }
                                                NetMessage.SendData(11, world, this.whoAmI, -1, "", num7 - 2, (float)(num8 - 1), (float)(num7 + 2), (float)(num8 + 1));
                                            }
                                            NetMessage.SendData(11, world, this.whoAmI, -1, "", sectionX - 2, (float)(sectionY - 1), (float)(sectionX + 2), (float)(sectionY + 1));
                                            for (int num12 = 0; num12 < 200; num12++)
                                            {
                                                if (world.getItemList()[num12].active)
                                                {
                                                    NetMessage.SendData(21, world, this.whoAmI, -1, "", num12, 0f, 0f, 0f);
                                                    NetMessage.SendData(22, world, this.whoAmI, -1, "", num12, 0f, 0f, 0f);
                                                }
                                            }
                                            for (int num13 = 0; num13 < 1000; num13++)
                                            {
                                                if (world.getNPCs()[num13].active)
                                                {
                                                    NetMessage.SendData(23, world, this.whoAmI, -1, "", num13, 0f, 0f, 0f);
                                                }
                                            }
                                            NetMessage.SendData(49, world, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (b == 9)
                                        {
                                            if (Statics.netMode == 1)
                                            {
                                                int num14 = BitConverter.ToInt32(this.readBuffer, start + 1);
                                                string string4 = Encoding.ASCII.GetString(this.readBuffer, start + 5, length - 5);
                                                world.getServer().getNetPlay().clientSock.statusMax += num14;
                                                world.getServer().getNetPlay().clientSock.statusText = string4;
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            if (b == 10)
                                            {
                                                short num15 = BitConverter.ToInt16(this.readBuffer, start + 1);
                                                int num16 = BitConverter.ToInt32(this.readBuffer, start + 3);
                                                int num17 = BitConverter.ToInt32(this.readBuffer, start + 7);
                                                num = start + 11;
                                                for (int num18 = num16; num18 < num16 + (int)num15; num18++)
                                                {
                                                    if (world.getTile()[num18, num17] == null)
                                                    {
                                                        world.getTile()[num18, num17] = new Tile();
                                                    }
                                                    byte b2 = this.readBuffer[num];
                                                    num++;
                                                    bool active = world.getTile()[num18, num17].active;
                                                    if ((b2 & 1) == 1)
                                                    {
                                                        world.getTile()[num18, num17].active = true;
                                                    }
                                                    else
                                                    {
                                                        world.getTile()[num18, num17].active = false;
                                                    }
                                                    if ((b2 & 2) == 2)
                                                    {
                                                        world.getTile()[num18, num17].lighted = true;
                                                    }
                                                    if ((b2 & 4) == 4)
                                                    {
                                                        world.getTile()[num18, num17].wall = 1;
                                                    }
                                                    else
                                                    {
                                                        world.getTile()[num18, num17].wall = 0;
                                                    }
                                                    if ((b2 & 8) == 8)
                                                    {
                                                        world.getTile()[num18, num17].liquid = 1;
                                                    }
                                                    else
                                                    {
                                                        world.getTile()[num18, num17].liquid = 0;
                                                    }
                                                    if (world.getTile()[num18, num17].active)
                                                    {
                                                        int type = (int)world.getTile()[num18, num17].type;
                                                        world.getTile()[num18, num17].type = this.readBuffer[num];
                                                        num++;
                                                        if (Statics.tileFrameImportant[(int)world.getTile()[num18, num17].type])
                                                        {
                                                            world.getTile()[num18, num17].frameX = BitConverter.ToInt16(this.readBuffer, num);
                                                            num += 2;
                                                            world.getTile()[num18, num17].frameY = BitConverter.ToInt16(this.readBuffer, num);
                                                            num += 2;
                                                        }
                                                        else
                                                        {
                                                            if (!active || (int)world.getTile()[num18, num17].type != type)
                                                            {
                                                                world.getTile()[num18, num17].frameX = -1;
                                                                world.getTile()[num18, num17].frameY = -1;
                                                            }
                                                        }
                                                    }
                                                    if (world.getTile()[num18, num17].wall > 0)
                                                    {
                                                        world.getTile()[num18, num17].wall = this.readBuffer[num];
                                                        num++;
                                                    }
                                                    if (world.getTile()[num18, num17].liquid > 0)
                                                    {
                                                        world.getTile()[num18, num17].liquid = this.readBuffer[num];
                                                        num++;
                                                        byte b3 = this.readBuffer[num];
                                                        num++;
                                                        if (b3 == 1)
                                                        {
                                                            world.getTile()[num18, num17].lava = true;
                                                        }
                                                        else
                                                        {
                                                            world.getTile()[num18, num17].lava = false;
                                                        }
                                                    }
                                                }
                                                if (Statics.netMode == 2)
                                                {
                                                    NetMessage.SendData((int)b, world, -1, this.whoAmI, "", (int)num15, (float)num16, (float)num17, 0f);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                if (b == 11)
                                                {
                                                    if (Statics.netMode == 1)
                                                    {
                                                        int startX = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                        num += 4;
                                                        int startY = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                        num += 4;
                                                        int endX = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                        num += 4;
                                                        int endY = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                        num += 4;
                                                        WorldGen.SectionTileFrame(startX, startY, world, endX, endY);
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    if (b == 12)
                                                    {
                                                        int num19 = (int)this.readBuffer[num];
                                                        num++;
                                                        world.getPlayerList()[num19].SpawnX = BitConverter.ToInt32(this.readBuffer, num);
                                                        num += 4;
                                                        world.getPlayerList()[num19].SpawnY = BitConverter.ToInt32(this.readBuffer, num);
                                                        num += 4;
                                                        world.getPlayerList()[num19].Spawn();
                                                        if (Statics.netMode == 2 && world.getServer().getNetPlay().serverSock[this.whoAmI].state >= 3)
                                                        {
                                                            NetMessage.buffer[this.whoAmI].broadcast = true;
                                                            NetMessage.SendData(12, world, -1, this.whoAmI, "", this.whoAmI, 0f, 0f, 0f);
                                                            if (world.getServer().getNetPlay().serverSock[this.whoAmI].state == 3)
                                                            {
                                                                world.getServer().getNetPlay().serverSock[this.whoAmI].state = 10;
                                                                NetMessage.greetPlayer(this.whoAmI, world);
                                                                NetMessage.syncPlayers(world);
                                                                return;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (b == 13)
                                                        {
                                                            int num20 = (int)this.readBuffer[num];
                                                            if (Statics.netMode == 1 && !world.getPlayerList()[num20].active)
                                                            {
                                                                NetMessage.SendData(15, world, -1, -1, "", 0, 0f, 0f, 0f);
                                                            }
                                                            num++;
                                                            int num21 = (int)this.readBuffer[num];
                                                            num++;
                                                            int selectedItem = (int)this.readBuffer[num];
                                                            num++;
                                                            float x = BitConverter.ToSingle(this.readBuffer, num);
                                                            num += 4;
                                                            float num22 = BitConverter.ToSingle(this.readBuffer, num);
                                                            num += 4;
                                                            float x2 = BitConverter.ToSingle(this.readBuffer, num);
                                                            num += 4;
                                                            float y = BitConverter.ToSingle(this.readBuffer, num);
                                                            num += 4;
                                                            world.getPlayerList()[num20].selectedItem = selectedItem;
                                                            world.getPlayerList()[num20].position.X = x;
                                                            world.getPlayerList()[num20].position.Y = num22;
                                                            world.getPlayerList()[num20].velocity.X = x2;
                                                            world.getPlayerList()[num20].velocity.Y = y;
                                                            world.getPlayerList()[num20].oldVelocity = world.getPlayerList()[num20].velocity;
                                                            world.getPlayerList()[num20].fallStart = (int)(num22 / 16f);
                                                            world.getPlayerList()[num20].controlUp = false;
                                                            world.getPlayerList()[num20].controlDown = false;
                                                            world.getPlayerList()[num20].controlLeft = false;
                                                            world.getPlayerList()[num20].controlRight = false;
                                                            world.getPlayerList()[num20].controlJump = false;
                                                            world.getPlayerList()[num20].controlUseItem = false;
                                                            world.getPlayerList()[num20].direction = -1;
                                                            if ((num21 & 1) == 1)
                                                            {
                                                                world.getPlayerList()[num20].controlUp = true;
                                                            }
                                                            if ((num21 & 2) == 2)
                                                            {
                                                                world.getPlayerList()[num20].controlDown = true;
                                                            }
                                                            if ((num21 & 4) == 4)
                                                            {
                                                                world.getPlayerList()[num20].controlLeft = true;
                                                            }
                                                            if ((num21 & 8) == 8)
                                                            {
                                                                world.getPlayerList()[num20].controlRight = true;
                                                            }
                                                            if ((num21 & 16) == 16)
                                                            {
                                                                world.getPlayerList()[num20].controlJump = true;
                                                            }
                                                            if ((num21 & 32) == 32)
                                                            {
                                                                world.getPlayerList()[num20].controlUseItem = true;
                                                            }
                                                            if ((num21 & 64) == 64)
                                                            {
                                                                world.getPlayerList()[num20].direction = 1;
                                                            }
                                                            if (Statics.netMode == 2 && world.getServer().getNetPlay().serverSock[this.whoAmI].state == 10)
                                                            {
                                                                NetMessage.SendData(13, world, -1, this.whoAmI, "", num20, 0f, 0f, 0f);
                                                                return;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (b == 14)
                                                            {
                                                                if (Statics.netMode == 1)
                                                                {
                                                                    int num23 = (int)this.readBuffer[num];
                                                                    num++;
                                                                    int num24 = (int)this.readBuffer[num];
                                                                    if (num24 == 1)
                                                                    {
                                                                        if (world.getPlayerList()[num23].active)
                                                                        {
                                                                            world.getPlayerList()[num23] = new Player(world);
                                                                        }
                                                                        world.getPlayerList()[num23].active = true;
                                                                        return;
                                                                    }
                                                                    world.getPlayerList()[num23].active = false;
                                                                    return;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (b == 15)
                                                                {
                                                                    if (Statics.netMode == 2)
                                                                    {
                                                                        NetMessage.syncPlayers(world);
                                                                        return;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (b == 16)
                                                                    {
                                                                        int num25 = (int)this.readBuffer[num];
                                                                        num++;
                                                                        int statLife = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                        num += 2;
                                                                        int statLifeMax = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                        if (Statics.netMode == 2)
                                                                        {
                                                                            num25 = this.whoAmI;
                                                                        }
                                                                        world.getPlayerList()[num25].statLife = statLife;
                                                                        world.getPlayerList()[num25].statLifeMax = statLifeMax;
                                                                        if (world.getPlayerList()[num25].statLife <= 0)
                                                                        {
                                                                            world.getPlayerList()[num25].dead = true;
                                                                        }
                                                                        if (Statics.netMode == 2)
                                                                        {
                                                                            NetMessage.SendData(16, world, -1, this.whoAmI, "", num25, 0f, 0f, 0f);
                                                                            return;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (b == 17)
                                                                        {
                                                                            byte b4 = this.readBuffer[num];
                                                                            num++;
                                                                            int num26 = BitConverter.ToInt32(this.readBuffer, num);
                                                                            num += 4;
                                                                            int num27 = BitConverter.ToInt32(this.readBuffer, num);
                                                                            num += 4;
                                                                            byte b5 = this.readBuffer[num];
                                                                            bool fail = false;
                                                                            if (b5 == 1)
                                                                            {
                                                                                fail = true;
                                                                            }
                                                                            if (world.getTile()[num26, num27] == null)
                                                                            {
                                                                                world.getTile()[num26, num27] = new Tile();
                                                                            }
                                                                            if (Statics.netMode == 2 && !world.getServer().getNetPlay().serverSock[this.whoAmI].tileSection[NetPlay.GetSectionX(num26), NetPlay.GetSectionY(num27)])
                                                                            {
                                                                                fail = true;
                                                                            }
                                                                            if (b4 == 0)
                                                                            {
                                                                                WorldGen.KillTile(num26, num27, world, fail, false, false);
                                                                            }
                                                                            else
                                                                            {
                                                                                if (b4 == 1)
                                                                                {
                                                                                    WorldGen.PlaceTile(num26, num27, world, (int)b5, false, true, -1);
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (b4 == 2)
                                                                                    {
                                                                                        WorldGen.KillWall(num26, num27, world, fail);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (b4 == 3)
                                                                                        {
                                                                                            WorldGen.PlaceWall(num26, num27, (int)b5, world, false);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (b4 == 4)
                                                                                            {
                                                                                                WorldGen.KillTile(num26, num27, world, fail, false, true);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            if (Statics.netMode == 2)
                                                                            {
                                                                                NetMessage.SendData(17, world, -1, this.whoAmI, "", (int)b4, (float)num26, (float)num27, (float)b5);
                                                                                if (b4 == 1 && b5 == 53)
                                                                                {
                                                                                    NetMessage.SendTileSquare(-1, num26, num27, 1, world);
                                                                                    return;
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (b == 18)
                                                                            {
                                                                                if (Statics.netMode == 1)
                                                                                {
                                                                                    byte b6 = this.readBuffer[num];
                                                                                    num++;
                                                                                    int num28 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                    num += 4;
                                                                                    short sunModY = BitConverter.ToInt16(this.readBuffer, num);
                                                                                    num += 2;
                                                                                    short moonModY = BitConverter.ToInt16(this.readBuffer, num);
                                                                                    num += 2;
                                                                                    if (b6 == 1)
                                                                                    {
                                                                                        world.setDayTime(true);
                                                                                        //Main.dayTime = true;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        world.setDayTime(false);
                                                                                        //Main.dayTime = false;
                                                                                    }
                                                                                    world.setTime((double)num28);
                                                                                    world.setSunModY(sunModY);
                                                                                    world.setMoonModY(moonModY);
                                                                                    //Main.time = (double)num28;
                                                                                    //Main.sunModY = sunModY;
                                                                                    //Main.moonModY = moonModY;
                                                                                    if (Statics.netMode == 2)
                                                                                    {
                                                                                        NetMessage.SendData(18, world, -1, this.whoAmI, "", 0, 0f, 0f, 0f);
                                                                                        return;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (b == 19)
                                                                                {
                                                                                    byte b7 = this.readBuffer[num];
                                                                                    num++;
                                                                                    int num29 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                    num += 4;
                                                                                    int num30 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                    num += 4;
                                                                                    int num31 = (int)this.readBuffer[num];
                                                                                    int direction = 0;
                                                                                    if (num31 == 0)
                                                                                    {
                                                                                        direction = -1;
                                                                                    }
                                                                                    if (b7 == 0)
                                                                                    {
                                                                                        WorldGen.OpenDoor(num29, num30, direction, world);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (b7 == 1)
                                                                                        {
                                                                                            WorldGen.CloseDoor(num29, num30, world, true);
                                                                                        }
                                                                                    }
                                                                                    if (Statics.netMode == 2)
                                                                                    {
                                                                                        NetMessage.SendData(19, world, -1, this.whoAmI, "", (int)b7, (float)num29, (float)num30, (float)num31);
                                                                                        return;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (b == 20)
                                                                                    {
                                                                                        short num32 = BitConverter.ToInt16(this.readBuffer, start + 1);
                                                                                        int num33 = BitConverter.ToInt32(this.readBuffer, start + 3);
                                                                                        int num34 = BitConverter.ToInt32(this.readBuffer, start + 7);
                                                                                        num = start + 11;
                                                                                        for (int num35 = num33; num35 < num33 + (int)num32; num35++)
                                                                                        {
                                                                                            for (int num36 = num34; num36 < num34 + (int)num32; num36++)
                                                                                            {
                                                                                                if (world.getTile()[num35, num36] == null)
                                                                                                {
                                                                                                    world.getTile()[num35, num36] = new Tile();
                                                                                                }
                                                                                                byte b8 = this.readBuffer[num];
                                                                                                num++;
                                                                                                bool active2 = world.getTile()[num35, num36].active;
                                                                                                if ((b8 & 1) == 1)
                                                                                                {
                                                                                                    world.getTile()[num35, num36].active = true;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    world.getTile()[num35, num36].active = false;
                                                                                                }
                                                                                                if ((b8 & 2) == 2)
                                                                                                {
                                                                                                    world.getTile()[num35, num36].lighted = true;
                                                                                                }
                                                                                                if ((b8 & 4) == 4)
                                                                                                {
                                                                                                    world.getTile()[num35, num36].wall = 1;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    world.getTile()[num35, num36].wall = 0;
                                                                                                }
                                                                                                if ((b8 & 8) == 8)
                                                                                                {
                                                                                                    world.getTile()[num35, num36].liquid = 1;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    world.getTile()[num35, num36].liquid = 0;
                                                                                                }
                                                                                                if (world.getTile()[num35, num36].active)
                                                                                                {
                                                                                                    int type2 = (int)world.getTile()[num35, num36].type;
                                                                                                    world.getTile()[num35, num36].type = this.readBuffer[num];
                                                                                                    num++;
                                                                                                    if (Statics.tileFrameImportant[(int)world.getTile()[num35, num36].type])
                                                                                                    {
                                                                                                        world.getTile()[num35, num36].frameX = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                        num += 2;
                                                                                                        world.getTile()[num35, num36].frameY = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                        num += 2;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (!active2 || (int)world.getTile()[num35, num36].type != type2)
                                                                                                        {
                                                                                                            world.getTile()[num35, num36].frameX = -1;
                                                                                                            world.getTile()[num35, num36].frameY = -1;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                                if (world.getTile()[num35, num36].wall > 0)
                                                                                                {
                                                                                                    world.getTile()[num35, num36].wall = this.readBuffer[num];
                                                                                                    num++;
                                                                                                }
                                                                                                if (world.getTile()[num35, num36].liquid > 0)
                                                                                                {
                                                                                                    world.getTile()[num35, num36].liquid = this.readBuffer[num];
                                                                                                    num++;
                                                                                                    byte b9 = this.readBuffer[num];
                                                                                                    num++;
                                                                                                    if (b9 == 1)
                                                                                                    {
                                                                                                        world.getTile()[num35, num36].lava = true;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        world.getTile()[num35, num36].lava = false;
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        WorldGen.RangeFrame(num33, num34, num33 + (int)num32, num34 + (int)num32, world);
                                                                                        if (Statics.netMode == 2)
                                                                                        {
                                                                                            NetMessage.SendData((int)b, world, -1, this.whoAmI, "", (int)num32, (float)num33, (float)num34, 0f);
                                                                                            return;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (b == 21)
                                                                                        {
                                                                                            short num37 = BitConverter.ToInt16(this.readBuffer, num);
                                                                                            num += 2;
                                                                                            float num38 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                            num += 4;
                                                                                            float num39 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                            num += 4;
                                                                                            float x3 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                            num += 4;
                                                                                            float y2 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                            num += 4;
                                                                                            byte stack2 = this.readBuffer[num];
                                                                                            num++;
                                                                                            string string5 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
                                                                                            if (Statics.netMode == 1)
                                                                                            {
                                                                                                if (string5 == "0")
                                                                                                {
                                                                                                    world.getItemList()[(int)num37].active = false;
                                                                                                    return;
                                                                                                }
                                                                                                world.getItemList()[(int)num37].SetDefaults(string5);
                                                                                                world.getItemList()[(int)num37].stack = (int)stack2;
                                                                                                world.getItemList()[(int)num37].position.X = num38;
                                                                                                world.getItemList()[(int)num37].position.Y = num39;
                                                                                                world.getItemList()[(int)num37].velocity.X = x3;
                                                                                                world.getItemList()[(int)num37].velocity.Y = y2;
                                                                                                world.getItemList()[(int)num37].active = true;
                                                                                                world.getItemList()[(int)num37].wet = Collision.WetCollision(world.getItemList()[(int)num37].position, world.getItemList()[(int)num37].width, world.getItemList()[(int)num37].height, world);
                                                                                                return;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (string5 == "0")
                                                                                                {
                                                                                                    if (num37 < 200)
                                                                                                    {
                                                                                                        world.getItemList()[(int)num37].active = false;
                                                                                                        NetMessage.SendData(21, world, -1, -1, "", (int)num37, 0f, 0f, 0f);
                                                                                                        return;
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    bool flag3 = false;
                                                                                                    if (num37 == 200)
                                                                                                    {
                                                                                                        flag3 = true;
                                                                                                    }
                                                                                                    if (flag3)
                                                                                                    {
                                                                                                        Item item = new Item();
                                                                                                        item.SetDefaults(string5);
                                                                                                        num37 = (short)Item.NewItem((int)num38, (int)num39, world, item.width, item.height, item.type, (int)stack2, true);
                                                                                                    }
                                                                                                    world.getItemList()[(int)num37].SetDefaults(string5);
                                                                                                    world.getItemList()[(int)num37].stack = (int)stack2;
                                                                                                    world.getItemList()[(int)num37].position.X = num38;
                                                                                                    world.getItemList()[(int)num37].position.Y = num39;
                                                                                                    world.getItemList()[(int)num37].velocity.X = x3;
                                                                                                    world.getItemList()[(int)num37].velocity.Y = y2;
                                                                                                    world.getItemList()[(int)num37].active = true;
                                                                                                    world.getItemList()[(int)num37].owner = Statics.myPlayer;
                                                                                                    if (flag3)
                                                                                                    {
                                                                                                        NetMessage.SendData(21, world, -1, -1, "", (int)num37, 0f, 0f, 0f);
                                                                                                        world.getItemList()[(int)num37].ownIgnore = this.whoAmI;
                                                                                                        world.getItemList()[(int)num37].ownTime = 100;
                                                                                                        world.getItemList()[(int)num37].FindOwner((int)num37, world);
                                                                                                        return;
                                                                                                    }
                                                                                                    NetMessage.SendData(21, world, -1, this.whoAmI, "", (int)num37, 0f, 0f, 0f);
                                                                                                    return;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (b == 22)
                                                                                            {
                                                                                                short num40 = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                num += 2;
                                                                                                byte b10 = this.readBuffer[num];
                                                                                                world.getItemList()[(int)num40].owner = (int)b10;
                                                                                                if ((int)b10 == Statics.myPlayer)
                                                                                                {
                                                                                                    world.getItemList()[(int)num40].keepTime = 15;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    world.getItemList()[(int)num40].keepTime = 0;
                                                                                                }
                                                                                                if (Statics.netMode == 2)
                                                                                                {
                                                                                                    world.getItemList()[(int)num40].owner = 8;
                                                                                                    world.getItemList()[(int)num40].keepTime = 15;
                                                                                                    NetMessage.SendData(22, world, -1, -1, "", (int)num40, 0f, 0f, 0f);
                                                                                                    return;
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (b == 23)
                                                                                                {
                                                                                                    short num41 = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                    num += 2;
                                                                                                    float x4 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                    num += 4;
                                                                                                    float y3 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                    num += 4;
                                                                                                    float x5 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                    num += 4;
                                                                                                    float y4 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                    num += 4;
                                                                                                    int target = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                                                    num += 2;
                                                                                                    int direction2 = (int)(this.readBuffer[num] - 1);
                                                                                                    num++;
                                                                                                    byte arg_20BE_0 = this.readBuffer[num];
                                                                                                    num++;
                                                                                                    int num42 = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                                                    num += 2;
                                                                                                    float[] array = new float[NPC.maxAI];
                                                                                                    for (int num43 = 0; num43 < NPC.maxAI; num43++)
                                                                                                    {
                                                                                                        array[num43] = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                        num += 4;
                                                                                                    }
                                                                                                    string string6 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
                                                                                                    if (!world.getNPCs()[(int)num41].active || world.getNPCs()[(int)num41].name != string6)
                                                                                                    {
                                                                                                        world.getNPCs()[(int)num41].active = true;
                                                                                                        world.getNPCs()[(int)num41].SetDefaults(string6);
                                                                                                    }
                                                                                                    world.getNPCs()[(int)num41].position.X = x4;
                                                                                                    world.getNPCs()[(int)num41].position.Y = y3;
                                                                                                    world.getNPCs()[(int)num41].velocity.X = x5;
                                                                                                    world.getNPCs()[(int)num41].velocity.Y = y4;
                                                                                                    world.getNPCs()[(int)num41].target = target;
                                                                                                    world.getNPCs()[(int)num41].direction = direction2;
                                                                                                    world.getNPCs()[(int)num41].life = num42;
                                                                                                    if (num42 <= 0)
                                                                                                    {
                                                                                                        world.getNPCs()[(int)num41].active = false;
                                                                                                    }
                                                                                                    for (int num44 = 0; num44 < NPC.maxAI; num44++)
                                                                                                    {
                                                                                                        world.getNPCs()[(int)num41].ai[num44] = array[num44];
                                                                                                    }
                                                                                                    return;
                                                                                                }
                                                                                                if (b == 24)
                                                                                                {
                                                                                                    short num45 = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                    num += 2;
                                                                                                    byte b11 = this.readBuffer[num];
                                                                                                    world.getNPCs()[(int)num45].StrikeNPC(world.getPlayerList()[(int)b11].inventory[world.getPlayerList()[(int)b11].selectedItem].damage,
                                                                                                        world.getPlayerList()[(int)b11].inventory[world.getPlayerList()[(int)b11].selectedItem].knockBack, world.getPlayerList()[(int)b11].direction, world);
                                                                                                    if (Statics.netMode == 2)
                                                                                                    {
                                                                                                        NetMessage.SendData(24, world, -1, this.whoAmI, "", (int)num45, (float)b11, 0f, 0f);
                                                                                                        NetMessage.SendData(23, world, -1, -1, "", (int)num45, 0f, 0f, 0f);
                                                                                                        return;
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (b == 25)
                                                                                                    {
                                                                                                        int num46 = (int)this.readBuffer[start + 1];
                                                                                                        if (Statics.netMode == 2)
                                                                                                        {
                                                                                                            num46 = this.whoAmI;
                                                                                                        }
                                                                                                        byte b12 = this.readBuffer[start + 2];
                                                                                                        byte b13 = this.readBuffer[start + 3];
                                                                                                        byte b14 = this.readBuffer[start + 4];
                                                                                                        string string7 = Encoding.ASCII.GetString(this.readBuffer, start + 5, length - 5);
                                                                                                        if (Statics.netMode == 1)
                                                                                                        {
                                                                                                            string newText = string7;
                                                                                                            if (num46 < 8)
                                                                                                            {
                                                                                                                newText = "<" + world.getPlayerList()[num46].name + "> " + string7;
                                                                                                                world.getPlayerList()[num46].chatText = string7;
                                                                                                                world.getPlayerList()[num46].chatShowTime = Statics.chatLength / 2;
                                                                                                            }
                                                                                                            //Main.NewText(newText, b12, b13, b14);
                                                                                                            Console.WriteLine(newText);
                                                                                                            return;
                                                                                                        }
                                                                                                        if (Statics.netMode == 2)
                                                                                                        {
                                                                                                            string text = string7.ToLower();
                                                                                                            if (text == "/playing")
                                                                                                            {
                                                                                                                string text2 = "";
                                                                                                                for (int num47 = 0; num47 < 8; num47++)
                                                                                                                {
                                                                                                                    if (world.getPlayerList()[num47].active)
                                                                                                                    {
                                                                                                                        if (text2 == "")
                                                                                                                        {
                                                                                                                            text2 += world.getPlayerList()[num47].name;
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            text2 = text2 + ", " + world.getPlayerList()[num47].name;
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                                NetMessage.SendData(25, world, this.whoAmI, -1, "Current players: " + text2 + ".", 255, 255f, 240f, 20f);
                                                                                                                return;
                                                                                                            }
                                                                                                            if (text.Length >= 4 && text.Substring(0, 4) == "/me ")
                                                                                                            {
                                                                                                                NetMessage.SendData(25, world, -1, -1, "*" + world.getPlayerList()[this.whoAmI].name + " " + string7.Substring(4), 255, 200f, 100f, 0f);
                                                                                                                return;
                                                                                                            }
                                                                                                            if (text.Length >= 6 && text.Substring(0, 6) == "/give ")
                                                                                                            {
                                                                                                                Player player2 = world.getPlayerList()[this.whoAmI];
                                                                                                               //for (int i = 0; i < 1; i++)
                                                                                                                //{
                                                                                                                    int item = Item.NewItem((int)player2.position.X, (int)player2.position.Y, world, player2.width, player2.height, 0, 1, false);
                                                                                                                    world.getItemList()[item].SetDefaults(string7.Substring(6));
                                                                                                                //}
                                                                                                                    NetMessage.SendData(25, world, this.whoAmI, -1, "Given item " + string7.Substring(6) + "!", 255, (float)b12, (float)b13, (float)b14);
                                                                                                                return;
                                                                                                            }
                                                                                                            if (text.Length < 3 || !(text.Substring(0, 3) == "/p "))
                                                                                                            {
                                                                                                                NetMessage.SendData(25, world, -1, -1, string7, num46, (float)b12, (float)b13, (float)b14);
                                                                                                                return;
                                                                                                            }
                                                                                                            if (world.getPlayerList()[this.whoAmI].team != 0)
                                                                                                            {
                                                                                                                for (int num48 = 0; num48 < 8; num48++)
                                                                                                                {
                                                                                                                    if (world.getPlayerList()[num48].team == world.getPlayerList()[this.whoAmI].team)
                                                                                                                    {
                                                                                                                        NetMessage.SendData(25, world, num48, -1, string7.Substring(3), num46, 
                                                                                                                            (float)Statics.teamColor[world.getPlayerList()[this.whoAmI].team].R, 
                                                                                                                            (float)Statics.teamColor[world.getPlayerList()[this.whoAmI].team].G,
                                                                                                                            (float)Statics.teamColor[world.getPlayerList()[this.whoAmI].team].B);
                                                                                                                    }
                                                                                                                }
                                                                                                                return;
                                                                                                            }
                                                                                                            NetMessage.SendData(25, world, this.whoAmI, -1, "You are not in a party!", 255, 255f, 240f, 20f);
                                                                                                            return;
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (b == 26)
                                                                                                        {
                                                                                                            byte b15 = this.readBuffer[num];
                                                                                                            num++;
                                                                                                            int num49 = (int)(this.readBuffer[num] - 1);
                                                                                                            num++;
                                                                                                            short num50 = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                            num += 2;
                                                                                                            byte b16 = this.readBuffer[num];
                                                                                                            bool pvp = false;
                                                                                                            if (b16 != 0)
                                                                                                            {
                                                                                                                pvp = true;
                                                                                                            }
                                                                                                            world.getPlayerList()[(int)b15].Hurt((int)num50, num49, pvp, true);
                                                                                                            if (Statics.netMode == 2)
                                                                                                            {
                                                                                                                NetMessage.SendData(26, world, -1, this.whoAmI, "", (int)b15, (float)num49, (float)num50, (float)b16);
                                                                                                                return;
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (b == 27)
                                                                                                            {
                                                                                                                short num51 = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                num += 2;
                                                                                                                float x6 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                                num += 4;
                                                                                                                float y5 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                                num += 4;
                                                                                                                float x7 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                                num += 4;
                                                                                                                float y6 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                                num += 4;
                                                                                                                float knockBack = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                                num += 4;
                                                                                                                short damage = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                num += 2;
                                                                                                                byte b17 = this.readBuffer[num];
                                                                                                                num++;
                                                                                                                byte b18 = this.readBuffer[num];
                                                                                                                num++;
                                                                                                                float[] array2 = new float[Projectile.maxAI];
                                                                                                                for (int num52 = 0; num52 < Projectile.maxAI; num52++)
                                                                                                                {
                                                                                                                    array2[num52] = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                                    num += 4;
                                                                                                                }
                                                                                                                int num53 = 1000;
                                                                                                                for (int num54 = 0; num54 < 1000; num54++)
                                                                                                                {
                                                                                                                    if (world.getProjectile()[num54].owner == (int)b17 && world.getProjectile()[num54].identity == (int)num51 && world.getProjectile()[num54].active)
                                                                                                                    {
                                                                                                                        num53 = num54;
                                                                                                                        break;
                                                                                                                    }
                                                                                                                }
                                                                                                                if (num53 == 1000)
                                                                                                                {
                                                                                                                    for (int num55 = 0; num55 < 1000; num55++)
                                                                                                                    {
                                                                                                                        if (!world.getProjectile()[num55].active)
                                                                                                                        {
                                                                                                                            num53 = num55;
                                                                                                                            break;
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                                if (!world.getProjectile()[num53].active || world.getProjectile()[num53].type != (int)b18)
                                                                                                                {
                                                                                                                    world.getProjectile()[num53].SetDefaults((int)b18);
                                                                                                                }
                                                                                                                world.getProjectile()[num53].identity = (int)num51;
                                                                                                                world.getProjectile()[num53].position.X = x6;
                                                                                                                world.getProjectile()[num53].position.Y = y5;
                                                                                                                world.getProjectile()[num53].velocity.X = x7;
                                                                                                                world.getProjectile()[num53].velocity.Y = y6;
                                                                                                                world.getProjectile()[num53].damage = (int)damage;
                                                                                                                world.getProjectile()[num53].type = (int)b18;
                                                                                                                world.getProjectile()[num53].owner = (int)b17;
                                                                                                                world.getProjectile()[num53].knockBack = knockBack;
                                                                                                                for (int num56 = 0; num56 < Projectile.maxAI; num56++)
                                                                                                                {
                                                                                                                    world.getProjectile()[num53].ai[num56] = array2[num56];
                                                                                                                }
                                                                                                                if (Statics.netMode == 2)
                                                                                                                {
                                                                                                                    NetMessage.SendData(27, world, -1, this.whoAmI, "", num53, 0f, 0f, 0f);
                                                                                                                    return;
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (b == 28)
                                                                                                                {
                                                                                                                    short num57 = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                    num += 2;
                                                                                                                    short num58 = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                    num += 2;
                                                                                                                    float num59 = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                                    num += 4;
                                                                                                                    int num60 = (int)(this.readBuffer[num] - 1);
                                                                                                                    if (num58 >= 0)
                                                                                                                    {
                                                                                                                        world.getNPCs()[(int)num57].StrikeNPC((int)num58, num59, num60, world);
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        world.getNPCs()[(int)num57].life = 0;
                                                                                                                        world.getNPCs()[(int)num57].HitEffect(world, 0, 10.0);
                                                                                                                        world.getNPCs()[(int)num57].active = false;
                                                                                                                    }
                                                                                                                    if (Statics.netMode == 2)
                                                                                                                    {
                                                                                                                        NetMessage.SendData(28, world, -1, this.whoAmI, "", (int)num57, (float)num58, num59, (float)num60);
                                                                                                                        NetMessage.SendData(23, world, -1, -1, "", (int)num57, 0f, 0f, 0f);
                                                                                                                        return;
                                                                                                                    }
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (b == 29)
                                                                                                                    {
                                                                                                                        short num61 = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                        num += 2;
                                                                                                                        byte b19 = this.readBuffer[num];
                                                                                                                        for (int num62 = 0; num62 < 1000; num62++)
                                                                                                                        {
                                                                                                                            if (world.getProjectile()[num62].owner == (int)b19 && world.getProjectile()[num62].identity == (int)num61 && world.getProjectile()[num62].active)
                                                                                                                            {
                                                                                                                                world.getProjectile()[num62].Kill(world);
                                                                                                                                break;
                                                                                                                            }
                                                                                                                        }
                                                                                                                        if (Statics.netMode == 2)
                                                                                                                        {
                                                                                                                            NetMessage.SendData(29, world, -1, this.whoAmI, "", (int)num61, (float)b19, 0f, 0f);
                                                                                                                            return;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (b == 30)
                                                                                                                        {
                                                                                                                            byte b20 = this.readBuffer[num];
                                                                                                                            num++;
                                                                                                                            byte b21 = this.readBuffer[num];
                                                                                                                            if (b21 == 1)
                                                                                                                            {
                                                                                                                                world.getPlayerList()[(int)b20].hostile = true;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                world.getPlayerList()[(int)b20].hostile = false;
                                                                                                                            }
                                                                                                                            if (Statics.netMode == 2)
                                                                                                                            {
                                                                                                                                NetMessage.SendData(30, world, -1, this.whoAmI, "", (int)b20, 0f, 0f, 0f);
                                                                                                                                string str = " has enabled PvP!";
                                                                                                                                if (b21 == 0)
                                                                                                                                {
                                                                                                                                    str = " has disabled PvP!";
                                                                                                                                }
                                                                                                                                NetMessage.SendData(25, world, -1, -1, world.getPlayerList()[(int)b20].name + str, 255, 
                                                                                                                                    (float)Statics.teamColor[world.getPlayerList()[(int)b20].team].R,
                                                                                                                                    (float)Statics.teamColor[world.getPlayerList()[(int)b20].team].G,
                                                                                                                                    (float)Statics.teamColor[world.getPlayerList()[(int)b20].team].B);
                                                                                                                                return;
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (b == 31)
                                                                                                                            {
                                                                                                                                if (Statics.netMode == 2)
                                                                                                                                {
                                                                                                                                    int x8 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                                                                    num += 4;
                                                                                                                                    int y7 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                                                                    num += 4;
                                                                                                                                    int num63 = Chest.FindChest(x8, y7, world);
                                                                                                                                    if (num63 > -1 && Chest.UsingChest(num63, world) == -1)
                                                                                                                                    {
                                                                                                                                        for (int num64 = 0; num64 < Chest.maxItems; num64++)
                                                                                                                                        {
                                                                                                                                            NetMessage.SendData(32, world, this.whoAmI, -1, "", num63, (float)num64, 0f, 0f);
                                                                                                                                        }
                                                                                                                                        NetMessage.SendData(33, world, this.whoAmI, -1, "", num63, 0f, 0f, 0f);
                                                                                                                                        world.getPlayerList()[this.whoAmI].chest = num63;
                                                                                                                                        return;
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (b == 32)
                                                                                                                                {
                                                                                                                                    int num65 = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                                    num += 2;
                                                                                                                                    int num66 = (int)this.readBuffer[num];
                                                                                                                                    num++;
                                                                                                                                    int stack3 = (int)this.readBuffer[num];
                                                                                                                                    num++;
                                                                                                                                    string string8 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
                                                                                                                                    if (world.getChests()[num65] == null)
                                                                                                                                    {
                                                                                                                                        world.getChests()[num65] = new Chest();
                                                                                                                                    }
                                                                                                                                    if (world.getChests()[num65].item[num66] == null)
                                                                                                                                    {
                                                                                                                                        world.getChests()[num65].item[num66] = new Item();
                                                                                                                                    }
                                                                                                                                    world.getChests()[num65].item[num66].SetDefaults(string8);
                                                                                                                                    world.getChests()[num65].item[num66].stack = stack3;
                                                                                                                                    return;
                                                                                                                                }
                                                                                                                                if (b == 33)
                                                                                                                                {
                                                                                                                                    int num67 = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                                    num += 2;
                                                                                                                                    int chestX = BitConverter.ToInt32(this.readBuffer, num);
                                                                                                                                    num += 4;
                                                                                                                                    int chestY = BitConverter.ToInt32(this.readBuffer, num);
                                                                                                                                    if (Statics.netMode == 1)
                                                                                                                                    {
                                                                                                                                        if (world.getPlayerList()[Statics.myPlayer].chest == -1)
                                                                                                                                        {
                                                                                                                                            Statics.playerInventory = true;
                                                                                                                                            //Main.PlaySound(10, -1, -1, 1);
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (world.getPlayerList()[Statics.myPlayer].chest != num67 && num67 != -1)
                                                                                                                                            {
                                                                                                                                                Statics.playerInventory = true;
                                                                                                                                                //Main.PlaySound(12, -1, -1, 1);
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (world.getPlayerList()[Statics.myPlayer].chest != -1 && num67 == -1)
                                                                                                                                                {
                                                                                                                                                    //Main.PlaySound(11, -1, -1, 1);
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        world.getPlayerList()[Statics.myPlayer].chest = num67;
                                                                                                                                        world.getPlayerList()[Statics.myPlayer].chestX = chestX;
                                                                                                                                        world.getPlayerList()[Statics.myPlayer].chestY = chestY;
                                                                                                                                        return;
                                                                                                                                    }
                                                                                                                                    world.getPlayerList()[this.whoAmI].chest = num67;
                                                                                                                                    return;
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    if (b == 34)
                                                                                                                                    {
                                                                                                                                        if (Statics.netMode == 2)
                                                                                                                                        {
                                                                                                                                            int num68 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                                                                            num += 4;
                                                                                                                                            int num69 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                                                                            WorldGen.KillTile(num68, num69, world, false, false, false);
                                                                                                                                            if (!world.getTile()[num68, num69].active)
                                                                                                                                            {
                                                                                                                                                NetMessage.SendData(17, world, -1, -1, "", 0, (float)num68, (float)num69, 0f);
                                                                                                                                                return;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (b == 35)
                                                                                                                                        {
                                                                                                                                            int num70 = (int)this.readBuffer[num];
                                                                                                                                            num++;
                                                                                                                                            int num71 = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                                            num += 2;
                                                                                                                                            if (num70 != Statics.myPlayer)
                                                                                                                                            {
                                                                                                                                                world.getPlayerList()[num70].HealEffect(num71);
                                                                                                                                            }
                                                                                                                                            if (Statics.netMode == 2)
                                                                                                                                            {
                                                                                                                                                NetMessage.SendData(35, world, -1, this.whoAmI, "", num70, (float)num71, 0f, 0f);
                                                                                                                                                return;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (b == 36)
                                                                                                                                            {
                                                                                                                                                int num72 = (int)this.readBuffer[num];
                                                                                                                                                num++;
                                                                                                                                                int num73 = (int)this.readBuffer[num];
                                                                                                                                                num++;
                                                                                                                                                int num74 = (int)this.readBuffer[num];
                                                                                                                                                num++;
                                                                                                                                                int num75 = (int)this.readBuffer[num];
                                                                                                                                                num++;
                                                                                                                                                int num76 = (int)this.readBuffer[num];
                                                                                                                                                num++;
                                                                                                                                                if (num73 == 0)
                                                                                                                                                {
                                                                                                                                                    world.getPlayerList()[num72].zoneEvil = false;
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    world.getPlayerList()[num72].zoneEvil = true;
                                                                                                                                                }
                                                                                                                                                if (num74 == 0)
                                                                                                                                                {
                                                                                                                                                    world.getPlayerList()[num72].zoneMeteor = false;
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    world.getPlayerList()[num72].zoneMeteor = true;
                                                                                                                                                }
                                                                                                                                                if (num75 == 0)
                                                                                                                                                {
                                                                                                                                                    world.getPlayerList()[num72].zoneDungeon = false;
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    world.getPlayerList()[num72].zoneDungeon = true;
                                                                                                                                                }
                                                                                                                                                if (num76 == 0)
                                                                                                                                                {
                                                                                                                                                    world.getPlayerList()[num72].zoneJungle = false;
                                                                                                                                                    return;
                                                                                                                                                }
                                                                                                                                                world.getPlayerList()[num72].zoneJungle = true;
                                                                                                                                                return;
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (b == 37)
                                                                                                                                                {
                                                                                                                                                    if (Statics.netMode == 1)
                                                                                                                                                    {
                                                                                                                                                        world.getServer().getNetPlay().password = "";
                                                                                                                                                        //Main.menuMode = 31;
                                                                                                                                                        return;
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    if (b == 38)
                                                                                                                                                    {
                                                                                                                                                        if (Statics.netMode == 2)
                                                                                                                                                        {
                                                                                                                                                            string string9 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
                                                                                                                                                            if (string9 == world.getServer().getNetPlay().password)
                                                                                                                                                            {
                                                                                                                                                                world.getServer().getNetPlay().serverSock[this.whoAmI].state = 1;
                                                                                                                                                                NetMessage.SendData(3, world, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
                                                                                                                                                                return;
                                                                                                                                                            }
                                                                                                                                                            NetMessage.SendData(2, world, this.whoAmI, -1, "Incorrect password.", 0, 0f, 0f, 0f);
                                                                                                                                                            return;
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        if (b == 39 && Statics.netMode == 1)
                                                                                                                                                        {
                                                                                                                                                            short num77 = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                                                            world.getItemList()[(int)num77].owner = 8;
                                                                                                                                                            NetMessage.SendData(22, world, -1, -1, "", (int)num77, 0f, 0f, 0f);
                                                                                                                                                            return;
                                                                                                                                                        }
                                                                                                                                                        if (b == 40)
                                                                                                                                                        {
                                                                                                                                                            byte b22 = this.readBuffer[num];
                                                                                                                                                            num++;
                                                                                                                                                            int talkNPC = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                                                            num += 2;
                                                                                                                                                            world.getPlayerList()[(int)b22].talkNPC = talkNPC;
                                                                                                                                                            if (Statics.netMode == 2)
                                                                                                                                                            {
                                                                                                                                                                NetMessage.SendData(40, world, -1, this.whoAmI, "", (int)b22, 0f, 0f, 0f);
                                                                                                                                                                return;
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
                                                                                                                                                            if (b == 41)
                                                                                                                                                            {
                                                                                                                                                                byte b23 = this.readBuffer[num];
                                                                                                                                                                num++;
                                                                                                                                                                float itemRotation = BitConverter.ToSingle(this.readBuffer, num);
                                                                                                                                                                num += 4;
                                                                                                                                                                int itemAnimation = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                                                                world.getPlayerList()[(int)b23].itemRotation = itemRotation;
                                                                                                                                                                world.getPlayerList()[(int)b23].itemAnimation = itemAnimation;
                                                                                                                                                                if (Statics.netMode == 2)
                                                                                                                                                                {
                                                                                                                                                                    NetMessage.SendData(41, world, -1, this.whoAmI, "", (int)b23, 0f, 0f, 0f);
                                                                                                                                                                    return;
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                if (b == 42)
                                                                                                                                                                {
                                                                                                                                                                    int num78 = (int)this.readBuffer[num];
                                                                                                                                                                    num++;
                                                                                                                                                                    int statMana = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                                                                    num += 2;
                                                                                                                                                                    int statManaMax = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                                                                    if (Statics.netMode == 2)
                                                                                                                                                                    {
                                                                                                                                                                        num78 = this.whoAmI;
                                                                                                                                                                    }
                                                                                                                                                                    world.getPlayerList()[num78].statMana = statMana;
                                                                                                                                                                    world.getPlayerList()[num78].statManaMax = statManaMax;
                                                                                                                                                                    if (Statics.netMode == 2)
                                                                                                                                                                    {
                                                                                                                                                                        NetMessage.SendData(42, world, -1, this.whoAmI, "", num78, 0f, 0f, 0f);
                                                                                                                                                                        return;
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                {
                                                                                                                                                                    if (b == 43)
                                                                                                                                                                    {
                                                                                                                                                                        int num79 = (int)this.readBuffer[num];
                                                                                                                                                                        num++;
                                                                                                                                                                        int num80 = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                                                                        num += 2;
                                                                                                                                                                        if (num79 != Statics.myPlayer)
                                                                                                                                                                        {
                                                                                                                                                                            world.getPlayerList()[num79].ManaEffect(num80);
                                                                                                                                                                        }
                                                                                                                                                                        if (Statics.netMode == 2)
                                                                                                                                                                        {
                                                                                                                                                                            NetMessage.SendData(43, world, -1, this.whoAmI, "", num79, (float)num80, 0f, 0f);
                                                                                                                                                                            return;
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                    {
                                                                                                                                                                        if (b == 44)
                                                                                                                                                                        {
                                                                                                                                                                            byte b24 = this.readBuffer[num];
                                                                                                                                                                            num++;
                                                                                                                                                                            int num81 = (int)(this.readBuffer[num] - 1);
                                                                                                                                                                            num++;
                                                                                                                                                                            short num82 = BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                                                                            num += 2;
                                                                                                                                                                            byte b25 = this.readBuffer[num];
                                                                                                                                                                            bool pvp2 = false;
                                                                                                                                                                            if (b25 != 0)
                                                                                                                                                                            {
                                                                                                                                                                                pvp2 = true;
                                                                                                                                                                            }
                                                                                                                                                                            world.getPlayerList()[(int)b24].KillMe((double)num82, num81, pvp2);
                                                                                                                                                                            if (Statics.netMode == 2)
                                                                                                                                                                            {
                                                                                                                                                                                NetMessage.SendData(44, world, -1, this.whoAmI, "", (int)b24, (float)num81, (float)num82, (float)b25);
                                                                                                                                                                                return;
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                        else
                                                                                                                                                                        {
                                                                                                                                                                            if (b == 45)
                                                                                                                                                                            {
                                                                                                                                                                                int num83 = (int)this.readBuffer[num];
                                                                                                                                                                                num++;
                                                                                                                                                                                int num84 = (int)this.readBuffer[num];
                                                                                                                                                                                num++;
                                                                                                                                                                                int team = world.getPlayerList()[num83].team;
                                                                                                                                                                                world.getPlayerList()[num83].team = num84;
                                                                                                                                                                                if (Statics.netMode == 2)
                                                                                                                                                                                {
                                                                                                                                                                                    NetMessage.SendData(45, world, -1, this.whoAmI, "", num83, 0f, 0f, 0f);
                                                                                                                                                                                    string str2 = "";
                                                                                                                                                                                    if (num84 == 0)
                                                                                                                                                                                    {
                                                                                                                                                                                        str2 = " is no longer on a party.";
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        if (num84 == 1)
                                                                                                                                                                                        {
                                                                                                                                                                                            str2 = " has joined the red party.";
                                                                                                                                                                                        }
                                                                                                                                                                                        else
                                                                                                                                                                                        {
                                                                                                                                                                                            if (num84 == 2)
                                                                                                                                                                                            {
                                                                                                                                                                                                str2 = " has joined the green party.";
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                if (num84 == 3)
                                                                                                                                                                                                {
                                                                                                                                                                                                    str2 = " has joined the blue party.";
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (num84 == 4)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        str2 = " has joined the yellow party.";
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                    for (int num85 = 0; num85 < 8; num85++)
                                                                                                                                                                                    {
                                                                                                                                                                                        if (num85 == this.whoAmI || (team > 0 && world.getPlayerList()[num85].team == team) || (num84 > 0 && world.getPlayerList()[num85].team == num84))
                                                                                                                                                                                        {
                                                                                                                                                                                            NetMessage.SendData(25, world, num85, -1, world.getPlayerList()[num83].name + str2, 255,
                                                                                                                                                                                                (float)Statics.teamColor[num84].R, (float)Statics.teamColor[num84].G, (float)Statics.teamColor[num84].B);
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                    return;
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                if (b == 46)
                                                                                                                                                                                {
                                                                                                                                                                                    if (Statics.netMode == 2)
                                                                                                                                                                                    {
                                                                                                                                                                                        int i2 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                                                                                                                        num += 4;
                                                                                                                                                                                        int j2 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                                                                                                                        num += 4;
                                                                                                                                                                                        int num86 = Sign.ReadSign(i2, j2, world);
                                                                                                                                                                                        if (num86 >= 0)
                                                                                                                                                                                        {
                                                                                                                                                                                            NetMessage.SendData(47, world, this.whoAmI, -1, "", num86, 0f, 0f, 0f);
                                                                                                                                                                                            return;
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    if (b == 47)
                                                                                                                                                                                    {
                                                                                                                                                                                        int num87 = (int)BitConverter.ToInt16(this.readBuffer, num);
                                                                                                                                                                                        num += 2;
                                                                                                                                                                                        int x9 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                                                                                                                        num += 4;
                                                                                                                                                                                        int y8 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                                                                                                                        num += 4;
                                                                                                                                                                                        string string10 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
                                                                                                                                                                                        world.getSigns()[num87] = new Sign();
                                                                                                                                                                                        world.getSigns()[num87].x = x9;
                                                                                                                                                                                        world.getSigns()[num87].y = y8;
                                                                                                                                                                                        Sign.TextSign(num87, string10, world);
                                                                                                                                                                                        if (Statics.netMode == 1 && world.getSigns()[num87] != null && num87 != world.getPlayerList()[Statics.myPlayer].sign)
                                                                                                                                                                                        {
                                                                                                                                                                                            Statics.playerInventory = false;
                                                                                                                                                                                            world.getPlayerList()[Statics.myPlayer].talkNPC = -1;
                                                                                                                                                                                            //Main.editSign = false;
                                                                                                                                                                                            //Main.PlaySound(10, -1, -1, 1);
                                                                                                                                                                                            world.getPlayerList()[Statics.myPlayer].sign = num87;
                                                                                                                                                                                           // Main.npcChatText = world.getSigns()[num87].text;
                                                                                                                                                                                            return;
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        if (b == 48)
                                                                                                                                                                                        {
                                                                                                                                                                                            int num88 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                                                                                                                            num += 4;
                                                                                                                                                                                            int num89 = BitConverter.ToInt32(this.readBuffer, num);
                                                                                                                                                                                            num += 4;
                                                                                                                                                                                            byte liquid = this.readBuffer[num];
                                                                                                                                                                                            num++;
                                                                                                                                                                                            byte b26 = this.readBuffer[num];
                                                                                                                                                                                            num++;
                                                                                                                                                                                            if (world.getTile()[num88, num89] == null)
                                                                                                                                                                                            {
                                                                                                                                                                                                world.getTile()[num88, num89] = new Tile();
                                                                                                                                                                                            }
                                                                                                                                                                                            lock (world.getTile()[num88, num89])
                                                                                                                                                                                            {
                                                                                                                                                                                                world.getTile()[num88, num89].liquid = liquid;
                                                                                                                                                                                                if (b26 == 1)
                                                                                                                                                                                                {
                                                                                                                                                                                                    world.getTile()[num88, num89].lava = true;
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    world.getTile()[num88, num89].lava = false;
                                                                                                                                                                                                }
                                                                                                                                                                                                if (Statics.netMode == 2)
                                                                                                                                                                                                {
                                                                                                                                                                                                    WorldGen.SquareTileFrame(num88, num89, world, true);
                                                                                                                                                                                                }
                                                                                                                                                                                                return;
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                        if (b == 49 && world.getServer().getNetPlay().clientSock.state == 6)
                                                                                                                                                                                        {
                                                                                                                                                                                            world.getServer().getNetPlay().clientSock.state = 10;
                                                                                                                                                                                            world.getPlayerList()[Statics.myPlayer].Spawn();
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
