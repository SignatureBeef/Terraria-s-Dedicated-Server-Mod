using System;
using Terraria_Server.Events;
using Terraria_Server.Plugin;
using Terraria_Server.Definitions;
using Terraria_Server.Collections;
using Terraria_Server.Logging;

namespace Terraria_Server.Messages
{
    public class ProjectileMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PROJECTILE;
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
            
            if (type > 54)
            {
                Netplay.slots[whoAmI].Kick ("Invalid projectile.");
                return;
            }
            else if (type == (int)ProjectileType.FEATHER_HARPY)
            {
                Netplay.slots[whoAmI].Kick("Harpy feather cheat detected.");
                return;
            }
            else if (type == (int)ProjectileType.HARPOON)
            {
                if (damage == 99)
                    ProgramLog.Debug.Log ("{0} sent a HARPOON projectile with damage 99.", Main.players[whoAmI].Name);
                //Netplay.slots[whoAmI].Kick("Harpoon hack detected.");
                return;
            }

            float[] aiInfo = new float[Projectile.MAX_AI];
            for (int i = 0; i < Projectile.MAX_AI; i++)
            {
                aiInfo[i] = BitConverter.ToSingle(readBuffer, num);
                num += 4;
            }
            
            int projectileIndex = getProjectileIndex(projectileOwner, projectileIdentity);
            Projectile oldProjectile = Main.projectile[projectileIndex];
            Projectile projectile = Registries.Projectile.Create(type);
            if (!projectile.Active || projectile.type != oldProjectile.type)
            {
                Netplay.slots[whoAmI].spamProjectile += 1f;
            }

            projectile.identity = projectileIdentity;
            projectile.Position.X = x;
            projectile.Position.Y = y;
            projectile.Velocity.X = vX;
            projectile.Velocity.Y = vY;
            projectile.damage = damage;
            projectile.Owner = projectileOwner;
            projectile.knockBack = knockBack;

            PlayerProjectileEvent playerEvent = new PlayerProjectileEvent();
            playerEvent.Sender = Main.players[whoAmI];
            playerEvent.Projectile = projectile;
            Program.server.PluginManager.processHook(Hooks.PLAYER_PROJECTILE, playerEvent);
            if (playerEvent.Cancelled)
            {
                // erase the projectile client-side
                projectile.Position.X = -1000;
                projectile.Position.Y = -1000;
                projectile.type = ProjectileType.UNKNOWN;
                
                var msg = NetMessage.PrepareThreadInstance ();
                msg.Projectile (projectile);
                msg.Send (whoAmI);

                return;
            }

            Main.projectile[projectileIndex] = projectile;

            for (int i = 0; i < Projectile.MAX_AI; i++)
            {
                projectile.ai[i] = aiInfo[i];
            }
            NetMessage.SendData(27, -1, whoAmI, "", projectileIndex);
        }


        private int getProjectileIndex(int owner, int identity)
        {
            int index = 1000;
            int firstInactive = index;
            Projectile projectile;
            for (int i = 0; i < index; i++)
            {
                projectile = Main.projectile[i];
                if (projectile.Owner == owner
                    && projectile.identity == identity
                    && projectile.Active)
                {
                    return i;
                }

                if (firstInactive == index && !projectile.Active)
                {
                    firstInactive = i;
                }
            }

            return firstInactive;
        }
    }
}
