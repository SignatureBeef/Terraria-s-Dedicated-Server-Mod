using System;

namespace Terraria_Server
{
    public class Chest
    {
        public static int maxItems = 20;
        public Item[] item = new Item[maxItems];
        public int x;
        public int y;

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public static int UsingChest(int i)
        {
            if (Main.chest[i] != null)
            {
                int index = 0;
                foreach(Player player in Main.player)
                {
                    if (player.active && player.chest == i)
                    {
                        return index;
                    }
                    index++;
                }
            }
            return -1;
        }

        public static int FindChest(int X, int Y)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Main.chest[i] != null && Main.chest[i].x == X && Main.chest[i].y == Y)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int CreateChest(int X, int Y)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Main.chest[i] != null && Main.chest[i].x == X && Main.chest[i].y == Y)
                {
                    return -1;
                }
            }
            for (int j = 0; j < 1000; j++)
            {
                if (Main.chest[j] == null)
                {
                    Main.chest[j] = new Chest();
                    Main.chest[j].x = X;
                    Main.chest[j].y = Y;
                    for (int k = 0; k < Chest.maxItems; k++)
                    {
                        Main.chest[j].item[k] = new Item();
                    }
                    return j;
                }
            }
            return -1;
        }

        public static bool DestroyChest(int X, int Y)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Main.chest[i] != null && Main.chest[i].x == X && Main.chest[i].y == Y)
                {
                    for (int j = 0; j < Chest.maxItems; j++)
                    {
                        if (Main.chest[i].item[j].type > 0 && Main.chest[i].item[j].stack > 0)
                        {
                            return false;
                        }
                    }
                    Main.chest[i] = null;
                    return true;
                }
            }
            return true;
        }

        public enum ShopType
        {
            ARMS_DEALER,
            CLOTHIER,
            DEMOLITIONIST,
            DRYAD,
            MERCHANT
        }

        private void SetupMerchant()
        {
            int num = 0;
            this.item[num].SetDefaults("Mining Helmet");
            num++;
            this.item[num].SetDefaults("Piggy Bank");
            num++;
            this.item[num].SetDefaults("Iron Anvil");
            num++;
            this.item[num].SetDefaults("Copper Pickaxe");
            num++;
            this.item[num].SetDefaults("Copper Axe");
            num++;
            this.item[num].SetDefaults("Torch");
            num++;
            this.item[num].SetDefaults("Lesser Healing Potion");
            num++;
            if (Main.player[Main.myPlayer].statManaMax == 200)
            {
                this.item[num].SetDefaults("Lesser Mana Potion");
                num++;
            }
            this.item[num].SetDefaults("Wooden Arrow");
            num++;
            this.item[num].SetDefaults("Shuriken");
            num++;
            if (Main.bloodMoon)
            {
                this.item[num].SetDefaults("Throwing Knife");
                num++;
            }
            if (!Main.dayTime)
            {
                this.item[num].SetDefaults("Glowstick");
                num++;
            }
        }

        private void SetupArmsDealer()
        {
            int num2 = 0;
            this.item[num2].SetDefaults("Musket Ball");
            num2++;
            if (Main.bloodMoon)
            {
                this.item[num2].SetDefaults("Silver Bullet");
                num2++;
            }
            if (NPC.downedBoss2 && !Main.dayTime)
            {
                this.item[num2].SetDefaults(47, false);
                num2++;
            }
            this.item[num2].SetDefaults("Flintlock Pistol");
            num2++;
            this.item[num2].SetDefaults("Minishark");
            num2++;
            if (Main.moonPhase == 4)
            {
                this.item[num2].SetDefaults(324, false);
                num2++;
                return;
            }
        }

        public void SetupDryad()
        {
            int num3 = 0;
            if (Main.bloodMoon)
            {
                this.item[num3].SetDefaults(67, false);
                num3++;
                this.item[num3].SetDefaults(59, false);
                num3++;
            }
            else
            {
                this.item[num3].SetDefaults("Purification Powder");
                num3++;
                this.item[num3].SetDefaults("Grass Seeds");
                num3++;
                this.item[num3].SetDefaults("Sunflower");
                num3++;
            }
            this.item[num3].SetDefaults("Acorn");
            num3++;
            this.item[num3].SetDefaults(114, false);
            num3++;
            return;
        }

        public void SetupDemolitionist()
        {
            int num4 = 0;
            this.item[num4].SetDefaults("Grenade");
            num4++;
            this.item[num4].SetDefaults("Bomb");
            num4++;
            this.item[num4].SetDefaults("Dynamite");
            num4++;
        }

        public void SetupClothier()
        {
            int num5 = 0;
            this.item[num5].SetDefaults(254, false);
            num5++;
            if (Main.dayTime)
            {
                this.item[num5].SetDefaults(242, false);
                num5++;
            }
            if (Main.moonPhase == 0)
            {
                this.item[num5].SetDefaults(245, false);
                num5++;
                this.item[num5].SetDefaults(246, false);
                num5++;
            }
            else
            {
                if (Main.moonPhase == 1)
                {
                    this.item[num5].SetDefaults(325, false);
                    num5++;
                    this.item[num5].SetDefaults(326, false);
                    num5++;
                }
            }
            this.item[num5].SetDefaults(269, false);
            num5++;
            this.item[num5].SetDefaults(270, false);
            num5++;
            this.item[num5].SetDefaults(271, false);
            num5++;
            if (Main.bloodMoon)
            {
                this.item[num5].SetDefaults(322, false);
                num5++;
            }
        }

        public void SetupShop(ShopType type)
        {
            for (int i = 0; i < maxItems; i++)
            {
                this.item[i] = new Item();
            }

            switch(type)
            {
                case ShopType.MERCHANT:
                    SetupMerchant();
                    break;
                case ShopType.ARMS_DEALER:
                    SetupArmsDealer();
                    break;
                case ShopType.DRYAD:
                    SetupDryad();
                    break;
                case ShopType.DEMOLITIONIST:
                    SetupDemolitionist();
                    break;
                case ShopType.CLOTHIER:
                    SetupClothier();
                    break;
            }
        }
    }
}
