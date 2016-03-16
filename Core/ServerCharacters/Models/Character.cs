namespace TDSM.Core.ServerCharacters.Models
{
    public class Character
    {
        public long Id { get; set; }

        public long? PlayerId { get; set; }

        public string UUID { get; set; }

        public int Health { get; set; }

        public int MaxHealth { get; set; }

        public int Mana { get; set; }

        public int MaxMana { get; set; }

        public int SpawnX { get; set; }

        public int SpawnY { get; set; }

        public int Hair { get; set; }

        public byte HairDye { get; set; }

        public int HideVisual { get; set; }

        public byte Difficulty { get; set; }

        public int HairColor { get; set; }

        public int SkinColor { get; set; }

        public int EyeColor { get; set; }

        public int ShirtColor { get; set; }

        public int UnderShirtColor { get; set; }

        public int PantsColor { get; set; }

        public int ShoeColor { get; set; }

        public int AnglerQuests { get; set; }

        public ServerCharacter ToServerCharacter()
        {
            return new ServerCharacter()
            {
                Id = this.Id,
                PlayerId = this.PlayerId,
                UUID = this.UUID,

                Health = this.Health,
                MaxHealth = this.MaxHealth,
                Mana = this.Mana,
                MaxMana = this.MaxMana,
                SpawnX = this.SpawnX,
                SpawnY = this.SpawnY,
                Hair = this.Hair,
                HairDye = this.HairDye,
                HideVisual = OTA.Data.DataEncoding.DecodeBits(this.HideVisual),
                Difficulty = this.Difficulty,
                HairColor = OTA.Data.DataEncoding.DecodeColor(this.HairColor),
                SkinColor = OTA.Data.DataEncoding.DecodeColor(this.SkinColor),
                EyeColor = OTA.Data.DataEncoding.DecodeColor(this.EyeColor),
                ShirtColor = OTA.Data.DataEncoding.DecodeColor(this.ShirtColor),
                UnderShirtColor = OTA.Data.DataEncoding.DecodeColor(this.UnderShirtColor),
                PantsColor = OTA.Data.DataEncoding.DecodeColor(this.PantsColor),
                ShoeColor = OTA.Data.DataEncoding.DecodeColor(this.ShoeColor),
                AnglerQuests = this.AnglerQuests
            };
        }
    }
}
