using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PlayerBuffsMessage : SlotMessageHandler
    {
        public PlayerBuffsMessage()
        {
            IgnoredStates = SlotState.ACCEPTED | SlotState.PLAYER_AUTH;
            ValidStates = SlotState.ASSIGNING_SLOT | SlotState.PLAYING;
        }

        public override Packet GetPacket()
        {
            return Packet.PLAYER_BUFFS;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = ReadByte(readBuffer);

			if (playerIndex != whoAmI && Entry.EnableCheatProtection)
            {
                Terraria.Netplay.serverSock[whoAmI].Kick("Cheating detected (PLAYER_BUFFS forgery).");
                return;
            }

            playerIndex = whoAmI;

            if (playerIndex == Main.myPlayer)
            {
                return;
            }

            var player = Main.player[playerIndex];
            for (int i = 0; i < 10; i++)
            {
                player.buffType[i] = (int)ReadByte(readBuffer);
                if (player.buffType[i] > 0)
                {
                    player.buffTime[i] = 60;
                }
                else
                {
                    player.buffTime[i] = 0;
                }
            }

            NewNetMessage.SendData((int)Packet.PLAYER_BUFFS, -1, whoAmI, String.Empty, playerIndex);
        }
    }
}
