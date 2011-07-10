using System;

namespace Terraria_Server.Messages
{
    public class UpdatePlayersMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.UPDATE_PLAYERS;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            NetMessage.syncPlayers();
        }
    }
}
