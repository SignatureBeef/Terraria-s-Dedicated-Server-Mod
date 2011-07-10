using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class SendTileRowMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.SEND_TILE_ROW;
        }

        public int? GetRequiredNetMode()
        {
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            short width = BitConverter.ToInt16(readBuffer, start + 1);
            int left = BitConverter.ToInt32(readBuffer, start + 3);
            int y = BitConverter.ToInt32(readBuffer, start + 7);
            num = start + 11;

            for (int x = left; x < left + (int)width; x++)
            {

                TileRef tile = Main.tile[x, y];

                byte b3 = readBuffer[num++];
                bool active = tile.Active;
                tile.Active = ((b3 & 1) == 1);

                if ((b3 & 2) == 2)
                {
                    tile.Lighted = true;
                }

                if ((b3 & 4) == 4)
                {
                    tile.Wall = 1;
                }
                else
                {
                    tile.Wall = 0;
                }

                if ((b3 & 8) == 8)
                {
                    tile.Liquid = 1;
                }
                else
                {
                    tile.Liquid = 0;
                }

                if (tile.Active)
                {
                    int type = (int)tile.Type;
                    tile.Type = readBuffer[num++];

                    if (Main.tileFrameImportant[(int)tile.Type])
                    {
                        tile.FrameX = BitConverter.ToInt16(readBuffer, num);
                        num += 2;
                        tile.FrameY = BitConverter.ToInt16(readBuffer, num);
                        num += 2;
                    }
                    else if (!active || (int)tile.Type != type)
                    {
                        tile.FrameX = -1;
                        tile.FrameY = -1;
                    }
                }

                if (tile.Wall > 0)
                {
                    tile.Wall = readBuffer[num++];
                }

                if (tile.Liquid > 0)
                {
                    tile.Liquid = readBuffer[num++];
                    byte lavaFlag = readBuffer[num++];
                    tile.Lava = (lavaFlag == 1);
                }
            }
            
            NetMessage.SendData((int)bufferData, -1, whoAmI, "", (int)width, (float)left, (float)y);
        }
    }
}
