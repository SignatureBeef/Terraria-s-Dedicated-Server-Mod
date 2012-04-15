using Terraria_Server.Logging;

namespace Terraria_Server
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
        
        public override void sendMessage(string Message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
            ProgramLog.Console.Print (Message);
        }
    }
}
