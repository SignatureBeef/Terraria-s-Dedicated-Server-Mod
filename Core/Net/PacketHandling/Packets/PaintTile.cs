using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class PaintTile : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.PAINT_TILE; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];

            var x = (int)buffer.reader.ReadInt16();
            var y = (int)buffer.reader.ReadInt16();
            var colour = buffer.reader.ReadByte();

            if (!WorldGen.InWorld(x, y, 3))
            {
                return true;
            }

            var player = Main.player[bufferId];

            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Sender = player,
                Player = player,
            };

            var args = new TDSMHookArgs.PaintTile
            {
                X = x,
                Y = y,
                Colour = colour
            };

            TDSMHookPoints.PaintTile.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
                return true;

            if (ctx.Result == HookResult.IGNORE)
                return true;

            if (ctx.Result == HookResult.RECTIFY)
            {
                NetMessage.SendTileSquare(bufferId, x, y, 1);
                return true;
            }
            
            WorldGen.paintTile(x, y, colour, false);
            if (Main.netMode == 2)
            {
                NetMessage.SendData(63, -1, bufferId, "", x, (float)y, (float)colour, 0f, 0, 0, 0);
            }
            return true;
        }
    }
}

