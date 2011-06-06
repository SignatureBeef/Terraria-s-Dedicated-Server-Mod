using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class LoginEvent : Event
    {
        public Player getPlayer()
        {
            return (Player)base.getSender();
        }

        private ServerSock socket = null;

        public ServerSock getSocket()
        {
            return socket;
        }

        public void setSocket(ServerSock Socket)
        {
            socket = Socket;
        }
    }
}
