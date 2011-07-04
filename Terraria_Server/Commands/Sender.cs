using System;
namespace Terraria_Server.Commands
{
    public class Sender
    {
        public Server getServer()
        {
            return Program.server;
        }

        public bool Op { get; set; }

        public virtual String getName()
        {
            return "CONSOLE";
        }

        public virtual void sendMessage(String Message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
            Program.tConsole.WriteLine(Message);
        }

    }
}
