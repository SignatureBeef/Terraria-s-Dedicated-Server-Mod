using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class KillProjectileMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.KILL_PROJECTILE;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            short identity = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte owner = readBuffer[num];
            
            owner = (byte)whoAmI;

            Projectile projectile;
            for (int i = 0; i < 1000; i++)
            {
                projectile = Main.projectile[i];
                if (projectile.Owner == (int)owner && projectile.identity == (int)identity && projectile.Active)
                {
                    projectile.Kill();
                    break;
                }
            }

            NetMessage.SendData(29, -1, whoAmI, "", (int)identity, (float)owner);
        }
    }
}
