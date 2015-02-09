using System;

namespace Terraria_Server.Messages
{
    public class PlayerManaUpdateMessage : SlotMessageHandler
    {
		public PlayerManaUpdateMessage ()
		{
			IgnoredStates = SlotState.ACCEPTED | SlotState.PLAYER_AUTH;
			ValidStates = SlotState.ASSIGNING_SLOT | SlotState.PLAYING;
		}

        public override Packet GetPacket()
        {
            return Packet.PLAYER_MANA_UPDATE;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = readBuffer[num];
            
            if (playerIndex != whoAmI)
            {
                NetPlay.slots[whoAmI].Kick ("Cheating detected (PLAYER_MANA_UPDATE forgery).");
                return;
            }
            
            playerIndex = whoAmI;
            num++;

            int statMana = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            int statManaMax = (int)BitConverter.ToInt16(readBuffer, num);

            Main.players[playerIndex].statMana = statMana;
            Main.players[playerIndex].statManaMax = statManaMax;

            //if (Main.players[playerIndex].HasHackedData())
            //{
            //    Main.players[playerIndex].Kick("No Hacked Mana is allowed.");
            //}
            
            NetMessage.SendData(42, -1, whoAmI, "", playerIndex);
        }
    }
}
