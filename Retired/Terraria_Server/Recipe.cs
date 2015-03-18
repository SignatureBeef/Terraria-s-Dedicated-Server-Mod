using System;
using Terraria_Server.Collections;

namespace Terraria_Server
{
	public class Recipe
	{
		public static int maxRequirements = 10;
		public static int maxRecipes = 200;
		public static int numRecipes = 0;
		private static Recipe newRecipe = new Recipe();
        public Item createItem;
		public Item[] requiredItem = new Item[Recipe.maxRequirements];
		public int[] requiredTile = new int[Recipe.maxRequirements];

		public Recipe()
		{
			for (int i = 0; i < Recipe.maxRequirements; i++)
			{
				this.requiredItem[i] = new Item();
				this.requiredTile[i] = -1;
			}
		}

		public void Create()
		{
			int num = 0;
			while (num < Recipe.maxRequirements && this.requiredItem[num].Type != 0)
			{
				int num2 = this.requiredItem[num].Stack;
				for (int i = 0; i < 44; i++)
				{
					if (Main.players[Main.myPlayer].inventory[i].IsTheSameAs(this.requiredItem[num]))
					{
						if (Main.players[Main.myPlayer].inventory[i].Stack > num2)
						{
							Main.players[Main.myPlayer].inventory[i].Stack -= num2;
							num2 = 0;
						}
						else
						{
							num2 -= Main.players[Main.myPlayer].inventory[i].Stack;
							Main.players[Main.myPlayer].inventory[i] = new Item();
						}
					}
					if (num2 <= 0)
					{
						break;
					}
				}
				num++;
			}
			Recipe.FindRecipes();
		}

		public static void FindRecipes()
		{
			int num = Main.availableRecipe[Main.focusRecipe];
			float num2 = Main.availableRecipeY[Main.focusRecipe];
			for (int i = 0; i < Recipe.maxRecipes; i++)
			{
				Main.availableRecipe[i] = 0;
			}
			Main.numAvailableRecipes = 0;
			int num3 = 0;
			while (num3 < Recipe.maxRecipes && Main.recipe[num3].createItem.Type != 0)
			{
				bool flag = true;
				int num4 = 0;
				while (num4 < Recipe.maxRequirements && Main.recipe[num3].requiredItem[num4].Type != 0)
				{
					int num5 = Main.recipe[num3].requiredItem[num4].Stack;
					for (int j = 0; j < 44; j++)
					{
						if (Main.players[Main.myPlayer].inventory[j].IsTheSameAs(Main.recipe[num3].requiredItem[num4]))
						{
							num5 -= Main.players[Main.myPlayer].inventory[j].Stack;
						}
						if (num5 <= 0)
						{
							break;
						}
					}
					if (num5 > 0)
					{
						flag = false;
						break;
					}
					num4++;
				}
				if (flag)
				{
					bool flag2 = true;
					int num6 = 0;
					while (num6 < Recipe.maxRequirements && Main.recipe[num3].requiredTile[num6] != -1)
					{
						if (!Main.players[Main.myPlayer].adjTile[Main.recipe[num3].requiredTile[num6]])
						{
							flag2 = false;
							break;
						}
						num6++;
					}
					if (flag2)
					{
						Main.availableRecipe[Main.numAvailableRecipes] = num3;
						Main.numAvailableRecipes++;
					}
				}
				num3++;
			}
			for (int k = 0; k < Main.numAvailableRecipes; k++)
			{
				if (num == Main.availableRecipe[k])
				{
					Main.focusRecipe = k;
					break;
				}
			}
			if (Main.focusRecipe >= Main.numAvailableRecipes)
			{
				Main.focusRecipe = Main.numAvailableRecipes - 1;
			}
			if (Main.focusRecipe < 0)
			{
				Main.focusRecipe = 0;
			}
			float num7 = Main.availableRecipeY[Main.focusRecipe] - num2;
			for (int l = 0; l < Recipe.maxRecipes; l++)
			{
				Main.availableRecipeY[l] -= num7;
			}
		}

