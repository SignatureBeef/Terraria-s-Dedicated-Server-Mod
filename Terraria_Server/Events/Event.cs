using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Commands;

namespace Terraria_Server.Events
{
    public class Event
    {
        bool cancelled = false;
        Sender messageSender = null;

        public bool getCancelled()
        {
            return cancelled;
        }

        public void setCancelled(bool Cancel)
        {
            cancelled = Cancel;
        }

        public Sender getSender()
        {
            return messageSender;
        }

        public void setSender(Sender Sender)
        {
            messageSender = Sender;
        }

    }
}
