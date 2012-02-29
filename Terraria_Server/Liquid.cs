using System;
using Terraria_Server.Logging;
using Terraria_Server.WorldMod;
using Terraria_Server.Language;

namespace Terraria_Server
{
	public struct Liquid //let's hope the change to struct doesn't break anything
	{
		public static int skipCount = 0;
		public static int stuckCount = 0;
		public static int stuckAmount = 0;
		public static int cycles = 10;
		public static int resLiquid = 5000;
		public static int maxLiquid = 5000;
		public static int numLiquid;
		public static bool stuck = false;
		public static bool quickFall = false;
		public static bool quickSettle = false;
		public static int panicCounter = 0;
		public static bool panicMode = false;
		public static int panicY = 0;
		private static int wetCounter;

		public int x;
		public int y;
		public int kill;
		public int delay;

		public static double QuickWater(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int verbose = 0, int minY = -1, int maxY = -1, ProgressLogger prog = null)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int num = 0;
			if (minY == -1)
			{
				minY = 3;
			}
			if (maxY == -1)
			{
				maxY = Main.maxTilesY - 3;
			}
			for (int i = maxY; i >= minY; i--)
			{
				if (verbose > 0)
				{
					float num2 = (float)(maxY - i) / (float)(maxY - minY + 1);
					num2 /= (float)verbose;

					if (prog != null)
						prog.Value += (int)(num2 * 100f + 1f);
				}
				else if (verbose < 0)
				{
					float num3 = (float)(maxY - i) / (float)(maxY - minY + 1);
					num3 /= (float)(-(float)verbose);

					if (prog != null)
						prog.Value += (int)(num3 * 100f + 1f);
				}

				for (int j = 0; j < 2; j++)
				{
					int num4 = 2;
					int num5 = Main.maxTilesX - 2;
					int num6 = 1;
					if (j == 1)
					{
						num4 = Main.maxTilesX - 2;
						num5 = 2;
						num6 = -1;
					}
					for (int num7 = num4; num7 != num5; num7 += num6)
					{
						if (TileRefs(num7, i).Liquid > 0)
						{
							int num8 = -num6;
							bool flag = false;
							int num9 = num7;
							int num10 = i;

							bool flag2 = TileRefs(num7, i).Lava;
							byte b = TileRefs(num7, i).Liquid;
							TileRefs(num7, i).SetLiquid(0);

							bool flag3 = true;
							int num11 = 0;
							while (flag3 && num9 > 3 && num9 < Main.maxTilesX - 3 && num10 < Main.maxTilesY - 3)
							{
								flag3 = false;
								while (TileRefs(num9, num10 + 1).Liquid == 0 && num10 < Main.maxTilesY - 5 && (!TileRefs(num9, num10 + 1).Active || !Main.tileSolid[(int)TileRefs(num9, num10 + 1).Type] || Main.tileSolidTop[(int)TileRefs(num9, num10 + 1).Type]))
								{
									flag = true;
									num8 = num6;
									num11 = 0;
									flag3 = true;
									num10++;
									if (num10 > WorldModify.waterLine)
									{
										flag2 = true;
									}
								}
								if (TileRefs(num9, num10 + 1).Liquid > 0 && TileRefs(num9, num10 + 1).Liquid < 255 && TileRefs(num9, num10 + 1).Lava == flag2)
								{
									int num12 = (int)(255 - TileRefs(num9, num10 + 1).Liquid);
									if (num12 > (int)b)
									{
										num12 = (int)b;
									}

									TileRefs(num9, num10 + 1).AddLiquid((byte)num12);

									b -= (byte)num12;
									if (b <= 0)
									{
										num++;
										break;
									}
								}
								if (num11 == 0)
								{
									if (TileRefs(num9 + num8, num10).Liquid == 0 && (!TileRefs(num9 + num8, num10).Active || !Main.tileSolid[(int)TileRefs(num9 + num8, num10).Type] || Main.tileSolidTop[(int)TileRefs(num9 + num8, num10).Type]))
									{
										num11 = num8;
									}
									else if (TileRefs(num9 - num8, num10).Liquid == 0 && (!TileRefs(num9 - num8, num10).Active || !Main.tileSolid[(int)TileRefs(num9 - num8, num10).Type] || Main.tileSolidTop[(int)TileRefs(num9 - num8, num10).Type]))
									{
										num11 = -num8;
									}
								}
								if (num11 != 0 && TileRefs(num9 + num11, num10).Liquid == 0 && (!TileRefs(num9 + num11, num10).Active || !Main.tileSolid[(int)TileRefs(num9 + num11, num10).Type] || Main.tileSolidTop[(int)TileRefs(num9 + num11, num10).Type]))
								{
									flag3 = true;
									num9 += num11;
								}
								if (flag && !flag3)
								{
									flag = false;
									flag3 = true;
									num8 = -num6;
									num11 = 0;
								}
							}
							if (num7 != num9 && i != num10)
							{
								num++;
							}

							TileRefs(num9, num10).SetLiquid(b);
							TileRefs(num9, num10).SetLava(flag2);

							if (TileRefs(num9 - 1, num10).Liquid > 0 && TileRefs(num9 - 1, num10).Lava != flag2)
							{
								if (flag2)
								{
									Liquid.LavaCheck(TileRefs, sandbox, num9, num10);
								}
								else
								{
									Liquid.LavaCheck(TileRefs, sandbox, num9 - 1, num10);
								}
							}
							else if (TileRefs(num9 + 1, num10).Liquid > 0 && TileRefs(num9 + 1, num10).Lava != flag2)
							{
								if (flag2)
								{
									Liquid.LavaCheck(TileRefs, sandbox, num9, num10);
								}
								else
								{
									Liquid.LavaCheck(TileRefs, sandbox, num9 + 1, num10);
								}
							}
							else if (TileRefs(num9, num10 - 1).Liquid > 0 && TileRefs(num9, num10 - 1).Lava != flag2)
							{
								if (flag2)
								{
									Liquid.LavaCheck(TileRefs, sandbox, num9, num10);
								}
								else
								{
									Liquid.LavaCheck(TileRefs, sandbox, num9, num10 - 1);
								}
							}
							else if (TileRefs(num9, num10 + 1).Liquid > 0 && TileRefs(num9, num10 + 1).Lava != flag2)
							{
								if (flag2)
								{
									Liquid.LavaCheck(TileRefs, sandbox, num9, num10);
								}
								else
								{
									Liquid.LavaCheck(TileRefs, sandbox, num9, num10 + 1);
								}
							}
						}
					}
				}
			}

