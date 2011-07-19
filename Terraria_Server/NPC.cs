using System;
using Terraria_Server.Misc;
using Terraria_Server.Plugin;
using Terraria_Server.Events;
using Terraria_Server.Commands;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;

namespace Terraria_Server
{
    public class NPC : BaseEntity
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

        public NPCType type { get; set; }

        public override int Type
        {
            get
            {
                return (int)type;
            }
            set
            {
                type = (NPCType)value;
            }
        }
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
        public float value;
        public bool wet;
        public byte wetCount;
        
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
        public int[] immune = new int[256];
        public int life;
        public int lifeMax;
        public int oldDirectionY;
        public Vector2 oldPosition;
        public Vector2 oldVelocity;
        public int soundDelay;
        public int whoAmI;
        
        public NPC()
        {
            color = default(Color);
            homeTileX = -1;
            homeTileY = -1;
            knockBackResist = 1f;
            Name = "";
            oldTarget = target;
            scale = 1f;
            slots = 1f;
            spriteDirection = -1;
            target = 255;
            targetRect = default(Rectangle);
            timeLeft = NPC.ACTIVE_TIME;
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
                    if (npc.wet)
                    {
                        if (npc.type == NPCType.N59_LAVA_SLIME)
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
                        if (npc.type == NPCType.N59_LAVA_SLIME)
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
                                if (npc.type == NPCType.N59_LAVA_SLIME)
                                {
                                    npc.Velocity.Y = npc.Velocity.Y - 2f;
                                }
                                npc.Velocity.X = npc.Velocity.X + (float)(3 * npc.direction);
                                if (npc.type == NPCType.N59_LAVA_SLIME)
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
                            if (npc.type == NPCType.N59_LAVA_SLIME)
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
                        if (Main.dayTime && (double)npc.Position.Y <= Main.worldSurface * 16.0 && npc.type == NPCType.N02_DEMON_EYE)
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
                            if ((!Main.dayTime || (double)npc.Position.Y > Main.worldSurface * 16.0 || npc.type == NPCType.N26_GOBLIN_PEON || npc.type == NPCType.N27_GOBLIN_THIEF || npc.type == NPCType.N28_GOBLIN_WARRIOR || npc.type == NPCType.N31_ANGRY_BONES || npc.type == NPCType.N47_CORRUPT_BUNNY || npc.type == NPCType.N67_CRAB) && npc.ai[3] < (float)num3)
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
                            if (npc.type == NPCType.N27_GOBLIN_THIEF)
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
                                if (npc.type == NPCType.N21_SKELETON || npc.type == NPCType.N26_GOBLIN_PEON || npc.type == NPCType.N31_ANGRY_BONES || npc.type == NPCType.N47_CORRUPT_BUNNY)
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
                                    if (npc.type == NPCType.N67_CRAB)
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
                            int num4 = (int)((npc.Position.X + (float)(npc.Width / 2) + (float)(15 * npc.direction)) / 16f);
                            int num5 = (int)((npc.Position.Y + (float)npc.Height - 15f) / 16f);
                            bool flag2 = true;
                            if (npc.type == NPCType.N47_CORRUPT_BUNNY || npc.type == NPCType.N67_CRAB)
                            {
                                flag2 = false;
                            }
                            if (Main.tile.At(num4, num5 - 1).Active && Main.tile.At(num4, num5 - 1).Type == 10 && flag2)
                            {
                                npc.ai[2] += 1f;
                                npc.ai[3] = 0f;
                                if (npc.ai[2] >= 60f)
                                {
                                    if (!Main.bloodMoon && npc.type == NPCType.N03_ZOMBIE)
                                    {
                                        npc.ai[1] = 0f;
                                    }
                                    npc.Velocity.X = 0.5f * (float)(-(float)npc.direction);
                                    npc.ai[1] += 1f;
                                    if (npc.type == NPCType.N27_GOBLIN_THIEF)
                                    {
                                        npc.ai[1] += 1f;
                                    }
                                    if (npc.type ==  NPCType.N31_ANGRY_BONES)
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
                                        if (npc.type == NPCType.N26_GOBLIN_PEON)
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
                                    if (Main.tile.At(num4, num5 - 2).Active && Main.tileSolid[(int)Main.tile.At(num4, num5 - 2).Type])
                                    {
                                        if (Main.tile.At(num4, num5 - 3).Active && Main.tileSolid[(int)Main.tile.At(num4, num5 - 3).Type])
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
                                        if (Main.tile.At(num4, num5 - 1).Active && Main.tileSolid[(int)Main.tile.At(num4, num5 - 1).Type])
                                        {
                                            npc.Velocity.Y = -6f;
                                            npc.netUpdate = true;
                                        }
                                        else
                                        {
                                            if (Main.tile.At(num4, num5).Active && Main.tileSolid[(int)Main.tile.At(num4, num5).Type])
                                            {
                                                npc.Velocity.Y = -5f;
                                                npc.netUpdate = true;
                                            }
                                            else
                                            {
                                                if (npc.directionY < 0 && npc.type != NPCType.N67_CRAB && (!Main.tile.At(num4, num5 + 1).Active || !Main.tileSolid[(int)Main.tile.At(num4, num5 + 1).Type]) && (!Main.tile.At(num4 + npc.direction, num5 + 1).Active || !Main.tileSolid[(int)Main.tile.At(num4 + npc.direction, num5 + 1).Type]))
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
                                if ((npc.type == NPCType.N31_ANGRY_BONES || npc.type == NPCType.N47_CORRUPT_BUNNY) && npc.Velocity.Y == 0f && Math.Abs(npc.Position.X + (float)(npc.Width / 2) - (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2))) < 100f && Math.Abs(npc.Position.Y + (float)(npc.Height / 2) - (Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2))) < 50f && ((npc.direction > 0 && npc.Velocity.X >= 1f) || (npc.direction < 0 && npc.Velocity.X <= -1f)))
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
                                float num6 = npc.Position.X + (float)(npc.Width / 2) - Main.players[npc.target].Position.X - (float)(Main.players[npc.target].Width / 2);
                                float num7 = npc.Position.Y + (float)npc.Height - 59f - Main.players[npc.target].Position.Y - (float)(Main.players[npc.target].Height / 2);
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
                                            Vector2 vector = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                            float num13 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector.X;
                                            float num14 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - 200f - vector.Y;
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
                                                if (npc.Position.Y + (float)npc.Height < Main.players[npc.target].Position.Y && num16 < 500f)
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
                                                        float num18 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector.X;
                                                        float num19 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector.Y;
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
                                                Vector2 vector4 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                float num23 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector4.X;
                                                float num24 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector4.Y;
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

                                            if (npc.ai[1] == 100f)
                                            {
                                                npc.ai[0] += 1f;
                                                npc.ai[1] = 0f;
                                                if (npc.ai[0] == 3f)
                                                {
                                                    npc.ai[2] = 0f;
                                                }
                                            }
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
                                                Vector2 vector5 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                float num28 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector5.X;
                                                float num29 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - 120f - vector5.Y;
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
                                                    Vector2 vector6 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                    float num32 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector6.X;
                                                    float num33 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector6.Y;
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
                                    if (npc.type == NPCType.N06_EATER_OF_SOULS || npc.type == NPCType.N42_HORNET)
                                    {
                                        num35 = 4f;
                                        num36 = 0.02f;
                                    }
                                    else
                                    {
                                        if (npc.type == NPCType.N23_METEOR_HEAD)
                                        {
                                            num35 = 2f;
                                            num36 = 0.03f;
                                        }
                                    }
                                    Vector2 vector7 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                    float num37 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector7.X;
                                    float num38 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector7.Y;
                                    float num39 = (float)Math.Sqrt((double)(num37 * num37 + num38 * num38));
                                    float num40 = num39;
                                    num39 = num35 / num39;
                                    num37 *= num39;
                                    num38 *= num39;
                                    if (npc.type == NPCType.N06_EATER_OF_SOULS || npc.type == NPCType.N42_HORNET)
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
                                        if (npc.type != NPCType.N06_EATER_OF_SOULS && npc.Velocity.X < 0f && num37 > 0f)
                                        {
                                            npc.Velocity.X = npc.Velocity.X + num36;
                                        }
                                    }
                                    else
                                    {
                                        if (npc.Velocity.X > num37)
                                        {
                                            npc.Velocity.X = npc.Velocity.X - num36;
                                            if (npc.type != NPCType.N06_EATER_OF_SOULS && npc.Velocity.X > 0f && num37 < 0f)
                                            {
                                                npc.Velocity.X = npc.Velocity.X - num36;
                                            }
                                        }
                                    }
                                    if (npc.Velocity.Y < num38)
                                    {
                                        npc.Velocity.Y = npc.Velocity.Y + num36;
                                        if (npc.type != NPCType.N06_EATER_OF_SOULS && npc.Velocity.Y < 0f && num38 > 0f)
                                        {
                                            npc.Velocity.Y = npc.Velocity.Y + num36;
                                        }
                                    }
                                    else
                                    {
                                        if (npc.Velocity.Y > num38)
                                        {
                                            npc.Velocity.Y = npc.Velocity.Y - num36;
                                            if (npc.type != NPCType.N06_EATER_OF_SOULS && npc.Velocity.Y > 0f && num38 < 0f)
                                            {
                                                npc.Velocity.Y = npc.Velocity.Y - num36;
                                            }
                                        }
                                    }
                                    if (npc.type == NPCType.N23_METEOR_HEAD)
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
                                        if (npc.type == NPCType.N06_EATER_OF_SOULS)
                                        {
                                            npc.rotation = (float)Math.Atan2((double)num38, (double)num37) - 1.57f;
                                        }
                                        else
                                        {
                                            if (npc.type == NPCType.N42_HORNET)
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
                                    if (npc.type == NPCType.N06_EATER_OF_SOULS || npc.type == NPCType.N23_METEOR_HEAD || npc.type == NPCType.N42_HORNET)
                                    {
                                        float num41 = 0.7f;
                                        if (npc.type == NPCType.N06_EATER_OF_SOULS)
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
                                    }
                                    if ((Main.dayTime && npc.type != NPCType.N06_EATER_OF_SOULS && npc.type != NPCType.N23_METEOR_HEAD && npc.type != NPCType.N42_HORNET) || Main.players[npc.target].dead)
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
                                        if ((npc.type == NPCType.N07_DEVOURER_HEAD || npc.type == NPCType.N08_DEVOURER_BODY || npc.type == NPCType.N10_GIANT_WORM_HEAD || npc.type == NPCType.N11_GIANT_WORM_BODY || npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD || npc.type == NPCType.N14_EATER_OF_WORLDS_BODY || npc.type == NPCType.N39_BONE_SERPENT_HEAD || npc.type == NPCType.N40_BONE_SERPENT_BODY) && npc.ai[0] == 0f)
                                        {
                                            if (npc.type == NPCType.N07_DEVOURER_HEAD || npc.type == NPCType.N10_GIANT_WORM_HEAD || npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD || npc.type == NPCType.N39_BONE_SERPENT_HEAD)
                                            {
                                                npc.ai[2] = 10f;
                                                if (npc.type == NPCType.N10_GIANT_WORM_HEAD)
                                                {
                                                    npc.ai[2] = 5f;
                                                }
                                                if (npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD)
                                                {
                                                    npc.ai[2] = 50f;
                                                }
                                                if (npc.type == NPCType.N39_BONE_SERPENT_HEAD)
                                                {
                                                    npc.ai[2] = 15f;
                                                }
                                                npc.ai[0] = (float)NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)(npc.Position.Y + (float)npc.Height), npc.Type + 1, npc.whoAmI);
                                            }
                                            else
                                            {
                                                if ((npc.type == NPCType.N08_DEVOURER_BODY || npc.type == NPCType.N11_GIANT_WORM_BODY || npc.type == NPCType.N14_EATER_OF_WORLDS_BODY || npc.type == NPCType.N40_BONE_SERPENT_BODY) && npc.ai[2] > 0f)
                                                {
                                                    npc.ai[0] = (float)NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)(npc.Position.Y + (float)npc.Height), npc.Type, npc.whoAmI);
                                                }
                                                else
                                                {
                                                    npc.ai[0] = (float)NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)(npc.Position.Y + (float)npc.Height), npc.Type + 1, npc.whoAmI);
                                                }
                                            }
                                            Main.npcs[(int)npc.ai[0]].ai[1] = (float)npc.whoAmI;
                                            Main.npcs[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;
                                            npc.netUpdate = true;
                                        }
                                        if ((npc.type == NPCType.N08_DEVOURER_BODY || npc.type == NPCType.N09_DEVOURER_TAIL || npc.type == NPCType.N11_GIANT_WORM_BODY || npc.type == NPCType.N12_GIANT_WORM_TAIL || npc.type == NPCType.N40_BONE_SERPENT_BODY || npc.type == NPCType.N41_BONE_SERPENT_TAIL) && (!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != npc.aiStyle))
                                        {
                                            npc.life = 0;
                                            npc.HitEffect(0, 10.0);
                                            npc.Active = false;
                                        }
                                        if ((npc.type == NPCType.N07_DEVOURER_HEAD || npc.type == NPCType.N08_DEVOURER_BODY || npc.type == NPCType.N10_GIANT_WORM_HEAD || npc.type == NPCType.N11_GIANT_WORM_BODY || npc.type == NPCType.N39_BONE_SERPENT_HEAD || npc.type == NPCType.N40_BONE_SERPENT_BODY) && !Main.npcs[(int)npc.ai[0]].Active)
                                        {
                                            npc.life = 0;
                                            npc.HitEffect(0, 10.0);
                                            npc.Active = false;
                                        }
                                        if (npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD || npc.type == NPCType.N14_EATER_OF_WORLDS_BODY || npc.type == NPCType.N15_EATER_OF_WORLDS_TAIL)
                                        {
                                            if (!Main.npcs[(int)npc.ai[1]].Active && !Main.npcs[(int)npc.ai[0]].Active)
                                            {
                                                npc.life = 0;
                                                npc.HitEffect(0, 10.0);
                                                npc.Active = false;
                                            }
                                            if (npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD && !Main.npcs[(int)npc.ai[0]].Active)
                                            {
                                                npc.life = 0;
                                                npc.HitEffect(0, 10.0);
                                                npc.Active = false;
                                            }
                                            if (npc.type == NPCType.N15_EATER_OF_WORLDS_TAIL && !Main.npcs[(int)npc.ai[1]].Active)
                                            {
                                                npc.life = 0;
                                                npc.HitEffect(0, 10.0);
                                                npc.Active = false;
                                            }
                                            if (npc.type == NPCType.N14_EATER_OF_WORLDS_BODY && !Main.npcs[(int)npc.ai[1]].Active)
                                            {
                                                int num45 = npc.whoAmI;
                                                int num46 = npc.life;
                                                float num47 = npc.ai[0];
                                                npc = Registries.NPC.Create(13);
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
                                            if (npc.type == NPCType.N14_EATER_OF_WORLDS_BODY && !Main.npcs[(int)npc.ai[0]].Active)
                                            {
                                                int num48 = npc.life;
                                                int num49 = npc.whoAmI;
                                                float num50 = npc.ai[1];
                                                npc = Registries.NPC.Create(npc.Type);
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
                                                    if (Main.npcs[l].Active && (Main.npcs[l].type == NPCType.N13_EATER_OF_WORLDS_HEAD || Main.npcs[l].type == NPCType.N14_EATER_OF_WORLDS_BODY || Main.npcs[l].type == NPCType.N15_EATER_OF_WORLDS_TAIL))
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
                                        int num52 = (int)((npc.Position.X + (float)npc.Width) / 16f) + 2;
                                        int num53 = (int)(npc.Position.Y / 16f) - 1;
                                        int num54 = (int)((npc.Position.Y + (float)npc.Height) / 16f) + 2;
                                        if (num51 < 0)
                                        {
                                            num51 = 0;
                                        }
                                        if (num52 >= Main.maxTilesX)
                                        {
                                            num52 = Main.maxTilesX - 1;
                                        }
                                        if (num53 < 0)
                                        {
                                            num53 = 0;
                                        }
                                        if (num54 >= Main.maxTilesY)
                                        {
                                            num54 = Main.maxTilesY - 1;
                                        }
                                        bool flag6 = false;
                                        for (int m = num51; m < num52; m++)
                                        {
                                            for (int n = num53; n < num54; n++)
                                            {
                                                if (((Main.tile.At(m, n).Active && (Main.tileSolid[(int)Main.tile.At(m, n).Type] || (Main.tileSolidTop[(int)Main.tile.At(m, n).Type] && Main.tile.At(m, n).FrameY == 0))) || Main.tile.At(m, n).Liquid > 64))
                                                {
                                                    Vector2 vector8;
                                                    vector8.X = (float)(m * 16);
                                                    vector8.Y = (float)(n * 16);
                                                    if (npc.Position.X + (float)npc.Width > vector8.X && npc.Position.X < vector8.X + 16f && npc.Position.Y + (float)npc.Height > vector8.Y && npc.Position.Y < vector8.Y + 16f)
                                                    {
                                                        flag6 = true;
                                                        if (Main.rand.Next(40) == 0 && Main.tile.At(m, n).Active)
                                                        {
                                                            WorldGen.KillTile(m, n, true, true, false);
                                                        }
                                                        if (Main.tile.At(m, n).Type == 2)
                                                        {
                                                            byte arg_4132_0 = Main.tile.At(m, n - 1).Type;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        float num55 = 8f;
                                        float num56 = 0.07f;
                                        if (npc.type == NPCType.N10_GIANT_WORM_HEAD)
                                        {
                                            num55 = 6f;
                                            num56 = 0.05f;
                                        }
                                        if (npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD)
                                        {
                                            num55 = 11f;
                                            num56 = 0.08f;
                                        }
                                        Vector2 vector9 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                        float num57 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector9.X;
                                        float num58 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector9.Y;
                                        float num59 = (float)Math.Sqrt((double)(num57 * num57 + num58 * num58));
                                        if (npc.ai[1] > 0f)
                                        {
                                            num57 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - vector9.X;
                                            num58 = Main.npcs[(int)npc.ai[1]].Position.Y + (float)(Main.npcs[(int)npc.ai[1]].Height / 2) - vector9.Y;
                                            npc.rotation = (float)Math.Atan2((double)num58, (double)num57) + 1.57f;
                                            num59 = (float)Math.Sqrt((double)(num57 * num57 + num58 * num58));
                                            num59 = (num59 - (float)npc.Width) / num59;
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
                                            if ((npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD || npc.type == NPCType.N07_DEVOURER_HEAD) && !Main.players[npc.target].zoneEvil)
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
                                            int num63 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
                                            int num64 = (int)(npc.Position.Y + (float)npc.Height + 1f) / 16;
                                            if (!npc.townNPC)
                                            {
                                                npc.homeTileX = num63;
                                                npc.homeTileY = num64;
                                            }
                                            if (npc.type == NPCType.N46_BUNNY && npc.target == 255)
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
                                                    if (Main.players[num65].Position.X + (float)(Main.players[num65].Width / 2) < npc.Position.X + (float)(npc.Width / 2))
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
                                            if (npc.type == NPCType.N37_OLD_MAN)
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
                                                    Rectangle rectangle = new Rectangle((int)(npc.Position.X + (float)(npc.Width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(npc.Position.Y + (float)(npc.Height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                                    if (num66 == 1)
                                                    {
                                                        rectangle = new Rectangle(npc.homeTileX * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, npc.homeTileY * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                                    }
                                                    for (int num67 = 0; num67 < 255; num67++)
                                                    {
                                                        if (Main.players[num67].Active)
                                                        {
                                                            Rectangle rectangle2 = new Rectangle((int)Main.players[num67].Position.X, (int)Main.players[num67].Position.Y, Main.players[num67].Width, Main.players[num67].Height);
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
                                                    if (npc.type == NPCType.N37_OLD_MAN || !Collision.SolidTiles(npc.homeTileX - 1, npc.homeTileX + 1, npc.homeTileY - 3, npc.homeTileY - 1))
                                                    {
                                                        npc.Velocity.X = 0f;
                                                        npc.Velocity.Y = 0f;
                                                        npc.Position.X = (float)(npc.homeTileX * 16 + 8 - npc.Width / 2);
                                                        npc.Position.Y = (float)(npc.homeTileY * 16 - npc.Height) - 0.1f;
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
                                                        if (npc.type == NPCType.N46_BUNNY)
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
                                                        if (npc.type == NPCType.N46_BUNNY)
                                                        {
                                                            npc.ai[1] -= (float)Main.rand.Next(100);
                                                        }
                                                        npc.ai[2] = 60f;
                                                        npc.netUpdate = true;
                                                    }
                                                    if (npc.closeDoor && ((npc.Position.X + (float)(npc.Width / 2)) / 16f > (float)(npc.doorX + 2) || (npc.Position.X + (float)(npc.Width / 2)) / 16f < (float)(npc.doorX - 2)))
                                                    {
                                                        bool flag9 = WorldGen.CloseDoor(npc.doorX, npc.doorY, false, DoorOpener.NPC);
                                                        if (flag9)
                                                        {
                                                            npc.closeDoor = false;
                                                            NetMessage.SendData(19, -1, -1, "", 1, (float)npc.doorX, (float)npc.doorY, (float)npc.direction);
                                                        }
                                                        if ((npc.Position.X + (float)(npc.Width / 2)) / 16f > (float)(npc.doorX + 4) || (npc.Position.X + (float)(npc.Width / 2)) / 16f < (float)(npc.doorX - 4) || (npc.Position.Y + (float)(npc.Height / 2)) / 16f > (float)(npc.doorY + 4) || (npc.Position.Y + (float)(npc.Height / 2)) / 16f < (float)(npc.doorY - 4))
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
                                                        int num68 = (int)((npc.Position.X + (float)(npc.Width / 2) + (float)(15 * npc.direction)) / 16f);
                                                        int num69 = (int)((npc.Position.Y + (float)npc.Height - 16f) / 16f);
                                                        if (npc.townNPC && Main.tile.At(num68, num69 - 2).Active && Main.tile.At(num68, num69 - 2).Type == 10 && (Main.rand.Next(10) == 0 || !Main.dayTime))
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
                                                                if (Main.tile.At(num68, num69 - 2).Active && Main.tileSolid[(int)Main.tile.At(num68, num69 - 2).Type] && !Main.tileSolidTop[(int)Main.tile.At(num68, num69 - 2).Type])
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
                                                                    if (Main.tile.At(num68, num69 - 1).Active && Main.tileSolid[(int)Main.tile.At(num68, num69 - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num68, num69 - 1).Type])
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
                                                                        if (Main.tile.At(num68, num69).Active && Main.tileSolid[(int)Main.tile.At(num68, num69).Type] && !Main.tileSolidTop[(int)Main.tile.At(num68, num69).Type])
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
                                                                    if (num63 >= npc.homeTileX - 35 && num63 <= npc.homeTileX + 35 && (!Main.tile.At(num68, num69 + 1).Active || !Main.tileSolid[(int)Main.tile.At(num68, num69 + 1).Type]) && (!Main.tile.At(num68 - npc.direction, num69 + 1).Active || !Main.tileSolid[(int)Main.tile.At(num68 - npc.direction, num69 + 1).Type]) && (!Main.tile.At(num68, num69 + 2).Active || !Main.tileSolid[(int)Main.tile.At(num68, num69 + 2).Type]) && (!Main.tile.At(num68 - npc.direction, num69 + 2).Active || !Main.tileSolid[(int)Main.tile.At(num68 - npc.direction, num69 + 2).Type]) && (!Main.tile.At(num68, num69 + 3).Active || !Main.tileSolid[(int)Main.tile.At(num68, num69 + 3).Type]) && (!Main.tile.At(num68 - npc.direction, num69 + 3).Active || !Main.tileSolid[(int)Main.tile.At(num68 - npc.direction, num69 + 3).Type]) && (!Main.tile.At(num68, num69 + 4).Active || !Main.tileSolid[(int)Main.tile.At(num68, num69 + 4).Type]) && (!Main.tile.At(num68 - npc.direction, num69 + 4).Active || !Main.tileSolid[(int)Main.tile.At(num68 - npc.direction, num69 + 4).Type]) && npc.type != NPCType.N46_BUNNY)
                                                                        {
                                                                            npc.direction *= -1;
                                                                            npc.Velocity.X = npc.Velocity.X * -1f;
                                                                            npc.netUpdate = true;
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
                                                            if (npc.Velocity.Y < 0f && npc.type == NPCType.N46_BUNNY)
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
                                                    npc.Position.X = npc.ai[2] * 16f - (float)(npc.Width / 2) + 8f;
                                                    npc.Position.Y = npc.ai[3] * 16f - (float)npc.Height;
                                                    npc.Velocity.X = 0f;
                                                    npc.Velocity.Y = 0f;
                                                    npc.ai[2] = 0f;
                                                    npc.ai[3] = 0f;
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
                                                                if ((num86 < num79 - 4 || num86 > num79 + 4 || num84 < num78 - 4 || num84 > num78 + 4) && (num86 < num81 - 1 || num86 > num81 + 1 || num84 < num80 - 1 || num84 > num80 + 1) && Main.tile.At(num84, num86).Active)
                                                                {
                                                                    bool flag12 = true;
                                                                    if (npc.type == NPCType.N32_DARK_CASTER && Main.tile.At(num84, num86 - 1).Wall == 0)
                                                                    {
                                                                        flag12 = false;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Main.tile.At(num84, num86 - 1).Lava)
                                                                        {
                                                                            flag12 = false;
                                                                        }
                                                                    }
                                                                    if (flag12 && Main.tileSolid[(int)Main.tile.At(num84, num86).Type] && !Collision.SolidTiles(num84 - 1, num84 + 1, num86 - 4, num86 - 1))
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
                                                        if (npc.type == NPCType.N29_GOBLIN_SORCERER || npc.type == NPCType.N45_TIM)
                                                        {
                                                            NPC.NewNPC((int)npc.Position.X + npc.Width / 2, (int)npc.Position.Y - 8, 30, 0);
                                                        }
                                                        else
                                                        {
                                                            if (npc.type == NPCType.N32_DARK_CASTER)
                                                            {
                                                                NPC.NewNPC((int)npc.Position.X + npc.Width / 2, (int)npc.Position.Y - 8, 33, 0);
                                                            }
                                                            else
                                                            {
                                                                NPC.NewNPC((int)npc.Position.X + npc.Width / 2 + npc.direction * 8, (int)npc.Position.Y + 20, 25, 0);
                                                            }
                                                        }
                                                    }
                                                }
                                                if (npc.type == NPCType.N29_GOBLIN_SORCERER || npc.type == NPCType.N45_TIM)
                                                {
                                                    if (Main.rand.Next(5) == 0)
                                                    {
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    if (npc.type == NPCType.N32_DARK_CASTER)
                                                    {
                                                        if (Main.rand.Next(2) == 0)
                                                        {
                                                            return;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Main.rand.Next(2) == 0)
                                                        {
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
                                                        if (npc.type == NPCType.N30_CHAOS_BALL)
                                                        {
                                                            NPC.maxSpawns = 8;
                                                        }
                                                        Vector2 vector10 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                        float num91 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector10.X;
                                                        float num92 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector10.Y;
                                                        float num93 = (float)Math.Sqrt((double)(num91 * num91 + num92 * num92));
                                                        num93 = num90 / num93;
                                                        npc.Velocity.X = num91 * num93;
                                                        npc.Velocity.Y = num92 * num93;
                                                    }
                                                    if (npc.timeLeft > 100)
                                                    {
                                                        npc.timeLeft = 100;
                                                    }
                                                    npc.rotation += 0.4f * (float)npc.direction;
                                                    return;
                                                }
                                                if (npc.aiStyle == 10)
                                                {
                                                    float num98 = 1f;
                                                    float num99 = 0.011f;
                                                    npc.TargetClosest(true);
                                                    Vector2 vector11 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                    float num100 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector11.X;
                                                    float num101 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector11.Y;
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
                                                            if (npc.type != NPCType.N68_DUNGEON_GUARDIAN)
                                                            {
                                                                int npcIndex = NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 36, npc.whoAmI);
                                                                Main.npcs[npcIndex].ai[0] = -1f;
                                                                Main.npcs[npcIndex].ai[1] = (float)npc.whoAmI;
                                                                Main.npcs[npcIndex].target = npc.target;
                                                                Main.npcs[npcIndex].netUpdate = true;
                                                                npcIndex = NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 36, npc.whoAmI);
                                                                Main.npcs[npcIndex].ai[0] = 1f;
                                                                Main.npcs[npcIndex].ai[1] = (float)npc.whoAmI;
                                                                Main.npcs[npcIndex].ai[3] = 150f;
                                                                Main.npcs[npcIndex].target = npc.target;
                                                                Main.npcs[npcIndex].netUpdate = true;
                                                            }
                                                        }
                                                        if (npc.type == NPCType.N68_DUNGEON_GUARDIAN && npc.ai[1] != 3f && npc.ai[1] != 2f)
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
                                                            if (npc.Position.X + (float)(npc.Width / 2) > Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2))
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
                                                            if (npc.Position.X + (float)(npc.Width / 2) < Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2))
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
                                                                Vector2 vector12 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                                float num105 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector12.X;
                                                                float num106 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector12.Y;
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
                                                                    Vector2 vector13 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                                    float num108 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector13.X;
                                                                    float num109 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector13.Y;
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
                                                        if (npc.ai[1] != 2f && npc.ai[1] != 3f && npc.type != NPCType.N68_DUNGEON_GUARDIAN)
                                                        {
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
                                                                    if (npc.Position.X + (float)(npc.Width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 120f * npc.ai[0])
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
                                                                    if (npc.Position.X + (float)(npc.Width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 120f * npc.ai[0])
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
                                                                    if (npc.Position.X + (float)(npc.Width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0])
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
                                                                    if (npc.Position.X + (float)(npc.Width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0])
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
                                                                Vector2 vector14 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                                float num113 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector14.X;
                                                                float num114 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector14.Y;
                                                                Math.Sqrt((double)(num113 * num113 + num114 * num114));
                                                                npc.rotation = (float)Math.Atan2((double)num114, (double)num113) + 1.57f;
                                                                return;
                                                            }
                                                            if (npc.ai[2] == 1f)
                                                            {
                                                                Vector2 vector15 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                                float num115 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector15.X;
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
                                                                    vector15 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                                    num115 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector15.X;
                                                                    num116 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector15.Y;
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
                                                                        Vector2 vector16 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                                        float num118 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector16.X;
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
                                                                        if (npc.Position.X + (float)(npc.Width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 500f || npc.Position.X + (float)(npc.Width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) + 500f)
                                                                        {
                                                                            npc.TargetClosest(true);
                                                                            npc.ai[2] = 5f;
                                                                            vector16 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                                            num118 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector16.X;
                                                                            num119 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector16.Y;
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
                                                                        if (npc.ai[2] == 5f && ((npc.Velocity.X > 0f && npc.Position.X + (float)(npc.Width / 2) > Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2)) || (npc.Velocity.X < 0f && npc.Position.X + (float)(npc.Width / 2) < Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2))))
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
                                                                if (!Main.tile.At((int)npc.ai[0], (int)npc.ai[1]).Active)
                                                                {
                                                                    npc.life = -1;
                                                                    npc.HitEffect(0, 10.0);
                                                                    npc.Active = false;
                                                                    return;
                                                                }
                                                                npc.TargetClosest(true);
                                                                float num121 = 0.05f;
                                                                float num122 = 150f;
                                                                if (npc.type == NPCType.N43_MAN_EATER)
                                                                {
                                                                    num122 = 200f;
                                                                }
                                                                Vector2 vector17 = new Vector2(npc.ai[0] * 16f + 8f, npc.ai[1] * 16f + 8f);
                                                                float num123 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - (float)(npc.Width / 2) - vector17.X;
                                                                float num124 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - (float)(npc.Height / 2) - vector17.Y;
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
                                                                if (npc.type == NPCType.N43_MAN_EATER)
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
                                                                    if (npc.type == NPCType.N49_CAVE_BAT || npc.type == NPCType.N51_JUNGLE_BAT || npc.type == NPCType.N60_HELLBAT)
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
                                                                    if (npc.type == NPCType.N48_HARPY)
                                                                    {
                                                                        npc.ai[0] += 1f;
                                                                        if (npc.ai[0] == 30f || npc.ai[0] == 60f || npc.ai[0] == 90f)
                                                                        {
                                                                            float num127 = 6f;
                                                                            Vector2 vector18 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                                            float num128 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width * 0.5f - vector18.X + (float)Main.rand.Next(-100, 101);
                                                                            float num129 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].Height * 0.5f - vector18.Y + (float)Main.rand.Next(-100, 101);
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
                                                                    if (npc.type == NPCType.N62_DEMON || npc.type == NPCType.N66_VOODOO_DEMON)
                                                                    {
                                                                        npc.ai[0] += 1f;
                                                                        if (npc.ai[0] == 20f || npc.ai[0] == 40f || npc.ai[0] == 60f || npc.ai[0] == 80f)
                                                                        {
                                                                            float num134 = 0.2f;
                                                                            Vector2 vector19 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                                            float num135 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width * 0.5f - vector19.X + (float)Main.rand.Next(-100, 101);
                                                                            float num136 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].Height * 0.5f - vector19.Y + (float)Main.rand.Next(-100, 101);
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
                                                                        if (npc.life > 0)
                                                                        {
                                                                            float num142 = (float)npc.life / (float)npc.lifeMax;
                                                                            num142 = num142 * 0.5f + 0.75f;
                                                                            if (num142 != npc.scale)
                                                                            {
                                                                                npc.Position.X = npc.Position.X + (float)(npc.Width / 2);
                                                                                npc.Position.Y = npc.Position.Y + (float)npc.Height;
                                                                                npc.scale = num142;
                                                                                npc.Width = (int)(98f * npc.scale);
                                                                                npc.Height = (int)(92f * npc.scale);
                                                                                npc.Position.X = npc.Position.X - (float)(npc.Width / 2);
                                                                                npc.Position.Y = npc.Position.Y - (float)npc.Height;
                                                                            }
                                                                            int num143 = (int)((double)npc.lifeMax * 0.05);
                                                                            if ((float)(npc.life + num143) < npc.ai[3])
                                                                            {
                                                                                npc.ai[3] = (float)npc.life;
                                                                                int num144 = Main.rand.Next(1, 4);
                                                                                for (int num145 = 0; num145 < num144; num145++)
                                                                                {
                                                                                    int x = (int)(npc.Position.X + (float)Main.rand.Next(npc.Width - 32));
                                                                                    int y = (int)(npc.Position.Y + (float)Main.rand.Next(npc.Height - 32));
                                                                                    int npcIndex = NPC.NewNPC(x, y, 1, 0);
                                                                                    Main.npcs[npcIndex] = Registries.NPC.Create(1);
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
                                                                                    if (npc.type == NPCType.N65_SHARK)
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
                                                                                    int num147 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
                                                                                    int num148 = (int)(npc.Position.Y + (float)(npc.Height / 2)) / 16;
                                                                                    if (Main.tile.At(num147, num148 - 1).Exists && Main.tile.At(num147, num148 - 1).Liquid > 128)
                                                                                    {
                                                                                        if (Main.tile.At(num147, num148 + 1).Active)
                                                                                        {
                                                                                            npc.ai[0] = -1f;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (Main.tile.At(num147, num148 + 2).Active)
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
                                                                                    if (npc.type == NPCType.N65_SHARK)
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
                                                                                        Rectangle rectangle3 = new Rectangle((int)Main.players[npc.target].Position.X, (int)Main.players[npc.target].Position.Y, Main.players[npc.target].Width, Main.players[npc.target].Height);
                                                                                        Rectangle rectangle4 = new Rectangle((int)npc.Position.X - 100, (int)npc.Position.Y - 100, npc.Width + 200, npc.Height + 200);
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
                                                                                        float num149 = Math.Abs(npc.Position.X + (float)(npc.Width / 2) - (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2)));
                                                                                        float num150 = Main.players[npc.target].Position.Y - (float)(npc.Height / 2);
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
                                                                                            Vector2 vector20 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                                                            float num153 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector20.X;
                                                                                            float num154 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector20.Y;
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
                                                                                        int num156 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
                                                                                        int num157 = (int)(npc.Position.Y + (float)(npc.Height / 2)) / 16;
                                                                                        if (Main.tile.At(num156, num157 - 1).Liquid > 128)
                                                                                        {
                                                                                            if (Main.tile.At(num156, num157 + 1).Active)
                                                                                            {
                                                                                                npc.ai[0] = -1f;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Main.tile.At(num156, num157 + 2).Active)
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
                                                                                        Vector2 vector21 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                                                                                        float num159 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector21.X;
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
                                                                                        if (flag15 && npc.ai[0] == 0f && Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
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
                                                                                            int num166 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
                                                                                            int num167 = (int)(npc.Position.X + (float)npc.Width) / 16;
                                                                                            int num168 = (int)(npc.Position.Y + (float)npc.Height) / 16;
                                                                                            bool flag16 = false;
                                                                                            if ((Main.tile.At(num165, num168).Active && Main.tileSolid[(int)Main.tile.At(num165, num168).Type]) || (Main.tile.At(num166, num168).Active && Main.tileSolid[(int)Main.tile.At(num166, num168).Type]) || (Main.tile.At(num167, num168).Active && Main.tileSolid[(int)Main.tile.At(num167, num168).Type]))
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
            if (this.type == NPCType.N01_BLUE_SLIME || this.type == NPCType.N16_MOTHER_SLIME || this.type == NPCType.N59_LAVA_SLIME)
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
            if (this.type == NPCType.N50_KING_SLIME)
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
            if (this.type == NPCType.N61_VULTURE)
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
            if (this.type == NPCType.N62_DEMON || this.type == NPCType.N66_VOODOO_DEMON)
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
            if (this.type == NPCType.N63_BLUE_JELLYFISH || this.type == NPCType.N64_PINK_JELLYFISH)
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
            if (this.type == NPCType.N02_DEMON_EYE || this.type == NPCType.N23_METEOR_HEAD)
            {
                if (this.type == NPCType.N02_DEMON_EYE)
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
            if (this.type == NPCType.N55_GOLDFISH || this.type == NPCType.N57_CORRUPT_GOLDFISH || this.type == NPCType.N58_PIRANHA)
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
            if (this.type == NPCType.N69_ANTLION)
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
            if (this.type == NPCType.N67_CRAB)
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
            if (this.type == NPCType.N65_SHARK)
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
            if (this.type == NPCType.N48_HARPY || this.type == NPCType.N49_CAVE_BAT || this.type == NPCType.N51_JUNGLE_BAT || this.type == NPCType.N60_HELLBAT)
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
            if (this.type == NPCType.N42_HORNET)
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
            if (this.type == NPCType.N43_MAN_EATER || this.type == NPCType.N56_SNATCHER)
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
            if (this.type == NPCType.N17_MERCHANT || this.type == NPCType.N18_NURSE || this.type == NPCType.N19_ARMS_DEALER || this.type == NPCType.N20_DRYAD || this.type == NPCType.N22_GUIDE || this.type == NPCType.N38_DEMOLITIONIST || this.type == NPCType.N26_GOBLIN_PEON || this.type == NPCType.N27_GOBLIN_THIEF || this.type == NPCType.N28_GOBLIN_WARRIOR || this.type == NPCType.N31_ANGRY_BONES || this.type == NPCType.N21_SKELETON || this.type == NPCType.N44_UNDEAD_MINER || this.type == NPCType.N54_CLOTHIER || this.type == NPCType.N37_OLD_MAN)
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
                    if (this.type == NPCType.N21_SKELETON || this.type == NPCType.N31_ANGRY_BONES || this.type == NPCType.N44_UNDEAD_MINER)
                    {
                        this.frame.Y = 0;
                    }
                }
            }
            else
            {
                if (this.type == NPCType.N03_ZOMBIE || this.type == NPCType.N52_DOCTOR_BONES || this.type == NPCType.N53_THE_GROOM)
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
                    if (this.type == NPCType.N46_BUNNY || this.type == NPCType.N47_CORRUPT_BUNNY)
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
                        if (this.type == NPCType.N04_EYE_OF_CTHULU)
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
                            if (this.type == NPCType.N05_SERVANT_OF_CTHULU)
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
                                if (this.type == NPCType.N06_EATER_OF_SOULS)
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
                                    if (this.type == NPCType.N24_FIRE_IMP)
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
                                        if (this.type == NPCType.N29_GOBLIN_SORCERER || this.type == NPCType.N32_DARK_CASTER || this.type == NPCType.N45_TIM)
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
            if (this.type == NPCType.N34_CURSED_SKULL)
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
                if (Main.players[i].Active && !Main.players[i].dead && (num == -1f || Math.Abs(Main.players[i].Position.X + (float)(Main.players[i].Width / 2) - this.Position.X + (float)(this.Width / 2)) + Math.Abs(Main.players[i].Position.Y + (float)(Main.players[i].Height / 2) - this.Position.Y + (float)(this.Height / 2)) < num))
                {
                    num = Math.Abs(Main.players[i].Position.X + (float)(Main.players[i].Width / 2) - this.Position.X + (float)(this.Width / 2)) + Math.Abs(Main.players[i].Position.Y + (float)(Main.players[i].Height / 2) - this.Position.Y + (float)(this.Height / 2));
                    this.target = i;
                }
            }
            if (this.target < 0 || this.target >= 255)
            {
                this.target = 0;
            }
            this.targetRect = new Rectangle((int)Main.players[this.target].Position.X, (int)Main.players[this.target].Position.Y, Main.players[this.target].Width, Main.players[this.target].Height);
            if (faceTarget)
            {
                this.direction = 1;
                if ((float)(this.targetRect.X + this.targetRect.Width / 2) < this.Position.X + (float)(this.Width / 2))
                {
                    this.direction = -1;
                }
                this.directionY = 1;
                if ((float)(this.targetRect.Y + this.targetRect.Height / 2) < this.Position.Y + (float)(this.Height / 2))
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
                if (this.type == NPCType.N08_DEVOURER_BODY || this.type == NPCType.N09_DEVOURER_TAIL || this.type == NPCType.N11_GIANT_WORM_BODY || this.type == NPCType.N12_GIANT_WORM_TAIL || this.type == NPCType.N14_EATER_OF_WORLDS_BODY || this.type == NPCType.N15_EATER_OF_WORLDS_TAIL || this.type == NPCType.N40_BONE_SERPENT_BODY || this.type == NPCType.N41_BONE_SERPENT_TAIL)
                {
                    return;
                }
                if (this.townNPC)
                {
                    if ((double)this.Position.Y < Main.worldSurface * 18.0)
                    {
                        Rectangle rectangle = new Rectangle((int)(this.Position.X + (float)(this.Width / 2) - (float)NPC.townRangeX), (int)(this.Position.Y + (float)(this.Height / 2) - (float)NPC.townRangeY), NPC.townRangeX * 2, NPC.townRangeY * 2);
                        for (int i = 0; i < 255; i++)
                        {
                            if (Main.players[i].Active && rectangle.Intersects(new Rectangle((int)Main.players[i].Position.X, (int)Main.players[i].Position.Y, Main.players[i].Width, Main.players[i].Height)))
                            {
                                Main.players[i].townNPCs += (int)NPC.npcSlots;
                            }
                        }
                    }
                    return;
                }
                bool flag = false;
                Rectangle rectangle2 = new Rectangle((int)(this.Position.X + (float)(this.Width / 2) - (float)NPC.activeRangeX), (int)(this.Position.Y + (float)(this.Height / 2) - (float)NPC.activeRangeY), NPC.activeRangeX * 2, NPC.activeRangeY * 2);
                Rectangle rectangle3 = new Rectangle((int)((double)(this.Position.X + (float)(this.Width / 2)) - (double)NPC.sWidth * 0.5 - (double)this.Width), (int)((double)(this.Position.Y + (float)(this.Height / 2)) - (double)NPC.sHeight * 0.5 - (double)this.Height), NPC.sWidth + this.Width * 2, NPC.sHeight + this.Height * 2);
                for (int j = 0; j < 255; j++)
                {
                    if (Main.players[j].Active)
                    {
                        if (rectangle2.Intersects(new Rectangle((int)Main.players[j].Position.X, (int)Main.players[j].Position.Y, Main.players[j].Width, Main.players[j].Height)))
                        {
                            flag = true;
                            if (this.type != NPCType.N25_BURNING_SPHERE && this.type != NPCType.N30_CHAOS_BALL && this.type != NPCType.N33_WATER_SPHERE)
                            {
                                Main.players[j].activeNPCs += (int)NPC.npcSlots;
                            }
                        }
                        if (rectangle3.Intersects(new Rectangle((int)Main.players[j].Position.X, (int)Main.players[j].Position.Y, Main.players[j].Width, Main.players[j].Height)))
                        {
                            this.timeLeft = NPC.ACTIVE_TIME;
                        }
                        if (this.type == NPCType.N07_DEVOURER_HEAD || this.type == NPCType.N10_GIANT_WORM_HEAD || this.type == NPCType.N13_EATER_OF_WORLDS_HEAD || this.type == NPCType.N39_BONE_SERPENT_HEAD)
                        {
                            flag = true;
                        }
                        if (this.boss || this.type == NPCType.N35_SKELETRON_HEAD || this.type == NPCType.N36_SKELETRON_HAND)
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
                        if (num6 >= Main.maxTilesX)
                        {
                            num6 = Main.maxTilesX - 1;
                        }
                        if (num7 < 0)
                        {
                            num7 = 0;
                        }
                        if (num8 >= Main.maxTilesY)
                        {
                            num8 = Main.maxTilesY - 1;
                        }
                        
                        if (num5 > num6)
                            num6 = Math.Min (num5 + NPC.spawnRangeX, Main.maxTilesX - 1);
                            
                        if (num7 > num8)
                            num8 = Math.Min (num7 + NPC.spawnRangeY, Main.maxTilesY - 1);
                         
                        int k = 0;
                        while (k < 50)
                        {
                            if (!(num5 < num6 && num7 < num8))
                            {
                                return; 
                            }
                            int num13 = Main.rand.Next(num5, num6);
                            int num14 = Main.rand.Next(num7, num8);
                            if (Main.tile.At(num13, num14).Active && Main.tileSolid[(int)Main.tile.At(num13, num14).Type])
                            {
                                goto IL_ACE;
                            }
                            if (!Main.wallHouse[(int)Main.tile.At(num13, num14).Wall])
                            {
                                if (!flag3 && (double)num14 < Main.worldSurface * 0.30000001192092896 && !flag5 && ((double)num13 < (double)Main.maxTilesX * 0.35 || (double)num13 > (double)Main.maxTilesX * 0.65))
                                {
                                    byte arg_986_0 = Main.tile.At(num13, num14).Type;
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
                                        if (Main.tile.At(num13, l).Active && Main.tileSolid[(int)Main.tile.At(num13, l).Type])
                                        {
                                            if (num13 < num9 || num13 > num10 || l < num11 || l > num12)
                                            {
                                                byte arg_9F4_0 = Main.tile.At(num13, l).Type;
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
                                if (num16 >= Main.maxTilesX)
                                {
                                    flag = false;
                                }
                                if (num17 < 0)
                                {
                                    flag = false;
                                }
                                if (num18 >= Main.maxTilesY)
                                {
                                    flag = false;
                                }
                                if (flag)
                                {
                                    for (int m = num15; m < num16; m++)
                                    {
                                        for (int n = num17; n < num18; n++)
                                        {
                                            if (Main.tile.At(m, n).Active && Main.tileSolid[(int)Main.tile.At(m, n).Type])
                                            {
                                                flag = false;
                                                break;
                                            }
                                            if (Main.tile.At(m, n).Lava)
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
                                Rectangle rectangle2 = new Rectangle((int)(Main.players[num19].Position.X + (float)(Main.players[num19].Width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.players[num19].Position.Y + (float)(Main.players[num19].Height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                if (rectangle.Intersects(rectangle2))
                                {
                                    flag = false;
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        if (Main.players[j].zoneDungeon && (!Main.tileDungeon[(int)Main.tile.At(num, num2).Type] || Main.tile.At(num, num2 - 1).Wall == 0))
                        {
                            flag = false;
                        }
                        if (Main.tile.At(num, num2 - 1).Liquid > 0 && Main.tile.At(num, num2 - 2).Liquid > 0 && !Main.tile.At(num, num2 - 1).Lava)
                        {
                            flag4 = true;
                        }
                    }
                    if (flag)
                    {
                        flag = false;
                        int num20 = (int)Main.tile.At(num, num2).Type;
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
                                                                        Main.npcs[npcIndex] = Registries.NPC.Create("Big Boned");
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Main.rand.Next(5) == 0)
                                                                        {
                                                                            Main.npcs[npcIndex] = Registries.NPC.Create("Short Bones");
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
                                                                                Main.npcs[npcIndex] = Registries.NPC.Create("Little Stinger");
                                                                            }
                                                                            else if (Main.rand.Next(4) == 0)
                                                                            {
                                                                                Main.npcs[npcIndex] = Registries.NPC.Create("Big Stinger");
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
                                                                                        Main.npcs[npcIndex] = Registries.NPC.Create("Little Eater");
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (Main.rand.Next(3) == 0)
                                                                                        {
                                                                                            Main.npcs[npcIndex] = Registries.NPC.Create("Big Eater");
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
                                                                                                                Main.npcs[npcIndex] = Registries.NPC.Create("Jungle Slime");
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (Main.rand.Next(3) == 0 || num22 < 200)
                                                                                                                {
                                                                                                                    Main.npcs[npcIndex] = Registries.NPC.Create("Green Slime", Main.npcs[npcIndex]);
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (Main.rand.Next(10) == 0 && num22 > 400)
                                                                                                                    {
                                                                                                                        Main.npcs[npcIndex] = Registries.NPC.Create("Purple Slime");
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
                                                                                                    Main.npcs[npcIndex] = Registries.NPC.Create("Yellow Slime");
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (Main.rand.Next(2) == 0)
                                                                                                    {
                                                                                                        Main.npcs[npcIndex] = Registries.NPC.Create("Blue Slime");
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        Main.npcs[npcIndex] = Registries.NPC.Create("Red Slime");
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
                                                                                                                Main.npcs[npcIndex] = Registries.NPC.Create("Jungle Slime");
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Main.npcs[npcIndex] = Registries.NPC.Create("Black Slime");
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
                        if (Main.npcs[npcIndex].type == NPCType.N01_BLUE_SLIME && Main.rand.Next(250) == 0)
                        {
                            Main.npcs[npcIndex] = Registries.NPC.Create("Pinky");
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
            if (num4 >= Main.maxTilesX)
            {
                num4 = Main.maxTilesX - 1;
            }
            if (num5 < 0)
            {
                num5 = 0;
            }
            if (num6 >= Main.maxTilesY)
            {
                num6 = Main.maxTilesY - 1;
            }
            for (int i = 0; i < 1000; i++)
            {
                int j = 0;
                while (j < 100)
                {
                    int num11 = Main.rand.Next(num3, num4);
                    int num12 = Main.rand.Next(num5, num6);
                    if (Main.tile.At(num11, num12).Active && Main.tileSolid[(int)Main.tile.At(num11, num12).Type])
                    {
                        goto IL_2E1;
                    }
                    if (Main.tile.At(num11, num12).Wall != 1)
                    {
                        int k = num12;
                        while (k < Main.maxTilesY)
                        {
                            if (Main.tile.At(num11, k).Active && Main.tileSolid[(int)Main.tile.At(num11, k).Type])
                            {
                                if (num11 < num7 || num11 > num8 || k < num9 || k > num10)
                                {
                                    byte arg_220_0 = Main.tile.At(num11, k).Type;
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
                        if (num14 >= Main.maxTilesX)
                        {
                            flag = false;
                        }
                        if (num15 < 0)
                        {
                            flag = false;
                        }
                        if (num16 >= Main.maxTilesY)
                        {
                            flag = false;
                        }
                        if (flag)
                        {
                            for (int l = num13; l < num14; l++)
                            {
                                for (int m = num15; m < num16; m++)
                                {
                                    if (Main.tile.At(l, m).Active && Main.tileSolid[(int)Main.tile.At(l, m).Type])
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
                            Rectangle rectangle2 = new Rectangle((int)(Main.players[n].Position.X + (float)(Main.players[n].Width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.players[n].Position.Y + (float)(Main.players[n].Height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
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
                NPCBossSummonEvent npcEvent = new NPCBossSummonEvent();
                npcEvent.BossType = Type;
                npcEvent.Sender = Main.players[playerIndex];
                Program.server.PluginManager.processHook(Hooks.NPC_BOSSSUMMON, npcEvent);
                if (npcEvent.Cancelled)
                {
                    return;
                }

                int npcIndex = NPC.NewNPC(num * 16 + 8, num2 * 16, Type, 1);
                Main.npcs[npcIndex].target = playerIndex;
                String str = Main.npcs[npcIndex].Name;
                if (Main.npcs[npcIndex].type == NPCType.N13_EATER_OF_WORLDS_HEAD)
                {
                    str = "Eater of Worlds";
                }
                if (Main.npcs[npcIndex].type == NPCType.N35_SKELETRON_HEAD)
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
                NPC npc = Registries.NPC.Create(type);
                //NPC oldNPC = Main.npcs[npcIndex];
                npc.Position.X = (float)(x - npc.Width / 2);
                npc.Position.Y = (float)(y - npc.Height);
                npc.Active = true;
                npc.timeLeft = (int)((double)NPC.ACTIVE_TIME * 1.25);
                npc.wet = Collision.WetCollision(npc.Position, npc.Width, npc.Height);

                if (!WorldGen.gen)
                {
                    NPCSpawnEvent npcEvent = new NPCSpawnEvent();
                    npcEvent.NPC = npc;
                    Sender sender = new Sender();
                    sender.Op = true;
                    npcEvent.Sender = sender;
                    Program.server.PluginManager.processHook(Hooks.NPC_SPAWN, npcEvent);
                    if (npcEvent.Cancelled)
                    {
                        return MAX_NPCS;
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
            npc = Registries.NPC.Create(newType);
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
                    if (this.townNPC && this.type != NPCType.N37_OLD_MAN)
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
                    Program.server.PluginManager.processHook(Plugin.Hooks.NPC_DEATH, Event);
                    if (Event.Cancelled)
                    {
                        return 0.0;
                    }

                    this.NPCLoot();
                    this.Active = false;
                    if (this.type == NPCType.N26_GOBLIN_PEON || this.type == NPCType.N27_GOBLIN_THIEF || this.type == NPCType.N28_GOBLIN_WARRIOR || this.type == NPCType.N29_GOBLIN_SORCERER)
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
            if (this.type == NPCType.N01_BLUE_SLIME || this.type == NPCType.N16_MOTHER_SLIME)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 23, Main.rand.Next(1, 3), false);
            }
            if (this.type == NPCType.N02_DEMON_EYE)
            {
                if (Main.rand.Next(3) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 38, 1, false);
                }
                else
                {
                    if (Main.rand.Next(100) == 0)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 236, 1, false);
                    }
                }
            }
            if (this.type == NPCType.N58_PIRANHA)
            {
                if (Main.rand.Next(500) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 263, 1, false);
                }
                else
                {
                    if (Main.rand.Next(40) == 0)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 118, 1, false);
                    }
                }
            }
            if (this.type == NPCType.N03_ZOMBIE && Main.rand.Next(50) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 216, 1, false);
            }
            if (this.type == NPCType.N66_VOODOO_DEMON)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 267, 1, false);
            }
            if (this.type == NPCType.N62_DEMON && Main.rand.Next(50) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 272, 1, false);
            }
            if (this.type == NPCType.N52_DOCTOR_BONES)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 251, 1, false);
            }
            if (this.type == NPCType.N53_THE_GROOM)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 239, 1, false);
            }
            if (this.type == NPCType.N54_CLOTHIER)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 260, 1, false);
            }
            if (this.type == NPCType.N55_GOLDFISH)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 261, 1, false);
            }
            if (this.type == NPCType.N69_ANTLION && Main.rand.Next(10) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 323, 1, false);
            }
            if (this.type == NPCType.N04_EYE_OF_CTHULU)
            {
                int stack = Main.rand.Next(30) + 20;
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 47, stack, false);
                stack = Main.rand.Next(20) + 10;
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 56, stack, false);
                stack = Main.rand.Next(20) + 10;
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 56, stack, false);
                stack = Main.rand.Next(20) + 10;
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 56, stack, false);
                stack = Main.rand.Next(3) + 1;
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 59, stack, false);
            }
            if (this.type == NPCType.N06_EATER_OF_SOULS && Main.rand.Next(3) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 68, 1, false);
            }
            if (this.type == NPCType.N07_DEVOURER_HEAD || this.type == NPCType.N08_DEVOURER_BODY || this.type == NPCType.N09_DEVOURER_TAIL)
            {
                if (Main.rand.Next(3) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 68, Main.rand.Next(1, 3), false);
                }
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 69, Main.rand.Next(3, 9), false);
            }
            if ((this.type == NPCType.N10_GIANT_WORM_HEAD || this.type == NPCType.N11_GIANT_WORM_BODY || this.type == NPCType.N12_GIANT_WORM_TAIL) && Main.rand.Next(500) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 215, 1, false);
            }
            if (this.type == NPCType.N47_CORRUPT_BUNNY && Main.rand.Next(75) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 243, 1, false);
            }
            if (this.type == NPCType.N39_BONE_SERPENT_HEAD || this.type == NPCType.N40_BONE_SERPENT_BODY || this.type == NPCType.N41_BONE_SERPENT_TAIL)
            {
                if (Main.rand.Next(100) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 220, 1, false);
                }
                else
                {
                    if (Main.rand.Next(100) == 0)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 218, 1, false);
                    }
                }
            }
            if (this.type == NPCType.N13_EATER_OF_WORLDS_HEAD || this.type == NPCType.N14_EATER_OF_WORLDS_BODY || this.type == NPCType.N15_EATER_OF_WORLDS_TAIL)
            {
                int stack2 = Main.rand.Next(1, 4);
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 86, stack2, false);
                if (Main.rand.Next(2) == 0)
                {
                    stack2 = Main.rand.Next(2, 6);
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 56, stack2, false);
                }
                if (this.boss)
                {
                    stack2 = Main.rand.Next(10, 30);
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 56, stack2, false);
                    stack2 = Main.rand.Next(10, 31);
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 56, stack2, false);
                }
            }
            if (this.type == NPCType.N63_BLUE_JELLYFISH || this.type == NPCType.N64_PINK_JELLYFISH)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 282, Main.rand.Next(1, 5), false);
            }
            if (this.type == NPCType.N21_SKELETON || this.type == NPCType.N44_UNDEAD_MINER)
            {
                if (Main.rand.Next(25) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 118, 1, false);
                }
                else
                {
                    if (this.type == NPCType.N44_UNDEAD_MINER)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 166, Main.rand.Next(1, 4), false);
                    }
                }
            }
            if (this.type == NPCType.N45_TIM)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 238, 1, false);
            }
            if (this.type == NPCType.N50_KING_SLIME)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, Main.rand.Next(256, 259), 1, false);
            }
            if (this.type == NPCType.N23_METEOR_HEAD && Main.rand.Next(50) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 116, 1, false);
            }
            if (this.type == NPCType.N24_FIRE_IMP)
            {
                if (Main.rand.Next(50) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 112, 1, false);
                }
                else
                {
                    if (Main.rand.Next(500) == 0)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 244, 1, false);
                    }
                }
            }
            if (this.type == NPCType.N31_ANGRY_BONES || this.type == NPCType.N32_DARK_CASTER)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 154, 1, false);
            }
            if (this.type == NPCType.N26_GOBLIN_PEON || this.type == NPCType.N27_GOBLIN_THIEF || this.type == NPCType.N28_GOBLIN_WARRIOR || this.type == NPCType.N29_GOBLIN_SORCERER)
            {
                if (Main.rand.Next(400) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 128, 1, false);
                }
                else
                {
                    if (Main.rand.Next(200) == 0)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 160, 1, false);
                    }
                    else
                    {
                        if (Main.rand.Next(2) == 0)
                        {
                            int stack3 = Main.rand.Next(1, 6);
                            Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 161, stack3, false);
                        }
                    }
                }
            }
            if (this.type == NPCType.N42_HORNET)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 209, 1, false);
            }
            if (this.type == NPCType.N43_MAN_EATER && Main.rand.Next(5) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 210, 1, false);
            }
            if (this.type == NPCType.N65_SHARK)
            {
                if (Main.rand.Next(50) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 268, 1, false);
                }
                else
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 319, 1, false);
                }
            }
            if (this.type == NPCType.N48_HARPY && Main.rand.Next(5) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 320, 1, false);
            }
            if (this.boss)
            {
                //boss kill
                int BossType = 0;
                if (this.type == NPCType.N04_EYE_OF_CTHULU)
                {
                    NPC.downedBoss1 = true;
                    BossType = 1;
                }
                if (this.type == NPCType.N13_EATER_OF_WORLDS_HEAD || this.type == NPCType.N14_EATER_OF_WORLDS_BODY || this.type == NPCType.N15_EATER_OF_WORLDS_TAIL)
                {
                    NPC.downedBoss2 = true;
                    this.Name = "Eater of Worlds";
                    BossType = 2;
                }
                if (this.type == NPCType.N35_SKELETRON_HEAD)
                {
                    NPC.downedBoss3 = true;
                    this.Name = "Skeletron";
                    BossType = 3;
                }
                int stack4 = Main.rand.Next(5, 16);
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 28, stack4, false);
                int num = Main.rand.Next(5) + 5;
                for (int i = 0; i < num; i++)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 58, 1, false);
                }
                
                NetMessage.SendData(25, -1, -1, this.Name + " has been defeated!", 255, 175f, 75f, 255f);

                NPCBossDeathEvent npcEvent = new NPCBossDeathEvent();
                npcEvent.Boss = BossType;
                Sender sender = new Sender();
                sender.Op = true;
                npcEvent.Sender = sender;
                Program.server.PluginManager.processHook(Hooks.NPC_BOSSDEATH, npcEvent);
            }
            if (Main.rand.Next(7) == 0 && this.lifeMax > 1)
            {
                if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statMana < Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statManaMax)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 184, 1, false);
                }
                else
                {
                    if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statLife < Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statLifeMax)
                    {
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 58, 1, false);
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
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 74, num3, false);
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
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 73, num4, false);
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
                            Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 72, num5, false);
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
                            Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 71, num6, false);
                        }
                    }
                }
            }
        }

        public void HitEffect(int hitDirection = 0, double dmg = 10.0)
        {
            if (this.type == NPCType.N01_BLUE_SLIME || this.type == NPCType.N16_MOTHER_SLIME)
            {
                if (this.life > 0)
                {
                    int num = 0;
                    while ((double)num < dmg / (double)this.lifeMax * 100.0)
                    {
                        num++;
                    }
                }
                else
                {
                    if (this.type == NPCType.N16_MOTHER_SLIME)
                    {
                        int spawnedSlimes = Main.rand.Next(2) + 2;
                        for (int slimeNum = 0; slimeNum < spawnedSlimes; slimeNum++)
                        {
                            int npcIndex = NPC.NewNPC((int)(this.Position.X + (float)(this.Width / 2)), (int)(this.Position.Y + (float)this.Height), 1, 0);
                            Main.npcs[npcIndex] = Registries.NPC.Create("Baby Slime");
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
            else if (this.type == NPCType.N59_LAVA_SLIME || this.type == NPCType.N60_HELLBAT)
            {
                if (this.life > 0)
                {
                    return;
                }
                if (this.type == NPCType.N59_LAVA_SLIME)
                {
                    int num5 = (int)(this.Position.X + (float)(this.Width / 2)) / 16;
                    int num6 = (int)(this.Position.Y + (float)(this.Height / 2)) / 16;
                    Main.tile.At(num5, num6).SetLava (true);
                    if (Main.tile.At(num5, num6).Liquid < 200)
                    {
                        Main.tile.At(num5, num6).SetLiquid (200);
                    }
                    WorldGen.TileFrame(num5, num6, false, false);
                    return;
                }
            }
            else if (this.type == NPCType.N50_KING_SLIME)
            {
                if (this.life > 0)
                {
                    return;
                }
                int num8 = Main.rand.Next(4) + 4;
                for (int m = 0; m < num8; m++)
                {
                    int x = (int)(this.Position.X + (float)Main.rand.Next(this.Width - 32));
                    int y = (int)(this.Position.Y + (float)Main.rand.Next(this.Height - 32));
                    int npcIndex = NPC.NewNPC(x, y, 1, 0);
                    Main.npcs[npcIndex] = Registries.NPC.Create(1);
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
            else if (this.type == NPCType.N49_CAVE_BAT || this.type == NPCType.N51_JUNGLE_BAT)
            {
                return;
            }
            else if (this.type == NPCType.N46_BUNNY || this.type == NPCType.N55_GOLDFISH || this.type == NPCType.N67_CRAB)
            {
                if (this.life > 0)
                {
                    return;
                }
                if (this.type == NPCType.N46_BUNNY)
                {
                    return;
                }
                if (this.type == NPCType.N67_CRAB)
                {
                    return;
                }
            }
            else if (this.type == NPCType.N47_CORRUPT_BUNNY || this.type == NPCType.N57_CORRUPT_GOLDFISH || this.type == NPCType.N58_PIRANHA)
            {
                return;
            }
            else if (this.type == NPCType.N02_DEMON_EYE)
            {
                return;
            }
            else if (this.type == NPCType.N69_ANTLION)
            {
                return;
            }
            else if (this.type == NPCType.N61_VULTURE)
            {
                return;
            }
            else if (this.type == NPCType.N65_SHARK)
            {
                return;
            }
            else if (this.type == NPCType.N03_ZOMBIE || this.type == NPCType.N52_DOCTOR_BONES || this.type == NPCType.N53_THE_GROOM)
            {
                return;
            }
            else if (this.type == NPCType.N04_EYE_OF_CTHULU)
            {
                return;
            }
            else if (this.type == NPCType.N05_SERVANT_OF_CTHULU)
            {
                return;
            }
            else if (this.type == NPCType.N06_EATER_OF_SOULS)
            {
                return;
            }
            else if (this.type == NPCType.N07_DEVOURER_HEAD || this.type == NPCType.N08_DEVOURER_BODY || this.type == NPCType.N09_DEVOURER_TAIL)
            {
                return;
            }
            else if (this.type == NPCType.N10_GIANT_WORM_HEAD || this.type == NPCType.N11_GIANT_WORM_BODY || this.type == NPCType.N12_GIANT_WORM_TAIL)
            {
                return;
            }
            else if (this.type == NPCType.N13_EATER_OF_WORLDS_HEAD || this.type == NPCType.N14_EATER_OF_WORLDS_BODY || this.type == NPCType.N15_EATER_OF_WORLDS_TAIL)
            {
                return;
            }
            else if (this.type == NPCType.N17_MERCHANT)
            {
                if (this.life > 0)
                {
                    return;
                }
                return;
            }
            else if (this.type == NPCType.N22_GUIDE)
            {
                return;
            }
            else if (this.type == NPCType.N37_OLD_MAN || this.type == NPCType.N54_CLOTHIER)
            {
                return;
            }
            else if (this.type == NPCType.N18_NURSE)
            {
                return;
            }
            else if (this.type == NPCType.N19_ARMS_DEALER)
            {
                return;
            }
            else if (this.type == NPCType.N38_DEMOLITIONIST)
            {
                return;
            }
            else if (this.type == NPCType.N20_DRYAD)
            {
                return;
            }
            else if (this.type == NPCType.N21_SKELETON || this.type == NPCType.N31_ANGRY_BONES || this.type == NPCType.N32_DARK_CASTER || this.type == NPCType.N44_UNDEAD_MINER || this.type == NPCType.N45_TIM)
            {
                return;
            }
            else if (this.type == NPCType.N39_BONE_SERPENT_HEAD || this.type == NPCType.N40_BONE_SERPENT_BODY || this.type == NPCType.N41_BONE_SERPENT_TAIL)
            {
                return;
            }
            else if (this.type == NPCType.N34_CURSED_SKULL)
            {
                return;
            }
            else if (this.type == NPCType.N35_SKELETRON_HEAD || this.type == NPCType.N36_SKELETRON_HAND)
            {
                return;
            }
            else if (this.type == NPCType.N23_METEOR_HEAD)
            {
                return;
            }
            else if (this.type == NPCType.N24_FIRE_IMP)
            {
                return;
            }
            else if (this.type == NPCType.N25_BURNING_SPHERE)
            {
                
                return;
            }
            else if (this.type == NPCType.N33_WATER_SPHERE)
            {
                return;
            }
            else if (this.type == NPCType.N26_GOBLIN_PEON || this.type == NPCType.N27_GOBLIN_THIEF || this.type == NPCType.N28_GOBLIN_WARRIOR || this.type == NPCType.N29_GOBLIN_SORCERER)
            {
                return;
            }
            else if (this.type == NPCType.N30_CHAOS_BALL)
            {
                return;
            }
            else if (this.type == NPCType.N42_HORNET)
            {
                return;
            }
            else if (this.type == NPCType.N43_MAN_EATER || this.type == NPCType.N56_SNATCHER)
            {
                return;
            }
            else if (this.type == NPCType.N48_HARPY)
            {
                return;
            }
            else if (this.type == NPCType.N62_DEMON || this.type == NPCType.N66_VOODOO_DEMON)
            {
                if (this.life > 0)
                {
                    return;
                }
            }

            Color newColor2 = new Color(50, 120, 255, 100);
            if (this.type == NPCType.N64_PINK_JELLYFISH)
            {
                newColor2 = new Color(225, 70, 140, 100);
            }
            if (this.life > 0)
            {
                return;
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
                if (Main.npcs[i].Active && Main.npcs[i].type == NPCType.N35_SKELETRON_HEAD)
                {
                    flag = false;
                    break;
                }
            }
            for (int j = 0; j < MAX_NPCS; j++)
            {
                if (Main.npcs[j].Active && Main.npcs[j].type == NPCType.N37_OLD_MAN)
                {
                    flag2 = true;
                    Main.npcs[j].ai[3] = 1f;
                    vector = Main.npcs[j].Position;
                    num = Main.npcs[j].Width;
                    num2 = Main.npcs[j].Height;

                    NetMessage.SendData(23, -1, -1, "", j);
                }
            }
            if (flag && flag2)
            {
                int npcIndex = NPC.NewNPC((int)vector.X + num / 2, (int)vector.Y + num2 / 2, 35, 0);
                Main.npcs[npcIndex].netUpdate = true;
                NetMessage.SendData(25, -1, -1, "Skeletron has awoken!", 255, 175f, 75f, 255f);
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
                    if (npc.type == NPCType.N46_BUNNY)
                    {
                        Transform(i, 47);
                        npc = Main.npcs[i];
                    }
                    else if (npc.type == NPCType.N55_GOLDFISH)
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
                if (npc.friendly && npc.type != NPCType.N37_OLD_MAN)
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
                        Rectangle rectangle = new Rectangle((int)npc.Position.X, (int)npc.Position.Y, npc.Width, npc.Height);
                        for (int k = 0; k < MAX_NPCS; k++)
                        {
                            if (Main.npcs[k].Active && !Main.npcs[k].friendly && Main.npcs[k].damage > 0)
                            {
                                Rectangle rectangle2 = new Rectangle((int)Main.npcs[k].Position.X, (int)Main.npcs[k].Position.Y, Main.npcs[k].Width, Main.npcs[k].Height);
                                if (rectangle.Intersects(rectangle2))
                                {
                                    int num3 = Main.npcs[k].damage;
                                    int num4 = 6;
                                    int num5 = 1;
                                    if (Main.npcs[k].Position.X + (float)(Main.npcs[k].Width / 2) > npc.Position.X + (float)(npc.Width / 2))
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
                    bool flag = Collision.LavaCollision(npc.Position, npc.Width, npc.Height);
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
                    bool flag2 = Collision.WetCollision(npc.Position, npc.Width, npc.Height);
                    if (flag2)
                    {
                        if (!npc.wet && npc.wetCount == 0)
                        {
                            npc.wetCount = 10;
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
                        npc.Velocity = Collision.TileCollision(npc.Position, npc.Velocity, npc.Width, npc.Height, flag3, flag3);
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
                        npc.Velocity = Collision.TileCollision(npc.Position, npc.Velocity, npc.Width, npc.Height, flag3, flag3);
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
            if (this.type == NPCType.N25_BURNING_SPHERE || this.type == NPCType.N30_CHAOS_BALL || this.type == NPCType.N33_WATER_SPHERE)
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
                if (Main.npcs[i].type == NPCType.N17_MERCHANT)
                {
                    flag = true;
                }
                else
                {
                    if (Main.npcs[i].type == NPCType.N18_NURSE)
                    {
                        flag2 = true;
                    }
                    else
                    {
                        if (Main.npcs[i].type == NPCType.N19_ARMS_DEALER)
                        {
                            flag3 = true;
                        }
                        else
                        {
                            if (Main.npcs[i].type == NPCType.N20_DRYAD)
                            {
                                flag4 = true;
                            }
                            else
                            {
                                if (Main.npcs[i].type == NPCType.N37_OLD_MAN)
                                {
                                    flag5 = true;
                                }
                                else
                                {
                                    if (Main.npcs[i].type == NPCType.N38_DEMOLITIONIST)
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
            if (this.type == NPCType.N17_MERCHANT)
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
                if (this.type == NPCType.N18_NURSE)
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
                    if (this.type == NPCType.N19_ARMS_DEALER)
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
                        if (this.type == NPCType.N20_DRYAD)
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
                            if (this.type == NPCType.N22_GUIDE)
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
                                if (this.type == NPCType.N37_OLD_MAN)
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
                                    if (this.type == NPCType.N38_DEMOLITIONIST)
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
                                        if (this.type == NPCType.N54_CLOTHIER)
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
                
        public override object Clone()
        {
            NPC cloned = (NPC)base.MemberwiseClone();
            NPC.npcSlots = cloned.slots;
            cloned.frame = default(Rectangle);
            cloned.Width = (int)((float)cloned.Width * cloned.scale);
            cloned.Height = (int)((float)cloned.Height * cloned.scale);
            cloned.life = cloned.lifeMax;
            cloned.ai = new float [NPC.MAX_AI];
            Array.Copy (ai, cloned.ai, NPC.MAX_AI);
            cloned.immune = new int[256];
            Array.Copy (immune, cloned.immune, 256);
            return cloned;
        }
    }
}
