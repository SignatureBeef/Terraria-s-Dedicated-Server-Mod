using System;

namespace Terraria_Server.Messages
{
    public class NPCTalkMessage : IMessage
    {

        public Packet GetPacket()
        {
            return Packet.NPC_TALK;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int playerIndex = playerIndex = whoAmI;

            int talkNPC = (int)BitConverter.ToInt16(readBuffer, num);
            num += 2;
            Main.players[playerIndex].talkNPC = talkNPC;
            
            NetMessage.SendData(40, -1, whoAmI, "", playerIndex);
        }
    }
}
