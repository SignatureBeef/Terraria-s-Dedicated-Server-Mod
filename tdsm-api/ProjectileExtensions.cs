using Terraria;

namespace tdsm.api
{
    public static class ProjectileExtensions
    {
        public static bool IsHighExplosive(this Projectile prj)
        {
            return prj.type == 29 ||
                    prj.type == 102 ||
                    prj.type == 37 ||
                    prj.type == 108;
        }

        public static bool IsExplosive(this Projectile prj)
        {
            return IsHighExplosive(prj) ||
                    prj.type == 30 ||
                    prj.type == 41;
        }
    }
}
