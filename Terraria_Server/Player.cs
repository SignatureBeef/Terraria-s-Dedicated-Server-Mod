using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Terraria_Server.Commands;

namespace Terraria_Server
{
    public class Player : Sender
    {
		private int tileTargetX;
		private int tileTargetY;
		private int jumpHeight = 15;
		private float jumpSpeed = 5.01f;

        private int itemGrabRange = 38;
        private float itemGrabSpeed = 0.45f;
        private float itemGrabSpeedMax = 4f;

        public bool pvpDeath;
        public bool zoneDungeon;
        public bool zoneEvil;
        public bool zoneMeteor;
        public bool zoneJungle;
        public bool boneArmor;
        public int townNPCs;
        public Vector2 position = new Vector2();
        public Vector2 velocity = new Vector2();
        public Vector2 oldVelocity = new Vector2();
        public double headFrameCounter;
        public double bodyFrameCounter;
        public double legFrameCounter;
        public bool immune;
        public int immuneTime;
        public int immuneAlphaDirection;
        public string playerPathName;
        public int immuneAlpha;
        public int team;
        public string chatText = "";
        public int sign = -1;
        public int chatShowTime;
        public int activeNPCs;
        public bool mouseInterface;
        public int changeItem = -1;
        public int selectedItem;
        public Item[] armor = new Item[8];
        public int itemAnimation;
        public int itemAnimationMax;
        public int itemTime;
        public float itemRotation;
        public int itemWidth;
        public int itemHeight;
        public Vector2 itemLocation = new Vector2();
        public int breathCD;
        public int breathMax = 200;
        public int breath = 200;
        public string setBonus = "";
        public Item[] inventory = new Item[44];
        public Item[] bank = new Item[Chest.maxItems];
        public float headRotation;
        public float bodyRotation;
        public float legRotation;
        public Vector2 headPosition = new Vector2();
        public Vector2 bodyPosition = new Vector2();
        public Vector2 legPosition = new Vector2();
        public Vector2 headVelocity = new Vector2();
        public Vector2 bodyVelocity = new Vector2();
        public Vector2 legVelocity = new Vector2();
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
        public int SpawnX = -1;
        public int SpawnY = -1;
        public int[] spX = new int[200];
        public int[] spY = new int[200];
        public string[] spN = new string[200];
        public int[] spI = new int[200];
        public static int tileRangeX = 5;
        public static int tileRangeY = 4;
        //private static int tileTargetX;
        //private static int tileTargetY;
        //private static int jumpHeight = 15;
        //private static float jumpSpeed = 5.01f;
        public bool[] adjTile = new bool[80];
        public bool[] oldAdjTile = new bool[80];
        //private static int itemGrabRange = 38;
        //private static float itemGrabSpeed = 0.45f;
        //private static float itemGrabSpeedMax = 4f;
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

        public World world = null;

        public void HealEffect(int healAmount)
        {
            //CombatText.NewText(new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height), new Color(100, 100, 255, 255), string.Concat(healAmount));
            //if (Main.netMode == 1 && this.whoAmi == Main.myPlayer)
            //{
            ////    NetMessage.SendData(35, -1, -1, "", this.whoAmi, (float)healAmount, 0f, 0f);
           // }
        }
        
        public void ManaEffect(int manaAmount)
        {
            //CombatText.NewText(new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height), new Color(180, 50, 255, 255), string.Concat(manaAmount));
            //if (Statics.netMode == 1 && this.whoAmi == Statics.myPlayer)
            //{
             //   NetMessage.SendData(43, -1, -1, "", this.whoAmi, (float)manaAmount, 0f, 0f);
            //}
        }

        public static byte FindClosest(Vector2 Position, int Width, int Height, World world)
        {
            byte result = 0;
            for (int i = 0; i < 8; i++)
            {
                if (world.getPlayerList()[i] != null && world.getPlayerList()[i].active)
                {
                    result = (byte)i;
                    break;
                }
            }
            float num = -1f;
            for (int j = 0; j < 8; j++)
            {
                if (world.getPlayerList()[j] != null && world.getPlayerList()[j].active && !world.getPlayerList()[j].dead && (num == -1f || Math.Abs(world.getPlayerList()[j].position.X + (float)(world.getPlayerList()[j].width / 2) - Position.X + (float)(Width / 2)) + Math.Abs(world.getPlayerList()[j].position.Y + (float)(world.getPlayerList()[j].height / 2) - Position.Y + (float)(Height / 2)) < num))
                {
                    num = Math.Abs(world.getPlayerList()[j].position.X + (float)(world.getPlayerList()[j].width / 2) - Position.X + (float)(Width / 2)) + Math.Abs(world.getPlayerList()[j].position.Y + (float)(world.getPlayerList()[j].height / 2) - Position.Y + (float)(Height / 2));
                    result = (byte)j;
                }
            }
            return result;
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
                                num2 += world.getProjectile()[this.grappling[i]].position.X + (float)(world.getProjectile()[this.grappling[i]].width / 2);
                                num3 += world.getProjectile()[this.grappling[i]].position.Y + (float)(world.getProjectile()[this.grappling[i]].height / 2);
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
            if (this.whoAmi == Statics.myPlayer)
            {
                this.FindSpawn(world);
                if (!Player.CheckSpawn(this.SpawnX, this.SpawnY, world))
                {
                    this.SpawnX = -1;
                    this.SpawnY = -1;
                }
            }
            if (Statics.netMode == 1 && this.whoAmi == Statics.myPlayer)
            {
                NetMessage.SendData(12, world, -1, -1, "", Statics.myPlayer, 0f, 0f, 0f);
                //Main.gameMenu = false;
            }
            this.headPosition = new Vector2();
            this.bodyPosition = new Vector2();
            this.legPosition = new Vector2();
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
                this.position.X = (float)(Statics.spawnTileX * 16 + 8 - this.width / 2);
                this.position.Y = (float)(Statics.spawnTileY * 16 - this.height);
                for (int i = Statics.spawnTileX - 1; i < Statics.spawnTileX + 2; i++)
                {
                    for (int j = Statics.spawnTileY - 3; j < Statics.spawnTileY; j++)
                    {
                        if (Statics.tileSolid[(int)world.getTile()[i, j].type] && !Statics.tileSolidTop[(int)world.getTile()[i, j].type])
                        {
                            if (world.getTile()[i, j].liquid > 0)
                            {
                                world.getTile()[i, j].lava = false;
                                world.getTile()[i, j].liquid = 0;
                                WorldGen.SquareTileFrame(i, j, world, true);
                            }
                            WorldGen.KillTile(i, j, world, false, false, false);
                        }
                    }
                }
            }
            this.wet = Collision.WetCollision(this.position, this.width, this.height, world);
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
            if (this.whoAmi == Statics.myPlayer)
            {
                //Lighting.lightCounter = Lighting.lightSkip + 1;
                //Main.screenPosition.X = this.position.X + (float)(this.width / 2) - (float)(Main.screenWidth / 2);
                //Main.screenPosition.Y = this.position.Y + (float)(this.height / 2) - (float)(Main.screenHeight / 2);
            }
        }

