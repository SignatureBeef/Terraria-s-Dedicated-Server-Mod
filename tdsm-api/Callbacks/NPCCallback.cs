using tdsm.api.Plugin;
namespace tdsm.api.Callbacks
{
    public static class NPCCallback
    {
        private static int _invasionTypeCounter = 3;

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
        }
    }
}
