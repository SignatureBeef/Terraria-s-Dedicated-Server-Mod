using System;
using Terraria_Server.Misc;
using Terraria_Server.Plugin;
using Terraria_Server.Events;
using Terraria_Server.Commands;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;

namespace Terraria_Server
{
    public class NPC : IRegisterableEntity, ICloneable
    {
        private const int ACTIVE_TIME = 750;
        public const int MAX_NPCS = 1000;
        public const int MAX_AI = 4;

        public static int immuneTime = 20;
        private static int spawnSpaceX = 3;
        private static int spawnSpaceY = 3;
        public static int sWidth = 1680;
        public static int sHeight = 1050;
        private static int spawnRangeX = (int)((double)(NPC.sWidth / 16) * 0.7);
        private static int spawnRangeY = (int)((double)(NPC.sHeight / 16) * 0.7);
        public static int safeRangeX = (int)((double)(NPC.sWidth / 16) * 0.52);
        public static int safeRangeY = (int)((double)(NPC.sHeight / 16) * 0.52);
        private static int activeRangeX = (int)((double)NPC.sWidth * 1.7);
        private static int activeRangeY = (int)((double)NPC.sHeight * 1.7);
        private static int townRangeX = NPC.sWidth;
        private static int townRangeY = NPC.sHeight;
        public static float npcSlots = 1f;
        private static bool noSpawnCycle = false;
        public static int defaultSpawnRate = 600;
        public static int defaultMaxSpawns = 5;
        public static bool downedBoss1 = false;
        public static bool downedBoss2 = false;
        public static bool downedBoss3 = false;
        public static int spawnRate = NPC.defaultSpawnRate;
        public static int maxSpawns = NPC.defaultMaxSpawns;

        public bool Active = true;
        public int alpha;
        public bool behindTiles;
        public bool boss;
        public bool collideX;
        public bool collideY;
        public Color color;
        public int direction;
        public double frameCounter;
        public bool friendly;
        public bool homeless;
        public int homeTileX;
        public int homeTileY;
        public float knockBackResist;
        public bool lavaImmune;
        public bool lavaWet;
        public String Name { get; set; }
        public bool netUpdate;
        public bool noGravity;
        public bool noTileCollide;
        public int oldDirection;
        public int oldTarget;
        public float rotation;
        public float scale;
        public float slots;
        public int soundHit;
        public int soundKilled;
        public int spriteDirection;
        public int target;
        public Rectangle targetRect;
        public int timeLeft;
        public bool townNPC;
        public int Type { get; set; }
        public float value;
        public bool wet;
        public byte wetCount;
        
        public Vector2 Position;
        public Vector2 Velocity;
        public float[] ai = new float[NPC.MAX_AI];
        public int aiAction;
        public int aiStyle;
        public bool closeDoor;
        public int damage;
        public int defense;
        public int directionY = 1;
        public int doorX;
        public int doorY;
        public Rectangle frame;
        public int friendlyRegen;
        public int height;
        public int[] immune = new int[256];
        public int life;
        public int lifeMax;
        public int oldDirectionY;
        public Vector2 oldPosition;
        public Vector2 oldVelocity;
        public int soundDelay;
        public int width;
        public int whoAmI;
        
        public NPC()
        {
            slots = 1f;
            color = default(Color);
            homeTileX = -1;
            homeTileY = -1;
            knockBackResist = 1f;
            Name = "";
            scale = 1f;
            spriteDirection = -1;
            target = 255;
            oldTarget = target;
            this.targetRect = default(Rectangle);
            timeLeft = NPC.ACTIVE_TIME;
            this.Type = Type;
        }

        private int SetGore(int goreType)
        {
            return Gore.NewGore(this.Position, this.Velocity, goreType);
        }

