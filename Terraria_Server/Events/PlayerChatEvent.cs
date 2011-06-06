using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class PlayerChatEvent : Event
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
