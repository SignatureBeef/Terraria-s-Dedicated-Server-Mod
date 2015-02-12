using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using tdsm.api;
using tdsm.api.Command;
using tdsm.api.Misc;
using tdsm.api.Permissions;
using tdsm.api.Plugin;
using tdsm.core;
using tdsm.core.ServerCore;
using Terraria;

namespace RestrictPlugin
{
    public partial class RestrictPlugin : BasePlugin
    {
        class RegistrationRequest
        {
            public string name;
            public string address;
            public string password;
        }

        class PropertiesFile : DataRegister
        {
            public PropertiesFile(string path)
                : base(path)
            {

            }

            public T GetValue<T>(string key, T defaultValue)
            {
                var item = base.Find(key);
                if (item != null)
                {

                }
                return defaultValue;
            }
        }

        PropertiesFile properties;
        PropertiesFile users;

        Dictionary<int, RegistrationRequest> requests;
        int requestCount = 0;

        bool allowGuests
        {
            get { return properties.GetValue("allow-guests", false); }
        }

        bool restrictGuests
        {
            get { return properties.GetValue("restrict-guests", true); }
        }

        bool restrictGuestsDoors
        {
            get { return properties.GetValue("restrict-guests-doors", true); }
        }

        bool restrictGuestsNPCs
        {
            get { return properties.GetValue("restrict-guests-npcs", true); }
        }

        string serverId
        {
            get { return properties.GetValue("server-id", "tdsm"); }
        }

        public Node ChestBreak;
        public Node ChestOpen;
        public Node DoorChange;
        public Node LiquidFlow;
        public Node NpcHurt;
        public Node ProjectileUse;
        public Node SignEdit;
        public Node WorldAlter;

        public RestrictPlugin()
        {
            Name = "Restrict";
            Description = "Restrict access to the server or character names.";
            Author = "UndeadMiner";
            Version = "0.38.0";
            TDSMBuild = 38;
        }

