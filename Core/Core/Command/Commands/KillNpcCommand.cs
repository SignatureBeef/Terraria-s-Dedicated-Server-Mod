using System;
using Terraria;
using OTA.Command;
using OTA;
using Microsoft.Xna.Framework;

namespace TDSM.Core.Command.Commands
{
    public class KillNpcCommand : CoreCommand
    {
        public override void Initialise()
        {
            Core.AddCommand("killnpc")
                .WithAccessLevel(AccessLevel.OP)
                .WithPermissionNode("tdsm.killnpc")
                .WithDescription("Kill all non town NPC's")
                .Calls(this.KillNPC);
        }

        void KillNPC(ISender sender, ArgumentList args)
        {
            if (Main.rand == null)
                Main.rand = new Random();

            if (Core._likeABoss) Core._likeABoss = false;

            int killed = 0;
            foreach (var npc in Main.npc)
            {
                if (npc != null && npc.active && !npc.townNPC && npc.whoAmI < 255)
                {
                    int damage = Int32.MaxValue;
                    npc.StrikeNPC(damage, 0, 0);
                    NetMessage.SendData((int)Packet.STRIKE_NPC, -1, -1, "", npc.whoAmI, damage);
                    NetMessage.SendData((int)Packet.NPC_INFO, -1, -1, "", npc.whoAmI, 0, 0, 0, 0, 0, 0);

                    killed++;
                }
            }

            sender.Message("Killed {0} npc(s)", Color.Green, killed);
        }
    }
}

