using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Events;

namespace Terraria_Server.Commands
{
    public class ConsoleSender : Sender
    {
        ConsoleCommand cCommand = null;

        public ConsoleSender(Server server)
        {
            cCommand = new ConsoleCommand();
            setServer(server);
        }

        public ConsoleCommand getConsoleCommand()
        {
            return cCommand;
        }

    }
}