        protected override void Initialized(object state)
        {
            //Probably should check for existing login systems, But i'm not sure what undead would prefer atm.
            //Server.UsingLoginSystem = true;

            requests = new Dictionary<int, RegistrationRequest>();

            string pluginFolder = Globals.PluginPath + Path.DirectorySeparatorChar + "Restrict";

            CreateDirectory(pluginFolder);

            properties = new PropertiesFile(pluginFolder + Path.DirectorySeparatorChar + "restrict.properties");
            properties.Load();
            var dummy1 = allowGuests;
            var dummy2 = restrictGuests;
            var dummy3 = restrictGuestsDoors;
            var dummy4 = serverId;
            var dummy5 = restrictGuestsNPCs;
            properties.Save();

            users = new PropertiesFile(pluginFolder + Path.DirectorySeparatorChar + "restrict_users.properties");
            users.Load();
            users.Save();

            AddCommand("ru")
                .WithDescription("Register users or change their accounts")
                .WithHelpText("Adding users or changing passwords:")
                .WithHelpText("    ru [-o] [-f] <name> <hash>")
                .WithHelpText("    ru [-o] [-f] <name> -p <password>")
                .WithHelpText("Changing op status:")
                .WithHelpText("    ru -o [-f] <name>")
                .WithHelpText("    ru    [-f] <name>")
                .WithHelpText("Options:")
                .WithHelpText("    -o    make the player an operator")
                .WithHelpText("    -f    force action even if player isn't online")
                .WithPermissionNode("restrict.ru")
                .Calls(LockUsers<ISender, ArgumentList>(this.RegisterCommand));

            AddCommand("ur")
                .WithDescription("Unregister users")
                .WithHelpText("Deleting users:")
                .WithHelpText("    ur [-f] <name>")
                .WithHelpText("Options:")
                .WithHelpText("    -f    force action even if player isn't online")
                .WithPermissionNode("restrict.ur")
                .Calls(LockUsers<ISender, ArgumentList>(this.UnregisterCommand));

            AddCommand("ro")
                .WithDescription("Configure Restrict")
                .WithHelpText("Displaying options:")
                .WithHelpText("    ro")
                .WithHelpText("Setting options:")
                .WithHelpText("    ro [-f] [-g {true|false}] [-r {true|false}] [-s <serverId>] [-L]")
                .WithHelpText("Options:")
                .WithHelpText("    -f    force action")
                .WithHelpText("    -g    allow guests to enter the game")
                .WithHelpText("    -r    restrict guests' ability to alter tiles")
                .WithHelpText("    -s    set the server identifier used in hashing passwords")
                .WithHelpText("    -L    reload the user database from disk")
                .WithPermissionNode("restrict.ro")
                .Calls(LockUsers<ISender, ArgumentList>(this.OptionsCommand));

            AddCommand("rr")
                .WithDescription("Manage registration requests")
                .WithHelpText("Usage: rr          list registration requests")
                .WithHelpText("       rr -g #     grant a registration request")
                .WithHelpText("       rr grant #")
                .WithHelpText("       rr -d #     deny a registration request")
                .WithHelpText("       rr deny #")
                .WithPermissionNode("restrict.rr")
                .Calls(LockUsers<ISender, ArgumentList>(this.RequestsCommand));

            AddCommand("pass")
                .WithDescription("Change your password")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithHelpText("Usage: /pass yourpassword")
                .WithPermissionNode("restrict.pass")
                .Calls(LockUsers<ISender, string>(this.PlayerPassCommand));

            AddCommand("reg")
                .WithDescription("Submit a registration request")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithHelpText("Usage: /reg yourpassword")
                .WithPermissionNode("restrict.reg")
                .Calls(LockUsers<ISender, string>(this.PlayerRegCommand));

            ChestBreak = AddAndCreateNode("restrict.chestbreak");
            ChestOpen = AddAndCreateNode("restrict.chestopen");
            DoorChange = AddAndCreateNode("restrict.doorchange");
            LiquidFlow = AddAndCreateNode("restrict.liquidflow");
            NpcHurt = AddAndCreateNode("restrict.npchurt");
            ProjectileUse = AddAndCreateNode("restrict.projectileuse");
            SignEdit = AddAndCreateNode("restrict.signedit");
            WorldAlter = AddAndCreateNode("restrict.worldalter");
        }

        Action<T, U> LockUsers<T, U>(Action<T, U> callback)
        {
            return delegate(T t, U u)
            {
                lock (users)
                {
                    callback(t, u);
                }
            };
        }

        protected override void Disposed(object state)
        {

        }

        protected override void Enabled()
        {
            Tools.WriteLine(base.Name + " enabled.");
        }

        protected override void Disabled()
        {
            Tools.WriteLine(base.Name + " disabled.");
        }

        [Hook(HookOrder.EARLY)]
        void OnPlayerDataReceived(ref HookContext ctx, ref HookArgs.PlayerDataReceived args)
        {
            ctx.SetKick("Malfunction during login process, try again.");

            if (!args.NameChecked)
            {
                string error;
                if (!args.CheckName(out error))
                {
                    ctx.SetKick(error);
                    return;
                }
            }

            var player = ctx.Player;
            if (player == null)
            {
                Tools.WriteLine("Null player passed to Restrict.OnPlayerDataReceived.");
                return;
            }

            var name = args.Name;
            var pname = NameTransform(name);
            var oname = OldNameTransform(name);
            string entry = null;

            lock (users)
            {
                entry = users.Find(pname) ?? users.Find(oname);
            }

            if (entry == null)
            {
                if (allowGuests)
                {
                    ctx.SetResult(HookResult.DEFAULT);
                    player.SetAuthenticatedAs(null);
                    (ctx.Connection as ClientConnection).DesiredQueue = 0; //(int)LoginPriority.QUEUE_LOW_PRIO;

                    Tools.WriteLine("<Restrict> Letting user {0} from {1} in as guest.", name, player.IPAddress);
                }
                else
                {
                    Tools.WriteLine("<Restrict> Unregistered user {0} from {1} attempted to connect.", name, player.IPAddress);
                    ctx.SetKick("Only registered users are allowed.");
                    return;
                }
                return;
            }

            Tools.WriteLine("<Restrict> Expecting password for user {0} from {1}.", name, player.IPAddress);
            ctx.SetResult(HookResult.ASK_PASS);
        }

