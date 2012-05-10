using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Commands;
using Terraria_Server;
using Terraria_Server.Misc;
using Regions.RegionWork;
using Terraria_Server.Permissions;

namespace Regions
{
    public class Commands
    {
        public RegionManager regionManager { get; set; }
        public Selection selection { get; set; }
        public Regions RegionsPlugin { get; set; }

        public Node Node_Select;
        public Node Node_Create;
        public Node Node_User;
        public Node Node_List;
        public Node Node_Projectile;
        public Node Node_Npcres;
        public Node Node_Opres;
        public Node Node_Here;
        public Node Node_ProtectAll;

        public static bool TryFindArg(ArgumentList args, string literal, out string Arg)
        {
            Arg = String.Empty;

            for (int i = 0; i < args.Count; i++)
            {
                if (args[i] == literal && (i + 1) < args.Count)
                {
                    Arg = args[i + 1];
                    return true;
                }
            }

            return false;
        }

        public void Region(ISender sender, ArgumentList args)
        {
            /* Meh [START] */
            bool ignoreError = false;
            string Command;
            if (args.TryGetString(0, out Command))
            {
                try
                {
                    if (args.TryPop("select"))
                    {
                        if (sender is Player && Regions.IsRestricted(Node_Select, sender as Player))
                            throw new CommandError("You do not have permissions for this command.");

                        SelectionToolToggle(sender, args);
                    }
                    else if (args.TryPop("create"))
                    {
                        if (sender is Player)
                        {
                            var player = sender as Player;

                            if (Regions.IsRestricted(Node_Create, player))
                            {
                                ignoreError = true;
                                throw new CommandError("You do not have permissions for this command.");
                            }

                            Vector2[] selected = selection.GetSelection(player);
                            if (selected == null
                                || (selected[0] == null || selected[0] == default(Vector2))
                                || (selected[1] == null || selected[1] == default(Vector2)))
                            {
                                sender.sendMessage("Please select a region first!", 255);
                                return;
                            }
                        }
                        Create(sender, args);
                    }
                    else if (args.TryPop("user"))
                    {
                        if (sender is Player && Regions.IsRestricted(Node_User, sender as Player))
                        {
                            ignoreError = true;
                            throw new CommandError("You do not have permissions for this command.");
                        }

                        bool add = args.TryPop("add");
                        bool remove = args.TryPop("remove");
                        bool clear = args.TryPop("clear");

                        if (add)
                            AddUser(sender, args);
                        else if (remove)
                            RemoveUser(sender, args);
                        else if (clear)
                            ClearRegion(sender, args);
                        else
                            throw new CommandError("Please review your command.");
                    }
                    else if (args.TryPop("list"))
                    {
                        if (sender is Player && Regions.IsRestricted(Node_List, sender as Player))
                            throw new CommandError("You do not have permissions for this command.");

                        List(sender, args);
                    }
                    else if (args.TryPop("projectile"))
                    {
                        if (sender is Player && Regions.IsRestricted(Node_Projectile, sender as Player))
                        {
                            ignoreError = true;
                            throw new CommandError("You do not have permissions for this command.");
                        }

                        bool add = args.TryPop("add");
                        bool remove = args.TryPop("remove");
                        bool clear = args.TryPop("clear");

                        if (add)
                            AddProjectile(sender, args);
                        else if (remove)
                            RemoveProjectile(sender, args);
                        else if (clear)
                            ClearProjectiles(sender, args);
                        else
                            throw new CommandError("Please review your command.");
                    }
                    else if (args.TryPop("npcres"))
                    {
                        if (sender is Player && Regions.IsRestricted(Node_Npcres, sender as Player))
                            throw new CommandError("You do not have permissions for this command.");

                        ToggleNPCRestrictions(sender, args);
                    }
                    else if (args.TryPop("opres"))
                    {
                        if (sender is Player && Regions.IsRestricted(Node_Opres, sender as Player))
                            throw new CommandError("You do not have permissions for this command.");

                        ToggleOPRestrictions(sender, args);
                    }
                    else if (args.TryPop("protectall"))
                    {
                        if (sender is Player && Regions.IsRestricted(Node_ProtectAll, sender as Player))
                            throw new CommandError("You do not have permissions for this command.");

                        ProtectMap(sender, args);
                    }
                    else if (args.TryPop("here"))
                    {
                        if (sender is Player && Regions.IsRestricted(Node_Here, sender as Player))
                            throw new CommandError("You do not have permissions for this command.");

                        RegionHere(sender, args);
                    }
                    else if (args.TryPop("reload"))
                    {
                        regionManager.LoadRegions();
                        sender.sendMessage("Regions reloaded.", 255);
                    }
                }
                catch (CommandError e)
                {
                    if (ignoreError)
                        return;

                    switch (Command)
                    {
                        //case "select":
                        //    break;
                        case "create":
                            sender.sendMessage("'region create': Creates a new Region.", 255);
                            sender.sendMessage("'region create' paremeters: -name <name> -desc <description>", 255);
                            sender.sendMessage("              (To restrict newplayers add -res)", 255);
                            sender.sendMessage("               (To restrict NPCs add -npcres)", 255);
                            break;
                        case "user":
                            sender.sendMessage("'region user': Adds, Removes or Clears users from a Region Slot.", 255);
                            sender.sendMessage("'region user' paremeters: add:remove:clear -name <name> -slot <region slot>", 255);
                            sender.sendMessage("              (To allow by ip use -ip instead of -name)", 255);
                            break;
                        case "projectile":
                            sender.sendMessage("'region projectile': Adds, Removes or Clears users from a Region Slot.", 255);
                            sender.sendMessage("'region projectile' paremeters: add:remove:clear -proj <id:name:*> -slot <region slot>", 255);
                            break;
                        //case "list":
                        //    break;
                        default:
                            throw e; //Unknown Error or an command with no args
                    }
                }
            }
            else
                sender.sendMessage("Region Commands: select, create, user, list, reload.", 255);
            /* Meh [END] */
        }

