using tdsm.api.Plugin;
using tdsm.core.Logging;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class KillPlayerPvPMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.KILL_PLAYER_PVP;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int start = num - 1;
            int playerIndex = ReadByte(readBuffer);

            if (playerIndex != whoAmI)
            {
                Server.slots[whoAmI].Kick("Cheating detected (KILL_PLAYER forgery).");
                return;
            }

            var player = Main.player[whoAmI];

            var ctx = new HookContext
            {
                Connection = Server.slots[whoAmI].conn,
                Sender = player,
                Player = player,
            };

            var args = new HookArgs.ObituaryReceived
            {
                Direction = (int)ReadByte(readBuffer) - 1,
                Damage = ReadInt16(readBuffer),
                PvpFlag = ReadByte(readBuffer)
            };
            //string obituary;
            //if (!ParseString(readBuffer, num + 1, length - num - 1 + start, out obituary))
            //{
            //    Server.slots[whoAmI].Kick("Invalid characters in obituary message.");
            //    return;
            //}
            args.Obituary = " " + ReadString(readBuffer);

            HookPoints.ObituaryReceived.Invoke(ref ctx, ref args);

            if (ctx.CheckForKick())
                return;

            if (ctx.Result == HookResult.IGNORE)
                return;

            ProgramLog.Log("{0} @ {1}: [Death] {2}{3}", player.IPAddress, whoAmI, player.Name ?? "<null>", args.Obituary);

            player.KillMe(args.Damage, args.Direction, args.PvpFlag == 1, args.Obituary);

            NewNetMessage.SendData(44, -1, whoAmI, args.Obituary, whoAmI, args.Direction, args.Damage, args.PvpFlag);
        }
    }
}
