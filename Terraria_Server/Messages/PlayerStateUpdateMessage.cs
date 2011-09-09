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
    public class PlayerStateUpdateMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_STATE_UPDATE;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            if (readBuffer[num++] != whoAmI)
            {
                NetPlay.slots[whoAmI].Kick ("Cheating detected (PLAYER_STATE_UPDATE forgery).");
                return;
            }
            
            Player player = Main.players[whoAmI];
            
            var data = new PlayerStateUpdateData ();
            data.Parse (readBuffer, num);
            
            var fallStart = (int)(player.Position.Y / 16f);

            PlayerMoveEvent playerEvent = new PlayerMoveEvent();
            playerEvent.Sender = player;
            playerEvent.Location = data.position;
            playerEvent.Velocity = data.velocity;
            playerEvent.FallStart = fallStart;
            Server.PluginManager.processHook(Hooks.PLAYER_MOVE, playerEvent);
            if (playerEvent.Cancelled)
            // does this even make sense? authoritative player location is kept client-side
            {
                return;
            }

            player.oldVelocity = player.Velocity;            
            
            data.ApplyParams (player);
            player.fallStart = fallStart;

            PlayerKeyPressEvent playerInteractEvent = new PlayerKeyPressEvent();
            playerInteractEvent.Sender = player;

            Key playerKeysPressed = new Key();
            playerKeysPressed.Up = data.ControlUp;
            playerKeysPressed.Down = data.ControlDown;
            playerKeysPressed.Left = data.ControlLeft;
            playerKeysPressed.Right = data.ControlRight;
            playerKeysPressed.Jump = data.ControlJump;

            playerInteractEvent.KeysPressed = playerKeysPressed;
            playerInteractEvent.MouseClicked = player.controlUseItem;
            playerInteractEvent.FacingDirection = player.direction;
            Server.PluginManager.processHook(Hooks.PLAYER_KEYPRESS, playerInteractEvent);
            if (playerEvent.Cancelled) // does this even make sense?
            {
                NetMessage.SendData(13, -1, whoAmI, "", whoAmI);
                return;
            }
            
            data.ApplyKeys (player);

            if (NetPlay.slots[whoAmI].state == SlotState.PLAYING)
            {
                NetMessage.SendData(13, -1, whoAmI, "", whoAmI);
            }
        }
    }
}
