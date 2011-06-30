using System;

namespace Terraria_Server.Messages
{
    public class PlayerChestUpdate : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PLAYER_CHEST_UPDATE;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int inventoryIndex = BitConverter.ToInt32(readBuffer, num);
            num += 2;
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);

            if (Main.netMode == 1)
            {
                Player player = Main.player[Main.myPlayer];
                if (player.chest == -1
                    || (player.chest != inventoryIndex && inventoryIndex != -1))
                {
                    Main.playerInventory = true;
                }

                player.chest = inventoryIndex;
                player.chestX = x;
                player.chestY = y;
            }
            else
            {
                Main.player[whoAmI].chest = inventoryIndex;
            }
        }
    }
}
