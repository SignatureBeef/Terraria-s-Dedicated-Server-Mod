using System;

namespace Terraria_Server.Messages
{
    public class UpdatePlayersMessage : SlotMessageHandler
    {
		public UpdatePlayersMessage ()
		{
			IgnoredStates = SlotState.ALL;
		}

        public override Packet GetPacket()
        {
            return Packet.UPDATE_PLAYERS;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            //var msg = NetMessage.PrepareThreadInstance();
            //msg.SendSyncOthersForPlayer (whoAmI);
        }
    }
}
