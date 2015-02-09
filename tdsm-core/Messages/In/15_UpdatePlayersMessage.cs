using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;

namespace tdsm.core.Messages.In
{
    public class UpdatePlayersMessage : SlotMessageHandler
    {
        public UpdatePlayersMessage()
        {
            IgnoredStates = SlotState.ALL;
        }

        public override Packet GetPacket()
        {
            return Packet.UPDATE_PLAYERS;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            var msg = NewNetMessage.PrepareThreadInstance();
            msg.SendSyncOthersForPlayer(whoAmI);
        }
    }
}
