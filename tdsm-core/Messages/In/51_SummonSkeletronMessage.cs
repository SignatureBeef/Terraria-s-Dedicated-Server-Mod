using System;
using tdsm.api.Plugin;
using tdsm.core.Messages.Out;
using tdsm.core.ServerCore;
using Terraria;


namespace tdsm.core.Messages.In
{
    public class SummonSkeletronMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.SUMMON_SKELETRON;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerId = ReadByte(readBuffer);
            byte action = ReadByte(readBuffer);

            if (playerId != whoAmI)
            {
                Server.slots[whoAmI].Kick("SummonSkeletron Player Forgery.");
                return;
            }

            if (action == 1)
            {
                var player = Main.player[whoAmI];

                var ctx = new HookContext
                {
                    Connection = player.Connection,
                    Sender = player,
                    Player = player,
                };

                var args = new HookArgs.PlayerTriggeredEvent
                {
                    Type = WorldEventType.BOSS,
                    Name = "Skeletron",
                };

                HookPoints.PlayerTriggeredEvent.Invoke(ref ctx, ref args);

                if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                    return;

                //ProgramLog.Users.Log ("{0} @ {1}: Skeletron summoned by {2}.", player.IPAddress, whoAmI, player.Name);
                //NewNetMessage.SendData (Packet.PLAYER_CHAT, -1, -1, string.Concat (player.Name, " has summoned Skeletron!"), 255, 255, 128, 150);
                NPC.SpawnSkeletron();
            }
            else if (action == 2)
            {
                NewNetMessage.SendData(51, -1, whoAmI, String.Empty, playerId, action, 0f, 0f, 0);
            }
        }
    }
}
