using System;
using Terraria_Server.Misc;

namespace Terraria_Server
{
	public class Gore
	{
		public static int goreTime = 600;
		public Vector2 position;
		public Vector2 velocity;
		public float rotation;
		public float scale;
		public int alpha;
		public int type;
		public float light;
		public bool active;
		public bool sticky = true;
		public int timeLeft = Gore.goreTime;

		public void Update()
		{
            return;
		}

		public static int NewGore(Vector2 Position, Vector2 Velocity, int Type)
		{
			if (Main.rand == null)
			{
				Main.rand = new Random();
			}

            return 0;
		}

		public Color GetAlpha(Color color)
		{
			int r;
			int g;
			int b;
			if (this.type == 16 || this.type == 17)
			{
				r = (int)color.R - this.alpha / 2;
				g = (int)color.G - this.alpha / 2;
				b = (int)color.B - this.alpha / 2;
			}
			else
			{
				r = (int)color.R - this.alpha;
				g = (int)color.G - this.alpha;
				b = (int)color.B - this.alpha;
			}

			int correctedAlpha = (int)color.A - this.alpha;
			if (correctedAlpha < 0)
			{
				correctedAlpha = 0;
			}
			if (correctedAlpha > 255)
			{
				correctedAlpha = 255;
			}
			return new Color(r, g, b, correctedAlpha);
		}
	}
}
