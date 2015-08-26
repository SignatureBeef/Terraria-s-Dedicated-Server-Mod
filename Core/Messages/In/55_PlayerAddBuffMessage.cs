using System;
using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class PlayerAddBuffMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.PLAYER_ADD_BUFF;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            //var playerId = readBuffer[num++];
            //var type = readBuffer[num++];
            //var time = BitConverter.ToInt16(readBuffer, num);

            //if (type > 26 || time > Player.MAX_BUFF_TIME
            //    || (playerId != whoAmI && (
            //    (
            //        type != (int)ProjectileType.N20_GREEN_LASER &&
            //        type != (int)ProjectileType.N24_SPIKY_BALL)
            //    || time > Player.MAX_BUFF_TIME)))
            //{
            //    ProgramLog.Log("PLAYER_ADD_BUFF: from={0}, for={1}, type={2}, time={3}", whoAmI, playerId, type, time);
            //    tdsm.api.Callbacks.Netplay.slots[whoAmI].Kick("Cheating detected (PLAYER_ADD_BUFF forgery).");
            //    return;
            //}

            //var entityId = whoAmI;
            //if (type == (int)ProjectileType.N20_GREEN_LASER ||
            //    type == (int)ProjectileType.N24_SPIKY_BALL)
            //    entityId = playerId;

            //Main.player[entityId].AddBuff(type, time, true);

            //NewNetMessage.SendData(55, entityId, -1, String.Empty, entityId, type, time, 0f, 0);



            int num129 = (int)ReadByte(readBuffer);
            int num130 = (int)ReadByte(readBuffer);
            int num131 = (int)ReadInt16(readBuffer);
            if (num129 != whoAmI && !Main.pvpBuff[num130])
            {
                return;
            }

            NewNetMessage.SendData(55, num129, -1, String.Empty, num129, (float)num130, (float)num131, 0f, 0);
        }
    }
}
