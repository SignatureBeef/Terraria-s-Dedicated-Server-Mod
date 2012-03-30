using System;
using System.Collections.Generic;
using System.Net;

using Terraria_Server.Plugins;
using Terraria_Server.Misc;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;
using Terraria_Server.WorldMod;
using Terraria_Server.Logging;
using Terraria_Server.Networking;
using Terraria_Server.Language;

namespace Terraria_Server
{
	/// <summary>
	/// Player class.  Controls all player variables and interactions
	/// </summary>
	public class Player : BaseEntity, ISender
	{
		public const Int32 MAX_BUFF_TIME = 1200;
		public const Int32 MAX_INVENTORY = 49;
		public const Int32 MAX_HEALTH = 400;
		public const Int32 MAX_MANA = 360;
		public const Int32 MAX_ITEMS = 255;
		public const Int32 NUM_ARMOR_HEAD = 44;
		public const Int32 NUM_ARMOR_BODY = 25;
		public const Int32 NUM_ARMOR_LEGS = 24;

		public bool tongued { get; set; }

		/// <summary>
		/// Whether this player is using TDCM
		/// </summary>
		public bool HasClientMod = false;

		/// <summary>
		/// If using tRPG, This will be set with their quest NPC name
		/// </summary>
		public string QuestNPCName { get; set; }

		/// <summary>
		/// Contains the current quest of the player if using tRPG
		/// </summary>
		public int CurrentQuest { get; set; }

		/// <summary>
		/// String representation of this player's IP address
		/// </summary>
		private string ipAddress = null;

		public IPAddress IpAddress
		{
			get
			{
				return System.Net.IPAddress.Parse(ipAddress.Split(':')[0]);
			}
		}

		/// <summary>
		/// Whether to allow this player to destroy their bed or not
		/// </summary>
		private bool bedDestruction = false;

		public bool enemySpawns;
		public int heldProj = -1;
		public bool killGuide;
		/// <summary>
		/// Current buff types applied to the player
		/// </summary>
		public int[] buffType = new int[10];
		/// <summary>
		/// Time left of buffs applied to the player
		/// </summary>
		public int[] buffTime = new int[10];

		/// <summary>
		/// Whether the player is playing in hardcore mode
		/// </summary>
		[Obsolete("Replaced by difficulty setting in 1.0.6")]
		public bool hardCore;
		/// <summary>
		/// Whether last death was caused by pvp interaction
		/// </summary>
		public bool pvpDeath;
		/// <summary>
		/// Whether player is currently in a dungeon zone
		/// </summary>
		public bool zoneDungeon;
		/// <summary>
		/// Whether player is currently in a corruption zone
		/// </summary>
		public bool zoneEvil;
		/// <summary>
		/// Whether player is currently in a meteor zone
		/// </summary>
		public bool zoneMeteor;
		/// <summary>
		/// Whether player is currently in a jungle zone
		/// </summary>
		public bool zoneJungle;
		public bool zoneHoly;
		/// <summary>
		/// Whether player is wearing bone armor
		/// </summary>
		public bool boneArmor;
		public float TownNPCs;
		/// <summary>
		/// Current speed and direction
		/// </summary>
		public Vector2 Velocity;
		/// <summary>
		/// Speed and direction at last update
		/// </summary>
		public Vector2 oldVelocity;
		public double headFrameCounter;
		public double bodyFrameCounter;
		public double legFrameCounter;
		/// <summary>
		/// Whether player is currently immune to damage
		/// </summary>
		public bool immune;
		/// <summary>
		/// Immunity time left
		/// </summary>
		public int immuneTime;
		public int immuneAlphaDirection;
		public int immuneAlpha;
		/// <summary>
		/// Team player currently belongs to
		/// </summary>
		public int team;
		/// <summary>
		/// Player's currently entered chat text
		/// </summary>
		public string chatText = "";
		public int sign = -1;
		/// <summary>
		/// Time left for chat window visibility
		/// </summary>
		public int chatShowTime;
		public float ActiveNPCs;
		public bool mouseInterface;
		public int changeItem = -1;
		/// <summary>
		/// Currently held/selected item index
		/// </summary>
		public int selectedItemIndex;
		public Item[] armor = new Item[11];
		public int itemAnimation;
		public int itemAnimationMax;
		public int itemTime;
		public float itemRotation;
		public int itemWidth;
		public int itemHeight;
		public Vector2 itemLocation;
		public int breathCD;
		/// <summary>
		/// Maximum player breath
		/// </summary>
		public int breathMax = 200;
		/// <summary>
		/// Current breath left
		/// </summary>
		public int breath = 200;
		public string setBonus = "";
		/// <summary>
		/// Player inventory array
		/// </summary>
		public Item[] inventory = new Item[MAX_INVENTORY];
		/// <summary>
		/// Player's piggy bank array
		/// </summary>
		public Item[] bank = new Item[Chest.MAX_ITEMS];
		public float headRotation;
		public float bodyRotation;
		public float legRotation;
		public Vector2 headPosition;
		public Vector2 bodyPosition;
		public Vector2 legPosition;
		public Vector2 headVelocity;
		public Vector2 bodyVelocity;
		public Vector2 legVelocity;
		public bool dead;
		public int respawnTimer;
		public int attackCD;
		public int potionDelay;
		public bool wet;
		public byte wetCount;
		public bool lavaWet;
		public int hitTile;
		public int hitTileX;
		public int hitTileY;
		public int jump;
		public int head = -1;
		public int body = -1;
		public int legs = -1;
		public Rectangle headFrame = new Rectangle();
		public Rectangle bodyFrame = new Rectangle();
		public Rectangle legFrame = new Rectangle();
		public Rectangle hairFrame = new Rectangle();
		public bool controlLeft;
		public bool controlRight;
		public bool controlUp;
		public bool controlDown;
		public bool controlJump;
		public bool controlUseItem;
		public bool controlUseTile;
		public bool controlThrow;
		public bool controlInv;
		public bool releaseJump;
		public bool releaseUseItem;
		public bool releaseUseTile;
		public bool releaseInventory;
		public bool delayUseItem;
		public int direction = 1;
		public bool showItemIcon;
		public int showItemIcon2;
		public int whoAmi;
		public int runSoundDelay;
		public float shadow;
		public float manaCost = 1f;
		public bool fireWalk;
		public Vector2[] shadowPos = new Vector2[3];
		public int shadowCount;
		public bool channel;
		public int step = -1;
		public float meleeSpeed = 1f;
		public int statDefense;
		public int statAttack;
		public int statLifeMax = 100;
		public int statLife = 100;
		public int statMana;
		public int statManaMax;
		public int lifeRegen;
		public int lifeRegenCount;
		public int manaRegen;
		public int manaRegenCount;
		public int manaRegenDelay;
		public bool noKnockback;
		public bool spaceGun;
		public float magicBoost = 1f;
		public int SpawnX = -1;
		public int SpawnY = -1;
		// client-only
		//		public int[] spX = new int[200];
		//		public int[] spY = new int[200];
		//		public String[] spN = new String[200];
		//		public int[] spI = new int[200];
		public static int tileRangeX = 5;
		public static int tileRangeY = 4;
		//private static int tileTargetX = 0;
		//private static int tileTargetY = 0;
		private static int jumpHeight = 15;
		private static float jumpSpeed = 5.01f;
		//		public bool[] adjTile = new bool[107];
		//		public bool[] oldAdjTile = new bool[107];
		private static int itemGrabRange = 38;
		private static float itemGrabSpeed = 0.45f;
		private static float itemGrabSpeedMax = 4f;
		public Color hairColor = new Color(215, 90, 55);
		public Color skinColor = new Color(255, 125, 90);
		public Color eyeColor = new Color(105, 90, 75);
		public Color shirtColor = new Color(175, 165, 140);
		public Color underShirtColor = new Color(160, 180, 215);
		public Color pantsColor = new Color(255, 230, 175);
		public Color shoeColor = new Color(160, 105, 60);
		public int hair;
		public bool hostile;
		public int accWatch;
		public int accDepthMeter;
		public bool accFlipper;
		public bool doubleJump;
		public bool jumpAgain;
		public bool spawnMax;
		public int[] grappling = new int[20];
		public int grapCount;
		public int rocketDelay;
		public int rocketDelay2;
		public bool rocketRelease;
		public bool rocketFrame;
		public bool rocketBoots;
		public bool canRocket;
		public bool jumpBoost;
		public bool noFallDmg;
		public int swimTime;
		public int chest = -1;
		public int chestX;
		public int chestY;
		public int talkNPC = -1;
		public int fallStart;
		public int slowCount;

		public bool socialShadow { get; set; }

		/// <summary>
		/// Per-player plugin states, using object or name as key
		/// </summary>
		public readonly System.Collections.Hashtable PluginData;

		/// <summary>
		/// Account or character name player is authenticated as. Null if no authentication plugin is running or user is guest
		/// </summary>
		public string AuthenticatedAs { get; set; }

		public string DisconnectReason { get; set; }

		public ClientConnection Connection { get; internal set; }

		public bool Op { get; set; }

		public int OldSpawnX { get; set; }
		public int OldSpawnY { get; set; }

		public int TeleSpawnX { get; set; }
		public int TeleSpawnY { get; set; }
		public int TeleRetries { get; set; }

		// this is used for commands that cost a lot of cpu or bandwidth
		// to enforce a time period between uses
		public DateTime LastCostlyCommand { get; set; }

		/// <summary>
		/// Whether the player is male or not
		/// </summary>
		public bool Male { get; set; }
		public byte Difficulty { get; set; }

		public bool ghost { get; set; }
		public int ghostFrame { get; set; } // not sure if those are used by us
		public int ghostFrameCounter { get; set; }
		public bool hbLocked { get; set; }

		internal Dictionary<ushort, uint> rowsToRectify = new Dictionary<ushort, uint>();

		private float maxRegenDelay;

		/// <summary>
		/// Second piggy bank array
		/// </summary>
		public Item[] bank2 = new Item[Chest.MAX_ITEMS];
		/// <summary>
		/// Ammo slots array
		/// </summary>
		public Item[] ammo = new Item[4];

		public int meleeCrit { get; set; }
		public int rangedCrit { get; set; }
		public int magicCrit { get; set; }

		public int statManaMax2 { get; set; }
		public int lifeRegenTime { get; set; }
		public bool manaRegenBuff { get; set; }

		public bool ammoCost80 { get; set; }
		public int stickyBreak { get; set; }
		public bool archery { get; set; }
		public bool poisoned { get; set; }
		public bool blind { get; set; }
		public bool onFire { get; set; }
		public bool noItems { get; set; }

		public float meleeDamage { get; set; }
		public float rangedDamage { get; set; }
		public float magicDamage { get; set; }
		public float moveSpeed { get; set; }

		/// <summary>
		/// Whether player is using a light orb
		/// </summary>
		public bool lightOrb { get; set; }

		// [TODO] 1.1
		public bool fairy { get; set; }

		public float gravDir = 1f;

		public bool gills { get; set; }

		public bool gravControl { get; set; }

		public int rocketTimeMax = 7;

		public int rocketTime { get; set; }

		public bool slowFall { get; set; }

		public bool waterWalk { get; set; }

		public bool lavaImmune { get; set; }

		public bool findTreasure { get; set; }

		public bool invis { get; set; }

		public bool detectCreature { get; set; }

		public bool nightVision { get; set; }

		public bool thorns { get; set; }

		/// <summary>
		/// Player class constructor
		/// </summary>
		public Player()
		{
			Width = 20;
			Height = 42;

			PluginData = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());

			for (int i = 0; i < MAX_INVENTORY; i++)
			{
				if (i < 11)
				{
					this.armor[i] = new Item();
					this.armor[i].Name = "";
				}
				this.inventory[i] = new Item();
				this.inventory[i].Name = "";
			}
			for (int j = 0; j < Chest.MAX_ITEMS; j++)
			{
				this.bank[j] = new Item();
				this.bank[j].Name = "";
			}

			for (int j = 0; j < this.bank2.Length; j++)
			{
				this.bank2[j] = new Item();
				this.bank2[j].Name = "";
			}
			for (int j = 0; j < this.ammo.Length; j++)
			{
				this.ammo[j] = new Item();
				this.ammo[j].Name = "";
			}

			this.grappling[0] = -1;
			this.inventory[0] = Registries.Item.Create("Copper Pickaxe");
			this.inventory[1] = Registries.Item.Create("Copper Axe");
			//			for (int k = 0; k < 80; k++)
			//			{
			//				this.adjTile[k] = false;
			//				this.oldAdjTile[k] = false;
			//			}

			OldSpawnX = -1;
			OldSpawnY = -1;

			TeleSpawnX = -1;
			TeleSpawnY = -1;

			Male = true;
			Difficulty = 0;

			meleeCrit = 4;
			rangedCrit = 4;
			magicCrit = 4;

			meleeDamage = 1f;
			rangedDamage = 1f;
			magicDamage = 1f;
			meleeSpeed = 1f;
			moveSpeed = 1f;

			CurrentQuest = -1;
		}

		/// <summary>
		/// Sends player a message
		/// </summary>
		/// <param name="Message">Message string</param>
		/// <param name="A">Alpha color value</param>
		/// <param name="R">Red color value</param>
		/// <param name="G">Green color value</param>
		/// <param name="B">Blue color value</param>
		public void sendMessage(string Message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
		{
			NetMessage.SendData((int)Packet.PLAYER_CHAT, whoAmi, -1, Message, A, R, G, B);
		}

		/// <summary>
		/// Sends player a message with specified color instead of individual values
		/// </summary>
		/// <param name="Message">Message string</param>
		/// <param name="chatColour">Color to send message with</param>
		public void sendMessage(string Message, Color chatColour)
		{
			NetMessage.SendData((int)Packet.PLAYER_CHAT, whoAmi, -1, Message, 255, chatColour.R, chatColour.G, chatColour.B);
		}

		/// <summary>
		/// Heals specified player
		/// </summary>
		/// <param name="healAmount">Amount to heal</param>
		/// <param name="overrider">.</param>
		/// <param name="remoteClient">.</param>
		public void HealEffect(int healAmount, bool overrider = false, int remoteClient = -1)
		{
			if (overrider || (this.whoAmi == Main.myPlayer))
			{
				NetMessage.SendData(35, remoteClient, -1, "", this.whoAmi, (float)healAmount);
			}
		}

		/// <summary>
		/// Restores mana
		/// </summary>
		/// <param name="manaAmount">Amount to restore</param>
		/// <param name="overrider">.</param>
		/// <param name="remoteClient">.</param>
		public void ManaEffect(int manaAmount, bool overrider = false, int remoteClient = -1)
		{
			if (overrider || (this.whoAmi == Main.myPlayer))
			{
				NetMessage.SendData(43, remoteClient, -1, "", this.whoAmi, (float)manaAmount);
			}
		}

		/// <summary>
		/// Finds closest player to a position
		/// </summary>
		/// <param name="Position">Center of search area</param>
		/// <param name="Width">Width of search area</param>
		/// <param name="Height">Height of search area</param>
		/// <returns>Found player index</returns>
		public static byte FindClosest(Vector2 Position, int Width, int Height)
		{
			byte result = 0;
			for (int i = 0; i < 255; i++)
			{
				if (Main.players[i].Active)
				{
					result = (byte)i;
					break;
				}
			}
			float num = -1f;
			for (int j = 0; j < 255; j++)
			{
				if (Main.players[j].Active && !Main.players[j].dead && (num == -1f || Math.Abs(Main.players[j].Position.X + (float)(Main.players[j].Width / 2) - Position.X + (float)(Width / 2)) + Math.Abs(Main.players[j].Position.Y + (float)(Main.players[j].Height / 2) - Position.Y + (float)(Height / 2)) < num))
				{
					num = Math.Abs(Main.players[j].Position.X + (float)(Main.players[j].Width / 2) - Position.X + (float)(Width / 2)) + Math.Abs(Main.players[j].Position.Y + (float)(Main.players[j].Height / 2) - Position.Y + (float)(Height / 2));
					result = (byte)j;
				}
			}
			return result;
		}

		/// <summary>
		/// Adds buff to player
		/// </summary>
		/// <param name="type">Type of buff</param>
		/// <param name="time">Buff time</param>
		/// <param name="quiet">Whether to announce</param>
		public void AddBuff(int type, int time, bool quiet = true)
		{
			ProgramLog.Death.Log("Adding buff {0} to {1}.", type, Name);
			int num = -1;
			for (int i = 0; i < 10; i++)
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
				int num2 = -1;
				for (int j = 0; j < 10; j++)
				{
					if (!Main.debuff[this.buffType[j]])
					{
						num2 = j;
						break;
					}
				}
				if (num2 == -1)
				{
					return;
				}
				for (int k = num2; k < 10; k++)
				{
					if (this.buffType[k] == 0)
					{
						num = k;
						break;
					}
				}
				if (num == -1)
				{
					this.DelBuff(num2);
				}
			}
			this.buffType[num] = type;
			this.buffTime[num] = time;
		}

		/// <summary>
		/// Removes buff from player
		/// </summary>
		/// <param name="b">Name of buff to remove</param>
		public void DelBuff(int b)
		{
			this.buffTime[b] = 0;
			this.buffType[b] = 0;
			for (int i = 0; i < 9; i++)
			{
				if (this.buffTime[i] == 0 || this.buffType[i] == 0)
				{
					for (int j = i + 1; j < 10; j++)
					{
						this.buffTime[j - 1] = this.buffTime[j];
						this.buffType[j - 1] = this.buffType[j];
						this.buffTime[j] = 0;
						this.buffType[j] = 0;
					}
				}
			}
		}

		/// <summary>
		/// Add buff to npc
		/// </summary>
		/// <param name="type">Tyep of buff</param>
		/// <param name="npc">NPC to add to</param>
		public void StatusNPC(int type, NPC npc)
		{
			switch (type)
			{
				case 121:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(24, 180, false);
					return;
				case 122:
					if (Main.rand.Next(10) == 0)
						npc.AddBuff(24, 180, false);
					return;
				case 190:
					if (Main.rand.Next(4) == 0)
						npc.AddBuff(20, 420, false);
					return;
				case 217:
					if (Main.rand.Next(5) == 0)
						npc.AddBuff(24, 180, false);
					return;
			}
		}

