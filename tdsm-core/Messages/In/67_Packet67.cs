namespace tdsm.core.Messages.In
{
    public class Packet67 : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PACKET_67;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num) { }
    }
}
