using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;
using Microsoft.Xna.Framework;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class TileSquare : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.TILE_SQUARE; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];
            
            short size = buffer.reader.ReadInt16();
            int startX = (int)buffer.reader.ReadInt16();
            int startY = (int)buffer.reader.ReadInt16();
            if (!WorldGen.InWorld(startX, startY, 3))
            {
                return true;
            }
            
            var ctx = new HookContext
            {
                Sender = player,
                Player = player,
                Connection = player.Connection.Socket
            };
            
            var args = new TDSMHookArgs.TileSquareReceived
            {
                X = startX,
                Y = startY,
                Size = size,
                ReadBuffer = buffer.readBuffer
                //                start = num
            };
            
            TDSMHookPoints.TileSquareReceived.Invoke(ref ctx, ref args);
            
            //            if (args.applied > 0)
            //            {
            //                WorldGen.RangeFrame(startX, startY, startX + (int)size, startY + (int)size);
            //                NetMessage.SendData((int)Packet.TILE_SQUARE, -1, bufferId, String.Empty, (int)size, (float)startX, (float)startY, 0f, 0);
            //            }
            
            if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                return true;
            
            BitsByte bitsByte10 = 0;
            BitsByte bitsByte11 = 0;
            for (int x = startX; x < startX + (int)size; x++)
            {
                for (int y = startY; y < startY + (int)size; y++)
                {
                    if (Main.tile[x, y] == null)
                    {
                        Main.tile[x, y] = new OTA.Memory.MemTile();
                    }
                    var tile = Main.tile[x, y];
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
            }

            return true;
        }
    }
}

