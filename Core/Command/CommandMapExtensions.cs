using System;
using System.Collections.Generic;
using System.Linq;
using OTA.Callbacks;
using OTA;
using OTA.Command;

namespace TDSM.Core.Command
{
    /// <summary>
    /// Command map extensions
    /// </summary>
    public static class CommandMapExtensions
    {
        /// <summary>
        /// Gets commands for a particular sender
        /// </summary>
        /// <returns>The available commands.</returns>
        /// <param name="map">Map.</param>
        /// <param name="sender">Sender.</param>
        public static Dictionary<string, CommandInfo> GetAvailableCommands(this Dictionary<string, CommandInfo> map, ISender sender)
        {
            return map
                .Where(x => CommandParser.CheckAccessLevel(x.Value, sender) && !x.Key.StartsWith("."))
                .ToDictionary(x => x.Key, y => y.Value);
        }

        /// <summary>
        /// Gets commands for a particular access level
        /// </summary>
        /// <returns>The available commands.</returns>
        /// <param name="map">Map.</param>
        /// <param name="access">Access.</param>
        public static Dictionary<string, CommandInfo> GetAvailableCommands(this Dictionary<string, CommandInfo> map, AccessLevel access)
        {
            return map
                .Where(x => x.Key != null && x.Value != null && x.Value.accessLevel == access && !x.Key.StartsWith("."))
                .ToDictionary(x => x.Key, y => y.Value);
        }

        /// <summary>
        /// Gets the available commands for the sender
        /// </summary>
        /// <returns>The available commands.</returns>
        public static Dictionary<string, CommandInfo> GetAvailableCommands(this ISender sender)
        {
            var available = Entry.CommandParser.serverCommands.GetAvailableCommands(sender);

            foreach (var plg in OTA.Plugin.PluginManager.EnumeratePlugins)
            {
                if (plg.IsEnabled)
                {
                    var additional = plg.GetPluginCommands().GetAvailableCommands(sender)
                        .Where(x => !x.Key.StartsWith(plg.Name.ToLower() + '.'))
                        .ToArray();
                    foreach (var pair in additional)
                    {
                        //Override defaults
                        if (available.ContainsKey(pair.Key))
                            available[pair.Key] = pair.Value;
                        else available.Add(pair.Key, pair.Value);
                    }
                }
            }

            return available;
        }
    }
}