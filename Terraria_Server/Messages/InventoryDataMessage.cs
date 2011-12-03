using System;
using System.Text;
using Terraria_Server.Collections;
using Terraria_Server.Plugins;

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
                NetPlay.slots[whoAmI].Kick ("Cheating detected (INVENTORY_DATA forgery).");
                return;
            }
            
			var player = Main.players[whoAmI];
			
			var ctx = new HookContext
			{
				Connection = NetPlay.slots[whoAmI].conn,
				Sender = player,
				Player = player,
			};
			
			var args = new HookArgs.InventoryItemReceived
			{
				InventorySlot = readBuffer[num++],
				Amount = readBuffer[num++],
				Name = Networking.StringCache.FindOrMake (new ArraySegment<Byte> (readBuffer, num, length - 4)),
			};
			
			HookPoints.InventoryItemReceived.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick ())
				return;
			
			if (ctx.Result == HookResult.IGNORE)
				return;
			
			var itemName = args.Name;
			var inventorySlot = args.InventorySlot;
			var stack = args.Amount;
			
			var item = Registries.Item.Create (itemName, stack);

            if (inventorySlot < (Player.MAX_INVENTORY - 1))
			{
				player.inventory[inventorySlot] = item;
			}
			else
			{
                player.armor[inventorySlot - (Player.MAX_INVENTORY - 1)] = item;
			}
			
			if (ctx.Result != HookResult.CONTINUE)
			{
				if (Server.RejectedItemsContains(itemName) ||
					Server.RejectedItemsContains(item.Type.ToString()))
				{
					player.Kick(((itemName.Length > 0) ? itemName : item.Type.ToString()) + " is not allowed on this server.");
				}
			}
			
			NetMessage.SendData(5, -1, whoAmI, itemName, playerIndex, (float)inventorySlot);
        }
    }
}
