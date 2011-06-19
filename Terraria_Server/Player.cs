using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Terraria_Server.Commands;
using Terraria_Server.Events;
using Terraria_Server.Plugin;
using System.Net;
using System.Collections;

namespace Terraria_Server
{
	public class Player : Sender
	{
        private string ipAddress = null;

		public bool pvpDeath;
		public bool zoneDungeon;
		public bool zoneEvil;
		public bool zoneMeteor;
		public bool zoneJungle;
		public bool boneArmor;
		public int townNPCs;
		public Vector2 position;
        public Vector2 velocity;
        public Vector2 oldVelocity;
		public double headFrameCounter;
		public double bodyFrameCounter;
		public double legFrameCounter;
		public bool immune;
		public int immuneTime;
		public int immuneAlphaDirection;
		public int immuneAlpha;
		public int team;
		public string chatText = "";
		public int sign = -1;
		public int chatShowTime;
		public int activeNPCs;
		public bool mouseInterface;
		public int changeItem = -1;
		public int selectedItem;
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
		public string setBonus = "";
		public Item[] inventory = new Item[44];
		public Item[] bank = new Item[Chest.maxItems];
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
		public string name = "";
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
		public bool active;
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
		public string[] spN = new string[200];
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


        public void HealEffect(int healAmount, bool overrider = false, int remoteClient = -1)
		{
			//CombatText.NewText(new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height), new Color(100, 100, 255, 255), string.Concat(healAmount));
            if (overrider || (Main.netMode == 1 && this.whoAmi == Main.myPlayer))
			{
                NetMessage.SendData(35, remoteClient, -1, "", this.whoAmi, (float)healAmount, 0f, 0f);
			}
		}

        public void ManaEffect(int manaAmount, bool overrider = false, int remoteClient = -1)
		{
			//CombatText.NewText(new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height), new Color(180, 50, 255, 255), string.Concat(manaAmount));
			if (overrider || (Main.netMode == 1 && this.whoAmi == Main.myPlayer))
			{
                NetMessage.SendData(43, remoteClient, -1, "", this.whoAmi, (float)manaAmount, 0f, 0f);
			}
		}
		
