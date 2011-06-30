using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public interface IMessage
    {
        Packet GetPacket();

        int? GetRequiredNetMode();

        void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData);
    }
}
