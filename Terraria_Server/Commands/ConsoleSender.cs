using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Events;

namespace Terraria_Server.Commands
{
    public class ConsoleSender : Sender
    {
        ConsoleCommandEvent cCommand = null;

        public ConsoleSender(Server server)
        {
            cCommand = new ConsoleCommandEvent();
            base.setOp(true);
        }

        public ConsoleCommandEvent getConsoleCommand()
        {
            return cCommand;
        }

    }
}
