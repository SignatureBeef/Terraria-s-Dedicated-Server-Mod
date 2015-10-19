using System;
using OTA.Plugin;
using Microsoft.Xna.Framework;
using Terraria;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        public struct PlayerDataReceived
        {
            public bool IsConnecting { get; set; }

            public string Name { get; set; }

            public int SkinVariant { get; set; }

            public int Hair { get; set; }

            public byte HairDye { get; set; }

            public bool[] HideVisual { get; set; }

            public byte HideMisc { get; set; }

            public byte Difficulty { get; set; }

            public bool ExtraAccessory { get; set; }

            public Color HairColor { get; set; }

            public Color SkinColor { get; set; }

            public Color EyeColor { get; set; }

            public Color ShirtColor { get; set; }

            public Color UnderShirtColor { get; set; }

            public Color PantsColor { get; set; }

            public Color ShoeColor { get; set; }

            public bool NameChecked { get; set; }

            public bool BansChecked { get; set; }

            public bool WhitelistChecked { get; set; }

            public int Parse(System.IO.BinaryReader reader, int at, int length)
            {
                reader.ReadByte(); //Ignore player id

                SkinVariant = (int)MathHelper.Clamp((float)(int)reader.ReadByte(), 0, 7);
                Hair = (int)reader.ReadByte();
                if (Hair >= 134)
                    Hair = 0;

                Name = reader.ReadString().Trim();
                HairDye = reader.ReadByte();

                BitsByte bb;

                HideVisual = new bool[10];
                bb = reader.ReadByte();
                for (int i = 0; i < 8; i++)
                    HideVisual[i] = bb[i];

                bb = reader.ReadByte();
                for (int i = 0; i < 2; i++)
                    HideVisual[i + 8] = bb[i];

                HideMisc = reader.ReadByte();
                HairColor = reader.ReadRGB();
                SkinColor = reader.ReadRGB();
                EyeColor = reader.ReadRGB();
                ShirtColor = reader.ReadRGB();
                UnderShirtColor = reader.ReadRGB();
                PantsColor = reader.ReadRGB();
                ShoeColor = reader.ReadRGB();

                bb = reader.ReadByte();
                Difficulty = 0;
                if (bb[0]) Difficulty += 1;
                if (bb[1]) Difficulty += 2;
                if (Difficulty > 2) Difficulty = 2;

                ExtraAccessory = bb[2];

                return 0;
            }

            public void Apply(Terraria.Player player)
            {
                player.difficulty = Difficulty;
                player.name = Name;

                player.skinVariant = SkinVariant;
                player.hair = Hair;
                player.hairDye = HairDye;
                player.hideVisual = HideVisual;
                player.hideMisc = HideMisc;

                player.hairColor = HairColor;
                player.skinColor = SkinColor;
                player.eyeColor = EyeColor;
                player.shirtColor = ShirtColor;
                player.underShirtColor = UnderShirtColor;
                player.pantsColor = PantsColor;
                player.shoeColor = ShoeColor;
                player.extraAccessory = ExtraAccessory;

                Netplay.Clients[player.whoAmI].Name = Name;
            }

            public static Color ParseColor(byte[] buf, ref int at)
            {
                return new Color(buf[at++], buf[at++], buf[at++]);
            }

            public bool CheckName(out string error)
            {
                error = null;
                NameChecked = true;

                if (Name.Length > 20)
                {
                    error = "Invalid name: longer than 20 characters.";
                    return false;
                }

                if (Name == "")
                {
                    error = "Invalid name: whitespace or empty.";
                    return false;
                }

                foreach (char c in Name)
                {
                    if (c < 32 || c > 126)
                    {
                        //Console.Write ((byte) c);
                        error = "Invalid name: contains non-printable characters.";
                        return false;
                    }
                    //Console.Write (c);
                }

                if (Name.Contains(" " + " "))
                {
                    error = "Invalid name: contains double spaces.";
                    return false;
                }

                return true;
            }
        }
    }

    public static partial class TDSMHookPoints
    {
        public static readonly HookPoint<TDSMHookArgs.PlayerDataReceived> PlayerDataReceived = new HookPoint<TDSMHookArgs.PlayerDataReceived>();
    }
}