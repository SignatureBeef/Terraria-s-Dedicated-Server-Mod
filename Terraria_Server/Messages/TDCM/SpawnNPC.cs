using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Logging;

namespace Terraria_Server.Messages.TDCM
{
    public class SpawnNPC : SlotMessageHandler
    {
        public SpawnNPC()
        {
            ValidStates = SlotState.SENDING_TILES | SlotState.PLAYING;
        }

        public override Packet GetPacket()
        {
            return Packet.CLIENT_MOD_SPAWN_NPC;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            var player = Main.players[whoAmI];

            if (!player.HasClientMod)
                player.Kick("Invalid Client Message, Acting as TDCM");

            //[TODO]

        }
    }
}
