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
			if (Main.netMode == 2)
			{
				return;
			}

			if (this.active)
			{
				if (this.type == 11 || this.type == 12 || this.type == 13 || this.type == 61 || this.type == 62 || this.type == 63)
				{
					this.velocity.Y = this.velocity.Y * 0.98f;
					this.velocity.X = this.velocity.X * 0.98f;
					this.scale -= 0.007f;
					if ((double)this.scale < 0.1)
					{
						this.scale = 0.1f;
						this.alpha = 255;
					}
				}
                else if (this.type == 16 || this.type == 17)
                {
                    this.velocity.Y = this.velocity.Y * 0.98f;
                    this.velocity.X = this.velocity.X * 0.98f;
                    this.scale -= 0.01f;
                    if ((double)this.scale < 0.1)
                    {
                        this.scale = 0.1f;
                        this.alpha = 255;
                    }
                }
                else
                {
                    this.velocity.Y = this.velocity.Y + 0.2f;
                }

				this.rotation += this.velocity.X * 0.1f;
				if (this.sticky)
				{
					if (this.timeLeft > 0)
					{
						this.timeLeft--;
					}
					else
					{
						this.alpha++;
					}
				}
				else
				{
					this.alpha += 2;
				}
				
                this.position += this.velocity;
				
                if (this.alpha >= 255)
				{
					this.active = false;
				}
			}
		}

		public static int NewGore(Vector2 Position, Vector2 Velocity, int Type)
		{
			if (Main.rand == null)
			{
				Main.rand = new Random();
			}

			if (Main.netMode == 2)
			{
				return 0;
			}

			int foundGore = 200;

			for (int i = 0; i < 200; i++)
			{
				if (!Main.gore[i].active)
				{
					foundGore = i;
					break;
				}
			}

			if (foundGore == 200)
			{
				return foundGore;
			}

			Main.gore[foundGore].light = 0f;
			Main.gore[foundGore].position = Position;
			Main.gore[foundGore].velocity = Velocity;
			Gore expr_84_cp_0 = Main.gore[foundGore];
			expr_84_cp_0.velocity.Y = expr_84_cp_0.velocity.Y - (float)Main.rand.Next(10, 31) * 0.1f;
			Gore expr_B1_cp_0 = Main.gore[foundGore];
			expr_B1_cp_0.velocity.X = expr_B1_cp_0.velocity.X + (float)Main.rand.Next(-20, 21) * 0.1f;
			Main.gore[foundGore].type = Type;
			Main.gore[foundGore].active = true;
			Main.gore[foundGore].alpha = 0;
			Main.gore[foundGore].rotation = 0f;
			Main.gore[foundGore].scale = 1f;
			if (Gore.goreTime == 0 || Type == 11 || Type == 12 || Type == 13 || Type == 16 || Type == 17 || Type == 61 || Type == 62 || Type == 63)
			{
				Main.gore[foundGore].sticky = false;
			}
			else
			{
				Main.gore[foundGore].sticky = true;
				Main.gore[foundGore].timeLeft = Gore.goreTime;
			}
			if (Type == 16 || Type == 17)
			{
				Main.gore[foundGore].alpha = 100;
				Main.gore[foundGore].scale = 0.7f;
				Main.gore[foundGore].light = 1f;
			}
			return foundGore;
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
