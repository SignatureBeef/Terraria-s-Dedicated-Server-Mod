using System;
using System.ComponentModel;
using System.Globalization;
namespace Microsoft.Xna.Framework
{
	/// <summary>Represents a four-component color using red, green, blue, and alpha data.</summary>
	//[TypeConverter(typeof(ColorConverter))]
	[Serializable]
	public struct Color : IPackedVector<uint>, IPackedVector, IEquatable<Color>
	{
		private uint packedValue;
		/// <summary>Gets or sets the red component value of this Color.</summary>
		public byte R
		{
			get
			{
				return (byte)this.packedValue;
			}
			set
			{
				this.packedValue = ((this.packedValue & 4294967040u) | (uint)value);
			}
		}
		/// <summary>Gets or sets the green component value of this Color.</summary>
		public byte G
		{
			get
			{
				return (byte)(this.packedValue >> 8);
			}
			set
			{
				this.packedValue = ((this.packedValue & 4294902015u) | (uint)((uint)value << 8));
			}
		}
		/// <summary>Gets or sets the blue component value of this Color.</summary>
		public byte B
		{
			get
			{
				return (byte)(this.packedValue >> 16);
			}
			set
			{
				this.packedValue = ((this.packedValue & 4278255615u) | (uint)((uint)value << 16));
			}
		}
		/// <summary>Gets or sets the alpha component value.</summary>
		public byte A
		{
			get
			{
				return (byte)(this.packedValue >> 24);
			}
			set
			{
				this.packedValue = ((this.packedValue & 16777215u) | (uint)((uint)value << 24));
			}
		}
		/// <summary>Gets the current color as a packed value.</summary>
		[CLSCompliant(false)]
		public uint PackedValue
		{
			get
			{
				return this.packedValue;
			}
			set
			{
				this.packedValue = value;
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:0 B:0 A:0.</summary>
		public static Color Transparent
		{
			get
			{
				return new Color(0u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:240 G:248 B:255 A:255.</summary>
		public static Color AliceBlue
		{
			get
			{
				return new Color(4294965488u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:250 G:235 B:215 A:255.</summary>
		public static Color AntiqueWhite
		{
			get
			{
				return new Color(4292340730u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:255 B:255 A:255.</summary>
		public static Color Aqua
		{
			get
			{
				return new Color(4294967040u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:127 G:255 B:212 A:255.</summary>
		public static Color Aquamarine
		{
			get
			{
				return new Color(4292149119u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:240 G:255 B:255 A:255.</summary>
		public static Color Azure
		{
			get
			{
				return new Color(4294967280u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:245 G:245 B:220 A:255.</summary>
		public static Color Beige
		{
			get
			{
				return new Color(4292670965u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:228 B:196 A:255.</summary>
		public static Color Bisque
		{
			get
			{
				return new Color(4291093759u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:0 B:0 A:255.</summary>
		public static Color Black
		{
			get
			{
				return new Color(4278190080u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:235 B:205 A:255.</summary>
		public static Color BlanchedAlmond
		{
			get
			{
				return new Color(4291685375u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:0 B:255 A:255.</summary>
		public static Color Blue
		{
			get
			{
				return new Color(4294901760u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:138 G:43 B:226 A:255.</summary>
		public static Color BlueViolet
		{
			get
			{
				return new Color(4293012362u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:165 G:42 B:42 A:255.</summary>
		public static Color Brown
		{
			get
			{
				return new Color(4280953509u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:222 G:184 B:135 A:255.</summary>
		public static Color BurlyWood
		{
			get
			{
				return new Color(4287084766u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:95 G:158 B:160 A:255.</summary>
		public static Color CadetBlue
		{
			get
			{
				return new Color(4288716383u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:127 G:255 B:0 A:255.</summary>
		public static Color Chartreuse
		{
			get
			{
				return new Color(4278255487u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:210 G:105 B:30 A:255.</summary>
		public static Color Chocolate
		{
			get
			{
				return new Color(4280183250u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:127 B:80 A:255.</summary>
		public static Color Coral
		{
			get
			{
				return new Color(4283465727u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:100 G:149 B:237 A:255.</summary>
		public static Color CornflowerBlue
		{
			get
			{
				return new Color(4293760356u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:248 B:220 A:255.</summary>
		public static Color Cornsilk
		{
			get
			{
				return new Color(4292671743u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:220 G:20 B:60 A:255.</summary>
		public static Color Crimson
		{
			get
			{
				return new Color(4282127580u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:255 B:255 A:255.</summary>
		public static Color Cyan
		{
			get
			{
				return new Color(4294967040u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:0 B:139 A:255.</summary>
		public static Color DarkBlue
		{
			get
			{
				return new Color(4287299584u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:139 B:139 A:255.</summary>
		public static Color DarkCyan
		{
			get
			{
				return new Color(4287335168u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:184 G:134 B:11 A:255.</summary>
		public static Color DarkGoldenrod
		{
			get
			{
				return new Color(4278945464u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:169 G:169 B:169 A:255.</summary>
		public static Color DarkGray
		{
			get
			{
				return new Color(4289309097u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:100 B:0 A:255.</summary>
		public static Color DarkGreen
		{
			get
			{
				return new Color(4278215680u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:189 G:183 B:107 A:255.</summary>
		public static Color DarkKhaki
		{
			get
			{
				return new Color(4285249469u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:139 G:0 B:139 A:255.</summary>
		public static Color DarkMagenta
		{
			get
			{
				return new Color(4287299723u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:85 G:107 B:47 A:255.</summary>
		public static Color DarkOliveGreen
		{
			get
			{
				return new Color(4281297749u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:140 B:0 A:255.</summary>
		public static Color DarkOrange
		{
			get
			{
				return new Color(4278226175u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:153 G:50 B:204 A:255.</summary>
		public static Color DarkOrchid
		{
			get
			{
				return new Color(4291572377u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:139 G:0 B:0 A:255.</summary>
		public static Color DarkRed
		{
			get
			{
				return new Color(4278190219u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:233 G:150 B:122 A:255.</summary>
		public static Color DarkSalmon
		{
			get
			{
				return new Color(4286224105u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:143 G:188 B:139 A:255.</summary>
		public static Color DarkSeaGreen
		{
			get
			{
				return new Color(4287347855u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:72 G:61 B:139 A:255.</summary>
		public static Color DarkSlateBlue
		{
			get
			{
				return new Color(4287315272u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:47 G:79 B:79 A:255.</summary>
		public static Color DarkSlateGray
		{
			get
			{
				return new Color(4283387695u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:206 B:209 A:255.</summary>
		public static Color DarkTurquoise
		{
			get
			{
				return new Color(4291939840u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:148 G:0 B:211 A:255.</summary>
		public static Color DarkViolet
		{
			get
			{
				return new Color(4292018324u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:20 B:147 A:255.</summary>
		public static Color DeepPink
		{
			get
			{
				return new Color(4287829247u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:191 B:255 A:255.</summary>
		public static Color DeepSkyBlue
		{
			get
			{
				return new Color(4294950656u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:105 G:105 B:105 A:255.</summary>
		public static Color DimGray
		{
			get
			{
				return new Color(4285098345u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:30 G:144 B:255 A:255.</summary>
		public static Color DodgerBlue
		{
			get
			{
				return new Color(4294938654u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:178 G:34 B:34 A:255.</summary>
		public static Color Firebrick
		{
			get
			{
				return new Color(4280427186u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:250 B:240 A:255.</summary>
		public static Color FloralWhite
		{
			get
			{
				return new Color(4293982975u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:34 G:139 B:34 A:255.</summary>
		public static Color ForestGreen
		{
			get
			{
				return new Color(4280453922u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:0 B:255 A:255.</summary>
		public static Color Fuchsia
		{
			get
			{
				return new Color(4294902015u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:220 G:220 B:220 A:255.</summary>
		public static Color Gainsboro
		{
			get
			{
				return new Color(4292664540u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:248 G:248 B:255 A:255.</summary>
		public static Color GhostWhite
		{
			get
			{
				return new Color(4294965496u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:215 B:0 A:255.</summary>
		public static Color Gold
		{
			get
			{
				return new Color(4278245375u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:218 G:165 B:32 A:255.</summary>
		public static Color Goldenrod
		{
			get
			{
				return new Color(4280329690u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:128 G:128 B:128 A:255.</summary>
		public static Color Gray
		{
			get
			{
				return new Color(4286611584u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:128 B:0 A:255.</summary>
		public static Color Green
		{
			get
			{
				return new Color(4278222848u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:173 G:255 B:47 A:255.</summary>
		public static Color GreenYellow
		{
			get
			{
				return new Color(4281335725u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:240 G:255 B:240 A:255.</summary>
		public static Color Honeydew
		{
			get
			{
				return new Color(4293984240u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:105 B:180 A:255.</summary>
		public static Color HotPink
		{
			get
			{
				return new Color(4290013695u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:205 G:92 B:92 A:255.</summary>
		public static Color IndianRed
		{
			get
			{
				return new Color(4284243149u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:75 G:0 B:130 A:255.</summary>
		public static Color Indigo
		{
			get
			{
				return new Color(4286709835u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:255 B:240 A:255.</summary>
		public static Color Ivory
		{
			get
			{
				return new Color(4293984255u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:240 G:230 B:140 A:255.</summary>
		public static Color Khaki
		{
			get
			{
				return new Color(4287424240u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:230 G:230 B:250 A:255.</summary>
		public static Color Lavender
		{
			get
			{
				return new Color(4294633190u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:240 B:245 A:255.</summary>
		public static Color LavenderBlush
		{
			get
			{
				return new Color(4294308095u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:124 G:252 B:0 A:255.</summary>
		public static Color LawnGreen
		{
			get
			{
				return new Color(4278254716u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:250 B:205 A:255.</summary>
		public static Color LemonChiffon
		{
			get
			{
				return new Color(4291689215u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:173 G:216 B:230 A:255.</summary>
		public static Color LightBlue
		{
			get
			{
				return new Color(4293318829u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:240 G:128 B:128 A:255.</summary>
		public static Color LightCoral
		{
			get
			{
				return new Color(4286611696u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:224 G:255 B:255 A:255.</summary>
		public static Color LightCyan
		{
			get
			{
				return new Color(4294967264u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:250 G:250 B:210 A:255.</summary>
		public static Color LightGoldenrodYellow
		{
			get
			{
				return new Color(4292016890u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:144 G:238 B:144 A:255.</summary>
		public static Color LightGreen
		{
			get
			{
				return new Color(4287688336u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:211 G:211 B:211 A:255.</summary>
		public static Color LightGray
		{
			get
			{
				return new Color(4292072403u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:182 B:193 A:255.</summary>
		public static Color LightPink
		{
			get
			{
				return new Color(4290885375u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:160 B:122 A:255.</summary>
		public static Color LightSalmon
		{
			get
			{
				return new Color(4286226687u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:32 G:178 B:170 A:255.</summary>
		public static Color LightSeaGreen
		{
			get
			{
				return new Color(4289376800u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:135 G:206 B:250 A:255.</summary>
		public static Color LightSkyBlue
		{
			get
			{
				return new Color(4294626951u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:119 G:136 B:153 A:255.</summary>
		public static Color LightSlateGray
		{
			get
			{
				return new Color(4288252023u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:176 G:196 B:222 A:255.</summary>
		public static Color LightSteelBlue
		{
			get
			{
				return new Color(4292789424u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:255 B:224 A:255.</summary>
		public static Color LightYellow
		{
			get
			{
				return new Color(4292935679u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:255 B:0 A:255.</summary>
		public static Color Lime
		{
			get
			{
				return new Color(4278255360u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:50 G:205 B:50 A:255.</summary>
		public static Color LimeGreen
		{
			get
			{
				return new Color(4281519410u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:250 G:240 B:230 A:255.</summary>
		public static Color Linen
		{
			get
			{
				return new Color(4293325050u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:0 B:255 A:255.</summary>
		public static Color Magenta
		{
			get
			{
				return new Color(4294902015u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:128 G:0 B:0 A:255.</summary>
		public static Color Maroon
		{
			get
			{
				return new Color(4278190208u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:102 G:205 B:170 A:255.</summary>
		public static Color MediumAquamarine
		{
			get
			{
				return new Color(4289383782u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:0 B:205 A:255.</summary>
		public static Color MediumBlue
		{
			get
			{
				return new Color(4291624960u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:186 G:85 B:211 A:255.</summary>
		public static Color MediumOrchid
		{
			get
			{
				return new Color(4292040122u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:147 G:112 B:219 A:255.</summary>
		public static Color MediumPurple
		{
			get
			{
				return new Color(4292571283u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:60 G:179 B:113 A:255.</summary>
		public static Color MediumSeaGreen
		{
			get
			{
				return new Color(4285641532u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:123 G:104 B:238 A:255.</summary>
		public static Color MediumSlateBlue
		{
			get
			{
				return new Color(4293814395u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:250 B:154 A:255.</summary>
		public static Color MediumSpringGreen
		{
			get
			{
				return new Color(4288346624u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:72 G:209 B:204 A:255.</summary>
		public static Color MediumTurquoise
		{
			get
			{
				return new Color(4291613000u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:199 G:21 B:133 A:255.</summary>
		public static Color MediumVioletRed
		{
			get
			{
				return new Color(4286911943u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:25 G:25 B:112 A:255.</summary>
		public static Color MidnightBlue
		{
			get
			{
				return new Color(4285536537u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:245 G:255 B:250 A:255.</summary>
		public static Color MintCream
		{
			get
			{
				return new Color(4294639605u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:228 B:225 A:255.</summary>
		public static Color MistyRose
		{
			get
			{
				return new Color(4292994303u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:228 B:181 A:255.</summary>
		public static Color Moccasin
		{
			get
			{
				return new Color(4290110719u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:222 B:173 A:255.</summary>
		public static Color NavajoWhite
		{
			get
			{
				return new Color(4289584895u);
			}
		}
		/// <summary>Gets a system-defined color R:0 G:0 B:128 A:255.</summary>
		public static Color Navy
		{
			get
			{
				return new Color(4286578688u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:253 G:245 B:230 A:255.</summary>
		public static Color OldLace
		{
			get
			{
				return new Color(4293326333u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:128 G:128 B:0 A:255.</summary>
		public static Color Olive
		{
			get
			{
				return new Color(4278222976u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:107 G:142 B:35 A:255.</summary>
		public static Color OliveDrab
		{
			get
			{
				return new Color(4280520299u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:165 B:0 A:255.</summary>
		public static Color Orange
		{
			get
			{
				return new Color(4278232575u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:69 B:0 A:255.</summary>
		public static Color OrangeRed
		{
			get
			{
				return new Color(4278207999u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:218 G:112 B:214 A:255.</summary>
		public static Color Orchid
		{
			get
			{
				return new Color(4292243674u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:238 G:232 B:170 A:255.</summary>
		public static Color PaleGoldenrod
		{
			get
			{
				return new Color(4289390830u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:152 G:251 B:152 A:255.</summary>
		public static Color PaleGreen
		{
			get
			{
				return new Color(4288215960u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:175 G:238 B:238 A:255.</summary>
		public static Color PaleTurquoise
		{
			get
			{
				return new Color(4293848751u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:219 G:112 B:147 A:255.</summary>
		public static Color PaleVioletRed
		{
			get
			{
				return new Color(4287852763u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:239 B:213 A:255.</summary>
		public static Color PapayaWhip
		{
			get
			{
				return new Color(4292210687u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:218 B:185 A:255.</summary>
		public static Color PeachPuff
		{
			get
			{
				return new Color(4290370303u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:205 G:133 B:63 A:255.</summary>
		public static Color Peru
		{
			get
			{
				return new Color(4282353101u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:192 B:203 A:255.</summary>
		public static Color Pink
		{
			get
			{
				return new Color(4291543295u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:221 G:160 B:221 A:255.</summary>
		public static Color Plum
		{
			get
			{
				return new Color(4292714717u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:176 G:224 B:230 A:255.</summary>
		public static Color PowderBlue
		{
			get
			{
				return new Color(4293320880u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:128 G:0 B:128 A:255.</summary>
		public static Color Purple
		{
			get
			{
				return new Color(4286578816u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:0 B:0 A:255.</summary>
		public static Color Red
		{
			get
			{
				return new Color(4278190335u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:188 G:143 B:143 A:255.</summary>
		public static Color RosyBrown
		{
			get
			{
				return new Color(4287598524u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:65 G:105 B:225 A:255.</summary>
		public static Color RoyalBlue
		{
			get
			{
				return new Color(4292962625u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:139 G:69 B:19 A:255.</summary>
		public static Color SaddleBrown
		{
			get
			{
				return new Color(4279453067u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:250 G:128 B:114 A:255.</summary>
		public static Color Salmon
		{
			get
			{
				return new Color(4285694202u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:244 G:164 B:96 A:255.</summary>
		public static Color SandyBrown
		{
			get
			{
				return new Color(4284523764u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:46 G:139 B:87 A:255.</summary>
		public static Color SeaGreen
		{
			get
			{
				return new Color(4283927342u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:245 B:238 A:255.</summary>
		public static Color SeaShell
		{
			get
			{
				return new Color(4293850623u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:160 G:82 B:45 A:255.</summary>
		public static Color Sienna
		{
			get
			{
				return new Color(4281160352u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:192 G:192 B:192 A:255.</summary>
		public static Color Silver
		{
			get
			{
				return new Color(4290822336u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:135 G:206 B:235 A:255.</summary>
		public static Color SkyBlue
		{
			get
			{
				return new Color(4293643911u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:106 G:90 B:205 A:255.</summary>
		public static Color SlateBlue
		{
			get
			{
				return new Color(4291648106u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:112 G:128 B:144 A:255.</summary>
		public static Color SlateGray
		{
			get
			{
				return new Color(4287660144u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:250 B:250 A:255.</summary>
		public static Color Snow
		{
			get
			{
				return new Color(4294638335u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:255 B:127 A:255.</summary>
		public static Color SpringGreen
		{
			get
			{
				return new Color(4286578432u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:70 G:130 B:180 A:255.</summary>
		public static Color SteelBlue
		{
			get
			{
				return new Color(4290019910u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:210 G:180 B:140 A:255.</summary>
		public static Color Tan
		{
			get
			{
				return new Color(4287411410u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:0 G:128 B:128 A:255.</summary>
		public static Color Teal
		{
			get
			{
				return new Color(4286611456u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:216 G:191 B:216 A:255.</summary>
		public static Color Thistle
		{
			get
			{
				return new Color(4292394968u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:99 B:71 A:255.</summary>
		public static Color Tomato
		{
			get
			{
				return new Color(4282868735u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:64 G:224 B:208 A:255.</summary>
		public static Color Turquoise
		{
			get
			{
				return new Color(4291878976u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:238 G:130 B:238 A:255.</summary>
		public static Color Violet
		{
			get
			{
				return new Color(4293821166u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:245 G:222 B:179 A:255.</summary>
		public static Color Wheat
		{
			get
			{
				return new Color(4289978101u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:255 B:255 A:255.</summary>
		public static Color White
		{
			get
			{
				return new Color(4294967295u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:245 G:245 B:245 A:255.</summary>
		public static Color WhiteSmoke
		{
			get
			{
				return new Color(4294309365u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:255 G:255 B:0 A:255.</summary>
		public static Color Yellow
		{
			get
			{
				return new Color(4278255615u);
			}
		}
		/// <summary>Gets a system-defined color with the value R:154 G:205 B:50 A:255.</summary>
		public static Color YellowGreen
		{
			get
			{
				return new Color(4281519514u);
			}
		}
		private Color(uint packedValue)
		{
			this.packedValue = packedValue;
		}
		/// <summary>Creates a new instance of the class.</summary>
		/// <param name="r">Red component.</param>
		/// <param name="g">Green component.</param>
		/// <param name="b">Blue component.</param>
		public Color(int r, int g, int b)
		{
			if (((r | g | b) & -256) != 0)
			{
				r = Color.ClampToByte64((long)r);
				g = Color.ClampToByte64((long)g);
				b = Color.ClampToByte64((long)b);
			}
			g <<= 8;
			b <<= 16;
			this.packedValue = (uint)(r | g | b | -16777216);
		}
		/// <summary>Creates a new instance of the class.</summary>
		/// <param name="r">Red component.</param>
		/// <param name="g">Green component.</param>
		/// <param name="b">Blue component.</param>
		/// <param name="a">Alpha component.</param>
		public Color(int r, int g, int b, int a)
		{
			if (((r | g | b | a) & -256) != 0)
			{
				r = Color.ClampToByte32(r);
				g = Color.ClampToByte32(g);
				b = Color.ClampToByte32(b);
				a = Color.ClampToByte32(a);
			}
			g <<= 8;
			b <<= 16;
			a <<= 24;
			this.packedValue = (uint)(r | g | b | a);
		}
		/// <summary>Creates a new instance of the class.</summary>
		/// <param name="r">Red component.</param>
		/// <param name="g">Green component.</param>
		/// <param name="b">Blue component.</param>
		public Color(float r, float g, float b)
		{
			this.packedValue = Color.PackHelper(r, g, b, 1f);
		}
		/// <summary>Creates a new instance of the class.</summary>
		/// <param name="r">Red component.</param>
		/// <param name="g">Green component.</param>
		/// <param name="b">Blue component.</param>
		/// <param name="a">Alpha component.</param>
		public Color(float r, float g, float b, float a)
		{
			this.packedValue = Color.PackHelper(r, g, b, a);
		}
		/// <summary>Creates a new instance of the class.</summary>
		/// <param name="vector">A three-component color.</param>
		public Color(Vector3 vector)
		{
			this.packedValue = Color.PackHelper(vector.X, vector.Y, vector.Z, 1f);
		}
		/// <summary>Creates a new instance of the class.</summary>
		/// <param name="vector">A four-component color.</param>
		public Color(Vector4 vector)
		{
			this.packedValue = Color.PackHelper(vector.X, vector.Y, vector.Z, vector.W);
		}
		/// <summary>Pack a four-component color from a vector format into the format of a color object.</summary>
		/// <param name="vector">A four-component color.</param>
		void IPackedVector.PackFromVector4(Vector4 vector)
		{
			this.packedValue = Color.PackHelper(vector.X, vector.Y, vector.Z, vector.W);
		}
		/// <summary>Convert a non premultipled color into color data that contains alpha.</summary>
		/// <param name="vector">A four-component color.</param>
		public static Color FromNonPremultiplied(Vector4 vector)
		{
			Color result;
			result.packedValue = Color.PackHelper(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W, vector.W);
			return result;
		}
		/// <summary>Converts a non-premultipled alpha color to a color that contains premultiplied alpha.</summary>
		/// <param name="r">Red component.</param>
		/// <param name="g">Green component.</param>
		/// <param name="b">Blue component.</param>
		/// <param name="a">Alpha component.</param>
		public static Color FromNonPremultiplied(int r, int g, int b, int a)
		{
			r = Color.ClampToByte64((long)r * (long)a / 255L);
			g = Color.ClampToByte64((long)g * (long)a / 255L);
			b = Color.ClampToByte64((long)b * (long)a / 255L);
			a = Color.ClampToByte32(a);
			g <<= 8;
			b <<= 16;
			a <<= 24;
			Color result;
			result.packedValue = (uint)(r | g | b | a);
			return result;
		}
		private static uint PackHelper(float vectorX, float vectorY, float vectorZ, float vectorW)
		{
			uint num = PackUNorm(255f, vectorX);
			uint num2 = PackUNorm(255f, vectorY) << 8;
			uint num3 = PackUNorm(255f, vectorZ) << 16;
			uint num4 = PackUNorm(255f, vectorW) << 24;
			return num | num2 | num3 | num4;
		}

        // Microsoft.Xna.Framework.Graphics.PackedVector.PackUtils
        public static uint PackUNorm(float bitmask, float value)
        {
            value *= bitmask;
            return (uint)ClampAndRound(value, 0f, bitmask);
        }

        // Microsoft.Xna.Framework.Graphics.PackedVector.PackUtils
        private static double ClampAndRound(float value, float min, float max)
        {
            if (float.IsNaN(value))
            {
                return 0.0;
            }
            if (float.IsInfinity(value))
            {
                return (double)(float.IsNegativeInfinity(value) ? min : max);
            }
            if (value < min)
            {
                return (double)min;
            }
            if (value > max)
            {
                return (double)max;
            }
            return Math.Round((double)value);
        }


		private static int ClampToByte32(int value)
		{
			if (value < 0)
			{
				return 0;
			}
			if (value > 255)
			{
				return 255;
			}
			return value;
		}
		private static int ClampToByte64(long value)
		{
			if (value < 0L)
			{
				return 0;
			}
			if (value > 255L)
			{
				return 255;
			}
			return (int)value;
		}
		/// <summary>Gets a three-component vector representation for this object.</summary>
		public Vector3 ToVector3()
		{
			Vector3 result;
			result.X = UnpackUNorm(255u, this.packedValue);
			result.Y = UnpackUNorm(255u, this.packedValue >> 8);
			result.Z = UnpackUNorm(255u, this.packedValue >> 16);
			return result;
		}
		/// <summary>Gets a four-component vector representation for this object.</summary>
		public Vector4 ToVector4()
		{
			Vector4 result;
			result.X = UnpackUNorm(255u, this.packedValue);
			result.Y = UnpackUNorm(255u, this.packedValue >> 8);
			result.Z = UnpackUNorm(255u, this.packedValue >> 16);
			result.W = UnpackUNorm(255u, this.packedValue >> 24);
			return result;
		}

        public static float UnpackUNorm(uint bitmask, uint value)
        {
            value &= bitmask;
            return value / bitmask;
        }
		/// <summary>Linearly interpolate a color.</summary>
		/// <param name="value1">A four-component color.</param>
		/// <param name="value2">A four-component color.</param>
		/// <param name="amount">Interpolation factor.</param>
		public static Color Lerp(Color value1, Color value2, float amount)
		{
			uint num = value1.packedValue;
			uint num2 = value2.packedValue;
			int num3 = (int)((byte)num);
			int num4 = (int)((byte)(num >> 8));
			int num5 = (int)((byte)(num >> 16));
			int num6 = (int)((byte)(num >> 24));
			int num7 = (int)((byte)num2);
			int num8 = (int)((byte)(num2 >> 8));
			int num9 = (int)((byte)(num2 >> 16));
			int num10 = (int)((byte)(num2 >> 24));
			int num11 = (int)PackUNorm(65536f, amount);
			int num12 = num3 + ((num7 - num3) * num11 >> 16);
			int num13 = num4 + ((num8 - num4) * num11 >> 16);
			int num14 = num5 + ((num9 - num5) * num11 >> 16);
			int num15 = num6 + ((num10 - num6) * num11 >> 16);
			Color result;
			result.packedValue = (uint)(num12 | num13 << 8 | num14 << 16 | num15 << 24);
			return result;
		}
		/// <summary>Multiply each color component by the scale factor.</summary>
		/// <param name="value">The source, four-component color.</param>
		/// <param name="scale">The scale factor.</param>
		public static Color Multiply(Color value, float scale)
		{
			uint num = value.packedValue;
			uint num2 = (uint)((byte)num);
			uint num3 = (uint)((byte)(num >> 8));
			uint num4 = (uint)((byte)(num >> 16));
			uint num5 = (uint)((byte)(num >> 24));
			scale *= 65536f;
			uint num6;
			if (scale < 0f)
			{
				num6 = 0u;
			}
			else
			{
				if (scale > 16777215f)
				{
					num6 = 16777215u;
				}
				else
				{
					num6 = (uint)scale;
				}
			}
			num2 = num2 * num6 >> 16;
			num3 = num3 * num6 >> 16;
			num4 = num4 * num6 >> 16;
			num5 = num5 * num6 >> 16;
			if (num2 > 255u)
			{
				num2 = 255u;
			}
			if (num3 > 255u)
			{
				num3 = 255u;
			}
			if (num4 > 255u)
			{
				num4 = 255u;
			}
			if (num5 > 255u)
			{
				num5 = 255u;
			}
			Color result;
			result.packedValue = (num2 | num3 << 8 | num4 << 16 | num5 << 24);
			return result;
		}
		/// <summary>Multiply operator.</summary>
		/// <param name="value">A four-component color</param>
		/// <param name="scale">Scale factor.</param>
		public static Color operator *(Color value, float scale)
		{
			uint num = value.packedValue;
			uint num2 = (uint)((byte)num);
			uint num3 = (uint)((byte)(num >> 8));
			uint num4 = (uint)((byte)(num >> 16));
			uint num5 = (uint)((byte)(num >> 24));
			scale *= 65536f;
			uint num6;
			if (scale < 0f)
			{
				num6 = 0u;
			}
			else
			{
				if (scale > 16777215f)
				{
					num6 = 16777215u;
				}
				else
				{
					num6 = (uint)scale;
				}
			}
			num2 = num2 * num6 >> 16;
			num3 = num3 * num6 >> 16;
			num4 = num4 * num6 >> 16;
			num5 = num5 * num6 >> 16;
			if (num2 > 255u)
			{
				num2 = 255u;
			}
			if (num3 > 255u)
			{
				num3 = 255u;
			}
			if (num4 > 255u)
			{
				num4 = 255u;
			}
			if (num5 > 255u)
			{
				num5 = 255u;
			}
			Color result;
			result.packedValue = (num2 | num3 << 8 | num4 << 16 | num5 << 24);
			return result;
		}
		/// <summary>Gets a string representation of this object.</summary>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{{R:{0} G:{1} B:{2} A:{3}}}", new object[]
			{
				this.R,
				this.G,
				this.B,
				this.A
			});
		}
		/// <summary>Serves as a hash function for a particular type.</summary>
		public override int GetHashCode()
		{
			return this.packedValue.GetHashCode();
		}
		/// <summary>Test an instance of a color object to see if it is equal to this object.</summary>
		/// <param name="obj">A color object.</param>
		public override bool Equals(object obj)
		{
			return obj is Color && this.Equals((Color)obj);
		}
		/// <summary>Test a color to see if it is equal to the color in this instance.</summary>
		/// <param name="other">A four-component color.</param>
		public bool Equals(Color other)
		{
			return this.packedValue.Equals(other.packedValue);
		}
		/// <summary>Equality operator.</summary>
		/// <param name="a">A four-component color.</param>
		/// <param name="b">A four-component color.</param>
		public static bool operator ==(Color a, Color b)
		{
			return a.Equals(b);
		}
		/// <summary>Equality operator for Testing two color objects to see if they are equal.</summary>
		/// <param name="a">A four-component color.</param>
		/// <param name="b">A four-component color.</param>
		public static bool operator !=(Color a, Color b)
		{
			return !a.Equals(b);
		}
	}
}
