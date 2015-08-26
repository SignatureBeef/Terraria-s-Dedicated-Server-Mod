using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Text;
using tdsm.api;
using tdsm.api.Plugin;
using tdsm.core.Logging;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.Out
{
    public partial class NewNetMessage
    {
        public static NewNetMessage PrepareThreadInstance() // binary compat
        {
            return PrepareThreadInstance(65535);
        }

        public static NewNetMessage PrepareThreadInstance(int size)
        {
            if (threadInstance == null || threadInstance.buf.Length < size)
            {
                threadInstance = new NewNetMessage(size);
            }
            else
            {
                threadInstance.sink.Position = 0;
                threadInstance.lenAt = 0;
            }
            return threadInstance;
        }

        public byte[] Output
        {
            get
            {
                var copy = new byte[sink.Position];
                Array.Copy(buf, copy, sink.Position);
                return copy;
            }
        }

        public ArraySegment<byte> Segment
        {
            get
            {
                return new ArraySegment<byte>(buf, 0, (int)sink.Position);
            }
        }

        public int Written
        {
            get
            {
                return (int)sink.Position;
            }
        }

        public NewNetMessage(int bufSize = 65535)
        {
            buf = new byte[bufSize];
            sink = new SealedMemoryStream(buf);
            bin = new SealedBinaryWriter(sink);
        }

        public void Clear()
        {
            lenAt = 0;
            sink.Position = 0;
        }

        public static void BootPlayer(int plr, string msg)
        {
            Terraria.Netplay.Clients[plr].Kick(msg);
        }

        public static int SendData(Packet packet, int remoteClient = -1, int ignoreClient = -1, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0)
        {
            return SendData((int)packet, remoteClient, ignoreClient, text, number, number2, number3, number4, number5);
        }

        public static int SendData(int packetId, int remoteClient = -1, int ignoreClient = -1, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0)
        {
            if (!Netplay.anyClients) return 0;

            try
            {
                var msg = PrepareThreadInstance();

                switch (packetId)
                {
                    case (int)Packet.CONNECTION_REQUEST:
                        msg.ConnectionRequest(Main.versionNumber);
                        break;

                    case (int)Packet.DISCONNECT:
                        msg.Disconnect(text);
                        break;

                    case (int)Packet.CONNECTION_RESPONSE:
                        msg.ConnectionResponse(remoteClient);
                        break;

                    case (int)Packet.PLAYER_DATA:
                        msg.PlayerData(number);
                        break;

                    case (int)Packet.INVENTORY_DATA:
                        msg.InventoryData(number, (byte)number2, (int)number3);
                        break;

                    case (int)Packet.WORLD_REQUEST:
                        msg.WorldRequest();
                        break;

                    case (int)Packet.WORLD_DATA:
                        msg.WorldData();
                        break;

                    case (int)Packet.REQUEST_TILE_BLOCK:
                        msg.RequestTileBlock();
                        break;

                    case (int)Packet.SEND_TILE_LOADING:
                        msg.SendTileLoading(number, text);
                        break;

                    case (int)Packet.SEND_TILE_ROW:
                        msg.SendTileRowCompressed(number, (int)number2, (short)number3, (short)number4, true);
                        break;

                    case (int)Packet.SEND_TILE_CONFIRM:
                        msg.SendTileConfirm(number, (int)number2, (int)number3, (int)number4);
                        break;

                    case (int)Packet.RECEIVING_PLAYER_JOINED:
                        msg.ReceivingPlayerJoined(number);
                        break;

                    case (int)Packet.PLAYER_STATE_UPDATE:
                        msg.PlayerStateUpdate(number);
                        break;

                    case (int)Packet.SYNCH_BEGIN:
                        msg.SynchBegin(number, (int)number2);
                        break;

                    case (int)Packet.UPDATE_PLAYERS:
                        msg.UpdatePlayers();
                        break;

                    case (int)Packet.PLAYER_HEALTH_UPDATE:
                        msg.PlayerHealthUpdate(number);
                        break;

                    case (int)Packet.TILE_BREAK:
                        msg.TileBreak(number, (int)number2, (int)number3, (int)number4, (int)number5);
                        break;

                    case (int)Packet.TIME_SUN_MOON_UPDATE:
                        msg.TimeSunMoonUpdate();
                        break;

                    case (int)Packet.DOOR_UPDATE:
                        msg.DoorUpdate(number, (int)number2, (int)number3, (int)number4);
                        break;

                    case (int)Packet.TILE_SQUARE:
                        msg.TileSquare(number, (int)number2, (int)number3);
                        break;

                    case (int)Packet.ITEM_INFO:
                        msg.ItemInfo(number, (byte)number2);
                        break;

                    case (int)Packet.ITEM_OWNER_INFO:
                        msg.ItemOwnerInfo(number);
                        break;

                    case (int)Packet.NPC_INFO:
                        msg.NPCInfo(number);
                        break;

                    case (int)Packet.STRIKE_NPC:
                        msg.StrikeNPC(number, (int)number2);
                        break;

                    case (int)Packet.PLAYER_CHAT:
                        msg.PlayerChat(number, text, (byte)number2, (byte)number3, (byte)number4);
                        break;

                    case (int)Packet.STRIKE_PLAYER:
                        msg.StrikePlayer(number, text, (int)number2, (int)number3, (int)number4);
                        break;

                    case (int)Packet.PROJECTILE:
                        msg.Projectile(Main.projectile[number]);
                        break;

                    case (int)Packet.DAMAGE_NPC:
                        msg.DamageNPC(number, (int)number2, number3, (int)number4);
                        break;

                    case (int)Packet.KILL_PROJECTILE:
                        msg.KillProjectile(number, (int)number2);
                        break;

                    case (int)Packet.PLAYER_PVP_CHANGE:
                        msg.PlayerPVPChange(number);
                        break;

                    case (int)Packet.OPEN_CHEST:
                        msg.OpenChest();
                        break;

                    case (int)Packet.CHEST_ITEM:
                        msg.ChestItem(number, (int)number2);
                        break;

                    case (int)Packet.PLAYER_CHEST_UPDATE:
                        msg.PlayerChestUpdate(number, text);
                        break;

                    case (int)Packet.KILL_TILE:
                        msg.KillTile(number, number2, number3, number4, number5);
                        break;

                    case (int)Packet.HEAL_PLAYER:
                        msg.HealPlayer(number, (int)number2);
                        break;

                    case (int)Packet.ENTER_ZONE:
                        msg.EnterZone(number);
                        break;

                    case (int)Packet.PASSWORD_REQUEST:
                        msg.PasswordRequest();
                        break;

                    case (int)Packet.PASSWORD_RESPONSE:
                        msg.PasswordResponse();
                        break;

                    case (int)Packet.ITEM_OWNER_UPDATE:
                        msg.ItemOwnerUpdate(number);
                        break;

                    case (int)Packet.NPC_TALK:
                        msg.NPCTalk(number);
                        break;

                    case (int)Packet.PLAYER_BALLSWING:
                        msg.PlayerBallswing(number);
                        break;

                    case (int)Packet.PLAYER_MANA_UPDATE:
                        msg.PlayerManaUpdate(number);
                        break;

                    case (int)Packet.PLAYER_USE_MANA_UPDATE:
                        msg.PlayerUseManaUpdate(number, (int)number2);
                        break;

                    case (int)Packet.KILL_PLAYER_PVP:
                        msg.KillPlayerPVP(number, text, (int)number2, (int)number3, (int)number4);
                        break;

                    case (int)Packet.PLAYER_JOIN_PARTY:
                        msg.PlayerJoinParty(number);
                        break;

                    case (int)Packet.READ_SIGN:
                        msg.ReadSign(number, (int)number2);
                        break;

                    case (int)Packet.WRITE_SIGN:
                        msg.WriteSign(number);
                        break;

                    case (int)Packet.FLOW_LIQUID:
                        msg.FlowLiquid(number, (int)number2);
                        break;

                    case (int)Packet.SEND_SPAWN:
                        msg.SendSpawn();
                        break;

                    case (int)Packet.PLAYER_BUFFS:
                        msg.PlayerBuffs(number);
                        break;

                    case (int)Packet.SUMMON_SKELETRON:
                        msg.SummonSkeletron((byte)number, (byte)number2);
                        break;

                    case (int)Packet.CHEST_UNLOCK:
                        msg.ChestUnlock(number, (int)number2, (int)number3, (int)number4);
                        break;

                    case (int)Packet.NPC_ADD_BUFF:
                        msg.NPCAddBuff(number, (int)number2, (int)number3);
                        break;

                    case (int)Packet.NPC_BUFFS:
                        msg.NPCBuffs(number);
                        break;

                    case (int)Packet.PLAYER_ADD_BUFF:
                        msg.PlayerAddBuff(number, (int)number2, (int)number3);
                        break;

                    case (int)Packet.NPC_NAME:
                        msg.NPCName(number, Main.npc[number].displayName);
                        break;

                    case (int)Packet.WORLD_BALANCE:
                        msg.WorldBalance(WorldGen.tGood, WorldGen.tEvil, WorldGen.tBlood);
                        break;

                    case (int)Packet.NPC_HOME:
                        msg.NPCHome(number, (int)number2, (int)number3, number4 == 1);
                        break;

                    case (int)Packet.SPAWN_NPCS:
                        //msg.SpawnNPC(number, (int)number2);
                        break;

                    case (int)Packet.PLAYER_DODGE:
                        msg.PlayerDodge(number, number2);
                        break;

                    case (int)Packet.PAINT_TILE:
                        break;

                    case (int)Packet.PAINT_WALL:
                        msg.PaintWall(number, number2, number3);
                        break;

                    case (int)Packet.TELEPORT:
                        msg.Teleport(number, number2, number3, number4, number5);
                        break;

                    case (int)Packet.PACKET_66:
                        break;

                    case (int)Packet.PACKET_67:
                        break;

                    case (int)Packet.PACKET_68:
                        break;

                    case (int)Packet.CHEST_NAME_UPDATE:
                        msg.Packet69(text, number, number2, number3, number4, number5);
                        break;

                    case (int)Packet.CATCH_NPC:
                        msg.CatchNPC(number, number2);
                        break;

                    case (int)Packet.RELEASE_NPC:
                        msg.ReleaseNPC(number, number2, number3, number4);
                        break;

                    case (int)Packet.TRAVEL_SHOP:
                        msg.TravelShop();
                        break;

                    case (int)Packet.TELEPORTATION_POTION:
                        break;

                    case (int)Packet.ANGLER_QUEST:
                        msg.AnglerQuest(text);
                        break;

                    case (int)Packet.ANGLER_FINISH_REGISTER:
                        break;

                    case (int)Packet.ANGLER_QUESTS_FINISHED:
                        msg.AngerQuestsFinished(number);
                        break;

                    case (int)Packet.HIT_SWITCH:
                        msg.HitSwitch((short)number, (short)number2);
                        break;

                    default:
                        {

                            ProgramLog.Log("Unknown send packet: {0}", packetId);
                            ////Unknown packet :3
                            //var ctx = new HookContext()
                            //{

                            //};

                            //var args = new HookArgs.UnkownSendPacket()
                            //{
                            //    Message = msg,
                            //    PacketId = packetId,
                            //    RemoteClient = remoteClient,
                            //    IgnoreClient = ignoreClient,
                            //    Text = text,
                            //    Number = number,
                            //    Number2 = number2,
                            //    Number3 = number3,
                            //    Number4 = number4,
                            //    Number5 = number5
                            //};

                            //HookPoints.UnkownSendPacket.Invoke(ref ctx, ref args);

                            ///* Update Locals */
                            //msg = args.Message;
                            //remoteClient = args.RemoteClient;
                            //ignoreClient = args.IgnoreClient;

                            //if (ctx.Result != HookResult.IGNORE)
                            //    return 0;
                            //else
                            break;
                        }
                }

                //var bytes = msg.Output;
                if (remoteClient == -1)
                {
                    msg.BroadcastExcept(ignoreClient);
                    //					for (int num11 = 0; num11 < 256; num11++)
                    //					{
                    //						if (num11 != ignoreClient && Netplay.slots[num11].state >= SlotState.PLAYING && Netplay.slots[num11].Connected)
                    //						{
                    //							NewNetMessage.buffer[num11].spamCount++;
                    //							Netplay.slots[num11].Send (bytes);
                    //						}
                    //					}

                }
                else if ((Terraria.Netplay.Clients[remoteClient] as ServerSlot).Connected)
                {
                    msg.Send(remoteClient);
                    //NewNetMessage.buffer[remoteClient].spamCount++;
                    //Netplay.slots[remoteClient].Send (bytes);
                }
                return msg.Written;
            }
            catch (Exception e)
            {
                ProgramLog.Log("SendData error: {0}", e);
            }
            return 0;
        }

        public static void SendTileSquare(int whoAmi, int tileX, int tileY, int size)
        {
            int num = (size - 1) / 2;
            float x = tileX - num;
            float y = tileY - num;
            NewNetMessage.SendData(20, whoAmi, -1, System.String.Empty, size, x, y, 0f);
        }

        public static void SendTileSquare(int whoAmi, int x, int y, int size, bool centered)
        {
            if (centered)
                SendTileSquare(whoAmi, x, y, size);
            else
                NewNetMessage.SendData(20, whoAmi, -1, System.String.Empty, size, x, y, 0f);
        }

        public static void SendSection(int whoAmi, int sectionX, int sectionY)
        {
            if (sectionX >= 0 && sectionY >= 0 && sectionX < Main.maxSectionsX && sectionY < Main.maxSectionsY)
            {
                Terraria.Netplay.Clients[whoAmi].TileSections[sectionX, sectionY] = true;
                try
                {
                    (Terraria.Netplay.Clients[whoAmi] as ServerSlot).conn.SendSection(sectionX, sectionY);
                }
                catch (NullReferenceException) { }
            }
        }

        public static void SendSection(int whoAmi, int sectionX, int sectionY, bool skipSent = false)
        {
            if (sectionX >= 0 && sectionY >= 0 && sectionX < Main.maxSectionsX && sectionY < Main.maxSectionsY)
            {
                if (!skipSent || !Terraria.Netplay.Clients[whoAmi].TileSections[sectionX, sectionY])
                {
                    Terraria.Netplay.Clients[whoAmi].TileSections[sectionX, sectionY] = true;
                    int number = sectionX * 200;
                    int num = sectionY * 150;
                    int num2 = 150;
                    for (int i = num; i < num + 150; i += num2)
                    {
                        NewNetMessage.SendData(10, whoAmi, -1, "", number, (float)i, 200f, (float)num2, 0);
                    }
                    for (int j = 0; j < 200; j++)
                    {
                        if (Main.npc[j].active && Main.npc[j].townNPC)
                        {
                            int sectionX2 = Netplay.GetSectionX((int)(Main.npc[j].position.X / 16f));
                            int sectionY2 = Netplay.GetSectionY((int)(Main.npc[j].position.Y / 16f));
                            if (sectionX2 == sectionX && sectionY2 == sectionY)
                            {
                                NewNetMessage.SendData(23, whoAmi, -1, "", j, 0f, 0f, 0f, 0);
                            }
                        }
                    }
                }
            }
        }

        public static void Broadcast(byte[] bytes)
        {
            //ProgramLog.Log ("Broadcast, {0} {1}", Netplay.slots[0].state, Netplay.slots[0].Connected);
            for (int k = 0; k < 255; k++)
            {
                var slot = Terraria.Netplay.Clients[k] as ServerSlot;
                if (slot.state >= SlotState.PLAYING && slot.Connected)
                {
                    slot.Send(bytes);
                }
            }
        }

        public static void BroadcastExcept(byte[] bytes, int i)
        {
            //ProgramLog.Log ("BroadcastExcept({2}), {0} {1}", Netplay.slots[0].state, Netplay.slots[0].Connected, i);
            for (int k = 0; k < 255; k++)
            {
                var slot = Terraria.Netplay.Clients[k] as ServerSlot;
                if (slot.state >= SlotState.PLAYING && slot.Connected && k != i)
                {
                    slot.Send(bytes);
                }
            }
        }

        public void Broadcast()
        {
            // this alt version copies the message to each player's tx buffer
            //			for (int k = 0; k < 255; k++)
            //			{
            //				if (Netplay.slots[k].state >= SlotState.PLAYING && Netplay.slots[k].Connected)
            //				{
            //					NewNetMessage.buffer[k].spamCount++;
            //					Send (k);
            //				}
            //			}

            Broadcast(Output);
        }

        public void BroadcastExcept(int i)
        {
            // this alt version copies the message to each player's tx buffer
            //			for (int k = 0; k < 255; k++)
            //			{
            //				if (Netplay.slots[k].state >= SlotState.PLAYING && Netplay.slots[k].Connected && k != i)
            //				{
            //					NewNetMessage.buffer[k].spamCount++;
            //					Send (k);
            //				}
            //			}

            BroadcastExcept(Output, i);
        }

        public void Send(int i)
        {
            var conn = (Terraria.Netplay.Clients[i] as ServerSlot).conn;
            if (conn != null)
                conn.CopyAndSend(Segment);
        }

        public void Send(ClientConnection conn)
        {
            conn.CopyAndSend(Segment);
        }

        public static void OnPlayerJoined(int plr)
        {
            var player = Main.player[plr];

            var ctx = new HookContext
            {
                Connection = player.Connection,
                Player = player,
                Sender = player,
            };

            var args = new HookArgs.PlayerEnteringGame
            {
                Slot = plr,
            };

            HookPoints.PlayerEnteringGame.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
            {
                return;
            }

            var msg = NewNetMessage.PrepareThreadInstance();
            if (ctx.Result == HookResult.DEFAULT)
            {
                var motd = (Main.motd ?? System.String.Empty).Split('@'); //"My greeting from properties".Split('@');
                for (int i = 0; i < motd.Length; i++)
                {
                    if (motd[i] != null && motd[i].Trim().Length > 0)
                    {
                        msg.PlayerChat(255, motd[i], 0, 0, 255);
                    }
                }

                string list = System.String.Empty;
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active)
                    {
                        if (list == System.String.Empty)
                            list += Main.player[i].name;
                        else
                            list = list + ", " + Main.player[i].name;
                    }
                }

                msg.PlayerChat(255, "Current players: " + list + ".", 255, 240, 20);
                msg.Send(plr); // send these before the login event, so messages from plugins come after
            }

            var slot = Terraria.Netplay.Clients[plr] as ServerSlot;

            slot.announced = true;

            // to player
            msg.Clear();
            msg.SyncAllNPCHomes();
            msg.SendSyncOthersForPlayer(plr);

            ProgramLog.Log("{0} @ {1}: ENTER {2}", slot.remoteAddress, plr, player.name);

            //if (player.HasHackedData())
            //{
            //    player.Kick("No Hacked Health or Mana is allowed.");
            //    return;
            //}

            // to other players
            msg.Clear();
            msg.PlayerChat(255, player.name + " has joined.", 255, 240, 20);
            msg.ReceivingPlayerJoined(plr);
            msg.SendSyncPlayerForOthers(plr); // broadcasts the preceding message too

            var args2 = new HookArgs.PlayerEnteredGame
            {
                Slot = plr,
            };

            ctx.SetResult(HookResult.DEFAULT, false);
            HookPoints.PlayerEnteredGame.Invoke(ref ctx, ref args2);

            if (ctx.CheckForKick())
            {
                return;
            }
        }

        public static void OnPlayerLeft(Player player, ServerSlot slot, bool announced)
        {
            player.active = false;

            if (announced)
            {
                ProgramLog.Log("{0} @ {1}: LEAVE {2}", slot.remoteAddress, slot.Id, player.name);

                var msg = NewNetMessage.PrepareThreadInstance();

                msg.SynchBegin(player.whoAmI, 0 /*inactive*/);

                if (player.DisconnectReason != null)
                    msg.PlayerChat(255, string.Concat(player.Name, " disconnected (", player.DisconnectReason, ")."), 255, 165, 0);
                else
                    msg.PlayerChat(255, string.Concat(player.name, " has left."), 255, 240, 20);

                msg.BroadcastExcept(player.whoAmI);
            }

            var ctx = new HookContext
            {
                Player = player,
                Sender = player,
            };

            var args = new HookArgs.PlayerLeftGame
            {
                Slot = slot.Id
            };

            HookPoints.PlayerLeftGame.Invoke(ref ctx, ref args);
        }

        [ThreadStatic]
        static bool useLiquidUpdateBuffer;

        [ThreadStatic]
        static bool disableLiquidUpdates;

        // only meant to be used from the update thread
        internal static bool UseLiquidUpdateBuffer
        {
            get { return useLiquidUpdateBuffer; }
            set { useLiquidUpdateBuffer = value; }
        }

        // only meant to be used from the update thread
        internal static bool DisableLiquidUpdates
        {
            get { return disableLiquidUpdates; }
            set { disableLiquidUpdates = value; }
        }

        public static void SendWater(int x, int y)
        {
            if (disableLiquidUpdates) return;

            if (useLiquidUpdateBuffer)
            {
                //LiquidUpdateBuffer.QueueUpdate(x, y);
                LiquidBuffer.AddBuffer(x, y);
                return;
            }

            byte[] bytes = null;

            for (int i = 0; i < 255; i++)
            {
                var slot = Terraria.Netplay.Clients[i] as ServerSlot;
                if (slot.state >= SlotState.PLAYING && slot.Connected)
                {
                    int X = x / 200;
                    int Y = y / 150;
                    if (X < (Main.maxTilesX / 200) && Y < (Main.maxTilesY / 150))
                    {
                        if (Terraria.Netplay.Clients[i].TileSections[X, Y])
                        {
                            if (bytes == null)
                            {
                                var msg = NewNetMessage.PrepareThreadInstance();
                                msg.FlowLiquid(x, y);
                                bytes = msg.Output;
                            }
                            slot.Send(bytes);
                        }
                    }
                    else
                    {
                        ProgramLog.Log("Water Index out of Bounds:");
                        ProgramLog.Log(string.Format("X: {0} Y: {1}, Axis: {2}, {3}", X, Y, Main.maxTilesX, Main.maxTilesY));
                    }
                }
            }
        }

        public void BuildPlayerUpdate(int i)
        {
            var player = Main.player[i];

            SynchBegin(i, 1); //active players only
            PlayerStateUpdate(i);
            PlayerHealthUpdate(i);
            PlayerPVPChange(i);
            PlayerJoinParty(i);
            PlayerManaUpdate(i);
            PlayerBuffs(i);
            PlayerData(i);

            for (int k = 0; k < 59 /*bar only*/; k++)
            {
                InventoryData(i, k, player.inventory[k].prefix);
            }

            for (int k = 0; k < 11; k++)
            {
                InventoryData(i, k + 59, player.armor[k].prefix);
            }
        }

        public void SyncNPCHome(int npcIndex)
        {
            var npc = Main.npc[npcIndex];
            NPCHome(npcIndex, npc.homeTileX, npc.homeTileY, npc.homeless);
        }

        public void SyncAllNPCHomes()
        {
            for (int npcIndex = 0; npcIndex < Main.npc.Length; npcIndex++)
            {
                var npc = Main.npc[npcIndex];
                if (npc.active && npc.townNPC && NPC.TypeToNum(npc.type) != -1)
                    NPCHome(npcIndex, npc.homeTileX, npc.homeTileY, npc.homeless);
            }
        }

        public static void SyncNPCHomes()
        {
            var msg = NewNetMessage.PrepareThreadInstance();
            msg.SyncAllNPCHomes();
            msg.Broadcast();
        }

        public static void SyncPlayers() /* always sends all updates to all players */
        {
            var msg = NewNetMessage.PrepareThreadInstance();

            for (int i = 0; i < 255; i++)
            {
                if (Terraria.Netplay.Clients[i].State() == SlotState.PLAYING)
                {
                    msg.Clear();
                    msg.BuildPlayerUpdate(i);
                    msg.BroadcastExcept(i);
                }
            }

            msg.Clear();

            for (int i = 0; i < 255; i++)
                if (Terraria.Netplay.Clients[i].State() != SlotState.PLAYING)
                    msg.SynchBegin(i, 0);

            msg.Broadcast();
        }

        public void SendSyncOthersForPlayer(int i)
        {
            for (int k = 0; k < 255; k++)
            {
                if (Terraria.Netplay.Clients[k].State() == SlotState.PLAYING && i != k)
                    BuildPlayerUpdate(k);
                else if (i != k)
                    SynchBegin(k, 0);

                if (Written >= 4096)
                {
                    Send(i);
                    Clear();
                }
            }

            if (Written > 0) Send(i);
        }

        public void SendSyncPlayerForOthers(int i)
        {
            // send info about this player to others
            BuildPlayerUpdate(i);
            BroadcastExcept(i);
        }

        //
        // PRIVATES
        //

        private readonly SealedMemoryStream sink;
        private readonly SealedBinaryWriter bin;
        private readonly byte[] buf;
        private int lenAt;

        [ThreadStatic]
        private static NewNetMessage threadInstance;

        sealed class SealedMemoryStream : System.IO.MemoryStream
        {
            public SealedMemoryStream(byte[] buf) : base(buf) { }
        }

        sealed class SealedBinaryWriter : System.IO.BinaryWriter
        {
            public SealedBinaryWriter(Stream stream) : base(stream, Encoding.UTF8) { }
        }

        public void Begin()
        {
            lenAt = (int)sink.Position;
            sink.Position += 2;
        }

        public void Begin(int id)
        {
            lenAt = (int)sink.Position;
            sink.Position += 2;
            sink.WriteByte((byte)id);
        }

        public void Begin(Packet id)
        {
            Begin((int)id);
        }

        public void End()
        {
            var pos = sink.Position;
            sink.Position = lenAt;
            bin.Write((short)(pos - lenAt));
            sink.Position = pos;
        }

        //public void Header(Packet id, int length)
        //{
        //    Header((int)id, length);
        //}

        //public void Header(int id, int length)
        //{
        //    bin.Write((short)length);
        //    sink.WriteByte((byte)id);
        //}

        public void Byte(byte data)
        {
            sink.WriteByte(data);
        }

        public void SByte(sbyte data)
        {
            //sink.Write(BitConverter.GetBytes(data), 0, 1);
            bin.Write(data);
        }

        public void Byte(int data)
        {
            sink.WriteByte((byte)data);
        }

        public void Byte(bool data)
        {
            sink.WriteByte((byte)(data ? 1 : 0));
        }

        public void Byte(byte[] data, int length)
        {
            sink.Write(data, 0, length);
        }

        public void Color(Color data)
        {
            sink.WriteByte(data.R);
            sink.WriteByte(data.G);
            sink.WriteByte(data.B);
        }

        public void Short(short data)
        {
            //sink.Write (BitConverter.GetBytes(data), 0, 2);
            bin.Write(data);
        }

        public void Short(int data)
        {
            Short((short)data);
        }

        public void UShort(ushort data)
        {
            bin.Write(data);
        }

        public void Bool(bool data)
        {
            bin.Write(data);
        }

        public void Int(int data)
        {
            //sink.Write (BitConverter.GetBytes(data), 0, 4);
            bin.Write(data);
        }

        public void Int(double data)
        {
            Int((int)data);
        }

