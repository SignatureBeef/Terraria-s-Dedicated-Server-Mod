using Terraria;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Chat;
using tdsm.api.Logging;
using tdsm.api.Plugin;
using Terraria.Net;
using System;
using Terraria.GameContent.Achievements;

namespace tdsm.api.Callbacks
{
    public static class MessageBufferCallback
    {
        //TODO Put these in a Messages folder as like in the old system.
        //When adding a packet ensure the section in the case ends with returning. These packets are safe to filter
        //Ones without a return statement will mean the code at the end of GetData doesnt get called;

        //        public static string PlayerNameFormatForChat = "<{0}> {1}";
        //        public static string PlayerNameFormatForConsole = PlayerNameFormatForChat;

        public static byte ProcessPacket(int bufferId, byte packetId, int start, int length)
        {
            //ProgramLog.Debug.Log("Slot/Packet: {0}/{1}", bufferId, packetId);
            //Console.WriteLine("Trace: {0}", Environment.StackTrace);
            switch ((Packet)packetId)
            {
                /* Misc / Command */
                case Packet.PLAYER_CHAT:
                    ProcessChat(bufferId); //Returns
                    return 0;

                /* Cheat protection */
                case Packet.TILE_BREAK:
                    ProcessTileBreak(bufferId); //Returns
                    return 0;
                case Packet.PROJECTILE:
                    ProcessProjectile(bufferId); //Returns
                    return 0;
                case Packet.CHEST:
                    ProcessChest(bufferId); //Client continues, server returns
                    return 0;
                case Packet.OPEN_CHEST:
                    ProcessChestOpen(bufferId); //Returns
                    return 0;
                case Packet.TILE_SQUARE:
                    ProcessTileSquare(bufferId); //Returns
                    return 0;
                case Packet.READ_SIGN:
                    ProcessReadSign(bufferId); //Returns
                    return 0;
                case Packet.WRITE_SIGN:
                    ProcessWriteSign(bufferId); //Returns
                    return 0;

                /* Password handling */
                case Packet.PASSWORD_RESPONSE: //Returns
                    ProcessPassword(bufferId);
                    return 0;
                case Packet.PLAYER_DATA:
                    ProcessPlayerData(bufferId, start, length); //Returns
                    return 0;
                case Packet.WORLD_REQUEST:
                    if (Netplay.Clients[bufferId].State == -2) return 0; //Ignore
                    break;
            }

            return packetId;
        }

        /// <summary>
        /// CHeck to see if a client sent a wrong message at the wrong state
        /// </summary>
        /// <param name="bufferId"></param>
        /// <param name="packetId"></param>
        /// <returns></returns>
        public static bool CheckForInvalidState(int bufferId, byte packetId)
        {
            var res = Main.netMode == 2 && Netplay.Clients[bufferId].State < 10
                && packetId > 12 && packetId != 93 && packetId != 16 && packetId != 42
                && packetId != 50 && packetId != 38 && packetId != 68;
            if (Netplay.Clients[bufferId].State == -2) return false;

            return res;
        }

        private static void ProcessPassword(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];
            var conn = Netplay.Clients[bufferId];

            if (Main.netMode != 2)
            {
                return;
            }
            var clientPassword = buffer.reader.ReadString();
            if (conn.State == -1)
            {
                var ctx = new HookContext
                {
                    Connection = conn.Socket
                };

                var args = new HookArgs.ServerPassReceived
                {
                    Password = clientPassword,
                };

                HookPoints.ServerPassReceived.Invoke(ref ctx, ref args);

                if (ctx.CheckForKick())
                    return;

                if (ctx.Result == HookResult.ASK_PASS)
                {
                    NetMessage.SendData((int)Packet.PASSWORD_REQUEST, bufferId);
                    return;
                }
                else if (ctx.Result == HookResult.CONTINUE || clientPassword == Netplay.ServerPassword)
                {
                    Netplay.Clients[bufferId].State = 1;
                    NetMessage.SendData((int)Packet.CONNECTION_RESPONSE, bufferId, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
                    return;
                }

                conn.Kick("Incorrect server password.");
            }
            else if (conn.State == -2)
            {
                //var name = player.name ?? "";

                var ctx = new HookContext
                {
                    Connection = conn.Socket,
                    Player = player,
                    Sender = player
                };

                var args = new HookArgs.PlayerPassReceived
                {
                    Password = clientPassword
                };

                HookPoints.PlayerPassReceived.Invoke(ref ctx, ref args);
                
                if (ctx.CheckForKick())
                    return;
                
                if (ctx.Result == HookResult.ASK_PASS)
                {
                    NetMessage.SendData((int)Packet.PASSWORD_REQUEST, bufferId);
                    return;
                }
                else // HookResult.DEFAULT
                {
                    //ProgramLog.Error.Log("Accepted player: " + player.name + "/" + (player.AuthenticatedAs ?? "null"));

                    //Continue with world request
                    Netplay.Clients[bufferId].State = 2;
                    Netplay.Clients[bufferId].ResetSections();
                    NetMessage.SendData(7, bufferId, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
                    Main.SyncAnInvasion(bufferId);

                    return;
                }
            }
        }

        private static void ProcessReadSign(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];

            if (Main.netMode != 2)
            {
                return;
            }
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            int id = Sign.ReadSign(x, y, true);

            if (id >= 0)
            {
                var ctx = new HookContext
                {
                    Connection = player.Connection.Socket,
                    Sender = player,
                    Player = player
                };

                var args = new HookArgs.SignTextGet
                {
                    X = x,
                    Y = y,
                    SignIndex = (short)id,
                    Text = (id >= 0 && Main.sign[id] != null) ? Main.sign[id].text : null,
                };

                HookPoints.SignTextGet.Invoke(ref ctx, ref args);

                if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                    return;

                if (args.Text != null)
                {
                    NetMessage.SendData(47, bufferId, -1, "", id, (float)bufferId, 0, 0, 0, 0, 0);
                    return;
                }
            }
        }

