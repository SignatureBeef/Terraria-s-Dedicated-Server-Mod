
using Terraria_Server.Events;
using System.Text;
using Terraria_Server.Plugin;
using Terraria_Server.Commands;
using System;
using Terraria_Server.Shops;
using Terraria_Server.Misc;

namespace Terraria_Server
{
    public class MessageBuffer
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

        public void GetData(int start, int length)
        {
            if (this.whoAmI < 256)
            {
                Netplay.serverSock[this.whoAmI].timeOut = 0;
            }
            else
            {
                Netplay.clientSock.timeOut = 0;
            }

            int num = start + 1;
            byte bufferData = this.readBuffer[start];

            if (Main.netMode == 1 && Netplay.clientSock.statusMax > 0)
            {
                Netplay.clientSock.statusCount++;
            }

            if (Main.verboseNetplay)
            {
                for (int j = start; j < start + length; j++)
                {
                    byte arg_85_0 = this.readBuffer[j];
                }
            }

            if (Main.netMode == 2)
            {
                if (bufferData != 38)
                {
                    if (Netplay.serverSock[this.whoAmI].state == -1)
                    {
                        NetMessage.SendData(2, this.whoAmI, -1, "Incorrect password.", 0, 0f, 0f, 0f);
                        return;
                    }

                    if (Netplay.serverSock[this.whoAmI].state < 10 && bufferData > 12 && bufferData != 16 && bufferData != 42 && bufferData != 50)
                    {
                        NetMessage.BootPlayer(this.whoAmI, "Invalid operation at this state.");
                    }
                }
            }

            if (Main.netMode == 1)
            {
                switch (bufferData)
                {
                    case 2:
                        disconnect(start, length);
                        break;
                    case 3:
                        method3(start);
                        break;
                    case 7:
                        method7(start, length, num);
                        break;
                    case 9:
                        method9(start, length);
                        break;
                    case 10:
                        method10(start, num, bufferData);
                        break;
                    case 11:
                        method11(num);
                        break;
                    case 14:
                        method14(num);
                        break;
                    case 23:
                        method23(start, length, num);
                        break;
                    case 37:
                        method37();
                        break;
                    case 39:
                        method39(num);
                        break;
                }
            }
            else if (Main.netMode == 2)
            {
                switch (bufferData)
                {
                    case 1:
                        login(start, length);
                        break;
                    case 6:
                        method6();
                        break;
                    case 8:
                        method8(num);
                        break;
                    case 15:
                        method15();
                        break;
                    case 31:
                        method31(num);
                        break;
                    case 34:
                        method34(num);
                        break;
                    case 38:
                        method38(start, length, num);
                        break;
                    case 46:
                        method46(num);
                        break;
                }
            }

            switch (bufferData)
            {
                case 4:
                    method4(start, length, num);
                    break;
                case 5:
                    method5(start, length);
                    break;
                case 12:
                    method12(num);
                    break;
                case 13:
                    method13(num);
                    break;
                case 16:
                    method16(num);
                    break;
                case 17:
                    method17(num);
                    break;
                case 19:
                    method19(num);
                    break;
                case 20:
                    method20(start, num, bufferData);
                    break;
                case 21:
                    method21(start, length, num);
                    break;
                case 22:
                    method22(num);
                    break;
                case 24:
                    method24(num);
                    break;
                case 25:
                    method25(start, length);
                    break;
                case (int)Packet.STRIKE_PLAYER:
                    methodStrike(start, length, num);
                    break;
                case (int)Packet.PROJECTILE:
                    methodProjectile(num);
                    break;
                case 28:
                    method28(num);
                    break;
                case 29:
                    method29(num);
                    break;
                case 30:
                    method30(num);
                    break;
                case 32:
                    method32(start, length, num);
                    break;
                case 33:
                    method33(num);
                    break;
                case 35:
                    method35(num);
                    break;
                case 36:
                    method36(num);
                    break;
                case 40:
                    method40(num);
                    break;
                case 41:
                    method41(num);
                    break;
                case 42:
                    method42(num);
                    break;
                case 43:
                    method43(num);
                    break;
                case 44:
                    method44(start, length, num);
                    break;
                case 45:
                    method45(num);
                    break;
                case 47:
                    method47(start, length, num);
                    break;
                case 48:
                    method48(num);
                    break;
                case 49:
                    method49();
                    break;
                case 50:
                    method50(num);
                    break;
                case 51:
                    method51(num);
                    break;
            }
        }

        private void login(int start, int length)
        {
            PlayerLoginEvent Event = new PlayerLoginEvent();
            Event.Socket = Netplay.serverSock[this.whoAmI];
            Event.Sender = Main.player[this.whoAmI];
            Program.server.getPluginManager().processHook(Plugin.Hooks.PLAYER_PRELOGIN, Event);
            if (Event.Cancelled)
            {
                NetMessage.SendData(2, this.whoAmI, -1, "Disconnected By Server.", 0, 0f, 0f, 0f);
                return;
            }

            if (Program.server.BanList.containsException(Netplay.serverSock[this.whoAmI].tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0]))
            {
                NetMessage.SendData(2, this.whoAmI, -1, "You are banned from this Server.", 0, 0f, 0f, 0f);
                return;
            }

