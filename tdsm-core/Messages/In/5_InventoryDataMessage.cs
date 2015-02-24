using System;
using tdsm.api;
using tdsm.api.Plugin;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class InventoryDataMessage : SlotMessageHandler
    {
        public InventoryDataMessage()
        {
            IgnoredStates = SlotState.ACCEPTED | SlotState.PLAYER_AUTH;
            ValidStates = SlotState.ASSIGNING_SLOT | SlotState.PLAYING;
        }

        public override Packet GetPacket()
        {
            return Packet.INVENTORY_DATA;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = ReadByte(readBuffer);

            if (playerIndex != whoAmI)
            {
                tdsm.api.Callbacks.NetplayCallback.slots[whoAmI].Kick("Cheating detected (INVENTORY_DATA forgery).");
                return;
            }

            //TODO Implement the item banning

            if (playerIndex == Main.myPlayer && !Main.ServerSideCharacter)
                return;

            var player = Main.player[whoAmI];

            var ctx = new HookContext
            {
                Connection = (tdsm.api.Callbacks.NetplayCallback.slots[whoAmI] as ServerSlot).conn,
                Sender = player,
                Player = player,
            };

            var args = new HookArgs.InventoryItemReceived
            {
                InventorySlot = (int)ReadByte(readBuffer),
                Amount = (int)ReadInt16(readBuffer),
                Prefix = (int)ReadByte(readBuffer),
                NetID = (int)ReadInt16(readBuffer),
                //Name = Networking.StringCache.FindOrMake (new ArraySegment<Byte> (readBuffer, num, length - 4)),
            };

            HookPoints.InventoryItemReceived.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
                return;

            if (ctx.Result == HookResult.IGNORE)
                return;

            lock (player)
            {
                if (args.InventorySlot < 59)
                {
                    player.inventory[args.InventorySlot] = new Item();
                    player.inventory[args.InventorySlot].netDefaults(args.NetID);
                    player.inventory[args.InventorySlot].stack = args.Amount;
                    player.inventory[args.InventorySlot].Prefix(args.Prefix);
                    if (playerIndex == Main.myPlayer && args.InventorySlot == 58)
                    {
                        Main.mouseItem = player.inventory[args.InventorySlot].Clone();
                    }
                }
                else
                {
                    if (args.InventorySlot >= 75 && args.InventorySlot <= 82)
                    {
                        int num7 = args.InventorySlot - 58 - 17;
                        player.dye[num7] = new Item();
                        player.dye[num7].netDefaults(args.NetID);
                        player.dye[num7].stack = args.Amount;
                        player.dye[num7].Prefix(args.Prefix);
                    }
                    else
                    {
                        int num8 = args.InventorySlot - 58 - 1;
                        player.armor[num8] = new Item();
                        player.armor[num8].netDefaults(args.NetID);
                        player.armor[num8].stack = args.Amount;
                        player.armor[num8].Prefix(args.Prefix);
                    }
                }

                if (playerIndex == whoAmI)
                {
                    NewNetMessage.SendData((int)Packet.INVENTORY_DATA, -1, whoAmI, String.Empty, playerIndex, (float)args.InventorySlot, (float)args.Prefix, 0f, 0);
                }
                return;
            }
        }
    }
}
