using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class Collision
    {
        /*public static bool CanHit(Vector2 Position1, int Width1, int Height1, Vector2 Position2, int Width2, int Height2)
        {
            int num = (int)((Position1.X + (float)(Width1 / 2)) / 16f);
            int num2 = (int)((Position1.Y + (float)(Height1 / 2)) / 16f);
            int num3 = (int)((Position2.X + (float)(Width2 / 2)) / 16f);
            int num4 = (int)((Position2.Y + (float)(Height2 / 2)) / 16f);
            while (true)
            {
                int num5 = Math.Abs(num - num3);
                int num6 = Math.Abs(num2 - num4);
                if (num == num3 && num2 == num4)
                {
                    break;
                }
                if (num5 > num6)
                {
                    if (num < num3)
                    {
                        num++;
                    }
                    else
                    {
                        num--;
                    }
                    if (world.getTile()[num, num2 - 1] == null)
                    {
                        world.getTile()[num, num2 - 1] = new Tile();
                    }
                    if (world.getTile()[num, num2 + 1] == null)
                    {
                        world.getTile()[num, num2 + 1] = new Tile();
                    }
                    if (world.getTile()[num, num2 - 1].active && Main.tileSolid[(int)world.getTile()[num, num2 - 1].type] && !Main.tileSolidTop[(int)world.getTile()[num, num2 - 1].type] && world.getTile()[num, num2 + 1].active && Main.tileSolid[(int)world.getTile()[num, num2 + 1].type] && !Main.tileSolidTop[(int)world.getTile()[num, num2 + 1].type])
                    {
                        return false;
                    }
                }
                else
                {
                    if (num2 < num4)
                    {
                        num2++;
                    }
                    else
                    {
                        num2--;
                    }
                    if (world.getTile()[num - 1, num2] == null)
                    {
                        world.getTile()[num - 1, num2] = new Tile();
                    }
                    if (world.getTile()[num + 1, num2] == null)
                    {
                        world.getTile()[num + 1, num2] = new Tile();
                    }
                    if (world.getTile()[num - 1, num2].active && Main.tileSolid[(int)world.getTile()[num - 1, num2].type] && !Main.tileSolidTop[(int)world.getTile()[num - 1, num2].type] && world.getTile()[num + 1, num2].active && Main.tileSolid[(int)world.getTile()[num + 1, num2].type] && !Main.tileSolidTop[(int)world.getTile()[num + 1, num2].type])
                    {
                        return false;
                    }
                }
                if (world.getTile()[num, num2] == null)
                {
                    world.getTile()[num, num2] = new Tile();
                }
                if (world.getTile()[num, num2].active && Main.tileSolid[(int)world.getTile()[num, num2].type] && !Main.tileSolidTop[(int)world.getTile()[num, num2].type])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool EmptyTile(int i, int j, bool ignoreTiles = false)
        {
            Rectangle rectangle = new Rectangle(i * 16, j * 16, 16, 16);
            if (world.getTile()[i, j].active && !ignoreTiles)
            {
                return false;
            }
            for (int k = 0; k < 8; k++)
            {
                if (world.getPlayerList()k].active && rectangle.Intersects(new Rectangle((int)world.getPlayerList()k].position.X, (int)world.getPlayerList()k].position.Y, world.getPlayerList()k].width, world.getPlayerList()k].height)))
                {
                    return false;
                }
            }
            for (int l = 0; l < 200; l++)
            {
                if (world.getItemList()l].active && rectangle.Intersects(new Rectangle((int)world.getItemList()l].position.X, (int)world.getItemList()l].position.Y, world.getItemList()l].width, world.getItemList()l].height)))
                {
                    return false;
                }
            }
            for (int m = 0; m < 1000; m++)
            {
                if (world.getNPCs()[m].active && rectangle.Intersects(new Rectangle((int)world.getNPCs()[m].position.X, (int)world.getNPCs()[m].position.Y, world.getNPCs()[m].width, world.getNPCs()[m].height)))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool DrownCollision(Vector2 Position, int Width, int Height)
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
            vector = new Vector2(vector.X - (float)(num / 2), Position.Y + 2f);
            int num3 = (int)(Position.X / 16f) - 1;
            int num4 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num5 = (int)(Position.Y / 16f) - 1;
            int num6 = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > maxTilesX)
            {
                num4 = maxTilesX;
            }
            if (num5 < 0)
            {
                num5 = 0;
            }
            if (num6 > maxTilesY)
            {
                num6 = maxTilesY;
            }
            for (int i = num3; i < num4; i++)
            {
                for (int j = num5; j < num6; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].liquid > 0)
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        int num7 = 16;
                        float num8 = (float)(0 - world.getTile()[i, j].liquid);
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
            int num3 = (int)(Position.X / 16f) - 1;
            int num4 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num5 = (int)(Position.Y / 16f) - 1;
            int num6 = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > maxTilesX)
            {
                num4 = maxTilesX;
            }
            if (num5 < 0)
            {
                num5 = 0;
            }
            if (num6 > maxTilesY)
            {
                num6 = maxTilesY;
            }
            for (int i = num3; i < num4; i++)
            {
                for (int j = num5; j < num6; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].liquid > 0)
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        int num7 = 16;
                        float num8 = (float)(0 - world.getTile()[i, j].liquid);
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
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (num < 0)
            {
                num = 0;
            }
            if (num2 > maxTilesX)
            {
                num2 = maxTilesX;
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > maxTilesY)
            {
                num4 = maxTilesY;
            }
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].liquid > 0 && world.getTile()[i, j].lava)
                    {
                        Vector2 vector;
                        vector.X = (float)(i * 16);
                        vector.Y = (float)(j * 16);
                        int num5 = 16;
                        float num6 = (float)(0 - world.getTile()[i, j].liquid);
                        num6 /= 32f;
                        vector.Y += num6 * 2f;
                        num5 -= (int)(num6 * 2f);
                        if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + (float)num5)
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
            Vector2 result = Velocity;
            Vector2 vector = Velocity;
            Vector2 vector2 = Position + Velocity;
            Vector2 vector3 = Position;
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            int num5 = -1;
            int num6 = -1;
            int num7 = -1;
            int num8 = -1;
            if (num < 0)
            {
                num = 0;
            }
            if (num2 > maxTilesX)
            {
                num2 = maxTilesX;
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > maxTilesY)
            {
                num4 = maxTilesY;
            }
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].active && (Main.tileSolid[(int)world.getTile()[i, j].type] || (Main.tileSolidTop[(int)world.getTile()[i, j].type] && world.getTile()[i, j].frameY == 0)))
                    {
                        Vector2 vector4;
                        vector4.X = (float)(i * 16);
                        vector4.Y = (float)(j * 16);
                        if (vector2.X + (float)Width > vector4.X && vector2.X < vector4.X + 16f && vector2.Y + (float)Height > vector4.Y && vector2.Y < vector4.Y + 16f)
                        {
                            if (vector3.Y + (float)Height <= vector4.Y)
                            {
                                if (!Main.tileSolidTop[(int)world.getTile()[i, j].type] || !fallThrough || (Velocity.Y > 1f && !fall2))
                                {
                                    num7 = i;
                                    num8 = j;
                                    if (num7 != num5)
                                    {
                                        result.Y = vector4.Y - (vector3.Y + (float)Height);
                                    }
                                }
                            }
                            else
                            {
                                if (vector3.X + (float)Width <= vector4.X && !Main.tileSolidTop[(int)world.getTile()[i, j].type])
                                {
                                    num5 = i;
                                    num6 = j;
                                    if (num6 != num8)
                                    {
                                        result.X = vector4.X - (vector3.X + (float)Width);
                                    }
                                    if (num7 == num5)
                                    {
                                        result.Y = vector.Y;
                                    }
                                }
                                else
                                {
                                    if (vector3.X >= vector4.X + 16f && !Main.tileSolidTop[(int)world.getTile()[i, j].type])
                                    {
                                        num5 = i;
                                        num6 = j;
                                        if (num6 != num8)
                                        {
                                            result.X = vector4.X + 16f - vector3.X;
                                        }
                                        if (num7 == num5)
                                        {
                                            result.Y = vector.Y;
                                        }
                                    }
                                    else
                                    {
                                        if (vector3.Y >= vector4.Y + 16f && !Main.tileSolidTop[(int)world.getTile()[i, j].type])
                                        {
                                            num7 = i;
                                            num8 = j;
                                            result.Y = vector4.Y + 16f - vector3.Y + 0.01f;
                                            if (num8 == num6)
                                            {
                                                result.X = vector.X + 0.01f;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
        public static Vector2 AnyCollision(Vector2 Position, Vector2 Velocity, int Width, int Height)
        {
            Vector2 result = Velocity;
            Vector2 vector = Velocity;
            Vector2 vector2 = Position + Velocity;
            Vector2 vector3 = Position;
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            int num5 = -1;
            int num6 = -1;
            int num7 = -1;
            int num8 = -1;
            if (num < 0)
            {
                num = 0;
            }
            if (num2 > maxTilesX)
            {
                num2 = maxTilesX;
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > maxTilesY)
            {
                num4 = maxTilesY;
            }
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].active)
                    {
                        Vector2 vector4;
                        vector4.X = (float)(i * 16);
                        vector4.Y = (float)(j * 16);
                        if (vector2.X + (float)Width > vector4.X && vector2.X < vector4.X + 16f && vector2.Y + (float)Height > vector4.Y && vector2.Y < vector4.Y + 16f)
                        {
                            if (vector3.Y + (float)Height <= vector4.Y)
                            {
                                num7 = i;
                                num8 = j;
                                if (num7 != num5)
                                {
                                    result.Y = vector4.Y - (vector3.Y + (float)Height);
                                }
                            }
                            else
                            {
                                if (vector3.X + (float)Width <= vector4.X && !Main.tileSolidTop[(int)world.getTile()[i, j].type])
                                {
                                    num5 = i;
                                    num6 = j;
                                    if (num6 != num8)
                                    {
                                        result.X = vector4.X - (vector3.X + (float)Width);
                                    }
                                    if (num7 == num5)
                                    {
                                        result.Y = vector.Y;
                                    }
                                }
                                else
                                {
                                    if (vector3.X >= vector4.X + 16f && !Main.tileSolidTop[(int)world.getTile()[i, j].type])
                                    {
                                        num5 = i;
                                        num6 = j;
                                        if (num6 != num8)
                                        {
                                            result.X = vector4.X + 16f - vector3.X;
                                        }
                                        if (num7 == num5)
                                        {
                                            result.Y = vector.Y;
                                        }
                                    }
                                    else
                                    {
                                        if (vector3.Y >= vector4.Y + 16f && !Main.tileSolidTop[(int)world.getTile()[i, j].type])
                                        {
                                            num7 = i;
                                            num8 = j;
                                            result.Y = vector4.Y + 16f - vector3.Y + 0.01f;
                                            if (num8 == num6)
                                            {
                                                result.X = vector.X + 0.01f;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
        public static void HitTiles(Vector2 Position, Vector2 Velocity, int Width, int Height)
        {
            Vector2 vector = Position + Velocity;
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (num < 0)
            {
                num = 0;
            }
            if (num2 > maxTilesX)
            {
                num2 = maxTilesX;
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > maxTilesY)
            {
                num4 = maxTilesY;
            }
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].active && (Main.tileSolid[(int)world.getTile()[i, j].type] || (Main.tileSolidTop[(int)world.getTile()[i, j].type] && world.getTile()[i, j].frameY == 0)))
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        if (vector.X + (float)Width >= vector2.X && vector.X <= vector2.X + 16f && vector.Y + (float)Height >= vector2.Y && vector.Y <= vector2.Y + 16f)
                        {
                            WorldGen.KillTile(i, j, true, true, false);
                        }
                    }
                }
            }
        }
        public static Vector2 HurtTiles(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fireImmune = false)
        {
            Vector2 vector = Position;
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (num < 0)
            {
                num = 0;
            }
            if (num2 > maxTilesX)
            {
                num2 = maxTilesX;
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > maxTilesY)
            {
                num4 = maxTilesY;
            }
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].active && (world.getTile()[i, j].type == 32 || world.getTile()[i, j].type == 37 || world.getTile()[i, j].type == 48 || world.getTile()[i, j].type == 53 || world.getTile()[i, j].type == 58 || world.getTile()[i, j].type == 69 || world.getTile()[i, j].type == 76))
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        int num5 = 0;
                        int type = (int)world.getTile()[i, j].type;
                        if (type == 32 || type == 69)
                        {
                            if (vector.X + (float)Width > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)Height > vector2.Y && (double)vector.Y < (double)vector2.Y + 16.01)
                            {
                                int num6 = 1;
                                if (vector.X + (float)(Width / 2) < vector2.X + 8f)
                                {
                                    num6 = -1;
                                }
                                num5 = 10;
                                if (type == 69)
                                {
                                    num5 = 25;
                                }
                                return new Vector2((float)num6, (float)num5);
                            }
                        }
                        else
                        {
                            if (type == 53)
                            {
                                if (vector.X + (float)Width - 2f >= vector2.X && vector.X + 2f <= vector2.X + 16f && vector.Y + (float)Height - 2f >= vector2.Y && vector.Y + 2f <= vector2.Y + 16f)
                                {
                                    int num7 = 1;
                                    if (vector.X + (float)(Width / 2) < vector2.X + 8f)
                                    {
                                        num7 = -1;
                                    }
                                    if (type == 53)
                                    {
                                        num5 = 20;
                                    }
                                    return new Vector2((float)num7, (float)num5);
                                }
                            }
                            else
                            {
                                if (vector.X + (float)Width >= vector2.X && vector.X <= vector2.X + 16f && vector.Y + (float)Height >= vector2.Y && (double)vector.Y <= (double)vector2.Y + 16.01)
                                {
                                    int num8 = 1;
                                    if (vector.X + (float)(Width / 2) < vector2.X + 8f)
                                    {
                                        num8 = -1;
                                    }
                                    if (!fireImmune && (type == 37 || type == 58 || type == 76))
                                    {
                                        num5 = 20;
                                    }
                                    if (type == 48)
                                    {
                                        num5 = 40;
                                    }
                                    return new Vector2((float)num8, (float)num5);
                                }
                            }
                        }
                    }
                }
            }
            return new Vector2();
        }
        public static bool StickyTiles(Vector2 Position, Vector2 Velocity, int Width, int Height)
        {
            Vector2 vector = Position;
            bool result = false;
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (num < 0)
            {
                num = 0;
            }
            if (num2 > maxTilesX)
            {
                num2 = maxTilesX;
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > maxTilesY)
            {
                num4 = maxTilesY;
            }
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].active && world.getTile()[i, j].type == 51)
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        if (vector.X + (float)Width > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)Height > vector2.Y && (double)vector.Y < (double)vector2.Y + 16.01)
                        {
                            if ((double)(Math.Abs(Velocity.X) + Math.Abs(Velocity.Y)) > 0.7 && Main.rand.Next(30) == 0)
                            {
                                Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, 30, 0f, 0f, 0, default(Color), 1f);
                            }
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
            if (endX >= maxTilesX)
            {
                return true;
            }
            if (startY < 0)
            {
                return true;
            }
            if (endY >= maxTilesY)
            {
                return true;
            }
            for (int i = startX; i < endX + 1; i++)
            {
                for (int j = startY; j < endY + 1; j++)
                {
                    if (world.getTile()[i, j] == null)
                    {
                        return false;
                    }
                    if (world.getTile()[i, j].active && Main.tileSolid[(int)world.getTile()[i, j].type] && !Main.tileSolidTop[(int)world.getTile()[i, j].type])
                    {
                        return true;
                    }
                }
            }
            return false;
        }*/

        public static void HitTiles(Vector2 Position, Vector2 Velocity, World world, int Width, int Height)
        {
            Vector2 vector = Position + Velocity;
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (num < 0)
            {
                num = 0;
            }
            if (num2 > world.getMaxTilesX())
            {
                num2 = world.getMaxTilesX();
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > world.getMaxTilesY())
            {
                num4 = world.getMaxTilesY();
            }
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].active && (Statics.tileSolid[(int)world.getTile()[i, j].type] || 
                        (Statics.tileSolidTop[(int)world.getTile()[i, j].type] && world.getTile()[i, j].frameY == 0)))
                    {
                        Vector2 vector2 = new Vector2();
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        if (vector.X + (float)Width >= vector2.X && vector.X <= vector2.X + 16f && vector.Y + (float)Height >= vector2.Y && vector.Y <= vector2.Y + 16f)
                        {
                            WorldGen.KillTile(i, j, world, true, true, false);
                        }
                    }
                }
            }
        }

        public static Vector2 AnyCollision(Vector2 Position, Vector2 Velocity, World world, int Width, int Height)
        {
            Vector2 result = Velocity;
            Vector2 vector = Velocity;
            Vector2 vector2 = Position + Velocity;
            Vector2 vector3 = Position;
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            int num5 = -1;
            int num6 = -1;
            int num7 = -1;
            int num8 = -1;
            if (num < 0)
            {
                num = 0;
            }
            if (num2 > world.getMaxTilesX())
            {
                num2 = world.getMaxTilesX();
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > world.getMaxTilesY())
            {
                num4 = world.getMaxTilesY();
            }
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].active)
                    {
                        Vector2 vector4 = new Vector2();
                        vector4.X = (float)(i * 16);
                        vector4.Y = (float)(j * 16);
                        if (vector2.X + (float)Width > vector4.X && vector2.X < vector4.X + 16f && vector2.Y + 
                            (float)Height > vector4.Y && vector2.Y < vector4.Y + 16f)
                        {
                            if (vector3.Y + (float)Height <= vector4.Y)
                            {
                                num7 = i;
                                num8 = j;
                                if (num7 != num5)
                                {
                                    result.Y = vector4.Y - (vector3.Y + (float)Height);
                                }
                            }
                            else
                            {
                                if (vector3.X + (float)Width <= vector4.X && !Statics.tileSolidTop[(int)world.getTile()[i, j].type])
                                {
                                    num5 = i;
                                    num6 = j;
                                    if (num6 != num8)
                                    {
                                        result.X = vector4.X - (vector3.X + (float)Width);
                                    }
                                    if (num7 == num5)
                                    {
                                        result.Y = vector.Y;
                                    }
                                }
                                else
                                {
                                    if (vector3.X >= vector4.X + 16f && !Statics.tileSolidTop[(int)world.getTile()[i, j].type])
                                    {
                                        num5 = i;
                                        num6 = j;
                                        if (num6 != num8)
                                        {
                                            result.X = vector4.X + 16f - vector3.X;
                                        }
                                        if (num7 == num5)
                                        {
                                            result.Y = vector.Y;
                                        }
                                    }
                                    else
                                    {
                                        if (vector3.Y >= vector4.Y + 16f && !Statics.tileSolidTop[(int)world.getTile()[i, j].type])
                                        {
                                            num7 = i;
                                            num8 = j;
                                            result.Y = vector4.Y + 16f - vector3.Y + 0.01f;
                                            if (num8 == num6)
                                            {
                                                result.X = vector.X + 0.01f;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static bool EmptyTile(int i, int j, World world, bool ignoreTiles = false)
        {
            Rectangle rectangle = new Rectangle(i * 16, j * 16, 16, 16);
            if (world.getTile()[i, j].active && !ignoreTiles)
            {
                return false;
            }
            for (int k = 0; k < 8; k++)
            {
                if (world.getPlayerList()[k].active && rectangle.Intersects(new Rectangle((int)world.getPlayerList()[k].position.X, (int)world.getPlayerList()[k].position.Y, world.getPlayerList()[k].width, world.getPlayerList()[k].height)))
                {
                    return false;
                }
            }
            for (int l = 0; l < 200; l++)
            {
                if (world.getItemList()[l].active && rectangle.Intersects(new Rectangle((int)world.getItemList()[l].position.X, (int)world.getItemList()[l].position.Y, world.getItemList()[l].width, world.getItemList()[l].height)))
                {
                    return false;
                }
            }
            for (int m = 0; m < 1000; m++)
            {
                if (world.getNPCs()[m].active && rectangle.Intersects(new Rectangle((int)world.getNPCs()[m].position.X, (int)world.getNPCs()[m].position.Y, world.getNPCs()[m].width, world.getNPCs()[m].height)))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool WetCollision(Vector2 Position, int Width, int Height, World world)
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
            int num3 = (int)(Position.X / 16f) - 1;
            int num4 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num5 = (int)(Position.Y / 16f) - 1;
            int num6 = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > world.getMaxTilesX())
            {
                num4 = world.getMaxTilesX();
            }
            if (num5 < 0)
            {
                num5 = 0;
            }
            if (num6 > world.getMaxTilesY())
            {
                num6 = world.getMaxTilesY();
            }
            for (int i = num3; i < num4; i++)
            {
                for (int j = num5; j < num6; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].liquid > 0)
                    {
                        Vector2 vector2 = new Vector2();
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        int num7 = 16;
                        float num8 = (float)(0 - world.getTile()[i, j].liquid);
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

        public static bool DrownCollision(Vector2 Position, int Width, int Height, World world)
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
            vector = new Vector2(vector.X - (float)(num / 2), Position.Y + 2f);
            int num3 = (int)(Position.X / 16f) - 1;
            int num4 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num5 = (int)(Position.Y / 16f) - 1;
            int num6 = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > world.getMaxTilesX())
            {
                num4 = world.getMaxTilesX();
            }
            if (num5 < 0)
            {
                num5 = 0;
            }
            if (num6 > world.getMaxTilesY())
            {
                num6 = world.getMaxTilesY();
            }
            for (int i = num3; i < num4; i++)
            {
                for (int j = num5; j < num6; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].liquid > 0)
                    {
                        Vector2 vector2 = new Vector2();
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        int num7 = 16;
                        float num8 = (float)(0 - world.getTile()[i, j].liquid);
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

        public static Vector2 HurtTiles(Vector2 Position, Vector2 Velocity, World world, int Width, int Height, bool fireImmune = false)
        {
            Vector2 vector = Position;
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (num < 0)
            {
                num = 0;
            }
            if (num2 > world.getMaxTilesX())
            {
                num2 = world.getMaxTilesX();
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > world.getMaxTilesY())
            {
                num4 = world.getMaxTilesY();
            }
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].active && (world.getTile()[i, j].type == 32 || world.getTile()[i, j].type == 37 || world.getTile()[i, j].type == 48 || world.getTile()[i, j].type == 53 || world.getTile()[i, j].type == 58 || world.getTile()[i, j].type == 69 || world.getTile()[i, j].type == 76))
                    {
                        Vector2 vector2 = new Vector2();
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        int num5 = 0;
                        int type = (int)world.getTile()[i, j].type;
                        if (type == 32 || type == 69)
                        {
                            if (vector.X + (float)Width > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)Height > vector2.Y && (double)vector.Y < (double)vector2.Y + 16.01)
                            {
                                int num6 = 1;
                                if (vector.X + (float)(Width / 2) < vector2.X + 8f)
                                {
                                    num6 = -1;
                                }
                                num5 = 10;
                                if (type == 69)
                                {
                                    num5 = 25;
                                }
                                return new Vector2((float)num6, (float)num5);
                            }
                        }
                        else
                        {
                            if (type == 53)
                            {
                                if (vector.X + (float)Width - 2f >= vector2.X && vector.X + 2f <= vector2.X + 16f && vector.Y + (float)Height - 2f >= vector2.Y && vector.Y + 2f <= vector2.Y + 16f)
                                {
                                    int num7 = 1;
                                    if (vector.X + (float)(Width / 2) < vector2.X + 8f)
                                    {
                                        num7 = -1;
                                    }
                                    if (type == 53)
                                    {
                                        num5 = 20;
                                    }
                                    return new Vector2((float)num7, (float)num5);
                                }
                            }
                            else
                            {
                                if (vector.X + (float)Width >= vector2.X && vector.X <= vector2.X + 16f && vector.Y + (float)Height >= vector2.Y && (double)vector.Y <= (double)vector2.Y + 16.01)
                                {
                                    int num8 = 1;
                                    if (vector.X + (float)(Width / 2) < vector2.X + 8f)
                                    {
                                        num8 = -1;
                                    }
                                    if (!fireImmune && (type == 37 || type == 58 || type == 76))
                                    {
                                        num5 = 20;
                                    }
                                    if (type == 48)
                                    {
                                        num5 = 40;
                                    }
                                    return new Vector2((float)num8, (float)num5);
                                }
                            }
                        }
                    }
                }
            }
            return new Vector2();
        }

        public static Vector2 TileCollision(Vector2 Position, Vector2 Velocity, World world, int Width, int Height, bool fallThrough = false, bool fall2 = false)
        {
            Vector2 result = Velocity;
            Vector2 vector = Velocity;
            Vector2 vector2 = Position + Velocity;
            Vector2 vector3 = Position;
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            int num5 = -1;
            int num6 = -1;
            int num7 = -1;
            int num8 = -1;
            if (num < 0)
            {
                num = 0;
            }
            if (num2 > world.getMaxTilesX())
            {
                num2 = world.getMaxTilesX();
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > world.getMaxTilesY())
            {
                num4 = world.getMaxTilesY();
            }
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].active && (Statics.tileSolid[(int)world.getTile()[i, j].type] || (Statics.tileSolidTop[(int)world.getTile()[i, j].type] && world.getTile()[i, j].frameY == 0)))
                    {
                        Vector2 vector4 = new Vector2();
                        vector4.X = (float)(i * 16);
                        vector4.Y = (float)(j * 16);
                        if (vector2.X + (float)Width > vector4.X && vector2.X < vector4.X + 16f && vector2.Y + (float)Height > vector4.Y && vector2.Y < vector4.Y + 16f)
                        {
                            if (vector3.Y + (float)Height <= vector4.Y)
                            {
                                if (!Statics.tileSolidTop[(int)world.getTile()[i, j].type] || !fallThrough || (Velocity.Y > 1f && !fall2))
                                {
                                    num7 = i;
                                    num8 = j;
                                    if (num7 != num5)
                                    {
                                        result.Y = vector4.Y - (vector3.Y + (float)Height);
                                    }
                                }
                            }
                            else
                            {
                                if (vector3.X + (float)Width <= vector4.X && !Statics.tileSolidTop[(int)world.getTile()[i, j].type])
                                {
                                    num5 = i;
                                    num6 = j;
                                    if (num6 != num8)
                                    {
                                        result.X = vector4.X - (vector3.X + (float)Width);
                                    }
                                    if (num7 == num5)
                                    {
                                        result.Y = vector.Y;
                                    }
                                }
                                else
                                {
                                    if (vector3.X >= vector4.X + 16f && !Statics.tileSolidTop[(int)world.getTile()[i, j].type])
                                    {
                                        num5 = i;
                                        num6 = j;
                                        if (num6 != num8)
                                        {
                                            result.X = vector4.X + 16f - vector3.X;
                                        }
                                        if (num7 == num5)
                                        {
                                            result.Y = vector.Y;
                                        }
                                    }
                                    else
                                    {
                                        if (vector3.Y >= vector4.Y + 16f && !Statics.tileSolidTop[(int)world.getTile()[i, j].type])
                                        {
                                            num7 = i;
                                            num8 = j;
                                            result.Y = vector4.Y + 16f - vector3.Y + 0.01f;
                                            if (num8 == num6)
                                            {
                                                result.X = vector.X + 0.01f;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static bool LavaCollision(Vector2 Position, World world, int Width, int Height)
        {
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (num < 0)
            {
                num = 0;
            }
            if (num2 > world.getMaxTilesX())
            {
                num2 = world.getMaxTilesX();
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > world.getMaxTilesY())
            {
                num4 = world.getMaxTilesY();
            }
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].liquid > 0 && world.getTile()[i, j].lava)
                    {
                        Vector2 vector = new Vector2();
                        vector.X = (float)(i * 16);
                        vector.Y = (float)(j * 16);
                        int num5 = 16;
                        float num6 = (float)(0 - world.getTile()[i, j].liquid);
                        num6 /= 32f;
                        vector.Y += num6 * 2f;
                        num5 -= (int)(num6 * 2f);
                        if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + (float)num5)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool StickyTiles(Vector2 Position, Vector2 Velocity, World world, int Width, int Height)
        {
            Vector2 vector = Position;
            bool result = false;
            int num = (int)(Position.X / 16f) - 1;
            int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int num3 = (int)(Position.Y / 16f) - 1;
            int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            if (num < 0)
            {
                num = 0;
            }
            if (num2 > world.getMaxTilesX())
            {
                num2 = world.getMaxTilesX();
            }
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > world.getMaxTilesY())
            {
                num4 = world.getMaxTilesY();
            }
            for (int i = num; i < num2; i++)
            {
                for (int j = num3; j < num4; j++)
                {
                    if (world.getTile()[i, j] != null && world.getTile()[i, j].active && world.getTile()[i, j].type == 51)
                    {
                        Vector2 vector2 = new Vector2();
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);
                        if (vector.X + (float)Width > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)Height > vector2.Y && (double)vector.Y < (double)vector2.Y + 16.01)
                        {
                            if ((double)(Math.Abs(Velocity.X) + Math.Abs(Velocity.Y)) > 0.7 && Statics.rand.Next(30) == 0)
                            {
                                Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), world, 16, 16, 30, 0f, 0f, 0, default(Color), 1f);
                            }
                            result = true;
                        }
                    }
                }
            }
            return result;
        }
    
        public static bool SolidTiles(int startX, int endX, int startY, int endY, World world)
		{
			if (startX < 0)
			{
				return true;
			}
			if (endX >= world.getMaxTilesX())
			{
				return true;
			}
			if (startY < 0)
			{
				return true;
			}
			if (endY >= world.getMaxTilesY())
			{
				return true;
			}
			for (int i = startX; i < endX + 1; i++)
			{
				for (int j = startY; j < endY + 1; j++)
				{
					if (world.getTile()[i, j] == null)
					{
						return false;
					}
					if (world.getTile()[i, j].active && Statics.tileSolid[(int)world.getTile()[i, j].type] && !Statics.tileSolidTop[(int)world.getTile()[i, j].type])
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
