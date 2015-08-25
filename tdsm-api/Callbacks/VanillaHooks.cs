using TDSM.API.Plugin;
using System;
using TDSM.API.Logging;


#if Full_API
using Terraria;
#endif
namespace TDSM.API.Callbacks
{
    #if !Full_API
    public class Player
    {

    }
    #endif

    public static class VanillaHooks
    {
        public static void OnPlayerEntering(Player player)
        {
            #if Full_API
            var ctx = new HookContext
            {
                Player = player,
                Sender = player
            };

            var args = new HookArgs.PlayerEnteringGame
            {
                Slot = player.whoAmI
            };

            ctx.SetResult(HookResult.DEFAULT, false);
            HookPoints.PlayerEnteringGame.Invoke(ref ctx, ref args);
            if (!ctx.CheckForKick())
            {
                NetMessage.SendData(4, -1, player.whoAmI, player.name, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
            }
            #endif
        }

        public static void OnPlayerLeave(Player player)
        {
            #if Full_API
            var ctx = new HookContext
            {
                Player = player,
                Sender = player
            };

            var args = new HookArgs.PlayerLeftGame
            {
                Slot = player.whoAmI
            };

            ctx.SetResult(HookResult.DEFAULT, false);
            HookPoints.PlayerLeftGame.Invoke(ref ctx, ref args);
            #endif
        }

        public static bool OnGreetPlayer(int playerId)
        {
            #if Full_API
            var player = Main.player[playerId];

            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Player = player,
                Sender = player
            };

            var args = new HookArgs.PlayerPreGreeting
            {
                Slot = playerId,
                Motd = String.IsNullOrEmpty(Main.motd) ? (Lang.mp[18] + " " + Main.worldName) : Main.motd,
                MotdColour = new Microsoft.Xna.Framework.Color(255, 240, 20)
            };

            HookPoints.PlayerPreGreeting.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
            {
                return false;
            }

            if (ctx.Result == HookResult.DEFAULT)
            {
                player.SendMessage(args.Motd, 255, args.MotdColour.R, args.MotdColour.G, args.MotdColour.B);

                string list = "";
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active)
                    {
                        if (list == "")
                            list += Main.player[i].name;
                        else
                            list = list + ", " + Main.player[i].Name;
                    }
                }

                player.SendMessage("Current players: " + list + ".", 255, 255, 240, 20);
            }

            ProgramLog.Users.Log("{0} @ {1}: ENTER {2}", Netplay.Clients[playerId].Socket.GetRemoteAddress(), playerId, player.name);

            var args2 = new HookArgs.PlayerEnteredGame
            {
                Slot = playerId
            };

            ctx.SetResult(HookResult.DEFAULT, false);
            HookPoints.PlayerEnteredGame.Invoke(ref ctx, ref args2);
            ctx.CheckForKick();
            #endif

            return false; //We implemented our own, so do not continue on with vanilla
        }

        public static string OnDeathMessage(int plr = -1, int npc = -1, int proj = -1, int other = -1)
        {
//            ProgramLog.Log("DEATH");
//            return "DEATH";
            var args = new HookArgs.DeathMessage()
            {
                Player = plr,
                NPC = npc,
                Projectile = proj,
                Other = other
            };

            Terraria.Player player = null;
            TDSM.API.Command.ISender sender = null;

            if (plr > -1 && plr < Terraria.Main.player.Length)
            {
                player = Terraria.Main.player[plr];
                sender = player;
            }
            if (npc > -1 && npc < Terraria.Main.npc.Length) sender = Terraria.Main.npc[npc];
            if (proj > -1 && proj < Terraria.Main.projectile.Length) sender = Terraria.Main.projectile[proj];

            var ctx = new HookContext()
            {
                Sender = sender,
                Player = player
            };

            HookPoints.DeathMessage.Invoke(ref ctx, ref args);

            var msg = ctx.ResultParam as string;
            if (msg == null) return Terraria.Lang.deathMsg(args.Player, args.NPC, args.Projectile, args.Other);
            return msg;
        }
    }
}
