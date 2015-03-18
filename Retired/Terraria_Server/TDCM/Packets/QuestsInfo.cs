using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Logging;
using Terraria_Server.Messages;
using Terraria_Server.Networking;

namespace Terraria_Server.TDCM.Packets
{
    public class QuestsInfo : MessageHandler
    {
        public QuestsInfo()
		{
			ValidStates = SlotState.ALL;
		}
		
        public override Packet GetPacket()
        {
            return Packet.CLIENT_MOD_QUESTG_INFO;
        }

        public override void Process(ClientConnection conn,  byte[] readBuffer, int length, int num)
        {
            QNPC_Types qType    = (QNPC_Types)readBuffer[num++];
            int QuestID         = readBuffer[num++];
            if (QuestID == 255)
                QuestID = -1;

            if (qType != QNPC_Types.GHOST &&
                qType != QNPC_Types.ALCHEMIST &&
                qType != QNPC_Types.DARK_MAGE &&
                qType != QNPC_Types.PALADIN &&
                qType != QNPC_Types.TINKERER)
            {
                conn.Kick("Sent unknown Quest NPC Name ID."); 
                return;
            }

            if (!Main.players[conn.SlotIndex].HasClientMod)
            {
                conn.Kick("Sent Quest NPC Name Packet without permissions.");
                return;
            }

            if (!Server.AllowTDCMRPG)
            {
                conn.Kick("Invalid Client Message, Acting as TDCM");
                return;
            }

            if(QuestID > (int)QuestType.QUESTS_END || QuestID < (int)QuestType.NO_QUEST)
            {
                conn.Kick("Uknown Quest ID.");
                return;
            }

            //Set the players Quest NPC Name & Current Quest Id
            Main.players[conn.SlotIndex].QuestNPCName = QuestNPCType.GetQuestNPCName(qType);
            Main.players[conn.SlotIndex].CurrentQuest = QuestID;
        }
    }
}

