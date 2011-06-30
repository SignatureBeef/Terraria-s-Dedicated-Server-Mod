using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class SendTileResponseMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.SEND_TILE_CONFIRM;
        }

        public int? GetRequiredNetMode()
        {
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int startX = (int)BitConverter.ToInt16(readBuffer, num);
            num += 4;
            int startY = (int)BitConverter.ToInt16(readBuffer, num);
            num += 4;
            int endX = (int)BitConverter.ToInt16(readBuffer, num);
            num += 4;
            int endY = (int)BitConverter.ToInt16(readBuffer, num);
            num += 4;
            WorldGen.SectionTileFrame(startX, startY, endX, endY);
        }
    }
}