#if UNSAFE
		public unsafe void FloatUnsafe(float data)
		{
			var bytes = (byte*)&data;
			sink.WriteByte(bytes[0]);
			sink.WriteByte(bytes[1]);
			sink.WriteByte(bytes[2]);
			sink.WriteByte(bytes[3]);
		}

		public void Float(float data)
		{
			if (BitConverter.IsLittleEndian)
				FloatUnsafe(data);
			else
				sink.Write(BitConverter.GetBytes(data), 0, 4);
		}
#else
        public void Float(float data)
        {
            //sink.Write(BitConverter.GetBytes(data), 0, 4);
            bin.Write(data);
        }
#endif

        public void String(string data, bool newLineOverride = false, bool useUTF = false)
        {
            //if (!useUTF)
            //{
            //    foreach (char c in data)
            //    {
            //        if ((c < 32 || c > 126) && !newLineOverride)
            //            sink.WriteByte((byte)'?');
            //        else
            //            sink.WriteByte((byte)c);
            //    }
            //}
            //else
            //{
            //    byte[] utf = System.Text.Encoding.UTF8.GetBytes(data);
            //    sink.Write(utf, 0, utf.Length);
            //}
            bin.Write(data);


            //uint num = (uint)data.Length;

            //while (num >= 128U)
            //{
            //    sink.WriteByte((byte)(num | 128U));
            //    num >>= 7;
            //}

            //sink.WriteByte((byte)num);

            //byte[] utf = System.Text.Encoding.Default.GetBytes(data);
            //sink.Write(utf, 0, utf.Length);
        }
    }
}
