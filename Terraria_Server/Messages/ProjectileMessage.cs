using System;
using Terraria_Server.Events;
using Terraria_Server.Plugins;
using Terraria_Server.Definitions;
using Terraria_Server.Collections;
using Terraria_Server.Logging;

namespace Terraria_Server.Messages
{
    public class ProjectileMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PROJECTILE;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
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
            
			var player = Main.players[whoAmI];
			
			var ctx = new HookContext
			{
				Connection = player.Connection,
				Player = player,
				Sender = player,
			};
			
			var args = new HookArgs.ProjectileReceived
			{
				X = x, Y = y, VX = vX, VY = vY,
				Id = projectileIdentity,
				Owner = projectileOwner,
				Knockback = knockBack,
				Damage = damage,
				TypeByte = type,
			};
			
			HookPoints.ProjectileReceived.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick ())
				return;
			
			if (ctx.Result == HookResult.IGNORE)
				return;
			
			if (ctx.Result == HookResult.ERASE)
			{
				var msg = NetMessage.PrepareThreadInstance ();
				msg.EraseProjectile (projectileIdentity, projectileOwner);
				msg.Send (whoAmI);
				return;
			}
			
			if (ctx.Result != HookResult.CONTINUE)
			{
				if (type > 55)
				{
					Netplay.slots[whoAmI].Kick ("Invalid projectile.");
					return;
				}
				else if (type == (int)ProjectileType.FEATHER_HARPY || type == (int)ProjectileType.STINGER || type == (int)ProjectileType.SICKLE_DEMON)
				{
					Netplay.slots[whoAmI].Kick ("Projectile cheat detected.");
					return;
				}
				else if (type == (int)ProjectileType.HARPOON)
				{
					if (Math.Abs (vX) + Math.Abs (vY) < 1e-4) // ideally, we'd want to figure out all projectiles that never have 0 velocity
					{
						Netplay.slots[whoAmI].Kick ("Harpoon cheat detected.");
						return;
					}
				}
			}
			
			var projectile = Registries.Projectile.Create(type);
			
			for (int i = 0; i < Projectile.MAX_AI; i++)
			{
				projectile.ai[i] = BitConverter.ToSingle(readBuffer, num);
				num += 4;
			}
            
            int projectileIndex = getProjectileIndex(projectileOwner, projectileIdentity);
            Projectile oldProjectile = Main.projectile[projectileIndex];
            if (!oldProjectile.Active || projectile.type != oldProjectile.type)
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
            //Program.server.PluginManager.processHook(Hooks.PLAYER_PROJECTILE, playerEvent);
            if (playerEvent.Cancelled || (!Program.properties.AllowExplosions && 
                (   type == (int)ProjectileType.BOMB        /* 28 */ || 
                    type == (int)ProjectileType.DYNAMITE    /* 29 */ ||
                    type == (int)ProjectileType.BOMB_STICKY /* 37 */
                ) && !Main.players[whoAmI].Op))
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
