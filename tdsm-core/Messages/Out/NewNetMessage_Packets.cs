using System;
using System.IO;
using System.IO.Compression;
using tdsm.api;
using Terraria;

namespace tdsm.core.Messages.Out
{
    public partial class NewNetMessage
    {
        public void ConnectionRequest(string version)
        {
            Begin(Packet.CONNECTION_REQUEST);
            String("tdsm.api");
            String(version);
            End();
        }

        public void Disconnect(string reason)
        {
            Begin(Packet.DISCONNECT);
            String(reason);
            End();
        }

        public void ConnectionResponse(int clientId)
        {
            Begin(Packet.CONNECTION_RESPONSE);
            Byte(clientId);
            End();
        }

        public void PlayerData(int playerId)
        {
            var player = Main.player[playerId];

            Begin(Packet.PLAYER_DATA);

            Byte(playerId);

            Byte(player.male ? 0 : 1);
            Byte(player.hair);

            String(player.name);

            Byte(player.hairDye);
            Byte(player.hideVisual);

            Color(player.hairColor);
            Color(player.skinColor);
            Color(player.eyeColor);
            Color(player.shirtColor);
            Color(player.underShirtColor);
            Color(player.pantsColor);
            Color(player.shoeColor);

            Byte(player.difficulty);

            End();
        }

        public void InventoryData(int playerId, int invId, int prefix)
        {
            var player = Main.player[playerId];

            Begin(Packet.INVENTORY_DATA);

            Byte(playerId);
            Byte(invId);

            Item item;

            if (invId < 59)
                item = player.inventory[invId];
            else
            {
                if (invId >= 75f && invId <= 82f)
                    item = player.dye[invId - 58 - 17];
                else
                    item = player.armor[invId - 58 - 1];
            }

            Short(Math.Max(0, item.stack));

            Byte(prefix);

            Short(item.netID);

            End();
        }

        public void WorldRequest()
        {
            Begin(Packet.WORLD_REQUEST);
            End();
        }

        public void WorldData(int spawnX, int spawnY, bool hardMode)
        {
            byte flags = 0;

            Begin(Packet.WORLD_DATA);

            Int(Main.time);

            if (Main.dayTime) flags += 1;
            if (Main.bloodMoon) flags += 2;
            if (Main.eclipse) flags += 4;

            Byte(flags);

            Byte(Main.moonPhase);

            Short(Main.maxTilesX);
            Short(Main.maxTilesY);
            Short(spawnX);
            Short(spawnY);

            Short((short)Main.worldSurface);
            Short((short)Main.rockLayer);
            Int(Main.worldID);
            String(Main.worldName);
            Byte(Main.moonType);
            Byte(WorldGen.treeBG);
            Byte(WorldGen.corruptBG);
            Byte(WorldGen.jungleBG);
            Byte(WorldGen.snowBG);
            Byte(WorldGen.hallowBG);
            Byte(WorldGen.crimsonBG);
            Byte(WorldGen.desertBG);
            Byte(WorldGen.oceanBG);
            Byte(Main.iceBackStyle);
            Byte(Main.jungleBackStyle);
            Byte(Main.hellBackStyle);
            Float(Main.windSpeedSet);
            Byte(Main.numClouds);

            for (int i = 0; i < 3; i++) Int(Main.treeX[i]);
            for (int j = 0; j < 4; j++) Byte((byte)Main.treeStyle[j]);
            for (int k = 0; k < 3; k++) Int(Main.caveBackX[k]);
            for (int l = 0; l < 4; l++) Byte((byte)Main.caveBackStyle[l]);

            if (!Main.raining)
                Main.maxRaining = 0f;
            Float(Main.maxRaining);

            flags = 0;
            if (WorldGen.shadowOrbSmashed) flags += 1;
            if (NPC.downedBoss1) flags += 2;
            if (NPC.downedBoss2) flags += 4;
            if (NPC.downedBoss3) flags += 8;
            if (hardMode) flags += 16;
            if (NPC.downedClown) flags += 32;
            if (NPC.downedPlantBoss) flags += 64;

            Byte(flags);

            flags = 0;
            if (NPC.downedMechBoss1) flags += 1;
            if (NPC.downedMechBoss2) flags += 2;
            if (NPC.downedMechBoss3) flags += 4;
            if (NPC.downedMechBossAny) flags += 8;
            if (Main.cloudBGActive >= 1f) flags += 16;
            if (WorldGen.crimson) flags += 32;
            if (Main.pumpkinMoon) flags += 64;
            if (Main.snowMoon) flags += 128;

            Byte(flags);

            End();
        }

