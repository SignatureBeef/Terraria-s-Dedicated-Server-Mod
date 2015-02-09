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
            //TODO add [LiquidFlowReceived] back in

            int x = (int)ReadInt16(readBuffer);
            int y = (int)ReadInt16(readBuffer);
            byte liquid = ReadByte(readBuffer);
            byte liquidType = ReadByte(readBuffer);

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
                    NewNetMessage.BootPlayer(whoAmI, "Cheating attempt detected: Liquid spam");
                    return;
                }
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
