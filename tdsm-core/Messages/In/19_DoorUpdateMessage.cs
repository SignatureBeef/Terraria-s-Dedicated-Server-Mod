using System;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class DoorUpdateMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.DOOR_UPDATE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            byte doorAction = ReadByte(readBuffer);
            int x = (int)ReadInt16(readBuffer);
            int y = (int)ReadInt16(readBuffer);
            int doorDirection = (ReadByte(readBuffer) == 0) ? -1 : 1;
            if (doorAction == 0)
            {
                WorldGen.OpenDoor(x, y, doorDirection);
            }
            else if (doorAction == 1)
            {
                WorldGen.CloseDoor(x, y, true);
            }

            NewNetMessage.SendData(19, -1, whoAmI, String.Empty, (int)doorAction, (float)x, (float)y, (float)((doorDirection == 1) ? 1 : 0), 0);
        }
    }
}
