using System;
using tdsm.api;
using tdsm.api.Plugin;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class KillProjectileMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.KILL_PROJECTILE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            //TODO [KillProjectileReceived]

            int identity = (int)ReadInt16(readBuffer);
            int playerId = (int)ReadByte(readBuffer);

			if (playerId != whoAmI && Entry.EnableCheatProtection)
            {
                Terraria.Netplay.Clients[whoAmI].Kick("Cheating detected (KILL_PROJECTILE forgery).");
                return;
            }

            var index = Tools.FindExistingProjectileForUser(whoAmI, identity);
            if (index == -1) return;

            var player = Main.player[whoAmI];

            var ctx = new HookContext
            {
                Connection = player.Connection,
                Player = player,
                Sender = player,
            };

            var args = new HookArgs.KillProjectileReceived
            {
                Id = identity,
                Owner = whoAmI,
                Index = index,
            };

            HookPoints.KillProjectileReceived.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
            {
                return;
            }

            if (ctx.Result == HookResult.IGNORE)
            {
                return;
            }

            if (ctx.Result == HookResult.RECTIFY)
            {
                var msg = NewNetMessage.PrepareThreadInstance();
                msg.Projectile(Main.projectile[index]);
                msg.Send(whoAmI);
                return;
            }

            var projectile = Main.projectile[index];

            if (projectile.owner == whoAmI && projectile.identity == identity)
            {
                projectile.Kill();
                NewNetMessage.SendData(29, -1, whoAmI, String.Empty, identity, (float)whoAmI);
            }
        }
    }
}