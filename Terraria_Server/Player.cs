using Terraria_Server.Commands;
using System;
using Terraria_Server.Events;
using Terraria_Server.Plugin;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Terraria_Server.Misc;
using Terraria_Server.Shops;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;
using Terraria_Server.WorldMod;

namespace Terraria_Server
{
	public class Player : BaseEntity, ISender
    {
        private const int MAX_INVENTORY = 44;
        private const int MAX_HEALTH = 400;
        private const int MAX_MANA = 200;

        public bool HasClientMod = false;

        private String ipAddress = null;
        private bool bedDestruction = false;

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
		public Item[] inventory = new Item[MAX_INVENTORY];
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
		
		// Plugins can keep per-player state in here, using their object or name as a key
		// The collection is synchronized
		public readonly System.Collections.Hashtable PluginData;
		
		// null if no authentication plugin is running or the user is a guest,
		// otherwise filled with the account or character name
		public string AuthenticatedAs { get; set; }

        public bool Op { get; set; }

        public void sendMessage(String Message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
            NetMessage.SendData((int)Packet.PLAYER_CHAT, whoAmi, -1, Message, A, R, G, B);
        }

        public void sendMessage(String Message, Color chatColour)
        {
            NetMessage.SendData((int)Packet.PLAYER_CHAT, whoAmi, -1, Message, 255, chatColour.R, chatColour.G, chatColour.B);
        }

        public void HealEffect(int healAmount, bool overrider = false, int remoteClient = -1)
		{
            if (overrider || (this.whoAmi == Main.myPlayer))
			{
                NetMessage.SendData(35, remoteClient, -1, "", this.whoAmi, (float)healAmount);
			}
		}

