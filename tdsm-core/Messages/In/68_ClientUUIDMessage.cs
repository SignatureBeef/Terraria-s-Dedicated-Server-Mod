using System;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class ClientUUIDMessage : MessageHandler
    {
        public ClientUUIDMessage()
        {
            ValidStates = SlotState.ACCEPTED | SlotState.ASSIGNING_SLOT;
        }

        public override Packet GetPacket()
        {
            return Packet.PACKET_68;
        }

        public override void Process(ClientConnection conn, byte[] readBuffer, int length, int num)
        {

            conn.Player.ClientUUId = ReadString(readBuffer);
        }
    }
}
