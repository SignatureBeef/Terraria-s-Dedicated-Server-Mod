using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Terraria_Server;
using Terraria_Server.Misc;
using Terraria_Server.Logging;
using Terraria_Server.Commands;
using Terraria_Server.Definitions;
using Terraria_Server.Permissions;
using Terraria_Server.Definitions.Tile;

using Regions.RegionWork;
using Terraria_Server.Plugins;

namespace Regions
{
    public class Regions : BasePlugin
    {
        /*
         * @Developers
         * 
         * Plugins need to be in .NET 4.0
         * Otherwise TDSM will be unable to load it. 
         */

        public static int SelectorItem = 0;
        public static bool UsingPermissions = false;

        public static string RegionsFolder
        {
            get
            {
                return Statics.PluginPath + Path.DirectorySeparatorChar + "Regions";
            }
        }
        public static string DataFolder
        {
            get
            {
                return RegionsFolder + Path.DirectorySeparatorChar + "Data";
            }
        }

        public Properties rProperties { get; set; }
        public RegionManager regionManager { get; set; }
        private bool SelectorPos = true; //false for 1st (mousePoints[0]), true for 2nd
        public Selection selection;

        public Commands commands;

        public Node DoorChange;
        public Node LiquidFlow;
        public Node TileBreak;
        public Node TilePlace;
        public Node ProjectileUse;

        protected override void Initialized(object state)
        {
            base.Name = "Regions";
            base.Description = "A region plugin for TDSM";
            base.Author = "DeathCradle";
            base.Version = "1";
            base.TDSMBuild = 36;

            if (!Directory.Exists(RegionsFolder))
                Directory.CreateDirectory(RegionsFolder);

            rProperties = new Properties(RegionsFolder + Path.DirectorySeparatorChar + "regions.properties");
            rProperties.Load();

            rProperties.AddHeaderLine("Use 'rectify=true' to ignore world alterations from");
            rProperties.AddHeaderLine("players who are blocked; Possibly saving bandwidth.");

            rProperties.pushData();
            rProperties.Save();

            SelectorItem = rProperties.SelectionToolID;

            regionManager = new RegionManager(DataFolder);

            selection = new Selection();

            commands = new Commands();
            commands.regionManager = regionManager;
            commands.RegionsPlugin = this;
            commands.selection = selection;


            AddCommand("region")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("Usage:    region [select, create, user, list, npcres, opres]")
                .WithDescription("Region Management.")
                .WithPermissionNode("regions")
                .Calls(commands.Region);

            AddCommand("regions")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("Usage:    regions [select, create, user, list, npcres, opres]")
                .WithDescription("Region Management.")
                .WithPermissionNode("regions") //Need another method to split the commands up.
                .Calls(commands.Region);
            
            Hook(HookPoints.PlayerWorldAlteration, OnPlayerWorldAlteration);
            Hook(HookPoints.LiquidFlowReceived, OnLiquidFlowReceived);
            Hook(HookPoints.ProjectileReceived, OnProjectileReceived);
            Hook(HookPoints.DoorStateChanged,   OnDoorStateChange);

            UsingPermissions = isRunningPermissions();
            if (UsingPermissions)
                Log("Using Permissions.");
            else
                Log("No Permissions Found\nUsing Internal User System");
        }

        protected override void Enabled()
        {            
            DoorChange      = Node.FromPath("regions.doorchange");
            LiquidFlow      = Node.FromPath("regions.liquidflow");
            TileBreak       = Node.FromPath("regions.tilebreak");
            TilePlace       = Node.FromPath("regions.tileplace");
            ProjectileUse   = Node.FromPath("regions.projectileuse");

            ProgramLog.Plugin.Log("Regions for TDSM #{0} enabled.", base.TDSMBuild);
        }

        protected override void Disabled()
        {
            ProgramLog.Plugin.Log("Regions disabled.");
        }

        public void Log(string fmt, params object[] args)
        {
            foreach (string line in String.Format(fmt, args).Split('\n'))
            {
                ProgramLog.Plugin.Log("[Regions] " + line);
            }
        }
        
        #region Events

