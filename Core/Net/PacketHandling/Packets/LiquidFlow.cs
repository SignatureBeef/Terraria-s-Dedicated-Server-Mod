using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;
using Microsoft.Xna.Framework;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class LiquidFlow : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.FLOW_LIQUID; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];
            
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            byte liquid = buffer.reader.ReadByte();
            byte liquidType = buffer.reader.ReadByte();
            
            if (Main.netMode == 2 && Netplay.spamCheck)
            {
                int centerX = (int)(Main.player[bufferId].position.X + (float)(Main.player[bufferId].width / 2));
                int centerY = (int)(Main.player[bufferId].position.Y + (float)(Main.player[bufferId].height / 2));
                int range = 10;
                int minX = centerX - range;
                int maxX = centerX + range;
                int minY = centerY - range;
                int maxY = centerY + range;
                if (x < minX || x > maxX || y < minY || y > maxY)
                {
                    NetMessage.BootPlayer(bufferId, "Cheating attempt detected: Liquid spam");
                    return true;
                }
            }
            
            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Player = player,
                Sender = player
            };
            
            var args = new TDSMHookArgs.LiquidFlowReceived
            {
                X = x,
                Y = y,
                Amount = liquid,
                Lava = liquidType == 1
            };
            
            TDSMHookPoints.LiquidFlowReceived.Invoke(ref ctx, ref args);
            
            if (ctx.CheckForKick())
                return true;
            
            if (ctx.Result == HookResult.IGNORE)
                return true;
            
            if (ctx.Result == HookResult.RECTIFY)
            {
                //                var msg = NewNetMessage.PrepareThreadInstance();
                //                msg.FlowLiquid(x, y);
                //                msg.Send(whoAmI);
                Terraria.NetMessage.SendData((int)Packet.FLOW_LIQUID, bufferId, -1, String.Empty, x, y);
                return true;
            }
            
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new OTA.Memory.MemTile();
            }
            lock (Main.tile[x, y])
            {
                Main.tile[x, y].liquid = liquid;
                Main.tile[x, y].liquidType((int)liquidType);
                if (Main.netMode == 2)
                {
                    WorldGen.SquareTileFrame(x, y, true);
                }
            }

            return true;
        }
    }
}