		/// <summary>
		/// Add buff to other player
		/// </summary>
		/// <param name="type">Type of buff to add</param>
		/// <param name="player">Player to buff</param>
		public void StatusPvP(int type, Player player)
		{
			switch (type)
			{
				case 121:
					if (Main.rand.Next(2) == 0)
						player.AddBuff(24, 180, false);
					return;
				case 122:
					if (Main.rand.Next(10) == 0)
						player.AddBuff(24, 180, false);
					return;
				case 190:
					if (Main.rand.Next(4) == 0)
						player.AddBuff(20, 420, false);
					return;
				case 217:
					if (Main.rand.Next(5) == 0)
						player.AddBuff(24, 180, false);
					return;
			}
		}

		/// <summary>
		/// Creates ghost of player
		/// </summary>
		public void Ghost()
		{
			this.immune = false;
			this.immuneAlpha = 0;
			this.controlUp = false;
			this.controlLeft = false;
			this.controlDown = false;
			this.controlRight = false;
			this.controlJump = false;

			if (this.controlUp || this.controlJump)
			{
				if (this.Velocity.Y > 0f)
				{
					this.Velocity.Y = this.Velocity.Y * 0.9f;
				}
				this.Velocity.Y = this.Velocity.Y - 0.1f;
				if (this.Velocity.Y < -3f)
				{
					this.Velocity.Y = -3f;
				}
			}
			else if (this.controlDown)
			{
				if (this.Velocity.Y < 0f)
				{
					this.Velocity.Y = this.Velocity.Y * 0.9f;
				}
				this.Velocity.Y = this.Velocity.Y + 0.1f;
				if (this.Velocity.Y > 3f)
				{
					this.Velocity.Y = 3f;
				}
			}
			else if ((double)this.Velocity.Y < -0.1 || (double)this.Velocity.Y > 0.1)
			{
				this.Velocity.Y = this.Velocity.Y * 0.9f;
			}
			else
			{
				this.Velocity.Y = 0f;
			}

			if (this.controlLeft && !this.controlRight)
			{
				if (this.Velocity.X > 0f)
				{
					this.Velocity.X = this.Velocity.X * 0.9f;
				}
				this.Velocity.X = this.Velocity.X - 0.1f;
				if (this.Velocity.X < -3f)
				{
					this.Velocity.X = -3f;
				}
			}
			else if (this.controlRight && !this.controlLeft)
			{
				if (this.Velocity.X < 0f)
				{
					this.Velocity.X = this.Velocity.X * 0.9f;
				}
				this.Velocity.X = this.Velocity.X + 0.1f;
				if (this.Velocity.X > 3f)
				{
					this.Velocity.X = 3f;
				}
			}
			else if ((double)this.Velocity.X < -0.1 || (double)this.Velocity.X > 0.1)
			{
				this.Velocity.X = this.Velocity.X * 0.9f;
			}
			else
			{
				this.Velocity.X = 0f;
			}

			this.Position += this.Velocity;
			this.ghostFrameCounter++;
			if (this.Velocity.X < 0f)
			{
				this.direction = -1;
			}
			else
			{
				if (this.Velocity.X > 0f)
				{
					this.direction = 1;
				}
			}
			if (this.ghostFrameCounter >= 8)
			{
				this.ghostFrameCounter = 0;
				this.ghostFrame++;
				if (this.ghostFrame >= 4)
				{
					this.ghostFrame = 0;
				}
			}
			if (this.Position.X < Main.leftWorld + 336f + 16f)
			{
				this.Position.X = Main.leftWorld + 336f + 16f;
				this.Velocity.X = 0f;
			}
			if (this.Position.X + (float)this.Width > Main.rightWorld - 336f - 32f)
			{
				this.Position.X = Main.rightWorld - 336f - 32f - (float)this.Width;
				this.Velocity.X = 0f;
			}
			if (this.Position.Y < Main.topWorld + 336f + 16f)
			{
				this.Position.Y = Main.topWorld + 336f + 16f;
				if ((double)this.Velocity.Y < -0.1)
				{
					this.Velocity.Y = -0.1f;
				}
			}
			if (this.Position.Y > Main.bottomWorld - 336f - 32f - (float)this.Height)
			{
				this.Position.Y = Main.bottomWorld - 336f - 32f - (float)this.Height;
				this.Velocity.Y = 0f;
			}
		}

		// factored out of UpdatePlayer
		/// <summary>
		/// Applies buffs added to player
		/// </summary>
		public void ApplyBuffs()
		{
			for (int l = 0; l < 10; l++)
			{
				if (this.buffType[l] > 0 && this.buffTime[l] > 0)
				{
					if (this.whoAmi == Main.myPlayer)
					{
						this.buffTime[l]--;
					}

					switch (this.buffType[l])
					{
						case 1:
							this.lavaImmune = true;
							this.fireWalk = true;
							break;

						case 2:
							this.lifeRegen += 2;
							break;

						case 3:
							this.moveSpeed += 0.25f;
							break;

						case 4:
							this.gills = true;
							break;

						case 5:
							this.statDefense += 10;
							break;

						case 6:
							this.manaRegenBuff = true;
							break;

						case 7:
							this.magicDamage += 0.2f;
							break;

						case 8:
							this.slowFall = true;
							break;

						case 9:
							this.findTreasure = true;
							break;

						case 10:
							this.invis = true;
							break;

						case 11:
							//Lighting.addLight((int)(this.position.X + (float)(this.width / 2)) / 16, (int)(this.position.Y + (float)(this.height / 2)) / 16, 1f);
							break;

						case 12:
							this.nightVision = true;
							break;

						case 13:
							this.enemySpawns = true;
							break;

						case 14:
							this.thorns = true;
							break;

						case 15:
							this.waterWalk = true;
							break;

						case 16:
							this.archery = true;
							break;

						case 17:
							this.detectCreature = true;
							break;

						case 18:
							this.gravControl = true;
							break;

						case 19:
							this.lightOrb = true;

							// I'm sure the client sends us the projectile anyway
							//							bool flag4 = true;
							//							for (int m = 0; m < 1000; m++)
							//							{
							//								if (Main.projectile[m].Active && Main.projectile[m].Owner == this.whoAmi && Main.projectile[m].Type == 18)
							//								{
							//									flag4 = false;
							//									break;
							//								}
							//							}
							//							if (flag4)
							//							{
							//								Projectile.NewProjectile(this.Position.X + (float)(this.Width / 2), this.Position.Y + (float)(this.Height / 2), 0f, 0f, 18, 0, 0f, this.whoAmi);
							//							}
							break;

						case 20:
							this.poisoned = true;
							break;

						case 21:
							this.potionDelay = this.buffTime[l];
							break;

						case 22:
							this.blind = true;
							break;

						case 23:
							this.noItems = true;
							break;

						case 24:
							this.onFire = true;
							break;

						case 25:
							this.statDefense -= 4;
							this.meleeCrit += 2;
							this.meleeDamage += 0.1f;
							this.meleeSpeed += 0.1f;
							break;

						case 26:
							this.statDefense++;
							this.meleeCrit++;
							this.meleeDamage += 0.05f;
							this.meleeSpeed += 0.05f;
							this.magicCrit++;
							this.magicDamage += 0.05f;
							this.rangedCrit++;
							this.magicDamage += 0.05f;
							this.moveSpeed += 0.1f;
							break;

						case 28:
							//if (!Main.dayTime && Main.moonPhase == 0 && this.wolfAcc && !this.merman)
							//{
							//    this.wereWolf = true;
							//    this.meleeCrit++;
							//    this.meleeDamage += 0.051f;
							//    this.meleeSpeed += 0.051f;
							//    this.statDefense++;
							//    this.moveSpeed += 0.05f;
							//}
							//else
							//{
							//    this.DelBuff(l);
							//}
							break;

						case 29:
							magicCrit += 2;
							magicDamage += 0.05f;
							statManaMax2 += 20;
							manaCost -= 0.02f;
							break;

						case 33:
							meleeDamage -= 0.051f;
							meleeSpeed -= 0.051f;
							statDefense -= 4;
							moveSpeed -= 0.1f;
							break;

						case 39:
							//onFire2 = true;
							break;
					}
				}
			}
		}