        private static void ProcessWriteSign(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];

            int signId = (int)buffer.reader.ReadInt16();
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            string text = buffer.reader.ReadString();

            string existing = null;
            if (Main.sign[signId] != null)
            {
                existing = Main.sign[signId].text;
            }

            Main.sign[signId] = new Sign();
            Main.sign[signId].x = x;
            Main.sign[signId].y = y;

            Sign.TextSign(signId, text);
            int ply = (int)buffer.reader.ReadByte();


            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Sender = player,
                Player = player,
            };

            var args = new HookArgs.SignTextSet
            {
                X = x,
                Y = y,
                SignIndex = signId,
                Text = text,
                OldSign = Main.sign[signId],
            };

            HookPoints.SignTextSet.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                return;

            if (Main.netMode == 2 && existing != text)
            {
                ply = bufferId;
                NetMessage.SendData(47, -1, bufferId, "", signId, (float)ply, 0, 0, 0, 0, 0);
            }

            if (Main.netMode == 1 && ply == Main.myPlayer && Main.sign[signId] != null)
            {
                Main.playerInventory = false;
                Main.player[Main.myPlayer].talkNPC = -1;
                Main.npcChatCornerItem = 0;
                Main.editSign = false;
                Main.PlaySound(10, -1, -1, 1);
                Main.player[Main.myPlayer].sign = signId;
                Main.npcChatText = Main.sign[signId].text;
            }
        }

        private static void ProcessTileSquare(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];

            short size = buffer.reader.ReadInt16();
            int startX = (int)buffer.reader.ReadInt16();
            int startY = (int)buffer.reader.ReadInt16();
            if (!WorldGen.InWorld(startX, startY, 3))
            {
                return;
            }

            var ctx = new HookContext
            {
                Sender = player,
                Player = player,
                Connection = player.Connection.Socket
            };

            var args = new HookArgs.TileSquareReceived
            {
                X = startX,
                Y = startY,
                Size = size,
                readBuffer = buffer.readBuffer
                //                start = num
            };

            HookPoints.TileSquareReceived.Invoke(ref ctx, ref args);

            //            if (args.applied > 0)
            //            {
            //                WorldGen.RangeFrame(startX, startY, startX + (int)size, startY + (int)size);
            //                NetMessage.SendData((int)Packet.TILE_SQUARE, -1, bufferId, String.Empty, (int)size, (float)startX, (float)startY, 0f, 0);
            //            }

            if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                return;

            BitsByte bitsByte10 = 0;
            BitsByte bitsByte11 = 0;
            for (int x = startX; x < startX + (int)size; x++)
            {
                for (int y = startY; y < startY + (int)size; y++)
                {
                    if (Main.tile[x, y] == null)
                    {
                        Main.tile[x, y] = new Tile();
                    }
                    Tile tile = Main.tile[x, y];
                    bool flag7 = tile.active();
                    bitsByte10 = buffer.reader.ReadByte();
                    bitsByte11 = buffer.reader.ReadByte();
                    tile.active(bitsByte10[0]);
                    tile.wall = (bitsByte10[2] ? (byte)1 : (byte)0);
                    bool flag8 = bitsByte10[3];
                    if (Main.netMode != 2)
                    {
                        tile.liquid = (flag8 ? (byte)1 : (byte)0);
                    }
                    tile.wire(bitsByte10[4]);
                    tile.halfBrick(bitsByte10[5]);
                    tile.actuator(bitsByte10[6]);
                    tile.inActive(bitsByte10[7]);
                    tile.wire2(bitsByte11[0]);
                    tile.wire3(bitsByte11[1]);
                    if (bitsByte11[2])
                    {
                        tile.color(buffer.reader.ReadByte());
                    }
                    if (bitsByte11[3])
                    {
                        tile.wallColor(buffer.reader.ReadByte());
                    }
                    if (tile.active())
                    {
                        int type3 = (int)tile.type;
                        tile.type = buffer.reader.ReadUInt16();
                        if (Main.tileFrameImportant[(int)tile.type])
                        {
                            tile.frameX = buffer.reader.ReadInt16();
                            tile.frameY = buffer.reader.ReadInt16();
                        }
                        else
                        {
                            if (!flag7 || (int)tile.type != type3)
                            {
                                tile.frameX = -1;
                                tile.frameY = -1;
                            }
                        }
                        byte b4 = 0;
                        if (bitsByte11[4])
                        {
                            b4 += 1;
                        }
                        if (bitsByte11[5])
                        {
                            b4 += 2;
                        }
                        if (bitsByte11[6])
                        {
                            b4 += 4;
                        }
                        tile.slope(b4);
                    }
                    if (tile.wall > 0)
                    {
                        tile.wall = buffer.reader.ReadByte();
                    }
                    if (flag8)
                    {
                        tile.liquid = buffer.reader.ReadByte();
                        tile.liquidType((int)buffer.reader.ReadByte());
                    }
                }
            }
            WorldGen.RangeFrame(startX, startY, startX + (int)size, startY + (int)size);
            if (Main.netMode == 2)
            {
                NetMessage.SendData((int)Packet.TILE_SQUARE, -1, bufferId, "", (int)size, (float)startX, (float)startY, 0, 0, 0, 0);
                return;
            }
        }

        private static void ProcessLiquidFlow(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];

            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            byte liquid = buffer.reader.ReadByte();
            byte liquidType = buffer.reader.ReadByte();

            if (Main.netMode == 2 && Netplay.spamCheck)
            {
                int centerX = (int)(Main.player[bufferId].position.X + (float)(Main.player[bufferId].width / 2));
                int centerY = (int)(Main.player[bufferId].position.Y + (float)(Main.player[bufferId].height / 2));
                int range = 10;
                int minX = centerX - range;
                int maxX = centerX + range;
                int minY = centerY - range;
                int maxY = centerY + range;
                if (x < minX || x > maxX || y < minY || y > maxY)
                {
                    NetMessage.BootPlayer(bufferId, "Cheating attempt detected: Liquid spam");
                    return;
                }
            }

            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Player = player,
                Sender = player
            };

            var args = new HookArgs.LiquidFlowReceived
            {
                X = x,
                Y = y,
                Amount = liquid,
                Lava = liquidType == 1
            };

            HookPoints.LiquidFlowReceived.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
                return;

            if (ctx.Result == HookResult.IGNORE)
                return;

            if (ctx.Result == HookResult.RECTIFY)
            {
                //                var msg = NewNetMessage.PrepareThreadInstance();
                //                msg.FlowLiquid(x, y);
                //                msg.Send(whoAmI);
                Terraria.NetMessage.SendData((int)Packet.FLOW_LIQUID, bufferId, -1, String.Empty, x, y);
                return;
            }

            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }
            lock (Main.tile[x, y])
            {
                Main.tile[x, y].liquid = liquid;
                Main.tile[x, y].liquidType((int)liquidType);
                if (Main.netMode == 2)
                {
                    WorldGen.SquareTileFrame(x, y, true);
                }
                return;
            }
        }

        private static void ProcessChestOpen(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];

            if (Main.netMode != 2)
            {
                return;
            }
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();

            if (Math.Abs(player.position.X / 16 - x) >= 7 || Math.Abs(player.position.Y / 16 - y) >= 7)
            {
                return;
            }

            int chestIndex = Chest.FindChest(x, y);
            if (chestIndex <= -1 || Chest.UsingChest(chestIndex) != -1)
            {
                return;
            }
            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Player = player,
                Sender = player
            };

            var args = new HookArgs.ChestOpenReceived
            {
                X = x,
                Y = y,
                ChestIndex = chestIndex
            };

            HookPoints.ChestOpenReceived.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
            {
                return;
            }

            if (ctx.Result == HookResult.IGNORE)
            {
                return;
            }

            if (ctx.Result == HookResult.DEFAULT && chestIndex > -1)
            {
                for (int num97 = 0; num97 < 40; num97++)
                {
                    NetMessage.SendData(32, bufferId, -1, "", chestIndex, (float)num97, 0, 0, 0, 0, 0);
                }
                NetMessage.SendData(33, bufferId, -1, "", chestIndex, 0, 0, 0, 0, 0, 0);
                Main.player[bufferId].chest = chestIndex;
                if (Main.myPlayer == bufferId)
                {
                    Main.recBigList = false;
                }
                Recipe.FindRecipes();
                NetMessage.SendData(80, -1, bufferId, "", bufferId, (float)chestIndex, 0, 0, 0, 0, 0);
                if (Main.tile[x, y].frameX >= 36 && Main.tile[x, y].frameX < 72)
                {
                    AchievementsHelper.HandleSpecialEvent(Main.player[bufferId], 16);
                }
            }
        }

        private static void ProcessChest(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];

            byte b7 = buffer.reader.ReadByte();
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            int style = (int)buffer.reader.ReadInt16();

            if (Math.Abs(player.position.X / 16 - x) >= 7 || Math.Abs(player.position.Y / 16 - y) >= 7)
            {
                return;
            }

            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Player = player,
                Sender = player
            };

            var args = new HookArgs.ChestBreakReceived
            {
                X = x,
                Y = y
            };

            HookPoints.ChestBreakReceived.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
            {
                return;
            }

            if (ctx.Result == HookResult.IGNORE)
            {
                return;
            }

            if (ctx.Result == HookResult.RECTIFY)
            {
                NetMessage.SendTileSquare(bufferId, x, y, 3);
                return;
            }

            if (Main.netMode == 2)
            {
                if (b7 == 0)
                {
                    int num105 = WorldGen.PlaceChest(x, y, 21, false, style);
                    if (num105 == -1)
                    {
                        NetMessage.SendData(34, bufferId, -1, "", (int)b7, (float)x, (float)y, (float)style, num105, 0, 0);
                        Item.NewItem(x * 16, y * 16, 32, 32, Chest.chestItemSpawn[style], 1, true, 0, false);
                        return;
                    }
                    NetMessage.SendData(34, -1, -1, "", (int)b7, (float)x, (float)y, (float)style, num105, 0, 0);
                    return;
                }
                else
                {
                    if (b7 == 2)
                    {
                        int num106 = WorldGen.PlaceChest(x, y, 88, false, style);
                        if (num106 == -1)
                        {
                            NetMessage.SendData(34, bufferId, -1, "", (int)b7, (float)x, (float)y, (float)style, num106, 0, 0);
                            Item.NewItem(x * 16, y * 16, 32, 32, Chest.dresserItemSpawn[style], 1, true, 0, false);
                            return;
                        }
                        NetMessage.SendData(34, -1, -1, "", (int)b7, (float)x, (float)y, (float)style, num106, 0, 0);
                        return;
                    }
                    else
                    {
                        Tile tile2 = Main.tile[x, y];
                        if (tile2.type == 21 && b7 == 1)
                        {
                            if (tile2.frameX % 36 != 0)
                            {
                                x--;
                            }
                            if (tile2.frameY % 36 != 0)
                            {
                                y--;
                            }
                            int number = Chest.FindChest(x, y);
                            WorldGen.KillTile(x, y, false, false, false);
                            if (!tile2.active())
                            {
                                NetMessage.SendData(34, -1, -1, "", (int)b7, (float)x, (float)y, 0, number, 0, 0);
                                return;
                            }
                            return;
                        }
                        else
                        {
                            if (tile2.type != 88 || b7 != 3)
                            {
                                return;
                            }
                            x -= (int)(tile2.frameX % 54 / 18);
                            if (tile2.frameY % 36 != 0)
                            {
                                y--;
                            }
                            int number2 = Chest.FindChest(x, y);
                            WorldGen.KillTile(x, y, false, false, false);
                            if (!tile2.active())
                            {
                                NetMessage.SendData(34, -1, -1, "", (int)b7, (float)x, (float)y, 0, number2, 0, 0);
                                return;
                            }
                            return;
                        }
                    }
                }
            }
            else
            {
                int num107 = (int)buffer.reader.ReadInt16();
                if (b7 == 0)
                {
                    if (num107 == -1)
                    {
                        WorldGen.KillTile(x, y, false, false, false);
                        return;
                    }
                    WorldGen.PlaceChestDirect(x, y, 21, style, num107);
                    return;
                }
                else
                {
                    if (b7 != 2)
                    {
                        Chest.DestroyChestDirect(x, y, num107);
                        WorldGen.KillTile(x, y, false, false, false);
                        return;
                    }
                    if (num107 == -1)
                    {
                        WorldGen.KillTile(x, y, false, false, false);
                        return;
                    }
                    WorldGen.PlaceDresserDirect(x, y, 88, style, num107);
                    return;
                }
            }
        }

        private static void ProcessPlayerData(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];

            var isConnection = player == null || player.Connection == null || !player.active;
            if (isConnection)
            {
                //player = new Player();
            }
            player.whoAmI = bufferId;
            player.IPAddress = Netplay.Clients[bufferId].Socket.GetRemoteAddress().GetIdentifier();

            if (bufferId == Main.myPlayer && !Main.ServerSideCharacter)
            {
                return;
            }

            var data = new HookArgs.PlayerDataReceived()
            {
                IsConnecting = isConnection
            };

            data.Parse(buffer.readBuffer, start, length);
            //            Skip(read);
            //
            //            if (data.Hair >= MAX_HAIR_ID)
            //            {
            //                data.Hair = 0;
            //            }
            //
            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Player = player,
                Sender = player,
            };
            //
            HookPoints.PlayerDataReceived.Invoke(ref ctx, ref data);
            //
            if (ctx.CheckForKick())
                return;

            if (!data.NameChecked)
            {
                string error;
                if (!data.CheckName(out error))
                {
                    player.Connection.Kick(error);
                    return;
                }
            }

            string address = player.IPAddress.Split(':')[0];
            //            if (!data.BansChecked)
            //            {
            //                if (Server.Bans.Contains(address) || Server.Bans.Contains(data.Name))
            //                {
            //                    ProgramLog.Admin.Log("Prevented banned user {0} from accessing the server.", data.Name);
            //                    conn.Kick("You are banned from this server.");
            //                    return;
            //                }
            //            }
            //
            //            if (!data.WhitelistChecked && Server.WhitelistEnabled)
            //            {
            //                if (!Server.Whitelist.Contains(address) && !Server.Whitelist.Contains(data.Name))
            //                {
            //                    ProgramLog.Admin.Log("Prevented non whitelisted user {0} from accessing the server.", data.Name);
            //                    conn.Kick("You do not have access to this server.");
            //                    return;
            //                }
            //            }

            data.Apply(player);
            
            if (isConnection)
            {
                if (ctx.Result == HookResult.ASK_PASS)
                {
                    Netplay.Clients[bufferId].State = -2;
                    //                    conn.State = SlotState.PLAYER_AUTH;


                    //                    var msg = NewNetMessage.PrepareThreadInstance();
                    //                    msg.PasswordRequest();
                    //                    conn.Send(msg.Output);
                    NetMessage.SendData((int)Packet.PASSWORD_REQUEST, bufferId);

                    return;
                }
                else // HookResult.DEFAULT
                {
                    // don't allow replacing connections for guests, but do for registered users
                    //                    if (conn.State < SlotState.PLAYING)
                    {
                        var lname = player.Name.ToLower();

                        foreach (var otherPlayer in Main.player)
                        {
                            //                            var otherSlot = Terraria.Netplay.Clients[otherPlayer.whoAmI];
                            if (otherPlayer.Name != null && lname == otherPlayer.Name.ToLower() && otherPlayer.whoAmI != bufferId) // && otherSlot.State >= SlotState.CONNECTED)
                            {
                                player.Kick("A \"" + otherPlayer.Name + "\" is already on this server.");
                                return;
                            }
                        }
                    }

                    //conn.Queue = (int)loginEvent.Priority; // actual queueing done on world request message

                    // and now decide whether to queue the connection
                    //SlotManager.Schedule (conn, (int)loginEvent.Priority);

                    //if (Netplay.Clients[bufferId].State == -2)
                    //{
                    //    //Netplay.Clients[bufferId].State = 1;
                    //    ProgramLog.Error.Log("User password accepted, send world");
                    //}
                    //else
                    //{
                        //Netplay.Clients[bufferId].State = 1;
                        //NetMessage.SendData((int)Packet.CONNECTION_RESPONSE, bufferId, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
                    //}
                }
            }
            Netplay.Clients[bufferId].Name = player.name;
            tdsm.api.Callbacks.VanillaHooks.OnPlayerEntering(player);
            return;

            //            int num6 = (int)buffer.reader.ReadByte();
            //            if (Main.netMode == 2)
            //            {
            //                num6 = bufferId;
            //            }
            //            if (num6 == Main.myPlayer && !Main.ServerSideCharacter)
            //            {
            //                return;
            //            }
            //            Player player = Main.player[num6];
            //            player.whoAmI = num6;
            //            player.skinVariant = (int)buffer.reader.ReadByte();
            //            player.skinVariant = (int)MathHelper.Clamp((float)player.skinVariant, 0, 7);
            //            player.hair = (int)buffer.reader.ReadByte();
            //            if (player.hair >= 134)
            //            {
            //                player.hair = 0;
            //            }
            //            player.name = buffer.reader.ReadString().Trim().Trim();
            //            player.hairDye = buffer.reader.ReadByte();
            //            BitsByte bitsByte = buffer.reader.ReadByte();
            //            for (int num7 = 0; num7 < 8; num7++)
            //            {
            //                player.hideVisual[num7] = bitsByte[num7];
            //            }
            //            bitsByte = buffer.reader.ReadByte();
            //            for (int num8 = 0; num8 < 2; num8++)
            //            {
            //                player.hideVisual[num8 + 8] = bitsByte[num8];
            //            }
            //            player.hideMisc = buffer.reader.ReadByte();
            //            player.hairColor = buffer.reader.ReadRGB();
            //            player.skinColor = buffer.reader.ReadRGB();
            //            player.eyeColor = buffer.reader.ReadRGB();
            //            player.shirtColor = buffer.reader.ReadRGB();
            //            player.underShirtColor = buffer.reader.ReadRGB();
            //            player.pantsColor = buffer.reader.ReadRGB();
            //            player.shoeColor = buffer.reader.ReadRGB();
            //            BitsByte bitsByte2 = buffer.reader.ReadByte();
            //            player.difficulty = 0;
            //            if (bitsByte2[0])
            //            {
            //                Player expr_B25 = player;
            //                expr_B25.difficulty += 1;
            //            }
            //            if (bitsByte2[1])
            //            {
            //                Player expr_B3F = player;
            //                expr_B3F.difficulty += 2;
            //            }
            //            if (player.difficulty > 2)
            //            {
            //                player.difficulty = 2;
            //            }
            //            player.extraAccessory = bitsByte2[2];
            //            if (Main.netMode != 2)
            //            {
            //                return;
            //            }
            //            bool flag = false;
            //            if (Netplay.Clients[bufferId].State < 10)
            //            {
            //                for (int num9 = 0; num9 < 255; num9++)
            //                {
            //                    if (num9 != num6 && player.name == Main.player[num9].name && Netplay.Clients[num9].IsActive)
            //                    {
            //                        flag = true;
            //                    }
            //                }
            //            }
            //            if (flag)
            //            {
            //                NetMessage.SendData(2, bufferId, -1, player.name + " " + Lang.mp[5], 0, 0, 0, 0, 0, 0, 0);
            //                return;
            //            }
            //            if (player.name.Length > Player.nameLen)
            //            {
            //                NetMessage.SendData(2, bufferId, -1, "Name is too long.", 0, 0, 0, 0, 0, 0, 0);
            //                return;
            //            }
            //            if (player.name == "")
            //            {
            //                NetMessage.SendData(2, bufferId, -1, "Empty name.", 0, 0, 0, 0, 0, 0, 0);
            //                return;
            //            }
            //
            //            var ctx = new HookContext
            //                {
            //                    Connection = player.conn,
            //                    Player = player,
            //                    Sender = player,
            //                };
            //
            //            HookPoints.PlayerDataReceived.Invoke (ref ctx, ref data);
            //
            //            if (ctx.CheckForKick ())
            //                return;
            //
            //            if (! data.NameChecked)
            //            {
            //                string error;
            //                if (! data.CheckName (out error))
            //                {
            //                    conn.Kick (error);
            //                    return;
            //                }
            //            }
            //
            //            if (! data.BansChecked)
            //            {
            //                string address = conn.RemoteAddress.Split(':')[0];
            //
            //                if (Server.BanList.containsException (address) || Server.BanList.containsException (data.Name))
            //                {
            //                    ProgramLog.Admin.Log ("Prevented user {0} from accessing the server.", data.Name);
            //                    conn.Kick ("You are banned from this server.");
            //                    return;
            //                }
            //            }
            //
            //            data.Apply (player);
            //
            //            if (ctx.Result == HookResult.ASK_PASS)
            //            {
            //                conn.State = SlotState.PLAYER_AUTH;
            //
            //                var msg = NetMessage.PrepareThreadInstance ();
            //                msg.PasswordRequest ();
            //                conn.Send (msg.Output);
            //
            //                return;
            //            }
            //            else // HookResult.DEFAULT
            //            {
            //                // don't allow replacing connections for guests, but do for registered users
            //                if (conn.State < SlotState.PLAYING)
            //                {
            //                    var lname = player.Name.ToLower();
            //
            //                    foreach (var otherPlayer in Main.players)
            //                    {
            //                        var otherSlot = NetPlay.slots[otherPlayer.whoAmi];
            //                        if (otherPlayer.Name != null && lname == otherPlayer.Name.ToLower() && otherSlot.state >= SlotState.CONNECTED)
            //                        {
            //                            conn.Kick ("A \"" + otherPlayer.Name + "\" is already on this server.");
            //                            return;
            //                        }
            //                    }
            //                }
            //
            //                //conn.Queue = (int)loginEvent.Priority; // actual queueing done on world request message
            //
            //                // and now decide whether to queue the connection
            //                //SlotManager.Schedule (conn, (int)loginEvent.Priority);
            //
            //                //NetMessage.SendData (4, -1, -1, player.Name, whoAmI);
            //            }
            //
            //            Netplay.Clients[bufferId].Name = player.name;
            //            NetMessage.SendData(4, -1, bufferId, player.name, num6, 0, 0, 0, 0, 0, 0);
        }

        private static void ProcessProjectile(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];

            int identity = (int)buffer.reader.ReadInt16();
            Vector2 position = buffer.reader.ReadVector2();
            Vector2 velocity = buffer.reader.ReadVector2();
            float knockBack = buffer.reader.ReadSingle();
            int damage = (int)buffer.reader.ReadInt16();
            int owner = (int)buffer.reader.ReadByte();
            int type = (int)buffer.reader.ReadInt16();
            BitsByte flags = buffer.reader.ReadByte();
            float[] ai = new float[Projectile.maxAI];

            for (int i = 0; i < Projectile.maxAI; i++)
            {
                if (flags[i])
                {
                    ai[i] = buffer.reader.ReadSingle();
                }
                else
                {
                    ai[i] = 0;
                }
            }
            if (Main.netMode == 2)
            {
                owner = bufferId;
                if (Main.projHostile[type])
                {
                    return;
                }
            }
            int index = 1000;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].owner == owner && Main.projectile[i].identity == identity && Main.projectile[i].active)
                {
                    index = i;
                    break;
                }
            }
            if (index == 1000)
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (!Main.projectile[i].active)
                    {
                        index = i;
                        break;
                    }
                }
            }

            var player = Main.player[bufferId];
            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Player = player,
                Sender = player,
            };

            var args = new HookArgs.ProjectileReceived
            {
                Position = position,
                Velocity = velocity,
                Id = identity,
                Owner = bufferId,
                Knockback = knockBack,
                Damage = damage,
                Type = type,
                AI = ai,
                ExistingIndex = index < 1000 ? index : -1
            };

            HookPoints.ProjectileReceived.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.DEFAULT)
            {
                if (index > -1 && index < 1000)
                {
                    Projectile projectile = Main.projectile[index];
                    if (!projectile.active || projectile.type != type)
                    {
                        projectile.SetDefaults(type);
                        if (Main.netMode == 2)
                        {
                            Netplay.Clients[bufferId].SpamProjectile += 1;
                        }
                    }
                    projectile.identity = identity;
                    projectile.position = position;
                    projectile.velocity = velocity;
                    projectile.type = type;
                    projectile.damage = damage;
                    projectile.knockBack = knockBack;
                    projectile.owner = owner;
                    for (int num85 = 0; num85 < Projectile.maxAI; num85++)
                    {
                        projectile.ai[num85] = ai[num85];
                    }
                    projectile.ProjectileFixDesperation();
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(27, -1, bufferId, "", index, 0, 0, 0, 0, 0, 0);
                    }
                }
            }
        }

        private static void ProcessTileBreak(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];

            byte action = buffer.reader.ReadByte();
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            short type = buffer.reader.ReadInt16();
            int style = (int)buffer.reader.ReadByte();
            bool fail = type == 1;

            if (!WorldGen.InWorld(x, y, 3))
            {
                return;
            }

            var player = Main.player[bufferId];

            //TODO implement the old methods
            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Sender = player,
                Player = player,
            };

            var args = new HookArgs.PlayerWorldAlteration
            {
                X = x,
                Y = y,
                Action = action,
                Type = type,
                Style = style
            };

            HookPoints.PlayerWorldAlteration.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
                return;

            if (ctx.Result == HookResult.IGNORE)
                return;

            if (ctx.Result == HookResult.RECTIFY)
            {
                //Terraria.WorldGen.SquareTileFrame (x, y, true);
                NetMessage.SendTileSquare(bufferId, x, y, 1);
                return;
            }

            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }

            if (Main.netMode == 2)
            {
                if (!fail)
                {
                    if (action == 0 || action == 2 || action == 4)
                    {
                        Netplay.Clients[bufferId].SpamDeleteBlock += 1;
                    }
                    if (action == 1 || action == 3)
                    {
                        Netplay.Clients[bufferId].SpamAddBlock += 1;
                    }
                }
                if (!Netplay.Clients[bufferId].TileSections[Netplay.GetSectionX(x), Netplay.GetSectionY(y)])
                {
                    fail = true;
                }
            }
            if (action == 0)
            {
                WorldGen.KillTile(x, y, fail, false, false);
            }
            if (action == 1)
            {
                WorldGen.PlaceTile(x, y, (int)type, false, true, -1, style);
            }
            if (action == 2)
            {
                WorldGen.KillWall(x, y, fail);
            }
            if (action == 3)
            {
                WorldGen.PlaceWall(x, y, (int)type, false);
            }
            if (action == 4)
            {
                WorldGen.KillTile(x, y, fail, false, true);
            }
            if (action == 5)
            {
                WorldGen.PlaceWire(x, y);
            }
            if (action == 6)
            {
                WorldGen.KillWire(x, y);
            }
            if (action == 7)
            {
                WorldGen.PoundTile(x, y);
            }
            if (action == 8)
            {
                WorldGen.PlaceActuator(x, y);
            }
            if (action == 9)
            {
                WorldGen.KillActuator(x, y);
            }
            if (action == 10)
            {
                WorldGen.PlaceWire2(x, y);
            }
            if (action == 11)
            {
                WorldGen.KillWire2(x, y);
            }
            if (action == 12)
            {
                WorldGen.PlaceWire3(x, y);
            }
            if (action == 13)
            {
                WorldGen.KillWire3(x, y);
            }
            if (action == 14)
            {
                WorldGen.SlopeTile(x, y, (int)type);
            }
            if (action == 15)
            {
                Minecart.FrameTrack(x, y, true, false);
            }
            if (Main.netMode != 2)
            {
                return;
            }
            NetMessage.SendData(17, -1, bufferId, "", (int)action, (float)x, (float)y, (float)type, style, 0, 0);
            if (action == 1 && type == 53)
            {
                NetMessage.SendTileSquare(-1, x, y, 1);
            }
        }

        private static void ProcessChat(int bufferId)
        {
            var buffer = NetMessage.buffer[bufferId];

            //Discard 
            buffer.reader.ReadByte();
            buffer.reader.ReadRGB();

            var chatText = buffer.reader.ReadString();

            var player = Main.player[bufferId];
            var color = Color.White;

            if (Main.netMode != 2)
                return;

            var lowered = chatText.ToLower();
            if (lowered == Lang.mp[6] || lowered == Lang.mp[21])
            {
                var players = "";
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active)
                    {
                        if (players.Length > 0)
                            players += ", ";
                        players += Main.player[i].name;
                    }
                }
                NetMessage.SendData(25, bufferId, -1, Lang.mp[7] + " " + players + ".", 255, 255, 240, 20, 0, 0, 0);
                return;
            }
            else if (lowered.StartsWith("/me "))
            {
                NetMessage.SendData(25, -1, -1, "*" + Main.player[bufferId].name + " " + chatText.Substring(4), 255, 200, 100, 0, 0, 0, 0);
                return;
            }
            else if (lowered == Lang.mp[8])
            {
                NetMessage.SendData(25, -1, -1, string.Concat(new object[]
                        {
                            "*",
                            Main.player[bufferId].name,
                            " ",
                            Lang.mp[9],
                            " ",
                            Main.rand.Next(1, 101)
                        }), 255, 255, 240, 20, 0, 0, 0);
                return;
            }
            else if (lowered.StartsWith("/p "))
            {
                int team = Main.player[bufferId].team;
                color = Main.teamColor[team];
                if (team != 0)
                {
                    for (int num74 = 0; num74 < 255; num74++)
                    {
                        if (Main.player[num74].team == team)
                        {
                            NetMessage.SendData(25, num74, -1, chatText.Substring(3), bufferId, (float)color.R, (float)color.G, (float)color.B, 0, 0, 0);
                        }
                    }
                    return;
                }
                NetMessage.SendData(25, bufferId, -1, Lang.mp[10], 255, 255, 240, 20, 0, 0, 0);
                return;
            }
            else
            {
                if (Main.player[bufferId].difficulty == 2)
                    color = Main.hcColor;
                else if (Main.player[bufferId].difficulty == 1)
                    color = Main.mcColor;

                var ctx = new HookContext
                {
                    Connection = player.Connection.Socket,
                    Sender = player,
                    Player = player
                };

                var args = new HookArgs.PlayerChat
                {
                    Message = chatText,
                    Color = color
                };

                HookPoints.PlayerChat.Invoke(ref ctx, ref args);

                if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                    return;

                NetMessage.SendData(25, -1, -1, chatText, bufferId, (float)color.R, (float)color.G, (float)color.B, 0, 0, 0);
                if (Main.dedServ)
                {
                    Tools.WriteLine("<" + Main.player[bufferId].name + "> " + chatText);
                }
            }
        }
    }
}