        public static void AI(int index)
        {
            NPC npc = Main.npcs[index];
            if (npc.aiStyle == 0)
            {
                npc.Velocity.X = npc.Velocity.X * 0.93f;
                if ((double)npc.Velocity.X > -0.1 && (double)npc.Velocity.X < 0.1)
                {
                    npc.Velocity.X = 0f;
                    return;
                }
            }
            else
            {
                if (npc.aiStyle == 1)
                {
                    if (npc.Type == 59)
                    {
                        Vector2 arg_D5_0 = new Vector2(npc.Position.X, npc.Position.Y);
                        int arg_D5_1 = npc.width;
                        int arg_D5_2 = npc.height;
                        int arg_D5_3 = 6;
                        float arg_D5_4 = npc.Velocity.X * 0.2f;
                        float arg_D5_5 = npc.Velocity.Y * 0.2f;
                        int arg_D5_6 = 100;
                        Color newColor = default(Color);
                        int num = Dust.NewDust(arg_D5_0, arg_D5_1, arg_D5_2, arg_D5_3, arg_D5_4, arg_D5_5, arg_D5_6, newColor, 1.7f);
                        Main.dust[num].noGravity = true;
                    }
                    if (npc.wet)
                    {
                        if (npc.Type == 59)
                        {
                            if (npc.Velocity.Y > 2f)
                            {
                                npc.Velocity.Y = npc.Velocity.Y * 0.9f;
                            }
                            else
                            {
                                if (npc.directionY < 0)
                                {
                                    npc.Velocity.Y = npc.Velocity.Y - 0.8f;
                                }
                            }
                            npc.Velocity.Y = npc.Velocity.Y - 0.5f;
                            if (npc.Velocity.Y < -10f)
                            {
                                npc.Velocity.Y = -10f;
                            }
                        }
                        else
                        {
                            if (npc.Velocity.Y > 2f)
                            {
                                npc.Velocity.Y = npc.Velocity.Y * 0.9f;
                            }
                            npc.Velocity.Y = npc.Velocity.Y - 0.5f;
                            if (npc.Velocity.Y < -4f)
                            {
                                npc.Velocity.Y = -4f;
                            }
                        }
                        npc.TargetClosest(true);
                    }
                    npc.aiAction = 0;
                    if (npc.ai[2] == 0f)
                    {
                        npc.ai[0] = -100f;
                        npc.ai[2] = 1f;
                        npc.TargetClosest(true);
                    }
                    if (npc.Velocity.Y == 0f)
                    {
                        if (npc.ai[3] == npc.Position.X)
                        {
                            npc.direction *= -1;
                        }
                        npc.ai[3] = 0f;
                        npc.Velocity.X = npc.Velocity.X * 0.8f;
                        if ((double)npc.Velocity.X > -0.1 && (double)npc.Velocity.X < 0.1)
                        {
                            npc.Velocity.X = 0f;
                        }
                        if (!Main.dayTime || npc.life != npc.lifeMax || (double)npc.Position.Y > Main.worldSurface * 16.0)
                        {
                            npc.ai[0] += 1f;
                        }
                        npc.ai[0] += 1f;
                        if (npc.Type == 59)
                        {
                            npc.ai[0] += 2f;
                        }
                        if (npc.ai[0] >= 0f)
                        {
                            npc.netUpdate = true;
                            if (!Main.dayTime || npc.life != npc.lifeMax || (double)npc.Position.Y > Main.worldSurface * 16.0)
                            {
                                npc.TargetClosest(true);
                            }
                            if (npc.ai[1] == 2f)
                            {
                                npc.Velocity.Y = -8f;
                                if (npc.Type == 59)
                                {
                                    npc.Velocity.Y = npc.Velocity.Y - 2f;
                                }
                                npc.Velocity.X = npc.Velocity.X + (float)(3 * npc.direction);
                                if (npc.Type == 59)
                                {
                                    npc.Velocity.X = npc.Velocity.X + 0.5f * (float)npc.direction;
                                }
                                npc.ai[0] = -200f;
                                npc.ai[1] = 0f;
                                npc.ai[3] = npc.Position.X;
                                return;
                            }
                            npc.Velocity.Y = -6f;
                            npc.Velocity.X = npc.Velocity.X + (float)(2 * npc.direction);
                            if (npc.Type == 59)
                            {
                                npc.Velocity.X = npc.Velocity.X + (float)(2 * npc.direction);
                            }
                            npc.ai[0] = -120f;
                            npc.ai[1] += 1f;
                            return;
                        }
                        else if (npc.ai[0] >= -30f)
                        {
                            npc.aiAction = 1;
                            return;
                        }
                    }
                    else if (npc.target < 255 && ((npc.direction == 1 && npc.Velocity.X < 3f) || (npc.direction == -1 && npc.Velocity.X > -3f)))
                    {
                        if ((npc.direction == -1 && (double)npc.Velocity.X < 0.1) || (npc.direction == 1 && (double)npc.Velocity.X > -0.1))
                        {
                            npc.Velocity.X = npc.Velocity.X + 0.2f * (float)npc.direction;
                            return;
                        }
                        npc.Velocity.X = npc.Velocity.X * 0.93f;
                        return;
                    }
                }
                else
                {
                    if (npc.aiStyle == 2)
                    {
                        npc.noGravity = true;
                        if (npc.collideX)
                        {
                            npc.Velocity.X = npc.oldVelocity.X * -0.5f;
                            if (npc.direction == -1 && npc.Velocity.X > 0f && npc.Velocity.X < 2f)
                            {
                                npc.Velocity.X = 2f;
                            }
                            if (npc.direction == 1 && npc.Velocity.X < 0f && npc.Velocity.X > -2f)
                            {
                                npc.Velocity.X = -2f;
                            }
                        }
                        if (npc.collideY)
                        {
                            npc.Velocity.Y = npc.oldVelocity.Y * -0.5f;
                            if (npc.Velocity.Y > 0f && npc.Velocity.Y < 1f)
                            {
                                npc.Velocity.Y = 1f;
                            }
                            if (npc.Velocity.Y < 0f && npc.Velocity.Y > -1f)
                            {
                                npc.Velocity.Y = -1f;
                            }
                        }
                        if (Main.dayTime && (double)npc.Position.Y <= Main.worldSurface * 16.0 && npc.Type == 2)
                        {
                            if (npc.timeLeft > 10)
                            {
                                npc.timeLeft = 10;
                            }
                            npc.directionY = -1;
                            if (npc.Velocity.Y > 0f)
                            {
                                npc.direction = 1;
                            }
                            npc.direction = -1;
                            if (npc.Velocity.X > 0f)
                            {
                                npc.direction = 1;
                            }
                        }
                        else
                        {
                            npc.TargetClosest(true);
                        }
                        if (npc.direction == -1 && npc.Velocity.X > -4f)
                        {
                            npc.Velocity.X = npc.Velocity.X - 0.1f;
                            if (npc.Velocity.X > 4f)
                            {
                                npc.Velocity.X = npc.Velocity.X - 0.1f;
                            }
                            else
                            {
                                if (npc.Velocity.X > 0f)
                                {
                                    npc.Velocity.X = npc.Velocity.X + 0.05f;
                                }
                            }
                            if (npc.Velocity.X < -4f)
                            {
                                npc.Velocity.X = -4f;
                            }
                        }
                        else
                        {
                            if (npc.direction == 1 && npc.Velocity.X < 4f)
                            {
                                npc.Velocity.X = npc.Velocity.X + 0.1f;
                                if (npc.Velocity.X < -4f)
                                {
                                    npc.Velocity.X = npc.Velocity.X + 0.1f;
                                }
                                else
                                {
                                    if (npc.Velocity.X < 0f)
                                    {
                                        npc.Velocity.X = npc.Velocity.X - 0.05f;
                                    }
                                }
                                if (npc.Velocity.X > 4f)
                                {
                                    npc.Velocity.X = 4f;
                                }
                            }
                        }
                        if (npc.directionY == -1 && (double)npc.Velocity.Y > -1.5)
                        {
                            npc.Velocity.Y = npc.Velocity.Y - 0.04f;
                            if ((double)npc.Velocity.Y > 1.5)
                            {
                                npc.Velocity.Y = npc.Velocity.Y - 0.05f;
                            }
                            else
                            {
                                if (npc.Velocity.Y > 0f)
                                {
                                    npc.Velocity.Y = npc.Velocity.Y + 0.03f;
                                }
                            }
                            if ((double)npc.Velocity.Y < -1.5)
                            {
                                npc.Velocity.Y = -1.5f;
                            }
                        }
                        else
                        {
                            if (npc.directionY == 1 && (double)npc.Velocity.Y < 1.5)
                            {
                                npc.Velocity.Y = npc.Velocity.Y + 0.04f;
                                if ((double)npc.Velocity.Y < -1.5)
                                {
                                    npc.Velocity.Y = npc.Velocity.Y + 0.05f;
                                }
                                else
                                {
                                    if (npc.Velocity.Y < 0f)
                                    {
                                        npc.Velocity.Y = npc.Velocity.Y - 0.03f;
                                    }
                                }
                                if ((double)npc.Velocity.Y > 1.5)
                                {
                                    npc.Velocity.Y = 1.5f;
                                }
                            }
                        }
                        if (npc.Type == 2 && Main.rand.Next(40) == 0)
                        {
                            Vector2 arg_ADF_0 = new Vector2(npc.Position.X, npc.Position.Y + (float)npc.height * 0.25f);
                            int arg_ADF_1 = npc.width;
                            int arg_ADF_2 = (int)((float)npc.height * 0.5f);
                            int arg_ADF_3 = 5;
                            float arg_ADF_4 = npc.Velocity.X;
                            float arg_ADF_5 = 2f;
                            int arg_ADF_6 = 0;
                            Color newColor = default(Color);
                            int num2 = Dust.NewDust(arg_ADF_0, arg_ADF_1, arg_ADF_2, arg_ADF_3, arg_ADF_4, arg_ADF_5, arg_ADF_6, newColor, 1f);
                            Dust expr_AF1_cp_0 = Main.dust[num2];
                            expr_AF1_cp_0.velocity.X = expr_AF1_cp_0.velocity.X * 0.5f;
                            Dust expr_B0E_cp_0 = Main.dust[num2];
                            expr_B0E_cp_0.velocity.Y = expr_B0E_cp_0.velocity.Y * 0.1f;
                        }
                        if (npc.wet)
                        {
                            if (npc.Velocity.Y > 0f)
                            {
                                npc.Velocity.Y = npc.Velocity.Y * 0.95f;
                            }
                            npc.Velocity.Y = npc.Velocity.Y - 0.5f;
                            if (npc.Velocity.Y < -4f)
                            {
                                npc.Velocity.Y = -4f;
                            }
                            npc.TargetClosest(true);
                            return;
                        }
                    }
                    else
                    {
                        if (npc.aiStyle == 3)
                        {
                            int num3 = 60;
                            bool flag = false;
                            if (npc.Velocity.Y == 0f && ((npc.Velocity.X > 0f && npc.direction < 0) || (npc.Velocity.X < 0f && npc.direction > 0)))
                            {
                                flag = true;
                            }
                            if (npc.Position.X == npc.oldPosition.X || npc.ai[3] >= (float)num3 || flag)
                            {
                                npc.ai[3] += 1f;
                            }
                            else
                            {
                                if ((double)Math.Abs(npc.Velocity.X) > 0.9 && npc.ai[3] > 0f)
                                {
                                    npc.ai[3] -= 1f;
                                }
                            }
                            if (npc.ai[3] > (float)(num3 * 10))
                            {
                                npc.ai[3] = 0f;
                            }
                            if (npc.ai[3] == (float)num3)
                            {
                                npc.netUpdate = true;
                            }
                            if ((!Main.dayTime || (double)npc.Position.Y > Main.worldSurface * 16.0 || npc.Type == 26 || npc.Type == 27 || npc.Type == 28 || npc.Type == 31 || npc.Type == 47 || npc.Type == 67) && npc.ai[3] < (float)num3)
                            {
                                npc.TargetClosest(true);
                            }
                            else
                            {
                                if (Main.dayTime && (double)(npc.Position.Y / 16f) < Main.worldSurface && npc.timeLeft > 10)
                                {
                                    npc.timeLeft = 10;
                                }
                                if (npc.Velocity.X == 0f)
                                {
                                    if (npc.Velocity.Y == 0f)
                                    {
                                        npc.ai[0] += 1f;
                                        if (npc.ai[0] >= 2f)
                                        {
                                            npc.direction *= -1;
                                            npc.spriteDirection = npc.direction;
                                            npc.ai[0] = 0f;
                                        }
                                    }
                                }
                                else
                                {
                                    npc.ai[0] = 0f;
                                }
                                if (npc.direction == 0)
                                {
                                    npc.direction = 1;
                                }
                            }
                            if (npc.Type == 27)
                            {
                                if (npc.Velocity.X < -2f || npc.Velocity.X > 2f)
                                {
                                    if (npc.Velocity.Y == 0f)
                                    {
                                        npc.Velocity *= 0.8f;
                                    }
                                }
                                else
                                {
                                    if (npc.Velocity.X < 2f && npc.direction == 1)
                                    {
                                        npc.Velocity.X = npc.Velocity.X + 0.07f;
                                        if (npc.Velocity.X > 2f)
                                        {
                                            npc.Velocity.X = 2f;
                                        }
                                    }
                                    else
                                    {
                                        if (npc.Velocity.X > -2f && npc.direction == -1)
                                        {
                                            npc.Velocity.X = npc.Velocity.X - 0.07f;
                                            if (npc.Velocity.X < -2f)
                                            {
                                                npc.Velocity.X = -2f;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (npc.Type == 21 || npc.Type == 26 || npc.Type == 31 || npc.Type == 47)
                                {
                                    if (npc.Velocity.X < -1.5f || npc.Velocity.X > 1.5f)
                                    {
                                        if (npc.Velocity.Y == 0f)
                                        {
                                            npc.Velocity *= 0.8f;
                                        }
                                    }
                                    else
                                    {
                                        if (npc.Velocity.X < 1.5f && npc.direction == 1)
                                        {
                                            npc.Velocity.X = npc.Velocity.X + 0.07f;
                                            if (npc.Velocity.X > 1.5f)
                                            {
                                                npc.Velocity.X = 1.5f;
                                            }
                                        }
                                        else
                                        {
                                            if (npc.Velocity.X > -1.5f && npc.direction == -1)
                                            {
                                                npc.Velocity.X = npc.Velocity.X - 0.07f;
                                                if (npc.Velocity.X < -1.5f)
                                                {
                                                    npc.Velocity.X = -1.5f;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (npc.Type == 67)
                                    {
                                        if (npc.Velocity.X < -0.5f || npc.Velocity.X > 0.5f)
                                        {
                                            if (npc.Velocity.Y == 0f)
                                            {
                                                npc.Velocity *= 0.7f;
                                            }
                                        }
                                        else
                                        {
                                            if (npc.Velocity.X < 0.5f && npc.direction == 1)
                                            {
                                                npc.Velocity.X = npc.Velocity.X + 0.03f;
                                                if (npc.Velocity.X > 0.5f)
                                                {
                                                    npc.Velocity.X = 0.5f;
                                                }
                                            }
                                            else
                                            {
                                                if (npc.Velocity.X > -0.5f && npc.direction == -1)
                                                {
                                                    npc.Velocity.X = npc.Velocity.X - 0.03f;
                                                    if (npc.Velocity.X < -0.5f)
                                                    {
                                                        npc.Velocity.X = -0.5f;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (npc.Velocity.X < -1f || npc.Velocity.X > 1f)
                                        {
                                            if (npc.Velocity.Y == 0f)
                                            {
                                                npc.Velocity *= 0.8f;
                                            }
                                        }
                                        else
                                        {
                                            if (npc.Velocity.X < 1f && npc.direction == 1)
                                            {
                                                npc.Velocity.X = npc.Velocity.X + 0.07f;
                                                if (npc.Velocity.X > 1f)
                                                {
                                                    npc.Velocity.X = 1f;
                                                }
                                            }
                                            else
                                            {
                                                if (npc.Velocity.X > -1f && npc.direction == -1)
                                                {
                                                    npc.Velocity.X = npc.Velocity.X - 0.07f;
                                                    if (npc.Velocity.X < -1f)
                                                    {
                                                        npc.Velocity.X = -1f;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (npc.Velocity.Y != 0f)
                            {
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                return;
                            }
                            int num4 = (int)((npc.Position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
                            int num5 = (int)((npc.Position.Y + (float)npc.height - 15f) / 16f);
                            if (Main.tile[num4, num5] == null)
                            {
                                Main.tile[num4, num5] = new Tile();
                            }
                            if (Main.tile[num4, num5 - 1] == null)
                            {
                                Main.tile[num4, num5 - 1] = new Tile();
                            }
                            if (Main.tile[num4, num5 - 2] == null)
                            {
                                Main.tile[num4, num5 - 2] = new Tile();
                            }
                            if (Main.tile[num4, num5 - 3] == null)
                            {
                                Main.tile[num4, num5 - 3] = new Tile();
                            }
                            if (Main.tile[num4, num5 + 1] == null)
                            {
                                Main.tile[num4, num5 + 1] = new Tile();
                            }
                            if (Main.tile[num4 + npc.direction, num5 - 1] == null)
                            {
                                Main.tile[num4 + npc.direction, num5 - 1] = new Tile();
                            }
                            if (Main.tile[num4 + npc.direction, num5 + 1] == null)
                            {
                                Main.tile[num4 + npc.direction, num5 + 1] = new Tile();
                            }
                            bool flag2 = true;
                            if (npc.Type == 47 || npc.Type == 67)
                            {
                                flag2 = false;
                            }
                            if (Main.tile[num4, num5 - 1].Active && Main.tile[num4, num5 - 1].type == 10 && flag2)
                            {
                                npc.ai[2] += 1f;
                                npc.ai[3] = 0f;
                                if (npc.ai[2] >= 60f)
                                {
                                    if (!Main.bloodMoon && npc.Type == 3)
                                    {
                                        npc.ai[1] = 0f;
                                    }
                                    npc.Velocity.X = 0.5f * (float)(-(float)npc.direction);
                                    npc.ai[1] += 1f;
                                    if (npc.Type == 27)
                                    {
                                        npc.ai[1] += 1f;
                                    }
                                    if (npc.Type == 31)
                                    {
                                        npc.ai[1] += 6f;
                                    }
                                    npc.ai[2] = 0f;
                                    bool flag3 = false;
                                    if (npc.ai[1] >= 10f)
                                    {
                                        flag3 = true;
                                        npc.ai[1] = 10f;
                                    }
                                    WorldGen.KillTile(num4, num5 - 1, true, false, false);
                                    if (flag3)
                                    {
                                        if (npc.Type == 26)
                                        {
                                            WorldGen.KillTile(num4, num5 - 1, false, false, false);
                                            
                                            NetMessage.SendData(17, -1, -1, "", 0, (float)num4, (float)(num5 - 1), 0f, 0);
                                            return;
                                        }
                                        else
                                        {
                                            bool flag4 = WorldGen.OpenDoor(num4, num5, npc.direction, npc.closeDoor, DoorOpener.NPC);
                                            if (!flag4)
                                            {
                                                npc.ai[3] = (float)num3;
                                                npc.netUpdate = true;
                                            }
                                            if (flag4)
                                            {
                                                NetMessage.SendData(19, -1, -1, "", 0, (float)num4, (float)num5, (float)npc.direction, 0);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if ((npc.Velocity.X < 0f && npc.spriteDirection == -1) || (npc.Velocity.X > 0f && npc.spriteDirection == 1))
                                {
                                    if (Main.tile[num4, num5 - 2].Active && Main.tileSolid[(int)Main.tile[num4, num5 - 2].type])
                                    {
                                        if (Main.tile[num4, num5 - 3].Active && Main.tileSolid[(int)Main.tile[num4, num5 - 3].type])
                                        {
                                            npc.Velocity.Y = -8f;
                                            npc.netUpdate = true;
                                        }
                                        else
                                        {
                                            npc.Velocity.Y = -7f;
                                            npc.netUpdate = true;
                                        }
                                    }
                                    else
                                    {
                                        if (Main.tile[num4, num5 - 1].Active && Main.tileSolid[(int)Main.tile[num4, num5 - 1].type])
                                        {
                                            npc.Velocity.Y = -6f;
                                            npc.netUpdate = true;
                                        }
                                        else
                                        {
                                            if (Main.tile[num4, num5].Active && Main.tileSolid[(int)Main.tile[num4, num5].type])
                                            {
                                                npc.Velocity.Y = -5f;
                                                npc.netUpdate = true;
                                            }
                                            else
                                            {
                                                if (npc.directionY < 0 && npc.Type != 67 && (!Main.tile[num4, num5 + 1].Active || !Main.tileSolid[(int)Main.tile[num4, num5 + 1].type]) && (!Main.tile[num4 + npc.direction, num5 + 1].Active || !Main.tileSolid[(int)Main.tile[num4 + npc.direction, num5 + 1].type]))
                                                {
                                                    npc.Velocity.Y = -8f;
                                                    npc.Velocity.X = npc.Velocity.X * 1.5f;
                                                    npc.netUpdate = true;
                                                }
                                                else
                                                {
                                                    npc.ai[1] = 0f;
                                                    npc.ai[2] = 0f;
                                                }
                                            }
                                        }
                                    }
                                }
                                if ((npc.Type == 31 || npc.Type == 47) && npc.Velocity.Y == 0f && Math.Abs(npc.Position.X + (float)(npc.width / 2) - (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2))) < 100f && Math.Abs(npc.Position.Y + (float)(npc.height / 2) - (Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2))) < 50f && ((npc.direction > 0 && npc.Velocity.X >= 1f) || (npc.direction < 0 && npc.Velocity.X <= -1f)))
                                {
                                    npc.Velocity.X = npc.Velocity.X * 2f;
                                    if (npc.Velocity.X > 3f)
                                    {
                                        npc.Velocity.X = 3f;
                                    }
                                    if (npc.Velocity.X < -3f)
                                    {
                                        npc.Velocity.X = -3f;
                                    }
                                    npc.Velocity.Y = -4f;
                                    npc.netUpdate = true;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (npc.aiStyle == 4)
                            {
                                if (npc.target < 0 || npc.target == 255 || Main.players[npc.target].dead || !Main.players[npc.target].Active)
                                {
                                    npc.TargetClosest(true);
                                }
                                bool dead = Main.players[npc.target].dead;
                                float num6 = npc.Position.X + (float)(npc.width / 2) - Main.players[npc.target].Position.X - (float)(Main.players[npc.target].width / 2);
                                float num7 = npc.Position.Y + (float)npc.height - 59f - Main.players[npc.target].Position.Y - (float)(Main.players[npc.target].height / 2);
                                float num8 = (float)Math.Atan2((double)num7, (double)num6) + 1.57f;
                                if (num8 < 0f)
                                {
                                    num8 += 6.283f;
                                }
                                else
                                {
                                    if ((double)num8 > 6.283)
                                    {
                                        num8 -= 6.283f;
                                    }
                                }
                                float num9 = 0f;
                                if (npc.ai[0] == 0f && npc.ai[1] == 0f)
                                {
                                    num9 = 0.02f;
                                }
                                if (npc.ai[0] == 0f && npc.ai[1] == 2f && npc.ai[2] > 40f)
                                {
                                    num9 = 0.05f;
                                }
                                if (npc.ai[0] == 3f && npc.ai[1] == 0f)
                                {
                                    num9 = 0.05f;
                                }
                                if (npc.ai[0] == 3f && npc.ai[1] == 2f && npc.ai[2] > 40f)
                                {
                                    num9 = 0.08f;
                                }
                                if (npc.rotation < num8)
                                {
                                    if ((double)(num8 - npc.rotation) > 3.1415)
                                    {
                                        npc.rotation -= num9;
                                    }
                                    else
                                    {
                                        npc.rotation += num9;
                                    }
                                }
                                else
                                {
                                    if (npc.rotation > num8)
                                    {
                                        if ((double)(npc.rotation - num8) > 3.1415)
                                        {
                                            npc.rotation += num9;
                                        }
                                        else
                                        {
                                            npc.rotation -= num9;
                                        }
                                    }
                                }
                                if (npc.rotation > num8 - num9 && npc.rotation < num8 + num9)
                                {
                                    npc.rotation = num8;
                                }
                                if (npc.rotation < 0f)
                                {
                                    npc.rotation += 6.283f;
                                }
                                else
                                {
                                    if ((double)npc.rotation > 6.283)
                                    {
                                        npc.rotation -= 6.283f;
                                    }
                                }
                                if (npc.rotation > num8 - num9 && npc.rotation < num8 + num9)
                                {
                                    npc.rotation = num8;
                                }
                                if (Main.rand.Next(5) == 0)
                                {
                                    Vector2 arg_1DAB_0 = new Vector2(npc.Position.X, npc.Position.Y + (float)npc.height * 0.25f);
                                    int arg_1DAB_1 = npc.width;
                                    int arg_1DAB_2 = (int)((float)npc.height * 0.5f);
                                    int arg_1DAB_3 = 5;
                                    float arg_1DAB_4 = npc.Velocity.X;
                                    float arg_1DAB_5 = 2f;
                                    int arg_1DAB_6 = 0;
                                    Color newColor = default(Color);
                                    int num10 = Dust.NewDust(arg_1DAB_0, arg_1DAB_1, arg_1DAB_2, arg_1DAB_3, arg_1DAB_4, arg_1DAB_5, arg_1DAB_6, newColor, 1f);
                                    Dust expr_1DBF_cp_0 = Main.dust[num10];
                                    expr_1DBF_cp_0.velocity.X = expr_1DBF_cp_0.velocity.X * 0.5f;
                                    Dust expr_1DDD_cp_0 = Main.dust[num10];
                                    expr_1DDD_cp_0.velocity.Y = expr_1DDD_cp_0.velocity.Y * 0.1f;
                                }
                                if (Main.dayTime || dead)
                                {
                                    npc.Velocity.Y = npc.Velocity.Y - 0.04f;
                                    if (npc.timeLeft > 10)
                                    {
                                        npc.timeLeft = 10;
                                        return;
                                    }
                                }
                                else
                                {
                                    if (npc.ai[0] == 0f)
                                    {
                                        if (npc.ai[1] == 0f)
                                        {
                                            float num11 = 5f;
                                            float num12 = 0.04f;
                                            Vector2 vector = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                            float num13 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector.X;
                                            float num14 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - 200f - vector.Y;
                                            float num15 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                                            float num16 = num15;
                                            num15 = num11 / num15;
                                            num13 *= num15;
                                            num14 *= num15;
                                            if (npc.Velocity.X < num13)
                                            {
                                                npc.Velocity.X = npc.Velocity.X + num12;
                                                if (npc.Velocity.X < 0f && num13 > 0f)
                                                {
                                                    npc.Velocity.X = npc.Velocity.X + num12;
                                                }
                                            }
                                            else
                                            {
                                                if (npc.Velocity.X > num13)
                                                {
                                                    npc.Velocity.X = npc.Velocity.X - num12;
                                                    if (npc.Velocity.X > 0f && num13 < 0f)
                                                    {
                                                        npc.Velocity.X = npc.Velocity.X - num12;
                                                    }
                                                }
                                            }
                                            if (npc.Velocity.Y < num14)
                                            {
                                                npc.Velocity.Y = npc.Velocity.Y + num12;
                                                if (npc.Velocity.Y < 0f && num14 > 0f)
                                                {
                                                    npc.Velocity.Y = npc.Velocity.Y + num12;
                                                }
                                            }
                                            else
                                            {
                                                if (npc.Velocity.Y > num14)
                                                {
                                                    npc.Velocity.Y = npc.Velocity.Y - num12;
                                                    if (npc.Velocity.Y > 0f && num14 < 0f)
                                                    {
                                                        npc.Velocity.Y = npc.Velocity.Y - num12;
                                                    }
                                                }
                                            }
                                            npc.ai[2] += 1f;
                                            if (npc.ai[2] >= 600f)
                                            {
                                                npc.ai[1] = 1f;
                                                npc.ai[2] = 0f;
                                                npc.ai[3] = 0f;
                                                npc.target = 255;
                                                npc.netUpdate = true;
                                            }
                                            else
                                            {
                                                if (npc.Position.Y + (float)npc.height < Main.players[npc.target].Position.Y && num16 < 500f)
                                                {
                                                    if (!Main.players[npc.target].dead)
                                                    {
                                                        npc.ai[3] += 1f;
                                                    }
                                                    if (npc.ai[3] >= 90f)
                                                    {
                                                        npc.ai[3] = 0f;
                                                        npc.rotation = num8;
                                                        float num17 = 5f;
                                                        float num18 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector.X;
                                                        float num19 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - vector.Y;
                                                        float num20 = (float)Math.Sqrt((double)(num18 * num18 + num19 * num19));
                                                        num20 = num17 / num20;
                                                        Vector2 vector2 = vector;
                                                        Vector2 vector3;
                                                        vector3.X = num18 * num20;
                                                        vector3.Y = num19 * num20;
                                                        vector2.X += vector3.X * 10f;
                                                        vector2.Y += vector3.Y * 10f;
                                                        
                                                        int npcIndex = NPC.NewNPC((int)vector2.X, (int)vector2.Y, 5, 0);
                                                        Main.npcs[npcIndex].Velocity.X = vector3.X;
                                                        Main.npcs[npcIndex].Velocity.Y = vector3.Y;
                                                        if (npcIndex < MAX_NPCS)
                                                        {
                                                            NetMessage.SendData(23, -1, -1, "", npcIndex);
                                                        }

                                                        for (int i = 0; i < 10; i++)
                                                        {
                                                            Vector2 arg_2324_0 = vector2;
                                                            int arg_2324_1 = 20;
                                                            int arg_2324_2 = 20;
                                                            int arg_2324_3 = 5;
                                                            float arg_2324_4 = vector3.X * 0.4f;
                                                            float arg_2324_5 = vector3.Y * 0.4f;
                                                            int arg_2324_6 = 0;
                                                            Color newColor = default(Color);
                                                            Dust.NewDust(arg_2324_0, arg_2324_1, arg_2324_2, arg_2324_3, arg_2324_4, arg_2324_5, arg_2324_6, newColor, 1f);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (npc.ai[1] == 1f)
                                            {
                                                npc.rotation = num8;
                                                float num22 = 7f;
                                                Vector2 vector4 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                float num23 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector4.X;
                                                float num24 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - vector4.Y;
                                                float num25 = (float)Math.Sqrt((double)(num23 * num23 + num24 * num24));
                                                num25 = num22 / num25;
                                                npc.Velocity.X = num23 * num25;
                                                npc.Velocity.Y = num24 * num25;
                                                npc.ai[1] = 2f;
                                            }
                                            else
                                            {
                                                if (npc.ai[1] == 2f)
                                                {
                                                    npc.ai[2] += 1f;
                                                    if (npc.ai[2] >= 40f)
                                                    {
                                                        npc.Velocity.X = npc.Velocity.X * 0.98f;
                                                        npc.Velocity.Y = npc.Velocity.Y * 0.98f;
                                                        if ((double)npc.Velocity.X > -0.1 && (double)npc.Velocity.X < 0.1)
                                                        {
                                                            npc.Velocity.X = 0f;
                                                        }
                                                        if ((double)npc.Velocity.Y > -0.1 && (double)npc.Velocity.Y < 0.1)
                                                        {
                                                            npc.Velocity.Y = 0f;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) - 1.57f;
                                                    }
                                                    if (npc.ai[2] >= 120f)
                                                    {
                                                        npc.ai[3] += 1f;
                                                        npc.ai[2] = 0f;
                                                        npc.target = 255;
                                                        npc.rotation = num8;
                                                        if (npc.ai[3] >= 3f)
                                                        {
                                                            npc.ai[1] = 0f;
                                                            npc.ai[3] = 0f;
                                                        }
                                                        else
                                                        {
                                                            npc.ai[1] = 1f;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if ((double)npc.life < (double)npc.lifeMax * 0.5)
                                        {
                                            npc.ai[0] = 1f;
                                            npc.ai[1] = 0f;
                                            npc.ai[2] = 0f;
                                            npc.ai[3] = 0f;
                                            npc.netUpdate = true;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (npc.ai[0] == 1f || npc.ai[0] == 2f)
                                        {
                                            if (npc.ai[0] == 1f)
                                            {
                                                npc.ai[2] += 0.005f;
                                                if ((double)npc.ai[2] > 0.5)
                                                {
                                                    npc.ai[2] = 0.5f;
                                                }
                                            }
                                            else
                                            {
                                                npc.ai[2] -= 0.005f;
                                                if (npc.ai[2] < 0f)
                                                {
                                                    npc.ai[2] = 0f;
                                                }
                                            }
                                            npc.rotation += npc.ai[2];
                                            npc.ai[1] += 1f;
                                            Color newColor;
                                            if (npc.ai[1] == 100f)
                                            {
                                                npc.ai[0] += 1f;
                                                npc.ai[1] = 0f;
                                                if (npc.ai[0] == 3f)
                                                {
                                                    npc.ai[2] = 0f;
                                                }
                                                else
                                                {
                                                    for (int j = 0; j < 2; j++)
                                                    {
                                                        Gore.NewGore(npc.Position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 8);
                                                        Gore.NewGore(npc.Position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7);
                                                        Gore.NewGore(npc.Position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 6);
                                                    }
                                                    for (int k = 0; k < 20; k++)
                                                    {
                                                        Vector2 arg_28B3_0 = npc.Position;
                                                        int arg_28B3_1 = npc.width;
                                                        int arg_28B3_2 = npc.height;
                                                        int arg_28B3_3 = 5;
                                                        float arg_28B3_4 = (float)Main.rand.Next(-30, 31) * 0.2f;
                                                        float arg_28B3_5 = (float)Main.rand.Next(-30, 31) * 0.2f;
                                                        int arg_28B3_6 = 0;
                                                        newColor = default(Color);
                                                        Dust.NewDust(arg_28B3_0, arg_28B3_1, arg_28B3_2, arg_28B3_3, arg_28B3_4, arg_28B3_5, arg_28B3_6, newColor, 1f);
                                                    }
                                                }
                                            }
                                            Vector2 arg_2932_0 = npc.Position;
                                            int arg_2932_1 = npc.width;
                                            int arg_2932_2 = npc.height;
                                            int arg_2932_3 = 5;
                                            float arg_2932_4 = (float)Main.rand.Next(-30, 31) * 0.2f;
                                            float arg_2932_5 = (float)Main.rand.Next(-30, 31) * 0.2f;
                                            int arg_2932_6 = 0;
                                            newColor = default(Color);
                                            Dust.NewDust(arg_2932_0, arg_2932_1, arg_2932_2, arg_2932_3, arg_2932_4, arg_2932_5, arg_2932_6, newColor, 1f);
                                            npc.Velocity.X = npc.Velocity.X * 0.98f;
                                            npc.Velocity.Y = npc.Velocity.Y * 0.98f;
                                            if ((double)npc.Velocity.X > -0.1 && (double)npc.Velocity.X < 0.1)
                                            {
                                                npc.Velocity.X = 0f;
                                            }
                                            if ((double)npc.Velocity.Y > -0.1 && (double)npc.Velocity.Y < 0.1)
                                            {
                                                npc.Velocity.Y = 0f;
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            npc.damage = 30;
                                            npc.defense = 6;
                                            if (npc.ai[1] == 0f)
                                            {
                                                float num26 = 6f;
                                                float num27 = 0.07f;
                                                Vector2 vector5 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                float num28 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector5.X;
                                                float num29 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - 120f - vector5.Y;
                                                float num30 = (float)Math.Sqrt((double)(num28 * num28 + num29 * num29));
                                                num30 = num26 / num30;
                                                num28 *= num30;
                                                num29 *= num30;
                                                if (npc.Velocity.X < num28)
                                                {
                                                    npc.Velocity.X = npc.Velocity.X + num27;
                                                    if (npc.Velocity.X < 0f && num28 > 0f)
                                                    {
                                                        npc.Velocity.X = npc.Velocity.X + num27;
                                                    }
                                                }
                                                else
                                                {
                                                    if (npc.Velocity.X > num28)
                                                    {
                                                        npc.Velocity.X = npc.Velocity.X - num27;
                                                        if (npc.Velocity.X > 0f && num28 < 0f)
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X - num27;
                                                        }
                                                    }
                                                }
                                                if (npc.Velocity.Y < num29)
                                                {
                                                    npc.Velocity.Y = npc.Velocity.Y + num27;
                                                    if (npc.Velocity.Y < 0f && num29 > 0f)
                                                    {
                                                        npc.Velocity.Y = npc.Velocity.Y + num27;
                                                    }
                                                }
                                                else
                                                {
                                                    if (npc.Velocity.Y > num29)
                                                    {
                                                        npc.Velocity.Y = npc.Velocity.Y - num27;
                                                        if (npc.Velocity.Y > 0f && num29 < 0f)
                                                        {
                                                            npc.Velocity.Y = npc.Velocity.Y - num27;
                                                        }
                                                    }
                                                }
                                                npc.ai[2] += 1f;
                                                if (npc.ai[2] >= 200f)
                                                {
                                                    npc.ai[1] = 1f;
                                                    npc.ai[2] = 0f;
                                                    npc.ai[3] = 0f;
                                                    npc.target = 255;
                                                    npc.netUpdate = true;
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                if (npc.ai[1] == 1f)
                                                {
                                                    npc.rotation = num8;
                                                    float num31 = 8f;
                                                    Vector2 vector6 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                    float num32 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector6.X;
                                                    float num33 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - vector6.Y;
                                                    float num34 = (float)Math.Sqrt((double)(num32 * num32 + num33 * num33));
                                                    num34 = num31 / num34;
                                                    npc.Velocity.X = num32 * num34;
                                                    npc.Velocity.Y = num33 * num34;
                                                    npc.ai[1] = 2f;
                                                    return;
                                                }
                                                if (npc.ai[1] == 2f)
                                                {
                                                    npc.ai[2] += 1f;
                                                    if (npc.ai[2] >= 40f)
                                                    {
                                                        npc.Velocity.X = npc.Velocity.X * 0.97f;
                                                        npc.Velocity.Y = npc.Velocity.Y * 0.97f;
                                                        if ((double)npc.Velocity.X > -0.1 && (double)npc.Velocity.X < 0.1)
                                                        {
                                                            npc.Velocity.X = 0f;
                                                        }
                                                        if ((double)npc.Velocity.Y > -0.1 && (double)npc.Velocity.Y < 0.1)
                                                        {
                                                            npc.Velocity.Y = 0f;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) - 1.57f;
                                                    }
                                                    if (npc.ai[2] >= 100f)
                                                    {
                                                        npc.ai[3] += 1f;
                                                        npc.ai[2] = 0f;
                                                        npc.target = 255;
                                                        npc.rotation = num8;
                                                        if (npc.ai[3] >= 3f)
                                                        {
                                                            npc.ai[1] = 0f;
                                                            npc.ai[3] = 0f;
                                                            return;
                                                        }
                                                        npc.ai[1] = 1f;
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (npc.aiStyle == 5)
                                {
                                    if (npc.target < 0 || npc.target == 255 || Main.players[npc.target].dead)
                                    {
                                        npc.TargetClosest(true);
                                    }
                                    float num35 = 6f;
                                    float num36 = 0.05f;
                                    if (npc.Type == 6 || npc.Type == 42)
                                    {
                                        num35 = 4f;
                                        num36 = 0.02f;
                                    }
                                    else
                                    {
                                        if (npc.Type == 23)
                                        {
                                            num35 = 2f;
                                            num36 = 0.03f;
                                        }
                                    }
                                    Vector2 vector7 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                    float num37 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector7.X;
                                    float num38 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - vector7.Y;
                                    float num39 = (float)Math.Sqrt((double)(num37 * num37 + num38 * num38));
                                    float num40 = num39;
                                    num39 = num35 / num39;
                                    num37 *= num39;
                                    num38 *= num39;
                                    if (npc.Type == 6 || npc.Type == 42)
                                    {
                                        if (num40 > 100f)
                                        {
                                            npc.ai[0] += 1f;
                                            if (npc.ai[0] > 0f)
                                            {
                                                npc.Velocity.Y = npc.Velocity.Y + 0.023f;
                                            }
                                            else
                                            {
                                                npc.Velocity.Y = npc.Velocity.Y - 0.023f;
                                            }
                                            if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                                            {
                                                npc.Velocity.X = npc.Velocity.X + 0.023f;
                                            }
                                            else
                                            {
                                                npc.Velocity.X = npc.Velocity.X - 0.023f;
                                            }
                                            if (npc.ai[0] > 200f)
                                            {
                                                npc.ai[0] = -200f;
                                            }
                                        }
                                        if (num40 < 150f)
                                        {
                                            npc.Velocity.X = npc.Velocity.X + num37 * 0.007f;
                                            npc.Velocity.Y = npc.Velocity.Y + num38 * 0.007f;
                                        }
                                    }
                                    if (Main.players[npc.target].dead)
                                    {
                                        num37 = (float)npc.direction * num35 / 2f;
                                        num38 = -num35 / 2f;
                                    }
                                    if (npc.Velocity.X < num37)
                                    {
                                        npc.Velocity.X = npc.Velocity.X + num36;
                                        if (npc.Type != 6 && npc.Velocity.X < 0f && num37 > 0f)
                                        {
                                            npc.Velocity.X = npc.Velocity.X + num36;
                                        }
                                    }
                                    else
                                    {
                                        if (npc.Velocity.X > num37)
                                        {
                                            npc.Velocity.X = npc.Velocity.X - num36;
                                            if (npc.Type != 6 && npc.Velocity.X > 0f && num37 < 0f)
                                            {
                                                npc.Velocity.X = npc.Velocity.X - num36;
                                            }
                                        }
                                    }
                                    if (npc.Velocity.Y < num38)
                                    {
                                        npc.Velocity.Y = npc.Velocity.Y + num36;
                                        if (npc.Type != 6 && npc.Velocity.Y < 0f && num38 > 0f)
                                        {
                                            npc.Velocity.Y = npc.Velocity.Y + num36;
                                        }
                                    }
                                    else
                                    {
                                        if (npc.Velocity.Y > num38)
                                        {
                                            npc.Velocity.Y = npc.Velocity.Y - num36;
                                            if (npc.Type != 6 && npc.Velocity.Y > 0f && num38 < 0f)
                                            {
                                                npc.Velocity.Y = npc.Velocity.Y - num36;
                                            }
                                        }
                                    }
                                    if (npc.Type == 23)
                                    {
                                        if (num37 > 0f)
                                        {
                                            npc.spriteDirection = 1;
                                            npc.rotation = (float)Math.Atan2((double)num38, (double)num37);
                                        }
                                        else
                                        {
                                            if (num37 < 0f)
                                            {
                                                npc.spriteDirection = -1;
                                                npc.rotation = (float)Math.Atan2((double)num38, (double)num37) + 3.14f;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (npc.Type == 6)
                                        {
                                            npc.rotation = (float)Math.Atan2((double)num38, (double)num37) - 1.57f;
                                        }
                                        else
                                        {
                                            if (npc.Type == 42)
                                            {
                                                if (npc.Velocity.X > 0f)
                                                {
                                                    npc.spriteDirection = 1;
                                                }
                                                if (npc.Velocity.X < 0f)
                                                {
                                                    npc.spriteDirection = -1;
                                                }
                                                npc.rotation = npc.Velocity.X * 0.1f;
                                            }
                                            else
                                            {
                                                npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) - 1.57f;
                                            }
                                        }
                                    }
                                    if (npc.Type == 6 || npc.Type == 23 || npc.Type == 42)
                                    {
                                        float num41 = 0.7f;
                                        if (npc.Type == 6)
                                        {
                                            num41 = 0.4f;
                                        }
                                        if (npc.collideX)
                                        {
                                            npc.netUpdate = true;
                                            npc.Velocity.X = npc.oldVelocity.X * -num41;
                                            if (npc.direction == -1 && npc.Velocity.X > 0f && npc.Velocity.X < 2f)
                                            {
                                                npc.Velocity.X = 2f;
                                            }
                                            if (npc.direction == 1 && npc.Velocity.X < 0f && npc.Velocity.X > -2f)
                                            {
                                                npc.Velocity.X = -2f;
                                            }
                                            npc.netUpdate = true;
                                        }
                                        if (npc.collideY)
                                        {
                                            npc.netUpdate = true;
                                            npc.Velocity.Y = npc.oldVelocity.Y * -num41;
                                            if (npc.Velocity.Y > 0f && (double)npc.Velocity.Y < 1.5)
                                            {
                                                npc.Velocity.Y = 2f;
                                            }
                                            if (npc.Velocity.Y < 0f && (double)npc.Velocity.Y > -1.5)
                                            {
                                                npc.Velocity.Y = -2f;
                                            }
                                        }
                                        if (npc.Type == 23)
                                        {
                                            Vector2 arg_368F_0 = new Vector2(npc.Position.X - npc.Velocity.X, npc.Position.Y - npc.Velocity.Y);
                                            int arg_368F_1 = npc.width;
                                            int arg_368F_2 = npc.height;
                                            int arg_368F_3 = 6;
                                            float arg_368F_4 = npc.Velocity.X * 0.2f;
                                            float arg_368F_5 = npc.Velocity.Y * 0.2f;
                                            int arg_368F_6 = 100;
                                            Color newColor = default(Color);
                                            int num42 = Dust.NewDust(arg_368F_0, arg_368F_1, arg_368F_2, arg_368F_3, arg_368F_4, arg_368F_5, arg_368F_6, newColor, 2f);
                                            Main.dust[num42].noGravity = true;
                                            Dust expr_36B1_cp_0 = Main.dust[num42];
                                            expr_36B1_cp_0.velocity.X = expr_36B1_cp_0.velocity.X * 0.3f;
                                            Dust expr_36CF_cp_0 = Main.dust[num42];
                                            expr_36CF_cp_0.velocity.Y = expr_36CF_cp_0.velocity.Y * 0.3f;
                                        }
                                        else
                                        {
                                            if (Main.rand.Next(20) == 0)
                                            {
                                                int num43 = Dust.NewDust(new Vector2(npc.Position.X, npc.Position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 18, npc.Velocity.X, 2f, npc.alpha, npc.color, npc.scale);
                                                Dust expr_376B_cp_0 = Main.dust[num43];
                                                expr_376B_cp_0.velocity.X = expr_376B_cp_0.velocity.X * 0.5f;
                                                Dust expr_3789_cp_0 = Main.dust[num43];
                                                expr_3789_cp_0.velocity.Y = expr_3789_cp_0.velocity.Y * 0.1f;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Main.rand.Next(40) == 0)
                                        {
                                            Vector2 arg_380E_0 = new Vector2(npc.Position.X, npc.Position.Y + (float)npc.height * 0.25f);
                                            int arg_380E_1 = npc.width;
                                            int arg_380E_2 = (int)((float)npc.height * 0.5f);
                                            int arg_380E_3 = 5;
                                            float arg_380E_4 = npc.Velocity.X;
                                            float arg_380E_5 = 2f;
                                            int arg_380E_6 = 0;
                                            Color newColor = default(Color);
                                            int num44 = Dust.NewDust(arg_380E_0, arg_380E_1, arg_380E_2, arg_380E_3, arg_380E_4, arg_380E_5, arg_380E_6, newColor, 1f);
                                            Dust expr_3822_cp_0 = Main.dust[num44];
                                            expr_3822_cp_0.velocity.X = expr_3822_cp_0.velocity.X * 0.5f;
                                            Dust expr_3840_cp_0 = Main.dust[num44];
                                            expr_3840_cp_0.velocity.Y = expr_3840_cp_0.velocity.Y * 0.1f;
                                        }
                                    }
                                    if ((Main.dayTime && npc.Type != 6 && npc.Type != 23 && npc.Type != 42) || Main.players[npc.target].dead)
                                    {
                                        npc.Velocity.Y = npc.Velocity.Y - num36 * 2f;
                                        if (npc.timeLeft > 10)
                                        {
                                            npc.timeLeft = 10;
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    if (npc.aiStyle == 6)
                                    {
                                        if (npc.target < 0 || npc.target == 255 || Main.players[npc.target].dead)
                                        {
                                            npc.TargetClosest(true);
                                        }
                                        if (Main.players[npc.target].dead && npc.timeLeft > 10)
                                        {
                                            npc.timeLeft = 10;
                                        }
                                        if ((npc.Type == 7 || npc.Type == 8 || npc.Type == 10 || npc.Type == 11 || npc.Type == 13 || npc.Type == 14 || npc.Type == 39 || npc.Type == 40) && npc.ai[0] == 0f)
                                        {
                                            if (npc.Type == 7 || npc.Type == 10 || npc.Type == 13 || npc.Type == 39)
                                            {
                                                npc.ai[2] = 10f;
                                                if (npc.Type == 10)
                                                {
                                                    npc.ai[2] = 5f;
                                                }
                                                if (npc.Type == 13)
                                                {
                                                    npc.ai[2] = 50f;
                                                }
                                                if (npc.Type == 39)
                                                {
                                                    npc.ai[2] = 15f;
                                                }
                                                npc.ai[0] = (float)NPC.NewNPC((int)(npc.Position.X + (float)(npc.width / 2)), (int)(npc.Position.Y + (float)npc.height), npc.Type + 1, npc.whoAmI);
                                            }
                                            else
                                            {
                                                if ((npc.Type == 8 || npc.Type == 11 || npc.Type == 14 || npc.Type == 40) && npc.ai[2] > 0f)
                                                {
                                                    npc.ai[0] = (float)NPC.NewNPC((int)(npc.Position.X + (float)(npc.width / 2)), (int)(npc.Position.Y + (float)npc.height), npc.Type, npc.whoAmI);
                                                }
                                                else
                                                {
                                                    npc.ai[0] = (float)NPC.NewNPC((int)(npc.Position.X + (float)(npc.width / 2)), (int)(npc.Position.Y + (float)npc.height), npc.Type + 1, npc.whoAmI);
                                                }
                                            }
                                            Main.npcs[(int)npc.ai[0]].ai[1] = (float)npc.whoAmI;
                                            Main.npcs[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;
                                            npc.netUpdate = true;
                                        }
                                        if ((npc.Type == 8 || npc.Type == 9 || npc.Type == 11 || npc.Type == 12 || npc.Type == 40 || npc.Type == 41) && (!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != npc.aiStyle))
                                        {
                                            npc.life = 0;
                                            npc.HitEffect(0, 10.0);
                                            npc.Active = false;
                                        }
                                        if ((npc.Type == 7 || npc.Type == 8 || npc.Type == 10 || npc.Type == 11 || npc.Type == 39 || npc.Type == 40) && !Main.npcs[(int)npc.ai[0]].Active)
                                        {
                                            npc.life = 0;
                                            npc.HitEffect(0, 10.0);
                                            npc.Active = false;
                                        }
                                        if (npc.Type == 13 || npc.Type == 14 || npc.Type == 15)
                                        {
                                            if (!Main.npcs[(int)npc.ai[1]].Active && !Main.npcs[(int)npc.ai[0]].Active)
                                            {
                                                npc.life = 0;
                                                npc.HitEffect(0, 10.0);
                                                npc.Active = false;
                                            }
                                            if (npc.Type == 13 && !Main.npcs[(int)npc.ai[0]].Active)
                                            {
                                                npc.life = 0;
                                                npc.HitEffect(0, 10.0);
                                                npc.Active = false;
                                            }
                                            if (npc.Type == 15 && !Main.npcs[(int)npc.ai[1]].Active)
                                            {
                                                npc.life = 0;
                                                npc.HitEffect(0, 10.0);
                                                npc.Active = false;
                                            }
                                            if (npc.Type == 14 && !Main.npcs[(int)npc.ai[1]].Active)
                                            {
                                                int num45 = npc.whoAmI;
                                                int num46 = npc.life;
                                                float num47 = npc.ai[0];
                                                npc = NPCRegistry.Create(13);
                                                Main.npcs[index] = npc;

                                                npc.life = num46;
                                                if (npc.life > npc.lifeMax)
                                                {
                                                    npc.life = npc.lifeMax;
                                                }
                                                npc.ai[0] = num47;
                                                npc.TargetClosest(true);
                                                npc.netUpdate = true;
                                                npc.whoAmI = num45;
                                            }
                                            if (npc.Type == 14 && !Main.npcs[(int)npc.ai[0]].Active)
                                            {
                                                int num48 = npc.life;
                                                int num49 = npc.whoAmI;
                                                float num50 = npc.ai[1];
                                                npc = NPCRegistry.Create(npc.Type);
                                                Main.npcs[index] = npc;
                                                npc.life = num48;
                                                if (npc.life > npc.lifeMax)
                                                {
                                                    npc.life = npc.lifeMax;
                                                }
                                                npc.ai[1] = num50;
                                                npc.TargetClosest(true);
                                                npc.netUpdate = true;
                                                npc.whoAmI = num49;
                                            }
                                            if (npc.life == 0)
                                            {
                                                bool flag5 = true;
                                                for (int l = 0; l < MAX_NPCS; l++)
                                                {
                                                    if (Main.npcs[l].Active && (Main.npcs[l].Type == 13 || Main.npcs[l].Type == 14 || Main.npcs[l].Type == 15))
                                                    {
                                                        flag5 = false;
                                                        break;
                                                    }
                                                }
                                                if (flag5)
                                                {
                                                    npc.boss = true;
                                                    npc.NPCLoot();
                                                }
                                            }
                                        }
                                        if (!npc.Active)
                                        {
                                            NetMessage.SendData(28, -1, -1, "", npc.whoAmI, -1f);
                                        }
                                        int num51 = (int)(npc.Position.X / 16f) - 1;
                                        int num52 = (int)((npc.Position.X + (float)npc.width) / 16f) + 2;
                                        int num53 = (int)(npc.Position.Y / 16f) - 1;
                                        int num54 = (int)((npc.Position.Y + (float)npc.height) / 16f) + 2;
                                        if (num51 < 0)
                                        {
                                            num51 = 0;
                                        }
                                        if (num52 > Main.maxTilesX)
                                        {
                                            num52 = Main.maxTilesX;
                                        }
                                        if (num53 < 0)
                                        {
                                            num53 = 0;
                                        }
                                        if (num54 > Main.maxTilesY)
                                        {
                                            num54 = Main.maxTilesY;
                                        }
                                        bool flag6 = false;
                                        for (int m = num51; m < num52; m++)
                                        {
                                            for (int n = num53; n < num54; n++)
                                            {
                                                if (Main.tile[m, n] != null && ((Main.tile[m, n].Active && (Main.tileSolid[(int)Main.tile[m, n].type] || (Main.tileSolidTop[(int)Main.tile[m, n].type] && Main.tile[m, n].frameY == 0))) || Main.tile[m, n].liquid > 64))
                                                {
                                                    Vector2 vector8;
                                                    vector8.X = (float)(m * 16);
                                                    vector8.Y = (float)(n * 16);
                                                    if (npc.Position.X + (float)npc.width > vector8.X && npc.Position.X < vector8.X + 16f && npc.Position.Y + (float)npc.height > vector8.Y && npc.Position.Y < vector8.Y + 16f)
                                                    {
                                                        flag6 = true;
                                                        if (Main.rand.Next(40) == 0 && Main.tile[m, n].Active)
                                                        {
                                                            WorldGen.KillTile(m, n, true, true, false);
                                                        }
                                                        if (Main.tile[m, n].type == 2)
                                                        {
                                                            byte arg_4132_0 = Main.tile[m, n - 1].type;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        float num55 = 8f;
                                        float num56 = 0.07f;
                                        if (npc.Type == 10)
                                        {
                                            num55 = 6f;
                                            num56 = 0.05f;
                                        }
                                        if (npc.Type == 13)
                                        {
                                            num55 = 11f;
                                            num56 = 0.08f;
                                        }
                                        Vector2 vector9 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                        float num57 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector9.X;
                                        float num58 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - vector9.Y;
                                        float num59 = (float)Math.Sqrt((double)(num57 * num57 + num58 * num58));
                                        if (npc.ai[1] > 0f)
                                        {
                                            num57 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].width / 2) - vector9.X;
                                            num58 = Main.npcs[(int)npc.ai[1]].Position.Y + (float)(Main.npcs[(int)npc.ai[1]].height / 2) - vector9.Y;
                                            npc.rotation = (float)Math.Atan2((double)num58, (double)num57) + 1.57f;
                                            num59 = (float)Math.Sqrt((double)(num57 * num57 + num58 * num58));
                                            num59 = (num59 - (float)npc.width) / num59;
                                            num57 *= num59;
                                            num58 *= num59;
                                            npc.Velocity = default(Vector2);
                                            npc.Position.X = npc.Position.X + num57;
                                            npc.Position.Y = npc.Position.Y + num58;
                                            return;
                                        }
                                        if (!flag6)
                                        {
                                            npc.TargetClosest(true);
                                            npc.Velocity.Y = npc.Velocity.Y + 0.11f;
                                            if (npc.Velocity.Y > num55)
                                            {
                                                npc.Velocity.Y = num55;
                                            }
                                            if ((double)(Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y)) < (double)num55 * 0.4)
                                            {
                                                if (npc.Velocity.X < 0f)
                                                {
                                                    npc.Velocity.X = npc.Velocity.X - num56 * 1.1f;
                                                }
                                                else
                                                {
                                                    npc.Velocity.X = npc.Velocity.X + num56 * 1.1f;
                                                }
                                            }
                                            else
                                            {
                                                if (npc.Velocity.Y == num55)
                                                {
                                                    if (npc.Velocity.X < num57)
                                                    {
                                                        npc.Velocity.X = npc.Velocity.X + num56;
                                                    }
                                                    else
                                                    {
                                                        if (npc.Velocity.X > num57)
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X - num56;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (npc.Velocity.Y > 4f)
                                                    {
                                                        if (npc.Velocity.X < 0f)
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X + num56 * 0.9f;
                                                        }
                                                        else
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X - num56 * 0.9f;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (npc.soundDelay == 0)
                                            {
                                                float num60 = num59 / 40f;
                                                if (num60 < 10f)
                                                {
                                                    num60 = 10f;
                                                }
                                                if (num60 > 20f)
                                                {
                                                    num60 = 20f;
                                                }
                                                npc.soundDelay = (int)num60;
                                            }
                                            num59 = (float)Math.Sqrt((double)(num57 * num57 + num58 * num58));
                                            float num61 = Math.Abs(num57);
                                            float num62 = Math.Abs(num58);
                                            num59 = num55 / num59;
                                            num57 *= num59;
                                            num58 *= num59;
                                            if ((npc.Type == 13 || npc.Type == 7) && !Main.players[npc.target].zoneEvil)
                                            {
                                                if ((double)(npc.Position.Y / 16f) > Main.rockLayer && npc.timeLeft > 2)
                                                {
                                                    npc.timeLeft = 2;
                                                }
                                                num57 = 0f;
                                                num58 = num55;
                                            }
                                            if ((npc.Velocity.X > 0f && num57 > 0f) || (npc.Velocity.X < 0f && num57 < 0f) || (npc.Velocity.Y > 0f && num58 > 0f) || (npc.Velocity.Y < 0f && num58 < 0f))
                                            {
                                                if (npc.Velocity.X < num57)
                                                {
                                                    npc.Velocity.X = npc.Velocity.X + num56;
                                                }
                                                else
                                                {
                                                    if (npc.Velocity.X > num57)
                                                    {
                                                        npc.Velocity.X = npc.Velocity.X - num56;
                                                    }
                                                }
                                                if (npc.Velocity.Y < num58)
                                                {
                                                    npc.Velocity.Y = npc.Velocity.Y + num56;
                                                }
                                                else
                                                {
                                                    if (npc.Velocity.Y > num58)
                                                    {
                                                        npc.Velocity.Y = npc.Velocity.Y - num56;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (num61 > num62)
                                                {
                                                    if (npc.Velocity.X < num57)
                                                    {
                                                        npc.Velocity.X = npc.Velocity.X + num56 * 1.1f;
                                                    }
                                                    else
                                                    {
                                                        if (npc.Velocity.X > num57)
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X - num56 * 1.1f;
                                                        }
                                                    }
                                                    if ((double)(Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y)) < (double)num55 * 0.5)
                                                    {
                                                        if (npc.Velocity.Y > 0f)
                                                        {
                                                            npc.Velocity.Y = npc.Velocity.Y + num56;
                                                        }
                                                        else
                                                        {
                                                            npc.Velocity.Y = npc.Velocity.Y - num56;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (npc.Velocity.Y < num58)
                                                    {
                                                        npc.Velocity.Y = npc.Velocity.Y + num56 * 1.1f;
                                                    }
                                                    else
                                                    {
                                                        if (npc.Velocity.Y > num58)
                                                        {
                                                            npc.Velocity.Y = npc.Velocity.Y - num56 * 1.1f;
                                                        }
                                                    }
                                                    if ((double)(Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y)) < (double)num55 * 0.5)
                                                    {
                                                        if (npc.Velocity.X > 0f)
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X + num56;
                                                        }
                                                        else
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X - num56;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) + 1.57f;
                                        return;
                                    }
                                    else
                                    {
                                        if (npc.aiStyle == 7)
                                        {
                                            int num63 = (int)(npc.Position.X + (float)(npc.width / 2)) / 16;
                                            int num64 = (int)(npc.Position.Y + (float)npc.height + 1f) / 16;
                                            if (!npc.townNPC)
                                            {
                                                npc.homeTileX = num63;
                                                npc.homeTileY = num64;
                                            }
                                            if (npc.Type == 46 && npc.target == 255)
                                            {
                                                npc.TargetClosest(true);
                                            }
                                            bool flag7 = false;
                                            npc.directionY = -1;
                                            if (npc.direction == 0)
                                            {
                                                npc.direction = 1;
                                            }
                                            for (int num65 = 0; num65 < 255; num65++)
                                            {
                                                if (Main.players[num65].Active && Main.players[num65].talkNPC == npc.whoAmI)
                                                {
                                                    flag7 = true;
                                                    if (npc.ai[0] != 0f)
                                                    {
                                                        npc.netUpdate = true;
                                                    }
                                                    npc.ai[0] = 0f;
                                                    npc.ai[1] = 300f;
                                                    npc.ai[2] = 100f;
                                                    if (Main.players[num65].Position.X + (float)(Main.players[num65].width / 2) < npc.Position.X + (float)(npc.width / 2))
                                                    {
                                                        npc.direction = -1;
                                                    }
                                                    else
                                                    {
                                                        npc.direction = 1;
                                                    }
                                                }
                                            }
                                            if (npc.ai[3] > 0f)
                                            {
                                                npc.life = -1;
                                                npc.HitEffect(0, 10.0);
                                                npc.Active = false;
                                            }
                                            if (npc.Type == 37)
                                            {
                                                npc.homeless = false;
                                                npc.homeTileX = Main.dungeonX;
                                                npc.homeTileY = Main.dungeonY;
                                                if (NPC.downedBoss3)
                                                {
                                                    npc.ai[3] = 1f;
                                                    npc.netUpdate = true;
                                                }
                                            }
                                            if (npc.townNPC && !Main.dayTime && (num63 != npc.homeTileX || num64 != npc.homeTileY) && !npc.homeless)
                                            {
                                                bool flag8 = true;
                                                for (int num66 = 0; num66 < 2; num66++)
                                                {
                                                    Rectangle rectangle = new Rectangle((int)(npc.Position.X + (float)(npc.width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(npc.Position.Y + (float)(npc.height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                                    if (num66 == 1)
                                                    {
                                                        rectangle = new Rectangle(npc.homeTileX * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, npc.homeTileY * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                                    }
                                                    for (int num67 = 0; num67 < 255; num67++)
                                                    {
                                                        if (Main.players[num67].Active)
                                                        {
                                                            Rectangle rectangle2 = new Rectangle((int)Main.players[num67].Position.X, (int)Main.players[num67].Position.Y, Main.players[num67].width, Main.players[num67].height);
                                                            if (rectangle2.Intersects(rectangle))
                                                            {
                                                                flag8 = false;
                                                                break;
                                                            }
                                                        }
                                                        if (!flag8)
                                                        {
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (flag8)
                                                {
                                                    if (npc.Type == 37 || !Collision.SolidTiles(npc.homeTileX - 1, npc.homeTileX + 1, npc.homeTileY - 3, npc.homeTileY - 1))
                                                    {
                                                        npc.Velocity.X = 0f;
                                                        npc.Velocity.Y = 0f;
                                                        npc.Position.X = (float)(npc.homeTileX * 16 + 8 - npc.width / 2);
                                                        npc.Position.Y = (float)(npc.homeTileY * 16 - npc.height) - 0.1f;
                                                        npc.netUpdate = true;
                                                    }
                                                    else
                                                    {
                                                        npc.homeless = true;
                                                        WorldGen.QuickFindHome(npc.whoAmI);
                                                    }
                                                }
                                            }
                                            if (npc.ai[0] == 0f)
                                            {
                                                if (npc.ai[2] > 0f)
                                                {
                                                    npc.ai[2] -= 1f;
                                                }
                                                if (!Main.dayTime && !flag7)
                                                {
                                                    if (num63 == npc.homeTileX && num64 == npc.homeTileY)
                                                    {
                                                        if (npc.Velocity.X != 0f)
                                                        {
                                                            npc.netUpdate = true;
                                                        }
                                                        if ((double)npc.Velocity.X > 0.1)
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X - 0.1f;
                                                        }
                                                        else
                                                        {
                                                            if ((double)npc.Velocity.X < -0.1)
                                                            {
                                                                npc.Velocity.X = npc.Velocity.X + 0.1f;
                                                            }
                                                            else
                                                            {
                                                                npc.Velocity.X = 0f;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!flag7)
                                                        {
                                                            if (num63 > npc.homeTileX)
                                                            {
                                                                npc.direction = -1;
                                                            }
                                                            else
                                                            {
                                                                npc.direction = 1;
                                                            }
                                                            npc.ai[0] = 1f;
                                                            npc.ai[1] = (float)(200 + Main.rand.Next(200));
                                                            npc.ai[2] = 0f;
                                                            npc.netUpdate = true;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if ((double)npc.Velocity.X > 0.1)
                                                    {
                                                        npc.Velocity.X = npc.Velocity.X - 0.1f;
                                                    }
                                                    else
                                                    {
                                                        if ((double)npc.Velocity.X < -0.1)
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X + 0.1f;
                                                        }
                                                        else
                                                        {
                                                            npc.Velocity.X = 0f;
                                                        }
                                                    }
                                                    if (npc.ai[1] > 0f)
                                                    {
                                                        npc.ai[1] -= 1f;
                                                    }
                                                    if (npc.ai[1] <= 0f)
                                                    {
                                                        npc.ai[0] = 1f;
                                                        npc.ai[1] = (float)(200 + Main.rand.Next(200));
                                                        if (npc.Type == 46)
                                                        {
                                                            npc.ai[1] += (float)Main.rand.Next(200, 400);
                                                        }
                                                        npc.ai[2] = 0f;
                                                        npc.netUpdate = true;
                                                    }
                                                }
                                                if ((Main.dayTime || (num63 == npc.homeTileX && num64 == npc.homeTileY)))
                                                {
                                                    if (num63 < npc.homeTileX - 25 || num63 > npc.homeTileX + 25)
                                                    {
                                                        if (npc.ai[2] == 0f)
                                                        {
                                                            if (num63 < npc.homeTileX - 50 && npc.direction == -1)
                                                            {
                                                                npc.direction = 1;
                                                                npc.netUpdate = true;
                                                                return;
                                                            }
                                                            if (num63 > npc.homeTileX + 50 && npc.direction == 1)
                                                            {
                                                                npc.direction = -1;
                                                                npc.netUpdate = true;
                                                                return;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Main.rand.Next(80) == 0 && npc.ai[2] == 0f)
                                                        {
                                                            npc.ai[2] = 200f;
                                                            npc.direction *= -1;
                                                            npc.netUpdate = true;
                                                            return;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (npc.ai[0] == 1f)
                                                {
                                                    if (!Main.dayTime && num63 == npc.homeTileX && num64 == npc.homeTileY)
                                                    {
                                                        npc.ai[0] = 0f;
                                                        npc.ai[1] = (float)(200 + Main.rand.Next(200));
                                                        npc.ai[2] = 60f;
                                                        npc.netUpdate = true;
                                                        return;
                                                    }
                                                    if (!npc.homeless && (num63 < npc.homeTileX - 35 || num63 > npc.homeTileX + 35))
                                                    {
                                                        if (npc.Position.X < (float)(npc.homeTileX * 16) && npc.direction == -1)
                                                        {
                                                            npc.direction = 1;
                                                            npc.Velocity.X = 0.1f;
                                                            npc.netUpdate = true;
                                                        }
                                                        else
                                                        {
                                                            if (npc.Position.X > (float)(npc.homeTileX * 16) && npc.direction == 1)
                                                            {
                                                                npc.direction = -1;
                                                                npc.Velocity.X = -0.1f;
                                                                npc.netUpdate = true;
                                                            }
                                                        }
                                                    }
                                                    npc.ai[1] -= 1f;
                                                    if (npc.ai[1] <= 0f)
                                                    {
                                                        npc.ai[0] = 0f;
                                                        npc.ai[1] = (float)(300 + Main.rand.Next(300));
                                                        if (npc.Type == 46)
                                                        {
                                                            npc.ai[1] -= (float)Main.rand.Next(100);
                                                        }
                                                        npc.ai[2] = 60f;
                                                        npc.netUpdate = true;
                                                    }
                                                    if (npc.closeDoor && ((npc.Position.X + (float)(npc.width / 2)) / 16f > (float)(npc.doorX + 2) || (npc.Position.X + (float)(npc.width / 2)) / 16f < (float)(npc.doorX - 2)))
                                                    {
                                                        bool flag9 = WorldGen.CloseDoor(npc.doorX, npc.doorY, false, DoorOpener.NPC);
                                                        if (flag9)
                                                        {
                                                            npc.closeDoor = false;
                                                            NetMessage.SendData(19, -1, -1, "", 1, (float)npc.doorX, (float)npc.doorY, (float)npc.direction);
                                                        }
                                                        if ((npc.Position.X + (float)(npc.width / 2)) / 16f > (float)(npc.doorX + 4) || (npc.Position.X + (float)(npc.width / 2)) / 16f < (float)(npc.doorX - 4) || (npc.Position.Y + (float)(npc.height / 2)) / 16f > (float)(npc.doorY + 4) || (npc.Position.Y + (float)(npc.height / 2)) / 16f < (float)(npc.doorY - 4))
                                                        {
                                                            npc.closeDoor = false;
                                                        }
                                                    }
                                                    if (npc.Velocity.X < -1f || npc.Velocity.X > 1f)
                                                    {
                                                        if (npc.Velocity.Y == 0f)
                                                        {
                                                            npc.Velocity *= 0.8f;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if ((double)npc.Velocity.X < 1.15 && npc.direction == 1)
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X + 0.07f;
                                                            if (npc.Velocity.X > 1f)
                                                            {
                                                                npc.Velocity.X = 1f;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (npc.Velocity.X > -1f && npc.direction == -1)
                                                            {
                                                                npc.Velocity.X = npc.Velocity.X - 0.07f;
                                                                if (npc.Velocity.X > 1f)
                                                                {
                                                                    npc.Velocity.X = 1f;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (npc.Velocity.Y == 0f)
                                                    {
                                                        if (npc.Position.X == npc.ai[2])
                                                        {
                                                            npc.direction *= -1;
                                                        }
                                                        npc.ai[2] = -1f;
                                                        int num68 = (int)((npc.Position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
                                                        int num69 = (int)((npc.Position.Y + (float)npc.height - 16f) / 16f);
                                                        if (Main.tile[num68, num69] == null)
                                                        {
                                                            Main.tile[num68, num69] = new Tile();
                                                        }
                                                        if (Main.tile[num68, num69 - 1] == null)
                                                        {
                                                            Main.tile[num68, num69 - 1] = new Tile();
                                                        }
                                                        if (Main.tile[num68, num69 - 2] == null)
                                                        {
                                                            Main.tile[num68, num69 - 2] = new Tile();
                                                        }
                                                        if (Main.tile[num68, num69 - 3] == null)
                                                        {
                                                            Main.tile[num68, num69 - 3] = new Tile();
                                                        }
                                                        if (Main.tile[num68, num69 + 1] == null)
                                                        {
                                                            Main.tile[num68, num69 + 1] = new Tile();
                                                        }
                                                        if (Main.tile[num68 + npc.direction, num69 - 1] == null)
                                                        {
                                                            Main.tile[num68 + npc.direction, num69 - 1] = new Tile();
                                                        }
                                                        if (Main.tile[num68 + npc.direction, num69 + 1] == null)
                                                        {
                                                            Main.tile[num68 + npc.direction, num69 + 1] = new Tile();
                                                        }
                                                        if (npc.townNPC && Main.tile[num68, num69 - 2].Active && Main.tile[num68, num69 - 2].type == 10 && (Main.rand.Next(10) == 0 || !Main.dayTime))
                                                        {
                                                            bool flag10 = WorldGen.OpenDoor(num68, num69 - 2, npc.direction, npc.closeDoor, DoorOpener.NPC);
                                                            if (flag10)
                                                            {
                                                                npc.closeDoor = true;
                                                                npc.doorX = num68;
                                                                npc.doorY = num69 - 2;
                                                                NetMessage.SendData(19, -1, -1, "", 0, (float)num68, (float)(num69 - 2), (float)npc.direction);
                                                                npc.netUpdate = true;
                                                                npc.ai[1] += 80f;
                                                                return;
                                                            }
                                                            if (WorldGen.OpenDoor(num68, num69 - 2, -npc.direction, npc.closeDoor, DoorOpener.NPC))
                                                            {
                                                                npc.closeDoor = true;
                                                                npc.doorX = num68;
                                                                npc.doorY = num69 - 2;
                                                                NetMessage.SendData(19, -1, -1, "", 0, (float)num68, (float)(num69 - 2), (float)(-(float)npc.direction));
                                                                npc.netUpdate = true;
                                                                npc.ai[1] += 80f;
                                                                return;
                                                            }
                                                            npc.direction *= -1;
                                                            npc.netUpdate = true;
                                                            return;
                                                        }
                                                        else
                                                        {
                                                            if ((npc.Velocity.X < 0f && npc.spriteDirection == -1) || (npc.Velocity.X > 0f && npc.spriteDirection == 1))
                                                            {
                                                                if (Main.tile[num68, num69 - 2].Active && Main.tileSolid[(int)Main.tile[num68, num69 - 2].type] && !Main.tileSolidTop[(int)Main.tile[num68, num69 - 2].type])
                                                                {
                                                                    if ((npc.direction == 1 && !Collision.SolidTiles(num68 - 2, num68 - 1, num69 - 5, num69 - 1)) || (npc.direction == -1 && !Collision.SolidTiles(num68 + 1, num68 + 2, num69 - 5, num69 - 1)))
                                                                    {
                                                                        if (!Collision.SolidTiles(num68, num68, num69 - 5, num69 - 3))
                                                                        {
                                                                            npc.Velocity.Y = -6f;
                                                                            npc.netUpdate = true;
                                                                        }
                                                                        else
                                                                        {
                                                                            npc.direction *= -1;
                                                                            npc.netUpdate = true;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        npc.direction *= -1;
                                                                        npc.netUpdate = true;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (Main.tile[num68, num69 - 1].Active && Main.tileSolid[(int)Main.tile[num68, num69 - 1].type] && !Main.tileSolidTop[(int)Main.tile[num68, num69 - 1].type])
                                                                    {
                                                                        if ((npc.direction == 1 && !Collision.SolidTiles(num68 - 2, num68 - 1, num69 - 4, num69 - 1)) || (npc.direction == -1 && !Collision.SolidTiles(num68 + 1, num68 + 2, num69 - 4, num69 - 1)))
                                                                        {
                                                                            if (!Collision.SolidTiles(num68, num68, num69 - 4, num69 - 2))
                                                                            {
                                                                                npc.Velocity.Y = -5f;
                                                                                npc.netUpdate = true;
                                                                            }
                                                                            else
                                                                            {
                                                                                npc.direction *= -1;
                                                                                npc.netUpdate = true;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            npc.direction *= -1;
                                                                            npc.netUpdate = true;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Main.tile[num68, num69].Active && Main.tileSolid[(int)Main.tile[num68, num69].type] && !Main.tileSolidTop[(int)Main.tile[num68, num69].type])
                                                                        {
                                                                            if ((npc.direction == 1 && !Collision.SolidTiles(num68 - 2, num68, num69 - 3, num69 - 1)) || (npc.direction == -1 && !Collision.SolidTiles(num68, num68 + 2, num69 - 3, num69 - 1)))
                                                                            {
                                                                                npc.Velocity.Y = -3.6f;
                                                                                npc.netUpdate = true;
                                                                            }
                                                                            else
                                                                            {
                                                                                npc.direction *= -1;
                                                                                npc.netUpdate = true;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                try
                                                                {
                                                                    if (Main.tile[num68, num69 + 1] == null)
                                                                    {
                                                                        Main.tile[num68, num69 + 1] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68 - npc.direction, num69 + 1] == null)
                                                                    {
                                                                        Main.tile[num68 - npc.direction, num69 + 1] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68, num69 + 2] == null)
                                                                    {
                                                                        Main.tile[num68, num69 + 2] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68 - npc.direction, num69 + 2] == null)
                                                                    {
                                                                        Main.tile[num68 - npc.direction, num69 + 2] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68, num69 + 3] == null)
                                                                    {
                                                                        Main.tile[num68, num69 + 3] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68 - npc.direction, num69 + 3] == null)
                                                                    {
                                                                        Main.tile[num68 - npc.direction, num69 + 3] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68, num69 + 4] == null)
                                                                    {
                                                                        Main.tile[num68, num69 + 4] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68 - npc.direction, num69 + 4] == null)
                                                                    {
                                                                        Main.tile[num68 - npc.direction, num69 + 4] = new Tile();
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num63 >= npc.homeTileX - 35 && num63 <= npc.homeTileX + 35 && (!Main.tile[num68, num69 + 1].Active || !Main.tileSolid[(int)Main.tile[num68, num69 + 1].type]) && (!Main.tile[num68 - npc.direction, num69 + 1].Active || !Main.tileSolid[(int)Main.tile[num68 - npc.direction, num69 + 1].type]) && (!Main.tile[num68, num69 + 2].Active || !Main.tileSolid[(int)Main.tile[num68, num69 + 2].type]) && (!Main.tile[num68 - npc.direction, num69 + 2].Active || !Main.tileSolid[(int)Main.tile[num68 - npc.direction, num69 + 2].type]) && (!Main.tile[num68, num69 + 3].Active || !Main.tileSolid[(int)Main.tile[num68, num69 + 3].type]) && (!Main.tile[num68 - npc.direction, num69 + 3].Active || !Main.tileSolid[(int)Main.tile[num68 - npc.direction, num69 + 3].type]) && (!Main.tile[num68, num69 + 4].Active || !Main.tileSolid[(int)Main.tile[num68, num69 + 4].type]) && (!Main.tile[num68 - npc.direction, num69 + 4].Active || !Main.tileSolid[(int)Main.tile[num68 - npc.direction, num69 + 4].type]) && npc.Type != 46)
                                                                        {
                                                                            npc.direction *= -1;
                                                                            npc.Velocity.X = npc.Velocity.X * -1f;
                                                                            npc.netUpdate = true;
                                                                        }
                                                                    }
                                                                }
                                                                catch
                                                                {
                                                                }
                                                                if (npc.Velocity.Y < 0f)
                                                                {
                                                                    npc.ai[2] = npc.Position.X;
                                                                }
                                                            }
                                                            if (npc.Velocity.Y < 0f && npc.wet)
                                                            {
                                                                npc.Velocity.Y = npc.Velocity.Y * 1.2f;
                                                            }
                                                            if (npc.Velocity.Y < 0f && npc.Type == 46)
                                                            {
                                                                npc.Velocity.Y = npc.Velocity.Y * 1.2f;
                                                                return;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (npc.aiStyle == 8)
                                            {
                                                npc.TargetClosest(true);
                                                npc.Velocity.X = npc.Velocity.X * 0.93f;
                                                if ((double)npc.Velocity.X > -0.1 && (double)npc.Velocity.X < 0.1)
                                                {
                                                    npc.Velocity.X = 0f;
                                                }
                                                if (npc.ai[0] == 0f)
                                                {
                                                    npc.ai[0] = 500f;
                                                }
                                                if (npc.ai[2] != 0f && npc.ai[3] != 0f)
                                                {
                                                    for (int num70 = 0; num70 < 50; num70++)
                                                    {
                                                        if (npc.Type == 29 || npc.Type == 45)
                                                        {
                                                            Vector2 arg_5FFC_0 = new Vector2(npc.Position.X, npc.Position.Y);
                                                            int arg_5FFC_1 = npc.width;
                                                            int arg_5FFC_2 = npc.height;
                                                            int arg_5FFC_3 = 27;
                                                            float arg_5FFC_4 = 0f;
                                                            float arg_5FFC_5 = 0f;
                                                            int arg_5FFC_6 = 100;
                                                            Color newColor = default(Color);
                                                            int num71 = Dust.NewDust(arg_5FFC_0, arg_5FFC_1, arg_5FFC_2, arg_5FFC_3, arg_5FFC_4, arg_5FFC_5, arg_5FFC_6, newColor, (float)Main.rand.Next(1, 3));
                                                            Dust expr_600B = Main.dust[num71];
                                                            expr_600B.velocity *= 3f;
                                                            if (Main.dust[num71].scale > 1f)
                                                            {
                                                                Main.dust[num71].noGravity = true;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (npc.Type == 32)
                                                            {
                                                                Vector2 arg_6098_0 = new Vector2(npc.Position.X, npc.Position.Y);
                                                                int arg_6098_1 = npc.width;
                                                                int arg_6098_2 = npc.height;
                                                                int arg_6098_3 = 29;
                                                                float arg_6098_4 = 0f;
                                                                float arg_6098_5 = 0f;
                                                                int arg_6098_6 = 100;
                                                                Color newColor = default(Color);
                                                                int num72 = Dust.NewDust(arg_6098_0, arg_6098_1, arg_6098_2, arg_6098_3, arg_6098_4, arg_6098_5, arg_6098_6, newColor, 2.5f);
                                                                Dust expr_60A7 = Main.dust[num72];
                                                                expr_60A7.velocity *= 3f;
                                                                Main.dust[num72].noGravity = true;
                                                            }
                                                            else
                                                            {
                                                                Vector2 arg_610F_0 = new Vector2(npc.Position.X, npc.Position.Y);
                                                                int arg_610F_1 = npc.width;
                                                                int arg_610F_2 = npc.height;
                                                                int arg_610F_3 = 6;
                                                                float arg_610F_4 = 0f;
                                                                float arg_610F_5 = 0f;
                                                                int arg_610F_6 = 100;
                                                                Color newColor = default(Color);
                                                                int num73 = Dust.NewDust(arg_610F_0, arg_610F_1, arg_610F_2, arg_610F_3, arg_610F_4, arg_610F_5, arg_610F_6, newColor, 2.5f);
                                                                Dust expr_611E = Main.dust[num73];
                                                                expr_611E.velocity *= 3f;
                                                                Main.dust[num73].noGravity = true;
                                                            }
                                                        }
                                                    }
                                                    npc.Position.X = npc.ai[2] * 16f - (float)(npc.width / 2) + 8f;
                                                    npc.Position.Y = npc.ai[3] * 16f - (float)npc.height;
                                                    npc.Velocity.X = 0f;
                                                    npc.Velocity.Y = 0f;
                                                    npc.ai[2] = 0f;
                                                    npc.ai[3] = 0f;
                                                    for (int num74 = 0; num74 < 50; num74++)
                                                    {
                                                        if (npc.Type == 29 || npc.Type == 45)
                                                        {
                                                            Vector2 arg_625E_0 = new Vector2(npc.Position.X, npc.Position.Y);
                                                            int arg_625E_1 = npc.width;
                                                            int arg_625E_2 = npc.height;
                                                            int arg_625E_3 = 27;
                                                            float arg_625E_4 = 0f;
                                                            float arg_625E_5 = 0f;
                                                            int arg_625E_6 = 100;
                                                            Color newColor = default(Color);
                                                            int num75 = Dust.NewDust(arg_625E_0, arg_625E_1, arg_625E_2, arg_625E_3, arg_625E_4, arg_625E_5, arg_625E_6, newColor, (float)Main.rand.Next(1, 3));
                                                            Dust expr_626D = Main.dust[num75];
                                                            expr_626D.velocity *= 3f;
                                                            if (Main.dust[num75].scale > 1f)
                                                            {
                                                                Main.dust[num75].noGravity = true;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (npc.Type == 32)
                                                            {
                                                                Vector2 arg_62FA_0 = new Vector2(npc.Position.X, npc.Position.Y);
                                                                int arg_62FA_1 = npc.width;
                                                                int arg_62FA_2 = npc.height;
                                                                int arg_62FA_3 = 29;
                                                                float arg_62FA_4 = 0f;
                                                                float arg_62FA_5 = 0f;
                                                                int arg_62FA_6 = 100;
                                                                Color newColor = default(Color);
                                                                int num76 = Dust.NewDust(arg_62FA_0, arg_62FA_1, arg_62FA_2, arg_62FA_3, arg_62FA_4, arg_62FA_5, arg_62FA_6, newColor, 2.5f);
                                                                Dust expr_6309 = Main.dust[num76];
                                                                expr_6309.velocity *= 3f;
                                                                Main.dust[num76].noGravity = true;
                                                            }
                                                            else
                                                            {
                                                                Vector2 arg_6371_0 = new Vector2(npc.Position.X, npc.Position.Y);
                                                                int arg_6371_1 = npc.width;
                                                                int arg_6371_2 = npc.height;
                                                                int arg_6371_3 = 6;
                                                                float arg_6371_4 = 0f;
                                                                float arg_6371_5 = 0f;
                                                                int arg_6371_6 = 100;
                                                                Color newColor = default(Color);
                                                                int num77 = Dust.NewDust(arg_6371_0, arg_6371_1, arg_6371_2, arg_6371_3, arg_6371_4, arg_6371_5, arg_6371_6, newColor, 2.5f);
                                                                Dust expr_6380 = Main.dust[num77];
                                                                expr_6380.velocity *= 3f;
                                                                Main.dust[num77].noGravity = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                npc.ai[0] += 1f;
                                                if (npc.ai[0] == 100f || npc.ai[0] == 200f || npc.ai[0] == 300f)
                                                {
                                                    npc.ai[1] = 30f;
                                                    npc.netUpdate = true;
                                                }
                                                else
                                                {
                                                    if (npc.ai[0] >= 650f)
                                                    {
                                                        npc.ai[0] = 1f;
                                                        int num78 = (int)Main.players[npc.target].Position.X / 16;
                                                        int num79 = (int)Main.players[npc.target].Position.Y / 16;
                                                        int num80 = (int)npc.Position.X / 16;
                                                        int num81 = (int)npc.Position.Y / 16;
                                                        int num82 = 20;
                                                        int num83 = 0;
                                                        bool flag11 = false;
                                                        if (Math.Abs(npc.Position.X - Main.players[npc.target].Position.X) + Math.Abs(npc.Position.Y - Main.players[npc.target].Position.Y) > 2000f)
                                                        {
                                                            num83 = 100;
                                                            flag11 = true;
                                                        }
                                                        while (!flag11 && num83 < 100)
                                                        {
                                                            num83++;
                                                            int num84 = Main.rand.Next(num78 - num82, num78 + num82);
                                                            int num85 = Main.rand.Next(num79 - num82, num79 + num82);
                                                            for (int num86 = num85; num86 < num79 + num82; num86++)
                                                            {
                                                                if ((num86 < num79 - 4 || num86 > num79 + 4 || num84 < num78 - 4 || num84 > num78 + 4) && (num86 < num81 - 1 || num86 > num81 + 1 || num84 < num80 - 1 || num84 > num80 + 1) && Main.tile[num84, num86].Active)
                                                                {
                                                                    bool flag12 = true;
                                                                    if (npc.Type == 32 && Main.tile[num84, num86 - 1].wall == 0)
                                                                    {
                                                                        flag12 = false;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Main.tile[num84, num86 - 1].lava)
                                                                        {
                                                                            flag12 = false;
                                                                        }
                                                                    }
                                                                    if (flag12 && Main.tileSolid[(int)Main.tile[num84, num86].type] && !Collision.SolidTiles(num84 - 1, num84 + 1, num86 - 4, num86 - 1))
                                                                    {
                                                                        npc.ai[1] = 20f;
                                                                        npc.ai[2] = (float)num84;
                                                                        npc.ai[3] = (float)num86;
                                                                        flag11 = true;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        npc.netUpdate = true;
                                                    }
                                                }
                                                if (npc.ai[1] > 0f)
                                                {
                                                    npc.ai[1] -= 1f;
                                                    if (npc.ai[1] == 25f)
                                                    {
                                                        if (npc.Type == 29 || npc.Type == 45)
                                                        {
                                                            NPC.NewNPC((int)npc.Position.X + npc.width / 2, (int)npc.Position.Y - 8, 30, 0);
                                                        }
                                                        else
                                                        {
                                                            if (npc.Type == 32)
                                                            {
                                                                NPC.NewNPC((int)npc.Position.X + npc.width / 2, (int)npc.Position.Y - 8, 33, 0);
                                                            }
                                                            else
                                                            {
                                                                NPC.NewNPC((int)npc.Position.X + npc.width / 2 + npc.direction * 8, (int)npc.Position.Y + 20, 25, 0);
                                                            }
                                                        }
                                                    }
                                                }
                                                if (npc.Type == 29 || npc.Type == 45)
                                                {
                                                    if (Main.rand.Next(5) == 0)
                                                    {
                                                        Vector2 arg_680C_0 = new Vector2(npc.Position.X, npc.Position.Y + 2f);
                                                        int arg_680C_1 = npc.width;
                                                        int arg_680C_2 = npc.height;
                                                        int arg_680C_3 = 27;
                                                        float arg_680C_4 = npc.Velocity.X * 0.2f;
                                                        float arg_680C_5 = npc.Velocity.Y * 0.2f;
                                                        int arg_680C_6 = 100;
                                                        Color newColor = default(Color);
                                                        int num87 = Dust.NewDust(arg_680C_0, arg_680C_1, arg_680C_2, arg_680C_3, arg_680C_4, arg_680C_5, arg_680C_6, newColor, 1.5f);
                                                        Main.dust[num87].noGravity = true;
                                                        Dust expr_682E_cp_0 = Main.dust[num87];
                                                        expr_682E_cp_0.velocity.X = expr_682E_cp_0.velocity.X * 0.5f;
                                                        Main.dust[num87].velocity.Y = -2f;
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    if (npc.Type == 32)
                                                    {
                                                        if (Main.rand.Next(2) == 0)
                                                        {
                                                            Vector2 arg_68D6_0 = new Vector2(npc.Position.X, npc.Position.Y + 2f);
                                                            int arg_68D6_1 = npc.width;
                                                            int arg_68D6_2 = npc.height;
                                                            int arg_68D6_3 = 29;
                                                            float arg_68D6_4 = npc.Velocity.X * 0.2f;
                                                            float arg_68D6_5 = npc.Velocity.Y * 0.2f;
                                                            int arg_68D6_6 = 100;
                                                            Color newColor = default(Color);
                                                            int num88 = Dust.NewDust(arg_68D6_0, arg_68D6_1, arg_68D6_2, arg_68D6_3, arg_68D6_4, arg_68D6_5, arg_68D6_6, newColor, 2f);
                                                            Main.dust[num88].noGravity = true;
                                                            Dust expr_68F8_cp_0 = Main.dust[num88];
                                                            expr_68F8_cp_0.velocity.X = expr_68F8_cp_0.velocity.X * 1f;
                                                            Dust expr_6916_cp_0 = Main.dust[num88];
                                                            expr_6916_cp_0.velocity.Y = expr_6916_cp_0.velocity.Y * 1f;
                                                            return;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Main.rand.Next(2) == 0)
                                                        {
                                                            Vector2 arg_6999_0 = new Vector2(npc.Position.X, npc.Position.Y + 2f);
                                                            int arg_6999_1 = npc.width;
                                                            int arg_6999_2 = npc.height;
                                                            int arg_6999_3 = 6;
                                                            float arg_6999_4 = npc.Velocity.X * 0.2f;
                                                            float arg_6999_5 = npc.Velocity.Y * 0.2f;
                                                            int arg_6999_6 = 100;
                                                            Color newColor = default(Color);
                                                            int num89 = Dust.NewDust(arg_6999_0, arg_6999_1, arg_6999_2, arg_6999_3, arg_6999_4, arg_6999_5, arg_6999_6, newColor, 2f);
                                                            Main.dust[num89].noGravity = true;
                                                            Dust expr_69BB_cp_0 = Main.dust[num89];
                                                            expr_69BB_cp_0.velocity.X = expr_69BB_cp_0.velocity.X * 1f;
                                                            Dust expr_69D9_cp_0 = Main.dust[num89];
                                                            expr_69D9_cp_0.velocity.Y = expr_69D9_cp_0.velocity.Y * 1f;
                                                            return;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (npc.aiStyle == 9)
                                                {
                                                    if (npc.target == 255)
                                                    {
                                                        npc.TargetClosest(true);
                                                        float num90 = 6f;
                                                        if (npc.Type == 30)
                                                        {
                                                            NPC.maxSpawns = 8;
                                                        }
                                                        Vector2 vector10 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                        float num91 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector10.X;
                                                        float num92 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - vector10.Y;
                                                        float num93 = (float)Math.Sqrt((double)(num91 * num91 + num92 * num92));
                                                        num93 = num90 / num93;
                                                        npc.Velocity.X = num91 * num93;
                                                        npc.Velocity.Y = num92 * num93;
                                                    }
                                                    if (npc.timeLeft > 100)
                                                    {
                                                        npc.timeLeft = 100;
                                                    }
                                                    for (int num94 = 0; num94 < 2; num94++)
                                                    {
                                                        if (npc.Type == 30)
                                                        {
                                                            Vector2 arg_6B8D_0 = new Vector2(npc.Position.X, npc.Position.Y + 2f);
                                                            int arg_6B8D_1 = npc.width;
                                                            int arg_6B8D_2 = npc.height;
                                                            int arg_6B8D_3 = 27;
                                                            float arg_6B8D_4 = npc.Velocity.X * 0.2f;
                                                            float arg_6B8D_5 = npc.Velocity.Y * 0.2f;
                                                            int arg_6B8D_6 = 100;
                                                            Color newColor = default(Color);
                                                            int num95 = Dust.NewDust(arg_6B8D_0, arg_6B8D_1, arg_6B8D_2, arg_6B8D_3, arg_6B8D_4, arg_6B8D_5, arg_6B8D_6, newColor, 2f);
                                                            Main.dust[num95].noGravity = true;
                                                            Dust expr_6BAA = Main.dust[num95];
                                                            expr_6BAA.velocity *= 0.3f;
                                                            Dust expr_6BCC_cp_0 = Main.dust[num95];
                                                            expr_6BCC_cp_0.velocity.X = expr_6BCC_cp_0.velocity.X - npc.Velocity.X * 0.2f;
                                                            Dust expr_6BF6_cp_0 = Main.dust[num95];
                                                            expr_6BF6_cp_0.velocity.Y = expr_6BF6_cp_0.velocity.Y - npc.Velocity.Y * 0.2f;
                                                        }
                                                        else
                                                        {
                                                            if (npc.Type == 33)
                                                            {
                                                                Vector2 arg_6C87_0 = new Vector2(npc.Position.X, npc.Position.Y + 2f);
                                                                int arg_6C87_1 = npc.width;
                                                                int arg_6C87_2 = npc.height;
                                                                int arg_6C87_3 = 29;
                                                                float arg_6C87_4 = npc.Velocity.X * 0.2f;
                                                                float arg_6C87_5 = npc.Velocity.Y * 0.2f;
                                                                int arg_6C87_6 = 100;
                                                                Color newColor = default(Color);
                                                                int num96 = Dust.NewDust(arg_6C87_0, arg_6C87_1, arg_6C87_2, arg_6C87_3, arg_6C87_4, arg_6C87_5, arg_6C87_6, newColor, 2f);
                                                                Main.dust[num96].noGravity = true;
                                                                Dust expr_6CA9_cp_0 = Main.dust[num96];
                                                                expr_6CA9_cp_0.velocity.X = expr_6CA9_cp_0.velocity.X * 0.3f;
                                                                Dust expr_6CC7_cp_0 = Main.dust[num96];
                                                                expr_6CC7_cp_0.velocity.Y = expr_6CC7_cp_0.velocity.Y * 0.3f;
                                                            }
                                                            else
                                                            {
                                                                Vector2 arg_6D3E_0 = new Vector2(npc.Position.X, npc.Position.Y + 2f);
                                                                int arg_6D3E_1 = npc.width;
                                                                int arg_6D3E_2 = npc.height;
                                                                int arg_6D3E_3 = 6;
                                                                float arg_6D3E_4 = npc.Velocity.X * 0.2f;
                                                                float arg_6D3E_5 = npc.Velocity.Y * 0.2f;
                                                                int arg_6D3E_6 = 100;
                                                                Color newColor = default(Color);
                                                                int num97 = Dust.NewDust(arg_6D3E_0, arg_6D3E_1, arg_6D3E_2, arg_6D3E_3, arg_6D3E_4, arg_6D3E_5, arg_6D3E_6, newColor, 2f);
                                                                Main.dust[num97].noGravity = true;
                                                                Dust expr_6D60_cp_0 = Main.dust[num97];
                                                                expr_6D60_cp_0.velocity.X = expr_6D60_cp_0.velocity.X * 0.3f;
                                                                Dust expr_6D7E_cp_0 = Main.dust[num97];
                                                                expr_6D7E_cp_0.velocity.Y = expr_6D7E_cp_0.velocity.Y * 0.3f;
                                                            }
                                                        }
                                                    }
                                                    npc.rotation += 0.4f * (float)npc.direction;
                                                    return;
                                                }
                                                if (npc.aiStyle == 10)
                                                {
                                                    float num98 = 1f;
                                                    float num99 = 0.011f;
                                                    npc.TargetClosest(true);
                                                    Vector2 vector11 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                    float num100 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector11.X;
                                                    float num101 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - vector11.Y;
                                                    float num102 = (float)Math.Sqrt((double)(num100 * num100 + num101 * num101));
                                                    float num103 = num102;
                                                    npc.ai[1] += 1f;
                                                    if (npc.ai[1] > 1000f)
                                                    {
                                                        num99 *= 6f;
                                                        num98 = 5f;
                                                        if (npc.ai[1] > 1030f)
                                                        {
                                                            npc.ai[1] = 0f;
                                                        }
                                                    }
                                                    if (num103 < 300f)
                                                    {
                                                        npc.ai[0] += 0.9f;
                                                        if (npc.ai[0] > 0f)
                                                        {
                                                            npc.Velocity.Y = npc.Velocity.Y + 0.019f;
                                                        }
                                                        else
                                                        {
                                                            npc.Velocity.Y = npc.Velocity.Y - 0.019f;
                                                        }
                                                        if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X + 0.019f;
                                                        }
                                                        else
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X - 0.019f;
                                                        }
                                                        if (npc.ai[0] > 200f)
                                                        {
                                                            npc.ai[0] = -200f;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (num103 < 350f)
                                                        {
                                                            num98 = 1.2f;
                                                            num99 = 0.04f;
                                                        }
                                                        else
                                                        {
                                                            if (num103 < 400f)
                                                            {
                                                                num98 = 2.5f;
                                                                num99 = 0.04f;
                                                            }
                                                            else
                                                            {
                                                                num98 = 4f;
                                                                num99 = 0.06f;
                                                            }
                                                        }
                                                    }
                                                    num102 = num98 / num102;
                                                    num100 *= num102;
                                                    num101 *= num102;
                                                    if (Main.players[npc.target].dead)
                                                    {
                                                        num100 = (float)npc.direction * num98 / 2f;
                                                        num101 = -num98 / 2f;
                                                    }
                                                    if (npc.Velocity.X < num100)
                                                    {
                                                        npc.Velocity.X = npc.Velocity.X + num99;
                                                    }
                                                    else
                                                    {
                                                        if (npc.Velocity.X > num100)
                                                        {
                                                            npc.Velocity.X = npc.Velocity.X - num99;
                                                        }
                                                    }
                                                    if (npc.Velocity.Y < num101)
                                                    {
                                                        npc.Velocity.Y = npc.Velocity.Y + num99;
                                                    }
                                                    else
                                                    {
                                                        if (npc.Velocity.Y > num101)
                                                        {
                                                            npc.Velocity.Y = npc.Velocity.Y - num99;
                                                        }
                                                    }
                                                    if (num100 > 0f)
                                                    {
                                                        npc.spriteDirection = -1;
                                                        npc.rotation = (float)Math.Atan2((double)num101, (double)num100);
                                                    }
                                                    if (num100 < 0f)
                                                    {
                                                        npc.spriteDirection = 1;
                                                        npc.rotation = (float)Math.Atan2((double)num101, (double)num100) + 3.14f;
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    if (npc.aiStyle == 11)
                                                    {
                                                        if (npc.ai[0] == 0f)
                                                        {
                                                            npc.TargetClosest(true);
                                                            npc.ai[0] = 1f;
                                                            if (npc.Type != 68)
                                                            {
                                                                int npcIndex = NPC.NewNPC((int)(npc.Position.X + (float)(npc.width / 2)), (int)npc.Position.Y + npc.height / 2, 36, npc.whoAmI);
                                                                Main.npcs[npcIndex].ai[0] = -1f;
                                                                Main.npcs[npcIndex].ai[1] = (float)npc.whoAmI;
                                                                Main.npcs[npcIndex].target = npc.target;
                                                                Main.npcs[npcIndex].netUpdate = true;
                                                                npcIndex = NPC.NewNPC((int)(npc.Position.X + (float)(npc.width / 2)), (int)npc.Position.Y + npc.height / 2, 36, npc.whoAmI);
                                                                Main.npcs[npcIndex].ai[0] = 1f;
                                                                Main.npcs[npcIndex].ai[1] = (float)npc.whoAmI;
                                                                Main.npcs[npcIndex].ai[3] = 150f;
                                                                Main.npcs[npcIndex].target = npc.target;
                                                                Main.npcs[npcIndex].netUpdate = true;
                                                            }
                                                        }
                                                        if (npc.Type == 68 && npc.ai[1] != 3f && npc.ai[1] != 2f)
                                                        {
                                                            npc.ai[1] = 2f;
                                                        }
                                                        if (Main.players[npc.target].dead || Math.Abs(npc.Position.X - Main.players[npc.target].Position.X) > 2000f || Math.Abs(npc.Position.Y - Main.players[npc.target].Position.Y) > 2000f)
                                                        {
                                                            npc.TargetClosest(true);
                                                            if (Main.players[npc.target].dead || Math.Abs(npc.Position.X - Main.players[npc.target].Position.X) > 2000f || Math.Abs(npc.Position.Y - Main.players[npc.target].Position.Y) > 2000f)
                                                            {
                                                                npc.ai[1] = 3f;
                                                            }
                                                        }
                                                        if (Main.dayTime && npc.ai[1] != 3f && npc.ai[1] != 2f)
                                                        {
                                                            npc.ai[1] = 2f;
                                                        }
                                                        if (npc.ai[1] == 0f)
                                                        {
                                                            npc.ai[2] += 1f;
                                                            if (npc.ai[2] >= 800f)
                                                            {
                                                                npc.ai[2] = 0f;
                                                                npc.ai[1] = 1f;
                                                                npc.TargetClosest(true);
                                                                npc.netUpdate = true;
                                                            }
                                                            npc.rotation = npc.Velocity.X / 15f;
                                                            if (npc.Position.Y > Main.players[npc.target].Position.Y - 250f)
                                                            {
                                                                if (npc.Velocity.Y > 0f)
                                                                {
                                                                    npc.Velocity.Y = npc.Velocity.Y * 0.98f;
                                                                }
                                                                npc.Velocity.Y = npc.Velocity.Y - 0.02f;
                                                                if (npc.Velocity.Y > 2f)
                                                                {
                                                                    npc.Velocity.Y = 2f;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (npc.Position.Y < Main.players[npc.target].Position.Y - 250f)
                                                                {
                                                                    if (npc.Velocity.Y < 0f)
                                                                    {
                                                                        npc.Velocity.Y = npc.Velocity.Y * 0.98f;
                                                                    }
                                                                    npc.Velocity.Y = npc.Velocity.Y + 0.02f;
                                                                    if (npc.Velocity.Y < -2f)
                                                                    {
                                                                        npc.Velocity.Y = -2f;
                                                                    }
                                                                }
                                                            }
                                                            if (npc.Position.X + (float)(npc.width / 2) > Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2))
                                                            {
                                                                if (npc.Velocity.X > 0f)
                                                                {
                                                                    npc.Velocity.X = npc.Velocity.X * 0.98f;
                                                                }
                                                                npc.Velocity.X = npc.Velocity.X - 0.05f;
                                                                if (npc.Velocity.X > 8f)
                                                                {
                                                                    npc.Velocity.X = 8f;
                                                                }
                                                            }
                                                            if (npc.Position.X + (float)(npc.width / 2) < Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2))
                                                            {
                                                                if (npc.Velocity.X < 0f)
                                                                {
                                                                    npc.Velocity.X = npc.Velocity.X * 0.98f;
                                                                }
                                                                npc.Velocity.X = npc.Velocity.X + 0.05f;
                                                                if (npc.Velocity.X < -8f)
                                                                {
                                                                    npc.Velocity.X = -8f;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (npc.ai[1] == 1f)
                                                            {
                                                                npc.ai[2] += 1f;
                                                                if (npc.ai[2] >= 400f)
                                                                {
                                                                    npc.ai[2] = 0f;
                                                                    npc.ai[1] = 0f;
                                                                }
                                                                npc.rotation += (float)npc.direction * 0.3f;
                                                                Vector2 vector12 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                                float num105 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector12.X;
                                                                float num106 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - vector12.Y;
                                                                float num107 = (float)Math.Sqrt((double)(num105 * num105 + num106 * num106));
                                                                num107 = 2f / num107;
                                                                npc.Velocity.X = num105 * num107;
                                                                npc.Velocity.Y = num106 * num107;
                                                            }
                                                            else
                                                            {
                                                                if (npc.ai[1] == 2f)
                                                                {
                                                                    npc.damage = 9999;
                                                                    npc.defense = 9999;
                                                                    npc.rotation += (float)npc.direction * 0.3f;
                                                                    Vector2 vector13 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                                    float num108 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector13.X;
                                                                    float num109 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - vector13.Y;
                                                                    float num110 = (float)Math.Sqrt((double)(num108 * num108 + num109 * num109));
                                                                    num110 = 8f / num110;
                                                                    npc.Velocity.X = num108 * num110;
                                                                    npc.Velocity.Y = num109 * num110;
                                                                }
                                                                else
                                                                {
                                                                    if (npc.ai[1] == 3f)
                                                                    {
                                                                        npc.Velocity.Y = npc.Velocity.Y - 0.1f;
                                                                        if (npc.Velocity.Y > 0f)
                                                                        {
                                                                            npc.Velocity.Y = npc.Velocity.Y * 0.95f;
                                                                        }
                                                                        npc.Velocity.X = npc.Velocity.X * 0.95f;
                                                                        if (npc.timeLeft > 50)
                                                                        {
                                                                            npc.timeLeft = 50;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        if (npc.ai[1] != 2f && npc.ai[1] != 3f && npc.Type != 68)
                                                        {
                                                            Vector2 arg_7AE4_0 = new Vector2(npc.Position.X + (float)(npc.width / 2) - 15f - npc.Velocity.X * 5f, npc.Position.Y + (float)npc.height - 2f);
                                                            int arg_7AE4_1 = 30;
                                                            int arg_7AE4_2 = 10;
                                                            int arg_7AE4_3 = 5;
                                                            float arg_7AE4_4 = -npc.Velocity.X * 0.2f;
                                                            float arg_7AE4_5 = 3f;
                                                            int arg_7AE4_6 = 0;
                                                            Color newColor = default(Color);
                                                            int num111 = Dust.NewDust(arg_7AE4_0, arg_7AE4_1, arg_7AE4_2, arg_7AE4_3, arg_7AE4_4, arg_7AE4_5, arg_7AE4_6, newColor, 2f);
                                                            Main.dust[num111].noGravity = true;
                                                            Dust expr_7B06_cp_0 = Main.dust[num111];
                                                            expr_7B06_cp_0.velocity.X = expr_7B06_cp_0.velocity.X * 1.3f;
                                                            Dust expr_7B24_cp_0 = Main.dust[num111];
                                                            expr_7B24_cp_0.velocity.X = expr_7B24_cp_0.velocity.X + npc.Velocity.X * 0.4f;
                                                            Dust expr_7B4E_cp_0 = Main.dust[num111];
                                                            expr_7B4E_cp_0.velocity.Y = expr_7B4E_cp_0.velocity.Y + (2f + npc.Velocity.Y);
                                                            for (int num112 = 0; num112 < 2; num112++)
                                                            {
                                                                Vector2 arg_7BC3_0 = new Vector2(npc.Position.X, npc.Position.Y + 120f);
                                                                int arg_7BC3_1 = npc.width;
                                                                int arg_7BC3_2 = 60;
                                                                int arg_7BC3_3 = 5;
                                                                float arg_7BC3_4 = npc.Velocity.X;
                                                                float arg_7BC3_5 = npc.Velocity.Y;
                                                                int arg_7BC3_6 = 0;
                                                                newColor = default(Color);
                                                                num111 = Dust.NewDust(arg_7BC3_0, arg_7BC3_1, arg_7BC3_2, arg_7BC3_3, arg_7BC3_4, arg_7BC3_5, arg_7BC3_6, newColor, 2f);
                                                                Main.dust[num111].noGravity = true;
                                                                Dust expr_7BE0 = Main.dust[num111];
                                                                expr_7BE0.velocity -= npc.Velocity;
                                                                Dust expr_7C03_cp_0 = Main.dust[num111];
                                                                expr_7C03_cp_0.velocity.Y = expr_7C03_cp_0.velocity.Y + 5f;
                                                            }
                                                            return;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (npc.aiStyle == 12)
                                                        {
                                                            npc.spriteDirection = -(int)npc.ai[0];
                                                            if (!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != 11)
                                                            {
                                                                npc.ai[2] += 10f;
                                                                if (npc.ai[2] > 50f)
                                                                {
                                                                    npc.life = -1;
                                                                    npc.HitEffect(0, 10.0);
                                                                    npc.Active = false;
                                                                }
                                                            }
                                                            if (npc.ai[2] == 0f || npc.ai[2] == 3f)
                                                            {
                                                                if (Main.npcs[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
                                                                {
                                                                    npc.timeLeft = 10;
                                                                }
                                                                if (Main.npcs[(int)npc.ai[1]].ai[1] != 0f)
                                                                {
                                                                    if (npc.Position.Y > Main.npcs[(int)npc.ai[1]].Position.Y - 100f)
                                                                    {
                                                                        if (npc.Velocity.Y > 0f)
                                                                        {
                                                                            npc.Velocity.Y = npc.Velocity.Y * 0.96f;
                                                                        }
                                                                        npc.Velocity.Y = npc.Velocity.Y - 0.07f;
                                                                        if (npc.Velocity.Y > 6f)
                                                                        {
                                                                            npc.Velocity.Y = 6f;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y - 100f)
                                                                        {
                                                                            if (npc.Velocity.Y < 0f)
                                                                            {
                                                                                npc.Velocity.Y = npc.Velocity.Y * 0.96f;
                                                                            }
                                                                            npc.Velocity.Y = npc.Velocity.Y + 0.07f;
                                                                            if (npc.Velocity.Y < -6f)
                                                                            {
                                                                                npc.Velocity.Y = -6f;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (npc.Position.X + (float)(npc.width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].width / 2) - 120f * npc.ai[0])
                                                                    {
                                                                        if (npc.Velocity.X > 0f)
                                                                        {
                                                                            npc.Velocity.X = npc.Velocity.X * 0.96f;
                                                                        }
                                                                        npc.Velocity.X = npc.Velocity.X - 0.1f;
                                                                        if (npc.Velocity.X > 8f)
                                                                        {
                                                                            npc.Velocity.X = 8f;
                                                                        }
                                                                    }
                                                                    if (npc.Position.X + (float)(npc.width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].width / 2) - 120f * npc.ai[0])
                                                                    {
                                                                        if (npc.Velocity.X < 0f)
                                                                        {
                                                                            npc.Velocity.X = npc.Velocity.X * 0.96f;
                                                                        }
                                                                        npc.Velocity.X = npc.Velocity.X + 0.1f;
                                                                        if (npc.Velocity.X < -8f)
                                                                        {
                                                                            npc.Velocity.X = -8f;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    npc.ai[3] += 1f;
                                                                    if (npc.ai[3] >= 300f)
                                                                    {
                                                                        npc.ai[2] += 1f;
                                                                        npc.ai[3] = 0f;
                                                                        npc.netUpdate = true;
                                                                    }
                                                                    if (npc.Position.Y > Main.npcs[(int)npc.ai[1]].Position.Y + 230f)
                                                                    {
                                                                        if (npc.Velocity.Y > 0f)
                                                                        {
                                                                            npc.Velocity.Y = npc.Velocity.Y * 0.96f;
                                                                        }
                                                                        npc.Velocity.Y = npc.Velocity.Y - 0.04f;
                                                                        if (npc.Velocity.Y > 3f)
                                                                        {
                                                                            npc.Velocity.Y = 3f;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y + 230f)
                                                                        {
                                                                            if (npc.Velocity.Y < 0f)
                                                                            {
                                                                                npc.Velocity.Y = npc.Velocity.Y * 0.96f;
                                                                            }
                                                                            npc.Velocity.Y = npc.Velocity.Y + 0.04f;
                                                                            if (npc.Velocity.Y < -3f)
                                                                            {
                                                                                npc.Velocity.Y = -3f;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (npc.Position.X + (float)(npc.width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0])
                                                                    {
                                                                        if (npc.Velocity.X > 0f)
                                                                        {
                                                                            npc.Velocity.X = npc.Velocity.X * 0.96f;
                                                                        }
                                                                        npc.Velocity.X = npc.Velocity.X - 0.07f;
                                                                        if (npc.Velocity.X > 8f)
                                                                        {
                                                                            npc.Velocity.X = 8f;
                                                                        }
                                                                    }
                                                                    if (npc.Position.X + (float)(npc.width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0])
                                                                    {
                                                                        if (npc.Velocity.X < 0f)
                                                                        {
                                                                            npc.Velocity.X = npc.Velocity.X * 0.96f;
                                                                        }
                                                                        npc.Velocity.X = npc.Velocity.X + 0.07f;
                                                                        if (npc.Velocity.X < -8f)
                                                                        {
                                                                            npc.Velocity.X = -8f;
                                                                        }
                                                                    }
                                                                }
                                                                Vector2 vector14 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                                float num113 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector14.X;
                                                                float num114 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector14.Y;
                                                                Math.Sqrt((double)(num113 * num113 + num114 * num114));
                                                                npc.rotation = (float)Math.Atan2((double)num114, (double)num113) + 1.57f;
                                                                return;
                                                            }
                                                            if (npc.ai[2] == 1f)
                                                            {
                                                                Vector2 vector15 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                                float num115 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector15.X;
                                                                float num116 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector15.Y;
                                                                float num117 = (float)Math.Sqrt((double)(num115 * num115 + num116 * num116));
                                                                npc.rotation = (float)Math.Atan2((double)num116, (double)num115) + 1.57f;
                                                                npc.Velocity.X = npc.Velocity.X * 0.95f;
                                                                npc.Velocity.Y = npc.Velocity.Y - 0.1f;
                                                                if (npc.Velocity.Y < -8f)
                                                                {
                                                                    npc.Velocity.Y = -8f;
                                                                }
                                                                if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y - 200f)
                                                                {
                                                                    npc.TargetClosest(true);
                                                                    npc.ai[2] = 2f;
                                                                    vector15 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                                    num115 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector15.X;
                                                                    num116 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - vector15.Y;
                                                                    num117 = (float)Math.Sqrt((double)(num115 * num115 + num116 * num116));
                                                                    num117 = 20f / num117;
                                                                    npc.Velocity.X = num115 * num117;
                                                                    npc.Velocity.Y = num116 * num117;
                                                                    npc.netUpdate = true;
                                                                    return;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (npc.ai[2] == 2f)
                                                                {
                                                                    if (npc.Position.Y > Main.players[npc.target].Position.Y || npc.Velocity.Y < 0f)
                                                                    {
                                                                        npc.ai[2] = 3f;
                                                                        return;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (npc.ai[2] == 4f)
                                                                    {
                                                                        Vector2 vector16 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                                        float num118 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector16.X;
                                                                        float num119 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector16.Y;
                                                                        float num120 = (float)Math.Sqrt((double)(num118 * num118 + num119 * num119));
                                                                        npc.rotation = (float)Math.Atan2((double)num119, (double)num118) + 1.57f;
                                                                        npc.Velocity.Y = npc.Velocity.Y * 0.95f;
                                                                        npc.Velocity.X = npc.Velocity.X + 0.1f * -npc.ai[0];
                                                                        if (npc.Velocity.X < -8f)
                                                                        {
                                                                            npc.Velocity.X = -8f;
                                                                        }
                                                                        if (npc.Velocity.X > 8f)
                                                                        {
                                                                            npc.Velocity.X = 8f;
                                                                        }
                                                                        if (npc.Position.X + (float)(npc.width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].width / 2) - 500f || npc.Position.X + (float)(npc.width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].width / 2) + 500f)
                                                                        {
                                                                            npc.TargetClosest(true);
                                                                            npc.ai[2] = 5f;
                                                                            vector16 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                                            num118 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector16.X;
                                                                            num119 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - vector16.Y;
                                                                            num120 = (float)Math.Sqrt((double)(num118 * num118 + num119 * num119));
                                                                            num120 = 20f / num120;
                                                                            npc.Velocity.X = num118 * num120;
                                                                            npc.Velocity.Y = num119 * num120;
                                                                            npc.netUpdate = true;
                                                                            return;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (npc.ai[2] == 5f && ((npc.Velocity.X > 0f && npc.Position.X + (float)(npc.width / 2) > Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2)) || (npc.Velocity.X < 0f && npc.Position.X + (float)(npc.width / 2) < Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2))))
                                                                        {
                                                                            npc.ai[2] = 0f;
                                                                            return;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (npc.aiStyle == 13)
                                                            {
                                                                if (Main.tile[(int)npc.ai[0], (int)npc.ai[1]] == null)
                                                                {
                                                                    Main.tile[(int)npc.ai[0], (int)npc.ai[1]] = new Tile();
                                                                }
                                                                if (!Main.tile[(int)npc.ai[0], (int)npc.ai[1]].Active)
                                                                {
                                                                    npc.life = -1;
                                                                    npc.HitEffect(0, 10.0);
                                                                    npc.Active = false;
                                                                    return;
                                                                }
                                                                npc.TargetClosest(true);
                                                                float num121 = 0.05f;
                                                                float num122 = 150f;
                                                                if (npc.Type == 43)
                                                                {
                                                                    num122 = 200f;
                                                                }
                                                                Vector2 vector17 = new Vector2(npc.ai[0] * 16f + 8f, npc.ai[1] * 16f + 8f);
                                                                float num123 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - (float)(npc.width / 2) - vector17.X;
                                                                float num124 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - (float)(npc.height / 2) - vector17.Y;
                                                                float num125 = (float)Math.Sqrt((double)(num123 * num123 + num124 * num124));
                                                                if (num125 > num122)
                                                                {
                                                                    num125 = num122 / num125;
                                                                    num123 *= num125;
                                                                    num124 *= num125;
                                                                }
                                                                if (npc.Position.X < npc.ai[0] * 16f + 8f + num123)
                                                                {
                                                                    npc.Velocity.X = npc.Velocity.X + num121;
                                                                    if (npc.Velocity.X < 0f && num123 > 0f)
                                                                    {
                                                                        npc.Velocity.X = npc.Velocity.X + num121 * 2f;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (npc.Position.X > npc.ai[0] * 16f + 8f + num123)
                                                                    {
                                                                        npc.Velocity.X = npc.Velocity.X - num121;
                                                                        if (npc.Velocity.X > 0f && num123 < 0f)
                                                                        {
                                                                            npc.Velocity.X = npc.Velocity.X - num121 * 2f;
                                                                        }
                                                                    }
                                                                }
                                                                if (npc.Position.Y < npc.ai[1] * 16f + 8f + num124)
                                                                {
                                                                    npc.Velocity.Y = npc.Velocity.Y + num121;
                                                                    if (npc.Velocity.Y < 0f && num124 > 0f)
                                                                    {
                                                                        npc.Velocity.Y = npc.Velocity.Y + num121 * 2f;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (npc.Position.Y > npc.ai[1] * 16f + 8f + num124)
                                                                    {
                                                                        npc.Velocity.Y = npc.Velocity.Y - num121;
                                                                        if (npc.Velocity.Y > 0f && num124 < 0f)
                                                                        {
                                                                            npc.Velocity.Y = npc.Velocity.Y - num121 * 2f;
                                                                        }
                                                                    }
                                                                }
                                                                if (npc.Type == 43)
                                                                {
                                                                    if (npc.Velocity.X > 3f)
                                                                    {
                                                                        npc.Velocity.X = 3f;
                                                                    }
                                                                    if (npc.Velocity.X < -3f)
                                                                    {
                                                                        npc.Velocity.X = -3f;
                                                                    }
                                                                    if (npc.Velocity.Y > 3f)
                                                                    {
                                                                        npc.Velocity.Y = 3f;
                                                                    }
                                                                    if (npc.Velocity.Y < -3f)
                                                                    {
                                                                        npc.Velocity.Y = -3f;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (npc.Velocity.X > 2f)
                                                                    {
                                                                        npc.Velocity.X = 2f;
                                                                    }
                                                                    if (npc.Velocity.X < -2f)
                                                                    {
                                                                        npc.Velocity.X = -2f;
                                                                    }
                                                                    if (npc.Velocity.Y > 2f)
                                                                    {
                                                                        npc.Velocity.Y = 2f;
                                                                    }
                                                                    if (npc.Velocity.Y < -2f)
                                                                    {
                                                                        npc.Velocity.Y = -2f;
                                                                    }
                                                                }
                                                                if (num123 > 0f)
                                                                {
                                                                    npc.spriteDirection = 1;
                                                                    npc.rotation = (float)Math.Atan2((double)num124, (double)num123);
                                                                }
                                                                if (num123 < 0f)
                                                                {
                                                                    npc.spriteDirection = -1;
                                                                    npc.rotation = (float)Math.Atan2((double)num124, (double)num123) + 3.14f;
                                                                }
                                                                if (npc.collideX)
                                                                {
                                                                    npc.netUpdate = true;
                                                                    npc.Velocity.X = npc.oldVelocity.X * -0.7f;
                                                                    if (npc.Velocity.X > 0f && npc.Velocity.X < 2f)
                                                                    {
                                                                        npc.Velocity.X = 2f;
                                                                    }
                                                                    if (npc.Velocity.X < 0f && npc.Velocity.X > -2f)
                                                                    {
                                                                        npc.Velocity.X = -2f;
                                                                    }
                                                                }
                                                                if (npc.collideY)
                                                                {
                                                                    npc.netUpdate = true;
                                                                    npc.Velocity.Y = npc.oldVelocity.Y * -0.7f;
                                                                    if (npc.Velocity.Y > 0f && npc.Velocity.Y < 2f)
                                                                    {
                                                                        npc.Velocity.Y = 2f;
                                                                    }
                                                                    if (npc.Velocity.Y < 0f && npc.Velocity.Y > -2f)
                                                                    {
                                                                        npc.Velocity.Y = -2f;
                                                                        return;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (npc.aiStyle == 14)
                                                                {
                                                                    if (npc.Type == 60)
                                                                    {
                                                                        Vector2 arg_908E_0 = new Vector2(npc.Position.X, npc.Position.Y);
                                                                        int arg_908E_1 = npc.width;
                                                                        int arg_908E_2 = npc.height;
                                                                        int arg_908E_3 = 6;
                                                                        float arg_908E_4 = npc.Velocity.X * 0.2f;
                                                                        float arg_908E_5 = npc.Velocity.Y * 0.2f;
                                                                        int arg_908E_6 = 100;
                                                                        Color newColor = default(Color);
                                                                        int num126 = Dust.NewDust(arg_908E_0, arg_908E_1, arg_908E_2, arg_908E_3, arg_908E_4, arg_908E_5, arg_908E_6, newColor, 2f);
                                                                        Main.dust[num126].noGravity = true;
                                                                    }
                                                                    npc.noGravity = true;
                                                                    if (npc.collideX)
                                                                    {
                                                                        npc.Velocity.X = npc.oldVelocity.X * -0.5f;
                                                                        if (npc.direction == -1 && npc.Velocity.X > 0f && npc.Velocity.X < 2f)
                                                                        {
                                                                            npc.Velocity.X = 2f;
                                                                        }
                                                                        if (npc.direction == 1 && npc.Velocity.X < 0f && npc.Velocity.X > -2f)
                                                                        {
                                                                            npc.Velocity.X = -2f;
                                                                        }
                                                                    }
                                                                    if (npc.collideY)
                                                                    {
                                                                        npc.Velocity.Y = npc.oldVelocity.Y * -0.5f;
                                                                        if (npc.Velocity.Y > 0f && npc.Velocity.Y < 1f)
                                                                        {
                                                                            npc.Velocity.Y = 1f;
                                                                        }
                                                                        if (npc.Velocity.Y < 0f && npc.Velocity.Y > -1f)
                                                                        {
                                                                            npc.Velocity.Y = -1f;
                                                                        }
                                                                    }
                                                                    npc.TargetClosest(true);
                                                                    if (npc.direction == -1 && npc.Velocity.X > -4f)
                                                                    {
                                                                        npc.Velocity.X = npc.Velocity.X - 0.1f;
                                                                        if (npc.Velocity.X > 4f)
                                                                        {
                                                                            npc.Velocity.X = npc.Velocity.X - 0.1f;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (npc.Velocity.X > 0f)
                                                                            {
                                                                                npc.Velocity.X = npc.Velocity.X + 0.05f;
                                                                            }
                                                                        }
                                                                        if (npc.Velocity.X < -4f)
                                                                        {
                                                                            npc.Velocity.X = -4f;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (npc.direction == 1 && npc.Velocity.X < 4f)
                                                                        {
                                                                            npc.Velocity.X = npc.Velocity.X + 0.1f;
                                                                            if (npc.Velocity.X < -4f)
                                                                            {
                                                                                npc.Velocity.X = npc.Velocity.X + 0.1f;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (npc.Velocity.X < 0f)
                                                                                {
                                                                                    npc.Velocity.X = npc.Velocity.X - 0.05f;
                                                                                }
                                                                            }
                                                                            if (npc.Velocity.X > 4f)
                                                                            {
                                                                                npc.Velocity.X = 4f;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (npc.directionY == -1 && (double)npc.Velocity.Y > -1.5)
                                                                    {
                                                                        npc.Velocity.Y = npc.Velocity.Y - 0.04f;
                                                                        if ((double)npc.Velocity.Y > 1.5)
                                                                        {
                                                                            npc.Velocity.Y = npc.Velocity.Y - 0.05f;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (npc.Velocity.Y > 0f)
                                                                            {
                                                                                npc.Velocity.Y = npc.Velocity.Y + 0.03f;
                                                                            }
                                                                        }
                                                                        if ((double)npc.Velocity.Y < -1.5)
                                                                        {
                                                                            npc.Velocity.Y = -1.5f;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (npc.directionY == 1 && (double)npc.Velocity.Y < 1.5)
                                                                        {
                                                                            npc.Velocity.Y = npc.Velocity.Y + 0.04f;
                                                                            if ((double)npc.Velocity.Y < -1.5)
                                                                            {
                                                                                npc.Velocity.Y = npc.Velocity.Y + 0.05f;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (npc.Velocity.Y < 0f)
                                                                                {
                                                                                    npc.Velocity.Y = npc.Velocity.Y - 0.03f;
                                                                                }
                                                                            }
                                                                            if ((double)npc.Velocity.Y > 1.5)
                                                                            {
                                                                                npc.Velocity.Y = 1.5f;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (npc.Type == 49 || npc.Type == 51 || npc.Type == 60)
                                                                    {
                                                                        if (npc.wet)
                                                                        {
                                                                            if (npc.Velocity.Y > 0f)
                                                                            {
                                                                                npc.Velocity.Y = npc.Velocity.Y * 0.95f;
                                                                            }
                                                                            npc.Velocity.Y = npc.Velocity.Y - 0.5f;
                                                                            if (npc.Velocity.Y < -4f)
                                                                            {
                                                                                npc.Velocity.Y = -4f;
                                                                            }
                                                                            npc.TargetClosest(true);
                                                                        }
                                                                        if (npc.direction == -1 && npc.Velocity.X > -4f)
                                                                        {
                                                                            npc.Velocity.X = npc.Velocity.X - 0.1f;
                                                                            if (npc.Velocity.X > 4f)
                                                                            {
                                                                                npc.Velocity.X = npc.Velocity.X - 0.1f;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (npc.Velocity.X > 0f)
                                                                                {
                                                                                    npc.Velocity.X = npc.Velocity.X + 0.05f;
                                                                                }
                                                                            }
                                                                            if (npc.Velocity.X < -4f)
                                                                            {
                                                                                npc.Velocity.X = -4f;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (npc.direction == 1 && npc.Velocity.X < 4f)
                                                                            {
                                                                                npc.Velocity.X = npc.Velocity.X + 0.1f;
                                                                                if (npc.Velocity.X < -4f)
                                                                                {
                                                                                    npc.Velocity.X = npc.Velocity.X + 0.1f;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (npc.Velocity.X < 0f)
                                                                                    {
                                                                                        npc.Velocity.X = npc.Velocity.X - 0.05f;
                                                                                    }
                                                                                }
                                                                                if (npc.Velocity.X > 4f)
                                                                                {
                                                                                    npc.Velocity.X = 4f;
                                                                                }
                                                                            }
                                                                        }
                                                                        if (npc.directionY == -1 && (double)npc.Velocity.Y > -1.5)
                                                                        {
                                                                            npc.Velocity.Y = npc.Velocity.Y - 0.04f;
                                                                            if ((double)npc.Velocity.Y > 1.5)
                                                                            {
                                                                                npc.Velocity.Y = npc.Velocity.Y - 0.05f;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (npc.Velocity.Y > 0f)
                                                                                {
                                                                                    npc.Velocity.Y = npc.Velocity.Y + 0.03f;
                                                                                }
                                                                            }
                                                                            if ((double)npc.Velocity.Y < -1.5)
                                                                            {
                                                                                npc.Velocity.Y = -1.5f;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (npc.directionY == 1 && (double)npc.Velocity.Y < 1.5)
                                                                            {
                                                                                npc.Velocity.Y = npc.Velocity.Y + 0.04f;
                                                                                if ((double)npc.Velocity.Y < -1.5)
                                                                                {
                                                                                    npc.Velocity.Y = npc.Velocity.Y + 0.05f;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (npc.Velocity.Y < 0f)
                                                                                    {
                                                                                        npc.Velocity.Y = npc.Velocity.Y - 0.03f;
                                                                                    }
                                                                                }
                                                                                if ((double)npc.Velocity.Y > 1.5)
                                                                                {
                                                                                    npc.Velocity.Y = 1.5f;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    if (npc.Type == 48)
                                                                    {
                                                                        npc.ai[0] += 1f;
                                                                        if (npc.ai[0] == 30f || npc.ai[0] == 60f || npc.ai[0] == 90f)
                                                                        {
                                                                            float num127 = 6f;
                                                                            Vector2 vector18 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                                            float num128 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].width * 0.5f - vector18.X + (float)Main.rand.Next(-100, 101);
                                                                            float num129 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].height * 0.5f - vector18.Y + (float)Main.rand.Next(-100, 101);
                                                                            float num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                                                                            num130 = num127 / num130;
                                                                            num128 *= num130;
                                                                            num129 *= num130;
                                                                            int num131 = 15;
                                                                            ProjectileType num132 = ProjectileType.FEATHER_HARPY;
                                                                            int num133 = Projectile.NewProjectile(vector18.X, vector18.Y, num128, num129, num132, num131, 0f, Main.myPlayer);
                                                                            Main.projectile[num133].timeLeft = 300;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (npc.ai[0] >= (float)(400 + Main.rand.Next(400)))
                                                                            {
                                                                                npc.ai[0] = 0f;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (npc.Type == 62 || npc.Type == 66)
                                                                    {
                                                                        npc.ai[0] += 1f;
                                                                        if (npc.ai[0] == 20f || npc.ai[0] == 40f || npc.ai[0] == 60f || npc.ai[0] == 80f)
                                                                        {
                                                                            float num134 = 0.2f;
                                                                            Vector2 vector19 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                                            float num135 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].width * 0.5f - vector19.X + (float)Main.rand.Next(-100, 101);
                                                                            float num136 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].height * 0.5f - vector19.Y + (float)Main.rand.Next(-100, 101);
                                                                            float num137 = (float)Math.Sqrt((double)(num135 * num135 + num136 * num136));
                                                                            num137 = num134 / num137;
                                                                            num135 *= num137;
                                                                            num136 *= num137;
                                                                            int num138 = 25;
                                                                            ProjectileType num139 = ProjectileType.SICKLE_DEMON;
                                                                            int num140 = Projectile.NewProjectile(vector19.X, vector19.Y, num135, num136, num139, num138, 0f, Main.myPlayer);
                                                                            Main.projectile[num140].timeLeft = 300;
                                                                            return;
                                                                        }
                                                                        if (npc.ai[0] >= (float)(300 + Main.rand.Next(300)))
                                                                        {
                                                                            npc.ai[0] = 0f;
                                                                            return;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (npc.aiStyle == 15)
                                                                    {
                                                                        npc.aiAction = 0;
                                                                        if (npc.ai[3] == 0f && npc.life > 0)
                                                                        {
                                                                            npc.ai[3] = (float)npc.lifeMax;
                                                                        }
                                                                        if (npc.ai[2] == 0f)
                                                                        {
                                                                            npc.ai[0] = -100f;
                                                                            npc.ai[2] = 1f;
                                                                            npc.TargetClosest(true);
                                                                        }
                                                                        if (npc.Velocity.Y == 0f)
                                                                        {
                                                                            npc.Velocity.X = npc.Velocity.X * 0.8f;
                                                                            if ((double)npc.Velocity.X > -0.1 && (double)npc.Velocity.X < 0.1)
                                                                            {
                                                                                npc.Velocity.X = 0f;
                                                                            }
                                                                            npc.ai[0] += 2f;
                                                                            if ((double)npc.life < (double)npc.lifeMax * 0.8)
                                                                            {
                                                                                npc.ai[0] += 1f;
                                                                            }
                                                                            if ((double)npc.life < (double)npc.lifeMax * 0.6)
                                                                            {
                                                                                npc.ai[0] += 1f;
                                                                            }
                                                                            if ((double)npc.life < (double)npc.lifeMax * 0.4)
                                                                            {
                                                                                npc.ai[0] += 2f;
                                                                            }
                                                                            if ((double)npc.life < (double)npc.lifeMax * 0.2)
                                                                            {
                                                                                npc.ai[0] += 3f;
                                                                            }
                                                                            if ((double)npc.life < (double)npc.lifeMax * 0.1)
                                                                            {
                                                                                npc.ai[0] += 4f;
                                                                            }
                                                                            if (npc.ai[0] >= 0f)
                                                                            {
                                                                                npc.netUpdate = true;
                                                                                npc.TargetClosest(true);
                                                                                if (npc.ai[1] == 3f)
                                                                                {
                                                                                    npc.Velocity.Y = -13f;
                                                                                    npc.Velocity.X = npc.Velocity.X + 3.5f * (float)npc.direction;
                                                                                    npc.ai[0] = -200f;
                                                                                    npc.ai[1] = 0f;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (npc.ai[1] == 2f)
                                                                                    {
                                                                                        npc.Velocity.Y = -6f;
                                                                                        npc.Velocity.X = npc.Velocity.X + 4.5f * (float)npc.direction;
                                                                                        npc.ai[0] = -120f;
                                                                                        npc.ai[1] += 1f;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        npc.Velocity.Y = -8f;
                                                                                        npc.Velocity.X = npc.Velocity.X + 4f * (float)npc.direction;
                                                                                        npc.ai[0] = -120f;
                                                                                        npc.ai[1] += 1f;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (npc.ai[0] >= -30f)
                                                                                {
                                                                                    npc.aiAction = 1;
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (npc.target < 255 && ((npc.direction == 1 && npc.Velocity.X < 3f) || (npc.direction == -1 && npc.Velocity.X > -3f)))
                                                                            {
                                                                                if ((npc.direction == -1 && (double)npc.Velocity.X < 0.1) || (npc.direction == 1 && (double)npc.Velocity.X > -0.1))
                                                                                {
                                                                                    npc.Velocity.X = npc.Velocity.X + 0.2f * (float)npc.direction;
                                                                                }
                                                                                else
                                                                                {
                                                                                    npc.Velocity.X = npc.Velocity.X * 0.93f;
                                                                                }
                                                                            }
                                                                        }
                                                                        int num141 = Dust.NewDust(npc.Position, npc.width, npc.height, 4, npc.Velocity.X, npc.Velocity.Y, 255, new Color(0, 80, 255, 80), npc.scale * 1.2f);
                                                                        Main.dust[num141].noGravity = true;
                                                                        Dust expr_A082 = Main.dust[num141];
                                                                        expr_A082.velocity *= 0.5f;
                                                                        if (npc.life > 0)
                                                                        {
                                                                            float num142 = (float)npc.life / (float)npc.lifeMax;
                                                                            num142 = num142 * 0.5f + 0.75f;
                                                                            if (num142 != npc.scale)
                                                                            {
                                                                                npc.Position.X = npc.Position.X + (float)(npc.width / 2);
                                                                                npc.Position.Y = npc.Position.Y + (float)npc.height;
                                                                                npc.scale = num142;
                                                                                npc.width = (int)(98f * npc.scale);
                                                                                npc.height = (int)(92f * npc.scale);
                                                                                npc.Position.X = npc.Position.X - (float)(npc.width / 2);
                                                                                npc.Position.Y = npc.Position.Y - (float)npc.height;
                                                                            }
                                                                            int num143 = (int)((double)npc.lifeMax * 0.05);
                                                                            if ((float)(npc.life + num143) < npc.ai[3])
                                                                            {
                                                                                npc.ai[3] = (float)npc.life;
                                                                                int num144 = Main.rand.Next(1, 4);
                                                                                for (int num145 = 0; num145 < num144; num145++)
                                                                                {
                                                                                    int x = (int)(npc.Position.X + (float)Main.rand.Next(npc.width - 32));
                                                                                    int y = (int)(npc.Position.Y + (float)Main.rand.Next(npc.height - 32));
                                                                                    int npcIndex = NPC.NewNPC(x, y, 1, 0);
                                                                                    Main.npcs[npcIndex] = NPCRegistry.Create(1);
                                                                                    Main.npcs[npcIndex].Velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                                                                                    Main.npcs[npcIndex].Velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                                                                                    Main.npcs[npcIndex].ai[1] = (float)Main.rand.Next(3);
                                                                                    if (npcIndex < MAX_NPCS)
                                                                                    {
                                                                                        NetMessage.SendData(23, -1, -1, "", npcIndex);
                                                                                    }
                                                                                }
                                                                                return;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (npc.aiStyle == 16)
                                                                        {
                                                                            if (npc.direction == 0)
                                                                            {
                                                                                npc.TargetClosest(true);
                                                                            }
                                                                            if (npc.wet)
                                                                            {
                                                                                if (npc.collideX)
                                                                                {
                                                                                    npc.Velocity.X = npc.Velocity.X * -1f;
                                                                                    npc.direction *= -1;
                                                                                }
                                                                                if (npc.collideY)
                                                                                {
                                                                                    if (npc.Velocity.Y > 0f)
                                                                                    {
                                                                                        npc.Velocity.Y = Math.Abs(npc.Velocity.Y) * -1f;
                                                                                        npc.directionY = -1;
                                                                                        npc.ai[0] = -1f;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (npc.Velocity.Y < 0f)
                                                                                        {
                                                                                            npc.Velocity.Y = Math.Abs(npc.Velocity.Y);
                                                                                            npc.directionY = 1;
                                                                                            npc.ai[0] = 1f;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                bool flag13 = false;
                                                                                if (!npc.friendly)
                                                                                {
                                                                                    npc.TargetClosest(false);
                                                                                    if (Main.players[npc.target].wet && !Main.players[npc.target].dead)
                                                                                    {
                                                                                        flag13 = true;
                                                                                    }
                                                                                }
                                                                                if (flag13)
                                                                                {
                                                                                    npc.TargetClosest(true);
                                                                                    if (npc.Type == 65)
                                                                                    {
                                                                                        npc.Velocity.X = npc.Velocity.X + (float)npc.direction * 0.15f;
                                                                                        npc.Velocity.Y = npc.Velocity.Y + (float)npc.directionY * 0.15f;
                                                                                        if (npc.Velocity.X > 5f)
                                                                                        {
                                                                                            npc.Velocity.X = 5f;
                                                                                        }
                                                                                        if (npc.Velocity.X < -5f)
                                                                                        {
                                                                                            npc.Velocity.X = -5f;
                                                                                        }
                                                                                        if (npc.Velocity.Y > 3f)
                                                                                        {
                                                                                            npc.Velocity.Y = 3f;
                                                                                        }
                                                                                        if (npc.Velocity.Y < -3f)
                                                                                        {
                                                                                            npc.Velocity.Y = -3f;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        npc.Velocity.X = npc.Velocity.X + (float)npc.direction * 0.1f;
                                                                                        npc.Velocity.Y = npc.Velocity.Y + (float)npc.directionY * 0.1f;
                                                                                        if (npc.Velocity.X > 3f)
                                                                                        {
                                                                                            npc.Velocity.X = 3f;
                                                                                        }
                                                                                        if (npc.Velocity.X < -3f)
                                                                                        {
                                                                                            npc.Velocity.X = -3f;
                                                                                        }
                                                                                        if (npc.Velocity.Y > 2f)
                                                                                        {
                                                                                            npc.Velocity.Y = 2f;
                                                                                        }
                                                                                        if (npc.Velocity.Y < -2f)
                                                                                        {
                                                                                            npc.Velocity.Y = -2f;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    npc.Velocity.X = npc.Velocity.X + (float)npc.direction * 0.1f;
                                                                                    if (npc.Velocity.X < -1f || npc.Velocity.X > 1f)
                                                                                    {
                                                                                        npc.Velocity.X = npc.Velocity.X * 0.95f;
                                                                                    }
                                                                                    if (npc.ai[0] == -1f)
                                                                                    {
                                                                                        npc.Velocity.Y = npc.Velocity.Y - 0.01f;
                                                                                        if ((double)npc.Velocity.Y < -0.3)
                                                                                        {
                                                                                            npc.ai[0] = 1f;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        npc.Velocity.Y = npc.Velocity.Y + 0.01f;
                                                                                        if ((double)npc.Velocity.Y > 0.3)
                                                                                        {
                                                                                            npc.ai[0] = -1f;
                                                                                        }
                                                                                    }
                                                                                    int num147 = (int)(npc.Position.X + (float)(npc.width / 2)) / 16;
                                                                                    int num148 = (int)(npc.Position.Y + (float)(npc.height / 2)) / 16;
                                                                                    if (Main.tile[num147, num148 - 1] == null)
                                                                                    {
                                                                                        Main.tile[num147, num148 - 1] = new Tile();
                                                                                    }
                                                                                    if (Main.tile[num147, num148 + 1] == null)
                                                                                    {
                                                                                        Main.tile[num147, num148 + 1] = new Tile();
                                                                                    }
                                                                                    if (Main.tile[num147, num148 + 2] == null)
                                                                                    {
                                                                                        Main.tile[num147, num148 + 2] = new Tile();
                                                                                    }
                                                                                    if (Main.tile[num147, num148 - 1].liquid > 128)
                                                                                    {
                                                                                        if (Main.tile[num147, num148 + 1].Active)
                                                                                        {
                                                                                            npc.ai[0] = -1f;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (Main.tile[num147, num148 + 2].Active)
                                                                                            {
                                                                                                npc.ai[0] = -1f;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    if ((double)npc.Velocity.Y > 0.4 || (double)npc.Velocity.Y < -0.4)
                                                                                    {
                                                                                        npc.Velocity.Y = npc.Velocity.Y * 0.95f;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (npc.Velocity.Y == 0f)
                                                                                {
                                                                                    if (npc.Type == 65)
                                                                                    {
                                                                                        npc.Velocity.X = npc.Velocity.X * 0.94f;
                                                                                        if ((double)npc.Velocity.X > -0.2 && (double)npc.Velocity.X < 0.2)
                                                                                        {
                                                                                            npc.Velocity.X = 0f;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        npc.Velocity.Y = (float)Main.rand.Next(-50, -20) * 0.1f;
                                                                                        npc.Velocity.X = (float)Main.rand.Next(-20, 20) * 0.1f;
                                                                                        npc.netUpdate = true;
                                                                                    }
                                                                                }
                                                                                npc.Velocity.Y = npc.Velocity.Y + 0.3f;
                                                                                if (npc.Velocity.Y > 10f)
                                                                                {
                                                                                    npc.Velocity.Y = 10f;
                                                                                }
                                                                                npc.ai[0] = 1f;
                                                                            }
                                                                            npc.rotation = npc.Velocity.Y * (float)npc.direction * 0.1f;
                                                                            if ((double)npc.rotation < -0.2)
                                                                            {
                                                                                npc.rotation = -0.2f;
                                                                            }
                                                                            if ((double)npc.rotation > 0.2)
                                                                            {
                                                                                npc.rotation = 0.2f;
                                                                                return;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (npc.aiStyle == 17)
                                                                            {
                                                                                npc.noGravity = true;
                                                                                if (npc.ai[0] == 0f)
                                                                                {
                                                                                    npc.TargetClosest(true);
                                                                                    if (npc.Velocity.X != 0f || npc.Velocity.Y != 0f)
                                                                                    {
                                                                                        npc.ai[0] = 1f;
                                                                                        npc.netUpdate = true;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Rectangle rectangle3 = new Rectangle((int)Main.players[npc.target].Position.X, (int)Main.players[npc.target].Position.Y, Main.players[npc.target].width, Main.players[npc.target].height);
                                                                                        Rectangle rectangle4 = new Rectangle((int)npc.Position.X - 100, (int)npc.Position.Y - 100, npc.width + 200, npc.height + 200);
                                                                                        if (rectangle4.Intersects(rectangle3) || npc.life < npc.lifeMax)
                                                                                        {
                                                                                            npc.ai[0] = 1f;
                                                                                            npc.Velocity.Y = npc.Velocity.Y - 6f;
                                                                                            npc.netUpdate = true;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (!Main.players[npc.target].dead)
                                                                                    {
                                                                                        if (npc.collideX)
                                                                                        {
                                                                                            npc.Velocity.X = npc.oldVelocity.X * -0.5f;
                                                                                            if (npc.direction == -1 && npc.Velocity.X > 0f && npc.Velocity.X < 2f)
                                                                                            {
                                                                                                npc.Velocity.X = 2f;
                                                                                            }
                                                                                            if (npc.direction == 1 && npc.Velocity.X < 0f && npc.Velocity.X > -2f)
                                                                                            {
                                                                                                npc.Velocity.X = -2f;
                                                                                            }
                                                                                        }
                                                                                        if (npc.collideY)
                                                                                        {
                                                                                            npc.Velocity.Y = npc.oldVelocity.Y * -0.5f;
                                                                                            if (npc.Velocity.Y > 0f && npc.Velocity.Y < 1f)
                                                                                            {
                                                                                                npc.Velocity.Y = 1f;
                                                                                            }
                                                                                            if (npc.Velocity.Y < 0f && npc.Velocity.Y > -1f)
                                                                                            {
                                                                                                npc.Velocity.Y = -1f;
                                                                                            }
                                                                                        }
                                                                                        npc.TargetClosest(true);
                                                                                        if (npc.direction == -1 && npc.Velocity.X > -3f)
                                                                                        {
                                                                                            npc.Velocity.X = npc.Velocity.X - 0.1f;
                                                                                            if (npc.Velocity.X > 3f)
                                                                                            {
                                                                                                npc.Velocity.X = npc.Velocity.X - 0.1f;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (npc.Velocity.X > 0f)
                                                                                                {
                                                                                                    npc.Velocity.X = npc.Velocity.X - 0.05f;
                                                                                                }
                                                                                            }
                                                                                            if (npc.Velocity.X < -3f)
                                                                                            {
                                                                                                npc.Velocity.X = -3f;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (npc.direction == 1 && npc.Velocity.X < 3f)
                                                                                            {
                                                                                                npc.Velocity.X = npc.Velocity.X + 0.1f;
                                                                                                if (npc.Velocity.X < -3f)
                                                                                                {
                                                                                                    npc.Velocity.X = npc.Velocity.X + 0.1f;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (npc.Velocity.X < 0f)
                                                                                                    {
                                                                                                        npc.Velocity.X = npc.Velocity.X + 0.05f;
                                                                                                    }
                                                                                                }
                                                                                                if (npc.Velocity.X > 3f)
                                                                                                {
                                                                                                    npc.Velocity.X = 3f;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        float num149 = Math.Abs(npc.Position.X + (float)(npc.width / 2) - (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2)));
                                                                                        float num150 = Main.players[npc.target].Position.Y - (float)(npc.height / 2);
                                                                                        if (num149 > 50f)
                                                                                        {
                                                                                            num150 -= 100f;
                                                                                        }
                                                                                        if (npc.Position.Y < num150)
                                                                                        {
                                                                                            npc.Velocity.Y = npc.Velocity.Y + 0.05f;
                                                                                            if (npc.Velocity.Y < 0f)
                                                                                            {
                                                                                                npc.Velocity.Y = npc.Velocity.Y + 0.01f;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            npc.Velocity.Y = npc.Velocity.Y - 0.05f;
                                                                                            if (npc.Velocity.Y > 0f)
                                                                                            {
                                                                                                npc.Velocity.Y = npc.Velocity.Y - 0.01f;
                                                                                            }
                                                                                        }
                                                                                        if (npc.Velocity.Y < -3f)
                                                                                        {
                                                                                            npc.Velocity.Y = -3f;
                                                                                        }
                                                                                        if (npc.Velocity.Y > 3f)
                                                                                        {
                                                                                            npc.Velocity.Y = 3f;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                if (npc.wet)
                                                                                {
                                                                                    if (npc.Velocity.Y > 0f)
                                                                                    {
                                                                                        npc.Velocity.Y = npc.Velocity.Y * 0.95f;
                                                                                    }
                                                                                    npc.Velocity.Y = npc.Velocity.Y - 0.5f;
                                                                                    if (npc.Velocity.Y < -4f)
                                                                                    {
                                                                                        npc.Velocity.Y = -4f;
                                                                                    }
                                                                                    npc.TargetClosest(true);
                                                                                    return;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (npc.aiStyle == 18)
                                                                                {
                                                                                    if (npc.direction == 0)
                                                                                    {
                                                                                        npc.TargetClosest(true);
                                                                                    }
                                                                                    if (!npc.wet)
                                                                                    {
                                                                                        npc.rotation += npc.Velocity.X * 0.1f;
                                                                                        if (npc.Velocity.Y == 0f)
                                                                                        {
                                                                                            npc.Velocity.X = npc.Velocity.X * 0.98f;
                                                                                            if ((double)npc.Velocity.X > -0.01 && (double)npc.Velocity.X < 0.01)
                                                                                            {
                                                                                                npc.Velocity.X = 0f;
                                                                                            }
                                                                                        }
                                                                                        npc.Velocity.Y = npc.Velocity.Y + 0.2f;
                                                                                        if (npc.Velocity.Y > 10f)
                                                                                        {
                                                                                            npc.Velocity.Y = 10f;
                                                                                        }
                                                                                        npc.ai[0] = 1f;
                                                                                        return;
                                                                                    }
                                                                                    if (npc.collideX)
                                                                                    {
                                                                                        npc.Velocity.X = npc.Velocity.X * -1f;
                                                                                        npc.direction *= -1;
                                                                                    }
                                                                                    if (npc.collideY)
                                                                                    {
                                                                                        if (npc.Velocity.Y > 0f)
                                                                                        {
                                                                                            npc.Velocity.Y = Math.Abs(npc.Velocity.Y) * -1f;
                                                                                            npc.directionY = -1;
                                                                                            npc.ai[0] = -1f;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (npc.Velocity.Y < 0f)
                                                                                            {
                                                                                                npc.Velocity.Y = Math.Abs(npc.Velocity.Y);
                                                                                                npc.directionY = 1;
                                                                                                npc.ai[0] = 1f;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    bool flag14 = false;
                                                                                    if (!npc.friendly)
                                                                                    {
                                                                                        npc.TargetClosest(false);
                                                                                        if (Main.players[npc.target].wet && !Main.players[npc.target].dead)
                                                                                        {
                                                                                            flag14 = true;
                                                                                        }
                                                                                    }
                                                                                    if (flag14)
                                                                                    {
                                                                                        npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) + 1.57f;
                                                                                        npc.Velocity *= 0.98f;
                                                                                        float num151 = 0.2f;
                                                                                        if (npc.Velocity.X > -num151 && npc.Velocity.X < num151 && npc.Velocity.Y > -num151 && npc.Velocity.Y < num151)
                                                                                        {
                                                                                            npc.TargetClosest(true);
                                                                                            float num152 = 7f;
                                                                                            Vector2 vector20 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                                                            float num153 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector20.X;
                                                                                            float num154 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].height / 2) - vector20.Y;
                                                                                            float num155 = (float)Math.Sqrt((double)(num153 * num153 + num154 * num154));
                                                                                            num155 = num152 / num155;
                                                                                            num153 *= num155;
                                                                                            num154 *= num155;
                                                                                            npc.Velocity.X = num153;
                                                                                            npc.Velocity.Y = num154;
                                                                                            return;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        npc.Velocity.X = npc.Velocity.X + (float)npc.direction * 0.02f;
                                                                                        npc.rotation = npc.Velocity.X * 0.4f;
                                                                                        if (npc.Velocity.X < -1f || npc.Velocity.X > 1f)
                                                                                        {
                                                                                            npc.Velocity.X = npc.Velocity.X * 0.95f;
                                                                                        }
                                                                                        if (npc.ai[0] == -1f)
                                                                                        {
                                                                                            npc.Velocity.Y = npc.Velocity.Y - 0.01f;
                                                                                            if (npc.Velocity.Y < -1f)
                                                                                            {
                                                                                                npc.ai[0] = 1f;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            npc.Velocity.Y = npc.Velocity.Y + 0.01f;
                                                                                            if (npc.Velocity.Y > 1f)
                                                                                            {
                                                                                                npc.ai[0] = -1f;
                                                                                            }
                                                                                        }
                                                                                        int num156 = (int)(npc.Position.X + (float)(npc.width / 2)) / 16;
                                                                                        int num157 = (int)(npc.Position.Y + (float)(npc.height / 2)) / 16;
                                                                                        if (Main.tile[num156, num157 - 1] == null)
                                                                                        {
                                                                                            Main.tile[num156, num157 - 1] = new Tile();
                                                                                        }
                                                                                        if (Main.tile[num156, num157 + 1] == null)
                                                                                        {
                                                                                            Main.tile[num156, num157 + 1] = new Tile();
                                                                                        }
                                                                                        if (Main.tile[num156, num157 + 2] == null)
                                                                                        {
                                                                                            Main.tile[num156, num157 + 2] = new Tile();
                                                                                        }
                                                                                        if (Main.tile[num156, num157 - 1].liquid > 128)
                                                                                        {
                                                                                            if (Main.tile[num156, num157 + 1].Active)
                                                                                            {
                                                                                                npc.ai[0] = -1f;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Main.tile[num156, num157 + 2].Active)
                                                                                                {
                                                                                                    npc.ai[0] = -1f;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            npc.ai[0] = 1f;
                                                                                        }
                                                                                        if ((double)npc.Velocity.Y > 1.2 || (double)npc.Velocity.Y < -1.2)
                                                                                        {
                                                                                            npc.Velocity.Y = npc.Velocity.Y * 0.99f;
                                                                                            return;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (npc.aiStyle == 19)
                                                                                    {
                                                                                        npc.TargetClosest(true);
                                                                                        float num158 = 12f;
                                                                                        Vector2 vector21 = new Vector2(npc.Position.X + (float)npc.width * 0.5f, npc.Position.Y + (float)npc.height * 0.5f);
                                                                                        float num159 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].width / 2) - vector21.X;
                                                                                        float num160 = Main.players[npc.target].Position.Y - vector21.Y;
                                                                                        float num161 = (float)Math.Sqrt((double)(num159 * num159 + num160 * num160));
                                                                                        num161 = num158 / num161;
                                                                                        num159 *= num161;
                                                                                        num160 *= num161;
                                                                                        bool flag15 = false;
                                                                                        if (npc.directionY < 0)
                                                                                        {
                                                                                            npc.rotation = (float)(Math.Atan2((double)num160, (double)num159) + 1.57);
                                                                                            flag15 = ((double)npc.rotation >= -1.2 && (double)npc.rotation <= 1.2);
                                                                                            if ((double)npc.rotation < -0.8)
                                                                                            {
                                                                                                npc.rotation = -0.8f;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if ((double)npc.rotation > 0.8)
                                                                                                {
                                                                                                    npc.rotation = 0.8f;
                                                                                                }
                                                                                            }
                                                                                            if (npc.Velocity.X != 0f)
                                                                                            {
                                                                                                npc.Velocity.X = npc.Velocity.X * 0.9f;
                                                                                                if ((double)npc.Velocity.X > -0.1 || (double)npc.Velocity.X < 0.1)
                                                                                                {
                                                                                                    npc.netUpdate = true;
                                                                                                    npc.Velocity.X = 0f;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        if (npc.ai[0] > 0f)
                                                                                        {
                                                                                            npc.ai[0] -= 1f;
                                                                                        }
                                                                                        if (flag15 && npc.ai[0] == 0f && Collision.CanHit(npc.Position, npc.width, npc.height, Main.players[npc.target].Position, Main.players[npc.target].width, Main.players[npc.target].height))
                                                                                        {
                                                                                            npc.ai[0] = 200f;
                                                                                            int num162 = 10;
                                                                                            ProjectileType num163 = ProjectileType.BALL_SAND_DROP;
                                                                                            int num164 = Projectile.NewProjectile(vector21.X, vector21.Y, num159, num160, num163, num162, 0f, Main.myPlayer);
                                                                                            Main.projectile[num164].ai[0] = 2f;
                                                                                            Main.projectile[num164].timeLeft = 300;
                                                                                            Main.projectile[num164].friendly = false;
                                                                                            NetMessage.SendData(27, -1, -1, "", num164);
                                                                                            npc.netUpdate = true;
                                                                                        }
                                                                                        try
                                                                                        {
                                                                                            int num165 = (int)npc.Position.X / 16;
                                                                                            int num166 = (int)(npc.Position.X + (float)(npc.width / 2)) / 16;
                                                                                            int num167 = (int)(npc.Position.X + (float)npc.width) / 16;
                                                                                            int num168 = (int)(npc.Position.Y + (float)npc.height) / 16;
                                                                                            bool flag16 = false;
                                                                                            if (Main.tile[num165, num168] == null)
                                                                                            {
                                                                                                Main.tile[num165, num168] = new Tile();
                                                                                            }
                                                                                            if (Main.tile[num166, num168] == null)
                                                                                            {
                                                                                                Main.tile[num165, num168] = new Tile();
                                                                                            }
                                                                                            if (Main.tile[num167, num168] == null)
                                                                                            {
                                                                                                Main.tile[num165, num168] = new Tile();
                                                                                            }
                                                                                            if ((Main.tile[num165, num168].Active && Main.tileSolid[(int)Main.tile[num165, num168].type]) || (Main.tile[num166, num168].Active && Main.tileSolid[(int)Main.tile[num166, num168].type]) || (Main.tile[num167, num168].Active && Main.tileSolid[(int)Main.tile[num167, num168].type]))
                                                                                            {
                                                                                                flag16 = true;
                                                                                            }
                                                                                            if (flag16)
                                                                                            {
                                                                                                npc.noGravity = true;
                                                                                                npc.noTileCollide = true;
                                                                                                npc.Velocity.Y = -0.2f;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                npc.noGravity = false;
                                                                                                npc.noTileCollide = false;
                                                                                                if (Main.rand.Next(2) == 0)
                                                                                                {
                                                                                                    Vector2 arg_BA65_0 = new Vector2(npc.Position.X - 4f, npc.Position.Y + (float)npc.height - 8f);
                                                                                                    int arg_BA65_1 = npc.width + 8;
                                                                                                    int arg_BA65_2 = 24;
                                                                                                    int arg_BA65_3 = 32;
                                                                                                    float arg_BA65_4 = 0f;
                                                                                                    float arg_BA65_5 = npc.Velocity.Y / 2f;
                                                                                                    int arg_BA65_6 = 0;
                                                                                                    Color newColor = default(Color);
                                                                                                    int num169 = Dust.NewDust(arg_BA65_0, arg_BA65_1, arg_BA65_2, arg_BA65_3, arg_BA65_4, arg_BA65_5, arg_BA65_6, newColor, 1f);
                                                                                                    Dust expr_BA79_cp_0 = Main.dust[num169];
                                                                                                    expr_BA79_cp_0.velocity.X = expr_BA79_cp_0.velocity.X * 0.4f;
                                                                                                    Dust expr_BA97_cp_0 = Main.dust[num169];
                                                                                                    expr_BA97_cp_0.velocity.Y = expr_BA97_cp_0.velocity.Y * -1f;
                                                                                                    if (Main.rand.Next(2) == 0)
                                                                                                    {
                                                                                                        Main.dust[num169].noGravity = true;
                                                                                                        Main.dust[num169].scale += 0.2f;
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        catch
                                                                                        {
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        public void FindFrame()
        {
            int num = 1;
            int num2 = 0;
            if (this.aiAction == 0)
            {
                if (this.Velocity.Y < 0f)
                {
                    num2 = 2;
                }
                else
                {
                    if (this.Velocity.Y > 0f)
                    {
                        num2 = 3;
                    }
                    else
                    {
                        if (this.Velocity.X != 0f)
                        {
                            num2 = 1;
                        }
                        else
                        {
                            num2 = 0;
                        }
                    }
                }
            }
            else
            {
                if (this.aiAction == 1)
                {
                    num2 = 4;
                }
            }
            if (this.Type == 1 || this.Type == 16 || this.Type == 59)
            {
                this.frameCounter += 1.0;
                if (num2 > 0)
                {
                    this.frameCounter += 1.0;
                }
                if (num2 == 4)
                {
                    this.frameCounter += 1.0;
                }
                if (this.frameCounter >= 8.0)
                {
                    this.frame.Y = this.frame.Y + num;
                    this.frameCounter = 0.0;
                }
                if (this.frame.Y >= num * Main.npcFrameCount[this.Type])
                {
                    this.frame.Y = 0;
                }
            }
            if (this.Type == 50)
            {
                if (this.Velocity.Y != 0f)
                {
                    this.frame.Y = num * 4;
                }
                else
                {
                    this.frameCounter += 1.0;
                    if (num2 > 0)
                    {
                        this.frameCounter += 1.0;
                    }
                    if (num2 == 4)
                    {
                        this.frameCounter += 1.0;
                    }
                    if (this.frameCounter >= 8.0)
                    {
                        this.frame.Y = this.frame.Y + num;
                        this.frameCounter = 0.0;
                    }
                    if (this.frame.Y >= num * 4)
                    {
                        this.frame.Y = 0;
                    }
                }
            }
            if (this.Type == 61)
            {
                this.spriteDirection = this.direction;
                this.rotation = this.Velocity.X * 0.1f;
                if (this.Velocity.X == 0f && this.Velocity.Y == 0f)
                {
                    this.frame.Y = 0;
                    this.frameCounter = 0.0;
                }
                else
                {
                    this.frameCounter += 1.0;
                    if (this.frameCounter < 4.0)
                    {
                        this.frame.Y = num;
                    }
                    else
                    {
                        this.frame.Y = num * 2;
                        if (this.frameCounter >= 7.0)
                        {
                            this.frameCounter = 0.0;
                        }
                    }
                }
            }
            if (this.Type == 62 || this.Type == 66)
            {
                this.spriteDirection = this.direction;
                this.rotation = this.Velocity.X * 0.1f;
                this.frameCounter += 1.0;
                if (this.frameCounter < 6.0)
                {
                    this.frame.Y = 0;
                }
                else
                {
                    this.frame.Y = num;
                    if (this.frameCounter >= 11.0)
                    {
                        this.frameCounter = 0.0;
                    }
                }
            }
            if (this.Type == 63 || this.Type == 64)
            {
                this.frameCounter += 1.0;
                if (this.frameCounter < 6.0)
                {
                    this.frame.Y = 0;
                }
                else
                {
                    if (this.frameCounter < 12.0)
                    {
                        this.frame.Y = num;
                    }
                    else
                    {
                        if (this.frameCounter < 18.0)
                        {
                            this.frame.Y = num * 2;
                        }
                        else
                        {
                            this.frame.Y = num * 3;
                            if (this.frameCounter >= 23.0)
                            {
                                this.frameCounter = 0.0;
                            }
                        }
                    }
                }
            }
            if (this.Type == 2 || this.Type == 23)
            {
                if (this.Type == 2)
                {
                    if (this.Velocity.X > 0f)
                    {
                        this.spriteDirection = 1;
                        this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X);
                    }
                    if (this.Velocity.X < 0f)
                    {
                        this.spriteDirection = -1;
                        this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 3.14f;
                    }
                }
                this.frameCounter += 1.0;
                if (this.frameCounter >= 8.0)
                {
                    this.frame.Y = this.frame.Y + num;
                    this.frameCounter = 0.0;
                }
                if (this.frame.Y >= num * Main.npcFrameCount[this.Type])
                {
                    this.frame.Y = 0;
                }
            }
            if (this.Type == 55 || this.Type == 57 || this.Type == 58)
            {
                this.spriteDirection = this.direction;
                this.frameCounter += 1.0;
                if (this.wet)
                {
                    if (this.frameCounter < 6.0)
                    {
                        this.frame.Y = 0;
                    }
                    else
                    {
                        if (this.frameCounter < 12.0)
                        {
                            this.frame.Y = num;
                        }
                        else
                        {
                            if (this.frameCounter < 18.0)
                            {
                                this.frame.Y = num * 2;
                            }
                            else
                            {
                                if (this.frameCounter < 24.0)
                                {
                                    this.frame.Y = num * 3;
                                }
                                else
                                {
                                    this.frameCounter = 0.0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (this.frameCounter < 6.0)
                    {
                        this.frame.Y = num * 4;
                    }
                    else
                    {
                        if (this.frameCounter < 12.0)
                        {
                            this.frame.Y = num * 5;
                        }
                        else
                        {
                            this.frameCounter = 0.0;
                        }
                    }
                }
            }
            if (this.Type == 69)
            {
                if (this.ai[0] < 190f)
                {
                    this.frameCounter += 1.0;
                    if (this.frameCounter >= 6.0)
                    {
                        this.frameCounter = 0.0;
                        this.frame.Y = this.frame.Y + num;
                        if (this.frame.Y / num >= Main.npcFrameCount[this.Type] - 1)
                        {
                            this.frame.Y = 0;
                        }
                    }
                }
                else
                {
                    this.frameCounter = 0.0;
                    this.frame.Y = num * (Main.npcFrameCount[this.Type] - 1);
                }
            }
            if (this.Type == 67)
            {
                if (this.Velocity.Y == 0f)
                {
                    this.spriteDirection = this.direction;
                }
                this.frameCounter += 1.0;
                if (this.frameCounter >= 6.0)
                {
                    this.frameCounter = 0.0;
                    this.frame.Y = this.frame.Y + num;
                    if (this.frame.Y / num >= Main.npcFrameCount[this.Type])
                    {
                        this.frame.Y = 0;
                    }
                }
            }
            if (this.Type == 65)
            {
                this.spriteDirection = this.direction;
                this.frameCounter += 1.0;
                if (this.wet)
                {
                    if (this.frameCounter < 6.0)
                    {
                        this.frame.Y = 0;
                    }
                    else
                    {
                        if (this.frameCounter < 12.0)
                        {
                            this.frame.Y = num;
                        }
                        else
                        {
                            if (this.frameCounter < 18.0)
                            {
                                this.frame.Y = num * 2;
                            }
                            else
                            {
                                if (this.frameCounter < 24.0)
                                {
                                    this.frame.Y = num * 3;
                                }
                                else
                                {
                                    this.frameCounter = 0.0;
                                }
                            }
                        }
                    }
                }
            }
            if (this.Type == 48 || this.Type == 49 || this.Type == 51 || this.Type == 60)
            {
                if (this.Velocity.X > 0f)
                {
                    this.spriteDirection = 1;
                }
                if (this.Velocity.X < 0f)
                {
                    this.spriteDirection = -1;
                }
                this.rotation = this.Velocity.X * 0.1f;
                this.frameCounter += 1.0;
                if (this.frameCounter >= 6.0)
                {
                    this.frame.Y = this.frame.Y + num;
                    this.frameCounter = 0.0;
                }
                if (this.frame.Y >= num * 4)
                {
                    this.frame.Y = 0;
                }
            }
            if (this.Type == 42)
            {
                this.frameCounter += 1.0;
                if (this.frameCounter < 2.0)
                {
                    this.frame.Y = 0;
                }
                else
                {
                    if (this.frameCounter < 4.0)
                    {
                        this.frame.Y = num;
                    }
                    else
                    {
                        if (this.frameCounter < 6.0)
                        {
                            this.frame.Y = num * 2;
                        }
                        else
                        {
                            if (this.frameCounter < 8.0)
                            {
                                this.frame.Y = num;
                            }
                            else
                            {
                                this.frameCounter = 0.0;
                            }
                        }
                    }
                }
            }
            if (this.Type == 43 || this.Type == 56)
            {
                this.frameCounter += 1.0;
                if (this.frameCounter < 6.0)
                {
                    this.frame.Y = 0;
                }
                else
                {
                    if (this.frameCounter < 12.0)
                    {
                        this.frame.Y = num;
                    }
                    else
                    {
                        if (this.frameCounter < 18.0)
                        {
                            this.frame.Y = num * 2;
                        }
                        else
                        {
                            if (this.frameCounter < 24.0)
                            {
                                this.frame.Y = num;
                            }
                        }
                    }
                }
                if (this.frameCounter == 23.0)
                {
                    this.frameCounter = 0.0;
                }
            }
            if (this.Type == 17 || this.Type == 18 || this.Type == 19 || this.Type == 20 || this.Type == 22 || this.Type == 38 || this.Type == 26 || this.Type == 27 || this.Type == 28 || this.Type == 31 || this.Type == 21 || this.Type == 44 || this.Type == 54 || this.Type == 37)
            {
                if (this.Velocity.Y == 0f)
                {
                    if (this.direction == 1)
                    {
                        this.spriteDirection = 1;
                    }
                    if (this.direction == -1)
                    {
                        this.spriteDirection = -1;
                    }
                    if (this.Velocity.X == 0f)
                    {
                        this.frame.Y = 0;
                        this.frameCounter = 0.0;
                    }
                    else
                    {
                        this.frameCounter += (double)(Math.Abs(this.Velocity.X) * 2f);
                        this.frameCounter += 1.0;
                        if (this.frameCounter > 6.0)
                        {
                            this.frame.Y = this.frame.Y + num;
                            this.frameCounter = 0.0;
                        }
                        if (this.frame.Y / num >= Main.npcFrameCount[this.Type])
                        {
                            this.frame.Y = num * 2;
                        }
                    }
                }
                else
                {
                    this.frameCounter = 0.0;
                    this.frame.Y = num;
                    if (this.Type == 21 || this.Type == 31 || this.Type == 44)
                    {
                        this.frame.Y = 0;
                    }
                }
            }
            else
            {
                if (this.Type == 3 || this.Type == 52 || this.Type == 53)
                {
                    if (this.Velocity.Y == 0f)
                    {
                        if (this.direction == 1)
                        {
                            this.spriteDirection = 1;
                        }
                        if (this.direction == -1)
                        {
                            this.spriteDirection = -1;
                        }
                    }
                    if (this.Velocity.Y != 0f || (this.direction == -1 && this.Velocity.X > 0f) || (this.direction == 1 && this.Velocity.X < 0f))
                    {
                        this.frameCounter = 0.0;
                        this.frame.Y = num * 2;
                    }
                    else
                    {
                        if (this.Velocity.X == 0f)
                        {
                            this.frameCounter = 0.0;
                            this.frame.Y = 0;
                        }
                        else
                        {
                            this.frameCounter += (double)Math.Abs(this.Velocity.X);
                            if (this.frameCounter < 8.0)
                            {
                                this.frame.Y = 0;
                            }
                            else
                            {
                                if (this.frameCounter < 16.0)
                                {
                                    this.frame.Y = num;
                                }
                                else
                                {
                                    if (this.frameCounter < 24.0)
                                    {
                                        this.frame.Y = num * 2;
                                    }
                                    else
                                    {
                                        if (this.frameCounter < 32.0)
                                        {
                                            this.frame.Y = num;
                                        }
                                        else
                                        {
                                            this.frameCounter = 0.0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (this.Type == 46 || this.Type == 47)
                    {
                        if (this.Velocity.Y == 0f)
                        {
                            if (this.direction == 1)
                            {
                                this.spriteDirection = 1;
                            }
                            if (this.direction == -1)
                            {
                                this.spriteDirection = -1;
                            }
                            if (this.Velocity.X == 0f)
                            {
                                this.frame.Y = 0;
                                this.frameCounter = 0.0;
                            }
                            else
                            {
                                this.frameCounter += (double)(Math.Abs(this.Velocity.X) * 1f);
                                this.frameCounter += 1.0;
                                if (this.frameCounter > 6.0)
                                {
                                    this.frame.Y = this.frame.Y + num;
                                    this.frameCounter = 0.0;
                                }
                                if (this.frame.Y / num >= Main.npcFrameCount[this.Type])
                                {
                                    this.frame.Y = 0;
                                }
                            }
                        }
                        else
                        {
                            if (this.Velocity.Y < 0f)
                            {
                                this.frameCounter = 0.0;
                                this.frame.Y = num * 4;
                            }
                            else
                            {
                                if (this.Velocity.Y > 0f)
                                {
                                    this.frameCounter = 0.0;
                                    this.frame.Y = num * 6;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.Type == 4)
                        {
                            this.frameCounter += 1.0;
                            if (this.frameCounter < 7.0)
                            {
                                this.frame.Y = 0;
                            }
                            else
                            {
                                if (this.frameCounter < 14.0)
                                {
                                    this.frame.Y = num;
                                }
                                else
                                {
                                    if (this.frameCounter < 21.0)
                                    {
                                        this.frame.Y = num * 2;
                                    }
                                    else
                                    {
                                        this.frameCounter = 0.0;
                                        this.frame.Y = 0;
                                    }
                                }
                            }
                            if (this.ai[0] > 1f)
                            {
                                this.frame.Y = this.frame.Y + num * 3;
                            }
                        }
                        else
                        {
                            if (this.Type == 5)
                            {
                                this.frameCounter += 1.0;
                                if (this.frameCounter >= 8.0)
                                {
                                    this.frame.Y = this.frame.Y + num;
                                    this.frameCounter = 0.0;
                                }
                                if (this.frame.Y >= num * Main.npcFrameCount[this.Type])
                                {
                                    this.frame.Y = 0;
                                }
                            }
                            else
                            {
                                if (this.Type == 6)
                                {
                                    this.frameCounter += 1.0;
                                    if (this.frameCounter >= 8.0)
                                    {
                                        this.frame.Y = this.frame.Y + num;
                                        this.frameCounter = 0.0;
                                    }
                                    if (this.frame.Y >= num * Main.npcFrameCount[this.Type])
                                    {
                                        this.frame.Y = 0;
                                    }
                                }
                                else
                                {
                                    if (this.Type == 24)
                                    {
                                        if (this.Velocity.Y == 0f)
                                        {
                                            if (this.direction == 1)
                                            {
                                                this.spriteDirection = 1;
                                            }
                                            if (this.direction == -1)
                                            {
                                                this.spriteDirection = -1;
                                            }
                                        }
                                        if (this.ai[1] > 0f)
                                        {
                                            if (this.frame.Y < 4)
                                            {
                                                this.frameCounter = 0.0;
                                            }
                                            this.frameCounter += 1.0;
                                            if (this.frameCounter <= 4.0)
                                            {
                                                this.frame.Y = num * 4;
                                            }
                                            else
                                            {
                                                if (this.frameCounter <= 8.0)
                                                {
                                                    this.frame.Y = num * 5;
                                                }
                                                else
                                                {
                                                    if (this.frameCounter <= 12.0)
                                                    {
                                                        this.frame.Y = num * 6;
                                                    }
                                                    else
                                                    {
                                                        if (this.frameCounter <= 16.0)
                                                        {
                                                            this.frame.Y = num * 7;
                                                        }
                                                        else
                                                        {
                                                            if (this.frameCounter <= 20.0)
                                                            {
                                                                this.frame.Y = num * 8;
                                                            }
                                                            else
                                                            {
                                                                this.frame.Y = num * 9;
                                                                this.frameCounter = 100.0;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            this.frameCounter += 1.0;
                                            if (this.frameCounter <= 4.0)
                                            {
                                                this.frame.Y = 0;
                                            }
                                            else
                                            {
                                                if (this.frameCounter <= 8.0)
                                                {
                                                    this.frame.Y = num;
                                                }
                                                else
                                                {
                                                    if (this.frameCounter <= 12.0)
                                                    {
                                                        this.frame.Y = num * 2;
                                                    }
                                                    else
                                                    {
                                                        this.frame.Y = num * 3;
                                                        if (this.frameCounter >= 16.0)
                                                        {
                                                            this.frameCounter = 0.0;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (this.Type == 29 || this.Type == 32 || this.Type == 45)
                                        {
                                            if (this.Velocity.Y == 0f)
                                            {
                                                if (this.direction == 1)
                                                {
                                                    this.spriteDirection = 1;
                                                }
                                                if (this.direction == -1)
                                                {
                                                    this.spriteDirection = -1;
                                                }
                                            }
                                            this.frame.Y = 0;
                                            if (this.Velocity.Y != 0f)
                                            {
                                                this.frame.Y = this.frame.Y + num;
                                            }
                                            else
                                            {
                                                if (this.ai[1] > 0f)
                                                {
                                                    this.frame.Y = this.frame.Y + num * 2;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (this.Type == 34)
            {
                this.frameCounter += 1.0;
                if (this.frameCounter >= 4.0)
                {
                    this.frame.Y = this.frame.Y + num;
                    this.frameCounter = 0.0;
                }
                if (this.frame.Y >= num * Main.npcFrameCount[this.Type])
                {
                    this.frame.Y = 0;
                }
            }
        }
        
        public void TargetClosest(bool faceTarget = true)
        {
            float num = -1f;
            for (int i = 0; i < 255; i++)
            {
                if (Main.players[i].Active && !Main.players[i].dead && (num == -1f || Math.Abs(Main.players[i].Position.X + (float)(Main.players[i].width / 2) - this.Position.X + (float)(this.width / 2)) + Math.Abs(Main.players[i].Position.Y + (float)(Main.players[i].height / 2) - this.Position.Y + (float)(this.height / 2)) < num))
                {
                    num = Math.Abs(Main.players[i].Position.X + (float)(Main.players[i].width / 2) - this.Position.X + (float)(this.width / 2)) + Math.Abs(Main.players[i].Position.Y + (float)(Main.players[i].height / 2) - this.Position.Y + (float)(this.height / 2));
                    this.target = i;
                }
            }
            if (this.target < 0 || this.target >= 255)
            {
                this.target = 0;
            }
            this.targetRect = new Rectangle((int)Main.players[this.target].Position.X, (int)Main.players[this.target].Position.Y, Main.players[this.target].width, Main.players[this.target].height);
            if (faceTarget)
            {
                this.direction = 1;
                if ((float)(this.targetRect.X + this.targetRect.Width / 2) < this.Position.X + (float)(this.width / 2))
                {
                    this.direction = -1;
                }
                this.directionY = 1;
                if ((float)(this.targetRect.Y + this.targetRect.Height / 2) < this.Position.Y + (float)(this.height / 2))
                {
                    this.directionY = -1;
                }
            }
            if (this.direction != this.oldDirection || this.directionY != this.oldDirectionY || this.target != this.oldTarget)
            {
                this.netUpdate = true;
            }
        }
        
        public void CheckActive()
        {
            if (this.Active)
            {
                if (this.Type == 8 || this.Type == 9 || this.Type == 11 || this.Type == 12 || this.Type == 14 || this.Type == 15 || this.Type == 40 || this.Type == 41)
                {
                    return;
                }
                if (this.townNPC)
                {
                    if ((double)this.Position.Y < Main.worldSurface * 18.0)
                    {
                        Rectangle rectangle = new Rectangle((int)(this.Position.X + (float)(this.width / 2) - (float)NPC.townRangeX), (int)(this.Position.Y + (float)(this.height / 2) - (float)NPC.townRangeY), NPC.townRangeX * 2, NPC.townRangeY * 2);
                        for (int i = 0; i < 255; i++)
                        {
                            if (Main.players[i].Active && rectangle.Intersects(new Rectangle((int)Main.players[i].Position.X, (int)Main.players[i].Position.Y, Main.players[i].width, Main.players[i].height)))
                            {
                                Main.players[i].townNPCs += (int)NPC.npcSlots;
                            }
                        }
                    }
                    return;
                }
                bool flag = false;
                Rectangle rectangle2 = new Rectangle((int)(this.Position.X + (float)(this.width / 2) - (float)NPC.activeRangeX), (int)(this.Position.Y + (float)(this.height / 2) - (float)NPC.activeRangeY), NPC.activeRangeX * 2, NPC.activeRangeY * 2);
                Rectangle rectangle3 = new Rectangle((int)((double)(this.Position.X + (float)(this.width / 2)) - (double)NPC.sWidth * 0.5 - (double)this.width), (int)((double)(this.Position.Y + (float)(this.height / 2)) - (double)NPC.sHeight * 0.5 - (double)this.height), NPC.sWidth + this.width * 2, NPC.sHeight + this.height * 2);
                for (int j = 0; j < 255; j++)
                {
                    if (Main.players[j].Active)
                    {
                        if (rectangle2.Intersects(new Rectangle((int)Main.players[j].Position.X, (int)Main.players[j].Position.Y, Main.players[j].width, Main.players[j].height)))
                        {
                            flag = true;
                            if (this.Type != 25 && this.Type != 30 && this.Type != 33)
                            {
                                Main.players[j].activeNPCs += (int)NPC.npcSlots;
                            }
                        }
                        if (rectangle3.Intersects(new Rectangle((int)Main.players[j].Position.X, (int)Main.players[j].Position.Y, Main.players[j].width, Main.players[j].height)))
                        {
                            this.timeLeft = NPC.ACTIVE_TIME;
                        }
                        if (this.Type == 7 || this.Type == 10 || this.Type == 13 || this.Type == 39)
                        {
                            flag = true;
                        }
                        if (this.boss || this.Type == 35 || this.Type == 36)
                        {
                            flag = true;
                        }
                    }
                }
                this.timeLeft--;
                if (this.timeLeft <= 0)
                {
                    flag = false;
                }
                if (!flag)
                {
                    NPC.noSpawnCycle = true;
                    this.Active = false;
                    
                    this.life = 0;
                    NetMessage.SendData(23, -1, -1, "", this.whoAmI);
                }
            }
        }
        
        public static void SpawnNPC()
        {
            if (NPC.noSpawnCycle)
            {
                NPC.noSpawnCycle = false;
                return;
            }
            bool flag = false;
            bool flag2 = false;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            for (int i = 0; i < 255; i++)
            {
                if (Main.players[i].Active)
                {
                    num3++;
                }
            }
            for (int j = 0; j < 255; j++)
            {
                if (Main.players[j].Active && !Main.players[j].dead)
                {
                    bool flag3 = false;
                    bool flag4 = false;
                    if (Main.players[j].Active && Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0 && (double)Main.players[j].Position.Y < Main.worldSurface * 16.0 + (double)NPC.sHeight)
                    {
                        int num4 = 3000;
                        if ((double)Main.players[j].Position.X > Main.invasionX * 16.0 - (double)num4 && (double)Main.players[j].Position.X < Main.invasionX * 16.0 + (double)num4)
                        {
                            flag3 = true;
                        }
                    }
                    flag = false;
                    NPC.spawnRate = NPC.defaultSpawnRate;
                    NPC.maxSpawns = NPC.defaultMaxSpawns;
                    if (Main.players[j].Position.Y > (float)((Main.maxTilesY - 200) * 16))
                    {
                        NPC.spawnRate = (int)((float)NPC.spawnRate * 0.4f);
                        NPC.maxSpawns = (int)((float)NPC.maxSpawns * 2.1f);
                    }
                    else
                    {
                        if ((double)Main.players[j].Position.Y > Main.rockLayer * 16.0 + (double)NPC.sHeight)
                        {
                            NPC.spawnRate = (int)((double)NPC.spawnRate * 0.4);
                            NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.9f);
                        }
                        else
                        {
                            if ((double)Main.players[j].Position.Y > Main.worldSurface * 16.0 + (double)NPC.sHeight)
                            {
                                NPC.spawnRate = (int)((double)NPC.spawnRate * 0.5);
                                NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.7f);
                            }
                            else
                            {
                                if (!Main.dayTime)
                                {
                                    NPC.spawnRate = (int)((double)NPC.spawnRate * 0.6);
                                    NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.3f);
                                    if (Main.bloodMoon)
                                    {
                                        NPC.spawnRate = (int)((double)NPC.spawnRate * 0.3);
                                        NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.8f);
                                    }
                                }
                            }
                        }
                    }
                    if (Main.players[j].zoneDungeon)
                    {
                        NPC.spawnRate = (int)((double)NPC.defaultSpawnRate * 0.22);
                        NPC.maxSpawns = NPC.defaultMaxSpawns * 2;
                    }
                    else
                    {
                        if (Main.players[j].zoneJungle)
                        {
                            NPC.spawnRate = (int)((double)NPC.spawnRate * 0.3);
                            NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.6f);
                        }
                        else
                        {
                            if (Main.players[j].zoneEvil)
                            {
                                NPC.spawnRate = (int)((double)NPC.spawnRate * 0.4);
                                NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.6f);
                            }
                            else
                            {
                                if (Main.players[j].zoneMeteor)
                                {
                                    NPC.spawnRate = (int)((double)NPC.spawnRate * 0.4);
                                    NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.1f);
                                }
                            }
                        }
                    }
                    if ((double)Main.players[j].activeNPCs < (double)NPC.maxSpawns * 0.2)
                    {
                        NPC.spawnRate = (int)((float)NPC.spawnRate * 0.6f);
                    }
                    else
                    {
                        if ((double)Main.players[j].activeNPCs < (double)NPC.maxSpawns * 0.4)
                        {
                            NPC.spawnRate = (int)((float)NPC.spawnRate * 0.7f);
                        }
                        else
                        {
                            if ((double)Main.players[j].activeNPCs < (double)NPC.maxSpawns * 0.6)
                            {
                                NPC.spawnRate = (int)((float)NPC.spawnRate * 0.8f);
                            }
                            else
                            {
                                if ((double)Main.players[j].activeNPCs < (double)NPC.maxSpawns * 0.8)
                                {
                                    NPC.spawnRate = (int)((float)NPC.spawnRate * 0.9f);
                                }
                            }
                        }
                    }
                    if ((double)(Main.players[j].Position.Y * 16f) > (Main.worldSurface + Main.rockLayer) / 2.0 || Main.players[j].zoneEvil)
                    {
                        if ((double)Main.players[j].activeNPCs < (double)NPC.maxSpawns * 0.2)
                        {
                            NPC.spawnRate = (int)((float)NPC.spawnRate * 0.7f);
                        }
                        else
                        {
                            if ((double)Main.players[j].activeNPCs < (double)NPC.maxSpawns * 0.4)
                            {
                                NPC.spawnRate = (int)((float)NPC.spawnRate * 0.9f);
                            }
                        }
                    }
                    if (Main.players[j].inventory[Main.players[j].selectedItemIndex].Type == 148)
                    {
                        NPC.spawnRate = (int)((double)NPC.spawnRate * 0.75);
                        NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.5f);
                    }
                    if (Main.players[j].enemySpawns)
                    {
                        NPC.spawnRate = (int)((double)NPC.spawnRate * 0.5);
                        NPC.maxSpawns = (int)((float)NPC.maxSpawns * 2f);
                    }
                    if ((double)NPC.spawnRate < (double)NPC.defaultSpawnRate * 0.1)
                    {
                        NPC.spawnRate = (int)((double)NPC.defaultSpawnRate * 0.1);
                    }
                    if (NPC.maxSpawns > NPC.defaultMaxSpawns * 3)
                    {
                        NPC.maxSpawns = NPC.defaultMaxSpawns * 3;
                    }
                    if (flag3)
                    {
                        NPC.maxSpawns = (int)((double)NPC.defaultMaxSpawns * (1.0 + 0.4 * (double)num3));
                        NPC.spawnRate = 30;
                    }
                    if (Main.players[j].zoneDungeon && !NPC.downedBoss3)
                    {
                        NPC.spawnRate = 10;
                    }
                    bool flag5 = false;
                    if (!flag3 && (!Main.bloodMoon || Main.dayTime) && !Main.players[j].zoneDungeon && !Main.players[j].zoneEvil && !Main.players[j].zoneMeteor)
                    {
                        if (Main.players[j].townNPCs == 1f)
                        {
                            if (Main.rand.Next(3) <= 1)
                            {
                                flag5 = true;
                                NPC.maxSpawns = (int)((double)((float)NPC.maxSpawns) * 0.6);
                            }
                            else
                            {
                                NPC.spawnRate = (int)((float)NPC.spawnRate * 2f);
                            }
                        }
                        else
                        {
                            if (Main.players[j].townNPCs == 2f)
                            {
                                if (Main.rand.Next(3) == 0)
                                {
                                    flag5 = true;
                                    NPC.maxSpawns = (int)((double)((float)NPC.maxSpawns) * 0.6);
                                }
                                else
                                {
                                    NPC.spawnRate = (int)((float)NPC.spawnRate * 3f);
                                }
                            }
                            else
                            {
                                if (Main.players[j].townNPCs >= 3f)
                                {
                                    flag5 = true;
                                    NPC.maxSpawns = (int)((double)((float)NPC.maxSpawns) * 0.6);
                                }
                            }
                        }
                    }
                    if (Main.players[j].Active && !Main.players[j].dead && Main.players[j].activeNPCs < (float)NPC.maxSpawns && Main.rand.Next(NPC.spawnRate) == 0)
                    {
                        int num5 = (int)(Main.players[j].Position.X / 16f) - NPC.spawnRangeX;
                        int num6 = (int)(Main.players[j].Position.X / 16f) + NPC.spawnRangeX;
                        int num7 = (int)(Main.players[j].Position.Y / 16f) - NPC.spawnRangeY;
                        int num8 = (int)(Main.players[j].Position.Y / 16f) + NPC.spawnRangeY;
                        int num9 = (int)(Main.players[j].Position.X / 16f) - NPC.safeRangeX;
                        int num10 = (int)(Main.players[j].Position.X / 16f) + NPC.safeRangeX;
                        int num11 = (int)(Main.players[j].Position.Y / 16f) - NPC.safeRangeY;
                        int num12 = (int)(Main.players[j].Position.Y / 16f) + NPC.safeRangeY;
                        if (num5 < 0)
                        {
                            num5 = 0;
                        }
                        if (num6 > Main.maxTilesX)
                        {
                            num6 = Main.maxTilesX;
                        }
                        if (num7 < 0)
                        {
                            num7 = 0;
                        }
                        if (num8 > Main.maxTilesY)
                        {
                            num8 = Main.maxTilesY;
                        }
                        int k = 0;
                        while (k < 50)
                        {
                            int num13 = Main.rand.Next(num5, num6);
                            int num14 = Main.rand.Next(num7, num8);
                            if (Main.tile[num13, num14].Active && Main.tileSolid[(int)Main.tile[num13, num14].type])
                            {
                                goto IL_ACE;
                            }
                            if (!Main.wallHouse[(int)Main.tile[num13, num14].wall])
                            {
                                if (!flag3 && (double)num14 < Main.worldSurface * 0.30000001192092896 && !flag5 && ((double)num13 < (double)Main.maxTilesX * 0.35 || (double)num13 > (double)Main.maxTilesX * 0.65))
                                {
                                    byte arg_986_0 = Main.tile[num13, num14].type;
                                    num = num13;
                                    num2 = num14;
                                    flag = true;
                                    flag2 = true;
                                }
                                else
                                {
                                    int l = num14;
                                    while (l < Main.maxTilesY)
                                    {
                                        if (Main.tile[num13, l].Active && Main.tileSolid[(int)Main.tile[num13, l].type])
                                        {
                                            if (num13 < num9 || num13 > num10 || l < num11 || l > num12)
                                            {
                                                byte arg_9F4_0 = Main.tile[num13, l].type;
                                                num = num13;
                                                num2 = l;
                                                flag = true;
                                                break;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            l++;
                                        }
                                    }
                                }
                                if (!flag)
                                {
                                    goto IL_ACE;
                                }
                                int num15 = num - NPC.spawnSpaceX / 2;
                                int num16 = num + NPC.spawnSpaceX / 2;
                                int num17 = num2 - NPC.spawnSpaceY;
                                int num18 = num2;
                                if (num15 < 0)
                                {
                                    flag = false;
                                }
                                if (num16 > Main.maxTilesX)
                                {
                                    flag = false;
                                }
                                if (num17 < 0)
                                {
                                    flag = false;
                                }
                                if (num18 > Main.maxTilesY)
                                {
                                    flag = false;
                                }
                                if (flag)
                                {
                                    for (int m = num15; m < num16; m++)
                                    {
                                        for (int n = num17; n < num18; n++)
                                        {
                                            if (Main.tile[m, n].Active && Main.tileSolid[(int)Main.tile[m, n].type])
                                            {
                                                flag = false;
                                                break;
                                            }
                                            if (Main.tile[m, n].lava)
                                            {
                                                flag = false;
                                                break;
                                            }
                                        }
                                    }
                                    goto IL_ACE;
                                }
                                goto IL_ACE;
                            }
                        IL_AD4:
                            k++;
                            continue;
                        IL_ACE:
                            if (!flag && !flag)
                            {
                                goto IL_AD4;
                            }
                            break;
                        }
                    }
                    if (flag)
                    {
                        Rectangle rectangle = new Rectangle(num * 16, num2 * 16, 16, 16);
                        for (int num19 = 0; num19 < 255; num19++)
                        {
                            if (Main.players[num19].Active)
                            {
                                Rectangle rectangle2 = new Rectangle((int)(Main.players[num19].Position.X + (float)(Main.players[num19].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.players[num19].Position.Y + (float)(Main.players[num19].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                if (rectangle.Intersects(rectangle2))
                                {
                                    flag = false;
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        if (Main.players[j].zoneDungeon && (!Main.tileDungeon[(int)Main.tile[num, num2].type] || Main.tile[num, num2 - 1].wall == 0))
                        {
                            flag = false;
                        }
                        if (Main.tile[num, num2 - 1].liquid > 0 && Main.tile[num, num2 - 2].liquid > 0 && !Main.tile[num, num2 - 1].lava)
                        {
                            flag4 = true;
                        }
                    }
                    if (flag)
                    {
                        flag = false;
                        int num20 = (int)Main.tile[num, num2].type;
                        int npcIndex = 1000;
                        if (flag2)
                        {
                            NPC.NewNPC(num * 16 + 8, num2 * 16, 48, 0);
                        }
                        else
                        {
                            if (flag3)
                            {
                                if (Main.rand.Next(9) == 0)
                                {
                                    NPC.NewNPC(num * 16 + 8, num2 * 16, 29, 0);
                                }
                                else
                                {
                                    if (Main.rand.Next(5) == 0)
                                    {
                                        NPC.NewNPC(num * 16 + 8, num2 * 16, 26, 0);
                                    }
                                    else
                                    {
                                        if (Main.rand.Next(3) == 0)
                                        {
                                            NPC.NewNPC(num * 16 + 8, num2 * 16, 27, 0);
                                        }
                                        else
                                        {
                                            NPC.NewNPC(num * 16 + 8, num2 * 16, 28, 0);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (flag4 && (num < 250 || num > Main.maxTilesX - 250) && num20 == 53 && (double)num2 < Main.rockLayer)
                                {
                                    if (Main.rand.Next(8) == 0)
                                    {
                                        NPC.NewNPC(num * 16 + 8, num2 * 16, 65, 0);
                                    }
                                    if (Main.rand.Next(3) == 0)
                                    {
                                        NPC.NewNPC(num * 16 + 8, num2 * 16, 67, 0);
                                    }
                                    else
                                    {
                                        NPC.NewNPC(num * 16 + 8, num2 * 16, 64, 0);
                                    }
                                }
                                else
                                {
                                    if (flag4 && (((double)num2 > Main.rockLayer && Main.rand.Next(2) == 0) || num20 == 60))
                                    {
                                        NPC.NewNPC(num * 16 + 8, num2 * 16, 58, 0);
                                    }
                                    else
                                    {
                                        if (flag4 && (double)num2 > Main.worldSurface && Main.rand.Next(3) == 0)
                                        {
                                            NPC.NewNPC(num * 16 + 8, num2 * 16, 63, 0);
                                        }
                                        else
                                        {
                                            if (flag4 && Main.rand.Next(4) == 0)
                                            {
                                                if (Main.players[j].zoneEvil)
                                                {
                                                    NPC.NewNPC(num * 16 + 8, num2 * 16, 57, 0);
                                                }
                                                else
                                                {
                                                    NPC.NewNPC(num * 16 + 8, num2 * 16, 55, 0);
                                                }
                                            }
                                            else
                                            {
                                                if (flag5)
                                                {
                                                    if (flag4)
                                                    {
                                                        NPC.NewNPC(num * 16 + 8, num2 * 16, 55, 0);
                                                    }
                                                    else
                                                    {
                                                        if (num20 != 2)
                                                        {
                                                            return;
                                                        }
                                                        NPC.NewNPC(num * 16 + 8, num2 * 16, 46, 0);
                                                    }
                                                }
                                                else
                                                {
                                                    if (Main.players[j].zoneDungeon)
                                                    {
                                                        if (!NPC.downedBoss3)
                                                        {
                                                            npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 68, 0);
                                                        }
                                                        else
                                                        {
                                                            if (Main.rand.Next(3) == 0)
                                                            {
                                                                npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 34, 0);
                                                            }
                                                            else
                                                            {
                                                                if (Main.rand.Next(6) == 0)
                                                                {
                                                                    npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 32, 0);
                                                                }
                                                                else
                                                                {
                                                                    npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 31, 0);
                                                                    if (Main.rand.Next(4) == 0)
                                                                    {
                                                                        Main.npcs[npcIndex] = NPCRegistry.Create("Big Boned");
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Main.rand.Next(5) == 0)
                                                                        {
                                                                            Main.npcs[npcIndex] = NPCRegistry.Create("Short Bones");
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Main.players[j].zoneMeteor)
                                                        {
                                                            npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 23, 0);
                                                        }
                                                        else
                                                        {
                                                            if (Main.players[j].zoneEvil && Main.rand.Next(50) == 0)
                                                            {
                                                                npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 7, 1);
                                                            }
                                                            else
                                                            {
                                                                if (num20 == 60 && Main.rand.Next(500) == 0 && !Main.dayTime)
                                                                {
                                                                    npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 52, 0);
                                                                }
                                                                else
                                                                {
                                                                    if (num20 == 60 && (double)num2 > (Main.worldSurface + Main.rockLayer) / 2.0)
                                                                    {
                                                                        if (Main.rand.Next(3) == 0)
                                                                        {
                                                                            npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 43, 0);
                                                                            Main.npcs[npcIndex].ai[0] = (float)num;
                                                                            Main.npcs[npcIndex].ai[1] = (float)num2;
                                                                            Main.npcs[npcIndex].netUpdate = true;
                                                                        }
                                                                        else
                                                                        {
                                                                            npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 42, 0);
                                                                            if (Main.rand.Next(4) == 0)
                                                                            {
                                                                                Main.npcs[npcIndex] = NPCRegistry.Create("Little Stinger");
                                                                            }
                                                                            else if (Main.rand.Next(4) == 0)
                                                                            {
                                                                                Main.npcs[npcIndex] = NPCRegistry.Create("Big Stinger");
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num20 == 60 && Main.rand.Next(4) == 0)
                                                                        {
                                                                            npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 51, 0);
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num20 == 60 && Main.rand.Next(8) == 0)
                                                                            {
                                                                                npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 56, 0);
                                                                                Main.npcs[npcIndex].ai[0] = (float)num;
                                                                                Main.npcs[npcIndex].ai[1] = (float)num2;
                                                                                Main.npcs[npcIndex].netUpdate = true;
                                                                            }
                                                                            else
                                                                            {
                                                                                if ((num20 == 22 && Main.players[j].zoneEvil) || num20 == 23 || num20 == 25)
                                                                                {
                                                                                    npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 6, 0);
                                                                                    if (Main.rand.Next(3) == 0)
                                                                                    {
                                                                                        Main.npcs[npcIndex] = NPCRegistry.Create("Little Eater");
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (Main.rand.Next(3) == 0)
                                                                                        {
                                                                                            Main.npcs[npcIndex] = NPCRegistry.Create("Big Eater");
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if ((double)num2 <= Main.worldSurface)
                                                                                    {
                                                                                        if (Main.dayTime)
                                                                                        {
                                                                                            int num22 = Math.Abs(num - Main.spawnTileX);
                                                                                            if (num22 < Main.maxTilesX / 3 && Main.rand.Next(10) == 0 && num20 == 2)
                                                                                            {
                                                                                                NPC.NewNPC(num * 16 + 8, num2 * 16, 46, 0);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (num22 > Main.maxTilesX / 3 && num20 == 2 && Main.rand.Next(300) == 0 && !NPC.AnyNPCs(50))
                                                                                                {
                                                                                                    npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 50, 0);
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (num20 == 53 && Main.rand.Next(5) == 0 && !flag4)
                                                                                                    {
                                                                                                        npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 69, 0);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (num20 == 53 && !flag4)
                                                                                                        {
                                                                                                            npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 61, 0);
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 1, 0);
                                                                                                            if (num20 == 60)
                                                                                                            {
                                                                                                                Main.npcs[npcIndex] = NPCRegistry.Create("Jungle Slime");
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (Main.rand.Next(3) == 0 || num22 < 200)
                                                                                                                {
                                                                                                                    Main.npcs[npcIndex] = NPCRegistry.Create("Green Slime");
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (Main.rand.Next(10) == 0 && num22 > 400)
                                                                                                                    {
                                                                                                                        Main.npcs[npcIndex] = NPCRegistry.Create("Purple Slime");
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (Main.rand.Next(6) == 0 || (Main.moonPhase == 4 && Main.rand.Next(2) == 0))
                                                                                            {
                                                                                                npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 2, 0);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Main.rand.Next(250) == 0 && Main.bloodMoon)
                                                                                                {
                                                                                                    NPC.NewNPC(num * 16 + 8, num2 * 16, 53, 0);
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    NPC.NewNPC(num * 16 + 8, num2 * 16, 3, 0);
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if ((double)num2 <= Main.rockLayer)
                                                                                        {
                                                                                            if (Main.rand.Next(30) == 0)
                                                                                            {
                                                                                                npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 10, 1);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 1, 0);
                                                                                                if (Main.rand.Next(5) == 0)
                                                                                                {
                                                                                                    Main.npcs[npcIndex] = NPCRegistry.Create("Yellow Slime");
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (Main.rand.Next(2) == 0)
                                                                                                    {
                                                                                                        Main.npcs[npcIndex] = NPCRegistry.Create("Blue Slime");
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Main.npcs[npcIndex] = NPCRegistry.Create("Red Slime");
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (num2 > Main.maxTilesY - 190)
                                                                                            {
                                                                                                if (Main.rand.Next(40) == 0 && !NPC.AnyNPCs(39))
                                                                                                {
                                                                                                    npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 39, 1);
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (Main.rand.Next(20) == 0)
                                                                                                    {
                                                                                                        npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 24, 0);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (Main.rand.Next(12) == 0)
                                                                                                        {
                                                                                                            if (Main.rand.Next(10) == 0)
                                                                                                            {
                                                                                                                npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 66, 0);
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 62, 0);
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (Main.rand.Next(3) == 0)
                                                                                                            {
                                                                                                                npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 59, 0);
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 60, 0);
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Main.rand.Next(35) == 0)
                                                                                                {
                                                                                                    npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 10, 1);
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (Main.rand.Next(10) == 0)
                                                                                                    {
                                                                                                        npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 16, 0);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (Main.rand.Next(4) == 0)
                                                                                                        {
                                                                                                            npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 1, 0);
                                                                                                            if (Main.players[j].zoneJungle)
                                                                                                            {
                                                                                                                Main.npcs[npcIndex] = NPCRegistry.Create("Jungle Slime");
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Main.npcs[npcIndex] = NPCRegistry.Create("Black Slime");
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (Main.rand.Next(2) == 0)
                                                                                                            {
                                                                                                                if ((double)num2 > (Main.rockLayer + (double)Main.maxTilesY) / 2.0 && Main.rand.Next(700) == 0)
                                                                                                                {
                                                                                                                    npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 45, 0);
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (Main.rand.Next(15) == 0)
                                                                                                                    {
                                                                                                                        npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 44, 0);
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 21, 0);
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (Main.players[j].zoneJungle)
                                                                                                                {
                                                                                                                    npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 51, 0);
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, 49, 0);
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (Main.npcs[npcIndex].Type == 1 && Main.rand.Next(250) == 0)
                        {
                            Main.npcs[npcIndex] = NPCRegistry.Create("Pinky");
                        }
                        if (npcIndex < MAX_NPCS)
                        {
                            NetMessage.SendData(23, -1, -1, "", npcIndex);
                            return;
                        }
                        break;
                    }
                }
            }
        }
        
        public static void SpawnOnPlayer(Player player, int playerIndex, int Type)
        {
            bool flag = false;
            int num = 0;
            int num2 = 0;
            int num3 = (int)(player.Position.X / 16f) - NPC.spawnRangeX * 3;
            int num4 = (int)(player.Position.X / 16f) + NPC.spawnRangeX * 3;
            int num5 = (int)(player.Position.Y / 16f) - NPC.spawnRangeY * 3;
            int num6 = (int)(player.Position.Y / 16f) + NPC.spawnRangeY * 3;
            int num7 = (int)(player.Position.X / 16f) - NPC.safeRangeX;
            int num8 = (int)(player.Position.X / 16f) + NPC.safeRangeX;
            int num9 = (int)(player.Position.Y / 16f) - NPC.safeRangeY;
            int num10 = (int)(player.Position.Y / 16f) + NPC.safeRangeY;
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (num4 > Main.maxTilesX)
            {
                num4 = Main.maxTilesX;
            }
            if (num5 < 0)
            {
                num5 = 0;
            }
            if (num6 > Main.maxTilesY)
            {
                num6 = Main.maxTilesY;
            }
            for (int i = 0; i < 1000; i++)
            {
                int j = 0;
                while (j < 100)
                {
                    int num11 = Main.rand.Next(num3, num4);
                    int num12 = Main.rand.Next(num5, num6);
                    if (Main.tile[num11, num12].Active && Main.tileSolid[(int)Main.tile[num11, num12].type])
                    {
                        goto IL_2E1;
                    }
                    if (Main.tile[num11, num12].wall != 1)
                    {
                        int k = num12;
                        while (k < Main.maxTilesY)
                        {
                            if (Main.tile[num11, k].Active && Main.tileSolid[(int)Main.tile[num11, k].type])
                            {
                                if (num11 < num7 || num11 > num8 || k < num9 || k > num10)
                                {
                                    byte arg_220_0 = Main.tile[num11, k].type;
                                    num = num11;
                                    num2 = k;
                                    flag = true;
                                    break;
                                }
                                break;
                            }
                            else
                            {
                                k++;
                            }
                        }
                        if (!flag)
                        {
                            goto IL_2E1;
                        }
                        int num13 = num - NPC.spawnSpaceX / 2;
                        int num14 = num + NPC.spawnSpaceX / 2;
                        int num15 = num2 - NPC.spawnSpaceY;
                        int num16 = num2;
                        if (num13 < 0)
                        {
                            flag = false;
                        }
                        if (num14 > Main.maxTilesX)
                        {
                            flag = false;
                        }
                        if (num15 < 0)
                        {
                            flag = false;
                        }
                        if (num16 > Main.maxTilesY)
                        {
                            flag = false;
                        }
                        if (flag)
                        {
                            for (int l = num13; l < num14; l++)
                            {
                                for (int m = num15; m < num16; m++)
                                {
                                    if (Main.tile[l, m].Active && Main.tileSolid[(int)Main.tile[l, m].type])
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                            goto IL_2E1;
                        }
                        goto IL_2E1;
                    }
                IL_2E7:
                    j++;
                    continue;
                IL_2E1:
                    if (!flag && !flag)
                    {
                        goto IL_2E7;
                    }
                    break;
                }
                if (flag)
                {
                    Rectangle rectangle = new Rectangle(num * 16, num2 * 16, 16, 16);
                    for (int n = 0; n < 255; n++)
                    {
                        if (Main.players[n].Active)
                        {
                            Rectangle rectangle2 = new Rectangle((int)(Main.players[n].Position.X + (float)(Main.players[n].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.players[n].Position.Y + (float)(Main.players[n].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                            if (rectangle.Intersects(rectangle2))
                            {
                                flag = false;
                            }
                        }
                    }
                }
                if (flag)
                {
                    break;
                }
            }
            if (flag)
            {
                //Boss summon?
                int npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, Type, 1);
                Main.npcs[npcIndex].target = playerIndex;
                String str = Main.npcs[npcIndex].Name;
                if (Main.npcs[npcIndex].Type == 13)
                {
                    str = "Eater of Worlds";
                }
                if (Main.npcs[npcIndex].Type == 35)
                {
                    str = "Skeletron";
                }
                if (npcIndex < MAX_NPCS)
                {
                    NetMessage.SendData(23, -1, -1, "", npcIndex);
                }
                
                NetMessage.SendData(25, -1, -1, str + " has awoken!", 255, 175f, 75f, 255f);
            }
        }
        
        public static int NewNPC(int x, int y, int type, int start = 0)
        {
            int npcIndex = -1;
            for (int i = start; i < MAX_NPCS; i++)
            {
                if (!Main.npcs[i].Active)
                {
                    npcIndex = i;
                    break;
                }
            }

            if (npcIndex >= 0)
            {
                NPC npc = NPCRegistry.Create(type);
                NPC oldNPC = Main.npcs[npcIndex];
                npc.Position.X = (float)(x - oldNPC.width / 2);
                npc.Position.Y = (float)(y - oldNPC.height);
                npc.Active = true;
                npc.timeLeft = (int)((double)NPC.ACTIVE_TIME * 1.25);
                npc.wet = Collision.WetCollision(oldNPC.Position, oldNPC.width, oldNPC.height);

                if (!WorldGen.gen)
                {
                    NPCSpawnEvent npcEvent = new NPCSpawnEvent();
                    npcEvent.NPC = npc;
                    Sender sender = new Sender();
                    sender.Op = true;
                    npcEvent.Sender = sender;
                    Program.server.getPluginManager().processHook(Hooks.NPC_SPAWN, npcEvent);
                    if (npcEvent.Cancelled)
                    {
                        return 1000;
                    }
                }
                

                Main.npcs[npcIndex] = npc;

                if (type == 50)
                {
                    NetMessage.SendData(25, -1, -1, npc.Name + " has awoken!", 255, 175f, 75f, 255f);
                }
                return npcIndex;
            }
            return MAX_NPCS;
        }

        public static void Transform(int npcIndex, int newType)
        {
            NPC npc = Main.npcs[npcIndex];
            Vector2 vector = npc.Velocity;
            int num = npc.spriteDirection;
            npc = NPCRegistry.Create(newType);
            Main.npcs[npcIndex] = npc;
            npc.spriteDirection = num;
            npc.TargetClosest(true);
            npc.Velocity = vector;
            npc.netUpdate = true;
            NetMessage.SendData(23, -1, -1, "", npc.whoAmI);
        }
        
        public double StrikeNPC(int Damage, float knockBack, int hitDirection)
        {
            if (!this.Active || this.life <= 0)
            {
                return 0.0;
            }
            double num = Main.CalculateDamage(Damage, this.defense);

            if (num >= 1.0)
            {
                if (this.townNPC)
                {
                    this.ai[0] = 1f;
                    this.ai[1] = (float)(300 + Main.rand.Next(300));
                    this.ai[2] = 0f;
                    this.direction = hitDirection;
                    this.netUpdate = true;
                }
                if (this.aiStyle == 8)
                {
                    this.ai[0] = 400f;
                    this.TargetClosest(true);
                }
                this.life -= (int)num;
                if (knockBack > 0f && this.knockBackResist > 0f)
                {
                    if (!this.noGravity)
                    {
                        this.Velocity.Y = -knockBack * 0.75f * this.knockBackResist;
                    }
                    else
                    {
                        this.Velocity.Y = -knockBack * 0.5f * this.knockBackResist;
                    }
                    this.Velocity.X = knockBack * (float)hitDirection * this.knockBackResist;
                }
                this.HitEffect(hitDirection, num);

                if (this.life <= 0)
                {
                    NPC.noSpawnCycle = true;
                    if (this.townNPC && this.Type != 37)
                    {
                        NetMessage.SendData(25, -1, -1, this.Name + " was slain...", 255, 255f, 25f, 25f);
                    }
                    if (this.townNPC && this.homeless && WorldGen.spawnNPC == this.Type)
                    {
                        WorldGen.spawnNPC = 0;
                    }

                    NPCDeathEvent Event = new NPCDeathEvent();
                    Event.Npc = this;
                    Event.Damage = Damage;
                    Event.KnockBack = knockBack;
                    Event.HitDirection = hitDirection;
                    Program.server.getPluginManager().processHook(Plugin.Hooks.NPC_DEATH, Event);
                    if (Event.Cancelled)
                    {
                        return 0.0;
                    }

                    this.NPCLoot();
                    this.Active = false;
                    if (this.Type == 26 || this.Type == 27 || this.Type == 28 || this.Type == 29)
                    {
                        Main.invasionSize--;
                    }
                }
                return num;
            }
            return 0.0;
        }
        
        public void NPCLoot()
        {
            if (this.Type == 1 || this.Type == 16)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 23, Main.rand.Next(1, 3), false);
            }
            if (this.Type == 2)
            {
                if (Main.rand.Next(3) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 38, 1, false);
                }
                else
                {
                    if (Main.rand.Next(100) == 0)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 236, 1, false);
                    }
                }
            }
            if (this.Type == 58)
            {
                if (Main.rand.Next(500) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 263, 1, false);
                }
                else
                {
                    if (Main.rand.Next(40) == 0)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 118, 1, false);
                    }
                }
            }
            if (this.Type == 3 && Main.rand.Next(50) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 216, 1, false);
            }
            if (this.Type == 66)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 267, 1, false);
            }
            if (this.Type == 62 && Main.rand.Next(50) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 272, 1, false);
            }
            if (this.Type == 52)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 251, 1, false);
            }
            if (this.Type == 53)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 239, 1, false);
            }
            if (this.Type == 54)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 260, 1, false);
            }
            if (this.Type == 55)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 261, 1, false);
            }
            if (this.Type == 69 && Main.rand.Next(10) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 323, 1, false);
            }
            if (this.Type == 4)
            {
                int stack = Main.rand.Next(30) + 20;
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 47, stack, false);
                stack = Main.rand.Next(20) + 10;
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 56, stack, false);
                stack = Main.rand.Next(20) + 10;
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 56, stack, false);
                stack = Main.rand.Next(20) + 10;
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 56, stack, false);
                stack = Main.rand.Next(3) + 1;
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 59, stack, false);
            }
            if (this.Type == 6 && Main.rand.Next(3) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 68, 1, false);
            }
            if (this.Type == 7 || this.Type == 8 || this.Type == 9)
            {
                if (Main.rand.Next(3) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 68, Main.rand.Next(1, 3), false);
                }
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 69, Main.rand.Next(3, 9), false);
            }
            if ((this.Type == 10 || this.Type == 11 || this.Type == 12) && Main.rand.Next(500) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 215, 1, false);
            }
            if (this.Type == 47 && Main.rand.Next(75) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 243, 1, false);
            }
            if (this.Type == 39 || this.Type == 40 || this.Type == 41)
            {
                if (Main.rand.Next(100) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 220, 1, false);
                }
                else
                {
                    if (Main.rand.Next(100) == 0)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 218, 1, false);
                    }
                }
            }
            if (this.Type == 13 || this.Type == 14 || this.Type == 15)
            {
                int stack2 = Main.rand.Next(1, 4);
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 86, stack2, false);
                if (Main.rand.Next(2) == 0)
                {
                    stack2 = Main.rand.Next(2, 6);
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 56, stack2, false);
                }
                if (this.boss)
                {
                    stack2 = Main.rand.Next(10, 30);
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 56, stack2, false);
                    stack2 = Main.rand.Next(10, 31);
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 56, stack2, false);
                }
            }
            if (this.Type == 63 || this.Type == 64)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 282, Main.rand.Next(1, 5), false);
            }
            if (this.Type == 21 || this.Type == 44)
            {
                if (Main.rand.Next(25) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 118, 1, false);
                }
                else
                {
                    if (this.Type == 44)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 166, Main.rand.Next(1, 4), false);
                    }
                }
            }
            if (this.Type == 45)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 238, 1, false);
            }
            if (this.Type == 50)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, Main.rand.Next(256, 259), 1, false);
            }
            if (this.Type == 23 && Main.rand.Next(50) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 116, 1, false);
            }
            if (this.Type == 24)
            {
                if (Main.rand.Next(50) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 112, 1, false);
                }
                else
                {
                    if (Main.rand.Next(500) == 0)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 244, 1, false);
                    }
                }
            }
            if (this.Type == 31 || this.Type == 32)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 154, 1, false);
            }
            if (this.Type == 26 || this.Type == 27 || this.Type == 28 || this.Type == 29)
            {
                if (Main.rand.Next(400) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 128, 1, false);
                }
                else
                {
                    if (Main.rand.Next(200) == 0)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 160, 1, false);
                    }
                    else
                    {
                        if (Main.rand.Next(2) == 0)
                        {
                            int stack3 = Main.rand.Next(1, 6);
                            Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 161, stack3, false);
                        }
                    }
                }
            }
            if (this.Type == 42)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 209, 1, false);
            }
            if (this.Type == 43 && Main.rand.Next(5) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 210, 1, false);
            }
            if (this.Type == 65)
            {
                if (Main.rand.Next(50) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 268, 1, false);
                }
                else
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 319, 1, false);
                }
            }
            if (this.Type == 48 && Main.rand.Next(5) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 320, 1, false);
            }
            if (this.boss)
            {
                if (this.Type == 4)
                {
                    NPC.downedBoss1 = true;
                }
                if (this.Type == 13 || this.Type == 14 || this.Type == 15)
                {
                    NPC.downedBoss2 = true;
                    this.Name = "Eater of Worlds";
                }
                if (this.Type == 35)
                {
                    NPC.downedBoss3 = true;
                    this.Name = "Skeletron";
                }
                int stack4 = Main.rand.Next(5, 16);
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 28, stack4, false);
                int num = Main.rand.Next(5) + 5;
                for (int i = 0; i < num; i++)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 58, 1, false);
                }
                
                NetMessage.SendData(25, -1, -1, this.Name + " has been defeated!", 255, 175f, 75f, 255f);
            }
            if (Main.rand.Next(7) == 0 && this.lifeMax > 1)
            {
                if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(this.Position, this.width, this.height)].statMana < Main.players[(int)Player.FindClosest(this.Position, this.width, this.height)].statManaMax)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 184, 1, false);
                }
                else
                {
                    if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(this.Position, this.width, this.height)].statLife < Main.players[(int)Player.FindClosest(this.Position, this.width, this.height)].statLifeMax)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 58, 1, false);
                    }
                }
            }
            float num2 = this.value;
            num2 *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
            if (Main.rand.Next(5) == 0)
            {
                num2 *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;
            }
            if (Main.rand.Next(10) == 0)
            {
                num2 *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;
            }
            if (Main.rand.Next(15) == 0)
            {
                num2 *= 1f + (float)Main.rand.Next(15, 31) * 0.01f;
            }
            if (Main.rand.Next(20) == 0)
            {
                num2 *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;
            }
            while ((int)num2 > 0)
            {
                if (num2 > 1000000f)
                {
                    int num3 = (int)(num2 / 1000000f);
                    if (num3 > 50 && Main.rand.Next(2) == 0)
                    {
                        num3 /= Main.rand.Next(3) + 1;
                    }
                    if (Main.rand.Next(2) == 0)
                    {
                        num3 /= Main.rand.Next(3) + 1;
                    }
                    num2 -= (float)(1000000 * num3);
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 74, num3, false);
                }
                else
                {
                    if (num2 > 10000f)
                    {
                        int num4 = (int)(num2 / 10000f);
                        if (num4 > 50 && Main.rand.Next(2) == 0)
                        {
                            num4 /= Main.rand.Next(3) + 1;
                        }
                        if (Main.rand.Next(2) == 0)
                        {
                            num4 /= Main.rand.Next(3) + 1;
                        }
                        num2 -= (float)(10000 * num4);
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 73, num4, false);
                    }
                    else
                    {
                        if (num2 > 100f)
                        {
                            int num5 = (int)(num2 / 100f);
                            if (num5 > 50 && Main.rand.Next(2) == 0)
                            {
                                num5 /= Main.rand.Next(3) + 1;
                            }
                            if (Main.rand.Next(2) == 0)
                            {
                                num5 /= Main.rand.Next(3) + 1;
                            }
                            num2 -= (float)(100 * num5);
                            Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 72, num5, false);
                        }
                        else
                        {
                            int num6 = (int)num2;
                            if (num6 > 50 && Main.rand.Next(2) == 0)
                            {
                                num6 /= Main.rand.Next(3) + 1;
                            }
                            if (Main.rand.Next(2) == 0)
                            {
                                num6 /= Main.rand.Next(4) + 1;
                            }
                            if (num6 < 1)
                            {
                                num6 = 1;
                            }
                            num2 -= (float)num6;
                            Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, 71, num6, false);
                        }
                    }
                }
            }
        }

        public void HitEffect(int hitDirection = 0, double dmg = 10.0)
        {
            if (this.Type == 1 || this.Type == 16)
            {
                if (this.life > 0)
                {
                    int num = 0;
                    while ((double)num < dmg / (double)this.lifeMax * 100.0)
                    {
                        Dust.NewDust(this.Position, this.width, this.height, 4, (float)hitDirection, -1f, this.alpha, this.color, 1f);
                        num++;
                    }
                }
                else
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Dust.NewDust(this.Position, this.width, this.height, 4, (float)(2 * hitDirection), -2f, this.alpha, this.color, 1f);
                    }
                    if (this.Type == 16)
                    {
                        int spawnedSlimes = Main.rand.Next(2) + 2;
                        for (int slimeNum = 0; slimeNum < spawnedSlimes; slimeNum++)
                        {
                            int npcIndex = NPC.NewNPC((int)(this.Position.X + (float)(this.width / 2)), (int)(this.Position.Y + (float)this.height), 1, 0);
                            Main.npcs[npcIndex] = NPCRegistry.Create("Baby Slime");
                            Main.npcs[npcIndex].Velocity.X = this.Velocity.X * 2f;
                            Main.npcs[npcIndex].Velocity.Y = this.Velocity.Y;
                            NPC npc = Main.npcs[npcIndex];
                            npc.Velocity.X = npc.Velocity.X + ((float)Main.rand.Next(-20, 20) * 0.1f + (float)(slimeNum * this.direction) * 0.3f);
                            npc.Velocity.Y = npc.Velocity.Y - ((float)Main.rand.Next(0, 10) * 0.1f + (float)slimeNum);
                            npc.ai[1] = (float)slimeNum;
                            if (npcIndex < MAX_NPCS)
                            {
                                NetMessage.SendData(23, -1, -1, "", npcIndex);
                            }
                        }
                    }
                }
            }
            else if (this.Type == 59 || this.Type == 60)
            {
                if (this.life > 0)
                {
                    int num4 = 0;
                    while ((double)num4 < dmg / (double)this.lifeMax * 80.0)
                    {
                        Vector2 arg_350_0 = this.Position;
                        int arg_350_1 = this.width;
                        int arg_350_2 = this.height;
                        int arg_350_3 = 6;
                        float arg_350_4 = (float)(hitDirection * 2);
                        float arg_350_5 = -1f;
                        int arg_350_6 = this.alpha;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_350_0, arg_350_1, arg_350_2, arg_350_3, arg_350_4, arg_350_5, arg_350_6, newColor, 1.5f);
                        num4++;
                    }
                    return;
                }
                for (int k = 0; k < 40; k++)
                {
                    Vector2 arg_3AB_0 = this.Position;
                    int arg_3AB_1 = this.width;
                    int arg_3AB_2 = this.height;
                    int arg_3AB_3 = 6;
                    float arg_3AB_4 = (float)(hitDirection * 2);
                    float arg_3AB_5 = -1f;
                    int arg_3AB_6 = this.alpha;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_3AB_0, arg_3AB_1, arg_3AB_2, arg_3AB_3, arg_3AB_4, arg_3AB_5, arg_3AB_6, newColor, 1.5f);
                }
                if (this.Type == 59)
                {
                    int num5 = (int)(this.Position.X + (float)(this.width / 2)) / 16;
                    int num6 = (int)(this.Position.Y + (float)(this.height / 2)) / 16;
                    Main.tile[num5, num6].lava = true;
                    if (Main.tile[num5, num6].liquid < 200)
                    {
                        Main.tile[num5, num6].liquid = 200;
                    }
                    WorldGen.TileFrame(num5, num6, false, false);
                    return;
                }
            }
            else if (this.Type == 50)
            {
                if (this.life > 0)
                {
                    int num7 = 0;
                    while ((double)num7 < dmg / (double)this.lifeMax * 300.0)
                    {
                        Dust.NewDust(this.Position, this.width, this.height, 4, (float)hitDirection, -1f, 175, new Color(0, 80, 255, 100), 1f);
                        num7++;
                    }
                    return;
                }
                for (int l = 0; l < 200; l++)
                {
                    Dust.NewDust(this.Position, this.width, this.height, 4, (float)(2 * hitDirection), -2f, 175, new Color(0, 80, 255, 100), 1f);
                }
                int num8 = Main.rand.Next(4) + 4;
                for (int m = 0; m < num8; m++)
                {
                    int x = (int)(this.Position.X + (float)Main.rand.Next(this.width - 32));
                    int y = (int)(this.Position.Y + (float)Main.rand.Next(this.height - 32));
                    int npcIndex = NPC.NewNPC(x, y, 1, 0);
                    Main.npcs[npcIndex] = NPCRegistry.Create(1);
                    Main.npcs[npcIndex].Velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                    Main.npcs[npcIndex].Velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                    Main.npcs[npcIndex].ai[1] = (float)Main.rand.Next(3);
                    if (npcIndex < 1000)
                    {
                        NetMessage.SendData(23, -1, -1, "", npcIndex);
                    }
                }
                return;
            }
            else if (this.Type == 49 || this.Type == 51)
            {
                if (this.life > 0)
                {
                    int num10 = 0;
                    while ((double)num10 < dmg / (double)this.lifeMax * 30.0)
                    {
                        Vector2 arg_69A_0 = this.Position;
                        int arg_69A_1 = this.width;
                        int arg_69A_2 = this.height;
                        int arg_69A_3 = 5;
                        float arg_69A_4 = (float)hitDirection;
                        float arg_69A_5 = -1f;
                        int arg_69A_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_69A_0, arg_69A_1, arg_69A_2, arg_69A_3, arg_69A_4, arg_69A_5, arg_69A_6, newColor, 1f);
                        num10++;
                    }
                    return;
                }
                for (int n = 0; n < 15; n++)
                {
                    Vector2 arg_6F0_0 = this.Position;
                    int arg_6F0_1 = this.width;
                    int arg_6F0_2 = this.height;
                    int arg_6F0_3 = 5;
                    float arg_6F0_4 = (float)(2 * hitDirection);
                    float arg_6F0_5 = -2f;
                    int arg_6F0_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_6F0_0, arg_6F0_1, arg_6F0_2, arg_6F0_3, arg_6F0_4, arg_6F0_5, arg_6F0_6, newColor, 1f);
                }
                if (this.Type == 51)
                {
                    SetGore(83);
                    return;
                }
                SetGore(82);
                return;
            }
            else if (this.Type == 46 || this.Type == 55 || this.Type == 67)
            {
                if (this.life > 0)
                {
                    int num11 = 0;
                    while ((double)num11 < dmg / (double)this.lifeMax * 20.0)
                    {
                        Vector2 arg_78F_0 = this.Position;
                        int arg_78F_1 = this.width;
                        int arg_78F_2 = this.height;
                        int arg_78F_3 = 5;
                        float arg_78F_4 = (float)hitDirection;
                        float arg_78F_5 = -1f;
                        int arg_78F_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_78F_0, arg_78F_1, arg_78F_2, arg_78F_3, arg_78F_4, arg_78F_5, arg_78F_6, newColor, 1f);
                        num11++;
                    }
                    return;
                }
                for (int num12 = 0; num12 < 10; num12++)
                {
                    Vector2 arg_7E5_0 = this.Position;
                    int arg_7E5_1 = this.width;
                    int arg_7E5_2 = this.height;
                    int arg_7E5_3 = 5;
                    float arg_7E5_4 = (float)(2 * hitDirection);
                    float arg_7E5_5 = -2f;
                    int arg_7E5_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_7E5_0, arg_7E5_1, arg_7E5_2, arg_7E5_3, arg_7E5_4, arg_7E5_5, arg_7E5_6, newColor, 1f);
                }
                if (this.Type == 46)
                {
                    SetGore(76);
                    Gore.NewGore(new Vector2(this.Position.X, this.Position.Y), this.Velocity, 77);
                    return;
                }
                if (this.Type == 67)
                {
                    SetGore(95);
                    SetGore(95);
                    SetGore(96);
                    return;
                }
            }
            else if (this.Type == 47 || this.Type == 57 || this.Type == 58)
            {
                if (this.life > 0)
                {
                    int num13 = 0;
                    while ((double)num13 < dmg / (double)this.lifeMax * 20.0)
                    {
                        Vector2 arg_8E2_0 = this.Position;
                        int arg_8E2_1 = this.width;
                        int arg_8E2_2 = this.height;
                        int arg_8E2_3 = 5;
                        float arg_8E2_4 = (float)hitDirection;
                        float arg_8E2_5 = -1f;
                        int arg_8E2_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_8E2_0, arg_8E2_1, arg_8E2_2, arg_8E2_3, arg_8E2_4, arg_8E2_5, arg_8E2_6, newColor, 1f);
                        num13++;
                    }
                    return;
                }
                for (int num14 = 0; num14 < 10; num14++)
                {
                    Vector2 arg_938_0 = this.Position;
                    int arg_938_1 = this.width;
                    int arg_938_2 = this.height;
                    int arg_938_3 = 5;
                    float arg_938_4 = (float)(2 * hitDirection);
                    float arg_938_5 = -2f;
                    int arg_938_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_938_0, arg_938_1, arg_938_2, arg_938_3, arg_938_4, arg_938_5, arg_938_6, newColor, 1f);
                }
                if (this.Type == 57)
                {
                    Gore.NewGore(new Vector2(this.Position.X, this.Position.Y), this.Velocity, 84);
                    return;
                }
                if (this.Type == 58)
                {
                    Gore.NewGore(new Vector2(this.Position.X, this.Position.Y), this.Velocity, 85);
                    return;
                }
                SetGore(78);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y), this.Velocity, 79);
                return;
            }
            else if (this.Type == 2)
            {
                if (this.life > 0)
                {
                    int num15 = 0;
                    while ((double)num15 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Vector2 arg_A34_0 = this.Position;
                        int arg_A34_1 = this.width;
                        int arg_A34_2 = this.height;
                        int arg_A34_3 = 5;
                        float arg_A34_4 = (float)hitDirection;
                        float arg_A34_5 = -1f;
                        int arg_A34_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_A34_0, arg_A34_1, arg_A34_2, arg_A34_3, arg_A34_4, arg_A34_5, arg_A34_6, newColor, 1f);
                        num15++;
                    }
                    return;
                }
                for (int num16 = 0; num16 < 50; num16++)
                {
                    Vector2 arg_A8A_0 = this.Position;
                    int arg_A8A_1 = this.width;
                    int arg_A8A_2 = this.height;
                    int arg_A8A_3 = 5;
                    float arg_A8A_4 = (float)(2 * hitDirection);
                    float arg_A8A_5 = -2f;
                    int arg_A8A_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_A8A_0, arg_A8A_1, arg_A8A_2, arg_A8A_3, arg_A8A_4, arg_A8A_5, arg_A8A_6, newColor, 1f);
                }
                SetGore(1);
                Gore.NewGore(new Vector2(this.Position.X + 14f, this.Position.Y), this.Velocity, 2);
                return;
            }
            else if (this.Type == 69)
            {
                if (this.life > 0)
                {
                    int num17 = 0;
                    while ((double)num17 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Vector2 arg_B23_0 = this.Position;
                        int arg_B23_1 = this.width;
                        int arg_B23_2 = this.height;
                        int arg_B23_3 = 5;
                        float arg_B23_4 = (float)hitDirection;
                        float arg_B23_5 = -1f;
                        int arg_B23_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_B23_0, arg_B23_1, arg_B23_2, arg_B23_3, arg_B23_4, arg_B23_5, arg_B23_6, newColor, 1f);
                        num17++;
                    }
                    return;
                }
                for (int num18 = 0; num18 < 50; num18++)
                {
                    Vector2 arg_B79_0 = this.Position;
                    int arg_B79_1 = this.width;
                    int arg_B79_2 = this.height;
                    int arg_B79_3 = 5;
                    float arg_B79_4 = (float)(2 * hitDirection);
                    float arg_B79_5 = -2f;
                    int arg_B79_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_B79_0, arg_B79_1, arg_B79_2, arg_B79_3, arg_B79_4, arg_B79_5, arg_B79_6, newColor, 1f);
                }
                SetGore(97);
                SetGore(98);
                return;
            }
            else if (this.Type == 61)
            {
                if (this.life > 0)
                {
                    int num19 = 0;
                    while ((double)num19 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Vector2 arg_BF9_0 = this.Position;
                        int arg_BF9_1 = this.width;
                        int arg_BF9_2 = this.height;
                        int arg_BF9_3 = 5;
                        float arg_BF9_4 = (float)hitDirection;
                        float arg_BF9_5 = -1f;
                        int arg_BF9_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_BF9_0, arg_BF9_1, arg_BF9_2, arg_BF9_3, arg_BF9_4, arg_BF9_5, arg_BF9_6, newColor, 1f);
                        num19++;
                    }
                    return;
                }
                for (int num20 = 0; num20 < 50; num20++)
                {
                    Vector2 arg_C4F_0 = this.Position;
                    int arg_C4F_1 = this.width;
                    int arg_C4F_2 = this.height;
                    int arg_C4F_3 = 5;
                    float arg_C4F_4 = (float)(2 * hitDirection);
                    float arg_C4F_5 = -2f;
                    int arg_C4F_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_C4F_0, arg_C4F_1, arg_C4F_2, arg_C4F_3, arg_C4F_4, arg_C4F_5, arg_C4F_6, newColor, 1f);
                }
                SetGore(86);
                Gore.NewGore(new Vector2(this.Position.X + 14f, this.Position.Y), this.Velocity, 87);
                Gore.NewGore(new Vector2(this.Position.X + 14f, this.Position.Y), this.Velocity, 88);
                return;
            }
            else if (this.Type == 65)
            {
                if (this.life > 0)
                {
                    int num21 = 0;
                    while ((double)num21 < dmg / (double)this.lifeMax * 150.0)
                    {
                        Vector2 arg_D19_0 = this.Position;
                        int arg_D19_1 = this.width;
                        int arg_D19_2 = this.height;
                        int arg_D19_3 = 5;
                        float arg_D19_4 = (float)hitDirection;
                        float arg_D19_5 = -1f;
                        int arg_D19_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_D19_0, arg_D19_1, arg_D19_2, arg_D19_3, arg_D19_4, arg_D19_5, arg_D19_6, newColor, 1f);
                        num21++;
                    }
                    return;
                }
                for (int num22 = 0; num22 < 75; num22++)
                {
                    Vector2 arg_D6F_0 = this.Position;
                    int arg_D6F_1 = this.width;
                    int arg_D6F_2 = this.height;
                    int arg_D6F_3 = 5;
                    float arg_D6F_4 = (float)(2 * hitDirection);
                    float arg_D6F_5 = -2f;
                    int arg_D6F_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_D6F_0, arg_D6F_1, arg_D6F_2, arg_D6F_3, arg_D6F_4, arg_D6F_5, arg_D6F_6, newColor, 1f);
                }
                Gore.NewGore(this.Position, this.Velocity * 0.8f, 89);
                Gore.NewGore(new Vector2(this.Position.X + 14f, this.Position.Y), this.Velocity * 0.8f, 90);
                Gore.NewGore(new Vector2(this.Position.X + 14f, this.Position.Y), this.Velocity * 0.8f, 91);
                Gore.NewGore(new Vector2(this.Position.X + 14f, this.Position.Y), this.Velocity * 0.8f, 92);
                return;
            }
            else if (this.Type == 3 || this.Type == 52 || this.Type == 53)
            {
                if (this.life > 0)
                {
                    int num23 = 0;
                    while ((double)num23 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Vector2 arg_EA3_0 = this.Position;
                        int arg_EA3_1 = this.width;
                        int arg_EA3_2 = this.height;
                        int arg_EA3_3 = 5;
                        float arg_EA3_4 = (float)hitDirection;
                        float arg_EA3_5 = -1f;
                        int arg_EA3_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_EA3_0, arg_EA3_1, arg_EA3_2, arg_EA3_3, arg_EA3_4, arg_EA3_5, arg_EA3_6, newColor, 1f);
                        num23++;
                    }
                    return;
                }
                for (int num24 = 0; num24 < 50; num24++)
                {
                    Vector2 arg_EFD_0 = this.Position;
                    int arg_EFD_1 = this.width;
                    int arg_EFD_2 = this.height;
                    int arg_EFD_3 = 5;
                    float arg_EFD_4 = 2.5f * (float)hitDirection;
                    float arg_EFD_5 = -2.5f;
                    int arg_EFD_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_EFD_0, arg_EFD_1, arg_EFD_2, arg_EFD_3, arg_EFD_4, arg_EFD_5, arg_EFD_6, newColor, 1f);
                }
                SetGore(3);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 20f), this.Velocity, 4);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 20f), this.Velocity, 4);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 34f), this.Velocity, 5);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 34f), this.Velocity, 5);
                return;
            }
            else if (this.Type == 4)
            {
                if (this.life > 0)
                {
                    int num25 = 0;
                    while ((double)num25 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Vector2 arg_101F_0 = this.Position;
                        int arg_101F_1 = this.width;
                        int arg_101F_2 = this.height;
                        int arg_101F_3 = 5;
                        float arg_101F_4 = (float)hitDirection;
                        float arg_101F_5 = -1f;
                        int arg_101F_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_101F_0, arg_101F_1, arg_101F_2, arg_101F_3, arg_101F_4, arg_101F_5, arg_101F_6, newColor, 1f);
                        num25++;
                    }
                    return;
                }
                for (int num26 = 0; num26 < 150; num26++)
                {
                    Vector2 arg_1075_0 = this.Position;
                    int arg_1075_1 = this.width;
                    int arg_1075_2 = this.height;
                    int arg_1075_3 = 5;
                    float arg_1075_4 = (float)(2 * hitDirection);
                    float arg_1075_5 = -2f;
                    int arg_1075_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_1075_0, arg_1075_1, arg_1075_2, arg_1075_3, arg_1075_4, arg_1075_5, arg_1075_6, newColor, 1f);
                }
                for (int num27 = 0; num27 < 2; num27++)
                {
                    Gore.NewGore(this.Position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 2);
                    Gore.NewGore(this.Position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7);
                    Gore.NewGore(this.Position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 9);
                    Gore.NewGore(this.Position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 10);
                }
                return;
            }
            else if (this.Type == 5)
            {
                if (this.life > 0)
                {
                    int num28 = 0;
                    while ((double)num28 < dmg / (double)this.lifeMax * 50.0)
                    {
                        Vector2 arg_11F7_0 = this.Position;
                        int arg_11F7_1 = this.width;
                        int arg_11F7_2 = this.height;
                        int arg_11F7_3 = 5;
                        float arg_11F7_4 = (float)hitDirection;
                        float arg_11F7_5 = -1f;
                        int arg_11F7_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_11F7_0, arg_11F7_1, arg_11F7_2, arg_11F7_3, arg_11F7_4, arg_11F7_5, arg_11F7_6, newColor, 1f);
                        num28++;
                    }
                    return;
                }
                for (int num29 = 0; num29 < 20; num29++)
                {
                    Vector2 arg_124D_0 = this.Position;
                    int arg_124D_1 = this.width;
                    int arg_124D_2 = this.height;
                    int arg_124D_3 = 5;
                    float arg_124D_4 = (float)(2 * hitDirection);
                    float arg_124D_5 = -2f;
                    int arg_124D_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_124D_0, arg_124D_1, arg_124D_2, arg_124D_3, arg_124D_4, arg_124D_5, arg_124D_6, newColor, 1f);
                }
                SetGore(6);
                SetGore(7);
                return;
            }
            else if (this.Type == 6)
            {
                if (this.life > 0)
                {
                    int num30 = 0;
                    while ((double)num30 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Dust.NewDust(this.Position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
                        num30++;
                    }
                    return;
                }
                for (int num31 = 0; num31 < 50; num31++)
                {
                    Dust.NewDust(this.Position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
                }
                int num32 = SetGore(14);
                Main.gore[num32].alpha = this.alpha;
                num32 = SetGore(15);
                Main.gore[num32].alpha = this.alpha;
                return;
            }
            else if (this.Type == 7 || this.Type == 8 || this.Type == 9)
            {
                if (this.life > 0)
                {
                    int num33 = 0;
                    while ((double)num33 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Dust.NewDust(this.Position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
                        num33++;
                    }
                    return;
                }
                for (int num34 = 0; num34 < 50; num34++)
                {
                    Dust.NewDust(this.Position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
                }
                int num35 = SetGore(this.Type - 7 + 18);
                Main.gore[num35].alpha = this.alpha;
                return;
            }
            else if (this.Type == 10 || this.Type == 11 || this.Type == 12)
            {
                if (this.life > 0)
                {
                    int num36 = 0;
                    while ((double)num36 < dmg / (double)this.lifeMax * 50.0)
                    {
                        Vector2 arg_14D5_0 = this.Position;
                        int arg_14D5_1 = this.width;
                        int arg_14D5_2 = this.height;
                        int arg_14D5_3 = 5;
                        float arg_14D5_4 = (float)hitDirection;
                        float arg_14D5_5 = -1f;
                        int arg_14D5_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_14D5_0, arg_14D5_1, arg_14D5_2, arg_14D5_3, arg_14D5_4, arg_14D5_5, arg_14D5_6, newColor, 1f);
                        num36++;
                    }
                    return;
                }
                for (int num37 = 0; num37 < 10; num37++)
                {
                    Vector2 arg_152F_0 = this.Position;
                    int arg_152F_1 = this.width;
                    int arg_152F_2 = this.height;
                    int arg_152F_3 = 5;
                    float arg_152F_4 = 2.5f * (float)hitDirection;
                    float arg_152F_5 = -2.5f;
                    int arg_152F_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_152F_0, arg_152F_1, arg_152F_2, arg_152F_3, arg_152F_4, arg_152F_5, arg_152F_6, newColor, 1f);
                }
                SetGore(this.Type - 7 + 18);
                return;
            }
            else if (this.Type == 13 || this.Type == 14 || this.Type == 15)
            {
                if (this.life > 0)
                {
                    int num38 = 0;
                    while ((double)num38 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Dust.NewDust(this.Position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
                        num38++;
                    }
                    return;
                }
                for (int num39 = 0; num39 < 50; num39++)
                {
                    Dust.NewDust(this.Position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
                }
                if (this.Type == 13)
                {
                    SetGore(24);
                    SetGore(25);
                    return;
                }
                if (this.Type == 14)
                {
                    SetGore(26);
                    SetGore(27);
                    return;
                }
                SetGore(28);
                SetGore(29);
                return;
            }
            else if (this.Type == 17)
            {
                if (this.life > 0)
                {
                    int num40 = 0;
                    while ((double)num40 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Vector2 arg_16F8_0 = this.Position;
                        int arg_16F8_1 = this.width;
                        int arg_16F8_2 = this.height;
                        int arg_16F8_3 = 5;
                        float arg_16F8_4 = (float)hitDirection;
                        float arg_16F8_5 = -1f;
                        int arg_16F8_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_16F8_0, arg_16F8_1, arg_16F8_2, arg_16F8_3, arg_16F8_4, arg_16F8_5, arg_16F8_6, newColor, 1f);
                        num40++;
                    }
                    return;
                }
                for (int num41 = 0; num41 < 50; num41++)
                {
                    Vector2 arg_1752_0 = this.Position;
                    int arg_1752_1 = this.width;
                    int arg_1752_2 = this.height;
                    int arg_1752_3 = 5;
                    float arg_1752_4 = 2.5f * (float)hitDirection;
                    float arg_1752_5 = -2.5f;
                    int arg_1752_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_1752_0, arg_1752_1, arg_1752_2, arg_1752_3, arg_1752_4, arg_1752_5, arg_1752_6, newColor, 1f);
                }
                SetGore(30);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 20f), this.Velocity, 31);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 20f), this.Velocity, 31);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 34f), this.Velocity, 32);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 34f), this.Velocity, 32);
                return;
            }
            else if (this.Type == 22)
            {
                ShowDamage(dmg, hitDirection, 73);
                return;
            }
            else if (this.Type == 37 || this.Type == 54)
            {
                ShowDamage(dmg, hitDirection, 58);
                return;
            }
            else if (this.Type == 18)
            {
                ShowDamage(dmg, hitDirection, 33);
                return;
            }
            else if (this.Type == 19)
            {
                ShowDamage(dmg, hitDirection, 36);
                return;
            }
            else if (this.Type == 38)
            {
                ShowDamage(dmg, hitDirection, 64);
                return;
            }
            else if (this.Type == 20)
            {
                ShowDamage(dmg, hitDirection, 39);
                return;
            }
            else if (this.Type == 21 || this.Type == 31 || this.Type == 32 || this.Type == 44 || this.Type == 45)
            {
                ShowDamage(dmg, hitDirection, 42, 50d, 26);
                return;
            }
            else if (this.Type == 39 || this.Type == 40 || this.Type == 41)
            {
                if (this.life > 0)
                {
                    int num56 = 0;
                    while ((double)num56 < dmg / (double)this.lifeMax * 50.0)
                    {
                        Vector2 arg_2351_0 = this.Position;
                        int arg_2351_1 = this.width;
                        int arg_2351_2 = this.height;
                        int arg_2351_3 = 26;
                        float arg_2351_4 = (float)hitDirection;
                        float arg_2351_5 = -1f;
                        int arg_2351_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_2351_0, arg_2351_1, arg_2351_2, arg_2351_3, arg_2351_4, arg_2351_5, arg_2351_6, newColor, 1f);
                        num56++;
                    }
                    return;
                }
                for (int num57 = 0; num57 < 20; num57++)
                {
                    Vector2 arg_23AC_0 = this.Position;
                    int arg_23AC_1 = this.width;
                    int arg_23AC_2 = this.height;
                    int arg_23AC_3 = 26;
                    float arg_23AC_4 = 2.5f * (float)hitDirection;
                    float arg_23AC_5 = -2.5f;
                    int arg_23AC_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_23AC_0, arg_23AC_1, arg_23AC_2, arg_23AC_3, arg_23AC_4, arg_23AC_5, arg_23AC_6, newColor, 1f);
                }
                SetGore(this.Type - 39 + 67);
                return;
            }
            else if (this.Type == 34)
            {
                if (this.life > 0)
                {
                    int num58 = 0;
                    while ((double)num58 < dmg / (double)this.lifeMax * 30.0)
                    {
                        Vector2 arg_245C_0 = new Vector2(this.Position.X, this.Position.Y);
                        int arg_245C_1 = this.width;
                        int arg_245C_2 = this.height;
                        int arg_245C_3 = 15;
                        float arg_245C_4 = -this.Velocity.X * 0.2f;
                        float arg_245C_5 = -this.Velocity.Y * 0.2f;
                        int arg_245C_6 = 100;
                        Color newColor = default(Color);
                        int num59 = Dust.NewDust(arg_245C_0, arg_245C_1, arg_245C_2, arg_245C_3, arg_245C_4, arg_245C_5, arg_245C_6, newColor, 1.8f);
                        Main.dust[num59].noLight = true;
                        Main.dust[num59].noGravity = true;
                        Dust expr_2487 = Main.dust[num59];
                        expr_2487.velocity *= 1.3f;
                        Vector2 arg_24F9_0 = new Vector2(this.Position.X, this.Position.Y);
                        int arg_24F9_1 = this.width;
                        int arg_24F9_2 = this.height;
                        int arg_24F9_3 = 26;
                        float arg_24F9_4 = -this.Velocity.X * 0.2f;
                        float arg_24F9_5 = -this.Velocity.Y * 0.2f;
                        int arg_24F9_6 = 0;
                        newColor = default(Color);
                        num59 = Dust.NewDust(arg_24F9_0, arg_24F9_1, arg_24F9_2, arg_24F9_3, arg_24F9_4, arg_24F9_5, arg_24F9_6, newColor, 0.9f);
                        Main.dust[num59].noLight = true;
                        Dust expr_2516 = Main.dust[num59];
                        expr_2516.velocity *= 1.3f;
                        num58++;
                    }
                    return;
                }
                for (int num60 = 0; num60 < 15; num60++)
                {
                    Vector2 arg_25B3_0 = new Vector2(this.Position.X, this.Position.Y);
                    int arg_25B3_1 = this.width;
                    int arg_25B3_2 = this.height;
                    int arg_25B3_3 = 15;
                    float arg_25B3_4 = -this.Velocity.X * 0.2f;
                    float arg_25B3_5 = -this.Velocity.Y * 0.2f;
                    int arg_25B3_6 = 100;
                    Color newColor = default(Color);
                    int num61 = Dust.NewDust(arg_25B3_0, arg_25B3_1, arg_25B3_2, arg_25B3_3, arg_25B3_4, arg_25B3_5, arg_25B3_6, newColor, 1.8f);
                    Main.dust[num61].noLight = true;
                    Main.dust[num61].noGravity = true;
                    Dust expr_25DE = Main.dust[num61];
                    expr_25DE.velocity *= 1.3f;
                    Vector2 arg_2650_0 = new Vector2(this.Position.X, this.Position.Y);
                    int arg_2650_1 = this.width;
                    int arg_2650_2 = this.height;
                    int arg_2650_3 = 26;
                    float arg_2650_4 = -this.Velocity.X * 0.2f;
                    float arg_2650_5 = -this.Velocity.Y * 0.2f;
                    int arg_2650_6 = 0;
                    newColor = default(Color);
                    num61 = Dust.NewDust(arg_2650_0, arg_2650_1, arg_2650_2, arg_2650_3, arg_2650_4, arg_2650_5, arg_2650_6, newColor, 0.9f);
                    Main.dust[num61].noLight = true;
                    Dust expr_266D = Main.dust[num61];
                    expr_266D.velocity *= 1.3f;
                }
                return;
            }
            else if (this.Type == 35 || this.Type == 36)
            {
                if (this.life > 0)
                {
                    int num62 = 0;
                    while ((double)num62 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Vector2 arg_26E2_0 = this.Position;
                        int arg_26E2_1 = this.width;
                        int arg_26E2_2 = this.height;
                        int arg_26E2_3 = 26;
                        float arg_26E2_4 = (float)hitDirection;
                        float arg_26E2_5 = -1f;
                        int arg_26E2_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_26E2_0, arg_26E2_1, arg_26E2_2, arg_26E2_3, arg_26E2_4, arg_26E2_5, arg_26E2_6, newColor, 1f);
                        num62++;
                    }
                    return;
                }
                for (int num63 = 0; num63 < 150; num63++)
                {
                    Vector2 arg_273D_0 = this.Position;
                    int arg_273D_1 = this.width;
                    int arg_273D_2 = this.height;
                    int arg_273D_3 = 26;
                    float arg_273D_4 = 2.5f * (float)hitDirection;
                    float arg_273D_5 = -2.5f;
                    int arg_273D_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_273D_0, arg_273D_1, arg_273D_2, arg_273D_3, arg_273D_4, arg_273D_5, arg_273D_6, newColor, 1f);
                }
                if (this.Type == 35)
                {
                    SetGore(54);
                    SetGore(55);
                    return;
                }
                SetGore(56);
                SetGore(57);
                SetGore(57);
                SetGore(57);
                return;
            }
            else if (this.Type == 23)
            {
                if (this.life > 0)
                {
                    int num64 = 0;
                    while ((double)num64 < dmg / (double)this.lifeMax * 100.0)
                    {
                        int num65 = 25;
                        if (Main.rand.Next(2) == 0)
                        {
                            num65 = 6;
                        }
                        Vector2 arg_2836_0 = this.Position;
                        int arg_2836_1 = this.width;
                        int arg_2836_2 = this.height;
                        int arg_2836_3 = num65;
                        float arg_2836_4 = (float)hitDirection;
                        float arg_2836_5 = -1f;
                        int arg_2836_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_2836_0, arg_2836_1, arg_2836_2, arg_2836_3, arg_2836_4, arg_2836_5, arg_2836_6, newColor, 1f);
                        Vector2 arg_2897_0 = new Vector2(this.Position.X, this.Position.Y);
                        int arg_2897_1 = this.width;
                        int arg_2897_2 = this.height;
                        int arg_2897_3 = 6;
                        float arg_2897_4 = this.Velocity.X * 0.2f;
                        float arg_2897_5 = this.Velocity.Y * 0.2f;
                        int arg_2897_6 = 100;
                        newColor = default(Color);
                        int num66 = Dust.NewDust(arg_2897_0, arg_2897_1, arg_2897_2, arg_2897_3, arg_2897_4, arg_2897_5, arg_2897_6, newColor, 2f);
                        Main.dust[num66].noGravity = true;
                        num64++;
                    }
                    return;
                }
                for (int num67 = 0; num67 < 50; num67++)
                {
                    int num68 = 25;
                    if (Main.rand.Next(2) == 0)
                    {
                        num68 = 6;
                    }
                    Vector2 arg_2914_0 = this.Position;
                    int arg_2914_1 = this.width;
                    int arg_2914_2 = this.height;
                    int arg_2914_3 = num68;
                    float arg_2914_4 = (float)(2 * hitDirection);
                    float arg_2914_5 = -2f;
                    int arg_2914_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_2914_0, arg_2914_1, arg_2914_2, arg_2914_3, arg_2914_4, arg_2914_5, arg_2914_6, newColor, 1f);
                }
                for (int num69 = 0; num69 < 50; num69++)
                {
                    Vector2 arg_2989_0 = new Vector2(this.Position.X, this.Position.Y);
                    int arg_2989_1 = this.width;
                    int arg_2989_2 = this.height;
                    int arg_2989_3 = 6;
                    float arg_2989_4 = this.Velocity.X * 0.2f;
                    float arg_2989_5 = this.Velocity.Y * 0.2f;
                    int arg_2989_6 = 100;
                    Color newColor = default(Color);
                    int num70 = Dust.NewDust(arg_2989_0, arg_2989_1, arg_2989_2, arg_2989_3, arg_2989_4, arg_2989_5, arg_2989_6, newColor, 2.5f);
                    Dust expr_2998 = Main.dust[num70];
                    expr_2998.velocity *= 6f;
                    Main.dust[num70].noGravity = true;
                }
                return;
            }
            else if (this.Type == 24)
            {
                if (this.life > 0)
                {
                    int num71 = 0;
                    while ((double)num71 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Vector2 arg_2A38_0 = new Vector2(this.Position.X, this.Position.Y);
                        int arg_2A38_1 = this.width;
                        int arg_2A38_2 = this.height;
                        int arg_2A38_3 = 6;
                        float arg_2A38_4 = this.Velocity.X;
                        float arg_2A38_5 = this.Velocity.Y;
                        int arg_2A38_6 = 100;
                        Color newColor = default(Color);
                        int num72 = Dust.NewDust(arg_2A38_0, arg_2A38_1, arg_2A38_2, arg_2A38_3, arg_2A38_4, arg_2A38_5, arg_2A38_6, newColor, 2.5f);
                        Main.dust[num72].noGravity = true;
                        num71++;
                    }
                    return;
                }
                for (int num73 = 0; num73 < 50; num73++)
                {
                    Vector2 arg_2AC6_0 = new Vector2(this.Position.X, this.Position.Y);
                    int arg_2AC6_1 = this.width;
                    int arg_2AC6_2 = this.height;
                    int arg_2AC6_3 = 6;
                    float arg_2AC6_4 = this.Velocity.X;
                    float arg_2AC6_5 = this.Velocity.Y;
                    int arg_2AC6_6 = 100;
                    Color newColor = default(Color);
                    int num74 = Dust.NewDust(arg_2AC6_0, arg_2AC6_1, arg_2AC6_2, arg_2AC6_3, arg_2AC6_4, arg_2AC6_5, arg_2AC6_6, newColor, 2.5f);
                    Main.dust[num74].noGravity = true;
                    Dust expr_2AE3 = Main.dust[num74];
                    expr_2AE3.velocity *= 2f;
                }
                SetGore(45);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 20f), this.Velocity, 46);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 20f), this.Velocity, 46);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 34f), this.Velocity, 47);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 34f), this.Velocity, 47);
                return;
            }
            else if (this.Type == 25)
            {
                for (int num75 = 0; num75 < 20; num75++)
                {
                    Vector2 arg_2C6A_0 = new Vector2(this.Position.X, this.Position.Y);
                    int arg_2C6A_1 = this.width;
                    int arg_2C6A_2 = this.height;
                    int arg_2C6A_3 = 6;
                    float arg_2C6A_4 = -this.Velocity.X * 0.2f;
                    float arg_2C6A_5 = -this.Velocity.Y * 0.2f;
                    int arg_2C6A_6 = 100;
                    Color newColor = default(Color);
                    int num76 = Dust.NewDust(arg_2C6A_0, arg_2C6A_1, arg_2C6A_2, arg_2C6A_3, arg_2C6A_4, arg_2C6A_5, arg_2C6A_6, newColor, 2f);
                    Main.dust[num76].noGravity = true;
                    Dust expr_2C87 = Main.dust[num76];
                    expr_2C87.velocity *= 2f;
                    Vector2 arg_2CF9_0 = new Vector2(this.Position.X, this.Position.Y);
                    int arg_2CF9_1 = this.width;
                    int arg_2CF9_2 = this.height;
                    int arg_2CF9_3 = 6;
                    float arg_2CF9_4 = -this.Velocity.X * 0.2f;
                    float arg_2CF9_5 = -this.Velocity.Y * 0.2f;
                    int arg_2CF9_6 = 100;
                    newColor = default(Color);
                    num76 = Dust.NewDust(arg_2CF9_0, arg_2CF9_1, arg_2CF9_2, arg_2CF9_3, arg_2CF9_4, arg_2CF9_5, arg_2CF9_6, newColor, 1f);
                    Dust expr_2D08 = Main.dust[num76];
                    expr_2D08.velocity *= 2f;
                }
                return;
            }
            else if (this.Type == 33)
            {
                for (int num77 = 0; num77 < 20; num77++)
                {
                    Vector2 arg_2DC0_0 = new Vector2(this.Position.X, this.Position.Y);
                    int arg_2DC0_1 = this.width;
                    int arg_2DC0_2 = this.height;
                    int arg_2DC0_3 = 29;
                    float arg_2DC0_4 = -this.Velocity.X * 0.2f;
                    float arg_2DC0_5 = -this.Velocity.Y * 0.2f;
                    int arg_2DC0_6 = 100;
                    Color newColor = default(Color);
                    int num78 = Dust.NewDust(arg_2DC0_0, arg_2DC0_1, arg_2DC0_2, arg_2DC0_3, arg_2DC0_4, arg_2DC0_5, arg_2DC0_6, newColor, 2f);
                    Main.dust[num78].noGravity = true;
                    Dust expr_2DDD = Main.dust[num78];
                    expr_2DDD.velocity *= 2f;
                    Vector2 arg_2E50_0 = new Vector2(this.Position.X, this.Position.Y);
                    int arg_2E50_1 = this.width;
                    int arg_2E50_2 = this.height;
                    int arg_2E50_3 = 29;
                    float arg_2E50_4 = -this.Velocity.X * 0.2f;
                    float arg_2E50_5 = -this.Velocity.Y * 0.2f;
                    int arg_2E50_6 = 100;
                    newColor = default(Color);
                    num78 = Dust.NewDust(arg_2E50_0, arg_2E50_1, arg_2E50_2, arg_2E50_3, arg_2E50_4, arg_2E50_5, arg_2E50_6, newColor, 1f);
                    Dust expr_2E5F = Main.dust[num78];
                    expr_2E5F.velocity *= 2f;
                }
                return;
            }
            else if (this.Type == 26 || this.Type == 27 || this.Type == 28 || this.Type == 29)
            {
                if (this.life > 0)
                {
                    int num79 = 0;
                    while ((double)num79 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Vector2 arg_2EE7_0 = this.Position;
                        int arg_2EE7_1 = this.width;
                        int arg_2EE7_2 = this.height;
                        int arg_2EE7_3 = 5;
                        float arg_2EE7_4 = (float)hitDirection;
                        float arg_2EE7_5 = -1f;
                        int arg_2EE7_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_2EE7_0, arg_2EE7_1, arg_2EE7_2, arg_2EE7_3, arg_2EE7_4, arg_2EE7_5, arg_2EE7_6, newColor, 1f);
                        num79++;
                    }
                    return;
                }
                for (int num80 = 0; num80 < 50; num80++)
                {
                    Vector2 arg_2F41_0 = this.Position;
                    int arg_2F41_1 = this.width;
                    int arg_2F41_2 = this.height;
                    int arg_2F41_3 = 5;
                    float arg_2F41_4 = 2.5f * (float)hitDirection;
                    float arg_2F41_5 = -2.5f;
                    int arg_2F41_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_2F41_0, arg_2F41_1, arg_2F41_2, arg_2F41_3, arg_2F41_4, arg_2F41_5, arg_2F41_6, newColor, 1f);
                }
                SetGore(48);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 20f), this.Velocity, 49);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 20f), this.Velocity, 49);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 34f), this.Velocity, 50);
                Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 34f), this.Velocity, 50);
                return;
            }
            else if (this.Type == 30)
            {
                for (int num81 = 0; num81 < 20; num81++)
                {
                    Vector2 arg_30B7_0 = new Vector2(this.Position.X, this.Position.Y);
                    int arg_30B7_1 = this.width;
                    int arg_30B7_2 = this.height;
                    int arg_30B7_3 = 27;
                    float arg_30B7_4 = -this.Velocity.X * 0.2f;
                    float arg_30B7_5 = -this.Velocity.Y * 0.2f;
                    int arg_30B7_6 = 100;
                    Color newColor = default(Color);
                    int num82 = Dust.NewDust(arg_30B7_0, arg_30B7_1, arg_30B7_2, arg_30B7_3, arg_30B7_4, arg_30B7_5, arg_30B7_6, newColor, 2f);
                    Main.dust[num82].noGravity = true;
                    Dust expr_30D4 = Main.dust[num82];
                    expr_30D4.velocity *= 2f;
                    Vector2 arg_3147_0 = new Vector2(this.Position.X, this.Position.Y);
                    int arg_3147_1 = this.width;
                    int arg_3147_2 = this.height;
                    int arg_3147_3 = 27;
                    float arg_3147_4 = -this.Velocity.X * 0.2f;
                    float arg_3147_5 = -this.Velocity.Y * 0.2f;
                    int arg_3147_6 = 100;
                    newColor = default(Color);
                    num82 = Dust.NewDust(arg_3147_0, arg_3147_1, arg_3147_2, arg_3147_3, arg_3147_4, arg_3147_5, arg_3147_6, newColor, 1f);
                    Dust expr_3156 = Main.dust[num82];
                    expr_3156.velocity *= 2f;
                }
                return;
            }
            else if (this.Type == 42)
            {
                if (this.life > 0)
                {
                    int num83 = 0;
                    while ((double)num83 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Dust.NewDust(this.Position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
                        num83++;
                    }
                    return;
                }
                for (int num84 = 0; num84 < 50; num84++)
                {
                    Dust.NewDust(this.Position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
                }
                SetGore(70);
                SetGore(71);
                return;
            }
            else if (this.Type == 43 || this.Type == 56)
            {
                if (this.life > 0)
                {
                    int num85 = 0;
                    while ((double)num85 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Dust.NewDust(this.Position, this.width, this.height, 40, (float)hitDirection, -1f, this.alpha, this.color, 1.2f);
                        num85++;
                    }
                    return;
                }
                for (int num86 = 0; num86 < 50; num86++)
                {
                    Dust.NewDust(this.Position, this.width, this.height, 40, (float)hitDirection, -2f, this.alpha, this.color, 1.2f);
                }
                SetGore(72);
                SetGore(72);
                return;
            }
            else if (this.Type == 48)
            {
                if (this.life > 0)
                {
                    int num87 = 0;
                    while ((double)num87 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Vector2 arg_337C_0 = this.Position;
                        int arg_337C_1 = this.width;
                        int arg_337C_2 = this.height;
                        int arg_337C_3 = 5;
                        float arg_337C_4 = (float)hitDirection;
                        float arg_337C_5 = -1f;
                        int arg_337C_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_337C_0, arg_337C_1, arg_337C_2, arg_337C_3, arg_337C_4, arg_337C_5, arg_337C_6, newColor, 1f);
                        num87++;
                    }
                    return;
                }
                for (int num88 = 0; num88 < 50; num88++)
                {
                    Vector2 arg_33D2_0 = this.Position;
                    int arg_33D2_1 = this.width;
                    int arg_33D2_2 = this.height;
                    int arg_33D2_3 = 5;
                    float arg_33D2_4 = (float)(2 * hitDirection);
                    float arg_33D2_5 = -2f;
                    int arg_33D2_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_33D2_0, arg_33D2_1, arg_33D2_2, arg_33D2_3, arg_33D2_4, arg_33D2_5, arg_33D2_6, newColor, 1f);
                }
                SetGore(80);
                SetGore(81);
                return;
            }
            else if (this.Type == 62 || this.Type == 66)
            {
                if (this.life > 0)
                {
                    int num89 = 0;
                    while ((double)num89 < dmg / (double)this.lifeMax * 100.0)
                    {
                        Vector2 arg_345C_0 = this.Position;
                        int arg_345C_1 = this.width;
                        int arg_345C_2 = this.height;
                        int arg_345C_3 = 5;
                        float arg_345C_4 = (float)hitDirection;
                        float arg_345C_5 = -1f;
                        int arg_345C_6 = 0;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_345C_0, arg_345C_1, arg_345C_2, arg_345C_3, arg_345C_4, arg_345C_5, arg_345C_6, newColor, 1f);
                        num89++;
                    }
                    return;
                }
                for (int num90 = 0; num90 < 50; num90++)
                {
                    Vector2 arg_34B2_0 = this.Position;
                    int arg_34B2_1 = this.width;
                    int arg_34B2_2 = this.height;
                    int arg_34B2_3 = 5;
                    float arg_34B2_4 = (float)(2 * hitDirection);
                    float arg_34B2_5 = -2f;
                    int arg_34B2_6 = 0;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_34B2_0, arg_34B2_1, arg_34B2_2, arg_34B2_3, arg_34B2_4, arg_34B2_5, arg_34B2_6, newColor, 1f);
                }
                SetGore(93);
                SetGore(94);
                SetGore(94);
            }

            Color newColor2 = new Color(50, 120, 255, 100);
            if (this.Type == 64)
            {
                newColor2 = new Color(225, 70, 140, 100);
            }
            if (this.life > 0)
            {
                int num91 = 0;
                while ((double)num91 < dmg / (double)this.lifeMax * 50.0)
                {
                    Dust.NewDust(this.Position, this.width, this.height, 4, (float)hitDirection, -1f, 0, newColor2, 1f);
                    num91++;
                }
                return;
            }
            for (int num92 = 0; num92 < 25; num92++)
            {
                Dust.NewDust(this.Position, this.width, this.height, 4, (float)(2 * hitDirection), -2f, 0, newColor2, 1f);
            }
        }
        
        public static bool AnyNPCs(int Type)
        {
            for (int i = 0; i < MAX_NPCS; i++)
            {
                if (Main.npcs[i].Active && Main.npcs[i].Type == Type)
                {
                    return true;
                }
            }
            return false;
        }
        
        public static void SpawnSkeletron()
        {
            bool flag = true;
            bool flag2 = false;
            Vector2 vector = default(Vector2);
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < MAX_NPCS; i++)
            {
                if (Main.npcs[i].Active && Main.npcs[i].Type == 35)
                {
                    flag = false;
                    break;
                }
            }
            for (int j = 0; j < MAX_NPCS; j++)
            {
                if (Main.npcs[j].Active && Main.npcs[j].Type == 37)
                {
                    flag2 = true;
                    Main.npcs[j].ai[3] = 1f;
                    vector = Main.npcs[j].Position;
                    num = Main.npcs[j].width;
                    num2 = Main.npcs[j].height;

                    NetMessage.SendData(23, -1, -1, "", j);
                }
            }
            if (flag && flag2)
            {
                int npcIndex = NPC.NewNPC((int)vector.X + num / 2, (int)vector.Y + num2 / 2, 35, 0);
                Main.npcs[npcIndex].netUpdate = true;
                String str = "Skeletron";
                
                NetMessage.SendData(25, -1, -1, str + " has awoken!", 255, 175f, 75f, 255f);
            }
        }
        
        public static void UpdateNPC(int i)
        {
            NPC npc = Main.npcs[i];
            npc.whoAmI = i;
            if (npc.Active)
            {
                if (Main.bloodMoon)
                {
                    if (npc.Type == 46)
                    {
                        Transform(i, 47);
                        npc = Main.npcs[i];
                    }
                    else if (npc.Type == 55)
                    {
                        Transform(i, 57);
                        npc = Main.npcs[i];
                    }
                }
                float num = 10f;
                float num2 = 0.3f;
                if (npc.wet)
                {
                    num2 = 0.2f;
                    num = 7f;
                }
                if (npc.soundDelay > 0)
                {
                    npc.soundDelay--;
                }
                if (npc.life <= 0)
                {
                    npc.Active = false;
                }
                npc.oldTarget = npc.target;
                npc.oldDirection = npc.direction;
                npc.oldDirectionY = npc.directionY;
                NPC.AI(i);

                for (int j = 0; j < 256; j++)
                {
                    if (npc.immune[j] > 0)
                    {
                        npc.immune[j]--;
                    }
                }
                if (!npc.noGravity)
                {
                    npc.Velocity.Y = npc.Velocity.Y + num2;
                    if (npc.Velocity.Y > num)
                    {
                        npc.Velocity.Y = num;
                    }
                }
                if ((double)npc.Velocity.X < 0.005 && (double)npc.Velocity.X > -0.005)
                {
                    npc.Velocity.X = 0f;
                }
                if (npc.friendly && npc.Type != 37)
                {
                    if (npc.life < npc.lifeMax)
                    {
                        npc.friendlyRegen++;
                        if (npc.friendlyRegen > 300)
                        {
                            npc.friendlyRegen = 0;
                            npc.life++;
                            npc.netUpdate = true;
                        }
                    }
                    if (npc.immune[255] == 0)
                    {
                        Rectangle rectangle = new Rectangle((int)npc.Position.X, (int)npc.Position.Y, npc.width, npc.height);
                        for (int k = 0; k < MAX_NPCS; k++)
                        {
                            if (Main.npcs[k].Active && !Main.npcs[k].friendly && Main.npcs[k].damage > 0)
                            {
                                Rectangle rectangle2 = new Rectangle((int)Main.npcs[k].Position.X, (int)Main.npcs[k].Position.Y, Main.npcs[k].width, Main.npcs[k].height);
                                if (rectangle.Intersects(rectangle2))
                                {
                                    int num3 = Main.npcs[k].damage;
                                    int num4 = 6;
                                    int num5 = 1;
                                    if (Main.npcs[k].Position.X + (float)(Main.npcs[k].width / 2) > npc.Position.X + (float)(npc.width / 2))
                                    {
                                        num5 = -1;
                                    }
                                    Main.npcs[i].StrikeNPC(num3, (float)num4, num5);
                                    NetMessage.SendData(28, -1, -1, "", i, (float)num3, (float)num4, (float)num5);
                                    npc.netUpdate = true;
                                    npc.immune[255] = 30;
                                }
                            }
                        }
                    }
                }
                if (!npc.noTileCollide)
                {
                    bool flag = Collision.LavaCollision(npc.Position, npc.width, npc.height);
                    if (flag)
                    {
                        npc.lavaWet = true;
                        if (!npc.lavaImmune && npc.immune[255] == 0)
                        {
                            npc.immune[255] = 30;
                            npc.StrikeNPC(50, 0f, 0);
                            
                            NetMessage.SendData(28, -1, -1, "", npc.whoAmI, 50f);
                        }
                    }
                    bool flag2 = Collision.WetCollision(npc.Position, npc.width, npc.height);
                    if (flag2)
                    {
                        if (!npc.wet && npc.wetCount == 0)
                        {
                            npc.wetCount = 10;
                            if (!flag)
                            {
                                for (int l = 0; l < 30; l++)
                                {
                                    int num6 = Dust.NewDust(new Vector2(npc.Position.X - 6f, npc.Position.Y + (float)(npc.height / 2) - 8f), npc.width + 12, 24, 33, 0f, 0f, 0, default(Color), 1f);
                                    Dust expr_4DC_cp_0 = Main.dust[num6];
                                    expr_4DC_cp_0.velocity.Y = expr_4DC_cp_0.velocity.Y - 4f;
                                    Dust expr_4FA_cp_0 = Main.dust[num6];
                                    expr_4FA_cp_0.velocity.X = expr_4FA_cp_0.velocity.X * 2.5f;
                                    Main.dust[num6].scale = 1.3f;
                                    Main.dust[num6].alpha = 100;
                                    Main.dust[num6].noGravity = true;
                                }

                            }
                            else
                            {
                                for (int m = 0; m < 10; m++)
                                {
                                    int num7 = Dust.NewDust(new Vector2(npc.Position.X - 6f, npc.Position.Y + (float)(npc.height / 2) - 8f), npc.width + 12, 24, 35, 0f, 0f, 0, default(Color), 1f);
                                    Dust expr_606_cp_0 = Main.dust[num7];
                                    expr_606_cp_0.velocity.Y = expr_606_cp_0.velocity.Y - 1.5f;
                                    Dust expr_624_cp_0 = Main.dust[num7];
                                    expr_624_cp_0.velocity.X = expr_624_cp_0.velocity.X * 2.5f;
                                    Main.dust[num7].scale = 1.3f;
                                    Main.dust[num7].alpha = 100;
                                    Main.dust[num7].noGravity = true;
                                }
                            }
                        }
                        npc.wet = true;
                    }
                    else
                    {
                        if (npc.wet)
                        {
                            npc.Velocity.X = npc.Velocity.X * 0.5f;
                            npc.wet = false;
                            if (npc.wetCount == 0)
                            {
                                npc.wetCount = 10;
                                if (!npc.lavaWet)
                                {
                                    for (int n = 0; n < 30; n++)
                                    {
                                        int num8 = Dust.NewDust(new Vector2(npc.Position.X - 6f, npc.Position.Y + (float)(npc.height / 2) - 8f), npc.width + 12, 24, 33, 0f, 0f, 0, default(Color), 1f);
                                        Dust expr_775_cp_0 = Main.dust[num8];
                                        expr_775_cp_0.velocity.Y = expr_775_cp_0.velocity.Y - 4f;
                                        Dust expr_793_cp_0 = Main.dust[num8];
                                        expr_793_cp_0.velocity.X = expr_793_cp_0.velocity.X * 2.5f;
                                        Main.dust[num8].scale = 1.3f;
                                        Main.dust[num8].alpha = 100;
                                        Main.dust[num8].noGravity = true;
                                    }
                                }
                                else
                                {
                                    for (int num9 = 0; num9 < 10; num9++)
                                    {
                                        int num10 = Dust.NewDust(new Vector2(npc.Position.X - 6f, npc.Position.Y + (float)(npc.height / 2) - 8f), npc.width + 12, 24, 35, 0f, 0f, 0, default(Color), 1f);
                                        Dust expr_89F_cp_0 = Main.dust[num10];
                                        expr_89F_cp_0.velocity.Y = expr_89F_cp_0.velocity.Y - 1.5f;
                                        Dust expr_8BD_cp_0 = Main.dust[num10];
                                        expr_8BD_cp_0.velocity.X = expr_8BD_cp_0.velocity.X * 2.5f;
                                        Main.dust[num10].scale = 1.3f;
                                        Main.dust[num10].alpha = 100;
                                        Main.dust[num10].noGravity = true;
                                    }
                                }
                            }
                        }
                    }
                    if (!npc.wet)
                    {
                        npc.lavaWet = false;
                    }
                    if (npc.wetCount > 0)
                    {
                        npc.wetCount -= 1;
                    }
                    bool flag3 = false;
                    if (npc.aiStyle == 10)
                    {
                        flag3 = true;
                    }
                    if (npc.aiStyle == 3 && npc.directionY == 1)
                    {
                        flag3 = true;
                    }
                    npc.oldVelocity = npc.Velocity;
                    npc.collideX = false;
                    npc.collideY = false;
                    if (npc.wet)
                    {
                        Vector2 vector = npc.Velocity;
                        npc.Velocity = Collision.TileCollision(npc.Position, npc.Velocity, npc.width, npc.height, flag3, flag3);
                        if (Collision.up)
                        {
                            npc.Velocity.Y = 0.01f;
                        }
                        Vector2 value = npc.Velocity * 0.5f;
                        if (npc.Velocity.X != vector.X)
                        {
                            value.X = npc.Velocity.X;
                            npc.collideX = true;
                        }
                        if (npc.Velocity.Y != vector.Y)
                        {
                            value.Y = npc.Velocity.Y;
                            npc.collideY = true;
                        }
                        npc.oldPosition = npc.Position;
                        npc.Position += value;
                    }
                    else
                    {
                        npc.Velocity = Collision.TileCollision(npc.Position, npc.Velocity, npc.width, npc.height, flag3, flag3);
                        if (Collision.up)
                        {
                            npc.Velocity.Y = 0.01f;
                        }
                        if (npc.oldVelocity.X != npc.Velocity.X)
                        {
                            npc.collideX = true;
                        }
                        if (npc.oldVelocity.Y != npc.Velocity.Y)
                        {
                            npc.collideY = true;
                        }
                        npc.oldPosition = npc.Position;
                        npc.Position += npc.Velocity;
                    }
                }
                else
                {
                    npc.oldPosition = npc.Position;
                    npc.Position += npc.Velocity;
                }
                if (!npc.Active)
                {
                    npc.netUpdate = true;
                }
                if (npc.netUpdate)
                {
                    NetMessage.SendData(23, -1, -1, "", i);
                }
                npc.FindFrame();
                npc.CheckActive();
                npc.netUpdate = false;
            }
        }
        
        public Color GetAlpha(Color newColor)
        {
            int r = (int)newColor.R - this.alpha;
            int g = (int)newColor.G - this.alpha;
            int b = (int)newColor.B - this.alpha;
            int num = (int)newColor.A - this.alpha;
            if (this.Type == 25 || this.Type == 30 || this.Type == 33)
            {
                r = (int)newColor.R;
                g = (int)newColor.G;
                b = (int)newColor.B;
            }
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
       
        public String GetChat()
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            for (int i = 0; i < MAX_NPCS; i++)
            {
                if (Main.npcs[i].Type == 17)
                {
                    flag = true;
                }
                else
                {
                    if (Main.npcs[i].Type == 18)
                    {
                        flag2 = true;
                    }
                    else
                    {
                        if (Main.npcs[i].Type == 19)
                        {
                            flag3 = true;
                        }
                        else
                        {
                            if (Main.npcs[i].Type == 20)
                            {
                                flag4 = true;
                            }
                            else
                            {
                                if (Main.npcs[i].Type == 37)
                                {
                                    flag5 = true;
                                }
                                else
                                {
                                    if (Main.npcs[i].Type == 38)
                                    {
                                        flag6 = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            String result = "";
            if (this.Type == 17)
            {
                if (Main.dayTime)
                {
                    if (Main.time < 16200.0)
                    {
                        if (Main.rand.Next(2) == 0)
                        {
                            result = "Sword beats paper, get one today.";
                        }
                        else
                        {
                            result = "Lovely morning, wouldn't you say? Was there something you needed?";
                        }
                    }
                    else
                    {
                        if (Main.time > 37800.0)
                        {
                            if (Main.rand.Next(2) == 0)
                            {
                                result = "Night be upon us soon, friend. Make your choices while you can.";
                            }
                            else
                            {
                                result = "Ah, they will tell tales of " + Main.players[Main.myPlayer].Name + " some day... good ones I'm sure.";
                            }
                        }
                        else
                        {
                            int num = Main.rand.Next(3);
                            if (num == 0)
                            {
                                result = "Check out my dirt blocks, they are extra dirty.";
                            }
                            if (num == 1)
                            {
                                result = "Boy, that sun is hot! I do have some perfectly ventilated armor.";
                            }
                            else
                            {
                                result = "The sun is high, but my prices are not.";
                            }
                        }
                    }
                }
                else
                {
                    if (Main.bloodMoon)
                    {
                        if (Main.rand.Next(2) == 0)
                        {
                            result = "Have you seen Chith...Shith.. Chat... The big eye?";
                        }
                        else
                        {
                            result = "Keep your eye on the prize, buy a lense!";
                        }
                    }
                    else
                    {
                        if (Main.time < 9720.0)
                        {
                            if (Main.rand.Next(2) == 0)
                            {
                                result = "Kosh, kapleck Mog. Oh sorry, that's klingon for 'Buy something or die.'";
                            }
                            else
                            {
                                result = Main.players[Main.myPlayer].Name + " is it? I've heard good things, friend!";
                            }
                        }
                        else
                        {
                            if (Main.time > 22680.0)
                            {
                                if (Main.rand.Next(2) == 0)
                                {
                                    result = "I hear there's a secret treasure... oh never mind.";
                                }
                                else
                                {
                                    result = "Angel Statue you say? I'm sorry, I'm not a junk dealer.";
                                }
                            }
                            else
                            {
                                int num2 = Main.rand.Next(3);
                                if (num2 == 0)
                                {
                                    result = "The last guy who was here left me some junk..er I mean.. treasures!";
                                }
                                if (num2 == 1)
                                {
                                    result = "I wonder if the moon is made of cheese...huh, what? Oh yes, buy something!";
                                }
                                else
                                {
                                    result = "Did you say gold?  I'll take that off of ya'.";
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.Type == 18)
                {
                    if (flag6 && Main.rand.Next(4) == 0)
                    {
                        result = "I wish that bomb maker would be more careful.  I'm getting tired of having to sew his limbs back on every day.";
                    }
                    else
                    {
                        if ((double)Main.players[Main.myPlayer].statLife < (double)Main.players[Main.myPlayer].statLifeMax * 0.33)
                        {
                            int num3 = Main.rand.Next(5);
                            if (num3 == 0)
                            {
                                result = "I think you look better this way.";
                            }
                            else
                            {
                                if (num3 == 1)
                                {
                                    result = "Eww.. What happened to your face?";
                                }
                                else
                                {
                                    if (num3 == 2)
                                    {
                                        result = "MY GOODNESS! I'm good but I'm not THAT good.";
                                    }
                                    else
                                    {
                                        if (num3 == 3)
                                        {
                                            result = "Dear friends we are gathered here today to bid farewell... oh, you'll be fine.";
                                        }
                                        else
                                        {
                                            result = "You left your arm over there. Let me get that for you..";
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if ((double)Main.players[Main.myPlayer].statLife < (double)Main.players[Main.myPlayer].statLifeMax * 0.66)
                            {
                                int num4 = Main.rand.Next(4);
                                if (num4 == 0)
                                {
                                    result = "Quit being such a baby! I've seen worse.";
                                }
                                else
                                {
                                    if (num4 == 1)
                                    {
                                        result = "That's gonna need stitches!";
                                    }
                                    else
                                    {
                                        if (num4 == 2)
                                        {
                                            result = "Trouble with those bullies again?";
                                        }
                                        else
                                        {
                                            result = "You look half digested. Have you been chasing slimes again?";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                int num5 = Main.rand.Next(3);
                                if (num5 == 0)
                                {
                                    result = "Turn your head and cough.";
                                }
                                else
                                {
                                    if (num5 == 1)
                                    {
                                        result = "That's not the biggest I've ever seen... Yes, I've seen bigger wounds for sure.";
                                    }
                                    else
                                    {
                                        result = "Show me where it hurts.";
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (this.Type == 19)
                    {
                        if (flag2 && Main.rand.Next(4) == 0)
                        {
                            result = "Make it quick! I've got a date with the nurse in an hour.";
                        }
                        else
                        {
                            if (flag4 && Main.rand.Next(4) == 0)
                            {
                                result = "That dryad is a looker.  Too bad she's such a prude.";
                            }
                            else
                            {
                                if (flag6 && Main.rand.Next(4) == 0)
                                {
                                    result = "Don't bother with that firework vendor, I've got all you need right here.";
                                }
                                else
                                {
                                    if (Main.bloodMoon)
                                    {
                                        result = "I love nights like tonight.  There is never a shortage of things to kill!";
                                    }
                                    else
                                    {
                                        int num6 = Main.rand.Next(2);
                                        if (num6 == 0)
                                        {
                                            result = "I see you're eyeballin' the Minishark.. You really don't want to know how it was made.";
                                        }
                                        else
                                        {
                                            if (num6 == 1)
                                            {
                                                result = "Keep your hands off my gun, buddy!";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.Type == 20)
                        {
                            if (flag3 && Main.rand.Next(4) == 0)
                            {
                                result = "I wish that gun seller would stop talking to me. Doesn't he realize I'm 500 years old?";
                            }
                            else
                            {
                                if (flag && Main.rand.Next(4) == 0)
                                {
                                    result = "That merchant keeps trying to sell me an angel statue. Everyone knows that they don't do anything.";
                                }
                                else
                                {
                                    if (flag5 && Main.rand.Next(4) == 0)
                                    {
                                        result = "Have you seen the old man walking around the dungeon? He doesn't look well at all...";
                                    }
                                    else
                                    {
                                        if (Main.bloodMoon)
                                        {
                                            result = "It is an evil moon tonight. Be careful.";
                                        }
                                        else
                                        {
                                            int num7 = Main.rand.Next(6);
                                            if (num7 == 0)
                                            {
                                                result = "You must cleanse the world of this corruption.";
                                            }
                                            else
                                            {
                                                if (num7 == 1)
                                                {
                                                    result = "Be safe; Terraria needs you!";
                                                }
                                                else
                                                {
                                                    if (num7 == 2)
                                                    {
                                                        result = "The sands of time are flowing. And well, you are not aging very gracefully.";
                                                    }
                                                    else
                                                    {
                                                        if (num7 == 3)
                                                        {
                                                            result = "What's this about me having more 'bark' than bite?";
                                                        }
                                                        else
                                                        {
                                                            if (num7 == 4)
                                                            {
                                                                result = "So two goblins walk into a bar, and one says to the other, 'Want to get a Gobblet of beer?!'";
                                                            }
                                                            else
                                                            {
                                                                result = "Be safe; Terraria needs you!";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.Type == 22)
                            {
                                if (Main.bloodMoon)
                                {
                                    result = "You can tell a Blood Moon is out when the sky turns red. There is something about it that causes monsters to swarm.";
                                }
                                else
                                {
                                    if (!Main.dayTime)
                                    {
                                        result = "You should stay indoors at night. It is very dangerous to be wandering around in the dark.";
                                    }
                                    else
                                    {
                                        int num8 = Main.rand.Next(3);
                                        if (num8 == 0)
                                        {
                                            result = "Greetings, " + Main.players[Main.myPlayer].Name + ". Is there something I can help you with?";
                                        }
                                        else
                                        {
                                            if (num8 == 1)
                                            {
                                                result = "I am here to give you advice on what to do next.  It is recommended that you talk with me anytime you get stuck.";
                                            }
                                            else
                                            {
                                                if (num8 == 2)
                                                {
                                                    result = "They say there is a person who will tell you how to survive in this land... oh wait. That's me.";
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (this.Type == 37)
                                {
                                    if (Main.dayTime)
                                    {
                                        int num9 = Main.rand.Next(2);
                                        if (num9 == 0)
                                        {
                                            result = "I cannot let you enter until you free me of my curse.";
                                        }
                                        else
                                        {
                                            if (num9 == 1)
                                            {
                                                result = "Come back at night if you wish to enter.";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Main.players[Main.myPlayer].statLifeMax < 300 || Main.players[Main.myPlayer].statDefense < 10)
                                        {
                                            int num10 = Main.rand.Next(2);
                                            if (num10 == 0)
                                            {
                                                result = "You are far to weak to defeat my curse.  Come back when you aren't so worthless.";
                                            }
                                            else
                                            {
                                                if (num10 == 1)
                                                {
                                                    result = "You pathetic fool.  You cannot hope to face my master as you are now.";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            int num11 = Main.rand.Next(2);
                                            if (num11 == 0)
                                            {
                                                result = "You just might be strong enough to free me from my curse...";
                                            }
                                            else
                                            {
                                                if (num11 == 1)
                                                {
                                                    result = "Stranger, do you possess the strength to defeat my master?";
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (this.Type == 38)
                                    {
                                        if (Main.bloodMoon)
                                        {
                                            result = "I've got something for them zombies alright!";
                                        }
                                        else
                                        {
                                            if (flag3 && Main.rand.Next(4) == 0)
                                            {
                                                result = "Even the gun dealer wants what I'm selling!";
                                            }
                                            else
                                            {
                                                if (flag2 && Main.rand.Next(4) == 0)
                                                {
                                                    result = "I'm sure the nurse will help if you accidentally lose a limb to these.";
                                                }
                                                else
                                                {
                                                    if (flag4 && Main.rand.Next(4) == 0)
                                                    {
                                                        result = "Why purify the world when you can just blow it up?";
                                                    }
                                                    else
                                                    {
                                                        int num12 = Main.rand.Next(4);
                                                        if (num12 == 0)
                                                        {
                                                            result = "Explosives are da' bomb these days.  Buy some now!";
                                                        }
                                                        else
                                                        {
                                                            if (num12 == 1)
                                                            {
                                                                result = "It's a good day to die!";
                                                            }
                                                            else
                                                            {
                                                                if (num12 == 2)
                                                                {
                                                                    result = "I wonder what happens if I... (BOOM!)... Oh, sorry, did you need that leg?";
                                                                }
                                                                else
                                                                {
                                                                    if (num12 == 3)
                                                                    {
                                                                        result = "Dynamite, my own special cure-all for what ails ya.";
                                                                    }
                                                                    else
                                                                    {
                                                                        result = "Check out my goods; they have explosive prices!";
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (this.Type == 54)
                                        {
                                            if (Main.bloodMoon)
                                            {
                                                result = Main.players[Main.myPlayer].Name + "... we have a problem! Its a blood moon out there!";
                                            }
                                            else
                                            {
                                                if (flag2 && Main.rand.Next(4) == 0)
                                                {
                                                    result = "T'were I younger, I would ask the nurse out. I used to be quite the lady killer.";
                                                }
                                                else
                                                {
                                                    if (Main.players[Main.myPlayer].head == 24)
                                                    {
                                                        result = "That Red Hat of yours looks familiar...";
                                                    }
                                                    else
                                                    {
                                                        int num13 = Main.rand.Next(4);
                                                        if (num13 == 0)
                                                        {
                                                            result = "Thanks again for freeing me from my curse. Felt like something jumped up and bit me";
                                                        }
                                                        else
                                                        {
                                                            if (num13 == 1)
                                                            {
                                                                result = "Mama always said I would make a great tailor.";
                                                            }
                                                            else
                                                            {
                                                                if (num13 == 2)
                                                                {
                                                                    result = "Life's like a box of clothes, you never know what you are gonna wear!";
                                                                }
                                                                else
                                                                {
                                                                    result = "Being cursed was lonely, so I once made a friend out of leather. I named him Wilson.";
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
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

        private void ShowDamage(double damage, float hitDirection, int goreStart, double lifeModifier = 100d, int type = 5, int alpha = 0, Color color = default(Color))
        {
            if (this.life > 0)
            {
                for (int i = 0; (double)i < (damage / (double)this.lifeMax * lifeModifier); i++)
                {
                    Dust.NewDust(new Vector2(Position.X, Position.Y), width, height, 5, hitDirection, -1f, alpha, default(Color), 1f);
                }
                return;
            }
            for (int num43 = 0; num43 < 50; num43++)
            {
                float speedX = 2.5f * (float)hitDirection;
                Dust.NewDust(new Vector2(Position.X, Position.Y), width, height, type, speedX, -2.5f, 0, default(Color), 1f);
            }
            SetGore(goreStart++);
            Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 20f), this.Velocity, goreStart);
            Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 20f), this.Velocity, goreStart++);
            Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 34f), this.Velocity, goreStart);
            Gore.NewGore(new Vector2(this.Position.X, this.Position.Y + 34f), this.Velocity, goreStart);
        }
        
        public object Clone()
        {
            return base.MemberwiseClone();
        }
    }
}