        public void SelectionToolToggle(ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                var player = sender as Player;
                if (selection.isInSelectionlist(player))
                {
                    selection.selectionPlayers.Remove(player.Name);
                    player.sendMessage("You have turned off the Selection Tool", ChatColor.Red);
                }
                else
                {
                    selection.selectionPlayers.Add(player.Name, new Vector2[] { default(Vector2), default(Vector2) });
                    player.sendMessage("You have turned on the Selection Tool", ChatColor.Green);
                }
            }

        }

        public void Create(ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                string Name = String.Empty;
                string Desc = String.Empty;
                bool Restrict = args.TryPop("-res");
                bool RestrictNPC = args.TryPop("-npcres");

                string tempDesc = String.Empty;
                if (!TryFindArg(args, "-desc", out Desc))
                {
                    Desc = tempDesc;
                }

                if (TryFindArg(args, "-name", out Name) && Name.Trim().Length > 0)
                {
                    var player = sender as Player;
                    if (selection.isInSelectionlist(player))
                    {
                        Vector2[] regionAxis = selection.GetSelection(player);

                        Region rgn = new Region();
                        rgn.Name = Name;
                        rgn.Description = Desc;
                        rgn.Point1 = regionAxis[0];
                        rgn.Point2 = regionAxis[1];
                        rgn.Restricted = Restrict;
                        rgn.RestrictedNPCs = RestrictNPC;

                        if (rgn.IsValidRegion())
                        {
                            regionManager.Regions.Add(rgn);
                            if (regionManager.SaveRegion(rgn))
                                player.sendMessage("Region '" + Name + "' was successfully created.", ChatColor.Green);
                            else
                                player.sendMessage("There was an issue while saving the region", ChatColor.Red);
                        }
                        else
                        {
                            player.sendMessage("There was an issue while creating the region", ChatColor.Red);
                        }
                    }
                    else
                    {
                        player.sendMessage("You need to select a region first!", ChatColor.Red);
                    }
                }
                else
                {
                    throw new CommandError("You have not specified certain arguments");
                }
            }
        }

        public void List(ISender sender, ArgumentList args)
        {
            for (int i = 0; i < regionManager.Regions.Count; i++)
            {
                sender.Message(255, "Slot {0} : {1} [ {2} ] ({3},{4})", i, regionManager.Regions[i].Name,
                    regionManager.Regions[i].Description,
                        regionManager.Regions[i].Point1.X, regionManager.Regions[i].Point1.Y);
            }

            if (regionManager.Regions.Count == 0)
            {
                sender.Message(255, "No regions.");
            }
        }

        public void AddUser(ISender sender, ArgumentList args)
        {
            string User = "", IP = "", regionName = "";
            int Slot = -1;


            //args.TryParseOne<String>("-ip", out IP); //Optional

            //IP or name?
            if (args.TryParseTwo<String, Int32>("-name", out User, "-slot", out Slot) ||
                args.TryParseTwo<String, Int32>("-ip", out User, "-slot", out Slot) ||
                args.TryParseTwo<String, String>("-name", out User, "-region", out regionName) ||
                args.TryParseTwo<String, String>("-ip", out User, "-region", out regionName))
            {
                string[] exceptions = new string[2];
                if (User.Length > 0)
                {
                    exceptions[0] = User;
                }
                if (IP.Length > 0)
                {
                    exceptions[1] = IP;
                }

                Region region = null;
                for (int i = 0; i < regionManager.Regions.Count; i++)
                {
                    if (Slot == i)
                    {
                        region = regionManager.Regions[i];
                        break;
                    }
                }

                //[TODO] TEST ME
                if (region == null && regionName.Length > 0)
                {
                    for (int i = 0; i < regionManager.Regions.Count; i++)
                    {
                        if (regionManager.Regions[i].Name.Trim().ToLower().Replace(" ", "").Equals(regionName.ToLower()))
                        {
                            region = regionManager.Regions[i];
                            break;
                        }
                    }
                }

                if (region == null)
                    throw new CommandError("Specified Region Slot was incorrect.");

                int usersAdded = 0;
                foreach (string toInflate in exceptions)
                {
                    if (toInflate != null)
                        foreach (string inflatee in toInflate.Split(','))
                        {
                            region.UserList.Add(inflatee);
                            usersAdded++;
                        }
                }

                if (usersAdded > 0)
                {
                    if (regionManager.SaveRegion(region))
                    {
                        sender.sendMessage(String.Format("{0} users were added to {1}", usersAdded, region.Name),
                        255, 0, 255);
                        RegionsPlugin.Log(sender.Name + " created region {0} with {1} user/s", region.Name, usersAdded);
                    }
                    else
                        sender.sendMessage(String.Format("Failed to save Region '{0}'", region.Name));
                }
                else
                    throw new CommandError("A user was not able to be added to a Region.");
            }
            else
                throw new CommandError("Invalid arguments, Please review your command.");
        }

        public void RemoveUser(ISender sender, ArgumentList args)
        {
            string User = "", IP = "";
            int Slot;

            TryFindArg(args, "-ip", out IP); //Optional

            //IP or name?
            if (args.TryParseTwo<String, Int32>("-name", out User, "-slot", out Slot))
            {
                string[] exceptions = new string[2];
                if (User.Length > 0)
                {
                    exceptions[0] = User;
                }
                if (IP.Length > 0)
                {
                    exceptions[1] = IP;
                }

                Region region = null;
                for (int i = 0; i < regionManager.Regions.Count; i++)
                {
                    if (Slot == i)
                    {
                        region = regionManager.Regions[i];
                        break;
                    }
                }

                if (region == null)
                    throw new CommandError("Specified Region Slot was incorrect.");

                int usersRemoved = 0;
                foreach (string toInflate in exceptions)
                {
                    if (toInflate != null)
                        foreach (string inflatee in toInflate.Split(','))
                        {
                            if (region.UserList.Contains(inflatee))
                            {
                                region.UserList.Remove(inflatee);
                                usersRemoved++;
                            }
                        }
                }

                if (usersRemoved > 0)
                {
                    if (regionManager.SaveRegion(region))
                    {
                        sender.sendMessage(String.Format("{0} users were removed from {1}", usersRemoved, region.Name),
                        255, 0, 255);
                    }
                    else
                        sender.sendMessage(String.Format("Failed to save Region '{0}'", region.Name));
                }
                else
                    throw new CommandError("A user was not able to be removed from a Region.");
            }
            else
                throw new CommandError("Invalid arguments, Please review your command.");
        }

        public void AddProjectile(ISender sender, ArgumentList args)
        {
            string projectiles, regionName = "";
            int Slot;

            if (args.TryParseTwo<String, Int32>("-proj", out projectiles, "-slot", out Slot) ||
                args.TryParseTwo<String, String>("-proj", out projectiles, "-region", out regionName))
            {
                Region region = null;
                for (int i = 0; i < regionManager.Regions.Count; i++)
                {
                    if (Slot == i)
                    {
                        region = regionManager.Regions[i];
                        break;
                    }
                }

                //[TODO] TEST ME
                if (region == null && regionName.Length > 0)
                {
                    for (int i = 0; i < regionManager.Regions.Count; i++)
                    {
                        if (regionManager.Regions[i].Name.Trim().ToLower().Replace(" ", "").Equals(regionName.ToLower()))
                        {
                            region = regionManager.Regions[i];
                            break;
                        }
                    }
                }

                if (region == null)
                    throw new CommandError("Specified Region Slot was incorrect.");

                int Count = 0;
                foreach (string proj in projectiles.Split(','))
                {
                    if (proj.Trim().Length > 0)
                    {
                        region.ProjectileList.Add(proj.Trim().ToLower().Replace(" ", ""));
                        Count++;
                    }
                }

                if (Count > 0)
                {
                    if (regionManager.SaveRegion(region))
                    {
                        string Proj = (projectiles == "*") ? "All" : Count.ToString();
                        sender.sendMessage(String.Format("{0} projectiles were blocked in Region '{1}'", Proj, region.Name),
                            255, 0, 255);
                    }
                    else
                        sender.sendMessage(String.Format("Failed to save Region '{0}'", region.Name));
                }
                else
                    sender.sendMessage(String.Format("No projectiles specified to add to Region '{0}'", region.Name));
            }
            else
                throw new CommandError("Please review your command");
        }

        public void RemoveProjectile(ISender sender, ArgumentList args)
        {
            string projectiles;
            int Slot;

            if (args.TryParseTwo<String, Int32>("-proj", out projectiles, "-slot", out Slot))
            {
                Region region = null;
                for (int i = 0; i < regionManager.Regions.Count; i++)
                {
                    if (Slot == i)
                    {
                        region = regionManager.Regions[i];
                        break;
                    }
                }

                if (region == null)
                    throw new CommandError("Specified Region Slot was incorrect.");

                int Count = 0;
                foreach (string proj in projectiles.Split(','))
                {
                    if (proj.Trim().Length > 0)
                    {
                        string projectile = proj.Trim().ToLower().Replace(" ", "");
                        while (region.ProjectileList.Contains(projectile))
                        {
                            if (region.ProjectileList.Remove(projectile))
                                Count++;
                        }
                    }
                }

                if (Count > 0)
                {
                    if (regionManager.SaveRegion(region))
                    {
                        string Proj = (projectiles == "*") ? "All" : Count.ToString();
                        sender.sendMessage(String.Format("{0} projectiles were unblocked in Region '{1}'", Proj, region.Name),
                            255, 0, 255);
                    }
                    else
                        sender.sendMessage(String.Format("Failed to save Region '{0}'", region.Name));
                }
                else
                    sender.sendMessage(String.Format("No projectiles specified to remove from Region '{0}'", region.Name));
            }
            else
                throw new CommandError("Please review your command");
        }

        public void ClearRegion(ISender sender, ArgumentList args)
        {
            string regionName = "";
            int Slot;

            if (args.TryParseOne<Int32>("-slot", out Slot) ||
                args.TryParseOne<String>("-region", out regionName))
            {
                Region region = null;
                for (int i = 0; i < regionManager.Regions.Count; i++)
                {
                    if (Slot == i)
                    {
                        region = regionManager.Regions[i];
                        break;
                    }
                }

                //[TODO] TEST ME
                if (region == null && regionName.Length > 0)
                {
                    for (int i = 0; i < regionManager.Regions.Count; i++)
                    {
                        if (regionManager.Regions[i].Name.Trim().ToLower().Replace(" ", "").Equals(regionName.ToLower()))
                        {
                            region = regionManager.Regions[i];
                            break;
                        }
                    }
                }

                if (region == null)
                    throw new CommandError("Specified Region Slot was incorrect.");

                region.UserList.Clear();
                if (regionManager.SaveRegion(region))
                {
                    sender.sendMessage(String.Format("Successfully cleared Players from Region '{0}'", region.Name), 255, 0, 255);
                }
                else
                    sender.sendMessage(String.Format("Failed to save Region '{0}'", region.Name));
            }
            else
                throw new CommandError("Invalid arguments, Please review your command.");
        }

        public void ClearProjectiles(ISender sender, ArgumentList args)
        {
            string regionName = "";
            int Slot;

            if (args.TryParseOne<Int32>("-slot", out Slot) ||
                args.TryParseOne<String>("-region", out regionName))
            {
                Region region = null;
                for (int i = 0; i < regionManager.Regions.Count; i++)
                {
                    if (Slot == i)
                    {
                        region = regionManager.Regions[i];
                        break;
                    }
                }

                //[TODO] TEST ME
                if (region == null && regionName.Length > 0)
                {
                    for (int i = 0; i < regionManager.Regions.Count; i++)
                    {
                        if (regionManager.Regions[i].Name.Trim().ToLower().Replace(" ", "").Equals(regionName.ToLower()))
                        {
                            region = regionManager.Regions[i];
                            break;
                        }
                    }
                }

                if (region == null)
                    throw new CommandError("Specified Region Slot was incorrect.");

                region.ProjectileList.Clear();
                if (regionManager.SaveRegion(region))
                {
                    sender.sendMessage(String.Format("Successfully cleared Projectiles from Region '{0}'", region.Name), 255, 0, 255);
                }
                else
                    sender.sendMessage(String.Format("Failed to save Region '{0}'", region.Name));
            }
            else
                throw new CommandError("Invalid arguments, Please review your command.");
        }

        public void ToggleNPCRestrictions(ISender sender, ArgumentList args)
        {
            string regionName = "";
            int Slot;

            if (args.TryParseOne<Int32>("-slot", out Slot) ||
                args.TryParseOne<String>("-region", out regionName))
            {
                Region region = null;
                for (int i = 0; i < regionManager.Regions.Count; i++)
                {
                    if (Slot == i)
                    {
                        region = regionManager.Regions[i];
                        break;
                    }
                }

                //[TODO] TEST ME
                if (region == null && regionName.Length > 0)
                {
                    for (int i = 0; i < regionManager.Regions.Count; i++)
                    {
                        if (regionManager.Regions[i].Name.Trim().ToLower().Replace(" ", "").Equals(regionName.ToLower()))
                        {
                            region = regionManager.Regions[i];
                            break;
                        }
                    }
                }

                if (region == null)
                    throw new CommandError("Specified Region Slot was incorrect.");

                region.RestrictedNPCs = !region.RestrictedNPCs;
                region.ProjectileList.Clear();
                if (regionManager.SaveRegion(region))
                {
                    sender.sendMessage(String.Format("NPC Restrictions is now {0} in Region '{1}'", (region.RestrictedNPCs) ? "on" : "off", region.Name), 255, 0, 255);
                }
                else
                    sender.sendMessage(String.Format("Failed to save Region '{0}'", region.Name));
            }
            else
                throw new CommandError("Invalid arguments, Please review your command.");
        }

        public void ToggleOPRestrictions(ISender sender, ArgumentList args)
        {
            string regionName = "";
            int Slot;

            if (args.TryParseOne<Int32>("-slot", out Slot) ||
                args.TryParseOne<String>("-region", out regionName))
            {
                Region region = null;
                for (int i = 0; i < regionManager.Regions.Count; i++)
                {
                    if (Slot == i)
                    {
                        region = regionManager.Regions[i];
                        break;
                    }
                }

                //[TODO] TEST ME
                if (region == null && regionName.Length > 0)
                {
                    for (int i = 0; i < regionManager.Regions.Count; i++)
                    {
                        if (regionManager.Regions[i].Name.Trim().ToLower().Replace(" ", "").Equals(regionName.ToLower()))
                        {
                            region = regionManager.Regions[i];
                            break;
                        }
                    }
                }

                if (region == null)
                    throw new CommandError("Specified Region Slot was incorrect.");

                region.Restricted = !region.Restricted;
                region.ProjectileList.Clear();
                if (regionManager.SaveRegion(region))
                {
                    sender.sendMessage(String.Format("OP Restrictions is now {0} in Region '{1}'", (region.Restricted) ? "on" : "off", region.Name), 255, 0, 255);
                }
                else
                    sender.sendMessage(String.Format("Failed to save Region '{0}'", region.Name));
            }
            else
                throw new CommandError("Invalid arguments, Please review your command.");
        }

        public void ProtectMap(ISender sender, ArgumentList args)
        {
            Vector2 Start = new Vector2(0, 0);
            Vector2 End = new Vector2(Main.maxTilesX, Main.maxTilesY);

            bool Restrict = args.TryPop("-res");
            bool RestrictNPC = args.TryPop("-npcres");

            string rgnName = "all";
            int count = 0;

            while (regionManager.ContainsRegion(rgnName))
            {
                //if(regionManager.ContainsRegion(rgnName))
                //{
                rgnName = "all" + count.ToString();
                count++;
                //}
                //else
                //    break;
            }

            Region rgn = new Region();
            rgn.Name = rgnName;
            rgn.Description = "A Region that protects the entire map";
            rgn.Point1 = Start;
            rgn.Point2 = End;
            rgn.Restricted = Restrict;
            rgn.RestrictedNPCs = RestrictNPC;

            if (rgn.IsValidRegion())
            {
                regionManager.Regions.Add(rgn);
                if (regionManager.SaveRegion(rgn))
                    sender.sendMessage("Region '" + rgnName + "' was successfully created.");
                else
                    sender.sendMessage("There was an issue while saving the region");
            }
            else
            {
                sender.sendMessage("There was an issue while creating the region");
            }
        }

        public void RegionHere(ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                var player = sender as Player;

                foreach (Region region in regionManager.Regions)
                {
                    if (region.HasPoint(player.Position / 16))
                        player.sendMessage("You are in Region '{0}'", ChatColor.Purple);
                }
            }
        }
    }
}
