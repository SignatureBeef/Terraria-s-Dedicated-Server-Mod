using System;
using TDSM.Core.ServerCharacters;
using System.Linq;
using TDSM.Core.Data.Models;
using OTA;

namespace TDSM.Core.Data.Extensions
{
    public static class CharacterExtensions
    {
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
            character.HideVisual = Tools.Encoding.EncodeInteger(player.hideVisual);
            character.Difficulty = player.difficulty;
            character.HairColor = Tools.Encoding.EncodeColor(player.hairColor);
            character.SkinColor = Tools.Encoding.EncodeColor(player.skinColor);
            character.EyeColor = Tools.Encoding.EncodeColor(player.eyeColor);
            character.ShirtColor = Tools.Encoding.EncodeColor(player.shirtColor);
            character.UnderShirtColor = Tools.Encoding.EncodeColor(player.underShirtColor);
            character.PantsColor = Tools.Encoding.EncodeColor(player.pantsColor);
            character.ShoeColor = Tools.Encoding.EncodeColor(player.shoeColor);
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
                HideVisual = Tools.Encoding.EncodeInteger(player.hideVisual),
                Difficulty = player.difficulty,
                HairColor = Tools.Encoding.EncodeColor(player.hairColor),
                SkinColor = Tools.Encoding.EncodeColor(player.skinColor),
                EyeColor = Tools.Encoding.EncodeColor(player.eyeColor),
                ShirtColor = Tools.Encoding.EncodeColor(player.shirtColor),
                UnderShirtColor = Tools.Encoding.EncodeColor(player.underShirtColor),
                PantsColor = Tools.Encoding.EncodeColor(player.pantsColor),
                ShoeColor = Tools.Encoding.EncodeColor(player.shoeColor),
                AnglerQuests = player.anglerQuestsFinished
            };
            ctx.Characters.Add(chr);

            ctx.SaveChanges();

            return chr;
        }
    }
}

