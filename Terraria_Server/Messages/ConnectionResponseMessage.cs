using System;


namespace Terraria_Server.Messages
{
    public class ConnectionResponseMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.CONNECTION_RESPONSE;
        }

        public int? GetRequiredNetMode()
        {
            return 1;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            if (Netplay.clientSock.state == 1)
            {
                Netplay.clientSock.state = 2;
            }

            int myPlayerNum = (int)readBuffer[start + 1];
            if (myPlayerNum != Main.myPlayer)
            {
                Main.players[myPlayerNum] = (Player)Main.players[Main.myPlayer].Clone();
                Main.players[Main.myPlayer] = new Player();
                Main.players[myPlayerNum].whoAmi = myPlayerNum;
                Main.myPlayer = myPlayerNum;
            }

            NetMessage.SendData(4, -1, -1, Main.players[Main.myPlayer].Name, Main.myPlayer);
            NetMessage.SendData(16, -1, -1, String.Empty, Main.myPlayer);
            NetMessage.SendData(42, -1, -1, String.Empty, Main.myPlayer);
            NetMessage.SendData(50, -1, -1, String.Empty, Main.myPlayer);

            int count = 0;
            foreach(Item item in Main.players[Main.myPlayer].inventory)
            {
                NetMessage.SendData(5, -1, -1, item.Name, Main.myPlayer, (float)count++);
            }

            //Count should equal 44 at this point.
            foreach (Item armor in Main.players[Main.myPlayer].armor)
            {
                NetMessage.SendData(5, -1, -1, armor.Name, Main.myPlayer, (float)count++);
            }

            NetMessage.SendData(6);
            if (Netplay.clientSock.state == 2)
            {
                Netplay.clientSock.state = 3;
            }
        }
    }
}
