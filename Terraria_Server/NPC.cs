
using System;
namespace Terraria_Server
{
	public class NPC
	{
		public static int immuneTime = 20;
		public static int maxAI = 4;
		private static int spawnSpaceX = 4;
		private static int spawnSpaceY = 4;
		private static int spawnRangeX = (int)((double)(Main.screenWidth / 16) * 1.2);
		private static int spawnRangeY = (int)((double)(Main.screenHeight / 16) * 1.2);
		public static int safeRangeX = (int)((double)(Main.screenWidth / 16) * 0.55);
		public static int safeRangeY = (int)((double)(Main.screenHeight / 16) * 0.55);
		private static int activeRangeX = Main.screenWidth * 2;
		private static int activeRangeY = Main.screenHeight * 2;
		private static int townRangeX = Main.screenWidth * 3;
		private static int townRangeY = Main.screenHeight * 3;
		private static int activeTime = 1000;
		private static int defaultSpawnRate = 700;
		private static int defaultMaxSpawns = 4;
		public bool wet;
		public byte wetCount;
		public bool lavaWet;
		public static bool downedBoss1 = false;
		public static bool downedBoss2 = false;
		public static bool downedBoss3 = false;
		private static int spawnRate = NPC.defaultSpawnRate;
		private static int maxSpawns = NPC.defaultMaxSpawns;
		public int soundDelay;
        public Vector2 position = new Vector2();
        public Vector2 velocity = new Vector2();
        public Vector2 oldPosition = new Vector2();
        public Vector2 oldVelocity = new Vector2();
		public int width;
		public int height;
		public bool active;
		public int[] immune = new int[256];
		public int direction = 1;
		public int directionY = 1;
		public int type;
		public float[] ai = new float[NPC.maxAI];
		public int aiAction;
		public int aiStyle;
		public int timeLeft;
		public int target = -1;
		public int damage;
		public int defense;
		public int soundHit;
		public int soundKilled;
		public int life;
		public int lifeMax;
        public Rectangle targetRect = new Rectangle();
		public double frameCounter;
		public Rectangle frame = new Rectangle();
		public string name;
		public Color color;
		public int alpha;
		public float scale = 1f;
		public float knockBackResist = 1f;
		public int oldDirection;
		public int oldDirectionY;
		public int oldTarget;
		public int whoAmI;
		public float rotation;
		public bool noGravity;
		public bool noTileCollide;
		public bool netUpdate;
		public bool collideX;
		public bool collideY;
		public bool boss;
		public int spriteDirection = -1;
		public bool behindTiles;
		public float value;
		public bool townNPC;
		public bool homeless;
		public int homeTileX = -1;
		public int homeTileY = -1;
		public bool friendly;
		public bool closeDoor;
		public int doorX;
		public int doorY;
		public int friendlyRegen;
		public void SetDefaults(string Name)
		{
			this.SetDefaults(0);
			if (Name == "Green Slime")
			{
				this.SetDefaults(1);
				this.name = Name;
				this.scale = 0.9f;
				this.damage = 8;
				this.defense = 2;
				this.life = 15;
				this.knockBackResist = 1.1f;
				this.color = new Color(0, 220, 40, 100);
				this.value = 3f;
			}
			else
			{
				if (Name == "Pinky")
				{
					this.SetDefaults(1);
					this.name = Name;
					this.scale = 0.6f;
					this.damage = 5;
					this.defense = 5;
					this.life = 150;
					this.knockBackResist = 1.4f;
					this.color = new Color(250, 30, 90, 90);
					this.value = 10000f;
				}
				else
				{
					if (Name == "Baby Slime")
					{
						this.SetDefaults(1);
						this.name = Name;
						this.scale = 0.9f;
						this.damage = 13;
						this.defense = 4;
						this.life = 30;
						this.knockBackResist = 0.95f;
						this.alpha = 120;
						this.color = new Color(0, 0, 0, 50);
						this.value = 10f;
					}
					else
					{
						if (Name == "Black Slime")
						{
							this.SetDefaults(1);
							this.name = Name;
							this.damage = 15;
							this.defense = 4;
							this.life = 45;
							this.color = new Color(0, 0, 0, 50);
							this.value = 20f;
						}
						else
						{
							if (Name == "Purple Slime")
							{
								this.SetDefaults(1);
								this.name = Name;
								this.scale = 1.2f;
								this.damage = 12;
								this.defense = 6;
								this.life = 40;
								this.knockBackResist = 0.9f;
								this.color = new Color(200, 0, 255, 150);
								this.value = 10f;
							}
							else
							{
								if (Name == "Red Slime")
								{
									this.SetDefaults(1);
									this.name = Name;
									this.damage = 12;
									this.defense = 4;
									this.life = 35;
									this.color = new Color(255, 30, 0, 100);
									this.value = 8f;
								}
								else
								{
									if (Name == "Yellow Slime")
									{
										this.SetDefaults(1);
										this.name = Name;
										this.scale = 1.2f;
										this.damage = 15;
										this.defense = 7;
										this.life = 45;
										this.color = new Color(255, 255, 0, 100);
										this.value = 10f;
									}
									else
									{
										if (Name != "")
										{
											for (int i = 1; i < 46; i++)
											{
												this.SetDefaults(i);
												if (this.name == Name)
												{
													break;
												}
												if (i == 45)
												{
													this.SetDefaults(0);
													this.active = false;
												}
											}
										}
										else
										{
											this.active = false;
										}
									}
								}
							}
						}
					}
				}
			}
			this.lifeMax = this.life;
		}
		public void SetDefaults(int Type)
		{
			this.lavaWet = false;
			this.wetCount = 0;
			this.wet = false;
			this.townNPC = false;
			this.homeless = false;
			this.homeTileX = -1;
			this.homeTileY = -1;
			this.friendly = false;
			this.behindTiles = false;
			this.boss = false;
			this.noTileCollide = false;
			this.rotation = 0f;
			this.active = true;
			this.alpha = 0;
			this.color = default(Color);
			this.collideX = false;
			this.collideY = false;
			this.direction = 0;
			this.oldDirection = this.direction;
			this.frameCounter = 0.0;
			this.netUpdate = false;
			this.knockBackResist = 1f;
			this.name = "";
			this.noGravity = false;
			this.scale = 1f;
			this.soundHit = 0;
			this.soundKilled = 0;
			this.spriteDirection = -1;
			this.target = 255;
			this.oldTarget = this.target;
			this.targetRect = default(Rectangle);
			this.timeLeft = NPC.activeTime;
			this.type = Type;
			this.value = 0f;
			for (int i = 0; i < NPC.maxAI; i++)
			{
				this.ai[i] = 0f;
			}
			if (this.type == 1)
			{
				this.name = "Blue Slime";
				this.width = 24;
				this.height = 18;
				this.aiStyle = 1;
				this.damage = 7;
				this.defense = 2;
				this.lifeMax = 25;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.alpha = 175;
				this.color = new Color(0, 80, 255, 100);
				this.value = 25f;
			}
			if (this.type == 2)
			{
				this.name = "Demon Eye";
				this.width = 30;
				this.height = 32;
				this.aiStyle = 2;
				this.damage = 18;
				this.defense = 2;
				this.lifeMax = 60;
				this.soundHit = 1;
				this.knockBackResist = 0.8f;
				this.soundKilled = 1;
				this.value = 75f;
			}
			if (this.type == 3)
			{
				this.name = "Zombie";
				this.width = 18;
				this.height = 40;
				this.aiStyle = 3;
				this.damage = 14;
				this.defense = 6;
				this.lifeMax = 45;
				this.soundHit = 1;
				this.soundKilled = 2;
				this.knockBackResist = 0.5f;
				this.value = 60f;
			}
			if (this.type == 4)
			{
				this.name = "Eye of Cthulhu";
				this.width = 100;
				this.height = 110;
				this.aiStyle = 4;
				this.damage = 18;
				this.defense = 12;
				this.lifeMax = 3000;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.knockBackResist = 0f;
				this.noGravity = true;
				this.noTileCollide = true;
				this.timeLeft = NPC.activeTime * 30;
				this.boss = true;
				this.value = 30000f;
			}
			if (this.type == 5)
			{
				this.name = "Servant of Cthulhu";
				this.width = 20;
				this.height = 20;
				this.aiStyle = 5;
				this.damage = 23;
				this.defense = 0;
				this.lifeMax = 8;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.noGravity = true;
				this.noTileCollide = true;
			}
			if (this.type == 6)
			{
				this.name = "Eater of Souls";
				this.width = 30;
				this.height = 30;
				this.aiStyle = 5;
				this.damage = 15;
				this.defense = 8;
				this.lifeMax = 40;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.noGravity = true;
				this.knockBackResist = 0.5f;
				this.value = 90f;
			}
			if (this.type == 7)
			{
				this.name = "Devourer Head";
				this.width = 22;
				this.height = 22;
				this.aiStyle = 6;
				this.damage = 28;
				this.defense = 2;
				this.lifeMax = 40;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.behindTiles = true;
				this.value = 300f;
			}
			if (this.type == 8)
			{
				this.name = "Devourer Body";
				this.width = 22;
				this.height = 22;
				this.aiStyle = 6;
				this.damage = 18;
				this.defense = 6;
				this.lifeMax = 60;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.behindTiles = true;
				this.value = 300f;
			}
			if (this.type == 9)
			{
				this.name = "Devourer Tail";
				this.width = 22;
				this.height = 22;
				this.aiStyle = 6;
				this.damage = 13;
				this.defense = 10;
				this.lifeMax = 100;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.behindTiles = true;
				this.value = 300f;
			}
			if (this.type == 10)
			{
				this.name = "Giant Worm Head";
				this.width = 14;
				this.height = 14;
				this.aiStyle = 6;
				this.damage = 8;
				this.defense = 0;
				this.lifeMax = 10;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.behindTiles = true;
				this.value = 200f;
			}
			if (this.type == 11)
			{
				this.name = "Giant Worm Body";
				this.width = 14;
				this.height = 14;
				this.aiStyle = 6;
				this.damage = 4;
				this.defense = 4;
				this.lifeMax = 15;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.behindTiles = true;
				this.value = 200f;
			}
			if (this.type == 12)
			{
				this.name = "Giant Worm Tail";
				this.width = 14;
				this.height = 14;
				this.aiStyle = 6;
				this.damage = 4;
				this.defense = 6;
				this.lifeMax = 20;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.behindTiles = true;
				this.value = 200f;
			}
			if (this.type == 13)
			{
				this.name = "Eater of Worlds Head";
				this.width = 38;
				this.height = 38;
				this.aiStyle = 6;
				this.damage = 40;
				this.defense = 0;
				this.lifeMax = 120;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.behindTiles = true;
				this.value = 300f;
			}
			if (this.type == 14)
			{
				this.name = "Eater of Worlds Body";
				this.width = 38;
				this.height = 38;
				this.aiStyle = 6;
				this.damage = 15;
				this.defense = 4;
				this.lifeMax = 200;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.behindTiles = true;
				this.value = 300f;
			}
			if (this.type == 15)
			{
				this.name = "Eater of Worlds Tail";
				this.width = 38;
				this.height = 38;
				this.aiStyle = 6;
				this.damage = 10;
				this.defense = 8;
				this.lifeMax = 300;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.behindTiles = true;
				this.value = 300f;
			}
			if (this.type == 16)
			{
				this.name = "Mother Slime";
				this.width = 36;
				this.height = 24;
				this.aiStyle = 1;
				this.damage = 20;
				this.defense = 7;
				this.lifeMax = 90;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.alpha = 120;
				this.color = new Color(0, 0, 0, 50);
				this.value = 75f;
				this.scale = 1.25f;
				this.knockBackResist = 0.6f;
			}
			if (this.type == 17)
			{
				this.townNPC = true;
				this.friendly = true;
				this.name = "Merchant";
				this.width = 18;
				this.height = 40;
				this.aiStyle = 7;
				this.damage = 10;
				this.defense = 15;
				this.lifeMax = 250;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.knockBackResist = 0.5f;
			}
			if (this.type == 18)
			{
				this.townNPC = true;
				this.friendly = true;
				this.name = "Nurse";
				this.width = 18;
				this.height = 40;
				this.aiStyle = 7;
				this.damage = 10;
				this.defense = 15;
				this.lifeMax = 250;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.knockBackResist = 0.5f;
			}
			if (this.type == 19)
			{
				this.townNPC = true;
				this.friendly = true;
				this.name = "Arms Dealer";
				this.width = 18;
				this.height = 40;
				this.aiStyle = 7;
				this.damage = 10;
				this.defense = 15;
				this.lifeMax = 250;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.knockBackResist = 0.5f;
			}
			if (this.type == 20)
			{
				this.townNPC = true;
				this.friendly = true;
				this.name = "Dryad";
				this.width = 18;
				this.height = 40;
				this.aiStyle = 7;
				this.damage = 10;
				this.defense = 15;
				this.lifeMax = 250;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.knockBackResist = 0.5f;
			}
			if (this.type == 21)
			{
				this.name = "Skeleton";
				this.width = 18;
				this.height = 40;
				this.aiStyle = 3;
				this.damage = 20;
				this.defense = 8;
				this.lifeMax = 60;
				this.soundHit = 2;
				this.soundKilled = 2;
				this.knockBackResist = 0.5f;
				this.value = 250f;
			}
			if (this.type == 22)
			{
				this.townNPC = true;
				this.friendly = true;
				this.name = "Guide";
				this.width = 18;
				this.height = 40;
				this.aiStyle = 7;
				this.damage = 10;
				this.defense = 15;
				this.lifeMax = 250;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.knockBackResist = 0.5f;
			}
			if (this.type == 23)
			{
				this.name = "Meteor Head";
				this.width = 22;
				this.height = 22;
				this.aiStyle = 5;
				this.damage = 25;
				this.defense = 10;
				this.lifeMax = 50;
				this.soundHit = 3;
				this.soundKilled = 3;
				this.noGravity = true;
				this.noTileCollide = true;
				this.value = 300f;
				this.knockBackResist = 0.8f;
			}
			else
			{
				if (this.type == 24)
				{
					this.name = "Fire Imp";
					this.width = 18;
					this.height = 40;
					this.aiStyle = 8;
					this.damage = 30;
					this.defense = 20;
					this.lifeMax = 80;
					this.soundHit = 1;
					this.soundKilled = 1;
					this.knockBackResist = 0.5f;
					this.value = 800f;
				}
			}
			if (this.type == 25)
			{
				this.name = "Burning Sphere";
				this.width = 16;
				this.height = 16;
				this.aiStyle = 9;
				this.damage = 25;
				this.defense = 0;
				this.lifeMax = 1;
				this.soundHit = 3;
				this.soundKilled = 3;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.alpha = 100;
			}
			if (this.type == 26)
			{
				this.name = "Goblin Peon";
				this.width = 18;
				this.height = 40;
				this.aiStyle = 3;
				this.damage = 12;
				this.defense = 4;
				this.lifeMax = 60;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.knockBackResist = 0.8f;
				this.value = 250f;
			}
			if (this.type == 27)
			{
				this.name = "Goblin Thief";
				this.width = 18;
				this.height = 40;
				this.aiStyle = 3;
				this.damage = 20;
				this.defense = 6;
				this.lifeMax = 80;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.knockBackResist = 0.7f;
				this.value = 600f;
			}
			if (this.type == 28)
			{
				this.name = "Goblin Warrior";
				this.width = 18;
				this.height = 40;
				this.aiStyle = 3;
				this.damage = 25;
				this.defense = 8;
				this.lifeMax = 110;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.knockBackResist = 0.5f;
				this.value = 500f;
			}
			else
			{
				if (this.type == 29)
				{
					this.name = "Goblin Sorcerer";
					this.width = 18;
					this.height = 40;
					this.aiStyle = 8;
					this.damage = 20;
					this.defense = 2;
					this.lifeMax = 40;
					this.soundHit = 1;
					this.soundKilled = 1;
					this.knockBackResist = 0.6f;
					this.value = 800f;
				}
				else
				{
					if (this.type == 30)
					{
						this.name = "Chaos Ball";
						this.width = 16;
						this.height = 16;
						this.aiStyle = 9;
						this.damage = 20;
						this.defense = 0;
						this.lifeMax = 1;
						this.soundHit = 3;
						this.soundKilled = 3;
						this.noGravity = true;
						this.noTileCollide = true;
						this.alpha = 100;
						this.knockBackResist = 0f;
					}
					else
					{
						if (this.type == 31)
						{
							this.name = "Angry Bones";
							this.width = 18;
							this.height = 40;
							this.aiStyle = 3;
							this.damage = 30;
							this.defense = 10;
							this.lifeMax = 100;
							this.soundHit = 2;
							this.soundKilled = 2;
							this.knockBackResist = 0.7f;
							this.value = 500f;
						}
						else
						{
							if (this.type == 32)
							{
								this.name = "Dark Caster";
								this.width = 18;
								this.height = 40;
								this.aiStyle = 8;
								this.damage = 20;
								this.defense = 4;
								this.lifeMax = 50;
								this.soundHit = 2;
								this.soundKilled = 2;
								this.knockBackResist = 0.6f;
								this.value = 800f;
							}
							else
							{
								if (this.type == 33)
								{
									this.name = "Water Sphere";
									this.width = 16;
									this.height = 16;
									this.aiStyle = 9;
									this.damage = 20;
									this.defense = 0;
									this.lifeMax = 1;
									this.soundHit = 3;
									this.soundKilled = 3;
									this.noGravity = true;
									this.noTileCollide = true;
									this.alpha = 100;
									this.knockBackResist = 0f;
								}
							}
						}
					}
				}
			}
			if (this.type == 34)
			{
				this.name = "Burning Skull";
				this.width = 26;
				this.height = 28;
				this.aiStyle = 10;
				this.damage = 25;
				this.defense = 30;
				this.lifeMax = 30;
				this.soundHit = 2;
				this.soundKilled = 2;
				this.noGravity = true;
				this.value = 300f;
				this.knockBackResist = 1.2f;
			}
			if (this.type == 35)
			{
				this.name = "Skeletron Head";
				this.width = 80;
				this.height = 102;
				this.aiStyle = 11;
				this.damage = 35;
				this.defense = 12;
				this.lifeMax = 6000;
				this.soundHit = 2;
				this.soundKilled = 2;
				this.noGravity = true;
				this.noTileCollide = true;
				this.value = 50000f;
				this.knockBackResist = 0f;
				this.boss = true;
			}
			if (this.type == 36)
			{
				this.name = "Skeletron Hand";
				this.width = 52;
				this.height = 52;
				this.aiStyle = 12;
				this.damage = 30;
				this.defense = 18;
				this.lifeMax = 1200;
				this.soundHit = 2;
				this.soundKilled = 2;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
			}
			if (this.type == 37)
			{
				this.townNPC = true;
				this.friendly = true;
				this.name = "Old Man";
				this.width = 18;
				this.height = 40;
				this.aiStyle = 7;
				this.damage = 10;
				this.defense = 100;
				this.lifeMax = 250;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.knockBackResist = 0.5f;
			}
			if (this.type == 38)
			{
				this.townNPC = true;
				this.friendly = true;
				this.name = "Demolitionist";
				this.width = 18;
				this.height = 40;
				this.aiStyle = 7;
				this.damage = 10;
				this.defense = 15;
				this.lifeMax = 250;
				this.soundHit = 1;
				this.soundKilled = 1;
				this.knockBackResist = 0.5f;
			}
			if (this.type == 39)
			{
				this.name = "Bone Serpent Head";
				this.width = 22;
				this.height = 22;
				this.aiStyle = 6;
				this.damage = 40;
				this.defense = 10;
				this.lifeMax = 120;
				this.soundHit = 2;
				this.soundKilled = 2;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.behindTiles = true;
				this.value = 1000f;
			}
			if (this.type == 40)
			{
				this.name = "Bone Serpent Body";
				this.width = 22;
				this.height = 22;
				this.aiStyle = 6;
				this.damage = 30;
				this.defense = 12;
				this.lifeMax = 150;
				this.soundHit = 2;
				this.soundKilled = 2;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.behindTiles = true;
				this.value = 1000f;
			}
			if (this.type == 41)
			{
				this.name = "Bone Serpent Tail";
				this.width = 22;
				this.height = 22;
				this.aiStyle = 6;
				this.damage = 20;
				this.defense = 18;
				this.lifeMax = 200;
				this.soundHit = 2;
				this.soundKilled = 2;
				this.noGravity = true;
				this.noTileCollide = true;
				this.knockBackResist = 0f;
				this.behindTiles = true;
				this.value = 1000f;
			}
			if (this.type == 42)
			{
				this.name = "Hornet";
				this.width = 34;
				this.height = 32;
				this.aiStyle = 2;
				this.damage = 40;
				this.defense = 14;
				this.lifeMax = 100;
				this.soundHit = 1;
				this.knockBackResist = 0.8f;
				this.soundKilled = 1;
				this.value = 750f;
			}
			if (this.type == 43)
			{
				this.noGravity = true;
				this.name = "Man Eater";
				this.width = 30;
				this.height = 30;
				this.aiStyle = 13;
				this.damage = 60;
				this.defense = 18;
				this.lifeMax = 200;
				this.soundHit = 1;
				this.knockBackResist = 0.7f;
				this.soundKilled = 1;
				this.value = 750f;
			}
			else
			{
				if (this.type == 44)
				{
					this.name = "Dead Miner";
					this.width = 18;
					this.height = 40;
					this.aiStyle = 3;
					this.damage = 20;
					this.defense = 8;
					this.lifeMax = 60;
					this.soundHit = 2;
					this.soundKilled = 2;
					this.knockBackResist = 0.5f;
					this.value = 250f;
				}
				else
				{
					if (this.type == 45)
					{
						this.name = "Tim";
						this.width = 18;
						this.height = 40;
						this.aiStyle = 8;
						this.damage = 20;
						this.defense = 4;
						this.lifeMax = 50;
						this.soundHit = 2;
						this.soundKilled = 2;
						this.knockBackResist = 0.6f;
						this.value = 800f;
					}
				}
			}
			if (Main.dedServ)
			{
				this.frame = default(Rectangle);
			}
            //else
            //{
            //    this.frame = new Rectangle(0, 0, Main.npcTexture[this.type].Width, Main.npcTexture[this.type].Height / Main.npcFrameCount[this.type]);
            //}
			this.width = (int)((float)this.width * this.scale);
			this.height = (int)((float)this.height * this.scale);
			this.life = this.lifeMax;
			if (Main.dumbAI)
			{
				this.aiStyle = 0;
			}
		}
		public void AI()
		{
			if (this.aiStyle == 0)
			{
				this.velocity.X = this.velocity.X * 0.93f;
				if ((double)this.velocity.X > -0.1 && (double)this.velocity.X < 0.1)
				{
					this.velocity.X = 0f;
					return;
				}
			}
			else
			{
				if (this.aiStyle == 1)
				{
					this.aiAction = 0;
					if (this.ai[2] == 0f)
					{
						this.ai[0] = -100f;
						this.ai[2] = 1f;
						this.TargetClosest();
					}
					if (this.velocity.Y == 0f)
					{
						if (this.ai[3] == this.position.X)
						{
							this.direction *= -1;
						}
						this.ai[3] = 0f;
						this.velocity.X = this.velocity.X * 0.8f;
						if ((double)this.velocity.X > -0.1 && (double)this.velocity.X < 0.1)
						{
							this.velocity.X = 0f;
						}
						if (!Main.dayTime || this.life != this.lifeMax || (double)this.position.Y > Main.worldSurface * 16.0)
						{
							this.ai[0] += 1f;
						}
						this.ai[0] += 1f;
						if (this.ai[0] >= 0f)
						{
							this.netUpdate = true;
							if (!Main.dayTime || this.life != this.lifeMax || (double)this.position.Y > Main.worldSurface * 16.0)
							{
								this.TargetClosest();
							}
							if (this.ai[1] == 2f)
							{
								this.velocity.Y = -8f;
								this.velocity.X = this.velocity.X + (float)(3 * this.direction);
								this.ai[0] = -200f;
								this.ai[1] = 0f;
								this.ai[3] = this.position.X;
								return;
							}
							this.velocity.Y = -6f;
							this.velocity.X = this.velocity.X + (float)(2 * this.direction);
							this.ai[0] = -120f;
							this.ai[1] += 1f;
							return;
						}
						else
						{
							if (this.ai[0] >= -30f)
							{
								this.aiAction = 1;
								return;
							}
						}
					}
					else
					{
						if (this.target < 255 && ((this.direction == 1 && this.velocity.X < 3f) || (this.direction == -1 && this.velocity.X > -3f)))
						{
							if ((this.direction == -1 && (double)this.velocity.X < 0.1) || (this.direction == 1 && (double)this.velocity.X > -0.1))
							{
								this.velocity.X = this.velocity.X + 0.2f * (float)this.direction;
								return;
							}
							this.velocity.X = this.velocity.X * 0.93f;
							return;
						}
					}
				}
				else
				{
					if (this.aiStyle == 2)
					{
						this.noGravity = true;
						if (this.collideX)
						{
							this.velocity.X = this.oldVelocity.X * -0.5f;
							if (this.direction == -1 && this.velocity.X > 0f && this.velocity.X < 2f)
							{
								this.velocity.X = 2f;
							}
							if (this.direction == 1 && this.velocity.X < 0f && this.velocity.X > -2f)
							{
								this.velocity.X = -2f;
							}
						}
						if (this.collideY)
						{
							this.velocity.Y = this.oldVelocity.Y * -0.5f;
							if (this.velocity.Y > 0f && this.velocity.Y < 1f)
							{
								this.velocity.Y = 1f;
							}
							if (this.velocity.Y < 0f && this.velocity.Y > -1f)
							{
								this.velocity.Y = -1f;
							}
						}
						if (Main.dayTime && (double)this.position.Y <= Main.worldSurface * 16.0 && this.type == 2)
						{
							if (this.timeLeft > 10)
							{
								this.timeLeft = 10;
							}
							this.directionY = -1;
							if (this.velocity.Y > 0f)
							{
								this.direction = 1;
							}
							this.direction = -1;
							if (this.velocity.X > 0f)
							{
								this.direction = 1;
							}
						}
						else
						{
							this.TargetClosest();
						}
						if (this.direction == -1 && this.velocity.X > -4f)
						{
							this.velocity.X = this.velocity.X - 0.1f;
							if (this.velocity.X > 4f)
							{
								this.velocity.X = this.velocity.X - 0.1f;
							}
							else
							{
								if (this.velocity.X > 0f)
								{
									this.velocity.X = this.velocity.X + 0.05f;
								}
							}
							if (this.velocity.X < -4f)
							{
								this.velocity.X = -4f;
							}
						}
						else
						{
							if (this.direction == 1 && this.velocity.X < 4f)
							{
								this.velocity.X = this.velocity.X + 0.1f;
								if (this.velocity.X < -4f)
								{
									this.velocity.X = this.velocity.X + 0.1f;
								}
								else
								{
									if (this.velocity.X < 0f)
									{
										this.velocity.X = this.velocity.X - 0.05f;
									}
								}
								if (this.velocity.X > 4f)
								{
									this.velocity.X = 4f;
								}
							}
						}
						if (this.directionY == -1 && (double)this.velocity.Y > -1.5)
						{
							this.velocity.Y = this.velocity.Y - 0.04f;
							if ((double)this.velocity.Y > 1.5)
							{
								this.velocity.Y = this.velocity.Y - 0.05f;
							}
							else
							{
								if (this.velocity.Y > 0f)
								{
									this.velocity.Y = this.velocity.Y + 0.03f;
								}
							}
							if ((double)this.velocity.Y < -1.5)
							{
								this.velocity.Y = -1.5f;
							}
						}
						else
						{
							if (this.directionY == 1 && (double)this.velocity.Y < 1.5)
							{
								this.velocity.Y = this.velocity.Y + 0.04f;
								if ((double)this.velocity.Y < -1.5)
								{
									this.velocity.Y = this.velocity.Y + 0.05f;
								}
								else
								{
									if (this.velocity.Y < 0f)
									{
										this.velocity.Y = this.velocity.Y - 0.03f;
									}
								}
								if ((double)this.velocity.Y > 1.5)
								{
									this.velocity.Y = 1.5f;
								}
							}
						}
						if (this.type == 2 && Main.rand.Next(40) == 0)
						{
							Vector2 arg_8C4_0 = new Vector2(this.position.X, this.position.Y + (float)this.height * 0.25f);
							int arg_8C4_1 = this.width;
							int arg_8C4_2 = (int)((float)this.height * 0.5f);
							int arg_8C4_3 = 5;
							float arg_8C4_4 = this.velocity.X;
							float arg_8C4_5 = 2f;
							int arg_8C4_6 = 0;
							Color newColor = default(Color);
							int num = Dust.NewDust(arg_8C4_0, arg_8C4_1, arg_8C4_2, arg_8C4_3, arg_8C4_4, arg_8C4_5, arg_8C4_6, newColor, 1f);
							Dust expr_8D6_cp_0 = Main.dust[num];
							expr_8D6_cp_0.velocity.X = expr_8D6_cp_0.velocity.X * 0.5f;
							Dust expr_8F3_cp_0 = Main.dust[num];
							expr_8F3_cp_0.velocity.Y = expr_8F3_cp_0.velocity.Y * 0.1f;
							return;
						}
					}
					else
					{
						if (this.aiStyle == 3)
						{
							int num2 = 60;
							bool flag = false;
							if (this.velocity.Y == 0f && ((this.velocity.X > 0f && this.direction < 0) || (this.velocity.X < 0f && this.direction > 0)))
							{
								flag = true;
							}
							if (this.position.X == this.oldPosition.X || this.ai[3] >= (float)num2 || flag)
							{
								this.ai[3] += 1f;
							}
							else
							{
								if ((double)Math.Abs(this.velocity.X) > 0.9 && this.ai[3] > 0f)
								{
									this.ai[3] -= 1f;
								}
							}
							if (this.ai[3] > (float)(num2 * 10))
							{
								this.ai[3] = 0f;
							}
							if (this.ai[3] == (float)num2)
							{
								this.netUpdate = true;
							}
							if ((!Main.dayTime || (double)this.position.Y > Main.worldSurface * 16.0 || this.type == 26 || this.type == 27 || this.type == 28 || this.type == 31) && this.ai[3] < (float)num2)
							{
								if ((this.type == 3 || this.type == 21 || this.type == 31) && Main.rand.Next(1000) == 0)
								{
									//Main.PlaySound(14, (int)this.position.X, (int)this.position.Y, 1);
								}
								this.TargetClosest();
							}
							else
							{
								if (this.timeLeft > 10)
								{
									this.timeLeft = 10;
								}
								if (this.velocity.X == 0f)
								{
									if (this.velocity.Y == 0f)
									{
										this.ai[0] += 1f;
										if (this.ai[0] >= 2f)
										{
											this.direction *= -1;
											this.spriteDirection = this.direction;
											this.ai[0] = 0f;
										}
									}
								}
								else
								{
									this.ai[0] = 0f;
								}
								if (this.direction == 0)
								{
									this.direction = 1;
								}
							}
							if (this.type == 27)
							{
								if (this.velocity.X < -2f || this.velocity.X > 2f)
								{
									if (this.velocity.Y == 0f)
									{
										this.velocity *= 0.8f;
									}
								}
								else
								{
									if (this.velocity.X < 2f && this.direction == 1)
									{
										this.velocity.X = this.velocity.X + 0.07f;
										if (this.velocity.X > 2f)
										{
											this.velocity.X = 2f;
										}
									}
									else
									{
										if (this.velocity.X > -2f && this.direction == -1)
										{
											this.velocity.X = this.velocity.X - 0.07f;
											if (this.velocity.X < -2f)
											{
												this.velocity.X = -2f;
											}
										}
									}
								}
							}
							else
							{
								if (this.type == 21 || this.type == 26 || this.type == 31)
								{
									if (this.velocity.X < -1.5f || this.velocity.X > 1.5f)
									{
										if (this.velocity.Y == 0f)
										{
											this.velocity *= 0.8f;
										}
									}
									else
									{
										if (this.velocity.X < 1.5f && this.direction == 1)
										{
											this.velocity.X = this.velocity.X + 0.07f;
											if (this.velocity.X > 1.5f)
											{
												this.velocity.X = 1.5f;
											}
										}
										else
										{
											if (this.velocity.X > -1.5f && this.direction == -1)
											{
												this.velocity.X = this.velocity.X - 0.07f;
												if (this.velocity.X < -1.5f)
												{
													this.velocity.X = -1.5f;
												}
											}
										}
									}
								}
								else
								{
									if (this.velocity.X < -1f || this.velocity.X > 1f)
									{
										if (this.velocity.Y == 0f)
										{
											this.velocity *= 0.8f;
										}
									}
									else
									{
										if (this.velocity.X < 1f && this.direction == 1)
										{
											this.velocity.X = this.velocity.X + 0.07f;
											if (this.velocity.X > 1f)
											{
												this.velocity.X = 1f;
											}
										}
										else
										{
											if (this.velocity.X > -1f && this.direction == -1)
											{
												this.velocity.X = this.velocity.X - 0.07f;
												if (this.velocity.X < -1f)
												{
													this.velocity.X = -1f;
												}
											}
										}
									}
								}
							}
							if (this.velocity.Y != 0f)
							{
								this.ai[1] = 0f;
								this.ai[2] = 0f;
								return;
							}
							int num3 = (int)((this.position.X + (float)(this.width / 2) + (float)(15 * this.direction)) / 16f);
							int num4 = (int)((this.position.Y + (float)this.height - 16f) / 16f);
							if (Main.tile[num3, num4] == null)
							{
								Main.tile[num3, num4] = new Tile();
							}
							if (Main.tile[num3, num4 - 1] == null)
							{
								Main.tile[num3, num4 - 1] = new Tile();
							}
							if (Main.tile[num3, num4 - 2] == null)
							{
								Main.tile[num3, num4 - 2] = new Tile();
							}
							if (Main.tile[num3, num4 - 3] == null)
							{
								Main.tile[num3, num4 - 3] = new Tile();
							}
							if (Main.tile[num3, num4 + 1] == null)
							{
								Main.tile[num3, num4 + 1] = new Tile();
							}
							if (Main.tile[num3 + this.direction, num4 - 1] == null)
							{
								Main.tile[num3 + this.direction, num4 - 1] = new Tile();
							}
							if (Main.tile[num3 + this.direction, num4 + 1] == null)
							{
								Main.tile[num3 + this.direction, num4 + 1] = new Tile();
							}
							if (Main.tile[num3, num4 - 1].active && Main.tile[num3, num4 - 1].type == 10)
							{
								this.ai[2] += 1f;
								this.ai[3] = 0f;
								if (this.ai[2] >= 60f)
								{
									if (!Main.bloodMoon && this.type == 3)
									{
										this.ai[1] = 0f;
									}
									this.velocity.X = 0.5f * (float)(-(float)this.direction);
									this.ai[1] += 1f;
									if (this.type == 27)
									{
										this.ai[1] += 1f;
									}
									if (this.type == 31)
									{
										this.ai[1] += 6f;
									}
									this.ai[2] = 0f;
									bool flag2 = false;
									if (this.ai[1] >= 10f)
									{
										flag2 = true;
										this.ai[1] = 10f;
									}
									WorldGen.KillTile(num3, num4 - 1, true, false, false);
									if ((Main.netMode != 1 || !flag2) && flag2 && Main.netMode != 1)
									{
										if (this.type == 26)
										{
											WorldGen.KillTile(num3, num4 - 1, false, false, false);
											if (Main.netMode == 2)
											{
												NetMessage.SendData(17, -1, -1, "", 0, (float)num3, (float)(num4 - 1), 0f);
												return;
											}
										}
										else
										{
											bool flag3 = WorldGen.OpenDoor(num3, num4, this.direction);
											if (!flag3)
											{
												this.ai[3] = (float)num2;
												this.netUpdate = true;
											}
											if (Main.netMode == 2 && flag3)
											{
												NetMessage.SendData(19, -1, -1, "", 0, (float)num3, (float)num4, (float)this.direction);
												return;
											}
										}
									}
								}
							}
							else
							{
								if ((this.velocity.X < 0f && this.spriteDirection == -1) || (this.velocity.X > 0f && this.spriteDirection == 1))
								{
									if (Main.tile[num3, num4 - 2].active && Main.tileSolid[(int)Main.tile[num3, num4 - 2].type])
									{
										if (Main.tile[num3, num4 - 3].active && Main.tileSolid[(int)Main.tile[num3, num4 - 3].type])
										{
											this.velocity.Y = -8f;
											this.netUpdate = true;
										}
										else
										{
											this.velocity.Y = -7f;
											this.netUpdate = true;
										}
									}
									else
									{
										if (Main.tile[num3, num4 - 1].active && Main.tileSolid[(int)Main.tile[num3, num4 - 1].type])
										{
											this.velocity.Y = -6f;
											this.netUpdate = true;
										}
										else
										{
											if (Main.tile[num3, num4].active && Main.tileSolid[(int)Main.tile[num3, num4].type])
											{
												this.velocity.Y = -5f;
												this.netUpdate = true;
											}
											else
											{
												if (this.directionY < 0 && (!Main.tile[num3, num4 + 1].active || !Main.tileSolid[(int)Main.tile[num3, num4 + 1].type]) && (!Main.tile[num3 + this.direction, num4 + 1].active || !Main.tileSolid[(int)Main.tile[num3 + this.direction, num4 + 1].type]))
												{
													this.velocity.Y = -8f;
													this.velocity.X = this.velocity.X * 1.5f;
													this.netUpdate = true;
												}
												else
												{
													this.ai[1] = 0f;
													this.ai[2] = 0f;
												}
											}
										}
									}
								}
								if (this.type == 31 && this.velocity.Y == 0f && Math.Abs(this.position.X + (float)(this.width / 2) - (Main.player[this.target].position.X - (float)(Main.player[this.target].width / 2))) < 100f && Math.Abs(this.position.Y + (float)(this.height / 2) - (Main.player[this.target].position.Y - (float)(Main.player[this.target].height / 2))) < 50f && ((this.direction > 0 && this.velocity.X > 1f) || (this.direction < 0 && this.velocity.X < -1f)))
								{
									this.velocity.X = this.velocity.X * 2f;
									if (this.velocity.X > 3f)
									{
										this.velocity.X = 3f;
									}
									if (this.velocity.X < -3f)
									{
										this.velocity.X = -3f;
									}
									this.velocity.Y = -4f;
									this.netUpdate = true;
									return;
								}
							}
						}
						else
						{
							if (this.aiStyle == 4)
							{
								if (this.target < 0 || this.target == 255 || Main.player[this.target].dead || !Main.player[this.target].active)
								{
									this.TargetClosest();
								}
								bool dead = Main.player[this.target].dead;
								float num5 = this.position.X + (float)(this.width / 2) - Main.player[this.target].position.X - (float)(Main.player[this.target].width / 2);
								float num6 = this.position.Y + (float)this.height - 59f - Main.player[this.target].position.Y - (float)(Main.player[this.target].height / 2);
								float num7 = (float)Math.Atan2((double)num6, (double)num5) + 1.57f;
								if (num7 < 0f)
								{
									num7 += 6.283f;
								}
								else
								{
									if ((double)num7 > 6.283)
									{
										num7 -= 6.283f;
									}
								}
								float num8 = 0f;
								if (this.ai[0] == 0f && this.ai[1] == 0f)
								{
									num8 = 0.02f;
								}
								if (this.ai[0] == 0f && this.ai[1] == 2f && this.ai[2] > 40f)
								{
									num8 = 0.05f;
								}
								if (this.ai[0] == 3f && this.ai[1] == 0f)
								{
									num8 = 0.05f;
								}
								if (this.ai[0] == 3f && this.ai[1] == 2f && this.ai[2] > 40f)
								{
									num8 = 0.08f;
								}
								if (this.rotation < num7)
								{
									if ((double)(num7 - this.rotation) > 3.1415)
									{
										this.rotation -= num8;
									}
									else
									{
										this.rotation += num8;
									}
								}
								else
								{
									if (this.rotation > num7)
									{
										if ((double)(this.rotation - num7) > 3.1415)
										{
											this.rotation += num8;
										}
										else
										{
											this.rotation -= num8;
										}
									}
								}
								if (this.rotation > num7 - num8 && this.rotation < num7 + num8)
								{
									this.rotation = num7;
								}
								if (this.rotation < 0f)
								{
									this.rotation += 6.283f;
								}
								else
								{
									if ((double)this.rotation > 6.283)
									{
										this.rotation -= 6.283f;
									}
								}
								if (this.rotation > num7 - num8 && this.rotation < num7 + num8)
								{
									this.rotation = num7;
								}
								if (Main.rand.Next(5) == 0)
								{
									Vector2 arg_1961_0 = new Vector2(this.position.X, this.position.Y + (float)this.height * 0.25f);
									int arg_1961_1 = this.width;
									int arg_1961_2 = (int)((float)this.height * 0.5f);
									int arg_1961_3 = 5;
									float arg_1961_4 = this.velocity.X;
									float arg_1961_5 = 2f;
									int arg_1961_6 = 0;
									Color newColor = default(Color);
									int num9 = Dust.NewDust(arg_1961_0, arg_1961_1, arg_1961_2, arg_1961_3, arg_1961_4, arg_1961_5, arg_1961_6, newColor, 1f);
									Dust expr_1975_cp_0 = Main.dust[num9];
									expr_1975_cp_0.velocity.X = expr_1975_cp_0.velocity.X * 0.5f;
									Dust expr_1993_cp_0 = Main.dust[num9];
									expr_1993_cp_0.velocity.Y = expr_1993_cp_0.velocity.Y * 0.1f;
								}
								if (Main.dayTime || dead)
								{
									this.velocity.Y = this.velocity.Y - 0.04f;
									if (this.timeLeft > 10)
									{
										this.timeLeft = 10;
										return;
									}
								}
								else
								{
									if (this.ai[0] == 0f)
									{
										if (this.ai[1] == 0f)
										{
											float num10 = 5f;
											float num11 = 0.04f;
											Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
											float num12 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
											float num13 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - 200f - vector.Y;
											float num14 = (float)Math.Sqrt((double)(num12 * num12 + num13 * num13));
											float num15 = num14;
											num14 = num10 / num14;
											num12 *= num14;
											num13 *= num14;
											if (this.velocity.X < num12)
											{
												this.velocity.X = this.velocity.X + num11;
												if (this.velocity.X < 0f && num12 > 0f)
												{
													this.velocity.X = this.velocity.X + num11;
												}
											}
											else
											{
												if (this.velocity.X > num12)
												{
													this.velocity.X = this.velocity.X - num11;
													if (this.velocity.X > 0f && num12 < 0f)
													{
														this.velocity.X = this.velocity.X - num11;
													}
												}
											}
											if (this.velocity.Y < num13)
											{
												this.velocity.Y = this.velocity.Y + num11;
												if (this.velocity.Y < 0f && num13 > 0f)
												{
													this.velocity.Y = this.velocity.Y + num11;
												}
											}
											else
											{
												if (this.velocity.Y > num13)
												{
													this.velocity.Y = this.velocity.Y - num11;
													if (this.velocity.Y > 0f && num13 < 0f)
													{
														this.velocity.Y = this.velocity.Y - num11;
													}
												}
											}
											this.ai[2] += 1f;
											if (this.ai[2] >= 600f)
											{
												this.ai[1] = 1f;
												this.ai[2] = 0f;
												this.ai[3] = 0f;
												this.target = 255;
												this.netUpdate = true;
											}
											else
											{
												if (this.position.Y + (float)this.height < Main.player[this.target].position.Y && num15 < 500f)
												{
													if (!Main.player[this.target].dead)
													{
														this.ai[3] += 1f;
													}
													if (this.ai[3] >= 90f)
													{
														this.ai[3] = 0f;
														this.rotation = num7;
														float num16 = 5f;
														float num17 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
														float num18 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector.Y;
														float num19 = (float)Math.Sqrt((double)(num17 * num17 + num18 * num18));
														num19 = num16 / num19;
														Vector2 vector2 = vector;
                                                        Vector2 vector3 = new Vector2();
														vector3.X = num17 * num19;
														vector3.Y = num18 * num19;
														vector2.X += vector3.X * 10f;
														vector2.Y += vector3.Y * 10f;
														if (Main.netMode != 1)
														{
															int num20 = NPC.NewNPC((int)vector2.X, (int)vector2.Y, 5, 0);
															Main.npc[num20].velocity.X = vector3.X;
															Main.npc[num20].velocity.Y = vector3.Y;
															if (Main.netMode == 2 && num20 < 1000)
															{
																NetMessage.SendData(23, -1, -1, "", num20, 0f, 0f, 0f);
															}
														}
														//Main.PlaySound(3, (int)vector2.X, (int)vector2.Y, 1);
														for (int i = 0; i < 10; i++)
														{
															Vector2 arg_1ED9_0 = vector2;
															int arg_1ED9_1 = 20;
															int arg_1ED9_2 = 20;
															int arg_1ED9_3 = 5;
															float arg_1ED9_4 = vector3.X * 0.4f;
															float arg_1ED9_5 = vector3.Y * 0.4f;
															int arg_1ED9_6 = 0;
															Color newColor = default(Color);
															Dust.NewDust(arg_1ED9_0, arg_1ED9_1, arg_1ED9_2, arg_1ED9_3, arg_1ED9_4, arg_1ED9_5, arg_1ED9_6, newColor, 1f);
														}
													}
												}
											}
										}
										else
										{
											if (this.ai[1] == 1f)
											{
												this.rotation = num7;
												float num21 = 7f;
												Vector2 vector4 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
												float num22 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector4.X;
												float num23 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector4.Y;
												float num24 = (float)Math.Sqrt((double)(num22 * num22 + num23 * num23));
												num24 = num21 / num24;
												this.velocity.X = num22 * num24;
												this.velocity.Y = num23 * num24;
												this.ai[1] = 2f;
											}
											else
											{
												if (this.ai[1] == 2f)
												{
													this.ai[2] += 1f;
													if (this.ai[2] >= 40f)
													{
														this.velocity.X = this.velocity.X * 0.98f;
														this.velocity.Y = this.velocity.Y * 0.98f;
														if ((double)this.velocity.X > -0.1 && (double)this.velocity.X < 0.1)
														{
															this.velocity.X = 0f;
														}
														if ((double)this.velocity.Y > -0.1 && (double)this.velocity.Y < 0.1)
														{
															this.velocity.Y = 0f;
														}
													}
													else
													{
														this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) - 1.57f;
													}
													if (this.ai[2] >= 120f)
													{
														this.ai[3] += 1f;
														this.ai[2] = 0f;
														this.target = 255;
														this.rotation = num7;
														if (this.ai[3] >= 3f)
														{
															this.ai[1] = 0f;
															this.ai[3] = 0f;
														}
														else
														{
															this.ai[1] = 1f;
														}
													}
												}
											}
										}
										if ((double)this.life < (double)this.lifeMax * 0.5)
										{
											this.ai[0] = 1f;
											this.ai[1] = 0f;
											this.ai[2] = 0f;
											this.ai[3] = 0f;
											this.netUpdate = true;
											return;
										}
									}
									else
									{
										if (this.ai[0] == 1f || this.ai[0] == 2f)
										{
											if (this.ai[0] == 1f)
											{
												this.ai[2] += 0.005f;
												if ((double)this.ai[2] > 0.5)
												{
													this.ai[2] = 0.5f;
												}
											}
											else
											{
												this.ai[2] -= 0.005f;
												if (this.ai[2] < 0f)
												{
													this.ai[2] = 0f;
												}
											}
											this.rotation += this.ai[2];
											this.ai[1] += 1f;
											Color newColor;
											if (this.ai[1] == 100f)
											{
												this.ai[0] += 1f;
												this.ai[1] = 0f;
												if (this.ai[0] == 3f)
												{
													this.ai[2] = 0f;
												}
												else
												{
													//Main.PlaySound(3, (int)this.position.X, (int)this.position.Y, 1);
													for (int j = 0; j < 2; j++)
													{
														Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 8);
														Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7);
														Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 6);
													}
													for (int k = 0; k < 20; k++)
													{
														Vector2 arg_2468_0 = this.position;
														int arg_2468_1 = this.width;
														int arg_2468_2 = this.height;
														int arg_2468_3 = 5;
														float arg_2468_4 = (float)Main.rand.Next(-30, 31) * 0.2f;
														float arg_2468_5 = (float)Main.rand.Next(-30, 31) * 0.2f;
														int arg_2468_6 = 0;
														newColor = default(Color);
														Dust.NewDust(arg_2468_0, arg_2468_1, arg_2468_2, arg_2468_3, arg_2468_4, arg_2468_5, arg_2468_6, newColor, 1f);
													}
													//Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
												}
											}
											Vector2 arg_24E7_0 = this.position;
											int arg_24E7_1 = this.width;
											int arg_24E7_2 = this.height;
											int arg_24E7_3 = 5;
											float arg_24E7_4 = (float)Main.rand.Next(-30, 31) * 0.2f;
											float arg_24E7_5 = (float)Main.rand.Next(-30, 31) * 0.2f;
											int arg_24E7_6 = 0;
											newColor = default(Color);
											Dust.NewDust(arg_24E7_0, arg_24E7_1, arg_24E7_2, arg_24E7_3, arg_24E7_4, arg_24E7_5, arg_24E7_6, newColor, 1f);
											this.velocity.X = this.velocity.X * 0.98f;
											this.velocity.Y = this.velocity.Y * 0.98f;
											if ((double)this.velocity.X > -0.1 && (double)this.velocity.X < 0.1)
											{
												this.velocity.X = 0f;
											}
											if ((double)this.velocity.Y > -0.1 && (double)this.velocity.Y < 0.1)
											{
												this.velocity.Y = 0f;
												return;
											}
										}
										else
										{
											this.damage = 30;
											this.defense = 6;
											if (this.ai[1] == 0f)
											{
												float num25 = 6f;
												float num26 = 0.07f;
												Vector2 vector5 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
												float num27 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector5.X;
												float num28 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - 120f - vector5.Y;
												float num29 = (float)Math.Sqrt((double)(num27 * num27 + num28 * num28));
												num29 = num25 / num29;
												num27 *= num29;
												num28 *= num29;
												if (this.velocity.X < num27)
												{
													this.velocity.X = this.velocity.X + num26;
													if (this.velocity.X < 0f && num27 > 0f)
													{
														this.velocity.X = this.velocity.X + num26;
													}
												}
												else
												{
													if (this.velocity.X > num27)
													{
														this.velocity.X = this.velocity.X - num26;
														if (this.velocity.X > 0f && num27 < 0f)
														{
															this.velocity.X = this.velocity.X - num26;
														}
													}
												}
												if (this.velocity.Y < num28)
												{
													this.velocity.Y = this.velocity.Y + num26;
													if (this.velocity.Y < 0f && num28 > 0f)
													{
														this.velocity.Y = this.velocity.Y + num26;
													}
												}
												else
												{
													if (this.velocity.Y > num28)
													{
														this.velocity.Y = this.velocity.Y - num26;
														if (this.velocity.Y > 0f && num28 < 0f)
														{
															this.velocity.Y = this.velocity.Y - num26;
														}
													}
												}
												this.ai[2] += 1f;
												if (this.ai[2] >= 200f)
												{
													this.ai[1] = 1f;
													this.ai[2] = 0f;
													this.ai[3] = 0f;
													this.target = 255;
													this.netUpdate = true;
													return;
												}
											}
											else
											{
												if (this.ai[1] == 1f)
												{
													//Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
													this.rotation = num7;
													float num30 = 8f;
													Vector2 vector6 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
													float num31 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector6.X;
													float num32 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector6.Y;
													float num33 = (float)Math.Sqrt((double)(num31 * num31 + num32 * num32));
													num33 = num30 / num33;
													this.velocity.X = num31 * num33;
													this.velocity.Y = num32 * num33;
													this.ai[1] = 2f;
													return;
												}
												if (this.ai[1] == 2f)
												{
													this.ai[2] += 1f;
													if (this.ai[2] >= 40f)
													{
														this.velocity.X = this.velocity.X * 0.97f;
														this.velocity.Y = this.velocity.Y * 0.97f;
														if ((double)this.velocity.X > -0.1 && (double)this.velocity.X < 0.1)
														{
															this.velocity.X = 0f;
														}
														if ((double)this.velocity.Y > -0.1 && (double)this.velocity.Y < 0.1)
														{
															this.velocity.Y = 0f;
														}
													}
													else
													{
														this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) - 1.57f;
													}
													if (this.ai[2] >= 100f)
													{
														this.ai[3] += 1f;
														this.ai[2] = 0f;
														this.target = 255;
														this.rotation = num7;
														if (this.ai[3] >= 3f)
														{
															this.ai[1] = 0f;
															this.ai[3] = 0f;
															return;
														}
														this.ai[1] = 1f;
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
								if (this.aiStyle == 5)
								{
									if (this.target < 0 || this.target == 255 || Main.player[this.target].dead)
									{
										this.TargetClosest();
									}
									float num34 = 6f;
									float num35 = 0.05f;
									if (this.type == 6)
									{
										num34 = 4f;
										num35 = 0.02f;
									}
									else
									{
										if (this.type == 23)
										{
											num34 = 2.5f;
											num35 = 0.02f;
										}
									}
									Vector2 vector7 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
									float num36 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector7.X;
									float num37 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector7.Y;
									float num38 = (float)Math.Sqrt((double)(num36 * num36 + num37 * num37));
									num38 = num34 / num38;
									num36 *= num38;
									num37 *= num38;
									if (Main.player[this.target].dead)
									{
										num36 = (float)this.direction * num34 / 2f;
										num37 = -num34 / 2f;
									}
									if (this.velocity.X < num36)
									{
										this.velocity.X = this.velocity.X + num35;
										if (this.velocity.X < 0f && num36 > 0f)
										{
											this.velocity.X = this.velocity.X + num35;
										}
									}
									else
									{
										if (this.velocity.X > num36)
										{
											this.velocity.X = this.velocity.X - num35;
											if (this.velocity.X > 0f && num36 < 0f)
											{
												this.velocity.X = this.velocity.X - num35;
											}
										}
									}
									if (this.velocity.Y < num37)
									{
										this.velocity.Y = this.velocity.Y + num35;
										if (this.velocity.Y < 0f && num37 > 0f)
										{
											this.velocity.Y = this.velocity.Y + num35;
										}
									}
									else
									{
										if (this.velocity.Y > num37)
										{
											this.velocity.Y = this.velocity.Y - num35;
											if (this.velocity.Y > 0f && num37 < 0f)
											{
												this.velocity.Y = this.velocity.Y - num35;
											}
										}
									}
									if (this.type == 23)
									{
										this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X);
									}
									else
									{
										this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) - 1.57f;
									}
									if (this.type == 6 || this.type == 23)
									{
										if (this.collideX)
										{
											this.netUpdate = true;
											this.velocity.X = this.oldVelocity.X * -0.7f;
											if (this.direction == -1 && this.velocity.X > 0f && this.velocity.X < 2f)
											{
												this.velocity.X = 2f;
											}
											if (this.direction == 1 && this.velocity.X < 0f && this.velocity.X > -2f)
											{
												this.velocity.X = -2f;
											}
										}
										if (this.collideY)
										{
											this.netUpdate = true;
											this.velocity.Y = this.oldVelocity.Y * -0.7f;
											if (this.velocity.Y > 0f && this.velocity.Y < 2f)
											{
												this.velocity.Y = 2f;
											}
											if (this.velocity.Y < 0f && this.velocity.Y > -2f)
											{
												this.velocity.Y = -2f;
											}
										}
										if (this.type == 23)
										{
											Vector2 arg_300F_0 = new Vector2(this.position.X - this.velocity.X, this.position.Y - this.velocity.Y);
											int arg_300F_1 = this.width;
											int arg_300F_2 = this.height;
											int arg_300F_3 = 6;
											float arg_300F_4 = this.velocity.X * 0.2f;
											float arg_300F_5 = this.velocity.Y * 0.2f;
											int arg_300F_6 = 100;
											Color newColor = default(Color);
											int num39 = Dust.NewDust(arg_300F_0, arg_300F_1, arg_300F_2, arg_300F_3, arg_300F_4, arg_300F_5, arg_300F_6, newColor, 2f);
											Main.dust[num39].noGravity = true;
											Dust expr_3031_cp_0 = Main.dust[num39];
											expr_3031_cp_0.velocity.X = expr_3031_cp_0.velocity.X * 0.3f;
											Dust expr_304F_cp_0 = Main.dust[num39];
											expr_304F_cp_0.velocity.Y = expr_304F_cp_0.velocity.Y * 0.3f;
										}
										else
										{
											if (Main.rand.Next(20) == 0)
											{
												int num40 = Dust.NewDust(new Vector2(this.position.X, this.position.Y + (float)this.height * 0.25f), this.width, (int)((float)this.height * 0.5f), 18, this.velocity.X, 2f, this.alpha, this.color, this.scale);
												Dust expr_30EB_cp_0 = Main.dust[num40];
												expr_30EB_cp_0.velocity.X = expr_30EB_cp_0.velocity.X * 0.5f;
												Dust expr_3109_cp_0 = Main.dust[num40];
												expr_3109_cp_0.velocity.Y = expr_3109_cp_0.velocity.Y * 0.1f;
											}
										}
									}
									else
									{
										if (Main.rand.Next(40) == 0)
										{
											Vector2 arg_318E_0 = new Vector2(this.position.X, this.position.Y + (float)this.height * 0.25f);
											int arg_318E_1 = this.width;
											int arg_318E_2 = (int)((float)this.height * 0.5f);
											int arg_318E_3 = 5;
											float arg_318E_4 = this.velocity.X;
											float arg_318E_5 = 2f;
											int arg_318E_6 = 0;
											Color newColor = default(Color);
											int num41 = Dust.NewDust(arg_318E_0, arg_318E_1, arg_318E_2, arg_318E_3, arg_318E_4, arg_318E_5, arg_318E_6, newColor, 1f);
											Dust expr_31A2_cp_0 = Main.dust[num41];
											expr_31A2_cp_0.velocity.X = expr_31A2_cp_0.velocity.X * 0.5f;
											Dust expr_31C0_cp_0 = Main.dust[num41];
											expr_31C0_cp_0.velocity.Y = expr_31C0_cp_0.velocity.Y * 0.1f;
										}
									}
									if ((Main.dayTime && this.type != 6 && this.type != 23) || Main.player[this.target].dead)
									{
										this.velocity.Y = this.velocity.Y - num35 * 2f;
										if (this.timeLeft > 10)
										{
											this.timeLeft = 10;
											return;
										}
									}
								}
								else
								{
									if (this.aiStyle == 6)
									{
										if (this.target < 0 || this.target == 255 || Main.player[this.target].dead)
										{
											this.TargetClosest();
										}
										if (Main.player[this.target].dead && this.timeLeft > 10)
										{
											this.timeLeft = 10;
										}
										if (Main.netMode != 1)
										{
											if ((this.type == 7 || this.type == 8 || this.type == 10 || this.type == 11 || this.type == 13 || this.type == 14 || this.type == 39 || this.type == 40) && this.ai[0] == 0f)
											{
												if (this.type == 7 || this.type == 10 || this.type == 13 || this.type == 39)
												{
													this.ai[2] = 10f;
													if (this.type == 10)
													{
														this.ai[2] = 5f;
													}
													if (this.type == 13)
													{
														this.ai[2] = 50f;
													}
													if (this.type == 39)
													{
														this.ai[2] = 15f;
													}
													this.ai[0] = (float)NPC.NewNPC((int)this.position.X, (int)this.position.Y, this.type + 1, this.whoAmI);
												}
												else
												{
													if ((this.type == 8 || this.type == 11 || this.type == 14 || this.type == 40) && this.ai[2] > 0f)
													{
														this.ai[0] = (float)NPC.NewNPC((int)this.position.X, (int)this.position.Y, this.type, this.whoAmI);
													}
													else
													{
														this.ai[0] = (float)NPC.NewNPC((int)this.position.X, (int)this.position.Y, this.type + 1, this.whoAmI);
													}
												}
												Main.npc[(int)this.ai[0]].ai[1] = (float)this.whoAmI;
												Main.npc[(int)this.ai[0]].ai[2] = this.ai[2] - 1f;
												this.netUpdate = true;
											}
											if ((this.type == 8 || this.type == 9 || this.type == 11 || this.type == 12 || this.type == 40 || this.type == 41) && !Main.npc[(int)this.ai[1]].active)
											{
												this.life = 0;
												this.HitEffect(0, 10.0);
												this.active = false;
											}
											if ((this.type == 7 || this.type == 8 || this.type == 10 || this.type == 11 || this.type == 39 || this.type == 40) && !Main.npc[(int)this.ai[0]].active)
											{
												this.life = 0;
												this.HitEffect(0, 10.0);
												this.active = false;
											}
											if (this.type == 13 || this.type == 14 || this.type == 15)
											{
												if (!Main.npc[(int)this.ai[1]].active && !Main.npc[(int)this.ai[0]].active)
												{
													this.life = 0;
													this.HitEffect(0, 10.0);
													this.active = false;
												}
												if (this.type == 13 && !Main.npc[(int)this.ai[0]].active)
												{
													this.life = 0;
													this.HitEffect(0, 10.0);
													this.active = false;
												}
												if (this.type == 15 && !Main.npc[(int)this.ai[1]].active)
												{
													this.life = 0;
													this.HitEffect(0, 10.0);
													this.active = false;
												}
												if (this.type == 14 && !Main.npc[(int)this.ai[1]].active)
												{
													this.type = 13;
													int num42 = this.whoAmI;
													int num43 = this.life;
													float num44 = this.ai[0];
													this.SetDefaults(this.type);
													this.life = num43;
													if (this.life > this.lifeMax)
													{
														this.life = this.lifeMax;
													}
													this.ai[0] = num44;
													this.TargetClosest();
													this.netUpdate = true;
													this.whoAmI = num42;
												}
												if (this.type == 14 && !Main.npc[(int)this.ai[0]].active)
												{
													int num45 = this.life;
													int num46 = this.whoAmI;
													float num47 = this.ai[1];
													this.SetDefaults(this.type);
													this.life = num45;
													if (this.life > this.lifeMax)
													{
														this.life = this.lifeMax;
													}
													this.ai[1] = num47;
													this.TargetClosest();
													this.netUpdate = true;
													this.whoAmI = num46;
												}
												if (this.life == 0)
												{
													bool flag4 = true;
													for (int l = 0; l < 1000; l++)
													{
														if (Main.npc[l].active && (Main.npc[l].type == 13 || Main.npc[l].type == 14 || Main.npc[l].type == 15))
														{
															flag4 = false;
															break;
														}
													}
													if (flag4)
													{
														this.boss = true;
														this.NPCLoot();
													}
												}
											}
											if (!this.active && Main.netMode == 2)
											{
												NetMessage.SendData(28, -1, -1, "", this.whoAmI, -1f, 0f, 0f);
											}
										}
										int num48 = (int)(this.position.X / 16f) - 1;
										int num49 = (int)((this.position.X + (float)this.width) / 16f) + 2;
										int num50 = (int)(this.position.Y / 16f) - 1;
										int num51 = (int)((this.position.Y + (float)this.height) / 16f) + 2;
										if (num48 < 0)
										{
											num48 = 0;
										}
										if (num49 > Main.maxTilesX)
										{
											num49 = Main.maxTilesX;
										}
										if (num50 < 0)
										{
											num50 = 0;
										}
										if (num51 > Main.maxTilesY)
										{
											num51 = Main.maxTilesY;
										}
										bool flag5 = false;
										for (int m = num48; m < num49; m++)
										{
											for (int n = num50; n < num51; n++)
											{
												if (Main.tile[m, n] != null && ((Main.tile[m, n].active && (Main.tileSolid[(int)Main.tile[m, n].type] || (Main.tileSolidTop[(int)Main.tile[m, n].type] && Main.tile[m, n].frameY == 0))) || Main.tile[m, n].liquid > 64))
												{
													Vector2 vector8 = new Vector2();
													vector8.X = (float)(m * 16);
													vector8.Y = (float)(n * 16);
													if (this.position.X + (float)this.width > vector8.X && this.position.X < vector8.X + 16f && this.position.Y + (float)this.height > vector8.Y && this.position.Y < vector8.Y + 16f)
													{
														flag5 = true;
														if (Main.rand.Next(40) == 0 && Main.tile[m, n].active)
														{
															WorldGen.KillTile(m, n, true, true, false);
														}
														if (Main.netMode != 1 && Main.tile[m, n].type == 2)
														{
															byte arg_3A52_0 = Main.tile[m, n - 1].type;
														}
													}
												}
											}
										}
										float num52 = 8f;
										float num53 = 0.07f;
										if (this.type == 10)
										{
											num52 = 6f;
											num53 = 0.05f;
										}
										if (this.type == 13)
										{
											num52 = 11f;
											num53 = 0.08f;
										}
										Vector2 vector9 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
										float num54 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector9.X;
										float num55 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector9.Y;
										float num56 = (float)Math.Sqrt((double)(num54 * num54 + num55 * num55));
										if (this.ai[1] > 0f)
										{
											num54 = Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - vector9.X;
											num55 = Main.npc[(int)this.ai[1]].position.Y + (float)(Main.npc[(int)this.ai[1]].height / 2) - vector9.Y;
											this.rotation = (float)Math.Atan2((double)num55, (double)num54) + 1.57f;
											num56 = (float)Math.Sqrt((double)(num54 * num54 + num55 * num55));
											num56 = (num56 - (float)this.width) / num56;
											num54 *= num56;
											num55 *= num56;
											this.velocity = new Vector2();
											this.position.X = this.position.X + num54;
											this.position.Y = this.position.Y + num55;
											return;
										}
										if (!flag5)
										{
											this.TargetClosest();
											this.velocity.Y = this.velocity.Y + 0.11f;
											if (this.velocity.Y > num52)
											{
												this.velocity.Y = num52;
											}
											if ((double)(Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) < (double)num52 * 0.4)
											{
												if (this.velocity.X < 0f)
												{
													this.velocity.X = this.velocity.X - num53 * 1.1f;
												}
												else
												{
													this.velocity.X = this.velocity.X + num53 * 1.1f;
												}
											}
											else
											{
												if (this.velocity.Y == num52)
												{
													if (this.velocity.X < num54)
													{
														this.velocity.X = this.velocity.X + num53;
													}
													else
													{
														if (this.velocity.X > num54)
														{
															this.velocity.X = this.velocity.X - num53;
														}
													}
												}
												else
												{
													if (this.velocity.Y > 4f)
													{
														if (this.velocity.X < 0f)
														{
															this.velocity.X = this.velocity.X + num53 * 0.9f;
														}
														else
														{
															this.velocity.X = this.velocity.X - num53 * 0.9f;
														}
													}
												}
											}
										}
										else
										{
											if (this.soundDelay == 0)
											{
												float num57 = num56 / 40f;
												if (num57 < 10f)
												{
													num57 = 10f;
												}
												if (num57 > 20f)
												{
													num57 = 20f;
												}
												this.soundDelay = (int)num57;
												//Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 1);
											}
											num56 = (float)Math.Sqrt((double)(num54 * num54 + num55 * num55));
											float num58 = Math.Abs(num54);
											float num59 = Math.Abs(num55);
											num56 = num52 / num56;
											num54 *= num56;
											num55 *= num56;
											if ((this.velocity.X > 0f && num54 > 0f) || (this.velocity.X < 0f && num54 < 0f) || (this.velocity.Y > 0f && num55 > 0f) || (this.velocity.Y < 0f && num55 < 0f))
											{
												if (this.velocity.X < num54)
												{
													this.velocity.X = this.velocity.X + num53;
												}
												else
												{
													if (this.velocity.X > num54)
													{
														this.velocity.X = this.velocity.X - num53;
													}
												}
												if (this.velocity.Y < num55)
												{
													this.velocity.Y = this.velocity.Y + num53;
												}
												else
												{
													if (this.velocity.Y > num55)
													{
														this.velocity.Y = this.velocity.Y - num53;
													}
												}
											}
											else
											{
												if (num58 > num59)
												{
													if (this.velocity.X < num54)
													{
														this.velocity.X = this.velocity.X + num53 * 1.1f;
													}
													else
													{
														if (this.velocity.X > num54)
														{
															this.velocity.X = this.velocity.X - num53 * 1.1f;
														}
													}
													if ((double)(Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) < (double)num52 * 0.5)
													{
														if (this.velocity.Y > 0f)
														{
															this.velocity.Y = this.velocity.Y + num53;
														}
														else
														{
															this.velocity.Y = this.velocity.Y - num53;
														}
													}
												}
												else
												{
													if (this.velocity.Y < num55)
													{
														this.velocity.Y = this.velocity.Y + num53 * 1.1f;
													}
													else
													{
														if (this.velocity.Y > num55)
														{
															this.velocity.Y = this.velocity.Y - num53 * 1.1f;
														}
													}
													if ((double)(Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) < (double)num52 * 0.5)
													{
														if (this.velocity.X > 0f)
														{
															this.velocity.X = this.velocity.X + num53;
														}
														else
														{
															this.velocity.X = this.velocity.X - num53;
														}
													}
												}
											}
										}
										this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) + 1.57f;
										return;
									}
									else
									{
										if (this.aiStyle == 7)
										{
											int num60 = (int)(this.position.X + (float)(this.width / 2)) / 16;
											int num61 = (int)(this.position.Y + (float)this.height + 1f) / 16;
											if (Main.netMode == 1)
											{
												this.homeTileX = num60;
												this.homeTileY = num61;
											}
											bool flag6 = false;
											this.directionY = -1;
											if (this.direction == 0)
											{
												this.direction = 1;
											}
											for (int num62 = 0; num62 < 255; num62++)
											{
												if (Main.player[num62].active && Main.player[num62].talkNPC == this.whoAmI)
												{
													flag6 = true;
													if (this.ai[0] != 0f)
													{
														this.netUpdate = true;
													}
													this.ai[0] = 0f;
													this.ai[1] = 300f;
													this.ai[2] = 100f;
													if (Main.player[num62].position.X + (float)(Main.player[num62].width / 2) < this.position.X + (float)(this.width / 2))
													{
														this.direction = -1;
													}
													else
													{
														this.direction = 1;
													}
												}
											}
											if (this.ai[3] > 0f)
											{
												this.life = -1;
												this.HitEffect(0, 10.0);
												this.active = false;
												if (this.type == 37)
												{
													//Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
												}
											}
											if (this.type == 37 && Main.netMode != 1)
											{
												this.homeless = false;
												this.homeTileX = Main.dungeonX;
												this.homeTileY = Main.dungeonY;
												if (NPC.downedBoss3)
												{
													this.ai[3] = 1f;
													this.netUpdate = true;
												}
												if (!Main.dayTime && flag6 && this.ai[3] == 0f)
												{
													bool flag7 = true;
													for (int num63 = 0; num63 < 1000; num63++)
													{
														if (Main.npc[num63].active && Main.npc[num63].type == 35)
														{
															flag7 = false;
															break;
														}
													}
													if (flag7)
													{
														int num64 = NPC.NewNPC((int)this.position.X + this.width / 2, (int)this.position.Y + this.height / 2, 35, 0);
														Main.npc[num64].netUpdate = true;
														string str = "Skeletron";
														if (Main.netMode == 0)
														{
															//Main.NewText(str + " has awoken!", 175, 75, 255);
														}
														else
														{
															if (Main.netMode == 2)
															{
																NetMessage.SendData(25, -1, -1, str + " has awoken!", 255, 175f, 75f, 255f);
															}
														}
													}
													this.ai[3] = 1f;
													this.netUpdate = true;
												}
											}
											if (Main.netMode != 1 && !Main.dayTime && (num60 != this.homeTileX || num61 != this.homeTileY) && !this.homeless)
											{
												bool flag8 = true;
												for (int num65 = 0; num65 < 2; num65++)
												{
													Rectangle rectangle = new Rectangle((int)(this.position.X + (float)(this.width / 2) - (float)(Main.screenWidth / 2) - (float)NPC.safeRangeX), (int)(this.position.Y + (float)(this.height / 2) - (float)(Main.screenHeight / 2) - (float)NPC.safeRangeY), Main.screenWidth + NPC.safeRangeX * 2, Main.screenHeight + NPC.safeRangeY * 2);
													if (num65 == 1)
													{
														rectangle = new Rectangle(this.homeTileX * 16 + 8 - Main.screenWidth / 2 - NPC.safeRangeX, this.homeTileY * 16 + 8 - Main.screenHeight / 2 - NPC.safeRangeY, Main.screenWidth + NPC.safeRangeX * 2, Main.screenHeight + NPC.safeRangeY * 2);
													}
													for (int num66 = 0; num66 < 255; num66++)
													{
														if (Main.player[num66].active)
														{
															Rectangle rectangle2 = new Rectangle((int)Main.player[num66].position.X, (int)Main.player[num66].position.Y, Main.player[num66].width, Main.player[num66].height);
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
													if (this.type == 37 || !Collision.SolidTiles(this.homeTileX - 1, this.homeTileX + 1, this.homeTileY - 3, this.homeTileY - 1))
													{
														this.velocity.X = 0f;
														this.velocity.Y = 0f;
														this.position.X = (float)(this.homeTileX * 16 + 8 - this.width / 2);
														this.position.Y = (float)(this.homeTileY * 16 - this.height) - 0.1f;
														this.netUpdate = true;
													}
													else
													{
														this.homeless = true;
														WorldGen.QuickFindHome(this.whoAmI);
													}
												}
											}
											if (this.ai[0] == 0f)
											{
												if (this.ai[2] > 0f)
												{
													this.ai[2] -= 1f;
												}
												if (!Main.dayTime && !flag6)
												{
													if (Main.netMode != 1)
													{
														if (num60 == this.homeTileX && num61 == this.homeTileY)
														{
															if (this.velocity.X != 0f)
															{
																this.netUpdate = true;
															}
															if ((double)this.velocity.X > 0.1)
															{
																this.velocity.X = this.velocity.X - 0.1f;
															}
															else
															{
																if ((double)this.velocity.X < -0.1)
																{
																	this.velocity.X = this.velocity.X + 0.1f;
																}
																else
																{
																	this.velocity.X = 0f;
																}
															}
														}
														else
														{
															if (!flag6)
															{
																if (num60 > this.homeTileX)
																{
																	this.direction = -1;
																}
																else
																{
																	this.direction = 1;
																}
																this.ai[0] = 1f;
																this.ai[1] = (float)(200 + Main.rand.Next(200));
																this.ai[2] = 0f;
																this.netUpdate = true;
															}
														}
													}
												}
												else
												{
													if ((double)this.velocity.X > 0.1)
													{
														this.velocity.X = this.velocity.X - 0.1f;
													}
													else
													{
														if ((double)this.velocity.X < -0.1)
														{
															this.velocity.X = this.velocity.X + 0.1f;
														}
														else
														{
															this.velocity.X = 0f;
														}
													}
													if (Main.netMode != 1)
													{
														if (this.ai[1] > 0f)
														{
															this.ai[1] -= 1f;
														}
														if (this.ai[1] <= 0f)
														{
															this.ai[0] = 1f;
															this.ai[1] = (float)(200 + Main.rand.Next(200));
															this.ai[2] = 0f;
															this.netUpdate = true;
														}
													}
												}
												if (Main.netMode != 1 && (Main.dayTime || (num60 == this.homeTileX && num61 == this.homeTileY)))
												{
													if (num60 < this.homeTileX - 25 || num60 > this.homeTileX + 25)
													{
														if (this.ai[2] == 0f)
														{
															if (num60 < this.homeTileX - 50 && this.direction == -1)
															{
																this.direction = 1;
																this.netUpdate = true;
																return;
															}
															if (num60 > this.homeTileX + 50 && this.direction == 1)
															{
																this.direction = -1;
																this.netUpdate = true;
																return;
															}
														}
													}
													else
													{
														if (Main.rand.Next(80) == 0 && this.ai[2] == 0f)
														{
															this.ai[2] = 200f;
															this.direction *= -1;
															this.netUpdate = true;
															return;
														}
													}
												}
											}
											else
											{
												if (this.ai[0] == 1f)
												{
													if (Main.netMode != 1 && !Main.dayTime && num60 == this.homeTileX && num61 == this.homeTileY)
													{
														this.ai[0] = 0f;
														this.ai[1] = (float)(200 + Main.rand.Next(200));
														this.ai[2] = 60f;
														this.netUpdate = true;
														return;
													}
													if (Main.netMode != 1 && !this.homeless && (num60 < this.homeTileX - 35 || num60 > this.homeTileX + 35))
													{
														if (this.position.X < (float)(this.homeTileX * 16) && this.direction == -1)
														{
															this.direction = 1;
															this.velocity.X = 0.1f;
															this.netUpdate = true;
														}
														else
														{
															if (this.position.X > (float)(this.homeTileX * 16) && this.direction == 1)
															{
																this.direction = -1;
																this.velocity.X = -0.1f;
																this.netUpdate = true;
															}
														}
													}
													this.ai[1] -= 1f;
													if (this.ai[1] <= 0f)
													{
														this.ai[0] = 0f;
														this.ai[1] = (float)(300 + Main.rand.Next(300));
														this.ai[2] = 60f;
														this.netUpdate = true;
													}
													if (this.closeDoor && ((this.position.X + (float)(this.width / 2)) / 16f > (float)(this.doorX + 2) || (this.position.X + (float)(this.width / 2)) / 16f < (float)(this.doorX - 2)))
													{
														bool flag9 = WorldGen.CloseDoor(this.doorX, this.doorY, false);
														if (flag9)
														{
															this.closeDoor = false;
															NetMessage.SendData(19, -1, -1, "", 1, (float)this.doorX, (float)this.doorY, (float)this.direction);
														}
														if ((this.position.X + (float)(this.width / 2)) / 16f > (float)(this.doorX + 4) || (this.position.X + (float)(this.width / 2)) / 16f < (float)(this.doorX - 4) || (this.position.Y + (float)(this.height / 2)) / 16f > (float)(this.doorY + 4) || (this.position.Y + (float)(this.height / 2)) / 16f < (float)(this.doorY - 4))
														{
															this.closeDoor = false;
														}
													}
													if (this.velocity.X < -1f || this.velocity.X > 1f)
													{
														if (this.velocity.Y == 0f)
														{
															this.velocity *= 0.8f;
														}
													}
													else
													{
														if ((double)this.velocity.X < 1.15 && this.direction == 1)
														{
															this.velocity.X = this.velocity.X + 0.07f;
															if (this.velocity.X > 1f)
															{
																this.velocity.X = 1f;
															}
														}
														else
														{
															if (this.velocity.X > -1f && this.direction == -1)
															{
																this.velocity.X = this.velocity.X - 0.07f;
																if (this.velocity.X > 1f)
																{
																	this.velocity.X = 1f;
																}
															}
														}
													}
													if (this.velocity.Y == 0f)
													{
														if (this.position.X == this.ai[2])
														{
															this.direction *= -1;
														}
														this.ai[2] = -1f;
														int num67 = (int)((this.position.X + (float)(this.width / 2) + (float)(15 * this.direction)) / 16f);
														int num68 = (int)((this.position.Y + (float)this.height - 16f) / 16f);
														if (Main.tile[num67, num68] == null)
														{
															Main.tile[num67, num68] = new Tile();
														}
														if (Main.tile[num67, num68 - 1] == null)
														{
															Main.tile[num67, num68 - 1] = new Tile();
														}
														if (Main.tile[num67, num68 - 2] == null)
														{
															Main.tile[num67, num68 - 2] = new Tile();
														}
														if (Main.tile[num67, num68 - 3] == null)
														{
															Main.tile[num67, num68 - 3] = new Tile();
														}
														if (Main.tile[num67, num68 + 1] == null)
														{
															Main.tile[num67, num68 + 1] = new Tile();
														}
														if (Main.tile[num67 + this.direction, num68 - 1] == null)
														{
															Main.tile[num67 + this.direction, num68 - 1] = new Tile();
														}
														if (Main.tile[num67 + this.direction, num68 + 1] == null)
														{
															Main.tile[num67 + this.direction, num68 + 1] = new Tile();
														}
														if (Main.tile[num67, num68 - 2].active && Main.tile[num67, num68 - 2].type == 10 && (Main.rand.Next(10) == 0 || !Main.dayTime))
														{
															if (Main.netMode != 1)
															{
																bool flag10 = WorldGen.OpenDoor(num67, num68 - 2, this.direction);
																if (flag10)
																{
																	this.closeDoor = true;
																	this.doorX = num67;
																	this.doorY = num68 - 2;
																	NetMessage.SendData(19, -1, -1, "", 0, (float)num67, (float)(num68 - 2), (float)this.direction);
																	this.netUpdate = true;
																	this.ai[1] += 80f;
																	return;
																}
																if (WorldGen.OpenDoor(num67, num68 - 2, -this.direction))
																{
																	this.closeDoor = true;
																	this.doorX = num67;
																	this.doorY = num68 - 2;
																	NetMessage.SendData(19, -1, -1, "", 0, (float)num67, (float)(num68 - 2), (float)(-(float)this.direction));
																	this.netUpdate = true;
																	this.ai[1] += 80f;
																	return;
																}
																this.direction *= -1;
																this.netUpdate = true;
																return;
															}
														}
														else
														{
															if ((this.velocity.X < 0f && this.spriteDirection == -1) || (this.velocity.X > 0f && this.spriteDirection == 1))
															{
																if (Main.tile[num67, num68 - 2].active && Main.tileSolid[(int)Main.tile[num67, num68 - 2].type] && !Main.tileSolidTop[(int)Main.tile[num67, num68 - 2].type])
																{
																	if ((this.direction == 1 && !Collision.SolidTiles(num67 - 2, num67 - 1, num68 - 5, num68 - 1)) || (this.direction == -1 && !Collision.SolidTiles(num67 + 1, num67 + 2, num68 - 5, num68 - 1)))
																	{
																		if (!Collision.SolidTiles(num67, num67, num68 - 5, num68 - 3))
																		{
																			this.velocity.Y = -6f;
																			this.netUpdate = true;
																		}
																		else
																		{
																			this.direction *= -1;
																			this.netUpdate = true;
																		}
																	}
																	else
																	{
																		this.direction *= -1;
																		this.netUpdate = true;
																	}
																}
																else
																{
																	if (Main.tile[num67, num68 - 1].active && Main.tileSolid[(int)Main.tile[num67, num68 - 1].type] && !Main.tileSolidTop[(int)Main.tile[num67, num68 - 1].type])
																	{
																		if ((this.direction == 1 && !Collision.SolidTiles(num67 - 2, num67 - 1, num68 - 4, num68 - 1)) || (this.direction == -1 && !Collision.SolidTiles(num67 + 1, num67 + 2, num68 - 4, num68 - 1)))
																		{
																			if (!Collision.SolidTiles(num67, num67, num68 - 4, num68 - 2))
																			{
																				this.velocity.Y = -5f;
																				this.netUpdate = true;
																			}
																			else
																			{
																				this.direction *= -1;
																				this.netUpdate = true;
																			}
																		}
																		else
																		{
																			this.direction *= -1;
																			this.netUpdate = true;
																		}
																	}
																	else
																	{
																		if (Main.tile[num67, num68].active && Main.tileSolid[(int)Main.tile[num67, num68].type] && !Main.tileSolidTop[(int)Main.tile[num67, num68].type])
																		{
																			if ((this.direction == 1 && !Collision.SolidTiles(num67 - 2, num67, num68 - 3, num68 - 1)) || (this.direction == -1 && !Collision.SolidTiles(num67, num67 + 2, num68 - 3, num68 - 1)))
																			{
																				this.velocity.Y = -3.6f;
																				this.netUpdate = true;
																			}
																			else
																			{
																				this.direction *= -1;
																				this.netUpdate = true;
																			}
																		}
																		else
																		{
																			if (num60 >= this.homeTileX - 35 && num60 <= this.homeTileX + 35 && (!Main.tile[num67, num68 + 1].active || !Main.tileSolid[(int)Main.tile[num67, num68 + 1].type]) && (!Main.tile[num67 - this.direction, num68 + 1].active || !Main.tileSolid[(int)Main.tile[num67 - this.direction, num68 + 1].type]) && (!Main.tile[num67, num68 + 2].active || !Main.tileSolid[(int)Main.tile[num67, num68 + 2].type]) && (!Main.tile[num67 - this.direction, num68 + 2].active || !Main.tileSolid[(int)Main.tile[num67 - this.direction, num68 + 2].type]) && (!Main.tile[num67, num68 + 3].active || !Main.tileSolid[(int)Main.tile[num67, num68 + 3].type]) && (!Main.tile[num67 - this.direction, num68 + 3].active || !Main.tileSolid[(int)Main.tile[num67 - this.direction, num68 + 3].type]) && (!Main.tile[num67, num68 + 4].active || !Main.tileSolid[(int)Main.tile[num67, num68 + 4].type]) && (!Main.tile[num67 - this.direction, num68 + 4].active || !Main.tileSolid[(int)Main.tile[num67 - this.direction, num68 + 4].type]))
																			{
																				this.direction *= -1;
																				this.velocity.X = this.velocity.X * -1f;
																				this.netUpdate = true;
																			}
																		}
																	}
																}
																if (this.velocity.Y < 0f)
																{
																	this.ai[2] = this.position.X;
																}
															}
															if (this.velocity.Y < 0f && this.wet)
															{
																this.velocity.Y = this.velocity.Y * 1.2f;
																return;
															}
														}
													}
												}
											}
										}
										else
										{
											if (this.aiStyle == 8)
											{
												this.TargetClosest();
												this.velocity.X = this.velocity.X * 0.93f;
												if ((double)this.velocity.X > -0.1 && (double)this.velocity.X < 0.1)
												{
													this.velocity.X = 0f;
												}
												if (this.ai[0] == 0f)
												{
													this.ai[0] = 500f;
												}
												if (this.ai[2] != 0f && this.ai[3] != 0f)
												{
													//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 8);
													for (int num69 = 0; num69 < 50; num69++)
													{
														if (this.type == 29 || this.type == 45)
														{
															Vector2 arg_5789_0 = new Vector2(this.position.X, this.position.Y);
															int arg_5789_1 = this.width;
															int arg_5789_2 = this.height;
															int arg_5789_3 = 27;
															float arg_5789_4 = 0f;
															float arg_5789_5 = 0f;
															int arg_5789_6 = 100;
															Color newColor = default(Color);
															int num70 = Dust.NewDust(arg_5789_0, arg_5789_1, arg_5789_2, arg_5789_3, arg_5789_4, arg_5789_5, arg_5789_6, newColor, (float)Main.rand.Next(1, 3));
															Dust expr_5798 = Main.dust[num70];
															expr_5798.velocity *= 3f;
															if (Main.dust[num70].scale > 1f)
															{
																Main.dust[num70].noGravity = true;
															}
														}
														else
														{
															if (this.type == 32)
															{
																Vector2 arg_5825_0 = new Vector2(this.position.X, this.position.Y);
																int arg_5825_1 = this.width;
																int arg_5825_2 = this.height;
																int arg_5825_3 = 29;
																float arg_5825_4 = 0f;
																float arg_5825_5 = 0f;
																int arg_5825_6 = 100;
																Color newColor = default(Color);
																int num71 = Dust.NewDust(arg_5825_0, arg_5825_1, arg_5825_2, arg_5825_3, arg_5825_4, arg_5825_5, arg_5825_6, newColor, 2.5f);
																Dust expr_5834 = Main.dust[num71];
																expr_5834.velocity *= 3f;
																Main.dust[num71].noGravity = true;
															}
															else
															{
																Vector2 arg_589C_0 = new Vector2(this.position.X, this.position.Y);
																int arg_589C_1 = this.width;
																int arg_589C_2 = this.height;
																int arg_589C_3 = 6;
																float arg_589C_4 = 0f;
																float arg_589C_5 = 0f;
																int arg_589C_6 = 100;
																Color newColor = default(Color);
																int num72 = Dust.NewDust(arg_589C_0, arg_589C_1, arg_589C_2, arg_589C_3, arg_589C_4, arg_589C_5, arg_589C_6, newColor, 2.5f);
																Dust expr_58AB = Main.dust[num72];
																expr_58AB.velocity *= 3f;
																Main.dust[num72].noGravity = true;
															}
														}
													}
													this.position.X = this.ai[2] * 16f - (float)(this.width / 2) + 8f;
													this.position.Y = this.ai[3] * 16f - (float)this.height;
													this.velocity.X = 0f;
													this.velocity.Y = 0f;
													this.ai[2] = 0f;
													this.ai[3] = 0f;
													//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 8);
													for (int num73 = 0; num73 < 50; num73++)
													{
														if (this.type == 29 || this.type == 45)
														{
															Vector2 arg_59EB_0 = new Vector2(this.position.X, this.position.Y);
															int arg_59EB_1 = this.width;
															int arg_59EB_2 = this.height;
															int arg_59EB_3 = 27;
															float arg_59EB_4 = 0f;
															float arg_59EB_5 = 0f;
															int arg_59EB_6 = 100;
															Color newColor = default(Color);
															int num74 = Dust.NewDust(arg_59EB_0, arg_59EB_1, arg_59EB_2, arg_59EB_3, arg_59EB_4, arg_59EB_5, arg_59EB_6, newColor, (float)Main.rand.Next(1, 3));
															Dust expr_59FA = Main.dust[num74];
															expr_59FA.velocity *= 3f;
															if (Main.dust[num74].scale > 1f)
															{
																Main.dust[num74].noGravity = true;
															}
														}
														else
														{
															if (this.type == 32)
															{
																Vector2 arg_5A87_0 = new Vector2(this.position.X, this.position.Y);
																int arg_5A87_1 = this.width;
																int arg_5A87_2 = this.height;
																int arg_5A87_3 = 29;
																float arg_5A87_4 = 0f;
																float arg_5A87_5 = 0f;
																int arg_5A87_6 = 100;
																Color newColor = default(Color);
																int num75 = Dust.NewDust(arg_5A87_0, arg_5A87_1, arg_5A87_2, arg_5A87_3, arg_5A87_4, arg_5A87_5, arg_5A87_6, newColor, 2.5f);
																Dust expr_5A96 = Main.dust[num75];
																expr_5A96.velocity *= 3f;
																Main.dust[num75].noGravity = true;
															}
															else
															{
																Vector2 arg_5AFE_0 = new Vector2(this.position.X, this.position.Y);
																int arg_5AFE_1 = this.width;
																int arg_5AFE_2 = this.height;
																int arg_5AFE_3 = 6;
																float arg_5AFE_4 = 0f;
																float arg_5AFE_5 = 0f;
																int arg_5AFE_6 = 100;
																Color newColor = default(Color);
																int num76 = Dust.NewDust(arg_5AFE_0, arg_5AFE_1, arg_5AFE_2, arg_5AFE_3, arg_5AFE_4, arg_5AFE_5, arg_5AFE_6, newColor, 2.5f);
																Dust expr_5B0D = Main.dust[num76];
																expr_5B0D.velocity *= 3f;
																Main.dust[num76].noGravity = true;
															}
														}
													}
												}
												this.ai[0] += 1f;
												if (this.ai[0] == 75f || this.ai[0] == 150f || this.ai[0] == 225f)
												{
													this.ai[1] = 30f;
													this.netUpdate = true;
												}
												else
												{
													if (this.ai[0] >= 450f && Main.netMode != 1)
													{
														this.ai[0] = 1f;
														int num77 = (int)Main.player[this.target].position.X / 16;
														int num78 = (int)Main.player[this.target].position.Y / 16;
														int num79 = (int)this.position.X / 16;
														int num80 = (int)this.position.Y / 16;
														int num81 = 20;
														int num82 = 0;
														bool flag11 = false;
														if (Math.Abs(this.position.X - Main.player[this.target].position.X) + Math.Abs(this.position.Y - Main.player[this.target].position.Y) > 2000f)
														{
															num82 = 100;
															flag11 = true;
														}
														while (!flag11 && num82 < 100)
														{
															num82++;
															int num83 = Main.rand.Next(num77 - num81, num77 + num81);
															int num84 = Main.rand.Next(num78 - num81, num78 + num81);
															for (int num85 = num84; num85 < num78 + num81; num85++)
															{
																if ((num85 < num78 - 4 || num85 > num78 + 4 || num83 < num77 - 4 || num83 > num77 + 4) && (num85 < num80 - 1 || num85 > num80 + 1 || num83 < num79 - 1 || num83 > num79 + 1) && Main.tile[num83, num85].active)
																{
																	bool flag12 = true;
																	if (this.type == 32 && Main.tile[num83, num85 - 1].wall == 0)
																	{
																		flag12 = false;
																	}
																	else
																	{
																		if (Main.tile[num83, num85 - 1].lava)
																		{
																			flag12 = false;
																		}
																	}
																	if (flag12 && Main.tileSolid[(int)Main.tile[num83, num85].type] && !Collision.SolidTiles(num83 - 1, num83 + 1, num85 - 4, num85 - 1))
																	{
																		this.ai[1] = 20f;
																		this.ai[2] = (float)num83;
																		this.ai[3] = (float)num85;
																		flag11 = true;
																		break;
																	}
																}
															}
														}
														this.netUpdate = true;
													}
												}
												if (this.ai[1] > 0f)
												{
													this.ai[1] -= 1f;
													if (this.ai[1] == 25f)
													{
														//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 8);
														if (Main.netMode != 1)
														{
															if (this.type == 29 || this.type == 45)
															{
																NPC.NewNPC((int)this.position.X + this.width / 2, (int)this.position.Y - 8, 30, 0);
															}
															else
															{
																if (this.type == 32)
																{
																	NPC.NewNPC((int)this.position.X + this.width / 2, (int)this.position.Y - 8, 33, 0);
																}
																else
																{
																	NPC.NewNPC((int)this.position.X + this.width / 2 + this.direction * 8, (int)this.position.Y + 20, 25, 0);
																}
															}
														}
													}
												}
												if (this.type == 29 || this.type == 45)
												{
													if (Main.rand.Next(5) == 0)
													{
														Vector2 arg_5F99_0 = new Vector2(this.position.X, this.position.Y + 2f);
														int arg_5F99_1 = this.width;
														int arg_5F99_2 = this.height;
														int arg_5F99_3 = 27;
														float arg_5F99_4 = this.velocity.X * 0.2f;
														float arg_5F99_5 = this.velocity.Y * 0.2f;
														int arg_5F99_6 = 100;
														Color newColor = default(Color);
														int num86 = Dust.NewDust(arg_5F99_0, arg_5F99_1, arg_5F99_2, arg_5F99_3, arg_5F99_4, arg_5F99_5, arg_5F99_6, newColor, 1.5f);
														Main.dust[num86].noGravity = true;
														Dust expr_5FBB_cp_0 = Main.dust[num86];
														expr_5FBB_cp_0.velocity.X = expr_5FBB_cp_0.velocity.X * 0.5f;
														Main.dust[num86].velocity.Y = -2f;
														return;
													}
												}
												else
												{
													if (this.type == 32)
													{
														if (Main.rand.Next(2) == 0)
														{
															Vector2 arg_6063_0 = new Vector2(this.position.X, this.position.Y + 2f);
															int arg_6063_1 = this.width;
															int arg_6063_2 = this.height;
															int arg_6063_3 = 29;
															float arg_6063_4 = this.velocity.X * 0.2f;
															float arg_6063_5 = this.velocity.Y * 0.2f;
															int arg_6063_6 = 100;
															Color newColor = default(Color);
															int num87 = Dust.NewDust(arg_6063_0, arg_6063_1, arg_6063_2, arg_6063_3, arg_6063_4, arg_6063_5, arg_6063_6, newColor, 2f);
															Main.dust[num87].noGravity = true;
															Dust expr_6085_cp_0 = Main.dust[num87];
															expr_6085_cp_0.velocity.X = expr_6085_cp_0.velocity.X * 1f;
															Dust expr_60A3_cp_0 = Main.dust[num87];
															expr_60A3_cp_0.velocity.Y = expr_60A3_cp_0.velocity.Y * 1f;
															return;
														}
													}
													else
													{
														if (Main.rand.Next(2) == 0)
														{
															Vector2 arg_6126_0 = new Vector2(this.position.X, this.position.Y + 2f);
															int arg_6126_1 = this.width;
															int arg_6126_2 = this.height;
															int arg_6126_3 = 6;
															float arg_6126_4 = this.velocity.X * 0.2f;
															float arg_6126_5 = this.velocity.Y * 0.2f;
															int arg_6126_6 = 100;
															Color newColor = default(Color);
															int num88 = Dust.NewDust(arg_6126_0, arg_6126_1, arg_6126_2, arg_6126_3, arg_6126_4, arg_6126_5, arg_6126_6, newColor, 2f);
															Main.dust[num88].noGravity = true;
															Dust expr_6148_cp_0 = Main.dust[num88];
															expr_6148_cp_0.velocity.X = expr_6148_cp_0.velocity.X * 1f;
															Dust expr_6166_cp_0 = Main.dust[num88];
															expr_6166_cp_0.velocity.Y = expr_6166_cp_0.velocity.Y * 1f;
															return;
														}
													}
												}
											}
											else
											{
												if (this.aiStyle == 9)
												{
													if (this.target == 255)
													{
														this.TargetClosest();
														float num89 = 6f;
														if (this.type == 30)
														{
															NPC.maxSpawns = 8;
														}
														Vector2 vector10 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
														float num90 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector10.X;
														float num91 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector10.Y;
														float num92 = (float)Math.Sqrt((double)(num90 * num90 + num91 * num91));
														num92 = num89 / num92;
														this.velocity.X = num90 * num92;
														this.velocity.Y = num91 * num92;
													}
													if (this.timeLeft > 100)
													{
														this.timeLeft = 100;
													}
													for (int num93 = 0; num93 < 2; num93++)
													{
														if (this.type == 30)
														{
															Vector2 arg_6319_0 = new Vector2(this.position.X, this.position.Y + 2f);
															int arg_6319_1 = this.width;
															int arg_6319_2 = this.height;
															int arg_6319_3 = 27;
															float arg_6319_4 = this.velocity.X * 0.2f;
															float arg_6319_5 = this.velocity.Y * 0.2f;
															int arg_6319_6 = 100;
															Color newColor = default(Color);
															int num94 = Dust.NewDust(arg_6319_0, arg_6319_1, arg_6319_2, arg_6319_3, arg_6319_4, arg_6319_5, arg_6319_6, newColor, 2f);
															Main.dust[num94].noGravity = true;
															Dust expr_6336 = Main.dust[num94];
															expr_6336.velocity *= 0.3f;
															Dust expr_6358_cp_0 = Main.dust[num94];
															expr_6358_cp_0.velocity.X = expr_6358_cp_0.velocity.X - this.velocity.X * 0.2f;
															Dust expr_6382_cp_0 = Main.dust[num94];
															expr_6382_cp_0.velocity.Y = expr_6382_cp_0.velocity.Y - this.velocity.Y * 0.2f;
														}
														else
														{
															if (this.type == 33)
															{
																Vector2 arg_6413_0 = new Vector2(this.position.X, this.position.Y + 2f);
																int arg_6413_1 = this.width;
																int arg_6413_2 = this.height;
																int arg_6413_3 = 29;
																float arg_6413_4 = this.velocity.X * 0.2f;
																float arg_6413_5 = this.velocity.Y * 0.2f;
																int arg_6413_6 = 100;
																Color newColor = default(Color);
																int num95 = Dust.NewDust(arg_6413_0, arg_6413_1, arg_6413_2, arg_6413_3, arg_6413_4, arg_6413_5, arg_6413_6, newColor, 2f);
																Main.dust[num95].noGravity = true;
																Dust expr_6435_cp_0 = Main.dust[num95];
																expr_6435_cp_0.velocity.X = expr_6435_cp_0.velocity.X * 0.3f;
																Dust expr_6453_cp_0 = Main.dust[num95];
																expr_6453_cp_0.velocity.Y = expr_6453_cp_0.velocity.Y * 0.3f;
															}
															else
															{
																Vector2 arg_64CA_0 = new Vector2(this.position.X, this.position.Y + 2f);
																int arg_64CA_1 = this.width;
																int arg_64CA_2 = this.height;
																int arg_64CA_3 = 6;
																float arg_64CA_4 = this.velocity.X * 0.2f;
																float arg_64CA_5 = this.velocity.Y * 0.2f;
																int arg_64CA_6 = 100;
																Color newColor = default(Color);
																int num96 = Dust.NewDust(arg_64CA_0, arg_64CA_1, arg_64CA_2, arg_64CA_3, arg_64CA_4, arg_64CA_5, arg_64CA_6, newColor, 2f);
																Main.dust[num96].noGravity = true;
																Dust expr_64EC_cp_0 = Main.dust[num96];
																expr_64EC_cp_0.velocity.X = expr_64EC_cp_0.velocity.X * 0.3f;
																Dust expr_650A_cp_0 = Main.dust[num96];
																expr_650A_cp_0.velocity.Y = expr_650A_cp_0.velocity.Y * 0.3f;
															}
														}
													}
													this.rotation += 0.4f * (float)this.direction;
													return;
												}
												if (this.aiStyle == 10)
												{
													if (this.collideX)
													{
														this.velocity.X = this.oldVelocity.X * -0.5f;
														if (this.direction == -1 && this.velocity.X > 0f && this.velocity.X < 2f)
														{
															this.velocity.X = 2f;
														}
														if (this.direction == 1 && this.velocity.X < 0f && this.velocity.X > -2f)
														{
															this.velocity.X = -2f;
														}
													}
													if (this.collideY)
													{
														this.velocity.Y = this.oldVelocity.Y * -0.5f;
														if (this.velocity.Y > 0f && this.velocity.Y < 1f)
														{
															this.velocity.Y = 1f;
														}
														if (this.velocity.Y < 0f && this.velocity.Y > -1f)
														{
															this.velocity.Y = -1f;
														}
													}
													this.TargetClosest();
													if (this.direction == -1 && this.velocity.X > -4f)
													{
														this.velocity.X = this.velocity.X - 0.1f;
														if (this.velocity.X > 4f)
														{
															this.velocity.X = this.velocity.X - 0.1f;
														}
														else
														{
															if (this.velocity.X > 0f)
															{
																this.velocity.X = this.velocity.X + 0.05f;
															}
														}
														if (this.velocity.X < -4f)
														{
															this.velocity.X = -4f;
														}
													}
													else
													{
														if (this.direction == 1 && this.velocity.X < 4f)
														{
															this.velocity.X = this.velocity.X + 0.1f;
															if (this.velocity.X < -4f)
															{
																this.velocity.X = this.velocity.X + 0.1f;
															}
															else
															{
																if (this.velocity.X < 0f)
																{
																	this.velocity.X = this.velocity.X - 0.05f;
																}
															}
															if (this.velocity.X > 4f)
															{
																this.velocity.X = 4f;
															}
														}
													}
													if (this.directionY == -1 && (double)this.velocity.Y > -1.5)
													{
														this.velocity.Y = this.velocity.Y - 0.04f;
														if ((double)this.velocity.Y > 1.5)
														{
															this.velocity.Y = this.velocity.Y - 0.05f;
														}
														else
														{
															if (this.velocity.Y > 0f)
															{
																this.velocity.Y = this.velocity.Y + 0.03f;
															}
														}
														if ((double)this.velocity.Y < -1.5)
														{
															this.velocity.Y = -1.5f;
														}
													}
													else
													{
														if (this.directionY == 1 && (double)this.velocity.Y < 1.5)
														{
															this.velocity.Y = this.velocity.Y + 0.04f;
															if ((double)this.velocity.Y < -1.5)
															{
																this.velocity.Y = this.velocity.Y + 0.05f;
															}
															else
															{
																if (this.velocity.Y < 0f)
																{
																	this.velocity.Y = this.velocity.Y - 0.03f;
																}
															}
															if ((double)this.velocity.Y > 1.5)
															{
																this.velocity.Y = 1.5f;
															}
														}
													}
													this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) - 1.57f;
													Vector2 arg_6A0A_0 = new Vector2(this.position.X - this.velocity.X, this.position.Y - this.velocity.Y);
													int arg_6A0A_1 = this.width;
													int arg_6A0A_2 = this.height;
													int arg_6A0A_3 = 6;
													float arg_6A0A_4 = this.velocity.X * 0.2f;
													float arg_6A0A_5 = this.velocity.Y * 0.2f;
													int arg_6A0A_6 = 100;
													Color newColor = default(Color);
													int num97 = Dust.NewDust(arg_6A0A_0, arg_6A0A_1, arg_6A0A_2, arg_6A0A_3, arg_6A0A_4, arg_6A0A_5, arg_6A0A_6, newColor, 2f);
													Main.dust[num97].noGravity = true;
													Main.dust[num97].noLight = true;
													Dust expr_6A3A_cp_0 = Main.dust[num97];
													expr_6A3A_cp_0.velocity.X = expr_6A3A_cp_0.velocity.X * 0.3f;
													Dust expr_6A58_cp_0 = Main.dust[num97];
													expr_6A58_cp_0.velocity.Y = expr_6A58_cp_0.velocity.Y * 0.3f;
													return;
												}
												if (this.aiStyle == 11)
												{
													if (this.ai[0] == 0f && Main.netMode != 1)
													{
														this.TargetClosest();
														this.ai[0] = 1f;
														int num98 = NPC.NewNPC((int)(this.position.X + (float)(this.width / 2)), (int)this.position.Y + this.height / 2, 36, this.whoAmI);
														Main.npc[num98].ai[0] = -1f;
														Main.npc[num98].ai[1] = (float)this.whoAmI;
														Main.npc[num98].target = this.target;
														Main.npc[num98].netUpdate = true;
														num98 = NPC.NewNPC((int)(this.position.X + (float)(this.width / 2)), (int)this.position.Y + this.height / 2, 36, this.whoAmI);
														Main.npc[num98].ai[0] = 1f;
														Main.npc[num98].ai[1] = (float)this.whoAmI;
														Main.npc[num98].ai[3] = 150f;
														Main.npc[num98].target = this.target;
														Main.npc[num98].netUpdate = true;
													}
													if (Main.player[this.target].dead || Math.Abs(this.position.X - Main.player[this.target].position.X) > 2000f || Math.Abs(this.position.Y - Main.player[this.target].position.Y) > 2000f)
													{
														this.TargetClosest();
														if (Main.player[this.target].dead || Math.Abs(this.position.X - Main.player[this.target].position.X) > 2000f || Math.Abs(this.position.Y - Main.player[this.target].position.Y) > 2000f)
														{
															this.ai[1] = 3f;
														}
													}
													if (Main.dayTime && this.ai[1] != 3f && this.ai[1] != 2f)
													{
														this.ai[1] = 2f;
														//Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
													}
													if (this.ai[1] == 0f)
													{
														this.ai[2] += 1f;
														if (this.ai[2] >= 800f)
														{
															this.ai[2] = 0f;
															this.ai[1] = 1f;
															this.TargetClosest();
															this.netUpdate = true;
														}
														this.rotation = this.velocity.X / 15f;
														if (this.position.Y > Main.player[this.target].position.Y - 250f)
														{
															if (this.velocity.Y > 0f)
															{
																this.velocity.Y = this.velocity.Y * 0.98f;
															}
															this.velocity.Y = this.velocity.Y - 0.02f;
															if (this.velocity.Y > 2f)
															{
																this.velocity.Y = 2f;
															}
														}
														else
														{
															if (this.position.Y < Main.player[this.target].position.Y - 250f)
															{
																if (this.velocity.Y < 0f)
																{
																	this.velocity.Y = this.velocity.Y * 0.98f;
																}
																this.velocity.Y = this.velocity.Y + 0.02f;
																if (this.velocity.Y < -2f)
																{
																	this.velocity.Y = -2f;
																}
															}
														}
														if (this.position.X + (float)(this.width / 2) > Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2))
														{
															if (this.velocity.X > 0f)
															{
																this.velocity.X = this.velocity.X * 0.98f;
															}
															this.velocity.X = this.velocity.X - 0.05f;
															if (this.velocity.X > 8f)
															{
																this.velocity.X = 8f;
															}
														}
														if (this.position.X + (float)(this.width / 2) < Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2))
														{
															if (this.velocity.X < 0f)
															{
																this.velocity.X = this.velocity.X * 0.98f;
															}
															this.velocity.X = this.velocity.X + 0.05f;
															if (this.velocity.X < -8f)
															{
																this.velocity.X = -8f;
															}
														}
													}
													else
													{
														if (this.ai[1] == 1f)
														{
															this.ai[2] += 1f;
															if (this.ai[2] == 2f)
															{
																//Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
															}
															if (this.ai[2] >= 400f)
															{
																this.ai[2] = 0f;
																this.ai[1] = 0f;
															}
															this.rotation += (float)this.direction * 0.3f;
															Vector2 vector11 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
															float num99 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector11.X;
															float num100 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector11.Y;
															float num101 = (float)Math.Sqrt((double)(num99 * num99 + num100 * num100));
															num101 = 2.5f / num101;
															this.velocity.X = num99 * num101;
															this.velocity.Y = num100 * num101;
														}
														else
														{
															if (this.ai[1] == 2f)
															{
																this.damage = 9999;
																this.defense = 9999;
																this.rotation += (float)this.direction * 0.3f;
																Vector2 vector12 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
																float num102 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector12.X;
																float num103 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector12.Y;
																float num104 = (float)Math.Sqrt((double)(num102 * num102 + num103 * num103));
																num104 = 8f / num104;
																this.velocity.X = num102 * num104;
																this.velocity.Y = num103 * num104;
															}
															else
															{
																if (this.ai[1] == 3f)
																{
																	this.velocity.Y = this.velocity.Y - 0.1f;
																	if (this.velocity.Y > 0f)
																	{
																		this.velocity.Y = this.velocity.Y * 0.95f;
																	}
																	this.velocity.X = this.velocity.X * 0.95f;
																	if (this.timeLeft > 50)
																	{
																		this.timeLeft = 50;
																	}
																}
															}
														}
													}
													if (this.ai[1] != 2f && this.ai[1] != 3f)
													{
														Vector2 arg_73B7_0 = new Vector2(this.position.X + (float)(this.width / 2) - 15f - this.velocity.X * 5f, this.position.Y + (float)this.height - 2f);
														int arg_73B7_1 = 30;
														int arg_73B7_2 = 10;
														int arg_73B7_3 = 5;
														float arg_73B7_4 = -this.velocity.X * 0.2f;
														float arg_73B7_5 = 3f;
														int arg_73B7_6 = 0;
														Color newColor = default(Color);
														int num105 = Dust.NewDust(arg_73B7_0, arg_73B7_1, arg_73B7_2, arg_73B7_3, arg_73B7_4, arg_73B7_5, arg_73B7_6, newColor, 2f);
														Main.dust[num105].noGravity = true;
														Dust expr_73D9_cp_0 = Main.dust[num105];
														expr_73D9_cp_0.velocity.X = expr_73D9_cp_0.velocity.X * 1.3f;
														Dust expr_73F7_cp_0 = Main.dust[num105];
														expr_73F7_cp_0.velocity.X = expr_73F7_cp_0.velocity.X + this.velocity.X * 0.4f;
														Dust expr_7421_cp_0 = Main.dust[num105];
														expr_7421_cp_0.velocity.Y = expr_7421_cp_0.velocity.Y + (2f + this.velocity.Y);
														for (int num106 = 0; num106 < 2; num106++)
														{
															Vector2 arg_7496_0 = new Vector2(this.position.X, this.position.Y + 120f);
															int arg_7496_1 = this.width;
															int arg_7496_2 = 60;
															int arg_7496_3 = 5;
															float arg_7496_4 = this.velocity.X;
															float arg_7496_5 = this.velocity.Y;
															int arg_7496_6 = 0;
															newColor = default(Color);
															num105 = Dust.NewDust(arg_7496_0, arg_7496_1, arg_7496_2, arg_7496_3, arg_7496_4, arg_7496_5, arg_7496_6, newColor, 2f);
															Main.dust[num105].noGravity = true;
															Dust expr_74B3 = Main.dust[num105];
															expr_74B3.velocity -= this.velocity;
															Dust expr_74D6_cp_0 = Main.dust[num105];
															expr_74D6_cp_0.velocity.Y = expr_74D6_cp_0.velocity.Y + 5f;
														}
														return;
													}
												}
												else
												{
													if (this.aiStyle == 12)
													{
														this.spriteDirection = -(int)this.ai[0];
														if (!Main.npc[(int)this.ai[1]].active || Main.npc[(int)this.ai[1]].aiStyle != 11)
														{
															this.ai[2] += 10f;
															if (this.ai[2] > 50f || Main.netMode != 2)
															{
																this.life = -1;
																this.HitEffect(0, 10.0);
																this.active = false;
															}
														}
														if (this.ai[2] == 0f || this.ai[2] == 3f)
														{
															if (Main.npc[(int)this.ai[1]].ai[1] == 3f && this.timeLeft > 10)
															{
																this.timeLeft = 10;
															}
															if (Main.npc[(int)this.ai[1]].ai[1] != 0f)
															{
																if (this.position.Y > Main.npc[(int)this.ai[1]].position.Y - 100f)
																{
																	if (this.velocity.Y > 0f)
																	{
																		this.velocity.Y = this.velocity.Y * 0.96f;
																	}
																	this.velocity.Y = this.velocity.Y - 0.07f;
																	if (this.velocity.Y > 6f)
																	{
																		this.velocity.Y = 6f;
																	}
																}
																else
																{
																	if (this.position.Y < Main.npc[(int)this.ai[1]].position.Y - 100f)
																	{
																		if (this.velocity.Y < 0f)
																		{
																			this.velocity.Y = this.velocity.Y * 0.96f;
																		}
																		this.velocity.Y = this.velocity.Y + 0.07f;
																		if (this.velocity.Y < -6f)
																		{
																			this.velocity.Y = -6f;
																		}
																	}
																}
																if (this.position.X + (float)(this.width / 2) > Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 120f * this.ai[0])
																{
																	if (this.velocity.X > 0f)
																	{
																		this.velocity.X = this.velocity.X * 0.96f;
																	}
																	this.velocity.X = this.velocity.X - 0.1f;
																	if (this.velocity.X > 8f)
																	{
																		this.velocity.X = 8f;
																	}
																}
																if (this.position.X + (float)(this.width / 2) < Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 120f * this.ai[0])
																{
																	if (this.velocity.X < 0f)
																	{
																		this.velocity.X = this.velocity.X * 0.96f;
																	}
																	this.velocity.X = this.velocity.X + 0.1f;
																	if (this.velocity.X < -8f)
																	{
																		this.velocity.X = -8f;
																	}
																}
															}
															else
															{
																this.ai[3] += 1f;
																if (this.ai[3] >= 300f)
																{
																	this.ai[2] += 1f;
																	this.ai[3] = 0f;
																	this.netUpdate = true;
																}
																if (this.position.Y > Main.npc[(int)this.ai[1]].position.Y + 230f)
																{
																	if (this.velocity.Y > 0f)
																	{
																		this.velocity.Y = this.velocity.Y * 0.96f;
																	}
																	this.velocity.Y = this.velocity.Y - 0.04f;
																	if (this.velocity.Y > 3f)
																	{
																		this.velocity.Y = 3f;
																	}
																}
																else
																{
																	if (this.position.Y < Main.npc[(int)this.ai[1]].position.Y + 230f)
																	{
																		if (this.velocity.Y < 0f)
																		{
																			this.velocity.Y = this.velocity.Y * 0.96f;
																		}
																		this.velocity.Y = this.velocity.Y + 0.04f;
																		if (this.velocity.Y < -3f)
																		{
																			this.velocity.Y = -3f;
																		}
																	}
																}
																if (this.position.X + (float)(this.width / 2) > Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 200f * this.ai[0])
																{
																	if (this.velocity.X > 0f)
																	{
																		this.velocity.X = this.velocity.X * 0.96f;
																	}
																	this.velocity.X = this.velocity.X - 0.07f;
																	if (this.velocity.X > 8f)
																	{
																		this.velocity.X = 8f;
																	}
																}
																if (this.position.X + (float)(this.width / 2) < Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 200f * this.ai[0])
																{
																	if (this.velocity.X < 0f)
																	{
																		this.velocity.X = this.velocity.X * 0.96f;
																	}
																	this.velocity.X = this.velocity.X + 0.07f;
																	if (this.velocity.X < -8f)
																	{
																		this.velocity.X = -8f;
																	}
																}
															}
															Vector2 vector13 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
															float num107 = Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 200f * this.ai[0] - vector13.X;
															float num108 = Main.npc[(int)this.ai[1]].position.Y + 230f - vector13.Y;
															Math.Sqrt((double)(num107 * num107 + num108 * num108));
															this.rotation = (float)Math.Atan2((double)num108, (double)num107) + 1.57f;
															return;
														}
														if (this.ai[2] == 1f)
														{
															Vector2 vector14 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
															float num109 = Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 200f * this.ai[0] - vector14.X;
															float num110 = Main.npc[(int)this.ai[1]].position.Y + 230f - vector14.Y;
															float num111 = (float)Math.Sqrt((double)(num109 * num109 + num110 * num110));
															this.rotation = (float)Math.Atan2((double)num110, (double)num109) + 1.57f;
															this.velocity.X = this.velocity.X * 0.95f;
															this.velocity.Y = this.velocity.Y - 0.1f;
															if (this.velocity.Y < -8f)
															{
																this.velocity.Y = -8f;
															}
															if (this.position.Y < Main.npc[(int)this.ai[1]].position.Y - 200f)
															{
																this.TargetClosest();
																this.ai[2] = 2f;
																vector14 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
																num109 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector14.X;
																num110 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector14.Y;
																num111 = (float)Math.Sqrt((double)(num109 * num109 + num110 * num110));
																num111 = 20f / num111;
																this.velocity.X = num109 * num111;
																this.velocity.Y = num110 * num111;
																this.netUpdate = true;
																return;
															}
														}
														else
														{
															if (this.ai[2] == 2f)
															{
																if (this.position.Y > Main.player[this.target].position.Y || this.velocity.Y < 0f)
																{
																	this.ai[2] = 3f;
																	return;
																}
															}
															else
															{
																if (this.ai[2] == 4f)
																{
																	Vector2 vector15 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
																	float num112 = Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 200f * this.ai[0] - vector15.X;
																	float num113 = Main.npc[(int)this.ai[1]].position.Y + 230f - vector15.Y;
																	float num114 = (float)Math.Sqrt((double)(num112 * num112 + num113 * num113));
																	this.rotation = (float)Math.Atan2((double)num113, (double)num112) + 1.57f;
																	this.velocity.Y = this.velocity.Y * 0.95f;
																	this.velocity.X = this.velocity.X + 0.1f * -this.ai[0];
																	if (this.velocity.X < -8f)
																	{
																		this.velocity.X = -8f;
																	}
																	if (this.velocity.X > 8f)
																	{
																		this.velocity.X = 8f;
																	}
																	if (this.position.X + (float)(this.width / 2) < Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 500f || this.position.X + (float)(this.width / 2) > Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) + 500f)
																	{
																		this.TargetClosest();
																		this.ai[2] = 5f;
																		vector15 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
																		num112 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector15.X;
																		num113 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector15.Y;
																		num114 = (float)Math.Sqrt((double)(num112 * num112 + num113 * num113));
																		num114 = 20f / num114;
																		this.velocity.X = num112 * num114;
																		this.velocity.Y = num113 * num114;
																		this.netUpdate = true;
																		return;
																	}
																}
																else
																{
																	if (this.ai[2] == 5f && ((this.velocity.X > 0f && this.position.X + (float)(this.width / 2) > Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2)) || (this.velocity.X < 0f && this.position.X + (float)(this.width / 2) < Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2))))
																	{
																		this.ai[2] = 0f;
																		return;
																	}
																}
															}
														}
													}
													else
													{
														if (this.aiStyle == 13)
														{
															if (Main.tile[(int)this.ai[0], (int)this.ai[1]] == null)
															{
																Main.tile[(int)this.ai[0], (int)this.ai[1]] = new Tile();
															}
															if (!Main.tile[(int)this.ai[0], (int)this.ai[1]].active)
															{
																this.life = -1;
																this.HitEffect(0, 10.0);
																this.active = false;
																return;
															}
															this.TargetClosest();
															float num115 = 0.05f;
															Vector2 vector16 = new Vector2(this.ai[0] * 16f + 8f, this.ai[1] * 16f + 8f);
															float num116 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - (float)(this.width / 2) - vector16.X;
															float num117 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - (float)(this.height / 2) - vector16.Y;
															float num118 = (float)Math.Sqrt((double)(num116 * num116 + num117 * num117));
															if (num118 > 150f)
															{
																num118 = 150f / num118;
																num116 *= num118;
																num117 *= num118;
															}
															if (this.position.X < this.ai[0] * 16f + 8f + num116)
															{
																this.velocity.X = this.velocity.X + num115;
																if (this.velocity.X < 0f && num116 > 0f)
																{
																	this.velocity.X = this.velocity.X + num115 * 2f;
																}
															}
															else
															{
																if (this.position.X > this.ai[0] * 16f + 8f + num116)
																{
																	this.velocity.X = this.velocity.X - num115;
																	if (this.velocity.X > 0f && num116 < 0f)
																	{
																		this.velocity.X = this.velocity.X - num115 * 2f;
																	}
																}
															}
															if (this.position.Y < this.ai[1] * 16f + 8f + num117)
															{
																this.velocity.Y = this.velocity.Y + num115;
																if (this.velocity.Y < 0f && num117 > 0f)
																{
																	this.velocity.Y = this.velocity.Y + num115 * 2f;
																}
															}
															else
															{
																if (this.position.Y > this.ai[1] * 16f + 8f + num117)
																{
																	this.velocity.Y = this.velocity.Y - num115;
																	if (this.velocity.Y > 0f && num117 < 0f)
																	{
																		this.velocity.Y = this.velocity.Y - num115 * 2f;
																	}
																}
															}
															if (this.velocity.X > 2f)
															{
																this.velocity.X = 2f;
															}
															if (this.velocity.X < -2f)
															{
																this.velocity.X = -2f;
															}
															if (this.velocity.Y > 2f)
															{
																this.velocity.Y = 2f;
															}
															if (this.velocity.Y < -2f)
															{
																this.velocity.Y = -2f;
															}
															if (num116 > 0f)
															{
																this.spriteDirection = 1;
																this.rotation = (float)Math.Atan2((double)num117, (double)num116);
															}
															if (num116 < 0f)
															{
																this.spriteDirection = -1;
																this.rotation = (float)Math.Atan2((double)num117, (double)num116) + 3.14f;
															}
															if (this.collideX)
															{
																this.netUpdate = true;
																this.velocity.X = this.oldVelocity.X * -0.7f;
																if (this.velocity.X > 0f && this.velocity.X < 2f)
																{
																	this.velocity.X = 2f;
																}
																if (this.velocity.X < 0f && this.velocity.X > -2f)
																{
																	this.velocity.X = -2f;
																}
															}
															if (this.collideY)
															{
																this.netUpdate = true;
																this.velocity.Y = this.oldVelocity.Y * -0.7f;
																if (this.velocity.Y > 0f && this.velocity.Y < 2f)
																{
																	this.velocity.Y = 2f;
																}
																if (this.velocity.Y < 0f && this.velocity.Y > -2f)
																{
																	this.velocity.Y = -2f;
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
            //if (!Main.dedServ)
            //{
            //    num = Main.npcTexture[this.type].Height / Main.npcFrameCount[this.type];
            //}
            if (this.frame == null)
            {
                this.frame = new Rectangle();
            }
			int num2 = 0;
			if (this.aiAction == 0)
			{
				if (this.velocity.Y < 0f)
				{
					num2 = 2;
				}
				else
				{
					if (this.velocity.Y > 0f)
					{
						num2 = 3;
					}
					else
					{
						if (this.velocity.X != 0f)
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
			if (this.type == 1 || this.type == 16)
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
				if (this.frame.Y >= num * Main.npcFrameCount[this.type])
				{
					this.frame.Y = 0;
				}
			}
			if (this.type == 2 || this.type == 23)
			{
				if (this.velocity.X > 0f)
				{
					this.spriteDirection = 1;
					this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X);
				}
				if (this.velocity.X < 0f)
				{
					this.spriteDirection = -1;
					this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) + 3.14f;
				}
				this.frameCounter += 1.0;
				if (this.frameCounter >= 8.0)
				{
					this.frame.Y = this.frame.Y + num;
					this.frameCounter = 0.0;
				}
				if (this.frame.Y >= num * Main.npcFrameCount[this.type])
				{
					this.frame.Y = 0;
				}
			}
			if (this.type == 42)
			{
				if (this.velocity.X > 0f)
				{
					this.spriteDirection = 1;
					this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X);
				}
				if (this.velocity.X < 0f)
				{
					this.spriteDirection = -1;
					this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) + 3.14f;
				}
				this.frameCounter += 1.0;
				if (this.frameCounter < 4.0)
				{
					this.frame.Y = 0;
				}
				else
				{
					if (this.frameCounter < 8.0)
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
							if (this.frameCounter < 16.0)
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
			if (this.type == 43)
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
			if (this.type == 17 || this.type == 18 || this.type == 19 || this.type == 20 || this.type == 22 || this.type == 38 || this.type == 26 || this.type == 27 || this.type == 28 || this.type == 31 || this.type == 21 || this.type == 44)
			{
				if (this.velocity.Y == 0f)
				{
					if (this.direction == 1)
					{
						this.spriteDirection = 1;
					}
					if (this.direction == -1)
					{
						this.spriteDirection = -1;
					}
					if (this.velocity.X == 0f)
					{
						this.frame.Y = 0;
						this.frameCounter = 0.0;
					}
					else
					{
						this.frameCounter += (double)(Math.Abs(this.velocity.X) * 2f);
						this.frameCounter += 1.0;
						if (this.frameCounter > 6.0)
						{
							this.frame.Y = this.frame.Y + num;
							this.frameCounter = 0.0;
						}
						if (this.frame.Y / num >= Main.npcFrameCount[this.type])
						{
							this.frame.Y = num * 2;
						}
					}
				}
				else
				{
					this.frameCounter = 0.0;
					this.frame.Y = num;
					if (this.type == 44 || this.type == 31 || this.type == 21)
					{
						this.frame.Y = num * 9;
					}
				}
			}
			else
			{
				if (this.type == 3 || this.townNPC || this.type == 21 || this.type == 26 || this.type == 27 || this.type == 28 || this.type == 31)
				{
					if (this.velocity.Y == 0f)
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
                    if (this.frame == null)
                    {
                        this.frame = new Rectangle();
                    }
					if (this.velocity.Y != 0f || (this.direction == -1 && this.velocity.X > 0f) || (this.direction == 1 && this.velocity.X < 0f))
					{
						this.frameCounter = 0.0;
						this.frame.Y = num * 2;
					}
					else
					{
						if (this.velocity.X == 0f)
						{
							this.frameCounter = 0.0;
							this.frame.Y = 0;
						}
						else
						{
							this.frameCounter += (double)Math.Abs(this.velocity.X);
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
					if (this.type == 4)
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
						if (this.type == 5)
						{
							this.frameCounter += 1.0;
							if (this.frameCounter >= 8.0)
							{
								this.frame.Y = this.frame.Y + num;
								this.frameCounter = 0.0;
							}
							if (this.frame.Y >= num * Main.npcFrameCount[this.type])
							{
								this.frame.Y = 0;
							}
						}
						else
						{
							if (this.type == 6)
							{
								this.frameCounter += 1.0;
								if (this.frameCounter >= 8.0)
								{
									this.frame.Y = this.frame.Y + num;
									this.frameCounter = 0.0;
								}
								if (this.frame.Y >= num * Main.npcFrameCount[this.type])
								{
									this.frame.Y = 0;
								}
							}
							else
							{
								if (this.type == 24)
								{
									if (this.velocity.Y == 0f)
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
									if (this.type == 29 || this.type == 32 || this.type == 45)
									{
										if (this.velocity.Y == 0f)
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
										if (this.velocity.Y != 0f)
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
			if (this.type == 34)
			{
				if (this.velocity.X > 0f)
				{
					this.spriteDirection = -1;
					this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X);
				}
				if (this.velocity.X < 0f)
				{
					this.spriteDirection = 1;
					this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) + 3.14f;
				}
				this.frameCounter += 1.0;
				if (this.frameCounter >= 4.0)
				{
					this.frame.Y = this.frame.Y + num;
					this.frameCounter = 0.0;
				}
				if (this.frame.Y >= num * Main.npcFrameCount[this.type])
				{
					this.frame.Y = 0;
				}
			}
		}
		public void TargetClosest()
		{
			float num = -1f;
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active && !Main.player[i].dead && (num == -1f || Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - this.position.X + (float)(this.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - this.position.Y + (float)(this.height / 2)) < num))
				{
					num = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - this.position.X + (float)(this.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - this.position.Y + (float)(this.height / 2));
					this.target = i;
				}
			}
			if (this.target < 0 || this.target >= 255)
			{
				this.target = 0;
			}
			this.targetRect = new Rectangle((int)Main.player[this.target].position.X, (int)Main.player[this.target].position.Y, Main.player[this.target].width, Main.player[this.target].height);
			this.direction = 1;
			if ((float)(this.targetRect.X + this.targetRect.Width / 2) < this.position.X + (float)(this.width / 2))
			{
				this.direction = -1;
			}
			this.directionY = 1;
			if ((float)(this.targetRect.Y + this.targetRect.Height / 2) < this.position.Y + (float)(this.height / 2))
			{
				this.directionY = -1;
			}
			if (this.direction != this.oldDirection || this.directionY != this.oldDirectionY || this.target != this.oldTarget)
			{
				this.netUpdate = true;
			}
		}
		public void CheckActive()
		{
			if (this.active)
			{
				if (this.type == 8 || this.type == 9 || this.type == 11 || this.type == 12 || this.type == 14 || this.type == 15 || this.type == 40 || this.type == 41)
				{
					return;
				}
				if (this.townNPC)
				{
					if ((double)this.position.Y < Main.worldSurface * 18.0)
					{
						Rectangle rectangle = new Rectangle((int)(this.position.X + (float)(this.width / 2) - (float)NPC.townRangeX), (int)(this.position.Y + (float)(this.height / 2) - (float)NPC.townRangeY), NPC.townRangeX * 2, NPC.townRangeY * 2);
						for (int i = 0; i < 255; i++)
						{
							if (Main.player[i].active && rectangle.Intersects(new Rectangle((int)Main.player[i].position.X, (int)Main.player[i].position.Y, Main.player[i].width, Main.player[i].height)))
							{
								Main.player[i].townNPCs++;
							}
						}
					}
					return;
				}
				bool flag = false;
				Rectangle rectangle2 = new Rectangle((int)(this.position.X + (float)(this.width / 2) - (float)NPC.activeRangeX), (int)(this.position.Y + (float)(this.height / 2) - (float)NPC.activeRangeY), NPC.activeRangeX * 2, NPC.activeRangeY * 2);
				Rectangle rectangle3 = new Rectangle((int)((double)(this.position.X + (float)(this.width / 2)) - (double)Main.screenWidth * 0.5 - (double)this.width), (int)((double)(this.position.Y + (float)(this.height / 2)) - (double)Main.screenHeight * 0.5 - (double)this.height), Main.screenWidth + this.width * 2, Main.screenHeight + this.height * 2);
				for (int j = 0; j < 255; j++)
				{
					if (Main.player[j].active)
					{
						if (rectangle2.Intersects(new Rectangle((int)Main.player[j].position.X, (int)Main.player[j].position.Y, Main.player[j].width, Main.player[j].height)))
						{
							flag = true;
							if (this.type != 25 && this.type != 30 && this.type != 33)
							{
								Main.player[j].activeNPCs++;
							}
						}
						if (rectangle3.Intersects(new Rectangle((int)Main.player[j].position.X, (int)Main.player[j].position.Y, Main.player[j].width, Main.player[j].height)))
						{
							this.timeLeft = NPC.activeTime;
						}
						if (this.type == 7 || this.type == 10 || this.type == 13)
						{
							flag = true;
						}
						if (this.boss || this.type == 35 || this.type == 36)
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
				if (!flag && Main.netMode != 1)
				{
					this.active = false;
					if (Main.netMode == 2)
					{
						this.life = 0;
						NetMessage.SendData(23, -1, -1, "", this.whoAmI, 0f, 0f, 0f);
					}
				}
			}
		}
		public static void SpawnNPC()
		{
			if (Main.stopSpawns)
			{
				return;
			}
			bool flag = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active)
				{
					num3++;
				}
			}
			int j = 0;
			while (j < 255)
			{
				bool flag2 = false;
				if (Main.player[j].active && Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0 && (double)Main.player[j].position.Y < Main.worldSurface * 16.0 + (double)Main.screenHeight)
				{
					int num4 = 3000;
					if ((double)Main.player[j].position.X > Main.invasionX * 16.0 - (double)num4 && (double)Main.player[j].position.X < Main.invasionX * 16.0 + (double)num4)
					{
						flag2 = true;
					}
				}
				flag = false;
				NPC.spawnRate = NPC.defaultSpawnRate;
				NPC.maxSpawns = NPC.defaultMaxSpawns;
				if (Main.player[j].position.Y > (float)((Main.maxTilesY - 200) * 16))
				{
					NPC.spawnRate = (int)((float)NPC.spawnRate * 1.5f);
					NPC.maxSpawns = (int)((float)NPC.maxSpawns * 0.5f);
				}
				else
				{
					if ((double)Main.player[j].position.Y > Main.rockLayer * 16.0 + (double)Main.screenHeight)
					{
						NPC.spawnRate = (int)((double)NPC.spawnRate * 0.7);
						NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.35f);
					}
					else
					{
						if ((double)Main.player[j].position.Y > Main.worldSurface * 16.0 + (double)Main.screenHeight)
						{
							NPC.spawnRate = (int)((double)NPC.spawnRate * 0.8);
							NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.1f);
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
				if (Main.player[j].zoneDungeon)
				{
					NPC.spawnRate = (int)((double)NPC.defaultSpawnRate * 0.1);
					NPC.maxSpawns = (int)((double)NPC.defaultMaxSpawns * 2.1);
				}
				else
				{
					if (Main.player[j].zoneEvil)
					{
						NPC.spawnRate = (int)((double)NPC.spawnRate * 0.5);
						NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.4f);
					}
					else
					{
						if (Main.player[j].zoneMeteor)
						{
							NPC.spawnRate = (int)((double)NPC.spawnRate * 0.5);
						}
						else
						{
							if (Main.player[j].zoneJungle)
							{
								NPC.spawnRate = (int)((double)NPC.spawnRate * 0.3);
								NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.6f);
							}
						}
					}
				}
				if ((double)NPC.spawnRate < (double)NPC.defaultSpawnRate * 0.1)
				{
					NPC.spawnRate = (int)((double)NPC.defaultSpawnRate * 0.1);
				}
				if ((double)NPC.maxSpawns > (double)NPC.defaultMaxSpawns * 2.5)
				{
					NPC.maxSpawns = (int)((double)NPC.defaultMaxSpawns * 2.5);
				}
				if (Main.player[j].inventory[Main.player[j].selectedItem].type == 49)
				{
					NPC.spawnRate = (int)((double)NPC.spawnRate * 0.75);
					NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.5f);
				}
				if (flag2)
				{
					NPC.maxSpawns = (int)((double)NPC.defaultMaxSpawns * (1.0 + 0.4 * (double)num3));
					NPC.spawnRate = 30;
				}
				if (!flag2 && (!Main.bloodMoon || Main.dayTime) && !Main.player[j].zoneDungeon && !Main.player[j].zoneEvil && !Main.player[j].zoneMeteor)
				{
					if (Main.player[j].townNPCs == 1)
					{
						NPC.maxSpawns = (int)((double)((float)NPC.maxSpawns) * 0.6);
						NPC.spawnRate = (int)((float)NPC.spawnRate * 2f);
					}
					else
					{
						if (Main.player[j].townNPCs == 2)
						{
							NPC.maxSpawns = (int)((double)((float)NPC.maxSpawns) * 0.3);
							NPC.spawnRate = (int)((float)NPC.spawnRate * 3f);
						}
						else
						{
							if (Main.player[j].townNPCs >= 3)
							{
								NPC.maxSpawns = 0;
								NPC.spawnRate = 99999;
							}
						}
					}
				}
				if (Main.player[j].active && !Main.player[j].dead && Main.player[j].activeNPCs < NPC.maxSpawns && Main.rand.Next(NPC.spawnRate) == 0)
				{
					int num5 = (int)(Main.player[j].position.X / 16f) - NPC.spawnRangeX;
					int num6 = (int)(Main.player[j].position.X / 16f) + NPC.spawnRangeX;
					int num7 = (int)(Main.player[j].position.Y / 16f) - NPC.spawnRangeY;
					int num8 = (int)(Main.player[j].position.Y / 16f) + NPC.spawnRangeY;
					int num9 = (int)(Main.player[j].position.X / 16f) - NPC.safeRangeX;
					int num10 = (int)(Main.player[j].position.X / 16f) + NPC.safeRangeX;
					int num11 = (int)(Main.player[j].position.Y / 16f) - NPC.safeRangeY;
					int num12 = (int)(Main.player[j].position.Y / 16f) + NPC.safeRangeY;
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
						if (Main.tile[num13, num14].active && Main.tileSolid[(int)Main.tile[num13, num14].type])
						{
							goto IL_844;
						}
						if (!Main.wallHouse[(int)Main.tile[num13, num14].wall])
						{
							int l = num14;
							while (l < Main.maxTilesY)
							{
								if (Main.tile[num13, l].active && Main.tileSolid[(int)Main.tile[num13, l].type])
								{
									if (num13 < num9 || num13 > num10 || l < num11 || l > num12)
									{
										byte arg_758_0 = Main.tile[num13, l].type;
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
							if (!flag)
							{
								goto IL_844;
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
										if (Main.tile[m, n].active && Main.tileSolid[(int)Main.tile[m, n].type])
										{
											flag = false;
											break;
										}
										if (Main.tile[m, n].lava && n < Main.maxTilesY - 200)
										{
											flag = false;
											break;
										}
									}
								}
								goto IL_844;
							}
							goto IL_844;
						}
						IL_84A:
						k++;
						continue;
						IL_844:
						if (!flag && !flag)
						{
							goto IL_84A;
						}
						break;
					}
				}
				if (flag)
				{
					Rectangle rectangle = new Rectangle(num * 16, num2 * 16, 16, 16);
					for (int num19 = 0; num19 < 255; num19++)
					{
						if (Main.player[num19].active)
						{
							Rectangle rectangle2 = new Rectangle((int)(Main.player[num19].position.X + (float)(Main.player[num19].width / 2) - (float)(Main.screenWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[num19].position.Y + (float)(Main.player[num19].height / 2) - (float)(Main.screenHeight / 2) - (float)NPC.safeRangeY), Main.screenWidth + NPC.safeRangeX * 2, Main.screenHeight + NPC.safeRangeY * 2);
							if (rectangle.Intersects(rectangle2))
							{
								flag = false;
							}
						}
					}
				}
				if (flag && Main.player[j].zoneDungeon && (!Main.tileDungeon[(int)Main.tile[num, num2].type] || Main.tile[num, num2 - 1].wall == 0))
				{
					flag = false;
				}
				if (flag)
				{
					flag = false;
					int num20 = (int)Main.tile[num, num2].type;
					int num21 = 1000;
					if (flag2)
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
						if (Main.player[j].zoneDungeon)
						{
							if (!NPC.downedBoss3)
							{
								num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 35, 0);
								Main.npc[num21].ai[0] = 1f;
								Main.npc[num21].ai[2] = 2f;
							}
							else
							{
								if (Main.rand.Next(4) == 0)
								{
									num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 34, 0);
								}
								else
								{
									if (Main.rand.Next(5) == 0)
									{
										num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 32, 0);
									}
									else
									{
										num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 31, 0);
									}
								}
							}
						}
						else
						{
							if (Main.player[j].zoneMeteor)
							{
								num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 23, 0);
							}
							else
							{
								if (Main.player[j].zoneEvil && Main.rand.Next(50) == 0)
								{
									num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 7, 1);
								}
								else
								{
									if (num20 == 60)
									{
										if (Main.rand.Next(3) == 0)
										{
											num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 43, 0);
											Main.npc[num21].ai[0] = (float)num;
											Main.npc[num21].ai[1] = (float)num2;
											Main.npc[num21].netUpdate = true;
										}
										else
										{
											num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 42, 0);
										}
									}
									else
									{
										if ((double)num2 <= Main.worldSurface)
										{
											if (num20 == 23 || num20 == 25)
											{
												num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 6, 0);
											}
											else
											{
												if (Main.dayTime)
												{
													int num22 = Math.Abs(num - Main.spawnTileX);
													num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 1, 0);
													if (Main.rand.Next(3) == 0 || num22 < 200)
													{
														Main.npc[num21].SetDefaults("Green Slime");
													}
													else
													{
														if (Main.rand.Next(10) == 0 && num22 > 400)
														{
															Main.npc[num21].SetDefaults("Purple Slime");
														}
													}
												}
												else
												{
													if (Main.rand.Next(6) == 0 || (Main.moonPhase == 4 && Main.rand.Next(2) == 0))
													{
														num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 2, 0);
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
													num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 10, 1);
												}
												else
												{
													num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 1, 0);
													if (Main.rand.Next(5) == 0)
													{
														Main.npc[num21].SetDefaults("Yellow Slime");
													}
													else
													{
														if (Main.rand.Next(2) == 0)
														{
															Main.npc[num21].SetDefaults("Blue Slime");
														}
														else
														{
															Main.npc[num21].SetDefaults("Red Slime");
														}
													}
												}
											}
											else
											{
												if (num2 > Main.maxTilesY - 190)
												{
													if (Main.rand.Next(5) == 0)
													{
														num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 39, 1);
													}
													else
													{
														num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 24, 0);
													}
												}
												else
												{
													if (Main.rand.Next(35) == 0)
													{
														num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 10, 1);
													}
													else
													{
														if (Main.rand.Next(5) == 0)
														{
															num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 16, 0);
														}
														else
														{
															if (Main.rand.Next(2) == 0)
															{
																if ((double)num2 > (Main.rockLayer + (double)Main.maxTilesY) / 2.0 && Main.rand.Next(100) == 0)
																{
																	num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 45, 0);
																}
																else
																{
																	if (Main.rand.Next(8) == 0)
																	{
																		num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 44, 0);
																	}
																	else
																	{
																		num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 21, 0);
																	}
																}
															}
															else
															{
																num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 1, 0);
																Main.npc[num21].SetDefaults("Black Slime");
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
					if (Main.npc[num21].type == 1 && Main.rand.Next(250) == 0)
					{
						Main.npc[num21].SetDefaults("Pinky");
					}
					if (Main.netMode == 2 && num21 < 1000)
					{
						NetMessage.SendData(23, -1, -1, "", num21, 0f, 0f, 0f);
						return;
					}
					break;
				}
				else
				{
					j++;
				}
			}
		}
		public static void SpawnOnPlayer(int plr, int Type)
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			int num3 = (int)(Main.player[plr].position.X / 16f) - NPC.spawnRangeX * 3;
			int num4 = (int)(Main.player[plr].position.X / 16f) + NPC.spawnRangeX * 3;
			int num5 = (int)(Main.player[plr].position.Y / 16f) - NPC.spawnRangeY * 3;
			int num6 = (int)(Main.player[plr].position.Y / 16f) + NPC.spawnRangeY * 3;
			int num7 = (int)(Main.player[plr].position.X / 16f) - NPC.safeRangeX;
			int num8 = (int)(Main.player[plr].position.X / 16f) + NPC.safeRangeX;
			int num9 = (int)(Main.player[plr].position.Y / 16f) - NPC.safeRangeY;
			int num10 = (int)(Main.player[plr].position.Y / 16f) + NPC.safeRangeY;
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
					if (Main.tile[num11, num12].active && Main.tileSolid[(int)Main.tile[num11, num12].type])
					{
						goto IL_2D8;
					}
					if (Main.tile[num11, num12].wall != 1)
					{
						int k = num12;
						while (k < Main.maxTilesY)
						{
							if (Main.tile[num11, k].active && Main.tileSolid[(int)Main.tile[num11, k].type])
							{
								if (num11 < num7 || num11 > num8 || k < num9 || k > num10)
								{
									byte arg_217_0 = Main.tile[num11, k].type;
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
							goto IL_2D8;
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
									if (Main.tile[l, m].active && Main.tileSolid[(int)Main.tile[l, m].type])
									{
										flag = false;
										break;
									}
								}
							}
							goto IL_2D8;
						}
						goto IL_2D8;
					}
					IL_2DE:
					j++;
					continue;
					IL_2D8:
					if (!flag && !flag)
					{
						goto IL_2DE;
					}
					break;
				}
				if (flag)
				{
					Rectangle rectangle = new Rectangle(num * 16, num2 * 16, 16, 16);
					for (int n = 0; n < 255; n++)
					{
						if (Main.player[n].active)
						{
							Rectangle rectangle2 = new Rectangle((int)(Main.player[n].position.X + (float)(Main.player[n].width / 2) - (float)(Main.screenWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[n].position.Y + (float)(Main.player[n].height / 2) - (float)(Main.screenHeight / 2) - (float)NPC.safeRangeY), Main.screenWidth + NPC.safeRangeX * 2, Main.screenHeight + NPC.safeRangeY * 2);
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
				int num17 = NPC.NewNPC(num * 16 + 8, num2 * 16, Type, 1);
				Main.npc[num17].target = plr;
				string str = Main.npc[num17].name;
				if (Main.npc[num17].type == 13)
				{
					str = "Eater of Worlds";
				}
				if (Main.npc[num17].type == 35)
				{
					str = "Skeletron";
				}
				if (Main.netMode == 2 && num17 < 1000)
				{
					NetMessage.SendData(23, -1, -1, "", num17, 0f, 0f, 0f);
				}
				if (Main.netMode == 0)
				{
					//Main.NewText(str + " has awoken!", 175, 75, 255);
					return;
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(25, -1, -1, str + " has awoken!", 255, 175f, 75f, 255f);
				}
			}
		}
		public static int NewNPC(int X, int Y, int Type, int Start = 0)
		{
			int num = -1;
			for (int i = Start; i < 1000; i++)
			{
				if (!Main.npc[i].active)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				Main.npc[num] = new NPC();
				Main.npc[num].SetDefaults(Type);
				Main.npc[num].position.X = (float)(X - Main.npc[num].width / 2);
				Main.npc[num].position.Y = (float)(Y - Main.npc[num].height);
				Main.npc[num].active = true;
				Main.npc[num].timeLeft = (int)((double)NPC.activeTime * 1.25);
				Main.npc[num].wet = Collision.WetCollision(Main.npc[num].position, Main.npc[num].width, Main.npc[num].height);
				return num;
			}
			return 1000;
		}
		public double StrikeNPC(int Damage, float knockBack, int hitDirection)
		{
			if (!this.active || this.life <= 0)
			{
				return 0.0;
			}
			double num = Main.CalculateDamage(Damage, this.defense);
			if (this.friendly)
			{
				////CombatText.NewText(new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height), new Color(255, 80, 90, 255), string.Concat((int)num));
			}
			else
			{
				////CombatText.NewText(new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height), new Color(255, 160, 80, 255), string.Concat((int)num));
			}
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
				if (this.aiStyle == 8 && Main.netMode != 1)
				{
					this.ai[0] = 400f;
					this.TargetClosest();
				}
				this.life -= (int)num;
				if (knockBack > 0f && this.knockBackResist > 0f)
				{
					if (!this.noGravity)
					{
						this.velocity.Y = -knockBack * 0.75f * this.knockBackResist;
					}
					else
					{
						this.velocity.Y = -knockBack * 0.5f * this.knockBackResist;
					}
					this.velocity.X = knockBack * (float)hitDirection * this.knockBackResist;
				}
				this.HitEffect(hitDirection, num);
				if (this.soundHit > 0)
				{
					//Main.PlaySound(3, (int)this.position.X, (int)this.position.Y, this.soundHit);
				}
				if (this.life <= 0)
				{
					if (this.townNPC && this.type != 37)
					{
						if (Main.netMode == 0)
						{
							//Main.NewText(this.name + " was slain...", 255, 25, 25);
						}
						else
						{
							if (Main.netMode == 2)
							{
								NetMessage.SendData(25, -1, -1, this.name + " was slain...", 255, 255f, 25f, 25f);
							}
						}
					}
					if (this.townNPC && Main.netMode != 1 && this.homeless && WorldGen.spawnNPC == this.type)
					{
						WorldGen.spawnNPC = 0;
					}
					if (this.soundKilled > 0)
					{
						//Main.PlaySound(4, (int)this.position.X, (int)this.position.Y, this.soundKilled);
					}
					this.NPCLoot();
					this.active = false;
					if (this.type == 26 || this.type == 27 || this.type == 28 || this.type == 29)
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
			if (this.type == 1 || this.type == 16)
			{
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 23, Main.rand.Next(1, 3), false);
			}
			if (this.type == 2)
			{
				if (Main.rand.Next(3) == 0)
				{
					Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 38, 1, false);
				}
				else
				{
					if (Main.rand.Next(100) == 0)
					{
						Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 236, 1, false);
					}
				}
			}
			if (this.type == 3 && Main.rand.Next(50) == 0)
			{
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 216, 1, false);
			}
			if (this.type == 4)
			{
				int stack = Main.rand.Next(30) + 20;
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 47, stack, false);
				stack = Main.rand.Next(20) + 10;
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, stack, false);
				stack = Main.rand.Next(20) + 10;
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, stack, false);
				stack = Main.rand.Next(20) + 10;
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, stack, false);
				stack = Main.rand.Next(3) + 1;
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 59, stack, false);
			}
			if (this.type == 6 && Main.rand.Next(3) == 0)
			{
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 68, 1, false);
			}
			if (this.type == 7 || this.type == 8 || this.type == 9)
			{
				if (Main.rand.Next(3) == 0)
				{
					Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 68, Main.rand.Next(1, 3), false);
				}
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 69, Main.rand.Next(3, 9), false);
			}
			if ((this.type == 10 || this.type == 11 || this.type == 12) && Main.rand.Next(500) == 0)
			{
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 215, 1, false);
			}
			if (this.type == 39 || this.type == 40 || this.type == 41)
			{
				if (Main.rand.Next(100) == 0)
				{
					Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 220, 1, false);
				}
				else
				{
					if (Main.rand.Next(100) == 0)
					{
						Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 218, 1, false);
					}
				}
			}
			if (this.type == 13 || this.type == 14 || this.type == 15)
			{
				int stack2 = Main.rand.Next(1, 4);
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 86, stack2, false);
				if (Main.rand.Next(2) == 0)
				{
					stack2 = Main.rand.Next(2, 6);
					Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, stack2, false);
				}
				if (this.boss)
				{
					stack2 = Main.rand.Next(15, 30);
					Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, stack2, false);
					stack2 = Main.rand.Next(15, 31);
					Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, stack2, false);
					int num = Main.rand.Next(100, 103);
					Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, num, 1, false);
				}
			}
			if (this.type == 21 || this.type == 44)
			{
				if (Main.rand.Next(25) == 0)
				{
					Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 118, 1, false);
				}
				else
				{
					if (this.type == 44)
					{
						Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 166, Main.rand.Next(1, 4), false);
					}
				}
			}
			if (this.type == 45)
			{
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 238, 1, false);
			}
			if (this.type == 23)
			{
				Main.rand.Next(3);
			}
			if (this.type == 24 && Main.rand.Next(50) == 0)
			{
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 112, 1, false);
			}
			if (this.type == 31 || this.type == 32)
			{
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 154, 1, false);
			}
			if (this.type == 26 || this.type == 27 || this.type == 28 || this.type == 29)
			{
				if (Main.rand.Next(400) == 0)
				{
					Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 128, 1, false);
				}
				else
				{
					if (Main.rand.Next(200) == 0)
					{
						Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 160, 1, false);
					}
					else
					{
						if (Main.rand.Next(2) == 0)
						{
							int stack3 = Main.rand.Next(1, 6);
							Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 161, stack3, false);
						}
					}
				}
			}
			if (this.type == 42)
			{
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 209, 1, false);
			}
			if (this.type == 43 && Main.rand.Next(5) == 0)
			{
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 210, 1, false);
			}
			if ((this.type == 42 || this.type == 43) && Main.rand.Next(150) == 0)
			{
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, Main.rand.Next(228, 231), 1, false);
			}
			if (this.boss)
			{
				if (this.type == 4)
				{
					NPC.downedBoss1 = true;
				}
				if (this.type == 13 || this.type == 14 || this.type == 15)
				{
					NPC.downedBoss2 = true;
					this.name = "Eater of Worlds";
				}
				if (this.type == 35)
				{
					NPC.downedBoss3 = true;
					this.name = "Skeletron";
				}
				int stack4 = Main.rand.Next(5, 16);
				Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 28, stack4, false);
				int num2 = Main.rand.Next(5) + 5;
				for (int i = 0; i < num2; i++)
				{
					Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 58, 1, false);
				}
				if (Main.netMode == 0)
				{
					//Main.NewText(this.name + " has been defeated!", 175, 75, 255);
				}
				else
				{
					if (Main.netMode == 2)
					{
						NetMessage.SendData(25, -1, -1, this.name + " has been defeated!", 255, 175f, 75f, 255f);
					}
				}
			}
			if (Main.rand.Next(7) == 0)
			{
				if (Main.rand.Next(2) == 0 && Main.player[(int)Player.FindClosest(this.position, this.width, this.height)].statMana < Main.player[(int)Player.FindClosest(this.position, this.width, this.height)].statManaMax)
				{
					Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 184, 1, false);
				}
				else
				{
					if (Main.rand.Next(2) == 0 && Main.player[(int)Player.FindClosest(this.position, this.width, this.height)].statLife < Main.player[(int)Player.FindClosest(this.position, this.width, this.height)].statLifeMax)
					{
						Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 58, 1, false);
					}
				}
			}
			float num3 = this.value;
			num3 *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
			if (Main.rand.Next(5) == 0)
			{
				num3 *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;
			}
			if (Main.rand.Next(10) == 0)
			{
				num3 *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;
			}
			if (Main.rand.Next(15) == 0)
			{
				num3 *= 1f + (float)Main.rand.Next(15, 31) * 0.01f;
			}
			if (Main.rand.Next(20) == 0)
			{
				num3 *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;
			}
			while ((int)num3 > 0)
			{
				if (num3 > 1000000f)
				{
					int num4 = (int)(num3 / 1000000f);
					if (num4 > 50 && Main.rand.Next(2) == 0)
					{
						num4 /= Main.rand.Next(3) + 1;
					}
					if (Main.rand.Next(2) == 0)
					{
						num4 /= Main.rand.Next(3) + 1;
					}
					num3 -= (float)(1000000 * num4);
					Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 74, num4, false);
				}
				else
				{
					if (num3 > 10000f)
					{
						int num5 = (int)(num3 / 10000f);
						if (num5 > 50 && Main.rand.Next(2) == 0)
						{
							num5 /= Main.rand.Next(3) + 1;
						}
						if (Main.rand.Next(2) == 0)
						{
							num5 /= Main.rand.Next(3) + 1;
						}
						num3 -= (float)(10000 * num5);
						Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 73, num5, false);
					}
					else
					{
						if (num3 > 100f)
						{
							int num6 = (int)(num3 / 100f);
							if (num6 > 50 && Main.rand.Next(2) == 0)
							{
								num6 /= Main.rand.Next(3) + 1;
							}
							if (Main.rand.Next(2) == 0)
							{
								num6 /= Main.rand.Next(3) + 1;
							}
							num3 -= (float)(100 * num6);
							Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 72, num6, false);
						}
						else
						{
							int num7 = (int)num3;
							if (num7 > 50 && Main.rand.Next(2) == 0)
							{
								num7 /= Main.rand.Next(3) + 1;
							}
							if (Main.rand.Next(2) == 0)
							{
								num7 /= Main.rand.Next(4) + 1;
							}
							if (num7 < 1)
							{
								num7 = 1;
							}
							num3 -= (float)num7;
							Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 71, num7, false);
						}
					}
				}
			}
		}
		public void HitEffect(int hitDirection = 0, double dmg = 10.0)
		{
			if (this.type == 1 || this.type == 16)
			{
				if (this.life > 0)
				{
					int num = 0;
					while ((double)num < dmg / (double)this.lifeMax * 100.0)
					{
						Dust.NewDust(this.position, this.width, this.height, 4, (float)hitDirection, -1f, this.alpha, this.color, 1f);
						num++;
					}
					return;
				}
				for (int i = 0; i < 50; i++)
				{
					Dust.NewDust(this.position, this.width, this.height, 4, (float)(2 * hitDirection), -2f, this.alpha, this.color, 1f);
				}
				if (Main.netMode != 1 && this.type == 16)
				{
					int num2 = Main.rand.Next(2) + 2;
					for (int j = 0; j < num2; j++)
					{
						int num3 = NPC.NewNPC((int)(this.position.X + (float)(this.width / 2)), (int)(this.position.Y + (float)this.height), 1, 0);
						Main.npc[num3].SetDefaults("Baby Slime");
						Main.npc[num3].velocity.X = this.velocity.X * 2f;
						Main.npc[num3].velocity.Y = this.velocity.Y;
						NPC expr_16F_cp_0 = Main.npc[num3];
						expr_16F_cp_0.velocity.X = expr_16F_cp_0.velocity.X + ((float)Main.rand.Next(-20, 20) * 0.1f + (float)(j * this.direction) * 0.3f);
						NPC expr_1AD_cp_0 = Main.npc[num3];
						expr_1AD_cp_0.velocity.Y = expr_1AD_cp_0.velocity.Y - ((float)Main.rand.Next(0, 10) * 0.1f + (float)j);
						Main.npc[num3].ai[1] = (float)j;
						if (Main.netMode == 2 && num3 < 1000)
						{
							NetMessage.SendData(23, -1, -1, "", num3, 0f, 0f, 0f);
						}
					}
					return;
				}
			}
			else
			{
				if (this.type == 2)
				{
					if (this.life > 0)
					{
						int num4 = 0;
						while ((double)num4 < dmg / (double)this.lifeMax * 100.0)
						{
							Vector2 arg_261_0 = this.position;
							int arg_261_1 = this.width;
							int arg_261_2 = this.height;
							int arg_261_3 = 5;
							float arg_261_4 = (float)hitDirection;
							float arg_261_5 = -1f;
							int arg_261_6 = 0;
							Color newColor = default(Color);
							Dust.NewDust(arg_261_0, arg_261_1, arg_261_2, arg_261_3, arg_261_4, arg_261_5, arg_261_6, newColor, 1f);
							num4++;
						}
						return;
					}
					for (int k = 0; k < 50; k++)
					{
						Vector2 arg_2B7_0 = this.position;
						int arg_2B7_1 = this.width;
						int arg_2B7_2 = this.height;
						int arg_2B7_3 = 5;
						float arg_2B7_4 = (float)(2 * hitDirection);
						float arg_2B7_5 = -2f;
						int arg_2B7_6 = 0;
						Color newColor = default(Color);
						Dust.NewDust(arg_2B7_0, arg_2B7_1, arg_2B7_2, arg_2B7_3, arg_2B7_4, arg_2B7_5, arg_2B7_6, newColor, 1f);
					}
					Gore.NewGore(this.position, this.velocity, 1);
					Gore.NewGore(new Vector2(this.position.X + 14f, this.position.Y), this.velocity, 2);
					return;
				}
				else
				{
					if (this.type == 3)
					{
						if (this.life > 0)
						{
							int num5 = 0;
							while ((double)num5 < dmg / (double)this.lifeMax * 100.0)
							{
								Vector2 arg_34F_0 = this.position;
								int arg_34F_1 = this.width;
								int arg_34F_2 = this.height;
								int arg_34F_3 = 5;
								float arg_34F_4 = (float)hitDirection;
								float arg_34F_5 = -1f;
								int arg_34F_6 = 0;
								Color newColor = default(Color);
								Dust.NewDust(arg_34F_0, arg_34F_1, arg_34F_2, arg_34F_3, arg_34F_4, arg_34F_5, arg_34F_6, newColor, 1f);
								num5++;
							}
							return;
						}
						for (int l = 0; l < 50; l++)
						{
							Vector2 arg_3A9_0 = this.position;
							int arg_3A9_1 = this.width;
							int arg_3A9_2 = this.height;
							int arg_3A9_3 = 5;
							float arg_3A9_4 = 2.5f * (float)hitDirection;
							float arg_3A9_5 = -2.5f;
							int arg_3A9_6 = 0;
							Color newColor = default(Color);
							Dust.NewDust(arg_3A9_0, arg_3A9_1, arg_3A9_2, arg_3A9_3, arg_3A9_4, arg_3A9_5, arg_3A9_6, newColor, 1f);
						}
						Gore.NewGore(this.position, this.velocity, 3);
						Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 4);
						Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 4);
						Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 5);
						Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 5);
						return;
					}
					else
					{
						if (this.type == 4)
						{
							if (this.life > 0)
							{
								int num6 = 0;
								while ((double)num6 < dmg / (double)this.lifeMax * 100.0)
								{
									Vector2 arg_4CB_0 = this.position;
									int arg_4CB_1 = this.width;
									int arg_4CB_2 = this.height;
									int arg_4CB_3 = 5;
									float arg_4CB_4 = (float)hitDirection;
									float arg_4CB_5 = -1f;
									int arg_4CB_6 = 0;
									Color newColor = default(Color);
									Dust.NewDust(arg_4CB_0, arg_4CB_1, arg_4CB_2, arg_4CB_3, arg_4CB_4, arg_4CB_5, arg_4CB_6, newColor, 1f);
									num6++;
								}
								return;
							}
							for (int m = 0; m < 150; m++)
							{
								Vector2 arg_521_0 = this.position;
								int arg_521_1 = this.width;
								int arg_521_2 = this.height;
								int arg_521_3 = 5;
								float arg_521_4 = (float)(2 * hitDirection);
								float arg_521_5 = -2f;
								int arg_521_6 = 0;
								Color newColor = default(Color);
								Dust.NewDust(arg_521_0, arg_521_1, arg_521_2, arg_521_3, arg_521_4, arg_521_5, arg_521_6, newColor, 1f);
							}
							for (int n = 0; n < 2; n++)
							{
								Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 2);
								Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7);
								Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 9);
								Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 10);
							}
							//Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
							return;
						}
						else
						{
							if (this.type == 5)
							{
								if (this.life > 0)
								{
									int num7 = 0;
									while ((double)num7 < dmg / (double)this.lifeMax * 50.0)
									{
										Vector2 arg_6A3_0 = this.position;
										int arg_6A3_1 = this.width;
										int arg_6A3_2 = this.height;
										int arg_6A3_3 = 5;
										float arg_6A3_4 = (float)hitDirection;
										float arg_6A3_5 = -1f;
										int arg_6A3_6 = 0;
										Color newColor = default(Color);
										Dust.NewDust(arg_6A3_0, arg_6A3_1, arg_6A3_2, arg_6A3_3, arg_6A3_4, arg_6A3_5, arg_6A3_6, newColor, 1f);
										num7++;
									}
									return;
								}
								for (int num8 = 0; num8 < 20; num8++)
								{
									Vector2 arg_6F9_0 = this.position;
									int arg_6F9_1 = this.width;
									int arg_6F9_2 = this.height;
									int arg_6F9_3 = 5;
									float arg_6F9_4 = (float)(2 * hitDirection);
									float arg_6F9_5 = -2f;
									int arg_6F9_6 = 0;
									Color newColor = default(Color);
									Dust.NewDust(arg_6F9_0, arg_6F9_1, arg_6F9_2, arg_6F9_3, arg_6F9_4, arg_6F9_5, arg_6F9_6, newColor, 1f);
								}
								Gore.NewGore(this.position, this.velocity, 6);
								Gore.NewGore(this.position, this.velocity, 7);
								return;
							}
							else
							{
								if (this.type == 6)
								{
									if (this.life > 0)
									{
										int num9 = 0;
										while ((double)num9 < dmg / (double)this.lifeMax * 100.0)
										{
											Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
											num9++;
										}
										return;
									}
									for (int num10 = 0; num10 < 50; num10++)
									{
										Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
									}
									int num11 = Gore.NewGore(this.position, this.velocity, 14);
									Main.gore[num11].alpha = this.alpha;
									num11 = Gore.NewGore(this.position, this.velocity, 15);
									Main.gore[num11].alpha = this.alpha;
									return;
								}
								else
								{
									if (this.type == 7 || this.type == 8 || this.type == 9)
									{
										if (this.life > 0)
										{
											int num12 = 0;
											while ((double)num12 < dmg / (double)this.lifeMax * 100.0)
											{
												Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
												num12++;
											}
											return;
										}
										for (int num13 = 0; num13 < 50; num13++)
										{
											Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
										}
										int num14 = Gore.NewGore(this.position, this.velocity, this.type - 7 + 18);
										Main.gore[num14].alpha = this.alpha;
										return;
									}
									else
									{
										if (this.type == 10 || this.type == 11 || this.type == 12)
										{
											if (this.life > 0)
											{
												int num15 = 0;
												while ((double)num15 < dmg / (double)this.lifeMax * 50.0)
												{
													Vector2 arg_981_0 = this.position;
													int arg_981_1 = this.width;
													int arg_981_2 = this.height;
													int arg_981_3 = 5;
													float arg_981_4 = (float)hitDirection;
													float arg_981_5 = -1f;
													int arg_981_6 = 0;
													Color newColor = default(Color);
													Dust.NewDust(arg_981_0, arg_981_1, arg_981_2, arg_981_3, arg_981_4, arg_981_5, arg_981_6, newColor, 1f);
													num15++;
												}
												return;
											}
											for (int num16 = 0; num16 < 10; num16++)
											{
												Vector2 arg_9DB_0 = this.position;
												int arg_9DB_1 = this.width;
												int arg_9DB_2 = this.height;
												int arg_9DB_3 = 5;
												float arg_9DB_4 = 2.5f * (float)hitDirection;
												float arg_9DB_5 = -2.5f;
												int arg_9DB_6 = 0;
												Color newColor = default(Color);
												Dust.NewDust(arg_9DB_0, arg_9DB_1, arg_9DB_2, arg_9DB_3, arg_9DB_4, arg_9DB_5, arg_9DB_6, newColor, 1f);
											}
											Gore.NewGore(this.position, this.velocity, this.type - 7 + 18);
											return;
										}
										else
										{
											if (this.type == 13 || this.type == 14 || this.type == 15)
											{
												if (this.life > 0)
												{
													int num17 = 0;
													while ((double)num17 < dmg / (double)this.lifeMax * 100.0)
													{
														Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
														num17++;
													}
													return;
												}
												for (int num18 = 0; num18 < 50; num18++)
												{
													Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
												}
												if (this.type == 13)
												{
													Gore.NewGore(this.position, this.velocity, 24);
													Gore.NewGore(this.position, this.velocity, 25);
													return;
												}
												if (this.type == 14)
												{
													Gore.NewGore(this.position, this.velocity, 26);
													Gore.NewGore(this.position, this.velocity, 27);
													return;
												}
												Gore.NewGore(this.position, this.velocity, 28);
												Gore.NewGore(this.position, this.velocity, 29);
												return;
											}
											else
											{
												if (this.type == 17)
												{
													if (this.life > 0)
													{
														int num19 = 0;
														while ((double)num19 < dmg / (double)this.lifeMax * 100.0)
														{
															Vector2 arg_BA4_0 = this.position;
															int arg_BA4_1 = this.width;
															int arg_BA4_2 = this.height;
															int arg_BA4_3 = 5;
															float arg_BA4_4 = (float)hitDirection;
															float arg_BA4_5 = -1f;
															int arg_BA4_6 = 0;
															Color newColor = default(Color);
															Dust.NewDust(arg_BA4_0, arg_BA4_1, arg_BA4_2, arg_BA4_3, arg_BA4_4, arg_BA4_5, arg_BA4_6, newColor, 1f);
															num19++;
														}
														return;
													}
													for (int num20 = 0; num20 < 50; num20++)
													{
														Vector2 arg_BFE_0 = this.position;
														int arg_BFE_1 = this.width;
														int arg_BFE_2 = this.height;
														int arg_BFE_3 = 5;
														float arg_BFE_4 = 2.5f * (float)hitDirection;
														float arg_BFE_5 = -2.5f;
														int arg_BFE_6 = 0;
														Color newColor = default(Color);
														Dust.NewDust(arg_BFE_0, arg_BFE_1, arg_BFE_2, arg_BFE_3, arg_BFE_4, arg_BFE_5, arg_BFE_6, newColor, 1f);
													}
													Gore.NewGore(this.position, this.velocity, 30);
													Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 31);
													Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 31);
													Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 32);
													Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 32);
													return;
												}
												else
												{
													if (this.type == 22)
													{
														if (this.life > 0)
														{
															int num21 = 0;
															while ((double)num21 < dmg / (double)this.lifeMax * 100.0)
															{
																Vector2 arg_D26_0 = this.position;
																int arg_D26_1 = this.width;
																int arg_D26_2 = this.height;
																int arg_D26_3 = 5;
																float arg_D26_4 = (float)hitDirection;
																float arg_D26_5 = -1f;
																int arg_D26_6 = 0;
																Color newColor = default(Color);
																Dust.NewDust(arg_D26_0, arg_D26_1, arg_D26_2, arg_D26_3, arg_D26_4, arg_D26_5, arg_D26_6, newColor, 1f);
																num21++;
															}
															return;
														}
														for (int num22 = 0; num22 < 50; num22++)
														{
															Vector2 arg_D80_0 = this.position;
															int arg_D80_1 = this.width;
															int arg_D80_2 = this.height;
															int arg_D80_3 = 5;
															float arg_D80_4 = 2.5f * (float)hitDirection;
															float arg_D80_5 = -2.5f;
															int arg_D80_6 = 0;
															Color newColor = default(Color);
															Dust.NewDust(arg_D80_0, arg_D80_1, arg_D80_2, arg_D80_3, arg_D80_4, arg_D80_5, arg_D80_6, newColor, 1f);
														}
														Gore.NewGore(this.position, this.velocity, 73);
														Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 74);
														Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 74);
														Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 75);
														Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 75);
														return;
													}
													else
													{
														if (this.type == 37)
														{
															if (this.life > 0)
															{
																int num23 = 0;
																while ((double)num23 < dmg / (double)this.lifeMax * 100.0)
																{
																	Vector2 arg_EA8_0 = this.position;
																	int arg_EA8_1 = this.width;
																	int arg_EA8_2 = this.height;
																	int arg_EA8_3 = 5;
																	float arg_EA8_4 = (float)hitDirection;
																	float arg_EA8_5 = -1f;
																	int arg_EA8_6 = 0;
																	Color newColor = default(Color);
																	Dust.NewDust(arg_EA8_0, arg_EA8_1, arg_EA8_2, arg_EA8_3, arg_EA8_4, arg_EA8_5, arg_EA8_6, newColor, 1f);
																	num23++;
																}
																return;
															}
															for (int num24 = 0; num24 < 50; num24++)
															{
																Vector2 arg_F02_0 = this.position;
																int arg_F02_1 = this.width;
																int arg_F02_2 = this.height;
																int arg_F02_3 = 5;
																float arg_F02_4 = 2.5f * (float)hitDirection;
																float arg_F02_5 = -2.5f;
																int arg_F02_6 = 0;
																Color newColor = default(Color);
																Dust.NewDust(arg_F02_0, arg_F02_1, arg_F02_2, arg_F02_3, arg_F02_4, arg_F02_5, arg_F02_6, newColor, 1f);
															}
															Gore.NewGore(this.position, this.velocity, 58);
															Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 59);
															Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 59);
															Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 60);
															Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 60);
															return;
														}
														else
														{
															if (this.type == 18)
															{
																if (this.life > 0)
																{
																	int num25 = 0;
																	while ((double)num25 < dmg / (double)this.lifeMax * 100.0)
																	{
																		Vector2 arg_102A_0 = this.position;
																		int arg_102A_1 = this.width;
																		int arg_102A_2 = this.height;
																		int arg_102A_3 = 5;
																		float arg_102A_4 = (float)hitDirection;
																		float arg_102A_5 = -1f;
																		int arg_102A_6 = 0;
																		Color newColor = default(Color);
																		Dust.NewDust(arg_102A_0, arg_102A_1, arg_102A_2, arg_102A_3, arg_102A_4, arg_102A_5, arg_102A_6, newColor, 1f);
																		num25++;
																	}
																	return;
																}
																for (int num26 = 0; num26 < 50; num26++)
																{
																	Vector2 arg_1084_0 = this.position;
																	int arg_1084_1 = this.width;
																	int arg_1084_2 = this.height;
																	int arg_1084_3 = 5;
																	float arg_1084_4 = 2.5f * (float)hitDirection;
																	float arg_1084_5 = -2.5f;
																	int arg_1084_6 = 0;
																	Color newColor = default(Color);
																	Dust.NewDust(arg_1084_0, arg_1084_1, arg_1084_2, arg_1084_3, arg_1084_4, arg_1084_5, arg_1084_6, newColor, 1f);
																}
																Gore.NewGore(this.position, this.velocity, 33);
																Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 34);
																Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 34);
																Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 35);
																Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 35);
																return;
															}
															else
															{
																if (this.type == 19)
																{
																	if (this.life > 0)
																	{
																		int num27 = 0;
																		while ((double)num27 < dmg / (double)this.lifeMax * 100.0)
																		{
																			Vector2 arg_11AC_0 = this.position;
																			int arg_11AC_1 = this.width;
																			int arg_11AC_2 = this.height;
																			int arg_11AC_3 = 5;
																			float arg_11AC_4 = (float)hitDirection;
																			float arg_11AC_5 = -1f;
																			int arg_11AC_6 = 0;
																			Color newColor = default(Color);
																			Dust.NewDust(arg_11AC_0, arg_11AC_1, arg_11AC_2, arg_11AC_3, arg_11AC_4, arg_11AC_5, arg_11AC_6, newColor, 1f);
																			num27++;
																		}
																		return;
																	}
																	for (int num28 = 0; num28 < 50; num28++)
																	{
																		Vector2 arg_1206_0 = this.position;
																		int arg_1206_1 = this.width;
																		int arg_1206_2 = this.height;
																		int arg_1206_3 = 5;
																		float arg_1206_4 = 2.5f * (float)hitDirection;
																		float arg_1206_5 = -2.5f;
																		int arg_1206_6 = 0;
																		Color newColor = default(Color);
																		Dust.NewDust(arg_1206_0, arg_1206_1, arg_1206_2, arg_1206_3, arg_1206_4, arg_1206_5, arg_1206_6, newColor, 1f);
																	}
																	Gore.NewGore(this.position, this.velocity, 36);
																	Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 37);
																	Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 37);
																	Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 38);
																	Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 38);
																	return;
																}
																else
																{
																	if (this.type == 38)
																	{
																		if (this.life > 0)
																		{
																			int num29 = 0;
																			while ((double)num29 < dmg / (double)this.lifeMax * 100.0)
																			{
																				Vector2 arg_132E_0 = this.position;
																				int arg_132E_1 = this.width;
																				int arg_132E_2 = this.height;
																				int arg_132E_3 = 5;
																				float arg_132E_4 = (float)hitDirection;
																				float arg_132E_5 = -1f;
																				int arg_132E_6 = 0;
																				Color newColor = default(Color);
																				Dust.NewDust(arg_132E_0, arg_132E_1, arg_132E_2, arg_132E_3, arg_132E_4, arg_132E_5, arg_132E_6, newColor, 1f);
																				num29++;
																			}
																			return;
																		}
																		for (int num30 = 0; num30 < 50; num30++)
																		{
																			Vector2 arg_1388_0 = this.position;
																			int arg_1388_1 = this.width;
																			int arg_1388_2 = this.height;
																			int arg_1388_3 = 5;
																			float arg_1388_4 = 2.5f * (float)hitDirection;
																			float arg_1388_5 = -2.5f;
																			int arg_1388_6 = 0;
																			Color newColor = default(Color);
																			Dust.NewDust(arg_1388_0, arg_1388_1, arg_1388_2, arg_1388_3, arg_1388_4, arg_1388_5, arg_1388_6, newColor, 1f);
																		}
																		Gore.NewGore(this.position, this.velocity, 64);
																		Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 65);
																		Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 65);
																		Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 66);
																		Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 66);
																		return;
																	}
																	else
																	{
																		if (this.type == 20)
																		{
																			if (this.life > 0)
																			{
																				int num31 = 0;
																				while ((double)num31 < dmg / (double)this.lifeMax * 100.0)
																				{
																					Vector2 arg_14B0_0 = this.position;
																					int arg_14B0_1 = this.width;
																					int arg_14B0_2 = this.height;
																					int arg_14B0_3 = 5;
																					float arg_14B0_4 = (float)hitDirection;
																					float arg_14B0_5 = -1f;
																					int arg_14B0_6 = 0;
																					Color newColor = default(Color);
																					Dust.NewDust(arg_14B0_0, arg_14B0_1, arg_14B0_2, arg_14B0_3, arg_14B0_4, arg_14B0_5, arg_14B0_6, newColor, 1f);
																					num31++;
																				}
																				return;
																			}
																			for (int num32 = 0; num32 < 50; num32++)
																			{
																				Vector2 arg_150A_0 = this.position;
																				int arg_150A_1 = this.width;
																				int arg_150A_2 = this.height;
																				int arg_150A_3 = 5;
																				float arg_150A_4 = 2.5f * (float)hitDirection;
																				float arg_150A_5 = -2.5f;
																				int arg_150A_6 = 0;
																				Color newColor = default(Color);
																				Dust.NewDust(arg_150A_0, arg_150A_1, arg_150A_2, arg_150A_3, arg_150A_4, arg_150A_5, arg_150A_6, newColor, 1f);
																			}
																			Gore.NewGore(this.position, this.velocity, 39);
																			Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 40);
																			Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 40);
																			Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 41);
																			Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 41);
																			return;
																		}
																		else
																		{
																			if (this.type == 21 || this.type == 31 || this.type == 32 || this.type == 44 || this.type == 45)
																			{
																				if (this.life > 0)
																				{
																					int num33 = 0;
																					while ((double)num33 < dmg / (double)this.lifeMax * 50.0)
																					{
																						Vector2 arg_165B_0 = this.position;
																						int arg_165B_1 = this.width;
																						int arg_165B_2 = this.height;
																						int arg_165B_3 = 26;
																						float arg_165B_4 = (float)hitDirection;
																						float arg_165B_5 = -1f;
																						int arg_165B_6 = 0;
																						Color newColor = default(Color);
																						Dust.NewDust(arg_165B_0, arg_165B_1, arg_165B_2, arg_165B_3, arg_165B_4, arg_165B_5, arg_165B_6, newColor, 1f);
																						num33++;
																					}
																					return;
																				}
																				for (int num34 = 0; num34 < 20; num34++)
																				{
																					Vector2 arg_16B6_0 = this.position;
																					int arg_16B6_1 = this.width;
																					int arg_16B6_2 = this.height;
																					int arg_16B6_3 = 26;
																					float arg_16B6_4 = 2.5f * (float)hitDirection;
																					float arg_16B6_5 = -2.5f;
																					int arg_16B6_6 = 0;
																					Color newColor = default(Color);
																					Dust.NewDust(arg_16B6_0, arg_16B6_1, arg_16B6_2, arg_16B6_3, arg_16B6_4, arg_16B6_5, arg_16B6_6, newColor, 1f);
																				}
																				Gore.NewGore(this.position, this.velocity, 42);
																				Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 43);
																				Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 43);
																				Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 44);
																				Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 44);
																				return;
																			}
																			else
																			{
																				if (this.type == 39 || this.type == 40 || this.type == 41)
																				{
																					if (this.life > 0)
																					{
																						int num35 = 0;
																						while ((double)num35 < dmg / (double)this.lifeMax * 50.0)
																						{
																							Vector2 arg_17F3_0 = this.position;
																							int arg_17F3_1 = this.width;
																							int arg_17F3_2 = this.height;
																							int arg_17F3_3 = 26;
																							float arg_17F3_4 = (float)hitDirection;
																							float arg_17F3_5 = -1f;
																							int arg_17F3_6 = 0;
																							Color newColor = default(Color);
																							Dust.NewDust(arg_17F3_0, arg_17F3_1, arg_17F3_2, arg_17F3_3, arg_17F3_4, arg_17F3_5, arg_17F3_6, newColor, 1f);
																							num35++;
																						}
																						return;
																					}
																					for (int num36 = 0; num36 < 20; num36++)
																					{
																						Vector2 arg_184E_0 = this.position;
																						int arg_184E_1 = this.width;
																						int arg_184E_2 = this.height;
																						int arg_184E_3 = 26;
																						float arg_184E_4 = 2.5f * (float)hitDirection;
																						float arg_184E_5 = -2.5f;
																						int arg_184E_6 = 0;
																						Color newColor = default(Color);
																						Dust.NewDust(arg_184E_0, arg_184E_1, arg_184E_2, arg_184E_3, arg_184E_4, arg_184E_5, arg_184E_6, newColor, 1f);
																					}
																					Gore.NewGore(this.position, this.velocity, this.type - 39 + 67);
																					return;
																				}
																				else
																				{
																					if (this.type == 34)
																					{
																						if (this.life > 0)
																						{
																							int num37 = 0;
																							while ((double)num37 < dmg / (double)this.lifeMax * 50.0)
																							{
																								Vector2 arg_18FD_0 = new Vector2(this.position.X, this.position.Y);
																								int arg_18FD_1 = this.width;
																								int arg_18FD_2 = this.height;
																								int arg_18FD_3 = 6;
																								float arg_18FD_4 = -this.velocity.X * 0.2f;
																								float arg_18FD_5 = -this.velocity.Y * 0.2f;
																								int arg_18FD_6 = 100;
																								Color newColor = default(Color);
																								int num38 = Dust.NewDust(arg_18FD_0, arg_18FD_1, arg_18FD_2, arg_18FD_3, arg_18FD_4, arg_18FD_5, arg_18FD_6, newColor, 3f);
																								Main.dust[num38].noLight = true;
																								Main.dust[num38].noGravity = true;
																								Dust expr_1928 = Main.dust[num38];
																								expr_1928.velocity *= 2f;
																								Vector2 arg_199A_0 = new Vector2(this.position.X, this.position.Y);
																								int arg_199A_1 = this.width;
																								int arg_199A_2 = this.height;
																								int arg_199A_3 = 6;
																								float arg_199A_4 = -this.velocity.X * 0.2f;
																								float arg_199A_5 = -this.velocity.Y * 0.2f;
																								int arg_199A_6 = 100;
																								newColor = default(Color);
																								num38 = Dust.NewDust(arg_199A_0, arg_199A_1, arg_199A_2, arg_199A_3, arg_199A_4, arg_199A_5, arg_199A_6, newColor, 2f);
																								Main.dust[num38].noLight = true;
																								Dust expr_19B7 = Main.dust[num38];
																								expr_19B7.velocity *= 2f;
																								num37++;
																							}
																							return;
																						}
																						for (int num39 = 0; num39 < 20; num39++)
																						{
																							Vector2 arg_1A53_0 = new Vector2(this.position.X, this.position.Y);
																							int arg_1A53_1 = this.width;
																							int arg_1A53_2 = this.height;
																							int arg_1A53_3 = 6;
																							float arg_1A53_4 = -this.velocity.X * 0.2f;
																							float arg_1A53_5 = -this.velocity.Y * 0.2f;
																							int arg_1A53_6 = 100;
																							Color newColor = default(Color);
																							int num40 = Dust.NewDust(arg_1A53_0, arg_1A53_1, arg_1A53_2, arg_1A53_3, arg_1A53_4, arg_1A53_5, arg_1A53_6, newColor, 3f);
																							Main.dust[num40].noLight = true;
																							Main.dust[num40].noGravity = true;
																							Dust expr_1A7E = Main.dust[num40];
																							expr_1A7E.velocity *= 2f;
																							Vector2 arg_1AF0_0 = new Vector2(this.position.X, this.position.Y);
																							int arg_1AF0_1 = this.width;
																							int arg_1AF0_2 = this.height;
																							int arg_1AF0_3 = 6;
																							float arg_1AF0_4 = -this.velocity.X * 0.2f;
																							float arg_1AF0_5 = -this.velocity.Y * 0.2f;
																							int arg_1AF0_6 = 100;
																							newColor = default(Color);
																							num40 = Dust.NewDust(arg_1AF0_0, arg_1AF0_1, arg_1AF0_2, arg_1AF0_3, arg_1AF0_4, arg_1AF0_5, arg_1AF0_6, newColor, 2f);
																							Main.dust[num40].noLight = true;
																							Dust expr_1B0D = Main.dust[num40];
																							expr_1B0D.velocity *= 2f;
																						}
																						return;
																					}
																					else
																					{
																						if (this.type == 35 || this.type == 36)
																						{
																							if (this.life > 0)
																							{
																								int num41 = 0;
																								while ((double)num41 < dmg / (double)this.lifeMax * 100.0)
																								{
																									Vector2 arg_1B82_0 = this.position;
																									int arg_1B82_1 = this.width;
																									int arg_1B82_2 = this.height;
																									int arg_1B82_3 = 26;
																									float arg_1B82_4 = (float)hitDirection;
																									float arg_1B82_5 = -1f;
																									int arg_1B82_6 = 0;
																									Color newColor = default(Color);
																									Dust.NewDust(arg_1B82_0, arg_1B82_1, arg_1B82_2, arg_1B82_3, arg_1B82_4, arg_1B82_5, arg_1B82_6, newColor, 1f);
																									num41++;
																								}
																								return;
																							}
																							for (int num42 = 0; num42 < 150; num42++)
																							{
																								Vector2 arg_1BDD_0 = this.position;
																								int arg_1BDD_1 = this.width;
																								int arg_1BDD_2 = this.height;
																								int arg_1BDD_3 = 26;
																								float arg_1BDD_4 = 2.5f * (float)hitDirection;
																								float arg_1BDD_5 = -2.5f;
																								int arg_1BDD_6 = 0;
																								Color newColor = default(Color);
																								Dust.NewDust(arg_1BDD_0, arg_1BDD_1, arg_1BDD_2, arg_1BDD_3, arg_1BDD_4, arg_1BDD_5, arg_1BDD_6, newColor, 1f);
																							}
																							if (this.type == 35)
																							{
																								Gore.NewGore(this.position, this.velocity, 54);
																								Gore.NewGore(this.position, this.velocity, 55);
																								return;
																							}
																							Gore.NewGore(this.position, this.velocity, 56);
																							Gore.NewGore(this.position, this.velocity, 57);
																							Gore.NewGore(this.position, this.velocity, 57);
																							Gore.NewGore(this.position, this.velocity, 57);
																							return;
																						}
																						else
																						{
																							if (this.type == 23)
																							{
																								if (this.life > 0)
																								{
																									int num43 = 0;
																									while ((double)num43 < dmg / (double)this.lifeMax * 100.0)
																									{
																										int num44 = 25;
																										if (Main.rand.Next(2) == 0)
																										{
																											num44 = 6;
																										}
																										Vector2 arg_1CD6_0 = this.position;
																										int arg_1CD6_1 = this.width;
																										int arg_1CD6_2 = this.height;
																										int arg_1CD6_3 = num44;
																										float arg_1CD6_4 = (float)hitDirection;
																										float arg_1CD6_5 = -1f;
																										int arg_1CD6_6 = 0;
																										Color newColor = default(Color);
																										Dust.NewDust(arg_1CD6_0, arg_1CD6_1, arg_1CD6_2, arg_1CD6_3, arg_1CD6_4, arg_1CD6_5, arg_1CD6_6, newColor, 1f);
																										Vector2 arg_1D37_0 = new Vector2(this.position.X, this.position.Y);
																										int arg_1D37_1 = this.width;
																										int arg_1D37_2 = this.height;
																										int arg_1D37_3 = 6;
																										float arg_1D37_4 = this.velocity.X * 0.2f;
																										float arg_1D37_5 = this.velocity.Y * 0.2f;
																										int arg_1D37_6 = 100;
																										newColor = default(Color);
																										int num45 = Dust.NewDust(arg_1D37_0, arg_1D37_1, arg_1D37_2, arg_1D37_3, arg_1D37_4, arg_1D37_5, arg_1D37_6, newColor, 2f);
																										Main.dust[num45].noGravity = true;
																										num43++;
																									}
																									return;
																								}
																								for (int num46 = 0; num46 < 50; num46++)
																								{
																									int num47 = 25;
																									if (Main.rand.Next(2) == 0)
																									{
																										num47 = 6;
																									}
																									Vector2 arg_1DB4_0 = this.position;
																									int arg_1DB4_1 = this.width;
																									int arg_1DB4_2 = this.height;
																									int arg_1DB4_3 = num47;
																									float arg_1DB4_4 = (float)(2 * hitDirection);
																									float arg_1DB4_5 = -2f;
																									int arg_1DB4_6 = 0;
																									Color newColor = default(Color);
																									Dust.NewDust(arg_1DB4_0, arg_1DB4_1, arg_1DB4_2, arg_1DB4_3, arg_1DB4_4, arg_1DB4_5, arg_1DB4_6, newColor, 1f);
																								}
																								for (int num48 = 0; num48 < 50; num48++)
																								{
																									Vector2 arg_1E29_0 = new Vector2(this.position.X, this.position.Y);
																									int arg_1E29_1 = this.width;
																									int arg_1E29_2 = this.height;
																									int arg_1E29_3 = 6;
																									float arg_1E29_4 = this.velocity.X * 0.2f;
																									float arg_1E29_5 = this.velocity.Y * 0.2f;
																									int arg_1E29_6 = 100;
																									Color newColor = default(Color);
																									int num49 = Dust.NewDust(arg_1E29_0, arg_1E29_1, arg_1E29_2, arg_1E29_3, arg_1E29_4, arg_1E29_5, arg_1E29_6, newColor, 2.5f);
																									Dust expr_1E38 = Main.dust[num49];
																									expr_1E38.velocity *= 6f;
																									Main.dust[num49].noGravity = true;
																								}
																								return;
																							}
																							else
																							{
																								if (this.type == 24)
																								{
																									if (this.life > 0)
																									{
																										int num50 = 0;
																										while ((double)num50 < dmg / (double)this.lifeMax * 100.0)
																										{
																											Vector2 arg_1ED8_0 = new Vector2(this.position.X, this.position.Y);
																											int arg_1ED8_1 = this.width;
																											int arg_1ED8_2 = this.height;
																											int arg_1ED8_3 = 6;
																											float arg_1ED8_4 = this.velocity.X;
																											float arg_1ED8_5 = this.velocity.Y;
																											int arg_1ED8_6 = 100;
																											Color newColor = default(Color);
																											int num51 = Dust.NewDust(arg_1ED8_0, arg_1ED8_1, arg_1ED8_2, arg_1ED8_3, arg_1ED8_4, arg_1ED8_5, arg_1ED8_6, newColor, 2.5f);
																											Main.dust[num51].noGravity = true;
																											num50++;
																										}
																										return;
																									}
																									for (int num52 = 0; num52 < 50; num52++)
																									{
																										Vector2 arg_1F66_0 = new Vector2(this.position.X, this.position.Y);
																										int arg_1F66_1 = this.width;
																										int arg_1F66_2 = this.height;
																										int arg_1F66_3 = 6;
																										float arg_1F66_4 = this.velocity.X;
																										float arg_1F66_5 = this.velocity.Y;
																										int arg_1F66_6 = 100;
																										Color newColor = default(Color);
																										int num53 = Dust.NewDust(arg_1F66_0, arg_1F66_1, arg_1F66_2, arg_1F66_3, arg_1F66_4, arg_1F66_5, arg_1F66_6, newColor, 2.5f);
																										Main.dust[num53].noGravity = true;
																										Dust expr_1F83 = Main.dust[num53];
																										expr_1F83.velocity *= 2f;
																									}
																									Gore.NewGore(this.position, this.velocity, 45);
																									Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 46);
																									Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 46);
																									Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 47);
																									Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 47);
																									return;
																								}
																								else
																								{
																									if (this.type == 25)
																									{
																										//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
																										for (int num54 = 0; num54 < 20; num54++)
																										{
																											Vector2 arg_210A_0 = new Vector2(this.position.X, this.position.Y);
																											int arg_210A_1 = this.width;
																											int arg_210A_2 = this.height;
																											int arg_210A_3 = 6;
																											float arg_210A_4 = -this.velocity.X * 0.2f;
																											float arg_210A_5 = -this.velocity.Y * 0.2f;
																											int arg_210A_6 = 100;
																											Color newColor = default(Color);
																											int num55 = Dust.NewDust(arg_210A_0, arg_210A_1, arg_210A_2, arg_210A_3, arg_210A_4, arg_210A_5, arg_210A_6, newColor, 2f);
																											Main.dust[num55].noGravity = true;
																											Dust expr_2127 = Main.dust[num55];
																											expr_2127.velocity *= 2f;
																											Vector2 arg_2199_0 = new Vector2(this.position.X, this.position.Y);
																											int arg_2199_1 = this.width;
																											int arg_2199_2 = this.height;
																											int arg_2199_3 = 6;
																											float arg_2199_4 = -this.velocity.X * 0.2f;
																											float arg_2199_5 = -this.velocity.Y * 0.2f;
																											int arg_2199_6 = 100;
																											newColor = default(Color);
																											num55 = Dust.NewDust(arg_2199_0, arg_2199_1, arg_2199_2, arg_2199_3, arg_2199_4, arg_2199_5, arg_2199_6, newColor, 1f);
																											Dust expr_21A8 = Main.dust[num55];
																											expr_21A8.velocity *= 2f;
																										}
																										return;
																									}
																									if (this.type == 33)
																									{
																										//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
																										for (int num56 = 0; num56 < 20; num56++)
																										{
																											Vector2 arg_2260_0 = new Vector2(this.position.X, this.position.Y);
																											int arg_2260_1 = this.width;
																											int arg_2260_2 = this.height;
																											int arg_2260_3 = 29;
																											float arg_2260_4 = -this.velocity.X * 0.2f;
																											float arg_2260_5 = -this.velocity.Y * 0.2f;
																											int arg_2260_6 = 100;
																											Color newColor = default(Color);
																											int num57 = Dust.NewDust(arg_2260_0, arg_2260_1, arg_2260_2, arg_2260_3, arg_2260_4, arg_2260_5, arg_2260_6, newColor, 2f);
																											Main.dust[num57].noGravity = true;
																											Dust expr_227D = Main.dust[num57];
																											expr_227D.velocity *= 2f;
																											Vector2 arg_22F0_0 = new Vector2(this.position.X, this.position.Y);
																											int arg_22F0_1 = this.width;
																											int arg_22F0_2 = this.height;
																											int arg_22F0_3 = 29;
																											float arg_22F0_4 = -this.velocity.X * 0.2f;
																											float arg_22F0_5 = -this.velocity.Y * 0.2f;
																											int arg_22F0_6 = 100;
																											newColor = default(Color);
																											num57 = Dust.NewDust(arg_22F0_0, arg_22F0_1, arg_22F0_2, arg_22F0_3, arg_22F0_4, arg_22F0_5, arg_22F0_6, newColor, 1f);
																											Dust expr_22FF = Main.dust[num57];
																											expr_22FF.velocity *= 2f;
																										}
																										return;
																									}
																									if (this.type == 26 || this.type == 27 || this.type == 28 || this.type == 29)
																									{
																										if (this.life > 0)
																										{
																											int num58 = 0;
																											while ((double)num58 < dmg / (double)this.lifeMax * 100.0)
																											{
																												Vector2 arg_2387_0 = this.position;
																												int arg_2387_1 = this.width;
																												int arg_2387_2 = this.height;
																												int arg_2387_3 = 5;
																												float arg_2387_4 = (float)hitDirection;
																												float arg_2387_5 = -1f;
																												int arg_2387_6 = 0;
																												Color newColor = default(Color);
																												Dust.NewDust(arg_2387_0, arg_2387_1, arg_2387_2, arg_2387_3, arg_2387_4, arg_2387_5, arg_2387_6, newColor, 1f);
																												num58++;
																											}
																											return;
																										}
																										for (int num59 = 0; num59 < 50; num59++)
																										{
																											Vector2 arg_23E1_0 = this.position;
																											int arg_23E1_1 = this.width;
																											int arg_23E1_2 = this.height;
																											int arg_23E1_3 = 5;
																											float arg_23E1_4 = 2.5f * (float)hitDirection;
																											float arg_23E1_5 = -2.5f;
																											int arg_23E1_6 = 0;
																											Color newColor = default(Color);
																											Dust.NewDust(arg_23E1_0, arg_23E1_1, arg_23E1_2, arg_23E1_3, arg_23E1_4, arg_23E1_5, arg_23E1_6, newColor, 1f);
																										}
																										Gore.NewGore(this.position, this.velocity, 48);
																										Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 49);
																										Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 49);
																										Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 50);
																										Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 50);
																										return;
																									}
																									else
																									{
																										if (this.type == 30)
																										{
																											//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
																											for (int num60 = 0; num60 < 20; num60++)
																											{
																												Vector2 arg_2557_0 = new Vector2(this.position.X, this.position.Y);
																												int arg_2557_1 = this.width;
																												int arg_2557_2 = this.height;
																												int arg_2557_3 = 27;
																												float arg_2557_4 = -this.velocity.X * 0.2f;
																												float arg_2557_5 = -this.velocity.Y * 0.2f;
																												int arg_2557_6 = 100;
																												Color newColor = default(Color);
																												int num61 = Dust.NewDust(arg_2557_0, arg_2557_1, arg_2557_2, arg_2557_3, arg_2557_4, arg_2557_5, arg_2557_6, newColor, 2f);
																												Main.dust[num61].noGravity = true;
																												Dust expr_2574 = Main.dust[num61];
																												expr_2574.velocity *= 2f;
																												Vector2 arg_25E7_0 = new Vector2(this.position.X, this.position.Y);
																												int arg_25E7_1 = this.width;
																												int arg_25E7_2 = this.height;
																												int arg_25E7_3 = 27;
																												float arg_25E7_4 = -this.velocity.X * 0.2f;
																												float arg_25E7_5 = -this.velocity.Y * 0.2f;
																												int arg_25E7_6 = 100;
																												newColor = default(Color);
																												num61 = Dust.NewDust(arg_25E7_0, arg_25E7_1, arg_25E7_2, arg_25E7_3, arg_25E7_4, arg_25E7_5, arg_25E7_6, newColor, 1f);
																												Dust expr_25F6 = Main.dust[num61];
																												expr_25F6.velocity *= 2f;
																											}
																											return;
																										}
																										if (this.type == 42)
																										{
																											if (this.life > 0)
																											{
																												int num62 = 0;
																												while ((double)num62 < dmg / (double)this.lifeMax * 100.0)
																												{
																													Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
																													num62++;
																												}
																												return;
																											}
																											for (int num63 = 0; num63 < 50; num63++)
																											{
																												Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
																											}
																											Gore.NewGore(this.position, this.velocity, 70);
																											Gore.NewGore(this.position, this.velocity, 71);
																											return;
																										}
																										else
																										{
																											if (this.type == 43)
																											{
																												if (this.life > 0)
																												{
																													int num64 = 0;
																													while ((double)num64 < dmg / (double)this.lifeMax * 100.0)
																													{
																														Dust.NewDust(this.position, this.width, this.height, 40, (float)hitDirection, -1f, this.alpha, this.color, 1.2f);
																														num64++;
																													}
																													return;
																												}
																												for (int num65 = 0; num65 < 50; num65++)
																												{
																													Dust.NewDust(this.position, this.width, this.height, 40, (float)hitDirection, -2f, this.alpha, this.color, 1.2f);
																												}
																												Gore.NewGore(this.position, this.velocity, 72);
																												Gore.NewGore(this.position, this.velocity, 72);
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
				}
			}
		}
		public void UpdateNPC(int i)
		{
			this.whoAmI = i;
			if (this.active)
			{
				float num = 10f;
				float num2 = 0.3f;
				if (this.wet)
				{
					num2 = 0.2f;
					num = 7f;
				}
				if (this.soundDelay > 0)
				{
					this.soundDelay--;
				}
				if (this.life <= 0)
				{
					this.active = false;
				}
				this.oldTarget = this.target;
				this.oldDirection = this.direction;
				this.oldDirectionY = this.directionY;
				this.AI();
				if (this.type == 44)
				{
					//Lighting.addLight((int)(this.position.X + (float)(this.width / 2)) / 16, (int)(this.position.Y + 4f) / 16, 0.6f);
				}
				for (int j = 0; j < 256; j++)
				{
					if (this.immune[j] > 0)
					{
						this.immune[j]--;
					}
				}
				if (!this.noGravity)
				{
					this.velocity.Y = this.velocity.Y + num2;
					if (this.velocity.Y > num)
					{
						this.velocity.Y = num;
					}
				}
				if ((double)this.velocity.X < 0.005 && (double)this.velocity.X > -0.005)
				{
					this.velocity.X = 0f;
				}
				if (Main.netMode != 1 && this.friendly && this.type != 37)
				{
					if (this.life < this.lifeMax)
					{
						this.friendlyRegen++;
						if (this.friendlyRegen > 300)
						{
							this.friendlyRegen = 0;
							this.life++;
							this.netUpdate = true;
						}
					}
					if (this.immune[255] == 0)
					{
						Rectangle rectangle = new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height);
						for (int k = 0; k < 1000; k++)
						{
							if (Main.npc[k].active && !Main.npc[k].friendly)
							{
								Rectangle rectangle2 = new Rectangle((int)Main.npc[k].position.X, (int)Main.npc[k].position.Y, Main.npc[k].width, Main.npc[k].height);
								if (rectangle.Intersects(rectangle2))
								{
									int num3 = Main.npc[k].damage;
									int num4 = 6;
									int num5 = 1;
									if (Main.npc[k].position.X + (float)(Main.npc[k].width / 2) > this.position.X + (float)(this.width / 2))
									{
										num5 = -1;
									}
									Main.npc[i].StrikeNPC(num3, (float)num4, num5);
									if (Main.netMode != 0)
									{
										NetMessage.SendData(28, -1, -1, "", i, (float)num3, (float)num4, (float)num5);
									}
									this.netUpdate = true;
									this.immune[255] = 30;
								}
							}
						}
					}
				}
				if (!this.noTileCollide)
				{
					bool flag = Collision.LavaCollision(this.position, this.width, this.height);
					if (flag)
					{
						this.lavaWet = true;
						if (Main.netMode != 1 && this.immune[255] == 0)
						{
							this.immune[255] = 30;
							this.StrikeNPC(50, 0f, 0);
							if (Main.netMode == 2 && Main.netMode != 0)
							{
								NetMessage.SendData(28, -1, -1, "", this.whoAmI, 50f, 0f, 0f);
							}
						}
					}
					bool flag2 = Collision.WetCollision(this.position, this.width, this.height);
					if (flag2)
					{
						if (!this.wet && this.wetCount == 0)
						{
							this.wetCount = 10;
							if (!flag)
							{
								for (int l = 0; l < 50; l++)
								{
									int num6 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 33, 0f, 0f, 0, default(Color), 1f);
									Dust expr_48A_cp_0 = Main.dust[num6];
									expr_48A_cp_0.velocity.Y = expr_48A_cp_0.velocity.Y - 4f;
									Dust expr_4A8_cp_0 = Main.dust[num6];
									expr_4A8_cp_0.velocity.X = expr_4A8_cp_0.velocity.X * 2.5f;
									Main.dust[num6].scale = 1.3f;
									Main.dust[num6].alpha = 100;
									Main.dust[num6].noGravity = true;
								}
								//Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 0);
							}
							else
							{
								for (int m = 0; m < 20; m++)
								{
									int num7 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 35, 0f, 0f, 0, default(Color), 1f);
									Dust expr_590_cp_0 = Main.dust[num7];
									expr_590_cp_0.velocity.Y = expr_590_cp_0.velocity.Y - 1.5f;
									Dust expr_5AE_cp_0 = Main.dust[num7];
									expr_5AE_cp_0.velocity.X = expr_5AE_cp_0.velocity.X * 2.5f;
									Main.dust[num7].scale = 1.3f;
									Main.dust[num7].alpha = 100;
									Main.dust[num7].noGravity = true;
								}
								//Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
							}
						}
						this.wet = true;
					}
					else
					{
						if (this.wet)
						{
							this.velocity.X = this.velocity.X * 0.5f;
							this.wet = false;
							if (this.wetCount == 0)
							{
								this.wetCount = 10;
								if (!this.lavaWet)
								{
									for (int n = 0; n < 50; n++)
									{
										int num8 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2)), this.width + 12, 24, 33, 0f, 0f, 0, default(Color), 1f);
										Dust expr_6DE_cp_0 = Main.dust[num8];
										expr_6DE_cp_0.velocity.Y = expr_6DE_cp_0.velocity.Y - 4f;
										Dust expr_6FC_cp_0 = Main.dust[num8];
										expr_6FC_cp_0.velocity.X = expr_6FC_cp_0.velocity.X * 2.5f;
										Main.dust[num8].scale = 1.3f;
										Main.dust[num8].alpha = 100;
										Main.dust[num8].noGravity = true;
									}
									//Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 0);
								}
								else
								{
									for (int num9 = 0; num9 < 20; num9++)
									{
										int num10 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 35, 0f, 0f, 0, default(Color), 1f);
										Dust expr_7E4_cp_0 = Main.dust[num10];
										expr_7E4_cp_0.velocity.Y = expr_7E4_cp_0.velocity.Y - 1.5f;
										Dust expr_802_cp_0 = Main.dust[num10];
										expr_802_cp_0.velocity.X = expr_802_cp_0.velocity.X * 2.5f;
										Main.dust[num10].scale = 1.3f;
										Main.dust[num10].alpha = 100;
										Main.dust[num10].noGravity = true;
									}
									//Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
								}
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
					bool flag3 = false;
					if (this.aiStyle == 10)
					{
						flag3 = true;
					}
					if (this.aiStyle == 3 && this.directionY == 1)
					{
						flag3 = true;
					}
					this.oldVelocity = this.velocity;
					this.collideX = false;
					this.collideY = false;
					if (this.wet)
					{
						Vector2 vector = this.velocity;
						this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, flag3, flag3);
						Vector2 value = this.velocity * 0.5f;
						if (this.velocity.X != vector.X)
						{
							value.X = this.velocity.X;
							this.collideX = true;
						}
						if (this.velocity.Y != vector.Y)
						{
							value.Y = this.velocity.Y;
							this.collideY = true;
						}
						this.oldPosition = this.position;
						this.position += value;
					}
					else
					{
						this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, flag3, flag3);
						if (this.oldVelocity.X != this.velocity.X)
						{
							this.collideX = true;
						}
						if (this.oldVelocity.Y != this.velocity.Y)
						{
							this.collideY = true;
						}
						this.oldPosition = this.position;
						this.position += this.velocity;
					}
				}
				else
				{
					this.oldPosition = this.position;
					this.position += this.velocity;
				}
				if (!this.active)
				{
					this.netUpdate = true;
				}
				if (Main.netMode == 2 && this.netUpdate)
				{
					NetMessage.SendData(23, -1, -1, "", i, 0f, 0f, 0f);
				}
				this.FindFrame();
				this.CheckActive();
				this.netUpdate = false;
			}
		}
		public Color GetAlpha(Color newColor)
		{
			int r = (int)newColor.R - this.alpha;
			int g = (int)newColor.G - this.alpha;
			int b = (int)newColor.B - this.alpha;
			int num = (int)newColor.A - this.alpha;
			if (this.type == 25 || this.type == 30 || this.type == 33)
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
		public string GetChat()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			for (int i = 0; i < 1000; i++)
			{
				if (Main.npc[i].type == 17)
				{
					flag = true;
				}
				else
				{
					if (Main.npc[i].type == 18)
					{
						flag2 = true;
					}
					else
					{
						if (Main.npc[i].type == 19)
						{
							flag3 = true;
						}
						else
						{
							if (Main.npc[i].type == 20)
							{
								flag4 = true;
							}
							else
							{
								if (Main.npc[i].type == 37)
								{
									flag5 = true;
								}
								else
								{
									if (Main.npc[i].type == 38)
									{
										flag6 = true;
									}
								}
							}
						}
					}
				}
			}
			string result = "";
			if (this.type == 17)
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
								result = "Ah, they will tell tales of " + Main.player[Main.myPlayer].name + " some day... good ones I'm sure.";
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
								result = "Kosh, kapleck Mog. Oh sorry, thats klingon for 'Buy something or die.'";
							}
							else
							{
								result = Main.player[Main.myPlayer].name + " is it? I've heard good things, friend!";
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
				if (this.type == 18)
				{
					if (flag6 && Main.rand.Next(4) == 0)
					{
						result = "I wish that bomb maker would be more careful.  I'm getting tired of having to sew his limbs back on every day.";
					}
					else
					{
						if ((double)Main.player[Main.myPlayer].statLife < (double)Main.player[Main.myPlayer].statLifeMax * 0.33)
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
							if ((double)Main.player[Main.myPlayer].statLife < (double)Main.player[Main.myPlayer].statLifeMax * 0.66)
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
										result = "Thats not the biggest I've ever seen... Yes, I've seen bigger wounds for sure.";
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
					if (this.type == 19)
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
						if (this.type == 20)
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
													result = "Be safe; Terraria_Server needs you!";
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
															result = "Whats this about me having more 'bark' than bite?";
														}
														else
														{
															if (num7 == 4)
															{
																result = "So two goblins walk into a bar, and one says to the other, 'Want to get a Gobblet of beer?!'";
															}
															else
															{
																result = "Be safe; Terraria_Server needs you!";
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
							if (this.type == 22)
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
											result = "Greetings, " + Main.player[Main.myPlayer].name + ". Is there something I can help you with?";
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
													result = "They say there is a person who will tell you how to survive in this land... oh wait. Thats me.";
												}
											}
										}
									}
								}
							}
							else
							{
								if (this.type == 37)
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
								}
								else
								{
									if (this.type == 38)
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
														int num10 = Main.rand.Next(4);
														if (num10 == 0)
														{
															result = "Explosives are da' bomb these days.  Buy some now!";
														}
														else
														{
															if (num10 == 1)
															{
																result = "It's a good day to die!";
															}
															else
															{
																if (num10 == 2)
																{
																	result = "I wonder what happens if I... (BOOM!)... Oh, sorry, did you need that leg?";
																}
																else
																{
																	if (num10 == 3)
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
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
