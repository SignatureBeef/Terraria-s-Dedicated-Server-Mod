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
        N74_BIRD,
		[XmlEnum(Name = "75")]
		N75_PIXIE,
		[XmlEnum(Name = "77")]
		N77_ARMORED_SKELETON,
		[XmlEnum(Name = "78")]
		N78_MUMMY,
		[XmlEnum(Name = "79")]
		N79_DARK_MUMMY,
		[XmlEnum(Name = "80")]
		N80_LIGHT_MUMMY,
		[XmlEnum(Name = "81")]
		N81_CORRUPT_SLIME,
		[XmlEnum(Name = "82")]
		N82_WRAITH,
		[XmlEnum(Name = "83")]
		N83_CURSED_HAMMER,
		[XmlEnum(Name = "84")]
		N84_ENCHANTED_SWORD,
		[XmlEnum(Name = "85")]
		N85_MIMIC,
		[XmlEnum(Name = "86")]
		N86_UNICORN,
		[XmlEnum(Name = "87")]
		N87_WYVERN_HEAD,
		[XmlEnum(Name = "88")]
		N88_WYVERN_LEGS,
		[XmlEnum(Name = "89")]
		N89_WYVERN_BODY,
		[XmlEnum(Name = "90")]
		N90_WYVERN_BODY_2,
		[XmlEnum(Name = "91")]
		N91_WYVERN_BODY_3,
		[XmlEnum(Name = "92")]
		N92_WYVERN_TAIL,
		[XmlEnum(Name = "93")]
		N93_GIANT_BAT,
		[XmlEnum(Name = "94")]
		N94_CORRUPTOR,
		[XmlEnum(Name = "95")]
		N95_DIGGER_HEAD,
		[XmlEnum(Name = "96")]
		N96_DIGGER_BODY,
		[XmlEnum(Name = "97")]
		N97_DIGGER_TAIL,
		[XmlEnum(Name = "98")]
		N98_SEEKER_HEAD,
		[XmlEnum(Name = "99")]
		N99_SEEKER_BODY,
		[XmlEnum(Name = "100")]
		N100_SEEKER_TAIL,
		[XmlEnum(Name = "101")]
		N101_CLINGER,
		[XmlEnum(Name = "102")]
		N102_ANGLER_FISH,
		[XmlEnum(Name = "103")]
		N103_GREEN_JELLYFISH,
		[XmlEnum(Name = "104")]
		N104_WEREWOLF,
		[XmlEnum(Name = "105")]
		N105_BOUND_GOBLIN,
		[XmlEnum(Name = "106")]
		N106_BOUND_WIZARD,
		[XmlEnum(Name = "107")]
		N107_GOBLIN_TINKERER,
		[XmlEnum(Name = "108")]
		N108_WIZARD,
		[XmlEnum(Name = "109")]
		N109_CLOWN,
		[XmlEnum(Name = "110")]
		N110_SKELETON_ARCHER,
		[XmlEnum(Name = "111")]
		N111_GOBLIN_ARCHER,
		[XmlEnum(Name = "112")]
		N112_VILE_SPIT,
		[XmlEnum(Name = "113")]
		N113_WALL_OF_FLESH,
		[XmlEnum(Name = "114")]
		N114_WALL_OF_FLESH_EYE,
		[XmlEnum(Name = "115")]
		N115_THE_HUNGRY,
		[XmlEnum(Name = "116")]
		N116_THE_HUNGRY_II,
		[XmlEnum(Name = "117")]
		N117_LEECH_HEAD,
		[XmlEnum(Name = "118")]
		N118_LEECH_BODY,
		[XmlEnum(Name = "119")]
		N119_LEECH_TAIL,
		[XmlEnum(Name = "120")]
		N120_CHAOS_ELEMENTAL,
		[XmlEnum(Name = "121")]
		N121_SLIMER,
		[XmlEnum(Name = "122")]
		N122_GASTROPOD,
		[XmlEnum(Name = "123")]
		N123_BOUND_MECHANIC,
		[XmlEnum(Name = "124")]
		N124_MECHANIC,
		[XmlEnum(Name = "125")]
		N125_RETINAZER,
		[XmlEnum(Name = "126")]
		N126_SPAZMATISM,
		[XmlEnum(Name = "127")]
		N127_SKELETRON_PRIME,
		[XmlEnum(Name = "128")]
		N128_PRIME_CANNON,
		[XmlEnum(Name = "129")]
		N129_PRIME_SAW,
		[XmlEnum(Name = "130")]
		N130_PRIME_VICE,
		[XmlEnum(Name = "131")]
		N131_PRIME_LASER,
		[XmlEnum(Name = "132")]
		N132_BALD_ZOMBIE,
		[XmlEnum(Name = "133")]
		N133_WANDERING_EYE,
		[XmlEnum(Name = "134")]
		N134_THE_DESTROYER,
		[XmlEnum(Name = "135")]
		N135_THE_DESTROYER_BODY,
		[XmlEnum(Name = "136")]
		N136_THE_DESTROYER_TAIL,
		[XmlEnum(Name = "137")]
		N137_ILLUMINANT_BAT,
		[XmlEnum(Name = "138")]
		N138_ILLUMINANT_SLIME,
		[XmlEnum(Name = "139")]
		N139_PROBE,
		[XmlEnum(Name = "140")]
		N140_POSSESSED_ARMOR,
		[XmlEnum(Name = "141")]
		N141_TOXIC_SLUDGE,
		[XmlEnum(Name = "142")]
		N142_SANTA_CLAUS,
		[XmlEnum(Name = "143")]
		N143_SNOWMAN_GANGSTA,
		[XmlEnum(Name = "144")]
		N144_MISTER_STABBY,
		[XmlEnum(Name = "145")]
		N145_SNOW_BALLA,
    }
}
