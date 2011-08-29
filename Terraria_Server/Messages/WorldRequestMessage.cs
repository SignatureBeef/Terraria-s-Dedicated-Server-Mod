using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Networking;

namespace Terraria_Server.Messages
{
    public class WorldRequestMessage : MessageHandler
    {
		public WorldRequestMessage ()
		{
			IgnoredStates = SlotState.PLAYER_AUTH;
			ValidStates = SlotState.ACCEPTED | SlotState.ASSIGNING_SLOT;
		}

        public override Packet GetPacket()
        {
            return Packet.WORLD_REQUEST;
        }

        public override void Process (ClientConnection conn, byte[] readBuffer, int length, int num)
        {
            if (conn.State == SlotState.ACCEPTED)
            {
                SlotManager.Schedule (conn, conn.Queue);
                return;
            }
            
            int whoAmI = conn.SlotIndex;

            if (Netplay.slots[whoAmI].state == SlotState.ASSIGNING_SLOT)
            {
                Netplay.slots[whoAmI].state = SlotState.SENDING_WORLD;
            }
            NetMessage.SendData(7, whoAmI);
        }
    }
}
