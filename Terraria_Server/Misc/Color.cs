
using System;
namespace Terraria_Server.Misc
{
	public struct Color
	{
		public byte R;
		public byte G;
		public byte B;

		public Color(int r, int g, int b)
		{
			this.R = (byte)r;
			this.G = (byte)g;
			this.B = (byte)b;
		}

		public static bool TryParseRGB(string rgb, out Color color)
		{
			color = new Color(255, 255, 255);

			if (rgb.Length == 7 && rgb[0] == '#')
			{
				int val = 0;
				int shift = 0;

				for (int i = 6; i >= 1; i--)
				{
					var c = rgb[i];
					if (c >= '0' && c <= '9')
						val |= ((int)c - (int)'0') << shift;
					else if (c >= 'a' && c <= 'f')
						val |= (10 + (int)c - (int)'a') << shift;
					else if (c >= 'A' && c <= 'F')
						val |= (10 + (int)c - (int)'A') << shift;
					else
						return false;
					shift += 4;
				}

				color.R = (byte)(val >> 16);
				color.G = (byte)((val >> 8) & 0xff);
				color.B = (byte)(val & 0xff);

				return true;
			}
			else
			{
				var split = rgb.Split(',');

				if (split.Length != 3) return false;

				if (!byte.TryParse(split[0], out color.R))
					return false;

				if (!byte.TryParse(split[1], out color.G))
					return false;

				if (!byte.TryParse(split[2], out color.B))
					return false;

				return true;
			}
		}

		public override string ToString()
		{
			return "{R:" + R + " G:" + G + " B:" + B + "}";
		}

		public string ToParsableString()
		{
			return String.Concat(R, ',', G, ',', B);
		}

		public static bool operator !=(Color color1, Color color2)
		{
			return (color1.B != color2.B || color1.G != color2.G || color1.R != color2.R);
		}

		public static bool operator ==(Color color1, Color color2)
		{
			return (color1.B == color2.B && color1.G == color2.G && color1.R == color2.R);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
