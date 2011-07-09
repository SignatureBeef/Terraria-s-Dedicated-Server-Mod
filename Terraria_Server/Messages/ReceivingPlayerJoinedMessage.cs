using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class ReceivingPlayerJoinedMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.RECEIVING_PLAYER_JOINED;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = whoAmI;

            Player player = Main.players[playerIndex];
            player.SpawnX = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            player.SpawnY = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            player.Spawn();

            if (Netplay.serverSock[whoAmI].state >= 3)
            {
                if (Netplay.serverSock[whoAmI].state == 3)
                {
                    Netplay.serverSock[whoAmI].state = 10;
                    NetMessage.greetPlayer(whoAmI);
                    NetMessage.syncPlayers();
                    NetMessage.buffer[whoAmI].broadcast = true;
                    NetMessage.SendData(12, -1, whoAmI, "", whoAmI);
                    return;
                }
                NetMessage.SendData(12, -1, whoAmI, "", whoAmI);
            }
        }
    }
}
