using System;

namespace Terraria_Server.Messages
{
    public class KillTileMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.KILL_TILE;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            if (Main.tile[x, y].type == 21)
            {
                WorldGen.KillTile(x, y);
                if (!Main.tile[x, y].Active)
                {
                    NetMessage.SendData(17, -1, -1, "", 0, (float)x, (float)y);
                }
            }
        }
    }
}