		/// <summary>
		/// Update player values
		/// </summary>
		/// <param name="TileRefs">Reference to the ITile method, For usage between Sandbox and Realtime</param>
		/// <param name="i">Index of player to update</param>
		/// <param name="sandbox">Sandbox referenc, if needed</param>
		public void UpdatePlayer(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			try
			{
				float num = 10f;
				float num2 = 0.4f;
				Player.jumpHeight = 15;
				Player.jumpSpeed = 5.01f;
				if (this.wet)
				{
					num2 = 0.2f;
					num = 5f;
					Player.jumpHeight = 30;
					Player.jumpSpeed = 6.01f;
				}
				float num3 = 3f;
				float num4 = 0.08f;
				float num5 = 0.2f;
				float num6 = num3;
				if (this.Active)
				{
					this.maxRegenDelay = (1f - (float)this.statMana / (float)this.statManaMax2) * 60f * 4f + 45f;
					this.shadowCount++;
					if (this.shadowCount == 1)
					{
						this.shadowPos[2] = this.shadowPos[1];
					}
					else
					{
						if (this.shadowCount == 2)
						{
							this.shadowPos[1] = this.shadowPos[0];
						}
						else
						{
							if (this.shadowCount >= 3)
							{
								this.shadowCount = 0;
								this.shadowPos[0] = this.Position;
							}
						}
					}
					this.whoAmi = i;
					if (this.runSoundDelay > 0)
					{
						this.runSoundDelay--;
					}
					if (this.attackCD > 0)
					{
						this.attackCD--;
					}
					if (this.itemAnimation == 0)
					{
						this.attackCD = 0;
					}
					if (this.chatShowTime > 0)
					{
						this.chatShowTime--;
					}
					if (this.potionDelay > 0)
					{
						this.potionDelay--;
					}

					if (this.ghost)
					{
						this.Ghost();
						return;
					}

					if (!this.dead)
					{
						// client-only, not updated to 1.0.6
						//                        if (i == Main.myPlayer)
						//                        {
						//                            this.controlUp = false;
						//                            this.controlLeft = false;
						//                            this.controlDown = false;
						//                            this.controlRight = false;
						//                            this.controlJump = false;
						//                            this.controlUseItem = false;
						//                            this.controlUseTile = false;
						//                            this.controlThrow = false;
						//                            this.controlInv = false;
						//
						//                            if (Main.playerInventory)
						//                            {
						//                                this.AdjTiles();
						//                            }
						//                            if (this.chest != -1)
						//                            {
						//                                int num12 = (int)(((double)this.Position.X + (double)this.Width * 0.5) / 16.0);
						//                                int num13 = (int)(((double)this.Position.Y + (double)this.Height * 0.5) / 16.0);
						//                                if (num12 < this.chestX - 5 || num12 > this.chestX + 6 || num13 < this.chestY - 4 || num13 > this.chestY + 5)
						//                                {
						//                                    this.chest = -1;
						//                                }
						//                                if (!TileRefs(this.chestX, this.chestY).Active)
						//                                {
						//                                    this.chest = -1;
						//                                }
						//                            }
						//                            if (this.Velocity.Y == 0f)
						//                            {
						//                                int num14 = (int)(this.Position.Y / 16f) - this.fallStart;
						//                                if (num14 > 25 && !this.noFallDmg)
						//                                {
						//                                    int damage = (num14 - 25) * 10;
						//                                    this.immune = false;
						//                                    this.Hurt(damage, -this.direction, false, false);
						//                                }
						//                                this.fallStart = (int)(this.Position.Y / 16f);
						//                            }
						//                            if (this.Velocity.Y < 0f || this.rocketDelay > 0 || this.wet || this.slowFall)
						//                            {
						//                                this.fallStart = (int)(this.Position.Y / 16f);
						//                            }
						//                        }
						//                        if (this.mouseInterface)
						//                        {
						//                            this.delayUseItem = true;
						//                        }
						if (this.immune)
						{
							this.immuneTime--;
							if (this.immuneTime <= 0)
							{
								this.immune = false;
							}
							this.immuneAlpha += this.immuneAlphaDirection * 50;
							if (this.immuneAlpha <= 50)
							{
								this.immuneAlphaDirection = 1;
							}
							else
							{
								if (this.immuneAlpha >= 205)
								{
									this.immuneAlphaDirection = -1;
								}
							}
						}
						else
						{
							this.immuneAlpha = 0;
						}
						this.statDefense = 0;
						this.accWatch = 0;
						this.accDepthMeter = 0;
						this.lifeRegen = 0;
						this.manaCost = 1f;
						this.meleeSpeed = 1f; this.meleeDamage = 1f;
						this.meleeDamage = 1f;
						this.rangedDamage = 1f;
						this.magicDamage = 1f;
						this.moveSpeed = 1f;
						this.boneArmor = false;
						this.rocketBoots = false;
						this.fireWalk = false;
						this.noKnockback = false;
						this.jumpBoost = false;
						this.noFallDmg = false;
						this.accFlipper = false;
						this.spawnMax = false;
						this.spaceGun = false;
						this.statManaMax2 = this.statManaMax;
						this.ammoCost80 = false;
						this.manaRegenBuff = false;
						this.meleeCrit = 4;
						this.rangedCrit = 4;
						this.magicCrit = 4;
						this.lightOrb = false;
						this.archery = false;
						this.poisoned = false;
						this.blind = false;
						this.onFire = false;
						this.noItems = false;

						//1.06?
						this.gills = false;
						this.gravControl = false;
						this.slowFall = false;
						this.waterWalk = false;
						this.lavaImmune = false;
						this.findTreasure = false;
						this.invis = false;
						this.detectCreature = false;
						this.nightVision = false;
						this.enemySpawns = false;
						this.thorns = false;

						this.ApplyBuffs();

						if (this.whoAmi == Main.myPlayer)
						{
							for (int n = 0; n < 10; n++)
							{
								if (this.buffType[n] > 0 && this.buffTime[n] <= 0)
								{
									this.DelBuff(n);
								}
							}
						}

						if (this.manaRegenDelay > 0 && !this.channel)
						{
							this.manaRegenDelay--;
							if ((this.Velocity.X == 0f && this.Velocity.Y == 0f) || this.grappling[0] >= 0 || this.manaRegenBuff)
							{
								this.manaRegenDelay--;
							}
						}
						if (this.manaRegenBuff && this.manaRegenDelay > 20)
						{
							this.manaRegenDelay = 20;
						}
						if (this.manaRegenDelay <= 0)
						{
							this.manaRegenDelay = 0;
							this.manaRegen = this.statManaMax2 / 7 + 1;
							if ((this.Velocity.X == 0f && this.Velocity.Y == 0f) || this.grappling[0] >= 0 || this.manaRegenBuff)
							{
								this.manaRegen += this.statManaMax2 / 2;
							}
							float num14 = (float)this.statMana / (float)this.statManaMax2 * 0.8f + 0.2f;
							if (this.manaRegenBuff)
							{
								num14 = 1f;
							}
							this.manaRegen = (int)((float)this.manaRegen * num14);
						}
						else
						{
							this.manaRegen = 0;
						}
						this.doubleJump = false;
						for (int a = 0; a < 8; a++)
						{
							this.statDefense += this.armor[a].defense;
							this.lifeRegen += this.armor[a].LifeRegen;
							if (this.armor[a].Type == 193)
							{
								this.fireWalk = true;
							}
							if (this.armor[a].Type == 238)
							{
								this.magicDamage += 0.15f;
							}
							if (this.armor[a].Type == 123 || this.armor[a].Type == 124 || this.armor[a].Type == 125)
							{
								this.magicDamage += 0.05f;
							}
							if (this.armor[a].Type == 151 || this.armor[a].Type == 152 || this.armor[a].Type == 153)
							{
								this.rangedDamage += 0.05f;
							}
							if (this.armor[a].Type == 111 || this.armor[a].Type == 228 || this.armor[a].Type == 229 || this.armor[a].Type == 230)
							{
								this.statManaMax2 += 20;
							}
							if (this.armor[a].Type == 228 || this.armor[a].Type == 229 || this.armor[a].Type == 230)
							{
								this.magicCrit += 3;
							}
							if (this.armor[a].Type == 100 || this.armor[a].Type == 101 || this.armor[a].Type == 102)
							{
								this.meleeSpeed += 0.07f;
							}
						}
						this.head = this.armor[0].HeadSlot;
						this.body = this.armor[1].BodySlot;
						this.legs = this.armor[2].LegSlot;
						for (int m = 3; m < 8; m++)
						{
							if (this.armor[m].Type == 15 && this.accWatch < 1)
							{
								this.accWatch = 1;
							}
							if (this.armor[m].Type == 16 && this.accWatch < 2)
							{
								this.accWatch = 2;
							}
							if (this.armor[m].Type == 17 && this.accWatch < 3)
							{
								this.accWatch = 3;
							}
							if (this.armor[m].Type == 18 && this.accDepthMeter < 1)
							{
								this.accDepthMeter = 1;
							}
							if (this.armor[m].Type == 53)
							{
								this.doubleJump = true;
							}
							if (this.armor[m].Type == 54)
							{
								num6 = 6f;
							}
							if (this.armor[m].Type == 128)
							{
								this.rocketBoots = true;
							}
							if (this.armor[m].Type == 156)
							{
								this.noKnockback = true;
							}
							if (this.armor[m].Type == 158)
							{
								this.noFallDmg = true;
							}
							if (this.armor[m].Type == 159)
							{
								this.jumpBoost = true;
							}
							if (this.armor[m].Type == 187)
							{
								this.accFlipper = true;
							}
							if (this.armor[m].Type == 211)
							{
								this.meleeSpeed += 0.12f;
							}
							if (this.armor[m].Type == 223)
							{
								this.manaCost -= 0.06f;
							}
							if (this.armor[m].Type == 285)
							{
								this.moveSpeed += 0.05f;
							}
							if (this.armor[m].Type == 212)
							{
								this.moveSpeed += 0.1f;
							}
							if (this.armor[m].Type == 267)
							{
								this.killGuide = true;
							}
						}
						if (this.head == 11)
						{
							int i2 = (int)(this.Position.X + (float)(this.Width / 2) + (float)(8 * this.direction)) / 16;
							int j2 = (int)(this.Position.Y + 2f) / 16;
						}
						this.setBonus = "";
						if ((this.head == 1 && this.body == 1 && this.legs == 1) || (this.head == 2 && this.body == 2 && this.legs == 2))
						{
							this.setBonus = "2 defense";
							this.statDefense += 2;
						}
						if ((this.head == 3 && this.body == 3 && this.legs == 3) || (this.head == 4 && this.body == 4 && this.legs == 4))
						{
							this.setBonus = "3 defense";
							this.statDefense += 3;
						}
						if (this.head == 5 && this.body == 5 && this.legs == 5)
						{
							this.setBonus = "15 % increased movement speed";
							this.moveSpeed += 0.15f;
						}
						if (this.head == 6 && this.body == 6 && this.legs == 6)
						{
							this.setBonus = "Space Gun costs 0 mana";
							this.spaceGun = true;
						}
						if (this.head == 7 && this.body == 7 && this.legs == 7)
						{
							this.setBonus = "20% chance to not consume ammo";
							this.ammoCost80 = true;
						}
						if (this.head == 8 && this.body == 8 && this.legs == 8)
						{
							this.setBonus = "16% reduced mana usage";
							this.manaCost -= 0.16f;
						}
						if (this.head == 9 && this.body == 9 && this.legs == 9)
						{
							this.setBonus = "17% extra melee damage";
							this.meleeDamage += 0.17f;
						}
						if (this.meleeSpeed > 4f)
						{
							this.meleeSpeed = 4f;
						}
						if ((double)this.moveSpeed > 1.4)
						{
							this.moveSpeed = 1.4f;
						}
						if (this.statManaMax2 > 400)
						{
							this.statManaMax2 = 400;
						}
						if (this.statDefense < 0)
						{
							this.statDefense = 0;
						}
						this.meleeSpeed = 1f / this.meleeSpeed;
						if (this.poisoned)
						{
							this.lifeRegenTime = 0;
							this.lifeRegen = -4;
						}
						if (this.onFire)
						{
							this.lifeRegenTime = 0;
							this.lifeRegen = -8;
						}
						this.lifeRegenTime++;
						float num17 = 0f;
						if (this.lifeRegenTime >= 300)
						{
							num17 += 1f;
						}
						if (this.lifeRegenTime >= 600)
						{
							num17 += 1f;
						}
						if (this.lifeRegenTime >= 900)
						{
							num17 += 1f;
						}
						if (this.lifeRegenTime >= 1200)
						{
							num17 += 1f;
						}
						if (this.lifeRegenTime >= 1500)
						{
							num17 += 1f;
						}
						if (this.lifeRegenTime >= 1800)
						{
							num17 += 1f;
						}
						if (this.lifeRegenTime >= 2400)
						{
							num17 += 1f;
						}
						if (this.lifeRegenTime >= 3000)
						{
							num17 += 1f;
						}
						if (this.lifeRegenTime >= 3600)
						{
							num17 += 1f;
							this.lifeRegenTime = 3600;
						}
						if (this.Velocity.X == 0f || this.grappling[0] > 0)
						{
							num17 *= 1.25f;
						}
						else
						{
							num17 *= 0.5f;
						}
						float num18 = (float)this.statLifeMax / 400f * 0.75f + 0.25f;
						num17 *= num18;
						this.lifeRegen += (int)Math.Round((double)num17);
						this.lifeRegenCount += this.lifeRegen;
						while (this.lifeRegenCount >= 120)
						{
							this.lifeRegenCount -= 120;
							if (this.statLife < this.statLifeMax)
							{
								this.statLife++;
							}
							if (this.statLife > this.statLifeMax)
							{
								this.statLife = this.statLifeMax;
							}
						}

						while (this.lifeRegenCount <= -120)
						{
							this.lifeRegenCount += 120;
							this.statLife--;
#if CLIENT_CODE
							if (this.statLife <= 0)
							{
								// I think this is meant to be client-side
								// all other calls to KillMe are client initiated
								// The stock server does it, but it might be causing problems

								if (this.poisoned)
								{
									this.KillMe(10.0, 0, false, " couldn't find the antidote");
								}
								else
								{
									if (this.onFire)
									{
										this.KillMe(10.0, 0, false, " couldn't put the fire out");
									}
								}
							}
#else
							if (statLife < 0) statLife = 0;
#endif //CLIENT_CODE
						}
						this.manaRegenCount += this.manaRegen;
						while (this.manaRegenCount >= 120)
						{
							this.manaRegenCount -= 120;
							if (this.statMana < this.statManaMax2)
							{
								this.statMana++;
							}
							if (this.statMana >= this.statManaMax2)
							{
								this.statMana = this.statManaMax2;
							}
						}
						if (this.manaRegenCount < 0)
						{
							this.manaRegenCount = 0;
						}
						num4 *= this.moveSpeed;
						num3 *= this.moveSpeed;
						if (this.jumpBoost)
						{
							Player.jumpHeight = 20;
							Player.jumpSpeed = 6.51f;
						}
						if (!this.doubleJump)
						{
							this.jumpAgain = false;
						}
						else
						{
							if (this.Velocity.Y == 0f)
							{
								this.jumpAgain = true;
							}
						}

						Item selectedItem = inventory[selectedItemIndex];
						if (this.grappling[0] == -1)
						{
							if (this.controlLeft && this.Velocity.X > -num3)
							{
								if (this.Velocity.X > num5)
								{
									this.Velocity.X = this.Velocity.X - num5;
								}
								this.Velocity.X = this.Velocity.X - num4;
								if (this.itemAnimation == 0 || this.inventory[this.selectedItemIndex].UseTurn)
								{
									this.direction = -1;
								}
							}
							else
							{
								if (this.controlRight && this.Velocity.X < num3)
								{
									if (this.Velocity.X < -num5)
									{
										this.Velocity.X = this.Velocity.X + num5;
									}
									this.Velocity.X = this.Velocity.X + num4;
									if (this.itemAnimation == 0 || this.inventory[this.selectedItemIndex].UseTurn)
									{
										this.direction = 1;
									}
								}
								else
								{
									if (this.controlLeft && this.Velocity.X > -num6)
									{
										if (this.itemAnimation == 0 || this.inventory[this.selectedItemIndex].UseTurn)
										{
											this.direction = -1;
										}
										if (this.Velocity.Y == 0f)
										{
											if (this.Velocity.X > num5)
											{
												this.Velocity.X = this.Velocity.X - num5;
											}
											this.Velocity.X = this.Velocity.X - num4 * 0.2f;
										}
										if (this.Velocity.X < -(num6 + num3) / 2f && this.Velocity.Y == 0f)
										{
											int num21 = 0;
											if (this.gravDir == -1f)
											{
												num21 -= this.Height;
											}
											if (this.runSoundDelay == 0 && this.Velocity.Y == 0f)
											{
												this.runSoundDelay = 9;
											}
										}
									}
									else
									{
										if (this.controlRight && this.Velocity.X < num6)
										{
											if (this.itemAnimation == 0 || this.inventory[this.selectedItemIndex].UseTurn)
											{
												this.direction = 1;
											}
											if (this.Velocity.Y == 0f)
											{
												if (this.Velocity.X < -num5)
												{
													this.Velocity.X = this.Velocity.X + num5;
												}
												this.Velocity.X = this.Velocity.X + num4 * 0.2f;
											}
											if (this.Velocity.X > (num6 + num3) / 2f && this.Velocity.Y == 0f)
											{
												int num23 = 0;
												if (this.gravDir == -1f)
												{
													num23 -= this.Height;
												}
												if (this.runSoundDelay == 0 && this.Velocity.Y == 0f)
												{
													this.runSoundDelay = 9;
												}
											}
										}
										else
										{
											if (this.Velocity.Y == 0f)
											{
												if (this.Velocity.X > num5)
												{
													this.Velocity.X = this.Velocity.X - num5;
												}
												else
												{
													if (this.Velocity.X < -num5)
													{
														this.Velocity.X = this.Velocity.X + num5;
													}
													else
													{
														this.Velocity.X = 0f;
													}
												}
											}
											else
											{
												if ((double)this.Velocity.X > (double)num5 * 0.5)
												{
													this.Velocity.X = this.Velocity.X - num5 * 0.5f;
												}
												else
												{
													if ((double)this.Velocity.X < (double)(-(double)num5) * 0.5)
													{
														this.Velocity.X = this.Velocity.X + num5 * 0.5f;
													}
													else
													{
														this.Velocity.X = 0f;
													}
												}
											}
										}
									}
								}
							}
							if (this.gravControl)
							{
								if (this.controlUp && this.gravDir == 1f)
								{
									this.gravDir = -1f;
									this.fallStart = (int)(this.Position.Y / 16f);
									this.jump = 0;
								}
								if (this.controlDown && this.gravDir == -1f)
								{
									this.gravDir = 1f;
									this.fallStart = (int)(this.Position.Y / 16f);
									this.jump = 0;
								}
							}
							else
							{
								this.gravDir = 1f;
							}
							if (this.controlJump)
							{
								if (this.jump > 0)
								{
									if (this.Velocity.Y == 0f)
									{
										this.jump = 0;
									}
									else
									{
										this.Velocity.Y = -Player.jumpSpeed * this.gravDir;
										this.jump--;
									}
								}
								else
								{
									if ((this.Velocity.Y == 0f || this.jumpAgain || (this.wet && this.accFlipper)) && this.releaseJump)
									{
										bool flag6 = false;
										if (this.wet && this.accFlipper)
										{
											if (this.swimTime == 0)
											{
												this.swimTime = 30;
											}
											flag6 = true;
										}
										this.jumpAgain = false;
										this.canRocket = false;
										this.rocketRelease = false;
										if (this.Velocity.Y == 0f && this.doubleJump)
										{
											this.jumpAgain = true;
										}
										if (this.Velocity.Y == 0f || flag6)
										{
											this.Velocity.Y = -Player.jumpSpeed * this.gravDir;
											this.jump = Player.jumpHeight;
										}
										else
										{
											int num25 = this.Height;
											if (this.gravDir == -1f)
											{
												num25 = 0;
											}
											this.Velocity.Y = -Player.jumpSpeed * this.gravDir;
											this.jump = Player.jumpHeight / 2;
										}
									}
								}
								this.releaseJump = false;
							}
							else
							{
								this.jump = 0;
								this.releaseJump = true;
								this.rocketRelease = true;
							}
							if (this.doubleJump && !this.jumpAgain && ((this.gravDir == 1f && this.Velocity.Y < 0f) ||
								(this.gravDir == -1f && this.Velocity.Y > 0f)) && !this.rocketBoots && !this.accFlipper)
							{
								int num29 = this.Height;
								if (this.gravDir == -1f)
								{
									num29 = -6;
								}
							}
							if (((this.gravDir == 1f && this.Velocity.Y > -Player.jumpSpeed) || (this.gravDir == -1f && this.Velocity.Y < Player.jumpSpeed)) && this.Velocity.Y != 0f)
							{
								this.canRocket = true;
							}
							if (this.Velocity.Y == 0f)
							{
								this.rocketTime = this.rocketTimeMax;
							}
							if (this.rocketBoots && this.controlJump && this.rocketDelay == 0 && this.canRocket && this.rocketRelease && !this.jumpAgain)
							{
								if (this.rocketTime > 0)
								{
									this.rocketTime--;
									this.rocketDelay = 10;
									if (this.rocketDelay2 <= 0)
									{
										this.rocketDelay2 = 30;
									}
								}
								else
								{
									this.canRocket = false;
								}
							}
							if (this.rocketDelay2 > 0)
							{
								this.rocketDelay2--;
							}
							if (this.rocketDelay == 0)
							{
								this.rocketFrame = false;
							}
							if (this.rocketDelay > 0)
							{
								int num31 = this.Height;
								if (this.gravDir == -1f)
								{
									num31 = 4;
								}
								this.rocketFrame = true;
								if (this.rocketDelay == 0)
								{
									this.releaseJump = true;
								}
								this.rocketDelay--;
								this.Velocity.Y = this.Velocity.Y - 0.1f * this.gravDir;
								if (this.gravDir == 1f)
								{
									if (this.Velocity.Y > 0f)
									{
										this.Velocity.Y = this.Velocity.Y - 0.5f;
									}
									else
									{
										if ((double)this.Velocity.Y > (double)(-(double)Player.jumpSpeed) * 0.5)
										{
											this.Velocity.Y = this.Velocity.Y - 0.1f;
										}
									}
									if (this.Velocity.Y < -Player.jumpSpeed * 1.5f)
									{
										this.Velocity.Y = -Player.jumpSpeed * 1.5f;
									}
								}
								else
								{
									if (this.Velocity.Y < 0f)
									{
										this.Velocity.Y = this.Velocity.Y + 0.5f;
									}
									else
									{
										if ((double)this.Velocity.Y < (double)Player.jumpSpeed * 0.5)
										{
											this.Velocity.Y = this.Velocity.Y + 0.1f;
										}
									}
									if (this.Velocity.Y > Player.jumpSpeed * 1.5f)
									{
										this.Velocity.Y = Player.jumpSpeed * 1.5f;
									}
								}
							}
							else
							{
								if (this.slowFall && ((!this.controlDown && this.gravDir == 1f) || (!this.controlUp && this.gravDir == -1f)))
								{
									if ((this.controlUp && this.gravDir == 1f) || (this.controlDown && this.gravDir == -1f))
									{
										this.Velocity.Y = this.Velocity.Y + num2 / 10f * this.gravDir;
									}
									else
									{
										this.Velocity.Y = this.Velocity.Y + num2 / 3f * this.gravDir;
									}
								}
								else
								{
									this.Velocity.Y = this.Velocity.Y + num2 * this.gravDir;
								}
							}
							if (this.gravDir == 1f)
							{
								if (this.Velocity.Y > num)
								{
									this.Velocity.Y = num;
								}
								if (this.slowFall && this.Velocity.Y > num / 3f && !this.controlDown)
								{
									this.Velocity.Y = num / 3f;
								}
								if (this.slowFall && this.Velocity.Y > num / 5f && this.controlUp)
								{
									this.Velocity.Y = num / 10f;
								}
							}
							else
							{
								if (this.Velocity.Y < -num)
								{
									this.Velocity.Y = -num;
								}
								if (this.slowFall && this.Velocity.Y < -num / 3f && !this.controlUp)
								{
									this.Velocity.Y = -num / 3f;
								}
								if (this.slowFall && this.Velocity.Y < -num / 5f && this.controlDown)
								{
									this.Velocity.Y = -num / 10f;
								}
							}
						}
						for (int num29 = 0; num29 < 200; num29++)
						{
							if (Main.item[num29].Active && Main.item[num29].NoGrabDelay == 0 && Main.item[num29].Owner == i)
							{
								Rectangle rectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);
								if (rectangle.Intersects(new Rectangle((int)Main.item[num29].Position.X, (int)Main.item[num29].Position.Y, Main.item[num29].Width, Main.item[num29].Height)))
								{
									if (i == Main.myPlayer && (selectedItem.Type != 0 || this.itemAnimation <= 0))
									{
										if (Main.item[num29].Type == 58)
										{
											this.statLife += 20;
											if (Main.myPlayer == this.whoAmi)
											{
												this.HealEffect(20);
											}
											if (this.statLife > this.statLifeMax)
											{
												this.statLife = this.statLifeMax;
											}
											Main.item[num29] = new Item();
										}
										else
										{
											if (Main.item[num29].Type == 184)
											{
												this.statMana += 20;
												if (Main.myPlayer == this.whoAmi)
												{
													this.ManaEffect(20);
												}
												if (this.statMana > this.statManaMax)
												{
													this.statMana = this.statManaMax;
												}
												Main.item[num29] = new Item();
											}
											else
											{
												Main.item[num29] = this.GetItem(i, Main.item[num29]);
											}
										}
									}
								}
								else
								{
									rectangle = new Rectangle((int)this.Position.X - Player.itemGrabRange, (int)this.Position.Y - Player.itemGrabRange, this.Width + Player.itemGrabRange * 2, this.Height + Player.itemGrabRange * 2);
									if (rectangle.Intersects(new Rectangle((int)Main.item[num29].Position.X, (int)Main.item[num29].Position.Y, Main.item[num29].Width, Main.item[num29].Height)) && this.ItemSpace(Main.item[num29]))
									{
										Main.item[num29].BeingGrabbed = true;
										if ((double)this.Position.X + (double)this.Width * 0.5 > (double)Main.item[num29].Position.X + (double)Main.item[num29].Width * 0.5)
										{
											if (Main.item[num29].Velocity.X < Player.itemGrabSpeedMax + this.Velocity.X)
											{
												Item expr_2C5D_cp_0 = Main.item[num29];
												expr_2C5D_cp_0.Velocity.X = expr_2C5D_cp_0.Velocity.X + Player.itemGrabSpeed;
											}
											if (Main.item[num29].Velocity.X < 0f)
											{
												Item expr_2C97_cp_0 = Main.item[num29];
												expr_2C97_cp_0.Velocity.X = expr_2C97_cp_0.Velocity.X + Player.itemGrabSpeed * 0.75f;
											}
										}
										else
										{
											if (Main.item[num29].Velocity.X > -Player.itemGrabSpeedMax + this.Velocity.X)
											{
												Item expr_2CE6_cp_0 = Main.item[num29];
												expr_2CE6_cp_0.Velocity.X = expr_2CE6_cp_0.Velocity.X - Player.itemGrabSpeed;
											}
											if (Main.item[num29].Velocity.X > 0f)
											{
												Item expr_2D1D_cp_0 = Main.item[num29];
												expr_2D1D_cp_0.Velocity.X = expr_2D1D_cp_0.Velocity.X - Player.itemGrabSpeed * 0.75f;
											}
										}
										if ((double)this.Position.Y + (double)this.Height * 0.5 > (double)Main.item[num29].Position.Y + (double)Main.item[num29].Height * 0.5)
										{
											if (Main.item[num29].Velocity.Y < Player.itemGrabSpeedMax)
											{
												Item expr_2DA6_cp_0 = Main.item[num29];
												expr_2DA6_cp_0.Velocity.Y = expr_2DA6_cp_0.Velocity.Y + Player.itemGrabSpeed;
											}
											if (Main.item[num29].Velocity.Y < 0f)
											{
												Item expr_2DE0_cp_0 = Main.item[num29];
												expr_2DE0_cp_0.Velocity.Y = expr_2DE0_cp_0.Velocity.Y + Player.itemGrabSpeed * 0.75f;
											}
										}
										else
										{
											if (Main.item[num29].Velocity.Y > -Player.itemGrabSpeedMax)
											{
												Item expr_2E20_cp_0 = Main.item[num29];
												expr_2E20_cp_0.Velocity.Y = expr_2E20_cp_0.Velocity.Y - Player.itemGrabSpeed;
											}
											if (Main.item[num29].Velocity.Y > 0f)
											{
												Item expr_2E57_cp_0 = Main.item[num29];
												expr_2E57_cp_0.Velocity.Y = expr_2E57_cp_0.Velocity.Y - Player.itemGrabSpeed * 0.75f;
											}
										}
									}
								}
							}
						}

						// this depends on having tileTargetX/tileTargetY, not updated to 1.0.6
						//                        if (this.Position.X / 16f - (float)Player.tileRangeX <= (float)Player.tileTargetX && (this.Position.X + (float)this.Width) / 16f + (float)Player.tileRangeX - 1f >= (float)Player.tileTargetX && this.Position.Y / 16f - (float)Player.tileRangeY <= (float)Player.tileTargetY && (this.Position.Y + (float)this.Height) / 16f + (float)Player.tileRangeY - 2f >= (float)Player.tileTargetY && TileRefs(Player.tileTargetX, Player.tileTargetY).Active)
						//                        {
						//                            if (this.controlUseTile)
						//                            {
						//                                if (this.releaseUseTile)
						//                                {
						//                                    if (TileRefs(Player.tileTargetX, Player.tileTargetY).Type == 4 || TileRefs(Player.tileTargetX, Player.tileTargetY).Type == 13 || TileRefs(Player.tileTargetX, Player.tileTargetY).Type == 33 || TileRefs(Player.tileTargetX, Player.tileTargetY).Type == 49 || (TileRefs(Player.tileTargetX, Player.tileTargetY).Type == 50 && TileRefs(Player.tileTargetX, Player.tileTargetY).FrameX == 90))
						//                                    {
						//                                        WorldModify.KillTile(TileRefs, sandbox, Player.tileTargetX, Player.tileTargetY, false, false, false);
						//                                    }
						//                                    else
						//                                    {
						//                                        if (TileRefs(Player.tileTargetX, Player.tileTargetY).Type == 79)
						//                                        {
						//                                            int num34 = Player.tileTargetX;
						//                                            int num35 = Player.tileTargetY;
						//                                            num34 += (int)(TileRefs(Player.tileTargetX, Player.tileTargetY).FrameX / 18 * -1);
						//                                            if (TileRefs(Player.tileTargetX, Player.tileTargetY).FrameX >= 72)
						//                                            {
						//                                                num34 += 4;
						//                                                num34++;
						//                                            }
						//                                            else
						//                                            {
						//                                                num34 += 2;
						//                                            }
						//                                            num35 += (int)(TileRefs(Player.tileTargetX, Player.tileTargetY).FrameY / 18 * -1);
						//                                            num35 += 2;
						//                                            if (Player.CheckSpawn(num34, num35))
						//                                            {
						//                                                this.ChangeSpawn(num34, num35);
						//                                            }
						//                                        }
						//                                        else
						//                                        {
						//                                            if (TileRefs(Player.tileTargetX, Player.tileTargetY).Type == 55)
						//                                            {
						//                                                bool flag4 = true;
						//                                                if (this.sign >= 0)
						//                                                {
						//                                                    int num36 = Sign.ReadSign(Player.tileTargetX, Player.tileTargetY);
						//                                                    if (num36 == this.sign)
						//                                                    {
						//                                                        this.sign = -1;
						//                                                        Main.npcChatText = "";
						//                                                        Main.editSign = false;
						//                                                        flag4 = false;
						//                                                    }
						//                                                }
						//                                                if (flag4)
						//                                                {
						//                                                    int num38 = (int)(TileRefs(Player.tileTargetX, Player.tileTargetY).FrameX / 18);
						//                                                    int num39 = (int)(TileRefs(Player.tileTargetX, Player.tileTargetY).FrameY / 18);
						//                                                    while (num38 > 1)
						//                                                    {
						//                                                        num38 -= 2;
						//                                                    }
						//                                                    int num40 = Player.tileTargetX - num38;
						//                                                    int num41 = Player.tileTargetY - num39;
						//                                                    if (TileRefs(num40, num41).Type == 55)
						//                                                    {
						//                                                        NetMessage.SendData(46, -1, -1, "", num40, (float)num41);
						//                                                    }
						//                                                }
						//                                            }
						//                                            else
						//                                            {
						//                                                if (TileRefs(Player.tileTargetX, Player.tileTargetY).Type == 10)
						//                                                {
						//                                                    WorldModify.OpenDoor(TileRefs, Player.tileTargetX, Player.tileTargetY, this.direction);
						//                                                    NetMessage.SendData(19, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, (float)this.direction);
						//                                                }
						//                                                else
						//                                                {
						//                                                    if (TileRefs(Player.tileTargetX, Player.tileTargetY).Type == 11)
						//                                                    {
						//                                                        if (WorldModify.CloseDoor(TileRefs, Player.tileTargetX, Player.tileTargetY, false))
						//                                                        {
						//                                                            NetMessage.SendData(19, -1, -1, "", 1, (float)Player.tileTargetX, (float)Player.tileTargetY, (float)this.direction);
						//                                                        }
						//                                                    }
						//                                                    else
						//                                                    {
						//                                                        if ((TileRefs(Player.tileTargetX, Player.tileTargetY).Type == 21 || TileRefs(Player.tileTargetX, Player.tileTargetY).Type == 29) && this.talkNPC == -1)
						//                                                        {
						//                                                            bool flag5 = false;
						//                                                            int num42 = Player.tileTargetX - (int)(TileRefs(Player.tileTargetX, Player.tileTargetY).FrameX / 18);
						//                                                            int num43 = Player.tileTargetY - (int)(TileRefs(Player.tileTargetX, Player.tileTargetY).FrameY / 18);
						//                                                            if (TileRefs(Player.tileTargetX, Player.tileTargetY).Type == 29)
						//                                                            {
						//                                                                flag5 = true;
						//                                                            }
						//                                                            int num44 = -1;
						//                                                            if (flag5)
						//                                                            {
						//                                                                num44 = -2;
						//                                                            }
						//                                                            else
						//                                                            {
						//                                                                num44 = Chest.FindChest(num42, num43);
						//                                                            }
						//                                                            if (num44 != -1)
						//                                                            {
						//                                                                if (num44 == this.chest)
						//                                                                {
						//                                                                    this.chest = -1;
						//                                                                }
						//                                                                else
						//                                                                {
						//                                                                    if (num44 != this.chest && this.chest == -1)
						//                                                                    {
						//                                                                        this.chest = num44;
						//                                                                        Main.playerInventory = true;
						//                                                                        this.chestX = num42;
						//                                                                        this.chestY = num43;
						//                                                                    }
						//                                                                    else
						//                                                                    {
						//                                                                        this.chest = num44;
						//                                                                        Main.playerInventory = true;
						//                                                                        this.chestX = num42;
						//                                                                        this.chestY = num43;
						//                                                                    }
						//                                                                }
						//                                                            }
						//                                                        }
						//                                                    }
						//                                                }
						//                                            }
						//                                        }
						//                                    }
						//                                }
						//                                this.releaseUseTile = false;
						//                            }
						//                            else
						//                            {
						//                                this.releaseUseTile = true;
						//                            }
						//                        }

						// client-only
						//                        if (Main.myPlayer == this.whoAmi)
						//                        {
						//                            if (this.talkNPC >= 0)
						//                            {
						//                                Rectangle rectangle2 = new Rectangle((int)(this.Position.X + (float)(this.Width / 2) - (float)(Player.tileRangeX * 16)), (int)(this.Position.Y + (float)(this.Height / 2) - (float)(Player.tileRangeY * 16)), Player.tileRangeX * 16 * 2, Player.tileRangeY * 16 * 2);
						//                                Rectangle value = new Rectangle((int)Main.npcs[this.talkNPC].Position.X, (int)Main.npcs[this.talkNPC].Position.Y, Main.npcs[this.talkNPC].Width, Main.npcs[this.talkNPC].Height);
						//                                if (!rectangle2.Intersects(value) || this.chest != -1 || !Main.npcs[this.talkNPC].Active)
						//                                {
						//                                    this.talkNPC = -1;
						//                                    Main.npcChatText = "";
						//                                }
						//                            }
						//                            if (this.sign >= 0)
						//                            {
						//                                Rectangle rectangle3 = new Rectangle((int)(this.Position.X + (float)(this.Width / 2) - (float)(Player.tileRangeX * 16)), (int)(this.Position.Y + (float)(this.Height / 2) - (float)(Player.tileRangeY * 16)), Player.tileRangeX * 16 * 2, Player.tileRangeY * 16 * 2);
						//                                Rectangle value2 = new Rectangle(Main.sign[this.sign].x * 16, Main.sign[this.sign].y * 16, 32, 32);
						//                                if (!rectangle3.Intersects(value2))
						//                                {
						//                                    this.sign = -1;
						//                                    Main.editSign = false;
						//                                    Main.npcChatText = "";
						//                                }
						//                            }
						//                            if (Main.editSign)
						//                            {
						//                                if (this.sign == -1)
						//                                {
						//                                    Main.editSign = false;
						//                                }
						//                            }
						// looks like players being hurt by npcs, might need that sometime
						//                            Rectangle rectangle4 = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);
						//                            for (int num45 = 0; num45 < NPC.MAX_NPCS; num45++)
						//                            {
						//                                if (Main.npcs[num45].Active && !Main.npcs[num45].friendly && rectangle4.Intersects(new Rectangle((int)Main.npcs[num45].Position.X, (int)Main.npcs[num45].Position.Y, Main.npcs[num45].Width, Main.npcs[num45].Height)))
						//                                {
						//                                    int hitDirection = -1;
						//                                    if (Main.npcs[num45].Position.X + (float)(Main.npcs[num45].Width / 2) < this.Position.X + (float)(this.Width / 2))
						//                                    {
						//                                        hitDirection = 1;
						//                                    }
						//                                    this.Hurt(Main.npcs[num45].damage, hitDirection, false, false);
						//                                }
						//                            }
						//                            Vector2 vector = Collision.HurtTiles(this.Position, this.Velocity, this.Width, this.Height, this.fireWalk);
						//                            if (vector.Y != 0f)
						//                            {
						//                                this.Hurt((int)vector.Y, (int)vector.X, false, false);
						//                            }
						//                        }
						if (this.grappling[0] >= 0)
						{
							this.rocketTime = this.rocketTimeMax;
							this.rocketDelay = 0;
							this.rocketFrame = false;
							this.canRocket = false;
							this.rocketRelease = false;
							this.fallStart = (int)(this.Position.Y / 16f);
							float num46 = 0f;
							float num47 = 0f;
							for (int num48 = 0; num48 < this.grapCount; num48++)
							{
								num46 += Main.projectile[this.grappling[num48]].Position.X + (float)(Main.projectile[this.grappling[num48]].Width / 2);
								num47 += Main.projectile[this.grappling[num48]].Position.Y + (float)(Main.projectile[this.grappling[num48]].Height / 2);
							}
							num46 /= (float)this.grapCount;
							num47 /= (float)this.grapCount;
							Vector2 vector2 = new Vector2(this.Position.X + (float)this.Width * 0.5f, this.Position.Y + (float)this.Height * 0.5f);
							float num49 = num46 - vector2.X;
							float num50 = num47 - vector2.Y;
							float num51 = (float)Math.Sqrt((double)(num49 * num49 + num50 * num50));
							float num52 = 11f;
							float num53 = num51;
							if (num51 > num52)
							{
								num53 = num52 / num51;
							}
							else
							{
								num53 = 1f;
							}
							num49 *= num53;
							num50 *= num53;
							this.Velocity.X = num49;
							this.Velocity.Y = num50;
							if (this.itemAnimation == 0)
							{
								if (this.Velocity.X > 0f)
								{
									this.direction = 1;
								}
								if (this.Velocity.X < 0f)
								{
									this.direction = -1;
								}
							}
							if (this.controlJump)
							{
								if (this.releaseJump)
								{
									if (this.Velocity.Y == 0f || (this.wet && (double)this.Velocity.Y > -0.02 && (double)this.Velocity.Y < 0.02))
									{
										this.Velocity.Y = -Player.jumpSpeed;
										this.jump = Player.jumpHeight / 2;
										this.releaseJump = false;
									}
									else
									{
										this.Velocity.Y = this.Velocity.Y + 0.01f;
										this.releaseJump = false;
									}
									if (this.doubleJump)
									{
										this.jumpAgain = true;
									}
									this.grappling[0] = 0;
									this.grapCount = 0;
									for (int num54 = 0; num54 < 1000; num54++)
									{
										if (Main.projectile[num54].Active && Main.projectile[num54].Owner == i && Main.projectile[num54].aiStyle == 7)
										{
											Main.projectile[num54].Kill(TileRefs, sandbox);
										}
									}
								}
							}
							else
							{
								this.releaseJump = true;
							}
						}
						if (Collision.StickyTiles(this.Position, this.Velocity, this.Width, this.Height))
						{
							//							if (this.whoAmi == Main.myPlayer && (this.Velocity.X != 0f || this.Velocity.Y != 0f))
							//							{
							//								this.stickyBreak++;
							//								if (this.stickyBreak > Main.rand.Next(20, 100))
							//								{
							//									this.stickyBreak = 0;
							//									int num75 = (int)vector3.X;
							//									int num76 = (int)vector3.Y;
							//									WorldGen.KillTile(TileRefs, sandbox, num75, num76, false, false, false);
							//									if (Main.netMode == 1 && !Main.tile[num75, num76].active && Main.netMode == 1)
							//									{
							//										NetMessage.SendData(17, -1, -1, "", 0, (float)num75, (float)num76, 0f, 0);
							//									}
							//								}
							//							}
							this.fallStart = (int)(this.Position.Y / 16f);
							this.jump = 0;
							if (this.Velocity.X > 1f)
							{
								this.Velocity.X = 1f;
							}
							if (this.Velocity.X < -1f)
							{
								this.Velocity.X = -1f;
							}
							if (this.Velocity.Y > 1f)
							{
								this.Velocity.Y = 1f;
							}
							if (this.Velocity.Y < -5f)
							{
								this.Velocity.Y = -5f;
							}
							if ((double)this.Velocity.X > 0.75 || (double)this.Velocity.X < -0.75)
							{
								this.Velocity.X = this.Velocity.X * 0.85f;
							}
							else
							{
								this.Velocity.X = this.Velocity.X * 0.6f;
							}
							if (this.Velocity.Y < 0f)
							{
								this.Velocity.Y = this.Velocity.Y * 0.96f;
							}
							else
							{
								this.Velocity.Y = this.Velocity.Y * 0.3f;
							}
						}
						else
						{
							this.stickyBreak = 0;
						}
						bool flag9 = Collision.DrownCollision(this.Position, this.Width, this.Height); //FIXME: add gravDir
						if (this.armor[0].Type == 250)
						{
							flag9 = true;
						}
						if (selectedItem.Type == 186)
						{
							try
							{
								int num77 = (int)((this.Position.X + (float)(this.Width / 2) + (float)(6 * this.direction)) / 16f);
								int num78 = 0;
								if (this.gravDir == -1f)
								{
									num78 = this.Height;
								}
								int num79 = (int)((this.Position.Y + (float)num78 - 44f * this.gravDir) / 16f);
								if (TileRefs(num77, num79).Liquid < 128)
								{
									if (!TileRefs(num77, num79).Active || !Main.tileSolid[(int)TileRefs(num77, num79).Type] || Main.tileSolidTop[(int)TileRefs(num77, num79).Type])
									{
										flag9 = false;
									}
								}
							}
							catch
							{
							}
						}
						if (this.gills)
							flag9 = !flag9;

						if (flag9 && Main.rand.Next(20) == 0 && !this.lavaWet)
						{
							int num81 = 0;
							if (this.gravDir == -1f)
							{
								num81 += this.Height - 12;
							}
						}
						int num82 = this.Height;
						if (this.waterWalk)
						{
							num82 -= 6;
						}
						bool flag10 = Collision.LavaCollision(this.Position, this.Width, num82);
						if (flag10)
						{
#if CLIENT_CODE
							if (!this.lavaImmune && Main.myPlayer == i && !this.immune)
							{
								this.AddBuff(24, 420, true);
								this.Hurt(80, 0, false, false, Player.getDeathMessage(-1, -1, -1, 2), false);
							}
#endif //CLIENT_CODE
							this.lavaWet = true;
						}
						bool flag8 = Collision.WetCollision(this.Position, this.Width, this.Height);
						if (flag8)
						{
							if (this.onFire && !this.lavaWet)
							{
								for (int num83 = 0; num83 < 10; num83++)
								{
									if (this.buffType[num83] == 24)
									{
										this.DelBuff(num83);
									}
								}
							}
							if (!this.wet)
							{
								if (this.wetCount == 0)
								{
									this.wetCount = 10;
								}
								this.wet = true;
							}
						}
						else
						{
							if (this.wet)
							{
								this.wet = false;
								if (this.jump > Player.jumpHeight / 5)
								{
									this.jump = Player.jumpHeight / 5;
								}
								if (this.wetCount == 0)
								{
									this.wetCount = 10;
								}
							}
						}
						if (!this.wet)
						{
							this.lavaWet = false;
						}
						if (this.wetCount > 0)
						{
							this.wetCount -= 1;
						}
						if (this.wet)
						{
							if (this.wet)
							{
								Vector2 v3 = this.Velocity;
								this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.Width, this.Height, this.controlDown, false);
								Vector2 value3 = this.Velocity * 0.5f;
								if (this.Velocity.X != v3.X)
								{
									value3.X = this.Velocity.X;
								}
								if (this.Velocity.Y != v3.Y)
								{
									value3.Y = this.Velocity.Y;
								}
								this.Position += value3;
							}
						}
						else
						{
							this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.Width, this.Height, this.controlDown, false);
							if (this.waterWalk)
							{
								this.Velocity = Collision.WaterCollision(this.Position, this.Velocity, this.Width, this.Height, this.controlDown, false);
							}
							this.Position += this.Velocity;
						}
						if (this.Velocity.Y == 0f)
						{
							if (this.gravDir == 1f && Collision.up)
							{
								this.Velocity.Y = 0.01f;
								this.jump = 0;
							}
							else
							{
								if (this.gravDir == -1f && Collision.down)
								{
									this.Velocity.Y = -0.01f;
									this.jump = 0;
								}
							}
						}
						if (this.Position.X < Main.leftWorld + 336f + 16f)
						{
							this.Position.X = Main.leftWorld + 336f + 16f;
							this.Velocity.X = 0f;
						}
						if (this.Position.X + (float)this.Width > Main.rightWorld - 336f - 32f)
						{
							this.Position.X = Main.rightWorld - 336f - 32f - (float)this.Width;
							this.Velocity.X = 0f;
						}
						if (this.Position.Y < Main.topWorld + 336f + 16f)
						{
							this.Position.Y = Main.topWorld + 336f + 16f;
							if ((double)this.Velocity.Y < -0.1)
							{
								this.Velocity.Y = -0.1f;
							}
						}
						if (this.Position.Y > Main.bottomWorld - 336f - 32f - (float)this.Height)
						{
							this.Position.Y = Main.bottomWorld - 336f - 32f - (float)this.Height;
							this.Velocity.Y = 0f;
						}
						this.ItemCheck(TileRefs, sandbox, i);
						this.PlayerFrame();

						if (this.statLife > this.statLifeMax)
						{
							this.statLife = this.statLifeMax;
						}
						this.grappling[0] = -1;
						this.grapCount = 0;
						return;
					}

