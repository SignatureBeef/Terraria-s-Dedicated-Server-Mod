using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class Item
    {

        public bool accessory;
        public bool active;
        public int alpha;
        public int ammo;
        public bool autoReuse;
        public int axe;
        public bool beingGrabbed;
        public int bodySlot;
        public bool buy;
        public bool channel;
        public Color color = new Color();
        public bool consumable;
        public int createTile;
        public int createWall;
        public int damage;
        public int defense;
        public int hammer;
        public int headSlot;
        public int healLife;
        public int healMana;
        public int height;
        public int holdStyle;
        public int keepTime;
        public float knockBack;
        public bool lavaWet;
        public int legSlot;
        public int lifeRegen;
        public int mana;
        public int manaRegen;
        public int maxStack;
        public string name;
        public int noGrabDelay;
        public bool noMelee;
        public bool noUseGraphic;
        public int owner;
        public int ownIgnore;
        public int ownTime;
        public int pick;
        public Vector2 position = new Vector2();
        public bool potion;
        public int rare;
        public int release;
        public float scale;
        public int shoot;
        public float shootSpeed;
        public int spawnTime;
        public int stack;
        public int tileBoost;
        public string toolTip;
        public int type;
        public int useAmmo;
        public int useAnimation;
        public int useSound;
        public int useStyle;
        public int useTime;
        public bool useTurn;
        public int value;
        public Vector2 velocity = new Vector2();
        public bool wet;
        public byte wetCount;
        public int width;
        public bool wornArmor;

        public static int potionDelay;

        public Item()
        {
            ownIgnore = -1;
            createTile = -1;
            createWall = -1;
            scale = 1.0F;
            headSlot = -1;
            bodySlot = -1;
            legSlot = -1;
            owner = 8;
        }

        static Item()
        {
            Item.potionDelay = 720;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void FindOwner(int whoAmI, World world)
        {
            if (this.keepTime <= 0)
            {
                int num = this.owner;
                this.owner = 8;
                float num2 = -1f;
                for (int i = 0; i < 8; i++)
                {
                    if (this.ownIgnore != i && world.getPlayerList()[i].active && world.getPlayerList()[i].ItemSpace(world.getItemList()[whoAmI]))
                    {
                        float num3 = Math.Abs(world.getPlayerList()[i].position.X + (float)(world.getPlayerList()[i].width / 2) - this.position.X - 
                            (float)(this.width / 2)) + Math.Abs(world.getPlayerList()[i].position.Y + (float)(world.getPlayerList()[i].height / 2) - 
                            this.position.Y - (float)this.height);
                        if (num3 < (float)(600 / 2 + 800 / 2) && (num2 == -1f || num3 < num2))
                        {
                            num2 = num3;
                            this.owner = i;
                        }
                    }
                }
                if (this.owner != num && ((num == Statics.myPlayer && Statics.netMode == 1) || (num == 8 && Statics.netMode == 2) || 
                    !world.getPlayerList()[num].active))
                {
                    NetMessage.SendData(21, world, -1, -1, "", whoAmI, 0f, 0f, 0f);
                    if (this.active)
                    {
                        NetMessage.SendData(22, world, -1, -1, "", whoAmI, 0f, 0f, 0f);
                    }
                }
            }
        }

        public Color GetAlpha(Color newColor)
        {
            // trial
            return newColor;
        }

        public Color GetColor(Color newColor)
        {
            int i1 = color.R - (255 - newColor.R);
            int i2 = color.G - (255 - newColor.G);
            int i3 = color.B - (255 - newColor.B);
            int i4 = color.A - (255 - newColor.A);
            if (i1 < 0)
                i1 = 0;
            if (i1 > 255)
                i1 = 255;
            if (i2 < 0)
                i2 = 0;
            if (i2 > 255)
                i2 = 255;
            if (i3 < 0)
                i3 = 0;
            if (i3 > 255)
                i3 = 255;
            if (i4 < 0)
                i4 = 0;
            if (i4 > 255)
                i4 = 255;
            return new Color(i1, i2, i3, i4);
        }

        public bool IsNotTheSameAs(Item compareItem)
        {
            return this.name != compareItem.name || this.stack != compareItem.stack;
        }

        public bool IsTheSameAs(Item compareItem)
        {
            return this.name == compareItem.name;
        }

        public void SetDefaults(string ItemName)
        {
            this.name = "";
            if (ItemName == "Gold Pickaxe")
            {
                this.SetDefaults(1);
                this.color = new Color(210, 190, 0, 100);
                this.useTime = 17;
                this.pick = 55;
                this.useAnimation = 20;
                this.scale = 1.05f;
                this.damage = 6;
                this.value = 10000;
            }
            else
            {
                if (ItemName == "Gold Broadsword")
                {
                    this.SetDefaults(4);
                    this.color = new Color(210, 190, 0, 100);
                    this.useAnimation = 20;
                    this.damage = 13;
                    this.scale = 1.05f;
                    this.value = 9000;
                }
                else
                {
                    if (ItemName == "Gold Shortsword")
                    {
                        this.SetDefaults(6);
                        this.color = new Color(210, 190, 0, 100);
                        this.damage = 11;
                        this.useAnimation = 11;
                        this.scale = 0.95f;
                        this.value = 7000;
                    }
                    else
                    {
                        if (ItemName == "Gold Axe")
                        {
                            this.SetDefaults(10);
                            this.color = new Color(210, 190, 0, 100);
                            this.useTime = 18;
                            this.axe = 11;
                            this.useAnimation = 26;
                            this.scale = 1.15f;
                            this.damage = 7;
                            this.value = 8000;
                        }
                        else
                        {
                            if (ItemName == "Gold Hammer")
                            {
                                this.SetDefaults(7);
                                this.color = new Color(210, 190, 0, 100);
                                this.useAnimation = 28;
                                this.useTime = 23;
                                this.scale = 1.25f;
                                this.damage = 9;
                                this.hammer = 55;
                                this.value = 8000;
                            }
                            else
                            {
                                if (ItemName == "Gold Bow")
                                {
                                    this.SetDefaults(99);
                                    this.useAnimation = 26;
                                    this.useTime = 26;
                                    this.color = new Color(210, 190, 0, 100);
                                    this.damage = 11;
                                    this.value = 7000;
                                }
                                else
                                {
                                    if (ItemName == "Silver Pickaxe")
                                    {
                                        this.SetDefaults(1);
                                        this.color = new Color(180, 180, 180, 100);
                                        this.useTime = 11;
                                        this.pick = 45;
                                        this.useAnimation = 19;
                                        this.scale = 1.05f;
                                        this.damage = 6;
                                        this.value = 5000;
                                    }
                                    else
                                    {
                                        if (ItemName == "Silver Broadsword")
                                        {
                                            this.SetDefaults(4);
                                            this.color = new Color(180, 180, 180, 100);
                                            this.useAnimation = 21;
                                            this.damage = 11;
                                            this.value = 4500;
                                        }
                                        else
                                        {
                                            if (ItemName == "Silver Shortsword")
                                            {
                                                this.SetDefaults(6);
                                                this.color = new Color(180, 180, 180, 100);
                                                this.damage = 9;
                                                this.useAnimation = 12;
                                                this.scale = 0.95f;
                                                this.value = 3500;
                                            }
                                            else
                                            {
                                                if (ItemName == "Silver Axe")
                                                {
                                                    this.SetDefaults(10);
                                                    this.color = new Color(180, 180, 180, 100);
                                                    this.useTime = 18;
                                                    this.axe = 10;
                                                    this.useAnimation = 26;
                                                    this.scale = 1.15f;
                                                    this.damage = 6;
                                                    this.value = 4000;
                                                }
                                                else
                                                {
                                                    if (ItemName == "Silver Hammer")
                                                    {
                                                        this.SetDefaults(7);
                                                        this.color = new Color(180, 180, 180, 100);
                                                        this.useAnimation = 29;
                                                        this.useTime = 19;
                                                        this.scale = 1.25f;
                                                        this.damage = 9;
                                                        this.hammer = 45;
                                                        this.value = 4000;
                                                    }
                                                    else
                                                    {
                                                        if (ItemName == "Silver Bow")
                                                        {
                                                            this.SetDefaults(99);
                                                            this.useAnimation = 27;
                                                            this.useTime = 27;
                                                            this.color = new Color(180, 180, 180, 100);
                                                            this.damage = 10;
                                                            this.value = 3500;
                                                        }
                                                        else
                                                        {
                                                            if (ItemName == "Copper Pickaxe")
                                                            {
                                                                this.SetDefaults(1);
                                                                this.color = new Color(180, 100, 45, 80);
                                                                this.useTime = 15;
                                                                this.pick = 35;
                                                                this.useAnimation = 23;
                                                                this.scale = 0.9f;
                                                                this.tileBoost = -1;
                                                                this.value = 500;
                                                            }
                                                            else
                                                            {
                                                                if (ItemName == "Copper Broadsword")
                                                                {
                                                                    this.SetDefaults(4);
                                                                    this.color = new Color(180, 100, 45, 80);
                                                                    this.useAnimation = 23;
                                                                    this.damage = 8;
                                                                    this.value = 450;
                                                                }
                                                                else
                                                                {
                                                                    if (ItemName == "Copper Shortsword")
                                                                    {
                                                                        this.SetDefaults(6);
                                                                        this.color = new Color(180, 100, 45, 80);
                                                                        this.damage = 6;
                                                                        this.useAnimation = 13;
                                                                        this.scale = 0.8f;
                                                                        this.value = 350;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (ItemName == "Copper Axe")
                                                                        {
                                                                            this.SetDefaults(10);
                                                                            this.color = new Color(180, 100, 45, 80);
                                                                            this.useTime = 21;
                                                                            this.axe = 8;
                                                                            this.useAnimation = 30;
                                                                            this.scale = 1f;
                                                                            this.damage = 3;
                                                                            this.tileBoost = -1;
                                                                            this.value = 400;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (ItemName == "Copper Hammer")
                                                                            {
                                                                                this.SetDefaults(7);
                                                                                this.color = new Color(180, 100, 45, 80);
                                                                                this.useAnimation = 33;
                                                                                this.useTime = 23;
                                                                                this.scale = 1.1f;
                                                                                this.damage = 4;
                                                                                this.hammer = 35;
                                                                                this.tileBoost = -1;
                                                                                this.value = 400;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (ItemName == "Copper Bow")
                                                                                {
                                                                                    this.SetDefaults(99);
                                                                                    this.useAnimation = 29;
                                                                                    this.useTime = 29;
                                                                                    this.color = new Color(180, 100, 45, 80);
                                                                                    this.damage = 8;
                                                                                    this.value = 350;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (ItemName != "")
                                                                                    {
                                                                                        for (int i = 0; i < 236; i++)
                                                                                        {
                                                                                            this.SetDefaults(i);
                                                                                            if (this.name == ItemName)
                                                                                            {
                                                                                                break;
                                                                                            }
                                                                                            if (i == 235)
                                                                                            {
                                                                                                this.SetDefaults(0);
                                                                                                this.name = "";
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
            if (this.type != 0)
            {
                this.name = ItemName;
            }
        }

        public void SetDefaults(int Type)
        {
            if ((Statics.netMode == 1) || (Statics.netMode == 2))
                owner = 8;
            else
                owner = Statics.myPlayer;
            mana = 0;
            wet = false;
            wetCount = 0;
            lavaWet = false;
            channel = false;
            manaRegen = 0;
            release = 0;
            noMelee = false;
            noUseGraphic = false;
            lifeRegen = 0;
            shootSpeed = 0.0F;
            active = true;
            alpha = 0;
            ammo = 0;
            useAmmo = 0;
            autoReuse = false;
            accessory = false;
            axe = 0;
            healMana = 0;
            bodySlot = -1;
            legSlot = -1;
            headSlot = -1;
            potion = false;
            color = new Color();
            consumable = false;
            createTile = -1;
            createWall = -1;
            damage = -1;
            defense = 0;
            hammer = 0;
            healLife = 0;
            holdStyle = 0;
            knockBack = 0.0F;
            maxStack = 1;
            pick = 0;
            rare = 0;
            scale = 1.0F;
            shoot = 0;
            stack = 1;
            toolTip = null;
            tileBoost = 0;
            type = Type;
            useStyle = 0;
            useSound = 0;
            useTime = 100;
            useAnimation = 100;
            value = 0;
            useTurn = false;
            buy = false;
            if (type == 1)
            {
                name = "Iron Pickaxe";
                useStyle = 1;
                useTurn = true;
                useAnimation = 20;
                useTime = 13;
                autoReuse = true;
                width = 24;
                height = 28;
                damage = 5;
                pick = 45;
                useSound = 1;
                knockBack = 2.0F;
                value = 2000;
            }
            else if (type == 2)
            {
                name = "Dirt Block";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 250;
                consumable = true;
                createTile = 0;
                width = 12;
                height = 12;
            }
            else if (type == 3)
            {
                name = "Stone Block";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 250;
                consumable = true;
                createTile = 1;
                width = 12;
                height = 12;
            }
            else if (type == 4)
            {
                name = "Iron Broadsword";
                useStyle = 1;
                useTurn = false;
                useAnimation = 21;
                useTime = 21;
                width = 24;
                height = 28;
                damage = 10;
                knockBack = 5.0F;
                useSound = 1;
                scale = 1.0F;
                value = 1800;
            }
            else if (type == 5)
            {
                name = "Mushroom";
                useStyle = 2;
                useSound = 2;
                useTurn = false;
                useAnimation = 17;
                useTime = 17;
                width = 16;
                height = 18;
                healLife = 20;
                maxStack = 99;
                consumable = true;
                potion = true;
                value = 50;
            }
            else if (type == 6)
            {
                name = "Iron Shortsword";
                useStyle = 3;
                useTurn = false;
                useAnimation = 12;
                useTime = 12;
                width = 24;
                height = 28;
                damage = 8;
                knockBack = 4.0F;
                scale = 0.9F;
                useSound = 1;
                useTurn = true;
                value = 1400;
            }
            else if (type == 7)
            {
                name = "Iron Hammer";
                autoReuse = true;
                useStyle = 1;
                useTurn = true;
                useAnimation = 30;
                useTime = 20;
                hammer = 45;
                width = 24;
                height = 28;
                damage = 7;
                knockBack = 5.5F;
                scale = 1.2F;
                useSound = 1;
                value = 1600;
            }
            else if (type == 8)
            {
                name = "Torch";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                holdStyle = 1;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 4;
                width = 10;
                height = 12;
                toolTip = "Provides light";
                value = 50;
            }
            else if (type == 9)
            {
                name = "Wood";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 250;
                consumable = true;
                createTile = 30;
                width = 8;
                height = 10;
            }
            else if (type == 10)
            {
                name = "Iron Axe";
                useStyle = 1;
                useTurn = true;
                useAnimation = 27;
                knockBack = 4.5F;
                useTime = 19;
                autoReuse = true;
                width = 24;
                height = 28;
                damage = 5;
                axe = 9;
                scale = 1.1F;
                useSound = 1;
                value = 1600;
            }
            else if (type == 11)
            {
                name = "Iron Ore";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 6;
                width = 12;
                height = 12;
                value = 500;
            }
            else if (type == 12)
            {
                name = "Copper Ore";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 7;
                width = 12;
                height = 12;
                value = 250;
            }
            else if (type == 13)
            {
                name = "Gold Ore";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 8;
                width = 12;
                height = 12;
                value = 2000;
            }
            else if (type == 14)
            {
                name = "Silver Ore";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 9;
                width = 12;
                height = 12;
                value = 1000;
            }
            else if (type == 15)
            {
                name = "Copper Watch";
                width = 24;
                height = 28;
                accessory = true;
                toolTip = "Tells the time";
                value = 1000;
            }
            else if (type == 16)
            {
                name = "Silver Watch";
                width = 24;
                height = 28;
                accessory = true;
                toolTip = "Tells the time";
                value = 5000;
            }
            else if (type == 17)
            {
                name = "Gold Watch";
                width = 24;
                height = 28;
                accessory = true;
                rare = 1;
                toolTip = "Tells the time";
                value = 10000;
            }
            else if (type == 18)
            {
                name = "Depth Meter";
                width = 24;
                height = 18;
                accessory = true;
                rare = 1;
                toolTip = "Shows depth";
                value = 10000;
            }
            else if (type == 19)
            {
                name = "Gold Bar";
                width = 20;
                height = 20;
                maxStack = 99;
                value = 6000;
            }
            else if (type == 20)
            {
                name = "Copper Bar";
                width = 20;
                height = 20;
                maxStack = 99;
                value = 750;
            }
            else if (type == 21)
            {
                name = "Silver Bar";
                width = 20;
                height = 20;
                maxStack = 99;
                value = 3000;
            }
            else if (type == 22)
            {
                name = "Iron Bar";
                width = 20;
                height = 20;
                maxStack = 99;
                value = 1500;
            }
            else if (type == 23)
            {
                name = "Gel";
                width = 10;
                height = 12;
                maxStack = 99;
                alpha = 175;
                color = new Color(0, 80, 255, 100);
                toolTip = "'Both tasty and flammable'";
                value = 5;
            }
            else if (type == 24)
            {
                name = "Wooden Sword";
                useStyle = 1;
                useTurn = false;
                useAnimation = 25;
                width = 24;
                height = 28;
                damage = 7;
                knockBack = 4.0F;
                scale = 0.95F;
                useSound = 1;
                value = 100;
            }
            else if (type == 25)
            {
                name = "Wooden Door";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                maxStack = 99;
                consumable = true;
                createTile = 10;
                width = 14;
                height = 28;
                value = 200;
            }
            else if (type == 26)
            {
                name = "Stone Wall";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 250;
                consumable = true;
                createWall = 1;
                width = 12;
                height = 12;
            }
            else if (type == 27)
            {
                name = "Acorn";
                useTurn = true;
                useStyle = 1;
                useAnimation = 15;
                useTime = 10;
                maxStack = 99;
                consumable = true;
                createTile = 20;
                width = 18;
                height = 18;
                value = 10;
            }
            else if (type == 28)
            {
                name = "Lesser Healing Potion";
                useSound = 3;
                healLife = 100;
                useStyle = 2;
                useTurn = true;
                useAnimation = 17;
                useTime = 17;
                maxStack = 30;
                consumable = true;
                width = 14;
                height = 24;
                potion = true;
                value = 200;
            }
            else if (type == 29)
            {
                name = "Life Crystal";
                maxStack = 99;
                consumable = true;
                width = 18;
                height = 18;
                useStyle = 4;
                useTime = 30;
                useSound = 4;
                useAnimation = 30;
                toolTip = "Increases maximum life";
                rare = 2;
            }
            else if (type == 30)
            {
                name = "Dirt Wall";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 250;
                consumable = true;
                createWall = 2;
                width = 12;
                height = 12;
            }
            else if (type == 31)
            {
                name = "Bottle";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 13;
                width = 16;
                height = 24;
                value = 100;
            }
            else if (type == 32)
            {
                name = "Wooden Table";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 14;
                width = 26;
                height = 20;
                value = 300;
            }
            else if (type == 33)
            {
                name = "Furnace";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 17;
                width = 26;
                height = 24;
                value = 300;
            }
            else if (type == 34)
            {
                name = "Wooden Chair";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 15;
                width = 12;
                height = 30;
                value = 150;
            }
            else if (type == 35)
            {
                name = "Iron Anvil";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 16;
                width = 28;
                height = 14;
                value = 5000;
            }
            else if (type == 36)
            {
                name = "Work Bench";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 18;
                width = 28;
                height = 14;
                value = 150;
            }
            else if (type == 37)
            {
                name = "Goggles";
                width = 28;
                height = 12;
                defense = 1;
                headSlot = 10;
                rare = 1;
                value = 1000;
            }
            else if (type == 38)
            {
                name = "Lens";
                width = 12;
                height = 20;
                maxStack = 99;
                value = 500;
            }
            else if (type == 39)
            {
                useStyle = 5;
                useAnimation = 30;
                useTime = 30;
                name = "Wooden Bow";
                width = 12;
                height = 28;
                shoot = 1;
                useAmmo = 1;
                useSound = 5;
                damage = 5;
                shootSpeed = 6.1F;
                noMelee = true;
                value = 100;
            }
            else if (type == 40)
            {
                name = "Wooden Arrow";
                shootSpeed = 3.0F;
                shoot = 1;
                damage = 5;
                width = 10;
                height = 28;
                maxStack = 250;
                consumable = true;
                ammo = 1;
                knockBack = 2.0F;
                value = 10;
            }
            else if (type == 41)
            {
                name = "Flaming Arrow";
                shootSpeed = 3.5F;
                shoot = 2;
                damage = 7;
                width = 10;
                height = 28;
                maxStack = 250;
                consumable = true;
                ammo = 1;
                knockBack = 2.0F;
                value = 15;
            }
            else if (type == 42)
            {
                useStyle = 1;
                name = "Shuriken";
                shootSpeed = 9.0F;
                shoot = 3;
                damage = 10;
                width = 18;
                height = 20;
                maxStack = 250;
                consumable = true;
                useSound = 1;
                useAnimation = 15;
                useTime = 15;
                noUseGraphic = true;
                noMelee = true;
                value = 20;
            }
            else if (type == 43)
            {
                useStyle = 4;
                name = "Suspicious Looking Eye";
                width = 22;
                height = 14;
                consumable = true;
                useAnimation = 45;
                useTime = 45;
                toolTip = "May cause terrible things to occur";
            }
            else if (type == 44)
            {
                useStyle = 5;
                useAnimation = 25;
                useTime = 25;
                name = "Demon Bow";
                width = 12;
                height = 28;
                shoot = 1;
                useAmmo = 1;
                useSound = 5;
                damage = 13;
                shootSpeed = 6.7F;
                knockBack = 1.0F;
                alpha = 30;
                rare = 1;
                noMelee = true;
                value = 18000;
            }
            else if (type == 45)
            {
                name = "War Axe of the Night";
                autoReuse = true;
                useStyle = 1;
                useAnimation = 30;
                knockBack = 6.0F;
                useTime = 15;
                width = 24;
                height = 28;
                damage = 21;
                axe = 15;
                scale = 1.2F;
                useSound = 1;
                rare = 1;
                value = 13500;
            }
            else if (type == 46)
            {
                name = "Light's Bane";
                useStyle = 1;
                useAnimation = 20;
                knockBack = 5.0F;
                width = 24;
                height = 28;
                damage = 16;
                scale = 1.1F;
                useSound = 1;
                rare = 1;
                value = 13500;
            }
            else if (type == 47)
            {
                name = "Unholy Arrow";
                shootSpeed = 3.4F;
                shoot = 4;
                damage = 8;
                width = 10;
                height = 28;
                maxStack = 250;
                consumable = true;
                ammo = 1;
                knockBack = 3.0F;
                alpha = 30;
                rare = 1;
                value = 40;
            }
            else if (type == 48)
            {
                name = "Chest";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 21;
                width = 26;
                height = 22;
                value = 500;
            }
            else if (type == 49)
            {
                name = "Band of Regeneration";
                width = 22;
                height = 22;
                accessory = true;
                lifeRegen = 1;
                rare = 1;
                toolTip = "Slowly regenerates life";
                value = 50000;
            }
            else if (type == 50)
            {
                name = "Magic Mirror";
                useTurn = true;
                width = 20;
                height = 20;
                useStyle = 4;
                useTime = 90;
                useSound = 6;
                useAnimation = 90;
                toolTip = "Gaze in the mirror to return home";
                rare = 1;
                value = 50000;
            }
            else if (type == 51)
            {
                name = "Jester's Arrow";
                shootSpeed = 0.5F;
                shoot = 5;
                damage = 9;
                width = 10;
                height = 28;
                maxStack = 250;
                consumable = true;
                ammo = 1;
                knockBack = 4.0F;
                rare = 1;
                value = 100;
            }
            else if (type == 52)
            {
                name = "Angel Statue";
                width = 24;
                height = 28;
                toolTip = "It doesn't do anything";
                value = 1;
            }
            else if (type == 53)
            {
                name = "Cloud in a Bottle";
                width = 16;
                height = 24;
                accessory = true;
                rare = 1;
                toolTip = "Allows the holder to double jump";
                value = 50000;
            }
            else if (type == 54)
            {
                name = "Hermes Boots";
                width = 28;
                height = 24;
                accessory = true;
                rare = 1;
                toolTip = "The wearer can run super fast";
                value = 50000;
            }
            else if (type == 55)
            {
                noMelee = true;
                useStyle = 1;
                name = "Enchanted Boomerang";
                shootSpeed = 10.0F;
                shoot = 6;
                damage = 13;
                knockBack = 8.0F;
                width = 14;
                height = 28;
                useSound = 1;
                useAnimation = 15;
                useTime = 15;
                noUseGraphic = true;
                rare = 1;
                value = 50000;
            }
            else if (type == 56)
            {
                name = "Demonite Ore";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 22;
                width = 12;
                height = 12;
                rare = 1;
                toolTip = "Pulsing with dark energy";
                value = 4000;
            }
            else if (type == 57)
            {
                name = "Demonite Bar";
                width = 20;
                height = 20;
                maxStack = 99;
                rare = 1;
                toolTip = "Pulsing with dark energy";
                value = 16000;
            }
            else if (type == 58)
            {
                name = "Heart";
                width = 12;
                height = 12;
            }
            else if (type == 59)
            {
                name = "Corrupt Seeds";
                useTurn = true;
                useStyle = 1;
                useAnimation = 15;
                useTime = 10;
                maxStack = 99;
                consumable = true;
                createTile = 23;
                width = 14;
                height = 14;
                value = 500;
            }
            else if (type == 60)
            {
                name = "Vile Mushroom";
                width = 16;
                height = 18;
                maxStack = 99;
                value = 50;
            }
            else if (type == 61)
            {
                name = "Ebonstone Block";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 250;
                consumable = true;
                createTile = 25;
                width = 12;
                height = 12;
            }
            else if (type == 62)
            {
                name = "Grass Seeds";
                useTurn = true;
                useStyle = 1;
                useAnimation = 15;
                useTime = 10;
                maxStack = 99;
                consumable = true;
                createTile = 2;
                width = 14;
                height = 14;
                value = 20;
            }
            else if (type == 63)
            {
                name = "Sunflower";
                useTurn = true;
                useStyle = 1;
                useAnimation = 15;
                useTime = 10;
                maxStack = 99;
                consumable = true;
                createTile = 27;
                width = 26;
                height = 26;
                value = 200;
            }
            else if (type == 64)
            {
                mana = 5;
                damage = 8;
                useStyle = 1;
                name = "Vilethorn";
                shootSpeed = 32.0F;
                shoot = 7;
                width = 26;
                height = 28;
                useSound = 8;
                useAnimation = 30;
                useTime = 30;
                rare = 1;
                noMelee = true;
                toolTip = "Summons a vile thorn";
                value = 10000;
            }
            else if (type == 65)
            {
                mana = 11;
                knockBack = 5.0F;
                alpha = 100;
                color = new Color(150, 150, 150, 0);
                damage = 15;
                useStyle = 1;
                scale = 1.15F;
                name = "Starfury";
                shootSpeed = 12.0F;
                shoot = 9;
                width = 14;
                height = 28;
                useSound = 9;
                useAnimation = 25;
                useTime = 10;
                rare = 1;
                toolTip = "Forged with the fury of heaven";
                value = 50000;
            }
            else if (type == 66)
            {
                useStyle = 1;
                name = "Purification Powder";
                shootSpeed = 4.0F;
                shoot = 10;
                width = 16;
                height = 24;
                maxStack = 99;
                consumable = true;
                useSound = 1;
                useAnimation = 15;
                useTime = 15;
                noMelee = true;
                toolTip = "Cleanses the corruption";
                value = 75;
            }
            else if (type == 67)
            {
                damage = 8;
                useStyle = 1;
                name = "Vile Powder";
                shootSpeed = 4.0F;
                shoot = 11;
                width = 16;
                height = 24;
                maxStack = 99;
                consumable = true;
                useSound = 1;
                useAnimation = 15;
                useTime = 15;
                noMelee = true;
                value = 100;
            }
            else if (type == 68)
            {
                name = "Rotten Chunk";
                width = 18;
                height = 20;
                maxStack = 99;
                toolTip = "Looks tasty!";
                value = 10;
            }
            else if (type == 69)
            {
                name = "Worm Tooth";
                width = 8;
                height = 20;
                maxStack = 99;
                value = 100;
            }
            else if (type == 70)
            {
                useStyle = 4;
                consumable = true;
                useAnimation = 45;
                useTime = 45;
                name = "Worm Food";
                width = 28;
                height = 28;
                toolTip = "May attract giant worms";
            }
            else if (type == 71)
            {
                name = "Copper Coin";
                width = 10;
                height = 12;
                maxStack = 100;
            }
            else if (type == 72)
            {
                name = "Silver Coin";
                width = 10;
                height = 12;
                maxStack = 100;
            }
            else if (type == 73)
            {
                name = "Gold Coin";
                width = 10;
                height = 12;
                maxStack = 100;
            }
            else if (type == 74)
            {
                name = "Platinum Coin";
                width = 10;
                height = 12;
                maxStack = 100;
            }
            else if (type == 75)
            {
                name = "Fallen Star";
                width = 18;
                height = 20;
                maxStack = 100;
                alpha = 75;
                ammo = 15;
                toolTip = "Disappears after the sunrise";
                value = 500;
                useStyle = 4;
                useSound = 4;
                useTurn = false;
                useAnimation = 17;
                useTime = 17;
                healMana = 20;
                consumable = true;
                rare = 1;
                potion = true;
            }
            else if (type == 76)
            {
                name = "Copper Greaves";
                width = 18;
                height = 28;
                defense = 1;
                legSlot = 1;
                value = 750;
            }
            else if (type == 77)
            {
                name = "Iron Greaves";
                width = 18;
                height = 28;
                defense = 2;
                legSlot = 2;
                value = 3000;
            }
            else if (type == 78)
            {
                name = "Silver Greaves";
                width = 18;
                height = 28;
                defense = 3;
                legSlot = 3;
                value = 7500;
            }
            else if (type == 79)
            {
                name = "Gold Greaves";
                width = 18;
                height = 28;
                defense = 4;
                legSlot = 4;
                value = 15000;
            }
            else if (type == 80)
            {
                name = "Copper Chainmail";
                width = 26;
                height = 28;
                defense = 2;
                bodySlot = 1;
                value = 1000;
            }
            else if (type == 81)
            {
                name = "Iron Chainmail";
                width = 26;
                height = 28;
                defense = 3;
                bodySlot = 2;
                value = 4000;
            }
            else if (type == 82)
            {
                name = "Silver Chainmail";
                width = 26;
                height = 28;
                defense = 4;
                bodySlot = 3;
                value = 10000;
            }
            else if (type == 83)
            {
                name = "Gold Chainmail";
                width = 26;
                height = 28;
                defense = 5;
                bodySlot = 4;
                value = 20000;
            }
            else if (type == 84)
            {
                noUseGraphic = true;
                damage = 0;
                knockBack = 7.0F;
                useStyle = 5;
                name = "Grappling Hook";
                shootSpeed = 11.0F;
                shoot = 13;
                width = 18;
                height = 28;
                useSound = 1;
                useAnimation = 20;
                useTime = 20;
                rare = 1;
                noMelee = true;
                value = 20000;
            }
            else if (type == 85)
            {
                name = "Iron Chain";
                width = 14;
                height = 20;
                maxStack = 99;
                value = 1000;
            }
            else if (type == 86)
            {
                name = "Shadow Scale";
                width = 14;
                height = 18;
                maxStack = 99;
                rare = 1;
                value = 500;
            }
            else if (type == 87)
            {
                name = "Piggy Bank";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 29;
                width = 20;
                height = 12;
                value = 10000;
            }
            else if (type == 88)
            {
                name = "Mining Helmet";
                width = 22;
                height = 16;
                defense = 1;
                headSlot = 11;
                rare = 1;
                value = 80000;
                toolTip = "Provides light when worn";
            }
            else if (type == 89)
            {
                name = "Copper Helmet";
                width = 22;
                height = 22;
                defense = 1;
                headSlot = 1;
                value = 1250;
            }
            else if (type == 90)
            {
                name = "Iron Helmet";
                width = 22;
                height = 22;
                defense = 2;
                headSlot = 2;
                value = 5000;
            }
            else if (type == 91)
            {
                name = "Silver Helmet";
                width = 22;
                height = 22;
                defense = 3;
                headSlot = 3;
                value = 12500;
            }
            else if (type == 92)
            {
                name = "Gold Helmet";
                width = 22;
                height = 22;
                defense = 4;
                headSlot = 4;
                value = 25000;
            }
            else if (type == 93)
            {
                name = "Wood Wall";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 250;
                consumable = true;
                createWall = 4;
                width = 12;
                height = 12;
            }
            else if (type == 94)
            {
                name = "Wood Platform";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 19;
                width = 8;
                height = 10;
            }
            else if (type == 95)
            {
                useStyle = 5;
                useAnimation = 20;
                useTime = 20;
                name = "Flintlock Pistol";
                width = 24;
                height = 28;
                shoot = 14;
                useAmmo = 14;
                useSound = 11;
                damage = 7;
                shootSpeed = 5.0F;
                noMelee = true;
                value = 50000;
                scale = 0.9F;
                rare = 1;
            }
            else if (type == 96)
            {
                useStyle = 5;
                autoReuse = true;
                useAnimation = 45;
                useTime = 45;
                name = "Musket";
                width = 44;
                height = 14;
                shoot = 10;
                useAmmo = 14;
                useSound = 11;
                damage = 14;
                shootSpeed = 8.0F;
                noMelee = true;
                value = 100000;
                knockBack = 4.0F;
                rare = 1;
            }
            else if (type == 97)
            {
                name = "Musket Ball";
                shootSpeed = 4.0F;
                shoot = 14;
                damage = 7;
                width = 8;
                height = 8;
                maxStack = 250;
                consumable = true;
                ammo = 14;
                knockBack = 2.0F;
                value = 8;
            }
            else if (type == 98)
            {
                useStyle = 5;
                autoReuse = true;
                useAnimation = 8;
                useTime = 8;
                name = "Minishark";
                width = 50;
                height = 18;
                shoot = 10;
                useAmmo = 14;
                useSound = 11;
                damage = 5;
                shootSpeed = 7.0F;
                noMelee = true;
                value = 500000;
                rare = 2;
                toolTip = "Half shark, half gun, completely awesome.";
            }
            else if (type == 99)
            {
                useStyle = 5;
                useAnimation = 28;
                useTime = 28;
                name = "Iron Bow";
                width = 12;
                height = 28;
                shoot = 1;
                useAmmo = 1;
                useSound = 5;
                damage = 9;
                shootSpeed = 6.6F;
                noMelee = true;
                value = 1400;
            }
            else if (type == 100)
            {
                name = "Shadow Greaves";
                width = 18;
                height = 28;
                defense = 6;
                legSlot = 5;
                rare = 1;
                value = 22500;
            }
            else if (type == 101)
            {
                name = "Shadow Scalemail";
                width = 26;
                height = 28;
                defense = 7;
                bodySlot = 5;
                rare = 1;
                value = 30000;
            }
            else
            {
                if (type == 102)
                {
                    name = "Shadow Helmet";
                    width = 22;
                    height = 22;
                    defense = 6;
                    headSlot = 5;
                    rare = 1;
                    value = 37500;
                }
                else
                {
                    if (type == 103)
                    {
                        name = "Nightmare Pickaxe";
                        useStyle = 1;
                        useTurn = true;
                        useAnimation = 20;
                        useTime = 15;
                        autoReuse = true;
                        width = 24;
                        height = 28;
                        damage = 11;
                        pick = 65;
                        useSound = 1;
                        knockBack = 3.0F;
                        rare = 1;
                        value = 18000;
                        scale = 1.15F;
                    }
                    else
                    {
                        if (type == 104)
                        {
                            name = "The Breaker";
                            autoReuse = true;
                            useStyle = 1;
                            useAnimation = 40;
                            useTime = 19;
                            hammer = 55;
                            width = 24;
                            height = 28;
                            damage = 28;
                            knockBack = 6.5F;
                            scale = 1.3F;
                            useSound = 1;
                            rare = 1;
                            value = 15000;
                        }
                        else
                        {
                            if (type == 105)
                            {
                                name = "Candle";
                                useStyle = 1;
                                useTurn = true;
                                useAnimation = 15;
                                useTime = 10;
                                autoReuse = true;
                                maxStack = 99;
                                consumable = true;
                                createTile = 33;
                                width = 8;
                                height = 18;
                                holdStyle = 1;
                            }
                            else
                            {
                                if (type == 106)
                                {
                                    name = "Copper Chandelier";
                                    useStyle = 1;
                                    useTurn = true;
                                    useAnimation = 15;
                                    useTime = 10;
                                    autoReuse = true;
                                    maxStack = 99;
                                    consumable = true;
                                    createTile = 34;
                                    width = 26;
                                    height = 26;
                                }
                                else
                                {
                                    if (type == 107)
                                    {
                                        name = "Silver Chandelier";
                                        useStyle = 1;
                                        useTurn = true;
                                        useAnimation = 15;
                                        useTime = 10;
                                        autoReuse = true;
                                        maxStack = 99;
                                        consumable = true;
                                        createTile = 35;
                                        width = 26;
                                        height = 26;
                                    }
                                    else
                                    {
                                        if (type == 108)
                                        {
                                            name = "Gold Chandelier";
                                            useStyle = 1;
                                            useTurn = true;
                                            useAnimation = 15;
                                            useTime = 10;
                                            autoReuse = true;
                                            maxStack = 99;
                                            consumable = true;
                                            createTile = 36;
                                            width = 26;
                                            height = 26;
                                        }
                                        else
                                        {
                                            if (type == 109)
                                            {
                                                name = "Mana Crystal";
                                                maxStack = 99;
                                                consumable = true;
                                                width = 18;
                                                height = 18;
                                                useStyle = 4;
                                                useTime = 30;
                                                useSound = 4;
                                                useAnimation = 30;
                                                toolTip = "Increases maximum mana";
                                                rare = 2;
                                            }
                                            else
                                            {
                                                if (type == 110)
                                                {
                                                    name = "Lesser Mana Potion";
                                                    useSound = 3;
                                                    healMana = 100;
                                                    useStyle = 2;
                                                    useTurn = true;
                                                    useAnimation = 17;
                                                    useTime = 17;
                                                    maxStack = 30;
                                                    consumable = true;
                                                    width = 14;
                                                    height = 24;
                                                    potion = true;
                                                    value = 1000;
                                                }
                                                else
                                                {
                                                    if (type == 111)
                                                    {
                                                        name = "Band of Starpower";
                                                        width = 22;
                                                        height = 22;
                                                        accessory = true;
                                                        manaRegen = 3;
                                                        rare = 1;
                                                        toolTip = "Slowly regenerates mana";
                                                        value = 50000;
                                                    }
                                                    else
                                                    {
                                                        if (type == 112)
                                                        {
                                                            mana = 10;
                                                            damage = 30;
                                                            useStyle = 1;
                                                            name = "Flower of Fire";
                                                            shootSpeed = 6.0F;
                                                            shoot = 15;
                                                            width = 26;
                                                            height = 28;
                                                            useSound = 8;
                                                            useAnimation = 30;
                                                            useTime = 30;
                                                            rare = 3;
                                                            noMelee = true;
                                                            knockBack = 5.0F;
                                                            toolTip = "Throws balls of fire";
                                                            value = 10000;
                                                        }
                                                        else
                                                        {
                                                            if (type == 113)
                                                            {
                                                                mana = 18;
                                                                channel = true;
                                                                damage = 30;
                                                                useStyle = 1;
                                                                name = "Magic Missile";
                                                                shootSpeed = 6.0F;
                                                                shoot = 16;
                                                                width = 26;
                                                                height = 28;
                                                                useSound = 9;
                                                                useAnimation = 20;
                                                                useTime = 20;
                                                                rare = 2;
                                                                noMelee = true;
                                                                knockBack = 5.0F;
                                                                toolTip = "Casts a controllable missile";
                                                                value = 10000;
                                                            }
                                                            else
                                                            {
                                                                if (type == 114)
                                                                {
                                                                    mana = 5;
                                                                    channel = true;
                                                                    damage = 0;
                                                                    useStyle = 1;
                                                                    name = "Dirt Rod";
                                                                    shoot = 17;
                                                                    width = 26;
                                                                    height = 28;
                                                                    useSound = 8;
                                                                    useAnimation = 20;
                                                                    useTime = 20;
                                                                    rare = 1;
                                                                    noMelee = true;
                                                                    knockBack = 5.0F;
                                                                    toolTip = "Magically move dirt";
                                                                    value = 200000;
                                                                }
                                                                else
                                                                {
                                                                    if (type == 115)
                                                                    {
                                                                        mana = 40;
                                                                        channel = true;
                                                                        damage = 0;
                                                                        useStyle = 4;
                                                                        name = "Orb of Light";
                                                                        shoot = 18;
                                                                        width = 24;
                                                                        height = 24;
                                                                        useSound = 8;
                                                                        useAnimation = 20;
                                                                        useTime = 20;
                                                                        rare = 1;
                                                                        noMelee = true;
                                                                        toolTip = "Creates a magical orb of light";
                                                                        value = 10000;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (type == 116)
                                                                        {
                                                                            name = "Meteorite";
                                                                            useStyle = 1;
                                                                            useTurn = true;
                                                                            useAnimation = 15;
                                                                            useTime = 10;
                                                                            autoReuse = true;
                                                                            maxStack = 250;
                                                                            consumable = true;
                                                                            createTile = 37;
                                                                            width = 12;
                                                                            height = 12;
                                                                            value = 1000;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (type == 117)
                                                                            {
                                                                                name = "Meteorite Bar";
                                                                                width = 20;
                                                                                height = 20;
                                                                                maxStack = 99;
                                                                                rare = 1;
                                                                                toolTip = "Warm to the touch";
                                                                                value = 7000;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (type == 118)
                                                                                {
                                                                                    name = "Hook";
                                                                                    maxStack = 99;
                                                                                    width = 18;
                                                                                    height = 18;
                                                                                    value = 1000;
                                                                                    toolTip = "Combine with chains to making a grappling hook";
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (type == 119)
                                                                                    {
                                                                                        noMelee = true;
                                                                                        useStyle = 1;
                                                                                        name = "Flamarang";
                                                                                        shootSpeed = 11.0F;
                                                                                        shoot = 19;
                                                                                        damage = 32;
                                                                                        knockBack = 8.0F;
                                                                                        width = 14;
                                                                                        height = 28;
                                                                                        useSound = 1;
                                                                                        useAnimation = 15;
                                                                                        useTime = 15;
                                                                                        noUseGraphic = true;
                                                                                        rare = 3;
                                                                                        value = 100000;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (type == 120)
                                                                                        {
                                                                                            useStyle = 5;
                                                                                            useAnimation = 25;
                                                                                            useTime = 25;
                                                                                            name = "Molten Fury";
                                                                                            width = 14;
                                                                                            height = 32;
                                                                                            shoot = 1;
                                                                                            useAmmo = 1;
                                                                                            useSound = 5;
                                                                                            damage = 29;
                                                                                            shootSpeed = 8.0F;
                                                                                            knockBack = 2.0F;
                                                                                            alpha = 30;
                                                                                            rare = 3;
                                                                                            noMelee = true;
                                                                                            scale = 1.1F;
                                                                                            value = 27000;
                                                                                            toolTip = "Lights wooden arrows ablaze";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (type == 121)
                                                                                            {
                                                                                                name = "Fiery Greatsword";
                                                                                                useStyle = 1;
                                                                                                useAnimation = 35;
                                                                                                knockBack = 6.5F;
                                                                                                width = 24;
                                                                                                height = 28;
                                                                                                damage = 34;
                                                                                                scale = 1.3F;
                                                                                                useSound = 1;
                                                                                                rare = 3;
                                                                                                value = 27000;
                                                                                                toolTip = "It's made out of fire!";
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
            if (type == 122)
            {
                name = "Molten Pickaxe";
                useStyle = 1;
                useTurn = true;
                useAnimation = 25;
                useTime = 25;
                autoReuse = true;
                width = 24;
                height = 28;
                damage = 18;
                pick = 100;
                scale = 1.15F;
                useSound = 1;
                knockBack = 2.0F;
                rare = 3;
                value = 27000;
            }
            else
            {
                if (type == 123)
                {
                    name = "Meteor Helmet";
                    width = 22;
                    height = 22;
                    defense = 4;
                    headSlot = 6;
                    rare = 1;
                    value = 45000;
                    manaRegen = 3;
                    toolTip = "Slowly regenerates mana";
                }
                else
                {
                    if (type == 124)
                    {
                        name = "Meteor Suit";
                        width = 26;
                        height = 28;
                        defense = 5;
                        bodySlot = 6;
                        rare = 1;
                        value = 30000;
                        manaRegen = 3;
                        toolTip = "Slowly regenerates mana";
                    }
                    else
                    {
                        if (type == 125)
                        {
                            name = "Meteor Leggings";
                            width = 18;
                            height = 28;
                            defense = 4;
                            legSlot = 6;
                            rare = 1;
                            manaRegen = 3;
                            value = 30000;
                            toolTip = "Slowly regenerates mana";
                        }
                        else
                        {
                            if (type == 126)
                            {
                                name = "Angel Statue";
                                width = 24;
                                height = 28;
                                toolTip = "It doesn't do anything";
                                value = 1;
                            }
                            else
                            {
                                if (type == 127)
                                {
                                    autoReuse = true;
                                    useStyle = 5;
                                    useAnimation = 18;
                                    useTime = 18;
                                    name = "Space Gun";
                                    width = 24;
                                    height = 28;
                                    shoot = 20;
                                    mana = 9;
                                    useSound = 12;
                                    knockBack = 1.0F;
                                    damage = 15;
                                    shootSpeed = 10.0F;
                                    noMelee = true;
                                    scale = 0.8F;
                                    rare = 1;
                                }
                                else
                                {
                                    if (type == 128)
                                    {
                                        mana = 7;
                                        name = "Rocket Boots";
                                        width = 28;
                                        height = 24;
                                        accessory = true;
                                        rare = 3;
                                        toolTip = "Allows flight";
                                        value = 50000;
                                    }
                                    else
                                    {
                                        if (type == 129)
                                        {
                                            name = "Gray Brick";
                                            useStyle = 1;
                                            useTurn = true;
                                            useAnimation = 15;
                                            useTime = 10;
                                            autoReuse = true;
                                            maxStack = 250;
                                            consumable = true;
                                            createTile = 38;
                                            width = 12;
                                            height = 12;
                                        }
                                        else
                                        {
                                            if (type == 130)
                                            {
                                                name = "Gray Brick Wall";
                                                useStyle = 1;
                                                useTurn = true;
                                                useAnimation = 15;
                                                useTime = 10;
                                                autoReuse = true;
                                                maxStack = 250;
                                                consumable = true;
                                                createWall = 5;
                                                width = 12;
                                                height = 12;
                                            }
                                            else
                                            {
                                                if (type == 131)
                                                {
                                                    name = "Red Brick";
                                                    useStyle = 1;
                                                    useTurn = true;
                                                    useAnimation = 15;
                                                    useTime = 10;
                                                    autoReuse = true;
                                                    maxStack = 250;
                                                    consumable = true;
                                                    createTile = 39;
                                                    width = 12;
                                                    height = 12;
                                                }
                                                else
                                                {
                                                    if (type == 132)
                                                    {
                                                        name = "Red Brick Wall";
                                                        useStyle = 1;
                                                        useTurn = true;
                                                        useAnimation = 15;
                                                        useTime = 10;
                                                        autoReuse = true;
                                                        maxStack = 250;
                                                        consumable = true;
                                                        createWall = 6;
                                                        width = 12;
                                                        height = 12;
                                                    }
                                                    else
                                                    {
                                                        if (type == 133)
                                                        {
                                                            name = "Clay Block";
                                                            useStyle = 1;
                                                            useTurn = true;
                                                            useAnimation = 15;
                                                            useTime = 10;
                                                            autoReuse = true;
                                                            maxStack = 250;
                                                            consumable = true;
                                                            createTile = 40;
                                                            width = 12;
                                                            height = 12;
                                                        }
                                                        else
                                                        {
                                                            if (type == 134)
                                                            {
                                                                name = "Blue Brick";
                                                                useStyle = 1;
                                                                useTurn = true;
                                                                useAnimation = 15;
                                                                useTime = 10;
                                                                autoReuse = true;
                                                                maxStack = 250;
                                                                consumable = true;
                                                                createTile = 41;
                                                                width = 12;
                                                                height = 12;
                                                            }
                                                            else
                                                            {
                                                                if (type == 135)
                                                                {
                                                                    name = "Blue Brick Wall";
                                                                    useStyle = 1;
                                                                    useTurn = true;
                                                                    useAnimation = 15;
                                                                    useTime = 10;
                                                                    autoReuse = true;
                                                                    maxStack = 250;
                                                                    consumable = true;
                                                                    createWall = 7;
                                                                    width = 12;
                                                                    height = 12;
                                                                }
                                                                else
                                                                {
                                                                    if (type == 136)
                                                                    {
                                                                        name = "Chain Lantern";
                                                                        useStyle = 1;
                                                                        useTurn = true;
                                                                        useAnimation = 15;
                                                                        useTime = 10;
                                                                        autoReuse = true;
                                                                        maxStack = 250;
                                                                        consumable = true;
                                                                        createTile = 42;
                                                                        width = 12;
                                                                        height = 28;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (type == 137)
                                                                        {
                                                                            name = "Green Brick";
                                                                            useStyle = 1;
                                                                            useTurn = true;
                                                                            useAnimation = 15;
                                                                            useTime = 10;
                                                                            autoReuse = true;
                                                                            maxStack = 250;
                                                                            consumable = true;
                                                                            createTile = 43;
                                                                            width = 12;
                                                                            height = 12;
                                                                        }
                                                                        else
                                                                        {
                                                                            if (type == 138)
                                                                            {
                                                                                name = "Green Brick Wall";
                                                                                useStyle = 1;
                                                                                useTurn = true;
                                                                                useAnimation = 15;
                                                                                useTime = 10;
                                                                                autoReuse = true;
                                                                                maxStack = 250;
                                                                                consumable = true;
                                                                                createWall = 8;
                                                                                width = 12;
                                                                                height = 12;
                                                                            }
                                                                            else
                                                                            {
                                                                                if (type == 139)
                                                                                {
                                                                                    name = "Pink Brick";
                                                                                    useStyle = 1;
                                                                                    useTurn = true;
                                                                                    useAnimation = 15;
                                                                                    useTime = 10;
                                                                                    autoReuse = true;
                                                                                    maxStack = 250;
                                                                                    consumable = true;
                                                                                    createTile = 44;
                                                                                    width = 12;
                                                                                    height = 12;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (type == 140)
                                                                                    {
                                                                                        name = "Pink Brick Wall";
                                                                                        useStyle = 1;
                                                                                        useTurn = true;
                                                                                        useAnimation = 15;
                                                                                        useTime = 10;
                                                                                        autoReuse = true;
                                                                                        maxStack = 250;
                                                                                        consumable = true;
                                                                                        createWall = 9;
                                                                                        width = 12;
                                                                                        height = 12;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (type == 141)
                                                                                        {
                                                                                            name = "Gold Brick";
                                                                                            useStyle = 1;
                                                                                            useTurn = true;
                                                                                            useAnimation = 15;
                                                                                            useTime = 10;
                                                                                            autoReuse = true;
                                                                                            maxStack = 250;
                                                                                            consumable = true;
                                                                                            createTile = 45;
                                                                                            width = 12;
                                                                                            height = 12;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (type == 142)
                                                                                            {
                                                                                                name = "Gold Brick Wall";
                                                                                                useStyle = 1;
                                                                                                useTurn = true;
                                                                                                useAnimation = 15;
                                                                                                useTime = 10;
                                                                                                autoReuse = true;
                                                                                                maxStack = 250;
                                                                                                consumable = true;
                                                                                                createWall = 10;
                                                                                                width = 12;
                                                                                                height = 12;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (type == 143)
                                                                                                {
                                                                                                    name = "Silver Brick";
                                                                                                    useStyle = 1;
                                                                                                    useTurn = true;
                                                                                                    useAnimation = 15;
                                                                                                    useTime = 10;
                                                                                                    autoReuse = true;
                                                                                                    maxStack = 250;
                                                                                                    consumable = true;
                                                                                                    createTile = 46;
                                                                                                    width = 12;
                                                                                                    height = 12;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (type == 144)
                                                                                                    {
                                                                                                        name = "Silver Brick Wall";
                                                                                                        useStyle = 1;
                                                                                                        useTurn = true;
                                                                                                        useAnimation = 15;
                                                                                                        useTime = 10;
                                                                                                        autoReuse = true;
                                                                                                        maxStack = 250;
                                                                                                        consumable = true;
                                                                                                        createWall = 11;
                                                                                                        width = 12;
                                                                                                        height = 12;
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (type == 145)
                                                                                                        {
                                                                                                            name = "Copper Brick";
                                                                                                            useStyle = 1;
                                                                                                            useTurn = true;
                                                                                                            useAnimation = 15;
                                                                                                            useTime = 10;
                                                                                                            autoReuse = true;
                                                                                                            maxStack = 250;
                                                                                                            consumable = true;
                                                                                                            createTile = 47;
                                                                                                            width = 12;
                                                                                                            height = 12;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (type == 146)
                                                                                                            {
                                                                                                                name = "Copper Brick Wall";
                                                                                                                useStyle = 1;
                                                                                                                useTurn = true;
                                                                                                                useAnimation = 15;
                                                                                                                useTime = 10;
                                                                                                                autoReuse = true;
                                                                                                                maxStack = 250;
                                                                                                                consumable = true;
                                                                                                                createWall = 12;
                                                                                                                width = 12;
                                                                                                                height = 12;
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (type == 147)
                                                                                                                {
                                                                                                                    name = "Spike";
                                                                                                                    useStyle = 1;
                                                                                                                    useTurn = true;
                                                                                                                    useAnimation = 15;
                                                                                                                    useTime = 10;
                                                                                                                    autoReuse = true;
                                                                                                                    maxStack = 250;
                                                                                                                    consumable = true;
                                                                                                                    createTile = 48;
                                                                                                                    width = 12;
                                                                                                                    height = 12;
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (type == 148)
                                                                                                                    {
                                                                                                                        name = "Water Candle";
                                                                                                                        useStyle = 1;
                                                                                                                        useTurn = true;
                                                                                                                        useAnimation = 15;
                                                                                                                        useTime = 10;
                                                                                                                        autoReuse = true;
                                                                                                                        maxStack = 99;
                                                                                                                        consumable = true;
                                                                                                                        createTile = 49;
                                                                                                                        width = 8;
                                                                                                                        height = 18;
                                                                                                                        holdStyle = 1;
                                                                                                                        toolTip = "Holding this may attract unwanted attention";
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (type == 149)
                                                                                                                        {
                                                                                                                            name = "Book";
                                                                                                                            useStyle = 1;
                                                                                                                            useTurn = true;
                                                                                                                            useAnimation = 15;
                                                                                                                            useTime = 10;
                                                                                                                            autoReuse = true;
                                                                                                                            maxStack = 99;
                                                                                                                            consumable = true;
                                                                                                                            createTile = 50;
                                                                                                                            width = 24;
                                                                                                                            height = 28;
                                                                                                                            toolTip = "It contains strange symbols";
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (type == 150)
                                                                                                                            {
                                                                                                                                name = "Cobweb";
                                                                                                                                useStyle = 1;
                                                                                                                                useTurn = true;
                                                                                                                                useAnimation = 15;
                                                                                                                                useTime = 10;
                                                                                                                                autoReuse = true;
                                                                                                                                maxStack = 250;
                                                                                                                                consumable = true;
                                                                                                                                createTile = 51;
                                                                                                                                width = 20;
                                                                                                                                height = 24;
                                                                                                                                alpha = 100;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (type == 151)
                                                                                                                                {
                                                                                                                                    name = "Necro Helmet";
                                                                                                                                    width = 22;
                                                                                                                                    height = 22;
                                                                                                                                    defense = 6;
                                                                                                                                    headSlot = 7;
                                                                                                                                    rare = 2;
                                                                                                                                    value = 45000;
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    if (type == 152)
                                                                                                                                    {
                                                                                                                                        name = "Necro Breastplate";
                                                                                                                                        width = 26;
                                                                                                                                        height = 28;
                                                                                                                                        defense = 7;
                                                                                                                                        bodySlot = 7;
                                                                                                                                        rare = 2;
                                                                                                                                        value = 30000;
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (type == 153)
                                                                                                                                        {
                                                                                                                                            name = "Necro Greaves";
                                                                                                                                            width = 18;
                                                                                                                                            height = 28;
                                                                                                                                            defense = 6;
                                                                                                                                            legSlot = 7;
                                                                                                                                            rare = 2;
                                                                                                                                            value = 30000;
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (type == 154)
                                                                                                                                            {
                                                                                                                                                name = "Bone";
                                                                                                                                                maxStack = 99;
                                                                                                                                                consumable = true;
                                                                                                                                                width = 12;
                                                                                                                                                height = 14;
                                                                                                                                                value = 50;
                                                                                                                                                useAnimation = 12;
                                                                                                                                                useTime = 12;
                                                                                                                                                useStyle = 1;
                                                                                                                                                useSound = 1;
                                                                                                                                                shootSpeed = 8.0F;
                                                                                                                                                noUseGraphic = true;
                                                                                                                                                damage = 22;
                                                                                                                                                knockBack = 4.0F;
                                                                                                                                                shoot = 21;
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (type == 155)
                                                                                                                                                {
                                                                                                                                                    autoReuse = true;
                                                                                                                                                    useTurn = true;
                                                                                                                                                    name = "Muramasa";
                                                                                                                                                    useStyle = 1;
                                                                                                                                                    useAnimation = 20;
                                                                                                                                                    knockBack = 3.0F;
                                                                                                                                                    width = 40;
                                                                                                                                                    height = 40;
                                                                                                                                                    damage = 22;
                                                                                                                                                    scale = 1.2F;
                                                                                                                                                    useSound = 1;
                                                                                                                                                    rare = 2;
                                                                                                                                                    value = 27000;
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    if (type == 156)
                                                                                                                                                    {
                                                                                                                                                        name = "Cobalt Shield";
                                                                                                                                                        width = 24;
                                                                                                                                                        height = 28;
                                                                                                                                                        rare = 2;
                                                                                                                                                        value = 27000;
                                                                                                                                                        accessory = true;
                                                                                                                                                        defense = 2;
                                                                                                                                                        toolTip = "Grants immunity to knockback";
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        if (type == 157)
                                                                                                                                                        {
                                                                                                                                                            mana = 12;
                                                                                                                                                            autoReuse = true;
                                                                                                                                                            name = "Aqua Scepter";
                                                                                                                                                            useStyle = 5;
                                                                                                                                                            useAnimation = 30;
                                                                                                                                                            useTime = 5;
                                                                                                                                                            knockBack = 3.0F;
                                                                                                                                                            width = 38;
                                                                                                                                                            height = 10;
                                                                                                                                                            damage = 15;
                                                                                                                                                            scale = 1.0F;
                                                                                                                                                            shoot = 22;
                                                                                                                                                            shootSpeed = 10.0F;
                                                                                                                                                            useSound = 13;
                                                                                                                                                            rare = 2;
                                                                                                                                                            value = 27000;
                                                                                                                                                            toolTip = "Sprays out a shower of water";
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
                                                                                                                                                            if (type == 158)
                                                                                                                                                            {
                                                                                                                                                                name = "Lucky Horseshoe";
                                                                                                                                                                width = 20;
                                                                                                                                                                height = 22;
                                                                                                                                                                rare = 1;
                                                                                                                                                                value = 27000;
                                                                                                                                                                accessory = true;
                                                                                                                                                                toolTip = "Negate fall damage";
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                if (type == 159)
                                                                                                                                                                {
                                                                                                                                                                    name = "Shiny Red Balloon";
                                                                                                                                                                    width = 14;
                                                                                                                                                                    height = 28;
                                                                                                                                                                    rare = 1;
                                                                                                                                                                    value = 27000;
                                                                                                                                                                    accessory = true;
                                                                                                                                                                    toolTip = "Increases jump height";
                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                {
                                                                                                                                                                    if (type == 160)
                                                                                                                                                                    {
                                                                                                                                                                        autoReuse = true;
                                                                                                                                                                        name = "Harpoon";
                                                                                                                                                                        useStyle = 5;
                                                                                                                                                                        useAnimation = 30;
                                                                                                                                                                        useTime = 30;
                                                                                                                                                                        knockBack = 6.0F;
                                                                                                                                                                        width = 30;
                                                                                                                                                                        height = 10;
                                                                                                                                                                        damage = 15;
                                                                                                                                                                        scale = 1.1F;
                                                                                                                                                                        shoot = 23;
                                                                                                                                                                        shootSpeed = 10.0F;
                                                                                                                                                                        useSound = 10;
                                                                                                                                                                        rare = 2;
                                                                                                                                                                        value = 27000;
                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                    {
                                                                                                                                                                        if (type == 161)
                                                                                                                                                                        {
                                                                                                                                                                            useStyle = 1;
                                                                                                                                                                            name = "Spiky Ball";
                                                                                                                                                                            shootSpeed = 5.0F;
                                                                                                                                                                            shoot = 24;
                                                                                                                                                                            knockBack = 1.0F;
                                                                                                                                                                            damage = 12;
                                                                                                                                                                            width = 10;
                                                                                                                                                                            height = 10;
                                                                                                                                                                            maxStack = 250;
                                                                                                                                                                            consumable = true;
                                                                                                                                                                            useSound = 1;
                                                                                                                                                                            useAnimation = 15;
                                                                                                                                                                            useTime = 15;
                                                                                                                                                                            noUseGraphic = true;
                                                                                                                                                                            noMelee = true;
                                                                                                                                                                            value = 20;
                                                                                                                                                                        }
                                                                                                                                                                        else
                                                                                                                                                                        {
                                                                                                                                                                            if (type == 162)
                                                                                                                                                                            {
                                                                                                                                                                                name = "Ball 'O Hurt";
                                                                                                                                                                                useStyle = 5;
                                                                                                                                                                                useAnimation = 30;
                                                                                                                                                                                useTime = 30;
                                                                                                                                                                                knockBack = 7.0F;
                                                                                                                                                                                width = 30;
                                                                                                                                                                                height = 10;
                                                                                                                                                                                damage = 15;
                                                                                                                                                                                scale = 1.1F;
                                                                                                                                                                                noUseGraphic = true;
                                                                                                                                                                                shoot = 25;
                                                                                                                                                                                shootSpeed = 12.0F;
                                                                                                                                                                                useSound = 1;
                                                                                                                                                                                rare = 1;
                                                                                                                                                                                value = 27000;
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                if (type == 163)
                                                                                                                                                                                {
                                                                                                                                                                                    name = "Blue Moon";
                                                                                                                                                                                    useStyle = 5;
                                                                                                                                                                                    useAnimation = 30;
                                                                                                                                                                                    useTime = 30;
                                                                                                                                                                                    knockBack = 7.0F;
                                                                                                                                                                                    width = 30;
                                                                                                                                                                                    height = 10;
                                                                                                                                                                                    damage = 30;
                                                                                                                                                                                    scale = 1.1F;
                                                                                                                                                                                    noUseGraphic = true;
                                                                                                                                                                                    shoot = 26;
                                                                                                                                                                                    shootSpeed = 12.0F;
                                                                                                                                                                                    useSound = 1;
                                                                                                                                                                                    rare = 2;
                                                                                                                                                                                    value = 27000;
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    if (type == 164)
                                                                                                                                                                                    {
                                                                                                                                                                                        autoReuse = false;
                                                                                                                                                                                        useStyle = 5;
                                                                                                                                                                                        useAnimation = 10;
                                                                                                                                                                                        useTime = 10;
                                                                                                                                                                                        name = "Handgun";
                                                                                                                                                                                        width = 24;
                                                                                                                                                                                        height = 28;
                                                                                                                                                                                        shoot = 14;
                                                                                                                                                                                        knockBack = 3.0F;
                                                                                                                                                                                        useAmmo = 14;
                                                                                                                                                                                        useSound = 11;
                                                                                                                                                                                        damage = 12;
                                                                                                                                                                                        shootSpeed = 10.0F;
                                                                                                                                                                                        noMelee = true;
                                                                                                                                                                                        value = 50000;
                                                                                                                                                                                        scale = 0.8F;
                                                                                                                                                                                        rare = 2;
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        if (type == 165)
                                                                                                                                                                                        {
                                                                                                                                                                                            rare = 2;
                                                                                                                                                                                            mana = 20;
                                                                                                                                                                                            useSound = 8;
                                                                                                                                                                                            name = "Water Bolt";
                                                                                                                                                                                            useStyle = 5;
                                                                                                                                                                                            damage = 15;
                                                                                                                                                                                            useAnimation = 20;
                                                                                                                                                                                            useTime = 20;
                                                                                                                                                                                            width = 24;
                                                                                                                                                                                            height = 28;
                                                                                                                                                                                            shoot = 27;
                                                                                                                                                                                            scale = 0.8F;
                                                                                                                                                                                            shootSpeed = 4.0F;
                                                                                                                                                                                            knockBack = 5.0F;
                                                                                                                                                                                            toolTip = "Casts a slow moving bolt of water";
                                                                                                                                                                                        }
                                                                                                                                                                                        else
                                                                                                                                                                                        {
                                                                                                                                                                                            if (type == 166)
                                                                                                                                                                                            {
                                                                                                                                                                                                useStyle = 1;
                                                                                                                                                                                                name = "Bomb";
                                                                                                                                                                                                shootSpeed = 5.0F;
                                                                                                                                                                                                shoot = 28;
                                                                                                                                                                                                width = 20;
                                                                                                                                                                                                height = 20;
                                                                                                                                                                                                maxStack = 20;
                                                                                                                                                                                                consumable = true;
                                                                                                                                                                                                useSound = 1;
                                                                                                                                                                                                useAnimation = 25;
                                                                                                                                                                                                useTime = 25;
                                                                                                                                                                                                noUseGraphic = true;
                                                                                                                                                                                                noMelee = true;
                                                                                                                                                                                                value = 500;
                                                                                                                                                                                                damage = 0;
                                                                                                                                                                                                toolTip = "A small explosion that will destroy some tiles";
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                if (type == 167)
                                                                                                                                                                                                {
                                                                                                                                                                                                    useStyle = 1;
                                                                                                                                                                                                    name = "Dynamite";
                                                                                                                                                                                                    shootSpeed = 4.0F;
                                                                                                                                                                                                    shoot = 29;
                                                                                                                                                                                                    width = 8;
                                                                                                                                                                                                    height = 28;
                                                                                                                                                                                                    maxStack = 3;
                                                                                                                                                                                                    consumable = true;
                                                                                                                                                                                                    useSound = 1;
                                                                                                                                                                                                    useAnimation = 40;
                                                                                                                                                                                                    useTime = 40;
                                                                                                                                                                                                    noUseGraphic = true;
                                                                                                                                                                                                    noMelee = true;
                                                                                                                                                                                                    value = 5000;
                                                                                                                                                                                                    rare = 1;
                                                                                                                                                                                                    toolTip = "A large explosion that will destroy most tiles";
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (type == 168)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        useStyle = 1;
                                                                                                                                                                                                        name = "Grenade";
                                                                                                                                                                                                        shootSpeed = 5.5F;
                                                                                                                                                                                                        shoot = 30;
                                                                                                                                                                                                        width = 20;
                                                                                                                                                                                                        height = 20;
                                                                                                                                                                                                        maxStack = 20;
                                                                                                                                                                                                        consumable = true;
                                                                                                                                                                                                        useSound = 1;
                                                                                                                                                                                                        useAnimation = 60;
                                                                                                                                                                                                        useTime = 60;
                                                                                                                                                                                                        noUseGraphic = true;
                                                                                                                                                                                                        noMelee = true;
                                                                                                                                                                                                        value = 500;
                                                                                                                                                                                                        damage = 60;
                                                                                                                                                                                                        knockBack = 8.0F;
                                                                                                                                                                                                        toolTip = "A small explosion that will not destroy tiles";
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else
                                                                                                                                                                                                    {
                                                                                                                                                                                                        if (type == 169)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            name = "Sand Block";
                                                                                                                                                                                                            useStyle = 1;
                                                                                                                                                                                                            useTurn = true;
                                                                                                                                                                                                            useAnimation = 15;
                                                                                                                                                                                                            useTime = 10;
                                                                                                                                                                                                            autoReuse = true;
                                                                                                                                                                                                            maxStack = 250;
                                                                                                                                                                                                            consumable = true;
                                                                                                                                                                                                            createTile = 53;
                                                                                                                                                                                                            width = 12;
                                                                                                                                                                                                            height = 12;
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else
                                                                                                                                                                                                        {
                                                                                                                                                                                                            if (type == 170)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                name = "Glass";
                                                                                                                                                                                                                useStyle = 1;
                                                                                                                                                                                                                useTurn = true;
                                                                                                                                                                                                                useAnimation = 15;
                                                                                                                                                                                                                useTime = 10;
                                                                                                                                                                                                                autoReuse = true;
                                                                                                                                                                                                                maxStack = 250;
                                                                                                                                                                                                                consumable = true;
                                                                                                                                                                                                                createTile = 54;
                                                                                                                                                                                                                width = 12;
                                                                                                                                                                                                                height = 12;
                                                                                                                                                                                                            }
                                                                                                                                                                                                            else
                                                                                                                                                                                                            {
                                                                                                                                                                                                                if (type == 171)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    name = "Sign";
                                                                                                                                                                                                                    useStyle = 1;
                                                                                                                                                                                                                    useTurn = true;
                                                                                                                                                                                                                    useAnimation = 15;
                                                                                                                                                                                                                    useTime = 10;
                                                                                                                                                                                                                    autoReuse = true;
                                                                                                                                                                                                                    maxStack = 250;
                                                                                                                                                                                                                    consumable = true;
                                                                                                                                                                                                                    createTile = 55;
                                                                                                                                                                                                                    width = 28;
                                                                                                                                                                                                                    height = 28;
                                                                                                                                                                                                                }
                                                                                                                                                                                                                else
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    if (type == 172)
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        name = "Ash Block";
                                                                                                                                                                                                                        useStyle = 1;
                                                                                                                                                                                                                        useTurn = true;
                                                                                                                                                                                                                        useAnimation = 15;
                                                                                                                                                                                                                        useTime = 10;
                                                                                                                                                                                                                        autoReuse = true;
                                                                                                                                                                                                                        maxStack = 250;
                                                                                                                                                                                                                        consumable = true;
                                                                                                                                                                                                                        createTile = 57;
                                                                                                                                                                                                                        width = 12;
                                                                                                                                                                                                                        height = 12;
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    else
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        if (type == 173)
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            name = "Obsidian";
                                                                                                                                                                                                                            useStyle = 1;
                                                                                                                                                                                                                            useTurn = true;
                                                                                                                                                                                                                            useAnimation = 15;
                                                                                                                                                                                                                            useTime = 10;
                                                                                                                                                                                                                            autoReuse = true;
                                                                                                                                                                                                                            maxStack = 250;
                                                                                                                                                                                                                            consumable = true;
                                                                                                                                                                                                                            createTile = 56;
                                                                                                                                                                                                                            width = 12;
                                                                                                                                                                                                                            height = 12;
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        else
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            if (type == 174)
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                name = "Hellstone";
                                                                                                                                                                                                                                useStyle = 1;
                                                                                                                                                                                                                                useTurn = true;
                                                                                                                                                                                                                                useAnimation = 15;
                                                                                                                                                                                                                                useTime = 10;
                                                                                                                                                                                                                                autoReuse = true;
                                                                                                                                                                                                                                maxStack = 250;
                                                                                                                                                                                                                                consumable = true;
                                                                                                                                                                                                                                createTile = 58;
                                                                                                                                                                                                                                width = 12;
                                                                                                                                                                                                                                height = 12;
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            else
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                if (type == 175)
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    name = "Hellstone Bar";
                                                                                                                                                                                                                                    width = 20;
                                                                                                                                                                                                                                    height = 20;
                                                                                                                                                                                                                                    maxStack = 99;
                                                                                                                                                                                                                                    rare = 2;
                                                                                                                                                                                                                                    toolTip = "Hot to the touch";
                                                                                                                                                                                                                                    value = 20000;
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    if (type == 176)
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        name = "Mud Block";
                                                                                                                                                                                                                                        useStyle = 1;
                                                                                                                                                                                                                                        useTurn = true;
                                                                                                                                                                                                                                        useAnimation = 15;
                                                                                                                                                                                                                                        useTime = 10;
                                                                                                                                                                                                                                        autoReuse = true;
                                                                                                                                                                                                                                        maxStack = 250;
                                                                                                                                                                                                                                        consumable = true;
                                                                                                                                                                                                                                        createTile = 59;
                                                                                                                                                                                                                                        width = 12;
                                                                                                                                                                                                                                        height = 12;
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        if (type == 177)
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            name = "Sapphire";
                                                                                                                                                                                                                                            maxStack = 99;
                                                                                                                                                                                                                                            alpha = 50;
                                                                                                                                                                                                                                            width = 10;
                                                                                                                                                                                                                                            height = 14;
                                                                                                                                                                                                                                            value = 7000;
                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            if (type == 178)
                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                name = "Ruby";
                                                                                                                                                                                                                                                maxStack = 99;
                                                                                                                                                                                                                                                alpha = 50;
                                                                                                                                                                                                                                                width = 10;
                                                                                                                                                                                                                                                height = 14;
                                                                                                                                                                                                                                                value = 20000;
                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                if (type == 179)
                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                    name = "Emerald";
                                                                                                                                                                                                                                                    maxStack = 99;
                                                                                                                                                                                                                                                    alpha = 50;
                                                                                                                                                                                                                                                    width = 10;
                                                                                                                                                                                                                                                    height = 14;
                                                                                                                                                                                                                                                    value = 15000;
                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                    if (type == 180)
                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                        name = "Topaz";
                                                                                                                                                                                                                                                        maxStack = 99;
                                                                                                                                                                                                                                                        alpha = 50;
                                                                                                                                                                                                                                                        width = 10;
                                                                                                                                                                                                                                                        height = 14;
                                                                                                                                                                                                                                                        value = 5000;
                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                        if (type == 181)
                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                            name = "Amethyst";
                                                                                                                                                                                                                                                            maxStack = 99;
                                                                                                                                                                                                                                                            alpha = 50;
                                                                                                                                                                                                                                                            width = 10;
                                                                                                                                                                                                                                                            height = 14;
                                                                                                                                                                                                                                                            value = 2500;
                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                            if (type == 182)
                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                name = "Diamond";
                                                                                                                                                                                                                                                                maxStack = 99;
                                                                                                                                                                                                                                                                alpha = 50;
                                                                                                                                                                                                                                                                width = 10;
                                                                                                                                                                                                                                                                height = 14;
                                                                                                                                                                                                                                                                value = 40000;
                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                if (type == 183)
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    name = "Glowing Mushroom";
                                                                                                                                                                                                                                                                    useStyle = 2;
                                                                                                                                                                                                                                                                    useSound = 2;
                                                                                                                                                                                                                                                                    useTurn = false;
                                                                                                                                                                                                                                                                    useAnimation = 17;
                                                                                                                                                                                                                                                                    useTime = 17;
                                                                                                                                                                                                                                                                    width = 16;
                                                                                                                                                                                                                                                                    height = 18;
                                                                                                                                                                                                                                                                    healLife = 50;
                                                                                                                                                                                                                                                                    maxStack = 99;
                                                                                                                                                                                                                                                                    consumable = true;
                                                                                                                                                                                                                                                                    potion = true;
                                                                                                                                                                                                                                                                    value = 50;
                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    if (type == 184)
                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                        name = "Star";
                                                                                                                                                                                                                                                                        width = 12;
                                                                                                                                                                                                                                                                        height = 12;
                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                        if (type == 185)
                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                            noUseGraphic = true;
                                                                                                                                                                                                                                                                            damage = 0;
                                                                                                                                                                                                                                                                            knockBack = 7.0F;
                                                                                                                                                                                                                                                                            useStyle = 5;
                                                                                                                                                                                                                                                                            name = "Ivy Whip";
                                                                                                                                                                                                                                                                            shootSpeed = 13.0F;
                                                                                                                                                                                                                                                                            shoot = 32;
                                                                                                                                                                                                                                                                            width = 18;
                                                                                                                                                                                                                                                                            height = 28;
                                                                                                                                                                                                                                                                            useSound = 1;
                                                                                                                                                                                                                                                                            useAnimation = 20;
                                                                                                                                                                                                                                                                            useTime = 20;
                                                                                                                                                                                                                                                                            rare = 3;
                                                                                                                                                                                                                                                                            noMelee = true;
                                                                                                                                                                                                                                                                            value = 20000;
                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                            if (type == 186)
                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                name = "Breathing Reed";
                                                                                                                                                                                                                                                                                width = 44;
                                                                                                                                                                                                                                                                                height = 44;
                                                                                                                                                                                                                                                                                rare = 1;
                                                                                                                                                                                                                                                                                value = 10000;
                                                                                                                                                                                                                                                                                holdStyle = 2;
                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                if (type == 187)
                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                    name = "Flipper";
                                                                                                                                                                                                                                                                                    width = 28;
                                                                                                                                                                                                                                                                                    height = 28;
                                                                                                                                                                                                                                                                                    rare = 1;
                                                                                                                                                                                                                                                                                    value = 10000;
                                                                                                                                                                                                                                                                                    accessory = true;
                                                                                                                                                                                                                                                                                    toolTip = "Grants the ability to swim";
                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                    if (type == 188)
                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                        name = "Healing Potion";
                                                                                                                                                                                                                                                                                        useSound = 3;
                                                                                                                                                                                                                                                                                        healLife = 200;
                                                                                                                                                                                                                                                                                        useStyle = 2;
                                                                                                                                                                                                                                                                                        useTurn = true;
                                                                                                                                                                                                                                                                                        useAnimation = 17;
                                                                                                                                                                                                                                                                                        useTime = 17;
                                                                                                                                                                                                                                                                                        maxStack = 30;
                                                                                                                                                                                                                                                                                        consumable = true;
                                                                                                                                                                                                                                                                                        width = 14;
                                                                                                                                                                                                                                                                                        height = 24;
                                                                                                                                                                                                                                                                                        rare = 1;
                                                                                                                                                                                                                                                                                        potion = true;
                                                                                                                                                                                                                                                                                        value = 1000;
                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                        if (type == 189)
                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                            name = "Mana Potion";
                                                                                                                                                                                                                                                                                            useSound = 3;
                                                                                                                                                                                                                                                                                            healMana = 200;
                                                                                                                                                                                                                                                                                            useStyle = 2;
                                                                                                                                                                                                                                                                                            useTurn = true;
                                                                                                                                                                                                                                                                                            useAnimation = 17;
                                                                                                                                                                                                                                                                                            useTime = 17;
                                                                                                                                                                                                                                                                                            maxStack = 30;
                                                                                                                                                                                                                                                                                            consumable = true;
                                                                                                                                                                                                                                                                                            width = 14;
                                                                                                                                                                                                                                                                                            height = 24;
                                                                                                                                                                                                                                                                                            rare = 1;
                                                                                                                                                                                                                                                                                            potion = true;
                                                                                                                                                                                                                                                                                            value = 1000;
                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                            if (type == 190)
                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                name = "Blade of Grass";
                                                                                                                                                                                                                                                                                                useStyle = 1;
                                                                                                                                                                                                                                                                                                useAnimation = 30;
                                                                                                                                                                                                                                                                                                knockBack = 3.0F;
                                                                                                                                                                                                                                                                                                width = 40;
                                                                                                                                                                                                                                                                                                height = 40;
                                                                                                                                                                                                                                                                                                damage = 28;
                                                                                                                                                                                                                                                                                                scale = 1.4F;
                                                                                                                                                                                                                                                                                                useSound = 1;
                                                                                                                                                                                                                                                                                                rare = 3;
                                                                                                                                                                                                                                                                                                value = 27000;
                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                if (type == 191)
                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                    noMelee = true;
                                                                                                                                                                                                                                                                                                    useStyle = 1;
                                                                                                                                                                                                                                                                                                    name = "Thorn Chakrum";
                                                                                                                                                                                                                                                                                                    shootSpeed = 11.0F;
                                                                                                                                                                                                                                                                                                    shoot = 33;
                                                                                                                                                                                                                                                                                                    damage = 25;
                                                                                                                                                                                                                                                                                                    knockBack = 8.0F;
                                                                                                                                                                                                                                                                                                    width = 14;
                                                                                                                                                                                                                                                                                                    height = 28;
                                                                                                                                                                                                                                                                                                    useSound = 1;
                                                                                                                                                                                                                                                                                                    useAnimation = 15;
                                                                                                                                                                                                                                                                                                    useTime = 15;
                                                                                                                                                                                                                                                                                                    noUseGraphic = true;
                                                                                                                                                                                                                                                                                                    rare = 3;
                                                                                                                                                                                                                                                                                                    value = 50000;
                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                    if (type == 192)
                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                        name = "Obsidian Brick";
                                                                                                                                                                                                                                                                                                        useStyle = 1;
                                                                                                                                                                                                                                                                                                        useTurn = true;
                                                                                                                                                                                                                                                                                                        useAnimation = 15;
                                                                                                                                                                                                                                                                                                        useTime = 10;
                                                                                                                                                                                                                                                                                                        autoReuse = true;
                                                                                                                                                                                                                                                                                                        maxStack = 250;
                                                                                                                                                                                                                                                                                                        consumable = true;
                                                                                                                                                                                                                                                                                                        createTile = 75;
                                                                                                                                                                                                                                                                                                        width = 12;
                                                                                                                                                                                                                                                                                                        height = 12;
                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                        if (type == 193)
                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                            name = "Obsidian Skull";
                                                                                                                                                                                                                                                                                                            width = 20;
                                                                                                                                                                                                                                                                                                            height = 22;
                                                                                                                                                                                                                                                                                                            rare = 2;
                                                                                                                                                                                                                                                                                                            value = 27000;
                                                                                                                                                                                                                                                                                                            accessory = true;
                                                                                                                                                                                                                                                                                                            defense = 2;
                                                                                                                                                                                                                                                                                                            toolTip = "Grants immunity to fire blocks";
                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                            if (type == 194)
                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                name = "Mushroom Grass Seeds";
                                                                                                                                                                                                                                                                                                                useTurn = true;
                                                                                                                                                                                                                                                                                                                useStyle = 1;
                                                                                                                                                                                                                                                                                                                useAnimation = 15;
                                                                                                                                                                                                                                                                                                                useTime = 10;
                                                                                                                                                                                                                                                                                                                maxStack = 99;
                                                                                                                                                                                                                                                                                                                consumable = true;
                                                                                                                                                                                                                                                                                                                createTile = 70;
                                                                                                                                                                                                                                                                                                                width = 14;
                                                                                                                                                                                                                                                                                                                height = 14;
                                                                                                                                                                                                                                                                                                                value = 150;
                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                if (type == 195)
                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                    name = "Jungle Grass Seeds";
                                                                                                                                                                                                                                                                                                                    useTurn = true;
                                                                                                                                                                                                                                                                                                                    useStyle = 1;
                                                                                                                                                                                                                                                                                                                    useAnimation = 15;
                                                                                                                                                                                                                                                                                                                    useTime = 10;
                                                                                                                                                                                                                                                                                                                    maxStack = 99;
                                                                                                                                                                                                                                                                                                                    consumable = true;
                                                                                                                                                                                                                                                                                                                    createTile = 60;
                                                                                                                                                                                                                                                                                                                    width = 14;
                                                                                                                                                                                                                                                                                                                    height = 14;
                                                                                                                                                                                                                                                                                                                    value = 150;
                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                    if (type == 196)
                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                        name = "Wooden Hammer";
                                                                                                                                                                                                                                                                                                                        autoReuse = true;
                                                                                                                                                                                                                                                                                                                        useStyle = 1;
                                                                                                                                                                                                                                                                                                                        useTurn = true;
                                                                                                                                                                                                                                                                                                                        useAnimation = 37;
                                                                                                                                                                                                                                                                                                                        useTime = 25;
                                                                                                                                                                                                                                                                                                                        hammer = 25;
                                                                                                                                                                                                                                                                                                                        width = 24;
                                                                                                                                                                                                                                                                                                                        height = 28;
                                                                                                                                                                                                                                                                                                                        damage = 2;
                                                                                                                                                                                                                                                                                                                        knockBack = 5.5F;
                                                                                                                                                                                                                                                                                                                        scale = 1.2F;
                                                                                                                                                                                                                                                                                                                        useSound = 1;
                                                                                                                                                                                                                                                                                                                        tileBoost = -1;
                                                                                                                                                                                                                                                                                                                        value = 50;
                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                        if (type == 197)
                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                            autoReuse = true;
                                                                                                                                                                                                                                                                                                                            useStyle = 5;
                                                                                                                                                                                                                                                                                                                            useAnimation = 12;
                                                                                                                                                                                                                                                                                                                            useTime = 12;
                                                                                                                                                                                                                                                                                                                            name = "Star Cannon";
                                                                                                                                                                                                                                                                                                                            width = 50;
                                                                                                                                                                                                                                                                                                                            height = 18;
                                                                                                                                                                                                                                                                                                                            shoot = 12;
                                                                                                                                                                                                                                                                                                                            useAmmo = 15;
                                                                                                                                                                                                                                                                                                                            useSound = 9;
                                                                                                                                                                                                                                                                                                                            damage = 75;
                                                                                                                                                                                                                                                                                                                            shootSpeed = 14.0F;
                                                                                                                                                                                                                                                                                                                            noMelee = true;
                                                                                                                                                                                                                                                                                                                            value = 500000;
                                                                                                                                                                                                                                                                                                                            rare = 2;
                                                                                                                                                                                                                                                                                                                            toolTip = "Shoots fallen stars";
                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                            if (type == 198)
                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                name = "Blue Phaseblade";
                                                                                                                                                                                                                                                                                                                                useStyle = 1;
                                                                                                                                                                                                                                                                                                                                useAnimation = 25;
                                                                                                                                                                                                                                                                                                                                knockBack = 3.0F;
                                                                                                                                                                                                                                                                                                                                width = 40;
                                                                                                                                                                                                                                                                                                                                height = 40;
                                                                                                                                                                                                                                                                                                                                damage = 21;
                                                                                                                                                                                                                                                                                                                                scale = 1.0F;
                                                                                                                                                                                                                                                                                                                                useSound = 15;
                                                                                                                                                                                                                                                                                                                                rare = 1;
                                                                                                                                                                                                                                                                                                                                value = 27000;
                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                if (type == 199)
                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                    name = "Red Phaseblade";
                                                                                                                                                                                                                                                                                                                                    useStyle = 1;
                                                                                                                                                                                                                                                                                                                                    useAnimation = 25;
                                                                                                                                                                                                                                                                                                                                    knockBack = 3.0F;
                                                                                                                                                                                                                                                                                                                                    width = 40;
                                                                                                                                                                                                                                                                                                                                    height = 40;
                                                                                                                                                                                                                                                                                                                                    damage = 21;
                                                                                                                                                                                                                                                                                                                                    scale = 1.0F;
                                                                                                                                                                                                                                                                                                                                    useSound = 15;
                                                                                                                                                                                                                                                                                                                                    rare = 1;
                                                                                                                                                                                                                                                                                                                                    value = 27000;
                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                    if (type == 200)
                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                        name = "Green Phaseblade";
                                                                                                                                                                                                                                                                                                                                        useStyle = 1;
                                                                                                                                                                                                                                                                                                                                        useAnimation = 25;
                                                                                                                                                                                                                                                                                                                                        knockBack = 3.0F;
                                                                                                                                                                                                                                                                                                                                        width = 40;
                                                                                                                                                                                                                                                                                                                                        height = 40;
                                                                                                                                                                                                                                                                                                                                        damage = 21;
                                                                                                                                                                                                                                                                                                                                        scale = 1.0F;
                                                                                                                                                                                                                                                                                                                                        useSound = 15;
                                                                                                                                                                                                                                                                                                                                        rare = 1;
                                                                                                                                                                                                                                                                                                                                        value = 27000;
                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                        if (type == 201)
                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                            name = "Purple Phaseblade";
                                                                                                                                                                                                                                                                                                                                            useStyle = 1;
                                                                                                                                                                                                                                                                                                                                            useAnimation = 25;
                                                                                                                                                                                                                                                                                                                                            knockBack = 3.0F;
                                                                                                                                                                                                                                                                                                                                            width = 40;
                                                                                                                                                                                                                                                                                                                                            height = 40;
                                                                                                                                                                                                                                                                                                                                            damage = 21;
                                                                                                                                                                                                                                                                                                                                            scale = 1.0F;
                                                                                                                                                                                                                                                                                                                                            useSound = 15;
                                                                                                                                                                                                                                                                                                                                            rare = 1;
                                                                                                                                                                                                                                                                                                                                            value = 27000;
                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                            if (type == 202)
                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                name = "White Phaseblade";
                                                                                                                                                                                                                                                                                                                                                useStyle = 1;
                                                                                                                                                                                                                                                                                                                                                useAnimation = 25;
                                                                                                                                                                                                                                                                                                                                                knockBack = 3.0F;
                                                                                                                                                                                                                                                                                                                                                width = 40;
                                                                                                                                                                                                                                                                                                                                                height = 40;
                                                                                                                                                                                                                                                                                                                                                damage = 21;
                                                                                                                                                                                                                                                                                                                                                scale = 1.0F;
                                                                                                                                                                                                                                                                                                                                                useSound = 15;
                                                                                                                                                                                                                                                                                                                                                rare = 1;
                                                                                                                                                                                                                                                                                                                                                value = 27000;
                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                if (type == 203)
                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                    name = "Yellow Phaseblade";
                                                                                                                                                                                                                                                                                                                                                    useStyle = 1;
                                                                                                                                                                                                                                                                                                                                                    useAnimation = 25;
                                                                                                                                                                                                                                                                                                                                                    knockBack = 3.0F;
                                                                                                                                                                                                                                                                                                                                                    width = 40;
                                                                                                                                                                                                                                                                                                                                                    height = 40;
                                                                                                                                                                                                                                                                                                                                                    damage = 21;
                                                                                                                                                                                                                                                                                                                                                    scale = 1.0F;
                                                                                                                                                                                                                                                                                                                                                    useSound = 15;
                                                                                                                                                                                                                                                                                                                                                    rare = 1;
                                                                                                                                                                                                                                                                                                                                                    value = 27000;
                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                    if (type == 204)
                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                        name = "Meteor Hamaxe";
                                                                                                                                                                                                                                                                                                                                                        useTurn = true;
                                                                                                                                                                                                                                                                                                                                                        autoReuse = true;
                                                                                                                                                                                                                                                                                                                                                        useStyle = 1;
                                                                                                                                                                                                                                                                                                                                                        useAnimation = 30;
                                                                                                                                                                                                                                                                                                                                                        useTime = 16;
                                                                                                                                                                                                                                                                                                                                                        hammer = 60;
                                                                                                                                                                                                                                                                                                                                                        axe = 20;
                                                                                                                                                                                                                                                                                                                                                        width = 24;
                                                                                                                                                                                                                                                                                                                                                        height = 28;
                                                                                                                                                                                                                                                                                                                                                        damage = 20;
                                                                                                                                                                                                                                                                                                                                                        knockBack = 7.0F;
                                                                                                                                                                                                                                                                                                                                                        scale = 1.2F;
                                                                                                                                                                                                                                                                                                                                                        useSound = 1;
                                                                                                                                                                                                                                                                                                                                                        rare = 1;
                                                                                                                                                                                                                                                                                                                                                        value = 15000;
                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                        if (type == 205)
                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                            name = "Empty Bucket";
                                                                                                                                                                                                                                                                                                                                                            useStyle = 1;
                                                                                                                                                                                                                                                                                                                                                            useTurn = true;
                                                                                                                                                                                                                                                                                                                                                            useAnimation = 15;
                                                                                                                                                                                                                                                                                                                                                            useTime = 10;
                                                                                                                                                                                                                                                                                                                                                            width = 20;
                                                                                                                                                                                                                                                                                                                                                            height = 20;
                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                            if (type == 206)
                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                name = "Water Bucket";
                                                                                                                                                                                                                                                                                                                                                                useStyle = 1;
                                                                                                                                                                                                                                                                                                                                                                useTurn = true;
                                                                                                                                                                                                                                                                                                                                                                useAnimation = 15;
                                                                                                                                                                                                                                                                                                                                                                useTime = 10;
                                                                                                                                                                                                                                                                                                                                                                width = 20;
                                                                                                                                                                                                                                                                                                                                                                height = 20;
                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                if (type == 207)
                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                    name = "Lava Bucket";
                                                                                                                                                                                                                                                                                                                                                                    useStyle = 1;
                                                                                                                                                                                                                                                                                                                                                                    useTurn = true;
                                                                                                                                                                                                                                                                                                                                                                    useAnimation = 15;
                                                                                                                                                                                                                                                                                                                                                                    useTime = 10;
                                                                                                                                                                                                                                                                                                                                                                    width = 20;
                                                                                                                                                                                                                                                                                                                                                                    height = 20;
                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                    if (type == 208)
                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                        name = "Jungle Rose";
                                                                                                                                                                                                                                                                                                                                                                        width = 20;
                                                                                                                                                                                                                                                                                                                                                                        height = 20;
                                                                                                                                                                                                                                                                                                                                                                        maxStack = 99;
                                                                                                                                                                                                                                                                                                                                                                        value = 100;
                                                                                                                                                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                                                                                                                                                    else
                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                        if (type == 209)
                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                            name = "Stinger";
                                                                                                                                                                                                                                                                                                                                                                            width = 16;
                                                                                                                                                                                                                                                                                                                                                                            height = 18;
                                                                                                                                                                                                                                                                                                                                                                            maxStack = 99;
                                                                                                                                                                                                                                                                                                                                                                            value = 200;
                                                                                                                                                                                                                                                                                                                                                                        }
                                                                                                                                                                                                                                                                                                                                                                        else
                                                                                                                                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                                                                                                                                            if (type == 210)
                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                name = "Vine";
                                                                                                                                                                                                                                                                                                                                                                                width = 14;
                                                                                                                                                                                                                                                                                                                                                                                height = 20;
                                                                                                                                                                                                                                                                                                                                                                                maxStack = 99;
                                                                                                                                                                                                                                                                                                                                                                                value = 1000;
                                                                                                                                                                                                                                                                                                                                                                            }
                                                                                                                                                                                                                                                                                                                                                                            else
                                                                                                                                                                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                                                                                                                                                                if (type == 211)
                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                    name = "Feral Claws";
                                                                                                                                                                                                                                                                                                                                                                                    width = 20;
                                                                                                                                                                                                                                                                                                                                                                                    height = 20;
                                                                                                                                                                                                                                                                                                                                                                                    accessory = true;
                                                                                                                                                                                                                                                                                                                                                                                    rare = 3;
                                                                                                                                                                                                                                                                                                                                                                                    toolTip = "10 % increased melee speed";
                                                                                                                                                                                                                                                                                                                                                                                    value = 50000;
                                                                                                                                                                                                                                                                                                                                                                                }
                                                                                                                                                                                                                                                                                                                                                                                else
                                                                                                                                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                                                                                                                                    if (type == 212)
                                                                                                                                                                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                                                                                                                                                                        name = "Anklet of the Wind";
                                                                                                                                                                                                                                                                                                                                                                                        width = 20;
                                                                                                                                                                                                                                                                                                                                                                                        height = 20;
                                                                                                                                                                                                                                                                                                                                                                                        accessory = true;
                                                                                                                                                                                                                                                                                                                                                                                        rare = 3;
                                                                                                                                                                                                                                                                                                                                                                                        toolTip = "10% increased movement speed";
                                                                                                                                                                                                                                                                                                                                                                                        value = 50000;
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
            if (type == 213)
            {
                name = "Staff of Regrowth";
                useStyle = 1;
                useTurn = true;
                useAnimation = 20;
                useTime = 13;
                autoReuse = true;
                width = 24;
                height = 28;
                damage = 20;
                createTile = 2;
                scale = 1.2F;
                useSound = 1;
                knockBack = 3.0F;
                rare = 3;
                value = 2000;
                toolTip = "Creates grass on dirt";
                return;
            }
            if (type == 214)
            {
                name = "Hellstone Brick";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 250;
                consumable = true;
                createTile = 76;
                width = 12;
                height = 12;
                return;
            }
            if (type == 215)
            {
                name = "Whoopie Cushion";
                width = 18;
                height = 18;
                useTurn = true;
                useTime = 30;
                useAnimation = 30;
                noUseGraphic = true;
                useStyle = 10;
                useSound = 16;
                rare = 2;
                toolTip = "May annoy others";
                value = 100;
                return;
            }
            if (type == 216)
            {
                name = "Shackle";
                width = 20;
                height = 20;
                rare = 1;
                value = 1500;
                accessory = true;
                defense = 1;
                return;
            }
            if (type == 217)
            {
                name = "Molten Hamaxe";
                useTurn = true;
                autoReuse = true;
                useStyle = 1;
                useAnimation = 27;
                useTime = 14;
                hammer = 70;
                axe = 30;
                width = 24;
                height = 28;
                damage = 20;
                knockBack = 7.0F;
                scale = 1.4F;
                useSound = 1;
                rare = 3;
                value = 15000;
                return;
            }
            if (type == 218)
            {
                mana = 20;
                channel = true;
                damage = 35;
                useStyle = 1;
                name = "Flamelash";
                shootSpeed = 6.0F;
                shoot = 34;
                width = 26;
                height = 28;
                useSound = 8;
                useAnimation = 20;
                useTime = 20;
                rare = 3;
                noMelee = true;
                knockBack = 5.0F;
                toolTip = "Summons a controllable ball of fire";
                value = 10000;
                return;
            }
            if (type == 219)
            {
                autoReuse = false;
                useStyle = 5;
                useAnimation = 10;
                useTime = 10;
                name = "Phoenix Blaster";
                width = 24;
                height = 28;
                shoot = 14;
                knockBack = 4.0F;
                useAmmo = 14;
                useSound = 11;
                damage = 28;
                shootSpeed = 13.0F;
                noMelee = true;
                value = 50000;
                scale = 0.9F;
                rare = 3;
                return;
            }
            if (type == 220)
            {
                name = "Sunfury";
                useStyle = 5;
                useAnimation = 30;
                useTime = 30;
                knockBack = 7.0F;
                width = 30;
                height = 10;
                damage = 40;
                scale = 1.1F;
                noUseGraphic = true;
                shoot = 35;
                shootSpeed = 12.0F;
                useSound = 1;
                rare = 3;
                value = 27000;
                return;
            }
            if (type == 221)
            {
                name = "Hellforge";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 77;
                width = 26;
                height = 24;
                value = 3000;
                return;
            }
            if (type == 222)
            {
                name = "Clay Pot";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                autoReuse = true;
                maxStack = 99;
                consumable = true;
                createTile = 78;
                width = 14;
                height = 14;
                value = 100;
                return;
            }
            if (type == 223)
            {
                name = "Nature's Gift";
                width = 20;
                height = 22;
                rare = 3;
                value = 27000;
                accessory = true;
                toolTip = "Spawn with max life and mana after death";
                return;
            }
            if (type == 224)
            {
                name = "Bed";
                useStyle = 1;
                useTurn = true;
                useAnimation = 15;
                useTime = 10;
                maxStack = 99;
                consumable = true;
                createTile = 79;
                width = 28;
                height = 20;
                value = 2000;
                return;
            }
            if (type == 225)
            {
                name = "Silk";
                maxStack = 99;
                width = 22;
                height = 22;
                value = 1000;
                return;
            }
            if (type == 226)
            {
                name = "Lesser Restoration Potion";
                useSound = 3;
                healMana = 100;
                healLife = 100;
                useStyle = 2;
                useTurn = true;
                useAnimation = 17;
                useTime = 17;
                maxStack = 20;
                consumable = true;
                width = 14;
                height = 24;
                potion = true;
                value = 2000;
                return;
            }
            if (type == 227)
            {
                name = "Restoration Potion";
                useSound = 3;
                healMana = 200;
                healLife = 200;
                useStyle = 2;
                useTurn = true;
                useAnimation = 17;
                useTime = 17;
                maxStack = 20;
                consumable = true;
                width = 14;
                height = 24;
                potion = true;
                value = 4000;
                return;
            }
            if (type == 228)
            {
                name = "Cobalt Helmet";
                width = 22;
                height = 22;
                defense = 6;
                headSlot = 8;
                rare = 3;
                value = 45000;
                toolTip = "Slowly regenerates mana";
                manaRegen = 4;
                return;
            }
            if (type == 229)
            {
                name = "Cobalt Breastplate";
                width = 26;
                height = 28;
                defense = 7;
                bodySlot = 8;
                rare = 3;
                value = 30000;
                toolTip = "Slowly regenerates mana";
                manaRegen = 4;
                return;
            }
            if (type == 230)
            {
                name = "Cobalt Greaves";
                width = 18;
                height = 28;
                defense = 6;
                legSlot = 8;
                rare = 3;
                value = 30000;
                toolTip = "Slowly regenerates mana";
                manaRegen = 3;
                return;
            }
            if (type == 231)
            {
                name = "Molten Helmet";
                width = 22;
                height = 22;
                defense = 9;
                headSlot = 9;
                rare = 3;
                value = 45000;
                return;
            }
            if (type == 232)
            {
                name = "Molten Breastplate";
                width = 26;
                height = 28;
                defense = 10;
                bodySlot = 9;
                rare = 3;
                value = 30000;
                return;
            }
            if (type == 233)
            {
                name = "Molten Greaves";
                width = 18;
                height = 28;
                defense = 9;
                legSlot = 9;
                rare = 3;
                value = 30000;
                return;
            }
            if (type == 234)
            {
                name = "Meteor Shot";
                shootSpeed = 3.0F;
                shoot = 36;
                damage = 9;
                width = 8;
                height = 8;
                maxStack = 250;
                consumable = true;
                ammo = 14;
                knockBack = 1.0F;
                value = 8;
                rare = 1;
                return;
            }
            if (type == 235)
            {
                useStyle = 1;
                name = "Sticky Bomb";
                shootSpeed = 5.0F;
                shoot = 37;
                width = 20;
                height = 20;
                maxStack = 20;
                consumable = true;
                useSound = 1;
                useAnimation = 25;
                useTime = 25;
                noUseGraphic = true;
                noMelee = true;
                value = 500;
                damage = 0;
                toolTip = "Tossing may be difficult.";
            }
        }

        public void UpdateItem(int i, World world)
        {
            if (this.active)
            {
                if (Statics.netMode == 0)
                {
                    this.owner = Statics.myPlayer;
                }
                float num = 0.1f;
                float num2 = 7f;
                if (this.wet)
                {
                    num2 = 5f;
                    num = 0.08f;
                }
                Vector2 value = this.velocity * 0.5f;
                if (this.ownTime > 0)
                {
                    this.ownTime--;
                }
                else
                {
                    this.ownIgnore = -1;
                }
                if (this.keepTime > 0)
                {
                    this.keepTime--;
                }
                if (!this.beingGrabbed)
                {
                    this.velocity.Y = this.velocity.Y + num;
                    if (this.velocity.Y > num2)
                    {
                        this.velocity.Y = num2;
                    }
                    this.velocity.X = this.velocity.X * 0.95f;
                    if ((double)this.velocity.X < 0.1 && (double)this.velocity.X > -0.1)
                    {
                        this.velocity.X = 0f;
                    }
                    bool flag = Collision.LavaCollision(this.position, world, this.width, this.height);
                    if (flag)
                    {
                        this.lavaWet = true;
                    }
                    if (Collision.WetCollision(this.position, this.width, this.height, world))
                    {
                        if (!this.wet)
                        {
                            if (this.wetCount == 0)
                            {
                                this.wetCount = 20;
                                if (!flag)
                                {
                                    for (int j = 0; j < 10; j++)
                                    {
                                        Color newColor = default(Color);
                                        int num3 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f)
                                            , world, this.width + 12, 24, 33, 0f, 0f, 0, newColor, 1f);
                                        Dust expr_263_cp_0 = world.getDust()[num3];
                                        expr_263_cp_0.velocity.Y = expr_263_cp_0.velocity.Y - 4f;
                                        Dust expr_281_cp_0 = world.getDust()[num3];
                                        expr_281_cp_0.velocity.X = expr_281_cp_0.velocity.X * 2.5f;
                                        world.getDust()[num3].scale = 1.3f;
                                        world.getDust()[num3].alpha = 100;
                                        world.getDust()[num3].noGravity = true;
                                    }
                                    //Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
                                }
                                else
                                {
                                    for (int k = 0; k < 5; k++)
                                    {
                                        Color newColor2 = default(Color);
                                        int num4 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), world
                                            , this.width + 12, 24, 35, 0f, 0f, 0, newColor2, 1f);
                                        Dust expr_374_cp_0 = world.getDust()[num4];
                                        expr_374_cp_0.velocity.Y = expr_374_cp_0.velocity.Y - 1.5f;
                                        Dust expr_392_cp_0 = world.getDust()[num4];
                                        expr_392_cp_0.velocity.X = expr_392_cp_0.velocity.X * 2.5f;
                                        world.getDust()[num4].scale = 1.3f;
                                        world.getDust()[num4].alpha = 100;
                                        world.getDust()[num4].noGravity = true;
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
                            Vector2 vector = this.velocity;
                            this.velocity = Collision.TileCollision(this.position, this.velocity, world, this.width, this.height, false, false);
                            if (this.velocity.X != vector.X)
                            {
                                value.X = this.velocity.X;
                            }
                            if (this.velocity.Y != vector.Y)
                            {
                                value.Y = this.velocity.Y;
                            }
                        }
                    }
                    else
                    {
                        this.velocity = Collision.TileCollision(this.position, this.velocity, world, this.width, this.height, false, false);
                    }
                    if (this.owner == Statics.myPlayer && this.lavaWet)
                    {
                        this.active = false;
                        this.type = 0;
                        this.name = "";
                        this.stack = 0;
                        if (Statics.netMode != 0)
                        {
                            NetMessage.SendData(21, world, -1, -1, "", i, 0f, 0f, 0f);
                        }
                    }
                    if (this.type == 75 && world.isDayTime())
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            Color newColor3 = default(Color);
                            Dust.NewDust(this.position, world, this.width, this.height, 15, this.velocity.X, this.velocity.Y, 150, newColor3, 1.2f);
                        }
                        for (int m = 0; m < 3; m++)
                        {
                            Gore.NewGore(this.position, new Vector2(this.velocity.X, this.velocity.Y), Statics.rand.Next(16, 18), world);
                        }
                        this.active = false;
                        this.type = 0;
                        this.stack = 0;
                        if (Statics.netMode == 2)
                        {
                            NetMessage.SendData(21, world, -1, -1, "", i, 0f, 0f, 0f);
                        }
                    }
                }
                else
                {
                    this.beingGrabbed = false;
                }
                if (this.type == 8 || this.type == 41 || (this.type == 75 || this.type == 105) || this.type == 116)
                {
                    if (!this.wet)
                    {
                        //Lighting.addLight((int)((this.position.X - 7f) / 16f), (int)((this.position.Y - 7f) / 16f), 1f);
                    }
                }
                else
                {
                    if (this.type == 183)
                    {
                        //Lighting.addLight((int)((this.position.X - 7f) / 16f), (int)((this.position.Y - 7f) / 16f), 0.5f);
                    }
                }
                if (this.type == 75)
                {
                    if (Statics.rand.Next(25) == 0)
                    {
                        Dust.NewDust(this.position, world, this.width, this.height, 15, this.velocity.X * 0.5f, this.velocity.Y * 0.5f, 150, default(Color), 1.2f);
                    }
                    if (Statics.rand.Next(50) == 0)
                    {
                        Gore.NewGore(this.position, new Vector2(this.velocity.X * 0.2f, this.velocity.Y * 0.2f), Statics.rand.Next(16, 18), world);
                    }
                }
                if (this.spawnTime < 2147483646)
                {
                    this.spawnTime++;
                }
                if (Statics.netMode == 2 && this.owner != Statics.myPlayer)
                {
                    this.release++;
                    if (this.release >= 300)
                    {
                        this.release = 0;
                        NetMessage.SendData(39, world, this.owner, -1, "", i, 0f, 0f, 0f);
                    }
                }
                if (this.wet)
                {
                    this.position += value;
                }
                else
                {
                    this.position += this.velocity;
                }
                if (this.noGrabDelay > 0)
                {
                    this.noGrabDelay--;
                }
            }
        }

        public static int NewItem(int X, int Y, World world, int Width, int Height, int Type, int Stack, bool noBroadcast)
        {
            if (WorldGen.gen)
                return 0;
            int i1 = 200;
            world.setItem(200, new Item());
            if (Statics.netMode != 1)
            {
                for (int i2 = 0; i2 < 200; i2++)
                {
                    if (!world.getItemList()[i2].active)
                    {
                        i1 = i2;
                        break;
                    }
                }
            }
            if ((i1 == 200) && (Statics.netMode != 1))
            {
                int i3 = 0;
                for (int i4 = 0; i4 < 200; i4++)
                {
                    if (world.getItemList()[i4].spawnTime > i3)
                    {
                        i3 = world.getItemList()[i4].spawnTime;
                        i1 = i4;
                    }
                }
            }
            Item item = new Item();
            world.getItemList()[i1] = new Item();
            item.SetDefaults(Type);
            item.position.X = (float)(X + (Width / 2) - (item.width / 2));
            item.position.Y = (float)(Y + (Height / 2) - (item.height / 2));
            item.wet = Collision.WetCollision(item.position, item.width, item.height, world);
            item.velocity.X = (float)Statics.rand.Next(-20, 21) * 0.1F;
            item.velocity.Y = (float)Statics.rand.Next(-30, -10) * 0.1F;
            item.active = true;
            item.spawnTime = 0;
            item.stack = Stack;
            if ((Statics.netMode == 2) && !noBroadcast)
            {
                NetMessage.SendData(21, world, -1, -1, "", i1, 0.0F, 0.0F, 0.0F);
                item.FindOwner(i1, world);
            }
            else if (Statics.netMode == 0)
            {
                item.owner = Statics.myPlayer;
            }
            world.setItem(i1, item);
            return i1;
        }

    }
}
