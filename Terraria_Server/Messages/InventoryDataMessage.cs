using System;
using System.Text;
using Terraria_Server.Collections;

namespace Terraria_Server.Messages
{
    public class InventoryDataMessage : SlotMessageHandler
    {
		public InventoryDataMessage ()
		{
			IgnoredStates = SlotState.ACCEPTED | SlotState.PLAYER_AUTH;
			ValidStates = SlotState.ASSIGNING_SLOT | SlotState.PLAYING;
		}

        public override Packet GetPacket()
        {
            return Packet.INVENTORY_DATA;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = readBuffer[num++];
            
            if (playerIndex != whoAmI)
            {
                Netplay.slots[whoAmI].Kick ("Cheating detected (INVENTORY_DATA forgery).");
                return;
            }
        
            playerIndex = whoAmI;

            if (playerIndex != Main.myPlayer)
            {
                Player player = Main.players[playerIndex];
                lock (player)
                {
                    int inventorySlot = (int)readBuffer[num++];
                    int stack = (int)readBuffer[num++];
                    String itemName = Encoding.ASCII.GetString (readBuffer, num, length - 4);
                    Item item = Registries.Item.Create(itemName, stack);
                    if (inventorySlot < 44)
                    {
                        player.inventory[inventorySlot] = item;
                    }
                    else
                    {
                        player.armor[inventorySlot - 44] = item;
                    }

                    NetMessage.SendData(5, -1, whoAmI, itemName, playerIndex, (float)inventorySlot);
                }
            }
        }
    }
}