            void OnPlayerWorldAlteration(ref HookContext ctx, ref HookArgs.PlayerWorldAlteration args)
            {
                Vector2 Position = new Vector2(args.X, args.Y);

                if (args.TileWasPlaced && args.Type == SelectorItem && selection.isInSelectionlist(ctx.Player))
                {
                    ctx.SetResult(HookResult.ERASE);
                    SelectorPos = !SelectorPos;

                    Vector2[] mousePoints = selection.GetSelection(ctx.Player);

                    if (!SelectorPos)
                        mousePoints[0] = Position;
                    else
                        mousePoints[1] = Position;

                    selection.SetSelection(ctx.Player, mousePoints);

                    ctx.Player.sendMessage(String.Format("You have selected block at {0},{1}, {2} position",
                        Position.X, Position.Y, (!SelectorPos) ? "First" : "Second"), ChatColor.Green);
                    return;
                }

                foreach (Region rgn in regionManager.Regions)
                {
                    if (rgn.HasPoint(Position))
                    {
                        if (IsRestrictedForUser(ctx.Player, rgn, ((args.TileWasRemoved || args.WallWasRemoved) ? TileBreak : TilePlace)))
                        {
                            ctx.SetResult(HookResult.ERASE);
                            ctx.Player.sendMessage("You cannot edit this area!", ChatColor.Red);
                            return;
                        }
                    }
                }
            }

            void OnLiquidFlowReceived(ref HookContext ctx, ref HookArgs.LiquidFlowReceived args)
            {
                Vector2 Position = new Vector2(args.X, args.Y);

                foreach (Region rgn in regionManager.Regions)
                {
                    if (rgn.HasPoint(Position))
                    {
                        if (IsRestrictedForUser(ctx.Player, rgn, LiquidFlow))
                        {
                            ctx.SetResult(HookResult.ERASE);
                            ctx.Player.sendMessage("You cannot edit this area!", ChatColor.Red);
                            return;
                        }
                    }
                }
            }

            void OnProjectileReceived(ref HookContext ctx, ref HookArgs.ProjectileReceived args)
            {
                Vector2 Position = new Vector2(args.X, args.Y);

                foreach (Region rgn in regionManager.Regions)
                {
                    if (rgn.HasPoint(Position / 16))
                    {
                        if (rgn.ProjectileList.Contains("*") ||
                            rgn.ProjectileList.Contains(args.Type.ToString()))// ||
                            //rgn.ProjectileList.Contains(args.Projectile.Name.ToLower().Replace(" ", "")))
                        {
                            if (IsRestrictedForUser(ctx.Player, rgn, ProjectileUse))
                            {
                                ctx.SetResult(HookResult.ERASE);
                                ctx.Player.sendMessage("You cannot edit this area!", ChatColor.Red);
                                return;
                            }
                        }
                    }
                }
            }

            void OnDoorStateChange(ref HookContext ctx, ref HookArgs.DoorStateChanged args)
            {
                foreach (Region rgn in regionManager.Regions)
                {
                    if (rgn.HasPoint(new Vector2(args.X, args.Y)))
                    {
                        if (ctx.Sender is Player)
                        {
                            if (IsRestrictedForUser(ctx.Player, rgn, DoorChange))
                            {
                                ctx.SetResult(HookResult.RECTIFY);
                                ctx.Player.sendMessage("You cannot edit this area!", ChatColor.Red);
                                return;
                            }
                        }
                        else if (ctx.Sender is NPC)
                        {
                            if (rgn.RestrictedNPCs)
                            {
                                ctx.SetResult(HookResult.RECTIFY); //[TODO] look into RECIFYing for NPC's, They don't need to be resent, only cancelled, IGRNORE?
                                return;
                            }
                        } 
                    }
                }  
            }

        #endregion

        public bool isRunningPermissions()
        {
            return Program.permissionManager.isPermittedImpl != null;
        }

        public bool IsRestrictedForUser(Player player, Region region, Node node)
        {
            if (UsingPermissions)
            {
                return Program.permissionManager.isPermittedImpl(node.Path, player);
            }

            return region.IsRestrictedForUser(player);
        }
    }
}
