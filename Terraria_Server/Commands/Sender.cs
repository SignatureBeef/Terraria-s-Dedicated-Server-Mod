namespace Terraria_Server.Commands
{
    public class Sender
    {
        public Server getServer()
        {
            return Program.server;
        }

        public bool Op { get; set; }

        public virtual string getName()
        {
            return "CONSOLE";
        }

        public virtual void sendMessage(string Message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
            Program.tConsole.WriteLine(Message);
        }

    }
}
