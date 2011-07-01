using System;

namespace Terraria_Server.Messages
{
    public class ProjectileMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PROJECTILE;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            short projectileIdentity = BitConverter.ToInt16(readBuffer, num);
            num += 2;
            float x = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float y = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float vX = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float vY = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            float knockBack = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            short damage = BitConverter.ToInt16(readBuffer, num);
            num += 2;

            byte projectileOwner = readBuffer[num++];
            byte type = readBuffer[num++];

            float[] aiInfo = new float[Projectile.MAX_AI];
            for (int i = 0; i < Projectile.MAX_AI; i++)
            {
                aiInfo[i] = BitConverter.ToSingle(readBuffer, num);
                num += 4;
            }
            
            int projectileIndex = getProjectileIndex(projectileOwner, projectileIdentity);
            Projectile projectile = Main.projectile[projectileIndex];
            if (!projectile.active || projectile.type != (int)type)
            {
                projectile.SetDefaults((int)type);
                if (Main.netMode == 2)
                {
                    Netplay.serverSock[whoAmI].spamProjectile += 1f;
                }
            }

            projectile.identity = (int)projectileIdentity;
            projectile.position.X = x;
            projectile.position.Y = y;
            projectile.velocity.X = vX;
            projectile.velocity.Y = vY;
            projectile.damage = (int)damage;
            projectile.type = (int)type;
            projectile.owner = (int)projectileOwner;
            projectile.knockBack = knockBack;

            for (int i = 0; i < Projectile.MAX_AI; i++)
            {
                projectile.ai[i] = aiInfo[i];
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(27, -1, whoAmI, "", projectileIndex);
            }
        }


        private int getProjectileIndex(int owner, int identity)
        {
            int index = 1000;
            int firstInactive = index;
            Projectile projectile;
            for (int i = 0; i < index; i++)
            {
                projectile = Main.projectile[i];
                if (projectile.owner == owner
                    && projectile.identity == identity
                    && projectile.active)
                {
                    return i;
                }

                if (firstInactive == index && !projectile.active)
                {
                    firstInactive = i;
                }
            }

            return firstInactive;
        }
    }
}
