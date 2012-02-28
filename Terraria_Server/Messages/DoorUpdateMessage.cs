using System;
using Terraria_Server.Definitions;
using Terraria_Server.WorldMod;

namespace Terraria_Server.Messages
{
    public class DoorUpdateMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.DOOR_UPDATE;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            byte doorAction = readBuffer[num++];
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            int doorDirection = (int)readBuffer[num];
            int direction = 0;
            if (doorDirection == 0)
            {
                direction = -1;
            }

            bool state = (doorAction == 0); //if open

            if (state)
            {
                lock (WorldModify.playerEditLock)
                    WorldModify.OpenDoor(null, null, x, y, direction, Main.players[whoAmI]);
            }
            else if (doorAction == 1)
            {
                lock (WorldModify.playerEditLock)
                    WorldModify.CloseDoor(null, null, x, y, false, Main.players[whoAmI]);
            }
                        
            NetMessage.SendData(19, -1, whoAmI, "", (int)doorAction, (float)x, (float)y, (float)doorDirection);
        }
    }
}
