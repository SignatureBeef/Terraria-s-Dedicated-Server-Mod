using System;

namespace Terraria_Server.Messages
{
    public class FlowLiquidMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.FLOW_LIQUID;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            byte liquid = readBuffer[num++];
            byte lavaFlag = readBuffer[num]++;

            if (Netplay.spamCheck)
            {
                int playerIndex = whoAmI;
                Player player = Main.players[playerIndex];
                int centerX = (int)(player.Position.X + (float)(player.width / 2));
                int centerY = (int)(player.Position.Y + (float)(player.height / 2));
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
            TileRef tile = Main.tile.At(x, y);
            {
                tile.SetLiquid (liquid);
                tile.SetLava (lavaFlag == 1);

                WorldGen.SquareTileFrame(x, y, true);
            }
        }
    }
}
