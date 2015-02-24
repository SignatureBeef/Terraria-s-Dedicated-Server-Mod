using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PlayerChestUpdate : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_CHEST_UPDATE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            var player = Main.player[whoAmI];

            var id = (int)ReadInt16(readBuffer);
            var chestX = (int)ReadInt16(readBuffer);
            var chestY = (int)ReadInt16(readBuffer);
            var nameLength = (int)ReadByte(readBuffer);

            var name = String.Empty;
            if (nameLength != 0)
            {
                if (nameLength <= 20)
                {
                    name = ReadString(readBuffer);
                }
                else if (nameLength != 255)
                {
                    nameLength = 0;
                }
            }

            if (Math.Abs(player.position.X / 16 - chestX) < 7 && Math.Abs(player.position.Y / 16 - chestY) < 7)
            {
                if (nameLength != 0)
                {
                    var playerChestId = Main.player[whoAmI].chest;
                    var playerChest = Main.chest[playerChestId];
                    playerChest.name = name;

                    NewNetMessage.SendData(69, -1, whoAmI, name, playerChestId, (float)playerChest.x, (float)playerChest.y, 0f, 0);
                }
                Main.player[whoAmI].chest = id;
            }
            else Main.player[whoAmI].chest = -1;
        }
    }
}
