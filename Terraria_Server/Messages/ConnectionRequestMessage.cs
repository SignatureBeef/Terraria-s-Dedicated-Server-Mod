using System;
using System.Text;
using Terraria_Server.Events;


namespace Terraria_Server.Messages
{
    public class ConnectionRequestMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.CONNECTION_REQUEST;
        }

        public int? GetRequiredNetMode()
        {
            return 2;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            ServerSock serverSock = Netplay.serverSock[whoAmI];
            PlayerLoginEvent loginEvent = new PlayerLoginEvent();
            loginEvent.Socket = serverSock;
            loginEvent.Sender = Main.player[whoAmI];
            Program.server.getPluginManager().processHook(Plugin.Hooks.PLAYER_PRELOGIN, loginEvent);
            if (loginEvent.Cancelled)
            {
                NetMessage.SendData(2, whoAmI, -1, "Disconnected By Server.");
                return;
            }

            String clientName = serverSock.tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0];

            if (Program.server.BanList.containsException(clientName))
            {
                NetMessage.SendData(2, whoAmI, -1, "You are banned from this Server.");
                return;
            }

            if (Program.properties.UseWhiteList && !Program.server.WhiteList.containsException(clientName))
            {
                NetMessage.SendData(2, whoAmI, -1, "You are not on the WhiteList.");
                return;
            }

            if (serverSock.state == 0)
            {
                string version = Encoding.ASCII.GetString(readBuffer, start + 1, length - 1);
                if (!(version == "Terraria" + Statics.CURRENT_RELEASE))
                {
                    NetMessage.SendData(2, whoAmI, -1, "You are not using the same version as this Server.");
                    return;
                }

                if (Netplay.password == null || Netplay.password == "")
                {
                    serverSock.state = 1;
                    NetMessage.SendData(3, whoAmI);
                    return;
                }

                serverSock.state = -1;
                NetMessage.SendData(37, whoAmI);
            }
        }
    }
}
