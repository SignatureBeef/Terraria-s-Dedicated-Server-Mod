using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class ChestUnlockMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.CHEST_UNLOCK;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            byte playerId = ReadByte(readBuffer);

			if (playerId != whoAmI && Entry.EnableCheatProtection)
            {
                tdsm.api.Callbacks.NetplayCallback.slots[whoAmI].Kick("Cheating detected (CHEST_UNLOCK forgery).");
                return;
            }

            byte action = ReadByte(readBuffer);

            var x = (int)ReadInt16(readBuffer);
            var y = (int)ReadInt16(readBuffer);

            if (action == 1)
            {
                Chest.Unlock(x, y);

                NewNetMessage.SendData(52, -1, whoAmI, String.Empty, playerId, action, x, y, 0);
                NewNetMessage.SendTileSquare(-1, x, y, 2);
            }
            else if (action == 2)
            {
                WorldGen.UnlockDoor(x, y);

                NewNetMessage.SendData(52, -1, whoAmI, String.Empty, playerId, action, x, y, 0);
                NewNetMessage.SendTileSquare(-1, x, y, 2);
            }
        }
    }
}
