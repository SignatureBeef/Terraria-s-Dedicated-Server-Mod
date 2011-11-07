using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Misc;
using TDSMExamplePlugin;

namespace TDSMPlugin.Commands
{
    public class PluginCommands
    {
        public static void ExampleCommand(ISender sender, ArgumentList args)
        {
            TDSM_Plugin MyPlugin = (TDSM_Plugin)args.Plugin; //Get the plugin object who's assigned to the "tdsmpluginexample"
            
            //if the user enters /tdsmpluginexample -test 1, it will retreive the next value '1' and put into 'arg' as an integer.
            int arg;
            if (args.TryParseOne<int>("-test", out arg))
                sender.sendMessage(sender.Name + " Argument: " + arg);
            else
            {
                //For new people to .NET, I would not really expect you to understand everything just yet.
                string Platform = Terraria_Server.Definitions.Platform.Type.ToString();
                switch (Terraria_Server.Definitions.Platform.Type)
                {
                    case Terraria_Server.Definitions.Platform.PlatformType.LINUX:
                        Platform = "Linux";
                        break;
                    case Terraria_Server.Definitions.Platform.PlatformType.MAC:
                        Platform = "Mac";
                        break;
                    case Terraria_Server.Definitions.Platform.PlatformType.WINDOWS:
                        Platform = "Windows";
                        break;
                }

                (sender as Player).sendMessage("TDSM Plugin Example, Running OS: " + Platform, ChatColor.DarkGreen);
            }                
        }
    }
}
