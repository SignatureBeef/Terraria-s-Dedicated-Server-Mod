using tdsm.api.Plugin;
namespace tdsm.api.Callbacks
{
    public static class NPCCallback
    {
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
    }
}
