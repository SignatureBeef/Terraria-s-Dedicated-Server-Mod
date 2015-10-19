using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;
using Microsoft.Xna.Framework;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class ReadSign : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.READ_SIGN; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];
            
            if (Main.netMode != 2)
            {
                return true;
            }
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            int id = Sign.ReadSign(x, y, true);
            
            if (id >= 0)
            {
                var ctx = new HookContext
                {
                    Connection = player.Connection.Socket,
                    Sender = player,
                    Player = player
                };
            
                var args = new TDSMHookArgs.SignTextGet
                {
                    X = x,
                    Y = y,
                    SignIndex = (short)id,
                    Text = (id >= 0 && Main.sign[id] != null) ? Main.sign[id].text : null,
                };
            
                TDSMHookPoints.SignTextGet.Invoke(ref ctx, ref args);
            
                if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                    return true;
            
                if (args.Text != null)
                {
                    NetMessage.SendData(47, bufferId, -1, "", id, (float)bufferId, 0, 0, 0, 0, 0);
                }
            }
            return true;
        }
    }
}

