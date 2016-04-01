using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class NpcStrike : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.STRIKE_NPC; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];
            
            int npcId = (int)buffer.reader.ReadInt16();
            int playerId = (int)buffer.reader.ReadByte();
            
            if (Main.netMode == 2)
                playerId = bufferId;
            
            var ply = Main.player[playerId];
            var ctx = new HookContext()
            {
                Sender = ply,
                Player = ply
            };
            var args = new TDSMHookArgs.NpcHurtReceived()
            {
                Victim = Terraria.Main.npc[npcId],
                Damage = ply.inventory[ply.selectedItem].damage,
                HitDirection = ply.direction,
                Knockback = ply.inventory[ply.selectedItem].knockBack,
                Critical = false
            };
            
            TDSMHookPoints.NpcHurtReceived.Invoke(ref ctx, ref args);
            
            if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE) return true;
            
            Terraria.Main.npc[npcId].StrikeNPC(args.Damage, args.Knockback, args.HitDirection, false, false, false);
            
            if (Main.netMode == 2)
            {
                NetMessage.SendData(24, -1, bufferId, "", npcId, (float)playerId, 0, 0, 0, 0, 0);
                NetMessage.SendData(23, -1, -1, "", npcId, 0, 0, 0, 0, 0, 0);
            }
            return true;
        }
    }
}

