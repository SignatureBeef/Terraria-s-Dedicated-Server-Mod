using System;

namespace Terraria_Server.Messages
{
    public class PlayerBallswingMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PLAYER_BALLSWING;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = whoAmI;

            float itemRotation = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            int itemAnimation = (int)BitConverter.ToInt16(readBuffer, num);
            Main.players[playerIndex].itemRotation = itemRotation;
            Main.players[playerIndex].itemAnimation = itemAnimation;
            
            NetMessage.SendData(41, -1, whoAmI, "", playerIndex);
        }
    }
}
