
namespace Terraria_Server.Misc
{
    public struct Color // TODO: change fields to bytes, delete A
    {
        public int R;
        public int G;
        public int B;
        public int A;

        public Color(int r, int g, int b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = 0;
        }

        public Color(int r, int g, int b, int a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }
        
		public static bool TryParseRGB (string rgb, out Color color)
		{
			color = new Color (255, 255, 255, 255);
			
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
				
				color.R = val >> 16;
				color.G = (val >> 8) & 0xff;
				color.B = val & 0xff;
				
				return true;
			}
			else
			{
				var split = rgb.Split (',');
				
				if (split.Length != 3) return false;
				
				if (! int.TryParse (split[0], out color.R))
					return false;
					
				if (! int.TryParse (split[1], out color.G))
					return false;

				if (! int.TryParse (split[2], out color.B))
					return false;
				
				return true;
			}
		}

    }
}
