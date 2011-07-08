using System;
namespace Terraria_Server.Commands
{
    /// <summary>
    /// Sender class for command sending/parsing
    /// </summary>
    public class Sender
    {
        /// <summary>
        /// Gets current Server instance
        /// </summary>
        /// <returns>Current Server instance</returns>
        public Server getServer()
        {
            return Program.server;
        }

        /// <summary>
        /// Get/set method for Sender's Op status
        /// </summary>
        public bool Op { get; set; }

        /// <summary>
        /// Gets the name of the sending entity; defaults to Console
        /// </summary>
        /// <returns>Sending entity name</returns>
        public virtual String getName()
        {
            return "CONSOLE";
        }

        /// <summary>
        /// Writes a message to the server console
        /// </summary>
        /// <param name="Message">Message to write to the console</param>
        /// <param name="A"></param>
        /// <param name="R">Red text color value</param>
        /// <param name="G">Green text color value</param>
        /// <param name="B">Blue text color value</param>
        public virtual void sendMessage(String Message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
            Program.tConsole.WriteLine(Message);
        }

    }
}
