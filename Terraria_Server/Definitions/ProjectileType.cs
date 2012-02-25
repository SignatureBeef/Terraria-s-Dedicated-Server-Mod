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
		[XmlEnum(Name = "0")]
		N0_UNKNOWN = 0,
		[XmlEnum(Name = "1")]
		N1_WOODEN_ARROW = 1,
		[XmlEnum(Name = "2")]
		N2_FIRE_ARROW = 2,
		[XmlEnum(Name = "3")]
		N3_SHURIKEN = 3,
		[XmlEnum(Name = "4")]
		N4_UNHOLY_ARROW = 4,
		[XmlEnum(Name = "5")]
		N5_JESTERS_ARROW = 5,
		[XmlEnum(Name = "6")]
		N6_ENCHANTED_BOOMERANG = 6,
		[XmlEnum(Name = "7")]
		N7_VILETHORN = 7,
		[XmlEnum(Name = "8")]
		N8_VILETHORN = 8,
		[XmlEnum(Name = "9")]
		N9_STARFURY = 9,
		[XmlEnum(Name = "10")]
		N10_PURIFICATION_POWDER = 10,
		[XmlEnum(Name = "11")]
		N11_VILE_POWDER = 11,
		[XmlEnum(Name = "12")]
		N12_FALLING_STAR = 12,
		[XmlEnum(Name = "13")]
		N13_HOOK = 13,
		[XmlEnum(Name = "14")]
		N14_BULLET = 14,
		[XmlEnum(Name = "15")]
		N15_BALL_OF_FIRE = 15,
		[XmlEnum(Name = "16")]
		N16_MAGIC_MISSILE = 16,
		[XmlEnum(Name = "17")]
		N17_DIRT_BALL = 17,
		[XmlEnum(Name = "18")]
		N18_ORB_OF_LIGHT = 18,
		[XmlEnum(Name = "19")]
		N19_FLAMARANG = 19,
		[XmlEnum(Name = "20")]
		N20_GREEN_LASER = 20,
		[XmlEnum(Name = "21")]
		N21_BONE = 21,
		[XmlEnum(Name = "22")]
		N22_WATER_STREAM = 22,
		[XmlEnum(Name = "23")]
		N23_HARPOON = 23,
		[XmlEnum(Name = "24")]
		N24_SPIKY_BALL = 24,
		[XmlEnum(Name = "25")]
		N25_BALL_O_HURT = 25,
		[XmlEnum(Name = "26")]
		N26_BLUE_MOON = 26,
		[XmlEnum(Name = "27")]
		N27_WATER_BOLT = 27,
		[XmlEnum(Name = "28")]
		N28_BOMB = 28,
		[XmlEnum(Name = "29")]
		N29_DYNAMITE = 29,
		[XmlEnum(Name = "30")]
		N30_GRENADE = 30,
		[XmlEnum(Name = "31")]
		N31_SAND_BALL = 31,
		[XmlEnum(Name = "32")]
		N32_IVY_WHIP = 32,
		[XmlEnum(Name = "33")]
		N33_THORN_CHAKRUM = 33,
		[XmlEnum(Name = "34")]
		N34_FLAMELASH = 34,
		[XmlEnum(Name = "35")]
		N35_SUNFURY = 35,
		[XmlEnum(Name = "36")]
		N36_METEOR_SHOT = 36,
		[XmlEnum(Name = "37")]
		N37_STICKY_BOMB = 37,
		[XmlEnum(Name = "38")]
		N38_HARPY_FEATHER = 38,
		[XmlEnum(Name = "39")]
		N39_MUD_BALL = 39,
		[XmlEnum(Name = "40")]
		N40_ASH_BALL = 40,
		[XmlEnum(Name = "41")]
		N41_HELLFIRE_ARROW = 41,
		[XmlEnum(Name = "42")]
		N42_SAND_BALL = 42,
		[XmlEnum(Name = "43")]
		N43_TOMBSTONE = 43,
		[XmlEnum(Name = "44")]
		N44_DEMON_SICKLE = 44,
		[XmlEnum(Name = "45")]
		N45_DEMON_SCYTHE = 45,
		[XmlEnum(Name = "46")]
		N46_DARK_LANCE = 46,
		[XmlEnum(Name = "47")]
		N47_TRIDENT = 47,
		[XmlEnum(Name = "48")]
		N48_THROWING_KNIFE = 48,
		[XmlEnum(Name = "49")]
		N49_SPEAR = 49,
		[XmlEnum(Name = "50")]
		N50_GLOWSTICK = 50,
		[XmlEnum(Name = "51")]
		N51_SEED = 51,
		[XmlEnum(Name = "52")]
		N52_WOODEN_BOOMERANG = 52,
		[XmlEnum(Name = "53")]
		N53_STICKY_GLOWSTICK = 53,
		[XmlEnum(Name = "54")]
		N54_POISONED_KNIFE = 54,
		[XmlEnum(Name = "55")]
		N55_STINGER = 55,
		[XmlEnum(Name = "56")]
		N56_EBONSAND_BALL = 56,
		[XmlEnum(Name = "57")]
		N57_COBALT_CHAINSAW = 57,
		[XmlEnum(Name = "58")]
		N58_MYTHRIL_CHAINSAW = 58,
		[XmlEnum(Name = "59")]
		N59_COBALT_DRILL = 59,
		[XmlEnum(Name = "60")]
		N60_MYTHRIL_DRILL = 60,
		[XmlEnum(Name = "61")]
		N61_ADAMANTITE_CHAINSAW = 61,
		[XmlEnum(Name = "62")]
		N62_ADAMANTITE_DRILL = 62,
		[XmlEnum(Name = "63")]
		N63_THE_DAO_OF_POW = 63,
		[XmlEnum(Name = "64")]
		N64_MYTHRIL_HALBERD = 64,
		[XmlEnum(Name = "65")]
		N65_EBONSAND_BALL = 65,
		[XmlEnum(Name = "66")]
		N66_ADAMANTITE_GLAIVE = 66,
		[XmlEnum(Name = "67")]
		N67_PEARL_SAND_BALL = 67,
		[XmlEnum(Name = "68")]
		N68_PEARL_SAND_BALL = 68,
		[XmlEnum(Name = "69")]
		N69_HOLY_WATER = 69,
		[XmlEnum(Name = "70")]
		N70_UNHOLY_WATER = 70,
		[XmlEnum(Name = "71")]
		N71_GRAVEL_BALL = 71,
		[XmlEnum(Name = "72")]
		N72_BLUE_FAIRY = 72,
		[XmlEnum(Name = "73")]
		N73_HOOK = 73,
		[XmlEnum(Name = "74")]
		N74_HOOK = 74,
		[XmlEnum(Name = "75")]
		N75_HAPPY_BOMB = 75,
		[XmlEnum(Name = "76")]
		N76_NOTE = 76,
		[XmlEnum(Name = "77")]
		N77_NOTE = 77,
		[XmlEnum(Name = "78")]
		N78_NOTE = 78,
		[XmlEnum(Name = "79")]
		N79_RAINBOW = 79,
		[XmlEnum(Name = "80")]
		N80_ICE_BLOCK = 80,
		[XmlEnum(Name = "81")]
		N81_WOODEN_ARROW = 81,
		[XmlEnum(Name = "82")]
		N82_FLAMING_ARROW = 82,
		[XmlEnum(Name = "83")]
		N83_EYE_LASER = 83,
		[XmlEnum(Name = "84")]
		N84_PINK_LASER = 84,
		[XmlEnum(Name = "85")]
		N85_FLAMES = 85,
		[XmlEnum(Name = "86")]
		N86_PINK_FAIRY = 86,
		[XmlEnum(Name = "87")]
		N87_PINK_FAIRY = 87,
		[XmlEnum(Name = "88")]
		N88_PURPLE_LASER = 88,
		[XmlEnum(Name = "89")]
		N89_CRYSTAL_BULLET = 89,
		[XmlEnum(Name = "90")]
		N90_CRYSTAL_SHARD = 90,
		[XmlEnum(Name = "91")]
		N91_HOLY_ARROW = 91,
		[XmlEnum(Name = "92")]
		N92_HALLOW_STAR = 92,
		[XmlEnum(Name = "93")]
		N93_MAGIC_DAGGER = 93,
		[XmlEnum(Name = "94")]
		N94_CRYSTAL_STORM = 94,
		[XmlEnum(Name = "95")]
		N95_CURSED_FLAME = 95,
		[XmlEnum(Name = "96")]
		N96_CURSED_FLAME = 96,
		[XmlEnum(Name = "97")]
		N97_COBALT_NAGINATA = 97,
		[XmlEnum(Name = "98")]
		N98_POISON_DART = 98,
		[XmlEnum(Name = "99")]
		N99_BOULDER = 99,
		[XmlEnum(Name = "100")]
		N100_DEATH_LASER = 100,
		[XmlEnum(Name = "101")]
		N101_EYE_FIRE = 101,
		[XmlEnum(Name = "102")]
		N102_BOMB = 102,
		[XmlEnum(Name = "103")]
		N103_CURSED_ARROW = 103,
		[XmlEnum(Name = "104")]
		N104_CURSED_BULLET = 104,
		[XmlEnum(Name = "105")]
		N105_GUNGNIR = 105,
		[XmlEnum(Name = "106")]
		N106_LIGHT_DISC = 106,
		[XmlEnum(Name = "107")]
		N107_HAMDRAX = 107,
		[XmlEnum(Name = "108")]
		N108_EXPLOSIVES = 108,
		[XmlEnum(Name = "109")]
		N109_SNOW_BALL = 109,
		[XmlEnum(Name = "110")]
		N110_BULLET = 110,
		[XmlEnum(Name = "111")]
		N111_BUNNY = 111,
	}

	public static class ProjectileTypeExtensions
	{
		public static bool IsHighExplosive(this ProjectileType type)
		{
			return type == ProjectileType.N29_DYNAMITE ||
					type == ProjectileType.N102_BOMB ||
					type == ProjectileType.N37_STICKY_BOMB ||
					type == ProjectileType.N108_EXPLOSIVES;
		}

		public static bool IsExplosive(this ProjectileType type)
		{
			return IsHighExplosive(type) ||
					type == ProjectileType.N30_GRENADE ||
					type == ProjectileType.N41_HELLFIRE_ARROW;
		}
	}
}
