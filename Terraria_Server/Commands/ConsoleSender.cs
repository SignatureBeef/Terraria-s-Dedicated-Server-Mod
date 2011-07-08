using Terraria_Server.Events;

namespace Terraria_Server.Commands
{
    /// <summary>
    /// ConsoleSender extension of Sender.  Allows for new ConsoleCommandEvents
    /// </summary>
    public class ConsoleSender : Sender
    {
        /// <summary>
        /// ConsoleSender constructor
        /// </summary>
        /// <param name="server">Current Server instance</param>
        public ConsoleSender(Server server)
        {
            ConsoleCommand = new ConsoleCommandEvent();
        }

        /// <summary>
        /// ConsoleCommandEvent set/retrieve
        /// </summary>
        public ConsoleCommandEvent ConsoleCommand { get; private set; }
    }
}
