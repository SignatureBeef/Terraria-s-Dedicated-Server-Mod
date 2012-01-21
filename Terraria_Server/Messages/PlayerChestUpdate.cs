using System;

namespace Terraria_Server.Messages
{
	public class PlayerChestUpdate : SlotMessageHandler
	{
		public override Packet GetPacket()
		{
			return Packet.PLAYER_CHEST_UPDATE;
		}

		public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
		{
			int inventoryIndex = (int)BitConverter.ToInt16(readBuffer, num);
			num += 2;
			int x = BitConverter.ToInt32(readBuffer, num);
			num += 4;
			int y = BitConverter.ToInt32(readBuffer, num);

			var player = Main.players[whoAmI];

			if (Math.Abs(player.Position.X / 16 - x) < 7 && Math.Abs(player.Position.Y / 16 - y) < 7)
			{
				Main.players[whoAmI].chest = inventoryIndex;
			}
			else
			{
				Main.players[whoAmI].chest = -1;
			}
		}
	}
}
