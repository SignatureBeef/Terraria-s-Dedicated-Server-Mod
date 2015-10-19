using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;
using Microsoft.Xna.Framework;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class BarrierStateChange : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.BARRIER_STATE_CHANGE; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            byte kind = buffer.reader.ReadByte();
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            if (!WorldGen.InWorld(x, y, 3)) return true;
            int direction = (buffer.reader.ReadByte() == 0) ? -1 : 1;
            
            var args = new TDSMHookArgs.BarrierStateChange()
            {
                X = x,
                Y = y,
                Direction = direction,
                Kind = kind
            };
            var ctx = new HookContext()
            {
                Sender = Main.player[bufferId],
                Player = Main.player[bufferId]
            };
            
            TDSMHookPoints.BarrierStateChange.Invoke(ref ctx, ref args);
            
            if (ctx.Result == HookResult.DEFAULT)
            {
                if (kind == 0) WorldGen.OpenDoor(x, y, direction);
                else if (kind == 1) WorldGen.CloseDoor(x, y, true);
                else if (kind == 2) WorldGen.ShiftTrapdoor(x, y, direction == 1, 1);
                else if (kind == 3) WorldGen.ShiftTrapdoor(x, y, direction == 1, 0);
                else if (kind == 4) WorldGen.ShiftTallGate(x, y, false);
                else if (kind == 5) WorldGen.ShiftTallGate(x, y, true);
            
                if (Main.netMode == 2)
                    NetMessage.SendData((int)Packet.BARRIER_STATE_CHANGE, -1, bufferId, "", (int)kind, (float)x, (float)y, (float)((direction == 1) ? 1 : 0));
            }
            else if (ctx.Result == HookResult.RECTIFY)
            {
                if (Main.netMode == 2)
                {
                    //Teleport
                    ctx.Player.Teleport(args.Position);
            
                    //I would think to send the real door state
                    if (kind == 0) kind = 1;
                    else if (kind == 1) kind = 0;
                    else if (kind == 2) kind = 3;
                    else if (kind == 3) kind = 2;
                    else if (kind == 4) kind = 5;
                    else if (kind == 5) kind = 4;
            
                    if (Main.netMode == 2)
                        NetMessage.SendData((int)Packet.BARRIER_STATE_CHANGE, -1, bufferId, "", (int)kind, (float)x, (float)y, (float)((direction == 1) ? 1 : 0));
                }
            }

            return true;
        }
    }
}

