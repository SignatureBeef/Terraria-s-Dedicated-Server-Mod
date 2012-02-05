using System;

namespace Terraria_Server.Messages
{
    public class NPCTalkMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.NPC_TALK;
        }

        public override void Process (int whoAmI, byte[] readBuffer, int length, int num)
        {
            int playerIndex = whoAmI;

            int talkNPC = (int)BitConverter.ToInt16(readBuffer, num + 1);

            Main.players[playerIndex].talkNPC = talkNPC;

			NetMessage.SendData(40, -1, whoAmI, "", playerIndex);
        }
    }
}
