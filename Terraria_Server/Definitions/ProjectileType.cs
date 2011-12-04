using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Terraria_Server.Definitions
{
    /// <summary>
    /// All available projectile types
    /// </summary>
    public enum ProjectileType : int
    {
        [XmlEnum(Name = "1")]
        N0_UNKNOWN,
        [XmlEnum(Name = "1")]
        N1_WOODEN_ARROW,
        [XmlEnum(Name = "2")]
        N2_FIRE_ARROW,
        [XmlEnum(Name = "3")]
        N3_SHURIKEN,
        [XmlEnum(Name = "4")]
        N4_UNHOLY_ARROW,
        [XmlEnum(Name = "5")]
        N5_JESTERS_ARROW,
        [XmlEnum(Name = "6")]
        N6_ENCHANTED_BOOMERANG,
        [XmlEnum(Name = "7")]
        N7_VILETHORN,
        [XmlEnum(Name = "8")]
        N8_VILETHORN,
        [XmlEnum(Name = "9")]
        N9_STARFURY,
        [XmlEnum(Name = "10")]
        N10_PURIFICATION_POWDER,
        [XmlEnum(Name = "11")]
        N11_VILE_POWDER,
        [XmlEnum(Name = "12")]
        N12_FALLING_STAR,
        [XmlEnum(Name = "13")]
        N13_HOOK,
        [XmlEnum(Name = "14")]
        N14_BULLET,
        [XmlEnum(Name = "15")]
        N15_BALL_OF_FIRE,
        [XmlEnum(Name = "16")]
        N16_MAGIC_MISSILE,
        [XmlEnum(Name = "17")]
        N17_DIRT_BALL,
        [XmlEnum(Name = "18")]
        N18_ORB_OF_LIGHT,
        [XmlEnum(Name = "19")]
        N19_FLAMARANG,
        [XmlEnum(Name = "20")]
        N20_GREEN_LASER,
        [XmlEnum(Name = "21")]
        N21_BONE,
        [XmlEnum(Name = "22")]
        N22_WATER_STREAM,
        [XmlEnum(Name = "23")]
        N23_HARPOON,
        [XmlEnum(Name = "24")]
        N24_SPIKY_BALL,
        [XmlEnum(Name = "25")]
        N25_BALL_O_HURT,
        [XmlEnum(Name = "26")]
        N26_BLUE_MOON,
        [XmlEnum(Name = "27")]
        N27_WATER_BOLT,
        [XmlEnum(Name = "28")]
        N28_BOMB,
        [XmlEnum(Name = "29")]
        N29_DYNAMITE,
        [XmlEnum(Name = "30")]
        N30_GRENADE,
        [XmlEnum(Name = "31")]
        N31_SAND_BALL,
        [XmlEnum(Name = "32")]
        N32_IVY_WHIP,
        [XmlEnum(Name = "33")]
        N33_THORN_CHAKRUM,
        [XmlEnum(Name = "34")]
        N34_FLAMELASH,
        [XmlEnum(Name = "35")]
        N35_SUNFURY,
        [XmlEnum(Name = "36")]
        N36_METEOR_SHOT,
        [XmlEnum(Name = "37")]
        N37_STICKY_BOMB,
        [XmlEnum(Name = "38")]
        N38_HARPY_FEATHER,
        [XmlEnum(Name = "39")]
        N39_MUD_BALL,
        [XmlEnum(Name = "40")]
        N40_ASH_BALL,
        [XmlEnum(Name = "41")]
        N41_HELLFIRE_ARROW,
        [XmlEnum(Name = "42")]
        N42_SAND_BALL,
        [XmlEnum(Name = "43")]
        N43_TOMBSTONE,
        [XmlEnum(Name = "44")]
        N44_DEMON_SICKLE,
        [XmlEnum(Name = "45")]
        N45_DEMON_SCYTHE,
        [XmlEnum(Name = "46")]
        N46_DARK_LANCE,
        [XmlEnum(Name = "47")]
        N47_TRIDENT,
        [XmlEnum(Name = "48")]
        N48_THROWING_KNIFE,
        [XmlEnum(Name = "49")]
        N49_SPEAR,
        [XmlEnum(Name = "50")]
        N50_GLOWSTICK,
        [XmlEnum(Name = "51")]
        N51_SEED,
        [XmlEnum(Name = "52")]
        N52_WOODEN_BOOMERANG,
        [XmlEnum(Name = "53")]
        N53_STICKY_GLOWSTICK,
        [XmlEnum(Name = "54")]
        N54_POISONED_KNIFE,
        [XmlEnum(Name = "55")]
        N55_STINGER,
        [XmlEnum(Name = "56")]
        N56_EBONSAND_BALL,
        [XmlEnum(Name = "57")]
        N57_COBALT_CHAINSAW,
        [XmlEnum(Name = "58")]
        N58_MYTHRIL_CHAINSAW,
        [XmlEnum(Name = "59")]
        N59_COBALT_DRILL,
        [XmlEnum(Name = "60")]
        N60_MYTHRIL_DRILL,
        [XmlEnum(Name = "61")]
        N61_ADAMANTITE_CHAINSAW,
        [XmlEnum(Name = "62")]
        N62_ADAMANTITE_DRILL,
        [XmlEnum(Name = "63")]
        N63_THE_DAO_OF_POW,
        [XmlEnum(Name = "64")]
        N64_MYTHRIL_HALBERD,
        [XmlEnum(Name = "65")]
        N65_EBONSAND_BALL,
        [XmlEnum(Name = "66")]
        N66_ADAMANTITE_GLAIVE,
        [XmlEnum(Name = "67")]
        N67_PEARL_SAND_BALL,
        [XmlEnum(Name = "68")]
        N68_PEARL_SAND_BALL,
        [XmlEnum(Name = "69")]
        N69_HOLY_WATER,
        [XmlEnum(Name = "70")]
        N70_UNHOLY_WATER,
        [XmlEnum(Name = "71")]
        N71_GRAVEL_BALL,
        [XmlEnum(Name = "72")]
        N72_BLUE_FAIRY,
        [XmlEnum(Name = "73")]
        N73_HOOK,
        [XmlEnum(Name = "74")]
        N74_HOOK,
        [XmlEnum(Name = "75")]
        N75_HAPPY_BOMB,
        [XmlEnum(Name = "76")]
        N76_NOTE,
        [XmlEnum(Name = "77")]
        N77_NOTE,
        [XmlEnum(Name = "78")]
        N78_NOTE,
        [XmlEnum(Name = "79")]
        N79_RAINBOW,
        [XmlEnum(Name = "80")]
        N80_ICE_BLOCK,
        [XmlEnum(Name = "81")]
        N81_WOODEN_ARROW,
        [XmlEnum(Name = "82")]
        N82_FLAMING_ARROW,
        [XmlEnum(Name = "83")]
        N83_EYE_LASER,
        [XmlEnum(Name = "84")]
        N84_PINK_LASER,
        [XmlEnum(Name = "85")]
        N85_FLAMES,
        [XmlEnum(Name = "86")]
        N86_PINK_FAIRY,
        [XmlEnum(Name = "87")]
        N87_PINK_FAIRY,
        [XmlEnum(Name = "88")]
        N88_PURPLE_LASER,
        [XmlEnum(Name = "89")]
        N89_CRYSTAL_BULLET,
        [XmlEnum(Name = "90")]
        N90_CRYSTAL_SHARD,
        [XmlEnum(Name = "91")]
        N91_HOLY_ARROW,
        [XmlEnum(Name = "92")]
        N92_HALLOW_STAR,
        [XmlEnum(Name = "93")]
        N93_MAGIC_DAGGER,
        [XmlEnum(Name = "94")]
        N94_CRYSTAL_STORM,
        [XmlEnum(Name = "95")]
        N95_CURSED_FLAME,
        [XmlEnum(Name = "96")]
        N96_CURSED_FLAME,
        [XmlEnum(Name = "97")]
        N97_COBALT_NAGINATA,
        [XmlEnum(Name = "98")]
        N98_POISON_DART,
        [XmlEnum(Name = "99")]
        N99_BOULDER,
        [XmlEnum(Name = "100")]
        N100_DEATH_LASER,
        [XmlEnum(Name = "101")]
        N101_EYE_FIRE,
        [XmlEnum(Name = "102")]
        N102_BOMB,
        [XmlEnum(Name = "103")]
        N103_CURSED_ARROW,
        [XmlEnum(Name = "104")]
        N104_CURSED_BULLET,
        [XmlEnum(Name = "105")]
        N105_GUNGNIR,
        [XmlEnum(Name = "106")]
        N106_LIGHT_DISC,
        [XmlEnum(Name = "107")]
        N107_HAMDRAX,
        [XmlEnum(Name = "108")]
        N108_EXPLOSIVES,
    }
    
	public static class ProjectileTypeExtensions
	{
		public static bool IsHighExplosive (this ProjectileType type)
		{
			return  type == ProjectileType.N29_DYNAMITE || 
                    type == ProjectileType.N102_BOMB || 
                    type == ProjectileType.N37_STICKY_BOMB;
		}
	}
}
