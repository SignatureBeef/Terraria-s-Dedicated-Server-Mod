using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static int NewDust(Vector2 Position, World world, int Width, int Height, int Type, float SpeedX = 0f, float SpeedY = 0f, int Alpha = 0, Color newColor = default(Color), float Scale = 1f)
        {
            if (WorldGen.gen)
            {
                return 0;
            }
            if (Statics.netMode == 2)
            {
                return 0;
            }
            int result = 0;
            int i = 0;
            while (i < 2000)
            {
                if (!world.getDust()[i].active)
                {
                    result = i;
                    world.getDust()[i].active = true;
                    world.getDust()[i].type = Type;
                    world.getDust()[i].noGravity = false;
                    world.getDust()[i].color = newColor;
                    world.getDust()[i].alpha = Alpha;
                    world.getDust()[i].position.X = Position.X + (float)Statics.rand.Next(Width - 4) + 4f;
                    world.getDust()[i].position.Y = Position.Y + (float)Statics.rand.Next(Height - 4) + 4f;
                    world.getDust()[i].velocity.X = (float)Statics.rand.Next(-20, 21) * 0.1f + SpeedX;
                    world.getDust()[i].velocity.Y = (float)Statics.rand.Next(-20, 21) * 0.1f + SpeedY;
                    world.getDust()[i].frame.X = 10 * Type;
                    world.getDust()[i].frame.Y = 10 * Statics.rand.Next(3);
                    world.getDust()[i].frame.Width = 8;
                    world.getDust()[i].frame.Height = 8;
                    world.getDust()[i].rotation = 0f;
                    world.getDust()[i].scale = 1f + (float)Statics.rand.Next(-20, 21) * 0.01f;
                    world.getDust()[i].scale *= Scale;
                    world.getDust()[i].noLight = false;
                    if (world.getDust()[i].type == 6 || world.getDust()[i].type == 29)
                    {
                        world.getDust()[i].velocity.Y = (float)Statics.rand.Next(-10, 6) * 0.1f;
                        Dust expr_220_cp_0 = world.getDust()[i];
                        expr_220_cp_0.velocity.X = expr_220_cp_0.velocity.X * 0.3f;
                        world.getDust()[i].scale *= 0.7f;
                    }
                    if (world.getDust()[i].type == 33)
                    {
                        world.getDust()[i].alpha = 170;
                        Dust expr_271 = world.getDust()[i];
                        expr_271.velocity *= 0.5f;
                        Dust expr_292_cp_0 = world.getDust()[i];
                        expr_292_cp_0.velocity.Y = expr_292_cp_0.velocity.Y + 1f;
                    }
                    if (world.getDust()[i].type == 41)
                    {
                        Dust expr_2BA = world.getDust()[i];
                        expr_2BA.velocity *= 0f;
                    }
                    if (world.getDust()[i].type != 34 && world.getDust()[i].type != 35)
                    {
                        break;
                    }
                    Dust expr_2F9 = world.getDust()[i];
                    expr_2F9.velocity *= 0.1f;
                    world.getDust()[i].velocity.Y = -0.5f;
                    if (world.getDust()[i].type == 34 && !Collision.WetCollision(new Vector2(world.getDust()[i].position.X, world.getDust()[i].position.Y - 8f), 4, 4, world))
                    {
                        world.getDust()[i].active = false;
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

        public static void UpdateDust(World world)
        {
            for (int i = 0; i < 2000; i++)
            {
                if (world.getDust()[i].active)
                {
                    Dust expr_1F = world.getDust()[i];
                    expr_1F.position += world.getDust()[i].velocity;
                    if (world.getDust()[i].type == 6 || world.getDust()[i].type == 29)
                    {
                        if (!world.getDust()[i].noGravity)
                        {
                            Dust expr_77_cp_0 = world.getDust()[i];
                            expr_77_cp_0.velocity.Y = expr_77_cp_0.velocity.Y + 0.05f;
                        }
                        if (!world.getDust()[i].noLight)
                        {
                            float num = world.getDust()[i].scale * 1.6f;
                            if (world.getDust()[i].type == 29)
                            {
                                num *= 0.3f;
                            }
                            if (num > 1f)
                            {
                                num = 1f;
                            }
                            //Lighting.addLight((int)(world.getDust()[i].position.X / 16f), (int)(world.getDust()[i].position.Y / 16f), num);
                        }
                    }
                    else
                    {
                        if (world.getDust()[i].type == 14 || world.getDust()[i].type == 16 || world.getDust()[i].type == 31)
                        {
                            Dust expr_149_cp_0 = world.getDust()[i];
                            expr_149_cp_0.velocity.Y = expr_149_cp_0.velocity.Y * 0.98f;
                            Dust expr_166_cp_0 = world.getDust()[i];
                            expr_166_cp_0.velocity.X = expr_166_cp_0.velocity.X * 0.98f;
                        }
                        else
                        {
                            if (world.getDust()[i].type == 32)
                            {
                                world.getDust()[i].scale -= 0.01f;
                                Dust expr_1B0_cp_0 = world.getDust()[i];
                                expr_1B0_cp_0.velocity.X = expr_1B0_cp_0.velocity.X * 0.96f;
                                Dust expr_1CD_cp_0 = world.getDust()[i];
                                expr_1CD_cp_0.velocity.Y = expr_1CD_cp_0.velocity.Y + 0.1f;
                            }
                            else
                            {
                                if (world.getDust()[i].type == 15)
                                {
                                    Dust expr_202_cp_0 = world.getDust()[i];
                                    expr_202_cp_0.velocity.Y = expr_202_cp_0.velocity.Y * 0.98f;
                                    Dust expr_21F_cp_0 = world.getDust()[i];
                                    expr_21F_cp_0.velocity.X = expr_21F_cp_0.velocity.X * 0.98f;
                                    float num2 = world.getDust()[i].scale;
                                    if (num2 > 1f)
                                    {
                                        num2 = 1f;
                                    }
                                    //Lighting.addLight((int)(world.getDust()[i].position.X / 16f), (int)(world.getDust()[i].position.Y / 16f), num2);
                                }
                                else
                                {
                                    if (world.getDust()[i].type == 20 || world.getDust()[i].type == 21)
                                    {
                                        world.getDust()[i].scale += 0.005f;
                                        Dust expr_2CD_cp_0 = world.getDust()[i];
                                        expr_2CD_cp_0.velocity.Y = expr_2CD_cp_0.velocity.Y * 0.94f;
                                        Dust expr_2EA_cp_0 = world.getDust()[i];
                                        expr_2EA_cp_0.velocity.X = expr_2EA_cp_0.velocity.X * 0.94f;
                                        float num3 = world.getDust()[i].scale * 0.8f;
                                        if (world.getDust()[i].type == 21)
                                        {
                                            num3 = world.getDust()[i].scale * 0.4f;
                                        }
                                        if (num3 > 1f)
                                        {
                                            num3 = 1f;
                                        }
                                        //Lighting.addLight((int)(world.getDust()[i].position.X / 16f), (int)(world.getDust()[i].position.Y / 16f), num3);
                                    }
                                    else
                                    {
                                        if (world.getDust()[i].type == 27)
                                        {
                                            Dust expr_394 = world.getDust()[i];
                                            expr_394.velocity *= 0.94f;
                                            world.getDust()[i].scale += 0.002f;
                                            float num4 = world.getDust()[i].scale;
                                            if (num4 > 1f)
                                            {
                                                num4 = 1f;
                                            }
                                            //Lighting.addLight((int)(world.getDust()[i].position.X / 16f), (int)(world.getDust()[i].position.Y / 16f), num4);
                                        }
                                        else
                                        {
                                            if (!world.getDust()[i].noGravity && world.getDust()[i].type != 41)
                                            {
                                                Dust expr_442_cp_0 = world.getDust()[i];
                                                expr_442_cp_0.velocity.Y = expr_442_cp_0.velocity.Y + 0.1f;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (world.getDust()[i].type == 5 && world.getDust()[i].noGravity)
                    {
                        world.getDust()[i].scale -= 0.04f;
                    }
                    if (world.getDust()[i].type == 33)
                    {
                        bool flag = Collision.WetCollision(new Vector2(world.getDust()[i].position.X, world.getDust()[i].position.Y), 4, 4, world);
                        if (flag)
                        {
                            world.getDust()[i].alpha += 20;
                            world.getDust()[i].scale -= 0.1f;
                        }
                        world.getDust()[i].alpha += 2;
                        world.getDust()[i].scale -= 0.005f;
                        if (world.getDust()[i].alpha > 255)
                        {
                            world.getDust()[i].scale = 0f;
                        }
                        Dust expr_558_cp_0 = world.getDust()[i];
                        expr_558_cp_0.velocity.X = expr_558_cp_0.velocity.X * 0.93f;
                        if (world.getDust()[i].velocity.Y > 4f)
                        {
                            world.getDust()[i].velocity.Y = 4f;
                        }
                        if (world.getDust()[i].noGravity)
                        {
                            if (world.getDust()[i].velocity.X < 0f)
                            {
                                world.getDust()[i].rotation -= 0.2f;
                            }
                            else
                            {
                                world.getDust()[i].rotation += 0.2f;
                            }
                            world.getDust()[i].scale += 0.03f;
                            Dust expr_616_cp_0 = world.getDust()[i];
                            expr_616_cp_0.velocity.X = expr_616_cp_0.velocity.X * 1.05f;
                            Dust expr_633_cp_0 = world.getDust()[i];
                            expr_633_cp_0.velocity.Y = expr_633_cp_0.velocity.Y + 0.15f;
                        }
                    }
                    if (world.getDust()[i].type == 35 && world.getDust()[i].noGravity)
                    {
                        world.getDust()[i].scale += 0.02f;
                        if (world.getDust()[i].scale < 1f)
                        {
                            Dust expr_69F_cp_0 = world.getDust()[i];
                            expr_69F_cp_0.velocity.Y = expr_69F_cp_0.velocity.Y + 0.075f;
                        }
                        Dust expr_6BC_cp_0 = world.getDust()[i];
                        expr_6BC_cp_0.velocity.X = expr_6BC_cp_0.velocity.X * 1.08f;
                        if (world.getDust()[i].velocity.X > 0f)
                        {
                            world.getDust()[i].rotation += 0.01f;
                        }
                        else
                        {
                            world.getDust()[i].rotation -= 0.01f;
                        }
                    }
                    else
                    {
                        if (world.getDust()[i].type == 34 || world.getDust()[i].type == 35)
                        {
                            if (!Collision.WetCollision(new Vector2(world.getDust()[i].position.X, world.getDust()[i].position.Y - 8f), 4, 4, world))
                            {
                                world.getDust()[i].scale = 0f;
                            }
                            else
                            {
                                world.getDust()[i].alpha += Statics.rand.Next(2);
                                if (world.getDust()[i].alpha > 255)
                                {
                                    world.getDust()[i].scale = 0f;
                                }
                                world.getDust()[i].velocity.Y = -0.5f;
                                if (world.getDust()[i].type == 34)
                                {
                                    world.getDust()[i].scale += 0.005f;
                                }
                                else
                                {
                                    world.getDust()[i].alpha++;
                                    world.getDust()[i].scale -= 0.01f;
                                    world.getDust()[i].velocity.Y = -0.2f;
                                }
                                Dust expr_862_cp_0 = world.getDust()[i];
                                expr_862_cp_0.velocity.X = expr_862_cp_0.velocity.X + (float)Statics.rand.Next(-10, 10) * 0.002f;
                                if ((double)world.getDust()[i].velocity.X < -0.25)
                                {
                                    world.getDust()[i].velocity.X = -0.25f;
                                }
                                if ((double)world.getDust()[i].velocity.X > 0.25)
                                {
                                    world.getDust()[i].velocity.X = 0.25f;
                                }
                            }
                            if (world.getDust()[i].type == 35)
                            {
                                float num5 = world.getDust()[i].scale * 1.6f;
                                if (num5 > 1f)
                                {
                                    num5 = 1f;
                                }
                                //Lighting.addLight((int)(world.getDust()[i].position.X / 16f), (int)(world.getDust()[i].position.Y / 16f), num5);
                            }
                        }
                    }
                    if (world.getDust()[i].type == 41)
                    {
                        Dust expr_973_cp_0 = world.getDust()[i];
                        expr_973_cp_0.velocity.X = expr_973_cp_0.velocity.X + (float)Statics.rand.Next(-10, 11) * 0.01f;
                        Dust expr_9A0_cp_0 = world.getDust()[i];
                        expr_9A0_cp_0.velocity.Y = expr_9A0_cp_0.velocity.Y + (float)Statics.rand.Next(-10, 11) * 0.01f;
                        if ((double)world.getDust()[i].velocity.X > 0.75)
                        {
                            world.getDust()[i].velocity.X = 0.75f;
                        }
                        if ((double)world.getDust()[i].velocity.X < -0.75)
                        {
                            world.getDust()[i].velocity.X = -0.75f;
                        }
                        if ((double)world.getDust()[i].velocity.Y > 0.75)
                        {
                            world.getDust()[i].velocity.Y = 0.75f;
                        }
                        if ((double)world.getDust()[i].velocity.Y < -0.75)
                        {
                            world.getDust()[i].velocity.Y = -0.75f;
                        }
                        world.getDust()[i].scale += 0.007f;
                        float num6 = world.getDust()[i].scale * 0.7f;
                        if (num6 > 1f)
                        {
                            num6 = 1f;
                        }
                        //Lighting.addLight((int)(world.getDust()[i].position.X / 16f), (int)(world.getDust()[i].position.Y / 16f), num6);
                    }
                    else
                    {
                        Dust expr_B0E_cp_0 = world.getDust()[i];
                        expr_B0E_cp_0.velocity.X = expr_B0E_cp_0.velocity.X * 0.99f;
                    }
                    world.getDust()[i].rotation += world.getDust()[i].velocity.X * 0.5f;
                    world.getDust()[i].scale -= 0.01f;
                    if (world.getDust()[i].noGravity)
                    {
                        Dust expr_B76 = world.getDust()[i];
                        expr_B76.velocity *= 0.92f;
                        world.getDust()[i].scale -= 0.04f;
                    }
                    if (world.getDust()[i].position.Y > Statics.screenPosition.Y + (float)Statics.screenHeight)
                    {
                        world.getDust()[i].active = false;
                    }
                    if ((double)world.getDust()[i].scale < 0.1)
                    {
                        world.getDust()[i].active = false;
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
