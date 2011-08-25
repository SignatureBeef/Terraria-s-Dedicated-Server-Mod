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

namespace Regions
{
    public class Regions : Plugin
    {
        public const Int32 SelectorItem = 0;

        public static Vector2[] mousePoints = { new Vector2(0, 0), new Vector2(0, 0) };
        private static Boolean SelectorPos = true; //false for 1st (mousePoints[0]), true for 2nd

        public override void Load()
        {
            base.Name = "Regions";
            base.Description = "A region plguin for TDSM";
            base.Author = "DeathCradle";
            base.Version = "0.1";
            base.TDSMBuild = 32;

            AddCommand("region select")
                .WithAccessLevel(AccessLevel.OP)
                .WithHelpText("Usage:    region select")
                .WithDescription("Turn on the Selectin Tool Mode")
                .Calls(Commands.SelectionToolToggle);

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
                    if (Event.Action == TileAction.PLACED && Event.TileType == TileType.BLOCK && (int)Event.Tile.Type == SelectorItem)
                    {
                        Event.Cancelled = true;
                        SelectorPos = !SelectorPos;

                        if (!SelectorPos)
                            mousePoints[0] = Event.Position;
                        else
                            mousePoints[1] = Event.Position;

                        (Event.Sender as Player).sendMessage(string.Format("You have selected block at {0},{1}, {2} position", 
                            Event.Position.X, Event.Position.Y, (!SelectorPos) ? "First" : "Second"), ChatColour.Green);
                    }
                }
            }

        #endregion

    }
}
