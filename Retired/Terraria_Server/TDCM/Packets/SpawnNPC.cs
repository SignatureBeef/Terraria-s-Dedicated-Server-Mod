using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Logging;
using Terraria_Server.Messages;
using Terraria_Server.Misc;
using Terraria_Server.TDCM.Packets.Quests;

namespace Terraria_Server.TDCM.Packets
{
    public class QuestKicker : Exception
    {
        public QuestKicker() { }

        public QuestKicker(string Info) : base(Info) { }
    }

    public class SpawnNPC : SlotMessageHandler
    {
        public SpawnNPC()
        {
            ValidStates = SlotState.SENDING_TILES | SlotState.PLAYING;
            NPC.NPCSpawnHandler += new NPC.NPCSpawn(QuestActions.NPC_NPCSpawnHandler);
        }

        public override Packet GetPacket()
        {
            return Packet.CLIENT_MOD_SPAWN_NPC;
        }

        public override void Process(int whoAmI, byte[] readBuffer, int length, int num)
        {
            var player = Main.players[whoAmI];

            if (!player.HasClientMod || !Server.AllowTDCMRPG)
            {
                player.Kick("Invalid Client Message, Acting as TDCM");
                return;
            }

            QuestType   type = (QuestType)readBuffer[num++];
            int         Args = readBuffer[num++];

            //Will hardcode NPC spawns to prevent hacks.
            try
            {
                if (type >= QuestType.SLIME_QUEST && type < QuestType.ALCHEMIST_1)
                    CheckQuest(QuestNPCNames.GHOST, player, type);
                else if (type >= QuestType.ALCHEMIST_1 && type < QuestType.TINKERER_1)
                    CheckQuest(QuestNPCNames.ALCHEMIST, player, type);
                else if (type >= QuestType.TINKERER_1 && type < QuestType.DARK_MAGE_1)
                    CheckQuest(QuestNPCNames.TINKERER, player, type);
                else if (type >= QuestType.DARK_MAGE_1 && type < QuestType.PALADIN_1)
                    CheckQuest(QuestNPCNames.DARK_MAGE, player, type);
                else if (type >= QuestType.PALADIN_1 && type < QuestType.QUESTS_END)
                    CheckQuest(QuestNPCNames.PALADIN, player, type);
                else
                    player.Kick("TDCM Quest Packet Forgery.");
            }
            catch (QuestKicker)
            {
                player.Kick("TDCM Quest NPC Name Forgery.");
            } 
        }

        /// <summary>
        /// Check's whether the Player meets requirements and processes from there.
        /// </summary>
        /// <param name="QuestGiverName"></param>
        /// <param name="player"></param>
        /// <param name="type"></param>
        public void CheckQuest(string QuestGiverName, Player player, QuestType type)
        {
            if (QuestGiverName != player.QuestNPCName)
                throw new QuestKicker();

            //[TODO] More checks

            //[TODO] More Quest crap
            switch (type)
            {
                case QuestType.NO_QUEST:
                case QuestType.SLIME_QUEST:
                case QuestType.CRAFTING_QUEST:
                case QuestType.MINING_QUEST:
                case QuestType.TRANSFORMATION:
                case QuestType.ALCHEMIST_1:
                case QuestType.ALCHEMIST_2:
                case QuestType.ALCHEMIST_3:
                case QuestType.TINKERER_1:
                case QuestType.DARK_MAGE_1:
                case QuestType.DARK_MAGE_2:
                case QuestType.PALADIN_1:
                case QuestType.PALADIN_2:
                case QuestType.QUESTS_END:
                    break;
                case QuestType.TINKERER_2:
                    QuestActions.Tinkerer_2(player);
                    break;
                case QuestType.TINKERER_3:
                    QuestActions.Tinkerer_3(player);
                    break;
                case QuestType.TINKERER_4:
                    QuestActions.Tinkerer_4(player);
                    break;
                case QuestType.DARK_MAGE_3:
                    QuestActions.Dark_Mage_3(player);
                    break;
                case QuestType.DARK_MAGE_4:
                    QuestActions.Dark_Mage_4(player);
                    break;
                case QuestType.PALADIN_3:
                    QuestActions.Paladin_3(player);
                    break;
            }
        }
    }
}