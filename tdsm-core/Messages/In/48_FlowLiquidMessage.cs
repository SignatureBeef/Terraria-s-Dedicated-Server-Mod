using tdsm.api.Plugin;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class FlowLiquidMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.FLOW_LIQUID;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int x = (int)ReadInt16(readBuffer);
            int y = (int)ReadInt16(readBuffer);
            byte liquid = ReadByte(readBuffer);
            byte liquidType = ReadByte(readBuffer);

            var player = Main.player[whoAmI];

            if (Main.netMode == 2 && Netplay.spamCheck)
            {
                int num113 = whoAmI;
                int num114 = (int)(Main.player[num113].position.X + (float)(Main.player[num113].width / 2));
                int num115 = (int)(Main.player[num113].position.Y + (float)(Main.player[num113].height / 2));
                int num116 = 10;
                int num117 = num114 - num116;
                int num118 = num114 + num116;
                int num119 = num115 - num116;
                int num120 = num115 + num116;
                if (x < num117 || x > num118 || y < num119 || y > num120)
                {
                    //NewNetMessage.BootPlayer(whoAmI, "Cheating attempt detected: Liquid spam");
                    Main.player[num113].Kick("Cheating attempt detected: Liquid spam");
                    return;
                }
            }
            var ctx = new HookContext
            {
                Connection = player.Connection,
                Player = player,
                Sender = player,
            };

            var args = new HookArgs.LiquidFlowReceived
            {
                X = x,
                Y = y,
                Amount = liquid,
                Lava = liquidType == 1,
            };

            HookPoints.LiquidFlowReceived.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
                return;

            if (ctx.Result == HookResult.IGNORE)
                return;

            if (ctx.Result == HookResult.RECTIFY)
            {
                var msg = NewNetMessage.PrepareThreadInstance();
                msg.FlowLiquid(x, y);
                msg.Send(whoAmI);
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
    }
}
