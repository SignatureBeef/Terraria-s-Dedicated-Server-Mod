using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Messages
{
    public class PlayerStateUpdateMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.PLAYER_STATE_UPDATE;
        }

        public int? GetRequiredNetMode()
        {
            return null;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = (int)readBuffer[num++];
            if (playerIndex == Main.myPlayer)
            {
                return;
            }

            Player player = Main.players[playerIndex];
            if (Main.netMode == 1 && !player.active)
            {
                NetMessage.SendData(15);
            }

            if (Main.netMode == 2)
            {
                playerIndex = whoAmI;
            }

            player.oldVelocity = player.velocity;

            int controlMap = (int)readBuffer[num++];
            player.selectedItemIndex = (int)readBuffer[num++];

            player.position.X = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            player.position.Y = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            player.velocity.X = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            player.velocity.Y = BitConverter.ToSingle(readBuffer, num);
            num += 4;

            player.fallStart = (int)(player.position.Y / 16f);

            player.controlUp = (controlMap & 1) == 1;
            player.controlDown = (controlMap & 2) == 2;
            player.controlLeft = (controlMap & 4) == 4;
            player.controlRight = (controlMap & 8) == 8;
            player.controlJump = (controlMap & 16) == 16;
            player.controlUseItem = (controlMap & 32) == 32;
            player.direction = (controlMap & 64) == 64 ? 1 : -1;

            if (Main.netMode == 2 && Netplay.serverSock[whoAmI].state == 10)
            {
                NetMessage.SendData(13, -1, whoAmI, "", playerIndex);
            }
        }
    }
}
