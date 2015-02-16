using System;
using tdsm.api;
using tdsm.api.Plugin;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class KillTileMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.KILL_TILE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            byte method = ReadByte(readBuffer);
            int x = (int)ReadInt16(readBuffer);
            int y = (int)ReadInt16(readBuffer);
            int type = (int)ReadInt16(readBuffer);

            var player = Main.player[whoAmI];

            var ctx = new HookContext
            {
                Connection = player.Connection,
                Player = player,
                Sender = player,
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
                NewNetMessage.SendTileSquare(whoAmI, x, y, 3);
                return;
            }

            {
                if (method == 0)
                {
                    int num92 = WorldGen.PlaceChest(x, y, 21, false, type);
                    if (num92 == -1)
                    {
                        NewNetMessage.SendData(34, whoAmI, -1, String.Empty, (int)method, (float)x, (float)y, (float)type, num92);
                        Item.NewItem(x * 16, y * 16, 32, 32, Chest.itemSpawn[type], 1, true, 0, false);
                        return;
                    }
                    NewNetMessage.SendData(34, -1, -1, String.Empty, (int)method, (float)x, (float)y, (float)type, num92);
                    return;
                }
                else
                {
                    Tile tile2 = Main.tile[x, y];
                    if (tile2.type != 21)
                    {
                        return;
                    }
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
                        NewNetMessage.SendData(34, -1, -1, String.Empty, (int)method, (float)x, (float)y, 0f, number);
                        return;
                    }
                    return;
                }
            }
        }
    }
}
