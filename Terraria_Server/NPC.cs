using System;
using System.Collections.Generic;

using Terraria_Server.Misc;
using Terraria_Server.Plugins;
using Terraria_Server.Commands;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;
using Terraria_Server.WorldMod;
using Terraria_Server.Logging;

namespace Terraria_Server
{
	/// <summary>
	/// Basic NPC class
	/// </summary>
    public class NPC : BaseEntity, ISender
    {
        internal delegate void NPCSpawn(int npcId);
        internal static event NPCSpawn NPCSpawnHandler;

		bool ISender.Op
		{
			get { return false; }
			set { }
		}
		
		void ISender.sendMessage (string a, int b, float c, float d, float e)
		{
		}
		
        private const int active_TIME = 750;
		/// <summary>
		/// Total allowable active NPCs.
		/// </summary>
        public const int MAX_NPCS = 200;
		/// <summary>
		/// Maximum AI chains
		/// </summary>
        public const int MAX_AI = 4;

        public bool justHit; ////////////////////TODO
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
		/// <summary>
		/// Number of slots NPC takes up when active
		/// </summary>
        public static float npcSlots = 1f;
        private static bool noSpawnCycle = false;
        public static int defaultSpawnRate = 600;
        public static int defaultMaxSpawns = 5;
        public static bool downedBoss1 = false;
        public static bool downedBoss2 = false;
        public static bool downedBoss3 = false;
        public static int spawnRate = NPC.defaultSpawnRate;
        public static int maxSpawns = NPC.defaultMaxSpawns;

		/// <summary>
		/// NPCType enum value
		/// </summary>
        public NPCType type { get; set; }

		/// <summary>
		/// Integer representation of NPCType enum
		/// </summary>
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
        
        public string DisplayName;

        public bool behindTiles;
        public bool boss;
        public bool collideX;
        public bool collideY;
        public int direction;
        public double frameCounter;
		/// <summary>
		/// Whether an NPC is friendly or not
		/// </summary>
        public bool friendly;
		/// <summary>
		/// Whether a friendly NPC currently has a home or not.
		/// </summary>
        public bool homeless;
		/// <summary>
		/// X coordinate of their home tile
		/// </summary>
        public int homeTileX;
		/// <summary>
		/// Y coordinate of their home tile
		/// </summary>
        public int homeTileY;
		/// <summary>
		/// Whether NPC is immune to lava
		/// </summary>
        public bool lavaImmune;
		/// <summary>
		/// Whether NPC is currently in lava
		/// </summary>
        public bool lavaWet;
		/// <summary>
		/// Whether the NPC needs to send an update to clients
		/// </summary>
        public bool netUpdate = true;
        public bool netUpdate2 = false;
		/// <summary>
		/// Whether NPC is affected by gravity
		/// </summary>
        public bool noGravity;
		/// <summary>
		/// Whether NPC is affected by tile collisions
		/// </summary>
        public bool noTileCollide;
        [DontClone] public int oldDirection;
        [DontClone] public int oldTarget;
        public float rotation;
        public int spriteDirection;
		/// <summary>
		/// ID of player target
		/// </summary>
        public int target;
		/// <summary>
		/// Specific rectangle the NPC is targeting
		/// </summary>
        public Rectangle targetRect;
        public int timeLeft;
		/// <summary>
		/// Whether NPC is a resident NPC or not (requires a home)
		/// </summary>
        public bool townNPC;
        public float value;
		/// <summary>
		/// Whether NPC is currently in water
		/// </summary>
        public bool wet;
        public byte wetCount;

		/// <summary>
		/// AI chain array
		/// </summary>
        public int aiAction;
        public bool closeDoor;
        public int directionY = 1;
        public int doorX;
        public int doorY;
        public Rectangle frame;
        public int friendlyRegen;
        public int oldDirectionY;
        public int soundDelay;

		[DontClone] public Vector2 Velocity;
		[DontClone] public Vector2 oldPosition;
		[DontClone] public Vector2 oldVelocity;
        
		[DeepClone] public float[] ai = new float[NPC.MAX_AI];
		[DeepClone] public float[] localAI = new float[NPC.MAX_AI];
		[DeepClone] public ushort[] immune = new ushort[256];
		[DeepClone] public int[] buffType = new int[5];
		[DeepClone] public int[] buffTime = new int[5];
		[DeepClone] public bool[] buffImmune = new bool[Main.MAX_BUFFS];
		
		public bool PoisonImmunity
		{
			get { return buffImmune[20]; }
			set { buffImmune[20] = value; }
		}
		
		public bool BurningImmunity
		{
			get { return buffImmune[24]; }
			set { buffImmune[24] = value; }
		}
		
		public bool ConfusionImmunity
		{
			get { return buffImmune[31]; }
			set { buffImmune[31] = value; }
		}
		
		public bool CurseImmunity
		{
			get { return buffImmune[39]; }
			set { buffImmune[39] = value; }
		}
		
		// for deserializing only
		
		public string Inherits
		{
			get { return ""; }
			set
			{
				if (value == "") return;
				
				int i;
				if (Int32.TryParse (value, out i))
				{
					var saveName = Name;
					Registries.NPC.SetDefaults (this, i);
					Name = saveName;
					//Logging.ProgramLog.Debug.Log ("{0}({1}) is inheriting {2}", Name, Type, i);
				}
				else
				{
					var saveName = Name;
					Registries.NPC.SetDefaults (this, value);
					Name = saveName;
					//Logging.ProgramLog.Debug.Log ("{0}({1}) is inheriting {2}", Name, Type, value);
				}
			}
		}
		
		public string ScaleDamage
		{
			get { return ""; }
			set
			{
				damage = (int) (damage * scale);
			}
		}
		
		public string ScaleDefense
		{
			get { return ""; }
			set
			{
				defense = (int) (defense * scale);
			}
		}
		
		public string ScaleLifeMax
		{
			get { return ""; }
			set
			{
				lifeMax = (int) (lifeMax * scale);
			}
			
		}
		
		public string ScaleValue
		{
			get { return ""; }
			set
			{
				this.value *= scale;
			}
		}
		
		public string ScaleSlots
		{
			get { return ""; }
			set
			{
				this.slots *= scale;
			}
		}
		
		public string ScaleKnockBackResist
		{
			get { return ""; }
			set
			{
				this.knockBackResist *= 2f - scale;
			}
		}
		
		private bool scaleOverrideAdjustment; //FIXME: ugly
		public string ScaleOverrideAdjustment
		{
			get { return ""; }
			set
			{
				scaleOverrideAdjustment = true;
			}
		}
		
        public int lifeRegen;
        public int lifeRegenCount;
        public bool poisoned;
        public bool onFire;
        public bool onFire2; //cursed flames
        public bool confused;
        public bool dontTakeDamage;
        
        public int defDamage;
        public int defDefense;
        //public System.Collections.BitArray buffImmune = new System.Collections.BitArray (27);
		/// <summary>
		/// Index number for Main.npcs[]
		/// </summary>
		[DontClone] public int whoAmI;
		
		/// <summary>
		/// NPC Constructor.  Sets many defaults
		/// </summary>
        public NPC()
        {
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
            timeLeft = NPC.active_TIME;
            netUpdate = true;
            buffImmune[31] = true; //confusion

            LoadAIFunctions();
        }

		/// <summary>
		/// Movement checks
		/// </summary>
		/// <param name="index">Main.npcs[] index number</param>
/*        public void AI(int index)
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
                                    if (npc.type == NPCType.N31_ANGRY_BONES)
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
                                    WorldModify.KillTile(num4, num5 - 1, true, false, false);
                                    if (flag3)
                                    {
                                        if (npc.type == NPCType.N26_GOBLIN_PEON)
                                        {
                                            WorldModify.KillTile(num4, num5 - 1, false, false, false);

                                            NetMessage.SendData(17, -1, -1, "", 0, (float)num4, (float)(num5 - 1), 0f, 0);
                                            return;
                                        }
                                        else
                                        {
                                            bool flag4 = WorldModify.OpenDoor(num4, num5, npc.direction, npc.closeDoor, DoorOpener.NPC);
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
												NPC.Transform(index, 13);// npc = Registries.NPC.Create(13);
                                                //Main.npcs[index] = npc;

												Main.npcs[index].life = num46;
												if (Main.npcs[index].life > Main.npcs[index].lifeMax)
                                                {
                                                    npc.life = npc.lifeMax;
                                                }
												Main.npcs[index].ai[0] = num47;
												//Main.npcs[index].TargetClosest(true);
												//Main.npcs[index].netUpdate = true;
												//Main.npcs[index].whoAmI = num45;
                                            }
                                            if (npc.type == NPCType.N14_EATER_OF_WORLDS_BODY && !Main.npcs[(int)npc.ai[0]].Active)
                                            {
                                                int num48 = npc.life;
                                                int num49 = npc.whoAmI;
                                                float num50 = npc.ai[1];
												NPC.Transform(index, npc.Type);
                                                //npc = Registries.NPC.Create(npc.Type);
                                                //Main.npcs[index] = npc;
												Main.npcs[index].life = num48;
												if (Main.npcs[index].life > Main.npcs[index].lifeMax)
                                                {
													Main.npcs[index].life = Main.npcs[index].lifeMax;
                                                }
												Main.npcs[index].ai[1] = num50;
                                                //npc.TargetClosest(true);
                                                //npc.netUpdate = true;
                                                //npc.whoAmI = num49;
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
                                                            WorldModify.KillTile(m, n, true, true, false);
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
                                                        WorldModify.QuickFindHome(npc.whoAmI);
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
                                                        bool flag9 = WorldModify.CloseDoor(npc.doorX, npc.doorY, false, DoorOpener.NPC);
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
                                                            bool flag10 = WorldModify.OpenDoor(num68, num69 - 2, npc.direction, npc.closeDoor, DoorOpener.NPC);
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
                                                            if (WorldModify.OpenDoor(num68, num69 - 2, -npc.direction, npc.closeDoor, DoorOpener.NPC))
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
                                                                            ProjectileType num132 = ProjectileType.N38_HARPY_FEATHER;
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
                                                                            ProjectileType num139 = ProjectileType.N44_DEMON_SICKLE;
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
                                                                                        if (Main.tile.At(num156, num157 - 1).Exists && Main.tile.At(num156, num157 - 1).Liquid > 128)
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
/*        */
		

        public void AI(int index)
        {
            NPC npc = Main.npcs[index];
            int aiStyle = npc.aiStyle;

            if (AIFunctions.ContainsKey(aiStyle))
            {
                bool flag = false;
                // TODO: Shouldn't 'this' and npc refer to the same thing? - CM
                if (!Main.dayTime || this.life != this.lifeMax || (double)this.Position.Y > Main.worldSurface * 16.0)
                {
                    flag = true;
                }

                // Perform AI
                AIFunctions[aiStyle](npc, flag);
            }
            else
            {
                // ???
                int arg_CD95_0 = aiStyle;
            }
        }

		public static bool NearSpikeBall (int x, int y)
		{
			Rectangle rectangle = new Rectangle(x * 16 - 200, y * 16 - 200, 400, 400);
			for (int i = 0; i < MAX_NPCS; i++)
			{
				var npc = Main.npcs[i];
				if (npc.Active && npc.aiStyle == 20)
				{
					Rectangle rectangle2 = new Rectangle((int)npc.ai[1], (int)npc.ai[2], 20, 20);
					if (rectangle.Intersects(rectangle2))
					{
						return true;
					}
				}
			}
			return false;
		}

        public void AddBuff(int type, int time, bool quiet = false)
        {
            if (this.buffImmune[type])
            {
                return;
            }
            if (!quiet)
            {
                NetMessage.SendData(54, -1, -1, "", this.whoAmI, 0f, 0f, 0f, 0);
            }
            int num = -1;
            for (int i = 0; i < 5; i++)
            {
                if (this.buffType[i] == type)
                {
                    if (this.buffTime[i] < time)
                    {
                        this.buffTime[i] = time;
                    }
                    return;
                }
            }
            while (num == -1)
            {
                int buffId = -1;
                for (int j = 0; j < 5; j++)
                {
                    if (!Main.debuff[this.buffType[j]])
                    {
                        buffId = j;
                        break;
                    }
                }
                if (buffId == -1)
                {
                    return;
                }
                for (int k = buffId; k < 5; k++)
                {
                    if (this.buffType[k] == 0)
                    {
                        num = k;
                        break;
                    }
                }
                if (num == -1)
                {
                    this.DelBuff(buffId);
                }
            }
            this.buffType[num] = type;
            this.buffTime[num] = time;
        }

        public void DelBuff(int b)
        {
            this.buffTime[b] = 0;
            this.buffType[b] = 0;
            for (int i = 0; i < 4; i++)
            {
                if (this.buffTime[i] == 0 || this.buffType[i] == 0)
                {
                    for (int j = i + 1; j < 5; j++)
                    {
                        this.buffTime[j - 1] = this.buffTime[j];
                        this.buffType[j - 1] = this.buffType[j];
                        this.buffTime[j] = 0;
                        this.buffType[j] = 0;
                    }
                }
            }
            
            NetMessage.SendData(54, -1, -1, "", this.whoAmI, 0f, 0f, 0f, 0);
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
                else if (this.Velocity.Y > 0f)
                {
                    num2 = 3;
                }
                else if (this.Velocity.X != 0f)
                {
                    num2 = 1;
                }
                else
                {
                    num2 = 0;
                }
            }
            else if (this.aiAction == 1)
            {
                num2 = 4;
            }
            if (this.type == NPCType.N01_BLUE_SLIME || this.type == NPCType.N16_MOTHER_SLIME || this.type == NPCType.N59_LAVA_SLIME || this.type == NPCType.N71_DUNGEON_SLIME)
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
                else if (this.frameCounter < 12.0)
                {
                    this.frame.Y = num;
                }
                else if (this.frameCounter < 18.0)
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
                    else if (this.frameCounter < 12.0)
                    {
                        this.frame.Y = num;
                    }
                    else if (this.frameCounter < 18.0)
                    {
                        this.frame.Y = num * 2;
                    }
                    else if (this.frameCounter < 24.0)
                    {
                        this.frame.Y = num * 3;
                    }
                    else
                    {
                        this.frameCounter = 0.0;
                    }
                }
                else
                {
                    if (this.frameCounter < 6.0)
                    {
                        this.frame.Y = num * 4;
                    }
                    else if (this.frameCounter < 12.0)
                    {
                        this.frame.Y = num * 5;
                    }
                    else
                    {
                        this.frameCounter = 0.0;
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
			if (this.type == NPCType.N72_BLAZING_WHEEL)
			{
				this.frameCounter += 1.0;
				if (this.frameCounter >= 3.0)
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
                    else if (this.frameCounter < 12.0)
                    {
                        this.frame.Y = num;
                    }
                    else if (this.frameCounter < 18.0)
                    {
                        this.frame.Y = num * 2;
                    }
                    else if (this.frameCounter < 24.0)
                    {
                        this.frame.Y = num * 3;
                    }
                    else
                    {
                        this.frameCounter = 0.0;
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
                else if (this.frameCounter < 4.0)
                {
                    this.frame.Y = num;
                }
                else if (this.frameCounter < 6.0)
                {
                    this.frame.Y = num * 2;
                }
                else if (this.frameCounter < 8.0)
                {
                    this.frame.Y = num;
                }
                else
                {
                    this.frameCounter = 0.0;
                }
            }
            if (this.type == NPCType.N43_MAN_EATER || this.type == NPCType.N56_SNATCHER)
            {
                this.frameCounter += 1.0;
                if (this.frameCounter < 6.0)
                {
                    this.frame.Y = 0;
                }
                else if (this.frameCounter < 12.0)
                {
                    this.frame.Y = num;
                }
                else if (this.frameCounter < 18.0)
                {
                    this.frame.Y = num * 2;
                }
                else if (this.frameCounter < 24.0)
                {
                    this.frame.Y = num;
                }
                if (this.frameCounter == 23.0)
                {
                    this.frameCounter = 0.0;
                }
            }
            if (this.type == NPCType.N17_MERCHANT || this.type == NPCType.N18_NURSE || this.type == NPCType.N19_ARMS_DEALER || this.type == NPCType.N20_DRYAD || this.type == NPCType.N22_GUIDE || this.type == NPCType.N38_DEMOLITIONIST || this.type == NPCType.N26_GOBLIN_PEON || this.type == NPCType.N27_GOBLIN_THIEF || this.type == NPCType.N28_GOBLIN_WARRIOR || this.type == NPCType.N31_ANGRY_BONES || this.type == NPCType.N21_SKELETON || this.type == NPCType.N44_UNDEAD_MINER || this.type == NPCType.N54_CLOTHIER || this.type == NPCType.N37_OLD_MAN || this.type == NPCType.N73_GOBLIN_SCOUT)
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
            else if (this.type == NPCType.N03_ZOMBIE || this.type == NPCType.N52_DOCTOR_BONES || this.type == NPCType.N53_THE_GROOM)
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
                else if (this.Velocity.X == 0f)
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
                    else if (this.frameCounter < 16.0)
                    {
                        this.frame.Y = num;
                    }
                    else if (this.frameCounter < 24.0)
                    {
                        this.frame.Y = num * 2;
                    }
                    else if (this.frameCounter < 32.0)
                    {
                        this.frame.Y = num;
                    }
                    else
                    {
                        this.frameCounter = 0.0;
                    }
                }
            }
            else if (this.type == NPCType.N46_BUNNY || this.type == NPCType.N47_CORRUPT_BUNNY)
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
                else if (this.Velocity.Y < 0f)
                {
                    this.frameCounter = 0.0;
                    this.frame.Y = num * 4;
                }
                else if (this.Velocity.Y > 0f)
                {
                    this.frameCounter = 0.0;
                    this.frame.Y = num * 6;
                }
            }
            else if (this.type == NPCType.N04_EYE_OF_CTHULU)
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
            else if (this.type == NPCType.N05_SERVANT_OF_CTHULU)
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
            else if (this.type == NPCType.N06_EATER_OF_SOULS)
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
            else if (this.type == NPCType.N24_FIRE_IMP)
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
                    else if (this.frameCounter <= 8.0)
                    {
                        this.frame.Y = num * 5;
                    }
                    else if (this.frameCounter <= 12.0)
                    {
                        this.frame.Y = num * 6;
                    }
                    else if (this.frameCounter <= 16.0)
                    {
                        this.frame.Y = num * 7;
                    }
                    else if (this.frameCounter <= 20.0)
                    {
                        this.frame.Y = num * 8;
                    }
                    else
                    {
                        this.frame.Y = num * 9;
                        this.frameCounter = 100.0;
                    }
                }
                else
                {
                    this.frameCounter += 1.0;
                    if (this.frameCounter <= 4.0)
                    {
                        this.frame.Y = 0;
                    }
                    else if (this.frameCounter <= 8.0)
                    {
                        this.frame.Y = num;
                    }
                    else if (this.frameCounter <= 12.0)
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
                    else if (this.ai[1] > 0f)
                    {
                        this.frame.Y = this.frame.Y + num * 2;
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

		/// <summary>
		/// Chooses player for the NPC to target/follow
		/// </summary>
		/// <param name="faceTarget">Whether or not to face chosen target</param>
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

		/// <summary>
		/// Checks NPC's active status
		/// </summary>
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
                                Main.players[i].townNPCs += (int)this.slots;
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
                                Main.players[j].activeNPCs += (int)this.slots;
                            }
                        }
                        if (rectangle3.Intersects(new Rectangle((int)Main.players[j].Position.X, (int)Main.players[j].Position.Y, Main.players[j].Width, Main.players[j].Height)))
                        {
                            this.timeLeft = NPC.active_TIME;
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

		/// <summary>
		/// Spawns all NPCs that need to be
		/// </summary>
        public static void SpawnNPC()
        {
            if (NPC.noSpawnCycle)
            {
                NPC.noSpawnCycle = false;
                return;
            }

            if (Main.stopSpawns)
                return;

            bool flag = false;
            bool flag2 = false;
            int x = 0;
            int y = 0;
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
                        //NPC.spawnRate = (int)((float)NPC.spawnRate * 0.4f);
                        NPC.maxSpawns = (int)((float)NPC.maxSpawns * 2.0f);
                    }
                    else if ((double)Main.players[j].Position.Y > Main.rockLayer * 16.0 + (double)NPC.sHeight)
                    {
                        NPC.spawnRate = (int)((double)NPC.spawnRate * 0.4);
                        NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.9f);
                    }
                    else if ((double)Main.players[j].Position.Y > Main.worldSurface * 16.0 + (double)NPC.sHeight)
                    {
                        NPC.spawnRate = (int)((double)NPC.spawnRate * 0.5);
                        NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.7f);
                    }
                    else if (!Main.dayTime)
                    {
                        NPC.spawnRate = (int)((double)NPC.spawnRate * 0.6);
                        NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.3f);
                        if (Main.bloodMoon)
                        {
                            NPC.spawnRate = (int)((double)NPC.spawnRate * 0.3);
                            NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.8f);
                        }
                    }
                    if (Main.players[j].zoneDungeon)
                    {
                        NPC.spawnRate = (int)((double)NPC.defaultSpawnRate * 0.35);
                        NPC.maxSpawns = (int) (NPC.maxSpawns * 1.85);
                    }
                    else if (Main.players[j].zoneJungle)
                    {
                        NPC.spawnRate = (int)((double)NPC.spawnRate * 0.3);
                        NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.6f);
                    }
                    else if (Main.players[j].zoneEvil)
                    {
                        NPC.spawnRate = (int)((double)NPC.spawnRate * 0.65);
                        NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.3f);
                    }
                    else if (Main.players[j].zoneMeteor)
                    {
                        NPC.spawnRate = (int)((double)NPC.spawnRate * 0.4);
                        NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.1f);
                    }
                    if ((double)Main.players[j].activeNPCs < (double)NPC.maxSpawns * 0.2)
                    {
                        NPC.spawnRate = (int)((float)NPC.spawnRate * 0.6f);
                    }
                    else if ((double)Main.players[j].activeNPCs < (double)NPC.maxSpawns * 0.4)
                    {
                        NPC.spawnRate = (int)((float)NPC.spawnRate * 0.7f);
                    }
                    else if ((double)Main.players[j].activeNPCs < (double)NPC.maxSpawns * 0.6)
                    {
                        NPC.spawnRate = (int)((float)NPC.spawnRate * 0.8f);
                    }
                    else if ((double)Main.players[j].activeNPCs < (double)NPC.maxSpawns * 0.8)
                    {
                        NPC.spawnRate = (int)((float)NPC.spawnRate * 0.9f);
                    }
                    if ((double)(Main.players[j].Position.Y * 16f) > (Main.worldSurface + Main.rockLayer) / 2.0 || Main.players[j].zoneEvil)
                    {
                        if ((double)Main.players[j].activeNPCs < (double)NPC.maxSpawns * 0.2)
                        {
                            NPC.spawnRate = (int)((float)NPC.spawnRate * 0.7f);
                        }
                        else if ((double)Main.players[j].activeNPCs < (double)NPC.maxSpawns * 0.4)
                        {
                            NPC.spawnRate = (int)((float)NPC.spawnRate * 0.9f);
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
                        else if (Main.players[j].townNPCs == 2f)
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
                        else if (Main.players[j].townNPCs >= 3f)
                        {
                            flag5 = true;
                            NPC.maxSpawns = (int)((double)((float)NPC.maxSpawns) * 0.6);
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
                            num6 = Math.Min(num5 + NPC.spawnRangeX, Main.maxTilesX - 1);

                        if (num7 > num8)
                            num8 = Math.Min(num7 + NPC.spawnRangeY, Main.maxTilesY - 1);

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
                                    x = num13;
                                    y = num14;
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
                                                x = num13;
                                                y = l;
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
                                int num15 = x - NPC.spawnSpaceX / 2;
                                int num16 = x + NPC.spawnSpaceX / 2;
                                int num17 = y - NPC.spawnSpaceY;
                                int num18 = y;
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
                        Rectangle rectangle = new Rectangle(x * 16, y * 16, 16, 16);
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
                        if (Main.players[j].zoneDungeon && (!Main.tileDungeon[(int)Main.tile.At(x, y).Type] || Main.tile.At(x, y - 1).Wall == 0))
                        {
                            flag = false;
                        }
                        if (Main.tile.At(x, y - 1).Liquid > 0 && Main.tile.At(x, y - 2).Liquid > 0 && !Main.tile.At(x, y - 1).Lava)
                        {
                            flag4 = true;
                        }
                    }
                    if (flag)
                    {
                        flag = false;
                        int num20 = (int)Main.tile.At(x, y).Type;
                        int npcIndex = MAX_NPCS;
                        if (flag2)
                        {
                            NPC.NewNPC(x * 16 + 8, y * 16, 48, 0);
                        }
                        else if (flag3)
                        {
                            if (Main.rand.Next(9) == 0)
                            {
                                NPC.NewNPC(x * 16 + 8, y * 16, 29, 0);
                            }
                            else if (Main.rand.Next(5) == 0)
                            {
                                NPC.NewNPC(x * 16 + 8, y * 16, 26, 0);
                            }
                            else if (Main.rand.Next(3) == 0)
                            {
                                NPC.NewNPC(x * 16 + 8, y * 16, 27, 0);
                            }
                            else
                            {
                                NPC.NewNPC(x * 16 + 8, y * 16, 28, 0);
                            }
                        }
                        else if (flag4 && (x < 250 || x > Main.maxTilesX - 250) && num20 == 53 && (double)y < Main.rockLayer)
                        {
                            if (Main.rand.Next(8) == 0)
                            {
                                NPC.NewNPC(x * 16 + 8, y * 16, 65, 0);
                            }
                            if (Main.rand.Next(3) == 0)
                            {
                                NPC.NewNPC(x * 16 + 8, y * 16, 67, 0);
                            }
                            else
                            {
                                NPC.NewNPC(x * 16 + 8, y * 16, 64, 0);
                            }
                        }
                        else if (flag4 && (((double)y > Main.rockLayer && Main.rand.Next(2) == 0) || num20 == 60))
                        {
                            NPC.NewNPC(x * 16 + 8, y * 16, 58, 0);
                        }
                        else if (flag4 && (double)y > Main.worldSurface && Main.rand.Next(3) == 0)
                        {
                            NPC.NewNPC(x * 16 + 8, y * 16, 63, 0);
                        }
                        else if (flag4 && Main.rand.Next(4) == 0)
                        {
                            if (Main.players[j].zoneEvil)
                            {
                                NPC.NewNPC(x * 16 + 8, y * 16, 57, 0);
                            }
                            else
                            {
                                NPC.NewNPC(x * 16 + 8, y * 16, 55, 0);
                            }
                        }
                        else if (flag5)
                        {
                            if (flag4)
                            {
                                NPC.NewNPC(x * 16 + 8, y * 16, 55, 0);
                            }
                            else
                            {
                                if (num20 != 2)
                                {
                                    return;
                                }
                                NPC.NewNPC(x * 16 + 8, y * 16, 46, 0);
                            }
                        }
                        else if (Main.players[j].zoneDungeon)
                        {
                            if (!NPC.downedBoss3)
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 68, 0);
                            }
                            else if (Main.rand.Next(43) == 0)
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 71, 0);
                            }
                            else if (Main.rand.Next(3) == 0 && !NPC.NearSpikeBall(x, y))
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 70, 0);
                            }
                            else if (Main.rand.Next(5) == 0)
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 72, 0);
                            }
                            else if (Main.rand.Next(7) == 0)
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 34, 0);
                            }
                            else if (Main.rand.Next(7) == 0)
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 32, 0);
                            }
                            else
                            {
                                string what = "Angry Bones";
                                if (Main.rand.Next(4) == 0)
                                    what = "Big Boned";
                                else if (Main.rand.Next(5) == 0)
                                    what = "Short Bones";
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, what, 0);
                            }
                        }
                        else if (Main.players[j].zoneMeteor)
                        {
                            npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 23, 0);
                        }
                        else if (Main.players[j].zoneEvil && Main.rand.Next(50) == 0)
                        {
                            npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 7, 1);
                        }
                        else if (num20 == 60 && Main.rand.Next(500) == 0 && !Main.dayTime)
                        {
                            npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 52, 0);
                        }
                        else if (num20 == 60 && (double)y > (Main.worldSurface + Main.rockLayer) / 2.0)
                        {
                            if (Main.rand.Next(3) == 0)
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 43, 0);
                                Main.npcs[npcIndex].ai[0] = (float)x;
                                Main.npcs[npcIndex].ai[1] = (float)y;
                                Main.npcs[npcIndex].netUpdate = true;
                            }
                            else
                            {
                                string what = "Hornet";
                                if (Main.rand.Next(4) == 0)
                                    what = "Little Stinger";
                                else if (Main.rand.Next(4) == 0)
                                    what = "Big Stinger";
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, what, 0);
                            }
                        }
                        else if (num20 == 60 && Main.rand.Next(4) == 0)
                        {
                            npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 51, 0);
                        }
                        else if (num20 == 60 && Main.rand.Next(8) == 0)
                        {
                            npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 56, 0);
                            Main.npcs[npcIndex].ai[0] = (float)x;
                            Main.npcs[npcIndex].ai[1] = (float)y;
                            Main.npcs[npcIndex].netUpdate = true;
                        }
                        else if ((num20 == 22 && Main.players[j].zoneEvil) || num20 == 23 || num20 == 25)
                        {
                            string what = "Eater of Souls";
                            if (Main.rand.Next(3) == 0)
                                what = "Little Eater";
                            else if (Main.rand.Next(3) == 0)
                                what = "Big Eater";
                            npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, what, 0);
                        }
                        else if ((double)y <= Main.worldSurface)
                        {
                            if (Main.dayTime)
                            {
                                int num22 = Math.Abs(x - Main.spawnTileX);
                                if (num22 < Main.maxTilesX / 3 && Main.rand.Next(10) == 0 && num20 == 2)
                                {
                                    NPC.NewNPC(x * 16 + 8, y * 16, 46, 0);
                                }
                                else if (num22 > Main.maxTilesX / 3 && num20 == 2 && Main.rand.Next(300) == 0 && !NPC.IsNPCSummoned(50))
                                {
                                    npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 50, 0);
                                }
                                else if (num20 == 53 && Main.rand.Next(5) == 0 && !flag4)
                                {
                                    npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 69, 0);
                                }
                                else if (num20 == 53 && !flag4)
                                {
                                    npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 61, 0);
                                }
                                else if (num22 > Main.maxTilesX / 3 && Main.rand.Next(20) == 0)
								{
									npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 73, 0);
								}
                                else
                                {
									string what = "Blue Slime";
									if (num20 == 60)
										what = "Jungle Slime";
									if (Main.rand.Next(3) == 0 || num22 < 200)
										what = "Green Slime";
									else if (Main.rand.Next(10) == 0 && num22 > 400)
										what = "Purple Slime";
									npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, what, 0);
                                }
                            }
                            else if (Main.rand.Next(6) == 0 || (Main.moonPhase == 4 && Main.rand.Next(2) == 0))
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 2, 0);
                            }
                            else if (Main.rand.Next(250) == 0 && Main.bloodMoon)
                            {
                                NPC.NewNPC(x * 16 + 8, y * 16, 53, 0);
                            }
                            else
                            {
                                NPC.NewNPC(x * 16 + 8, y * 16, 3, 0);
                            }
                        }
                        else if ((double)y <= Main.rockLayer)
                        {
                            if (Main.rand.Next(50) == 0)
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 10, 1);
                            }
                            else
                            {
                                string what = "Red Slime";
                                if (Main.rand.Next(5) == 0)
                                    what = "Yellow Slime";
                                else if (Main.rand.Next(2) == 0)
                                    what = "Blue Slime";
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, what, 0);
                            }
                        }
                        else if (y > Main.maxTilesY - 190)
                        {
                            if (Main.rand.Next(40) == 0 && !NPC.IsNPCSummoned(39))
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 39, 1);
                            }
                            else if (Main.rand.Next(14) == 0)
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 24, 0);
                            }
                            else if (Main.rand.Next(10) == 0)
                            {
                                if (Main.rand.Next(10) == 0)
                                {
                                    npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 66, 0);
                                }
                                else
                                {
                                    npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 62, 0);
                                }
                            }
                            else if (Main.rand.Next(3) == 0)
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 59, 0);
                            }
                            else
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 60, 0);
                            }
                        }
                        else if (Main.rand.Next(55) == 0)
                        {
                            npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 10, 1);
                        }
                        else if (Main.rand.Next(10) == 0)
                        {
                            npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 16, 0);
                        }
                        else if (Main.rand.Next(4) == 0)
                        {
                            string what = "Black Slime";
                            if (Main.players[j].zoneJungle)
                                what = "Jungle Slime";
                            npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, what, 0);
                        }
                        else if (Main.rand.Next(2) == 0)
                        {
                            if ((double)y > (Main.rockLayer + (double)Main.maxTilesY) / 2.0 && Main.rand.Next(700) == 0)
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 45, 0);
                            }
                            else if (Main.rand.Next(15) == 0)
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 44, 0);
                            }
                            else
                            {
                                npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 21, 0);
                            }
                        }
                        else if (Main.players[j].zoneJungle)
                        {
                            npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 51, 0);
                        }
                        else
                        {
                            npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, 49, 0);
                        }
                        if (Main.npcs[npcIndex].type == NPCType.N01_BLUE_SLIME && Main.rand.Next(250) == 0)
                        {
                            Main.npcs[npcIndex].Transform ("Pinky");
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
        
		/// <summary>
		/// Spawns specified NPC type on specified player
		/// </summary>
		/// <param name="player">Instance of player to spawn on</param>
		/// <param name="playerIndex">Index of player to spawn on</param>
		/// <param name="Type">Type of NPC to spawn</param>
        public static void SpawnOnPlayer(Player player, int playerIndex, int Type)
        {
            if (Main.stopSpawns)
                return;

            bool flag = false;
            int x = 0;
            int y = 0;
            int minX = (int)(player.Position.X / 16f) - NPC.spawnRangeX * 3;
            int maxX = (int)(player.Position.X / 16f) + NPC.spawnRangeX * 3;
            int minY = (int)(player.Position.Y / 16f) - NPC.spawnRangeY * 3;
            int maxY = (int)(player.Position.Y / 16f) + NPC.spawnRangeY * 3;
            int xLeft = (int)(player.Position.X / 16f) - NPC.safeRangeX;
            int xRight = (int)(player.Position.X / 16f) + NPC.safeRangeX;
            int yLeft = (int)(player.Position.Y / 16f) - NPC.safeRangeY;
            int yRight = (int)(player.Position.Y / 16f) + NPC.safeRangeY;
            if (minX < 0)
            {
                minX = 0;
            }
            if (maxX >= Main.maxTilesX)
            {
                maxX = Main.maxTilesX - 1;
            }
            if (minY < 0)
            {
                minY = 0;
            }
            if (maxY >= Main.maxTilesY)
            {
                maxY = Main.maxTilesY - 1;
            }
            for (int i = 0; i < MAX_NPCS; i++)
            {
                int j = 0;
                while (j < 100)
                {
                    int tileX = Main.rand.Next(minX, maxX);
                    int tileY = Main.rand.Next(minY, maxY);
                    if (Main.tile.At(tileX, tileY).Active && Main.tileSolid[(int)Main.tile.At(tileX, tileY).Type])
                    {
                        if (!flag && !flag)
                        {
                            j++;
                            continue;
                        }
                        break;
                    }
                    if (Main.tile.At(tileX, tileY).Wall != 1)
                    {
                        int yTile = tileY;
                        while (yTile < Main.maxTilesY)
                        {
                            if (Main.tile.At(tileX, yTile).Active && Main.tileSolid[(int)Main.tile.At(tileX, yTile).Type])
                            {
                                if (tileX < xLeft || tileX > xRight || yTile < yLeft || yTile > yRight)
                                {
                                    //byte arg_220_0 = Main.tile.At(tileX, y).Type;
                                    x = tileX;
                                    y = yTile;
                                    flag = true;
                                    break;
                                }
                                break;
                            }
                            else
                            {
                                yTile++;
                            }
                        }
                        if (!flag)
                        {
                            if (!flag && !flag)
                            {
                                j++;
                                continue;
                            }
                            break;
                        }
                        int spawnMinX = x - NPC.spawnSpaceX / 2;
                        int spawnMaxX = x + NPC.spawnSpaceX / 2;
                        int spawnMinY = y - NPC.spawnSpaceY;
                        int spawnMaxY = y;
                        if (spawnMinX < 0)
                        {
                            flag = false;
                        }
                        if (spawnMaxX >= Main.maxTilesX)
                        {
                            flag = false;
                        }
                        if (spawnMinY < 0)
                        {
                            flag = false;
                        }
                        if (spawnMaxY >= Main.maxTilesY)
                        {
                            flag = false;
                        }
                        if (flag)
                        {
                            for (int l = spawnMinX; l < spawnMaxX; l++)
                            {
                                for (int m = spawnMinY; m < spawnMaxY; m++)
                                {
                                    if (Main.tile.At(l, m).Active && Main.tileSolid[(int)Main.tile.At(l, m).Type])
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!flag && !flag)
                        {
                            j++;
                            continue;
                        }
                        break;
                    }                    
                }
                if (flag)
                {
                    Rectangle rectangle = new Rectangle(x * 16, y * 16, 16, 16);
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
                int npcIndex = NPC.NewNPC(x * 16 + 8, y * 16, Type, 1);
                Main.npcs[npcIndex].target = playerIndex;
                string str = Main.npcs[npcIndex].Name;
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
		
		static bool InvokeNpcCreationHook (int x, int y, string type, out NPC npc)
		{
			npc = null;
			
			if (!WorldModify.gen)
			{
				var ctx = new HookContext
				{
				};
				
				var args = new HookArgs.NpcCreation
				{
					X = x, Y = y,
					Name = type,
				};
				
				HookPoints.NpcCreation.Invoke (ref ctx, ref args);
				
				if (ctx.Result == HookResult.IGNORE)
					return false;
				
				npc = args.CreatedNpc;
			}
			
			return true;
		}
		
		/// <summary>
		/// Creates new instance of specified NPC at specified
		/// </summary>
		/// <param name="x">X coordinate to create at</param>
		/// <param name="y">Y coordinate to create at</param>
		/// <param name="type">Type of NPC to create</param>
		/// <param name="start">Index to start from looking for free index</param>
		/// <returns>Main.npcs[] index value</returns>
		public static int NewNPC (int x, int y, int type, int start = 0)
		{
			NPC hnpc;
			if (! InvokeNpcCreationHook (x, y, Registries.NPC.GetTemplate(type).Name, out hnpc))
				return MAX_NPCS;
			
			int id = FindNPCSlot (start);
			if (id >= 0)
			{
				var npc = hnpc ?? Registries.NPC.Create (type);
				id = NewNPC (x, y, npc, id);
				
				if (id >= 0 && type == 50)
				{
					//TODO: move elsewhere
					NetMessage.SendData(25, -1, -1, npc.Name + " has awoken!", 255, 175f, 75f, 255f);
				}

                if (NPCSpawnHandler != null)
                    NPCSpawnHandler.Invoke(id);
				
				return id;
			}
			return MAX_NPCS;
		}
		
		public static int NewNPC (int x, int y, string name, int start = 0)
		{
			NPC hnpc;
			if (! InvokeNpcCreationHook (x, y, name, out hnpc))
				return MAX_NPCS;
			
			int id = FindNPCSlot (start);
			if (id >= 0)
			{
                NewNPC(x, y, hnpc ?? Registries.NPC.Create(name), id);

                if (NPCSpawnHandler != null)
                    NPCSpawnHandler.Invoke(id);

				return id;
			}
			return MAX_NPCS;
		}
		
		public static int FindNPCSlot (int start)
		{
			for (int i = start; i < MAX_NPCS; i++)
			{
				if (!Main.npcs[i].Active)
				{
					return i;
				}
			}
			return -1;
		}

        public static int NewNPC(int x, int y, NPC npc, int npcIndex)
        {
            npc.Position.X = (float)(x - npc.Width / 2);
            npc.Position.Y = (float)(y - npc.Height);
            npc.Active = true;
            npc.timeLeft = (int)((double)NPC.active_TIME * 1.25);
            npc.wet = Collision.WetCollision(npc.Position, npc.Width, npc.Height);

            Main.npcs[npcIndex] = npc;
            
            return npcIndex;
        }
		
		public void SetDefaults (int type)
		{
            if (Main.stopSpawns)
                return;

			oldDirection = direction;
			oldTarget = target;
			Registries.NPC.SetDefaults (this, type);
			life = lifeMax;
			Width = (int) (Width * scale);
			Height = (int) (Height * scale);
			if (scaleOverrideAdjustment && (Height == 16 || Height == 32))
				Height += 1; //FIXME: this is really ugly
			defDamage = damage;
			defDefense = defense;
		}
		
		public void SetDefaults (string type)
        {
            if (Main.stopSpawns)
                return;

			oldDirection = direction;
			oldTarget = target;
			Registries.NPC.SetDefaults (this, type);
			life = lifeMax;
			Width = (int) (Width * scale);
			Height = (int) (Height * scale);
			if (scaleOverrideAdjustment && (Height == 16 || Height == 32))
				Height += 1; //FIXME: this is really ugly
			defDamage = damage;
			defDefense = defense;
		}
		
		/// <summary>
		/// Transforms specified NPC into specified type.
		/// Used currently for bunny/goldfish to evil bunny/goldfish and Eater of Worlds segmenting transformations
		/// </summary>
		/// <param name="newType"></param>
        public void Transform (int newType)
		{
			var v = Velocity;
			SetDefaults (newType);
			Velocity = v;
			
			if (Main.npcs[whoAmI] == this)
			{
				Active = true;
				TargetClosest(true);
				netUpdate = true;
				NetMessage.SendData (23, -1, -1, "", whoAmI);
			}
		}
        
		public void Transform (string newType)
		{
			var v = Velocity;
			SetDefaults (newType);
			Velocity = v;
			
			if (Main.npcs[whoAmI] == this)
			{
				Active = true;
				TargetClosest(true);
				netUpdate = true;
				NetMessage.SendData (23, -1, -1, "", whoAmI);
			}
		}

		/// <summary>
		/// Damages the NPC
        /// </summary>
        /// <param name="aggressor">Sender who struck the NPC</param>
        /// <param name="Damage">Damage to calculate</param>
        /// <param name="knockBack">Knockback amount</param>
        /// <param name="hitDirection">Direction of strike</param>
        /// <param name="crit">If the hit was critical</param>
		/// <returns>Amount of damage actually done</returns>
		public bool StrikeNPC (ISender aggressor, int Damage, float knockBack, int hitDirection, bool crit = false)
		{
			if (!this.Active || this.life <= 0)
			{
				return false;
			}
			
			var proj = aggressor as Projectile;
			
			var ctx = new HookContext
			{
				Sender = aggressor,
				Player = proj != null ? (proj.Creator as Player) : (aggressor as Player),
			};
			
			ctx.Connection = ctx.Player != null ? ctx.Player.Connection : null;
			
			var args = new HookArgs.NpcHurt
			{
				Victim = this,
				Damage = Damage,
				HitDirection = hitDirection,
				Knockback = knockBack,
				Critical = crit,
			};
			
			HookPoints.NpcHurt.Invoke (ref ctx, ref args);
			
			if (ctx.CheckForKick () || ctx.Result == HookResult.IGNORE)
				return false;
			
			StrikeNPCInternal (args.Damage, args.Knockback, args.HitDirection, args.Critical);
			
			return true;
		}
		
        public double StrikeNPCInternal (int Damage, float knockBack, int hitDirection, bool crit = false)
        {
            if (!this.Active || this.life <= 0)
            {
                return 0.0;
            }
            double damage = Main.CalculateDamage(Damage, this.defense);
            if (crit) damage *= 2.0;

            if (damage >= 1.0)
            {
                this.justHit = true;
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
                this.life -= (int)damage;
                if (knockBack > 0f && this.knockBackResist > 0f)
                {
					float vel = knockBack * this.knockBackResist;
					if (crit)
					{
						vel *= 1.4f;
					}
					if (damage * 10.0 < (double)this.lifeMax)
					{
						if (hitDirection < 0 && this.Velocity.X > -vel)
						{
							if (this.Velocity.X > 0f)
							{
								this.Velocity.X = this.Velocity.X - vel;
							}
							this.Velocity.X = this.Velocity.X - vel;
							if (this.Velocity.X < -vel)
							{
								this.Velocity.X = -vel;
							}
						}
						else
						{
							if (hitDirection > 0 && this.Velocity.X < vel)
							{
								if (this.Velocity.X < 0f)
								{
									this.Velocity.X = this.Velocity.X + vel;
								}
								this.Velocity.X = this.Velocity.X + vel;
								if (this.Velocity.X > vel)
								{
									this.Velocity.X = vel;
								}
							}
						}
						if (!this.noGravity)
						{
							vel *= -0.75f;
						}
						else
						{
							vel *= -0.5f;
						}
						if (this.Velocity.Y > vel)
						{
							this.Velocity.Y = this.Velocity.Y + vel;
							if (this.Velocity.Y < vel)
							{
								this.Velocity.Y = vel;
							}
						}
					}
					else
					{
						if (!this.noGravity)
						{
							this.Velocity.Y = -vel * 0.75f * this.knockBackResist;
						}
						else
						{
							this.Velocity.Y = -vel * 0.5f * this.knockBackResist;
						}
						this.Velocity.X = vel * (float)hitDirection * this.knockBackResist;
					}
                }
                this.HitEffect(hitDirection, damage);

                if (this.life <= 0)
                {
                    NPC.noSpawnCycle = true;
                    if (this.townNPC && this.type != NPCType.N37_OLD_MAN)
                    {
                        NetMessage.SendData(25, -1, -1, this.Name + " was slain...", 255, 255f, 25f, 25f);
                    }
                    if (this.townNPC && this.homeless && WorldModify.spawnNPC == this.Type)
                    {
                        WorldModify.spawnNPC = 0;
                    }

                    this.NPCLoot();
                    this.Active = false;
                    if (this.type == NPCType.N26_GOBLIN_PEON || this.type == NPCType.N27_GOBLIN_THIEF || this.type == NPCType.N28_GOBLIN_WARRIOR || this.type == NPCType.N29_GOBLIN_SORCERER)
                    {
                        Main.invasionSize--;
                    }
                }
                return damage;
            }
            return 0.0;
        }

		/// <summary>
		/// Drops loot from dead NPC
		/// </summary>
        public void NPCLoot()
        {
            if (this.type == NPCType.N01_BLUE_SLIME || this.type == NPCType.N16_MOTHER_SLIME)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 23, Main.rand.Next(1, 3), false);
            }
            else if (this.type == NPCType.N71_DUNGEON_SLIME)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 327, 1, false);
            }
            else if (this.type == NPCType.N02_DEMON_EYE)
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
            else if (this.type == NPCType.N58_PIRANHA)
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
            else if (this.type == NPCType.N03_ZOMBIE && Main.rand.Next(50) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 216, 1, false);
            }
            else if (this.type == NPCType.N66_VOODOO_DEMON)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 267, 1, false);
            }
            else if (this.type == NPCType.N62_DEMON && Main.rand.Next(50) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 272, 1, false);
            }
            else if (this.type == NPCType.N52_DOCTOR_BONES)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 251, 1, false);
            }
            else if (this.type == NPCType.N53_THE_GROOM)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 239, 1, false);
            }
            else if (this.type == NPCType.N54_CLOTHIER)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 260, 1, false);
            }
            else if (this.type == NPCType.N55_GOLDFISH)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 261, 1, false);
            }
            else if (this.type == NPCType.N69_ANTLION && Main.rand.Next(10) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 323, 1, false);
            }
            else if (this.type == NPCType.N73_GOBLIN_SCOUT)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 362, 1, false);
            }
            else if (this.type == NPCType.N04_EYE_OF_CTHULU)
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
            else if (this.type == NPCType.N06_EATER_OF_SOULS && Main.rand.Next(3) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 68, 1, false);
            }
            else if (this.type == NPCType.N07_DEVOURER_HEAD || this.type == NPCType.N08_DEVOURER_BODY || this.type == NPCType.N09_DEVOURER_TAIL)
            {
                if (Main.rand.Next(3) == 0)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 68, Main.rand.Next(1, 3), false);
                }
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 69, Main.rand.Next(3, 9), false);
            }
            else if ((this.type == NPCType.N10_GIANT_WORM_HEAD || this.type == NPCType.N11_GIANT_WORM_BODY || this.type == NPCType.N12_GIANT_WORM_TAIL) && Main.rand.Next(500) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 215, 1, false);
            }
            else if (this.type == NPCType.N47_CORRUPT_BUNNY && Main.rand.Next(75) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 243, 1, false);
            }
			else if (this.type == NPCType.N13_EATER_OF_WORLDS_HEAD || this.type == NPCType.N14_EATER_OF_WORLDS_BODY || this.type == NPCType.N15_EATER_OF_WORLDS_TAIL)
			{
				int stack2 = Main.rand.Next(1, 3);
				if (Main.rand.Next(2) == 0)
				{
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 86, stack2, false);
				}
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
				if (Main.rand.Next(3) == 0 && Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statLife < Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statLifeMax)
				{
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 58, 1, false);
				}
			}
