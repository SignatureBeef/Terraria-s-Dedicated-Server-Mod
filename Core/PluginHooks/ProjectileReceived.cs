using System;
using OTA.Plugin;
using Microsoft.Xna.Framework;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        public struct ProjectileReceived
        {
            public int Id { get; set; }

            public Vector2 Position { get; set; }

            public Vector2 Velocity { get; set; }
            //public float X { get; set; }
            //public float Y { get; set; }
            //public float VX { get; set; }
            //public float VY { get; set; }
            public float Knockback { get; set; }

            public int Damage { get; set; }

            public int Owner { get; set; }

            public int Type { get; set; }

            public float[] AI { get; set; }

            public float AI_0
            {
                get { return AI[0]; }
                set { AI[0] = value; }
            }

            public float AI_1
            {
                get { return AI[01]; }
                set { AI[01] = value; }
            }

            public int ExistingIndex { get; set; }

            internal Terraria.Projectile projectile;

            public Terraria.Projectile CreateProjectile()
            {
                if (projectile != null)
                    return projectile;

                //                var index = Projectile.ReserveSlot(Id, Owner);
                //
                //                if (index == 1000) return null;
                //
                //                projectile = Registries.Projectile.Create(TypeByte);

                //                projectile.Id = index;
                //                Apply(projectile);

                return projectile;
            }

            public void Apply(Terraria.Projectile projectile)
            {
                //                if (Owner < 255)
                //                    projectile.Creator = Main.player[Owner];
                projectile.identity = Id;
                projectile.owner = Owner;
                projectile.damage = Damage;
                projectile.knockBack = Knockback;
                //                projectile.position = new Vector2(X, Y);
                //                projectile.velocity = new Vector2(VX, VY);
                projectile.ai = AI;
            }

            internal void CleanupProjectile()
            {
                if (projectile != null)
                {
                    //                    Projectile.FreeSlot(projectile.identity, projectile.Owner, projectile.Id);
                    projectile = null;
                }
            }

            //            public ProjectileType Type
            //            {
            //                get { return (ProjectileType)TypeByte; }
            //                set { TypeByte = (byte)value; }
            //            }

            public Terraria.Projectile Current
            {
                get { return Terraria.Main.projectile[Id]; }
            }
        }
    }

    public static partial class TDSMHookPoints
    {
        public static readonly HookPoint<TDSMHookArgs.ProjectileReceived> ProjectileReceived = new HookPoint<TDSMHookArgs.ProjectileReceived>();
    }
}