			return (double)num;
		}

		public void Update(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (TileRefs(this.x, this.y).Active && Main.tileSolid[(int)TileRefs(this.x, this.y).Type] && !Main.tileSolidTop[(int)TileRefs(this.x, this.y).Type])
			{
				this.kill = 9;
				return;
			}

			byte liquid = TileRefs(this.x, this.y).Liquid;
			float num = 0f;
			if (this.y > Main.maxTilesY - 200 && !TileRefs(this.x, this.y).Lava && TileRefs(this.x, this.y).Liquid > 0)
			{
				byte b = 2;
				if (TileRefs(this.x, this.y).Liquid < b)
					b = TileRefs(this.x, this.y).Liquid;

				TileRefs(this.x, this.y).AddLiquid(-b);
			}

			if (TileRefs(this.x, this.y).Liquid == 0)
			{
				this.kill = 9;
				return;
			}

			if (TileRefs(this.x, this.y).Lava)
			{
				Liquid.LavaCheck(TileRefs, sandbox, this.x, this.y);
				if (!Liquid.quickFall)
				{
					if (this.delay < 5)
					{
						this.delay++;
						return;
					}
					this.delay = 0;
				}
			}
			else
			{
				if (TileRefs(this.x - 1, this.y).Lava)
					Liquid.AddWater(TileRefs, sandbox, this.x - 1, this.y);

				if (TileRefs(this.x + 1, this.y).Lava)
					Liquid.AddWater(TileRefs, sandbox, this.x + 1, this.y);

				if (TileRefs(this.x, this.y - 1).Lava)
					Liquid.AddWater(TileRefs, sandbox, this.x, this.y - 1);

				if (TileRefs(this.x, this.y + 1).Lava)
					Liquid.AddWater(TileRefs, sandbox, this.x, this.y + 1);
			}
			if ((!TileRefs(this.x, this.y + 1).Active || !Main.tileSolid[(int)TileRefs(this.x, this.y + 1).Type] || Main.tileSolidTop[(int)TileRefs(this.x, this.y + 1).Type]) && (TileRefs(this.x, this.y + 1).Liquid <= 0 || TileRefs(this.x, this.y + 1).Lava == TileRefs(this.x, this.y).Lava) && TileRefs(this.x, this.y + 1).Liquid < 255)
			{
				num = (float)(255 - TileRefs(this.x, this.y + 1).Liquid);
				if (num > (float)TileRefs(this.x, this.y).Liquid)
					num = (float)TileRefs(this.x, this.y).Liquid;

				TileRefs(this.x, this.y).AddLiquid(-(byte)num);
				TileRefs(this.x, this.y + 1).AddLiquid((byte)num);

				TileRefs(this.x, this.y + 1).SetLava(TileRefs(this.x, this.y).Lava);

				Liquid.AddWater(TileRefs, sandbox, this.x, this.y + 1);

				TileRefs(this.x, this.y + 1).SetSkipLiquid(true);
				TileRefs(this.x, this.y).SetSkipLiquid(true);

				if (TileRefs(this.x, this.y).Liquid > 250)
					TileRefs(this.x, this.y).SetLiquid(255);
				else
				{
					Liquid.AddWater(TileRefs, sandbox, this.x - 1, this.y);
					Liquid.AddWater(TileRefs, sandbox, this.x + 1, this.y);
				}
			}
			if (TileRefs(this.x, this.y).Liquid > 0)
			{
				bool flag = true;
				bool flag2 = true;
				bool flag3 = true;
				bool flag4 = true;
				if (TileRefs(this.x - 1, this.y).Active && Main.tileSolid[(int)TileRefs(this.x - 1, this.y).Type] && !Main.tileSolidTop[(int)TileRefs(this.x - 1, this.y).Type])
				{
					flag = false;
				}
				else if (TileRefs(this.x - 1, this.y).Liquid > 0 && TileRefs(this.x - 1, this.y).Lava != TileRefs(this.x, this.y).Lava)
				{
					flag = false;
				}
				else if (TileRefs(this.x - 2, this.y).Active && Main.tileSolid[(int)TileRefs(this.x - 2, this.y).Type] && !Main.tileSolidTop[(int)TileRefs(this.x - 2, this.y).Type])
				{
					flag3 = false;
				}
				else if (TileRefs(this.x - 2, this.y).Liquid == 0)
				{
					flag3 = false;
				}
				else if (TileRefs(this.x - 2, this.y).Liquid > 0 && TileRefs(this.x - 2, this.y).Lava != TileRefs(this.x, this.y).Lava)
				{
					flag3 = false;
				}

				if (TileRefs(this.x + 1, this.y).Active && Main.tileSolid[(int)TileRefs(this.x + 1, this.y).Type] && !Main.tileSolidTop[(int)TileRefs(this.x + 1, this.y).Type])
				{
					flag2 = false;
				}
				else if (TileRefs(this.x + 1, this.y).Liquid > 0 && TileRefs(this.x + 1, this.y).Lava != TileRefs(this.x, this.y).Lava)
				{
					flag2 = false;
				}
				else if (TileRefs(this.x + 2, this.y).Active && Main.tileSolid[(int)TileRefs(this.x + 2, this.y).Type] && !Main.tileSolidTop[(int)TileRefs(this.x + 2, this.y).Type])
				{
					flag4 = false;
				}
				else if (TileRefs(this.x + 2, this.y).Liquid == 0)
				{
					flag4 = false;
				}
				else if (TileRefs(this.x + 2, this.y).Liquid > 0 && TileRefs(this.x + 2, this.y).Lava != TileRefs(this.x, this.y).Lava)
				{
					flag4 = false;
				}

				int num2 = 0;
				if (TileRefs(this.x, this.y).Liquid < 3)
				{
					num2 = -1;
				}
				if (flag && flag2)
				{
					if (flag3 && flag4)
					{
						bool flag5 = true;
						bool flag6 = true;
						if (TileRefs(this.x - 3, this.y).Active && Main.tileSolid[(int)TileRefs(this.x - 3, this.y).Type] && !Main.tileSolidTop[(int)TileRefs(this.x - 3, this.y).Type])
						{
							flag5 = false;
						}
						else if (TileRefs(this.x - 3, this.y).Liquid == 0)
						{
							flag5 = false;
						}
						else if (TileRefs(this.x - 3, this.y).Lava != TileRefs(this.x, this.y).Lava)
						{
							flag5 = false;
						}

						if (TileRefs(this.x + 3, this.y).Active && Main.tileSolid[(int)TileRefs(this.x + 3, this.y).Type] && !Main.tileSolidTop[(int)TileRefs(this.x + 3, this.y).Type])
						{
							flag6 = false;
						}
						else if (TileRefs(this.x + 3, this.y).Liquid == 0)
						{
							flag6 = false;
						}
						else if (TileRefs(this.x + 3, this.y).Lava != TileRefs(this.x, this.y).Lava)
						{
							flag6 = false;
						}

						if (flag5 && flag6)
						{
							num = (float)((int)(TileRefs(this.x - 1, this.y).Liquid + TileRefs(this.x + 1, this.y).Liquid + TileRefs(this.x - 2, this.y).Liquid + TileRefs(this.x + 2, this.y).Liquid + TileRefs(this.x - 3, this.y).Liquid + TileRefs(this.x + 3, this.y).Liquid + TileRefs(this.x, this.y).Liquid) + num2);
							num = (float)Math.Round((double)(num / 7f));
							int num3 = 0;

							TileRefs(this.x - 1, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x - 1, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x - 1, this.y);
								TileRefs(this.x - 1, this.y).SetLiquid((byte)num);
							}
							else
							{
								num3++;
							}

							TileRefs(this.x + 1, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x + 1, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x + 1, this.y);
								TileRefs(this.x + 1, this.y).SetLiquid((byte)num);
							}
							else
							{
								num3++;
							}

							TileRefs(this.x - 2, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x - 2, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x - 2, this.y);
								TileRefs(this.x - 2, this.y).SetLiquid((byte)num);
							}
							else
							{
								num3++;
							}

							TileRefs(this.x + 2, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x + 2, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x + 2, this.y);
								TileRefs(this.x + 2, this.y).SetLiquid((byte)num);
							}
							else
							{
								num3++;
							}

							TileRefs(this.x - 3, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x - 3, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x - 3, this.y);
								TileRefs(this.x - 3, this.y).SetLiquid((byte)num);
							}
							else
							{
								num3++;
							}

