using Terraria;

namespace tdsm.core.Messages.In
{
    public class AnglerFinishRegister : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.ANGLER_FINISH_REGISTER;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            if (Main.netMode != 2)
            {
                return;
            }
            string name = Main.player[whoAmI].name;
            if (!Main.anglerWhoFinishedToday.Contains(name))
            {
                Main.anglerWhoFinishedToday.Add(name);
                return;
            }
            return;
        }
    }
}
