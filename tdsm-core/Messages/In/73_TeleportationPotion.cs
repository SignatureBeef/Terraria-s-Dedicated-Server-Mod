using tdsm.api;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class Packet73 : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.TELEPORTATION_POTION;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            Main.player[whoAmI].TeleportationPotion();
        }
    }
}
