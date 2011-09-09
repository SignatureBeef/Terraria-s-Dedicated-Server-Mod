using System;
using Terraria_Server.Events;
using Terraria_Server.Misc;
using Terraria_Server.Plugin;
using Terraria_Server.WorldMod;

namespace Terraria_Server.Messages
{
    public class KillTileMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.KILL_TILE;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            if (Main.tile.At(x, y).Type == 21)
            {
                PlayerChestBreakEvent playerEvent = new PlayerChestBreakEvent();
                playerEvent.Sender = Main.players[whoAmI];
                playerEvent.Location = new Vector2(x, y);
                Server.PluginManager.processHook(Hooks.PLAYER_CHESTBREAK, playerEvent);
                if (playerEvent.Cancelled)
                {
                    NetMessage.SendTileSquare(whoAmI, x, y, 1);
                    return;
                }

                WorldModify.KillTile(x, y);
                if (!Main.tile.At(x, y).Active)
                {
                    NetMessage.SendData(17, -1, -1, "", 0, (float)x, (float)y);
                }
            }
        }
    }
}
