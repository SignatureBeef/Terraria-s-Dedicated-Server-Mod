using System;

namespace Terraria_Server.Messages
{
    public class FlowLiquidMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.FLOW_LIQUID;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            byte liquid = readBuffer[num++];
            byte lavaFlag = readBuffer[num]++;

            if (Main.netMode == 2 && Netplay.spamCheck)
            {
                int playerIndex = whoAmI;
                Player player = Main.players[playerIndex];
                int centerX = (int)(player.position.X + (float)(player.width / 2));
                int centerY = (int)(player.position.Y + (float)(player.height / 2));
                int disperseDistance = 10;
                int left = centerX - disperseDistance;
                int right = centerX + disperseDistance;
                int top = centerY - disperseDistance;
                int bottom = centerY + disperseDistance;
                if (centerX < left || centerX > right || centerY < top || centerY > bottom)
                {
                    NetMessage.BootPlayer(whoAmI, "Cheating attempt detected: Liquid spam");
                    return;
                }
            }
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }
            Tile tile = Main.tile[x, y];
            lock (tile)
            {
                tile.liquid = liquid;
                tile.lava = (lavaFlag == 1);

                if (Main.netMode == 2)
                {
                    WorldGen.SquareTileFrame(x, y, true);
                }
            }
        }
    }
}
