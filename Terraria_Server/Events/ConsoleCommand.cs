using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Commands;

namespace Terraria_Server.Events
{
    public class ConsoleCommand : Event
    {
        string message = "";

        public string getMessage()
        {
            return message;
        }

        public void setMessage(string Message)
        {
            message = Message;
        }

    }
}