        public static byte FindClosest(Vector2 Position, int Width, int Height)
		{
			byte result = 0;
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active)
				{
					result = (byte)i;
					break;
				}
			}
			float num = -1f;
			for (int j = 0; j < 255; j++)
			{
				if (Main.player[j].active && !Main.player[j].dead && (num == -1f || Math.Abs(Main.player[j].position.X + (float)(Main.player[j].width / 2) - Position.X + (float)(Width / 2)) + Math.Abs(Main.player[j].position.Y + (float)(Main.player[j].height / 2) - Position.Y + (float)(Height / 2)) < num))
				{
					num = Math.Abs(Main.player[j].position.X + (float)(Main.player[j].width / 2) - Position.X + (float)(Width / 2)) + Math.Abs(Main.player[j].position.Y + (float)(Main.player[j].height / 2) - Position.Y + (float)(Height / 2));
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
                if (this.active)
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
                                this.shadowPos[0] = this.position;
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
                            if (Main.dungeonTiles >= 250 && (double)this.position.Y > Main.worldSurface * 16.0 + (double)Main.screenHeight)
                            {
                                int num7 = (int)this.position.X / 16;
                                int num8 = (int)this.position.Y / 16;
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
                                int num12 = (int)(((double)this.position.X + (double)this.width * 0.5) / 16.0);
                                int num13 = (int)(((double)this.position.Y + (double)this.height * 0.5) / 16.0);
                                if (num12 < this.chestX - 5 || num12 > this.chestX + 6 || num13 < this.chestY - 4 || num13 > this.chestY + 5)
                                {
                                    this.chest = -1;
                                }
                                if (!Main.tile[this.chestX, this.chestY].active)
                                {
                                    this.chest = -1;
                                }
                            }
                            if (this.velocity.Y == 0f)
                            {
                                int num14 = (int)(this.position.Y / 16f) - this.fallStart;
                                if (num14 > 25 && !this.noFallDmg)
                                {
                                    int damage = (num14 - 25) * 10;
                                    this.immune = false;
                                    this.Hurt(damage, -this.direction, false, false);
                                }
                                this.fallStart = (int)(this.position.Y / 16f);
                            }
                            if (this.velocity.Y < 0f || this.rocketDelay > 0 || this.wet)
                            {
                                this.fallStart = (int)(this.position.Y / 16f);
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
                            this.statDefense += this.armor[l].defense;
                            this.lifeRegen += this.armor[l].lifeRegen;
                            this.manaRegen += this.armor[l].manaRegen;
                            if (this.armor[l].type == 193)
                            {
                                this.fireWalk = true;
                            }
                            if (this.armor[l].type == 238)
                            {
                                this.magicBoost *= 1.15f;
                            }
                        }
                        this.head = this.armor[0].headSlot;
                        this.body = this.armor[1].bodySlot;
                        this.legs = this.armor[2].legSlot;
                        for (int m = 3; m < 8; m++)
                        {
                            if (this.armor[m].type == 15 && this.accWatch < 1)
                            {
                                this.accWatch = 1;
                            }
                            if (this.armor[m].type == 16 && this.accWatch < 2)
                            {
                                this.accWatch = 2;
                            }
                            if (this.armor[m].type == 17 && this.accWatch < 3)
                            {
                                this.accWatch = 3;
                            }
                            if (this.armor[m].type == 18 && this.accDepthMeter < 1)
                            {
                                this.accDepthMeter = 1;
                            }
                            if (this.armor[m].type == 53)
                            {
                                this.doubleJump = true;
                            }
                            if (this.armor[m].type == 54)
                            {
                                num6 = 6f;
                            }
                            if (this.armor[m].type == 128)
                            {
                                this.rocketBoots = true;
                            }
                            if (this.armor[m].type == 156)
                            {
                                this.noKnockback = true;
                            }
                            if (this.armor[m].type == 158)
                            {
                                this.noFallDmg = true;
                            }
                            if (this.armor[m].type == 159)
                            {
                                this.jumpBoost = true;
                            }
                            if (this.armor[m].type == 187)
                            {
                                this.accFlipper = true;
                            }
                            if (this.armor[m].type == 211)
                            {
                                this.meleeSpeed *= 0.9f;
                            }
                            if (this.armor[m].type == 223)
                            {
                                this.spawnMax = true;
                            }
                            if (this.armor[m].type == 212)
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
                            int i2 = (int)(this.position.X + (float)(this.width / 2) + (float)(8 * this.direction)) / 16;
                            int j2 = (int)(this.position.Y + 2f) / 16;
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
                                Vector2 arg_147A_0 = new Vector2(this.position.X, this.position.Y);
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
                            if (Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y) > 1f && !this.rocketFrame)
                            {
                                for (int n = 0; n < 2; n++)
                                {
                                    Vector2 arg_1561_0 = new Vector2(this.position.X - this.velocity.X * 2f, this.position.Y - 2f - this.velocity.Y * 2f);
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
                                    expr_1583_cp_0.velocity.X = expr_1583_cp_0.velocity.X - this.velocity.X * 0.5f;
                                    Dust expr_15AD_cp_0 = Main.dust[num15];
                                    expr_15AD_cp_0.velocity.Y = expr_15AD_cp_0.velocity.Y - this.velocity.Y * 0.5f;
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
                            if (Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y) > 1f)
                            {
                                Vector2 arg_1701_0 = new Vector2(this.position.X - this.velocity.X * 2f, this.position.Y - 2f - this.velocity.Y * 2f);
                                int arg_1701_1 = this.width;
                                int arg_1701_2 = this.height;
                                int arg_1701_3 = 40;
                                float arg_1701_4 = 0f;
                                float arg_1701_5 = 0f;
                                int arg_1701_6 = 50;
                                Color newColor = new Color();
                                int num16 = Dust.NewDust(arg_1701_0, arg_1701_1, arg_1701_2, arg_1701_3, arg_1701_4, arg_1701_5, arg_1701_6, newColor, 1.4f);
                                Main.dust[num16].noGravity = true;
                                Main.dust[num16].velocity.X = this.velocity.X * 0.25f;
                                Main.dust[num16].velocity.Y = this.velocity.Y * 0.25f;
                            }
                        }
                        if (this.head == 9 && this.body == 9 && this.legs == 9)
                        {
                            this.setBonus = "5 defense";
                            this.statDefense += 5;
                            if (Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y) > 1f && !this.rocketFrame)
                            {
                                for (int num17 = 0; num17 < 2; num17++)
                                {
                                    Vector2 arg_1847_0 = new Vector2(this.position.X - this.velocity.X * 2f, this.position.Y - 2f - this.velocity.Y * 2f);
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
                                    expr_1869_cp_0.velocity.X = expr_1869_cp_0.velocity.X - this.velocity.X * 0.5f;
                                    Dust expr_1893_cp_0 = Main.dust[num18];
                                    expr_1893_cp_0.velocity.Y = expr_1893_cp_0.velocity.Y - this.velocity.Y * 0.5f;
                                }
                            }
                        }
                        if (!this.doubleJump)
                        {
                            this.jumpAgain = false;
                        }
                        else
                        {
                            if (this.velocity.Y == 0f)
                            {
                                this.jumpAgain = true;
                            }
                        }
                        if ((double)this.meleeSpeed < 0.7)
                        {
                            this.meleeSpeed = 0.7f;
                        }
                        if (this.grappling[0] == -1)
                        {
                            if (this.controlLeft && this.velocity.X > -num3)
                            {
                                if (this.velocity.X > num5)
                                {
                                    this.velocity.X = this.velocity.X - num5;
                                }
                                this.velocity.X = this.velocity.X - num4;
                                if (this.itemAnimation == 0 || this.inventory[this.selectedItem].useTurn)
                                {
                                    this.direction = -1;
                                }
                            }
                            else
                            {
                                if (this.controlRight && this.velocity.X < num3)
                                {
                                    if (this.velocity.X < -num5)
                                    {
                                        this.velocity.X = this.velocity.X + num5;
                                    }
                                    this.velocity.X = this.velocity.X + num4;
                                    if (this.itemAnimation == 0 || this.inventory[this.selectedItem].useTurn)
                                    {
                                        this.direction = 1;
                                    }
                                }
                                else
                                {
                                    if (this.controlLeft && this.velocity.X > -num6)
                                    {
                                        if (this.itemAnimation == 0 || this.inventory[this.selectedItem].useTurn)
                                        {
                                            this.direction = -1;
                                        }
                                        if (this.velocity.Y == 0f)
                                        {
                                            if (this.velocity.X > num5)
                                            {
                                                this.velocity.X = this.velocity.X - num5;
                                            }
                                            this.velocity.X = this.velocity.X - num4 * 0.2f;
                                        }
                                        if (this.velocity.X < -(num6 + num3) / 2f && this.velocity.Y == 0f)
                                        {
                                            if (this.runSoundDelay == 0 && this.velocity.Y == 0f)
                                            {
                                                //Main.PlaySound(17, (int)this.position.X, (int)this.position.Y, 1);
                                                this.runSoundDelay = 9;
                                            }
                                            Vector2 arg_1B6C_0 = new Vector2(this.position.X - 4f, this.position.Y + (float)this.height);
                                            int arg_1B6C_1 = this.width + 8;
                                            int arg_1B6C_2 = 4;
                                            int arg_1B6C_3 = 16;
                                            float arg_1B6C_4 = -this.velocity.X * 0.5f;
                                            float arg_1B6C_5 = this.velocity.Y * 0.5f;
                                            int arg_1B6C_6 = 50;
                                            Color newColor = new Color();
                                            int num19 = Dust.NewDust(arg_1B6C_0, arg_1B6C_1, arg_1B6C_2, arg_1B6C_3, arg_1B6C_4, arg_1B6C_5, arg_1B6C_6, newColor, 1.5f);
                                            Main.dust[num19].velocity.X = Main.dust[num19].velocity.X * 0.2f;
                                            Main.dust[num19].velocity.Y = Main.dust[num19].velocity.Y * 0.2f;
                                        }
                                    }
                                    else
                                    {
                                        if (this.controlRight && this.velocity.X < num6)
                                        {
                                            if (this.itemAnimation == 0 || this.inventory[this.selectedItem].useTurn)
                                            {
                                                this.direction = 1;
                                            }
                                            if (this.velocity.Y == 0f)
                                            {
                                                if (this.velocity.X < -num5)
                                                {
                                                    this.velocity.X = this.velocity.X + num5;
                                                }
                                                this.velocity.X = this.velocity.X + num4 * 0.2f;
                                            }
                                            if (this.velocity.X > (num6 + num3) / 2f && this.velocity.Y == 0f)
                                            {
                                                if (this.runSoundDelay == 0 && this.velocity.Y == 0f)
                                                {
                                                    //Main.PlaySound(17, (int)this.position.X, (int)this.position.Y, 1);
                                                    this.runSoundDelay = 9;
                                                }
                                                Vector2 arg_1D34_0 = new Vector2(this.position.X - 4f, this.position.Y + (float)this.height);
                                                int arg_1D34_1 = this.width + 8;
                                                int arg_1D34_2 = 4;
                                                int arg_1D34_3 = 16;
                                                float arg_1D34_4 = -this.velocity.X * 0.5f;
                                                float arg_1D34_5 = this.velocity.Y * 0.5f;
                                                int arg_1D34_6 = 50;
                                                Color newColor = new Color();
                                                int num20 = Dust.NewDust(arg_1D34_0, arg_1D34_1, arg_1D34_2, arg_1D34_3, arg_1D34_4, arg_1D34_5, arg_1D34_6, newColor, 1.5f);
                                                Main.dust[num20].velocity.X = Main.dust[num20].velocity.X * 0.2f;
                                                Main.dust[num20].velocity.Y = Main.dust[num20].velocity.Y * 0.2f;
                                            }
                                        }
                                        else
                                        {
                                            if (this.velocity.Y == 0f)
                                            {
                                                if (this.velocity.X > num5)
                                                {
                                                    this.velocity.X = this.velocity.X - num5;
                                                }
                                                else
                                                {
                                                    if (this.velocity.X < -num5)
                                                    {
                                                        this.velocity.X = this.velocity.X + num5;
                                                    }
                                                    else
                                                    {
                                                        this.velocity.X = 0f;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((double)this.velocity.X > (double)num5 * 0.5)
                                                {
                                                    this.velocity.X = this.velocity.X - num5 * 0.5f;
                                                }
                                                else
                                                {
                                                    if ((double)this.velocity.X < (double)(-(double)num5) * 0.5)
                                                    {
                                                        this.velocity.X = this.velocity.X + num5 * 0.5f;
                                                    }
                                                    else
                                                    {
                                                        this.velocity.X = 0f;
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
                                    if (this.velocity.Y > -Player.jumpSpeed + num2 * 2f)
                                    {
                                        this.jump = 0;
                                    }
                                    else
                                    {
                                        this.velocity.Y = -Player.jumpSpeed;
                                        this.jump--;
                                    }
                                }
                                else
                                {
                                    if ((this.velocity.Y == 0f || this.jumpAgain || (this.wet && this.accFlipper)) && this.releaseJump)
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
                                        if (this.velocity.Y == 0f && this.doubleJump)
                                        {
                                            this.jumpAgain = true;
                                        }
                                        if (this.velocity.Y == 0f || flag3)
                                        {
                                            this.velocity.Y = -Player.jumpSpeed;
                                            this.jump = Player.jumpHeight;
                                        }
                                        else
                                        {
                                            //Main.PlaySound(16, (int)this.position.X, (int)this.position.Y, 1);
                                            this.velocity.Y = -Player.jumpSpeed;
                                            this.jump = Player.jumpHeight / 2;
                                            for (int num21 = 0; num21 < 10; num21++)
                                            {
                                                Vector2 arg_2064_0 = new Vector2(this.position.X - 34f, this.position.Y + (float)this.height - 16f);
                                                int arg_2064_1 = 102;
                                                int arg_2064_2 = 32;
                                                int arg_2064_3 = 16;
                                                float arg_2064_4 = -this.velocity.X * 0.5f;
                                                float arg_2064_5 = this.velocity.Y * 0.5f;
                                                int arg_2064_6 = 100;
                                                Color newColor = new Color();
                                                int num22 = Dust.NewDust(arg_2064_0, arg_2064_1, arg_2064_2, arg_2064_3, arg_2064_4, arg_2064_5, arg_2064_6, newColor, 1.5f);
                                                Main.dust[num22].velocity.X = Main.dust[num22].velocity.X * 0.5f - this.velocity.X * 0.1f;
                                                Main.dust[num22].velocity.Y = Main.dust[num22].velocity.Y * 0.5f - this.velocity.Y * 0.3f;
                                            }
                                            int num23 = Gore.NewGore(new Vector2(this.position.X + (float)(this.width / 2) - 16f, this.position.Y + (float)this.height - 16f), new Vector2(-this.velocity.X, -this.velocity.Y), Main.rand.Next(11, 14));
                                            Main.gore[num23].velocity.X = Main.gore[num23].velocity.X * 0.1f - this.velocity.X * 0.1f;
                                            Main.gore[num23].velocity.Y = Main.gore[num23].velocity.Y * 0.1f - this.velocity.Y * 0.05f;
                                            num23 = Gore.NewGore(new Vector2(this.position.X - 36f, this.position.Y + (float)this.height - 16f), new Vector2(-this.velocity.X, -this.velocity.Y), Main.rand.Next(11, 14));
                                            Main.gore[num23].velocity.X = Main.gore[num23].velocity.X * 0.1f - this.velocity.X * 0.1f;
                                            Main.gore[num23].velocity.Y = Main.gore[num23].velocity.Y * 0.1f - this.velocity.Y * 0.05f;
                                            num23 = Gore.NewGore(new Vector2(this.position.X + (float)this.width + 4f, this.position.Y + (float)this.height - 16f), new Vector2(-this.velocity.X, -this.velocity.Y), Main.rand.Next(11, 14));
                                            Main.gore[num23].velocity.X = Main.gore[num23].velocity.X * 0.1f - this.velocity.X * 0.1f;
                                            Main.gore[num23].velocity.Y = Main.gore[num23].velocity.Y * 0.1f - this.velocity.Y * 0.05f;
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
                            if (this.doubleJump && !this.jumpAgain && this.velocity.Y < 0f && !this.rocketBoots && !this.accFlipper)
                            {
                                Vector2 arg_2456_0 = new Vector2(this.position.X - 4f, this.position.Y + (float)this.height);
                                int arg_2456_1 = this.width + 8;
                                int arg_2456_2 = 4;
                                int arg_2456_3 = 16;
                                float arg_2456_4 = -this.velocity.X * 0.5f;
                                float arg_2456_5 = this.velocity.Y * 0.5f;
                                int arg_2456_6 = 100;
                                Color newColor = new Color();
                                int num24 = Dust.NewDust(arg_2456_0, arg_2456_1, arg_2456_2, arg_2456_3, arg_2456_4, arg_2456_5, arg_2456_6, newColor, 1.5f);
                                Main.dust[num24].velocity.X = Main.dust[num24].velocity.X * 0.5f - this.velocity.X * 0.1f;
                                Main.dust[num24].velocity.Y = Main.dust[num24].velocity.Y * 0.5f - this.velocity.Y * 0.3f;
                            }
                            if (this.velocity.Y > -Player.jumpSpeed && this.velocity.Y != 0f)
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
                                        //Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 13);
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
                                        Vector2 arg_2650_0 = new Vector2(this.position.X - 4f, this.position.Y + (float)this.height - 10f);
                                        int arg_2650_1 = 8;
                                        int arg_2650_2 = 8;
                                        int arg_2650_3 = 6;
                                        float arg_2650_4 = 0f;
                                        float arg_2650_5 = 0f;
                                        int arg_2650_6 = 100;
                                        Color newColor = new Color();
                                        int num27 = Dust.NewDust(arg_2650_0, arg_2650_1, arg_2650_2, arg_2650_3, arg_2650_4, arg_2650_5, arg_2650_6, newColor, 2.5f);
                                        Main.dust[num27].noGravity = true;
                                        Main.dust[num27].velocity.X = Main.dust[num27].velocity.X * 1f - 2f - this.velocity.X * 0.3f;
                                        Main.dust[num27].velocity.Y = Main.dust[num27].velocity.Y * 1f + 2f - this.velocity.Y * 0.3f;
                                    }
                                    else
                                    {
                                        Vector2 arg_2743_0 = new Vector2(this.position.X + (float)this.width - 4f, this.position.Y + (float)this.height - 10f);
                                        int arg_2743_1 = 8;
                                        int arg_2743_2 = 8;
                                        int arg_2743_3 = 6;
                                        float arg_2743_4 = 0f;
                                        float arg_2743_5 = 0f;
                                        int arg_2743_6 = 100;
                                        Color newColor = new Color();
                                        int num28 = Dust.NewDust(arg_2743_0, arg_2743_1, arg_2743_2, arg_2743_3, arg_2743_4, arg_2743_5, arg_2743_6, newColor, 2.5f);
                                        Main.dust[num28].noGravity = true;
                                        Main.dust[num28].velocity.X = Main.dust[num28].velocity.X * 1f + 2f - this.velocity.X * 0.3f;
                                        Main.dust[num28].velocity.Y = Main.dust[num28].velocity.Y * 1f + 2f - this.velocity.Y * 0.3f;
                                    }
                                }
                                if (this.rocketDelay == 0)
                                {
                                    this.releaseJump = true;
                                }
                                this.rocketDelay--;
                                this.velocity.Y = this.velocity.Y - 0.1f;
                                if (this.velocity.Y > 0f)
                                {
                                    this.velocity.Y = this.velocity.Y - 0.3f;
                                }
                                if (this.velocity.Y < -Player.jumpSpeed)
                                {
                                    this.velocity.Y = -Player.jumpSpeed;
                                }
                            }
                            else
                            {
                                this.velocity.Y = this.velocity.Y + num2;
                            }
                            if (this.velocity.Y > num)
                            {
                                this.velocity.Y = num;
                            }
                        }
                        for (int num29 = 0; num29 < 200; num29++)
                        {
                            if (Main.item[num29].active && Main.item[num29].noGrabDelay == 0 && Main.item[num29].owner == i)
                            {
                                Rectangle rectangle = new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height);
                                if (rectangle.Intersects(new Rectangle((int)Main.item[num29].position.X, (int)Main.item[num29].position.Y, Main.item[num29].width, Main.item[num29].height)))
                                {
                                    if (i == Main.myPlayer && (this.inventory[this.selectedItem].type != 0 || this.itemAnimation <= 0))
                                    {
                                        if (Main.item[num29].type == 58)
                                        {
                                            //Main.PlaySound(7, (int)this.position.X, (int)this.position.Y, 1);
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
                                                NetMessage.SendData(21, -1, -1, "", num29, 0f, 0f, 0f);
                                            }
                                        }
                                        else
                                        {
                                            if (Main.item[num29].type == 184)
                                            {
                                                //Main.PlaySound(7, (int)this.position.X, (int)this.position.Y, 1);
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
                                                    NetMessage.SendData(21, -1, -1, "", num29, 0f, 0f, 0f);
                                                }
                                            }
                                            else
                                            {
                                                Main.item[num29] = this.GetItem(i, Main.item[num29]);
                                                if (Main.netMode == 1)
                                                {
                                                    NetMessage.SendData(21, -1, -1, "", num29, 0f, 0f, 0f);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    rectangle = new Rectangle((int)this.position.X - Player.itemGrabRange, (int)this.position.Y - Player.itemGrabRange, this.width + Player.itemGrabRange * 2, this.height + Player.itemGrabRange * 2);
                                    if (rectangle.Intersects(new Rectangle((int)Main.item[num29].position.X, (int)Main.item[num29].position.Y, Main.item[num29].width, Main.item[num29].height)) && this.ItemSpace(Main.item[num29]))
                                    {
                                        Main.item[num29].beingGrabbed = true;
                                        if ((double)this.position.X + (double)this.width * 0.5 > (double)Main.item[num29].position.X + (double)Main.item[num29].width * 0.5)
                                        {
                                            if (Main.item[num29].velocity.X < Player.itemGrabSpeedMax + this.velocity.X)
                                            {
                                                Item expr_2C5D_cp_0 = Main.item[num29];
                                                expr_2C5D_cp_0.velocity.X = expr_2C5D_cp_0.velocity.X + Player.itemGrabSpeed;
                                            }
                                            if (Main.item[num29].velocity.X < 0f)
                                            {
                                                Item expr_2C97_cp_0 = Main.item[num29];
                                                expr_2C97_cp_0.velocity.X = expr_2C97_cp_0.velocity.X + Player.itemGrabSpeed * 0.75f;
                                            }
                                        }
                                        else
                                        {
                                            if (Main.item[num29].velocity.X > -Player.itemGrabSpeedMax + this.velocity.X)
                                            {
                                                Item expr_2CE6_cp_0 = Main.item[num29];
                                                expr_2CE6_cp_0.velocity.X = expr_2CE6_cp_0.velocity.X - Player.itemGrabSpeed;
                                            }
                                            if (Main.item[num29].velocity.X > 0f)
                                            {
                                                Item expr_2D1D_cp_0 = Main.item[num29];
                                                expr_2D1D_cp_0.velocity.X = expr_2D1D_cp_0.velocity.X - Player.itemGrabSpeed * 0.75f;
                                            }
                                        }
                                        if ((double)this.position.Y + (double)this.height * 0.5 > (double)Main.item[num29].position.Y + (double)Main.item[num29].height * 0.5)
                                        {
                                            if (Main.item[num29].velocity.Y < Player.itemGrabSpeedMax)
                                            {
                                                Item expr_2DA6_cp_0 = Main.item[num29];
                                                expr_2DA6_cp_0.velocity.Y = expr_2DA6_cp_0.velocity.Y + Player.itemGrabSpeed;
                                            }
                                            if (Main.item[num29].velocity.Y < 0f)
                                            {
                                                Item expr_2DE0_cp_0 = Main.item[num29];
                                                expr_2DE0_cp_0.velocity.Y = expr_2DE0_cp_0.velocity.Y + Player.itemGrabSpeed * 0.75f;
                                            }
                                        }
                                        else
                                        {
                                            if (Main.item[num29].velocity.Y > -Player.itemGrabSpeedMax)
                                            {
                                                Item expr_2E20_cp_0 = Main.item[num29];
                                                expr_2E20_cp_0.velocity.Y = expr_2E20_cp_0.velocity.Y - Player.itemGrabSpeed;
                                            }
                                            if (Main.item[num29].velocity.Y > 0f)
                                            {
                                                Item expr_2E57_cp_0 = Main.item[num29];
                                                expr_2E57_cp_0.velocity.Y = expr_2E57_cp_0.velocity.Y - Player.itemGrabSpeed * 0.75f;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (this.position.X / 16f - (float)Player.tileRangeX <= (float)Player.tileTargetX && (this.position.X + (float)this.width) / 16f + (float)Player.tileRangeX - 1f >= (float)Player.tileTargetX && this.position.Y / 16f - (float)Player.tileRangeY <= (float)Player.tileTargetY && (this.position.Y + (float)this.height) / 16f + (float)Player.tileRangeY - 2f >= (float)Player.tileTargetY && Main.tile[Player.tileTargetX, Player.tileTargetY].active)
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
                                            NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, 0f);
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
                                                //Main.NewText("Spawn point set!", 255, 240, 20);
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
                                                        //Main.PlaySound(11, -1, -1, 1);
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
                                                        //Main.PlaySound(10, -1, -1, 1);
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
                                                            NetMessage.SendData(46, -1, -1, "", num40, (float)num41, 0f, 0f);
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
                                                                    //Main.PlaySound(11, -1, -1, 1);
                                                                }
                                                                else
                                                                {
                                                                    NetMessage.SendData(31, -1, -1, "", num42, (float)num43, 0f, 0f);
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
                                                                        //Main.PlaySound(11, -1, -1, 1);
                                                                    }
                                                                    else
                                                                    {
                                                                        if (num44 != this.chest && this.chest == -1)
                                                                        {
                                                                            this.chest = num44;
                                                                            Main.playerInventory = true;
                                                                            //Main.PlaySound(10, -1, -1, 1);
                                                                            this.chestX = num42;
                                                                            this.chestY = num43;
                                                                        }
                                                                        else
                                                                        {
                                                                            this.chest = num44;
                                                                            Main.playerInventory = true;
                                                                            //Main.PlaySound(12, -1, -1, 1);
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
                                Rectangle rectangle2 = new Rectangle((int)(this.position.X + (float)(this.width / 2) - (float)(Player.tileRangeX * 16)), (int)(this.position.Y + (float)(this.height / 2) - (float)(Player.tileRangeY * 16)), Player.tileRangeX * 16 * 2, Player.tileRangeY * 16 * 2);
                                Rectangle value = new Rectangle((int)Main.npc[this.talkNPC].position.X, (int)Main.npc[this.talkNPC].position.Y, Main.npc[this.talkNPC].width, Main.npc[this.talkNPC].height);
                                if (!rectangle2.Intersects(value) || this.chest != -1 || !Main.npc[this.talkNPC].active)
                                {
                                    if (this.chest == -1)
                                    {
                                        //Main.PlaySound(11, -1, -1, 1);
                                    }
                                    this.talkNPC = -1;
                                    Main.npcChatText = "";
                                }
                            }
                            if (this.sign >= 0)
                            {
                                Rectangle rectangle3 = new Rectangle((int)(this.position.X + (float)(this.width / 2) - (float)(Player.tileRangeX * 16)), (int)(this.position.Y + (float)(this.height / 2) - (float)(Player.tileRangeY * 16)), Player.tileRangeX * 16 * 2, Player.tileRangeY * 16 * 2);
                                Rectangle value2 = new Rectangle(Main.sign[this.sign].x * 16, Main.sign[this.sign].y * 16, 32, 32);
                                if (!rectangle3.Intersects(value2))
                                {
                                    //Main.PlaySound(11, -1, -1, 1);
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
                                else
                                {
                                    //Main.npcChatText = Main.GetInputText(Main.npcChatText);
                                    //if (Main.inputTextEnter)
                                    //{
                                    //    byte[] bytes = new byte[]
                                    //    {
                                    //        10
                                    //    };
                                    //    Main.npcChatText += Encoding.ASCII.GetString(bytes);
                                    //}
                                }
                            }
                            Rectangle rectangle4 = new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height);
                            for (int num45 = 0; num45 < 1000; num45++)
                            {
                                if (Main.npc[num45].active && !Main.npc[num45].friendly && rectangle4.Intersects(new Rectangle((int)Main.npc[num45].position.X, (int)Main.npc[num45].position.Y, Main.npc[num45].width, Main.npc[num45].height)))
                                {
                                    int hitDirection = -1;
                                    if (Main.npc[num45].position.X + (float)(Main.npc[num45].width / 2) < this.position.X + (float)(this.width / 2))
                                    {
                                        hitDirection = 1;
                                    }
                                    this.Hurt(Main.npc[num45].damage, hitDirection, false, false);
                                }
                            }
                            Vector2 vector = Collision.HurtTiles(this.position, this.velocity, this.width, this.height, this.fireWalk);
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
                            this.fallStart = (int)(this.position.Y / 16f);
                            float num46 = 0f;
                            float num47 = 0f;
                            for (int num48 = 0; num48 < this.grapCount; num48++)
                            {
                                num46 += Main.projectile[this.grappling[num48]].position.X + (float)(Main.projectile[this.grappling[num48]].width / 2);
                                num47 += Main.projectile[this.grappling[num48]].position.Y + (float)(Main.projectile[this.grappling[num48]].height / 2);
                            }
                            num46 /= (float)this.grapCount;
                            num47 /= (float)this.grapCount;
                            Vector2 vector2 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
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
                            this.velocity.X = num49;
                            this.velocity.Y = num50;
                            if (this.itemAnimation == 0)
                            {
                                if (this.velocity.X > 0f)
                                {
                                    this.direction = 1;
                                }
                                if (this.velocity.X < 0f)
                                {
                                    this.direction = -1;
                                }
                            }
                            if (this.controlJump)
                            {
                                if (this.releaseJump)
                                {
                                    if (this.velocity.Y == 0f || (this.wet && (double)this.velocity.Y > -0.02 && (double)this.velocity.Y < 0.02))
                                    {
                                        this.velocity.Y = -Player.jumpSpeed;
                                        this.jump = Player.jumpHeight / 2;
                                        this.releaseJump = false;
                                    }
                                    else
                                    {
                                        this.velocity.Y = this.velocity.Y + 0.01f;
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
                                        if (Main.projectile[num54].active && Main.projectile[num54].owner == i && Main.projectile[num54].aiStyle == 7)
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
                        if (Collision.StickyTiles(this.position, this.velocity, this.width, this.height))
                        {
                            this.fallStart = (int)(this.position.Y / 16f);
                            this.jump = 0;
                            if (this.velocity.X > 1f)
                            {
                                this.velocity.X = 1f;
                            }
                            if (this.velocity.X < -1f)
                            {
                                this.velocity.X = -1f;
                            }
                            if (this.velocity.Y > 1f)
                            {
                                this.velocity.Y = 1f;
                            }
                            if (this.velocity.Y < -5f)
                            {
                                this.velocity.Y = -5f;
                            }
                            if ((double)this.velocity.X > 0.75 || (double)this.velocity.X < -0.75)
                            {
                                this.velocity.X = this.velocity.X * 0.85f;
                            }
                            else
                            {
                                this.velocity.X = this.velocity.X * 0.6f;
                            }
                            if (this.velocity.Y < 0f)
                            {
                                this.velocity.Y = this.velocity.Y * 0.96f;
                            }
                            else
                            {
                                this.velocity.Y = this.velocity.Y * 0.3f;
                            }
                        }
                        bool flag6 = Collision.DrownCollision(this.position, this.width, this.height);
                        if (this.inventory[this.selectedItem].type == 186)
                        {
                            try
                            {
                                int num55 = (int)((this.position.X + (float)(this.width / 2) + (float)(6 * this.direction)) / 16f);
                                int num56 = (int)((this.position.Y - 44f) / 16f);
                                if (Main.tile[num55, num56].liquid < 128)
                                {
                                    if (Main.tile[num55, num56] == null)
                                    {
                                        Main.tile[num55, num56] = new Tile();
                                    }
                                    if (!Main.tile[num55, num56].active || !Main.tileSolid[(int)Main.tile[num55, num56].type] || Main.tileSolidTop[(int)Main.tile[num55, num56].type])
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
                                if (this.inventory[this.selectedItem].type == 186)
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
                            if (this.inventory[this.selectedItem].type == 186)
                            {
                                Vector2 arg_4204_0 = new Vector2(this.position.X + (float)(10 * this.direction) + 4f, this.position.Y - 54f);
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
                                Vector2 arg_425D_0 = new Vector2(this.position.X + (float)(12 * this.direction), this.position.Y + 4f);
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
                        bool flag7 = Collision.LavaCollision(this.position, this.width, this.height);
                        if (flag7)
                        {
                            if (Main.myPlayer == i)
                            {
                                this.Hurt(100, 0, false, false);
                            }
                            this.lavaWet = true;
                        }
                        bool flag8 = Collision.WetCollision(this.position, this.width, this.height);
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
                                            Vector2 arg_4340_0 = new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f);
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
                                        //Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 0);
                                    }
                                    else
                                    {
                                        for (int num60 = 0; num60 < 20; num60++)
                                        {
                                            Vector2 arg_4446_0 = new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f);
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
                                        //Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
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
                                            Vector2 arg_4599_0 = new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2));
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
                                        //Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 0);
                                    }
                                    else
                                    {
                                        for (int num64 = 0; num64 < 20; num64++)
                                        {
                                            Vector2 arg_469F_0 = new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f);
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
                        if (this.wet)
                        {
                            if (this.wet)
                            {
                                Vector2 vector3 = this.velocity;
                                this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, this.controlDown, false);
                                Vector2 value3 = this.velocity * 0.5f;
                                if (this.velocity.X != vector3.X)
                                {
                                    value3.X = this.velocity.X;
                                }
                                if (this.velocity.Y != vector3.Y)
                                {
                                    value3.Y = this.velocity.Y;
                                }
                                this.position += value3;
                            }
                        }
                        else
                        {
                            this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, this.controlDown, false);
                            this.position += this.velocity;
                        }
                        if (this.position.X < Main.leftWorld + 336f + 16f)
                        {
                            this.position.X = Main.leftWorld + 336f + 16f;
                            this.velocity.X = 0f;
                        }
                        if (this.position.X + (float)this.width > Main.rightWorld - 336f - 32f)
                        {
                            this.position.X = Main.rightWorld - 336f - 32f - (float)this.width;
                            this.velocity.X = 0f;
                        }
                        if (this.position.Y < Main.topWorld + 336f + 16f)
                        {
                            this.position.Y = Main.topWorld + 336f + 16f;
                            if ((double)this.velocity.Y < -0.1)
                            {
                                this.velocity.Y = -0.1f;
                            }
                        }
                        if (this.position.Y > Main.bottomWorld - 336f - 32f - (float)this.height)
                        {
                            this.position.Y = Main.bottomWorld - 336f - 32f - (float)this.height;
                            this.velocity.Y = 0f;
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
                Console.WriteLine("Error In UpdatePlayer: " + e.Message);
                Console.WriteLine("Stack: " + e.StackTrace);
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
					if (num == -1 && (this.inventory[k].type == 0 || this.inventory[k].stack == 0))
					{
						num = k;
					}
					while (this.inventory[k].type == 74 && this.inventory[k].stack < this.inventory[k].maxStack && j >= 1000000)
					{
						this.inventory[k].stack++;
						j -= 1000000;
						this.DoCoins(k);
						if (this.inventory[k].stack == 0 && num == -1)
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
					if (num2 == -1 && (this.inventory[l].type == 0 || this.inventory[l].stack == 0))
					{
						num2 = l;
					}
					while (this.inventory[l].type == 73 && this.inventory[l].stack < this.inventory[l].maxStack && j >= 10000)
					{
						this.inventory[l].stack++;
						j -= 10000;
						this.DoCoins(l);
						if (this.inventory[l].stack == 0 && num2 == -1)
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
					if (num3 == -1 && (this.inventory[m].type == 0 || this.inventory[m].stack == 0))
					{
						num3 = m;
					}
					while (this.inventory[m].type == 72 && this.inventory[m].stack < this.inventory[m].maxStack && j >= 100)
					{
						this.inventory[m].stack++;
						j -= 100;
						this.DoCoins(m);
						if (this.inventory[m].stack == 0 && num3 == -1)
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
					if (num4 == -1 && (this.inventory[n].type == 0 || this.inventory[n].stack == 0))
					{
						num4 = n;
					}
					while (this.inventory[n].type == 71 && this.inventory[n].stack < this.inventory[n].maxStack && j >= 1)
					{
						this.inventory[n].stack++;
						j--;
						this.DoCoins(n);
						if (this.inventory[n].stack == 0 && num4 == -1)
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
				if (this.inventory[j].type == 71)
				{
					num += this.inventory[j].stack;
				}
				if (this.inventory[j].type == 72)
				{
					num += this.inventory[j].stack * 100;
				}
				if (this.inventory[j].type == 73)
				{
					num += this.inventory[j].stack * 10000;
				}
				if (this.inventory[j].type == 74)
				{
					num += this.inventory[j].stack * 1000000;
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
							if (this.inventory[k].type == 74)
							{
								while (this.inventory[k].stack > 0 && i >= 1000000)
								{
									i -= 1000000;
									this.inventory[k].stack--;
									if (this.inventory[k].stack == 0)
									{
										this.inventory[k].type = 0;
									}
								}
							}
						}
					}
					if (i >= 10000)
					{
						for (int l = 0; l < 44; l++)
						{
							if (this.inventory[l].type == 73)
							{
								while (this.inventory[l].stack > 0 && i >= 10000)
								{
									i -= 10000;
									this.inventory[l].stack--;
									if (this.inventory[l].stack == 0)
									{
										this.inventory[l].type = 0;
									}
								}
							}
						}
					}
					if (i >= 100)
					{
						for (int m = 0; m < 44; m++)
						{
							if (this.inventory[m].type == 72)
							{
								while (this.inventory[m].stack > 0 && i >= 100)
								{
									i -= 100;
									this.inventory[m].stack--;
									if (this.inventory[m].stack == 0)
									{
										this.inventory[m].type = 0;
									}
								}
							}
						}
					}
					if (i >= 1)
					{
						for (int n = 0; n < 44; n++)
						{
							if (this.inventory[n].type == 71)
							{
								while (this.inventory[n].stack > 0 && i >= 1)
								{
									i--;
									this.inventory[n].stack--;
									if (this.inventory[n].stack == 0)
									{
										this.inventory[n].type = 0;
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
							if (this.inventory[num3].type == 0 || this.inventory[num3].stack == 0)
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
								if (this.inventory[num5].type == 74 && this.inventory[num5].stack >= 1)
								{
									this.inventory[num5].stack--;
									if (this.inventory[num5].stack == 0)
									{
										this.inventory[num5].type = 0;
									}
									this.inventory[num2].SetDefaults(73);
									this.inventory[num2].stack = 100;
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
									if (this.inventory[num6].type == 73 && this.inventory[num6].stack >= 1)
									{
										this.inventory[num6].stack--;
										if (this.inventory[num6].stack == 0)
										{
											this.inventory[num6].type = 0;
										}
										this.inventory[num2].SetDefaults(72);
										this.inventory[num2].stack = 100;
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
										if (this.inventory[num7].type == 72 && this.inventory[num7].stack >= 1)
										{
											this.inventory[num7].stack--;
											if (this.inventory[num7].stack == 0)
											{
												this.inventory[num7].type = 0;
											}
											this.inventory[num2].SetDefaults(71);
											this.inventory[num2].stack = 100;
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
									if (this.inventory[num8].type == 73 && this.inventory[num8].stack >= 1)
									{
										this.inventory[num8].stack--;
										if (this.inventory[num8].stack == 0)
										{
											this.inventory[num8].type = 0;
										}
										this.inventory[num2].SetDefaults(72);
										this.inventory[num2].stack = 100;
										flag = false;
										break;
									}
								}
							}
							if (flag && i < 1000000)
							{
								for (int num9 = 0; num9 < 44; num9++)
								{
									if (this.inventory[num9].type == 74 && this.inventory[num9].stack >= 1)
									{
										this.inventory[num9].stack--;
										if (this.inventory[num9].stack == 0)
										{
											this.inventory[num9].type = 0;
										}
										this.inventory[num2].SetDefaults(73);
										this.inventory[num2].stack = 100;
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
			int num3 = (int)((this.position.X + (float)(this.width / 2)) / 16f);
			int num4 = (int)((this.position.Y + (float)this.height) / 16f);
			for (int j = num3 - num; j <= num3 + num; j++)
			{
				for (int k = num4 - num2; k < num4 + num2; k++)
				{
					if (Main.tile[j, k].active)
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
			this.head = this.armor[0].headSlot;
			this.body = this.armor[1].bodySlot;
			this.legs = this.armor[2].legSlot;
			this.bodyFrame.Width = 40;
			this.bodyFrame.Height = 56;
			this.legFrame.Width = 40;
			this.legFrame.Height = 56;
			this.bodyFrame.X = 0;
			this.legFrame.X = 0;
			if (this.itemAnimation > 0 && this.inventory[this.selectedItem].useStyle != 10)
			{
				if (this.inventory[this.selectedItem].useStyle == 1 || this.inventory[this.selectedItem].type == 0)
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
					if (this.inventory[this.selectedItem].useStyle == 2)
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
						if (this.inventory[this.selectedItem].useStyle == 3)
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
							if (this.inventory[this.selectedItem].useStyle == 4)
							{
								this.bodyFrame.Y = this.bodyFrame.Height * 2;
							}
							else
							{
								if (this.inventory[this.selectedItem].useStyle == 5)
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
				if (this.inventory[this.selectedItem].holdStyle == 1)
				{
					this.bodyFrame.Y = this.bodyFrame.Height * 3;
				}
				else
				{
					if (this.inventory[this.selectedItem].holdStyle == 2)
					{
						this.bodyFrame.Y = this.bodyFrame.Height * 2;
					}
					else
					{
						if (this.grappling[0] >= 0)
						{
							Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
							float num2 = 0f;
							float num3 = 0f;
							for (int i = 0; i < this.grapCount; i++)
							{
								num2 += Main.projectile[this.grappling[i]].position.X + (float)(Main.projectile[this.grappling[i]].width / 2);
								num3 += Main.projectile[this.grappling[i]].position.Y + (float)(Main.projectile[this.grappling[i]].height / 2);
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
								if (this.velocity.Y != 0f)
								{
									this.bodyFrameCounter = 0.0;
									this.bodyFrame.Y = this.bodyFrame.Height * 5;
								}
								else
								{
									if (this.velocity.X != 0f)
									{
										this.bodyFrameCounter += (double)Math.Abs(this.velocity.X) * 1.5;
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
				if (this.velocity.Y != 0f || this.grappling[0] > -1)
				{
					this.legFrameCounter = 0.0;
					this.legFrame.Y = this.legFrame.Height * 5;
					return;
				}
				if (this.velocity.X != 0f)
				{
					this.legFrameCounter += (double)Math.Abs(this.velocity.X) * 1.3;
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
				NetMessage.SendData(12, -1, -1, "", Main.myPlayer, 0f, 0f, 0f);
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
			this.active = true;
			if (this.SpawnX >= 0 && this.SpawnY >= 0)
			{
				this.position.X = (float)(this.SpawnX * 16 + 8 - this.width / 2);
				this.position.Y = (float)(this.SpawnY * 16 - this.height);
			}
			else
			{
				this.position.X = (float)(Main.spawnTileX * 16 + 8 - this.width / 2);
				this.position.Y = (float)(Main.spawnTileY * 16 - this.height);
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
			this.wet = Collision.WetCollision(this.position, this.width, this.height);
			this.wetCount = 0;
			this.lavaWet = false;
			this.fallStart = (int)(this.position.Y / 16f);
			this.velocity.X = 0f;
			this.velocity.Y = 0f;
			this.talkNPC = -1;
			if (this.pvpDeath)
			{
				this.pvpDeath = false;
				this.immuneTime = 300;
				this.statLife = this.statLifeMax;
			}
			if (this.whoAmi == Main.myPlayer)
			{
				//Lighting.lightCounter = //Lighting.lightSkip + 1;
				Main.screenPosition.X = this.position.X + (float)(this.width / 2) - (float)(Main.screenWidth / 2);
				Main.screenPosition.Y = this.position.Y + (float)(this.height / 2) - (float)(Main.screenHeight / 2);
			}
		}
		
        public double Hurt(int Damage, int hitDirection, bool pvp = false, bool quiet = false)
		{
            if (!this.immune && !Main.godMode)
			{
				int num = Damage;

                PlayerHurtEvent playerEvent = new PlayerHurtEvent();
                playerEvent.setSender(new Sender());
                playerEvent.setDamage(Damage);
                Program.server.getPluginManager().processHook(Hooks.CONSOLE_COMMAND, playerEvent);
                if (playerEvent.getCancelled())
                {
                    return 0.0;
                }

                num = playerEvent.getDamage();

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
						NetMessage.SendData(13, -1, -1, "", this.whoAmi, 0f, 0f, 0f);
						NetMessage.SendData(16, -1, -1, "", this.whoAmi, 0f, 0f, 0f);
						NetMessage.SendData(26, -1, -1, "", this.whoAmi, (float)hitDirection, (float)Damage, (float)num3);
					}
					//CombatText.NewText(new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height), new Color(255, 80, 90, 255), string.Concat((int)num2));
					this.statLife -= (int)num2;
					this.immune = true;
					this.immuneTime = 40;
					if (pvp)
					{
						this.immuneTime = 8;
					}
					if (!this.noKnockback && hitDirection != 0)
					{
						this.velocity.X = 4.5f * (float)hitDirection;
						this.velocity.Y = -3.5f;
                    }
                    //if (this.boneArmor)
                    //{
                    //    //Main.PlaySound(3, (int)this.position.X, (int)this.position.Y, 2);
                    //}
                    //else
                    //{
                    //    if (this.hair == 5 || this.hair == 6 || this.hair == 9 || this.hair == 11)
                    //    {
                    //        //Main.PlaySound(20, (int)this.position.X, (int)this.position.Y, 1);
                    //    }
                    //    else
                    //    {
                    //        //Main.PlaySound(1, (int)this.position.X, (int)this.position.Y, 1);
                    //    }
                    //}
					if (this.statLife > 0)
					{
						int num4 = 0;
						while ((double)num4 < num2 / (double)this.statLifeMax * 100.0)
						{
							if (this.boneArmor)
							{
								Dust.NewDust(this.position, this.width, this.height, 26, (float)(2 * hitDirection), -2f, 0,  new Color(), 1f);
							}
							else
							{
								Dust.NewDust(this.position, this.width, this.height, 5, (float)(2 * hitDirection), -2f, 0,  new Color(), 1f);
							}
							num4++;
						}
					}
					else
					{
						this.statLife = 0;
						if (this.whoAmi == Main.myPlayer)
						{
							this.KillMe(num2, hitDirection, pvp);
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
		
        public void KillMe(double dmg, int hitDirection, bool pvp = false)
		{
			if (Main.godMode && Main.myPlayer == this.whoAmi)
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
			//Main.PlaySound(5, (int)this.position.X, (int)this.position.Y, 1);
			this.headVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			this.bodyVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			this.legVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
			this.headVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			this.bodyVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			this.legVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
			int num = 0;
			while ((double)num < 20.0 + dmg / (double)this.statLifeMax * 100.0)
			{
				if (this.boneArmor)
				{
					Dust.NewDust(this.position, this.width, this.height, 26, (float)(2 * hitDirection), -2f, 0,  new Color(), 1f);
				}
				else
				{
					Dust.NewDust(this.position, this.width, this.height, 5, (float)(2 * hitDirection), -2f, 0,  new Color(), 1f);
				}
				num++;
			}
			this.dead = true;
			this.respawnTimer = 600;
			this.immuneAlpha = 0;
			if (Main.netMode == 2)
			{
				NetMessage.SendData(25, -1, -1, this.name + " was slain...", 255, 225f, 25f, 25f);
			}
			if (this.whoAmi == Main.myPlayer)
			{
				WorldGen.saveToonWhilePlaying();
			}
			if (Main.netMode == 1 && this.whoAmi == Main.myPlayer)
			{
				int num2 = 0;
				if (pvp)
				{
					num2 = 1;
				}
				NetMessage.SendData(44, -1, -1, "", this.whoAmi, (float)hitDirection, (float)((int)dmg), (float)num2);
			}
			if (!pvp && this.whoAmi == Main.myPlayer)
			{
				this.DropItems();
			}
		}
		
        public bool ItemSpace(Item newItem)
		{
			if (newItem.type == 58)
			{
				return true;
			}
			if (newItem.type == 184)
			{
				return true;
			}
			int num = 40;
			if (newItem.type == 71 || newItem.type == 72 || newItem.type == 73 || newItem.type == 74)
			{
				num = 44;
			}
			for (int i = 0; i < num; i++)
			{
				if (this.inventory[i].type == 0)
				{
					return true;
				}
			}
			for (int j = 0; j < num; j++)
			{
				if (this.inventory[j].type > 0 && this.inventory[j].stack < this.inventory[j].maxStack && newItem.IsTheSameAs(this.inventory[j]))
				{
					return true;
				}
			}
			return false;
		}
		
        public void DoCoins(int i)
		{
			if (this.inventory[i].stack == 100 && (this.inventory[i].type == 71 || this.inventory[i].type == 72 || this.inventory[i].type == 73))
			{
				this.inventory[i].SetDefaults(this.inventory[i].type + 1);
				for (int j = 0; j < 44; j++)
				{
					if (this.inventory[j].IsTheSameAs(this.inventory[i]) && j != i && this.inventory[j].stack < this.inventory[j].maxStack)
					{
						this.inventory[j].stack++;
						this.inventory[i].SetDefaults("");
						this.inventory[i].active = false;
						this.inventory[i].name = "";
						this.inventory[i].type = 0;
						this.inventory[i].stack = 0;
						this.DoCoins(j);
					}
				}
			}
		}
		
        public Item GetItem(int plr, Item newItem)
		{
			if (newItem.noGrabDelay > 0)
			{
				return newItem;
			}
			int num = 0;
			if (newItem.type == 71 || newItem.type == 72 || newItem.type == 73 || newItem.type == 74)
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
				if (this.inventory[num2].type > 0 && this.inventory[num2].stack < this.inventory[num2].maxStack && newItem.IsTheSameAs(this.inventory[num2]))
				{
					//Main.PlaySound(7, (int)this.position.X, (int)this.position.Y, 1);
					if (newItem.stack + this.inventory[num2].stack <= this.inventory[num2].maxStack)
					{
						this.inventory[num2].stack += newItem.stack;
						this.DoCoins(num2);
						if (plr == Main.myPlayer)
						{
							Recipe.FindRecipes();
						}
						return new Item();
					}
					newItem.stack -= this.inventory[num2].maxStack - this.inventory[num2].stack;
					this.inventory[num2].stack = this.inventory[num2].maxStack;
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
				if (this.inventory[num3].type == 0)
				{
					this.inventory[num3] = newItem;
					this.DoCoins(num3);
					//Main.PlaySound(7, (int)this.position.X, (int)this.position.Y, 1);
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
			if (this.inventory[this.selectedItem].autoReuse)
			{
				this.releaseUseItem = true;
				if (this.itemAnimation == 1 && this.inventory[this.selectedItem].stack > 0)
				{
					this.itemAnimation = 0;
				}
			}
			if (this.controlUseItem && this.itemAnimation == 0 && this.releaseUseItem && this.inventory[this.selectedItem].useStyle > 0)
			{
				bool flag = true;
				if (this.inventory[this.selectedItem].shoot == 6 || this.inventory[this.selectedItem].shoot == 19 || this.inventory[this.selectedItem].shoot == 33)
				{
					for (int j = 0; j < 1000; j++)
					{
						if (Main.projectile[j].active && Main.projectile[j].owner == Main.myPlayer && Main.projectile[j].type == this.inventory[this.selectedItem].shoot)
						{
							flag = false;
						}
					}
				}
				if (this.inventory[this.selectedItem].potion)
				{
					if (this.potionDelay <= 0)
					{
						this.potionDelay = Item.potionDelay;
					}
					else
					{
						flag = false;
					}
				}
				if (this.inventory[this.selectedItem].mana > 0 && (this.inventory[this.selectedItem].type != 127 || !this.spaceGun))
				{
					if (this.statMana >= (int)((float)this.inventory[this.selectedItem].mana * this.manaCost))
					{
						this.statMana -= (int)((float)this.inventory[this.selectedItem].mana * this.manaCost);
					}
					else
					{
						flag = false;
					}
				}
				if (this.inventory[this.selectedItem].type == 43 && Main.dayTime)
				{
					flag = false;
				}
				if (this.inventory[this.selectedItem].type == 70 && !this.zoneEvil)
				{
					flag = false;
				}
				if (this.inventory[this.selectedItem].shoot == 17 && flag && i == Main.myPlayer)
				{
                    //int num = (int)((float)Main.mouseState.X + Main.screenPosition.X) / 16;
                    //int num2 = (int)((float)Main.mouseState.Y + Main.screenPosition.Y) / 16;
                    //if (Main.tile[num, num2].active && (Main.tile[num, num2].type == 0 || Main.tile[num, num2].type == 2 || Main.tile[num, num2].type == 23))
                    //{
                    //    WorldGen.KillTile(num, num2, false, false, true);
                    //    if (!Main.tile[num, num2].active)
                    //    {
                    //        if (Main.netMode == 1)
                    //        {
                    //            NetMessage.SendData(17, -1, -1, "", 4, (float)num, (float)num2, 0f);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        flag = false;
                    //    }
                    //}
                    //else
                    //{
                    //    flag = false;
                    //}
				}
				if (flag && this.inventory[this.selectedItem].useAmmo > 0)
				{
					flag = false;
					for (int k = 0; k < 44; k++)
					{
						if (this.inventory[k].ammo == this.inventory[this.selectedItem].useAmmo && this.inventory[k].stack > 0)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					if (this.grappling[0] > -1)
					{
						if (this.controlRight)
						{
							this.direction = 1;
						}
						else
						{
							if (this.controlLeft)
							{
								this.direction = -1;
							}
						}
					}
					this.channel = this.inventory[this.selectedItem].channel;
					this.attackCD = 0;
					if (this.inventory[this.selectedItem].shoot > 0 || this.inventory[this.selectedItem].damage == 0)
					{
						this.meleeSpeed = 1f;
					}
					this.itemAnimation = (int)((float)this.inventory[this.selectedItem].useAnimation * this.meleeSpeed);
					this.itemAnimationMax = (int)((float)this.inventory[this.selectedItem].useAnimation * this.meleeSpeed);
					if (this.inventory[this.selectedItem].useSound > 0)
					{
						//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, this.inventory[this.selectedItem].useSound);
					}
				}
				if (flag && this.inventory[this.selectedItem].shoot == 18)
				{
					for (int l = 0; l < 1000; l++)
					{
						if (Main.projectile[l].active && Main.projectile[l].owner == i && Main.projectile[l].type == this.inventory[this.selectedItem].shoot)
						{
							Main.projectile[l].Kill();
						}
					}
				}
			}
			if (!this.controlUseItem)
			{
				this.channel = false;
			}
			if (!Main.dedServ)
			{
				if (this.itemAnimation > 0)
				{
					if (this.inventory[this.selectedItem].mana > 0)
					{
						this.manaRegenDelay = 180;
					}
					if (Main.dedServ)
					{
						this.itemHeight = this.inventory[this.selectedItem].height;
						this.itemWidth = this.inventory[this.selectedItem].width;
					}
                    //else
                    //{
                    //    this.itemHeight = Main.itemTexture[this.inventory[this.selectedItem].type].Height;
                    //    this.itemWidth = Main.itemTexture[this.inventory[this.selectedItem].type].Width;
                    //}
					this.itemAnimation--;
                    //if (this.inventory[this.selectedItem].useStyle == 1)
                    //{
                    //    if ((double)this.itemAnimation < (double)this.itemAnimationMax * 0.333)
                    //    {
                    //        float num3 = 10f;
                    //        if (Main.itemTexture[this.inventory[this.selectedItem].type].Width > 32)
                    //        {
                    //            num3 = 14f;
                    //        //}
                    //        this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - num3) * (float)this.direction;
                    //        this.itemLocation.Y = this.position.Y + 24f;
                    //    }
                    //    else
                    //    {
                    //        if ((double)this.itemAnimation < (double)this.itemAnimationMax * 0.666)
                    //        {
                    //            float num4 = 10f;
                    //            if (Main.itemTexture[this.inventory[this.selectedItem].type].Width > 32)
                    //            {
                    //                num4 = 18f;
                    //            }
                    //            this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - num4) * (float)this.direction;
                    //            num4 = 10f;
                    //            if (Main.itemTexture[this.inventory[this.selectedItem].type].Height > 32)
                    //            {
                    //                num4 = 8f;
                    //            }
                    //            this.itemLocation.Y = this.position.Y + num4;
                    //        }
                    //        else
                    //        {
                    //            float num5 = 6f;
                    //            if (Main.itemTexture[this.inventory[this.selectedItem].type].Width > 32)
                    //            {
                    //                num5 = 14f;
                    //            }
                    //            this.itemLocation.X = this.position.X + (float)this.width * 0.5f - ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - num5) * (float)this.direction;
                    //            num5 = 10f;
                    //            if (Main.itemTexture[this.inventory[this.selectedItem].type].Height > 32)
                    //            {
                    //                num5 = 10f;
                    //            }
                    //            this.itemLocation.Y = this.position.Y + num5;
                    //        }
                    //    }
                    //    this.itemRotation = ((float)this.itemAnimation / (float)this.itemAnimationMax - 0.5f) * (float)(-(float)this.direction) * 3.5f - (float)this.direction * 0.3f;
                    //}
                    //else
                    //{
                    //    if (this.inventory[this.selectedItem].useStyle == 2)
                    //    {
                    //        this.itemRotation = (float)this.itemAnimation / (float)this.itemAnimationMax * (float)this.direction * 2f + -1.4f * (float)this.direction;
                    //        if ((double)this.itemAnimation < (double)this.itemAnimationMax * 0.5)
                    //        {
                    //            this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - 9f - this.itemRotation * 12f * (float)this.direction) * (float)this.direction;
                    //            this.itemLocation.Y = this.position.Y + 38f + this.itemRotation * (float)this.direction * 4f;
                    //        }
                    //        else
                    //        {
                    //            this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - 9f - this.itemRotation * 16f * (float)this.direction) * (float)this.direction;
                    //            this.itemLocation.Y = this.position.Y + 38f + this.itemRotation * (float)this.direction;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (this.inventory[this.selectedItem].useStyle == 3)
                    //        {
                    //            if ((double)this.itemAnimation > (double)this.itemAnimationMax * 0.666)
                    //            {
                    //                this.itemLocation.X = -1000f;
                    //                this.itemLocation.Y = -1000f;
                    //                this.itemRotation = -1.3f * (float)this.direction;
                    //            }
                    //            else
                    //            {
                    //                this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - 4f) * (float)this.direction;
                    //                this.itemLocation.Y = this.position.Y + 24f;
                    //                float num6 = (float)this.itemAnimation / (float)this.itemAnimationMax * (float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * (float)this.direction * this.inventory[this.selectedItem].scale * 1.2f - (float)(10 * this.direction);
                    //                if (num6 > -4f && this.direction == -1)
                    //                {
                    //                    num6 = -8f;
                    //                }
                    //                if (num6 < 4f && this.direction == 1)
                    //                {
                    //                    num6 = 8f;
                    //                }
                    //                this.itemLocation.X = this.itemLocation.X - num6;
                    //                this.itemRotation = 0.8f * (float)this.direction;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //if (this.inventory[this.selectedItem].useStyle == 4)
                    //            //{
                    //            //    this.itemRotation = 0f;
                    //            //    this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - 9f - this.itemRotation * 14f * (float)this.direction) * (float)this.direction;
                    //            //    this.itemLocation.Y = this.position.Y + (float)Main.itemTexture[this.inventory[this.selectedItem].type].Height * 0.5f;
                    //            //}
                    //            //else
                    //            //{
                    //            //    if (this.inventory[this.selectedItem].useStyle == 5)
                    //            //    {
                    //            //        this.itemLocation.X = this.position.X + (float)this.width * 0.5f - (float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - (float)(this.direction * 2);
                    //            //        this.itemLocation.Y = this.position.Y + (float)this.height * 0.5f - (float)Main.itemTexture[this.inventory[this.selectedItem].type].Height * 0.5f;
                    //            //    }
                    //            //}
                    //        }
                    //    }
                    //}
				}
				else
				{
					if (this.inventory[this.selectedItem].holdStyle == 1)
					{
                        //this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f + 4f) * (float)this.direction;
                        //this.itemLocation.Y = this.position.Y + 24f;
                        //this.itemRotation = 0f;
					}
					else
					{
						if (this.inventory[this.selectedItem].holdStyle == 2)
						{
							this.itemLocation.X = this.position.X + (float)this.width * 0.5f + (float)(6 * this.direction);
							this.itemLocation.Y = this.position.Y + 16f;
							this.itemRotation = 0.79f * (float)(-(float)this.direction);
						}
					}
				}
			}
			if (this.inventory[this.selectedItem].type == 8)
			{
				int maxValue = 20;
				if (this.itemAnimation > 0)
				{
					maxValue = 7;
				}
				if (this.direction == -1)
				{
					if (Main.rand.Next(maxValue) == 0)
					{
						Vector2 arg_ECC_0 = new Vector2(this.itemLocation.X - 16f, this.itemLocation.Y - 14f);
						int arg_ECC_1 = 4;
						int arg_ECC_2 = 4;
						int arg_ECC_3 = 6;
						float arg_ECC_4 = 0f;
						float arg_ECC_5 = 0f;
						int arg_ECC_6 = 100;
						Color newColor =  new Color();
						Dust.NewDust(arg_ECC_0, arg_ECC_1, arg_ECC_2, arg_ECC_3, arg_ECC_4, arg_ECC_5, arg_ECC_6, newColor, 1f);
					}
					//Lighting.addLight((int)((this.itemLocation.X - 16f + this.velocity.X) / 16f), (int)((this.itemLocation.Y - 14f) / 16f), 1f);
				}
				else
				{
					if (Main.rand.Next(maxValue) == 0)
					{
						Vector2 arg_F70_0 = new Vector2(this.itemLocation.X + 6f, this.itemLocation.Y - 14f);
						int arg_F70_1 = 4;
						int arg_F70_2 = 4;
						int arg_F70_3 = 6;
						float arg_F70_4 = 0f;
						float arg_F70_5 = 0f;
						int arg_F70_6 = 100;
						Color newColor =  new Color();
						Dust.NewDust(arg_F70_0, arg_F70_1, arg_F70_2, arg_F70_3, arg_F70_4, arg_F70_5, arg_F70_6, newColor, 1f);
					}
					//Lighting.addLight((int)((this.itemLocation.X + 6f + this.velocity.X) / 16f), (int)((this.itemLocation.Y - 14f) / 16f), 1f);
				}
			}
			else
			{
				if (this.inventory[this.selectedItem].type == 105)
				{
					int maxValue2 = 20;
					if (this.itemAnimation > 0)
					{
						maxValue2 = 7;
					}
					if (this.direction == -1)
					{
						if (Main.rand.Next(maxValue2) == 0)
						{
							Vector2 arg_1049_0 = new Vector2(this.itemLocation.X - 12f, this.itemLocation.Y - 20f);
							int arg_1049_1 = 4;
							int arg_1049_2 = 4;
							int arg_1049_3 = 6;
							float arg_1049_4 = 0f;
							float arg_1049_5 = 0f;
							int arg_1049_6 = 100;
							Color newColor =  new Color();
							Dust.NewDust(arg_1049_0, arg_1049_1, arg_1049_2, arg_1049_3, arg_1049_4, arg_1049_5, arg_1049_6, newColor, 1f);
						}
						//Lighting.addLight((int)((this.itemLocation.X - 16f + this.velocity.X) / 16f), (int)((this.itemLocation.Y - 14f) / 16f), 1f);
					}
					else
					{
						if (Main.rand.Next(maxValue2) == 0)
						{
							Vector2 arg_10ED_0 = new Vector2(this.itemLocation.X + 4f, this.itemLocation.Y - 20f);
							int arg_10ED_1 = 4;
							int arg_10ED_2 = 4;
							int arg_10ED_3 = 6;
							float arg_10ED_4 = 0f;
							float arg_10ED_5 = 0f;
							int arg_10ED_6 = 100;
							Color newColor =  new Color();
							Dust.NewDust(arg_10ED_0, arg_10ED_1, arg_10ED_2, arg_10ED_3, arg_10ED_4, arg_10ED_5, arg_10ED_6, newColor, 1f);
						}
						//Lighting.addLight((int)((this.itemLocation.X + 6f + this.velocity.X) / 16f), (int)((this.itemLocation.Y - 14f) / 16f), 1f);
					}
				}
				else
				{
					if (this.inventory[this.selectedItem].type == 148)
					{
						int maxValue3 = 10;
						if (this.itemAnimation > 0)
						{
							maxValue3 = 7;
						}
						if (this.direction == -1)
						{
							if (Main.rand.Next(maxValue3) == 0)
							{
								Vector2 arg_11CA_0 = new Vector2(this.itemLocation.X - 12f, this.itemLocation.Y - 20f);
								int arg_11CA_1 = 4;
								int arg_11CA_2 = 4;
								int arg_11CA_3 = 29;
								float arg_11CA_4 = 0f;
								float arg_11CA_5 = 0f;
								int arg_11CA_6 = 100;
								Color newColor =  new Color();
								Dust.NewDust(arg_11CA_0, arg_11CA_1, arg_11CA_2, arg_11CA_3, arg_11CA_4, arg_11CA_5, arg_11CA_6, newColor, 1f);
							}
							//Lighting.addLight((int)((this.itemLocation.X - 16f + this.velocity.X) / 16f), (int)((this.itemLocation.Y - 14f) / 16f), 1f);
						}
						else
						{
							if (Main.rand.Next(maxValue3) == 0)
							{
								Vector2 arg_126F_0 = new Vector2(this.itemLocation.X + 4f, this.itemLocation.Y - 20f);
								int arg_126F_1 = 4;
								int arg_126F_2 = 4;
								int arg_126F_3 = 29;
								float arg_126F_4 = 0f;
								float arg_126F_5 = 0f;
								int arg_126F_6 = 100;
								Color newColor =  new Color();
								Dust.NewDust(arg_126F_0, arg_126F_1, arg_126F_2, arg_126F_3, arg_126F_4, arg_126F_5, arg_126F_6, newColor, 1f);
							}
							//Lighting.addLight((int)((this.itemLocation.X + 6f + this.velocity.X) / 16f), (int)((this.itemLocation.Y - 14f) / 16f), 1f);
						}
					}
				}
			}
			if (this.controlUseItem)
			{
				this.releaseUseItem = false;
			}
			else
			{
				this.releaseUseItem = true;
			}
			if (this.itemTime > 0)
			{
				this.itemTime--;
			}
			if (i == Main.myPlayer)
			{
				if (this.inventory[this.selectedItem].shoot > 0 && this.itemAnimation > 0 && this.itemTime == 0)
				{
					int num7 = this.inventory[this.selectedItem].shoot;
					float num8 = this.inventory[this.selectedItem].shootSpeed;
					bool flag2 = false;
					int num9 = this.inventory[this.selectedItem].damage;
					float num10 = this.inventory[this.selectedItem].knockBack;
					if (num7 == 13 || num7 == 32)
					{
						this.grappling[0] = -1;
						this.grapCount = 0;
						for (int m = 0; m < 1000; m++)
						{
							if (Main.projectile[m].active && Main.projectile[m].owner == i && Main.projectile[m].type == 13)
							{
								Main.projectile[m].Kill();
							}
						}
					}
					if (this.inventory[this.selectedItem].useAmmo > 0)
					{
						for (int n = 0; n < 44; n++)
						{
							if (this.inventory[n].ammo == this.inventory[this.selectedItem].useAmmo && this.inventory[n].stack > 0)
							{
								if (this.inventory[n].shoot > 0)
								{
									num7 = this.inventory[n].shoot;
								}
								num8 += this.inventory[n].shootSpeed;
								num9 += this.inventory[n].damage;
								num10 += this.inventory[n].knockBack;
								this.inventory[n].stack--;
								if (this.inventory[n].stack <= 0)
								{
									this.inventory[n].active = false;
									this.inventory[n].name = "";
									this.inventory[n].type = 0;
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
					if (num7 == 9 && (double)this.position.Y > Main.worldSurface * 16.0 + (double)(Main.screenHeight / 2))
					{
						flag2 = false;
					}
					if (flag2)
					{
                        //if (this.inventory[this.selectedItem].mana > 0)
                        //{
                        //    num9 = (int)Math.Round((double)((float)num9 * this.magicBoost));
                        //}
                        //if (num7 == 1 && this.inventory[this.selectedItem].type == 120)
                        //{
                        //    num7 = 2;
                        //}
                        //this.itemTime = this.inventory[this.selectedItem].useTime;
                        //if ((float)Main.mouseState.X + Main.screenPosition.X > this.position.X + (float)this.width * 0.5f)
                        //{
                        //    this.direction = 1;
                        //}
                        //else
                        //{
                        //    this.direction = -1;
                        //}
                        //Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                        //if (num7 == 9)
                        //{
                        //    vector = new Vector2(this.position.X + (float)this.width * 0.5f + (float)(Main.rand.Next(601) * -(float)this.direction), this.position.Y + (float)this.height * 0.5f - 300f - (float)Main.rand.Next(100));
                        //    num10 = 0f;
                        //}
                        //float num11 = (float)Main.mouseState.X + Main.screenPosition.X - vector.X;
                        //float num12 = (float)Main.mouseState.Y + Main.screenPosition.Y - vector.Y;
                        //float num13 = (float)Math.Sqrt((double)(num11 * num11 + num12 * num12));
                        //num13 = num8 / num13;
                        //num11 *= num13;
                        //num12 *= num13;
                        //if (num7 == 12)
                        //{
                        //    vector.X += num11 * 3f;
                        //    vector.Y += num12 * 3f;
                        //}
                        //if (this.inventory[this.selectedItem].useStyle == 5)
                        //{
                        //    this.itemRotation = (float)Math.Atan2((double)(num12 * (float)this.direction), (double)(num11 * (float)this.direction));
                        //    NetMessage.SendData(13, -1, -1, "", this.whoAmi, 0f, 0f, 0f);
                        //    NetMessage.SendData(41, -1, -1, "", this.whoAmi, 0f, 0f, 0f);
                        //}
                        //if (num7 == 17)
                        //{
                        //    vector.X = (float)Main.mouseState.X + Main.screenPosition.X;
                        //    vector.Y = (float)Main.mouseState.Y + Main.screenPosition.Y;
                        //}
                        //Projectile.NewProjectile(vector.X, vector.Y, num11, num12, num7, num9, num10, i);
					}
					else
					{
						if (this.inventory[this.selectedItem].useStyle == 5)
						{
							this.itemRotation = 0f;
							NetMessage.SendData(41, -1, -1, "", this.whoAmi, 0f, 0f, 0f);
						}
					}
				}
				if (this.inventory[this.selectedItem].type >= 205 && this.inventory[this.selectedItem].type <= 207 && this.position.X / 16f - (float)Player.tileRangeX - (float)this.inventory[this.selectedItem].tileBoost <= (float)Player.tileTargetX && (this.position.X + (float)this.width) / 16f + (float)Player.tileRangeX + (float)this.inventory[this.selectedItem].tileBoost - 1f >= (float)Player.tileTargetX && this.position.Y / 16f - (float)Player.tileRangeY - (float)this.inventory[this.selectedItem].tileBoost <= (float)Player.tileTargetY && (this.position.Y + (float)this.height) / 16f + (float)Player.tileRangeY + (float)this.inventory[this.selectedItem].tileBoost - 2f >= (float)Player.tileTargetY)
				{
					this.showItemIcon = true;
					if (this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
					{
						if (this.inventory[this.selectedItem].type == 205)
						{
							bool lava = Main.tile[Player.tileTargetX, Player.tileTargetY].lava;
							int num14 = 0;
							for (int num15 = Player.tileTargetX - 1; num15 <= Player.tileTargetX + 1; num15++)
							{
								for (int num16 = Player.tileTargetY - 1; num16 <= Player.tileTargetY + 1; num16++)
								{
									if (Main.tile[num15, num16].lava == lava)
									{
										num14 += (int)Main.tile[num15, num16].liquid;
									}
								}
							}
							if (Main.tile[Player.tileTargetX, Player.tileTargetY].liquid > 0 && num14 > 100)
							{
								bool lava2 = Main.tile[Player.tileTargetX, Player.tileTargetY].lava;
								if (!Main.tile[Player.tileTargetX, Player.tileTargetY].lava)
								{
									this.inventory[this.selectedItem].SetDefaults(206);
								}
								else
								{
									this.inventory[this.selectedItem].SetDefaults(207);
								}
								//Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
								this.itemTime = this.inventory[this.selectedItem].useTime;
								int num17 = (int)Main.tile[Player.tileTargetX, Player.tileTargetY].liquid;
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
								for (int num18 = Player.tileTargetX - 1; num18 <= Player.tileTargetX + 1; num18++)
								{
									for (int num19 = Player.tileTargetY - 1; num19 <= Player.tileTargetY + 1; num19++)
									{
										if (num17 < 256 && Main.tile[num18, num19].lava == lava)
										{
											int num20 = (int)Main.tile[num18, num19].liquid;
											if (num20 + num17 > 255)
											{
												num20 = 255 - num17;
											}
											num17 += num20;
											Tile expr_1C0E = Main.tile[num18, num19];
											expr_1C0E.liquid -= (byte)num20;
											Main.tile[num18, num19].lava = lava2;
											if (Main.tile[num18, num19].liquid == 0)
											{
												Main.tile[num18, num19].lava = false;
											}
											WorldGen.SquareTileFrame(num18, num19, false);
											if (Main.netMode == 1)
											{
												NetMessage.sendWater(num18, num19);
											}
											else
											{
												Liquid.AddWater(num18, num19);
											}
										}
									}
								}
							}
						}
						else
						{
							if (Main.tile[Player.tileTargetX, Player.tileTargetY].liquid < 200 && (!Main.tile[Player.tileTargetX, Player.tileTargetY].active || !Main.tileSolid[(int)Main.tile[Player.tileTargetX, Player.tileTargetY].type] || !Main.tileSolidTop[(int)Main.tile[Player.tileTargetX, Player.tileTargetY].type]))
							{
								if (this.inventory[this.selectedItem].type == 207)
								{
									if (Main.tile[Player.tileTargetX, Player.tileTargetY].liquid == 0 || Main.tile[Player.tileTargetX, Player.tileTargetY].lava)
									{
										//Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
										Main.tile[Player.tileTargetX, Player.tileTargetY].lava = true;
										Main.tile[Player.tileTargetX, Player.tileTargetY].liquid = 255;
										WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY, true);
										this.inventory[this.selectedItem].SetDefaults(205);
										this.itemTime = this.inventory[this.selectedItem].useTime;
										if (Main.netMode == 1)
										{
											NetMessage.sendWater(Player.tileTargetX, Player.tileTargetY);
										}
									}
								}
								else
								{
									if (Main.tile[Player.tileTargetX, Player.tileTargetY].liquid == 0 || !Main.tile[Player.tileTargetX, Player.tileTargetY].lava)
									{
										//Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
										Main.tile[Player.tileTargetX, Player.tileTargetY].lava = false;
										Main.tile[Player.tileTargetX, Player.tileTargetY].liquid = 255;
										WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY, true);
										this.inventory[this.selectedItem].SetDefaults(205);
										this.itemTime = this.inventory[this.selectedItem].useTime;
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
				if ((this.inventory[this.selectedItem].pick > 0 || this.inventory[this.selectedItem].axe > 0 || this.inventory[this.selectedItem].hammer > 0) && this.position.X / 16f - (float)Player.tileRangeX - (float)this.inventory[this.selectedItem].tileBoost <= (float)Player.tileTargetX && (this.position.X + (float)this.width) / 16f + (float)Player.tileRangeX + (float)this.inventory[this.selectedItem].tileBoost - 1f >= (float)Player.tileTargetX && this.position.Y / 16f - (float)Player.tileRangeY - (float)this.inventory[this.selectedItem].tileBoost <= (float)Player.tileTargetY && (this.position.Y + (float)this.height) / 16f + (float)Player.tileRangeY + (float)this.inventory[this.selectedItem].tileBoost - 2f >= (float)Player.tileTargetY)
				{
					this.showItemIcon = true;
					if (Main.tile[Player.tileTargetX, Player.tileTargetY].active && this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
					{
						if (this.hitTileX != Player.tileTargetX || this.hitTileY != Player.tileTargetY)
						{
							this.hitTile = 0;
							this.hitTileX = Player.tileTargetX;
							this.hitTileY = Player.tileTargetY;
						}
						if (Main.tileNoFail[(int)Main.tile[Player.tileTargetX, Player.tileTargetY].type])
						{
							this.hitTile = 100;
						}
						if (Main.tile[Player.tileTargetX, Player.tileTargetY].type != 27)
						{
							if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 4 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 10 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 11 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 12 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 13 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 14 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 15 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 16 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 17 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 18 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 19 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 21 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 26 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 28 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 29 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 31 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 33 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 34 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 35 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 36 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 42 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 48 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 49 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 50 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 54 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 55 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 77 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 78 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 79)
							{
								if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 48)
								{
									this.hitTile += this.inventory[this.selectedItem].hammer / 3;
								}
								else
								{
									this.hitTile += this.inventory[this.selectedItem].hammer;
								}
								if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 77 && this.inventory[this.selectedItem].hammer < 60)
								{
									this.hitTile = 0;
								}
								if (this.inventory[this.selectedItem].hammer > 0)
								{
									if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 26)
									{
										this.Hurt(this.statLife / 2, -this.direction, false, false);
										WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, true, false, false);
										if (Main.netMode == 1)
										{
											NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, 1f);
										}
									}
									else
									{
										if (this.hitTile >= 100)
										{
											if (Main.netMode == 1 && Main.tile[Player.tileTargetX, Player.tileTargetY].type == 21)
											{
												WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, true, false, false);
												NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, 1f);
												NetMessage.SendData(34, -1, -1, "", Player.tileTargetX, (float)Player.tileTargetY, 0f, 0f);
											}
											else
											{
												this.hitTile = 0;
												WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, false, false, false);
												if (Main.netMode == 1)
												{
													NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, 0f);
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
									}
									this.itemTime = this.inventory[this.selectedItem].useTime;
								}
							}
							else
							{
								if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 5 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 30 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 72)
								{
									if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 30)
									{
										this.hitTile += this.inventory[this.selectedItem].axe * 5;
									}
									else
									{
										this.hitTile += this.inventory[this.selectedItem].axe;
									}
									if (this.inventory[this.selectedItem].axe > 0)
									{
										if (this.hitTile >= 100)
										{
											this.hitTile = 0;
											WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, false, false, false);
											if (Main.netMode == 1)
											{
												NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, 0f);
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
										this.itemTime = this.inventory[this.selectedItem].useTime;
									}
								}
								else
								{
									if (this.inventory[this.selectedItem].pick > 0)
									{
										if (Main.tileDungeon[(int)Main.tile[Player.tileTargetX, Player.tileTargetY].type] || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 37 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 25 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 58)
										{
											this.hitTile += this.inventory[this.selectedItem].pick / 2;
										}
										else
										{
											this.hitTile += this.inventory[this.selectedItem].pick;
										}
										if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 25 && this.inventory[this.selectedItem].pick < 65)
										{
											this.hitTile = 0;
										}
										else
										{
											if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 37 && this.inventory[this.selectedItem].pick < 55)
											{
												this.hitTile = 0;
											}
											else
											{
												if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 56 && this.inventory[this.selectedItem].pick < 65)
												{
													this.hitTile = 0;
												}
												else
												{
													if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 58 && this.inventory[this.selectedItem].pick < 65)
													{
														this.hitTile = 0;
													}
												}
											}
										}
										if (Main.tile[Player.tileTargetX, Player.tileTargetY].type == 0 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 40 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 53 || Main.tile[Player.tileTargetX, Player.tileTargetY].type == 59)
										{
											this.hitTile += this.inventory[this.selectedItem].pick;
										}
										if (this.hitTile >= 100)
										{
											this.hitTile = 0;
											WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, false, false, false);
											if (Main.netMode == 1)
											{
												NetMessage.SendData(17, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, 0f);
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
										this.itemTime = this.inventory[this.selectedItem].useTime;
									}
								}
							}
						}
					}
					if (Main.tile[Player.tileTargetX, Player.tileTargetY].wall > 0 && this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem && this.inventory[this.selectedItem].hammer > 0)
					{
						bool flag3 = true;
						if (!Main.wallHouse[(int)Main.tile[Player.tileTargetX, Player.tileTargetY].wall])
						{
							flag3 = false;
							for (int num21 = Player.tileTargetX - 1; num21 < Player.tileTargetX + 2; num21++)
							{
								for (int num22 = Player.tileTargetY - 1; num22 < Player.tileTargetY + 2; num22++)
								{
									if (Main.tile[num21, num22].wall != Main.tile[Player.tileTargetX, Player.tileTargetY].wall)
									{
										flag3 = true;
										break;
									}
								}
							}
						}
						if (flag3)
						{
							if (this.hitTileX != Player.tileTargetX || this.hitTileY != Player.tileTargetY)
							{
								this.hitTile = 0;
								this.hitTileX = Player.tileTargetX;
								this.hitTileY = Player.tileTargetY;
							}
							this.hitTile += this.inventory[this.selectedItem].hammer;
							if (this.hitTile >= 100)
							{
								this.hitTile = 0;
								WorldGen.KillWall(Player.tileTargetX, Player.tileTargetY, false);
								if (Main.netMode == 1)
								{
									NetMessage.SendData(17, -1, -1, "", 2, (float)Player.tileTargetX, (float)Player.tileTargetY, 0f);
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
							this.itemTime = this.inventory[this.selectedItem].useTime;
						}
					}
				}
				if (this.inventory[this.selectedItem].type == 29 && this.itemAnimation > 0 && this.statLifeMax < 400 && this.itemTime == 0)
				{
					this.itemTime = this.inventory[this.selectedItem].useTime;
					this.statLifeMax += 20;
					this.statLife += 20;
					if (Main.myPlayer == this.whoAmi)
					{
						this.HealEffect(20);
					}
				}
				if (this.inventory[this.selectedItem].type == 109 && this.itemAnimation > 0 && this.statManaMax < 200 && this.itemTime == 0)
				{
					this.itemTime = this.inventory[this.selectedItem].useTime;
					this.statManaMax += 20;
					this.statMana += 20;
					if (Main.myPlayer == this.whoAmi)
					{
						this.ManaEffect(20);
					}
				}
				if (this.inventory[this.selectedItem].createTile >= 0 && this.position.X / 16f - (float)Player.tileRangeX - (float)this.inventory[this.selectedItem].tileBoost <= (float)Player.tileTargetX && (this.position.X + (float)this.width) / 16f + (float)Player.tileRangeX + (float)this.inventory[this.selectedItem].tileBoost - 1f >= (float)Player.tileTargetX && this.position.Y / 16f - (float)Player.tileRangeY - (float)this.inventory[this.selectedItem].tileBoost <= (float)Player.tileTargetY && (this.position.Y + (float)this.height) / 16f + (float)Player.tileRangeY + (float)this.inventory[this.selectedItem].tileBoost - 2f >= (float)Player.tileTargetY)
				{
					this.showItemIcon = true;
					if ((!Main.tile[Player.tileTargetX, Player.tileTargetY].active || this.inventory[this.selectedItem].createTile == 23 || this.inventory[this.selectedItem].createTile == 2 || this.inventory[this.selectedItem].createTile == 60 || this.inventory[this.selectedItem].createTile == 70) && this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
					{
						bool flag4 = false;
						if (this.inventory[this.selectedItem].createTile == 23 || this.inventory[this.selectedItem].createTile == 2)
						{
							if (Main.tile[Player.tileTargetX, Player.tileTargetY].active && Main.tile[Player.tileTargetX, Player.tileTargetY].type == 0)
							{
								flag4 = true;
							}
						}
						else
						{
							if (this.inventory[this.selectedItem].createTile == 60 || this.inventory[this.selectedItem].createTile == 70)
							{
								if (Main.tile[Player.tileTargetX, Player.tileTargetY].active && Main.tile[Player.tileTargetX, Player.tileTargetY].type == 59)
								{
									flag4 = true;
								}
							}
							else
							{
								if (this.inventory[this.selectedItem].createTile == 4)
								{
									int num23 = (int)Main.tile[Player.tileTargetX, Player.tileTargetY + 1].type;
									int num24 = (int)Main.tile[Player.tileTargetX - 1, Player.tileTargetY].type;
									int num25 = (int)Main.tile[Player.tileTargetX + 1, Player.tileTargetY].type;
									int num26 = (int)Main.tile[Player.tileTargetX - 1, Player.tileTargetY - 1].type;
									int num27 = (int)Main.tile[Player.tileTargetX + 1, Player.tileTargetY - 1].type;
									int num28 = (int)Main.tile[Player.tileTargetX - 1, Player.tileTargetY - 1].type;
									int num29 = (int)Main.tile[Player.tileTargetX + 1, Player.tileTargetY + 1].type;
									if (!Main.tile[Player.tileTargetX, Player.tileTargetY + 1].active)
									{
										num23 = -1;
									}
									if (!Main.tile[Player.tileTargetX - 1, Player.tileTargetY].active)
									{
										num24 = -1;
									}
									if (!Main.tile[Player.tileTargetX + 1, Player.tileTargetY].active)
									{
										num25 = -1;
									}
									if (!Main.tile[Player.tileTargetX - 1, Player.tileTargetY - 1].active)
									{
										num26 = -1;
									}
									if (!Main.tile[Player.tileTargetX + 1, Player.tileTargetY - 1].active)
									{
										num27 = -1;
									}
									if (!Main.tile[Player.tileTargetX - 1, Player.tileTargetY + 1].active)
									{
										num28 = -1;
									}
									if (!Main.tile[Player.tileTargetX + 1, Player.tileTargetY + 1].active)
									{
										num29 = -1;
									}
									if (num23 >= 0 && Main.tileSolid[num23] && !Main.tileNoAttach[num23])
									{
										flag4 = true;
									}
									else
									{
										if ((num24 >= 0 && Main.tileSolid[num24] && !Main.tileNoAttach[num24]) || (num24 == 5 && num26 == 5 && num28 == 5))
										{
											flag4 = true;
										}
										else
										{
											if ((num25 >= 0 && Main.tileSolid[num25] && !Main.tileNoAttach[num25]) || (num25 == 5 && num27 == 5 && num29 == 5))
											{
												flag4 = true;
											}
										}
									}
								}
								else
								{
									if (this.inventory[this.selectedItem].createTile == 78)
									{
										if (Main.tile[Player.tileTargetX, Player.tileTargetY + 1].active && (Main.tileSolid[(int)Main.tile[Player.tileTargetX, Player.tileTargetY + 1].type] || Main.tileTable[(int)Main.tile[Player.tileTargetX, Player.tileTargetY + 1].type]))
										{
											flag4 = true;
										}
									}
									else
									{
										if (this.inventory[this.selectedItem].createTile == 13 || this.inventory[this.selectedItem].createTile == 29 || this.inventory[this.selectedItem].createTile == 33 || this.inventory[this.selectedItem].createTile == 49)
										{
											if (Main.tile[Player.tileTargetX, Player.tileTargetY + 1].active && Main.tileTable[(int)Main.tile[Player.tileTargetX, Player.tileTargetY + 1].type])
											{
												flag4 = true;
											}
										}
										else
										{
											if (this.inventory[this.selectedItem].createTile == 51)
											{
												if (Main.tile[Player.tileTargetX + 1, Player.tileTargetY].active || Main.tile[Player.tileTargetX + 1, Player.tileTargetY].wall > 0 || Main.tile[Player.tileTargetX - 1, Player.tileTargetY].active || Main.tile[Player.tileTargetX - 1, Player.tileTargetY].wall > 0 || Main.tile[Player.tileTargetX, Player.tileTargetY + 1].active || Main.tile[Player.tileTargetX, Player.tileTargetY + 1].wall > 0 || Main.tile[Player.tileTargetX, Player.tileTargetY - 1].active || Main.tile[Player.tileTargetX, Player.tileTargetY - 1].wall > 0)
												{
													flag4 = true;
												}
											}
											else
											{
												if ((Main.tile[Player.tileTargetX + 1, Player.tileTargetY].active && Main.tileSolid[(int)Main.tile[Player.tileTargetX + 1, Player.tileTargetY].type]) || (Main.tile[Player.tileTargetX + 1, Player.tileTargetY].wall > 0 || (Main.tile[Player.tileTargetX - 1, Player.tileTargetY].active && Main.tileSolid[(int)Main.tile[Player.tileTargetX - 1, Player.tileTargetY].type])) || (Main.tile[Player.tileTargetX - 1, Player.tileTargetY].wall > 0 || (Main.tile[Player.tileTargetX, Player.tileTargetY + 1].active && Main.tileSolid[(int)Main.tile[Player.tileTargetX, Player.tileTargetY + 1].type])) || (Main.tile[Player.tileTargetX, Player.tileTargetY + 1].wall > 0 || (Main.tile[Player.tileTargetX, Player.tileTargetY - 1].active && Main.tileSolid[(int)Main.tile[Player.tileTargetX, Player.tileTargetY - 1].type])) || Main.tile[Player.tileTargetX, Player.tileTargetY - 1].wall > 0)
												{
													flag4 = true;
												}
											}
										}
									}
								}
							}
						}
						if (flag4 && WorldGen.PlaceTile(Player.tileTargetX, Player.tileTargetY, this.inventory[this.selectedItem].createTile, false, false, this.whoAmi))
						{
							this.itemTime = this.inventory[this.selectedItem].useTime;
							if (Main.netMode == 1)
							{
								NetMessage.SendData(17, -1, -1, "", 1, (float)Player.tileTargetX, (float)Player.tileTargetY, (float)this.inventory[this.selectedItem].createTile);
							}
							if (this.inventory[this.selectedItem].createTile == 15)
							{
								if (this.direction == 1)
								{
									Tile expr_37BD = Main.tile[Player.tileTargetX, Player.tileTargetY];
									expr_37BD.frameX += 18;
									Tile expr_37E2 = Main.tile[Player.tileTargetX, Player.tileTargetY - 1];
									expr_37E2.frameX += 18;
								}
								if (Main.netMode == 1)
								{
									NetMessage.SendTileSquare(-1, Player.tileTargetX - 1, Player.tileTargetY - 1, 3);
								}
							}
							else
							{
								if (this.inventory[this.selectedItem].createTile == 79 && Main.netMode == 1)
								{
									NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 5);
								}
							}
						}
					}
				}
				if (this.inventory[this.selectedItem].createWall >= 0)
				{
                    //Player.tileTargetX = (int)(((float)Main.mouseState.X + Main.screenPosition.X) / 16f);
                    //Player.tileTargetY = (int)(((float)Main.mouseState.Y + Main.screenPosition.Y) / 16f);
                    //if (this.position.X / 16f - (float)Player.tileRangeX - (float)this.inventory[this.selectedItem].tileBoost <= (float)Player.tileTargetX && (this.position.X + (float)this.width) / 16f + (float)Player.tileRangeX + (float)this.inventory[this.selectedItem].tileBoost - 1f >= (float)Player.tileTargetX && this.position.Y / 16f - (float)Player.tileRangeY - (float)this.inventory[this.selectedItem].tileBoost <= (float)Player.tileTargetY && (this.position.Y + (float)this.height) / 16f + (float)Player.tileRangeY + (float)this.inventory[this.selectedItem].tileBoost - 2f >= (float)Player.tileTargetY)
                    //{
                    //    this.showItemIcon = true;
                    //    if (this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem && (Main.tile[Player.tileTargetX + 1, Player.tileTargetY].active || Main.tile[Player.tileTargetX + 1, Player.tileTargetY].wall > 0 || Main.tile[Player.tileTargetX - 1, Player.tileTargetY].active || Main.tile[Player.tileTargetX - 1, Player.tileTargetY].wall > 0 || Main.tile[Player.tileTargetX, Player.tileTargetY + 1].active || Main.tile[Player.tileTargetX, Player.tileTargetY + 1].wall > 0 || Main.tile[Player.tileTargetX, Player.tileTargetY - 1].active || Main.tile[Player.tileTargetX, Player.tileTargetY - 1].wall > 0) && (int)Main.tile[Player.tileTargetX, Player.tileTargetY].wall != this.inventory[this.selectedItem].createWall)
                    //    {
                    //        WorldGen.PlaceWall(Player.tileTargetX, Player.tileTargetY, this.inventory[this.selectedItem].createWall, false);
                    //        if ((int)Main.tile[Player.tileTargetX, Player.tileTargetY].wall == this.inventory[this.selectedItem].createWall)
                    //        {
                    //            this.itemTime = this.inventory[this.selectedItem].useTime;
                    //            if (Main.netMode == 1)
                    //            {
                    //                NetMessage.SendData(17, -1, -1, "", 3, (float)Player.tileTargetX, (float)Player.tileTargetY, (float)this.inventory[this.selectedItem].createWall);
                    //            }
                    //        }
                    //    }
                    //}
				}
			}
			if (this.inventory[this.selectedItem].damage >= 0 && this.inventory[this.selectedItem].type > 0 && !this.inventory[this.selectedItem].noMelee && this.itemAnimation > 0)
			{
				bool flag5 = false;
				Rectangle rectangle = new Rectangle((int)this.itemLocation.X, (int)this.itemLocation.Y, 32, 32);
                //if (!Main.dedServ)
                //{
                //    rectangle = new Rectangle((int)this.itemLocation.X, (int)this.itemLocation.Y, Main.itemTexture[this.inventory[this.selectedItem].type].Width, Main.itemTexture[this.inventory[this.selectedItem].type].Height);
                //}
				rectangle.Width = (int)((float)rectangle.Width * this.inventory[this.selectedItem].scale);
				rectangle.Height = (int)((float)rectangle.Height * this.inventory[this.selectedItem].scale);
				if (this.direction == -1)
				{
					rectangle.X -= rectangle.Width;
				}
				rectangle.Y -= rectangle.Height;
				if (this.inventory[this.selectedItem].useStyle == 1)
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
					if (this.inventory[this.selectedItem].useStyle == 3)
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
					if ((this.inventory[this.selectedItem].type == 44 || this.inventory[this.selectedItem].type == 45 || this.inventory[this.selectedItem].type == 46 || this.inventory[this.selectedItem].type == 103 || this.inventory[this.selectedItem].type == 104) && Main.rand.Next(15) == 0)
					{
						Vector2 arg_3FC1_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
						int arg_3FC1_1 = rectangle.Width;
						int arg_3FC1_2 = rectangle.Height;
						int arg_3FC1_3 = 14;
						float arg_3FC1_4 = (float)(this.direction * 2);
						float arg_3FC1_5 = 0f;
						int arg_3FC1_6 = 150;
						Color newColor =  new Color();
						Dust.NewDust(arg_3FC1_0, arg_3FC1_1, arg_3FC1_2, arg_3FC1_3, arg_3FC1_4, arg_3FC1_5, arg_3FC1_6, newColor, 1.3f);
					}
					if (this.inventory[this.selectedItem].type == 65)
					{
						if (Main.rand.Next(5) == 0)
						{
							Vector2 arg_4030_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
							int arg_4030_1 = rectangle.Width;
							int arg_4030_2 = rectangle.Height;
							int arg_4030_3 = 15;
							float arg_4030_4 = 0f;
							float arg_4030_5 = 0f;
							int arg_4030_6 = 150;
							Color newColor =  new Color();
							Dust.NewDust(arg_4030_0, arg_4030_1, arg_4030_2, arg_4030_3, arg_4030_4, arg_4030_5, arg_4030_6, newColor, 1.2f);
						}
						if (Main.rand.Next(10) == 0)
						{
							Gore.NewGore(new Vector2((float)rectangle.X, (float)rectangle.Y), new Vector2(), Main.rand.Next(16, 18));
						}
					}
					if (this.inventory[this.selectedItem].type == 190 || this.inventory[this.selectedItem].type == 213)
					{
						Vector2 arg_410A_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
						int arg_410A_1 = rectangle.Width;
						int arg_410A_2 = rectangle.Height;
						int arg_410A_3 = 40;
						float arg_410A_4 = this.velocity.X * 0.2f + (float)(this.direction * 3);
						float arg_410A_5 = this.velocity.Y * 0.2f;
						int arg_410A_6 = 0;
						Color newColor =  new Color();
						int num30 = Dust.NewDust(arg_410A_0, arg_410A_1, arg_410A_2, arg_410A_3, arg_410A_4, arg_410A_5, arg_410A_6, newColor, 1.2f);
						Main.dust[num30].noGravity = true;
					}
					if (this.inventory[this.selectedItem].type == 121)
					{
						for (int num31 = 0; num31 < 2; num31++)
						{
							Vector2 arg_41A1_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
							int arg_41A1_1 = rectangle.Width;
							int arg_41A1_2 = rectangle.Height;
							int arg_41A1_3 = 6;
							float arg_41A1_4 = this.velocity.X * 0.2f + (float)(this.direction * 3);
							float arg_41A1_5 = this.velocity.Y * 0.2f;
							int arg_41A1_6 = 100;
							Color newColor =  new Color();
							int num32 = Dust.NewDust(arg_41A1_0, arg_41A1_1, arg_41A1_2, arg_41A1_3, arg_41A1_4, arg_41A1_5, arg_41A1_6, newColor, 2.5f);
							Main.dust[num32].noGravity = true;
							Dust expr_41C3_cp_0 = Main.dust[num32];
							expr_41C3_cp_0.velocity.X = expr_41C3_cp_0.velocity.X * 2f;
							Dust expr_41E1_cp_0 = Main.dust[num32];
							expr_41E1_cp_0.velocity.Y = expr_41E1_cp_0.velocity.Y * 2f;
						}
					}
					if (this.inventory[this.selectedItem].type == 122 || this.inventory[this.selectedItem].type == 217)
					{
						Vector2 arg_4290_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
						int arg_4290_1 = rectangle.Width;
						int arg_4290_2 = rectangle.Height;
						int arg_4290_3 = 6;
						float arg_4290_4 = this.velocity.X * 0.2f + (float)(this.direction * 3);
						float arg_4290_5 = this.velocity.Y * 0.2f;
						int arg_4290_6 = 100;
						Color newColor =  new Color();
						int num33 = Dust.NewDust(arg_4290_0, arg_4290_1, arg_4290_2, arg_4290_3, arg_4290_4, arg_4290_5, arg_4290_6, newColor, 1.9f);
						Main.dust[num33].noGravity = true;
					}
					if (this.inventory[this.selectedItem].type == 155)
					{
						Vector2 arg_4323_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
						int arg_4323_1 = rectangle.Width;
						int arg_4323_2 = rectangle.Height;
						int arg_4323_3 = 29;
						float arg_4323_4 = this.velocity.X * 0.2f + (float)(this.direction * 3);
						float arg_4323_5 = this.velocity.Y * 0.2f;
						int arg_4323_6 = 100;
						Color newColor =  new Color();
						int num34 = Dust.NewDust(arg_4323_0, arg_4323_1, arg_4323_2, arg_4323_3, arg_4323_4, arg_4323_5, arg_4323_6, newColor, 2f);
						Main.dust[num34].noGravity = true;
						Dust expr_4345_cp_0 = Main.dust[num34];
						expr_4345_cp_0.velocity.X = expr_4345_cp_0.velocity.X / 2f;
						Dust expr_4363_cp_0 = Main.dust[num34];
						expr_4363_cp_0.velocity.Y = expr_4363_cp_0.velocity.Y / 2f;
					}
					if (this.inventory[this.selectedItem].type >= 198 && this.inventory[this.selectedItem].type <= 203)
					{
						//Lighting.addLight((int)((this.itemLocation.X + 6f + this.velocity.X) / 16f), (int)((this.itemLocation.Y - 14f) / 16f), 0.5f);
					}
					if (Main.myPlayer == i)
					{
						int num35 = rectangle.X / 16;
						int num36 = (rectangle.X + rectangle.Width) / 16 + 1;
						int num37 = rectangle.Y / 16;
						int num38 = (rectangle.Y + rectangle.Height) / 16 + 1;
						for (int num39 = num35; num39 < num36; num39++)
						{
							for (int num40 = num37; num40 < num38; num40++)
							{
								if (Main.tile[num39, num40].type == 3 || Main.tile[num39, num40].type == 24 || Main.tile[num39, num40].type == 28 || Main.tile[num39, num40].type == 32 || Main.tile[num39, num40].type == 51 || Main.tile[num39, num40].type == 52 || Main.tile[num39, num40].type == 61 || Main.tile[num39, num40].type == 62 || Main.tile[num39, num40].type == 69 || Main.tile[num39, num40].type == 71 || Main.tile[num39, num40].type == 73 || Main.tile[num39, num40].type == 74)
								{
									WorldGen.KillTile(num39, num40, false, false, false);
									if (Main.netMode == 1)
									{
										NetMessage.SendData(17, -1, -1, "", 0, (float)num39, (float)num40, 0f);
									}
								}
							}
						}
						for (int num41 = 0; num41 < 1000; num41++)
						{
							if (Main.npc[num41].active && Main.npc[num41].immune[i] == 0 && this.attackCD == 0 && !Main.npc[num41].friendly)
							{
								Rectangle value = new Rectangle((int)Main.npc[num41].position.X, (int)Main.npc[num41].position.Y, Main.npc[num41].width, Main.npc[num41].height);
								if (rectangle.Intersects(value) && (Main.npc[num41].noTileCollide || Collision.CanHit(this.position, this.width, this.height, Main.npc[num41].position, Main.npc[num41].width, Main.npc[num41].height)))
								{
									Main.npc[num41].StrikeNPC(this.inventory[this.selectedItem].damage, this.inventory[this.selectedItem].knockBack, this.direction);
									if (Main.netMode == 1)
									{
										NetMessage.SendData(24, -1, -1, "", num41, (float)i, 0f, 0f);
									}
									Main.npc[num41].immune[i] = this.itemAnimation;
									this.attackCD = (int)((double)this.itemAnimationMax * 0.33);
								}
							}
						}
						if (this.hostile)
						{
							for (int num42 = 0; num42 < 255; num42++)
							{
								if (num42 != i && Main.player[num42].active && Main.player[num42].hostile && !Main.player[num42].immune && !Main.player[num42].dead && (Main.player[i].team == 0 || Main.player[i].team != Main.player[num42].team))
								{
									Rectangle value2 = new Rectangle((int)Main.player[num42].position.X, (int)Main.player[num42].position.Y, Main.player[num42].width, Main.player[num42].height);
									if (rectangle.Intersects(value2) && Collision.CanHit(this.position, this.width, this.height, Main.player[num42].position, Main.player[num42].width, Main.player[num42].height))
									{
										Main.player[num42].Hurt(this.inventory[this.selectedItem].damage, this.direction, true, false);
										if (Main.netMode != 0)
										{
											NetMessage.SendData(26, -1, -1, "", num42, (float)this.direction, (float)this.inventory[this.selectedItem].damage, 1f);
										}
										this.attackCD = (int)((double)this.itemAnimationMax * 0.33);
									}
								}
							}
						}
					}
				}
			}
			if (this.itemTime == 0 && this.itemAnimation > 0)
			{
				if (this.inventory[this.selectedItem].healLife > 0)
				{
					this.statLife += this.inventory[this.selectedItem].healLife;
					this.itemTime = this.inventory[this.selectedItem].useTime;
					if (Main.myPlayer == this.whoAmi)
					{
						this.HealEffect(this.inventory[this.selectedItem].healLife);
					}
				}
				if (this.inventory[this.selectedItem].healMana > 0)
				{
					this.statMana += this.inventory[this.selectedItem].healMana;
					this.itemTime = this.inventory[this.selectedItem].useTime;
					if (Main.myPlayer == this.whoAmi)
					{
						this.ManaEffect(this.inventory[this.selectedItem].healMana);
					}
				}
			}
			if (this.itemTime == 0 && this.itemAnimation > 0 && (this.inventory[this.selectedItem].type == 43 || this.inventory[this.selectedItem].type == 70))
			{
				this.itemTime = this.inventory[this.selectedItem].useTime;
				bool flag6 = false;
				int num43 = 4;
				if (this.inventory[this.selectedItem].type == 43)
				{
					num43 = 4;
				}
				else
				{
					if (this.inventory[this.selectedItem].type == 70)
					{
						num43 = 13;
					}
				}
				for (int num44 = 0; num44 < 1000; num44++)
				{
					if (Main.npc[num44].active && Main.npc[num44].type == num43)
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
				else
				{
					if (this.inventory[this.selectedItem].type == 43)
					{
						if (!Main.dayTime)
						{
							//Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
							if (Main.netMode != 1)
							{
								NPC.SpawnOnPlayer(i, 4);
							}
						}
					}
					else
					{
						if (this.inventory[this.selectedItem].type == 70 && this.zoneEvil)
						{
							//Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
							if (Main.netMode != 1)
							{
								NPC.SpawnOnPlayer(i, 13);
							}
						}
					}
				}
			}
			if (this.inventory[this.selectedItem].type == 50 && this.itemAnimation > 0)
			{
				if (Main.rand.Next(2) == 0)
				{
					Vector2 arg_4BFA_0 = this.position;
					int arg_4BFA_1 = this.width;
					int arg_4BFA_2 = this.height;
					int arg_4BFA_3 = 15;
					float arg_4BFA_4 = 0f;
					float arg_4BFA_5 = 0f;
					int arg_4BFA_6 = 150;
					Color newColor =  new Color();
					Dust.NewDust(arg_4BFA_0, arg_4BFA_1, arg_4BFA_2, arg_4BFA_3, arg_4BFA_4, arg_4BFA_5, arg_4BFA_6, newColor, 1.1f);
				}
				if (this.itemTime == 0)
				{
					this.itemTime = this.inventory[this.selectedItem].useTime;
				}
				else
				{
					if (this.itemTime == this.inventory[this.selectedItem].useTime / 2)
					{
						for (int num45 = 0; num45 < 70; num45++)
						{
							Vector2 arg_4C93_0 = this.position;
							int arg_4C93_1 = this.width;
							int arg_4C93_2 = this.height;
							int arg_4C93_3 = 15;
							float arg_4C93_4 = this.velocity.X * 0.5f;
							float arg_4C93_5 = this.velocity.Y * 0.5f;
							int arg_4C93_6 = 150;
							Color newColor =  new Color();
							Dust.NewDust(arg_4C93_0, arg_4C93_1, arg_4C93_2, arg_4C93_3, arg_4C93_4, arg_4C93_5, arg_4C93_6, newColor, 1.5f);
						}
						this.grappling[0] = -1;
						this.grapCount = 0;
						for (int num46 = 0; num46 < 1000; num46++)
						{
							if (Main.projectile[num46].active && Main.projectile[num46].owner == i && Main.projectile[num46].aiStyle == 7)
							{
								Main.projectile[num46].Kill();
							}
						}
						this.Spawn();
						for (int num47 = 0; num47 < 70; num47++)
						{
							Vector2 arg_4D42_0 = this.position;
							int arg_4D42_1 = this.width;
							int arg_4D42_2 = this.height;
							int arg_4D42_3 = 15;
							float arg_4D42_4 = 0f;
							float arg_4D42_5 = 0f;
							int arg_4D42_6 = 150;
							Color newColor =  new Color();
							Dust.NewDust(arg_4D42_0, arg_4D42_1, arg_4D42_2, arg_4D42_3, arg_4D42_4, arg_4D42_5, arg_4D42_6, newColor, 1.5f);
						}
					}
				}
			}
			if (i == Main.myPlayer)
			{
				if (this.itemTime == this.inventory[this.selectedItem].useTime && this.inventory[this.selectedItem].consumable)
				{
					this.inventory[this.selectedItem].stack--;
					if (this.inventory[this.selectedItem].stack <= 0)
					{
						this.itemTime = this.itemAnimation;
					}
				}
				if (this.inventory[this.selectedItem].stack <= 0 && this.itemAnimation == 0)
				{
					this.inventory[this.selectedItem] = new Item();
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
				if (this.inventory[i].type >= 71 && this.inventory[i].type <= 74)
				{
					int num = Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, this.inventory[i].type, 1, false);
					int num2 = this.inventory[i].stack / 2;
					num2 = this.inventory[i].stack - num2;
					this.inventory[i].stack -= num2;
					if (this.inventory[i].stack <= 0)
					{
						this.inventory[i] = new Item();
					}
					Main.item[num].stack = num2;
					Main.item[num].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
					Main.item[num].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
					Main.item[num].noGrabDelay = 100;
					if (Main.netMode == 1)
					{
						NetMessage.SendData(21, -1, -1, "", num, 0f, 0f, 0f);
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
			player.selectedItem = this.selectedItem;
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
			player.position.X = this.position.X;
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
		
        private static void EncryptFile(string inputFile, string outputFile)
		{
			string s = "h3y_gUyZ";
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
		
        private static bool DecryptFile(string inputFile, string outputFile)
		{
			string s = "h3y_gUyZ";
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
			if (!Main.tile[x, y - 1].active || Main.tile[x, y - 1].type != 79)
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
					if (Main.tile[i, j].active && Main.tileSolid[(int)Main.tile[i, j].type] && !Main.tileSolidTop[(int)Main.tile[i, j].type])
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
		
        public static void SavePlayer(Player newPlayer)
		{
            string playerPath = Statics.getPlayerPath + "\\" + newPlayer.name;
            try
            {
                Directory.CreateDirectory(Statics.getPlayerPath);
            }
            catch
            {
            }
            if (playerPath == null)
            {
                return;
            }
			string destFileName = playerPath + ".bak";
			if (File.Exists(playerPath))
			{
				File.Copy(playerPath, destFileName, true);
			}
			string text = playerPath + ".dat";
			using (FileStream fileStream = new FileStream(text, FileMode.Create))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
				{
					binaryWriter.Write(Statics.currentRelease);
					binaryWriter.Write(newPlayer.name);
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
						if (newPlayer.armor[i].name == null)
						{
							newPlayer.armor[i].name = "";
						}
						binaryWriter.Write(newPlayer.armor[i].name);
					}
					for (int j = 0; j < 44; j++)
					{
						if (newPlayer.inventory[j].name == null)
						{
							newPlayer.inventory[j].name = "";
						}
						binaryWriter.Write(newPlayer.inventory[j].name);
						binaryWriter.Write(newPlayer.inventory[j].stack);
					}
					for (int k = 0; k < Chest.maxItems; k++)
					{
						if (newPlayer.bank[k].name == null)
						{
							newPlayer.bank[k].name = "";
						}
						binaryWriter.Write(newPlayer.bank[k].name);
						binaryWriter.Write(newPlayer.bank[k].stack);
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
		
        public static Player LoadPlayer(string playerPath)
		{
			bool flag = false;
			if (Main.rand == null)
			{
				Main.rand = new Random((int)DateTime.Now.Ticks);
			}
			Player player = new Player();
			try
			{
				string text = playerPath + ".dat";
				flag = Player.DecryptFile(playerPath, text);
				if (!flag)
				{
					using (FileStream fileStream = new FileStream(text, FileMode.Open))
					{
						using (BinaryReader binaryReader = new BinaryReader(fileStream))
						{
							int release = binaryReader.ReadInt32();
							player.name = binaryReader.ReadString();
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
								player.inventory[j].stack = binaryReader.ReadInt32();
							}
							for (int k = 0; k < Chest.maxItems; k++)
							{
								player.bank[k].SetDefaults(Item.VersionName(binaryReader.ReadString(), release));
								player.bank[k].stack = binaryReader.ReadInt32();
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
			string text2 = playerPath + ".bak";
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
					this.armor[i].name = "";
				}
				this.inventory[i] = new Item();
				this.inventory[i].name = "";
			}
			for (int j = 0; j < Chest.maxItems; j++)
			{
				this.bank[j] = new Item();
				this.bank[j].name = "";
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

        public ServerSock getSocket()
        {
            return Netplay.serverSock[this.whoAmi];
        }

        public void Kick(string Reason = null)
        {
            string message = "You have been Kicked from this Server.";

            if (Reason != null)
            {
                message = Reason;
            }

            NetMessage.SendData(2, this.whoAmi, -1, message, 0, 0f, 0f, 0f);
        }

        public string getIPAddress()
        {
            return ipAddress;
        }

        public void setIPAddress(string IPaddress)
        {
            ipAddress = IPaddress;
        }

        public Vector2 getLocation()
        {
            return position;
        }

        public void setLocation(Vector2 Location)
        {
            position = Location;
        }

        public void teleportTo(float tileX, float tileY)
        {
            //Preserve out Spawn point.
            int xPreserve = Main.spawnTileX;
            int yPreserve = Main.spawnTileY;

            //The spawn the client wants is the from player Pos /16.
            //This is because the Client reads frames, Not Tile Records.
            Main.spawnTileX = ((int)tileX / 16);
            Main.spawnTileY = ((int)tileY / 16);

            NetMessage.SendData((int)Packet.WORLD_DATA, this.whoAmi, -1, "", 0, 0f, 0f, 0f); //Trigger Client Data Update (Updates Spawn Position)
            NetMessage.SendData((int)Packet.WORLD_DATA, -1, -1, "", 0, 0f, 0f, 0f); //Trigger Client Data Update (Updates Spawn Position)
            NetMessage.SendData((int)Packet.RECEIVING_PLAYER_JOINED, -1, -1, "", this.whoAmi, 0f, 0f, 0f); //Trigger the player to spawn

            this.UpdatePlayer(this.whoAmi); //Update players data (I don't think needed by default, But hay)

            this.position.X = tileX;
            this.position.Y = tileY;

            //NetMessage.syncPlayers(); //Sync the Players Position.

            //Return our preserved Spawn Point.
            Main.spawnTileX = xPreserve;
            Main.spawnTileY = yPreserve;

            //NetMessage.SendData((int)Packet.WORLD_DATA, this.whoAmi, -1, "", 0, 0f, 0f, 0f); //Trigger the Client Data Update again to reset the Spawn Point
            //NetMessage.SendData((int)Packet.WORLD_DATA, -1, this.whoAmi, "", 0, 0f, 0f, 0f); //Trigger the Client Data Update again to reset the Spawn Point

            this.SpawnX = Main.spawnTileX;
            this.SpawnY = Main.spawnTileY;

            this.Spawn(); //Tell the Client to Spawn (Sets Defaults)
            this.UpdatePlayer(this.whoAmi); //Update players data (I don't think needed by default, But hay)

            NetMessage.SendData((int)Packet.WORLD_DATA, this.whoAmi, -1, "", 0, 0f, 0f, 0f); //Trigger Client Data Update (Updates Spawn Position)
            //NetMessage.SendData((int)Packet.WORLD_DATA, -1, -1, "", 0, 0f, 0f, 0f); //Trigger Client Data Update (Updates Spawn Position)
            //NetMessage.SendData((int)Packet.RECEIVING_PLAYER_JOINED, -1, -1, "", this.whoAmi, 0f, 0f, 0f); //Trigger the player to spawn

            NetMessage.syncPlayers(); //Sync the Players Position.
        }

        public void teleportTo(Player player)
        {
            this.teleportTo(player.position.X, player.position.Y);
        }

        public static string getPassword(string server, Server Server)
        {
            foreach (string listee in Server.getOpList().getArrayList())
            {
                if (listee != null && listee.Trim().ToLower().Length > 0)
                {
                    string userPass = listee.Trim().ToLower();
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

        public string getPassword()
        {
            return Player.getPassword(this.name, this.getServer());
        }

        public static bool isInOpList(string Name, Server Server)
        {
            foreach (string listee in Server.getOpList().getArrayList())
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

        public bool isInOpList()
        {
            return Player.isInOpList(this.name, this.getServer());
        }

        public string getOpListKey()
        {
            return this.name.Trim().ToLower() + getPassword();
        }

    }
}
