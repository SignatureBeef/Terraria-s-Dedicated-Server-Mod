using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.TDCM
{
    public enum QNPC_Types : int
    {
        GHOST = 0,
        ALCHEMIST = 1,
        DARK_MAGE = 2,
        PALADIN = 3,
        TINKERER = 4,
        UNKNOWN = 5
    }

    public static class QuestNPCType
    {
        public static string GetQuestNPCName(QNPC_Types Id)
        {
            string name;
            switch (Id)
            {
                case QNPC_Types.GHOST:
                    name = QuestNPCNames.GHOST;
                    break;
                case QNPC_Types.ALCHEMIST:
                    name = QuestNPCNames.ALCHEMIST;
                    break;
                case QNPC_Types.DARK_MAGE:
                    name = QuestNPCNames.DARK_MAGE;
                    break;
                case QNPC_Types.PALADIN:
                    name = QuestNPCNames.PALADIN;
                    break;
                case QNPC_Types.TINKERER:
                    name = QuestNPCNames.TINKERER;
                    break;
                default:
                    name = Statics.TDCM_QUEST_GIVER;
                        break;
            }

            return name;
        }
    }

    public class QuestNPCNames
    {
        public const string PREFIX      = "Ghostly ";
        public const string GHOST       = "Mythic Ghost";
        public const string ALCHEMIST   = PREFIX + "Alchemist";
        public const string DARK_MAGE   = PREFIX + "Mage"; 
        public const string PALADIN     = PREFIX + "Paladin";
        public const string TINKERER    = PREFIX + "Tinkerer";
    }
}
