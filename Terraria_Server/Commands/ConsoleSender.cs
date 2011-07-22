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
        public ConsoleSender ()
        {
            Op = true;
            Name = "CONSOLE";
            // I don't know what the hell was the deal with this
        }
    }
}