        public void WorldData(int spawnX, int spawnY)
        {
            WorldData(spawnX, spawnY, Main.hardMode);
        }

        public void WorldData()
        {
            WorldData(Main.spawnTileX, Main.spawnTileY);
        }

        public void WorldData(bool hardMode)
        {
            WorldData(Main.spawnTileX, Main.spawnTileY, hardMode);
        }

        public void RequestTileBlock()
        {
            throw new NotImplementedException("NewNetMessage.RequestTileBlock()");
        }

        public void SendTileLoading(int number, string text)
        {
            Begin(Packet.SEND_TILE_LOADING);

            Int(number);
            String(text);

            End();
        }

        private void Tile(Tile tile)
        {
            byte flags = 0;

            var active = tile.active();
            var wall = tile.wall;
            var liquid = tile.liquid;

            if (active) flags += 1;
            //if (tile.Lighted)    flags += 2; //UNUSED
            if (wall > 0) flags += 4;
            if (liquid > 0) flags += 8;
            if (tile.wire()) flags += 16;
            if (tile.halfBrick()) flags += 32;
            if (tile.actuator()) flags += 64;
            if (tile.inActive()) flags += 128;

            Byte(flags);

            byte tileColour = 0, wallColour = 0;
            flags = 0;
            if (tile.wire2()) flags += 1;
            if (tile.wire3()) flags += 2;
            if (tile.active() && tile.color() > 0)
            {
                flags += 4;
                tileColour = tile.color();
            }
            if (tile.wall > 0 && tile.wallColor() > 0)
            {
                flags += 8;
                wallColour = tile.wallColor();
            }
            Byte(flags + (byte)(tile.slope() << 4));

            if (tileColour > 0) Byte(tileColour);
            if (wallColour > 0) Byte(wallColour);

            if (tile.active())
            {
                UShort(tile.type);
                if (Main.tileFrameImportant[(int)tile.type])
                {
                    Short(tile.frameX);
                    Short(tile.frameY);
                }
            }
            if (tile.wall > 0) Byte(tile.wall);

            if (tile.liquid > 0)
            {
                Byte(tile.liquid);
                Byte(tile.liquidType());
            }
        }

        private void Tile(int x, int y)
        {
            Tile(Main.tile[x, y]);
        }


        //#if TEST_COMPRESSION
        private int TileSize(Tile tile)
        {
            int count = 1;

            var active = tile.active();
            var wall = tile.wall;
            var liquid = tile.liquid;

            if (active)
            {
                var type = tile.type;

                count += 1;

                if (Main.tileFrameImportant[type])
                {
                    count += 4;
                }
            }

            if (wall > 0)
            {
                count += 1;
            }

            if (liquid > 0)
            {
                count += 2;
            }

            return count;
        }

        private byte CompressedTileFlags(Tile tile, Tile last)
        {
            byte flags = 0;

            var active = tile.active();
            var type = tile.type;

            if (active != last.active()) flags |= 1;
            //if (tile.lighted != last.) flags |= 2;
            if (tile.wall != last.wall) flags |= 4;
            if (tile.liquid != last.liquid) flags |= 8;
            if (tile.lava() != last.lava()) flags |= 16;
            if (active)
            {
                if (last.type != type || (flags & 1) != 0) flags |= 32;

                if (Main.tileFrameImportant[type] && (last.frameX != tile.frameX || last.frameY != tile.frameY || (flags & 1) != 0))
                {
                    flags |= 64;
                }
            }

            return flags;
        }