							TileRefs(this.x + 3, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x + 3, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x + 3, this.y);
								TileRefs(this.x + 3, this.y).SetLiquid((byte)num);
							}
							else
							{
								num3++;
							}

							if (TileRefs(this.x - 1, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x - 1, this.y);
							}
							if (TileRefs(this.x + 1, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x + 1, this.y);
							}
							if (TileRefs(this.x - 2, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x - 2, this.y);
							}
							if (TileRefs(this.x + 2, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x + 2, this.y);
							}
							if (TileRefs(this.x - 3, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x - 3, this.y);
							}
							if (TileRefs(this.x + 3, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x + 3, this.y);
							}
							if (num3 != 6 || TileRefs(this.x, this.y - 1).Liquid <= 0)
							{
								TileRefs(this.x, this.y).SetLiquid((byte)num);
							}
						}
						else
						{
							int num4 = 0;
							num = (float)((int)(TileRefs(this.x - 1, this.y).Liquid + TileRefs(this.x + 1, this.y).Liquid + TileRefs(this.x - 2, this.y).Liquid + TileRefs(this.x + 2, this.y).Liquid + TileRefs(this.x, this.y).Liquid) + num2);
							num = (float)Math.Round((double)(num / 5f));

							TileRefs(this.x - 1, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x - 1, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x - 1, this.y);
								TileRefs(this.x - 1, this.y).SetLiquid((byte)num);
							}
							else
							{
								num4++;
							}

							TileRefs(this.x + 1, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x + 1, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x + 1, this.y);
								TileRefs(this.x + 1, this.y).SetLiquid((byte)num);
							}
							else
							{
								num4++;
							}

							TileRefs(this.x - 2, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x - 2, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x - 2, this.y);
								TileRefs(this.x - 2, this.y).SetLiquid((byte)num);
							}
							else
							{
								num4++;
							}

							TileRefs(this.x + 2, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x + 2, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x + 2, this.y);
								TileRefs(this.x + 2, this.y).SetLiquid((byte)num);
							}
							else
							{
								num4++;
							}

							if (TileRefs(this.x - 1, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x - 1, this.y);
							}
							if (TileRefs(this.x + 1, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x + 1, this.y);
							}
							if (TileRefs(this.x - 2, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x - 2, this.y);
							}
							if (TileRefs(this.x + 2, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x + 2, this.y);
							}
							if (num4 != 4 || TileRefs(this.x, this.y - 1).Liquid <= 0)
							{
								TileRefs(this.x, this.y).SetLiquid((byte)num);
							}
						}
					}
					else
					{
						if (flag3)
						{
							num = (float)((int)(TileRefs(this.x - 1, this.y).Liquid + TileRefs(this.x + 1, this.y).Liquid + TileRefs(this.x - 2, this.y).Liquid + TileRefs(this.x, this.y).Liquid) + num2);
							num = (float)Math.Round((double)(num / 4f) + 0.001);

							TileRefs(this.x - 1, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x - 1, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x - 1, this.y);
								TileRefs(this.x - 1, this.y).SetLiquid((byte)num);
							}

							TileRefs(this.x + 1, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x + 1, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x + 1, this.y);
								TileRefs(this.x + 1, this.y).SetLiquid((byte)num);
							}

							TileRefs(this.x - 2, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x - 2, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
							{
								TileRefs(this.x - 2, this.y).SetLiquid((byte)num);
								Liquid.AddWater(TileRefs, sandbox, this.x - 2, this.y);
							}

							TileRefs(this.x, this.y).SetLiquid((byte)num);
						}
						else
						{
							if (flag4)
							{
								num = (float)((int)(TileRefs(this.x - 1, this.y).Liquid + TileRefs(this.x + 1, this.y).Liquid + TileRefs(this.x + 2, this.y).Liquid + TileRefs(this.x, this.y).Liquid) + num2);
								num = (float)Math.Round((double)(num / 4f) + 0.001);

								TileRefs(this.x - 1, this.y).SetLava(TileRefs(this.x, this.y).Lava);

								if (TileRefs(this.x - 1, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
								{
									Liquid.AddWater(TileRefs, sandbox, this.x - 1, this.y);
									TileRefs(this.x - 1, this.y).SetLiquid((byte)num);
								}

								TileRefs(this.x + 1, this.y).SetLava(TileRefs(this.x, this.y).Lava);

								if (TileRefs(this.x + 1, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
								{
									Liquid.AddWater(TileRefs, sandbox, this.x + 1, this.y);
									TileRefs(this.x + 1, this.y).SetLiquid((byte)num);
								}

								TileRefs(this.x + 2, this.y).SetLava(TileRefs(this.x, this.y).Lava);

								if (TileRefs(this.x + 2, this.y).Liquid != (byte)num || TileRefs(this.x, this.y).Liquid != (byte)num)
								{
									TileRefs(this.x + 2, this.y).SetLiquid((byte)num);
									Liquid.AddWater(TileRefs, sandbox, this.x + 2, this.y);
								}

								TileRefs(this.x, this.y).SetLiquid((byte)num);
							}
							else
							{
								num = (float)((int)(TileRefs(this.x - 1, this.y).Liquid + TileRefs(this.x + 1, this.y).Liquid + TileRefs(this.x, this.y).Liquid) + num2);
								num = (float)Math.Round((double)(num / 3f) + 0.001);

								TileRefs(this.x - 1, this.y).SetLava(TileRefs(this.x, this.y).Lava);

								if (TileRefs(this.x - 1, this.y).Liquid != (byte)num)
								{
									TileRefs(this.x - 1, this.y).SetLiquid((byte)num);
								}
								if (TileRefs(this.x, this.y).Liquid != (byte)num || TileRefs(this.x - 1, this.y).Liquid != (byte)num)
								{
									Liquid.AddWater(TileRefs, sandbox, this.x - 1, this.y);
								}

								TileRefs(this.x + 1, this.y).SetLava(TileRefs(this.x, this.y).Lava);

								if (TileRefs(this.x + 1, this.y).Liquid != (byte)num)
								{
									TileRefs(this.x + 1, this.y).SetLiquid((byte)num);
								}
								if (TileRefs(this.x, this.y).Liquid != (byte)num || TileRefs(this.x + 1, this.y).Liquid != (byte)num)
								{
									Liquid.AddWater(TileRefs, sandbox, this.x + 1, this.y);
								}

								TileRefs(this.x, this.y).SetLiquid((byte)num);
							}
						}
					}
				}
				else
				{
					if (flag)
					{
						num = (float)((int)(TileRefs(this.x - 1, this.y).Liquid + TileRefs(this.x, this.y).Liquid) + num2);
						num = (float)Math.Round((double)(num / 2f) + 0.001);
						if (TileRefs(this.x - 1, this.y).Liquid != (byte)num)
						{
							TileRefs(this.x - 1, this.y).SetLiquid((byte)num);
						}

						TileRefs(this.x - 1, this.y).SetLava(TileRefs(this.x, this.y).Lava);

						if (TileRefs(this.x, this.y).Liquid != (byte)num || TileRefs(this.x - 1, this.y).Liquid != (byte)num)
						{
							Liquid.AddWater(TileRefs, sandbox, this.x - 1, this.y);
						}

						TileRefs(this.x, this.y).SetLiquid((byte)num);
					}
					else
					{
						if (flag2)
						{
							num = (float)((int)(TileRefs(this.x + 1, this.y).Liquid + TileRefs(this.x, this.y).Liquid) + num2);
							num = (float)Math.Round((double)(num / 2f) + 0.001);
							if (TileRefs(this.x + 1, this.y).Liquid != (byte)num)
							{
								TileRefs(this.x + 1, this.y).SetLiquid((byte)num);
							}

							TileRefs(this.x + 1, this.y).SetLava(TileRefs(this.x, this.y).Lava);

							if (TileRefs(this.x, this.y).Liquid != (byte)num || TileRefs(this.x + 1, this.y).Liquid != (byte)num)
							{
								Liquid.AddWater(TileRefs, sandbox, this.x + 1, this.y);
							}

							TileRefs(this.x, this.y).SetLiquid((byte)num);
						}
					}
				}
			}
			if (TileRefs(this.x, this.y).Liquid == liquid)
			{
				this.kill++;
				return;
			}
			if (TileRefs(this.x, this.y).Liquid == 254 && liquid == 255)
			{
				TileRefs(this.x, this.y).SetLiquid(255);
				this.kill++;
				return;
			}
			Liquid.AddWater(TileRefs, sandbox, this.x, this.y - 1);
			this.kill = 0;
		}

		public static void StartPanic()
		{
			if (!Liquid.panicMode)
			{
				WorldModify.waterLine = Main.maxTilesY;
				Liquid.numLiquid = 0;
				LiquidBuffer.numLiquidBuffer = 0;
				Liquid.panicCounter = 0;
				Liquid.panicMode = true;
				Liquid.panicY = Main.maxTilesY - 3;

				ProgramLog.Log("Forcing water to settle.");
			}
		}

		public static void UpdateLiquid(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			//{
			    //Liquid.cycles = 30;
			//    Liquid.maxLiquid = 6000;
			//}

			if (!WorldModify.gen)
			{
				if (!Liquid.panicMode)
				{
					if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > 4000)
					{
						Liquid.panicCounter++;
						if (Liquid.panicCounter > 1800 || Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > 13500)
							Liquid.StartPanic();
					}
					else
						Liquid.panicCounter = 0;
				}

				if (Liquid.panicMode)
				{
					int num = 0;
					while (Liquid.panicY >= 3 && num < 5)
					{
						num++;
						Liquid.QuickWater(TileRefs, sandbox, 0, Liquid.panicY, Liquid.panicY);
						Liquid.panicY--;
						if (Liquid.panicY < 3)
						{
							ProgramLog.Log(Languages.Water_WaterHasBeenSettled);
							Liquid.panicCounter = 0;
							Liquid.panicMode = false;

							using (var prog = new ProgressLogger(Main.maxTilesX - 2, Languages.Water_PerformingWaterCheck))
								WorldModify.WaterCheck(TileRefs, sandbox, prog);

							for (int i = 0; i < 255; i++)
							{
								for (int j = 0; j < Main.maxSectionsX; j++)
								{
									for (int k = 0; k < Main.maxSectionsY; k++)
										NetPlay.slots[i].tileSection[j, k] = false;
								}
							}
						}
					}
					return;
				}
			}

			Liquid.quickFall = Liquid.quickSettle || Liquid.numLiquid > 2000;
			Liquid.wetCounter++;

			int num2 = Liquid.maxLiquid / Liquid.cycles;
			int num3 = num2 * (Liquid.wetCounter - 1);
			int num4 = num2 * Liquid.wetCounter;

			if (Liquid.wetCounter == Liquid.cycles)
				num4 = Liquid.numLiquid;

			if (num4 > Liquid.numLiquid)
			{
				num4 = Liquid.numLiquid;
				Liquid.wetCounter = Liquid.cycles;
			}

			if (Liquid.quickFall)
			{
				for (int l = num3; l < num4; l++)
				{
					Main.liquid[l].delay = 10;
					Main.liquid[l].Update(TileRefs, sandbox);
					TileRefs(Main.liquid[l].x, Main.liquid[l].y).SetSkipLiquid(false);
				}
			}
			else
			{
				for (int m = num3; m < num4; m++)
				{
					if (!TileRefs(Main.liquid[m].x, Main.liquid[m].y).SkipLiquid)
						Main.liquid[m].Update(TileRefs, sandbox);
					else
						TileRefs(Main.liquid[m].x, Main.liquid[m].y).SetSkipLiquid(false);
				}
			}

			if (Liquid.wetCounter >= Liquid.cycles)
			{
				Liquid.wetCounter = 0;

				for (int n = Liquid.numLiquid - 1; n >= 0; n--)
				{
					if (Main.liquid[n].kill > 3)
						Liquid.DelWater(TileRefs, sandbox, n);
				}

				int num5 = Liquid.maxLiquid - (Liquid.maxLiquid - Liquid.numLiquid);
				if (num5 > LiquidBuffer.numLiquidBuffer)
					num5 = LiquidBuffer.numLiquidBuffer;

				for (int num6 = 0; num6 < num5; num6++)
				{
					TileRefs(Main.liquidBuffer[0].x, Main.liquidBuffer[0].y).SetCheckingLiquid(false);
					Liquid.AddWater(TileRefs, sandbox, Main.liquidBuffer[0].x, Main.liquidBuffer[0].y);
					LiquidBuffer.DelBuffer(0);
				}

				if (Liquid.numLiquid > 0 && Liquid.numLiquid > Liquid.stuckAmount - 50 && Liquid.numLiquid < Liquid.stuckAmount + 50)
				{
					Liquid.stuckCount++;
					if (Liquid.stuckCount >= 10000)
					{
						Liquid.stuck = true;

						for (int num7 = Liquid.numLiquid - 1; num7 >= 0; num7--)
							Liquid.DelWater(TileRefs, sandbox, num7);

						Liquid.stuck = false;
						Liquid.stuckCount = 0;
						return;
					}
				}
				else
				{
					Liquid.stuckCount = 0;
					Liquid.stuckAmount = Liquid.numLiquid;
				}
			}
		}

		public static void AddWater(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (TileRefs(x, y).CheckingLiquid || (x >= Main.maxTilesX - 5 || y >= Main.maxTilesY - 5) || (x < 5 || y < 5) || TileRefs(x, y).Liquid == 0)
				return;

			if (Liquid.numLiquid >= Liquid.maxLiquid - 1)
			{
				LiquidBuffer.AddBuffer(TileRefs, x, y);
				return;
			}

			TileRefs(x, y).SetCheckingLiquid(true);
			Main.liquid[Liquid.numLiquid].kill = 0;
			Main.liquid[Liquid.numLiquid].x = x;
			Main.liquid[Liquid.numLiquid].y = y;
			Main.liquid[Liquid.numLiquid].delay = 0;
			TileRefs(x, y).SetSkipLiquid(false);

			Liquid.numLiquid++;

			if (Liquid.numLiquid < Liquid.maxLiquid / 3)
			{
				NetMessage.SendWater(x, y);
			}
			if (TileRefs(x, y).Active && (Main.tileWaterDeath[(int)TileRefs(x, y).Type] || (TileRefs(x, y).Lava && Main.tileLavaDeath[(int)TileRefs(x, y).Type])))
			{
				if (TileRefs(x, y).Type == 4 && TileRefs(x, y).FrameY == 176)
					return;

				if (WorldModify.gen)
				{
					TileRefs(x, y).SetActive(false);
					return;
				}
				WorldModify.KillTile(TileRefs, sandbox, x, y);
				NetMessage.SendData(17, -1, -1, "", 0, (float)x, (float)y);
			}
		}

		public static void LavaCheck(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int x, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if ((TileRefs(x - 1, y).Liquid > 0 && !TileRefs(x - 1, y).Lava) || (TileRefs(x + 1, y).Liquid > 0 && !TileRefs(x + 1, y).Lava) || (TileRefs(x, y - 1).Liquid > 0 && !TileRefs(x, y - 1).Lava))
			{
				int liq = 0;
				if (!TileRefs(x - 1, y).Lava)
				{
					liq += (int)TileRefs(x - 1, y).Liquid;
					TileRefs(x - 1, y).SetLiquid(0);
				}
				if (!TileRefs(x + 1, y).Lava)
				{
					liq += (int)TileRefs(x + 1, y).Liquid;
					TileRefs(x + 1, y).SetLiquid(0);
				}
				if (!TileRefs(x, y - 1).Lava)
				{
					liq += (int)TileRefs(x, y - 1).Liquid;
					TileRefs(x, y - 1).SetLiquid(0);
				}
				if (liq >= 32 && !TileRefs(x, y).Active)
				{
					TileRefs(x, y).SetLiquid(0);
					TileRefs(x, y).SetLava(false);
					WorldModify.PlaceTile(TileRefs, sandbox, x, y, 56, true, true, -1, 0);
					WorldModify.SquareTileFrame(TileRefs, sandbox,x, y, true);

					NetMessage.SendTileSquare(-1, x - 1, y - 1, 3);
					return;
				}
			}
			else if (TileRefs(x, y + 1).Liquid > 0 && !TileRefs(x, y + 1).Lava && !TileRefs(x, y + 1).Active)
			{
				TileRefs(x, y).SetLiquid(0);
				TileRefs(x, y).SetLava(false);
				TileRefs(x, y + 1).SetLiquid(0);
				WorldModify.PlaceTile(TileRefs, sandbox, x, y + 1, 56, true, true, -1, 0);
				WorldModify.SquareTileFrame(TileRefs, sandbox,x, y + 1, true);
				NetMessage.SendTileSquare(-1, x - 1, y, 3);
			}
		}

		public static void DelWater(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int id)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int x = Main.liquid[id].x;
			int y = Main.liquid[id].y;
			if (TileRefs(x, y).Liquid < 2)
			{
				TileRefs(x, y).SetLiquid(0);

				if (TileRefs(x - 1, y).Liquid < 2)
					TileRefs(x - 1, y).SetLiquid(0);

				if (TileRefs(x + 1, y).Liquid < 2)
					TileRefs(x + 1, y).SetLiquid(0);
			}
			else if (TileRefs(x, y).Liquid < 20)
			{
				if ((TileRefs(x - 1, y).Liquid < TileRefs(x, y).Liquid && (!TileRefs(x - 1, y).Active || !Main.tileSolid[(int)TileRefs(x - 1, y).Type] || Main.tileSolidTop[(int)TileRefs(x - 1, y).Type])) || (TileRefs(x + 1, y).Liquid < TileRefs(x, y).Liquid && (!TileRefs(x + 1, y).Active || !Main.tileSolid[(int)TileRefs(x + 1, y).Type] || Main.tileSolidTop[(int)TileRefs(x + 1, y).Type])) || (TileRefs(x, y + 1).Liquid < 255 && (!TileRefs(x, y + 1).Active || !Main.tileSolid[(int)TileRefs(x, y + 1).Type] || Main.tileSolidTop[(int)TileRefs(x, y + 1).Type])))
				{
					TileRefs(x, y).SetLiquid(0);
				}
			}
			else if (TileRefs(x, y + 1).Liquid < 255 && (!TileRefs(x, y + 1).Active || !Main.tileSolid[(int)TileRefs(x, y + 1).Type] || Main.tileSolidTop[(int)TileRefs(x, y + 1).Type]) && !Liquid.stuck)
			{
				Main.liquid[id].kill = 0;
				return;
			}

			if (TileRefs(x, y).Liquid < 250 && TileRefs(x, y - 1).Liquid > 0)
				Liquid.AddWater(TileRefs, sandbox, x, y - 1);

			if (TileRefs(x, y).Liquid == 0)
			{
				TileRefs(x, y).SetLava(false);
			}
			else
			{
				if ((TileRefs(x + 1, y).Liquid > 0 && TileRefs(x + 1, y + 1).Liquid < 250 && !TileRefs(x + 1, y + 1).Active) || (TileRefs(x - 1, y).Liquid > 0 && TileRefs(x - 1, y + 1).Liquid < 250 && !TileRefs(x - 1, y + 1).Active))
				{
					Liquid.AddWater(TileRefs, sandbox, x - 1, y);
					Liquid.AddWater(TileRefs, sandbox, x + 1, y);
				}
				if (TileRefs(x, y).Lava)
				{
					Liquid.LavaCheck(TileRefs, sandbox, x, y);
					for (int i = x - 1; i <= x + 1; i++)
					{
						for (int j = y - 1; j <= y + 1; j++)
						{
							if (TileRefs(i, j).Active)
							{
								if (TileRefs(i, j).Type == 2 || TileRefs(i, j).Type == 23 || TileRefs(i, j).Type == 109)
								{
									TileRefs(i, j).SetType(0);
									WorldModify.SquareTileFrame(TileRefs, sandbox,i, j, true);
									NetMessage.SendTileSquare(-1, x, y, 3);
								}
								else if (TileRefs(i, j).Type == 60 || TileRefs(i, j).Type == 70)
								{
									TileRefs(i, j).SetType(59);
									WorldModify.SquareTileFrame(TileRefs, sandbox,i, j, true);
									NetMessage.SendTileSquare(-1, x, y, 3);
								}
							}
						}
					}
				}
			}

			NetMessage.SendWater(x, y);

			Liquid.numLiquid--;
			TileRefs(Main.liquid[id].x, Main.liquid[id].y).SetCheckingLiquid(false);
			Main.liquid[id].x = Main.liquid[Liquid.numLiquid].x;
			Main.liquid[id].y = Main.liquid[Liquid.numLiquid].y;
			Main.liquid[id].kill = Main.liquid[Liquid.numLiquid].kill;

			if (Main.tileAlch[(int)TileRefs(x, y).Type])
				WorldModify.CheckAlch(TileRefs, sandbox, x, y);
		}
	}
}
