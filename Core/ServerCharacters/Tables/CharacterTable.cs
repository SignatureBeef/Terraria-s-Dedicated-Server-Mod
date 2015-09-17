using System;
using OTA.Data;
using TDSM.Core.ServerCharacters;
using OTA;
using TDSM.Core.Data;
using System.Linq;
using TDSM.Core.Data.Models;

namespace TDSM.Core.ServerCharacters.Tables
{
    internal class CharacterTable
    {
        public static Character NewCharacter
        (
            CharacterMode mode,
            string auth,
            string clientUUID,
            int health,
            int maxHealth,
            int mana,
            int maxMana,
            int spawnX,
            int spawnY,
            int hair,
            byte hairDye,
            bool[] hideVisual,
            byte difficulty,
            Microsoft.Xna.Framework.Color hairColor,
            Microsoft.Xna.Framework.Color skinColor,
            Microsoft.Xna.Framework.Color eyeColor,
            Microsoft.Xna.Framework.Color shirtColor,
            Microsoft.Xna.Framework.Color underShirtColor,
            Microsoft.Xna.Framework.Color pantsColor,
            Microsoft.Xna.Framework.Color shoeColor, 
            int anglerQuests
        )
        {
            return NewCharacter
            (
                mode,
                auth,
                clientUUID,
                health,
                maxHealth,
                mana,
                maxMana,
                spawnX,
                spawnY,
                hair,
                hairDye,
                Tools.Encoding.EncodeInteger(hideVisual),
                difficulty,
                Tools.Encoding.EncodeColor(hairColor),
                Tools.Encoding.EncodeColor(skinColor),
                Tools.Encoding.EncodeColor(eyeColor),
                Tools.Encoding.EncodeColor(shirtColor),
                Tools.Encoding.EncodeColor(underShirtColor),
                Tools.Encoding.EncodeColor(pantsColor),
                Tools.Encoding.EncodeColor(shoeColor), 
                anglerQuests
            );
        }

        public static Character NewCharacter
        (
            CharacterMode mode,
            string auth,
            string clientUUID,
            int health,
            int maxHealth,
            int mana,
            int maxMana,
            int spawnX,
            int spawnY,
            int hair,
            byte hairDye,
            int hideVisual,
            byte difficulty,
            uint hairColor,
            uint skinColor,
            uint eyeColor,
            uint shirtColor,
            uint underShirtColor,
            uint pantsColor,
            uint shoeColor, 
            int anglerQuests
        )
        {
            int? userId = null;
            if (mode == CharacterMode.AUTH)
            {
                var user = AuthenticatedUsers.GetUser(auth);
                userId = user.Id;
            }
            else if (mode != CharacterMode.UUID)
                return null;

            using (var ctx = new TContext())
            {
                Character chr = new Character()
                {
                    UserId = userId,
                    UUID = clientUUID,
                    Health = health,
                    MaxHealth = maxHealth,
                    Mana = mana,
                    MaxMana = maxMana,
                    SpawnX = spawnX,
                    SpawnY = spawnY,
                    Hair = hair,
                    HairDye = hairDye,
                    HideVisual = hideVisual,
                    Difficulty = difficulty,
                    HairColor = hairColor,
                    SkinColor = skinColor,
                    EyeColor = eyeColor,
                    ShirtColor = shirtColor,
                    UnderShirtColor = underShirtColor,
                    PantsColor = pantsColor,
                    ShoeColor = shoeColor,
                    AnglerQuests = anglerQuests
                };
                ctx.Characters.Add(chr);

                ctx.SaveChanges();

                return chr;
            }
        }

        public static Character UpdateCharacter
        (
            CharacterMode mode,
            string auth,
            string clientUUID,
            int health,
            int maxHealth,
            int mana,
            int maxMana,
            int spawnX,
            int spawnY,
            int hair,
            byte hairDye,
            bool[] hideVisual,
            byte difficulty,
            Microsoft.Xna.Framework.Color hairColor,
            Microsoft.Xna.Framework.Color skinColor,
            Microsoft.Xna.Framework.Color eyeColor,
            Microsoft.Xna.Framework.Color shirtColor,
            Microsoft.Xna.Framework.Color underShirtColor,
            Microsoft.Xna.Framework.Color pantsColor,
            Microsoft.Xna.Framework.Color shoeColor, 
            int anglerQuests
        )
        {
            return UpdateCharacter
            (
                mode,
                auth,
                clientUUID,
                health,
                maxHealth,
                mana,
                maxMana,
                spawnX,
                spawnY,
                hair,
                hairDye,
                Tools.Encoding.EncodeInteger(hideVisual),
                difficulty,
                Tools.Encoding.EncodeColor(hairColor),
                Tools.Encoding.EncodeColor(skinColor),
                Tools.Encoding.EncodeColor(eyeColor),
                Tools.Encoding.EncodeColor(shirtColor),
                Tools.Encoding.EncodeColor(underShirtColor),
                Tools.Encoding.EncodeColor(pantsColor),
                Tools.Encoding.EncodeColor(shoeColor), 
                anglerQuests
            );
        }

        public static Character GetCharacter(CharacterMode mode, string auth, string clientUUID)
        {
            int userId = 0;
            if (mode == CharacterMode.AUTH)
            {
                var user = AuthenticatedUsers.GetUser(auth);

                if (user == null)
                {
                    OTA.Logging.ProgramLog.Error.Log("No user found ");
                    return null;
                }

                userId = user.Id;
            }
            else if (mode != CharacterMode.UUID)
                return null;
            
            using (var ctx = new TContext())
            {
                if (mode == CharacterMode.AUTH)
                {
                    return ctx.Characters.SingleOrDefault(x => x.UserId == userId);
                }
                else
                {
                    return ctx.Characters.SingleOrDefault(x => x.UUID == clientUUID);
                }
            }
        }

        public static Character UpdateCharacter
        (
            CharacterMode mode,
            string auth,
            string clientUUID,
            int health,
            int maxHealth,
            int mana,
            int maxMana,
            int spawnX,
            int spawnY,
            int hair,
            byte hairDye,
            int hideVisual,
            byte difficulty,
            uint hairColor,
            uint skinColor,
            uint eyeColor,
            uint shirtColor,
            uint underShirtColor,
            uint pantsColor,
            uint shoeColor, 
            int anglerQuests
        )
        {
            int? userId = null;
            if (mode == CharacterMode.AUTH)
            {
                var user = AuthenticatedUsers.GetUser(auth);
                userId = user.Id;
            }
            else if (mode != CharacterMode.UUID)
                return null;
            
            using (var ctx = new TContext())
            {
                Character chr;
                if (mode == CharacterMode.AUTH)
                {
                    chr = ctx.Characters.Single(x => x.UserId == userId.Value);
                }
                else
                {
                    chr = ctx.Characters.Single(x => x.UUID == clientUUID);
                }

                chr.Health = health;
                chr.MaxHealth = maxHealth;
                chr.Mana = mana;
                chr.MaxMana = maxMana;
                chr.SpawnX = spawnX;
                chr.SpawnY = spawnY;
                chr.Hair = hair;
                chr.HairDye = hairDye;
                chr.HideVisual = hideVisual;
                chr.Difficulty = difficulty;
                chr.HairColor = hairColor;
                chr.SkinColor = skinColor;
                chr.EyeColor = eyeColor;
                chr.ShirtColor = shirtColor;
                chr.UnderShirtColor = underShirtColor;
                chr.PantsColor = pantsColor;
                chr.ShoeColor = shoeColor;
                chr.AnglerQuests = anglerQuests;

                ctx.SaveChanges();

                return chr;
            }
        }
    }
}