        private void CompressedTileBody(byte flags, Tile tile)
        {
            Byte(flags);

            var type = tile.type;
            var wall = tile.wall;
            var liquid = tile.liquid;

            if (tile.active())
            {
                if ((flags & 32) != 0)
                {
                    Byte(type);
                }

                if ((flags & 64) != 0)
                {
                    Short(tile.frameX);
                    Short(tile.frameY);
                }
            }

            if ((flags & 4) != 0)
            {
                Byte(wall);
            }

            if ((flags & 8) != 0)
            {
                Byte(liquid);
            }
        }

        public void TileRowCompressed(int numColumns, int firstColumn, int row)
        {
            Begin(Packet.TILE_ROW_COMPRESSED);

            Short(numColumns);
            Int(firstColumn);
            Int(row);

            Tile last = default(Tile);
            int run = 0;

            for (int col = firstColumn; col < firstColumn + numColumns; col++)
            {
                var tile = Main.tile[col, row];

                byte flags = CompressedTileFlags(tile, last);
                if (flags != 0)
                {
                    while (run > 0)
                    {
                        int count = Math.Min(run, 127);
                        Byte(count | 128);
                        run -= count;
                    }

                    CompressedTileBody(flags, tile);

                    last = tile;
                }
                else
                {
                    run += 1;
                }
            }

            while (run > 0)
            {
                int count = Math.Min(run, 127);
                Byte(count | 128);
                run -= count;
            }

            End();
        }

        public int TileRowSize(int numColumns, int firstColumn, int row)
        {
            int count = 5 + 2 + 4 + 4;

            for (int col = firstColumn; col < firstColumn + numColumns; col++)
            {
                count += TileSize(Main.tile[col, row]);
            }

            return count;
        }
        //#endif

        public void SendTileRowCompressed(int xStart, int yStart, short width, short height, bool packChests)
        {
            Begin(Packet.SEND_TILE_ROW);

            var bufferStart = this.bin.BaseStream.Position;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(xStart);
                    binaryWriter.Write(yStart);
                    binaryWriter.Write(width);
                    binaryWriter.Write(height);
                    NetMessage.CompressTileBlock_Inner(binaryWriter, xStart, yStart, (int)width, (int)height, packChests);
                    int num = this.buf.Length;
                    if ((long)bufferStart + memoryStream.Length > (long)num)
                    {
                        //result = (int)((long)(num - bufferStart) + memoryStream.Length);
                    }
                    else
                    {
                        memoryStream.Position = 0L;
                        MemoryStream memoryStream2 = new MemoryStream();
                        using (DeflateStream deflateStream = new DeflateStream(memoryStream2, CompressionMode.Compress, true))
                        {
                            memoryStream.CopyTo(deflateStream);
                            deflateStream.Flush();
                            deflateStream.Close();
                            deflateStream.Dispose();
                        }
                        if (memoryStream.Length <= memoryStream2.Length)
                        {
                            memoryStream.Position = 0L;
                            //buffer[bufferStart] = 0;
                            Byte(0);
                            //memoryStream.Read(buffer, bufferStart, (int)memoryStream.Length);
                            //result = (int)memoryStream.Length + 1;

                            var b = new byte[128];
                            int read = 0;
                            while ((read = memoryStream.Read(b, 0, b.Length)) > 0)
                            {
                                Byte(b, read);
                            }
                        }
                        else
                        {
                            memoryStream2.Position = 0L;
                            Byte(1);
                            //memoryStream2.Read(buffer, bufferStart, (int)memoryStream2.Length);
                            //memoryStream2.CopyTo(sink);
                            //result = (int)memoryStream2.Length + 1;

                            var b = new byte[128];
                            int read = 0;
                            while ((read = memoryStream2.Read(b, 0, b.Length)) > 0)
                            {
                                Byte(b, read);
                            }
                        }
                    }
                }
            }