//            if (this.type == NPCType.N39_BONE_SERPENT_HEAD || this.type == NPCType.N40_BONE_SERPENT_BODY || this.type == NPCType.N41_BONE_SERPENT_TAIL)
//            {
//                if (Main.rand.Next(100) == 0)
//                {
//                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 220, 1, false);
//                }
//                else
//                {
//                    if (Main.rand.Next(100) == 0)
//                    {
//                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 218, 1, false);
//                    }
//                }
//            }
            else if (this.type == NPCType.N63_BLUE_JELLYFISH || this.type == NPCType.N64_PINK_JELLYFISH)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 282, Main.rand.Next(1, 5), false);
            }
            else if (this.type == NPCType.N21_SKELETON || this.type == NPCType.N44_UNDEAD_MINER)
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
            else if (this.type == NPCType.N45_TIM)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 238, 1, false);
            }
            else if (this.type == NPCType.N50_KING_SLIME)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, Main.rand.Next(256, 259), 1, false);
            }
            else if (this.type == NPCType.N23_METEOR_HEAD && Main.rand.Next(50) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 116, 1, false);
            }
			else if (this.type == NPCType.N24_FIRE_IMP && Main.rand.Next(300) == 0)
			{
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 244, 1, false);
			}
			else if (this.type == NPCType.N31_ANGRY_BONES || this.type == NPCType.N32_DARK_CASTER || this.type == NPCType.N34_CURSED_SKULL)
			{
				if (Main.rand.Next(75) == 0)
				{
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 327, 1, false);
				}
				else
				{
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 154, Main.rand.Next(1, 4), false);
				}
			}
            else if (this.type == NPCType.N26_GOBLIN_PEON || this.type == NPCType.N27_GOBLIN_THIEF || this.type == NPCType.N28_GOBLIN_WARRIOR || this.type == NPCType.N29_GOBLIN_SORCERER)
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
            else if (this.type == NPCType.N42_HORNET && Main.rand.Next(2) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 209, 1, false);
            }
            else if (this.type == NPCType.N43_MAN_EATER && Main.rand.Next(4) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 210, 1, false);
            }
            else if (this.type == NPCType.N65_SHARK)
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
            else if (this.type == NPCType.N48_HARPY && Main.rand.Next(5) == 0)
            {
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 320, 1, false);
            }
            if (this.boss)
            {
                //boss kill
                //int BossType = 0;
                if (this.type == NPCType.N04_EYE_OF_CTHULU)
                {
                    NPC.downedBoss1 = true;
                    //BossType = 1;
                }
                if (this.type == NPCType.N13_EATER_OF_WORLDS_HEAD || this.type == NPCType.N14_EATER_OF_WORLDS_BODY || this.type == NPCType.N15_EATER_OF_WORLDS_TAIL)
                {
                    NPC.downedBoss2 = true;
                    this.Name = "Eater of Worlds";
                    //BossType = 2;
                }
                if (this.type == NPCType.N35_SKELETRON_HEAD)
                {
                    NPC.downedBoss3 = true;
                    this.Name = "Skeletron";
                    //BossType = 3;
                }
                int stack4 = Main.rand.Next(5, 16);
                Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 28, stack4, false);
                int num = Main.rand.Next(5) + 5;
                for (int i = 0; i < num; i++)
                {
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 58, 1, false);
                }

                NetMessage.SendData(25, -1, -1, this.Name + " has been defeated!", 255, 175f, 75f, 255f);

