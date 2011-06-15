using System;

namespace Terraria_Server
{
    public class NPC
    {
        public static int immuneTime = 20;
        public static int maxAI = 4;
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
        private static bool noSpawnCycle = false;
        private static int activeTime = 750;
        private static int defaultSpawnRate = 600;
        private static int defaultMaxSpawns = 5;
        public bool wet = false;
        public byte wetCount = 0;
        public bool lavaWet = false;
        public static bool downedBoss1 = false;
        public static bool downedBoss2 = false;
        public static bool downedBoss3 = false;
        private static int spawnRate = NPC.defaultSpawnRate;
        private static int maxSpawns = NPC.defaultMaxSpawns;
        public int soundDelay = 0;
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 oldPosition;
        public Vector2 oldVelocity;
        public int width;
        public int height;
        public bool active;
        public int[] immune = new int[256];
        public int direction = 1;
        public int directionY = 1;
        public int type;
        public float[] ai = new float[NPC.maxAI];
        public int aiAction = 0;
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
        public Color color = new Color();
        public int alpha;
        public float scale = 1f;
        public float knockBackResist = 1f;
        public int oldDirection = 0;
        public int oldDirectionY = 0;
        public int oldTarget = 0;
        public int whoAmI = 0;
        public float rotation = 0f;
        public bool noGravity = false;
        public bool noTileCollide = false;
        public bool netUpdate = false;
        public bool collideX = false;
        public bool collideY = false;
        public bool boss = false;
        public int spriteDirection = -1;
        public bool behindTiles;
        public float value;
        public bool townNPC = false;
        public bool homeless = false;
        public int homeTileX = -1;
        public int homeTileY = -1;
        public bool friendly = false;
        public bool closeDoor = false;
        public int doorX = 0;
        public int doorY = 0;
        public int friendlyRegen = 0;
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
                                        if (Name == "Jungle Slime")
                                        {
                                            this.SetDefaults(1);
                                            this.name = Name;
                                            this.damage = 18;
                                            this.defense = 6;
                                            this.life = 60;
                                            this.scale = 1.1f;
                                            this.color = new Color(143, 215, 93, 100);
                                            this.value = 500f;
                                        }
                                        else
                                        {
                                            if (Name != "")
                                            {
                                                for (int i = 1; i < 59; i++)
                                                {
                                                    this.SetDefaults(i);
                                                    if (this.name == Name)
                                                    {
                                                        break;
                                                    }
                                                    if (i == 58)
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
                this.damage = 50;
                this.defense = 2;
                this.lifeMax = 140;
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
                this.damage = 25;
                this.defense = 7;
                this.lifeMax = 230;
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
                this.damage = 15;
                this.defense = 10;
                this.lifeMax = 350;
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
                this.defense = 15;
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
                    this.name = "Undead Miner";
                    this.width = 18;
                    this.height = 40;
                    this.aiStyle = 3;
                    this.damage = 22;
                    this.defense = 9;
                    this.lifeMax = 70;
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
                        this.lifeMax = 200;
                        this.soundHit = 2;
                        this.soundKilled = 2;
                        this.knockBackResist = 0.6f;
                        this.value = 5000f;
                    }
                    else
                    {
                        if (this.type == 46)
                        {
                            this.name = "Bunny";
                            this.friendly = true;
                            this.width = 18;
                            this.height = 20;
                            this.aiStyle = 7;
                            this.damage = 0;
                            this.defense = 0;
                            this.lifeMax = 5;
                            this.soundHit = 1;
                            this.soundKilled = 1;
                        }
                        else
                        {
                            if (this.type == 47)
                            {
                                this.name = "Corrupt Bunny";
                                this.width = 18;
                                this.height = 20;
                                this.aiStyle = 3;
                                this.damage = 30;
                                this.defense = 6;
                                this.lifeMax = 100;
                                this.soundHit = 1;
                                this.soundKilled = 1;
                                this.value = 500f;
                            }
                            else
                            {
                                if (this.type == 48)
                                {
                                    this.name = "Harpey";
                                    this.width = 24;
                                    this.height = 34;
                                    this.aiStyle = 14;
                                    this.damage = 25;
                                    this.defense = 8;
                                    this.lifeMax = 100;
                                    this.soundHit = 1;
                                    this.knockBackResist = 0.6f;
                                    this.soundKilled = 1;
                                    this.value = 500f;
                                }
                                else
                                {
                                    if (this.type == 49)
                                    {
                                        this.name = "Cave Bat";
                                        this.width = 12;
                                        this.height = 12;
                                        this.aiStyle = 14;
                                        this.damage = 25;
                                        this.defense = 2;
                                        this.lifeMax = 45;
                                        this.soundHit = 1;
                                        this.knockBackResist = 0.8f;
                                        this.soundKilled = 1;
                                        this.value = 100f;
                                    }
                                    else
                                    {
                                        if (this.type == 50)
                                        {
                                            this.boss = true;
                                            this.name = "King Slime";
                                            this.width = 98;
                                            this.height = 92;
                                            this.aiStyle = 15;
                                            this.damage = 40;
                                            this.defense = 10;
                                            this.lifeMax = 2000;
                                            this.knockBackResist = 0f;
                                            this.soundHit = 1;
                                            this.soundKilled = 1;
                                            this.alpha = 30;
                                            this.value = 10000f;
                                            this.scale = 1.25f;
                                        }
                                        else
                                        {
                                            if (this.type == 51)
                                            {
                                                this.name = "Jungle Bat";
                                                this.width = 12;
                                                this.height = 12;
                                                this.aiStyle = 14;
                                                this.damage = 20;
                                                this.defense = 4;
                                                this.lifeMax = 70;
                                                this.soundHit = 1;
                                                this.knockBackResist = 0.8f;
                                                this.soundKilled = 1;
                                                this.value = 100f;
                                            }
                                            else
                                            {
                                                if (this.type == 52)
                                                {
                                                    this.name = "Doctor Bones";
                                                    this.width = 18;
                                                    this.height = 40;
                                                    this.aiStyle = 3;
                                                    this.damage = 20;
                                                    this.defense = 10;
                                                    this.lifeMax = 500;
                                                    this.soundHit = 1;
                                                    this.soundKilled = 2;
                                                    this.knockBackResist = 0.5f;
                                                    this.value = 60f;
                                                }
                                                else
                                                {
                                                    if (this.type == 53)
                                                    {
                                                        this.name = "The Groom";
                                                        this.width = 18;
                                                        this.height = 40;
                                                        this.aiStyle = 3;
                                                        this.damage = 14;
                                                        this.defense = 8;
                                                        this.lifeMax = 200;
                                                        this.soundHit = 1;
                                                        this.soundKilled = 2;
                                                        this.knockBackResist = 0.5f;
                                                        this.value = 60f;
                                                    }
                                                    else
                                                    {
                                                        if (this.type == 54)
                                                        {
                                                            this.townNPC = true;
                                                            this.friendly = true;
                                                            this.name = "Clothier";
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
                                                        else
                                                        {
                                                            if (this.type == 55)
                                                            {
                                                                this.friendly = true;
                                                                this.noGravity = true;
                                                                this.name = "Goldfish";
                                                                this.width = 20;
                                                                this.height = 18;
                                                                this.aiStyle = 16;
                                                                this.damage = 0;
                                                                this.defense = 0;
                                                                this.lifeMax = 5;
                                                                this.soundHit = 1;
                                                                this.soundKilled = 1;
                                                                this.knockBackResist = 0.5f;
                                                            }
                                                            else
                                                            {
                                                                if (this.type == 56)
                                                                {
                                                                    this.noGravity = true;
                                                                    this.name = "Snatcher";
                                                                    this.width = 30;
                                                                    this.height = 30;
                                                                    this.aiStyle = 13;
                                                                    this.damage = 25;
                                                                    this.defense = 10;
                                                                    this.lifeMax = 100;
                                                                    this.soundHit = 1;
                                                                    this.knockBackResist = 0.7f;
                                                                    this.soundKilled = 1;
                                                                    this.value = 500f;
                                                                }
                                                                else
                                                                {
                                                                    if (this.type == 57)
                                                                    {
                                                                        this.noGravity = true;
                                                                        this.name = "Corrupt Goldfish";
                                                                        this.width = 18;
                                                                        this.height = 20;
                                                                        this.aiStyle = 16;
                                                                        this.damage = 30;
                                                                        this.defense = 6;
                                                                        this.lifeMax = 100;
                                                                        this.soundHit = 1;
                                                                        this.soundKilled = 1;
                                                                        this.value = 500f;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (this.type == 58)
                                                                        {
                                                                            this.noGravity = true;
                                                                            this.name = "Piranha";
                                                                            this.width = 18;
                                                                            this.height = 20;
                                                                            this.aiStyle = 16;
                                                                            this.damage = 25;
                                                                            this.defense = 2;
                                                                            this.lifeMax = 30;
                                                                            this.soundHit = 1;
                                                                            this.soundKilled = 1;
                                                                            this.value = 300f;
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
        }
        public void AI()
        {
            if (this.aiStyle == 0)
            {
                this.velocity.X = this.velocity.X * 0.93f;
                if ((double)this.velocity.X > -0.1 && (double)this.velocity.X < 0.1)
                {
                    this.velocity.X = 0f;
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
                        this.TargetClosest(true);
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
                                this.TargetClosest(true);
                            }
                            if (this.ai[1] == 2f)
                            {
                                this.velocity.Y = -8f;
                                this.velocity.X = this.velocity.X + (float)(3 * this.direction);
                                this.ai[0] = -200f;
                                this.ai[1] = 0f;
                                this.ai[3] = this.position.X;
                            }
                            else
                            {
                                this.velocity.Y = -6f;
                                this.velocity.X = this.velocity.X + (float)(2 * this.direction);
                                this.ai[0] = -120f;
                                this.ai[1] += 1f;
                            }
                        }
                        else
                        {
                            if (this.ai[0] >= -30f)
                            {
                                this.aiAction = 1;
                            }
                        }
                    }
                    else
                    {
                        if (this.target < 255)
                        {
                            if ((this.direction == 1 && this.velocity.X < 3f) || (this.direction == -1 && this.velocity.X > -3f))
                            {
                                if ((this.direction == -1 && (double)this.velocity.X < 0.1) || (this.direction == 1 && (double)this.velocity.X > -0.1))
                                {
                                    this.velocity.X = this.velocity.X + 0.2f * (float)this.direction;
                                }
                                else
                                {
                                    this.velocity.X = this.velocity.X * 0.93f;
                                }
                            }
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
                            this.TargetClosest(true);
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
                            Vector2 arg_AC4_0 = new Vector2(this.position.X, this.position.Y + (float)this.height * 0.25f);
                            int arg_AC4_1 = this.width;
                            int arg_AC4_2 = (int)((float)this.height * 0.5f);
                            int arg_AC4_3 = 5;
                            float arg_AC4_4 = this.velocity.X;
                            float arg_AC4_5 = 2f;
                            int arg_AC4_6 = 0;
                            Color newColor = default(Color);
                            int num = Dust.NewDust(arg_AC4_0, arg_AC4_1, arg_AC4_2, arg_AC4_3, arg_AC4_4, arg_AC4_5, arg_AC4_6, newColor, 1f);
                            Dust expr_AD6_cp_0 = Main.dust[num];
                            expr_AD6_cp_0.velocity.X = expr_AD6_cp_0.velocity.X * 0.5f;
                            Dust expr_AF3_cp_0 = Main.dust[num];
                            expr_AF3_cp_0.velocity.Y = expr_AF3_cp_0.velocity.Y * 0.1f;
                        }
                    }
                    else
                    {
                        if (this.aiStyle == 3)
                        {
                            int num2 = 60;
                            bool flag = false;
                            if (this.velocity.Y == 0f)
                            {
                                if ((this.velocity.X > 0f && this.direction < 0) || (this.velocity.X < 0f && this.direction > 0))
                                {
                                    flag = true;
                                }
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
                            if ((!Main.dayTime || (double)this.position.Y > Main.worldSurface * 16.0 || this.type == 26 || this.type == 27 || this.type == 28 || this.type == 31 || this.type == 47) && this.ai[3] < (float)num2)
                            {
                                if (this.type == 3 || this.type == 21 || this.type == 31)
                                {
                                    if (Main.rand.Next(1000) == 0)
                                    {
                                        //Main.PlaySound(14, (int)this.position.X, (int)this.position.Y, 1);
                                    }
                                }
                                this.TargetClosest(true);
                            }
                            else
                            {
                                if (Main.dayTime && (double)(this.position.Y / 16f) < Main.worldSurface)
                                {
                                    if (this.timeLeft > 10)
                                    {
                                        this.timeLeft = 10;
                                    }
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
                                if (this.type == 21 || this.type == 26 || this.type == 31 || this.type == 47)
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
                            if (this.velocity.Y == 0f)
                            {
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
                                bool flag2 = true;
                                if (this.type == 47)
                                {
                                    flag2 = false;
                                }
                                if (Main.tile[num3, num4 - 1].active && Main.tile[num3, num4 - 1].type == 10 && flag2)
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
                                        bool flag3 = false;
                                        if (this.ai[1] >= 10f)
                                        {
                                            flag3 = true;
                                            this.ai[1] = 10f;
                                        }
                                        WorldGen.KillTile(num3, num4 - 1, true, false, false);
                                        if (Main.netMode != 1 || !flag3)
                                        {
                                            if (flag3 && Main.netMode != 1)
                                            {
                                                if (this.type == 26)
                                                {
                                                    WorldGen.KillTile(num3, num4 - 1, false, false, false);
                                                    if (Main.netMode == 2)
                                                    {
                                                        NetMessage.SendData(17, -1, -1, "", 0, (float)num3, (float)(num4 - 1), 0f);
                                                    }
                                                }
                                                else
                                                {
                                                    bool flag4 = WorldGen.OpenDoor(num3, num4, this.direction);
                                                    if (!flag4)
                                                    {
                                                        this.ai[3] = (float)num2;
                                                        this.netUpdate = true;
                                                    }
                                                    if (Main.netMode == 2 && flag4)
                                                    {
                                                        NetMessage.SendData(19, -1, -1, "", 0, (float)num3, (float)num4, (float)this.direction);
                                                    }
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
                                    if ((this.type == 31 || this.type == 47) && this.velocity.Y == 0f)
                                    {
                                        if (Math.Abs(this.position.X + (float)(this.width / 2) - (Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2))) < 100f && Math.Abs(this.position.Y + (float)(this.height / 2) - (Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2))) < 50f)
                                        {
                                            if ((this.direction > 0 && this.velocity.X >= 1f) || (this.direction < 0 && this.velocity.X <= -1f))
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
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                this.ai[1] = 0f;
                                this.ai[2] = 0f;
                            }
                        }
                        else
                        {
                            if (this.aiStyle == 4)
                            {
                                if (this.target < 0 || this.target == 255 || Main.player[this.target].dead || !Main.player[this.target].active)
                                {
                                    this.TargetClosest(true);
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
                                    Vector2 arg_1F8E_0 = new Vector2(this.position.X, this.position.Y + (float)this.height * 0.25f);
                                    int arg_1F8E_1 = this.width;
                                    int arg_1F8E_2 = (int)((float)this.height * 0.5f);
                                    int arg_1F8E_3 = 5;
                                    float arg_1F8E_4 = this.velocity.X;
                                    float arg_1F8E_5 = 2f;
                                    int arg_1F8E_6 = 0;
                                    Color newColor = default(Color);
                                    int num = Dust.NewDust(arg_1F8E_0, arg_1F8E_1, arg_1F8E_2, arg_1F8E_3, arg_1F8E_4, arg_1F8E_5, arg_1F8E_6, newColor, 1f);
                                    Dust expr_1FA0_cp_0 = Main.dust[num];
                                    expr_1FA0_cp_0.velocity.X = expr_1FA0_cp_0.velocity.X * 0.5f;
                                    Dust expr_1FBD_cp_0 = Main.dust[num];
                                    expr_1FBD_cp_0.velocity.Y = expr_1FBD_cp_0.velocity.Y * 0.1f;
                                }
                                if (Main.dayTime || dead)
                                {
                                    this.velocity.Y = this.velocity.Y - 0.04f;
                                    if (this.timeLeft > 10)
                                    {
                                        this.timeLeft = 10;
                                    }
                                }
                                else
                                {
                                    if (this.ai[0] == 0f)
                                    {
                                        if (this.ai[1] == 0f)
                                        {
                                            float num9 = 5f;
                                            float num10 = 0.04f;
                                            Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                            float num11 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
                                            float num12 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - 200f - vector.Y;
                                            float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                            float num14 = num13;
                                            num13 = num9 / num13;
                                            num11 *= num13;
                                            num12 *= num13;
                                            if (this.velocity.X < num11)
                                            {
                                                this.velocity.X = this.velocity.X + num10;
                                                if (this.velocity.X < 0f && num11 > 0f)
                                                {
                                                    this.velocity.X = this.velocity.X + num10;
                                                }
                                            }
                                            else
                                            {
                                                if (this.velocity.X > num11)
                                                {
                                                    this.velocity.X = this.velocity.X - num10;
                                                    if (this.velocity.X > 0f && num11 < 0f)
                                                    {
                                                        this.velocity.X = this.velocity.X - num10;
                                                    }
                                                }
                                            }
                                            if (this.velocity.Y < num12)
                                            {
                                                this.velocity.Y = this.velocity.Y + num10;
                                                if (this.velocity.Y < 0f && num12 > 0f)
                                                {
                                                    this.velocity.Y = this.velocity.Y + num10;
                                                }
                                            }
                                            else
                                            {
                                                if (this.velocity.Y > num12)
                                                {
                                                    this.velocity.Y = this.velocity.Y - num10;
                                                    if (this.velocity.Y > 0f && num12 < 0f)
                                                    {
                                                        this.velocity.Y = this.velocity.Y - num10;
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
                                                if (this.position.Y + (float)this.height < Main.player[this.target].position.Y && num14 < 500f)
                                                {
                                                    if (!Main.player[this.target].dead)
                                                    {
                                                        this.ai[3] += 1f;
                                                    }
                                                    if (this.ai[3] >= 90f)
                                                    {
                                                        this.ai[3] = 0f;
                                                        this.rotation = num7;
                                                        float num15 = 5f;
                                                        float num16 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
                                                        float num17 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector.Y;
                                                        float num18 = (float)Math.Sqrt((double)(num16 * num16 + num17 * num17));
                                                        num18 = num15 / num18;
                                                        Vector2 vector2 = vector;
                                                        Vector2 vector3;
                                                        vector3.X = num16 * num18;
                                                        vector3.Y = num17 * num18;
                                                        vector2.X += vector3.X * 10f;
                                                        vector2.Y += vector3.Y * 10f;
                                                        if (Main.netMode != 1)
                                                        {
                                                            int num19 = NPC.NewNPC((int)vector2.X, (int)vector2.Y, 5, 0);
                                                            Main.npc[num19].velocity.X = vector3.X;
                                                            Main.npc[num19].velocity.Y = vector3.Y;
                                                            if (Main.netMode == 2 && num19 < 1000)
                                                            {
                                                                NetMessage.SendData(23, -1, -1, "", num19, 0f, 0f, 0f);
                                                            }
                                                        }
                                                        //Main.PlaySound(3, (int)vector2.X, (int)vector2.Y, 1);
                                                        for (int i = 0; i < 10; i++)
                                                        {
                                                            Vector2 arg_25D0_0 = vector2;
                                                            int arg_25D0_1 = 20;
                                                            int arg_25D0_2 = 20;
                                                            int arg_25D0_3 = 5;
                                                            float arg_25D0_4 = vector3.X * 0.4f;
                                                            float arg_25D0_5 = vector3.Y * 0.4f;
                                                            int arg_25D0_6 = 0;
                                                            Color newColor = default(Color);
                                                            Dust.NewDust(arg_25D0_0, arg_25D0_1, arg_25D0_2, arg_25D0_3, arg_25D0_4, arg_25D0_5, arg_25D0_6, newColor, 1f);
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
                                                float num9 = 7f;
                                                Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                float num11 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
                                                float num12 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector.Y;
                                                float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                num13 = num9 / num13;
                                                this.velocity.X = num11 * num13;
                                                this.velocity.Y = num12 * num13;
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
                                                    for (int i = 0; i < 2; i++)
                                                    {
                                                        Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 8);
                                                        Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7);
                                                        Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 6);
                                                    }
                                                    for (int i = 0; i < 20; i++)
                                                    {
                                                        Vector2 arg_2C18_0 = this.position;
                                                        int arg_2C18_1 = this.width;
                                                        int arg_2C18_2 = this.height;
                                                        int arg_2C18_3 = 5;
                                                        float arg_2C18_4 = (float)Main.rand.Next(-30, 31) * 0.2f;
                                                        float arg_2C18_5 = (float)Main.rand.Next(-30, 31) * 0.2f;
                                                        int arg_2C18_6 = 0;
                                                        newColor = default(Color);
                                                        Dust.NewDust(arg_2C18_0, arg_2C18_1, arg_2C18_2, arg_2C18_3, arg_2C18_4, arg_2C18_5, arg_2C18_6, newColor, 1f);
                                                    }
                                                    //Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
                                                }
                                            }
                                            Vector2 arg_2CA1_0 = this.position;
                                            int arg_2CA1_1 = this.width;
                                            int arg_2CA1_2 = this.height;
                                            int arg_2CA1_3 = 5;
                                            float arg_2CA1_4 = (float)Main.rand.Next(-30, 31) * 0.2f;
                                            float arg_2CA1_5 = (float)Main.rand.Next(-30, 31) * 0.2f;
                                            int arg_2CA1_6 = 0;
                                            newColor = default(Color);
                                            Dust.NewDust(arg_2CA1_0, arg_2CA1_1, arg_2CA1_2, arg_2CA1_3, arg_2CA1_4, arg_2CA1_5, arg_2CA1_6, newColor, 1f);
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
                                            this.damage = 30;
                                            this.defense = 6;
                                            if (this.ai[1] == 0f)
                                            {
                                                float num9 = 6f;
                                                float num10 = 0.07f;
                                                Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                float num11 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
                                                float num12 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - 120f - vector.Y;
                                                float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                num13 = num9 / num13;
                                                num11 *= num13;
                                                num12 *= num13;
                                                if (this.velocity.X < num11)
                                                {
                                                    this.velocity.X = this.velocity.X + num10;
                                                    if (this.velocity.X < 0f && num11 > 0f)
                                                    {
                                                        this.velocity.X = this.velocity.X + num10;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.velocity.X > num11)
                                                    {
                                                        this.velocity.X = this.velocity.X - num10;
                                                        if (this.velocity.X > 0f && num11 < 0f)
                                                        {
                                                            this.velocity.X = this.velocity.X - num10;
                                                        }
                                                    }
                                                }
                                                if (this.velocity.Y < num12)
                                                {
                                                    this.velocity.Y = this.velocity.Y + num10;
                                                    if (this.velocity.Y < 0f && num12 > 0f)
                                                    {
                                                        this.velocity.Y = this.velocity.Y + num10;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.velocity.Y > num12)
                                                    {
                                                        this.velocity.Y = this.velocity.Y - num10;
                                                        if (this.velocity.Y > 0f && num12 < 0f)
                                                        {
                                                            this.velocity.Y = this.velocity.Y - num10;
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
                                                }
                                            }
                                            else
                                            {
                                                if (this.ai[1] == 1f)
                                                {
                                                    //Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
                                                    this.rotation = num7;
                                                    float num9 = 8f;
                                                    Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                    float num11 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
                                                    float num12 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector.Y;
                                                    float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                    num13 = num9 / num13;
                                                    this.velocity.X = num11 * num13;
                                                    this.velocity.Y = num12 * num13;
                                                    this.ai[1] = 2f;
                                                }
                                                else
                                                {
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
                                                            }
                                                            else
                                                            {
                                                                this.ai[1] = 1f;
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
                                if (this.aiStyle == 5)
                                {
                                    if (this.target < 0 || this.target == 255 || Main.player[this.target].dead)
                                    {
                                        this.TargetClosest(true);
                                    }
                                    float num9 = 6f;
                                    float num10 = 0.05f;
                                    if (this.type == 6)
                                    {
                                        num9 = 4f;
                                        num10 = 0.02f;
                                    }
                                    else
                                    {
                                        if (this.type == 23)
                                        {
                                            num9 = 2.5f;
                                            num10 = 0.02f;
                                        }
                                    }
                                    Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                    float num11 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
                                    float num12 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector.Y;
                                    float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                    num13 = num9 / num13;
                                    num11 *= num13;
                                    num12 *= num13;
                                    if (Main.player[this.target].dead)
                                    {
                                        num11 = (float)this.direction * num9 / 2f;
                                        num12 = -num9 / 2f;
                                    }
                                    if (this.velocity.X < num11)
                                    {
                                        this.velocity.X = this.velocity.X + num10;
                                        if (this.velocity.X < 0f && num11 > 0f)
                                        {
                                            this.velocity.X = this.velocity.X + num10;
                                        }
                                    }
                                    else
                                    {
                                        if (this.velocity.X > num11)
                                        {
                                            this.velocity.X = this.velocity.X - num10;
                                            if (this.velocity.X > 0f && num11 < 0f)
                                            {
                                                this.velocity.X = this.velocity.X - num10;
                                            }
                                        }
                                    }
                                    if (this.velocity.Y < num12)
                                    {
                                        this.velocity.Y = this.velocity.Y + num10;
                                        if (this.velocity.Y < 0f && num12 > 0f)
                                        {
                                            this.velocity.Y = this.velocity.Y + num10;
                                        }
                                    }
                                    else
                                    {
                                        if (this.velocity.Y > num12)
                                        {
                                            this.velocity.Y = this.velocity.Y - num10;
                                            if (this.velocity.Y > 0f && num12 < 0f)
                                            {
                                                this.velocity.Y = this.velocity.Y - num10;
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
                                            Vector2 arg_39C3_0 = new Vector2(this.position.X - this.velocity.X, this.position.Y - this.velocity.Y);
                                            int arg_39C3_1 = this.width;
                                            int arg_39C3_2 = this.height;
                                            int arg_39C3_3 = 6;
                                            float arg_39C3_4 = this.velocity.X * 0.2f;
                                            float arg_39C3_5 = this.velocity.Y * 0.2f;
                                            int arg_39C3_6 = 100;
                                            Color newColor = default(Color);
                                            int num = Dust.NewDust(arg_39C3_0, arg_39C3_1, arg_39C3_2, arg_39C3_3, arg_39C3_4, arg_39C3_5, arg_39C3_6, newColor, 2f);
                                            Main.dust[num].noGravity = true;
                                            Dust expr_39E2_cp_0 = Main.dust[num];
                                            expr_39E2_cp_0.velocity.X = expr_39E2_cp_0.velocity.X * 0.3f;
                                            Dust expr_39FF_cp_0 = Main.dust[num];
                                            expr_39FF_cp_0.velocity.Y = expr_39FF_cp_0.velocity.Y * 0.3f;
                                        }
                                        else
                                        {
                                            if (Main.rand.Next(20) == 0)
                                            {
                                                int num = Dust.NewDust(new Vector2(this.position.X, this.position.Y + (float)this.height * 0.25f), this.width, (int)((float)this.height * 0.5f), 18, this.velocity.X, 2f, this.alpha, this.color, this.scale);
                                                Dust expr_3AA6_cp_0 = Main.dust[num];
                                                expr_3AA6_cp_0.velocity.X = expr_3AA6_cp_0.velocity.X * 0.5f;
                                                Dust expr_3AC3_cp_0 = Main.dust[num];
                                                expr_3AC3_cp_0.velocity.Y = expr_3AC3_cp_0.velocity.Y * 0.1f;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Main.rand.Next(40) == 0)
                                        {
                                            Vector2 arg_3B57_0 = new Vector2(this.position.X, this.position.Y + (float)this.height * 0.25f);
                                            int arg_3B57_1 = this.width;
                                            int arg_3B57_2 = (int)((float)this.height * 0.5f);
                                            int arg_3B57_3 = 5;
                                            float arg_3B57_4 = this.velocity.X;
                                            float arg_3B57_5 = 2f;
                                            int arg_3B57_6 = 0;
                                            Color newColor = default(Color);
                                            int num = Dust.NewDust(arg_3B57_0, arg_3B57_1, arg_3B57_2, arg_3B57_3, arg_3B57_4, arg_3B57_5, arg_3B57_6, newColor, 1f);
                                            Dust expr_3B69_cp_0 = Main.dust[num];
                                            expr_3B69_cp_0.velocity.X = expr_3B69_cp_0.velocity.X * 0.5f;
                                            Dust expr_3B86_cp_0 = Main.dust[num];
                                            expr_3B86_cp_0.velocity.Y = expr_3B86_cp_0.velocity.Y * 0.1f;
                                        }
                                    }
                                    if ((Main.dayTime && this.type != 6 && this.type != 23) || Main.player[this.target].dead)
                                    {
                                        this.velocity.Y = this.velocity.Y - num10 * 2f;
                                        if (this.timeLeft > 10)
                                        {
                                            this.timeLeft = 10;
                                        }
                                    }
                                }
                                else
                                {
                                    if (this.aiStyle == 6)
                                    {
                                        if (this.target < 0 || this.target == 255 || Main.player[this.target].dead)
                                        {
                                            this.TargetClosest(true);
                                        }
                                        if (Main.player[this.target].dead)
                                        {
                                            if (this.timeLeft > 10)
                                            {
                                                this.timeLeft = 10;
                                            }
                                        }
                                        if (Main.netMode != 1)
                                        {
                                            if (this.type == 7 || this.type == 8 || this.type == 10 || this.type == 11 || this.type == 13 || this.type == 14 || this.type == 39 || this.type == 40)
                                            {
                                                if (this.ai[0] == 0f)
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
                                            }
                                            if (this.type == 8 || this.type == 9 || this.type == 11 || this.type == 12 || this.type == 40 || this.type == 41)
                                            {
                                                if (!Main.npc[(int)this.ai[1]].active)
                                                {
                                                    this.life = 0;
                                                    this.HitEffect(0, 10.0);
                                                    this.active = false;
                                                }
                                            }
                                            if (this.type == 7 || this.type == 8 || this.type == 10 || this.type == 11 || this.type == 39 || this.type == 40)
                                            {
                                                if (!Main.npc[(int)this.ai[0]].active)
                                                {
                                                    this.life = 0;
                                                    this.HitEffect(0, 10.0);
                                                    this.active = false;
                                                }
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
                                                    int i = this.whoAmI;
                                                    int num20 = this.life;
                                                    float num21 = this.ai[0];
                                                    this.SetDefaults(this.type);
                                                    this.life = num20;
                                                    if (this.life > this.lifeMax)
                                                    {
                                                        this.life = this.lifeMax;
                                                    }
                                                    this.ai[0] = num21;
                                                    this.TargetClosest(true);
                                                    this.netUpdate = true;
                                                    this.whoAmI = i;
                                                }
                                                if (this.type == 14 && !Main.npc[(int)this.ai[0]].active)
                                                {
                                                    int num20 = this.life;
                                                    int i = this.whoAmI;
                                                    float num22 = this.ai[1];
                                                    this.SetDefaults(this.type);
                                                    this.life = num20;
                                                    if (this.life > this.lifeMax)
                                                    {
                                                        this.life = this.lifeMax;
                                                    }
                                                    this.ai[1] = num22;
                                                    this.TargetClosest(true);
                                                    this.netUpdate = true;
                                                    this.whoAmI = i;
                                                }
                                                if (this.life == 0)
                                                {
                                                    bool flag5 = true;
                                                    for (int i = 0; i < 1000; i++)
                                                    {
                                                        if (Main.npc[i].active)
                                                        {
                                                            if (Main.npc[i].type == 13 || Main.npc[i].type == 14 || Main.npc[i].type == 15)
                                                            {
                                                                flag5 = false;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    if (flag5)
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
                                        int num23 = (int)(this.position.X / 16f) - 1;
                                        int num24 = (int)((this.position.X + (float)this.width) / 16f) + 2;
                                        int num25 = (int)(this.position.Y / 16f) - 1;
                                        int num26 = (int)((this.position.Y + (float)this.height) / 16f) + 2;
                                        if (num23 < 0)
                                        {
                                            num23 = 0;
                                        }
                                        if (num24 > Main.maxTilesX)
                                        {
                                            num24 = Main.maxTilesX;
                                        }
                                        if (num25 < 0)
                                        {
                                            num25 = 0;
                                        }
                                        if (num26 > Main.maxTilesY)
                                        {
                                            num26 = Main.maxTilesY;
                                        }
                                        bool flag6 = false;
                                        for (int i = num23; i < num24; i++)
                                        {
                                            for (int j = num25; j < num26; j++)
                                            {
                                                if (Main.tile[i, j] != null && ((Main.tile[i, j].active && (Main.tileSolid[(int)Main.tile[i, j].type] || (Main.tileSolidTop[(int)Main.tile[i, j].type] && Main.tile[i, j].frameY == 0))) || Main.tile[i, j].liquid > 64))
                                                {
                                                    Vector2 vector4;
                                                    vector4.X = (float)(i * 16);
                                                    vector4.Y = (float)(j * 16);
                                                    if (this.position.X + (float)this.width > vector4.X && this.position.X < vector4.X + 16f && this.position.Y + (float)this.height > vector4.Y && this.position.Y < vector4.Y + 16f)
                                                    {
                                                        flag6 = true;
                                                        if (Main.rand.Next(40) == 0)
                                                        {
                                                            if (Main.tile[i, j].active)
                                                            {
                                                                WorldGen.KillTile(i, j, true, true, false);
                                                            }
                                                        }
                                                        if (Main.netMode != 1)
                                                        {
                                                            if (Main.tile[i, j].type == 2)
                                                            {
                                                                if (Main.tile[i, j - 1].type != 27)
                                                                {
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        float num9 = 8f;
                                        float num10 = 0.07f;
                                        if (this.type == 10)
                                        {
                                            num9 = 6f;
                                            num10 = 0.05f;
                                        }
                                        if (this.type == 13)
                                        {
                                            num9 = 11f;
                                            num10 = 0.08f;
                                        }
                                        Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                        float num11 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
                                        float num12 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector.Y;
                                        float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                        if (this.ai[1] > 0f)
                                        {
                                            num11 = Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - vector.X;
                                            num12 = Main.npc[(int)this.ai[1]].position.Y + (float)(Main.npc[(int)this.ai[1]].height / 2) - vector.Y;
                                            this.rotation = (float)Math.Atan2((double)num12, (double)num11) + 1.57f;
                                            num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                            num13 = (num13 - (float)this.width) / num13;
                                            num11 *= num13;
                                            num12 *= num13;
                                            this.velocity = default(Vector2);
                                            this.position.X = this.position.X + num11;
                                            this.position.Y = this.position.Y + num12;
                                        }
                                        else
                                        {
                                            if (!flag6)
                                            {
                                                this.TargetClosest(true);
                                                this.velocity.Y = this.velocity.Y + 0.11f;
                                                if (this.velocity.Y > num9)
                                                {
                                                    this.velocity.Y = num9;
                                                }
                                                if ((double)(Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) < (double)num9 * 0.4)
                                                {
                                                    if (this.velocity.X < 0f)
                                                    {
                                                        this.velocity.X = this.velocity.X - num10 * 1.1f;
                                                    }
                                                    else
                                                    {
                                                        this.velocity.X = this.velocity.X + num10 * 1.1f;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.velocity.Y == num9)
                                                    {
                                                        if (this.velocity.X < num11)
                                                        {
                                                            this.velocity.X = this.velocity.X + num10;
                                                        }
                                                        else
                                                        {
                                                            if (this.velocity.X > num11)
                                                            {
                                                                this.velocity.X = this.velocity.X - num10;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (this.velocity.Y > 4f)
                                                        {
                                                            if (this.velocity.X < 0f)
                                                            {
                                                                this.velocity.X = this.velocity.X + num10 * 0.9f;
                                                            }
                                                            else
                                                            {
                                                                this.velocity.X = this.velocity.X - num10 * 0.9f;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (this.soundDelay == 0)
                                                {
                                                    float num27 = num13 / 40f;
                                                    if (num27 < 10f)
                                                    {
                                                        num27 = 10f;
                                                    }
                                                    if (num27 > 20f)
                                                    {
                                                        num27 = 20f;
                                                    }
                                                    this.soundDelay = (int)num27;
                                                    //Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 1);
                                                }
                                                num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                float num28 = Math.Abs(num11);
                                                float num29 = Math.Abs(num12);
                                                num13 = num9 / num13;
                                                num11 *= num13;
                                                num12 *= num13;
                                                if ((this.velocity.X > 0f && num11 > 0f) || (this.velocity.X < 0f && num11 < 0f) || (this.velocity.Y > 0f && num12 > 0f) || (this.velocity.Y < 0f && num12 < 0f))
                                                {
                                                    if (this.velocity.X < num11)
                                                    {
                                                        this.velocity.X = this.velocity.X + num10;
                                                    }
                                                    else
                                                    {
                                                        if (this.velocity.X > num11)
                                                        {
                                                            this.velocity.X = this.velocity.X - num10;
                                                        }
                                                    }
                                                    if (this.velocity.Y < num12)
                                                    {
                                                        this.velocity.Y = this.velocity.Y + num10;
                                                    }
                                                    else
                                                    {
                                                        if (this.velocity.Y > num12)
                                                        {
                                                            this.velocity.Y = this.velocity.Y - num10;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (num28 > num29)
                                                    {
                                                        if (this.velocity.X < num11)
                                                        {
                                                            this.velocity.X = this.velocity.X + num10 * 1.1f;
                                                        }
                                                        else
                                                        {
                                                            if (this.velocity.X > num11)
                                                            {
                                                                this.velocity.X = this.velocity.X - num10 * 1.1f;
                                                            }
                                                        }
                                                        if ((double)(Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) < (double)num9 * 0.5)
                                                        {
                                                            if (this.velocity.Y > 0f)
                                                            {
                                                                this.velocity.Y = this.velocity.Y + num10;
                                                            }
                                                            else
                                                            {
                                                                this.velocity.Y = this.velocity.Y - num10;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (this.velocity.Y < num12)
                                                        {
                                                            this.velocity.Y = this.velocity.Y + num10 * 1.1f;
                                                        }
                                                        else
                                                        {
                                                            if (this.velocity.Y > num12)
                                                            {
                                                                this.velocity.Y = this.velocity.Y - num10 * 1.1f;
                                                            }
                                                        }
                                                        if ((double)(Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) < (double)num9 * 0.5)
                                                        {
                                                            if (this.velocity.X > 0f)
                                                            {
                                                                this.velocity.X = this.velocity.X + num10;
                                                            }
                                                            else
                                                            {
                                                                this.velocity.X = this.velocity.X - num10;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) + 1.57f;
                                        }
                                    }
                                    else
                                    {
                                        if (this.aiStyle == 7)
                                        {
                                            int num30 = (int)(this.position.X + (float)(this.width / 2)) / 16;
                                            int num31 = (int)(this.position.Y + (float)this.height + 1f) / 16;
                                            if (Main.netMode == 1 || !this.townNPC)
                                            {
                                                this.homeTileX = num30;
                                                this.homeTileY = num31;
                                            }
                                            if (this.type == 46 && this.target == 255)
                                            {
                                                this.TargetClosest(true);
                                            }
                                            bool flag7 = false;
                                            this.directionY = -1;
                                            if (this.direction == 0)
                                            {
                                                this.direction = 1;
                                            }
                                            for (int j = 0; j < 255; j++)
                                            {
                                                if (Main.player[j].active && Main.player[j].talkNPC == this.whoAmI)
                                                {
                                                    flag7 = true;
                                                    if (this.ai[0] != 0f)
                                                    {
                                                        this.netUpdate = true;
                                                    }
                                                    this.ai[0] = 0f;
                                                    this.ai[1] = 300f;
                                                    this.ai[2] = 100f;
                                                    if (Main.player[j].position.X + (float)(Main.player[j].width / 2) < this.position.X + (float)(this.width / 2))
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
                                            if (this.type == 37)
                                            {
                                                if (Main.netMode != 1)
                                                {
                                                    this.homeless = false;
                                                    this.homeTileX = Main.dungeonX;
                                                    this.homeTileY = Main.dungeonY;
                                                    if (NPC.downedBoss3)
                                                    {
                                                        this.ai[3] = 1f;
                                                        this.netUpdate = true;
                                                    }
                                                    if (!Main.dayTime && flag7)
                                                    {
                                                        if (this.ai[3] == 0f)
                                                        {
                                                            bool flag8 = true;
                                                            for (int i = 0; i < 1000; i++)
                                                            {
                                                                if (Main.npc[i].active && Main.npc[i].type == 35)
                                                                {
                                                                    flag8 = false;
                                                                    break;
                                                                }
                                                            }
                                                            if (flag8)
                                                            {
                                                                int num19 = NPC.NewNPC((int)this.position.X + this.width / 2, (int)this.position.Y + this.height / 2, 35, 0);
                                                                Main.npc[num19].netUpdate = true;
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
                                                }
                                            }
                                            if (Main.netMode != 1 && this.townNPC && !Main.dayTime && (num30 != this.homeTileX || num31 != this.homeTileY))
                                            {
                                                if (!this.homeless)
                                                {
                                                    bool flag9 = true;
                                                    for (int k = 0; k < 2; k++)
                                                    {
                                                        Rectangle rectangle = new Rectangle((int)(this.position.X + (float)(this.width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(this.position.Y + (float)(this.height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                                        if (k == 1)
                                                        {
                                                            rectangle = new Rectangle(this.homeTileX * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, this.homeTileY * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                                        }
                                                        for (int i = 0; i < 255; i++)
                                                        {
                                                            if (Main.player[i].active)
                                                            {
                                                                Rectangle rectangle2 = new Rectangle((int)Main.player[i].position.X, (int)Main.player[i].position.Y, Main.player[i].width, Main.player[i].height);
                                                                if (rectangle2.Intersects(rectangle))
                                                                {
                                                                    flag9 = false;
                                                                    break;
                                                                }
                                                            }
                                                            if (!flag9)
                                                            {
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    if (flag9)
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
                                            }
                                            if (this.ai[0] == 0f)
                                            {
                                                if (this.ai[2] > 0f)
                                                {
                                                    this.ai[2] -= 1f;
                                                }
                                                if (!Main.dayTime && !flag7)
                                                {
                                                    if (Main.netMode != 1)
                                                    {
                                                        if (num30 == this.homeTileX && num31 == this.homeTileY)
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
                                                            if (!flag7)
                                                            {
                                                                if (num30 > this.homeTileX)
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
                                                            if (this.type == 46)
                                                            {
                                                                this.ai[1] += (float)Main.rand.Next(200, 400);
                                                            }
                                                            this.ai[2] = 0f;
                                                            this.netUpdate = true;
                                                        }
                                                    }
                                                }
                                                if (Main.netMode != 1 && (Main.dayTime || (num30 == this.homeTileX && num31 == this.homeTileY)))
                                                {
                                                    if (num30 < this.homeTileX - 25 || num30 > this.homeTileX + 25)
                                                    {
                                                        if (this.ai[2] == 0f)
                                                        {
                                                            if (num30 < this.homeTileX - 50 && this.direction == -1)
                                                            {
                                                                this.direction = 1;
                                                                this.netUpdate = true;
                                                            }
                                                            else
                                                            {
                                                                if (num30 > this.homeTileX + 50 && this.direction == 1)
                                                                {
                                                                    this.direction = -1;
                                                                    this.netUpdate = true;
                                                                }
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
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (this.ai[0] == 1f)
                                                {
                                                    if (Main.netMode != 1 && !Main.dayTime && num30 == this.homeTileX && num31 == this.homeTileY)
                                                    {
                                                        this.ai[0] = 0f;
                                                        this.ai[1] = (float)(200 + Main.rand.Next(200));
                                                        this.ai[2] = 60f;
                                                        this.netUpdate = true;
                                                    }
                                                    else
                                                    {
                                                        if (Main.netMode != 1)
                                                        {
                                                            if (!this.homeless)
                                                            {
                                                                if (num30 < this.homeTileX - 35 || num30 > this.homeTileX + 35)
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
                                                            }
                                                        }
                                                        this.ai[1] -= 1f;
                                                        if (this.ai[1] <= 0f)
                                                        {
                                                            this.ai[0] = 0f;
                                                            this.ai[1] = (float)(300 + Main.rand.Next(300));
                                                            if (this.type == 46)
                                                            {
                                                                this.ai[1] -= (float)Main.rand.Next(100);
                                                            }
                                                            this.ai[2] = 60f;
                                                            this.netUpdate = true;
                                                        }
                                                        if (this.closeDoor)
                                                        {
                                                            if ((this.position.X + (float)(this.width / 2)) / 16f > (float)(this.doorX + 2) || (this.position.X + (float)(this.width / 2)) / 16f < (float)(this.doorX - 2))
                                                            {
                                                                bool flag10 = WorldGen.CloseDoor(this.doorX, this.doorY, false);
                                                                if (flag10)
                                                                {
                                                                    this.closeDoor = false;
                                                                    NetMessage.SendData(19, -1, -1, "", 1, (float)this.doorX, (float)this.doorY, (float)this.direction);
                                                                }
                                                                if ((this.position.X + (float)(this.width / 2)) / 16f > (float)(this.doorX + 4) || (this.position.X + (float)(this.width / 2)) / 16f < (float)(this.doorX - 4) || (this.position.Y + (float)(this.height / 2)) / 16f > (float)(this.doorY + 4) || (this.position.Y + (float)(this.height / 2)) / 16f < (float)(this.doorY - 4))
                                                                {
                                                                    this.closeDoor = false;
                                                                }
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
                                                            if (this.townNPC && Main.tile[num3, num4 - 2].active && Main.tile[num3, num4 - 2].type == 10 && (Main.rand.Next(10) == 0 || !Main.dayTime))
                                                            {
                                                                if (Main.netMode != 1)
                                                                {
                                                                    if (Program.properties.isNPCDoorOpenCancelled())
                                                                    {
                                                                        return;
                                                                    }
                                                                    bool flag4 = WorldGen.OpenDoor(num3, num4 - 2, this.direction);
                                                                    if (flag4)
                                                                    {
                                                                        this.closeDoor = true;
                                                                        this.doorX = num3;
                                                                        this.doorY = num4 - 2;
                                                                        NetMessage.SendData(19, -1, -1, "", 0, (float)num3, (float)(num4 - 2), (float)this.direction);
                                                                        this.netUpdate = true;
                                                                        this.ai[1] += 80f;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (WorldGen.OpenDoor(num3, num4 - 2, -this.direction))
                                                                        {
                                                                            this.closeDoor = true;
                                                                            this.doorX = num3;
                                                                            this.doorY = num4 - 2;
                                                                            NetMessage.SendData(19, -1, -1, "", 0, (float)num3, (float)(num4 - 2), (float)(-(float)this.direction));
                                                                            this.netUpdate = true;
                                                                            this.ai[1] += 80f;
                                                                        }
                                                                        else
                                                                        {
                                                                            this.direction *= -1;
                                                                            this.netUpdate = true;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if ((this.velocity.X < 0f && this.spriteDirection == -1) || (this.velocity.X > 0f && this.spriteDirection == 1))
                                                                {
                                                                    if (Main.tile[num3, num4 - 2].active && Main.tileSolid[(int)Main.tile[num3, num4 - 2].type] && !Main.tileSolidTop[(int)Main.tile[num3, num4 - 2].type])
                                                                    {
                                                                        if ((this.direction == 1 && !Collision.SolidTiles(num3 - 2, num3 - 1, num4 - 5, num4 - 1)) || (this.direction == -1 && !Collision.SolidTiles(num3 + 1, num3 + 2, num4 - 5, num4 - 1)))
                                                                        {
                                                                            if (!Collision.SolidTiles(num3, num3, num4 - 5, num4 - 3))
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
                                                                        if (Main.tile[num3, num4 - 1].active && Main.tileSolid[(int)Main.tile[num3, num4 - 1].type] && !Main.tileSolidTop[(int)Main.tile[num3, num4 - 1].type])
                                                                        {
                                                                            if ((this.direction == 1 && !Collision.SolidTiles(num3 - 2, num3 - 1, num4 - 4, num4 - 1)) || (this.direction == -1 && !Collision.SolidTiles(num3 + 1, num3 + 2, num4 - 4, num4 - 1)))
                                                                            {
                                                                                if (!Collision.SolidTiles(num3, num3, num4 - 4, num4 - 2))
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
                                                                            if (Main.tile[num3, num4].active && Main.tileSolid[(int)Main.tile[num3, num4].type] && !Main.tileSolidTop[(int)Main.tile[num3, num4].type])
                                                                            {
                                                                                if ((this.direction == 1 && !Collision.SolidTiles(num3 - 2, num3, num4 - 3, num4 - 1)) || (this.direction == -1 && !Collision.SolidTiles(num3, num3 + 2, num4 - 3, num4 - 1)))
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
                                                                        }
                                                                    }
                                                                    if (Main.tile[num3, num4 + 1] == null)
                                                                    {
                                                                        Main.tile[num3, num4 + 1] = new Tile();
                                                                    }
                                                                    if (Main.tile[num3 - this.direction, num4 + 1] == null)
                                                                    {
                                                                        Main.tile[num3 - this.direction, num4 + 1] = new Tile();
                                                                    }
                                                                    if (Main.tile[num3, num4 + 2] == null)
                                                                    {
                                                                        Main.tile[num3, num4 + 2] = new Tile();
                                                                    }
                                                                    if (Main.tile[num3 - this.direction, num4 + 2] == null)
                                                                    {
                                                                        Main.tile[num3 - this.direction, num4 + 2] = new Tile();
                                                                    }
                                                                    if (Main.tile[num3, num4 + 3] == null)
                                                                    {
                                                                        Main.tile[num3, num4 + 3] = new Tile();
                                                                    }
                                                                    if (Main.tile[num3 - this.direction, num4 + 3] == null)
                                                                    {
                                                                        Main.tile[num3 - this.direction, num4 + 3] = new Tile();
                                                                    }
                                                                    if (Main.tile[num3, num4 + 4] == null)
                                                                    {
                                                                        Main.tile[num3, num4 + 4] = new Tile();
                                                                    }
                                                                    if (Main.tile[num3 - this.direction, num4 + 4] == null)
                                                                    {
                                                                        Main.tile[num3 - this.direction, num4 + 4] = new Tile();
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num30 >= this.homeTileX - 35 && num30 <= this.homeTileX + 35 && (!Main.tile[num3, num4 + 1].active || !Main.tileSolid[(int)Main.tile[num3, num4 + 1].type]) && (!Main.tile[num3 - this.direction, num4 + 1].active || !Main.tileSolid[(int)Main.tile[num3 - this.direction, num4 + 1].type]) && (!Main.tile[num3, num4 + 2].active || !Main.tileSolid[(int)Main.tile[num3, num4 + 2].type]) && (!Main.tile[num3 - this.direction, num4 + 2].active || !Main.tileSolid[(int)Main.tile[num3 - this.direction, num4 + 2].type]) && (!Main.tile[num3, num4 + 3].active || !Main.tileSolid[(int)Main.tile[num3, num4 + 3].type]) && (!Main.tile[num3 - this.direction, num4 + 3].active || !Main.tileSolid[(int)Main.tile[num3 - this.direction, num4 + 3].type]) && (!Main.tile[num3, num4 + 4].active || !Main.tileSolid[(int)Main.tile[num3, num4 + 4].type]) && (!Main.tile[num3 - this.direction, num4 + 4].active || !Main.tileSolid[(int)Main.tile[num3 - this.direction, num4 + 4].type]))
                                                                        {
                                                                            if (this.type != 46)
                                                                            {
                                                                                this.direction *= -1;
                                                                                this.velocity.X = this.velocity.X * -1f;
                                                                                this.netUpdate = true;
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
                                                                }
                                                                if (this.velocity.Y < 0f && this.type == 46)
                                                                {
                                                                    this.velocity.Y = this.velocity.Y * 1.2f;
                                                                }
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
                                                this.TargetClosest(true);
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
                                                    for (int i = 0; i < 50; i++)
                                                    {
                                                        if (this.type == 29 || this.type == 45)
                                                        {
                                                            Vector2 arg_6B1B_0 = new Vector2(this.position.X, this.position.Y);
                                                            int arg_6B1B_1 = this.width;
                                                            int arg_6B1B_2 = this.height;
                                                            int arg_6B1B_3 = 27;
                                                            float arg_6B1B_4 = 0f;
                                                            float arg_6B1B_5 = 0f;
                                                            int arg_6B1B_6 = 100;
                                                            Color newColor = default(Color);
                                                            int num = Dust.NewDust(arg_6B1B_0, arg_6B1B_1, arg_6B1B_2, arg_6B1B_3, arg_6B1B_4, arg_6B1B_5, arg_6B1B_6, newColor, (float)Main.rand.Next(1, 3));
                                                            Dust expr_6B28 = Main.dust[num];
                                                            expr_6B28.velocity *= 3f;
                                                            if (Main.dust[num].scale > 1f)
                                                            {
                                                                Main.dust[num].noGravity = true;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (this.type == 32)
                                                            {
                                                                Vector2 arg_6BC6_0 = new Vector2(this.position.X, this.position.Y);
                                                                int arg_6BC6_1 = this.width;
                                                                int arg_6BC6_2 = this.height;
                                                                int arg_6BC6_3 = 29;
                                                                float arg_6BC6_4 = 0f;
                                                                float arg_6BC6_5 = 0f;
                                                                int arg_6BC6_6 = 100;
                                                                Color newColor = default(Color);
                                                                int num = Dust.NewDust(arg_6BC6_0, arg_6BC6_1, arg_6BC6_2, arg_6BC6_3, arg_6BC6_4, arg_6BC6_5, arg_6BC6_6, newColor, 2.5f);
                                                                Dust expr_6BD3 = Main.dust[num];
                                                                expr_6BD3.velocity *= 3f;
                                                                Main.dust[num].noGravity = true;
                                                            }
                                                            else
                                                            {
                                                                Vector2 arg_6C3C_0 = new Vector2(this.position.X, this.position.Y);
                                                                int arg_6C3C_1 = this.width;
                                                                int arg_6C3C_2 = this.height;
                                                                int arg_6C3C_3 = 6;
                                                                float arg_6C3C_4 = 0f;
                                                                float arg_6C3C_5 = 0f;
                                                                int arg_6C3C_6 = 100;
                                                                Color newColor = default(Color);
                                                                int num = Dust.NewDust(arg_6C3C_0, arg_6C3C_1, arg_6C3C_2, arg_6C3C_3, arg_6C3C_4, arg_6C3C_5, arg_6C3C_6, newColor, 2.5f);
                                                                Dust expr_6C49 = Main.dust[num];
                                                                expr_6C49.velocity *= 3f;
                                                                Main.dust[num].noGravity = true;
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
                                                    for (int i = 0; i < 50; i++)
                                                    {
                                                        if (this.type == 29 || this.type == 45)
                                                        {
                                                            Vector2 arg_6D9F_0 = new Vector2(this.position.X, this.position.Y);
                                                            int arg_6D9F_1 = this.width;
                                                            int arg_6D9F_2 = this.height;
                                                            int arg_6D9F_3 = 27;
                                                            float arg_6D9F_4 = 0f;
                                                            float arg_6D9F_5 = 0f;
                                                            int arg_6D9F_6 = 100;
                                                            Color newColor = default(Color);
                                                            int num = Dust.NewDust(arg_6D9F_0, arg_6D9F_1, arg_6D9F_2, arg_6D9F_3, arg_6D9F_4, arg_6D9F_5, arg_6D9F_6, newColor, (float)Main.rand.Next(1, 3));
                                                            Dust expr_6DAC = Main.dust[num];
                                                            expr_6DAC.velocity *= 3f;
                                                            if (Main.dust[num].scale > 1f)
                                                            {
                                                                Main.dust[num].noGravity = true;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (this.type == 32)
                                                            {
                                                                Vector2 arg_6E4A_0 = new Vector2(this.position.X, this.position.Y);
                                                                int arg_6E4A_1 = this.width;
                                                                int arg_6E4A_2 = this.height;
                                                                int arg_6E4A_3 = 29;
                                                                float arg_6E4A_4 = 0f;
                                                                float arg_6E4A_5 = 0f;
                                                                int arg_6E4A_6 = 100;
                                                                Color newColor = default(Color);
                                                                int num = Dust.NewDust(arg_6E4A_0, arg_6E4A_1, arg_6E4A_2, arg_6E4A_3, arg_6E4A_4, arg_6E4A_5, arg_6E4A_6, newColor, 2.5f);
                                                                Dust expr_6E57 = Main.dust[num];
                                                                expr_6E57.velocity *= 3f;
                                                                Main.dust[num].noGravity = true;
                                                            }
                                                            else
                                                            {
                                                                Vector2 arg_6EC0_0 = new Vector2(this.position.X, this.position.Y);
                                                                int arg_6EC0_1 = this.width;
                                                                int arg_6EC0_2 = this.height;
                                                                int arg_6EC0_3 = 6;
                                                                float arg_6EC0_4 = 0f;
                                                                float arg_6EC0_5 = 0f;
                                                                int arg_6EC0_6 = 100;
                                                                Color newColor = default(Color);
                                                                int num = Dust.NewDust(arg_6EC0_0, arg_6EC0_1, arg_6EC0_2, arg_6EC0_3, arg_6EC0_4, arg_6EC0_5, arg_6EC0_6, newColor, 2.5f);
                                                                Dust expr_6ECD = Main.dust[num];
                                                                expr_6ECD.velocity *= 3f;
                                                                Main.dust[num].noGravity = true;
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
                                                        int num32 = (int)Main.player[this.target].position.X / 16;
                                                        int num33 = (int)Main.player[this.target].position.Y / 16;
                                                        int num34 = (int)this.position.X / 16;
                                                        int num35 = (int)this.position.Y / 16;
                                                        int num36 = 20;
                                                        int num37 = 0;
                                                        bool flag11 = false;
                                                        if (Math.Abs(this.position.X - Main.player[this.target].position.X) + Math.Abs(this.position.Y - Main.player[this.target].position.Y) > 2000f)
                                                        {
                                                            num37 = 100;
                                                            flag11 = true;
                                                        }
                                                        while (!flag11 && num37 < 100)
                                                        {
                                                            num37++;
                                                            int num38 = Main.rand.Next(num32 - num36, num32 + num36);
                                                            int j = Main.rand.Next(num33 - num36, num33 + num36);
                                                            for (int l = j; l < num33 + num36; l++)
                                                            {
                                                                if (l < num33 - 4 || l > num33 + 4 || num38 < num32 - 4 || num38 > num32 + 4)
                                                                {
                                                                    if (l < num35 - 1 || l > num35 + 1 || num38 < num34 - 1 || num38 > num34 + 1)
                                                                    {
                                                                        if (Main.tile[num38, l].active)
                                                                        {
                                                                            bool flag12 = true;
                                                                            if (this.type == 32 && Main.tile[num38, l - 1].wall == 0)
                                                                            {
                                                                                flag12 = false;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (Main.tile[num38, l - 1].lava)
                                                                                {
                                                                                    flag12 = false;
                                                                                }
                                                                            }
                                                                            if (flag12 && Main.tileSolid[(int)Main.tile[num38, l].type])
                                                                            {
                                                                                if (!Collision.SolidTiles(num38 - 1, num38 + 1, l - 4, l - 1))
                                                                                {
                                                                                    this.ai[1] = 20f;
                                                                                    this.ai[2] = (float)num38;
                                                                                    this.ai[3] = (float)l;
                                                                                    flag11 = true;
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
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
                                                        Vector2 arg_7438_0 = new Vector2(this.position.X, this.position.Y + 2f);
                                                        int arg_7438_1 = this.width;
                                                        int arg_7438_2 = this.height;
                                                        int arg_7438_3 = 27;
                                                        float arg_7438_4 = this.velocity.X * 0.2f;
                                                        float arg_7438_5 = this.velocity.Y * 0.2f;
                                                        int arg_7438_6 = 100;
                                                        Color newColor = default(Color);
                                                        int num39 = Dust.NewDust(arg_7438_0, arg_7438_1, arg_7438_2, arg_7438_3, arg_7438_4, arg_7438_5, arg_7438_6, newColor, 1.5f);
                                                        Main.dust[num39].noGravity = true;
                                                        Dust expr_745A_cp_0 = Main.dust[num39];
                                                        expr_745A_cp_0.velocity.X = expr_745A_cp_0.velocity.X * 0.5f;
                                                        Main.dust[num39].velocity.Y = -2f;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.type == 32)
                                                    {
                                                        if (Main.rand.Next(2) == 0)
                                                        {
                                                            Vector2 arg_751D_0 = new Vector2(this.position.X, this.position.Y + 2f);
                                                            int arg_751D_1 = this.width;
                                                            int arg_751D_2 = this.height;
                                                            int arg_751D_3 = 29;
                                                            float arg_751D_4 = this.velocity.X * 0.2f;
                                                            float arg_751D_5 = this.velocity.Y * 0.2f;
                                                            int arg_751D_6 = 100;
                                                            Color newColor = default(Color);
                                                            int num39 = Dust.NewDust(arg_751D_0, arg_751D_1, arg_751D_2, arg_751D_3, arg_751D_4, arg_751D_5, arg_751D_6, newColor, 2f);
                                                            Main.dust[num39].noGravity = true;
                                                            Dust expr_753F_cp_0 = Main.dust[num39];
                                                            expr_753F_cp_0.velocity.X = expr_753F_cp_0.velocity.X * 1f;
                                                            Dust expr_755D_cp_0 = Main.dust[num39];
                                                            expr_755D_cp_0.velocity.Y = expr_755D_cp_0.velocity.Y * 1f;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Main.rand.Next(2) == 0)
                                                        {
                                                            Vector2 arg_75F2_0 = new Vector2(this.position.X, this.position.Y + 2f);
                                                            int arg_75F2_1 = this.width;
                                                            int arg_75F2_2 = this.height;
                                                            int arg_75F2_3 = 6;
                                                            float arg_75F2_4 = this.velocity.X * 0.2f;
                                                            float arg_75F2_5 = this.velocity.Y * 0.2f;
                                                            int arg_75F2_6 = 100;
                                                            Color newColor = default(Color);
                                                            int num39 = Dust.NewDust(arg_75F2_0, arg_75F2_1, arg_75F2_2, arg_75F2_3, arg_75F2_4, arg_75F2_5, arg_75F2_6, newColor, 2f);
                                                            Main.dust[num39].noGravity = true;
                                                            Dust expr_7614_cp_0 = Main.dust[num39];
                                                            expr_7614_cp_0.velocity.X = expr_7614_cp_0.velocity.X * 1f;
                                                            Dust expr_7632_cp_0 = Main.dust[num39];
                                                            expr_7632_cp_0.velocity.Y = expr_7632_cp_0.velocity.Y * 1f;
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
                                                        this.TargetClosest(true);
                                                        float num9 = 6f;
                                                        if (this.type == 30)
                                                        {
                                                            NPC.maxSpawns = 8;
                                                        }
                                                        Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                        float num11 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
                                                        float num12 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector.Y;
                                                        float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                        num13 = num9 / num13;
                                                        this.velocity.X = num11 * num13;
                                                        this.velocity.Y = num12 * num13;
                                                    }
                                                    if (this.timeLeft > 100)
                                                    {
                                                        this.timeLeft = 100;
                                                    }
                                                    for (int i = 0; i < 2; i++)
                                                    {
                                                        if (this.type == 30)
                                                        {
                                                            Vector2 arg_7825_0 = new Vector2(this.position.X, this.position.Y + 2f);
                                                            int arg_7825_1 = this.width;
                                                            int arg_7825_2 = this.height;
                                                            int arg_7825_3 = 27;
                                                            float arg_7825_4 = this.velocity.X * 0.2f;
                                                            float arg_7825_5 = this.velocity.Y * 0.2f;
                                                            int arg_7825_6 = 100;
                                                            Color newColor = default(Color);
                                                            int num = Dust.NewDust(arg_7825_0, arg_7825_1, arg_7825_2, arg_7825_3, arg_7825_4, arg_7825_5, arg_7825_6, newColor, 2f);
                                                            Main.dust[num].noGravity = true;
                                                            Dust expr_783F = Main.dust[num];
                                                            expr_783F.velocity *= 0.3f;
                                                            Dust expr_7860_cp_0 = Main.dust[num];
                                                            expr_7860_cp_0.velocity.X = expr_7860_cp_0.velocity.X - this.velocity.X * 0.2f;
                                                            Dust expr_7889_cp_0 = Main.dust[num];
                                                            expr_7889_cp_0.velocity.Y = expr_7889_cp_0.velocity.Y - this.velocity.Y * 0.2f;
                                                        }
                                                        else
                                                        {
                                                            if (this.type == 33)
                                                            {
                                                                Vector2 arg_7925_0 = new Vector2(this.position.X, this.position.Y + 2f);
                                                                int arg_7925_1 = this.width;
                                                                int arg_7925_2 = this.height;
                                                                int arg_7925_3 = 29;
                                                                float arg_7925_4 = this.velocity.X * 0.2f;
                                                                float arg_7925_5 = this.velocity.Y * 0.2f;
                                                                int arg_7925_6 = 100;
                                                                Color newColor = default(Color);
                                                                int num = Dust.NewDust(arg_7925_0, arg_7925_1, arg_7925_2, arg_7925_3, arg_7925_4, arg_7925_5, arg_7925_6, newColor, 2f);
                                                                Main.dust[num].noGravity = true;
                                                                Dust expr_7944_cp_0 = Main.dust[num];
                                                                expr_7944_cp_0.velocity.X = expr_7944_cp_0.velocity.X * 0.3f;
                                                                Dust expr_7961_cp_0 = Main.dust[num];
                                                                expr_7961_cp_0.velocity.Y = expr_7961_cp_0.velocity.Y * 0.3f;
                                                            }
                                                            else
                                                            {
                                                                Vector2 arg_79DA_0 = new Vector2(this.position.X, this.position.Y + 2f);
                                                                int arg_79DA_1 = this.width;
                                                                int arg_79DA_2 = this.height;
                                                                int arg_79DA_3 = 6;
                                                                float arg_79DA_4 = this.velocity.X * 0.2f;
                                                                float arg_79DA_5 = this.velocity.Y * 0.2f;
                                                                int arg_79DA_6 = 100;
                                                                Color newColor = default(Color);
                                                                int num = Dust.NewDust(arg_79DA_0, arg_79DA_1, arg_79DA_2, arg_79DA_3, arg_79DA_4, arg_79DA_5, arg_79DA_6, newColor, 2f);
                                                                Main.dust[num].noGravity = true;
                                                                Dust expr_79F9_cp_0 = Main.dust[num];
                                                                expr_79F9_cp_0.velocity.X = expr_79F9_cp_0.velocity.X * 0.3f;
                                                                Dust expr_7A16_cp_0 = Main.dust[num];
                                                                expr_7A16_cp_0.velocity.Y = expr_7A16_cp_0.velocity.Y * 0.3f;
                                                            }
                                                        }
                                                    }
                                                    this.rotation += 0.4f * (float)this.direction;
                                                }
                                                else
                                                {
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
                                                        this.TargetClosest(true);
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
                                                        Vector2 arg_8023_0 = new Vector2(this.position.X - this.velocity.X, this.position.Y - this.velocity.Y);
                                                        int arg_8023_1 = this.width;
                                                        int arg_8023_2 = this.height;
                                                        int arg_8023_3 = 6;
                                                        float arg_8023_4 = this.velocity.X * 0.2f;
                                                        float arg_8023_5 = this.velocity.Y * 0.2f;
                                                        int arg_8023_6 = 100;
                                                        Color newColor = default(Color);
                                                        int num = Dust.NewDust(arg_8023_0, arg_8023_1, arg_8023_2, arg_8023_3, arg_8023_4, arg_8023_5, arg_8023_6, newColor, 2f);
                                                        Main.dust[num].noGravity = true;
                                                        Main.dust[num].noLight = true;
                                                        Dust expr_804F_cp_0 = Main.dust[num];
                                                        expr_804F_cp_0.velocity.X = expr_804F_cp_0.velocity.X * 0.3f;
                                                        Dust expr_806C_cp_0 = Main.dust[num];
                                                        expr_806C_cp_0.velocity.Y = expr_806C_cp_0.velocity.Y * 0.3f;
                                                    }
                                                    else
                                                    {
                                                        if (this.aiStyle == 11)
                                                        {
                                                            if (this.ai[0] == 0f && Main.netMode != 1)
                                                            {
                                                                this.TargetClosest(true);
                                                                this.ai[0] = 1f;
                                                                int num19 = NPC.NewNPC((int)(this.position.X + (float)(this.width / 2)), (int)this.position.Y + this.height / 2, 36, this.whoAmI);
                                                                Main.npc[num19].ai[0] = -1f;
                                                                Main.npc[num19].ai[1] = (float)this.whoAmI;
                                                                Main.npc[num19].target = this.target;
                                                                Main.npc[num19].netUpdate = true;
                                                                num19 = NPC.NewNPC((int)(this.position.X + (float)(this.width / 2)), (int)this.position.Y + this.height / 2, 36, this.whoAmI);
                                                                Main.npc[num19].ai[0] = 1f;
                                                                Main.npc[num19].ai[1] = (float)this.whoAmI;
                                                                Main.npc[num19].ai[3] = 150f;
                                                                Main.npc[num19].target = this.target;
                                                                Main.npc[num19].netUpdate = true;
                                                            }
                                                            if (Main.player[this.target].dead || Math.Abs(this.position.X - Main.player[this.target].position.X) > 2000f || Math.Abs(this.position.Y - Main.player[this.target].position.Y) > 2000f)
                                                            {
                                                                this.TargetClosest(true);
                                                                if (Main.player[this.target].dead || Math.Abs(this.position.X - Main.player[this.target].position.X) > 2000f || Math.Abs(this.position.Y - Main.player[this.target].position.Y) > 2000f)
                                                                {
                                                                    this.ai[1] = 3f;
                                                                }
                                                            }
                                                            if (Main.dayTime && this.ai[1] != 3f)
                                                            {
                                                                if (this.ai[1] != 2f)
                                                                {
                                                                    this.ai[1] = 2f;
                                                                    //Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
                                                                }
                                                            }
                                                            if (this.ai[1] == 0f)
                                                            {
                                                                this.ai[2] += 1f;
                                                                if (this.ai[2] >= 800f)
                                                                {
                                                                    this.ai[2] = 0f;
                                                                    this.ai[1] = 1f;
                                                                    this.TargetClosest(true);
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
                                                                    Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                    float num11 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
                                                                    float num12 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector.Y;
                                                                    float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                                    num13 = 2.5f / num13;
                                                                    this.velocity.X = num11 * num13;
                                                                    this.velocity.Y = num12 * num13;
                                                                }
                                                                else
                                                                {
                                                                    if (this.ai[1] == 2f)
                                                                    {
                                                                        this.damage = 9999;
                                                                        this.defense = 9999;
                                                                        this.rotation += (float)this.direction * 0.3f;
                                                                        Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                        float num11 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
                                                                        float num12 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector.Y;
                                                                        float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                                        num13 = 8f / num13;
                                                                        this.velocity.X = num11 * num13;
                                                                        this.velocity.Y = num12 * num13;
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
                                                                Vector2 arg_8B00_0 = new Vector2(this.position.X + (float)(this.width / 2) - 15f - this.velocity.X * 5f, this.position.Y + (float)this.height - 2f);
                                                                int arg_8B00_1 = 30;
                                                                int arg_8B00_2 = 10;
                                                                int arg_8B00_3 = 5;
                                                                float arg_8B00_4 = -this.velocity.X * 0.2f;
                                                                float arg_8B00_5 = 3f;
                                                                int arg_8B00_6 = 0;
                                                                Color newColor = default(Color);
                                                                int num = Dust.NewDust(arg_8B00_0, arg_8B00_1, arg_8B00_2, arg_8B00_3, arg_8B00_4, arg_8B00_5, arg_8B00_6, newColor, 2f);
                                                                Main.dust[num].noGravity = true;
                                                                Dust expr_8B1F_cp_0 = Main.dust[num];
                                                                expr_8B1F_cp_0.velocity.X = expr_8B1F_cp_0.velocity.X * 1.3f;
                                                                Dust expr_8B3C_cp_0 = Main.dust[num];
                                                                expr_8B3C_cp_0.velocity.X = expr_8B3C_cp_0.velocity.X + this.velocity.X * 0.4f;
                                                                Dust expr_8B65_cp_0 = Main.dust[num];
                                                                expr_8B65_cp_0.velocity.Y = expr_8B65_cp_0.velocity.Y + (2f + this.velocity.Y);
                                                                for (int i = 0; i < 2; i++)
                                                                {
                                                                    Vector2 arg_8BDB_0 = new Vector2(this.position.X, this.position.Y + 120f);
                                                                    int arg_8BDB_1 = this.width;
                                                                    int arg_8BDB_2 = 60;
                                                                    int arg_8BDB_3 = 5;
                                                                    float arg_8BDB_4 = this.velocity.X;
                                                                    float arg_8BDB_5 = this.velocity.Y;
                                                                    int arg_8BDB_6 = 0;
                                                                    newColor = default(Color);
                                                                    num = Dust.NewDust(arg_8BDB_0, arg_8BDB_1, arg_8BDB_2, arg_8BDB_3, arg_8BDB_4, arg_8BDB_5, arg_8BDB_6, newColor, 2f);
                                                                    Main.dust[num].noGravity = true;
                                                                    Dust expr_8BF5 = Main.dust[num];
                                                                    expr_8BF5.velocity -= this.velocity;
                                                                    Dust expr_8C17_cp_0 = Main.dust[num];
                                                                    expr_8C17_cp_0.velocity.Y = expr_8C17_cp_0.velocity.Y + 5f;
                                                                }
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
                                                                    if (Main.npc[(int)this.ai[1]].ai[1] == 3f)
                                                                    {
                                                                        if (this.timeLeft > 10)
                                                                        {
                                                                            this.timeLeft = 10;
                                                                        }
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
                                                                    Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                    float num11 = Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 200f * this.ai[0] - vector.X;
                                                                    float num12 = Main.npc[(int)this.ai[1]].position.Y + 230f - vector.Y;
                                                                    float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                                    this.rotation = (float)Math.Atan2((double)num12, (double)num11) + 1.57f;
                                                                }
                                                                else
                                                                {
                                                                    if (this.ai[2] == 1f)
                                                                    {
                                                                        Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                        float num11 = Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 200f * this.ai[0] - vector.X;
                                                                        float num12 = Main.npc[(int)this.ai[1]].position.Y + 230f - vector.Y;
                                                                        float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                                        this.rotation = (float)Math.Atan2((double)num12, (double)num11) + 1.57f;
                                                                        this.velocity.X = this.velocity.X * 0.95f;
                                                                        this.velocity.Y = this.velocity.Y - 0.1f;
                                                                        if (this.velocity.Y < -8f)
                                                                        {
                                                                            this.velocity.Y = -8f;
                                                                        }
                                                                        if (this.position.Y < Main.npc[(int)this.ai[1]].position.Y - 200f)
                                                                        {
                                                                            this.TargetClosest(true);
                                                                            this.ai[2] = 2f;
                                                                            vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                            num11 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
                                                                            num12 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector.Y;
                                                                            num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                                            num13 = 20f / num13;
                                                                            this.velocity.X = num11 * num13;
                                                                            this.velocity.Y = num12 * num13;
                                                                            this.netUpdate = true;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (this.ai[2] == 2f)
                                                                        {
                                                                            if (this.position.Y > Main.player[this.target].position.Y || this.velocity.Y < 0f)
                                                                            {
                                                                                this.ai[2] = 3f;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (this.ai[2] == 4f)
                                                                            {
                                                                                Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                                float num11 = Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 200f * this.ai[0] - vector.X;
                                                                                float num12 = Main.npc[(int)this.ai[1]].position.Y + 230f - vector.Y;
                                                                                float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                                                this.rotation = (float)Math.Atan2((double)num12, (double)num11) + 1.57f;
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
                                                                                    this.TargetClosest(true);
                                                                                    this.ai[2] = 5f;
                                                                                    vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                                    num11 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - vector.X;
                                                                                    num12 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - vector.Y;
                                                                                    num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                                                    num13 = 20f / num13;
                                                                                    this.velocity.X = num11 * num13;
                                                                                    this.velocity.Y = num12 * num13;
                                                                                    this.netUpdate = true;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (this.ai[2] == 5f)
                                                                                {
                                                                                    if ((this.velocity.X > 0f && this.position.X + (float)(this.width / 2) > Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2)) || (this.velocity.X < 0f && this.position.X + (float)(this.width / 2) < Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2)))
                                                                                    {
                                                                                        this.ai[2] = 0f;
                                                                                    }
                                                                                }
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
                                                                    }
                                                                    else
                                                                    {
                                                                        this.TargetClosest(true);
                                                                        float num10 = 0.05f;
                                                                        Vector2 vector = new Vector2(this.ai[0] * 16f + 8f, this.ai[1] * 16f + 8f);
                                                                        float num11 = Main.player[this.target].position.X + (float)(Main.player[this.target].width / 2) - (float)(this.width / 2) - vector.X;
                                                                        float num12 = Main.player[this.target].position.Y + (float)(Main.player[this.target].height / 2) - (float)(this.height / 2) - vector.Y;
                                                                        float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                                        if (num13 > 150f)
                                                                        {
                                                                            num13 = 150f / num13;
                                                                            num11 *= num13;
                                                                            num12 *= num13;
                                                                        }
                                                                        if (this.position.X < this.ai[0] * 16f + 8f + num11)
                                                                        {
                                                                            this.velocity.X = this.velocity.X + num10;
                                                                            if (this.velocity.X < 0f && num11 > 0f)
                                                                            {
                                                                                this.velocity.X = this.velocity.X + num10 * 2f;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (this.position.X > this.ai[0] * 16f + 8f + num11)
                                                                            {
                                                                                this.velocity.X = this.velocity.X - num10;
                                                                                if (this.velocity.X > 0f && num11 < 0f)
                                                                                {
                                                                                    this.velocity.X = this.velocity.X - num10 * 2f;
                                                                                }
                                                                            }
                                                                        }
                                                                        if (this.position.Y < this.ai[1] * 16f + 8f + num12)
                                                                        {
                                                                            this.velocity.Y = this.velocity.Y + num10;
                                                                            if (this.velocity.Y < 0f && num12 > 0f)
                                                                            {
                                                                                this.velocity.Y = this.velocity.Y + num10 * 2f;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (this.position.Y > this.ai[1] * 16f + 8f + num12)
                                                                            {
                                                                                this.velocity.Y = this.velocity.Y - num10;
                                                                                if (this.velocity.Y > 0f && num12 < 0f)
                                                                                {
                                                                                    this.velocity.Y = this.velocity.Y - num10 * 2f;
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
                                                                        if (num11 > 0f)
                                                                        {
                                                                            this.spriteDirection = 1;
                                                                            this.rotation = (float)Math.Atan2((double)num12, (double)num11);
                                                                        }
                                                                        if (num11 < 0f)
                                                                        {
                                                                            this.spriteDirection = -1;
                                                                            this.rotation = (float)Math.Atan2((double)num12, (double)num11) + 3.14f;
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
                                                                else
                                                                {
                                                                    if (this.aiStyle == 14)
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
                                                                        this.TargetClosest(true);
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
                                                                        if (this.type == 49)
                                                                        {
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
                                                                        }
                                                                        if (Main.netMode != 1 && this.type == 48)
                                                                        {
                                                                            this.ai[0] += 1f;
                                                                            if (this.ai[0] == 30f || this.ai[0] == 60f || this.ai[0] == 90f)
                                                                            {
                                                                                float num40 = 6f;
                                                                                Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                                float num11 = Main.player[this.target].position.X + (float)Main.player[this.target].width * 0.5f - vector.X + (float)Main.rand.Next(-100, 101);
                                                                                float num12 = Main.player[this.target].position.Y + (float)Main.player[this.target].height * 0.5f - vector.Y + (float)Main.rand.Next(-100, 101);
                                                                                float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                                                                                num13 = num40 / num13;
                                                                                num11 *= num13;
                                                                                num12 *= num13;
                                                                                int num41 = 15;
                                                                                int num42 = Projectile.NewProjectile(vector.X, vector.Y, num11, num12, 38, num41, 0f, Main.myPlayer);
                                                                                Main.projectile[num42].timeLeft = 300;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (this.ai[0] >= (float)(400 + Main.rand.Next(400)))
                                                                                {
                                                                                    this.ai[0] = 0f;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (this.aiStyle == 15)
                                                                        {
                                                                            this.aiAction = 0;
                                                                            if (this.ai[3] == 0f && this.life > 0)
                                                                            {
                                                                                this.ai[3] = (float)this.lifeMax;
                                                                            }
                                                                            if (this.ai[2] == 0f)
                                                                            {
                                                                                this.ai[0] = -100f;
                                                                                this.ai[2] = 1f;
                                                                                this.TargetClosest(true);
                                                                            }
                                                                            if (this.velocity.Y == 0f)
                                                                            {
                                                                                this.velocity.X = this.velocity.X * 0.8f;
                                                                                if ((double)this.velocity.X > -0.1 && (double)this.velocity.X < 0.1)
                                                                                {
                                                                                    this.velocity.X = 0f;
                                                                                }
                                                                                this.ai[0] += 2f;
                                                                                if ((double)this.life < (double)this.lifeMax * 0.8)
                                                                                {
                                                                                    this.ai[0] += 1f;
                                                                                }
                                                                                if ((double)this.life < (double)this.lifeMax * 0.6)
                                                                                {
                                                                                    this.ai[0] += 1f;
                                                                                }
                                                                                if ((double)this.life < (double)this.lifeMax * 0.4)
                                                                                {
                                                                                    this.ai[0] += 2f;
                                                                                }
                                                                                if ((double)this.life < (double)this.lifeMax * 0.2)
                                                                                {
                                                                                    this.ai[0] += 3f;
                                                                                }
                                                                                if ((double)this.life < (double)this.lifeMax * 0.1)
                                                                                {
                                                                                    this.ai[0] += 4f;
                                                                                }
                                                                                if (this.ai[0] >= 0f)
                                                                                {
                                                                                    this.netUpdate = true;
                                                                                    this.TargetClosest(true);
                                                                                    if (this.ai[1] == 3f)
                                                                                    {
                                                                                        this.velocity.Y = -13f;
                                                                                        this.velocity.X = this.velocity.X + 3.5f * (float)this.direction;
                                                                                        this.ai[0] = -200f;
                                                                                        this.ai[1] = 0f;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (this.ai[1] == 2f)
                                                                                        {
                                                                                            this.velocity.Y = -6f;
                                                                                            this.velocity.X = this.velocity.X + 4.5f * (float)this.direction;
                                                                                            this.ai[0] = -120f;
                                                                                            this.ai[1] += 1f;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            this.velocity.Y = -8f;
                                                                                            this.velocity.X = this.velocity.X + 4f * (float)this.direction;
                                                                                            this.ai[0] = -120f;
                                                                                            this.ai[1] += 1f;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (this.ai[0] >= -30f)
                                                                                    {
                                                                                        this.aiAction = 1;
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (this.target < 255)
                                                                                {
                                                                                    if ((this.direction == 1 && this.velocity.X < 3f) || (this.direction == -1 && this.velocity.X > -3f))
                                                                                    {
                                                                                        if ((this.direction == -1 && (double)this.velocity.X < 0.1) || (this.direction == 1 && (double)this.velocity.X > -0.1))
                                                                                        {
                                                                                            this.velocity.X = this.velocity.X + 0.2f * (float)this.direction;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            this.velocity.X = this.velocity.X * 0.93f;
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            int num = Dust.NewDust(this.position, this.width, this.height, 4, this.velocity.X, this.velocity.Y, 255, new Color(0, 80, 255, 80), this.scale * 1.2f);
                                                                            Main.dust[num].noGravity = true;
                                                                            Dust expr_B2AF = Main.dust[num];
                                                                            expr_B2AF.velocity *= 0.5f;
                                                                            if (this.life > 0)
                                                                            {
                                                                                float num43 = (float)this.life / (float)this.lifeMax;
                                                                                num43 = num43 * 0.5f + 0.75f;
                                                                                if (num43 != this.scale)
                                                                                {
                                                                                    this.position.X = this.position.X + (float)(this.width / 2);
                                                                                    this.position.Y = this.position.Y + (float)this.height;
                                                                                    this.scale = num43;
                                                                                    this.width = (int)(98f * this.scale);
                                                                                    this.height = (int)(92f * this.scale);
                                                                                    this.position.X = this.position.X - (float)(this.width / 2);
                                                                                    this.position.Y = this.position.Y - (float)this.height;
                                                                                }
                                                                                if (Main.netMode != 1)
                                                                                {
                                                                                    int num44 = (int)((double)this.lifeMax * 0.05);
                                                                                    if ((float)(this.life + num44) < this.ai[3])
                                                                                    {
                                                                                        this.ai[3] = (float)this.life;
                                                                                        int num45 = Main.rand.Next(1, 4);
                                                                                        for (int i = 0; i < num45; i++)
                                                                                        {
                                                                                            int x = (int)(this.position.X + (float)Main.rand.Next(this.width - 32));
                                                                                            int y = (int)(this.position.Y + (float)Main.rand.Next(this.height - 32));
                                                                                            int num19 = NPC.NewNPC(x, y, 1, 0);
                                                                                            Main.npc[num19].SetDefaults(1);
                                                                                            Main.npc[num19].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                                                                                            Main.npc[num19].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                                                                                            Main.npc[num19].ai[1] = (float)Main.rand.Next(3);
                                                                                            if (Main.netMode == 2 && num19 < 1000)
                                                                                            {
                                                                                                NetMessage.SendData(23, -1, -1, "", num19, 0f, 0f, 0f);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (this.aiStyle == 16)
                                                                            {
                                                                                if (this.direction == 0)
                                                                                {
                                                                                    this.TargetClosest(true);
                                                                                }
                                                                                if (this.wet)
                                                                                {
                                                                                    if (this.collideX)
                                                                                    {
                                                                                        this.velocity.X = this.velocity.X * -1f;
                                                                                        this.direction *= -1;
                                                                                    }
                                                                                    if (this.collideY)
                                                                                    {
                                                                                        if (this.velocity.Y > 0f)
                                                                                        {
                                                                                            this.velocity.Y = Math.Abs(this.velocity.Y) * -1f;
                                                                                            this.directionY = -1;
                                                                                            this.ai[0] = -1f;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (this.velocity.Y < 0f)
                                                                                            {
                                                                                                this.velocity.Y = Math.Abs(this.velocity.Y);
                                                                                                this.directionY = 1;
                                                                                                this.ai[0] = 1f;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    bool flag13 = false;
                                                                                    if (!this.friendly)
                                                                                    {
                                                                                        this.TargetClosest(false);
                                                                                        if (Main.player[this.target].wet && !Main.player[this.target].dead)
                                                                                        {
                                                                                            flag13 = true;
                                                                                        }
                                                                                    }
                                                                                    if (flag13)
                                                                                    {
                                                                                        this.TargetClosest(true);
                                                                                        this.velocity.X = this.velocity.X + (float)this.direction * 0.1f;
                                                                                        this.velocity.Y = this.velocity.Y + (float)this.directionY * 0.1f;
                                                                                        if (this.velocity.X > 3f)
                                                                                        {
                                                                                            this.velocity.X = 3f;
                                                                                        }
                                                                                        if (this.velocity.X < -3f)
                                                                                        {
                                                                                            this.velocity.X = -3f;
                                                                                        }
                                                                                        if (this.velocity.Y > 2f)
                                                                                        {
                                                                                            this.velocity.Y = 2f;
                                                                                        }
                                                                                        if (this.velocity.Y < -2f)
                                                                                        {
                                                                                            this.velocity.Y = -2f;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        this.velocity.X = this.velocity.X + (float)this.direction * 0.1f;
                                                                                        if (this.velocity.X < -1f || this.velocity.X > 1f)
                                                                                        {
                                                                                            this.velocity.X = this.velocity.X * 0.95f;
                                                                                        }
                                                                                        if (this.ai[0] == -1f)
                                                                                        {
                                                                                            this.velocity.Y = this.velocity.Y - 0.01f;
                                                                                            if ((double)this.velocity.Y < -0.3)
                                                                                            {
                                                                                                this.ai[0] = 1f;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            this.velocity.Y = this.velocity.Y + 0.01f;
                                                                                            if ((double)this.velocity.Y > 0.3)
                                                                                            {
                                                                                                this.ai[0] = -1f;
                                                                                            }
                                                                                        }
                                                                                        int i = (int)(this.position.X + (float)(this.width / 2)) / 16;
                                                                                        int j = (int)(this.position.Y + (float)(this.height / 2)) / 16;
                                                                                        if (Main.tile[i, j - 1] == null)
                                                                                        {
                                                                                            Main.tile[i, j - 1] = new Tile();
                                                                                        }
                                                                                        if (Main.tile[i, j + 1] == null)
                                                                                        {
                                                                                            Main.tile[i, j + 1] = new Tile();
                                                                                        }
                                                                                        if (Main.tile[i, j + 2] == null)
                                                                                        {
                                                                                            Main.tile[i, j + 2] = new Tile();
                                                                                        }
                                                                                        if (Main.tile[i, j - 1].liquid > 128)
                                                                                        {
                                                                                            if (Main.tile[i, j + 1].active)
                                                                                            {
                                                                                                this.ai[0] = -1f;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Main.tile[i, j + 2].active)
                                                                                                {
                                                                                                    this.ai[0] = -1f;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        if ((double)this.velocity.Y > 0.4 || (double)this.velocity.Y < -0.4)
                                                                                        {
                                                                                            this.velocity.Y = this.velocity.Y * 0.95f;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (this.velocity.Y == 0f)
                                                                                    {
                                                                                        if (Main.netMode != 1)
                                                                                        {
                                                                                            this.velocity.Y = (float)Main.rand.Next(-50, -20) * 0.1f;
                                                                                            this.velocity.X = (float)Main.rand.Next(-20, 20) * 0.1f;
                                                                                            this.netUpdate = true;
                                                                                        }
                                                                                    }
                                                                                    this.velocity.Y = this.velocity.Y + 0.3f;
                                                                                    if (this.velocity.Y > 10f)
                                                                                    {
                                                                                        this.velocity.Y = 10f;
                                                                                    }
                                                                                    this.ai[0] = 1f;
                                                                                }
                                                                                this.rotation = this.velocity.Y * (float)this.direction * 0.1f;
                                                                                if ((double)this.rotation < -0.2)
                                                                                {
                                                                                    this.rotation = -0.2f;
                                                                                }
                                                                                if ((double)this.rotation > 0.2)
                                                                                {
                                                                                    this.rotation = 0.2f;
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
            //if (!Main.dedServ)
            //{
            //    num = Main.npcTexture[this.type].Height / Main.npcFrameCount[this.type];
            //}
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
            if (this.type == 50)
            {
                if (this.velocity.Y != 0f)
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
            if (this.type == 55 || this.type == 57 || this.type == 58)
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
            if (this.type == 48 || this.type == 49 || this.type == 51)
            {
                if (this.velocity.X > 0f)
                {
                    this.spriteDirection = 1;
                }
                if (this.velocity.X < 0f)
                {
                    this.spriteDirection = -1;
                }
                this.rotation = this.velocity.X * 0.1f;
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
            if (this.type == 42)
            {
                if (this.velocity.X > 0f)
                {
                    this.spriteDirection = 1;
                }
                if (this.velocity.X < 0f)
                {
                    this.spriteDirection = -1;
                }
                this.rotation = this.velocity.X * 0.1f;
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
            if (this.type == 43 || this.type == 56)
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
            if (this.type == 17 || this.type == 18 || this.type == 19 || this.type == 20 || this.type == 22 || this.type == 38 || this.type == 26 || this.type == 27 || this.type == 28 || this.type == 31 || this.type == 21 || this.type == 44 || this.type == 54 || this.type == 37)
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
                    if (this.type == 21 || this.type == 31 || this.type == 44)
                    {
                        this.frame.Y = 0;
                    }
                }
            }
            else
            {
                if (this.type == 3 || this.type == 52 || this.type == 53)
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
                    if (this.type == 46 || this.type == 47)
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
                                this.frameCounter += (double)(Math.Abs(this.velocity.X) * 1f);
                                this.frameCounter += 1.0;
                                if (this.frameCounter > 6.0)
                                {
                                    this.frame.Y = this.frame.Y + num;
                                    this.frameCounter = 0.0;
                                }
                                if (this.frame.Y / num >= Main.npcFrameCount[this.type])
                                {
                                    this.frame.Y = 0;
                                }
                            }
                        }
                        else
                        {
                            if (this.velocity.Y < 0f)
                            {
                                this.frameCounter = 0.0;
                                this.frame.Y = num * 4;
                            }
                            else
                            {
                                if (this.velocity.Y > 0f)
                                {
                                    this.frameCounter = 0.0;
                                    this.frame.Y = num * 6;
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
        public void TargetClosest(bool faceTarget = true)
        {
            float num = -1f;
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                {
                    if (num == -1f || Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - this.position.X + (float)(this.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - this.position.Y + (float)(this.height / 2)) < num)
                    {
                        num = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - this.position.X + (float)(this.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - this.position.Y + (float)(this.height / 2));
                        this.target = i;
                    }
                }
            }
            if (this.target < 0 || this.target >= 255)
            {
                this.target = 0;
            }
            this.targetRect = new Rectangle((int)Main.player[this.target].position.X, (int)Main.player[this.target].position.Y, Main.player[this.target].width, Main.player[this.target].height);
            if (faceTarget)
            {
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
                if (this.type != 8 && this.type != 9 && this.type != 11 && this.type != 12 && this.type != 14 && this.type != 15 && this.type != 40 && this.type != 41)
                {
                    if (this.townNPC)
                    {
                        if ((double)this.position.Y < Main.worldSurface * 18.0)
                        {
                            Rectangle rectangle = new Rectangle((int)(this.position.X + (float)(this.width / 2) - (float)NPC.townRangeX), (int)(this.position.Y + (float)(this.height / 2) - (float)NPC.townRangeY), NPC.townRangeX * 2, NPC.townRangeY * 2);
                            for (int i = 0; i < 255; i++)
                            {
                                if (Main.player[i].active)
                                {
                                    if (rectangle.Intersects(new Rectangle((int)Main.player[i].position.X, (int)Main.player[i].position.Y, Main.player[i].width, Main.player[i].height)))
                                    {
                                        Main.player[i].townNPCs++;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        bool flag = false;
                        Rectangle rectangle2 = new Rectangle((int)(this.position.X + (float)(this.width / 2) - (float)NPC.activeRangeX), (int)(this.position.Y + (float)(this.height / 2) - (float)NPC.activeRangeY), NPC.activeRangeX * 2, NPC.activeRangeY * 2);
                        Rectangle rectangle3 = new Rectangle((int)((double)(this.position.X + (float)(this.width / 2)) - (double)NPC.sWidth * 0.5 - (double)this.width), (int)((double)(this.position.Y + (float)(this.height / 2)) - (double)NPC.sHeight * 0.5 - (double)this.height), NPC.sWidth + this.width * 2, NPC.sHeight + this.height * 2);
                        for (int i = 0; i < 255; i++)
                        {
                            if (Main.player[i].active)
                            {
                                if (rectangle2.Intersects(new Rectangle((int)Main.player[i].position.X, (int)Main.player[i].position.Y, Main.player[i].width, Main.player[i].height)))
                                {
                                    flag = true;
                                    if (this.type != 25 && this.type != 30 && this.type != 33)
                                    {
                                        Main.player[i].activeNPCs++;
                                    }
                                }
                                if (rectangle3.Intersects(new Rectangle((int)Main.player[i].position.X, (int)Main.player[i].position.Y, Main.player[i].width, Main.player[i].height)))
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
                            NPC.noSpawnCycle = true;
                            this.active = false;
                            if (Main.netMode == 2)
                            {
                                this.life = 0;
                                NetMessage.SendData(23, -1, -1, "", this.whoAmI, 0f, 0f, 0f);
                            }
                        }
                    }
                }
            }
        }
        public static void SpawnNPC()
        {
            if (!Main.stopSpawns)
            {
                if (NPC.noSpawnCycle)
                {
                    NPC.noSpawnCycle = false;
                }
                else
                {
                    bool flag = false;
                    bool flag2 = false;
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
                    for (int i = 0; i < 255; i++)
                    {
                        if (Main.player[i].active && !Main.player[i].dead)
                        {
                            bool flag3 = false;
                            bool flag4 = false;
                            if (Main.player[i].active && Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0)
                            {
                                if ((double)Main.player[i].position.Y < Main.worldSurface * 16.0 + (double)NPC.sHeight)
                                {
                                    int num4 = 3000;
                                    if ((double)Main.player[i].position.X > Main.invasionX * 16.0 - (double)num4 && (double)Main.player[i].position.X < Main.invasionX * 16.0 + (double)num4)
                                    {
                                        flag3 = true;
                                    }
                                }
                            }
                            flag = false;
                            NPC.spawnRate = NPC.defaultSpawnRate;
                            NPC.maxSpawns = NPC.defaultMaxSpawns;
                            if (Main.player[i].position.Y > (float)((Main.maxTilesY - 200) * 16))
                            {
                                NPC.spawnRate = (int)((float)NPC.spawnRate * 0.9f);
                                NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.1f);
                            }
                            else
                            {
                                if ((double)Main.player[i].position.Y > Main.rockLayer * 16.0 + (double)NPC.sHeight)
                                {
                                    NPC.spawnRate = (int)((double)NPC.spawnRate * 0.4);
                                    NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.9f);
                                }
                                else
                                {
                                    if ((double)Main.player[i].position.Y > Main.worldSurface * 16.0 + (double)NPC.sHeight)
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
                            if (Main.player[i].zoneDungeon)
                            {
                                NPC.spawnRate = (int)((double)NPC.defaultSpawnRate * 0.15);
                                NPC.maxSpawns = NPC.defaultMaxSpawns * 2;
                            }
                            else
                            {
                                if (Main.player[i].zoneEvil)
                                {
                                    NPC.spawnRate = (int)((double)NPC.spawnRate * 0.4);
                                    NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.6f);
                                }
                                else
                                {
                                    if (Main.player[i].zoneMeteor)
                                    {
                                        NPC.spawnRate = (int)((double)NPC.spawnRate * 0.5);
                                    }
                                    else
                                    {
                                        if (Main.player[i].zoneJungle)
                                        {
                                            NPC.spawnRate = (int)((double)NPC.spawnRate * 0.3);
                                            NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.6f);
                                        }
                                    }
                                }
                            }
                            if ((double)Main.player[i].activeNPCs < (double)NPC.maxSpawns * 0.2)
                            {
                                NPC.spawnRate = (int)((float)NPC.spawnRate * 0.6f);
                            }
                            else
                            {
                                if ((double)Main.player[i].activeNPCs < (double)NPC.maxSpawns * 0.4)
                                {
                                    NPC.spawnRate = (int)((float)NPC.spawnRate * 0.7f);
                                }
                                else
                                {
                                    if ((double)Main.player[i].activeNPCs < (double)NPC.maxSpawns * 0.6)
                                    {
                                        NPC.spawnRate = (int)((float)NPC.spawnRate * 0.8f);
                                    }
                                    else
                                    {
                                        if ((double)Main.player[i].activeNPCs < (double)NPC.maxSpawns * 0.8)
                                        {
                                            NPC.spawnRate = (int)((float)NPC.spawnRate * 0.9f);
                                        }
                                    }
                                }
                            }
                            if ((double)(Main.player[i].position.Y * 16f) > (Main.worldSurface + Main.rockLayer) / 2.0 || Main.player[i].zoneEvil)
                            {
                                if ((double)Main.player[i].activeNPCs < (double)NPC.maxSpawns * 0.2)
                                {
                                    NPC.spawnRate = (int)((float)NPC.spawnRate * 0.7f);
                                }
                                else
                                {
                                    if ((double)Main.player[i].activeNPCs < (double)NPC.maxSpawns * 0.4)
                                    {
                                        NPC.spawnRate = (int)((float)NPC.spawnRate * 0.9f);
                                    }
                                }
                            }
                            if ((double)NPC.spawnRate < (double)NPC.defaultSpawnRate * 0.1)
                            {
                                NPC.spawnRate = (int)((double)NPC.defaultSpawnRate * 0.1);
                            }
                            if (NPC.maxSpawns > NPC.defaultMaxSpawns * 3)
                            {
                                NPC.maxSpawns = NPC.defaultMaxSpawns * 3;
                            }
                            if (Main.player[i].inventory[Main.player[i].selectedItem].type == 148)
                            {
                                NPC.spawnRate = (int)((double)NPC.spawnRate * 0.75);
                                NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.5f);
                            }
                            if (flag3)
                            {
                                NPC.maxSpawns = (int)((double)NPC.defaultMaxSpawns * (1.0 + 0.4 * (double)num3));
                                NPC.spawnRate = 30;
                            }
                            bool flag5 = false;
                            if (!flag3 && (!Main.bloodMoon || Main.dayTime) && !Main.player[i].zoneDungeon && !Main.player[i].zoneEvil && !Main.player[i].zoneMeteor)
                            {
                                if (Main.player[i].townNPCs == 1)
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
                                    if (Main.player[i].townNPCs == 2)
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
                                        if (Main.player[i].townNPCs >= 3)
                                        {
                                            flag5 = true;
                                            NPC.maxSpawns = (int)((double)((float)NPC.maxSpawns) * 0.6);
                                        }
                                    }
                                }
                            }
                            if (Main.alwaysSpawn > 0)
                            {
                                NPC.spawnRate = 1;
                            }
                            if (Main.player[i].active && !Main.player[i].dead && Main.player[i].activeNPCs < NPC.maxSpawns && Main.rand.Next(NPC.spawnRate) == 0)
                            {
                                int num5 = (int)(Main.player[i].position.X / 16f) - NPC.spawnRangeX;
                                int num6 = (int)(Main.player[i].position.X / 16f) + NPC.spawnRangeX;
                                int num7 = (int)(Main.player[i].position.Y / 16f) - NPC.spawnRangeY;
                                int num8 = (int)(Main.player[i].position.Y / 16f) + NPC.spawnRangeY;
                                int num9 = (int)(Main.player[i].position.X / 16f) - NPC.safeRangeX;
                                int num10 = (int)(Main.player[i].position.X / 16f) + NPC.safeRangeX;
                                int num11 = (int)(Main.player[i].position.Y / 16f) - NPC.safeRangeY;
                                int num12 = (int)(Main.player[i].position.Y / 16f) + NPC.safeRangeY;
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
                                int j = 0;
                                while (j < 50)
                                {
                                    int k = Main.rand.Next(num5, num6);
                                    int num13 = Main.rand.Next(num7, num8);
                                    if (Main.tile[k, num13].active && Main.tileSolid[(int)Main.tile[k, num13].type])
                                    {
                                        goto IL_CC7;
                                    }
                                    if (!Main.wallHouse[(int)Main.tile[k, num13].wall])
                                    {
                                        if (!flag3 && (double)num13 < Main.worldSurface * 0.30000001192092896 && !flag5 && ((double)k < (double)Main.maxTilesX * 0.35 || (double)k > (double)Main.maxTilesX * 0.65))
                                        {
                                            int num14 = (int)Main.tile[k, num13].type;
                                            num = k;
                                            num2 = num13;
                                            flag = true;
                                            flag2 = true;
                                        }
                                        else
                                        {
                                            for (int l = num13; l < Main.maxTilesY; l++)
                                            {
                                                if (Main.tile[k, l].active && Main.tileSolid[(int)Main.tile[k, l].type])
                                                {
                                                    if (k < num9 || k > num10 || l < num11 || l > num12)
                                                    {
                                                        int num14 = (int)Main.tile[k, l].type;
                                                        num = k;
                                                        num2 = l;
                                                        flag = true;
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        if (flag)
                                        {
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
                                                    for (int l = num17; l < num18; l++)
                                                    {
                                                        if (Main.tile[m, l].active && Main.tileSolid[(int)Main.tile[m, l].type])
                                                        {
                                                            flag = false;
                                                            break;
                                                        }
                                                        if (Main.tile[m, l].lava && l < Main.maxTilesY - 200)
                                                        {
                                                            flag = false;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        goto IL_CC7;
                                    }
                                IL_CE4:
                                    j++;
                                    continue;
                                IL_CC7:
                                    if (flag)
                                    {
                                        break;
                                    }
                                    if (flag)
                                    {
                                        break;
                                    }
                                    goto IL_CE4;
                                }
                            }
                            if (flag)
                            {
                                Rectangle rectangle = new Rectangle(num * 16, num2 * 16, 16, 16);
                                for (int k = 0; k < 255; k++)
                                {
                                    if (Main.player[k].active)
                                    {
                                        Rectangle rectangle2 = new Rectangle((int)(Main.player[k].position.X + (float)(Main.player[k].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[k].position.Y + (float)(Main.player[k].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                        if (rectangle.Intersects(rectangle2))
                                        {
                                            flag = false;
                                        }
                                    }
                                }
                            }
                            if (flag)
                            {
                                if (Main.player[i].zoneDungeon)
                                {
                                    if (!Main.tileDungeon[(int)Main.tile[num, num2].type] || Main.tile[num, num2 - 1].wall == 0)
                                    {
                                        flag = false;
                                    }
                                }
                                if (Main.tile[num, num2 - 1].liquid > 0 && Main.tile[num, num2 - 2].liquid > 0)
                                {
                                    flag4 = true;
                                }
                            }
                            if (flag)
                            {
                                flag = false;
                                int num19 = (int)Main.tile[num, num2].type;
                                int num20 = 1000;
                                if (Main.alwaysSpawn > 0)
                                {
                                    NPC.NewNPC(num * 16 + 8, num2 * 16, Main.alwaysSpawn, 1);
                                    break;
                                }
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
                                        if (flag4 && (((double)num2 > Main.rockLayer && Main.rand.Next(2) == 0) || num19 == 60))
                                        {
                                            NPC.NewNPC(num * 16 + 8, num2 * 16, 58, 0);
                                        }
                                        else
                                        {
                                            if (flag4 && Main.rand.Next(4) == 0)
                                            {
                                                NPC.NewNPC(num * 16 + 8, num2 * 16, 55, 0);
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
                                                        if (num19 != 2)
                                                        {
                                                            break;
                                                        }
                                                        NPC.NewNPC(num * 16 + 8, num2 * 16, 46, 0);
                                                    }
                                                }
                                                else
                                                {
                                                    if (Main.player[i].zoneDungeon)
                                                    {
                                                        if (!NPC.downedBoss3)
                                                        {
                                                            num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 35, 0);
                                                            Main.npc[num20].ai[0] = 1f;
                                                            Main.npc[num20].ai[2] = 2f;
                                                        }
                                                        else
                                                        {
                                                            if (Main.rand.Next(4) == 0)
                                                            {
                                                                num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 34, 0);
                                                            }
                                                            else
                                                            {
                                                                if (Main.rand.Next(5) == 0)
                                                                {
                                                                    num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 32, 0);
                                                                }
                                                                else
                                                                {
                                                                    num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 31, 0);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Main.player[i].zoneMeteor)
                                                        {
                                                            num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 23, 0);
                                                        }
                                                        else
                                                        {
                                                            if (Main.player[i].zoneEvil && Main.rand.Next(50) == 0)
                                                            {
                                                                num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 7, 1);
                                                            }
                                                            else
                                                            {
                                                                if (num19 == 60 && Main.rand.Next(500) == 0 && !Main.dayTime)
                                                                {
                                                                    num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 52, 0);
                                                                }
                                                                else
                                                                {
                                                                    if (num19 == 60 && (double)num2 > (Main.worldSurface + Main.rockLayer) / 2.0)
                                                                    {
                                                                        if (Main.rand.Next(3) == 0)
                                                                        {
                                                                            num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 43, 0);
                                                                            Main.npc[num20].ai[0] = (float)num;
                                                                            Main.npc[num20].ai[1] = (float)num2;
                                                                            Main.npc[num20].netUpdate = true;
                                                                        }
                                                                        else
                                                                        {
                                                                            num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 42, 0);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num19 == 60 && Main.rand.Next(4) == 0)
                                                                        {
                                                                            num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 51, 0);
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num19 == 60 && Main.rand.Next(8) == 0)
                                                                            {
                                                                                num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 56, 0);
                                                                                Main.npc[num20].ai[0] = (float)num;
                                                                                Main.npc[num20].ai[1] = (float)num2;
                                                                                Main.npc[num20].netUpdate = true;
                                                                            }
                                                                            else
                                                                            {
                                                                                if ((double)num2 <= Main.worldSurface)
                                                                                {
                                                                                    if (num19 == 23 || num19 == 25)
                                                                                    {
                                                                                        num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 6, 0);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (Main.dayTime)
                                                                                        {
                                                                                            int num21 = Math.Abs(num - Main.spawnTileX);
                                                                                            if (num21 < Main.maxTilesX / 3 && Main.rand.Next(10) == 0 && num19 == 2)
                                                                                            {
                                                                                                NPC.NewNPC(num * 16 + 8, num2 * 16, 46, 0);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (num21 > Main.maxTilesX / 3 && num19 == 2 && Main.rand.Next(300) == 0)
                                                                                                {
                                                                                                    num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 50, 0);
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 1, 0);
                                                                                                    if (num19 == 60)
                                                                                                    {
                                                                                                        Main.npc[num20].SetDefaults("Jungle Slime");
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (Main.rand.Next(3) == 0 || num21 < 200)
                                                                                                        {
                                                                                                            Main.npc[num20].SetDefaults("Green Slime");
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (Main.rand.Next(10) == 0 && num21 > 400)
                                                                                                            {
                                                                                                                Main.npc[num20].SetDefaults("Purple Slime");
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
                                                                                                num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 2, 0);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Main.rand.Next(250) == 0 && Main.bloodMoon)
                                                                                                {
                                                                                                    NPC.NewNPC(num * 16 + 8, num2 * 16, 52, 0);
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    NPC.NewNPC(num * 16 + 8, num2 * 16, 3, 0);
                                                                                                }
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
                                                                                            num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 10, 1);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 1, 0);
                                                                                            if (Main.rand.Next(5) == 0)
                                                                                            {
                                                                                                Main.npc[num20].SetDefaults("Yellow Slime");
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Main.rand.Next(2) == 0)
                                                                                                {
                                                                                                    Main.npc[num20].SetDefaults("Blue Slime");
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    Main.npc[num20].SetDefaults("Red Slime");
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
                                                                                                num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 39, 1);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 24, 0);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (Main.rand.Next(35) == 0)
                                                                                            {
                                                                                                num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 10, 1);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Main.rand.Next(10) == 0)
                                                                                                {
                                                                                                    num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 16, 0);
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (Main.rand.Next(4) == 0)
                                                                                                    {
                                                                                                        num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 1, 0);
                                                                                                        Main.npc[num20].SetDefaults("Black Slime");
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (Main.rand.Next(2) == 0)
                                                                                                        {
                                                                                                            if ((double)num2 > (Main.rockLayer + (double)Main.maxTilesY) / 2.0 && Main.rand.Next(700) == 0)
                                                                                                            {
                                                                                                                num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 45, 0);
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (Main.rand.Next(15) == 0)
                                                                                                                {
                                                                                                                    num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 44, 0);
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 21, 0);
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            num20 = NPC.NewNPC(num * 16 + 8, num2 * 16, 49, 0);
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
                                if (Main.npc[num20].type == 1 && Main.rand.Next(250) == 0)
                                {
                                    Main.npc[num20].SetDefaults("Pinky");
                                }
                                if (Main.netMode == 2 && num20 < 1000)
                                {
                                    NetMessage.SendData(23, -1, -1, "", num20, 0f, 0f, 0f);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        public static void SpawnOnPlayer(int plr, int Type)
        {
            if (Main.netMode != 1)
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
                        int k = Main.rand.Next(num3, num4);
                        int num11 = Main.rand.Next(num5, num6);
                        if (Main.tile[k, num11].active && Main.tileSolid[(int)Main.tile[k, num11].type])
                        {
                            goto IL_3BB;
                        }
                        if (Main.tile[k, num11].wall != 1)
                        {
                            for (int l = num11; l < Main.maxTilesY; l++)
                            {
                                if (Main.tile[k, l].active && Main.tileSolid[(int)Main.tile[k, l].type])
                                {
                                    if (k < num7 || k > num8 || l < num9 || l > num10)
                                    {
                                        int num12 = (int)Main.tile[k, l].type;
                                        num = k;
                                        num2 = l;
                                        flag = true;
                                    }
                                    break;
                                }
                            }
                            if (flag)
                            {
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
                                    for (int m = num13; m < num14; m++)
                                    {
                                        for (int l = num15; l < num16; l++)
                                        {
                                            if (Main.tile[m, l].active && Main.tileSolid[(int)Main.tile[m, l].type])
                                            {
                                                flag = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            goto IL_3BB;
                        }
                    IL_3D8:
                        j++;
                        continue;
                    IL_3BB:
                        if (flag)
                        {
                            break;
                        }
                        if (flag)
                        {
                            break;
                        }
                        goto IL_3D8;
                    }
                    if (flag)
                    {
                        Rectangle rectangle = new Rectangle(num * 16, num2 * 16, 16, 16);
                        for (int k = 0; k < 255; k++)
                        {
                            if (Main.player[k].active)
                            {
                                Rectangle rectangle2 = new Rectangle((int)(Main.player[k].position.X + (float)(Main.player[k].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[k].position.Y + (float)(Main.player[k].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
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
                    }
                    else
                    {
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(25, -1, -1, str + " has awoken!", 255, 175f, 75f, 255f);
                        }
                    }
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
            int result;
            if (num >= 0)
            {
                Main.npc[num] = new NPC();
                Main.npc[num].SetDefaults(Type);
                Main.npc[num].position.X = (float)(X - Main.npc[num].width / 2);
                Main.npc[num].position.Y = (float)(Y - Main.npc[num].height);
                Main.npc[num].active = true;
                Main.npc[num].timeLeft = (int)((double)NPC.activeTime * 1.25);
                Main.npc[num].wet = Collision.WetCollision(Main.npc[num].position, Main.npc[num].width, Main.npc[num].height);
                if (Type == 50)
                {
                    if (Main.netMode == 0)
                    {
                        //Main.NewText(Main.npc[num].name + " has awoken!", 175, 75, 255);
                    }
                    else
                    {
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(25, -1, -1, Main.npc[num].name + " has awoken!", 255, 175f, 75f, 255f);
                        }
                    }
                }
                result = num;
            }
            else
            {
                result = 1000;
            }
            return result;
        }
        public void Transform(int newType)
        {
            if (Main.netMode != 1)
            {
                Vector2 vector = this.velocity;
                int num = this.spriteDirection;
                this.SetDefaults(newType);
                this.spriteDirection = num;
                this.TargetClosest(true);
                this.velocity = vector;
                if (Main.netMode == 2)
                {
                    this.netUpdate = true;
                    NetMessage.SendData(23, -1, -1, "", this.whoAmI, 0f, 0f, 0f);
                }
            }
        }
        public double StrikeNPC(int Damage, float knockBack, int hitDirection)
        {
            double result;
            if (!this.active || this.life <= 0)
            {
                result = 0.0;
            }
            else
            {
                double num = Main.CalculateDamage(Damage, this.defense);
                if (this.friendly)
                {
                    //CombatText.NewText(new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height), new Color(255, 80, 90, 255), string.Concat((int)num));
                }
                else
                {
                    //CombatText.NewText(new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height), new Color(255, 160, 80, 255), string.Concat((int)num));
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
                        this.TargetClosest(true);
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
                        NPC.noSpawnCycle = true;
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
                    result = num;
                }
                else
                {
                    result = 0.0;
                }
            }
            return result;
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
            if (this.type == 58)
            {
                if (Main.rand.Next(500) == 0)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 263, 1, false);
                }
                else
                {
                    if (Main.rand.Next(40) == 0)
                    {
                        Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 118, 1, false);
                    }
                }
            }
            if (this.type == 3)
            {
                if (Main.rand.Next(50) == 0)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 216, 1, false);
                }
            }
            if (this.type == 52)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 251, 1, false);
            }
            if (this.type == 53)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 239, 1, false);
            }
            if (this.type == 54)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 260, 1, false);
            }
            if (this.type == 55)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 261, 1, false);
            }
            if (this.type == 4)
            {
                int num = Main.rand.Next(30) + 20;
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 47, num, false);
                num = Main.rand.Next(20) + 10;
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, num, false);
                num = Main.rand.Next(20) + 10;
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, num, false);
                num = Main.rand.Next(20) + 10;
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, num, false);
                num = Main.rand.Next(3) + 1;
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 59, num, false);
            }
            if (this.type == 6)
            {
                if (Main.rand.Next(3) == 0)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 68, 1, false);
                }
            }
            if (this.type == 7 || this.type == 8 || this.type == 9)
            {
                if (Main.rand.Next(3) == 0)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 68, Main.rand.Next(1, 3), false);
                }
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 69, Main.rand.Next(3, 9), false);
            }
            if (this.type == 10 || this.type == 11 || this.type == 12)
            {
                if (Main.rand.Next(500) == 0)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 215, 1, false);
                }
            }
            if (this.type == 47)
            {
                if (Main.rand.Next(75) == 0)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 243, 1, false);
                }
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
                int num = Main.rand.Next(1, 4);
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 86, num, false);
                if (Main.rand.Next(2) == 0)
                {
                    num = Main.rand.Next(2, 6);
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, num, false);
                }
                if (this.boss)
                {
                    num = Main.rand.Next(15, 30);
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, num, false);
                    num = Main.rand.Next(15, 31);
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, num, false);
                    int num2 = Main.rand.Next(100, 103);
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, num2, 1, false);
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
            if (this.type == 50)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, Main.rand.Next(256, 259), 1, false);
            }
            if (this.type == 23)
            {
                if (Main.rand.Next(50) == 0)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 116, 1, false);
                }
            }
            if (this.type == 24)
            {
                if (Main.rand.Next(50) == 0)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 112, 1, false);
                }
                else
                {
                    if (Main.rand.Next(500) == 0)
                    {
                        Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 244, 1, false);
                    }
                }
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
                            int num = Main.rand.Next(1, 6);
                            Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 161, num, false);
                        }
                    }
                }
            }
            if (this.type == 42)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 209, 1, false);
            }
            if (this.type == 43)
            {
                if (Main.rand.Next(5) == 0)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 210, 1, false);
                }
            }
            if (this.type == 42 || this.type == 43)
            {
                if (Main.rand.Next(150) == 0)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, Main.rand.Next(228, 231), 1, false);
                }
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
                int num = Main.rand.Next(5, 16);
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 28, num, false);
                int num3 = Main.rand.Next(5) + 5;
                for (int i = 0; i < num3; i++)
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
            if (Main.rand.Next(7) == 0 && this.lifeMax > 1)
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
            float num4 = this.value;
            num4 *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
            if (Main.rand.Next(5) == 0)
            {
                num4 *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;
            }
            if (Main.rand.Next(10) == 0)
            {
                num4 *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;
            }
            if (Main.rand.Next(15) == 0)
            {
                num4 *= 1f + (float)Main.rand.Next(15, 31) * 0.01f;
            }
            if (Main.rand.Next(20) == 0)
            {
                num4 *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;
            }
            while ((int)num4 > 0)
            {
                if (num4 > 1000000f)
                {
                    int num = (int)(num4 / 1000000f);
                    if (num > 50 && Main.rand.Next(2) == 0)
                    {
                        num /= Main.rand.Next(3) + 1;
                    }
                    if (Main.rand.Next(2) == 0)
                    {
                        num /= Main.rand.Next(3) + 1;
                    }
                    num4 -= (float)(1000000 * num);
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 74, num, false);
                }
                else
                {
                    if (num4 > 10000f)
                    {
                        int num = (int)(num4 / 10000f);
                        if (num > 50 && Main.rand.Next(2) == 0)
                        {
                            num /= Main.rand.Next(3) + 1;
                        }
                        if (Main.rand.Next(2) == 0)
                        {
                            num /= Main.rand.Next(3) + 1;
                        }
                        num4 -= (float)(10000 * num);
                        Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 73, num, false);
                    }
                    else
                    {
                        if (num4 > 100f)
                        {
                            int num = (int)(num4 / 100f);
                            if (num > 50 && Main.rand.Next(2) == 0)
                            {
                                num /= Main.rand.Next(3) + 1;
                            }
                            if (Main.rand.Next(2) == 0)
                            {
                                num /= Main.rand.Next(3) + 1;
                            }
                            num4 -= (float)(100 * num);
                            Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 72, num, false);
                        }
                        else
                        {
                            int num = (int)num4;
                            if (num > 50 && Main.rand.Next(2) == 0)
                            {
                                num /= Main.rand.Next(3) + 1;
                            }
                            if (Main.rand.Next(2) == 0)
                            {
                                num /= Main.rand.Next(4) + 1;
                            }
                            if (num < 1)
                            {
                                num = 1;
                            }
                            num4 -= (float)num;
                            Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 71, num, false);
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
                    int i = 0;
                    while ((double)i < dmg / (double)this.lifeMax * 100.0)
                    {
                        Dust.NewDust(this.position, this.width, this.height, 4, (float)hitDirection, -1f, this.alpha, this.color, 1f);
                        i++;
                    }
                }
                else
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Dust.NewDust(this.position, this.width, this.height, 4, (float)(2 * hitDirection), -2f, this.alpha, this.color, 1f);
                    }
                    if (Main.netMode != 1)
                    {
                        if (this.type == 16)
                        {
                            int num = Main.rand.Next(2) + 2;
                            for (int j = 0; j < num; j++)
                            {
                                int num2 = NPC.NewNPC((int)(this.position.X + (float)(this.width / 2)), (int)(this.position.Y + (float)this.height), 1, 0);
                                Main.npc[num2].SetDefaults("Baby Slime");
                                Main.npc[num2].velocity.X = this.velocity.X * 2f;
                                Main.npc[num2].velocity.Y = this.velocity.Y;
                                NPC expr_1AB_cp_0 = Main.npc[num2];
                                expr_1AB_cp_0.velocity.X = expr_1AB_cp_0.velocity.X + ((float)Main.rand.Next(-20, 20) * 0.1f + (float)(j * this.direction) * 0.3f);
                                NPC expr_1E8_cp_0 = Main.npc[num2];
                                expr_1E8_cp_0.velocity.Y = expr_1E8_cp_0.velocity.Y - ((float)Main.rand.Next(0, 10) * 0.1f + (float)j);
                                Main.npc[num2].ai[1] = (float)j;
                                if (Main.netMode == 2 && num2 < 1000)
                                {
                                    NetMessage.SendData(23, -1, -1, "", num2, 0f, 0f, 0f);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.type == 50)
                {
                    if (this.life > 0)
                    {
                        int i = 0;
                        while ((double)i < dmg / (double)this.lifeMax * 300.0)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 4, (float)hitDirection, -1f, 175, new Color(0, 80, 255, 100), 1f);
                            i++;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 200; i++)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 4, (float)(2 * hitDirection), -2f, 175, new Color(0, 80, 255, 100), 1f);
                        }
                        if (Main.netMode != 1)
                        {
                            int num = Main.rand.Next(4) + 4;
                            for (int j = 0; j < num; j++)
                            {
                                int x = (int)(this.position.X + (float)Main.rand.Next(this.width - 32));
                                int y = (int)(this.position.Y + (float)Main.rand.Next(this.height - 32));
                                int num2 = NPC.NewNPC(x, y, 1, 0);
                                Main.npc[num2].SetDefaults(1);
                                Main.npc[num2].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                                Main.npc[num2].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                                Main.npc[num2].ai[1] = (float)Main.rand.Next(3);
                                if (Main.netMode == 2 && num2 < 1000)
                                {
                                    NetMessage.SendData(23, -1, -1, "", num2, 0f, 0f, 0f);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (this.type == 49 || this.type == 51)
                    {
                        if (this.life > 0)
                        {
                            int i = 0;
                            while ((double)i < dmg / (double)this.lifeMax * 30.0)
                            {
                                Vector2 arg_501_0 = this.position;
                                int arg_501_1 = this.width;
                                int arg_501_2 = this.height;
                                int arg_501_3 = 5;
                                float arg_501_4 = (float)hitDirection;
                                float arg_501_5 = -1f;
                                int arg_501_6 = 0;
                                Color newColor = default(Color);
                                Dust.NewDust(arg_501_0, arg_501_1, arg_501_2, arg_501_3, arg_501_4, arg_501_5, arg_501_6, newColor, 1f);
                                i++;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 15; i++)
                            {
                                Vector2 arg_561_0 = this.position;
                                int arg_561_1 = this.width;
                                int arg_561_2 = this.height;
                                int arg_561_3 = 5;
                                float arg_561_4 = (float)(2 * hitDirection);
                                float arg_561_5 = -2f;
                                int arg_561_6 = 0;
                                Color newColor = default(Color);
                                Dust.NewDust(arg_561_0, arg_561_1, arg_561_2, arg_561_3, arg_561_4, arg_561_5, arg_561_6, newColor, 1f);
                            }
                            if (this.type == 51)
                            {
                                Gore.NewGore(this.position, this.velocity, 83);
                            }
                            else
                            {
                                Gore.NewGore(this.position, this.velocity, 82);
                            }
                        }
                    }
                    else
                    {
                        if (this.type == 46 || this.type == 55)
                        {
                            if (this.life > 0)
                            {
                                int i = 0;
                                while ((double)i < dmg / (double)this.lifeMax * 20.0)
                                {
                                    Vector2 arg_625_0 = this.position;
                                    int arg_625_1 = this.width;
                                    int arg_625_2 = this.height;
                                    int arg_625_3 = 5;
                                    float arg_625_4 = (float)hitDirection;
                                    float arg_625_5 = -1f;
                                    int arg_625_6 = 0;
                                    Color newColor = default(Color);
                                    Dust.NewDust(arg_625_0, arg_625_1, arg_625_2, arg_625_3, arg_625_4, arg_625_5, arg_625_6, newColor, 1f);
                                    i++;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    Vector2 arg_685_0 = this.position;
                                    int arg_685_1 = this.width;
                                    int arg_685_2 = this.height;
                                    int arg_685_3 = 5;
                                    float arg_685_4 = (float)(2 * hitDirection);
                                    float arg_685_5 = -2f;
                                    int arg_685_6 = 0;
                                    Color newColor = default(Color);
                                    Dust.NewDust(arg_685_0, arg_685_1, arg_685_2, arg_685_3, arg_685_4, arg_685_5, arg_685_6, newColor, 1f);
                                }
                                if (this.type == 46)
                                {
                                    Gore.NewGore(this.position, this.velocity, 76);
                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y), this.velocity, 77);
                                }
                            }
                        }
                        else
                        {
                            if (this.type == 47 || this.type == 57 || this.type == 58)
                            {
                                if (this.life > 0)
                                {
                                    int i = 0;
                                    while ((double)i < dmg / (double)this.lifeMax * 20.0)
                                    {
                                        Vector2 arg_764_0 = this.position;
                                        int arg_764_1 = this.width;
                                        int arg_764_2 = this.height;
                                        int arg_764_3 = 5;
                                        float arg_764_4 = (float)hitDirection;
                                        float arg_764_5 = -1f;
                                        int arg_764_6 = 0;
                                        Color newColor = default(Color);
                                        Dust.NewDust(arg_764_0, arg_764_1, arg_764_2, arg_764_3, arg_764_4, arg_764_5, arg_764_6, newColor, 1f);
                                        i++;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        Vector2 arg_7C4_0 = this.position;
                                        int arg_7C4_1 = this.width;
                                        int arg_7C4_2 = this.height;
                                        int arg_7C4_3 = 5;
                                        float arg_7C4_4 = (float)(2 * hitDirection);
                                        float arg_7C4_5 = -2f;
                                        int arg_7C4_6 = 0;
                                        Color newColor = default(Color);
                                        Dust.NewDust(arg_7C4_0, arg_7C4_1, arg_7C4_2, arg_7C4_3, arg_7C4_4, arg_7C4_5, arg_7C4_6, newColor, 1f);
                                    }
                                    if (this.type == 57)
                                    {
                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y), this.velocity, 84);
                                    }
                                    else
                                    {
                                        if (this.type == 58)
                                        {
                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y), this.velocity, 85);
                                        }
                                        else
                                        {
                                            Gore.NewGore(this.position, this.velocity, 78);
                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y), this.velocity, 79);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (this.type == 2)
                                {
                                    if (this.life > 0)
                                    {
                                        int i = 0;
                                        while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                        {
                                            Vector2 arg_8F8_0 = this.position;
                                            int arg_8F8_1 = this.width;
                                            int arg_8F8_2 = this.height;
                                            int arg_8F8_3 = 5;
                                            float arg_8F8_4 = (float)hitDirection;
                                            float arg_8F8_5 = -1f;
                                            int arg_8F8_6 = 0;
                                            Color newColor = default(Color);
                                            Dust.NewDust(arg_8F8_0, arg_8F8_1, arg_8F8_2, arg_8F8_3, arg_8F8_4, arg_8F8_5, arg_8F8_6, newColor, 1f);
                                            i++;
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < 50; i++)
                                        {
                                            Vector2 arg_958_0 = this.position;
                                            int arg_958_1 = this.width;
                                            int arg_958_2 = this.height;
                                            int arg_958_3 = 5;
                                            float arg_958_4 = (float)(2 * hitDirection);
                                            float arg_958_5 = -2f;
                                            int arg_958_6 = 0;
                                            Color newColor = default(Color);
                                            Dust.NewDust(arg_958_0, arg_958_1, arg_958_2, arg_958_3, arg_958_4, arg_958_5, arg_958_6, newColor, 1f);
                                        }
                                        Gore.NewGore(this.position, this.velocity, 1);
                                        Gore.NewGore(new Vector2(this.position.X + 14f, this.position.Y), this.velocity, 2);
                                    }
                                }
                                else
                                {
                                    if (this.type == 3 || this.type == 52 || this.type == 53)
                                    {
                                        if (this.life > 0)
                                        {
                                            int i = 0;
                                            while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                            {
                                                Vector2 arg_A25_0 = this.position;
                                                int arg_A25_1 = this.width;
                                                int arg_A25_2 = this.height;
                                                int arg_A25_3 = 5;
                                                float arg_A25_4 = (float)hitDirection;
                                                float arg_A25_5 = -1f;
                                                int arg_A25_6 = 0;
                                                Color newColor = default(Color);
                                                Dust.NewDust(arg_A25_0, arg_A25_1, arg_A25_2, arg_A25_3, arg_A25_4, arg_A25_5, arg_A25_6, newColor, 1f);
                                                i++;
                                            }
                                        }
                                        else
                                        {
                                            for (int i = 0; i < 50; i++)
                                            {
                                                Vector2 arg_A89_0 = this.position;
                                                int arg_A89_1 = this.width;
                                                int arg_A89_2 = this.height;
                                                int arg_A89_3 = 5;
                                                float arg_A89_4 = 2.5f * (float)hitDirection;
                                                float arg_A89_5 = -2.5f;
                                                int arg_A89_6 = 0;
                                                Color newColor = default(Color);
                                                Dust.NewDust(arg_A89_0, arg_A89_1, arg_A89_2, arg_A89_3, arg_A89_4, arg_A89_5, arg_A89_6, newColor, 1f);
                                            }
                                            Gore.NewGore(this.position, this.velocity, 3);
                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 4);
                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 4);
                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 5);
                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 5);
                                        }
                                    }
                                    else
                                    {
                                        if (this.type == 4)
                                        {
                                            if (this.life > 0)
                                            {
                                                int i = 0;
                                                while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                {
                                                    Vector2 arg_BC9_0 = this.position;
                                                    int arg_BC9_1 = this.width;
                                                    int arg_BC9_2 = this.height;
                                                    int arg_BC9_3 = 5;
                                                    float arg_BC9_4 = (float)hitDirection;
                                                    float arg_BC9_5 = -1f;
                                                    int arg_BC9_6 = 0;
                                                    Color newColor = default(Color);
                                                    Dust.NewDust(arg_BC9_0, arg_BC9_1, arg_BC9_2, arg_BC9_3, arg_BC9_4, arg_BC9_5, arg_BC9_6, newColor, 1f);
                                                    i++;
                                                }
                                            }
                                            else
                                            {
                                                for (int i = 0; i < 150; i++)
                                                {
                                                    Vector2 arg_C29_0 = this.position;
                                                    int arg_C29_1 = this.width;
                                                    int arg_C29_2 = this.height;
                                                    int arg_C29_3 = 5;
                                                    float arg_C29_4 = (float)(2 * hitDirection);
                                                    float arg_C29_5 = -2f;
                                                    int arg_C29_6 = 0;
                                                    Color newColor = default(Color);
                                                    Dust.NewDust(arg_C29_0, arg_C29_1, arg_C29_2, arg_C29_3, arg_C29_4, arg_C29_5, arg_C29_6, newColor, 1f);
                                                }
                                                for (int i = 0; i < 2; i++)
                                                {
                                                    Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 2);
                                                    Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7);
                                                    Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 9);
                                                    Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 10);
                                                }
                                                //Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
                                            }
                                        }
                                        else
                                        {
                                            if (this.type == 5)
                                            {
                                                if (this.life > 0)
                                                {
                                                    int i = 0;
                                                    while ((double)i < dmg / (double)this.lifeMax * 50.0)
                                                    {
                                                        Vector2 arg_DCE_0 = this.position;
                                                        int arg_DCE_1 = this.width;
                                                        int arg_DCE_2 = this.height;
                                                        int arg_DCE_3 = 5;
                                                        float arg_DCE_4 = (float)hitDirection;
                                                        float arg_DCE_5 = -1f;
                                                        int arg_DCE_6 = 0;
                                                        Color newColor = default(Color);
                                                        Dust.NewDust(arg_DCE_0, arg_DCE_1, arg_DCE_2, arg_DCE_3, arg_DCE_4, arg_DCE_5, arg_DCE_6, newColor, 1f);
                                                        i++;
                                                    }
                                                }
                                                else
                                                {
                                                    for (int i = 0; i < 20; i++)
                                                    {
                                                        Vector2 arg_E2B_0 = this.position;
                                                        int arg_E2B_1 = this.width;
                                                        int arg_E2B_2 = this.height;
                                                        int arg_E2B_3 = 5;
                                                        float arg_E2B_4 = (float)(2 * hitDirection);
                                                        float arg_E2B_5 = -2f;
                                                        int arg_E2B_6 = 0;
                                                        Color newColor = default(Color);
                                                        Dust.NewDust(arg_E2B_0, arg_E2B_1, arg_E2B_2, arg_E2B_3, arg_E2B_4, arg_E2B_5, arg_E2B_6, newColor, 1f);
                                                    }
                                                    Gore.NewGore(this.position, this.velocity, 6);
                                                    Gore.NewGore(this.position, this.velocity, 7);
                                                }
                                            }
                                            else
                                            {
                                                if (this.type == 6)
                                                {
                                                    if (this.life > 0)
                                                    {
                                                        int i = 0;
                                                        while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                        {
                                                            Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
                                                            i++;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        for (int i = 0; i < 50; i++)
                                                        {
                                                            Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
                                                        }
                                                        int num3 = Gore.NewGore(this.position, this.velocity, 14);
                                                        Main.gore[num3].alpha = this.alpha;
                                                        num3 = Gore.NewGore(this.position, this.velocity, 15);
                                                        Main.gore[num3].alpha = this.alpha;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.type == 7 || this.type == 8 || this.type == 9)
                                                    {
                                                        if (this.life > 0)
                                                        {
                                                            int i = 0;
                                                            while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                            {
                                                                Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
                                                                i++;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            for (int i = 0; i < 50; i++)
                                                            {
                                                                Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
                                                            }
                                                            int num3 = Gore.NewGore(this.position, this.velocity, this.type - 7 + 18);
                                                            Main.gore[num3].alpha = this.alpha;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (this.type == 10 || this.type == 11 || this.type == 12)
                                                        {
                                                            if (this.life > 0)
                                                            {
                                                                int i = 0;
                                                                while ((double)i < dmg / (double)this.lifeMax * 50.0)
                                                                {
                                                                    Vector2 arg_1124_0 = this.position;
                                                                    int arg_1124_1 = this.width;
                                                                    int arg_1124_2 = this.height;
                                                                    int arg_1124_3 = 5;
                                                                    float arg_1124_4 = (float)hitDirection;
                                                                    float arg_1124_5 = -1f;
                                                                    int arg_1124_6 = 0;
                                                                    Color newColor = default(Color);
                                                                    Dust.NewDust(arg_1124_0, arg_1124_1, arg_1124_2, arg_1124_3, arg_1124_4, arg_1124_5, arg_1124_6, newColor, 1f);
                                                                    i++;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                for (int i = 0; i < 10; i++)
                                                                {
                                                                    Vector2 arg_1185_0 = this.position;
                                                                    int arg_1185_1 = this.width;
                                                                    int arg_1185_2 = this.height;
                                                                    int arg_1185_3 = 5;
                                                                    float arg_1185_4 = 2.5f * (float)hitDirection;
                                                                    float arg_1185_5 = -2.5f;
                                                                    int arg_1185_6 = 0;
                                                                    Color newColor = default(Color);
                                                                    Dust.NewDust(arg_1185_0, arg_1185_1, arg_1185_2, arg_1185_3, arg_1185_4, arg_1185_5, arg_1185_6, newColor, 1f);
                                                                }
                                                                Gore.NewGore(this.position, this.velocity, this.type - 7 + 18);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (this.type == 13 || this.type == 14 || this.type == 15)
                                                            {
                                                                if (this.life > 0)
                                                                {
                                                                    int i = 0;
                                                                    while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                    {
                                                                        Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
                                                                        i++;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    for (int i = 0; i < 50; i++)
                                                                    {
                                                                        Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
                                                                    }
                                                                    if (this.type == 13)
                                                                    {
                                                                        Gore.NewGore(this.position, this.velocity, 24);
                                                                        Gore.NewGore(this.position, this.velocity, 25);
                                                                    }
                                                                    else
                                                                    {
                                                                        if (this.type == 14)
                                                                        {
                                                                            Gore.NewGore(this.position, this.velocity, 26);
                                                                            Gore.NewGore(this.position, this.velocity, 27);
                                                                        }
                                                                        else
                                                                        {
                                                                            Gore.NewGore(this.position, this.velocity, 28);
                                                                            Gore.NewGore(this.position, this.velocity, 29);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (this.type == 17)
                                                                {
                                                                    if (this.life > 0)
                                                                    {
                                                                        int i = 0;
                                                                        while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                        {
                                                                            Vector2 arg_13B1_0 = this.position;
                                                                            int arg_13B1_1 = this.width;
                                                                            int arg_13B1_2 = this.height;
                                                                            int arg_13B1_3 = 5;
                                                                            float arg_13B1_4 = (float)hitDirection;
                                                                            float arg_13B1_5 = -1f;
                                                                            int arg_13B1_6 = 0;
                                                                            Color newColor = default(Color);
                                                                            Dust.NewDust(arg_13B1_0, arg_13B1_1, arg_13B1_2, arg_13B1_3, arg_13B1_4, arg_13B1_5, arg_13B1_6, newColor, 1f);
                                                                            i++;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        for (int i = 0; i < 50; i++)
                                                                        {
                                                                            Vector2 arg_1415_0 = this.position;
                                                                            int arg_1415_1 = this.width;
                                                                            int arg_1415_2 = this.height;
                                                                            int arg_1415_3 = 5;
                                                                            float arg_1415_4 = 2.5f * (float)hitDirection;
                                                                            float arg_1415_5 = -2.5f;
                                                                            int arg_1415_6 = 0;
                                                                            Color newColor = default(Color);
                                                                            Dust.NewDust(arg_1415_0, arg_1415_1, arg_1415_2, arg_1415_3, arg_1415_4, arg_1415_5, arg_1415_6, newColor, 1f);
                                                                        }
                                                                        Gore.NewGore(this.position, this.velocity, 30);
                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 31);
                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 31);
                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 32);
                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 32);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (this.type == 22)
                                                                    {
                                                                        if (this.life > 0)
                                                                        {
                                                                            int i = 0;
                                                                            while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                            {
                                                                                Vector2 arg_155B_0 = this.position;
                                                                                int arg_155B_1 = this.width;
                                                                                int arg_155B_2 = this.height;
                                                                                int arg_155B_3 = 5;
                                                                                float arg_155B_4 = (float)hitDirection;
                                                                                float arg_155B_5 = -1f;
                                                                                int arg_155B_6 = 0;
                                                                                Color newColor = default(Color);
                                                                                Dust.NewDust(arg_155B_0, arg_155B_1, arg_155B_2, arg_155B_3, arg_155B_4, arg_155B_5, arg_155B_6, newColor, 1f);
                                                                                i++;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            for (int i = 0; i < 50; i++)
                                                                            {
                                                                                Vector2 arg_15BF_0 = this.position;
                                                                                int arg_15BF_1 = this.width;
                                                                                int arg_15BF_2 = this.height;
                                                                                int arg_15BF_3 = 5;
                                                                                float arg_15BF_4 = 2.5f * (float)hitDirection;
                                                                                float arg_15BF_5 = -2.5f;
                                                                                int arg_15BF_6 = 0;
                                                                                Color newColor = default(Color);
                                                                                Dust.NewDust(arg_15BF_0, arg_15BF_1, arg_15BF_2, arg_15BF_3, arg_15BF_4, arg_15BF_5, arg_15BF_6, newColor, 1f);
                                                                            }
                                                                            Gore.NewGore(this.position, this.velocity, 73);
                                                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 74);
                                                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 74);
                                                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 75);
                                                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 75);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (this.type == 37 || this.type == 54)
                                                                        {
                                                                            if (this.life > 0)
                                                                            {
                                                                                int i = 0;
                                                                                while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                                {
                                                                                    Vector2 arg_1712_0 = this.position;
                                                                                    int arg_1712_1 = this.width;
                                                                                    int arg_1712_2 = this.height;
                                                                                    int arg_1712_3 = 5;
                                                                                    float arg_1712_4 = (float)hitDirection;
                                                                                    float arg_1712_5 = -1f;
                                                                                    int arg_1712_6 = 0;
                                                                                    Color newColor = default(Color);
                                                                                    Dust.NewDust(arg_1712_0, arg_1712_1, arg_1712_2, arg_1712_3, arg_1712_4, arg_1712_5, arg_1712_6, newColor, 1f);
                                                                                    i++;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                for (int i = 0; i < 50; i++)
                                                                                {
                                                                                    Vector2 arg_1776_0 = this.position;
                                                                                    int arg_1776_1 = this.width;
                                                                                    int arg_1776_2 = this.height;
                                                                                    int arg_1776_3 = 5;
                                                                                    float arg_1776_4 = 2.5f * (float)hitDirection;
                                                                                    float arg_1776_5 = -2.5f;
                                                                                    int arg_1776_6 = 0;
                                                                                    Color newColor = default(Color);
                                                                                    Dust.NewDust(arg_1776_0, arg_1776_1, arg_1776_2, arg_1776_3, arg_1776_4, arg_1776_5, arg_1776_6, newColor, 1f);
                                                                                }
                                                                                Gore.NewGore(this.position, this.velocity, 58);
                                                                                Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 59);
                                                                                Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 59);
                                                                                Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 60);
                                                                                Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 60);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (this.type == 18)
                                                                            {
                                                                                if (this.life > 0)
                                                                                {
                                                                                    int i = 0;
                                                                                    while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                                    {
                                                                                        Vector2 arg_18BC_0 = this.position;
                                                                                        int arg_18BC_1 = this.width;
                                                                                        int arg_18BC_2 = this.height;
                                                                                        int arg_18BC_3 = 5;
                                                                                        float arg_18BC_4 = (float)hitDirection;
                                                                                        float arg_18BC_5 = -1f;
                                                                                        int arg_18BC_6 = 0;
                                                                                        Color newColor = default(Color);
                                                                                        Dust.NewDust(arg_18BC_0, arg_18BC_1, arg_18BC_2, arg_18BC_3, arg_18BC_4, arg_18BC_5, arg_18BC_6, newColor, 1f);
                                                                                        i++;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    for (int i = 0; i < 50; i++)
                                                                                    {
                                                                                        Vector2 arg_1920_0 = this.position;
                                                                                        int arg_1920_1 = this.width;
                                                                                        int arg_1920_2 = this.height;
                                                                                        int arg_1920_3 = 5;
                                                                                        float arg_1920_4 = 2.5f * (float)hitDirection;
                                                                                        float arg_1920_5 = -2.5f;
                                                                                        int arg_1920_6 = 0;
                                                                                        Color newColor = default(Color);
                                                                                        Dust.NewDust(arg_1920_0, arg_1920_1, arg_1920_2, arg_1920_3, arg_1920_4, arg_1920_5, arg_1920_6, newColor, 1f);
                                                                                    }
                                                                                    Gore.NewGore(this.position, this.velocity, 33);
                                                                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 34);
                                                                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 34);
                                                                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 35);
                                                                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 35);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (this.type == 19)
                                                                                {
                                                                                    if (this.life > 0)
                                                                                    {
                                                                                        int i = 0;
                                                                                        while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                                        {
                                                                                            Vector2 arg_1A66_0 = this.position;
                                                                                            int arg_1A66_1 = this.width;
                                                                                            int arg_1A66_2 = this.height;
                                                                                            int arg_1A66_3 = 5;
                                                                                            float arg_1A66_4 = (float)hitDirection;
                                                                                            float arg_1A66_5 = -1f;
                                                                                            int arg_1A66_6 = 0;
                                                                                            Color newColor = default(Color);
                                                                                            Dust.NewDust(arg_1A66_0, arg_1A66_1, arg_1A66_2, arg_1A66_3, arg_1A66_4, arg_1A66_5, arg_1A66_6, newColor, 1f);
                                                                                            i++;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        for (int i = 0; i < 50; i++)
                                                                                        {
                                                                                            Vector2 arg_1ACA_0 = this.position;
                                                                                            int arg_1ACA_1 = this.width;
                                                                                            int arg_1ACA_2 = this.height;
                                                                                            int arg_1ACA_3 = 5;
                                                                                            float arg_1ACA_4 = 2.5f * (float)hitDirection;
                                                                                            float arg_1ACA_5 = -2.5f;
                                                                                            int arg_1ACA_6 = 0;
                                                                                            Color newColor = default(Color);
                                                                                            Dust.NewDust(arg_1ACA_0, arg_1ACA_1, arg_1ACA_2, arg_1ACA_3, arg_1ACA_4, arg_1ACA_5, arg_1ACA_6, newColor, 1f);
                                                                                        }
                                                                                        Gore.NewGore(this.position, this.velocity, 36);
                                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 37);
                                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 37);
                                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 38);
                                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 38);
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (this.type == 38)
                                                                                    {
                                                                                        if (this.life > 0)
                                                                                        {
                                                                                            int i = 0;
                                                                                            while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                                            {
                                                                                                Vector2 arg_1C10_0 = this.position;
                                                                                                int arg_1C10_1 = this.width;
                                                                                                int arg_1C10_2 = this.height;
                                                                                                int arg_1C10_3 = 5;
                                                                                                float arg_1C10_4 = (float)hitDirection;
                                                                                                float arg_1C10_5 = -1f;
                                                                                                int arg_1C10_6 = 0;
                                                                                                Color newColor = default(Color);
                                                                                                Dust.NewDust(arg_1C10_0, arg_1C10_1, arg_1C10_2, arg_1C10_3, arg_1C10_4, arg_1C10_5, arg_1C10_6, newColor, 1f);
                                                                                                i++;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            for (int i = 0; i < 50; i++)
                                                                                            {
                                                                                                Vector2 arg_1C74_0 = this.position;
                                                                                                int arg_1C74_1 = this.width;
                                                                                                int arg_1C74_2 = this.height;
                                                                                                int arg_1C74_3 = 5;
                                                                                                float arg_1C74_4 = 2.5f * (float)hitDirection;
                                                                                                float arg_1C74_5 = -2.5f;
                                                                                                int arg_1C74_6 = 0;
                                                                                                Color newColor = default(Color);
                                                                                                Dust.NewDust(arg_1C74_0, arg_1C74_1, arg_1C74_2, arg_1C74_3, arg_1C74_4, arg_1C74_5, arg_1C74_6, newColor, 1f);
                                                                                            }
                                                                                            Gore.NewGore(this.position, this.velocity, 64);
                                                                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 65);
                                                                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 65);
                                                                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 66);
                                                                                            Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 66);
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (this.type == 20)
                                                                                        {
                                                                                            if (this.life > 0)
                                                                                            {
                                                                                                int i = 0;
                                                                                                while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                                                {
                                                                                                    Vector2 arg_1DBA_0 = this.position;
                                                                                                    int arg_1DBA_1 = this.width;
                                                                                                    int arg_1DBA_2 = this.height;
                                                                                                    int arg_1DBA_3 = 5;
                                                                                                    float arg_1DBA_4 = (float)hitDirection;
                                                                                                    float arg_1DBA_5 = -1f;
                                                                                                    int arg_1DBA_6 = 0;
                                                                                                    Color newColor = default(Color);
                                                                                                    Dust.NewDust(arg_1DBA_0, arg_1DBA_1, arg_1DBA_2, arg_1DBA_3, arg_1DBA_4, arg_1DBA_5, arg_1DBA_6, newColor, 1f);
                                                                                                    i++;
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                for (int i = 0; i < 50; i++)
                                                                                                {
                                                                                                    Vector2 arg_1E1E_0 = this.position;
                                                                                                    int arg_1E1E_1 = this.width;
                                                                                                    int arg_1E1E_2 = this.height;
                                                                                                    int arg_1E1E_3 = 5;
                                                                                                    float arg_1E1E_4 = 2.5f * (float)hitDirection;
                                                                                                    float arg_1E1E_5 = -2.5f;
                                                                                                    int arg_1E1E_6 = 0;
                                                                                                    Color newColor = default(Color);
                                                                                                    Dust.NewDust(arg_1E1E_0, arg_1E1E_1, arg_1E1E_2, arg_1E1E_3, arg_1E1E_4, arg_1E1E_5, arg_1E1E_6, newColor, 1f);
                                                                                                }
                                                                                                Gore.NewGore(this.position, this.velocity, 39);
                                                                                                Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 40);
                                                                                                Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 40);
                                                                                                Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 41);
                                                                                                Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 41);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (this.type == 21 || this.type == 31 || this.type == 32 || this.type == 44 || this.type == 45)
                                                                                            {
                                                                                                if (this.life > 0)
                                                                                                {
                                                                                                    int i = 0;
                                                                                                    while ((double)i < dmg / (double)this.lifeMax * 50.0)
                                                                                                    {
                                                                                                        Vector2 arg_1F90_0 = this.position;
                                                                                                        int arg_1F90_1 = this.width;
                                                                                                        int arg_1F90_2 = this.height;
                                                                                                        int arg_1F90_3 = 26;
                                                                                                        float arg_1F90_4 = (float)hitDirection;
                                                                                                        float arg_1F90_5 = -1f;
                                                                                                        int arg_1F90_6 = 0;
                                                                                                        Color newColor = default(Color);
                                                                                                        Dust.NewDust(arg_1F90_0, arg_1F90_1, arg_1F90_2, arg_1F90_3, arg_1F90_4, arg_1F90_5, arg_1F90_6, newColor, 1f);
                                                                                                        i++;
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    for (int i = 0; i < 20; i++)
                                                                                                    {
                                                                                                        Vector2 arg_1FF5_0 = this.position;
                                                                                                        int arg_1FF5_1 = this.width;
                                                                                                        int arg_1FF5_2 = this.height;
                                                                                                        int arg_1FF5_3 = 26;
                                                                                                        float arg_1FF5_4 = 2.5f * (float)hitDirection;
                                                                                                        float arg_1FF5_5 = -2.5f;
                                                                                                        int arg_1FF5_6 = 0;
                                                                                                        Color newColor = default(Color);
                                                                                                        Dust.NewDust(arg_1FF5_0, arg_1FF5_1, arg_1FF5_2, arg_1FF5_3, arg_1FF5_4, arg_1FF5_5, arg_1FF5_6, newColor, 1f);
                                                                                                    }
                                                                                                    Gore.NewGore(this.position, this.velocity, 42);
                                                                                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 43);
                                                                                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 43);
                                                                                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 44);
                                                                                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 44);
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (this.type == 39 || this.type == 40 || this.type == 41)
                                                                                                {
                                                                                                    if (this.life > 0)
                                                                                                    {
                                                                                                        int i = 0;
                                                                                                        while ((double)i < dmg / (double)this.lifeMax * 50.0)
                                                                                                        {
                                                                                                            Vector2 arg_2153_0 = this.position;
                                                                                                            int arg_2153_1 = this.width;
                                                                                                            int arg_2153_2 = this.height;
                                                                                                            int arg_2153_3 = 26;
                                                                                                            float arg_2153_4 = (float)hitDirection;
                                                                                                            float arg_2153_5 = -1f;
                                                                                                            int arg_2153_6 = 0;
                                                                                                            Color newColor = default(Color);
                                                                                                            Dust.NewDust(arg_2153_0, arg_2153_1, arg_2153_2, arg_2153_3, arg_2153_4, arg_2153_5, arg_2153_6, newColor, 1f);
                                                                                                            i++;
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        for (int i = 0; i < 20; i++)
                                                                                                        {
                                                                                                            Vector2 arg_21B5_0 = this.position;
                                                                                                            int arg_21B5_1 = this.width;
                                                                                                            int arg_21B5_2 = this.height;
                                                                                                            int arg_21B5_3 = 26;
                                                                                                            float arg_21B5_4 = 2.5f * (float)hitDirection;
                                                                                                            float arg_21B5_5 = -2.5f;
                                                                                                            int arg_21B5_6 = 0;
                                                                                                            Color newColor = default(Color);
                                                                                                            Dust.NewDust(arg_21B5_0, arg_21B5_1, arg_21B5_2, arg_21B5_3, arg_21B5_4, arg_21B5_5, arg_21B5_6, newColor, 1f);
                                                                                                        }
                                                                                                        Gore.NewGore(this.position, this.velocity, this.type - 39 + 67);
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (this.type == 34)
                                                                                                    {
                                                                                                        if (this.life > 0)
                                                                                                        {
                                                                                                            int i = 0;
                                                                                                            while ((double)i < dmg / (double)this.lifeMax * 50.0)
                                                                                                            {
                                                                                                                Vector2 arg_2282_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                int arg_2282_1 = this.width;
                                                                                                                int arg_2282_2 = this.height;
                                                                                                                int arg_2282_3 = 6;
                                                                                                                float arg_2282_4 = -this.velocity.X * 0.2f;
                                                                                                                float arg_2282_5 = -this.velocity.Y * 0.2f;
                                                                                                                int arg_2282_6 = 100;
                                                                                                                Color newColor = default(Color);
                                                                                                                int num4 = Dust.NewDust(arg_2282_0, arg_2282_1, arg_2282_2, arg_2282_3, arg_2282_4, arg_2282_5, arg_2282_6, newColor, 3f);
                                                                                                                Main.dust[num4].noLight = true;
                                                                                                                Main.dust[num4].noGravity = true;
                                                                                                                Dust expr_22AD = Main.dust[num4];
                                                                                                                expr_22AD.velocity *= 2f;
                                                                                                                Vector2 arg_231F_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                int arg_231F_1 = this.width;
                                                                                                                int arg_231F_2 = this.height;
                                                                                                                int arg_231F_3 = 6;
                                                                                                                float arg_231F_4 = -this.velocity.X * 0.2f;
                                                                                                                float arg_231F_5 = -this.velocity.Y * 0.2f;
                                                                                                                int arg_231F_6 = 100;
                                                                                                                newColor = default(Color);
                                                                                                                num4 = Dust.NewDust(arg_231F_0, arg_231F_1, arg_231F_2, arg_231F_3, arg_231F_4, arg_231F_5, arg_231F_6, newColor, 2f);
                                                                                                                Main.dust[num4].noLight = true;
                                                                                                                Dust expr_233C = Main.dust[num4];
                                                                                                                expr_233C.velocity *= 2f;
                                                                                                                i++;
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            for (int i = 0; i < 20; i++)
                                                                                                            {
                                                                                                                Vector2 arg_23E2_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                int arg_23E2_1 = this.width;
                                                                                                                int arg_23E2_2 = this.height;
                                                                                                                int arg_23E2_3 = 6;
                                                                                                                float arg_23E2_4 = -this.velocity.X * 0.2f;
                                                                                                                float arg_23E2_5 = -this.velocity.Y * 0.2f;
                                                                                                                int arg_23E2_6 = 100;
                                                                                                                Color newColor = default(Color);
                                                                                                                int num4 = Dust.NewDust(arg_23E2_0, arg_23E2_1, arg_23E2_2, arg_23E2_3, arg_23E2_4, arg_23E2_5, arg_23E2_6, newColor, 3f);
                                                                                                                Main.dust[num4].noLight = true;
                                                                                                                Main.dust[num4].noGravity = true;
                                                                                                                Dust expr_240D = Main.dust[num4];
                                                                                                                expr_240D.velocity *= 2f;
                                                                                                                Vector2 arg_247F_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                int arg_247F_1 = this.width;
                                                                                                                int arg_247F_2 = this.height;
                                                                                                                int arg_247F_3 = 6;
                                                                                                                float arg_247F_4 = -this.velocity.X * 0.2f;
                                                                                                                float arg_247F_5 = -this.velocity.Y * 0.2f;
                                                                                                                int arg_247F_6 = 100;
                                                                                                                newColor = default(Color);
                                                                                                                num4 = Dust.NewDust(arg_247F_0, arg_247F_1, arg_247F_2, arg_247F_3, arg_247F_4, arg_247F_5, arg_247F_6, newColor, 2f);
                                                                                                                Main.dust[num4].noLight = true;
                                                                                                                Dust expr_249C = Main.dust[num4];
                                                                                                                expr_249C.velocity *= 2f;
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (this.type == 35 || this.type == 36)
                                                                                                        {
                                                                                                            if (this.life > 0)
                                                                                                            {
                                                                                                                int i = 0;
                                                                                                                while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                                                                {
                                                                                                                    Vector2 arg_2532_0 = this.position;
                                                                                                                    int arg_2532_1 = this.width;
                                                                                                                    int arg_2532_2 = this.height;
                                                                                                                    int arg_2532_3 = 26;
                                                                                                                    float arg_2532_4 = (float)hitDirection;
                                                                                                                    float arg_2532_5 = -1f;
                                                                                                                    int arg_2532_6 = 0;
                                                                                                                    Color newColor = default(Color);
                                                                                                                    Dust.NewDust(arg_2532_0, arg_2532_1, arg_2532_2, arg_2532_3, arg_2532_4, arg_2532_5, arg_2532_6, newColor, 1f);
                                                                                                                    i++;
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                for (int i = 0; i < 150; i++)
                                                                                                                {
                                                                                                                    Vector2 arg_2597_0 = this.position;
                                                                                                                    int arg_2597_1 = this.width;
                                                                                                                    int arg_2597_2 = this.height;
                                                                                                                    int arg_2597_3 = 26;
                                                                                                                    float arg_2597_4 = 2.5f * (float)hitDirection;
                                                                                                                    float arg_2597_5 = -2.5f;
                                                                                                                    int arg_2597_6 = 0;
                                                                                                                    Color newColor = default(Color);
                                                                                                                    Dust.NewDust(arg_2597_0, arg_2597_1, arg_2597_2, arg_2597_3, arg_2597_4, arg_2597_5, arg_2597_6, newColor, 1f);
                                                                                                                }
                                                                                                                if (this.type == 35)
                                                                                                                {
                                                                                                                    Gore.NewGore(this.position, this.velocity, 54);
                                                                                                                    Gore.NewGore(this.position, this.velocity, 55);
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    Gore.NewGore(this.position, this.velocity, 56);
                                                                                                                    Gore.NewGore(this.position, this.velocity, 57);
                                                                                                                    Gore.NewGore(this.position, this.velocity, 57);
                                                                                                                    Gore.NewGore(this.position, this.velocity, 57);
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (this.type == 23)
                                                                                                            {
                                                                                                                if (this.life > 0)
                                                                                                                {
                                                                                                                    int i = 0;
                                                                                                                    while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                                                                    {
                                                                                                                        int num5 = 25;
                                                                                                                        if (Main.rand.Next(2) == 0)
                                                                                                                        {
                                                                                                                            num5 = 6;
                                                                                                                        }
                                                                                                                        Vector2 arg_26C8_0 = this.position;
                                                                                                                        int arg_26C8_1 = this.width;
                                                                                                                        int arg_26C8_2 = this.height;
                                                                                                                        int arg_26C8_3 = num5;
                                                                                                                        float arg_26C8_4 = (float)hitDirection;
                                                                                                                        float arg_26C8_5 = -1f;
                                                                                                                        int arg_26C8_6 = 0;
                                                                                                                        Color newColor = default(Color);
                                                                                                                        Dust.NewDust(arg_26C8_0, arg_26C8_1, arg_26C8_2, arg_26C8_3, arg_26C8_4, arg_26C8_5, arg_26C8_6, newColor, 1f);
                                                                                                                        Vector2 arg_2729_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                        int arg_2729_1 = this.width;
                                                                                                                        int arg_2729_2 = this.height;
                                                                                                                        int arg_2729_3 = 6;
                                                                                                                        float arg_2729_4 = this.velocity.X * 0.2f;
                                                                                                                        float arg_2729_5 = this.velocity.Y * 0.2f;
                                                                                                                        int arg_2729_6 = 100;
                                                                                                                        newColor = default(Color);
                                                                                                                        int num4 = Dust.NewDust(arg_2729_0, arg_2729_1, arg_2729_2, arg_2729_3, arg_2729_4, arg_2729_5, arg_2729_6, newColor, 2f);
                                                                                                                        Main.dust[num4].noGravity = true;
                                                                                                                        i++;
                                                                                                                    }
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    for (int i = 0; i < 50; i++)
                                                                                                                    {
                                                                                                                        int num5 = 25;
                                                                                                                        if (Main.rand.Next(2) == 0)
                                                                                                                        {
                                                                                                                            num5 = 6;
                                                                                                                        }
                                                                                                                        Vector2 arg_27BC_0 = this.position;
                                                                                                                        int arg_27BC_1 = this.width;
                                                                                                                        int arg_27BC_2 = this.height;
                                                                                                                        int arg_27BC_3 = num5;
                                                                                                                        float arg_27BC_4 = (float)(2 * hitDirection);
                                                                                                                        float arg_27BC_5 = -2f;
                                                                                                                        int arg_27BC_6 = 0;
                                                                                                                        Color newColor = default(Color);
                                                                                                                        Dust.NewDust(arg_27BC_0, arg_27BC_1, arg_27BC_2, arg_27BC_3, arg_27BC_4, arg_27BC_5, arg_27BC_6, newColor, 1f);
                                                                                                                    }
                                                                                                                    for (int i = 0; i < 50; i++)
                                                                                                                    {
                                                                                                                        Vector2 arg_2835_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                        int arg_2835_1 = this.width;
                                                                                                                        int arg_2835_2 = this.height;
                                                                                                                        int arg_2835_3 = 6;
                                                                                                                        float arg_2835_4 = this.velocity.X * 0.2f;
                                                                                                                        float arg_2835_5 = this.velocity.Y * 0.2f;
                                                                                                                        int arg_2835_6 = 100;
                                                                                                                        Color newColor = default(Color);
                                                                                                                        int num4 = Dust.NewDust(arg_2835_0, arg_2835_1, arg_2835_2, arg_2835_3, arg_2835_4, arg_2835_5, arg_2835_6, newColor, 2.5f);
                                                                                                                        Dust expr_2844 = Main.dust[num4];
                                                                                                                        expr_2844.velocity *= 6f;
                                                                                                                        Main.dust[num4].noGravity = true;
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (this.type == 24)
                                                                                                                {
                                                                                                                    if (this.life > 0)
                                                                                                                    {
                                                                                                                        int i = 0;
                                                                                                                        while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                                                                        {
                                                                                                                            Vector2 arg_2902_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                            int arg_2902_1 = this.width;
                                                                                                                            int arg_2902_2 = this.height;
                                                                                                                            int arg_2902_3 = 6;
                                                                                                                            float arg_2902_4 = this.velocity.X;
                                                                                                                            float arg_2902_5 = this.velocity.Y;
                                                                                                                            int arg_2902_6 = 100;
                                                                                                                            Color newColor = default(Color);
                                                                                                                            int num4 = Dust.NewDust(arg_2902_0, arg_2902_1, arg_2902_2, arg_2902_3, arg_2902_4, arg_2902_5, arg_2902_6, newColor, 2.5f);
                                                                                                                            Main.dust[num4].noGravity = true;
                                                                                                                            i++;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        for (int i = 0; i < 50; i++)
                                                                                                                        {
                                                                                                                            Vector2 arg_299A_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                            int arg_299A_1 = this.width;
                                                                                                                            int arg_299A_2 = this.height;
                                                                                                                            int arg_299A_3 = 6;
                                                                                                                            float arg_299A_4 = this.velocity.X;
                                                                                                                            float arg_299A_5 = this.velocity.Y;
                                                                                                                            int arg_299A_6 = 100;
                                                                                                                            Color newColor = default(Color);
                                                                                                                            int num4 = Dust.NewDust(arg_299A_0, arg_299A_1, arg_299A_2, arg_299A_3, arg_299A_4, arg_299A_5, arg_299A_6, newColor, 2.5f);
                                                                                                                            Main.dust[num4].noGravity = true;
                                                                                                                            Dust expr_29B7 = Main.dust[num4];
                                                                                                                            expr_29B7.velocity *= 2f;
                                                                                                                        }
                                                                                                                        Gore.NewGore(this.position, this.velocity, 45);
                                                                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 46);
                                                                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 46);
                                                                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 47);
                                                                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 47);
                                                                                                                    }
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (this.type == 25)
                                                                                                                    {
                                                                                                                        //Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
                                                                                                                        for (int k = 0; k < 20; k++)
                                                                                                                        {
                                                                                                                            Vector2 arg_2B54_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                            int arg_2B54_1 = this.width;
                                                                                                                            int arg_2B54_2 = this.height;
                                                                                                                            int arg_2B54_3 = 6;
                                                                                                                            float arg_2B54_4 = -this.velocity.X * 0.2f;
                                                                                                                            float arg_2B54_5 = -this.velocity.Y * 0.2f;
                                                                                                                            int arg_2B54_6 = 100;
                                                                                                                            Color newColor = default(Color);
                                                                                                                            int num4 = Dust.NewDust(arg_2B54_0, arg_2B54_1, arg_2B54_2, arg_2B54_3, arg_2B54_4, arg_2B54_5, arg_2B54_6, newColor, 2f);
                                                                                                                            Main.dust[num4].noGravity = true;
                                                                                                                            Dust expr_2B71 = Main.dust[num4];
                                                                                                                            expr_2B71.velocity *= 2f;
                                                                                                                            Vector2 arg_2BE3_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                            int arg_2BE3_1 = this.width;
                                                                                                                            int arg_2BE3_2 = this.height;
                                                                                                                            int arg_2BE3_3 = 6;
                                                                                                                            float arg_2BE3_4 = -this.velocity.X * 0.2f;
                                                                                                                            float arg_2BE3_5 = -this.velocity.Y * 0.2f;
                                                                                                                            int arg_2BE3_6 = 100;
                                                                                                                            newColor = default(Color);
                                                                                                                            num4 = Dust.NewDust(arg_2BE3_0, arg_2BE3_1, arg_2BE3_2, arg_2BE3_3, arg_2BE3_4, arg_2BE3_5, arg_2BE3_6, newColor, 1f);
                                                                                                                            Dust expr_2BF2 = Main.dust[num4];
                                                                                                                            expr_2BF2.velocity *= 2f;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (this.type == 33)
                                                                                                                        {
                                                                                                                            //Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
                                                                                                                            for (int k = 0; k < 20; k++)
                                                                                                                            {
                                                                                                                                Vector2 arg_2CC2_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                int arg_2CC2_1 = this.width;
                                                                                                                                int arg_2CC2_2 = this.height;
                                                                                                                                int arg_2CC2_3 = 29;
                                                                                                                                float arg_2CC2_4 = -this.velocity.X * 0.2f;
                                                                                                                                float arg_2CC2_5 = -this.velocity.Y * 0.2f;
                                                                                                                                int arg_2CC2_6 = 100;
                                                                                                                                Color newColor = default(Color);
                                                                                                                                int num4 = Dust.NewDust(arg_2CC2_0, arg_2CC2_1, arg_2CC2_2, arg_2CC2_3, arg_2CC2_4, arg_2CC2_5, arg_2CC2_6, newColor, 2f);
                                                                                                                                Main.dust[num4].noGravity = true;
                                                                                                                                Dust expr_2CDF = Main.dust[num4];
                                                                                                                                expr_2CDF.velocity *= 2f;
                                                                                                                                Vector2 arg_2D52_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                int arg_2D52_1 = this.width;
                                                                                                                                int arg_2D52_2 = this.height;
                                                                                                                                int arg_2D52_3 = 29;
                                                                                                                                float arg_2D52_4 = -this.velocity.X * 0.2f;
                                                                                                                                float arg_2D52_5 = -this.velocity.Y * 0.2f;
                                                                                                                                int arg_2D52_6 = 100;
                                                                                                                                newColor = default(Color);
                                                                                                                                num4 = Dust.NewDust(arg_2D52_0, arg_2D52_1, arg_2D52_2, arg_2D52_3, arg_2D52_4, arg_2D52_5, arg_2D52_6, newColor, 1f);
                                                                                                                                Dust expr_2D61 = Main.dust[num4];
                                                                                                                                expr_2D61.velocity *= 2f;
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (this.type == 26 || this.type == 27 || this.type == 28 || this.type == 29)
                                                                                                                            {
                                                                                                                                if (this.life > 0)
                                                                                                                                {
                                                                                                                                    int i = 0;
                                                                                                                                    while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                                                                                    {
                                                                                                                                        Vector2 arg_2E0C_0 = this.position;
                                                                                                                                        int arg_2E0C_1 = this.width;
                                                                                                                                        int arg_2E0C_2 = this.height;
                                                                                                                                        int arg_2E0C_3 = 5;
                                                                                                                                        float arg_2E0C_4 = (float)hitDirection;
                                                                                                                                        float arg_2E0C_5 = -1f;
                                                                                                                                        int arg_2E0C_6 = 0;
                                                                                                                                        Color newColor = default(Color);
                                                                                                                                        Dust.NewDust(arg_2E0C_0, arg_2E0C_1, arg_2E0C_2, arg_2E0C_3, arg_2E0C_4, arg_2E0C_5, arg_2E0C_6, newColor, 1f);
                                                                                                                                        i++;
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    for (int i = 0; i < 50; i++)
                                                                                                                                    {
                                                                                                                                        Vector2 arg_2E70_0 = this.position;
                                                                                                                                        int arg_2E70_1 = this.width;
                                                                                                                                        int arg_2E70_2 = this.height;
                                                                                                                                        int arg_2E70_3 = 5;
                                                                                                                                        float arg_2E70_4 = 2.5f * (float)hitDirection;
                                                                                                                                        float arg_2E70_5 = -2.5f;
                                                                                                                                        int arg_2E70_6 = 0;
                                                                                                                                        Color newColor = default(Color);
                                                                                                                                        Dust.NewDust(arg_2E70_0, arg_2E70_1, arg_2E70_2, arg_2E70_3, arg_2E70_4, arg_2E70_5, arg_2E70_6, newColor, 1f);
                                                                                                                                    }
                                                                                                                                    Gore.NewGore(this.position, this.velocity, 48);
                                                                                                                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 49);
                                                                                                                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 49);
                                                                                                                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 50);
                                                                                                                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 50);
                                                                                                                                }
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (this.type == 30)
                                                                                                                                {
                                                                                                                                    //Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
                                                                                                                                    for (int k = 0; k < 20; k++)
                                                                                                                                    {
                                                                                                                                        Vector2 arg_2FFC_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                        int arg_2FFC_1 = this.width;
                                                                                                                                        int arg_2FFC_2 = this.height;
                                                                                                                                        int arg_2FFC_3 = 27;
                                                                                                                                        float arg_2FFC_4 = -this.velocity.X * 0.2f;
                                                                                                                                        float arg_2FFC_5 = -this.velocity.Y * 0.2f;
                                                                                                                                        int arg_2FFC_6 = 100;
                                                                                                                                        Color newColor = default(Color);
                                                                                                                                        int num4 = Dust.NewDust(arg_2FFC_0, arg_2FFC_1, arg_2FFC_2, arg_2FFC_3, arg_2FFC_4, arg_2FFC_5, arg_2FFC_6, newColor, 2f);
                                                                                                                                        Main.dust[num4].noGravity = true;
                                                                                                                                        Dust expr_3019 = Main.dust[num4];
                                                                                                                                        expr_3019.velocity *= 2f;
                                                                                                                                        Vector2 arg_308C_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                        int arg_308C_1 = this.width;
                                                                                                                                        int arg_308C_2 = this.height;
                                                                                                                                        int arg_308C_3 = 27;
                                                                                                                                        float arg_308C_4 = -this.velocity.X * 0.2f;
                                                                                                                                        float arg_308C_5 = -this.velocity.Y * 0.2f;
                                                                                                                                        int arg_308C_6 = 100;
                                                                                                                                        newColor = default(Color);
                                                                                                                                        num4 = Dust.NewDust(arg_308C_0, arg_308C_1, arg_308C_2, arg_308C_3, arg_308C_4, arg_308C_5, arg_308C_6, newColor, 1f);
                                                                                                                                        Dust expr_309B = Main.dust[num4];
                                                                                                                                        expr_309B.velocity *= 2f;
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    if (this.type == 42)
                                                                                                                                    {
                                                                                                                                        if (this.life > 0)
                                                                                                                                        {
                                                                                                                                            int i = 0;
                                                                                                                                            while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                                                                                            {
                                                                                                                                                Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
                                                                                                                                                i++;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            for (int i = 0; i < 50; i++)
                                                                                                                                            {
                                                                                                                                                Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
                                                                                                                                            }
                                                                                                                                            Gore.NewGore(this.position, this.velocity, 70);
                                                                                                                                            Gore.NewGore(this.position, this.velocity, 71);
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (this.type == 43 || this.type == 56)
                                                                                                                                        {
                                                                                                                                            if (this.life > 0)
                                                                                                                                            {
                                                                                                                                                int i = 0;
                                                                                                                                                while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                                                                                                {
                                                                                                                                                    Dust.NewDust(this.position, this.width, this.height, 40, (float)hitDirection, -1f, this.alpha, this.color, 1.2f);
                                                                                                                                                    i++;
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                for (int i = 0; i < 50; i++)
                                                                                                                                                {
                                                                                                                                                    Dust.NewDust(this.position, this.width, this.height, 40, (float)hitDirection, -2f, this.alpha, this.color, 1.2f);
                                                                                                                                                }
                                                                                                                                                Gore.NewGore(this.position, this.velocity, 72);
                                                                                                                                                Gore.NewGore(this.position, this.velocity, 72);
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (this.type == 48)
                                                                                                                                            {
                                                                                                                                                if (this.life > 0)
                                                                                                                                                {
                                                                                                                                                    int i = 0;
                                                                                                                                                    while ((double)i < dmg / (double)this.lifeMax * 100.0)
                                                                                                                                                    {
                                                                                                                                                        Vector2 arg_332E_0 = this.position;
                                                                                                                                                        int arg_332E_1 = this.width;
                                                                                                                                                        int arg_332E_2 = this.height;
                                                                                                                                                        int arg_332E_3 = 5;
                                                                                                                                                        float arg_332E_4 = (float)hitDirection;
                                                                                                                                                        float arg_332E_5 = -1f;
                                                                                                                                                        int arg_332E_6 = 0;
                                                                                                                                                        Color newColor = default(Color);
                                                                                                                                                        Dust.NewDust(arg_332E_0, arg_332E_1, arg_332E_2, arg_332E_3, arg_332E_4, arg_332E_5, arg_332E_6, newColor, 1f);
                                                                                                                                                        i++;
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    for (int i = 0; i < 50; i++)
                                                                                                                                                    {
                                                                                                                                                        Vector2 arg_338B_0 = this.position;
                                                                                                                                                        int arg_338B_1 = this.width;
                                                                                                                                                        int arg_338B_2 = this.height;
                                                                                                                                                        int arg_338B_3 = 5;
                                                                                                                                                        float arg_338B_4 = (float)(2 * hitDirection);
                                                                                                                                                        float arg_338B_5 = -2f;
                                                                                                                                                        int arg_338B_6 = 0;
                                                                                                                                                        Color newColor = default(Color);
                                                                                                                                                        Dust.NewDust(arg_338B_0, arg_338B_1, arg_338B_2, arg_338B_3, arg_338B_4, arg_338B_5, arg_338B_6, newColor, 1f);
                                                                                                                                                    }
                                                                                                                                                    Gore.NewGore(this.position, this.velocity, 80);
                                                                                                                                                    Gore.NewGore(this.position, this.velocity, 81);
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
                if (Main.netMode != 1)
                {
                    if (Main.bloodMoon)
                    {
                        if (this.type == 46)
                        {
                            this.Transform(47);
                        }
                        else
                        {
                            if (this.type == 55)
                            {
                                this.Transform(57);
                            }
                        }
                    }
                }
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
                //if (this.type == 44)
                //{
                //    Lighting.addLight((int)(this.position.X + (float)(this.width / 2)) / 16, (int)(this.position.Y + 4f) / 16, 0.6f);
                //}
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
                        for (int j = 0; j < 1000; j++)
                        {
                            if (Main.npc[j].active && !Main.npc[j].friendly && Main.npc[j].damage > 0)
                            {
                                Rectangle rectangle2 = new Rectangle((int)Main.npc[j].position.X, (int)Main.npc[j].position.Y, Main.npc[j].width, Main.npc[j].height);
                                if (rectangle.Intersects(rectangle2))
                                {
                                    int num3 = Main.npc[j].damage;
                                    int num4 = 6;
                                    int num5 = 1;
                                    if (Main.npc[j].position.X + (float)(Main.npc[j].width / 2) > this.position.X + (float)(this.width / 2))
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
                        if (Main.netMode != 1)
                        {
                            if (this.immune[255] == 0)
                            {
                                this.immune[255] = 30;
                                this.StrikeNPC(50, 0f, 0);
                                if (Main.netMode == 2)
                                {
                                    if (Main.netMode != 0)
                                    {
                                        NetMessage.SendData(28, -1, -1, "", this.whoAmI, 50f, 0f, 0f);
                                    }
                                }
                            }
                        }
                    }
                    bool flag2 = Collision.WetCollision(this.position, this.width, this.height);
                    if (flag2)
                    {
                        if (!this.wet)
                        {
                            if (this.wetCount == 0)
                            {
                                this.wetCount = 10;
                                if (!flag)
                                {
                                    for (int k = 0; k < 50; k++)
                                    {
                                        Vector2 arg_5ED_0 = new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f);
                                        int arg_5ED_1 = this.width + 12;
                                        int arg_5ED_2 = 24;
                                        int arg_5ED_3 = 33;
                                        float arg_5ED_4 = 0f;
                                        float arg_5ED_5 = 0f;
                                        int arg_5ED_6 = 0;
                                        Color newColor = default(Color);
                                        int num6 = Dust.NewDust(arg_5ED_0, arg_5ED_1, arg_5ED_2, arg_5ED_3, arg_5ED_4, arg_5ED_5, arg_5ED_6, newColor, 1f);
                                        Dust expr_601_cp_0 = Main.dust[num6];
                                        expr_601_cp_0.velocity.Y = expr_601_cp_0.velocity.Y - 4f;
                                        Dust expr_61F_cp_0 = Main.dust[num6];
                                        expr_61F_cp_0.velocity.X = expr_61F_cp_0.velocity.X * 2.5f;
                                        Main.dust[num6].scale = 1.3f;
                                        Main.dust[num6].alpha = 100;
                                        Main.dust[num6].noGravity = true;
                                    }
                                    //Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 0);
                                }
                                else
                                {
                                    for (int k = 0; k < 20; k++)
                                    {
                                        Vector2 arg_6FE_0 = new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f);
                                        int arg_6FE_1 = this.width + 12;
                                        int arg_6FE_2 = 24;
                                        int arg_6FE_3 = 35;
                                        float arg_6FE_4 = 0f;
                                        float arg_6FE_5 = 0f;
                                        int arg_6FE_6 = 0;
                                        Color newColor = default(Color);
                                        int num6 = Dust.NewDust(arg_6FE_0, arg_6FE_1, arg_6FE_2, arg_6FE_3, arg_6FE_4, arg_6FE_5, arg_6FE_6, newColor, 1f);
                                        Dust expr_712_cp_0 = Main.dust[num6];
                                        expr_712_cp_0.velocity.Y = expr_712_cp_0.velocity.Y - 1.5f;
                                        Dust expr_730_cp_0 = Main.dust[num6];
                                        expr_730_cp_0.velocity.X = expr_730_cp_0.velocity.X * 2.5f;
                                        Main.dust[num6].scale = 1.3f;
                                        Main.dust[num6].alpha = 100;
                                        Main.dust[num6].noGravity = true;
                                    }
                                    //Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
                                }
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
                                    for (int k = 0; k < 50; k++)
                                    {
                                        Vector2 arg_871_0 = new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2));
                                        int arg_871_1 = this.width + 12;
                                        int arg_871_2 = 24;
                                        int arg_871_3 = 33;
                                        float arg_871_4 = 0f;
                                        float arg_871_5 = 0f;
                                        int arg_871_6 = 0;
                                        Color newColor = default(Color);
                                        int num6 = Dust.NewDust(arg_871_0, arg_871_1, arg_871_2, arg_871_3, arg_871_4, arg_871_5, arg_871_6, newColor, 1f);
                                        Dust expr_885_cp_0 = Main.dust[num6];
                                        expr_885_cp_0.velocity.Y = expr_885_cp_0.velocity.Y - 4f;
                                        Dust expr_8A3_cp_0 = Main.dust[num6];
                                        expr_8A3_cp_0.velocity.X = expr_8A3_cp_0.velocity.X * 2.5f;
                                        Main.dust[num6].scale = 1.3f;
                                        Main.dust[num6].alpha = 100;
                                        Main.dust[num6].noGravity = true;
                                    }
                                    //Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 0);
                                }
                                else
                                {
                                    for (int k = 0; k < 20; k++)
                                    {
                                        Vector2 arg_982_0 = new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f);
                                        int arg_982_1 = this.width + 12;
                                        int arg_982_2 = 24;
                                        int arg_982_3 = 35;
                                        float arg_982_4 = 0f;
                                        float arg_982_5 = 0f;
                                        int arg_982_6 = 0;
                                        Color newColor = default(Color);
                                        int num6 = Dust.NewDust(arg_982_0, arg_982_1, arg_982_2, arg_982_3, arg_982_4, arg_982_5, arg_982_6, newColor, 1f);
                                        Dust expr_996_cp_0 = Main.dust[num6];
                                        expr_996_cp_0.velocity.Y = expr_996_cp_0.velocity.Y - 1.5f;
                                        Dust expr_9B4_cp_0 = Main.dust[num6];
                                        expr_9B4_cp_0.velocity.X = expr_9B4_cp_0.velocity.X * 2.5f;
                                        Main.dust[num6].scale = 1.3f;
                                        Main.dust[num6].alpha = 100;
                                        Main.dust[num6].noGravity = true;
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
                        int num = Main.rand.Next(2);
                        if (num == 0)
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
                            int num = Main.rand.Next(2);
                            if (num == 0)
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
                        int num = Main.rand.Next(2);
                        if (num == 0)
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
                            int num = Main.rand.Next(2);
                            if (num == 0)
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
                                int num = Main.rand.Next(2);
                                if (num == 0)
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
                                int num = Main.rand.Next(3);
                                if (num == 0)
                                {
                                    result = "The last guy who was here left me some junk..er I mean.. treasures!";
                                }
                                if (num == 1)
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
                            int num = Main.rand.Next(5);
                            if (num == 0)
                            {
                                result = "I think you look better this way.";
                            }
                            else
                            {
                                if (num == 1)
                                {
                                    result = "Eww.. What happened to your face?";
                                }
                                else
                                {
                                    if (num == 2)
                                    {
                                        result = "MY GOODNESS! I'm good but I'm not THAT good.";
                                    }
                                    else
                                    {
                                        if (num == 3)
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
                                int num = Main.rand.Next(4);
                                if (num == 0)
                                {
                                    result = "Quit being such a baby! I've seen worse.";
                                }
                                else
                                {
                                    if (num == 1)
                                    {
                                        result = "That's gonna need stitches!";
                                    }
                                    else
                                    {
                                        if (num == 2)
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
                                int num = Main.rand.Next(3);
                                if (num == 0)
                                {
                                    result = "Turn your head and cough.";
                                }
                                else
                                {
                                    if (num == 1)
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
                                        int num = Main.rand.Next(2);
                                        if (num == 0)
                                        {
                                            result = "I see you're eyeballin' the Minishark.. You really don't want to know how it was made.";
                                        }
                                        else
                                        {
                                            if (num == 1)
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
                                            int num = Main.rand.Next(6);
                                            if (num == 0)
                                            {
                                                result = "You must cleanse the world of this corruption.";
                                            }
                                            else
                                            {
                                                if (num == 1)
                                                {
                                                    result = "Be safe; Terraria needs you!";
                                                }
                                                else
                                                {
                                                    if (num == 2)
                                                    {
                                                        result = "The sands of time are flowing. And well, you are not aging very gracefully.";
                                                    }
                                                    else
                                                    {
                                                        if (num == 3)
                                                        {
                                                            result = "Whats this about me having more 'bark' than bite?";
                                                        }
                                                        else
                                                        {
                                                            if (num == 4)
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
                                        int num = Main.rand.Next(3);
                                        if (num == 0)
                                        {
                                            result = "Greetings, " + Main.player[Main.myPlayer].name + ". Is there something I can help you with?";
                                        }
                                        else
                                        {
                                            if (num == 1)
                                            {
                                                result = "I am here to give you advice on what to do next.  It is recommended that you talk with me anytime you get stuck.";
                                            }
                                            else
                                            {
                                                if (num == 2)
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
                                        int num = Main.rand.Next(2);
                                        if (num == 0)
                                        {
                                            result = "I cannot let you enter until you free me of my curse.";
                                        }
                                        else
                                        {
                                            if (num == 1)
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
                                                        int num = Main.rand.Next(4);
                                                        if (num == 0)
                                                        {
                                                            result = "Explosives are da' bomb these days.  Buy some now!";
                                                        }
                                                        else
                                                        {
                                                            if (num == 1)
                                                            {
                                                                result = "It's a good day to die!";
                                                            }
                                                            else
                                                            {
                                                                if (num == 2)
                                                                {
                                                                    result = "I wonder what happens if I... (BOOM!)... Oh, sorry, did you need that leg?";
                                                                }
                                                                else
                                                                {
                                                                    if (num == 3)
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
                                        if (this.type == 54)
                                        {
                                            if (Main.bloodMoon)
                                            {
                                                result = Main.player[Main.myPlayer].name + "... we have a problem! Its a blood moon out there!";
                                            }
                                            else
                                            {
                                                if (flag2 && Main.rand.Next(4) == 0)
                                                {
                                                    result = "T'were I younger, I would ask the nurse out. I used to be quite the lady killer.";
                                                }
                                                else
                                                {
                                                    if (Main.player[Main.myPlayer].head == 24)
                                                    {
                                                        result = "That Red Hat of yours looks familiar...";
                                                    }
                                                    else
                                                    {
                                                        int num = Main.rand.Next(4);
                                                        if (num == 0)
                                                        {
                                                            result = "Thanks again for freeing me from my curse. Felt like something jumped up and bit me";
                                                        }
                                                        else
                                                        {
                                                            if (num == 1)
                                                            {
                                                                result = "Mama always said I would make a great tailor.";
                                                            }
                                                            else
                                                            {
                                                                if (num == 2)
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
    }
}
