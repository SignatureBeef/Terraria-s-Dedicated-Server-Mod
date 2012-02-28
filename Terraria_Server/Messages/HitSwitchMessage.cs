using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.WorldMod;

namespace Terraria_Server.Messages
{
	public class HitSwitchMessage : SlotMessageHandler
	{
		public override Packet GetPacket()
		{
			return Packet.HIT_SWITCH;
		}

		public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
		{
			int X = BitConverter.ToInt32(readBuffer, num);
			num += 4;
			int Y = BitConverter.ToInt32(readBuffer, num);
			num += 4;

			WorldModify.hitSwitch(null, null, X, Y, Main.players[whoAmI]);			
			NetMessage.SendData(59, -1, whoAmI, "", X, (float)Y, 0f, 0f, 0);
		}
	}
}
