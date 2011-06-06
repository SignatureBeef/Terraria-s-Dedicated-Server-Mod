
using System;
namespace Terraria_Server
{
	public class Dust
	{
		public Vector2 position = new Vector2();
        public Vector2 velocity = new Vector2();
		public bool noGravity;
		public float scale;
		public float rotation;
		public bool noLight;
		public bool active;
		public int type;
		public Color color = new Color();
		public int alpha;
		public Rectangle frame = new Rectangle();
		public static int NewDust(Vector2 Position, int Width, int Height, int Type, float SpeedX = 0f, float SpeedY = 0f, int Alpha = 0, Color newColor = default(Color), float Scale = 1f)
		{
			if (WorldGen.gen)
			{
				return 0;
			}
			if (Main.netMode == 2)
			{
				return 0;
			}
			int result = 0;
			int i = 0;
			while (i < 2000)
			{
				if (!Main.dust[i].active)
				{
					result = i;
					Main.dust[i].active = true;
					Main.dust[i].type = Type;
					Main.dust[i].noGravity = false;
					Main.dust[i].color = newColor;
					Main.dust[i].alpha = Alpha;
					Main.dust[i].position.X = Position.X + (float)Main.rand.Next(Width - 4) + 4f;
					Main.dust[i].position.Y = Position.Y + (float)Main.rand.Next(Height - 4) + 4f;
					Main.dust[i].velocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + SpeedX;
					Main.dust[i].velocity.Y = (float)Main.rand.Next(-20, 21) * 0.1f + SpeedY;
					Main.dust[i].frame.X = 10 * Type;
					Main.dust[i].frame.Y = 10 * Main.rand.Next(3);
					Main.dust[i].frame.Width = 8;
					Main.dust[i].frame.Height = 8;
					Main.dust[i].rotation = 0f;
					Main.dust[i].scale = 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
					Main.dust[i].scale *= Scale;
					Main.dust[i].noLight = false;
					if (Main.dust[i].type == 6 || Main.dust[i].type == 29)
					{
						Main.dust[i].velocity.Y = (float)Main.rand.Next(-10, 6) * 0.1f;
						Dust expr_220_cp_0 = Main.dust[i];
						expr_220_cp_0.velocity.X = expr_220_cp_0.velocity.X * 0.3f;
						Main.dust[i].scale *= 0.7f;
					}
					if (Main.dust[i].type == 33)
					{
						Main.dust[i].alpha = 170;
						Dust expr_271 = Main.dust[i];
						expr_271.velocity *= 0.5f;
						Dust expr_292_cp_0 = Main.dust[i];
						expr_292_cp_0.velocity.Y = expr_292_cp_0.velocity.Y + 1f;
					}
					if (Main.dust[i].type == 41)
					{
						Dust expr_2BA = Main.dust[i];
						expr_2BA.velocity *= 0f;
					}
					if (Main.dust[i].type != 34 && Main.dust[i].type != 35)
					{
						break;
					}
					Dust expr_2F9 = Main.dust[i];
					expr_2F9.velocity *= 0.1f;
					Main.dust[i].velocity.Y = -0.5f;
					if (Main.dust[i].type == 34 && !Collision.WetCollision(new Vector2(Main.dust[i].position.X, Main.dust[i].position.Y - 8f), 4, 4))
					{
						Main.dust[i].active = false;
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			return result;
		}
		public static void UpdateDust()
		{
			for (int i = 0; i < 2000; i++)
			{
				if (Main.dust[i].active)
				{
					Dust expr_1F = Main.dust[i];
					expr_1F.position += Main.dust[i].velocity;
					if (Main.dust[i].type == 6 || Main.dust[i].type == 29)
					{
						if (!Main.dust[i].noGravity)
						{
							Dust expr_77_cp_0 = Main.dust[i];
							expr_77_cp_0.velocity.Y = expr_77_cp_0.velocity.Y + 0.05f;
						}
						if (!Main.dust[i].noLight)
						{
							float num = Main.dust[i].scale * 1.6f;
							if (Main.dust[i].type == 29)
							{
								num *= 0.3f;
							}
							if (num > 1f)
							{
								num = 1f;
							}
							//Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num);
						}
					}
					else
					{
						if (Main.dust[i].type == 14 || Main.dust[i].type == 16 || Main.dust[i].type == 31)
						{
							Dust expr_149_cp_0 = Main.dust[i];
							expr_149_cp_0.velocity.Y = expr_149_cp_0.velocity.Y * 0.98f;
							Dust expr_166_cp_0 = Main.dust[i];
							expr_166_cp_0.velocity.X = expr_166_cp_0.velocity.X * 0.98f;
						}
						else
						{
							if (Main.dust[i].type == 32)
							{
								Main.dust[i].scale -= 0.01f;
								Dust expr_1B0_cp_0 = Main.dust[i];
								expr_1B0_cp_0.velocity.X = expr_1B0_cp_0.velocity.X * 0.96f;
								Dust expr_1CD_cp_0 = Main.dust[i];
								expr_1CD_cp_0.velocity.Y = expr_1CD_cp_0.velocity.Y + 0.1f;
							}
							else
							{
								if (Main.dust[i].type == 15)
								{
									Dust expr_202_cp_0 = Main.dust[i];
									expr_202_cp_0.velocity.Y = expr_202_cp_0.velocity.Y * 0.98f;
									Dust expr_21F_cp_0 = Main.dust[i];
									expr_21F_cp_0.velocity.X = expr_21F_cp_0.velocity.X * 0.98f;
									float num2 = Main.dust[i].scale;
									if (num2 > 1f)
									{
										num2 = 1f;
									}
									//Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num2);
								}
								else
								{
									if (Main.dust[i].type == 20 || Main.dust[i].type == 21)
									{
										Main.dust[i].scale += 0.005f;
										Dust expr_2CD_cp_0 = Main.dust[i];
										expr_2CD_cp_0.velocity.Y = expr_2CD_cp_0.velocity.Y * 0.94f;
										Dust expr_2EA_cp_0 = Main.dust[i];
										expr_2EA_cp_0.velocity.X = expr_2EA_cp_0.velocity.X * 0.94f;
										float num3 = Main.dust[i].scale * 0.8f;
										if (Main.dust[i].type == 21)
										{
											num3 = Main.dust[i].scale * 0.4f;
										}
										if (num3 > 1f)
										{
											num3 = 1f;
										}
										//Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num3);
									}
									else
									{
										if (Main.dust[i].type == 27)
										{
											Dust expr_394 = Main.dust[i];
											expr_394.velocity *= 0.94f;
											Main.dust[i].scale += 0.002f;
											float num4 = Main.dust[i].scale;
											if (num4 > 1f)
											{
												num4 = 1f;
											}
											//Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num4);
										}
										else
										{
											if (!Main.dust[i].noGravity && Main.dust[i].type != 41)
											{
												Dust expr_442_cp_0 = Main.dust[i];
												expr_442_cp_0.velocity.Y = expr_442_cp_0.velocity.Y + 0.1f;
											}
										}
									}
								}
							}
						}
					}
					if (Main.dust[i].type == 5 && Main.dust[i].noGravity)
					{
						Main.dust[i].scale -= 0.04f;
					}
					if (Main.dust[i].type == 33)
					{
						bool flag = Collision.WetCollision(new Vector2(Main.dust[i].position.X, Main.dust[i].position.Y), 4, 4);
						if (flag)
						{
							Main.dust[i].alpha += 20;
							Main.dust[i].scale -= 0.1f;
						}
						Main.dust[i].alpha += 2;
						Main.dust[i].scale -= 0.005f;
						if (Main.dust[i].alpha > 255)
						{
							Main.dust[i].scale = 0f;
						}
						Dust expr_558_cp_0 = Main.dust[i];
						expr_558_cp_0.velocity.X = expr_558_cp_0.velocity.X * 0.93f;
						if (Main.dust[i].velocity.Y > 4f)
						{
							Main.dust[i].velocity.Y = 4f;
						}
						if (Main.dust[i].noGravity)
						{
							if (Main.dust[i].velocity.X < 0f)
							{
								Main.dust[i].rotation -= 0.2f;
							}
							else
							{
								Main.dust[i].rotation += 0.2f;
							}
							Main.dust[i].scale += 0.03f;
							Dust expr_616_cp_0 = Main.dust[i];
							expr_616_cp_0.velocity.X = expr_616_cp_0.velocity.X * 1.05f;
							Dust expr_633_cp_0 = Main.dust[i];
							expr_633_cp_0.velocity.Y = expr_633_cp_0.velocity.Y + 0.15f;
						}
					}
					if (Main.dust[i].type == 35 && Main.dust[i].noGravity)
					{
						Main.dust[i].scale += 0.02f;
						if (Main.dust[i].scale < 1f)
						{
							Dust expr_69F_cp_0 = Main.dust[i];
							expr_69F_cp_0.velocity.Y = expr_69F_cp_0.velocity.Y + 0.075f;
						}
						Dust expr_6BC_cp_0 = Main.dust[i];
						expr_6BC_cp_0.velocity.X = expr_6BC_cp_0.velocity.X * 1.08f;
						if (Main.dust[i].velocity.X > 0f)
						{
							Main.dust[i].rotation += 0.01f;
						}
						else
						{
							Main.dust[i].rotation -= 0.01f;
						}
					}
					else
					{
						if (Main.dust[i].type == 34 || Main.dust[i].type == 35)
						{
							if (!Collision.WetCollision(new Vector2(Main.dust[i].position.X, Main.dust[i].position.Y - 8f), 4, 4))
							{
								Main.dust[i].scale = 0f;
							}
							else
							{
								Main.dust[i].alpha += Main.rand.Next(2);
								if (Main.dust[i].alpha > 255)
								{
									Main.dust[i].scale = 0f;
								}
								Main.dust[i].velocity.Y = -0.5f;
								if (Main.dust[i].type == 34)
								{
									Main.dust[i].scale += 0.005f;
								}
								else
								{
									Main.dust[i].alpha++;
									Main.dust[i].scale -= 0.01f;
									Main.dust[i].velocity.Y = -0.2f;
								}
								Dust expr_862_cp_0 = Main.dust[i];
								expr_862_cp_0.velocity.X = expr_862_cp_0.velocity.X + (float)Main.rand.Next(-10, 10) * 0.002f;
								if ((double)Main.dust[i].velocity.X < -0.25)
								{
									Main.dust[i].velocity.X = -0.25f;
								}
								if ((double)Main.dust[i].velocity.X > 0.25)
								{
									Main.dust[i].velocity.X = 0.25f;
								}
							}
							if (Main.dust[i].type == 35)
							{
								float num5 = Main.dust[i].scale * 1.6f;
								if (num5 > 1f)
								{
									num5 = 1f;
								}
								//Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num5);
							}
						}
					}
					if (Main.dust[i].type == 41)
					{
						Dust expr_973_cp_0 = Main.dust[i];
						expr_973_cp_0.velocity.X = expr_973_cp_0.velocity.X + (float)Main.rand.Next(-10, 11) * 0.01f;
						Dust expr_9A0_cp_0 = Main.dust[i];
						expr_9A0_cp_0.velocity.Y = expr_9A0_cp_0.velocity.Y + (float)Main.rand.Next(-10, 11) * 0.01f;
						if ((double)Main.dust[i].velocity.X > 0.75)
						{
							Main.dust[i].velocity.X = 0.75f;
						}
						if ((double)Main.dust[i].velocity.X < -0.75)
						{
							Main.dust[i].velocity.X = -0.75f;
						}
						if ((double)Main.dust[i].velocity.Y > 0.75)
						{
							Main.dust[i].velocity.Y = 0.75f;
						}
						if ((double)Main.dust[i].velocity.Y < -0.75)
						{
							Main.dust[i].velocity.Y = -0.75f;
						}
						Main.dust[i].scale += 0.007f;
						float num6 = Main.dust[i].scale * 0.7f;
						if (num6 > 1f)
						{
							num6 = 1f;
						}
						//Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num6);
					}
					else
					{
						Dust expr_B0E_cp_0 = Main.dust[i];
						expr_B0E_cp_0.velocity.X = expr_B0E_cp_0.velocity.X * 0.99f;
					}
					Main.dust[i].rotation += Main.dust[i].velocity.X * 0.5f;
					Main.dust[i].scale -= 0.01f;
					if (Main.dust[i].noGravity)
					{
						Dust expr_B76 = Main.dust[i];
						expr_B76.velocity *= 0.92f;
						Main.dust[i].scale -= 0.04f;
					}
					if (Main.dust[i].position.Y > Main.screenPosition.Y + (float)Main.screenHeight)
					{
						Main.dust[i].active = false;
					}
					if ((double)Main.dust[i].scale < 0.1)
					{
						Main.dust[i].active = false;
					}
				}
			}
		}
		public Color GetAlpha(Color newColor)
		{
			int r;
			int g;
			int b;
			if (this.type == 15 || this.type == 20 || this.type == 21 || this.type == 29 || this.type == 35 || this.type == 41)
			{
				r = (int)newColor.R - this.alpha / 3;
				g = (int)newColor.G - this.alpha / 3;
				b = (int)newColor.B - this.alpha / 3;
			}
			else
			{
				r = (int)newColor.R - this.alpha;
				g = (int)newColor.G - this.alpha;
				b = (int)newColor.B - this.alpha;
			}
			int num = (int)newColor.A - this.alpha;
			if (num < 0)
			{
				num = 0;
			}
			if (num > 255)
			{
				num = 255;
			}
			return new Color(r, g, b, num);
		}
		public Color GetColor(Color newColor)
		{
			int num = (int)(this.color.R - (255 - newColor.R));
			int num2 = (int)(this.color.G - (255 - newColor.G));
			int num3 = (int)(this.color.B - (255 - newColor.B));
			int num4 = (int)(this.color.A - (255 - newColor.A));
			if (num < 0)
			{
				num = 0;
			}
			if (num > 255)
			{
				num = 255;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > 255)
			{
				num2 = 255;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num3 > 255)
			{
				num3 = 255;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			if (num4 > 255)
			{
				num4 = 255;
			}
			return new Color(num, num2, num3, num4);
		}
	}
}
