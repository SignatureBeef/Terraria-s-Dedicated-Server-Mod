using Terraria_Server.Commands;
using System;
using Terraria_Server.Events;
using Terraria_Server.Plugin;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Terraria_Server.Misc;
using Terraria_Server.Shops;
using Terraria_Server.Definitions;

namespace Terraria_Server
{
	public class Player : Sender
	{
        private String ipAddress = null;
        private bool godMode = false;

        public bool enemySpawns;
        public int heldProj = -1;
        public bool killGuide;
        public int[] buffType = new int[10];
        public int[] buffTime = new int[10];
        public bool hardCore;
		public bool pvpDeath;
		public bool zoneDungeon;
		public bool zoneEvil;
		public bool zoneMeteor;
		public bool zoneJungle;
		public bool boneArmor;
		public int townNPCs;
		public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 oldVelocity;
		public double headFrameCounter;
		public double bodyFrameCounter;
		public double legFrameCounter;
		public bool immune;
		public int immuneTime;
		public int immuneAlphaDirection;
		public int immuneAlpha;
		public int team;
		public String chatText = "";
		public int sign = -1;
		public int chatShowTime;
		public int activeNPCs;
		public bool mouseInterface;
		public int changeItem = -1;
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
		public int breathMax = 200;
		public int breath = 200;
		public String setBonus = "";
		public Item[] inventory = new Item[44];
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
		public String Name = "";
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
		public bool Active;
		public int width = 20;
		public int height = 42;
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
		public int[] spX = new int[200];
		public int[] spY = new int[200];
		public String[] spN = new String[200];
		public int[] spI = new int[200];
		public static int tileRangeX = 5;
		public static int tileRangeY = 4;
		private static int tileTargetX = 0;
		private static int tileTargetY = 0;
		private static int jumpHeight = 15;
		private static float jumpSpeed = 5.01f;
		public bool[] adjTile = new bool[80];
		public bool[] oldAdjTile = new bool[80];
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

        public override String getName()
        {
            return Name;
        }

        public override void sendMessage(String Message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
            NetMessage.SendData((int)Packet.PLAYER_CHAT, whoAmi, -1, Message, A, R, G, B);
        }

        public void HealEffect(int healAmount, bool overrider = false, int remoteClient = -1)
		{
            if (overrider || (Main.netMode == 1 && this.whoAmi == Main.myPlayer))
			{
                NetMessage.SendData(35, remoteClient, -1, "", this.whoAmi, (float)healAmount);
			}
		}

