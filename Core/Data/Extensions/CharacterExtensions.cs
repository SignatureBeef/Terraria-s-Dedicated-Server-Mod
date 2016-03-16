using System;
using TDSM.Core.ServerCharacters;
using System.Linq;
using TDSM.Core.Data.Models;
using OTA;
using OTA.Misc;
using OTA.Data;
using System.Data;
using OTA.Data.Dapper.Extensions;
using Dapper.Contrib.Extensions;
using TDSM.Core.ServerCharacters.Models;

namespace TDSM.Core.Data.Extensions
{
    public static class CharacterExtensions
    {
#if ENTITY_FRAMEWORK_7
        public static Character GetCharacter(this TContext ctx, CharacterMode mode, int userId, string clientUUID)
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

        public static Character UpdateCharacter(this Character character, int userId, Terraria.Player player)
        {
            character.UserId = userId;
            character.UUID = player.ClientUUId;
            character.Health = player.statLife;
            character.MaxHealth = player.statLifeMax;
            character.Mana = player.statMana;
            character.MaxMana = player.statManaMax;
            character.SpawnX = player.SpawnX;
            character.SpawnY = player.SpawnY;
            character.Hair = player.hair;
            character.HairDye = player.hairDye;
            character.HideVisual = DataEncoding.EncodeInteger(player.hideVisual);
            character.Difficulty = player.difficulty;
            character.HairColor = DataEncoding.EncodeColor(player.hairColor);
            character.SkinColor = DataEncoding.EncodeColor(player.skinColor);
            character.EyeColor = DataEncoding.EncodeColor(player.eyeColor);
            character.ShirtColor = DataEncoding.EncodeColor(player.shirtColor);
            character.UnderShirtColor = DataEncoding.EncodeColor(player.underShirtColor);
            character.PantsColor = DataEncoding.EncodeColor(player.pantsColor);
            character.ShoeColor = DataEncoding.EncodeColor(player.shoeColor);
            character.AnglerQuests = player.anglerQuestsFinished;

            return character;
        }

        public static Character AddCharacter(this TContext ctx, int userId, Terraria.Player player)
        {
            Character chr = new Character()
            {
                UserId = userId,
                UUID = player.ClientUUId,
                Health = player.statLife,
                MaxHealth = player.statLifeMax,
                Mana = player.statMana,
                MaxMana = player.statManaMax,
                SpawnX = player.SpawnX,
                SpawnY = player.SpawnY,
                Hair = player.hair,
                HairDye = player.hairDye,
                HideVisual = DataEncoding.EncodeInteger(player.hideVisual),
                Difficulty = player.difficulty,
                HairColor = DataEncoding.EncodeColor(player.hairColor),
                SkinColor = DataEncoding.EncodeColor(player.skinColor),
                EyeColor = DataEncoding.EncodeColor(player.eyeColor),
                ShirtColor = DataEncoding.EncodeColor(player.shirtColor),
                UnderShirtColor = DataEncoding.EncodeColor(player.underShirtColor),
                PantsColor = DataEncoding.EncodeColor(player.pantsColor),
                ShoeColor = DataEncoding.EncodeColor(player.shoeColor),
                AnglerQuests = player.anglerQuestsFinished
            };
            ctx.Characters.Add(chr);

            ctx.SaveChanges();

            return chr;
        }
#elif DAPPER
        public static Character GetCharacter(this IDbConnection ctx, CharacterMode mode, int playerId, string clientUUID)
        {
            if (mode == CharacterMode.AUTH)
            {
                return ctx.SingleOrDefault<Character>(new { PlayerId = playerId });
            }
            else
            {
                return ctx.SingleOrDefault<Character>(new { UUID = clientUUID });
            }
        }

        public static Character UpdateCharacter(this Character character, IDbConnection ctx, int playerId, Terraria.Player player)
        {
            character.PlayerId = playerId;
            character.UUID = player.ClientUUId;
            character.Health = player.statLife;
            character.MaxHealth = player.statLifeMax;
            character.Mana = player.statMana;
            character.MaxMana = player.statManaMax;
            character.SpawnX = player.SpawnX;
            character.SpawnY = player.SpawnY;
            character.Hair = player.hair;
            character.HairDye = player.hairDye;
            character.HideVisual = DataEncoding.EncodeInteger(player.hideVisual);
            character.Difficulty = player.difficulty;
            character.HairColor = DataEncoding.EncodeColorAsInt32(player.hairColor);
            character.SkinColor = DataEncoding.EncodeColorAsInt32(player.skinColor);
            character.EyeColor = DataEncoding.EncodeColorAsInt32(player.eyeColor);
            character.ShirtColor = DataEncoding.EncodeColorAsInt32(player.shirtColor);
            character.UnderShirtColor = DataEncoding.EncodeColorAsInt32(player.underShirtColor);
            character.PantsColor = DataEncoding.EncodeColorAsInt32(player.pantsColor);
            character.ShoeColor = DataEncoding.EncodeColorAsInt32(player.shoeColor);
            character.AnglerQuests = player.anglerQuestsFinished;

            ctx.Update(character);

            return character;
        }

        public static Character AddCharacter(this IDbConnection ctx, int playerId, Terraria.Player player)
        {
            Character chr = new Character()
            {
                PlayerId = playerId,
                UUID = player.ClientUUId,
                Health = player.statLife,
                MaxHealth = player.statLifeMax,
                Mana = player.statMana,
                MaxMana = player.statManaMax,
                SpawnX = player.SpawnX,
                SpawnY = player.SpawnY,
                Hair = player.hair,
                HairDye = player.hairDye,
                HideVisual = DataEncoding.EncodeInteger(player.hideVisual),
                Difficulty = player.difficulty,
                HairColor = DataEncoding.EncodeColorAsInt32(player.hairColor),
                SkinColor = DataEncoding.EncodeColorAsInt32(player.skinColor),
                EyeColor = DataEncoding.EncodeColorAsInt32(player.eyeColor),
                ShirtColor = DataEncoding.EncodeColorAsInt32(player.shirtColor),
                UnderShirtColor = DataEncoding.EncodeColorAsInt32(player.underShirtColor),
                PantsColor = DataEncoding.EncodeColorAsInt32(player.pantsColor),
                ShoeColor = DataEncoding.EncodeColorAsInt32(player.shoeColor),
                AnglerQuests = player.anglerQuestsFinished
            };

            chr.Id = ctx.Insert(chr);

            return chr;
        }
#endif
    }
}

