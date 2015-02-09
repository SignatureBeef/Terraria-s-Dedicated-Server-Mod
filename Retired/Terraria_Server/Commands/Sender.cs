using System;
using Terraria_Server.Logging;

namespace Terraria_Server
{
    /// <summary>
    /// Sender class for command sending/parsing
    /// </summary>
    public abstract class Sender : ISender
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

        /// <summary>
        /// Writes a message to the server console
        /// </summary>
        /// <param name="Message">Message to write to the console</param>
        /// <param name="A"></param>
        /// <param name="R">Red text color value</param>
        /// <param name="G">Green text color value</param>
        /// <param name="B">Blue text color value</param>
        public abstract void sendMessage(string Message, int A = 255, float R = 255f, float G = 0f, float B = 0f);
    }
}
