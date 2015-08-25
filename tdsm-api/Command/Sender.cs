using System.Collections.Generic;
using System.Linq;
using TDSM.API.Callbacks;

namespace TDSM.API.Command
{

    #if Full_API
    /// <summary>
    /// Sender class for command sending/parsing
    /// </summary>
    public abstract class WorldSender : Terraria.Entity, ISender
    #else
    public abstract class WorldSender : ISender
    #endif
    {
        public bool Op { get; set; }

        public string SenderName
        {
            get
            { return this.name; }
        }

        public void SendMessage(string message, int sender = 255, byte R = 255, byte G = 255, byte B = 255)
        {

        }

        public Dictionary<string, CommandInfo> GetAvailableCommands()
        {
            return null;
        }
    }

    #if Full_API
    /// <summary>
    /// Sender class for command sending/parsing
    /// </summary>
    public abstract class Sender : Terraria.Entity, ISender
    #else
    public abstract class Sender : ISender
    #endif
    {
        /// <summary>
        /// Get/set method for Sender's Op status
        /// </summary>
        public bool Op { get; set; }

        /// <summary>
        /// Gets the name of the sending entity; defaults to Console
        /// </summary>
        /// <returns>Sending entity name</returns>
        public virtual string Name { get; protected set; }

        public string SenderName
        {
            get
            { return this.Name; }
        }

        /// <summary>
        /// Writes a message to the server console
        /// </summary>
        /// <param name="Message">Message to write to the console</param>
        /// <param name="A"></param>
        /// <param name="R">Red text color value</param>
        /// <param name="G">Green text color value</param>
        /// <param name="B">Blue text color value</param>
        public abstract void SendMessage(string message, int sender = 255, byte R = 255, byte G = 255, byte B = 255);

        //public bool Is(Type type)
        //{
        //    return this.GetType().IsAssignableFrom(type);
        //}

        #if Full_API
        public bool IsPlayer()
        {
            return this is Terraria.Player;
        }
        #endif

        public Dictionary<string, CommandInfo> GetAvailableCommands()
        {
            var available = UserInput.CommandParser.serverCommands.GetAvailableCommands(this);

            foreach (var plg in PluginManager.EnumeratePlugins)
            {
                var additional = plg.commands.GetAvailableCommands(this)
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

            return available;
        }
    }

    public static class CommandMapExtensions
    {
        public static Dictionary<string, CommandInfo> GetAvailableCommands(this Dictionary<string, CommandInfo> map, ISender sender)
        {
            return map
                .Where(x => CommandParser.CheckAccessLevel(x.Value, sender) && !x.Key.StartsWith("."))
                .ToDictionary(x => x.Key, y => y.Value);
        }

        public static Dictionary<string, CommandInfo> GetAvailableCommands(this Dictionary<string, CommandInfo> map, AccessLevel access)
        {
            return map
                .Where(x => x.Value.accessLevel == access && !x.Key.StartsWith("."))
                .ToDictionary(x => x.Key, y => y.Value);
        }
    }
}