using System;
using Terraria_Server.Misc;
using Terraria_Server.WorldMod;

namespace Terraria_Server
{
    public static class Collision
    {
        [ThreadStatic]
        public static bool up;
        
        [ThreadStatic]
        public static bool down;
        
        public static bool CanHit(Vector2 Position1, int Width1, int Height1, Vector2 Position2, int Width2, int Height2)
        {
            int entityX = (int)((Position1.X + (float)(Width1 / 2)) / 16f);
            int entityY = (int)((Position1.Y + (float)(Height1 / 2)) / 16f);
            int targetX = (int)((Position2.X + (float)(Width2 / 2)) / 16f);
            int targetY = (int)((Position2.Y + (float)(Height2 / 2)) / 16f);
            while (true)
			{
				if (entityX == targetX && entityY == targetY)
				{
					break;
				}
                int distanceX = Math.Abs(entityX - targetX);
                int distanceY = Math.Abs(entityY - targetY);
                if (distanceX > distanceY)
                {
                    if (entityX < targetX)
                    {
                        entityX++;
                    }
                    else
                    {
                        entityX--;
                    }
                    if (Main.tile.At(entityX, entityY - 1).Active && Main.tileSolid[(int)Main.tile.At(entityX, entityY - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(entityX, entityY - 1).Type] && Main.tile.At(entityX, entityY + 1).Active && Main.tileSolid[(int)Main.tile.At(entityX, entityY + 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(entityX, entityY + 1).Type])
                    {
                        return false;
                    }
                }
                else
                {
                    if (entityY < targetY)
                    {
                        entityY++;
                    }
                    else
                    {
                        entityY--;
                    }
                    if (Main.tile.At(entityX - 1, entityY).Active && Main.tileSolid[(int)Main.tile.At(entityX - 1, entityY).Type] && !Main.tileSolidTop[(int)Main.tile.At(entityX - 1, entityY).Type] && Main.tile.At(entityX + 1, entityY).Active && Main.tileSolid[(int)Main.tile.At(entityX + 1, entityY).Type] && !Main.tileSolidTop[(int)Main.tile.At(entityX + 1, entityY).Type])
                    {
                        return false;
                    }
                }
                if (Main.tile.At(entityX, entityY).Active && Main.tileSolid[(int)Main.tile.At(entityX, entityY).Type] && !Main.tileSolidTop[(int)Main.tile.At(entityX, entityY).Type])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool EmptyTile(int x, int y, bool ignoreTiles = false)
        {
            Rectangle rectangle = new Rectangle(x * 16, y * 16, 16, 16);
            if (Main.tile.At(x, y).Active && !ignoreTiles)
            {
                return false;
            }

            foreach(Player player in Main.players)
            {
                if (player.Active && player.Intersects(rectangle))
                {
                    return false;
                }
            }

            for (int i = 0; i < 200; i++)
            {
                if (Main.item[i].Active && Main.item[i].Intersects(rectangle))
                {
                    return false;
                }
            }

            for (int i = 0; i < NPC.MAX_NPCS; i++)
            {
                if (Main.npcs[i].Active && Main.npcs[i].Intersects(rectangle))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool DrownCollision(Vector2 Position, int Width, int Height, float gravDir = -1f)
        {
            Vector2 vector = new Vector2(Position.X + (float)(Width / 2), Position.Y + (float)(Height / 2));
            int num = 10;
            int num2 = 12;
            if (num > Width)
            {
                num = Width;
            }
            if (num2 > Height)
            {
                num2 = Height;
            }
            vector = new Vector2(vector.X - (float)(num / 2), Position.Y + -2f);
            if (gravDir == -1f)
            {
                vector.Y += (float)(Height / 2 - 6);
            }
            int left = (int)(Position.X / 16f) - 1;
            int right = (int)((Position.X + (float)Width) / 16f) + 2;
            int top = (int)(Position.Y / 16f) - 1;
            int bottom = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (left < 0)
            {
                left = 0;
            }
            if (right > Main.maxTilesX)
            {
                right = Main.maxTilesX;
            }
            if (top < 0)
            {
                top = 0;
            }
            if (bottom > Main.maxTilesY)
            {
                bottom = Main.maxTilesY;
            }
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    if (Main.tile.At(i, j).Liquid > 0)
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        int num7 = 16;
                        float num8 = (float)(256 - Main.tile.At(i, j).Liquid);
                        num8 /= 32f;
                        vector2.Y += num8 * 2f;
                        num7 -= (int)(num8 * 2f);
                        if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num7)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool WetCollision(Vector2 Position, int Width, int Height)
        {
            Vector2 vector = new Vector2(Position.X + (float)(Width / 2), Position.Y + (float)(Height / 2));
            int num = 10;
            int num2 = Height / 2;
            if (num > Width)
            {
                num = Width;
            }
            if (num2 > Height)
            {
                num2 = Height;
            }
            vector = new Vector2(vector.X - (float)(num / 2), vector.Y - (float)(num2 / 2));
            int left = (int)(Position.X / 16f) - 1;
            int right = (int)((Position.X + (float)Width) / 16f) + 2;
            int top = (int)(Position.Y / 16f) - 1;
            int bottom = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (left < 0)
            {
                left = 0;
            }
            if (right > Main.maxTilesX)
            {
                right = Main.maxTilesX;
            }
            if (top < 0)
            {
                top = 0;
            }
            if (bottom > Main.maxTilesY)
            {
                bottom = Main.maxTilesY;
            }
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    if (Main.tile.At(i, j).Liquid > 0)
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        int num7 = 16;
                        float num8 = (float)(256 - Main.tile.At(i, j).Liquid);
                        num8 /= 32f;
                        vector2.Y += num8 * 2f;
                        num7 -= (int)(num8 * 2f);
                        if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num7)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool LavaCollision(Vector2 Position, int Width, int Height)
        {
            int left = (int)(Position.X / 16f) - 1;
            int right = (int)((Position.X + (float)Width) / 16f) + 2;
            int top = (int)(Position.Y / 16f) - 1;
            int bottom = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (left < 0)
            {
                left = 0;
            }
            if (right > Main.maxTilesX)
            {
                right = Main.maxTilesX;
            }
            if (top < 0)
            {
                top = 0;
            }
            if (bottom > Main.maxTilesY)
            {
                bottom = Main.maxTilesY;
            }
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    if (Main.tile.At(i, j).Liquid > 0 && Main.tile.At(i, j).Lava)
                    {
                        Vector2 vector;
                        vector.X = (float)(i * 16);
                        vector.Y = (float)(j * 16);
                        int num6 = 16;
                        float num7 = (float)(0 - Main.tile.At(i, j).Liquid);
                        num7 /= 32f;
                        vector.Y += num7 * 2f;
                        num6 -= (int)(num7 * 2f);
                        if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + (float)num6)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

		public static Vector2 TileCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough = false, bool fall2 = false)
		{
			Collision.up = false;
			Collision.down = false;
			Vector2 result = Velocity;
			Vector2 nextPos = Position + Velocity;
			int left = (int)(Position.X / 16f) - 1;
			int right = (int)((Position.X + (float)Width) / 16f) + 2;
			int top = (int)(Position.Y / 16f) - 1;
			int bottom = (int)((Position.Y + (float)Height) / 16f) + 2;
			int num5 = -1;
			int num6 = -1;
			int num7 = -1;
			int num8 = -1;
			if (left < 0)
			{
				left = 0;
			}
			if (right > Main.maxTilesX)
			{
				right = Main.maxTilesX;
			}
			if (top < 0)
			{
				top = 0;
			}
			if (bottom > Main.maxTilesY)
			{
				bottom = Main.maxTilesY;
			}
			for (int i = left; i < right; i++)
			{
				for (int j = top; j < bottom; j++)
				{
					if (Main.tile.At(i, j).Active && (Main.tileSolid[(int)Main.tile.At(i, j).Type] || (Main.tileSolidTop[(int)Main.tile.At(i, j).Type] && Main.tile.At(i, j).FrameY == 0)))
					{
						Vector2 vector4;
						vector4.X = (float)(i * 16);
						vector4.Y = (float)(j * 16);
						if (nextPos.X + (float)Width > vector4.X && nextPos.X < vector4.X + 16f && nextPos.Y + (float)Height > vector4.Y && nextPos.Y < vector4.Y + 16f)
						{
							if (Position.Y + (float)Height <= vector4.Y)
							{
								Collision.down = true;
								if (!Main.tileSolidTop[(int)Main.tile.At(i, j).Type] || !fallThrough || (Velocity.Y > 1f && !fall2))
								{
									num7 = i;
									num8 = j;
									if (num7 != num5)
									{
										result.Y = vector4.Y - (Position.Y + (float)Height);
									}
								}
							}
							else if (Position.X + (float)Width <= vector4.X && !Main.tileSolidTop[(int)Main.tile.At(i, j).Type])
							{
								num5 = i;
								num6 = j;
								if (num6 != num8)
								{
									result.X = vector4.X - (Position.X + (float)Width);
								}
								if (num7 == num5)
								{
									result.Y = Velocity.Y;
								}
							}
							else if (Position.X >= vector4.X + 16f && !Main.tileSolidTop[(int)Main.tile.At(i, j).Type])
							{
								num5 = i;
								num6 = j;
								if (num6 != num8)
								{
									result.X = vector4.X + 16f - Position.X;
								}
								if (num7 == num5)
								{
									result.Y = Velocity.Y;
								}
							}
							else if (Position.Y >= vector4.Y + 16f && !Main.tileSolidTop[(int)Main.tile.At(i, j).Type])
							{
								Collision.up = true;
								num7 = i;
								num8 = j;
								result.Y = vector4.Y + 16f - Position.Y;
								if (num8 == num6)
								{
									result.X = Velocity.X;
								}
							}
						}
					}
				}
			}
			return result;
		}

		public static bool SolidCollision(Vector2 Position, int Width, int Height)
		{
			int left = (int)(Position.X / 16f) - 1;
			int right = (int)((Position.X + (float)Width) / 16f) + 2;
			int top = (int)(Position.Y / 16f) - 1;
			int bottom = (int)((Position.Y + (float)Height) / 16f) + 2;
			if (left < 0)
			{
				left = 0;
			}
			if (right > Main.maxTilesX)
			{
				right = Main.maxTilesX;
			}
			if (top < 0)
			{
				top = 0;
			}
			if (bottom > Main.maxTilesY)
			{
				bottom = Main.maxTilesY;
			}
			for (int i = left; i < right; i++)
			{
				for (int j = top; j < bottom; j++)
				{
					if (Main.tile.At(i, j).Active && Main.tileSolid[(int)Main.tile.At(i, j).Type] && !Main.tileSolidTop[(int)Main.tile.At(i, j).Type])
					{
						Vector2 vector;
						vector.X = (float)(i * 16);
						vector.Y = (float)(j * 16);
						if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + 16f)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

        public static Vector2 WaterCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough = false, bool fall2 = false)
        {
            Vector2 result = Velocity;
            Vector2 nextPos = Position + Velocity;
            int left = (int)(Position.X / 16f) - 1;
            int right = (int)((Position.X + (float)Width) / 16f) + 2;
            int top = (int)(Position.Y / 16f) - 1;
            int bottom = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (left < 0)
            {
                left = 0;
            }
            if (right > Main.maxTilesX)
            {
                right = Main.maxTilesX;
            }
            if (top < 0)
            {
                top = 0;
            }
            if (bottom > Main.maxTilesY)
            {
                bottom = Main.maxTilesY;
            }
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    if (Main.tile.At(i, j).Liquid > 0)
                    {
                        int num5 = (int)Math.Round((double)((float)Main.tile.At(i, j).Liquid / 32f)) * 2;
                        Vector2 vector3;
                        vector3.X = (float)(i * 16);
                        vector3.Y = (float)(j * 16 + 16 - num5);
                        if (nextPos.X + (float)Width > vector3.X && nextPos.X < vector3.X + 16f && nextPos.Y + (float)Height > vector3.Y && nextPos.Y < vector3.Y + (float)num5 && Position.Y + (float)Height <= vector3.Y && !fallThrough)
                        {
                            result.Y = vector3.Y - (Position.Y + (float)Height);
                        }
                    }
                }
            }
            return result;
        }

        public static Vector2 AnyCollision(Vector2 Position, Vector2 Velocity, int Width, int Height)
        {
            Vector2 result = Velocity;
            Vector2 nextPos = Position + Velocity;
            int left = (int)(Position.X / 16f) - 1;
            int right = (int)((Position.X + (float)Width) / 16f) + 2;
            int top = (int)(Position.Y / 16f) - 1;
            int bottom = (int)((Position.Y + (float)Height) / 16f) + 2;
            int num5 = -1;
            int num6 = -1;
            int num7 = -1;
            int num8 = -1;
            if (left < 0)
            {
                left = 0;
            }
            if (right > Main.maxTilesX)
            {
                right = Main.maxTilesX;
            }
            if (top < 0)
            {
                top = 0;
            }
            if (bottom > Main.maxTilesY)
            {
                bottom = Main.maxTilesY;
            }
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    if (Main.tile.At(i, j).Active)
                    {
                        Vector2 vector4;
                        vector4.X = (float)(i * 16);
                        vector4.Y = (float)(j * 16);
                        if (nextPos.X + (float)Width > vector4.X && nextPos.X < vector4.X + 16f && nextPos.Y + (float)Height > vector4.Y && nextPos.Y < vector4.Y + 16f)
                        {
                            if (Position.Y + (float)Height <= vector4.Y)
                            {
                                num7 = i;
                                num8 = j;
                                if (num7 != num5)
                                {
                                    result.Y = vector4.Y - (Position.Y + (float)Height);
                                }
                            }
                            else if (Position.X + (float)Width <= vector4.X && !Main.tileSolidTop[(int)Main.tile.At(i, j).Type])
                            {
                                num5 = i;
                                num6 = j;
                                if (num6 != num8)
                                {
                                    result.X = vector4.X - (Position.X + (float)Width);
                                }
                                if (num7 == num5)
                                {
                                    result.Y = Velocity.Y;
                                }
                            }
                            else if (Position.X >= vector4.X + 16f && !Main.tileSolidTop[(int)Main.tile.At(i, j).Type])
                            {
                                num5 = i;
                                num6 = j;
                                if (num6 != num8)
                                {
                                    result.X = vector4.X + 16f - Position.X;
                                }
                                if (num7 == num5)
                                {
                                    result.Y = Velocity.Y;
                                }
                            }
                            else if (Position.Y >= vector4.Y + 16f && !Main.tileSolidTop[(int)Main.tile.At(i, j).Type])
                            {
                                num7 = i;
                                num8 = j;
                                result.Y = vector4.Y + 16f - Position.Y + 0.01f;
                                if (num8 == num6)
                                {
                                    result.X = Velocity.X + 0.01f;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

		public static void HitTiles(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, Vector2 Position, Vector2 Velocity, int Width, int Height)
        {
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

            Vector2 nextPos = Position + Velocity;
            int left = (int)(Position.X / 16f) - 1;
            int right = (int)((Position.X + (float)Width) / 16f) + 2;
            int top = (int)(Position.Y / 16f) - 1;
            int bottom = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (left < 0)
            {
                left = 0;
            }
            if (right > Main.maxTilesX)
            {
                right = Main.maxTilesX;
            }
            if (top < 0)
            {
                top = 0;
            }
            if (bottom > Main.maxTilesY)
            {
                bottom = Main.maxTilesY;
            }
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    if (TileRefs(i, j).Active && (Main.tileSolid[(int)TileRefs(i, j).Type] || (Main.tileSolidTop[(int)TileRefs(i, j).Type] && TileRefs(i, j).FrameY == 0)))
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        if (nextPos.X + (float)Width >= vector2.X && nextPos.X <= vector2.X + 16f && nextPos.Y + (float)Height >= vector2.Y && nextPos.Y <= vector2.Y + 16f)
                        {
							WorldModify.KillTile(TileRefs, sandbox, i, j, true, true);
                        }
                    }
                }
            }
        }

		public static Vector2 HurtTiles(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, Vector2 Position, Vector2 Velocity, int Width, int Height, bool fireImmune = false)
        {
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

            int left = (int)(Position.X / 16f) - 1;
            int right = (int)((Position.X + (float)Width) / 16f) + 2;
            int top = (int)(Position.Y / 16f) - 1;
            int bottom = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (left < 0)
            {
                left = 0;
            }
            if (right > Main.maxTilesX)
            {
                right = Main.maxTilesX;
            }
            if (top < 0)
            {
                top = 0;
            }
            if (bottom > Main.maxTilesY)
            {
                bottom = Main.maxTilesY;
            }
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    if (TileRefs(i, j).Active && (TileRefs(i, j).Type == 32 || TileRefs(i, j).Type == 37 || TileRefs(i, j).Type == 48 || TileRefs(i, j).Type == 53 || TileRefs(i, j).Type == 57 || TileRefs(i, j).Type == 58 || TileRefs(i, j).Type == 59 || TileRefs(i, j).Type == 69 || TileRefs(i, j).Type == 76 || TileRefs(i, j).Type == 80))
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        int Y = 0;
                        int type = (int)TileRefs(i, j).Type;
                        if (type == 32 || type == 69 || type == 80)
                        {
                            if (Position.X + (float)Width > vector2.X && Position.X < vector2.X + 16f && Position.Y + (float)Height > vector2.Y && (double)Position.Y < (double)vector2.Y + 16.01)
                            {
                                int directionX = 1;
                                if (Position.X + (float)(Width / 2) < vector2.X + 8f)
                                    directionX = -1;

                                Y = 10;
                                if (type == 69)
                                    Y = 17;
								else if (type == 80)
                                    Y = 6;
								if (type == 32 || type == 69)
                                    WorldModify.KillTile(TileRefs, sandbox, i, j);								

                                return new Vector2((float)directionX, (float)Y);
                            }
                        }
                        else if (type == 53 || type == 112 || type == 116 || type == 123)
                        {
                            if (Position.X + (float)Width - 2f >= vector2.X && Position.X + 2f <= vector2.X + 16f && Position.Y + (float)Height - 2f >= vector2.Y && Position.Y + 2f <= vector2.Y + 16f)
                            {
                                int directionX = 1;
                                if (Position.X + (float)(Width / 2) < vector2.X + 8f)
                                    directionX = -1;

                                Y = 20;
                                return new Vector2((float)directionX, (float)Y);
                            }
                        }
                        else if (Position.X + (float)Width >= vector2.X && Position.X <= vector2.X + 16f && Position.Y + (float)Height >= vector2.Y && (double)Position.Y <= (double)vector2.Y + 16.01)
                        {
                            int directionX = 1;
                            if (Position.X + (float)(Width / 2) < vector2.X + 8f)
                                directionX = -1;

                            if (!fireImmune && (type == 37 || type == 58 || type == 76))
                                Y = 20;
                            if (type == 48)
                                Y = 40;

                            return new Vector2((float)directionX, (float)Y);
                        }
                    }
                }
            }
            return default(Vector2);
        }

        public static bool StickyTiles(Vector2 Position, Vector2 Velocity, int Width, int Height)
        {
            bool result = false;
            int left = (int)(Position.X / 16f) - 1;
            int right = (int)((Position.X + (float)Width) / 16f) + 2;
            int top = (int)(Position.Y / 16f) - 1;
            int bottom = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (left < 0)
            {
                left = 0;
            }
            if (right > Main.maxTilesX)
            {
                right = Main.maxTilesX;
            }
            if (top < 0)
            {
                top = 0;
            }
            if (bottom > Main.maxTilesY)
            {
                bottom = Main.maxTilesY;
            }
            for (int i = left; i < right; i++)
            {
                for (int j = top; j < bottom; j++)
                {
                    if (Main.tile.At(i, j).Active && Main.tile.At(i, j).Type == 51)
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        if (Position.X + (float)Width > vector2.X && Position.X < vector2.X + 16f && Position.Y + (float)Height > vector2.Y && (double)Position.Y < (double)vector2.Y + 16.01)
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        public static bool SolidTiles(int startX, int endX, int startY, int endY)
        {
            if (startX < 0)
            {
                return true;
            }
            if (endX >= Main.maxTilesX)
            {
                return true;
            }
            if (startY < 0)
            {
                return true;
            }
            if (endY >= Main.maxTilesY)
            {
                return true;
            }
            for (int i = startX; i < endX + 1; i++)
            {
                for (int j = startY; j < endY + 1; j++)
                {
                    if (Main.tile.At(i, j).Active && Main.tileSolid[(int)Main.tile.At(i, j).Type] && !Main.tileSolidTop[(int)Main.tile.At(i, j).Type])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

		public static bool SwitchTiles(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, Vector2 Position, int Width, int Height, Vector2 oldPosition, ISender Sender)
        {
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;

            if (num < 0)
                num = 0;

            if (num2 > Main.maxTilesX)
                num2 = Main.maxTilesX;

            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesY)
                num4 = Main.maxTilesY;

            for (int x = num; x < num2; x++)
            {
                for (int y = num3; y < num4; y++)
                {
                    if (TileRefs(x, y).Active && TileRefs(x, y).Type == 135)
                    {
                        Vector2 vector;
                        vector.X = (float)(x * 16);
                        vector.Y = (float)(y * 16 + 12);
                        if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + 
							(float)Height > vector.Y && (double)Position.Y < (double)vector.Y + 4.01 && (oldPosition.X + 
							(float)Width <= vector.X || oldPosition.X >= vector.X + 16f || oldPosition.Y + 
							(float)Height <= vector.Y || (double)oldPosition.Y >= (double)vector.Y + 16.01))
                        {
							WorldModify.hitSwitch(TileRefs, sandbox, x, y, Sender);
                            NetMessage.SendData(59, -1, -1, "", x, (float)y, 0f, 0f, 0);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
