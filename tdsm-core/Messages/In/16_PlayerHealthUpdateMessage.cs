using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PlayerHealthUpdateMessage : SlotMessageHandler
    {
        public PlayerHealthUpdateMessage()
        {
            IgnoredStates = SlotState.ACCEPTED | SlotState.PLAYER_AUTH | SlotState.ASSIGNING_SLOT;
            ValidStates = SlotState.PLAYING;
        }

        public override Packet GetPacket()
        {
            return Packet.PLAYER_HEALTH_UPDATE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = (int)ReadByte(readBuffer);

			if (playerIndex != whoAmI && Entry.EnableCheatProtection)
            {
                Terraria.Netplay.Clients[whoAmI].Kick("Cheating detected (PLAYER_HEALTH_UPDATE forgery).");
                return;
            }

            if (playerIndex == Main.myPlayer && !Main.ServerSideCharacter)
            {
                return;
            }
            //if (Main.netMode == 2)
            //{
            //    playerIndex = whoAmI;
            //}
            var plr = Main.player[playerIndex];
            plr.statLife = (int)ReadInt16(readBuffer);
            plr.statLifeMax = (int)ReadInt16(readBuffer);

            if (plr.statLifeMax < 100)
                plr.statLifeMax = 100;
            plr.dead = (plr.statLife <= 0);

            NewNetMessage.SendData((int)Packet.PLAYER_HEALTH_UPDATE, -1, whoAmI, String.Empty, playerIndex, 0f, 0f, 0f, 0);
        }
    }
}
