using System;
using Terraria;
using OTA;
using OTA.ID;
using OTA.Plugin;
using TDSM.Core.Plugin.Hooks;

namespace TDSM.Core.Net.PacketHandling.Packets
{
    public class NpcHurt : IPacketHandler
    {
        public Packet PacketId
        {
            get { return Packet.DAMAGE_NPC; }
        }

        public bool Read(int bufferId, int start, int length)
        {
            var buffer = NetMessage.buffer[bufferId];

            var npcId = (int)buffer.reader.ReadInt16();
            var damage = (int)buffer.reader.ReadInt16();
            var knockBack = buffer.reader.ReadSingle();
            var hitDirection = (int)(buffer.reader.ReadByte() - 1);
            var critical = buffer.reader.ReadByte();

            if (Main.netMode == 2)
            {
                if (damage < 0) damage = 0;
                Main.npc[npcId].PlayerInteraction(buffer.whoAmI);
            }

            var ply = Main.player[bufferId];
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

            if (damage >= 0)
                Main.npc[npcId].StrikeNPC(damage, knockBack, hitDirection, critical == 1, false, true);
            else
            {
                Main.npc[npcId].life = 0;
                Main.npc[npcId].HitEffect(0, 10.0);
                Main.npc[npcId].active = false;
            }

            if (Main.netMode != 2)
                return true;

            NetMessage.SendData(28, -1, buffer.whoAmI, String.Empty, npcId, (float)damage, knockBack, (float)hitDirection, (int)critical);
            if (Main.npc[npcId].life <= 0)
                NetMessage.SendData(23, -1, -1, String.Empty, npcId);
            else Main.npc[npcId].netUpdate = true;

            if (Main.npc[npcId].realLife < 0)
                return true;

            if (Main.npc[Main.npc[npcId].realLife].life <= 0)
            {
                NetMessage.SendData(23, -1, -1, String.Empty, Main.npc[npcId].realLife);
                return true;
            }

            Main.npc[Main.npc[npcId].realLife].netUpdate = true;
            return true;
        }
    }
}

