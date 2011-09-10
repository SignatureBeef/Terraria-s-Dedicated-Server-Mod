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
            
			if (projectileOwner != whoAmI)
			{
				ProgramLog.Debug.Log ("Ignoring unowned projectile from {0} ({1})", whoAmI, projectileOwner);
			}
			
			if (Projectile.MAX_AI != 2)
			{
				throw new Exception ("Projectile receiving code hasn't been updated!");
			}
			
			var ai0 = BitConverter.ToSingle (readBuffer, num); num += 4;
			var ai1 = BitConverter.ToSingle (readBuffer, num);
			
			var player = Main.players[whoAmI];
			
			var projectileIndex = Projectile.FindExisting (projectileIdentity, projectileOwner);
			
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
				AI_0 = ai0, AI_1 = ai1,
				ExistingIndex = projectileIndex < 1000 ? projectileIndex : -1,
			};
			
			HookPoints.ProjectileReceived.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick ())
			{
				args.CleanupProjectile ();
				return;
			}
			
			if (ctx.Result == HookResult.IGNORE)
			{
				args.CleanupProjectile ();
				return;
			}
			
			if (ctx.Result == HookResult.RECTIFY)
			{
				args.CleanupProjectile ();
				
				if (projectileIndex < 1000)
				{
					var msg = NetMessage.PrepareThreadInstance ();
					msg.Projectile (Main.projectile[projectileIndex]);
					msg.Send (whoAmI);
					return;
				}
				else
				{
					ctx.SetResult (HookResult.ERASE);
				}
			}
			
			if (ctx.Result == HookResult.ERASE)
			{
				args.CleanupProjectile ();
				
				var msg = NetMessage.PrepareThreadInstance ();
				msg.EraseProjectile (projectileIdentity, projectileOwner);
				msg.Send (whoAmI);
				return;
			}
			
			if (ctx.Result != HookResult.CONTINUE)
			{
				if (type > 55)
				{
					args.CleanupProjectile ();
					NetPlay.slots[whoAmI].Kick ("Invalid projectile.");
					return;
				}
				else if (type == (int)ProjectileType.FEATHER_HARPY || type == (int)ProjectileType.STINGER || type == (int)ProjectileType.SICKLE_DEMON)
				{
					args.CleanupProjectile ();
					NetPlay.slots[whoAmI].Kick ("Projectile cheat detected.");
					return;
				}
				else if (type == (int)ProjectileType.HARPOON)
				{
					args.CleanupProjectile ();
					if (Math.Abs (vX) + Math.Abs (vY) < 1e-4) // ideally, we'd want to figure out all projectiles that never have 0 velocity
					{
						NetPlay.slots[whoAmI].Kick ("Harpoon cheat detected.");
						return;
					}
				}
			}
			
			Projectile projectile;
			
			if (args.ExistingIndex >= 0)
			{
				//ProgramLog.Debug.Log ("Updated projectile {0} ({1}/{2}/{3})", projectileIndex, projectileOwner, projectileIdentity, args.Type);
				args.CleanupProjectile ();
				projectile = Main.projectile[args.ExistingIndex];
				args.Apply (projectile);
			}
			else
			{
				projectile = args.CreateProjectile ();
				
				if (projectile == null)
				{
					//ProgramLog.Debug.Log ("No slots left for projectile ({1}/{2}/{3})", projectileOwner, projectileIdentity, args.Type);
					return;
				}
				
				projectileIndex = projectile.whoAmI;
				//ProgramLog.Debug.Log ("Created projectile {0} ({1}/{2}/{3})", projectileIndex, projectileOwner, projectileIdentity, args.Type);
			}
			
//            int projectileIndex = getProjectileIndex(projectileOwner, projectileIdentity);
//            Projectile oldProjectile = Main.projectile[projectileIndex];
//            if (!oldProjectile.Active || projectile.type != oldProjectile.type)
//            {
//                NetPlay.slots[whoAmI].spamProjectile += 1f;
//            }

//            if (playerEvent.Cancelled || (!Program.properties.AllowExplosions && 
//                (   type == (int)ProjectileType.BOMB        /* 28 */ || 
//                    type == (int)ProjectileType.DYNAMITE    /* 29 */ ||
//                    type == (int)ProjectileType.BOMB_STICKY /* 37 */
//                ) && !Main.players[whoAmI].Op))
//            {
//                // erase the projectile client-side
//                projectile.Position.X = -1000;
//                projectile.Position.Y = -1000;
//                projectile.type = ProjectileType.UNKNOWN;
//                
//                var msg = NetMessage.PrepareThreadInstance ();
//                msg.Projectile (projectile);
//                msg.Send (whoAmI);
//
//                return;
//            }

            Main.projectile[projectileIndex] = projectile;

            NetMessage.SendData(27, -1, whoAmI, "", projectileIndex);
        }
    }
}
