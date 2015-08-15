using TDSM.API.Plugin;

namespace TDSM.API.Callbacks
{
    public static class NPCCallback
    {
        public static Rand CheckedRand = new Rand();

        private static int _invasionTypeCounter = 20;

        /// <summary>
        /// Returns a new invasion type that is dedicated for the callee
        /// </summary>
        /// <returns></returns>
        public static int AssignInvasionType()
        {
            return System.Threading.Interlocked.Increment(ref _invasionTypeCounter);
        }

        public static bool CanSpawnNPC(int x, int y, int type, int start = 0)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NPCSpawn()
            {
                X = x,
                Y = y,
                Type = type,
                Start = start
            };

            HookPoints.NPCSpawn.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnInvasionNPCSpawn(int x, int y)
        {
            var ctx = new HookContext();
            var args = new HookArgs.InvasionNPCSpawn()
            {
                X = x,
                Y = y
            };

            HookPoints.InvasionNPCSpawn.Invoke(ref ctx, ref args);

//            return ctx.Result == HookResult.DEFAULT;
        }

        #if Full_API
        public static void OnNPCKilled(Terraria.NPC npc)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NPCKilled()
            {
                Type = npc.type,
                NetId = npc.netID
            };

            HookPoints.NPCKilled.Invoke(ref ctx, ref args);
        }
        
#else
        public static void OnNPCKilled(object npc)
        {
        }
        #endif
    }
}