        public void ManaEffect(int manaAmount, bool overrider = false, int remoteClient = -1)
		{
			if (overrider || (this.whoAmi == Main.myPlayer))
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
				if (Main.players[j].Active && !Main.players[j].dead && (num == -1f || Math.Abs(Main.players[j].Position.X + (float)(Main.players[j].Width / 2) - Position.X + (float)(Width / 2)) + Math.Abs(Main.players[j].Position.Y + (float)(Main.players[j].Height / 2) - Position.Y + (float)(Height / 2)) < num))
				{
					num = Math.Abs(Main.players[j].Position.X + (float)(Main.players[j].Width / 2) - Position.X + (float)(Width / 2)) + Math.Abs(Main.players[j].Position.Y + (float)(Main.players[j].Height / 2) - Position.Y + (float)(Height / 2));
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
                            this.zoneDungeon = false;
                            this.zoneJungle = false;
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
                                int num12 = (int)(((double)this.Position.X + (double)this.Width * 0.5) / 16.0);
                                int num13 = (int)(((double)this.Position.Y + (double)this.Height * 0.5) / 16.0);
                                if (num12 < this.chestX - 5 || num12 > this.chestX + 6 || num13 < this.chestY - 4 || num13 > this.chestY + 5)
                                {
                                    this.chest = -1;
                                }
                                if (!Main.tile.At(this.chestX, this.chestY).Active)
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
                            int i2 = (int)(this.Position.X + (float)(this.Width / 2) + (float)(8 * this.direction)) / 16;
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
                        }
                        if (this.head == 6 && this.body == 6 && this.legs == 6)
                        {
                            this.setBonus = "Space Gun costs 0 mana";
                            this.spaceGun = true;
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
                        }
                        if (this.head == 9 && this.body == 9 && this.legs == 9)
                        {
                            this.setBonus = "5 defense";
                            this.statDefense += 5;
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
                        if (this.Position.X / 16f - (float)Player.tileRangeX <= (float)Player.tileTargetX && (this.Position.X + (float)this.Width) / 16f + (float)Player.tileRangeX - 1f >= (float)Player.tileTargetX && this.Position.Y / 16f - (float)Player.tileRangeY <= (float)Player.tileTargetY && (this.Position.Y + (float)this.Height) / 16f + (float)Player.tileRangeY - 2f >= (float)Player.tileTargetY && Main.tile.At(Player.tileTargetX, Player.tileTargetY).Active)
                        {
                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 79)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 224;
                            }
                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 21)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 48;
                            }
                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 4)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 8;
                            }
                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 13)
                            {
                                this.showItemIcon = true;
                                if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameX == 18)
                                {
                                    this.showItemIcon2 = 28;
                                }
                                else
                                {
                                    if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameX == 36)
                                    {
                                        this.showItemIcon2 = 110;
                                    }
                                    else
                                    {
                                        this.showItemIcon2 = 31;
                                    }
                                }
                            }
                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 29)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 87;
                            }
                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 33)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 105;
                            }
                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 49)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 148;
                            }
                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 50 && Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameX == 90)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 165;
                            }
                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 55)
                            {
                                int num30 = (int)(Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameX / 18);
                                int num31 = (int)(Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameY / 18);
                                while (num30 > 1)
                                {
                                    num30 -= 2;
                                }
                                int num32 = Player.tileTargetX - num30;
                                int num33 = Player.tileTargetY - num31;
                            }
                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 10 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 11)
                            {
                                this.showItemIcon = true;
                                this.showItemIcon2 = 25;
                            }
                            if (this.controlUseTile)
                            {
                                if (this.releaseUseTile)
                                {
                                    if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 4 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 13 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 33 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 49 || (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 50 && Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameX == 90))
                                    {
                                        WorldModify.KillTile(Player.tileTargetX, Player.tileTargetY, false, false, false);
                                    }
                                    else
                                    {
                                        if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 79)
                                        {
                                            int num34 = Player.tileTargetX;
                                            int num35 = Player.tileTargetY;
                                            num34 += (int)(Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameX / 18 * -1);
                                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameX >= 72)
                                            {
                                                num34 += 4;
                                                num34++;
                                            }
                                            else
                                            {
                                                num34 += 2;
                                            }
                                            num35 += (int)(Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameY / 18 * -1);
                                            num35 += 2;
                                            if (Player.CheckSpawn(num34, num35))
                                            {
                                                this.ChangeSpawn(num34, num35);
                                            }
                                        }
                                        else
                                        {
                                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 55)
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
                                                    int num38 = (int)(Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameX / 18);
                                                    int num39 = (int)(Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameY / 18);
                                                    while (num38 > 1)
                                                    {
                                                        num38 -= 2;
                                                    }
                                                    int num40 = Player.tileTargetX - num38;
                                                    int num41 = Player.tileTargetY - num39;
                                                    if (Main.tile.At(num40, num41).Type == 55)
                                                    {
                                                        NetMessage.SendData(46, -1, -1, "", num40, (float)num41);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 10)
                                                {
                                                    WorldModify.OpenDoor(Player.tileTargetX, Player.tileTargetY, this.direction);
                                                    NetMessage.SendData(19, -1, -1, "", 0, (float)Player.tileTargetX, (float)Player.tileTargetY, (float)this.direction);
                                                }
                                                else
                                                {
                                                    if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 11)
                                                    {
                                                        if (WorldModify.CloseDoor(Player.tileTargetX, Player.tileTargetY, false))
                                                        {
                                                            NetMessage.SendData(19, -1, -1, "", 1, (float)Player.tileTargetX, (float)Player.tileTargetY, (float)this.direction);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if ((Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 21 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 29) && this.talkNPC == -1)
                                                        {
                                                            bool flag5 = false;
                                                            int num42 = Player.tileTargetX - (int)(Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameX / 18);
                                                            int num43 = Player.tileTargetY - (int)(Main.tile.At(Player.tileTargetX, Player.tileTargetY).FrameY / 18);
                                                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 29)
                                                            {
                                                                flag5 = true;
                                                            }
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
                                Rectangle rectangle2 = new Rectangle((int)(this.Position.X + (float)(this.Width / 2) - (float)(Player.tileRangeX * 16)), (int)(this.Position.Y + (float)(this.Height / 2) - (float)(Player.tileRangeY * 16)), Player.tileRangeX * 16 * 2, Player.tileRangeY * 16 * 2);
                                Rectangle value = new Rectangle((int)Main.npcs[this.talkNPC].Position.X, (int)Main.npcs[this.talkNPC].Position.Y, Main.npcs[this.talkNPC].Width, Main.npcs[this.talkNPC].Height);
                                if (!rectangle2.Intersects(value) || this.chest != -1 || !Main.npcs[this.talkNPC].Active)
                                {
                                    this.talkNPC = -1;
                                    Main.npcChatText = "";
                                }
                            }
                            if (this.sign >= 0)
                            {
                                Rectangle rectangle3 = new Rectangle((int)(this.Position.X + (float)(this.Width / 2) - (float)(Player.tileRangeX * 16)), (int)(this.Position.Y + (float)(this.Height / 2) - (float)(Player.tileRangeY * 16)), Player.tileRangeX * 16 * 2, Player.tileRangeY * 16 * 2);
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
                            Rectangle rectangle4 = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);
                            for (int num45 = 0; num45 < NPC.MAX_NPCS; num45++)
                            {
                                if (Main.npcs[num45].Active && !Main.npcs[num45].friendly && rectangle4.Intersects(new Rectangle((int)Main.npcs[num45].Position.X, (int)Main.npcs[num45].Position.Y, Main.npcs[num45].Width, Main.npcs[num45].Height)))
                                {
                                    int hitDirection = -1;
                                    if (Main.npcs[num45].Position.X + (float)(Main.npcs[num45].Width / 2) < this.Position.X + (float)(this.Width / 2))
                                    {
                                        hitDirection = 1;
                                    }
                                    this.Hurt(Main.npcs[num45].damage, hitDirection, false, false);
                                }
                            }
                            Vector2 vector = Collision.HurtTiles(this.Position, this.Velocity, this.Width, this.Height, this.fireWalk);
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
                        if (Collision.StickyTiles(this.Position, this.Velocity, this.Width, this.Height))
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
                        bool flag6 = Collision.DrownCollision(this.Position, this.Width, this.Height);
                        if (selectedItem.Type == 186)
                        {
                            try
                            {
                                int num55 = (int)((this.Position.X + (float)(this.Width / 2) + (float)(6 * this.direction)) / 16f);
                                int num56 = (int)((this.Position.Y - 44f) / 16f);
                                if (Main.tile.At(num55, num56).Liquid < 128)
                                {
                                    if (!Main.tile.At(num55, num56).Active || !Main.tileSolid[(int)Main.tile.At(num55, num56).Type] || Main.tileSolidTop[(int)Main.tile.At(num55, num56).Type])
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
                        bool flag7 = Collision.LavaCollision(this.Position, this.Width, this.Height);
                        if (flag7)
                        {
                            if (Main.myPlayer == i)
                            {
                                this.Hurt(100, 0, false, false);
                            }
                            this.lavaWet = true;
                        }
                        bool flag8 = Collision.WetCollision(this.Position, this.Width, this.Height);
                        if (flag8)
                        {
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
                                Vector2 vector3 = this.Velocity;
                                this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.Width, this.Height, this.controlDown, false);
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
                            this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.Width, this.Height, this.controlDown, false);
                            this.Position += this.Velocity;
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
			Item[] array = new Item[MAX_INVENTORY];
			for (int i = 0; i < MAX_INVENTORY; i++)
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
		
        public void AdjTiles()
		{
			int num = 4;
			int num2 = 3;
			for (int i = 0; i < 80; i++)
			{
				this.oldAdjTile[i] = this.adjTile[i];
				this.adjTile[i] = false;
			}
			int num3 = (int)((this.Position.X + (float)(this.Width / 2)) / 16f);
			int num4 = (int)((this.Position.Y + (float)this.Height) / 16f);
			for (int j = num3 - num; j <= num3 + num; j++)
			{
				for (int k = num4 - num2; k < num4 + num2; k++)
				{
					if (Main.tile.At(j, k).Active)
					{
						this.adjTile[(int)Main.tile.At(j, k).Type] = true;
						if (Main.tile.At(j, k).Type == 77)
						{
							this.adjTile[17] = true;
						}
					}
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
						if (Main.tileSolid[(int)Main.tile.At(i, j).Type] && !Main.tileSolidTop[(int)Main.tile.At(i, j).Type])
						{
							if (Main.tile.At(i, j).Liquid > 0)
							{
								Main.tile.At(i, j).SetLava (false);
								Main.tile.At(i, j).SetLiquid (0);
								WorldModify.SquareTileFrame(i, j, true);
							}
							WorldModify.KillTile(i, j, false, false, false);
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

        public double Hurt(int Damage, int hitDirection, bool pvp = false, bool quiet = false, String deathText = " was slain...")
        {
            if (!this.immune)
            {
                int num = Damage;

                PlayerHurtEvent playerEvent = new PlayerHurtEvent();
                playerEvent.Sender = this;
                playerEvent.Damage = Damage;
                Program.server.PluginManager.processHook(Hooks.PLAYER_HURT, playerEvent);
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

        public void KillMe(double dmg, int hitDirection, bool pvp = false, String deathText = " was slain...")
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

            if (this.hardCore)
            {
                float num = (float)Main.rand.Next(-35, 36) * 0.1f;
                while (num < 2f && num > -2f)
                {
                    num += (float)Main.rand.Next(-30, 31) * 0.1f;
                }
                int num2 = Projectile.NewProjectile(this.Position.X + (float)(this.Width / 2), this.Position.Y + (float)(this.head / 2), (float)Main.rand.Next(10, 30) * 0.1f * (float)hitDirection + num, (float)Main.rand.Next(-40, -20) * 0.1f, ProjectileType.TOMBSTONE, this.statLifeMax + this.statManaMax, 0f, Main.myPlayer);
                Main.projectile[num2].miscText = this.Name + deathText;
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

            this.dead = true;
            this.respawnTimer = 600;
            this.immuneAlpha = 0;
            NetMessage.SendData(25, -1, -1, this.Name + deathText, 255, 225f, 25f, 25f);

            if (!pvp && this.whoAmi == Main.myPlayer && !this.hardCore)
            {
                this.DropCoins();
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
					num2 = MAX_INVENTORY + i;
				}
				if (this.inventory[num2].Type > 0 && this.inventory[num2].Stack < this.inventory[num2].MaxStack && newItem.IsTheSameAs(this.inventory[num2]))
				{
					if (newItem.Stack + this.inventory[num2].Stack <= this.inventory[num2].MaxStack)
					{
						this.inventory[num2].Stack += newItem.Stack;
						this.DoCoins(num2);
                        //if (plr == Main.myPlayer)
                        //{
                        //    Recipe.FindRecipes();
                        //}
						return new Item();
					}
					newItem.Stack -= this.inventory[num2].MaxStack - this.inventory[num2].Stack;
					this.inventory[num2].Stack = this.inventory[num2].MaxStack;
					this.DoCoins(num2);
                    //if (plr == Main.myPlayer)
                    //{
                    //    Recipe.FindRecipes();
                    //}
				}
			}
			for (int j = num; j < 40; j++)
			{
				int num3 = j;
				if (num3 < 0)
				{
					num3 = MAX_INVENTORY + j;
				}
				if (this.inventory[num3].Type == 0)
				{
					this.inventory[num3] = newItem;
					this.DoCoins(num3);
                    //if (plr == Main.myPlayer)
                    //{
                    //    Recipe.FindRecipes();
                    //}
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
                        if (Main.projectile[j].Active && Main.projectile[j].Owner == Main.myPlayer && Main.projectile[j].type == selectedItem.Shoot)
                        {
                            flag = false;
                        }
                    }
                }

                if (selectedItem.Potion)
                {
                    if (this.potionDelay <= 0)
                    {
                        potionDelay = Item.POTION_DELAY;
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
                        if (Main.projectile[j].Active && Main.projectile[j].Owner == i && Main.projectile[j].type == selectedItem.Shoot)
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
                    itemLocation.X = Position.X + (float)Width * 0.5f + 20f * (float)direction;
                }
                itemLocation.Y = Position.Y + 24f;
                itemRotation = 0f;
            }
            else if (selectedItem.HoldStyle == 2)
            {
                itemLocation.X = Position.X + (float)Width * 0.5f + (float)(6 * direction);
                itemLocation.Y = Position.Y + 16f;
                itemRotation = 0.79f * (float)(-(float)direction);
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
                            if (Main.projectile[j].Active && Main.projectile[j].Owner == i)
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
                        Vector2 vector = new Vector2(Position.X + (float)Width * 0.5f, Position.Y + (float)Height * 0.5f);
                        if (shoot == ProjectileType.STARFURY)
                        {
                            vector = new Vector2(Position.X + (float)Width * 0.5f + (float)(Main.rand.Next(601) * -(float)direction), Position.Y + (float)Height * 0.5f - 300f - (float)Main.rand.Next(100));
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
                        && (Position.X + (float)Width) / 16f + (float)Player.tileRangeX + (float)selectedItem.TileBoost - 1f >= (float)Player.tileTargetX
                        && Position.Y / 16f - (float)Player.tileRangeY - (float)selectedItem.TileBoost <= (float)Player.tileTargetY
                        && (this.Position.Y + (float)this.Height) / 16f + (float)Player.tileRangeY + (float)selectedItem.TileBoost - 2f >= (float)Player.tileTargetY)
                    {
                        showItemIcon = true;

                        if (itemTime == 0 && itemAnimation > 0 && controlUseItem)
                        {
                            if (selectedItem.Type == 205)
                            {
                                bool lava = Main.tile.At(Player.tileTargetX, Player.tileTargetY).Lava;
                                int num10 = 0;
                                for (int x = Player.tileTargetX - 1; x <= Player.tileTargetX + 1; x++)
                                {
                                    for (int y = Player.tileTargetY - 1; y <= Player.tileTargetY + 1; y++)
                                    {
                                        if (Main.tile.At(x, y).Lava == lava)
                                        {
                                            num10 += (int)Main.tile.At(x, y).Liquid;
                                        }
                                    }
                                }

                                if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Liquid > 0 && num10 > 100)
                                {
                                    bool lava2 = Main.tile.At(Player.tileTargetX, Player.tileTargetY).Lava;
                                    if (!Main.tile.At(Player.tileTargetX, Player.tileTargetY).Lava)
                                    {
                                        selectedItem = Registries.Item.Create(206);
                                    }
                                    else
                                    {
                                        selectedItem = Registries.Item.Create(207);
                                    }
                                    inventory[selectedItemIndex] = selectedItem;

                                    itemTime = selectedItem.UseTime;
                                    int num11 = (int)Main.tile.At(Player.tileTargetX, Player.tileTargetY).Liquid;
                                    Main.tile.At(Player.tileTargetX, Player.tileTargetY).SetLiquid (0);
                                    Main.tile.At(Player.tileTargetX, Player.tileTargetY).SetLava (false);
                                    WorldModify.SquareTileFrame(Player.tileTargetX, Player.tileTargetY, false);

                                    Liquid.AddWater(Player.tileTargetX, Player.tileTargetY);

                                    for (int x = Player.tileTargetX - 1; x <= Player.tileTargetX + 1; x++)
                                    {
                                        for (int y = Player.tileTargetY - 1; y <= Player.tileTargetY + 1; y++)
                                        {
                                            if (num11 < 256 && Main.tile.At(x, y).Lava == lava)
                                            {
                                                int num12 = (int)Main.tile.At(x, y).Liquid;

                                                if (num12 + num11 > 255)
                                                {
                                                    num12 = 255 - num11;
                                                }

                                                num11 += num12;
                                                TileRef expr_20A0 = Main.tile.At(x, y);
                                                expr_20A0.SetLiquid ((byte) (expr_20A0.Liquid - (byte)num12));
                                                Main.tile.At(x, y).SetLava (lava2);

                                                if (Main.tile.At(x, y).Liquid == 0)
                                                {
                                                    Main.tile.At(x, y).SetLava (false);
                                                }

                                                WorldModify.SquareTileFrame(x, y, false);

                                                Liquid.AddWater(x, y);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Liquid < 200)
                            {
                                if (!Main.tile.At(Player.tileTargetX, Player.tileTargetY).Active || !Main.tileSolid[(int)Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type] || !Main.tileSolidTop[(int)Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type])
                                {
                                    if (selectedItem.Type == 207)
                                    {
                                        if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Liquid == 0 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Lava)
                                        {
                                            Main.tile.At(Player.tileTargetX, Player.tileTargetY).SetLava (true);
                                            Main.tile.At(Player.tileTargetX, Player.tileTargetY).SetLiquid (255);
                                            WorldModify.SquareTileFrame(Player.tileTargetX, Player.tileTargetY, true);
                                            selectedItem = Registries.Item.Create(205);
                                            inventory[selectedItemIndex] = selectedItem;
                                            this.itemTime = selectedItem.UseTime;
                                        }
                                    }
                                    else if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Liquid == 0 || !Main.tile.At(Player.tileTargetX, Player.tileTargetY).Lava)
                                    {
                                        Main.tile.At(Player.tileTargetX, Player.tileTargetY).SetLava (false);
                                        Main.tile.At(Player.tileTargetX, Player.tileTargetY).SetLiquid (255);
                                        WorldModify.SquareTileFrame(Player.tileTargetX, Player.tileTargetY, true);
                                        selectedItem = Registries.Item.Create(205);
                                        inventory[selectedItemIndex] = selectedItem;
                                        this.itemTime = selectedItem.UseTime;
                                    }
                                }
                            }
                        }
                    }
                }

                if (selectedItem.Pick > 0 || selectedItem.Axe > 0 || selectedItem.Hammer > 0)
                {
                    if (Position.X / 16f - (float)Player.tileRangeX - (float)selectedItem.TileBoost <= (float)Player.tileTargetX 
                        && (Position.X + (float)Width) / 16f + (float)Player.tileRangeX + (float)selectedItem.TileBoost - 1f >= (float)Player.tileTargetX 
                        && Position.Y / 16f - (float)Player.tileRangeY - (float)selectedItem.TileBoost <= (float)Player.tileTargetY 
                        && (Position.Y + (float)Height) / 16f + (float)Player.tileRangeY + (float)selectedItem.TileBoost - 2f >= (float)Player.tileTargetY)
                    {
                        showItemIcon = true;
                        if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Active)
                        {
                            if (itemTime == 0 && itemAnimation > 0 && controlUseItem)
                            {
                                if (hitTileX != Player.tileTargetX || hitTileY != Player.tileTargetY)
                                {
                                    hitTile = 0;
                                    hitTileX = Player.tileTargetX;
                                    hitTileY = Player.tileTargetY;
                                }
                                if (Main.tileNoFail[(int)Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type])
                                {
                                    hitTile = 100;
                                }
                                if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type != 27)
                                {
                                    if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 4 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 10 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 11 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 12 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 13 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 14 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 15 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 16 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 17 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 18 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 19 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 21 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 26 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 28 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 29 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 31 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 33 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 34 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 35 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 36 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 42 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 48 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 49 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 50 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 54 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 55 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 77 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 78 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 79)
                                    {
                                        if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 48)
                                        {
                                            hitTile += selectedItem.Hammer / 3;
                                        }
                                        else
                                        {
                                            hitTile += selectedItem.Hammer;
                                        }

                                        if ((double)Player.tileTargetY > Main.rockLayer 
                                            && Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 77 
                                            && selectedItem.Hammer < 60)
                                        {
                                            hitTile = 0;
                                        }

                                        if (selectedItem.Hammer > 0)
                                        {
                                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 26)
                                            {
                                                Hurt(this.statLife / 2, -direction, false, false);
                                                WorldModify.KillTile(Player.tileTargetX, Player.tileTargetY, true, false, false);
                                            }
                                            else if (hitTile >= 100)
                                            {
                                                hitTile = 0;
                                                WorldModify.KillTile(Player.tileTargetX, Player.tileTargetY, false, false, false);
                                            }
                                            else
                                            {
                                                WorldModify.KillTile(Player.tileTargetX, Player.tileTargetY, true, false, false);
                                            }

                                            itemTime = inventory[this.selectedItemIndex].UseTime;
                                        }
                                    }
                                    else
                                    {
                                        if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 5 
                                            || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 30 
                                            || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 72)
                                        {
                                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 30)
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
                                                    WorldModify.KillTile(Player.tileTargetX, Player.tileTargetY, false, false, false);
                                                }
                                                else
                                                {
                                                    WorldModify.KillTile(Player.tileTargetX, Player.tileTargetY, true, false, false);
                                                }
                                                this.itemTime = selectedItem.UseTime;
                                            }
                                        }
                                        else
                                        {
                                            if (selectedItem.Pick > 0)
                                            {
                                                if (Main.tileDungeon[(int)Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type] || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 37 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 25 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 58)
                                                {
                                                    hitTile += selectedItem.Pick / 2;
                                                }
                                                else
                                                {
                                                    hitTile += selectedItem.Pick;
                                                }
                                                if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 25 && selectedItem.Pick < 65)
                                                {
                                                    hitTile = 0;
                                                }
                                                else
                                                {
                                                    if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 37 && selectedItem.Pick < 55)
                                                    {
                                                        hitTile = 0;
                                                    }
                                                    else
                                                    {
                                                        if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 56 && selectedItem.Pick < 65)
                                                        {
                                                            hitTile = 0;
                                                        }
                                                        else
                                                        {
                                                            if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 58 && selectedItem.Pick < 65)
                                                            {
                                                                hitTile = 0;
                                                            }
                                                            else
                                                            {
                                                                if (Main.tileDungeon[(int)Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type] && selectedItem.Pick < 65)
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
                                                if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 0 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 40 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 53 || Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 59)
                                                {
                                                    hitTile += selectedItem.Pick;
                                                }
                                                if (hitTile >= 100)
                                                {
                                                    hitTile = 0;
                                                    WorldModify.KillTile(Player.tileTargetX, Player.tileTargetY, false, false, false);
                                                }
                                                else
                                                {
                                                    WorldModify.KillTile(Player.tileTargetX, Player.tileTargetY, true, false, false);
                                                }
                                                this.itemTime = selectedItem.UseTime;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Wall > 0)
                        {
                            if (this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
                            {
                                if (selectedItem.Hammer > 0)
                                {
                                    bool flag3 = true;
                                    if (!Main.wallHouse[(int)Main.tile.At(Player.tileTargetX, Player.tileTargetY).Wall])
                                    {
                                        flag3 = false;
                                        for (int k = Player.tileTargetX - 1; k < Player.tileTargetX + 2; k++)
                                        {
                                            for (int l = Player.tileTargetY - 1; l < Player.tileTargetY + 2; l++)
                                            {
                                                if (Main.tile.At(k, l).Wall != Main.tile.At(Player.tileTargetX, Player.tileTargetY).Wall)
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
                                            WorldModify.KillWall(Player.tileTargetX, Player.tileTargetY, false);
                                        }
                                        else
                                        {
                                            WorldModify.KillWall(Player.tileTargetX, Player.tileTargetY, true);
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
                    if (this.Position.X / 16f - (float)Player.tileRangeX - (float)selectedItem.TileBoost <= (float)Player.tileTargetX && (this.Position.X + (float)this.Width) / 16f + (float)Player.tileRangeX + (float)selectedItem.TileBoost - 1f >= (float)Player.tileTargetX && this.Position.Y / 16f - (float)Player.tileRangeY - (float)selectedItem.TileBoost <= (float)Player.tileTargetY && (this.Position.Y + (float)this.Height) / 16f + (float)Player.tileRangeY + (float)selectedItem.TileBoost - 2f >= (float)Player.tileTargetY)
                    {
                        this.showItemIcon = true;
                        if (!Main.tile.At(Player.tileTargetX, Player.tileTargetY).Active || selectedItem.CreateTile == 23 || selectedItem.CreateTile == 2 || selectedItem.CreateTile == 60 || selectedItem.CreateTile == 70)
                        {
                            if (this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
                            {
                                bool flag4 = false;
                                if (selectedItem.CreateTile == 23 || selectedItem.CreateTile == 2)
                                {
                                    if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Active && Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 0)
                                    {
                                        flag4 = true;
                                    }
                                }
                                else
                                {
                                    if (selectedItem.CreateTile == 60 || selectedItem.CreateTile == 70)
                                    {
                                        if (Main.tile.At(Player.tileTargetX, Player.tileTargetY).Active && Main.tile.At(Player.tileTargetX, Player.tileTargetY).Type == 59)
                                        {
                                            flag4 = true;
                                        }
                                    }
                                    else
                                    {
                                        if (selectedItem.CreateTile == 4)
                                        {
                                            int num13 = (int)Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Type;
                                            int num14 = (int)Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY).Type;
                                            int num15 = (int)Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY).Type;
                                            int num16 = (int)Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY - 1).Type;
                                            int num17 = (int)Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY - 1).Type;
                                            int num18 = (int)Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY - 1).Type;
                                            int num19 = (int)Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY + 1).Type;
                                            if (!Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Active)
                                            {
                                                num13 = -1;
                                            }
                                            if (!Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY).Active)
                                            {
                                                num14 = -1;
                                            }
                                            if (!Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY).Active)
                                            {
                                                num15 = -1;
                                            }
                                            if (!Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY - 1).Active)
                                            {
                                                num16 = -1;
                                            }
                                            if (!Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY - 1).Active)
                                            {
                                                num17 = -1;
                                            }
                                            if (!Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY + 1).Active)
                                            {
                                                num18 = -1;
                                            }
                                            if (!Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY + 1).Active)
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
                                                if (Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Active && (Main.tileSolid[(int)Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Type] || Main.tileTable[(int)Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Type]))
                                                {
                                                    flag4 = true;
                                                }
                                            }
                                            else
                                            {
                                                if (selectedItem.CreateTile == 13 || selectedItem.CreateTile == 29 || selectedItem.CreateTile == 33 || selectedItem.CreateTile == 49)
                                                {
                                                    if (Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Active && Main.tileTable[(int)Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Type])
                                                    {
                                                        flag4 = true;
                                                    }
                                                }
                                                else
                                                {
                                                    if (selectedItem.CreateTile == 51)
                                                    {
                                                        if (Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY).Active || Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY).Wall > 0 || Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY).Active || Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY).Wall > 0 || Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Active || Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Wall > 0 || Main.tile.At(Player.tileTargetX, Player.tileTargetY - 1).Active || Main.tile.At(Player.tileTargetX, Player.tileTargetY - 1).Wall > 0)
                                                        {
                                                            flag4 = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if ((Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY).Active && Main.tileSolid[(int)Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY).Type]) || Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY).Wall > 0 || (Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY).Active && Main.tileSolid[(int)Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY).Type]) || Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY).Wall > 0 || (Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Active && Main.tileSolid[(int)Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Type]) || Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Wall > 0 || (Main.tile.At(Player.tileTargetX, Player.tileTargetY - 1).Active && Main.tileSolid[(int)Main.tile.At(Player.tileTargetX, Player.tileTargetY - 1).Type]) || Main.tile.At(Player.tileTargetX, Player.tileTargetY - 1).Wall > 0)
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
                                    if (WorldModify.PlaceTile(Player.tileTargetX, Player.tileTargetY, selectedItem.CreateTile, false, false, this.whoAmi))
                                    {
                                        this.itemTime = selectedItem.UseTime;
                                        if (selectedItem.CreateTile == 15)
                                        {
                                            if (this.direction == 1)
                                            {
                                                TileRef expr_40C8 = Main.tile.At(Player.tileTargetX, Player.tileTargetY);
                                                expr_40C8.SetFrameX ((short) (expr_40C8.FrameX + 18));
                                                TileRef expr_40ED = Main.tile.At(Player.tileTargetX, Player.tileTargetY - 1);
                                                expr_40ED.SetFrameX ((short) (expr_40ED.FrameX + 18));
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
                    if (this.Position.X / 16f - (float)Player.tileRangeX - (float)selectedItem.TileBoost <= (float)Player.tileTargetX && (this.Position.X + (float)this.Width) / 16f + (float)Player.tileRangeX + (float)selectedItem.TileBoost - 1f >= (float)Player.tileTargetX && this.Position.Y / 16f - (float)Player.tileRangeY - (float)selectedItem.TileBoost <= (float)Player.tileTargetY && (this.Position.Y + (float)this.Height) / 16f + (float)Player.tileRangeY + (float)selectedItem.TileBoost - 2f >= (float)Player.tileTargetY)
                    {
                        this.showItemIcon = true;
                        if (this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
                        {
                            if (Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY).Active || Main.tile.At(Player.tileTargetX + 1, Player.tileTargetY).Wall > 0 || Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY).Active || Main.tile.At(Player.tileTargetX - 1, Player.tileTargetY).Wall > 0 || Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Active || Main.tile.At(Player.tileTargetX, Player.tileTargetY + 1).Wall > 0 || Main.tile.At(Player.tileTargetX, Player.tileTargetY - 1).Active || Main.tile.At(Player.tileTargetX, Player.tileTargetY - 1).Wall > 0)
                            {
                                if ((int)Main.tile.At(Player.tileTargetX, Player.tileTargetY).Wall != selectedItem.CreateWall)
                                {
                                    WorldModify.PlaceWall(Player.tileTargetX, Player.tileTargetY, selectedItem.CreateWall, false);
                                    if ((int)Main.tile.At(Player.tileTargetX, Player.tileTargetY).Wall == selectedItem.CreateWall)
                                    {
                                        this.itemTime = selectedItem.UseTime;
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
                                    if (Main.tile.At(k, l).Type == 3 || Main.tile.At(k, l).Type == 24 || Main.tile.At(k, l).Type == 28 || Main.tile.At(k, l).Type == 32 || Main.tile.At(k, l).Type == 51 || Main.tile.At(k, l).Type == 52 || Main.tile.At(k, l).Type == 61 || Main.tile.At(k, l).Type == 62 || Main.tile.At(k, l).Type == 69 || Main.tile.At(k, l).Type == 71 || Main.tile.At(k, l).Type == 73 || Main.tile.At(k, l).Type == 74)
                                    {
                                        WorldModify.KillTile(k, l, false, false, false);
                                    }
                                }
                            }
                            for (int j = 0; j < NPC.MAX_NPCS; j++)
                            {
                                if (Main.npcs[j].Active && Main.npcs[j].immune[i] == 0 && this.attackCD == 0 && !Main.npcs[j].friendly)
                                {
                                    Rectangle value = new Rectangle((int)Main.npcs[j].Position.X, (int)Main.npcs[j].Position.Y, Main.npcs[j].Width, Main.npcs[j].Height);
                                    if (rectangle.Intersects(value))
                                    {
                                        if (Main.npcs[j].noTileCollide || Collision.CanHit(this.Position, this.Width, this.Height, Main.npcs[j].Position, Main.npcs[j].Width, Main.npcs[j].Height))
                                        {
                                            Main.npcs[j].StrikeNPC(selectedItem.Damage, selectedItem.KnockBack, this.direction);
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
                                            Rectangle value2 = new Rectangle((int)Main.players[j].Position.X, (int)Main.players[j].Position.Y, Main.players[j].Width, Main.players[j].Height);
                                            if (rectangle.Intersects(value2))
                                            {
                                                if (Collision.CanHit(this.Position, this.Width, this.Height, Main.players[j].Position, Main.players[j].Width, Main.players[j].Height))
                                                {
                                                    Main.players[j].Hurt(selectedItem.Damage, this.direction, true, false);
                                                    NetMessage.SendData(26, -1, -1, "", j, (float)this.direction, (float)selectedItem.Damage, 1f);
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
                else
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
                if (this.itemTime == 0)
                {
                    this.itemTime = selectedItem.UseTime;
                }
                else
                {
                    if (this.itemTime == selectedItem.UseTime / 2)
                    {
                        this.grappling[0] = -1;
                        this.grapCount = 0;
                        for (int j = 0; j < 1000; j++)
                        {
                            if (Main.projectile[j].Active && Main.projectile[j].Owner == i)
                            {
                                if (Main.projectile[j].aiStyle == 7)
                                {
                                    Main.projectile[j].Kill();
                                }
                            }
                        }
                        this.Spawn();
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
		
        public override object Clone()
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
			if (!Main.tile.At(x, y - 1).Active || Main.tile.At(x, y - 1).Type != 79)
			{
				return false;
			}
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = y - 3; j < y; j++)
				{
					if (! Main.tile.At(i, j).Exists)
					{
						return false;
					}
					if (Main.tile.At(i, j).Active && Main.tileSolid[(int)Main.tile.At(i, j).Type] && !Main.tileSolidTop[(int)Main.tile.At(i, j).Type])
					{
						return false;
					}
				}
			}
			return WorldModify.StartRoomCheck(x, y - 1);
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
            for (int i = 0; i < MAX_INVENTORY; i++)
            {
                if (type == this.inventory[i].Type)
                {
                    return true;
                }
            }
            return false;
        }
		
        public Player()
		{
            Width = 20;
		    Height = 42;
			
			PluginData = System.Collections.Hashtable.Synchronized (new System.Collections.Hashtable());
			
			for (int i = 0; i < MAX_INVENTORY; i++)
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
			this.inventory[0] = Registries.Item.Create("Copper Pickaxe");
            this.inventory[1] = Registries.Item.Create("Copper Axe");
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
				Main.projectile[proj].Name, 
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
                        result = text + " by " + Main.projectile[proj].Name + ".";
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

        public ServerSlot Slot
        {
            get { return Netplay.slots[this.whoAmi]; }
        }

        public void Kick(String Reason = null)
        {
            String message = "You have been Kicked from this Server.";

            if (Reason != null)
            {
                message = Reason;
            }

            Netplay.slots[whoAmi].Kick (message);
        }

        public String IPAddress
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

        public Vector2 Location
        {
            get
            {
                return Position;
            }
            set {
                Position = value;
            }
        }

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

        public bool teleportTo(float tileX, float tileY)
        {
            PlayerTeleportEvent playerEvent = new PlayerTeleportEvent();
            playerEvent.ToLocation = new Vector2(tileX, tileY);
            playerEvent.FromLocation = new Vector2(this.Position.X, this.Position.Y);
            playerEvent.Sender = this;
            Program.server.PluginManager.processHook(Hooks.PLAYER_TELEPORT, playerEvent);
            if (playerEvent.Cancelled)
            {
                return false;
            }

            //Preserve our Spawn point.
            int spawnTileX = Main.spawnTileX;
            int spawnTileY = Main.spawnTileY;

            //Set our new target position
            Main.spawnTileX = ((int)tileX) / 16;
            Main.spawnTileY = ((int)tileY) / 16;

            bool destroyed = false;
            if (Main.players[this.whoAmi].SpawnX >= 0 && Main.players[this.whoAmi].SpawnY >= 0) //Do they have a bed?
            {
                if (bedDestruction) //Do they want their bed destroyed?
                {
                    NetMessage.SendData((int)Packet.WORLD_DATA, this.whoAmi);
                    Main.tile.At((int)tileX, (int)tileY - 1).SetActive(false);
                    NetMessage.SendTileSquare(this.whoAmi, (int)tileX, (int)tileY - 1, 200);
                    NetMessage.SendData((int)Packet.RECEIVING_PLAYER_JOINED, this.whoAmi, -1, "", this.whoAmi, 0f, 0f, 0f);
                    Main.tile.At((int)tileX, (int)tileY - 1).SetActive(true);
                    destroyed = true;
                }
            }
            else
            {
                NetMessage.SendData((int)Packet.WORLD_DATA, this.whoAmi);
                NetMessage.SendData((int)Packet.RECEIVING_PLAYER_JOINED, this.whoAmi, -1, "", this.whoAmi, 0f, 0f, 0f);
            }
            //Return to defaults
            Main.spawnTileX = spawnTileX;
            Main.spawnTileY = spawnTileY;
            NetMessage.SendData((int)Packet.WORLD_DATA, this.whoAmi);
            return destroyed;
        }

        public void teleportTo(Player player)
        {
            this.teleportTo(player.Position.X, player.Position.Y);
        }

        public static String GetPlayerPassword(String PlayerName, Server Server)
        {
            foreach (String listee in Server.OpList.WhiteList)
            {
                if (listee != null && listee.Trim().ToLower().Length > 0)
                {
                    String userPass = listee.Trim().ToLower();
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

        public String Password
        {
            get
            {
                return Player.GetPlayerPassword(this.Name, Program.server);
            }
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
            return Player.isInOpList(this.Name, Program.server);            
        }

        public String GetOpListKey
        {
            get
            {
                return this.Name.Trim().ToLower() + Password;
            }
        }

        public bool HasHackedData()
        {
            if (!Program.properties.HackedData)
            {
                if (statMana > MAX_MANA || statManaMax > MAX_MANA ||
                    statLife > MAX_HEALTH || statLifeMax > MAX_HEALTH)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
