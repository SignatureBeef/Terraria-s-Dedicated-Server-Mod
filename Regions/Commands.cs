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
            {
                SelectionToolToggle(server, sender, args);
            }
            else if (args.TryPop("create"))
            {
                Create(server, sender, args);
            }
        }

        public static void SelectionToolToggle(Server server, ISender sender, ArgumentList args)
        {
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
            if (sender is Player)
            {
                String Name = "";
                String Desc = "";
                Boolean Restrict = args.TryPop("-res");
                Boolean RestrictNPC = args.TryPop("-npcres");
                
                if (args.TryParseTwo<String, String>("-name", out Name, "-desc", out Desc)
                    && Name.Trim().Length > 0)
                {
                    var player = sender as Player;
                    if (Selection.isInSelectionlist(player))
                    {
                        Vector2[] regionAxis = Selection.GetSelection(player);

                        Region.Region rgn = new Region.Region();
                        rgn.Name = Name;
                        rgn.Description = Desc;
                        rgn.Point1 = regionAxis[0];
                        rgn.Point2 = regionAxis[1];
                        rgn.Restricted = Restrict;
                        rgn.RestrictedNPCs = RestrictNPC;

                        if (rgn.IsValidRegion())
                        {
                            Regions.regionManager.Regions.Add(rgn);
                            if(Regions.regionManager.SaveRegion(rgn))
                                player.sendMessage("Region '" + Name + "' was successfully created.", ChatColour.Green);
                            else
                                player.sendMessage("There was an issue while saving the region", ChatColour.Red);
                        }
                        else
                        {
                            player.sendMessage("There was an issue while creating the region", ChatColour.Red);
                        }
                    }
                    else
                    {
                        player.sendMessage("You need to select a region first!", ChatColour.Red);
                    }
                }
                else
                {
                    throw new CommandError("You have not specified certain arguments");
                }
            }
        }
    }
}
