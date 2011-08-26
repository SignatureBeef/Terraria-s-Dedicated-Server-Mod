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

namespace Regions
{
    public class Regions : Plugin
    {
        public const Int32 SelectorItem = 0;

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
        //public static Vector2[] mousePoints = { new Vector2(0, 0), new Vector2(0, 0) };
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

            //registerHook(Hooks.PLAYER_KEYPRESS); 
            registerHook(Hooks.PLAYER_TILECHANGE);
        }

        public override void Enable()
        {
            ProgramLog.Log("Regions for TDSM #" + base.TDSMBuild + " enabled.");
        }

        public override void Disable()
        {
            ProgramLog.Log("Regions disabled.");
        }
        
        #region Events

            //public override void onPlayerKeyPress(PlayerKeyPressEvent Event)
            //{
            //    if(Event.MouseClicked)
            //        Event.
            //}

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
                            Event.Position.X, Event.Position.Y, (!SelectorPos) ? "First" : "Second"), ChatColour.Green);
                        return;
                    }


                    foreach (Region.Region rgn in regionManager.Regions)
                    {
                        if (rgn.HasPoint(Event.Position))
                        {
                            if (rgn.Restricted && player.Op || !rgn.ContainsUser(player.Name))
                            {
                                Event.Cancelled = true;
                                player.sendMessage("You cannot edit this area!", ChatColour.Red);
                                return;
                            }
                        }
                    }

                }
            }

        #endregion

    }
}
