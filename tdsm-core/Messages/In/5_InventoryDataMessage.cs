using System;
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
                Server.slots[whoAmI].Kick("Cheating detected (INVENTORY_DATA forgery).");
                return;
            }

            //TODO [InventoryItemReceived]
            //TODO Implement the item banning


            if (playerIndex == Main.myPlayer && !Main.ServerSideCharacter)
            {
                return;
            }
            Player player3 = Main.player[playerIndex];
            lock (player3)
            {
                int num5 = (int)ReadByte(readBuffer);
                int stack = (int)ReadInt16(readBuffer);
                int num6 = (int)ReadByte(readBuffer);
                int type = (int)ReadInt16(readBuffer);
                if (num5 < 59)
                {
                    player3.inventory[num5] = new Item();
                    player3.inventory[num5].netDefaults(type);
                    player3.inventory[num5].stack = stack;
                    player3.inventory[num5].Prefix(num6);
                    if (playerIndex == Main.myPlayer && num5 == 58)
                    {
                        Main.mouseItem = player3.inventory[num5].Clone();
                    }
                }
                else
                {
                    if (num5 >= 75 && num5 <= 82)
                    {
                        int num7 = num5 - 58 - 17;
                        player3.dye[num7] = new Item();
                        player3.dye[num7].netDefaults(type);
                        player3.dye[num7].stack = stack;
                        player3.dye[num7].Prefix(num6);
                    }
                    else
                    {
                        int num8 = num5 - 58 - 1;
                        player3.armor[num8] = new Item();
                        player3.armor[num8].netDefaults(type);
                        player3.armor[num8].stack = stack;
                        player3.armor[num8].Prefix(num6);
                    }
                }

                if (playerIndex == whoAmI)
                {
                    NewNetMessage.SendData((int)Packet.INVENTORY_DATA, -1, whoAmI, String.Empty, playerIndex, (float)num5, (float)num6, 0f, 0);
                }
                return;
            }
        }
    }
}
