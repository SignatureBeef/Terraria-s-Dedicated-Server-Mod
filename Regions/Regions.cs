using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugin;
using Terraria_Server.Commands;
using Terraria_Server.Logging;
using Terraria_Server.Events;
using Terraria_Server.Misc;
using Terraria_Server.Definitions.Tile;
using Terraria_Server;
using System.IO;
using Regions.RegionWork;
using Terraria_Server.Definitions;
using Terraria_Server.Permissions;
using Regions.Nodes;

namespace Regions
{
    public class Regions : Plugin
    {
        public const Int32 SelectorItem = 0;
        public static Boolean UsingPermissions = false;

        public static String RegionsFolder
        {
            get
            {
                return Statics.PluginPath + Path.DirectorySeparatorChar + "Regions";
            }
        }
        public static String DataFolder
        {
            get
            {
                return RegionsFolder + Path.DirectorySeparatorChar + "Data";
            }
        }

        public static Properties rProperties { get; set; }
        public static RegionManager regionManager { get; set; }
        private static Boolean SelectorPos = true; //false for 1st (mousePoints[0]), true for 2nd

        public Node DoorChange;
        public Node LiquidFlow;
        public Node TileBreak;
        public Node TilePlace;
        public Node ProjectileUse;
        
        public override void Load()
        {
            base.Name = "Regions";
            base.Description = "A region plugin for TDSM";
            base.Author = "DeathCradle";
            base.Version = "1";
            base.TDSMBuild = 32;

            if (!Directory.Exists(RegionsFolder))
                Directory.CreateDirectory(RegionsFolder);

            rProperties = new Properties(RegionsFolder + Path.DirectorySeparatorChar + "regions.properties");
            rProperties.Load();
            rProperties.pushData();
            rProperties.Save();

            regionManager = new RegionManager(DataFolder);

            AddCommand("region")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("Usage:    region select")
                .WithDescription("Region Management.")
                .Calls(Commands.Region);

            registerHook(Hooks.PLAYER_TILECHANGE);
            registerHook(Hooks.PLAYER_FLOWLIQUID);
            registerHook(Hooks.PLAYER_PROJECTILE);
            registerHook(Hooks.DOOR_STATECHANGE);

            UsingPermissions = isRunningPermissions();
            if (UsingPermissions)
                Log("Using Permissions.");
            else
                Log("No Permissions Found, Using Internal User System");
        }

        public override void Enable()
        {
            DoorChange      = new DoorChange    ().GetNode();
            LiquidFlow      = new LiquidFlow    ().GetNode();
            TileBreak       = new TileBreak     ().GetNode();
            TilePlace       = new TilePlace     ().GetNode();
            ProjectileUse   = new ProjectileUse ().GetNode();

            ProgramLog.Log("Regions for TDSM #{0} enabled.", base.TDSMBuild);
        }

        public override void Disable()
        {
            ProgramLog.Plugin.Log("Regions disabled.");
        }

        public static void Log(String fmt, params object[] args)
        {
            ProgramLog.Plugin.Log(fmt, args);
        }
        
        #region Events
        
            public override void onPlayerTileChange(PlayerTileChangeEvent Event)
            {
                if (Event.Cancelled)
                    return;

                if(Event.Sender is Player) {

                    var player = Event.Sender as Player;

                    if (Event.Action == TileAction.PLACED && Event.TileType == TileType.BLOCK && (int)Event.Tile.Type == SelectorItem
                        && Selection.isInSelectionlist(player))
                    {
                        Event.Cancelled = true;
                        SelectorPos = !SelectorPos;

                        Vector2[] mousePoints = Selection.GetSelection(player);
                        if (!SelectorPos)
                            mousePoints[0] = Event.Position;
                        else
                            mousePoints[1] = Event.Position;

                        Selection.SetSelection(player, mousePoints);

                        player.sendMessage(string.Format("You have selected block at {0},{1}, {2} position", 
                            Event.Position.X, Event.Position.Y, (!SelectorPos) ? "First" : "Second"), ChatColor.Green);
                        return;
                    }


                    foreach (Region rgn in regionManager.Regions)
                    {
                        if (rgn.HasPoint(Event.Position))
                        {
                            if (IsRestrictedForUser(player, rgn, ((Event.Action == TileAction.BREAK) ? TileBreak : TilePlace)))
                            {
                                Event.Cancelled = true;
                                player.sendMessage("You cannot edit this area!", ChatColor.Red);
                                return;
                            }
                        }
                    }

                }
            }

            public override void onPlayerFlowLiquid(PlayerFlowLiquidEvent Event)
            {
                if (Event.Cancelled)
                    return;

                foreach (Region rgn in regionManager.Regions)
                {
                    if (rgn.HasPoint(Event.Position))
                    {
                        if (IsRestrictedForUser(Event.Player, rgn, LiquidFlow))
                        {
                            Event.Cancelled = true;
                            Event.Player.sendMessage("You cannot edit this area!", ChatColor.Red);
                            return;
                        }
                    }
                }
            }

            public override void onPlayerProjectileUse(PlayerProjectileEvent Event)
            {
                if (Event.Cancelled)
                    return;

                foreach (Region rgn in regionManager.Regions)
                {
                    if (rgn.HasPoint(Event.Projectile.Position / 16))
                    {
                        if( rProperties.BlockedProjectiles.Contains("*") ||
                            rProperties.BlockedProjectiles.Contains(Event.Projectile.Type.ToString()) ||
                            rProperties.BlockedProjectiles.Contains(Event.Projectile.Name.ToLower().Replace(" ", "")))
                        {
                            if (IsRestrictedForUser(Event.Player, rgn, ProjectileUse))
                            {
                                Event.Cancelled = true;
                                Event.Player.sendMessage("You cannot edit this area!", ChatColor.Red);
                                return;
                            }
                        }
                    }
                }
            }

            public override void onDoorStateChange(DoorStateChangeEvent Event)
            {
                if (Event.Cancelled)
                    return;

                var player = Event.Sender as Player;
                foreach (Region rgn in regionManager.Regions)
                {
                    if (rgn.HasPoint(new Vector2(Event.X, Event.Y)))
                    {
                        if (Event.Opener == DoorOpener.PLAYER)
                        {
                            if (IsRestrictedForUser(player, rgn, DoorChange))
                            {
                                Event.Cancelled = true;
                                player.sendMessage("You cannot edit this area!", ChatColor.Red);
                                return;
                            }
                        }
                        else if (Event.Opener == DoorOpener.NPC)
                        {
                            if (rgn.RestrictedNPCs)
                            {
                                Event.Cancelled = true;
                                return;
                            }
                        }
                    }
                }            
            }

        #endregion

        public static Boolean isRunningPermissions()
        {
            return Terraria_Server.Permissions.Node.isPermittedImpl != null;
        }

        public static Boolean IsRestrictedForUser(Player player, Region region, Node node)
        {
            if (UsingPermissions)
            {
                return Terraria_Server.Permissions.Node.isPermittedImpl(node, player);
            }

            return region.IsRestrictedForUser(player);
        }
    }
}
