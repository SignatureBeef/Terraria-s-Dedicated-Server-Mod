using System;

namespace Terraria_Server.Messages
{
    public class PlayerBallswingMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_BALLSWING;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = readBuffer[num];
            
            if (playerIndex != whoAmI)
            {
                Netplay.slots[whoAmI].Kick ("Cheating detected (PLAYER_BALLSWING forgery).");
                return;
            }

            playerIndex = whoAmI;
            num++;

            float itemRotation = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            int itemAnimation = (int)BitConverter.ToInt16(readBuffer, num);
            Main.players[playerIndex].itemRotation = itemRotation;
            Main.players[playerIndex].itemAnimation = itemAnimation;
            
            NetMessage.SendData(41, -1, whoAmI, "", playerIndex);
        }
    }
}
