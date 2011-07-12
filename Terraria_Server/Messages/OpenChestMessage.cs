using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Shops;
using Terraria_Server.Plugin;
using Terraria_Server.Events;

namespace Terraria_Server.Messages
{
    public class OpenChestMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.OPEN_CHEST;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int x = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int y = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int chestIndex = Chest.FindChest(x, y);

            var chestEvent = new PlayerChestOpenEvent();
            chestEvent.Sender = Main.players[whoAmI];
            chestEvent.ID = chestIndex;
            Program.server.PluginManager.processHook(Hooks.PLAYER_CHEST, chestEvent);
            if (chestEvent.Cancelled)
            {
                return;
            }

            if (chestIndex > -1 && Chest.UsingChest(chestIndex) == -1)
            {
                for (int i = 0; i < Chest.MAX_ITEMS; i++)
                {
                    NetMessage.SendData(32, whoAmI, -1, "", chestIndex, (float)i);
                }
                NetMessage.SendData(33, whoAmI, -1, "", chestIndex);
                Main.players[whoAmI].chest = chestIndex;
                return;
            }
        }
    }
}
