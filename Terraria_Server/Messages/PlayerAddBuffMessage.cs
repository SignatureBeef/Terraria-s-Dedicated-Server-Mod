using System;
using Terraria_Server.Logging;
using Terraria_Server.Definitions;

namespace Terraria_Server.Messages
{
	public class PlayerAddBuffMessage : SlotMessageHandler
	{
		public override Packet GetPacket()
		{
			return Packet.PLAYER_ADD_BUFF;
		}

		public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
		{
			var playerId = readBuffer[num++];
			var type = readBuffer[num++];
			var time = BitConverter.ToInt16(readBuffer, num);

			if (type > 26 || time > Player.MAX_BUFF_TIME /*max buff time*/
				|| (playerId != whoAmI && (
				(
					type != (int)ProjectileType.N20_GREEN_LASER &&
					type != (int)ProjectileType.N24_SPIKY_BALL)
				|| time > Player.MAX_BUFF_TIME /*max debuff time*/)))
			{
				ProgramLog.Debug.Log("PLAYER_ADD_BUFF: from={0}, for={1}, type={2}, time={3}", whoAmI, playerId, type, time);
				NetPlay.slots[whoAmI].Kick("Cheating detected (PLAYER_ADD_BUFF forgery).");
				return;
			}

			if (type == (int)ProjectileType.N20_GREEN_LASER ||
				type == (int)ProjectileType.N24_SPIKY_BALL)
				whoAmI = playerId;

			Main.players[whoAmI].AddBuff(type, time, true);

			NetMessage.SendData(55, whoAmI, -1, String.Empty, whoAmI, type, time, 0f, 0);
		}
	}
}
