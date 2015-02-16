using System;
using tdsm.api;
using tdsm.api.Plugin;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class OpenChestMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.OPEN_CHEST;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int x = ReadInt16(readBuffer);
            int y = ReadInt16(readBuffer);

            var player = Main.player[whoAmI];

            if (Math.Abs(player.position.X / 16 - x) >= 7 || Math.Abs(player.position.Y / 16 - y) >= 7)
            {
                return;
            }

            int chestIndex = Chest.FindChest(x, y);

            var ctx = new HookContext
            {
                Connection = player.Connection,
                Player = player,
                Sender = player,
            };

            var args = new HookArgs.ChestOpenReceived
            {
                X = x,
                Y = y,
                ChestIndex = chestIndex,
            };

            HookPoints.ChestOpenReceived.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
            {
                return;
            }

            if (ctx.Result == HookResult.IGNORE)
            {
                return;
            }

            if (ctx.Result == HookResult.DEFAULT && chestIndex > -1)
            {
                var user = Chest.UsingChest(chestIndex);
                if (user >= 0 && user != whoAmI) return;

                for (int i = 0; i < Chest.maxItems; i++)
                {
                    NewNetMessage.SendData(32, whoAmI, -1, String.Empty, chestIndex, (float)i);
                }
                NewNetMessage.SendData(33, whoAmI, -1, String.Empty, chestIndex);
                Main.player[whoAmI].chest = chestIndex;
                return;
            }
        }
    }
}
