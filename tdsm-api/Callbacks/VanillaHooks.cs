
using tdsm.api.Plugin;
using Terraria;
namespace tdsm.api.Callbacks
{
    public static class VanillaHooks
    {
        public static void OnPlayerJoined(Player player)
        {
            var ctx = new HookContext
            {
                Connection = player.Connection,
                Player = player,
                Sender = player
            };

            var args = new HookArgs.PlayerEnteredGame
            {
                Slot = player.whoAmi
            };

            ctx.SetResult(HookResult.DEFAULT, false);
            HookPoints.PlayerEnteredGame.Invoke(ref ctx, ref args);
            ctx.CheckForKick();
        }
    }
}
