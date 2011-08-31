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
using Regions.Region;
using Terraria_Server.Definitions;
using Terraria_Server.Permissions;

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
        
        public override void Load()
        {
            base.Name = "Regions";
            base.Description = "A region plguin for TDSM";
            base.Author = "DeathCradle";
            base.Version = "0.1";
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
                Log("No Permissions System Found, Using Internal User System");
        }

        public override void Enable()
        {
            ProgramLog.Log("Regions for TDSM #" + base.TDSMBuild + " enabled.");
        }

        public override void Disable()
        {
            ProgramLog.Log("Regions disabled.");
        }

        public static void Log(String fmt, params object[] args)
        {
            ProgramLog.Log(fmt, args);
        }
        
        #region Events
        
            public override void onPlayerTileChange(PlayerTileChangeEvent Event)
            {
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


                    foreach (Region.Region rgn in regionManager.Regions)
                    {
                        if (rgn.HasPoint(Event.Position))
                        {
                            if (rgn.IsRestrictedForUser(player))
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
                foreach (Region.Region rgn in regionManager.Regions)
                {
                    if (rgn.HasPoint(Event.Position))
                    {
                        if (rgn.IsRestrictedForUser(Event.Player))
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
                foreach (Region.Region rgn in regionManager.Regions)
                {
                    if (rgn.HasPoint(Event.Projectile.Position / 16))
                    {
                        if (rgn.IsRestrictedForUser(Event.Player))
                        {
                            Event.Cancelled = true;
                            Event.Player.sendMessage("You cannot edit this area!", ChatColor.Red);
                            return;
                        }
                    }
                }
            }

            public override void onDoorStateChange(DoorStateChangeEvent Event)
            {

                    var player = Event.Sender as Player;

                    foreach (Region.Region rgn in regionManager.Regions)
                    {
                        if (rgn.HasPoint(new Vector2(Event.X, Event.Y)))
                        {
                            if (Event.Opener == DoorOpener.PLAYER)
                            {
                                if (rgn.IsRestrictedForUser(player))
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

        public static Boolean IsRestrictedForUser(Player player, Region.Region region, Node node)
        {
            if (UsingPermissions)
            {
                return Terraria_Server.Permissions.Node.isPermittedImpl(node, player);
            }

            return region.IsRestrictedForUser(player);
        }
    }
}