//                NPCBossDeathEvent npcEvent = new NPCBossDeathEvent();
//                npcEvent.Boss = BossType;
//                Sender sender = new Sender();
//                sender.Op = true;
//                npcEvent.Sender = sender;
                //Server.PluginManager.processHook(Hooks.NPC_BOSSDEATH, npcEvent);
            }
            if (Main.rand.Next(6) == 0 && this.lifeMax > 1)
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
            float stackModifier = this.value;
            stackModifier *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
            if (Main.rand.Next(5) == 0)
            {
                stackModifier *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;
            }
            if (Main.rand.Next(10) == 0)
            {
                stackModifier *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;
            }
            if (Main.rand.Next(15) == 0)
            {
                stackModifier *= 1f + (float)Main.rand.Next(15, 31) * 0.01f;
            }
            if (Main.rand.Next(20) == 0)
            {
                stackModifier *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;
            }
            while ((int)stackModifier > 0)
            {
                if (stackModifier > 1000000f)
                {
                    int stack = (int)(stackModifier / 1000000f);
                    if (stack > 50 && Main.rand.Next(2) == 0)
                    {
                        stack /= Main.rand.Next(3) + 1;
                    }
                    if (Main.rand.Next(2) == 0)
                    {
                        stack /= Main.rand.Next(3) + 1;
                    }
                    stackModifier -= (float)(1000000 * stack);
                    Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 74, stack, false);
                }
                else
                {
                    if (stackModifier > 10000f)
                    {
                        int stack = (int)(stackModifier / 10000f);
                        if (stack > 50 && Main.rand.Next(2) == 0)
                        {
                            stack /= Main.rand.Next(3) + 1;
                        }
                        if (Main.rand.Next(2) == 0)
                        {
                            stack /= Main.rand.Next(3) + 1;
                        }
                        stackModifier -= (float)(10000 * stack);
                        Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 73, stack, false);
                    }
                    else
                    {
                        if (stackModifier > 100f)
                        {
                            int stack = (int)(stackModifier / 100f);
                            if (stack > 50 && Main.rand.Next(2) == 0)
                            {
                                stack /= Main.rand.Next(3) + 1;
                            }
                            if (Main.rand.Next(2) == 0)
                            {
                                stack /= Main.rand.Next(3) + 1;
                            }
                            stackModifier -= (float)(100 * stack);
                            Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 72, stack, false);
                        }
                        else
                        {
                            int stack = (int)stackModifier;
                            if (stack > 50 && Main.rand.Next(2) == 0)
                            {
                                stack /= Main.rand.Next(3) + 1;
                            }
                            if (Main.rand.Next(2) == 0)
                            {
                                stack /= Main.rand.Next(4) + 1;
                            }
                            if (stack < 1)
                            {
                                stack = 1;
                            }
                            stackModifier -= (float)stack;
                            Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 71, stack, false);
                        }
                    }
                }
            }
        }

		/// <summary>
		/// Runs effects of striking NPCs
		/// </summary>
		/// <param name="hitDirection">Direction of strike</param>
		/// <param name="dmg">Raw amount of damage done</param>
        public void HitEffect(int hitDirection = 0, double dmg = 10.0)
        {
            if (this.type == NPCType.N01_BLUE_SLIME || this.type == NPCType.N16_MOTHER_SLIME || this.type == NPCType.N71_DUNGEON_SLIME)
            {
                if (this.life > 0)
                {
                    int pointlessInteger = 0;
                    while ((double)pointlessInteger < dmg / (double)this.lifeMax * 100.0)
                    {
                        pointlessInteger++;
                    }
                }
                else
                {
                    if (this.type == NPCType.N16_MOTHER_SLIME)
                    {
                        int spawnedSlimes = Main.rand.Next(2) + 2;
                        for (int slimeNum = 0; slimeNum < spawnedSlimes; slimeNum++)
                        {
                            int npcIndex = NPC.NewNPC((int)(this.Position.X + (float)(this.Width / 2)), (int)(this.Position.Y + (float)this.Height), "Baby Slime", 0);
                            NPC npc = Main.npcs[npcIndex];
                            npc.Velocity.X = this.Velocity.X * 2f;
                            npc.Velocity.Y = this.Velocity.Y;
                            
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
// removed in 1.0.6
//            else if (this.type == NPCType.N59_LAVA_SLIME || this.type == NPCType.N60_HELLBAT)
//            {
//                if (this.life > 0)
//                {
//                    return;
//                }
//                if (this.type == NPCType.N59_LAVA_SLIME)
//                {
//                    int num5 = (int)(this.Position.X + (float)(this.Width / 2)) / 16;
//                    int num6 = (int)(this.Position.Y + (float)(this.Height / 2)) / 16;
//                    Main.tile.At(num5, num6).SetLava(true);
//                    if (Main.tile.At(num5, num6).Liquid < 200)
//                    {
//                        Main.tile.At(num5, num6).SetLiquid(200);
//                    }
//                    WorldModify.TileFrame(num5, num6, false, false);
//                    return;
//                }
//            }
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

		/// <summary>
		/// Method used to spawn Skeletron
		/// </summary>
        public static void SpawnSkeletron()
        {
            if (Main.stopSpawns)
                return;

            bool flag = true;
            bool flag2 = false;
            Vector2 vector = default(Vector2);
            int width = 0;
            int height = 0;
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
                    width = Main.npcs[j].Width;
                    height = Main.npcs[j].Height;

                    NetMessage.SendData(23, -1, -1, "", j);
                }
            }
            if (flag && flag2)
            {
                int npcIndex = NPC.NewNPC((int)vector.X + width / 2, (int)vector.Y + height / 2, 35, 0);
                Main.npcs[npcIndex].netUpdate = true;
                NetMessage.SendData(25, -1, -1, "Skeletron has awoken!", 255, 175f, 75f, 255f);
            }
        }

		/// <summary>
		/// Updates specified NPC based on any changes, including environment
		/// </summary>
		/// <param name="i">Main.npcs[] index of NPC to update</param>
        public static void UpdateNPC(int i)
        {
            NPC npc = Main.npcs[i];
            npc.whoAmI = i;
            if (npc.Active)
            {
				npc.lifeRegen = 0;
				npc.poisoned = false;
				npc.onFire = false;
				for (int j = 0; j < 5; j++)
				{
					if (npc.buffType[j] > 0 && npc.buffTime[j] > 0)
					{
						npc.buffTime[j]--;
						if (npc.buffType[j] == 20)
						{
							npc.poisoned = true;
						}
						else
						{
							if (npc.buffType[j] == 24)
							{
								npc.onFire = true;
							}
						}
					}
				}
				
				for (int k = 0; k < 5; k++)
				{
					if (npc.buffType[k] > 0 && npc.buffTime[k] <= 0)
					{
						npc.DelBuff(k);
						NetMessage.SendData(54, -1, -1, "", npc.whoAmI, 0f, 0f, 0f, 0);
					}
				}
				
				if (npc.poisoned)
				{
					npc.lifeRegen = -4;
				}
				if (npc.onFire)
				{
					npc.lifeRegen = -8;
				}
				
				npc.lifeRegenCount += npc.lifeRegen;
				while (npc.lifeRegenCount >= 120)
				{
					npc.lifeRegenCount -= 120;
					if (npc.life < npc.lifeMax)
					{
						npc.life++;
					}
					if (npc.life > npc.lifeMax)
					{
						npc.life = npc.lifeMax;
					}
				}
				while (npc.lifeRegenCount <= -120)
				{
					npc.lifeRegenCount += 120;
					npc.life--;
					if (npc.life <= 0)
					{
						npc.life = 1;
						npc.StrikeNPC (World.Sender, 9999, 0f, 0, false);
						NetMessage.SendData(28, -1, -1, "", npc.whoAmI, 9999f, 0f, 0f, 0);
					}
				}
                if (Main.bloodMoon)
                {
                    if (npc.type == NPCType.N46_BUNNY)
                    {
                        npc.Transform (47);
                    }
                    else if (npc.type == NPCType.N55_GOLDFISH)
                    {
                        npc.Transform (57);
                    }
                }
                float maxVel = 10f;
                float vel = 0.3f;
                
				float num4 = (float)(Main.maxTilesX / 4200);
				num4 *= num4;
				float num5 = (float)((double)(npc.Position.Y / 16f - (60f + 10f * num4)) / (Main.worldSurface / 6.0));
				if ((double)num5 < 0.25)
				{
					num5 = 0.25f;
				}
				if (num5 > 1f)
				{
					num5 = 1f;
				}
				vel *= num5;
				
                if (npc.wet)
                {
                    vel = 0.2f;
                    maxVel = 7f;
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
                npc.AI(i);

                for (int j = 0; j < 256; j++)
                {
                    if (npc.immune[j] > 0)
                    {
                        npc.immune[j]--;
                    }
                }
                if (!npc.noGravity)
                {
                    npc.Velocity.Y = npc.Velocity.Y + vel;
                    if (npc.Velocity.Y > maxVel)
                    {
                        npc.Velocity.Y = maxVel;
                    }
                }
                if ((double)npc.Velocity.X < 0.005 && (double)npc.Velocity.X > -0.005)
                {
                    npc.Velocity.X = 0f;
                }
                if (npc.type != NPCType.N37_OLD_MAN && (npc.friendly || npc.type == NPCType.N46_BUNNY || npc.type == NPCType.N55_GOLDFISH || npc.type == NPCType.N74_BIRD))
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
                                    int damage = Main.npcs[k].damage;
                                    int knockBack = 6;
                                    int direction = 1;
                                    if (Main.npcs[k].Position.X + (float)(Main.npcs[k].Width / 2) > npc.Position.X + (float)(npc.Width / 2))
                                    {
                                        direction = -1;
                                    }
									if (Main.npcs[i].StrikeNPC (Main.npcs[k], damage, (float)knockBack, direction))
									{
										NetMessage.SendData(28, -1, -1, "", i, (float)damage, (float)knockBack, (float)direction);
										npc.netUpdate = true;
										npc.immune[255] = 30;
									}
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
                        if (!npc.lavaImmune && (!npc.dontTakeDamage) && npc.immune[255] == 0)
                        {
							if (npc.StrikeNPC (World.Sender, 50, 0f, 0))
							{
								npc.AddBuff(24, 420, false);
								npc.immune[255] = 30;
								NetMessage.SendData(28, -1, -1, "", npc.whoAmI, 50f);
							}
                        }
                    }
                    
					bool flag2 = false;
					if (npc.type == NPCType.N72_BLAZING_WHEEL)
					{
						flag2 = false;
						npc.wetCount = 0;
						flag = false;
					}
					else
					{
						flag2 = Collision.WetCollision(npc.Position, npc.Width, npc.Height);
					}
					
                    if (flag2)
                    {
						if (npc.onFire && !npc.lavaWet)
						{
							for (int n = 0; n < 5; n++)
							{
								if (npc.buffType[n] == 24)
								{
									npc.DelBuff(n);
								}
							}
						}
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
					if (npc.aiStyle == 14)
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
						if (npc.type == NPCType.N72_BLAZING_WHEEL)
						{
							Vector2 vector2 = new Vector2(npc.Position.X + (float)(npc.Width / 2), npc.Position.Y + (float)(npc.Height / 2));
							int width = 12;
							int height = 12;
							vector2.X -= (float)(width / 2);
							vector2.Y -= (float)(height / 2);
							npc.Velocity = Collision.TileCollision(vector2, npc.Velocity, width, height, true, true);
						}
						else
						{
							npc.Velocity = Collision.TileCollision(npc.Position, npc.Velocity, npc.Width, npc.Height, flag3, flag3);
						}
                        
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
				if (!npc.noTileCollide && npc.lifeMax > 1 && Collision.SwitchTiles(npc.Position, npc.Width, npc.Height, npc.oldPosition) && npc.type == NPCType.N46_BUNNY)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 400f;
					npc.ai[2] = 0f;
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
                npc.justHit = false;
            }
        }

		/// <summary>
		/// Gets chat information based off of environment
		/// </summary>
		/// <returns>String to display in chat bubble</returns>
        public string GetChat()
        {
            bool merchantActive = false;
            bool nurseActive = false;
            bool armsDealerActive = false;
            bool dryadActive = false;
            bool oldManActive = false;
            bool demolitionistActive = false;
            for (int i = 0; i < MAX_NPCS; i++)
            {
				switch(Main.npcs[i].type)
				{
					case NPCType.N17_MERCHANT:
						{
							merchantActive = true;
							break;
						}
					case NPCType.N18_NURSE:
						{
							nurseActive = true;
							break;
						}
                    case NPCType.N19_ARMS_DEALER:
						{
							armsDealerActive = true;
							break;
						}
                    case NPCType.N20_DRYAD:
                        {
                            dryadActive = true;
							break;
                        }
					case NPCType.N37_OLD_MAN:
                        {
                            oldManActive = true;
							break;
                        }
					case NPCType.N38_DEMOLITIONIST:
                        {
                            demolitionistActive = true;
							break;
                        }
					default:
						break;
                }
            }
            string result = "";
			switch (this.type)
			{
				case NPCType.N17_MERCHANT:
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
							else if (Main.time > 37800.0)
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
								else if (num == 1)
								{
									result = "Boy, that sun is hot! I do have some perfectly ventilated armor.";
								}
								else
								{
									result = "The sun is high, but my prices are not.";
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
							else if (Main.time < 9720.0)
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
							else if (Main.time > 22680.0)
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
						break;
					}
				case NPCType.N18_NURSE:
					{
						if (demolitionistActive && Main.rand.Next(4) == 0)
						{
							result = "I wish that bomb maker would be more careful.  I'm getting tired of having to sew his limbs back on every day.";
						}
						else if ((double)Main.players[Main.myPlayer].statLife < (double)Main.players[Main.myPlayer].statLifeMax * 0.33)
						{
							int num3 = Main.rand.Next(5);
							if (num3 == 0)
							{
								result = "I think you look better this way.";
							}
							else if (num3 == 1)
							{
								result = "Eww.. What happened to your face?";
							}
							else if (num3 == 2)
							{
								result = "MY GOODNESS! I'm good but I'm not THAT good.";
							}
							else if (num3 == 3)
							{
								result = "Dear friends we are gathered here today to bid farewell... oh, you'll be fine.";
							}
							else
							{
								result = "You left your arm over there. Let me get that for you..";
							}
						}
						else if ((double)Main.players[Main.myPlayer].statLife < (double)Main.players[Main.myPlayer].statLifeMax * 0.66)
						{
							int num4 = Main.rand.Next(4);
							if (num4 == 0)
							{
								result = "Quit being such a baby! I've seen worse.";
							}
							else if (num4 == 1)
							{
								result = "That's gonna need stitches!";
							}
							else if (num4 == 2)
							{
								result = "Trouble with those bullies again?";
							}
							else
							{
								result = "You look half digested. Have you been chasing slimes again?";
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
						break;
					}
				case NPCType.N19_ARMS_DEALER:
					{
						if (nurseActive && Main.rand.Next(4) == 0)
						{
							result = "Make it quick! I've got a date with the nurse in an hour.";
						}
						else if (dryadActive && Main.rand.Next(4) == 0)
						{
							result = "That dryad is a looker.  Too bad she's such a prude.";
						}
						else if (demolitionistActive && Main.rand.Next(4) == 0)
						{
							result = "Don't bother with that firework vendor, I've got all you need right here.";
						}
						else if (Main.bloodMoon)
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
							else if (num6 == 1)
							{
								result = "Keep your hands off my gun, buddy!";
							}
						}
						break;
					}
				case NPCType.N20_DRYAD:
					{
						if (armsDealerActive && Main.rand.Next(4) == 0)
						{
							result = "I wish that gun seller would stop talking to me. Doesn't he realize I'm 500 years old?";
						}
						else if (merchantActive && Main.rand.Next(4) == 0)
						{
							result = "That merchant keeps trying to sell me an angel statue. Everyone knows that they don't do anything.";
						}
						else if (oldManActive && Main.rand.Next(4) == 0)
						{
							result = "Have you seen the old man walking around the dungeon? He doesn't look well at all...";
						}
						else if (Main.bloodMoon)
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
							else if (num7 == 1)
							{
								result = "Be safe; Terraria needs you!";
							}
							else if (num7 == 2)
							{
								result = "The sands of time are flowing. And well, you are not aging very gracefully.";
							}
							else if (num7 == 3)
							{
								result = "What's this about me having more 'bark' than bite?";
							}
							else if (num7 == 4)
							{
								result = "So two goblins walk into a bar, and one says to the other, 'Want to get a Gobblet of beer?!'";
							}
							else
							{
								result = "Be safe; Terraria needs you!";
							}
						}
						break;
					}
				case NPCType.N22_GUIDE:
					{
						if (Main.bloodMoon)
						{
							result = "You can tell a Blood Moon is out when the sky turns red. There is something about it that causes monsters to swarm.";
						}
						else if (!Main.dayTime)
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
							else if (num8 == 1)
							{
								result = "I am here to give you advice on what to do next.  It is recommended that you talk with me anytime you get stuck.";
							}
							else if (num8 == 2)
							{
								result = "They say there is a person who will tell you how to survive in this land... oh wait. That's me.";
							}
						}
						break;
					}
				case NPCType.N37_OLD_MAN:
					{
						if (Main.dayTime)
						{
							int num9 = Main.rand.Next(2);
							if (num9 == 0)
							{
								result = "I cannot let you enter until you free me of my curse.";
							}
							else if (num9 == 1)
							{
								result = "Come back at night if you wish to enter.";
							}
						}
						else if (Main.players[Main.myPlayer].statLifeMax < 300 || Main.players[Main.myPlayer].statDefense < 10)
						{
							int num10 = Main.rand.Next(2);
							if (num10 == 0)
							{
								result = "You are far to weak to defeat my curse.  Come back when you aren't so worthless.";
							}
							else if (num10 == 1)
							{
								result = "You pathetic fool.  You cannot hope to face my master as you are now.";
							}
						}
						else
						{
							int num11 = Main.rand.Next(2);
							if (num11 == 0)
							{
								result = "You just might be strong enough to free me from my curse...";
							}
							else if (num11 == 1)
							{
								result = "Stranger, do you possess the strength to defeat my master?";
							}
						}
						break;
					}
				case NPCType.N38_DEMOLITIONIST:
					{
						if (Main.bloodMoon)
						{
							result = "I've got something for them zombies alright!";
						}
						else if (armsDealerActive && Main.rand.Next(4) == 0)
						{
							result = "Even the gun dealer wants what I'm selling!";
						}
						else if (nurseActive && Main.rand.Next(4) == 0)
						{
							result = "I'm sure the nurse will help if you accidentally lose a limb to these.";
						}
						else if (dryadActive && Main.rand.Next(4) == 0)
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
							else if (num12 == 1)
							{
								result = "It's a good day to die!";
							}
							else if (num12 == 2)
							{
								result = "I wonder what happens if I... (BOOM!)... Oh, sorry, did you need that leg?";
							}
							else if (num12 == 3)
							{
								result = "Dynamite, my own special cure-all for what ails ya.";
							}
							else
							{
								result = "Check out my goods; they have explosive prices!";
							}
						}
						break;
					}
				case NPCType.N54_CLOTHIER:
					{
						if (Main.bloodMoon)
						{
							result = Main.players[Main.myPlayer].Name + "... we have a problem! Its a blood moon out there!";
						}
						else if (nurseActive && Main.rand.Next(4) == 0)
						{
							result = "T'were I younger, I would ask the nurse out. I used to be quite the lady killer.";
						}
						else if (Main.players[Main.myPlayer].head == 24)
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
							else if (num13 == 1)
							{
								result = "Mama always said I would make a great tailor.";
							}
							else if (num13 == 2)
							{
								result = "Life's like a box of clothes, you never know what you are gonna wear!";
							}
							else
							{
								result = "Being cursed was lonely, so I once made a friend out of leather. I named him Wilson.";
							}
						}
						break;
					}
				default:
					break;
			}
            return result;
        }

		/// <summary>
		/// Clones this NPC
		/// </summary>
		/// <returns>Cloned NPC object</returns>
        public override object Clone()
        {
            NPC cloned = (NPC)base.MemberwiseClone();
            //NPC.npcSlots = cloned.slots;
            cloned.frame = default(Rectangle);
            cloned.Width = (int)((float)cloned.Width * cloned.scale);
            cloned.Height = (int)((float)cloned.Height * cloned.scale);
            if (cloned.scaleOverrideAdjustment && (cloned.Height == 16 || cloned.Height == 32))
                cloned.Height += 1; //FIXME: this is really ugly
            cloned.life = cloned.lifeMax;
            cloned.defDamage = cloned.damage;
            cloned.defDefense = cloned.defense;
            //cloned.life = (int)(cloned.lifeMax * cloned.scale);
            //cloned.defense = (int)(cloned.
            //cloned.slots *= cloned.scale;
            
            cloned.ai = new float[NPC.MAX_AI];
            Array.Copy(ai, cloned.ai, NPC.MAX_AI);
            cloned.immune = new ushort[256];
            Array.Copy(immune, cloned.immune, 256);
            cloned.buffType = new int[5];
            Array.Copy (buffType, cloned.buffType, 5);
            cloned.buffTime = new int[5];
            Array.Copy (buffTime, cloned.buffTime, 5);
            //cloned.buffImmune = new System.Collections.BitArray (buffImmune);
            cloned.buffImmune = new bool[27];
            Array.Copy (buffImmune, cloned.buffImmune, 27);
            return cloned;
        }

        //AI Stuff

        private delegate void AIFunction(NPC npc, bool flag);

        private static Dictionary<int, AIFunction> AIFunctions = new Dictionary<int, AIFunction>();

        private static bool aiLoaded = false;

		//[ToDo] 1.1 - Implement all
		private int realLife;

        private void LoadAIFunctions()
        {
            if (!aiLoaded)
            {
                aiLoaded = true;

                AIFunctions.Add(0, AIUnknown);
                AIFunctions.Add(1, AISlime);
                AIFunctions.Add(2, AIDemonEye);
                AIFunctions.Add(3, AIFighter);
                AIFunctions.Add(4, AIEoC);
                AIFunctions.Add(5, AIFlyDirect);
                AIFunctions.Add(6, AIWorm);
                AIFunctions.Add(7, AIFriendly);
                AIFunctions.Add(8, AIWizard);
                AIFunctions.Add(9, AISphere);
                AIFunctions.Add(10, AICursedSkull);
                AIFunctions.Add(11, AISkeletronHead);
                AIFunctions.Add(12, AISkeletronHand);
                AIFunctions.Add(13, AIMunchyPlant);
                AIFunctions.Add(14, AIFlyWinged);
                AIFunctions.Add(15, AIKingSlime);
                AIFunctions.Add(16, AIFish);
                AIFunctions.Add(17, AIVulture);
                AIFunctions.Add(18, AIJellyFish);
                AIFunctions.Add(19, AIAntlion);
                AIFunctions.Add(20, AISpikedBall);
				AIFunctions.Add(21, AIBlazingWheel);
				AIFunctions.Add(22, AISlowDebuffFlying);
				AIFunctions.Add(23, AITool);
				AIFunctions.Add(24, AIBird);
				AIFunctions.Add(25, AIMimic);
				AIFunctions.Add(26, AIUnicorn);
				AIFunctions.Add(27, AIWallOfFlesh);
				AIFunctions.Add(28, AIWallOfFlesh_Eye);
				AIFunctions.Add(29, AITheHungry);
				AIFunctions.Add(30, AIRetinazer);
				AIFunctions.Add(31, AISpazmatism);
				AIFunctions.Add(32, AISkeletronPrime);
				AIFunctions.Add(33, AIPrimeSaw);
				AIFunctions.Add(34, AIPrimeVice);
				AIFunctions.Add(35, AIPrimeCannon);
				AIFunctions.Add(36, AIPrimeLaser);
				AIFunctions.Add(37, AITheDestroyer);
            }
        }

        // 0
        private void AIUnknown(NPC npc, bool flag)
        {
            npc.Velocity.X = npc.Velocity.X * 0.93f;
            if ((double)npc.Velocity.X > -0.1 && (double)npc.Velocity.X < 0.1)
            {
                npc.Velocity.X = 0f;
                return;
            }
        }

        // 1
        private void AISlime(NPC npc, bool flagg)
        {
            bool flag = (!Main.dayTime) || (npc.life != npc.lifeMax) || (npc.Position.Y > Main.worldSurface * 16.0) || (npc.Type == 81);
            
            if (npc.ai[2] > 1f)
            {
                npc.ai[2] -= 1f;
            }
            if (npc.wet)
            {
				if (this.collideY)
				{
					this.Velocity.Y = -2f;
				}
                if (npc.Velocity.Y < 0f && npc.ai[3] == npc.Position.X)
                {
                    npc.direction *= -1;
                    npc.ai[2] = 200f;
                }
                if (npc.Velocity.Y > 0f)
                {
                    npc.ai[3] = npc.Position.X;
                }
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
                if (npc.ai[2] == 1f && flag)
                {
                    npc.TargetClosest(true);
                }
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
                    npc.ai[2] = 200f;
                }
                npc.ai[3] = 0f;
                npc.Velocity.X = npc.Velocity.X * 0.8f;
                if ((double)npc.Velocity.X > -0.1 && (double)npc.Velocity.X < 0.1)
                {
                    npc.Velocity.X = 0f;
                }
                if (flag)
                {
                    npc.ai[0] += 1f;
                }
                npc.ai[0] += 1f;
                if (npc.Type == 59)
                {
                    npc.ai[0] += 2f;
                }
                if (npc.Type == 71)
                {
                    npc.ai[0] += 3f;
                }
				if (this.Type == 138)
				{
					this.ai[0] += 2f;
				}
				if (this.Type == 81)
				{
					if (this.scale >= 0f)
					{
						this.ai[0] += 4f;
					}
					else
					{
						this.ai[0] += 1f;
					}
				}
                if (npc.ai[0] >= 0f)
                {
                    npc.netUpdate = true;
                    if (flag && npc.ai[2] == 1f)
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
                    }
					else
					{
						npc.Velocity.Y = -6f;
						npc.Velocity.X = npc.Velocity.X + (float)(2 * npc.direction);
						if (npc.Type == 59)
						{
						    npc.Velocity.X = npc.Velocity.X + (float)(2 * npc.direction);
						}
						npc.ai[0] = -120f;
						npc.ai[1] += 1f;
					}
					
					if (this.Type == 141)
					{
						this.Velocity.Y = this.Velocity.Y * 1.3f;
						this.Velocity.X = this.Velocity.X * 1.2f;
						return;
					}
                }
                else
                {
                    if (npc.ai[0] >= -30f)
                    {
                        npc.aiAction = 1;
                        return;
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
                        return;
                    }
                    npc.Velocity.X = npc.Velocity.X * 0.93f;
                    return;
                }
            }
        }

        // 2
        private void AIDemonEye(NPC npc, bool flag)
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

        // 3
        private void AIFighter(NPC npc, bool flag)
        {
            int num3 = 60;
            bool flag2 = false;
            if (npc.Velocity.Y == 0f && ((npc.Velocity.X > 0f && npc.direction < 0) || (npc.Velocity.X < 0f && npc.direction > 0)))
            {
                flag2 = true;
            }
            if (npc.Position.X == npc.oldPosition.X || npc.ai[3] >= (float)num3 || flag2)
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
            if (npc.justHit)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] == (float)num3)
            {
                npc.netUpdate = true;
            }
            if ((!Main.dayTime || (double)npc.Position.Y > Main.worldSurface * 16.0 || npc.Type == 26 || npc.Type == 27 || npc.Type == 28 || npc.Type == 31 || npc.Type == 47 || npc.Type == 67 || npc.Type == 73) && npc.ai[3] < (float)num3)
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
                if (npc.Type == 21 || npc.Type == 26 || npc.Type == 31 || npc.Type == 47 || npc.Type == 73)
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
            int tileX = (int)((npc.Position.X + (float)(npc.Width / 2) + (float)(15 * npc.direction)) / 16f);
            int tileY = (int)((npc.Position.Y + (float)npc.Height - 15f) / 16f);

            if (!(npc.Type == 47 || npc.Type == 67))
            {
                if (Main.tile.At(tileX, tileY - 1).Active && Main.tile.At(tileX, tileY - 1).Type == 10)
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
                        bool flag4 = false;
                        if (npc.ai[1] >= 10f)
                        {
                            flag4 = true;
                            npc.ai[1] = 10f;
                        }
                        WorldModify.KillTile(tileX, tileY - 1, true, false, false);
                        if (flag4)
                        {
                            if (npc.Type == 26)
                            {
                                WorldModify.KillTile(tileX, tileY - 1, false, false, false);
                                NetMessage.SendData(17, -1, -1, "", 0, (float)tileX, (float)(tileY - 1), 0f, 0);
                                return;
                            }
                            else
                            {
                                bool flag5 = WorldModify.OpenDoor(tileX, tileY, npc.direction, npc);
                                if (!flag5)
                                {
                                    npc.ai[3] = (float)num3;
                                    npc.netUpdate = true;
                                }
                                else
                                {
                                    NetMessage.SendData(19, -1, -1, "", 0, (float)tileX, (float)tileY, (float)npc.direction, 0);
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
                        if (Main.tile.At(tileX, tileY - 2).Active && Main.tileSolid[(int)Main.tile.At(tileX, tileY - 2).Type])
                        {
                            if (Main.tile.At(tileX, tileY - 3).Active && Main.tileSolid[(int)Main.tile.At(tileX, tileY - 3).Type])
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
                            if (Main.tile.At(tileX, tileY - 1).Active && Main.tileSolid[(int)Main.tile.At(tileX, tileY - 1).Type])
                            {
                                npc.Velocity.Y = -6f;
                                npc.netUpdate = true;
                            }
                            else
                            {
                                if (Main.tile.At(tileX, tileY).Active && Main.tileSolid[(int)Main.tile.At(tileX, tileY).Type])
                                {
                                    npc.Velocity.Y = -5f;
                                    npc.netUpdate = true;
                                }
                                else
                                {
                                    if (npc.directionY < 0 && npc.Type != 67 && (!Main.tile.At(tileX, tileY + 1).Active || !Main.tileSolid[(int)Main.tile.At(tileX, tileY + 1).Type]) && (!Main.tile.At(tileX + npc.direction, tileY + 1).Active || !Main.tileSolid[(int)Main.tile.At(tileX + npc.direction, tileY + 1).Type]))
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
                    if ((npc.Type == 31 || npc.Type == 47) && npc.Velocity.Y == 0f && Math.Abs(npc.Position.X + (float)(npc.Width / 2) - (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2))) < 100f && Math.Abs(npc.Position.Y + (float)(npc.Height / 2) - (Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2))) < 50f && ((npc.direction > 0 && npc.Velocity.X >= 1f) || (npc.direction < 0 && npc.Velocity.X <= -1f)))
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
        }

        // 4
        private void AIEoC(NPC npc, bool flag)
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
                                if (npc.ai[3] >= 110f)
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

                                    int num21 = NPC.NewNPC((int)vector2.X, (int)vector2.Y, 5, 0);
                                    Main.npcs[num21].Velocity.X = vector3.X;
                                    Main.npcs[num21].Velocity.Y = vector3.Y;
                                    if (num21 < MAX_NPCS)
                                    {
                                        NetMessage.SendData(23, -1, -1, "", num21, 0f, 0f, 0f, 0);
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
                            float num22 = 6f;
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
                                if (npc.ai[2] >= 150f)
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
                        npc.damage = 23;
                        npc.defense = 0;
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
                                float num31 = 6.8f;
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
                                if (npc.ai[2] >= 130f)
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

        // 5
        private void AIFlyDirect(NPC npc, bool flag)
        {
            if (npc.target < 0 || npc.target == 255 || Main.players[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
			var target = Main.players[npc.target];
			
			float maxVel = 6f;
			float accel = 0.05f;
			
			switch (npc.Type)
			{
				case 6:
				{
					maxVel = 4f;
					accel = 0.02f;
					break;
				}
				
				case 94:
				{
					maxVel = 4.2f;
					accel = 0.022f;
					break;
				}
				
				case 42:
				{
					maxVel = 3.5f;
					accel = 0.021f;
					break;
				}
				
				case 23:
				{
					maxVel = 1f;
					accel = 0.03f;
					break;
				}
				
				case 5:
				{
					maxVel = 5f;
					accel = 0.03f;
					break;
				}
			}
			
			Vector2 center = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
			float dx = target.Position.X + (float)(target.Width / 2);
			float dy = target.Position.Y + (float)(target.Height / 2);
			center.X = ((int)(center.X / 8f) * 8);
			center.Y = ((int)(center.Y / 8f) * 8);
			dx = (float)((int)(dx / 8f) * 8) - center.X;
			dy = (float)((int)(dy / 8f) * 8) - center.Y;
			
			float dist = (float)Math.Sqrt((double)(dx * dx + dy * dy));
			float dist2 = dist;
			bool flag9 = dist > 600f;
			
			if (dist == 0f)
			{
				dx = npc.Velocity.X;
				dy = npc.Velocity.Y;
			}
			else
			{
				dist = maxVel / dist;
				dx *= dist;
				dy *= dist;
			}
			
			if (npc.Type == 6 || npc.Type == 42 || npc.Type == 94 || npc.Type == 139)
			{
				if (dist2 > 100f || npc.Type == 42 || npc.Type == 94)
				{
					npc.ai[0] += 1f;
					
					if (npc.ai[0] > 0f)
						npc.Velocity.Y = npc.Velocity.Y + 0.023f;
					else
						npc.Velocity.Y = npc.Velocity.Y - 0.023f;
					
					if (npc.ai[0] < -100f || npc.ai[0] > 100f)
						npc.Velocity.X = npc.Velocity.X + 0.023f;
					else
						npc.Velocity.X = npc.Velocity.X - 0.023f;
					
					if (npc.ai[0] > 200f)
						npc.ai[0] = -200f;
				}
				if (dist2 < 150f && (npc.Type == 6 || npc.Type == 94))
				{
					npc.Velocity.X = npc.Velocity.X + dx * 0.007f;
					npc.Velocity.Y = npc.Velocity.Y + dy * 0.007f;
				}
			}
			
			if (target.dead)
			{
				dx = (float)npc.direction * maxVel / 2f;
				dy = -maxVel / 2f;
			}
			
			if (npc.Velocity.X < dx)
			{
				npc.Velocity.X = npc.Velocity.X + accel;
				if (npc.Type != 6 && npc.Type != 42 && npc.Type != 94 && npc.Type != 139 && npc.Velocity.X < 0f && dx > 0f)
					npc.Velocity.X = npc.Velocity.X + accel;
			}
			else if (npc.Velocity.X > dx)
			{
				npc.Velocity.X = npc.Velocity.X - accel;
				if (npc.Type != 6 && npc.Type != 42 && npc.Type != 94 && npc.Type != 139 && npc.Velocity.X > 0f && dx < 0f)
					npc.Velocity.X = npc.Velocity.X - accel;
			}
			
			if (npc.Velocity.Y < dy)
			{
				npc.Velocity.Y = npc.Velocity.Y + accel;
				if (npc.Type != 6 && npc.Type != 42 && npc.Type != 94 && npc.Type != 139 && npc.Velocity.Y < 0f && dy > 0f)
					npc.Velocity.Y = npc.Velocity.Y + accel;
			}
			else if (npc.Velocity.Y > dy)
			{
				npc.Velocity.Y = npc.Velocity.Y - accel;
				if (npc.Type != 6 && npc.Type != 42 && npc.Type != 94 && npc.Type != 139 && npc.Velocity.Y > 0f && dy < 0f)
					npc.Velocity.Y = npc.Velocity.Y - accel;
			}

			if (npc.Type == 23)
			{
				if (dx > 0f)
				{
					npc.spriteDirection = 1;
					npc.rotation = (float)Math.Atan2((double)dy, (double)dx);
				}
				else if (dx < 0f)
				{
					npc.spriteDirection = -1;
					npc.rotation = (float)Math.Atan2((double)dy, (double)dx) + 3.14f;
				}
			}
			else if (npc.Type == 139)
			{
				npc.localAI[0] += 1f;
				if (npc.justHit)
					npc.localAI[0] = 0f;
				
				if (npc.localAI[0] >= 120f)
				{
					npc.localAI[0] = 0f;
					if (Collision.CanHit(npc.Position, npc.Width, npc.Height, target.Position, target.Width, target.Height))
					{
						Projectile.NewProjectile (center.X, center.Y, dx, dy, 84, 25, 0f, Main.myPlayer);
					}
				}
				int tx = (int)npc.Position.X + npc.Width / 2;
				int ty = (int)npc.Position.Y + npc.Height / 2;
				tx /= 16;
				ty /= 16;
				
				if (dx > 0f)
				{
					npc.spriteDirection = 1;
					npc.rotation = (float)Math.Atan2((double)dy, (double)dx);
				}
				else if (dx < 0f)
				{
					npc.spriteDirection = -1;
					npc.rotation = (float)Math.Atan2((double)dy, (double)dx) + 3.14f;
				}
			}
			else if (npc.Type == 6 || npc.Type == 94)
			{
				npc.rotation = (float)Math.Atan2((double)dy, (double)dx) - 1.57f;
			}
			else if (npc.Type == 42)
			{
				if (dx > 0f)
				{
					npc.spriteDirection = 1;
				}
				else if (dx < 0f)
				{
					npc.spriteDirection = -1;
				}
				npc.rotation = npc.Velocity.X * 0.1f;
			}
			else
			{
				npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) - 1.57f;
			}
			
			if (npc.Type == 6 || npc.Type == 23 || npc.Type == 42 || npc.Type == 94 || npc.Type == 139)
			{
				float vscale = 0.7f;
				if (npc.Type == 6)
				    vscale = 0.4f;
				
				if (npc.collideX)
				{
					npc.netUpdate = true;
					npc.Velocity.X = npc.oldVelocity.X * -vscale;
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
					npc.netUpdate = true;
					npc.Velocity.Y = npc.oldVelocity.Y * -vscale;
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
			if ((npc.Type == 6 || npc.Type == 94) && npc.wet)
			{
				if (npc.Velocity.Y > 0f)
					npc.Velocity.Y = npc.Velocity.Y * 0.95f;
				
				npc.Velocity.Y = npc.Velocity.Y - 0.3f;
				
				if (npc.Velocity.Y < -2f)
					npc.Velocity.Y = -2f;
			}
			if (npc.Type == 42)
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
				if (npc.ai[1] == 101f)
					npc.ai[1] = 0f;
				
				npc.ai[1] += (float)Main.rand.Next(5, 20) * 0.1f * npc.scale;
				if (npc.ai[1] >= 130f)
				{
					if (Collision.CanHit(npc.Position, npc.Width, npc.Height, target.Position, target.Width, target.Height))
					{
						float num45 = 8f;
						Vector2 center2 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)(npc.Height / 2));
						float rdx = target.Position.X + (float)target.Width * 0.5f - center2.X + (float)Main.rand.Next(-20, 21);
						float rdy = target.Position.Y + (float)target.Height * 0.5f - center2.Y + (float)Main.rand.Next(-20, 21);
						if ((rdx < 0f && npc.Velocity.X < 0f) || (rdx > 0f && npc.Velocity.X > 0f))
						{
							float rdist = (float)Math.Sqrt((double)(rdx * rdx + rdy * rdy));
							rdist = num45 / rdist;
							rdx *= rdist;
							rdy *= rdist;
							int num49 = (int)(13f * npc.scale);
							int num50 = 55;
							int num51 = Projectile.NewProjectile(center2.X, center2.Y, rdx, rdy, num50, num49, 0f, Main.myPlayer);
							Main.projectile[num51].timeLeft = 300;
							npc.ai[1] = 101f;
							npc.netUpdate = true;
						}
						else
						{
							npc.ai[1] = 0f;
						}
					}
					else
					{
						npc.ai[1] = 0f;
					}
				}
			}
			
			if (npc.Type == 139 && flag9)
			{
				if ((npc.Velocity.X > 0f && dx > 0f) || (npc.Velocity.X < 0f && dx < 0f))
				{
					if (Math.Abs(npc.Velocity.X) < 12f)
					{
						npc.Velocity.X = npc.Velocity.X * 1.05f;
					}
				}
				else
				{
					npc.Velocity.X = npc.Velocity.X * 0.9f;
				}
			}
			else if (npc.Type == 94 && !target.dead)
			{
				if (npc.justHit)
				{
					npc.localAI[0] = 0f;
				}
				npc.localAI[0] += 1f;
				if (npc.localAI[0] == 180f)
				{
					if (Collision.CanHit(npc.Position, npc.Width, npc.Height, target.Position, target.Width, target.Height))
					{
						NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2) + npc.Velocity.X), (int)(npc.Position.Y + (float)(npc.Height / 2) + npc.Velocity.Y), 112, 0);
					}
					npc.localAI[0] = 0f;
				}
			}
			
			if ((Main.dayTime && npc.Type != 6 && npc.Type != 23 && npc.Type != 42 && npc.Type != 94) || target.dead)
			{
				npc.Velocity.Y = npc.Velocity.Y - accel * 2f;
				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
					return;
				}
			}
			
			if (((npc.Velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.Velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.Velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.Velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
			{
				npc.netUpdate = true;
				return;
			}
		}

        // 6
        private void AIWorm(NPC npc, bool flag)
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
                    npc.ai[2] = (float)Main.rand.Next(8, 13);
                    if (npc.Type == 10)
                    {
                        npc.ai[2] = (float)Main.rand.Next(4, 7);
                    }
                    if (npc.Type == 13)
                    {
                        npc.ai[2] = (float)Main.rand.Next(45, 56);
                    }
                    if (npc.Type == 39)
                    {
                        npc.ai[2] = (float)Main.rand.Next(12, 19);
                    }
                    npc.ai[0] = (float)NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)(npc.Position.Y + (float)npc.Height), npc.Type + 1, npc.whoAmI);
                }
                else
                {
                    if ((npc.Type == 8 || npc.Type == 11 || npc.Type == 14 || npc.Type == 40) && npc.ai[2] > 0f)
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
            if (npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD || npc.type == NPCType.N14_EATER_OF_WORLDS_BODY || npc.type == NPCType.N15_EATER_OF_WORLDS_TAIL)
            {
                //If this segment has no segment before or after it, DIE!
                if (!Main.npcs[(int)npc.ai[1]].Active && !Main.npcs[(int)npc.ai[0]].Active)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.Active = false;
                }
                //If we are a head connected to nothing, DIE!
                if (npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD && !Main.npcs[(int)npc.ai[0]].Active)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.Active = false;
                }
                //If we are a tail connected to nothing, DIE!
                if (npc.type == NPCType.N15_EATER_OF_WORLDS_TAIL && !Main.npcs[(int)npc.ai[1]].Active)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.Active = false;
                }
                //If the next segment forward has died, become a head
                if (npc.type == NPCType.N14_EATER_OF_WORLDS_BODY && !Main.npcs[(int)npc.ai[1]].Active)
                {
                    npc.type = NPCType.N13_EATER_OF_WORLDS_HEAD;
                    int num52 = npc.whoAmI;
                    float num53 = (float)npc.life / (float)npc.lifeMax;
                    float num54 = npc.ai[0];
                    //npc.SetDefaults(npc.Type, -1f);
                    //npc = Registries.NPC.Create(npc.Type); WTF!?
                    Registries.NPC.SetDefaults(npc, 13); //FIXME: remember to tweak
                    npc.Active = true;
                    npc.life = (int)((float)npc.lifeMax * num53);
                    npc.ai[0] = num54;
                    npc.TargetClosest(true);
                    npc.netUpdate = true;
                    npc.whoAmI = num52;
                }
                //If the next segment behind us has died, become a tail
                if (npc.type == NPCType.N14_EATER_OF_WORLDS_BODY && !Main.npcs[(int)npc.ai[0]].Active)
                {
                    npc.type = NPCType.N15_EATER_OF_WORLDS_TAIL;
                    int num55 = npc.whoAmI;
                    float num56 = (float)npc.life / (float)npc.lifeMax;
                    float num57 = npc.ai[1];
                    //npc.SetDefaults(npc.Type, -1f);
                    //npc = Registries.NPC.Create(npc.Type);
                    Registries.NPC.SetDefaults(npc, 14); //FIXME: remember to tweak
                    npc.Active = true;
                    npc.life = (int)((float)npc.lifeMax * num56);
                    npc.ai[1] = num57;
                    npc.TargetClosest(true);
                    npc.netUpdate = true;
                    npc.whoAmI = num55;
                }
                if (npc.life == 0)
                {
                    bool flag6 = true;
                    for (int l = 0; l < MAX_NPCS; l++)
                    {
                        if (Main.npcs[l].Active && (Main.npcs[l].type == NPCType.N13_EATER_OF_WORLDS_HEAD ||
                                                     Main.npcs[l].type == NPCType.N14_EATER_OF_WORLDS_BODY ||
                                                     Main.npcs[l].type == NPCType.N15_EATER_OF_WORLDS_TAIL))
                        {
                            flag6 = false;
                            break;
                        }
                    }
                    if (flag6)
                    {
                        npc.boss = true;
                        npc.NPCLoot();
                    }
                }
            }
            if (!npc.Active)
            {
                NetMessage.SendData(28, -1, -1, "", npc.whoAmI, -1f, 0f, 0f, 0);
            }
            int num58 = (int)(npc.Position.X / 16f) - 1;
            int num59 = (int)((npc.Position.X + (float)npc.Width) / 16f) + 2;
            int num60 = (int)(npc.Position.Y / 16f) - 1;
            int num61 = (int)((npc.Position.Y + (float)npc.Height) / 16f) + 2;
            if (num58 < 0)
            {
                num58 = 0;
            }
            if (num59 > Main.maxTilesX)
            {
                num59 = Main.maxTilesX;
            }
            if (num60 < 0)
            {
                num60 = 0;
            }
            if (num61 > Main.maxTilesY)
            {
                num61 = Main.maxTilesY;
            }
            bool flag7 = false;
            for (int m = num58; m < num59; m++)
            {
                for (int n = num60; n < num61; n++)
                {
                    if (Main.tile.At(m, n).Exists && ((Main.tile.At(m, n).Active && (Main.tileSolid[(int)Main.tile.At(m, n).Type] || (Main.tileSolidTop[(int)Main.tile.At(m, n).Type] && Main.tile.At(m, n).FrameY == 0))) || Main.tile.At(m, n).Liquid > 64))
                    {
                        Vector2 vector9;
                        vector9.X = (float)(m * 16);
                        vector9.Y = (float)(n * 16);
                        if (npc.Position.X + (float)npc.Width > vector9.X && npc.Position.X < vector9.X + 16f && npc.Position.Y + (float)npc.Height > vector9.Y && npc.Position.Y < vector9.Y + 16f)
                        {
                            flag7 = true;
                            if (Main.rand.Next(40) == 0 && Main.tile.At(m, n).Active)
                            {
                                WorldModify.KillTile(m, n, true, true, false);
                            }
                            //if (Main.tile.At(m, n).Type == 2)
                            //{
                            //    byte arg_4656_0 = Main.tile.At(m, n - 1).Type;
                            //}
                        }
                    }
                }
            }
            if (!flag7 && (npc.Type == 7 || npc.Type == 10 || npc.Type == 13 || npc.Type == 39))
            {
                Rectangle rectangle = new Rectangle((int)npc.Position.X, (int)npc.Position.Y, npc.Width, npc.Height);
                int num62 = 1000;
                bool flag8 = true;
                for (int num63 = 0; num63 < 255; num63++)
                {
                    if (Main.players[num63].Active)
                    {
                        Rectangle rectangle2 = new Rectangle((int)Main.players[num63].Position.X - num62, (int)Main.players[num63].Position.Y - num62, num62 * 2, num62 * 2);
                        if (rectangle.Intersects(rectangle2))
                        {
                            flag8 = false;
                            break;
                        }
                    }
                }
                if (flag8)
                {
                    flag7 = true;
                }
            }
            float num64 = 8f;
            float num65 = 0.07f;
            if (npc.Type == 10)
            {
                num64 = 6f;
                num65 = 0.05f;
            }
            if (npc.Type == 13)
            {
                num64 = 10f;
                num65 = 0.07f;
            }
            Vector2 vector10 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
            float num66 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector10.X;
            float num67 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector10.Y;
            float num68 = (float)Math.Sqrt((double)(num66 * num66 + num67 * num67));
            if (npc.ai[1] > 0f)
            {
                num66 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - vector10.X;
                num67 = Main.npcs[(int)npc.ai[1]].Position.Y + (float)(Main.npcs[(int)npc.ai[1]].Height / 2) - vector10.Y;
                npc.rotation = (float)Math.Atan2((double)num67, (double)num66) + 1.57f;
                num68 = (float)Math.Sqrt((double)(num66 * num66 + num67 * num67));
                num68 = (num68 - (float)npc.Width) / num68;
                num66 *= num68;
                num67 *= num68;
                npc.Velocity = default(Vector2);
                npc.Position.X = npc.Position.X + num66;
                npc.Position.Y = npc.Position.Y + num67;
                return;
            }
            if (!flag7)
            {
                npc.TargetClosest(true);
                npc.Velocity.Y = npc.Velocity.Y + 0.11f;
                if (npc.Velocity.Y > num64)
                {
                    npc.Velocity.Y = num64;
                }
                if ((double)(Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y)) < (double)num64 * 0.4)
                {
                    if (npc.Velocity.X < 0f)
                    {
                        npc.Velocity.X = npc.Velocity.X - num65 * 1.1f;
                    }
                    else
                    {
                        npc.Velocity.X = npc.Velocity.X + num65 * 1.1f;
                    }
                }
                else
                {
                    if (npc.Velocity.Y == num64)
                    {
                        if (npc.Velocity.X < num66)
                        {
                            npc.Velocity.X = npc.Velocity.X + num65;
                        }
                        else
                        {
                            if (npc.Velocity.X > num66)
                            {
                                npc.Velocity.X = npc.Velocity.X - num65;
                            }
                        }
                    }
                    else
                    {
                        if (npc.Velocity.Y > 4f)
                        {
                            if (npc.Velocity.X < 0f)
                            {
                                npc.Velocity.X = npc.Velocity.X + num65 * 0.9f;
                            }
                            else
                            {
                                npc.Velocity.X = npc.Velocity.X - num65 * 0.9f;
                            }
                        }
                    }
                }
            }
            else
            {
                if (npc.soundDelay == 0)
                {
                    float num69 = num68 / 40f;
                    if (num69 < 10f)
                    {
                        num69 = 10f;
                    }
                    if (num69 > 20f)
                    {
                        num69 = 20f;
                    }
                    npc.soundDelay = (int)num69;
                }
                num68 = (float)Math.Sqrt((double)(num66 * num66 + num67 * num67));
                float num70 = Math.Abs(num66);
                float num71 = Math.Abs(num67);
                num68 = num64 / num68;
                num66 *= num68;
                num67 *= num68;
                if ((npc.Type == 13 || npc.Type == 7) && !Main.players[npc.target].zoneEvil)
                {
                    bool flag9 = true;
                    for (int num72 = 0; num72 < 255; num72++)
                    {
                        if (Main.players[num72].Active && !Main.players[num72].dead && Main.players[num72].zoneEvil)
                        {
                            flag9 = false;
                        }
                    }
                    if (flag9)
                    {
                        if ((double)(npc.Position.Y / 16f) > (Main.rockLayer + (double)Main.maxTilesY) / 2.0)
                        {
                            npc.Active = false;
                            int num73 = (int)npc.ai[0];
                            while (num73 > 0 && num73 < MAX_NPCS && Main.npcs[num73].Active && Main.npcs[num73].aiStyle == npc.aiStyle)
                            {
                                int num74 = (int)Main.npcs[num73].ai[0];
                                Main.npcs[num73].Active = false;
                                npc.life = 0;
                                NetMessage.SendData(23, -1, -1, "", num73, 0f, 0f, 0f, 0);
                                num73 = num74;
                            }
                            NetMessage.SendData(23, -1, -1, "", npc.whoAmI, 0f, 0f, 0f, 0);
                        }
                        num66 = 0f;
                        num67 = num64;
                    }
                }
                if ((npc.Velocity.X > 0f && num66 > 0f) || (npc.Velocity.X < 0f && num66 < 0f) || (npc.Velocity.Y > 0f && num67 > 0f) || (npc.Velocity.Y < 0f && num67 < 0f))
                {
                    if (npc.Velocity.X < num66)
                    {
                        npc.Velocity.X = npc.Velocity.X + num65;
                    }
                    else
                    {
                        if (npc.Velocity.X > num66)
                        {
                            npc.Velocity.X = npc.Velocity.X - num65;
                        }
                    }
                    if (npc.Velocity.Y < num67)
                    {
                        npc.Velocity.Y = npc.Velocity.Y + num65;
                    }
                    else
                    {
                        if (npc.Velocity.Y > num67)
                        {
                            npc.Velocity.Y = npc.Velocity.Y - num65;
                        }
                    }
                }
                else
                {
                    if (num70 > num71)
                    {
                        if (npc.Velocity.X < num66)
                        {
                            npc.Velocity.X = npc.Velocity.X + num65 * 1.1f;
                        }
                        else
                        {
                            if (npc.Velocity.X > num66)
                            {
                                npc.Velocity.X = npc.Velocity.X - num65 * 1.1f;
                            }
                        }
                        if ((double)(Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y)) < (double)num64 * 0.5)
                        {
                            if (npc.Velocity.Y > 0f)
                            {
                                npc.Velocity.Y = npc.Velocity.Y + num65;
                            }
                            else
                            {
                                npc.Velocity.Y = npc.Velocity.Y - num65;
                            }
                        }
                    }
                    else
                    {
                        if (npc.Velocity.Y < num67)
                        {
                            npc.Velocity.Y = npc.Velocity.Y + num65 * 1.1f;
                        }
                        else
                        {
                            if (npc.Velocity.Y > num67)
                            {
                                npc.Velocity.Y = npc.Velocity.Y - num65 * 1.1f;
                            }
                        }
                        if ((double)(Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y)) < (double)num64 * 0.5)
                        {
                            if (npc.Velocity.X > 0f)
                            {
                                npc.Velocity.X = npc.Velocity.X + num65;
                            }
                            else
                            {
                                npc.Velocity.X = npc.Velocity.X - num65;
                            }
                        }
                    }
                }
            }
            npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) + 1.57f;
            return;
        }

        // 7
        private void AIFriendly(NPC npc, bool flag)
        {
            int num75 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
            int num76 = (int)(npc.Position.Y + (float)npc.Height + 1f) / 16;
            if (!npc.townNPC)
            {
                npc.homeTileX = num75;
                npc.homeTileY = num76;
            }
            if (npc.Type == 46 && npc.target == 255)
            {
                npc.TargetClosest(true);
            }
            bool flag10 = false;
            npc.directionY = -1;
            if (npc.direction == 0)
            {
                npc.direction = 1;
            }
            for (int num77 = 0; num77 < 255; num77++)
            {
                if (Main.players[num77].Active && Main.players[num77].talkNPC == npc.whoAmI)
                {
                    flag10 = true;
                    if (npc.ai[0] != 0f)
                    {
                        npc.netUpdate = true;
                    }
                    npc.ai[0] = 0f;
                    npc.ai[1] = 300f;
                    npc.ai[2] = 100f;
                    if (Main.players[num77].Position.X + (float)(Main.players[num77].Width / 2) < npc.Position.X + (float)(npc.Width / 2))
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
            if (npc.townNPC && !Main.dayTime && (num75 != npc.homeTileX || num76 != npc.homeTileY) && !npc.homeless)
            {
                bool flag11 = true;
                for (int num78 = 0; num78 < 2; num78++)
                {
                    Rectangle rectangle3 = new Rectangle((int)(npc.Position.X + (float)(npc.Width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(npc.Position.Y + (float)(npc.Height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                    if (num78 == 1)
                    {
                        rectangle3 = new Rectangle(npc.homeTileX * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, npc.homeTileY * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                    }
                    for (int num79 = 0; num79 < 255; num79++)
                    {
                        if (Main.players[num79].Active)
                        {
                            Rectangle rectangle4 = new Rectangle((int)Main.players[num79].Position.X, (int)Main.players[num79].Position.Y, Main.players[num79].Width, Main.players[num79].Height);
                            if (rectangle4.Intersects(rectangle3))
                            {
                                flag11 = false;
                                break;
                            }
                        }
                        if (!flag11)
                        {
                            break;
                        }
                    }
                }
                if (flag11)
                {
                    if (npc.Type == 37 || !Collision.SolidTiles(npc.homeTileX - 1, npc.homeTileX + 1, npc.homeTileY - 3, npc.homeTileY - 1))
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
                        WorldModify.QuickFindHome(npc.whoAmI);
                    }
                }
            }
            if (npc.ai[0] == 0f)
            {
                if (npc.ai[2] > 0f)
                {
                    npc.ai[2] -= 1f;
                }
                if (!Main.dayTime && !flag10)
                {
                    if (num75 == npc.homeTileX && num76 == npc.homeTileY)
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
                        if (!flag10)
                        {
                            if (num75 > npc.homeTileX)
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
                if ((Main.dayTime || (num75 == npc.homeTileX && num76 == npc.homeTileY)))
                {
                    if (num75 < npc.homeTileX - 25 || num75 > npc.homeTileX + 25)
                    {
                        if (npc.ai[2] == 0f)
                        {
                            if (num75 < npc.homeTileX - 50 && npc.direction == -1)
                            {
                                npc.direction = 1;
                                npc.netUpdate = true;
                                return;
                            }
                            if (num75 > npc.homeTileX + 50 && npc.direction == 1)
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
                    if (!Main.dayTime && num75 == npc.homeTileX && num76 == npc.homeTileY)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = (float)(200 + Main.rand.Next(200));
                        npc.ai[2] = 60f;
                        npc.netUpdate = true;
                        return;
                    }
                    if (!npc.homeless && (num75 < npc.homeTileX - 35 || num75 > npc.homeTileX + 35))
                    {
                        if (npc.Position.X < (float)(npc.homeTileX * 16) && npc.direction == -1)
                        {
                            npc.direction = 1;
                            npc.netUpdate = true;
                            npc.ai[1] = 0f;
                        }
                        else
                        {
                            if (npc.Position.X > (float)(npc.homeTileX * 16) && npc.direction == 1)
                            {
                                npc.direction = -1;
                                npc.netUpdate = true;
                                npc.ai[1] = 0f;
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
                    if (npc.closeDoor && ((npc.Position.X + (float)(npc.Width / 2)) / 16f > (float)(npc.doorX + 2) || (npc.Position.X + (float)(npc.Width / 2)) / 16f < (float)(npc.doorX - 2)))
                    {
                        bool flag12 = WorldModify.CloseDoor(npc.doorX, npc.doorY, false, npc);
                        if (flag12)
                        {
                            npc.closeDoor = false;
                            NetMessage.SendData(19, -1, -1, "", 1, (float)npc.doorX, (float)npc.doorY, (float)npc.direction, 0);
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
                        int num80 = (int)((npc.Position.X + (float)(npc.Width / 2) + (float)(15 * npc.direction)) / 16f);
                        int num81 = (int)((npc.Position.Y + (float)npc.Height - 16f) / 16f);

                        if (npc.townNPC && Main.tile.At(num80, num81 - 2).Active && Main.tile.At(num80, num81 - 2).Type == 10 && (Main.rand.Next(10) == 0 || !Main.dayTime))
                        {
                            bool flag13 = WorldModify.OpenDoor(num80, num81 - 2, npc.direction, npc);
                            if (flag13)
                            {
                                npc.closeDoor = true;
                                npc.doorX = num80;
                                npc.doorY = num81 - 2;
                                NetMessage.SendData(19, -1, -1, "", 0, (float)num80, (float)(num81 - 2), (float)npc.direction, 0);
                                npc.netUpdate = true;
                                npc.ai[1] += 80f;
                                return;
                            }
                            if (WorldModify.OpenDoor(num80, num81 - 2, -npc.direction, npc))
                            {
                                npc.closeDoor = true;
                                npc.doorX = num80;
                                npc.doorY = num81 - 2;
                                NetMessage.SendData(19, -1, -1, "", 0, (float)num80, (float)(num81 - 2), (float)(-(float)npc.direction), 0);
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
                                if (Main.tile.At(num80, num81 - 2).Active && Main.tileSolid[(int)Main.tile.At(num80, num81 - 2).Type] && !Main.tileSolidTop[(int)Main.tile.At(num80, num81 - 2).Type])
                                {
                                    if ((npc.direction == 1 && !Collision.SolidTiles(num80 - 2, num80 - 1, num81 - 5, num81 - 1)) || (npc.direction == -1 && !Collision.SolidTiles(num80 + 1, num80 + 2, num81 - 5, num81 - 1)))
                                    {
                                        if (!Collision.SolidTiles(num80, num80, num81 - 5, num81 - 3))
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
                                    if (Main.tile.At(num80, num81 - 1).Active && Main.tileSolid[(int)Main.tile.At(num80, num81 - 1).Type] && !Main.tileSolidTop[(int)Main.tile.At(num80, num81 - 1).Type])
                                    {
                                        if ((npc.direction == 1 && !Collision.SolidTiles(num80 - 2, num80 - 1, num81 - 4, num81 - 1)) || (npc.direction == -1 && !Collision.SolidTiles(num80 + 1, num80 + 2, num81 - 4, num81 - 1)))
                                        {
                                            if (!Collision.SolidTiles(num80, num80, num81 - 4, num81 - 2))
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
                                        if (Main.tile.At(num80, num81).Active && Main.tileSolid[(int)Main.tile.At(num80, num81).Type] && !Main.tileSolidTop[(int)Main.tile.At(num80, num81).Type])
                                        {
                                            if ((npc.direction == 1 && !Collision.SolidTiles(num80 - 2, num80, num81 - 3, num81 - 1)) || (npc.direction == -1 && !Collision.SolidTiles(num80, num80 + 2, num81 - 3, num81 - 1)))
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
                                    if (Main.tile.At(num80 - npc.direction, num81 + 4).Exists)
                                    {
                                        if (num75 >= npc.homeTileX - 35 && num75 <= npc.homeTileX + 35 && (!Main.tile.At(num80, num81 + 1).Active || !Main.tileSolid[(int)Main.tile.At(num80, num81 + 1).Type]) && (!Main.tile.At(num80 - npc.direction, num81 + 1).Active || !Main.tileSolid[(int)Main.tile.At(num80 - npc.direction, num81 + 1).Type]) && (!Main.tile.At(num80, num81 + 2).Active || !Main.tileSolid[(int)Main.tile.At(num80, num81 + 2).Type]) && (!Main.tile.At(num80 - npc.direction, num81 + 2).Active || !Main.tileSolid[(int)Main.tile.At(num80 - npc.direction, num81 + 2).Type]) && (!Main.tile.At(num80, num81 + 3).Active || !Main.tileSolid[(int)Main.tile.At(num80, num81 + 3).Type]) && (!Main.tile.At(num80 - npc.direction, num81 + 3).Active || !Main.tileSolid[(int)Main.tile.At(num80 - npc.direction, num81 + 3).Type]) && (!Main.tile.At(num80, num81 + 4).Active || !Main.tileSolid[(int)Main.tile.At(num80, num81 + 4).Type]) && (!Main.tile.At(num80 - npc.direction, num81 + 4).Active || !Main.tileSolid[(int)Main.tile.At(num80 - npc.direction, num81 + 4).Type]) && npc.Type != 46)
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

        // 8
        private void AIWizard(NPC npc, bool flag)
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
                    int num90 = (int)Main.players[npc.target].Position.X / 16;
                    int num91 = (int)Main.players[npc.target].Position.Y / 16;
                    int num92 = (int)npc.Position.X / 16;
                    int num93 = (int)npc.Position.Y / 16;
                    int num94 = 20;
                    int num95 = 0;
                    bool flag14 = false;
                    if (Math.Abs(npc.Position.X - Main.players[npc.target].Position.X) + Math.Abs(npc.Position.Y - Main.players[npc.target].Position.Y) > 2000f)
                    {
                        num95 = 100;
                        flag14 = true;
                    }
                    while (!flag14 && num95 < 100)
                    {
                        num95++;
                        int num96 = Main.rand.Next(num90 - num94, num90 + num94);
                        int num97 = Main.rand.Next(num91 - num94, num91 + num94);
                        for (int num98 = num97; num98 < num91 + num94; num98++)
                        {
                            if ((num98 < num91 - 4 || num98 > num91 + 4 || num96 < num90 - 4 || num96 > num90 + 4) && (num98 < num93 - 1 || num98 > num93 + 1 || num96 < num92 - 1 || num96 > num92 + 1) && Main.tile.At(num96, num98).Active)
                            {
                                bool flag15 = true;
                                if (npc.Type == 32 && Main.tile.At(num96, num98 - 1).Wall == 0)
                                {
                                    flag15 = false;
                                }
                                else
                                {
                                    if (Main.tile.At(num96, num98 - 1).Lava)
                                    {
                                        flag15 = false;
                                    }
                                }
                                if (flag15 && Main.tileSolid[(int)Main.tile.At(num96, num98).Type] && !Collision.SolidTiles(num96 - 1, num96 + 1, num98 - 4, num98 - 1))
                                {
                                    npc.ai[1] = 20f;
                                    npc.ai[2] = (float)num96;
                                    npc.ai[3] = (float)num98;
                                    flag14 = true;
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
                        NPC.NewNPC((int)npc.Position.X + npc.Width / 2, (int)npc.Position.Y - 8, 30, 0);
                    }
                    else
                    {
                        if (npc.Type == 32)
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
            if (npc.Type == 29 || npc.Type == 45)
            {
                if (Main.rand.Next(5) == 0)
                {
                    return;
                }
            }
            else
            {
                if (npc.Type == 32)
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

        // 9
        private void AISphere(NPC npc, bool flag)
        {
            if (npc.target == 255)
            {
                npc.TargetClosest(true);
                float num102 = 6f;
                if (npc.Type == 25)
                {
                    num102 = 5f;
                }
                Vector2 vector11 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                float num103 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector11.X;
                float num104 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector11.Y;
                float num105 = (float)Math.Sqrt((double)(num103 * num103 + num104 * num104));
                num105 = num102 / num105;
                npc.Velocity.X = num103 * num105;
                npc.Velocity.Y = num104 * num105;
            }
            if (npc.timeLeft > 100)
            {
                npc.timeLeft = 100;
            }
            npc.rotation += 0.4f * (float)npc.direction;
            return;
        }

        // 10
        private void AICursedSkull(NPC npc, bool flag)
        {
            float num110 = 1f;
            float num111 = 0.011f;
            npc.TargetClosest(true);
            Vector2 vector12 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
            float num112 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector12.X;
            float num113 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector12.Y;
            float num114 = (float)Math.Sqrt((double)(num112 * num112 + num113 * num113));
            float num115 = num114;
            npc.ai[1] += 1f;
            if (npc.ai[1] > 600f)
            {
                num111 *= 8f;
                num110 = 4f;
                if (npc.ai[1] > 650f)
                {
                    npc.ai[1] = 0f;
                }
            }
            else
            {
                if (num115 < 250f)
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
            }
            if (num115 > 350f)
            {
                num110 = 5f;
                num111 = 0.3f;
            }
            else
            {
                if (num115 > 300f)
                {
                    num110 = 3f;
                    num111 = 0.2f;
                }
                else
                {
                    if (num115 > 250f)
                    {
                        num110 = 1.5f;
                        num111 = 0.1f;
                    }
                }
            }
            num114 = num110 / num114;
            num112 *= num114;
            num113 *= num114;
            if (Main.players[npc.target].dead)
            {
                num112 = (float)npc.direction * num110 / 2f;
                num113 = -num110 / 2f;
            }
            if (npc.Velocity.X < num112)
            {
                npc.Velocity.X = npc.Velocity.X + num111;
            }
            else
            {
                if (npc.Velocity.X > num112)
                {
                    npc.Velocity.X = npc.Velocity.X - num111;
                }
            }
            if (npc.Velocity.Y < num113)
            {
                npc.Velocity.Y = npc.Velocity.Y + num111;
            }
            else
            {
                if (npc.Velocity.Y > num113)
                {
                    npc.Velocity.Y = npc.Velocity.Y - num111;
                }
            }
            if (num112 > 0f)
            {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2((double)num113, (double)num112);
            }
            if (num112 < 0f)
            {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2((double)num113, (double)num112) + 3.14f;
                return;
            }
        }

        // 11
        private void AISkeletronHead(NPC npc, bool flag)
        {
            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(true);
                npc.ai[0] = 1f;
                if (npc.Type != 68)
                {
                    int num116 = NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 36, npc.whoAmI);
                    Main.npcs[num116].ai[0] = -1f;
                    Main.npcs[num116].ai[1] = (float)npc.whoAmI;
                    Main.npcs[num116].target = npc.target;
                    Main.npcs[num116].netUpdate = true;
                    num116 = NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 36, npc.whoAmI);
                    Main.npcs[num116].ai[0] = 1f;
                    Main.npcs[num116].ai[1] = (float)npc.whoAmI;
                    Main.npcs[num116].ai[3] = 150f;
                    Main.npcs[num116].target = npc.target;
                    Main.npcs[num116].netUpdate = true;
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
                npc.defense = 10;
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
                    npc.defense = 0;
                    npc.ai[2] += 1f;

                    if (npc.ai[2] >= 400f)
                    {
                        npc.ai[2] = 0f;
                        npc.ai[1] = 0f;
                    }
                    npc.rotation += (float)npc.direction * 0.3f;
                    Vector2 vector13 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                    float num117 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector13.X;
                    float num118 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector13.Y;
                    float num119 = (float)Math.Sqrt((double)(num117 * num117 + num118 * num118));
                    num119 = 1.5f / num119;
                    npc.Velocity.X = num117 * num119;
                    npc.Velocity.Y = num118 * num119;
                }
                else
                {
                    if (npc.ai[1] == 2f)
                    {
                        npc.damage = 9999;
                        npc.defense = 9999;
                        npc.rotation += (float)npc.direction * 0.3f;
                        Vector2 vector14 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                        float num120 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector14.X;
                        float num121 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector14.Y;
                        float num122 = (float)Math.Sqrt((double)(num120 * num120 + num121 * num121));
                        num122 = 8f / num122;
                        npc.Velocity.X = num120 * num122;
                        npc.Velocity.Y = num121 * num122;
                    }
                    else
                    {
                        if (npc.ai[1] == 3f)
                        {
                            npc.Velocity.Y = npc.Velocity.Y + 0.1f;
                            if (npc.Velocity.Y < 0f)
                            {
                                npc.Velocity.Y = npc.Velocity.Y * 0.95f;
                            }
                            npc.Velocity.X = npc.Velocity.X * 0.95f;
                            if (npc.timeLeft > 500)
                            {
                                npc.timeLeft = 500;
                            }
                        }
                    }
                }
            }
            if (npc.ai[1] != 2f && npc.ai[1] != 3f && npc.Type != 68)
            {
                return;
            }
        }

        // 12
        private void AISkeletronHand(NPC npc, bool flag)
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
                Vector2 vector15 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                float num125 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector15.X;
                float num126 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector15.Y;
                Math.Sqrt((double)(num125 * num125 + num126 * num126));
                npc.rotation = (float)Math.Atan2((double)num126, (double)num125) + 1.57f;
                return;
            }
            if (npc.ai[2] == 1f)
            {
                Vector2 vector16 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                float num127 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector16.X;
                float num128 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector16.Y;
                float num129 = (float)Math.Sqrt((double)(num127 * num127 + num128 * num128));
                npc.rotation = (float)Math.Atan2((double)num128, (double)num127) + 1.57f;
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
                    vector16 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                    num127 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector16.X;
                    num128 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector16.Y;
                    num129 = (float)Math.Sqrt((double)(num127 * num127 + num128 * num128));
                    num129 = 18f / num129;
                    npc.Velocity.X = num127 * num129;
                    npc.Velocity.Y = num128 * num129;
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
                        Vector2 vector17 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                        float num130 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector17.X;
                        float num131 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector17.Y;
                        float num132 = (float)Math.Sqrt((double)(num130 * num130 + num131 * num131));
                        npc.rotation = (float)Math.Atan2((double)num131, (double)num130) + 1.57f;
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
                            vector17 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                            num130 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector17.X;
                            num131 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector17.Y;
                            num132 = (float)Math.Sqrt((double)(num130 * num130 + num131 * num131));
                            num132 = 17f / num132;
                            npc.Velocity.X = num130 * num132;
                            npc.Velocity.Y = num131 * num132;
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

        // 13
        private void AIMunchyPlant(NPC npc, bool flag)
        {
            if (!Main.tile.At((int)npc.ai[0], (int)npc.ai[1]).Active)
            {
                npc.life = -1;
                npc.HitEffect(0, 10.0);
                npc.Active = false;
                return;
            }
            npc.TargetClosest(true);
            float num133 = 0.035f;
            float num134 = 150f;
            if (npc.Type == 43)
            {
                num134 = 250f;
            }
            npc.ai[2] += 1f;
            if (npc.ai[2] > 300f)
            {
                num134 = (float)((int)((double)num134 * 1.3));
                if (npc.ai[2] > 450f)
                {
                    npc.ai[2] = 0f;
                }
            }
            Vector2 vector18 = new Vector2(npc.ai[0] * 16f + 8f, npc.ai[1] * 16f + 8f);
            float num135 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - (float)(npc.Width / 2) - vector18.X;
            float num136 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - (float)(npc.Height / 2) - vector18.Y;
            float num137 = (float)Math.Sqrt((double)(num135 * num135 + num136 * num136));
            if (num137 > num134)
            {
                num137 = num134 / num137;
                num135 *= num137;
                num136 *= num137;
            }
            if (npc.Position.X < npc.ai[0] * 16f + 8f + num135)
            {
                npc.Velocity.X = npc.Velocity.X + num133;
                if (npc.Velocity.X < 0f && num135 > 0f)
                {
                    npc.Velocity.X = npc.Velocity.X + num133 * 1.5f;
                }
            }
            else
            {
                if (npc.Position.X > npc.ai[0] * 16f + 8f + num135)
                {
                    npc.Velocity.X = npc.Velocity.X - num133;
                    if (npc.Velocity.X > 0f && num135 < 0f)
                    {
                        npc.Velocity.X = npc.Velocity.X - num133 * 1.5f;
                    }
                }
            }
            if (npc.Position.Y < npc.ai[1] * 16f + 8f + num136)
            {
                npc.Velocity.Y = npc.Velocity.Y + num133;
                if (npc.Velocity.Y < 0f && num136 > 0f)
                {
                    npc.Velocity.Y = npc.Velocity.Y + num133 * 1.5f;
                }
            }
            else
            {
                if (npc.Position.Y > npc.ai[1] * 16f + 8f + num136)
                {
                    npc.Velocity.Y = npc.Velocity.Y - num133;
                    if (npc.Velocity.Y > 0f && num136 < 0f)
                    {
                        npc.Velocity.Y = npc.Velocity.Y - num133 * 1.5f;
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
            if (num135 > 0f)
            {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2((double)num136, (double)num135);
            }
            if (num135 < 0f)
            {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2((double)num136, (double)num135) + 3.14f;
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

        // 14
        private void AIFlyWinged(NPC npc, bool flag)
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
            if (npc.Type == 49 || npc.Type == 51 || npc.Type == 60 || npc.Type == 62 || npc.Type == 66)
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
                if (npc.Type == 60)
                {
                    if (npc.direction == -1 && npc.Velocity.X > -4f)
                    {
                        npc.Velocity.X = npc.Velocity.X - 0.1f;
                        if (npc.Velocity.X > 4f)
                        {
                            npc.Velocity.X = npc.Velocity.X - 0.07f;
                        }
                        else
                        {
                            if (npc.Velocity.X > 0f)
                            {
                                npc.Velocity.X = npc.Velocity.X + 0.03f;
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
                                npc.Velocity.X = npc.Velocity.X + 0.07f;
                            }
                            else
                            {
                                if (npc.Velocity.X < 0f)
                                {
                                    npc.Velocity.X = npc.Velocity.X - 0.03f;
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
                            npc.Velocity.Y = npc.Velocity.Y - 0.03f;
                        }
                        else
                        {
                            if (npc.Velocity.Y > 0f)
                            {
                                npc.Velocity.Y = npc.Velocity.Y + 0.02f;
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
                                npc.Velocity.Y = npc.Velocity.Y + 0.03f;
                            }
                            else
                            {
                                if (npc.Velocity.Y < 0f)
                                {
                                    npc.Velocity.Y = npc.Velocity.Y - 0.02f;
                                }
                            }
                            if ((double)npc.Velocity.Y > 1.5)
                            {
                                npc.Velocity.Y = 1.5f;
                            }
                        }
                    }
                }
                else
                {
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
            }
            npc.ai[1] += 1f;
            if (npc.ai[1] > 200f)
            {
                if (!Main.players[npc.target].wet && Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
                {
                    npc.ai[1] = 0f;
                }
                float num139 = 0.2f;
                float num140 = 0.1f;
                float num141 = 4f;
                float num142 = 1.5f;
                if (npc.Type == 48 || npc.Type == 62 || npc.Type == 66)
                {
                    num139 = 0.12f;
                    num140 = 0.07f;
                    num141 = 3f;
                    num142 = 1.25f;
                }
                if (npc.ai[1] > 1000f)
                {
                    npc.ai[1] = 0f;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] > 0f)
                {
                    if (npc.Velocity.Y < num142)
                    {
                        npc.Velocity.Y = npc.Velocity.Y + num140;
                    }
                }
                else
                {
                    if (npc.Velocity.Y > -num142)
                    {
                        npc.Velocity.Y = npc.Velocity.Y - num140;
                    }
                }
                if (npc.ai[2] < -150f || npc.ai[2] > 150f)
                {
                    if (npc.Velocity.X < num141)
                    {
                        npc.Velocity.X = npc.Velocity.X + num139;
                    }
                }
                else
                {
                    if (npc.Velocity.X > -num141)
                    {
                        npc.Velocity.X = npc.Velocity.X - num139;
                    }
                }
                if (npc.ai[2] > 300f)
                {
                    npc.ai[2] = -300f;
                }
            }
            if (npc.Type == 48)
            {
                npc.ai[0] += 1f;
                if (npc.ai[0] == 30f || npc.ai[0] == 60f || npc.ai[0] == 90f)
                {
                    if (Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
                    {
                        float num143 = 6f;
                        Vector2 vector19 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                        float num144 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width * 0.5f - vector19.X + (float)Main.rand.Next(-100, 101);
                        float num145 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].Height * 0.5f - vector19.Y + (float)Main.rand.Next(-100, 101);
                        float num146 = (float)Math.Sqrt((double)(num144 * num144 + num145 * num145));
                        num146 = num143 / num146;
                        num144 *= num146;
                        num145 *= num146;
                        int num147 = 15;
                        int num148 = 38;
                        int num149 = Projectile.NewProjectile(vector19.X, vector19.Y, num144, num145, num148, num147, 0f, Main.myPlayer);
                        Main.projectile[num149].timeLeft = 300;
                    }
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
                    if (Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
                    {
                        float num150 = 0.2f;
                        Vector2 vector20 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                        float num151 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width * 0.5f - vector20.X + (float)Main.rand.Next(-100, 101);
                        float num152 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].Height * 0.5f - vector20.Y + (float)Main.rand.Next(-100, 101);
                        float num153 = (float)Math.Sqrt((double)(num151 * num151 + num152 * num152));
                        num153 = num150 / num153;
                        num151 *= num153;
                        num152 *= num153;
                        int num154 = 21;
                        int num155 = 44;
                        int num156 = Projectile.NewProjectile(vector20.X, vector20.Y, num151, num152, num155, num154, 0f, Main.myPlayer);
                        Main.projectile[num156].timeLeft = 300;
                        return;
                    }
                }
                else
                {
                    if (npc.ai[0] >= (float)(300 + Main.rand.Next(300)))
                    {
                        npc.ai[0] = 0f;
                        return;
                    }
                }
            }
        }

        // 15
        private void AIKingSlime(NPC npc, bool flag)
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
                float num158 = (float)npc.life / (float)npc.lifeMax;
                num158 = num158 * 0.5f + 0.75f;
                if (num158 != npc.scale)
                {
                    npc.Position.X = npc.Position.X + (float)(npc.Width / 2);
                    npc.Position.Y = npc.Position.Y + (float)npc.Height;
                    npc.scale = num158;
                    npc.Width = (int)(98f * npc.scale);
                    npc.Height = (int)(92f * npc.scale);
                    npc.Position.X = npc.Position.X - (float)(npc.Width / 2);
                    npc.Position.Y = npc.Position.Y - (float)npc.Height;
                }
                int num159 = (int)((double)npc.lifeMax * 0.05);
                if ((float)(npc.life + num159) < npc.ai[3])
                {
                    npc.ai[3] = (float)npc.life;
                    int num160 = Main.rand.Next(1, 4);
                    for (int num161 = 0; num161 < num160; num161++)
                    {
                        int x = (int)(npc.Position.X + (float)Main.rand.Next(npc.Width - 32));
                        int y = (int)(npc.Position.Y + (float)Main.rand.Next(npc.Height - 32));
                        int num162 = NPC.NewNPC(x, y, 1, 0);
                        //Main.npcs[num162].SetDefaults(1, -1f);
                        //Main.npcs[num162] = Registries.NPC.Create(1);
                        Main.npcs[num162].Velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                        Main.npcs[num162].Velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                        Main.npcs[num162].ai[1] = (float)Main.rand.Next(3);
                        if (num162 < MAX_NPCS)
                        {
                            NetMessage.SendData(23, -1, -1, "", num162, 0f, 0f, 0f, 0);
                        }
                    }
                    return;
                }
            }
        }

        // 16
        private void AIFish(NPC npc, bool flag)
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
                bool flag16 = false;
                if (!npc.friendly)
                {
                    npc.TargetClosest(false);
                    if (Main.players[npc.target].wet && !Main.players[npc.target].dead)
                    {
                        flag16 = true;
                    }
                }
                if (flag16)
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
                    int num163 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
                    int num164 = (int)(npc.Position.Y + (float)(npc.Height / 2)) / 16;

                    if (Main.tile.At(num163, num164 - 1).Exists && Main.tile.At(num163, num164 - 1).Liquid > 128)
                    {
                        if (Main.tile.At(num163, num164 + 1).Active)
                        {
                            npc.ai[0] = -1f;
                        }
                        else
                        {
                            if (Main.tile.At(num163, num164 + 2).Active)
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

        // 17
        private void AIVulture(NPC npc, bool flag)
        {
            npc.noGravity = true;
            if (npc.ai[0] == 0f)
            {
                npc.noGravity = false;
                npc.TargetClosest(true);
                if (npc.Velocity.X != 0f || npc.Velocity.Y < 0f || (double)npc.Velocity.Y > 0.3)
                {
                    npc.ai[0] = 1f;
                    npc.netUpdate = true;
                }
                else
                {
                    Rectangle rectangle5 = new Rectangle((int)Main.players[npc.target].Position.X, (int)Main.players[npc.target].Position.Y, Main.players[npc.target].Width, Main.players[npc.target].Height);
                    Rectangle rectangle6 = new Rectangle((int)npc.Position.X - 100, (int)npc.Position.Y - 100, npc.Width + 200, npc.Height + 200);
                    if (rectangle6.Intersects(rectangle5) || npc.life < npc.lifeMax)
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
                    float num165 = Math.Abs(npc.Position.X + (float)(npc.Width / 2) - (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2)));
                    float num166 = Main.players[npc.target].Position.Y - (float)(npc.Height / 2);
                    if (num165 > 50f)
                    {
                        num166 -= 100f;
                    }
                    if (npc.Position.Y < num166)
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

        // 18
        private void AIJellyFish(NPC npc, bool flag)
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
            bool flag17 = false;
            if (!npc.friendly)
            {
                npc.TargetClosest(false);
                if (Main.players[npc.target].wet && !Main.players[npc.target].dead)
                {
                    flag17 = true;
                }
            }
            if (flag17)
            {
                npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) + 1.57f;
                npc.Velocity *= 0.98f;
                float num167 = 0.2f;
                if (npc.Velocity.X > -num167 && npc.Velocity.X < num167 && npc.Velocity.Y > -num167 && npc.Velocity.Y < num167)
                {
                    npc.TargetClosest(true);
                    float num168 = 7f;
                    Vector2 vector21 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
                    float num169 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector21.X;
                    float num170 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector21.Y;
                    float num171 = (float)Math.Sqrt((double)(num169 * num169 + num170 * num170));
                    num171 = num168 / num171;
                    num169 *= num171;
                    num170 *= num171;
                    npc.Velocity.X = num169;
                    npc.Velocity.Y = num170;
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
                int num172 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
                int num173 = (int)(npc.Position.Y + (float)(npc.Height / 2)) / 16;

                if (Main.tile.At(num172, num173 - 1).Exists && Main.tile.At(num172, num173 - 1).Liquid > 128)
                {
                    if (Main.tile.At(num172, num173 + 1).Active)
                    {
                        npc.ai[0] = -1f;
                    }
                    else
                    {
                        if (Main.tile.At(num172, num173 + 2).Active)
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

        // 19
        private void AIAntlion(NPC npc, bool flag)
        {
            npc.TargetClosest(true);
            float num174 = 12f;
            Vector2 vector22 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
            float num175 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector22.X;
            float num176 = Main.players[npc.target].Position.Y - vector22.Y;
            float num177 = (float)Math.Sqrt((double)(num175 * num175 + num176 * num176));
            num177 = num174 / num177;
            num175 *= num177;
            num176 *= num177;
            bool flag18 = false;
            if (npc.directionY < 0)
            {
                npc.rotation = (float)(Math.Atan2((double)num176, (double)num175) + 1.57);
                flag18 = ((double)npc.rotation >= -1.2 && (double)npc.rotation <= 1.2);
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
            if (flag18 && npc.ai[0] == 0f && Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
            {
                npc.ai[0] = 200f;
                int num178 = 10;
                int num179 = 31;
                int num180 = Projectile.NewProjectile(vector22.X, vector22.Y, num175, num176, num179, num178, 0f, Main.myPlayer);
                Main.projectile[num180].ai[0] = 2f;
                Main.projectile[num180].timeLeft = 300;
                Main.projectile[num180].friendly = false;
                NetMessage.SendData(27, -1, -1, "", num180, 0f, 0f, 0f, 0);
                npc.netUpdate = true;
            }
            try
            {
                int num181 = (int)npc.Position.X / 16;
                int num182 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
                int num183 = (int)(npc.Position.X + (float)npc.Width) / 16;
                int num184 = (int)(npc.Position.Y + (float)npc.Height) / 16;
                bool flag19 = false;

                if ((Main.tile.At(num181, num184).Active && Main.tileSolid[
                    (int)Main.tile.At(num181, num184).Type]) ||
                        (Main.tile.At(num182, num184).Active && Main.tileSolid[
                            (int)Main.tile.At(num182, num184).Type]) ||
                                (Main.tile.At(num183, num184).Active && Main.tileSolid[
                                    (int)Main.tile.At(num183, num184).Type]))
                {
                    flag19 = true;
                }
                if (flag19)
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
                return;
            }
            catch
            {
                return;
            }
        }

        // 20
        private void AISpikedBall(NPC npc, bool flag)
        {
            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(true);
                npc.direction *= -1;
                npc.directionY *= -1;
                npc.Position.Y = npc.Position.Y + (float)(npc.Height / 2 + 8);
                npc.ai[1] = npc.Position.X + (float)(npc.Width / 2);
                npc.ai[2] = npc.Position.Y + (float)(npc.Height / 2);
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
                if (npc.directionY == 0)
                {
                    npc.directionY = 1;
                }
                npc.ai[3] = 1f + (float)Main.rand.Next(15) * 0.1f;
                npc.Velocity.Y = (float)(npc.directionY * 6) * npc.ai[3];
                npc.ai[0] += 1f;
                npc.netUpdate = true;
                return;
            }
            else
            {
                float num186 = 6f * npc.ai[3];
                float num187 = 0.2f * npc.ai[3];
                float num188 = num186 / num187 / 2f;
                if (npc.ai[0] >= 1f && npc.ai[0] < (float)((int)num188))
                {
                    npc.Velocity.Y = (float)npc.directionY * num186;
                    npc.ai[0] += 1f;
                    return;
                }
                if (npc.ai[0] >= (float)((int)num188))
                {
                    npc.netUpdate = true;
                    npc.Velocity.Y = 0f;
                    npc.directionY *= -1;
                    npc.Velocity.X = num186 * (float)npc.direction;
                    npc.ai[0] = -1f;
                    return;
                }
                if (npc.directionY > 0)
                {
                    if (npc.Velocity.Y >= num186)
                    {
                        npc.netUpdate = true;
                        npc.directionY *= -1;
                        npc.Velocity.Y = num186;
                    }
                }
                else
                {
                    if (npc.directionY < 0 && npc.Velocity.Y <= -num186)
                    {
                        npc.directionY *= -1;
                        npc.Velocity.Y = -num186;
                    }
                }
                if (npc.direction > 0)
                {
                    if (npc.Velocity.X >= num186)
                    {
                        npc.direction *= -1;
                        npc.Velocity.X = num186;
                    }
                }
                else
                {
                    if (npc.direction < 0 && npc.Velocity.X <= -num186)
                    {
                        npc.direction *= -1;
                        npc.Velocity.X = -num186;
                    }
                }
                npc.Velocity.X = npc.Velocity.X + num187 * (float)npc.direction;
                npc.Velocity.Y = npc.Velocity.Y + num187 * (float)npc.directionY;
                return;
            }
        }

        // 21
        private void AIBlazingWheel(NPC npc, bool flag)
        {
            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(true);
                npc.directionY = 1;
                npc.ai[0] = 1f;
            }
            int num189 = 6;
            if (npc.ai[1] == 0f)
            {
                npc.rotation += (float)(npc.direction * npc.directionY) * 0.13f;
                if (npc.collideY)
                {
                    npc.ai[0] = 2f;
                }
                if (!npc.collideY && npc.ai[0] == 2f)
                {
                    npc.direction = -npc.direction;
                    npc.ai[1] = 1f;
                    npc.ai[0] = 1f;
                }
                if (npc.collideX)
                {
                    npc.directionY = -npc.directionY;
                    npc.ai[1] = 1f;
                }
            }
            else
            {
                npc.rotation -= (float)(npc.direction * npc.directionY) * 0.13f;
                if (npc.collideX)
                {
                    npc.ai[0] = 2f;
                }
                if (!npc.collideX && npc.ai[0] == 2f)
                {
                    npc.directionY = -npc.directionY;
                    npc.ai[1] = 0f;
                    npc.ai[0] = 1f;
                }
                if (npc.collideY)
                {
                    npc.direction = -npc.direction;
                    npc.ai[1] = 0f;
                }
            }
            npc.Velocity.X = (float)(num189 * npc.direction);
            npc.Velocity.Y = (float)(num189 * npc.directionY);
            return;
        }

		// 22
		private void AISlowDebuffFlying(NPC npc, bool flag)
		{
			bool flag25 = false;
			if (npc.justHit)
			{
				npc.ai[2] = 0f;
			}
			if (npc.ai[2] >= 0f)
			{
				int num258 = 16;
				bool flag26 = false;
				bool flag27 = false;
				if (npc.Position.X > npc.ai[0] - (float)num258 && npc.Position.X < npc.ai[0] + (float)num258)
				{
					flag26 = true;
				}
				else
				{
					if ((npc.Velocity.X < 0f && npc.direction > 0) || (npc.Velocity.X > 0f && npc.direction < 0))
					{
						flag26 = true;
					}
				}
				num258 += 24;
				if (npc.Position.Y > npc.ai[1] - (float)num258 && npc.Position.Y < npc.ai[1] + (float)num258)
				{
					flag27 = true;
				}
				if (flag26 && flag27)
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 30f && num258 == 16)
					{
						flag25 = true;
					}
					if (npc.ai[2] >= 60f)
					{
						npc.ai[2] = -200f;
						npc.direction *= -1;
						npc.Velocity.X = npc.Velocity.X * -1f;
						npc.collideX = false;
					}
				}
				else
				{
					npc.ai[0] = npc.Position.X;
					npc.ai[1] = npc.Position.Y;
					npc.ai[2] = 0f;
				}
				npc.TargetClosest(true);
			}
			else
			{
				npc.ai[2] += 1f;
				if (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) > npc.Position.X + (float)(npc.Width / 2))
				{
					npc.direction = -1;
				}
				else
				{
					npc.direction = 1;
				}
			}
			int num259 = (int)((npc.Position.X + (float)(npc.Width / 2)) / 16f) + npc.direction * 2;
			int num260 = (int)((npc.Position.Y + (float)npc.Height) / 16f);
			bool flag28 = true;
			bool flag29 = false;
			int num261 = 3;
			if (npc.Type == 122)
			{
				if (npc.justHit)
				{
					npc.ai[3] = 0f;
					npc.localAI[1] = 0f;
				}
				float num262 = 7f;
				Vector2 vector27 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
				float num263 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector27.X;
				float num264 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector27.Y;
				float num265 = (float)Math.Sqrt((double)(num263 * num263 + num264 * num264));
				num265 = num262 / num265;
				num263 *= num265;
				num264 *= num265;
				if (npc.ai[3] == 32f)
				{
					int num266 = 25;
					int num267 = 84;
					Projectile.NewProjectile(vector27.X, vector27.Y, num263, num264, num267, num266, 0f, Main.myPlayer);
				}
				num261 = 8;
				if (npc.ai[3] > 0f)
				{
					npc.ai[3] += 1f;
					if (npc.ai[3] >= 64f)
					{
						npc.ai[3] = 0f;
					}
				}
				if (npc.ai[3] == 0f)
				{
					npc.localAI[1] += 1f;
					if (npc.localAI[1] > 120f && Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
					{
						npc.localAI[1] = 0f;
						npc.ai[3] = 1f;
						npc.netUpdate = true;
					}
				}
			}
			else if (npc.Type == 75)
					num261 = 4;

			for (int num269 = num260; num269 < num260 + num261; num269++)
			{
				if ((Main.tile.At(num259, num269).Active && Main.tileSolid[(int)Main.tile.At(num259, num269).Type]) || Main.tile.At(num259, num269).Liquid > 0)
				{
					if (num269 <= num260 + 1)flag29 = true;

					flag28 = false;
					break;
				}
			}
			if (flag25)
			{
				flag29 = false;
				flag28 = true;
			}
			if (flag28)
			{
				if (npc.Type == 75)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.2f;
					if (npc.Velocity.Y > 2f)
						npc.Velocity.Y = 2f;
				}
				else
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.1f;
					if (npc.Velocity.Y > 3f)
						npc.Velocity.Y = 3f;
				}
			}
			else
			{
				if (npc.Type == 75)
				{
					if ((npc.directionY < 0 && npc.Velocity.Y > 0f) || flag29)
						npc.Velocity.Y = npc.Velocity.Y - 0.2f;
				}
				else
				{
					if (npc.directionY < 0 && npc.Velocity.Y > 0f)
						npc.Velocity.Y = npc.Velocity.Y - 0.1f;
				}
				if (npc.Velocity.Y < -4f)
					npc.Velocity.Y = -4f;
			}
			if (npc.Type == 75 && npc.wet)
			{
				npc.Velocity.Y = npc.Velocity.Y - 0.2f;
				if (npc.Velocity.Y < -2f)
					npc.Velocity.Y = -2f;
			}
			if (npc.collideX)
			{
				npc.Velocity.X = npc.oldVelocity.X * -0.4f;
				if (npc.direction == -1 && npc.Velocity.X > 0f && npc.Velocity.X < 1f)
					npc.Velocity.X = 1f;

				if (npc.direction == 1 && npc.Velocity.X < 0f && npc.Velocity.X > -1f)
					npc.Velocity.X = -1f;
			}
			if (npc.collideY)
			{
				npc.Velocity.Y = npc.oldVelocity.Y * -0.25f;
				if (npc.Velocity.Y > 0f && npc.Velocity.Y < 1f)
					npc.Velocity.Y = 1f;

				if (npc.Velocity.Y < 0f && npc.Velocity.Y > -1f)
					npc.Velocity.Y = -1f;
			}
			float num270 = 2f;
			if (npc.Type == 75)
			{
				num270 = 3f;
			}
			if (npc.direction == -1 && npc.Velocity.X > -num270)
			{
				npc.Velocity.X = npc.Velocity.X - 0.1f;
				if (npc.Velocity.X > num270)
					npc.Velocity.X = npc.Velocity.X - 0.1f;
				else if (npc.Velocity.X > 0f)
						npc.Velocity.X = npc.Velocity.X + 0.05f;

				if (npc.Velocity.X < -num270)
					npc.Velocity.X = -num270;
			}
			else if (npc.direction == 1 && npc.Velocity.X < num270)
			{
				npc.Velocity.X = npc.Velocity.X + 0.1f;
				if (npc.Velocity.X < -num270)
					npc.Velocity.X = npc.Velocity.X + 0.1f;
				else if (npc.Velocity.X < 0f)
					npc.Velocity.X = npc.Velocity.X - 0.05f;

				if (npc.Velocity.X > num270)
					npc.Velocity.X = num270;
			}
			if (npc.directionY == -1 && (double)npc.Velocity.Y > -1.5)
			{
				npc.Velocity.Y = npc.Velocity.Y - 0.04f;
				if ((double)npc.Velocity.Y > 1.5)
					npc.Velocity.Y = npc.Velocity.Y - 0.05f;
				else if (npc.Velocity.Y > 0f)
					npc.Velocity.Y = npc.Velocity.Y + 0.03f;

				if ((double)npc.Velocity.Y < -1.5)
					npc.Velocity.Y = -1.5f;
			}
			else if (npc.directionY == 1 && (double)npc.Velocity.Y < 1.5)
			{
				npc.Velocity.Y = npc.Velocity.Y + 0.04f;
				if ((double)npc.Velocity.Y < -1.5)
					npc.Velocity.Y = npc.Velocity.Y + 0.05f;
				else if (npc.Velocity.Y < 0f)
					npc.Velocity.Y = npc.Velocity.Y - 0.03f;

				if ((double)npc.Velocity.Y > 1.5)
					npc.Velocity.Y = 1.5f;
			}
		}

		// 23
		private void AITool(NPC npc, bool flag)
		{
			npc.noGravity = true;
			npc.noTileCollide = true;
			if (npc.target < 0 || npc.target == 255 || Main.players[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			if (npc.ai[0] == 0f)
			{
				float num271 = 9f;
				Vector2 vector28 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
				float num272 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector28.X;
				float num273 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector28.Y;
				float num274 = (float)Math.Sqrt((double)(num272 * num272 + num273 * num273));
				num274 = num271 / num274;
				num272 *= num274;
				num273 *= num274;
				npc.Velocity.X = num272;
				npc.Velocity.Y = num273;
				npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) + 0.785f;
				npc.ai[0] = 1f;
				npc.ai[1] = 0f;
				return;
			}
			if (npc.ai[0] == 1f)
			{
				if (npc.justHit)
				{
					npc.ai[0] = 2f;
					npc.ai[1] = 0f;
				}
				npc.Velocity *= 0.99f;
				npc.ai[1] += 1f;
				if (npc.ai[1] >= 100f)
				{
					npc.ai[0] = 2f;
					npc.ai[1] = 0f;
					npc.Velocity.X = 0f;
					npc.Velocity.Y = 0f;
					return;
				}
			}
			else
			{
				if (npc.justHit)
				{
					npc.ai[0] = 2f;
					npc.ai[1] = 0f;
				}
				npc.Velocity *= 0.96f;
				npc.ai[1] += 1f;
				float num275 = npc.ai[1] / 120f;
				num275 = 0.1f + num275 * 0.4f;
				npc.rotation += num275 * (float)npc.direction;
				if (npc.ai[1] >= 120f)
				{
					npc.netUpdate = true;
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					return;
				}
			}
		}

		// 24
		private void AIBird(NPC npc, bool flag)
		{
			npc.noGravity = true;
			if (npc.ai[0] == 0f)
			{
				npc.noGravity = false;
				npc.TargetClosest(true);
				if (npc.Velocity.X != 0f || npc.Velocity.Y < 0f || (double)npc.Velocity.Y > 0.3)
				{
					npc.ai[0] = 1f;
					npc.netUpdate = true;
					npc.direction = -npc.direction;
				}
				else
				{
					Rectangle rectangle7 = new Rectangle((int)Main.players[npc.target].Position.X, 
						(int)Main.players[npc.target].Position.Y, Main.players[npc.target].Width, Main.players[npc.target].Height);
					Rectangle rectangle8 = new Rectangle((int)npc.Position.X - 100, (int)npc.Position.Y - 100, npc.Width + 200, npc.Height + 200);
					if (rectangle8.Intersects(rectangle7) || npc.life < npc.lifeMax)
					{
						npc.ai[0] = 1f;
						npc.Velocity.Y = npc.Velocity.Y - 6f;
						npc.netUpdate = true;
						npc.direction = -npc.direction;
					}
				}
			}
			else
			{
				if (!Main.players[npc.target].dead)
				{
					if (npc.collideX)
					{
						npc.direction *= -1;
						npc.Velocity.X = npc.oldVelocity.X * -0.5f;
						if (npc.direction == -1 && npc.Velocity.X > 0f && npc.Velocity.X < 2f)
							npc.Velocity.X = 2f;
						if (npc.direction == 1 && npc.Velocity.X < 0f && npc.Velocity.X > -2f)
							npc.Velocity.X = -2f;
					}
					if (npc.collideY)
					{
						npc.Velocity.Y = npc.oldVelocity.Y * -0.5f;
						if (npc.Velocity.Y > 0f && npc.Velocity.Y < 1f)
							npc.Velocity.Y = 1f;
						if (npc.Velocity.Y < 0f && npc.Velocity.Y > -1f)
							npc.Velocity.Y = -1f;
					}
					if (npc.direction == -1 && npc.Velocity.X > -3f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.1f;
						if (npc.Velocity.X > 3f)
							npc.Velocity.X = npc.Velocity.X - 0.1f;
						else if (npc.Velocity.X > 0f)
							npc.Velocity.X = npc.Velocity.X - 0.05f;
		
						if (npc.Velocity.X < -3f)
							npc.Velocity.X = -3f;
					}
					else
					{
						if (npc.direction == 1 && npc.Velocity.X < 3f)
						{
							npc.Velocity.X = npc.Velocity.X + 0.1f;
							if (npc.Velocity.X < -3f)
								npc.Velocity.X = npc.Velocity.X + 0.1f;
							else if (npc.Velocity.X < 0f)
								npc.Velocity.X = npc.Velocity.X + 0.05f;

							if (npc.Velocity.X > 3f)
								npc.Velocity.X = 3f;
						}
					}
					int num276 = (int)((npc.Position.X + (float)(npc.Width / 2)) / 16f) + npc.direction;
					int num277 = (int)((npc.Position.Y + (float)npc.Height) / 16f);
					bool flag30 = true;
					int num278 = 15;
					bool flag31 = false;
					for (int num279 = num277; num279 < num277 + num278; num279++)
					{
						if ((Main.tile.At(num276, num279).Active && Main.tileSolid[(int)Main.tile.At(num276, num279).Type]) || Main.tile.At(num276, num279).Liquid > 0)
						{
							if (num279 < num277 + 5)flag31 = true;

							flag30 = false;
							break;
						}
					}
					if (flag30)
						npc.Velocity.Y = npc.Velocity.Y + 0.1f;
					else
						npc.Velocity.Y = npc.Velocity.Y - 0.1f;
					if (flag31)
						npc.Velocity.Y = npc.Velocity.Y - 0.2f;
					if (npc.Velocity.Y > 3f)
						npc.Velocity.Y = 3f;
					if (npc.Velocity.Y < -4f)
						npc.Velocity.Y = -4f;
				}
			}
			if (npc.wet)
			{
				if (npc.Velocity.Y > 0f)
					npc.Velocity.Y = npc.Velocity.Y * 0.95f;

				npc.Velocity.Y = npc.Velocity.Y - 0.5f;
	
				if (npc.Velocity.Y < -4f)
					npc.Velocity.Y = -4f;

				npc.TargetClosest(true);
				return;
			}
		}

		// 25
		private void AIMimic(NPC npc, bool flag)
		{
			if (npc.ai[3] == 0f)
			{
				npc.Position.X = npc.Position.X + 8f;
				if (npc.Position.Y / 16f > (float)(Main.maxTilesY - 200))
				{
					npc.ai[3] = 3f;
				}
				else
				{
					if ((double)(npc.Position.Y / 16f) > Main.worldSurface)
					{
						npc.ai[3] = 2f;
					}
					else
					{
						npc.ai[3] = 1f;
					}
				}
			}
			if (npc.ai[0] == 0f)
			{
				npc.TargetClosest(true);
				if (npc.Velocity.X != 0f || npc.Velocity.Y < 0f || (double)npc.Velocity.Y > 0.3)
				{
					npc.ai[0] = 1f;
					npc.netUpdate = true;
					return;
				}
				Rectangle rectangle9 = new Rectangle((int)Main.players[npc.target].Position.X, (int)Main.players[npc.target].Position.Y, Main.players[npc.target].Width, Main.players[npc.target].Height);
				Rectangle rectangle10 = new Rectangle((int)npc.Position.X - 100, (int)npc.Position.Y - 100, npc.Width + 200, npc.Height + 200);
				if (rectangle10.Intersects(rectangle9) || npc.life < npc.lifeMax)
				{
					npc.ai[0] = 1f;
					npc.netUpdate = true;
					return;
				}
			}
			else
			{
				if (npc.Velocity.Y == 0f)
				{
					npc.ai[2] += 1f;
					int num280 = 20;
					if (npc.ai[1] == 0f)
					{
						num280 = 12;
					}
					if (npc.ai[2] < (float)num280)
					{
						npc.Velocity.X = npc.Velocity.X * 0.9f;
						return;
					}
					npc.ai[2] = 0f;
					npc.TargetClosest(true);
					npc.spriteDirection = npc.direction;
					npc.ai[1] += 1f;
					if (npc.ai[1] == 2f)
					{
						npc.Velocity.X = (float)npc.direction * 2.5f;
						npc.Velocity.Y = -8f;
						npc.ai[1] = 0f;
					}
					else
					{
						npc.Velocity.X = (float)npc.direction * 3.5f;
						npc.Velocity.Y = -4f;
					}
					npc.netUpdate = true;
					return;
				}
				else
				{
					if (npc.direction == 1 && npc.Velocity.X < 1f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.1f;
						return;
					}
					if (npc.direction == -1 && npc.Velocity.X > -1f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.1f;
						return;
					}
				}
			}
		}

		// 26
		private void AIUnicorn(NPC npc, bool flag)
		{
			int num281 = 30;
			bool flag32 = false;
			if (npc.Velocity.Y == 0f && ((npc.Velocity.X > 0f && npc.direction < 0) || (npc.Velocity.X < 0f && npc.direction > 0)))
			{
				flag32 = true;
				npc.ai[3] += 1f;
			}
			if (npc.Position.X == npc.oldPosition.X || npc.ai[3] >= (float)num281 || flag32)
			{
				npc.ai[3] += 1f;
			}
			else
			{
				if (npc.ai[3] > 0f)
				{
					npc.ai[3] -= 1f;
				}
			}
			if (npc.ai[3] > (float)(num281 * 10))
			{
				npc.ai[3] = 0f;
			}
			if (npc.justHit)
			{
				npc.ai[3] = 0f;
			}
			if (npc.ai[3] == (float)num281)
			{
				npc.netUpdate = true;
			}
			if (npc.ai[3] < (float)num281)
			{
				npc.TargetClosest(true);
			}
			else
			{
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
				npc.directionY = -1;
				if (npc.direction == 0)
				{
					npc.direction = 1;
				}
			}
			float num282 = 6f;
			if (npc.Velocity.Y == 0f || npc.wet || (npc.Velocity.X <= 0f && npc.direction < 0) || (npc.Velocity.X >= 0f && npc.direction > 0))
			{
				if (npc.Velocity.X < -num282 || npc.Velocity.X > num282)
				{
					if (npc.Velocity.Y == 0f)
					{
						npc.Velocity *= 0.8f;
					}
				}
				else
				{
					if (npc.Velocity.X < num282 && npc.direction == 1)
					{
						npc.Velocity.X = npc.Velocity.X + 0.07f;
						if (npc.Velocity.X > num282)
						{
							npc.Velocity.X = num282;
						}
					}
					else
					{
						if (npc.Velocity.X > -num282 && npc.direction == -1)
						{
							npc.Velocity.X = npc.Velocity.X - 0.07f;
							if (npc.Velocity.X < -num282)
							{
								npc.Velocity.X = -num282;
							}
						}
					}
				}
			}
			if (npc.Velocity.Y == 0f)
			{
				int num283 = (int)((npc.Position.X + (float)(npc.Width / 2) + (float)((npc.Width / 2 + 2) * npc.direction) + npc.Velocity.X * 5f) / 16f);
				int num284 = (int)((npc.Position.Y + (float)npc.Height - 15f) / 16f);
				if ((npc.Velocity.X < 0f && npc.spriteDirection == -1) || (npc.Velocity.X > 0f && npc.spriteDirection == 1))
				{
					if (Main.tile.At(num283, num284 - 2).Active && Main.tileSolid[(int)Main.tile.At(num283, num284 - 2).Type])
					{
						if (Main.tile.At(num283, num284 - 3).Active && Main.tileSolid[(int)Main.tile.At(num283, num284 - 3).Type])
						{
							npc.Velocity.Y = -8.5f;
							npc.netUpdate = true;
							return;
						}
						npc.Velocity.Y = -7.5f;
						npc.netUpdate = true;
						return;
					}
					else
					{
						if (Main.tile.At(num283, num284 - 1).Active && Main.tileSolid[(int)Main.tile.At(num283, num284 - 1).Type])
						{
							npc.Velocity.Y = -7f;
							npc.netUpdate = true;
							return;
						}
						if (Main.tile.At(num283, num284).Active && Main.tileSolid[(int)Main.tile.At(num283, num284).Type])
						{
							npc.Velocity.Y = -6f;
							npc.netUpdate = true;
							return;
						}
						if ((npc.directionY < 0 || Math.Abs(npc.Velocity.X) > 3f) && (!Main.tile.At(num283, num284 + 1).Active || !Main.tileSolid[(int)Main.tile.At(num283, num284 + 1).Type]) && (!Main.tile.At(num283 + npc.direction, num284 + 1).Active || !Main.tileSolid[(int)Main.tile.At(num283 + npc.direction, num284 + 1).Type]))
						{
							npc.Velocity.Y = -8f;
							npc.netUpdate = true;
							return;
						}
					}
				}
			}
		}

		// 27
		private void AIWallOfFlesh(NPC npc, bool flag)
		{
			if (npc.Position.X < 160f || npc.Position.X > (float)((Main.maxTilesX - 10) * 16))
				npc.Active = false;

			if (npc.localAI[0] == 0f)
			{
				npc.localAI[0] = 1f;
				Main.WallOfFlesh_B = -1;
				Main.WallOfFlesh_T = -1;
			}
			npc.ai[1] += 1f;

			if (npc.ai[2] == 0f)
			{
				if ((double)npc.life < (double)npc.lifeMax * 0.5)
					npc.ai[1] += 1f;

				if ((double)npc.life < (double)npc.lifeMax * 0.2)
					npc.ai[1] += 1f;

				if (npc.ai[1] > 2700f)
					npc.ai[2] = 1f;
			}
			if (npc.ai[2] > 0f && npc.ai[1] > 60f)
			{
				int num285 = 3;
				if ((double)npc.life < (double)npc.lifeMax * 0.3)
					num285++;

				npc.ai[2] += 1f;
				npc.ai[1] = 0f;
				if (npc.ai[2] > (float)num285)
					npc.ai[2] = 0f;
				
				int num286 = NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)(npc.Position.Y + (float)(npc.Height / 2) + 20f), 117, 1);
				Main.npcs[num286].Velocity.X = (float)(npc.direction * 8);
			}
			npc.localAI[3] += 1f;
			if (npc.localAI[3] >= (float)(600 + Main.rand.Next(1000)))
			{
				npc.localAI[3] = (float)(-(float)Main.rand.Next(200));
			}
			Main.WallOfFlesh = npc.whoAmI;
			int num287 = (int)(npc.Position.X / 16f);
			int num288 = (int)((npc.Position.X + (float)npc.Width) / 16f);
			int num289 = (int)((npc.Position.Y + (float)(npc.Height / 2)) / 16f);
			int num290 = 0;
			int tileY = num289 + 7;
			while (num290 < 15 && tileY > Main.maxTilesY - 200)
			{
				tileY++;
				for (int num292 = num287; num292 <= num288; num292++)
				{
					try
					{
						if (WorldModify.SolidTile(num292, tileY) || Main.tile.At(num292, tileY).Liquid > 0)
							num290++;
					}
					catch
					{
						num290 += 15;
					}
				}
			}
			tileY += 4;

			if (Main.WallOfFlesh_B == -1)
				Main.WallOfFlesh_B = tileY * 16;
			else if (Main.WallOfFlesh_B > tileY * 16)
			{
				Main.WallOfFlesh_B--;
				if (Main.WallOfFlesh_B < tileY * 16)
					Main.WallOfFlesh_B = tileY * 16;
			}
			else if (Main.WallOfFlesh_B < tileY * 16)
			{
				Main.WallOfFlesh_B++;
				if (Main.WallOfFlesh_B > tileY * 16)
				{
					Main.WallOfFlesh_B = tileY * 16;
				}
			}
			num290 = 0;
			tileY = num289 - 7;
			while (num290 < 15 && tileY < Main.maxTilesY - 10)
			{
				tileY--;
				for (int tileX = num287; tileX <= num288; tileX++)
				{
					try
					{
						if (WorldModify.SolidTile(tileX, tileY) || Main.tile.At(tileX, tileY).Liquid > 0)
						{
							num290++;
						}
					}
					catch
					{
						num290 += 15;
					}
				}
			}
			tileY -= 4;

			if (Main.WallOfFlesh_T == -1)
				Main.WallOfFlesh_T = tileY * 16;

			else if (Main.WallOfFlesh_T > tileY * 16)
			{
				Main.WallOfFlesh_T--;

				if (Main.WallOfFlesh_T < tileY * 16)
					Main.WallOfFlesh_T = tileY * 16;
			}
			else if (Main.WallOfFlesh_T < tileY * 16)
			{
				Main.WallOfFlesh_T++;

				if (Main.WallOfFlesh_T > tileY * 16)
					Main.WallOfFlesh_T = tileY * 16;
			}

			float num294 = (float)((Main.WallOfFlesh_B + Main.WallOfFlesh_T) / 2 - npc.Height / 2);
			if (npc.Position.Y > num294 + 1f)
				npc.Velocity.Y = -1f;
			else if (npc.Position.Y < num294 - 1f)
					npc.Velocity.Y = 1f;

			npc.Velocity.Y = 0f;
			npc.Position.Y = num294;

			float num295 = 1.5f;
			if ((double)npc.life < (double)npc.lifeMax * 0.75)
			{
				num295 += 0.25f;
			}
			if ((double)npc.life < (double)npc.lifeMax * 0.5)
				num295 += 0.4f;

			if ((double)npc.life < (double)npc.lifeMax * 0.25)
				num295 += 0.5f;

			if ((double)npc.life < (double)npc.lifeMax * 0.1)
				num295 += 0.6f;

			if (npc.Velocity.X == 0f)
			{
				npc.TargetClosest(true);
				npc.Velocity.X = (float)npc.direction;
			}
			if (npc.Velocity.X < 0f)
			{
				npc.Velocity.X = -num295;
				npc.direction = -1;
			}
			else
			{
				npc.Velocity.X = num295;
				npc.direction = 1;
			}
			npc.spriteDirection = npc.direction;
			Vector2 vector29 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
			float num296 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector29.X;
			float num297 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector29.Y;
			float num298 = (float)Math.Sqrt((double)(num296 * num296 + num297 * num297));
			num296 *= num298;
			num297 *= num298;
			if (npc.direction > 0)
			{
				if (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) > npc.Position.X + (float)(npc.Width / 2))
					npc.rotation = (float)Math.Atan2((double)(-(double)num297), (double)(-(double)num296)) + 3.14f;
				else
					npc.rotation = 0f;
			}
			else
			{
				if (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) < npc.Position.X + (float)(npc.Width / 2))
					npc.rotation = (float)Math.Atan2((double)num297, (double)num296) + 3.14f;
				else
					npc.rotation = 0f;
			}
			if (npc.localAI[0] == 1f)
			{
				npc.localAI[0] = 2f;
				num294 = (float)((Main.WallOfFlesh_B + Main.WallOfFlesh_T) / 2);
				num294 = (num294 + (float)Main.WallOfFlesh_T) / 2f;
				int num299 = NPC.NewNPC((int)npc.Position.X, (int)num294, 114, npc.whoAmI);
				Main.npcs[num299].ai[0] = 1f;
				num294 = (float)((Main.WallOfFlesh_B + Main.WallOfFlesh_T) / 2);
				num294 = (num294 + (float)Main.WallOfFlesh_B) / 2f;
				num299 = NPC.NewNPC((int)npc.Position.X, (int)num294, 114, npc.whoAmI);
				Main.npcs[num299].ai[0] = -1f;
				num294 = (float)((Main.WallOfFlesh_B + Main.WallOfFlesh_T) / 2);
				num294 = (num294 + (float)Main.WallOfFlesh_B) / 2f;
				for (int num300 = 0; num300 < 11; num300++)
				{
					num299 = NPC.NewNPC((int)npc.Position.X, (int)num294, 115, npc.whoAmI);
					Main.npcs[num299].ai[0] = (float)num300 * 0.1f - 0.05f;
				}
				return;
			}
		}

		// 28
		private void AIWallOfFlesh_Eye(NPC npc, bool flag)
		{
			if (Main.WallOfFlesh < 0)
			{
				npc.Active = false;
				return;
			}

			npc.realLife = Main.WallOfFlesh;
			npc.TargetClosest(true);
			npc.Position.X = Main.npcs[Main.WallOfFlesh].Position.X;
			npc.direction = Main.npcs[Main.WallOfFlesh].direction;
			npc.spriteDirection = npc.direction;
			float num301 = (float)((Main.WallOfFlesh_B + Main.WallOfFlesh_T) / 2);
			if (npc.ai[0] > 0f)
			{
				num301 = (num301 + (float)Main.WallOfFlesh_T) / 2f;
			}
			else
			{
				num301 = (num301 + (float)Main.WallOfFlesh_B) / 2f;
			}
			num301 -= (float)(npc.Height / 2);
			if (npc.Position.Y > num301 + 1f)
			{
				npc.Velocity.Y = -1f;
			}
			else
			{
				if (npc.Position.Y < num301 - 1f)
				{
					npc.Velocity.Y = 1f;
				}
				else
				{
					npc.Velocity.Y = 0f;
					npc.Position.Y = num301;
				}
			}
			if (npc.Velocity.Y > 5f)
			{
				npc.Velocity.Y = 5f;
			}
			if (npc.Velocity.Y < -5f)
			{
				npc.Velocity.Y = -5f;
			}
			Vector2 vector30 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
			float num302 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector30.X;
			float num303 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector30.Y;
			float num304 = (float)Math.Sqrt((double)(num302 * num302 + num303 * num303));
			num302 *= num304;
			num303 *= num304;
			bool flag33 = true;
			if (npc.direction > 0)
			{
				if (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) > npc.Position.X + (float)(npc.Width / 2))
				{
					npc.rotation = (float)Math.Atan2((double)(-(double)num303), (double)(-(double)num302)) + 3.14f;
				}
				else
				{
					npc.rotation = 0f;
					flag33 = false;
				}
			}
			else
			{
				if (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) < npc.Position.X + (float)(npc.Width / 2))
				{
					npc.rotation = (float)Math.Atan2((double)num303, (double)num302) + 3.14f;
				}
				else
				{
					npc.rotation = 0f;
					flag33 = false;
				}
			}
			int num305 = 4;
			npc.localAI[1] += 1f;
			if ((double)Main.npcs[Main.WallOfFlesh].life < (double)Main.npcs[Main.WallOfFlesh].lifeMax * 0.75)
			{
				npc.localAI[1] += 1f;
				num305++;
			}
			if ((double)Main.npcs[Main.WallOfFlesh].life < (double)Main.npcs[Main.WallOfFlesh].lifeMax * 0.5)
			{
				npc.localAI[1] += 1f;
				num305++;
			}
			if ((double)Main.npcs[Main.WallOfFlesh].life < (double)Main.npcs[Main.WallOfFlesh].lifeMax * 0.25)
			{
				npc.localAI[1] += 1f;
				num305 += 2;
			}
			if ((double)Main.npcs[Main.WallOfFlesh].life < (double)Main.npcs[Main.WallOfFlesh].lifeMax * 0.1)
			{
				npc.localAI[1] += 2f;
				num305 += 3;
			}
			if (npc.localAI[2] == 0f)
			{
				if (npc.localAI[1] > 600f)
				{
					npc.localAI[2] = 1f;
					npc.localAI[1] = 0f;
					return;
				}
			}
			else
			{
				if (npc.localAI[1] > 45f && Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
				{
					npc.localAI[1] = 0f;
					npc.localAI[2] += 1f;
					if (npc.localAI[2] >= (float)num305)
					{
						npc.localAI[2] = 0f;
					}
					if (flag33)
					{
						float num306 = 9f;
						int num307 = 11;
						int num308 = 83;
						if ((double)Main.npcs[Main.WallOfFlesh].life < (double)Main.npcs[Main.WallOfFlesh].lifeMax * 0.5)
						{
							num307++;
							num306 += 1f;
						}
						if ((double)Main.npcs[Main.WallOfFlesh].life < (double)Main.npcs[Main.WallOfFlesh].lifeMax * 0.25)
						{
							num307++;
							num306 += 1f;
						}
						if ((double)Main.npcs[Main.WallOfFlesh].life < (double)Main.npcs[Main.WallOfFlesh].lifeMax * 0.1)
						{
							num307 += 2;
							num306 += 2f;
						}
						vector30 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
						num302 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width * 0.5f - vector30.X;
						num303 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].Height * 0.5f - vector30.Y;
						num304 = (float)Math.Sqrt((double)(num302 * num302 + num303 * num303));
						num304 = num306 / num304;
						num302 *= num304;
						num303 *= num304;
						vector30.X += num302;
						vector30.Y += num303;
						Projectile.NewProjectile(vector30.X, vector30.Y, num302, num303, num308, num307, 0f, Main.myPlayer);
						return;
					}
				}
			}
		}

		// 29
		private void AITheHungry(NPC npc, bool flag)
		{
			if (npc.justHit)
				npc.ai[1] = 10f;

			if (Main.WallOfFlesh < 0)
			{
				npc.Active = false;
				return;
			}

			npc.TargetClosest(true);
			float num309 = 0.1f;
			float num310 = 300f;
			if ((double)Main.npcs[Main.WallOfFlesh].life < (double)Main.npcs[Main.WallOfFlesh].lifeMax * 0.25)
			{
				npc.damage = 75;
				npc.defense = 40;
				num310 = 900f;
			}
			else if ((double)Main.npcs[Main.WallOfFlesh].life < (double)Main.npcs[Main.WallOfFlesh].lifeMax * 0.5)
			{
				npc.damage = 60;
				npc.defense = 30;
				num310 = 700f;
			}
			else if ((double)Main.npcs[Main.WallOfFlesh].life < (double)Main.npcs[Main.WallOfFlesh].lifeMax * 0.75)
			{
				npc.damage = 45;
				npc.defense = 20;
				num310 = 500f;
			}

			float num311 = Main.npcs[Main.WallOfFlesh].Position.X + (float)(Main.npcs[Main.WallOfFlesh].Width / 2);
			float num312 = Main.npcs[Main.WallOfFlesh].Position.Y;
			float num313 = (float)(Main.WallOfFlesh_B - Main.WallOfFlesh_T);
			num312 = (float)Main.WallOfFlesh_T + num313 * npc.ai[0];
			npc.ai[2] += 1f;
			if (npc.ai[2] > 100f)
			{
				num310 = (float)((int)(num310 * 1.3f));

				if (npc.ai[2] > 200f)
					npc.ai[2] = 0f;
			}

			Vector2 vector31 = new Vector2(num311, num312);
			float num314 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - (float)(npc.Width / 2) - vector31.X;
			float num315 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - (float)(npc.Height / 2) - vector31.Y;
			float num316 = (float)Math.Sqrt((double)(num314 * num314 + num315 * num315));
			if (npc.ai[1] == 0f)
			{
				if (num316 > num310)
				{
					num316 = num310 / num316;
					num314 *= num316;
					num315 *= num316;
				}
				if (npc.Position.X < num311 + num314)
				{
					npc.Velocity.X = npc.Velocity.X + num309;

					if (npc.Velocity.X < 0f && num314 > 0f)
						npc.Velocity.X = npc.Velocity.X + num309 * 2.5f;
				}
				else if (npc.Position.X > num311 + num314)
				{
					npc.Velocity.X = npc.Velocity.X - num309;

					if (npc.Velocity.X > 0f && num314 < 0f)
						npc.Velocity.X = npc.Velocity.X - num309 * 2.5f;
				}
				if (npc.Position.Y < num312 + num315)
				{
					npc.Velocity.Y = npc.Velocity.Y + num309;

					if (npc.Velocity.Y < 0f && num315 > 0f)
						npc.Velocity.Y = npc.Velocity.Y + num309 * 2.5f;
				}
				else if (npc.Position.Y > num312 + num315)
				{
					npc.Velocity.Y = npc.Velocity.Y - num309;

					if (npc.Velocity.Y > 0f && num315 < 0f)
						npc.Velocity.Y = npc.Velocity.Y - num309 * 2.5f;
				}

				if (npc.Velocity.X > 4f)
					npc.Velocity.X = 4f;
				if (npc.Velocity.X < -4f)
					npc.Velocity.X = -4f;
				if (npc.Velocity.Y > 4f)
					npc.Velocity.Y = 4f;
				if (npc.Velocity.Y < -4f)
					npc.Velocity.Y = -4f;
			}
			else
			{
				if (npc.ai[1] > 0f)
					npc.ai[1] -= 1f;
				else
					npc.ai[1] = 0f;
			}
	
			if (num314 > 0f)
			{
				npc.spriteDirection = 1;
				npc.rotation = (float)Math.Atan2((double)num315, (double)num314);
			}
			if (num314 < 0f)
			{
				npc.spriteDirection = -1;
				npc.rotation = (float)Math.Atan2((double)num315, (double)num314) + 3.14f;
			}
		}

		// 30
		private void AIRetinazer(NPC npc, bool flag)
		{
			if (npc.target < 0 || npc.target == 255 || Main.players[npc.target].dead || !Main.players[npc.target].Active)
			{
				npc.TargetClosest(true);
			}
			bool dead2 = Main.players[npc.target].dead;
			float num317 = npc.Position.X + (float)(npc.Width / 2) - Main.players[npc.target].Position.X - (float)(Main.players[npc.target].Width / 2);
			float num318 = npc.Position.Y + (float)npc.Height - 59f - Main.players[npc.target].Position.Y - (float)(Main.players[npc.target].Height / 2);
			float num319 = (float)Math.Atan2((double)num318, (double)num317) + 1.57f;
			if (num319 < 0f)
			{
				num319 += 6.283f;
			}
			else
			{
				if ((double)num319 > 6.283)
				{
					num319 -= 6.283f;
				}
			}
			float num320 = 0.1f;
			if (npc.rotation < num319)
			{
				if ((double)(num319 - npc.rotation) > 3.1415)
				{
					npc.rotation -= num320;
				}
				else
				{
					npc.rotation += num320;
				}
			}
			else
			{
				if (npc.rotation > num319)
				{
					if ((double)(npc.rotation - num319) > 3.1415)
					{
						npc.rotation += num320;
					}
					else
					{
						npc.rotation -= num320;
					}
				}
			}
			if (npc.rotation > num319 - num320 && npc.rotation < num319 + num320)
			{
				npc.rotation = num319;
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
			if (npc.rotation > num319 - num320 && npc.rotation < num319 + num320)
			{
				npc.rotation = num319;
			}
			if (Main.dayTime || dead2)
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
						float num322 = 7f;
						float num323 = 0.1f;
						int num324 = 1;
						if (npc.Position.X + (float)(npc.Width / 2) < Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width)
						{
							num324 = -1;
						}
						Vector2 vector32 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
						float num325 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) + (float)(num324 * 300) - vector32.X;
						float num326 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - 300f - vector32.Y;
						float num327 = (float)Math.Sqrt((double)(num325 * num325 + num326 * num326));
						float num328 = num327;
						num327 = num322 / num327;
						num325 *= num327;
						num326 *= num327;
						if (npc.Velocity.X < num325)
						{
							npc.Velocity.X = npc.Velocity.X + num323;
							if (npc.Velocity.X < 0f && num325 > 0f)
							{
								npc.Velocity.X = npc.Velocity.X + num323;
							}
						}
						else
						{
							if (npc.Velocity.X > num325)
							{
								npc.Velocity.X = npc.Velocity.X - num323;
								if (npc.Velocity.X > 0f && num325 < 0f)
								{
									npc.Velocity.X = npc.Velocity.X - num323;
								}
							}
						}
						if (npc.Velocity.Y < num326)
						{
							npc.Velocity.Y = npc.Velocity.Y + num323;
							if (npc.Velocity.Y < 0f && num326 > 0f)
							{
								npc.Velocity.Y = npc.Velocity.Y + num323;
							}
						}
						else
						{
							if (npc.Velocity.Y > num326)
							{
								npc.Velocity.Y = npc.Velocity.Y - num323;
								if (npc.Velocity.Y > 0f && num326 < 0f)
								{
									npc.Velocity.Y = npc.Velocity.Y - num323;
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
							if (npc.Position.Y + (float)npc.Height < Main.players[npc.target].Position.Y && num328 < 400f)
							{
								if (!Main.players[npc.target].dead)
								{
									npc.ai[3] += 1f;
								}
								if (npc.ai[3] >= 60f)
								{
									npc.ai[3] = 0f;
									vector32 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
									num325 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector32.X;
									num326 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector32.Y;

									float num329 = 9f;
									int num330 = 20;
									int num331 = 83;
									num327 = (float)Math.Sqrt((double)(num325 * num325 + num326 * num326));
									num327 = num329 / num327;
									num325 *= num327;
									num326 *= num327;
									num325 += (float)Main.rand.Next(-40, 41) * 0.08f;
									num326 += (float)Main.rand.Next(-40, 41) * 0.08f;
									vector32.X += num325 * 15f;
									vector32.Y += num326 * 15f;
									Projectile.NewProjectile(vector32.X, vector32.Y, num325, num326, num331, num330, 0f, Main.myPlayer);
								}
							}
						}
					}
					else
					{
						if (npc.ai[1] == 1f)
						{
							npc.rotation = num319;
							float num332 = 12f;
							Vector2 vector33 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
							float num333 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector33.X;
							float num334 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector33.Y;
							float num335 = (float)Math.Sqrt((double)(num333 * num333 + num334 * num334));
							num335 = num332 / num335;
							npc.Velocity.X = num333 * num335;
							npc.Velocity.Y = num334 * num335;
							npc.ai[1] = 2f;
						}
						else
						{
							if (npc.ai[1] == 2f)
							{
								npc.ai[2] += 1f;
								if (npc.ai[2] >= 25f)
								{
									npc.Velocity.X = npc.Velocity.X * 0.96f;
									npc.Velocity.Y = npc.Velocity.Y * 0.96f;
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
								if (npc.ai[2] >= 70f)
								{
									npc.ai[3] += 1f;
									npc.ai[2] = 0f;
									npc.target = 255;
									npc.rotation = num319;
									if (npc.ai[3] >= 4f)
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
								npc.ai[2] = 0f;
						}
						Vector2 arg_1529F_0 = npc.Position;
						int arg_1529F_1 = npc.Width;
						int arg_1529F_2 = npc.Height;
						int arg_1529F_3 = 5;
						float arg_1529F_4 = (float)Main.rand.Next(-30, 31) * 0.2f;
						float arg_1529F_5 = (float)Main.rand.Next(-30, 31) * 0.2f;
						int arg_1529F_6 = 0;
						newColor = default(Color);

						npc.Velocity.X = npc.Velocity.X * 0.98f;
						npc.Velocity.Y = npc.Velocity.Y * 0.98f;

						if ((double)npc.Velocity.X > -0.1 && (double)npc.Velocity.X < 0.1)
							npc.Velocity.X = 0f;

						if ((double)npc.Velocity.Y > -0.1 && (double)npc.Velocity.Y < 0.1)
						{
							npc.Velocity.Y = 0f;
							return;
						}
					}
					else
					{
						npc.damage = (int)((double)npc.defDamage * 1.5);
						npc.defense = npc.defDefense + 15;
						//npc.soundHit = 4;
						if (npc.ai[1] == 0f)
						{
							float num338 = 8f;
							float num339 = 0.15f;
							Vector2 vector34 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
							float num340 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector34.X;
							float num341 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - 300f - vector34.Y;
							float num342 = (float)Math.Sqrt((double)(num340 * num340 + num341 * num341));
							num342 = num338 / num342;
							num340 *= num342;
							num341 *= num342;
							if (npc.Velocity.X < num340)
							{
								npc.Velocity.X = npc.Velocity.X + num339;

								if (npc.Velocity.X < 0f && num340 > 0f)
									npc.Velocity.X = npc.Velocity.X + num339;
							}
							else
							{
								if (npc.Velocity.X > num340)
								{
									npc.Velocity.X = npc.Velocity.X - num339;

									if (npc.Velocity.X > 0f && num340 < 0f)
										npc.Velocity.X = npc.Velocity.X - num339;
								}
							}
							if (npc.Velocity.Y < num341)
							{
								npc.Velocity.Y = npc.Velocity.Y + num339;
								if (npc.Velocity.Y < 0f && num341 > 0f)
								{
									npc.Velocity.Y = npc.Velocity.Y + num339;
								}
							}
							else
							{
								if (npc.Velocity.Y > num341)
								{
									npc.Velocity.Y = npc.Velocity.Y - num339;
									if (npc.Velocity.Y > 0f && num341 < 0f)
									{
										npc.Velocity.Y = npc.Velocity.Y - num339;
									}
								}
							}
							npc.ai[2] += 1f;
							if (npc.ai[2] >= 300f)
							{
								npc.ai[1] = 1f;
								npc.ai[2] = 0f;
								npc.ai[3] = 0f;
								npc.TargetClosest(true);
								npc.netUpdate = true;
							}
							vector34 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
							num340 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector34.X;
							num341 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector34.Y;
							npc.rotation = (float)Math.Atan2((double)num341, (double)num340) - 1.57f;

							npc.localAI[1] += 1f;
							if ((double)npc.life < (double)npc.lifeMax * 0.75)
							{
								npc.localAI[1] += 1f;
							}
							if ((double)npc.life < (double)npc.lifeMax * 0.5)
							{
								npc.localAI[1] += 1f;
							}
							if ((double)npc.life < (double)npc.lifeMax * 0.25)
							{
								npc.localAI[1] += 1f;
							}
							if ((double)npc.life < (double)npc.lifeMax * 0.1)
							{
								npc.localAI[1] += 2f;
							}
							if (npc.localAI[1] > 140f && Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
							{
								npc.localAI[1] = 0f;
								float num343 = 9f;
								int num344 = 25;
								int num345 = 100;
								num342 = (float)Math.Sqrt((double)(num340 * num340 + num341 * num341));
								num342 = num343 / num342;
								num340 *= num342;
								num341 *= num342;
								vector34.X += num340 * 15f;
								vector34.Y += num341 * 15f;
								Projectile.NewProjectile(vector34.X, vector34.Y, num340, num341, num345, num344, 0f, Main.myPlayer);
								return;
							}
						}
						else
						{
							int num346 = 1;
							if (npc.Position.X + (float)(npc.Width / 2) < Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width)
							{
								num346 = -1;
							}
							float num347 = 8f;
							float num348 = 0.2f;
							Vector2 vector35 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
							float num349 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) + (float)(num346 * 340) - vector35.X;
							float num350 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector35.Y;
							float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
							num351 = num347 / num351;
							num349 *= num351;
							num350 *= num351;
							if (npc.Velocity.X < num349)
							{
								npc.Velocity.X = npc.Velocity.X + num348;
								if (npc.Velocity.X < 0f && num349 > 0f)
								{
									npc.Velocity.X = npc.Velocity.X + num348;
								}
							}
							else
							{
								if (npc.Velocity.X > num349)
								{
									npc.Velocity.X = npc.Velocity.X - num348;
									if (npc.Velocity.X > 0f && num349 < 0f)
									{
										npc.Velocity.X = npc.Velocity.X - num348;
									}
								}
							}
							if (npc.Velocity.Y < num350)
							{
								npc.Velocity.Y = npc.Velocity.Y + num348;
								if (npc.Velocity.Y < 0f && num350 > 0f)
								{
									npc.Velocity.Y = npc.Velocity.Y + num348;
								}
							}
							else
							{
								if (npc.Velocity.Y > num350)
								{
									npc.Velocity.Y = npc.Velocity.Y - num348;
									if (npc.Velocity.Y > 0f && num350 < 0f)
									{
										npc.Velocity.Y = npc.Velocity.Y - num348;
									}
								}
							}
							vector35 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
							num349 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector35.X;
							num350 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector35.Y;
							npc.rotation = (float)Math.Atan2((double)num350, (double)num349) - 1.57f;

							npc.localAI[1] += 1f;
							if ((double)npc.life < (double)npc.lifeMax * 0.75)
							{
								npc.localAI[1] += 1f;
							}
							if ((double)npc.life < (double)npc.lifeMax * 0.5)
							{
								npc.localAI[1] += 1f;
							}
							if ((double)npc.life < (double)npc.lifeMax * 0.25)
							{
								npc.localAI[1] += 1f;
							}
							if ((double)npc.life < (double)npc.lifeMax * 0.1)
							{
								npc.localAI[1] += 2f;
							}
							if (npc.localAI[1] > 45f && Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
							{
								npc.localAI[1] = 0f;
								float num352 = 9f;
								int num353 = 20;
								int num354 = 100;
								num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
								num351 = num352 / num351;
								num349 *= num351;
								num350 *= num351;
								vector35.X += num349 * 15f;
								vector35.Y += num350 * 15f;
								Projectile.NewProjectile(vector35.X, vector35.Y, num349, num350, num354, num353, 0f, Main.myPlayer);
							}
							npc.ai[2] += 1f;
							if (npc.ai[2] >= 200f)
							{
								npc.ai[1] = 0f;
								npc.ai[2] = 0f;
								npc.ai[3] = 0f;
								npc.TargetClosest(true);
								npc.netUpdate = true;
								return;
							}
						}
					}
				}
			}
		}

		// 31
		private void AISpazmatism(NPC npc, bool flag)
		{
			if (npc.target < 0 || npc.target == 255 || Main.players[npc.target].dead || !Main.players[npc.target].Active)
			{
				npc.TargetClosest(true);
			}
			bool dead3 = Main.players[npc.target].dead;
			float num355 = npc.Position.X + (float)(npc.Width / 2) - Main.players[npc.target].Position.X - (float)(Main.players[npc.target].Width / 2);
			float num356 = npc.Position.Y + (float)npc.Height - 59f - Main.players[npc.target].Position.Y - (float)(Main.players[npc.target].Height / 2);
			float num357 = (float)Math.Atan2((double)num356, (double)num355) + 1.57f;

			if (num357 < 0f)
				num357 += 6.283f;
			else if ((double)num357 > 6.283)
				num357 -= 6.283f;

			float num358 = 0.15f;
			if (npc.rotation < num357)
			{
				if ((double)(num357 - npc.rotation) > 3.1415)
					npc.rotation -= num358;
				else
					npc.rotation += num358;
			}
			else if (npc.rotation > num357)
			{
				if ((double)(npc.rotation - num357) > 3.1415)
					npc.rotation += num358;
				else
					npc.rotation -= num358;
			}
			if (npc.rotation > num357 - num358 && npc.rotation < num357 + num358)
				npc.rotation = num357;

			if (npc.rotation < 0f)
				npc.rotation += 6.283f;
			else if ((double)npc.rotation > 6.283)
				npc.rotation -= 6.283f;

			if (npc.rotation > num357 - num358 && npc.rotation < num357 + num358)
				npc.rotation = num357;

			if (Main.dayTime || dead3)
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
						npc.TargetClosest(true);
						float num360 = 12f;
						float num361 = 0.4f;
						int num362 = 1;
						if (npc.Position.X + (float)(npc.Width / 2) < Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width)
							num362 = -1;

						Vector2 vector36 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
						float num363 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) + (float)(num362 * 400) - vector36.X;
						float num364 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector36.Y;
						float num365 = (float)Math.Sqrt((double)(num363 * num363 + num364 * num364));
						num365 = num360 / num365;
						num363 *= num365;
						num364 *= num365;
						if (npc.Velocity.X < num363)
						{
							npc.Velocity.X = npc.Velocity.X + num361;
							if (npc.Velocity.X < 0f && num363 > 0f)
								npc.Velocity.X = npc.Velocity.X + num361;
						}
						else
						{
							if (npc.Velocity.X > num363)
							{
								npc.Velocity.X = npc.Velocity.X - num361;
								if (npc.Velocity.X > 0f && num363 < 0f)
									npc.Velocity.X = npc.Velocity.X - num361;
							}
						}
						if (npc.Velocity.Y < num364)
						{
							npc.Velocity.Y = npc.Velocity.Y + num361;
							if (npc.Velocity.Y < 0f && num364 > 0f)
								npc.Velocity.Y = npc.Velocity.Y + num361;
						}
						else
						{
							if (npc.Velocity.Y > num364)
							{
								npc.Velocity.Y = npc.Velocity.Y - num361;
								if (npc.Velocity.Y > 0f && num364 < 0f)
									npc.Velocity.Y = npc.Velocity.Y - num361;
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
							if (!Main.players[npc.target].dead)
								npc.ai[3] += 1f;

							if (npc.ai[3] >= 60f)
							{
								npc.ai[3] = 0f;
								vector36 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
								num363 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector36.X;
								num364 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector36.Y;

								float num366 = 12f;
								int num367 = 25;
								int num368 = 96;
								num365 = (float)Math.Sqrt((double)(num363 * num363 + num364 * num364));
								num365 = num366 / num365;
								num363 *= num365;
								num364 *= num365;
								num363 += (float)Main.rand.Next(-40, 41) * 0.05f;
								num364 += (float)Main.rand.Next(-40, 41) * 0.05f;
								vector36.X += num363 * 4f;
								vector36.Y += num364 * 4f;
								Projectile.NewProjectile(vector36.X, vector36.Y, num363, num364, num368, num367, 0f, Main.myPlayer);
							}
						}
					}
					else
					{
						if (npc.ai[1] == 1f)
						{
							npc.rotation = num357;
							float num369 = 13f;
							Vector2 vector37 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
							float num370 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector37.X;
							float num371 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector37.Y;
							float num372 = (float)Math.Sqrt((double)(num370 * num370 + num371 * num371));
							num372 = num369 / num372;
							npc.Velocity.X = num370 * num372;
							npc.Velocity.Y = num371 * num372;
							npc.ai[1] = 2f;
						}
						else
						{
							if (npc.ai[1] == 2f)
							{
								npc.ai[2] += 1f;
								if (npc.ai[2] >= 8f)
								{
									npc.Velocity.X = npc.Velocity.X * 0.9f;
									npc.Velocity.Y = npc.Velocity.Y * 0.9f;
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
								if (npc.ai[2] >= 42f)
								{
									npc.ai[3] += 1f;
									npc.ai[2] = 0f;
									npc.target = 255;
									npc.rotation = num357;
									if (npc.ai[3] >= 10f)
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
						//npc.soundHit = 4;
						npc.damage = (int)((double)npc.defDamage * 1.5);
						npc.defense = npc.defDefense + 25;
						if (npc.ai[1] == 0f)
						{
							float num375 = 4f;
							float num376 = 0.1f;
							int num377 = 1;
							if (npc.Position.X + (float)(npc.Width / 2) < Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width)
							{
								num377 = -1;
							}
							Vector2 vector38 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
							float num378 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) + (float)(num377 * 180) - vector38.X;
							float num379 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector38.Y;
							float num380 = (float)Math.Sqrt((double)(num378 * num378 + num379 * num379));
							num380 = num375 / num380;
							num378 *= num380;
							num379 *= num380;
							if (npc.Velocity.X < num378)
							{
								npc.Velocity.X = npc.Velocity.X + num376;
								if (npc.Velocity.X < 0f && num378 > 0f)
								{
									npc.Velocity.X = npc.Velocity.X + num376;
								}
							}
							else
							{
								if (npc.Velocity.X > num378)
								{
									npc.Velocity.X = npc.Velocity.X - num376;
									if (npc.Velocity.X > 0f && num378 < 0f)
									{
										npc.Velocity.X = npc.Velocity.X - num376;
									}
								}
							}
							if (npc.Velocity.Y < num379)
							{
								npc.Velocity.Y = npc.Velocity.Y + num376;
								if (npc.Velocity.Y < 0f && num379 > 0f)
								{
									npc.Velocity.Y = npc.Velocity.Y + num376;
								}
							}
							else
							{
								if (npc.Velocity.Y > num379)
								{
									npc.Velocity.Y = npc.Velocity.Y - num376;
									if (npc.Velocity.Y > 0f && num379 < 0f)
									{
										npc.Velocity.Y = npc.Velocity.Y - num376;
									}
								}
							}
							npc.ai[2] += 1f;
							if (npc.ai[2] >= 400f)
							{
								npc.ai[1] = 1f;
								npc.ai[2] = 0f;
								npc.ai[3] = 0f;
								npc.target = 255;
								npc.netUpdate = true;
							}
							if (Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
							{
								npc.localAI[2] += 1f;
								if (npc.localAI[2] > 22f)
								{
									npc.localAI[2] = 0f;
								}
								npc.localAI[1] += 1f;
								if ((double)npc.life < (double)npc.lifeMax * 0.75)
								{
									npc.localAI[1] += 1f;
								}
								if ((double)npc.life < (double)npc.lifeMax * 0.5)
								{
									npc.localAI[1] += 1f;
								}
								if ((double)npc.life < (double)npc.lifeMax * 0.25)
								{
									npc.localAI[1] += 1f;
								}
								if ((double)npc.life < (double)npc.lifeMax * 0.1)
								{
									npc.localAI[1] += 2f;
								}
								if (npc.localAI[1] > 8f)
								{
									npc.localAI[1] = 0f;
									float num381 = 6f;
									int num382 = 30;
									int num383 = 101;
									vector38 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
									num378 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector38.X;
									num379 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector38.Y;
									num380 = (float)Math.Sqrt((double)(num378 * num378 + num379 * num379));
									num380 = num381 / num380;
									num378 *= num380;
									num379 *= num380;
									num379 += (float)Main.rand.Next(-40, 41) * 0.01f;
									num378 += (float)Main.rand.Next(-40, 41) * 0.01f;
									num379 += npc.Velocity.Y * 0.5f;
									num378 += npc.Velocity.X * 0.5f;
									vector38.X -= num378 * 1f;
									vector38.Y -= num379 * 1f;
									Projectile.NewProjectile(vector38.X, vector38.Y, num378, num379, num383, num382, 0f, Main.myPlayer);
									return;
								}
							}
						}
						else
						{
							if (npc.ai[1] == 1f)
							{
								npc.rotation = num357;
								float num384 = 14f;
								Vector2 vector39 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
								float num385 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector39.X;
								float num386 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector39.Y;
								float num387 = (float)Math.Sqrt((double)(num385 * num385 + num386 * num386));
								num387 = num384 / num387;
								npc.Velocity.X = num385 * num387;
								npc.Velocity.Y = num386 * num387;
								npc.ai[1] = 2f;
								return;
							}
							if (npc.ai[1] == 2f)
							{
								npc.ai[2] += 1f;
								if (npc.ai[2] >= 50f)
								{
									npc.Velocity.X = npc.Velocity.X * 0.93f;
									npc.Velocity.Y = npc.Velocity.Y * 0.93f;
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
								if (npc.ai[2] >= 80f)
								{
									npc.ai[3] += 1f;
									npc.ai[2] = 0f;
									npc.target = 255;
									npc.rotation = num357;
									if (npc.ai[3] >= 6f)
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

		// 32
		private void AISkeletronPrime(NPC npc, bool flag)
		{
			npc.damage = npc.defDamage;
			npc.defense = npc.defDefense;
			if (npc.ai[0] == 0f)
			{
				npc.TargetClosest(true);
				npc.ai[0] = 1f;
				if (npc.Type != 68)
				{
					int num388 = NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 128, npc.whoAmI);
					Main.npcs[num388].ai[0] = -1f;
					Main.npcs[num388].ai[1] = (float)npc.whoAmI;
					Main.npcs[num388].target = npc.target;
					Main.npcs[num388].netUpdate = true;
					num388 = NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 129, npc.whoAmI);
					Main.npcs[num388].ai[0] = 1f;
					Main.npcs[num388].ai[1] = (float)npc.whoAmI;
					Main.npcs[num388].target = npc.target;
					Main.npcs[num388].netUpdate = true;
					num388 = NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 130, npc.whoAmI);
					Main.npcs[num388].ai[0] = -1f;
					Main.npcs[num388].ai[1] = (float)npc.whoAmI;
					Main.npcs[num388].target = npc.target;
					Main.npcs[num388].ai[3] = 150f;
					Main.npcs[num388].netUpdate = true;
					num388 = NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 131, npc.whoAmI);
					Main.npcs[num388].ai[0] = 1f;
					Main.npcs[num388].ai[1] = (float)npc.whoAmI;
					Main.npcs[num388].target = npc.target;
					Main.npcs[num388].netUpdate = true;
					Main.npcs[num388].ai[3] = 150f;
				}
			}
			if (npc.Type == 68 && npc.ai[1] != 3f && npc.ai[1] != 2f)
			{
				npc.ai[1] = 2f;
			}
			if (Main.players[npc.target].dead || Math.Abs(npc.Position.X - Main.players[npc.target].Position.X) > 6000f || Math.Abs(npc.Position.Y - Main.players[npc.target].Position.Y) > 6000f)
			{
				npc.TargetClosest(true);
				if (Main.players[npc.target].dead || Math.Abs(npc.Position.X - Main.players[npc.target].Position.X) > 6000f || Math.Abs(npc.Position.Y - Main.players[npc.target].Position.Y) > 6000f)
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
				if (npc.ai[2] >= 600f)
				{
					npc.ai[2] = 0f;
					npc.ai[1] = 1f;
					npc.TargetClosest(true);
					npc.netUpdate = true;
				}
				npc.rotation = npc.Velocity.X / 15f;
				if (npc.Position.Y > Main.players[npc.target].Position.Y - 200f)
				{
					if (npc.Velocity.Y > 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y * 0.98f;
					}
					npc.Velocity.Y = npc.Velocity.Y - 0.1f;
					if (npc.Velocity.Y > 2f)
					{
						npc.Velocity.Y = 2f;
					}
				}
				else
				{
					if (npc.Position.Y < Main.players[npc.target].Position.Y - 500f)
					{
						if (npc.Velocity.Y < 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y * 0.98f;
						}
						npc.Velocity.Y = npc.Velocity.Y + 0.1f;
						if (npc.Velocity.Y < -2f)
						{
							npc.Velocity.Y = -2f;
						}
					}
				}
				if (npc.Position.X + (float)(npc.Width / 2) > Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) + 100f)
				{
					if (npc.Velocity.X > 0f)
					{
						npc.Velocity.X = npc.Velocity.X * 0.98f;
					}
					npc.Velocity.X = npc.Velocity.X - 0.1f;
					if (npc.Velocity.X > 8f)
					{
						npc.Velocity.X = 8f;
					}
				}
				if (npc.Position.X + (float)(npc.Width / 2) < Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - 100f)
				{
					if (npc.Velocity.X < 0f)
					{
						npc.Velocity.X = npc.Velocity.X * 0.98f;
					}
					npc.Velocity.X = npc.Velocity.X + 0.1f;
					if (npc.Velocity.X < -8f)
					{
						npc.Velocity.X = -8f;
						return;
					}
				}
			}
			else
			{
				if (npc.ai[1] == 1f)
				{
					npc.defense *= 2;
					npc.damage *= 2;
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 400f)
					{
						npc.ai[2] = 0f;
						npc.ai[1] = 0f;
					}
					npc.rotation += (float)npc.direction * 0.3f;
					Vector2 vector40 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num389 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector40.X;
					float num390 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector40.Y;
					float num391 = (float)Math.Sqrt((double)(num389 * num389 + num390 * num390));
					num391 = 2f / num391;
					npc.Velocity.X = num389 * num391;
					npc.Velocity.Y = num390 * num391;
					return;
				}
				if (npc.ai[1] == 2f)
				{
					npc.damage = 9999;
					npc.defense = 9999;
					npc.rotation += (float)npc.direction * 0.3f;
					Vector2 vector41 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num392 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector41.X;
					float num393 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector41.Y;
					float num394 = (float)Math.Sqrt((double)(num392 * num392 + num393 * num393));
					num394 = 8f / num394;
					npc.Velocity.X = num392 * num394;
					npc.Velocity.Y = num393 * num394;
					return;
				}
				if (npc.ai[1] == 3f)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.1f;
					if (npc.Velocity.Y < 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y * 0.95f;
					}
					npc.Velocity.X = npc.Velocity.X * 0.95f;
					if (npc.timeLeft > 500)
					{
						npc.timeLeft = 500;
						return;
					}
				}
			}
		}

		// 33
		private void AIPrimeSaw(NPC npc, bool flag)
		{
			Vector2 vector42 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
			float num395 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector42.X;
			float num396 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector42.Y;
			float num397 = (float)Math.Sqrt((double)(num395 * num395 + num396 * num396));
			if (npc.ai[2] != 99f)
			{
				if (num397 > 800f)
				{
					npc.ai[2] = 99f;
				}
			}
			else
			{
				if (num397 < 400f)
				{
					npc.ai[2] = 0f;
				}
			}
			npc.spriteDirection = -(int)npc.ai[0];
			if (!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != 32)
			{
				npc.ai[2] += 10f;
			}
			if (npc.ai[2] == 99f)
			{
				if (npc.Position.Y > Main.npcs[(int)npc.ai[1]].Position.Y)
				{
					if (npc.Velocity.Y > 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y * 0.96f;
					}
					npc.Velocity.Y = npc.Velocity.Y - 0.1f;
					if (npc.Velocity.Y > 8f)
					{
						npc.Velocity.Y = 8f;
					}
				}
				else
				{
					if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y)
					{
						if (npc.Velocity.Y < 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y * 0.96f;
						}
						npc.Velocity.Y = npc.Velocity.Y + 0.1f;
						if (npc.Velocity.Y < -8f)
						{
							npc.Velocity.Y = -8f;
						}
					}
				}
				if (npc.Position.X + (float)(npc.Width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2))
				{
					if (npc.Velocity.X > 0f)
					{
						npc.Velocity.X = npc.Velocity.X * 0.96f;
					}
					npc.Velocity.X = npc.Velocity.X - 0.5f;
					if (npc.Velocity.X > 12f)
					{
						npc.Velocity.X = 12f;
					}
				}
				if (npc.Position.X + (float)(npc.Width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2))
				{
					if (npc.Velocity.X < 0f)
					{
						npc.Velocity.X = npc.Velocity.X * 0.96f;
					}
					npc.Velocity.X = npc.Velocity.X + 0.5f;
					if (npc.Velocity.X < -12f)
					{
						npc.Velocity.X = -12f;
						return;
					}
				}
			}
			else
			{
				if (npc.ai[2] == 0f || npc.ai[2] == 3f)
				{
					if (Main.npcs[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
					{
						npc.timeLeft = 10;
					}
					if (Main.npcs[(int)npc.ai[1]].ai[1] != 0f)
					{
						npc.TargetClosest(true);
						if (Main.players[npc.target].dead)
						{
							npc.Velocity.Y = npc.Velocity.Y + 0.1f;
							if (npc.Velocity.Y > 16f)
							{
								npc.Velocity.Y = 16f;
							}
						}
						else
						{
							Vector2 vector43 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
							float num398 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector43.X;
							float num399 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector43.Y;
							float num400 = (float)Math.Sqrt((double)(num398 * num398 + num399 * num399));
							num400 = 7f / num400;
							num398 *= num400;
							num399 *= num400;
							npc.rotation = (float)Math.Atan2((double)num399, (double)num398) - 1.57f;
							if (npc.Velocity.X > num398)
							{
								if (npc.Velocity.X > 0f)
								{
									npc.Velocity.X = npc.Velocity.X * 0.97f;
								}
								npc.Velocity.X = npc.Velocity.X - 0.05f;
							}
							if (npc.Velocity.X < num398)
							{
								if (npc.Velocity.X < 0f)
								{
									npc.Velocity.X = npc.Velocity.X * 0.97f;
								}
								npc.Velocity.X = npc.Velocity.X + 0.05f;
							}
							if (npc.Velocity.Y > num399)
							{
								if (npc.Velocity.Y > 0f)
								{
									npc.Velocity.Y = npc.Velocity.Y * 0.97f;
								}
								npc.Velocity.Y = npc.Velocity.Y - 0.05f;
							}
							if (npc.Velocity.Y < num399)
							{
								if (npc.Velocity.Y < 0f)
								{
									npc.Velocity.Y = npc.Velocity.Y * 0.97f;
								}
								npc.Velocity.Y = npc.Velocity.Y + 0.05f;
							}
						}
						npc.ai[3] += 1f;
						if (npc.ai[3] >= 600f)
						{
							npc.ai[2] = 0f;
							npc.ai[3] = 0f;
							npc.netUpdate = true;
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
						if (npc.Position.Y > Main.npcs[(int)npc.ai[1]].Position.Y + 320f)
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
							if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y + 260f)
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
						if (npc.Position.X + (float)(npc.Width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2))
						{
							if (npc.Velocity.X > 0f)
							{
								npc.Velocity.X = npc.Velocity.X * 0.96f;
							}
							npc.Velocity.X = npc.Velocity.X - 0.3f;
							if (npc.Velocity.X > 12f)
							{
								npc.Velocity.X = 12f;
							}
						}
						if (npc.Position.X + (float)(npc.Width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 250f)
						{
							if (npc.Velocity.X < 0f)
							{
								npc.Velocity.X = npc.Velocity.X * 0.96f;
							}
							npc.Velocity.X = npc.Velocity.X + 0.3f;
							if (npc.Velocity.X < -12f)
							{
								npc.Velocity.X = -12f;
							}
						}
					}
					Vector2 vector44 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num401 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector44.X;
					float num402 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector44.Y;
					Math.Sqrt((double)(num401 * num401 + num402 * num402));
					npc.rotation = (float)Math.Atan2((double)num402, (double)num401) + 1.57f;
					return;
				}
				if (npc.ai[2] == 1f)
				{
					Vector2 vector45 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num403 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector45.X;
					float num404 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector45.Y;
					float num405 = (float)Math.Sqrt((double)(num403 * num403 + num404 * num404));
					npc.rotation = (float)Math.Atan2((double)num404, (double)num403) + 1.57f;
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
						vector45 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
						num403 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector45.X;
						num404 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector45.Y;
						num405 = (float)Math.Sqrt((double)(num403 * num403 + num404 * num404));
						num405 = 22f / num405;
						npc.Velocity.X = num403 * num405;
						npc.Velocity.Y = num404 * num405;
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
							npc.TargetClosest(true);
							Vector2 vector46 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
							float num406 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector46.X;
							float num407 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector46.Y;
							float num408 = (float)Math.Sqrt((double)(num406 * num406 + num407 * num407));
							num408 = 7f / num408;
							num406 *= num408;
							num407 *= num408;
							if (npc.Velocity.X > num406)
							{
								if (npc.Velocity.X > 0f)
								{
									npc.Velocity.X = npc.Velocity.X * 0.97f;
								}
								npc.Velocity.X = npc.Velocity.X - 0.05f;
							}
							if (npc.Velocity.X < num406)
							{
								if (npc.Velocity.X < 0f)
								{
									npc.Velocity.X = npc.Velocity.X * 0.97f;
								}
								npc.Velocity.X = npc.Velocity.X + 0.05f;
							}
							if (npc.Velocity.Y > num407)
							{
								if (npc.Velocity.Y > 0f)
								{
									npc.Velocity.Y = npc.Velocity.Y * 0.97f;
								}
								npc.Velocity.Y = npc.Velocity.Y - 0.05f;
							}
							if (npc.Velocity.Y < num407)
							{
								if (npc.Velocity.Y < 0f)
								{
									npc.Velocity.Y = npc.Velocity.Y * 0.97f;
								}
								npc.Velocity.Y = npc.Velocity.Y + 0.05f;
							}
							npc.ai[3] += 1f;
							if (npc.ai[3] >= 600f)
							{
								npc.ai[2] = 0f;
								npc.ai[3] = 0f;
								npc.netUpdate = true;
							}
							vector46 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
							num406 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector46.X;
							num407 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector46.Y;
							num408 = (float)Math.Sqrt((double)(num406 * num406 + num407 * num407));
							npc.rotation = (float)Math.Atan2((double)num407, (double)num406) + 1.57f;
							return;
						}
						if (npc.ai[2] == 5f && ((npc.Velocity.X > 0f && npc.Position.X + (float)(npc.Width / 2) > Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2)) || (npc.Velocity.X < 0f && npc.Position.X + (float)(npc.Width / 2) < Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2))))
						{
							npc.ai[2] = 0f;
							return;
						}
					}
				}
			}
		}

		// 34
		private void AIPrimeVice(NPC npc, bool flag)
		{
			npc.spriteDirection = -(int)npc.ai[0];
			Vector2 vector47 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
			float num409 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector47.X;
			float num410 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector47.Y;
			float num411 = (float)Math.Sqrt((double)(num409 * num409 + num410 * num410));
			if (npc.ai[2] != 99f)
			{
				if (num411 > 800f)
				{
					npc.ai[2] = 99f;
				}
			}
			else
			{
				if (num411 < 400f)
				{
					npc.ai[2] = 0f;
				}
			}
			if (!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != 32)
			{
				npc.ai[2] += 10f;
			}
			if (npc.ai[2] == 99f)
			{
				if (npc.Position.Y > Main.npcs[(int)npc.ai[1]].Position.Y)
				{
					if (npc.Velocity.Y > 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y * 0.96f;
					}
					npc.Velocity.Y = npc.Velocity.Y - 0.1f;
					if (npc.Velocity.Y > 8f)
					{
						npc.Velocity.Y = 8f;
					}
				}
				else
				{
					if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y)
					{
						if (npc.Velocity.Y < 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y * 0.96f;
						}
						npc.Velocity.Y = npc.Velocity.Y + 0.1f;
						if (npc.Velocity.Y < -8f)
						{
							npc.Velocity.Y = -8f;
						}
					}
				}
				if (npc.Position.X + (float)(npc.Width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2))
				{
					if (npc.Velocity.X > 0f)
					{
						npc.Velocity.X = npc.Velocity.X * 0.96f;
					}
					npc.Velocity.X = npc.Velocity.X - 0.5f;
					if (npc.Velocity.X > 12f)
					{
						npc.Velocity.X = 12f;
					}
				}
				if (npc.Position.X + (float)(npc.Width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2))
				{
					if (npc.Velocity.X < 0f)
					{
						npc.Velocity.X = npc.Velocity.X * 0.96f;
					}
					npc.Velocity.X = npc.Velocity.X + 0.5f;
					if (npc.Velocity.X < -12f)
					{
						npc.Velocity.X = -12f;
						return;
					}
				}
			}
			else
			{
				if (npc.ai[2] == 0f || npc.ai[2] == 3f)
				{
					if (Main.npcs[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
					{
						npc.timeLeft = 10;
					}
					if (Main.npcs[(int)npc.ai[1]].ai[1] != 0f)
					{
						npc.TargetClosest(true);
						npc.TargetClosest(true);
						if (Main.players[npc.target].dead)
						{
							npc.Velocity.Y = npc.Velocity.Y + 0.1f;
							if (npc.Velocity.Y > 16f)
							{
								npc.Velocity.Y = 16f;
							}
						}
						else
						{
							Vector2 vector48 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
							float num412 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector48.X;
							float num413 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector48.Y;
							float num414 = (float)Math.Sqrt((double)(num412 * num412 + num413 * num413));
							num414 = 12f / num414;
							num412 *= num414;
							num413 *= num414;
							npc.rotation = (float)Math.Atan2((double)num413, (double)num412) - 1.57f;
							if (Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y) < 2f)
							{
								npc.rotation = (float)Math.Atan2((double)num413, (double)num412) - 1.57f;
								npc.Velocity.X = num412;
								npc.Velocity.Y = num413;
								npc.netUpdate = true;
							}
							else
							{
								npc.Velocity *= 0.97f;
							}
							npc.ai[3] += 1f;
							if (npc.ai[3] >= 600f)
							{
								npc.ai[2] = 0f;
								npc.ai[3] = 0f;
								npc.netUpdate = true;
							}
						}
					}
					else
					{
						npc.ai[3] += 1f;
						if (npc.ai[3] >= 600f)
						{
							npc.ai[2] += 1f;
							npc.ai[3] = 0f;
							npc.netUpdate = true;
						}
						if (npc.Position.Y > Main.npcs[(int)npc.ai[1]].Position.Y + 300f)
						{
							if (npc.Velocity.Y > 0f)
							{
								npc.Velocity.Y = npc.Velocity.Y * 0.96f;
							}
							npc.Velocity.Y = npc.Velocity.Y - 0.1f;
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
								npc.Velocity.Y = npc.Velocity.Y + 0.1f;
								if (npc.Velocity.Y < -3f)
								{
									npc.Velocity.Y = -3f;
								}
							}
						}
						if (npc.Position.X + (float)(npc.Width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) + 250f)
						{
							if (npc.Velocity.X > 0f)
							{
								npc.Velocity.X = npc.Velocity.X * 0.94f;
							}
							npc.Velocity.X = npc.Velocity.X - 0.3f;
							if (npc.Velocity.X > 9f)
							{
								npc.Velocity.X = 9f;
							}
						}
						if (npc.Position.X + (float)(npc.Width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2))
						{
							if (npc.Velocity.X < 0f)
							{
								npc.Velocity.X = npc.Velocity.X * 0.94f;
							}
							npc.Velocity.X = npc.Velocity.X + 0.2f;
							if (npc.Velocity.X < -8f)
							{
								npc.Velocity.X = -8f;
							}
						}
					}
					Vector2 vector49 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num415 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector49.X;
					float num416 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector49.Y;
					Math.Sqrt((double)(num415 * num415 + num416 * num416));
					npc.rotation = (float)Math.Atan2((double)num416, (double)num415) + 1.57f;
					return;
				}
				if (npc.ai[2] == 1f)
				{
					if (npc.Velocity.Y > 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y * 0.9f;
					}
					Vector2 vector50 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num417 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 280f * npc.ai[0] - vector50.X;
					float num418 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector50.Y;
					float num419 = (float)Math.Sqrt((double)(num417 * num417 + num418 * num418));
					npc.rotation = (float)Math.Atan2((double)num418, (double)num417) + 1.57f;
					npc.Velocity.X = (npc.Velocity.X * 5f + Main.npcs[(int)npc.ai[1]].Velocity.X) / 6f;
					npc.Velocity.X = npc.Velocity.X + 0.5f;
					npc.Velocity.Y = npc.Velocity.Y - 0.5f;
					if (npc.Velocity.Y < -9f)
					{
						npc.Velocity.Y = -9f;
					}
					if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y - 280f)
					{
						npc.TargetClosest(true);
						npc.ai[2] = 2f;
						vector50 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
						num417 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector50.X;
						num418 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector50.Y;
						num419 = (float)Math.Sqrt((double)(num417 * num417 + num418 * num418));
						num419 = 20f / num419;
						npc.Velocity.X = num417 * num419;
						npc.Velocity.Y = num418 * num419;
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
							if (npc.ai[3] >= 4f)
							{
								npc.ai[2] = 3f;
								npc.ai[3] = 0f;
								return;
							}
							npc.ai[2] = 1f;
							npc.ai[3] += 1f;
							return;
						}
					}
					else
					{
						if (npc.ai[2] == 4f)
						{
							Vector2 vector51 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
							float num420 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector51.X;
							float num421 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector51.Y;
							float num422 = (float)Math.Sqrt((double)(num420 * num420 + num421 * num421));
							npc.rotation = (float)Math.Atan2((double)num421, (double)num420) + 1.57f;
							npc.Velocity.Y = (npc.Velocity.Y * 5f + Main.npcs[(int)npc.ai[1]].Velocity.Y) / 6f;
							npc.Velocity.X = npc.Velocity.X + 0.5f;
							if (npc.Velocity.X > 12f)
							{
								npc.Velocity.X = 12f;
							}
							if (npc.Position.X + (float)(npc.Width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 500f || npc.Position.X + (float)(npc.Width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) + 500f)
							{
								npc.TargetClosest(true);
								npc.ai[2] = 5f;
								vector51 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
								num420 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector51.X;
								num421 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector51.Y;
								num422 = (float)Math.Sqrt((double)(num420 * num420 + num421 * num421));
								num422 = 17f / num422;
								npc.Velocity.X = num420 * num422;
								npc.Velocity.Y = num421 * num422;
								npc.netUpdate = true;
								return;
							}
						}
						else
						{
							if (npc.ai[2] == 5f && npc.Position.X + (float)(npc.Width / 2) < Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - 100f)
							{
								if (npc.ai[3] >= 4f)
								{
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;
									return;
								}
								npc.ai[2] = 4f;
								npc.ai[3] += 1f;
								return;
							}
						}
					}
				}
			}
		}

		// 35
		private void AIPrimeCannon(NPC npc, bool flag)
		{
			npc.spriteDirection = -(int)npc.ai[0];
			if (!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != 32)
			{
				npc.ai[2] += 10f;
			}
			if (npc.ai[2] == 0f)
			{
				if (Main.npcs[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
				}
				if (Main.npcs[(int)npc.ai[1]].ai[1] != 0f)
				{
					npc.localAI[0] += 2f;
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
					if (npc.ai[3] >= 1100f)
					{
						npc.localAI[0] = 0f;
						npc.ai[2] = 1f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					if (npc.Position.Y > Main.npcs[(int)npc.ai[1]].Position.Y - 150f)
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
						if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y - 150f)
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
					if (npc.Position.X + (float)(npc.Width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) + 200f)
					{
						if (npc.Velocity.X > 0f)
						{
							npc.Velocity.X = npc.Velocity.X * 0.96f;
						}
						npc.Velocity.X = npc.Velocity.X - 0.2f;
						if (npc.Velocity.X > 8f)
						{
							npc.Velocity.X = 8f;
						}
					}
					if (npc.Position.X + (float)(npc.Width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) + 160f)
					{
						if (npc.Velocity.X < 0f)
						{
							npc.Velocity.X = npc.Velocity.X * 0.96f;
						}
						npc.Velocity.X = npc.Velocity.X + 0.2f;
						if (npc.Velocity.X < -8f)
						{
							npc.Velocity.X = -8f;
						}
					}
				}
				Vector2 vector52 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
				float num423 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector52.X;
				float num424 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector52.Y;
				float num425 = (float)Math.Sqrt((double)(num423 * num423 + num424 * num424));
				npc.rotation = (float)Math.Atan2((double)num424, (double)num423) + 1.57f;

				npc.localAI[0] += 1f;
				if (npc.localAI[0] > 140f)
				{
					npc.localAI[0] = 0f;
					float num426 = 12f;
					int num427 = 0;
					int num428 = 102;
					num425 = num426 / num425;
					num423 = -num423 * num425;
					num424 = -num424 * num425;
					num423 += (float)Main.rand.Next(-40, 41) * 0.01f;
					num424 += (float)Main.rand.Next(-40, 41) * 0.01f;
					vector52.X += num423 * 4f;
					vector52.Y += num424 * 4f;
					Projectile.NewProjectile(vector52.X, vector52.Y, num423, num424, num428, num427, 0f, Main.myPlayer);
					return;
				}
			}
			else
			{
				if (npc.ai[2] == 1f)
				{
					npc.ai[3] += 1f;
					if (npc.ai[3] >= 300f)
					{
						npc.localAI[0] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					Vector2 vector53 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num429 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - vector53.X;
					float num430 = Main.npcs[(int)npc.ai[1]].Position.Y - vector53.Y;
					num430 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - 80f - vector53.Y;
					float num431 = (float)Math.Sqrt((double)(num429 * num429 + num430 * num430));
					num431 = 6f / num431;
					num429 *= num431;
					num430 *= num431;
					if (npc.Velocity.X > num429)
					{
						if (npc.Velocity.X > 0f)
						{
							npc.Velocity.X = npc.Velocity.X * 0.9f;
						}
						npc.Velocity.X = npc.Velocity.X - 0.04f;
					}
					if (npc.Velocity.X < num429)
					{
						if (npc.Velocity.X < 0f)
						{
							npc.Velocity.X = npc.Velocity.X * 0.9f;
						}
						npc.Velocity.X = npc.Velocity.X + 0.04f;
					}
					if (npc.Velocity.Y > num430)
					{
						if (npc.Velocity.Y > 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y * 0.9f;
						}
						npc.Velocity.Y = npc.Velocity.Y - 0.08f;
					}
					if (npc.Velocity.Y < num430)
					{
						if (npc.Velocity.Y < 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y * 0.9f;
						}
						npc.Velocity.Y = npc.Velocity.Y + 0.08f;
					}
					npc.TargetClosest(true);
					vector53 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					num429 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector53.X;
					num430 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector53.Y;
					num431 = (float)Math.Sqrt((double)(num429 * num429 + num430 * num430));
					npc.rotation = (float)Math.Atan2((double)num430, (double)num429) - 1.57f;

					npc.localAI[0] += 1f;
					if (npc.localAI[0] > 40f)
					{
						npc.localAI[0] = 0f;
						float num432 = 10f;
						int num433 = 0;
						int num434 = 102;
						num431 = num432 / num431;
						num429 *= num431;
						num430 *= num431;
						num429 += (float)Main.rand.Next(-40, 41) * 0.01f;
						num430 += (float)Main.rand.Next(-40, 41) * 0.01f;
						vector53.X += num429 * 4f;
						vector53.Y += num430 * 4f;
						Projectile.NewProjectile(vector53.X, vector53.Y, num429, num430, num434, num433, 0f, Main.myPlayer);
						return;
					}
				}
			}
		}

		// 36
		private void AIPrimeLaser(NPC npc, bool flag)
		{
			npc.spriteDirection = -(int)npc.ai[0];
			if (!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != 32)
			{
				npc.ai[2] += 10f;
			}
			if (npc.ai[2] == 0f || npc.ai[2] == 3f)
			{
				if (Main.npcs[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
				}
				if (Main.npcs[(int)npc.ai[1]].ai[1] != 0f)
				{
					npc.localAI[0] += 3f;
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
					if (npc.ai[3] >= 800f)
					{
						npc.ai[2] += 1f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					if (npc.Position.Y > Main.npcs[(int)npc.ai[1]].Position.Y - 100f)
					{
						if (npc.Velocity.Y > 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y * 0.96f;
						}
						npc.Velocity.Y = npc.Velocity.Y - 0.1f;
						if (npc.Velocity.Y > 3f)
						{
							npc.Velocity.Y = 3f;
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
							npc.Velocity.Y = npc.Velocity.Y + 0.1f;
							if (npc.Velocity.Y < -3f)
							{
								npc.Velocity.Y = -3f;
							}
						}
					}
					if (npc.Position.X + (float)(npc.Width / 2) > Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 180f * npc.ai[0])
					{
						if (npc.Velocity.X > 0f)
						{
							npc.Velocity.X = npc.Velocity.X * 0.96f;
						}
						npc.Velocity.X = npc.Velocity.X - 0.14f;
						if (npc.Velocity.X > 8f)
						{
							npc.Velocity.X = 8f;
						}
					}
					if (npc.Position.X + (float)(npc.Width / 2) < Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 180f * npc.ai[0])
					{
						if (npc.Velocity.X < 0f)
						{
							npc.Velocity.X = npc.Velocity.X * 0.96f;
						}
						npc.Velocity.X = npc.Velocity.X + 0.14f;
						if (npc.Velocity.X < -8f)
						{
							npc.Velocity.X = -8f;
						}
					}
				}
				npc.TargetClosest(true);
				Vector2 vector54 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
				float num435 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector54.X;
				float num436 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector54.Y;
				float num437 = (float)Math.Sqrt((double)(num435 * num435 + num436 * num436));
				npc.rotation = (float)Math.Atan2((double)num436, (double)num435) - 1.57f;
				npc.localAI[0] += 1f;
				if (npc.localAI[0] > 200f)
				{
					npc.localAI[0] = 0f;
					float num438 = 8f;
					int num439 = 25;
					int num440 = 100;
					num437 = num438 / num437;
					num435 *= num437;
					num436 *= num437;
					num435 += (float)Main.rand.Next(-40, 41) * 0.05f;
					num436 += (float)Main.rand.Next(-40, 41) * 0.05f;
					vector54.X += num435 * 8f;
					vector54.Y += num436 * 8f;
					Projectile.NewProjectile(vector54.X, vector54.Y, num435, num436, num440, num439, 0f, Main.myPlayer);
					return;
				}
			}
			else
			{
				if (npc.ai[2] == 1f)
				{
					npc.ai[3] += 1f;
					if (npc.ai[3] >= 200f)
					{
						npc.localAI[0] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					Vector2 vector55 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num441 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - 350f - vector55.X;
					float num442 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - 20f - vector55.Y;
					float num443 = (float)Math.Sqrt((double)(num441 * num441 + num442 * num442));
					num443 = 7f / num443;
					num441 *= num443;
					num442 *= num443;
					if (npc.Velocity.X > num441)
					{
						if (npc.Velocity.X > 0f)
						{
							npc.Velocity.X = npc.Velocity.X * 0.9f;
						}
						npc.Velocity.X = npc.Velocity.X - 0.1f;
					}
					if (npc.Velocity.X < num441)
					{
						if (npc.Velocity.X < 0f)
						{
							npc.Velocity.X = npc.Velocity.X * 0.9f;
						}
						npc.Velocity.X = npc.Velocity.X + 0.1f;
					}
					if (npc.Velocity.Y > num442)
					{
						if (npc.Velocity.Y > 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y * 0.9f;
						}
						npc.Velocity.Y = npc.Velocity.Y - 0.03f;
					}
					if (npc.Velocity.Y < num442)
					{
						if (npc.Velocity.Y < 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y * 0.9f;
						}
						npc.Velocity.Y = npc.Velocity.Y + 0.03f;
					}
					npc.TargetClosest(true);
					vector55 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					num441 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector55.X;
					num442 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector55.Y;
					num443 = (float)Math.Sqrt((double)(num441 * num441 + num442 * num442));
					npc.rotation = (float)Math.Atan2((double)num442, (double)num441) - 1.57f;

					npc.localAI[0] += 1f;
					if (npc.localAI[0] > 80f)
					{
						npc.localAI[0] = 0f;
						float num444 = 10f;
						int num445 = 25;
						int num446 = 100;
						num443 = num444 / num443;
						num441 *= num443;
						num442 *= num443;
						num441 += (float)Main.rand.Next(-40, 41) * 0.05f;
						num442 += (float)Main.rand.Next(-40, 41) * 0.05f;
						vector55.X += num441 * 8f;
						vector55.Y += num442 * 8f;
						Projectile.NewProjectile(vector55.X, vector55.Y, num441, num442, num446, num445, 0f, Main.myPlayer);
						return;
					}
				}
			}
		}

		// 37
		private void AITheDestroyer(NPC npc, bool flag)
		{
			if (npc.ai[3] > 0f)
			{
				npc.realLife = (int)npc.ai[3];
			}
			if (npc.target < 0 || npc.target == 255 || Main.players[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			if (npc.Type > 134)
			{
				bool flag34 = false;
				if (npc.ai[1] <= 0f)
				{
					flag34 = true;
				}
				else
				{
					if (Main.npcs[(int)npc.ai[1]].life <= 0)
					{
						flag34 = true;
					}
				}
				if (flag34)
				{
					npc.life = 0;
					npc.HitEffect(0, 10.0);
					npc.checkDead();
				}
			}
			if (npc.ai[0] == 0f && npc.Type == 134)
			{
				npc.ai[3] = (float)npc.whoAmI;
				npc.realLife = npc.whoAmI;
				int num447 = npc.whoAmI;
				int num448 = 80;
				for (int num449 = 0; num449 <= num448; num449++)
				{
					int num450 = 135;
					if (num449 == num448)
					{
						num450 = 136;
					}
					int num451 = NPC.NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)(npc.Position.Y + (float)npc.Height), num450, npc.whoAmI);
					Main.npcs[num451].ai[3] = (float)npc.whoAmI;
					Main.npcs[num451].realLife = npc.whoAmI;
					Main.npcs[num451].ai[1] = (float)num447;
					Main.npcs[num447].ai[0] = (float)num451;
					NetMessage.SendData(23, -1, -1, "", num451, 0f, 0f, 0f, 0);
					num447 = num451;
				}
			}
			if (npc.Type == 135)
			{
				npc.localAI[0] += (float)Main.rand.Next(4);
				if (npc.localAI[0] >= (float)Main.rand.Next(1400, 26000))
				{
					npc.localAI[0] = 0f;
					npc.TargetClosest(true);
					if (Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
					{
						float num452 = 8f;
						Vector2 vector56 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)(npc.Height / 2));
						float num453 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width * 0.5f - vector56.X + (float)Main.rand.Next(-20, 21);
						float num454 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].Height * 0.5f - vector56.Y + (float)Main.rand.Next(-20, 21);
						float num455 = (float)Math.Sqrt((double)(num453 * num453 + num454 * num454));
						num455 = num452 / num455;
						num453 *= num455;
						num454 *= num455;
						num453 += (float)Main.rand.Next(-20, 21) * 0.05f;
						num454 += (float)Main.rand.Next(-20, 21) * 0.05f;
						int num456 = 22;
						int num457 = 100;
						vector56.X += num453 * 5f;
						vector56.Y += num454 * 5f;
						int num458 = Projectile.NewProjectile(vector56.X, vector56.Y, num453, num454, num457, num456, 0f, Main.myPlayer);
						Main.projectile[num458].timeLeft = 300;
						npc.netUpdate = true;
					}
				}
			}
			int num459 = (int)(npc.Position.X / 16f) - 1;
			int num460 = (int)((npc.Position.X + (float)npc.Width) / 16f) + 2;
			int num461 = (int)(npc.Position.Y / 16f) - 1;
			int num462 = (int)((npc.Position.Y + (float)npc.Height) / 16f) + 2;
			if (num459 < 0)
			{
				num459 = 0;
			}
			if (num460 > Main.maxTilesX)
			{
				num460 = Main.maxTilesX;
			}
			if (num461 < 0)
			{
				num461 = 0;
			}
			if (num462 > Main.maxTilesY)
			{
				num462 = Main.maxTilesY;
			}
			bool flag35 = false;
			if (!flag35)
			{
				for (int num463 = num459; num463 < num460; num463++)
				{
					for (int num464 = num461; num464 < num462; num464++)
					{
						if (((Main.tile.At(num463, num464).Active && (Main.tileSolid[(int)Main.tile.At(num463, num464).Type] || 
							(Main.tileSolidTop[(int)Main.tile.At(num463, num464).Type] && Main.tile.At(num463, num464).FrameY == 0))) ||
								Main.tile.At(num463, num464).Liquid > 64))
						{
							Vector2 vector57;
							vector57.X = (float)(num463 * 16);
							vector57.Y = (float)(num464 * 16);
							if (npc.Position.X + (float)npc.Width > vector57.X && npc.Position.X < vector57.X + 16f && npc.Position.Y + (float)npc.Height > vector57.Y && npc.Position.Y < vector57.Y + 16f)
							{
								flag35 = true;
								break;
							}
						}
					}
				}
			}
			if (!flag35)
			{
				npc.localAI[1] = 1f;
				if (npc.Type == 134)
				{
					Rectangle rectangle11 = new Rectangle((int)npc.Position.X, (int)npc.Position.Y, npc.Width, npc.Height);
					int num465 = 1000;
					bool flag36 = true;
					if (npc.Position.Y > Main.players[npc.target].Position.Y)
					{
						for (int num466 = 0; num466 < 255; num466++)
						{
							if (Main.players[num466].Active)
							{
								Rectangle rectangle12 = new Rectangle((int)Main.players[num466].Position.X - num465, (int)Main.players[num466].Position.Y - num465, num465 * 2, num465 * 2);
								if (rectangle11.Intersects(rectangle12))
								{
									flag36 = false;
									break;
								}
							}
						}
						if (flag36)
						{
							flag35 = true;
						}
					}
				}
			}
			else
			{
				npc.localAI[1] = 0f;
			}
			float num467 = 16f;
			if (Main.dayTime || Main.players[npc.target].dead)
			{
				flag35 = false;
				npc.Velocity.Y = npc.Velocity.Y + 1f;
				if ((double)npc.Position.Y > Main.worldSurface * 16.0)
				{
					npc.Velocity.Y = npc.Velocity.Y + 1f;
					num467 = 32f;
				}
				if ((double)npc.Position.Y > Main.rockLayer * 16.0)
				{
					for (int num468 = 0; num468 < 200; num468++)
					{
						if (Main.npcs[num468].aiStyle == npc.aiStyle)
						{
							Main.npcs[num468].Active = false;
						}
					}
				}
			}
			float num469 = 0.1f;
			float num470 = 0.15f;
			Vector2 vector58 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
			float num471 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2);
			float num472 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2);
			num471 = (float)((int)(num471 / 16f) * 16);
			num472 = (float)((int)(num472 / 16f) * 16);
			vector58.X = (float)((int)(vector58.X / 16f) * 16);
			vector58.Y = (float)((int)(vector58.Y / 16f) * 16);
			num471 -= vector58.X;
			num472 -= vector58.Y;
			float num473 = (float)Math.Sqrt((double)(num471 * num471 + num472 * num472));
			if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npcs.Length)
			{
				try
				{
					vector58 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					num471 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - vector58.X;
					num472 = Main.npcs[(int)npc.ai[1]].Position.Y + (float)(Main.npcs[(int)npc.ai[1]].Height / 2) - vector58.Y;
				}
				catch
				{
				}
				npc.rotation = (float)Math.Atan2((double)num472, (double)num471) + 1.57f;
				num473 = (float)Math.Sqrt((double)(num471 * num471 + num472 * num472));
				int num474 = (int)(44f * npc.scale);
				num473 = (num473 - (float)num474) / num473;
				num471 *= num473;
				num472 *= num473;
				npc.Velocity = default(Vector2);
				npc.Position.X = npc.Position.X + num471;
				npc.Position.Y = npc.Position.Y + num472;
				return;
			}
			if (!flag35)
			{
				npc.TargetClosest(true);
				npc.Velocity.Y = npc.Velocity.Y + 0.15f;
				if (npc.Velocity.Y > num467)
				{
					npc.Velocity.Y = num467;
				}
				if ((double)(Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y)) < (double)num467 * 0.4)
				{
					if (npc.Velocity.X < 0f)
					{
						npc.Velocity.X = npc.Velocity.X - num469 * 1.1f;
					}
					else
					{
						npc.Velocity.X = npc.Velocity.X + num469 * 1.1f;
					}
				}
				else
				{
					if (npc.Velocity.Y == num467)
					{
						if (npc.Velocity.X < num471)
						{
							npc.Velocity.X = npc.Velocity.X + num469;
						}
						else
						{
							if (npc.Velocity.X > num471)
							{
								npc.Velocity.X = npc.Velocity.X - num469;
							}
						}
					}
					else
					{
						if (npc.Velocity.Y > 4f)
						{
							if (npc.Velocity.X < 0f)
							{
								npc.Velocity.X = npc.Velocity.X + num469 * 0.9f;
							}
							else
							{
								npc.Velocity.X = npc.Velocity.X - num469 * 0.9f;
							}
						}
					}
				}
			}
			else
			{
				if (npc.soundDelay == 0)
				{
					float num475 = num473 / 40f;
					if (num475 < 10f)
					{
						num475 = 10f;
					}
					if (num475 > 20f)
					{
						num475 = 20f;
					}
					npc.soundDelay = (int)num475;
				}
				num473 = (float)Math.Sqrt((double)(num471 * num471 + num472 * num472));
				float num476 = Math.Abs(num471);
				float num477 = Math.Abs(num472);
				float num478 = num467 / num473;
				num471 *= num478;
				num472 *= num478;
				if (((npc.Velocity.X > 0f && num471 > 0f) || (npc.Velocity.X < 0f && num471 < 0f)) && ((npc.Velocity.Y > 0f && num472 > 0f) || (npc.Velocity.Y < 0f && num472 < 0f)))
				{
					if (npc.Velocity.X < num471)
					{
						npc.Velocity.X = npc.Velocity.X + num470;
					}
					else
					{
						if (npc.Velocity.X > num471)
						{
							npc.Velocity.X = npc.Velocity.X - num470;
						}
					}
					if (npc.Velocity.Y < num472)
					{
						npc.Velocity.Y = npc.Velocity.Y + num470;
					}
					else
					{
						if (npc.Velocity.Y > num472)
						{
							npc.Velocity.Y = npc.Velocity.Y - num470;
						}
					}
				}
				if ((npc.Velocity.X > 0f && num471 > 0f) || (npc.Velocity.X < 0f && num471 < 0f) || (npc.Velocity.Y > 0f && num472 > 0f) || (npc.Velocity.Y < 0f && num472 < 0f))
				{
					if (npc.Velocity.X < num471)
					{
						npc.Velocity.X = npc.Velocity.X + num469;
					}
					else
					{
						if (npc.Velocity.X > num471)
						{
							npc.Velocity.X = npc.Velocity.X - num469;
						}
					}
					if (npc.Velocity.Y < num472)
					{
						npc.Velocity.Y = npc.Velocity.Y + num469;
					}
					else
					{
						if (npc.Velocity.Y > num472)
						{
							npc.Velocity.Y = npc.Velocity.Y - num469;
						}
					}
					if ((double)Math.Abs(num472) < (double)num467 * 0.2 && ((npc.Velocity.X > 0f && num471 < 0f) || (npc.Velocity.X < 0f && num471 > 0f)))
					{
						if (npc.Velocity.Y > 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y + num469 * 2f;
						}
						else
						{
							npc.Velocity.Y = npc.Velocity.Y - num469 * 2f;
						}
					}
					if ((double)Math.Abs(num471) < (double)num467 * 0.2 && ((npc.Velocity.Y > 0f && num472 < 0f) || (npc.Velocity.Y < 0f && num472 > 0f)))
					{
						if (npc.Velocity.X > 0f)
						{
							npc.Velocity.X = npc.Velocity.X + num469 * 2f;
						}
						else
						{
							npc.Velocity.X = npc.Velocity.X - num469 * 2f;
						}
					}
				}
				else
				{
					if (num476 > num477)
					{
						if (npc.Velocity.X < num471)
						{
							npc.Velocity.X = npc.Velocity.X + num469 * 1.1f;
						}
						else
						{
							if (npc.Velocity.X > num471)
							{
								npc.Velocity.X = npc.Velocity.X - num469 * 1.1f;
							}
						}
						if ((double)(Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y)) < (double)num467 * 0.5)
						{
							if (npc.Velocity.Y > 0f)
							{
								npc.Velocity.Y = npc.Velocity.Y + num469;
							}
							else
							{
								npc.Velocity.Y = npc.Velocity.Y - num469;
							}
						}
					}
					else
					{
						if (npc.Velocity.Y < num472)
						{
							npc.Velocity.Y = npc.Velocity.Y + num469 * 1.1f;
						}
						else
						{
							if (npc.Velocity.Y > num472)
							{
								npc.Velocity.Y = npc.Velocity.Y - num469 * 1.1f;
							}
						}
						if ((double)(Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y)) < (double)num467 * 0.5)
						{
							if (npc.Velocity.X > 0f)
							{
								npc.Velocity.X = npc.Velocity.X + num469;
							}
							else
							{
								npc.Velocity.X = npc.Velocity.X - num469;
							}
						}
					}
				}
			}
			npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) + 1.57f;
			if (npc.Type == 134)
			{
				if (flag35)
				{
					if (npc.localAI[0] != 1f)
					{
						npc.netUpdate = true;
					}
					npc.localAI[0] = 1f;
				}
				else
				{
					if (npc.localAI[0] != 0f)
					{
						npc.netUpdate = true;
					}
					npc.localAI[0] = 0f;
				}
				if (((npc.Velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.Velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.Velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.Velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
				{
					npc.netUpdate = true;
				}
			}
		}

        /// <summary>
        /// Checks if there are any active NPCs of specified type
        /// </summary>
        /// <param name="Id">Id of NPC to check for</param>
        /// <returns>True if active, false if not</returns>
        public static bool IsNPCSummoned(int Id)
        {
            for (int i = 0; i < Main.npcs.Length; i++)
            {
                NPC npc = Main.npcs[i];
                if (npc != null && npc.Active && npc.whoAmI == Id)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if there are any active NPCs of specified name
        /// </summary>
        /// <param name="Name">Name of NPC to check for</param>
        /// <returns>True if active, false if not</returns>
        public static bool IsNPCSummoned(string Name)
        {
            int Id;
            return TryFindNPCByName(Name, out Id);
        }

        public static bool TryFindNPCByName(string Name, out int Id)
        {
            Id = default(Int32);

            for (int i = 0; i < Main.npcs.Length; i++)
            {
                NPC npc = Main.npcs[i];
                if (npc != null && npc.Active && npc.Name == Name)
                {
                    Id = npc.whoAmI;
                    return true;
                }
            }
            return false;
        }

        public static void SpawnGuide()
        {
            int GuideIndex = NewNPC(Main.spawnTileX * 16, Main.spawnTileY * 16, 22, 0);
            Main.npcs[GuideIndex].homeTileX = Main.spawnTileX;
            Main.npcs[GuideIndex].homeTileY = Main.spawnTileY;
            Main.npcs[GuideIndex].direction = 1;
            Main.npcs[GuideIndex].homeless = true;
        }

        public static void SpawnTDCMQuestGiver()
        {
            if (IsNPCSummoned(Statics.TDCM_QUEST_GIVER) || !Program.properties.AllowTDCMRPG)
                return;

            Vector2 Spawn = World.GetRandomClearTile(Main.spawnTileX, Main.spawnTileY, 100, true, 100, 50);

            int npcIndex = NewNPC((int)Spawn.X * 16, (int)Spawn.Y * 16, 22, 0);
            Main.npcs[npcIndex].Name = Statics.TDCM_QUEST_GIVER;
            Main.npcs[npcIndex].homeTileX = (int)Spawn.X;
            Main.npcs[npcIndex].homeTileY = (int)Spawn.Y;
            Main.npcs[npcIndex].direction = 1;
            Main.npcs[npcIndex].homeless = true;
            //Main.npcs[npcIndex].dontTakeDamage = true;

            ProgramLog.Debug.Log(
                String.Format("{0} spawned at {1},{2}.", Statics.TDCM_QUEST_GIVER, Spawn.X, Spawn.Y)
            );
        }

        /// <summary>
        /// Server Only
        /// </summary>
        /// <param name="pos"></param>
        public static void SpawnWallOfFlesh(Vector2 pos)
        {
            if (pos.Y / 16f < (float)(Main.maxTilesY - 205))
                return;

            if (Main.WallOfFlesh >= 0)
            {
                ProgramLog.Log("Attempt to call SpawnWOF with an existing Entity.");
                return;
            }

            Player.FindClosest(pos, 16, 16);
            int direction = 1;
            if (pos.X / 16f > (float)(Main.maxTilesX / 2))
                direction = -1;

            bool playerOrSuitable = false;
            int findX = (int)pos.X;
            while (!playerOrSuitable)
            {
                playerOrSuitable = true;

                foreach (var ply in Main.players)
                {
                    if (ply.Active && ply.Position.X > (float)(findX - 1200) && ply.Position.X < (float)(findX + 1200))
                    {
                        findX -= direction * 16;
                        playerOrSuitable = false;
                    }
                }

                if (findX / 16 < 20 || findX / 16 > Main.maxTilesX - 20)
                    playerOrSuitable = true;
            }
            int posY = (int)pos.Y;
            int posX = findX / 16;
            int tileY = posY / 16;
            int nextY = 0;
            try
            {
                while (WorldModify.SolidTile(posX, tileY - nextY) || Main.tile.At(posX, tileY - nextY).Liquid >= 100)
                {
                    if (!WorldModify.SolidTile(posX, tileY + nextY) && Main.tile.At(posX, tileY + nextY).Liquid < 100)
                    {
                        tileY += nextY;
                        posY = tileY * 16;
                        int num7 = NPC.NewNPC(findX, posY, 113, 0);
                        if (Main.npcs[num7].DisplayName == String.Empty)
                            Main.npcs[num7].DisplayName = Main.npcs[num7].Name;

                        NetMessage.SendData(25, -1, -1, Main.npcs[num7].DisplayName + " has awoken!", 255, 175f, 75f, 255f, 0);
                    }
                    nextY++;
                }
                tileY -= nextY;
            }
            catch { }
        }

		public void checkDead()
		{
			if (!this.Active || (this.realLife >= 0 && this.realLife != this.whoAmI))
				return;

			if (this.life <= 0)
			{
				NPC.noSpawnCycle = true;
				if (this.townNPC && this.Type != 37)
				{
					string str = this.Name;
					if (this.DisplayName != "")
						str = this.DisplayName;
					
					NetMessage.SendData(25, -1, -1, str + " was slain...", 255, 255f, 25f, 25f, 0);

					Main.chrName[this.Type] = "";
					NPC.SetNames();
					NetMessage.SendData(56, -1, -1, "", this.Type);
				}

				if (this.townNPC && this.homeless && WorldModify.spawnNPC == this.Type)
					WorldModify.spawnNPC = 0;

				this.NPCLoot();
				this.Active = false;

				if (this.Type == 26 || this.Type == 27 || this.Type == 28 || this.Type == 29 || this.Type == 111)
					Main.invasionSize--;
			}
		}

		// [ToDo] Simplify by using arrays instead of switches?
		public static void SetNames()
		{
			if (WorldModify.genRand == null)
				WorldModify.genRand = new Random();

			foreach (var i in new int[] { 17, 18, 19, 20, 22, 38, 54, 107, 108, 124 })
			{
				if (Main.chrName[i] == null)
					Main.chrName[i] = String.Empty;
			}

			GenerateNurseName();

			GenerateMechanicName();

			GenerateArmsDealerName();

			GenerateGuideName();

			GenerateDryadName();

			GenerateDemolitionistName();

			GenerateWizardName();

			GenerateMerchantName();

			GenerateClothierName();

			GenerateGoblinTinkererName();
		}

		public static void GenerateNurseName()
		{
			string _name;

			switch (WorldModify.genRand.Next(23))
			{
				case 0:
					_name = "Molly";
					break;
				case 1:
					_name = "Amy";
					break;
				case 2:
					_name = "Claire";
					break;
				case 3:
					_name = "Emily";
					break;
				case 4:
					_name = "Katie";
					break;
				case 5:
					_name = "Madeline";
					break;
				case 6:
					_name = "Katelyn";
					break;
				case 7:
					_name = "Emma";
					break;
				case 8:
					_name = "Abigail";
					break;
				case 9:
					_name = "Carly";
					break;
				case 10:
					_name = "Jenna";
					break;
				case 11:
					_name = "Heather";
					break;
				case 12:
					_name = "Katherine";
					break;
				case 13:
					_name = "Caitlin";
					break;
				case 14:
					_name = "Kaitlin";
					break;
				case 15:
					_name = "Holly";
					break;
				case 16:
					_name = "Kaitlyn";
					break;
				case 17:
					_name = "Hannah";
					break;
				case 18:
					_name = "Kathryn";
					break;
				case 19:
					_name = "Lorraine";
					break;
				case 20:
					_name = "Helen";
					break;
				case 21:
					_name = "Kayla";
					break;
				default:
					_name = "Allison";
					break;
			}

			if (Main.chrName[18] == String.Empty)
				Main.chrName[18] = _name;
		}

		public static void GenerateMechanicName()
		{
			string _name;
			switch (WorldModify.genRand.Next(24))
			{
				case 0:
					_name = "Shayna";
					break;
				case 1:
					_name = "Korrie";
					break;
				case 2:
					_name = "Ginger";
					break;
				case 3:
					_name = "Brooke";
					break;
				case 4:
					_name = "Jenny";
					break;
				case 5:
					_name = "Autumn";
					break;
				case 6:
					_name = "Nancy";
					break;
				case 7:
					_name = "Ella";
					break;
				case 8:
					_name = "Kayla";
					break;
				case 9:
					_name = "Beth";
					break;
				case 10:
					_name = "Sophia";
					break;
				case 11:
					_name = "Marshanna";
					break;
				case 12:
					_name = "Lauren";
					break;
				case 13:
					_name = "Trisha";
					break;
				case 14:
					_name = "Shirlena";
					break;
				case 15:
					_name = "Sheena";
					break;
				case 16:
					_name = "Ellen";
					break;
				case 17:
					_name = "Amy";
					break;
				case 18:
					_name = "Dawn";
					break;
				case 19:
					_name = "Susana";
					break;
				case 20:
					_name = "Meredith";
					break;
				case 21:
					_name = "Selene";
					break;
				case 22:
					_name = "Terra";
					break;
				default:
					_name = "Sally";
					break;
			}

			if (Main.chrName[124] == String.Empty)
				Main.chrName[124] = _name;
		}

		public static void GenerateGoblinTinkererName()
		{
			string _name;

			switch (WorldModify.genRand.Next(25))
			{
				case 0:
					_name = "Grodax";
					break;
				case 1:
					_name = "Sarx";
					break;
				case 2:
					_name = "Xon";
					break;
				case 3:
					_name = "Mrunok";
					break;
				case 4:
					_name = "Nuxatk";
					break;
				case 5:
					_name = "Tgerd";
					break;
				case 6:
					_name = "Darz";
					break;
				case 7:
					_name = "Smador";
					break;
				case 8:
					_name = "Stazen";
					break;
				case 9:
					_name = "Mobart";
					break;
				case 10:
					_name = "Knogs";
					break;
				case 11:
					_name = "Tkanus";
					break;
				case 12:
					_name = "Negurk";
					break;
				case 13:
					_name = "Nort";
					break;
				case 14:
					_name = "Durnok";
					break;
				case 15:
					_name = "Trogem";
					break;
				case 16:
					_name = "Stezom";
					break;
				case 17:
					_name = "Gnudar";
					break;
				case 18:
					_name = "Ragz";
					break;
				case 19:
					_name = "Fahd";
					break;
				case 20:
					_name = "Xanos";
					break;
				case 21:
					_name = "Arback";
					break;
				case 22:
					_name = "Fjell";
					break;
				case 23:
					_name = "Dalek";
					break;
				default:
					_name = "Knub";
					break;
			}

			if (Main.chrName[107] == String.Empty)
				Main.chrName[107] = _name;
		}

		public static void GenerateClothierName()
		{
			string _name;

			switch (WorldModify.genRand.Next(24))
			{
				case 0:
					_name = "Sebastian";
					break;
				case 1:
					_name = "Rupert";
					break;
				case 2:
					_name = "Clive";
					break;
				case 3:
					_name = "Nigel";
					break;
				case 4:
					_name = "Mervyn";
					break;
				case 5:
					_name = "Cedric";
					break;
				case 6:
					_name = "Pip";
					break;
				case 7:
					_name = "Cyril";
					break;
				case 8:
					_name = "Fitz";
					break;
				case 9:
					_name = "Lloyd";
					break;
				case 10:
					_name = "Arthur";
					break;
				case 11:
					_name = "Rodney";
					break;
				case 12:
					_name = "Graham";
					break;
				case 13:
					_name = "Edward";
					break;
				case 14:
					_name = "Alfred";
					break;
				case 15:
					_name = "Edmund";
					break;
				case 16:
					_name = "Henry";
					break;
				case 17:
					_name = "Herald";
					break;
				case 18:
					_name = "Roland";
					break;
				case 19:
					_name = "Lincoln";
					break;
				case 20:
					_name = "Lloyd";
					break;
				case 21:
					_name = "Edgar";
					break;
				case 22:
					_name = "Eustace";
					break;
				default:
					_name = "Rodrick";
					break;
			}

			if (Main.chrName[54] == String.Empty)
				Main.chrName[54] = _name;
		}

		public static void GenerateMerchantName()
		{
			string _name;

			switch (WorldModify.genRand.Next(23))
			{
				case 0:
					_name = "Alfred";
					break;
				case 1:
					_name = "Barney";
					break;
				case 2:
					_name = "Calvin";
					break;
				case 3:
					_name = "Edmund";
					break;
				case 4:
					_name = "Edwin";
					break;
				case 5:
					_name = "Eugene";
					break;
				case 6:
					_name = "Frank";
					break;
				case 7:
					_name = "Frederick";
					break;
				case 8:
					_name = "Gilbert";
					break;
				case 9:
					_name = "Gus";
					break;
				case 10:
					_name = "Wilbur";
					break;
				case 11:
					_name = "Seymour";
					break;
				case 12:
					_name = "Louis";
					break;
				case 13:
					_name = "Humphrey";
					break;
				case 14:
					_name = "Harold";
					break;
				case 15:
					_name = "Milton";
					break;
				case 16:
					_name = "Mortimer";
					break;
				case 17:
					_name = "Howard";
					break;
				case 18:
					_name = "Walter";
					break;
				case 19:
					_name = "Finn";
					break;
				case 20:
					_name = "Isacc";
					break;
				case 21:
					_name = "Joseph";
					break;
				default:
					_name = "Ralph";
					break;
			}

			if (Main.chrName[17] == String.Empty)
				Main.chrName[17] = _name;
		}

		public static void GenerateWizardName()
		{
			string _name;

			switch (WorldModify.genRand.Next(21))
			{
				case 0:
					_name = "Dalamar";
					break;
				case 1:
					_name = "Dulais";
					break;
				case 2:
					_name = "Elric";
					break;
				case 3:
					_name = "Arddun";
					break;
				case 4:
					_name = "Maelor";
					break;
				case 5:
					_name = "Leomund";
					break;
				case 6:
					_name = "Hirael";
					break;
				case 7:
					_name = "Gwentor";
					break;
				case 8:
					_name = "Greum";
					break;
				case 9:
					_name = "Gearroid";
					break;
				case 10:
					_name = "Fizban";
					break;
				case 11:
					_name = "Ningauble";
					break;
				case 12:
					_name = "Seonag";
					break;
				case 13:
					_name = "Sargon";
					break;
				case 14:
					_name = "Merlyn";
					break;
				case 15:
					_name = "Magius";
					break;
				case 16:
					_name = "Berwyn";
					break;
				case 17:
					_name = "Arwyn";
					break;
				case 18:
					_name = "Alasdair";
					break;
				case 19:
					_name = "Tagar";
					break;
				default:
					_name = "Xanadu";
					break;
			}

			if (Main.chrName[108] == String.Empty)
				Main.chrName[108] = _name;
		}

		public static void GenerateDemolitionistName()
		{
			string _name;

			switch (WorldModify.genRand.Next(22))
			{
				case 0:
					_name = "Dolbere";
					break;
				case 1:
					_name = "Bazdin";
					break;
				case 2:
					_name = "Durim";
					break;
				case 3:
					_name = "Tordak";
					break;
				case 4:
					_name = "Garval";
					break;
				case 5:
					_name = "Morthal";
					break;
				case 6:
					_name = "Oten";
					break;
				case 7:
					_name = "Dolgen";
					break;
				case 8:
					_name = "Gimli";
					break;
				case 9:
					_name = "Gimut";
					break;
				case 10:
					_name = "Duerthen";
					break;
				case 11:
					_name = "Beldin";
					break;
				case 12:
					_name = "Jarut";
					break;
				case 13:
					_name = "Ovbere";
					break;
				case 14:
					_name = "Norkas";
					break;
				case 15:
					_name = "Dolgrim";
					break;
				case 16:
					_name = "Boften";
					break;
				case 17:
					_name = "Norsun";
					break;
				case 18:
					_name = "Dias";
					break;
				case 19:
					_name = "Fikod";
					break;
				case 20:
					_name = "Urist";
					break;
				default:
					_name = "Darur";
					break;
			}

			if (Main.chrName[38] == String.Empty)
				Main.chrName[38] = _name;
		}

		public static void GenerateDryadName()
		{
			string _name;

			switch (WorldModify.genRand.Next(22))
			{
				case 0:
					_name = "Alalia";
					break;
				case 1:
					_name = "Amelia"; //Another duplicate. Might be intended, but idc.
					break;
				case 2:
					_name = "Alura";
					break;
				case 3:
					_name = "Ariella";
					break;
				case 4:
					_name = "Caelia";
					break;
				case 5:
					_name = "Calista";
					break;
				case 6:
					_name = "Chryseis";
					break;
				case 7:
					_name = "Emerenta";
					break;
				case 8:
					_name = "Elysia";
					break;
				case 9:
					_name = "Evvie";
					break;
				case 10:
					_name = "Faye";
					break;
				case 11:
					_name = "Felicitae";
					break;
				case 12:
					_name = "Lunette";
					break;
				case 13:
					_name = "Nata";
					break;
				case 14:
					_name = "Nissa";
					break;
				case 15:
					_name = "Tatiana";
					break;
				case 16:
					_name = "Rosalva";
					break;
				case 17:
					_name = "Shea";
					break;
				case 18:
					_name = "Tania";
					break;
				case 19:
					_name = "Isis";
					break;
				case 20:
					_name = "Celestia";
					break;
				default:
					_name = "Xylia";
					break;
			}

			if (Main.chrName[20] == String.Empty)
				Main.chrName[20] = _name;
		}

		public static void GenerateGuideName()
		{
			string _name;

			switch (WorldModify.genRand.Next(35))
			{
				case 0:
					_name = "Jake";
					break;
				case 1:
					_name = "Connor";
					break;
				case 2:
					_name = "Tanner";
					break;
				case 3:
					_name = "Wyatt";
					break;
				case 4:
					_name = "Cody";
					break;
				case 5:
					_name = "Dustin";
					break;
				case 6:
					_name = "Luke";
					break;
				case 7:
					_name = "Jack";
					break;
				case 8:
					_name = "Scott";
					break;
				case 9:
					_name = "Logan";
					break;
				case 10:
					_name = "Cole";
					break;
				case 11:
					_name = "Lucas";
					break;
				case 12:
					_name = "Bradley";
					break;
				case 13:
					_name = "Jacob";
					break;
				case 14:
					_name = "Garrett";
					break;
				case 15:
					_name = "Dylan";
					break;
				case 16:
					_name = "Maxwell";
					break;
				case 17:
					_name = "Steve";
					break;
				case 18:
					_name = "Brett";
					break;
				case 19:
					_name = "Andrew";
					break;
				case 20:
					_name = "Harley";
					break;
				case 21:
					_name = "Kyle";
					break;
				case 22:
					_name = "Cole"; //Redigit, You had a duplicate...
					break;
				case 23:
					_name = "Ryan";
					break;
				case 24:
					_name = "Jeffrey";
					break;
				case 25:
					_name = "Seth";
					break;
				case 26:
					_name = "Marty";
					break;
				case 27:
					_name = "Brandon";
					break;
				case 28:
					_name = "Zach";
					break;
				case 29:
					_name = "Jeff";
					break;
				case 30:
					_name = "Daniel";
					break;
				case 31:
					_name = "Trent";
					break;
				case 32:
					_name = "Kevin";
					break;
				case 33:
					_name = "Brian";
					break;
				default:
					_name = "Colin";
					break;
			}

			if (Main.chrName[22] == String.Empty)
				Main.chrName[22] = _name;
		}

		public static void GenerateArmsDealerName()
		{
			string _name;

			switch (WorldModify.genRand.Next(23))
			{
				case 0:
					_name = "DeShawn";
					break;
				case 1:
					_name = "DeAndre";
					break;
				case 2:
					_name = "Marquis";
					break;
				case 3:
					_name = "Darnell";
					break;
				case 4:
					_name = "Terrell";
					break;
				case 5:
					_name = "Malik";
					break;
				case 6:
					_name = "Trevon";
					break;
				case 7:
					_name = "Tyrone";
					break;
				case 8:
					_name = "Willie";
					break;
				case 9:
					_name = "Dominique";
					break;
				case 10:
					_name = "Demetrius";
					break;
				case 11:
					_name = "Reginald";
					break;
				case 12:
					_name = "Jamal";
					break;
				case 13:
					_name = "Maurice";
					break;
				case 14:
					_name = "Jalen";
					break;
				case 15:
					_name = "Darius";
					break;
				case 16:
					_name = "Xavier";
					break;
				case 17:
					_name = "Terrance";
					break;
				case 18:
					_name = "Andre";
					break;
				case 19:
					_name = "Dante";
					break;
				case 20:
					_name = "Brimst";
					break;
				case 21:
					_name = "Bronson";
					break;
				default:
					_name = "Darryl";
					break;
			}

			if (Main.chrName[19] == String.Empty)
				Main.chrName[19] = _name;
		}


        // [TODO] 1.1
        public static bool downedClown { get; set; }
        public static bool downedGoblins { get; set; }
        public static bool savedMech { get; set; }
        public static bool savedWizard { get; set; }
        public static bool savedGoblin { get; set; }

    }
}

