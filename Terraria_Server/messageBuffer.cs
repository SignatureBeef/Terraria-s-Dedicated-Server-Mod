
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
        public const int BUFFER_MAX = 65535;

        private const int MAX_HAIR_ID = 17;

        public bool broadcast;
        public bool checkBytes;

        public byte[] readBuffer;
        public byte[] writeBuffer;

        public int messageLength;
        public int spamCount;
        public int totalData;
        public int whoAmI;

        public void Reset()
        {
            readBuffer = new byte[BUFFER_MAX];
            writeBuffer = new byte[BUFFER_MAX];
            messageLength = 0;
            totalData = 0;
            spamCount = 0;
        }

        public void GetData(int start, int length)
        {
            if (whoAmI < 256)
            {
                Netplay.serverSock[whoAmI].timeOut = 0;
            }
            else
            {
                Netplay.clientSock.timeOut = 0;
            }

            int num = start + 1;
            byte bufferData = readBuffer[start];

            if (Main.netMode == 1 && Netplay.clientSock.statusMax > 0)
            {
                Netplay.clientSock.statusCount++;
            }

            if (Main.netMode == 2)
            {
                if (bufferData != 38)
                {
                    if (Netplay.serverSock[whoAmI].state == -1)
                    {
                        NetMessage.SendData(2, whoAmI, -1, "Incorrect password.");
                        return;
                    }

                    if (Netplay.serverSock[whoAmI].state < 10 && bufferData > 12 && bufferData != 16 && bufferData != 42 && bufferData != 50)
                    {
                        NetMessage.BootPlayer(whoAmI, "Invalid operation at this state.");
                    }
                }
            }

            //Certain events are only processed under specific net modes. Check for these modes first.
            bool dataSkipped = false;
            if (Main.netMode == 1)
            {
                switch (bufferData)
                {
                    case 2:
                        disconnect(start, length);
                        break;
                    case 3:
                        syncInventory(start);
                        break;
                    case 7:
                        syncWorldTime(start, length, num);
                        break;
                    case 9:
                        method9(start, length);
                        break;
                    case 10:
                        processTiles(start, num, bufferData);
                        break;
                    case 11:
                        method11(num);
                        break;
                    case 14:
                        activatePlayer(num);
                        break;
                    case 23:
                        processNPCs(start, length, num);
                        break;
                    case 37:
                        isAutoPass();
                        break;
                    case 39:
                        disownItem(num);
                        break;
                    default:
                        dataSkipped = true;
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
                        updateTileData(num);
                        break;
                    case 15:
                        syncPlayers();
                        break;
                    case 31:
                        useChest(num);
                        break;
                    case 34:
                        killTile(num);
                        break;
                    case 38:
                        verifyPassword(start, length, num);
                        break;
                    case 46:
                        ReadSign(num);
                        break;
                    default:
                        dataSkipped = true;
                        break;
                }
            }

            /*
             * If the message was not already caught it isn't specific to a certain net mode
             * and should be processed as such.
            */
            if (dataSkipped)
            {
                switch (bufferData)
                {
                    case 4:
                        syncPlayer(start, length, num);
                        break;
                    case 5:
                        syncStacks(start, length);
                        break;
                    case 12:
                        spawnPlayer(num);
                        break;
                    case 13:
                        syncPlayerInput(num);
                        break;
                    case 16:
                        checkPlayerDeath(num);
                        break;
                    case 17:
                        modTile(num);
                        break;
                    case 19:
                        syncDoor(num);
                        break;
                    case 20:
                        processTiles2(start, num, bufferData);
                        break;
                    case 21:
                        moveItem(start, length, num);
                        break;
                    case 22:
                        itemOwner(num);
                        break;
                    case 24:
                        playerAttackNPC(num);
                        break;
                    case 25:
                        processMessageEvent(start, length);
                        break;
                    case (int)Packet.STRIKE_PLAYER:
                        processStrike(start, length, num);
                        break;
                    case (int)Packet.PROJECTILE:
                        processProjectile(num);
                        break;
                    case 28:
                        processNPCDamage(num);
                        break;
                    case 29:
                        killProjectile(num);
                        break;
                    case 30:
                        switchPvP(num);
                        break;
                    case 32:
                        addToChest(start, length, num);
                        break;
                    case 33:
                        manageInventory(num);
                        break;
                    case 35:
                        heal(num);
                        break;
                    case 36:
                        enterZone(num);
                        break;
                    case 40:
                        talkToNPC(num);
                        break;
                    case 41:
                        rotateItem(num);
                        break;
                    case 42:
                        setMana(num);
                        break;
                    case 43:
                        useMana(num);
                        break;
                    case 44:
                        pvpKill(start, length, num);
                        break;
                    case 45:
                        joinParty(num);
                        break;
                    case 47:
                        WriteSign(start, length, num);
                        break;
                    case 48:
                        FlowLiquid(num);
                        break;
                    case 49:
                        SpawnPlayerAlt();
                        break;
                    case 50:
                        Buffs(num);
                        break;
                    case 51:
                        SummonSkeletron(num);
                        break;
                }
            }
        }


        private void login(int start, int length)
        {
            ServerSock serverSock = Netplay.serverSock[whoAmI];
            PlayerLoginEvent loginEvent = new PlayerLoginEvent();
            loginEvent.Socket = serverSock;
            loginEvent.Sender = Main.player[whoAmI];
            Program.server.getPluginManager().processHook(Plugin.Hooks.PLAYER_PRELOGIN, loginEvent);
            if (loginEvent.Cancelled)
            {
                NetMessage.SendData(2, whoAmI, -1, "Disconnected By Server.");
                return;
            }

            String clientName = serverSock.tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0];

            if (Program.server.BanList.containsException(clientName))
            {
                NetMessage.SendData(2, whoAmI, -1, "You are banned from this Server.");
                return;
            }

            if (Program.properties.UseWhiteList && !Program.server.WhiteList.containsException(clientName))
            {
                NetMessage.SendData(2, whoAmI, -1, "You are not on the WhiteList.");
                return;
            }

            if (serverSock.state == 0)
            {
                string version = Encoding.ASCII.GetString(readBuffer, start + 1, length - 1);
                if (!(version == "Terraria" + Statics.CURRENT_RELEASE))
                {
                    NetMessage.SendData(2, whoAmI, -1, "You are not using the same version as this Server.");
                    return;
                }

                if (Netplay.password == null || Netplay.password == "")
                {
                    serverSock.state = 1;
                    NetMessage.SendData(3, whoAmI);
                    return;
                }

                serverSock.state = -1;
                NetMessage.SendData(37, whoAmI);
            }
        }


        private void disconnect(int start, int length)
        {
            Netplay.disconnect = true;
            Main.statusText = Encoding.ASCII.GetString(readBuffer, start + 1, length - 1);
        }


        private void syncInventory(int start)
        {
            if (Netplay.clientSock.state == 1)
            {
                Netplay.clientSock.state = 2;
            }

            int myPlayerNum = (int)readBuffer[start + 1];
            if (myPlayerNum != Main.myPlayer)
            {
                Main.player[myPlayerNum] = (Player)Main.player[Main.myPlayer].Clone();
                Main.player[Main.myPlayer] = new Player();
                Main.player[myPlayerNum].whoAmi = myPlayerNum;
                Main.myPlayer = myPlayerNum;
            }

            NetMessage.SendData(4, -1, -1, Main.player[Main.myPlayer].name, Main.myPlayer);
            NetMessage.SendData(16, -1, -1, String.Empty, Main.myPlayer);
            NetMessage.SendData(42, -1, -1, String.Empty, Main.myPlayer);
            NetMessage.SendData(50, -1, -1, String.Empty, Main.myPlayer);

            int count = 0;
            foreach(Item item in Main.player[Main.myPlayer].inventory)
            {
                NetMessage.SendData(5, -1, -1, item.Name, Main.myPlayer, (float)count++);
            }

            //Count should equal 44 at this point.
            foreach (Item armor in Main.player[Main.myPlayer].armor)
            {
                NetMessage.SendData(5, -1, -1, armor.Name, Main.myPlayer, (float)count++);
            }

            NetMessage.SendData(6);
            if (Netplay.clientSock.state == 2)
            {
                Netplay.clientSock.state = 3;
            }
        }


        private void syncPlayer(int start, int length, int num)
        {
            int playerIndex;
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }
            else
            {
                playerIndex = (int)readBuffer[start + 1];
            }

            if (playerIndex == Main.myPlayer)
            {
                return;
            }

            int hairId = (int)readBuffer[start + 2];
            if (hairId >= MAX_HAIR_ID)
            {
                hairId = 0;
            }

            Player player = Main.player[playerIndex];
            player.hair = hairId;
            player.whoAmi = playerIndex;
            num += 2;

            num = setColor(player.hairColor, num);
            num = setColor(player.skinColor, num);
            num = setColor(player.eyeColor, num);
            num = setColor(player.shirtColor, num);
            num = setColor(player.underShirtColor, num);
            num = setColor(player.pantsColor, num);
            num = setColor(player.shoeColor, num);

            player.hardCore = (readBuffer[num++] != 0);

            player.name = Encoding.ASCII.GetString(readBuffer, num, length - num + start).Trim();

            if (Main.netMode == 2)
            {
                if (Netplay.serverSock[whoAmI].state < 10)
                {
                    int count = 0;
                    foreach(Player otherPlayer in Main.player)
                    {
                        if (count++ != playerIndex && player.name.Equals(otherPlayer.name) && Netplay.serverSock[count].active)
                        {
                            NetMessage.SendData(2, whoAmI, -1, player.name + " is already on this server.");
                            return;
                        }
                    }
                }

                if (player.name.Length > 20)
                {
                    NetMessage.SendData(2, whoAmI, -1, "Name is too long.");
                    return;
                }

                if (player.name == "")
                {
                    NetMessage.SendData(2, whoAmI, -1, "Empty name.");
                    return;
                }

                Netplay.serverSock[whoAmI].oldName = player.name;
                Netplay.serverSock[whoAmI].name = player.name;
                NetMessage.SendData(4, -1, whoAmI, player.name, playerIndex);
            }
        }


        private int setColor(Color color, int bufferPos)
        {
            color.R = readBuffer[bufferPos++];
            color.G = readBuffer[bufferPos++];
            color.B = readBuffer[bufferPos++];
            return bufferPos;
        }


        private void syncStacks(int start, int length)
        {
            int playerIndex = (int)readBuffer[start + 1];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            if (playerIndex != Main.myPlayer)
            {
                Player player = Main.player[playerIndex];
                lock (player)
                {
                    int inventorySlot = (int)readBuffer[start + 2];
                    int stack = (int)readBuffer[start + 3];
                    string itemName = Encoding.ASCII.GetString(readBuffer, start + 4, length - 4);
                    if (inventorySlot < 44)
                    {
                        player.inventory[inventorySlot] = new Item();
                        player.inventory[inventorySlot].SetDefaults(itemName);
                        player.inventory[inventorySlot].Stack = stack;
                    }
                    else
                    {
                        player.armor[inventorySlot - 44] = new Item();
                        player.armor[inventorySlot - 44].SetDefaults(itemName);
                        player.armor[inventorySlot - 44].Stack = stack;
                    }

                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(5, -1, whoAmI, itemName, playerIndex, (float)inventorySlot);
                    }
                }
            }
        }


        private void method6()
        {
            if (Netplay.serverSock[whoAmI].state == 1)
            {
                Netplay.serverSock[whoAmI].state = 2;
            }
            NetMessage.SendData(7, whoAmI);
        }


        private void syncWorldTime(int start, int length, int num)
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
            if (Netplay.clientSock.state == 3)
            {
                Netplay.clientSock.state = 4;
            }
        }


        private void updateTileData(int num)
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

            ServerSock serverSock = Netplay.serverSock[whoAmI];
            if (serverSock.state == 2)
            {
                serverSock.state = 3;
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
            for (int num13 = 0; num13 < 200; num13++)
            {
                if (Main.item[num13].Active)
                {
                    NetMessage.SendData(21, whoAmI, -1, "", num13);
                    NetMessage.SendData(22, whoAmI, -1, "", num13);
                }
            }
            
            //Can't switch to a for each because there are 1001 NPCs.
            for (int num14 = 0; num14 < 1000; num14++)
            {
                if (Main.npc[num14].active)
                {
                    NetMessage.SendData(23, whoAmI, -1, "", num14);
                }
            }
            NetMessage.SendData(49, whoAmI);
        }


        private void method9(int start, int length)
        {
            int num14 = BitConverter.ToInt32(readBuffer, start + 1);
            string string4 = Encoding.ASCII.GetString(readBuffer, start + 5, length - 5);
            Netplay.clientSock.statusMax += num14;
            Netplay.clientSock.statusText = string4;
        }


        private void processTiles(int start, int num, byte bufferData)
        {
            short width = BitConverter.ToInt16(readBuffer, start + 1);
            int left = BitConverter.ToInt32(readBuffer, start + 3);
            int y = BitConverter.ToInt32(readBuffer, start + 7);
            num = start + 11;

            for (int x = left; x < left + (int)width; x++)
            {
                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                }

                Tile tile = Main.tile[x, y];

                byte b3 = readBuffer[num++];
                bool active = tile.active;
                tile.active = ((b3 & 1) == 1);

                if ((b3 & 2) == 2)
                {
                    tile.lighted = true;
                }

                if ((b3 & 4) == 4)
                {
                    tile.wall = 1;
                }
                else
                {
                    tile.wall = 0;
                }

                if ((b3 & 8) == 8)
                {
                    tile.liquid = 1;
                }
                else
                {
                    tile.liquid = 0;
                }

                if (tile.active)
                {
                    int type = (int)tile.type;
                    tile.type = readBuffer[num++];

                    if (Main.tileFrameImportant[(int)tile.type])
                    {
                        tile.frameX = BitConverter.ToInt16(readBuffer, num);
                        num += 2;
                        tile.frameY = BitConverter.ToInt16(readBuffer, num);
                        num += 2;
                    }
                    else if (!active || (int)tile.type != type)
                    {
                        tile.frameX = -1;
                        tile.frameY = -1;
                    }
                }

                if (tile.wall > 0)
                {
                    tile.wall = readBuffer[num++];
                }

                if (tile.liquid > 0)
                {
                    tile.liquid = readBuffer[num++];
                    byte lavaFlag = readBuffer[num++];
                    tile.lava = (lavaFlag == 1);
                }
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData((int)bufferData, -1, whoAmI, "", (int)width, (float)left, (float)y);
            }
        }


        private void method11(int num)
        {
            int startX = (int)BitConverter.ToInt16(readBuffer, num);
            num += 4;
            int startY = (int)BitConverter.ToInt16(readBuffer, num);
            num += 4;
            int endX = (int)BitConverter.ToInt16(readBuffer, num);
            num += 4;
            int endY = (int)BitConverter.ToInt16(readBuffer, num);
            num += 4;
            WorldGen.SectionTileFrame(startX, startY, endX, endY);
        }


        private void spawnPlayer(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            Player player = Main.player[playerIndex];
            player.SpawnX = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            player.SpawnY = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            player.Spawn();

            if (Main.netMode == 2 && Netplay.serverSock[whoAmI].state >= 3)
            {
                if (Netplay.serverSock[whoAmI].state == 3)
                {
                    Netplay.serverSock[whoAmI].state = 10;
                    NetMessage.greetPlayer(whoAmI);
                    NetMessage.syncPlayers();
                    NetMessage.buffer[whoAmI].broadcast = true;
                    NetMessage.SendData(12, -1, whoAmI, "", whoAmI);
                    return;
                }
                NetMessage.SendData(12, -1, whoAmI, "", whoAmI);
            }
        }


        private void syncPlayerInput(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            if (playerIndex == Main.myPlayer)
            {
                return;
            }

            Player player = Main.player[playerIndex];
            if (Main.netMode == 1 && !player.active)
            {
                NetMessage.SendData(15);
            }

            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            player.oldVelocity = player.velocity;

            int controlMap = (int)readBuffer[num++];
            player.selectedItem = (int)readBuffer[num++];

            player.position.X = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            player.position.Y = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            player.velocity.X = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            player.velocity.Y = BitConverter.ToSingle(readBuffer, num);
            num += 4;

            player.fallStart = (int)(player.position.Y / 16f);

            player.controlUp = (controlMap & 1) == 1;
            player.controlDown = (controlMap & 2) == 2;
            player.controlLeft = (controlMap & 4) == 4;
            player.controlRight = (controlMap & 8) == 8;
            player.controlJump = (controlMap & 16) == 16;
            player.controlUseItem = (controlMap & 32) == 32;
            player.direction = (controlMap & 64) == 64 ? 1 : -1;

            if (Main.netMode == 2 && Netplay.serverSock[whoAmI].state == 10)
            {
                NetMessage.SendData(13, -1, whoAmI, "", playerIndex);
            }
        }


        private void activatePlayer(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            int isActive = (int)readBuffer[num];
            if (isActive == 1)
            {
                if (!Main.player[playerIndex].active)
                {
                    Main.player[playerIndex] = new Player();
                }
                Main.player[playerIndex].active = true;
                return;
            }
            Main.player[playerIndex].active = false;
        }


        private void syncPlayers()
        {
            NetMessage.syncPlayers();
        }


        private void checkPlayerDeath(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            if (playerIndex == Main.myPlayer)
            {
                return;
            }

            int statLife = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int statLifeMax = (int)BitConverter.ToInt16(readBuffer, num);

            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            Player player = Main.player[playerIndex];
            player.statLife = statLife;
            player.statLifeMax = statLifeMax;

            if (player.statLife <= 0)
            {
                player.dead = true;
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(16, -1, whoAmI, "", playerIndex);
            }
        }


        private void modTile(int num)
        {
            byte tileAction = readBuffer[num++];
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            byte tileType = readBuffer[num++];
            int style = (int)readBuffer[num];
            bool failFlag = (tileType == 1);

            Tile tile = new Tile();

            if (Main.tile[x, y] != null)
            {
                tile = WorldGen.cloneTile(Main.tile[x, y]);
            }
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }

            tile.tileX = x;
            tile.tileY = y;

            TileBreakEvent breakEvent = new TileBreakEvent();
            breakEvent.Sender = Main.player[whoAmI];
            breakEvent.Tile = tile;
            breakEvent.Type = tileType;
            breakEvent.Position = new Vector2(x, y);
            Program.server.getPluginManager().processHook(Hooks.TILE_BREAK, breakEvent);
            if (breakEvent.Cancelled)
            {
                NetMessage.SendTileSquare(whoAmI, x, y, 1);
                return;
            }

            if (Main.netMode == 2)
            {
                if (!failFlag)
                {
                    if (tileAction == 0 || tileAction == 2 || tileAction == 4)
                    {
                        Netplay.serverSock[whoAmI].spamDelBlock += 1f;
                    }
                    else if (tileAction == 1 || tileAction == 3)
                    {
                        Netplay.serverSock[whoAmI].spamAddBlock += 1f;
                    }
                }

                if (!Netplay.serverSock[whoAmI].tileSection[Netplay.GetSectionX(x), Netplay.GetSectionY(y)])
                {
                    failFlag = true;
                }
            }

            switch (tileAction)
            {
                case 0:
                    WorldGen.KillTile(x, y, failFlag, false, false);
                    break;
                case 1:
                    WorldGen.PlaceTile(x, y, (int)tileType, false, true, -1, style);
                    break;
                case 2:
                    WorldGen.KillWall(x, y, failFlag);
                    break;
                case 3:
                    WorldGen.PlaceWall(x, y, (int)tileType, false);
                    break;
                case 4:
                    WorldGen.KillTile(x, y, failFlag, false, true);
                    break;
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(17, -1, whoAmI, "", (int)tileAction, (float)x, (float)y, (float)tileType);
                if (tileAction == 1 && tileType == 53)
                {
                    NetMessage.SendTileSquare(-1, x, y, 1);
                }
            }
        }


        private void syncDoor(int num)
        {
            byte doorAction = readBuffer[num++];
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            int doorDirection = (int)readBuffer[num];
            int direction = 0;
            if (doorDirection == 0)
            {
                direction = -1;
            }

            bool state = (doorAction == 0); //if open

            if (state)
            {
                WorldGen.OpenDoor(x, y, direction, state, DoorOpener.PLAYER, Main.player[whoAmI]);
            }
            else if (doorAction == 1)
            {
                WorldGen.CloseDoor(x, y, true, DoorOpener.PLAYER, Main.player[whoAmI]);
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(19, -1, whoAmI, "", (int)doorAction, (float)x, (float)y, (float)doorDirection);
            }
        }


        private void processTiles2(int start, int num, byte bufferData)
        {
            short size = BitConverter.ToInt16(readBuffer, start + 1);
            int left = BitConverter.ToInt32(readBuffer, start + 3);
            int top = BitConverter.ToInt32(readBuffer, start + 7);
            num = start + 11;
            for (int x = left; x < left + (int)size; x++)
            {
                for (int y = top; y < top + (int)size; y++)
                {
                    if (Main.tile[x, y] == null)
                    {
                        Main.tile[x, y] = new Tile();
                    }
                    Tile tile = Main.tile[x, y];

                    byte b9 = readBuffer[num++];

                    bool wasActive = tile.active;

                    tile.active = ((b9 & 1) == 1);

                    if ((b9 & 2) == 2)
                    {
                        tile.lighted = true;
                    }

                    if ((b9 & 4) == 4)
                    {
                        tile.wall = 1;
                    }
                    else
                    {
                        tile.wall = 0;
                    }

                    if ((b9 & 8) == 8)
                    {
                        tile.liquid = 1;
                    }
                    else
                    {
                        tile.liquid = 0;
                    }

                    if (tile.active)
                    {
                        int wasType = (int)tile.type;
                        tile.type = readBuffer[num++];
                        if (Main.tileFrameImportant[(int)tile.type])
                        {
                            tile.frameX = BitConverter.ToInt16(readBuffer, num);
                            num += 2;
                            tile.frameY = BitConverter.ToInt16(readBuffer, num);
                            num += 2;
                        }
                        else if (!wasActive || (int)tile.type != wasType)
                        {
                            tile.frameX = -1;
                            tile.frameY = -1;
                        }
                    }

                    if (tile.wall > 0)
                    {
                        tile.wall = readBuffer[num++];
                    }

                    if (tile.liquid > 0)
                    {
                        tile.liquid = readBuffer[num++];
                        byte b10 = readBuffer[num++];
                        tile.lava = (b10 == 1);
                    }
                }
            }

            WorldGen.RangeFrame(left, top, left + (int)size, top + (int)size);
            if (Main.netMode == 2)
            {
                NetMessage.SendData((int)bufferData, -1, whoAmI, "", (int)size, (float)left, (float)top);
            }
        }


        private void moveItem(int start, int length, int num)
        {
            short itemIndex = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            float num39 = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float num40 = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float x3 = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float y2 = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            byte stackSize = readBuffer[num++];

            string string4 = Encoding.ASCII.GetString(readBuffer, num, length - num + start);

            Item item = Main.item[(int)itemIndex];
            if (Main.netMode == 1)
            {
                if (string4 == "0")
                {
                    item.Active = false;
                    return;
                }
                item.SetDefaults(string4);
                item.Stack = (int)stackSize;
                item.Position.X = num39;
                item.Position.Y = num40;
                item.Velocity.X = x3;
                item.Velocity.Y = y2;
                item.Active = true;
                item.Wet = Collision.WetCollision(item.Position, item.Width, item.Height);
            }
            else
            {
                if (string4 == "0")
                {
                    if (itemIndex < 200)
                    {
                        item.Active = false;
                        NetMessage.SendData(21, -1, -1, "", (int)itemIndex, 0f, 0f, 0f, 0);
                    }
                }
                else
                {
                    bool isNewItem = false;
                    if (itemIndex == 200)
                    {
                        isNewItem = true;
                        Item newItem = new Item();
                        newItem.SetDefaults(string4);
                        itemIndex = (short)Item.NewItem((int)num39, (int)num40, newItem.Width, newItem.Height, newItem.Type, (int)stackSize, true);
                        item = Main.item[(int)itemIndex];
                    }

                    item.SetDefaults(string4);
                    item.Stack = (int)stackSize;
                    item.Position.X = num39;
                    item.Position.Y = num40;
                    item.Velocity.X = x3;
                    item.Velocity.Y = y2;
                    item.Active = true;
                    item.Owner = Main.myPlayer;

                    if (isNewItem)
                    {
                        NetMessage.SendData(21, -1, -1, "", (int)itemIndex);
                        item.OwnIgnore = whoAmI;
                        item.OwnTime = 100;
                        item.FindOwner((int)itemIndex);
                        return;
                    }
                    NetMessage.SendData(21, -1, whoAmI, "", (int)itemIndex);
                }
            }
        }


        private void itemOwner(int num)
        {
            short itemIndex = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte owner = readBuffer[num];
            Item item = Main.item[(int)itemIndex];

            if (Main.netMode == 2)
            {
                if (item.Owner != whoAmI)
                {
                    return;
                }

                item.Owner = 255;
                item.KeepTime = 15;
                NetMessage.SendData(22, -1, -1, "", (int)itemIndex);
            }
            else
            {
                item.Owner = (int)owner;
                item.KeepTime = ((int)owner == Main.myPlayer) ? 15 : 0;
            }
        }


        private void processNPCs(int start, int length, int num)
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

            string npcName = Encoding.ASCII.GetString(readBuffer, num, length - num + start);

            NPC npc = Main.npc[(int)npcIndex];
            if (!npc.active || npc.name != npcName)
            {
                npc.active = true;
                npc.SetDefaults(npcName);
            }

            npc.position.X = x;
            npc.position.Y = y;
            npc.velocity.X = vX;
            npc.velocity.Y = vY;
            npc.target = target;
            npc.direction = direction;
            npc.life = life;

            if (life <= 0)
            {
                npc.active = false;
            }

            for (int i = 0; i < NPC.MAX_AI; i++)
            {
                npc.ai[i] = aiInfo[i];
            }
        }

        private void playerAttackNPC(int num)
        {
            short npcIndex = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte playerIndex = readBuffer[num];

            if (Main.netMode == 2)
            {
                playerIndex = (byte)whoAmI;
            }

            Player player = Main.player[(int)playerIndex];
            Main.npc[(int)npcIndex].StrikeNPC(player.inventory[player.selectedItem].Damage, player.inventory[player.selectedItem].KnockBack, player.direction);
            
            if (Main.netMode == 2)
            {
                NetMessage.SendData(24, -1, whoAmI, "", (int)npcIndex, (float)playerIndex);
                NetMessage.SendData(23, -1, -1, "", (int)npcIndex);
            }
        }


        private void processMessageEvent(int start, int length)
        {
            int num46 = (int)readBuffer[start + 1];

            if (Main.netMode == 2)
            {
                num46 = whoAmI;
            }

            if (Main.netMode == 2)
            {
                string chat = Encoding.ASCII.GetString(readBuffer, start + 5, length - 5).ToLower().Trim();

                if (chat.Length > 0)
                {
                    if (chat.Substring(0, 1).Equals("/"))
                    {
                        if (!ProcessMessage(new PlayerCommandEvent(), chat, Hooks.PLAYER_COMMAND))
                        {
                            return;
                        }

                        Program.tConsole.WriteLine(Main.player[whoAmI].name + " Sent Command: " + chat);
                        Program.commandParser.parsePlayerCommand(Main.player[whoAmI], chat);
                        return;
                    }
                    else
                    {
                        if (!ProcessMessage(new PlayerChatEvent(), chat, Hooks.PLAYER_CHAT))
                        {
                            return;
                        }
                    }

                    NetMessage.SendData(25, -1, -1, chat, num46, (float)255, (float)255, (float)255);
                    if (Main.dedServ)
                    {
                        Program.tConsole.WriteLine("<" + Main.player[whoAmI].name + "> " + chat);
                    }
                }

            }
        }


        private bool ProcessMessage(BaseMessageEvent messageEvent, String text, Hooks hook)
        {
            messageEvent.Message = text;
            messageEvent.Sender = Main.player[whoAmI];
            Program.server.getPluginManager().processHook(hook, messageEvent);

            return !messageEvent.Cancelled;
        }


        private void processStrike(int start, int length, int num)
        {
            int playerIndex = (int)readBuffer[num++];
            Player player = Main.player[playerIndex];
            if (Main.netMode == 2 && whoAmI != playerIndex && (!player.hostile || !Main.player[whoAmI].hostile))
            {
                return;
            }

            int hitDirection = (int)(readBuffer[num++] - 1);
            short damage = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte pvpFlag = readBuffer[num++];
            bool pvp = (pvpFlag != 0);
            string deathText = Encoding.ASCII.GetString(readBuffer, num, length - num + start);

            if (player.Hurt((int)damage, hitDirection, pvp, true, deathText) > 0.0)
            {
                NetMessage.SendData(26, -1, whoAmI, deathText, playerIndex, (float)hitDirection, (float)damage, (float)pvpFlag, 0);
            }
        }

        private void processProjectile(int num)
        {
            short projectileIdentity = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            float x = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float y = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float vX = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float vY = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float knockBack = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            short damage = BitConverter.ToInt16(readBuffer, num);
            num += 2;

            byte projectileOwner = readBuffer[num++];
            byte type = readBuffer[num++];

            float[] aiInfo = new float[Projectile.MAX_AI];
            for (int i = 0; i < Projectile.MAX_AI; i++)
            {
                aiInfo[i] = BitConverter.ToSingle(readBuffer, num);
                num += 4;
            }
            
            int projectileIndex = getProjectileIndex(projectileOwner, projectileIdentity);
            Projectile projectile = Main.projectile[projectileIndex];
            if (!projectile.active || projectile.type != (int)type)
            {
                projectile.SetDefaults((int)type);
                if (Main.netMode == 2)
                {
                    Netplay.serverSock[whoAmI].spamProjectile += 1f;
                }
            }

            projectile.identity = (int)projectileIdentity;
            projectile.position.X = x;
            projectile.position.Y = y;
            projectile.velocity.X = vX;
            projectile.velocity.Y = vY;
            projectile.damage = (int)damage;
            projectile.type = (int)type;
            projectile.owner = (int)projectileOwner;
            projectile.knockBack = knockBack;

            for (int i = 0; i < Projectile.MAX_AI; i++)
            {
                projectile.ai[i] = aiInfo[i];
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(27, -1, whoAmI, "", projectileIndex, 0f, 0f, 0f);
            }
        }


        private int getProjectileIndex(int owner, int identity)
        {
            int index = 1000;
            int firstInactive = index;
            Projectile projectile;
            for (int i = 0; i < index; i++)
            {
                projectile = Main.projectile[i];
                if (projectile.owner == owner
                    && projectile.identity == identity
                    && projectile.active)
                {
                    return i;
                }

                if (firstInactive == index && !projectile.active)
                {
                    firstInactive = i;
                }
            }

            return firstInactive;
        }


        private void processNPCDamage(int num)
        {
            short npcIndex = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            short damage = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            float knockback = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            int direction = (int)(readBuffer[num] - 1);

            NPC npc = Main.npc[(int)npcIndex];
            if (damage >= 0)
            {
                npc.StrikeNPC((int)damage, knockback, direction);
            }
            else
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.active = false;
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(28, -1, whoAmI, "", (int)npcIndex, (float)damage, knockback, (float)direction);
                NetMessage.SendData(23, -1, -1, "", (int)npcIndex);
            }
        }


        private void killProjectile(int num)
        {
            short identity = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte owner = readBuffer[num];
            if (Main.netMode == 2)
            {
                owner = (byte)whoAmI;
            }

            Projectile projectile;
            for (int i = 0; i < 1000; i++)
            {
                projectile = Main.projectile[i];
                if (projectile.owner == (int)owner && projectile.identity == (int)identity && projectile.active)
                {
                    projectile.Kill();
                    break;
                }
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(29, -1, whoAmI, "", (int)identity, (float)owner);
            }
        }

        
        private void switchPvP(int num)
        {
            int playerIndex = readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            Player player = Main.player[playerIndex];
            player.hostile = (readBuffer[num] == 1);

            if (Main.netMode == 2)
            {
                NetMessage.SendData(30, -1, whoAmI, "", playerIndex);

                String message;
                if(player.hostile)
                {
                    message = " has enabled PvP!";
                }
                else
                {
                    message = " has disabled PvP!";
                }
                NetMessage.SendData(25, -1, -1, player.name + message, 255, (float)Main.teamColor[player.team].R, (float)Main.teamColor[player.team].G, (float)Main.teamColor[player.team].B);
            }
        }


        private void useChest(int num)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int chestIndex = Chest.FindChest(x, y);
            var chestEvent = new ChestOpenEvent();
            chestEvent.Sender = Main.player[whoAmI];
            chestEvent.ID = chestIndex;
            Program.server.getPluginManager().processHook(Hooks.PLAYER_CHEST, chestEvent);
            
            if (chestEvent.Cancelled)
            {
                return;
            }

            if (chestIndex > -1 && Chest.UsingChest(chestIndex) == -1)
            {
                for (int i = 0; i < Chest.MAX_ITEMS; i++)
                {
                    NetMessage.SendData(32, whoAmI, -1, "", chestIndex, (float)i);
                }
                NetMessage.SendData(33, whoAmI, -1, "", chestIndex);
                Main.player[whoAmI].chest = chestIndex;
                return;
            }
        }


        private void addToChest(int start, int length, int num)
        {
            int chestIndex = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int contentsIndex = (int)readBuffer[num++];
            int stackSize = (int)readBuffer[num++];

            string string8 = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            
            if (Main.chest[chestIndex] == null)
            {
                Main.chest[chestIndex] = new Chest();
            }
            
            if (Main.chest[chestIndex].contents[contentsIndex] == null)
            {
                Main.chest[chestIndex].contents[contentsIndex] = new Item();
            }

            Main.chest[chestIndex].contents[contentsIndex].SetDefaults(string8);
            Main.chest[chestIndex].contents[contentsIndex].Stack = stackSize;
        }


        private void manageInventory(int num)
        {
            int inventoryIndex = BitConverter.ToInt32(readBuffer, num);
            num += 2;
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);

            if (Main.netMode == 1)
            {
                Player player = Main.player[Main.myPlayer];
                if (player.chest == -1
                    || (player.chest != inventoryIndex && inventoryIndex != -1))
                {
                    Main.playerInventory = true;
                }

                player.chest = inventoryIndex;
                player.chestX = x;
                player.chestY = y;
            }
            else
            {
                Main.player[whoAmI].chest = inventoryIndex;
            }
        }


        private void killTile(int num)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            if (Main.tile[x, y].type == 21)
            {
                WorldGen.KillTile(x, y);
                if (!Main.tile[x, y].active)
                {
                    NetMessage.SendData(17, -1, -1, "", 0, (float)x, (float)y);
                }
            }
        }


        private void heal(int num)
        {
            int playerIndex = (int)readBuffer[num++];

            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int heal = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;

            if (playerIndex != Main.myPlayer)
            {
                Main.player[playerIndex].HealEffect(heal);
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(35, -1, whoAmI, "", playerIndex, (float)heal);
            }
        }


        private void enterZone(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            Player player = Main.player[playerIndex];
            player.zoneEvil = (readBuffer[num++] != 0);
            player.zoneMeteor = (readBuffer[num++] != 0);
            player.zoneDungeon = (readBuffer[num++] != 0);
            player.zoneJungle = (readBuffer[num++] != 0);

            if (Main.netMode == 2)
            {
                NetMessage.SendData(36, -1, whoAmI, "", playerIndex);
            }
        }


        private void isAutoPass()
        {
            if (Main.autoPass)
            {
                NetMessage.SendData(38, -1, -1, Netplay.password);
                Main.autoPass = false;
                return;
            }
            Netplay.password = "";
            Main.menuMode = 31;
        }


        private void verifyPassword(int start, int length, int num)
        {
            string password = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            if (password == Netplay.password)
            {
                Netplay.serverSock[whoAmI].state = 1;
                NetMessage.SendData(3, whoAmI);
                return;
            }
            NetMessage.SendData(2, whoAmI, -1, "Incorrect password.");
        }


        private void disownItem(int num)
        {
            short itemIndex = BitConverter.ToInt16(readBuffer, num);
            Main.item[(int)itemIndex].Owner = 255;
            NetMessage.SendData(22, -1, -1, "", (int)itemIndex);
        }


        private void talkToNPC(int num)
        {
            int playerIndex = readBuffer[num++];

            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int talkNPC = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            Main.player[playerIndex].talkNPC = talkNPC;
            if (Main.netMode == 2)
            {
                NetMessage.SendData(40, -1, whoAmI, "", playerIndex);
            }
        }

        private void rotateItem(int num)
        {
            int playerIndex = readBuffer[num++];

            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            float itemRotation = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            int itemAnimation = (int)BitConverter.ToInt16(readBuffer, num);
            Main.player[playerIndex].itemRotation = itemRotation;
            Main.player[playerIndex].itemAnimation = itemAnimation;
            if (Main.netMode == 2)
            {
                NetMessage.SendData(41, -1, whoAmI, "", playerIndex);
            }
        }


        private void setMana(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int statMana = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int statManaMax = (int)BitConverter.ToInt16(readBuffer, num);

            Main.player[playerIndex].statMana = statMana;
            Main.player[playerIndex].statManaMax = statManaMax;
            if (Main.netMode == 2)
            {
                NetMessage.SendData(42, -1, whoAmI, "", playerIndex);
            }
        }

        private void useMana(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int manaAmount = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;

            if (playerIndex != Main.myPlayer)
            {
                Main.player[playerIndex].ManaEffect(manaAmount);
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(43, -1, whoAmI, "", playerIndex, (float)manaAmount);
            }
        }


        private void pvpKill(int start, int length, int num)
        {
            int playerIndex = readBuffer[num++];
            if (playerIndex == Main.myPlayer)
            {
                return;
            }

            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int direction = (int)(readBuffer[num++] - 1);

            short damage = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte pvpFlag = readBuffer[num++];

            string deathText = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            bool pvp = (pvpFlag != 0);

            Main.player[playerIndex].KillMe((double)damage, direction, pvp, deathText);

            if (Main.netMode == 2)
            {
                NetMessage.SendData(44, -1, whoAmI, deathText, playerIndex, (float)direction, (float)damage, (float)pvpFlag, 0);
            }
        }


        private void joinParty(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            int teamIndex = (int)readBuffer[num++];
            Player player = Main.player[playerIndex];
            int currentTeam = player.team;

            if (Main.netMode == 2)
            {
                NetMessage.SendData(45, -1, whoAmI, "", playerIndex);
                Party party = Party.NONE;
                string joinMessage = "";
                switch (teamIndex)
                {
                    case 0:
                        joinMessage = " is no longer on a party.";
                        break;
                    case 1:
                        joinMessage = " has joined the red party.";
                        party = Party.RED;
                        break;
                    case 2:
                        joinMessage = " has joined the green party.";
                        party = Party.GREEN;
                        break;
                    case 3:
                        joinMessage = " has joined the blue party.";
                        party = Party.BLUE;
                        break;
                    case 4:
                        joinMessage = " has joined the yellow party.";
                        party = Party.YELLOW;
                        break;
                }

                PartyChangeEvent changeEvent = new PartyChangeEvent();
                changeEvent.PartyType = party;
                changeEvent.Sender = Main.player[whoAmI];
                Program.server.getPluginManager().processHook(Hooks.PLAYER_PARTYCHANGE, changeEvent);
                if (changeEvent.Cancelled)
                {
                    return;
                }

                player.team = teamIndex;
                for (int i = 0; i < 255; i++)
                {
                    if (i == whoAmI
                        || (currentTeam > 0 && player.team == currentTeam)
                        || (teamIndex > 0 && player.team == teamIndex))
                    {
                        NetMessage.SendData(25, i, -1, player.name + joinMessage, 255, (float)Main.teamColor[teamIndex].R, (float)Main.teamColor[teamIndex].G, (float)Main.teamColor[teamIndex].B);
                    }
                }
            }
        }


        private void ReadSign(int num)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            int signIndex = Sign.ReadSign(x, y);
            if (signIndex >= 0)
            {
                NetMessage.SendData(47, whoAmI, -1, "", signIndex);
            }
        }


        private void WriteSign(int start, int length, int num)
        {
            int signIndex = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            string string11 = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
            Main.sign[signIndex] = new Sign();
            Sign sign = Main.sign[signIndex];
            sign.x = x;
            sign.y = y;
            Sign.TextSign(signIndex, string11);
            Player player = Main.player[Main.myPlayer];

            if (Main.netMode == 1 
                && sign != null
                && signIndex != player.sign)
            {
                Main.playerInventory = false;
                player.talkNPC = -1;
                Main.editSign = false;
                player.sign = signIndex;
                Main.npcChatText = sign.text;
            }
        }

        private void FlowLiquid(int num)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            byte liquid = readBuffer[num++];
            byte lavaFlag = readBuffer[num]++;

            if (Main.netMode == 2 && Netplay.spamCheck)
            {
                int playerIndex = whoAmI;
                Player player = Main.player[playerIndex];
                int centerX = (int)(player.position.X + (float)(player.width / 2));
                int centerY = (int)(player.position.Y + (float)(player.height / 2));
                int disperseDistance = 10;
                int left = centerX - disperseDistance;
                int right = centerX + disperseDistance;
                int top = centerY - disperseDistance;
                int bottom = centerY + disperseDistance;
                if (centerX < left || centerX > right || centerY < top || centerY > bottom)
                {
                    NetMessage.BootPlayer(whoAmI, "Cheating attempt detected: Liquid spam");
                    return;
                }
            }
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }
            Tile tile = Main.tile[x, y];
            lock (tile)
            {
                tile.liquid = liquid;
                tile.lava = (lavaFlag == 1);

                if (Main.netMode == 2)
                {
                    WorldGen.SquareTileFrame(x, y, true);
                }
            }
        }


        private void SpawnPlayerAlt()
        {
            if (Netplay.clientSock.state == 6)
            {
                Netplay.clientSock.state = 10;
                Main.player[Main.myPlayer].Spawn();
            }
        }

        private void Buffs(int num)
        {
            int playerIndex = (int)readBuffer[num++];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }
            else if (playerIndex == Main.myPlayer)
            {
                return;
            }

            Player player = Main.player[playerIndex];
            for (int i = 0; i < 10; i++)
            {
                player.buffType[i] = (int)readBuffer[num++];
                if (player.buffType[i] > 0)
                {
                    player.buffTime[i] = 60;
                }
                else
                {
                    player.buffTime[i] = 0;
                }
            }

            if (Main.netMode == 2)
            {
                NetMessage.SendData(50, -1, whoAmI, "", playerIndex);
            }
        }

        private void SummonSkeletron(int num)
        {
            byte buffer = readBuffer[num];
            if (buffer == 1)
            {
                NPC.SpawnSkeletron();
            }
        }
    }
}