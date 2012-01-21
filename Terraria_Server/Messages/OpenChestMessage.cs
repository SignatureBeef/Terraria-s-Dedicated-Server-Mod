using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugins;

namespace Terraria_Server.Messages
{
	public class OpenChestMessage : SlotMessageHandler
	{
		public override Packet GetPacket()
		{
			return Packet.OPEN_CHEST;
		}

		public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
		{
			int x = BitConverter.ToInt32(readBuffer, num);
			num += 4;
			int y = BitConverter.ToInt32(readBuffer, num);
			num += 4;

			var player = Main.players[whoAmI];

			if (Math.Abs(player.Position.X / 16 - x) >= 7 || Math.Abs(player.Position.Y / 16 - y) >= 7)
			{
				return;
			}

			int chestIndex = Chest.FindChest(x, y);

			var ctx = new HookContext
			{
				Connection = player.Connection,
				Player = player,
				Sender = player,
			};

			var args = new HookArgs.ChestOpenReceived
			{
				X = x,
				Y = y,
				ChestIndex = (short)chestIndex,
			};

			HookPoints.ChestOpenReceived.Invoke(ref ctx, ref args);

			if (ctx.CheckForKick())
			{
				return;
			}

			if (ctx.Result == HookResult.IGNORE)
			{
				return;
			}

			if (ctx.Result == HookResult.DEFAULT && chestIndex > -1)
			{
				var user = Chest.UsingChest(chestIndex);
				if (user >= 0 && user != whoAmI) return;

				for (int i = 0; i < Chest.MAX_ITEMS; i++)
				{
					NetMessage.SendData(32, whoAmI, -1, "", chestIndex, (float)i);
				}
				NetMessage.SendData(33, whoAmI, -1, "", chestIndex);
				Main.players[whoAmI].chest = chestIndex;
				return;
			}
		}
	}
}
