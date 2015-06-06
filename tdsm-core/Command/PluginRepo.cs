using System;
using tdsm.api.Command;

namespace tdsm.core
{
    public partial class Entry
    {
        public void RepositoryCommand(ISender sender, ArgumentList args)
        {
            var cmd = args.GetString(0);
            switch (cmd)
            {
                /*case "search": Lets see how many plugins we have
                    break;*/
                case "status":
                    var pkg = args.GetString(1);
                    var updateAll = pkg == "--all";

                    sender.Message("Finding updates...");

                    var packages = Repository.GetAvailableUpdate(updateAll ? null : pkg);
                    if (packages != null && packages.Length > 0)
                    {
                        sender.Message("Found update{0}:", packages.Length > 1 ? "s" : String.Empty);
                        foreach (var upd in packages)
                        {
                            var vers = String.Empty;
                            if (!String.IsNullOrEmpty(upd.Version)) vers = " [" + upd.Version + "]";

                            var desc = String.Empty;
                            if (!String.IsNullOrEmpty(upd.ShortDescription)) vers = " - " + upd.ShortDescription;

                            sender.Message(upd.Name + vers + desc);
                        }
                    }
                    else
                    {
                        sender.Message(pkg == "--all" ? "No updates available" : ("No package called:" + pkg), Microsoft.Xna.Framework.Color.Green);
                    }
                    break;
                case "update":
                    break;
                default:
                    throw new CommandError("No such command: " + cmd);
            }
        }

    }
}
