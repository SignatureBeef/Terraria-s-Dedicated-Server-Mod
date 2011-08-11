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
        UNKNOWN,
        [XmlEnum(Name = "1")]
        ARROW_WOODEN,
        [XmlEnum(Name = "2")]
        ARROW_FIRE,
        [XmlEnum(Name = "3")]
        SHURIKEN,
        [XmlEnum(Name = "4")]
        ARROW_UNHOLY,
        [XmlEnum(Name = "5")]
        ARROW_JESTER,
        [XmlEnum(Name = "6")]
        BOOMERANG_ENCHANTED,
        [XmlEnum(Name = "7")]
        VILETHORN,
        [XmlEnum(Name = "8")]
        VILETHORN_B,
        [XmlEnum(Name = "9")]
        STARFURY,
        [XmlEnum(Name = "10")]
        POWDER_PURIFICATION,
        [XmlEnum(Name = "11")]
        POWDER_VILE,
        [XmlEnum(Name = "12")]
        FALLEN_STAR,
        [XmlEnum(Name = "13")]
        HOOK,
        [XmlEnum(Name = "14")]
        BALL_MUSKET,
        [XmlEnum(Name = "15")]
        BALL_OF_FIRE,
        [XmlEnum(Name = "16")]
        MISSILE_MAGIC,
        [XmlEnum(Name = "17")]
        BALL_DIRT,
        [XmlEnum(Name = "18")]
        ORB_OF_LIGHT,
        [XmlEnum(Name = "19")]
        FLAMARANG,
        [XmlEnum(Name = "20")]
        LASER_GREEN,
        [XmlEnum(Name = "21")]
        BONE,
        [XmlEnum(Name = "22")]
        STREAM_WATER,
        [XmlEnum(Name = "23")]
        HARPOON,
        [XmlEnum(Name = "24")]
        BALL_SPIKY,
        [XmlEnum(Name = "25")]
        BALL_O_HURT,
        [XmlEnum(Name = "26")]
        BLUE_MOON,
        [XmlEnum(Name = "27")]
        BOLT_WATER,
        [XmlEnum(Name = "28")]
        BOMB,
        [XmlEnum(Name = "29")]
        DYNAMITE,
        [XmlEnum(Name = "30")]
        GRENADE,
        [XmlEnum(Name = "31")]
        BALL_SAND_DROP,
        [XmlEnum(Name = "32")]
        WHIP_IVY,
        [XmlEnum(Name = "33")]
        CHAKRUM_THORN,
        [XmlEnum(Name = "34")]
        FLAMELASH,
        [XmlEnum(Name = "35")]
        SUNFURY,
        [XmlEnum(Name = "36")]
        SHOT_METEOR,
        [XmlEnum(Name = "37")]
        BOMB_STICKY,
        [XmlEnum(Name = "38")]
        FEATHER_HARPY,
        [XmlEnum(Name = "39")]
        BALL_MUD,
        [XmlEnum(Name = "40")]
        BALL_ASH,
        [XmlEnum(Name = "41")]
        ARROW_HELLFIRE,
        [XmlEnum(Name = "42")]
        BALL_SAND_GUN,
        [XmlEnum(Name = "43")]
        TOMBSTONE,
        [XmlEnum(Name = "44")]
        SICKLE_DEMON,
        [XmlEnum(Name = "45")]
        SCYTHE_DEMON,
        [XmlEnum(Name = "46")]
        LANCE_DARK,
        [XmlEnum(Name = "47")]
        TRIDENT,
        [XmlEnum(Name = "48")]
        KNIFE_THROWING,
        [XmlEnum(Name = "49")]
        SPEAR,
        [XmlEnum(Name = "50")]
        GLOWSTICK,
        [XmlEnum(Name = "51")]
        SEED,
        [XmlEnum(Name = "52")]
        BOOMERANG_WOODEN,
        [XmlEnum(Name = "53")]
        GLOWSTICK_STICKY,
        [XmlEnum(Name = "54")]
        KNIFE_POISONED,
        [XmlEnum(Name = "55")]
        STINGER,
    }
}
