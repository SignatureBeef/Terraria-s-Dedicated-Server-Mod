using System;
using Terraria_Server.Plugin;
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
                WorldModify.OpenDoor(x, y, direction, state, DoorOpener.PLAYER, Main.players[whoAmI]);
            }
            else if (doorAction == 1)
            {
                WorldModify.CloseDoor(x, y, true, DoorOpener.PLAYER, Main.players[whoAmI]);
            }
                        
            NetMessage.SendData(19, -1, whoAmI, "", (int)doorAction, (float)x, (float)y, (float)doorDirection);
        }
    }
}
