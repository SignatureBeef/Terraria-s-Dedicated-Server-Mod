using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Plugin;
using Terraria_Server.Misc;
using Terraria_Server.Definitions;

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

            Player player = (Player)Main.players[playerIndex].Clone();
            
            playerIndex = whoAmI;

            player.oldVelocity = player.Velocity;

            int controlMap = (int)readBuffer[num++];
            player.selectedItemIndex = (int)readBuffer[num++];

            player.Position.X = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            player.Position.Y = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            player.Velocity.X = BitConverter.ToSingle(readBuffer, num);
            num += 4;
            player.Velocity.Y = BitConverter.ToSingle(readBuffer, num);
            num += 4;

            player.fallStart = (int)(player.Position.Y / 16f);

            player.controlUp = (controlMap & 1) == 1;
            player.controlDown = (controlMap & 2) == 2;
            player.controlLeft = (controlMap & 4) == 4;
            player.controlRight = (controlMap & 8) == 8;
            player.controlJump = (controlMap & 16) == 16;
            player.controlUseItem = (controlMap & 32) == 32;
            player.direction = (controlMap & 64) == 64 ? 1 : -1;
                        
            PlayerMoveEvent playerEvent = new PlayerMoveEvent();
            playerEvent.Sender = Main.players[playerIndex]; //We want to use the old player. (Possibly, to keep old Data?)
            playerEvent.Location = player.Position;
            playerEvent.Velocity = player.Velocity;
            playerEvent.FallStart = player.fallStart;
            Program.server.getPluginManager().processHook(Hooks.PLAYER_MOVE, playerEvent);
            if (playerEvent.Cancelled)
            {
                return;
            }

            PlayerKeyPressEvent playerInteractEvent = new PlayerKeyPressEvent();
            playerInteractEvent.Sender = Main.players[playerIndex];

            Key playerKeysPressed = new Key();
            playerKeysPressed.Up = player.controlUp;
            playerKeysPressed.Down = player.controlDown;
            playerKeysPressed.Left = player.controlLeft;
            playerKeysPressed.Right = player.controlRight;
            playerKeysPressed.Jump = player.controlJump;

            playerInteractEvent.KeysPressed = playerKeysPressed;
            playerInteractEvent.MouseClicked = player.controlUseItem;
            playerInteractEvent.FacingDirection = player.direction;
            Program.server.getPluginManager().processHook(Hooks.PLAYER_KEYPRESS, playerInteractEvent);
            if (playerEvent.Cancelled)
            {
                return;
            }

            Main.players[playerIndex] = player;

            if (Main.netMode == 2 && Netplay.serverSock[whoAmI].state == 10)
            {
                NetMessage.SendData(13, -1, whoAmI, "", playerIndex);
            }
        }
    }
}
