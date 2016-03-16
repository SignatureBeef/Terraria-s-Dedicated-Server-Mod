using System;
using OTA.Data;
using TDSM.Core.ServerCharacters;
using OTA;
using TDSM.Core.Data;
using System.Linq;
using TDSM.Core.Data.Models;
using OTA.Misc;
using System.Data;
using Dapper.Contrib.Extensions;
using OTA.Data.Dapper.Extensions;
using TDSM.Core.ServerCharacters.Models;

namespace TDSM.Core.ServerCharacters.Tables
{
#if ENTITY_FRAMEWORK_7
    internal class CharacterTable
    {
        public static Character NewCharacter
        (
            TContext ctx,
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
                ctx,
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
                DataEncoding.EncodeInteger(hideVisual),
                difficulty,
                DataEncoding.EncodeColor(hairColor),
                DataEncoding.EncodeColor(skinColor),
                DataEncoding.EncodeColor(eyeColor),
                DataEncoding.EncodeColor(shirtColor),
                DataEncoding.EncodeColor(underShirtColor),
                DataEncoding.EncodeColor(pantsColor),
                DataEncoding.EncodeColor(shoeColor), 
                anglerQuests
            );
        }

        public static Character NewCharacter
        (
            TContext ctx,
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

        public static Character UpdateCharacter
        (
            TContext ctx,
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
                ctx,
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
                DataEncoding.EncodeInteger(hideVisual),
                difficulty,
                DataEncoding.EncodeColor(hairColor),
                DataEncoding.EncodeColor(skinColor),
                DataEncoding.EncodeColor(eyeColor),
                DataEncoding.EncodeColor(shirtColor),
                DataEncoding.EncodeColor(underShirtColor),
                DataEncoding.EncodeColor(pantsColor),
                DataEncoding.EncodeColor(shoeColor), 
                anglerQuests
            );
        }

        public static Character GetCharacter(TContext ctx, CharacterMode mode, string auth, string clientUUID)
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
            
            if (mode == CharacterMode.AUTH)
            {
                return ctx.Characters.SingleOrDefault(x => x.UserId == userId);
            }
            else
            {
                return ctx.Characters.SingleOrDefault(x => x.UUID == clientUUID);
            }
        }

        public static Character UpdateCharacter
        (
            TContext ctx,
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
#elif DAPPER
    internal class CharacterTable
    {
        public static Character NewCharacter
        (
            IDbConnection ctx,
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
                ctx,
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
                DataEncoding.EncodeInteger(hideVisual),
                difficulty,
                DataEncoding.EncodeColorAsInt32(hairColor),
                DataEncoding.EncodeColorAsInt32(skinColor),
                DataEncoding.EncodeColorAsInt32(eyeColor),
                DataEncoding.EncodeColorAsInt32(shirtColor),
                DataEncoding.EncodeColorAsInt32(underShirtColor),
                DataEncoding.EncodeColorAsInt32(pantsColor),
                DataEncoding.EncodeColorAsInt32(shoeColor),
                anglerQuests
            );
        }

        public static Character NewCharacter
        (
            IDbConnection ctx,
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
            int hairColor,
            int skinColor,
            int eyeColor,
            int shirtColor,
            int underShirtColor,
            int pantsColor,
            int shoeColor,
            int anglerQuests
        )
        {
            long? playerId = null;
            if (mode == CharacterMode.AUTH)
            {
                var user = Authentication.GetPlayer(auth);
                playerId = user.Id;
            }
            else if (mode != CharacterMode.UUID)
                return null;

            Character chr = new Character()
            {
                PlayerId = playerId,
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

            chr.Id = ctx.Insert(chr);

            return chr;
        }

        public static Character UpdateCharacter
        (
            IDbConnection ctx,
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
                ctx,
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
                DataEncoding.EncodeInteger(hideVisual),
                difficulty,
                DataEncoding.EncodeColorAsInt32(hairColor),
                DataEncoding.EncodeColorAsInt32(skinColor),
                DataEncoding.EncodeColorAsInt32(eyeColor),
                DataEncoding.EncodeColorAsInt32(shirtColor),
                DataEncoding.EncodeColorAsInt32(underShirtColor),
                DataEncoding.EncodeColorAsInt32(pantsColor),
                DataEncoding.EncodeColorAsInt32(shoeColor),
                anglerQuests
            );
        }

        public static Character GetCharacter(IDbConnection ctx, CharacterMode mode, string auth, string clientUUID)
        {
            long playerId = 0;
            if (mode == CharacterMode.AUTH)
            {
                var player = Authentication.GetPlayer(auth);

                if (player == null)
                {
                    OTA.Logging.ProgramLog.Error.Log("No user found ");
                    return null;
                }

                playerId = player.Id;
            }
            else if (mode != CharacterMode.UUID)
                return null;

            if (mode == CharacterMode.AUTH)
            {
                return ctx.SingleOrDefault<Character>(new { PlayerId = playerId });
            }
            else
            {
                return ctx.SingleOrDefault<Character>(new { UUID = clientUUID });
            }
        }

        public static Character UpdateCharacter
        (
            IDbConnection ctx,
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
            int hairColor,
            int skinColor,
            int eyeColor,
            int shirtColor,
            int underShirtColor,
            int pantsColor,
            int shoeColor,
            int anglerQuests
        )
        {
            long? playerId = null;
            if (mode == CharacterMode.AUTH)
            {
                var player = Authentication.GetPlayer(auth);
                playerId = player.Id;
            }
            else if (mode != CharacterMode.UUID)
                return null;

            Character chr;
            if (mode == CharacterMode.AUTH)
            {
                chr = ctx.SingleOrDefault<Character>(new { PlayerId = playerId.Value });
            }
            else
            {
                chr = ctx.SingleOrDefault<Character>(new { UUID = clientUUID });
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

            ctx.Update(chr);

            return chr;
        }
    }
#endif
}