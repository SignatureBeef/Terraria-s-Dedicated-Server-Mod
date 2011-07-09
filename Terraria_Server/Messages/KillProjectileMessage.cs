using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class KillProjectileMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.KILL_PROJECTILE;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            short identity = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            byte owner = readBuffer[num];
            
            owner = (byte)whoAmI;

            Projectile projectile;
            for (int i = 0; i < 1000; i++)
            {
                projectile = Main.projectile[i];
                if (projectile.Owner == (int)owner && projectile.identity == (int)identity && projectile.active)
                {
                    projectile.Kill();
                    break;
                }
            }

            NetMessage.SendData(29, -1, whoAmI, "", (int)identity, (float)owner);
        }
    }
}
