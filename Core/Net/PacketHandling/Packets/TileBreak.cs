using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class TileBreak : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.TILE_BREAK; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];

            ActionType action = (ActionType)buffer.reader.ReadByte();
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            short type = buffer.reader.ReadInt16();
            int style = (int)buffer.reader.ReadByte();
            bool fail = type == 1;

            if (!WorldGen.InWorld(x, y, 3))
            {
                return true;
            }

            var player = Main.player[bufferId];

            //TODO implement the old methods
            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Sender = player,
                Player = player,
            };

            var args = new TDSMHookArgs.PlayerWorldAlteration
            {
                X = x,
                Y = y,
                Action = action,
                Type = type,
                Style = style
            };

            TDSMHookPoints.PlayerWorldAlteration.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
                return true;

            if (ctx.Result == HookResult.IGNORE)
                return true;

            if (ctx.Result == HookResult.RECTIFY)
            {
                //Terraria.WorldGen.SquareTileFrame (x, y, true);
                NetMessage.SendTileSquare(bufferId, x, y, 1);
                return true;
            }

            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new OTA.Memory.MemTile();
            }

            if (Main.netMode == 2)
            {
                if (!fail)
                {
                    if (action == ActionType.KillTile || action == ActionType.KillWall || action == ActionType.KillTile1)
                    {
                        Netplay.Clients[bufferId].SpamDeleteBlock += 1;
                    }
                    if (action == ActionType.PlaceTile || action == ActionType.PlaceWall)
                    {
                        Netplay.Clients[bufferId].SpamAddBlock += 1;
                    }
                }
                if (!Netplay.Clients[bufferId].TileSections[Netplay.GetSectionX(x), Netplay.GetSectionY(y)])
                {
                    fail = true;
                }
            }
            switch (action)
            {
                case ActionType.KillTile:
                    WorldGen.KillTile(x, y, fail, false, false);
                    break;
                case ActionType.PlaceTile:
                    WorldGen.PlaceTile(x, y, (int)type, false, true, -1, style);
                    break;
                case ActionType.KillWall:
                    WorldGen.KillWall(x, y, fail);
                    break;
                case ActionType.PlaceWall:
                    WorldGen.PlaceWall(x, y, (int)type, false);
                    break;
                case ActionType.KillTile1:
                    WorldGen.KillTile(x, y, fail, false, true);
                    break;
                case ActionType.PlaceWire:
                    WorldGen.PlaceWall(x, y, (int)type, false);
                    break;
                case ActionType.KillWire:
                    WorldGen.KillWire(x, y);
                    break;
                case ActionType.PoundTile:
                    WorldGen.PoundTile(x, y);
                    break;
                case ActionType.PlaceActuator:
                    WorldGen.PlaceActuator(x, y);
                    break;
                case ActionType.KillActuator:
                    WorldGen.KillActuator(x, y);
                    break;
                case ActionType.PlaceWire2:
                    WorldGen.PlaceWire2(x, y);
                    break;
                case ActionType.KillWire2:
                    WorldGen.KillWire2(x, y);
                    break;
                case ActionType.PlaceWire3:
                    WorldGen.PlaceWire3(x, y);
                    break;
                case ActionType.KillWire3:
                    WorldGen.KillWire3(x, y);
                    break;
                case ActionType.SlopeTile:
                    WorldGen.SlopeTile(x, y, (int)type);
                    break;
                case ActionType.FrameTrack:
                    Minecart.FrameTrack(x, y, true, false);
                    break;
                case ActionType.PlaceWire4:
                    WorldGen.PlaceWire4(x, y);
                    break;
                case ActionType.KillWire4:
                    WorldGen.KillWire4(x, y);
                    break;
                case ActionType.PlaceLogicGate:
                    Wiring.SetCurrentUser(bufferId);
                    Wiring.PokeLogicGate(x, y);
                    Wiring.SetCurrentUser(-1);
                    return true;
                case ActionType.Actuate:
                    Wiring.SetCurrentUser(bufferId);
                    Wiring.Actuate(x, y);
                    Wiring.SetCurrentUser(-1);
                    return true;

            }
            if (Main.netMode != 2)
            {
                return true;
            }
            NetMessage.SendData(17, -1, bufferId, "", (int)action, (float)x, (float)y, (float)type, style, 0, 0);
            if (action == ActionType.PlaceTile && type == 53)
            {
                NetMessage.SendTileSquare(-1, x, y, 1);
            }

            return true;
        }
    }
}

