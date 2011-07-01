using System;
using Terraria_Server.Misc;
using Terraria_Server.Plugin;

namespace Terraria_Server
{
    public class NPC
    {
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
        private static float npcSlots = 1f;
        private static bool noSpawnCycle = false;
        private static int activeTime = 750;
        public static int defaultSpawnRate = 600;
        public static int defaultMaxSpawns = 5;
        public bool wet;
        public byte wetCount;
        public bool lavaWet;
        public static bool downedBoss1 = false;
        public static bool downedBoss2 = false;
        public static bool downedBoss3 = false;
        public static int spawnRate = NPC.defaultSpawnRate;
        public static int maxSpawns = NPC.defaultMaxSpawns;
        public int soundDelay;
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
        public float[] ai = new float[NPC.MAX_AI];
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
        public Rectangle targetRect;
        public double frameCounter;
        public Rectangle frame;
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
        public bool lavaImmune;
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

        private int SetGore(int goreType)
        {
            return Gore.NewGore(this.position, this.velocity, goreType);
        }

        public void SetDefaults(string Name)
        {
            this.SetDefaults(0);
            if (Name == "Green Slime")
            {
                this.SetDefaults(1);
                this.name = Name;
                this.scale = 0.9f;
                this.damage = 6;
                this.defense = 0;
                this.life = 14;
                this.knockBackResist = 1.2f;
                this.color = new Color(0, 220, 40, 100);
                this.value = 3f;
            }
            else if (Name == "Pinky")
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
            else if (Name == "Baby Slime")
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
            else if (Name == "Black Slime")
            {
                this.SetDefaults(1);
                this.name = Name;
                this.damage = 15;
                this.defense = 4;
                this.life = 45;
                this.color = new Color(0, 0, 0, 50);
                this.value = 20f;
            }
            else if (Name == "Purple Slime")
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
            else if (Name == "Red Slime")
            {
                this.SetDefaults(1);
                this.name = Name;
                this.damage = 12;
                this.defense = 4;
                this.life = 35;
                this.color = new Color(255, 30, 0, 100);
                this.value = 8f;
            }
            else if (Name == "Yellow Slime")
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
            else if (Name == "Jungle Slime")
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
            else if (Name == "Little Eater")
            {
                this.SetDefaults(6);
                this.name = Name;
                this.scale = 0.85f;
                this.defense = (int)((float)this.defense * this.scale);
                this.damage = (int)((float)this.damage * this.scale);
                this.life = (int)((float)this.life * this.scale);
                this.value = (float)((int)(this.value * this.scale));
                NPC.npcSlots *= this.scale;
                this.knockBackResist *= 2f - this.scale;
            }
            else if (Name == "Big Eater")
            {
                this.SetDefaults(6);
                this.name = Name;
                this.scale = 1.15f;
                this.defense = (int)((float)this.defense * this.scale);
                this.damage = (int)((float)this.damage * this.scale);
                this.life = (int)((float)this.life * this.scale);
                this.value = (float)((int)(this.value * this.scale));
                NPC.npcSlots *= this.scale;
                this.knockBackResist *= 2f - this.scale;
            }
            else if (Name == "Short Bones")
            {
                this.SetDefaults(31);
                this.name = Name;
                this.scale = 0.9f;
                this.defense = (int)((float)this.defense * this.scale);
                this.damage = (int)((float)this.damage * this.scale);
                this.life = (int)((float)this.life * this.scale);
                this.value = (float)((int)(this.value * this.scale));
            }
            else if (Name == "Big Boned")
            {
                this.SetDefaults(31);
                this.name = Name;
                this.scale = 1.15f;
                this.defense = (int)((float)this.defense * this.scale);
                this.damage = (int)((double)((float)this.damage * this.scale) * 1.1);
                this.life = (int)((double)((float)this.life * this.scale) * 1.1);
                this.value = (float)((int)(this.value * this.scale));
                NPC.npcSlots = 2f;
                this.knockBackResist *= 2f - this.scale;
            }
            else if (Name == "Little Stinger")
            {
                this.SetDefaults(42);
                this.name = Name;
                this.scale = 0.85f;
                this.defense = (int)((float)this.defense * this.scale);
                this.damage = (int)((float)this.damage * this.scale);
                this.life = (int)((float)this.life * this.scale);
                this.value = (float)((int)(this.value * this.scale));
                NPC.npcSlots *= this.scale;
                this.knockBackResist *= 2f - this.scale;
            }
            else if (Name == "Big Stinger")
            {
                this.SetDefaults(42);
                this.name = Name;
                this.scale = 1.15f;
                this.defense = (int)((float)this.defense * this.scale);
                this.damage = (int)((float)this.damage * this.scale);
                this.life = (int)((float)this.life * this.scale);
                this.value = (float)((int)(this.value * this.scale));
                NPC.npcSlots *= this.scale;
                this.knockBackResist *= 2f - this.scale;
            }
            else if (Name != "")
            {
                for (int i = 1; i < 70; i++)
                {
                    this.SetDefaults(i);

                    if (this.name == Name)
                    {
                        break;
                    }

                    if (i == 69)
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

            this.lifeMax = this.life;
        }
       
        public void SetDefaults(int Type)
        {
            NPC.npcSlots = 1f;
            this.lavaImmune = false;
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

            for (int i = 0; i < NPC.MAX_AI; i++)
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
            else
            {
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
                else
                {
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
                    else
                    {
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
                        else
                        {
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
                            else
                            {
                                if (this.type == 6)
                                {
                                    NPC.npcSlots = 0.75f;
                                    this.name = "Eater of Souls";
                                    this.width = 30;
                                    this.height = 30;
                                    this.aiStyle = 5;
                                    this.damage = 23;
                                    this.defense = 8;
                                    this.lifeMax = 40;
                                    this.soundHit = 1;
                                    this.soundKilled = 1;
                                    this.noGravity = true;
                                    this.knockBackResist = 0.5f;
                                    this.value = 90f;
                                }
                                else
                                {
                                    if (this.type == 7)
                                    {
                                        NPC.npcSlots = 3.5f;
                                        this.name = "Devourer Head";
                                        this.width = 22;
                                        this.height = 22;
                                        this.aiStyle = 6;
                                        this.damage = 32;
                                        this.defense = 2;
                                        this.lifeMax = 30;
                                        this.soundHit = 1;
                                        this.soundKilled = 1;
                                        this.noGravity = true;
                                        this.noTileCollide = true;
                                        this.knockBackResist = 0f;
                                        this.behindTiles = true;
                                        this.value = 140f;
                                    }
                                    else
                                    {
                                        if (this.type == 8)
                                        {
                                            this.name = "Devourer Body";
                                            this.width = 22;
                                            this.height = 22;
                                            this.aiStyle = 6;
                                            this.damage = 16;
                                            this.defense = 6;
                                            this.lifeMax = 45;
                                            this.soundHit = 1;
                                            this.soundKilled = 1;
                                            this.noGravity = true;
                                            this.noTileCollide = true;
                                            this.knockBackResist = 0f;
                                            this.behindTiles = true;
                                            this.value = 140f;
                                        }
                                        else
                                        {
                                            if (this.type == 9)
                                            {
                                                this.name = "Devourer Tail";
                                                this.width = 22;
                                                this.height = 22;
                                                this.aiStyle = 6;
                                                this.damage = 13;
                                                this.defense = 10;
                                                this.lifeMax = 70;
                                                this.soundHit = 1;
                                                this.soundKilled = 1;
                                                this.noGravity = true;
                                                this.noTileCollide = true;
                                                this.knockBackResist = 0f;
                                                this.behindTiles = true;
                                                this.value = 140f;
                                            }
                                            else
                                            {
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
                                                    this.value = 40f;
                                                }
                                                else
                                                {
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
                                                        this.value = 40f;
                                                    }
                                                    else
                                                    {
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
                                                            this.value = 40f;
                                                        }
                                                        else
                                                        {
                                                            if (this.type == 13)
                                                            {
                                                                NPC.npcSlots = 8f;
                                                                this.name = "Eater of Worlds Head";
                                                                this.width = 38;
                                                                this.height = 38;
                                                                this.aiStyle = 6;
                                                                this.damage = 43;
                                                                this.defense = 0;
                                                                this.lifeMax = 80;
                                                                this.soundHit = 1;
                                                                this.soundKilled = 1;
                                                                this.noGravity = true;
                                                                this.noTileCollide = true;
                                                                this.knockBackResist = 0f;
                                                                this.behindTiles = true;
                                                                this.value = 300f;
                                                                this.scale = 1.1f;
                                                            }
                                                            else
                                                            {
                                                                if (this.type == 14)
                                                                {
                                                                    this.name = "Eater of Worlds Body";
                                                                    this.width = 38;
                                                                    this.height = 38;
                                                                    this.aiStyle = 6;
                                                                    this.damage = 18;
                                                                    this.defense = 6;
                                                                    this.lifeMax = 210;
                                                                    this.soundHit = 1;
                                                                    this.soundKilled = 1;
                                                                    this.noGravity = true;
                                                                    this.noTileCollide = true;
                                                                    this.knockBackResist = 0f;
                                                                    this.behindTiles = true;
                                                                    this.value = 300f;
                                                                    this.scale = 1.1f;
                                                                }
                                                                else
                                                                {
                                                                    if (this.type == 15)
                                                                    {
                                                                        this.name = "Eater of Worlds Tail";
                                                                        this.width = 38;
                                                                        this.height = 38;
                                                                        this.aiStyle = 6;
                                                                        this.damage = 11;
                                                                        this.defense = 7;
                                                                        this.lifeMax = 300;
                                                                        this.soundHit = 1;
                                                                        this.soundKilled = 1;
                                                                        this.noGravity = true;
                                                                        this.noTileCollide = true;
                                                                        this.knockBackResist = 0f;
                                                                        this.behindTiles = true;
                                                                        this.value = 300f;
                                                                        this.scale = 1.1f;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (this.type == 16)
                                                                        {
                                                                            NPC.npcSlots = 2f;
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
                                                                        else
                                                                        {
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
                                                                            else
                                                                            {
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
                                                                                else
                                                                                {
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
                                                                                    else
                                                                                    {
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
                                                                                        else
                                                                                        {
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
                                                                                                this.value = 100f;
                                                                                            }
                                                                                            else
                                                                                            {
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
                                                                                                else
                                                                                                {
                                                                                                    if (this.type == 23)
                                                                                                    {
                                                                                                        this.name = "Meteor Head";
                                                                                                        this.width = 22;
                                                                                                        this.height = 22;
                                                                                                        this.aiStyle = 5;
                                                                                                        this.damage = 40;
                                                                                                        this.defense = 6;
                                                                                                        this.lifeMax = 26;
                                                                                                        this.soundHit = 3;
                                                                                                        this.soundKilled = 3;
                                                                                                        this.noGravity = true;
                                                                                                        this.noTileCollide = true;
                                                                                                        this.value = 80f;
                                                                                                        this.knockBackResist = 0.4f;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (this.type == 24)
                                                                                                        {
                                                                                                            NPC.npcSlots = 3f;
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
                                                                                                            this.lavaImmune = true;
                                                                                                            this.value = 400f;
                                                                                                        }
                                                                                                        else
                                                                                                        {
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
                                                                                                            else
                                                                                                            {
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
                                                                                                                    this.value = 100f;
                                                                                                                }
                                                                                                                else
                                                                                                                {
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
                                                                                                                        this.value = 200f;
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
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
                                                                                                                            this.value = 150f;
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
                                                                                                                                this.value = 200f;
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
                                                                                                                                        this.knockBackResist = 0.6f;
                                                                                                                                        this.value = 130f;
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
                                                                                                                                            this.value = 140f;
                                                                                                                                            NPC.npcSlots = 2f;
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
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (this.type == 34)
                                                                                                                                                {
                                                                                                                                                    this.name = "Cursed Skull";
                                                                                                                                                    this.width = 26;
                                                                                                                                                    this.height = 28;
                                                                                                                                                    this.aiStyle = 10;
                                                                                                                                                    this.damage = 35;
                                                                                                                                                    this.defense = 6;
                                                                                                                                                    this.lifeMax = 80;
                                                                                                                                                    this.soundHit = 2;
                                                                                                                                                    this.soundKilled = 2;
                                                                                                                                                    this.noGravity = true;
                                                                                                                                                    this.noTileCollide = true;
                                                                                                                                                    this.value = 150f;
                                                                                                                                                    this.knockBackResist = 0f;
                                                                                                                                                    NPC.npcSlots = 0.75f;
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    if (this.type == 35)
                                                                                                                                                    {
                                                                                                                                                        this.name = "Skeletron Head";
                                                                                                                                                        this.width = 80;
                                                                                                                                                        this.height = 102;
                                                                                                                                                        this.aiStyle = 11;
                                                                                                                                                        this.damage = 30;
                                                                                                                                                        this.defense = 10;
                                                                                                                                                        this.lifeMax = 5000;
                                                                                                                                                        this.soundHit = 2;
                                                                                                                                                        this.soundKilled = 2;
                                                                                                                                                        this.noGravity = true;
                                                                                                                                                        this.noTileCollide = true;
                                                                                                                                                        this.value = 50000f;
                                                                                                                                                        this.knockBackResist = 0f;
                                                                                                                                                        this.boss = true;
                                                                                                                                                        NPC.npcSlots = 6f;
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        if (this.type == 36)
                                                                                                                                                        {
                                                                                                                                                            this.name = "Skeletron Hand";
                                                                                                                                                            this.width = 52;
                                                                                                                                                            this.height = 52;
                                                                                                                                                            this.aiStyle = 12;
                                                                                                                                                            this.damage = 25;
                                                                                                                                                            this.defense = 18;
                                                                                                                                                            this.lifeMax = 800;
                                                                                                                                                            this.soundHit = 2;
                                                                                                                                                            this.soundKilled = 2;
                                                                                                                                                            this.noGravity = true;
                                                                                                                                                            this.noTileCollide = true;
                                                                                                                                                            this.knockBackResist = 0f;
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
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
                                                                                                                                                            else
                                                                                                                                                            {
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
                                                                                                                                                                else
                                                                                                                                                                {
                                                                                                                                                                    if (this.type == 39)
                                                                                                                                                                    {
                                                                                                                                                                        NPC.npcSlots = 6f;
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
                                                                                                                                                                        this.value = 300f;
                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                    {
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
                                                                                                                                                                            this.value = 300f;
                                                                                                                                                                        }
                                                                                                                                                                        else
                                                                                                                                                                        {
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
                                                                                                                                                                                this.value = 300f;
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                if (this.type == 42)
                                                                                                                                                                                {
                                                                                                                                                                                    this.name = "Hornet";
                                                                                                                                                                                    this.width = 34;
                                                                                                                                                                                    this.height = 32;
                                                                                                                                                                                    this.aiStyle = 5;
                                                                                                                                                                                    this.damage = 40;
                                                                                                                                                                                    this.defense = 12;
                                                                                                                                                                                    this.lifeMax = 100;
                                                                                                                                                                                    this.soundHit = 1;
                                                                                                                                                                                    this.knockBackResist = 0.8f;
                                                                                                                                                                                    this.soundKilled = 1;
                                                                                                                                                                                    this.value = 200f;
                                                                                                                                                                                    this.noGravity = true;
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    if (this.type == 43)
                                                                                                                                                                                    {
                                                                                                                                                                                        this.noGravity = true;
                                                                                                                                                                                        this.noTileCollide = true;
                                                                                                                                                                                        this.behindTiles = true;
                                                                                                                                                                                        this.name = "Man Eater";
                                                                                                                                                                                        this.width = 30;
                                                                                                                                                                                        this.height = 30;
                                                                                                                                                                                        this.aiStyle = 13;
                                                                                                                                                                                        this.damage = 60;
                                                                                                                                                                                        this.defense = 14;
                                                                                                                                                                                        this.lifeMax = 200;
                                                                                                                                                                                        this.soundHit = 1;
                                                                                                                                                                                        this.knockBackResist = 0f;
                                                                                                                                                                                        this.soundKilled = 1;
                                                                                                                                                                                        this.value = 350f;
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
                                                                                                                                                                                                        this.damage = 20;
                                                                                                                                                                                                        this.defense = 4;
                                                                                                                                                                                                        this.lifeMax = 70;
                                                                                                                                                                                                        this.soundHit = 1;
                                                                                                                                                                                                        this.soundKilled = 1;
                                                                                                                                                                                                        this.value = 500f;
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else
                                                                                                                                                                                                    {
                                                                                                                                                                                                        if (this.type == 48)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            this.name = "Harpy";
                                                                                                                                                                                                            this.width = 24;
                                                                                                                                                                                                            this.height = 34;
                                                                                                                                                                                                            this.aiStyle = 14;
                                                                                                                                                                                                            this.damage = 25;
                                                                                                                                                                                                            this.defense = 8;
                                                                                                                                                                                                            this.lifeMax = 100;
                                                                                                                                                                                                            this.soundHit = 1;
                                                                                                                                                                                                            this.knockBackResist = 0.6f;
                                                                                                                                                                                                            this.soundKilled = 1;
                                                                                                                                                                                                            this.value = 300f;
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else
                                                                                                                                                                                                        {
                                                                                                                                                                                                            if (this.type == 49)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                NPC.npcSlots = 0.5f;
                                                                                                                                                                                                                this.name = "Cave Bat";
                                                                                                                                                                                                                this.width = 12;
                                                                                                                                                                                                                this.height = 12;
                                                                                                                                                                                                                this.aiStyle = 14;
                                                                                                                                                                                                                this.damage = 15;
                                                                                                                                                                                                                this.defense = 2;
                                                                                                                                                                                                                this.lifeMax = 25;
                                                                                                                                                                                                                this.soundHit = 1;
                                                                                                                                                                                                                this.knockBackResist = 0.8f;
                                                                                                                                                                                                                this.soundKilled = 4;
                                                                                                                                                                                                                this.value = 90f;
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
                                                                                                                                                                                                                        NPC.npcSlots = 0.5f;
                                                                                                                                                                                                                        this.name = "Jungle Bat";
                                                                                                                                                                                                                        this.width = 12;
                                                                                                                                                                                                                        this.height = 12;
                                                                                                                                                                                                                        this.aiStyle = 14;
                                                                                                                                                                                                                        this.damage = 20;
                                                                                                                                                                                                                        this.defense = 4;
                                                                                                                                                                                                                        this.lifeMax = 60;
                                                                                                                                                                                                                        this.soundHit = 1;
                                                                                                                                                                                                                        this.knockBackResist = 0.8f;
                                                                                                                                                                                                                        this.soundKilled = 4;
                                                                                                                                                                                                                        this.value = 80f;
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
                                                                                                                                                                                                                            this.value = 1000f;
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
                                                                                                                                                                                                                                this.value = 1000f;
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
                                                                                                                                                                                                                                            this.noTileCollide = true;
                                                                                                                                                                                                                                            this.behindTiles = true;
                                                                                                                                                                                                                                            this.noGravity = true;
                                                                                                                                                                                                                                            this.name = "Snatcher";
                                                                                                                                                                                                                                            this.width = 30;
                                                                                                                                                                                                                                            this.height = 30;
                                                                                                                                                                                                                                            this.aiStyle = 13;
                                                                                                                                                                                                                                            this.damage = 25;
                                                                                                                                                                                                                                            this.defense = 10;
                                                                                                                                                                                                                                            this.lifeMax = 100;
                                                                                                                                                                                                                                            this.soundHit = 1;
                                                                                                                                                                                                                                            this.knockBackResist = 0f;
                                                                                                                                                                                                                                            this.soundKilled = 1;
                                                                                                                                                                                                                                            this.value = 90f;
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
                                                                                                                                                                                                                                                    NPC.npcSlots = 0.5f;
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
                                                                                                                                                                                                                                                    this.value = 50f;
                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                    if (this.type == 59)
                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                        this.name = "Lava Slime";
                                                                                                                                                                                                                                                        this.width = 24;
                                                                                                                                                                                                                                                        this.height = 18;
                                                                                                                                                                                                                                                        this.aiStyle = 1;
                                                                                                                                                                                                                                                        this.damage = 15;
                                                                                                                                                                                                                                                        this.defense = 10;
                                                                                                                                                                                                                                                        this.lifeMax = 50;
                                                                                                                                                                                                                                                        this.soundHit = 1;
                                                                                                                                                                                                                                                        this.soundKilled = 1;
                                                                                                                                                                                                                                                        this.scale = 1.1f;
                                                                                                                                                                                                                                                        this.alpha = 50;
                                                                                                                                                                                                                                                        this.lavaImmune = true;
                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                        if (this.type == 60)
                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                            NPC.npcSlots = 0.5f;
                                                                                                                                                                                                                                                            this.name = "Hellbat";
                                                                                                                                                                                                                                                            this.width = 12;
                                                                                                                                                                                                                                                            this.height = 12;
                                                                                                                                                                                                                                                            this.aiStyle = 14;
                                                                                                                                                                                                                                                            this.damage = 30;
                                                                                                                                                                                                                                                            this.defense = 8;
                                                                                                                                                                                                                                                            this.lifeMax = 55;
                                                                                                                                                                                                                                                            this.soundHit = 1;
                                                                                                                                                                                                                                                            this.knockBackResist = 0.8f;
                                                                                                                                                                                                                                                            this.soundKilled = 4;
                                                                                                                                                                                                                                                            this.value = 120f;
                                                                                                                                                                                                                                                            this.scale = 1.1f;
                                                                                                                                                                                                                                                            this.lavaImmune = true;
                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                            if (this.type == 61)
                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                this.name = "Vulture";
                                                                                                                                                                                                                                                                this.width = 36;
                                                                                                                                                                                                                                                                this.height = 36;
                                                                                                                                                                                                                                                                this.aiStyle = 17;
                                                                                                                                                                                                                                                                this.damage = 15;
                                                                                                                                                                                                                                                                this.defense = 4;
                                                                                                                                                                                                                                                                this.lifeMax = 40;
                                                                                                                                                                                                                                                                this.soundHit = 1;
                                                                                                                                                                                                                                                                this.knockBackResist = 0.8f;
                                                                                                                                                                                                                                                                this.soundKilled = 1;
                                                                                                                                                                                                                                                                this.value = 60f;
                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                if (this.type == 62)
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    NPC.npcSlots = 2f;
                                                                                                                                                                                                                                                                    this.name = "Demon";
                                                                                                                                                                                                                                                                    this.width = 28;
                                                                                                                                                                                                                                                                    this.height = 48;
                                                                                                                                                                                                                                                                    this.aiStyle = 14;
                                                                                                                                                                                                                                                                    this.damage = 50;
                                                                                                                                                                                                                                                                    this.defense = 8;
                                                                                                                                                                                                                                                                    this.lifeMax = 120;
                                                                                                                                                                                                                                                                    this.soundHit = 1;
                                                                                                                                                                                                                                                                    this.knockBackResist = 0.6f;
                                                                                                                                                                                                                                                                    this.soundKilled = 1;
                                                                                                                                                                                                                                                                    this.value = 300f;
                                                                                                                                                                                                                                                                    this.lavaImmune = true;
                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    if (this.type == 63)
                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                        this.noGravity = true;
                                                                                                                                                                                                                                                                        this.name = "Blue Jellyfish";
                                                                                                                                                                                                                                                                        this.width = 26;
                                                                                                                                                                                                                                                                        this.height = 26;
                                                                                                                                                                                                                                                                        this.aiStyle = 18;
                                                                                                                                                                                                                                                                        this.damage = 20;
                                                                                                                                                                                                                                                                        this.defense = 2;
                                                                                                                                                                                                                                                                        this.lifeMax = 30;
                                                                                                                                                                                                                                                                        this.soundHit = 1;
                                                                                                                                                                                                                                                                        this.soundKilled = 1;
                                                                                                                                                                                                                                                                        this.value = 100f;
                                                                                                                                                                                                                                                                        this.alpha = 20;
                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                        if (this.type == 64)
                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                            this.noGravity = true;
                                                                                                                                                                                                                                                                            this.name = "Pink Jellyfish";
                                                                                                                                                                                                                                                                            this.width = 26;
                                                                                                                                                                                                                                                                            this.height = 26;
                                                                                                                                                                                                                                                                            this.aiStyle = 18;
                                                                                                                                                                                                                                                                            this.damage = 30;
                                                                                                                                                                                                                                                                            this.defense = 6;
                                                                                                                                                                                                                                                                            this.lifeMax = 70;
                                                                                                                                                                                                                                                                            this.soundHit = 1;
                                                                                                                                                                                                                                                                            this.soundKilled = 1;
                                                                                                                                                                                                                                                                            this.value = 100f;
                                                                                                                                                                                                                                                                            this.alpha = 20;
                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                            if (this.type == 65)
                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                this.noGravity = true;
                                                                                                                                                                                                                                                                                this.name = "Shark";
                                                                                                                                                                                                                                                                                this.width = 100;
                                                                                                                                                                                                                                                                                this.height = 24;
                                                                                                                                                                                                                                                                                this.aiStyle = 16;
                                                                                                                                                                                                                                                                                this.damage = 40;
                                                                                                                                                                                                                                                                                this.defense = 2;
                                                                                                                                                                                                                                                                                this.lifeMax = 300;
                                                                                                                                                                                                                                                                                this.soundHit = 1;
                                                                                                                                                                                                                                                                                this.soundKilled = 1;
                                                                                                                                                                                                                                                                                this.value = 400f;
                                                                                                                                                                                                                                                                                this.knockBackResist = 0.7f;
                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                if (this.type == 66)
                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                    NPC.npcSlots = 2f;
                                                                                                                                                                                                                                                                                    this.name = "Voodoo Demon";
                                                                                                                                                                                                                                                                                    this.width = 28;
                                                                                                                                                                                                                                                                                    this.height = 48;
                                                                                                                                                                                                                                                                                    this.aiStyle = 14;
                                                                                                                                                                                                                                                                                    this.damage = 50;
                                                                                                                                                                                                                                                                                    this.defense = 8;
                                                                                                                                                                                                                                                                                    this.lifeMax = 120;
                                                                                                                                                                                                                                                                                    this.soundHit = 1;
                                                                                                                                                                                                                                                                                    this.knockBackResist = 0.6f;
                                                                                                                                                                                                                                                                                    this.soundKilled = 1;
                                                                                                                                                                                                                                                                                    this.value = 1000f;
                                                                                                                                                                                                                                                                                    this.lavaImmune = true;
                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                    if (this.type == 67)
                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                        this.name = "Crab";
                                                                                                                                                                                                                                                                                        this.width = 28;
                                                                                                                                                                                                                                                                                        this.height = 20;
                                                                                                                                                                                                                                                                                        this.aiStyle = 3;
                                                                                                                                                                                                                                                                                        this.damage = 20;
                                                                                                                                                                                                                                                                                        this.defense = 10;
                                                                                                                                                                                                                                                                                        this.lifeMax = 40;
                                                                                                                                                                                                                                                                                        this.soundHit = 1;
                                                                                                                                                                                                                                                                                        this.soundKilled = 1;
                                                                                                                                                                                                                                                                                        this.value = 60f;
                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                        if (this.type == 68)
                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                            this.name = "Dungeon Guardian";
                                                                                                                                                                                                                                                                                            this.width = 80;
                                                                                                                                                                                                                                                                                            this.height = 102;
                                                                                                                                                                                                                                                                                            this.aiStyle = 11;
                                                                                                                                                                                                                                                                                            this.damage = 9000;
                                                                                                                                                                                                                                                                                            this.defense = 9000;
                                                                                                                                                                                                                                                                                            this.lifeMax = 9999;
                                                                                                                                                                                                                                                                                            this.soundHit = 2;
                                                                                                                                                                                                                                                                                            this.soundKilled = 2;
                                                                                                                                                                                                                                                                                            this.noGravity = true;
                                                                                                                                                                                                                                                                                            this.noTileCollide = true;
                                                                                                                                                                                                                                                                                            this.knockBackResist = 0f;
                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                            if (this.type == 69)
                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                this.name = "Antlion";
                                                                                                                                                                                                                                                                                                this.width = 24;
                                                                                                                                                                                                                                                                                                this.height = 24;
                                                                                                                                                                                                                                                                                                this.aiStyle = 19;
                                                                                                                                                                                                                                                                                                this.damage = 10;
                                                                                                                                                                                                                                                                                                this.defense = 6;
                                                                                                                                                                                                                                                                                                this.lifeMax = 45;
                                                                                                                                                                                                                                                                                                this.soundHit = 1;
                                                                                                                                                                                                                                                                                                this.soundKilled = 1;
                                                                                                                                                                                                                                                                                                this.knockBackResist = 0f;
                                                                                                                                                                                                                                                                                                this.value = 60f;
                                                                                                                                                                                                                                                                                                this.behindTiles = true;
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
            if (Main.dedServ)
            {
                this.frame = default(Rectangle);
            }
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
                    return;
                }
            }
            else
            {
                if (this.aiStyle == 1)
                {
                    if (this.type == 59)
                    {
                        Vector2 arg_D5_0 = new Vector2(this.position.X, this.position.Y);
                        int arg_D5_1 = this.width;
                        int arg_D5_2 = this.height;
                        int arg_D5_3 = 6;
                        float arg_D5_4 = this.velocity.X * 0.2f;
                        float arg_D5_5 = this.velocity.Y * 0.2f;
                        int arg_D5_6 = 100;
                        Color newColor = default(Color);
                        int num = Dust.NewDust(arg_D5_0, arg_D5_1, arg_D5_2, arg_D5_3, arg_D5_4, arg_D5_5, arg_D5_6, newColor, 1.7f);
                        Main.dust[num].noGravity = true;
                    }
                    if (this.wet)
                    {
                        if (this.type == 59)
                        {
                            if (this.velocity.Y > 2f)
                            {
                                this.velocity.Y = this.velocity.Y * 0.9f;
                            }
                            else
                            {
                                if (this.directionY < 0)
                                {
                                    this.velocity.Y = this.velocity.Y - 0.8f;
                                }
                            }
                            this.velocity.Y = this.velocity.Y - 0.5f;
                            if (this.velocity.Y < -10f)
                            {
                                this.velocity.Y = -10f;
                            }
                        }
                        else
                        {
                            if (this.velocity.Y > 2f)
                            {
                                this.velocity.Y = this.velocity.Y * 0.9f;
                            }
                            this.velocity.Y = this.velocity.Y - 0.5f;
                            if (this.velocity.Y < -4f)
                            {
                                this.velocity.Y = -4f;
                            }
                        }
                        this.TargetClosest(true);
                    }
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
                        if (this.type == 59)
                        {
                            this.ai[0] += 2f;
                        }
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
                                if (this.type == 59)
                                {
                                    this.velocity.Y = this.velocity.Y - 2f;
                                }
                                this.velocity.X = this.velocity.X + (float)(3 * this.direction);
                                if (this.type == 59)
                                {
                                    this.velocity.X = this.velocity.X + 0.5f * (float)this.direction;
                                }
                                this.ai[0] = -200f;
                                this.ai[1] = 0f;
                                this.ai[3] = this.position.X;
                                return;
                            }
                            this.velocity.Y = -6f;
                            this.velocity.X = this.velocity.X + (float)(2 * this.direction);
                            if (this.type == 59)
                            {
                                this.velocity.X = this.velocity.X + (float)(2 * this.direction);
                            }
                            this.ai[0] = -120f;
                            this.ai[1] += 1f;
                            return;
                        }
                        else if (this.ai[0] >= -30f)
                        {
                            this.aiAction = 1;
                            return;
                        }
                    }
                    else if (this.target < 255 && ((this.direction == 1 && this.velocity.X < 3f) || (this.direction == -1 && this.velocity.X > -3f)))
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
                            Vector2 arg_ADF_0 = new Vector2(this.position.X, this.position.Y + (float)this.height * 0.25f);
                            int arg_ADF_1 = this.width;
                            int arg_ADF_2 = (int)((float)this.height * 0.5f);
                            int arg_ADF_3 = 5;
                            float arg_ADF_4 = this.velocity.X;
                            float arg_ADF_5 = 2f;
                            int arg_ADF_6 = 0;
                            Color newColor = default(Color);
                            int num2 = Dust.NewDust(arg_ADF_0, arg_ADF_1, arg_ADF_2, arg_ADF_3, arg_ADF_4, arg_ADF_5, arg_ADF_6, newColor, 1f);
                            Dust expr_AF1_cp_0 = Main.dust[num2];
                            expr_AF1_cp_0.velocity.X = expr_AF1_cp_0.velocity.X * 0.5f;
                            Dust expr_B0E_cp_0 = Main.dust[num2];
                            expr_B0E_cp_0.velocity.Y = expr_B0E_cp_0.velocity.Y * 0.1f;
                        }
                        if (this.wet)
                        {
                            if (this.velocity.Y > 0f)
                            {
                                this.velocity.Y = this.velocity.Y * 0.95f;
                            }
                            this.velocity.Y = this.velocity.Y - 0.5f;
                            if (this.velocity.Y < -4f)
                            {
                                this.velocity.Y = -4f;
                            }
                            this.TargetClosest(true);
                            return;
                        }
                    }
                    else
                    {
                        if (this.aiStyle == 3)
                        {
                            int num3 = 60;
                            bool flag = false;
                            if (this.velocity.Y == 0f && ((this.velocity.X > 0f && this.direction < 0) || (this.velocity.X < 0f && this.direction > 0)))
                            {
                                flag = true;
                            }
                            if (this.position.X == this.oldPosition.X || this.ai[3] >= (float)num3 || flag)
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
                            if (this.ai[3] > (float)(num3 * 10))
                            {
                                this.ai[3] = 0f;
                            }
                            if (this.ai[3] == (float)num3)
                            {
                                this.netUpdate = true;
                            }
                            if ((!Main.dayTime || (double)this.position.Y > Main.worldSurface * 16.0 || this.type == 26 || this.type == 27 || this.type == 28 || this.type == 31 || this.type == 47 || this.type == 67) && this.ai[3] < (float)num3)
                            {
                                this.TargetClosest(true);
                            }
                            else
                            {
                                if (Main.dayTime && (double)(this.position.Y / 16f) < Main.worldSurface && this.timeLeft > 10)
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
                                    if (this.type == 67)
                                    {
                                        if (this.velocity.X < -0.5f || this.velocity.X > 0.5f)
                                        {
                                            if (this.velocity.Y == 0f)
                                            {
                                                this.velocity *= 0.7f;
                                            }
                                        }
                                        else
                                        {
                                            if (this.velocity.X < 0.5f && this.direction == 1)
                                            {
                                                this.velocity.X = this.velocity.X + 0.03f;
                                                if (this.velocity.X > 0.5f)
                                                {
                                                    this.velocity.X = 0.5f;
                                                }
                                            }
                                            else
                                            {
                                                if (this.velocity.X > -0.5f && this.direction == -1)
                                                {
                                                    this.velocity.X = this.velocity.X - 0.03f;
                                                    if (this.velocity.X < -0.5f)
                                                    {
                                                        this.velocity.X = -0.5f;
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
                            }
                            if (this.velocity.Y != 0f)
                            {
                                this.ai[1] = 0f;
                                this.ai[2] = 0f;
                                return;
                            }
                            int num4 = (int)((this.position.X + (float)(this.width / 2) + (float)(15 * this.direction)) / 16f);
                            int num5 = (int)((this.position.Y + (float)this.height - 15f) / 16f);
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
                            if (Main.tile[num4 + this.direction, num5 - 1] == null)
                            {
                                Main.tile[num4 + this.direction, num5 - 1] = new Tile();
                            }
                            if (Main.tile[num4 + this.direction, num5 + 1] == null)
                            {
                                Main.tile[num4 + this.direction, num5 + 1] = new Tile();
                            }
                            bool flag2 = true;
                            if (this.type == 47 || this.type == 67)
                            {
                                flag2 = false;
                            }
                            if (Main.tile[num4, num5 - 1].active && Main.tile[num4, num5 - 1].type == 10 && flag2)
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
                                    WorldGen.KillTile(num4, num5 - 1, true, false, false);
                                    if ((Main.netMode != 1 || !flag3) && flag3 && Main.netMode != 1)
                                    {
                                        if (this.type == 26)
                                        {
                                            WorldGen.KillTile(num4, num5 - 1, false, false, false);
                                            if (Main.netMode == 2)
                                            {
                                                NetMessage.SendData(17, -1, -1, "", 0, (float)num4, (float)(num5 - 1), 0f, 0);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            bool flag4 = WorldGen.OpenDoor(num4, num5, this.direction, this.closeDoor, DoorOpener.NPC);
                                            if (!flag4)
                                            {
                                                this.ai[3] = (float)num3;
                                                this.netUpdate = true;
                                            }
                                            if (Main.netMode == 2 && flag4)
                                            {
                                                NetMessage.SendData(19, -1, -1, "", 0, (float)num4, (float)num5, (float)this.direction, 0);
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
                                    if (Main.tile[num4, num5 - 2].active && Main.tileSolid[(int)Main.tile[num4, num5 - 2].type])
                                    {
                                        if (Main.tile[num4, num5 - 3].active && Main.tileSolid[(int)Main.tile[num4, num5 - 3].type])
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
                                        if (Main.tile[num4, num5 - 1].active && Main.tileSolid[(int)Main.tile[num4, num5 - 1].type])
                                        {
                                            this.velocity.Y = -6f;
                                            this.netUpdate = true;
                                        }
                                        else
                                        {
                                            if (Main.tile[num4, num5].active && Main.tileSolid[(int)Main.tile[num4, num5].type])
                                            {
                                                this.velocity.Y = -5f;
                                                this.netUpdate = true;
                                            }
                                            else
                                            {
                                                if (this.directionY < 0 && this.type != 67 && (!Main.tile[num4, num5 + 1].active || !Main.tileSolid[(int)Main.tile[num4, num5 + 1].type]) && (!Main.tile[num4 + this.direction, num5 + 1].active || !Main.tileSolid[(int)Main.tile[num4 + this.direction, num5 + 1].type]))
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
                                if ((this.type == 31 || this.type == 47) && this.velocity.Y == 0f && Math.Abs(this.position.X + (float)(this.width / 2) - (Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2))) < 100f && Math.Abs(this.position.Y + (float)(this.height / 2) - (Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2))) < 50f && ((this.direction > 0 && this.velocity.X >= 1f) || (this.direction < 0 && this.velocity.X <= -1f)))
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
                                if (this.target < 0 || this.target == 255 || Main.players[this.target].dead || !Main.players[this.target].active)
                                {
                                    this.TargetClosest(true);
                                }
                                bool dead = Main.players[this.target].dead;
                                float num6 = this.position.X + (float)(this.width / 2) - Main.players[this.target].position.X - (float)(Main.players[this.target].width / 2);
                                float num7 = this.position.Y + (float)this.height - 59f - Main.players[this.target].position.Y - (float)(Main.players[this.target].height / 2);
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
                                if (this.ai[0] == 0f && this.ai[1] == 0f)
                                {
                                    num9 = 0.02f;
                                }
                                if (this.ai[0] == 0f && this.ai[1] == 2f && this.ai[2] > 40f)
                                {
                                    num9 = 0.05f;
                                }
                                if (this.ai[0] == 3f && this.ai[1] == 0f)
                                {
                                    num9 = 0.05f;
                                }
                                if (this.ai[0] == 3f && this.ai[1] == 2f && this.ai[2] > 40f)
                                {
                                    num9 = 0.08f;
                                }
                                if (this.rotation < num8)
                                {
                                    if ((double)(num8 - this.rotation) > 3.1415)
                                    {
                                        this.rotation -= num9;
                                    }
                                    else
                                    {
                                        this.rotation += num9;
                                    }
                                }
                                else
                                {
                                    if (this.rotation > num8)
                                    {
                                        if ((double)(this.rotation - num8) > 3.1415)
                                        {
                                            this.rotation += num9;
                                        }
                                        else
                                        {
                                            this.rotation -= num9;
                                        }
                                    }
                                }
                                if (this.rotation > num8 - num9 && this.rotation < num8 + num9)
                                {
                                    this.rotation = num8;
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
                                if (this.rotation > num8 - num9 && this.rotation < num8 + num9)
                                {
                                    this.rotation = num8;
                                }
                                if (Main.rand.Next(5) == 0)
                                {
                                    Vector2 arg_1DAB_0 = new Vector2(this.position.X, this.position.Y + (float)this.height * 0.25f);
                                    int arg_1DAB_1 = this.width;
                                    int arg_1DAB_2 = (int)((float)this.height * 0.5f);
                                    int arg_1DAB_3 = 5;
                                    float arg_1DAB_4 = this.velocity.X;
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
                                            float num11 = 5f;
                                            float num12 = 0.04f;
                                            Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                            float num13 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector.X;
                                            float num14 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - 200f - vector.Y;
                                            float num15 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                                            float num16 = num15;
                                            num15 = num11 / num15;
                                            num13 *= num15;
                                            num14 *= num15;
                                            if (this.velocity.X < num13)
                                            {
                                                this.velocity.X = this.velocity.X + num12;
                                                if (this.velocity.X < 0f && num13 > 0f)
                                                {
                                                    this.velocity.X = this.velocity.X + num12;
                                                }
                                            }
                                            else
                                            {
                                                if (this.velocity.X > num13)
                                                {
                                                    this.velocity.X = this.velocity.X - num12;
                                                    if (this.velocity.X > 0f && num13 < 0f)
                                                    {
                                                        this.velocity.X = this.velocity.X - num12;
                                                    }
                                                }
                                            }
                                            if (this.velocity.Y < num14)
                                            {
                                                this.velocity.Y = this.velocity.Y + num12;
                                                if (this.velocity.Y < 0f && num14 > 0f)
                                                {
                                                    this.velocity.Y = this.velocity.Y + num12;
                                                }
                                            }
                                            else
                                            {
                                                if (this.velocity.Y > num14)
                                                {
                                                    this.velocity.Y = this.velocity.Y - num12;
                                                    if (this.velocity.Y > 0f && num14 < 0f)
                                                    {
                                                        this.velocity.Y = this.velocity.Y - num12;
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
                                                if (this.position.Y + (float)this.height < Main.players[this.target].position.Y && num16 < 500f)
                                                {
                                                    if (!Main.players[this.target].dead)
                                                    {
                                                        this.ai[3] += 1f;
                                                    }
                                                    if (this.ai[3] >= 90f)
                                                    {
                                                        this.ai[3] = 0f;
                                                        this.rotation = num8;
                                                        float num17 = 5f;
                                                        float num18 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector.X;
                                                        float num19 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - vector.Y;
                                                        float num20 = (float)Math.Sqrt((double)(num18 * num18 + num19 * num19));
                                                        num20 = num17 / num20;
                                                        Vector2 vector2 = vector;
                                                        Vector2 vector3;
                                                        vector3.X = num18 * num20;
                                                        vector3.Y = num19 * num20;
                                                        vector2.X += vector3.X * 10f;
                                                        vector2.Y += vector3.Y * 10f;
                                                        if (Main.netMode != 1)
                                                        {
                                                            int num21 = NPC.NewNPC((int)vector2.X, (int)vector2.Y, 5, 0);
                                                            Main.npc[num21].velocity.X = vector3.X;
                                                            Main.npc[num21].velocity.Y = vector3.Y;
                                                            if (Main.netMode == 2 && num21 < 1000)
                                                            {
                                                                NetMessage.SendData(23, -1, -1, "", num21);
                                                            }
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
                                            if (this.ai[1] == 1f)
                                            {
                                                this.rotation = num8;
                                                float num22 = 7f;
                                                Vector2 vector4 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                float num23 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector4.X;
                                                float num24 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - vector4.Y;
                                                float num25 = (float)Math.Sqrt((double)(num23 * num23 + num24 * num24));
                                                num25 = num22 / num25;
                                                this.velocity.X = num23 * num25;
                                                this.velocity.Y = num24 * num25;
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
                                                        this.rotation = num8;
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
                                                    for (int j = 0; j < 2; j++)
                                                    {
                                                        Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 8);
                                                        Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7);
                                                        Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 6);
                                                    }
                                                    for (int k = 0; k < 20; k++)
                                                    {
                                                        Vector2 arg_28B3_0 = this.position;
                                                        int arg_28B3_1 = this.width;
                                                        int arg_28B3_2 = this.height;
                                                        int arg_28B3_3 = 5;
                                                        float arg_28B3_4 = (float)Main.rand.Next(-30, 31) * 0.2f;
                                                        float arg_28B3_5 = (float)Main.rand.Next(-30, 31) * 0.2f;
                                                        int arg_28B3_6 = 0;
                                                        newColor = default(Color);
                                                        Dust.NewDust(arg_28B3_0, arg_28B3_1, arg_28B3_2, arg_28B3_3, arg_28B3_4, arg_28B3_5, arg_28B3_6, newColor, 1f);
                                                    }
                                                }
                                            }
                                            Vector2 arg_2932_0 = this.position;
                                            int arg_2932_1 = this.width;
                                            int arg_2932_2 = this.height;
                                            int arg_2932_3 = 5;
                                            float arg_2932_4 = (float)Main.rand.Next(-30, 31) * 0.2f;
                                            float arg_2932_5 = (float)Main.rand.Next(-30, 31) * 0.2f;
                                            int arg_2932_6 = 0;
                                            newColor = default(Color);
                                            Dust.NewDust(arg_2932_0, arg_2932_1, arg_2932_2, arg_2932_3, arg_2932_4, arg_2932_5, arg_2932_6, newColor, 1f);
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
                                                float num26 = 6f;
                                                float num27 = 0.07f;
                                                Vector2 vector5 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                float num28 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector5.X;
                                                float num29 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - 120f - vector5.Y;
                                                float num30 = (float)Math.Sqrt((double)(num28 * num28 + num29 * num29));
                                                num30 = num26 / num30;
                                                num28 *= num30;
                                                num29 *= num30;
                                                if (this.velocity.X < num28)
                                                {
                                                    this.velocity.X = this.velocity.X + num27;
                                                    if (this.velocity.X < 0f && num28 > 0f)
                                                    {
                                                        this.velocity.X = this.velocity.X + num27;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.velocity.X > num28)
                                                    {
                                                        this.velocity.X = this.velocity.X - num27;
                                                        if (this.velocity.X > 0f && num28 < 0f)
                                                        {
                                                            this.velocity.X = this.velocity.X - num27;
                                                        }
                                                    }
                                                }
                                                if (this.velocity.Y < num29)
                                                {
                                                    this.velocity.Y = this.velocity.Y + num27;
                                                    if (this.velocity.Y < 0f && num29 > 0f)
                                                    {
                                                        this.velocity.Y = this.velocity.Y + num27;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.velocity.Y > num29)
                                                    {
                                                        this.velocity.Y = this.velocity.Y - num27;
                                                        if (this.velocity.Y > 0f && num29 < 0f)
                                                        {
                                                            this.velocity.Y = this.velocity.Y - num27;
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
                                                    this.rotation = num8;
                                                    float num31 = 8f;
                                                    Vector2 vector6 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                    float num32 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector6.X;
                                                    float num33 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - vector6.Y;
                                                    float num34 = (float)Math.Sqrt((double)(num32 * num32 + num33 * num33));
                                                    num34 = num31 / num34;
                                                    this.velocity.X = num32 * num34;
                                                    this.velocity.Y = num33 * num34;
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
                                                        this.rotation = num8;
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
                                    if (this.target < 0 || this.target == 255 || Main.players[this.target].dead)
                                    {
                                        this.TargetClosest(true);
                                    }
                                    float num35 = 6f;
                                    float num36 = 0.05f;
                                    if (this.type == 6 || this.type == 42)
                                    {
                                        num35 = 4f;
                                        num36 = 0.02f;
                                    }
                                    else
                                    {
                                        if (this.type == 23)
                                        {
                                            num35 = 2f;
                                            num36 = 0.03f;
                                        }
                                    }
                                    Vector2 vector7 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                    float num37 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector7.X;
                                    float num38 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - vector7.Y;
                                    float num39 = (float)Math.Sqrt((double)(num37 * num37 + num38 * num38));
                                    float num40 = num39;
                                    num39 = num35 / num39;
                                    num37 *= num39;
                                    num38 *= num39;
                                    if (this.type == 6 || this.type == 42)
                                    {
                                        if (num40 > 100f)
                                        {
                                            this.ai[0] += 1f;
                                            if (this.ai[0] > 0f)
                                            {
                                                this.velocity.Y = this.velocity.Y + 0.023f;
                                            }
                                            else
                                            {
                                                this.velocity.Y = this.velocity.Y - 0.023f;
                                            }
                                            if (this.ai[0] < -100f || this.ai[0] > 100f)
                                            {
                                                this.velocity.X = this.velocity.X + 0.023f;
                                            }
                                            else
                                            {
                                                this.velocity.X = this.velocity.X - 0.023f;
                                            }
                                            if (this.ai[0] > 200f)
                                            {
                                                this.ai[0] = -200f;
                                            }
                                        }
                                        if (num40 < 150f)
                                        {
                                            this.velocity.X = this.velocity.X + num37 * 0.007f;
                                            this.velocity.Y = this.velocity.Y + num38 * 0.007f;
                                        }
                                    }
                                    if (Main.players[this.target].dead)
                                    {
                                        num37 = (float)this.direction * num35 / 2f;
                                        num38 = -num35 / 2f;
                                    }
                                    if (this.velocity.X < num37)
                                    {
                                        this.velocity.X = this.velocity.X + num36;
                                        if (this.type != 6 && this.velocity.X < 0f && num37 > 0f)
                                        {
                                            this.velocity.X = this.velocity.X + num36;
                                        }
                                    }
                                    else
                                    {
                                        if (this.velocity.X > num37)
                                        {
                                            this.velocity.X = this.velocity.X - num36;
                                            if (this.type != 6 && this.velocity.X > 0f && num37 < 0f)
                                            {
                                                this.velocity.X = this.velocity.X - num36;
                                            }
                                        }
                                    }
                                    if (this.velocity.Y < num38)
                                    {
                                        this.velocity.Y = this.velocity.Y + num36;
                                        if (this.type != 6 && this.velocity.Y < 0f && num38 > 0f)
                                        {
                                            this.velocity.Y = this.velocity.Y + num36;
                                        }
                                    }
                                    else
                                    {
                                        if (this.velocity.Y > num38)
                                        {
                                            this.velocity.Y = this.velocity.Y - num36;
                                            if (this.type != 6 && this.velocity.Y > 0f && num38 < 0f)
                                            {
                                                this.velocity.Y = this.velocity.Y - num36;
                                            }
                                        }
                                    }
                                    if (this.type == 23)
                                    {
                                        if (num37 > 0f)
                                        {
                                            this.spriteDirection = 1;
                                            this.rotation = (float)Math.Atan2((double)num38, (double)num37);
                                        }
                                        else
                                        {
                                            if (num37 < 0f)
                                            {
                                                this.spriteDirection = -1;
                                                this.rotation = (float)Math.Atan2((double)num38, (double)num37) + 3.14f;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (this.type == 6)
                                        {
                                            this.rotation = (float)Math.Atan2((double)num38, (double)num37) - 1.57f;
                                        }
                                        else
                                        {
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
                                            }
                                            else
                                            {
                                                this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) - 1.57f;
                                            }
                                        }
                                    }
                                    if (this.type == 6 || this.type == 23 || this.type == 42)
                                    {
                                        float num41 = 0.7f;
                                        if (this.type == 6)
                                        {
                                            num41 = 0.4f;
                                        }
                                        if (this.collideX)
                                        {
                                            this.netUpdate = true;
                                            this.velocity.X = this.oldVelocity.X * -num41;
                                            if (this.direction == -1 && this.velocity.X > 0f && this.velocity.X < 2f)
                                            {
                                                this.velocity.X = 2f;
                                            }
                                            if (this.direction == 1 && this.velocity.X < 0f && this.velocity.X > -2f)
                                            {
                                                this.velocity.X = -2f;
                                            }
                                            this.netUpdate = true;
                                        }
                                        if (this.collideY)
                                        {
                                            this.netUpdate = true;
                                            this.velocity.Y = this.oldVelocity.Y * -num41;
                                            if (this.velocity.Y > 0f && (double)this.velocity.Y < 1.5)
                                            {
                                                this.velocity.Y = 2f;
                                            }
                                            if (this.velocity.Y < 0f && (double)this.velocity.Y > -1.5)
                                            {
                                                this.velocity.Y = -2f;
                                            }
                                        }
                                        if (this.type == 23)
                                        {
                                            Vector2 arg_368F_0 = new Vector2(this.position.X - this.velocity.X, this.position.Y - this.velocity.Y);
                                            int arg_368F_1 = this.width;
                                            int arg_368F_2 = this.height;
                                            int arg_368F_3 = 6;
                                            float arg_368F_4 = this.velocity.X * 0.2f;
                                            float arg_368F_5 = this.velocity.Y * 0.2f;
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
                                                int num43 = Dust.NewDust(new Vector2(this.position.X, this.position.Y + (float)this.height * 0.25f), this.width, (int)((float)this.height * 0.5f), 18, this.velocity.X, 2f, this.alpha, this.color, this.scale);
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
                                            Vector2 arg_380E_0 = new Vector2(this.position.X, this.position.Y + (float)this.height * 0.25f);
                                            int arg_380E_1 = this.width;
                                            int arg_380E_2 = (int)((float)this.height * 0.5f);
                                            int arg_380E_3 = 5;
                                            float arg_380E_4 = this.velocity.X;
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
                                    if ((Main.dayTime && this.type != 6 && this.type != 23 && this.type != 42) || Main.players[this.target].dead)
                                    {
                                        this.velocity.Y = this.velocity.Y - num36 * 2f;
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
                                        if (this.target < 0 || this.target == 255 || Main.players[this.target].dead)
                                        {
                                            this.TargetClosest(true);
                                        }
                                        if (Main.players[this.target].dead && this.timeLeft > 10)
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
                                                    this.ai[0] = (float)NPC.NewNPC((int)(this.position.X + (float)(this.width / 2)), (int)(this.position.Y + (float)this.height), this.type + 1, this.whoAmI);
                                                }
                                                else
                                                {
                                                    if ((this.type == 8 || this.type == 11 || this.type == 14 || this.type == 40) && this.ai[2] > 0f)
                                                    {
                                                        this.ai[0] = (float)NPC.NewNPC((int)(this.position.X + (float)(this.width / 2)), (int)(this.position.Y + (float)this.height), this.type, this.whoAmI);
                                                    }
                                                    else
                                                    {
                                                        this.ai[0] = (float)NPC.NewNPC((int)(this.position.X + (float)(this.width / 2)), (int)(this.position.Y + (float)this.height), this.type + 1, this.whoAmI);
                                                    }
                                                }
                                                Main.npc[(int)this.ai[0]].ai[1] = (float)this.whoAmI;
                                                Main.npc[(int)this.ai[0]].ai[2] = this.ai[2] - 1f;
                                                this.netUpdate = true;
                                            }
                                            if ((this.type == 8 || this.type == 9 || this.type == 11 || this.type == 12 || this.type == 40 || this.type == 41) && (!Main.npc[(int)this.ai[1]].active || Main.npc[(int)this.ai[1]].aiStyle != this.aiStyle))
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
                                                    int num45 = this.whoAmI;
                                                    int num46 = this.life;
                                                    float num47 = this.ai[0];
                                                    this.SetDefaults(this.type);
                                                    this.life = num46;
                                                    if (this.life > this.lifeMax)
                                                    {
                                                        this.life = this.lifeMax;
                                                    }
                                                    this.ai[0] = num47;
                                                    this.TargetClosest(true);
                                                    this.netUpdate = true;
                                                    this.whoAmI = num45;
                                                }
                                                if (this.type == 14 && !Main.npc[(int)this.ai[0]].active)
                                                {
                                                    int num48 = this.life;
                                                    int num49 = this.whoAmI;
                                                    float num50 = this.ai[1];
                                                    this.SetDefaults(this.type);
                                                    this.life = num48;
                                                    if (this.life > this.lifeMax)
                                                    {
                                                        this.life = this.lifeMax;
                                                    }
                                                    this.ai[1] = num50;
                                                    this.TargetClosest(true);
                                                    this.netUpdate = true;
                                                    this.whoAmI = num49;
                                                }
                                                if (this.life == 0)
                                                {
                                                    bool flag5 = true;
                                                    for (int l = 0; l < 1000; l++)
                                                    {
                                                        if (Main.npc[l].active && (Main.npc[l].type == 13 || Main.npc[l].type == 14 || Main.npc[l].type == 15))
                                                        {
                                                            flag5 = false;
                                                            break;
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
                                                NetMessage.SendData(28, -1, -1, "", this.whoAmI, -1f);
                                            }
                                        }
                                        int num51 = (int)(this.position.X / 16f) - 1;
                                        int num52 = (int)((this.position.X + (float)this.width) / 16f) + 2;
                                        int num53 = (int)(this.position.Y / 16f) - 1;
                                        int num54 = (int)((this.position.Y + (float)this.height) / 16f) + 2;
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
                                                if (Main.tile[m, n] != null && ((Main.tile[m, n].active && (Main.tileSolid[(int)Main.tile[m, n].type] || (Main.tileSolidTop[(int)Main.tile[m, n].type] && Main.tile[m, n].frameY == 0))) || Main.tile[m, n].liquid > 64))
                                                {
                                                    Vector2 vector8;
                                                    vector8.X = (float)(m * 16);
                                                    vector8.Y = (float)(n * 16);
                                                    if (this.position.X + (float)this.width > vector8.X && this.position.X < vector8.X + 16f && this.position.Y + (float)this.height > vector8.Y && this.position.Y < vector8.Y + 16f)
                                                    {
                                                        flag6 = true;
                                                        if (Main.rand.Next(40) == 0 && Main.tile[m, n].active)
                                                        {
                                                            WorldGen.KillTile(m, n, true, true, false);
                                                        }
                                                        if (Main.netMode != 1 && Main.tile[m, n].type == 2)
                                                        {
                                                            byte arg_4132_0 = Main.tile[m, n - 1].type;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        float num55 = 8f;
                                        float num56 = 0.07f;
                                        if (this.type == 10)
                                        {
                                            num55 = 6f;
                                            num56 = 0.05f;
                                        }
                                        if (this.type == 13)
                                        {
                                            num55 = 11f;
                                            num56 = 0.08f;
                                        }
                                        Vector2 vector9 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                        float num57 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector9.X;
                                        float num58 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - vector9.Y;
                                        float num59 = (float)Math.Sqrt((double)(num57 * num57 + num58 * num58));
                                        if (this.ai[1] > 0f)
                                        {
                                            num57 = Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - vector9.X;
                                            num58 = Main.npc[(int)this.ai[1]].position.Y + (float)(Main.npc[(int)this.ai[1]].height / 2) - vector9.Y;
                                            this.rotation = (float)Math.Atan2((double)num58, (double)num57) + 1.57f;
                                            num59 = (float)Math.Sqrt((double)(num57 * num57 + num58 * num58));
                                            num59 = (num59 - (float)this.width) / num59;
                                            num57 *= num59;
                                            num58 *= num59;
                                            this.velocity = default(Vector2);
                                            this.position.X = this.position.X + num57;
                                            this.position.Y = this.position.Y + num58;
                                            return;
                                        }
                                        if (!flag6)
                                        {
                                            this.TargetClosest(true);
                                            this.velocity.Y = this.velocity.Y + 0.11f;
                                            if (this.velocity.Y > num55)
                                            {
                                                this.velocity.Y = num55;
                                            }
                                            if ((double)(Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) < (double)num55 * 0.4)
                                            {
                                                if (this.velocity.X < 0f)
                                                {
                                                    this.velocity.X = this.velocity.X - num56 * 1.1f;
                                                }
                                                else
                                                {
                                                    this.velocity.X = this.velocity.X + num56 * 1.1f;
                                                }
                                            }
                                            else
                                            {
                                                if (this.velocity.Y == num55)
                                                {
                                                    if (this.velocity.X < num57)
                                                    {
                                                        this.velocity.X = this.velocity.X + num56;
                                                    }
                                                    else
                                                    {
                                                        if (this.velocity.X > num57)
                                                        {
                                                            this.velocity.X = this.velocity.X - num56;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.velocity.Y > 4f)
                                                    {
                                                        if (this.velocity.X < 0f)
                                                        {
                                                            this.velocity.X = this.velocity.X + num56 * 0.9f;
                                                        }
                                                        else
                                                        {
                                                            this.velocity.X = this.velocity.X - num56 * 0.9f;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (this.soundDelay == 0)
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
                                                this.soundDelay = (int)num60;
                                            }
                                            num59 = (float)Math.Sqrt((double)(num57 * num57 + num58 * num58));
                                            float num61 = Math.Abs(num57);
                                            float num62 = Math.Abs(num58);
                                            num59 = num55 / num59;
                                            num57 *= num59;
                                            num58 *= num59;
                                            if ((this.type == 13 || this.type == 7) && !Main.players[this.target].zoneEvil)
                                            {
                                                if ((double)(this.position.Y / 16f) > Main.rockLayer && this.timeLeft > 2)
                                                {
                                                    this.timeLeft = 2;
                                                }
                                                num57 = 0f;
                                                num58 = num55;
                                            }
                                            if ((this.velocity.X > 0f && num57 > 0f) || (this.velocity.X < 0f && num57 < 0f) || (this.velocity.Y > 0f && num58 > 0f) || (this.velocity.Y < 0f && num58 < 0f))
                                            {
                                                if (this.velocity.X < num57)
                                                {
                                                    this.velocity.X = this.velocity.X + num56;
                                                }
                                                else
                                                {
                                                    if (this.velocity.X > num57)
                                                    {
                                                        this.velocity.X = this.velocity.X - num56;
                                                    }
                                                }
                                                if (this.velocity.Y < num58)
                                                {
                                                    this.velocity.Y = this.velocity.Y + num56;
                                                }
                                                else
                                                {
                                                    if (this.velocity.Y > num58)
                                                    {
                                                        this.velocity.Y = this.velocity.Y - num56;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (num61 > num62)
                                                {
                                                    if (this.velocity.X < num57)
                                                    {
                                                        this.velocity.X = this.velocity.X + num56 * 1.1f;
                                                    }
                                                    else
                                                    {
                                                        if (this.velocity.X > num57)
                                                        {
                                                            this.velocity.X = this.velocity.X - num56 * 1.1f;
                                                        }
                                                    }
                                                    if ((double)(Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) < (double)num55 * 0.5)
                                                    {
                                                        if (this.velocity.Y > 0f)
                                                        {
                                                            this.velocity.Y = this.velocity.Y + num56;
                                                        }
                                                        else
                                                        {
                                                            this.velocity.Y = this.velocity.Y - num56;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.velocity.Y < num58)
                                                    {
                                                        this.velocity.Y = this.velocity.Y + num56 * 1.1f;
                                                    }
                                                    else
                                                    {
                                                        if (this.velocity.Y > num58)
                                                        {
                                                            this.velocity.Y = this.velocity.Y - num56 * 1.1f;
                                                        }
                                                    }
                                                    if ((double)(Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) < (double)num55 * 0.5)
                                                    {
                                                        if (this.velocity.X > 0f)
                                                        {
                                                            this.velocity.X = this.velocity.X + num56;
                                                        }
                                                        else
                                                        {
                                                            this.velocity.X = this.velocity.X - num56;
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
                                            int num63 = (int)(this.position.X + (float)(this.width / 2)) / 16;
                                            int num64 = (int)(this.position.Y + (float)this.height + 1f) / 16;
                                            if (Main.netMode == 1 || !this.townNPC)
                                            {
                                                this.homeTileX = num63;
                                                this.homeTileY = num64;
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
                                            for (int num65 = 0; num65 < 255; num65++)
                                            {
                                                if (Main.players[num65].active && Main.players[num65].talkNPC == this.whoAmI)
                                                {
                                                    flag7 = true;
                                                    if (this.ai[0] != 0f)
                                                    {
                                                        this.netUpdate = true;
                                                    }
                                                    this.ai[0] = 0f;
                                                    this.ai[1] = 300f;
                                                    this.ai[2] = 100f;
                                                    if (Main.players[num65].position.X + (float)(Main.players[num65].width / 2) < this.position.X + (float)(this.width / 2))
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
                                            }
                                            if (Main.netMode != 1 && this.townNPC && !Main.dayTime && (num63 != this.homeTileX || num64 != this.homeTileY) && !this.homeless)
                                            {
                                                bool flag8 = true;
                                                for (int num66 = 0; num66 < 2; num66++)
                                                {
                                                    Rectangle rectangle = new Rectangle((int)(this.position.X + (float)(this.width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(this.position.Y + (float)(this.height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                                    if (num66 == 1)
                                                    {
                                                        rectangle = new Rectangle(this.homeTileX * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, this.homeTileY * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                                    }
                                                    for (int num67 = 0; num67 < 255; num67++)
                                                    {
                                                        if (Main.players[num67].active)
                                                        {
                                                            Rectangle rectangle2 = new Rectangle((int)Main.players[num67].position.X, (int)Main.players[num67].position.Y, Main.players[num67].width, Main.players[num67].height);
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
                                                if (!Main.dayTime && !flag7)
                                                {
                                                    if (Main.netMode != 1)
                                                    {
                                                        if (num63 == this.homeTileX && num64 == this.homeTileY)
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
                                                                if (num63 > this.homeTileX)
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
                                                if (Main.netMode != 1 && (Main.dayTime || (num63 == this.homeTileX && num64 == this.homeTileY)))
                                                {
                                                    if (num63 < this.homeTileX - 25 || num63 > this.homeTileX + 25)
                                                    {
                                                        if (this.ai[2] == 0f)
                                                        {
                                                            if (num63 < this.homeTileX - 50 && this.direction == -1)
                                                            {
                                                                this.direction = 1;
                                                                this.netUpdate = true;
                                                                return;
                                                            }
                                                            if (num63 > this.homeTileX + 50 && this.direction == 1)
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
                                                    if (Main.netMode != 1 && !Main.dayTime && num63 == this.homeTileX && num64 == this.homeTileY)
                                                    {
                                                        this.ai[0] = 0f;
                                                        this.ai[1] = (float)(200 + Main.rand.Next(200));
                                                        this.ai[2] = 60f;
                                                        this.netUpdate = true;
                                                        return;
                                                    }
                                                    if (Main.netMode != 1 && !this.homeless && (num63 < this.homeTileX - 35 || num63 > this.homeTileX + 35))
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
                                                        if (this.type == 46)
                                                        {
                                                            this.ai[1] -= (float)Main.rand.Next(100);
                                                        }
                                                        this.ai[2] = 60f;
                                                        this.netUpdate = true;
                                                    }
                                                    if (this.closeDoor && ((this.position.X + (float)(this.width / 2)) / 16f > (float)(this.doorX + 2) || (this.position.X + (float)(this.width / 2)) / 16f < (float)(this.doorX - 2)))
                                                    {
                                                        bool flag9 = WorldGen.CloseDoor(this.doorX, this.doorY, false, DoorOpener.NPC);
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
                                                        int num68 = (int)((this.position.X + (float)(this.width / 2) + (float)(15 * this.direction)) / 16f);
                                                        int num69 = (int)((this.position.Y + (float)this.height - 16f) / 16f);
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
                                                        if (Main.tile[num68 + this.direction, num69 - 1] == null)
                                                        {
                                                            Main.tile[num68 + this.direction, num69 - 1] = new Tile();
                                                        }
                                                        if (Main.tile[num68 + this.direction, num69 + 1] == null)
                                                        {
                                                            Main.tile[num68 + this.direction, num69 + 1] = new Tile();
                                                        }
                                                        if (this.townNPC && Main.tile[num68, num69 - 2].active && Main.tile[num68, num69 - 2].type == 10 && (Main.rand.Next(10) == 0 || !Main.dayTime))
                                                        {
                                                            if (Main.netMode != 1)
                                                            {
                                                                bool flag10 = WorldGen.OpenDoor(num68, num69 - 2, this.direction, this.closeDoor, DoorOpener.NPC);
                                                                if (flag10)
                                                                {
                                                                    this.closeDoor = true;
                                                                    this.doorX = num68;
                                                                    this.doorY = num69 - 2;
                                                                    NetMessage.SendData(19, -1, -1, "", 0, (float)num68, (float)(num69 - 2), (float)this.direction);
                                                                    this.netUpdate = true;
                                                                    this.ai[1] += 80f;
                                                                    return;
                                                                }
                                                                if (WorldGen.OpenDoor(num68, num69 - 2, -this.direction, this.closeDoor, DoorOpener.NPC))
                                                                {
                                                                    this.closeDoor = true;
                                                                    this.doorX = num68;
                                                                    this.doorY = num69 - 2;
                                                                    NetMessage.SendData(19, -1, -1, "", 0, (float)num68, (float)(num69 - 2), (float)(-(float)this.direction));
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
                                                                if (Main.tile[num68, num69 - 2].active && Main.tileSolid[(int)Main.tile[num68, num69 - 2].type] && !Main.tileSolidTop[(int)Main.tile[num68, num69 - 2].type])
                                                                {
                                                                    if ((this.direction == 1 && !Collision.SolidTiles(num68 - 2, num68 - 1, num69 - 5, num69 - 1)) || (this.direction == -1 && !Collision.SolidTiles(num68 + 1, num68 + 2, num69 - 5, num69 - 1)))
                                                                    {
                                                                        if (!Collision.SolidTiles(num68, num68, num69 - 5, num69 - 3))
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
                                                                    if (Main.tile[num68, num69 - 1].active && Main.tileSolid[(int)Main.tile[num68, num69 - 1].type] && !Main.tileSolidTop[(int)Main.tile[num68, num69 - 1].type])
                                                                    {
                                                                        if ((this.direction == 1 && !Collision.SolidTiles(num68 - 2, num68 - 1, num69 - 4, num69 - 1)) || (this.direction == -1 && !Collision.SolidTiles(num68 + 1, num68 + 2, num69 - 4, num69 - 1)))
                                                                        {
                                                                            if (!Collision.SolidTiles(num68, num68, num69 - 4, num69 - 2))
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
                                                                        if (Main.tile[num68, num69].active && Main.tileSolid[(int)Main.tile[num68, num69].type] && !Main.tileSolidTop[(int)Main.tile[num68, num69].type])
                                                                        {
                                                                            if ((this.direction == 1 && !Collision.SolidTiles(num68 - 2, num68, num69 - 3, num69 - 1)) || (this.direction == -1 && !Collision.SolidTiles(num68, num68 + 2, num69 - 3, num69 - 1)))
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
                                                                try
                                                                {
                                                                    if (Main.tile[num68, num69 + 1] == null)
                                                                    {
                                                                        Main.tile[num68, num69 + 1] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68 - this.direction, num69 + 1] == null)
                                                                    {
                                                                        Main.tile[num68 - this.direction, num69 + 1] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68, num69 + 2] == null)
                                                                    {
                                                                        Main.tile[num68, num69 + 2] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68 - this.direction, num69 + 2] == null)
                                                                    {
                                                                        Main.tile[num68 - this.direction, num69 + 2] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68, num69 + 3] == null)
                                                                    {
                                                                        Main.tile[num68, num69 + 3] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68 - this.direction, num69 + 3] == null)
                                                                    {
                                                                        Main.tile[num68 - this.direction, num69 + 3] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68, num69 + 4] == null)
                                                                    {
                                                                        Main.tile[num68, num69 + 4] = new Tile();
                                                                    }
                                                                    if (Main.tile[num68 - this.direction, num69 + 4] == null)
                                                                    {
                                                                        Main.tile[num68 - this.direction, num69 + 4] = new Tile();
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num63 >= this.homeTileX - 35 && num63 <= this.homeTileX + 35 && (!Main.tile[num68, num69 + 1].active || !Main.tileSolid[(int)Main.tile[num68, num69 + 1].type]) && (!Main.tile[num68 - this.direction, num69 + 1].active || !Main.tileSolid[(int)Main.tile[num68 - this.direction, num69 + 1].type]) && (!Main.tile[num68, num69 + 2].active || !Main.tileSolid[(int)Main.tile[num68, num69 + 2].type]) && (!Main.tile[num68 - this.direction, num69 + 2].active || !Main.tileSolid[(int)Main.tile[num68 - this.direction, num69 + 2].type]) && (!Main.tile[num68, num69 + 3].active || !Main.tileSolid[(int)Main.tile[num68, num69 + 3].type]) && (!Main.tile[num68 - this.direction, num69 + 3].active || !Main.tileSolid[(int)Main.tile[num68 - this.direction, num69 + 3].type]) && (!Main.tile[num68, num69 + 4].active || !Main.tileSolid[(int)Main.tile[num68, num69 + 4].type]) && (!Main.tile[num68 - this.direction, num69 + 4].active || !Main.tileSolid[(int)Main.tile[num68 - this.direction, num69 + 4].type]) && this.type != 46)
                                                                        {
                                                                            this.direction *= -1;
                                                                            this.velocity.X = this.velocity.X * -1f;
                                                                            this.netUpdate = true;
                                                                        }
                                                                    }
                                                                }
                                                                catch
                                                                {
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
                                                    for (int num70 = 0; num70 < 50; num70++)
                                                    {
                                                        if (this.type == 29 || this.type == 45)
                                                        {
                                                            Vector2 arg_5FFC_0 = new Vector2(this.position.X, this.position.Y);
                                                            int arg_5FFC_1 = this.width;
                                                            int arg_5FFC_2 = this.height;
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
                                                            if (this.type == 32)
                                                            {
                                                                Vector2 arg_6098_0 = new Vector2(this.position.X, this.position.Y);
                                                                int arg_6098_1 = this.width;
                                                                int arg_6098_2 = this.height;
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
                                                                Vector2 arg_610F_0 = new Vector2(this.position.X, this.position.Y);
                                                                int arg_610F_1 = this.width;
                                                                int arg_610F_2 = this.height;
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
                                                    this.position.X = this.ai[2] * 16f - (float)(this.width / 2) + 8f;
                                                    this.position.Y = this.ai[3] * 16f - (float)this.height;
                                                    this.velocity.X = 0f;
                                                    this.velocity.Y = 0f;
                                                    this.ai[2] = 0f;
                                                    this.ai[3] = 0f;
                                                    for (int num74 = 0; num74 < 50; num74++)
                                                    {
                                                        if (this.type == 29 || this.type == 45)
                                                        {
                                                            Vector2 arg_625E_0 = new Vector2(this.position.X, this.position.Y);
                                                            int arg_625E_1 = this.width;
                                                            int arg_625E_2 = this.height;
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
                                                            if (this.type == 32)
                                                            {
                                                                Vector2 arg_62FA_0 = new Vector2(this.position.X, this.position.Y);
                                                                int arg_62FA_1 = this.width;
                                                                int arg_62FA_2 = this.height;
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
                                                                Vector2 arg_6371_0 = new Vector2(this.position.X, this.position.Y);
                                                                int arg_6371_1 = this.width;
                                                                int arg_6371_2 = this.height;
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
                                                this.ai[0] += 1f;
                                                if (this.ai[0] == 100f || this.ai[0] == 200f || this.ai[0] == 300f)
                                                {
                                                    this.ai[1] = 30f;
                                                    this.netUpdate = true;
                                                }
                                                else
                                                {
                                                    if (this.ai[0] >= 650f && Main.netMode != 1)
                                                    {
                                                        this.ai[0] = 1f;
                                                        int num78 = (int)Main.players[this.target].position.X / 16;
                                                        int num79 = (int)Main.players[this.target].position.Y / 16;
                                                        int num80 = (int)this.position.X / 16;
                                                        int num81 = (int)this.position.Y / 16;
                                                        int num82 = 20;
                                                        int num83 = 0;
                                                        bool flag11 = false;
                                                        if (Math.Abs(this.position.X - Main.players[this.target].position.X) + Math.Abs(this.position.Y - Main.players[this.target].position.Y) > 2000f)
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
                                                                if ((num86 < num79 - 4 || num86 > num79 + 4 || num84 < num78 - 4 || num84 > num78 + 4) && (num86 < num81 - 1 || num86 > num81 + 1 || num84 < num80 - 1 || num84 > num80 + 1) && Main.tile[num84, num86].active)
                                                                {
                                                                    bool flag12 = true;
                                                                    if (this.type == 32 && Main.tile[num84, num86 - 1].wall == 0)
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
                                                                        this.ai[1] = 20f;
                                                                        this.ai[2] = (float)num84;
                                                                        this.ai[3] = (float)num86;
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
                                                        Vector2 arg_680C_0 = new Vector2(this.position.X, this.position.Y + 2f);
                                                        int arg_680C_1 = this.width;
                                                        int arg_680C_2 = this.height;
                                                        int arg_680C_3 = 27;
                                                        float arg_680C_4 = this.velocity.X * 0.2f;
                                                        float arg_680C_5 = this.velocity.Y * 0.2f;
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
                                                    if (this.type == 32)
                                                    {
                                                        if (Main.rand.Next(2) == 0)
                                                        {
                                                            Vector2 arg_68D6_0 = new Vector2(this.position.X, this.position.Y + 2f);
                                                            int arg_68D6_1 = this.width;
                                                            int arg_68D6_2 = this.height;
                                                            int arg_68D6_3 = 29;
                                                            float arg_68D6_4 = this.velocity.X * 0.2f;
                                                            float arg_68D6_5 = this.velocity.Y * 0.2f;
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
                                                            Vector2 arg_6999_0 = new Vector2(this.position.X, this.position.Y + 2f);
                                                            int arg_6999_1 = this.width;
                                                            int arg_6999_2 = this.height;
                                                            int arg_6999_3 = 6;
                                                            float arg_6999_4 = this.velocity.X * 0.2f;
                                                            float arg_6999_5 = this.velocity.Y * 0.2f;
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
                                                if (this.aiStyle == 9)
                                                {
                                                    if (this.target == 255)
                                                    {
                                                        this.TargetClosest(true);
                                                        float num90 = 6f;
                                                        if (this.type == 30)
                                                        {
                                                            NPC.maxSpawns = 8;
                                                        }
                                                        Vector2 vector10 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                        float num91 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector10.X;
                                                        float num92 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - vector10.Y;
                                                        float num93 = (float)Math.Sqrt((double)(num91 * num91 + num92 * num92));
                                                        num93 = num90 / num93;
                                                        this.velocity.X = num91 * num93;
                                                        this.velocity.Y = num92 * num93;
                                                    }
                                                    if (this.timeLeft > 100)
                                                    {
                                                        this.timeLeft = 100;
                                                    }
                                                    for (int num94 = 0; num94 < 2; num94++)
                                                    {
                                                        if (this.type == 30)
                                                        {
                                                            Vector2 arg_6B8D_0 = new Vector2(this.position.X, this.position.Y + 2f);
                                                            int arg_6B8D_1 = this.width;
                                                            int arg_6B8D_2 = this.height;
                                                            int arg_6B8D_3 = 27;
                                                            float arg_6B8D_4 = this.velocity.X * 0.2f;
                                                            float arg_6B8D_5 = this.velocity.Y * 0.2f;
                                                            int arg_6B8D_6 = 100;
                                                            Color newColor = default(Color);
                                                            int num95 = Dust.NewDust(arg_6B8D_0, arg_6B8D_1, arg_6B8D_2, arg_6B8D_3, arg_6B8D_4, arg_6B8D_5, arg_6B8D_6, newColor, 2f);
                                                            Main.dust[num95].noGravity = true;
                                                            Dust expr_6BAA = Main.dust[num95];
                                                            expr_6BAA.velocity *= 0.3f;
                                                            Dust expr_6BCC_cp_0 = Main.dust[num95];
                                                            expr_6BCC_cp_0.velocity.X = expr_6BCC_cp_0.velocity.X - this.velocity.X * 0.2f;
                                                            Dust expr_6BF6_cp_0 = Main.dust[num95];
                                                            expr_6BF6_cp_0.velocity.Y = expr_6BF6_cp_0.velocity.Y - this.velocity.Y * 0.2f;
                                                        }
                                                        else
                                                        {
                                                            if (this.type == 33)
                                                            {
                                                                Vector2 arg_6C87_0 = new Vector2(this.position.X, this.position.Y + 2f);
                                                                int arg_6C87_1 = this.width;
                                                                int arg_6C87_2 = this.height;
                                                                int arg_6C87_3 = 29;
                                                                float arg_6C87_4 = this.velocity.X * 0.2f;
                                                                float arg_6C87_5 = this.velocity.Y * 0.2f;
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
                                                                Vector2 arg_6D3E_0 = new Vector2(this.position.X, this.position.Y + 2f);
                                                                int arg_6D3E_1 = this.width;
                                                                int arg_6D3E_2 = this.height;
                                                                int arg_6D3E_3 = 6;
                                                                float arg_6D3E_4 = this.velocity.X * 0.2f;
                                                                float arg_6D3E_5 = this.velocity.Y * 0.2f;
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
                                                    this.rotation += 0.4f * (float)this.direction;
                                                    return;
                                                }
                                                if (this.aiStyle == 10)
                                                {
                                                    float num98 = 1f;
                                                    float num99 = 0.011f;
                                                    this.TargetClosest(true);
                                                    Vector2 vector11 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                    float num100 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector11.X;
                                                    float num101 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - vector11.Y;
                                                    float num102 = (float)Math.Sqrt((double)(num100 * num100 + num101 * num101));
                                                    float num103 = num102;
                                                    this.ai[1] += 1f;
                                                    if (this.ai[1] > 1000f)
                                                    {
                                                        num99 *= 6f;
                                                        num98 = 5f;
                                                        if (this.ai[1] > 1030f)
                                                        {
                                                            this.ai[1] = 0f;
                                                        }
                                                    }
                                                    if (num103 < 300f)
                                                    {
                                                        this.ai[0] += 0.9f;
                                                        if (this.ai[0] > 0f)
                                                        {
                                                            this.velocity.Y = this.velocity.Y + 0.019f;
                                                        }
                                                        else
                                                        {
                                                            this.velocity.Y = this.velocity.Y - 0.019f;
                                                        }
                                                        if (this.ai[0] < -100f || this.ai[0] > 100f)
                                                        {
                                                            this.velocity.X = this.velocity.X + 0.019f;
                                                        }
                                                        else
                                                        {
                                                            this.velocity.X = this.velocity.X - 0.019f;
                                                        }
                                                        if (this.ai[0] > 200f)
                                                        {
                                                            this.ai[0] = -200f;
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
                                                    if (Main.players[this.target].dead)
                                                    {
                                                        num100 = (float)this.direction * num98 / 2f;
                                                        num101 = -num98 / 2f;
                                                    }
                                                    if (this.velocity.X < num100)
                                                    {
                                                        this.velocity.X = this.velocity.X + num99;
                                                    }
                                                    else
                                                    {
                                                        if (this.velocity.X > num100)
                                                        {
                                                            this.velocity.X = this.velocity.X - num99;
                                                        }
                                                    }
                                                    if (this.velocity.Y < num101)
                                                    {
                                                        this.velocity.Y = this.velocity.Y + num99;
                                                    }
                                                    else
                                                    {
                                                        if (this.velocity.Y > num101)
                                                        {
                                                            this.velocity.Y = this.velocity.Y - num99;
                                                        }
                                                    }
                                                    if (num100 > 0f)
                                                    {
                                                        this.spriteDirection = -1;
                                                        this.rotation = (float)Math.Atan2((double)num101, (double)num100);
                                                    }
                                                    if (num100 < 0f)
                                                    {
                                                        this.spriteDirection = 1;
                                                        this.rotation = (float)Math.Atan2((double)num101, (double)num100) + 3.14f;
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.aiStyle == 11)
                                                    {
                                                        if (this.ai[0] == 0f && Main.netMode != 1)
                                                        {
                                                            this.TargetClosest(true);
                                                            this.ai[0] = 1f;
                                                            if (this.type != 68)
                                                            {
                                                                int num104 = NPC.NewNPC((int)(this.position.X + (float)(this.width / 2)), (int)this.position.Y + this.height / 2, 36, this.whoAmI);
                                                                Main.npc[num104].ai[0] = -1f;
                                                                Main.npc[num104].ai[1] = (float)this.whoAmI;
                                                                Main.npc[num104].target = this.target;
                                                                Main.npc[num104].netUpdate = true;
                                                                num104 = NPC.NewNPC((int)(this.position.X + (float)(this.width / 2)), (int)this.position.Y + this.height / 2, 36, this.whoAmI);
                                                                Main.npc[num104].ai[0] = 1f;
                                                                Main.npc[num104].ai[1] = (float)this.whoAmI;
                                                                Main.npc[num104].ai[3] = 150f;
                                                                Main.npc[num104].target = this.target;
                                                                Main.npc[num104].netUpdate = true;
                                                            }
                                                        }
                                                        if (this.type == 68 && this.ai[1] != 3f && this.ai[1] != 2f)
                                                        {
                                                            this.ai[1] = 2f;
                                                        }
                                                        if (Main.players[this.target].dead || Math.Abs(this.position.X - Main.players[this.target].position.X) > 2000f || Math.Abs(this.position.Y - Main.players[this.target].position.Y) > 2000f)
                                                        {
                                                            this.TargetClosest(true);
                                                            if (Main.players[this.target].dead || Math.Abs(this.position.X - Main.players[this.target].position.X) > 2000f || Math.Abs(this.position.Y - Main.players[this.target].position.Y) > 2000f)
                                                            {
                                                                this.ai[1] = 3f;
                                                            }
                                                        }
                                                        if (Main.dayTime && this.ai[1] != 3f && this.ai[1] != 2f)
                                                        {
                                                            this.ai[1] = 2f;
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
                                                            if (this.position.Y > Main.players[this.target].position.Y - 250f)
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
                                                                if (this.position.Y < Main.players[this.target].position.Y - 250f)
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
                                                            if (this.position.X + (float)(this.width / 2) > Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2))
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
                                                            if (this.position.X + (float)(this.width / 2) < Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2))
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
                                                                if (this.ai[2] >= 400f)
                                                                {
                                                                    this.ai[2] = 0f;
                                                                    this.ai[1] = 0f;
                                                                }
                                                                this.rotation += (float)this.direction * 0.3f;
                                                                Vector2 vector12 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                float num105 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector12.X;
                                                                float num106 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - vector12.Y;
                                                                float num107 = (float)Math.Sqrt((double)(num105 * num105 + num106 * num106));
                                                                num107 = 2f / num107;
                                                                this.velocity.X = num105 * num107;
                                                                this.velocity.Y = num106 * num107;
                                                            }
                                                            else
                                                            {
                                                                if (this.ai[1] == 2f)
                                                                {
                                                                    this.damage = 9999;
                                                                    this.defense = 9999;
                                                                    this.rotation += (float)this.direction * 0.3f;
                                                                    Vector2 vector13 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                    float num108 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector13.X;
                                                                    float num109 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - vector13.Y;
                                                                    float num110 = (float)Math.Sqrt((double)(num108 * num108 + num109 * num109));
                                                                    num110 = 8f / num110;
                                                                    this.velocity.X = num108 * num110;
                                                                    this.velocity.Y = num109 * num110;
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
                                                        if (this.ai[1] != 2f && this.ai[1] != 3f && this.type != 68)
                                                        {
                                                            Vector2 arg_7AE4_0 = new Vector2(this.position.X + (float)(this.width / 2) - 15f - this.velocity.X * 5f, this.position.Y + (float)this.height - 2f);
                                                            int arg_7AE4_1 = 30;
                                                            int arg_7AE4_2 = 10;
                                                            int arg_7AE4_3 = 5;
                                                            float arg_7AE4_4 = -this.velocity.X * 0.2f;
                                                            float arg_7AE4_5 = 3f;
                                                            int arg_7AE4_6 = 0;
                                                            Color newColor = default(Color);
                                                            int num111 = Dust.NewDust(arg_7AE4_0, arg_7AE4_1, arg_7AE4_2, arg_7AE4_3, arg_7AE4_4, arg_7AE4_5, arg_7AE4_6, newColor, 2f);
                                                            Main.dust[num111].noGravity = true;
                                                            Dust expr_7B06_cp_0 = Main.dust[num111];
                                                            expr_7B06_cp_0.velocity.X = expr_7B06_cp_0.velocity.X * 1.3f;
                                                            Dust expr_7B24_cp_0 = Main.dust[num111];
                                                            expr_7B24_cp_0.velocity.X = expr_7B24_cp_0.velocity.X + this.velocity.X * 0.4f;
                                                            Dust expr_7B4E_cp_0 = Main.dust[num111];
                                                            expr_7B4E_cp_0.velocity.Y = expr_7B4E_cp_0.velocity.Y + (2f + this.velocity.Y);
                                                            for (int num112 = 0; num112 < 2; num112++)
                                                            {
                                                                Vector2 arg_7BC3_0 = new Vector2(this.position.X, this.position.Y + 120f);
                                                                int arg_7BC3_1 = this.width;
                                                                int arg_7BC3_2 = 60;
                                                                int arg_7BC3_3 = 5;
                                                                float arg_7BC3_4 = this.velocity.X;
                                                                float arg_7BC3_5 = this.velocity.Y;
                                                                int arg_7BC3_6 = 0;
                                                                newColor = default(Color);
                                                                num111 = Dust.NewDust(arg_7BC3_0, arg_7BC3_1, arg_7BC3_2, arg_7BC3_3, arg_7BC3_4, arg_7BC3_5, arg_7BC3_6, newColor, 2f);
                                                                Main.dust[num111].noGravity = true;
                                                                Dust expr_7BE0 = Main.dust[num111];
                                                                expr_7BE0.velocity -= this.velocity;
                                                                Dust expr_7C03_cp_0 = Main.dust[num111];
                                                                expr_7C03_cp_0.velocity.Y = expr_7C03_cp_0.velocity.Y + 5f;
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
                                                                Vector2 vector14 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                float num113 = Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 200f * this.ai[0] - vector14.X;
                                                                float num114 = Main.npc[(int)this.ai[1]].position.Y + 230f - vector14.Y;
                                                                Math.Sqrt((double)(num113 * num113 + num114 * num114));
                                                                this.rotation = (float)Math.Atan2((double)num114, (double)num113) + 1.57f;
                                                                return;
                                                            }
                                                            if (this.ai[2] == 1f)
                                                            {
                                                                Vector2 vector15 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                float num115 = Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 200f * this.ai[0] - vector15.X;
                                                                float num116 = Main.npc[(int)this.ai[1]].position.Y + 230f - vector15.Y;
                                                                float num117 = (float)Math.Sqrt((double)(num115 * num115 + num116 * num116));
                                                                this.rotation = (float)Math.Atan2((double)num116, (double)num115) + 1.57f;
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
                                                                    vector15 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                    num115 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector15.X;
                                                                    num116 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - vector15.Y;
                                                                    num117 = (float)Math.Sqrt((double)(num115 * num115 + num116 * num116));
                                                                    num117 = 20f / num117;
                                                                    this.velocity.X = num115 * num117;
                                                                    this.velocity.Y = num116 * num117;
                                                                    this.netUpdate = true;
                                                                    return;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (this.ai[2] == 2f)
                                                                {
                                                                    if (this.position.Y > Main.players[this.target].position.Y || this.velocity.Y < 0f)
                                                                    {
                                                                        this.ai[2] = 3f;
                                                                        return;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (this.ai[2] == 4f)
                                                                    {
                                                                        Vector2 vector16 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                        float num118 = Main.npc[(int)this.ai[1]].position.X + (float)(Main.npc[(int)this.ai[1]].width / 2) - 200f * this.ai[0] - vector16.X;
                                                                        float num119 = Main.npc[(int)this.ai[1]].position.Y + 230f - vector16.Y;
                                                                        float num120 = (float)Math.Sqrt((double)(num118 * num118 + num119 * num119));
                                                                        this.rotation = (float)Math.Atan2((double)num119, (double)num118) + 1.57f;
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
                                                                            vector16 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                            num118 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector16.X;
                                                                            num119 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - vector16.Y;
                                                                            num120 = (float)Math.Sqrt((double)(num118 * num118 + num119 * num119));
                                                                            num120 = 20f / num120;
                                                                            this.velocity.X = num118 * num120;
                                                                            this.velocity.Y = num119 * num120;
                                                                            this.netUpdate = true;
                                                                            return;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (this.ai[2] == 5f && ((this.velocity.X > 0f && this.position.X + (float)(this.width / 2) > Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2)) || (this.velocity.X < 0f && this.position.X + (float)(this.width / 2) < Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2))))
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
                                                                this.TargetClosest(true);
                                                                float num121 = 0.05f;
                                                                float num122 = 150f;
                                                                if (this.type == 43)
                                                                {
                                                                    num122 = 200f;
                                                                }
                                                                Vector2 vector17 = new Vector2(this.ai[0] * 16f + 8f, this.ai[1] * 16f + 8f);
                                                                float num123 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - (float)(this.width / 2) - vector17.X;
                                                                float num124 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - (float)(this.height / 2) - vector17.Y;
                                                                float num125 = (float)Math.Sqrt((double)(num123 * num123 + num124 * num124));
                                                                if (num125 > num122)
                                                                {
                                                                    num125 = num122 / num125;
                                                                    num123 *= num125;
                                                                    num124 *= num125;
                                                                }
                                                                if (this.position.X < this.ai[0] * 16f + 8f + num123)
                                                                {
                                                                    this.velocity.X = this.velocity.X + num121;
                                                                    if (this.velocity.X < 0f && num123 > 0f)
                                                                    {
                                                                        this.velocity.X = this.velocity.X + num121 * 2f;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (this.position.X > this.ai[0] * 16f + 8f + num123)
                                                                    {
                                                                        this.velocity.X = this.velocity.X - num121;
                                                                        if (this.velocity.X > 0f && num123 < 0f)
                                                                        {
                                                                            this.velocity.X = this.velocity.X - num121 * 2f;
                                                                        }
                                                                    }
                                                                }
                                                                if (this.position.Y < this.ai[1] * 16f + 8f + num124)
                                                                {
                                                                    this.velocity.Y = this.velocity.Y + num121;
                                                                    if (this.velocity.Y < 0f && num124 > 0f)
                                                                    {
                                                                        this.velocity.Y = this.velocity.Y + num121 * 2f;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (this.position.Y > this.ai[1] * 16f + 8f + num124)
                                                                    {
                                                                        this.velocity.Y = this.velocity.Y - num121;
                                                                        if (this.velocity.Y > 0f && num124 < 0f)
                                                                        {
                                                                            this.velocity.Y = this.velocity.Y - num121 * 2f;
                                                                        }
                                                                    }
                                                                }
                                                                if (this.type == 43)
                                                                {
                                                                    if (this.velocity.X > 3f)
                                                                    {
                                                                        this.velocity.X = 3f;
                                                                    }
                                                                    if (this.velocity.X < -3f)
                                                                    {
                                                                        this.velocity.X = -3f;
                                                                    }
                                                                    if (this.velocity.Y > 3f)
                                                                    {
                                                                        this.velocity.Y = 3f;
                                                                    }
                                                                    if (this.velocity.Y < -3f)
                                                                    {
                                                                        this.velocity.Y = -3f;
                                                                    }
                                                                }
                                                                else
                                                                {
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
                                                                }
                                                                if (num123 > 0f)
                                                                {
                                                                    this.spriteDirection = 1;
                                                                    this.rotation = (float)Math.Atan2((double)num124, (double)num123);
                                                                }
                                                                if (num123 < 0f)
                                                                {
                                                                    this.spriteDirection = -1;
                                                                    this.rotation = (float)Math.Atan2((double)num124, (double)num123) + 3.14f;
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
                                                                        return;
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (this.aiStyle == 14)
                                                                {
                                                                    if (this.type == 60)
                                                                    {
                                                                        Vector2 arg_908E_0 = new Vector2(this.position.X, this.position.Y);
                                                                        int arg_908E_1 = this.width;
                                                                        int arg_908E_2 = this.height;
                                                                        int arg_908E_3 = 6;
                                                                        float arg_908E_4 = this.velocity.X * 0.2f;
                                                                        float arg_908E_5 = this.velocity.Y * 0.2f;
                                                                        int arg_908E_6 = 100;
                                                                        Color newColor = default(Color);
                                                                        int num126 = Dust.NewDust(arg_908E_0, arg_908E_1, arg_908E_2, arg_908E_3, arg_908E_4, arg_908E_5, arg_908E_6, newColor, 2f);
                                                                        Main.dust[num126].noGravity = true;
                                                                    }
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
                                                                    if (this.type == 49 || this.type == 51 || this.type == 60)
                                                                    {
                                                                        if (this.wet)
                                                                        {
                                                                            if (this.velocity.Y > 0f)
                                                                            {
                                                                                this.velocity.Y = this.velocity.Y * 0.95f;
                                                                            }
                                                                            this.velocity.Y = this.velocity.Y - 0.5f;
                                                                            if (this.velocity.Y < -4f)
                                                                            {
                                                                                this.velocity.Y = -4f;
                                                                            }
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
                                                                    }
                                                                    if (Main.netMode != 1)
                                                                    {
                                                                        if (this.type == 48)
                                                                        {
                                                                            this.ai[0] += 1f;
                                                                            if (this.ai[0] == 30f || this.ai[0] == 60f || this.ai[0] == 90f)
                                                                            {
                                                                                float num127 = 6f;
                                                                                Vector2 vector18 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                                float num128 = Main.players[this.target].position.X + (float)Main.players[this.target].width * 0.5f - vector18.X + (float)Main.rand.Next(-100, 101);
                                                                                float num129 = Main.players[this.target].position.Y + (float)Main.players[this.target].height * 0.5f - vector18.Y + (float)Main.rand.Next(-100, 101);
                                                                                float num130 = (float)Math.Sqrt((double)(num128 * num128 + num129 * num129));
                                                                                num130 = num127 / num130;
                                                                                num128 *= num130;
                                                                                num129 *= num130;
                                                                                int num131 = 15;
                                                                                int num132 = 38;
                                                                                int num133 = Projectile.NewProjectile(vector18.X, vector18.Y, num128, num129, num132, num131, 0f, Main.myPlayer);
                                                                                Main.projectile[num133].timeLeft = 300;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (this.ai[0] >= (float)(400 + Main.rand.Next(400)))
                                                                                {
                                                                                    this.ai[0] = 0f;
                                                                                }
                                                                            }
                                                                        }
                                                                        if (this.type == 62 || this.type == 66)
                                                                        {
                                                                            this.ai[0] += 1f;
                                                                            if (this.ai[0] == 20f || this.ai[0] == 40f || this.ai[0] == 60f || this.ai[0] == 80f)
                                                                            {
                                                                                float num134 = 0.2f;
                                                                                Vector2 vector19 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                                float num135 = Main.players[this.target].position.X + (float)Main.players[this.target].width * 0.5f - vector19.X + (float)Main.rand.Next(-100, 101);
                                                                                float num136 = Main.players[this.target].position.Y + (float)Main.players[this.target].height * 0.5f - vector19.Y + (float)Main.rand.Next(-100, 101);
                                                                                float num137 = (float)Math.Sqrt((double)(num135 * num135 + num136 * num136));
                                                                                num137 = num134 / num137;
                                                                                num135 *= num137;
                                                                                num136 *= num137;
                                                                                int num138 = 25;
                                                                                int num139 = 44;
                                                                                int num140 = Projectile.NewProjectile(vector19.X, vector19.Y, num135, num136, num139, num138, 0f, Main.myPlayer);
                                                                                Main.projectile[num140].timeLeft = 300;
                                                                                return;
                                                                            }
                                                                            if (this.ai[0] >= (float)(300 + Main.rand.Next(300)))
                                                                            {
                                                                                this.ai[0] = 0f;
                                                                                return;
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
                                                                            if (this.target < 255 && ((this.direction == 1 && this.velocity.X < 3f) || (this.direction == -1 && this.velocity.X > -3f)))
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
                                                                        int num141 = Dust.NewDust(this.position, this.width, this.height, 4, this.velocity.X, this.velocity.Y, 255, new Color(0, 80, 255, 80), this.scale * 1.2f);
                                                                        Main.dust[num141].noGravity = true;
                                                                        Dust expr_A082 = Main.dust[num141];
                                                                        expr_A082.velocity *= 0.5f;
                                                                        if (this.life > 0)
                                                                        {
                                                                            float num142 = (float)this.life / (float)this.lifeMax;
                                                                            num142 = num142 * 0.5f + 0.75f;
                                                                            if (num142 != this.scale)
                                                                            {
                                                                                this.position.X = this.position.X + (float)(this.width / 2);
                                                                                this.position.Y = this.position.Y + (float)this.height;
                                                                                this.scale = num142;
                                                                                this.width = (int)(98f * this.scale);
                                                                                this.height = (int)(92f * this.scale);
                                                                                this.position.X = this.position.X - (float)(this.width / 2);
                                                                                this.position.Y = this.position.Y - (float)this.height;
                                                                            }
                                                                            if (Main.netMode != 1)
                                                                            {
                                                                                int num143 = (int)((double)this.lifeMax * 0.05);
                                                                                if ((float)(this.life + num143) < this.ai[3])
                                                                                {
                                                                                    this.ai[3] = (float)this.life;
                                                                                    int num144 = Main.rand.Next(1, 4);
                                                                                    for (int num145 = 0; num145 < num144; num145++)
                                                                                    {
                                                                                        int x = (int)(this.position.X + (float)Main.rand.Next(this.width - 32));
                                                                                        int y = (int)(this.position.Y + (float)Main.rand.Next(this.height - 32));
                                                                                        int num146 = NPC.NewNPC(x, y, 1, 0);
                                                                                        Main.npc[num146].SetDefaults(1);
                                                                                        Main.npc[num146].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                                                                                        Main.npc[num146].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                                                                                        Main.npc[num146].ai[1] = (float)Main.rand.Next(3);
                                                                                        if (Main.netMode == 2 && num146 < 1000)
                                                                                        {
                                                                                            NetMessage.SendData(23, -1, -1, "", num146);
                                                                                        }
                                                                                    }
                                                                                    return;
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
                                                                                    if (Main.players[this.target].wet && !Main.players[this.target].dead)
                                                                                    {
                                                                                        flag13 = true;
                                                                                    }
                                                                                }
                                                                                if (flag13)
                                                                                {
                                                                                    this.TargetClosest(true);
                                                                                    if (this.type == 65)
                                                                                    {
                                                                                        this.velocity.X = this.velocity.X + (float)this.direction * 0.15f;
                                                                                        this.velocity.Y = this.velocity.Y + (float)this.directionY * 0.15f;
                                                                                        if (this.velocity.X > 5f)
                                                                                        {
                                                                                            this.velocity.X = 5f;
                                                                                        }
                                                                                        if (this.velocity.X < -5f)
                                                                                        {
                                                                                            this.velocity.X = -5f;
                                                                                        }
                                                                                        if (this.velocity.Y > 3f)
                                                                                        {
                                                                                            this.velocity.Y = 3f;
                                                                                        }
                                                                                        if (this.velocity.Y < -3f)
                                                                                        {
                                                                                            this.velocity.Y = -3f;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
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
                                                                                    int num147 = (int)(this.position.X + (float)(this.width / 2)) / 16;
                                                                                    int num148 = (int)(this.position.Y + (float)(this.height / 2)) / 16;
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
                                                                                        if (Main.tile[num147, num148 + 1].active)
                                                                                        {
                                                                                            this.ai[0] = -1f;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (Main.tile[num147, num148 + 2].active)
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
                                                                                    if (this.type == 65)
                                                                                    {
                                                                                        this.velocity.X = this.velocity.X * 0.94f;
                                                                                        if ((double)this.velocity.X > -0.2 && (double)this.velocity.X < 0.2)
                                                                                        {
                                                                                            this.velocity.X = 0f;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (Main.netMode != 1)
                                                                                        {
                                                                                            this.velocity.Y = (float)Main.rand.Next(-50, -20) * 0.1f;
                                                                                            this.velocity.X = (float)Main.rand.Next(-20, 20) * 0.1f;
                                                                                            this.netUpdate = true;
                                                                                        }
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
                                                                                return;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (this.aiStyle == 17)
                                                                            {
                                                                                this.noGravity = true;
                                                                                if (this.ai[0] == 0f)
                                                                                {
                                                                                    this.TargetClosest(true);
                                                                                    if (Main.netMode != 1)
                                                                                    {
                                                                                        if (this.velocity.X != 0f || this.velocity.Y != 0f)
                                                                                        {
                                                                                            this.ai[0] = 1f;
                                                                                            this.netUpdate = true;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Rectangle rectangle3 = new Rectangle((int)Main.players[this.target].position.X, (int)Main.players[this.target].position.Y, Main.players[this.target].width, Main.players[this.target].height);
                                                                                            Rectangle rectangle4 = new Rectangle((int)this.position.X - 100, (int)this.position.Y - 100, this.width + 200, this.height + 200);
                                                                                            if (rectangle4.Intersects(rectangle3) || this.life < this.lifeMax)
                                                                                            {
                                                                                                this.ai[0] = 1f;
                                                                                                this.velocity.Y = this.velocity.Y - 6f;
                                                                                                this.netUpdate = true;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (!Main.players[this.target].dead)
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
                                                                                        if (this.direction == -1 && this.velocity.X > -3f)
                                                                                        {
                                                                                            this.velocity.X = this.velocity.X - 0.1f;
                                                                                            if (this.velocity.X > 3f)
                                                                                            {
                                                                                                this.velocity.X = this.velocity.X - 0.1f;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (this.velocity.X > 0f)
                                                                                                {
                                                                                                    this.velocity.X = this.velocity.X - 0.05f;
                                                                                                }
                                                                                            }
                                                                                            if (this.velocity.X < -3f)
                                                                                            {
                                                                                                this.velocity.X = -3f;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (this.direction == 1 && this.velocity.X < 3f)
                                                                                            {
                                                                                                this.velocity.X = this.velocity.X + 0.1f;
                                                                                                if (this.velocity.X < -3f)
                                                                                                {
                                                                                                    this.velocity.X = this.velocity.X + 0.1f;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (this.velocity.X < 0f)
                                                                                                    {
                                                                                                        this.velocity.X = this.velocity.X + 0.05f;
                                                                                                    }
                                                                                                }
                                                                                                if (this.velocity.X > 3f)
                                                                                                {
                                                                                                    this.velocity.X = 3f;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        float num149 = Math.Abs(this.position.X + (float)(this.width / 2) - (Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2)));
                                                                                        float num150 = Main.players[this.target].position.Y - (float)(this.height / 2);
                                                                                        if (num149 > 50f)
                                                                                        {
                                                                                            num150 -= 100f;
                                                                                        }
                                                                                        if (this.position.Y < num150)
                                                                                        {
                                                                                            this.velocity.Y = this.velocity.Y + 0.05f;
                                                                                            if (this.velocity.Y < 0f)
                                                                                            {
                                                                                                this.velocity.Y = this.velocity.Y + 0.01f;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            this.velocity.Y = this.velocity.Y - 0.05f;
                                                                                            if (this.velocity.Y > 0f)
                                                                                            {
                                                                                                this.velocity.Y = this.velocity.Y - 0.01f;
                                                                                            }
                                                                                        }
                                                                                        if (this.velocity.Y < -3f)
                                                                                        {
                                                                                            this.velocity.Y = -3f;
                                                                                        }
                                                                                        if (this.velocity.Y > 3f)
                                                                                        {
                                                                                            this.velocity.Y = 3f;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                if (this.wet)
                                                                                {
                                                                                    if (this.velocity.Y > 0f)
                                                                                    {
                                                                                        this.velocity.Y = this.velocity.Y * 0.95f;
                                                                                    }
                                                                                    this.velocity.Y = this.velocity.Y - 0.5f;
                                                                                    if (this.velocity.Y < -4f)
                                                                                    {
                                                                                        this.velocity.Y = -4f;
                                                                                    }
                                                                                    this.TargetClosest(true);
                                                                                    return;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (this.aiStyle == 18)
                                                                                {
                                                                                    if (this.direction == 0)
                                                                                    {
                                                                                        this.TargetClosest(true);
                                                                                    }
                                                                                    if (!this.wet)
                                                                                    {
                                                                                        this.rotation += this.velocity.X * 0.1f;
                                                                                        if (this.velocity.Y == 0f)
                                                                                        {
                                                                                            this.velocity.X = this.velocity.X * 0.98f;
                                                                                            if ((double)this.velocity.X > -0.01 && (double)this.velocity.X < 0.01)
                                                                                            {
                                                                                                this.velocity.X = 0f;
                                                                                            }
                                                                                        }
                                                                                        this.velocity.Y = this.velocity.Y + 0.2f;
                                                                                        if (this.velocity.Y > 10f)
                                                                                        {
                                                                                            this.velocity.Y = 10f;
                                                                                        }
                                                                                        this.ai[0] = 1f;
                                                                                        return;
                                                                                    }
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
                                                                                    bool flag14 = false;
                                                                                    if (!this.friendly)
                                                                                    {
                                                                                        this.TargetClosest(false);
                                                                                        if (Main.players[this.target].wet && !Main.players[this.target].dead)
                                                                                        {
                                                                                            flag14 = true;
                                                                                        }
                                                                                    }
                                                                                    if (flag14)
                                                                                    {
                                                                                        this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) + 1.57f;
                                                                                        this.velocity *= 0.98f;
                                                                                        float num151 = 0.2f;
                                                                                        if (this.velocity.X > -num151 && this.velocity.X < num151 && this.velocity.Y > -num151 && this.velocity.Y < num151)
                                                                                        {
                                                                                            this.TargetClosest(true);
                                                                                            float num152 = 7f;
                                                                                            Vector2 vector20 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                                            float num153 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector20.X;
                                                                                            float num154 = Main.players[this.target].position.Y + (float)(Main.players[this.target].height / 2) - vector20.Y;
                                                                                            float num155 = (float)Math.Sqrt((double)(num153 * num153 + num154 * num154));
                                                                                            num155 = num152 / num155;
                                                                                            num153 *= num155;
                                                                                            num154 *= num155;
                                                                                            this.velocity.X = num153;
                                                                                            this.velocity.Y = num154;
                                                                                            return;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        this.velocity.X = this.velocity.X + (float)this.direction * 0.02f;
                                                                                        this.rotation = this.velocity.X * 0.4f;
                                                                                        if (this.velocity.X < -1f || this.velocity.X > 1f)
                                                                                        {
                                                                                            this.velocity.X = this.velocity.X * 0.95f;
                                                                                        }
                                                                                        if (this.ai[0] == -1f)
                                                                                        {
                                                                                            this.velocity.Y = this.velocity.Y - 0.01f;
                                                                                            if (this.velocity.Y < -1f)
                                                                                            {
                                                                                                this.ai[0] = 1f;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            this.velocity.Y = this.velocity.Y + 0.01f;
                                                                                            if (this.velocity.Y > 1f)
                                                                                            {
                                                                                                this.ai[0] = -1f;
                                                                                            }
                                                                                        }
                                                                                        int num156 = (int)(this.position.X + (float)(this.width / 2)) / 16;
                                                                                        int num157 = (int)(this.position.Y + (float)(this.height / 2)) / 16;
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
                                                                                            if (Main.tile[num156, num157 + 1].active)
                                                                                            {
                                                                                                this.ai[0] = -1f;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (Main.tile[num156, num157 + 2].active)
                                                                                                {
                                                                                                    this.ai[0] = -1f;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            this.ai[0] = 1f;
                                                                                        }
                                                                                        if ((double)this.velocity.Y > 1.2 || (double)this.velocity.Y < -1.2)
                                                                                        {
                                                                                            this.velocity.Y = this.velocity.Y * 0.99f;
                                                                                            return;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (this.aiStyle == 19)
                                                                                    {
                                                                                        this.TargetClosest(true);
                                                                                        float num158 = 12f;
                                                                                        Vector2 vector21 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                                                        float num159 = Main.players[this.target].position.X + (float)(Main.players[this.target].width / 2) - vector21.X;
                                                                                        float num160 = Main.players[this.target].position.Y - vector21.Y;
                                                                                        float num161 = (float)Math.Sqrt((double)(num159 * num159 + num160 * num160));
                                                                                        num161 = num158 / num161;
                                                                                        num159 *= num161;
                                                                                        num160 *= num161;
                                                                                        bool flag15 = false;
                                                                                        if (this.directionY < 0)
                                                                                        {
                                                                                            this.rotation = (float)(Math.Atan2((double)num160, (double)num159) + 1.57);
                                                                                            flag15 = ((double)this.rotation >= -1.2 && (double)this.rotation <= 1.2);
                                                                                            if ((double)this.rotation < -0.8)
                                                                                            {
                                                                                                this.rotation = -0.8f;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if ((double)this.rotation > 0.8)
                                                                                                {
                                                                                                    this.rotation = 0.8f;
                                                                                                }
                                                                                            }
                                                                                            if (this.velocity.X != 0f)
                                                                                            {
                                                                                                this.velocity.X = this.velocity.X * 0.9f;
                                                                                                if ((double)this.velocity.X > -0.1 || (double)this.velocity.X < 0.1)
                                                                                                {
                                                                                                    this.netUpdate = true;
                                                                                                    this.velocity.X = 0f;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        if (this.ai[0] > 0f)
                                                                                        {
                                                                                            this.ai[0] -= 1f;
                                                                                        }
                                                                                        if (Main.netMode != 1 && flag15 && this.ai[0] == 0f && Collision.CanHit(this.position, this.width, this.height, Main.players[this.target].position, Main.players[this.target].width, Main.players[this.target].height))
                                                                                        {
                                                                                            this.ai[0] = 200f;
                                                                                            int num162 = 10;
                                                                                            int num163 = 31;
                                                                                            int num164 = Projectile.NewProjectile(vector21.X, vector21.Y, num159, num160, num163, num162, 0f, Main.myPlayer);
                                                                                            Main.projectile[num164].ai[0] = 2f;
                                                                                            Main.projectile[num164].timeLeft = 300;
                                                                                            Main.projectile[num164].friendly = false;
                                                                                            NetMessage.SendData(27, -1, -1, "", num164);
                                                                                            this.netUpdate = true;
                                                                                        }
                                                                                        try
                                                                                        {
                                                                                            int num165 = (int)this.position.X / 16;
                                                                                            int num166 = (int)(this.position.X + (float)(this.width / 2)) / 16;
                                                                                            int num167 = (int)(this.position.X + (float)this.width) / 16;
                                                                                            int num168 = (int)(this.position.Y + (float)this.height) / 16;
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
                                                                                            if ((Main.tile[num165, num168].active && Main.tileSolid[(int)Main.tile[num165, num168].type]) || (Main.tile[num166, num168].active && Main.tileSolid[(int)Main.tile[num166, num168].type]) || (Main.tile[num167, num168].active && Main.tileSolid[(int)Main.tile[num167, num168].type]))
                                                                                            {
                                                                                                flag16 = true;
                                                                                            }
                                                                                            if (flag16)
                                                                                            {
                                                                                                this.noGravity = true;
                                                                                                this.noTileCollide = true;
                                                                                                this.velocity.Y = -0.2f;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                this.noGravity = false;
                                                                                                this.noTileCollide = false;
                                                                                                if (Main.rand.Next(2) == 0)
                                                                                                {
                                                                                                    Vector2 arg_BA65_0 = new Vector2(this.position.X - 4f, this.position.Y + (float)this.height - 8f);
                                                                                                    int arg_BA65_1 = this.width + 8;
                                                                                                    int arg_BA65_2 = 24;
                                                                                                    int arg_BA65_3 = 32;
                                                                                                    float arg_BA65_4 = 0f;
                                                                                                    float arg_BA65_5 = this.velocity.Y / 2f;
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
            if (this.type == 1 || this.type == 16 || this.type == 59)
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
            if (this.type == 61)
            {
                this.spriteDirection = this.direction;
                this.rotation = this.velocity.X * 0.1f;
                if (this.velocity.X == 0f && this.velocity.Y == 0f)
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
            if (this.type == 62 || this.type == 66)
            {
                this.spriteDirection = this.direction;
                this.rotation = this.velocity.X * 0.1f;
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
            if (this.type == 63 || this.type == 64)
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
            if (this.type == 2 || this.type == 23)
            {
                if (this.type == 2)
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
            if (this.type == 69)
            {
                if (this.ai[0] < 190f)
                {
                    this.frameCounter += 1.0;
                    if (this.frameCounter >= 6.0)
                    {
                        this.frameCounter = 0.0;
                        this.frame.Y = this.frame.Y + num;
                        if (this.frame.Y / num >= Main.npcFrameCount[this.type] - 1)
                        {
                            this.frame.Y = 0;
                        }
                    }
                }
                else
                {
                    this.frameCounter = 0.0;
                    this.frame.Y = num * (Main.npcFrameCount[this.type] - 1);
                }
            }
            if (this.type == 67)
            {
                if (this.velocity.Y == 0f)
                {
                    this.spriteDirection = this.direction;
                }
                this.frameCounter += 1.0;
                if (this.frameCounter >= 6.0)
                {
                    this.frameCounter = 0.0;
                    this.frame.Y = this.frame.Y + num;
                    if (this.frame.Y / num >= Main.npcFrameCount[this.type])
                    {
                        this.frame.Y = 0;
                    }
                }
            }
            if (this.type == 65)
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
            if (this.type == 48 || this.type == 49 || this.type == 51 || this.type == 60)
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
                if (Main.players[i].active && !Main.players[i].dead && (num == -1f || Math.Abs(Main.players[i].position.X + (float)(Main.players[i].width / 2) - this.position.X + (float)(this.width / 2)) + Math.Abs(Main.players[i].position.Y + (float)(Main.players[i].height / 2) - this.position.Y + (float)(this.height / 2)) < num))
                {
                    num = Math.Abs(Main.players[i].position.X + (float)(Main.players[i].width / 2) - this.position.X + (float)(this.width / 2)) + Math.Abs(Main.players[i].position.Y + (float)(Main.players[i].height / 2) - this.position.Y + (float)(this.height / 2));
                    this.target = i;
                }
            }
            if (this.target < 0 || this.target >= 255)
            {
                this.target = 0;
            }
            this.targetRect = new Rectangle((int)Main.players[this.target].position.X, (int)Main.players[this.target].position.Y, Main.players[this.target].width, Main.players[this.target].height);
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
                            if (Main.players[i].active && rectangle.Intersects(new Rectangle((int)Main.players[i].position.X, (int)Main.players[i].position.Y, Main.players[i].width, Main.players[i].height)))
                            {
                                Main.players[i].townNPCs += (int)NPC.npcSlots;
                            }
                        }
                    }
                    return;
                }
                bool flag = false;
                Rectangle rectangle2 = new Rectangle((int)(this.position.X + (float)(this.width / 2) - (float)NPC.activeRangeX), (int)(this.position.Y + (float)(this.height / 2) - (float)NPC.activeRangeY), NPC.activeRangeX * 2, NPC.activeRangeY * 2);
                Rectangle rectangle3 = new Rectangle((int)((double)(this.position.X + (float)(this.width / 2)) - (double)NPC.sWidth * 0.5 - (double)this.width), (int)((double)(this.position.Y + (float)(this.height / 2)) - (double)NPC.sHeight * 0.5 - (double)this.height), NPC.sWidth + this.width * 2, NPC.sHeight + this.height * 2);
                for (int j = 0; j < 255; j++)
                {
                    if (Main.players[j].active)
                    {
                        if (rectangle2.Intersects(new Rectangle((int)Main.players[j].position.X, (int)Main.players[j].position.Y, Main.players[j].width, Main.players[j].height)))
                        {
                            flag = true;
                            if (this.type != 25 && this.type != 30 && this.type != 33)
                            {
                                Main.players[j].activeNPCs += (int)NPC.npcSlots;
                            }
                        }
                        if (rectangle3.Intersects(new Rectangle((int)Main.players[j].position.X, (int)Main.players[j].position.Y, Main.players[j].width, Main.players[j].height)))
                        {
                            this.timeLeft = NPC.activeTime;
                        }
                        if (this.type == 7 || this.type == 10 || this.type == 13 || this.type == 39)
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
                        NetMessage.SendData(23, -1, -1, "", this.whoAmI);
                    }
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
                if (Main.players[i].active)
                {
                    num3++;
                }
            }
            for (int j = 0; j < 255; j++)
            {
                if (Main.players[j].active && !Main.players[j].dead)
                {
                    bool flag3 = false;
                    bool flag4 = false;
                    if (Main.players[j].active && Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0 && (double)Main.players[j].position.Y < Main.worldSurface * 16.0 + (double)NPC.sHeight)
                    {
                        int num4 = 3000;
                        if ((double)Main.players[j].position.X > Main.invasionX * 16.0 - (double)num4 && (double)Main.players[j].position.X < Main.invasionX * 16.0 + (double)num4)
                        {
                            flag3 = true;
                        }
                    }
                    flag = false;
                    NPC.spawnRate = NPC.defaultSpawnRate;
                    NPC.maxSpawns = NPC.defaultMaxSpawns;
                    if (Main.players[j].position.Y > (float)((Main.maxTilesY - 200) * 16))
                    {
                        NPC.spawnRate = (int)((float)NPC.spawnRate * 0.4f);
                        NPC.maxSpawns = (int)((float)NPC.maxSpawns * 2.1f);
                    }
                    else
                    {
                        if ((double)Main.players[j].position.Y > Main.rockLayer * 16.0 + (double)NPC.sHeight)
                        {
                            NPC.spawnRate = (int)((double)NPC.spawnRate * 0.4);
                            NPC.maxSpawns = (int)((float)NPC.maxSpawns * 1.9f);
                        }
                        else
                        {
                            if ((double)Main.players[j].position.Y > Main.worldSurface * 16.0 + (double)NPC.sHeight)
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
                    if ((double)(Main.players[j].position.Y * 16f) > (Main.worldSurface + Main.rockLayer) / 2.0 || Main.players[j].zoneEvil)
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
                    if (Main.players[j].active && !Main.players[j].dead && Main.players[j].activeNPCs < (float)NPC.maxSpawns && Main.rand.Next(NPC.spawnRate) == 0)
                    {
                        int num5 = (int)(Main.players[j].position.X / 16f) - NPC.spawnRangeX;
                        int num6 = (int)(Main.players[j].position.X / 16f) + NPC.spawnRangeX;
                        int num7 = (int)(Main.players[j].position.Y / 16f) - NPC.spawnRangeY;
                        int num8 = (int)(Main.players[j].position.Y / 16f) + NPC.spawnRangeY;
                        int num9 = (int)(Main.players[j].position.X / 16f) - NPC.safeRangeX;
                        int num10 = (int)(Main.players[j].position.X / 16f) + NPC.safeRangeX;
                        int num11 = (int)(Main.players[j].position.Y / 16f) - NPC.safeRangeY;
                        int num12 = (int)(Main.players[j].position.Y / 16f) + NPC.safeRangeY;
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
                                        if (Main.tile[num13, l].active && Main.tileSolid[(int)Main.tile[num13, l].type])
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
                                            if (Main.tile[m, n].active && Main.tileSolid[(int)Main.tile[m, n].type])
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
                            if (Main.players[num19].active)
                            {
                                Rectangle rectangle2 = new Rectangle((int)(Main.players[num19].position.X + (float)(Main.players[num19].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.players[num19].position.Y + (float)(Main.players[num19].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
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
                        int num21 = 1000;
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
                                                            num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 68, 0);
                                                        }
                                                        else
                                                        {
                                                            if (Main.rand.Next(3) == 0)
                                                            {
                                                                num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 34, 0);
                                                            }
                                                            else
                                                            {
                                                                if (Main.rand.Next(6) == 0)
                                                                {
                                                                    num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 32, 0);
                                                                }
                                                                else
                                                                {
                                                                    num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 31, 0);
                                                                    if (Main.rand.Next(4) == 0)
                                                                    {
                                                                        Main.npc[num21].SetDefaults("Big Boned");
                                                                    }
                                                                    else
                                                                    {
                                                                        if (Main.rand.Next(5) == 0)
                                                                        {
                                                                            Main.npc[num21].SetDefaults("Short Bones");
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
                                                            num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 23, 0);
                                                        }
                                                        else
                                                        {
                                                            if (Main.players[j].zoneEvil && Main.rand.Next(50) == 0)
                                                            {
                                                                num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 7, 1);
                                                            }
                                                            else
                                                            {
                                                                if (num20 == 60 && Main.rand.Next(500) == 0 && !Main.dayTime)
                                                                {
                                                                    num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 52, 0);
                                                                }
                                                                else
                                                                {
                                                                    if (num20 == 60 && (double)num2 > (Main.worldSurface + Main.rockLayer) / 2.0)
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
                                                                            if (Main.rand.Next(4) == 0)
                                                                            {
                                                                                Main.npc[num21].SetDefaults("Little Stinger");
                                                                            }
                                                                            else
                                                                            {
                                                                                if (Main.rand.Next(4) == 0)
                                                                                {
                                                                                    Main.npc[num21].SetDefaults("Big Stinger");
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num20 == 60 && Main.rand.Next(4) == 0)
                                                                        {
                                                                            num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 51, 0);
                                                                        }
                                                                        else
                                                                        {
                                                                            if (num20 == 60 && Main.rand.Next(8) == 0)
                                                                            {
                                                                                num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 56, 0);
                                                                                Main.npc[num21].ai[0] = (float)num;
                                                                                Main.npc[num21].ai[1] = (float)num2;
                                                                                Main.npc[num21].netUpdate = true;
                                                                            }
                                                                            else
                                                                            {
                                                                                if ((num20 == 22 && Main.players[j].zoneEvil) || num20 == 23 || num20 == 25)
                                                                                {
                                                                                    num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 6, 0);
                                                                                    if (Main.rand.Next(3) == 0)
                                                                                    {
                                                                                        Main.npc[num21].SetDefaults("Little Eater");
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (Main.rand.Next(3) == 0)
                                                                                        {
                                                                                            Main.npc[num21].SetDefaults("Big Eater");
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
                                                                                                    num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 50, 0);
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (num20 == 53 && Main.rand.Next(5) == 0 && !flag4)
                                                                                                    {
                                                                                                        num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 69, 0);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (num20 == 53 && !flag4)
                                                                                                        {
                                                                                                            num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 61, 0);
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 1, 0);
                                                                                                            if (num20 == 60)
                                                                                                            {
                                                                                                                Main.npc[num21].SetDefaults("Jungle Slime");
                                                                                                            }
                                                                                                            else
                                                                                                            {
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
                                                                                                        }
                                                                                                    }
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
                                                                                                if (Main.rand.Next(40) == 0 && !NPC.AnyNPCs(39))
                                                                                                {
                                                                                                    num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 39, 1);
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (Main.rand.Next(20) == 0)
                                                                                                    {
                                                                                                        num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 24, 0);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (Main.rand.Next(12) == 0)
                                                                                                        {
                                                                                                            if (Main.rand.Next(10) == 0)
                                                                                                            {
                                                                                                                num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 66, 0);
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 62, 0);
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (Main.rand.Next(3) == 0)
                                                                                                            {
                                                                                                                num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 59, 0);
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 60, 0);
                                                                                                            }
                                                                                                        }
                                                                                                    }
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
                                                                                                    if (Main.rand.Next(10) == 0)
                                                                                                    {
                                                                                                        num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 16, 0);
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (Main.rand.Next(4) == 0)
                                                                                                        {
                                                                                                            num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 1, 0);
                                                                                                            if (Main.players[j].zoneJungle)
                                                                                                            {
                                                                                                                Main.npc[num21].SetDefaults("Jungle Slime");
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Main.npc[num21].SetDefaults("Black Slime");
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (Main.rand.Next(2) == 0)
                                                                                                            {
                                                                                                                if ((double)num2 > (Main.rockLayer + (double)Main.maxTilesY) / 2.0 && Main.rand.Next(700) == 0)
                                                                                                                {
                                                                                                                    num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 45, 0);
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (Main.rand.Next(15) == 0)
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
                                                                                                                if (Main.players[j].zoneJungle)
                                                                                                                {
                                                                                                                    num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 51, 0);
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    num21 = NPC.NewNPC(num * 16 + 8, num2 * 16, 49, 0);
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
                        if (Main.npc[num21].type == 1 && Main.rand.Next(250) == 0)
                        {
                            Main.npc[num21].SetDefaults("Pinky");
                        }
                        if (Main.netMode == 2 && num21 < 1000)
                        {
                            NetMessage.SendData(23, -1, -1, "", num21);
                            return;
                        }
                        break;
                    }
                }
            }
        }
        
        public static void SpawnOnPlayer(Player player, int playerIndex, int Type)
        {
            if (Main.netMode == 1)
            {
                return;
            }
            bool flag = false;
            int num = 0;
            int num2 = 0;
            int num3 = (int)(player.position.X / 16f) - NPC.spawnRangeX * 3;
            int num4 = (int)(player.position.X / 16f) + NPC.spawnRangeX * 3;
            int num5 = (int)(player.position.Y / 16f) - NPC.spawnRangeY * 3;
            int num6 = (int)(player.position.Y / 16f) + NPC.spawnRangeY * 3;
            int num7 = (int)(player.position.X / 16f) - NPC.safeRangeX;
            int num8 = (int)(player.position.X / 16f) + NPC.safeRangeX;
            int num9 = (int)(player.position.Y / 16f) - NPC.safeRangeY;
            int num10 = (int)(player.position.Y / 16f) + NPC.safeRangeY;
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
                        goto IL_2E1;
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
                                    if (Main.tile[l, m].active && Main.tileSolid[(int)Main.tile[l, m].type])
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
                        if (Main.players[n].active)
                        {
                            Rectangle rectangle2 = new Rectangle((int)(Main.players[n].position.X + (float)(Main.players[n].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.players[n].position.Y + (float)(Main.players[n].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
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
                Main.npc[num17].target = playerIndex;
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
                    NetMessage.SendData(23, -1, -1, "", num17);
                }
                if (Main.netMode == 0)
                {
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
                if (Type == 50)
                {
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(25, -1, -1, Main.npc[num].name + " has awoken!", 255, 175f, 75f, 255f);
                    }
                }
                return num;
            }
            return 1000;
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
                    NetMessage.SendData(23, -1, -1, "", this.whoAmI);
                }
            }
        }
        
        public double StrikeNPC(int Damage, float knockBack, int hitDirection)
        {
            if (!this.active || this.life <= 0)
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

                if (this.life <= 0)
                {
                    NPC.noSpawnCycle = true;
                    if (this.townNPC && this.type != 37)
                    {
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(25, -1, -1, this.name + " was slain...", 255, 255f, 25f, 25f);
                        }
                    }
                    if (this.townNPC && Main.netMode != 1 && this.homeless && WorldGen.spawnNPC == this.type)
                    {
                        WorldGen.spawnNPC = 0;
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
            if (this.type == 3 && Main.rand.Next(50) == 0)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 216, 1, false);
            }
            if (this.type == 66)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 267, 1, false);
            }
            if (this.type == 62 && Main.rand.Next(50) == 0)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 272, 1, false);
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
            if (this.type == 69 && Main.rand.Next(10) == 0)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 323, 1, false);
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
            if (this.type == 47 && Main.rand.Next(75) == 0)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 243, 1, false);
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
                    stack2 = Main.rand.Next(10, 30);
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, stack2, false);
                    stack2 = Main.rand.Next(10, 31);
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 56, stack2, false);
                }
            }
            if (this.type == 63 || this.type == 64)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 282, Main.rand.Next(1, 5), false);
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
            if (this.type == 23 && Main.rand.Next(50) == 0)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 116, 1, false);
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
            if (this.type == 65)
            {
                if (Main.rand.Next(50) == 0)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 268, 1, false);
                }
                else
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 319, 1, false);
                }
            }
            if (this.type == 48 && Main.rand.Next(5) == 0)
            {
                Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 320, 1, false);
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
                int num = Main.rand.Next(5) + 5;
                for (int i = 0; i < num; i++)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 58, 1, false);
                }

                if (Main.netMode == 2)
                {
                    NetMessage.SendData(25, -1, -1, this.name + " has been defeated!", 255, 175f, 75f, 255f);
                }
            }
            if (Main.rand.Next(7) == 0 && this.lifeMax > 1)
            {
                if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(this.position, this.width, this.height)].statMana < Main.players[(int)Player.FindClosest(this.position, this.width, this.height)].statManaMax)
                {
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 184, 1, false);
                }
                else
                {
                    if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(this.position, this.width, this.height)].statLife < Main.players[(int)Player.FindClosest(this.position, this.width, this.height)].statLifeMax)
                    {
                        Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 58, 1, false);
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
                    Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 74, num3, false);
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
                        Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 73, num4, false);
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
                            Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 72, num5, false);
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
                            Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 71, num6, false);
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
                }
                else
                {
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
                            NPC expr_173_cp_0 = Main.npc[num3];
                            expr_173_cp_0.velocity.X = expr_173_cp_0.velocity.X + ((float)Main.rand.Next(-20, 20) * 0.1f + (float)(j * this.direction) * 0.3f);
                            NPC expr_1B1_cp_0 = Main.npc[num3];
                            expr_1B1_cp_0.velocity.Y = expr_1B1_cp_0.velocity.Y - ((float)Main.rand.Next(0, 10) * 0.1f + (float)j);
                            Main.npc[num3].ai[1] = (float)j;
                            if (Main.netMode == 2 && num3 < 1000)
                            {
                                NetMessage.SendData(23, -1, -1, "", num3);
                            }
                        }
                    }
                }
            }
            if (this.type != 63 && this.type != 64)
            {
                if (this.type == 59 || this.type == 60)
                {
                    if (this.life > 0)
                    {
                        int num4 = 0;
                        while ((double)num4 < dmg / (double)this.lifeMax * 80.0)
                        {
                            Vector2 arg_350_0 = this.position;
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
                        Vector2 arg_3AB_0 = this.position;
                        int arg_3AB_1 = this.width;
                        int arg_3AB_2 = this.height;
                        int arg_3AB_3 = 6;
                        float arg_3AB_4 = (float)(hitDirection * 2);
                        float arg_3AB_5 = -1f;
                        int arg_3AB_6 = this.alpha;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_3AB_0, arg_3AB_1, arg_3AB_2, arg_3AB_3, arg_3AB_4, arg_3AB_5, arg_3AB_6, newColor, 1.5f);
                    }
                    if (this.type == 59 && Main.netMode != 1)
                    {
                        int num5 = (int)(this.position.X + (float)(this.width / 2)) / 16;
                        int num6 = (int)(this.position.Y + (float)(this.height / 2)) / 16;
                        Main.tile[num5, num6].lava = true;
                        if (Main.tile[num5, num6].liquid < 200)
                        {
                            Main.tile[num5, num6].liquid = 200;
                        }
                        WorldGen.TileFrame(num5, num6, false, false);
                        return;
                    }
                }
                else
                {
                    if (this.type == 50)
                    {
                        if (this.life > 0)
                        {
                            int num7 = 0;
                            while ((double)num7 < dmg / (double)this.lifeMax * 300.0)
                            {
                                Dust.NewDust(this.position, this.width, this.height, 4, (float)hitDirection, -1f, 175, new Color(0, 80, 255, 100), 1f);
                                num7++;
                            }
                            return;
                        }
                        for (int l = 0; l < 200; l++)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 4, (float)(2 * hitDirection), -2f, 175, new Color(0, 80, 255, 100), 1f);
                        }
                        if (Main.netMode != 1)
                        {
                            int num8 = Main.rand.Next(4) + 4;
                            for (int m = 0; m < num8; m++)
                            {
                                int x = (int)(this.position.X + (float)Main.rand.Next(this.width - 32));
                                int y = (int)(this.position.Y + (float)Main.rand.Next(this.height - 32));
                                int num9 = NPC.NewNPC(x, y, 1, 0);
                                Main.npc[num9].SetDefaults(1);
                                Main.npc[num9].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                                Main.npc[num9].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                                Main.npc[num9].ai[1] = (float)Main.rand.Next(3);
                                if (Main.netMode == 2 && num9 < 1000)
                                {
                                    NetMessage.SendData(23, -1, -1, "", num9);
                                }
                            }
                            return;
                        }
                    }
                    else
                    {
                        if (this.type == 49 || this.type == 51)
                        {
                            if (this.life > 0)
                            {
                                int num10 = 0;
                                while ((double)num10 < dmg / (double)this.lifeMax * 30.0)
                                {
                                    Vector2 arg_69A_0 = this.position;
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
                                Vector2 arg_6F0_0 = this.position;
                                int arg_6F0_1 = this.width;
                                int arg_6F0_2 = this.height;
                                int arg_6F0_3 = 5;
                                float arg_6F0_4 = (float)(2 * hitDirection);
                                float arg_6F0_5 = -2f;
                                int arg_6F0_6 = 0;
                                Color newColor = default(Color);
                                Dust.NewDust(arg_6F0_0, arg_6F0_1, arg_6F0_2, arg_6F0_3, arg_6F0_4, arg_6F0_5, arg_6F0_6, newColor, 1f);
                            }
                            if (this.type == 51)
                            {
                                SetGore(83);
                                return;
                            }
                            SetGore(82);
                            return;
                        }
                        else
                        {
                            if (this.type == 46 || this.type == 55 || this.type == 67)
                            {
                                if (this.life > 0)
                                {
                                    int num11 = 0;
                                    while ((double)num11 < dmg / (double)this.lifeMax * 20.0)
                                    {
                                        Vector2 arg_78F_0 = this.position;
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
                                    Vector2 arg_7E5_0 = this.position;
                                    int arg_7E5_1 = this.width;
                                    int arg_7E5_2 = this.height;
                                    int arg_7E5_3 = 5;
                                    float arg_7E5_4 = (float)(2 * hitDirection);
                                    float arg_7E5_5 = -2f;
                                    int arg_7E5_6 = 0;
                                    Color newColor = default(Color);
                                    Dust.NewDust(arg_7E5_0, arg_7E5_1, arg_7E5_2, arg_7E5_3, arg_7E5_4, arg_7E5_5, arg_7E5_6, newColor, 1f);
                                }
                                if (this.type == 46)
                                {
                                    SetGore(76);
                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y), this.velocity, 77);
                                    return;
                                }
                                if (this.type == 67)
                                {
                                    SetGore(95);
                                    SetGore(95);
                                    SetGore(96);
                                    return;
                                }
                            }
                            else
                            {
                                if (this.type == 47 || this.type == 57 || this.type == 58)
                                {
                                    if (this.life > 0)
                                    {
                                        int num13 = 0;
                                        while ((double)num13 < dmg / (double)this.lifeMax * 20.0)
                                        {
                                            Vector2 arg_8E2_0 = this.position;
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
                                        Vector2 arg_938_0 = this.position;
                                        int arg_938_1 = this.width;
                                        int arg_938_2 = this.height;
                                        int arg_938_3 = 5;
                                        float arg_938_4 = (float)(2 * hitDirection);
                                        float arg_938_5 = -2f;
                                        int arg_938_6 = 0;
                                        Color newColor = default(Color);
                                        Dust.NewDust(arg_938_0, arg_938_1, arg_938_2, arg_938_3, arg_938_4, arg_938_5, arg_938_6, newColor, 1f);
                                    }
                                    if (this.type == 57)
                                    {
                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y), this.velocity, 84);
                                        return;
                                    }
                                    if (this.type == 58)
                                    {
                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y), this.velocity, 85);
                                        return;
                                    }
                                    SetGore(78);
                                    Gore.NewGore(new Vector2(this.position.X, this.position.Y), this.velocity, 79);
                                    return;
                                }
                                else
                                {
                                    if (this.type == 2)
                                    {
                                        if (this.life > 0)
                                        {
                                            int num15 = 0;
                                            while ((double)num15 < dmg / (double)this.lifeMax * 100.0)
                                            {
                                                Vector2 arg_A34_0 = this.position;
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
                                            Vector2 arg_A8A_0 = this.position;
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
                                        Gore.NewGore(new Vector2(this.position.X + 14f, this.position.Y), this.velocity, 2);
                                        return;
                                    }
                                    else
                                    {
                                        if (this.type == 69)
                                        {
                                            if (this.life > 0)
                                            {
                                                int num17 = 0;
                                                while ((double)num17 < dmg / (double)this.lifeMax * 100.0)
                                                {
                                                    Vector2 arg_B23_0 = this.position;
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
                                                Vector2 arg_B79_0 = this.position;
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
                                        else
                                        {
                                            if (this.type == 61)
                                            {
                                                if (this.life > 0)
                                                {
                                                    int num19 = 0;
                                                    while ((double)num19 < dmg / (double)this.lifeMax * 100.0)
                                                    {
                                                        Vector2 arg_BF9_0 = this.position;
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
                                                    Vector2 arg_C4F_0 = this.position;
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
                                                Gore.NewGore(new Vector2(this.position.X + 14f, this.position.Y), this.velocity, 87);
                                                Gore.NewGore(new Vector2(this.position.X + 14f, this.position.Y), this.velocity, 88);
                                                return;
                                            }
                                            else
                                            {
                                                if (this.type == 65)
                                                {
                                                    if (this.life > 0)
                                                    {
                                                        int num21 = 0;
                                                        while ((double)num21 < dmg / (double)this.lifeMax * 150.0)
                                                        {
                                                            Vector2 arg_D19_0 = this.position;
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
                                                        Vector2 arg_D6F_0 = this.position;
                                                        int arg_D6F_1 = this.width;
                                                        int arg_D6F_2 = this.height;
                                                        int arg_D6F_3 = 5;
                                                        float arg_D6F_4 = (float)(2 * hitDirection);
                                                        float arg_D6F_5 = -2f;
                                                        int arg_D6F_6 = 0;
                                                        Color newColor = default(Color);
                                                        Dust.NewDust(arg_D6F_0, arg_D6F_1, arg_D6F_2, arg_D6F_3, arg_D6F_4, arg_D6F_5, arg_D6F_6, newColor, 1f);
                                                    }
                                                    Gore.NewGore(this.position, this.velocity * 0.8f, 89);
                                                    Gore.NewGore(new Vector2(this.position.X + 14f, this.position.Y), this.velocity * 0.8f, 90);
                                                    Gore.NewGore(new Vector2(this.position.X + 14f, this.position.Y), this.velocity * 0.8f, 91);
                                                    Gore.NewGore(new Vector2(this.position.X + 14f, this.position.Y), this.velocity * 0.8f, 92);
                                                    return;
                                                }
                                                else
                                                {
                                                    if (this.type == 3 || this.type == 52 || this.type == 53)
                                                    {
                                                        if (this.life > 0)
                                                        {
                                                            int num23 = 0;
                                                            while ((double)num23 < dmg / (double)this.lifeMax * 100.0)
                                                            {
                                                                Vector2 arg_EA3_0 = this.position;
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
                                                            Vector2 arg_EFD_0 = this.position;
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
                                                                int num25 = 0;
                                                                while ((double)num25 < dmg / (double)this.lifeMax * 100.0)
                                                                {
                                                                    Vector2 arg_101F_0 = this.position;
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
                                                                Vector2 arg_1075_0 = this.position;
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
                                                                Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 2);
                                                                Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 7);
                                                                Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 9);
                                                                Gore.NewGore(this.position, new Vector2((float)Main.rand.Next(-30, 31) * 0.2f, (float)Main.rand.Next(-30, 31) * 0.2f), 10);
                                                            }
                                                            return;
                                                        }
                                                        else
                                                        {
                                                            if (this.type == 5)
                                                            {
                                                                if (this.life > 0)
                                                                {
                                                                    int num28 = 0;
                                                                    while ((double)num28 < dmg / (double)this.lifeMax * 50.0)
                                                                    {
                                                                        Vector2 arg_11F7_0 = this.position;
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
                                                                    Vector2 arg_124D_0 = this.position;
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
                                                            else
                                                            {
                                                                if (this.type == 6)
                                                                {
                                                                    if (this.life > 0)
                                                                    {
                                                                        int num30 = 0;
                                                                        while ((double)num30 < dmg / (double)this.lifeMax * 100.0)
                                                                        {
                                                                            Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
                                                                            num30++;
                                                                        }
                                                                        return;
                                                                    }
                                                                    for (int num31 = 0; num31 < 50; num31++)
                                                                    {
                                                                        Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
                                                                    }
                                                                    int num32 = SetGore(14);
                                                                    Main.gore[num32].alpha = this.alpha;
                                                                    num32 = SetGore(15);
                                                                    Main.gore[num32].alpha = this.alpha;
                                                                    return;
                                                                }
                                                                else
                                                                {
                                                                    if (this.type == 7 || this.type == 8 || this.type == 9)
                                                                    {
                                                                        if (this.life > 0)
                                                                        {
                                                                            int num33 = 0;
                                                                            while ((double)num33 < dmg / (double)this.lifeMax * 100.0)
                                                                            {
                                                                                Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
                                                                                num33++;
                                                                            }
                                                                            return;
                                                                        }
                                                                        for (int num34 = 0; num34 < 50; num34++)
                                                                        {
                                                                            Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
                                                                        }
                                                                        int num35 = SetGore(this.type - 7 + 18);
                                                                        Main.gore[num35].alpha = this.alpha;
                                                                        return;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (this.type == 10 || this.type == 11 || this.type == 12)
                                                                        {
                                                                            if (this.life > 0)
                                                                            {
                                                                                int num36 = 0;
                                                                                while ((double)num36 < dmg / (double)this.lifeMax * 50.0)
                                                                                {
                                                                                    Vector2 arg_14D5_0 = this.position;
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
                                                                                Vector2 arg_152F_0 = this.position;
                                                                                int arg_152F_1 = this.width;
                                                                                int arg_152F_2 = this.height;
                                                                                int arg_152F_3 = 5;
                                                                                float arg_152F_4 = 2.5f * (float)hitDirection;
                                                                                float arg_152F_5 = -2.5f;
                                                                                int arg_152F_6 = 0;
                                                                                Color newColor = default(Color);
                                                                                Dust.NewDust(arg_152F_0, arg_152F_1, arg_152F_2, arg_152F_3, arg_152F_4, arg_152F_5, arg_152F_6, newColor, 1f);
                                                                            }
                                                                            SetGore(this.type - 7 + 18);
                                                                            return;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (this.type == 13 || this.type == 14 || this.type == 15)
                                                                            {
                                                                                if (this.life > 0)
                                                                                {
                                                                                    int num38 = 0;
                                                                                    while ((double)num38 < dmg / (double)this.lifeMax * 100.0)
                                                                                    {
                                                                                        Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
                                                                                        num38++;
                                                                                    }
                                                                                    return;
                                                                                }
                                                                                for (int num39 = 0; num39 < 50; num39++)
                                                                                {
                                                                                    Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
                                                                                }
                                                                                if (this.type == 13)
                                                                                {
                                                                                    SetGore(24);
                                                                                    SetGore(25);
                                                                                    return;
                                                                                }
                                                                                if (this.type == 14)
                                                                                {
                                                                                    SetGore(26);
                                                                                    SetGore(27);
                                                                                    return;
                                                                                }
                                                                                SetGore(28);
                                                                                SetGore(29);
                                                                                return;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (this.type == 17)
                                                                                {
                                                                                    if (this.life > 0)
                                                                                    {
                                                                                        int num40 = 0;
                                                                                        while ((double)num40 < dmg / (double)this.lifeMax * 100.0)
                                                                                        {
                                                                                            Vector2 arg_16F8_0 = this.position;
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
                                                                                        Vector2 arg_1752_0 = this.position;
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
                                                                                            int num42 = 0;
                                                                                            while ((double)num42 < dmg / (double)this.lifeMax * 100.0)
                                                                                            {
                                                                                                Vector2 arg_187A_0 = this.position;
                                                                                                int arg_187A_1 = this.width;
                                                                                                int arg_187A_2 = this.height;
                                                                                                int arg_187A_3 = 5;
                                                                                                float arg_187A_4 = (float)hitDirection;
                                                                                                float arg_187A_5 = -1f;
                                                                                                int arg_187A_6 = 0;
                                                                                                Color newColor = default(Color);
                                                                                                Dust.NewDust(arg_187A_0, arg_187A_1, arg_187A_2, arg_187A_3, arg_187A_4, arg_187A_5, arg_187A_6, newColor, 1f);
                                                                                                num42++;
                                                                                            }
                                                                                            return;
                                                                                        }
                                                                                        for (int num43 = 0; num43 < 50; num43++)
                                                                                        {
                                                                                            Vector2 arg_18D4_0 = this.position;
                                                                                            int arg_18D4_1 = this.width;
                                                                                            int arg_18D4_2 = this.height;
                                                                                            int arg_18D4_3 = 5;
                                                                                            float arg_18D4_4 = 2.5f * (float)hitDirection;
                                                                                            float arg_18D4_5 = -2.5f;
                                                                                            int arg_18D4_6 = 0;
                                                                                            Color newColor = default(Color);
                                                                                            Dust.NewDust(arg_18D4_0, arg_18D4_1, arg_18D4_2, arg_18D4_3, arg_18D4_4, arg_18D4_5, arg_18D4_6, newColor, 1f);
                                                                                        }
                                                                                        SetGore(73);
                                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 74);
                                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 74);
                                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 75);
                                                                                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 75);
                                                                                        return;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (this.type == 37 || this.type == 54)
                                                                                        {
                                                                                            if (this.life > 0)
                                                                                            {
                                                                                                int num44 = 0;
                                                                                                while ((double)num44 < dmg / (double)this.lifeMax * 100.0)
                                                                                                {
                                                                                                    Vector2 arg_1A06_0 = this.position;
                                                                                                    int arg_1A06_1 = this.width;
                                                                                                    int arg_1A06_2 = this.height;
                                                                                                    int arg_1A06_3 = 5;
                                                                                                    float arg_1A06_4 = (float)hitDirection;
                                                                                                    float arg_1A06_5 = -1f;
                                                                                                    int arg_1A06_6 = 0;
                                                                                                    Color newColor = default(Color);
                                                                                                    Dust.NewDust(arg_1A06_0, arg_1A06_1, arg_1A06_2, arg_1A06_3, arg_1A06_4, arg_1A06_5, arg_1A06_6, newColor, 1f);
                                                                                                    num44++;
                                                                                                }
                                                                                                return;
                                                                                            }
                                                                                            for (int num45 = 0; num45 < 50; num45++)
                                                                                            {
                                                                                                Vector2 arg_1A60_0 = this.position;
                                                                                                int arg_1A60_1 = this.width;
                                                                                                int arg_1A60_2 = this.height;
                                                                                                int arg_1A60_3 = 5;
                                                                                                float arg_1A60_4 = 2.5f * (float)hitDirection;
                                                                                                float arg_1A60_5 = -2.5f;
                                                                                                int arg_1A60_6 = 0;
                                                                                                Color newColor = default(Color);
                                                                                                Dust.NewDust(arg_1A60_0, arg_1A60_1, arg_1A60_2, arg_1A60_3, arg_1A60_4, arg_1A60_5, arg_1A60_6, newColor, 1f);
                                                                                            }
                                                                                            SetGore(58);
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
                                                                                                    int num46 = 0;
                                                                                                    while ((double)num46 < dmg / (double)this.lifeMax * 100.0)
                                                                                                    {
                                                                                                        Vector2 arg_1B88_0 = this.position;
                                                                                                        int arg_1B88_1 = this.width;
                                                                                                        int arg_1B88_2 = this.height;
                                                                                                        int arg_1B88_3 = 5;
                                                                                                        float arg_1B88_4 = (float)hitDirection;
                                                                                                        float arg_1B88_5 = -1f;
                                                                                                        int arg_1B88_6 = 0;
                                                                                                        Color newColor = default(Color);
                                                                                                        Dust.NewDust(arg_1B88_0, arg_1B88_1, arg_1B88_2, arg_1B88_3, arg_1B88_4, arg_1B88_5, arg_1B88_6, newColor, 1f);
                                                                                                        num46++;
                                                                                                    }
                                                                                                    return;
                                                                                                }
                                                                                                for (int num47 = 0; num47 < 50; num47++)
                                                                                                {
                                                                                                    Vector2 arg_1BE2_0 = this.position;
                                                                                                    int arg_1BE2_1 = this.width;
                                                                                                    int arg_1BE2_2 = this.height;
                                                                                                    int arg_1BE2_3 = 5;
                                                                                                    float arg_1BE2_4 = 2.5f * (float)hitDirection;
                                                                                                    float arg_1BE2_5 = -2.5f;
                                                                                                    int arg_1BE2_6 = 0;
                                                                                                    Color newColor = default(Color);
                                                                                                    Dust.NewDust(arg_1BE2_0, arg_1BE2_1, arg_1BE2_2, arg_1BE2_3, arg_1BE2_4, arg_1BE2_5, arg_1BE2_6, newColor, 1f);
                                                                                                }
                                                                                                SetGore(33);
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
                                                                                                        int num48 = 0;
                                                                                                        while ((double)num48 < dmg / (double)this.lifeMax * 100.0)
                                                                                                        {
                                                                                                            Vector2 arg_1D0A_0 = this.position;
                                                                                                            int arg_1D0A_1 = this.width;
                                                                                                            int arg_1D0A_2 = this.height;
                                                                                                            int arg_1D0A_3 = 5;
                                                                                                            float arg_1D0A_4 = (float)hitDirection;
                                                                                                            float arg_1D0A_5 = -1f;
                                                                                                            int arg_1D0A_6 = 0;
                                                                                                            Color newColor = default(Color);
                                                                                                            Dust.NewDust(arg_1D0A_0, arg_1D0A_1, arg_1D0A_2, arg_1D0A_3, arg_1D0A_4, arg_1D0A_5, arg_1D0A_6, newColor, 1f);
                                                                                                            num48++;
                                                                                                        }
                                                                                                        return;
                                                                                                    }
                                                                                                    for (int num49 = 0; num49 < 50; num49++)
                                                                                                    {
                                                                                                        Vector2 arg_1D64_0 = this.position;
                                                                                                        int arg_1D64_1 = this.width;
                                                                                                        int arg_1D64_2 = this.height;
                                                                                                        int arg_1D64_3 = 5;
                                                                                                        float arg_1D64_4 = 2.5f * (float)hitDirection;
                                                                                                        float arg_1D64_5 = -2.5f;
                                                                                                        int arg_1D64_6 = 0;
                                                                                                        Color newColor = default(Color);
                                                                                                        Dust.NewDust(arg_1D64_0, arg_1D64_1, arg_1D64_2, arg_1D64_3, arg_1D64_4, arg_1D64_5, arg_1D64_6, newColor, 1f);
                                                                                                    }
                                                                                                    SetGore(36);
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
                                                                                                            int num50 = 0;
                                                                                                            while ((double)num50 < dmg / (double)this.lifeMax * 100.0)
                                                                                                            {
                                                                                                                Vector2 arg_1E8C_0 = this.position;
                                                                                                                int arg_1E8C_1 = this.width;
                                                                                                                int arg_1E8C_2 = this.height;
                                                                                                                int arg_1E8C_3 = 5;
                                                                                                                float arg_1E8C_4 = (float)hitDirection;
                                                                                                                float arg_1E8C_5 = -1f;
                                                                                                                int arg_1E8C_6 = 0;
                                                                                                                Color newColor = default(Color);
                                                                                                                Dust.NewDust(arg_1E8C_0, arg_1E8C_1, arg_1E8C_2, arg_1E8C_3, arg_1E8C_4, arg_1E8C_5, arg_1E8C_6, newColor, 1f);
                                                                                                                num50++;
                                                                                                            }
                                                                                                            return;
                                                                                                        }
                                                                                                        for (int num51 = 0; num51 < 50; num51++)
                                                                                                        {
                                                                                                            Vector2 arg_1EE6_0 = this.position;
                                                                                                            int arg_1EE6_1 = this.width;
                                                                                                            int arg_1EE6_2 = this.height;
                                                                                                            int arg_1EE6_3 = 5;
                                                                                                            float arg_1EE6_4 = 2.5f * (float)hitDirection;
                                                                                                            float arg_1EE6_5 = -2.5f;
                                                                                                            int arg_1EE6_6 = 0;
                                                                                                            Color newColor = default(Color);
                                                                                                            Dust.NewDust(arg_1EE6_0, arg_1EE6_1, arg_1EE6_2, arg_1EE6_3, arg_1EE6_4, arg_1EE6_5, arg_1EE6_6, newColor, 1f);
                                                                                                        }
                                                                                                        SetGore(64);
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
                                                                                                                int num52 = 0;
                                                                                                                while ((double)num52 < dmg / (double)this.lifeMax * 100.0)
                                                                                                                {
                                                                                                                    Vector2 arg_200E_0 = this.position;
                                                                                                                    int arg_200E_1 = this.width;
                                                                                                                    int arg_200E_2 = this.height;
                                                                                                                    int arg_200E_3 = 5;
                                                                                                                    float arg_200E_4 = (float)hitDirection;
                                                                                                                    float arg_200E_5 = -1f;
                                                                                                                    int arg_200E_6 = 0;
                                                                                                                    Color newColor = default(Color);
                                                                                                                    Dust.NewDust(arg_200E_0, arg_200E_1, arg_200E_2, arg_200E_3, arg_200E_4, arg_200E_5, arg_200E_6, newColor, 1f);
                                                                                                                    num52++;
                                                                                                                }
                                                                                                                return;
                                                                                                            }
                                                                                                            for (int num53 = 0; num53 < 50; num53++)
                                                                                                            {
                                                                                                                Vector2 arg_2068_0 = this.position;
                                                                                                                int arg_2068_1 = this.width;
                                                                                                                int arg_2068_2 = this.height;
                                                                                                                int arg_2068_3 = 5;
                                                                                                                float arg_2068_4 = 2.5f * (float)hitDirection;
                                                                                                                float arg_2068_5 = -2.5f;
                                                                                                                int arg_2068_6 = 0;
                                                                                                                Color newColor = default(Color);
                                                                                                                Dust.NewDust(arg_2068_0, arg_2068_1, arg_2068_2, arg_2068_3, arg_2068_4, arg_2068_5, arg_2068_6, newColor, 1f);
                                                                                                            }
                                                                                                            SetGore(39);
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
                                                                                                                    int num54 = 0;
                                                                                                                    while ((double)num54 < dmg / (double)this.lifeMax * 50.0)
                                                                                                                    {
                                                                                                                        Vector2 arg_21B9_0 = this.position;
                                                                                                                        int arg_21B9_1 = this.width;
                                                                                                                        int arg_21B9_2 = this.height;
                                                                                                                        int arg_21B9_3 = 26;
                                                                                                                        float arg_21B9_4 = (float)hitDirection;
                                                                                                                        float arg_21B9_5 = -1f;
                                                                                                                        int arg_21B9_6 = 0;
                                                                                                                        Color newColor = default(Color);
                                                                                                                        Dust.NewDust(arg_21B9_0, arg_21B9_1, arg_21B9_2, arg_21B9_3, arg_21B9_4, arg_21B9_5, arg_21B9_6, newColor, 1f);
                                                                                                                        num54++;
                                                                                                                    }
                                                                                                                    return;
                                                                                                                }
                                                                                                                for (int num55 = 0; num55 < 20; num55++)
                                                                                                                {
                                                                                                                    Vector2 arg_2214_0 = this.position;
                                                                                                                    int arg_2214_1 = this.width;
                                                                                                                    int arg_2214_2 = this.height;
                                                                                                                    int arg_2214_3 = 26;
                                                                                                                    float arg_2214_4 = 2.5f * (float)hitDirection;
                                                                                                                    float arg_2214_5 = -2.5f;
                                                                                                                    int arg_2214_6 = 0;
                                                                                                                    Color newColor = default(Color);
                                                                                                                    Dust.NewDust(arg_2214_0, arg_2214_1, arg_2214_2, arg_2214_3, arg_2214_4, arg_2214_5, arg_2214_6, newColor, 1f);
                                                                                                                }
                                                                                                                SetGore(42);
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
                                                                                                                        int num56 = 0;
                                                                                                                        while ((double)num56 < dmg / (double)this.lifeMax * 50.0)
                                                                                                                        {
                                                                                                                            Vector2 arg_2351_0 = this.position;
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
                                                                                                                        Vector2 arg_23AC_0 = this.position;
                                                                                                                        int arg_23AC_1 = this.width;
                                                                                                                        int arg_23AC_2 = this.height;
                                                                                                                        int arg_23AC_3 = 26;
                                                                                                                        float arg_23AC_4 = 2.5f * (float)hitDirection;
                                                                                                                        float arg_23AC_5 = -2.5f;
                                                                                                                        int arg_23AC_6 = 0;
                                                                                                                        Color newColor = default(Color);
                                                                                                                        Dust.NewDust(arg_23AC_0, arg_23AC_1, arg_23AC_2, arg_23AC_3, arg_23AC_4, arg_23AC_5, arg_23AC_6, newColor, 1f);
                                                                                                                    }
                                                                                                                    SetGore(this.type - 39 + 67);
                                                                                                                    return;
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (this.type == 34)
                                                                                                                    {
                                                                                                                        if (this.life > 0)
                                                                                                                        {
                                                                                                                            int num58 = 0;
                                                                                                                            while ((double)num58 < dmg / (double)this.lifeMax * 30.0)
                                                                                                                            {
                                                                                                                                Vector2 arg_245C_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                int arg_245C_1 = this.width;
                                                                                                                                int arg_245C_2 = this.height;
                                                                                                                                int arg_245C_3 = 15;
                                                                                                                                float arg_245C_4 = -this.velocity.X * 0.2f;
                                                                                                                                float arg_245C_5 = -this.velocity.Y * 0.2f;
                                                                                                                                int arg_245C_6 = 100;
                                                                                                                                Color newColor = default(Color);
                                                                                                                                int num59 = Dust.NewDust(arg_245C_0, arg_245C_1, arg_245C_2, arg_245C_3, arg_245C_4, arg_245C_5, arg_245C_6, newColor, 1.8f);
                                                                                                                                Main.dust[num59].noLight = true;
                                                                                                                                Main.dust[num59].noGravity = true;
                                                                                                                                Dust expr_2487 = Main.dust[num59];
                                                                                                                                expr_2487.velocity *= 1.3f;
                                                                                                                                Vector2 arg_24F9_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                int arg_24F9_1 = this.width;
                                                                                                                                int arg_24F9_2 = this.height;
                                                                                                                                int arg_24F9_3 = 26;
                                                                                                                                float arg_24F9_4 = -this.velocity.X * 0.2f;
                                                                                                                                float arg_24F9_5 = -this.velocity.Y * 0.2f;
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
                                                                                                                            Vector2 arg_25B3_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                            int arg_25B3_1 = this.width;
                                                                                                                            int arg_25B3_2 = this.height;
                                                                                                                            int arg_25B3_3 = 15;
                                                                                                                            float arg_25B3_4 = -this.velocity.X * 0.2f;
                                                                                                                            float arg_25B3_5 = -this.velocity.Y * 0.2f;
                                                                                                                            int arg_25B3_6 = 100;
                                                                                                                            Color newColor = default(Color);
                                                                                                                            int num61 = Dust.NewDust(arg_25B3_0, arg_25B3_1, arg_25B3_2, arg_25B3_3, arg_25B3_4, arg_25B3_5, arg_25B3_6, newColor, 1.8f);
                                                                                                                            Main.dust[num61].noLight = true;
                                                                                                                            Main.dust[num61].noGravity = true;
                                                                                                                            Dust expr_25DE = Main.dust[num61];
                                                                                                                            expr_25DE.velocity *= 1.3f;
                                                                                                                            Vector2 arg_2650_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                            int arg_2650_1 = this.width;
                                                                                                                            int arg_2650_2 = this.height;
                                                                                                                            int arg_2650_3 = 26;
                                                                                                                            float arg_2650_4 = -this.velocity.X * 0.2f;
                                                                                                                            float arg_2650_5 = -this.velocity.Y * 0.2f;
                                                                                                                            int arg_2650_6 = 0;
                                                                                                                            newColor = default(Color);
                                                                                                                            num61 = Dust.NewDust(arg_2650_0, arg_2650_1, arg_2650_2, arg_2650_3, arg_2650_4, arg_2650_5, arg_2650_6, newColor, 0.9f);
                                                                                                                            Main.dust[num61].noLight = true;
                                                                                                                            Dust expr_266D = Main.dust[num61];
                                                                                                                            expr_266D.velocity *= 1.3f;
                                                                                                                        }
                                                                                                                        return;
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (this.type == 35 || this.type == 36)
                                                                                                                        {
                                                                                                                            if (this.life > 0)
                                                                                                                            {
                                                                                                                                int num62 = 0;
                                                                                                                                while ((double)num62 < dmg / (double)this.lifeMax * 100.0)
                                                                                                                                {
                                                                                                                                    Vector2 arg_26E2_0 = this.position;
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
                                                                                                                                Vector2 arg_273D_0 = this.position;
                                                                                                                                int arg_273D_1 = this.width;
                                                                                                                                int arg_273D_2 = this.height;
                                                                                                                                int arg_273D_3 = 26;
                                                                                                                                float arg_273D_4 = 2.5f * (float)hitDirection;
                                                                                                                                float arg_273D_5 = -2.5f;
                                                                                                                                int arg_273D_6 = 0;
                                                                                                                                Color newColor = default(Color);
                                                                                                                                Dust.NewDust(arg_273D_0, arg_273D_1, arg_273D_2, arg_273D_3, arg_273D_4, arg_273D_5, arg_273D_6, newColor, 1f);
                                                                                                                            }
                                                                                                                            if (this.type == 35)
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
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (this.type == 23)
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
                                                                                                                                        Vector2 arg_2836_0 = this.position;
                                                                                                                                        int arg_2836_1 = this.width;
                                                                                                                                        int arg_2836_2 = this.height;
                                                                                                                                        int arg_2836_3 = num65;
                                                                                                                                        float arg_2836_4 = (float)hitDirection;
                                                                                                                                        float arg_2836_5 = -1f;
                                                                                                                                        int arg_2836_6 = 0;
                                                                                                                                        Color newColor = default(Color);
                                                                                                                                        Dust.NewDust(arg_2836_0, arg_2836_1, arg_2836_2, arg_2836_3, arg_2836_4, arg_2836_5, arg_2836_6, newColor, 1f);
                                                                                                                                        Vector2 arg_2897_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                        int arg_2897_1 = this.width;
                                                                                                                                        int arg_2897_2 = this.height;
                                                                                                                                        int arg_2897_3 = 6;
                                                                                                                                        float arg_2897_4 = this.velocity.X * 0.2f;
                                                                                                                                        float arg_2897_5 = this.velocity.Y * 0.2f;
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
                                                                                                                                    Vector2 arg_2914_0 = this.position;
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
                                                                                                                                    Vector2 arg_2989_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                    int arg_2989_1 = this.width;
                                                                                                                                    int arg_2989_2 = this.height;
                                                                                                                                    int arg_2989_3 = 6;
                                                                                                                                    float arg_2989_4 = this.velocity.X * 0.2f;
                                                                                                                                    float arg_2989_5 = this.velocity.Y * 0.2f;
                                                                                                                                    int arg_2989_6 = 100;
                                                                                                                                    Color newColor = default(Color);
                                                                                                                                    int num70 = Dust.NewDust(arg_2989_0, arg_2989_1, arg_2989_2, arg_2989_3, arg_2989_4, arg_2989_5, arg_2989_6, newColor, 2.5f);
                                                                                                                                    Dust expr_2998 = Main.dust[num70];
                                                                                                                                    expr_2998.velocity *= 6f;
                                                                                                                                    Main.dust[num70].noGravity = true;
                                                                                                                                }
                                                                                                                                return;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (this.type == 24)
                                                                                                                                {
                                                                                                                                    if (this.life > 0)
                                                                                                                                    {
                                                                                                                                        int num71 = 0;
                                                                                                                                        while ((double)num71 < dmg / (double)this.lifeMax * 100.0)
                                                                                                                                        {
                                                                                                                                            Vector2 arg_2A38_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                            int arg_2A38_1 = this.width;
                                                                                                                                            int arg_2A38_2 = this.height;
                                                                                                                                            int arg_2A38_3 = 6;
                                                                                                                                            float arg_2A38_4 = this.velocity.X;
                                                                                                                                            float arg_2A38_5 = this.velocity.Y;
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
                                                                                                                                        Vector2 arg_2AC6_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                        int arg_2AC6_1 = this.width;
                                                                                                                                        int arg_2AC6_2 = this.height;
                                                                                                                                        int arg_2AC6_3 = 6;
                                                                                                                                        float arg_2AC6_4 = this.velocity.X;
                                                                                                                                        float arg_2AC6_5 = this.velocity.Y;
                                                                                                                                        int arg_2AC6_6 = 100;
                                                                                                                                        Color newColor = default(Color);
                                                                                                                                        int num74 = Dust.NewDust(arg_2AC6_0, arg_2AC6_1, arg_2AC6_2, arg_2AC6_3, arg_2AC6_4, arg_2AC6_5, arg_2AC6_6, newColor, 2.5f);
                                                                                                                                        Main.dust[num74].noGravity = true;
                                                                                                                                        Dust expr_2AE3 = Main.dust[num74];
                                                                                                                                        expr_2AE3.velocity *= 2f;
                                                                                                                                    }
                                                                                                                                    SetGore(45);
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
                                                                                                                                        for (int num75 = 0; num75 < 20; num75++)
                                                                                                                                        {
                                                                                                                                            Vector2 arg_2C6A_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                            int arg_2C6A_1 = this.width;
                                                                                                                                            int arg_2C6A_2 = this.height;
                                                                                                                                            int arg_2C6A_3 = 6;
                                                                                                                                            float arg_2C6A_4 = -this.velocity.X * 0.2f;
                                                                                                                                            float arg_2C6A_5 = -this.velocity.Y * 0.2f;
                                                                                                                                            int arg_2C6A_6 = 100;
                                                                                                                                            Color newColor = default(Color);
                                                                                                                                            int num76 = Dust.NewDust(arg_2C6A_0, arg_2C6A_1, arg_2C6A_2, arg_2C6A_3, arg_2C6A_4, arg_2C6A_5, arg_2C6A_6, newColor, 2f);
                                                                                                                                            Main.dust[num76].noGravity = true;
                                                                                                                                            Dust expr_2C87 = Main.dust[num76];
                                                                                                                                            expr_2C87.velocity *= 2f;
                                                                                                                                            Vector2 arg_2CF9_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                            int arg_2CF9_1 = this.width;
                                                                                                                                            int arg_2CF9_2 = this.height;
                                                                                                                                            int arg_2CF9_3 = 6;
                                                                                                                                            float arg_2CF9_4 = -this.velocity.X * 0.2f;
                                                                                                                                            float arg_2CF9_5 = -this.velocity.Y * 0.2f;
                                                                                                                                            int arg_2CF9_6 = 100;
                                                                                                                                            newColor = default(Color);
                                                                                                                                            num76 = Dust.NewDust(arg_2CF9_0, arg_2CF9_1, arg_2CF9_2, arg_2CF9_3, arg_2CF9_4, arg_2CF9_5, arg_2CF9_6, newColor, 1f);
                                                                                                                                            Dust expr_2D08 = Main.dust[num76];
                                                                                                                                            expr_2D08.velocity *= 2f;
                                                                                                                                        }
                                                                                                                                        return;
                                                                                                                                    }
                                                                                                                                    if (this.type == 33)
                                                                                                                                    {
                                                                                                                                        for (int num77 = 0; num77 < 20; num77++)
                                                                                                                                        {
                                                                                                                                            Vector2 arg_2DC0_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                            int arg_2DC0_1 = this.width;
                                                                                                                                            int arg_2DC0_2 = this.height;
                                                                                                                                            int arg_2DC0_3 = 29;
                                                                                                                                            float arg_2DC0_4 = -this.velocity.X * 0.2f;
                                                                                                                                            float arg_2DC0_5 = -this.velocity.Y * 0.2f;
                                                                                                                                            int arg_2DC0_6 = 100;
                                                                                                                                            Color newColor = default(Color);
                                                                                                                                            int num78 = Dust.NewDust(arg_2DC0_0, arg_2DC0_1, arg_2DC0_2, arg_2DC0_3, arg_2DC0_4, arg_2DC0_5, arg_2DC0_6, newColor, 2f);
                                                                                                                                            Main.dust[num78].noGravity = true;
                                                                                                                                            Dust expr_2DDD = Main.dust[num78];
                                                                                                                                            expr_2DDD.velocity *= 2f;
                                                                                                                                            Vector2 arg_2E50_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                            int arg_2E50_1 = this.width;
                                                                                                                                            int arg_2E50_2 = this.height;
                                                                                                                                            int arg_2E50_3 = 29;
                                                                                                                                            float arg_2E50_4 = -this.velocity.X * 0.2f;
                                                                                                                                            float arg_2E50_5 = -this.velocity.Y * 0.2f;
                                                                                                                                            int arg_2E50_6 = 100;
                                                                                                                                            newColor = default(Color);
                                                                                                                                            num78 = Dust.NewDust(arg_2E50_0, arg_2E50_1, arg_2E50_2, arg_2E50_3, arg_2E50_4, arg_2E50_5, arg_2E50_6, newColor, 1f);
                                                                                                                                            Dust expr_2E5F = Main.dust[num78];
                                                                                                                                            expr_2E5F.velocity *= 2f;
                                                                                                                                        }
                                                                                                                                        return;
                                                                                                                                    }
                                                                                                                                    if (this.type == 26 || this.type == 27 || this.type == 28 || this.type == 29)
                                                                                                                                    {
                                                                                                                                        if (this.life > 0)
                                                                                                                                        {
                                                                                                                                            int num79 = 0;
                                                                                                                                            while ((double)num79 < dmg / (double)this.lifeMax * 100.0)
                                                                                                                                            {
                                                                                                                                                Vector2 arg_2EE7_0 = this.position;
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
                                                                                                                                            Vector2 arg_2F41_0 = this.position;
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
                                                                                                                                            for (int num81 = 0; num81 < 20; num81++)
                                                                                                                                            {
                                                                                                                                                Vector2 arg_30B7_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                                int arg_30B7_1 = this.width;
                                                                                                                                                int arg_30B7_2 = this.height;
                                                                                                                                                int arg_30B7_3 = 27;
                                                                                                                                                float arg_30B7_4 = -this.velocity.X * 0.2f;
                                                                                                                                                float arg_30B7_5 = -this.velocity.Y * 0.2f;
                                                                                                                                                int arg_30B7_6 = 100;
                                                                                                                                                Color newColor = default(Color);
                                                                                                                                                int num82 = Dust.NewDust(arg_30B7_0, arg_30B7_1, arg_30B7_2, arg_30B7_3, arg_30B7_4, arg_30B7_5, arg_30B7_6, newColor, 2f);
                                                                                                                                                Main.dust[num82].noGravity = true;
                                                                                                                                                Dust expr_30D4 = Main.dust[num82];
                                                                                                                                                expr_30D4.velocity *= 2f;
                                                                                                                                                Vector2 arg_3147_0 = new Vector2(this.position.X, this.position.Y);
                                                                                                                                                int arg_3147_1 = this.width;
                                                                                                                                                int arg_3147_2 = this.height;
                                                                                                                                                int arg_3147_3 = 27;
                                                                                                                                                float arg_3147_4 = -this.velocity.X * 0.2f;
                                                                                                                                                float arg_3147_5 = -this.velocity.Y * 0.2f;
                                                                                                                                                int arg_3147_6 = 100;
                                                                                                                                                newColor = default(Color);
                                                                                                                                                num82 = Dust.NewDust(arg_3147_0, arg_3147_1, arg_3147_2, arg_3147_3, arg_3147_4, arg_3147_5, arg_3147_6, newColor, 1f);
                                                                                                                                                Dust expr_3156 = Main.dust[num82];
                                                                                                                                                expr_3156.velocity *= 2f;
                                                                                                                                            }
                                                                                                                                            return;
                                                                                                                                        }
                                                                                                                                        if (this.type == 42)
                                                                                                                                        {
                                                                                                                                            if (this.life > 0)
                                                                                                                                            {
                                                                                                                                                int num83 = 0;
                                                                                                                                                while ((double)num83 < dmg / (double)this.lifeMax * 100.0)
                                                                                                                                                {
                                                                                                                                                    Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -1f, this.alpha, this.color, this.scale);
                                                                                                                                                    num83++;
                                                                                                                                                }
                                                                                                                                                return;
                                                                                                                                            }
                                                                                                                                            for (int num84 = 0; num84 < 50; num84++)
                                                                                                                                            {
                                                                                                                                                Dust.NewDust(this.position, this.width, this.height, 18, (float)hitDirection, -2f, this.alpha, this.color, this.scale);
                                                                                                                                            }
                                                                                                                                            SetGore(70);
                                                                                                                                            SetGore(71);
                                                                                                                                            return;
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (this.type == 43 || this.type == 56)
                                                                                                                                            {
                                                                                                                                                if (this.life > 0)
                                                                                                                                                {
                                                                                                                                                    int num85 = 0;
                                                                                                                                                    while ((double)num85 < dmg / (double)this.lifeMax * 100.0)
                                                                                                                                                    {
                                                                                                                                                        Dust.NewDust(this.position, this.width, this.height, 40, (float)hitDirection, -1f, this.alpha, this.color, 1.2f);
                                                                                                                                                        num85++;
                                                                                                                                                    }
                                                                                                                                                    return;
                                                                                                                                                }
                                                                                                                                                for (int num86 = 0; num86 < 50; num86++)
                                                                                                                                                {
                                                                                                                                                    Dust.NewDust(this.position, this.width, this.height, 40, (float)hitDirection, -2f, this.alpha, this.color, 1.2f);
                                                                                                                                                }
                                                                                                                                                SetGore(72);
                                                                                                                                                SetGore(72);
                                                                                                                                                return;
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (this.type == 48)
                                                                                                                                                {
                                                                                                                                                    if (this.life > 0)
                                                                                                                                                    {
                                                                                                                                                        int num87 = 0;
                                                                                                                                                        while ((double)num87 < dmg / (double)this.lifeMax * 100.0)
                                                                                                                                                        {
                                                                                                                                                            Vector2 arg_337C_0 = this.position;
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
                                                                                                                                                        Vector2 arg_33D2_0 = this.position;
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
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    if (this.type == 62 || this.type == 66)
                                                                                                                                                    {
                                                                                                                                                        if (this.life > 0)
                                                                                                                                                        {
                                                                                                                                                            int num89 = 0;
                                                                                                                                                            while ((double)num89 < dmg / (double)this.lifeMax * 100.0)
                                                                                                                                                            {
                                                                                                                                                                Vector2 arg_345C_0 = this.position;
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
                                                                                                                                                            Vector2 arg_34B2_0 = this.position;
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
                return;
            }
            Color newColor2 = new Color(50, 120, 255, 100);
            if (this.type == 64)
            {
                newColor2 = new Color(225, 70, 140, 100);
            }
            if (this.life > 0)
            {
                int num91 = 0;
                while ((double)num91 < dmg / (double)this.lifeMax * 50.0)
                {
                    Dust.NewDust(this.position, this.width, this.height, 4, (float)hitDirection, -1f, 0, newColor2, 1f);
                    num91++;
                }
                return;
            }
            for (int num92 = 0; num92 < 25; num92++)
            {
                Dust.NewDust(this.position, this.width, this.height, 4, (float)(2 * hitDirection), -2f, 0, newColor2, 1f);
            }
        }
        
        public static bool AnyNPCs(int Type)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == Type)
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
            for (int i = 0; i < 1000; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == 35)
                {
                    flag = false;
                    break;
                }
            }
            for (int j = 0; j < 1000; j++)
            {
                if (Main.npc[j].active && Main.npc[j].type == 37)
                {
                    flag2 = true;
                    Main.npc[j].ai[3] = 1f;
                    vector = Main.npc[j].position;
                    num = Main.npc[j].width;
                    num2 = Main.npc[j].height;
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(23, -1, -1, "", j);
                    }
                }
            }
            if (flag && flag2)
            {
                int num3 = NPC.NewNPC((int)vector.X + num / 2, (int)vector.Y + num2 / 2, 35, 0);
                Main.npc[num3].netUpdate = true;
                string str = "Skeletron";
                if (Main.netMode == 0)
                {
                    return;
                }
                if (Main.netMode == 2)
                {
                    NetMessage.SendData(25, -1, -1, str + " has awoken!", 255, 175f, 75f, 255f);
                }
            }
        }
        
        public void UpdateNPC(int i)
        {
            this.whoAmI = i;
            if (this.active)
            {
                if (Main.netMode != 1 && Main.bloodMoon)
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
                            if (Main.npc[k].active && !Main.npc[k].friendly && Main.npc[k].damage > 0)
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
                        if (!this.lavaImmune && Main.netMode != 1 && this.immune[255] == 0)
                        {
                            this.immune[255] = 30;
                            this.StrikeNPC(50, 0f, 0);
                            if (Main.netMode == 2 && Main.netMode != 0)
                            {
                                NetMessage.SendData(28, -1, -1, "", this.whoAmI, 50f);
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
                                for (int l = 0; l < 30; l++)
                                {
                                    int num6 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 33, 0f, 0f, 0, default(Color), 1f);
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
                                    int num7 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 35, 0f, 0f, 0, default(Color), 1f);
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
                                    for (int n = 0; n < 30; n++)
                                    {
                                        int num8 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 33, 0f, 0f, 0, default(Color), 1f);
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
                                        int num10 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 35, 0f, 0f, 0, default(Color), 1f);
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
                        if (Collision.up)
                        {
                            this.velocity.Y = 0.01f;
                        }
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
                        if (Collision.up)
                        {
                            this.velocity.Y = 0.01f;
                        }
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
                    NetMessage.SendData(23, -1, -1, "", i);
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
                                result = "Ah, they will tell tales of " + Main.players[Main.myPlayer].name + " some day... good ones I'm sure.";
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
                                result = Main.players[Main.myPlayer].name + " is it? I've heard good things, friend!";
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
                                            result = "Greetings, " + Main.players[Main.myPlayer].name + ". Is there something I can help you with?";
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
                                        if (this.type == 54)
                                        {
                                            if (Main.bloodMoon)
                                            {
                                                result = Main.players[Main.myPlayer].name + "... we have a problem! Its a blood moon out there!";
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
        
        public object Clone()
        {
            return base.MemberwiseClone();
        }
    }
}
