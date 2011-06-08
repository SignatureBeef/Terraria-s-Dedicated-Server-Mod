using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Commands
{
    public class Sender
    {
        //private Server server = null;
        private bool op = false;

        public Server getServer()
        {
            return Program.server;
        }

        //public void setServer(Server Server)
        //{
        //    server = Server;
        //}

        public void setOp(bool Op)
        {
            op = Op;
        }

        public bool isOp()
        {
            return op;
        }

    }
}
