using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Commands
{
    public class Sender
    {
        private Server server = null;

        public Server getServer()
        {
            return server;
        }

        public void setServer(Server Server)
        {
            server = Server;
        }

    }
}
