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
            int playerIndex = readBuffer[num];
            
            if (playerIndex != whoAmI)
            {
                Netplay.slots[whoAmI].Kick ("Cheating detected (RECEIVING_PLAYER_JOINED forgery).");
                return;
            }

            playerIndex = whoAmI;
            num++;
            
            Player player = Main.players[playerIndex];
            
            if (player.SpawnX >= 0 && player.SpawnY >= 0)
            {
                player.OldSpawnX = player.SpawnX;
                player.OldSpawnY = player.SpawnY;
            }

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
                    NetMessage.OnPlayerJoined (whoAmI); // this also forwards the message
                }
                else
                {
                    var msg = NetMessage.PrepareThreadInstance ();
                    if (player.SpawnX == -1 && player.SpawnY == -1 && player.TeleSpawnX != -1)
                    {
                        msg.ReceivingPlayerJoined (whoAmI, player.TeleSpawnX, player.TeleSpawnY);
                        player.TeleSpawnX = -1;
                        player.TeleSpawnY = -1;
                    }
                    else
                        msg.ReceivingPlayerJoined (whoAmI);
                    msg.BroadcastExcept (whoAmI);
                }
            }
        }
    }
}
