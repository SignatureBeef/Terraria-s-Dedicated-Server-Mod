using tdsm.api;
using Terraria;

namespace tdsm.core.Messages.In
{
	public class ChestItemMessage : SlotMessageHandler
	{
		public override Packet GetPacket()
		{
			return Packet.CHEST_ITEM;
		}

		public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
		{
            var chestIndex = (int)ReadInt16(readBuffer);
            var contentsIndex = (int)ReadByte(readBuffer);
            var stackSize = (int)ReadInt16(readBuffer);
            var prefix = (int)ReadByte(readBuffer);
            var type3 = (int)ReadInt16(readBuffer);

            if (Main.chest[chestIndex] == null)
                Main.chest[chestIndex] = new Chest(false);

            if (Main.chest[chestIndex].item[contentsIndex] == null)
                Main.chest[chestIndex].item[contentsIndex] = new Item();

            Main.chest[chestIndex].item[contentsIndex].netDefaults(type3);
            Main.chest[chestIndex].item[contentsIndex].Prefix(prefix);
            Main.chest[chestIndex].item[contentsIndex].stack = stackSize;
		}
	}
}
