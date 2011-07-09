using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class SendTileLoadingMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.SEND_TILE_LOADING_MESSAGE;
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
                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                }

                Tile tile = Main.tile[x, y];

                byte b3 = readBuffer[num++];
                bool active = tile.Active;
                tile.Active = ((b3 & 1) == 1);

                if ((b3 & 2) == 2)
                {
                    tile.lighted = true;
                }

                if ((b3 & 4) == 4)
                {
                    tile.wall = 1;
                }
                else
                {
                    tile.wall = 0;
                }

                if ((b3 & 8) == 8)
                {
                    tile.liquid = 1;
                }
                else
                {
                    tile.liquid = 0;
                }

                if (tile.Active)
                {
                    int type = (int)tile.type;
                    tile.type = readBuffer[num++];

                    if (Main.tileFrameImportant[(int)tile.type])
                    {
                        tile.frameX = BitConverter.ToInt16(readBuffer, num);
                        num += 2;
                        tile.frameY = BitConverter.ToInt16(readBuffer, num);
                        num += 2;
                    }
                    else if (!active || (int)tile.type != type)
                    {
                        tile.frameX = -1;
                        tile.frameY = -1;
                    }
                }

                if (tile.wall > 0)
                {
                    tile.wall = readBuffer[num++];
                }

                if (tile.liquid > 0)
                {
                    tile.liquid = readBuffer[num++];
                    byte lavaFlag = readBuffer[num++];
                    tile.lava = (lavaFlag == 1);
                }
            }
            
            NetMessage.SendData((int)bufferData, -1, whoAmI, "", (int)width, (float)left, (float)y);
        }
    }
}