        [Hook(HookOrder.EARLY)]
        void OnPlayerPassReceived(ref HookContext ctx, ref HookArgs.PlayerPassReceived args)
        {
            ctx.SetKick("Malfunction during login process, try again.");

            var player = ctx.Player;
            if (player == null)
            {
                Tools.WriteLine("Null player passed to Restrict.OnPlayerPassReceived.");
                return;
            }

            var name = player.Name;
            var pname = NameTransform(name);
            var oname = OldNameTransform(name);
            string entry = null;

            lock (users)
            {
                entry = users.Find(pname) ?? users.Find(oname);
            }

            if (entry == null)
            {
                if (allowGuests)
                {
                    ctx.SetResult(HookResult.DEFAULT);
                    player.SetAuthenticatedAs(null);
                    (ctx.Connection as ClientConnection).DesiredQueue = 0;
                }
                else
                    ctx.SetKick("Only registered users are allowed.");

                return;
            }

            var split = entry.Split(':');
            var hash = split[0];
            var hash2 = Hash(name, args.Password);

            String.Format("User: {0}, Pass: {1}, Hash: {3}, Hash2: {2}", name, args.Password, hash2, hash);

            if (hash != hash2)
            {
                ctx.SetKick("Incorrect password for user: " + name);
                return;
            }

            if (split.Length > 1 && split[1] == "op")
            {
                player.Op = true;
                //TOD ctx.Connection.DesiredQueue = 3;
            }
            else
            {
                //TOD ctx.Connection.DesiredQueue = 1;
            }

            player.SetAuthenticatedAs(name);
            ctx.SetResult(HookResult.DEFAULT);
        }

