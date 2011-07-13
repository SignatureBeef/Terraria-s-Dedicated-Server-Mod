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

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = whoAmI;

            Player player = Main.players[playerIndex];
            player.SpawnX = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            player.SpawnY = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            player.Spawn();

            if (Netplay.slots[whoAmI].state >= SlotState.SENDING_TILES)
            {
                if (Netplay.slots[whoAmI].state == SlotState.SENDING_TILES)
                {
                    Netplay.slots[whoAmI].state = SlotState.PLAYING;
                    NetMessage.GreetPlayer(whoAmI);
                    NetMessage.SyncPlayers();
                    NetMessage.SendData(12, -1, whoAmI, "", whoAmI);
                    return;
                }
                NetMessage.SendData(12, -1, whoAmI, "", whoAmI);
            }
        }
    }
}
