using tdsm.api;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.Messages.In
{
    public class NPCHomeMessage : SlotMessageHandler
    {
        public override Packet GetPacket()
        {
            return Packet.NPC_HOME;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            int num137 = (int)ReadInt16(readBuffer);
            int num138 = (int)ReadInt16(readBuffer);
            int num139 = (int)ReadInt16(readBuffer);
            byte b10 = ReadByte(readBuffer);
            if (num137 >= 200 && Entry.EnableCheatProtection)
            {
                NewNetMessage.BootPlayer(whoAmI, "Cheating attempt detected: Invalid kick-out");
                return;
            }
            if (Main.netMode == 1)
            {
                Main.npc[num137].homeless = (b10 == 1);
                Main.npc[num137].homeTileX = num138;
                Main.npc[num137].homeTileY = num139;
                return;
            }
            if (b10 == 0)
            {
                WorldGen.kickOut(num137);
                return;
            }
            WorldGen.moveRoom(num138, num139, num137);
            return;
        }
    }
}
