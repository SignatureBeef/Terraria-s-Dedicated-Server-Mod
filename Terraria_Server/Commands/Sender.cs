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
        //private string name = "";

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

        public string getName()
        {
            if (this is Player)
            {
                return ((Player)this).name;
            }
            return "CONSOLE";
        }

        public void sendMessage(string Message)
        {
            if (this is Player)
            {
                NetMessage.SendData(25, ((Player)this).whoAmi, -1, Message, 255, 255f, 0f, 0f);
            }
            else
            {
                Console.WriteLine(Message);
            }
        }

    }
}
