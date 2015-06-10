using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PlayerBallswingMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_BALLSWING;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = ReadByte(readBuffer);

			if (playerIndex != whoAmI && Entry.EnableCheatProtection)
            {
                Terraria.Netplay.serverSock[whoAmI].Kick("Cheating detected (PLAYER_BALLSWING forgery).");
                return;
            }

            playerIndex = whoAmI;

            float itemRotation = ReadSingle(readBuffer);
            num += 4;
            int itemAnimation = (int)ReadInt16(readBuffer);
            Main.player[playerIndex].itemRotation = itemRotation;
            Main.player[playerIndex].itemAnimation = itemAnimation;
            Main.player[playerIndex].channel = Main.player[playerIndex].inventory[Main.player[playerIndex].selectedItem].channel;

            NewNetMessage.SendData(41, -1, whoAmI, String.Empty, playerIndex);
        }
    }
}
