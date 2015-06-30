using System;
using tdsm.api;
using tdsm.api.Plugin;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class TileBreakMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.TILE_BREAK;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            byte action = ReadByte(readBuffer);
            int x = (int)ReadInt16(readBuffer);
            int y = (int)ReadInt16(readBuffer);
            short type = ReadInt16(readBuffer);
            int style = (int)ReadByte(readBuffer);
            bool fail = type == 1;

            var player = Main.player[whoAmI];

            if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
            {
                player.Kick("Out of range tile received from client.");
                return;
            }

            if (!Terraria.Netplay.serverSock[whoAmI].tileSection[Netplay.GetSectionX(x), Netplay.GetSectionY(y)])
            {
                Tools.WriteLine("{0} @ {1}: {2} attempted to alter world in unloaded tile.");
                return;
            }

            //TODO implement the old methods
            var ctx = new HookContext
            {
                Connection = (Terraria.Netplay.serverSock[whoAmI] as ServerSlot).conn,
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

                NewNetMessage.SendTileSquare(whoAmI, x, y, 1);
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
                        Terraria.Netplay.serverSock[whoAmI].spamDelBlock += 1f;
                    }
                    if (action == 1 || action == 3)
                    {
                        Terraria.Netplay.serverSock[whoAmI].spamAddBlock += 1f;
                    }
                }
                if (!Terraria.Netplay.serverSock[whoAmI].tileSection[Netplay.GetSectionX(x), Netplay.GetSectionY(y)])
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
            NewNetMessage.SendData(17, -1, whoAmI, String.Empty, (int)action, (float)x, (float)y, (float)type, style);
            if (action == 1 && type == 53)
            {
                NewNetMessage.SendTileSquare(-1, x, y, 1);
                return;
            }
        }
    }
}