        public void UpdatePlayer(int i)
        {
            float num = 10f;
            float num2 = 0.4f;
            jumpHeight = 15;
            jumpSpeed = 5.01f;
            if (this.wet)
            {
                num2 = 0.2f;
                num = 5f;
                jumpHeight = 30;
                jumpSpeed = 6.01f;
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
                    if (i == Statics.myPlayer)
                    {
                        //Statics.npcChatText = "";
                        //Statics.editSign = false;
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
                    if (this.respawnTimer <= 0 && Statics.myPlayer == this.whoAmi)
                    {
                        this.Spawn();
                        return;
                    }
                }
                else
                {
                    if (i == Statics.myPlayer)
                    {
                        this.zoneEvil = false;
                        if (Statics.evilTiles >= 500)
                        {
                            this.zoneEvil = true;
                        }
                        this.zoneMeteor = false;
                        if (Statics.meteorTiles >= 50)
                        {
                            this.zoneMeteor = true;
                        }
                        this.zoneDungeon = false;
                        if (Statics.dungeonTiles >= 250 && (double)this.position.Y > world.getWorldSurface() * 16.0 + (double)Statics.screenHeight)
                        {
                            int num7 = (int)this.position.X / 16;
                            int num8 = (int)this.position.Y / 16;
                            if (world.getTile()[num7, num8].wall > 0 && !Statics.wallHouse[(int)world.getTile()[num7, num8].wall])
                            {
                                this.zoneDungeon = true;
                            }
                        }
                        this.zoneJungle = false;
                        if (Statics.jungleTiles >= 200)
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


                        if (this.controlInv)
                        {
                            if (this.releaseInventory)
                            {
                                if (this.talkNPC >= 0)
                                {
                                    this.talkNPC = -1;
                                    //Main.npcChatText = "";
                                    //Main.PlaySound(11, -1, -1, 1);
                                }
                                else
                                {
                                    if (this.sign >= 0)
                                    {
                                        this.sign = -1;
                                        //Main.editSign = false;
                                        //Main.npcChatText = "";
                                        //Main.PlaySound(11, -1, -1, 1);
                                    }
                                    else
                                    {
                                        if (!Statics.playerInventory)
                                        {
                                            Recipe.FindRecipes(world);
                                            Statics.playerInventory = true;
                                            ////Main.PlaySound(10, -1, -1, 1);
                                        }
                                        else
                                        {
                                            Statics.playerInventory = false;
                                            //Main.PlaySound(11, -1, -1, 1);
                                        }
                                    }
                                }
                            }
                            this.releaseInventory = false;
                        }
                        else
                        {
                            this.releaseInventory = true;
                        }
                        if (this.delayUseItem)
                        {
                            if (!this.controlUseItem)
                            {
                                this.delayUseItem = false;
                            }
                            this.controlUseItem = false;
                        }
                        if (this.itemAnimation == 0 && this.itemTime == 0)
                        {
                            Recipe.FindRecipes(world);
                            //if ((this.controlThrow && this.inventory[this.selectedItem].type > 0) || (((Main.mouseState.LeftButton == ButtonState.Pressed && !this.mouseInterface && Main.mouseLeftRelease) || !Main.playerInventory) && Main.mouseItem.type > 0))
                            //{
                            //    Item item = new Item();
                            //    bool flag = false;
                            //    //if (((Main.mouseState.LeftButton == ButtonState.Pressed && !this.mouseInterface && Main.mouseLeftRelease) || !Main.playerInventory) && Main.mouseItem.type > 0)
                            //    //{
                            //    //    item = this.inventory[this.selectedItem];
                            //    //    this.inventory[this.selectedItem] = Main.mouseItem;
                            //    //    this.delayUseItem = true;
                            //    //    this.controlUseItem = false;
                            //    //    flag = true;
                            //    //}
                            //    int num9 = Item.NewItem((int)this.position.X, (int)this.position.Y, world, this.width, this.height, this.inventory[this.selectedItem].type, 1, false);
                            //    if (!flag && this.inventory[this.selectedItem].type == 8 && this.inventory[this.selectedItem].stack > 1)
                            //    {
                            //        this.inventory[this.selectedItem].stack--;
                            //    }
                            //    else
                            //    {
                            //        this.inventory[this.selectedItem].position = world.getItemList()[num9].position;
                            //        world.getItemList()[num9] = this.inventory[this.selectedItem];
                            //        this.inventory[this.selectedItem] = new Item();
                            //    }
                            //    if (Statics.netMode == 0)
                            //    {
                            //        world.getItemList()[num9].noGrabDelay = 100;
                            //    }
                            //    world.getItemList()[num9].velocity.Y = -2f;
                            //    world.getItemList()[num9].velocity.X = (float)(4 * this.direction) + this.velocity.X;
                            //    //if (((Main.mouseState.LeftButton == ButtonState.Pressed && !this.mouseInterface) || !Main.playerInventory) && Main.mouseItem.type > 0)
                            //    //{
                            //    //    this.inventory[this.selectedItem] = item;
                            //    //    Main.mouseItem = new Item();
                            //    //}
                            //    //else
                            //    {
                            //        this.itemAnimation = 10;
                            //        this.itemAnimationMax = 10;
                            //        //}
                            //        Recipe.FindRecipes(world);
                            //        if (Statics.netMode == 1)
                            //        {
                            //            NetMessage.SendData(21, world, -1, -1, "", num9, 0f, 0f, 0f);
                            //        }
                            //    }
                            //    if (!Statics.playerInventory)
                            //    {
                            //        int num10 = this.selectedItem;
                            //        //if (!Main.chatMode)
                            //        //{
                            //        //    if (Main.keyState.IsKeyDown(Keys.D1))
                            //        //    {
                            //        //        this.selectedItem = 0;
                            //        //    }
                            //        //    if (Main.keyState.IsKeyDown(Keys.D2))
                            //        //    {
                            //        //        this.selectedItem = 1;
                            //        //    }
                            //        //    if (Main.keyState.IsKeyDown(Keys.D3))
                            //        //    {
                            //        //        this.selectedItem = 2;
                            //        //    }
                            //        //    if (Main.keyState.IsKeyDown(Keys.D4))
                            //        //    {
                            //        //        this.selectedItem = 3;
                            //        //    }
                            //        //    if (Main.keyState.IsKeyDown(Keys.D5))
                            //        //    {
                            //        //        this.selectedItem = 4;
                            //        //    }
                            //        //    if (Main.keyState.IsKeyDown(Keys.D6))
                            //        //    {
                            //        //        this.selectedItem = 5;
                            //        //    }
                            //        //    if (Main.keyState.IsKeyDown(Keys.D7))
                            //        //    {
                            //        //        this.selectedItem = 6;
                            //        //    }
                            //        //    if (Main.keyState.IsKeyDown(Keys.D8))
                            //        //    {
                            //        //        this.selectedItem = 7;
                            //        //    }
                            //        //    if (Main.keyState.IsKeyDown(Keys.D9))
                            //        //    {
                            //        //        this.selectedItem = 8;
                            //        //    }
                            //        //    if (Main.keyState.IsKeyDown(Keys.D0))
                            //        //    {
                            //        //        this.selectedItem = 9;
                            //        //    }
                            //        //}
                            //        //if (num10 != this.selectedItem)
                            //        //{
                            //        //    Main.PlaySound(12, -1, -1, 1);
                            //        //}
                            //        //int k;
                            //        //for (k = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 120; k > 9; k -= 10)
                            //        //{
                            //        //}
                            //        //while (k < 0)
                            //        //{
                            //        //    k += 10;
                            //        //}
                            //        //this.selectedItem -= k;
                            //        //if (k != 0)
                            //        //{
                            //        //    Main.PlaySound(12, -1, -1, 1);
                            //        //}
                            //        if (this.changeItem >= 0)
                            //        {
                            //            //if (this.selectedItem != this.changeItem)
                            //            //{
                            //            //    Main.PlaySound(12, -1, -1, 1);
                            //            //}
                            //            this.selectedItem = this.changeItem;
                            //            this.changeItem = -1;
                            //        }
                            //        while (this.selectedItem > 9)
                            //        {
                            //            this.selectedItem -= 10;
                            //        }
                            //        while (this.selectedItem < 0)
                            //        {
                            //            this.selectedItem += 10;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        //int num11 = (Statics.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 120;
                            //        //Statics.focusRecipe += num11;
                            //        //if (Main.focusRecipe > Main.numAvailableRecipes - 1)
                            //        //{
                            //        //    Statics.focusRecipe = Statics.numAvailableRecipes - 1;
                            //        //}
                            //        //if (Statics.focusRecipe < 0)
                            //        //{
                            //        //    Main.focusRecipe = 0;
                            //        //}
                            //    }
                            //}
                        }

                        if (Statics.playerInventory)
                        {
                            this.AdjTiles(world);
                        }
                        if (this.chest != -1)
                        {
                            int num12 = (int)(((double)this.position.X + (double)this.width * 0.5) / 16.0);
                            int num13 = (int)(((double)this.position.Y + (double)this.height * 0.5) / 16.0);
                            if (num12 < this.chestX - 5 || num12 > this.chestX + 6 || num13 < this.chestY - 4 || num13 > this.chestY + 5)
                            {
                                if (this.chest != -1)
                                {
                                    //Main.PlaySound(11, -1, -1, 1);
                                }
                                this.chest = -1;
                            }
                            if (!world.getTile()[this.chestX, this.chestY].active)
                            {
                                //Main.PlaySound(11, -1, -1, 1);
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
                        if (this.rocketDelay > 0 || this.wet)
                        {
                            this.fallStart = (int)(this.position.Y / 16f);
                        }
                    }
                    if (this.mouseInterface)
                    {
                        this.delayUseItem = true;
                    }
                    //tileTargetX = (int)(((float)Main.mouseState.X + Main.screenPosition.X) / 16f);
                    //tileTargetY = (int)(((float)Main.mouseState.Y + Main.screenPosition.Y) / 16f);
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
                        //Lighting.addLight(i2, j2, 0.8f);
                    }
                    if (this.jumpBoost)
                    {
                        jumpHeight = 20;
                        jumpSpeed = 6.51f;
                    }
                    this.setBonus = "";
                    if ((this.head == 1 && this.body == 1 && this.legs == 1) || (this.head == 2 && this.body == 2 && this.legs == 2))
                    {
                        this.setBonus = "2 defense";
                        this.statDefense++;
                    }
                    if ((this.head == 3 && this.body == 3 && this.legs == 3) || (this.head == 4 && this.body == 4 && this.legs == 4))
                    {
                        this.setBonus = "3 defense";
                        this.statDefense += 2;
                    }
                    if (this.head == 5 && this.body == 5 && this.legs == 5)
                    {
                        this.setBonus = "15 % increased melee speed";
                        this.meleeSpeed *= 0.85f;
                        if (Statics.rand.Next(10) == 0)
                        {
                            Vector2 arg_1429_0 = new Vector2(this.position.X, this.position.Y);
                            int arg_1429_1 = this.width;
                            int arg_1429_2 = this.height;
                            int arg_1429_3 = 14;
                            float arg_1429_4 = 0f;
                            float arg_1429_5 = 0f;
                            int arg_1429_6 = 200;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_1429_0, world, arg_1429_1, arg_1429_2, arg_1429_3, arg_1429_4, arg_1429_5, arg_1429_6, newColor, 1.2f);
                        }
                    }
                    if (this.head == 6 && this.body == 6 && this.legs == 6)
                    {
                        this.setBonus = "20% reduced mana usage";
                        this.manaCost *= 0.8f;
                        if (Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y) > 1f && !this.rocketFrame)
                        {
                            for (int n = 0; n < 2; n++)
                            {
                                Vector2 arg_151B_0 = new Vector2(this.position.X - this.velocity.X * 2f, this.position.Y - 2f - this.velocity.Y * 2f);
                                int arg_151B_1 = this.width;
                                int arg_151B_2 = this.height;
                                int arg_151B_3 = 6;
                                float arg_151B_4 = 0f;
                                float arg_151B_5 = 0f;
                                int arg_151B_6 = 100;
                                Color newColor = default(Color);
                                int num15 = Dust.NewDust(arg_151B_0, world, arg_151B_1, arg_151B_2, arg_151B_3, arg_151B_4, arg_151B_5, arg_151B_6, newColor, 2f);
                                world.getDust()[num15].noGravity = true;
                                Dust expr_153D_cp_0 = world.getDust()[num15];
                                expr_153D_cp_0.velocity.X = expr_153D_cp_0.velocity.X - this.velocity.X * 0.5f;
                                Dust expr_1567_cp_0 = world.getDust()[num15];
                                expr_1567_cp_0.velocity.Y = expr_1567_cp_0.velocity.Y - this.velocity.Y * 0.5f;
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
                        if (Statics.rand.Next(10) == 0)
                        {
                            Vector2 arg_1677_0 = new Vector2(this.position.X, this.position.Y);
                            int arg_1677_1 = this.width;
                            int arg_1677_2 = this.height;
                            int arg_1677_3 = 14;
                            float arg_1677_4 = 0f;
                            float arg_1677_5 = 0f;
                            int arg_1677_6 = 200;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_1677_0, world, arg_1677_1, arg_1677_2, arg_1677_3, arg_1677_4, arg_1677_5, arg_1677_6, newColor, 1.2f);
                        }
                    }
                    if (this.head == 9 && this.body == 9 && this.legs == 9)
                    {
                        this.setBonus = "10 defense";
                        if (Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y) > 1f && !this.rocketFrame)
                        {
                            for (int num16 = 0; num16 < 2; num16++)
                            {
                                Vector2 arg_175A_0 = new Vector2(this.position.X - this.velocity.X * 2f, this.position.Y - 2f - this.velocity.Y * 2f);
                                int arg_175A_1 = this.width;
                                int arg_175A_2 = this.height;
                                int arg_175A_3 = 6;
                                float arg_175A_4 = 0f;
                                float arg_175A_5 = 0f;
                                int arg_175A_6 = 100;
                                Color newColor = default(Color);
                                int num17 = Dust.NewDust(arg_175A_0, world, arg_175A_1, arg_175A_2, arg_175A_3, arg_175A_4, arg_175A_5, arg_175A_6, newColor, 2f);
                                world.getDust()[num17].noGravity = true;
                                Dust expr_177C_cp_0 = world.getDust()[num17];
                                expr_177C_cp_0.velocity.X = expr_177C_cp_0.velocity.X - this.velocity.X * 0.5f;
                                Dust expr_17A6_cp_0 = world.getDust()[num17];
                                expr_17A6_cp_0.velocity.Y = expr_17A6_cp_0.velocity.Y - this.velocity.Y * 0.5f;
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
                                        Vector2 arg_1A7F_0 = new Vector2(this.position.X - 4f, this.position.Y + (float)this.height);
                                        int arg_1A7F_1 = this.width + 8;
                                        int arg_1A7F_2 = 4;
                                        int arg_1A7F_3 = 16;
                                        float arg_1A7F_4 = -this.velocity.X * 0.5f;
                                        float arg_1A7F_5 = this.velocity.Y * 0.5f;
                                        int arg_1A7F_6 = 50;
                                        Color newColor = default(Color);
                                        int num18 = Dust.NewDust(arg_1A7F_0, world, arg_1A7F_1, arg_1A7F_2, arg_1A7F_3, arg_1A7F_4, arg_1A7F_5, arg_1A7F_6, newColor, 1.5f);
                                        world.getDust()[num18].velocity.X = world.getDust()[num18].velocity.X * 0.2f;
                                        world.getDust()[num18].velocity.Y = world.getDust()[num18].velocity.Y * 0.2f;
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
                                            Vector2 arg_1C47_0 = new Vector2(this.position.X - 4f, this.position.Y + (float)this.height);
                                            int arg_1C47_1 = this.width + 8;
                                            int arg_1C47_2 = 4;
                                            int arg_1C47_3 = 16;
                                            float arg_1C47_4 = -this.velocity.X * 0.5f;
                                            float arg_1C47_5 = this.velocity.Y * 0.5f;
                                            int arg_1C47_6 = 50;
                                            Color newColor = default(Color);
                                            int num19 = Dust.NewDust(arg_1C47_0, world, arg_1C47_1, arg_1C47_2, arg_1C47_3, arg_1C47_4, arg_1C47_5, arg_1C47_6, newColor, 1.5f);
                                            world.getDust()[num19].velocity.X = world.getDust()[num19].velocity.X * 0.2f;
                                            world.getDust()[num19].velocity.Y = world.getDust()[num19].velocity.Y * 0.2f;
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
                                if (this.velocity.Y > -jumpSpeed + num2 * 2f)
                                {
                                    this.jump = 0;
                                }
                                else
                                {
                                    this.velocity.Y = -jumpSpeed;
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
                                        this.velocity.Y = -jumpSpeed;
                                        this.jump = jumpHeight;
                                    }
                                    else
                                    {
                                        //Main.PlaySound(16, (int)this.position.X, (int)this.position.Y, 1);
                                        this.velocity.Y = -jumpSpeed;
                                        this.jump = jumpHeight / 2;
                                        for (int num20 = 0; num20 < 10; num20++)
                                        {
                                            Vector2 arg_1F77_0 = new Vector2(this.position.X - 34f, this.position.Y + (float)this.height - 16f);
                                            int arg_1F77_1 = 102;
                                            int arg_1F77_2 = 32;
                                            int arg_1F77_3 = 16;
                                            float arg_1F77_4 = -this.velocity.X * 0.5f;
                                            float arg_1F77_5 = this.velocity.Y * 0.5f;
                                            int arg_1F77_6 = 100;
                                            Color newColor = default(Color);
                                            int num21 = Dust.NewDust(arg_1F77_0, world, arg_1F77_1, arg_1F77_2, arg_1F77_3, arg_1F77_4, arg_1F77_5, arg_1F77_6, newColor, 1.5f);
                                            world.getDust()[num21].velocity.X = world.getDust()[num21].velocity.X * 0.5f - this.velocity.X * 0.1f;
                                            world.getDust()[num21].velocity.Y = world.getDust()[num21].velocity.Y * 0.5f - this.velocity.Y * 0.3f;
                                        }
                                        int num22 = Gore.NewGore(new Vector2(this.position.X + (float)(this.width / 2) - 16f, this.position.Y + (float)this.height - 16f), new Vector2(-this.velocity.X, -this.velocity.Y), Statics.rand.Next(11, 14), world);
                                        world.getGore()[num22].velocity.X = world.getGore()[num22].velocity.X * 0.1f - this.velocity.X * 0.1f;
                                        world.getGore()[num22].velocity.Y = world.getGore()[num22].velocity.Y * 0.1f - this.velocity.Y * 0.05f;
                                        num22 = Gore.NewGore(new Vector2(this.position.X - 36f, this.position.Y + (float)this.height - 16f), new Vector2(-this.velocity.X, -this.velocity.Y), Statics.rand.Next(11, 14), world);
                                        world.getGore()[num22].velocity.X = world.getGore()[num22].velocity.X * 0.1f - this.velocity.X * 0.1f;
                                        world.getGore()[num22].velocity.Y = world.getGore()[num22].velocity.Y * 0.1f - this.velocity.Y * 0.05f;
                                        num22 = Gore.NewGore(new Vector2(this.position.X + (float)this.width + 4f, this.position.Y + (float)this.height - 16f), new Vector2(-this.velocity.X, -this.velocity.Y), Statics.rand.Next(11, 14), world);
                                        world.getGore()[num22].velocity.X = world.getGore()[num22].velocity.X * 0.1f - this.velocity.X * 0.1f;
                                        world.getGore()[num22].velocity.Y = world.getGore()[num22].velocity.Y * 0.1f - this.velocity.Y * 0.05f;
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
                            Vector2 arg_2369_0 = new Vector2(this.position.X - 4f, this.position.Y + (float)this.height);
                            int arg_2369_1 = this.width + 8;
                            int arg_2369_2 = 4;
                            int arg_2369_3 = 16;
                            float arg_2369_4 = -this.velocity.X * 0.5f;
                            float arg_2369_5 = this.velocity.Y * 0.5f;
                            int arg_2369_6 = 100;
                            Color newColor = default(Color);
                            int num23 = Dust.NewDust(arg_2369_0, world, arg_2369_1, arg_2369_2, arg_2369_3, arg_2369_4, arg_2369_5, arg_2369_6, newColor, 1.5f);
                            world.getDust()[num23].velocity.X = world.getDust()[num23].velocity.X * 0.5f - this.velocity.X * 0.1f;
                            world.getDust()[num23].velocity.Y = world.getDust()[num23].velocity.Y * 0.5f - this.velocity.Y * 0.3f;
                        }
                        if (this.velocity.Y > -jumpSpeed && this.velocity.Y != 0f)
                        {
                            this.canRocket = true;
                        }
                        if (this.rocketBoots && this.controlJump && this.rocketDelay == 0 && this.canRocket && this.rocketRelease && !this.jumpAgain)
                        {
                            int num24 = 7;
                            if (this.statMana >= (int)((float)num24 * this.manaCost))
                            {
                                this.manaRegenDelay = 180;
                                this.statMana -= (int)((float)num24 * this.manaCost);
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
                            for (int num25 = 0; num25 < 2; num25++)
                            {
                                if (num25 == 0)
                                {
                                    Vector2 arg_2563_0 = new Vector2(this.position.X - 4f, this.position.Y + (float)this.height - 10f);
                                    int arg_2563_1 = 8;
                                    int arg_2563_2 = 8;
                                    int arg_2563_3 = 6;
                                    float arg_2563_4 = 0f;
                                    float arg_2563_5 = 0f;
                                    int arg_2563_6 = 100;
                                    Color newColor = default(Color);
                                    int num26 = Dust.NewDust(arg_2563_0, world, arg_2563_1, arg_2563_2, arg_2563_3, arg_2563_4, arg_2563_5, arg_2563_6, newColor, 2.5f);
                                    world.getDust()[num26].noGravity = true;
                                    world.getDust()[num26].velocity.X = world.getDust()[num26].velocity.X * 1f - 2f - this.velocity.X * 0.3f;
                                    world.getDust()[num26].velocity.Y = world.getDust()[num26].velocity.Y * 1f + 2f - this.velocity.Y * 0.3f;
                                }
                                else
                                {
                                    Vector2 arg_2656_0 = new Vector2(this.position.X + (float)this.width - 4f, this.position.Y + (float)this.height - 10f);
                                    int arg_2656_1 = 8;
                                    int arg_2656_2 = 8;
                                    int arg_2656_3 = 6;
                                    float arg_2656_4 = 0f;
                                    float arg_2656_5 = 0f;
                                    int arg_2656_6 = 100;
                                    Color newColor = default(Color);
                                    int num27 = Dust.NewDust(arg_2656_0, world, arg_2656_1, arg_2656_2, arg_2656_3, arg_2656_4, arg_2656_5, arg_2656_6, newColor, 2.5f);
                                    world.getDust()[num27].noGravity = true;
                                    world.getDust()[num27].velocity.X = world.getDust()[num27].velocity.X * 1f + 2f - this.velocity.X * 0.3f;
                                    world.getDust()[num27].velocity.Y = world.getDust()[num27].velocity.Y * 1f + 2f - this.velocity.Y * 0.3f;
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
                            if (this.velocity.Y < -jumpSpeed)
                            {
                                this.velocity.Y = -jumpSpeed;
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
                    for (int num28 = 0; num28 < 200; num28++)
                    {
                        if (world.getItemList()[num28].active && world.getItemList()[num28].noGrabDelay == 0 && world.getItemList()[num28].owner == i)
                        {
                            Rectangle rectangle = new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height);
                            if (rectangle.Intersects(new Rectangle((int)world.getItemList()[num28].position.X, (int)world.getItemList()[num28].position.Y, world.getItemList()[num28].width, world.getItemList()[num28].height)))
                            {
                                if (i == Statics.myPlayer && (this.inventory[this.selectedItem].type != 0 || this.itemAnimation <= 0))
                                {
                                    if (world.getItemList()[num28].type == 58)
                                    {
                                        //Main.PlaySound(7, (int)this.position.X, (int)this.position.Y, 1);
                                        this.statLife += 20;
                                        if (Statics.myPlayer == this.whoAmi)
                                        {
                                            this.HealEffect(20);
                                        }
                                        if (this.statLife > this.statLifeMax)
                                        {
                                            this.statLife = this.statLifeMax;
                                        }
                                        world.getItemList()[num28] = new Item();
                                        if (Statics.netMode == 1)
                                        {
                                            NetMessage.SendData(21, world, -1, -1, "", num28, 0f, 0f, 0f);
                                        }
                                    }
                                    else
                                    {
                                        if (world.getItemList()[num28].type == 184)
                                        {
                                            //Main.PlaySound(7, (int)this.position.X, (int)this.position.Y, 1);
                                            this.statMana += 20;
                                            if (Statics.myPlayer == this.whoAmi)
                                            {
                                                this.ManaEffect(20);
                                            }
                                            if (this.statMana > this.statManaMax)
                                            {
                                                this.statMana = this.statManaMax;
                                            }
                                            world.getItemList()[num28] = new Item();
                                            if (Statics.netMode == 1)
                                            {
                                                NetMessage.SendData(21, world, -1, -1, "", num28, 0f, 0f, 0f);
                                            }
                                        }
                                        else
                                        {
                                            world.getItemList()[num28] = this.GetItem(i, world.getItemList()[num28]);
                                            if (Statics.netMode == 1)
                                            {
                                                NetMessage.SendData(21, world, -1, -1, "", num28, 0f, 0f, 0f);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                rectangle = new Rectangle((int)this.position.X - itemGrabRange, (int)this.position.Y - itemGrabRange, this.width + itemGrabRange * 2, this.height + itemGrabRange * 2);
                                if (rectangle.Intersects(new Rectangle((int)world.getItemList()[num28].position.X, (int)world.getItemList()[num28].position.Y, world.getItemList()[num28].width, world.getItemList()[num28].height)) && this.ItemSpace(world.getItemList()[num28]))
                                {
                                    world.getItemList()[num28].beingGrabbed = true;
                                    if ((double)this.position.X + (double)this.width * 0.5 > (double)world.getItemList()[num28].position.X + (double)world.getItemList()[num28].width * 0.5)
                                    {
                                        if (world.getItemList()[num28].velocity.X < itemGrabSpeedMax + this.velocity.X)
                                        {
                                            Item expr_2B70_cp_0 = world.getItemList()[num28];
                                            expr_2B70_cp_0.velocity.X = expr_2B70_cp_0.velocity.X + itemGrabSpeed;
                                        }
                                        if (world.getItemList()[num28].velocity.X < 0f)
                                        {
                                            Item expr_2BAA_cp_0 = world.getItemList()[num28];
                                            expr_2BAA_cp_0.velocity.X = expr_2BAA_cp_0.velocity.X + itemGrabSpeed * 0.75f;
                                        }
                                    }
                                    else
                                    {
                                        if (world.getItemList()[num28].velocity.X > -itemGrabSpeedMax + this.velocity.X)
                                        {
                                            Item expr_2BF9_cp_0 = world.getItemList()[num28];
                                            expr_2BF9_cp_0.velocity.X = expr_2BF9_cp_0.velocity.X - itemGrabSpeed;
                                        }
                                        if (world.getItemList()[num28].velocity.X > 0f)
                                        {
                                            Item expr_2C30_cp_0 = world.getItemList()[num28];
                                            expr_2C30_cp_0.velocity.X = expr_2C30_cp_0.velocity.X - itemGrabSpeed * 0.75f;
                                        }
                                    }
                                    if ((double)this.position.Y + (double)this.height * 0.5 > (double)world.getItemList()[num28].position.Y + (double)world.getItemList()[num28].height * 0.5)
                                    {
                                        if (world.getItemList()[num28].velocity.Y < itemGrabSpeedMax)
                                        {
                                            Item expr_2CB9_cp_0 = world.getItemList()[num28];
                                            expr_2CB9_cp_0.velocity.Y = expr_2CB9_cp_0.velocity.Y + itemGrabSpeed;
                                        }
                                        if (world.getItemList()[num28].velocity.Y < 0f)
                                        {
                                            Item expr_2CF3_cp_0 = world.getItemList()[num28];
                                            expr_2CF3_cp_0.velocity.Y = expr_2CF3_cp_0.velocity.Y + itemGrabSpeed * 0.75f;
                                        }
                                    }
                                    else
                                    {
                                        if (world.getItemList()[num28].velocity.Y > -itemGrabSpeedMax)
                                        {
                                            Item expr_2D33_cp_0 = world.getItemList()[num28];
                                            expr_2D33_cp_0.velocity.Y = expr_2D33_cp_0.velocity.Y - itemGrabSpeed;
                                        }
                                        if (world.getItemList()[num28].velocity.Y > 0f)
                                        {
                                            Item expr_2D6A_cp_0 = world.getItemList()[num28];
                                            expr_2D6A_cp_0.velocity.Y = expr_2D6A_cp_0.velocity.Y - itemGrabSpeed * 0.75f;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (this.position.X / 16f - (float)tileRangeX <= (float)tileTargetX && (this.position.X + (float)this.width) / 16f + (float)tileRangeX - 1f >= (float)tileTargetX && this.position.Y / 16f - (float)tileRangeY <= (float)tileTargetY && (this.position.Y + (float)this.height) / 16f + (float)tileRangeY - 2f >= (float)tileTargetY && world.getTile()[tileTargetX, tileTargetY].active)
                    {
                        if (world.getTile()[tileTargetX, tileTargetY].type == 79)
                        {
                            this.showItemIcon = true;
                            this.showItemIcon2 = 224;
                        }
                        if (world.getTile()[tileTargetX, tileTargetY].type == 21)
                        {
                            this.showItemIcon = true;
                            this.showItemIcon2 = 48;
                        }
                        if (world.getTile()[tileTargetX, tileTargetY].type == 4)
                        {
                            this.showItemIcon = true;
                            this.showItemIcon2 = 8;
                        }
                        if (world.getTile()[tileTargetX, tileTargetY].type == 13)
                        {
                            this.showItemIcon = true;
                            if (world.getTile()[tileTargetX, tileTargetY].frameX == 18)
                            {
                                this.showItemIcon2 = 28;
                            }
                            else
                            {
                                if (world.getTile()[tileTargetX, tileTargetY].frameX == 36)
                                {
                                    this.showItemIcon2 = 110;
                                }
                                else
                                {
                                    this.showItemIcon2 = 31;
                                }
                            }
                        }
                        if (world.getTile()[tileTargetX, tileTargetY].type == 29)
                        {
                            this.showItemIcon = true;
                            this.showItemIcon2 = 87;
                        }
                        if (world.getTile()[tileTargetX, tileTargetY].type == 33)
                        {
                            this.showItemIcon = true;
                            this.showItemIcon2 = 105;
                        }
                        if (world.getTile()[tileTargetX, tileTargetY].type == 49)
                        {
                            this.showItemIcon = true;
                            this.showItemIcon2 = 148;
                        }
                        if (world.getTile()[tileTargetX, tileTargetY].type == 50 && world.getTile()[tileTargetX, tileTargetY].frameX == 90)
                        {
                            this.showItemIcon = true;
                            this.showItemIcon2 = 165;
                        }
                        if (world.getTile()[tileTargetX, tileTargetY].type == 55)
                        {
                            int num29 = (int)(world.getTile()[tileTargetX, tileTargetY].frameX / 18);
                            int num30 = (int)(world.getTile()[tileTargetX, tileTargetY].frameY / 18);
                            while (num29 > 1)
                            {
                                num29 -= 2;
                            }
                            int num31 = tileTargetX - num29;
                            int num32 = tileTargetY - num30;
                            //main.signBubble = true;
                            // Main.signX = num31 * 16 + 16;
                            //Main.signY = num32 * 16;
                        }
                        if (world.getTile()[tileTargetX, tileTargetY].type == 10 || world.getTile()[tileTargetX, tileTargetY].type == 11)
                        {
                            this.showItemIcon = true;
                            this.showItemIcon2 = 25;
                        }
                        if (this.controlUseTile)
                        {
                            if (this.releaseUseTile)
                            {
                                if (world.getTile()[tileTargetX, tileTargetY].type == 4 || world.getTile()[tileTargetX, tileTargetY].type == 13 || world.getTile()[tileTargetX, tileTargetY].type == 33 || world.getTile()[tileTargetX, tileTargetY].type == 49 || (world.getTile()[tileTargetX, tileTargetY].type == 50 && world.getTile()[tileTargetX, tileTargetY].frameX == 90))
                                {
                                    WorldGen.KillTile(tileTargetX, tileTargetY, world, false, false, false);
                                    if (Statics.netMode == 1)
                                    {
                                        NetMessage.SendData(17, world, -1, -1, "", 0, (float)tileTargetX, (float)tileTargetY, 0f);
                                    }
                                }
                                else
                                {
                                    if (world.getTile()[tileTargetX, tileTargetY].type == 79)
                                    {
                                        int num33 = tileTargetX;
                                        int num34 = tileTargetY;
                                        num33 += (int)(world.getTile()[tileTargetX, tileTargetY].frameX / 18 * -1);
                                        if (world.getTile()[tileTargetX, tileTargetY].frameX >= 72)
                                        {
                                            num33 += 4;
                                            num33++;
                                        }
                                        else
                                        {
                                            num33 += 2;
                                        }
                                        num34 += (int)(world.getTile()[tileTargetX, tileTargetY].frameY / 18 * -1);
                                        num34 += 2;
                                        if (CheckSpawn(num33, num34, world))
                                        {
                                            this.ChangeSpawn(num33, num34, world);
                                            //Main.NewText("Spawn point set!", 255, 240, 20);
                                            Console.WriteLine("Spawn point set!");
                                        }
                                    }
                                    else
                                    {
                                        if (world.getTile()[tileTargetX, tileTargetY].type == 55)
                                        {
                                            bool flag4 = true;
                                            if (this.sign >= 0)
                                            {
                                                int num35 = Sign.ReadSign(tileTargetX, tileTargetY, world);
                                                if (num35 == this.sign)
                                                {
                                                    this.sign = -1;
                                                    //Main.npcChatText = "";
                                                    Statics.editSign = false;
                                                    //Main.PlaySound(11, -1, -1, 1);
                                                    flag4 = false;
                                                }
                                            }
                                            if (flag4)
                                            {
                                                //if (Statics.netMode == 0)
                                                //{
                                                //    this.talkNPC = -1;
                                                //    Main.playerInventory = false;
                                                //    Main.editSign = false;
                                                //    Main.PlaySound(10, -1, -1, 1);
                                                //    int num36 = Sign.ReadSign(tileTargetX, tileTargetY);
                                                //    this.sign = num36;
                                                //    Main.npcChatText = world.getSigns()[num36].text;
                                                //}
                                                //else
                                                //{
                                                int num37 = (int)(world.getTile()[tileTargetX, tileTargetY].frameX / 18);
                                                int num38 = (int)(world.getTile()[tileTargetX, tileTargetY].frameY / 18);
                                                while (num37 > 1)
                                                {
                                                    num37 -= 2;
                                                }
                                                int num39 = tileTargetX - num37;
                                                int num40 = tileTargetY - num38;
                                                if (world.getTile()[num39, num40].type == 55)
                                                {
                                                    NetMessage.SendData(46, world, -1, -1, "", num39, (float)num40, 0f, 0f);
                                                }
                                                //}
                                            }
                                        }
                                        else
                                        {
                                            if (world.getTile()[tileTargetX, tileTargetY].type == 10)
                                            {
                                                WorldGen.OpenDoor(tileTargetX, tileTargetY, this.direction, world);
                                                NetMessage.SendData(19, world, -1, -1, "", 0, (float)tileTargetX, (float)tileTargetY, (float)this.direction);
                                            }
                                            else
                                            {
                                                if (world.getTile()[tileTargetX, tileTargetY].type == 11)
                                                {
                                                    if (WorldGen.CloseDoor(tileTargetX, tileTargetY, world, false))
                                                    {
                                                        NetMessage.SendData(19, world, -1, -1, "", 1, (float)tileTargetX, (float)tileTargetY, (float)this.direction);
                                                    }
                                                }
                                                else
                                                {
                                                    if ((world.getTile()[tileTargetX, tileTargetY].type == 21 || world.getTile()[tileTargetX, tileTargetY].type == 29) && this.talkNPC == -1)
                                                    {
                                                        bool flag5 = false;
                                                        int num41 = tileTargetX - (int)(world.getTile()[tileTargetX, tileTargetY].frameX / 18);
                                                        int num42 = tileTargetY - (int)(world.getTile()[tileTargetX, tileTargetY].frameY / 18);
                                                        if (world.getTile()[tileTargetX, tileTargetY].type == 29)
                                                        {
                                                            flag5 = true;
                                                        }
                                                        if (Statics.netMode == 1 && !flag5)
                                                        {
                                                            if (num41 == this.chestX && num42 == this.chestY && this.chest != -1)
                                                            {
                                                                this.chest = -1;
                                                                //Main.PlaySound(11, -1, -1, 1);
                                                            }
                                                            else
                                                            {
                                                                NetMessage.SendData(31, world, -1, -1, "", num41, (float)num42, 0f, 0f);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            int num43 = -1;
                                                            if (flag5)
                                                            {
                                                                num43 = -2;
                                                            }
                                                            else
                                                            {
                                                                num43 = Chest.FindChest(num41, num42, world);
                                                            }
                                                            if (num43 != -1)
                                                            {
                                                                if (num43 == this.chest)
                                                                {
                                                                    this.chest = -1;
                                                                    //Main.PlaySound(11, -1, -1, 1);
                                                                }
                                                                else
                                                                {
                                                                    if (num43 != this.chest && this.chest == -1)
                                                                    {
                                                                        this.chest = num43;
                                                                        Statics.playerInventory = true;
                                                                        //Main.PlaySound(10, -1, -1, 1);
                                                                        this.chestX = num41;
                                                                        this.chestY = num42;
                                                                    }
                                                                    else
                                                                    {
                                                                        this.chest = num43;
                                                                        Statics.playerInventory = true;
                                                                        //Main.PlaySound(12, -1, -1, 1);
                                                                        this.chestX = num41;
                                                                        this.chestY = num42;
                                                                    }
                                                                }
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
                    if (Statics.myPlayer == this.whoAmi)
                    {
                        if (this.talkNPC >= 0)
                        {
                            Rectangle rectangle2 = new Rectangle((int)(this.position.X + (float)(this.width / 2) - (float)(tileRangeX * 16)), (int)(this.position.Y + (float)(this.height / 2) - (float)(tileRangeY * 16)), tileRangeX * 16 * 2, tileRangeY * 16 * 2);
                            Rectangle value = new Rectangle((int)world.getNPCs()[this.talkNPC].position.X, (int)world.getNPCs()[this.talkNPC].position.Y, world.getNPCs()[this.talkNPC].width, world.getNPCs()[this.talkNPC].height);
                            if (!rectangle2.Intersects(value) || this.chest != -1 || !world.getNPCs()[this.talkNPC].active)
                            {
                                if (this.chest == -1)
                                {
                                    //Main.PlaySound(11, -1, -1, 1);
                                }
                                this.talkNPC = -1;
                                //Main.npcChatText = "";
                            }
                        }
                        if (this.sign >= 0)
                        {
                            Rectangle rectangle3 = new Rectangle((int)(this.position.X + (float)(this.width / 2) - (float)(tileRangeX * 16)), (int)(this.position.Y + (float)(this.height / 2) - (float)(tileRangeY * 16)), tileRangeX * 16 * 2, tileRangeY * 16 * 2);
                            Rectangle value2 = new Rectangle(world.getSigns()[this.sign].x * 16, world.getSigns()[this.sign].y * 16, 32, 32);
                            if (!rectangle3.Intersects(value2))
                            {
                                //Main.PlaySound(11, -1, -1, 1);
                                this.sign = -1;
                                Statics.editSign = false;
                                //StaticsStatics.npcChatText = "";
                            }
                        }
                        if (Statics.editSign)
                        {
                            if (this.sign == -1)
                            {
                                Statics.editSign = false;
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
                        for (int num44 = 0; num44 < 1000; num44++)
                        {
                            if (world.getNPCs()[num44].active && !world.getNPCs()[num44].friendly && rectangle4.Intersects(new Rectangle((int)world.getNPCs()[num44].position.X, (int)world.getNPCs()[num44].position.Y, world.getNPCs()[num44].width, world.getNPCs()[num44].height)))
                            {
                                int hitDirection = -1;
                                if (world.getNPCs()[num44].position.X + (float)(world.getNPCs()[num44].width / 2) < this.position.X + (float)(this.width / 2))
                                {
                                    hitDirection = 1;
                                }
                                this.Hurt(world.getNPCs()[num44].damage, hitDirection, false, false);
                            }
                        }
                        Vector2 vector = Collision.HurtTiles(this.position, this.velocity, world, this.width, this.height, this.fireWalk);
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
                        float num45 = 0f;
                        float num46 = 0f;
                        for (int num47 = 0; num47 < this.grapCount; num47++)
                        {
                            num45 += world.getProjectile()[this.grappling[num47]].position.X + (float)(world.getProjectile()[this.grappling[num47]].width / 2);
                            num46 += world.getProjectile()[this.grappling[num47]].position.Y + (float)(world.getProjectile()[this.grappling[num47]].height / 2);
                        }
                        num45 /= (float)this.grapCount;
                        num46 /= (float)this.grapCount;
                        Vector2 vector2 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                        float num48 = num45 - vector2.X;
                        float num49 = num46 - vector2.Y;
                        float num50 = (float)Math.Sqrt((double)(num48 * num48 + num49 * num49));
                        float num51 = 11f;
                        float num52 = num50;
                        if (num50 > num51)
                        {
                            num52 = num51 / num50;
                        }
                        else
                        {
                            num52 = 1f;
                        }
                        num48 *= num52;
                        num49 *= num52;
                        this.velocity.X = num48;
                        this.velocity.Y = num49;
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
                                    this.velocity.Y = -jumpSpeed;
                                    this.jump = jumpHeight / 2;
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
                                for (int num53 = 0; num53 < 1000; num53++)
                                {
                                    if (world.getProjectile()[num53].active && world.getProjectile()[num53].owner == i && world.getProjectile()[num53].aiStyle == 7)
                                    {
                                        world.getProjectile()[num53].Kill(world);
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.releaseJump = true;
                        }
                    }
                    if (Collision.StickyTiles(this.position, this.velocity, world, this.width, this.height))
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
                    bool flag6 = Collision.DrownCollision(this.position, this.width, this.height, world);
                    if (this.inventory[this.selectedItem].type == 186)
                    {
                        try
                        {
                            int num54 = (int)((this.position.X + (float)(this.width / 2) + (float)(6 * this.direction)) / 16f);
                            int num55 = (int)((this.position.Y - 44f) / 16f);
                            if (world.getTile()[num54, num55].liquid < 128)
                            {
                                if (world.getTile()[num54, num55] == null)
                                {
                                    world.getTile()[num54, num55] = new Tile();
                                }
                                if (!world.getTile()[num54, num55].active || !Statics.tileSolid[(int)world.getTile()[num54, num55].type] || Statics.tileSolidTop[(int)world.getTile()[num54, num55].type])
                                {
                                    flag6 = false;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (Statics.myPlayer == i)
                    {
                        if (flag6)
                        {
                            this.breathCD++;
                            int num56 = 7;
                            if (this.inventory[this.selectedItem].type == 186)
                            {
                                num56 *= 2;
                            }
                            if (this.breathCD >= num56)
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
                    if (flag6 && Statics.rand.Next(20) == 0)
                    {
                        if (this.inventory[this.selectedItem].type == 186)
                        {
                            Vector2 arg_4117_0 = new Vector2(this.position.X + (float)(10 * this.direction) + 4f, this.position.Y - 54f);
                            int arg_4117_1 = this.width - 8;
                            int arg_4117_2 = 8;
                            int arg_4117_3 = 34;
                            float arg_4117_4 = 0f;
                            float arg_4117_5 = 0f;
                            int arg_4117_6 = 0;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_4117_0, world, arg_4117_1, arg_4117_2, arg_4117_3, arg_4117_4, arg_4117_5, arg_4117_6, newColor, 1.2f);
                        }
                        else
                        {
                            Vector2 arg_4170_0 = new Vector2(this.position.X + (float)(12 * this.direction), this.position.Y + 4f);
                            int arg_4170_1 = this.width - 8;
                            int arg_4170_2 = 8;
                            int arg_4170_3 = 34;
                            float arg_4170_4 = 0f;
                            float arg_4170_5 = 0f;
                            int arg_4170_6 = 0;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_4170_0, world, arg_4170_1, arg_4170_2, arg_4170_3, arg_4170_4, arg_4170_5, arg_4170_6, newColor, 1.2f);
                        }
                    }
                    bool flag7 = Collision.LavaCollision(this.position, world, this.width, this.height);
                    if (flag7)
                    {
                        if (Statics.myPlayer == i)
                        {
                            this.Hurt(100, 0, false, false);
                        }
                        this.lavaWet = true;
                    }
                    bool flag8 = Collision.WetCollision(this.position, this.width, this.height, world);
                    if (flag8)
                    {
                        if (!this.wet)
                        {
                            if (this.wetCount == 0)
                            {
                                this.wetCount = 10;
                                if (!flag7)
                                {
                                    for (int num57 = 0; num57 < 50; num57++)
                                    {
                                        Vector2 arg_4253_0 = new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f);
                                        int arg_4253_1 = this.width + 12;
                                        int arg_4253_2 = 24;
                                        int arg_4253_3 = 33;
                                        float arg_4253_4 = 0f;
                                        float arg_4253_5 = 0f;
                                        int arg_4253_6 = 0;
                                        Color newColor = default(Color);
                                        int num58 = Dust.NewDust(arg_4253_0, world, arg_4253_1, arg_4253_2, arg_4253_3, arg_4253_4, arg_4253_5, arg_4253_6, newColor, 1f);
                                        Dust expr_4267_cp_0 = world.getDust()[num58];
                                        expr_4267_cp_0.velocity.Y = expr_4267_cp_0.velocity.Y - 4f;
                                        Dust expr_4285_cp_0 = world.getDust()[num58];
                                        expr_4285_cp_0.velocity.X = expr_4285_cp_0.velocity.X * 2.5f;
                                        world.getDust()[num58].scale = 1.3f;
                                        world.getDust()[num58].alpha = 100;
                                        world.getDust()[num58].noGravity = true;
                                    }
                                    //Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 0);
                                }
                                else
                                {
                                    for (int num59 = 0; num59 < 20; num59++)
                                    {
                                        Vector2 arg_4359_0 = new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f);
                                        int arg_4359_1 = this.width + 12;
                                        int arg_4359_2 = 24;
                                        int arg_4359_3 = 35;
                                        float arg_4359_4 = 0f;
                                        float arg_4359_5 = 0f;
                                        int arg_4359_6 = 0;
                                        Color newColor = default(Color);
                                        int num60 = Dust.NewDust(arg_4359_0, world, arg_4359_1, arg_4359_2, arg_4359_3, arg_4359_4, arg_4359_5, arg_4359_6, newColor, 1f);
                                        Dust expr_436D_cp_0 = world.getDust()[num60];
                                        expr_436D_cp_0.velocity.Y = expr_436D_cp_0.velocity.Y - 1.5f;
                                        Dust expr_438B_cp_0 = world.getDust()[num60];
                                        expr_438B_cp_0.velocity.X = expr_438B_cp_0.velocity.X * 2.5f;
                                        world.getDust()[num60].scale = 1.3f;
                                        world.getDust()[num60].alpha = 100;
                                        world.getDust()[num60].noGravity = true;
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
                            if (this.jump > jumpHeight / 5)
                            {
                                this.jump = jumpHeight / 5;
                            }
                            if (this.wetCount == 0)
                            {
                                this.wetCount = 10;
                                if (!this.lavaWet)
                                {
                                    for (int num61 = 0; num61 < 50; num61++)
                                    {
                                        Vector2 arg_44AC_0 = new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2));
                                        int arg_44AC_1 = this.width + 12;
                                        int arg_44AC_2 = 24;
                                        int arg_44AC_3 = 33;
                                        float arg_44AC_4 = 0f;
                                        float arg_44AC_5 = 0f;
                                        int arg_44AC_6 = 0;
                                        Color newColor = default(Color);
                                        int num62 = Dust.NewDust(arg_44AC_0, world, arg_44AC_1, arg_44AC_2, arg_44AC_3, arg_44AC_4, arg_44AC_5, arg_44AC_6, newColor, 1f);
                                        Dust expr_44C0_cp_0 = world.getDust()[num62];
                                        expr_44C0_cp_0.velocity.Y = expr_44C0_cp_0.velocity.Y - 4f;
                                        Dust expr_44DE_cp_0 = world.getDust()[num62];
                                        expr_44DE_cp_0.velocity.X = expr_44DE_cp_0.velocity.X * 2.5f;
                                        world.getDust()[num62].scale = 1.3f;
                                        world.getDust()[num62].alpha = 100;
                                        world.getDust()[num62].noGravity = true;
                                    }
                                    //Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 0);
                                }
                                else
                                {
                                    for (int num63 = 0; num63 < 20; num63++)
                                    {
                                        Vector2 arg_45B2_0 = new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f);
                                        int arg_45B2_1 = this.width + 12;
                                        int arg_45B2_2 = 24;
                                        int arg_45B2_3 = 35;
                                        float arg_45B2_4 = 0f;
                                        float arg_45B2_5 = 0f;
                                        int arg_45B2_6 = 0;
                                        Color newColor = default(Color);
                                        int num64 = Dust.NewDust(arg_45B2_0, world, arg_45B2_1, arg_45B2_2, arg_45B2_3, arg_45B2_4, arg_45B2_5, arg_45B2_6, newColor, 1f);
                                        Dust expr_45C6_cp_0 = world.getDust()[num64];
                                        expr_45C6_cp_0.velocity.Y = expr_45C6_cp_0.velocity.Y - 1.5f;
                                        Dust expr_45E4_cp_0 = world.getDust()[num64];
                                        expr_45E4_cp_0.velocity.X = expr_45E4_cp_0.velocity.X * 2.5f;
                                        world.getDust()[num64].scale = 1.3f;
                                        world.getDust()[num64].alpha = 100;
                                        world.getDust()[num64].noGravity = true;
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
                            this.velocity = Collision.TileCollision(this.position, this.velocity, world, this.width, this.height, this.controlDown, false);
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
                        this.velocity = Collision.TileCollision(this.position, this.velocity, world, this.width, this.height, this.controlDown, false);
                        this.position += this.velocity;
                    }
                    if (this.position.X < world.getLeftWorld() + 336f + 16f)
                    {
                        this.position.X = world.getLeftWorld() + 336f + 16f;
                        this.velocity.X = 0f;
                    }
                    if (this.position.X + (float)this.width > world.getRightWorld() - 336f - 32f)
                    {
                        this.position.X = world.getRightWorld() - 336f - 32f - (float)this.width;
                        this.velocity.X = 0f;
                    }
                    if (this.position.Y < world.getTopWorld() + 336f + 16f)
                    {
                        this.position.Y = world.getTopWorld() + 336f + 16f;
                        this.velocity.Y = 0f;
                    }
                    if (this.position.Y > world.getBottomWorld() - 336f - 32f - (float)this.height)
                    {
                        this.position.Y = world.getBottomWorld() - 336f - 32f - (float)this.height;
                        this.velocity.Y = 0f;
                    }
                    this.ItemCheck(i, world);
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

        public void AdjTiles(World world)
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
                    if (world.getTile()[j, k].active)
                    {
                        this.adjTile[(int)world.getTile()[j, k].type] = true;
                        if (world.getTile()[j, k].type == 77)
                        {
                            this.adjTile[17] = true;
                        }
                    }
                }
            }
            if (Statics.playerInventory)
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
                    Recipe.FindRecipes(world);
                }
            }
        }

        /*public void PlayerFrame()
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
                                num2 += world.getProjectile()[this.grappling[i]].position.X + (float)(world.getProjectile()[this.grappling[i]].width / 2);
                                num3 += world.getProjectile()[this.grappling[i]].position.Y + (float)(world.getProjectile()[this.grappling[i]].height / 2);
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
        }*/
        
        public void Spawn(World world)
        {
            if (this.whoAmi == Statics.myPlayer)
            {
                this.FindSpawn(world);
                if (!CheckSpawn(this.SpawnX, this.SpawnY, world))
                {
                    this.SpawnX = -1;
                    this.SpawnY = -1;
                }
            }
            if (Statics.netMode == 1 && this.whoAmi == Statics.myPlayer)
            {
                NetMessage.SendData(12, world, -1, -1, "", Statics.myPlayer, 0f, 0f, 0f);
                //Main.gameMenu = false;
            }
            this.headPosition = new Vector2();
            this.bodyPosition = new Vector2();
            this.legPosition = new Vector2();
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
                this.position.X = (float)(Statics.spawnTileX * 16 + 8 - this.width / 2);
                this.position.Y = (float)(Statics.spawnTileY * 16 - this.height);
                for (int i = Statics.spawnTileX - 1; i < Statics.spawnTileX + 2; i++)
                {
                    for (int j = Statics.spawnTileY - 3; j < Statics.spawnTileY; j++)
                    {
                        if (Statics.tileSolid[(int)world.getTile()[i, j].type] && !Statics.tileSolidTop[(int)world.getTile()[i, j].type])
                        {
                            if (world.getTile()[i, j].liquid > 0)
                            {
                                world.getTile()[i, j].lava = false;
                                world.getTile()[i, j].liquid = 0;
                                WorldGen.SquareTileFrame(i, j, world, true);
                            }
                            WorldGen.KillTile(i, j, world, false, false, false);
                        }
                    }
                }
            }
            this.wet = Collision.WetCollision(this.position, this.width, this.height, world);
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
            if (this.whoAmi == Statics.myPlayer)
            {
                //Lighting.lightCounter = Lighting.lightSkip + 1;
                Statics.screenPosition.X = this.position.X + (float)(this.width / 2) - (float)(Statics.screenWidth / 2);
                Statics.screenPosition.Y = this.position.Y + (float)(this.height / 2) - (float)(Statics.screenHeight / 2);
            }
        }

        public bool isGodMode()
        {
            //if config has god mode then else false
            //if player has god mode on return bool.
            return false;
        }

        public static double CalculateDamage(int Damage, int Defense)
        {
            double num = (double)Damage - (double)Defense * 0.5;
            if (num < 1.0)
            {
                num = 1.0;
            }
            return num;
        }

        public double Hurt(int Damage, int hitDirection, bool pvp = false, bool quiet = false)
        {
            if (!this.immune && !isGodMode())
            {
                int num = Damage;
                if (pvp)
                {
                    num *= 2;
                }
                double num2 = CalculateDamage(num, this.statDefense);
                if (num2 >= 1.0)
                {
                    if (Statics.netMode == 1 && this.whoAmi == Statics.myPlayer && !quiet)
                    {
                        int num3 = 0;
                        if (pvp)
                        {
                            num3 = 1;
                        }
                        NetMessage.SendData(13, world, -1, -1, "", this.whoAmi, 0f, 0f, 0f);
                        NetMessage.SendData(16, world, -1, -1, "", this.whoAmi, 0f, 0f, 0f);
                        NetMessage.SendData(26, world, -1, -1, "", this.whoAmi, (float)hitDirection, (float)Damage, (float)num3);
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
                                Dust.NewDust(this.position, world, this.width, this.height, 26, (float)(2 * hitDirection), -2f, 0, default(Color), 1f);
                            }
                            else
                            {
                                Dust.NewDust(this.position, world, this.width, this.height, 5, (float)(2 * hitDirection), -2f, 0, default(Color), 1f);
                            }
                            num4++;
                        }
                    }
                    else
                    {
                        this.statLife = 0;
                        if (this.whoAmi == Statics.myPlayer)
                        {
                            this.KillMe(num2, hitDirection, pvp);
                        }
                    }
                }
                if (pvp)
                {
                    num2 = CalculateDamage(num, this.statDefense);
                }
                return num2;
            }
            return 0.0;
        }

        public void KillMe(double dmg, int hitDirection, bool pvp = false)
        {
            if (isGodMode() && Statics.myPlayer == this.whoAmi)
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
            this.headVelocity.Y = (float)Statics.rand.Next(-40, -10) * 0.1f;
            this.bodyVelocity.Y = (float)Statics.rand.Next(-40, -10) * 0.1f;
            this.legVelocity.Y = (float)Statics.rand.Next(-40, -10) * 0.1f;
            this.headVelocity.X = (float)Statics.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
            this.bodyVelocity.X = (float)Statics.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
            this.legVelocity.X = (float)Statics.rand.Next(-20, 21) * 0.1f + (float)(2 * hitDirection);
            int num = 0;
            while ((double)num < 20.0 + dmg / (double)this.statLifeMax * 100.0)
            {
                if (this.boneArmor)
                {
                    Dust.NewDust(this.position, world, this.width, this.height, 26, (float)(2 * hitDirection), -2f, 0, default(Color), 1f);
                }
                else
                {
                    Dust.NewDust(this.position, world, this.width, this.height, 5, (float)(2 * hitDirection), -2f, 0, default(Color), 1f);
                }
                num++;
            }
            this.dead = true;
            this.respawnTimer = 600;
            this.immuneAlpha = 0;
            if (Statics.netMode == 2)
            {
                NetMessage.SendData(25, world, -1, -1, this.name + " was slain...", 255, 225f, 25f, 25f);
            }
            if (this.whoAmi == Statics.myPlayer)
            {
                WorldGen.saveToonWhilePlaying(world);
            }
            if (Statics.netMode == 1 && this.whoAmi == Statics.myPlayer)
            {
                int num2 = 0;
                if (pvp)
                {
                    num2 = 1;
                }
                NetMessage.SendData(44, world, -1, -1, "", this.whoAmi, (float)hitDirection, (float)((int)dmg), (float)num2);
            }
            if (!pvp && this.whoAmi == Statics.myPlayer)
            {
                this.DropItems(world);
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
                        if (plr == Statics.myPlayer)
                        {
                            Recipe.FindRecipes(world);
                        }
                        return new Item();
                    }
                    newItem.stack -= this.inventory[num2].maxStack - this.inventory[num2].stack;
                    this.inventory[num2].stack = this.inventory[num2].maxStack;
                    this.DoCoins(num2);
                    if (plr == Statics.myPlayer)
                    {
                        Recipe.FindRecipes(world);
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
                    if (plr == Statics.myPlayer)
                    {
                        Recipe.FindRecipes(world);
                    }
                    return new Item();
                }
            }
            return newItem;
        }

        public void ItemCheck(int i, World world)
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
                        if (world.getProjectile()[j].active && world.getProjectile()[j].owner == Statics.myPlayer && world.getProjectile()[j].type == this.inventory[this.selectedItem].shoot)
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
                if (this.inventory[this.selectedItem].mana > 0)
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
                if (this.inventory[this.selectedItem].type == 43 && world.isDayTime())
                {
                    flag = false;
                }
                if (this.inventory[this.selectedItem].type == 70 && !this.zoneEvil)
                {
                    flag = false;
                }
                if (this.inventory[this.selectedItem].shoot == 17 && flag && i == Statics.myPlayer)
                {
                    //int num = (int)((float)Main.mouseState.X + Main.screenPosition.X) / 16;
                    //int num2 = (int)((float)Main.mouseState.Y + Main.screenPosition.Y) / 16;
                    //if (world.getTile()[num, num2].active && (world.getTile()[num, num2].type == 0 || world.getTile()[num, num2].type == 2 || world.getTile()[num, num2].type == 23))
                    //{
                    //    WorldGen.KillTile(num, num2, world, false, false, true);
                    //    if (!world.getTile()[num, num2].active)
                    //    {
                    //       if (Statics.netMode == 1)
                    //        {
                    //            NetMessage.SendData(17, world, -1, -1, "", 4, (float)num, (float)num2, 0f);
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
                        if (world.getProjectile()[l].active && world.getProjectile()[l].owner == i && world.getProjectile()[l].type == this.inventory[this.selectedItem].shoot)
                        {
                            world.getProjectile()[l].Kill(world);
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
                if (this.inventory[this.selectedItem].mana > 0)
                {
                    this.manaRegenDelay = 180;
                }
                //this.itemHeight = Statics.itemTexture[this.inventory[this.selectedItem].type].Height;
                //this.itemWidth = Main.itemTexture[this.inventory[this.selectedItem].type].Width;
                this.itemAnimation--;
                if (this.inventory[this.selectedItem].useStyle == 1)
                {
                    //if ((double)this.itemAnimation < (double)this.itemAnimationMax * 0.333)
                    //{
                    //    float num3 = 10f;
                    //    //if (Main.itemTexture[this.inventory[this.selectedItem].type].Width > 32)
                    //    //{
                    //    //    num3 = 14f;
                    //    //}
                    //    //this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - num3) * (float)this.direction;
                    //    this.itemLocation.Y = this.position.Y + 24f;
                    //}
                    //else
                    //{
                    //    if ((double)this.itemAnimation < (double)this.itemAnimationMax * 0.666)
                    //    {
                    //        float num4 = 10f;
                    //        //if (Main.itemTexture[this.inventory[this.selectedItem].type].Width > 32)
                    //        //{
                    //        //    num4 = 18f;
                    //        //}
                    //        this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - num4) * (float)this.direction;
                    //        num4 = 10f;
                    //        //if (Main.itemTexture[this.inventory[this.selectedItem].type].Height > 32)
                    //        //{
                    //        //    num4 = 8f;
                    //        //}
                    //        this.itemLocation.Y = this.position.Y + num4;
                    //    }
                    //    else
                    //    {
                    //        float num5 = 6f;
                    //        //if (Main.itemTexture[this.inventory[this.selectedItem].type].Width > 32)
                    //        //{
                    //        //    num5 = 14f;
                    //        //}
                    //        this.itemLocation.X = this.position.X + (float)this.width * 0.5f - ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - num5) * (float)this.direction;
                    //        num5 = 10f;
                    //        //if (Main.itemTexture[this.inventory[this.selectedItem].type].Height > 32)
                    //        //{
                    //        //    num5 = 10f;
                    //        //}
                    //        this.itemLocation.Y = this.position.Y + num5;
                    //    }
                    //}
                    this.itemRotation = ((float)this.itemAnimation / (float)this.itemAnimationMax - 0.5f) * (float)(-(float)this.direction) * 3.5f - (float)this.direction * 0.3f;
                }
                else
                {
                    if (this.inventory[this.selectedItem].useStyle == 2)
                    {
                        this.itemRotation = (float)this.itemAnimation / (float)this.itemAnimationMax * (float)this.direction * 2f + -1.4f * (float)this.direction;
                        //if ((double)this.itemAnimation < (double)this.itemAnimationMax * 0.5)
                        //{
                        //    this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - 9f - this.itemRotation * 12f * (float)this.direction) * (float)this.direction;
                        //    this.itemLocation.Y = this.position.Y + 38f + this.itemRotation * (float)this.direction * 4f;
                        //}
                        //else
                        //{
                        //    this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - 9f - this.itemRotation * 16f * (float)this.direction) * (float)this.direction;
                        //    this.itemLocation.Y = this.position.Y + 38f + this.itemRotation * (float)this.direction;
                        //}
                    }
                    else
                    {
                        if (this.inventory[this.selectedItem].useStyle == 3)
                        {
                            if ((double)this.itemAnimation > (double)this.itemAnimationMax * 0.666)
                            {
                                this.itemLocation.X = -1000f;
                                this.itemLocation.Y = -1000f;
                                this.itemRotation = -1.3f * (float)this.direction;
                            }
                            else
                            {
                                //this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - 4f) * (float)this.direction;
                                //this.itemLocation.Y = this.position.Y + 24f;
                                //float num6 = (float)this.itemAnimation / (float)this.itemAnimationMax * (float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * (float)this.direction * this.inventory[this.selectedItem].scale * 1.2f - (float)(10 * this.direction);
                                //if (num6 > -4f && this.direction == -1)
                                //{
                                //    num6 = -8f;
                                //}
                                //if (num6 < 4f && this.direction == 1)
                                //{
                                //    num6 = 8f;
                                //}
                                //this.itemLocation.X = this.itemLocation.X - num6;
                                //this.itemRotation = 0.8f * (float)this.direction;
                            }
                        }
                        else
                        {
                            //if (this.inventory[this.selectedItem].useStyle == 4)
                            //{
                            //    this.itemRotation = 0f;
                            //    this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - 9f - this.itemRotation * 14f * (float)this.direction) * (float)this.direction;
                            //    this.itemLocation.Y = this.position.Y + (float)Main.itemTexture[this.inventory[this.selectedItem].type].Height * 0.5f;
                            //}
                            //else
                            //{
                            //    if (this.inventory[this.selectedItem].useStyle == 5)
                            //    {
                            //        this.itemLocation.X = this.position.X + (float)this.width * 0.5f - (float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f - (float)(this.direction * 2);
                            //        this.itemLocation.Y = this.position.Y + (float)this.height * 0.5f - (float)Main.itemTexture[this.inventory[this.selectedItem].type].Height * 0.5f;
                            //    }
                            //}
                        }
                    }
                }
            }
            else
            {
                //if (this.inventory[this.selectedItem].holdStyle == 1)
                //{
                //    this.itemLocation.X = this.position.X + (float)this.width * 0.5f + ((float)Main.itemTexture[this.inventory[this.selectedItem].type].Width * 0.5f + 4f) * (float)this.direction;
                //    this.itemLocation.Y = this.position.Y + 24f;
                //    this.itemRotation = 0f;
                //}
                //else
                //{
                    if (this.inventory[this.selectedItem].holdStyle == 2)
                    {
                        this.itemLocation.X = this.position.X + (float)this.width * 0.5f + (float)(6 * this.direction);
                        this.itemLocation.Y = this.position.Y + 16f;
                        this.itemRotation = 0.79f * (float)(-(float)this.direction);
                    }
                //}
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
                    if (Statics.rand.Next(maxValue) == 0)
                    {
                        Vector2 arg_E6B_0 = new Vector2(this.itemLocation.X - 16f, this.itemLocation.Y - 14f);
                        int arg_E6B_1 = 4;
                        int arg_E6B_2 = 4;
                        int arg_E6B_3 = 6;
                        float arg_E6B_4 = 0f;
                        float arg_E6B_5 = 0f;
                        int arg_E6B_6 = 100;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_E6B_0, world, arg_E6B_1, arg_E6B_2, arg_E6B_3, arg_E6B_4, arg_E6B_5, arg_E6B_6, newColor, 1f);
                    }
                    //Lighting.addLight((int)((this.itemLocation.X - 16f + this.velocity.X) / 16f), (int)((this.itemLocation.Y - 14f) / 16f), 1f);
                }
                else
                {
                    if (Statics.rand.Next(maxValue) == 0)
                    {
                        Vector2 arg_F0F_0 = new Vector2(this.itemLocation.X + 6f, this.itemLocation.Y - 14f);
                        int arg_F0F_1 = 4;
                        int arg_F0F_2 = 4;
                        int arg_F0F_3 = 6;
                        float arg_F0F_4 = 0f;
                        float arg_F0F_5 = 0f;
                        int arg_F0F_6 = 100;
                        Color newColor = default(Color);
                        Dust.NewDust(arg_F0F_0, world, arg_F0F_1, arg_F0F_2, arg_F0F_3, arg_F0F_4, arg_F0F_5, arg_F0F_6, newColor, 1f);
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
                        if (Statics.rand.Next(maxValue2) == 0)
                        {
                            Vector2 arg_FE8_0 = new Vector2(this.itemLocation.X - 12f, this.itemLocation.Y - 20f);
                            int arg_FE8_1 = 4;
                            int arg_FE8_2 = 4;
                            int arg_FE8_3 = 6;
                            float arg_FE8_4 = 0f;
                            float arg_FE8_5 = 0f;
                            int arg_FE8_6 = 100;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_FE8_0, world, arg_FE8_1, arg_FE8_2, arg_FE8_3, arg_FE8_4, arg_FE8_5, arg_FE8_6, newColor, 1f);
                        }
                        //Lighting.addLight((int)((this.itemLocation.X - 16f + this.velocity.X) / 16f), (int)((this.itemLocation.Y - 14f) / 16f), 1f);
                    }
                    else
                    {
                        if (Statics.rand.Next(maxValue2) == 0)
                        {
                            Vector2 arg_108C_0 = new Vector2(this.itemLocation.X + 4f, this.itemLocation.Y - 20f);
                            int arg_108C_1 = 4;
                            int arg_108C_2 = 4;
                            int arg_108C_3 = 6;
                            float arg_108C_4 = 0f;
                            float arg_108C_5 = 0f;
                            int arg_108C_6 = 100;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_108C_0, world, arg_108C_1, arg_108C_2, arg_108C_3, arg_108C_4, arg_108C_5, arg_108C_6, newColor, 1f);
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
                            if (Statics.rand.Next(maxValue3) == 0)
                            {
                                Vector2 arg_1169_0 = new Vector2(this.itemLocation.X - 12f, this.itemLocation.Y - 20f);
                                int arg_1169_1 = 4;
                                int arg_1169_2 = 4;
                                int arg_1169_3 = 29;
                                float arg_1169_4 = 0f;
                                float arg_1169_5 = 0f;
                                int arg_1169_6 = 100;
                                Color newColor = default(Color);
                                Dust.NewDust(arg_1169_0, world, arg_1169_1, arg_1169_2, arg_1169_3, arg_1169_4, arg_1169_5, arg_1169_6, newColor, 1f);
                            }
                            //Lighting.addLight((int)((this.itemLocation.X - 16f + this.velocity.X) / 16f), (int)((this.itemLocation.Y - 14f) / 16f), 1f);
                        }
                        else
                        {
                            if (Statics.rand.Next(maxValue3) == 0)
                            {
                                Vector2 arg_120E_0 = new Vector2(this.itemLocation.X + 4f, this.itemLocation.Y - 20f);
                                int arg_120E_1 = 4;
                                int arg_120E_2 = 4;
                                int arg_120E_3 = 29;
                                float arg_120E_4 = 0f;
                                float arg_120E_5 = 0f;
                                int arg_120E_6 = 100;
                                Color newColor = default(Color);
                                Dust.NewDust(arg_120E_0, world, arg_120E_1, arg_120E_2, arg_120E_3, arg_120E_4, arg_120E_5, arg_120E_6, newColor, 1f);
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
            if (i == Statics.myPlayer)
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
                            if (world.getProjectile()[m].active && world.getProjectile()[m].owner == i && world.getProjectile()[m].type == 13)
                            {
                                world.getProjectile()[m].Kill(world);
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
                    if (num7 == 9 && (double)this.position.Y > world.getWorldSurface() * 16.0 + (double)(800 / 2))
                    {
                        flag2 = false;
                    }
                    if (flag2)
                    {
                        if (num7 == 1 && this.inventory[this.selectedItem].type == 120)
                        {
                            num7 = 2;
                        }
                        this.itemTime = this.inventory[this.selectedItem].useTime;
                        //if ((float)Main.mouseState.X + Main.screenPosition.X > this.position.X + (float)this.width * 0.5f)
                        //{
                        //    this.direction = 1;
                        //}
                        //else
                        //{
                        //    this.direction = -1;
                        //}
                        Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                        if (num7 == 9)
                        {
                            vector = new Vector2(this.position.X + (float)this.width * 0.5f + (float)(Statics.rand.Next(601) * -(float)this.direction), this.position.Y + (float)this.height * 0.5f - 300f - (float)Statics.rand.Next(100));
                            num10 = 0f;
                        }
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
                            NetMessage.SendData(41, world, -1, -1, "", this.whoAmi, 0f, 0f, 0f);
                        }
                    }
                }
                if (this.inventory[this.selectedItem].type >= 205 && this.inventory[this.selectedItem].type <= 207 && this.position.X / 16f - (float)tileRangeX - (float)this.inventory[this.selectedItem].tileBoost <= (float)tileTargetX && (this.position.X + (float)this.width) / 16f + (float)tileRangeX + (float)this.inventory[this.selectedItem].tileBoost - 1f >= (float)tileTargetX && this.position.Y / 16f - (float)tileRangeY - (float)this.inventory[this.selectedItem].tileBoost <= (float)tileTargetY && (this.position.Y + (float)this.height) / 16f + (float)tileRangeY + (float)this.inventory[this.selectedItem].tileBoost - 2f >= (float)tileTargetY)
                {
                    this.showItemIcon = true;
                    if (this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
                    {
                        if (this.inventory[this.selectedItem].type == 205)
                        {
                            bool lava = world.getTile()[tileTargetX, tileTargetY].lava;
                            int num14 = 0;
                            for (int num15 = tileTargetX - 1; num15 <= tileTargetX + 1; num15++)
                            {
                                for (int num16 = tileTargetY - 1; num16 <= tileTargetY + 1; num16++)
                                {
                                    if (world.getTile()[num15, num16].lava == lava)
                                    {
                                        num14 += (int)world.getTile()[num15, num16].liquid;
                                    }
                                }
                            }
                            if (world.getTile()[tileTargetX, tileTargetY].liquid > 0 && num14 > 100)
                            {
                                bool lava2 = world.getTile()[tileTargetX, tileTargetY].lava;
                                if (!world.getTile()[tileTargetX, tileTargetY].lava)
                                {
                                    this.inventory[this.selectedItem].SetDefaults(206);
                                }
                                else
                                {
                                    this.inventory[this.selectedItem].SetDefaults(207);
                                }
                                //Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
                                this.itemTime = this.inventory[this.selectedItem].useTime;
                                int num17 = (int)world.getTile()[tileTargetX, tileTargetY].liquid;
                                world.getTile()[tileTargetX, tileTargetY].liquid = 0;
                                world.getTile()[tileTargetX, tileTargetY].lava = false;
                                WorldGen.SquareTileFrame(tileTargetX, tileTargetY, world, false);
                                if (Statics.netMode == 1)
                                {
                                    NetMessage.sendWater(tileTargetX, tileTargetY, world);
                                }
                                else
                                {
                                    Liquid.AddWater(tileTargetX, tileTargetY, world);
                                }
                                for (int num18 = tileTargetX - 1; num18 <= tileTargetX + 1; num18++)
                                {
                                    for (int num19 = tileTargetY - 1; num19 <= tileTargetY + 1; num19++)
                                    {
                                        if (num17 < 256 && world.getTile()[num18, num19].lava == lava)
                                        {
                                            int num20 = (int)world.getTile()[num18, num19].liquid;
                                            if (num20 + num17 > 255)
                                            {
                                                num20 = 255 - num17;
                                            }
                                            num17 += num20;
                                            Tile expr_1B85 = world.getTile()[num18, num19];
                                            expr_1B85.liquid -= (byte)num20;
                                            world.getTile()[num18, num19].lava = lava2;
                                            if (world.getTile()[num18, num19].liquid == 0)
                                            {
                                                world.getTile()[num18, num19].lava = false;
                                            }
                                            WorldGen.SquareTileFrame(num18, num19, world, false);
                                            if (Statics.netMode == 1)
                                            {
                                                NetMessage.sendWater(num18, num19, world);
                                            }
                                            else
                                            {
                                                Liquid.AddWater(num18, num19, world);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (world.getTile()[tileTargetX, tileTargetY].liquid < 200 && (!world.getTile()[tileTargetX, tileTargetY].active || !Statics.tileSolid[(int)world.getTile()[tileTargetX, tileTargetY].type] || !Statics.tileSolidTop[(int)world.getTile()[tileTargetX, tileTargetY].type]))
                            {
                                if (this.inventory[this.selectedItem].type == 207)
                                {
                                    if (world.getTile()[tileTargetX, tileTargetY].liquid == 0 || world.getTile()[tileTargetX, tileTargetY].lava)
                                    {
                                        //Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
                                        world.getTile()[tileTargetX, tileTargetY].lava = true;
                                        world.getTile()[tileTargetX, tileTargetY].liquid = 255;
                                        WorldGen.SquareTileFrame(tileTargetX, tileTargetY, world, true);
                                        this.inventory[this.selectedItem].SetDefaults(205);
                                        this.itemTime = this.inventory[this.selectedItem].useTime;
                                        if (Statics.netMode == 1)
                                        {
                                            NetMessage.sendWater(tileTargetX, tileTargetY, world);
                                        }
                                    }
                                }
                                else
                                {
                                    if (world.getTile()[tileTargetX, tileTargetY].liquid == 0 || !world.getTile()[tileTargetX, tileTargetY].lava)
                                    {
                                        //Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
                                        world.getTile()[tileTargetX, tileTargetY].lava = false;
                                        world.getTile()[tileTargetX, tileTargetY].liquid = 255;
                                        WorldGen.SquareTileFrame(tileTargetX, tileTargetY, world, true);
                                        this.inventory[this.selectedItem].SetDefaults(205);
                                        this.itemTime = this.inventory[this.selectedItem].useTime;
                                        if (Statics.netMode == 1)
                                        {
                                            NetMessage.sendWater(tileTargetX, tileTargetY, world);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if ((this.inventory[this.selectedItem].pick > 0 || this.inventory[this.selectedItem].axe > 0 || this.inventory[this.selectedItem].hammer > 0) && this.position.X / 16f - (float)tileRangeX - (float)this.inventory[this.selectedItem].tileBoost <= (float)tileTargetX && (this.position.X + (float)this.width) / 16f + (float)tileRangeX + (float)this.inventory[this.selectedItem].tileBoost - 1f >= (float)tileTargetX && this.position.Y / 16f - (float)tileRangeY - (float)this.inventory[this.selectedItem].tileBoost <= (float)tileTargetY && (this.position.Y + (float)this.height) / 16f + (float)tileRangeY + (float)this.inventory[this.selectedItem].tileBoost - 2f >= (float)tileTargetY)
                {
                    this.showItemIcon = true;
                    if (world.getTile()[tileTargetX, tileTargetY].active && this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
                    {
                        if (this.hitTileX != tileTargetX || this.hitTileY != tileTargetY)
                        {
                            this.hitTile = 0;
                            this.hitTileX = tileTargetX;
                            this.hitTileY = tileTargetY;
                        }
                        if (Statics.tileNoFail[(int)world.getTile()[tileTargetX, tileTargetY].type])
                        {
                            this.hitTile = 100;
                        }
                        if (world.getTile()[tileTargetX, tileTargetY].type != 27)
                        {
                            if (world.getTile()[tileTargetX, tileTargetY].type == 4 || world.getTile()[tileTargetX, tileTargetY].type == 10 || world.getTile()[tileTargetX, tileTargetY].type == 11 || world.getTile()[tileTargetX, tileTargetY].type == 12 || world.getTile()[tileTargetX, tileTargetY].type == 13 || world.getTile()[tileTargetX, tileTargetY].type == 14 || world.getTile()[tileTargetX, tileTargetY].type == 15 || world.getTile()[tileTargetX, tileTargetY].type == 16 || world.getTile()[tileTargetX, tileTargetY].type == 17 || world.getTile()[tileTargetX, tileTargetY].type == 18 || world.getTile()[tileTargetX, tileTargetY].type == 19 || world.getTile()[tileTargetX, tileTargetY].type == 21 || world.getTile()[tileTargetX, tileTargetY].type == 26 || world.getTile()[tileTargetX, tileTargetY].type == 28 || world.getTile()[tileTargetX, tileTargetY].type == 29 || world.getTile()[tileTargetX, tileTargetY].type == 31 || world.getTile()[tileTargetX, tileTargetY].type == 33 || world.getTile()[tileTargetX, tileTargetY].type == 34 || world.getTile()[tileTargetX, tileTargetY].type == 35 || world.getTile()[tileTargetX, tileTargetY].type == 36 || world.getTile()[tileTargetX, tileTargetY].type == 42 || world.getTile()[tileTargetX, tileTargetY].type == 48 || world.getTile()[tileTargetX, tileTargetY].type == 49 || world.getTile()[tileTargetX, tileTargetY].type == 50 || world.getTile()[tileTargetX, tileTargetY].type == 54 || world.getTile()[tileTargetX, tileTargetY].type == 55 || world.getTile()[tileTargetX, tileTargetY].type == 77 || world.getTile()[tileTargetX, tileTargetY].type == 78 || world.getTile()[tileTargetX, tileTargetY].type == 79)
                            {
                                if (world.getTile()[tileTargetX, tileTargetY].type == 48)
                                {
                                    this.hitTile += this.inventory[this.selectedItem].hammer / 3;
                                }
                                else
                                {
                                    this.hitTile += this.inventory[this.selectedItem].hammer;
                                }
                                if (world.getTile()[tileTargetX, tileTargetY].type == 77 && this.inventory[this.selectedItem].hammer < 60)
                                {
                                    this.hitTile = 0;
                                }
                                if (this.inventory[this.selectedItem].hammer > 0)
                                {
                                    if (world.getTile()[tileTargetX, tileTargetY].type == 26)
                                    {
                                        this.Hurt(this.statLife / 2, -this.direction, false, false);
                                        WorldGen.KillTile(tileTargetX, tileTargetY, world, true, false, false);
                                        if (Statics.netMode == 1)
                                        {
                                            NetMessage.SendData(17, world, -1, -1, "", 0, (float)tileTargetX, (float)tileTargetY, 1f);
                                        }
                                    }
                                    else
                                    {
                                        if (this.hitTile >= 100)
                                        {
                                            if (Statics.netMode == 1 && world.getTile()[tileTargetX, tileTargetY].type == 21)
                                            {
                                                WorldGen.KillTile(tileTargetX, tileTargetY, world, true, false, false);
                                                NetMessage.SendData(17, world, -1, -1, "", 0, (float)tileTargetX, (float)tileTargetY, 1f);
                                                NetMessage.SendData(34, world, -1, -1, "", tileTargetX, (float)tileTargetY, 0f, 0f);
                                            }
                                            else
                                            {
                                                this.hitTile = 0;
                                                WorldGen.KillTile(tileTargetX, tileTargetY, world, false, false, false);
                                                if (Statics.netMode == 1)
                                                {
                                                    NetMessage.SendData(17, world, -1, -1, "", 0, (float)tileTargetX, (float)tileTargetY, 0f);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            WorldGen.KillTile(tileTargetX, tileTargetY, world, true, false, false);
                                            if (Statics.netMode == 1)
                                            {
                                                NetMessage.SendData(17, world, -1, -1, "", 0, (float)tileTargetX, (float)tileTargetY, 1f);
                                            }
                                        }
                                    }
                                    this.itemTime = this.inventory[this.selectedItem].useTime;
                                }
                            }
                            else
                            {
                                if (world.getTile()[tileTargetX, tileTargetY].type == 5 || world.getTile()[tileTargetX, tileTargetY].type == 30 || world.getTile()[tileTargetX, tileTargetY].type == 72)
                                {
                                    if (world.getTile()[tileTargetX, tileTargetY].type == 30)
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
                                            WorldGen.KillTile(tileTargetX, tileTargetY, world, false, false, false);
                                            if (Statics.netMode == 1)
                                            {
                                                NetMessage.SendData(17, world, -1, -1, "", 0, (float)tileTargetX, (float)tileTargetY, 0f);
                                            }
                                        }
                                        else
                                        {
                                            WorldGen.KillTile(tileTargetX, tileTargetY, world, true, false, false);
                                            if (Statics.netMode == 1)
                                            {
                                                NetMessage.SendData(17, world, -1, -1, "", 0, (float)tileTargetX, (float)tileTargetY, 1f);
                                            }
                                        }
                                        this.itemTime = this.inventory[this.selectedItem].useTime;
                                    }
                                }
                                else
                                {
                                    if (this.inventory[this.selectedItem].pick > 0)
                                    {
                                        if (Statics.tileDungeon[(int)world.getTile()[tileTargetX, tileTargetY].type] || world.getTile()[tileTargetX, tileTargetY].type == 37 || world.getTile()[tileTargetX, tileTargetY].type == 25 || world.getTile()[tileTargetX, tileTargetY].type == 58)
                                        {
                                            this.hitTile += this.inventory[this.selectedItem].pick / 2;
                                        }
                                        else
                                        {
                                            this.hitTile += this.inventory[this.selectedItem].pick;
                                        }
                                        if (world.getTile()[tileTargetX, tileTargetY].type == 25 && this.inventory[this.selectedItem].pick < 65)
                                        {
                                            this.hitTile = 0;
                                        }
                                        else
                                        {
                                            if (world.getTile()[tileTargetX, tileTargetY].type == 37 && this.inventory[this.selectedItem].pick < 55)
                                            {
                                                this.hitTile = 0;
                                            }
                                            else
                                            {
                                                if (world.getTile()[tileTargetX, tileTargetY].type == 56 && this.inventory[this.selectedItem].pick < 65)
                                                {
                                                    this.hitTile = 0;
                                                }
                                                else
                                                {
                                                    if (world.getTile()[tileTargetX, tileTargetY].type == 58 && this.inventory[this.selectedItem].pick < 65)
                                                    {
                                                        this.hitTile = 0;
                                                    }
                                                }
                                            }
                                        }
                                        if (world.getTile()[tileTargetX, tileTargetY].type == 0 || world.getTile()[tileTargetX, tileTargetY].type == 40 || world.getTile()[tileTargetX, tileTargetY].type == 53 || world.getTile()[tileTargetX, tileTargetY].type == 59)
                                        {
                                            this.hitTile += this.inventory[this.selectedItem].pick;
                                        }
                                        if (this.hitTile >= 100)
                                        {
                                            this.hitTile = 0;
                                            WorldGen.KillTile(tileTargetX, tileTargetY, world, false, false, false);
                                            if (Statics.netMode == 1)
                                            {
                                                NetMessage.SendData(17, world, -1, -1, "", 0, (float)tileTargetX, (float)tileTargetY, 0f);
                                            }
                                        }
                                        else
                                        {
                                            WorldGen.KillTile(tileTargetX, tileTargetY, world, true, false, false);
                                            if (Statics.netMode == 1)
                                            {
                                                NetMessage.SendData(17, world, -1, -1, "", 0, (float)tileTargetX, (float)tileTargetY, 1f);
                                            }
                                        }
                                        this.itemTime = this.inventory[this.selectedItem].useTime;
                                    }
                                }
                            }
                        }
                    }
                    if (world.getTile()[tileTargetX, tileTargetY].wall > 0 && this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem && this.inventory[this.selectedItem].hammer > 0)
                    {
                        bool flag3 = true;
                        if (!Statics.wallHouse[(int)world.getTile()[tileTargetX, tileTargetY].wall])
                        {
                            flag3 = false;
                            for (int num21 = tileTargetX - 1; num21 < tileTargetX + 2; num21++)
                            {
                                for (int num22 = tileTargetY - 1; num22 < tileTargetY + 2; num22++)
                                {
                                    if (world.getTile()[num21, num22].wall != world.getTile()[tileTargetX, tileTargetY].wall)
                                    {
                                        flag3 = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (flag3)
                        {
                            if (this.hitTileX != tileTargetX || this.hitTileY != tileTargetY)
                            {
                                this.hitTile = 0;
                                this.hitTileX = tileTargetX;
                                this.hitTileY = tileTargetY;
                            }
                            this.hitTile += this.inventory[this.selectedItem].hammer;
                            if (this.hitTile >= 100)
                            {
                                this.hitTile = 0;
                                WorldGen.KillWall(tileTargetX, tileTargetY, world, false);
                                if (Statics.netMode == 1)
                                {
                                    NetMessage.SendData(17, world, -1, -1, "", 2, (float)tileTargetX, (float)tileTargetY, 0f);
                                }
                            }
                            else
                            {
                                WorldGen.KillWall(tileTargetX, tileTargetY, world, true);
                                if (Statics.netMode == 1)
                                {
                                    NetMessage.SendData(17, world, -1, -1, "", 2, (float)tileTargetX, (float)tileTargetY, 1f);
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
                    if (Statics.myPlayer == this.whoAmi)
                    {
                        this.HealEffect(20);
                    }
                }
                if (this.inventory[this.selectedItem].type == 109 && this.itemAnimation > 0 && this.statManaMax < 200 && this.itemTime == 0)
                {
                    this.itemTime = this.inventory[this.selectedItem].useTime;
                    this.statManaMax += 20;
                    this.statMana += 20;
                    if (Statics.myPlayer == this.whoAmi)
                    {
                        this.ManaEffect(20);
                    }
                }
                if (this.inventory[this.selectedItem].createTile >= 0 && this.position.X / 16f - (float)tileRangeX - (float)this.inventory[this.selectedItem].tileBoost <= (float)tileTargetX && (this.position.X + (float)this.width) / 16f + (float)tileRangeX + (float)this.inventory[this.selectedItem].tileBoost - 1f >= (float)tileTargetX && this.position.Y / 16f - (float)tileRangeY - (float)this.inventory[this.selectedItem].tileBoost <= (float)tileTargetY && (this.position.Y + (float)this.height) / 16f + (float)tileRangeY + (float)this.inventory[this.selectedItem].tileBoost - 2f >= (float)tileTargetY)
                {
                    this.showItemIcon = true;
                    if ((!world.getTile()[tileTargetX, tileTargetY].active || this.inventory[this.selectedItem].createTile == 23 || this.inventory[this.selectedItem].createTile == 2 || this.inventory[this.selectedItem].createTile == 60 || this.inventory[this.selectedItem].createTile == 70) && this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem)
                    {
                        bool flag4 = false;
                        if (this.inventory[this.selectedItem].createTile == 23 || this.inventory[this.selectedItem].createTile == 2)
                        {
                            if (world.getTile()[tileTargetX, tileTargetY].active && world.getTile()[tileTargetX, tileTargetY].type == 0)
                            {
                                flag4 = true;
                            }
                        }
                        else
                        {
                            if (this.inventory[this.selectedItem].createTile == 60 || this.inventory[this.selectedItem].createTile == 70)
                            {
                                if (world.getTile()[tileTargetX, tileTargetY].active && world.getTile()[tileTargetX, tileTargetY].type == 59)
                                {
                                    flag4 = true;
                                }
                            }
                            else
                            {
                                if (this.inventory[this.selectedItem].createTile == 4)
                                {
                                    int num23 = (int)world.getTile()[tileTargetX, tileTargetY + 1].type;
                                    int num24 = (int)world.getTile()[tileTargetX - 1, tileTargetY].type;
                                    int num25 = (int)world.getTile()[tileTargetX + 1, tileTargetY].type;
                                    int num26 = (int)world.getTile()[tileTargetX - 1, tileTargetY - 1].type;
                                    int num27 = (int)world.getTile()[tileTargetX + 1, tileTargetY - 1].type;
                                    int num28 = (int)world.getTile()[tileTargetX - 1, tileTargetY - 1].type;
                                    int num29 = (int)world.getTile()[tileTargetX + 1, tileTargetY + 1].type;
                                    if (!world.getTile()[tileTargetX, tileTargetY + 1].active)
                                    {
                                        num23 = -1;
                                    }
                                    if (!world.getTile()[tileTargetX - 1, tileTargetY].active)
                                    {
                                        num24 = -1;
                                    }
                                    if (!world.getTile()[tileTargetX + 1, tileTargetY].active)
                                    {
                                        num25 = -1;
                                    }
                                    if (!world.getTile()[tileTargetX - 1, tileTargetY - 1].active)
                                    {
                                        num26 = -1;
                                    }
                                    if (!world.getTile()[tileTargetX + 1, tileTargetY - 1].active)
                                    {
                                        num27 = -1;
                                    }
                                    if (!world.getTile()[tileTargetX - 1, tileTargetY + 1].active)
                                    {
                                        num28 = -1;
                                    }
                                    if (!world.getTile()[tileTargetX + 1, tileTargetY + 1].active)
                                    {
                                        num29 = -1;
                                    }
                                    if (num23 >= 0 && Statics.tileSolid[num23] && !Statics.tileNoAttach[num23])
                                    {
                                        flag4 = true;
                                    }
                                    else
                                    {
                                        if ((num24 >= 0 && Statics.tileSolid[num24] && !Statics.tileNoAttach[num24]) || (num24 == 5 && num26 == 5 && num28 == 5))
                                        {
                                            flag4 = true;
                                        }
                                        else
                                        {
                                            if ((num25 >= 0 && Statics.tileSolid[num25] && !Statics.tileNoAttach[num25]) || (num25 == 5 && num27 == 5 && num29 == 5))
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
                                        if (world.getTile()[tileTargetX, tileTargetY + 1].active && (Statics.tileSolid[(int)world.getTile()[tileTargetX, tileTargetY + 1].type] || Statics.tileTable[(int)world.getTile()[tileTargetX, tileTargetY + 1].type]))
                                        {
                                            flag4 = true;
                                        }
                                    }
                                    else
                                    {
                                        if (this.inventory[this.selectedItem].createTile == 13 || this.inventory[this.selectedItem].createTile == 29 || this.inventory[this.selectedItem].createTile == 33 || this.inventory[this.selectedItem].createTile == 49)
                                        {
                                            if (world.getTile()[tileTargetX, tileTargetY + 1].active && Statics.tileTable[(int)world.getTile()[tileTargetX, tileTargetY + 1].type])
                                            {
                                                flag4 = true;
                                            }
                                        }
                                        else
                                        {
                                            if (this.inventory[this.selectedItem].createTile == 51)
                                            {
                                                if (world.getTile()[tileTargetX + 1, tileTargetY].active || world.getTile()[tileTargetX + 1, tileTargetY].wall > 0 || world.getTile()[tileTargetX - 1, tileTargetY].active || world.getTile()[tileTargetX - 1, tileTargetY].wall > 0 || world.getTile()[tileTargetX, tileTargetY + 1].active || world.getTile()[tileTargetX, tileTargetY + 1].wall > 0 || world.getTile()[tileTargetX, tileTargetY - 1].active || world.getTile()[tileTargetX, tileTargetY - 1].wall > 0)
                                                {
                                                    flag4 = true;
                                                }
                                            }
                                            else
                                            {
                                                if ((world.getTile()[tileTargetX + 1, tileTargetY].active && Statics.tileSolid[(int)world.getTile()[tileTargetX + 1, tileTargetY].type]) || (world.getTile()[tileTargetX + 1, tileTargetY].wall > 0 || (world.getTile()[tileTargetX - 1, tileTargetY].active && Statics.tileSolid[(int)world.getTile()[tileTargetX - 1, tileTargetY].type])) || (world.getTile()[tileTargetX - 1, tileTargetY].wall > 0 || (world.getTile()[tileTargetX, tileTargetY + 1].active && Statics.tileSolid[(int)world.getTile()[tileTargetX, tileTargetY + 1].type])) || (world.getTile()[tileTargetX, tileTargetY + 1].wall > 0 || (world.getTile()[tileTargetX, tileTargetY - 1].active && Statics.tileSolid[(int)world.getTile()[tileTargetX, tileTargetY - 1].type])) || world.getTile()[tileTargetX, tileTargetY - 1].wall > 0)
                                                {
                                                    flag4 = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (flag4 && WorldGen.PlaceTile(tileTargetX, tileTargetY, world, this.inventory[this.selectedItem].createTile, false, false, this.whoAmi))
                        {
                            this.itemTime = this.inventory[this.selectedItem].useTime;
                            if (Statics.netMode == 1)
                            {
                                NetMessage.SendData(17, world, -1, -1, "", 1, (float)tileTargetX, (float)tileTargetY, (float)this.inventory[this.selectedItem].createTile);
                            }
                            if (this.inventory[this.selectedItem].createTile == 15)
                            {
                                if (this.direction == 1)
                                {
                                    Tile expr_3734 = world.getTile()[tileTargetX, tileTargetY];
                                    expr_3734.frameX += 18;
                                    Tile expr_3759 = world.getTile()[tileTargetX, tileTargetY - 1];
                                    expr_3759.frameX += 18;
                                }
                                if (Statics.netMode == 1)
                                {
                                    NetMessage.SendTileSquare(-1, tileTargetX - 1, tileTargetY - 1, 3, world);
                                }
                            }
                            else
                            {
                                if (this.inventory[this.selectedItem].createTile == 79 && Statics.netMode == 1)
                                {
                                    NetMessage.SendTileSquare(-1, tileTargetX, tileTargetY, 5, world);
                                }
                            }
                        }
                    }
                }
                if (this.inventory[this.selectedItem].createWall >= 0)
                {
                    //tileTargetX = (int)(((float)Main.mouseState.X + Main.screenPosition.X) / 16f);
                    //tileTargetY = (int)(((float)Main.mouseState.Y + Main.screenPosition.Y) / 16f);
                    //if (this.position.X / 16f - (float)tileRangeX - (float)this.inventory[this.selectedItem].tileBoost <= (float)tileTargetX && (this.position.X + (float)this.width) / 16f + (float)tileRangeX + (float)this.inventory[this.selectedItem].tileBoost - 1f >= (float)tileTargetX && this.position.Y / 16f - (float)tileRangeY - (float)this.inventory[this.selectedItem].tileBoost <= (float)tileTargetY && (this.position.Y + (float)this.height) / 16f + (float)tileRangeY + (float)this.inventory[this.selectedItem].tileBoost - 2f >= (float)tileTargetY)
                    //{
                    //    this.showItemIcon = true;
                    //    if (this.itemTime == 0 && this.itemAnimation > 0 && this.controlUseItem && (world.getTile()[tileTargetX + 1, tileTargetY].active || world.getTile()[tileTargetX + 1, tileTargetY].wall > 0 || world.getTile()[tileTargetX - 1, tileTargetY].active || world.getTile()[tileTargetX - 1, tileTargetY].wall > 0 || world.getTile()[tileTargetX, tileTargetY + 1].active || world.getTile()[tileTargetX, tileTargetY + 1].wall > 0 || world.getTile()[tileTargetX, tileTargetY - 1].active || world.getTile()[tileTargetX, tileTargetY - 1].wall > 0) && (int)world.getTile()[tileTargetX, tileTargetY].wall != this.inventory[this.selectedItem].createWall)
                    //    {
                    //        WorldGen.PlaceWall(tileTargetX, tileTargetY, this.inventory[this.selectedItem].createWall, false);
                    //        if ((int)world.getTile()[tileTargetX, tileTargetY].wall == this.inventory[this.selectedItem].createWall)
                    //        {
                    //            this.itemTime = this.inventory[this.selectedItem].useTime;
                    //            if (Main.netMode == 1)
                    //            {
                    //                NetMessage.SendData(17, -1, -1, "", 3, (float)tileTargetX, (float)tileTargetY, (float)this.inventory[this.selectedItem].createWall);
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            if (this.inventory[this.selectedItem].damage >= 0 && this.inventory[this.selectedItem].type > 0 && !this.inventory[this.selectedItem].noMelee && this.itemAnimation > 0)
            {
                bool flag5 = false;
                //Rectangle rectangle = new Rectangle((int)this.itemLocation.X, (int)this.itemLocation.Y, Main.itemTexture[this.inventory[this.selectedItem].type].Width, Main.itemTexture[this.inventory[this.selectedItem].type].Height);
                //rectangle.Width = (int)((float)rectangle.Width * this.inventory[this.selectedItem].scale);
                //rectangle.Height = (int)((float)rectangle.Height * this.inventory[this.selectedItem].scale);
                //if (this.direction == -1)
                //{
                //    rectangle.X -= rectangle.Width;
                //}
                //rectangle.Y -= rectangle.Height;
                //if (this.inventory[this.selectedItem].useStyle == 1)
                //{
                //    if ((double)this.itemAnimation < (double)this.itemAnimationMax * 0.333)
                //    {
                //        if (this.direction == -1)
                //        {
                //            rectangle.X -= (int)((double)rectangle.Width * 1.4 - (double)rectangle.Width);
                //        }
                //        rectangle.Width = (int)((double)rectangle.Width * 1.4);
                //        rectangle.Y += (int)((double)rectangle.Height * 0.5);
                //        rectangle.Height = (int)((double)rectangle.Height * 1.1);
                //    }
                //    else
                //    {
                //        if ((double)this.itemAnimation >= (double)this.itemAnimationMax * 0.666)
                //        {
                //            if (this.direction == 1)
                //            {
                //                rectangle.X -= (int)((double)rectangle.Width * 1.2);
                //            }
                //            rectangle.Width *= 2;
                //            rectangle.Y -= (int)((double)rectangle.Height * 1.4 - (double)rectangle.Height);
                //            rectangle.Height = (int)((double)rectangle.Height * 1.4);
                //        }
                //    }
                //}
                //else
                //{
                //    if (this.inventory[this.selectedItem].useStyle == 3)
                //    {
                //        if ((double)this.itemAnimation > (double)this.itemAnimationMax * 0.666)
                //        {
                //            flag5 = true;
                //        }
                //        else
                //        {
                //            if (this.direction == -1)
                //            {
                //                rectangle.X -= (int)((double)rectangle.Width * 1.4 - (double)rectangle.Width);
                //            }
                //            rectangle.Width = (int)((double)rectangle.Width * 1.4);
                //            rectangle.Y += (int)((double)rectangle.Height * 0.6);
                //            rectangle.Height = (int)((double)rectangle.Height * 0.6);
                //        }
                //    }
                //}
                if (!flag5)
                {
                    /*if ((this.inventory[this.selectedItem].type == 44 || this.inventory[this.selectedItem].type == 45 || this.inventory[this.selectedItem].type == 46 || this.inventory[this.selectedItem].type == 103 || this.inventory[this.selectedItem].type == 104) && Statics.rand.Next(15) == 0)
                    {
                        Vector2 arg_3F0E_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
                        int arg_3F0E_1 = rectangle.Width;
                        int arg_3F0E_2 = rectangle.Height;
                        int arg_3F0E_3 = 14;
                        float arg_3F0E_4 = (float)(this.direction * 2);
                        float arg_3F0E_5 = 0f;
                        int arg_3F0E_6 = 150;
                        Color newColor = default(Color);
                        //Dust.NewDust(arg_3F0E_0, arg_3F0E_1, arg_3F0E_2, arg_3F0E_3, arg_3F0E_4, arg_3F0E_5, arg_3F0E_6, newColor, 1.3f);
                    }
                    if (this.inventory[this.selectedItem].type == 65)
                    {
                        if (Statics.rand.Next(5) == 0)
                        {
                            Vector2 arg_3F7D_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
                            int arg_3F7D_1 = rectangle.Width;
                            int arg_3F7D_2 = rectangle.Height;
                            int arg_3F7D_3 = 15;
                            float arg_3F7D_4 = 0f;
                            float arg_3F7D_5 = 0f;
                            int arg_3F7D_6 = 150;
                            Color newColor = default(Color);
                            //Dust.NewDust(arg_3F7D_0, arg_3F7D_1, arg_3F7D_2, arg_3F7D_3, arg_3F7D_4, arg_3F7D_5, arg_3F7D_6, newColor, 1.2f);
                        }
                        if (Statics.rand.Next(10) == 0)
                        {
                            //Gore.NewGore(new Vector2((float)rectangle.X, (float)rectangle.Y), new Vector2(), Main.rand.Next(16, 18));
                        }
                    }
                    if (this.inventory[this.selectedItem].type == 190 || this.inventory[this.selectedItem].type == 213)
                    {
                        Vector2 arg_4057_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
                        int arg_4057_1 = rectangle.Width;
                        int arg_4057_2 = rectangle.Height;
                        int arg_4057_3 = 40;
                        float arg_4057_4 = this.velocity.X * 0.2f + (float)(this.direction * 3);
                        float arg_4057_5 = this.velocity.Y * 0.2f;
                        int arg_4057_6 = 0;
                        Color newColor = default(Color);
                        //int num30 = Dust.NewDust(arg_4057_0, arg_4057_1, arg_4057_2, arg_4057_3, arg_4057_4, arg_4057_5, arg_4057_6, newColor, 1.2f);
                        //world.getDust()[num30].noGravity = true;
                    }
                    if (this.inventory[this.selectedItem].type == 121)
                    {
                        for (int num31 = 0; num31 < 2; num31++)
                        {
                            Vector2 arg_40EE_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
                            int arg_40EE_1 = rectangle.Width;
                            int arg_40EE_2 = rectangle.Height;
                            int arg_40EE_3 = 6;
                            float arg_40EE_4 = this.velocity.X * 0.2f + (float)(this.direction * 3);
                            float arg_40EE_5 = this.velocity.Y * 0.2f;
                            int arg_40EE_6 = 100;
                            Color newColor = default(Color);
                            //int num32 = Dust.NewDust(arg_40EE_0, arg_40EE_1, arg_40EE_2, arg_40EE_3, arg_40EE_4, arg_40EE_5, arg_40EE_6, newColor, 2.5f);
                            //world.getDust()[num32].noGravity = true;
                            //Dust expr_4110_cp_0 = world.getDust()[num32];
                            //expr_4110_cp_0.velocity.X = expr_4110_cp_0.velocity.X * 2f;
                            //Dust expr_412E_cp_0 = world.getDust()[num32];
                            //expr_412E_cp_0.velocity.Y = expr_412E_cp_0.velocity.Y * 2f;
                        }
                    }
                    if (this.inventory[this.selectedItem].type == 122 || this.inventory[this.selectedItem].type == 217)
                    {
                        Vector2 arg_41DD_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
                        int arg_41DD_1 = rectangle.Width;
                        int arg_41DD_2 = rectangle.Height;
                        int arg_41DD_3 = 6;
                        float arg_41DD_4 = this.velocity.X * 0.2f + (float)(this.direction * 3);
                        float arg_41DD_5 = this.velocity.Y * 0.2f;
                        int arg_41DD_6 = 100;
                        //Color newColor = default(Color);
                        //int num33 = Dust.NewDust(arg_41DD_0, arg_41DD_1, arg_41DD_2, arg_41DD_3, arg_41DD_4, arg_41DD_5, arg_41DD_6, newColor, 1.9f);
                        //world.getDust()[num33].noGravity = true;
                    }
                    if (this.inventory[this.selectedItem].type == 155)
                    {
                        Vector2 arg_4270_0 = new Vector2((float)rectangle.X, (float)rectangle.Y);
                        int arg_4270_1 = rectangle.Width;
                        int arg_4270_2 = rectangle.Height;
                        int arg_4270_3 = 29;
                        float arg_4270_4 = this.velocity.X * 0.2f + (float)(this.direction * 3);
                        float arg_4270_5 = this.velocity.Y * 0.2f;
                        int arg_4270_6 = 100;
                        //Color newColor = default(Color);
                        //int num34 = Dust.NewDust(arg_4270_0, arg_4270_1, arg_4270_2, arg_4270_3, arg_4270_4, arg_4270_5, arg_4270_6, newColor, 2f);
                        //world.getDust()[num34].noGravity = true;
                        //Dust expr_4292_cp_0 = world.getDust()[num34];
                        //expr_4292_cp_0.velocity.X = expr_4292_cp_0.velocity.X / 2f;
                        //Dust expr_42B0_cp_0 = world.getDust()[num34];
                        //expr_42B0_cp_0.velocity.Y = expr_42B0_cp_0.velocity.Y / 2f;
                    }
                    if (this.inventory[this.selectedItem].type >= 198 && this.inventory[this.selectedItem].type <= 203)
                    {
                        Lighting.addLight((int)((this.itemLocation.X + 6f + this.velocity.X) / 16f), (int)((this.itemLocation.Y - 14f) / 16f), 0.5f);
                    }*/
                    if (Statics.myPlayer == i)
                    {
                        /*int num35 = rectangle.X / 16;
                        int num36 = (rectangle.X + rectangle.Width) / 16 + 1;
                        int num37 = rectangle.Y / 16;
                        int num38 = (rectangle.Y + rectangle.Height) / 16 + 1;
                        for (int num39 = num35; num39 < num36; num39++)
                        {
                            for (int num40 = num37; num40 < num38; num40++)
                            {
                                if (world.getTile()[num39, num40].type == 3 || world.getTile()[num39, num40].type == 24 || world.getTile()[num39, num40].type == 28 || world.getTile()[num39, num40].type == 32 || world.getTile()[num39, num40].type == 51 || world.getTile()[num39, num40].type == 52 || world.getTile()[num39, num40].type == 61 || world.getTile()[num39, num40].type == 62 || world.getTile()[num39, num40].type == 69 || world.getTile()[num39, num40].type == 71 || world.getTile()[num39, num40].type == 73 || world.getTile()[num39, num40].type == 74)
                                {
                                    WorldGen.KillTile(num39, num40, world, false, false, false);
                                    if (Statics.netMode == 1)
                                    {
                                        NetMessage.SendData(17, world, -1, -1, "", 0, (float)num39, (float)num40, 0f);
                                    }
                                }
                            }
                        }
                        for (int num41 = 0; num41 < 1000; num41++)
                        {
                            if (world.getNPCs()[num41].active && world.getNPCs()[num41].immune[i] == 0 && this.attackCD == 0 && !world.getNPCs()[num41].friendly)
                            {
                                Rectangle value = new Rectangle((int)world.getNPCs()[num41].position.X, (int)world.getNPCs()[num41].position.Y, world.getNPCs()[num41].width, world.getNPCs()[num41].height);
                                if (rectangle.Intersects(value) && (world.getNPCs()[num41].noTileCollide || Collision.CanHit(this.position, this.width, this.height, world.getNPCs()[num41].position, world.getNPCs()[num41].width, world.getNPCs()[num41].height)))
                                {
                                    world.getNPCs()[num41].StrikeNPC(this.inventory[this.selectedItem].damage, this.inventory[this.selectedItem].knockBack, this.direction);
                                    if (Statics.netMode == 1)
                                    {
                                        NetMessage.SendData(24, world, -1, -1, "", num41, (float)i, 0f, 0f);
                                    }
                                    world.getNPCs()[num41].immune[i] = this.itemAnimation;
                                    this.attackCD = (int)((double)this.itemAnimationMax * 0.33);
                                }
                            }
                        }
                        if (this.hostile)
                        {
                            for (int num42 = 0; num42 < 8; num42++)
                            {
                                if (num42 != i && world.getPlayerList()[num42].active && world.getPlayerList()[num42].hostile && !world.getPlayerList()[num42].immune && !world.getPlayerList()[num42].dead && (world.getPlayerList()[i].team == 0 || world.getPlayerList()[i].team != world.getPlayerList()[num42].team))
                                {
                                    Rectangle value2 = new Rectangle((int)world.getPlayerList()[num42].position.X, (int)world.getPlayerList()[num42].position.Y, world.getPlayerList()[num42].width, world.getPlayerList()[num42].height);
                                    if (rectangle.Intersects(value2) && Collision.CanHit(this.position, this.width, this.height, world.getPlayerList()[num42].position, world.getPlayerList()[num42].width, world.getPlayerList()[num42].height))
                                    {
                                        world.getPlayerList()[num42].Hurt(this.inventory[this.selectedItem].damage, this.direction, true, false);
                                        if (Statics.netMode != 0)
                                        {
                                            NetMessage.SendData(26, world, -1, -1, "", num42, (float)this.direction, (float)this.inventory[this.selectedItem].damage, 1f);
                                        }
                                        this.attackCD = (int)((double)this.itemAnimationMax * 0.33);
                                    }
                                }
                            }
                        }*/
                    }
                }
            }
            if (this.itemTime == 0 && this.itemAnimation > 0)
            {
                if (this.inventory[this.selectedItem].healLife > 0)
                {
                    this.statLife += this.inventory[this.selectedItem].healLife;
                    this.itemTime = this.inventory[this.selectedItem].useTime;
                    if (Statics.myPlayer == this.whoAmi)
                    {
                        this.HealEffect(this.inventory[this.selectedItem].healLife);
                    }
                }
                if (this.inventory[this.selectedItem].healMana > 0)
                {
                    this.statMana += this.inventory[this.selectedItem].healMana;
                    this.itemTime = this.inventory[this.selectedItem].useTime;
                    if (Statics.myPlayer == this.whoAmi)
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
                    if (world.getNPCs()[num44].active && world.getNPCs()[num44].type == num43)
                    {
                        flag6 = true;
                        break;
                    }
                }
                if (flag6)
                {
                    if (Statics.myPlayer == this.whoAmi)
                    {
                        this.Hurt(this.statLife * (this.statDefense + 1), -this.direction, false, false);
                    }
                }
                else
                {
                    if (this.inventory[this.selectedItem].type == 43)
                    {
                        if (!world.isDayTime())
                        {
                            //Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
                            if (Statics.netMode != 1)
                            {
                                NPC.SpawnOnPlayer(i, 4, world);
                            }
                        }
                    }
                    else
                    {
                        if (this.inventory[this.selectedItem].type == 70 && this.zoneEvil)
                        {
                            //Main.PlaySound(15, (int)this.position.X, (int)this.position.Y, 0);
                            if (Statics.netMode != 1)
                            {
                                NPC.SpawnOnPlayer(i, 13, world);
                            }
                        }
                    }
                }
            }
            if (this.inventory[this.selectedItem].type == 50 && this.itemAnimation > 0)
            {
                if (Statics.rand.Next(2) == 0)
                {
                    Vector2 arg_4B43_0 = this.position;
                    int arg_4B43_1 = this.width;
                    int arg_4B43_2 = this.height;
                    int arg_4B43_3 = 15;
                    float arg_4B43_4 = 0f;
                    float arg_4B43_5 = 0f;
                    int arg_4B43_6 = 150;
                    Color newColor = default(Color);
                    Dust.NewDust(arg_4B43_0, world, arg_4B43_1, arg_4B43_2, arg_4B43_3, arg_4B43_4, arg_4B43_5, arg_4B43_6, newColor, 1.1f);
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
                            Vector2 arg_4BDC_0 = this.position;
                            int arg_4BDC_1 = this.width;
                            int arg_4BDC_2 = this.height;
                            int arg_4BDC_3 = 15;
                            float arg_4BDC_4 = this.velocity.X * 0.5f;
                            float arg_4BDC_5 = this.velocity.Y * 0.5f;
                            int arg_4BDC_6 = 150;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_4BDC_0, world, arg_4BDC_1, arg_4BDC_2, arg_4BDC_3, arg_4BDC_4, arg_4BDC_5, arg_4BDC_6, newColor, 1.5f);
                        }
                        this.grappling[0] = -1;
                        this.grapCount = 0;
                        for (int num46 = 0; num46 < 1000; num46++)
                        {
                            if (world.getProjectile()[num46].active && world.getProjectile()[num46].owner == i && world.getProjectile()[num46].aiStyle == 7)
                            {
                                world.getProjectile()[num46].Kill(world);
                            }
                        }
                        Spawn(world);
                        for (int num47 = 0; num47 < 70; num47++)
                        {
                            Vector2 arg_4C8B_0 = this.position;
                            int arg_4C8B_1 = this.width;
                            int arg_4C8B_2 = this.height;
                            int arg_4C8B_3 = 15;
                            float arg_4C8B_4 = 0f;
                            float arg_4C8B_5 = 0f;
                            int arg_4C8B_6 = 150;
                            Color newColor = default(Color);
                            Dust.NewDust(arg_4C8B_0, world, arg_4C8B_1, arg_4C8B_2, arg_4C8B_3, arg_4C8B_4, arg_4C8B_5, arg_4C8B_6, newColor, 1.5f);
                        }
                    }
                }
            }
            if (i == Statics.myPlayer)
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

        public void DropItems(World world)
        {
            for (int i = 0; i < 44; i++)
            {
                if (this.inventory[i].type >= 71 && this.inventory[i].type <= 74)
                {
                    int num = Item.NewItem((int)this.position.X, (int)this.position.Y, world, this.width, this.height, this.inventory[i].type, 1, false);
                    int num2 = this.inventory[i].stack / 2;
                    num2 = this.inventory[i].stack - num2;
                    this.inventory[i].stack -= num2;
                    if (this.inventory[i].stack <= 0)
                    {
                        this.inventory[i] = new Item();
                    }
                    world.getItemList()[num].stack = num2;
                    world.getItemList()[num].velocity.Y = (float)Statics.rand.Next(-20, 1) * 0.2f;
                    world.getItemList()[num].velocity.X = (float)Statics.rand.Next(-20, 21) * 0.2f;
                    world.getItemList()[num].noGrabDelay = 100;
                    if (Statics.netMode == 1)
                    {
                        NetMessage.SendData(21, world, -1, -1, "", num, 0f, 0f, 0f);
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
            Player player = new Player(world);
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
                inventory[i] = (Item)this.inventory[i].Clone();
                if (i < 8)
                {
                    armor[i] = (Item)this.armor[i].Clone();
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
            bool result;
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
                result = true;
                return result;
            }
            return false;
        }
        
        public static bool CheckSpawn(int x, int y, World world)
        {
            if (x < 10 || x > world.getMaxTilesX() - 10 || y < 10 || y > world.getMaxTilesX() - 10)
            {
                return false;
            }
            if (world.getTile()[x, y - 1] == null)
            {
                return false;
            }
            if (!world.getTile()[x, y - 1].active || world.getTile()[x, y - 1].type != 79)
            {
                return false;
            }
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 3; j < y; j++)
                {
                    if (world.getTile()[i, j] == null)
                    {
                        return false;
                    }
                    if (world.getTile()[i, j].active && Statics.tileSolid[(int)world.getTile()[i, j].type] && !Statics.tileSolidTop[(int)world.getTile()[i, j].type])
                    {
                        return false;
                    }
                }
            }
            return WorldGen.StartRoomCheck(x, y - 1, world);
        }
        
        public void FindSpawn(World world)
        {
            for (int i = 0; i < 200; i++)
            {
                if (this.spN[i] == null)
                {
                    this.SpawnX = -1;
                    this.SpawnY = -1;
                    return;
                }
                if (this.spN[i] == world.getName() && this.spI[i] == world.getId())
                {
                    this.SpawnX = this.spX[i];
                    this.SpawnY = this.spY[i];
                    return;
                }
            }
        }
        
        public void ChangeSpawn(int x, int y, World world)
        {
            int num = 0;
            while (num < 200 && this.spN[num] != null)
            {
                if (this.spN[num] == world.getName() && this.spI[num] == world.getId())
                {
                    for (int i = num; i > 0; i--)
                    {
                        this.spN[i] = this.spN[i - 1];
                        this.spI[i] = this.spI[i - 1];
                        this.spX[i] = this.spX[i - 1];
                        this.spY[i] = this.spY[i - 1];
                    }
                    this.spN[0] = world.getName();
                    this.spI[0] = world.getId();
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
            this.spN[0] = world.getName();
            this.spI[0] = world.getId();
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
            EncryptFile(text, playerPath);
            File.Delete(text);
        }
        
        public static Player LoadPlayer(string playerPath, World world)
        {
            bool flag = false;
            if (Statics.rand == null)
            {
                Statics.rand = new Random((int)DateTime.Now.Ticks);
            }
            Player player = new Player(world);
            Player result;
            try
            {
                string text = playerPath + ".dat";
                flag = DecryptFile(playerPath, text);
                if (!flag)
                {
                    using (FileStream fileStream = new FileStream(text, FileMode.Open))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(fileStream))
                        {
                            binaryReader.ReadInt32();
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
                                player.armor[i].SetDefaults(binaryReader.ReadString());
                            }
                            for (int j = 0; j < 44; j++)
                            {
                                player.inventory[j].SetDefaults(binaryReader.ReadString());
                                player.inventory[j].stack = binaryReader.ReadInt32();
                            }
                            for (int k = 0; k < Chest.maxItems; k++)
                            {
                                player.bank[k].SetDefaults(binaryReader.ReadString());
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
                    result = player;
                    return result;
                }
            }
            catch
            {
                flag = true;
            }
            if (!flag)
            {
                return new Player(world);
            }
            string text2 = playerPath + ".bak";
            if (File.Exists(text2))
            {
                File.Delete(playerPath);
                File.Move(text2, playerPath);
                return LoadPlayer(playerPath, world);
            }
            return new Player(world);
        }
       
        public string getPlayerSavePath() {
            return Statics.getPlayerPath + "\\" + name;
        }

        public Player(World World)
        {
            world = World;
            setServer(world.getServer());
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
    
    }
}
