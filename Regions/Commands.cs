using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Commands;
using Terraria_Server;
using Terraria_Server.Misc;
using Regions.Region;

namespace Regions
{
    public class Commands
    {
        public static void Region(Server server, ISender sender, ArgumentList args)
        {
            if (args.TryPop("select"))
                SelectionToolToggle(server, sender, args);
        }

        public static void SelectionToolToggle(Server server, ISender sender, ArgumentList args)
        {
            //Region.Region region = new Region.Region();

            //region.Name = "test";
            //region.Point1 = new Vector2(1, 3);
            //region.Point2 = new Vector2(30, 14);
            //if (Regions.regionManager.SaveRegion(region))
            //    sender.sendMessage("Save success!");
            //else
            //    sender.sendMessage("Save fail.");
            if (sender is Player)
            {
                var player = sender as Player;
                if (Selection.isInSelectionlist(player))
                {
                    Selection.selectionPlayers.Remove(player.Name);
                    player.sendMessage("You have turned off the Selection Tool", ChatColour.Red);
                }
                else
                {
                    Selection.selectionPlayers.Add(player.Name, new Vector2[] { default(Vector2), default(Vector2) });
                    player.sendMessage("You have turned on the Selection Tool", ChatColour.Green);
                }
            }

        }

        public static void Create(Server server, ISender sender, ArgumentList args)
        {

        }
    }
}
