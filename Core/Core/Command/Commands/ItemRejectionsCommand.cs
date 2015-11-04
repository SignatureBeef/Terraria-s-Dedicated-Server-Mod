//using System;
//
//namespace TDSM.Core.Command.Commands
//{
//    public class ItemRejectionsCommand : CoreCommand
//    {
//        public override void Initialise()
//        {
//            this.AddCommand("itemrej")
//                .WithAccessLevel(AccessLevel.OP)
//                .WithDescription("Manage item rejections")
//                .WithHelpText("-add|-remove <id:name>")
//                .WithHelpText("-clear")
//                .WithPermissionNode("tdsm.itemrej")
//                .Calls(this.ItemRejection);
//        }
//        /// <summary>
//        /// Manage item rejections
//        /// </summary>
//        /// <param name="sender">Sending player</param>
//        /// <param name="args">Arguments sent with command</param>
//        public void ItemRejection(ISender sender, ArgumentList args)
//        {
//            #if TDSMServer
//            string exception;
//            if (args.TryParseOne<String>("-add", out exception))
//            {
//            if (!Server.ItemRejections.Contains(exception))
//            {
//            Server.ItemRejections.Add(exception);
//            sender.Message(exception + " was successfully added.");
//            }
//            else
//            {
//            throw new CommandError("Item already exists.");
//            }
//            }
//            else if (args.TryParseOne<String>("-remove", out exception))
//            {
//            if (Server.ItemRejections.Contains(exception))
//            {
//            Server.ItemRejections.Remove(exception);
//            sender.Message(exception + " was successfully removed.");
//            }
//            else
//            {
//            throw new CommandError("Item does not exist.");
//            }
//            }
//            else if (args.TryPop("-clear"))
//            {
//            Server.ItemRejections.Clear();
//            sender.Message("Item rejection list cleared.");
//            }
//            else
//            {
//            throw new CommandError("Expected argument -add|-remove|-clear");
//            }
//            #endif
//        }
//    }
//}
//
