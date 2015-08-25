using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class HitSwitchMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.HIT_SWITCH;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int x = (int)ReadInt16(readBuffer);
            int y = (int)ReadInt16(readBuffer);
            Wiring.HitSwitch(x, y);
            if (Main.netMode == 2)
            {
                NewNetMessage.SendData(59, -1, whoAmI, String.Empty, x, (float)y, 0f, 0f, 0);
                return;
            }
            return;
        }
    }
}
