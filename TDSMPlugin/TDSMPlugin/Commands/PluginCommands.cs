using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Misc;

namespace TDSMExamplePlugin.Commands
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
                string platform = Platform.Type.ToString();
                switch (Platform.Type)
                {
                    case Platform.PlatformType.LINUX:
						platform = "Linux";
                        break;
                    case Platform.PlatformType.MAC:
						platform = "Mac";
                        break;
                    case Platform.PlatformType.WINDOWS:
						platform = "Windows";
                        break;
                }

				(sender as Player).sendMessage("TDSM Plugin Example, Running OS: " + platform, ChatColor.DarkGreen);
            }                
        }
    }
}
