using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;
using Microsoft.Xna.Framework;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class ProjectileReceived : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.PROJECTILE; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            
            int identity = (int)buffer.reader.ReadInt16();
            Vector2 position = buffer.reader.ReadVector2();
            Vector2 velocity = buffer.reader.ReadVector2();
            float knockBack = buffer.reader.ReadSingle();
            int damage = (int)buffer.reader.ReadInt16();
            int owner = (int)buffer.reader.ReadByte();
            int type = (int)buffer.reader.ReadInt16();
            BitsByte flags = buffer.reader.ReadByte();
            float[] ai = new float[Projectile.maxAI];
            
            for (int i = 0; i < Projectile.maxAI; i++)
            {
                if (flags[i])
                {
                    ai[i] = buffer.reader.ReadSingle();
                }
                else
                {
                    ai[i] = 0;
                }
            }
            int uuid = (int)(flags[Projectile.maxAI] ? buffer.reader.ReadInt16() : -1);
            if (uuid >= 1000)
            {
                uuid = -1;
            }
            if (Main.netMode == 2)
            {
                owner = bufferId;
                if (Main.projHostile[type])
                {
                    return true;
                }
            }
            int index = 1000;
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].owner == owner && Main.projectile[i].identity == identity && Main.projectile[i].active)
                {
                    index = i;
                    break;
                }
            }
            if (index == 1000)
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (!Main.projectile[i].active)
                    {
                        index = i;
                        break;
                    }
                }
            }
            
            var player = Main.player[bufferId];
            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Player = player,
                Sender = player,
            };
            
            var args = new TDSMHookArgs.ProjectileReceived
            {
                Position = position,
                Velocity = velocity,
                Id = identity,
                Owner = bufferId,
                Knockback = knockBack,
                Damage = damage,
                Type = type,
                AI = ai,
                ExistingIndex = index < 1000 ? index : -1
            };
            
            TDSMHookPoints.ProjectileReceived.Invoke(ref ctx, ref args);
            
            if (ctx.Result == HookResult.DEFAULT)
            {
                if (index > -1 && index < 1000)
                {
                    Projectile projectile = Main.projectile[index];
                    if (!projectile.active || projectile.type != type)
                    {
                        projectile.SetDefaults(type);
                        if (Main.netMode == 2)
                        {
                            Netplay.Clients[bufferId].SpamProjectile += 1;
                        }
                    }
                    projectile.identity = identity;
                    projectile.position = position;
                    projectile.velocity = velocity;
                    projectile.type = type;
                    projectile.damage = damage;
                    projectile.knockBack = knockBack;
                    projectile.owner = owner;
                    for (int i = 0; i < Projectile.maxAI; i++)
                    {
                        projectile.ai[i] = ai[i];
                    }
                    if (uuid >= 0)
                    {
                        projectile.projUUID = uuid;
                        Main.projectileIdentity[owner, uuid] = index;
                    }
                    projectile.ProjectileFixDesperation();
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(27, -1, bufferId, "", index, 0, 0, 0, 0, 0, 0);
                    }
                }
            }
            return true;
        }
    }
}

