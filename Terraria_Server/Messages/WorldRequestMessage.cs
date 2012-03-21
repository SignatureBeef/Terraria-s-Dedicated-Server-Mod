using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Networking;
using Terraria_Server.Plugins;

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
                SlotManager.Schedule (conn, conn.DesiredQueue);
                return;
            }
            
            int whoAmI = conn.SlotIndex;

            if (NetPlay.slots[whoAmI].state == SlotState.ASSIGNING_SLOT)
            {
                NetPlay.slots[whoAmI].state = SlotState.SENDING_WORLD;
            }

			var ctx = new HookContext() { Connection = conn, Player = conn.Player };
			var args = new HookArgs.WorldRequestMessage()
			{
				SpawnX = Main.spawnTileX,
				SpawnY = Main.spawnTileY
			};
			HookPoints.WorldRequestMessage.Invoke(ref ctx, ref args);

            //NetMessage.SendData(7, whoAmI);
			var msg = NetMessage.PrepareThreadInstance();
			msg.WorldData(args.SpawnX, args.SpawnY);
			msg.Send(whoAmI);
        }
    }
}
