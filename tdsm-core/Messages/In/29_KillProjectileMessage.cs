using System;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class KillProjectileMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.KILL_PROJECTILE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            //TODO [KillProjectileReceived]


            int num79 = (int)ReadInt16(readBuffer);
            int num80 = (int)ReadByte(readBuffer);

            num80 = whoAmI;

            for (int num81 = 0; num81 < 1000; num81++)
            {
                if (Main.projectile[num81].owner == num80 && Main.projectile[num81].identity == num79 && Main.projectile[num81].active)
                {
                    Main.projectile[num81].Kill();
                    break;
                }
            }

            NewNetMessage.SendData(29, -1, whoAmI, String.Empty, num79, (float)num80, 0f, 0f, 0);
        }
    }
}