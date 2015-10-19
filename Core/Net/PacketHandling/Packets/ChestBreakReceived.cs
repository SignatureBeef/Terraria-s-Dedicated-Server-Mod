using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;
using Microsoft.Xna.Framework;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class ChestBreakReceived : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.CHEST; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];
            
            byte b7 = buffer.reader.ReadByte();
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            int type = (int)buffer.reader.ReadInt16();
            
            if (Math.Abs(player.position.X / 16 - x) >= 7 || Math.Abs(player.position.Y / 16 - y) >= 7)
            {
                return true;
            }
            
            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Player = player,
                Sender = player
            };
            
            var args = new TDSMHookArgs.ChestBreakReceived
            {
                X = x,
                Y = y
            };
            
            TDSMHookPoints.ChestBreakReceived.Invoke(ref ctx, ref args);
            
            if (ctx.CheckForKick())
                return true;
            
            if (ctx.Result == HookResult.IGNORE)
                return true;
            
            if (ctx.Result == HookResult.RECTIFY)
            {
                NetMessage.SendTileSquare(bufferId, x, y, 3);
                return true;
            }
            
            if (Main.netMode == 2)
            {
                if (b7 == 0)
                {
                    int num107 = WorldGen.PlaceChest(x, y, 21, false, type);
                    if (num107 == -1)
                    {
                        NetMessage.SendData(34, bufferId, -1, "", (int)b7, (float)x, (float)y, (float)type, num107, 0, 0);
                        Item.NewItem(x * 16, y * 16, 32, 32, Chest.chestItemSpawn[type], 1, true, 0, false, false);
                        return true;
                    }
                    NetMessage.SendData(34, -1, -1, "", (int)b7, (float)x, (float)y, (float)type, num107, 0, 0);
                    return true;
                }
                else if (b7 == 2)
                {
                    int num108 = WorldGen.PlaceChest(x, y, 88, false, type);
                    if (num108 == -1)
                    {
                        NetMessage.SendData(34, bufferId, -1, "", (int)b7, (float)x, (float)y, (float)type, num108, 0, 0);
                        Item.NewItem(x * 16, y * 16, 32, 32, Chest.dresserItemSpawn[type], 1, true, 0, false, false);
                        return true;
                    }
                    NetMessage.SendData(34, -1, -1, "", (int)b7, (float)x, (float)y, (float)type, num108, 0, 0);
                    return true;
                }
                else
                {
                    var tile2 = Main.tile[x, y];
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
                            NetMessage.SendData(34, -1, -1, "", (int)b7, (float)x, (float)y, 0f, number, 0, 0);
                            return true;
                        }
                        return true;
                    }
                    else
                    {
                        if (tile2.type != 88 || b7 != 3)
                        {
                            return true;
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
                            NetMessage.SendData(34, -1, -1, "", (int)b7, (float)x, (float)y, 0f, number2, 0, 0);
                            return true;
                        }
                        return true;
                    }
                }
            }
            else
            {
                int id = (int)buffer.reader.ReadInt16();
                if (b7 == 0)
                {
                    if (id == -1)
                    {
                        WorldGen.KillTile(x, y, false, false, false);
                        return true;
                    }
                    WorldGen.PlaceChestDirect(x, y, 21, type, id);
                    return true;
                }
                else
                {
                    if (b7 != 2)
                    {
                        Chest.DestroyChestDirect(x, y, id);
                        WorldGen.KillTile(x, y, false, false, false);
                        return true;
                    }
                    if (id == -1)
                    {
                        WorldGen.KillTile(x, y, false, false, false);
                        return true;
                    }
                    WorldGen.PlaceDresserDirect(x, y, 88, type, id);
                    return true;
                }
            }
            return true;
        }
    }
}

