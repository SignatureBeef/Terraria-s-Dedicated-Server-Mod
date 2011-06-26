using Terraria_Server.Events;

namespace Terraria_Server.Commands
{
    public class ConsoleSender : Sender
    {
        public ConsoleSender(Server server)
        {
            ConsoleCommand = new ConsoleCommandEvent();
        }

        public ConsoleCommandEvent ConsoleCommand { get; private set; }
    }
}
