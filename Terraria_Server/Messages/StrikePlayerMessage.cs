using System;
using System.Text;

namespace Terraria_Server.Messages
{
    public class StrikePlayerMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.STRIKE_PLAYER;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
			int start = num - 1;
			int victimId = (int)readBuffer[num++];
			var aggressor = Main.players[whoAmI];
			Player victim = Main.players[victimId];
			
			if (whoAmI != victimId && (!victim.hostile || !Main.players[whoAmI].hostile))
			{
				return;
			}
			
			int hitDirection = (int)(readBuffer[num++] - 1);
			short damage = BitConverter.ToInt16(readBuffer, num);
			num += 2;
			byte pvpFlag = readBuffer[num++];
			bool pvp = (pvpFlag != 0);
			byte crit = readBuffer[num++];
			string deathText = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
			
			if (victim.Hurt(aggressor, (int)damage, hitDirection, pvp, true, deathText, crit == 1) > 0.0)
			{
				NetMessage.SendData(26, -1, whoAmI, deathText, victimId, (float)hitDirection, (float)damage, (float)pvpFlag, 0/*this is still 0 O_o*/);
			}
        }
    }
}
