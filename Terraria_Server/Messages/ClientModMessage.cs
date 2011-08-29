using System;
using Terraria_Server.Plugin;
using Terraria_Server.Definitions;
using Terraria_Server.WorldMod;
using Terraria_Server.Logging;

namespace Terraria_Server.Messages
{
    public class ClientModMessage : SlotMessageHandler
    {
		public ClientModMessage ()
		{
			ValidStates = SlotState.SENDING_TILES | SlotState.PLAYING;
		}
		
        public override Packet GetPacket()
        {
            return Packet.CLIENT_MOD;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            var player = Main.players[whoAmI];
            player.HasClientMod = true;
            ProgramLog.Log(player.Name + " has logged in with the TDCM Client");
        }
    }
}
