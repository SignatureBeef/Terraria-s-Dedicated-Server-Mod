using System;

namespace Terraria_Server.Messages
{
    public class PlayerBuffsMessage : SlotMessageHandler
    {
		public PlayerBuffsMessage ()
		{
			IgnoredStates = SlotState.ACCEPTED | SlotState.PLAYER_AUTH;
			ValidStates = SlotState.ASSIGNING_SLOT | SlotState.PLAYING;
		}

        public override Packet GetPacket()
        {
            return Packet.PLAYER_BUFFS;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = readBuffer[num];
            
            if (playerIndex != whoAmI)
            {
                NetPlay.slots[whoAmI].Kick ("Cheating detected (PLAYER_BUFFS forgery).");
                return;
            }
            
            playerIndex = whoAmI;
            num++;

            if (playerIndex == Main.myPlayer)
            {
                return;
            }

            Player player = Main.players[playerIndex];
            for (int i = 0; i < 10; i++)
            {
                player.buffType[i] = (int)readBuffer[num++];
                if (player.buffType[i] > 0)
                {
                    player.buffTime[i] = 60;
                }
                else
                {
                    player.buffTime[i] = 0;
                }
            }
            
            NetMessage.SendData(50, -1, whoAmI, "", playerIndex);
        }
    }
}
