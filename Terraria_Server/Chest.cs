using System;

namespace Terraria_Server
{
	public struct Chest
	{
		public const Int32 MAX_ITEMS = 20;
		public Item[] contents;
		public int x;
		public int y;

		public static Chest InitializeChest(int X, int Y)
		{
			var chest = new Chest()
			{
				x = X,
				y = Y
			};

			chest.ClearItems();

			return chest;
		}

		public static void Unlock(Func<Int32, Int32, ITile> TileRefs, int X, int Y)
		{
			if(TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			for (int i = X; i <= X + 1; i++)
			{
				for (int j = Y; j <= Y + 1; j++)
				{
					if ((TileRefs(i, j).FrameX >= 72 && TileRefs(i, j).FrameX <= 106) || (TileRefs(i, j).FrameX >= 144 && TileRefs(i, j).FrameX <= 178))
					{
						TileRefs(i, j).AddFrameX(-36);
					}
				}
			}
		}

		public bool HasContents()
		{
			foreach (var item in contents)
			{
				if (item.Type > 0 && item.Stack > 0)
					return true;
			}
			return false;
		}

		public static int UsingChest(int i)
		{
			var chest = Main.chest[i];
			if (chest != default(Chest))
			{
				int index = 0;
				foreach (var player in Main.players)
				{
					if (player.Active && player.chest == i)
					{
						if (Math.Abs(player.Position.X / 16 - chest.x) < 7 && Math.Abs(player.Position.Y / 16 - chest.y) < 7)
							return index;
						else
							player.chest = -1;
					}
					index++;
				}
			}
			return -1;
		}

		public static int FindChest(int X, int Y)
		{
			for (int i = 0; i < Main.MAX_CHESTS; i++)
			{
				if (Main.chest[i] != default(Chest) && Main.chest[i].x == X && Main.chest[i].y == Y)
					return i;
			}
			return -1;
		}

		public static int CreateChest(int X, int Y)
		{
			//Opposite logic of find chest in that if the chest is found we now return -1.
			if (FindChest(X, Y) != -1)
				return -1;

			for (int i = 0; i < Main.MAX_CHESTS; i++)
			{
				if (Main.chest[i] == default(Chest))
				{
					Main.chest[i] = InitializeChest(X, Y);
					return i;
				}
			}
			return -1;
		}

		public static bool DestroyChest(int X, int Y)
		{
			int chestIndex = FindChest(X, Y);
			if (chestIndex == -1)
				return true;

			Chest chestToDestroy = Main.chest[chestIndex];
			if (chestToDestroy.HasContents())
				return false;

			Main.chest[chestIndex].Clean();
			return true;
		}

		public void ClearItems()
		{
			contents = new Item[MAX_ITEMS];
			for (int i = 0; i < contents.Length; i++)
				contents[i] = new Item();
		}

		public void Clean()
		{
			x = 0;
			y = 0;

			ClearItems();
		}

		public static bool operator ==(Chest chest1, Chest chest2)
		{
			return
				chest1.x == chest2.x
				&&
				chest1.y == chest2.y
				&&
				chest1.contents == chest2.contents;
		}

		public static bool operator !=(Chest chest1, Chest chest2)
		{
			return
				chest1.x != chest2.x
				||
				chest1.y != chest2.y
				||
				chest1.contents != chest2.contents;
		}

		public override bool Equals(object obj)
		{
			//if (obj is Chest)
			//    return (Chest)obj == this;

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ x ^ y ^ contents.Length;
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
