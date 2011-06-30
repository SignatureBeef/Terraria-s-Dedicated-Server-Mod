using System;
using System.Text;

namespace Terraria_Server.Messages
{
    public class InventoryDataMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.INVENTORY_DATA;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = (int)readBuffer[start + 1];
            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            if (playerIndex != Main.myPlayer)
            {
                Player player = Main.player[playerIndex];
                lock (player)
                {
                    int inventorySlot = (int)readBuffer[start + 2];
                    int stack = (int)readBuffer[start + 3];
                    string itemName = Encoding.ASCII.GetString(readBuffer, start + 4, length - 4);
                    if (inventorySlot < 44)
                    {
                        player.inventory[inventorySlot] = new Item();
                        player.inventory[inventorySlot].SetDefaults(itemName);
                        player.inventory[inventorySlot].Stack = stack;
                    }
                    else
                    {
                        player.armor[inventorySlot - 44] = new Item();
                        player.armor[inventorySlot - 44].SetDefaults(itemName);
                        player.armor[inventorySlot - 44].Stack = stack;
                    }

                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(5, -1, whoAmI, itemName, playerIndex, (float)inventorySlot);
                    }
                }
            }
        }
    }
}