            End();
        }

        //public void SendTileRow(int numColumns, int firstColumn, int row)
        //{
        //    Begin(Packet.SEND_TILE_ROW);

        //    Short(numColumns);
        //    Int(firstColumn);
        //    Int(row);

        //    for (int col = firstColumn; col < firstColumn + numColumns; col++)
        //    {
        //        var tile = Main.tile[col, row];

        //        Tile(tile);

        //        int run = 0;

        //        for (int i = col + 1; i < firstColumn + numColumns; i++)
        //        {
        //            var tile2 = Main.tile[i, row];

        //            if (tile.active() != tile2.active()) break;

        //            if (tile.active())
        //            {
        //                if (tile.type != tile2.type) break;

        //                if (Main.tileFrameImportant[tile.type])
        //                {
        //                    if (tile.frameX != tile2.frameX || tile.frameY != tile2.frameY) break;
        //                }
        //            }

        //            if (tile.wall != tile2.wall || tile.liquid != tile2.liquid || tile.lava() != tile2.lava() || tile.wire() != tile2.wire())
        //                break;

        //            run += 1;
        //        }

        //        Short(run);
        //        col += run;
        //    }

        //    End();
        //}

        public void SendTileConfirm(int sectionX, int sectionY, int sectionXAgain, int sectionYAgain)
        {
            Begin(Packet.SEND_TILE_CONFIRM);

            Short(sectionX);
            Short(sectionY);
            Short(sectionXAgain);
            Short(sectionYAgain);

            End();
        }

        public void ReceivingPlayerJoined(int playerId)
        {
            var player = Main.player[playerId];

            Begin(Packet.RECEIVING_PLAYER_JOINED);

            Byte(playerId);
            Short(player.SpawnX);
            Short(player.SpawnY);

            End();
        }

        public void ReceivingPlayerJoined(int playerId, int sx, int sy)
        {
            Begin(Packet.RECEIVING_PLAYER_JOINED);

            Byte(playerId);
            Short(sx);
            Short(sy);

            End();
        }

        public void PlayerStateUpdate(int playerId)
        {
            var player = Main.player[playerId];
            PlayerStateUpdate(playerId, player.position.X, player.position.Y);
        }

        public void PlayerStateUpdate(int playerId, float px, float py)
        {
            var player = Main.player[playerId];

            Begin(Packet.PLAYER_STATE_UPDATE);

            Byte(playerId);

            byte flags = 0;
            if (player.controlUp) flags += 1;
            if (player.controlDown) flags += 2;
            if (player.controlLeft) flags += 4;
            if (player.controlRight) flags += 8;
            if (player.controlJump) flags += 16;
            if (player.controlUseItem) flags += 32;
            if (player.direction == 1) flags += 64;
            Byte(flags);

            flags = 0;
            if (player.pulley) flags += 1;
            if (player.pulley && player.pulleyDir == 2) flags += 2;
            if (player.velocity != Microsoft.Xna.Framework.Vector2.Zero) flags += 4;
            Byte(flags);

            Byte(player.selectedItem);

            Float(px);
            Float(py);
            Float(player.velocity.X);
            Float(player.velocity.Y);

            End();
        }

        public void SynchBegin(int playerId, int active)
        {
            Begin(Packet.SYNCH_BEGIN);

            Byte(playerId);
            Byte(active);
            End();
        }

        public void UpdatePlayers()
        {
            //Header (Packet.UPDATE_PLAYERS, 0);
            throw new NotImplementedException("NewNetMessage.UpdatePlayers()");
        }

        public void PlayerHealthUpdate(int playerId)
        {
            var player = Main.player[playerId];

            Begin(Packet.PLAYER_HEALTH_UPDATE);

            Byte(playerId);
            Short(player.statLife);
            Short(player.statLifeMax);

            End();
        }

        public void TileBreak(int tileAction, int x, int y, int tileType = 0, int style = 0)
        {
            Begin(Packet.TILE_BREAK);

            Byte(tileAction);
            Short(x);
            Short(y);
            Short(tileType);
            Byte(style);

            End();
        }

        public void TimeSunMoonUpdate()
        {
            throw new NotImplementedException("NewNetMessage.TimeSunMoonUpdate()");
        }

        public void DoorUpdate(int doorAction, int x, int y, int doorDirection)
        {
            Begin(Packet.DOOR_UPDATE);

            Byte(doorAction);
            Short(x);
            Short(y);
            Byte(doorDirection == 1);

            End();
        }

        public void TileSquare(int size, int X, int Y)
        {
            Begin(Packet.TILE_SQUARE);

            Short(size);
            Short(X);
            Short(Y);

            for (int x = X; x < X + size; x++)
            {
                for (int y = Y; y < Y + size; y++)
                {
                    Tile(x, y);
                }
            }

            End();
        }

        public void SingleTileSquare(int X, int Y, Tile tile)
        {
            Begin(Packet.TILE_SQUARE);

            Short(1);
            Int(X);
            Int(Y);

            Tile(tile);

            End();
        }

        public void ItemInfo(int itemId, byte number2)
        {
            var item = Main.item[itemId];

            Begin(Packet.ITEM_INFO);

            Short(itemId);

            Float(item.position.X);
            Float(item.position.Y);
            Float(item.velocity.X);
            Float(item.velocity.Y);

            Short(item.stack);
            Byte(item.prefix);
            Byte(number2);

            if (item.active && item.stack > 0)
                Short(item.netID);
            else
                Short(0);

            End();
        }

        public void ItemOwnerInfo(int itemId)
        {
            var item = Main.item[itemId];

            Begin(Packet.ITEM_OWNER_INFO);

            Short(itemId);
            Byte(item.owner);

            End();
        }

        public void NPCInfo(int npcId)
        {
            var npc = Main.npc[npcId];

            Begin(Packet.NPC_INFO);

            //Short(npcId);

            //Float(npc.position.X);
            //Float(npc.position.Y);
            //Float(npc.velocity.X);
            //Float(npc.velocity.Y);

            //Byte(npc.target);

            //Byte(npc.direction + 1);
            //Byte(npc.directionY + 1);

            //if (npc.active)
            //    Int(npc.life);
            //else
            //    Int(0);

            //if (!npc.active || npc.life <= 0)
            //    npc.netSkip = 0;

            //for (int i = 0; i < npc.maxAI; i++)
            //    Float(npc.ai[i]);

            //Int(npc.netID);

            Short(npcId);

            Float(npc.position.X);
            Float(npc.position.Y);
            Float(npc.velocity.X);
            Float(npc.velocity.Y);

            Byte(npc.target);

            int num6 = npc.life;
            if (!npc.active)
            {
                num6 = 0;
            }
            if (!npc.active || npc.life <= 0)
            {
                npc.netSkip = 0;
            }
            if (npc.name == null)
            {
                npc.name = System.String.Empty;
            }

            var AISet = new bool[4];

            var flags = 0;
            if (npc.direction > 0) flags += 1;
            if (npc.directionY > 0) flags += 2;
            if (AISet[0] = (npc.ai[0] != 0f)) flags += 4;
            if (AISet[1] = (npc.ai[1] != 0f)) flags += 8;
            if (AISet[2] = (npc.ai[2] != 0f)) flags += 16;
            if (AISet[3] = (npc.ai[3] != 0f)) flags += 32;
            if (npc.spriteDirection > 0) flags += 64;
            if (num6 == npc.lifeMax) flags += 128;


            BitsByte bb8 = 0;
            bb8[0] = (npc.direction > 0);
            bb8[1] = (npc.directionY > 0);
            bb8[2] = (AISet[0] = (npc.ai[0] != 0f));
            bb8[3] = (AISet[1] = (npc.ai[1] != 0f));
            bb8[4] = (AISet[2] = (npc.ai[2] != 0f));
            bb8[5] = (AISet[3] = (npc.ai[3] != 0f));
            bb8[6] = (npc.spriteDirection > 0);
            bb8[7] = (num6 == npc.lifeMax);

            Byte((byte)flags);

            for (int x = 0; x < NPC.maxAI; x++)
            {
                if (AISet[x]) Float(npc.ai[x]);
            }

            Short(npc.netID);

            if (!bb8[7])
            {
                if (Main.npcLifeBytes[npc.netID] == 2)
                {
                    Short(num6);
                }
                else
                {
                    if (Main.npcLifeBytes[npc.netID] == 4)
                    {
                        Int(num6);
                    }
                    else
                    {
                        SByte((sbyte)num6);
                    }
                }
            }
            if (Main.npcCatchable[npc.type])
            {
                Byte(npc.releaseOwner);
            }

            End();
        }

        public void StrikeNPC(int npcId, int playerId)
        {
            Begin(Packet.STRIKE_NPC);

            Short(npcId);
            Byte(playerId);

            End();
        }

        public void PlayerChat(int playerId, string text, int r, int g, int b)
        {
            Begin(Packet.PLAYER_CHAT);

            Byte(playerId);
            Byte(r);
            Byte(g);
            Byte(b);

            String((text ?? System.String.Empty), useUTF: true);

            End();
        }

        public void StrikePlayer(int victimId, string deathText, int direction, int damage, int pvpFlag, bool crit = false)
        {
            Begin(Packet.STRIKE_PLAYER);

            Byte(victimId);
            Byte(direction + 1);
            Short(damage);

            String(deathText);

            var flags = 0;
            if (pvpFlag == 1) flags += 1;
            if (crit) flags += 2;

            Byte(flags);

            End();
        }

        public void Projectile(Projectile proj)
        {
            Begin(Packet.PROJECTILE);

            Short(proj.identity);

            Float(proj.position.X);
            Float(proj.position.Y);
            Float(proj.velocity.X);
            Float(proj.velocity.Y);
            Float(proj.knockBack);

            Short(proj.damage);

            Byte(proj.owner);
            Short(proj.type);

            var flags = 0;
            for (int i = 0; i < Terraria.Projectile.maxAI; i++)
            {
                if (proj.ai[i] != 0f)
                    flags += 1 << i;
            }
            Byte(flags);

            for (int i = 0; i < Terraria.Projectile.maxAI; i++)
            {
                if ((flags & (1 << i)) != 0)
                    Float(proj.ai[i]);
            }

            End();
        }

        //public void EraseProjectile(int id, int owner)
        //{
        //    Begin(Packet.PROJECTILE);

        //    Short(id);

        //    Float(-1000);
        //    Float(-1000);
        //    Float(1);
        //    Float(1);
        //    Float(0);

        //    Short(0);

        //    Byte(owner);
        //    Byte(0);

        //    for (int i = 0; i < Terraria.Projectile.maxAI; i++)
        //        Float(0.0f);

        //    End();
        //}

        public void DamageNPC(int npcId, int damage, float knockback, int direction, bool crit = false)
        {
            Begin(Packet.DAMAGE_NPC);

            Short(npcId);
            Short(damage);

            Float(knockback);

            Byte(direction + 1);
            Byte(crit ? 1 : 0);

            End();
        }

        public void KillProjectile(int identity, int owner)
        {
            Begin(Packet.KILL_PROJECTILE);

            Short(identity);
            Byte(owner);

            End();
        }

        public void PlayerPVPChange(int playerId)
        {
            var player = Main.player[playerId];

            Begin(Packet.PLAYER_PVP_CHANGE);

            Byte(playerId);
            Byte(player.hostile);

            End();
        }

        public void OpenChest()
        {
            throw new NotImplementedException("NewNetMessage.OpenChest()");
        }

        public void ChestItem(int chestId, int itemId)
        {
            var chest = Main.chest[chestId];
            var item = chest.item[itemId];

            Begin(Packet.CHEST_ITEM);

            Short(chestId);

            Byte(itemId);
            Short(item.stack);
            Byte(item.prefix);

            if (item.name == null)
                Short(0);
            else
                Short(item.netID);

            End();
        }

        public void PlayerChestUpdate(int chestId, string text)
        {
            Begin(Packet.PLAYER_CHEST_UPDATE);

            Short(chestId);

            if (chestId > -1)
            {
                var chest = Main.chest[chestId];

                Short(chest.x);
                Short(chest.y);
            }
            else
            {
                Short(0);
                Short(0);
            }

            var length = text.Length;
            if (text.Length == 0 || text.Length > 20)
                Byte(255);
            else
            {
                Byte((byte)text.Length);
                String(text);
            }

            End();
        }

        public void KillTile(int number, float number2, float number3, float number4, int number5)
        {
            //throw new NotImplementedException("NewNetMessage.KillTile()");
            Begin(Packet.KILL_TILE);

            Byte((byte)number);
            Short((short)number2);
            Short((short)number3);
            Short((short)number4);

            Netplay.GetSectionX((int)number2);
            Netplay.GetSectionY((int)number3);

            Short((short)number5);

            End();
        }

        public void HealPlayer(int playerId, int amount)
        {
            Begin(Packet.HEAL_PLAYER);

            Byte(playerId);
            Short(amount);

            End();
        }

        public void EnterZone(int playerId)
        {
            var player = Main.player[playerId];

            Begin(Packet.ENTER_ZONE);

            Byte(playerId);

            var flags = 0;
            if (player.zoneEvil) flags += 1;
            if (player.zoneMeteor) flags += 2;
            if (player.zoneDungeon) flags += 4;
            if (player.zoneJungle) flags += 8;
            if (player.zoneHoly) flags += 16;
            if (player.zoneSnow) flags += 32;
            if (player.zoneBlood) flags += 64;
            if (player.zoneCandle) flags += 128;

            Byte(flags);

            End();
        }

        public void PasswordRequest()
        {
            Begin(Packet.PASSWORD_REQUEST);
            End();
        }

        public void PasswordResponse()
        {
            throw new NotImplementedException("NewNetMessage.PasswordResponse()");
        }

        public void ItemOwnerUpdate(int itemId)
        {
            Begin(Packet.ITEM_OWNER_UPDATE);

            Short(itemId);

            End();
        }

        public void NPCTalk(int playerId)
        {
            var player = Main.player[playerId];

            Begin(Packet.NPC_TALK);

            Byte(playerId);
            Short(player.talkNPC);

            End();
        }

        public void PlayerBallswing(int playerId)
        {
            var player = Main.player[playerId];

            Begin(Packet.PLAYER_BALLSWING);

            Byte(playerId);
            Float(player.itemRotation);
            Short(player.itemAnimation);

            End();
        }

        public void PlayerManaUpdate(int playerId)
        {
            var player = Main.player[playerId];

            Begin(Packet.PLAYER_MANA_UPDATE);

            Byte(playerId);
            Short(player.statMana);
            Short(player.statManaMax);

            End();
        }

        public void PlayerUseManaUpdate(int playerId, int amount)
        {
            Begin(Packet.PLAYER_USE_MANA_UPDATE);

            Byte(playerId);
            Short(amount);

            End();
        }

        public void KillPlayerPVP(int victimId, string deathText, int direction, int damage, int pvpFlag)
        {
            Begin(Packet.KILL_PLAYER_PVP);

            Byte(victimId);
            Byte(direction + 1);
            Short(damage);
            Byte(pvpFlag);

            String(deathText, useUTF: true);

            End();
        }

        public void PlayerJoinParty(int playerId)
        {
            var player = Main.player[playerId];

            Begin(Packet.PLAYER_JOIN_PARTY);

            Byte(playerId);
            Byte(player.team);

            End();
        }

        public void ReadSign(int x, int y)
        {
            Begin(Packet.READ_SIGN);

            Short(x);
            Short(y);

            End();
        }

        public void WriteSign(int signId)
        {
            var sign = Main.sign[signId];

            Begin(Packet.WRITE_SIGN);

            Short(signId);
            Short(sign.x);
            Short(sign.y);

            String(sign.text, useUTF: true);

            End();
        }

        public void WriteSign(int signId, int x, int y, string text)
        {
            Begin(Packet.WRITE_SIGN);

            Short(signId);
            Short(x);
            Short(y);

            String(text, useUTF: true);

            End();
        }

        public void FlowLiquid(int x, int y)
        {
            var tile = Main.tile[x, y];

            Begin(Packet.FLOW_LIQUID);

            Short(x);
            Short(y);
            Byte(tile.liquid);
            Byte(tile.liquidType());

            End();
        }

        public void SendSpawn()
        {
            Begin(Packet.SEND_SPAWN);
            End();
        }

        public void PlayerBuffs(int playerId)
        {
            var player = Main.player[playerId];

            Begin(Packet.PLAYER_BUFFS);

            Byte(playerId);

            for (int i = 0; i < 22; i++)
            {
                Byte(player.buffType[i]);
            }

            End();
        }

        public void SummonSkeletron(int action, int param)
        {
            Begin(Packet.SUMMON_SKELETRON);

            Byte(action);
            Byte(param);

            End();
        }

        public void ChestUnlock(int playerId, int param, int x, int y)
        {
            Begin(Packet.CHEST_UNLOCK);

            Byte(playerId);
            Byte(param);

            Short(x);
            Short(y);

            End();
        }

        public void NPCAddBuff(int npcId, int type, int time)
        {
            Begin(Packet.NPC_ADD_BUFF);

            Short(npcId);
            Byte(type);
            Short(time);

            End();
        }

        public void NPCBuffs(int npcId)
        {
            Begin(Packet.NPC_BUFFS);

            Short(npcId);

            var npc = Main.npc[npcId];
            for (int i = 0; i < 5; i++)
            {
                Byte(npc.buffType[i]);
                Short(npc.buffTime[i]);
            }

            End();
        }

        public void PlayerAddBuff(int playerId, int type, int time)
        {
            Begin(Packet.PLAYER_ADD_BUFF);

            Byte(playerId);
            Byte(type);
            Short(time);

            End();
        }

        public void NPCName(int id, string text)
        {
            Begin(Packet.NPC_NAME);

            Short(id);
            String(text);

            End();
        }

        public void WorldBalance(byte good, byte evil, byte blood)
        {
            Begin(Packet.WORLD_BALANCE);

            Byte(good);
            Byte(evil);
            Byte(blood);

            End();
        }

        public void PlayHarp(int playerId, float note)
        {
            Begin(Packet.PLAY_HARP);

            Byte(playerId);
            Float(note);

            End();
        }

        public void HitSwitch(int x, int y)
        {
            Begin(Packet.HIT_SWITCH);

            Short(x);
            Short(y);

            End();
        }

        public void NPCHome(int npcId, int homeTileX, int homeTileY, bool homeless)
        {
            Begin(Packet.NPC_HOME);

            Short(npcId);
            Short(homeTileX);
            Short(homeTileY);
            Byte(homeless);

            End();
        }

        public void PlayerDodge(int number, float number2)
        {
            Begin(Packet.PLAYER_DODGE);

            Byte(number);
            Byte((int)number2);

            End();
        }

        public void Packet63()
        {
            throw new NotImplementedException("NewNetMessage.Packet63");
        }

        public void PaintWall(int number, float number2, float number3)
        {
            Begin(Packet.PAINT_WALL);

            Short(number);
            Short((int)number2);
            Byte((int)number3);

            End();
        }

        public void Teleport(int number, float number2, float number3, float number4, int number5)
        {
            Begin(Packet.TELEPORT);

            var flags = 0;
            if ((number & 1) == 1) flags += 1;
            if ((number & 2) == 2) flags += 2;
            if ((number5 & 1) == 1) flags += 4;
            if ((number5 & 2) == 2) flags += 8;

            Byte(flags);
            Short((short)number2);
            Float(number3);
            Float(number4);

            End();
        }

        public void Packet68()
        {
            //dont't send this
        }

        public void Packet69(string text, int number, float number2, float number3, float number4, int number5)
        {
            Begin(Packet.PACKET_69);

            Netplay.GetSectionX((int)number2);
            Netplay.GetSectionY((int)number3);
            Short((short)number);
            Short((short)number2);
            Short((short)number3);
            String(text);

            End();
        }

        public void CatchNPC(int number, float number2)
        {
            Begin(Packet.CATCH_NPC);

            Short(number);
            Byte((byte)number2);

            End();
        }

        public void ReleaseNPC(int number, float number2, float number3, float number4)
        {
            Begin(Packet.RELEASE_NPC);

            Int(number);
            Int((int)number2);
            Short((short)number3);
            Byte((byte)number4);

            End();
        }

        public void TravelShop()
        {
            Begin(Packet.TRAVEL_SHOP);

            for (int i = 0; i < Chest.maxItems; i++)
                Short((short)Main.travelShop[i]);

            End();
        }

        public void AnglerQuest(string text)
        {
            Begin(Packet.ANGLER_QUEST);

            Byte((byte)Main.anglerQuest);
            var res = Main.anglerWhoFinishedToday.Contains(text);
            Bool(res);

            End();
        }

        public void AngerQuestsFinished(int number)
        {
            Begin(Packet.ANGLER_QUESTS_FINISHED);

            Byte((byte)number);
            Int(Main.player[number].anglerQuestsFinished);

            End();
        }
    }
}
