using System;
namespace Terraria_Server
{
	public class Chest
	{
		public static int maxItems = 20;
		public Item[] item = new Item[Chest.maxItems];
		public int x;
		public int y;
		public static int UsingChest(int i)
		{
			if (Main.chest[i] != null)
			{
				for (int j = 0; j < 255; j++)
				{
					if (Main.player[j].chest == i)
					{
						return j;
					}
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
		public void SetupShop(int type)
		{
			for (int i = 0; i < Chest.maxItems; i++)
			{
				this.item[i] = new Item();
			}
			if (type == 1)
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
				this.item[num].SetDefaults("Wooden Arrow");
				num++;
				this.item[num].SetDefaults("Shuriken");
				num++;
				return;
			}
			if (type == 2)
			{
				int num2 = 0;
				this.item[num2].SetDefaults("Musket Ball");
				num2++;
				this.item[num2].SetDefaults("Flintlock Pistol");
				num2++;
				this.item[num2].SetDefaults("Minishark");
				num2++;
				return;
			}
			if (type == 3)
			{
				int num3 = 0;
				this.item[num3].SetDefaults("Purification Powder");
				num3++;
				this.item[num3].SetDefaults("Acorn");
				num3++;
				this.item[num3].SetDefaults("Grass Seeds");
				num3++;
				this.item[num3].SetDefaults("Sunflower");
				num3++;
				this.item[num3].SetDefaults(114);
				num3++;
				return;
			}
			if (type == 4)
			{
				int num4 = 0;
				this.item[num4].SetDefaults("Grenade");
				num4++;
				this.item[num4].SetDefaults("Bomb");
				num4++;
				this.item[num4].SetDefaults("Dynamite");
				num4++;
			}
		}
	}
}
