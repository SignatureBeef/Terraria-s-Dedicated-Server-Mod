using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;
using Microsoft.Xna.Framework;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class WriteSign : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.WRITE_SIGN; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            var player = Main.player[bufferId];
            
            int signId = (int)buffer.reader.ReadInt16();
            int x = (int)buffer.reader.ReadInt16();
            int y = (int)buffer.reader.ReadInt16();
            string text = buffer.reader.ReadString();
            
            string existing = null;
            if (Main.sign[signId] != null)
            {
                existing = Main.sign[signId].text;
            }
            
            Main.sign[signId] = new Sign();
            Main.sign[signId].x = x;
            Main.sign[signId].y = y;
            
            Sign.TextSign(signId, text);
            int ply = (int)buffer.reader.ReadByte();

            var ctx = new HookContext
            {
                Connection = player.Connection.Socket,
                Sender = player,
                Player = player,
            };
            
            var args = new TDSMHookArgs.SignTextSet
            {
                X = x,
                Y = y,
                SignIndex = signId,
                Text = text,
                OldSign = Main.sign[signId],
            };
            
            TDSMHookPoints.SignTextSet.Invoke(ref ctx, ref args);
            
            if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                return true;
            
            if (Main.netMode == 2 && existing != text)
            {
                ply = bufferId;
                NetMessage.SendData(47, -1, bufferId, "", signId, (float)ply, 0, 0, 0, 0, 0);
            }
            
            if (Main.netMode == 1 && ply == Main.myPlayer && Main.sign[signId] != null)
            {
                Main.playerInventory = false;
                Main.player[Main.myPlayer].talkNPC = -1;
                Main.npcChatCornerItem = 0;
                Main.editSign = false;
                Main.PlaySound(10, -1, -1, 1);
                Main.player[Main.myPlayer].sign = signId;
                Main.npcChatText = Main.sign[signId].text;
            }

            return true;
        }
    }
}

