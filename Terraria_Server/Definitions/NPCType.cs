using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Terraria_Server.Definitions
{
    public enum NPCType : int
    {
        [XmlEnum(Name = "0")]
        N00_UNKNOWN,
        [XmlEnum(Name = "1")]
        N01_BLUE_SLIME,
        [XmlEnum(Name = "2")]
        N02_DEMON_EYE,
        [XmlEnum(Name = "3")]
        N03_ZOMBIE,
        [XmlEnum(Name = "4")]
        N04_EYE_OF_CTHULU,
        [XmlEnum(Name = "5")]
        N05_SERVANT_OF_CTHULU,
        [XmlEnum(Name = "6")]
        N06_EATER_OF_SOULS,
        [XmlEnum(Name = "7")]
        N07_DEVOURER_HEAD,
        [XmlEnum(Name = "8")]
        N08_DEVOURER_BODY,
        [XmlEnum(Name = "9")]
        N09_DEVOURER_TAIL,
        [XmlEnum(Name = "10")]
        N10_GIANT_WORM_HEAD,
        [XmlEnum(Name = "11")]
        N11_GIANT_WORM_BODY,
        [XmlEnum(Name = "12")]
        N12_GIANT_WORM_TAIL,
        [XmlEnum(Name = "13")]
        N13_EATER_OF_WORLDS_HEAD,
        [XmlEnum(Name = "14")]
        N14_EATER_OF_WORLDS_BODY,
        [XmlEnum(Name = "15")]
        N15_EATER_OF_WORLDS_TAIL,
        [XmlEnum(Name = "16")]
        N16_MOTHER_SLIME,
        [XmlEnum(Name = "17")]
        N17_MERCHANT,
        [XmlEnum(Name = "18")]
        N18_NURSE,
        [XmlEnum(Name = "19")]
        N19_ARMS_DEALER,
        [XmlEnum(Name = "20")]
        N20_DRYAD,
        [XmlEnum(Name = "21")]
        N21_SKELETON,
        [XmlEnum(Name = "22")]
        N22_GUIDE,
        [XmlEnum(Name = "23")]
        N23_METEOR_HEAD,
        [XmlEnum(Name = "24")]
        N24_FIRE_IMP,
        [XmlEnum(Name = "25")]
        N25_BURNING_SPHERE,
        [XmlEnum(Name = "26")]
        N26_GOBLIN_PEON,
        [XmlEnum(Name = "27")]
        N27_GOBLIN_THIEF,
        [XmlEnum(Name = "28")]
        N28_GOBLIN_WARRIOR,
        [XmlEnum(Name = "29")]
        N29_GOBLIN_SORCERER,
        [XmlEnum(Name = "30")]
        N30_CHAOS_BALL,
        [XmlEnum(Name = "31")]
        N31_ANGRY_BONES,
        [XmlEnum(Name = "32")]
        N32_DARK_CASTER,
        [XmlEnum(Name = "33")]
        N33_WATER_SPHERE,
        [XmlEnum(Name = "34")]
        N34_CURSED_SKULL,
        [XmlEnum(Name = "35")]
        N35_SKELETRON_HEAD,
        [XmlEnum(Name = "36")]
        N36_SKELETRON_HAND,
        [XmlEnum(Name = "37")]
        N37_OLD_MAN,
        [XmlEnum(Name = "38")]
        N38_DEMOLITIONIST,
        [XmlEnum(Name = "39")]
        N39_BONE_SERPENT_HEAD,
        [XmlEnum(Name = "40")]
        N40_BONE_SERPENT_BODY,
        [XmlEnum(Name = "41")]
        N41_BONE_SERPENT_TAIL,
        [XmlEnum(Name = "42")]
        N42_HORNET,
        [XmlEnum(Name = "43")]
        N43_MAN_EATER,
        [XmlEnum(Name = "44")]
        N44_UNDEAD_MINER,
        [XmlEnum(Name = "45")]
        N45_TIM,
        [XmlEnum(Name = "46")]
        N46_BUNNY,
        [XmlEnum(Name = "47")]
        N47_CORRUPT_BUNNY,
        [XmlEnum(Name = "48")]
        N48_HARPY,
        [XmlEnum(Name = "49")]
        N49_CAVE_BAT,
        [XmlEnum(Name = "50")]
        N50_KING_SLIME,
        [XmlEnum(Name = "51")]
        N51_JUNGLE_BAT,
        [XmlEnum(Name = "52")]
        N52_DOCTOR_BONES,
        [XmlEnum(Name = "53")]
        N53_THE_GROOM,
        [XmlEnum(Name = "54")]
        N54_CLOTHIER,
        [XmlEnum(Name = "55")]
        N55_GOLDFISH,
        [XmlEnum(Name = "56")]
        N56_SNATCHER,
        [XmlEnum(Name = "57")]
        N57_CORRUPT_GOLDFISH,
        [XmlEnum(Name = "58")]
        N58_PIRANHA,
        [XmlEnum(Name = "59")]
        N59_LAVA_SLIME,
        [XmlEnum(Name = "60")]
        N60_HELLBAT,
        [XmlEnum(Name = "61")]
        N61_VULTURE,
        [XmlEnum(Name = "62")]
        N62_DEMON,
        [XmlEnum(Name = "63")]
        N63_BLUE_JELLYFISH,
        [XmlEnum(Name = "64")]
        N64_PINK_JELLYFISH,
        [XmlEnum(Name = "65")]
        N65_SHARK,
        [XmlEnum(Name = "66")]
        N66_VOODOO_DEMON,
        [XmlEnum(Name = "67")]
        N67_CRAB,
        [XmlEnum(Name = "68")]
        N68_DUNGEON_GUARDIAN,
        [XmlEnum(Name = "69")]
        N69_ANTLION,
        [XmlEnum(Name = "70")]
        N70_SPIKE_BALL,
        [XmlEnum(Name = "71")]
        N71_DUNGEON_SLIME,
        [XmlEnum(Name = "72")]
        N72_BLAZING_WHEEL,
        [XmlEnum(Name = "73")]
        N73_GOBLIN_SCOUT,
        [XmlEnum(Name = "74")]
        N74_BIRD
    }
}
