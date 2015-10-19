using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;
using Microsoft.Xna.Framework;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class ChestOpen : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.OPEN_CHEST; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];
            
            if (Main.netMode != 2)
                return true;
            
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            
            if (Math.Abs(player.position.X / 16 - x) >= 7 || Math.Abs(player.position.Y / 16 - y) >= 7)
                return true;
            
            int chestIndex = Chest.FindChest(x, y);
            if (chestIndex <= -1 || Chest.UsingChest(chestIndex) != -1)
                return true;
            
            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Player = player,
                Sender = player
            };
            
            var args = new TDSMHookArgs.ChestOpenReceived
            {
                X = x,
                Y = y,
                ChestIndex = chestIndex
            };
            
            TDSMHookPoints.ChestOpenReceived.Invoke(ref ctx, ref args);
            
            if (ctx.CheckForKick())
                return true;
            
            if (ctx.Result == HookResult.IGNORE)
                return true;
            
            if (ctx.Result == HookResult.DEFAULT && chestIndex > -1)
            {
                for (int i = 0; i < 40; i++)
                {
                    NetMessage.SendData(32, bufferId, -1, "", chestIndex, (float)i, 0, 0, 0, 0, 0);
                }
                NetMessage.SendData(33, bufferId, -1, "", chestIndex, 0, 0, 0, 0, 0, 0);
                Main.player[bufferId].chest = chestIndex;
                if (Main.myPlayer == bufferId)
                {
                    Main.recBigList = false;
                }
            
                NetMessage.SendData(80, -1, bufferId, "", bufferId, (float)chestIndex, 0, 0, 0, 0, 0);
            }

            return true;
        }
    }
}