        [Hook(HookOrder.TERMINAL)]
        void OnJoin(ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
        {
            var player = ctx.Player;

            if (player.Name == null) return;

            if (player.GetAuthenticatedAs() == null)
                player.Message(255, "You are a guest, to register type: /reg yourpassword");
            else if (player.Op)
                player.Message(255, new Color(128, 128, 255), "This humble server welcomes back Their Operating Highness.");
            else
                player.Message(255, new Color(128, 255, 128), "Welcome back, registered user.");

            return;
        }

        //		public override void onPlayerLogout (PlayerLogoutEvent ev)
        //		{
        //			if (ev.Player.Name == null) return;
        //
        //			PlayerRecord record;
        //			if (records.TryGetValue (ev.Player.Name, out record))
        //			{
        //				records.Remove (ev.Player.Name);
        //			}
        //		}

        [Hook(HookOrder.EARLY)]
        void OnSignTextSet(ref HookContext ctx, ref HookArgs.SignTextSet args)
        {
            var player = ctx.Player;

            if (player == null || player.Name == null)
            {
                Tools.WriteLine("<Restrict> Invalid player in OnSignTextSet.");
                ctx.SetResult(HookResult.IGNORE);
                return;
            }

            if (!restrictGuests) return;

            if (player.GetAuthenticatedAs() == null)
            {
                ctx.SetResult(HookResult.IGNORE);
                player.SendMessage("<Restrict> You are not allowed to edit signs as a guest.");
                player.SendMessage("<Restrict> Type \"/reg password\" to request registration.");
            }
            else if (IsRestrictedForUser(ctx.Player, SignEdit))
            {
                ctx.SetResult(HookResult.IGNORE);
                player.SendMessage("<Restrict> You are not allowed to edit signs without permissions.");
            }
        }

        [Hook(HookOrder.EARLY)]
        void OnAlter(ref HookContext ctx, ref HookArgs.PlayerWorldAlteration args)
        {
            var player = ctx.Player;
            //TODO
            //if (player == null && ctx.Sender is Projectile)
            //    player = (ctx.Sender as Projectile).Creator as Player;

            if (player == null || player.Name == null)
            {
                Tools.WriteLine("<Restrict> Invalid player in OnAlter.");
                ctx.SetResult(HookResult.IGNORE);
                return;
            }

            if (!restrictGuests) return;

            if (player.GetAuthenticatedAs() == null)
            {
                ctx.SetResult(HookResult.RECTIFY);
                player.SendMessage("<Restrict> You are not allowed to alter the world as a guest.");
                player.SendMessage("<Restrict> Type \"/reg password\" to request registration.");
            }
            else if (IsRestrictedForUser(ctx.Player, WorldAlter))
            {
                ctx.SetResult(HookResult.RECTIFY);
                player.SendMessage("<Restrict> You are not allowed to alter the world without permissions.");
            }
        }

        [Hook(HookOrder.EARLY)]
        void OnChestBreak(ref HookContext ctx, ref HookArgs.ChestBreakReceived args)
        {
            var player = ctx.Player;

            if (player == null || player.Name == null)
            {
                Tools.WriteLine("<Restrict> Invalid player in OnChestBreak.");
                ctx.SetResult(HookResult.IGNORE);
                return;
            }

            if (!restrictGuests) return;

            if (player.GetAuthenticatedAs() == null)
            {
                ctx.SetResult(HookResult.RECTIFY);
                player.SendMessage("<Restrict> You are not allowed to alter the world as a guest.");
                player.SendMessage("<Restrict> Type \"/reg password\" to request registration.");
            }
            else if (IsRestrictedForUser(ctx.Player, ChestBreak))
            {
                ctx.SetResult(HookResult.RECTIFY);
                player.SendMessage("<Restrict> You are not allowed to alter the world without permissions.");
            }
        }

        [Hook(HookOrder.EARLY)]
        void OnChestOpen(ref HookContext ctx, ref HookArgs.ChestOpenReceived args)
        {
            var player = ctx.Player;

            if (player == null || player.Name == null)
            {
                Tools.WriteLine("<Restrict> Invalid player in OnChestOpen.");
                ctx.SetResult(HookResult.IGNORE);
                return;
            }

            if (!restrictGuests) return;

            if (player.GetAuthenticatedAs() == null)
            {
                ctx.SetResult(HookResult.IGNORE);
                player.SendMessage("<Restrict> You are not allowed to open chests as a guest.");
                player.SendMessage("<Restrict> Type \"/reg password\" to request registration.");
            }
            else if (IsRestrictedForUser(ctx.Player, ChestOpen))
            {
                ctx.SetResult(HookResult.RECTIFY);
                player.SendMessage("<Restrict> You are not allowed to open chests without permissions.");
            }
        }

        [Hook(HookOrder.LATE)]
        void OnLiquidFlow(ref HookContext ctx, ref HookArgs.LiquidFlowReceived args)
        {
            var player = ctx.Player;

            if (player == null || player.Name == null)
            {
                Tools.WriteLine("<Restrict> Invalid player in OnLiquidFlow.");
                ctx.SetResult(HookResult.IGNORE);
                return;
            }

            if (!restrictGuests) return;

            if (player.GetAuthenticatedAs ()== null)
            {
                ctx.SetResult(HookResult.RECTIFY);
                player.SendMessage("<Restrict> You are not allowed to alter the world as a guest.");
                player.SendMessage("<Restrict> Type \"/reg password\" to request registration.");
            }
            else if (IsRestrictedForUser(ctx.Player, LiquidFlow))
            {
                ctx.SetResult(HookResult.RECTIFY);
                player.SendMessage("<Restrict> You are not allowed to alter the world without permissions.");
            }
        }

        [Hook(HookOrder.EARLY)]
        void OnProjectile(ref HookContext ctx, ref HookArgs.ProjectileReceived args)
        {
            var player = ctx.Player;
            //TODO
            //if (player == null && ctx.Sender is Projectile)
            //    player = (ctx.Sender as Projectile).Creator as Player;

            if (player == null || player.Name == null)
            {
                Tools.WriteLine("<Restrict> Invalid player in OnProjectile.");
                ctx.SetResult(HookResult.IGNORE);
                return;
            }

            if (!restrictGuests) return;

            //if (player.GetAuthenticatedAs() == null)
            {
                /* TODO
				switch (args.Type)
				{
					case ProjectileType.N10_PURIFICATION_POWDER:
					case ProjectileType.N11_VILE_POWDER:
					case ProjectileType.N28_BOMB:
					case ProjectileType.N37_STICKY_BOMB:
					case ProjectileType.N29_DYNAMITE:
					case ProjectileType.N30_GRENADE:
					case ProjectileType.N31_SAND_BALL:
					case ProjectileType.N39_MUD_BALL:
					case ProjectileType.N40_ASH_BALL:
					case ProjectileType.N42_SAND_BALL:
					case ProjectileType.N43_TOMBSTONE:
					case ProjectileType.N50_GLOWSTICK:
					case ProjectileType.N53_STICKY_GLOWSTICK:
						ctx.SetResult (HookResult.ERASE);
                        if (player.GetAuthenticatedAs() == null)
                        {
                            player.SendMessage("<Restrict> You are not allowed to use this projectile as a guest.");
                            player.SendMessage("<Restrict> Type \"/reg password\" to request registration.");
                        }
                        else if (IsRestrictedForUser(ctx.Player, ProjectileUse))
                            player.SendMessage("<Restrict> You are not allowed to use this projectile without permissions.");

						return;
					default:
						break;
				}*/
            }

            return;
        }

        [Hook(HookOrder.EARLY)]
        void OnDoorStateChanged(ref HookContext ctx, ref HookArgs.DoorStateChanged args)
        {
            if ((!restrictGuests) || (!restrictGuestsDoors)) return;

            var player = ctx.Player;

            if (player == null) return;

            if (player.GetAuthenticatedAs() == null)
            {
                ctx.SetResult(HookResult.RECTIFY);
                player.SendMessage("<Restrict> You are not allowed to open and close doors as a guest.");
                player.SendMessage("<Restrict> Type \"/reg password\" to request registration.");
            }
            else if (IsRestrictedForUser(ctx.Player, DoorChange))
            {
                ctx.SetResult(HookResult.RECTIFY);
                player.SendMessage("<Restrict> You are not allowed to open and close doors without permissions.");
            }
        }

        [Hook(HookOrder.EARLY)]
        void OnNPCHurt(ref HookContext ctx, ref HookArgs.NpcHurt args)
        {
            if ((!restrictGuests) || (!restrictGuestsNPCs)) return;

            var player = ctx.Player;

            if (player == null) return;

             if (player.GetAuthenticatedAs() == null)
            {
                ctx.SetResult(HookResult.IGNORE);
                player.SendMessage("<Restrict> You are not allowed to hurt NPCs as a guest.");
                player.SendMessage("<Restrict> Type \"/reg password\" to request registration.");
            }
            else if (IsRestrictedForUser(ctx.Player, NpcHurt))
            {
                ctx.SetResult(HookResult.IGNORE);
                player.SendMessage("<Restrict> You are not allowed to hurt NPCs without permissions.");
            }
        }

        //TODO
        //#region Permissions
        //        public bool IsRunningPermissions()
        //        {
        //            return Program.permissionManager.IsPermittedImpl != null;
        //        }

                public bool IsRestrictedForUser(Player player, Node node)
                {
        //            if (IsRunningPermissions())
        //            {
        //                var isPermitted = Program.permissionManager.IsPermittedImpl(node.Path, player);
        //                var isOp        = player.Op;

        //                return !isPermitted && !isOp;
        //            }

                    return !player.Op;
                }
        //#endregion
    }
}
