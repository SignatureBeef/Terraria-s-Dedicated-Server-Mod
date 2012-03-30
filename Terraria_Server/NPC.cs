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
	/// 
	public class NPC : BaseEntity, ISender
	{
		internal delegate void NPCSpawn(int npcId);
		internal static event NPCSpawn NPCSpawnHandler;

		bool ISender.Op
		{
			get { return false; }
			set { }
		}

		void ISender.sendMessage(string a, int b, float c, float d, float e)
		{
		}

		private const int ACTIVE_TIME = 750;
		/// <summary>
		/// Total allowable active NPCs.
		/// </summary>
		public const int MAX_NPCS = 200;
		/// <summary>
		/// Maximum AI chains
		/// </summary>
		public const int MAX_AI = 4;

		//1.1.2/1?
		public int netSkip = -2;

		public bool oldHomeless;
		public int oldHomeTileX = -1;
		public int oldHomeTileY = -1;
		public int netSpam;
		public Vector2[] oldPos = new Vector2[10];

		public bool justHit; ////////////////////TODO
		public static int immuneTime = 20;
		private static int spawnSpaceX = 3;
		private static int spawnSpaceY = 3;
		public static int sWidth = 1680;
		public static int sHeight = 1050;
		private static int spawnRangeX = (int)((double)(sWidth / 16) * 0.7);
		private static int spawnRangeY = (int)((double)(sHeight / 16) * 0.7);
		public static int pawnsafeRangeX = (int)((double)(sWidth / 16) * 0.52);
		public static int safeRangeY = (int)((double)(sHeight / 16) * 0.52);
		public static int safeRangeX = (int)((double)(sWidth / 16) * 0.52);
		private static int activeRangeX = (int)((double)sWidth * 1.7);
		private static int activeRangeY = (int)((double)sHeight * 1.7);
		private static int townRangeX = sWidth;
		private static int townRangeY = sHeight;
		/// <summary>
		/// Number of slots NPC takes up when active
		/// </summary>
		//public static float npcSlots = 1f;
		private static bool noSpawnCycle = false;
		public static int defaultSpawnRate = 600;
		public static int defaultMaxSpawns = 5;
		public static bool downedBoss1 = false;
		public static bool downedBoss2 = false;
		public static bool downedBoss3 = false;
		public static int spawnRate = defaultSpawnRate;
		public static int maxSpawns = defaultMaxSpawns;

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
		[DontClone]
		public int oldDirection;
		[DontClone]
		public int oldTarget;
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

		public const Int32 MAX_BUFF_TIME = 5;
		public const Int32 MAX_BUFF_TYPE = 5;
		public const Int32 MAX_BUFF_IMMUNE = 256;

		[DontClone]
		public Vector2 Velocity;
		[DontClone]
		public Vector2 oldPosition;
		[DontClone]
		public Vector2 oldVelocity;

		[DeepClone]
		public float[] ai = new float[MAX_AI];
		[DeepClone]
		public float[] localAI = new float[MAX_AI];
		[DeepClone]
		public ushort[] immune = new ushort[MAX_BUFF_IMMUNE];
		[DeepClone]
		public int[] buffType = new int[MAX_BUFF_TYPE];
		[DeepClone]
		public int[] buffTime = new int[MAX_BUFF_TIME];
		[DeepClone]
		public bool[] buffImmune = new bool[Main.MAX_BUFFS];

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
				if (Int32.TryParse(value, out i))
				{
					var saveName = Name;
					Registries.NPC.SetDefaults(this, i);
					Name = saveName;
					//Logging.ProgramLog.Debug.Log ("{0}({1}) is inheriting {2}", Name, Type, i);
				}
				else
				{
					var saveName = Name;
					Registries.NPC.SetDefaults(this, value);
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
				damage = (int)(damage * scale);
			}
		}

		public string ScaleDefense
		{
			get { return ""; }
			set
			{
				defense = (int)(defense * scale);
			}
		}

		public string ScaleLifeMax
		{
			get { return ""; }
			set
			{
				lifeMax = (int)(lifeMax * scale);
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
		[DontClone]
		public int whoAmI;

		[DontClone]
		public bool MadeSpawn;

		/// <summary>
		/// NPC Constructor.  Sets many defaults
		/// </summary>
		public NPC()
		{
			homeTileX = -1;
			homeTileY = -1;
			knockBackResist = 1f;
			Name = String.Empty;
			oldTarget = target;
			scale = 1f;
			slots = 1f;
			spriteDirection = -1;
			target = 255;
			targetRect = default(Rectangle);
			timeLeft = ACTIVE_TIME;
			netUpdate = true;
			buffImmune[31] = true; //confusion

			LoadAIFunctions();
		}

		/// <summary>
		/// Movement checks
		/// </summary>
		/// <param name="index">Main.npcs[] index number</param>
		public static void AI(int index)
		{
			NPC npc = Main.npcs[index];
			int aiStyle = npc.aiStyle;

			if (AIFunctions.ContainsKey(aiStyle))
				AIFunctions[aiStyle](npc, false, TileCollection.ITileAt);
		}

		public static bool NearSpikeBall(int x, int y)
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
				if (this.type == NPCType.N08_DEVOURER_BODY || this.type == NPCType.N09_DEVOURER_TAIL ||
					this.type == NPCType.N11_GIANT_WORM_BODY || this.type == NPCType.N12_GIANT_WORM_TAIL ||
					this.type == NPCType.N14_EATER_OF_WORLDS_BODY || this.type == NPCType.N15_EATER_OF_WORLDS_TAIL ||
					this.type == NPCType.N40_BONE_SERPENT_BODY || this.type == NPCType.N41_BONE_SERPENT_TAIL ||
					this.type == NPCType.N96_DIGGER_BODY || this.type == NPCType.N97_DIGGER_TAIL ||
					this.type == NPCType.N99_SEEKER_BODY || this.type == NPCType.N100_SEEKER_TAIL ||
					(this.type > NPCType.N87_WYVERN_HEAD && this.type <= NPCType.N92_WYVERN_TAIL) ||
					this.type == NPCType.N118_LEECH_BODY || this.type == NPCType.N119_LEECH_TAIL ||
					this.type == NPCType.N113_WALL_OF_FLESH || this.type == NPCType.N114_WALL_OF_FLESH_EYE ||
					this.type == NPCType.N115_THE_HUNGRY)
				{
					return;
				}
				if (this.type >= NPCType.N134_THE_DESTROYER && this.type <= NPCType.N136_THE_DESTROYER_TAIL)
				{
					return;
				}
				if (this.townNPC)
				{
					Rectangle rectangle = new Rectangle((int)(this.Position.X + (float)(this.Width / 2) - (float)townRangeX), (int)(this.Position.Y + (float)(this.Height / 2) - (float)townRangeY), townRangeX * 2, townRangeY * 2);
					for (int i = 0; i < 255; i++)
					{
						if (Main.players[i].Active && rectangle.Intersects(new Rectangle((int)Main.players[i].Position.X, (int)Main.players[i].Position.Y, Main.players[i].Width, Main.players[i].Height)))
						{
							Main.players[i].TownNPCs += this.slots;
						}
					}
					return;
				}
				bool flag = false;
				Rectangle rectangle2 = new Rectangle((int)(this.Position.X + (float)(this.Width / 2) - (float)activeRangeX), (int)(this.Position.Y + (float)(this.Height / 2) - (float)activeRangeY), activeRangeX * 2, activeRangeY * 2);
				Rectangle rectangle3 = new Rectangle((int)((double)(this.Position.X + (float)(this.Width / 2)) - (double)sWidth * 0.5 - (double)this.Width), (int)((double)(this.Position.Y + (float)(this.Height / 2)) - (double)sHeight * 0.5 - (double)this.Height), sWidth + this.Width * 2, sHeight + this.Height * 2);
				for (int j = 0; j < 255; j++)
				{
					if (Main.players[j].Active)
					{
						if (rectangle2.Intersects(new Rectangle((int)Main.players[j].Position.X, (int)Main.players[j].Position.Y, Main.players[j].Width, Main.players[j].Height)))
						{
							flag = true;
							if (this.type != NPCType.N25_BURNING_SPHERE && this.type != NPCType.N30_CHAOS_BALL &&
								this.type != NPCType.N33_WATER_SPHERE && this.lifeMax > 0)
							{
								Main.players[j].ActiveNPCs += this.slots;
							}
						}
						if (rectangle3.Intersects(new Rectangle((int)Main.players[j].Position.X, (int)Main.players[j].Position.Y, Main.players[j].Width, Main.players[j].Height)))
						{
							this.timeLeft = ACTIVE_TIME;
						}
						if (this.type == NPCType.N07_DEVOURER_HEAD || this.type == NPCType.N10_GIANT_WORM_HEAD ||
							this.type == NPCType.N13_EATER_OF_WORLDS_HEAD || this.type == NPCType.N39_BONE_SERPENT_HEAD ||
							this.type == NPCType.N87_WYVERN_HEAD || this.boss || this.type == NPCType.N35_SKELETRON_HEAD ||
							this.type == NPCType.N36_SKELETRON_HAND ||
							this.type == NPCType.N127_SKELETRON_PRIME || this.type == NPCType.N128_PRIME_CANNON ||
							this.type == NPCType.N129_PRIME_SAW || this.type == NPCType.N130_PRIME_VICE || this.type == NPCType.N131_PRIME_LASER)
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
					noSpawnCycle = true;
					this.Active = false;
					this.netSkip = -1;
					this.life = 0;

					NetMessage.SendData(23, -1, -1, "", this.whoAmI, 0f, 0f, 0f, 0);

					if (this.aiStyle == 6)
					{
						for (int k = (int)this.ai[0]; k > 0; k = (int)Main.npcs[k].ai[0])
						{
							if (Main.npcs[k].Active)
								Main.npcs[k].Kill();
						}
					}
				}
			}
		}

		/// <summary>
		/// Kills the NPC
		/// </summary>
		public void Kill()
		{
			Active = false;
			life = 0;
			netSkip = -1;
			NetMessage.SendData(23, -1, -1, String.Empty, whoAmI);
		}

		/// <summary>
		/// Spawns all NPCs that need to be
		/// </summary>
		public static void SpawnNPC()
		{
			if (noSpawnCycle)
			{
				noSpawnCycle = false;
				return;
			}

			bool flag = false;
			bool flag2 = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < Main.MAX_PLAYERS; i++)
			{
				if (Main.players[i].Active)
					num3++;
			}
			for (int j = 0; j < Main.MAX_PLAYERS; j++)
			{
				if (Main.players[j].Active && !Main.players[j].dead)
				{
					bool flag3 = false;
					bool flag4 = false;
					bool flag5 = false;
					if (Main.players[j].Active && Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0 && (double)Main.players[j].Position.Y < Main.worldSurface * 16.0 + (double)sHeight)
					{
						int num4 = 3000;
						if ((double)Main.players[j].Position.X > Main.invasionX * 16.0 - (double)num4 && (double)Main.players[j].Position.X < Main.invasionX * 16.0 + (double)num4)
						{
							flag4 = true;
						}
					}
					flag = false;
					spawnRate = defaultSpawnRate;
					maxSpawns = defaultMaxSpawns;
					if (Main.hardMode)
					{
						spawnRate = (int)((double)defaultSpawnRate * 0.9);
						maxSpawns = defaultMaxSpawns + 1;
					}
					if (Main.players[j].Position.Y > (float)((Main.maxTilesY - 200) * 16))
					{
						maxSpawns = (int)((float)maxSpawns * 2f);
					}
					else if ((double)Main.players[j].Position.Y > Main.rockLayer * 16.0 + (double)sHeight)
					{
						spawnRate = (int)((double)spawnRate * 0.4);
						maxSpawns = (int)((float)maxSpawns * 1.9f);
					}
					else if ((double)Main.players[j].Position.Y > Main.worldSurface * 16.0 + (double)sHeight)
					{
						if (Main.hardMode)
						{
							spawnRate = (int)((double)spawnRate * 0.45);
							maxSpawns = (int)((float)maxSpawns * 1.8f);
						}
						else
						{
							spawnRate = (int)((double)spawnRate * 0.5);
							maxSpawns = (int)((float)maxSpawns * 1.7f);
						}
					}
					else if (!Main.dayTime)
					{
						spawnRate = (int)((double)spawnRate * 0.6);
						maxSpawns = (int)((float)maxSpawns * 1.3f);
						if (Main.bloodMoon)
						{
							spawnRate = (int)((double)spawnRate * 0.3);
							maxSpawns = (int)((float)maxSpawns * 1.8f);
						}
					}
					if (Main.players[j].zoneDungeon)
					{
						spawnRate = (int)((double)spawnRate * 0.4);
						maxSpawns = (int)((float)maxSpawns * 1.7f);
					}
					else if (Main.players[j].zoneJungle)
					{
						spawnRate = (int)((double)spawnRate * 0.4);
						maxSpawns = (int)((float)maxSpawns * 1.5f);
					}
					else if (Main.players[j].zoneEvil)
					{
						spawnRate = (int)((double)spawnRate * 0.65);
						maxSpawns = (int)((float)maxSpawns * 1.3f);
					}
					else if (Main.players[j].zoneMeteor)
					{
						spawnRate = (int)((double)spawnRate * 0.4);
						maxSpawns = (int)((float)maxSpawns * 1.1f);
					}
					if (Main.players[j].zoneHoly && (double)Main.players[j].Position.Y > Main.rockLayer * 16.0 + (double)sHeight)
					{
						spawnRate = (int)((double)spawnRate * 0.65);
						maxSpawns = (int)((float)maxSpawns * 1.3f);
					}
					if (Main.WallOfFlesh >= 0 && Main.players[j].Position.Y > (float)((Main.maxTilesY - 200) * 16))
					{
						maxSpawns = (int)((float)maxSpawns * 0.3f);
						spawnRate *= 3;
					}
					if ((double)Main.players[j].ActiveNPCs < (double)maxSpawns * 0.2)
					{
						spawnRate = (int)((float)spawnRate * 0.6f);
					}
					else if ((double)Main.players[j].ActiveNPCs < (double)maxSpawns * 0.4)
					{
						spawnRate = (int)((float)spawnRate * 0.7f);
					}
					else if ((double)Main.players[j].ActiveNPCs < (double)maxSpawns * 0.6)
					{
						spawnRate = (int)((float)spawnRate * 0.8f);
					}
					else if ((double)Main.players[j].ActiveNPCs < (double)maxSpawns * 0.8)
					{
						spawnRate = (int)((float)spawnRate * 0.9f);
					}
					if ((double)(Main.players[j].Position.Y * 16f) > (Main.worldSurface + Main.rockLayer) / 2.0 || Main.players[j].zoneEvil)
					{
						if ((double)Main.players[j].ActiveNPCs < (double)maxSpawns * 0.2)
						{
							spawnRate = (int)((float)spawnRate * 0.7f);
						}
						else if ((double)Main.players[j].ActiveNPCs < (double)maxSpawns * 0.4)
						{
							spawnRate = (int)((float)spawnRate * 0.9f);
						}
					}
					if (Main.players[j].inventory[Main.players[j].selectedItemIndex].Type == 148)
					{
						spawnRate = (int)((double)spawnRate * 0.75);
						maxSpawns = (int)((float)maxSpawns * 1.5f);
					}
					if (Main.players[j].enemySpawns)
					{
						spawnRate = (int)((double)spawnRate * 0.5);
						maxSpawns = (int)((float)maxSpawns * 2f);
					}
					if ((double)spawnRate < (double)defaultSpawnRate * 0.1)
					{
						spawnRate = (int)((double)defaultSpawnRate * 0.1);
					}
					if (maxSpawns > defaultMaxSpawns * 3)
					{
						maxSpawns = defaultMaxSpawns * 3;
					}
					if (flag4)
					{
						maxSpawns = (int)((double)defaultMaxSpawns * (2.0 + 0.3 * (double)num3));
						spawnRate = 20;
					}
					if (Main.players[j].zoneDungeon && !downedBoss3)
					{
						spawnRate = 10;
					}
					bool flag6 = false;
					if (!flag4 && (!Main.bloodMoon || Main.dayTime) && !Main.players[j].zoneDungeon && !Main.players[j].zoneEvil && !Main.players[j].zoneMeteor)
					{
						if (Main.players[j].TownNPCs == 1f)
						{
							flag3 = true;
							if (Main.rand.Next(3) <= 1)
							{
								flag6 = true;
								maxSpawns = (int)((double)((float)maxSpawns) * 0.6);
							}
							else
							{
								spawnRate = (int)((float)spawnRate * 2f);
							}
						}
						else if (Main.players[j].TownNPCs == 2f)
						{
							flag3 = true;
							if (Main.rand.Next(3) == 0)
							{
								flag6 = true;
								maxSpawns = (int)((double)((float)maxSpawns) * 0.6);
							}
							else
							{
								spawnRate = (int)((float)spawnRate * 3f);
							}
						}
						else if (Main.players[j].TownNPCs >= 3f)
						{
							flag3 = true;
							flag6 = true;
							maxSpawns = (int)((double)((float)maxSpawns) * 0.6);
						}
					}
					if (Main.players[j].Active && !Main.players[j].dead && Main.players[j].ActiveNPCs < (float)maxSpawns && Main.rand.Next(spawnRate) == 0)
					{
						int num5 = (int)(Main.players[j].Position.X / 16f) - spawnRangeX;
						int num6 = (int)(Main.players[j].Position.X / 16f) + spawnRangeX;
						int num7 = (int)(Main.players[j].Position.Y / 16f) - spawnRangeY;
						int num8 = (int)(Main.players[j].Position.Y / 16f) + spawnRangeY;
						int num9 = (int)(Main.players[j].Position.X / 16f) - safeRangeX;
						int num10 = (int)(Main.players[j].Position.X / 16f) + safeRangeX;
						int num11 = (int)(Main.players[j].Position.Y / 16f) - safeRangeY;
						int num12 = (int)(Main.players[j].Position.Y / 16f) + safeRangeY;
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
							if (Main.tile.At(num13, num14).Active && Main.tileSolid[(int)Main.tile.At(num13, num14).Type])
							{
								goto IL_C34;
							}
							if (!Main.wallHouse[(int)Main.tile.At(num13, num14).Wall])
							{
								if (!flag4 && (double)num14 < Main.worldSurface * 0.34999999403953552 && !flag6 && ((double)num13 < (double)Main.maxTilesX * 0.45 || (double)num13 > (double)Main.maxTilesX * 0.55 || Main.hardMode))
								{
									num = num13;
									num2 = num14;
									flag = true;
									flag2 = true;
								}
								else if (!flag4 && (double)num14 < Main.worldSurface * 0.44999998807907104 && !flag6 && Main.hardMode && Main.rand.Next(10) == 0)
								{
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
												byte arg_B5A_0 = Main.tile.At(num13, l).Type;
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
									goto IL_C34;
								}
								int num15 = num - spawnSpaceX / 2;
								int num16 = num + spawnSpaceX / 2;
								int num17 = num2 - spawnSpaceY;
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
									goto IL_C34;
								}
								goto IL_C34;
							}
						IL_C3A:
							k++;
							continue;
						IL_C34:
							if (!flag && !flag)
							{
								goto IL_C3A;
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
								Rectangle rectangle2 = new Rectangle(
									(int)(Main.players[num19].Position.X + (float)(Main.players[num19].Width / 2) - (float)(sWidth / 2) - (float)safeRangeX),
									(int)(Main.players[num19].Position.Y + (float)(Main.players[num19].Height / 2) - (float)(sHeight / 2) - (float)safeRangeY),
								sWidth + safeRangeX * 2,
								sHeight + safeRangeY * 2);
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
							flag5 = true;
						}
					}
					if (flag)
					{
						flag = false;
						int num20 = (int)Main.tile.At(num, num2).Type;
						int npcIndex = 200;
						if (flag2)
						{
							if (Main.hardMode && Main.rand.Next(10) == 0 && !IsNPCSummoned((int)NPCType.N87_WYVERN_HEAD))
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N87_WYVERN_HEAD, 1);
							}
							else
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N48_HARPY, 0);
							}
						}
						else if (flag4)
						{
							if (Main.invasionType == InvasionType.GOBLIN_ARMY)
							{
								if (Main.rand.Next(9) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N29_GOBLIN_SORCERER, 0);
								}
								else if (Main.rand.Next(5) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N26_GOBLIN_PEON, 0);
								}
								else if (Main.rand.Next(3) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N111_GOBLIN_ARCHER, 0);
								}
								else if (Main.rand.Next(3) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N27_GOBLIN_THIEF, 0);
								}
								else
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N28_GOBLIN_WARRIOR, 0);
								}
							}
							else if (Main.invasionType == InvasionType.FROST_LEGION)
							{
								if (Main.rand.Next(7) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N145_SNOW_BALLA, 0);
								}
								else if (Main.rand.Next(3) == 0)
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N143_SNOWMAN_GANGSTA, 0);
								}
								else
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N144_MISTER_STABBY, 0);
								}
							}
						}
						else if (flag5 && (num < 250 || num > Main.maxTilesX - 250) && num20 == 53 && (double)num2 < Main.rockLayer)
						{
							if (Main.rand.Next(8) == 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N65_SHARK, 0);
							}
							if (Main.rand.Next(3) == 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N67_CRAB, 0);
							}
							else
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N64_PINK_JELLYFISH, 0);
							}
						}
						else if (flag5 && (((double)num2 > Main.rockLayer && Main.rand.Next(2) == 0) || num20 == 60))
						{
							if (Main.hardMode && Main.rand.Next(3) > 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N102_ANGLER_FISH, 0);
							}
							else
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N58_PIRANHA, 0);
							}
						}
						else if (flag5 && (double)num2 > Main.worldSurface && Main.rand.Next(3) == 0)
						{
							if (Main.hardMode)
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N103_GREEN_JELLYFISH, 0);
							}
							else
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N63_BLUE_JELLYFISH, 0);
							}
						}
						else if (flag5 && Main.rand.Next(4) == 0)
						{
							if (Main.players[j].zoneEvil)
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N57_CORRUPT_GOLDFISH, 0);
							}
							else
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N55_GOLDFISH, 0);
							}
						}
						else if (downedGoblins && Main.rand.Next(20) == 0 && !flag5 && (double)num2 >= Main.rockLayer && num2 < Main.maxTilesY - 210 && !savedGoblin && !IsNPCSummoned(105))
						{
							NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N105_BOUND_GOBLIN, 0);
						}
						else if (Main.hardMode && Main.rand.Next(20) == 0 && !flag5 && (double)num2 >= Main.rockLayer && num2 < Main.maxTilesY - 210 && !savedWizard && !IsNPCSummoned(106))
						{
							NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N106_BOUND_WIZARD, 0);
						}
						else if (flag6)
						{
							if (flag5)
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N55_GOLDFISH, 0);
							}
							else
							{
								if (num20 != 2 && num20 != 109 && num20 != 147 && (double)num2 <= Main.worldSurface)
								{
									return;
								}
								if (Main.rand.Next(2) == 0 && (double)num2 <= Main.worldSurface)
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N74_BIRD, 0);
								}
								else
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N46_BUNNY, 0);
								}
							}
						}
						else if (Main.players[j].zoneDungeon)
						{
							if (!downedBoss3)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N68_DUNGEON_GUARDIAN, 0);
							}
							else if (!savedMech && Main.rand.Next(5) == 0 && !flag5 && !IsNPCSummoned(123) && (double)num2 > Main.rockLayer)
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N123_BOUND_MECHANIC, 0);
							}
							else if (Main.rand.Next(37) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N71_DUNGEON_SLIME, 0);
							}
							else if (Main.rand.Next(4) == 0 && !NearSpikeBall(num, num2))
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N70_SPIKE_BALL, 0);
							}
							else if (Main.rand.Next(15) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N72_BLAZING_WHEEL, 0);
							}
							else if (Main.rand.Next(9) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N43_MAN_EATER, 0);
							}
							else if (Main.rand.Next(7) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N32_DARK_CASTER, 0);
							}
							else
							{
								var what = String.Empty;

								if (Main.rand.Next(4) == 0)
									what = "Big Boned";
								else if (Main.rand.Next(5) == 0)
									what = "Short Bones";

								if (what == String.Empty)
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N31_ANGRY_BONES, 0);
								else
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, what, 0);
							}
						}
						else if (Main.players[j].zoneMeteor)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N23_METEOR_HEAD, 0);
						}
						else if (Main.players[j].zoneEvil && Main.rand.Next(65) == 0)
						{
							if (Main.hardMode && Main.rand.Next(4) != 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N98_SEEKER_HEAD, 1);
							}
							else
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N07_DEVOURER_HEAD, 1);
							}
						}
						else if (Main.hardMode && (double)num2 > Main.worldSurface && Main.rand.Next(75) == 0)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N85_MIMIC, 0);
						}
						else if (Main.hardMode && Main.tile.At(num, num2 - 1).Wall == 2 && Main.rand.Next(20) == 0)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N85_MIMIC, 0);
						}
						else if (Main.hardMode && (double)num2 <= Main.worldSurface && !Main.dayTime && (Main.rand.Next(20) == 0 || (Main.rand.Next(5) == 0 && Main.moonPhase == 4)))
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N82_WRAITH, 0);
						}
						else if (num20 == 60 && Main.rand.Next(500) == 0 && !Main.dayTime)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N52_DOCTOR_BONES, 0);
						}
						else if (num20 == 60 && (double)num2 > (Main.worldSurface + Main.rockLayer) / 2.0)
						{
							if (Main.rand.Next(3) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N43_MAN_EATER, 0);
								Main.npcs[npcIndex].ai[0] = (float)num;
								Main.npcs[npcIndex].ai[1] = (float)num2;
								Main.npcs[npcIndex].netUpdate = true;
							}
							else
							{
								var what = String.Empty;

								if (Main.rand.Next(4) == 0)
									what = "Little Stinger";
								else if (Main.rand.Next(4) == 0)
									what = "Big Stinger";

								if (what == String.Empty)
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N42_HORNET, 0);
								else
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, what, 0);
							}
						}
						else if (num20 == 60 && Main.rand.Next(4) == 0)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N51_JUNGLE_BAT, 0);
						}
						else if (num20 == 60 && Main.rand.Next(8) == 0)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N56_SNATCHER, 0);
							Main.npcs[npcIndex].ai[0] = (float)num;
							Main.npcs[npcIndex].ai[1] = (float)num2;
							Main.npcs[npcIndex].netUpdate = true;
						}
						else if (Main.hardMode && num20 == 53 && Main.rand.Next(3) == 0)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N78_MUMMY, 0);
						}
						else if (Main.hardMode && num20 == 112 && Main.rand.Next(2) == 0)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N79_DARK_MUMMY, 0);
						}
						else if (Main.hardMode && num20 == 116 && Main.rand.Next(2) == 0)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N80_LIGHT_MUMMY, 0);
						}
						else if (Main.hardMode && !flag5 && (double)num2 < Main.rockLayer && (num20 == 116 || num20 == 117 || num20 == 109))
						{
							if (!Main.dayTime && Main.rand.Next(2) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N122_GASTROPOD, 0);
							}
							else if (Main.rand.Next(10) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N86_UNICORN, 0);
							}
							else
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N75_PIXIE, 0);
							}
						}
						else if (!flag3 && Main.hardMode && Main.rand.Next(50) == 0 && !flag5 && (double)num2 >= Main.rockLayer && (num20 == 116 || num20 == 117 || num20 == 109))
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N84_ENCHANTED_SWORD, 0);
						}
						else if ((num20 == 22 && Main.players[j].zoneEvil) || num20 == 23 || num20 == 25 || num20 == 112)
						{
							if (Main.hardMode && (double)num2 >= Main.rockLayer && Main.rand.Next(3) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N101_CLINGER, 0);
								Main.npcs[npcIndex].ai[0] = (float)num;
								Main.npcs[npcIndex].ai[1] = (float)num2;
								Main.npcs[npcIndex].netUpdate = true;
							}
							else if (Main.hardMode && Main.rand.Next(3) == 0)
							{
								if (Main.rand.Next(3) == 0)
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N121_SLIMER, 0);
								}
								else
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N81_CORRUPT_SLIME, 0);
								}
							}
							else if (Main.hardMode && (double)num2 >= Main.rockLayer && Main.rand.Next(40) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N83_CURSED_HAMMER, 0);
							}
							else if (Main.hardMode && (Main.rand.Next(2) == 0 || (double)num2 > Main.rockLayer))
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N94_CORRUPTOR, 0);
							}
							else
							{
								var what = String.Empty;

								if (Main.rand.Next(3) == 0)
									what = "Little Eater";
								else if (Main.rand.Next(3) == 0)
									what = "Big Eater";

								if (what == String.Empty)
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N06_EATER_OF_SOULS, 0);
								else
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, what, 0);
							}
						}
						else if ((double)num2 <= Main.worldSurface)
						{
							if (Main.dayTime)
							{
								int num22 = Math.Abs(num - Main.spawnTileX);
								if (num22 < Main.maxTilesX / 3 && Main.rand.Next(15) == 0 &&
									(num20 == 2 || num20 == 109 || num20 == 147))
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N46_BUNNY, 0);
								}
								else if (num22 < Main.maxTilesX / 3 && Main.rand.Next(15) == 0 && (num20 == 2 || num20 == 109 || num20 == 147))
								{
									NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N74_BIRD, 0);
								}
								else if (num22 > Main.maxTilesX / 3 && num20 == 2 && Main.rand.Next(300) == 0 && !IsNPCSummoned(50))
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N50_KING_SLIME, 0);
								}
								else if (num20 == 53 && Main.rand.Next(5) == 0 && !flag5)
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N69_ANTLION, 0);
								}
								else if (num20 == 53 && !flag5)
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N61_VULTURE, 0);
								}
								else if (num22 > Main.maxTilesX / 3 && Main.rand.Next(15) == 0)
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N73_GOBLIN_SCOUT, 0);
								}
								else
								{
									var what = "Blue Slime";

									if (num20 == 60)
										what = "Jungle Slime";
									else if (Main.rand.Next(3) == 0 || num22 < 200)
										what = "Green Slime";
									else if (Main.rand.Next(10) == 0 && num22 > 400)
										what = "Purple Slime";

									npcIndex = NewNPC(num * 16 + 8, num2 * 16, what, 0);
								}
							}
							else if (Main.rand.Next(6) == 0 || (Main.moonPhase == 4 && Main.rand.Next(2) == 0))
							{
								if (Main.hardMode && Main.rand.Next(3) == 0)
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N133_WANDERING_EYE, 0);
								}
								else
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N02_DEMON_EYE, 0);
								}
							}
							else if (Main.hardMode && Main.rand.Next(50) == 0 && Main.bloodMoon && !IsNPCSummoned(109))
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N109_CLOWN, 0);
							}
							else if (Main.rand.Next(250) == 0 && Main.bloodMoon)
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N53_THE_GROOM, 0);
							}
							else if (Main.moonPhase == 0 && Main.hardMode && Main.rand.Next(3) != 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N104_WEREWOLF, 0);
							}
							else if (Main.hardMode && Main.rand.Next(3) == 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N140_POSSESSED_ARMOR, 0);
							}
							else if (Main.rand.Next(3) == 0)
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N132_BALD_ZOMBIE, 0);
							}
							else
							{
								NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N03_ZOMBIE, 0);
							}
						}
						else if ((double)num2 <= Main.rockLayer)
						{
							if (!flag3 && Main.rand.Next(50) == 0)
							{
								if (Main.hardMode)
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N95_DIGGER_HEAD, 1);
								}
								else
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N10_GIANT_WORM_HEAD, 1);
								}
							}
							else if (Main.hardMode && Main.rand.Next(3) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N140_POSSESSED_ARMOR, 0);
							}
							else if (Main.hardMode && Main.rand.Next(4) != 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N141_TOXIC_SLUDGE, 0);
							}
							else
							{
								var what = "Red Slime";

								if (Main.rand.Next(5) == 0)
									what = "Yellow Slime";
								else if (Main.rand.Next(2) == 0)
									what = "Blue Slime";

								npcIndex = NewNPC(num * 16 + 8, num2 * 16, what, 0);
							}
						}
						else if (num2 > Main.maxTilesY - 190)
						{
							if (Main.rand.Next(40) == 0 && !IsNPCSummoned((int)NPCType.N39_BONE_SERPENT_HEAD))
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N39_BONE_SERPENT_HEAD, 1);
							}
							else if (Main.rand.Next(14) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N24_FIRE_IMP, 0);
							}
							else if (Main.rand.Next(8) == 0)
							{
								if (Main.rand.Next(7) == 0)
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N66_VOODOO_DEMON, 0);
								}
								else
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N62_DEMON, 0);
								}
							}
							else if (Main.rand.Next(3) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N59_LAVA_SLIME, 0);
							}
							else
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N60_HELLBAT, 0);
							}
						}
						else if ((num20 == 116 || num20 == 117) && !flag3 && Main.rand.Next(8) == 0)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N120_CHAOS_ELEMENTAL, 0);
						}
						else if (!flag3 && Main.rand.Next(75) == 0 && !Main.players[j].zoneHoly)
						{
							if (Main.hardMode)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N95_DIGGER_HEAD, 1);
							}
							else
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N10_GIANT_WORM_HEAD, 1);
							}
						}
						else if (!Main.hardMode && Main.rand.Next(10) == 0)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N16_MOTHER_SLIME, 0);
						}
						else if (!Main.hardMode && Main.rand.Next(4) == 0)
						{
							var what = "Black Slime";

							if (Main.players[j].zoneJungle)
								what = "Jungle Slime";

							npcIndex = NewNPC(num * 16 + 8, num2 * 16, what, 0);
						}
						else if (Main.rand.Next(2) == 0)
						{
							if ((double)num2 > (Main.rockLayer + (double)Main.maxTilesY) / 2.0 && Main.rand.Next(700) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N45_TIM, 0);
							}
							else if (Main.hardMode && Main.rand.Next(10) != 0)
							{
								if (Main.rand.Next(2) == 0)
								{
									if ((double)num2 > (Main.rockLayer + (double)Main.maxTilesY) / 2.0 && Main.rand.Next(5) == 0)
										npcIndex = NewNPC(num * 16 + 8, num2 * 16, "Heavy Skeleton", 0);
									else
										npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N77_ARMORED_SKELETON, 0);
								}
								else
								{
									npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N110_SKELETON_ARCHER, 0);
								}
							}
							else if (Main.rand.Next(15) == 0)
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N44_UNDEAD_MINER, 0);
							}
							else
							{
								npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N21_SKELETON, 0);
							}
						}
						else if (Main.hardMode && (Main.players[j].zoneHoly & Main.rand.Next(2) == 0))
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N138_ILLUMINANT_SLIME, 0);
						}
						else if (Main.players[j].zoneJungle)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N51_JUNGLE_BAT, 0);
						}
						else if (Main.hardMode && Main.players[j].zoneHoly)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N137_ILLUMINANT_BAT, 0);
						}
						else if (Main.hardMode && Main.rand.Next(6) > 0)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N93_GIANT_BAT, 0);
						}
						else
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, (int)NPCType.N49_CAVE_BAT, 0);
						}

						if (Main.npcs[npcIndex].Type == 1 && Main.rand.Next(250) == 0)
						{
							npcIndex = NewNPC(num * 16 + 8, num2 * 16, "Pinky", 0);
						}
						if (npcIndex < 200)
						{
							NetMessage.SendData(23, -1, -1, "", npcIndex, 0f, 0f, 0f, 0);
							return;
						}
						break;
					}
				}
			}
		}

		/// <summary>
		/// Spawns specified NPC type on specified player. (Generally a BOSS)
		/// </summary>
		/// <param name="playerIndex">Index of player to spawn on</param>
		/// <param name="Type">Type of NPC to spawn</param>
		/// <param name="makespawn">Forces the NPC to spawn even if spawning is disallowed</param>
		public static void SpawnOnPlayer(int playerIndex, int Type, bool makespawn = false)
		{
			if (Main.stopSpawns && !makespawn)
				return;

			if (!makespawn && IsNPCSummoned(Type)) //Monitor this, Possible hack
			{
				return;
			}

			bool flag = false;
			int x = 0;
			int y = 0;
			int num3 = (int)(Main.players[playerIndex].Position.X / 16f) - spawnRangeX * 2;
			int num4 = (int)(Main.players[playerIndex].Position.X / 16f) + spawnRangeX * 2;
			int num5 = (int)(Main.players[playerIndex].Position.Y / 16f) - spawnRangeY * 2;
			int num6 = (int)(Main.players[playerIndex].Position.Y / 16f) + spawnRangeY * 2;
			int num7 = (int)(Main.players[playerIndex].Position.X / 16f) - safeRangeX;
			int num8 = (int)(Main.players[playerIndex].Position.X / 16f) + safeRangeX;
			int num9 = (int)(Main.players[playerIndex].Position.Y / 16f) - safeRangeY;
			int num10 = (int)(Main.players[playerIndex].Position.Y / 16f) + safeRangeY;

			if (num3 < 0)
				num3 = 0;
			if (num4 > Main.maxTilesX)
				num4 = Main.maxTilesX;
			if (num5 < 0)
				num5 = 0;
			if (num6 > Main.maxTilesY)
				num6 = Main.maxTilesY;

			for (int i = 0; i < 1000; i++)
			{
				int j = 0;
				while (j < 100)
				{
					int num11 = Main.rand.Next(num3, num4);
					int num12 = Main.rand.Next(num5, num6);
					if (Main.tile.At(num11, num12).Active && Main.tileSolid[(int)Main.tile.At(num11, num12).Type])
					{
						if (!flag && !flag)
						{
							j++;
							continue;
						}
						break;
					}
					if (!Main.wallHouse[(int)Main.tile.At(num11, num12).Wall] || i >= 999)
					{
						int k = num12;
						while (k < Main.maxTilesY)
						{
							if (Main.tile.At(num11, k).Active && Main.tileSolid[(int)Main.tile.At(num11, k).Type])
							{
								if (num11 < num7 || num11 > num8 || k < num9 || k > num10 || i == 999)
								{
									x = num11;
									y = k;
									flag = true;
								}
								break;
							}
							else
							{
								k++;
							}
						}
						if (!flag || i >= 999)
						{
							if (!flag && !flag)
							{
								j++;
								continue;
							}
							break;
						}
						int num13 = x - spawnSpaceX / 2;
						int num14 = x + spawnSpaceX / 2;
						int num15 = y - spawnSpaceY;
						int num16 = y;
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
									if (Main.tile.At(l, m).Active && Main.tileSolid[(int)Main.tile.At(l, m).Type])
									{
										flag = false;
										break;
									}
								}
							}
						}
						j++;
						continue;
					}
				}
				if (flag && i < 999)
				{
					Rectangle rectangle = new Rectangle(x * 16, y * 16, 16, 16);
					for (int n = 0; n < Main.MAX_PLAYERS; n++)
					{
						if (Main.players[n].Active)
						{
							Rectangle rectangle2 = new Rectangle((int)(Main.players[n].Position.X + (float)(Main.players[n].Width / 2) - (float)(sWidth / 2) -
								(float)safeRangeX), (int)(Main.players[n].Position.Y + (float)(Main.players[n].Height / 2) - (float)(sHeight / 2) -
								(float)safeRangeY), sWidth + safeRangeX * 2, sHeight + safeRangeY * 2);
							if (rectangle.Intersects(rectangle2))
								flag = true;
						}
					}
				}
				if (flag)
					break;
			}
			if (flag)
			{
				var player = Main.players[playerIndex];
				var ctx = new HookContext
				{
					Connection = player.Connection,
					Sender = player,
					Player = player,
				};

				var args = new HookArgs.PlayerTriggeredEvent
				{
					X = x,
					Y = y,
					Type = WorldEventType.BOSS,
					Name = ((NPCType)Type).ToString()
				};

				HookPoints.PlayerTriggeredEvent.Invoke(ref ctx, ref args);

				if (ctx.CheckForKick())
					return;
				else if (ctx.Result != HookResult.IGNORE)
				{
					if (Type == (int)NPCType.N113_WALL_OF_FLESH && Main.hardMode)
					{
						Main.hardMode = false;
						var msg = NetMessage.PrepareThreadInstance();
						msg.WorldData(false);
						msg.Broadcast();
					}

					int npcIndex = NewNPC(x * 16 + 8, y * 16, Type, 1, makespawn);
					if (npcIndex == 200)
						return;

					Main.npcs[npcIndex].target = playerIndex;
					Main.npcs[npcIndex].timeLeft *= 20;

					string npcName = Main.npcs[npcIndex].Name;
					if (!String.IsNullOrEmpty(Main.npcs[npcIndex].DisplayName))
						npcName = Main.npcs[npcIndex].DisplayName;

					if (npcIndex < 200)
						NetMessage.SendData(23, -1, -1, String.Empty, npcIndex);

					/*if (Type == (int)NPCType.N125_RETINAZER)
					{
						NetMessage.SendData(25, -1, -1, "The Twins have awoken!", 255, 175f, 75f, 255f);
						return;
					}
					else if (Type != (int)NPCType.N82_WRAITH && Type != (int)NPCType.N126_SPAZMATISM && Type != (int)NPCType.N50_KING_SLIME)
						NetMessage.SendData(25, -1, -1, npcName + " has awoken!", 255, 175f, 75f, 255f);*/

					if (Type != (int)NPCType.N82_WRAITH && Type != (int)NPCType.N126_SPAZMATISM && Type != (int)NPCType.N50_KING_SLIME)
					{
						if (Type == (int)NPCType.N125_RETINAZER)
						{
							npcName = "The Twins";
							ProgramLog.Users.Log("{0} @ {1}: {3} summoned by {2}.", player.IPAddress, player.whoAmi, player.Name, npcName);

							var twinsMessage = String.Format("{0} have been awoken by {1}", npcName, player.Name);
							NetMessage.SendData(Packet.PLAYER_CHAT, -1, -1, twinsMessage, 255, 255, 128, 150);
						}

						ProgramLog.Users.Log("{0} @ {1}: {3} summoned by {2}.", player.IPAddress, player.whoAmi, player.Name, npcName);

						var bossMessage = String.Format("{0} has summoned the {1}!", player.Name, npcName);
						NetMessage.SendData(Packet.PLAYER_CHAT, -1, -1, bossMessage, 255, 255, 128, 150);
					}
				}
			}
		}

		static bool InvokeNpcCreationHook(int x, int y, string type, out NPC npc)
		{
			npc = null;

			if (!WorldModify.gen)
			{
				var ctx = new HookContext
				{
				};

				var args = new HookArgs.NpcCreation
				{
					X = x,
					Y = y,
					Name = type,
				};

				HookPoints.NpcCreation.Invoke(ref ctx, ref args);

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
		/// <param name="start">Index to start from looking for free index</param>
		/// <param name="y">Y coordinate to create at</param>
		/// <param name="type">Type of NPC to create</param>
		/// <param name="makespawn">Forces the NPC to spawn even if spawning is disabled</param>
		/// <returns>Main.npcs[] index value</returns>
		public static int NewNPC(int x, int y, int type, int start = 0, bool makespawn = false)
		{
			if (Main.stopSpawns && !makespawn)
				return MAX_NPCS;

			NPC hnpc;
			if (!InvokeNpcCreationHook(x, y, Registries.NPC.GetTemplate(type).Name, out hnpc))
				return MAX_NPCS;

			int id = FindNPCSlot(start);
			if (id >= 0)
			{
				var npc = hnpc ?? Registries.NPC.Create(type);
				id = NewNPC(x, y, npc, id, makespawn);

				//if (id >= 0 && type == 50)
				//{
				//    //TODO: move elsewhere
				//    NetMessage.SendData(25, -1, -1, npc.Name + " has awoken!", 255, 175f, 75f, 255f);
				//}

				if (NPCSpawnHandler != null)
					NPCSpawnHandler.Invoke(id);

				return id;
			}
			return MAX_NPCS;
		}

		public static int NewNPC(int x, int y, string name, int start = 0, bool makespawn = false)
		{
			if (Main.stopSpawns && !makespawn)
				return MAX_NPCS;

			NPC hnpc;
			if (!InvokeNpcCreationHook(x, y, name, out hnpc))
				return MAX_NPCS;

			int id = FindNPCSlot(start);
			if (id >= 0)
			{
				NewNPC(x, y, hnpc ?? Registries.NPC.Create(name), id, makespawn);

				if (NPCSpawnHandler != null)
					NPCSpawnHandler.Invoke(id);

				return id;
			}
			return MAX_NPCS;
		}

		public static int FindNPCSlot(int start)
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

		public static int NewNPC(int x, int y, NPC npc, int npcIndex, bool makespawn = false)
		{
			if (Main.stopSpawns && !makespawn)
				return MAX_NPCS;

			if (npc.townNPC && IsNPCSummoned(npc.type))
				return MAX_NPCS;

			npc.Position.X = (float)(x - npc.Width / 2);
			npc.Position.Y = (float)(y - npc.Height);
			npc.Active = true;
			npc.timeLeft = (int)((double)ACTIVE_TIME * 1.25);
			npc.wet = Collision.WetCollision(npc.Position, npc.Width, npc.Height);

			npc.MadeSpawn = makespawn;

			Main.npcs[npcIndex] = npc;

			return npcIndex;
		}

		public void SetDefaults(int type)
		{
			if (Main.stopSpawns)
				return;

			Transfer(type);
		}

		/// <summary>
		/// Mainly to manage the code when cloning and transfroming NPC's
		/// </summary>
		/// <param name="type">New type of the NPC</param>
		public void Transfer(int type)
		{
			/* Preserve data - [todo] Look into others */
			var pos = Position;

			Registries.NPC.SetDefaults(this, type);

			life = lifeMax;
			defDamage = damage;
			defDefense = defense;
			NetID = Type;
			Active = true;

			Position = pos;
		}

		/// <summary>
		/// Transforms specified NPC into specified type.
		/// Used currently for bunny/goldfish to evil bunny/goldfish and Eater of Worlds segmenting transformations
		/// </summary>
		/// <param name="newType"></param>
		public void Transform(int newType)
		{
			Transfer(newType);

			TargetClosest(true);
			netUpdate = true;
			NetMessage.SendData(23, -1, -1, String.Empty, whoAmI);
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
		public bool StrikeNPC(ISender aggressor, int Damage, float knockBack, int hitDirection, bool crit = false)
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

			HookPoints.NpcHurt.Invoke(ref ctx, ref args);

			if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
				return false;

			StrikeNPCInternal(args.Damage, args.Knockback, args.HitDirection, args.Critical);

			return true;
		}

		public double StrikeNPCInternal(int Damage, float knockBack, int hitDirection, bool crit)
		{
			if (!this.Active || this.life <= 0)
				return 0.0;

			double damage = Main.CalculateDamage((int)Damage, this.defense);
			if (crit)
				damage *= 2.0;

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

				if (this.realLife >= 0)
				{
					Main.npcs[this.realLife].life -= (int)damage;
					this.life = Main.npcs[this.realLife].life;
					this.lifeMax = Main.npcs[this.realLife].lifeMax;
				}
				else
					this.life -= (int)damage;

				if (knockBack > 0f && this.knockBackResist > 0f)
				{
					float vel = knockBack * this.knockBackResist;
					if (vel > 8f)
						vel = 8f;

					if (crit)
						vel *= 1.4f;

					if (damage * 10.0 < (double)this.lifeMax)
					{
						if (hitDirection < 0 && this.Velocity.X > -vel)
						{
							if (this.Velocity.X > 0f)
								this.Velocity.X = this.Velocity.X - vel;

							this.Velocity.X = this.Velocity.X - vel;

							if (this.Velocity.X < -vel)
								this.Velocity.X = -vel;
						}
						else if (hitDirection > 0 && this.Velocity.X < vel)
						{
							if (this.Velocity.X < 0f)
								this.Velocity.X = this.Velocity.X + vel;

							this.Velocity.X = this.Velocity.X + vel;

							if (this.Velocity.X > vel)
								this.Velocity.X = vel;
						}

						vel *= (!noGravity) ? -0.75f : -0.5f;

						if (this.Velocity.Y > vel)
						{
							this.Velocity.Y = this.Velocity.Y + vel;

							if (this.Velocity.Y < vel)
								this.Velocity.Y = vel;
						}
					}
					else
					{
						var val = (!noGravity) ? 0.75f : 0.5f;

						this.Velocity.Y = -vel * val * this.knockBackResist;
						this.Velocity.X = vel * (float)hitDirection * this.knockBackResist;
					}
				}
				if ((this.type == NPCType.N113_WALL_OF_FLESH || this.type == NPCType.N114_WALL_OF_FLESH_EYE) && this.life <= 0)
				{
					for (int i = 0; i < MAX_NPCS; i++)
					{
						if (Main.npcs[i].Active && (Main.npcs[i].type == NPCType.N113_WALL_OF_FLESH ||
							Main.npcs[i].type == NPCType.N114_WALL_OF_FLESH_EYE))
						{
							Main.npcs[i].HitEffect(hitDirection, damage);
						}
					}
				}
				else
					this.HitEffect(hitDirection, damage);

				var npc = (realLife >= 0) ? Main.npcs[this.realLife] : this;
				npc.CheckDead();

				return damage;
			}
			return 0.0;
		}

		/// <summary>
		/// Drops loot from dead NPC
		/// </summary>
		public void NPCLoot()
		{
			if (Main.hardMode && this.lifeMax > 1 && this.damage > 0 && !this.friendly &&
				(double)this.Position.Y > Main.rockLayer * 16.0 && Main.rand.Next(7) == 0 && this.type != NPCType.N121_SLIMER && this.value > 0f)
			{
				if (Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].zoneEvil)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 521, 1, false, 0);

				if (Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].zoneHoly)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 520, 1, false, 0);
			}

			if (Main.Xmas && this.lifeMax > 1 && this.damage > 0 && !this.friendly && this.type != NPCType.N121_SLIMER && this.value > 0f && Main.rand.Next(13) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, Main.rand.Next(599, 602), 1, false, 0);

			if (this.type == NPCType.N109_CLOWN && !downedClown)
			{
				downedClown = true;
				NetMessage.SendData(7);
			}

			if (this.type == NPCType.N85_MIMIC && this.value > 0f)
			{
				int rdm = Main.rand.Next(7);
				var item = -1;

				switch (rdm)
				{
					case 0:
						item = 437;
						break;
					case 1:
						item = 517;
						break;
					case 2:
						item = 535;
						break;
					case 3:
						item = 536;
						break;
					case 4:
						item = 532;
						break;
					case 5:
						item = 393;
						break;
					case 6:
						item = 554;
						break;
				}

				if (item != -1)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, item, 1, false, -1);
			}

			if (this.type == NPCType.N87_WYVERN_HEAD)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 575, Main.rand.Next(5, 11), false, 0);

			if (this.type == NPCType.N143_SNOWMAN_GANGSTA || this.type == NPCType.N144_MISTER_STABBY || this.type == NPCType.N145_SNOW_BALLA)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 593, Main.rand.Next(5, 11), false, 0);

			if (this.type == NPCType.N79_DARK_MUMMY)
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 527, 1, false, 0);
			}
			else if (this.type == NPCType.N80_LIGHT_MUMMY && Main.rand.Next(10) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 528, 1, false, 0);

			if (this.type == NPCType.N101_CLINGER || this.type == NPCType.N98_SEEKER_HEAD)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 522, Main.rand.Next(2, 6), false, 0);

			if (this.type == NPCType.N86_UNICORN)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 526, 1, false, 0);

			if (this.type == NPCType.N113_WALL_OF_FLESH)
			{
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 367, 1, false, -1);

				if (Main.rand.Next(2) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, Main.rand.Next(489, 492), 1, false, -1);
				else
				{
					int rdm = Main.rand.Next(3);
					var item = -1;

					switch (rdm)
					{
						case 0:
							item = 514;
							break;
						case 1:
							item = 426;
							break;
						case 2:
							item = 434;
							break;
					}

					if (item != -1)
						Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, item, 1, false, -1);
				}

				int num3 = (int)(this.Position.X + (float)(this.Width / 2)) / 16;
				int num4 = (int)(this.Position.Y + (float)(this.Height / 2)) / 16;
				int num5 = this.Width / 2 / 16 + 1;
				for (int i = num3 - num5; i <= num3 + num5; i++)
				{
					for (int j = num4 - num5; j <= num4 + num5; j++)
					{
						if ((i == num3 - num5 || i == num3 + num5 || j == num4 - num5 || j == num4 + num5) && !Main.tile.At(i, j).Active)
						{
							Main.tile.At(i, j).SetType(140);
							Main.tile.At(i, j).SetActive(true);
						}
						Main.tile.At(i, j).SetLava(false);
						Main.tile.At(i, j).SetLiquid(0);

						NetMessage.SendTileSquare(-1, i, j, 1);
					}
				}
			}

			if (this.type == NPCType.N01_BLUE_SLIME || this.type == NPCType.N16_MOTHER_SLIME || this.type == NPCType.N138_ILLUMINANT_SLIME || this.type == NPCType.N141_TOXIC_SLUDGE)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 23, Main.rand.Next(1, 3), false, 0);

			if (this.type == NPCType.N75_PIXIE)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 501, Main.rand.Next(1, 4), false, 0);

			if (this.type == NPCType.N81_CORRUPT_SLIME)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 23, Main.rand.Next(2, 5), false, 0);

			if (this.type == NPCType.N122_GASTROPOD)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 23, Main.rand.Next(5, 11), false, 0);

			if (this.type == NPCType.N71_DUNGEON_SLIME)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 327, 1, false, 0);

			if (this.type == NPCType.N02_DEMON_EYE)
			{
				if (Main.rand.Next(3) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 38, 1, false, 0);
				else if (Main.rand.Next(100) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 236, 1, false, 0);
			}

			if (this.type == NPCType.N104_WEREWOLF && Main.rand.Next(60) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 485, 1, false, -1);

			if (this.type == NPCType.N58_PIRANHA)
			{
				if (Main.rand.Next(500) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 263, 1, false, 0);
				else if (Main.rand.Next(40) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 118, 1, false, 0);
			}

			if (this.type == NPCType.N102_ANGLER_FISH && Main.rand.Next(500) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 263, 1, false, 0);

			if ((this.type == NPCType.N03_ZOMBIE || this.type == NPCType.N132_BALD_ZOMBIE) && Main.rand.Next(50) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 216, 1, false, -1);

			if (this.type == NPCType.N66_VOODOO_DEMON)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 267, 1, false, 0);

			if (this.type == NPCType.N62_DEMON && Main.rand.Next(50) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 272, 1, false, -1);

			if (this.type == NPCType.N52_DOCTOR_BONES)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 251, 1, false, 0);

			if (this.type == NPCType.N53_THE_GROOM)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 239, 1, false, 0);

			if (this.type == NPCType.N54_CLOTHIER)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 260, 1, false, 0);

			if (this.type == NPCType.N55_GOLDFISH)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 261, 1, false, 0);

			if (this.type == NPCType.N69_ANTLION && Main.rand.Next(7) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 323, 1, false, 0);

			if (this.type == NPCType.N73_GOBLIN_SCOUT)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 362, Main.rand.Next(1, 3), false, 0);

			if (this.type == NPCType.N04_EYE_OF_CTHULHU)
			{
				int stack = Main.rand.Next(30) + 20;
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 47, stack, false, 0);
				stack = Main.rand.Next(20) + 10;
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 56, stack, false, 0);
				stack = Main.rand.Next(20) + 10;
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 56, stack, false, 0);
				stack = Main.rand.Next(20) + 10;
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 56, stack, false, 0);
				stack = Main.rand.Next(3) + 1;
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 59, stack, false, 0);
			}

			if ((this.type == NPCType.N06_EATER_OF_SOULS || this.type == NPCType.N94_CORRUPTOR) && Main.rand.Next(3) == 0)
			{
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 68, 1, false, 0);
			}
			if (this.type == NPCType.N07_DEVOURER_HEAD || this.type == NPCType.N08_DEVOURER_BODY || this.type == NPCType.N09_DEVOURER_TAIL)
			{
				if (Main.rand.Next(3) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 68, Main.rand.Next(1, 3), false, 0);

				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 69, Main.rand.Next(3, 9), false, 0);
			}
			if ((this.type == NPCType.N10_GIANT_WORM_HEAD || this.type == NPCType.N11_GIANT_WORM_BODY || this.type == NPCType.N12_GIANT_WORM_TAIL ||
				this.type == NPCType.N95_DIGGER_HEAD || this.type == NPCType.N96_DIGGER_BODY || this.type == NPCType.N97_DIGGER_TAIL) && Main.rand.Next(500) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 215, 1, false, 0);

			if (this.type == NPCType.N47_CORRUPT_BUNNY && Main.rand.Next(75) == 0)
			{
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 243, 1, false, 0);
			}
			if (this.type == NPCType.N13_EATER_OF_WORLDS_HEAD || this.type == NPCType.N14_EATER_OF_WORLDS_BODY || this.type == NPCType.N15_EATER_OF_WORLDS_TAIL)
			{
				int stack2 = Main.rand.Next(1, 3);
				if (Main.rand.Next(2) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 86, stack2, false, 0);

				if (Main.rand.Next(2) == 0)
				{
					stack2 = Main.rand.Next(2, 6);
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 56, stack2, false, 0);
				}
				if (this.boss)
				{
					stack2 = Main.rand.Next(10, 30);
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 56, stack2, false, 0);
					stack2 = Main.rand.Next(10, 31);
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 56, stack2, false, 0);
				}
				if (Main.rand.Next(3) == 0 && Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statLife < Main.players[
					(int)Player.FindClosest(this.Position, this.Width, this.Height)].statLifeMax)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 58, 1, false, 0);
			}
			if (this.type == NPCType.N116_THE_HUNGRY_II || this.type == NPCType.N117_LEECH_HEAD || this.type == NPCType.N118_LEECH_BODY ||
				this.type == NPCType.N119_LEECH_TAIL || this.type == NPCType.N139_PROBE)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 58, 1, false, 0);

			if (this.type == NPCType.N63_BLUE_JELLYFISH || this.type == NPCType.N64_PINK_JELLYFISH || this.type == NPCType.N103_GREEN_JELLYFISH)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 282, Main.rand.Next(1, 5), false, 0);

			if (this.type == NPCType.N21_SKELETON || this.type == NPCType.N44_UNDEAD_MINER)
			{
				if (Main.rand.Next(25) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 118, 1, false, 0);
				else if (this.type == NPCType.N44_UNDEAD_MINER)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, Main.rand.Next(410, 412), 1, false, 0);
					else
						Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 166, Main.rand.Next(1, 4), false, 0);
				}
			}

			if (this.type == NPCType.N45_TIM)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 238, 1, false, 0);

			if (this.type == NPCType.N50_KING_SLIME)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, Main.rand.Next(256, 259), 1, false, 0);

			if (this.type == NPCType.N23_METEOR_HEAD && Main.rand.Next(50) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 116, 1, false, 0);

			if (this.type == NPCType.N24_FIRE_IMP && Main.rand.Next(300) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 244, 1, false, 0);

			if (this.type == NPCType.N31_ANGRY_BONES || this.type == NPCType.N32_DARK_CASTER || this.type == NPCType.N34_CURSED_SKULL)
			{
				if (Main.rand.Next(65) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 327, 1, false, 0);
				else
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 154, Main.rand.Next(1, 4), false, 0);
			}
			if (this.type == NPCType.N26_GOBLIN_PEON || this.type == NPCType.N27_GOBLIN_THIEF || this.type == NPCType.N28_GOBLIN_WARRIOR ||
				this.type == NPCType.N29_GOBLIN_SORCERER || this.type == NPCType.N111_GOBLIN_ARCHER)
			{
				if (Main.rand.Next(200) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 160, 1, false, 0);
				else if (Main.rand.Next(2) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 161, Main.rand.Next(1, 6), false, 0);
			}

			if (this.type == NPCType.N42_HORNET && Main.rand.Next(2) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 209, 1, false, 0);

			if (this.type == NPCType.N43_MAN_EATER && Main.rand.Next(4) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 210, 1, false, 0);

			if (this.type == NPCType.N65_SHARK)
			{
				if (Main.rand.Next(50) == 0)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 268, 1, false, 0);
				else
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 319, 1, false, 0);
			}

			if (this.type == NPCType.N48_HARPY && Main.rand.Next(2) == 0)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 320, 1, false, 0);

			if (this.type == NPCType.N125_RETINAZER || this.type == NPCType.N126_SPAZMATISM)
			{
				int npcType = (this.type == NPCType.N125_RETINAZER) ? (int)NPCType.N126_SPAZMATISM : (int)NPCType.N125_RETINAZER;

				if (!IsNPCSummoned(npcType))
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 549, Main.rand.Next(20, 31), false, 0);
				else
				{
					this.value = 0f;
					this.boss = false;
				}
			}
			else
			{
				if (this.type == NPCType.N127_SKELETRON_PRIME)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 547, Main.rand.Next(20, 31), false, 0);
				else if (this.type == NPCType.N134_THE_DESTROYER)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 548, Main.rand.Next(20, 31), false, 0);
			}
			if (this.boss)
			{
				if (this.type == NPCType.N04_EYE_OF_CTHULHU)
					downedBoss1 = true;
				else
				{
					if (this.type == NPCType.N13_EATER_OF_WORLDS_HEAD || this.type == NPCType.N134_THE_DESTROYER || this.type == NPCType.N15_EATER_OF_WORLDS_TAIL)
					{
						downedBoss2 = true;
						this.Name = "Eater of Worlds";
					}
					else if (this.type == NPCType.N35_SKELETRON_HEAD)
					{
						downedBoss3 = true;
						this.Name = "Skeletron";
					}
					else
						this.Name = this.DisplayName;
				}
				string str = this.Name;
				if (!String.IsNullOrEmpty(this.DisplayName))
					str = this.DisplayName;

				int stack4 = Main.rand.Next(5, 16);
				int num7 = 28;
				if (this.type == NPCType.N113_WALL_OF_FLESH)
					num7 = 188;

				if (this.type > NPCType.N113_WALL_OF_FLESH)
					num7 = 499;

				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, num7, stack4, false, 0);

				int num8 = Main.rand.Next(5) + 5;
				for (int k = 0; k < num8; k++)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 58, 1, false, 0);

				if (this.type == NPCType.N125_RETINAZER || this.type == NPCType.N126_SPAZMATISM)
				{
					if (!IsNPCSummoned(NPCType.N125_RETINAZER) && !IsNPCSummoned(NPCType.N126_SPAZMATISM))
						NetMessage.SendData(25, -1, -1, "The Twins have been defeated!", 255, 175f, 75f, 255f, 0);
				}
				else
					NetMessage.SendData(25, -1, -1, str + " has been defeated!", 255, 175f, 75f, 255f, 0);

				if (this.type == NPCType.N113_WALL_OF_FLESH || this.type == NPCType.N114_WALL_OF_FLESH_EYE)
					WorldModify.StartHardMode();

				NetMessage.SendData(7);
			}

			if (Main.rand.Next(6) == 0 && this.lifeMax > 1 && this.damage > 0)
			{
				if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statMana < Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statManaMax)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 184, 1, false, 0);
				else if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statLife < Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statLifeMax)
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 58, 1, false, 0);
			}

			if (Main.rand.Next(2) == 0 && this.lifeMax > 1 && this.damage > 0 && Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statMana < Main.players[(int)Player.FindClosest(this.Position, this.Width, this.Height)].statManaMax)
				Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 184, 1, false, 0);

			float num9 = this.value;
			num9 *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
			if (Main.rand.Next(5) == 0)
				num9 *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;

			if (Main.rand.Next(10) == 0)
				num9 *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;

			if (Main.rand.Next(15) == 0)
				num9 *= 1f + (float)Main.rand.Next(15, 31) * 0.01f;

			if (Main.rand.Next(20) == 0)
				num9 *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;

			while ((int)num9 > 0)
			{
				if (num9 > 1000000f)
				{
					int num10 = (int)(num9 / 1000000f);
					if (num10 > 50 && Main.rand.Next(5) == 0)
						num10 /= Main.rand.Next(3) + 1;

					if (Main.rand.Next(5) == 0)
						num10 /= Main.rand.Next(3) + 1;

					num9 -= (float)(1000000 * num10);
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 74, num10, false, 0);
				}
				else if (num9 > 10000f)
				{
					int num11 = (int)(num9 / 10000f);
					if (num11 > 50 && Main.rand.Next(5) == 0)
						num11 /= Main.rand.Next(3) + 1;

					if (Main.rand.Next(5) == 0)
						num11 /= Main.rand.Next(3) + 1;

					num9 -= (float)(10000 * num11);
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 73, num11, false, 0);
				}
				else if (num9 > 100f)
				{
					int num12 = (int)(num9 / 100f);
					if (num12 > 50 && Main.rand.Next(5) == 0)
						num12 /= Main.rand.Next(3) + 1;

					if (Main.rand.Next(5) == 0)
						num12 /= Main.rand.Next(3) + 1;

					num9 -= (float)(100 * num12);
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 72, num12, false, 0);
				}
				else
				{
					int num13 = (int)num9;
					if (num13 > 50 && Main.rand.Next(5) == 0)
						num13 /= Main.rand.Next(3) + 1;

					if (Main.rand.Next(5) == 0)
						num13 /= Main.rand.Next(4) + 1;

					if (num13 < 1)
						num13 = 1;

					num9 -= (float)num13;
					Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 71, num13, false, 0);
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
				if (this.life <= 0)
				{
					if (this.type == NPCType.N16_MOTHER_SLIME)
					{
						int spawnedSlimes = Main.rand.Next(2) + 2;
						for (int slimeNum = 0; slimeNum < spawnedSlimes; slimeNum++)
						{
							int npcIndex = NewNPC((int)(this.Position.X + (float)(this.Width / 2)), (int)(this.Position.Y + (float)this.Height), "Baby Slime", 0);
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
					int npcIndex = NewNPC(x, y, 1, 0);
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
		public static void SpawnSkeletron(Player player)
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
				int npcIndex = NewNPC((int)vector.X + width / 2, (int)vector.Y + height / 2, 35, 0);
				Main.npcs[npcIndex].netUpdate = true;
				//NetMessage.SendData(25, -1, -1, "Skeletron has awoken!", 255, 175f, 75f, 255f);
				ProgramLog.Users.Log("{0} @ {1}: Skeletron summoned by {2}.", player.IPAddress, player.whoAmi, player.Name);
				NetMessage.SendData(Packet.PLAYER_CHAT, -1, -1, String.Concat(player.Name, " has awoken Skeletron!"), 255, 255, 128, 150);
							
			}
		}

		/// <summary>
		/// Updates specified NPC based on any changes, including environment
		/// </summary>
		/// <param name="i">Main.npcs[] index of NPC to update</param>
		public static void UpdateNPC(int i)
		{
			var npc = Main.npcs[i];
			npc.whoAmI = i;
			if (npc.Active)
			{
				if (String.IsNullOrEmpty(npc.DisplayName))
					npc.DisplayName = npc.Name;

				if (npc.townNPC && Main.chrName[npc.Type] != String.Empty)
					npc.DisplayName = Main.chrName[npc.Type];

				npc.lifeRegen = 0;
				npc.poisoned = false;
				npc.onFire = false;
				npc.onFire2 = false;
				npc.confused = false;
				for (int j = 0; j < 5; j++)
				{
					if (npc.buffType[j] > 0 && npc.buffTime[j] > 0)
					{
						npc.buffTime[j]--;
						if (npc.buffType[j] == 20)
						{
							npc.poisoned = true;
						}
						else if (npc.buffType[j] == 24)
						{
							npc.onFire = true;
						}
						else if (npc.buffType[j] == 31)
						{
							npc.confused = true;
						}
						else if (npc.buffType[j] == 39)
						{
							npc.onFire2 = true;
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

				if (!npc.dontTakeDamage)
				{
					if (npc.poisoned)
					{
						npc.lifeRegen = -4;
					}
					if (npc.onFire)
					{
						npc.lifeRegen = -8;
					}
					if (npc.onFire2)
					{
						npc.lifeRegen = -12;
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
						int num = npc.whoAmI;
						if (npc.realLife >= 0)
						{
							num = npc.realLife;
						}
						Main.npcs[num].life--;
						if (Main.npcs[num].life <= 0)
						{
							Main.npcs[num].life = 1;
							Main.npcs[num].StrikeNPC(World.Sender, 9999, 0f, 0, false);
							NetMessage.SendData(28, -1, -1, "", num, 9999f);
						}
					}
				}
				if (Main.bloodMoon)
				{
					if (npc.Type == 46)
					{
						npc.Transform(47);
					}
					else if (npc.Type == 55)
					{
						npc.Transform(57);
					}
				}
				float num2 = 10f;
				float num3 = 0.3f;
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
				num3 *= num5;
				if (npc.wet)
				{
					num3 = 0.2f;
					num2 = 7f;
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
				AI(i);

				for (int l = 0; l < 256; l++)
				{
					if (npc.immune[l] > 0)
					{
						npc.immune[l]--;
					}
				}
				if (!npc.noGravity && !npc.noTileCollide)
				{
					int num6 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
					int num7 = (int)(npc.Position.Y + (float)(npc.Height / 2)) / 16;
					if (!Main.tile.At(num6, num7).Exists)
					{
						num3 = 0f;
						npc.Velocity.X = 0f;
						npc.Velocity.Y = 0f;
					}
				}
				if (!npc.noGravity)
				{
					npc.Velocity.Y = npc.Velocity.Y + num3;
					if (npc.Velocity.Y > num2)
					{
						npc.Velocity.Y = num2;
					}
				}
				if ((double)npc.Velocity.X < 0.005 && (double)npc.Velocity.X > -0.005)
				{
					npc.Velocity.X = 0f;
				}
				if (npc.Type != 37 && (npc.friendly || npc.Type == 46 || npc.Type == 55 || npc.Type == 74))
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
						for (int m = 0; m < 200; m++)
						{
							if (Main.npcs[m].Active && !Main.npcs[m].friendly && Main.npcs[m].damage > 0)
							{
								Rectangle rectangle2 = new Rectangle((int)Main.npcs[m].Position.X, (int)Main.npcs[m].Position.Y, Main.npcs[m].Width, Main.npcs[m].Height);
								if (rectangle.Intersects(rectangle2))
								{
									int num8 = Main.npcs[m].damage;
									int num9 = 6;
									int num10 = 1;
									if (Main.npcs[m].Position.X + (float)(Main.npcs[m].Width / 2) > npc.Position.X + (float)(npc.Width / 2))
									{
										num10 = -1;
									}
									Main.npcs[i].StrikeNPC(Main.npcs[m], num8, (float)num9, num10, false);

									NetMessage.SendData(28, -1, -1, "", i, (float)num8, (float)num9, (float)num10, 0);
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
						if (!npc.lavaImmune && !npc.dontTakeDamage && npc.immune[255] == 0)
						{
							npc.AddBuff(24, 420, false);
							npc.immune[255] = 30;
							npc.StrikeNPC(npc, 50, 0f, 0, false);

							NetMessage.SendData(28, -1, -1, "", npc.whoAmI, 50f, 0f, 0f, 0);
						}
					}
					bool flag2 = false;
					if (npc.Type == 72)
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
						npc.wet = true;
					}
					else if (npc.wet)
					{
						npc.Velocity.X = npc.Velocity.X * 0.5f;
						npc.wet = false;
						if (npc.wetCount == 0)
						{
							npc.wetCount = 10;
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
						if (npc.Type == 72)
						{
							Vector2 vector2 = new Vector2(npc.Position.X + (float)(npc.Width / 2), npc.Position.Y + (float)(npc.Height / 2));
							int num19 = 12;
							int num20 = 12;
							vector2.X -= (float)(num19 / 2);
							vector2.Y -= (float)(num20 / 2);
							npc.Velocity = Collision.TileCollision(vector2, npc.Velocity, num19, num20, true, true);
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
				if (!npc.noTileCollide && npc.lifeMax > 1 && Collision.SwitchTiles(null, null, npc.Position, npc.Width, npc.Height, npc.oldPosition, npc) && npc.Type == 46)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 400f;
					npc.ai[2] = 0f;
				}
				if (!npc.Active)
				{
					npc.netUpdate = true;
				}

				if (npc.townNPC)
				{
					npc.netSpam = 0;
				}
				if (npc.netUpdate2)
				{
					npc.netUpdate = true;
				}
				if (!npc.Active)
				{
					npc.netSpam = 0;
				}
				if (npc.netUpdate)
				{
					if (npc.netSpam <= 180)
					{
						npc.netSpam += 60;
						NetMessage.SendData(23, -1, -1, String.Empty, i);
						npc.netUpdate2 = false;
					}
					else
					{
						npc.netUpdate2 = true;
					}
				}
				if (npc.netSpam > 0)
				{
					npc.netSpam--;
				}
				if (npc.Active && npc.townNPC && TypeToNum(npc.Type) != -1)
				{
					if (npc.homeless != npc.oldHomeless || npc.homeTileX != npc.oldHomeTileX || npc.homeTileY != npc.oldHomeTileY)
					{
						int num21 = 0;
						if (npc.homeless)
						{
							num21 = 1;
						}
						NetMessage.SendData(60, -1, -1, "", i, (float)Main.npcs[i].homeTileX, (float)Main.npcs[i].homeTileY, (float)num21, 0);
					}
					npc.oldHomeless = npc.homeless;
					npc.oldHomeTileX = npc.homeTileX;
					npc.oldHomeTileY = npc.homeTileY;
				}

				npc.FindFrame();
				npc.CheckActive();
				npc.netUpdate = false;
				npc.justHit = false;
				if (npc.Type == 120 || npc.Type == 137 || npc.Type == 138)
				{
					for (int num22 = npc.oldPos.Length - 1; num22 > 0; num22--)
					{
						npc.oldPos[num22] = npc.oldPos[num22 - 1];
					}
					npc.oldPos[0] = npc.Position;
					return;
				}
				if (npc.Type == 94)
				{
					for (int num23 = npc.oldPos.Length - 1; num23 > 0; num23--)
					{
						npc.oldPos[num23] = npc.oldPos[num23 - 1];
					}
					npc.oldPos[0] = npc.Position;
					return;
				}
				if (npc.Type == 125 || npc.Type == 126 || npc.Type == 127 || npc.Type == 128 || npc.Type == 129 || npc.Type == 130 || npc.Type == 131 || npc.Type == 139 || npc.Type == 140)
				{
					for (int num24 = npc.oldPos.Length - 1; num24 > 0; num24--)
					{
						npc.oldPos[num24] = npc.oldPos[num24 - 1];
					}
					npc.oldPos[0] = npc.Position;
				}
			}
		}

		/// <summary>
		/// Clones this NPC
		/// </summary>
		/// <returns>Cloned NPC object</returns>
		public override object Clone()
		{
			NPC cloned = (NPC)base.MemberwiseClone();
			//npcSlots = cloned.slots;
			//cloned.frame = default(Rectangle);
			//cloned.Width = (int)((float)cloned.Width * cloned.scale);
			//cloned.Height = (int)((float)cloned.Height * cloned.scale);
			if (cloned.scaleOverrideAdjustment && (cloned.Height == 16 || cloned.Height == 32))
				cloned.Height += 1; //FIXME: this is really ugly
			cloned.life = cloned.lifeMax;
			cloned.defDamage = cloned.damage;
			cloned.defDefense = cloned.defense;

			cloned.ai = new float[MAX_AI];
			Array.Copy(ai, cloned.ai, MAX_AI);

			cloned.immune = new ushort[MAX_BUFF_IMMUNE];
			Array.Copy(immune, cloned.immune, MAX_BUFF_IMMUNE);

			cloned.buffType = new int[MAX_BUFF_TYPE];
			Array.Copy(buffType, cloned.buffType, MAX_BUFF_TYPE);

			cloned.buffTime = new int[MAX_BUFF_TIME];
			Array.Copy(buffTime, cloned.buffTime, MAX_BUFF_TIME);

			cloned.buffImmune = new bool[Main.MAX_BUFFS];
			Array.Copy(buffImmune, cloned.buffImmune, Main.MAX_BUFFS);

			//if (cloned.NetID == cloned.Type)
			//    cloned.NetID = cloned.Type;

			return cloned;
		}

#region AI
		//AI Stuff
		private delegate void AIFunction(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs);

		private static Dictionary<Int32, AIFunction> AIFunctions = new Dictionary<Int32, AIFunction>();

		private static bool aiLoaded = false;
		private int realLife = -1;

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
				AIFunctions.Add(38, AISnow);
			}
		}

		// 0 - 1.1.2
		private void AIUnknown(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			for (int i = 0; i < Main.MAX_PLAYERS; i++)
			{
				if (Main.players[i].Active && Main.players[i].talkNPC == npc.whoAmI)
				{
					if (npc.type == NPCType.N105_BOUND_GOBLIN)
					{
						npc.Transform((int)NPCType.N107_GOBLIN_TINKERER);
						return;
					}
					if (npc.type == NPCType.N106_BOUND_WIZARD)
					{
						npc.Transform((int)NPCType.N108_WIZARD);
						return;
					}
					if (npc.type == NPCType.N123_BOUND_MECHANIC)
					{
						npc.Transform((int)NPCType.N124_MECHANIC);
						return;
					}
				}
			}

			npc.Velocity.X = npc.Velocity.X * 0.93f;

			if ((double)npc.Velocity.X > -0.1 && (double)npc.Velocity.X < 0.1)
				npc.Velocity.X = 0f;

			npc.TargetClosest(true);
			npc.spriteDirection = npc.direction;
			return;
		}

		// 1 - 1.1.2
		private void AISlime(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (!Main.dayTime || npc.life != npc.lifeMax || (double)npc.Position.Y > Main.worldSurface * 16.0 || npc.type == NPCType.N81_CORRUPT_SLIME)
			{
				flag = true;
			}

			if (npc.ai[2] > 1f)
			{
				npc.ai[2] -= 1f;
			}
			if (npc.wet)
			{
				if (npc.collideY)
				{
					npc.Velocity.Y = -2f;
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
				if (npc.type == NPCType.N59_LAVA_SLIME)
				{
					if (npc.Velocity.Y > 2f)
					{
						npc.Velocity.Y = npc.Velocity.Y * 0.9f;
					}
					else if (npc.directionY < 0)
					{
						npc.Velocity.Y = npc.Velocity.Y - 0.8f;
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
				if (npc.type == NPCType.N59_LAVA_SLIME)
				{
					npc.ai[0] += 2f;
				}
				if (npc.type == NPCType.N71_DUNGEON_SLIME)
				{
					npc.ai[0] += 3f;
				}
				if (npc.type == NPCType.N138_ILLUMINANT_SLIME)
				{
					npc.ai[0] += 2f;
				}
				if (npc.type == NPCType.N81_CORRUPT_SLIME)
				{
					if (npc.scale >= 0f)
					{
						npc.ai[0] += 4f;
					}
					else
					{
						npc.ai[0] += 1f;
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
					}
					else
					{
						npc.Velocity.Y = -6f;
						npc.Velocity.X = npc.Velocity.X + (float)(2 * npc.direction);
						if (npc.type == NPCType.N59_LAVA_SLIME)
						{
							npc.Velocity.X = npc.Velocity.X + (float)(2 * npc.direction);
						}
						npc.ai[0] = -120f;
						npc.ai[1] += 1f;
					}
					if (npc.type == NPCType.N141_TOXIC_SLUDGE)
					{
						npc.Velocity.Y = npc.Velocity.Y * 1.3f;
						npc.Velocity.X = npc.Velocity.X * 1.2f;
						return;
					}
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

		// 2 - 1.1.2
		private void AIDemonEye(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
			if (Main.dayTime && (double)npc.Position.Y <= Main.worldSurface * 16.0 && (npc.type == NPCType.N02_DEMON_EYE || npc.type == NPCType.N133_WANDERING_EYE))
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
			if (npc.type == NPCType.N116_THE_HUNGRY_II)
			{
				npc.TargetClosest(true);

				if (npc.direction == -1 && npc.Velocity.X > -6f)
				{
					npc.Velocity.X = npc.Velocity.X - 0.1f;
					if (npc.Velocity.X > 6f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.1f;
					}
					else if (npc.Velocity.X > 0f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.2f;
					}
					if (npc.Velocity.X < -6f)
					{
						npc.Velocity.X = -6f;
					}
				}
				else if (npc.direction == 1 && npc.Velocity.X < 6f)
				{
					npc.Velocity.X = npc.Velocity.X + 0.1f;
					if (npc.Velocity.X < -6f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.1f;
					}
					else if (npc.Velocity.X < 0f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.2f;
					}
					if (npc.Velocity.X > 6f)
					{
						npc.Velocity.X = 6f;
					}
				}

				if (npc.directionY == -1 && (double)npc.Velocity.Y > -2.5)
				{
					npc.Velocity.Y = npc.Velocity.Y - 0.04f;
					if ((double)npc.Velocity.Y > 2.5)
					{
						npc.Velocity.Y = npc.Velocity.Y - 0.05f;
					}
					else if (npc.Velocity.Y > 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y - 0.15f;
					}
					if ((double)npc.Velocity.Y < -2.5)
					{
						npc.Velocity.Y = -2.5f;
					}
				}
				else if (npc.directionY == 1 && (double)npc.Velocity.Y < 1.5)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.04f;
					if ((double)npc.Velocity.Y < -2.5)
					{
						npc.Velocity.Y = npc.Velocity.Y + 0.05f;
					}
					else if (npc.Velocity.Y < 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y + 0.15f;
					}
					if ((double)npc.Velocity.Y > 2.5)
					{
						npc.Velocity.Y = 2.5f;
					}
				}
			}
			else if (npc.type == NPCType.N133_WANDERING_EYE)
			{
				if ((double)npc.life < (double)npc.lifeMax * 0.5)
				{
					if (npc.direction == -1 && npc.Velocity.X > -6f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.1f;
						if (npc.Velocity.X > 6f)
						{
							npc.Velocity.X = npc.Velocity.X - 0.1f;
						}
						else if (npc.Velocity.X > 0f)
						{
							npc.Velocity.X = npc.Velocity.X + 0.05f;
						}
						if (npc.Velocity.X < -6f)
						{
							npc.Velocity.X = -6f;
						}
					}
					else if (npc.direction == 1 && npc.Velocity.X < 6f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.1f;
						if (npc.Velocity.X < -6f)
						{
							npc.Velocity.X = npc.Velocity.X + 0.1f;
						}
						else if (npc.Velocity.X < 0f)
						{
							npc.Velocity.X = npc.Velocity.X - 0.05f;
						}
						if (npc.Velocity.X > 6f)
						{
							npc.Velocity.X = 6f;
						}
					}

					if (npc.directionY == -1 && npc.Velocity.Y > -4f)
					{
						npc.Velocity.Y = npc.Velocity.Y - 0.1f;
						if (npc.Velocity.Y > 4f)
						{
							npc.Velocity.Y = npc.Velocity.Y - 0.1f;
						}
						else if (npc.Velocity.Y > 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y + 0.05f;
						}
						if (npc.Velocity.Y < -4f)
						{
							npc.Velocity.Y = -4f;
						}
					}
					else if (npc.directionY == 1 && npc.Velocity.Y < 4f)
					{
						npc.Velocity.Y = npc.Velocity.Y + 0.1f;
						if (npc.Velocity.Y < -4f)
						{
							npc.Velocity.Y = npc.Velocity.Y + 0.1f;
						}
						else if (npc.Velocity.Y < 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y - 0.05f;
						}
						if (npc.Velocity.Y > 4f)
						{
							npc.Velocity.Y = 4f;
						}
					}
				}
				else if (npc.direction == -1 && npc.Velocity.X > -4f)
				{
					npc.Velocity.X = npc.Velocity.X - 0.1f;
					if (npc.Velocity.X > 4f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.1f;
					}
					else if (npc.Velocity.X > 0f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.05f;
					}
					if (npc.Velocity.X < -4f)
					{
						npc.Velocity.X = -4f;
					}
				}
				else if (npc.direction == 1 && npc.Velocity.X < 4f)
				{
					npc.Velocity.X = npc.Velocity.X + 0.1f;
					if (npc.Velocity.X < -4f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.1f;
					}
					else if (npc.Velocity.X < 0f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.05f;
					}
					if (npc.Velocity.X > 4f)
					{
						npc.Velocity.X = 4f;
					}
				}
				if (npc.directionY == -1 && (double)npc.Velocity.Y > -1.5)
				{
					npc.Velocity.Y = npc.Velocity.Y - 0.04f;
					if ((double)npc.Velocity.Y > 1.5)
					{
						npc.Velocity.Y = npc.Velocity.Y - 0.05f;
					}
					else if (npc.Velocity.Y > 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y + 0.03f;
					}
					if ((double)npc.Velocity.Y < -1.5)
					{
						npc.Velocity.Y = -1.5f;
					}
				}
				else if (npc.directionY == 1 && (double)npc.Velocity.Y < 1.5)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.04f;
					if ((double)npc.Velocity.Y < -1.5)
					{
						npc.Velocity.Y = npc.Velocity.Y + 0.05f;
					}
					else if (npc.Velocity.Y < 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y - 0.03f;
					}
					if ((double)npc.Velocity.Y > 1.5)
					{
						npc.Velocity.Y = 1.5f;
					}
				}
			}
			else if (npc.direction == -1 && npc.Velocity.X > -4f)
			{
				npc.Velocity.X = npc.Velocity.X - 0.1f;
				if (npc.Velocity.X > 4f)
				{
					npc.Velocity.X = npc.Velocity.X - 0.1f;
				}
				else if (npc.Velocity.X > 0f)
				{
					npc.Velocity.X = npc.Velocity.X + 0.05f;
				}
				if (npc.Velocity.X < -4f)
				{
					npc.Velocity.X = -4f;
				}
			}
			else if (npc.direction == 1 && npc.Velocity.X < 4f)
			{
				npc.Velocity.X = npc.Velocity.X + 0.1f;
				if (npc.Velocity.X < -4f)
				{
					npc.Velocity.X = npc.Velocity.X + 0.1f;
				}
				else if (npc.Velocity.X < 0f)
				{
					npc.Velocity.X = npc.Velocity.X - 0.05f;
				}
				if (npc.Velocity.X > 4f)
				{
					npc.Velocity.X = 4f;
				}
			}

			if (npc.directionY == -1 && (double)npc.Velocity.Y > -1.5)
			{
				npc.Velocity.Y = npc.Velocity.Y - 0.04f;
				if ((double)npc.Velocity.Y > 1.5)
				{
					npc.Velocity.Y = npc.Velocity.Y - 0.05f;
				}
				else if (npc.Velocity.Y > 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.03f;
				}
				if ((double)npc.Velocity.Y < -1.5)
				{
					npc.Velocity.Y = -1.5f;
				}
			}
			else if (npc.directionY == 1 && (double)npc.Velocity.Y < 1.5)
			{
				npc.Velocity.Y = npc.Velocity.Y + 0.04f;
				if ((double)npc.Velocity.Y < -1.5)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.05f;
				}
				else if (npc.Velocity.Y < 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y - 0.03f;
				}
				if ((double)npc.Velocity.Y > 1.5)
				{
					npc.Velocity.Y = 1.5f;
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

		// 3 - 1.1.2
		private void AIFighter(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			int num5 = 60;
			if (npc.type == NPCType.N120_CHAOS_ELEMENTAL)
			{
				num5 = 20;
				if (npc.ai[3] == -120f)
				{
					npc.Velocity *= 0f;
					npc.ai[3] = 0f;

					Vector2 vector = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num6 = npc.oldPos[2].X + (float)npc.Width * 0.5f - vector.X;
					float num7 = npc.oldPos[2].Y + (float)npc.Height * 0.5f - vector.Y;
					float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
					num8 = 2f / num8;
					num6 *= num8;
					num7 *= num8;
				}
			}
			bool flag2 = false;
			bool flag3 = true;
			if (npc.type == NPCType.N47_CORRUPT_BUNNY || npc.type == NPCType.N67_CRAB || npc.type == NPCType.N109_CLOWN || npc.type == NPCType.N110_SKELETON_ARCHER ||
				npc.type == NPCType.N111_GOBLIN_ARCHER || npc.type == NPCType.N120_CHAOS_ELEMENTAL)
			{
				flag3 = false;
			}
			if ((npc.type != NPCType.N110_SKELETON_ARCHER && npc.type != NPCType.N111_GOBLIN_ARCHER) || npc.ai[2] <= 0f)
			{
				if (npc.Velocity.Y == 0f && ((npc.Velocity.X > 0f && npc.direction < 0) || (npc.Velocity.X < 0f && npc.direction > 0)))
				{
					flag2 = true;
				}
				if (npc.Position.X == npc.oldPosition.X || npc.ai[3] >= (float)num5 || flag2)
				{
					npc.ai[3] += 1f;
				}
				else if ((double)Math.Abs(npc.Velocity.X) > 0.9 && npc.ai[3] > 0f)
				{
					npc.ai[3] -= 1f;
				}
				if (npc.ai[3] > (float)(num5 * 10))
				{
					npc.ai[3] = 0f;
				}
				if (npc.justHit)
				{
					npc.ai[3] = 0f;
				}
				if (npc.ai[3] == (float)num5)
				{
					npc.netUpdate = true;
				}
			}
			if ((!Main.dayTime || (double)npc.Position.Y > Main.worldSurface * 16.0 || npc.type == NPCType.N26_GOBLIN_PEON || npc.type == NPCType.N27_GOBLIN_THIEF ||
				npc.type == NPCType.N28_GOBLIN_WARRIOR || npc.type == NPCType.N31_ANGRY_BONES || npc.type == NPCType.N47_CORRUPT_BUNNY || npc.type == NPCType.N67_CRAB ||
				npc.type == NPCType.N73_GOBLIN_SCOUT || npc.type == NPCType.N77_ARMORED_SKELETON || npc.type == NPCType.N78_MUMMY || npc.type == NPCType.N79_DARK_MUMMY ||
				npc.type == NPCType.N80_LIGHT_MUMMY || npc.type == NPCType.N110_SKELETON_ARCHER || npc.type == NPCType.N111_GOBLIN_ARCHER ||
				npc.type == NPCType.N120_CHAOS_ELEMENTAL) && npc.ai[3] < (float)num5)
			{
				npc.TargetClosest(true);
			}
			else if ((npc.type != NPCType.N110_SKELETON_ARCHER && npc.type != NPCType.N111_GOBLIN_ARCHER) || npc.ai[2] <= 0f)
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

			if (npc.type == NPCType.N120_CHAOS_ELEMENTAL)
			{
				if (npc.Velocity.X < -3f || npc.Velocity.X > 3f)
				{
					if (npc.Velocity.Y == 0f)
					{
						npc.Velocity *= 0.8f;
					}
				}
				else if (npc.Velocity.X < 3f && npc.direction == 1)
				{
					if (npc.Velocity.Y == 0f && npc.Velocity.X < 0f)
					{
						npc.Velocity.X = npc.Velocity.X * 0.99f;
					}
					npc.Velocity.X = npc.Velocity.X + 0.07f;
					if (npc.Velocity.X > 3f)
					{
						npc.Velocity.X = 3f;
					}
				}
				else if (npc.Velocity.X > -3f && npc.direction == -1)
				{
					if (npc.Velocity.Y == 0f && npc.Velocity.X > 0f)
					{
						npc.Velocity.X = npc.Velocity.X * 0.99f;
					}
					npc.Velocity.X = npc.Velocity.X - 0.07f;
					if (npc.Velocity.X < -3f)
					{
						npc.Velocity.X = -3f;
					}
				}
			}
			else if (npc.type == NPCType.N27_GOBLIN_THIEF || npc.type == NPCType.N77_ARMORED_SKELETON || npc.type == NPCType.N104_WEREWOLF)
			{
				if (npc.Velocity.X < -2f || npc.Velocity.X > 2f)
				{
					if (npc.Velocity.Y == 0f)
					{
						npc.Velocity *= 0.8f;
					}
				}
				else if (npc.Velocity.X < 2f && npc.direction == 1)
				{
					npc.Velocity.X = npc.Velocity.X + 0.07f;
					if (npc.Velocity.X > 2f)
					{
						npc.Velocity.X = 2f;
					}
				}
				else if (npc.Velocity.X > -2f && npc.direction == -1)
				{
					npc.Velocity.X = npc.Velocity.X - 0.07f;
					if (npc.Velocity.X < -2f)
					{
						npc.Velocity.X = -2f;
					}
				}
			}
			else if (npc.type == NPCType.N109_CLOWN)
			{
				if (npc.Velocity.X < -2f || npc.Velocity.X > 2f)
				{
					if (npc.Velocity.Y == 0f)
					{
						npc.Velocity *= 0.8f;
					}
				}
				else if (npc.Velocity.X < 2f && npc.direction == 1)
				{
					npc.Velocity.X = npc.Velocity.X + 0.04f;
					if (npc.Velocity.X > 2f)
					{
						npc.Velocity.X = 2f;
					}
				}
				else if (npc.Velocity.X > -2f && npc.direction == -1)
				{
					npc.Velocity.X = npc.Velocity.X - 0.04f;
					if (npc.Velocity.X < -2f)
					{
						npc.Velocity.X = -2f;
					}
				}
			}
			else if (npc.type == NPCType.N21_SKELETON || npc.type == NPCType.N26_GOBLIN_PEON || npc.type == NPCType.N31_ANGRY_BONES || npc.type == NPCType.N47_CORRUPT_BUNNY ||
					npc.type == NPCType.N73_GOBLIN_SCOUT || npc.type == NPCType.N140_POSSESSED_ARMOR)
			{
				if (npc.Velocity.X < -1.5f || npc.Velocity.X > 1.5f)
				{
					if (npc.Velocity.Y == 0f)
					{
						npc.Velocity *= 0.8f;
					}
				}
				else if (npc.Velocity.X < 1.5f && npc.direction == 1)
				{
					npc.Velocity.X = npc.Velocity.X + 0.07f;
					if (npc.Velocity.X > 1.5f)
					{
						npc.Velocity.X = 1.5f;
					}
				}
				else if (npc.Velocity.X > -1.5f && npc.direction == -1)
				{
					npc.Velocity.X = npc.Velocity.X - 0.07f;
					if (npc.Velocity.X < -1.5f)
					{
						npc.Velocity.X = -1.5f;
					}
				}
			}
			else if (npc.type == NPCType.N67_CRAB)
			{
				if (npc.Velocity.X < -0.5f || npc.Velocity.X > 0.5f)
				{
					if (npc.Velocity.Y == 0f)
					{
						npc.Velocity *= 0.7f;
					}
				}
				else if (npc.Velocity.X < 0.5f && npc.direction == 1)
				{
					npc.Velocity.X = npc.Velocity.X + 0.03f;
					if (npc.Velocity.X > 0.5f)
					{
						npc.Velocity.X = 0.5f;
					}
				}
				else if (npc.Velocity.X > -0.5f && npc.direction == -1)
				{
					npc.Velocity.X = npc.Velocity.X - 0.03f;
					if (npc.Velocity.X < -0.5f)
					{
						npc.Velocity.X = -0.5f;
					}
				}
			}
			else if (npc.type == NPCType.N78_MUMMY || npc.type == NPCType.N79_DARK_MUMMY || npc.type == NPCType.N80_LIGHT_MUMMY)
			{
				float num11 = 1f;
				float num12 = 0.05f;
				if (npc.life < npc.lifeMax / 2)
				{
					num11 = 2f;
					num12 = 0.1f;
				}
				if (npc.type == NPCType.N79_DARK_MUMMY)
				{
					num11 *= 1.5f;
				}
				if (npc.Velocity.X < -num11 || npc.Velocity.X > num11)
				{
					if (npc.Velocity.Y == 0f)
					{
						npc.Velocity *= 0.7f;
					}
				}
				else if (npc.Velocity.X < num11 && npc.direction == 1)
				{
					npc.Velocity.X = npc.Velocity.X + num12;
					if (npc.Velocity.X > num11)
					{
						npc.Velocity.X = num11;
					}
				}
				else if (npc.Velocity.X > -num11 && npc.direction == -1)
				{
					npc.Velocity.X = npc.Velocity.X - num12;
					if (npc.Velocity.X < -num11)
					{
						npc.Velocity.X = -num11;
					}
				}
			}
			else if (npc.type != NPCType.N110_SKELETON_ARCHER && npc.type != NPCType.N111_GOBLIN_ARCHER)
			{
				if (npc.Velocity.X < -1f || npc.Velocity.X > 1f)
				{
					if (npc.Velocity.Y == 0f)
					{
						npc.Velocity *= 0.8f;
					}
				}
				else if (npc.Velocity.X < 1f && npc.direction == 1)
				{
					npc.Velocity.X = npc.Velocity.X + 0.07f;
					if (npc.Velocity.X > 1f)
					{
						npc.Velocity.X = 1f;
					}
				}
				else if (npc.Velocity.X > -1f && npc.direction == -1)
				{
					npc.Velocity.X = npc.Velocity.X - 0.07f;
					if (npc.Velocity.X < -1f)
					{
						npc.Velocity.X = -1f;
					}
				}
			}

			if (npc.type == NPCType.N110_SKELETON_ARCHER || npc.type == NPCType.N111_GOBLIN_ARCHER)
			{
				if (npc.confused)
				{
					npc.ai[2] = 0f;
				}
				else if (npc.ai[1] > 0f)
				{
					npc.ai[1] -= 1f;
				}
				if (npc.justHit)
				{
					npc.ai[1] = 30f;
					npc.ai[2] = 0f;
				}
				int num13 = 70;
				if (npc.type == NPCType.N111_GOBLIN_ARCHER)
				{
					num13 = 180;
				}
				if (npc.ai[2] > 0f)
				{
					npc.TargetClosest(true);
					if (npc.ai[1] == (float)(num13 / 2))
					{
						float num14 = 11f;
						if (npc.type == NPCType.N111_GOBLIN_ARCHER)
						{
							num14 = 9f;
						}
						Vector2 vector2 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
						float num15 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width * 0.5f - vector2.X;
						float num16 = Math.Abs(num15) * 0.1f;
						float num17 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].Height * 0.5f - vector2.Y - num16;
						num15 += (float)Main.rand.Next(-40, 41);
						num17 += (float)Main.rand.Next(-40, 41);
						float num18 = (float)Math.Sqrt((double)(num15 * num15 + num17 * num17));
						npc.netUpdate = true;
						num18 = num14 / num18;
						num15 *= num18;
						num17 *= num18;
						int num19 = 35;
						if (npc.type == NPCType.N111_GOBLIN_ARCHER)
						{
							num19 = 11;
						}
						int num20 = 82;
						if (npc.type == NPCType.N111_GOBLIN_ARCHER)
						{
							num20 = 81;
						}
						vector2.X += num15;
						vector2.Y += num17;

						Projectile.NewProjectile(vector2.X, vector2.Y, num15, num17, num20, num19, 0f, Main.myPlayer);

						if (Math.Abs(num17) > Math.Abs(num15) * 2f)
						{
							if (num17 > 0f)
							{
								npc.ai[2] = 1f;
							}
							else
							{
								npc.ai[2] = 5f;
							}
						}
						else if (Math.Abs(num15) > Math.Abs(num17) * 2f)
						{
							npc.ai[2] = 3f;
						}
						else if (num17 > 0f)
						{
							npc.ai[2] = 2f;
						}
						else
						{
							npc.ai[2] = 4f;
						}

						if (npc.Velocity.Y != 0f || npc.ai[1] <= 0f)
						{
							npc.ai[2] = 0f;
							npc.ai[1] = 0f;
						}
						else
						{
							npc.Velocity.X = npc.Velocity.X * 0.9f;
							npc.spriteDirection = npc.direction;
						}
					}
					if (npc.ai[2] <= 0f && npc.Velocity.Y == 0f && npc.ai[1] <= 0f && !Main.players[npc.target].dead && Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
					{
						float num21 = 10f;
						Vector2 vector3 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
						float num22 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width * 0.5f - vector3.X;
						float num23 = Math.Abs(num22) * 0.1f;
						float num24 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].Height * 0.5f - vector3.Y - num23;
						num22 += (float)Main.rand.Next(-40, 41);
						num24 += (float)Main.rand.Next(-40, 41);
						float num25 = (float)Math.Sqrt((double)(num22 * num22 + num24 * num24));
						if (num25 < 700f)
						{
							npc.netUpdate = true;
							npc.Velocity.X = npc.Velocity.X * 0.5f;
							num25 = num21 / num25;
							num22 *= num25;
							num24 *= num25;
							npc.ai[2] = 3f;
							npc.ai[1] = (float)num13;
							if (Math.Abs(num24) > Math.Abs(num22) * 2f)
							{
								if (num24 > 0f)
								{
									npc.ai[2] = 1f;
								}
								else
								{
									npc.ai[2] = 5f;
								}
							}
							else if (Math.Abs(num22) > Math.Abs(num24) * 2f)
							{
								npc.ai[2] = 3f;
							}
							else if (num24 > 0f)
							{
								npc.ai[2] = 2f;
							}
							else
							{
								npc.ai[2] = 4f;
							}
						}
					}
					if (npc.ai[2] <= 0f)
					{
						if (npc.Velocity.X < -1f || npc.Velocity.X > 1f)
						{
							if (npc.Velocity.Y == 0f)
							{
								npc.Velocity *= 0.8f;
							}
						}
						else if (npc.Velocity.X < 1f && npc.direction == 1)
						{
							npc.Velocity.X = npc.Velocity.X + 0.07f;
							if (npc.Velocity.X > 1f)
							{
								npc.Velocity.X = 1f;
							}
						}
						else if (npc.Velocity.X > -1f && npc.direction == -1)
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
			if (npc.type == NPCType.N109_CLOWN && !Main.players[npc.target].dead)
			{
				if (npc.justHit)
				{
					npc.ai[2] = 0f;
				}
				npc.ai[2] += 1f;
				if (npc.ai[2] > 450f)
				{
					Vector2 vector4 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f - (float)(npc.direction * 24), npc.Position.Y + 4f);
					int num26 = 3 * npc.direction;
					int num27 = -5;
					int num28 = Projectile.NewProjectile(vector4.X, vector4.Y, (float)num26, (float)num27, 75, 0, 0f, Main.myPlayer);
					Main.projectile[num28].timeLeft = 300;
					npc.ai[2] = 0f;
				}
			}
			bool flag4 = false;
			if (npc.Velocity.Y == 0f)
			{
				int num29 = (int)(npc.Position.Y + (float)npc.Height + 8f) / 16;
				int num30 = (int)npc.Position.X / 16;
				int num31 = (int)(npc.Position.X + (float)npc.Width) / 16;
				for (int l = num30; l <= num31; l++)
				{
					if (TileRefs(l, num29).Active && Main.tileSolid[(int)TileRefs(l, num29).Type])
					{
						flag4 = true;
						break;
					}
				}
			}
			if (flag4)
			{
				int num32 = (int)((npc.Position.X + (float)(npc.Width / 2) + (float)(15 * npc.direction)) / 16f);
				int num33 = (int)((npc.Position.Y + (float)npc.Height - 15f) / 16f);
				if (npc.type == NPCType.N109_CLOWN)
				{
					num32 = (int)((npc.Position.X + (float)(npc.Width / 2) + (float)((npc.Width / 2 + 16) * npc.direction)) / 16f);
				}

				if (TileRefs(num32, num33 - 1).Active && TileRefs(num32, num33 - 1).Type == 10 && flag3)
				{
					npc.ai[2] += 1f;
					npc.ai[3] = 0f;
					if (npc.ai[2] >= 60f)
					{
						if (!Main.bloodMoon && (npc.type == NPCType.N03_ZOMBIE || npc.type == NPCType.N132_BALD_ZOMBIE))
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
						bool flag5 = false;
						if (npc.ai[1] >= 10f)
						{
							flag5 = true;
							npc.ai[1] = 10f;
						}

						WorldModify.KillTile(TileRefs, null, num32, num33 - 1, true);
						if (!flag5 && flag5)
						{
							if (npc.type == NPCType.N26_GOBLIN_PEON)
							{
								WorldModify.KillTile(TileRefs, null, num32, num33 - 1);
								NetMessage.SendData(17, -1, -1, String.Empty, 0, (float)num32, (float)(num33 - 1));
							}
							else
							{
								bool flag6 = WorldModify.OpenDoor(TileRefs, null, num32, num33, npc.direction, npc);
								if (!flag6)
								{
									npc.ai[3] = (float)num5;
									npc.netUpdate = true;
								}
								if (flag6)
								{
									NetMessage.SendData(19, -1, -1, String.Empty, 0, (float)num32, (float)num33, (float)npc.direction, 0);
								}
							}
						}
					}
				}
				else if ((npc.Velocity.X < 0f && npc.spriteDirection == -1) || (npc.Velocity.X > 0f && npc.spriteDirection == 1))
				{
					if (TileRefs(num32, num33 - 2).Active && Main.tileSolid[(int)TileRefs(num32, num33 - 2).Type])
					{
						if (TileRefs(num32, num33 - 3).Active && Main.tileSolid[(int)TileRefs(num32, num33 - 3).Type])
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
					else if (TileRefs(num32, num33 - 1).Active && Main.tileSolid[(int)TileRefs(num32, num33 - 1).Type])
					{
						npc.Velocity.Y = -6f;
						npc.netUpdate = true;
					}
					else if (TileRefs(num32, num33).Active && Main.tileSolid[(int)TileRefs(num32, num33).Type])
					{
						npc.Velocity.Y = -5f;
						npc.netUpdate = true;
					}
					else if (npc.directionY < 0 && npc.type != NPCType.N67_CRAB && (!TileRefs(num32, num33 + 1).Active ||
							!Main.tileSolid[(int)TileRefs(num32, num33 + 1).Type]) && (!TileRefs(num32 + npc.direction, num33 + 1).Active ||
							!Main.tileSolid[(int)TileRefs(num32 + npc.direction, num33 + 1).Type]))
					{
						npc.Velocity.Y = -8f;
						npc.Velocity.X = npc.Velocity.X * 1.5f;
						npc.netUpdate = true;
					}
					else if (flag3)
					{
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
					}
				}
				if ((npc.type == NPCType.N31_ANGRY_BONES || npc.type == NPCType.N47_CORRUPT_BUNNY || npc.type == NPCType.N77_ARMORED_SKELETON || npc.type == NPCType.N104_WEREWOLF)
					&& npc.Velocity.Y == 0f && Math.Abs(npc.Position.X + (float)(npc.Width / 2) - (Main.players[npc.target].Position.X +
					(float)(Main.players[npc.target].Width / 2))) < 100f && Math.Abs(npc.Position.Y + (float)(npc.Height / 2) -
					(Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2))) < 50f && ((npc.direction > 0 && npc.Velocity.X >= 1f) ||
					(npc.direction < 0 && npc.Velocity.X <= -1f)))
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
				}
				if (npc.type == NPCType.N120_CHAOS_ELEMENTAL && npc.Velocity.Y < 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y * 1.1f;
				}
			}
			else if (flag3)
			{
				npc.ai[1] = 0f;
				npc.ai[2] = 0f;
			}

			if (npc.type == NPCType.N120_CHAOS_ELEMENTAL && npc.ai[3] >= (float)num5)
			{
				int num34 = (int)Main.players[npc.target].Position.X / 16;
				int num35 = (int)Main.players[npc.target].Position.Y / 16;
				int num36 = (int)npc.Position.X / 16;
				int num37 = (int)npc.Position.Y / 16;
				int num38 = 20;
				int num39 = 0;
				bool flag7 = false;
				if (Math.Abs(npc.Position.X - Main.players[npc.target].Position.X) + Math.Abs(npc.Position.Y - Main.players[npc.target].Position.Y) > 2000f)
				{
					num39 = 100;
					flag7 = true;
				}
				while (!flag7)
				{
					if (num39 >= 100)
					{
						return;
					}
					num39++;
					int num40 = Main.rand.Next(num34 - num38, num34 + num38);
					int num41 = Main.rand.Next(num35 - num38, num35 + num38);
					for (int m = num41; m < num35 + num38; m++)
					{
						if ((m < num35 - 4 || m > num35 + 4 || num40 < num34 - 4 || num40 > num34 + 4) && (m < num37 - 1 || m > num37 + 1 || num40 < num36 - 1 || num40 > num36 + 1) && Main.tile.At(num40, m).Active)
						{
							bool flag8 = true;
							if (npc.type == NPCType.N03_ZOMBIE && Main.tile.At(num40, m - 1).Wall == 0)
							{
								flag8 = false;
							}
							else if (TileRefs(num40, m - 1).Lava)
							{
								flag8 = false;
							}
							if (flag8 && Main.tileSolid[(int)TileRefs(num40, m).Type] && !Collision.SolidTiles(num40 - 1, num40 + 1, m - 4, m - 1))
							{
								npc.Position.X = (float)(num40 * 16 - npc.Width / 2);
								npc.Position.Y = (float)(m * 16 - npc.Height);
								npc.netUpdate = true;
								npc.ai[3] = -120f;
							}
						}
					}
				}
			}
		}

		// 4 - 1.1.2
		private void AIEoC(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (npc.target < 0 || npc.target == 255 || Main.players[npc.target].dead || !Main.players[npc.target].Active)
			{
				npc.TargetClosest(true);
			}
			bool dead = Main.players[npc.target].dead;
			float num42 = npc.Position.X + (float)(npc.Width / 2) - Main.players[npc.target].Position.X - (float)(Main.players[npc.target].Width / 2);
			float num43 = npc.Position.Y + (float)npc.Height - 59f - Main.players[npc.target].Position.Y - (float)(Main.players[npc.target].Height / 2);
			float num44 = (float)Math.Atan2((double)num43, (double)num42) + 1.57f;
			if (num44 < 0f)
			{
				num44 += 6.283f;
			}
			else if ((double)num44 > 6.283)
			{
				num44 -= 6.283f;
			}

			float num45 = 0f;
			if (npc.ai[0] == 0f && npc.ai[1] == 0f)
			{
				num45 = 0.02f;
			}
			if (npc.ai[0] == 0f && npc.ai[1] == 2f && npc.ai[2] > 40f)
			{
				num45 = 0.05f;
			}
			if (npc.ai[0] == 3f && npc.ai[1] == 0f)
			{
				num45 = 0.05f;
			}
			if (npc.ai[0] == 3f && npc.ai[1] == 2f && npc.ai[2] > 40f)
			{
				num45 = 0.08f;
			}
			if (npc.rotation < num44)
			{
				if ((double)(num44 - npc.rotation) > 3.1415)
				{
					npc.rotation -= num45;
				}
				else
				{
					npc.rotation += num45;
				}
			}
			else if (npc.rotation > num44)
			{
				if ((double)(npc.rotation - num44) > 3.1415)
				{
					npc.rotation += num45;
				}
				else
				{
					npc.rotation -= num45;
				}
			}

			if (npc.rotation > num44 - num45 && npc.rotation < num44 + num45)
			{
				npc.rotation = num44;
			}
			if (npc.rotation < 0f)
			{
				npc.rotation += 6.283f;
			}
			else if ((double)npc.rotation > 6.283)
			{
				npc.rotation -= 6.283f;
			}

			if (npc.rotation > num44 - num45 && npc.rotation < num44 + num45)
			{
				npc.rotation = num44;
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
			else if (npc.ai[0] == 0f)
			{
				if (npc.ai[1] == 0f)
				{
					float num47 = 5f;
					float num48 = 0.04f;
					Vector2 vector5 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num49 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector5.X;
					float num50 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - 200f - vector5.Y;
					float num51 = (float)Math.Sqrt((double)(num49 * num49 + num50 * num50));
					float num52 = num51;
					num51 = num47 / num51;
					num49 *= num51;
					num50 *= num51;
					if (npc.Velocity.X < num49)
					{
						npc.Velocity.X = npc.Velocity.X + num48;
						if (npc.Velocity.X < 0f && num49 > 0f)
						{
							npc.Velocity.X = npc.Velocity.X + num48;
						}
					}
					else if (npc.Velocity.X > num49)
					{
						npc.Velocity.X = npc.Velocity.X - num48;
						if (npc.Velocity.X > 0f && num49 < 0f)
						{
							npc.Velocity.X = npc.Velocity.X - num48;
						}
					}

					if (npc.Velocity.Y < num50)
					{
						npc.Velocity.Y = npc.Velocity.Y + num48;
						if (npc.Velocity.Y < 0f && num50 > 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y + num48;
						}
					}
					else if (npc.Velocity.Y > num50)
					{
						npc.Velocity.Y = npc.Velocity.Y - num48;
						if (npc.Velocity.Y > 0f && num50 < 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y - num48;
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
					else if (npc.Position.Y + (float)npc.Height < Main.players[npc.target].Position.Y && num52 < 500f)
					{
						if (!Main.players[npc.target].dead)
						{
							npc.ai[3] += 1f;
						}
						if (npc.ai[3] >= 110f)
						{
							npc.ai[3] = 0f;
							npc.rotation = num44;
							float num53 = 5f;
							float num54 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector5.X;
							float num55 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector5.Y;
							float num56 = (float)Math.Sqrt((double)(num54 * num54 + num55 * num55));
							num56 = num53 / num56;
							Vector2 vector6 = vector5;
							Vector2 vector7;
							vector7.X = num54 * num56;
							vector7.Y = num55 * num56;
							vector6.X += vector7.X * 10f;
							vector6.Y += vector7.Y * 10f;

							int num57 = NewNPC((int)vector6.X, (int)vector6.Y, 5, 0);
							Main.npcs[num57].Velocity.X = vector7.X;
							Main.npcs[num57].Velocity.Y = vector7.Y;
							if (num57 < 200)
							{
								NetMessage.SendData(23, -1, -1, "", num57, 0f, 0f, 0f, 0);
							}
						}
					}
				}
				else if (npc.ai[1] == 1f)
				{
					npc.rotation = num44;
					float num58 = 6f;
					Vector2 vector8 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num59 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector8.X;
					float num60 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector8.Y;
					float num61 = (float)Math.Sqrt((double)(num59 * num59 + num60 * num60));
					num61 = num58 / num61;
					npc.Velocity.X = num59 * num61;
					npc.Velocity.Y = num60 * num61;
					npc.ai[1] = 2f;
				}
				else if (npc.ai[1] == 2f)
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
						npc.rotation = num44;
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
			else if (npc.ai[0] == 1f || npc.ai[0] == 2f)
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
					float num64 = 6f;
					float num65 = 0.07f;
					Vector2 vector9 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num66 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector9.X;
					float num67 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - 120f - vector9.Y;
					float num68 = (float)Math.Sqrt((double)(num66 * num66 + num67 * num67));
					num68 = num64 / num68;
					num66 *= num68;
					num67 *= num68;
					if (npc.Velocity.X < num66)
					{
						npc.Velocity.X = npc.Velocity.X + num65;
						if (npc.Velocity.X < 0f && num66 > 0f)
						{
							npc.Velocity.X = npc.Velocity.X + num65;
						}
					}
					else if (npc.Velocity.X > num66)
					{
						npc.Velocity.X = npc.Velocity.X - num65;
						if (npc.Velocity.X > 0f && num66 < 0f)
						{
							npc.Velocity.X = npc.Velocity.X - num65;
						}
					}
					if (npc.Velocity.Y < num67)
					{
						npc.Velocity.Y = npc.Velocity.Y + num65;
						if (npc.Velocity.Y < 0f && num67 > 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y + num65;
						}
					}
					else if (npc.Velocity.Y > num67)
					{
						npc.Velocity.Y = npc.Velocity.Y - num65;
						if (npc.Velocity.Y > 0f && num67 < 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y - num65;
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
						npc.rotation = num44;
						float num69 = 6.8f;
						Vector2 vector10 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
						float num70 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector10.X;
						float num71 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector10.Y;
						float num72 = (float)Math.Sqrt((double)(num70 * num70 + num71 * num71));
						num72 = num69 / num72;
						npc.Velocity.X = num70 * num72;
						npc.Velocity.Y = num71 * num72;
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
							npc.rotation = num44;
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

		// 5 - 1.1.2
		private void AIFlyDirect(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (npc.target < 0 || npc.target == 255 || Main.players[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			float num73 = 6f;
			float num74 = 0.05f;
			if (npc.type == NPCType.N06_EATER_OF_SOULS)
			{
				num73 = 4f;
				num74 = 0.02f;
			}
			else if (npc.type == NPCType.N94_CORRUPTOR)
			{
				num73 = 4.2f;
				num74 = 0.022f;
			}
			else if (npc.type == NPCType.N42_HORNET)
			{
				num73 = 3.5f;
				num74 = 0.021f;
			}
			else if (npc.type == NPCType.N23_METEOR_HEAD)
			{
				num73 = 1f;
				num74 = 0.03f;
			}
			else if (npc.type == NPCType.N05_SERVANT_OF_CTHULHU)
			{
				num73 = 5f;
				num74 = 0.03f;
			}
			Vector2 vector11 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
			float num75 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2);
			float num76 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2);
			num75 = (float)((int)(num75 / 8f) * 8);
			num76 = (float)((int)(num76 / 8f) * 8);
			vector11.X = (float)((int)(vector11.X / 8f) * 8);
			vector11.Y = (float)((int)(vector11.Y / 8f) * 8);
			num75 -= vector11.X;
			num76 -= vector11.Y;
			float num77 = (float)Math.Sqrt((double)(num75 * num75 + num76 * num76));
			float num78 = num77;
			bool flag9 = false;
			if (num77 > 600f)
			{
				flag9 = true;
			}
			if (num77 == 0f)
			{
				num75 = npc.Velocity.X;
				num76 = npc.Velocity.Y;
			}
			else
			{
				num77 = num73 / num77;
				num75 *= num77;
				num76 *= num77;
			}
			if (npc.type == NPCType.N06_EATER_OF_SOULS || npc.type == NPCType.N42_HORNET || npc.type == NPCType.N94_CORRUPTOR || npc.type == NPCType.N139_PROBE)
			{
				if (num78 > 100f || npc.type == NPCType.N42_HORNET || npc.type == NPCType.N94_CORRUPTOR)
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
				if (num78 < 150f && (npc.type == NPCType.N06_EATER_OF_SOULS || npc.type == NPCType.N94_CORRUPTOR))
				{
					npc.Velocity.X = npc.Velocity.X + num75 * 0.007f;
					npc.Velocity.Y = npc.Velocity.Y + num76 * 0.007f;
				}
			}
			if (Main.players[npc.target].dead)
			{
				num75 = (float)npc.direction * num73 / 2f;
				num76 = -num73 / 2f;
			}
			if (npc.Velocity.X < num75)
			{
				npc.Velocity.X = npc.Velocity.X + num74;
				if (npc.type != NPCType.N06_EATER_OF_SOULS && npc.type != NPCType.N42_HORNET && npc.type != NPCType.N94_CORRUPTOR &&
					npc.type != NPCType.N139_PROBE && npc.Velocity.X < 0f && num75 > 0f)
				{
					npc.Velocity.X = npc.Velocity.X + num74;
				}
			}
			else if (npc.Velocity.X > num75)
			{
				npc.Velocity.X = npc.Velocity.X - num74;
				if (npc.type != NPCType.N06_EATER_OF_SOULS && npc.type != NPCType.N42_HORNET && npc.type != NPCType.N94_CORRUPTOR &&
					npc.type != NPCType.N139_PROBE && npc.Velocity.X > 0f && num75 < 0f)
				{
					npc.Velocity.X = npc.Velocity.X - num74;
				}
			}

			if (npc.Velocity.Y < num76)
			{
				npc.Velocity.Y = npc.Velocity.Y + num74;
				if (npc.type != NPCType.N06_EATER_OF_SOULS && npc.type != NPCType.N42_HORNET && npc.type != NPCType.N94_CORRUPTOR && npc.type != NPCType.N139_PROBE && npc.Velocity.Y < 0f && num76 > 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y + num74;
				}
			}
			else if (npc.Velocity.Y > num76)
			{
				npc.Velocity.Y = npc.Velocity.Y - num74;
				if (npc.type != NPCType.N06_EATER_OF_SOULS && npc.type != NPCType.N42_HORNET && npc.type != NPCType.N94_CORRUPTOR && npc.type != NPCType.N139_PROBE && npc.Velocity.Y > 0f && num76 < 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y - num74;
				}
			}

			if (npc.type == NPCType.N23_METEOR_HEAD)
			{
				if (num75 > 0f)
				{
					npc.spriteDirection = 1;
					npc.rotation = (float)Math.Atan2((double)num76, (double)num75);
				}
				else if (num75 < 0f)
				{
					npc.spriteDirection = -1;
					npc.rotation = (float)Math.Atan2((double)num76, (double)num75) + 3.14f;
				}
			}
			else if (npc.type == NPCType.N139_PROBE)
			{
				npc.localAI[0] += 1f;
				if (npc.justHit)
				{
					npc.localAI[0] = 0f;
				}
				if (npc.localAI[0] >= 120f)
				{
					npc.localAI[0] = 0f;
					if (Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
					{
						int num79 = 25;
						int num80 = 84;
						Projectile.NewProjectile(vector11.X, vector11.Y, num75, num76, num80, num79, 0f, Main.myPlayer);
					}
				}
				int num81 = (int)npc.Position.X + npc.Width / 2;
				int num82 = (int)npc.Position.Y + npc.Height / 2;
				num81 /= 16;
				num82 /= 16;

				if (num75 > 0f)
				{
					npc.spriteDirection = 1;
					npc.rotation = (float)Math.Atan2((double)num76, (double)num75);
				}
				if (num75 < 0f)
				{
					npc.spriteDirection = -1;
					npc.rotation = (float)Math.Atan2((double)num76, (double)num75) + 3.14f;
				}
			}
			else if (npc.type == NPCType.N06_EATER_OF_SOULS || npc.type == NPCType.N94_CORRUPTOR)
			{
				npc.rotation = (float)Math.Atan2((double)num76, (double)num75) - 1.57f;
			}
			else if (npc.type == NPCType.N42_HORNET)
			{
				if (num75 > 0f)
				{
					npc.spriteDirection = 1;
				}
				if (num75 < 0f)
				{
					npc.spriteDirection = -1;
				}
				npc.rotation = npc.Velocity.X * 0.1f;
			}
			else
			{
				npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) - 1.57f;
			}

			if (npc.type == NPCType.N06_EATER_OF_SOULS || npc.type == NPCType.N23_METEOR_HEAD || npc.type == NPCType.N42_HORNET ||
				npc.type == NPCType.N94_CORRUPTOR || npc.type == NPCType.N139_PROBE)
			{
				float num83 = 0.7f;
				if (npc.type == NPCType.N06_EATER_OF_SOULS)
				{
					num83 = 0.4f;
				}
				if (npc.collideX)
				{
					npc.netUpdate = true;
					npc.Velocity.X = npc.oldVelocity.X * -num83;
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
					npc.Velocity.Y = npc.oldVelocity.Y * -num83;
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
			if ((npc.type == NPCType.N06_EATER_OF_SOULS || npc.type == NPCType.N94_CORRUPTOR) && npc.wet)
			{
				if (npc.Velocity.Y > 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y * 0.95f;
				}
				npc.Velocity.Y = npc.Velocity.Y - 0.3f;
				if (npc.Velocity.Y < -2f)
				{
					npc.Velocity.Y = -2f;
				}
			}
			if (npc.type == NPCType.N42_HORNET)
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
				{
					npc.ai[1] = 0f;
				}

				npc.ai[1] += (float)Main.rand.Next(5, 20) * 0.1f * npc.scale;
				if (npc.ai[1] >= 130f)
				{
					if (Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
					{
						float num87 = 8f;
						Vector2 vector12 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)(npc.Height / 2));
						float num88 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width * 0.5f - vector12.X + (float)Main.rand.Next(-20, 21);
						float num89 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].Height * 0.5f - vector12.Y + (float)Main.rand.Next(-20, 21);
						if ((num88 < 0f && npc.Velocity.X < 0f) || (num88 > 0f && npc.Velocity.X > 0f))
						{
							float num90 = (float)Math.Sqrt((double)(num88 * num88 + num89 * num89));
							num90 = num87 / num90;
							num88 *= num90;
							num89 *= num90;
							int num91 = (int)(13f * npc.scale);
							int num92 = 55;
							int num93 = Projectile.NewProjectile(vector12.X, vector12.Y, num88, num89, num92, num91, 0f, Main.myPlayer);
							Main.projectile[num93].timeLeft = 300;
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
			if (npc.type == NPCType.N139_PROBE && flag9)
			{
				if ((npc.Velocity.X > 0f && num75 > 0f) || (npc.Velocity.X < 0f && num75 < 0f))
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
			if (npc.type == NPCType.N94_CORRUPTOR && !Main.players[npc.target].dead)
			{
				if (npc.justHit)
				{
					npc.localAI[0] = 0f;
				}
				npc.localAI[0] += 1f;
				if (npc.localAI[0] == 180f)
				{
					if (Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
					{
						NewNPC((int)(npc.Position.X + (float)(npc.Width / 2) + npc.Velocity.X), (int)(npc.Position.Y + (float)(npc.Height / 2) + npc.Velocity.Y), 112, 0);
					}
					npc.localAI[0] = 0f;
				}
			}
			if ((Main.dayTime && npc.type != NPCType.N06_EATER_OF_SOULS && npc.type != NPCType.N23_METEOR_HEAD && npc.type != NPCType.N42_HORNET && npc.type != NPCType.N94_CORRUPTOR) || Main.players[npc.target].dead)
			{
				npc.Velocity.Y = npc.Velocity.Y - num74 * 2f;
				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
				}
			}
			if (((npc.Velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.Velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.Velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.Velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
			{
				npc.netUpdate = true;
				return;
			}
		}

		// 6 - 1.1.2
		private void AIWorm(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (npc.type == NPCType.N117_LEECH_HEAD && npc.localAI[1] == 0f)
			{
				npc.localAI[1] = 1f;
			}
			if (npc.type >= NPCType.N13_EATER_OF_WORLDS_HEAD && npc.type <= NPCType.N15_EATER_OF_WORLDS_TAIL)
			{
				npc.realLife = -1;
			}
			else if (npc.ai[3] > 0f)
			{
				npc.realLife = (int)npc.ai[3];
			}

			if (npc.target < 0 || npc.target == 255 || Main.players[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			if (Main.players[npc.target].dead && npc.timeLeft > 300)
			{
				npc.timeLeft = 300;
			}

			if (npc.type == NPCType.N87_WYVERN_HEAD && npc.ai[0] == 0f)
			{
				npc.ai[3] = (float)npc.whoAmI;
				npc.realLife = npc.whoAmI;
				int num96 = npc.whoAmI;
				for (int num97 = 0; num97 < 14; num97++)
				{
					int num98 = 89;
					if (num97 == 1 || num97 == 8)
					{
						num98 = 88;
					}
					else if (num97 == 11)
					{
						num98 = 90;
					}
					else if (num97 == 12)
					{
						num98 = 91;
					}
					else if (num97 == 13)
					{
						num98 = 92;
					}
					int num99 = NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)),
						(int)(npc.Position.Y + (float)npc.Height), num98, npc.whoAmI, true);
					Main.npcs[num99].ai[3] = (float)npc.whoAmI;
					Main.npcs[num99].realLife = npc.whoAmI;
					Main.npcs[num99].ai[1] = (float)num96;
					Main.npcs[num96].ai[0] = (float)num99;
					NetMessage.SendData(23, -1, -1, "", num99, 0f, 0f, 0f, 0);
					num96 = num99;
				}
			}
			if ((npc.type == NPCType.N07_DEVOURER_HEAD || npc.type == NPCType.N08_DEVOURER_BODY || npc.type == NPCType.N10_GIANT_WORM_HEAD ||
				npc.type == NPCType.N11_GIANT_WORM_BODY || npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD || npc.type == NPCType.N14_EATER_OF_WORLDS_BODY ||
				npc.type == NPCType.N39_BONE_SERPENT_HEAD || npc.type == NPCType.N40_BONE_SERPENT_BODY || npc.type == NPCType.N95_DIGGER_HEAD ||
				npc.type == NPCType.N96_DIGGER_BODY || npc.type == NPCType.N98_SEEKER_HEAD || npc.type == NPCType.N99_SEEKER_BODY || npc.type == NPCType.N117_LEECH_HEAD ||
				npc.type == NPCType.N118_LEECH_BODY) && npc.ai[0] == 0f)
			{
				if (npc.type == NPCType.N07_DEVOURER_HEAD || npc.type == NPCType.N10_GIANT_WORM_HEAD || npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD ||
					npc.type == NPCType.N39_BONE_SERPENT_HEAD || npc.type == NPCType.N95_DIGGER_HEAD || npc.type == NPCType.N98_SEEKER_HEAD ||
					npc.type == NPCType.N117_LEECH_HEAD)
				{
					if (npc.type < NPCType.N13_EATER_OF_WORLDS_HEAD || npc.type > NPCType.N15_EATER_OF_WORLDS_TAIL)
					{
						npc.ai[3] = (float)npc.whoAmI;
						npc.realLife = npc.whoAmI;
					}
					npc.ai[2] = (float)Main.rand.Next(8, 13);
					if (npc.type == NPCType.N10_GIANT_WORM_HEAD)
					{
						npc.ai[2] = (float)Main.rand.Next(4, 7);
					}
					if (npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD)
					{
						npc.ai[2] = (float)Main.rand.Next(45, 56);
					}
					if (npc.type == NPCType.N39_BONE_SERPENT_HEAD)
					{
						npc.ai[2] = (float)Main.rand.Next(12, 19);
					}
					if (npc.type == NPCType.N95_DIGGER_HEAD)
					{
						npc.ai[2] = (float)Main.rand.Next(6, 12);
					}
					if (npc.type == NPCType.N98_SEEKER_HEAD)
					{
						npc.ai[2] = (float)Main.rand.Next(20, 26);
					}
					if (npc.type == NPCType.N117_LEECH_HEAD)
					{
						npc.ai[2] = (float)Main.rand.Next(3, 6);
					}
					npc.ai[0] = (float)NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)(npc.Position.Y + (float)npc.Height),
						(int)npc.type + 1, npc.whoAmI, true);
				}
				else
				{
					if ((npc.type == NPCType.N08_DEVOURER_BODY || npc.type == NPCType.N11_GIANT_WORM_BODY || npc.type == NPCType.N14_EATER_OF_WORLDS_BODY ||
						npc.type == NPCType.N40_BONE_SERPENT_BODY || npc.type == NPCType.N96_DIGGER_BODY || npc.type == NPCType.N99_SEEKER_BODY ||
						npc.type == NPCType.N118_LEECH_BODY) && npc.ai[2] > 0f)
					{
						npc.ai[0] = (float)NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)(npc.Position.Y + (float)npc.Height),
							(int)npc.type, npc.whoAmI, true);
					}
					else
					{
						npc.ai[0] = (float)NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)(npc.Position.Y + (float)npc.Height),
							(int)npc.type + 1, npc.whoAmI, true);
					}
				}
				if (npc.type < NPCType.N13_EATER_OF_WORLDS_HEAD || npc.type > NPCType.N15_EATER_OF_WORLDS_TAIL)
				{
					Main.npcs[(int)npc.ai[0]].ai[3] = npc.ai[3];
					Main.npcs[(int)npc.ai[0]].realLife = npc.realLife;
				}
				Main.npcs[(int)npc.ai[0]].ai[1] = (float)npc.whoAmI;
				Main.npcs[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;
				npc.netUpdate = true;
			}
			if ((npc.type == NPCType.N08_DEVOURER_BODY || npc.type == NPCType.N09_DEVOURER_TAIL || npc.type == NPCType.N11_GIANT_WORM_BODY ||
				npc.type == NPCType.N12_GIANT_WORM_TAIL || npc.type == NPCType.N40_BONE_SERPENT_BODY || npc.type == NPCType.N41_BONE_SERPENT_TAIL ||
				npc.type == NPCType.N96_DIGGER_BODY || npc.type == NPCType.N97_DIGGER_TAIL || npc.type == NPCType.N99_SEEKER_BODY || npc.type == NPCType.N100_SEEKER_TAIL ||
				(npc.type > NPCType.N87_WYVERN_HEAD && npc.type <= NPCType.N90_WYVERN_BODY_2) ||
				npc.type == NPCType.N118_LEECH_BODY || npc.type == NPCType.N119_LEECH_TAIL) &&
				(!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != npc.aiStyle))
			{
				npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.Active = false;
			}
			if ((npc.type == NPCType.N07_DEVOURER_HEAD || npc.type == NPCType.N08_DEVOURER_BODY || npc.type == NPCType.N10_GIANT_WORM_HEAD ||
				npc.type == NPCType.N11_GIANT_WORM_BODY || npc.type == NPCType.N39_BONE_SERPENT_HEAD || npc.type == NPCType.N40_BONE_SERPENT_BODY ||
				npc.type == NPCType.N95_DIGGER_HEAD || npc.type == NPCType.N96_DIGGER_BODY || npc.type == NPCType.N98_SEEKER_HEAD || npc.type == NPCType.N99_SEEKER_BODY ||
				(npc.type >= NPCType.N87_WYVERN_HEAD && npc.type < NPCType.N92_WYVERN_TAIL) || npc.type == NPCType.N117_LEECH_HEAD || npc.type == NPCType.N118_LEECH_BODY) &&
				(!Main.npcs[(int)npc.ai[0]].Active || Main.npcs[(int)npc.ai[0]].aiStyle != npc.aiStyle))
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
				if (npc.type == NPCType.N14_EATER_OF_WORLDS_BODY && (!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != npc.aiStyle))
				{
					npc.type = NPCType.N13_EATER_OF_WORLDS_HEAD;
					int num100 = npc.whoAmI;
					float num101 = (float)npc.life / (float)npc.lifeMax;
					float num102 = npc.ai[0];
					npc.SetDefaults((int)npc.type);
					npc.life = (int)((float)npc.lifeMax * num101);
					npc.ai[0] = num102;
					npc.TargetClosest(true);
					npc.netUpdate = true;
					npc.whoAmI = num100;
				}
				if (npc.type == NPCType.N14_EATER_OF_WORLDS_BODY && (!Main.npcs[(int)npc.ai[0]].Active || Main.npcs[(int)npc.ai[0]].aiStyle != npc.aiStyle))
				{
					int num103 = npc.whoAmI;
					float num104 = (float)npc.life / (float)npc.lifeMax;
					float num105 = npc.ai[1];
					npc.SetDefaults((int)npc.type);
					npc.life = (int)((float)npc.lifeMax * num104);
					npc.ai[1] = num105;
					npc.TargetClosest(true);
					npc.netUpdate = true;
					npc.whoAmI = num103;
				}
				if (npc.life == 0)
				{
					bool EoWAlive = false;
					for (int npcId = 0; npcId < MAX_NPCS; npcId++)
					{
						var fNpc = Main.npcs[npcId];

						if (fNpc.Active)
						{
							EoWAlive = (
								fNpc.type == NPCType.N13_EATER_OF_WORLDS_HEAD ||
								fNpc.type == NPCType.N14_EATER_OF_WORLDS_BODY ||
								fNpc.type == NPCType.N15_EATER_OF_WORLDS_TAIL);

							if (EoWAlive)
								break;
						}
					}

					if (!EoWAlive)
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

			int num107 = (int)(npc.Position.X / 16f) - 1;
			int num108 = (int)((npc.Position.X + (float)npc.Width) / 16f) + 2;
			int num109 = (int)(npc.Position.Y / 16f) - 1;
			int num110 = (int)((npc.Position.Y + (float)npc.Height) / 16f) + 2;
			if (num107 < 0)
			{
				num107 = 0;
			}
			if (num108 > Main.maxTilesX)
			{
				num108 = Main.maxTilesX;
			}
			if (num109 < 0)
			{
				num109 = 0;
			}
			if (num110 > Main.maxTilesY)
			{
				num110 = Main.maxTilesY;
			}
			bool flag11 = false;
			if (npc.type >= NPCType.N87_WYVERN_HEAD && npc.type <= NPCType.N92_WYVERN_TAIL)
			{
				flag11 = true;
			}
			if (!flag11)
			{
				for (int num111 = num107; num111 < num108; num111++)
				{
					for (int num112 = num109; num112 < num110; num112++)
					{
						if (((Main.tile.At(num111, num112).Active && (Main.tileSolid[(int)Main.tile.At(num111, num112).Type] ||
							(Main.tileSolidTop[(int)Main.tile.At(num111, num112).Type] && Main.tile.At(num111, num112).FrameY == 0))) ||
								Main.tile.At(num111, num112).Liquid > 64))
						{
							Vector2 vector13;
							vector13.X = (float)(num111 * 16);
							vector13.Y = (float)(num112 * 16);
							if (npc.Position.X + (float)npc.Width > vector13.X && npc.Position.X < vector13.X + 16f && npc.Position.Y + (float)npc.Height > vector13.Y && npc.Position.Y < vector13.Y + 16f)
							{
								flag11 = true;
								if (Main.rand.Next(100) == 0 && npc.type != NPCType.N117_LEECH_HEAD && Main.tile.At(num111, num112).Active)
								{
									WorldModify.KillTile(TileRefs, null, num111, num112, true, true);
								}
							}
						}
					}
				}
			}
			if (!flag11 && (npc.type == NPCType.N07_DEVOURER_HEAD || npc.type == NPCType.N10_GIANT_WORM_HEAD || npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD ||
				npc.type == NPCType.N39_BONE_SERPENT_HEAD || npc.type == NPCType.N95_DIGGER_HEAD || npc.type == NPCType.N98_SEEKER_HEAD || npc.type == NPCType.N117_LEECH_HEAD))
			{
				Rectangle rectangle = new Rectangle((int)npc.Position.X, (int)npc.Position.Y, npc.Width, npc.Height);
				int num113 = 1000;
				bool flag12 = true;
				for (int num114 = 0; num114 < 255; num114++)
				{
					if (Main.players[num114].Active)
					{
						Rectangle rectangle2 = new Rectangle((int)Main.players[num114].Position.X - num113, (int)Main.players[num114].Position.Y - num113, num113 * 2, num113 * 2);
						if (rectangle.Intersects(rectangle2))
						{
							flag12 = false;
							break;
						}
					}
				}
				if (flag12)
				{
					flag11 = true;
				}
			}
			if (npc.type >= NPCType.N87_WYVERN_HEAD && npc.type <= NPCType.N92_WYVERN_TAIL)
			{
				if (npc.Velocity.X < 0f)
				{
					npc.spriteDirection = 1;
				}
				else if (npc.Velocity.X > 0f)
				{
					npc.spriteDirection = -1;
				}
			}
			float num115 = 8f;
			float num116 = 0.07f;
			if (npc.type == NPCType.N95_DIGGER_HEAD)
			{
				num115 = 5.5f;
				num116 = 0.045f;
			}
			if (npc.type == NPCType.N10_GIANT_WORM_HEAD)
			{
				num115 = 6f;
				num116 = 0.05f;
			}
			if (npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD)
			{
				num115 = 10f;
				num116 = 0.07f;
			}
			if (npc.type == NPCType.N87_WYVERN_HEAD)
			{
				num115 = 11f;
				num116 = 0.25f;
			}
			if (npc.type == NPCType.N117_LEECH_HEAD && Main.WallOfFlesh >= 0)
			{
				float num117 = (float)Main.npcs[Main.WallOfFlesh].life / (float)Main.npcs[Main.WallOfFlesh].lifeMax;
				if ((double)num117 < 0.5)
				{
					num115 += 1f;
					num116 += 0.1f;
				}
				if ((double)num117 < 0.25)
				{
					num115 += 1f;
					num116 += 0.1f;
				}
				if ((double)num117 < 0.1)
				{
					num115 += 2f;
					num116 += 0.1f;
				}
			}
			Vector2 vector14 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
			float num118 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2);
			float num119 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2);
			num118 = (float)((int)(num118 / 16f) * 16);
			num119 = (float)((int)(num119 / 16f) * 16);
			vector14.X = (float)((int)(vector14.X / 16f) * 16);
			vector14.Y = (float)((int)(vector14.Y / 16f) * 16);
			num118 -= vector14.X;
			num119 -= vector14.Y;
			float num120 = (float)Math.Sqrt((double)(num118 * num118 + num119 * num119));
			if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npcs.Length)
			{
				try
				{
					vector14 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					num118 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - vector14.X;
					num119 = Main.npcs[(int)npc.ai[1]].Position.Y + (float)(Main.npcs[(int)npc.ai[1]].Height / 2) - vector14.Y;
				}
				catch
				{ }

				npc.rotation = (float)Math.Atan2((double)num119, (double)num118) + 1.57f;
				num120 = (float)Math.Sqrt((double)(num118 * num118 + num119 * num119));
				int num121 = npc.Width;
				if (npc.type >= NPCType.N87_WYVERN_HEAD && npc.type <= NPCType.N92_WYVERN_TAIL)
				{
					num121 = 42;
				}
				num120 = (num120 - (float)num121) / num120;
				num118 *= num120;
				num119 *= num120;
				npc.Velocity = default(Vector2);
				npc.Position.X = npc.Position.X + num118;
				npc.Position.Y = npc.Position.Y + num119;
				if (npc.type >= NPCType.N87_WYVERN_HEAD && npc.type <= NPCType.N92_WYVERN_TAIL)
				{
					if (num118 < 0f)
					{
						npc.spriteDirection = 1;
						return;
					}
					if (num118 > 0f)
					{
						npc.spriteDirection = -1;
						return;
					}
				}
			}
			else if (!flag11)
			{
				npc.TargetClosest(true);
				npc.Velocity.Y = npc.Velocity.Y + 0.11f;
				if (npc.Velocity.Y > num115)
				{
					npc.Velocity.Y = num115;
				}
				if ((double)(Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y)) < (double)num115 * 0.4)
				{
					if (npc.Velocity.X < 0f)
					{
						npc.Velocity.X = npc.Velocity.X - num116 * 1.1f;
					}
					else
					{
						npc.Velocity.X = npc.Velocity.X + num116 * 1.1f;
					}
				}
				else if (npc.Velocity.Y == num115)
				{
					if (npc.Velocity.X < num118)
					{
						npc.Velocity.X = npc.Velocity.X + num116;
					}
					else if (npc.Velocity.X > num118)
					{
						npc.Velocity.X = npc.Velocity.X - num116;
					}
				}
				else if (npc.Velocity.Y > 4f)
				{
					if (npc.Velocity.X < 0f)
					{
						npc.Velocity.X = npc.Velocity.X + num116 * 0.9f;
					}
					else
					{
						npc.Velocity.X = npc.Velocity.X - num116 * 0.9f;
					}
				}
			}
			else
			{
				if (npc.type != NPCType.N87_WYVERN_HEAD && npc.type != NPCType.N117_LEECH_HEAD && npc.soundDelay == 0)
				{
					float num122 = num120 / 40f;
					if (num122 < 10f)
					{
						num122 = 10f;
					}
					if (num122 > 20f)
					{
						num122 = 20f;
					}
					npc.soundDelay = (int)num122;
				}
				num120 = (float)Math.Sqrt((double)(num118 * num118 + num119 * num119));
				float num123 = Math.Abs(num118);
				float num124 = Math.Abs(num119);
				float num125 = num115 / num120;
				num118 *= num125;
				num119 *= num125;
				if ((npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD || npc.type == NPCType.N07_DEVOURER_HEAD) && !Main.players[npc.target].zoneEvil)
				{
					bool flag13 = true;
					for (int num126 = 0; num126 < 255; num126++)
					{
						if (Main.players[num126].Active && !Main.players[num126].dead && Main.players[num126].zoneEvil)
						{
							flag13 = false;
						}
					}
					if (flag13 & !npc.MadeSpawn)
					{
						if ((double)(npc.Position.Y / 16f) > (Main.rockLayer + (double)Main.maxTilesY) / 2.0)
						{
							npc.Active = false;
							int num127 = (int)npc.ai[0];
							while (num127 > 0 && num127 < 200 && Main.npcs[num127].Active && Main.npcs[num127].aiStyle == npc.aiStyle)
							{
								int num128 = (int)Main.npcs[num127].ai[0];
								Main.npcs[num127].Active = false;
								npc.life = 0;

								NetMessage.SendData(23, -1, -1, "", num127);

								num127 = num128;
							}

							NetMessage.SendData(23, -1, -1, "", npc.whoAmI);
						}
						num118 = 0f;
						num119 = num115;
					}
				}
				bool flag14 = false;
				if (npc.type == NPCType.N87_WYVERN_HEAD)
				{
					if (((npc.Velocity.X > 0f && num118 < 0f) || (npc.Velocity.X < 0f && num118 > 0f) || (npc.Velocity.Y > 0f && num119 < 0f) || (npc.Velocity.Y < 0f && num119 > 0f)) && Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y) > num116 / 2f && num120 < 300f)
					{
						flag14 = true;
						if (Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y) < num115)
						{
							npc.Velocity *= 1.1f;
						}
					}
					if (npc.Position.Y > Main.players[npc.target].Position.Y || (double)(Main.players[npc.target].Position.Y / 16f) > Main.worldSurface || Main.players[npc.target].dead)
					{
						flag14 = true;
						if (Math.Abs(npc.Velocity.X) < num115 / 2f)
						{
							if (npc.Velocity.X == 0f)
							{
								npc.Velocity.X = npc.Velocity.X - (float)npc.direction;
							}
							npc.Velocity.X = npc.Velocity.X * 1.1f;
						}
						else if (npc.Velocity.Y > -num115)
						{
							npc.Velocity.Y = npc.Velocity.Y - num116;
						}
					}
				}
				if (!flag14)
				{
					if ((npc.Velocity.X > 0f && num118 > 0f) || (npc.Velocity.X < 0f && num118 < 0f) || (npc.Velocity.Y > 0f && num119 > 0f) || (npc.Velocity.Y < 0f && num119 < 0f))
					{
						if (npc.Velocity.X < num118)
						{
							npc.Velocity.X = npc.Velocity.X + num116;
						}
						else if (npc.Velocity.X > num118)
						{
							npc.Velocity.X = npc.Velocity.X - num116;
						}

						if (npc.Velocity.Y < num119)
						{
							npc.Velocity.Y = npc.Velocity.Y + num116;
						}
						else if (npc.Velocity.Y > num119)
						{
							npc.Velocity.Y = npc.Velocity.Y - num116;
						}

						if ((double)Math.Abs(num119) < (double)num115 * 0.2 && ((npc.Velocity.X > 0f && num118 < 0f) || (npc.Velocity.X < 0f && num118 > 0f)))
						{
							if (npc.Velocity.Y > 0f)
							{
								npc.Velocity.Y = npc.Velocity.Y + num116 * 2f;
							}
							else
							{
								npc.Velocity.Y = npc.Velocity.Y - num116 * 2f;
							}
						}
						if ((double)Math.Abs(num118) < (double)num115 * 0.2 && ((npc.Velocity.Y > 0f && num119 < 0f) || (npc.Velocity.Y < 0f && num119 > 0f)))
						{
							if (npc.Velocity.X > 0f)
							{
								npc.Velocity.X = npc.Velocity.X + num116 * 2f;
							}
							else
							{
								npc.Velocity.X = npc.Velocity.X - num116 * 2f;
							}
						}
					}
					else if (num123 > num124)
					{
						if (npc.Velocity.X < num118)
						{
							npc.Velocity.X = npc.Velocity.X + num116 * 1.1f;
						}
						else if (npc.Velocity.X > num118)
						{
							npc.Velocity.X = npc.Velocity.X - num116 * 1.1f;
						}
						if ((double)(Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y)) < (double)num115 * 0.5)
						{
							if (npc.Velocity.Y > 0f)
							{
								npc.Velocity.Y = npc.Velocity.Y + num116;
							}
							else
							{
								npc.Velocity.Y = npc.Velocity.Y - num116;
							}
						}
					}
					else
					{
						if (npc.Velocity.Y < num119)
						{
							npc.Velocity.Y = npc.Velocity.Y + num116 * 1.1f;
						}
						else if (npc.Velocity.Y > num119)
						{
							npc.Velocity.Y = npc.Velocity.Y - num116 * 1.1f;
						}

						if ((double)(Math.Abs(npc.Velocity.X) + Math.Abs(npc.Velocity.Y)) < (double)num115 * 0.5)
						{
							if (npc.Velocity.X > 0f)
							{
								npc.Velocity.X = npc.Velocity.X + num116;
							}
							else
							{
								npc.Velocity.X = npc.Velocity.X - num116;
							}
						}
					}
				}
			}

			npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) + 1.57f;
			if (npc.type == NPCType.N07_DEVOURER_HEAD || npc.type == NPCType.N10_GIANT_WORM_HEAD ||
				npc.type == NPCType.N13_EATER_OF_WORLDS_HEAD || npc.type == NPCType.N39_BONE_SERPENT_HEAD || npc.type == NPCType.N95_DIGGER_HEAD ||
				npc.type == NPCType.N98_SEEKER_HEAD || npc.type == NPCType.N117_LEECH_HEAD)
			{
				if (flag11)
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
					return;
				}
			}
		}

		// 7 - 1.1.2
		private void AIFriendly(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (npc.type == NPCType.N142_SANTA_CLAUS && !Main.Xmas)
			{
				npc.StrikeNPC(World.Sender, 9999, 0f, 0);
				NetMessage.SendData(28, -1, -1, "", npc.whoAmI, 9999f, 0f, 0f, 0);
			}
			int num129 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
			int num130 = (int)(npc.Position.Y + (float)npc.Height + 1f) / 16;

			if (num129 < 0 || num129 > Main.maxTilesX)
				return;
			else if (num130 < 0 || num130 > Main.maxTilesY)
				return;

			if (npc.type == NPCType.N107_GOBLIN_TINKERER)
			{
				savedGoblin = true;
			}
			if (npc.type == NPCType.N108_WIZARD)
			{
				savedWizard = true;
			}
			if (npc.type == NPCType.N124_MECHANIC)
			{
				savedMech = true;
			}
			if (npc.type == NPCType.N46_BUNNY && npc.target == 255)
			{
				npc.TargetClosest(true);
			}
			bool flag15 = false;
			npc.directionY = -1;
			if (npc.direction == 0)
			{
				npc.direction = 1;
			}
			for (int num131 = 0; num131 < Main.MAX_PLAYERS; num131++)
			{
				if (Main.players[num131].Active && Main.players[num131].talkNPC == npc.whoAmI)
				{
					flag15 = true;
					if (npc.ai[0] != 0f)
					{
						npc.netUpdate = true;
					}
					npc.ai[0] = 0f;
					npc.ai[1] = 300f;
					npc.ai[2] = 100f;
					if (Main.players[num131].Position.X + (float)(Main.players[num131].Width / 2) < npc.Position.X + (float)(npc.Width / 2))
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
				if (downedBoss3)
				{
					npc.ai[3] = 1f;
					npc.netUpdate = true;
				}
			}
			int num132 = npc.homeTileY;
			if (npc.homeTileY > 0)
			{
				while (!WorldModify.SolidTile(TileRefs, npc.homeTileX, num132) && num132 < Main.maxTilesY - 20)
				{
					num132++;
				}
			}
			var type = (int)Main.tile.At(num129, num130).Type;
			if (npc.townNPC && (!Main.dayTime || Main.tileDungeon[type]) && (num129 != npc.homeTileX || num130 != num132) && !npc.homeless)
			{
				bool flag16 = true;
				for (int num133 = 0; num133 < 2; num133++)
				{
					Rectangle rectangle3 = new Rectangle((int)(npc.Position.X + (float)(npc.Width / 2) - (float)(sWidth / 2) - (float)safeRangeX), (int)(npc.Position.Y + (float)(npc.Height / 2) - (float)(sHeight / 2) - (float)safeRangeY), sWidth + safeRangeX * 2, sHeight + safeRangeY * 2);
					if (num133 == 1)
					{
						rectangle3 = new Rectangle(npc.homeTileX * 16 + 8 - sWidth / 2 - safeRangeX, num132 * 16 + 8 - sHeight / 2 - safeRangeY, sWidth + safeRangeX * 2, sHeight + safeRangeY * 2);
					}
					for (int num134 = 0; num134 < 255; num134++)
					{
						if (Main.players[num134].Active)
						{
							Rectangle rectangle4 = new Rectangle((int)Main.players[num134].Position.X, (int)Main.players[num134].Position.Y,
								Main.players[num134].Width, Main.players[num134].Height);
							if (rectangle4.Intersects(rectangle3))
							{
								flag16 = false;
								break;
							}
						}
						if (!flag16)
						{
							break;
						}
					}
				}
				if (flag16)
				{
					if (npc.type == NPCType.N37_OLD_MAN || !Collision.SolidTiles(npc.homeTileX - 1, npc.homeTileX + 1, num132 - 3, num132 - 1))
					{
						npc.Velocity.X = 0f;
						npc.Velocity.Y = 0f;
						npc.Position.X = (float)(npc.homeTileX * 16 + 8 - npc.Width / 2);
						npc.Position.Y = (float)(num132 * 16 - npc.Height) - 0.1f;
						npc.netUpdate = true;
					}
					else
					{
						npc.homeless = true;
						WorldModify.QuickFindHome(TileRefs, npc.whoAmI);
					}
				}
			}
			if (npc.ai[0] == 0f)
			{
				if (npc.ai[2] > 0f)
				{
					npc.ai[2] -= 1f;
				}
				if (!Main.dayTime && !flag15 && npc.type != NPCType.N46_BUNNY)
				{
					if (num129 == npc.homeTileX && num130 == num132)
					{
						if (npc.Velocity.X != 0f)
						{
							npc.netUpdate = true;
						}
						if ((double)npc.Velocity.X > 0.1)
						{
							npc.Velocity.X = npc.Velocity.X - 0.1f;
						}
						else if ((double)npc.Velocity.X < -0.1)
						{
							npc.Velocity.X = npc.Velocity.X + 0.1f;
						}
						else
						{
							npc.Velocity.X = 0f;
						}
					}
					else if (!flag15)
					{
						if (num129 > npc.homeTileX)
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
				else
				{
					if ((double)npc.Velocity.X > 0.1)
					{
						npc.Velocity.X = npc.Velocity.X - 0.1f;
					}
					else if ((double)npc.Velocity.X < -0.1)
					{
						npc.Velocity.X = npc.Velocity.X + 0.1f;
					}
					else
					{
						npc.Velocity.X = 0f;
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
				if ((Main.dayTime || (num129 == npc.homeTileX && num130 == num132)))
				{
					if (num129 < npc.homeTileX - 25 || num129 > npc.homeTileX + 25)
					{
						if (npc.ai[2] == 0f)
						{
							if (num129 < npc.homeTileX - 50 && npc.direction == -1)
							{
								npc.direction = 1;
								npc.netUpdate = true;
								return;
							}
							if (num129 > npc.homeTileX + 50 && npc.direction == 1)
							{
								npc.direction = -1;
								npc.netUpdate = true;
								return;
							}
						}
					}
					else if (Main.rand.Next(80) == 0 && npc.ai[2] == 0f)
					{
						npc.ai[2] = 200f;
						npc.direction *= -1;
						npc.netUpdate = true;
						return;
					}
				}
			}
			else
			{
				if (npc.ai[0] == 1f)
				{
					if (!Main.dayTime && num129 == npc.homeTileX && num130 == npc.homeTileY && npc.type != NPCType.N46_BUNNY)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = (float)(200 + Main.rand.Next(200));
						npc.ai[2] = 60f;
						npc.netUpdate = true;
						return;
					}
					if (!npc.homeless && !Main.tileDungeon[(int)Main.tile.At(num129, num130).Type] && (num129 < npc.homeTileX - 35 || num129 > npc.homeTileX + 35))
					{
						if (npc.Position.X < (float)(npc.homeTileX * 16) && npc.direction == -1)
						{
							npc.ai[1] -= 5f;
						}
						else if (npc.Position.X > (float)(npc.homeTileX * 16) && npc.direction == 1)
						{
							npc.ai[1] -= 5f;
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
						bool flag17 = WorldModify.CloseDoor(TileRefs, null, npc.doorX, npc.doorY, false, npc);
						if (flag17)
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
					else if ((double)npc.Velocity.X < 1.15 && npc.direction == 1)
					{
						npc.Velocity.X = npc.Velocity.X + 0.07f;
						if (npc.Velocity.X > 1f)
						{
							npc.Velocity.X = 1f;
						}
					}
					else if (npc.Velocity.X > -1f && npc.direction == -1)
					{
						npc.Velocity.X = npc.Velocity.X - 0.07f;
						if (npc.Velocity.X > 1f)
						{
							npc.Velocity.X = 1f;
						}
					}

					if (npc.Velocity.Y == 0f)
					{
						if (npc.Position.X == npc.ai[2])
						{
							npc.direction *= -1;
						}
						npc.ai[2] = -1f;
						int num135 = (int)((npc.Position.X + (float)(npc.Width / 2) + (float)(15 * npc.direction)) / 16f);
						int num136 = (int)((npc.Position.Y + (float)npc.Height - 16f) / 16f);

						if (npc.townNPC && Main.tile.At(num135, num136 - 2).Active && Main.tile.At(num135, num136 - 2).Type == 10 &&
							(Main.rand.Next(10) == 0 || !Main.dayTime))
						{
							bool flag18 = WorldModify.OpenDoor(TileRefs, null, num135, num136 - 2, npc.direction, npc);
							if (flag18)
							{
								npc.closeDoor = true;
								npc.doorX = num135;
								npc.doorY = num136 - 2;
								NetMessage.SendData(19, -1, -1, "", 0, (float)num135, (float)(num136 - 2), (float)npc.direction, 0);
								npc.netUpdate = true;
								npc.ai[1] += 80f;
								return;
							}
							if (WorldModify.OpenDoor(TileRefs, null, num135, num136 - 2, -npc.direction, npc))
							{
								npc.closeDoor = true;
								npc.doorX = num135;
								npc.doorY = num136 - 2;
								NetMessage.SendData(19, -1, -1, "", 0, (float)num135, (float)(num136 - 2), (float)(-(float)npc.direction), 0);
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
								if (Main.tile.At(num135, num136 - 2).Active && Main.tileSolid[(int)Main.tile.At(num135, num136 - 2).Type] &&
									!Main.tileSolidTop[(int)Main.tile.At(num135, num136 - 2).Type])
								{
									if ((npc.direction == 1 && !Collision.SolidTiles(num135 - 2, num135 - 1, num136 - 5, num136 - 1)) ||
										(npc.direction == -1 && !Collision.SolidTiles(num135 + 1, num135 + 2, num136 - 5, num136 - 1)))
									{
										if (!Collision.SolidTiles(num135, num135, num136 - 5, num136 - 3))
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
									if (Main.tile.At(num135, num136 - 1).Active && Main.tileSolid[(int)Main.tile.At(num135, num136 - 1).Type] &&
										!Main.tileSolidTop[(int)Main.tile.At(num135, num136 - 1).Type])
									{
										if ((npc.direction == 1 && !Collision.SolidTiles(num135 - 2, num135 - 1, num136 - 4, num136 - 1)) ||
											(npc.direction == -1 && !Collision.SolidTiles(num135 + 1, num135 + 2, num136 - 4, num136 - 1)))
										{
											if (!Collision.SolidTiles(num135, num135, num136 - 4, num136 - 2))
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
									else if (Main.tile.At(num135, num136).Active && Main.tileSolid[(int)Main.tile.At(num135, num136).Type] &&
											!Main.tileSolidTop[(int)Main.tile.At(num135, num136).Type])
									{
										if ((npc.direction == 1 && !Collision.SolidTiles(num135 - 2, num135, num136 - 3, num136 - 1)) ||
											(npc.direction == -1 && !Collision.SolidTiles(num135, num135 + 2, num136 - 3, num136 - 1)))
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
								try
								{
									if (num129 >= npc.homeTileX - 35 && num129 <= npc.homeTileX + 35 && (!Main.tile.At(num135, num136 + 1).Active ||
										!Main.tileSolid[(int)Main.tile.At(num135, num136 + 1).Type]) && (!Main.tile.At(num135 - npc.direction, num136 + 1).Active ||
											!Main.tileSolid[(int)Main.tile.At(num135 - npc.direction, num136 + 1).Type]) && (!Main.tile.At(num135, num136 + 2).Active ||
												!Main.tileSolid[(int)Main.tile.At(num135, num136 + 2).Type]) && (!Main.tile.At(num135 - npc.direction, num136 + 2).Active ||
													!Main.tileSolid[(int)Main.tile.At(num135 - npc.direction, num136 + 2).Type]) &&
														(!Main.tile.At(num135, num136 + 3).Active || !Main.tileSolid[(int)Main.tile.At(num135, num136 + 3).Type]) &&
															(!Main.tile.At(num135 - npc.direction, num136 + 3).Active ||
																!Main.tileSolid[(int)Main.tile.At(num135 - npc.direction, num136 + 3).Type]) &&
																	(!Main.tile.At(num135, num136 + 4).Active || !Main.tileSolid[(int)Main.tile.At(num135, num136 + 4).Type]) &&
																		(!Main.tile.At(num135 - npc.direction, num136 + 4).Active ||
																			!Main.tileSolid[(int)Main.tile.At(num135 - npc.direction, num136 + 4).Type])
																				&& npc.type != NPCType.N46_BUNNY)
									{
										npc.direction *= -1;
										npc.Velocity.X = npc.Velocity.X * -1f;
										npc.netUpdate = true;
									}
								}
								catch
								{ }

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

		// 8 - 1.1.2
		private void AIWizard(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
			else if (npc.ai[0] >= 650f)
			{
				npc.ai[0] = 1f;
				int num145 = (int)Main.players[npc.target].Position.X / 16;
				int num146 = (int)Main.players[npc.target].Position.Y / 16;
				int num147 = (int)npc.Position.X / 16;
				int num148 = (int)npc.Position.Y / 16;
				int num149 = 20;
				int num150 = 0;
				bool flag19 = false;
				if (Math.Abs(npc.Position.X - Main.players[npc.target].Position.X) + Math.Abs(npc.Position.Y - Main.players[npc.target].Position.Y) > 2000f)
				{
					num150 = 100;
					flag19 = true;
				}
				while (!flag19 && num150 < 100)
				{
					num150++;
					int num151 = Main.rand.Next(num145 - num149, num145 + num149);
					int num152 = Main.rand.Next(num146 - num149, num146 + num149);
					for (int num153 = num152; num153 < num146 + num149; num153++)
					{
						if ((num153 < num146 - 4 || num153 > num146 + 4 || num151 < num145 - 4 || num151 > num145 + 4) &&
							(num153 < num148 - 1 || num153 > num148 + 1 || num151 < num147 - 1 || num151 > num147 + 1) &&
							Main.tile.At(num151, num153).Active)
						{
							bool flag20 = true;
							if (npc.type == NPCType.N32_DARK_CASTER && Main.tile.At(num151, num153 - 1).Wall == 0)
							{
								flag20 = false;
							}
							else if (Main.tile.At(num151, num153 - 1).Lava)
							{
								flag20 = false;
							}

							if (flag20 && Main.tileSolid[(int)Main.tile.At(num151, num153).Type] &&
								!Collision.SolidTiles(num151 - 1, num151 + 1, num153 - 4, num153 - 1))
							{
								npc.ai[1] = 20f;
								npc.ai[2] = (float)num151;
								npc.ai[3] = (float)num153;
								flag19 = true;
								break;
							}
						}
					}
				}
				npc.netUpdate = true;
			}

			if (npc.ai[1] > 0f)
			{
				npc.ai[1] -= 1f;
				if (npc.ai[1] == 25f)
				{
					if (npc.type == NPCType.N29_GOBLIN_SORCERER || npc.type == NPCType.N45_TIM)
					{
						NewNPC((int)npc.Position.X + npc.Width / 2, (int)npc.Position.Y - 8, 30, 0);
					}
					else if (npc.type == NPCType.N32_DARK_CASTER)
					{
						NewNPC((int)npc.Position.X + npc.Width / 2, (int)npc.Position.Y - 8, 33, 0);
					}
					else
					{
						NewNPC((int)npc.Position.X + npc.Width / 2 + npc.direction * 8, (int)npc.Position.Y + 20, 25, 0);
					}
				}
			}
		}

		// 9 - 1.1.2
		private void AISphere(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (npc.target == 255)
			{
				npc.TargetClosest(true);
				float num157 = 6f;
				if (npc.type == NPCType.N25_BURNING_SPHERE)
				{
					num157 = 5f;
				}
				if (npc.type == NPCType.N112_VILE_SPIT)
				{
					num157 = 7f;
				}
				Vector2 vector15 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
				float num158 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector15.X;
				float num159 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector15.Y;
				float num160 = (float)Math.Sqrt((double)(num158 * num158 + num159 * num159));
				num160 = num157 / num160;
				npc.Velocity.X = num158 * num160;
				npc.Velocity.Y = num159 * num160;
			}
			if (npc.type == NPCType.N112_VILE_SPIT)
			{
				npc.ai[0] += 1f;
				if (npc.ai[0] > 3f)
				{
					npc.ai[0] = 3f;
				}
				if (npc.ai[0] == 2f)
				{
					npc.Position += npc.Velocity;
				}
			}
			if (npc.type == NPCType.N112_VILE_SPIT && Collision.SolidCollision(npc.Position, npc.Width, npc.Height))
			{
				int num163 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
				int num164 = (int)(npc.Position.Y + (float)(npc.Height / 2)) / 16;
				int num165 = 8;
				for (int num166 = num163 - num165; num166 <= num163 + num165; num166++)
				{
					for (int num167 = num164 - num165; num167 < num164 + num165; num167++)
					{
						if ((double)(Math.Abs(num166 - num163) + Math.Abs(num167 - num164)) < (double)num165 * 0.5)
						{
							if (Main.tile.At(num166, num167).Type == 2)
							{
								Main.tile.At(num166, num167).SetType(23);
								WorldModify.SquareTileFrame(TileRefs, null, num166, num167, true);
								NetMessage.SendTileSquare(-1, num166, num167, 1);
							}
							else if (Main.tile.At(num166, num167).Type == 1)
							{
								Main.tile.At(num166, num167).SetType(25);
								WorldModify.SquareTileFrame(TileRefs, null, num166, num167, true);
								NetMessage.SendTileSquare(-1, num166, num167, 1);
							}
							else if (Main.tile.At(num166, num167).Type == 53)
							{
								Main.tile.At(num166, num167).SetType(112);
								WorldModify.SquareTileFrame(TileRefs, null, num166, num167, true);
								NetMessage.SendTileSquare(-1, num166, num167, 1);
							}
							else if (Main.tile.At(num166, num167).Type == 109)
							{
								Main.tile.At(num166, num167).SetType(23);
								WorldModify.SquareTileFrame(TileRefs, null, num166, num167, true);
								NetMessage.SendTileSquare(-1, num166, num167, 1);
							}
							else if (Main.tile.At(num166, num167).Type == 117)
							{
								Main.tile.At(num166, num167).SetType(25);
								WorldModify.SquareTileFrame(TileRefs, null, num166, num167, true);
								NetMessage.SendTileSquare(-1, num166, num167, 1);
							}
							else if (Main.tile.At(num166, num167).Type == 116)
							{
								Main.tile.At(num166, num167).SetType(112);
								WorldModify.SquareTileFrame(TileRefs, null, num166, num167, true);
								NetMessage.SendTileSquare(-1, num166, num167, 1);
							}
						}
					}
				}
				npc.StrikeNPC(World.Sender, 999, 0f, 0, false);
			}
			if (npc.timeLeft > 100)
			{
				npc.timeLeft = 100;
			}

			npc.rotation += 0.4f * (float)npc.direction;
			return;
		}

		// 10 - 1.1.2
		private void AICursedSkull(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			float num173 = 1f;
			float num174 = 0.011f;
			npc.TargetClosest(true);
			Vector2 vector16 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
			float num175 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector16.X;
			float num176 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector16.Y;
			float num177 = (float)Math.Sqrt((double)(num175 * num175 + num176 * num176));
			float num178 = num177;
			npc.ai[1] += 1f;
			if (npc.ai[1] > 600f)
			{
				num174 *= 8f;
				num173 = 4f;
				if (npc.ai[1] > 650f)
				{
					npc.ai[1] = 0f;
				}
			}
			else if (num178 < 250f)
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

			if (num178 > 350f)
			{
				num173 = 5f;
				num174 = 0.3f;
			}
			else if (num178 > 300f)
			{
				num173 = 3f;
				num174 = 0.2f;
			}
			else if (num178 > 250f)
			{
				num173 = 1.5f;
				num174 = 0.1f;
			}

			num177 = num173 / num177;
			num175 *= num177;
			num176 *= num177;

			if (Main.players[npc.target].dead)
			{
				num175 = (float)npc.direction * num173 / 2f;
				num176 = -num173 / 2f;
			}
			if (npc.Velocity.X < num175)
			{
				npc.Velocity.X = npc.Velocity.X + num174;
			}
			else if (npc.Velocity.X > num175)
			{
				npc.Velocity.X = npc.Velocity.X - num174;
			}

			if (npc.Velocity.Y < num176)
			{
				npc.Velocity.Y = npc.Velocity.Y + num174;
			}
			else if (npc.Velocity.Y > num176)
			{
				npc.Velocity.Y = npc.Velocity.Y - num174;
			}

			if (num175 > 0f)
			{
				npc.spriteDirection = -1;
				npc.rotation = (float)Math.Atan2((double)num176, (double)num175);
			}
			if (num175 < 0f)
			{
				npc.spriteDirection = 1;
				npc.rotation = (float)Math.Atan2((double)num176, (double)num175) + 3.14f;
				return;
			}
		}

		// 11 - 1.1.2
		private void AISkeletronHead(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (npc.ai[0] == 0f)
			{
				npc.TargetClosest(true);
				npc.ai[0] = 1f;
				if (npc.type != NPCType.N68_DUNGEON_GUARDIAN)
				{
					int num179 = NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 36, npc.whoAmI);
					Main.npcs[num179].ai[0] = -1f;
					Main.npcs[num179].ai[1] = (float)npc.whoAmI;
					Main.npcs[num179].target = npc.target;
					Main.npcs[num179].netUpdate = true;
					num179 = NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 36, npc.whoAmI);
					Main.npcs[num179].ai[0] = 1f;
					Main.npcs[num179].ai[1] = (float)npc.whoAmI;
					Main.npcs[num179].ai[3] = 150f;
					Main.npcs[num179].target = npc.target;
					Main.npcs[num179].netUpdate = true;
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
				else if (npc.Position.Y < Main.players[npc.target].Position.Y - 250f)
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
			else if (npc.ai[1] == 1f)
			{
				npc.defense = 0;
				npc.ai[2] += 1f;

				if (npc.ai[2] >= 400f)
				{
					npc.ai[2] = 0f;
					npc.ai[1] = 0f;
				}
				npc.rotation += (float)npc.direction * 0.3f;
				Vector2 vector17 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
				float num180 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector17.X;
				float num181 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector17.Y;
				float num182 = (float)Math.Sqrt((double)(num180 * num180 + num181 * num181));
				num182 = 1.5f / num182;
				npc.Velocity.X = num180 * num182;
				npc.Velocity.Y = num181 * num182;
			}
			else if (npc.ai[1] == 2f)
			{
				npc.damage = 9999;
				npc.defense = 9999;
				npc.rotation += (float)npc.direction * 0.3f;
				Vector2 vector18 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
				float num183 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector18.X;
				float num184 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector18.Y;
				float num185 = (float)Math.Sqrt((double)(num183 * num183 + num184 * num184));
				num185 = 8f / num185;
				npc.Velocity.X = num183 * num185;
				npc.Velocity.Y = num184 * num185;
			}
			else if (npc.ai[1] == 3f)
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

		// 12 - 1.1.2
		private void AISkeletronHand(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
					else if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y - 100f)
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
					else if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y + 230f)
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
				Vector2 vector19 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
				float num188 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector19.X;
				float num189 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector19.Y;
				Math.Sqrt((double)(num188 * num188 + num189 * num189));
				npc.rotation = (float)Math.Atan2((double)num189, (double)num188) + 1.57f;
				return;
			}
			if (npc.ai[2] == 1f)
			{
				Vector2 vector20 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
				float num190 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector20.X;
				float num191 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector20.Y;
				float num192 = (float)Math.Sqrt((double)(num190 * num190 + num191 * num191));
				npc.rotation = (float)Math.Atan2((double)num191, (double)num190) + 1.57f;
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
					vector20 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					num190 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector20.X;
					num191 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector20.Y;
					num192 = (float)Math.Sqrt((double)(num190 * num190 + num191 * num191));
					num192 = 18f / num192;
					npc.Velocity.X = num190 * num192;
					npc.Velocity.Y = num191 * num192;
					npc.netUpdate = true;
					return;
				}
			}
			else if (npc.ai[2] == 2f && npc.Position.Y > Main.players[npc.target].Position.Y || npc.Velocity.Y < 0f)
			{
				npc.ai[2] = 3f;
				return;
			}
			else if (npc.ai[2] == 4f)
			{
				Vector2 vector21 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
				float num193 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector21.X;
				float num194 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector21.Y;
				float num195 = (float)Math.Sqrt((double)(num193 * num193 + num194 * num194));
				npc.rotation = (float)Math.Atan2((double)num194, (double)num193) + 1.57f;
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
					vector21 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					num193 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector21.X;
					num194 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector21.Y;
					num195 = (float)Math.Sqrt((double)(num193 * num193 + num194 * num194));
					num195 = 17f / num195;
					npc.Velocity.X = num193 * num195;
					npc.Velocity.Y = num194 * num195;
					npc.netUpdate = true;
					return;
				}
			}
			else if (npc.ai[2] == 5f && ((npc.Velocity.X > 0f && npc.Position.X + (float)(npc.Width / 2) > Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2)) || (npc.Velocity.X < 0f && npc.Position.X + (float)(npc.Width / 2) < Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2))))
			{
				npc.ai[2] = 0f;
				return;
			}
		}

		// 13 - 1.1.2
		private void AIMunchyPlant(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (!Main.tile.At((int)npc.ai[0], (int)npc.ai[1]).Active)
			{
				npc.life = -1;
				npc.HitEffect(0, 10.0);
				npc.Active = false;
				return;
			}
			npc.TargetClosest(true);
			float num196 = 0.035f;
			float num197 = 150f;

			if (npc.type == NPCType.N43_MAN_EATER)
			{
				num197 = 250f;
			}
			if (npc.type == NPCType.N101_CLINGER)
			{
				num197 = 175f;
			}

			npc.ai[2] += 1f;

			if (npc.ai[2] > 300f)
			{
				num197 = (float)((int)((double)num197 * 1.3));
				if (npc.ai[2] > 450f)
				{
					npc.ai[2] = 0f;
				}
			}

			Vector2 vector22 = new Vector2(npc.ai[0] * 16f + 8f, npc.ai[1] * 16f + 8f);
			float num198 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - (float)(npc.Width / 2) - vector22.X;
			float num199 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - (float)(npc.Height / 2) - vector22.Y;
			float num200 = (float)Math.Sqrt((double)(num198 * num198 + num199 * num199));
			if (num200 > num197)
			{
				num200 = num197 / num200;
				num198 *= num200;
				num199 *= num200;
			}
			if (npc.Position.X < npc.ai[0] * 16f + 8f + num198)
			{
				npc.Velocity.X = npc.Velocity.X + num196;
				if (npc.Velocity.X < 0f && num198 > 0f)
				{
					npc.Velocity.X = npc.Velocity.X + num196 * 1.5f;
				}
			}
			else if (npc.Position.X > npc.ai[0] * 16f + 8f + num198)
			{
				npc.Velocity.X = npc.Velocity.X - num196;
				if (npc.Velocity.X > 0f && num198 < 0f)
				{
					npc.Velocity.X = npc.Velocity.X - num196 * 1.5f;
				}
			}

			if (npc.Position.Y < npc.ai[1] * 16f + 8f + num199)
			{
				npc.Velocity.Y = npc.Velocity.Y + num196;
				if (npc.Velocity.Y < 0f && num199 > 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y + num196 * 1.5f;
				}
			}
			else if (npc.Position.Y > npc.ai[1] * 16f + 8f + num199)
			{
				npc.Velocity.Y = npc.Velocity.Y - num196;
				if (npc.Velocity.Y > 0f && num199 < 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y - num196 * 1.5f;
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
			if (num198 > 0f)
			{
				npc.spriteDirection = 1;
				npc.rotation = (float)Math.Atan2((double)num199, (double)num198);
			}
			if (num198 < 0f)
			{
				npc.spriteDirection = -1;
				npc.rotation = (float)Math.Atan2((double)num199, (double)num198) + 3.14f;
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
				}
			}
			if (npc.type == NPCType.N101_CLINGER && !Main.players[npc.target].dead)
			{
				if (npc.justHit)
				{
					npc.localAI[0] = 0f;
				}
				npc.localAI[0] += 1f;
				if (npc.localAI[0] >= 120f)
				{
					if (!Collision.SolidCollision(npc.Position, npc.Width, npc.Height) && Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
					{
						float num201 = 10f;
						vector22 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
						num198 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width * 0.5f - vector22.X + (float)Main.rand.Next(-10, 11);
						num199 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].Height * 0.5f - vector22.Y + (float)Main.rand.Next(-10, 11);
						num200 = (float)Math.Sqrt((double)(num198 * num198 + num199 * num199));
						num200 = num201 / num200;
						num198 *= num200;
						num199 *= num200;
						int num202 = 22;
						int num203 = 96;
						int num204 = Projectile.NewProjectile(vector22.X, vector22.Y, num198, num199, num203, num202, 0f, Main.myPlayer);
						Main.projectile[num204].timeLeft = 300;
						npc.localAI[0] = 0f;
						return;
					}
					npc.localAI[0] = 100f;
					return;
				}
			}
		}

		// 14 - 1.1.2
		private void AIFlyWinged(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
				else if (npc.Velocity.X > 0f)
				{
					npc.Velocity.X = npc.Velocity.X + 0.05f;
				}
				if (npc.Velocity.X < -4f)
				{
					npc.Velocity.X = -4f;
				}
			}
			else if (npc.direction == 1 && npc.Velocity.X < 4f)
			{
				npc.Velocity.X = npc.Velocity.X + 0.1f;
				if (npc.Velocity.X < -4f)
				{
					npc.Velocity.X = npc.Velocity.X + 0.1f;
				}
				else if (npc.Velocity.X < 0f)
				{
					npc.Velocity.X = npc.Velocity.X - 0.05f;
				}
				if (npc.Velocity.X > 4f)
				{
					npc.Velocity.X = 4f;
				}
			}

			if (npc.directionY == -1 && (double)npc.Velocity.Y > -1.5)
			{
				npc.Velocity.Y = npc.Velocity.Y - 0.04f;
				if ((double)npc.Velocity.Y > 1.5)
				{
					npc.Velocity.Y = npc.Velocity.Y - 0.05f;
				}
				else if (npc.Velocity.Y > 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.03f;
				}
				if ((double)npc.Velocity.Y < -1.5)
				{
					npc.Velocity.Y = -1.5f;
				}
			}
			else if (npc.directionY == 1 && (double)npc.Velocity.Y < 1.5)
			{
				npc.Velocity.Y = npc.Velocity.Y + 0.04f;
				if ((double)npc.Velocity.Y < -1.5)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.05f;
				}
				else if (npc.Velocity.Y < 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y - 0.03f;
				}
				if ((double)npc.Velocity.Y > 1.5)
				{
					npc.Velocity.Y = 1.5f;
				}
			}

			if (npc.type == NPCType.N49_CAVE_BAT || npc.type == NPCType.N51_JUNGLE_BAT || npc.type == NPCType.N60_HELLBAT ||
				npc.type == NPCType.N62_DEMON || npc.type == NPCType.N66_VOODOO_DEMON || npc.type == NPCType.N93_GIANT_BAT ||
				npc.type == NPCType.N137_ILLUMINANT_BAT)
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
				if (npc.type == NPCType.N60_HELLBAT)
				{
					if (npc.direction == -1 && npc.Velocity.X > -4f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.1f;
						if (npc.Velocity.X > 4f)
						{
							npc.Velocity.X = npc.Velocity.X - 0.07f;
						}
						else if (npc.Velocity.X > 0f)
						{
							npc.Velocity.X = npc.Velocity.X + 0.03f;
						}

						if (npc.Velocity.X < -4f)
						{
							npc.Velocity.X = -4f;
						}
					}
					else if (npc.direction == 1 && npc.Velocity.X < 4f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.1f;
						if (npc.Velocity.X < -4f)
						{
							npc.Velocity.X = npc.Velocity.X + 0.07f;
						}
						else if (npc.Velocity.X < 0f)
						{
							npc.Velocity.X = npc.Velocity.X - 0.03f;
						}
						if (npc.Velocity.X > 4f)
						{
							npc.Velocity.X = 4f;
						}
					}

					if (npc.directionY == -1 && (double)npc.Velocity.Y > -1.5)
					{
						npc.Velocity.Y = npc.Velocity.Y - 0.04f;
						if ((double)npc.Velocity.Y > 1.5)
						{
							npc.Velocity.Y = npc.Velocity.Y - 0.03f;
						}
						else if (npc.Velocity.Y > 0f)
						{
							npc.Velocity.Y = npc.Velocity.Y + 0.02f;
						}
						if ((double)npc.Velocity.Y < -1.5)
						{
							npc.Velocity.Y = -1.5f;
						}
					}
					else if (npc.directionY == 1 && (double)npc.Velocity.Y < 1.5)
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
				else if (npc.direction == -1 && npc.Velocity.X > -4f)
				{
					npc.Velocity.X = npc.Velocity.X - 0.1f;
					if (npc.Velocity.X > 4f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.1f;
					}
					else if (npc.Velocity.X > 0f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.05f;
					}

					if (npc.Velocity.X < -4f)
					{
						npc.Velocity.X = -4f;
					}
				}
				else if (npc.direction == 1 && npc.Velocity.X < 4f)
				{
					npc.Velocity.X = npc.Velocity.X + 0.1f;
					if (npc.Velocity.X < -4f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.1f;
					}
					else if (npc.Velocity.X < 0f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.05f;
					}

					if (npc.Velocity.X > 4f)
					{
						npc.Velocity.X = 4f;
					}
				}

				if (npc.directionY == -1 && (double)npc.Velocity.Y > -1.5)
				{
					npc.Velocity.Y = npc.Velocity.Y - 0.04f;
					if ((double)npc.Velocity.Y > 1.5)
					{
						npc.Velocity.Y = npc.Velocity.Y - 0.05f;
					}
					else if (npc.Velocity.Y > 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y + 0.03f;
					}

					if ((double)npc.Velocity.Y < -1.5)
					{
						npc.Velocity.Y = -1.5f;
					}
				}
				else if (npc.directionY == 1 && (double)npc.Velocity.Y < 1.5)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.04f;
					if ((double)npc.Velocity.Y < -1.5)
					{
						npc.Velocity.Y = npc.Velocity.Y + 0.05f;
					}
					else if (npc.Velocity.Y < 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y - 0.03f;
					}

					if ((double)npc.Velocity.Y > 1.5)
					{
						npc.Velocity.Y = 1.5f;
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
				float num206 = 0.2f;
				float num207 = 0.1f;
				float num208 = 4f;
				float num209 = 1.5f;
				if (npc.type == NPCType.N48_HARPY || npc.type == NPCType.N62_DEMON || npc.type == NPCType.N66_VOODOO_DEMON)
				{
					num206 = 0.12f;
					num207 = 0.07f;
					num208 = 3f;
					num209 = 1.25f;
				}
				if (npc.ai[1] > 1000f)
				{
					npc.ai[1] = 0f;
				}
				npc.ai[2] += 1f;
				if (npc.ai[2] > 0f)
				{
					if (npc.Velocity.Y < num209)
					{
						npc.Velocity.Y = npc.Velocity.Y + num207;
					}
				}
				else if (npc.Velocity.Y > -num209)
				{
					npc.Velocity.Y = npc.Velocity.Y - num207;
				}

				if (npc.ai[2] < -150f || npc.ai[2] > 150f)
				{
					if (npc.Velocity.X < num208)
					{
						npc.Velocity.X = npc.Velocity.X + num206;
					}
				}
				else if (npc.Velocity.X > -num208)
				{
					npc.Velocity.X = npc.Velocity.X - num206;
				}

				if (npc.ai[2] > 300f)
				{
					npc.ai[2] = -300f;
				}
			}

			if (npc.type == NPCType.N48_HARPY)
			{
				npc.ai[0] += 1f;
				if (npc.ai[0] == 30f || npc.ai[0] == 60f || npc.ai[0] == 90f)
				{
					if (Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
					{
						float num210 = 6f;
						Vector2 vector23 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
						float num211 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width * 0.5f - vector23.X + (float)Main.rand.Next(-100, 101);
						float num212 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].Height * 0.5f - vector23.Y + (float)Main.rand.Next(-100, 101);
						float num213 = (float)Math.Sqrt((double)(num211 * num211 + num212 * num212));
						num213 = num210 / num213;
						num211 *= num213;
						num212 *= num213;
						int num214 = 15;
						int num215 = 38;
						int num216 = Projectile.NewProjectile(vector23.X, vector23.Y, num211, num212, num215, num214, 0f, Main.myPlayer);
						Main.projectile[num216].timeLeft = 300;
					}
				}
				else if (npc.ai[0] >= (float)(400 + Main.rand.Next(400)))
				{
					npc.ai[0] = 0f;
				}
			}
			if (npc.type == NPCType.N62_DEMON || npc.type == NPCType.N66_VOODOO_DEMON)
			{
				npc.ai[0] += 1f;
				if (npc.ai[0] == 20f || npc.ai[0] == 40f || npc.ai[0] == 60f || npc.ai[0] == 80f)
				{
					if (Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
					{
						float num217 = 0.2f;
						Vector2 vector24 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
						float num218 = Main.players[npc.target].Position.X + (float)Main.players[npc.target].Width * 0.5f - vector24.X + (float)Main.rand.Next(-100, 101);
						float num219 = Main.players[npc.target].Position.Y + (float)Main.players[npc.target].Height * 0.5f - vector24.Y + (float)Main.rand.Next(-100, 101);
						float num220 = (float)Math.Sqrt((double)(num218 * num218 + num219 * num219));
						num220 = num217 / num220;
						num218 *= num220;
						num219 *= num220;
						int num221 = 21;
						int num222 = 44;
						int num223 = Projectile.NewProjectile(vector24.X, vector24.Y, num218, num219, num222, num221, 0f, Main.myPlayer);
						Main.projectile[num223].timeLeft = 300;
						return;
					}
				}
				else if (npc.ai[0] >= (float)(300 + Main.rand.Next(300)))
				{
					npc.ai[0] = 0f;
					return;
				}
			}
		}

		// 15 - 1.1.2
		private void AIKingSlime(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
					else if (npc.ai[1] == 2f)
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
				else if (npc.ai[0] >= -30f)
				{
					npc.aiAction = 1;
				}
			}
			else if (npc.target < 255 && ((npc.direction == 1 && npc.Velocity.X < 3f) || (npc.direction == -1 && npc.Velocity.X > -3f)))
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

			if (npc.life > 0)
			{
				float num225 = (float)npc.life / (float)npc.lifeMax;
				num225 = num225 * 0.5f + 0.75f;
				if (num225 != npc.scale)
				{
					npc.Position.X = npc.Position.X + (float)(npc.Width / 2);
					npc.Position.Y = npc.Position.Y + (float)npc.Height;
					npc.scale = num225;
					npc.Width = (int)(98f * npc.scale);
					npc.Height = (int)(92f * npc.scale);
					npc.Position.X = npc.Position.X - (float)(npc.Width / 2);
					npc.Position.Y = npc.Position.Y - (float)npc.Height;
				}
				int num226 = (int)((double)npc.lifeMax * 0.05);
				if ((float)(npc.life + num226) < npc.ai[3])
				{
					npc.ai[3] = (float)npc.life;
					int num227 = Main.rand.Next(1, 4);
					for (int num228 = 0; num228 < num227; num228++)
					{
						int x = (int)(npc.Position.X + (float)Main.rand.Next(npc.Width - 32));
						int y = (int)(npc.Position.Y + (float)Main.rand.Next(npc.Height - 32));
						int num229 = NewNPC(x, y, 1, 0);
						Main.npcs[num229].SetDefaults(1);
						Main.npcs[num229].Velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
						Main.npcs[num229].Velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
						Main.npcs[num229].ai[1] = (float)Main.rand.Next(3);

						if (num229 < 200)
							NetMessage.SendData(23, -1, -1, String.Empty, num229);
					}
					return;
				}
			}
		}

		// 16 - 1.1.2
		private void AIFish(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (npc.direction == 0)
			{
				npc.TargetClosest(true);
			}
			if (npc.wet)
			{
				bool flag21 = false;
				if (npc.type != NPCType.N55_GOLDFISH)
				{
					npc.TargetClosest(false);
					if (Main.players[npc.target].wet && !Main.players[npc.target].dead)
					{
						flag21 = true;
					}
				}
				if (!flag21)
				{
					if (npc.collideX)
					{
						npc.Velocity.X = npc.Velocity.X * -1f;
						npc.direction *= -1;
						npc.netUpdate = true;
					}
					if (npc.collideY)
					{
						npc.netUpdate = true;
						if (npc.Velocity.Y > 0f)
						{
							npc.Velocity.Y = Math.Abs(npc.Velocity.Y) * -1f;
							npc.directionY = -1;
							npc.ai[0] = -1f;
						}
						else if (npc.Velocity.Y < 0f)
						{
							npc.Velocity.Y = Math.Abs(npc.Velocity.Y);
							npc.directionY = 1;
							npc.ai[0] = 1f;
						}
					}
				}

				if (flag21)
				{
					npc.TargetClosest(true);
					if (npc.type == NPCType.N65_SHARK || npc.type == NPCType.N102_ANGLER_FISH)
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
					int num230 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
					int num231 = (int)(npc.Position.Y + (float)(npc.Height / 2)) / 16;

					if (Main.tile.At(num230, num231 - 1).Liquid > 128)
					{
						if (Main.tile.At(num230, num231 + 1).Active)
						{
							npc.ai[0] = -1f;
						}
						else if (Main.tile.At(num230, num231 + 2).Active)
						{
							npc.ai[0] = -1f;
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

		// 17 - 1.1.2
		private void AIVulture(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
			else if (!Main.players[npc.target].dead)
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
					else if (npc.Velocity.X > 0f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.05f;
					}

					if (npc.Velocity.X < -3f)
					{
						npc.Velocity.X = -3f;
					}
				}
				else if (npc.direction == 1 && npc.Velocity.X < 3f)
				{
					npc.Velocity.X = npc.Velocity.X + 0.1f;
					if (npc.Velocity.X < -3f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.1f;
					}
					else if (npc.Velocity.X < 0f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.05f;
					}

					if (npc.Velocity.X > 3f)
					{
						npc.Velocity.X = 3f;
					}
				}

				float num232 = Math.Abs(npc.Position.X + (float)(npc.Width / 2) - (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2)));
				float num233 = Main.players[npc.target].Position.Y - (float)(npc.Height / 2);
				if (num232 > 50f)
				{
					num233 -= 100f;
				}
				if (npc.Position.Y < num233)
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

		// 18 - 1.1.2
		private void AIJellyFish(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
				else if (npc.Velocity.Y < 0f)
				{
					npc.Velocity.Y = Math.Abs(npc.Velocity.Y);
					npc.directionY = 1;
					npc.ai[0] = 1f;
				}
			}
			bool flag22 = false;
			if (!npc.friendly)
			{
				npc.TargetClosest(false);
				if (Main.players[npc.target].wet && !Main.players[npc.target].dead)
				{
					flag22 = true;
				}
			}
			if (flag22)
			{
				npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) + 1.57f;
				npc.Velocity *= 0.98f;
				float num234 = 0.2f;
				if (npc.type == NPCType.N103_GREEN_JELLYFISH)
				{
					npc.Velocity *= 0.98f;
					num234 = 0.6f;
				}
				if (npc.Velocity.X > -num234 && npc.Velocity.X < num234 && npc.Velocity.Y > -num234 && npc.Velocity.Y < num234)
				{
					npc.TargetClosest(true);
					float num235 = 7f;
					if (npc.type == NPCType.N103_GREEN_JELLYFISH)
					{
						num235 = 9f;
					}
					Vector2 vector25 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
					float num236 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector25.X;
					float num237 = Main.players[npc.target].Position.Y + (float)(Main.players[npc.target].Height / 2) - vector25.Y;
					float num238 = (float)Math.Sqrt((double)(num236 * num236 + num237 * num237));
					num238 = num235 / num238;
					num236 *= num238;
					num237 *= num238;
					npc.Velocity.X = num236;
					npc.Velocity.Y = num237;
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
				int num239 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
				int num240 = (int)(npc.Position.Y + (float)(npc.Height / 2)) / 16;

				if (Main.tile.At(num239, num240 - 1).Liquid > 128)
				{
					if (Main.tile.At(num239, num240 + 1).Active)
					{
						npc.ai[0] = -1f;
					}
					else if (Main.tile.At(num239, num240 + 2).Active)
					{
						npc.ai[0] = -1f;
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

		// 19 - 1.1.2
		private void AIAntlion(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			npc.TargetClosest(true);
			float num241 = 12f;
			Vector2 vector26 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
			float num242 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector26.X;
			float num243 = Main.players[npc.target].Position.Y - vector26.Y;
			float num244 = (float)Math.Sqrt((double)(num242 * num242 + num243 * num243));
			num244 = num241 / num244;
			num242 *= num244;
			num243 *= num244;
			bool flag23 = false;
			if (npc.directionY < 0)
			{
				npc.rotation = (float)(Math.Atan2((double)num243, (double)num242) + 1.57);
				flag23 = ((double)npc.rotation >= -1.2 && (double)npc.rotation <= 1.2);
				if ((double)npc.rotation < -0.8)
				{
					npc.rotation = -0.8f;
				}
				else if ((double)npc.rotation > 0.8)
				{
					npc.rotation = 0.8f;
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
			if (flag23 && npc.ai[0] == 0f && Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
			{
				npc.ai[0] = 200f;
				int num245 = 10;
				int num246 = 31;
				int num247 = Projectile.NewProjectile(vector26.X, vector26.Y, num242, num243, num246, num245, 0f, Main.myPlayer);
				Main.projectile[num247].ai[0] = 2f;
				Main.projectile[num247].timeLeft = 300;
				Main.projectile[num247].friendly = false;
				NetMessage.SendData(27, -1, -1, "", num247, 0f, 0f, 0f, 0);
				npc.netUpdate = true;
			}
			try
			{
				int num248 = (int)npc.Position.X / 16;
				int num249 = (int)(npc.Position.X + (float)(npc.Width / 2)) / 16;
				int num250 = (int)(npc.Position.X + (float)npc.Width) / 16;
				int num251 = (int)(npc.Position.Y + (float)npc.Height) / 16;
				bool flag24 = false;

				if ((Main.tile.At(num248, num251).Active && Main.tileSolid[(int)Main.tile.At(num248, num251).Type]) ||
					(Main.tile.At(num249, num251).Active && Main.tileSolid[(int)Main.tile.At(num249, num251).Type]) ||
						(Main.tile.At(num250, num251).Active && Main.tileSolid[(int)Main.tile.At(num250, num251).Type]))
				{
					flag24 = true;
				}
				if (flag24)
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
			{ }
		}

		// 20 - 1.1.2
		private void AISpikedBall(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
			}
			else
			{
				float num253 = 6f * npc.ai[3];
				float num254 = 0.2f * npc.ai[3];
				float num255 = num253 / num254 / 2f;
				if (npc.ai[0] >= 1f && npc.ai[0] < (float)((int)num255))
				{
					npc.Velocity.Y = (float)npc.directionY * num253;
					npc.ai[0] += 1f;
					return;
				}

				if (npc.ai[0] >= (float)((int)num255))
				{
					npc.netUpdate = true;
					npc.Velocity.Y = 0f;
					npc.directionY *= -1;
					npc.Velocity.X = num253 * (float)npc.direction;
					npc.ai[0] = -1f;
					return;
				}

				if (npc.directionY > 0 && npc.Velocity.Y >= num253)
				{
					npc.netUpdate = true;
					npc.directionY *= -1;
					npc.Velocity.Y = num253;
				}
				else if (npc.directionY < 0 && npc.Velocity.Y <= -num253)
				{
					npc.directionY *= -1;
					npc.Velocity.Y = -num253;
				}

				if (npc.direction > 0 && npc.Velocity.X >= num253)
				{
					npc.direction *= -1;
					npc.Velocity.X = num253;
				}
				else if (npc.direction < 0 && npc.Velocity.X <= -num253)
				{
					npc.direction *= -1;
					npc.Velocity.X = -num253;
				}
				npc.Velocity.X = npc.Velocity.X + num254 * (float)npc.direction;
				npc.Velocity.Y = npc.Velocity.Y + num254 * (float)npc.directionY;
			}
		}

		// 21 - 1.1.2
		private void AIBlazingWheel(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (npc.ai[0] == 0f)
			{
				npc.TargetClosest(true);
				npc.directionY = 1;
				npc.ai[0] = 1f;
			}
			int num256 = 6;
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
			npc.Velocity.X = (float)(num256 * npc.direction);
			npc.Velocity.Y = (float)(num256 * npc.directionY);
		}

		// 22 - 1.1.2
		private void AISlowDebuffFlying(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
				else if ((npc.Velocity.X < 0f && npc.direction > 0) || (npc.Velocity.X > 0f && npc.direction < 0))
				{
					flag26 = true;
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
			if (npc.type == NPCType.N122_GASTROPOD)
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
			else if (npc.type == NPCType.N75_PIXIE)
			{
				num261 = 4;
			}

			for (int num269 = num260; num269 < num260 + num261; num269++)
			{
				if ((Main.tile.At(num259, num269).Active && Main.tileSolid[(int)Main.tile.At(num259, num269).Type]) ||
					Main.tile.At(num259, num269).Liquid > 0)
				{
					if (num269 <= num260 + 1)
					{
						flag29 = true;
					}
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
				if (npc.type == NPCType.N75_PIXIE)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.2f;
					if (npc.Velocity.Y > 2f)
					{
						npc.Velocity.Y = 2f;
					}
				}
				else
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.1f;
					if (npc.Velocity.Y > 3f)
					{
						npc.Velocity.Y = 3f;
					}
				}
			}
			else
			{
				if (npc.type == NPCType.N75_PIXIE)
				{
					if ((npc.directionY < 0 && npc.Velocity.Y > 0f) || flag29)
					{
						npc.Velocity.Y = npc.Velocity.Y - 0.2f;
					}
				}
				else if (npc.directionY < 0 && npc.Velocity.Y > 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y - 0.1f;
				}

				if (npc.Velocity.Y < -4f)
				{
					npc.Velocity.Y = -4f;
				}
			}
			if (npc.type == NPCType.N75_PIXIE && npc.wet)
			{
				npc.Velocity.Y = npc.Velocity.Y - 0.2f;
				if (npc.Velocity.Y < -2f)
				{
					npc.Velocity.Y = -2f;
				}
			}
			if (npc.collideX)
			{
				npc.Velocity.X = npc.oldVelocity.X * -0.4f;
				if (npc.direction == -1 && npc.Velocity.X > 0f && npc.Velocity.X < 1f)
				{
					npc.Velocity.X = 1f;
				}
				if (npc.direction == 1 && npc.Velocity.X < 0f && npc.Velocity.X > -1f)
				{
					npc.Velocity.X = -1f;
				}
			}
			if (npc.collideY)
			{
				npc.Velocity.Y = npc.oldVelocity.Y * -0.25f;
				if (npc.Velocity.Y > 0f && npc.Velocity.Y < 1f)
				{
					npc.Velocity.Y = 1f;
				}
				if (npc.Velocity.Y < 0f && npc.Velocity.Y > -1f)
				{
					npc.Velocity.Y = -1f;
				}
			}
			float num270 = 2f;
			if (npc.type == NPCType.N75_PIXIE)
			{
				num270 = 3f;
			}
			if (npc.direction == -1 && npc.Velocity.X > -num270)
			{
				npc.Velocity.X = npc.Velocity.X - 0.1f;
				if (npc.Velocity.X > num270)
				{
					npc.Velocity.X = npc.Velocity.X - 0.1f;
				}
				else if (npc.Velocity.X > 0f)
				{
					npc.Velocity.X = npc.Velocity.X + 0.05f;
				}

				if (npc.Velocity.X < -num270)
				{
					npc.Velocity.X = -num270;
				}
			}
			else if (npc.direction == 1 && npc.Velocity.X < num270)
			{
				npc.Velocity.X = npc.Velocity.X + 0.1f;
				if (npc.Velocity.X < -num270)
				{
					npc.Velocity.X = npc.Velocity.X + 0.1f;
				}
				else if (npc.Velocity.X < 0f)
				{
					npc.Velocity.X = npc.Velocity.X - 0.05f;
				}

				if (npc.Velocity.X > num270)
				{
					npc.Velocity.X = num270;
				}
			}

			if (npc.directionY == -1 && (double)npc.Velocity.Y > -1.5)
			{
				npc.Velocity.Y = npc.Velocity.Y - 0.04f;
				if ((double)npc.Velocity.Y > 1.5)
				{
					npc.Velocity.Y = npc.Velocity.Y - 0.05f;
				}
				else if (npc.Velocity.Y > 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.03f;
				}

				if ((double)npc.Velocity.Y < -1.5)
				{
					npc.Velocity.Y = -1.5f;
				}
			}
			else if (npc.directionY == 1 && (double)npc.Velocity.Y < 1.5)
			{
				npc.Velocity.Y = npc.Velocity.Y + 0.04f;
				if ((double)npc.Velocity.Y < -1.5)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.05f;
				}
				else if (npc.Velocity.Y < 0f)
				{
					npc.Velocity.Y = npc.Velocity.Y - 0.03f;
				}

				if ((double)npc.Velocity.Y > 1.5)
				{
					npc.Velocity.Y = 1.5f;
				}
			}
		}

		// 23 - 1.1.2
		private void AITool(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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

		// 24 - 1.1.2
		private void AIBird(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
					Rectangle rectangle7 = new Rectangle((int)Main.players[npc.target].Position.X, (int)Main.players[npc.target].Position.Y, Main.players[npc.target].Width, Main.players[npc.target].Height);
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
			else if (!Main.players[npc.target].dead)
			{
				if (npc.collideX)
				{
					npc.direction *= -1;
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

				if (npc.direction == -1 && npc.Velocity.X > -3f)
				{
					npc.Velocity.X = npc.Velocity.X - 0.1f;
					if (npc.Velocity.X > 3f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.1f;
					}
					else if (npc.Velocity.X > 0f)
					{
						npc.Velocity.X = npc.Velocity.X - 0.05f;
					}

					if (npc.Velocity.X < -3f)
					{
						npc.Velocity.X = -3f;
					}
				}
				else if (npc.direction == 1 && npc.Velocity.X < 3f)
				{
					npc.Velocity.X = npc.Velocity.X + 0.1f;
					if (npc.Velocity.X < -3f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.1f;
					}
					else if (npc.Velocity.X < 0f)
					{
						npc.Velocity.X = npc.Velocity.X + 0.05f;
					}

					if (npc.Velocity.X > 3f)
					{
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
					if ((Main.tile.At(num276, num279).Active && Main.tileSolid[(int)Main.tile.At(num276, num279).Type]) ||
						Main.tile.At(num276, num279).Liquid > 0)
					{
						if (num279 < num277 + 5)
						{
							flag31 = true;
						}
						flag30 = false;
						break;
					}
				}
				if (flag30)
				{
					npc.Velocity.Y = npc.Velocity.Y + 0.1f;
				}
				else
				{
					npc.Velocity.Y = npc.Velocity.Y - 0.1f;
				}
				if (flag31)
				{
					npc.Velocity.Y = npc.Velocity.Y - 0.2f;
				}
				if (npc.Velocity.Y > 3f)
				{
					npc.Velocity.Y = 3f;
				}
				if (npc.Velocity.Y < -4f)
				{
					npc.Velocity.Y = -4f;
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

		// 25 - 1.1.2
		private void AIMimic(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (npc.ai[3] == 0f)
			{
				npc.Position.X = npc.Position.X + 8f;
				if (npc.Position.Y / 16f > (float)(Main.maxTilesY - 200))
				{
					npc.ai[3] = 3f;
				}
				else if ((double)(npc.Position.Y / 16f) > Main.worldSurface)
				{
					npc.ai[3] = 2f;
				}
				else
				{
					npc.ai[3] = 1f;
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
			else if (npc.Velocity.Y == 0f)
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
			}
			else
			{
				if (npc.direction == 1 && npc.Velocity.X < 1f)
				{
					npc.Velocity.X = npc.Velocity.X + 0.1f;
				}
				if (npc.direction == -1 && npc.Velocity.X > -1f)
				{
					npc.Velocity.X = npc.Velocity.X - 0.1f;
				}
			}
		}

		// 26 - 1.1.2
		private void AIUnicorn(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
			else if (npc.ai[3] > 0f)
			{
				npc.ai[3] -= 1f;
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
				else if (npc.Velocity.X < num282 && npc.direction == 1)
				{
					npc.Velocity.X = npc.Velocity.X + 0.07f;
					if (npc.Velocity.X > num282)
					{
						npc.Velocity.X = num282;
					}
				}
				else if (npc.Velocity.X > -num282 && npc.direction == -1)
				{
					npc.Velocity.X = npc.Velocity.X - 0.07f;
					if (npc.Velocity.X < -num282)
					{
						npc.Velocity.X = -num282;
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
						if ((npc.directionY < 0 || Math.Abs(npc.Velocity.X) > 3f) && (!Main.tile.At(num283, num284 + 1).Active ||
							!Main.tileSolid[(int)Main.tile.At(num283, num284 + 1).Type]) &&
								(!Main.tile.At(num283 + npc.direction, num284 + 1).Active ||
									!Main.tileSolid[(int)Main.tile.At(num283 + npc.direction, num284 + 1).Type]))
						{
							npc.Velocity.Y = -8f;
							npc.netUpdate = true;
							return;
						}
					}
				}
			}
		}

		// 27 - 1.1.2
		private void AIWallOfFlesh(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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

				int num286 = NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)(npc.Position.Y + (float)(npc.Height / 2) + 20f),
					(int)NPCType.N117_LEECH_HEAD, 1, npc.MadeSpawn);
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
						if (WorldModify.SolidTile(TileRefs, num292, tileY) || Main.tile.At(num292, tileY).Liquid > 0)
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
						if (WorldModify.SolidTile(TileRefs, tileX, tileY) || Main.tile.At(tileX, tileY).Liquid > 0)
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
				int num299 = NewNPC((int)npc.Position.X, (int)num294, 114, npc.whoAmI);
				Main.npcs[num299].ai[0] = 1f;
				num294 = (float)((Main.WallOfFlesh_B + Main.WallOfFlesh_T) / 2);
				num294 = (num294 + (float)Main.WallOfFlesh_B) / 2f;
				num299 = NewNPC((int)npc.Position.X, (int)num294, 114, npc.whoAmI);
				Main.npcs[num299].ai[0] = -1f;
				num294 = (float)((Main.WallOfFlesh_B + Main.WallOfFlesh_T) / 2);
				num294 = (num294 + (float)Main.WallOfFlesh_B) / 2f;
				for (int num300 = 0; num300 < 11; num300++)
				{
					num299 = NewNPC((int)npc.Position.X, (int)num294, (int)NPCType.N115_THE_HUNGRY, npc.whoAmI, npc.MadeSpawn);
					Main.npcs[num299].ai[0] = (float)num300 * 0.1f - 0.05f;
				}
				return;
			}
		}

		// 28 - 1.1.2
		private void AIWallOfFlesh_Eye(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
				num301 = (num301 + (float)Main.WallOfFlesh_T) / 2f;
			else
				num301 = (num301 + (float)Main.WallOfFlesh_B) / 2f;

			num301 -= (float)(npc.Height / 2);
			if (npc.Position.Y > num301 + 1f)
			{
				npc.Velocity.Y = -1f;
			}
			else if (npc.Position.Y < num301 - 1f)
			{
				npc.Velocity.Y = 1f;
			}
			else
			{
				npc.Velocity.Y = 0f;
				npc.Position.Y = num301;
			}

			if (npc.Velocity.Y > 5f)
				npc.Velocity.Y = 5f;
			if (npc.Velocity.Y < -5f)
				npc.Velocity.Y = -5f;

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
			else if (Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) < npc.Position.X + (float)(npc.Width / 2))
			{
				npc.rotation = (float)Math.Atan2((double)num303, (double)num302) + 3.14f;
			}
			else
			{
				npc.rotation = 0f;
				flag33 = false;
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
			else if (npc.localAI[1] > 45f && Collision.CanHit(npc.Position, npc.Width, npc.Height, Main.players[npc.target].Position, Main.players[npc.target].Width, Main.players[npc.target].Height))
			{
				npc.localAI[1] = 0f;
				npc.localAI[2] += 1f;

				if (npc.localAI[2] >= (float)num305)
					npc.localAI[2] = 0f;

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

		// 29 - 1.1.2
		private void AITheHungry(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (npc.justHit)
			{
				npc.ai[1] = 10f;
			}
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
				{
					npc.ai[2] = 0f;
				}
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
					{
						npc.Velocity.X = npc.Velocity.X + num309 * 2.5f;
					}
				}
				else if (npc.Position.X > num311 + num314)
				{
					npc.Velocity.X = npc.Velocity.X - num309;
					if (npc.Velocity.X > 0f && num314 < 0f)
					{
						npc.Velocity.X = npc.Velocity.X - num309 * 2.5f;
					}
				}

				if (npc.Position.Y < num312 + num315)
				{
					npc.Velocity.Y = npc.Velocity.Y + num309;
					if (npc.Velocity.Y < 0f && num315 > 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y + num309 * 2.5f;
					}
				}
				else if (npc.Position.Y > num312 + num315)
				{
					npc.Velocity.Y = npc.Velocity.Y - num309;
					if (npc.Velocity.Y > 0f && num315 < 0f)
					{
						npc.Velocity.Y = npc.Velocity.Y - num309 * 2.5f;
					}
				}

				if (npc.Velocity.X > 4f)
				{
					npc.Velocity.X = 4f;
				}
				if (npc.Velocity.X < -4f)
				{
					npc.Velocity.X = -4f;
				}
				if (npc.Velocity.Y > 4f)
				{
					npc.Velocity.Y = 4f;
				}
				if (npc.Velocity.Y < -4f)
				{
					npc.Velocity.Y = -4f;
				}
			}
			else
			{
				if (npc.ai[1] > 0f)
				{
					npc.ai[1] -= 1f;
				}
				else
				{
					npc.ai[1] = 0f;
				}
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

		// 30 - 1.1.2
		private void AIRetinazer(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
			else if ((double)num319 > 6.283)
			{
				num319 -= 6.283f;
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
			else if (npc.rotation > num319)
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

			if (npc.rotation > num319 - num320 && npc.rotation < num319 + num320)
			{
				npc.rotation = num319;
			}
			if (npc.rotation < 0f)
			{
				npc.rotation += 6.283f;
			}
			else if ((double)npc.rotation > 6.283)
			{
				npc.rotation -= 6.283f;
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
						else if (npc.Velocity.X > num325)
						{
							npc.Velocity.X = npc.Velocity.X - num323;
							if (npc.Velocity.X > 0f && num325 < 0f)
							{
								npc.Velocity.X = npc.Velocity.X - num323;
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
						else if (npc.Velocity.Y > num326)
						{
							npc.Velocity.Y = npc.Velocity.Y - num323;
							if (npc.Velocity.Y > 0f && num326 < 0f)
							{
								npc.Velocity.Y = npc.Velocity.Y - num323;
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
						else if (npc.Position.Y + (float)npc.Height < Main.players[npc.target].Position.Y && num328 < 400f)
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
								{
									npc.Velocity.X = npc.Velocity.X + num339;
								}
							}
							else if (npc.Velocity.X > num340)
							{
								npc.Velocity.X = npc.Velocity.X - num339;
								if (npc.Velocity.X > 0f && num340 < 0f)
								{
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
							else if (npc.Velocity.Y > num341)
							{
								npc.Velocity.Y = npc.Velocity.Y - num339;
								if (npc.Velocity.Y > 0f && num341 < 0f)
								{
									npc.Velocity.Y = npc.Velocity.Y - num339;
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
							else if (npc.Velocity.X > num349)
							{
								npc.Velocity.X = npc.Velocity.X - num348;
								if (npc.Velocity.X > 0f && num349 < 0f)
								{
									npc.Velocity.X = npc.Velocity.X - num348;
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
							else if (npc.Velocity.Y > num350)
							{
								npc.Velocity.Y = npc.Velocity.Y - num348;
								if (npc.Velocity.Y > 0f && num350 < 0f)
								{
									npc.Velocity.Y = npc.Velocity.Y - num348;
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

		// 31 - 1.1.2
		private void AISpazmatism(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
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
			{
				num357 += 6.283f;
			}
			else
			{
				if ((double)num357 > 6.283)
				{
					num357 -= 6.283f;
				}
			}
			float num358 = 0.15f;
			if (npc.rotation < num357)
			{
				if ((double)(num357 - npc.rotation) > 3.1415)
				{
					npc.rotation -= num358;
				}
				else
				{
					npc.rotation += num358;
				}
			}
			else
			{
				if (npc.rotation > num357)
				{
					if ((double)(npc.rotation - num357) > 3.1415)
					{
						npc.rotation += num358;
					}
					else
					{
						npc.rotation -= num358;
					}
				}
			}
			if (npc.rotation > num357 - num358 && npc.rotation < num357 + num358)
			{
				npc.rotation = num357;
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
			if (npc.rotation > num357 - num358 && npc.rotation < num357 + num358)
			{
				npc.rotation = num357;
			}

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
						{
							num362 = -1;
						}
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
							{
								npc.Velocity.X = npc.Velocity.X + num361;
							}
						}
						else
						{
							if (npc.Velocity.X > num363)
							{
								npc.Velocity.X = npc.Velocity.X - num361;
								if (npc.Velocity.X > 0f && num363 < 0f)
								{
									npc.Velocity.X = npc.Velocity.X - num361;
								}
							}
						}
						if (npc.Velocity.Y < num364)
						{
							npc.Velocity.Y = npc.Velocity.Y + num361;
							if (npc.Velocity.Y < 0f && num364 > 0f)
							{
								npc.Velocity.Y = npc.Velocity.Y + num361;
							}
						}
						else
						{
							if (npc.Velocity.Y > num364)
							{
								npc.Velocity.Y = npc.Velocity.Y - num361;
								if (npc.Velocity.Y > 0f && num364 < 0f)
								{
									npc.Velocity.Y = npc.Velocity.Y - num361;
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
							if (!Main.players[npc.target].dead)
							{
								npc.ai[3] += 1f;
							}
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
								//if (Main.netMode != 1)
								{
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

		// 32 - 1.1.2
		private void AISkeletronPrime(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			npc.damage = npc.defDamage;
			npc.defense = npc.defDefense;
			if (npc.ai[0] == 0f)
			{
				npc.TargetClosest(true);
				npc.ai[0] = 1f;
				if (npc.type != NPCType.N68_DUNGEON_GUARDIAN)
				{
					int num388 = NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 128, npc.whoAmI);
					Main.npcs[num388].ai[0] = -1f;
					Main.npcs[num388].ai[1] = (float)npc.whoAmI;
					Main.npcs[num388].target = npc.target;
					Main.npcs[num388].netUpdate = true;
					num388 = NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 129, npc.whoAmI);
					Main.npcs[num388].ai[0] = 1f;
					Main.npcs[num388].ai[1] = (float)npc.whoAmI;
					Main.npcs[num388].target = npc.target;
					Main.npcs[num388].netUpdate = true;
					num388 = NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 130, npc.whoAmI);
					Main.npcs[num388].ai[0] = -1f;
					Main.npcs[num388].ai[1] = (float)npc.whoAmI;
					Main.npcs[num388].target = npc.target;
					Main.npcs[num388].ai[3] = 150f;
					Main.npcs[num388].netUpdate = true;
					num388 = NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)npc.Position.Y + npc.Height / 2, 131, npc.whoAmI);
					Main.npcs[num388].ai[0] = 1f;
					Main.npcs[num388].ai[1] = (float)npc.whoAmI;
					Main.npcs[num388].target = npc.target;
					Main.npcs[num388].netUpdate = true;
					Main.npcs[num388].ai[3] = 150f;
				}
			}
			if (npc.type == NPCType.N68_DUNGEON_GUARDIAN && npc.ai[1] != 3f && npc.ai[1] != 2f)
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
				else if (npc.Position.Y < Main.players[npc.target].Position.Y - 500f)
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
			else if (npc.ai[1] == 1f)
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

		// 33 - 1.1.2
		private void AIPrimeSaw(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			Vector2 vector42 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
			float num395 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector42.X;
			float num396 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector42.Y;
			float num397 = (float)Math.Sqrt((double)(num395 * num395 + num396 * num396));
			if (npc.ai[2] != 99f && num397 > 800f)
			{
				npc.ai[2] = 99f;
			}
			else if (num397 < 400f)
			{
				npc.ai[2] = 0f;
			}

			npc.spriteDirection = -(int)npc.ai[0];
			if (!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != 32)
			{
				npc.ai[2] += 10f;
				if (npc.ai[2] > 50f)
				{
					npc.life = -1;
					npc.HitEffect(0, 10.0);
					npc.Active = false;
				}
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
				else if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y)
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
			else if (npc.ai[2] == 0f || npc.ai[2] == 3f)
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
					else if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y + 260f)
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
			else if (npc.ai[2] == 2f)
			{
				if (npc.Position.Y > Main.players[npc.target].Position.Y || npc.Velocity.Y < 0f)
				{
					npc.ai[2] = 3f;
					return;
				}
			}
			else if (npc.ai[2] == 4f)
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

		// 34 - 1.1.2
		private void AIPrimeVice(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			npc.spriteDirection = -(int)npc.ai[0];
			Vector2 vector47 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f, npc.Position.Y + (float)npc.Height * 0.5f);
			float num409 = Main.npcs[(int)npc.ai[1]].Position.X + (float)(Main.npcs[(int)npc.ai[1]].Width / 2) - 200f * npc.ai[0] - vector47.X;
			float num410 = Main.npcs[(int)npc.ai[1]].Position.Y + 230f - vector47.Y;
			float num411 = (float)Math.Sqrt((double)(num409 * num409 + num410 * num410));

			if (npc.ai[2] != 99f && num411 > 800f)
			{
				npc.ai[2] = 99f;
			}
			else if (num411 < 400f)
			{
				npc.ai[2] = 0f;
			}

			if (!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != 32)
			{
				npc.ai[2] += 10f;
				if (npc.ai[2] > 50f)
				{
					npc.life = -1;
					npc.HitEffect(0, 10.0);
					npc.Active = false;
				}
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
				else if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y)
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
			else if (npc.ai[2] == 0f || npc.ai[2] == 3f)
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
					else if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y + 230f)
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
			else if (npc.ai[2] == 2f)
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
			else if (npc.ai[2] == 4f)
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
			else if (npc.ai[2] == 5f && npc.Position.X + (float)(npc.Width / 2) < Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - 100f)
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

		// 35 - 1.1.2
		private void AIPrimeCannon(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			npc.spriteDirection = -(int)npc.ai[0];
			if (!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != 32)
			{
				npc.ai[2] += 10f;
				if (npc.ai[2] > 50f)
				{
					npc.life = -1;
					npc.HitEffect(0, 10.0);
					npc.Active = false;
				}
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
					else if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y - 100f)
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
					else if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y - 150f)
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
			else if (npc.ai[2] == 1f)
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

		// 36 - 1.1.2
		private void AIPrimeLaser(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			npc.spriteDirection = -(int)npc.ai[0];
			if (!Main.npcs[(int)npc.ai[1]].Active || Main.npcs[(int)npc.ai[1]].aiStyle != 32)
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
					else if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y - 100f)
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
					else if (npc.Position.Y < Main.npcs[(int)npc.ai[1]].Position.Y - 100f)
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
			else if (npc.ai[2] == 1f)
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
			}
		}

		// 37 - 1.1.2
		private void AITheDestroyer(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			if (npc.ai[3] > 0f)
			{
				npc.realLife = (int)npc.ai[3];
			}
			if (npc.target < 0 || npc.target == 255 || Main.players[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			if (npc.type > NPCType.N134_THE_DESTROYER)
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
					npc.CheckDead();
				}
			}
			if (npc.ai[0] == 0f && npc.type == NPCType.N134_THE_DESTROYER)
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
					int num451 = NewNPC((int)(npc.Position.X + (float)(npc.Width / 2)), (int)(npc.Position.Y + (float)npc.Height), num450, npc.whoAmI, Main.stopSpawns);
					Main.npcs[num451].ai[3] = (float)npc.whoAmI;
					Main.npcs[num451].realLife = npc.whoAmI;
					Main.npcs[num451].ai[1] = (float)num447;
					Main.npcs[num447].ai[0] = (float)num451;
					NetMessage.SendData(23, -1, -1, "", num451, 0f, 0f, 0f, 0);
					num447 = num451;
				}
			}
			if (npc.type == NPCType.N135_THE_DESTROYER_BODY)
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
				if (npc.type == NPCType.N134_THE_DESTROYER)
				{
					Rectangle rectangle11 = new Rectangle((int)npc.Position.X, (int)npc.Position.Y, npc.Width, npc.Height);
					int num465 = 1000;
					bool flag36 = true;
					if (npc.Position.Y > Main.players[npc.target].Position.Y)
					{
						for (int num466 = 0; num466 < Main.MAX_PLAYERS; num466++)
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
				{ }

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
				else if (npc.Velocity.Y == num467)
				{
					if (npc.Velocity.X < num471)
					{
						npc.Velocity.X = npc.Velocity.X + num469;
					}
					else if (npc.Velocity.X > num471)
					{
						npc.Velocity.X = npc.Velocity.X - num469;
					}
				}
				else if (npc.Velocity.Y > 4f)
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
					else if (npc.Velocity.X > num471)
					{
						npc.Velocity.X = npc.Velocity.X - num470;
					}
					if (npc.Velocity.Y < num472)
					{
						npc.Velocity.Y = npc.Velocity.Y + num470;
					}
					else if (npc.Velocity.Y > num472)
					{
						npc.Velocity.Y = npc.Velocity.Y - num470;
					}
				}
				if ((npc.Velocity.X > 0f && num471 > 0f) || (npc.Velocity.X < 0f && num471 < 0f) || (npc.Velocity.Y > 0f && num472 > 0f) || (npc.Velocity.Y < 0f && num472 < 0f))
				{
					if (npc.Velocity.X < num471)
					{
						npc.Velocity.X = npc.Velocity.X + num469;
					}
					else if (npc.Velocity.X > num471)
					{
						npc.Velocity.X = npc.Velocity.X - num469;
					}
					if (npc.Velocity.Y < num472)
					{
						npc.Velocity.Y = npc.Velocity.Y + num469;
					}
					else if (npc.Velocity.Y > num472)
					{
						npc.Velocity.Y = npc.Velocity.Y - num469;
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
				else if (num476 > num477)
				{
					if (npc.Velocity.X < num471)
					{
						npc.Velocity.X = npc.Velocity.X + num469 * 1.1f;
					}
					else if (npc.Velocity.X > num471)
					{
						npc.Velocity.X = npc.Velocity.X - num469 * 1.1f;
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
					else if (npc.Velocity.Y > num472)
					{
						npc.Velocity.Y = npc.Velocity.Y - num469 * 1.1f;
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
			npc.rotation = (float)Math.Atan2((double)npc.Velocity.Y, (double)npc.Velocity.X) + 1.57f;
			if (npc.type == NPCType.N134_THE_DESTROYER)
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
					return;
				}
			}
		}

		// 38 - 1.1.2
		private void AISnow(NPC npc, bool flag, Func<Int32, Int32, ITile> TileRefs)
		{
			float num479 = 4f;
			float num480 = 1f;
			if (npc.type == NPCType.N143_SNOWMAN_GANGSTA)
			{
				num479 = 3f;
				num480 = 0.7f;
			}
			if (npc.type == NPCType.N145_SNOW_BALLA)
			{
				num479 = 3.5f;
				num480 = 0.8f;
			}
			if (npc.type == NPCType.N143_SNOWMAN_GANGSTA)
			{
				npc.ai[2] += 1f;
				if (npc.ai[2] >= 120f)
				{
					npc.ai[2] = 0f;

					Vector2 vector59 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f - (float)(npc.direction * 12), npc.Position.Y + (float)npc.Height * 0.5f);
					float speedX = (float)(12 * npc.spriteDirection);
					float speedY = 0f;

					int num481 = 25;
					int num482 = 110;
					int num483 = Projectile.NewProjectile(vector59.X, vector59.Y, speedX, speedY, num482, num481, 0f, Main.myPlayer);
					Main.projectile[num483].ai[0] = 2f;
					Main.projectile[num483].timeLeft = 300;
					Main.projectile[num483].friendly = false;
					NetMessage.SendData(27, -1, -1, "", num483, 0f, 0f, 0f, 0);
					npc.netUpdate = true;
				}
			}
			if (npc.type == NPCType.N144_MISTER_STABBY && npc.ai[1] >= 3f)
			{
				npc.TargetClosest(true);
				npc.spriteDirection = npc.direction;
				if (npc.Velocity.Y == 0f)
				{
					npc.Velocity.X = npc.Velocity.X * 0.9f;
					npc.ai[2] += 1f;
					if ((double)npc.Velocity.X > -0.3 && (double)npc.Velocity.X < 0.3)
					{
						npc.Velocity.X = 0f;
					}
					if (npc.ai[2] >= 200f)
					{
						npc.ai[2] = 0f;
						npc.ai[1] = 0f;
					}
				}
			}
			else
			{
				if (npc.type == NPCType.N145_SNOW_BALLA && npc.ai[1] >= 3f)
				{
					npc.TargetClosest(true);
					if (npc.Velocity.Y == 0f)
					{
						npc.Velocity.X = npc.Velocity.X * 0.9f;
						npc.ai[2] += 1f;
						if ((double)npc.Velocity.X > -0.3 && (double)npc.Velocity.X < 0.3)
						{
							npc.Velocity.X = 0f;
						}
						if (npc.ai[2] >= 16f)
						{
							npc.ai[2] = 0f;
							npc.ai[1] = 0f;
						}
					}
					if (npc.Velocity.X == 0f && npc.Velocity.Y == 0f && npc.ai[2] == 8f)
					{
						float num484 = 10f;
						Vector2 vector60 = new Vector2(npc.Position.X + (float)npc.Width * 0.5f - (float)(npc.direction * 12), npc.Position.Y + (float)npc.Height * 0.25f);
						float num485 = Main.players[npc.target].Position.X + (float)(Main.players[npc.target].Width / 2) - vector60.X;
						float num486 = Main.players[npc.target].Position.Y - vector60.Y;
						float num487 = (float)Math.Sqrt((double)(num485 * num485 + num486 * num486));
						num487 = num484 / num487;
						num485 *= num487;
						num486 *= num487;

						int num488 = 35;
						int num489 = 109;
						int num490 = Projectile.NewProjectile(vector60.X, vector60.Y, num485, num486, num489, num488, 0f, Main.myPlayer);
						Main.projectile[num490].ai[0] = 2f;
						Main.projectile[num490].timeLeft = 300;
						Main.projectile[num490].friendly = false;
						NetMessage.SendData(27, -1, -1, "", num490, 0f, 0f, 0f, 0);
						npc.netUpdate = true;
					}
				}
				else
				{
					if (npc.Velocity.Y == 0f)
					{
						if (npc.localAI[2] == npc.Position.X)
						{
							npc.direction *= -1;
							npc.ai[3] = 60f;
						}
						npc.localAI[2] = npc.Position.X;
						if (npc.ai[3] == 0f)
						{
							npc.TargetClosest(true);
						}
						npc.ai[0] += 1f;
						if (npc.ai[0] > 2f)
						{
							npc.ai[0] = 0f;
							npc.ai[1] += 1f;
							npc.Velocity.Y = -8.2f;
							npc.Velocity.X = npc.Velocity.X + (float)npc.direction * num480 * 1.1f;
						}
						else
						{
							npc.Velocity.Y = -6f;
							npc.Velocity.X = npc.Velocity.X + (float)npc.direction * num480 * 0.9f;
						}
						npc.spriteDirection = npc.direction;
					}
					npc.Velocity.X = npc.Velocity.X + (float)npc.direction * num480 * 0.01f;
				}
			}
			if (npc.ai[3] > 0f)
			{
				npc.ai[3] -= 1f;
			}
			if (npc.Velocity.X > num479 && npc.direction > 0)
			{
				npc.Velocity.X = 4f;
			}
			if (npc.Velocity.X < -num479 && npc.direction < 0)
			{
				npc.Velocity.X = -4f;
			}
		}
#endregion
		/// <summary>
		/// Checks if there are any active NPCs of specified type
		/// </summary>
		/// <param name="type">NPCType of the NPC to check for</param>
		/// <returns>True if active, false if not</returns>
		public static bool IsNPCSummoned(NPCType type)
		{
			return IsNPCSummoned((int)type);
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
				if (npc != null && npc.Active && npc.Type == Id)
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

		public static SpawnFlags SpawnWallOfFlesh(Func<Int32, Int32, ITile> TileRefs, Vector2 pos)
		{
			if (pos.Y / 16f < (float)(Main.maxTilesY - 205))
				return SpawnFlags.TILE_CONFLICT;

			if (Main.WallOfFlesh >= 0)
				return SpawnFlags.SUMMONED;

			//Player.FindClosest(pos, 16, 16);

			int num = 1;
			if (pos.X / 16f > (float)(Main.maxTilesX / 2))
				num = -1;

			bool flag = false;
			int num2 = (int)pos.X;

			while (!flag)
			{
				flag = true;

				for (int i = 0; i < 255; i++)
				{
					if (Main.players[i].Active && Main.players[i].Position.X > (float)(num2 - 1200) && Main.players[i].Position.X < (float)(num2 + 1200))
					{
						num2 -= num * 16;
						flag = false;
					}
				}

				if (num2 / 16 < 20 || num2 / 16 > Main.maxTilesX - 20)
					flag = true;
			}
			int num3 = (int)pos.Y;
			int num4 = num2 / 16;
			int num5 = num3 / 16;
			int num6 = 0;
			try
			{
				while (WorldModify.SolidTile(TileRefs, num4, num5 - num6) || TileRefs(num4, num5 - num6).Liquid >= 100)
				{
					if (!WorldModify.SolidTile(TileRefs, num4, num5 + num6) && TileRefs(num4, num5 + num6).Liquid < 100)
					{
						num5 += num6;
						break;
					}
					num6++;
				}
				num5 -= num6;
			}
			catch { }

			num3 = num5 * 16;
			int num7 = NewNPC(num2, num3, 113, 0);
			if (String.IsNullOrEmpty(Main.npcs[num7].DisplayName))
				Main.npcs[num7].DisplayName = Main.npcs[num7].Name;

			NetMessage.SendData(25, -1, -1, Main.npcs[num7].DisplayName + " has awoken!", 255, 175f, 75f, 255f, 0);
			return SpawnFlags.SUMMONED;
		}

		public void CheckDead()
		{
			if (!this.Active || this.realLife >= 0 && this.realLife != this.whoAmI)
				return;

			if (this.life <= 0)
			{
				noSpawnCycle = true;
				if (this.townNPC && this.type != NPCType.N37_OLD_MAN)
				{
					string str = this.Name;
					if (!String.IsNullOrEmpty(this.DisplayName))
						str = this.DisplayName;

					NetMessage.SendData(25, -1, -1, str + " was slain...", 255, 255f, 25f, 25f, 0);

					Main.chrName[(int)this.type] = String.Empty;
					SetNames();
					NetMessage.SendData(56, -1, -1, String.Empty, (int)this.type);
				}

				if (this.townNPC && this.homeless && WorldModify.spawnNPC == (int)this.type)
					WorldModify.spawnNPC = 0;

				this.NPCLoot();
				this.Active = false;

				if (this.type == NPCType.N26_GOBLIN_PEON || this.type == NPCType.N27_GOBLIN_THIEF || this.type == NPCType.N28_GOBLIN_WARRIOR ||
					this.type == NPCType.N29_GOBLIN_SORCERER || this.type == NPCType.N111_GOBLIN_ARCHER || this.type == NPCType.N143_SNOWMAN_GANGSTA ||
					this.type == NPCType.N144_MISTER_STABBY || this.type == NPCType.N145_SNOW_BALLA)
				{
					Main.invasionSize--;
				}

				if (type >= NPCType.N87_WYVERN_HEAD && type <= NPCType.N92_WYVERN_TAIL)
				{
					for (int i = 0; i < MAX_NPCS; i++)
					{
						if (Main.npcs[i].Active &&
							Main.npcs[i].type >= NPCType.N87_WYVERN_HEAD && Main.npcs[i].type <= NPCType.N92_WYVERN_TAIL)
						{
							Main.npcs[i].Kill();
						}
					}
				}
			}
		}

		public static void ClearNames()
		{
			for (int i = 0; i < Main.MAX_NPC_NAMES; i++)
				Main.chrName[i] = String.Empty;
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

		public static bool MechSpawn(float x, float y, int type)
		{
			int found = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < Main.npcs.Length; i++)
			{
				if (Main.npcs[i].Active && Main.npcs[i].Type == type)
				{
					found++;
					Vector2 vector = new Vector2(x, y);
					float _x = Main.npcs[i].Position.X - vector.X;
					float _y = Main.npcs[i].Position.Y - vector.Y;
					float _srt = (float)Math.Sqrt((double)(_x * _x + _y * _y));
					if (_srt < 200f)
						num2++;

					if (_srt < 600f)
						num3++;
				}
			}
			return num2 < 3 && num3 < 6 && found < 10;
		}

		// [TODO] 1.1
		public static bool downedClown { get; set; }
		public static bool downedGoblins { get; set; }
		public static bool savedMech { get; set; }
		public static bool savedWizard { get; set; }
		public static bool savedGoblin { get; set; }

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
			if (this.Type == 1 || this.Type == 16 || this.Type == 59 || this.Type == 71 || this.Type == 81 || this.Type == 138)
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
			if (this.Type == 141)
			{
				this.spriteDirection = this.direction;
				if (this.Velocity.Y != 0f)
				{
					this.frame.Y = num * 2;
				}
				else
				{
					this.frameCounter += 1.0;
					if (this.frameCounter >= 8.0)
					{
						this.frame.Y = this.frame.Y + num;
						this.frameCounter = 0.0;
					}
					if (this.frame.Y > num)
					{
						this.frame.Y = 0;
					}
				}
			}
			if (this.Type == 143)
			{
				if (this.Velocity.Y > 0f)
				{
					this.frameCounter += 1.0;
				}
				else
				{
					if (this.Velocity.Y < 0f)
					{
						this.frameCounter -= 1.0;
					}
				}
				if (this.frameCounter < 6.0)
				{
					this.frame.Y = num;
				}
				else
				{
					if (this.frameCounter < 12.0)
					{
						this.frame.Y = num * 2;
					}
					else
					{
						if (this.frameCounter < 18.0)
						{
							this.frame.Y = num * 3;
						}
					}
				}
				if (this.frameCounter < 0.0)
				{
					this.frameCounter = 0.0;
				}
				if (this.frameCounter > 17.0)
				{
					this.frameCounter = 17.0;
				}
			}
			if (this.Type == 144)
			{
				if (this.Velocity.X == 0f && this.Velocity.Y == 0f)
				{
					this.localAI[3] += 1f;
					if (this.localAI[3] < 6f)
					{
						this.frame.Y = 0;
					}
					else
					{
						if (this.localAI[3] < 12f)
						{
							this.frame.Y = num;
						}
					}
					if (this.localAI[3] >= 11f)
					{
						this.localAI[3] = 0f;
					}
				}
				else
				{
					if (this.Velocity.Y > 0f)
					{
						this.frameCounter += 1.0;
					}
					else
					{
						if (this.Velocity.Y < 0f)
						{
							this.frameCounter -= 1.0;
						}
					}
					if (this.frameCounter < 6.0)
					{
						this.frame.Y = num * 2;
					}
					else
					{
						if (this.frameCounter < 12.0)
						{
							this.frame.Y = num * 3;
						}
						else
						{
							if (this.frameCounter < 18.0)
							{
								this.frame.Y = num * 4;
							}
						}
					}
					if (this.frameCounter < 0.0)
					{
						this.frameCounter = 0.0;
					}
					if (this.frameCounter > 17.0)
					{
						this.frameCounter = 17.0;
					}
				}
			}
			if (this.Type == 145)
			{
				if (this.Velocity.X == 0f && this.Velocity.Y == 0f)
				{
					if (this.ai[2] < 4f)
					{
						this.frame.Y = 0;
					}
					else
					{
						if (this.ai[2] < 8f)
						{
							this.frame.Y = num;
						}
						else
						{
							if (this.ai[2] < 12f)
							{
								this.frame.Y = num * 2;
							}
							else
							{
								if (this.ai[2] < 16f)
								{
									this.frame.Y = num * 3;
								}
							}
						}
					}
				}
				else
				{
					if (this.Velocity.Y > 0f)
					{
						this.frameCounter += 1.0;
					}
					else
					{
						if (this.Velocity.Y < 0f)
						{
							this.frameCounter -= 1.0;
						}
					}
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
							if (this.frameCounter < 18.0)
							{
								this.frame.Y = num * 6;
							}
						}
					}
					if (this.frameCounter < 0.0)
					{
						this.frameCounter = 0.0;
					}
					if (this.frameCounter > 17.0)
					{
						this.frameCounter = 17.0;
					}
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
			if (this.Type == 135)
			{
				if (this.ai[2] == 0f)
				{
					this.frame.Y = 0;
				}
				else
				{
					this.frame.Y = num;
				}
			}
			if (this.Type == 85)
			{
				if (this.ai[0] == 0f)
				{
					this.frameCounter = 0.0;
					this.frame.Y = 0;
				}
				else
				{
					int num3 = 3;
					if (this.Velocity.Y == 0f)
					{
						this.frameCounter -= 1.0;
					}
					else
					{
						this.frameCounter += 1.0;
					}
					if (this.frameCounter < 0.0)
					{
						this.frameCounter = 0.0;
					}
					if (this.frameCounter > (double)(num3 * 4))
					{
						this.frameCounter = (double)(num3 * 4);
					}
					if (this.frameCounter < (double)num3)
					{
						this.frame.Y = num;
					}
					else
					{
						if (this.frameCounter < (double)(num3 * 2))
						{
							this.frame.Y = num * 2;
						}
						else
						{
							if (this.frameCounter < (double)(num3 * 3))
							{
								this.frame.Y = num * 3;
							}
							else
							{
								if (this.frameCounter < (double)(num3 * 4))
								{
									this.frame.Y = num * 4;
								}
								else
								{
									if (this.frameCounter < (double)(num3 * 5))
									{
										this.frame.Y = num * 5;
									}
									else
									{
										if (this.frameCounter < (double)(num3 * 6))
										{
											this.frame.Y = num * 4;
										}
										else
										{
											if (this.frameCounter < (double)(num3 * 7))
											{
												this.frame.Y = num * 3;
											}
											else
											{
												this.frame.Y = num * 2;
												if (this.frameCounter >= (double)(num3 * 8))
												{
													this.frameCounter = (double)num3;
												}
											}
										}
									}
								}
							}
						}
					}
				}
				if (this.ai[3] == 2f)
				{
					this.frame.Y = this.frame.Y + num * 6;
				}
				else
				{
					if (this.ai[3] == 3f)
					{
						this.frame.Y = this.frame.Y + num * 12;
					}
				}
			}
			if (this.Type == 113 || this.Type == 114)
			{
				if (this.ai[2] == 0f)
				{
					this.frameCounter += 1.0;
					if (this.frameCounter >= 12.0)
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
					this.frame.Y = 0;
					this.frameCounter = -60.0;
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
			if (this.Type == 122)
			{
				this.spriteDirection = this.direction;
				this.rotation = this.Velocity.X * 0.05f;
				if (this.ai[3] > 0f)
				{
					int num4 = (int)(this.ai[3] / 8f);
					this.frameCounter = 0.0;
					this.frame.Y = (num4 + 3) * num;
				}
				else
				{
					this.frameCounter += 1.0;
					if (this.frameCounter >= 8.0)
					{
						this.frame.Y = this.frame.Y + num;
						this.frameCounter = 0.0;
					}
					if (this.frame.Y >= num * 3)
					{
						this.frame.Y = 0;
					}
				}
			}
			if (this.Type == 74)
			{
				this.spriteDirection = this.direction;
				this.rotation = this.Velocity.X * 0.1f;
				if (this.Velocity.X == 0f && this.Velocity.Y == 0f)
				{
					this.frame.Y = num * 4;
					this.frameCounter = 0.0;
				}
				else
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
			if (this.Type == 63 || this.Type == 64 || this.Type == 103)
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
			if (this.Type == 2 || this.Type == 23 || this.Type == 121)
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
				else
				{
					if (this.Type == 2 || this.Type == 121)
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
			if (this.Type == 133)
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
				this.frameCounter += 1.0;
				if (this.frameCounter >= 8.0)
				{
					this.frame.Y = num;
				}
				else
				{
					this.frame.Y = 0;
				}
				if (this.frameCounter >= 16.0)
				{
					this.frame.Y = 0;
					this.frameCounter = 0.0;
				}
				if ((double)this.life < (double)this.lifeMax * 0.5)
				{
					this.frame.Y = this.frame.Y + num * 2;
				}
			}
			if (this.Type == 116)
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
				this.frameCounter += 1.0;
				if (this.frameCounter >= 5.0)
				{
					this.frame.Y = this.frame.Y + num;
					this.frameCounter = 0.0;
				}
				if (this.frame.Y >= num * Main.npcFrameCount[this.Type])
				{
					this.frame.Y = 0;
				}
			}
			if (this.Type == 75)
			{
				if (this.Velocity.X > 0f)
				{
					this.spriteDirection = 1;
				}
				else
				{
					this.spriteDirection = -1;
				}
				this.rotation = this.Velocity.X * 0.1f;
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
			if (this.Type == 55 || this.Type == 57 || this.Type == 58 || this.Type == 102)
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
			if (this.Type == 86)
			{
				if (this.Velocity.Y == 0f || this.wet)
				{
					if (this.Velocity.X < -2f)
					{
						this.spriteDirection = -1;
					}
					else
					{
						if (this.Velocity.X > 2f)
						{
							this.spriteDirection = 1;
						}
						else
						{
							this.spriteDirection = this.direction;
						}
					}
				}
				if (this.Velocity.Y != 0f)
				{
					this.frame.Y = num * 15;
					this.frameCounter = 0.0;
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
						if (Math.Abs(this.Velocity.X) < 3f)
						{
							this.frameCounter += (double)Math.Abs(this.Velocity.X);
							if (this.frameCounter >= 6.0)
							{
								this.frameCounter = 0.0;
								this.frame.Y = this.frame.Y + num;
								if (this.frame.Y / num >= 9)
								{
									this.frame.Y = num;
								}
								if (this.frame.Y / num <= 0)
								{
									this.frame.Y = num;
								}
							}
						}
						else
						{
							this.frameCounter += (double)Math.Abs(this.Velocity.X);
							if (this.frameCounter >= 10.0)
							{
								this.frameCounter = 0.0;
								this.frame.Y = this.frame.Y + num;
								if (this.frame.Y / num >= 15)
								{
									this.frame.Y = num * 9;
								}
								if (this.frame.Y / num <= 8)
								{
									this.frame.Y = num * 9;
								}
							}
						}
					}
				}
			}
			if (this.Type == 127)
			{
				if (this.ai[1] == 0f)
				{
					this.frameCounter += 1.0;
					if (this.frameCounter >= 12.0)
					{
						this.frameCounter = 0.0;
						this.frame.Y = this.frame.Y + num;
						if (this.frame.Y / num >= 2)
						{
							this.frame.Y = 0;
						}
					}
				}
				else
				{
					this.frameCounter = 0.0;
					this.frame.Y = num * 2;
				}
			}
			if (this.Type == 129)
			{
				if (this.Velocity.Y == 0f)
				{
					this.spriteDirection = this.direction;
				}
				this.frameCounter += 1.0;
				if (this.frameCounter >= 2.0)
				{
					this.frameCounter = 0.0;
					this.frame.Y = this.frame.Y + num;
					if (this.frame.Y / num >= Main.npcFrameCount[this.Type])
					{
						this.frame.Y = 0;
					}
				}
			}
			if (this.Type == 130)
			{
				if (this.Velocity.Y == 0f)
				{
					this.spriteDirection = this.direction;
				}
				this.frameCounter += 1.0;
				if (this.frameCounter >= 8.0)
				{
					this.frameCounter = 0.0;
					this.frame.Y = this.frame.Y + num;
					if (this.frame.Y / num >= Main.npcFrameCount[this.Type])
					{
						this.frame.Y = 0;
					}
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
			if (this.Type == 109)
			{
				if (this.Velocity.Y == 0f && ((this.Velocity.X <= 0f && this.direction < 0) || (this.Velocity.X >= 0f && this.direction > 0)))
				{
					this.spriteDirection = this.direction;
				}
				this.frameCounter += (double)Math.Abs(this.Velocity.X);
				if (this.frameCounter >= 7.0)
				{
					this.frameCounter -= 7.0;
					this.frame.Y = this.frame.Y + num;
					if (this.frame.Y / num >= Main.npcFrameCount[this.Type])
					{
						this.frame.Y = 0;
					}
				}
			}
			if (this.Type == 83 || this.Type == 84)
			{
				if (this.ai[0] == 2f)
				{
					this.frameCounter = 0.0;
					this.frame.Y = 0;
				}
				else
				{
					this.frameCounter += 1.0;
					if (this.frameCounter >= 4.0)
					{
						this.frameCounter = 0.0;
						this.frame.Y = this.frame.Y + num;
						if (this.frame.Y / num >= Main.npcFrameCount[this.Type])
						{
							this.frame.Y = 0;
						}
					}
				}
			}
			if (this.Type == 72)
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
			if (this.Type == 48 || this.Type == 49 || this.Type == 51 || this.Type == 60 || this.Type == 82 || this.Type == 93 || this.Type == 137)
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
			if (this.Type == 115)
			{
				this.frameCounter += 1.0;
				if (this.frameCounter < 3.0)
				{
					this.frame.Y = 0;
				}
				else
				{
					if (this.frameCounter < 6.0)
					{
						this.frame.Y = num;
					}
					else
					{
						if (this.frameCounter < 12.0)
						{
							this.frame.Y = num * 2;
						}
						else
						{
							if (this.frameCounter < 15.0)
							{
								this.frame.Y = num;
							}
						}
					}
				}
				if (this.frameCounter == 15.0)
				{
					this.frameCounter = 0.0;
				}
			}
			if (this.Type == 101)
			{
				this.frameCounter += 1.0;
				if (this.frameCounter > 6.0)
				{
					this.frame.Y = this.frame.Y + num * 2;
					this.frameCounter = 0.0;
				}
				if (this.frame.Y > num * 2)
				{
					this.frame.Y = 0;
				}
			}
			if (this.Type == 17 || this.Type == 18 || this.Type == 19 || this.Type == 20 || this.Type == 22 || this.Type == 142 || this.Type == 38 || this.Type == 26 || this.Type == 27 || this.Type == 28 || this.Type == 31 || this.Type == 21 || this.Type == 44 || this.Type == 54 || this.Type == 37 || this.Type == 73 || this.Type == 77 || this.Type == 78 || this.Type == 79 || this.Type == 80 || this.Type == 104 || this.Type == 107 || this.Type == 108 || this.Type == 120 || this.Type == 124 || this.Type == 140)
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
						if (this.Type == 140)
						{
							this.frame.Y = num;
							this.frameCounter = 0.0;
						}
						else
						{
							this.frame.Y = 0;
							this.frameCounter = 0.0;
						}
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
					if (this.Type == 21 || this.Type == 31 || this.Type == 44 || this.Type == 77 || this.Type == 78 || this.Type == 79 || this.Type == 80 || this.Type == 120 || this.Type == 140)
					{
						this.frame.Y = 0;
					}
				}
			}
			else
			{
				if (this.Type == 110)
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
						if (this.ai[2] > 0f)
						{
							this.spriteDirection = this.direction;
							this.frame.Y = num * (int)this.ai[2];
							this.frameCounter = 0.0;
						}
						else
						{
							if (this.frame.Y < num * 6)
							{
								this.frame.Y = num * 6;
							}
							this.frameCounter += (double)(Math.Abs(this.Velocity.X) * 2f);
							this.frameCounter += (double)this.Velocity.X;
							if (this.frameCounter > 6.0)
							{
								this.frame.Y = this.frame.Y + num;
								this.frameCounter = 0.0;
							}
							if (this.frame.Y / num >= Main.npcFrameCount[this.Type])
							{
								this.frame.Y = num * 6;
							}
						}
					}
					else
					{
						this.frameCounter = 0.0;
						this.frame.Y = 0;
					}
				}
			}
			if (this.Type == 111)
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
					if (this.ai[2] > 0f)
					{
						this.spriteDirection = this.direction;
						this.frame.Y = num * ((int)this.ai[2] - 1);
						this.frameCounter = 0.0;
					}
					else
					{
						if (this.frame.Y < num * 7)
						{
							this.frame.Y = num * 7;
						}
						this.frameCounter += (double)(Math.Abs(this.Velocity.X) * 2f);
						this.frameCounter += (double)(this.Velocity.X * 1.3f);
						if (this.frameCounter > 6.0)
						{
							this.frame.Y = this.frame.Y + num;
							this.frameCounter = 0.0;
						}
						if (this.frame.Y / num >= Main.npcFrameCount[this.Type])
						{
							this.frame.Y = num * 7;
						}
					}
				}
				else
				{
					this.frameCounter = 0.0;
					this.frame.Y = num * 6;
				}
			}
			else
			{
				if (this.Type == 3 || this.Type == 52 || this.Type == 53 || this.Type == 132)
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
						if (this.Type == 4 || this.Type == 125 || this.Type == 126)
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
								if (this.Type == 94)
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
												this.frame.Y = num;
												if (this.frameCounter >= 23.0)
												{
													this.frameCounter = 0.0;
												}
											}
										}
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

		public static int NumToType(int type)
		{
			switch (type)
			{
				case 2:
					return 17;
				case 3:
					return 18;
				case 6:
					return 19;
				case 5:
					return 20;
				case 1:
					return 22;
				case 4:
					return 38;
				case 7:
					return 54;
				case 9:
					return 107;
				case 10:
					return 108;
				case 8:
					return 124;
				case 11:
					return 142;
				default:
					return -1;
			}
		}

		public static int TypeToNum(int type)
		{
			switch (type)
			{
				case 17:
					return 2;
				case 18:
					return 3;
				case 19:
					return 6;
				case 20:
					return 5;
				case 22:
					return 1;
				case 38:
					return 4;
				case 54:
					return 7;
				case 107:
					return 9;
				case 108:
					return 10;
				case 124:
					return 8;
				case 142:
					return 11;
				default:
					return -1;
			}
		}

		//TODO 1.1.2
		public static bool downedFrost { get; set; }
	}

	public enum SpawnFlags
	{
		SUMMONED,
		TILE_CONFLICT,
		//EXISTING,
		//FAILED
	}
}

