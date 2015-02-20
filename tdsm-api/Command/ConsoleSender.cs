namespace tdsm.api.Command
{
    /// <summary>
    /// ConsoleSender extension of Sender.  Allows for new ConsoleCommandEvents
    /// </summary>
    public class ConsoleSender : Sender
    {
        /// <summary>
        /// ConsoleSender constructor
        /// </summary>
        public ConsoleSender()
        {
            Op = true;
            Name = "CONSOLE";
            // I don't know what the hell was the deal with this
        }

        public override void SendMessage(string message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
            System.Console.WriteLine(message);
        }
    }
}