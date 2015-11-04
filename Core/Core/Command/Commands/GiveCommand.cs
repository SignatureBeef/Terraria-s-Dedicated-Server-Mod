using System;
using TDSM.Core.Definitions;
using OTA.Command;
using Terraria;
using OTA.Misc;

namespace TDSM.Core.Command.Commands
{
    public class GiveCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("give")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Give a player items")
                .WithHelpText("<amount> <itemname:itemid> [prefix] [player]")
                .WithPermissionNode("tdsm.give")
                .Calls(this.Give);
        }

        /// <summary>
        /// Gives specified item to the specified player.
        /// </summary>
        /// <param name="sender">Sending player</param>
        /// <param name="args">Arguments sent with command</param>
        public void Give(ISender sender, ArgumentList args)
        {
            // /give <stack> <item> [prefix] [player]
            var index = 0;
            int stack = args.GetInt(index++);
            string name = args.GetString(index++);

            //            var max = Tools.AvailableItemSlots; //Perhaps remove a few incase of new drops
            //            if (stack > max)
            //            {
            //                stack = max; // Set to Tools.AvailableItemSlots because number given was larger than this.
            //            }
            int id;
            var results = Int32.TryParse(name, out id) ? DefinitionManager.FindItem(id) : DefinitionManager.FindItem(name);
            if (results != null && results.Length > 0)
            {
                if (results.Length > 1)
                    throw new CommandError(String.Format("More than 1 item found, total is: {0}", results.Length));

                var item = results[0];
                string prefix;
                if (args.TryGetString(index, out prefix))
                {
                    try
                    {
                        Affix afx;
                        if (Enum.TryParse(prefix, out afx))
                        {
                            item.Prefix = (int)afx;
                            index++;
                        }
                    }
                    catch (ArgumentException)
                    {
                        throw new CommandError(String.Format("Error, the Prefix you entered was not found: {0}", args.GetString(3)));
                    }
                }

                Player receiver;
                if (!args.TryGetOnlinePlayer(index, out receiver))
                {
                    if (sender is Player)
                        receiver = sender as Player;
                    else throw new CommandError("Expected an online player");
                }

                receiver.GiveItem(item.Id, stack, item.MaxStack, sender, item.NetId, true, item.Prefix);
            }
            else
                throw new CommandError(String.Format("No item known by: {0}", name));
        }
    }
}

