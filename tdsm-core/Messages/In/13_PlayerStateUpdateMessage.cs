using System;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PlayerStateUpdateMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_STATE_UPDATE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            if (ReadByte(readBuffer) != whoAmI)
            {
                Server.slots[whoAmI].Kick("Cheating detected (PLAYER_STATE_UPDATE forgery).");
                return;
            }

            //TODO [StateUpdateReceived]

            Player player5 = Main.player[whoAmI];
            BitsByte bitsByte4 = ReadByte(readBuffer);
            player5.controlUp = bitsByte4[0];
            player5.controlDown = bitsByte4[1];
            player5.controlLeft = bitsByte4[2];
            player5.controlRight = bitsByte4[3];
            player5.controlJump = bitsByte4[4];
            player5.controlUseItem = bitsByte4[5];
            player5.direction = (bitsByte4[6] ? 1 : -1);
            BitsByte bitsByte5 = ReadByte(readBuffer);
            if (bitsByte5[0])
            {
                player5.pulley = true;
                player5.pulleyDir = (byte)(bitsByte5[1] ? 2 : 1);
            }
            else
            {
                player5.pulley = false;
            }
            player5.selectedItem = (int)ReadByte(readBuffer);
            player5.position = ReadVector2(readBuffer);
            if (bitsByte5[2])
            {
                player5.velocity = ReadVector2(readBuffer);
            }
            if (Main.netMode == 2 && Server.slots[whoAmI].state == SlotState.PLAYING)
            {
                NewNetMessage.SendData(13, -1, whoAmI, String.Empty, whoAmI, 0f, 0f, 0f, 0);
                return;
            }
        }
    }
}