					// this.dead == true
					this.poisoned = false;
					this.onFire = false;
					this.blind = false;
					this.gravDir = 1f;

					for (int num92 = 0; num92 < 10; num92++)
					{
						this.buffTime[num92] = 0;
						this.buffType[num92] = 0;
					}

					this.grappling[0] = -1;
					this.grappling[1] = -1;
					this.grappling[2] = -1;
					this.sign = -1;
					this.talkNPC = -1;
					this.statLife = 0;
					this.channel = false;
					this.potionDelay = 0;
					this.chest = -1;
					this.changeItem = -1;
					this.itemAnimation = 0;
					this.immuneAlpha += 2;
					if (this.immuneAlpha > 255)
					{
						this.immuneAlpha = 255;
					}
					this.headPosition += this.headVelocity;
					this.bodyPosition += this.bodyVelocity;
					this.legPosition += this.legVelocity;
					this.headRotation += this.headVelocity.X * 0.1f;
					this.bodyRotation += this.bodyVelocity.X * 0.1f;
					this.legRotation += this.legVelocity.X * 0.1f;
					this.headVelocity.Y = this.headVelocity.Y + 0.1f;
					this.bodyVelocity.Y = this.bodyVelocity.Y + 0.1f;
					this.legVelocity.Y = this.legVelocity.Y + 0.1f;
					this.headVelocity.X = this.headVelocity.X * 0.99f;
					this.bodyVelocity.X = this.bodyVelocity.X * 0.99f;
					this.legVelocity.X = this.legVelocity.X * 0.99f;
					if (this.Difficulty == 2)
					{
						if (this.respawnTimer > 0)
						{
							this.respawnTimer--;
							return;
						}

						var action = Program.properties.HardcoreDeathAction;
						if (action == "respawn")
						{
							this.respawnTimer = Int32.MaxValue;
							this.Respawn();
							return;
						}
						else if (action == "kick")
						{
							Kick("Rest in peace.");
							return;
						}

						if (this.whoAmi == Main.myPlayer)
						{
							this.ghost = true;
							return;
						}
					}
					else
					{
						this.respawnTimer--;
						if (this.respawnTimer <= 0 && Program.properties.MaxRespawnTime > 0)
						{
							this.respawnTimer = Int32.MaxValue;
							this.Respawn();
							//this.Spawn();
							return;
						}
					}
				}
			}
			catch (Exception e)
			{
				ProgramLog.Log("Error In UpdatePlayer: " + e.Message);
				ProgramLog.Log("Stack: " + e.StackTrace);
			}
		}

		/// <summary>
		/// Gives money for selling an item
		/// </summary>
		/// <param name="price">Value of item</param>
		/// <param name="stack">Number sold</param>
		/// <returns></returns>
		public bool SellItem(int price, int stack)
		{
			if (price <= 0)
			{
				return false;
			}
			Item[] array = new Item[MAX_INVENTORY];
			for (int i = 0; i < MAX_INVENTORY; i++)
			{
				array[i] = new Item();
				array[i] = (Item)this.inventory[i].Clone();
			}
			int j = price / 5;
			j *= stack;
			if (j < 1)
			{
				j = 1;
			}
			bool flag = false;
			while (j >= 1000000)
			{
				if (flag)
				{
					break;
				}
				int num = -1;
				for (int k = 43; k >= 0; k--)
				{
					if (num == -1 && (this.inventory[k].Type == 0 || this.inventory[k].Stack == 0))
					{
						num = k;
					}
					while (this.inventory[k].Type == 74 && this.inventory[k].Stack < this.inventory[k].MaxStack && j >= 1000000)
					{
						this.inventory[k].Stack++;
						j -= 1000000;
						this.DoCoins(k);
						if (this.inventory[k].Stack == 0 && num == -1)
						{
							num = k;
						}
					}
				}
				if (j >= 1000000)
				{
					if (num == -1)
					{
						flag = true;
					}
					else
					{
						this.inventory[num] = Registries.Item.Create(74);
						j -= 1000000;
					}
				}
			}
			while (j >= 10000)
			{
				if (flag)
				{
					break;
				}
				int num2 = -1;
				for (int l = 43; l >= 0; l--)
				{
					if (num2 == -1 && (this.inventory[l].Type == 0 || this.inventory[l].Stack == 0))
					{
						num2 = l;
					}
					while (this.inventory[l].Type == 73 && this.inventory[l].Stack < this.inventory[l].MaxStack && j >= 10000)
					{
						this.inventory[l].Stack++;
						j -= 10000;
						this.DoCoins(l);
						if (this.inventory[l].Stack == 0 && num2 == -1)
						{
							num2 = l;
						}
					}
				}
				if (j >= 10000)
				{
					if (num2 == -1)
					{
						flag = true;
					}
					else
					{
						this.inventory[num2] = Registries.Item.Create(73);
						j -= 10000;
					}
				}
			}
			while (j >= 100)
			{
				if (flag)
				{
					break;
				}
				int num3 = -1;
				for (int m = 43; m >= 0; m--)
				{
					if (num3 == -1 && (this.inventory[m].Type == 0 || this.inventory[m].Stack == 0))
					{
						num3 = m;
					}
					while (this.inventory[m].Type == 72 && this.inventory[m].Stack < this.inventory[m].MaxStack && j >= 100)
					{
						this.inventory[m].Stack++;
						j -= 100;
						this.DoCoins(m);
						if (this.inventory[m].Stack == 0 && num3 == -1)
						{
							num3 = m;
						}
					}
				}
				if (j >= 100)
				{
					if (num3 == -1)
					{
						flag = true;
					}
					else
					{
						this.inventory[num3] = Registries.Item.Create(72);
						j -= 100;
					}
				}
			}
			while (j >= 1 && !flag)
			{
				int num4 = -1;
				for (int n = 43; n >= 0; n--)
				{
					if (num4 == -1 && (this.inventory[n].Type == 0 || this.inventory[n].Stack == 0))
					{
						num4 = n;
					}
					while (this.inventory[n].Type == 71 && this.inventory[n].Stack < this.inventory[n].MaxStack && j >= 1)
					{
						this.inventory[n].Stack++;
						j--;
						this.DoCoins(n);
						if (this.inventory[n].Stack == 0 && num4 == -1)
						{
							num4 = n;
						}
					}
				}
				if (j >= 1)
				{
					if (num4 == -1)
					{
						flag = true;
					}
					else
					{
						this.inventory[num4] = Registries.Item.Create(71);
						j--;
					}
				}
			}
			if (flag)
			{
				for (int num5 = 0; num5 < MAX_INVENTORY; num5++)
				{
					this.inventory[num5] = (Item)array[num5].Clone();
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// Buy item
		/// </summary>
		/// <param name="price">cost of item</param>
		/// <returns>Whether item was bought</returns>
		public bool BuyItem(int price)
		{
			if (price == 0)
			{
				return false;
			}
			int num = 0;
			int i = price;
			Item[] array = new Item[MAX_INVENTORY];
			for (int j = 0; j < MAX_INVENTORY; j++)
			{
				array[j] = new Item();
				array[j] = (Item)this.inventory[j].Clone();
				if (this.inventory[j].Type == 71)
				{
					num += this.inventory[j].Stack;
				}
				if (this.inventory[j].Type == 72)
				{
					num += this.inventory[j].Stack * 100;
				}
				if (this.inventory[j].Type == 73)
				{
					num += this.inventory[j].Stack * 10000;
				}
				if (this.inventory[j].Type == 74)
				{
					num += this.inventory[j].Stack * 1000000;
				}
			}
			if (num >= price)
			{
				i = price;
				while (i > 0)
				{
					if (i >= 1000000)
					{
						for (int k = 0; k < MAX_INVENTORY; k++)
						{
							if (this.inventory[k].Type == 74)
							{
								while (this.inventory[k].Stack > 0 && i >= 1000000)
								{
									i -= 1000000;
									this.inventory[k].Stack--;
									if (this.inventory[k].Stack == 0)
									{
										this.inventory[k].Type = 0;
									}
								}
							}
						}
					}
					if (i >= 10000)
					{
						for (int l = 0; l < MAX_INVENTORY; l++)
						{
							if (this.inventory[l].Type == 73)
							{
								while (this.inventory[l].Stack > 0 && i >= 10000)
								{
									i -= 10000;
									this.inventory[l].Stack--;
									if (this.inventory[l].Stack == 0)
									{
										this.inventory[l].Type = 0;
									}
								}
							}
						}
					}
					if (i >= 100)
					{
						for (int m = 0; m < MAX_INVENTORY; m++)
						{
							if (this.inventory[m].Type == 72)
							{
								while (this.inventory[m].Stack > 0 && i >= 100)
								{
									i -= 100;
									this.inventory[m].Stack--;
									if (this.inventory[m].Stack == 0)
									{
										this.inventory[m].Type = 0;
									}
								}
							}
						}
					}
					if (i >= 1)
					{
						for (int n = 0; n < MAX_INVENTORY; n++)
						{
							if (this.inventory[n].Type == 71)
							{
								while (this.inventory[n].Stack > 0 && i >= 1)
								{
									i--;
									this.inventory[n].Stack--;
									if (this.inventory[n].Stack == 0)
									{
										this.inventory[n].Type = 0;
									}
								}
							}
						}
					}
					if (i > 0)
					{
						int num2 = -1;
						for (int num3 = 43; num3 >= 0; num3--)
						{
							if (this.inventory[num3].Type == 0 || this.inventory[num3].Stack == 0)
							{
								num2 = num3;
								break;
							}
						}
						if (num2 < 0)
						{
							for (int num4 = 0; num4 < MAX_INVENTORY; num4++)
							{
								this.inventory[num4] = (Item)array[num4].Clone();
							}
							return false;
						}
						bool flag = true;
						if (i >= 10000)
						{
							for (int num5 = 0; num5 < MAX_INVENTORY; num5++)
							{
								if (this.inventory[num5].Type == 74 && this.inventory[num5].Stack >= 1)
								{
									this.inventory[num5].Stack--;
									if (this.inventory[num5].Stack == 0)
									{
										this.inventory[num5].Type = 0;
									}
									this.inventory[num2] = Registries.Item.Create(73, 100);
									flag = false;
									break;
								}
							}
						}
						else
						{
							if (i >= 100)
							{
								for (int num6 = 0; num6 < MAX_INVENTORY; num6++)
								{
									if (this.inventory[num6].Type == 73 && this.inventory[num6].Stack >= 1)
									{
										this.inventory[num6].Stack--;
										if (this.inventory[num6].Stack == 0)
										{
											this.inventory[num6].Type = 0;
										}
										this.inventory[num2] = Registries.Item.Create(72, 100);
										flag = false;
										break;
									}
								}
							}
							else
							{
								if (i >= 1)
								{
									for (int num7 = 0; num7 < MAX_INVENTORY; num7++)
									{
										if (this.inventory[num7].Type == 72 && this.inventory[num7].Stack >= 1)
										{
											this.inventory[num7].Stack--;
											if (this.inventory[num7].Stack == 0)
											{
												this.inventory[num7].Type = 0;
											}
											this.inventory[num2] = Registries.Item.Create(71, 100);
											flag = false;
											break;
										}
									}
								}
							}
						}
						if (flag)
						{
							if (i < 10000)
							{
								for (int num8 = 0; num8 < MAX_INVENTORY; num8++)
								{
									if (this.inventory[num8].Type == 73 && this.inventory[num8].Stack >= 1)
									{
										this.inventory[num8].Stack--;
										if (this.inventory[num8].Stack == 0)
										{
											this.inventory[num8].Type = 0;
										}
										this.inventory[num2] = Registries.Item.Create(72, 100);
										flag = false;
										break;
									}
								}
							}
							if (flag && i < 1000000)
							{
								for (int num9 = 0; num9 < MAX_INVENTORY; num9++)
								{
									if (this.inventory[num9].Type == 74 && this.inventory[num9].Stack >= 1)
									{
										this.inventory[num9].Stack--;
										if (this.inventory[num9].Stack == 0)
										{
											this.inventory[num9].Type = 0;
										}
										this.inventory[num2] = Registries.Item.Create(73, 100);
										flag = false;
										break;
									}
								}
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		// I don't think this does anything, also not updated for 1.0.6, not even for 1.0.5 apparently
		//        public void AdjTiles()
		//		{
		//			int num = 4;
		//			int num2 = 3;
		//			for (int i = 0; i < 107; i++)
		//			{
		//				this.oldAdjTile[i] = this.adjTile[i];
		//				this.adjTile[i] = false;
		//			}
		//			int num3 = (int)((this.Position.X + (float)(this.Width / 2)) / 16f);
		//			int num4 = (int)((this.Position.Y + (float)this.Height) / 16f);
		//			for (int j = num3 - num; j <= num3 + num; j++)
		//			{
		//				for (int k = num4 - num2; k < num4 + num2; k++)
		//				{
		//					if (Main.tile.At(j, k).Active)
		//					{
		//						this.adjTile[(int)Main.tile.At(j, k).Type] = true;
		//						if (Main.tile.At(j, k).Type == 77)
		//						{
		//							this.adjTile[17] = true;
		//						}
		//					}
		//				}
		//			}
		//		}

		public void PlayerFrame()
		{
			if (this.swimTime > 0)
			{
				this.swimTime--;
				if (!this.wet)
				{
					this.swimTime = 0;
				}
			}

			this.head = this.armor[0].HeadSlot;
			this.body = this.armor[1].BodySlot;
			this.legs = this.armor[2].LegSlot;
			if (!this.hostile)
			{
				if (this.armor[8].HeadSlot >= 0)
				{
					this.head = this.armor[8].HeadSlot;
				}
				if (this.armor[9].BodySlot >= 0)
				{
					this.body = this.armor[9].BodySlot;
				}
				if (this.armor[10].LegSlot >= 0)
				{
					this.legs = this.armor[10].LegSlot;
				}
			}
			this.socialShadow = false;
			if (this.head == 5 && this.body == 5 && this.legs == 5)
			{
				this.socialShadow = true;
			}
			if (this.head == 7 && this.body == 7 && this.legs == 7)
			{
				this.boneArmor = true;
			}

			this.bodyFrame.Width = 40;
			this.bodyFrame.Height = 56;
			this.legFrame.Width = 40;
			this.legFrame.Height = 56;
			this.bodyFrame.X = 0;
			this.legFrame.X = 0;

			Item selectedItem = inventory[selectedItemIndex];
			if (this.itemAnimation > 0 && selectedItem.UseStyle != 10)
			{
				if (selectedItem.UseStyle == 1 || selectedItem.Type == 0)
				{
					if ((double)this.itemAnimation < (double)this.itemAnimationMax * 0.333)
					{
						this.bodyFrame.Y = this.bodyFrame.Height * 3;
					}
					else
					{
						if ((double)this.itemAnimation < (double)this.itemAnimationMax * 0.666)
						{
							this.bodyFrame.Y = this.bodyFrame.Height * 2;
						}
						else
						{
							this.bodyFrame.Y = this.bodyFrame.Height;
						}
					}
				}
				else
				{
					if (selectedItem.UseStyle == 2)
					{
						if ((double)this.itemAnimation < (double)this.itemAnimationMax * 0.5)
						{
							this.bodyFrame.Y = this.bodyFrame.Height * 4;
						}
						else
						{
							this.bodyFrame.Y = this.bodyFrame.Height * 5;
						}
					}
					else
					{
						if (selectedItem.UseStyle == 3)
						{
							if ((double)this.itemAnimation > (double)this.itemAnimationMax * 0.666)
							{
								this.bodyFrame.Y = this.bodyFrame.Height * 3;
							}
							else
							{
								this.bodyFrame.Y = this.bodyFrame.Height * 3;
							}
						}
						else
						{
							if (selectedItem.UseStyle == 4)
							{
								this.bodyFrame.Y = this.bodyFrame.Height * 2;
							}
							else
							{
								if (selectedItem.UseStyle == 5)
								{
									if (this.inventory[this.selectedItemIndex].Type == 281)
									{
										this.bodyFrame.Y = this.bodyFrame.Height * 2;
									}
									else
									{
										float num4 = this.itemRotation * (float)this.direction;
										this.bodyFrame.Y = this.bodyFrame.Height * 3;
										if ((double)num4 < -0.75)
										{
											this.bodyFrame.Y = this.bodyFrame.Height * 2;
											if (this.gravDir == -1f)
											{
												this.bodyFrame.Y = this.bodyFrame.Height * 4;
											}
										}
										if ((double)num4 > 0.6)
										{
											this.bodyFrame.Y = this.bodyFrame.Height * 4;
											if (this.gravDir == -1f)
											{
												this.bodyFrame.Y = this.bodyFrame.Height * 2;
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
				if (selectedItem.HoldStyle == 1)
				{
					this.bodyFrame.Y = this.bodyFrame.Height * 3;
				}
				else
				{
					if (selectedItem.HoldStyle == 2)
					{
						this.bodyFrame.Y = this.bodyFrame.Height * 2;
					}
					else
					{
						if (this.grappling[0] >= 0)
						{
							Vector2 vector = new Vector2(this.Position.X + (float)this.Width * 0.5f, this.Position.Y + (float)this.Height * 0.5f);
							float num2 = 0f;
							float num3 = 0f;
							for (int i = 0; i < this.grapCount; i++)
							{
								num2 += Main.projectile[this.grappling[i]].Position.X + (float)(Main.projectile[this.grappling[i]].Width / 2);
								num3 += Main.projectile[this.grappling[i]].Position.Y + (float)(Main.projectile[this.grappling[i]].Height / 2);
							}
							num2 /= (float)this.grapCount;
							num3 /= (float)this.grapCount;
							num2 -= vector.X;
							num3 -= vector.Y;
							if (num3 < 0f && Math.Abs(num3) > Math.Abs(num2))
							{
								this.bodyFrame.Y = this.bodyFrame.Height * 2;
								if (this.gravDir == -1f)
									this.bodyFrame.Y = this.bodyFrame.Height * 4;
							}
							else
							{
								if (num3 > 0f && Math.Abs(num3) > Math.Abs(num2))
								{
									this.bodyFrame.Y = this.bodyFrame.Height * 4;
									if (this.gravDir == -1f)
										this.bodyFrame.Y = this.bodyFrame.Height * 2;
								}
								else
								{
									this.bodyFrame.Y = this.bodyFrame.Height * 3;
								}
							}
						}
						else
						{
							if (this.swimTime > 0)
							{
								if (this.swimTime > 20)
								{
									this.bodyFrame.Y = 0;
								}
								else
								{
									if (this.swimTime > 10)
									{
										this.bodyFrame.Y = this.bodyFrame.Height * 5;
									}
									else
									{
										this.bodyFrame.Y = 0;
									}
								}
							}
							else
							{
								if (this.Velocity.Y != 0f)
								{
									this.bodyFrameCounter = 0.0;
									this.bodyFrame.Y = this.bodyFrame.Height * 5;
								}
								else
								{
									if (this.Velocity.X != 0f)
									{
										this.bodyFrameCounter += (double)Math.Abs(this.Velocity.X) * 1.5;
										this.bodyFrame.Y = this.legFrame.Y;
									}
									else
									{
										this.bodyFrameCounter = 0.0;
										this.bodyFrame.Y = 0;
									}
								}
							}
						}
					}
				}
			}
			if (this.swimTime > 0)
			{
				this.legFrameCounter += 2.0;
				while (this.legFrameCounter > 8.0)
				{
					this.legFrameCounter -= 8.0;
					this.legFrame.Y = this.legFrame.Y + this.legFrame.Height;
				}
				if (this.legFrame.Y < this.legFrame.Height * 7)
				{
					this.legFrame.Y = this.legFrame.Height * 19;
					return;
				}
				if (this.legFrame.Y > this.legFrame.Height * 19)
				{
					this.legFrame.Y = this.legFrame.Height * 7;
					return;
				}
			}
			else
			{
				if (this.Velocity.Y != 0f || this.grappling[0] > -1)
				{
					this.legFrameCounter = 0.0;
					this.legFrame.Y = this.legFrame.Height * 5;
					return;
				}
				if (this.Velocity.X != 0f)
				{
					this.legFrameCounter += (double)Math.Abs(this.Velocity.X) * 1.3;
					while (this.legFrameCounter > 8.0)
					{
						this.legFrameCounter -= 8.0;
						this.legFrame.Y = this.legFrame.Y + this.legFrame.Height;
					}
					if (this.legFrame.Y < this.legFrame.Height * 7)
					{
						this.legFrame.Y = this.legFrame.Height * 19;
						return;
					}
					if (this.legFrame.Y > this.legFrame.Height * 19)
					{
						this.legFrame.Y = this.legFrame.Height * 7;
						return;
					}
				}
				else
				{
					this.legFrameCounter = 0.0;
					this.legFrame.Y = 0;
				}
			}
		}

		/// <summary>
		/// Spawns player
		/// </summary>
		public void Spawn(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;
			//			if (this.whoAmi == Main.myPlayer)
			//			{
			//				this.FindSpawn();
			//				if (!Player.CheckSpawn(this.SpawnX, this.SpawnY))
			//				{
			//					this.SpawnX = -1;
			//					this.SpawnY = -1;
			//				}
			//			}

			this.headPosition = default(Vector2);
			this.bodyPosition = default(Vector2);
			this.legPosition = default(Vector2);
			this.headRotation = 0f;
			this.bodyRotation = 0f;
			this.legRotation = 0f;
			if (this.statLife <= 0)
			{
				this.statLife = 100;
				this.breath = this.breathMax;
				if (this.spawnMax)
				{
					this.statLife = this.statLifeMax;
					this.statMana = this.statManaMax2;
				}
			}
			this.immune = true;
			this.dead = false;
			this.immuneTime = 0;
			this.Active = true;
			if (this.SpawnX >= 0 && this.SpawnY >= 0)
			{
				this.Position.X = (float)(this.SpawnX * 16 + 8 - this.Width / 2);
				this.Position.Y = (float)(this.SpawnY * 16 - this.Height);
			}
			else
			{
				this.Position.X = (float)(Main.spawnTileX * 16 + 8 - this.Width / 2);
				this.Position.Y = (float)(Main.spawnTileY * 16 - this.Height);
				for (int i = Main.spawnTileX - 1; i < Main.spawnTileX + 2; i++)
				{
					for (int j = Main.spawnTileY - 3; j < Main.spawnTileY; j++)
					{
						if (Main.tileSolid[(int)TileRefs(i, j).Type] && !Main.tileSolidTop[(int)TileRefs(i, j).Type])
						{
							if (TileRefs(i, j).Liquid > 0)
							{
								TileRefs(i, j).SetLava(false);
								TileRefs(i, j).SetLiquid(0);
								WorldModify.SquareTileFrame(TileRefs, sandbox, i, j, true);
							}
							WorldModify.KillTile(TileRefs, sandbox, i, j);
						}
					}
				}
			}
			this.wet = Collision.WetCollision(this.Position, this.Width, this.Height);
			this.wetCount = 0;
			this.lavaWet = false;
			this.fallStart = (int)(this.Position.Y / 16f);
			this.Velocity.X = 0f;
			this.Velocity.Y = 0f;
			this.talkNPC = -1;
			if (this.pvpDeath)
			{
				this.pvpDeath = false;
				this.immuneTime = 300;
				this.statLife = this.statLifeMax;
			}
			if (this.whoAmi == Main.myPlayer)
			{
				Main.screenPosition.X = this.Position.X + (float)(this.Width / 2) - (float)(Main.screenWidth / 2);
				Main.screenPosition.Y = this.Position.Y + (float)(this.Height / 2) - (float)(Main.screenHeight / 2);
			}
		}

		/// <summary>
		/// Hurts player
		/// </summary>
		/// <param name="aggressor">Sender who hurt the Player</param>
		/// <param name="Damage">Damage to do</param>
		/// <param name="hitDirection">Direction of attack</param>
		/// <param name="pvp">Whether attack is PvP</param>
		/// <param name="quiet">Whether to announce</param>
		/// <param name="deathText">Text to display upon death</param>
		/// <param name="crit">Whether the hit is critical</param>
		/// <returns>Damage done</returns>
		public double Hurt(ISender aggressor, int Damage, int hitDirection, bool pvp = false, bool quiet = false, string deathText = " was slain...", bool crit = false)
		{
			if (this.immune) return 0.0;

			var proj = aggressor as Projectile;
			var plr = aggressor as Player;

			var ctx = new HookContext
			{
				Sender = aggressor,
				Player = proj != null ? (proj.Creator as Player) : plr,
			};

			ctx.Connection = ctx.Player != null ? ctx.Player.Connection : null;

			var args = new HookArgs.PlayerHurt
			{
				Victim = this,
				Damage = Damage,
				HitDirection = hitDirection,
				Pvp = pvp,
				Quiet = quiet,
				Obituary = deathText,
				Critical = crit,
			};

			HookPoints.PlayerHurt.Invoke(ref ctx, ref args);

			if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
				return 0.0;

			if (ctx.Result == HookResult.RECTIFY)
			{
				var conn = ctx.Connection;
				if (conn != null)
				{
					var msg = NetMessage.PrepareThreadInstance();
					msg.PlayerHealthUpdate(whoAmi);
					msg.Send(conn);
				}
				return 0.0;
			}

			NetMessage.SendData(26, -1, plr != null ? plr.whoAmi : -1, args.Obituary, whoAmi, args.HitDirection, args.Damage, args.Pvp ? 1 : 0, args.Critical ? 1 : 0);

			return HurtInternal(args.Damage, args.HitDirection, args.Pvp, args.Quiet, args.Obituary, args.Critical);
		}

		public double HurtInternal(int Damage, int hitDirection, bool pvp = false, bool quiet = false, string deathText = " was slain...", bool crit = false)
		{
			if (!this.immune)
			{
				int num = Damage;

				if (pvp)
				{
					num *= 2;
				}
				double num2 = Main.CalculateDamage(num, this.statDefense);
				if (crit)
				{
					num *= 2;
				}
				if (num2 >= 1.0)
				{
					this.statLife -= (int)num2;
					this.immune = true;
					this.immuneTime = 40;
					this.lifeRegenTime = 0;
					if (pvp)
					{
						this.immuneTime = 8;
					}
					if (!this.noKnockback && hitDirection != 0)
					{
						this.Velocity.X = 4.5f * (float)hitDirection;
						this.Velocity.Y = -3.5f;
					}

					if (this.statLife > 0)
					{
						int num4 = 0;
						while ((double)num4 < num2 / (double)this.statLifeMax * 100.0)
						{
							num4++;
						}
					}
					else
					{
						this.statLife = 0;
						if (this.whoAmi == Main.myPlayer)
						{
							this.KillMe(num2, hitDirection, pvp, deathText);
						}
					}
				}
				if (pvp)
				{
					num2 = Main.CalculateDamage(num, this.statDefense);
				}
				return num2;
			}
			return 0.0;
		}

		/// <summary>
		/// Drops coins, used upon death
		/// </summary>
		public void DropCoins()
		{
			for (int i = 0; i < MAX_INVENTORY; i++)
			{
				if (this.inventory[i].Type >= 71 && this.inventory[i].Type <= 74)
				{
					int num = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, this.inventory[i].Type, 1, false);
					int num2 = this.inventory[i].Stack / 2;
					num2 = this.inventory[i].Stack - num2;
					this.inventory[i].Stack -= num2;
					if (this.inventory[i].Stack <= 0)
					{
						this.inventory[i] = new Item();
					}
					Main.item[num].Stack = num2;
					Main.item[num].Velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
					Main.item[num].Velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
					Main.item[num].NoGrabDelay = 100;
				}
			}
		}

		/// <summary>
		/// Kills player
		/// </summary>
		/// <param name="dmg">Damage done</param>
		/// <param name="hitDirection">Direction of attack</param>
		/// <param name="pvp">Whether the attack was PvP</param>
		/// <param name="deathText">Text to display upon death</param>
		public void KillMe(double dmg, int hitDirection, bool pvp = false, string deathText = " was slain...")
		{
			if ((Main.myPlayer == this.whoAmi))
			{
				return;
			}
			if (this.dead)
			{
				return;
			}
			if (pvp)
			{
				this.pvpDeath = true;
			}

			//if (this.Difficulty == 0)
			{
				float num = (float)Main.rand.Next(-35, 36) * 0.1f;
				while (num < 2f && num > -2f)
				{
					num += (float)Main.rand.Next(-30, 31) * 0.1f;
				}
				int num2 = Projectile.NewProjectile(this.Position.X + (float)(this.Width / 2),
					this.Position.Y + (float)(this.head / 2), (float)Main.rand.Next(10, 30) * 0.1f * (float)hitDirection + num,
					(float)Main.rand.Next(-40, -20) * 0.1f, ProjectileType.N43_TOMBSTONE, 50, 0f, Main.myPlayer);
				Main.projectile[num2].miscText = this.Name + deathText;
				if (Main.myPlayer == this.whoAmi)
				{
					this.statLifeMax = 100;
					this.statManaMax = 0;
					//this.DropItems(); //Client code?
				}
			}
			this.headVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			this.bodyVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			this.legVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			this.headVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			this.bodyVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			this.legVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);

			this.dead = true;
			var t = Program.properties.MaxRespawnTime;
			if (t > 0)
				this.respawnTimer = 60 * t;
			else
				this.respawnTimer = 600;
			this.immuneAlpha = 0;
			NetMessage.SendData(25, -1, -1, this.Name + deathText, 255, 225f, 25f, 25f);

			if (!pvp && this.whoAmi == Main.myPlayer && this.Difficulty == 0)
			{
				this.DropCoins();
			}
		}

		/// <summary>
		/// Checks for space for an item
		/// </summary>
		/// <param name="newItem">Item to check space for</param>
		/// <returns>True if there's room, false if not</returns>
		public bool ItemSpace(Item newItem)
		{
			if (newItem.Type == 58)
			{
				return true;
			}
			if (newItem.Type == 184)
			{
				return true;
			}
			int num = 40;
			if (newItem.Type == 71 || newItem.Type == 72 || newItem.Type == 73 || newItem.Type == 74)
			{
				num = 44;
			}
			for (int i = 0; i < num; i++)
			{
				if (this.inventory[i].Type == 0)
				{
					return true;
				}
			}
			for (int j = 0; j < num; j++)
			{
				if (this.inventory[j].Type > 0 && this.inventory[j].Stack < this.inventory[j].MaxStack && newItem.IsTheSameAs(this.inventory[j]))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Updates players coins values
		/// </summary>
		/// <param name="inventoryIndex">Index of coin to update</param>
		public void DoCoins(int inventoryIndex)
		{
			Item item = inventory[inventoryIndex];
			if ((item.Type == 71 || item.Type == 72 || item.Type == 73) && item.Stack == item.MaxStack)
			{
				item = Registries.Item.Create(item.Type + 1);
				inventory[inventoryIndex] = item;

				for (int i = 0; i < MAX_INVENTORY; i++)
				{
					Item compareItem = inventory[i];
					if (compareItem.IsTheSameAs(item) && i != inventoryIndex && compareItem.Stack < compareItem.MaxStack)
					{
						compareItem.Stack++;
						inventory[inventoryIndex] = Registries.Item.Default;
						this.DoCoins(i);
					}
				}
			}
		}

		// client-side
		/// <summary>
		/// Gets all available ammo for item to put as ammo number.  Client-side
		/// </summary>
		/// <param name="plr">Player index</param>
		/// <param name="newItem">Item to check for</param>
		/// <returns>.</returns>
		public Item FillAmmo(int plr, Item newItem)
		{
			for (int i = 0; i < 4; i++)
			{
				if (this.ammo[i].Type > 0 && this.ammo[i].Stack < this.ammo[i].MaxStack && newItem.IsTheSameAs(this.ammo[i]))
				{
					if (newItem.Stack + this.ammo[i].Stack <= this.ammo[i].MaxStack)
					{
						this.ammo[i].Stack += newItem.Stack;
						this.DoCoins(i);
						return new Item();
					}
					newItem.Stack -= this.ammo[i].MaxStack - this.ammo[i].Stack;
					this.ammo[i].Stack = this.ammo[i].MaxStack;
					this.DoCoins(i);
				}
			}
			if (newItem.Type != 169 && newItem.Type != 75)
			{
				for (int j = 0; j < 4; j++)
				{
					if (this.ammo[j].Type == 0)
					{
						this.ammo[j] = newItem;
						this.DoCoins(j);
						return new Item();
					}
				}
			}
			return newItem;
		}

		/// <summary>
		/// Pick up item.  Client-side
		/// </summary>
		/// <param name="plr">Player index</param>
		/// <param name="newItem">Item to pick up</param>
		/// <returns>Item picked up.  Returns default if not picked up</returns>
		public Item GetItem(int plr, Item newItem)
		{
			Item item = newItem;
			int num = 40;
			if (newItem.NoGrabDelay > 0)
			{
				return item;
			}
			int num2 = 0;
			if (newItem.Type == 71 || newItem.Type == 72 || newItem.Type == 73 || newItem.Type == 74)
			{
				num2 = -4;
				num = 44;
			}
			if (item.Ammo > 0)
			{
				item = this.FillAmmo(plr, item);
				if (item.Type == 0 || item.Stack == 0)
				{
					return new Item();
				}
			}
			for (int i = num2; i < 40; i++)
			{
				int num3 = i;
				if (num3 < 0)
				{
					num3 = 44 + i;
				}
				if (this.inventory[num3].Type > 0 && this.inventory[num3].Stack < this.inventory[num3].MaxStack && item.IsTheSameAs(this.inventory[num3]))
				{
					if (item.Stack + this.inventory[num3].Stack <= this.inventory[num3].MaxStack)
					{
						this.inventory[num3].Stack += item.Stack;
						this.DoCoins(num3);
						return new Item();
					}
					item.Stack -= this.inventory[num3].MaxStack - this.inventory[num3].Stack;
					this.inventory[num3].Stack = this.inventory[num3].MaxStack;
					this.DoCoins(num3);
				}
			}
			for (int j = num - 1; j >= 0; j--)
			{
				if (this.inventory[j].Type == 0)
				{
					this.inventory[j] = item;
					this.DoCoins(j);
					return new Item();
				}
			}
			return item;
		}

		public void ItemCheck(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			int dmg = this.inventory[this.selectedItemIndex].damage;
			if (dmg > 0)
			{
				if (this.inventory[this.selectedItemIndex].Melee)
				{
					dmg = (int)((float)dmg * this.meleeDamage);
				}
				else if (this.inventory[this.selectedItemIndex].Ranged)
				{
					dmg = (int)((float)dmg * this.rangedDamage);
				}
				else if (this.inventory[this.selectedItemIndex].Magic)
				{
					dmg = (int)((float)dmg * this.magicDamage);
				}
			}

			Item selectedItem = inventory[selectedItemIndex];
			if (selectedItem.AutoReuse && !this.noItems)
			{
				releaseUseItem = true;
				if (itemAnimation == 1 && selectedItem.Stack > 0)
				{
					itemAnimation = 0;
				}
			}
			if (this.itemAnimation == 0 && ReUseDelay > 0)
			{
				this.itemAnimation = ReUseDelay;
				this.itemTime = ReUseDelay;
				ReUseDelay = 0;
			}
			if (controlUseItem && itemAnimation == 0 && releaseUseItem && selectedItem.UseStyle > 0)
			{
				bool flag = true;
				if (this.noItems)
				{
					flag = false;
				}
				if (selectedItem.Shoot == ProjectileType.N6_ENCHANTED_BOOMERANG ||
					selectedItem.Shoot == ProjectileType.N19_FLAMARANG ||
					selectedItem.Shoot == ProjectileType.N33_THORN_CHAKRUM)
				{
					for (int j = 0; j < Main.maxProjectiles; j++)
					{
						if (Main.projectile[j].Active && Main.projectile[j].Owner == Main.myPlayer && Main.projectile[j].type == selectedItem.Shoot)
						{
							flag = false;
						}
					}
				}
				if (selectedItem.Shoot == ProjectileType.N13_HOOK || selectedItem.Shoot == ProjectileType.N32_IVY_WHIP)
				{
					for (int k = 0; k < 1000; k++)
					{
						if (Main.projectile[k].Active && Main.projectile[k].Owner == Main.myPlayer && Main.projectile[k].type == selectedItem.Shoot && Main.projectile[k].ai[0] != 2f)
						{
							flag = false;
						}
					}
				}
				if (selectedItem.Potion && flag)
				{
					if (this.potionDelay <= 0)
					{
						potionDelay = Item.POTION_DELAY;
						this.AddBuff(21, this.potionDelay, true);
					}
					else
					{
						flag = false;
					}
				}

				if (selectedItem.Mana > 0 && flag)
				{
					if (selectedItem.Type != 127 || !spaceGun)
					{
						if (statMana >= (int)((float)selectedItem.Mana * manaCost))
						{
							statMana -= (int)((float)selectedItem.Mana * manaCost);
						}
						else
						{
							flag = false;
						}
					}
				}

				if (selectedItem.Type == 43 && Main.dayTime)
				{
					flag = false;
				}
				if (selectedItem.Type == 70 && !this.zoneEvil)
				{
					flag = false;
				}

				if (flag)
				{
					if (selectedItem.UseAmmo > 0)
					{
						flag = false;
						for (int l = 0; l < 44; l++)
						{
							if (l < 4 && this.ammo[l].Ammo == selectedItem.UseAmmo && this.ammo[l].Stack > 0)
							{
								flag = true;
								break;
							}
							if (this.inventory[l].Ammo == selectedItem.UseAmmo && this.inventory[l].Stack > 0)
							{
								flag = true;
								break;
							}
						}
					}
				}

				if (flag)
				{
					if (grappling[0] > -1)
					{
						if (controlRight)
						{
							direction = 1;
						}
						else if (controlLeft)
						{
							direction = -1;
						}
					}

					channel = selectedItem.Channel;
					attackCD = 0;

					if (selectedItem.Melee)
					{
						this.itemAnimation = (int)((float)selectedItem.UseAnimation * this.meleeSpeed);
						this.itemAnimationMax = (int)((float)selectedItem.UseAnimation * this.meleeSpeed);
					}
					else
					{
						this.itemAnimation = selectedItem.UseAnimation;
						this.itemAnimationMax = selectedItem.UseAnimation;
						ReUseDelay = selectedItem.ReUseDelay;
					}
				}

				if (flag && selectedItem.Shoot == ProjectileType.N18_ORB_OF_LIGHT)
				{
					for (int j = 0; j < 1000; j++)
					{
						if (Main.projectile[j].Active && Main.projectile[j].Owner == i && Main.projectile[j].type == selectedItem.Shoot)
						{
							Main.projectile[j].Kill(TileRefs, sandbox);
						}
					}
				}
			}

			if (!this.controlUseItem)
			{
				this.channel = false;
			}

			if (this.itemAnimation > 0)
			{
				if (selectedItem.Mana > 0)
				{
					this.manaRegenDelay = (int)this.maxRegenDelay;
				}

				itemHeight = selectedItem.Height;
				itemWidth = selectedItem.Width;
				itemAnimation--;
			}
			else if (selectedItem.HoldStyle == 1)
			{
				this.itemLocation.X = this.Position.X + (float)this.Width * 0.5f + 20f * (float)this.direction;

				this.itemLocation.Y = this.Position.Y + 24f;
				this.itemRotation = 0f;
				if (this.gravDir == -1f)
				{
					this.itemRotation = -this.itemRotation;
					this.itemLocation.Y = this.Position.Y + (float)this.Height + (this.Position.Y - this.itemLocation.Y);
				}
			}
			else if (selectedItem.HoldStyle == 2)
			{
				this.itemLocation.X = this.Position.X + (float)this.Width * 0.5f + (float)(6 * this.direction);
				this.itemLocation.Y = this.Position.Y + 16f;
				this.itemRotation = 0.79f * (float)(-(float)this.direction);
				if (this.gravDir == -1f)
				{
					this.itemRotation = -this.itemRotation;
					this.itemLocation.Y = this.Position.Y + (float)this.Height + (this.Position.Y - this.itemLocation.Y);
				}
			}

			releaseUseItem = !controlUseItem;

			if (this.itemTime > 0)
			{
				this.itemTime--;
			}
			if (i == Main.myPlayer)
			{
				if (selectedItem.Shoot > 0 && itemAnimation > 0 && itemTime == 0)
				{
					ProjectileType shoot = selectedItem.Shoot;
					float shootSpeed = selectedItem.ShootSpeed;

					if (selectedItem.Melee && shoot != ProjectileType.N25_BALL_O_HURT && shoot !=
						ProjectileType.N26_BLUE_MOON && shoot != ProjectileType.N35_SUNFURY)
					{
						shootSpeed /= this.meleeSpeed;
					}

					bool flag2 = false;
					int damage = dmg;
					float knockBack = selectedItem.KnockBack;
					if (shoot == ProjectileType.N13_HOOK || shoot == ProjectileType.N32_IVY_WHIP)
					{
						grappling[0] = -1;
						grapCount = 0;
						for (int j = 0; j < Main.maxProjectiles; j++)
						{
							if (Main.projectile[j].Active && Main.projectile[j].Owner == i)
							{
								if (Main.projectile[j].type == ProjectileType.N13_HOOK)
								{
									Main.projectile[j].Kill(TileRefs, sandbox);
								}
							}
						}
					}

					if (selectedItem.UseAmmo > 0)
					{
						Item item = null;
						bool flag3 = false;
						for (int num12 = 0; num12 < 4; num12++)
						{
							if (this.ammo[num12].Ammo == selectedItem.UseAmmo && this.ammo[num12].Stack > 0)
							{
								item = this.ammo[num12];
								flag2 = true;
								flag3 = true;
								break;
							}
						}
						if (!flag3)
						{
							for (int num13 = 0; num13 < 44; num13++)
							{
								if (this.inventory[num13].Ammo == selectedItem.UseAmmo && this.inventory[num13].Stack > 0)
								{
									item = this.inventory[num13];
									flag2 = true;
									break;
								}
							}
						}
						if (flag2)
						{
							if (item != null && item.Shoot != 0)
							{
								shoot = item.Shoot;
							}
							shootSpeed += item.ShootSpeed;
							if (item.Ranged)
							{
								damage += (int)((float)item.damage * this.rangedDamage);
							}
							else
							{
								damage += item.damage;
							}

							if (selectedItem.UseAmmo == ProjectileType.N1_WOODEN_ARROW && this.archery)
							{
								if (shootSpeed < 20f)
								{
									shootSpeed *= 1.2f;
									if (shootSpeed > 20f)
									{
										shootSpeed = 20f;
									}
								}
								damage = (int)((double)((float)damage) * 1.2);
							}
							if (item != null) knockBack += item.KnockBack;
							bool flag4 = false;
							if (selectedItem.Type == 98 && Main.rand.Next(3) == 0)
							{
								flag4 = true;
							}
							if (this.ammoCost80 && Main.rand.Next(5) == 0)
							{
								flag4 = true;
							}
							if (!flag4 && item != null)
							{
								item.Stack--;
								if (item.Stack <= 0)
								{
									item.Active = false;
									item.Name = "";
									item.Type = 0;
								}
							}
						}
					}
					else
					{
						flag2 = true;
					}

					if (flag2)
					{
						if (damage == 1 && selectedItem.Type == 120)
						{
							damage = 2;
						}
					}
					else if (selectedItem.UseStyle == 5)
					{
						itemRotation = 0f;
						//NetMessage.SendData(41, -1, -1, "", this.whoAmi);
					}
				}

				if (selectedItem.Type == 29 && this.itemAnimation > 0 && this.statLifeMax < 400 && this.itemTime == 0)
				{
					if (this.itemTime == 0)
					{
						this.itemTime = selectedItem.UseTime;
						this.statLifeMax += 20;
						this.statLife += 20;
						if (Main.myPlayer == this.whoAmi)
						{
							this.HealEffect(20);
						}
					}
				}
				if (selectedItem.Type == 109 && this.itemAnimation > 0 && this.statManaMax < 200 && this.itemTime == 0)
				{
					if (this.itemTime == 0)
					{
						this.itemTime = selectedItem.UseTime;
						this.statManaMax += 20;
						this.statMana += 20;
						if (Main.myPlayer == this.whoAmi)
						{
							this.ManaEffect(20);
						}
					}
				}
			}
			if (selectedItem.damage >= 0 && selectedItem.Type > 0 && !selectedItem.NoMelee)
			{
				if (this.itemAnimation > 0)
				{
					//bool flag5 = false;
					Rectangle rectangle = new Rectangle((int)this.itemLocation.X, (int)this.itemLocation.Y, 32, 32);
					rectangle.Width = (int)((float)rectangle.Width * selectedItem.scale);
					rectangle.Height = (int)((float)rectangle.Height * selectedItem.scale);
					if (this.direction == -1)
					{
						rectangle.X -= rectangle.Width;
					}
					if (this.gravDir == 1f)
					{
						rectangle.Y -= rectangle.Height;
					}
					rectangle.Y -= rectangle.Height;
					if (selectedItem.UseStyle == 1)
					{
						if ((double)this.itemAnimation < (double)this.itemAnimationMax * 0.333)
						{
							if (this.direction == -1)
							{
								rectangle.X -= (int)((double)rectangle.Width * 1.4 - (double)rectangle.Width);
							}
							rectangle.Width = (int)((double)rectangle.Width * 1.4);
							rectangle.Y += (int)((double)rectangle.Height * 0.5 * (double)this.gravDir);
							rectangle.Height = (int)((double)rectangle.Height * 1.1);
						}
						else
						{
							if ((double)this.itemAnimation >= (double)this.itemAnimationMax * 0.666)
							{
								if (this.direction == 1)
								{
									rectangle.X -= (int)((double)rectangle.Width * 1.2);
								}
								rectangle.Width *= 2;
								rectangle.Y -= (int)(((double)rectangle.Height * 1.4 - (double)rectangle.Height) * (double)this.gravDir);
								rectangle.Height = (int)((double)rectangle.Height * 1.4);
							}
						}
					}
					else if (selectedItem.UseStyle == 3)
					{
						if (!((double)this.itemAnimation > (double)this.itemAnimationMax * 0.666))
						{
							if (this.direction == -1)
							{
								rectangle.X -= (int)((double)rectangle.Width * 1.4 - (double)rectangle.Width);
							}
							rectangle.Width = (int)((double)rectangle.Width * 1.4);
							rectangle.Y += (int)((double)rectangle.Height * 0.6);
							rectangle.Height = (int)((double)rectangle.Height * 0.6);
						}
					}
				}
			}
			if (this.itemTime == 0 && this.itemAnimation > 0)
			{
				if (selectedItem.HealLife > 0)
				{
					this.statLife += selectedItem.HealLife;
					this.itemTime = selectedItem.UseTime;
					if (Main.myPlayer == this.whoAmi)
					{
						this.HealEffect(selectedItem.HealLife);
					}
				}
				if (selectedItem.HealMana > 0)
				{
					this.statMana += selectedItem.HealMana;
					this.itemTime = selectedItem.UseTime;
					if (Main.myPlayer == this.whoAmi)
					{
						this.ManaEffect(selectedItem.HealMana);
					}
				}
				if (selectedItem.BuffType > 0)
				{
					if (this.whoAmi == Main.myPlayer)
					{
						this.AddBuff(selectedItem.BuffType, selectedItem.BuffTime, true);
					}
					this.itemTime = selectedItem.UseTime;
				}
			}
			if (this.itemTime == 0 && this.itemAnimation > 0 && selectedItem.Type == 361)
			{
				this.itemTime = selectedItem.UseTime;
				if (Main.invasionType == 0)
				{
					var ctx = new HookContext
					{
						Connection = Connection,
						Sender = this,
						Player = this,
					};

					var args = new HookArgs.PlayerTriggeredEvent
					{
						Type = WorldEventType.INVASION,
					};

					HookPoints.PlayerTriggeredEvent.Invoke(ref ctx, ref args);

					if (ctx.CheckForKick())
						return;
					else if (ctx.Result != HookResult.IGNORE)
					{
						ProgramLog.Users.Log("{0} @ {1}: Invasion triggered by {2}.", IPAddress, whoAmi, Name);
						NetMessage.SendData(Packet.PLAYER_CHAT, -1, -1, string.Concat(Name, " has summoned an invasion!"), 255, 255, 128, 150);
						Main.invasionDelay = 0;
						Main.StartInvasion();
					}
				}
			}
			if (this.itemTime == 0 && this.itemAnimation > 0 && (selectedItem.Type == 43 || selectedItem.Type == 70))
			{
				this.itemTime = selectedItem.UseTime;

				if ((selectedItem.Type == 43 && !Main.dayTime) || (selectedItem.Type == 70 && zoneEvil))
				{
					/*var ctx = new HookContext
					{
						Connection = Connection,
						Sender = this,
						Player = this,
					};

					var args = new HookArgs.PlayerTriggeredEvent
					{
						X = (int)(Position.X / 16),
						Y = (int)(Position.Y / 16),
						Type = WorldEventType.BOSS,
						Name = selectedItem.Type == 43 ? "Eye of Cthulhu" : "Eater of Worlds",
					};

					HookPoints.PlayerTriggeredEvent.Invoke(ref ctx, ref args);

					if (ctx.CheckForKick())
						return;
					else if (ctx.Result != HookResult.IGNORE)
					{
						if (selectedItem.Type == 43)
						{
							ProgramLog.Users.Log("{0} @ {1}: Eye of Cthulhu summoned by {2}.", IPAddress, whoAmi, Name);
							NetMessage.SendData(Packet.PLAYER_CHAT, -1, -1, string.Concat(Name, " has summoned the Eye of Cthulhu!"), 255, 255, 128, 150);
							NPC.SpawnOnPlayer(i, 4);
						}
						else if (selectedItem.Type == 70)
						{
							ProgramLog.Users.Log("{0} @ {1}: Eater of Worlds summoned by {2}.", IPAddress, whoAmi, Name);
							NetMessage.SendData(Packet.PLAYER_CHAT, -1, -1, string.Concat(Name, " has summoned the Eater of Worlds!"), 255, 255, 128, 150);
							NPC.SpawnOnPlayer(i, 13);
						}
					}*/
				}
			}

			if (selectedItem.Type == 50 && this.itemAnimation > 0)
			{
				if (this.itemTime == 0)
				{
					this.itemTime = selectedItem.UseTime;
				}
				else if (this.itemTime == selectedItem.UseTime / 2)
				{
					this.grappling[0] = -1;
					this.grapCount = 0;
					for (int j = 0; j < 1000; j++)
					{
						if (Main.projectile[j].Active && Main.projectile[j].Owner == i)
						{
							if (Main.projectile[j].aiStyle == 7)
							{
								Main.projectile[j].Kill(TileRefs, sandbox);
							}
						}
					}
					this.Spawn(TileRefs, sandbox);
				}
			}
		}

		/// <summary>
		/// Clones player values
		/// </summary>
		/// <returns>Cloned player</returns>
		public override object Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>
		/// Creates a player clone
		/// </summary>
		/// <returns>Cloned player</returns>
		public object clientClone()
		{
			Player player = new Player();
			player.zoneEvil = this.zoneEvil;
			player.zoneMeteor = this.zoneMeteor;
			player.zoneDungeon = this.zoneDungeon;
			player.zoneJungle = this.zoneJungle;
			player.direction = this.direction;
			player.selectedItemIndex = this.selectedItemIndex;
			player.controlUp = this.controlUp;
			player.controlDown = this.controlDown;
			player.controlLeft = this.controlLeft;
			player.controlRight = this.controlRight;
			player.controlJump = this.controlJump;
			player.controlUseItem = this.controlUseItem;
			player.statLife = this.statLife;
			player.statLifeMax = this.statLifeMax;
			player.statMana = this.statMana;
			player.statManaMax = this.statManaMax;
			player.Position.X = this.Position.X;
			player.chest = this.chest;
			player.talkNPC = this.talkNPC;
			for (int i = 0; i < MAX_INVENTORY; i++)
			{
				player.inventory[i] = (Item)this.inventory[i].Clone();
				if (i < 8)
				{
					player.armor[i] = (Item)this.armor[i].Clone();
				}
			}
			return player;
		}

		/// <summary>
		/// Checks whether location is suitable for assigning to spawn
		/// </summary>
		/// <param name="TileRefs">Reference to the ITile method, For usage between Sandbox and Realtime</param>
		/// <param name="x">X value of location</param>
		/// <param name="y">Y value of location</param>
		/// <returns>True if location can be set as spawn</returns>
		public static bool CheckSpawn(Func<Int32, Int32, ITile> TileRefs, int x, int y)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (x < 10 || x > Main.maxTilesX - 10 || y < 10 || y > Main.maxTilesX - 10)
			{
				return false;
			}
			if (!TileRefs(x, y - 1).Active || TileRefs(x, y - 1).Type != 79)
			{
				return false;
			}
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = y - 3; j < y; j++)
				{
					if (!TileRefs(i, j).Exists)
					{
						return false;
					}
					if (TileRefs(i, j).Active && Main.tileSolid[(int)TileRefs(i, j).Type] && !Main.tileSolidTop[(int)TileRefs(i, j).Type])
					{
						return false;
					}
				}
			}
			return WorldModify.StartRoomCheck(TileRefs, x, y - 1);
		}

		// client only
		//        public void FindSpawn()
		//		{
		//			for (int i = 0; i < 200; i++)
		//			{
		//				if (this.spN[i] == null)
		//				{
		//					this.SpawnX = -1;
		//					this.SpawnY = -1;
		//					return;
		//				}
		//				if (this.spN[i] == Main.worldName && this.spI[i] == Main.worldID)
		//				{
		//					this.SpawnX = this.spX[i];
		//					this.SpawnY = this.spY[i];
		//					return;
		//				}
		//			}
		//		}

		/// <summary>
		/// Changes player's spawn point
		/// </summary>
		/// <param name="x">New X spawn coordinate</param>
		/// <param name="y">New Y spawn coordinate</param>
		public void ChangeSpawn(int x, int y)
		{
			SpawnX = x;
			SpawnY = y;
			// this is client stuff for remembering spawn Positions for different worlds
			//			int num = 0;
			//			while (num < 200 && this.spN[num] != null)
			//			{
			//				if (this.spN[num] == Main.worldName && this.spI[num] == Main.worldID)
			//				{
			//					for (int i = num; i > 0; i--)
			//					{
			//						this.spN[i] = this.spN[i - 1];
			//						this.spI[i] = this.spI[i - 1];
			//						this.spX[i] = this.spX[i - 1];
			//						this.spY[i] = this.spY[i - 1];
			//					}
			//					this.spN[0] = Main.worldName;
			//					this.spI[0] = Main.worldID;
			//					this.spX[0] = x;
			//					this.spY[0] = y;
			//					return;
			//				}
			//				num++;
			//			}
			//			for (int j = 199; j > 0; j--)
			//			{
			//				if (this.spN[j - 1] != null)
			//				{
			//					this.spN[j] = this.spN[j - 1];
			//					this.spI[j] = this.spI[j - 1];
			//					this.spX[j] = this.spX[j - 1];
			//					this.spY[j] = this.spY[j - 1];
			//				}
			//			}
			//			this.spN[0] = Main.worldName;
			//			this.spI[0] = Main.worldID;
			//			this.spX[0] = x;
			//			this.spY[0] = y;
		}

		/// <summary>
		/// Checks whether player is carrying specified item
		/// TODO: include checking inside piggy banks
		/// </summary>
		/// <param name="Type">Item type to check for</param>
		/// <returns>True if player is carrying, false if not</returns>
		public bool HasItem(int Type)
		{
			for (int i = 0; i < MAX_INVENTORY; i++)
			{
				if (Type == this.inventory[i].Type)
				{
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Get death message string of falling related causes.
		/// </summary>
		/// <returns></returns>
		public static string GetDeathMessageForGround()
		{
			string deathMessage = "";
			int randomMessage = Main.rand.Next(5);

			switch (randomMessage)
			{
				case 0:
					deathMessage = " fell to their death.";
					break;
				case 1:
					deathMessage = " faceplanted the ground.";
					break;
				case 2:
					deathMessage = " fell victim to gravity.";
					break;
				case 3:
					deathMessage = " left a small crater.";
					break;
				case 4:
					deathMessage = " didn't bounce.";
					break;
			}

			return deathMessage;
		}


		/// <summary>
		/// Get death message string of water related causes.
		/// </summary>
		/// <returns></returns>
		public static string GetDeathMessageForWater()
		{
			string deathMessage = "";
			int randomMessage = Main.rand.Next(4);

			switch (randomMessage)
			{
				case 0:
					deathMessage = " forgot to breathe.";
					break;
				case 1:
					deathMessage = " is sleeping with the fish.";
					break;
				case 2:
					deathMessage = " drowned.";
					break;
				case 3:
					deathMessage = " is shark food.";
					break;
			}

			return deathMessage;
		}

		/// <summary>
		/// Get death message string of lava related causes.
		/// </summary>
		/// <returns></returns>
		public static string GetDeathMessageForLava()
		{
			string deathMessage = "";
			int randomMessage = Main.rand.Next(4);

			switch (randomMessage)
			{
				case 0:
					deathMessage = " got melted.";
					break;
				case 1:
					deathMessage = " was incinerated.";
					break;
				case 2:
					deathMessage = " tried to swim in lava.";
					break;
				case 3:
					deathMessage = " likes to play in magma.";
					break;
			}

			return deathMessage;
		}

		/// <summary>
		/// Get death message string of standard death reason
		/// </summary>
		/// <param name="plr">Player index</param>
		/// <param name="npc">NPC type</param>
		/// <param name="proj">Projectile type</param>
		/// <param name="other">0 = fall, 1 = drown, 2 = lava</param>
		/// <returns></returns>
		public static string getDeathMessage(int plr = -1, int npc = -1, int proj = -1, int other = -1)
		{
			string deathMessage = "";
			int randomDeath = Main.rand.Next(11);
			string deathText = "";

			switch (randomDeath)
			{
				case 0:
					deathText = " was slain";
					break;
				case 1:
					deathText = " was eviscerated";
					break;
				case 2:
					deathText = " was murdered";
					break;
				case 3:
					deathText = "'s face was torn off";
					break;
				case 4:
					deathText = "'s entrails were ripped out";
					break;
				case 5:
					deathText = " was destroyed";
					break;
				case 6:
					deathText = "'s skull was crushed";
					break;
				case 7:
					deathText = " got massacred";
					break;
				case 8:
					deathText = " got impaled";
					break;
				case 9:
					deathText = " was torn in half";
					break;
				case 10:
					deathText = " was decapitated";
					break;
			}

			if (plr >= 0 && plr < 255)
			{
				if (proj >= 0 && Main.projectile[proj].Name != "")
					deathMessage = String.Format("{0} by {1}'s {2}.", deathText, Main.players[plr].Name, Main.projectile[proj].Name);
				else
					deathMessage = String.Format("{0} by {1}'s {2}.", deathText, Main.players[plr].Name, Main.players[plr].inventory[Main.players[plr].selectedItemIndex].Name);
			}
			else if (npc >= 0 && Main.npcs[npc].Name != "")
				deathMessage = String.Format("{0} by {1}.", deathText, Main.npcs[npc].Name);

			else if (proj >= 0 && Main.projectile[proj].Name != "")
				deathMessage = String.Format("{0} by {1}.", deathText, Main.projectile[proj].Name);

			else if (other >= 0)
			{
				switch (other)
				{
					case 0:
						deathMessage = GetDeathMessageForGround();
						break;
					case 1:
						deathMessage = GetDeathMessageForWater();
						break;
					case 2:
						deathMessage = GetDeathMessageForLava();
						break;
					case 3:
						deathMessage = deathText + ".";
						break;
				}
			}
			return deathMessage;
		}

		/// <summary>
		/// Gets current server slot player is assigned to
		/// </summary>
		public ServerSlot Slot
		{
			get
			{
				var whoAmi = this.whoAmi;
				if (whoAmi >= 0)
					return NetPlay.slots[whoAmi];
				else
					return null;
			}
		}

		/// <summary>
		/// Kicks player
		/// </summary>
		/// <param name="reason">Reason for kick</param>
		public void Kick(string reason = null)
		{
			var conn = Connection;
			if (conn != null)
			{
				var message = Languages.Disconnected;

				if (reason != null)
				{
					message = reason;
				}

				conn.Kick(message);
			}
		}

		/// <summary>
		/// Get/Set for player's IP address
		/// </summary>
		public string IPAddress
		{
			get
			{
				return ipAddress;
			}
			set
			{
				ipAddress = value;
			}
		}

		/// <summary>
		/// Player's position in tile coordinate format
		/// </summary>
		public Vector2 TileLocation
		{
			get
			{
				return new Vector2(Position.X / 16, Position.Y / 16);
			}
			set
			{
				Position.X = value.X * 16;
				Position.Y = value.Y * 16;
			}
		}

		/// <summary>
		/// Player's position
		/// </summary>
		public Vector2 Location
		{
			get
			{
				return Position;
			}
			set
			{
				Position = value;
			}
		}

		/// <summary>
		/// Get/Set for allowing bed destruction
		/// </summary>
		public bool AllowBedDestroy
		{
			get
			{
				return bedDestruction;
			}
			set
			{
				bedDestruction = value;
			}
		}
		/// <summary>
		/// Teleports player to specified location
		/// </summary>
		/// <param name="tx">X pixel coordinate to teleport to</param>
		/// <param name="ty">Y pixel coordinate to teleport to</param>
		/// <returns>True on success, false on failure</returns>
		public bool Teleport(float tx, float ty)
		{
			return Teleport((int)(tx / 16), (int)(ty / 16));
		}

		//[Obsolete("Renamed to Player.Teleport")]
		//public bool teleportTo (float tx, float ty)
		//{
		//    return Teleport ((int) (tx / 16), (int) (ty / 16));
		//}

		private int teleportInProgress;

		public void TeleportDone()
		{
			TeleRetries = 0;
			TeleSpawnX = -1;
			TeleSpawnY = -1;
			System.Threading.Interlocked.CompareExchange(ref this.teleportInProgress, 0, 1);
		}

		public bool Respawn()
		{
			if (Main.players[whoAmi] != this) return false;
			if (System.Threading.Interlocked.CompareExchange(ref this.teleportInProgress, 1, 0) != 0) return false;

			TeleSpawnX = -1;
			TeleSpawnY = -1;
			TeleRetries = 0;

			var msg = NetMessage.PrepareThreadInstance();
			msg.ReceivingPlayerJoined(whoAmi);
			msg.Send(whoAmi);

			return true;
		}

		public bool Teleport(int tx, int ty)
		{
			return Teleport(tx, ty, false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Location">Location in to Teleport to</param>
		/// <param name="TileFormat">Whether to convert to Tile Format</param>
		/// <returns></returns>
		public bool Teleport(Vector2 Location, bool TileFormat = true)
		{
			if (!TileFormat)
				return Teleport(Location.X, Location.Y);
			else
				return Teleport((int)Location.X, (int)Location.Y, false);
		}

		internal bool Teleport(int tx, int ty, bool retrying)
		{
			if (Main.players[whoAmi] != this)
			{
				ProgramLog.Error.Log("Attempt to teleport inactive player {0}.", Name ?? IPAddress);
				return false;
			}

			if (!retrying && System.Threading.Interlocked.CompareExchange(ref this.teleportInProgress, 1, 0) != 0)
			{
				ProgramLog.Error.Log("Teleportation of player {0} already in progress.", Name ?? IPAddress);
				return false;
			}

			if (tx < 0 || ty < 0 || tx >= Main.maxTilesX || ty >= Main.maxTilesY)
			{
				ProgramLog.Error.Log("Attempt to teleport player {0} to invalid location: {1}, {2}.", Name ?? IPAddress, tx, ty);
				return false;
			}

			var ctx = new HookContext
			{
				Connection = this.Connection,
				Player = this,
				Sender = this,
			};

			var args = new HookArgs.PlayerTeleport
			{
				ToLocation = new Vector2(tx, ty)
			};

			HookPoints.PlayerTeleport.Invoke(ref ctx, ref args);

			if (ctx.Result == HookResult.IGNORE)
				return false;

			bool changeSpawn = false;

			int ox = Main.spawnTileX;
			int oy = Main.spawnTileY;
			if (SpawnX >= 0 && SpawnY >= 0)
			{
				changeSpawn = true;
				ox = SpawnX;
				oy = SpawnY;
			}
			else if (OldSpawnX >= 0 && OldSpawnY >= 0)
			{
				changeSpawn = true;
				ox = OldSpawnX;
				oy = OldSpawnY;
			}

			var slot = NetPlay.slots[whoAmi];
			int sx = tx / 200;
			int sy = ty / 150;

			// send up to 9 sections around the player
			int fromX = Math.Max(0, sx - 1);
			int fromY = Math.Max(0, sy - 1);
			int toX = Math.Min(sx + 1, Main.maxTilesX / 200 - 1);
			int toY = Math.Min(sy + 1, Main.maxTilesY / 150 - 1);

			int sections = 0;

			for (int x = fromX; x <= toX; x++)
			{
				for (int y = fromY; y <= toY; y++)
				{
					if (!slot.tileSection[x, y])
					{
						sections += 1;
					}
				}
			}

			var msg = NetMessage.PrepareThreadInstance();

			if (sections > 0)
			{
				msg.SendTileLoading(sections * 150, "Teleporting...");
				msg.Send(whoAmi);
				msg.Clear();

				for (int x = fromX; x <= toX; x++)
				{
					for (int y = fromY; y <= toY; y++)
					{
						if (!slot.tileSection[x, y])
						{
							NetMessage.SendSection(whoAmi, x, y);
						}
					}
				}

				msg.SendTileConfirm(fromX, fromY, toX, toY);
				msg.Send(whoAmi);
				msg.Clear();
			}

			// kill players' hooks and vines
			foreach (var proj in Main.projectile)
			{
				if (proj.Active && proj.Owner == whoAmi
					&& (proj.type == ProjectileType.N13_HOOK || proj.type == ProjectileType.N32_IVY_WHIP))
				{
					proj.Active = false;
					proj.type = ProjectileType.N0_UNKNOWN;
					msg.Projectile(proj);
				}
			}
			msg.Broadcast();
			msg.Clear();

			int left = 0;
			int right = -1;
			if (changeSpawn && oy > 1)
			{
				// invalidate player's bed temporarily
				// we used to kill a tile under the bed, but that could cause
				// side-effects, like killing objects hanging from the ceiling below

				left = Math.Max(0, ox - 4);
				right = Math.Min(ox + 4, Main.maxTilesX);

				while (left < Main.maxTilesX && Main.tile.At(left, oy - 1).Type != 79) left += 1;
				while (right > 0 && Main.tile.At(right, oy - 1).Type != 79) right -= 1;

				for (int x = left; x <= right; x++)
				{
					var data = Main.tile.At(x, oy - 1).Data;
					data.Active = false;
					msg.SingleTileSquare(x, oy - 1, data);
					data = Main.tile.At(x, oy - 2).Data;
					data.Active = false;
					msg.SingleTileSquare(x, oy - 2, data);
				}
			}

			// change the global spawn point
			msg.WorldData(tx, ty);

			// trigger respawn
			TeleSpawnX = tx;
			TeleSpawnY = ty;
			msg.ReceivingPlayerJoined(whoAmi);

			// fix holes at target location
			int fx = Math.Max(0, Math.Min(Main.maxTilesX - 8, tx - 4));
			int fy = Math.Max(0, Math.Min(Main.maxTilesY - 8, ty - 4));
			msg.TileSquare(7, fx, fy);

			//			msg.Send (whoAmi);
			//			msg.Clear ();

			if (changeSpawn && oy > 1)
			{
				// restore player's bed
				msg.TileSquare(1, ox, oy);

				if (right - left >= 0 && oy >= 2)
					msg.TileSquare(right - left + 1, left, oy - 2);
			}

			// restore the global spawn point
			msg.WorldData();

			msg.Send(whoAmi);

			return true;
		}

		/// <summary>
		/// Teleports player to specified player
		/// </summary>
		/// <param name="player">Player to teleport to</param>
		public bool Teleport(Player player)
		{
			return Teleport((int)(player.Position.X / 16), (int)(player.Position.Y / 16));
		}

		//[Obsolete("Renamed to Player.Teleport")]
		//public void teleportTo (Player player)
		//{
		//    Teleport ((int) (player.Position.X / 16), (int) (player.Position.Y / 16));
		//}

		/// <summary>
		/// Gets a player's server password
		/// </summary>
		/// <param name="PlayerName">Player's name</param>
		/// <returns>Password string</returns>
		public static string GetPlayerPassword(string PlayerName)
		{
			foreach (string listee in Server.OpList.WhiteList)
			{
				if (listee != null && listee.Trim().ToLower().Length > 0)
				{
					string userPass = listee.Trim().ToLower();
					if (userPass.Contains(":"))
					{
						if (userPass.Split(':')[0] == PlayerName.Trim().ToLower())
						{
							return userPass.Split(':')[1];
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Sets player's password
		/// </summary>
		public string Password
		{
			get
			{
				return Player.GetPlayerPassword(this.Name);
			}
		}

		/// <summary>
		/// Checks player's op status
		/// </summary>
		/// <param name="Name">Player's name</param>
		/// <returns>True if op, false if not</returns>
		public static bool isInOpList(string Name)
		{
			foreach (string listee in Server.OpList.WhiteList)
			{
				if (listee != null && listee.Trim().ToLower().Length > 0)
				{
					string userPass = listee.Trim().ToLower();
					if (userPass.Contains(":"))
					{
						if (userPass.Split(':')[0] == Name.Trim().ToLower())
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Easy call for op check
		/// </summary>
		/// <returns>True if op, false if not</returns>
		public bool isInOpList()
		{
			return Player.isInOpList(this.Name);
		}

		/// <summary>
		/// Gets players op password
		/// </summary>
		public string GetOpListKey
		{
			get
			{
				return this.Name.Trim().ToLower() + Password;
			}
		}

		/// <summary>
		/// Checks whether player is hacking health or mana
		/// TODO: add other hack checks?
		/// </summary>
		/// <returns></returns>
		public bool HasHackedData()
		{
			if (!Program.properties.HackedData)
			{
				if (statMana > MAX_MANA || statManaMax > MAX_MANA || statLife > MAX_HEALTH || statLifeMax > MAX_HEALTH)
					return true;

				foreach (Item item in inventory)
				{
					if (item.Stack > MAX_ITEMS)
						return true;
				}
			}
			return false;
		}

		public int GiveItem(int ItemId, int Stack, ISender sender, int NetID, bool NotifyOps = true, int prefix = 0)
		{
			var index = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, ItemId, Stack, false, prefix, NetID);

			if (NotifyOps)
				Server.notifyOps("Giving " + this.Name + " some " + ItemId.ToString() + " [" + sender.Name + "]", true);

			return index;
		}

		public int ReUseDelay { get; set; }
	}
}
