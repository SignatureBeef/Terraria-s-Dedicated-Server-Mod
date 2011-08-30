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
                if(sender is Player)
                {
                    if(Selection.isInSelectionlist(sender as Player))
                    {
                        sender.sendMessage("Please finish the region selection first!", 255);
                        return;
                    }
                }
                Create(server, sender, args);
            }
            else if (args.TryPop("user"))
            {
                Boolean add = args.TryPop("add");
                Boolean remove = args.TryPop("remove");

                if (add)
                    AddUser(server, sender, args);
                else if (remove)
                    RemoveUser(server, sender, args);
                else
                    throw new CommandError("Please review your command.");
            }
            else if (args.TryPop("list"))
            {
                List(server, sender, args);
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

        public static void List(Server server, ISender sender, ArgumentList args)
        {
            for (int i = 0; i < Regions.regionManager.Regions.Count; i++)
            {
                sender.sendMessage(string.Format("Slot {0} : {1} [ {2} ] ({3},{4})", i, Regions.regionManager.Regions[i].Name, 
                    Regions.regionManager.Regions[i].Description,
                        Regions.regionManager.Regions[i].Point1.X, Regions.regionManager.Regions[i].Point1.Y), 
                        255, 255, 0, 0);
            }
        }

        public static void AddUser(Server server, ISender sender, ArgumentList args)
        {
            String User = "", IP = "";
            Int32 Slot;

            args.TryParseOne<String>("-ip", out IP); //Optional

            //IP or name?
            if (args.TryParseTwo<String, Int32>("-name", out User, "-slot", out Slot))
            {
                String[] exceptions = new String[2];
                if (User.Length > 0)
                {
                    exceptions[0] = User;
                }
                if (IP.Length > 0)
                {
                    exceptions[1] = IP;
                }

                Region.Region region = null;
                for (int i = 0; i < Regions.regionManager.Regions.Count; i++)
                {
                    if (Slot == i)
                        region = Regions.regionManager.Regions[i];
                    break;
                }

                if (region == null)
                    throw new CommandError("Specified Region Slot was incorrect.");

                //List<String> users = new List<String>();
                int usersAdded = 0;
                foreach (String toInflate in exceptions)
                {
                    if (toInflate != null)
                        foreach (String inflatee in toInflate.Split(','))
                        {
                            region.UserList.Add(inflatee);
                            usersAdded++;
                        }
                }

                if (usersAdded > 0)
                    sender.sendMessage(string.Format("{0} users were added to {1}", usersAdded, region.Name),
                        255, 0, 255, 0);
                else
                    throw new CommandError("A user was not able to be added to a Region.");
            }
            else
                throw new CommandError("Invalid arguments, Please review your command.");
        }

        public static void RemoveUser(Server server, ISender sender, ArgumentList args)
        {
            String User = "", IP = "";
            Int32 Slot;

            args.TryParseOne<String>("-ip", out IP); //Optional

            //IP or name?
            if (args.TryParseTwo<String, Int32>("-name", out User, "-slot", out Slot))
            {
                String[] exceptions = new String[2];
                if (User.Length > 0)
                {
                    exceptions[0] = User;
                }
                if (IP.Length > 0)
                {
                    exceptions[1] = IP;
                }

                Region.Region region = null;
                for (int i = 0; i < Regions.regionManager.Regions.Count; i++)
                {
                    if (Slot == i)
                        region = Regions.regionManager.Regions[i];
                    break;
                }

                if (region == null)
                    throw new CommandError("Specified Region Slot was incorrect.");

                int usersRemoved = 0;
                foreach (String toInflate in exceptions)
                {
                    if (toInflate != null)
                        foreach (String inflatee in toInflate.Split(','))
                        {
                            if (region.UserList.Contains(inflatee))
                            {
                                region.UserList.Add(inflatee);
                                usersRemoved++;
                            }
                        }
                }

                if (usersRemoved > 0)
                    sender.sendMessage(string.Format("{0} users were added to {1}", usersRemoved, region.Name),
                        255, 0, 255, 0);
                else
                    throw new CommandError("A user was not able to be removed from a Region.");
            }
            else
                throw new CommandError("Invalid arguments, Please review your command.");
        }
    }
}
