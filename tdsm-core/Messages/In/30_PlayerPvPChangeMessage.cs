using System;
using tdsm.api;
using tdsm.api.Plugin;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PlayerPvPChangeMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_PVP_CHANGE;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = ReadByte(readBuffer);

			if (playerIndex != whoAmI && Entry.EnableCheatProtection)
            {
                Terraria.Netplay.serverSock[whoAmI].Kick("Cheating detected (PLAYER_PVP_CHANGE forgery).");
                return;
            }

            var pvp = ReadByte(readBuffer) == 1;
            var player = Main.player[whoAmI];

            var ctx = new HookContext
            {
                Connection = player.Connection,
                Player = player,
                Sender = player,
            };

            var args = new HookArgs.PvpSettingReceived
            {
                PvpFlag = pvp,
            };

            HookPoints.PvpSettingReceived.Invoke(ref ctx, ref args);

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
                NewNetMessage.SendData(30, whoAmI, -1, String.Empty, whoAmI);
                return;
            }

            player.hostile = pvp;

            string message = (player.hostile) ? " has enabled PvP!" : " has disabled PvP!";

            NewNetMessage.SendData(30, -1, whoAmI, String.Empty, whoAmI);
            NewNetMessage.SendData(25, -1, -1, player.Name + message, 255, (float)Main.teamColor[player.team].R, (float)Main.teamColor[player.team].G, (float)Main.teamColor[player.team].B);
        }
    }
}
