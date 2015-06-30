using System;
using tdsm.api;
using tdsm.api.Plugin;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class ProjectileMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PROJECTILE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            //TODO [ProjectileReceived]

            var identity = (int)ReadInt16(readBuffer);
            var position = ReadVector2(readBuffer);
            var velocity = ReadVector2(readBuffer);
            var knockBack = ReadSingle(readBuffer);
            var damage = (int)ReadInt16(readBuffer);
            /*var targetOwner =*/
            ReadByte(readBuffer);
            var type = (int)ReadInt16(readBuffer);
            var flags = (BitsByte)ReadByte(readBuffer);
            var ai = new float[Projectile.maxAI];

            for (var i = 0; i < Projectile.maxAI; i++)
            {
                if (flags[i]) ai[i] = ReadSingle(readBuffer);
                else ai[i] = 0f;
            }

            if (Main.projHostile[type]) return;

            var index = 1000;

            //Attempt to find the existing projectile.
            for (var i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].owner == whoAmI && Main.projectile[i].identity == identity && Main.projectile[i].active)
                {
                    index = i;
                    break;
                }
            }

            if (index == 1000)
            {
                //Find the next slot since there was no existing projectile.
                for (var i = 0; i < 1000; i++)
                {
                    if (!Main.projectile[i].active)
                    {
                        index = i;
                        break;
                    }
                }
            }

            var player = Main.player[whoAmI];
            var ctx = new HookContext
            {
                Connection = player.Connection,
                Player = player,
                Sender = player,
            };

            var args = new HookArgs.ProjectileReceived
            {
                Position = position,
                Velocity = velocity,
                Id = identity,
                Owner = whoAmI,
                Knockback = knockBack,
                Damage = damage,
                Type = type,
                AI = ai,
                ExistingIndex = index < 1000 ? index : -1
            };

            HookPoints.ProjectileReceived.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.DEFAULT)
            {
                if (index > -1 && index < 1000)
                {
                    var projectile = Main.projectile[index];
                    if (!projectile.active || projectile.type != type)
                    {
                        projectile.SetDefaults(type);
                        Terraria.Netplay.serverSock[whoAmI].spamProjectile += 1f;
                    }

                    projectile.identity = identity;
                    projectile.position = position;
                    projectile.velocity = velocity;
                    projectile.type = type;
                    projectile.damage = damage;
                    projectile.knockBack = knockBack;
                    projectile.owner = whoAmI;

                    for (var i = 0; i < Projectile.maxAI; i++)
                        projectile.ai[i] = ai[i];

                    NewNetMessage.SendData(Packet.PROJECTILE, -1, whoAmI, String.Empty, index);
                }
            }
        }
    }
}