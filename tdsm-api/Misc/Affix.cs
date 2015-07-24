﻿using System;

namespace TDSM.API.Misc
{
    public enum Affix : byte
    {
        None = 0,
        Large = 1,
        Massive = 2,
        Dangerous = 3,
        Savage = 4,
        Sharp = 5,
        Pointy = 6,
        Tiny = 7,
        Terrible = 8,
        Small = 9,
        Dull = 10,
        Unhappy = 11,
        Bulky = 12,
        Shameful = 13,
        Heavy = 14,
        Light = 15,
        Sighted = 16,
        Rapid = 17,
        Hasty = 18,
        Intimidating = 19,
        Deadly = 20,
        Staunch = 21,
        Awful = 22,
        Lethargic = 23,
        Awkward = 24,
        Powerful = 25,
        Mystic = 26,
        Adept = 27,
        Masterful = 28,
        Inept = 29,
        Ignorant = 30,
        Deranged = 31,
        Intense = 32,
        Taboo = 33,
        Celestial = 34,
        Furious = 35,
        Keen = 36,
        Superior = 37,
        Forceful = 38,
        Broken = 39,
        Damaged = 40,
        Shoddy = 41,
        Quick = 42,
        Deadly_2 = 43,
        Agile = 44,
        Nimble = 45,
        Murderous = 46,
        Slow = 47,
        Sluggish = 48,
        Lazy = 49,
        Annoying = 50,
        Nasty = 51,
        Manic = 52,
        Hurtful = 53,
        Strong = 54,
        Unpleasant = 55,
        Weak = 56,
        Ruthless = 57,
        Frenzying = 58,
        Godly = 59,
        Demonic = 60,
        Zealous = 61,
        Hard = 62,
        Guarding = 63,
        Armored = 64,
        Warding = 65,
        Arcane = 66,
        Precise = 67,
        Lucky = 68,
        Jagged = 69,
        Spiked = 70,
        Angry = 71,
        Menacing = 72,
        Brisk = 73,
        Fleeting = 74,
        Hasty_2 = 75,
        Quick_2 = 76,
        Wild = 77,
        Rash = 78,
        Intrepid = 79,
        Violent = 80,
        Legendary = 81,
        Unreal = 82,
        Mythical = 83
    }

    public static class AffixExtensions
    {
        public static bool Parse(string str, out Affix affix, bool ignoreCase = false)
        {
            affix = Affix.None;

            foreach (var val in Enum.GetValues(typeof(Affix)))
            {
                var afx = (Affix)val;
                if (afx.ToString() == str || (ignoreCase && afx.ToString().ToLower() == str.ToLower()))
                {
                    affix = afx;
                    return true;
                }
            }

            return false;
        }
    }
}