		public static void SetupRecipes()
		{
            Recipe.numRecipes = 0;
			Recipe.newRecipe.createItem = Registries.Item.Create(28);
			Recipe.newRecipe.createItem.Stack = 2;
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(5);
            Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(23, 2);
            Recipe.newRecipe.requiredItem[2] = Registries.Item.Create(31, 2);
			Recipe.newRecipe.requiredTile[0] = 13;
			Recipe.addRecipe();
            Recipe.newRecipe.createItem = Registries.Item.Create(188);
            Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(28, 2);
            Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(183);
			Recipe.newRecipe.requiredTile[0] = 13;
			Recipe.addRecipe();
            Recipe.newRecipe.createItem = Registries.Item.Create(110, 2);
            Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(75);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(23, 2);
			Recipe.newRecipe.requiredItem[2] = Registries.Item.Create(31);
			Recipe.newRecipe.requiredItem[2].Stack = 2;
			Recipe.newRecipe.requiredTile[0] = 13;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(189);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(110, 2);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(183);
			Recipe.newRecipe.requiredTile[0] = 13;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(226);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(28);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(110);
			Recipe.newRecipe.requiredTile[0] = 13;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(227);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(188);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(189);
			Recipe.newRecipe.requiredTile[0] = 13;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(67, 5);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(60);
			Recipe.newRecipe.requiredTile[0] = 13;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(31, 2);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create("Glass");
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(8, 3);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(23);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9);
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(235);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(166);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(23, 5);
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(170);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(169, 2);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(222);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(133, 6);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(129);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(3, 2);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(130, 4);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(129);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(131);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(133, 2);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(132, 4);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(131);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(145);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(3);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(12);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(146, 4);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(145);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(144, 4);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(143);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(143);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(3);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(14);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(142, 4);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(141);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(141);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(3);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(13);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(214);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(174);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(1);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(192);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(173, 2);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(30, 4);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(2);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(26);
			Recipe.newRecipe.createItem.Stack = 4;
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(3);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(93, 4);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(94);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9);
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(25);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9, 6);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(34);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9, 4);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Sign");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9);
			Recipe.newRecipe.requiredItem[0].Stack = 6;
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(48);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9, 8);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(22, 2);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(32);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9, 8);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(36);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9, 10);
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(24);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9, 7);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(196);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9, 8);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(40, 3);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(3);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(39);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9, 10);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(224);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(9, 15);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(225, 5);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(225);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(150, 10);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(41, 5);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(40, 5);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(8);
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(33);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(3, 20);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 4);
			Recipe.newRecipe.requiredItem[2] = Registries.Item.Create(8, 3);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(20);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(12, 3);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Copper Pickaxe");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(20, 12);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 4);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Copper Axe");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(20, 9);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 3);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Copper Hammer");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(20, 10);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 3);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Copper Broadsword");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(20, 8);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Copper Shortsword");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(20, 7);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Copper Bow");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(20, 7);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Copper Helmet");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(20, 15);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(80);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(20, 25);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(76);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(20, 20);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(15);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(20, 10);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(85);
			Recipe.newRecipe.requiredTile[0] = 14;
			Recipe.newRecipe.requiredTile[1] = 15;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(106);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(20, 4);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(8, 4);
			Recipe.newRecipe.requiredItem[2] = Registries.Item.Create(85);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(22);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(11, 3);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(35);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(22, 5);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(205);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(22, 3);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(1);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(22, 12);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 3);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(10);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(22, 9);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 3);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(7);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(22, 10);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 3);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(4);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(22, 8);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(6);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(22, 7);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(99);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(22, 7);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(90);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(22, 20);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(81);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(22, 30);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(77);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(22, 25);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(85);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(22, 3);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(21);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(14, 4);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Silver Pickaxe");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(21, 12);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 4);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Silver Axe");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(21, 9);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 3);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Silver Hammer");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(21, 10);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 3);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Silver Broadsword");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(21, 8);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Silver Bow");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(21, 7);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(91);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(21, 20);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(82);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(21, 30);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(78);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(21, 25);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(16);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(21, 10);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(85);
			Recipe.newRecipe.requiredTile[0] = 14;
			Recipe.newRecipe.requiredTile[1] = 15;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(107);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(21, 4);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(8, 4);
			Recipe.newRecipe.requiredItem[2] = Registries.Item.Create(85);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(19);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(13, 4);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Gold Pickaxe");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(19, 12);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 4);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Gold Axe");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(19, 9);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 3);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Gold Hammer");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(19, 10);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(9, 3);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Gold Broadsword");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(19, 8);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Gold Shortsword");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(19, 7);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create("Gold Bow");
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(19, 7);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(92);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(19, 25);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(83);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(19, 35);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
            Recipe.newRecipe.createItem = Registries.Item.Create(79);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(19, 30);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(17);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(19, 10);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(85);
			Recipe.newRecipe.requiredTile[0] = 14;
			Recipe.newRecipe.requiredTile[1] = 15;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(108);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(19, 4);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(8, 4);
			Recipe.newRecipe.requiredItem[2] = Registries.Item.Create(85);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(105);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(19);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(8);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(57);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(56, 4);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(44);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(57, 8);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(47, 2);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(40, 2);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(69);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(45);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(57, 10);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(46);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(57, 10);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(102);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(57, 15);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(86, 10);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(101);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(57, 25);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(86, 20);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(100);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(57, 20);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(86, 15);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(103);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(57, 12);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(86, 6);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(104);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(57, 10);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(86, 5);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(84);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(85, 3);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(118, 1);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(117);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(116, 6);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(198);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(117, 20);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(177, 10);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(199);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(117, 20);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(178, 10);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(200);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(117, 20);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(179, 10);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(201);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(117, 20);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(181, 10);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(202);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(117, 20);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(182, 10);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(203);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(117, 20);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(180, 10);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(204);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(117, 35);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(127);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(95);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(117, 30);
			Recipe.newRecipe.requiredItem[2] = Registries.Item.Create(75, 10);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(197);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(98);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(117, 20);
			Recipe.newRecipe.requiredItem[2] = Registries.Item.Create(75, 5);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(123);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(117, 25);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(124);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(117, 35);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(125);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(117, 30);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(234, 100);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(117);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(151);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(154, 25);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(150, 40);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(152);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(154, 35);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(150, 50);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(153);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(154, 30);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(150, 45);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(175);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(174, 6);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(173, 2);
			Recipe.newRecipe.requiredTile[0] = 77;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(119);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(175, 15);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(55);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(120);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(175, 25);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(121);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(175, 35);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(122);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(175, 35);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(217);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(175, 35);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(219);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(175, 20);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(164);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(231);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(175, 30);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(232);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(175, 40);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(233);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(175, 35);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(190);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create("Silver Broadsword");
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(208, 40);
			Recipe.newRecipe.requiredItem[2] = Registries.Item.Create(209, 20);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(191);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(208, 40);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(209, 30);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(185);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(84);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(208, 30);
			Recipe.newRecipe.requiredItem[2] = Registries.Item.Create(210, 3);
			Recipe.newRecipe.requiredTile[0] = 16;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(18);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(20, 10);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(21, 8);
			Recipe.newRecipe.requiredItem[2] = Registries.Item.Create(19, 6);
			Recipe.newRecipe.requiredTile[0] = 14;
			Recipe.newRecipe.requiredTile[1] = 15;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(193);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(173, 20);
			Recipe.newRecipe.requiredTile[0] = 17;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(37);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(38, 2);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.newRecipe.requiredTile[1] = 15;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(237);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(236, 2);
			Recipe.newRecipe.requiredTile[0] = 18;
			Recipe.newRecipe.requiredTile[1] = 15;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(109);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(75, 10);
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(43);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(38, 10);
			Recipe.newRecipe.requiredTile[0] = 26;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(70);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(67, 30);
			Recipe.newRecipe.requiredItem[1] = Registries.Item.Create(68, 15);
			Recipe.newRecipe.requiredTile[0] = 26;
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(71, 100);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(72);
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(72);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(71, 100);
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(72, 100);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(73);
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(73);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(72, 100);
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(73, 100);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(74);
			Recipe.addRecipe();
			Recipe.newRecipe.createItem = Registries.Item.Create(74);
			Recipe.newRecipe.requiredItem[0] = Registries.Item.Create(73, 100);
			Recipe.addRecipe();
		}

		private static void addRecipe()
		{
			Main.recipe[Recipe.numRecipes] = Recipe.newRecipe;
			Recipe.newRecipe = new Recipe();
			Recipe.numRecipes++;
		}
	}
}