            if (Program.properties.UseWhiteList && !Program.server.WhiteList.containsException(Netplay.serverSock[this.whoAmI].tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0]))
            {
                NetMessage.SendData(2, this.whoAmI, -1, "You are not on the WhiteList.", 0, 0f, 0f, 0f);
                return;
            }

            if (Netplay.serverSock[this.whoAmI].state == 0)
            {
                string version = Encoding.ASCII.GetString(this.readBuffer, start + 1, length - 1);
                if (!(version == "Terraria" + Statics.CURRENT_RELEASE))
                {
                    NetMessage.SendData(2, this.whoAmI, -1, "You are not using the same version as this Server.", 0, 0f, 0f, 0f);
                    return;
                }

                if (Netplay.password == null || Netplay.password == "")
                {
                    Netplay.serverSock[this.whoAmI].state = 1;
                    NetMessage.SendData(3, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
                    return;
                }

                Netplay.serverSock[this.whoAmI].state = -1;
                NetMessage.SendData(37, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
            }
        }

        private void disconnect(int start, int length)
        {
            Netplay.disconnect = true;
            Main.statusText = Encoding.ASCII.GetString(this.readBuffer, start + 1, length - 1);
        }

        private void method3(int start)
        {
            if (Netplay.clientSock.state == 1)
            {
                Netplay.clientSock.state = 2;
            }

            int num2 = (int)this.readBuffer[start + 1];
            if (num2 != Main.myPlayer)
            {
                Main.player[num2] = (Player)Main.player[Main.myPlayer].Clone();
                Main.player[Main.myPlayer] = new Player();
                Main.player[num2].whoAmi = num2;
                Main.myPlayer = num2;
            }
            NetMessage.SendData(4, -1, -1, Main.player[Main.myPlayer].name, Main.myPlayer, 0f, 0f, 0f, 0);
            NetMessage.SendData(16, -1, -1, "", Main.myPlayer, 0f, 0f, 0f, 0);
            NetMessage.SendData(42, -1, -1, "", Main.myPlayer, 0f, 0f, 0f, 0);
            NetMessage.SendData(50, -1, -1, "", Main.myPlayer, 0f, 0f, 0f, 0);
            for (int k = 0; k < 44; k++)
            {
                NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].inventory[k].Name, Main.myPlayer, (float)k, 0f, 0f, 0);
            }
            NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[0].Name, Main.myPlayer, 44f, 0f, 0f, 0);
            NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[1].Name, Main.myPlayer, 45f, 0f, 0f, 0);
            NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[2].Name, Main.myPlayer, 46f, 0f, 0f, 0);
            NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[3].Name, Main.myPlayer, 47f, 0f, 0f, 0);
            NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[4].Name, Main.myPlayer, 48f, 0f, 0f, 0);
            NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[5].Name, Main.myPlayer, 49f, 0f, 0f, 0);
            NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[6].Name, Main.myPlayer, 50f, 0f, 0f, 0);
            NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[7].Name, Main.myPlayer, 51f, 0f, 0f, 0);
            NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[8].Name, Main.myPlayer, 52f, 0f, 0f, 0);
            NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[9].Name, Main.myPlayer, 53f, 0f, 0f, 0);
            NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[10].Name, Main.myPlayer, 54f, 0f, 0f, 0);
            NetMessage.SendData(6, -1, -1, "", 0, 0f, 0f, 0f, 0);
            if (Netplay.clientSock.state == 2)
            {
                Netplay.clientSock.state = 3;
                return;
            }
        }

        private void method4(int start, int length, int num)
        {
            bool flag = false;
            int num3 = (int)this.readBuffer[start + 1];
            if (Main.netMode == 2)
            {
                num3 = this.whoAmI;
            }
            if (num3 == Main.myPlayer)
            {
                return;
            }
            int num4 = (int)this.readBuffer[start + 2];
            if (num4 >= 17)
            {
                num4 = 0;
            }
            Main.player[num3].hair = num4;
            Main.player[num3].whoAmi = num3;
            num += 2;
            Main.player[num3].hairColor.R = this.readBuffer[num];
            num++;
            Main.player[num3].hairColor.G = this.readBuffer[num];
            num++;
            Main.player[num3].hairColor.B = this.readBuffer[num];
            num++;
            Main.player[num3].skinColor.R = this.readBuffer[num];
            num++;
            Main.player[num3].skinColor.G = this.readBuffer[num];
            num++;
            Main.player[num3].skinColor.B = this.readBuffer[num];
            num++;
            Main.player[num3].eyeColor.R = this.readBuffer[num];
            num++;
            Main.player[num3].eyeColor.G = this.readBuffer[num];
            num++;
            Main.player[num3].eyeColor.B = this.readBuffer[num];
            num++;
            Main.player[num3].shirtColor.R = this.readBuffer[num];
            num++;
            Main.player[num3].shirtColor.G = this.readBuffer[num];
            num++;
            Main.player[num3].shirtColor.B = this.readBuffer[num];
            num++;
            Main.player[num3].underShirtColor.R = this.readBuffer[num];
            num++;
            Main.player[num3].underShirtColor.G = this.readBuffer[num];
            num++;
            Main.player[num3].underShirtColor.B = this.readBuffer[num];
            num++;
            Main.player[num3].pantsColor.R = this.readBuffer[num];
            num++;
            Main.player[num3].pantsColor.G = this.readBuffer[num];
            num++;
            Main.player[num3].pantsColor.B = this.readBuffer[num];
            num++;
            Main.player[num3].shoeColor.R = this.readBuffer[num];
            num++;
            Main.player[num3].shoeColor.G = this.readBuffer[num];
            num++;
            Main.player[num3].shoeColor.B = this.readBuffer[num];
            num++;
            if (this.readBuffer[num] == 0)
            {
                Main.player[num3].hardCore = false;
            }
            else
            {
                Main.player[num3].hardCore = true;
            }
            num++;
            string text = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
            text = text.Trim();
            Main.player[num3].name = text.Trim();
            if (Main.netMode == 2)
            {
                if (Netplay.serverSock[this.whoAmI].state < 10)
                {
                    for (int l = 0; l < 255; l++)
                    {
                        if (l != num3 && text == Main.player[l].name && Netplay.serverSock[l].active)
                        {
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    NetMessage.SendData(2, this.whoAmI, -1, text + " is already on this server.", 0, 0f, 0f, 0f, 0);
                    return;
                }
                if (text.Length > 20)
                {
                    NetMessage.SendData(2, this.whoAmI, -1, "Name is too long.", 0, 0f, 0f, 0f, 0);
                    return;
                }
                if (text == "")
                {
                    NetMessage.SendData(2, this.whoAmI, -1, "Empty name.", 0, 0f, 0f, 0f, 0);
                    return;
                }
                Netplay.serverSock[this.whoAmI].oldName = text;
                Netplay.serverSock[this.whoAmI].name = text;
                NetMessage.SendData(4, -1, this.whoAmI, text, num3, 0f, 0f, 0f, 0);
                return;
            }
        }

        private void method5(int start, int length)
        {
            int num2 = (int)this.readBuffer[start + 1];
            if (Main.netMode == 2)
            {
                num2 = this.whoAmI;
            }
            if (num2 != Main.myPlayer)
            {
                lock (Main.player[num2])
                {
                    int num3 = (int)this.readBuffer[start + 2];
                    int stack = (int)this.readBuffer[start + 3];
                    string string3 = Encoding.ASCII.GetString(this.readBuffer, start + 4, length - 4);
                    if (num3 < 44)
                    {
                        Main.player[num2].inventory[num3] = new Item();
                        Main.player[num2].inventory[num3].SetDefaults(string3);
                        Main.player[num2].inventory[num3].Stack = stack;
                    }
                    else
                    {
                        Main.player[num2].armor[num3 - 44] = new Item();
                        Main.player[num2].armor[num3 - 44].SetDefaults(string3);
                        Main.player[num2].armor[num3 - 44].Stack = stack;
                    }
                    if (Main.netMode == 2 && num2 == this.whoAmI)
                    {
                        NetMessage.SendData(5, -1, this.whoAmI, string3, num2, (float)num3, 0f, 0f, 0);
                    }
                }
            }
        }

        private void method6()
        {

            if (Netplay.serverSock[this.whoAmI].state == 1)
            {
                Netplay.serverSock[this.whoAmI].state = 2;
            }
            NetMessage.SendData(7, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
            return;
        }

        private void method7(int start, int length, int num)
        {
            Main.time = (double)BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            Main.dayTime = false;
            if (this.readBuffer[num] == 1)
            {
                Main.dayTime = true;
            }
            num++;
            Main.moonPhase = (int)this.readBuffer[num];
            num++;
            int num6 = (int)this.readBuffer[num];
            num++;
            if (num6 == 1)
            {
                Main.bloodMoon = true;
            }
            else
            {
                Main.bloodMoon = false;
            }
            Main.maxTilesX = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            Main.maxTilesY = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            Main.spawnTileX = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            Main.spawnTileY = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            Main.worldSurface = (double)BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            Main.rockLayer = (double)BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            Main.worldID = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            byte b2 = this.readBuffer[num];
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
            num++;
            Main.worldName = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
            if (Netplay.clientSock.state == 3)
            {
                Netplay.clientSock.state = 4;
                return;
            }
        }

        private void method8(int num)
        {
            int num8 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            int num9 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            bool flag3 = true;
            if (num8 == -1 || num9 == -1)
            {
                flag3 = false;
            }
            else
            {
                if (num8 < 10 || num8 > Main.maxTilesX - 10)
                {
                    flag3 = false;
                }
                else
                {
                    if (num9 < 10 || num9 > Main.maxTilesY - 10)
                    {
                        flag3 = false;
                    }
                }
            }
            int num10 = 1350;
            if (flag3)
            {
                num10 *= 2;
            }
            if (Netplay.serverSock[this.whoAmI].state == 2)
            {
                Netplay.serverSock[this.whoAmI].state = 3;
            }
            NetMessage.SendData(9, this.whoAmI, -1, "Receiving tile data", num10, 0f, 0f, 0f, 0);
            Netplay.serverSock[this.whoAmI].statusText2 = "is receiving tile data";
            Netplay.serverSock[this.whoAmI].statusMax += num10;
            int sectionX = Netplay.GetSectionX(Main.spawnTileX);
            int sectionY = Netplay.GetSectionY(Main.spawnTileY);
            for (int m = sectionX - 2; m < sectionX + 3; m++)
            {
                for (int n = sectionY - 1; n < sectionY + 2; n++)
                {
                    NetMessage.SendSection(this.whoAmI, m, n);
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
                        NetMessage.SendSection(this.whoAmI, num11, num12);
                    }
                }
                NetMessage.SendData(11, this.whoAmI, -1, "", num8 - 2, (float)(num9 - 1), (float)(num8 + 2), (float)(num9 + 1), 0);
            }
            NetMessage.SendData(11, this.whoAmI, -1, "", sectionX - 2, (float)(sectionY - 1), (float)(sectionX + 2), (float)(sectionY + 1), 0);
            for (int num13 = 0; num13 < 200; num13++)
            {
                if (Main.item[num13].Active)
                {
                    NetMessage.SendData(21, this.whoAmI, -1, "", num13, 0f, 0f, 0f, 0);
                    NetMessage.SendData(22, this.whoAmI, -1, "", num13, 0f, 0f, 0f, 0);
                }
            }
            for (int num14 = 0; num14 < 1000; num14++)
            {
                if (Main.npc[num14].active)
                {
                    NetMessage.SendData(23, this.whoAmI, -1, "", num14, 0f, 0f, 0f, 0);
                }
            }
            NetMessage.SendData(49, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
            return;
        }

        private void method9(int start, int length)
        {
            int num14 = BitConverter.ToInt32(this.readBuffer, start + 1);
            string string4 = Encoding.ASCII.GetString(this.readBuffer, start + 5, length - 5);
            Netplay.clientSock.statusMax += num14;
            Netplay.clientSock.statusText = string4;
        }

        private void method10(int start, int num, byte bufferData)
        {
            short num16 = BitConverter.ToInt16(this.readBuffer, start + 1);
            int num17 = BitConverter.ToInt32(this.readBuffer, start + 3);
            int num18 = BitConverter.ToInt32(this.readBuffer, start + 7);
            num = start + 11;
            for (int num19 = num17; num19 < num17 + (int)num16; num19++)
            {
                if (Main.tile[num19, num18] == null)
                {
                    Main.tile[num19, num18] = new Tile();
                }
                byte b3 = this.readBuffer[num];
                num++;
                bool active = Main.tile[num19, num18].active;
                if ((b3 & 1) == 1)
                {
                    Main.tile[num19, num18].active = true;
                }
                else
                {
                    Main.tile[num19, num18].active = false;
                }
                if ((b3 & 2) == 2)
                {
                    Main.tile[num19, num18].lighted = true;
                }
                if ((b3 & 4) == 4)
                {
                    Main.tile[num19, num18].wall = 1;
                }
                else
                {
                    Main.tile[num19, num18].wall = 0;
                }
                if ((b3 & 8) == 8)
                {
                    Main.tile[num19, num18].liquid = 1;
                }
                else
                {
                    Main.tile[num19, num18].liquid = 0;
                }
                if (Main.tile[num19, num18].active)
                {
                    int type = (int)Main.tile[num19, num18].type;
                    Main.tile[num19, num18].type = this.readBuffer[num];
                    num++;
                    if (Main.tileFrameImportant[(int)Main.tile[num19, num18].type])
                    {
                        Main.tile[num19, num18].frameX = BitConverter.ToInt16(this.readBuffer, num);
                        num += 2;
                        Main.tile[num19, num18].frameY = BitConverter.ToInt16(this.readBuffer, num);
                        num += 2;
                    }
                    else
                    {
                        if (!active || (int)Main.tile[num19, num18].type != type)
                        {
                            Main.tile[num19, num18].frameX = -1;
                            Main.tile[num19, num18].frameY = -1;
                        }
                    }
                }
                if (Main.tile[num19, num18].wall > 0)
                {
                    Main.tile[num19, num18].wall = this.readBuffer[num];
                    num++;
                }
                if (Main.tile[num19, num18].liquid > 0)
                {
                    Main.tile[num19, num18].liquid = this.readBuffer[num];
                    num++;
                    byte b4 = this.readBuffer[num];
                    num++;
                    if (b4 == 1)
                    {
                        Main.tile[num19, num18].lava = true;
                    }
                    else
                    {
                        Main.tile[num19, num18].lava = false;
                    }
                }
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData((int)bufferData, -1, this.whoAmI, "", (int)num16, (float)num17, (float)num18, 0f, 0);
                return;
            }
        }

        private void method11(int num)
        {
            int startX = (int)BitConverter.ToInt16(this.readBuffer, num);
            num += 4;
            int startY = (int)BitConverter.ToInt16(this.readBuffer, num);
            num += 4;
            int endX = (int)BitConverter.ToInt16(this.readBuffer, num);
            num += 4;
            int endY = (int)BitConverter.ToInt16(this.readBuffer, num);
            num += 4;
            WorldGen.SectionTileFrame(startX, startY, endX, endY);
            return;
        }

        private void method12(int num)
        {
            int num20 = (int)this.readBuffer[num];
            if (Main.netMode == 2)
            {
                num20 = this.whoAmI;
            }
            num++;
            Main.player[num20].SpawnX = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            Main.player[num20].SpawnY = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            Main.player[num20].Spawn();
            if (Main.netMode == 2 && Netplay.serverSock[this.whoAmI].state >= 3)
            {
                if (Netplay.serverSock[this.whoAmI].state == 3)
                {
                    Netplay.serverSock[this.whoAmI].state = 10;
                    NetMessage.greetPlayer(this.whoAmI);
                    NetMessage.syncPlayers();
                    NetMessage.buffer[this.whoAmI].broadcast = true;
                    NetMessage.SendData(12, -1, this.whoAmI, "", this.whoAmI, 0f, 0f, 0f, 0);
                    return;
                }
                NetMessage.SendData(12, -1, this.whoAmI, "", this.whoAmI, 0f, 0f, 0f, 0);
                return;
            }
        }

        private void method13(int num)
        {
            int num21 = (int)this.readBuffer[num];
            if (num21 == Main.myPlayer)
            {
                return;
            }
            if (Main.netMode == 1 && !Main.player[num21].active)
            {
                NetMessage.SendData(15, -1, -1, "", 0, 0f, 0f, 0f, 0);
            }
            if (Main.netMode == 2)
            {
                num21 = this.whoAmI;
            }
            num++;
            int num22 = (int)this.readBuffer[num];
            num++;
            int selectedItem = (int)this.readBuffer[num];
            num++;
            float x = BitConverter.ToSingle(this.readBuffer, num);
            num += 4;
            float num23 = BitConverter.ToSingle(this.readBuffer, num);
            num += 4;
            float x2 = BitConverter.ToSingle(this.readBuffer, num);
            num += 4;
            float y = BitConverter.ToSingle(this.readBuffer, num);
            num += 4;
            Main.player[num21].selectedItem = selectedItem;
            Main.player[num21].position.X = x;
            Main.player[num21].position.Y = num23;
            Main.player[num21].velocity.X = x2;
            Main.player[num21].velocity.Y = y;
            Main.player[num21].oldVelocity = Main.player[num21].velocity;
            Main.player[num21].fallStart = (int)(num23 / 16f);
            Main.player[num21].controlUp = false;
            Main.player[num21].controlDown = false;
            Main.player[num21].controlLeft = false;
            Main.player[num21].controlRight = false;
            Main.player[num21].controlJump = false;
            Main.player[num21].controlUseItem = false;
            Main.player[num21].direction = -1;
            if ((num22 & 1) == 1)
            {
                Main.player[num21].controlUp = true;
            }
            if ((num22 & 2) == 2)
            {
                Main.player[num21].controlDown = true;
            }
            if ((num22 & 4) == 4)
            {
                Main.player[num21].controlLeft = true;
            }
            if ((num22 & 8) == 8)
            {
                Main.player[num21].controlRight = true;
            }
            if ((num22 & 16) == 16)
            {
                Main.player[num21].controlJump = true;
            }
            if ((num22 & 32) == 32)
            {
                Main.player[num21].controlUseItem = true;
            }
            if ((num22 & 64) == 64)
            {
                Main.player[num21].direction = 1;
            }
            if (Main.netMode == 2 && Netplay.serverSock[this.whoAmI].state == 10)
            {
                NetMessage.SendData(13, -1, this.whoAmI, "", num21, 0f, 0f, 0f, 0);
                return;
            }
        }

        private void method14(int num)
        {

            int num24 = (int)this.readBuffer[num];
            num++;
            int num25 = (int)this.readBuffer[num];
            if (num25 == 1)
            {
                if (!Main.player[num24].active)
                {
                    Main.player[num24] = new Player();
                }
                Main.player[num24].active = true;
                return;
            }
            Main.player[num24].active = false;
            return;
        }

        private void method15()
        {
            NetMessage.syncPlayers();
        }

        private void method16(int num)
        {
            int num26 = (int)this.readBuffer[num];
            num++;
            if (num26 == Main.myPlayer)
            {
                return;
            }
            int statLife = (int)BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            int statLifeMax = (int)BitConverter.ToInt16(this.readBuffer, num);
            if (Main.netMode == 2)
            {
                num26 = this.whoAmI;
            }
            Main.player[num26].statLife = statLife;
            Main.player[num26].statLifeMax = statLifeMax;
            if (Main.player[num26].statLife <= 0)
            {
                Main.player[num26].dead = true;
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(16, -1, this.whoAmI, "", num26, 0f, 0f, 0f, 0);
                return;
            }
        }

        private void method17(int num)
        {
            byte b5 = this.readBuffer[num];
            num++;
            int num27 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            int num28 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            byte b6 = this.readBuffer[num];
            num++;
            int style = (int)this.readBuffer[num];
            bool flag4 = false;
            if (b6 == 1)
            {
                flag4 = true;
            }

            Tile tile = new Tile();

            if (Main.tile[num27, num28] != null)
            {
                tile = WorldGen.cloneTile(Main.tile[num27, num28]);
            }
            if (Main.tile[num27, num28] == null)
            {
                Main.tile[num27, num28] = new Tile();
            }

            tile.tileX = num27;
            tile.tileY = num28;

            TileBreakEvent Event = new TileBreakEvent();
            Event.Sender = Main.player[this.whoAmI];
            Event.Tile = tile;
            Event.Type = b6;
            Event.Position = new Vector2(num27, num28);
            Program.server.getPluginManager().processHook(Hooks.TILE_BREAK, Event);
            if (Event.Cancelled)
            {
                NetMessage.SendTileSquare(this.whoAmI, num27, num28, 1);
                return;
            }

            if (Main.netMode == 2)
            {
                if (!flag4)
                {
                    if (b5 == 0 || b5 == 2 || b5 == 4)
                    {
                        Netplay.serverSock[this.whoAmI].spamDelBlock += 1f;
                    }
                    else
                    {
                        if (b5 == 1 || b5 == 3)
                        {
                            Netplay.serverSock[this.whoAmI].spamAddBlock += 1f;
                        }
                    }
                }
                if (!Netplay.serverSock[this.whoAmI].tileSection[Netplay.GetSectionX(num27), Netplay.GetSectionY(num28)])
                {
                    flag4 = true;
                }
            }
            if (b5 == 0)
            {
                WorldGen.KillTile(num27, num28, flag4, false, false);
            }
            else
            {
                if (b5 == 1)
                {
                    WorldGen.PlaceTile(num27, num28, (int)b6, false, true, -1, style);
                }
                else
                {
                    if (b5 == 2)
                    {
                        WorldGen.KillWall(num27, num28, flag4);
                    }
                    else
                    {
                        if (b5 == 3)
                        {
                            WorldGen.PlaceWall(num27, num28, (int)b6, false);
                        }
                        else
                        {
                            if (b5 == 4)
                            {
                                WorldGen.KillTile(num27, num28, flag4, false, true);
                            }
                        }
                    }
                }
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(17, -1, this.whoAmI, "", (int)b5, (float)num27, (float)num28, (float)b6, 0);
                if (b5 == 1 && b6 == 53)
                {
                    NetMessage.SendTileSquare(-1, num27, num28, 1);
                    return;
                }
            }
        }

        private void method19(int num)
        {
            byte b8 = this.readBuffer[num];
            num++;
            int x = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            int num32 = (int)this.readBuffer[num];
            int direction = 0;
            if (num32 == 0)
            {
                direction = -1;
            }

            bool state = false;

            if (b8 == 0) //if open
            {
                state = true;
            }

            if (b8 == 0)
            {
                WorldGen.OpenDoor(x, y, direction, state, DoorOpener.PLAYER, Main.player[this.whoAmI]);
            }
            else
            {
                if (b8 == 1)
                {
                    WorldGen.CloseDoor(x, y, true, DoorOpener.PLAYER, Main.player[this.whoAmI]);
                }
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(19, -1, this.whoAmI, "", (int)b8, (float)x, (float)y, (float)num32, 0);
                return;
            }
        }

        private void method20(int start, int num, byte bufferData)
        {
            short num33 = BitConverter.ToInt16(this.readBuffer, start + 1);
            int num34 = BitConverter.ToInt32(this.readBuffer, start + 3);
            int num35 = BitConverter.ToInt32(this.readBuffer, start + 7);
            num = start + 11;
            for (int num36 = num34; num36 < num34 + (int)num33; num36++)
            {
                for (int num37 = num35; num37 < num35 + (int)num33; num37++)
                {
                    if (Main.tile[num36, num37] == null)
                    {
                        Main.tile[num36, num37] = new Tile();
                    }
                    byte b9 = this.readBuffer[num];
                    num++;
                    bool active2 = Main.tile[num36, num37].active;
                    if ((b9 & 1) == 1)
                    {
                        Main.tile[num36, num37].active = true;
                    }
                    else
                    {
                        Main.tile[num36, num37].active = false;
                    }
                    if ((b9 & 2) == 2)
                    {
                        Main.tile[num36, num37].lighted = true;
                    }
                    if ((b9 & 4) == 4)
                    {
                        Main.tile[num36, num37].wall = 1;
                    }
                    else
                    {
                        Main.tile[num36, num37].wall = 0;
                    }
                    if ((b9 & 8) == 8)
                    {
                        Main.tile[num36, num37].liquid = 1;
                    }
                    else
                    {
                        Main.tile[num36, num37].liquid = 0;
                    }
                    if (Main.tile[num36, num37].active)
                    {
                        int type2 = (int)Main.tile[num36, num37].type;
                        Main.tile[num36, num37].type = this.readBuffer[num];
                        num++;
                        if (Main.tileFrameImportant[(int)Main.tile[num36, num37].type])
                        {
                            Main.tile[num36, num37].frameX = BitConverter.ToInt16(this.readBuffer, num);
                            num += 2;
                            Main.tile[num36, num37].frameY = BitConverter.ToInt16(this.readBuffer, num);
                            num += 2;
                        }
                        else
                        {
                            if (!active2 || (int)Main.tile[num36, num37].type != type2)
                            {
                                Main.tile[num36, num37].frameX = -1;
                                Main.tile[num36, num37].frameY = -1;
                            }
                        }
                    }
                    if (Main.tile[num36, num37].wall > 0)
                    {
                        Main.tile[num36, num37].wall = this.readBuffer[num];
                        num++;
                    }
                    if (Main.tile[num36, num37].liquid > 0)
                    {
                        Main.tile[num36, num37].liquid = this.readBuffer[num];
                        num++;
                        byte b10 = this.readBuffer[num];
                        num++;
                        if (b10 == 1)
                        {
                            Main.tile[num36, num37].lava = true;
                        }
                        else
                        {
                            Main.tile[num36, num37].lava = false;
                        }
                    }
                }
            }
            WorldGen.RangeFrame(num34, num35, num34 + (int)num33, num35 + (int)num33);
            if (Main.netMode == 2)
            {
                NetMessage.SendData((int)bufferData, -1, this.whoAmI, "", (int)num33, (float)num34, (float)num35, 0f, 0);
                return;
            }
        }

        private void method21(int start, int length, int num)
        {
            short num38 = BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            float num39 = BitConverter.ToSingle(this.readBuffer, num);
            num += 4;
            float num40 = BitConverter.ToSingle(this.readBuffer, num);
            num += 4;
            float x3 = BitConverter.ToSingle(this.readBuffer, num);
            num += 4;
            float y2 = BitConverter.ToSingle(this.readBuffer, num);
            num += 4;
            byte stack2 = this.readBuffer[num];
            num++;
            string string4 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
            if (Main.netMode == 1)
            {
                if (string4 == "0")
                {
                    Main.item[(int)num38].Active = false;
                    return;
                }
                Main.item[(int)num38].SetDefaults(string4);
                Main.item[(int)num38].Stack = (int)stack2;
                Main.item[(int)num38].Position.X = num39;
                Main.item[(int)num38].Position.Y = num40;
                Main.item[(int)num38].Velocity.X = x3;
                Main.item[(int)num38].Velocity.Y = y2;
                Main.item[(int)num38].Active = true;
                Main.item[(int)num38].Wet = Collision.WetCollision(Main.item[(int)num38].Position, Main.item[(int)num38].Width, Main.item[(int)num38].Height);
                return;
            }
            else
            {
                if (string4 == "0")
                {
                    if (num38 < 200)
                    {
                        Main.item[(int)num38].Active = false;
                        NetMessage.SendData(21, -1, -1, "", (int)num38, 0f, 0f, 0f, 0);
                        return;
                    }
                }
                else
                {
                    bool flag5 = false;
                    if (num38 == 200)
                    {
                        flag5 = true;
                    }
                    if (flag5)
                    {
                        Item item = new Item();
                        item.SetDefaults(string4);
                        num38 = (short)Item.NewItem((int)num39, (int)num40, item.Width, item.Height, item.Type, (int)stack2, true);
                    }
                    Main.item[(int)num38].SetDefaults(string4);
                    Main.item[(int)num38].Stack = (int)stack2;
                    Main.item[(int)num38].Position.X = num39;
                    Main.item[(int)num38].Position.Y = num40;
                    Main.item[(int)num38].Velocity.X = x3;
                    Main.item[(int)num38].Velocity.Y = y2;
                    Main.item[(int)num38].Active = true;
                    Main.item[(int)num38].Owner = Main.myPlayer;
                    if (flag5)
                    {
                        NetMessage.SendData(21, -1, -1, "", (int)num38, 0f, 0f, 0f, 0);
                        Main.item[(int)num38].OwnIgnore = this.whoAmI;
                        Main.item[(int)num38].OwnTime = 100;
                        Main.item[(int)num38].FindOwner((int)num38);
                        return;
                    }
                    NetMessage.SendData(21, -1, this.whoAmI, "", (int)num38, 0f, 0f, 0f, 0);
                    return;
                }
            }
        }

        private void method22(int num)
        {
            short num41 = BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            byte b11 = this.readBuffer[num];
            if (Main.netMode == 2 && Main.item[(int)num41].Owner != this.whoAmI)
            {
                return;
            }
            Main.item[(int)num41].Owner = (int)b11;
            if ((int)b11 == Main.myPlayer)
            {
                Main.item[(int)num41].KeepTime = 15;
            }
            else
            {
                Main.item[(int)num41].KeepTime = 0;
            }
            if (Main.netMode == 2)
            {
                Main.item[(int)num41].Owner = 255;
                Main.item[(int)num41].KeepTime = 15;
                NetMessage.SendData(22, -1, -1, "", (int)num41, 0f, 0f, 0f, 0);
                return;
            }
        }

        private void method23(int start, int length, int num)
        {
            short num42 = BitConverter.ToInt16(this.readBuffer, num);
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
            byte arg_2465_0 = this.readBuffer[num];
            num++;
            int num43 = (int)BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            float[] array = new float[NPC.maxAI];
            for (int num44 = 0; num44 < NPC.maxAI; num44++)
            {
                array[num44] = BitConverter.ToSingle(this.readBuffer, num);
                num += 4;
            }
            string string5 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
            if (!Main.npc[(int)num42].active || Main.npc[(int)num42].name != string5)
            {
                Main.npc[(int)num42].active = true;
                Main.npc[(int)num42].SetDefaults(string5);
            }
            Main.npc[(int)num42].position.X = x4;
            Main.npc[(int)num42].position.Y = y3;
            Main.npc[(int)num42].velocity.X = x5;
            Main.npc[(int)num42].velocity.Y = y4;
            Main.npc[(int)num42].target = target;
            Main.npc[(int)num42].direction = direction2;
            Main.npc[(int)num42].life = num43;
            if (num43 <= 0)
            {
                Main.npc[(int)num42].active = false;
            }
            for (int num45 = 0; num45 < NPC.maxAI; num45++)
            {
                Main.npc[(int)num42].ai[num45] = array[num45];
            }
            return;
        }

        private void method24(int num)
        {
            short num46 = BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            byte b12 = this.readBuffer[num];
            if (Main.netMode == 2)
            {
                b12 = (byte)this.whoAmI;
            }
            Main.npc[(int)num46].StrikeNPC(Main.player[(int)b12].inventory[Main.player[(int)b12].selectedItem].Damage, Main.player[(int)b12].inventory[Main.player[(int)b12].selectedItem].KnockBack, Main.player[(int)b12].direction);
            if (Main.netMode == 2)
            {
                NetMessage.SendData(24, -1, this.whoAmI, "", (int)num46, (float)b12, 0f, 0f, 0);
                NetMessage.SendData(23, -1, -1, "", (int)num46, 0f, 0f, 0f, 0);
                return;
            }
        }

        private void method25(int start, int length)
        {
            int num46 = (int)this.readBuffer[start + 1];
            if (Main.netMode == 2)
            {
                num46 = this.whoAmI;
            }
            byte b12 = this.readBuffer[start + 2];
            byte b13 = this.readBuffer[start + 3];
            byte b14 = this.readBuffer[start + 4];

            b12 = 255;
            b13 = 255;
            b14 = 255;

            string string7 = Encoding.ASCII.GetString(this.readBuffer, start + 5, length - 5);

            if (Main.netMode == 2)
            {
                string Chat = string7.ToLower().Trim();

                if (Chat.Length > 0)
                {
                    if (Chat.Substring(0, 1).Equals("/"))
                    {
                        PlayerCommandEvent Event = new PlayerCommandEvent();
                        Event.Message = Chat;
                        Event.Sender = Main.player[this.whoAmI];
                        Program.server.getPluginManager().processHook(Plugin.Hooks.PLAYER_COMMAND, Event);
                        if (Event.Cancelled)
                        {
                            return;
                        }

                        Program.tConsole.WriteLine(Main.player[this.whoAmI].name + " Sent Command: " + string7);

                        Program.commandParser.parsePlayerCommand(Main.player[this.whoAmI], Chat);
                        return;
                    }
                    else
                    {

                        PlayerChatEvent Event = new PlayerChatEvent();
                        Event.Message = Chat;
                        Event.Sender = Main.player[this.whoAmI];
                        Program.server.getPluginManager().processHook(Plugin.Hooks.PLAYER_CHAT, Event);

                        if (Event.Cancelled)
                        {
                            return;
                        }
                    }

                    NetMessage.SendData(25, -1, -1, string7, num46, (float)b12, (float)b13, (float)b14);
                    if (Main.dedServ)
                    {
                        Program.tConsole.WriteLine("<" + Main.player[this.whoAmI].name + "> " + string7);
                        return;
                    }
                }

            }
        }

        private void methodStrike(int start, int length, int num)
        {
            byte b16 = this.readBuffer[num];
            if (Main.netMode == 2 && this.whoAmI != (int)b16 && (!Main.player[(int)b16].hostile || !Main.player[this.whoAmI].hostile))
            {
                return;
            }
            num++;
            int num50 = (int)(this.readBuffer[num] - 1);
            num++;
            short num51 = BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            byte b17 = this.readBuffer[num];
            num++;
            bool pvp = false;
            string string7 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
            if (b17 != 0)
            {
                pvp = true;
            }
            if (Main.player[(int)b16].Hurt((int)num51, num50, pvp, true, string7) > 0.0)
            {
                NetMessage.SendData(26, -1, this.whoAmI, string7, (int)b16, (float)num50, (float)num51, (float)b17, 0);
                return;
            }
        }

        private void methodProjectile(int num)
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
                if (Main.projectile[num54].owner == (int)b17 && Main.projectile[num54].identity == (int)num51 && Main.projectile[num54].active)
                {
                    num53 = num54;
                    break;
                }
            }
            if (num53 == 1000)
            {
                for (int num55 = 0; num55 < 1000; num55++)
                {
                    if (!Main.projectile[num55].active)
                    {
                        num53 = num55;
                        break;
                    }
                }
            }
            if (!Main.projectile[num53].active || Main.projectile[num53].type != (int)b18)
            {
                Main.projectile[num53].SetDefaults((int)b18);
                if (Main.netMode == 2)
                {
                    Netplay.serverSock[this.whoAmI].spamProjectile += 1f;
                }
            }
            Main.projectile[num53].identity = (int)num51;
            Main.projectile[num53].position.X = x6;
            Main.projectile[num53].position.Y = y5;
            Main.projectile[num53].velocity.X = x7;
            Main.projectile[num53].velocity.Y = y6;
            Main.projectile[num53].damage = (int)damage;
            Main.projectile[num53].type = (int)b18;
            Main.projectile[num53].owner = (int)b17;
            Main.projectile[num53].knockBack = knockBack;
            for (int num56 = 0; num56 < Projectile.maxAI; num56++)
            {
                Main.projectile[num53].ai[num56] = array2[num56];
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(27, -1, this.whoAmI, "", num53, 0f, 0f, 0f);
                return;
            }
        }

        private void method28(int num)
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
                Main.npc[(int)num57].StrikeNPC((int)num58, num59, num60);
            }
            else
            {
                Main.npc[(int)num57].life = 0;
                Main.npc[(int)num57].HitEffect(0, 10.0);
                Main.npc[(int)num57].active = false;
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(28, -1, this.whoAmI, "", (int)num57, (float)num58, num59, (float)num60);
                NetMessage.SendData(23, -1, -1, "", (int)num57, 0f, 0f, 0f);
                return;
            }
        }

        private void method29(int num)
        {
            short num62 = BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            byte b20 = this.readBuffer[num];
            if (Main.netMode == 2)
            {
                b20 = (byte)this.whoAmI;
            }
            for (int num63 = 0; num63 < 1000; num63++)
            {
                if (Main.projectile[num63].owner == (int)b20 && Main.projectile[num63].identity == (int)num62 && Main.projectile[num63].active)
                {
                    Main.projectile[num63].Kill();
                    break;
                }
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(29, -1, this.whoAmI, "", (int)num62, (float)b20, 0f, 0f, 0);
                return;
            }
        }

        private void method30(int num)
        {
            byte b21 = this.readBuffer[num];
            if (Main.netMode == 2)
            {
                b21 = (byte)this.whoAmI;
            }
            num++;
            byte b22 = this.readBuffer[num];
            if (b22 == 1)
            {
                Main.player[(int)b21].hostile = true;
            }
            else
            {
                Main.player[(int)b21].hostile = false;
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(30, -1, this.whoAmI, "", (int)b21, 0f, 0f, 0f, 0);
                string str = " has enabled PvP!";
                if (b22 == 0)
                {
                    str = " has disabled PvP!";
                }
                NetMessage.SendData(25, -1, -1, Main.player[(int)b21].name + str, 255, (float)Main.teamColor[Main.player[(int)b21].team].R, (float)Main.teamColor[Main.player[(int)b21].team].G, (float)Main.teamColor[Main.player[(int)b21].team].B, 0);
                return;
            }
        }

        private void method31(int num)
        {
            int x8 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            int y7 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            int num63 = Chest.FindChest(x8, y7);
            var Event = new ChestOpenEvent();
            Event.Sender = Main.player[whoAmI];
            Event.ID = num63;
            Program.server.getPluginManager().processHook(Hooks.PLAYER_CHEST, Event);
            if (Event.Cancelled) return;
            if (num63 > -1 && Chest.UsingChest(num63) == -1)
            {
                for (int num64 = 0; num64 < Chest.maxItems; num64++)
                {
                    NetMessage.SendData(32, this.whoAmI, -1, "", num63, (float)num64, 0f, 0f);
                }
                NetMessage.SendData(33, this.whoAmI, -1, "", num63, 0f, 0f, 0f);
                Main.player[this.whoAmI].chest = num63;
                return;
            }
        }

        private void method32(int start, int length, int num)
        {
            int num65 = (int)BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            int num66 = (int)this.readBuffer[num];
            num++;
            int stack3 = (int)this.readBuffer[num];
            num++;
            string string8 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
            if (Main.chest[num65] == null)
            {
                Main.chest[num65] = new Chest();
            }
            if (Main.chest[num65].contents[num66] == null)
            {
                Main.chest[num65].contents[num66] = new Item();
            }
            Main.chest[num65].contents[num66].SetDefaults(string8);
            Main.chest[num65].contents[num66].Stack = stack3;
            return;
        }

        private void method33(int num)
        {
            int num67 = BitConverter.ToInt32(this.readBuffer, num);
            num += 2;
            int chestX = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            int chestY = BitConverter.ToInt32(this.readBuffer, num);
            if (Main.netMode == 1)
            {
                if (Main.player[Main.myPlayer].chest == -1)
                {
                    Main.playerInventory = true;
                }
                else
                {
                    if (Main.player[Main.myPlayer].chest != num67 && num67 != -1)
                    {
                        Main.playerInventory = true;
                    }
                }
                Main.player[Main.myPlayer].chest = num67;
                Main.player[Main.myPlayer].chestX = chestX;
                Main.player[Main.myPlayer].chestY = chestY;
                return;
            }
            Main.player[this.whoAmI].chest = num67;
            return;
        }

        private void method34(int num)
        {
            int num69 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            int num70 = BitConverter.ToInt32(this.readBuffer, num);
            if (Main.tile[num69, num70].type == 21)
            {
                WorldGen.KillTile(num69, num70, false, false, false);
                if (!Main.tile[num69, num70].active)
                {
                    NetMessage.SendData(17, -1, -1, "", 0, (float)num69, (float)num70, 0f, 0);
                    return;
                }
            }
        }

        private void method35(int num)
        {
            int num71 = (int)this.readBuffer[num];
            if (Main.netMode == 2)
            {
                num71 = this.whoAmI;
            }
            num++;
            int num72 = (int)BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            if (num71 != Main.myPlayer)
            {
                Main.player[num71].HealEffect(num72);
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(35, -1, this.whoAmI, "", num71, (float)num72, 0f, 0f, 0);
                return;
            }
        }

        private void method36(int num)
        {
            int num73 = (int)this.readBuffer[num];
            if (Main.netMode == 2)
            {
                num73 = this.whoAmI;
            }
            num++;
            int num74 = (int)this.readBuffer[num];
            num++;
            int num75 = (int)this.readBuffer[num];
            num++;
            int num76 = (int)this.readBuffer[num];
            num++;
            int num77 = (int)this.readBuffer[num];
            num++;
            if (num74 == 0)
            {
                Main.player[num73].zoneEvil = false;
            }
            else
            {
                Main.player[num73].zoneEvil = true;
            }
            if (num75 == 0)
            {
                Main.player[num73].zoneMeteor = false;
            }
            else
            {
                Main.player[num73].zoneMeteor = true;
            }
            if (num76 == 0)
            {
                Main.player[num73].zoneDungeon = false;
            }
            else
            {
                Main.player[num73].zoneDungeon = true;
            }
            if (num77 == 0)
            {
                Main.player[num73].zoneJungle = false;
            }
            else
            {
                Main.player[num73].zoneJungle = true;
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(36, -1, this.whoAmI, "", num73, 0f, 0f, 0f, 0);
                return;
            }
        }

        private void method37()
        {
            if (Main.autoPass)
            {
                NetMessage.SendData(38, -1, -1, Netplay.password, 0, 0f, 0f, 0f, 0);
                Main.autoPass = false;
                return;
            }
            Netplay.password = "";
            Main.menuMode = 31;
        }

        private void method38(int start, int length, int num)
        {
            string string9 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
            if (string9 == Netplay.password)
            {
                Netplay.serverSock[this.whoAmI].state = 1;
                NetMessage.SendData(3, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
                return;
            }
            NetMessage.SendData(2, this.whoAmI, -1, "Incorrect password.", 0, 0f, 0f, 0f, 0);
        }

        private void method39(int num)
        {
            short num78 = BitConverter.ToInt16(this.readBuffer, num);
            Main.item[(int)num78].Owner = 255;
            NetMessage.SendData(22, -1, -1, "", (int)num78, 0f, 0f, 0f, 0);
        }

        private void method40(int num)
        {
            byte b23 = this.readBuffer[num];
            if (Main.netMode == 2)
            {
                b23 = (byte)this.whoAmI;
            }
            num++;
            int talkNPC = (int)BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            Main.player[(int)b23].talkNPC = talkNPC;
            if (Main.netMode == 2)
            {
                NetMessage.SendData(40, -1, this.whoAmI, "", (int)b23, 0f, 0f, 0f, 0);
            }
        }

        private void method41(int num)
        {
            byte b24 = this.readBuffer[num];
            if (Main.netMode == 2)
            {
                b24 = (byte)this.whoAmI;
            }
            num++;
            float itemRotation = BitConverter.ToSingle(this.readBuffer, num);
            num += 4;
            int itemAnimation = (int)BitConverter.ToInt16(this.readBuffer, num);
            Main.player[(int)b24].itemRotation = itemRotation;
            Main.player[(int)b24].itemAnimation = itemAnimation;
            if (Main.netMode == 2)
            {
                NetMessage.SendData(41, -1, this.whoAmI, "", (int)b24, 0f, 0f, 0f, 0);
            }
        }

        private void method42(int num)
        {
            int num79 = (int)this.readBuffer[num];
            if (Main.netMode == 2)
            {
                num79 = this.whoAmI;
            }
            num++;
            int statMana = (int)BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            int statManaMax = (int)BitConverter.ToInt16(this.readBuffer, num);
            if (Main.netMode == 2)
            {
                num79 = this.whoAmI;
            }
            Main.player[num79].statMana = statMana;
            Main.player[num79].statManaMax = statManaMax;
            if (Main.netMode == 2)
            {
                NetMessage.SendData(42, -1, this.whoAmI, "", num79, 0f, 0f, 0f, 0);
                return;
            }
        }

        private void method43(int num)
        {
            int num80 = (int)this.readBuffer[num];
            if (Main.netMode == 2)
            {
                num80 = this.whoAmI;
            }
            num++;
            int num81 = (int)BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            if (num80 != Main.myPlayer)
            {
                Main.player[num80].ManaEffect(num81);
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(43, -1, this.whoAmI, "", num80, (float)num81, 0f, 0f, 0);
                return;
            }
        }

        private void method44(int start, int length, int num)
        {
            byte b25 = this.readBuffer[num];
            if ((int)b25 == Main.myPlayer)
            {
                return;
            }
            if (Main.netMode == 2)
            {
                b25 = (byte)this.whoAmI;
            }
            num++;
            int num82 = (int)(this.readBuffer[num] - 1);
            num++;
            short num83 = BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            byte b26 = this.readBuffer[num];
            num++;
            string string10 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
            bool pvp2 = false;
            if (b26 != 0)
            {
                pvp2 = true;
            }
            Main.player[(int)b25].KillMe((double)num83, num82, pvp2, string10);
            if (Main.netMode == 2)
            {
                NetMessage.SendData(44, -1, this.whoAmI, string10, (int)b25, (float)num82, (float)num83, (float)b26, 0);
            }
        }

        private void method45(int num)
        {
            int num83 = (int)this.readBuffer[num];
            if (Main.netMode == 2)
            {
                num83 = this.whoAmI;
            }
            num++;
            int num84 = (int)this.readBuffer[num];
            num++;
            int team = Main.player[num83].team;
            if (Main.netMode == 2)
            {
                NetMessage.SendData(45, -1, this.whoAmI, "", num83, 0f, 0f, 0f);
                Party party = Party.NONE;
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
                        party = Party.RED;
                    }
                    else
                    {
                        if (num84 == 2)
                        {
                            str2 = " has joined the green party.";
                            party = Party.GREEN;
                        }
                        else
                        {
                            if (num84 == 3)
                            {
                                str2 = " has joined the blue party.";
                                party = Party.BLUE;
                            }
                            else
                            {
                                if (num84 == 4)
                                {
                                    str2 = " has joined the yellow party.";
                                    party = Party.YELLOW;
                                }
                            }
                        }
                    }
                }
                PartyChangeEvent Event = new PartyChangeEvent();
                Event.PartyType = party;
                Event.Sender = Main.player[this.whoAmI];
                Program.server.getPluginManager().processHook(Plugin.Hooks.PLAYER_PARTYCHANGE, Event);
                if (Event.Cancelled)
                {
                    return;
                }

                Main.player[num83].team = num84;
                for (int num85 = 0; num85 < 255; num85++)
                {
                    if (num85 == this.whoAmI || (team > 0 && Main.player[num85].team == team) || (num84 > 0 && Main.player[num85].team == num84))
                    {
                        NetMessage.SendData(25, num85, -1, Main.player[num83].name + str2, 255, (float)Main.teamColor[num84].R, (float)Main.teamColor[num84].G, (float)Main.teamColor[num84].B);
                    }
                }
                return;
            }
        }

        private void method46(int num)
        {
            int i2 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            int j2 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            int num87 = Sign.ReadSign(i2, j2);
            if (num87 >= 0)
            {
                NetMessage.SendData(47, this.whoAmI, -1, "", num87, 0f, 0f, 0f, 0);
                return;
            }
        }

        private void method47(int start, int length, int num)
        {
            int num88 = (int)BitConverter.ToInt16(this.readBuffer, num);
            num += 2;
            int x9 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            int y8 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            string string11 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
            Main.sign[num88] = new Sign();
            Main.sign[num88].x = x9;
            Main.sign[num88].y = y8;
            Sign.TextSign(num88, string11);
            if (Main.netMode == 1 && Main.sign[num88] != null && num88 != Main.player[Main.myPlayer].sign)
            {
                Main.playerInventory = false;
                Main.player[Main.myPlayer].talkNPC = -1;
                Main.editSign = false;
                Main.player[Main.myPlayer].sign = num88;
                Main.npcChatText = Main.sign[num88].text;
                return;
            }
        }

        private void method48(int num)
        {
            int num89 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            int num90 = BitConverter.ToInt32(this.readBuffer, num);
            num += 4;
            byte liquid = this.readBuffer[num];
            num++;
            byte b27 = this.readBuffer[num];
            num++;
            if (Main.netMode == 2 && Netplay.spamCheck)
            {
                int num91 = this.whoAmI;
                int num92 = (int)(Main.player[num91].position.X + (float)(Main.player[num91].width / 2));
                int num93 = (int)(Main.player[num91].position.Y + (float)(Main.player[num91].height / 2));
                int num94 = 10;
                int num95 = num92 - num94;
                int num96 = num92 + num94;
                int num97 = num93 - num94;
                int num98 = num93 + num94;
                if (num92 < num95 || num92 > num96 || num93 < num97 || num93 > num98)
                {
                    NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Liquid spam");
                    return;
                }
            }
            if (Main.tile[num89, num90] == null)
            {
                Main.tile[num89, num90] = new Tile();
            }
            lock (Main.tile[num89, num90])
            {
                Main.tile[num89, num90].liquid = liquid;
                if (b27 == 1)
                {
                    Main.tile[num89, num90].lava = true;
                }
                else
                {
                    Main.tile[num89, num90].lava = false;
                }
                if (Main.netMode == 2)
                {
                    WorldGen.SquareTileFrame(num89, num90, true);
                }
                return;
            }
        }

        private void method49()
        {
            if (Netplay.clientSock.state == 6)
            {
                Netplay.clientSock.state = 10;
                Main.player[Main.myPlayer].Spawn();
                return;
            }
        }

        private void method50(int num)
        {
            int num99 = (int)this.readBuffer[num];
            num++;
            if (Main.netMode == 2)
            {
                num99 = this.whoAmI;
            }
            else
            {
                if (num99 == Main.myPlayer)
                {
                    return;
                }
            }

            for (int num100 = 0; num100 < 10; num100++)
            {
                Main.player[num99].buffType[num100] = (int)this.readBuffer[num];
                if (Main.player[num99].buffType[num100] > 0)
                {
                    Main.player[num99].buffTime[num100] = 60;
                }
                else
                {
                    Main.player[num99].buffTime[num100] = 0;
                }
                num++;
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(50, -1, this.whoAmI, "", num99, 0f, 0f, 0f, 0);
                return;
            }
        }

        private void method51(int num)
        {
            byte b28 = this.readBuffer[num];
            if (b28 == 1)
            {
                NPC.SpawnSkeletron();
            }
        }

    }
}