        public void ManaEffect(int manaAmount, bool overrider = false, int remoteClient = -1)
		{
			if (overrider || (Main.netMode == 1 && this.whoAmi == Main.myPlayer))
			{
                NetMessage.SendData(43, remoteClient, -1, "", this.whoAmi, (float)manaAmount);
			}
		}
		
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
				if (Main.players[j].Active && !Main.players[j].dead && (num == -1f || Math.Abs(Main.players[j].Position.X + (float)(Main.players[j].width / 2) - Position.X + (float)(Width / 2)) + Math.Abs(Main.players[j].Position.Y + (float)(Main.players[j].height / 2) - Position.Y + (float)(Height / 2)) < num))
				{
					num = Math.Abs(Main.players[j].Position.X + (float)(Main.players[j].width / 2) - Position.X + (float)(Width / 2)) + Math.Abs(Main.players[j].Position.Y + (float)(Main.players[j].height / 2) - Position.Y + (float)(Height / 2));
					result = (byte)j;
				}
			}
			return result;
		}
		
        public void UpdatePlayer(int i)
		{
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
                    if (this.dead)
                    {
                        if (i == Main.myPlayer)
                        {
                            Main.npcChatText = "";
                            Main.editSign = false;
                        }
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
                        this.respawnTimer--;
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
                        if (this.respawnTimer <= 0 && Main.myPlayer == this.whoAmi)
                        {
                            this.Spawn();
                            return;
                        }
                    }
                    else
                    {
                        if (i == Main.myPlayer)
                        {
                            this.zoneEvil = false;
                            if (Main.evilTiles >= 500)
                            {
                                this.zoneEvil = true;
                            }
                            this.zoneMeteor = false;
                            if (Main.meteorTiles >= 50)
                            {
                                this.zoneMeteor = true;
                            }
                            this.zoneDungeon = false;
                            if (Main.dungeonTiles >= 250 && (double)this.Position.Y > Main.worldSurface * 16.0 + (double)Main.screenHeight)
                            {
                                int num7 = (int)this.Position.X / 16;
                                int num8 = (int)this.Position.Y / 16;
                                if (Main.tile[num7, num8].wall > 0 && !Main.wallHouse[(int)Main.tile[num7, num8].wall])
                                {
                                    this.zoneDungeon = true;
                                }
                            }
                            this.zoneJungle = false;
                            if (Main.jungleTiles >= 200)
                            {
                                this.zoneJungle = true;
                            }
                            this.controlUp = false;
                            this.controlLeft = false;
                            this.controlDown = false;
                            this.controlRight = false;
                            this.controlJump = false;
                            this.controlUseItem = false;
                            this.controlUseTile = false;
                            this.controlThrow = false;
                            this.controlInv = false;

                            if (Main.playerInventory)
                            {
                                this.AdjTiles();
                            }
                            if (this.chest != -1)
                            {
                                int num12 = (int)(((double)this.Position.X + (double)this.width * 0.5) / 16.0);
                                int num13 = (int)(((double)this.Position.Y + (double)this.height * 0.5) / 16.0);
                                if (num12 < this.chestX - 5 || num12 > this.chestX + 6 || num13 < this.chestY - 4 || num13 > this.chestY + 5)
                                {
                                    this.chest = -1;
                                }
                                if (!Main.tile[this.chestX, this.chestY].Active)
                                {
                                    this.chest = -1;
                                }
                            }
                            if (this.Velocity.Y == 0f)
                            {
                                int num14 = (int)(this.Position.Y / 16f) - this.fallStart;
                                if (num14 > 25 && !this.noFallDmg)
                                {
                                    int damage = (num14 - 25) * 10;
                                    this.immune = false;
                                    this.Hurt(damage, -this.direction, false, false);
                                }
                                this.fallStart = (int)(this.Position.Y / 16f);
                            }
                            if (this.Velocity.Y < 0f || this.rocketDelay > 0 || this.wet)
                            {
                                this.fallStart = (int)(this.Position.Y / 16f);
                            }
                        }
                        if (this.mouseInterface)
                        {
                            this.delayUseItem = true;
                        }
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
                        if (this.manaRegenDelay > 0)
                        {
                            this.manaRegenDelay--;
                        }
                        this.statDefense = 0;
                        this.accWatch = 0;
                        this.accDepthMeter = 0;
                        this.lifeRegen = 0;
                        this.manaCost = 1f;
                        this.meleeSpeed = 1f;
                        this.boneArmor = false;
                        this.rocketBoots = false;
                        this.fireWalk = false;
                        this.noKnockback = false;
                        this.jumpBoost = false;
                        this.noFallDmg = false;
                        this.accFlipper = false;
                        this.spawnMax = false;
                        this.spaceGun = false;
                        this.magicBoost = 1f;
                        if (this.manaRegenDelay == 0)
                        {
                            this.manaRegen = this.statManaMax / 30 + 1;
                        }
                        else
                        {
                            this.manaRegen = 0;
                        }
                        this.doubleJump = false;
                        for (int l = 0; l < 8; l++)
                        {
                            this.statDefense += this.armor[l].Defense;
                            this.lifeRegen += this.armor[l].LifeRegen;
                            this.manaRegen += this.armor[l].ManaRegen;
                            if (this.armor[l].Type == 193)
                            {
                                this.fireWalk = true;
                            }
                            if (this.armor[l].Type == 238)
                            {
                                this.magicBoost *= 1.15f;
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
                                this.meleeSpeed *= 0.9f;
                            }
                            if (this.armor[m].Type == 223)
                            {
                                this.spawnMax = true;
                            }
                            if (this.armor[m].Type == 212)
                            {
                                num4 *= 1.1f;
                                num3 *= 1.1f;
                            }
                        }
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
                        this.manaRegenCount += this.manaRegen;
                        while (this.manaRegenCount >= 120)
                        {
                            this.manaRegenCount -= 120;
                            if (this.statMana < this.statManaMax)
                            {
                                this.statMana++;
                            }
                            if (this.statMana > this.statManaMax)
                            {
                                this.statMana = this.statManaMax;
                            }
                        }
                        if (this.head == 11)
                        {
                            int i2 = (int)(this.Position.X + (float)(this.width / 2) + (float)(8 * this.direction)) / 16;
                            int j2 = (int)(this.Position.Y + 2f) / 16;
                        }
                        if (this.jumpBoost)
                        {
                            Player.jumpHeight = 20;
                            Player.jumpSpeed = 6.51f;
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
                            this.setBonus = "15 % increased melee speed";
                            this.meleeSpeed *= 0.85f;
                            if (Main.rand.Next(10) == 0)
                            {
                                Vector2 arg_147A_0 = new Vector2(this.Position.X, this.Position.Y);
                                int arg_147A_1 = this.width;
                                int arg_147A_2 = this.height;
                                int arg_147A_3 = 14;
                                float arg_147A_4 = 0f;
                                float arg_147A_5 = 0f;
                                int arg_147A_6 = 200;
                                Color newColor = new Color();
                                Dust.NewDust(arg_147A_0, arg_147A_1, arg_147A_2, arg_147A_3, arg_147A_4, arg_147A_5, arg_147A_6, newColor, 1.2f);
                            }
                        }
                        if (this.head == 6 && this.body == 6 && this.legs == 6)
                        {
                            this.setBonus = "Space Gun costs 0 mana";
                            this.spaceGun = true;
                            if (Math.Abs(this.Velocity.X) + Math.Abs(this.Velocity.Y) > 1f && !this.rocketFrame)
                            {
                                for (int n = 0; n < 2; n++)
                                {
                                    Vector2 arg_1561_0 = new Vector2(this.Position.X - this.Velocity.X * 2f, this.Position.Y - 2f - this.Velocity.Y * 2f);
                                    int arg_1561_1 = this.width;
                                    int arg_1561_2 = this.height;
                                    int arg_1561_3 = 6;
                                    float arg_1561_4 = 0f;
                                    float arg_1561_5 = 0f;
                                    int arg_1561_6 = 100;
                                    Color newColor = new Color();
                                    int num15 = Dust.NewDust(arg_1561_0, arg_1561_1, arg_1561_2, arg_1561_3, arg_1561_4, arg_1561_5, arg_1561_6, newColor, 2f);
                                    Main.dust[num15].noGravity = true;
                                    Dust expr_1583_cp_0 = Main.dust[num15];
                                    expr_1583_cp_0.velocity.X = expr_1583_cp_0.velocity.X - this.Velocity.X * 0.5f;
                                    Dust expr_15AD_cp_0 = Main.dust[num15];
                                    expr_15AD_cp_0.velocity.Y = expr_15AD_cp_0.velocity.Y - this.Velocity.Y * 0.5f;
                                }
                            }
                        }
                        if (this.head == 7 && this.body == 7 && this.legs == 7)
                        {
                            num4 *= 1.3f;
                            num3 *= 1.3f;
                            this.setBonus = "30% increased movement speed";
                            this.boneArmor = true;
                        }
                        if (this.head == 8 && this.body == 8 && this.legs == 8)
                        {
                            this.setBonus = "25% reduced mana usage";
                            this.manaCost *= 0.75f;
                            this.meleeSpeed *= 0.9f;
                            if (Math.Abs(this.Velocity.X) + Math.Abs(this.Velocity.Y) > 1f)
                            {
                                Vector2 arg_1701_0 = new Vector2(this.Position.X - this.Velocity.X * 2f, this.Position.Y - 2f - this.Velocity.Y * 2f);
                                int arg_1701_1 = this.width;
                                int arg_1701_2 = this.height;
                                int arg_1701_3 = 40;
                                float arg_1701_4 = 0f;
                                float arg_1701_5 = 0f;
                                int arg_1701_6 = 50;
                                Color newColor = new Color();
                                int num16 = Dust.NewDust(arg_1701_0, arg_1701_1, arg_1701_2, arg_1701_3, arg_1701_4, arg_1701_5, arg_1701_6, newColor, 1.4f);
                                Main.dust[num16].noGravity = true;
                                Main.dust[num16].velocity.X = this.Velocity.X * 0.25f;
                                Main.dust[num16].velocity.Y = this.Velocity.Y * 0.25f;
                            }
                        }
                        if (this.head == 9 && this.body == 9 && this.legs == 9)
                        {
                            this.setBonus = "5 defense";
                            this.statDefense += 5;
                            if (Math.Abs(this.Velocity.X) + Math.Abs(this.Velocity.Y) > 1f && !this.rocketFrame)
                            {
                                for (int num17 = 0; num17 < 2; num17++)
                                {
                                    Vector2 arg_1847_0 = new Vector2(this.Position.X - this.Velocity.X * 2f, this.Position.Y - 2f - this.Velocity.Y * 2f);
                                    int arg_1847_1 = this.width;
                                    int arg_1847_2 = this.height;
                                    int arg_1847_3 = 6;
                                    float arg_1847_4 = 0f;
                                    float arg_1847_5 = 0f;
                                    int arg_1847_6 = 100;
                                    Color newColor = new Color();
                                    int num18 = Dust.NewDust(arg_1847_0, arg_1847_1, arg_1847_2, arg_1847_3, arg_1847_4, arg_1847_5, arg_1847_6, newColor, 2f);
                                    Main.dust[num18].noGravity = true;
                                    Dust expr_1869_cp_0 = Main.dust[num18];
                                    expr_1869_cp_0.velocity.X = expr_1869_cp_0.velocity.X - this.Velocity.X * 0.5f;
                                    Dust expr_1893_cp_0 = Main.dust[num18];
                                    expr_1893_cp_0.velocity.Y = expr_1893_cp_0.velocity.Y - this.Velocity.Y * 0.5f;
                                }
                            }
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
                        if ((double)this.meleeSpeed < 0.7)
                        {
                            this.meleeSpeed = 0.7f;
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
                                if (itemAnimation == 0 || selectedItem.UseTurn)
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
                                    if (this.itemAnimation == 0 || selectedItem.UseTurn)
                                    {
                                        this.direction = 1;
                                    }
                                }
                                else
                                {
                                    if (this.controlLeft && this.Velocity.X > -num6)
                                    {
                                        if (this.itemAnimation == 0 || selectedItem.UseTurn)
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
                                            if (this.runSoundDelay == 0 && this.Velocity.Y == 0f)
                                            {
                                                this.runSoundDelay = 9;
                                            }
                                            Vector2 arg_1B6C_0 = new Vector2(this.Position.X - 4f, this.Position.Y + (float)this.height);
                                            int arg_1B6C_1 = this.width + 8;
                                            int arg_1B6C_2 = 4;
                                            int arg_1B6C_3 = 16;
                                            float arg_1B6C_4 = -this.Velocity.X * 0.5f;
                                            float arg_1B6C_5 = this.Velocity.Y * 0.5f;
                                            int arg_1B6C_6 = 50;
                                            Color newColor = new Color();
                                            int num19 = Dust.NewDust(arg_1B6C_0, arg_1B6C_1, arg_1B6C_2, arg_1B6C_3, arg_1B6C_4, arg_1B6C_5, arg_1B6C_6, newColor, 1.5f);
                                            Main.dust[num19].velocity.X = Main.dust[num19].velocity.X * 0.2f;
                                            Main.dust[num19].velocity.Y = Main.dust[num19].velocity.Y * 0.2f;
                                        }
                                    }
                                    else
                                    {
                                        if (this.controlRight && this.Velocity.X < num6)
                                        {
                                            if (this.itemAnimation == 0 || selectedItem.UseTurn)
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
                                                if (this.runSoundDelay == 0 && this.Velocity.Y == 0f)
                                                {
                                                    this.runSoundDelay = 9;
                                                }
                                                Vector2 arg_1D34_0 = new Vector2(this.Position.X - 4f, this.Position.Y + (float)this.height);
                                                int arg_1D34_1 = this.width + 8;
                                                int arg_1D34_2 = 4;
                                                int arg_1D34_3 = 16;
                                                float arg_1D34_4 = -this.Velocity.X * 0.5f;
                                                float arg_1D34_5 = this.Velocity.Y * 0.5f;
                                                int arg_1D34_6 = 50;
                                                Color newColor = new Color();
                                                int num20 = Dust.NewDust(arg_1D34_0, arg_1D34_1, arg_1D34_2, arg_1D34_3, arg_1D34_4, arg_1D34_5, arg_1D34_6, newColor, 1.5f);
                                                Main.dust[num20].velocity.X = Main.dust[num20].velocity.X * 0.2f;
                                                Main.dust[num20].velocity.Y = Main.dust[num20].velocity.Y * 0.2f;
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
                            if (this.controlJump)
                            {
                                if (this.jump > 0)
                                {
                                    if (this.Velocity.Y > -Player.jumpSpeed + num2 * 2f)
                                    {
                                        this.jump = 0;
                                    }
                                    else
                                    {
                                        this.Velocity.Y = -Player.jumpSpeed;
                                        this.jump--;
                                    }
                                }
                                else
                                {
                                    if ((this.Velocity.Y == 0f || this.jumpAgain || (this.wet && this.accFlipper)) && this.releaseJump)
                                    {
                                        bool flag3 = false;
                                        if (this.wet && this.accFlipper)
                                        {
                                            if (this.swimTime == 0)
                                            {
                                                this.swimTime = 30;
                                            }
                                            flag3 = true;
                                        }
                                        this.jumpAgain = false;
                                        this.canRocket = false;
                                        this.rocketRelease = false;
                                        if (this.Velocity.Y == 0f && this.doubleJump)
                                        {
                                            this.jumpAgain = true;
                                        }
                                        if (this.Velocity.Y == 0f || flag3)
                                        {
                                            this.Velocity.Y = -Player.jumpSpeed;
                                            this.jump = Player.jumpHeight;
                                        }
                                        else
                                        {
                                            this.Velocity.Y = -Player.jumpSpeed;
                                            this.jump = Player.jumpHeight / 2;
                                            for (int num21 = 0; num21 < 10; num21++)
                                            {
                                                Vector2 arg_2064_0 = new Vector2(this.Position.X - 34f, this.Position.Y + (float)this.height - 16f);
                                                int arg_2064_1 = 102;
                                                int arg_2064_2 = 32;
                                                int arg_2064_3 = 16;
                                                float arg_2064_4 = -this.Velocity.X * 0.5f;
                                                float arg_2064_5 = this.Velocity.Y * 0.5f;
                                                int arg_2064_6 = 100;
                                                Color newColor = new Color();
                                                int num22 = Dust.NewDust(arg_2064_0, arg_2064_1, arg_2064_2, arg_2064_3, arg_2064_4, arg_2064_5, arg_2064_6, newColor, 1.5f);
                                                Main.dust[num22].velocity.X = Main.dust[num22].velocity.X * 0.5f - this.Velocity.X * 0.1f;
                                                Main.dust[num22].velocity.Y = Main.dust[num22].velocity.Y * 0.5f - this.Velocity.Y * 0.3f;
                                            }
                                            int num23 = Gore.NewGore(new Vector2(this.Position.X + (float)(this.width / 2) - 16f, this.Position.Y + (float)this.height - 16f), new Vector2(-this.Velocity.X, -this.Velocity.Y), Main.rand.Next(11, 14));
                                            Main.gore[num23].velocity.X = Main.gore[num23].velocity.X * 0.1f - this.Velocity.X * 0.1f;
                                            Main.gore[num23].velocity.Y = Main.gore[num23].velocity.Y * 0.1f - this.Velocity.Y * 0.05f;
                                            num23 = Gore.NewGore(new Vector2(this.Position.X - 36f, this.Position.Y + (float)this.height - 16f), new Vector2(-this.Velocity.X, -this.Velocity.Y), Main.rand.Next(11, 14));
                                            Main.gore[num23].velocity.X = Main.gore[num23].velocity.X * 0.1f - this.Velocity.X * 0.1f;
                                            Main.gore[num23].velocity.Y = Main.gore[num23].velocity.Y * 0.1f - this.Velocity.Y * 0.05f;
                                            num23 = Gore.NewGore(new Vector2(this.Position.X + (float)this.width + 4f, this.Position.Y + (float)this.height - 16f), new Vector2(-this.Velocity.X, -this.Velocity.Y), Main.rand.Next(11, 14));
                                            Main.gore[num23].velocity.X = Main.gore[num23].velocity.X * 0.1f - this.Velocity.X * 0.1f;
                                            Main.gore[num23].velocity.Y = Main.gore[num23].velocity.Y * 0.1f - this.Velocity.Y * 0.05f;
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
                            if (this.doubleJump && !this.jumpAgain && this.Velocity.Y < 0f && !this.rocketBoots && !this.accFlipper)
                            {
                                Vector2 arg_2456_0 = new Vector2(this.Position.X - 4f, this.Position.Y + (float)this.height);
                                int arg_2456_1 = this.width + 8;
                                int arg_2456_2 = 4;
                                int arg_2456_3 = 16;
                                float arg_2456_4 = -this.Velocity.X * 0.5f;
                                float arg_2456_5 = this.Velocity.Y * 0.5f;
                                int arg_2456_6 = 100;
                                Color newColor = new Color();
                                int num24 = Dust.NewDust(arg_2456_0, arg_2456_1, arg_2456_2, arg_2456_3, arg_2456_4, arg_2456_5, arg_2456_6, newColor, 1.5f);
                                Main.dust[num24].velocity.X = Main.dust[num24].velocity.X * 0.5f - this.Velocity.X * 0.1f;
                                Main.dust[num24].velocity.Y = Main.dust[num24].velocity.Y * 0.5f - this.Velocity.Y * 0.3f;
                            }
                            if (this.Velocity.Y > -Player.jumpSpeed && this.Velocity.Y != 0f)
                            {
                                this.canRocket = true;
                            }
                            if (this.rocketBoots && this.controlJump && this.rocketDelay == 0 && this.canRocket && this.rocketRelease && !this.jumpAgain)
                            {
                                int num25 = 7;
                                if (this.statMana >= (int)((float)num25 * this.manaCost))
                                {
                                    this.manaRegenDelay = 180;
                                    this.statMana -= (int)((float)num25 * this.manaCost);
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
                                this.rocketFrame = true;
                                for (int num26 = 0; num26 < 2; num26++)
                                {
                                    if (num26 == 0)
                                    {
                                        Vector2 arg_2650_0 = new Vector2(this.Position.X - 4f, this.Position.Y + (float)this.height - 10f);
                                        int arg_2650_1 = 8;
                                        int arg_2650_2 = 8;
                                        int arg_2650_3 = 6;
                                        float arg_2650_4 = 0f;
                                        float arg_2650_5 = 0f;
                                        int arg_2650_6 = 100;
                                        Color newColor = new Color();
                                        int num27 = Dust.NewDust(arg_2650_0, arg_2650_1, arg_2650_2, arg_2650_3, arg_2650_4, arg_2650_5, arg_2650_6, newColor, 2.5f);
                                        Main.dust[num27].noGravity = true;
                                        Main.dust[num27].velocity.X = Main.dust[num27].velocity.X * 1f - 2f - this.Velocity.X * 0.3f;
                                        Main.dust[num27].velocity.Y = Main.dust[num27].velocity.Y * 1f + 2f - this.Velocity.Y * 0.3f;
                                    }
                                    else
                                    {
                                        Vector2 arg_2743_0 = new Vector2(this.Position.X + (float)this.width - 4f, this.Position.Y + (float)this.height - 10f);
                                        int arg_2743_1 = 8;
                                        int arg_2743_2 = 8;
                                        int arg_2743_3 = 6;
                                        float arg_2743_4 = 0f;
                                        float arg_2743_5 = 0f;
                                        int arg_2743_6 = 100;
                                        Color newColor = new Color();
                                        int num28 = Dust.NewDust(arg_2743_0, arg_2743_1, arg_2743_2, arg_2743_3, arg_2743_4, arg_2743_5, arg_2743_6, newColor, 2.5f);
                                        Main.dust[num28].noGravity = true;
                                        Main.dust[num28].velocity.X = Main.dust[num28].velocity.X * 1f + 2f - this.Velocity.X * 0.3f;
                                        Main.dust[num28].velocity.Y = Main.dust[num28].velocity.Y * 1f + 2f - this.Velocity.Y * 0.3f;
                                    }
                                }
                                if (this.rocketDelay == 0)
                                {
                                    this.releaseJump = true;
                                }
                                this.rocketDelay--;
                                this.Velocity.Y = this.Velocity.Y - 0.1f;
                                if (this.Velocity.Y > 0f)
                                {
                                    this.Velocity.Y = this.Velocity.Y - 0.3f;
                                }
                                if (this.Velocity.Y < -Player.jumpSpeed)
                                {
                                    this.Velocity.Y = -Player.jumpSpeed;
                                }
                            }
                            else
                            {
                                this.Velocity.Y = this.Velocity.Y + num2;
                            }
                            if (this.Velocity.Y > num)
                            {
                                this.Velocity.Y = num;
                            }
                        }
                        for (int num29 = 0; num29 < 200; num29++)
                        {
                            if (Main.item[num29].Active && Main.item[num29].NoGrabDelay == 0 && Main.item[num29].Owner == i)
                            {
                                Rectangle rectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.width, this.height);
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
                                            if (Main.netMode == 1)
                                            {
                                                NetMessage.SendData(21, -1, -1, "", num29);
                                            }
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
                                                if (Main.netMode == 1)
                                                {
                                                    NetMessage.SendData(21, -1, -1, "", num29);
                                                }
                                            }
                                            else
                                            {
                                                Main.item[num29] = this.GetItem(i, Main.item[num29]);
                                                if (Main.netMode == 1)
                                                {
                                                    NetMessage.SendData(21, -1, -1, "", num29);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    rectangle = new Rectangle((int)this.Position.X - Player.itemGrabRange, (int)this.Position.Y - Player.itemGrabRange, this.width + Player.itemGrabRange * 2, this.height + Player.itemGrabRange * 2);
                                    if (rectangle.Intersects(new Rectangle((int)Main.item[num29].Position.X, (int)Main.item[num29].Position.Y, Main.item[num29].Width, Main.item[num29].Height)) && this.ItemSpace(Main.item[num29]))
                                    {
                                        Main.item[num29].BeingGrabbed = true;
                                        if ((double)this.Position.X + (double)this.width * 0.5 > (double)Main.item[num29].Position.X + (double)Main.item[num29].Width * 0.5)
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
                                        if ((double)this.Position.Y + (double)this.height * 0.5 > (double)Main.item[num29].Position.Y + (double)Main.item[num29].Height * 0.5)
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
                        if (this.Position.X / 16f - (float)Player.tileRangeX <= (float)Player.tileTargetX && (this.Position.X + (float)this.width) / 16f + (float)Player.tileRangeX - 1f >= (float)Player.tileTargetX && this.Position.Y / 16f - (float)Player.tileRangeY <= (float)Player.tileTargetY && (this.Position.Y + (float)this.height) / 16f + (float)Player.tileRangeY - 2f >= (float)Player.tileTargetY && Main.tile[Player.tileTargetX, Player.tileTargetY].Active)
                        {
                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 79)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 224;
                            }
                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 21)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 48;
                            }
                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 4)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 8;
                            }
                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 13)
                            {
                                this.showItemIcon = true;
                                if (Main.tile[Player.tileTargetX, Player.tileTargetY].frameX == 18)
                                {
                                    this.showItemIcon2 = 28;
                                }
                                else
                                {
                                    if (Main.tile[Player.tileTargetX, Player.tileTargetY].frameX == 36)
                                    {
                                        this.showItemIcon2 = 110;
                                    }
                                    else
                                    {
                                        this.showItemIcon2 = 31;
                                    }
                                }
                            }
                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 29)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 87;
                            }
                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 33)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 105;
                            }
                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 49)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 148;
                            }
                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 50 && Main.tile[Player.tileTargetX, Player.tileTargetY].frameX == 90)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 165;
                            }
                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 55)
                            {
                                int num30 = (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameX / 18);
                                int num31 = (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameY / 18);
                                while (num30 > 1)
                                {
                                    num30 -= 2;
                                }
                                int num32 = Player.tileTargetX - num30;
                                int num33 = Player.tileTargetY - num31;
                                Main.signBubble = true;
                                Main.signX = num32 * 16 + 16;
                                Main.signY = num33 * 16;
                            }
                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 10 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 11)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 25;
                            }
                            if (this.controlUseTile)
                            {
                                if (this.releaseUseTile)
                                {
                                    if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 4 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 13 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 33 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 49 || (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 50 && Main.tile[Player.tileTargetX, Player.tileTargetY].frameX == 90))
                                    {
                                        WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, false, false, false);
                                        if (Main.netMode == 1)
                                        {
                                            NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY);
                                        }
                                    }
                                    else
                                    {
                                        if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 79)
                                        {
                                            int num34 = Player.tileTargetX;
                                            int num35 = Player.tileTargetY;
                                            num34 += (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameX / 18 * -1);
                                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].frameX >= 72)
                                            {
                                                num34 += 4;
                                                num34++;
                                            }
                                            else
                                            {
                                                num34 += 2;
                                            }
                                            num35 += (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameY / 18 * -1);
                                            num35 += 2;
                                            if (Player.CheckSpawn(num34, num35))
                                            {
                                                this.ChangeSpawn(num34, num35);
                                            }
                                        }
                                        else
                                        {
                                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 55)
                                            {
                                                bool flag4 = true;
                                                if (this.sign >= 0)
                                                {
                                                    int num36 = Sign.ReadSign(Player.tileTargetX, Player.tileTargetY);
                                                    if (num36 == this.sign)
                                                    {
                                                        this.sign = -1;
                                                        Main.npcChatText = "";
                                                        Main.editSign = false;
                                                        flag4 = false;
                                                    }
                                                }
                                                if (flag4)
                                                {
                                                    if (Main.netMode == 0)
                                                    {
                                                        this.talkNPC = -1;
                                                        Main.playerInventory = false;
                                                        Main.editSign = false;
                                                        int num37 = Sign.ReadSign(Player.tileTargetX, Player.tileTargetY);
                                                        this.sign = num37;
                                                        Main.npcChatText = Main.sign[num37].text;
                                                    }
                                                    else
                                                    {
                                                        int num38 = (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameX / 18);
                                                        int num39 = (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameY / 18);
                                                        while (num38 > 1)
                                                        {
                                                            num38 -= 2;
                                                        }
                                                        int num40 = Player.tileTargetX - num38;
                                                        int num41 = Player.tileTargetY - num39;
                                                        if (Main.tile[num40, num41].type == 55)
                                                        {
                                                            NetMessage.SendData(46, -1, -1, "", num40, (float)num41);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 10)
                                                {
                                                    WorldGen.OpenDoor(Player.tileTargetX, Player.tileTargetY, this.direction);
                                                    NetMessage.SendData(19, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, (float)this.direction);
                                                }
                                                else
                                                {
                                                    if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 11)
                                                    {
                                                        if (WorldGen.CloseDoor(Player.tileTargetX, Player.tileTargetY, false))
                                                        {
                                                            NetMessage.SendData(19, -1, -1, "", 1, (float)Player.tileTargetX, (float)Player.tileTargetY, (float)this.direction);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if ((Main.tile[Player.tileTargetX, Player.tileTargetY].type == 21 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 29) && this.talkNPC == -1)
                                                        {
                                                            bool flag5 = false;
                                                            int num42 = Player.tileTargetX - (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameX / 18);
                                                            int num43 = Player.tileTargetY - (int)(Main.tile[Player.tileTargetX, Player.tileTargetY].frameY / 18);
                                                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 29)
                                                            {
                                                                flag5 = true;
                                                            }
                                                            if (Main.netMode == 1 && !flag5)
                                                            {
                                                                if (num42 == this.chestX && num43 == this.chestY && this.chest != -1)
                                                                {
                                                                    this.chest = -1;
                                                                }
                                                                else
                                                                {
                                                                    NetMessage.SendData(31, -1, -1, "", num42, (float)num43);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                int num44 = -1;
                                                                if (flag5)
                                                                {
                                                                    num44 = -2;
                                                                }
                                                                else
                                                                {
                                                                    num44 = Chest.FindChest(num42, num43);
                                                                }
                                                                if (num44 != -1)
                                                                {
                                                                    if (num44 == this.chest)
                                                                    {
                                                                        this.chest = -1;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num44 != this.chest && this.chest == -1)
                                                                        {
                                                                            this.chest = num44;
                                                                            Main.playerInventory = true;
                                                                            this.chestX = num42;
                                                                            this.chestY = num43;
                                                                        }
                                                                        else
                                                                        {
                                                                            this.chest = num44;
                                                                            Main.playerInventory = true;
                                                                            this.chestX = num42;
                                                                            this.chestY = num43;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                this.releaseUseTile = false;
                            }
                            else
                            {
                                this.releaseUseTile = true;
                            }
                        }
                        if (Main.myPlayer == this.whoAmi)
                        {
                            if (this.talkNPC >= 0)
                            {
                                Rectangle rectangle2 = new Rectangle((int)(this.Position.X + (float)(this.width / 2) - (float)(Player.tileRangeX * 16)), (int)(this.Position.Y + (float)(this.height / 2) - (float)(Player.tileRangeY * 16)), Player.tileRangeX * 16 * 2, Player.tileRangeY * 16 * 2);
                                Rectangle value = new Rectangle((int)Main.npcs[this.talkNPC].Position.X, (int)Main.npcs[this.talkNPC].Position.Y, Main.npcs[this.talkNPC].width, Main.npcs[this.talkNPC].height);
                                if (!rectangle2.Intersects(value) || this.chest != -1 || !Main.npcs[this.talkNPC].Active)
                                {
                                    this.talkNPC = -1;
                                    Main.npcChatText = "";
                                }
                            }
                            if (this.sign >= 0)
                            {
                                Rectangle rectangle3 = new Rectangle((int)(this.Position.X + (float)(this.width / 2) - (float)(Player.tileRangeX * 16)), (int)(this.Position.Y + (float)(this.height / 2) - (float)(Player.tileRangeY * 16)), Player.tileRangeX * 16 * 2, Player.tileRangeY * 16 * 2);
                                Rectangle value2 = new Rectangle(Main.sign[this.sign].x * 16, Main.sign[this.sign].y * 16, 32, 32);
                                if (!rectangle3.Intersects(value2))
                                {
                                    this.sign = -1;
                                    Main.editSign = false;
                                    Main.npcChatText = "";
                                }
                            }
                            if (Main.editSign)
                            {
                                if (this.sign == -1)
                                {
                                    Main.editSign = false;
                                }
                            }
                            Rectangle rectangle4 = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.width, this.height);
                            for (int num45 = 0; num45 < NPC.MAX_NPCS; num45++)
                            {
                                if (Main.npcs[num45].Active && !Main.npcs[num45].friendly && rectangle4.Intersects(new Rectangle((int)Main.npcs[num45].Position.X, (int)Main.npcs[num45].Position.Y, Main.npcs[num45].width, Main.npcs[num45].height)))
                                {
                                    int hitDirection = -1;
                                    if (Main.npcs[num45].Position.X + (float)(Main.npcs[num45].width / 2) < this.Position.X + (float)(this.width / 2))
                                    {
                                        hitDirection = 1;
                                    }
                                    this.Hurt(Main.npcs[num45].damage, hitDirection, false, false);
                                }
                            }
                            Vector2 vector = Collision.HurtTiles(this.Position, this.Velocity, this.width, this.height, this.fireWalk);
                            if (vector.Y != 0f)
                            {
                                this.Hurt((int)vector.Y, (int)vector.X, false, false);
                            }
                        }
                        if (this.grappling[0] >= 0)
                        {
                            this.rocketDelay = 0;
                            this.rocketFrame = false;
                            this.canRocket = false;
                            this.rocketRelease = false;
                            this.fallStart = (int)(this.Position.Y / 16f);
                            float num46 = 0f;
                            float num47 = 0f;
                            for (int num48 = 0; num48 < this.grapCount; num48++)
                            {
                                num46 += Main.projectile[this.grappling[num48]].Position.X + (float)(Main.projectile[this.grappling[num48]].width / 2);
                                num47 += Main.projectile[this.grappling[num48]].Position.Y + (float)(Main.projectile[this.grappling[num48]].height / 2);
                            }
                            num46 /= (float)this.grapCount;
                            num47 /= (float)this.grapCount;
                            Vector2 vector2 = new Vector2(this.Position.X + (float)this.width * 0.5f, this.Position.Y + (float)this.height * 0.5f);
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
                                        if (Main.projectile[num54].active && Main.projectile[num54].Owner == i && Main.projectile[num54].aiStyle == 7)
                                        {
                                            Main.projectile[num54].Kill();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                this.releaseJump = true;
                            }
                        }
                        if (Collision.StickyTiles(this.Position, this.Velocity, this.width, this.height))
                        {
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
                        bool flag6 = Collision.DrownCollision(this.Position, this.width, this.height);
                        if (selectedItem.Type == 186)
                        {
                            try
                            {
                                int num55 = (int)((this.Position.X + (float)(this.width / 2) + (float)(6 * this.direction)) / 16f);
                                int num56 = (int)((this.Position.Y - 44f) / 16f);
                                if (Main.tile[num55, num56].liquid < 128)
                                {
                                    if (Main.tile[num55, num56] == null)
                                    {
                                        Main.tile[num55, num56] = new Tile();
                                    }
                                    if (!Main.tile[num55, num56].Active || !Main.tileSolid[(int)Main.tile[num55, num56].type] || Main.tileSolidTop[(int)Main.tile[num55, num56].type])
                                    {
                                        flag6 = false;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                        if (Main.myPlayer == i)
                        {
                            if (flag6)
                            {
                                this.breathCD++;
                                int num57 = 7;
                                if (selectedItem.Type == 186)
                                {
                                    num57 *= 2;
                                }
                                if (this.breathCD >= num57)
                                {
                                    this.breathCD = 0;
                                    this.breath--;
                                    if (this.breath <= 0)
                                    {
                                        this.breath = 0;
                                        this.statLife -= 2;
                                        if (this.statLife <= 0)
                                        {
                                            this.statLife = 0;
                                            this.KillMe(10.0, 0, false);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                this.breath += 3;
                                if (this.breath > this.breathMax)
                                {
                                    this.breath = this.breathMax;
                                }
                                this.breathCD = 0;
                            }
                        }
                        if (flag6 && Main.rand.Next(20) == 0)
                        {
                            if (selectedItem.Type == 186)
                            {
                                Vector2 arg_4204_0 = new Vector2(this.Position.X + (float)(10 * this.direction) + 4f, this.Position.Y - 54f);
                                int arg_4204_1 = this.width - 8;
                                int arg_4204_2 = 8;
                                int arg_4204_3 = 34;
                                float arg_4204_4 = 0f;
                                float arg_4204_5 = 0f;
                                int arg_4204_6 = 0;
                                Color newColor = new Color();
                                Dust.NewDust(arg_4204_0, arg_4204_1, arg_4204_2, arg_4204_3, arg_4204_4, arg_4204_5, arg_4204_6, newColor, 1.2f);
                            }
                            else
                            {
                                Vector2 arg_425D_0 = new Vector2(this.Position.X + (float)(12 * this.direction), this.Position.Y + 4f);
                                int arg_425D_1 = this.width - 8;
                                int arg_425D_2 = 8;
                                int arg_425D_3 = 34;
                                float arg_425D_4 = 0f;
                                float arg_425D_5 = 0f;
                                int arg_425D_6 = 0;
                                Color newColor = new Color();
                                Dust.NewDust(arg_425D_0, arg_425D_1, arg_425D_2, arg_425D_3, arg_425D_4, arg_425D_5, arg_425D_6, newColor, 1.2f);
                            }
                        }
                        bool flag7 = Collision.LavaCollision(this.Position, this.width, this.height);
                        if (flag7)
                        {
                            if (Main.myPlayer == i)
                            {
                                this.Hurt(100, 0, false, false);
                            }
                            this.lavaWet = true;
                        }
                        bool flag8 = Collision.WetCollision(this.Position, this.width, this.height);
                        if (flag8)
                        {
                            if (!this.wet)
                            {
                                if (this.wetCount == 0)
                                {
                                    this.wetCount = 10;
                                    if (!flag7)
                                    {
                                        for (int num58 = 0; num58 < 50; num58++)
                                        {
                                            Vector2 arg_4340_0 = new Vector2(this.Position.X - 6f, this.Position.Y + (float)(this.height / 2) - 8f);
                                            int arg_4340_1 = this.width + 12;
                                            int arg_4340_2 = 24;
                                            int arg_4340_3 = 33;
                                            float arg_4340_4 = 0f;
                                            float arg_4340_5 = 0f;
                                            int arg_4340_6 = 0;
                                            Color newColor = new Color();
                                            int num59 = Dust.NewDust(arg_4340_0, arg_4340_1, arg_4340_2, arg_4340_3, arg_4340_4, arg_4340_5, arg_4340_6, newColor, 1f);
                                            Dust expr_4354_cp_0 = Main.dust[num59];
                                            expr_4354_cp_0.velocity.Y = expr_4354_cp_0.velocity.Y - 4f;
                                            Dust expr_4372_cp_0 = Main.dust[num59];
                                            expr_4372_cp_0.velocity.X = expr_4372_cp_0.velocity.X * 2.5f;
                                            Main.dust[num59].scale = 1.3f;
                                            Main.dust[num59].alpha = 100;
                                            Main.dust[num59].noGravity = true;
                                        }
                                    }
                                    else
                                    {
                                        for (int num60 = 0; num60 < 20; num60++)
                                        {
                                            Vector2 arg_4446_0 = new Vector2(this.Position.X - 6f, this.Position.Y + (float)(this.height / 2) - 8f);
                                            int arg_4446_1 = this.width + 12;
                                            int arg_4446_2 = 24;
                                            int arg_4446_3 = 35;
                                            float arg_4446_4 = 0f;
                                            float arg_4446_5 = 0f;
                                            int arg_4446_6 = 0;
                                            Color newColor = new Color();
                                            int num61 = Dust.NewDust(arg_4446_0, arg_4446_1, arg_4446_2, arg_4446_3, arg_4446_4, arg_4446_5, arg_4446_6, newColor, 1f);
                                            Dust expr_445A_cp_0 = Main.dust[num61];
                                            expr_445A_cp_0.velocity.Y = expr_445A_cp_0.velocity.Y - 1.5f;
                                            Dust expr_4478_cp_0 = Main.dust[num61];
                                            expr_4478_cp_0.velocity.X = expr_4478_cp_0.velocity.X * 2.5f;
                                            Main.dust[num61].scale = 1.3f;
                                            Main.dust[num61].alpha = 100;
                                            Main.dust[num61].noGravity = true;
                                        }
                                    }
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
                                    if (!this.lavaWet)
                                    {
                                        for (int num62 = 0; num62 < 50; num62++)
                                        {
                                            Vector2 arg_4599_0 = new Vector2(this.Position.X - 6f, this.Position.Y + (float)(this.height / 2));
                                            int arg_4599_1 = this.width + 12;
                                            int arg_4599_2 = 24;
                                            int arg_4599_3 = 33;
                                            float arg_4599_4 = 0f;
                                            float arg_4599_5 = 0f;
                                            int arg_4599_6 = 0;
                                            Color newColor = new Color();
                                            int num63 = Dust.NewDust(arg_4599_0, arg_4599_1, arg_4599_2, arg_4599_3, arg_4599_4, arg_4599_5, arg_4599_6, newColor, 1f);
                                            Dust expr_45AD_cp_0 = Main.dust[num63];
                                            expr_45AD_cp_0.velocity.Y = expr_45AD_cp_0.velocity.Y - 4f;
                                            Dust expr_45CB_cp_0 = Main.dust[num63];
                                            expr_45CB_cp_0.velocity.X = expr_45CB_cp_0.velocity.X * 2.5f;
                                            Main.dust[num63].scale = 1.3f;
                                            Main.dust[num63].alpha = 100;
                                            Main.dust[num63].noGravity = true;
                                        }
                                    }
                                    else
                                    {
                                        for (int num64 = 0; num64 < 20; num64++)
                                        {
                                            Vector2 arg_469F_0 = new Vector2(this.Position.X - 6f, this.Position.Y + (float)(this.height / 2) - 8f);
                                            int arg_469F_1 = this.width + 12;
                                            int arg_469F_2 = 24;
                                            int arg_469F_3 = 35;
                                            float arg_469F_4 = 0f;
                                            float arg_469F_5 = 0f;
                                            int arg_469F_6 = 0;
                                            Color newColor = new Color();
                                            int num65 = Dust.NewDust(arg_469F_0, arg_469F_1, arg_469F_2, arg_469F_3, arg_469F_4, arg_469F_5, arg_469F_6, newColor, 1f);
                                            Dust expr_46B3_cp_0 = Main.dust[num65];
                                            expr_46B3_cp_0.velocity.Y = expr_46B3_cp_0.velocity.Y - 1.5f;
                                            Dust expr_46D1_cp_0 = Main.dust[num65];
                                            expr_46D1_cp_0.velocity.X = expr_46D1_cp_0.velocity.X * 2.5f;
                                            Main.dust[num65].scale = 1.3f;
                                            Main.dust[num65].alpha = 100;
                                            Main.dust[num65].noGravity = true;
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
                        if (this.wet)
                        {
                            if (this.wet)
                            {
                                Vector2 vector3 = this.Velocity;
                                this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.width, this.height, this.controlDown, false);
                                Vector2 value3 = this.Velocity * 0.5f;
                                if (this.Velocity.X != vector3.X)
                                {
                                    value3.X = this.Velocity.X;
                                }
                                if (this.Velocity.Y != vector3.Y)
                                {
                                    value3.Y = this.Velocity.Y;
                                }
                                this.Position += value3;
                            }
                        }
                        else
                        {
                            this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.width, this.height, this.controlDown, false);
                            this.Position += this.Velocity;
                        }
                        if (this.Position.X < Main.leftWorld + 336f + 16f)
                        {
                            this.Position.X = Main.leftWorld + 336f + 16f;
                            this.Velocity.X = 0f;
                        }
                        if (this.Position.X + (float)this.width > Main.rightWorld - 336f - 32f)
                        {
                            this.Position.X = Main.rightWorld - 336f - 32f - (float)this.width;
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
                        if (this.Position.Y > Main.bottomWorld - 336f - 32f - (float)this.height)
                        {
                            this.Position.Y = Main.bottomWorld - 336f - 32f - (float)this.height;
                            this.Velocity.Y = 0f;
                        }
                        this.ItemCheck(i);
                        this.PlayerFrame();
                        if (this.statLife > this.statLifeMax)
                        {
                            this.statLife = this.statLifeMax;
                        }
                        this.grappling[0] = -1;
                        this.grapCount = 0;
                    }
                }
            }
            catch (Exception e)
            {
                Program.tConsole.WriteLine("Error In UpdatePlayer: " + e.Message);
                Program.tConsole.WriteLine("Stack: " + e.StackTrace);
            }
		}
		
        public bool SellItem(int price)
		{
			if (price <= 0)
			{
				return false;
			}
			Item[] array = new Item[44];
			for (int i = 0; i < 44; i++)
			{
				array[i] = new Item();
				array[i] = (Item)this.inventory[i].Clone();
			}
			int j = price / 5;
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
						this.inventory[num].SetDefaults(74);
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
						this.inventory[num2].SetDefaults(73);
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
						this.inventory[num3].SetDefaults(72);
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
						this.inventory[num4].SetDefaults(71);
						j--;
					}
				}
			}
			if (flag)
			{
				for (int num5 = 0; num5 < 44; num5++)
				{
					this.inventory[num5] = (Item)array[num5].Clone();
				}
				return false;
			}
			return true;
		}
		
        public bool BuyItem(int price)
		{
			if (price == 0)
			{
				return false;
			}
			int num = 0;
			int i = price;
			Item[] array = new Item[44];
			for (int j = 0; j < 44; j++)
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
						for (int k = 0; k < 44; k++)
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
						for (int l = 0; l < 44; l++)
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
						for (int m = 0; m < 44; m++)
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
						for (int n = 0; n < 44; n++)
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
							for (int num4 = 0; num4 < 44; num4++)
							{
								this.inventory[num4] = (Item)array[num4].Clone();
							}
							return false;
						}
						bool flag = true;
						if (i >= 10000)
						{
							for (int num5 = 0; num5 < 44; num5++)
							{
								if (this.inventory[num5].Type == 74 && this.inventory[num5].Stack >= 1)
								{
									this.inventory[num5].Stack--;
									if (this.inventory[num5].Stack == 0)
									{
										this.inventory[num5].Type = 0;
									}
									this.inventory[num2].SetDefaults(73);
									this.inventory[num2].Stack = 100;
									flag = false;
									break;
								}
							}
						}
						else
						{
							if (i >= 100)
							{
								for (int num6 = 0; num6 < 44; num6++)
								{
									if (this.inventory[num6].Type == 73 && this.inventory[num6].Stack >= 1)
									{
										this.inventory[num6].Stack--;
										if (this.inventory[num6].Stack == 0)
										{
											this.inventory[num6].Type = 0;
										}
										this.inventory[num2].SetDefaults(72);
										this.inventory[num2].Stack = 100;
										flag = false;
										break;
									}
								}
							}
							else
							{
								if (i >= 1)
								{
									for (int num7 = 0; num7 < 44; num7++)
									{
										if (this.inventory[num7].Type == 72 && this.inventory[num7].Stack >= 1)
										{
											this.inventory[num7].Stack--;
											if (this.inventory[num7].Stack == 0)
											{
												this.inventory[num7].Type = 0;
											}
											this.inventory[num2].SetDefaults(71);
											this.inventory[num2].Stack = 100;
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
								for (int num8 = 0; num8 < 44; num8++)
								{
									if (this.inventory[num8].Type == 73 && this.inventory[num8].Stack >= 1)
									{
										this.inventory[num8].Stack--;
										if (this.inventory[num8].Stack == 0)
										{
											this.inventory[num8].Type = 0;
										}
										this.inventory[num2].SetDefaults(72);
										this.inventory[num2].Stack = 100;
										flag = false;
										break;
									}
								}
							}
							if (flag && i < 1000000)
							{
								for (int num9 = 0; num9 < 44; num9++)
								{
									if (this.inventory[num9].Type == 74 && this.inventory[num9].Stack >= 1)
									{
										this.inventory[num9].Stack--;
										if (this.inventory[num9].Stack == 0)
										{
											this.inventory[num9].Type = 0;
										}
										this.inventory[num2].SetDefaults(73);
										this.inventory[num2].Stack = 100;
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
		
        public void AdjTiles()
		{
			int num = 4;
			int num2 = 3;
			for (int i = 0; i < 80; i++)
			{
				this.oldAdjTile[i] = this.adjTile[i];
				this.adjTile[i] = false;
			}
			int num3 = (int)((this.Position.X + (float)(this.width / 2)) / 16f);
			int num4 = (int)((this.Position.Y + (float)this.height) / 16f);
			for (int j = num3 - num; j <= num3 + num; j++)
			{
				for (int k = num4 - num2; k < num4 + num2; k++)
				{
					if (Main.tile[j, k].Active)
					{
						this.adjTile[(int)Main.tile[j, k].type] = true;
						if (Main.tile[j, k].type == 77)
						{
							this.adjTile[17] = true;
						}
					}
				}
			}
			if (Main.playerInventory)
			{
				bool flag = false;
				for (int l = 0; l < 80; l++)
				{
					if (this.oldAdjTile[l] != this.adjTile[l])
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					Recipe.FindRecipes();
				}
			}
		}
		
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
									float num = this.itemRotation * (float)this.direction;
									this.bodyFrame.Y = this.bodyFrame.Height * 3;
									if ((double)num < -0.75)
									{
										this.bodyFrame.Y = this.bodyFrame.Height * 2;
									}
									if ((double)num > 0.6)
									{
										this.bodyFrame.Y = this.bodyFrame.Height * 4;
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
							Vector2 vector = new Vector2(this.Position.X + (float)this.width * 0.5f, this.Position.Y + (float)this.height * 0.5f);
							float num2 = 0f;
							float num3 = 0f;
							for (int i = 0; i < this.grapCount; i++)
							{
								num2 += Main.projectile[this.grappling[i]].Position.X + (float)(Main.projectile[this.grappling[i]].width / 2);
								num3 += Main.projectile[this.grappling[i]].Position.Y + (float)(Main.projectile[this.grappling[i]].height / 2);
							}
							num2 /= (float)this.grapCount;
							num3 /= (float)this.grapCount;
							num2 -= vector.X;
							num3 -= vector.Y;
							if (num3 < 0f && Math.Abs(num3) > Math.Abs(num2))
							{
								this.bodyFrame.Y = this.bodyFrame.Height * 2;
							}
							else
							{
								if (num3 > 0f && Math.Abs(num3) > Math.Abs(num2))
								{
									this.bodyFrame.Y = this.bodyFrame.Height * 4;
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
		
        public void Spawn()
		{
			if (this.whoAmi == Main.myPlayer)
			{
				this.FindSpawn();
				if (!Player.CheckSpawn(this.SpawnX, this.SpawnY))
				{
					this.SpawnX = -1;
					this.SpawnY = -1;
				}
			}
			if (Main.netMode == 1 && this.whoAmi == Main.myPlayer)
			{
				NetMessage.SendData(12, -1, -1, "", Main.myPlayer);
				Main.gameMenu = false;
			}
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
					this.statMana = this.statManaMax;
				}
			}
			this.immune = true;
			this.dead = false;
			this.immuneTime = 0;
			this.Active = true;
			if (this.SpawnX >= 0 && this.SpawnY >= 0)
			{
				this.Position.X = (float)(this.SpawnX * 16 + 8 - this.width / 2);
				this.Position.Y = (float)(this.SpawnY * 16 - this.height);
			}
			else
			{
				this.Position.X = (float)(Main.spawnTileX * 16 + 8 - this.width / 2);
				this.Position.Y = (float)(Main.spawnTileY * 16 - this.height);
				for (int i = Main.spawnTileX - 1; i < Main.spawnTileX + 2; i++)
				{
					for (int j = Main.spawnTileY - 3; j < Main.spawnTileY; j++)
					{
						if (Main.tileSolid[(int)Main.tile[i, j].type] && !Main.tileSolidTop[(int)Main.tile[i, j].type])
						{
							if (Main.tile[i, j].liquid > 0)
							{
								Main.tile[i, j].lava = false;
								Main.tile[i, j].liquid = 0;
								WorldGen.SquareTileFrame(i, j, true);
							}
							WorldGen.KillTile(i, j, false, false, false);
						}
					}
				}
			}
			this.wet = Collision.WetCollision(this.Position, this.width, this.height);
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
				Main.screenPosition.X = this.Position.X + (float)(this.width / 2) - (float)(Main.screenWidth / 2);
				Main.screenPosition.Y = this.Position.Y + (float)(this.height / 2) - (float)(Main.screenHeight / 2);
			}
		}

        public double Hurt(int Damage, int hitDirection, bool pvp = false, bool quiet = false, String deathText = " was slain...")
        {
            if (!this.immune)
            {
                int num = Damage;

                PlayerHurtEvent playerEvent = new PlayerHurtEvent();
                playerEvent.Sender = this;
                playerEvent.Damage = Damage;
                Program.server.getPluginManager().processHook(Hooks.PLAYER_HURT, playerEvent);
                if (playerEvent.Cancelled)
                {
                    return 0.0;
                }

                num = playerEvent.Damage;

                if (pvp)
                {
                    num *= 2;
                }
                double num2 = Main.CalculateDamage(num, this.statDefense);
                if (num2 >= 1.0)
                {
                    if (Main.netMode == 1 && this.whoAmi == Main.myPlayer && !quiet)
                    {
                        int num3 = 0;
                        if (pvp)
                        {
                            num3 = 1;
                        }
                        NetMessage.SendData(13, -1, -1, "", this.whoAmi);
                        NetMessage.SendData(16, -1, -1, "", this.whoAmi);
                        NetMessage.SendData(26, -1, -1, "", this.whoAmi, (float)hitDirection, (float)Damage, (float)num3);
                    }
                    this.statLife -= (int)num2;
                    this.immune = true;
                    this.immuneTime = 40;
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
                            if (this.boneArmor)
                            {
                                Dust.NewDust(this.Position, this.width, this.height, 26, (float)(2 * hitDirection), -2f, 0, default(Color), 1f);
                            }
                            else
                            {
                                Dust.NewDust(this.Position, this.width, this.height, 5, (float)(2 * hitDirection), -2f, 0, default(Color), 1f);
                            }
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

        public void DropCoins()
        {
            for (int i = 0; i < 44; i++)
            {
                if (this.inventory[i].Type >= 71 && this.inventory[i].Type <= 74)
                {
                    int num = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, this.inventory[i].Type, 1, false);
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
                    if (Main.netMode == 1)
                    {
                        NetMessage.SendData(21, -1, -1, "", num);
                    }
                }
            }
        }

        public void KillMe(double dmg, int hitDirection, bool pvp = false, String deathText = " was slain...")
        {
            if ((Main.godMode && Main.myPlayer == this.whoAmi) || this.godMode)
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

            if (this.hardCore)
            {
                if (Main.netMode != 1)
                {
                    float num = (float)Main.rand.Next(-35, 36) * 0.1f;
                    while (num < 2f && num > -2f)
                    {
                        num += (float)Main.rand.Next(-30, 31) * 0.1f;
                    }
                    int num2 = Projectile.NewProjectile(this.Position.X + (float)(this.width / 2), this.Position.Y + (float)(this.head / 2), (float)Main.rand.Next(10, 30) * 0.1f * (float)hitDirection + num, (float)Main.rand.Next(-40, -20) * 0.1f, ProjectileType.TOMBSTONE, this.statLifeMax + this.statManaMax, 0f, Main.myPlayer);
                    Main.projectile[num2].miscText = this.Name + deathText;
                }
                if (Main.myPlayer == this.whoAmi)
                {
                    this.statLifeMax = 100;
                    this.statManaMax = 0;
                    this.DropItems();
                }
            }
            this.headVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            this.bodyVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            this.legVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            this.headVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
            this.bodyVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
            this.legVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
            int num3 = 0;
            while ((double)num3 < 20.0 + dmg / (double)this.statLifeMax * 100.0)
            {
                if (this.boneArmor)
                {
                    Dust.NewDust(this.Position, this.width, this.height, 26, (float)(2 * hitDirection), -2f, 0, default(Color), 1f);
                }
                else
                {
                    Dust.NewDust(this.Position, this.width, this.height, 5, (float)(2 * hitDirection), -2f, 0, default(Color), 1f);
                }
                num3++;
            }
            this.dead = true;
            this.respawnTimer = 600;
            this.immuneAlpha = 0;
            if (Main.netMode == 2)
            {
                NetMessage.SendData(25, -1, -1, this.Name + deathText, 255, 225f, 25f, 25f);
            }

            if (Main.netMode == 1 && this.whoAmi == Main.myPlayer)
            {
                int num4 = 0;
                if (pvp)
                {
                    num4 = 1;
                }
                NetMessage.SendData(44, -1, -1, deathText, this.whoAmi, (float)hitDirection, (float)((int)dmg), (float)num4);
            }
            if (!pvp && this.whoAmi == Main.myPlayer && !this.hardCore)
            {
                this.DropCoins();
            }
            if (this.whoAmi == Main.myPlayer)
            {
                try
                {
                    WorldGen.saveToonWhilePlaying();
                }
                catch
                {
                }
            }
        }

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
		
        public void DoCoins(int i)
		{
			if (this.inventory[i].Stack == 100 && (this.inventory[i].Type == 71 || this.inventory[i].Type == 72 || this.inventory[i].Type == 73))
			{
				this.inventory[i].SetDefaults(this.inventory[i].Type + 1);
				for (int j = 0; j < 44; j++)
				{
					if (this.inventory[j].IsTheSameAs(this.inventory[i]) && j != i && this.inventory[j].Stack < this.inventory[j].MaxStack)
					{
						this.inventory[j].Stack++;
						this.inventory[i].SetDefaults("");
						this.inventory[i].Active = false;
						this.inventory[i].Name = "";
						this.inventory[i].Type = 0;
						this.inventory[i].Stack = 0;
						this.DoCoins(j);
					}
				}
			}
		}
		
        public Item GetItem(int plr, Item newItem)
		{
			if (newItem.NoGrabDelay > 0)
			{
				return newItem;
			}
			int num = 0;
			if (newItem.Type == 71 || newItem.Type == 72 || newItem.Type == 73 || newItem.Type == 74)
			{
				num = -4;
			}
			for (int i = num; i < 40; i++)
			{
				int num2 = i;
				if (num2 < 0)
				{
					num2 = 44 + i;
				}
				if (this.inventory[num2].Type > 0 && this.inventory[num2].Stack < this.inventory[num2].MaxStack && newItem.IsTheSameAs(this.inventory[num2]))
				{
					if (newItem.Stack + this.inventory[num2].Stack <= this.inventory[num2].MaxStack)
					{
						this.inventory[num2].Stack += newItem.Stack;
						this.DoCoins(num2);
						if (plr == Main.myPlayer)
						{
							Recipe.FindRecipes();
						}
						return new Item();
					}
					newItem.Stack -= this.inventory[num2].MaxStack - this.inventory[num2].Stack;
					this.inventory[num2].Stack = this.inventory[num2].MaxStack;
					this.DoCoins(num2);
					if (plr == Main.myPlayer)
					{
						Recipe.FindRecipes();
					}
				}
			}
			for (int j = num; j < 40; j++)
			{
				int num3 = j;
				if (num3 < 0)
				{
					num3 = 44 + j;
				}
				if (this.inventory[num3].Type == 0)
				{
					this.inventory[num3] = newItem;
					this.DoCoins(num3);
					if (plr == Main.myPlayer)
					{
						Recipe.FindRecipes();
					}
					return new Item();
				}
			}
			return newItem;
		}

        public void ItemCheck(int i)
        {
            Item selectedItem = inventory[selectedItemIndex];
            if (selectedItem.AutoReuse)
            {
                releaseUseItem = true;
                if (itemAnimation == 1 && selectedItem.Stack > 0)
                {
                    itemAnimation = 0;
                }
            }

            if (controlUseItem && itemAnimation == 0 && releaseUseItem && selectedItem.UseStyle > 0)
            {
                bool flag = true;
                if (selectedItem.Shoot == ProjectileType.BOOMERANG_ENCHANTED || selectedItem.Shoot == ProjectileType.FLAMARANG || selectedItem.Shoot == ProjectileType.CHAKRUM_THORN)
                {
                    for (int j = 0; j < Main.maxProjectiles; j++)
                    {
                        if (Main.projectile[j].active && Main.projectile[j].Owner == Main.myPlayer && Main.projectile[j].type == selectedItem.Shoot)
                        {
                            flag = false;
                        }
                    }
                }

                if (selectedItem.Potion)
                {
                    if (this.potionDelay <= 0)
                    {
                        potionDelay = Item.potionDelay;
                    }
                    else
                    {
                        flag = false;
                    }
                }

                if (selectedItem.Mana > 0)
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
                        for (int j = 0; j < Main.maxInventory; j++)
                        {
                            if (inventory[j].Ammo == selectedItem.UseAmmo && inventory[j].Stack > 0)
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
                    if (selectedItem.Shoot > 0 || selectedItem.Damage == 0)
                    {
                        meleeSpeed = 1f;
                    }
                    itemAnimation = (int)((float)selectedItem.UseAnimation * meleeSpeed);
                    itemAnimationMax = (int)((float)selectedItem.UseAnimation * meleeSpeed);
                }

                if (flag && selectedItem.Shoot == ProjectileType.ORB_OF_LIGHT)
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        if (Main.projectile[j].active && Main.projectile[j].Owner == i && Main.projectile[j].type == selectedItem.Shoot)
                        {
                            Main.projectile[j].Kill();
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
                    this.manaRegenDelay = 180;
                }

                if (Main.dedServ)
                {
                    itemHeight = selectedItem.Height;
                    itemWidth = selectedItem.Width;
                }
                itemAnimation--;
            }
            else if (selectedItem.HoldStyle == 1)
            {
                if (Main.dedServ)
                {
                    itemLocation.X = Position.X + (float)width * 0.5f + 20f * (float)direction;
                }
                itemLocation.Y = Position.Y + 24f;
                itemRotation = 0f;
            }
            else if (selectedItem.HoldStyle == 2)
            {
                itemLocation.X = Position.X + (float)width * 0.5f + (float)(6 * direction);
                itemLocation.Y = Position.Y + 16f;
                itemRotation = 0.79f * (float)(-(float)direction);
            }

            if (selectedItem.Type == 8)
            {
                int maxValue = 20;
                if (itemAnimation > 0)
                {
                    maxValue = 7;
                }

                if (direction == -1)
                {
                    if (Main.rand.Next(maxValue) == 0)
                    {
                        Vector2 arg_1182_0 = new Vector2(itemLocation.X - 16f, itemLocation.Y - 14f);
                        int arg_1182_1 = 4;
                        int arg_1182_2 = 4;
                        int arg_1182_3 = 6;
                        float arg_1182_4 = 0f;
                        float arg_1182_5 = 0f;
                        int arg_1182_6 = 100;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_1182_0, arg_1182_1, arg_1182_2, arg_1182_3, arg_1182_4, arg_1182_5, arg_1182_6, newColor, 1f);
                    }
                }
                else if (Main.rand.Next(maxValue) == 0)
                {
                    Vector2 arg_1235_0 = new Vector2(itemLocation.X + 6f, itemLocation.Y - 14f);
                    int arg_1235_1 = 4;
                    int arg_1235_2 = 4;
                    int arg_1235_3 = 6;
                    float arg_1235_4 = 0f;
                    float arg_1235_5 = 0f;
                    int arg_1235_6 = 100;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_1235_0, arg_1235_1, arg_1235_2, arg_1235_3, arg_1235_4, arg_1235_5, arg_1235_6, newColor, 1f);
                }
            }
            else
            {
                if (selectedItem.Type == 105)
                {
                    int maxValue = 20;
                    if (itemAnimation > 0)
                    {
                        maxValue = 7;
                    }
                    if (direction == -1)
                    {
                        if (Main.rand.Next(maxValue) == 0)
                        {
                            Vector2 arg_133C_0 = new Vector2(itemLocation.X - 12f, itemLocation.Y - 20f);
                            int arg_133C_1 = 4;
                            int arg_133C_2 = 4;
                            int arg_133C_3 = 6;
                            float arg_133C_4 = 0f;
                            float arg_133C_5 = 0f;
                            int arg_133C_6 = 100;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_133C_0, arg_133C_1, arg_133C_2, arg_133C_3, arg_133C_4, arg_133C_5, arg_133C_6, newColor, 1f);
                        }
                    }
                    else if (Main.rand.Next(maxValue) == 0)
                    {
                        Vector2 arg_13EF_0 = new Vector2(itemLocation.X + 4f, itemLocation.Y - 20f);
                        int arg_13EF_1 = 4;
                        int arg_13EF_2 = 4;
                        int arg_13EF_3 = 6;
                        float arg_13EF_4 = 0f;
                        float arg_13EF_5 = 0f;
                        int arg_13EF_6 = 100;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_13EF_0, arg_13EF_1, arg_13EF_2, arg_13EF_3, arg_13EF_4, arg_13EF_5, arg_13EF_6, newColor, 1f);
                    }
                }
                else if (selectedItem.Type == 148)
                {
                    int maxValue = 10;
                    if (this.itemAnimation > 0)
                    {
                        maxValue = 7;
                    }

                    if (this.direction == -1)
                    {
                        if (Main.rand.Next(maxValue) == 0)
                        {
                            Vector2 arg_14FA_0 = new Vector2(itemLocation.X - 12f, itemLocation.Y - 20f);
                            int arg_14FA_1 = 4;
                            int arg_14FA_2 = 4;
                            int arg_14FA_3 = 29;
                            float arg_14FA_4 = 0f;
                            float arg_14FA_5 = 0f;
                            int arg_14FA_6 = 100;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_14FA_0, arg_14FA_1, arg_14FA_2, arg_14FA_3, arg_14FA_4, arg_14FA_5, arg_14FA_6, newColor, 1f);
                        }
                    }
                    else if (Main.rand.Next(maxValue) == 0)
                    {
                        Vector2 arg_15AE_0 = new Vector2(itemLocation.X + 4f, itemLocation.Y - 20f);
                        int arg_15AE_1 = 4;
                        int arg_15AE_2 = 4;
                        int arg_15AE_3 = 29;
                        float arg_15AE_4 = 0f;
                        float arg_15AE_5 = 0f;
                        int arg_15AE_6 = 100;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_15AE_0, arg_15AE_1, arg_15AE_2, arg_15AE_3, arg_15AE_4, arg_15AE_5, arg_15AE_6, newColor, 1f);
                    }
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
                    bool flag2 = false;
                    int damage = selectedItem.Damage;
                    float knockBack = selectedItem.KnockBack;
                    if (shoot == ProjectileType.HOOK || shoot == ProjectileType.WHIP_IVY)
                    {
                        grappling[0] = -1;
                        grapCount = 0;
                        for (int j = 0; j < Main.maxProjectiles; j++)
                        {
                            if (Main.projectile[j].active && Main.projectile[j].Owner == i)
                            {
                                if (Main.projectile[j].type == ProjectileType.HOOK)
                                {
                                    Main.projectile[j].Kill();
                                }
                            }
                        }
                    }

                    if (selectedItem.UseAmmo > 0)
                    {
                        for (int j = 0; j < Main.maxInventory; j++)
                        {
                            Item inventoryItem = inventory[j];
                            if (inventoryItem.Ammo == selectedItem.UseAmmo && inventoryItem.Stack > 0)
                            {
                                if (inventory[j].Shoot > 0)
                                {
                                    shoot = inventoryItem.Shoot;
                                }

                                shootSpeed += inventoryItem.ShootSpeed;
                                damage += inventoryItem.Damage;
                                knockBack += inventoryItem.KnockBack;
                                inventoryItem.Stack--;
                                if (inventoryItem.Stack <= 0)
                                {
                                    inventoryItem.Active = false;
                                    inventoryItem.Name = "";
                                    inventoryItem.Type = 0;
                                }
                                flag2 = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        flag2 = true;
                    }

                    if (shoot == ProjectileType.STARFURY && (double)Position.Y > Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2))
                    {
                        flag2 = false;
                    }

                    if (flag2)
                    {
                        if (selectedItem.Mana > 0)
                        {
                            damage = (int)Math.Round((double)((float)damage * this.magicBoost));
                        }

                        if (shoot == ProjectileType.ARROW_WOODEN && selectedItem.Type == 120)
                        {
                            shoot = ProjectileType.ARROW_FIRE;
                        }

                        itemTime = selectedItem.UseTime;
                        direction = -1;
                        Vector2 vector = new Vector2(Position.X + (float)width * 0.5f, Position.Y + (float)height * 0.5f);
                        if (shoot == ProjectileType.STARFURY)
                        {
                            vector = new Vector2(Position.X + (float)width * 0.5f + (float)(Main.rand.Next(601) * -(float)direction), Position.Y + (float)height * 0.5f - 300f - (float)Main.rand.Next(100));
                            knockBack = 0f;
                        }
                    }
                    else if (selectedItem.UseStyle == 5)
                    {
                        itemRotation = 0f;
                        NetMessage.SendData(41, -1, -1, "", this.whoAmi);
                    }
                }

                if (selectedItem.Type >= 205 && selectedItem.Type <= 207)
                {
                    if (Position.X / 16f - (float)Player.tileRangeX - (float)selectedItem.TileBoost <= (float)Player.tileTargetX
                        && (Position.X + (float)width) / 16f + (float)Player.tileRangeX + (float)selectedItem.TileBoost - 1f >= (float)Player.tileTargetX
                        && Position.Y / 16f - (float)Player.tileRangeY - (float)selectedItem.TileBoost <= (float)Player.tileTargetY
                        && (this.Position.Y + (float)this.height) / 16f + (float)Player.tileRangeY + (float)selectedItem.TileBoost - 2f >= (float)Player.tileTargetY)
                    {
                        showItemIcon = true;

                        if (itemTime == 0 && itemAnimation > 0 && controlUseItem)
                        {
                            if (selectedItem.Type == 205)
                            {
                                bool lava = Main.tile[Player.tileTargetX, Player.tileTargetY].lava;
                                int num10 = 0;
                                for (int x = Player.tileTargetX - 1; x <= Player.tileTargetX + 1; x++)
                                {
                                    for (int y = Player.tileTargetY - 1; y <= Player.tileTargetY + 1; y++)
                                    {
                                        if (Main.tile[x, y].lava == lava)
                                        {
                                            num10 += (int)Main.tile[x, y].liquid;
                                        }
                                    }
                                }

                                if (Main.tile[Player.tileTargetX, Player.tileTargetY].liquid > 0 && num10 > 100)
                                {
                                    bool lava2 = Main.tile[Player.tileTargetX, Player.tileTargetY].lava;
                                    if (!Main.tile[Player.tileTargetX, Player.tileTargetY].lava)
                                    {
                                        selectedItem.SetDefaults(206);
                                    }
                                    else
                                    {
                                        selectedItem.SetDefaults(207);
                                    }

                                    itemTime = selectedItem.UseTime;
                                    int num11 = (int)Main.tile[Player.tileTargetX, Player.tileTargetY].liquid;
                                    Main.tile[Player.tileTargetX, Player.tileTargetY].liquid = 0;
                                    Main.tile[Player.tileTargetX, Player.tileTargetY].lava = false;
                                    WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY, false);

                                    if (Main.netMode == 1)
                                    {
                                        NetMessage.sendWater(Player.tileTargetX, Player.tileTargetY);
                                    }
                                    else
                                    {
                                        Liquid.AddWater(Player.tileTargetX, Player.tileTargetY);
                                    }

                                    for (int x = Player.tileTargetX - 1; x <= Player.tileTargetX + 1; x++)
                                    {
                                        for (int y = Player.tileTargetY - 1; y <= Player.tileTargetY + 1; y++)
                                        {
                                            if (num11 < 256 && Main.tile[x, y].lava == lava)
                                            {
                                                int num12 = (int)Main.tile[x, y].liquid;

                                                if (num12 + num11 > 255)
                                                {
                                                    num12 = 255 - num11;
                                                }

                                                num11 += num12;
                                                Tile expr_20A0 = Main.tile[x, y];
                                                expr_20A0.liquid -= (byte)num12;
                                                Main.tile[x, y].lava = lava2;

                                                if (Main.tile[x, y].liquid == 0)
                                                {
                                                    Main.tile[x, y].lava = false;
                                                }

                                                WorldGen.SquareTileFrame(x, y, false);

                                                if (Main.netMode == 1)
                                                {
                                                    NetMessage.sendWater(x, y);
                                                }
                                                else
                                                {
                                                    Liquid.AddWater(x, y);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (Main.tile[Player.tileTargetX, Player.tileTargetY].liquid < 200)
                            {
                                if (!Main.tile[Player.tileTargetX, Player.tileTargetY].Active || !Main.tileSolid[(int)Main.tile[Player.tileTargetX, Player.tileTargetY].type] || !Main.tileSolidTop[(int)Main.tile[Player.tileTargetX, Player.tileTargetY].type])
                                {
                                    if (selectedItem.Type == 207)
                                    {
                                        if (Main.tile[Player.tileTargetX, Player.tileTargetY].liquid == 0 || Main.tile[Player.tileTargetX, Player.tileTargetY].lava)
                                        {
                                            Main.tile[Player.tileTargetX, Player.tileTargetY].lava = true;
                                            Main.tile[Player.tileTargetX, Player.tileTargetY].liquid = 255;
                                            WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY, true);
                                            selectedItem.SetDefaults(205);
                                            this.itemTime = selectedItem.UseTime;
                                            if (Main.netMode == 1)
                                            {
                                                NetMessage.sendWater(Player.tileTargetX, Player.tileTargetY);
                                            }
                                        }
                                    }
                                    else if (Main.tile[Player.tileTargetX, Player.tileTargetY].liquid == 0 || !Main.tile[Player.tileTargetX, Player.tileTargetY].lava)
                                    {
                                        Main.tile[Player.tileTargetX, Player.tileTargetY].lava = false;
                                        Main.tile[Player.tileTargetX, Player.tileTargetY].liquid = 255;
                                        WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY, true);
                                        selectedItem.SetDefaults(205);
                                        this.itemTime = selectedItem.UseTime;

                                        if (Main.netMode == 1)
                                        {
                                            NetMessage.sendWater(Player.tileTargetX, Player.tileTargetY);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (selectedItem.Pick > 0 || selectedItem.Axe > 0 || selectedItem.Hammer > 0)
                {
                    if (Position.X / 16f - (float)Player.tileRangeX - (float)selectedItem.TileBoost <= (float)Player.tileTargetX 
                        && (Position.X + (float)width) / 16f + (float)Player.tileRangeX + (float)selectedItem.TileBoost - 1f >= (float)Player.tileTargetX 
                        && Position.Y / 16f - (float)Player.tileRangeY - (float)selectedItem.TileBoost <= (float)Player.tileTargetY 
                        && (Position.Y + (float)height) / 16f + (float)Player.tileRangeY + (float)selectedItem.TileBoost - 2f >= (float)Player.tileTargetY)
                    {
                        showItemIcon = true;
                        if (Main.tile[Player.tileTargetX, Player.tileTargetY].Active)
                        {
                            if (itemTime == 0 && itemAnimation > 0 && controlUseItem)
                            {
                                if (hitTileX != Player.tileTargetX || hitTileY != Player.tileTargetY)
                                {
                                    hitTile = 0;
                                    hitTileX = Player.tileTargetX;
                                    hitTileY = Player.tileTargetY;
                                }
                                if (Main.tileNoFail[(int)Main.tile[Player.tileTargetX, Player.tileTargetY].type])
                                {
                                    hitTile = 100;
                                }
                                if (Main.tile[Player.tileTargetX, Player.tileTargetY].type != 27)
                                {
                                    if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 4 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 10 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 11 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 12 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 13 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 14 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 15 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 16 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 17 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 18 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 19 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 21 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 26 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 28 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 29 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 31 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 33 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 34 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 35 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 36 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 42 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 48 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 49 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 50 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 54 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 55 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 77 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 78 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 79)
                                    {
                                        if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 48)
                                        {
                                            hitTile += selectedItem.Hammer / 3;
                                        }
                                        else
                                        {
                                            hitTile += selectedItem.Hammer;
                                        }

                                        if ((double)Player.tileTargetY > Main.rockLayer 
                                            && Main.tile[Player.tileTargetX, Player.tileTargetY].type == 77 
                                            && selectedItem.Hammer < 60)
                                        {
                                            hitTile = 0;
                                        }

                                        if (selectedItem.Hammer > 0)
                                        {
                                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 26)
                                            {
                                                Hurt(this.statLife / 2, -direction, false, false);
                                                WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, true, false, false);
                                                if (Main.netMode == 1)
                                                {
                                                    NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, 1f);
                                                }
                                            }
                                            else if (hitTile >= 100)
                                            {
                                                if (Main.netMode == 1 && Main.tile[Player.tileTargetX, Player.tileTargetY].type == 21)
                                                {
                                                    WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, true, false, false);
                                                    NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, 1f);
                                                    NetMessage.SendData(34, -1, -1, "", Player.tileTargetX, (float)Player.tileTargetY);
                                                }
                                                else
                                                {
                                                    hitTile = 0;
                                                    WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, false, false, false);
                                                    if (Main.netMode == 1)
                                                    {
                                                        NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, true, false, false);
                                                if (Main.netMode == 1)
                                                {
                                                    NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, 1f);
                                                }
                                            }

                                            itemTime = inventory[this.selectedItemIndex].UseTime;
                                        }
                                    }
                                    else
                                    {
                                        if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 5 
                                            || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 30 
                                            || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 72)
                                        {
                                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 30)
                                            {
                                                hitTile += selectedItem.Axe * 5;
                                            }
                                            else
                                            {
                                                hitTile += selectedItem.Axe;
                                            }
                                            if (selectedItem.Axe > 0)
                                            {
                                                if (hitTile >= 100)
                                                {
                                                    hitTile = 0;
                                                    WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, false, false, false);
                                                    if (Main.netMode == 1)
                                                    {
                                                        NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY);
                                                    }
                                                }
                                                else
                                                {
                                                    WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, true, false, false);
                                                    if (Main.netMode == 1)
                                                    {
                                                        NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, 1f);
                                                    }
                                                }
                                                this.itemTime = selectedItem.UseTime;
                                            }
                                        }
                                        else
                                        {
                                            if (selectedItem.Pick > 0)
                                            {
                                                if (Main.tileDungeon[(int)Main.tile[Player.tileTargetX, Player.tileTargetY].type] || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 37 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 25 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 58)
                                                {
                                                    hitTile += selectedItem.Pick / 2;
                                                }
                                                else
                                                {
                                                    hitTile += selectedItem.Pick;
                                                }
                                                if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 25 && selectedItem.Pick < 65)
                                                {
                                                    hitTile = 0;
                                                }
                                                else
                                                {
                                                    if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 37 && selectedItem.Pick < 55)
                                                    {
                                                        hitTile = 0;
                                                    }
                                                    else
                                                    {
                                                        if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 56 && selectedItem.Pick < 65)
                                                        {
                                                            hitTile = 0;
                                                        }
                                                        else
                                                        {
                                                            if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 58 && selectedItem.Pick < 65)
                                                            {
                                                                hitTile = 0;
                                                            }
                                                            else
                                                            {
                                                                if (Main.tileDungeon[(int)Main.tile[Player.tileTargetX, Player.tileTargetY].type] && selectedItem.Pick < 65)
                                                                {
                                                                    if ((double)Player.tileTargetX < (double)Main.maxTilesX * 0.25 || (double)Player.tileTargetX > (double)Main.maxTilesX * 0.75)
                                                                    {
                                                                        hitTile = 0;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 0 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 40 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 53 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 59)
                                                {
                                                    hitTile += selectedItem.Pick;
                                                }
                                                if (hitTile >= 100)
                                                {
                                                    hitTile = 0;
                                                    WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, false, false, false);
                                                    if (Main.netMode == 1)
                                                    {
                                                        NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY);
                                                    }
                                                }
                                                else
                                                {
                                                    WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, true, false, false);
                                                    if (Main.netMode == 1)
                                                    {
                                                        NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, 1f);
                                                    }
                                                }
                                                this.itemTime = selectedItem.UseTime;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (Main.tile[Player.tileTargetX, Player.tileTargetY].wall > 0)
                        {
                            if (this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
                            {
                                if (selectedItem.Hammer > 0)
                                {
                                    bool flag3 = true;
                                    if (!Main.wallHouse[(int)Main.tile[Player.tileTargetX, Player.tileTargetY].wall])
                                    {
                                        flag3 = false;
                                        for (int k = Player.tileTargetX - 1; k < Player.tileTargetX + 2; k++)
                                        {
                                            for (int l = Player.tileTargetY - 1; l < Player.tileTargetY + 2; l++)
                                            {
                                                if (Main.tile[k, l].wall != Main.tile[Player.tileTargetX, Player.tileTargetY].wall)
                                                {
                                                    flag3 = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (flag3)
                                    {
                                        if (hitTileX != Player.tileTargetX || hitTileY != Player.tileTargetY)
                                        {
                                            hitTile = 0;
                                            hitTileX = Player.tileTargetX;
                                            hitTileY = Player.tileTargetY;
                                        }
                                        hitTile += selectedItem.Hammer;
                                        if (hitTile >= 100)
                                        {
                                            hitTile = 0;
                                            WorldGen.KillWall(Player.tileTargetX, Player.tileTargetY, false);
                                            if (Main.netMode == 1)
                                            {
                                                NetMessage.SendData(17, -1, -1, "", 2, (float)Player.tileTargetX, (float)Player.tileTargetY);
                                            }
                                        }
                                        else
                                        {
                                            WorldGen.KillWall(Player.tileTargetX, Player.tileTargetY, true);
                                            if (Main.netMode == 1)
                                            {
                                                NetMessage.SendData(17, -1, -1, "", 2, (float)Player.tileTargetX, (float)Player.tileTargetY, 1f);
                                            }
                                        }
                                        this.itemTime = selectedItem.UseTime;
                                    }
                                }
                            }
                        }
                    }
                }
                if (selectedItem.Type == 29 && this.itemAnimation > 0 && this.statLifeMax < 400)
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
                if (selectedItem.Type == 109 && this.itemAnimation > 0 && this.statManaMax < 200)
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
                if (selectedItem.CreateTile >= 0)
                {
                    if (this.Position.X / 16f - (float)Player.tileRangeX - (float)selectedItem.TileBoost <= (float)Player.tileTargetX && (this.Position.X + (float)this.width) / 16f + (float)Player.tileRangeX + (float)selectedItem.TileBoost - 1f >= (float)Player.tileTargetX && this.Position.Y / 16f - (float)Player.tileRangeY - (float)selectedItem.TileBoost <= (float)Player.tileTargetY && (this.Position.Y + (float)this.height) / 16f + (float)Player.tileRangeY + (float)selectedItem.TileBoost - 2f >= (float)Player.tileTargetY)
                    {
                        this.showItemIcon = true;
                        if (!Main.tile[Player.tileTargetX, Player.tileTargetY].Active || selectedItem.CreateTile == 23 || selectedItem.CreateTile == 2 || selectedItem.CreateTile == 60 || selectedItem.CreateTile == 70)
                        {
                            if (this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
                            {
                                bool flag4 = false;
                                if (selectedItem.CreateTile == 23 || selectedItem.CreateTile == 2)
                                {
                                    if (Main.tile[Player.tileTargetX, Player.tileTargetY].Active && Main.tile[Player.tileTargetX, Player.tileTargetY].type == 0)
                                    {
                                        flag4 = true;
                                    }
                                }
                                else
                                {
                                    if (selectedItem.CreateTile == 60 || selectedItem.CreateTile == 70)
                                    {
                                        if (Main.tile[Player.tileTargetX, Player.tileTargetY].Active && Main.tile[Player.tileTargetX, Player.tileTargetY].type == 59)
                                        {
                                            flag4 = true;
                                        }
                                    }
                                    else
                                    {
                                        if (selectedItem.CreateTile == 4)
                                        {
                                            int num13 = (int)Main.tile[Player.tileTargetX, Player.tileTargetY + 1].type;
                                            int num14 = (int)Main.tile[Player.tileTargetX - 1, Player.tileTargetY].type;
                                            int num15 = (int)Main.tile[Player.tileTargetX + 1, Player.tileTargetY].type;
                                            int num16 = (int)Main.tile[Player.tileTargetX - 1, Player.tileTargetY - 1].type;
                                            int num17 = (int)Main.tile[Player.tileTargetX + 1, Player.tileTargetY - 1].type;
                                            int num18 = (int)Main.tile[Player.tileTargetX - 1, Player.tileTargetY - 1].type;
                                            int num19 = (int)Main.tile[Player.tileTargetX + 1, Player.tileTargetY + 1].type;
                                            if (!Main.tile[Player.tileTargetX, Player.tileTargetY + 1].Active)
                                            {
                                                num13 = -1;
                                            }
                                            if (!Main.tile[Player.tileTargetX - 1, Player.tileTargetY].Active)
                                            {
                                                num14 = -1;
                                            }
                                            if (!Main.tile[Player.tileTargetX + 1, Player.tileTargetY].Active)
                                            {
                                                num15 = -1;
                                            }
                                            if (!Main.tile[Player.tileTargetX - 1, Player.tileTargetY - 1].Active)
                                            {
                                                num16 = -1;
                                            }
                                            if (!Main.tile[Player.tileTargetX + 1, Player.tileTargetY - 1].Active)
                                            {
                                                num17 = -1;
                                            }
                                            if (!Main.tile[Player.tileTargetX - 1, Player.tileTargetY + 1].Active)
                                            {
                                                num18 = -1;
                                            }
                                            if (!Main.tile[Player.tileTargetX + 1, Player.tileTargetY + 1].Active)
                                            {
                                                num19 = -1;
                                            }
                                            if (num13 >= 0 && Main.tileSolid[num13] && !Main.tileNoAttach[num13])
                                            {
                                                flag4 = true;
                                            }
                                            else
                                            {
                                                if ((num14 >= 0 && Main.tileSolid[num14] && !Main.tileNoAttach[num14]) || (num14 == 5 && num16 == 5 && num18 == 5))
                                                {
                                                    flag4 = true;
                                                }
                                                else
                                                {
                                                    if ((num15 >= 0 && Main.tileSolid[num15] && !Main.tileNoAttach[num15]) || (num15 == 5 && num17 == 5 && num19 == 5))
                                                    {
                                                        flag4 = true;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (selectedItem.CreateTile == 78)
                                            {
                                                if (Main.tile[Player.tileTargetX, Player.tileTargetY + 1].Active && (Main.tileSolid[(int)Main.tile[Player.tileTargetX, Player.tileTargetY + 1].type] || Main.tileTable[(int)Main.tile[Player.tileTargetX, Player.tileTargetY + 1].type]))
                                                {
                                                    flag4 = true;
                                                }
                                            }
                                            else
                                            {
                                                if (selectedItem.CreateTile == 13 || selectedItem.CreateTile == 29 || selectedItem.CreateTile == 33 || selectedItem.CreateTile == 49)
                                                {
                                                    if (Main.tile[Player.tileTargetX, Player.tileTargetY + 1].Active && Main.tileTable[(int)Main.tile[Player.tileTargetX, Player.tileTargetY + 1].type])
                                                    {
                                                        flag4 = true;
                                                    }
                                                }
                                                else
                                                {
                                                    if (selectedItem.CreateTile == 51)
                                                    {
                                                        if (Main.tile[Player.tileTargetX + 1, Player.tileTargetY].Active || Main.tile[Player.tileTargetX + 1, Player.tileTargetY].wall > 0 || Main.tile[Player.tileTargetX - 1, Player.tileTargetY].Active || Main.tile[Player.tileTargetX - 1, Player.tileTargetY].wall > 0 || Main.tile[Player.tileTargetX, Player.tileTargetY + 1].Active || Main.tile[Player.tileTargetX, Player.tileTargetY + 1].wall > 0 || Main.tile[Player.tileTargetX, Player.tileTargetY - 1].Active || Main.tile[Player.tileTargetX, Player.tileTargetY - 1].wall > 0)
                                                        {
                                                            flag4 = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if ((Main.tile[Player.tileTargetX + 1, Player.tileTargetY].Active && Main.tileSolid[(int)Main.tile[Player.tileTargetX + 1, Player.tileTargetY].type]) || Main.tile[Player.tileTargetX + 1, Player.tileTargetY].wall > 0 || (Main.tile[Player.tileTargetX - 1, Player.tileTargetY].Active && Main.tileSolid[(int)Main.tile[Player.tileTargetX - 1, Player.tileTargetY].type]) || Main.tile[Player.tileTargetX - 1, Player.tileTargetY].wall > 0 || (Main.tile[Player.tileTargetX, Player.tileTargetY + 1].Active && Main.tileSolid[(int)Main.tile[Player.tileTargetX, Player.tileTargetY + 1].type]) || Main.tile[Player.tileTargetX, Player.tileTargetY + 1].wall > 0 || (Main.tile[Player.tileTargetX, Player.tileTargetY - 1].Active && Main.tileSolid[(int)Main.tile[Player.tileTargetX, Player.tileTargetY - 1].type]) || Main.tile[Player.tileTargetX, Player.tileTargetY - 1].wall > 0)
                                                        {
                                                            flag4 = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (flag4)
                                {
                                    if (WorldGen.PlaceTile(Player.tileTargetX, Player.tileTargetY, selectedItem.CreateTile, false, false, this.whoAmi))
                                    {
                                        this.itemTime = selectedItem.UseTime;
                                        if (Main.netMode == 1)
                                        {
                                            NetMessage.SendData(17, -1, -1, "", 1, (float)Player.tileTargetX, (float)Player.tileTargetY, (float)selectedItem.CreateTile);
                                        }
                                        if (selectedItem.CreateTile == 15)
                                        {
                                            if (this.direction == 1)
                                            {
                                                Tile expr_40C8 = Main.tile[Player.tileTargetX, Player.tileTargetY];
                                                expr_40C8.frameX += 18;
                                                Tile expr_40ED = Main.tile[Player.tileTargetX, Player.tileTargetY - 1];
                                                expr_40ED.frameX += 18;
                                            }
                                            if (Main.netMode == 1)
                                            {
                                                NetMessage.SendTileSquare(-1, Player.tileTargetX - 1, Player.tileTargetY - 1, 3);
                                            }
                                        }
                                        else
                                        {
                                            if (selectedItem.CreateTile == 79)
                                            {
                                                if (Main.netMode == 1)
                                                {
                                                    NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 5);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (selectedItem.CreateWall >= 0)
                {
                    if (this.Position.X / 16f - (float)Player.tileRangeX - (float)selectedItem.TileBoost <= (float)Player.tileTargetX && (this.Position.X + (float)this.width) / 16f + (float)Player.tileRangeX + (float)selectedItem.TileBoost - 1f >= (float)Player.tileTargetX && this.Position.Y / 16f - (float)Player.tileRangeY - (float)selectedItem.TileBoost <= (float)Player.tileTargetY && (this.Position.Y + (float)this.height) / 16f + (float)Player.tileRangeY + (float)selectedItem.TileBoost - 2f >= (float)Player.tileTargetY)
                    {
                        this.showItemIcon = true;
                        if (this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
                        {
                            if (Main.tile[Player.tileTargetX + 1, Player.tileTargetY].Active || Main.tile[Player.tileTargetX + 1, Player.tileTargetY].wall > 0 || Main.tile[Player.tileTargetX - 1, Player.tileTargetY].Active || Main.tile[Player.tileTargetX - 1, Player.tileTargetY].wall > 0 || Main.tile[Player.tileTargetX, Player.tileTargetY + 1].Active || Main.tile[Player.tileTargetX, Player.tileTargetY + 1].wall > 0 || Main.tile[Player.tileTargetX, Player.tileTargetY - 1].Active || Main.tile[Player.tileTargetX, Player.tileTargetY - 1].wall > 0)
                            {
                                if ((int)Main.tile[Player.tileTargetX, Player.tileTargetY].wall != selectedItem.CreateWall)
                                {
                                    WorldGen.PlaceWall(Player.tileTargetX, Player.tileTargetY, selectedItem.CreateWall, false);
                                    if ((int)Main.tile[Player.tileTargetX, Player.tileTargetY].wall == selectedItem.CreateWall)
                                    {
                                        this.itemTime = selectedItem.UseTime;
                                        if (Main.netMode == 1)
                                        {
                                            NetMessage.SendData(17, -1, -1, "", 3, (float)Player.tileTargetX, (float)Player.tileTargetY, (float)selectedItem.CreateWall);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (selectedItem.Damage >= 0 && selectedItem.Type > 0 && !selectedItem.NoMelee)
            {
                if (this.itemAnimation > 0)
                {
                    bool flag5 = false;
                    Rectangle rectangle = new Rectangle((int)this.itemLocation.X, (int)this.itemLocation.Y, 32, 32);
                    rectangle.Width = (int)((float)rectangle.Width * selectedItem.Scale);
                    rectangle.Height = (int)((float)rectangle.Height * selectedItem.Scale);
                    if (this.direction == -1)
                    {
                        rectangle.X -= rectangle.Width;
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
                            rectangle.Y += (int)((double)rectangle.Height * 0.5);
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
                                rectangle.Y -= (int)((double)rectangle.Height * 1.4 - (double)rectangle.Height);
                                rectangle.Height = (int)((double)rectangle.Height * 1.4);
                            }
                        }
                    }
                    else
                    {
                        if (selectedItem.UseStyle == 3)
                        {
                            if ((double)this.itemAnimation > (double)this.itemAnimationMax * 0.666)
                            {
                                flag5 = true;
                            }
                            else
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
                    if (!flag5)
                    {
                        if (selectedItem.Type == 44 || selectedItem.Type == 45 || selectedItem.Type == 46 || selectedItem.Type == 103 || selectedItem.Type == 104)
                        {
                            if (Main.rand.Next(15) == 0)
                            {
                                Vector2 arg_49D5_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
                                int arg_49D5_1 = rectangle.Width;
                                int arg_49D5_2 = rectangle.Height;
                                int arg_49D5_3 = 14;
                                float arg_49D5_4 = (float)(this.direction * 2);
                                float arg_49D5_5 = 0f;
                                int arg_49D5_6 = 150;
                                Color newColor = default(Color);
                                Dust.NewDust(arg_49D5_0, arg_49D5_1, arg_49D5_2, arg_49D5_3, arg_49D5_4, arg_49D5_5, arg_49D5_6, newColor, 1.3f);
                            }
                        }
                        if (selectedItem.Type == 65)
                        {
                            if (Main.rand.Next(5) == 0)
                            {
                                Vector2 arg_4A5B_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
                                int arg_4A5B_1 = rectangle.Width;
                                int arg_4A5B_2 = rectangle.Height;
                                int arg_4A5B_3 = 15;
                                float arg_4A5B_4 = 0f;
                                float arg_4A5B_5 = 0f;
                                int arg_4A5B_6 = 150;
                                Color newColor = default(Color);
                                Dust.NewDust(arg_4A5B_0, arg_4A5B_1, arg_4A5B_2, arg_4A5B_3, arg_4A5B_4, arg_4A5B_5, arg_4A5B_6, newColor, 1.2f);
                            }
                            if (Main.rand.Next(10) == 0)
                            {
                                Gore.NewGore(new Vector2((float)rectangle.X, (float)rectangle.Y), default(Vector2), Main.rand.Next(16, 18));
                            }
                        }
                        if (selectedItem.Type == 190 || selectedItem.Type == 213)
                        {
                            Vector2 arg_4B50_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
                            int arg_4B50_1 = rectangle.Width;
                            int arg_4B50_2 = rectangle.Height;
                            int arg_4B50_3 = 40;
                            float arg_4B50_4 = this.Velocity.X * 0.2f + (float)(this.direction * 3);
                            float arg_4B50_5 = this.Velocity.Y * 0.2f;
                            int arg_4B50_6 = 0;
                            Color newColor = default(Color);
                            int num20 = Dust.NewDust(arg_4B50_0, arg_4B50_1, arg_4B50_2, arg_4B50_3, arg_4B50_4, arg_4B50_5, arg_4B50_6, newColor, 1.2f);
                            Main.dust[num20].noGravity = true;
                        }
                        if (selectedItem.Type == 121)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                Vector2 arg_4BF2_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
                                int arg_4BF2_1 = rectangle.Width;
                                int arg_4BF2_2 = rectangle.Height;
                                int arg_4BF2_3 = 6;
                                float arg_4BF2_4 = this.Velocity.X * 0.2f + (float)(this.direction * 3);
                                float arg_4BF2_5 = this.Velocity.Y * 0.2f;
                                int arg_4BF2_6 = 100;
                                Color newColor = default(Color);
                                int num20 = Dust.NewDust(arg_4BF2_0, arg_4BF2_1, arg_4BF2_2, arg_4BF2_3, arg_4BF2_4, arg_4BF2_5, arg_4BF2_6, newColor, 2.5f);
                                Main.dust[num20].noGravity = true;
                                Dust expr_4C14_cp_0 = Main.dust[num20];
                                expr_4C14_cp_0.velocity.X = expr_4C14_cp_0.velocity.X * 2f;
                                Dust expr_4C32_cp_0 = Main.dust[num20];
                                expr_4C32_cp_0.velocity.Y = expr_4C32_cp_0.velocity.Y * 2f;
                            }
                        }
                        if (selectedItem.Type == 122 || selectedItem.Type == 217)
                        {
                            Vector2 arg_4CF3_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
                            int arg_4CF3_1 = rectangle.Width;
                            int arg_4CF3_2 = rectangle.Height;
                            int arg_4CF3_3 = 6;
                            float arg_4CF3_4 = this.Velocity.X * 0.2f + (float)(this.direction * 3);
                            float arg_4CF3_5 = this.Velocity.Y * 0.2f;
                            int arg_4CF3_6 = 100;
                            Color newColor = default(Color);
                            int num20 = Dust.NewDust(arg_4CF3_0, arg_4CF3_1, arg_4CF3_2, arg_4CF3_3, arg_4CF3_4, arg_4CF3_5, arg_4CF3_6, newColor, 1.9f);
                            Main.dust[num20].noGravity = true;
                        }
                        if (selectedItem.Type == 155)
                        {
                            Vector2 arg_4D91_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
                            int arg_4D91_1 = rectangle.Width;
                            int arg_4D91_2 = rectangle.Height;
                            int arg_4D91_3 = 29;
                            float arg_4D91_4 = this.Velocity.X * 0.2f + (float)(this.direction * 3);
                            float arg_4D91_5 = this.Velocity.Y * 0.2f;
                            int arg_4D91_6 = 100;
                            Color newColor = default(Color);
                            int num20 = Dust.NewDust(arg_4D91_0, arg_4D91_1, arg_4D91_2, arg_4D91_3, arg_4D91_4, arg_4D91_5, arg_4D91_6, newColor, 2f);
                            Main.dust[num20].noGravity = true;
                            Dust expr_4DB3_cp_0 = Main.dust[num20];
                            expr_4DB3_cp_0.velocity.X = expr_4DB3_cp_0.velocity.X / 2f;
                            Dust expr_4DD1_cp_0 = Main.dust[num20];
                            expr_4DD1_cp_0.velocity.Y = expr_4DD1_cp_0.velocity.Y / 2f;
                        }

                        if (Main.myPlayer == i)
                        {
                            int num21 = rectangle.X / 16;
                            int num22 = (rectangle.X + rectangle.Width) / 16 + 1;
                            int num23 = rectangle.Y / 16;
                            int num24 = (rectangle.Y + rectangle.Height) / 16 + 1;
                            for (int k = num21; k < num22; k++)
                            {
                                for (int l = num23; l < num24; l++)
                                {
                                    if (Main.tile[k, l].type == 3 || Main.tile[k, l].type == 24 || Main.tile[k, l].type == 28 || Main.tile[k, l].type == 32 || Main.tile[k, l].type == 51 || Main.tile[k, l].type == 52 || Main.tile[k, l].type == 61 || Main.tile[k, l].type == 62 || Main.tile[k, l].type == 69 || Main.tile[k, l].type == 71 || Main.tile[k, l].type == 73 || Main.tile[k, l].type == 74)
                                    {
                                        WorldGen.KillTile(k, l, false, false, false);
                                        if (Main.netMode == 1)
                                        {
                                            NetMessage.SendData(17, -1, -1, "", 0, (float)k, (float)l);
                                        }
                                    }
                                }
                            }
                            for (int j = 0; j < NPC.MAX_NPCS; j++)
                            {
                                if (Main.npcs[j].Active && Main.npcs[j].immune[i] == 0 && this.attackCD == 0 && !Main.npcs[j].friendly)
                                {
                                    Rectangle value = new Rectangle((int)Main.npcs[j].Position.X, (int)Main.npcs[j].Position.Y, Main.npcs[j].width, Main.npcs[j].height);
                                    if (rectangle.Intersects(value))
                                    {
                                        if (Main.npcs[j].noTileCollide || Collision.CanHit(this.Position, this.width, this.height, Main.npcs[j].Position, Main.npcs[j].width, Main.npcs[j].height))
                                        {
                                            Main.npcs[j].StrikeNPC(selectedItem.Damage, selectedItem.KnockBack, this.direction);
                                            if (Main.netMode == 1)
                                            {
                                                NetMessage.SendData(24, -1, -1, "", j, (float)i);
                                            }
                                            Main.npcs[j].immune[i] = this.itemAnimation;
                                            this.attackCD = (int)((double)this.itemAnimationMax * 0.33);
                                        }
                                    }
                                }
                            }
                            if (this.hostile)
                            {
                                for (int j = 0; j < 255; j++)
                                {
                                    if (j != i && Main.players[j].Active && Main.players[j].hostile && !Main.players[j].immune && !Main.players[j].dead)
                                    {
                                        if (Main.players[i].team == 0 || Main.players[i].team != Main.players[j].team)
                                        {
                                            Rectangle value2 = new Rectangle((int)Main.players[j].Position.X, (int)Main.players[j].Position.Y, Main.players[j].width, Main.players[j].height);
                                            if (rectangle.Intersects(value2))
                                            {
                                                if (Collision.CanHit(this.Position, this.width, this.height, Main.players[j].Position, Main.players[j].width, Main.players[j].height))
                                                {
                                                    Main.players[j].Hurt(selectedItem.Damage, this.direction, true, false);
                                                    if (Main.netMode != 0)
                                                    {
                                                        NetMessage.SendData(26, -1, -1, "", j, (float)this.direction, (float)selectedItem.Damage, 1f);
                                                    }
                                                    this.attackCD = (int)((double)this.itemAnimationMax * 0.33);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
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
            }
            if (this.itemTime == 0 && this.itemAnimation > 0 && (selectedItem.Type == 43 || selectedItem.Type == 70))
            {
                this.itemTime = selectedItem.UseTime;
                bool flag6 = false;
                int num25 = 4;
                if (selectedItem.Type == 43)
                {
                    num25 = 4;
                }
                else
                {
                    if (selectedItem.Type == 70)
                    {
                        num25 = 13;
                    }
                }
                for (int j = 0; j < 1000; j++)
                {
                    if (Main.npcs[j].Active && Main.npcs[j].Type == num25)
                    {
                        flag6 = true;
                        break;
                    }
                }
                if (flag6)
                {
                    if (Main.myPlayer == this.whoAmi)
                    {
                        this.Hurt(this.statLife * (this.statDefense + 1), -this.direction, false, false);
                    }
                }
                else if (Main.netMode != 1)
                {
                    if (selectedItem.Type == 43 && !Main.dayTime)
                    {
                        NPC.SpawnOnPlayer(Main.players[i], i, 4);
                    }
                    else if (selectedItem.Type == 70 && zoneEvil)
                    {
                        NPC.SpawnOnPlayer(Main.players[i], i, 13);
                    }
                }
            }

            if (selectedItem.Type == 50 && this.itemAnimation > 0)
            {
                if (Main.rand.Next(2) == 0)
                {
                    Vector2 arg_5792_0 = this.Position;
                    int arg_5792_1 = this.width;
                    int arg_5792_2 = this.height;
                    int arg_5792_3 = 15;
                    float arg_5792_4 = 0f;
                    float arg_5792_5 = 0f;
                    int arg_5792_6 = 150;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_5792_0, arg_5792_1, arg_5792_2, arg_5792_3, arg_5792_4, arg_5792_5, arg_5792_6, newColor, 1.1f);
                }
                if (this.itemTime == 0)
                {
                    this.itemTime = selectedItem.UseTime;
                }
                else
                {
                    if (this.itemTime == selectedItem.UseTime / 2)
                    {
                        for (int j = 0; j < 70; j++)
                        {
                            Vector2 arg_5842_0 = this.Position;
                            int arg_5842_1 = this.width;
                            int arg_5842_2 = this.height;
                            int arg_5842_3 = 15;
                            float arg_5842_4 = this.Velocity.X * 0.5f;
                            float arg_5842_5 = this.Velocity.Y * 0.5f;
                            int arg_5842_6 = 150;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_5842_0, arg_5842_1, arg_5842_2, arg_5842_3, arg_5842_4, arg_5842_5, arg_5842_6, newColor, 1.5f);
                        }
                        this.grappling[0] = -1;
                        this.grapCount = 0;
                        for (int j = 0; j < 1000; j++)
                        {
                            if (Main.projectile[j].active && Main.projectile[j].Owner == i)
                            {
                                if (Main.projectile[j].aiStyle == 7)
                                {
                                    Main.projectile[j].Kill();
                                }
                            }
                        }
                        this.Spawn();
                        for (int j = 0; j < 70; j++)
                        {
                            Vector2 arg_5910_0 = this.Position;
                            int arg_5910_1 = this.width;
                            int arg_5910_2 = this.height;
                            int arg_5910_3 = 15;
                            float arg_5910_4 = 0f;
                            float arg_5910_5 = 0f;
                            int arg_5910_6 = 150;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_5910_0, arg_5910_1, arg_5910_2, arg_5910_3, arg_5910_4, arg_5910_5, arg_5910_6, newColor, 1.5f);
                        }
                    }
                }
            }
            if (i == Main.myPlayer)
            {
                if (this.itemTime == selectedItem.UseTime && selectedItem.Consumable)
                {
                    selectedItem.Stack--;
                    if (selectedItem.Stack <= 0)
                    {
                        this.itemTime = this.itemAnimation;
                    }
                }
                if (selectedItem.Stack <= 0 && this.itemAnimation == 0)
                {
                    inventory[selectedItemIndex] = new Item();
                }
            }
        }

        public Color GetImmuneAlpha(Color newColor)
		{
			float num = (float)(255 - this.immuneAlpha) / 255f;
			if (this.shadow > 0f)
			{
				num *= 1f - this.shadow;
			}
			int r = (int)((float)newColor.R * num);
			int g = (int)((float)newColor.G * num);
			int b = (int)((float)newColor.B * num);
			int num2 = (int)((float)newColor.A * num);
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > 255)
			{
				num2 = 255;
			}
			return new Color(r, g, b, num2);
		}
		
        public Color GetDeathAlpha(Color newColor)
		{
			int r = (int)newColor.R + (int)((double)this.immuneAlpha * 0.9);
			int g = (int)newColor.G + (int)((double)this.immuneAlpha * 0.5);
			int b = (int)newColor.B + (int)((double)this.immuneAlpha * 0.5);
			int num = (int)newColor.A + (int)((double)this.immuneAlpha * 0.4);
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
		
        public void DropItems()
		{
			for (int i = 0; i < 44; i++)
			{
				if (this.inventory[i].Type >= 71 && this.inventory[i].Type <= 74)
				{
					int num = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.width, this.height, this.inventory[i].Type, 1, false);
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
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", num);
					}
				}
			}
		}
		
        public object Clone()
		{
			return base.MemberwiseClone();
		}
		
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
			for (int i = 0; i < 44; i++)
			{
				player.inventory[i] = (Item)this.inventory[i].Clone();
				if (i < 8)
				{
					player.armor[i] = (Item)this.armor[i].Clone();
				}
			}
			return player;
		}
		
        private static void EncryptFile(String inputFile, String outputFile)
		{
			String s = "h3y_gUyZ";
			UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
			byte[] bytes = unicodeEncoding.GetBytes(s);
			FileStream fileStream = new FileStream(outputFile, FileMode.Create);
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			CryptoStream cryptoStream = new CryptoStream(fileStream, rijndaelManaged.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
			FileStream fileStream2 = new FileStream(inputFile, FileMode.Open);
			int num;
			while ((num = fileStream2.ReadByte()) != -1)
			{
				cryptoStream.WriteByte((byte)num);
			}
			fileStream2.Close();
			cryptoStream.Close();
			fileStream.Close();
		}
		
        private static bool DecryptFile(String inputFile, String outputFile)
		{
			String s = "h3y_gUyZ";
			UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
			byte[] bytes = unicodeEncoding.GetBytes(s);
			FileStream fileStream = new FileStream(inputFile, FileMode.Open);
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			CryptoStream cryptoStream = new CryptoStream(fileStream, rijndaelManaged.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
			FileStream fileStream2 = new FileStream(outputFile, FileMode.Create);

			try
			{
				int num;
				while ((num = cryptoStream.ReadByte()) != -1)
				{
					fileStream2.WriteByte((byte)num);
				}
				fileStream2.Close();
				cryptoStream.Close();
				fileStream.Close();
			}
			catch
			{
				fileStream2.Close();
				fileStream.Close();
				File.Delete(outputFile);
				return true;
			}
			return false;
		}
		
        public static bool CheckSpawn(int x, int y)
		{
			if (x < 10 || x > Main.maxTilesX - 10 || y < 10 || y > Main.maxTilesX - 10)
			{
				return false;
			}
			if (Main.tile[x, y - 1] == null)
			{
				return false;
			}
			if (!Main.tile[x, y - 1].Active || Main.tile[x, y - 1].type != 79)
			{
				return false;
			}
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = y - 3; j < y; j++)
				{
					if (Main.tile[i, j] == null)
					{
						return false;
					}
					if (Main.tile[i, j].Active && Main.tileSolid[(int)Main.tile[i, j].type] && !Main.tileSolidTop[(int)Main.tile[i, j].type])
					{
						return false;
					}
				}
			}
			return WorldGen.StartRoomCheck(x, y - 1);
		}
		
        public void FindSpawn()
		{
			for (int i = 0; i < 200; i++)
			{
				if (this.spN[i] == null)
				{
					this.SpawnX = -1;
					this.SpawnY = -1;
					return;
				}
				if (this.spN[i] == Main.worldName && this.spI[i] == Main.worldID)
				{
					this.SpawnX = this.spX[i];
					this.SpawnY = this.spY[i];
					return;
				}
			}
		}
		
        public void ChangeSpawn(int x, int y)
		{
			int num = 0;
			while (num < 200 && this.spN[num] != null)
			{
				if (this.spN[num] == Main.worldName && this.spI[num] == Main.worldID)
				{
					for (int i = num; i > 0; i--)
					{
						this.spN[i] = this.spN[i - 1];
						this.spI[i] = this.spI[i - 1];
						this.spX[i] = this.spX[i - 1];
						this.spY[i] = this.spY[i - 1];
					}
					this.spN[0] = Main.worldName;
					this.spI[0] = Main.worldID;
					this.spX[0] = x;
					this.spY[0] = y;
					return;
				}
				num++;
			}
			for (int j = 199; j > 0; j--)
			{
				if (this.spN[j - 1] != null)
				{
					this.spN[j] = this.spN[j - 1];
					this.spI[j] = this.spI[j - 1];
					this.spX[j] = this.spX[j - 1];
					this.spY[j] = this.spY[j - 1];
				}
			}
			this.spN[0] = Main.worldName;
			this.spI[0] = Main.worldID;
			this.spX[0] = x;
			this.spY[0] = y;
		}

        public bool HasItem(int type)
        {
            for (int i = 0; i < 44; i++)
            {
                if (type == this.inventory[i].Type)
                {
                    return true;
                }
            }
            return false;
        }
		
        public static void SavePlayer(Player newPlayer)
		{
            String playerPath = Statics.PlayerPath + "\\" + newPlayer.Name;
            try
            {
                Directory.CreateDirectory(Statics.PlayerPath);
            }
            catch
            {
            }
            if (playerPath == null)
            {
                return;
            }
			String destFileName = playerPath + ".bak";
			if (File.Exists(playerPath))
			{
				File.Copy(playerPath, destFileName, true);
			}
			String text = playerPath + ".dat";
			using (FileStream fileStream = new FileStream(text, FileMode.Create))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					binaryWriter.Write(Statics.CURRENT_TERRARIA_RELEASE);
					binaryWriter.Write(newPlayer.Name);
					binaryWriter.Write(newPlayer.hair);
					binaryWriter.Write(newPlayer.statLife);
					binaryWriter.Write(newPlayer.statLifeMax);
					binaryWriter.Write(newPlayer.statMana);
					binaryWriter.Write(newPlayer.statManaMax);
					binaryWriter.Write(newPlayer.hairColor.R);
					binaryWriter.Write(newPlayer.hairColor.G);
					binaryWriter.Write(newPlayer.hairColor.B);
					binaryWriter.Write(newPlayer.skinColor.R);
					binaryWriter.Write(newPlayer.skinColor.G);
					binaryWriter.Write(newPlayer.skinColor.B);
					binaryWriter.Write(newPlayer.eyeColor.R);
					binaryWriter.Write(newPlayer.eyeColor.G);
					binaryWriter.Write(newPlayer.eyeColor.B);
					binaryWriter.Write(newPlayer.shirtColor.R);
					binaryWriter.Write(newPlayer.shirtColor.G);
					binaryWriter.Write(newPlayer.shirtColor.B);
					binaryWriter.Write(newPlayer.underShirtColor.R);
					binaryWriter.Write(newPlayer.underShirtColor.G);
					binaryWriter.Write(newPlayer.underShirtColor.B);
					binaryWriter.Write(newPlayer.pantsColor.R);
					binaryWriter.Write(newPlayer.pantsColor.G);
					binaryWriter.Write(newPlayer.pantsColor.B);
					binaryWriter.Write(newPlayer.shoeColor.R);
					binaryWriter.Write(newPlayer.shoeColor.G);
					binaryWriter.Write(newPlayer.shoeColor.B);
					for (int i = 0; i < 8; i++)
					{
						if (newPlayer.armor[i].Name == null)
						{
							newPlayer.armor[i].Name = "";
						}
						binaryWriter.Write(newPlayer.armor[i].Name);
					}
					for (int j = 0; j < 44; j++)
					{
						if (newPlayer.inventory[j].Name == null)
						{
							newPlayer.inventory[j].Name = "";
						}
						binaryWriter.Write(newPlayer.inventory[j].Name);
						binaryWriter.Write(newPlayer.inventory[j].Stack);
					}
					for (int k = 0; k < Chest.MAX_ITEMS; k++)
					{
						if (newPlayer.bank[k].Name == null)
						{
							newPlayer.bank[k].Name = "";
						}
						binaryWriter.Write(newPlayer.bank[k].Name);
						binaryWriter.Write(newPlayer.bank[k].Stack);
					}
					for (int l = 0; l < 200; l++)
					{
						if (newPlayer.spN[l] == null)
						{
							binaryWriter.Write(-1);
							break;
						}
						binaryWriter.Write(newPlayer.spX[l]);
						binaryWriter.Write(newPlayer.spY[l]);
						binaryWriter.Write(newPlayer.spI[l]);
						binaryWriter.Write(newPlayer.spN[l]);
					}
					binaryWriter.Close();
				}
			}
			Player.EncryptFile(text, playerPath);
			File.Delete(text);
		}
		
        public static Player LoadPlayer(String playerPath)
		{
			bool flag = false;
			if (Main.rand == null)
			{
				Main.rand = new Random((int)DateTime.Now.Ticks);
			}
			Player player = new Player();
			try
			{
				String text = playerPath + ".dat";
				flag = Player.DecryptFile(playerPath, text);
				if (!flag)
				{
					using (FileStream fileStream = new FileStream(text, FileMode.Open))
					{
						using (BinaryReader binaryReader = new BinaryReader(fileStream))
						{
							int release = binaryReader.ReadInt32();
							player.Name = binaryReader.ReadString();
							player.hair = binaryReader.ReadInt32();
							player.statLife = binaryReader.ReadInt32();
							player.statLifeMax = binaryReader.ReadInt32();
							if (player.statLife > player.statLifeMax)
							{
								player.statLife = player.statLifeMax;
							}
							player.statMana = binaryReader.ReadInt32();
							player.statManaMax = binaryReader.ReadInt32();
							if (player.statMana > player.statManaMax)
							{
								player.statMana = player.statManaMax;
							}
							player.hairColor.R = binaryReader.ReadByte();
							player.hairColor.G = binaryReader.ReadByte();
							player.hairColor.B = binaryReader.ReadByte();
							player.skinColor.R = binaryReader.ReadByte();
							player.skinColor.G = binaryReader.ReadByte();
							player.skinColor.B = binaryReader.ReadByte();
							player.eyeColor.R = binaryReader.ReadByte();
							player.eyeColor.G = binaryReader.ReadByte();
							player.eyeColor.B = binaryReader.ReadByte();
							player.shirtColor.R = binaryReader.ReadByte();
							player.shirtColor.G = binaryReader.ReadByte();
							player.shirtColor.B = binaryReader.ReadByte();
							player.underShirtColor.R = binaryReader.ReadByte();
							player.underShirtColor.G = binaryReader.ReadByte();
							player.underShirtColor.B = binaryReader.ReadByte();
							player.pantsColor.R = binaryReader.ReadByte();
							player.pantsColor.G = binaryReader.ReadByte();
							player.pantsColor.B = binaryReader.ReadByte();
							player.shoeColor.R = binaryReader.ReadByte();
							player.shoeColor.G = binaryReader.ReadByte();
							player.shoeColor.B = binaryReader.ReadByte();
							for (int i = 0; i < 8; i++)
							{
								player.armor[i].SetDefaults(Item.VersionName(binaryReader.ReadString(), release));
							}
							for (int j = 0; j < 44; j++)
							{
								player.inventory[j].SetDefaults(Item.VersionName(binaryReader.ReadString(), release));
								player.inventory[j].Stack = binaryReader.ReadInt32();
							}
							for (int k = 0; k < Chest.MAX_ITEMS; k++)
							{
								player.bank[k].SetDefaults(Item.VersionName(binaryReader.ReadString(), release));
								player.bank[k].Stack = binaryReader.ReadInt32();
							}
							for (int l = 0; l < 200; l++)
							{
								int num = binaryReader.ReadInt32();
								if (num == -1)
								{
									break;
								}
								player.spX[l] = num;
								player.spY[l] = binaryReader.ReadInt32();
								player.spI[l] = binaryReader.ReadInt32();
								player.spN[l] = binaryReader.ReadString();
							}
							binaryReader.Close();
						}
					}
					player.PlayerFrame();
					File.Delete(text);
                    return player;
				}
			}
			catch
			{
				flag = true;
			}
			if (!flag)
			{
				return new Player();
			}
			String text2 = playerPath + ".bak";
			if (File.Exists(text2))
			{
				File.Delete(playerPath);
				File.Move(text2, playerPath);
				return Player.LoadPlayer(playerPath);
			}
			return new Player();
		}
		
        public Player()
		{
			for (int i = 0; i < 44; i++)
			{
				if (i < 8)
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
			this.grappling[0] = -1;
			this.inventory[0].SetDefaults("Copper Pickaxe");
			this.inventory[1].SetDefaults("Copper Axe");
			for (int k = 0; k < 80; k++)
			{
				this.adjTile[k] = false;
				this.oldAdjTile[k] = false;
			}
		}

        public static String getDeathMessage(int plr = -1, int npc = -1, int proj = -1, int other = -1)
        {
            String result = "";
            int num = Main.rand.Next(11);
            String text = "";
            if (num == 0)
            {
                text = " was slain";
            }
            else
            {
                if (num == 1)
                {
                    text = " was eviscerated";
                }
                else
                {
                    if (num == 2)
                    {
                        text = " was murdered";
                    }
                    else
                    {
                        if (num == 3)
                        {
                            text = "'s face was torn off";
                        }
                        else
                        {
                            if (num == 4)
                            {
                                text = "'s entrails were ripped out";
                            }
                            else
                            {
                                if (num == 5)
                                {
                                    text = " was destroyed";
                                }
                                else
                                {
                                    if (num == 6)
                                    {
                                        text = "'s skull was crushed";
                                    }
                                    else
                                    {
                                        if (num == 7)
                                        {
                                            text = " got massacred";
                                        }
                                        else
                                        {
                                            if (num == 8)
                                            {
                                                text = " got impaled";
                                            }
                                            else
                                            {
                                                if (num == 9)
                                                {
                                                    text = " was torn in half";
                                                }
                                                else
                                                {
                                                    if (num == 10)
                                                    {
                                                        text = " was decapitated";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (plr >= 0 && plr < 255)
            {
                if (proj >= 0)
                {
                    result = String.Concat(new String[]
			{
				text, 
				" by ", 
				Main.players[plr].Name, 
				"'s ", 
				Main.projectile[proj].name, 
				"."
			});
                }
                else
                {
                    result = String.Concat(new String[]
			{
				text, 
				" by ", 
				Main.players[plr].Name, 
				"'s ", 
				Main.players[plr].inventory[Main.players[plr].selectedItemIndex].Name, 
				"."
			});
                }
            }
            else
            {
                if (npc >= 0)
                {
                    result = text + " by " + Main.npcs[npc].Name + ".";
                }
                else
                {
                    if (proj >= 0)
                    {
                        result = text + " by " + Main.projectile[proj].name + ".";
                    }
                    else
                    {
                        if (other >= 0)
                        {
                            if (other == 0)
                            {
                                int num2 = Main.rand.Next(5);
                                if (num2 == 0)
                                {
                                    result = " fell to their death.";
                                }
                                else
                                {
                                    if (num2 == 1)
                                    {
                                        result = " faceplanted the ground.";
                                    }
                                    else
                                    {
                                        if (num2 == 2)
                                        {
                                            result = " fell victim to gravity.";
                                        }
                                        else
                                        {
                                            if (num2 == 3)
                                            {
                                                result = " left a small crater.";
                                            }
                                            else
                                            {
                                                if (num2 == 4)
                                                {
                                                    result = " didn't bounce.";
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (other == 1)
                                {
                                    int num3 = Main.rand.Next(4);
                                    if (num3 == 0)
                                    {
                                        result = " forgot to breathe.";
                                    }
                                    else
                                    {
                                        if (num3 == 1)
                                        {
                                            result = " is sleeping with the fish.";
                                        }
                                        else
                                        {
                                            if (num3 == 2)
                                            {
                                                result = " drowned.";
                                            }
                                            else
                                            {
                                                if (num3 == 3)
                                                {
                                                    result = " is shark food.";
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (other == 2)
                                    {
                                        int num4 = Main.rand.Next(4);
                                        if (num4 == 0)
                                        {
                                            result = " got melted.";
                                        }
                                        else
                                        {
                                            if (num4 == 1)
                                            {
                                                result = " was incinerated.";
                                            }
                                            else
                                            {
                                                if (num4 == 2)
                                                {
                                                    result = " tried to swim in lava.";
                                                }
                                                else
                                                {
                                                    if (num4 == 3)
                                                    {
                                                        result = " likes to play in magma.";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (other == 3)
                                        {
                                            result = text + ".";
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

        public ServerSock getSocket()
        {
            return Netplay.serverSock[this.whoAmi];
        }

        public void Kick(String Reason = null)
        {
            String message = "You have been Kicked from this Server.";

            if (Reason != null)
            {
                message = Reason;
            }

            NetMessage.SendData(2, this.whoAmi, -1, message);
        }

        public String getIPAddress()
        {
            return ipAddress;
        }

        public void setIPAddress(String IPaddress)
        {
            ipAddress = IPaddress;
        }

        public Vector2 getTileLocation()
        {
            return new Vector2(Position.X * 16, Position.Y * 16);
        }

        public void setTileLocation(Vector2 Location)
        {
            Position.X = Location.X / 16;
            Position.Y = Location.Y / 16;
        }

        public Vector2 getLocation()
        {
            return Position;
        }

        public void setLocation(Vector2 Location)
        {
            Position = Location;
        }

        public void teleportTo(float tileX, float tileY)
        {

            PlayerTeleportEvent playerEvent = new PlayerTeleportEvent();
            playerEvent.ToLocation = new Vector2(tileX, tileY);
            playerEvent.FromLocation = new Vector2(this.Position.X, this.Position.Y);
            playerEvent.Sender = this;
            Program.server.getPluginManager().processHook(Hooks.PLAYER_TELEPORT, playerEvent);
            if (playerEvent.Cancelled)
            {
                return;
            }

            //Preserve out Spawn point.
            int xPreserve = Main.spawnTileX;
            int yPreserve = Main.spawnTileY;

            //The spawn the client wants is the from player Pos /16.
            //This is because the Client reads frames, Not Tile Records.
            Main.spawnTileX = ((int)tileX / 16);
            Main.spawnTileY = ((int)tileY / 16);

            NetMessage.SendData((int)Packet.WORLD_DATA, this.whoAmi); //Trigger Client Data Update (Updates Spawn Position)
            NetMessage.SendData((int)Packet.WORLD_DATA); //Trigger Client Data Update (Updates Spawn Position)
            NetMessage.SendData((int)Packet.RECEIVING_PLAYER_JOINED, -1, -1, "", this.whoAmi); //Trigger the player to spawn

            this.UpdatePlayer(this.whoAmi); //Update players data (I don't think needed by default, But hay)

            this.Position.X = tileX;
            this.Position.Y = tileY;

            //Return our preserved Spawn Point.
            Main.spawnTileX = xPreserve;
            Main.spawnTileY = yPreserve;

            this.SpawnX = Main.spawnTileX;
            this.SpawnY = Main.spawnTileY;

            this.Spawn(); //Tell the Client to Spawn (Sets Defaults)
            this.UpdatePlayer(this.whoAmi); //Update players data (I don't think needed by default, But hay)

            NetMessage.SendData((int)Packet.WORLD_DATA, this.whoAmi); //Trigger Client Data Update (Updates Spawn Position)

            NetMessage.syncPlayers(); //Sync the Players Position.
        }

        public void teleportTo(Player player)
        {
            this.teleportTo(player.Position.X, player.Position.Y);
        }

        public static String getPassword(String server, Server Server)
        {
            foreach (String listee in Server.OpList.WhiteList)
            {
                if (listee != null && listee.Trim().ToLower().Length > 0)
                {
                    String userPass = listee.Trim().ToLower();
                    if (userPass.Contains(":"))
                    {
                        if (userPass.Split(':')[0] == server.Trim().ToLower())
                        {
                            return userPass.Split(':')[1];
                        }
                    }
                }
            }
            return null;
        }

        public String getPassword()
        {
            return Player.getPassword(this.Name, this.getServer());
        }

        public static bool isInOpList(String Name, Server Server)
        {
            foreach (String listee in Server.OpList.WhiteList)
            {
                if (listee != null && listee.Trim().ToLower().Length > 0)
                {
                    String userPass = listee.Trim().ToLower();
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

        public bool isInOpList()
        {
            return Player.isInOpList(this.Name, this.getServer());
        }

        public String getOpListKey()
        {
            return this.Name.Trim().ToLower() + getPassword();
        }

        public bool getGodMode()
        {
            return this.godMode;
        }

        public void setGodMode(bool State)
        {
            this.godMode = State;
        }

    }
}
