using System;
using System.IO;
using System.Diagnostics;
using Terraria_Server.Commands;
using Terraria_Server.Misc;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;
using Terraria_Server.Logging;
using Terraria_Server.Networking;

namespace Terraria_Server.WorldMod
{
	public interface ISandbox
	{
		void Initialize();

		void AddWater(int x, int y);
		void ShadowOrbSmashed(int x, int y);
		void NewItem(int x, int y, int w, int h, int type, int stack = 1, bool noBroadcast = false);
		void KillSign(int x, int y);
		void DestroyChest(int x, int y);

		bool ActiveAt(int x, int y);
		bool LitAt(int x, int y);
		bool LavaAt(int x, int y);

		byte TypeAt(int x, int y);
		byte WallAt(int x, int y);
		byte Liquid(int x, int y);
		byte FrameAt(int x, int y);

		short FrameXAt(int x, int y);
		short FrameYAt(int x, int y);

		void SetActiveAt(int x, int y, bool val);
		void SetLitAt(int x, int y, bool val);
		void SetLavaAt(int x, int y, bool val);

		void SetTypeAt(int x, int y, byte val);
		void SetWallAt(int x, int y, byte val);
		void SetLiquid(int x, int y, byte val);
		void SetFrameAt(int x, int y, byte val);

		void SetFrameXAt(int x, int y, short val);
		void SetFrameYAt(int x, int y, short val);

		void FallingBlockProjectile(int x, int y, int type);

		bool Wire(int x, int y);
		void SetWire(int x, int y, bool val);

		void SetSkipLiquid(int x, int y, bool val);
		bool SkipLiquid(int x, int y);

		void SetCheckingLiquid(int x, int y, bool val);
		bool CheckingLiquid(int x, int y);

		void AddLiquid(int x, int y, int val);
		void AddFrameX(int x, int y, int val);
		void AddFrameY(int x, int y, int val);
	}

	public class SandboxEditor
	{
		//		public static Shaper<T> Create<T> ()
		//			where T : ISandbox
		//		{
		//			return new Shaper<T> (sandbox);
		//		}
	}

	public class SandboxEditor<TBox> : SandboxEditor
		where TBox : ISandbox
	{
		TBox sandbox;

		public TBox Sandbox { get { return sandbox; } }

		public SandboxEditor()
		{
			sandbox = default(TBox);
			sandbox.Initialize();
		}

		public SandboxEditor(TBox sandbox)
		{
			this.sandbox = sandbox;
		}

		public struct TRef : ITile
		{
			readonly SandboxEditor<TBox> editor;
			readonly short x;
			readonly short y;

			public TRef(SandboxEditor<TBox> shaper, int x, int y)
			{
				this.editor = shaper;
				this.x = (short)x;
				this.y = (short)y;
			}

			bool ITile.Active { get { return editor.sandbox.ActiveAt(x, y); } }
			public bool Active { get { return editor.sandbox.ActiveAt(x, y); } }
			public void SetActive(bool val)
			{
				editor.sandbox.SetActiveAt(x, y, val);
			}

			public bool Lighted { get { return editor.sandbox.LitAt(x, y); } }
			public void SetLighted(bool val)
			{
				editor.sandbox.SetLitAt(x, y, val);
			}

			public bool Lava { get { return editor.sandbox.LavaAt(x, y); } }
			public void SetLava(bool val)
			{
				editor.sandbox.SetLavaAt(x, y, val);
			}

			public byte Type { get { return editor.sandbox.TypeAt(x, y); } }
			public void SetType(byte val)
			{
				editor.sandbox.SetTypeAt(x, y, val);
			}

			public byte Wall { get { return editor.sandbox.WallAt(x, y); } }
			public void SetWall(byte val)
			{
				editor.sandbox.SetWallAt(x, y, val);
			}

			public byte Liquid { get { return editor.sandbox.Liquid(x, y); } }
			public void SetLiquid(byte val)
			{
				editor.sandbox.SetLiquid(x, y, val);
			}

			public byte FrameNumber { get { return editor.sandbox.FrameAt(x, y); } }
			public void SetFrameNumber(byte val)
			{
				editor.sandbox.SetFrameAt(x, y, val);
			}

			public short FrameX { get { return editor.sandbox.FrameXAt(x, y); } }
			public void SetFrameX(short val)
			{
				editor.sandbox.SetFrameXAt(x, y, val);
			}

			public short FrameY { get { return editor.sandbox.FrameYAt(x, y); } }
			public void SetFrameY(short val)
			{
				editor.sandbox.SetFrameYAt(x, y, val);
			}

			public bool Wire { get { return editor.sandbox.Wire(x, y); } }
			public void SetWire(bool val)
			{
				editor.sandbox.SetWire(x, y, val);
			}

			public void SetSkipLiquid(bool val)
			{
				editor.sandbox.SetSkipLiquid(x, y, val);
			}
			public bool SkipLiquid { get { return editor.sandbox.SkipLiquid(x, y); } }

			public void SetCheckingLiquid(bool val)
			{
				editor.sandbox.SetCheckingLiquid(x, y, val);
			}
			public bool CheckingLiquid { get { return editor.sandbox.CheckingLiquid(x, y); } }

			public void AddLiquid(int val)
			{
				editor.sandbox.AddLiquid(x, y, val);
			}
			public void AddFrameX(int val)
			{
				editor.sandbox.AddFrameX(x, y, val);
			}
			public void AddFrameY(int val)
			{
				editor.sandbox.AddFrameY(x, y, val);
			}
		}

		private const int RECTANGLE_OFFSET = 25;
		private const int TILE_OFFSET = 15;
		private const int TILES_OFFSET_2 = 10;
		private const int TILE_OFFSET_3 = 16;
		private const int TILE_OFFSET_4 = 23;
		private const int TILE_SCALE = 16;
		private const int TREE_RADIUS = 2;
		private const int MAX_TILE_SETS = 107;

		public const bool noTileActions = false;
		public bool spawnMeteor = false;
		private bool mergeUp = false;
		private bool mergeDown = false;
		private bool mergeLeft = false;
		private bool mergeRight = false;
		public const bool stopDrops = false;
		public const bool noLiquidCheck = false;


		Random genRand = new Random((int)DateTime.Now.Ticks);

		private bool destroyObject = false;

		public const int maxRoomTiles = 1900;

		public TRef TileAt(int x, int y)
		{
			return new TRef(this, x, y);
		}

		public ITile ITileAt(int x, int y)
		{
			return new TRef(this, x, y);
		}

		public bool EmptyTileCheck(int startX, int endX, int startY, int endY, int ignoreStyle = -1)
		{
			if (startX < 0)
			{
				return false;
			}
			if (endX >= Main.maxTilesX)
			{
				return false;
			}
			if (startY < 0)
			{
				return false;
			}
			if (endY >= Main.maxTilesY)
			{
				return false;
			}
			for (int i = startX; i < endX + 1; i++)
			{
				for (int j = startY; j < endY + 1; j++)
				{
					if (TileAt(i, j).Active)
					{
						if (ignoreStyle == -1)
						{
							return false;
						}

						int type = TileAt(i, j).Type;

						if (ignoreStyle == 11 && type != 11)
						{
							return false;
						}
						if (ignoreStyle == 20 && type != 20 && type != 3 && type != 24 && type != 61 && type != 32 && type != 69 && type != 73 && type != 74)
						{
							return false;
						}
						if (ignoreStyle == 71 && type != 71)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public bool PlaceDoor(int i, int j, int type)
		{
			bool result;
			try
			{
				if (TileAt(i, j - 2).Active && Main.tileSolid[(int)TileAt(i, j - 2).Type] && TileAt(i, j + 2).Active && Main.tileSolid[(int)TileAt(i, j + 2).Type])
				{
					TileAt(i, j - 1).SetActive(true);
					TileAt(i, j - 1).SetType(10);
					TileAt(i, j - 1).SetFrameY(0);
					TileAt(i, j - 1).SetFrameX((short)(genRand.Next(3) * 18));
					TileAt(i, j).SetActive(true);
					TileAt(i, j).SetType(10);
					TileAt(i, j).SetFrameY(18);
					TileAt(i, j).SetFrameX((short)(genRand.Next(3) * 18));
					TileAt(i, j + 1).SetActive(true);
					TileAt(i, j + 1).SetType(10);
					TileAt(i, j + 1).SetFrameY(36);
					TileAt(i, j + 1).SetFrameX((short)(genRand.Next(3) * 18));
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public bool CloseDoor(int x, int y, bool forced, ISender sender)
		{
			if (sender == null)
			{
				sender = new ConsoleSender();
			}

			if (Program.properties.NPCDoorOpenCancel && sender is NPC)
				return false;

			int num = 0;
			int num2 = x;
			int num3 = y;

			int frameX = (int)TileAt(x, y).FrameX;
			int frameY = (int)TileAt(x, y).FrameY;
			if (frameX == 0)
			{
				num2 = x;
				num = 1;
			}
			else if (frameX == 18)
			{
				num2 = x - 1;
				num = 1;
			}
			else if (frameX == 36)
			{
				num2 = x + 1;
				num = -1;
			}
			else if (frameX == 54)
			{
				num2 = x;
				num = -1;
			}
			if (frameY == 0)
			{
				num3 = y;
			}
			else if (frameY == 18)
			{
				num3 = y - 1;
			}
			else if (frameY == 36)
			{
				num3 = y - 2;
			}
			int num4 = num2;
			if (num == -1)
			{
				num4 = num2 - 1;
			}
			if (!forced)
			{
				for (int k = num3; k < num3 + 3; k++)
				{
					if (!Collision.EmptyTile(num2, k, true))
					{
						return false;
					}
				}
			}
			for (int l = num4; l < num4 + 2; l++)
			{
				for (int m = num3; m < num3 + 3; m++)
				{
					if (l == num2)
					{
						TileAt(l, m).SetType(10);
						TileAt(l, m).SetFrameX((short)(genRand.Next(3) * 18));
					}
					else
					{
						TileAt(l, m).SetActive(false);
					}
				}
			}
			for (int n = num2 - 1; n <= num2 + 1; n++)
			{
				for (int num5 = num3 - 1; num5 <= num3 + 2; num5++)
				{
					TileFrame(n, num5, false, false);
				}
			}
			return true;
		}

		public bool OpenDoor(int x, int y, int direction, ISender sender)
		{
			if (sender == null)
			{
				sender = new ConsoleSender();
			}

			if (Program.properties.NPCDoorOpenCancel && sender is NPC)
				return false;

			int num = 0;
			if (TileAt(x, y - 1).FrameY == 0 && TileAt(x, y - 1).Type == TileAt(x, y).Type)
			{
				num = y - 1;
			}
			else if (TileAt(x, y - 2).FrameY == 0 && TileAt(x, y - 2).Type == TileAt(x, y).Type)
			{
				num = y - 2;
			}
			else if (TileAt(x, y + 1).FrameY == 0 && TileAt(x, y + 1).Type == TileAt(x, y).Type)
			{
				num = y + 1;
			}
			else
			{
				num = y;
			}
			int num2 = x;
			short num3 = 0;
			int num4;
			if (direction == -1)
			{
				num2 = x - 1;
				num3 = 36;
				num4 = x - 1;
			}
			else
			{
				num2 = x;
				num4 = x + 1;
			}
			bool flag = true;
			for (int k = num; k < num + 3; k++)
			{
				if (TileAt(num4, k).Active)
				{
					if (TileAt(num4, k).Type != 3 && TileAt(num4, k).Type != 24 && TileAt(num4, k).Type != 52 && TileAt(num4, k).Type != 61 && TileAt(num4, k).Type != 62 && TileAt(num4, k).Type != 69 && TileAt(num4, k).Type != 71 && TileAt(num4, k).Type != 73 && TileAt(num4, k).Type != 74)
					{
						flag = false;
						break;
					}
					KillTile(num4, k, false, false, false);
				}
			}
			if (flag)
			{
				TileAt(num2, num).SetActive(true);
				TileAt(num2, num).SetType(11);
				TileAt(num2, num).SetFrameY(0);
				TileAt(num2, num).SetFrameX(num3);
				TileAt(num2 + 1, num).SetActive(true);
				TileAt(num2 + 1, num).SetType(11);
				TileAt(num2 + 1, num).SetFrameY(0);
				TileAt(num2 + 1, num).SetFrameX((short)(num3 + 18));
				TileAt(num2, num + 1).SetActive(true);
				TileAt(num2, num + 1).SetType(11);
				TileAt(num2, num + 1).SetFrameY(18);
				TileAt(num2, num + 1).SetFrameX(num3);
				TileAt(num2 + 1, num + 1).SetActive(true);
				TileAt(num2 + 1, num + 1).SetType(11);
				TileAt(num2 + 1, num + 1).SetFrameY(18);
				TileAt(num2 + 1, num + 1).SetFrameX((short)(num3 + 18));
				TileAt(num2, num + 2).SetActive(true);
				TileAt(num2, num + 2).SetType(11);
				TileAt(num2, num + 2).SetFrameY(36);
				TileAt(num2, num + 2).SetFrameX(num3);
				TileAt(num2 + 1, num + 2).SetActive(true);
				TileAt(num2 + 1, num + 2).SetType(11);
				TileAt(num2 + 1, num + 2).SetFrameY(36);
				TileAt(num2 + 1, num + 2).SetFrameX((short)(num3 + 18));
				for (int l = num2 - 1; l <= num2 + 2; l++)
				{
					for (int m = num - 1; m <= num + 2; m++)
					{
						TileFrame(l, m, false, false);
					}
				}
			}
			return flag;
		}

		public void Check1x2(int x, int j, byte type)
		{
			if (destroyObject)
			{
				return;
			}
			int num = j;
			bool flag = true;

			int FrameY = (int)TileAt(x, num).FrameY;

			int num2 = 0;
			while (FrameY >= 40)
			{
				FrameY -= 40;
				num2++;
			}

			if (FrameY == 18)
			{
				num--;
			}
			if ((int)TileAt(x, num).FrameY == 40 * num2 && (int)TileAt(x, num + 1).FrameY == 40 * num2 + 18 && TileAt(x, num).Type == type && TileAt(x, num + 1).Type == type)
			{
				flag = false;
			}
			if (!TileAt(x, num + 2).Active || !Main.tileSolid[(int)TileAt(x, num + 2).Type])
			{
				flag = true;
			}
			if (TileAt(x, num + 2).Type != 2 && TileAt(x, num).Type == 20)
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				if (TileAt(x, num).Type == type)
				{
					KillTile(x, num, false, false, false);
				}
				if (TileAt(x, num + 1).Type == type)
				{
					KillTile(x, num + 1, false, false, false);
				}
				if (type == 15)
				{
					if (num2 == 1)
					{
						sandbox.NewItem(x * 16, num * 16, 32, 32, 358, 1, false);
					}
					else
					{
						sandbox.NewItem(x * 16, num * 16, 32, 32, 34, 1, false);
					}
				}
				destroyObject = false;
			}
		}

		public void CheckOnTable1x1(int x, int y, int type)
		{
			if ((!TileAt(x, y + 1).Active || !Main.tileTable[(int)TileAt(x, y + 1).Type]))
			{
				if (type == 78)
				{
					if (!TileAt(x, y + 1).Active || !Main.tileSolid[(int)TileAt(x, y + 1).Type])
					{
						KillTile(x, y, false, false, false);
						return;
					}
				}
				else
				{
					KillTile(x, y, false, false, false);
				}
			}
		}

		public void CheckSign(int x, int y, int type)
		{
			if (destroyObject)
			{
				return;
			}
			int num = x - 2;
			int num2 = x + 3;
			int num3 = y - 2;
			int num4 = y + 3;
			if (num < 0)
			{
				return;
			}
			if (num2 > Main.maxTilesX)
			{
				return;
			}
			if (num3 < 0)
			{
				return;
			}
			if (num4 > Main.maxTilesY)
			{
				return;
			}
			bool flag = false;
			int k = (int)(TileAt(x, y).FrameX / 18);
			int num5 = (int)(TileAt(x, y).FrameY / 18);
			while (k > 1)
			{
				k -= 2;
			}
			int num6 = x - k;
			int num7 = y - num5;
			int num8 = (int)(TileAt(num6, num7).FrameX / 18 / 2);
			num = num6;
			num2 = num6 + 2;
			num3 = num7;
			num4 = num7 + 2;
			k = 0;
			for (int l = num; l < num2; l++)
			{
				num5 = 0;
				for (int m = num3; m < num4; m++)
				{
					if (!TileAt(l, m).Active || (int)TileAt(l, m).Type != type)
					{
						flag = true;
						break;
					}
					if ((int)(TileAt(l, m).FrameX / 18) != k + num8 * 2 || (int)(TileAt(l, m).FrameY / 18) != num5)
					{
						flag = true;
						break;
					}
					num5++;
				}
				k++;
			}
			if (!flag)
			{
				if (type == 85)
				{
					if (TileAt(num6, num7 + 2).Active && Main.tileSolid[(int)TileAt(num6, num7 + 2).Type] && TileAt(num6 + 1, num7 + 2).Active && Main.tileSolid[(int)TileAt(num6 + 1, num7 + 2).Type])
					{
						num8 = 0;
					}
					else
					{
						flag = true;
					}
				}
				else if (TileAt(num6, num7 + 2).Active && Main.tileSolid[(int)TileAt(num6, num7 + 2).Type] && TileAt(num6 + 1, num7 + 2).Active && Main.tileSolid[(int)TileAt(num6 + 1, num7 + 2).Type])
				{
					num8 = 0;
				}
				else if (TileAt(num6, num7 - 1).Active && Main.tileSolid[(int)TileAt(num6, num7 - 1).Type] && !Main.tileSolidTop[(int)TileAt(num6, num7 - 1).Type] && TileAt(num6 + 1, num7 - 1).Active && Main.tileSolid[(int)TileAt(num6 + 1, num7 - 1).Type] && !Main.tileSolidTop[(int)TileAt(num6 + 1, num7 - 1).Type])
				{
					num8 = 1;
				}
				else if (TileAt(num6 - 1, num7).Active && Main.tileSolid[(int)TileAt(num6 - 1, num7).Type] && !Main.tileSolidTop[(int)TileAt(num6 - 1, num7).Type] && TileAt(num6 - 1, num7 + 1).Active && Main.tileSolid[(int)TileAt(num6 - 1, num7 + 1).Type] && !Main.tileSolidTop[(int)TileAt(num6 - 1, num7 + 1).Type])
				{
					num8 = 2;
				}
				else if (TileAt(num6 + 2, num7).Active && Main.tileSolid[(int)TileAt(num6 + 2, num7).Type] && !Main.tileSolidTop[(int)TileAt(num6 + 2, num7).Type] && TileAt(num6 + 2, num7 + 1).Active && Main.tileSolid[(int)TileAt(num6 + 2, num7 + 1).Type] && !Main.tileSolidTop[(int)TileAt(num6 + 2, num7 + 1).Type])
				{
					num8 = 3;
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int n = num; n < num2; n++)
				{
					for (int num9 = num3; num9 < num4; num9++)
					{
						if ((int)TileAt(n, num9).Type == type)
						{
							KillTile(n, num9, false, false, false);
						}
					}
				}
				sandbox.KillSign(num6, num7);
				if (type == 85)
				{
					sandbox.NewItem(x * 16, y * 16, 32, 32, 321, 1, false);
				}
				else
				{
					sandbox.NewItem(x * 16, y * 16, 32, 32, 171, 1, false);
				}
				destroyObject = false;
				return;
			}
			int num10 = 36 * num8;
			for (int num11 = 0; num11 < 2; num11++)
			{
				for (int num12 = 0; num12 < 2; num12++)
				{
					TileAt(num6 + num11, num7 + num12).SetActive(true);
					TileAt(num6 + num11, num7 + num12).SetType((byte)type);
					TileAt(num6 + num11, num7 + num12).SetFrameX((short)(num10 + 18 * num11));
					TileAt(num6 + num11, num7 + num12).SetFrameY((short)(18 * num12));
				}
			}
		}

		public bool PlaceSign(int x, int y, int type)
		{
			int num = x - 2;
			int num2 = x + 3;
			int num3 = y - 2;
			int num4 = y + 3;
			if (num < 0)
			{
				return false;
			}
			if (num2 > Main.maxTilesX)
			{
				return false;
			}
			if (num3 < 0)
			{
				return false;
			}
			if (num4 > Main.maxTilesY)
			{
				return false;
			}
			int num5 = x;
			int num6 = y;
			int num7 = 0;
			if (type == 55)
			{
				if (TileAt(x, y + 1).Active && Main.tileSolid[(int)TileAt(x, y + 1).Type] && TileAt(x + 1, y + 1).Active && Main.tileSolid[(int)TileAt(x + 1, y + 1).Type])
				{
					num6--;
					num7 = 0;
				}
				else if (TileAt(x, y - 1).Active && Main.tileSolid[(int)TileAt(x, y - 1).Type] && !Main.tileSolidTop[(int)TileAt(x, y - 1).Type] && TileAt(x + 1, y - 1).Active && Main.tileSolid[(int)TileAt(x + 1, y - 1).Type] && !Main.tileSolidTop[(int)TileAt(x + 1, y - 1).Type])
				{
					num7 = 1;
				}
				else if (TileAt(x - 1, y).Active && Main.tileSolid[(int)TileAt(x - 1, y).Type] && !Main.tileSolidTop[(int)TileAt(x - 1, y).Type] && TileAt(x - 1, y + 1).Active && Main.tileSolid[(int)TileAt(x - 1, y + 1).Type] && !Main.tileSolidTop[(int)TileAt(x - 1, y + 1).Type])
				{
					num7 = 2;
				}
				else
				{
					if (!TileAt(x + 1, y).Active || !Main.tileSolid[(int)TileAt(x + 1, y).Type] || Main.tileSolidTop[(int)TileAt(x + 1, y).Type] || !TileAt(x + 1, y + 1).Active || !Main.tileSolid[(int)TileAt(x + 1, y + 1).Type] || Main.tileSolidTop[(int)TileAt(x + 1, y + 1).Type])
					{
						return false;
					}
					num5--;
					num7 = 3;
				}
			}
			else if (type == 85)
			{
				if (!TileAt(x, y + 1).Active || !Main.tileSolid[(int)TileAt(x, y + 1).Type] || !TileAt(x + 1, y + 1).Active || !Main.tileSolid[(int)TileAt(x + 1, y + 1).Type])
				{
					return false;
				}
				num6--;
				num7 = 0;
			}
			if (TileAt(num5, num6).Active || TileAt(num5 + 1, num6).Active || TileAt(num5, num6 + 1).Active || TileAt(num5 + 1, num6 + 1).Active)
			{
				return false;
			}
			int num8 = 36 * num7;
			for (int k = 0; k < 2; k++)
			{
				for (int l = 0; l < 2; l++)
				{
					TileAt(num5 + k, num6 + l).SetActive(true);
					TileAt(num5 + k, num6 + l).SetType((byte)type);
					TileAt(num5 + k, num6 + l).SetFrameX((short)(num8 + 18 * k));
					TileAt(num5 + k, num6 + l).SetFrameY((short)(18 * l));
				}
			}
			return true;
		}

		public void PlaceOnTable1x1(int x, int y, int type, int style = 0)
		{
			bool flag = false;
			if (!TileAt(x, y).Active && TileAt(x, y + 1).Active && Main.tileTable[(int)TileAt(x, y + 1).Type])
			{
				flag = true;
			}
			if (type == 78 && !TileAt(x, y).Active && TileAt(x, y + 1).Active && Main.tileSolid[(int)TileAt(x, y + 1).Type])
			{
				flag = true;
			}
			if (flag)
			{
				TileAt(x, y).SetActive(true);
				TileAt(x, y).SetFrameX((short)(style * 18));
				TileAt(x, y).SetFrameY(0);
				TileAt(x, y).SetType((byte)type);
				if (type == 50)
				{
					TileAt(x, y).SetFrameX((short)(18 * genRand.Next(5)));
				}
			}
		}

		public bool PlaceAlch(int x, int y, int style)
		{
			if (!TileAt(x, y).Active && TileAt(x, y + 1).Active)
			{
				bool flag = false;
				if (style == 0)
				{
					if (TileAt(x, y + 1).Type != 2 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0)
					{
						flag = true;
					}
				}
				else if (style == 1)
				{
					if (TileAt(x, y + 1).Type != 60 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0)
					{
						flag = true;
					}
				}
				else if (style == 2)
				{
					if (TileAt(x, y + 1).Type != 0 && TileAt(x, y + 1).Type != 59 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0)
					{
						flag = true;
					}
				}
				else if (style == 3)
				{
					if (TileAt(x, y + 1).Type != 23 && TileAt(x, y + 1).Type != 25 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0)
					{
						flag = true;
					}
				}
				else if (style == 4)
				{
					if (TileAt(x, y + 1).Type != 53 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && TileAt(x, y).Lava)
					{
						flag = true;
					}
				}
				else if (style == 5)
				{
					if (TileAt(x, y + 1).Type != 57 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && !TileAt(x, y).Lava)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					TileAt(x, y).SetActive(true);
					TileAt(x, y).SetType(82);
					TileAt(x, y).SetFrameX((short)(18 * style));
					TileAt(x, y).SetFrameY(0);
					return true;
				}
			}
			return false;
		}

		public void GrowAlch(int x, int y)
		{
			if (TileAt(x, y).Active)
			{
				if (TileAt(x, y).Type == 82 && genRand.Next(50) == 0)
				{
					TileAt(x, y).SetType(83);

					SquareTileFrame(x, y, true);
					return;
				}
				if (TileAt(x, y).FrameX == 36)
				{
					if (TileAt(x, y).Type == 83)
					{
						TileAt(x, y).SetType(84);
					}
					else
					{
						TileAt(x, y).SetType(83);
					}
				}
			}
		}

		public void PlantAlch()
		{
			int num = genRand.Next(20, Main.maxTilesX - 20);
			int num2 = 0;
			if (genRand.Next(40) == 0)
			{
				num2 = genRand.Next((int)(Main.rockLayer + (double)Main.maxTilesY) / 2, Main.maxTilesY - 20);
			}
			else if (genRand.Next(10) == 0)
			{
				num2 = genRand.Next(0, Main.maxTilesY - 20);
			}
			else
			{
				num2 = genRand.Next((int)Main.worldSurface, Main.maxTilesY - 20);
			}
			while (num2 < Main.maxTilesY - 20 && !TileAt(num, num2).Active)
			{
				num2++;
			}
			if (TileAt(num, num2).Active && !TileAt(num, num2 - 1).Active && TileAt(num, num2 - 1).Liquid == 0)
			{
				if (TileAt(num, num2).Type == 2)
				{
					PlaceAlch(num, num2 - 1, 0);
				}
				if (TileAt(num, num2).Type == 60)
				{
					PlaceAlch(num, num2 - 1, 1);
				}
				if (TileAt(num, num2).Type == 0 || TileAt(num, num2).Type == 59)
				{
					PlaceAlch(num, num2 - 1, 2);
				}
				if (TileAt(num, num2).Type == 23 || TileAt(num, num2).Type == 25)
				{
					PlaceAlch(num, num2 - 1, 3);
				}
				if (TileAt(num, num2).Type == 53)
				{
					PlaceAlch(num, num2 - 1, 4);
				}
				if (TileAt(num, num2).Type == 57)
				{
					PlaceAlch(num, num2 - 1, 5);
				}
			}
		}

		public void CheckAlch(int x, int y)
		{
			bool flag = false;
			if (!TileAt(x, y + 1).Active)
			{
				flag = true;
			}
			int num = (int)(TileAt(x, y).FrameX / 18);
			TileAt(x, y).SetFrameY(0);
			if (!flag)
			{
				if (num == 0)
				{
					if (TileAt(x, y + 1).Type != 2 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && TileAt(x, y).Lava)
					{
						flag = true;
					}
				}
				else if (num == 1)
				{
					if (TileAt(x, y + 1).Type != 60 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && TileAt(x, y).Lava)
					{
						flag = true;
					}
				}
				else if (num == 2)
				{
					if (TileAt(x, y + 1).Type != 0 && TileAt(x, y + 1).Type != 59 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && TileAt(x, y).Lava)
					{
						flag = true;
					}
				}
				else if (num == 3)
				{
					if (TileAt(x, y + 1).Type != 23 && TileAt(x, y + 1).Type != 25 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && TileAt(x, y).Lava)
					{
						flag = true;
					}
				}
				else if (num == 4)
				{
					if (TileAt(x, y + 1).Type != 53 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && TileAt(x, y).Lava)
					{
						flag = true;
					}
					if (TileAt(x, y).Type != 82 && !TileAt(x, y).Lava)
					{
						if (TileAt(x, y).Liquid > 16)
						{
							if (TileAt(x, y).Type == 83)
							{
								TileAt(x, y).SetType(84);
							}
						}
						else if (TileAt(x, y).Type == 84)
						{
							TileAt(x, y).SetType(83);
						}
					}
				}
				else if (num == 5)
				{
					if (TileAt(x, y + 1).Type != 57 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && !TileAt(x, y).Lava)
					{
						flag = true;
					}
					if (TileAt(x, y).Type != 82 && TileAt(x, y).Lava && TileAt(x, y).Type != 82 && TileAt(x, y).Lava)
					{
						if (TileAt(x, y).Liquid > 16)
						{
							if (TileAt(x, y).Type == 83)
							{
								TileAt(x, y).SetType(84);
							}
						}
						else if (TileAt(x, y).Type == 84)
						{
							TileAt(x, y).SetType(83);
						}
					}
				}
			}
			if (flag)
			{
				KillTile(x, y, false, false, false);
			}
		}

		public void Place1x2(int x, int y, int type, int style)
		{
			short frameX = 0;
			if (type == 20)
			{
				frameX = (short)(genRand.Next(3) * 18);
			}
			if (TileAt(x, y + 1).Active && Main.tileSolid[(int)TileAt(x, y + 1).Type] && !TileAt(x, y - 1).Active)
			{
				short frameHeight = (short)(style * 40);
				TileAt(x, y - 1).SetActive(true);
				TileAt(x, y - 1).SetFrameY(frameHeight);
				TileAt(x, y - 1).SetFrameX(frameX);
				TileAt(x, y - 1).SetType((byte)type);
				TileAt(x, y).SetActive(true);
				TileAt(x, y).SetFrameY((short)(frameHeight + 18));
				TileAt(x, y).SetFrameX(frameX);
				TileAt(x, y).SetType((byte)type);
			}
		}

		public void PlaceBanner(int x, int y, int type, int style = 0)
		{
			int FrameLength = style * 18;
			if (TileAt(x, y - 1).Active && Main.tileSolid[(int)TileAt(x, y - 1).Type] && !Main.tileSolidTop[(int)TileAt(x, y - 1).Type] && !TileAt(x, y).Active && !TileAt(x, y + 1).Active && !TileAt(x, y + 2).Active)
			{
				TileAt(x, y).SetActive(true);
				TileAt(x, y).SetFrameY(0);
				TileAt(x, y).SetFrameX((short)FrameLength);
				TileAt(x, y).SetType((byte)type);
				TileAt(x, y + 1).SetActive(true);
				TileAt(x, y + 1).SetFrameY(18);
				TileAt(x, y + 1).SetFrameX((short)FrameLength);
				TileAt(x, y + 1).SetType((byte)type);
				TileAt(x, y + 2).SetActive(true);
				TileAt(x, y + 2).SetFrameY(36);
				TileAt(x, y + 2).SetFrameX((short)FrameLength);
				TileAt(x, y + 2).SetType((byte)type);
			}
		}

		public void CheckBanner(int x, int j, byte type)
		{
			if (destroyObject)
			{
				return;
			}
			int num = j - (int)(TileAt(x, j).FrameY / 18);
			int frameX = (int)TileAt(x, j).FrameX;
			bool flag = false;
			for (int i = 0; i < 3; i++)
			{
				if (!TileAt(x, num + i).Active)
				{
					flag = true;
				}
				else
				{
					if (TileAt(x, num + i).Type != type)
					{
						flag = true;
					}
					else
					{
						if ((int)TileAt(x, num + i).FrameY != i * 18)
						{
							flag = true;
						}
						else
						{
							if ((int)TileAt(x, num + i).FrameX != frameX)
							{
								flag = true;
							}
						}
					}
				}
			}
			if (!TileAt(x, num - 1).Active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)TileAt(x, num - 1).Type])
			{
				flag = true;
			}
			if (Main.tileSolidTop[(int)TileAt(x, num - 1).Type])
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				for (int k = 0; k < 3; k++)
				{
					if (TileAt(x, num + k).Type == type)
					{
						KillTile(x, num + k, false, false, false);
					}
				}
				if (type == 91)
				{
					int num2 = frameX / 18;
					sandbox.NewItem(x * 16, (num + 1) * 16, 32, 32, 337 + num2, 1, false);
				}
				destroyObject = false;
			}
		}

		public void Place1x2Top(int x, int y, int type)
		{
			short frameX = 0;
			if (TileAt(x, y - 1).Active && Main.tileSolid[(int)TileAt(x, y - 1).Type] && !Main.tileSolidTop[(int)TileAt(x, y - 1).Type] && !TileAt(x, y + 1).Active)
			{
				TileAt(x, y).SetActive(true);
				TileAt(x, y).SetFrameY(0);
				TileAt(x, y).SetFrameX(frameX);
				TileAt(x, y).SetType((byte)type);
				TileAt(x, y + 1).SetActive(true);
				TileAt(x, y + 1).SetFrameY(18);
				TileAt(x, y + 1).SetFrameX(frameX);
				TileAt(x, y + 1).SetType((byte)type);
			}
		}

		private TRef GetTile(int x, int y)
		{
			return TileAt(x, y);
		}

		public void Check1x2Top(int x, int y, byte type)
		{
			if (destroyObject)
			{
				return;
			}

			bool flag = true;

			if (TileAt(x, y).FrameY == 18)
			{
				y--;
			}

			if (TileAt(x, y).FrameY == 0
				&& TileAt(x, y + 1).FrameY == 18
				&& TileAt(x, y).Type == type
				&& TileAt(x, y + 1).Type == type)
			{
				flag = false;
			}

			if (!TileAt(x, y - 1).Active
				|| !Main.tileSolid[(int)TileAt(x, y - 1).Type]
				|| Main.tileSolidTop[(int)TileAt(x, y - 1).Type])
			{
				flag = true;
			}

			if (flag)
			{
				destroyObject = true;
				if (TileAt(x, y).Type == type)
				{
					KillTile(x, y, false, false, false);
				}
				if (TileAt(x, y + 1).Type == type)
				{
					KillTile(x, y + 1, false, false, false);
				}
				if (type == 42)
				{
					sandbox.NewItem(x * 16, y * 16, 32, 32, 136, 1, false);
				}
				destroyObject = false;
			}
		}

		public void Check2x1(int x, int y, byte type)
		{
			if (destroyObject)
			{
				return;
			}

			bool flag = true;

			if (TileAt(x, y).FrameX == 18)
			{
				x--;
			}

			if (TileAt(x, y).FrameX == 0
				&& TileAt(x + 1, y).FrameX == 18
				&& TileAt(x, y).Type == type
				&& TileAt(x + 1, y).Type == type)
			{
				flag = false;
			}

			if (type == 29 || type == 103)
			{
				if (!TileAt(x, y + 1).Active || !Main.tileTable[(int)TileAt(x, y + 1).Type])
				{
					flag = true;
				}
				if (!TileAt(x + 1, y + 1).Active || !Main.tileTable[(int)TileAt(x + 1, y + 1).Type])
				{
					flag = true;
				}
			}
			else
			{
				if (!TileAt(x, y + 1).Active || !Main.tileSolid[(int)TileAt(x, y + 1).Type])
				{
					flag = true;
				}
				if (!TileAt(x + 1, y + 1).Active || !Main.tileSolid[(int)TileAt(x + 1, y + 1).Type])
				{
					flag = true;
				}
			}

			if (flag)
			{
				destroyObject = true;
				if (TileAt(x, y).Type == type)
				{
					KillTile(x, y, false, false, false);
				}
				if (TileAt(x + 1, y).Type == type)
				{
					KillTile(x + 1, y, false, false, false);
				}
				if (type == 16)
				{
					sandbox.NewItem(x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 35, 1, false);
				}
				if (type == 18)
				{
					sandbox.NewItem(x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 36, 1, false);
				}
				if (type == 29)
				{
					sandbox.NewItem(x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 87, 1, false);
				}
				if (type == 103)
				{
					sandbox.NewItem(x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 356, 1, false);
				}
				destroyObject = false;
				SquareTileFrame(x, y, true);
				SquareTileFrame(x + 1, y, true);
			}
		}

		public void Place2x1(int x, int y, int type)
		{
			bool flag = false;
			if (type != 29 && type != 103
				&& TileAt(x, y + 1).Active
				&& TileAt(x + 1, y + 1).Active
				&& Main.tileSolid[(int)TileAt(x, y + 1).Type]
				&& Main.tileSolid[(int)TileAt(x + 1, y + 1).Type]
				&& !TileAt(x, y).Active
				&& !TileAt(x + 1, y).Active)
			{
				flag = true;
			}
			else
			{
				if ((type == 29 || type == 103)
					&& TileAt(x, y + 1).Active
					&& TileAt(x + 1, y + 1).Active
					&& Main.tileTable[(int)TileAt(x, y + 1).Type]
					&& Main.tileTable[(int)TileAt(x + 1, y + 1).Type]
					&& !TileAt(x, y).Active
					&& !TileAt(x + 1, y).Active)
				{
					flag = true;
				}
			}

			if (flag)
			{
				TileAt(x, y).SetActive(true);
				TileAt(x, y).SetFrameY(0);
				TileAt(x, y).SetFrameX(0);
				TileAt(x, y).SetType((byte)type);
				TileAt(x + 1, y).SetActive(true);
				TileAt(x + 1, y).SetFrameY(0);
				TileAt(x + 1, y).SetFrameX(18);
				TileAt(x + 1, y).SetType((byte)type);
			}
		}

		public void Check4x2(int i, int j, int type)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i;
			num += (int)(TileAt(i, j).FrameX / 18 * -1);
			if ((type == 79 || type == 90) && TileAt(i, j).FrameX >= 72)
			{
				num += 4;
			}
			int num2 = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 4; k++)
			{
				for (int l = num2; l < num2 + 2; l++)
				{
					int num3 = (k - num) * 18;
					if ((type == 79 || type == 90) && TileAt(i, j).FrameX >= 72)
					{
						num3 = (k - num + 4) * 18;
					}
					if (!TileAt(k, l).Active || (int)TileAt(k, l).Type != type || (int)TileAt(k, l).FrameX != num3 || (int)TileAt(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (!TileAt(k, num2 + 2).Active || !Main.tileSolid[(int)TileAt(k, num2 + 2).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int m = num; m < num + 4; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)TileAt(m, n).Type == type && TileAt(m, n).Active)
						{
							KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 79)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 224, 1, false);
				}
				if (type == 90)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 336, 1, false);
				}
				destroyObject = false;
				for (int num4 = num - 1; num4 < num + 4; num4++)
				{
					for (int num5 = num2 - 1; num5 < num2 + 4; num5++)
					{
						TileFrame(num4, num5, false, false);
					}
				}
			}
		}

		public void Check2x2(int i, int j, int type)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i + (int)(TileAt(i, j).FrameX / 18 * -1);
			int num2 = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 2; k++)
			{
				for (int l = num2; l < num2 + 2; l++)
				{
					if (!TileAt(k, l).Active || (int)TileAt(k, l).Type != type || (int)TileAt(k, l).FrameX != (k - num) * 18 || (int)TileAt(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (type == 95)
				{
					if (!TileAt(k, num2 - 1).Active ||
						!Main.tileSolid[(int)TileAt(k, num2 - 1).Type] ||
						Main.tileSolidTop[(int)TileAt(k, num2 - 1).Type])
					{
						flag = true;
					}
				}
				else
				{
					if (!TileAt(k, num2 + 2).Active ||
						(!Main.tileSolid[(int)TileAt(k, num2 + 2).Type] &&
						!Main.tileTable[(int)TileAt(k, num2 + 2).Type]))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int m = num; m < num + 2; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)TileAt(m, n).Type == type && TileAt(m, n).Active)
						{
							KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 85)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 321, 1, false);
				}
				if (type == 94)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 352, 1, false);
				}
				if (type == 95)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 344, 1, false);
				}
				if (type == 96)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 345, 1, false);
				}
				if (type == 97)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 346, 1, false);
				}
				if (type == 98)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 347, 1, false);
				}
				if (type == 99)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 348, 1, false);
				}
				if (type == 100)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 349, 1, false);
				}
				destroyObject = false;
				for (int num3 = num - 1; num3 < num + 3; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 3; num4++)
					{
						TileFrame(num3, num4, false, false);
					}
				}
			}
		}

		public void Check3x2(int i, int j, int type)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i + (int)(TileAt(i, j).FrameX / 18 * -1);
			int num2 = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num2; l < num2 + 2; l++)
				{
					if (!TileAt(k, l).Active || (int)TileAt(k, l).Type != type || (int)TileAt(k, l).FrameX != (k - num) * 18 || (int)TileAt(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (!TileAt(k, num2 + 2).Active || !Main.tileSolid[(int)TileAt(k, num2 + 2).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)TileAt(m, n).Type == type && TileAt(m, n).Active)
						{
							KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 14)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 32, 1, false);
				}
				else if (type == 17)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 33, 1, false);
				}
				else if (type == 77)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 221, 1, false);
				}
				else if (type == 86)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 332, 1, false);
				}
				else if (type == 87)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 333, 1, false);
				}
				else if (type == 88)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 334, 1, false);
				}
				else if (type == 89)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 335, 1, false);
				}
				destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						TileFrame(num3, num4, false, false);
					}
				}
			}
		}

		public void Place4x2(int x, int y, int type, int direction = -1)
		{
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = x - 1; i < x + 3; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileAt(i, j).Active)
					{
						flag = false;
					}
				}
				if (!TileAt(i, y + 1).Active || !Main.tileSolid[(int)TileAt(i, y + 1).Type])
				{
					flag = false;
				}
			}
			short num = 0;
			if (direction == 1)
			{
				num = 72;
			}
			if (flag)
			{
				TileAt(x - 1, y - 1).SetActive(true);
				TileAt(x - 1, y - 1).SetFrameY(0);
				TileAt(x - 1, y - 1).SetFrameX(num);
				TileAt(x - 1, y - 1).SetType((byte)type);
				TileAt(x, y - 1).SetActive(true);
				TileAt(x, y - 1).SetFrameY(0);
				TileAt(x, y - 1).SetFrameX((short)(18 + num));
				TileAt(x, y - 1).SetType((byte)type);
				TileAt(x + 1, y - 1).SetActive(true);
				TileAt(x + 1, y - 1).SetFrameY(0);
				TileAt(x + 1, y - 1).SetFrameX((short)(36 + num));
				TileAt(x + 1, y - 1).SetType((byte)type);
				TileAt(x + 2, y - 1).SetActive(true);
				TileAt(x + 2, y - 1).SetFrameY(0);
				TileAt(x + 2, y - 1).SetFrameX((short)(54 + num));
				TileAt(x + 2, y - 1).SetType((byte)type);
				TileAt(x - 1, y).SetActive(true);
				TileAt(x - 1, y).SetFrameY(18);
				TileAt(x - 1, y).SetFrameX(num);
				TileAt(x - 1, y).SetType((byte)type);
				TileAt(x, y).SetActive(true);
				TileAt(x, y).SetFrameY(18);
				TileAt(x, y).SetFrameX((short)(18 + num));
				TileAt(x, y).SetType((byte)type);
				TileAt(x + 1, y).SetActive(true);
				TileAt(x + 1, y).SetFrameY(18);
				TileAt(x + 1, y).SetFrameX((short)(36 + num));
				TileAt(x + 1, y).SetType((byte)type);
				TileAt(x + 2, y).SetActive(true);
				TileAt(x + 2, y).SetFrameY(18);
				TileAt(x + 2, y).SetFrameX((short)(54 + num));
				TileAt(x + 2, y).SetType((byte)type);
			}
		}

		public void Place2x2(int x, int superY, int type)
		{
			int y = superY;
			if (type == 95)
			{
				y++;
			}
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = x - 1; i < x + 1; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileAt(i, j).Active)
					{
						flag = false;
					}
					if (type == 98 && TileAt(i, j).Liquid > 0)
					{
						flag = false;
					}
				}
				if (type == 95)
				{
					if (!TileAt(i, y - 2).Active ||
						!Main.tileSolid[(int)TileAt(i, y - 2).Type] ||
						Main.tileSolidTop[(int)TileAt(i, y - 2).Type])
					{
						flag = false;
					}
				}
				else
				{
					if (!TileAt(i, y + 1).Active || (
						!Main.tileSolid[(int)TileAt(i, y + 1).Type] &&
						!Main.tileTable[(int)TileAt(i, y + 1).Type]))
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				TileAt(x - 1, y - 1).SetActive(true);
				TileAt(x - 1, y - 1).SetFrameY(0);
				TileAt(x - 1, y - 1).SetFrameX(0);
				TileAt(x - 1, y - 1).SetType((byte)type);
				TileAt(x, y - 1).SetActive(true);
				TileAt(x, y - 1).SetFrameY(0);
				TileAt(x, y - 1).SetFrameX(18);
				TileAt(x, y - 1).SetType((byte)type);
				TileAt(x - 1, y).SetActive(true);
				TileAt(x - 1, y).SetFrameY(18);
				TileAt(x - 1, y).SetFrameX(0);
				TileAt(x - 1, y).SetType((byte)type);
				TileAt(x, y).SetActive(true);
				TileAt(x, y).SetFrameY(18);
				TileAt(x, y).SetFrameX(18);
				TileAt(x, y).SetType((byte)type);
			}
		}

		public void Place3x2(int x, int y, int type)
		{
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = x - 1; i < x + 2; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileAt(i, j).Active)
					{
						flag = false;
					}
				}
				if (!TileAt(i, y + 1).Active || !Main.tileSolid[(int)TileAt(i, y + 1).Type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				TileAt(x - 1, y - 1).SetActive(true);
				TileAt(x - 1, y - 1).SetFrameY(0);
				TileAt(x - 1, y - 1).SetFrameX(0);
				TileAt(x - 1, y - 1).SetType((byte)type);
				TileAt(x, y - 1).SetActive(true);
				TileAt(x, y - 1).SetFrameY(0);
				TileAt(x, y - 1).SetFrameX(18);
				TileAt(x, y - 1).SetType((byte)type);
				TileAt(x + 1, y - 1).SetActive(true);
				TileAt(x + 1, y - 1).SetFrameY(0);
				TileAt(x + 1, y - 1).SetFrameX(36);
				TileAt(x + 1, y - 1).SetType((byte)type);
				TileAt(x - 1, y).SetActive(true);
				TileAt(x - 1, y).SetFrameY(18);
				TileAt(x - 1, y).SetFrameX(0);
				TileAt(x - 1, y).SetType((byte)type);
				TileAt(x, y).SetActive(true);
				TileAt(x, y).SetFrameY(18);
				TileAt(x, y).SetFrameX(18);
				TileAt(x, y).SetType((byte)type);
				TileAt(x + 1, y).SetActive(true);
				TileAt(x + 1, y).SetFrameY(18);
				TileAt(x + 1, y).SetFrameX(36);
				TileAt(x + 1, y).SetType((byte)type);
			}
		}

		public void Check3x3(int i, int j, int type)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i + (int)(TileAt(i, j).FrameX / 18 * -1);
			int num2 = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num2; l < num2 + 3; l++)
				{
					if (!TileAt(k, l).Active || (int)TileAt(k, l).Type != type || (int)TileAt(k, l).FrameX != (k - num) * 18 || (int)TileAt(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
			}
			if (type == 106)
			{
				for (int m = num; m < num + 3; m++)
				{
					if (!TileAt(m, num2 + 3).Active || !Main.tileSolid[(int)TileAt(m, num2 + 3).Type])
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				if (!TileAt(num + 1, num2 - 1).Active || !Main.tileSolid[(int)TileAt(num + 1, num2 - 1).Type] || Main.tileSolidTop[(int)TileAt(num + 1, num2 - 1).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)TileAt(m, n).Type == type && TileAt(m, n).Active)
						{
							KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 34)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 106, 1, false);
				}
				else if (type == 35)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 107, 1, false);
				}
				else if (type == 36)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 108, 1, false);
				}
				else
				{
					if (type == 106)
					{
						sandbox.NewItem(i * 16, j * 16, 32, 32, 363, 1, false);
					}
				}
				destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						TileFrame(num3, num4, false, false);
					}
				}
			}
		}

		public void Place3x3(int x, int y, int type)
		{
			bool flag = true;
			int num = 0;
			if (type == 106)
			{
				num = -2;
				for (int i = x - 1; i < x + 2; i++)
				{
					for (int j = y - 2; j < y + 1; j++)
					{
						if (TileAt(i, j).Active)
						{
							flag = false;
						}
					}
				}
				for (int k = x - 1; k < x + 2; k++)
				{
					if (!TileAt(k, y + 1).Active || !Main.tileSolid[(int)TileAt(k, y + 1).Type])
					{
						flag = false;
						break;
					}
				}
			}
			else
			{
				for (int l = x - 1; l < x + 2; l++)
				{
					for (int m = y; m < y + 3; m++)
					{
						if (TileAt(l, m).Active)
						{
							flag = false;
						}
					}
				}
				if (!TileAt(x, y - 1).Active || !Main.tileSolid[(int)TileAt(x, y - 1).Type] || Main.tileSolidTop[(int)TileAt(x, y - 1).Type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				TileAt(x - 1, y + num).SetActive(true);
				TileAt(x - 1, y + num).SetFrameY(0);
				TileAt(x - 1, y + num).SetFrameX(0);
				TileAt(x - 1, y + num).SetType((byte)type);
				TileAt(x, y + num).SetActive(true);
				TileAt(x, y + num).SetFrameY(0);
				TileAt(x, y + num).SetFrameX(18);
				TileAt(x, y + num).SetType((byte)type);
				TileAt(x + 1, y + num).SetActive(true);
				TileAt(x + 1, y + num).SetFrameY(0);
				TileAt(x + 1, y + num).SetFrameX(36);
				TileAt(x + 1, y + num).SetType((byte)type);
				TileAt(x - 1, y + 1 + num).SetActive(true);
				TileAt(x - 1, y + 1 + num).SetFrameY(18);
				TileAt(x - 1, y + 1 + num).SetFrameX(0);
				TileAt(x - 1, y + 1 + num).SetType((byte)type);
				TileAt(x, y + 1 + num).SetActive(true);
				TileAt(x, y + 1 + num).SetFrameY(18);
				TileAt(x, y + 1 + num).SetFrameX(18);
				TileAt(x, y + 1 + num).SetType((byte)type);
				TileAt(x + 1, y + 1 + num).SetActive(true);
				TileAt(x + 1, y + 1 + num).SetFrameY(18);
				TileAt(x + 1, y + 1 + num).SetFrameX(36);
				TileAt(x + 1, y + 1 + num).SetType((byte)type);
				TileAt(x - 1, y + 2 + num).SetActive(true);
				TileAt(x - 1, y + 2 + num).SetFrameY(36);
				TileAt(x - 1, y + 2 + num).SetFrameX(0);
				TileAt(x - 1, y + 2 + num).SetType((byte)type);
				TileAt(x, y + 2 + num).SetActive(true);
				TileAt(x, y + 2 + num).SetFrameY(36);
				TileAt(x, y + 2 + num).SetFrameX(18);
				TileAt(x, y + 2 + num).SetType((byte)type);
				TileAt(x + 1, y + 2 + num).SetActive(true);
				TileAt(x + 1, y + 2 + num).SetFrameY(36);
				TileAt(x + 1, y + 2 + num).SetFrameX(36);
				TileAt(x + 1, y + 2 + num).SetType((byte)type);
			}
		}

		public void Check3x4(int i, int j, int type)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i + (int)(TileAt(i, j).FrameX / 18 * -1);
			int num2 = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num2; l < num2 + 4; l++)
				{
					if (!TileAt(k, l).Active || (int)TileAt(k, l).Type != type ||
						(int)TileAt(k, l).FrameX != (k - num) * 18 || (int)TileAt(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (!TileAt(k, num2 + 4).Active || !Main.tileSolid[(int)TileAt(k, num2 + 4).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 4; n++)
					{
						if ((int)TileAt(m, n).Type == type && TileAt(m, n).Active)
						{
							KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 101)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 354, 1, false);
				}
				else
				{
					if (type == 102)
					{
						sandbox.NewItem(i * 16, j * 16, 32, 32, 355, 1, false);
					}
				}
				destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						TileFrame(num3, num4, false, false);
					}
				}
			}
		}

		public void Check1xX(int x, int j, byte type)
		{
			if (destroyObject)
			{
				return;
			}
			int num = j - (int)(TileAt(x, j).FrameY / 18);
			int frameX = (int)TileAt(x, j).FrameX;
			int num2 = 3;
			if (type == 92)
			{
				num2 = 6;
			}
			bool flag = false;
			for (int i = 0; i < num2; i++)
			{
				if (!TileAt(x, num + i).Active)
				{
					flag = true;
				}
				else
				{
					if (TileAt(x, num + i).Type != type)
					{
						flag = true;
					}
					else
					{
						if ((int)TileAt(x, num + i).FrameY != i * 18)
						{
							flag = true;
						}
						else
						{
							if ((int)TileAt(x, num + i).FrameX != frameX)
							{
								flag = true;
							}
						}
					}
				}
			}
			if (!TileAt(x, num + num2).Active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)TileAt(x, num + num2).Type])
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				for (int k = 0; k < num2; k++)
				{
					if (TileAt(x, num + k).Type == type)
					{
						KillTile(x, num + k, false, false, false);
					}
				}
				if (type == 92)
				{
					sandbox.NewItem(x * 16, j * 16, 32, 32, 341, 1, false);
				}
				if (type == 93)
				{
					sandbox.NewItem(x * 16, j * 16, 32, 32, 342, 1, false);
				}
				destroyObject = false;
			}
		}

		public void Check2xX(int i, int j, byte type)
		{
			if (destroyObject)
			{
				return;
			}
			int num = i;
			if (TileAt(i, j).FrameX == 18)
			{
				num--;
			}
			int num2 = j - (int)(TileAt(num, j).FrameY / 18);
			int frameX = (int)TileAt(num, num2).FrameX;
			int num3 = 3;
			if (type == 104)
			{
				num3 = 5;
			}
			bool flag = false;
			for (int k = 0; k < num3; k++)
			{
				if (!TileAt(num, num2 + k).Active)
				{
					flag = true;
				}
				else
				{
					if (TileAt(num, num2 + k).Type != type)
					{
						flag = true;
					}
					else
					{
						if ((int)TileAt(num, num2 + k).FrameY != k * 18)
						{
							flag = true;
						}
						else
						{
							if ((int)TileAt(num, num2 + k).FrameX != frameX)
							{
								flag = true;
							}
						}
					}
				}
				if (!TileAt(num + 1, num2 + k).Active)
				{
					flag = true;
				}
				else
				{
					if (TileAt(num + 1, num2 + k).Type != type)
					{
						flag = true;
					}
					else
					{
						if ((int)TileAt(num + 1, num2 + k).FrameY != k * 18)
						{
							flag = true;
						}
						else
						{
							if ((int)TileAt(num + 1, num2 + k).FrameX != frameX + 18)
							{
								flag = true;
							}
						}
					}
				}
			}
			if (!TileAt(num, num2 + num3).Active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)TileAt(num, num2 + num3).Type])
			{
				flag = true;
			}
			if (!TileAt(num + 1, num2 + num3).Active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)TileAt(num + 1, num2 + num3).Type])
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				for (int l = 0; l < num3; l++)
				{
					if (TileAt(num, num2 + l).Type == type)
					{
						KillTile(num, num2 + l, false, false, false);
					}
					if (TileAt(num + 1, num2 + l).Type == type)
					{
						KillTile(num + 1, num2 + l, false, false, false);
					}
				}
				if (type == 104)
				{
					sandbox.NewItem(num * 16, j * 16, 32, 32, 359, 1, false);
				}
				if (type == 105)
				{
					sandbox.NewItem(num * 16, j * 16, 32, 32, 360, 1, false);
				}
				destroyObject = false;
			}
		}

		public void PlaceSunflower(int x, int y, int type = 27)
		{
			if ((double)y > Main.worldSurface - 1.0)
			{
				return;
			}
			bool flag = true;
			for (int i = x; i < x + 2; i++)
			{
				for (int j = y - 3; j < y + 1; j++)
				{
					if (TileAt(i, j).Active || TileAt(i, j).Wall > 0)
					{
						flag = false;
					}
				}
				if (!TileAt(i, y + 1).Active || TileAt(i, y + 1).Type != 2)
				{
					flag = false;
				}
			}
			if (flag)
			{
				for (int k = 0; k < 2; k++)
				{
					for (int l = -3; l < 1; l++)
					{
						int num = k * 18 + genRand.Next(3) * 36;
						int num2 = (l + 3) * 18;
						TileAt(x + k, y + l).SetActive(true);
						TileAt(x + k, y + l).SetFrameX((short)num);
						TileAt(x + k, y + l).SetFrameY((short)num2);
						TileAt(x + k, y + l).SetType((byte)type);
					}
				}
			}
		}

		public void CheckSunflower(int i, int j, int type = 27)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int k = 0;
			k += (int)(TileAt(i, j).FrameX / 18);
			int num = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			while (k > 1)
			{
				k -= 2;
			}
			k *= -1;
			k += i;
			for (int l = k; l < k + 2; l++)
			{
				for (int m = num; m < num + 4; m++)
				{
					int n;
					for (n = (int)(TileAt(l, m).FrameX / 18); n > 1; n -= 2)
					{
					}
					if (!TileAt(l, m).Active || (int)TileAt(l, m).Type != type || n != l - k || (int)TileAt(l, m).FrameY != (m - num) * 18)
					{
						flag = true;
					}
				}
				if (!TileAt(l, num + 4).Active || TileAt(l, num + 4).Type != 2)
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 4; num3++)
					{
						if ((int)TileAt(num2, num3).Type == type && TileAt(num2, num3).Active)
						{
							KillTile(num2, num3, false, false, false);
						}
					}
				}
				sandbox.NewItem(i * 16, j * 16, 32, 32, 63, 1, false);
				destroyObject = false;
			}
		}

		public bool PlacePot(int x, int y, int type = 28)
		{
			bool flag = true;
			for (int i = x; i < x + 2; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileAt(i, j).Active)
					{
						flag = false;
					}
				}
				if (!TileAt(i, y + 1).Active || !Main.tileSolid[(int)TileAt(i, y + 1).Type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				for (int k = 0; k < 2; k++)
				{
					for (int l = -1; l < 1; l++)
					{
						int num = k * 18 + genRand.Next(3) * 36;
						int num2 = (l + 1) * 18;
						TileAt(x + k, y + l).SetActive(true);
						TileAt(x + k, y + l).SetFrameX((short)num);
						TileAt(x + k, y + l).SetFrameY((short)num2);
						TileAt(x + k, y + l).SetType((byte)type);
					}
				}
				return true;
			}
			return false;
		}

		public bool CheckCactus(int i, int j)
		{
			int num = j;
			int num2 = i;
			while (TileAt(num2, num).Active && TileAt(num2, num).Type == 80)
			{
				num++;
				if (!TileAt(num2, num).Active || TileAt(num2, num).Type != 80)
				{
					if (TileAt(num2 - 1, num).Active && TileAt(num2 - 1, num).Type == 80 && TileAt(num2 - 1, num - 1).Active && TileAt(num2 - 1, num - 1).Type == 80 && num2 >= i)
					{
						num2--;
					}
					if (TileAt(num2 + 1, num).Active && TileAt(num2 + 1, num).Type == 80 && TileAt(num2 + 1, num - 1).Active && TileAt(num2 + 1, num - 1).Type == 80 && num2 <= i)
					{
						num2++;
					}
				}
			}
			if (!TileAt(num2, num).Active || TileAt(num2, num).Type != 53)
			{
				KillTile(i, j, false, false, false);
				return true;
			}
			if (i != num2)
			{
				if ((!TileAt(i, j + 1).Active || TileAt(i, j + 1).Type != 80) && (!TileAt(i - 1, j).Active || TileAt(i - 1, j).Type != 80) && (!TileAt(i + 1, j).Active || TileAt(i + 1, j).Type != 80))
				{
					KillTile(i, j, false, false, false);
					return true;
				}
			}
			else if (i == num2 && (!TileAt(i, j + 1).Active || (TileAt(i, j + 1).Type != 80 && TileAt(i, j + 1).Type != 53)))
			{
				KillTile(i, j, false, false, false);
				return true;
			}
			return false;
		}

		public void PlantCactus(int i, int j)
		{
			GrowCactus(i, j);
			for (int k = 0; k < 150; k++)
			{
				int i2 = genRand.Next(i - 1, i + 2);
				int j2 = genRand.Next(j - 10, j + 2);
				GrowCactus(i2, j2);
			}
		}

		public void CactusFrame(int i, int j)
		{
			try
			{
				int num = j;
				int num2 = i;
				if (!CheckCactus(i, j))
				{
					while (TileAt(num2, num).Active && TileAt(num2, num).Type == 80)
					{
						num++;
						if (!TileAt(num2, num).Active || TileAt(num2, num).Type != 80)
						{
							if (TileAt(num2 - 1, num).Active && TileAt(num2 - 1, num).Type == 80 && TileAt(num2 - 1, num - 1).Active && TileAt(num2 - 1, num - 1).Type == 80 && num2 >= i)
							{
								num2--;
							}
							if (TileAt(num2 + 1, num).Active && TileAt(num2 + 1, num).Type == 80 && TileAt(num2 + 1, num - 1).Active && TileAt(num2 + 1, num - 1).Type == 80 && num2 <= i)
							{
								num2++;
							}
						}
					}
					num--;
					int num3 = i - num2;
					num2 = i;
					num = j;
					int type = (int)TileAt(i - 2, j).Type;
					int num4 = (int)TileAt(i - 1, j).Type;
					int num5 = (int)TileAt(i + 1, j).Type;
					int num6 = (int)TileAt(i, j - 1).Type;
					int num7 = (int)TileAt(i, j + 1).Type;
					int num8 = (int)TileAt(i - 1, j + 1).Type;
					int num9 = (int)TileAt(i + 1, j + 1).Type;
					if (!TileAt(i - 1, j).Active)
					{
						num4 = -1;
					}
					if (!TileAt(i + 1, j).Active)
					{
						num5 = -1;
					}
					if (!TileAt(i, j - 1).Active)
					{
						num6 = -1;
					}
					if (!TileAt(i, j + 1).Active)
					{
						num7 = -1;
					}
					if (!TileAt(i - 1, j + 1).Active)
					{
						num8 = -1;
					}
					if (!TileAt(i + 1, j + 1).Active)
					{
						num9 = -1;
					}
					short num10 = TileAt(i, j).FrameX;
					short num11 = TileAt(i, j).FrameY;
					if (num3 == 0)
					{
						if (num6 != 80)
						{
							if (num4 == 80 && num5 == 80 && num8 != 80 && num9 != 80 && type != 80)
							{
								num10 = 90;
								num11 = 0;
							}
							else if (num4 == 80 && num8 != 80 && type != 80)
							{
								num10 = 72;
								num11 = 0;
							}
							else if (num5 == 80 && num9 != 80)
							{
								num10 = 18;
								num11 = 0;
							}
							else
							{
								num10 = 0;
								num11 = 0;
							}
						}
						else if (num4 == 80 && num5 == 80 && num8 != 80 && num9 != 80 && type != 80)
						{
							num10 = 90;
							num11 = 36;
						}
						else if (num4 == 80 && num8 != 80 && type != 80)
						{
							num10 = 72;
							num11 = 36;
						}
						else if (num5 == 80 && num9 != 80)
						{
							num10 = 18;
							num11 = 36;
						}
						else if (num7 >= 0 && Main.tileSolid[num7])
						{
							num10 = 0;
							num11 = 36;
						}
						else
						{
							num10 = 0;
							num11 = 18;
						}
					}
					else if (num3 == -1)
					{
						if (num5 == 80)
						{
							if (num6 != 80 && num7 != 80)
							{
								num10 = 108;
								num11 = 36;
							}
							else if (num7 != 80)
							{
								num10 = 54;
								num11 = 36;
							}
							else if (num6 != 80)
							{
								num10 = 54;
								num11 = 0;
							}
							else
							{
								num10 = 54;
								num11 = 18;
							}
						}
						else if (num6 != 80)
						{
							num10 = 54;
							num11 = 0;
						}
						else
						{
							num10 = 54;
							num11 = 18;
						}
					}
					else if (num3 == 1)
					{
						if (num4 == 80)
						{
							if (num6 != 80 && num7 != 80)
							{
								num10 = 108;
								num11 = 16;
							}
							else if (num7 != 80)
							{
								num10 = 36;
								num11 = 36;
							}
							else if (num6 != 80)
							{
								num10 = 36;
								num11 = 0;
							}
							else
							{
								num10 = 36;
								num11 = 18;
							}
						}
						else if (num6 != 80)
						{
							num10 = 36;
							num11 = 0;
						}
						else
						{
							num10 = 36;
							num11 = 18;
						}
					}
					if (num10 != TileAt(i, j).FrameX || num11 != TileAt(i, j).FrameY)
					{
						TileAt(i, j).SetFrameX(num10);
						TileAt(i, j).SetFrameY(num11);
						SquareTileFrame(i, j, true);
					}
				}
			}
			catch
			{
				TileAt(i, j).SetFrameX(0);
				TileAt(i, j).SetFrameY(0);
			}
		}

		public void GrowCactus(int i, int j)
		{
			int num = j;
			int num2 = i;
			if (!TileAt(i, j).Active)
			{
				return;
			}
			if (TileAt(i, j - 1).Liquid > 0)
			{
				return;
			}
			if (TileAt(i, j).Type != 53 && TileAt(i, j).Type != 80)
			{
				return;
			}
			if (TileAt(i, j).Type == 53)
			{
				if (TileAt(i, j - 1).Active || TileAt(i - 1, j - 1).Active || TileAt(i + 1, j - 1).Active)
				{
					return;
				}
				int num3 = 0;
				int num4 = 0;
				for (int k = i - 6; k <= i + 6; k++)
				{
					for (int l = j - 3; l <= j + 1; l++)
					{
						try
						{
							if (TileAt(k, l).Active)
							{
								if (TileAt(k, l).Type == 80)
								{
									num3++;
									if (num3 >= 4)
									{
										return;
									}
								}
								if (TileAt(k, l).Type == 53)
								{
									num4++;
								}
							}
						}
						catch
						{
						}
					}
				}
				if (num4 > 10)
				{
					TileAt(i, j - 1).SetActive(true);
					TileAt(i, j - 1).SetType(80);

					SquareTileFrame(num2, num - 1, true);
					return;
				}
				return;
			}
			else
			{
				if (TileAt(i, j).Type != 80)
				{
					return;
				}
				while (TileAt(num2, num).Active && TileAt(num2, num).Type == 80)
				{
					num++;
					if (!TileAt(num2, num).Active || TileAt(num2, num).Type != 80)
					{
						if (TileAt(num2 - 1, num).Active && TileAt(num2 - 1, num).Type == 80 && TileAt(num2 - 1, num - 1).Active && TileAt(num2 - 1, num - 1).Type == 80 && num2 >= i)
						{
							num2--;
						}
						if (TileAt(num2 + 1, num).Active && TileAt(num2 + 1, num).Type == 80 && TileAt(num2 + 1, num - 1).Active && TileAt(num2 + 1, num - 1).Type == 80 && num2 <= i)
						{
							num2++;
						}
					}
				}
				num--;
				int num5 = num - j;
				int num6 = i - num2;
				num2 = i - num6;
				num = j;
				int num7 = 11 - num5;
				int num8 = 0;
				for (int m = num2 - 2; m <= num2 + 2; m++)
				{
					for (int n = num - num7; n <= num + num5; n++)
					{
						if (TileAt(m, n).Active && TileAt(m, n).Type == 80)
						{
							num8++;
						}
					}
				}
				if (num8 < genRand.Next(11, 13))
				{
					num2 = i;
					num = j;
					if (num6 == 0)
					{
						if (num5 == 0)
						{
							if (TileAt(num2, num - 1).Active)
							{
								return;
							}
							TileAt(num2, num - 1).SetActive(true);
							TileAt(num2, num - 1).SetType(80);
							SquareTileFrame(num2, num - 1, true);
							return;
						}
						else
						{
							bool flag = false;
							bool flag2 = false;
							if (TileAt(num2, num - 1).Active && TileAt(num2, num - 1).Type == 80)
							{
								if (!TileAt(num2 - 1, num).Active && !TileAt(num2 - 2, num + 1).Active && !TileAt(num2 - 1, num - 1).Active && !TileAt(num2 - 1, num + 1).Active && !TileAt(num2 - 2, num).Active)
								{
									flag = true;
								}
								if (!TileAt(num2 + 1, num).Active && !TileAt(num2 + 2, num + 1).Active && !TileAt(num2 + 1, num - 1).Active && !TileAt(num2 + 1, num + 1).Active && !TileAt(num2 + 2, num).Active)
								{
									flag2 = true;
								}
							}
							int num9 = genRand.Next(3);
							if (num9 == 0 && flag)
							{
								TileAt(num2 - 1, num).SetActive(true);
								TileAt(num2 - 1, num).SetType(80);
								SquareTileFrame(num2 - 1, num, true);
								return;
							}
							else if (num9 == 1 && flag2)
							{
								TileAt(num2 + 1, num).SetActive(true);
								TileAt(num2 + 1, num).SetType(80);
								SquareTileFrame(num2 + 1, num, true);
								return;
							}
							else
							{
								if (num5 >= genRand.Next(2, 8))
								{
									return;
								}
								//if (TileAt(num2 - 1, num - 1).Active)
								//{
								//    byte arg_5E0_0 = TileAt(num2 - 1, num - 1).Type;
								//}
								if (TileAt(num2 + 1, num - 1).Active && TileAt(num2 + 1, num - 1).Type == 80)
								{
									return;
								}
								TileAt(num2, num - 1).SetActive(true);
								TileAt(num2, num - 1).SetType(80);
								SquareTileFrame(num2, num - 1, true);
								return;
							}
						}
					}
					else
					{
						if (TileAt(num2, num - 1).Active || TileAt(num2, num - 2).Active || TileAt(num2 + num6, num - 1).Active || !TileAt(num2 - num6, num - 1).Active || TileAt(num2 - num6, num - 1).Type != 80)
						{
							return;
						}
						TileAt(num2, num - 1).SetActive(true);
						TileAt(num2, num - 1).SetType(80);
						SquareTileFrame(num2, num - 1, true);
						return;
					}
				}
			}
		}

		public void CheckPot(int i, int j, int type = 28)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int k = 0;
			k += (int)(TileAt(i, j).FrameX / 18);
			int num = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			while (k > 1)
			{
				k -= 2;
			}
			k *= -1;
			k += i;
			for (int l = k; l < k + 2; l++)
			{
				for (int m = num; m < num + 2; m++)
				{
					int n;
					for (n = (int)(TileAt(l, m).FrameX / 18); n > 1; n -= 2)
					{
					}
					if (!TileAt(l, m).Active || (int)TileAt(l, m).Type != type || n != l - k || (int)TileAt(l, m).FrameY != (m - num) * 18)
					{
						flag = true;
					}
				}
				if (!TileAt(l, num + 2).Active || !Main.tileSolid[(int)TileAt(l, num + 2).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 2; num3++)
					{
						if ((int)TileAt(num2, num3).Type == type && TileAt(num2, num3).Active)
						{
							KillTile(num2, num3, false, false, false);
						}
					}
				}
				if (genRand.Next(50) == 0)
				{
					if ((double)j < Main.worldSurface)
					{
						int num4 = genRand.Next(4);
						if (num4 == 0)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 292, 1, false);
						}
						if (num4 == 1)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 298, 1, false);
						}
						if (num4 == 2)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 299, 1, false);
						}
						if (num4 == 3)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 290, 1, false);
						}
					}
					else if ((double)j < Main.rockLayer)
					{
						int num5 = genRand.Next(7);
						if (num5 == 0)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 289, 1, false);
						}
						if (num5 == 1)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 298, 1, false);
						}
						if (num5 == 2)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 299, 1, false);
						}
						if (num5 == 3)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 290, 1, false);
						}
						if (num5 == 4)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 303, 1, false);
						}
						if (num5 == 5)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 291, 1, false);
						}
						if (num5 == 6)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 304, 1, false);
						}
					}
					else if (j < Main.maxTilesY - 200)
					{
						int num6 = genRand.Next(10);
						if (num6 == 0)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 296, 1, false);
						}
						if (num6 == 1)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 295, 1, false);
						}
						if (num6 == 2)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 299, 1, false);
						}
						if (num6 == 3)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 302, 1, false);
						}
						if (num6 == 4)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 303, 1, false);
						}
						if (num6 == 5)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 305, 1, false);
						}
						if (num6 == 6)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 301, 1, false);
						}
						if (num6 == 7)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 302, 1, false);
						}
						if (num6 == 8)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 297, 1, false);
						}
						if (num6 == 9)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 304, 1, false);
						}
					}
					else
					{
						int num7 = genRand.Next(12);
						if (num7 == 0)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 296, 1, false);
						}
						if (num7 == 1)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 295, 1, false);
						}
						if (num7 == 2)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 293, 1, false);
						}
						if (num7 == 3)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 288, 1, false);
						}
						if (num7 == 4)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 294, 1, false);
						}
						if (num7 == 5)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 297, 1, false);
						}
						if (num7 == 6)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 304, 1, false);
						}
						if (num7 == 7)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 305, 1, false);
						}
						if (num7 == 8)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 301, 1, false);
						}
						if (num7 == 9)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 302, 1, false);
						}
						if (num7 == 10)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 288, 1, false);
						}
						if (num7 == 11)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 300, 1, false);
						}
					}
				}
				else
				{
					int num8 = Main.rand.Next(10);
					if (num8 == 0 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLife < Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLifeMax)
					{
						sandbox.NewItem(i * 16, j * 16, 16, 16, 58, 1, false);
					}
					else if (num8 == 1 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statMana < Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statManaMax)
					{
						sandbox.NewItem(i * 16, j * 16, 16, 16, 184, 1, false);
					}
					else if (num8 == 2)
					{
						int stack = Main.rand.Next(3) + 1;
						if (TileAt(i, j).Liquid > 0)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 282, stack, false);
						}
						else
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 8, stack, false);
						}
					}
					else if (num8 == 3)
					{
						int stack2 = Main.rand.Next(8) + 3;
						int type2 = 40;
						if (j > Main.maxTilesY - 200)
						{
							type2 = 265;
						}
						sandbox.NewItem(i * 16, j * 16, 16, 16, type2, stack2, false);
					}
					else if (num8 == 4)
					{
						int type3 = 28;
						if (j > Main.maxTilesY - 200)
						{
							type3 = 188;
						}
						sandbox.NewItem(i * 16, j * 16, 16, 16, type3, 1, false);
					}
					else if (num8 == 5 && (double)j > Main.rockLayer)
					{
						int stack3 = Main.rand.Next(4) + 1;
						sandbox.NewItem(i * 16, j * 16, 16, 16, 166, stack3, false);
					}
					else
					{
						float num9 = (float)(200 + genRand.Next(-100, 101));
						if ((double)j < Main.worldSurface)
						{
							num9 *= 0.5f;
						}
						else if ((double)j < Main.rockLayer)
						{
							num9 *= 0.75f;
						}
						else if (j > Main.maxTilesY - 250)
						{
							num9 *= 1.25f;
						}
						num9 *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
						if (Main.rand.Next(5) == 0)
						{
							num9 *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;
						}
						if (Main.rand.Next(10) == 0)
						{
							num9 *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;
						}
						if (Main.rand.Next(15) == 0)
						{
							num9 *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;
						}
						if (Main.rand.Next(20) == 0)
						{
							num9 *= 1f + (float)Main.rand.Next(40, 81) * 0.01f;
						}
						if (Main.rand.Next(25) == 0)
						{
							num9 *= 1f + (float)Main.rand.Next(50, 101) * 0.01f;
						}
						while ((int)num9 > 0)
						{
							if (num9 > 1000000f)
							{
								int num10 = (int)(num9 / 1000000f);
								if (num10 > 50 && Main.rand.Next(2) == 0)
								{
									num10 /= Main.rand.Next(3) + 1;
								}
								if (Main.rand.Next(2) == 0)
								{
									num10 /= Main.rand.Next(3) + 1;
								}
								num9 -= (float)(1000000 * num10);
								sandbox.NewItem(i * 16, j * 16, 16, 16, 74, num10, false);
							}
							else if (num9 > 10000f)
							{
								int num11 = (int)(num9 / 10000f);
								if (num11 > 50 && Main.rand.Next(2) == 0)
								{
									num11 /= Main.rand.Next(3) + 1;
								}
								if (Main.rand.Next(2) == 0)
								{
									num11 /= Main.rand.Next(3) + 1;
								}
								num9 -= (float)(10000 * num11);
								sandbox.NewItem(i * 16, j * 16, 16, 16, 73, num11, false);
							}
							else if (num9 > 100f)
							{
								int num12 = (int)(num9 / 100f);
								if (num12 > 50 && Main.rand.Next(2) == 0)
								{
									num12 /= Main.rand.Next(3) + 1;
								}
								if (Main.rand.Next(2) == 0)
								{
									num12 /= Main.rand.Next(3) + 1;
								}
								num9 -= (float)(100 * num12);
								sandbox.NewItem(i * 16, j * 16, 16, 16, 72, num12, false);
							}
							else
							{
								int num13 = (int)num9;
								if (num13 > 50 && Main.rand.Next(2) == 0)
								{
									num13 /= Main.rand.Next(3) + 1;
								}
								if (Main.rand.Next(2) == 0)
								{
									num13 /= Main.rand.Next(4) + 1;
								}
								if (num13 < 1)
								{
									num13 = 1;
								}
								num9 -= (float)num13;
								sandbox.NewItem(i * 16, j * 16, 16, 16, 71, num13, false);
							}
						}
					}
				}
				destroyObject = false;
			}
		}

		public int PlaceChest(int x, int y, int type = 21, bool notNearOtherChests = false, int style = 0)
		{
			bool flag = true;
			int num = -1;
			for (int modX = x; modX < x + 2; modX++)
			{
				for (int modY = y - 1; modY < y + 1; modY++)
				{
					var tile = GetTile(modX, modY);
					if (tile.Active || tile.Lava)
					{
						flag = false;
					}
				}

				if (!TileAt(modX, y + 1).Active || !Main.tileSolid[(int)TileAt(modX, y + 1).Type])
				{
					flag = false;
				}
			}
			if (flag && notNearOtherChests)
			{
				for (int k = x - 30; k < x + 30; k++)
				{
					for (int l = y - 10; l < y + 10; l++)
					{
						try
						{
							if (TileAt(k, l).Active && TileAt(k, l).Type == 21)
							{
								flag = false;
								return -1;
							}
						}
						catch
						{
						}
					}
				}
			}
			if (flag)
			{
				num = Chest.CreateChest(x, y - 1);
				if (num == -1)
				{
					flag = false;
				}
			}
			if (flag)
			{
				TileAt(x, y - 1).SetActive(true);
				TileAt(x, y - 1).SetFrameY(0);
				TileAt(x, y - 1).SetFrameX((short)(36 * style));
				TileAt(x, y - 1).SetType((byte)type);
				TileAt(x + 1, y - 1).SetActive(true);
				TileAt(x + 1, y - 1).SetFrameY(0);
				TileAt(x + 1, y - 1).SetFrameX((short)(18 + 36 * style));
				TileAt(x + 1, y - 1).SetType((byte)type);
				TileAt(x, y).SetActive(true);
				TileAt(x, y).SetFrameY(18);
				TileAt(x, y).SetFrameX((short)(36 * style));
				TileAt(x, y).SetType((byte)type);
				TileAt(x + 1, y).SetActive(true);
				TileAt(x + 1, y).SetFrameY(18);
				TileAt(x + 1, y).SetFrameX((short)(18 + 36 * style));
				TileAt(x + 1, y).SetType((byte)type);
			}
			return num;
		}

		public void CheckChest(int i, int j, int type)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int k = 0;
			k += (int)(TileAt(i, j).FrameX / 18);
			int num = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			while (k > 1)
			{
				k -= 2;
			}
			k *= -1;
			k += i;
			for (int l = k; l < k + 2; l++)
			{
				for (int m = num; m < num + 2; m++)
				{
					int n;
					for (n = (int)(TileAt(l, m).FrameX / 18); n > 1; n -= 2)
					{
					}
					if (!TileAt(l, m).Active || (int)TileAt(l, m).Type != type || n != l - k || (int)TileAt(l, m).FrameY != (m - num) * 18)
					{
						flag = true;
					}
				}
				if (!TileAt(l, num + 2).Active || !Main.tileSolid[(int)TileAt(l, num + 2).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				int type2 = 48;
				if (TileAt(i, j).FrameX >= 216)
				{
					type2 = 348;
				}
				else if (TileAt(i, j).FrameX >= 180)
				{
					type2 = 343;
				}
				else if (TileAt(i, j).FrameX >= 108)
				{
					type2 = 328;
				}
				else if (TileAt(i, j).FrameX >= 36)
				{
					type2 = 306;
				}
				destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 3; num3++)
					{
						if ((int)TileAt(num2, num3).Type == type && TileAt(num2, num3).Active)
						{
							sandbox.DestroyChest(num2, num3);
							KillTile(num2, num3, false, false, false);
						}
					}
				}
				sandbox.NewItem(i * 16, j * 16, 32, 32, type2, 1, false);
				destroyObject = false;
			}
		}

		public void Place1xX(int x, int y, int type, int style = 0)
		{
			int num = style * 18;
			int num2 = 3;
			if (type == 92)
			{
				num2 = 6;
			}
			bool flag = true;
			for (int i = y - num2 + 1; i < y + 1; i++)
			{
				if (TileAt(x, i).Active)
				{
					flag = false;
				}
				if (type == 93 && TileAt(x, i).Liquid > 0)
				{
					flag = false;
				}
			}
			if (flag && TileAt(x, y + 1).Active && Main.tileSolid[(int)TileAt(x, y + 1).Type])
			{
				for (int j = 0; j < num2; j++)
				{
					TileAt(x, y - num2 + 1 + j).SetActive(true);
					TileAt(x, y - num2 + 1 + j).SetFrameY((short)(j * 18));
					TileAt(x, y - num2 + 1 + j).SetFrameX((short)num);
					TileAt(x, y - num2 + 1 + j).SetType((byte)type);
				}
			}
		}

		public void Place2xX(int x, int y, int type, int style = 0)
		{
			int num = style * 18;
			int num2 = 3;
			if (type == 104)
			{
				num2 = 5;
			}
			bool flag = true;
			for (int i = y - num2 + 1; i < y + 1; i++)
			{
				if (TileAt(x, i).Active)
				{
					flag = false;
				}
				if (TileAt(x + 1, i).Active)
				{
					flag = false;
				}
			}
			if (flag && TileAt(x, y + 1).Active && Main.tileSolid[(int)TileAt(x, y + 1).Type] &&
				TileAt(x + 1, y + 1).Active && Main.tileSolid[(int)TileAt(x + 1, y + 1).Type])
			{
				for (int j = 0; j < num2; j++)
				{
					TileAt(x, y - num2 + 1 + j).SetActive(true);
					TileAt(x, y - num2 + 1 + j).SetFrameY((short)(j * 18));
					TileAt(x, y - num2 + 1 + j).SetFrameX((short)num);
					TileAt(x, y - num2 + 1 + j).SetType((byte)type);
					TileAt(x + 1, y - num2 + 1 + j).SetActive(true);
					TileAt(x + 1, y - num2 + 1 + j).SetFrameY((short)(j * 18));
					TileAt(x + 1, y - num2 + 1 + j).SetFrameX((short)(num + 18));
					TileAt(x + 1, y - num2 + 1 + j).SetType((byte)type);
				}
			}
		}

		public void Place3x4(int x, int y, int type)
		{
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = x - 1; i < x + 2; i++)
			{
				for (int j = y - 3; j < y + 1; j++)
				{
					if (TileAt(i, j).Active)
					{
						flag = false;
					}
				}
				if (!TileAt(i, y + 1).Active || !Main.tileSolid[(int)TileAt(i, y + 1).Type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				for (int k = -3; k <= 0; k++)
				{
					short frameY = (short)((3 + k) * 18);
					TileAt(x - 1, y + k).SetActive(true);
					TileAt(x - 1, y + k).SetFrameY(frameY);
					TileAt(x - 1, y + k).SetFrameX(0);
					TileAt(x - 1, y + k).SetType((byte)type);
					TileAt(x, y + k).SetActive(true);
					TileAt(x, y + k).SetFrameY(frameY);
					TileAt(x, y + k).SetFrameX(18);
					TileAt(x, y + k).SetType((byte)type);
					TileAt(x + 1, y + k).SetActive(true);
					TileAt(x + 1, y + k).SetFrameY(frameY);
					TileAt(x + 1, y + k).SetFrameX(36);
					TileAt(x + 1, y + k).SetType((byte)type);
				}
			}
		}

		public bool PlaceTile(int i, int j, int type, bool mute = false, bool forced = false, int plr = -1, int style = 0)
		{
			if (type >= MAX_TILE_SETS)
			{
				return false;
			}
			bool result = false;
			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
			{
				if (forced || Collision.EmptyTile(i, j, false) || !Main.tileSolid[type] || (type == 23 && TileAt(i, j).Type == 0 && TileAt(i, j).Active) || (type == 2 && TileAt(i, j).Type == 0 && TileAt(i, j).Active) || (type == 60 && TileAt(i, j).Type == 59 && TileAt(i, j).Active) || (type == 70 && TileAt(i, j).Type == 59 && TileAt(i, j).Active))
				{
					if (type == 23 && (TileAt(i, j).Type != 0 || !TileAt(i, j).Active))
					{
						return false;
					}
					if (type == 2 && (TileAt(i, j).Type != 0 || !TileAt(i, j).Active))
					{
						return false;
					}
					if (type == 60 && (TileAt(i, j).Type != 59 || !TileAt(i, j).Active))
					{
						return false;
					}
					if (type == 81)
					{
						if (TileAt(i - 1, j).Active || TileAt(i + 1, j).Active || TileAt(i, j - 1).Active)
						{
							return false;
						}
						if (!TileAt(i, j + 1).Active || !Main.tileSolid[(int)TileAt(i, j + 1).Type])
						{
							return false;
						}
					}
					if (TileAt(i, j).Liquid > 0 && (type == 3 || type == 4 || type == 20 || type == 24 || type == 27 || type == 32 || type == 51 || type == 69 || type == 72))
					{
						return false;
					}
					TileAt(i, j).SetFrameY(0);
					TileAt(i, j).SetFrameX(0);
					if (type == 3 || type == 24)
					{
						if (j + 1 < Main.maxTilesY && TileAt(i, j + 1).Active && ((TileAt(i, j + 1).Type == 2 && type == 3) || (TileAt(i, j + 1).Type == 23 && type == 24) || (TileAt(i, j + 1).Type == 78 && type == 3)))
						{
							if (type == 24 && genRand.Next(13) == 0)
							{
								TileAt(i, j).SetActive(true);
								TileAt(i, j).SetType(32);
								SquareTileFrame(i, j, true);
							}
							else if (TileAt(i, j + 1).Type == 78)
							{
								TileAt(i, j).SetActive(true);
								TileAt(i, j).SetType((byte)type);
								TileAt(i, j).SetFrameX((short)(genRand.Next(2) * 18 + 108));
							}
							else if (TileAt(i, j).Wall == 0 && TileAt(i, j + 1).Wall == 0)
							{
								if (genRand.Next(50) == 0 || (type == 24 && genRand.Next(40) == 0))
								{
									TileAt(i, j).SetActive(true);
									TileAt(i, j).SetType((byte)type);
									TileAt(i, j).SetFrameX(144);
								}
								else if (genRand.Next(35) == 0)
								{
									TileAt(i, j).SetActive(true);
									TileAt(i, j).SetType((byte)type);
									TileAt(i, j).SetFrameX((short)(genRand.Next(2) * 18 + 108));
								}
								else
								{
									TileAt(i, j).SetActive(true);
									TileAt(i, j).SetType((byte)type);
									TileAt(i, j).SetFrameX((short)(genRand.Next(6) * 18));
								}
							}
						}
					}
					else if (type == 61)
					{
						if (j + 1 < Main.maxTilesY && TileAt(i, j + 1).Active && TileAt(i, j + 1).Type == 60)
						{
							if (genRand.Next(10) == 0 && (double)j > Main.worldSurface)
							{
								TileAt(i, j).SetActive(true);
								TileAt(i, j).SetType(69);
								SquareTileFrame(i, j, true);
							}
							else if (genRand.Next(15) == 0 && (double)j > Main.worldSurface)
							{
								TileAt(i, j).SetActive(true);
								TileAt(i, j).SetType((byte)type);
								TileAt(i, j).SetFrameX(144);
							}
							else if (genRand.Next(1000) == 0 && (double)j > Main.rockLayer)
							{
								TileAt(i, j).SetActive(true);
								TileAt(i, j).SetType((byte)type);
								TileAt(i, j).SetFrameX(162);
							}
							else
							{
								TileAt(i, j).SetActive(true);
								TileAt(i, j).SetType((byte)type);
								if ((double)j > Main.rockLayer)
								{
									TileAt(i, j).SetFrameX((short)(genRand.Next(8) * 18));
								}
								else
								{
									TileAt(i, j).SetFrameX((short)(genRand.Next(6) * 18));
								}
							}
						}
					}
					else if (type == 71)
					{
						if (j + 1 < Main.maxTilesY && TileAt(i, j + 1).Active && TileAt(i, j + 1).Type == 70)
						{
							TileAt(i, j).SetActive(true);
							TileAt(i, j).SetType((byte)type);
							TileAt(i, j).SetFrameX((short)(genRand.Next(5) * 18));
						}
					}
					else if (type == 4)
					{
						if ((TileAt(i - 1, j).Active && (Main.tileSolid[(int)TileAt(i - 1, j).Type] || (TileAt(i - 1, j).Type == 5 && TileAt(i - 1, j - 1).Type == 5 && TileAt(i - 1, j + 1).Type == 5))) || (TileAt(i + 1, j).Active && (Main.tileSolid[(int)TileAt(i + 1, j).Type] || (TileAt(i + 1, j).Type == 5 && TileAt(i + 1, j - 1).Type == 5 && TileAt(i + 1, j + 1).Type == 5))) || (TileAt(i, j + 1).Active && Main.tileSolid[(int)TileAt(i, j + 1).Type]))
						{
							TileAt(i, j).SetActive(true);
							TileAt(i, j).SetType((byte)type);
							SquareTileFrame(i, j, true);
						}
					}
					else if (type == 10)
					{
						if (!TileAt(i, j - 1).Active && !TileAt(i, j - 2).Active && TileAt(i, j - 3).Active && Main.tileSolid[(int)TileAt(i, j - 3).Type])
						{
							PlaceDoor(i, j - 1, type);
							SquareTileFrame(i, j, true);
						}
						else
						{
							if (TileAt(i, j + 1).Active || TileAt(i, j + 2).Active || !TileAt(i, j + 3).Active || !Main.tileSolid[(int)TileAt(i, j + 3).Type])
							{
								return false;
							}
							PlaceDoor(i, j + 1, type);
							SquareTileFrame(i, j, true);
						}
					}
					else if (type == 34 || type == 35 || type == 36 || type == 106)
					{
						Place3x3(i, j, type);
						SquareTileFrame(i, j, true);
					}
					else if (type == 13 || type == 33 || type == 49 || type == 50 || type == 78)
					{
						PlaceOnTable1x1(i, j, type, style);
						SquareTileFrame(i, j, true);
					}
					else if (type == 14 || type == 26 || type == 86 || type == 87 || type == 88 || type == 89)
					{
						Place3x2(i, j, type);
						SquareTileFrame(i, j, true);
					}
					else if (type == 20)
					{
						if (TileAt(i, j + 1).Active && TileAt(i, j + 1).Type == 2)
						{
							Place1x2(i, j, type, style);
							SquareTileFrame(i, j, true);
						}
					}
					else if (type == 15)
					{
						Place1x2(i, j, type, style);
						SquareTileFrame(i, j, true);
					}
					else if (type == 16 || type == 18 || type == 29 || type == 103)
					{
						Place2x1(i, j, type);
						SquareTileFrame(i, j, true);
					}
					else if (type == 92 || type == 93)
					{
						Place1xX(i, j, type, 0);
						SquareTileFrame(i, j, true);
					}
					else if (type == 104 || type == 105)
					{
						Place2xX(i, j, type, 0);
						SquareTileFrame(i, j, true);
					}
					else if (type == 17 || type == 77)
					{
						Place3x2(i, j, type);
						SquareTileFrame(i, j, true);
					}
					else if (type == 21)
					{
						PlaceChest(i, j, type, false, style);
						SquareTileFrame(i, j, true);
					}
					else if (type == 91)
					{
						PlaceBanner(i, j, type, style);
						SquareTileFrame(i, j, true);
					}
					else if (type == 101 || type == 102)
					{
						Place3x4(i, j, type);
						SquareTileFrame(i, j, true);
					}
					else if (type == 27)
					{
						PlaceSunflower(i, j, 27);
						SquareTileFrame(i, j, true);
					}
					else if (type == 28)
					{
						PlacePot(i, j, 28);
						SquareTileFrame(i, j, true);
					}
					else if (type == 42)
					{
						Place1x2Top(i, j, type);
						SquareTileFrame(i, j, true);
					}
					else if (type == 55 || type == 85)
					{
						PlaceSign(i, j, type);
					}
					else if (Main.tileAlch[type])
					{
						PlaceAlch(i, j, style);
					}
					else if (type == 94 || type == 95 || type == 96 || type == 97 || type == 98 || type == 99 || type == 100)
					{
						Place2x2(i, j, type);
					}
					else if (type == 79 || type == 90)
					{
						int direction = 1;
						if (plr > -1)
						{
							direction = Main.players[plr].direction;
						}
						Place4x2(i, j, type, direction);
					}
					else if (type == 81)
					{
						TileAt(i, j).SetFrameX((short)(26 * genRand.Next(6)));
						TileAt(i, j).SetActive(true);
						TileAt(i, j).SetType((byte)type);
					}
					else
					{
						TileAt(i, j).SetActive(true);
						TileAt(i, j).SetType((byte)type);
					}
					if (TileAt(i, j).Active && !mute)
					{
						SquareTileFrame(i, j, true);
						result = true;
					}
				}
			}
			return result;
		}

		public void KillWall(int i, int j, bool fail = false)
		{
			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
			{
				if (TileAt(i, j).Wall > 0)
				{
					if (fail)
					{
						return;
					}
					int num2 = 0;
					if (TileAt(i, j).Wall == 1)
					{
						num2 = 26;
					}
					if (TileAt(i, j).Wall == 4)
					{
						num2 = 93;
					}
					if (TileAt(i, j).Wall == 5)
					{
						num2 = 130;
					}
					if (TileAt(i, j).Wall == 6)
					{
						num2 = 132;
					}
					if (TileAt(i, j).Wall == 7)
					{
						num2 = 135;
					}
					if (TileAt(i, j).Wall == 8)
					{
						num2 = 138;
					}
					if (TileAt(i, j).Wall == 9)
					{
						num2 = 140;
					}
					if (TileAt(i, j).Wall == 10)
					{
						num2 = 142;
					}
					if (TileAt(i, j).Wall == 11)
					{
						num2 = 144;
					}
					if (TileAt(i, j).Wall == 12)
					{
						num2 = 146;
					}
					if (TileAt(i, j).Wall == 14)
					{
						num2 = 330;
					}
					if (TileAt(i, j).Wall == 16)
					{
						num2 = 30;
					}
					if (TileAt(i, j).Wall == 17)
					{
						num2 = 135;
					}
					if (TileAt(i, j).Wall == 18)
					{
						num2 = 138;
					}
					if (TileAt(i, j).Wall == 19)
					{
						num2 = 140;
					}
					if (TileAt(i, j).Wall == 20)
					{
						num2 = 330;
					}
					if (num2 > 0)
					{
						sandbox.NewItem(i * 16, j * 16, 16, 16, num2, 1, false);
					}
					TileAt(i, j).SetWall(0);
				}
			}
		}

		public void KillTile(int x, int y, bool fail = false, bool effectOnly = false, bool noItem = false, Player player = null)
		{
			//WorldModify.KillTile(x,y, Ti
		}

		public bool PlayerLOS(int x, int y)
		{
			Rectangle rectangle = new Rectangle(x * 16, y * 16, 16, 16);
			for (int i = 0; i < 255; i++)
			{
				if (Main.players[i].Active)
				{
					Rectangle value = new Rectangle((int)((double)Main.players[i].Position.X + (double)Main.players[i].Width * 0.5 - (double)NPC.sWidth * 0.6), (int)((double)Main.players[i].Position.Y + (double)Main.players[i].Height * 0.5 - (double)NPC.sHeight * 0.6), (int)((double)NPC.sWidth * 1.2), (int)((double)NPC.sHeight * 1.2));
					if (rectangle.Intersects(value))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void PlaceWall(int i, int j, int type, bool mute = false)
		{
			if ((int)TileAt(i, j).Wall != type)
			{
				for (int k = i - 1; k < i + 2; k++)
				{
					for (int l = j - 1; l < j + 2; l++)
					{
						if (TileAt(k, l).Wall > 0 && (int)TileAt(k, l).Wall != type)
						{
							bool flag = false;
							if (TileAt(i, j).Wall == 0 && (type == 2 || type == 16) && (TileAt(k, l).Wall == 2 || TileAt(k, l).Wall == 16))
							{
								flag = true;
							}
							if (!flag)
							{
								return;
							}
						}
					}
				}
				TileAt(i, j).SetWall((byte)type);
			}
		}

		public void SpreadGrass(int i, int j, int dirt = 0, int grass = 2, bool repeat = true)
		{
			if ((int)TileAt(i, j).Type != dirt || !TileAt(i, j).Active || ((double)j < Main.worldSurface && grass == 70) || ((double)j >= Main.worldSurface && dirt == 0))
			{
				return;
			}
			int num = i - 1;
			int num2 = i + 2;
			int num3 = j - 1;
			int num4 = j + 2;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 > Main.maxTilesX)
			{
				num2 = Main.maxTilesX;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 > Main.maxTilesY)
			{
				num4 = Main.maxTilesY;
			}
			bool flag = true;
			for (int k = num; k < num2; k++)
			{
				for (int l = num3; l < num4; l++)
				{
					if (!TileAt(k, l).Active || !Main.tileSolid[(int)TileAt(k, l).Type])
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag)
			{
				if (grass == 23 && TileAt(i, j - 1).Type == 27)
				{
					return;
				}
				TileAt(i, j).SetType((byte)grass);
				for (int m = num; m < num2; m++)
				{
					for (int n = num3; n < num4; n++)
					{
						if (TileAt(m, n).Active && (int)TileAt(m, n).Type == dirt && repeat)
						{
							SpreadGrass(m, n, dirt, grass, true);
						}
					}
				}
			}
		}

		public void SquareTileFrame(int i, int j, bool resetFrame = true)
		{
			TileFrame(i - 1, j - 1, false, false);
			TileFrame(i - 1, j, false, false);
			TileFrame(i - 1, j + 1, false, false);
			TileFrame(i, j - 1, false, false);
			TileFrame(i, j, resetFrame, false);
			TileFrame(i, j + 1, false, false);
			TileFrame(i + 1, j - 1, false, false);
			TileFrame(i + 1, j, false, false);
			TileFrame(i + 1, j + 1, false, false);
		}

		public void SectionTileFrame(int startX, int startY, int endX, int endY)
		{
			int num = startX * 200;
			int num2 = (endX + 1) * 200;
			int num3 = startY * 150;
			int num4 = (endY + 1) * 150;
			if (num < 1)
			{
				num = 1;
			}
			if (num3 < 1)
			{
				num3 = 1;
			}
			if (num > Main.maxTilesX - 2)
			{
				num = Main.maxTilesX - 2;
			}
			if (num3 > Main.maxTilesY - 2)
			{
				num3 = Main.maxTilesY - 2;
			}
			for (int i = num - 1; i < num2 + 1; i++)
			{
				for (int j = num3 - 1; j < num4 + 1; j++)
				{
					TileFrame(i, j, true, true);
				}
			}
		}

		public void RangeFrame(int startX, int startY, int endX, int endY)
		{
			int num = endX + 1;
			int num2 = endY + 1;
			for (int i = startX - 1; i < num + 1; i++)
			{
				for (int j = startY - 1; j < num2 + 1; j++)
				{
					TileFrame(i, j, false, false);
				}
			}
		}

		public void PlantCheck(int i, int j)
		{
			int num = -1;
			int type = (int)TileAt(i, j).Type;
			if (j + 1 >= Main.maxTilesY)
			{
				num = type;
			}
			if (j + 1 < Main.maxTilesY && TileAt(i, j + 1).Active)
			{
				num = (int)TileAt(i, j + 1).Type;
			}
			if ((type == 3 && num != 2 && num != 78) || (type == 24 && num != 23) || (type == 61 && num != 60) || (type == 71 && num != 70) || (type == 73 && num != 2 && num != 78) || (type == 74 && num != 60))
			{
				KillTile(i, j, false, false, false);
			}
		}

		public void TileFrame(int i, int j, bool resetFrame = false, bool noBreak = false)
		{
			if (i > 5 && j > 5 && i < Main.maxTilesX - 5 && j < Main.maxTilesY - 5)
			{
				if (TileAt(i, j).Liquid > 0 && !noLiquidCheck)
				{
					sandbox.AddWater(i, j);
				}
				if (TileAt(i, j).Active)
				{
					if (noBreak && Main.tileFrameImportant[(int)TileAt(i, j).Type])
					{
						return;
					}
					int num = -1;
					int num2 = -1;
					int num3 = -1;
					int num4 = -1;
					int num5 = -1;
					int num6 = -1;
					int num7 = -1;
					int num8 = -1;
					int num9 = (int)TileAt(i, j).Type;
					if (Main.tileStone[num9])
					{
						num9 = 1;
					}
					int frameX = (int)TileAt(i, j).FrameX;
					int frameY = (int)TileAt(i, j).FrameY;
					Rectangle rectangle;
					rectangle.X = -1;
					rectangle.Y = -1;
					mergeUp = false;
					mergeDown = false;
					mergeLeft = false;
					mergeRight = false;
					if (TileAt(i - 1, j).Active)
					{
						num4 = (int)TileAt(i - 1, j).Type;
					}
					if (TileAt(i + 1, j).Active)
					{
						num5 = (int)TileAt(i + 1, j).Type;
					}
					if (TileAt(i, j - 1).Active)
					{
						num2 = (int)TileAt(i, j - 1).Type;
					}
					if (TileAt(i, j + 1).Active)
					{
						num7 = (int)TileAt(i, j + 1).Type;
					}
					if (TileAt(i - 1, j - 1).Active)
					{
						num = (int)TileAt(i - 1, j - 1).Type;
					}
					if (TileAt(i + 1, j - 1).Active)
					{
						num3 = (int)TileAt(i + 1, j - 1).Type;
					}
					if (TileAt(i - 1, j + 1).Active)
					{
						num6 = (int)TileAt(i - 1, j + 1).Type;
					}
					if (TileAt(i + 1, j + 1).Active)
					{
						num8 = (int)TileAt(i + 1, j + 1).Type;
					}
					if (num4 >= 0 && Main.tileStone[num4])
					{
						num4 = 1;
					}
					if (num5 >= 0 && Main.tileStone[num5])
					{
						num5 = 1;
					}
					if (num2 >= 0 && Main.tileStone[num2])
					{
						num2 = 1;
					}
					if (num7 >= 0 && Main.tileStone[num7])
					{
						num7 = 1;
					}
					if (num >= 0 && Main.tileStone[num])
					{
						num = 1;
					}
					if (num3 >= 0 && Main.tileStone[num3])
					{
						num3 = 1;
					}
					if (num6 >= 0 && Main.tileStone[num6])
					{
						num6 = 1;
					}
					if (num8 >= 0 && Main.tileStone[num8])
					{
						num8 = 1;
					}
					if (num9 != 0 && num9 != 1)
					{
						if (num9 == 3 || num9 == 24 || num9 == 61 || num9 == 71 || num9 == 73 || num9 == 74)
						{
							PlantCheck(i, j);
							return;
						}
						if (num9 == 4)
						{
							if (num7 >= 0 && Main.tileSolid[num7] && !Main.tileNoAttach[num7])
							{
								TileAt(i, j).SetFrameX(0);
								return;
							}
							if ((num4 >= 0 && Main.tileSolid[num4] && !Main.tileNoAttach[num4]) || (num4 == 5 && num == 5 && num6 == 5))
							{
								TileAt(i, j).SetFrameX(22);
								return;
							}
							if ((num5 >= 0 && Main.tileSolid[num5] && !Main.tileNoAttach[num5]) || (num5 == 5 && num3 == 5 && num8 == 5))
							{
								TileAt(i, j).SetFrameX(44);
								return;
							}
							KillTile(i, j, false, false, false);
							return;
						}
						else
						{
							if (num9 == 80)
							{
								CactusFrame(i, j);
								return;
							}
							if (num9 == 12 || num9 == 31)
							{
								if (!destroyObject)
								{
									int num10 = i;
									int num11 = j;
									if (TileAt(i, j).FrameX == 0)
									{
										num10 = i;
									}
									else
									{
										num10 = i - 1;
									}
									if (TileAt(i, j).FrameY == 0)
									{
										num11 = j;
									}
									else
									{
										num11 = j - 1;
									}
									if ((!TileAt(num10, num11).Active || (int)TileAt(num10, num11).Type != num9 ||
										!TileAt(num10 + 1, num11).Active || (int)TileAt(num10 + 1, num11).Type != num9 ||
											!TileAt(num10, num11 + 1).Active || (int)TileAt(num10, num11 + 1).Type != num9 ||
												!TileAt(num10 + 1, num11 + 1).Active ||
													(int)TileAt(num10 + 1, num11 + 1).Type != num9))
									{
										destroyObject = true;
										if ((int)TileAt(num10, num11).Type == num9)
										{
											KillTile(num10, num11, false, false, false);
										}
										if ((int)TileAt(num10 + 1, num11).Type == num9)
										{
											KillTile(num10 + 1, num11, false, false, false);
										}
										if ((int)TileAt(num10, num11 + 1).Type == num9)
										{
											KillTile(num10, num11 + 1, false, false, false);
										}
										if ((int)TileAt(num10 + 1, num11 + 1).Type == num9)
										{
											KillTile(num10 + 1, num11 + 1, false, false, false);
										}
										if (!noTileActions)
										{
											if (num9 == 12)
											{
												sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 29, 1, false);
											}
											else
											{
												if (num9 == 31)
												{
													if (genRand.Next(2) == 0)
													{
														spawnMeteor = true;
													}
													int num12 = Main.rand.Next(5);
													if (!WorldModify.shadowOrbSmashed)
													{
														num12 = 0;
													}
													if (num12 == 0)
													{
														sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 96, 1, false);
														int stack = genRand.Next(25, 51);
														sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 97, stack, false);
													}
													else
													{
														if (num12 == 1)
														{
															sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 64, 1, false);
														}
														else
														{
															if (num12 == 2)
															{
																sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 162, 1, false);
															}
															else
															{
																if (num12 == 3)
																{
																	sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 115, 1, false);
																}
																else
																{
																	if (num12 == 4)
																	{
																		sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 111, 1, false);
																	}
																}
															}
														}
													}
													sandbox.ShadowOrbSmashed(num10, num11);
												}
											}
										}
										destroyObject = false;
									}
								}
								return;
							}
							if (num9 == 19)
							{
								if (num4 == num9 && num5 == num9)
								{
									if (TileAt(i, j).FrameNumber == 0)
									{
										rectangle.X = 0;
										rectangle.Y = 0;
									}
									if (TileAt(i, j).FrameNumber == 1)
									{
										rectangle.X = 0;
										rectangle.Y = 18;
									}
									if (TileAt(i, j).FrameNumber == 2)
									{
										rectangle.X = 0;
										rectangle.Y = 36;
									}
								}
								else
								{
									if (num4 == num9 && num5 == -1)
									{
										if (TileAt(i, j).FrameNumber == 0)
										{
											rectangle.X = 18;
											rectangle.Y = 0;
										}
										if (TileAt(i, j).FrameNumber == 1)
										{
											rectangle.X = 18;
											rectangle.Y = 18;
										}
										if (TileAt(i, j).FrameNumber == 2)
										{
											rectangle.X = 18;
											rectangle.Y = 36;
										}
									}
									else
									{
										if (num4 == -1 && num5 == num9)
										{
											if (TileAt(i, j).FrameNumber == 0)
											{
												rectangle.X = 36;
												rectangle.Y = 0;
											}
											if (TileAt(i, j).FrameNumber == 1)
											{
												rectangle.X = 36;
												rectangle.Y = 18;
											}
											if (TileAt(i, j).FrameNumber == 2)
											{
												rectangle.X = 36;
												rectangle.Y = 36;
											}
										}
										else
										{
											if (num4 != num9 && num5 == num9)
											{
												if (TileAt(i, j).FrameNumber == 0)
												{
													rectangle.X = 54;
													rectangle.Y = 0;
												}
												if (TileAt(i, j).FrameNumber == 1)
												{
													rectangle.X = 54;
													rectangle.Y = 18;
												}
												if (TileAt(i, j).FrameNumber == 2)
												{
													rectangle.X = 54;
													rectangle.Y = 36;
												}
											}
											else
											{
												if (num4 == num9 && num5 != num9)
												{
													if (TileAt(i, j).FrameNumber == 0)
													{
														rectangle.X = 72;
														rectangle.Y = 0;
													}
													if (TileAt(i, j).FrameNumber == 1)
													{
														rectangle.X = 72;
														rectangle.Y = 18;
													}
													if (TileAt(i, j).FrameNumber == 2)
													{
														rectangle.X = 72;
														rectangle.Y = 36;
													}
												}
												else
												{
													if (num4 != num9 && num4 != -1 && num5 == -1)
													{
														if (TileAt(i, j).FrameNumber == 0)
														{
															rectangle.X = 108;
															rectangle.Y = 0;
														}
														if (TileAt(i, j).FrameNumber == 1)
														{
															rectangle.X = 108;
															rectangle.Y = 18;
														}
														if (TileAt(i, j).FrameNumber == 2)
														{
															rectangle.X = 108;
															rectangle.Y = 36;
														}
													}
													else
													{
														if (num4 == -1 && num5 != num9 && num5 != -1)
														{
															if (TileAt(i, j).FrameNumber == 0)
															{
																rectangle.X = 126;
																rectangle.Y = 0;
															}
															if (TileAt(i, j).FrameNumber == 1)
															{
																rectangle.X = 126;
																rectangle.Y = 18;
															}
															if (TileAt(i, j).FrameNumber == 2)
															{
																rectangle.X = 126;
																rectangle.Y = 36;
															}
														}
														else
														{
															if (TileAt(i, j).FrameNumber == 0)
															{
																rectangle.X = 90;
																rectangle.Y = 0;
															}
															if (TileAt(i, j).FrameNumber == 1)
															{
																rectangle.X = 90;
																rectangle.Y = 18;
															}
															if (TileAt(i, j).FrameNumber == 2)
															{
																rectangle.X = 90;
																rectangle.Y = 36;
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
								if (num9 == 10)
								{
									if (!destroyObject)
									{
										int frameY2 = (int)TileAt(i, j).FrameY;
										int num17 = j;
										bool flag = false;
										if (frameY2 == 0)
										{
											num17 = j;
										}
										if (frameY2 == 18)
										{
											num17 = j - 1;
										}
										if (frameY2 == 36)
										{
											num17 = j - 2;
										}
										if (!TileAt(i, num17 - 1).Active || !Main.tileSolid[(int)TileAt(i, num17 - 1).Type])
										{
											flag = true;
										}
										if (!TileAt(i, num17 + 3).Active || !Main.tileSolid[(int)TileAt(i, num17 + 3).Type])
										{
											flag = true;
										}
										if (!TileAt(i, num17).Active || (int)TileAt(i, num17).Type != num9)
										{
											flag = true;
										}
										if (!TileAt(i, num17 + 1).Active || (int)TileAt(i, num17 + 1).Type != num9)
										{
											flag = true;
										}
										if (!TileAt(i, num17 + 2).Active || (int)TileAt(i, num17 + 2).Type != num9)
										{
											flag = true;
										}
										if (flag)
										{
											destroyObject = true;
											KillTile(i, num17, false, false, false);
											KillTile(i, num17 + 1, false, false, false);
											KillTile(i, num17 + 2, false, false, false);
											sandbox.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
										}
										destroyObject = false;
									}
									return;
								}
								if (num9 == 11)
								{
									if (!destroyObject)
									{
										int num18 = 0;
										int num19 = i;
										int num20 = j;
										int frameX2 = (int)TileAt(i, j).FrameX;
										int frameY3 = (int)TileAt(i, j).FrameY;
										bool flag2 = false;
										if (frameX2 == 0)
										{
											num19 = i;
											num18 = 1;
										}
										else
										{
											if (frameX2 == 18)
											{
												num19 = i - 1;
												num18 = 1;
											}
											else
											{
												if (frameX2 == 36)
												{
													num19 = i + 1;
													num18 = -1;
												}
												else
												{
													if (frameX2 == 54)
													{
														num19 = i;
														num18 = -1;
													}
												}
											}
										}
										if (frameY3 == 0)
										{
											num20 = j;
										}
										else
										{
											if (frameY3 == 18)
											{
												num20 = j - 1;
											}
											else
											{
												if (frameY3 == 36)
												{
													num20 = j - 2;
												}
											}
										}
										if (!TileAt(num19, num20 - 1).Active ||
											!Main.tileSolid[(int)TileAt(num19, num20 - 1).Type] ||
											!TileAt(num19, num20 + 3).Active ||
											!Main.tileSolid[(int)TileAt(num19, num20 + 3).Type])
										{
											flag2 = true;
											destroyObject = true;
											sandbox.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
										}
										int num21 = num19;
										if (num18 == -1)
										{
											num21 = num19 - 1;
										}
										for (int l = num21; l < num21 + 2; l++)
										{
											for (int m = num20; m < num20 + 3; m++)
											{
												if (!flag2 && (TileAt(l, m).Type != 11 || !TileAt(l, m).Active))
												{
													destroyObject = true;
													sandbox.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
													flag2 = true;
													l = num21;
													m = num20;
												}
												if (flag2)
												{
													KillTile(l, m, false, false, false);
												}
											}
										}
										destroyObject = false;
									}
									return;
								}
								if (num9 == 34 || num9 == 35 || num9 == 36 || num9 == 106)
								{
									Check3x3(i, j, (int)((byte)num9));
									return;
								}
								if (num9 == 15 || num9 == 20)
								{
									Check1x2(i, j, (byte)num9);
									return;
								}
								if (num9 == 14 || num9 == 17 || num9 == 26 || num9 == 77 || num9 == 86 || num9 == 87 || num9 == 88 || num9 == 89)
								{
									Check3x2(i, j, (int)((byte)num9));
									return;
								}
								if (num9 == 16 || num9 == 18 || num9 == 29 || num9 == 103)
								{
									Check2x1(i, j, (byte)num9);
									return;
								}
								if (num9 == 13 || num9 == 33 || num9 == 49 || num9 == 50 || num9 == 78)
								{
									CheckOnTable1x1(i, j, (int)((byte)num9));
									return;
								}
								if (num9 == 21)
								{
									CheckChest(i, j, (int)((byte)num9));
									return;
								}
								if (num9 == 27)
								{
									CheckSunflower(i, j, 27);
									return;
								}
								if (num9 == 28)
								{
									CheckPot(i, j, 28);
									return;
								}
								if (num9 == 91)
								{
									CheckBanner(i, j, (byte)num9);
									return;
								}
								if (num9 == 92 || num9 == 93)
								{
									Check1xX(i, j, (byte)num9);
									return;
								}
								if (num9 == 104 || num9 == 105)
								{
									Check2xX(i, j, (byte)num9);
								}
								else
								{
									if (num9 == 101 || num9 == 102)
									{
										Check3x4(i, j, (int)((byte)num9));
										return;
									}
									if (num9 == 42)
									{
										Check1x2Top(i, j, (byte)num9);
										return;
									}
									if (num9 == 55 || num9 == 85)
									{
										CheckSign(i, j, num9);
										return;
									}
									if (num9 == 79 || num9 == 90)
									{
										Check4x2(i, j, num9);
										return;
									}
									if (num9 == 85 || num9 == 94 || num9 == 95 || num9 == 96 || num9 == 97 || num9 == 98 || num9 == 99 || num9 == 100)
									{
										Check2x2(i, j, num9);
										return;
									}
									if (num9 == 81)
									{
										if (num4 != -1 || num2 != -1 || num5 != -1)
										{
											KillTile(i, j, false, false, false);
											return;
										}
										if (num7 < 0 || !Main.tileSolid[num7])
										{
											KillTile(i, j, false, false, false);
										}
										return;
									}
									else
									{
										if (Main.tileAlch[num9])
										{
											CheckAlch(i, j);
											return;
										}
										if (num9 == 72)
										{
											if (num7 != num9 && num7 != 70)
											{
												KillTile(i, j, false, false, false);
											}
											else
											{
												if (num2 != num9 && TileAt(i, j).FrameX == 0)
												{
													TileAt(i, j).SetFrameNumber((byte)genRand.Next(3));
													if (TileAt(i, j).FrameNumber == 0)
													{
														TileAt(i, j).SetFrameX(18);
														TileAt(i, j).SetFrameY(0);
													}
													if (TileAt(i, j).FrameNumber == 1)
													{
														TileAt(i, j).SetFrameX(18);
														TileAt(i, j).SetFrameY(18);
													}
													if (TileAt(i, j).FrameNumber == 2)
													{
														TileAt(i, j).SetFrameX(18);
														TileAt(i, j).SetFrameY(36);
													}
												}
											}
										}
										else
										{
											if (num9 == 5)
											{
												if (num7 == 23)
												{
													num7 = 2;
												}
												if (num7 == 60)
												{
													num7 = 2;
												}
												if (TileAt(i, j).FrameX >= 22 && TileAt(i, j).FrameX <= 44 && TileAt(i, j).FrameY >= 132 && TileAt(i, j).FrameY <= 176)
												{
													if ((num4 != num9 && num5 != num9) || num7 != 2)
													{
														KillTile(i, j, false, false, false);
													}
												}
												else
												{
													if ((TileAt(i, j).FrameX == 88 && TileAt(i, j).FrameY >= 0 && TileAt(i, j).FrameY <= 44) || (TileAt(i, j).FrameX == 66 && TileAt(i, j).FrameY >= 66 && TileAt(i, j).FrameY <= 130) || (TileAt(i, j).FrameX == 110 && TileAt(i, j).FrameY >= 66 && TileAt(i, j).FrameY <= 110) || (TileAt(i, j).FrameX == 132 && TileAt(i, j).FrameY >= 0 && TileAt(i, j).FrameY <= 176))
													{
														if (num4 == num9 && num5 == num9)
														{
															if (TileAt(i, j).FrameNumber == 0)
															{
																TileAt(i, j).SetFrameX(110);
																TileAt(i, j).SetFrameY(66);
															}
															if (TileAt(i, j).FrameNumber == 1)
															{
																TileAt(i, j).SetFrameX(110);
																TileAt(i, j).SetFrameY(88);
															}
															if (TileAt(i, j).FrameNumber == 2)
															{
																TileAt(i, j).SetFrameX(110);
																TileAt(i, j).SetFrameY(110);
															}
														}
														else
														{
															if (num4 == num9)
															{
																if (TileAt(i, j).FrameNumber == 0)
																{
																	TileAt(i, j).SetFrameX(88);
																	TileAt(i, j).SetFrameY(0);
																}
																if (TileAt(i, j).FrameNumber == 1)
																{
																	TileAt(i, j).SetFrameX(88);
																	TileAt(i, j).SetFrameY(22);
																}
																if (TileAt(i, j).FrameNumber == 2)
																{
																	TileAt(i, j).SetFrameX(88);
																	TileAt(i, j).SetFrameY(44);
																}
															}
															else
															{
																if (num5 == num9)
																{
																	if (TileAt(i, j).FrameNumber == 0)
																	{
																		TileAt(i, j).SetFrameX(66);
																		TileAt(i, j).SetFrameY(66);
																	}
																	if (TileAt(i, j).FrameNumber == 1)
																	{
																		TileAt(i, j).SetFrameX(66);
																		TileAt(i, j).SetFrameY(88);
																	}
																	if (TileAt(i, j).FrameNumber == 2)
																	{
																		TileAt(i, j).SetFrameX(66);
																		TileAt(i, j).SetFrameY(110);
																	}
																}
																else
																{
																	if (TileAt(i, j).FrameNumber == 0)
																	{
																		TileAt(i, j).SetFrameX(0);
																		TileAt(i, j).SetFrameY(0);
																	}
																	if (TileAt(i, j).FrameNumber == 1)
																	{
																		TileAt(i, j).SetFrameX(0);
																		TileAt(i, j).SetFrameY(22);
																	}
																	if (TileAt(i, j).FrameNumber == 2)
																	{
																		TileAt(i, j).SetFrameX(0);
																		TileAt(i, j).SetFrameY(44);
																	}
																}
															}
														}
													}
												}
												if (TileAt(i, j).FrameY >= 132 && TileAt(i, j).FrameY <= 176 && (TileAt(i, j).FrameX == 0 || TileAt(i, j).FrameX == 66 || TileAt(i, j).FrameX == 88))
												{
													if (num7 != 2)
													{
														KillTile(i, j, false, false, false);
													}
													if (num4 != num9 && num5 != num9)
													{
														if (TileAt(i, j).FrameNumber == 0)
														{
															TileAt(i, j).SetFrameX(0);
															TileAt(i, j).SetFrameY(0);
														}
														if (TileAt(i, j).FrameNumber == 1)
														{
															TileAt(i, j).SetFrameX(0);
															TileAt(i, j).SetFrameY(22);
														}
														if (TileAt(i, j).FrameNumber == 2)
														{
															TileAt(i, j).SetFrameX(0);
															TileAt(i, j).SetFrameY(44);
														}
													}
													else
													{
														if (num4 != num9)
														{
															if (TileAt(i, j).FrameNumber == 0)
															{
																TileAt(i, j).SetFrameX(0);
																TileAt(i, j).SetFrameY(132);
															}
															if (TileAt(i, j).FrameNumber == 1)
															{
																TileAt(i, j).SetFrameX(0);
																TileAt(i, j).SetFrameY(154);
															}
															if (TileAt(i, j).FrameNumber == 2)
															{
																TileAt(i, j).SetFrameX(0);
																TileAt(i, j).SetFrameY(176);
															}
														}
														else
														{
															if (num5 != num9)
															{
																if (TileAt(i, j).FrameNumber == 0)
																{
																	TileAt(i, j).SetFrameX(66);
																	TileAt(i, j).SetFrameY(132);
																}
																if (TileAt(i, j).FrameNumber == 1)
																{
																	TileAt(i, j).SetFrameX(66);
																	TileAt(i, j).SetFrameY(154);
																}
																if (TileAt(i, j).FrameNumber == 2)
																{
																	TileAt(i, j).SetFrameX(66);
																	TileAt(i, j).SetFrameY(176);
																}
															}
															else
															{
																if (TileAt(i, j).FrameNumber == 0)
																{
																	TileAt(i, j).SetFrameX(88);
																	TileAt(i, j).SetFrameY(132);
																}
																if (TileAt(i, j).FrameNumber == 1)
																{
																	TileAt(i, j).SetFrameX(88);
																	TileAt(i, j).SetFrameY(154);
																}
																if (TileAt(i, j).FrameNumber == 2)
																{
																	TileAt(i, j).SetFrameX(88);
																	TileAt(i, j).SetFrameY(176);
																}
															}
														}
													}
												}
												if ((TileAt(i, j).FrameX == 66 && (TileAt(i, j).FrameY == 0 || TileAt(i, j).FrameY == 22 || TileAt(i, j).FrameY == 44)) || (TileAt(i, j).FrameX == 88 && (TileAt(i, j).FrameY == 66 || TileAt(i, j).FrameY == 88 || TileAt(i, j).FrameY == 110)) || (TileAt(i, j).FrameX == 44 && (TileAt(i, j).FrameY == 198 || TileAt(i, j).FrameY == 220 || TileAt(i, j).FrameY == 242)) || (TileAt(i, j).FrameX == 66 && (TileAt(i, j).FrameY == 198 || TileAt(i, j).FrameY == 220 || TileAt(i, j).FrameY == 242)))
												{
													if (num4 != num9 && num5 != num9)
													{
														KillTile(i, j, false, false, false);
													}
												}
												else
												{
													if (num7 == -1 || num7 == 23)
													{
														KillTile(i, j, false, false, false);
													}
													else
													{
														if (num2 != num9 && TileAt(i, j).FrameY < 198 && ((TileAt(i, j).FrameX != 22 && TileAt(i, j).FrameX != 44) || TileAt(i, j).FrameY < 132))
														{
															if (num4 == num9 || num5 == num9)
															{
																if (num7 == num9)
																{
																	if (num4 == num9 && num5 == num9)
																	{
																		if (TileAt(i, j).FrameNumber == 0)
																		{
																			TileAt(i, j).SetFrameX(132);
																			TileAt(i, j).SetFrameY(132);
																		}
																		if (TileAt(i, j).FrameNumber == 1)
																		{
																			TileAt(i, j).SetFrameX(132);
																			TileAt(i, j).SetFrameY(154);
																		}
																		if (TileAt(i, j).FrameNumber == 2)
																		{
																			TileAt(i, j).SetFrameX(132);
																			TileAt(i, j).SetFrameY(176);
																		}
																	}
																	else
																	{
																		if (num4 == num9)
																		{
																			if (TileAt(i, j).FrameNumber == 0)
																			{
																				TileAt(i, j).SetFrameX(132);
																				TileAt(i, j).SetFrameY(0);
																			}
																			if (TileAt(i, j).FrameNumber == 1)
																			{
																				TileAt(i, j).SetFrameX(132);
																				TileAt(i, j).SetFrameY(22);
																			}
																			if (TileAt(i, j).FrameNumber == 2)
																			{
																				TileAt(i, j).SetFrameX(132);
																				TileAt(i, j).SetFrameY(44);
																			}
																		}
																		else
																		{
																			if (num5 == num9)
																			{
																				if (TileAt(i, j).FrameNumber == 0)
																				{
																					TileAt(i, j).SetFrameX(132);
																					TileAt(i, j).SetFrameY(66);
																				}
																				if (TileAt(i, j).FrameNumber == 1)
																				{
																					TileAt(i, j).SetFrameX(132);
																					TileAt(i, j).SetFrameY(88);
																				}
																				if (TileAt(i, j).FrameNumber == 2)
																				{
																					TileAt(i, j).SetFrameX(132);
																					TileAt(i, j).SetFrameY(110);
																				}
																			}
																		}
																	}
																}
																else
																{
																	if (num4 == num9 && num5 == num9)
																	{
																		if (TileAt(i, j).FrameNumber == 0)
																		{
																			TileAt(i, j).SetFrameX(154);
																			TileAt(i, j).SetFrameY(132);
																		}
																		if (TileAt(i, j).FrameNumber == 1)
																		{
																			TileAt(i, j).SetFrameX(154);
																			TileAt(i, j).SetFrameY(154);
																		}
																		if (TileAt(i, j).FrameNumber == 2)
																		{
																			TileAt(i, j).SetFrameX(154);
																			TileAt(i, j).SetFrameY(176);
																		}
																	}
																	else
																	{
																		if (num4 == num9)
																		{
																			if (TileAt(i, j).FrameNumber == 0)
																			{
																				TileAt(i, j).SetFrameX(154);
																				TileAt(i, j).SetFrameY(0);
																			}
																			if (TileAt(i, j).FrameNumber == 1)
																			{
																				TileAt(i, j).SetFrameX(154);
																				TileAt(i, j).SetFrameY(22);
																			}
																			if (TileAt(i, j).FrameNumber == 2)
																			{
																				TileAt(i, j).SetFrameX(154);
																				TileAt(i, j).SetFrameY(44);
																			}
																		}
																		else
																		{
																			if (num5 == num9)
																			{
																				if (TileAt(i, j).FrameNumber == 0)
																				{
																					TileAt(i, j).SetFrameX(154);
																					TileAt(i, j).SetFrameY(66);
																				}
																				if (TileAt(i, j).FrameNumber == 1)
																				{
																					TileAt(i, j).SetFrameX(154);
																					TileAt(i, j).SetFrameY(88);
																				}
																				if (TileAt(i, j).FrameNumber == 2)
																				{
																					TileAt(i, j).SetFrameX(154);
																					TileAt(i, j).SetFrameY(110);
																				}
																			}
																		}
																	}
																}
															}
															else
															{
																if (TileAt(i, j).FrameNumber == 0)
																{
																	TileAt(i, j).SetFrameX(110);
																	TileAt(i, j).SetFrameY(0);
																}
																if (TileAt(i, j).FrameNumber == 1)
																{
																	TileAt(i, j).SetFrameX(110);
																	TileAt(i, j).SetFrameY(22);
																}
																if (TileAt(i, j).FrameNumber == 2)
																{
																	TileAt(i, j).SetFrameX(110);
																	TileAt(i, j).SetFrameY(44);
																}
															}
														}
													}
												}
												rectangle.X = (int)TileAt(i, j).FrameX;
												rectangle.Y = (int)TileAt(i, j).FrameY;
											}
										}
									}
								}
							}
						}
					}
					if (Main.tileFrameImportant[(int)TileAt(i, j).Type])
					{
						return;
					}
					int num22 = 0;
					if (resetFrame)
					{
						num22 = genRand.Next(0, 3);
						TileAt(i, j).SetFrameNumber((byte)num22);
					}
					else
					{
						num22 = (int)TileAt(i, j).FrameNumber;
					}
					if (num9 == 0)
					{
						if (num2 >= 0 && Main.tileMergeDirt[num2])
						{
							TileFrame(i, j - 1, false, false);
							if (mergeDown)
							{
								num2 = num9;
							}
						}
						if (num7 >= 0 && Main.tileMergeDirt[num7])
						{
							TileFrame(i, j + 1, false, false);
							if (mergeUp)
							{
								num7 = num9;
							}
						}
						if (num4 >= 0 && Main.tileMergeDirt[num4])
						{
							TileFrame(i - 1, j, false, false);
							if (mergeRight)
							{
								num4 = num9;
							}
						}
						if (num5 >= 0 && Main.tileMergeDirt[num5])
						{
							TileFrame(i + 1, j, false, false);
							if (mergeLeft)
							{
								num5 = num9;
							}
						}
						if (num >= 0 && Main.tileMergeDirt[num])
						{
							num = num9;
						}
						if (num3 >= 0 && Main.tileMergeDirt[num3])
						{
							num3 = num9;
						}
						if (num6 >= 0 && Main.tileMergeDirt[num6])
						{
							num6 = num9;
						}
						if (num8 >= 0 && Main.tileMergeDirt[num8])
						{
							num8 = num9;
						}
						if (num2 == 2)
						{
							num2 = num9;
						}
						if (num7 == 2)
						{
							num7 = num9;
						}
						if (num4 == 2)
						{
							num4 = num9;
						}
						if (num5 == 2)
						{
							num5 = num9;
						}
						if (num == 2)
						{
							num = num9;
						}
						if (num3 == 2)
						{
							num3 = num9;
						}
						if (num6 == 2)
						{
							num6 = num9;
						}
						if (num8 == 2)
						{
							num8 = num9;
						}
						if (num2 == 23)
						{
							num2 = num9;
						}
						if (num7 == 23)
						{
							num7 = num9;
						}
						if (num4 == 23)
						{
							num4 = num9;
						}
						if (num5 == 23)
						{
							num5 = num9;
						}
						if (num == 23)
						{
							num = num9;
						}
						if (num3 == 23)
						{
							num3 = num9;
						}
						if (num6 == 23)
						{
							num6 = num9;
						}
						if (num8 == 23)
						{
							num8 = num9;
						}
					}
					else
					{
						if (num9 == 57)
						{
							if (num2 == 58)
							{
								TileFrame(i, j - 1, false, false);
								if (mergeDown)
								{
									num2 = num9;
								}
							}
							if (num7 == 58)
							{
								TileFrame(i, j + 1, false, false);
								if (mergeUp)
								{
									num7 = num9;
								}
							}
							if (num4 == 58)
							{
								TileFrame(i - 1, j, false, false);
								if (mergeRight)
								{
									num4 = num9;
								}
							}
							if (num5 == 58)
							{
								TileFrame(i + 1, j, false, false);
								if (mergeLeft)
								{
									num5 = num9;
								}
							}
							if (num == 58)
							{
								num = num9;
							}
							if (num3 == 58)
							{
								num3 = num9;
							}
							if (num6 == 58)
							{
								num6 = num9;
							}
							if (num8 == 58)
							{
								num8 = num9;
							}
						}
						else
						{
							if (num9 == 59)
							{
								if (num2 == 60)
								{
									num2 = num9;
								}
								if (num7 == 60)
								{
									num7 = num9;
								}
								if (num4 == 60)
								{
									num4 = num9;
								}
								if (num5 == 60)
								{
									num5 = num9;
								}
								if (num == 60)
								{
									num = num9;
								}
								if (num3 == 60)
								{
									num3 = num9;
								}
								if (num6 == 60)
								{
									num6 = num9;
								}
								if (num8 == 60)
								{
									num8 = num9;
								}
								if (num2 == 70)
								{
									num2 = num9;
								}
								if (num7 == 70)
								{
									num7 = num9;
								}
								if (num4 == 70)
								{
									num4 = num9;
								}
								if (num5 == 70)
								{
									num5 = num9;
								}
								if (num == 70)
								{
									num = num9;
								}
								if (num3 == 70)
								{
									num3 = num9;
								}
								if (num6 == 70)
								{
									num6 = num9;
								}
								if (num8 == 70)
								{
									num8 = num9;
								}
							}
						}
					}
					if (Main.tileMergeDirt[num9])
					{
						if (num2 == 0)
						{
							num2 = -2;
						}
						if (num7 == 0)
						{
							num7 = -2;
						}
						if (num4 == 0)
						{
							num4 = -2;
						}
						if (num5 == 0)
						{
							num5 = -2;
						}
						if (num == 0)
						{
							num = -2;
						}
						if (num3 == 0)
						{
							num3 = -2;
						}
						if (num6 == 0)
						{
							num6 = -2;
						}
						if (num8 == 0)
						{
							num8 = -2;
						}
					}
					else
					{
						if (num9 == 58)
						{
							if (num2 == 57)
							{
								num2 = -2;
							}
							if (num7 == 57)
							{
								num7 = -2;
							}
							if (num4 == 57)
							{
								num4 = -2;
							}
							if (num5 == 57)
							{
								num5 = -2;
							}
							if (num == 57)
							{
								num = -2;
							}
							if (num3 == 57)
							{
								num3 = -2;
							}
							if (num6 == 57)
							{
								num6 = -2;
							}
							if (num8 == 57)
							{
								num8 = -2;
							}
						}
						else
						{
							if (num9 == 59)
							{
								if (num2 == 1)
								{
									num2 = -2;
								}
								if (num7 == 1)
								{
									num7 = -2;
								}
								if (num4 == 1)
								{
									num4 = -2;
								}
								if (num5 == 1)
								{
									num5 = -2;
								}
								if (num == 1)
								{
									num = -2;
								}
								if (num3 == 1)
								{
									num3 = -2;
								}
								if (num6 == 1)
								{
									num6 = -2;
								}
								if (num8 == 1)
								{
									num8 = -2;
								}
							}
						}
					}
					if (num9 == 32)
					{
						if (num7 == 23)
						{
							num7 = num9;
						}
					}
					else
					{
						if (num9 == 69)
						{
							if (num7 == 60)
							{
								num7 = num9;
							}
						}
						else
						{
							if (num9 == 51)
							{
								if (num2 > -1 && !Main.tileNoAttach[num2])
								{
									num2 = num9;
								}
								if (num7 > -1 && !Main.tileNoAttach[num7])
								{
									num7 = num9;
								}
								if (num4 > -1 && !Main.tileNoAttach[num4])
								{
									num4 = num9;
								}
								if (num5 > -1 && !Main.tileNoAttach[num5])
								{
									num5 = num9;
								}
								if (num > -1 && !Main.tileNoAttach[num])
								{
									num = num9;
								}
								if (num3 > -1 && !Main.tileNoAttach[num3])
								{
									num3 = num9;
								}
								if (num6 > -1 && !Main.tileNoAttach[num6])
								{
									num6 = num9;
								}
								if (num8 > -1 && !Main.tileNoAttach[num8])
								{
									num8 = num9;
								}
							}
						}
					}
					if (num2 > -1 && !Main.tileSolid[num2] && num2 != num9)
					{
						num2 = -1;
					}
					if (num7 > -1 && !Main.tileSolid[num7] && num7 != num9)
					{
						num7 = -1;
					}
					if (num4 > -1 && !Main.tileSolid[num4] && num4 != num9)
					{
						num4 = -1;
					}
					if (num5 > -1 && !Main.tileSolid[num5] && num5 != num9)
					{
						num5 = -1;
					}
					if (num > -1 && !Main.tileSolid[num] && num != num9)
					{
						num = -1;
					}
					if (num3 > -1 && !Main.tileSolid[num3] && num3 != num9)
					{
						num3 = -1;
					}
					if (num6 > -1 && !Main.tileSolid[num6] && num6 != num9)
					{
						num6 = -1;
					}
					if (num8 > -1 && !Main.tileSolid[num8] && num8 != num9)
					{
						num8 = -1;
					}
					if (num9 == 2 || num9 == 23 || num9 == 60 || num9 == 70)
					{
						int num23 = 0;
						if (num9 == 60 || num9 == 70)
						{
							num23 = 59;
						}
						else
						{
							if (num9 == 2)
							{
								if (num2 == 23)
								{
									num2 = num23;
								}
								if (num7 == 23)
								{
									num7 = num23;
								}
								if (num4 == 23)
								{
									num4 = num23;
								}
								if (num5 == 23)
								{
									num5 = num23;
								}
								if (num == 23)
								{
									num = num23;
								}
								if (num3 == 23)
								{
									num3 = num23;
								}
								if (num6 == 23)
								{
									num6 = num23;
								}
								if (num8 == 23)
								{
									num8 = num23;
								}
							}
							else
							{
								if (num9 == 23)
								{
									if (num2 == 2)
									{
										num2 = num23;
									}
									if (num7 == 2)
									{
										num7 = num23;
									}
									if (num4 == 2)
									{
										num4 = num23;
									}
									if (num5 == 2)
									{
										num5 = num23;
									}
									if (num == 2)
									{
										num = num23;
									}
									if (num3 == 2)
									{
										num3 = num23;
									}
									if (num6 == 2)
									{
										num6 = num23;
									}
									if (num8 == 2)
									{
										num8 = num23;
									}
								}
							}
						}
						if (num2 != num9 && num2 != num23 && (num7 == num9 || num7 == num23))
						{
							if (num4 == num23 && num5 == num9)
							{
								if (num22 == 0)
								{
									rectangle.X = 0;
									rectangle.Y = 198;
								}
								if (num22 == 1)
								{
									rectangle.X = 18;
									rectangle.Y = 198;
								}
								if (num22 == 2)
								{
									rectangle.X = 36;
									rectangle.Y = 198;
								}
							}
							else
							{
								if (num4 == num9 && num5 == num23)
								{
									if (num22 == 0)
									{
										rectangle.X = 54;
										rectangle.Y = 198;
									}
									if (num22 == 1)
									{
										rectangle.X = 72;
										rectangle.Y = 198;
									}
									if (num22 == 2)
									{
										rectangle.X = 90;
										rectangle.Y = 198;
									}
								}
							}
						}
						else
						{
							if (num7 != num9 && num7 != num23 && (num2 == num9 || num2 == num23))
							{
								if (num4 == num23 && num5 == num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 0;
										rectangle.Y = 216;
									}
									if (num22 == 1)
									{
										rectangle.X = 18;
										rectangle.Y = 216;
									}
									if (num22 == 2)
									{
										rectangle.X = 36;
										rectangle.Y = 216;
									}
								}
								else
								{
									if (num4 == num9 && num5 == num23)
									{
										if (num22 == 0)
										{
											rectangle.X = 54;
											rectangle.Y = 216;
										}
										if (num22 == 1)
										{
											rectangle.X = 72;
											rectangle.Y = 216;
										}
										if (num22 == 2)
										{
											rectangle.X = 90;
											rectangle.Y = 216;
										}
									}
								}
							}
							else
							{
								if (num4 != num9 && num4 != num23 && (num5 == num9 || num5 == num23))
								{
									if (num2 == num23 && num7 == num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 72;
											rectangle.Y = 144;
										}
										if (num22 == 1)
										{
											rectangle.X = 72;
											rectangle.Y = 162;
										}
										if (num22 == 2)
										{
											rectangle.X = 72;
											rectangle.Y = 180;
										}
									}
									else
									{
										if (num7 == num9 && num5 == num2)
										{
											if (num22 == 0)
											{
												rectangle.X = 72;
												rectangle.Y = 90;
											}
											if (num22 == 1)
											{
												rectangle.X = 72;
												rectangle.Y = 108;
											}
											if (num22 == 2)
											{
												rectangle.X = 72;
												rectangle.Y = 126;
											}
										}
									}
								}
								else
								{
									if (num5 != num9 && num5 != num23 && (num4 == num9 || num4 == num23))
									{
										if (num2 == num23 && num7 == num9)
										{
											if (num22 == 0)
											{
												rectangle.X = 90;
												rectangle.Y = 144;
											}
											if (num22 == 1)
											{
												rectangle.X = 90;
												rectangle.Y = 162;
											}
											if (num22 == 2)
											{
												rectangle.X = 90;
												rectangle.Y = 180;
											}
										}
										else
										{
											if (num7 == num9 && num5 == num2)
											{
												if (num22 == 0)
												{
													rectangle.X = 90;
													rectangle.Y = 90;
												}
												if (num22 == 1)
												{
													rectangle.X = 90;
													rectangle.Y = 108;
												}
												if (num22 == 2)
												{
													rectangle.X = 90;
													rectangle.Y = 126;
												}
											}
										}
									}
									else
									{
										if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num9)
										{
											if (num != num9 && num3 != num9 && num6 != num9 && num8 != num9)
											{
												if (num8 == num23)
												{
													if (num22 == 0)
													{
														rectangle.X = 108;
														rectangle.Y = 324;
													}
													if (num22 == 1)
													{
														rectangle.X = 126;
														rectangle.Y = 324;
													}
													if (num22 == 2)
													{
														rectangle.X = 144;
														rectangle.Y = 324;
													}
												}
												else
												{
													if (num3 == num23)
													{
														if (num22 == 0)
														{
															rectangle.X = 108;
															rectangle.Y = 342;
														}
														if (num22 == 1)
														{
															rectangle.X = 126;
															rectangle.Y = 342;
														}
														if (num22 == 2)
														{
															rectangle.X = 144;
															rectangle.Y = 342;
														}
													}
													else
													{
														if (num6 == num23)
														{
															if (num22 == 0)
															{
																rectangle.X = 108;
																rectangle.Y = 360;
															}
															if (num22 == 1)
															{
																rectangle.X = 126;
																rectangle.Y = 360;
															}
															if (num22 == 2)
															{
																rectangle.X = 144;
																rectangle.Y = 360;
															}
														}
														else
														{
															if (num == num23)
															{
																if (num22 == 0)
																{
																	rectangle.X = 108;
																	rectangle.Y = 378;
																}
																if (num22 == 1)
																{
																	rectangle.X = 126;
																	rectangle.Y = 378;
																}
																if (num22 == 2)
																{
																	rectangle.X = 144;
																	rectangle.Y = 378;
																}
															}
															else
															{
																if (num22 == 0)
																{
																	rectangle.X = 144;
																	rectangle.Y = 234;
																}
																if (num22 == 1)
																{
																	rectangle.X = 198;
																	rectangle.Y = 234;
																}
																if (num22 == 2)
																{
																	rectangle.X = 252;
																	rectangle.Y = 234;
																}
															}
														}
													}
												}
											}
											else
											{
												if (num != num9 && num8 != num9)
												{
													if (num22 == 0)
													{
														rectangle.X = 36;
														rectangle.Y = 306;
													}
													if (num22 == 1)
													{
														rectangle.X = 54;
														rectangle.Y = 306;
													}
													if (num22 == 2)
													{
														rectangle.X = 72;
														rectangle.Y = 306;
													}
												}
												else
												{
													if (num3 != num9 && num6 != num9)
													{
														if (num22 == 0)
														{
															rectangle.X = 90;
															rectangle.Y = 306;
														}
														if (num22 == 1)
														{
															rectangle.X = 108;
															rectangle.Y = 306;
														}
														if (num22 == 2)
														{
															rectangle.X = 126;
															rectangle.Y = 306;
														}
													}
													else
													{
														if (num != num9 && num3 == num9 && num6 == num9 && num8 == num9)
														{
															if (num22 == 0)
															{
																rectangle.X = 54;
																rectangle.Y = 108;
															}
															if (num22 == 1)
															{
																rectangle.X = 54;
																rectangle.Y = 144;
															}
															if (num22 == 2)
															{
																rectangle.X = 54;
																rectangle.Y = 180;
															}
														}
														else
														{
															if (num == num9 && num3 != num9 && num6 == num9 && num8 == num9)
															{
																if (num22 == 0)
																{
																	rectangle.X = 36;
																	rectangle.Y = 108;
																}
																if (num22 == 1)
																{
																	rectangle.X = 36;
																	rectangle.Y = 144;
																}
																if (num22 == 2)
																{
																	rectangle.X = 36;
																	rectangle.Y = 180;
																}
															}
															else
															{
																if (num == num9 && num3 == num9 && num6 != num9 && num8 == num9)
																{
																	if (num22 == 0)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 90;
																	}
																	if (num22 == 1)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 126;
																	}
																	if (num22 == 2)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 162;
																	}
																}
																else
																{
																	if (num == num9 && num3 == num9 && num6 == num9 && num8 != num9)
																	{
																		if (num22 == 0)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 90;
																		}
																		if (num22 == 1)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 126;
																		}
																		if (num22 == 2)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 162;
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
											if (num2 == num9 && num7 == num23 && num4 == num9 && num5 == num9 && num == -1 && num3 == -1)
											{
												if (num22 == 0)
												{
													rectangle.X = 108;
													rectangle.Y = 18;
												}
												if (num22 == 1)
												{
													rectangle.X = 126;
													rectangle.Y = 18;
												}
												if (num22 == 2)
												{
													rectangle.X = 144;
													rectangle.Y = 18;
												}
											}
											else
											{
												if (num2 == num23 && num7 == num9 && num4 == num9 && num5 == num9 && num6 == -1 && num8 == -1)
												{
													if (num22 == 0)
													{
														rectangle.X = 108;
														rectangle.Y = 36;
													}
													if (num22 == 1)
													{
														rectangle.X = 126;
														rectangle.Y = 36;
													}
													if (num22 == 2)
													{
														rectangle.X = 144;
														rectangle.Y = 36;
													}
												}
												else
												{
													if (num2 == num9 && num7 == num9 && num4 == num23 && num5 == num9 && num3 == -1 && num8 == -1)
													{
														if (num22 == 0)
														{
															rectangle.X = 198;
															rectangle.Y = 0;
														}
														if (num22 == 1)
														{
															rectangle.X = 198;
															rectangle.Y = 18;
														}
														if (num22 == 2)
														{
															rectangle.X = 198;
															rectangle.Y = 36;
														}
													}
													else
													{
														if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num23 && num == -1 && num6 == -1)
														{
															if (num22 == 0)
															{
																rectangle.X = 180;
																rectangle.Y = 0;
															}
															if (num22 == 1)
															{
																rectangle.X = 180;
																rectangle.Y = 18;
															}
															if (num22 == 2)
															{
																rectangle.X = 180;
																rectangle.Y = 36;
															}
														}
														else
														{
															if (num2 == num9 && num7 == num23 && num4 == num9 && num5 == num9)
															{
																if (num3 != -1)
																{
																	if (num22 == 0)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 108;
																	}
																	if (num22 == 1)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 144;
																	}
																	if (num22 == 2)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 180;
																	}
																}
																else
																{
																	if (num != -1)
																	{
																		if (num22 == 0)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 108;
																		}
																		if (num22 == 1)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 144;
																		}
																		if (num22 == 2)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 180;
																		}
																	}
																}
															}
															else
															{
																if (num2 == num23 && num7 == num9 && num4 == num9 && num5 == num9)
																{
																	if (num8 != -1)
																	{
																		if (num22 == 0)
																		{
																			rectangle.X = 54;
																			rectangle.Y = 90;
																		}
																		if (num22 == 1)
																		{
																			rectangle.X = 54;
																			rectangle.Y = 126;
																		}
																		if (num22 == 2)
																		{
																			rectangle.X = 54;
																			rectangle.Y = 162;
																		}
																	}
																	else
																	{
																		if (num6 != -1)
																		{
																			if (num22 == 0)
																			{
																				rectangle.X = 36;
																				rectangle.Y = 90;
																			}
																			if (num22 == 1)
																			{
																				rectangle.X = 36;
																				rectangle.Y = 126;
																			}
																			if (num22 == 2)
																			{
																				rectangle.X = 36;
																				rectangle.Y = 162;
																			}
																		}
																	}
																}
																else
																{
																	if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num23)
																	{
																		if (num != -1)
																		{
																			if (num22 == 0)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 90;
																			}
																			if (num22 == 1)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 126;
																			}
																			if (num22 == 2)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 162;
																			}
																		}
																		else
																		{
																			if (num6 != -1)
																			{
																				if (num22 == 0)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 108;
																				}
																				if (num22 == 1)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 144;
																				}
																				if (num22 == 2)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 180;
																				}
																			}
																		}
																	}
																	else
																	{
																		if (num2 == num9 && num7 == num9 && num4 == num23 && num5 == num9)
																		{
																			if (num3 != -1)
																			{
																				if (num22 == 0)
																				{
																					rectangle.X = 36;
																					rectangle.Y = 90;
																				}
																				if (num22 == 1)
																				{
																					rectangle.X = 36;
																					rectangle.Y = 126;
																				}
																				if (num22 == 2)
																				{
																					rectangle.X = 36;
																					rectangle.Y = 162;
																				}
																			}
																			else
																			{
																				if (num8 != -1)
																				{
																					if (num22 == 0)
																					{
																						rectangle.X = 36;
																						rectangle.Y = 108;
																					}
																					if (num22 == 1)
																					{
																						rectangle.X = 36;
																						rectangle.Y = 144;
																					}
																					if (num22 == 2)
																					{
																						rectangle.X = 36;
																						rectangle.Y = 180;
																					}
																				}
																			}
																		}
																		else
																		{
																			if ((num2 == num23 && num7 == num9 && num4 == num9 && num5 == num9) || (num2 == num9 && num7 == num23 && num4 == num9 && num5 == num9) || (num2 == num9 && num7 == num9 && num4 == num23 && num5 == num9) || (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num23))
																			{
																				if (num22 == 0)
																				{
																					rectangle.X = 18;
																					rectangle.Y = 18;
																				}
																				if (num22 == 1)
																				{
																					rectangle.X = 36;
																					rectangle.Y = 18;
																				}
																				if (num22 == 2)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 18;
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
						if ((num2 == num9 || num2 == num23) && (num7 == num9 || num7 == num23) && (num4 == num9 || num4 == num23) && (num5 == num9 || num5 == num23))
						{
							if (num != num9 && num != num23 && (num3 == num9 || num3 == num23) && (num6 == num9 || num6 == num23) && (num8 == num9 || num8 == num23))
							{
								if (num22 == 0)
								{
									rectangle.X = 54;
									rectangle.Y = 108;
								}
								if (num22 == 1)
								{
									rectangle.X = 54;
									rectangle.Y = 144;
								}
								if (num22 == 2)
								{
									rectangle.X = 54;
									rectangle.Y = 180;
								}
							}
							else
							{
								if (num3 != num9 && num3 != num23 && (num == num9 || num == num23) && (num6 == num9 || num6 == num23) && (num8 == num9 || num8 == num23))
								{
									if (num22 == 0)
									{
										rectangle.X = 36;
										rectangle.Y = 108;
									}
									if (num22 == 1)
									{
										rectangle.X = 36;
										rectangle.Y = 144;
									}
									if (num22 == 2)
									{
										rectangle.X = 36;
										rectangle.Y = 180;
									}
								}
								else
								{
									if (num6 != num9 && num6 != num23 && (num == num9 || num == num23) && (num3 == num9 || num3 == num23) && (num8 == num9 || num8 == num23))
									{
										if (num22 == 0)
										{
											rectangle.X = 54;
											rectangle.Y = 90;
										}
										if (num22 == 1)
										{
											rectangle.X = 54;
											rectangle.Y = 126;
										}
										if (num22 == 2)
										{
											rectangle.X = 54;
											rectangle.Y = 162;
										}
									}
									else
									{
										if (num8 != num9 && num8 != num23 && (num == num9 || num == num23) && (num6 == num9 || num6 == num23) && (num3 == num9 || num3 == num23))
										{
											if (num22 == 0)
											{
												rectangle.X = 36;
												rectangle.Y = 90;
											}
											if (num22 == 1)
											{
												rectangle.X = 36;
												rectangle.Y = 126;
											}
											if (num22 == 2)
											{
												rectangle.X = 36;
												rectangle.Y = 162;
											}
										}
									}
								}
							}
						}
						if (num2 != num23 && num2 != num9 && num7 == num9 && num4 != num23 && num4 != num9 && num5 == num9 && num8 != num23 && num8 != num9)
						{
							if (num22 == 0)
							{
								rectangle.X = 90;
								rectangle.Y = 270;
							}
							if (num22 == 1)
							{
								rectangle.X = 108;
								rectangle.Y = 270;
							}
							if (num22 == 2)
							{
								rectangle.X = 126;
								rectangle.Y = 270;
							}
						}
						else
						{
							if (num2 != num23 && num2 != num9 && num7 == num9 && num4 == num9 && num5 != num23 && num5 != num9 && num6 != num23 && num6 != num9)
							{
								if (num22 == 0)
								{
									rectangle.X = 144;
									rectangle.Y = 270;
								}
								if (num22 == 1)
								{
									rectangle.X = 162;
									rectangle.Y = 270;
								}
								if (num22 == 2)
								{
									rectangle.X = 180;
									rectangle.Y = 270;
								}
							}
							else
							{
								if (num7 != num23 && num7 != num9 && num2 == num9 && num4 != num23 && num4 != num9 && num5 == num9 && num3 != num23 && num3 != num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 90;
										rectangle.Y = 288;
									}
									if (num22 == 1)
									{
										rectangle.X = 108;
										rectangle.Y = 288;
									}
									if (num22 == 2)
									{
										rectangle.X = 126;
										rectangle.Y = 288;
									}
								}
								else
								{
									if (num7 != num23 && num7 != num9 && num2 == num9 && num4 == num9 && num5 != num23 && num5 != num9 && num != num23 && num != num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 144;
											rectangle.Y = 288;
										}
										if (num22 == 1)
										{
											rectangle.X = 162;
											rectangle.Y = 288;
										}
										if (num22 == 2)
										{
											rectangle.X = 180;
											rectangle.Y = 288;
										}
									}
									else
									{
										if (num2 != num9 && num2 != num23 && num7 == num9 && num4 == num9 && num5 == num9 && num6 != num9 && num6 != num23 && num8 != num9 && num8 != num23)
										{
											if (num22 == 0)
											{
												rectangle.X = 144;
												rectangle.Y = 216;
											}
											if (num22 == 1)
											{
												rectangle.X = 198;
												rectangle.Y = 216;
											}
											if (num22 == 2)
											{
												rectangle.X = 252;
												rectangle.Y = 216;
											}
										}
										else
										{
											if (num7 != num9 && num7 != num23 && num2 == num9 && num4 == num9 && num5 == num9 && num != num9 && num != num23 && num3 != num9 && num3 != num23)
											{
												if (num22 == 0)
												{
													rectangle.X = 144;
													rectangle.Y = 252;
												}
												if (num22 == 1)
												{
													rectangle.X = 198;
													rectangle.Y = 252;
												}
												if (num22 == 2)
												{
													rectangle.X = 252;
													rectangle.Y = 252;
												}
											}
											else
											{
												if (num4 != num9 && num4 != num23 && num7 == num9 && num2 == num9 && num5 == num9 && num3 != num9 && num3 != num23 && num8 != num9 && num8 != num23)
												{
													if (num22 == 0)
													{
														rectangle.X = 126;
														rectangle.Y = 234;
													}
													if (num22 == 1)
													{
														rectangle.X = 180;
														rectangle.Y = 234;
													}
													if (num22 == 2)
													{
														rectangle.X = 234;
														rectangle.Y = 234;
													}
												}
												else
												{
													if (num5 != num9 && num5 != num23 && num7 == num9 && num2 == num9 && num4 == num9 && num != num9 && num != num23 && num6 != num9 && num6 != num23)
													{
														if (num22 == 0)
														{
															rectangle.X = 162;
															rectangle.Y = 234;
														}
														if (num22 == 1)
														{
															rectangle.X = 216;
															rectangle.Y = 234;
														}
														if (num22 == 2)
														{
															rectangle.X = 270;
															rectangle.Y = 234;
														}
													}
													else
													{
														if (num2 != num23 && num2 != num9 && (num7 == num23 || num7 == num9) && num4 == num23 && num5 == num23)
														{
															if (num22 == 0)
															{
																rectangle.X = 36;
																rectangle.Y = 270;
															}
															if (num22 == 1)
															{
																rectangle.X = 54;
																rectangle.Y = 270;
															}
															if (num22 == 2)
															{
																rectangle.X = 72;
																rectangle.Y = 270;
															}
														}
														else
														{
															if (num7 != num23 && num7 != num9 && (num2 == num23 || num2 == num9) && num4 == num23 && num5 == num23)
															{
																if (num22 == 0)
																{
																	rectangle.X = 36;
																	rectangle.Y = 288;
																}
																if (num22 == 1)
																{
																	rectangle.X = 54;
																	rectangle.Y = 288;
																}
																if (num22 == 2)
																{
																	rectangle.X = 72;
																	rectangle.Y = 288;
																}
															}
															else
															{
																if (num4 != num23 && num4 != num9 && (num5 == num23 || num5 == num9) && num2 == num23 && num7 == num23)
																{
																	if (num22 == 0)
																	{
																		rectangle.X = 0;
																		rectangle.Y = 270;
																	}
																	if (num22 == 1)
																	{
																		rectangle.X = 0;
																		rectangle.Y = 288;
																	}
																	if (num22 == 2)
																	{
																		rectangle.X = 0;
																		rectangle.Y = 306;
																	}
																}
																else
																{
																	if (num5 != num23 && num5 != num9 && (num4 == num23 || num4 == num9) && num2 == num23 && num7 == num23)
																	{
																		if (num22 == 0)
																		{
																			rectangle.X = 18;
																			rectangle.Y = 270;
																		}
																		if (num22 == 1)
																		{
																			rectangle.X = 18;
																			rectangle.Y = 288;
																		}
																		if (num22 == 2)
																		{
																			rectangle.X = 18;
																			rectangle.Y = 306;
																		}
																	}
																	else
																	{
																		if (num2 == num9 && num7 == num23 && num4 == num23 && num5 == num23)
																		{
																			if (num22 == 0)
																			{
																				rectangle.X = 198;
																				rectangle.Y = 288;
																			}
																			if (num22 == 1)
																			{
																				rectangle.X = 216;
																				rectangle.Y = 288;
																			}
																			if (num22 == 2)
																			{
																				rectangle.X = 234;
																				rectangle.Y = 288;
																			}
																		}
																		else
																		{
																			if (num2 == num23 && num7 == num9 && num4 == num23 && num5 == num23)
																			{
																				if (num22 == 0)
																				{
																					rectangle.X = 198;
																					rectangle.Y = 270;
																				}
																				if (num22 == 1)
																				{
																					rectangle.X = 216;
																					rectangle.Y = 270;
																				}
																				if (num22 == 2)
																				{
																					rectangle.X = 234;
																					rectangle.Y = 270;
																				}
																			}
																			else
																			{
																				if (num2 == num23 && num7 == num23 && num4 == num9 && num5 == num23)
																				{
																					if (num22 == 0)
																					{
																						rectangle.X = 198;
																						rectangle.Y = 306;
																					}
																					if (num22 == 1)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 306;
																					}
																					if (num22 == 2)
																					{
																						rectangle.X = 234;
																						rectangle.Y = 306;
																					}
																				}
																				else
																				{
																					if (num2 == num23 && num7 == num23 && num4 == num23 && num5 == num9)
																					{
																						if (num22 == 0)
																						{
																							rectangle.X = 144;
																							rectangle.Y = 306;
																						}
																						if (num22 == 1)
																						{
																							rectangle.X = 162;
																							rectangle.Y = 306;
																						}
																						if (num22 == 2)
																						{
																							rectangle.X = 180;
																							rectangle.Y = 306;
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
						if (num2 != num9 && num2 != num23 && num7 == num9 && num4 == num9 && num5 == num9)
						{
							if ((num6 == num23 || num6 == num9) && num8 != num23 && num8 != num9)
							{
								if (num22 == 0)
								{
									rectangle.X = 0;
									rectangle.Y = 324;
								}
								if (num22 == 1)
								{
									rectangle.X = 18;
									rectangle.Y = 324;
								}
								if (num22 == 2)
								{
									rectangle.X = 36;
									rectangle.Y = 324;
								}
							}
							else
							{
								if ((num8 == num23 || num8 == num9) && num6 != num23 && num6 != num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 54;
										rectangle.Y = 324;
									}
									if (num22 == 1)
									{
										rectangle.X = 72;
										rectangle.Y = 324;
									}
									if (num22 == 2)
									{
										rectangle.X = 90;
										rectangle.Y = 324;
									}
								}
							}
						}
						else
						{
							if (num7 != num9 && num7 != num23 && num2 == num9 && num4 == num9 && num5 == num9)
							{
								if ((num == num23 || num == num9) && num3 != num23 && num3 != num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 0;
										rectangle.Y = 342;
									}
									if (num22 == 1)
									{
										rectangle.X = 18;
										rectangle.Y = 342;
									}
									if (num22 == 2)
									{
										rectangle.X = 36;
										rectangle.Y = 342;
									}
								}
								else
								{
									if ((num3 == num23 || num3 == num9) && num != num23 && num != num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 54;
											rectangle.Y = 342;
										}
										if (num22 == 1)
										{
											rectangle.X = 72;
											rectangle.Y = 342;
										}
										if (num22 == 2)
										{
											rectangle.X = 90;
											rectangle.Y = 342;
										}
									}
								}
							}
							else
							{
								if (num4 != num9 && num4 != num23 && num2 == num9 && num7 == num9 && num5 == num9)
								{
									if ((num3 == num23 || num3 == num9) && num8 != num23 && num8 != num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 54;
											rectangle.Y = 360;
										}
										if (num22 == 1)
										{
											rectangle.X = 72;
											rectangle.Y = 360;
										}
										if (num22 == 2)
										{
											rectangle.X = 90;
											rectangle.Y = 360;
										}
									}
									else
									{
										if ((num8 == num23 || num8 == num9) && num3 != num23 && num3 != num9)
										{
											if (num22 == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 360;
											}
											if (num22 == 1)
											{
												rectangle.X = 18;
												rectangle.Y = 360;
											}
											if (num22 == 2)
											{
												rectangle.X = 36;
												rectangle.Y = 360;
											}
										}
									}
								}
								else
								{
									if (num5 != num9 && num5 != num23 && num2 == num9 && num7 == num9 && num4 == num9)
									{
										if ((num == num23 || num == num9) && num6 != num23 && num6 != num9)
										{
											if (num22 == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 378;
											}
											if (num22 == 1)
											{
												rectangle.X = 18;
												rectangle.Y = 378;
											}
											if (num22 == 2)
											{
												rectangle.X = 36;
												rectangle.Y = 378;
											}
										}
										else
										{
											if ((num6 == num23 || num6 == num9) && num != num23 && num != num9)
											{
												if (num22 == 0)
												{
													rectangle.X = 54;
													rectangle.Y = 378;
												}
												if (num22 == 1)
												{
													rectangle.X = 72;
													rectangle.Y = 378;
												}
												if (num22 == 2)
												{
													rectangle.X = 90;
													rectangle.Y = 378;
												}
											}
										}
									}
								}
							}
						}
						if ((num2 == num9 || num2 == num23) && (num7 == num9 || num7 == num23) && (num4 == num9 || num4 == num23) && (num5 == num9 || num5 == num23) && num != -1 && num3 != -1 && num6 != -1 && num8 != -1)
						{
							if (num22 == 0)
							{
								rectangle.X = 18;
								rectangle.Y = 18;
							}
							if (num22 == 1)
							{
								rectangle.X = 36;
								rectangle.Y = 18;
							}
							if (num22 == 2)
							{
								rectangle.X = 54;
								rectangle.Y = 18;
							}
						}
						if (num2 == num23)
						{
							num2 = -2;
						}
						if (num7 == num23)
						{
							num7 = -2;
						}
						if (num4 == num23)
						{
							num4 = -2;
						}
						if (num5 == num23)
						{
							num5 = -2;
						}
						if (num == num23)
						{
							num = -2;
						}
						if (num3 == num23)
						{
							num3 = -2;
						}
						if (num6 == num23)
						{
							num6 = -2;
						}
						if (num8 == num23)
						{
							num8 = -2;
						}
					}
					if ((num9 == 1 || num9 == 2 || num9 == 6 || num9 == 7 || num9 == 8 || num9 == 9 || num9 == 22 || num9 == 23 || num9 == 25 || num9 == 37 || num9 == 40 || num9 == 53 || num9 == 56 || num9 == 58 || num9 == 59 || num9 == 60 || num9 == 70) && rectangle.X == -1 && rectangle.Y == -1)
					{
						if (num2 >= 0 && num2 != num9)
						{
							num2 = -1;
						}
						if (num7 >= 0 && num7 != num9)
						{
							num7 = -1;
						}
						if (num4 >= 0 && num4 != num9)
						{
							num4 = -1;
						}
						if (num5 >= 0 && num5 != num9)
						{
							num5 = -1;
						}
						if (num2 != -1 && num7 != -1 && num4 != -1 && num5 != -1)
						{
							if (num2 == -2 && num7 == num9 && num4 == num9 && num5 == num9)
							{
								if (num22 == 0)
								{
									rectangle.X = 144;
									rectangle.Y = 108;
								}
								if (num22 == 1)
								{
									rectangle.X = 162;
									rectangle.Y = 108;
								}
								if (num22 == 2)
								{
									rectangle.X = 180;
									rectangle.Y = 108;
								}
								mergeUp = true;
							}
							else
							{
								if (num2 == num9 && num7 == -2 && num4 == num9 && num5 == num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 144;
										rectangle.Y = 90;
									}
									if (num22 == 1)
									{
										rectangle.X = 162;
										rectangle.Y = 90;
									}
									if (num22 == 2)
									{
										rectangle.X = 180;
										rectangle.Y = 90;
									}
									mergeDown = true;
								}
								else
								{
									if (num2 == num9 && num7 == num9 && num4 == -2 && num5 == num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 162;
											rectangle.Y = 126;
										}
										if (num22 == 1)
										{
											rectangle.X = 162;
											rectangle.Y = 144;
										}
										if (num22 == 2)
										{
											rectangle.X = 162;
											rectangle.Y = 162;
										}
										mergeLeft = true;
									}
									else
									{
										if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == -2)
										{
											if (num22 == 0)
											{
												rectangle.X = 144;
												rectangle.Y = 126;
											}
											if (num22 == 1)
											{
												rectangle.X = 144;
												rectangle.Y = 144;
											}
											if (num22 == 2)
											{
												rectangle.X = 144;
												rectangle.Y = 162;
											}
											mergeRight = true;
										}
										else
										{
											if (num2 == -2 && num7 == num9 && num4 == -2 && num5 == num9)
											{
												if (num22 == 0)
												{
													rectangle.X = 36;
													rectangle.Y = 90;
												}
												if (num22 == 1)
												{
													rectangle.X = 36;
													rectangle.Y = 126;
												}
												if (num22 == 2)
												{
													rectangle.X = 36;
													rectangle.Y = 162;
												}
												mergeUp = true;
												mergeLeft = true;
											}
											else
											{
												if (num2 == -2 && num7 == num9 && num4 == num9 && num5 == -2)
												{
													if (num22 == 0)
													{
														rectangle.X = 54;
														rectangle.Y = 90;
													}
													if (num22 == 1)
													{
														rectangle.X = 54;
														rectangle.Y = 126;
													}
													if (num22 == 2)
													{
														rectangle.X = 54;
														rectangle.Y = 162;
													}
													mergeUp = true;
													mergeRight = true;
												}
												else
												{
													if (num2 == num9 && num7 == -2 && num4 == -2 && num5 == num9)
													{
														if (num22 == 0)
														{
															rectangle.X = 36;
															rectangle.Y = 108;
														}
														if (num22 == 1)
														{
															rectangle.X = 36;
															rectangle.Y = 144;
														}
														if (num22 == 2)
														{
															rectangle.X = 36;
															rectangle.Y = 180;
														}
														mergeDown = true;
														mergeLeft = true;
													}
													else
													{
														if (num2 == num9 && num7 == -2 && num4 == num9 && num5 == -2)
														{
															if (num22 == 0)
															{
																rectangle.X = 54;
																rectangle.Y = 108;
															}
															if (num22 == 1)
															{
																rectangle.X = 54;
																rectangle.Y = 144;
															}
															if (num22 == 2)
															{
																rectangle.X = 54;
																rectangle.Y = 180;
															}
															mergeDown = true;
															mergeRight = true;
														}
														else
														{
															if (num2 == num9 && num7 == num9 && num4 == -2 && num5 == -2)
															{
																if (num22 == 0)
																{
																	rectangle.X = 180;
																	rectangle.Y = 126;
																}
																if (num22 == 1)
																{
																	rectangle.X = 180;
																	rectangle.Y = 144;
																}
																if (num22 == 2)
																{
																	rectangle.X = 180;
																	rectangle.Y = 162;
																}
																mergeLeft = true;
																mergeRight = true;
															}
															else
															{
																if (num2 == -2 && num7 == -2 && num4 == num9 && num5 == num9)
																{
																	if (num22 == 0)
																	{
																		rectangle.X = 144;
																		rectangle.Y = 180;
																	}
																	if (num22 == 1)
																	{
																		rectangle.X = 162;
																		rectangle.Y = 180;
																	}
																	if (num22 == 2)
																	{
																		rectangle.X = 180;
																		rectangle.Y = 180;
																	}
																	mergeUp = true;
																	mergeDown = true;
																}
																else
																{
																	if (num2 == -2 && num7 == num9 && num4 == -2 && num5 == -2)
																	{
																		if (num22 == 0)
																		{
																			rectangle.X = 198;
																			rectangle.Y = 90;
																		}
																		if (num22 == 1)
																		{
																			rectangle.X = 198;
																			rectangle.Y = 108;
																		}
																		if (num22 == 2)
																		{
																			rectangle.X = 198;
																			rectangle.Y = 126;
																		}
																		mergeUp = true;
																		mergeLeft = true;
																		mergeRight = true;
																	}
																	else
																	{
																		if (num2 == num9 && num7 == -2 && num4 == -2 && num5 == -2)
																		{
																			if (num22 == 0)
																			{
																				rectangle.X = 198;
																				rectangle.Y = 144;
																			}
																			if (num22 == 1)
																			{
																				rectangle.X = 198;
																				rectangle.Y = 162;
																			}
																			if (num22 == 2)
																			{
																				rectangle.X = 198;
																				rectangle.Y = 180;
																			}
																			mergeDown = true;
																			mergeLeft = true;
																			mergeRight = true;
																		}
																		else
																		{
																			if (num2 == -2 && num7 == -2 && num4 == num9 && num5 == -2)
																			{
																				if (num22 == 0)
																				{
																					rectangle.X = 216;
																					rectangle.Y = 144;
																				}
																				if (num22 == 1)
																				{
																					rectangle.X = 216;
																					rectangle.Y = 162;
																				}
																				if (num22 == 2)
																				{
																					rectangle.X = 216;
																					rectangle.Y = 180;
																				}
																				mergeUp = true;
																				mergeDown = true;
																				mergeRight = true;
																			}
																			else
																			{
																				if (num2 == -2 && num7 == -2 && num4 == -2 && num5 == num9)
																				{
																					if (num22 == 0)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 90;
																					}
																					if (num22 == 1)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 108;
																					}
																					if (num22 == 2)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 126;
																					}
																					mergeUp = true;
																					mergeDown = true;
																					mergeLeft = true;
																				}
																				else
																				{
																					if (num2 == -2 && num7 == -2 && num4 == -2 && num5 == -2)
																					{
																						if (num22 == 0)
																						{
																							rectangle.X = 108;
																							rectangle.Y = 198;
																						}
																						if (num22 == 1)
																						{
																							rectangle.X = 126;
																							rectangle.Y = 198;
																						}
																						if (num22 == 2)
																						{
																							rectangle.X = 144;
																							rectangle.Y = 198;
																						}
																						mergeUp = true;
																						mergeDown = true;
																						mergeLeft = true;
																						mergeRight = true;
																					}
																					else
																					{
																						if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num9)
																						{
																							if (num == -2)
																							{
																								if (num22 == 0)
																								{
																									rectangle.X = 18;
																									rectangle.Y = 108;
																								}
																								if (num22 == 1)
																								{
																									rectangle.X = 18;
																									rectangle.Y = 144;
																								}
																								if (num22 == 2)
																								{
																									rectangle.X = 18;
																									rectangle.Y = 180;
																								}
																							}
																							if (num3 == -2)
																							{
																								if (num22 == 0)
																								{
																									rectangle.X = 0;
																									rectangle.Y = 108;
																								}
																								if (num22 == 1)
																								{
																									rectangle.X = 0;
																									rectangle.Y = 144;
																								}
																								if (num22 == 2)
																								{
																									rectangle.X = 0;
																									rectangle.Y = 180;
																								}
																							}
																							if (num6 == -2)
																							{
																								if (num22 == 0)
																								{
																									rectangle.X = 18;
																									rectangle.Y = 90;
																								}
																								if (num22 == 1)
																								{
																									rectangle.X = 18;
																									rectangle.Y = 126;
																								}
																								if (num22 == 2)
																								{
																									rectangle.X = 18;
																									rectangle.Y = 162;
																								}
																							}
																							if (num8 == -2)
																							{
																								if (num22 == 0)
																								{
																									rectangle.X = 0;
																									rectangle.Y = 90;
																								}
																								if (num22 == 1)
																								{
																									rectangle.X = 0;
																									rectangle.Y = 126;
																								}
																								if (num22 == 2)
																								{
																									rectangle.X = 0;
																									rectangle.Y = 162;
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
						else
						{
							if (num9 != 2 && num9 != 23 && num9 != 60 && num9 != 70)
							{
								if (num2 == -1 && num7 == -2 && num4 == num9 && num5 == num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 234;
										rectangle.Y = 0;
									}
									if (num22 == 1)
									{
										rectangle.X = 252;
										rectangle.Y = 0;
									}
									if (num22 == 2)
									{
										rectangle.X = 270;
										rectangle.Y = 0;
									}
									mergeDown = true;
								}
								else
								{
									if (num2 == -2 && num7 == -1 && num4 == num9 && num5 == num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 234;
											rectangle.Y = 18;
										}
										if (num22 == 1)
										{
											rectangle.X = 252;
											rectangle.Y = 18;
										}
										if (num22 == 2)
										{
											rectangle.X = 270;
											rectangle.Y = 18;
										}
										mergeUp = true;
									}
									else
									{
										if (num2 == num9 && num7 == num9 && num4 == -1 && num5 == -2)
										{
											if (num22 == 0)
											{
												rectangle.X = 234;
												rectangle.Y = 36;
											}
											if (num22 == 1)
											{
												rectangle.X = 252;
												rectangle.Y = 36;
											}
											if (num22 == 2)
											{
												rectangle.X = 270;
												rectangle.Y = 36;
											}
											mergeRight = true;
										}
										else
										{
											if (num2 == num9 && num7 == num9 && num4 == -2 && num5 == -1)
											{
												if (num22 == 0)
												{
													rectangle.X = 234;
													rectangle.Y = 54;
												}
												if (num22 == 1)
												{
													rectangle.X = 252;
													rectangle.Y = 54;
												}
												if (num22 == 2)
												{
													rectangle.X = 270;
													rectangle.Y = 54;
												}
												mergeLeft = true;
											}
										}
									}
								}
							}
							if (num2 != -1 && num7 != -1 && num4 == -1 && num5 == num9)
							{
								if (num2 == -2 && num7 == num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 72;
										rectangle.Y = 144;
									}
									if (num22 == 1)
									{
										rectangle.X = 72;
										rectangle.Y = 162;
									}
									if (num22 == 2)
									{
										rectangle.X = 72;
										rectangle.Y = 180;
									}
									mergeUp = true;
								}
								else
								{
									if (num7 == -2 && num2 == num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 72;
											rectangle.Y = 90;
										}
										if (num22 == 1)
										{
											rectangle.X = 72;
											rectangle.Y = 108;
										}
										if (num22 == 2)
										{
											rectangle.X = 72;
											rectangle.Y = 126;
										}
										mergeDown = true;
									}
								}
							}
							else
							{
								if (num2 != -1 && num7 != -1 && num4 == num9 && num5 == -1)
								{
									if (num2 == -2 && num7 == num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 90;
											rectangle.Y = 144;
										}
										if (num22 == 1)
										{
											rectangle.X = 90;
											rectangle.Y = 162;
										}
										if (num22 == 2)
										{
											rectangle.X = 90;
											rectangle.Y = 180;
										}
										mergeUp = true;
									}
									else
									{
										if (num7 == -2 && num2 == num9)
										{
											if (num22 == 0)
											{
												rectangle.X = 90;
												rectangle.Y = 90;
											}
											if (num22 == 1)
											{
												rectangle.X = 90;
												rectangle.Y = 108;
											}
											if (num22 == 2)
											{
												rectangle.X = 90;
												rectangle.Y = 126;
											}
											mergeDown = true;
										}
									}
								}
								else
								{
									if (num2 == -1 && num7 == num9 && num4 != -1 && num5 != -1)
									{
										if (num4 == -2 && num5 == num9)
										{
											if (num22 == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 198;
											}
											if (num22 == 1)
											{
												rectangle.X = 18;
												rectangle.Y = 198;
											}
											if (num22 == 2)
											{
												rectangle.X = 36;
												rectangle.Y = 198;
											}
											mergeLeft = true;
										}
										else
										{
											if (num5 == -2 && num4 == num9)
											{
												if (num22 == 0)
												{
													rectangle.X = 54;
													rectangle.Y = 198;
												}
												if (num22 == 1)
												{
													rectangle.X = 72;
													rectangle.Y = 198;
												}
												if (num22 == 2)
												{
													rectangle.X = 90;
													rectangle.Y = 198;
												}
												mergeRight = true;
											}
										}
									}
									else
									{
										if (num2 == num9 && num7 == -1 && num4 != -1 && num5 != -1)
										{
											if (num4 == -2 && num5 == num9)
											{
												if (num22 == 0)
												{
													rectangle.X = 0;
													rectangle.Y = 216;
												}
												if (num22 == 1)
												{
													rectangle.X = 18;
													rectangle.Y = 216;
												}
												if (num22 == 2)
												{
													rectangle.X = 36;
													rectangle.Y = 216;
												}
												mergeLeft = true;
											}
											else
											{
												if (num5 == -2 && num4 == num9)
												{
													if (num22 == 0)
													{
														rectangle.X = 54;
														rectangle.Y = 216;
													}
													if (num22 == 1)
													{
														rectangle.X = 72;
														rectangle.Y = 216;
													}
													if (num22 == 2)
													{
														rectangle.X = 90;
														rectangle.Y = 216;
													}
													mergeRight = true;
												}
											}
										}
										else
										{
											if (num2 != -1 && num7 != -1 && num4 == -1 && num5 == -1)
											{
												if (num2 == -2 && num7 == -2)
												{
													if (num22 == 0)
													{
														rectangle.X = 108;
														rectangle.Y = 216;
													}
													if (num22 == 1)
													{
														rectangle.X = 108;
														rectangle.Y = 234;
													}
													if (num22 == 2)
													{
														rectangle.X = 108;
														rectangle.Y = 252;
													}
													mergeUp = true;
													mergeDown = true;
												}
												else
												{
													if (num2 == -2)
													{
														if (num22 == 0)
														{
															rectangle.X = 126;
															rectangle.Y = 144;
														}
														if (num22 == 1)
														{
															rectangle.X = 126;
															rectangle.Y = 162;
														}
														if (num22 == 2)
														{
															rectangle.X = 126;
															rectangle.Y = 180;
														}
														mergeUp = true;
													}
													else
													{
														if (num7 == -2)
														{
															if (num22 == 0)
															{
																rectangle.X = 126;
																rectangle.Y = 90;
															}
															if (num22 == 1)
															{
																rectangle.X = 126;
																rectangle.Y = 108;
															}
															if (num22 == 2)
															{
																rectangle.X = 126;
																rectangle.Y = 126;
															}
															mergeDown = true;
														}
													}
												}
											}
											else
											{
												if (num2 == -1 && num7 == -1 && num4 != -1 && num5 != -1)
												{
													if (num4 == -2 && num5 == -2)
													{
														if (num22 == 0)
														{
															rectangle.X = 162;
															rectangle.Y = 198;
														}
														if (num22 == 1)
														{
															rectangle.X = 180;
															rectangle.Y = 198;
														}
														if (num22 == 2)
														{
															rectangle.X = 198;
															rectangle.Y = 198;
														}
														mergeLeft = true;
														mergeRight = true;
													}
													else
													{
														if (num4 == -2)
														{
															if (num22 == 0)
															{
																rectangle.X = 0;
																rectangle.Y = 252;
															}
															if (num22 == 1)
															{
																rectangle.X = 18;
																rectangle.Y = 252;
															}
															if (num22 == 2)
															{
																rectangle.X = 36;
																rectangle.Y = 252;
															}
															mergeLeft = true;
														}
														else
														{
															if (num5 == -2)
															{
																if (num22 == 0)
																{
																	rectangle.X = 54;
																	rectangle.Y = 252;
																}
																if (num22 == 1)
																{
																	rectangle.X = 72;
																	rectangle.Y = 252;
																}
																if (num22 == 2)
																{
																	rectangle.X = 90;
																	rectangle.Y = 252;
																}
																mergeRight = true;
															}
														}
													}
												}
												else
												{
													if (num2 == -2 && num7 == -1 && num4 == -1 && num5 == -1)
													{
														if (num22 == 0)
														{
															rectangle.X = 108;
															rectangle.Y = 144;
														}
														if (num22 == 1)
														{
															rectangle.X = 108;
															rectangle.Y = 162;
														}
														if (num22 == 2)
														{
															rectangle.X = 108;
															rectangle.Y = 180;
														}
														mergeUp = true;
													}
													else
													{
														if (num2 == -1 && num7 == -2 && num4 == -1 && num5 == -1)
														{
															if (num22 == 0)
															{
																rectangle.X = 108;
																rectangle.Y = 90;
															}
															if (num22 == 1)
															{
																rectangle.X = 108;
																rectangle.Y = 108;
															}
															if (num22 == 2)
															{
																rectangle.X = 108;
																rectangle.Y = 126;
															}
															mergeDown = true;
														}
														else
														{
															if (num2 == -1 && num7 == -1 && num4 == -2 && num5 == -1)
															{
																if (num22 == 0)
																{
																	rectangle.X = 0;
																	rectangle.Y = 234;
																}
																if (num22 == 1)
																{
																	rectangle.X = 18;
																	rectangle.Y = 234;
																}
																if (num22 == 2)
																{
																	rectangle.X = 36;
																	rectangle.Y = 234;
																}
																mergeLeft = true;
															}
															else
															{
																if (num2 == -1 && num7 == -1 && num4 == -1 && num5 == -2)
																{
																	if (num22 == 0)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 234;
																	}
																	if (num22 == 1)
																	{
																		rectangle.X = 72;
																		rectangle.Y = 234;
																	}
																	if (num22 == 2)
																	{
																		rectangle.X = 90;
																		rectangle.Y = 234;
																	}
																	mergeRight = true;
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
					if (rectangle.X < 0 || rectangle.Y < 0)
					{
						if (num9 == 2 || num9 == 23 || num9 == 60 || num9 == 70)
						{
							if (num2 == -2)
							{
								num2 = num9;
							}
							if (num7 == -2)
							{
								num7 = num9;
							}
							if (num4 == -2)
							{
								num4 = num9;
							}
							if (num5 == -2)
							{
								num5 = num9;
							}
							if (num == -2)
							{
								num = num9;
							}
							if (num3 == -2)
							{
								num3 = num9;
							}
							if (num6 == -2)
							{
								num6 = num9;
							}
							if (num8 == -2)
							{
								num8 = num9;
							}
						}
						if (num2 == num9 && num7 == num9 && (num4 == num9 & num5 == num9))
						{
							if (num != num9 && num3 != num9)
							{
								if (num22 == 0)
								{
									rectangle.X = 108;
									rectangle.Y = 18;
								}
								if (num22 == 1)
								{
									rectangle.X = 126;
									rectangle.Y = 18;
								}
								if (num22 == 2)
								{
									rectangle.X = 144;
									rectangle.Y = 18;
								}
							}
							else
							{
								if (num6 != num9 && num8 != num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 108;
										rectangle.Y = 36;
									}
									if (num22 == 1)
									{
										rectangle.X = 126;
										rectangle.Y = 36;
									}
									if (num22 == 2)
									{
										rectangle.X = 144;
										rectangle.Y = 36;
									}
								}
								else
								{
									if (num != num9 && num6 != num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 180;
											rectangle.Y = 0;
										}
										if (num22 == 1)
										{
											rectangle.X = 180;
											rectangle.Y = 18;
										}
										if (num22 == 2)
										{
											rectangle.X = 180;
											rectangle.Y = 36;
										}
									}
									else
									{
										if (num3 != num9 && num8 != num9)
										{
											if (num22 == 0)
											{
												rectangle.X = 198;
												rectangle.Y = 0;
											}
											if (num22 == 1)
											{
												rectangle.X = 198;
												rectangle.Y = 18;
											}
											if (num22 == 2)
											{
												rectangle.X = 198;
												rectangle.Y = 36;
											}
										}
										else
										{
											if (num22 == 0)
											{
												rectangle.X = 18;
												rectangle.Y = 18;
											}
											if (num22 == 1)
											{
												rectangle.X = 36;
												rectangle.Y = 18;
											}
											if (num22 == 2)
											{
												rectangle.X = 54;
												rectangle.Y = 18;
											}
										}
									}
								}
							}
						}
						else
						{
							if (num2 != num9 && num7 == num9 && (num4 == num9 & num5 == num9))
							{
								if (num22 == 0)
								{
									rectangle.X = 18;
									rectangle.Y = 0;
								}
								if (num22 == 1)
								{
									rectangle.X = 36;
									rectangle.Y = 0;
								}
								if (num22 == 2)
								{
									rectangle.X = 54;
									rectangle.Y = 0;
								}
							}
							else
							{
								if (num2 == num9 && num7 != num9 && (num4 == num9 & num5 == num9))
								{
									if (num22 == 0)
									{
										rectangle.X = 18;
										rectangle.Y = 36;
									}
									if (num22 == 1)
									{
										rectangle.X = 36;
										rectangle.Y = 36;
									}
									if (num22 == 2)
									{
										rectangle.X = 54;
										rectangle.Y = 36;
									}
								}
								else
								{
									if (num2 == num9 && num7 == num9 && (num4 != num9 & num5 == num9))
									{
										if (num22 == 0)
										{
											rectangle.X = 0;
											rectangle.Y = 0;
										}
										if (num22 == 1)
										{
											rectangle.X = 0;
											rectangle.Y = 18;
										}
										if (num22 == 2)
										{
											rectangle.X = 0;
											rectangle.Y = 36;
										}
									}
									else
									{
										if (num2 == num9 && num7 == num9 && (num4 == num9 & num5 != num9))
										{
											if (num22 == 0)
											{
												rectangle.X = 72;
												rectangle.Y = 0;
											}
											if (num22 == 1)
											{
												rectangle.X = 72;
												rectangle.Y = 18;
											}
											if (num22 == 2)
											{
												rectangle.X = 72;
												rectangle.Y = 36;
											}
										}
										else
										{
											if (num2 != num9 && num7 == num9 && (num4 != num9 & num5 == num9))
											{
												if (num22 == 0)
												{
													rectangle.X = 0;
													rectangle.Y = 54;
												}
												if (num22 == 1)
												{
													rectangle.X = 36;
													rectangle.Y = 54;
												}
												if (num22 == 2)
												{
													rectangle.X = 72;
													rectangle.Y = 54;
												}
											}
											else
											{
												if (num2 != num9 && num7 == num9 && (num4 == num9 & num5 != num9))
												{
													if (num22 == 0)
													{
														rectangle.X = 18;
														rectangle.Y = 54;
													}
													if (num22 == 1)
													{
														rectangle.X = 54;
														rectangle.Y = 54;
													}
													if (num22 == 2)
													{
														rectangle.X = 90;
														rectangle.Y = 54;
													}
												}
												else
												{
													if (num2 == num9 && num7 != num9 && (num4 != num9 & num5 == num9))
													{
														if (num22 == 0)
														{
															rectangle.X = 0;
															rectangle.Y = 72;
														}
														if (num22 == 1)
														{
															rectangle.X = 36;
															rectangle.Y = 72;
														}
														if (num22 == 2)
														{
															rectangle.X = 72;
															rectangle.Y = 72;
														}
													}
													else
													{
														if (num2 == num9 && num7 != num9 && (num4 == num9 & num5 != num9))
														{
															if (num22 == 0)
															{
																rectangle.X = 18;
																rectangle.Y = 72;
															}
															if (num22 == 1)
															{
																rectangle.X = 54;
																rectangle.Y = 72;
															}
															if (num22 == 2)
															{
																rectangle.X = 90;
																rectangle.Y = 72;
															}
														}
														else
														{
															if (num2 == num9 && num7 == num9 && (num4 != num9 & num5 != num9))
															{
																if (num22 == 0)
																{
																	rectangle.X = 90;
																	rectangle.Y = 0;
																}
																if (num22 == 1)
																{
																	rectangle.X = 90;
																	rectangle.Y = 18;
																}
																if (num22 == 2)
																{
																	rectangle.X = 90;
																	rectangle.Y = 36;
																}
															}
															else
															{
																if (num2 != num9 && num7 != num9 && (num4 == num9 & num5 == num9))
																{
																	if (num22 == 0)
																	{
																		rectangle.X = 108;
																		rectangle.Y = 72;
																	}
																	if (num22 == 1)
																	{
																		rectangle.X = 126;
																		rectangle.Y = 72;
																	}
																	if (num22 == 2)
																	{
																		rectangle.X = 144;
																		rectangle.Y = 72;
																	}
																}
																else
																{
																	if (num2 != num9 && num7 == num9 && (num4 != num9 & num5 != num9))
																	{
																		if (num22 == 0)
																		{
																			rectangle.X = 108;
																			rectangle.Y = 0;
																		}
																		if (num22 == 1)
																		{
																			rectangle.X = 126;
																			rectangle.Y = 0;
																		}
																		if (num22 == 2)
																		{
																			rectangle.X = 144;
																			rectangle.Y = 0;
																		}
																	}
																	else
																	{
																		if (num2 == num9 && num7 != num9 && (num4 != num9 & num5 != num9))
																		{
																			if (num22 == 0)
																			{
																				rectangle.X = 108;
																				rectangle.Y = 54;
																			}
																			if (num22 == 1)
																			{
																				rectangle.X = 126;
																				rectangle.Y = 54;
																			}
																			if (num22 == 2)
																			{
																				rectangle.X = 144;
																				rectangle.Y = 54;
																			}
																		}
																		else
																		{
																			if (num2 != num9 && num7 != num9 && (num4 != num9 & num5 == num9))
																			{
																				if (num22 == 0)
																				{
																					rectangle.X = 162;
																					rectangle.Y = 0;
																				}
																				if (num22 == 1)
																				{
																					rectangle.X = 162;
																					rectangle.Y = 18;
																				}
																				if (num22 == 2)
																				{
																					rectangle.X = 162;
																					rectangle.Y = 36;
																				}
																			}
																			else
																			{
																				if (num2 != num9 && num7 != num9 && (num4 == num9 & num5 != num9))
																				{
																					if (num22 == 0)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 0;
																					}
																					if (num22 == 1)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 18;
																					}
																					if (num22 == 2)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 36;
																					}
																				}
																				else
																				{
																					if (num2 != num9 && num7 != num9 && (num4 != num9 & num5 != num9))
																					{
																						if (num22 == 0)
																						{
																							rectangle.X = 162;
																							rectangle.Y = 54;
																						}
																						if (num22 == 1)
																						{
																							rectangle.X = 180;
																							rectangle.Y = 54;
																						}
																						if (num22 == 2)
																						{
																							rectangle.X = 198;
																							rectangle.Y = 54;
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
					if (rectangle.X <= -1 || rectangle.Y <= -1)
					{
						if (num22 <= 0)
						{
							rectangle.X = 18;
							rectangle.Y = 18;
						}
						if (num22 == 1)
						{
							rectangle.X = 36;
							rectangle.Y = 18;
						}
						if (num22 >= 2)
						{
							rectangle.X = 54;
							rectangle.Y = 18;
						}
					}
					TileAt(i, j).SetFrameX((short)rectangle.X);
					TileAt(i, j).SetFrameY((short)rectangle.Y);
					if (num9 == 52 || num9 == 62)
					{
						if (!TileAt(i, j - 1).Active)
						{
							num2 = -1;
						}
						else
						{
							num2 = (int)TileAt(i, j - 1).Type;
						}
						if (num2 != num9 && num2 != 2 && num2 != 60)
						{
							KillTile(i, j, false, false, false);
						}
					}
					if (!noTileActions && num9 == 53)
					{
						if (!TileAt(i, j + 1).Active)
						{
							bool flag4 = true;
							if (TileAt(i, j - 1).Active && TileAt(i, j - 1).Type == 21)
							{
								flag4 = false;
							}
							if (flag4)
							{
								int type2 = 31;
								if (num9 == 59)
								{
									type2 = 39;
								}
								if (num9 == 57)
								{
									type2 = 40;
								}
								TileAt(i, j).SetActive(false);
								sandbox.FallingBlockProjectile(i, j, type2);
								//                                    int num25 = Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0.41f, type2, 10, 0f, Main.myPlayer);
								//                                    Main.projectile[num25].Velocity.Y = 0.5f;
								//                                    Projectile expr_6501_cp_0 = Main.projectile[num25];
								//                                    expr_6501_cp_0.Position.Y = expr_6501_cp_0.Position.Y + 2f;
								//                                    Main.projectile[num25].ai[0] = 1f;
								SquareTileFrame(i, j, true);
							}
						}
					}
					if (rectangle.X != frameX && rectangle.Y != frameY && frameX >= 0 && frameY >= 0)
					{
						bool flag5 = mergeUp;
						bool flag6 = mergeDown;
						bool flag7 = mergeLeft;
						bool flag8 = mergeRight;
						TileFrame(i - 1, j, false, false);
						TileFrame(i + 1, j, false, false);
						TileFrame(i, j - 1, false, false);
						TileFrame(i, j + 1, false, false);
						mergeUp = flag5;
						mergeDown = flag6;
						mergeLeft = flag7;
						mergeRight = flag8;
					}
				}
			}
		}

		/*public bool EmptyTileCheck(int startX, int endX, int startY, int endY, int ignoreStyle = -1)
		{
			if (startX < 0)
			{
				return false;
			}
			if (endX >= Main.maxTilesX)
			{
				return false;
			}
			if (startY < 0)
			{
				return false;
			}
			if (endY >= Main.maxTilesY)
			{
				return false;
			}
			for (int i = startX; i < endX + 1; i++)
			{
				for (int j = startY; j < endY + 1; j++)
				{
					if (TileAt(i, j).Active)
					{
						if (ignoreStyle == -1)
						{
							return false;
						}
						
						int type = TileAt(i, j).Type;
						
						if (ignoreStyle == 11 && type != 11)
						{
							return false;
						}
						if (ignoreStyle == 20 && type != 20 && type != 3 && type != 24 && type != 61 && type != 32 && type != 69 && type != 73 && type != 74)
						{
							return false;
						}
						if (ignoreStyle == 71 && type != 71)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		
		public bool PlaceDoor(int i, int j, int type)
		{
			bool result;
			try
			{
				if (TileAt(i, j - 2).Active && Main.tileSolid[(int)TileAt(i, j - 2).Type] && TileAt(i, j + 2).Active && Main.tileSolid[(int)TileAt(i, j + 2).Type])
				{
					TileAt(i, j - 1).SetActive (true);
					TileAt(i, j - 1).SetType (10);
					TileAt(i, j - 1).SetFrameY (0);
					TileAt(i, j - 1).SetFrameX ((short)(genRand.Next(3) * 18));
					TileAt(i, j).SetActive (true);
					TileAt(i, j).SetType (10);
					TileAt(i, j).SetFrameY (18);
					TileAt(i, j).SetFrameX ((short)(genRand.Next(3) * 18));
					TileAt(i, j + 1).SetActive (true);
					TileAt(i, j + 1).SetType (10);
					TileAt(i, j + 1).SetFrameY (36);
					TileAt(i, j + 1).SetFrameX ((short)(genRand.Next(3) * 18));
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public bool CloseDoor(int x, int y, bool forced, ISender sender)
		{
			if (sender == null)
			{
				sender = new ConsoleSender ();
			}

			if (Program.properties.NPCDoorOpenCancel && sender is NPC)
				return false;

			int num = 0;
			int num2 = x;
			int num3 = y;
			
			int frameX = (int)TileAt(x, y).FrameX;
			int frameY = (int)TileAt(x, y).FrameY;
			if (frameX == 0)
			{
				num2 = x;
				num = 1;
			}
			else if (frameX == 18)
			{
				num2 = x - 1;
				num = 1;
			}
			else if (frameX == 36)
			{
				num2 = x + 1;
				num = -1;
			}
			else if (frameX == 54)
			{
				num2 = x;
				num = -1;
			}
			if (frameY == 0)
			{
				num3 = y;
			}
			else if (frameY == 18)
			{
				num3 = y - 1;
			}
			else if (frameY == 36)
			{
				num3 = y - 2;
			}
			int num4 = num2;
			if (num == -1)
			{
				num4 = num2 - 1;
			}
			if (!forced)
			{
				for (int k = num3; k < num3 + 3; k++)
				{
					if (!Collision.EmptyTile(num2, k, true))
					{
						return false;
					}
				}
			}
			for (int l = num4; l < num4 + 2; l++)
			{
				for (int m = num3; m < num3 + 3; m++)
				{
					if (l == num2)
					{
						TileAt(l, m).SetType (10);
						TileAt(l, m).SetFrameX ((short)(genRand.Next(3) * 18));
					}
					else
					{
						TileAt(l, m).SetActive (false);
					}
				}
			}
			for (int n = num2 - 1; n <= num2 + 1; n++)
			{
				for (int num5 = num3 - 1; num5 <= num3 + 2; num5++)
				{
					TileFrame(n, num5, false, false);
				}
			}
			return true;
		}

		public bool OpenDoor(int x, int y, int direction, ISender sender)
		{
			if (sender == null)
			{
				sender = new ConsoleSender();
			}

			if (Program.properties.NPCDoorOpenCancel && sender is NPC)
				return false;
			
			int num = 0;
			if (TileAt(x, y - 1).FrameY == 0 && TileAt(x, y - 1).Type == TileAt(x, y).Type)
			{
				num = y - 1;
			}
			else if (TileAt(x, y - 2).FrameY == 0 && TileAt(x, y - 2).Type == TileAt(x, y).Type)
			{
				num = y - 2;
			}
			else if (TileAt(x, y + 1).FrameY == 0 && TileAt(x, y + 1).Type == TileAt(x, y).Type)
			{
				num = y + 1;
			}
			else
			{
				num = y;
			}
			int num2 = x;
			short num3 = 0;
			int num4;
			if (direction == -1)
			{
				num2 = x - 1;
				num3 = 36;
				num4 = x - 1;
			}
			else
			{
				num2 = x;
				num4 = x + 1;
			}
			bool flag = true;
			for (int k = num; k < num + 3; k++)
			{
				if (TileAt(num4, k).Active)
				{
					if (TileAt(num4, k).Type != 3 && TileAt(num4, k).Type != 24 && TileAt(num4, k).Type != 52 && TileAt(num4, k).Type != 61 && TileAt(num4, k).Type != 62 && TileAt(num4, k).Type != 69 && TileAt(num4, k).Type != 71 && TileAt(num4, k).Type != 73 && TileAt(num4, k).Type != 74)
					{
						flag = false;
						break;
					}
					KillTile(num4, k, false, false, false);
				}
			}
			if (flag)
			{
				TileAt(num2, num).SetActive (true);
				TileAt(num2, num).SetType (11);
				TileAt(num2, num).SetFrameY (0);
				TileAt(num2, num).SetFrameX (num3);
				TileAt(num2 + 1, num).SetActive (true);
				TileAt(num2 + 1, num).SetType (11);
				TileAt(num2 + 1, num).SetFrameY (0);
				TileAt(num2 + 1, num).SetFrameX ((short)(num3 + 18));
				TileAt(num2, num + 1).SetActive (true);
				TileAt(num2, num + 1).SetType (11);
				TileAt(num2, num + 1).SetFrameY (18);
				TileAt(num2, num + 1).SetFrameX (num3);
				TileAt(num2 + 1, num + 1).SetActive (true);
				TileAt(num2 + 1, num + 1).SetType (11);
				TileAt(num2 + 1, num + 1).SetFrameY (18);
				TileAt(num2 + 1, num + 1).SetFrameX ((short)(num3 + 18));
				TileAt(num2, num + 2).SetActive (true);
				TileAt(num2, num + 2).SetType (11);
				TileAt(num2, num + 2).SetFrameY (36);
				TileAt(num2, num + 2).SetFrameX (num3);
				TileAt(num2 + 1, num + 2).SetActive (true);
				TileAt(num2 + 1, num + 2).SetType (11);
				TileAt(num2 + 1, num + 2).SetFrameY (36);
				TileAt(num2 + 1, num + 2).SetFrameX ((short)(num3 + 18));
				for (int l = num2 - 1; l <= num2 + 2; l++)
				{
					for (int m = num - 1; m <= num + 2; m++)
					{
						TileFrame(l, m, false, false);
					}
				}
			}
			return flag;
		}
		
		public void Check1x2(int x, int j, byte type)
		{
			if (destroyObject)
			{
				return;
			}
			int num = j;
			bool flag = true;

			int FrameY = (int)TileAt(x, num).FrameY;

			int num2 = 0;
			while (FrameY >= 40)
			{
				FrameY -= 40;
				num2++;
			}

			if (FrameY == 18)
			{
				num--;
			}
			if ((int)TileAt(x, num).FrameY == 40 * num2 && (int)TileAt(x, num + 1).FrameY == 40 * num2 + 18 && TileAt(x, num).Type == type && TileAt(x, num + 1).Type == type)
			{
				flag = false;
			}
			if (!TileAt(x, num + 2).Active || !Main.tileSolid[(int)TileAt(x, num + 2).Type])
			{
				flag = true;
			}
			if (TileAt(x, num + 2).Type != 2 && TileAt(x, num).Type == 20)
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				if (TileAt(x, num).Type == type)
				{
					KillTile(x, num, false, false, false);
				}
				if (TileAt(x, num + 1).Type == type)
				{
					KillTile(x, num + 1, false, false, false);
				}
				if (type == 15)
				{
					if (num2 == 1)
					{
						sandbox.NewItem(x * 16, num * 16, 32, 32, 358, 1, false);
					}
					else
					{
						sandbox.NewItem(x * 16, num * 16, 32, 32, 34, 1, false);
					}
				}
				destroyObject = false;
			}
		}
		
		public void CheckOnTable1x1(int x, int y, int type)
		{
			if ((!TileAt(x, y + 1).Active || !Main.tileTable[(int)TileAt(x, y + 1).Type]))
			{
				if (type == 78)
				{
					if (!TileAt(x, y + 1).Active || !Main.tileSolid[(int)TileAt(x, y + 1).Type])
					{
						KillTile(x, y, false, false, false);
						return;
					}
				}
				else
				{
					KillTile(x, y, false, false, false);
				}
			}
		}
		
		public void CheckSign(int x, int y, int type)
		{
			if (destroyObject)
			{
				return;
			}
			int num = x - 2;
			int num2 = x + 3;
			int num3 = y - 2;
			int num4 = y + 3;
			if (num < 0)
			{
				return;
			}
			if (num2 > Main.maxTilesX)
			{
				return;
			}
			if (num3 < 0)
			{
				return;
			}
			if (num4 > Main.maxTilesY)
			{
				return;
			}
			bool flag = false;
			int k = (int)(TileAt(x, y).FrameX / 18);
			int num5 = (int)(TileAt(x, y).FrameY / 18);
			while (k > 1)
			{
				k -= 2;
			}
			int num6 = x - k;
			int num7 = y - num5;
			int num8 = (int)(TileAt(num6, num7).FrameX / 18 / 2);
			num = num6;
			num2 = num6 + 2;
			num3 = num7;
			num4 = num7 + 2;
			k = 0;
			for (int l = num; l < num2; l++)
			{
				num5 = 0;
				for (int m = num3; m < num4; m++)
				{
					if (!TileAt(l, m).Active || (int)TileAt(l, m).Type != type)
					{
						flag = true;
						break;
					}
					if ((int)(TileAt(l, m).FrameX / 18) != k + num8 * 2 || (int)(TileAt(l, m).FrameY / 18) != num5)
					{
						flag = true;
						break;
					}
					num5++;
				}
				k++;
			}
			if (!flag)
			{
				if (type == 85)
				{
					if (TileAt(num6, num7 + 2).Active && Main.tileSolid[(int)TileAt(num6, num7 + 2).Type] && TileAt(num6 + 1, num7 + 2).Active && Main.tileSolid[(int)TileAt(num6 + 1, num7 + 2).Type])
					{
						num8 = 0;
					}
					else
					{
						flag = true;
					}
				}
				else if (TileAt(num6, num7 + 2).Active && Main.tileSolid[(int)TileAt(num6, num7 + 2).Type] && TileAt(num6 + 1, num7 + 2).Active && Main.tileSolid[(int)TileAt(num6 + 1, num7 + 2).Type])
				{
					num8 = 0;
				}
				else if (TileAt(num6, num7 - 1).Active && Main.tileSolid[(int)TileAt(num6, num7 - 1).Type] && !Main.tileSolidTop[(int)TileAt(num6, num7 - 1).Type] && TileAt(num6 + 1, num7 - 1).Active && Main.tileSolid[(int)TileAt(num6 + 1, num7 - 1).Type] && !Main.tileSolidTop[(int)TileAt(num6 + 1, num7 - 1).Type])
				{
					num8 = 1;
				}
				else if (TileAt(num6 - 1, num7).Active && Main.tileSolid[(int)TileAt(num6 - 1, num7).Type] && !Main.tileSolidTop[(int)TileAt(num6 - 1, num7).Type] && TileAt(num6 - 1, num7 + 1).Active && Main.tileSolid[(int)TileAt(num6 - 1, num7 + 1).Type] && !Main.tileSolidTop[(int)TileAt(num6 - 1, num7 + 1).Type])
				{
					num8 = 2;
				}
				else if (TileAt(num6 + 2, num7).Active && Main.tileSolid[(int)TileAt(num6 + 2, num7).Type] && !Main.tileSolidTop[(int)TileAt(num6 + 2, num7).Type] && TileAt(num6 + 2, num7 + 1).Active && Main.tileSolid[(int)TileAt(num6 + 2, num7 + 1).Type] && !Main.tileSolidTop[(int)TileAt(num6 + 2, num7 + 1).Type])
				{
					num8 = 3;
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int n = num; n < num2; n++)
				{
					for (int num9 = num3; num9 < num4; num9++)
					{
						if ((int)TileAt(n, num9).Type == type)
						{
							KillTile(n, num9, false, false, false);
						}
					}
				}
				sandbox.KillSign (num6, num7);
				if (type == 85)
				{
					sandbox.NewItem(x * 16, y * 16, 32, 32, 321, 1, false);
				}
				else
				{
					sandbox.NewItem(x * 16, y * 16, 32, 32, 171, 1, false);
				}
				destroyObject = false;
				return;
			}
			int num10 = 36 * num8;
			for (int num11 = 0; num11 < 2; num11++)
			{
				for (int num12 = 0; num12 < 2; num12++)
				{
					TileAt(num6 + num11, num7 + num12).SetActive (true);
					TileAt(num6 + num11, num7 + num12).SetType ((byte)type);
					TileAt(num6 + num11, num7 + num12).SetFrameX ((short)(num10 + 18 * num11));
					TileAt(num6 + num11, num7 + num12).SetFrameY ((short)(18 * num12));
				}
			}
		}
		
		public bool PlaceSign(int x, int y, int type)
		{
			int num = x - 2;
			int num2 = x + 3;
			int num3 = y - 2;
			int num4 = y + 3;
			if (num < 0)
			{
				return false;
			}
			if (num2 > Main.maxTilesX)
			{
				return false;
			}
			if (num3 < 0)
			{
				return false;
			}
			if (num4 > Main.maxTilesY)
			{
				return false;
			}
			int num5 = x;
			int num6 = y;
			int num7 = 0;
			if (type == 55)
			{
				if (TileAt(x, y + 1).Active && Main.tileSolid[(int)TileAt(x, y + 1).Type] && TileAt(x + 1, y + 1).Active && Main.tileSolid[(int)TileAt(x + 1, y + 1).Type])
				{
					num6--;
					num7 = 0;
				}
				else if (TileAt(x, y - 1).Active && Main.tileSolid[(int)TileAt(x, y - 1).Type] && !Main.tileSolidTop[(int)TileAt(x, y - 1).Type] && TileAt(x + 1, y - 1).Active && Main.tileSolid[(int)TileAt(x + 1, y - 1).Type] && !Main.tileSolidTop[(int)TileAt(x + 1, y - 1).Type])
				{
					num7 = 1;
				}
				else if (TileAt(x - 1, y).Active && Main.tileSolid[(int)TileAt(x - 1, y).Type] && !Main.tileSolidTop[(int)TileAt(x - 1, y).Type] && TileAt(x - 1, y + 1).Active && Main.tileSolid[(int)TileAt(x - 1, y + 1).Type] && !Main.tileSolidTop[(int)TileAt(x - 1, y + 1).Type])
				{
					num7 = 2;
				}
				else
				{
					if (!TileAt(x + 1, y).Active || !Main.tileSolid[(int)TileAt(x + 1, y).Type] || Main.tileSolidTop[(int)TileAt(x + 1, y).Type] || !TileAt(x + 1, y + 1).Active || !Main.tileSolid[(int)TileAt(x + 1, y + 1).Type] || Main.tileSolidTop[(int)TileAt(x + 1, y + 1).Type])
					{
						return false;
					}
					num5--;
					num7 = 3;
				}
			}
			else if (type == 85)
			{
				if (!TileAt(x, y + 1).Active || !Main.tileSolid[(int)TileAt(x, y + 1).Type] || !TileAt(x + 1, y + 1).Active || !Main.tileSolid[(int)TileAt(x + 1, y + 1).Type])
				{
					return false;
				}
				num6--;
				num7 = 0;
			}
			if (TileAt(num5, num6).Active || TileAt(num5 + 1, num6).Active || TileAt(num5, num6 + 1).Active || TileAt(num5 + 1, num6 + 1).Active)
			{
				return false;
			}
			int num8 = 36 * num7;
			for (int k = 0; k < 2; k++)
			{
				for (int l = 0; l < 2; l++)
				{
					TileAt(num5 + k, num6 + l).SetActive (true);
					TileAt(num5 + k, num6 + l).SetType ((byte)type);
					TileAt(num5 + k, num6 + l).SetFrameX ((short)(num8 + 18 * k));
					TileAt(num5 + k, num6 + l).SetFrameY ((short)(18 * l));
				}
			}
			return true;
		}
		
		public void PlaceOnTable1x1(int x, int y, int type, int style = 0)
		{
			bool flag = false;
			if (!TileAt(x, y).Active && TileAt(x, y + 1).Active && Main.tileTable[(int)TileAt(x, y + 1).Type])
			{
				flag = true;
			}
			if (type == 78 && !TileAt(x, y).Active && TileAt(x, y + 1).Active && Main.tileSolid[(int)TileAt(x, y + 1).Type])
			{
				flag = true;
			}
			if (flag)
			{
				TileAt(x, y).SetActive (true);
				TileAt(x, y).SetFrameX((short)(style * 18));
				TileAt(x, y).SetFrameY (0);
				TileAt(x, y).SetType ((byte)type);
				if (type == 50)
				{
					TileAt(x, y).SetFrameX ((short)(18 * genRand.Next(5)));
				}
			}
		}
		
		public bool PlaceAlch(int x, int y, int style)
		{
			if (!TileAt(x, y).Active && TileAt(x, y + 1).Active)
			{
				bool flag = false;
				if (style == 0)
				{
					if (TileAt(x, y + 1).Type != 2 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0)
					{
						flag = true;
					}
				}
				else if (style == 1)
				{
					if (TileAt(x, y + 1).Type != 60 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0)
					{
						flag = true;
					}
				}
				else if (style == 2)
				{
					if (TileAt(x, y + 1).Type != 0 && TileAt(x, y + 1).Type != 59 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0)
					{
						flag = true;
					}
				}
				else if (style == 3)
				{
					if (TileAt(x, y + 1).Type != 23 && TileAt(x, y + 1).Type != 25 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0)
					{
						flag = true;
					}
				}
				else if (style == 4)
				{
					if (TileAt(x, y + 1).Type != 53 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && TileAt(x, y).Lava)
					{
						flag = true;
					}
				}
				else if (style == 5)
				{
					if (TileAt(x, y + 1).Type != 57 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && !TileAt(x, y).Lava)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					TileAt(x, y).SetActive (true);
					TileAt(x, y).SetType (82);
					TileAt(x, y).SetFrameX ((short)(18 * style));
					TileAt(x, y).SetFrameY (0);
					return true;
				}
			}
			return false;
		}
		
		public void GrowAlch(int x, int y)
		{
			if (TileAt(x, y).Active)
			{
				if (TileAt(x, y).Type == 82 && genRand.Next(50) == 0)
				{
					TileAt(x, y).SetType (83);
					
					SquareTileFrame(x, y, true);
					return;
				}
				if (TileAt(x, y).FrameX == 36)
				{
					if (TileAt(x, y).Type == 83)
					{
						TileAt(x, y).SetType (84);
					}
					else
					{
						TileAt(x, y).SetType (83);
					}
				}
			}
		}
		
		public void PlantAlch()
		{
			int num = genRand.Next(20, Main.maxTilesX - 20);
			int num2 = 0;
			if (genRand.Next(40) == 0)
			{
				num2 = genRand.Next((int)(Main.rockLayer + (double)Main.maxTilesY) / 2, Main.maxTilesY - 20);
			}
			else if (genRand.Next(10) == 0)
			{
				num2 = genRand.Next(0, Main.maxTilesY - 20);
			}
			else
			{
				num2 = genRand.Next((int)Main.worldSurface, Main.maxTilesY - 20);
			}
			while (num2 < Main.maxTilesY - 20 && !TileAt(num, num2).Active)
			{
				num2++;
			}
			if (TileAt(num, num2).Active && !TileAt(num, num2 - 1).Active && TileAt(num, num2 - 1).Liquid == 0)
			{
				if (TileAt(num, num2).Type == 2)
				{
					PlaceAlch(num, num2 - 1, 0);
				}
				if (TileAt(num, num2).Type == 60)
				{
					PlaceAlch(num, num2 - 1, 1);
				}
				if (TileAt(num, num2).Type == 0 || TileAt(num, num2).Type == 59)
				{
					PlaceAlch(num, num2 - 1, 2);
				}
				if (TileAt(num, num2).Type == 23 || TileAt(num, num2).Type == 25)
				{
					PlaceAlch(num, num2 - 1, 3);
				}
				if (TileAt(num, num2).Type == 53)
				{
					PlaceAlch(num, num2 - 1, 4);
				}
				if (TileAt(num, num2).Type == 57)
				{
					PlaceAlch(num, num2 - 1, 5);
				}
			}
		}
		
		public void CheckAlch(int x, int y)
		{
			bool flag = false;
			if (!TileAt(x, y + 1).Active)
			{
				flag = true;
			}
			int num = (int)(TileAt(x, y).FrameX / 18);
			TileAt(x, y).SetFrameY (0);
			if (!flag)
			{
				if (num == 0)
				{
					if (TileAt(x, y + 1).Type != 2 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && TileAt(x, y).Lava)
					{
						flag = true;
					}
				}
				else if (num == 1)
				{
					if (TileAt(x, y + 1).Type != 60 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && TileAt(x, y).Lava)
					{
						flag = true;
					}
				}
				else if (num == 2)
				{
					if (TileAt(x, y + 1).Type != 0 && TileAt(x, y + 1).Type != 59 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && TileAt(x, y).Lava)
					{
						flag = true;
					}
				}
				else if (num == 3)
				{
					if (TileAt(x, y + 1).Type != 23 && TileAt(x, y + 1).Type != 25 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && TileAt(x, y).Lava)
					{
						flag = true;
					}
				}
				else if (num == 4)
				{
					if (TileAt(x, y + 1).Type != 53 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && TileAt(x, y).Lava)
					{
						flag = true;
					}
					if (TileAt(x, y).Type != 82 && !TileAt(x, y).Lava)
					{
						if (TileAt(x, y).Liquid > 16)
						{
							if (TileAt(x, y).Type == 83)
							{
								TileAt(x, y).SetType (84);
							}
						}
						else if (TileAt(x, y).Type == 84)
						{
							TileAt(x, y).SetType (83);
						}
					}
				}
				else if (num == 5)
				{
					if (TileAt(x, y + 1).Type != 57 && TileAt(x, y + 1).Type != 78)
					{
						flag = true;
					}
					if (TileAt(x, y).Liquid > 0 && !TileAt(x, y).Lava)
					{
						flag = true;
					}
					if (TileAt(x, y).Type != 82 && TileAt(x, y).Lava && TileAt(x, y).Type != 82 && TileAt(x, y).Lava)
					{
						if (TileAt(x, y).Liquid > 16)
						{
							if (TileAt(x, y).Type == 83)
							{
								TileAt(x, y).SetType (84);
							}
						}
						else if (TileAt(x, y).Type == 84)
						{
							TileAt(x, y).SetType (83);
						}
					}
				}
			}
			if (flag)
			{
				KillTile(x, y, false, false, false);
			}
		}

		public void Place1x2(int x, int y, int type, int style)
		{
			short frameX = 0;
			if (type == 20)
			{
				frameX = (short)(genRand.Next(3) * 18);
			}
			if (TileAt(x, y + 1).Active && Main.tileSolid[(int)TileAt(x, y + 1).Type] && !TileAt(x, y - 1).Active)
			{
				short frameHeight = (short)(style * 40);
				TileAt(x, y - 1).SetActive (true);
				TileAt(x, y - 1).SetFrameY(frameHeight);
				TileAt(x, y - 1).SetFrameX (frameX);
				TileAt(x, y - 1).SetType ((byte)type);
				TileAt(x, y).SetActive (true);
				TileAt(x, y).SetFrameY((short)(frameHeight + 18));
				TileAt(x, y).SetFrameX (frameX);
				TileAt(x, y).SetType ((byte)type);
			}
		}

		public void PlaceBanner(int x, int y, int type, int style = 0)
		{
			int FrameLength = style * 18;
			if (TileAt(x, y - 1).Active && Main.tileSolid[(int)TileAt(x, y - 1).Type] && !Main.tileSolidTop[(int)TileAt(x, y - 1).Type] && !TileAt(x, y).Active && !TileAt(x, y + 1).Active && !TileAt(x, y + 2).Active)
			{
				TileAt(x, y).SetActive(true);
				TileAt(x, y).SetFrameY(0);
				TileAt(x, y).SetFrameX((short)FrameLength);
				TileAt(x, y).SetType((byte)type);
				TileAt(x, y + 1).SetActive(true);
				TileAt(x, y + 1).SetFrameY(18);
				TileAt(x, y + 1).SetFrameX((short)FrameLength);
				TileAt(x, y + 1).SetType((byte)type);
				TileAt(x, y + 2).SetActive(true);
				TileAt(x, y + 2).SetFrameY(36);
				TileAt(x, y + 2).SetFrameX((short)FrameLength);
				TileAt(x, y + 2).SetType((byte)type);
			}
		}

		public void CheckBanner(int x, int j, byte type)
		{
			if (destroyObject)
			{
				return;
			}
			int num = j - (int)(TileAt(x, j).FrameY / 18);
			int frameX = (int)TileAt(x, j).FrameX;
			bool flag = false;
			for (int i = 0; i < 3; i++)
			{
				if (!TileAt(x, num + i).Active)
				{
					flag = true;
				}
				else
				{
					if (TileAt(x, num + i).Type != type)
					{
						flag = true;
					}
					else
					{
						if ((int)TileAt(x, num + i).FrameY != i * 18)
						{
							flag = true;
						}
						else
						{
							if ((int)TileAt(x, num + i).FrameX != frameX)
							{
								flag = true;
							}
						}
					}
				}
			}
			if (!TileAt(x, num - 1).Active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)TileAt(x, num - 1).Type])
			{
				flag = true;
			}
			if (Main.tileSolidTop[(int)TileAt(x, num - 1).Type])
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				for (int k = 0; k < 3; k++)
				{
					if (TileAt(x, num + k).Type == type)
					{
						KillTile(x, num + k, false, false, false);
					}
				}
				if (type == 91)
				{
					int num2 = frameX / 18;
					sandbox.NewItem(x * 16, (num + 1) * 16, 32, 32, 337 + num2, 1, false);
				}
				destroyObject = false;
			}
		}
	
		public void Place1x2Top(int x, int y, int type)
		{
			short frameX = 0;
			if (TileAt(x, y - 1).Active && Main.tileSolid[(int)TileAt(x, y - 1).Type] && !Main.tileSolidTop[(int)TileAt(x, y - 1).Type] && !TileAt(x, y + 1).Active)
			{
				TileAt(x, y).SetActive (true);
				TileAt(x, y).SetFrameY (0);
				TileAt(x, y).SetFrameX (frameX);
				TileAt(x, y).SetType ((byte)type);
				TileAt(x, y + 1).SetActive (true);
				TileAt(x, y + 1).SetFrameY (18);
				TileAt(x, y + 1).SetFrameX (frameX);
				TileAt(x, y + 1).SetType ((byte)type);
			}
		}

		private TRef GetTile(int x, int y)
		{
			return TileAt(x, y);
		}

		public void Check1x2Top(int x, int y, byte type)
		{
			if (destroyObject)
			{
				return;
			}
			
			bool flag = true;
			
			if (TileAt(x, y).FrameY == 18)
			{
				y--;
			}
			
			if (TileAt(x, y).FrameY == 0
				&& TileAt(x, y + 1).FrameY == 18
				&& TileAt(x, y).Type == type
				&& TileAt(x, y + 1).Type == type)
			{
				flag = false;
			}
			
			if (!TileAt(x, y - 1).Active
				|| !Main.tileSolid[(int)TileAt(x, y - 1).Type]
				|| Main.tileSolidTop[(int)TileAt(x, y - 1).Type])
			{
				flag = true;
			}
			
			if (flag)
			{
				destroyObject = true;
				if (TileAt(x, y).Type == type)
				{
					KillTile(x, y, false, false, false);
				}
				if (TileAt(x, y + 1).Type == type)
				{
					KillTile(x, y + 1, false, false, false);
				}
				if (type == 42)
				{
					sandbox.NewItem(x * 16, y * 16, 32, 32, 136, 1, false);
				}
				destroyObject = false;
			}
		}
		
		public void Check2x1(int x, int y, byte type)
		{
			if (destroyObject)
			{
				return;
			}
			
			bool flag = true;
			
			if (TileAt(x, y).FrameX == 18)
			{
				x--;
			}
			
			if (TileAt(x, y).FrameX == 0
				&& TileAt(x + 1, y).FrameX == 18
				&& TileAt(x, y).Type == type
				&& TileAt(x + 1, y).Type == type)
			{
				flag = false;
			}
			
			if (type == 29 || type == 103)
			{
				if (!TileAt(x, y + 1).Active || !Main.tileTable[(int)TileAt(x, y + 1).Type])
				{
					flag = true;
				}
				if (!TileAt(x + 1, y + 1).Active || !Main.tileTable[(int)TileAt(x + 1, y + 1).Type])
				{
					flag = true;
				}
			}
			else
			{
				if (!TileAt(x, y + 1).Active || !Main.tileSolid[(int)TileAt(x, y + 1).Type])
				{
					flag = true;
				}
				if (!TileAt(x + 1, y + 1).Active || !Main.tileSolid[(int)TileAt(x + 1, y + 1).Type])
				{
					flag = true;
				}
			}
			
			if (flag)
			{
				destroyObject = true;
				if (TileAt(x, y).Type == type)
				{
					KillTile(x, y, false, false, false);
				}
				if (TileAt(x + 1, y).Type == type)
				{
					KillTile(x + 1, y, false, false, false);
				}
				if (type == 16)
				{
					sandbox.NewItem(x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 35, 1, false);
				}
				if (type == 18)
				{
					sandbox.NewItem(x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 36, 1, false);
				}
				if (type == 29)
				{
					sandbox.NewItem(x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 87, 1, false);
				}
				if (type == 103)
				{
					sandbox.NewItem(x * TILE_OFFSET_3, y * TILE_OFFSET_3, 32, 32, 356, 1, false);
				}
				destroyObject = false;
				SquareTileFrame(x, y, true);
				SquareTileFrame(x + 1, y, true);
			}
		}
		
		public void Place2x1(int x, int y, int type)
		{
			bool flag = false;
			if (type != 29 && type != 103
				&& TileAt(x, y + 1).Active
				&& TileAt(x + 1, y + 1).Active
				&& Main.tileSolid[(int)TileAt(x, y + 1).Type]
				&& Main.tileSolid[(int)TileAt(x + 1, y + 1).Type]
				&& !TileAt(x, y).Active
				&& !TileAt(x + 1, y).Active)
			{
				flag = true;
			}
			else
			{
				if ((type == 29 || type == 103)
					&& TileAt(x, y + 1).Active
					&& TileAt(x + 1, y + 1).Active
					&& Main.tileTable[(int)TileAt(x, y + 1).Type]
					&& Main.tileTable[(int)TileAt(x + 1, y + 1).Type]
					&& !TileAt(x, y).Active
					&& !TileAt(x + 1, y).Active)
				{
					flag = true;
				}
			}
			
			if (flag)
			{
				TileAt(x, y).SetActive (true);
				TileAt(x, y).SetFrameY (0);
				TileAt(x, y).SetFrameX (0);
				TileAt(x, y).SetType ((byte)type);
				TileAt(x + 1, y).SetActive (true);
				TileAt(x + 1, y).SetFrameY (0);
				TileAt(x + 1, y).SetFrameX (18);
				TileAt(x + 1, y).SetType ((byte)type);
			}
		}
		
		public void Check4x2(int i, int j, int type)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i;
			num += (int)(TileAt(i, j).FrameX / 18 * -1);
			if ((type == 79 || type == 90) && TileAt(i, j).FrameX >= 72)
			{
				num += 4;
			}
			int num2 = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 4; k++)
			{
				for (int l = num2; l < num2 + 2; l++)
				{
					int num3 = (k - num) * 18;
					if ((type == 79 || type == 90) && TileAt(i, j).FrameX >= 72)
					{
						num3 = (k - num + 4) * 18;
					}
					if (!TileAt(k, l).Active || (int)TileAt(k, l).Type != type || (int)TileAt(k, l).FrameX != num3 || (int)TileAt(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (!TileAt(k, num2 + 2).Active || !Main.tileSolid[(int)TileAt(k, num2 + 2).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int m = num; m < num + 4; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)TileAt(m, n).Type == type && TileAt(m, n).Active)
						{
							KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 79)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 224, 1, false);
				}
				if (type == 90)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 336, 1, false);
				}
				destroyObject = false;
				for (int num4 = num - 1; num4 < num + 4; num4++)
				{
					for (int num5 = num2 - 1; num5 < num2 + 4; num5++)
					{
						TileFrame(num4, num5, false, false);
					}
				}
			}
		}
		
		public void Check2x2(int i, int j, int type)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i + (int)(TileAt(i, j).FrameX / 18 * -1);
			int num2 = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 2; k++)
			{
				for (int l = num2; l < num2 + 2; l++)
				{
					if (!TileAt(k, l).Active || (int)TileAt(k, l).Type != type || (int)TileAt(k, l).FrameX != (k - num) * 18 || (int)TileAt(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (type == 95)
				{
					if (!TileAt(k, num2 - 1).Active || 
						!Main.tileSolid[(int)TileAt(k, num2 - 1).Type] || 
						Main.tileSolidTop[(int)TileAt(k, num2 - 1).Type])
					{
						flag = true;
					}
				}
				else
				{
					if (!TileAt(k, num2 + 2).Active || 
						(!Main.tileSolid[(int)TileAt(k, num2 + 2).Type] && 
						!Main.tileTable[(int)TileAt(k, num2 + 2).Type]))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int m = num; m < num + 2; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)TileAt(m, n).Type == type && TileAt(m, n).Active)
						{
							KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 85)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 321, 1, false);
				}
				if (type == 94)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 352, 1, false);
				}
				if (type == 95)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 344, 1, false);
				}
				if (type == 96)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 345, 1, false);
				}
				if (type == 97)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 346, 1, false);
				}
				if (type == 98)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 347, 1, false);
				}
				if (type == 99)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 348, 1, false);
				}
				if (type == 100)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 349, 1, false);
				}
				destroyObject = false;
				for (int num3 = num - 1; num3 < num + 3; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 3; num4++)
					{
						TileFrame(num3, num4, false, false);
					}
				}
			}
		}
		
		public void Check3x2(int i, int j, int type)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i + (int)(TileAt(i, j).FrameX / 18 * -1);
			int num2 = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num2; l < num2 + 2; l++)
				{
					if (!TileAt(k, l).Active || (int)TileAt(k, l).Type != type || (int)TileAt(k, l).FrameX != (k - num) * 18 || (int)TileAt(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (!TileAt(k, num2 + 2).Active || !Main.tileSolid[(int)TileAt(k, num2 + 2).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)TileAt(m, n).Type == type && TileAt(m, n).Active)
						{
							KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 14)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 32, 1, false);
				}
				else if (type == 17)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 33, 1, false);
				}
				else if (type == 77)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 221, 1, false);
				}
				else if (type == 86)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 332, 1, false);
				}
				else if (type == 87)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 333, 1, false);
				}
				else if (type == 88)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 334, 1, false);
				}
				else if (type == 89)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 335, 1, false);
				}
				destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						TileFrame(num3, num4, false, false);
					}
				}
			}
		}
		
		public void Place4x2(int x, int y, int type, int direction = -1)
		{
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = x - 1; i < x + 3; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileAt(i, j).Active)
					{
						flag = false;
					}
				}
				if (!TileAt(i, y + 1).Active || !Main.tileSolid[(int)TileAt(i, y + 1).Type])
				{
					flag = false;
				}
			}
			short num = 0;
			if (direction == 1)
			{
				num = 72;
			}
			if (flag)
			{
				TileAt(x - 1, y - 1).SetActive (true);
				TileAt(x - 1, y - 1).SetFrameY (0);
				TileAt(x - 1, y - 1).SetFrameX (num);
				TileAt(x - 1, y - 1).SetType ((byte)type);
				TileAt(x, y - 1).SetActive (true);
				TileAt(x, y - 1).SetFrameY (0);
				TileAt(x, y - 1).SetFrameX ((short)(18 + num));
				TileAt(x, y - 1).SetType ((byte)type);
				TileAt(x + 1, y - 1).SetActive (true);
				TileAt(x + 1, y - 1).SetFrameY (0);
				TileAt(x + 1, y - 1).SetFrameX ((short)(36 + num));
				TileAt(x + 1, y - 1).SetType ((byte)type);
				TileAt(x + 2, y - 1).SetActive (true);
				TileAt(x + 2, y - 1).SetFrameY (0);
				TileAt(x + 2, y - 1).SetFrameX ((short)(54 + num));
				TileAt(x + 2, y - 1).SetType ((byte)type);
				TileAt(x - 1, y).SetActive (true);
				TileAt(x - 1, y).SetFrameY (18);
				TileAt(x - 1, y).SetFrameX (num);
				TileAt(x - 1, y).SetType ((byte)type);
				TileAt(x, y).SetActive (true);
				TileAt(x, y).SetFrameY (18);
				TileAt(x, y).SetFrameX ((short)(18 + num));
				TileAt(x, y).SetType ((byte)type);
				TileAt(x + 1, y).SetActive (true);
				TileAt(x + 1, y).SetFrameY (18);
				TileAt(x + 1, y).SetFrameX ((short)(36 + num));
				TileAt(x + 1, y).SetType ((byte)type);
				TileAt(x + 2, y).SetActive (true);
				TileAt(x + 2, y).SetFrameY (18);
				TileAt(x + 2, y).SetFrameX ((short)(54 + num));
				TileAt(x + 2, y).SetType ((byte)type);
			}
		}

		public void Place2x2(int x, int superY, int type)
		{
			int y = superY;
			if (type == 95)
			{
				y++;
			}
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = x - 1; i < x + 1; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileAt(i, j).Active)
					{
						flag = false;
					}
					if (type == 98 && TileAt(i, j).Liquid > 0)
					{
						flag = false;
					}
				}
				if (type == 95)
				{
					if (!TileAt(i, y - 2).Active || 
						!Main.tileSolid[(int)TileAt(i, y - 2).Type] || 
						Main.tileSolidTop[(int)TileAt(i, y - 2).Type])
					{
						flag = false;
					}
				}
				else
				{
					if (!TileAt(i, y + 1).Active || (
						!Main.tileSolid[(int)TileAt(i, y + 1).Type] && 
						!Main.tileTable[(int)TileAt(i, y + 1).Type]))
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				TileAt(x - 1, y - 1).SetActive (true);
				TileAt(x - 1, y - 1).SetFrameY (0);
				TileAt(x - 1, y - 1).SetFrameX (0);
				TileAt(x - 1, y - 1).SetType ((byte)type);
				TileAt(x, y - 1).SetActive (true);
				TileAt(x, y - 1).SetFrameY (0);
				TileAt(x, y - 1).SetFrameX (18);
				TileAt(x, y - 1).SetType ((byte)type);
				TileAt(x - 1, y).SetActive (true);
				TileAt(x - 1, y).SetFrameY (18);
				TileAt(x - 1, y).SetFrameX (0);
				TileAt(x - 1, y).SetType ((byte)type);
				TileAt(x, y).SetActive (true);
				TileAt(x, y).SetFrameY (18);
				TileAt(x, y).SetFrameX (18);
				TileAt(x, y).SetType ((byte)type);
			}
		}
		
		public void Place3x2(int x, int y, int type)
		{
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = x - 1; i < x + 2; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileAt(i, j).Active)
					{
						flag = false;
					}
				}
				if (!TileAt(i, y + 1).Active || !Main.tileSolid[(int)TileAt(i, y + 1).Type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				TileAt(x - 1, y - 1).SetActive (true);
				TileAt(x - 1, y - 1).SetFrameY (0);
				TileAt(x - 1, y - 1).SetFrameX (0);
				TileAt(x - 1, y - 1).SetType ((byte)type);
				TileAt(x, y - 1).SetActive (true);
				TileAt(x, y - 1).SetFrameY (0);
				TileAt(x, y - 1).SetFrameX (18);
				TileAt(x, y - 1).SetType ((byte)type);
				TileAt(x + 1, y - 1).SetActive (true);
				TileAt(x + 1, y - 1).SetFrameY (0);
				TileAt(x + 1, y - 1).SetFrameX (36);
				TileAt(x + 1, y - 1).SetType ((byte)type);
				TileAt(x - 1, y).SetActive (true);
				TileAt(x - 1, y).SetFrameY (18);
				TileAt(x - 1, y).SetFrameX (0);
				TileAt(x - 1, y).SetType ((byte)type);
				TileAt(x, y).SetActive (true);
				TileAt(x, y).SetFrameY (18);
				TileAt(x, y).SetFrameX (18);
				TileAt(x, y).SetType ((byte)type);
				TileAt(x + 1, y).SetActive (true);
				TileAt(x + 1, y).SetFrameY (18);
				TileAt(x + 1, y).SetFrameX (36);
				TileAt(x + 1, y).SetType ((byte)type);
			}
		}
		
		public void Check3x3(int i, int j, int type)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i + (int)(TileAt(i, j).FrameX / 18 * -1);
			int num2 = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num2; l < num2 + 3; l++)
				{
					if (!TileAt(k, l).Active || (int)TileAt(k, l).Type != type || (int)TileAt(k, l).FrameX != (k - num) * 18 || (int)TileAt(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
			}
			if (type == 106)
			{
				for (int m = num; m < num + 3; m++)
				{
					if (!TileAt(m, num2 + 3).Active || !Main.tileSolid[(int)TileAt(m, num2 + 3).Type])
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				if (!TileAt(num + 1, num2 - 1).Active || !Main.tileSolid[(int)TileAt(num + 1, num2 - 1).Type] || Main.tileSolidTop[(int)TileAt(num + 1, num2 - 1).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)TileAt(m, n).Type == type && TileAt(m, n).Active)
						{
							KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 34)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 106, 1, false);
				}
				else if (type == 35)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 107, 1, false);
				}
				else if (type == 36)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 108, 1, false);
				}
				else
				{
					if (type == 106)
					{
						sandbox.NewItem(i * 16, j * 16, 32, 32, 363, 1, false);
					}
				}
				destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						TileFrame(num3, num4, false, false);
					}
				}
			}
		}
		
		public void Place3x3(int x, int y, int type)
		{
			bool flag = true;
			int num = 0;
			if (type == 106)
			{
				num = -2;
				for (int i = x - 1; i < x + 2; i++)
				{
					for (int j = y - 2; j < y + 1; j++)
					{
						if (TileAt(i, j).Active)
						{
							flag = false;
						}
					}
				}
				for (int k = x - 1; k < x + 2; k++)
				{
					if (!TileAt(k, y + 1).Active || !Main.tileSolid[(int)TileAt(k, y + 1).Type])
					{
						flag = false;
						break;
					}
				}
			}
			else
			{
				for (int l = x - 1; l < x + 2; l++)
				{
					for (int m = y; m < y + 3; m++)
					{
						if (TileAt(l, m).Active)
						{
							flag = false;
						}
					}
				}
				if (!TileAt(x, y - 1).Active || !Main.tileSolid[(int)TileAt(x, y - 1).Type] || Main.tileSolidTop[(int)TileAt(x, y - 1).Type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				TileAt(x - 1, y + num).SetActive (true);
				TileAt(x - 1, y + num).SetFrameY(0);
				TileAt(x - 1, y + num).SetFrameX(0);
				TileAt(x - 1, y + num).SetType((byte)type);
				TileAt(x, y + num).SetActive(true);
				TileAt(x, y + num).SetFrameY(0);
				TileAt(x, y + num).SetFrameX(18);
				TileAt(x, y + num).SetType((byte)type);
				TileAt(x + 1, y + num).SetActive(true);
				TileAt(x + 1, y + num).SetFrameY(0);
				TileAt(x + 1, y + num).SetFrameX(36);
				TileAt(x + 1, y + num).SetType((byte)type);
				TileAt(x - 1, y + 1 + num).SetActive(true);
				TileAt(x - 1, y + 1 + num).SetFrameY(18);
				TileAt(x - 1, y + 1 + num).SetFrameX(0);
				TileAt(x - 1, y + 1 + num).SetType((byte)type);
				TileAt(x, y + 1 + num).SetActive(true);
				TileAt(x, y + 1 + num).SetFrameY(18);
				TileAt(x, y + 1 + num).SetFrameX(18);
				TileAt(x, y + 1 + num).SetType((byte)type);
				TileAt(x + 1, y + 1 + num).SetActive(true);
				TileAt(x + 1, y + 1 + num).SetFrameY(18);
				TileAt(x + 1, y + 1 + num).SetFrameX(36);
				TileAt(x + 1, y + 1 + num).SetType((byte)type);
				TileAt(x - 1, y + 2 + num).SetActive(true);
				TileAt(x - 1, y + 2 + num).SetFrameY(36);
				TileAt(x - 1, y + 2 + num).SetFrameX(0);
				TileAt(x - 1, y + 2 + num).SetType((byte)type);
				TileAt(x, y + 2 + num).SetActive(true);
				TileAt(x, y + 2 + num).SetFrameY(36);
				TileAt(x, y + 2 + num).SetFrameX(18);
				TileAt(x, y + 2 + num).SetType((byte)type);
				TileAt(x + 1, y + 2 + num).SetActive(true);
				TileAt(x + 1, y + 2 + num).SetFrameY(36);
				TileAt(x + 1, y + 2 + num).SetFrameX(36);
				TileAt(x + 1, y + 2 + num).SetType((byte)type);
			}
		}

		public void Check3x4(int i, int j, int type)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i + (int)(TileAt(i, j).FrameX / 18 * -1);
			int num2 = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num2; l < num2 + 4; l++)
				{
					if (!TileAt(k, l).Active || (int)TileAt(k, l).Type != type || 
						(int)TileAt(k, l).FrameX != (k - num) * 18 || (int)TileAt(k, l).FrameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (!TileAt(k, num2 + 4).Active || !Main.tileSolid[(int)TileAt(k, num2 + 4).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 4; n++)
					{
						if ((int)TileAt(m, n).Type == type && TileAt(m, n).Active)
						{
							KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 101)
				{
					sandbox.NewItem(i * 16, j * 16, 32, 32, 354, 1, false);
				}
				else
				{
					if (type == 102)
					{
						sandbox.NewItem(i * 16, j * 16, 32, 32, 355, 1, false);
					}
				}
				destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						TileFrame(num3, num4, false, false);
					}
				}
			}
		}

		public void Check1xX(int x, int j, byte type)
		{
			if (destroyObject)
			{
				return;
			}
			int num = j - (int)(TileAt(x, j).FrameY / 18);
			int frameX = (int)TileAt(x, j).FrameX;
			int num2 = 3;
			if (type == 92)
			{
				num2 = 6;
			}
			bool flag = false;
			for (int i = 0; i < num2; i++)
			{
				if (!TileAt(x, num + i).Active)
				{
					flag = true;
				}
				else
				{
					if (TileAt(x, num + i).Type != type)
					{
						flag = true;
					}
					else
					{
						if ((int)TileAt(x, num + i).FrameY != i * 18)
						{
							flag = true;
						}
						else
						{
							if ((int)TileAt(x, num + i).FrameX != frameX)
							{
								flag = true;
							}
						}
					}
				}
			}
			if (!TileAt(x, num + num2).Active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)TileAt(x, num + num2).Type])
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				for (int k = 0; k < num2; k++)
				{
					if (TileAt(x, num + k).Type == type)
					{
						KillTile(x, num + k, false, false, false);
					}
				}
				if (type == 92)
				{
					sandbox.NewItem(x * 16, j * 16, 32, 32, 341, 1, false);
				}
				if (type == 93)
				{
					sandbox.NewItem(x * 16, j * 16, 32, 32, 342, 1, false);
				}
				destroyObject = false;
			}
		}

		public void Check2xX(int i, int j, byte type)
		{
			if (destroyObject)
			{
				return;
			}
			int num = i;
			if (TileAt(i, j).FrameX == 18)
			{
				num--;
			}
			int num2 = j - (int)(TileAt(num, j).FrameY / 18);
			int frameX = (int)TileAt(num, num2).FrameX;
			int num3 = 3;
			if (type == 104)
			{
				num3 = 5;
			}
			bool flag = false;
			for (int k = 0; k < num3; k++)
			{
				if (!TileAt(num, num2 + k).Active)
				{
					flag = true;
				}
				else
				{
					if (TileAt(num, num2 + k).Type != type)
					{
						flag = true;
					}
					else
					{
						if ((int)TileAt(num, num2 + k).FrameY != k * 18)
						{
							flag = true;
						}
						else
						{
							if ((int)TileAt(num, num2 + k).FrameX != frameX)
							{
								flag = true;
							}
						}
					}
				}
				if (!TileAt(num + 1, num2 + k).Active)
				{
					flag = true;
				}
				else
				{
					if (TileAt(num + 1, num2 + k).Type != type)
					{
						flag = true;
					}
					else
					{
						if ((int)TileAt(num + 1, num2 + k).FrameY != k * 18)
						{
							flag = true;
						}
						else
						{
							if ((int)TileAt(num + 1, num2 + k).FrameX != frameX + 18)
							{
								flag = true;
							}
						}
					}
				}
			}
			if (!TileAt(num, num2 + num3).Active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)TileAt(num, num2 + num3).Type])
			{
				flag = true;
			}
			if (!TileAt(num + 1, num2 + num3).Active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)TileAt(num + 1, num2 + num3).Type])
			{
				flag = true;
			}
			if (flag)
			{
				destroyObject = true;
				for (int l = 0; l < num3; l++)
				{
					if (TileAt(num, num2 + l).Type == type)
					{
						KillTile(num, num2 + l, false, false, false);
					}
					if (TileAt(num + 1, num2 + l).Type == type)
					{
						KillTile(num + 1, num2 + l, false, false, false);
					}
				}
				if (type == 104)
				{
					sandbox.NewItem(num * 16, j * 16, 32, 32, 359, 1, false);
				}
				if (type == 105)
				{
					sandbox.NewItem(num * 16, j * 16, 32, 32, 360, 1, false);
				}
				destroyObject = false;
			}
		}

		public void PlaceSunflower(int x, int y, int type = 27)
		{
			if ((double)y > Main.worldSurface - 1.0)
			{
				return;
			}
			bool flag = true;
			for (int i = x; i < x + 2; i++)
			{
				for (int j = y - 3; j < y + 1; j++)
				{
					if (TileAt(i, j).Active || TileAt(i, j).Wall > 0)
					{
						flag = false;
					}
				}
				if (!TileAt(i, y + 1).Active || TileAt(i, y + 1).Type != 2)
				{
					flag = false;
				}
			}
			if (flag)
			{
				for (int k = 0; k < 2; k++)
				{
					for (int l = -3; l < 1; l++)
					{
						int num = k * 18 + genRand.Next(3) * 36;
						int num2 = (l + 3) * 18;
						TileAt(x + k, y + l).SetActive (true);
						TileAt(x + k, y + l).SetFrameX ((short)num);
						TileAt(x + k, y + l).SetFrameY ((short)num2);
						TileAt(x + k, y + l).SetType ((byte)type);
					}
				}
			}
		}
		
		public void CheckSunflower(int i, int j, int type = 27)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int k = 0;
			k += (int)(TileAt(i, j).FrameX / 18);
			int num = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			while (k > 1)
			{
				k -= 2;
			}
			k *= -1;
			k += i;
			for (int l = k; l < k + 2; l++)
			{
				for (int m = num; m < num + 4; m++)
				{
					int n;
					for (n = (int)(TileAt(l, m).FrameX / 18); n > 1; n -= 2)
					{
					}
					if (!TileAt(l, m).Active || (int)TileAt(l, m).Type != type || n != l - k || (int)TileAt(l, m).FrameY != (m - num) * 18)
					{
						flag = true;
					}
				}
				if (!TileAt(l, num + 4).Active || TileAt(l, num + 4).Type != 2)
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 4; num3++)
					{
						if ((int)TileAt(num2, num3).Type == type && TileAt(num2, num3).Active)
						{
							KillTile(num2, num3, false, false, false);
						}
					}
				}
				sandbox.NewItem(i * 16, j * 16, 32, 32, 63, 1, false);
				destroyObject = false;
			}
		}
		
		public bool PlacePot(int x, int y, int type = 28)
		{
			bool flag = true;
			for (int i = x; i < x + 2; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (TileAt(i, j).Active)
					{
						flag = false;
					}
				}
				if (!TileAt(i, y + 1).Active || !Main.tileSolid[(int)TileAt(i, y + 1).Type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				for (int k = 0; k < 2; k++)
				{
					for (int l = -1; l < 1; l++)
					{
						int num = k * 18 + genRand.Next(3) * 36;
						int num2 = (l + 1) * 18;
						TileAt(x + k, y + l).SetActive (true);
						TileAt(x + k, y + l).SetFrameX ((short)num);
						TileAt(x + k, y + l).SetFrameY ((short)num2);
						TileAt(x + k, y + l).SetType ((byte)type);
					}
				}
				return true;
			}
			return false;
		}
		
		public bool CheckCactus(int i, int j)
		{
			int num = j;
			int num2 = i;
			while (TileAt(num2, num).Active && TileAt(num2, num).Type == 80)
			{
				num++;
				if (!TileAt(num2, num).Active || TileAt(num2, num).Type != 80)
				{
					if (TileAt(num2 - 1, num).Active && TileAt(num2 - 1, num).Type == 80 && TileAt(num2 - 1, num - 1).Active && TileAt(num2 - 1, num - 1).Type == 80 && num2 >= i)
					{
						num2--;
					}
					if (TileAt(num2 + 1, num).Active && TileAt(num2 + 1, num).Type == 80 && TileAt(num2 + 1, num - 1).Active && TileAt(num2 + 1, num - 1).Type == 80 && num2 <= i)
					{
						num2++;
					}
				}
			}
			if (!TileAt(num2, num).Active || TileAt(num2, num).Type != 53)
			{
				KillTile(i, j, false, false, false);
				return true;
			}
			if (i != num2)
			{
				if ((!TileAt(i, j + 1).Active || TileAt(i, j + 1).Type != 80) && (!TileAt(i - 1, j).Active || TileAt(i - 1, j).Type != 80) && (!TileAt(i + 1, j).Active || TileAt(i + 1, j).Type != 80))
				{
					KillTile(i, j, false, false, false);
					return true;
				}
			}
			else if (i == num2 && (!TileAt(i, j + 1).Active || (TileAt(i, j + 1).Type != 80 && TileAt(i, j + 1).Type != 53)))
			{
				KillTile(i, j, false, false, false);
				return true;
			}
			return false;
		}
		
		public void PlantCactus(int i, int j)
		{
			GrowCactus(i, j);
			for (int k = 0; k < 150; k++)
			{
				int i2 = genRand.Next(i - 1, i + 2);
				int j2 = genRand.Next(j - 10, j + 2);
				GrowCactus(i2, j2);
			}
		}
		
		public void CactusFrame(int i, int j)
		{
			try
			{
				int num = j;
				int num2 = i;
				if (!CheckCactus(i, j))
				{
					while (TileAt(num2, num).Active && TileAt(num2, num).Type == 80)
					{
						num++;
						if (!TileAt(num2, num).Active || TileAt(num2, num).Type != 80)
						{
							if (TileAt(num2 - 1, num).Active && TileAt(num2 - 1, num).Type == 80 && TileAt(num2 - 1, num - 1).Active && TileAt(num2 - 1, num - 1).Type == 80 && num2 >= i)
							{
								num2--;
							}
							if (TileAt(num2 + 1, num).Active && TileAt(num2 + 1, num).Type == 80 && TileAt(num2 + 1, num - 1).Active && TileAt(num2 + 1, num - 1).Type == 80 && num2 <= i)
							{
								num2++;
							}
						}
					}
					num--;
					int num3 = i - num2;
					num2 = i;
					num = j;
					int type = (int)TileAt(i - 2, j).Type;
					int num4 = (int)TileAt(i - 1, j).Type;
					int num5 = (int)TileAt(i + 1, j).Type;
					int num6 = (int)TileAt(i, j - 1).Type;
					int num7 = (int)TileAt(i, j + 1).Type;
					int num8 = (int)TileAt(i - 1, j + 1).Type;
					int num9 = (int)TileAt(i + 1, j + 1).Type;
					if (!TileAt(i - 1, j).Active)
					{
						num4 = -1;
					}
					if (!TileAt(i + 1, j).Active)
					{
						num5 = -1;
					}
					if (!TileAt(i, j - 1).Active)
					{
						num6 = -1;
					}
					if (!TileAt(i, j + 1).Active)
					{
						num7 = -1;
					}
					if (!TileAt(i - 1, j + 1).Active)
					{
						num8 = -1;
					}
					if (!TileAt(i + 1, j + 1).Active)
					{
						num9 = -1;
					}
					short num10 = TileAt(i, j).FrameX;
					short num11 = TileAt(i, j).FrameY;
					if (num3 == 0)
					{
						if (num6 != 80)
						{
							if (num4 == 80 && num5 == 80 && num8 != 80 && num9 != 80 && type != 80)
							{
								num10 = 90;
								num11 = 0;
							}
							else if (num4 == 80 && num8 != 80 && type != 80)
							{
								num10 = 72;
								num11 = 0;
							}
							else if (num5 == 80 && num9 != 80)
							{
								num10 = 18;
								num11 = 0;
							}
							else
							{
								num10 = 0;
								num11 = 0;
							}
						}
						else if (num4 == 80 && num5 == 80 && num8 != 80 && num9 != 80 && type != 80)
						{
							num10 = 90;
							num11 = 36;
						}
						else if (num4 == 80 && num8 != 80 && type != 80)
						{
							num10 = 72;
							num11 = 36;
						}
						else if (num5 == 80 && num9 != 80)
						{
							num10 = 18;
							num11 = 36;
						}
						else if (num7 >= 0 && Main.tileSolid[num7])
						{
							num10 = 0;
							num11 = 36;
						}
						else
						{
							num10 = 0;
							num11 = 18;
						}
					}
					else if (num3 == -1)
					{
						if (num5 == 80)
						{
							if (num6 != 80 && num7 != 80)
							{
								num10 = 108;
								num11 = 36;
							}
							else if (num7 != 80)
							{
								num10 = 54;
								num11 = 36;
							}
							else if (num6 != 80)
							{
								num10 = 54;
								num11 = 0;
							}
							else
							{
								num10 = 54;
								num11 = 18;
							}
						}
						else if (num6 != 80)
						{
							num10 = 54;
							num11 = 0;
						}
						else
						{
							num10 = 54;
							num11 = 18;
						}
					}
					else if (num3 == 1)
					{
						if (num4 == 80)
						{
							if (num6 != 80 && num7 != 80)
							{
								num10 = 108;
								num11 = 16;
							}
							else if (num7 != 80)
							{
								num10 = 36;
								num11 = 36;
							}
							else if (num6 != 80)
							{
								num10 = 36;
								num11 = 0;
							}
							else
							{
								num10 = 36;
								num11 = 18;
							}
						}
						else if (num6 != 80)
						{
							num10 = 36;
							num11 = 0;
						}
						else
						{
							num10 = 36;
							num11 = 18;
						}
					}
					if (num10 != TileAt(i, j).FrameX || num11 != TileAt(i, j).FrameY)
					{
						TileAt(i, j).SetFrameX (num10);
						TileAt(i, j).SetFrameY (num11);
						SquareTileFrame(i, j, true);
					}
				}
			}
			catch
			{
				TileAt(i, j).SetFrameX (0);
				TileAt(i, j).SetFrameY (0);
			}
		}
		
		public void GrowCactus(int i, int j)
		{
			int num = j;
			int num2 = i;
			if (!TileAt(i, j).Active)
			{
				return;
			}
			if (TileAt(i, j - 1).Liquid > 0)
			{
				return;
			}
			if (TileAt(i, j).Type != 53 && TileAt(i, j).Type != 80)
			{
				return;
			}
			if (TileAt(i, j).Type == 53)
			{
				if (TileAt(i, j - 1).Active || TileAt(i - 1, j - 1).Active || TileAt(i + 1, j - 1).Active)
				{
					return;
				}
				int num3 = 0;
				int num4 = 0;
				for (int k = i - 6; k <= i + 6; k++)
				{
					for (int l = j - 3; l <= j + 1; l++)
					{
						try
						{
							if (TileAt(k, l).Active)
							{
								if (TileAt(k, l).Type == 80)
								{
									num3++;
									if (num3 >= 4)
									{
										return;
									}
								}
								if (TileAt(k, l).Type == 53)
								{
									num4++;
								}
							}
						}
						catch
						{
						}
					}
				}
				if (num4 > 10)
				{
					TileAt(i, j - 1).SetActive (true);
					TileAt(i, j - 1).SetType (80);
					
					SquareTileFrame(num2, num - 1, true);
					return;
				}
				return;
			}
			else
			{
				if (TileAt(i, j).Type != 80)
				{
					return;
				}
				while (TileAt(num2, num).Active && TileAt(num2, num).Type == 80)
				{
					num++;
					if (!TileAt(num2, num).Active || TileAt(num2, num).Type != 80)
					{
						if (TileAt(num2 - 1, num).Active && TileAt(num2 - 1, num).Type == 80 && TileAt(num2 - 1, num - 1).Active && TileAt(num2 - 1, num - 1).Type == 80 && num2 >= i)
						{
							num2--;
						}
						if (TileAt(num2 + 1, num).Active && TileAt(num2 + 1, num).Type == 80 && TileAt(num2 + 1, num - 1).Active && TileAt(num2 + 1, num - 1).Type == 80 && num2 <= i)
						{
							num2++;
						}
					}
				}
				num--;
				int num5 = num - j;
				int num6 = i - num2;
				num2 = i - num6;
				num = j;
				int num7 = 11 - num5;
				int num8 = 0;
				for (int m = num2 - 2; m <= num2 + 2; m++)
				{
					for (int n = num - num7; n <= num + num5; n++)
					{
						if (TileAt(m, n).Active && TileAt(m, n).Type == 80)
						{
							num8++;
						}
					}
				}
				if (num8 < genRand.Next(11, 13))
				{
					num2 = i;
					num = j;
					if (num6 == 0)
					{
						if (num5 == 0)
						{
							if (TileAt(num2, num - 1).Active)
							{
								return;
							}
							TileAt(num2, num - 1).SetActive (true);
							TileAt(num2, num - 1).SetType (80);
							SquareTileFrame(num2, num - 1, true);
							return;
						}
						else
						{
							bool flag = false;
							bool flag2 = false;
							if (TileAt(num2, num - 1).Active && TileAt(num2, num - 1).Type == 80)
							{
								if (!TileAt(num2 - 1, num).Active && !TileAt(num2 - 2, num + 1).Active && !TileAt(num2 - 1, num - 1).Active && !TileAt(num2 - 1, num + 1).Active && !TileAt(num2 - 2, num).Active)
								{
									flag = true;
								}
								if (!TileAt(num2 + 1, num).Active && !TileAt(num2 + 2, num + 1).Active && !TileAt(num2 + 1, num - 1).Active && !TileAt(num2 + 1, num + 1).Active && !TileAt(num2 + 2, num).Active)
								{
									flag2 = true;
								}
							}
							int num9 = genRand.Next(3);
							if (num9 == 0 && flag)
							{
								TileAt(num2 - 1, num).SetActive (true);
								TileAt(num2 - 1, num).SetType (80);
								SquareTileFrame(num2 - 1, num, true);
								return;
							}
							else if (num9 == 1 && flag2)
							{
								TileAt(num2 + 1, num).SetActive (true);
								TileAt(num2 + 1, num).SetType (80);
								SquareTileFrame(num2 + 1, num, true);
								return;
							}
							else
							{
								if (num5 >= genRand.Next(2, 8))
								{
									return;
								}
								//if (TileAt(num2 - 1, num - 1).Active)
								//{
								//    byte arg_5E0_0 = TileAt(num2 - 1, num - 1).Type;
								//}
								if (TileAt(num2 + 1, num - 1).Active && TileAt(num2 + 1, num - 1).Type == 80)
								{
									return;
								}
								TileAt(num2, num - 1).SetActive (true);
								TileAt(num2, num - 1).SetType (80);
								SquareTileFrame(num2, num - 1, true);
								return;
							}
						}
					}
					else
					{
						if (TileAt(num2, num - 1).Active || TileAt(num2, num - 2).Active || TileAt(num2 + num6, num - 1).Active || !TileAt(num2 - num6, num - 1).Active || TileAt(num2 - num6, num - 1).Type != 80)
						{
							return;
						}
						TileAt(num2, num - 1).SetActive (true);
						TileAt(num2, num - 1).SetType (80);
						SquareTileFrame(num2, num - 1, true);
						return;
					}
				}
			}
		}
		
		public void CheckPot(int i, int j, int type = 28)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int k = 0;
			k += (int)(TileAt(i, j).FrameX / 18);
			int num = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			while (k > 1)
			{
				k -= 2;
			}
			k *= -1;
			k += i;
			for (int l = k; l < k + 2; l++)
			{
				for (int m = num; m < num + 2; m++)
				{
					int n;
					for (n = (int)(TileAt(l, m).FrameX / 18); n > 1; n -= 2)
					{
					}
					if (!TileAt(l, m).Active || (int)TileAt(l, m).Type != type || n != l - k || (int)TileAt(l, m).FrameY != (m - num) * 18)
					{
						flag = true;
					}
				}
				if (!TileAt(l, num + 2).Active || !Main.tileSolid[(int)TileAt(l, num + 2).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 2; num3++)
					{
						if ((int)TileAt(num2, num3).Type == type && TileAt(num2, num3).Active)
						{
							KillTile(num2, num3, false, false, false);
						}
					}
				}
				if (genRand.Next(50) == 0)
				{
					if ((double)j < Main.worldSurface)
					{
						int num4 = genRand.Next(4);
						if (num4 == 0)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 292, 1, false);
						}
						if (num4 == 1)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 298, 1, false);
						}
						if (num4 == 2)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 299, 1, false);
						}
						if (num4 == 3)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 290, 1, false);
						}
					}
					else if ((double)j < Main.rockLayer)
					{
						int num5 = genRand.Next(7);
						if (num5 == 0)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 289, 1, false);
						}
						if (num5 == 1)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 298, 1, false);
						}
						if (num5 == 2)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 299, 1, false);
						}
						if (num5 == 3)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 290, 1, false);
						}
						if (num5 == 4)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 303, 1, false);
						}
						if (num5 == 5)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 291, 1, false);
						}
						if (num5 == 6)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 304, 1, false);
						}
					}
					else if (j < Main.maxTilesY - 200)
					{
						int num6 = genRand.Next(10);
						if (num6 == 0)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 296, 1, false);
						}
						if (num6 == 1)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 295, 1, false);
						}
						if (num6 == 2)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 299, 1, false);
						}
						if (num6 == 3)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 302, 1, false);
						}
						if (num6 == 4)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 303, 1, false);
						}
						if (num6 == 5)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 305, 1, false);
						}
						if (num6 == 6)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 301, 1, false);
						}
						if (num6 == 7)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 302, 1, false);
						}
						if (num6 == 8)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 297, 1, false);
						}
						if (num6 == 9)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 304, 1, false);
						}
					}
					else
					{
						int num7 = genRand.Next(12);
						if (num7 == 0)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 296, 1, false);
						}
						if (num7 == 1)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 295, 1, false);
						}
						if (num7 == 2)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 293, 1, false);
						}
						if (num7 == 3)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 288, 1, false);
						}
						if (num7 == 4)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 294, 1, false);
						}
						if (num7 == 5)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 297, 1, false);
						}
						if (num7 == 6)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 304, 1, false);
						}
						if (num7 == 7)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 305, 1, false);
						}
						if (num7 == 8)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 301, 1, false);
						}
						if (num7 == 9)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 302, 1, false);
						}
						if (num7 == 10)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 288, 1, false);
						}
						if (num7 == 11)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 300, 1, false);
						}
					}
				}
				else
				{
					int num8 = Main.rand.Next(10);
					if (num8 == 0 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLife < Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLifeMax)
					{
						sandbox.NewItem(i * 16, j * 16, 16, 16, 58, 1, false);
					}
					else if (num8 == 1 && Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statMana < Main.players[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statManaMax)
					{
						sandbox.NewItem(i * 16, j * 16, 16, 16, 184, 1, false);
					}
					else if (num8 == 2)
					{
						int stack = Main.rand.Next(3) + 1;
						if (TileAt(i, j).Liquid > 0)
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 282, stack, false);
						}
						else
						{
							sandbox.NewItem(i * 16, j * 16, 16, 16, 8, stack, false);
						}
					}
					else if (num8 == 3)
					{
						int stack2 = Main.rand.Next(8) + 3;
						int type2 = 40;
						if (j > Main.maxTilesY - 200)
						{
							type2 = 265;
						}
						sandbox.NewItem(i * 16, j * 16, 16, 16, type2, stack2, false);
					}
					else if (num8 == 4)
					{
						int type3 = 28;
						if (j > Main.maxTilesY - 200)
						{
							type3 = 188;
						}
						sandbox.NewItem(i * 16, j * 16, 16, 16, type3, 1, false);
					}
					else if (num8 == 5 && (double)j > Main.rockLayer)
					{
						int stack3 = Main.rand.Next(4) + 1;
						sandbox.NewItem(i * 16, j * 16, 16, 16, 166, stack3, false);
					}
					else
					{
						float num9 = (float)(200 + genRand.Next(-100, 101));
						if ((double)j < Main.worldSurface)
						{
							num9 *= 0.5f;
						}
						else if ((double)j < Main.rockLayer)
						{
							num9 *= 0.75f;
						}
						else if (j > Main.maxTilesY - 250)
						{
							num9 *= 1.25f;
						}
						num9 *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
						if (Main.rand.Next(5) == 0)
						{
							num9 *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;
						}
						if (Main.rand.Next(10) == 0)
						{
							num9 *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;
						}
						if (Main.rand.Next(15) == 0)
						{
							num9 *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;
						}
						if (Main.rand.Next(20) == 0)
						{
							num9 *= 1f + (float)Main.rand.Next(40, 81) * 0.01f;
						}
						if (Main.rand.Next(25) == 0)
						{
							num9 *= 1f + (float)Main.rand.Next(50, 101) * 0.01f;
						}
						while ((int)num9 > 0)
						{
							if (num9 > 1000000f)
							{
								int num10 = (int)(num9 / 1000000f);
								if (num10 > 50 && Main.rand.Next(2) == 0)
								{
									num10 /= Main.rand.Next(3) + 1;
								}
								if (Main.rand.Next(2) == 0)
								{
									num10 /= Main.rand.Next(3) + 1;
								}
								num9 -= (float)(1000000 * num10);
								sandbox.NewItem(i * 16, j * 16, 16, 16, 74, num10, false);
							}
							else if (num9 > 10000f)
							{
								int num11 = (int)(num9 / 10000f);
								if (num11 > 50 && Main.rand.Next(2) == 0)
								{
									num11 /= Main.rand.Next(3) + 1;
								}
								if (Main.rand.Next(2) == 0)
								{
									num11 /= Main.rand.Next(3) + 1;
								}
								num9 -= (float)(10000 * num11);
								sandbox.NewItem(i * 16, j * 16, 16, 16, 73, num11, false);
							}
							else if (num9 > 100f)
							{
								int num12 = (int)(num9 / 100f);
								if (num12 > 50 && Main.rand.Next(2) == 0)
								{
									num12 /= Main.rand.Next(3) + 1;
								}
								if (Main.rand.Next(2) == 0)
								{
									num12 /= Main.rand.Next(3) + 1;
								}
								num9 -= (float)(100 * num12);
								sandbox.NewItem(i * 16, j * 16, 16, 16, 72, num12, false);
							}
							else
							{
								int num13 = (int)num9;
								if (num13 > 50 && Main.rand.Next(2) == 0)
								{
									num13 /= Main.rand.Next(3) + 1;
								}
								if (Main.rand.Next(2) == 0)
								{
									num13 /= Main.rand.Next(4) + 1;
								}
								if (num13 < 1)
								{
									num13 = 1;
								}
								num9 -= (float)num13;
								sandbox.NewItem(i * 16, j * 16, 16, 16, 71, num13, false);
							}
						}
					}
				}
				destroyObject = false;
			}
		}
		
		public int PlaceChest(int x, int y, int type = 21, bool notNearOtherChests = false, int style = 0)
		{
			bool flag = true;
			int num = -1;
			for (int modX = x; modX < x + 2; modX++)
			{
				for (int modY = y - 1; modY < y + 1; modY++)
				{
					var tile = GetTile(modX, modY);
					if (tile.Active || tile.Lava)
					{
						flag = false;
					}
				}

				if (!TileAt(modX, y + 1).Active || !Main.tileSolid[(int)TileAt(modX, y + 1).Type])
				{
					flag = false;
				}
			}
			if (flag && notNearOtherChests)
			{
				for (int k = x - 30; k < x + 30; k++)
				{
					for (int l = y - 10; l < y + 10; l++)
					{
						try
						{
							if (TileAt(k, l).Active && TileAt(k, l).Type == 21)
							{
								flag = false;
								return -1;
							}
						}
						catch
						{
						}
					}
				}
			}
			if (flag)
			{
				num = Chest.CreateChest(x, y - 1);
				if (num == -1)
				{
					flag = false;
				}
			}
			if (flag)
			{
				TileAt(x, y - 1).SetActive (true);
				TileAt(x, y - 1).SetFrameY (0);
				TileAt(x, y - 1).SetFrameX ((short)(36 * style));
				TileAt(x, y - 1).SetType ((byte)type);
				TileAt(x + 1, y - 1).SetActive (true);
				TileAt(x + 1, y - 1).SetFrameY (0);
				TileAt(x + 1, y - 1).SetFrameX ((short)(18 + 36 * style));
				TileAt(x + 1, y - 1).SetType ((byte)type);
				TileAt(x, y).SetActive (true);
				TileAt(x, y).SetFrameY (18);
				TileAt(x, y).SetFrameX ((short)(36 * style));
				TileAt(x, y).SetType ((byte)type);
				TileAt(x + 1, y).SetActive (true);
				TileAt(x + 1, y).SetFrameY (18);
				TileAt(x + 1, y).SetFrameX ((short)(18 + 36 * style));
				TileAt(x + 1, y).SetType ((byte)type);
			}
			return num;
		}
		
		public void CheckChest(int i, int j, int type)
		{
			if (destroyObject)
			{
				return;
			}
			bool flag = false;
			int k = 0;
			k += (int)(TileAt(i, j).FrameX / 18);
			int num = j + (int)(TileAt(i, j).FrameY / 18 * -1);
			while (k > 1)
			{
				k -= 2;
			}
			k *= -1;
			k += i;
			for (int l = k; l < k + 2; l++)
			{
				for (int m = num; m < num + 2; m++)
				{
					int n;
					for (n = (int)(TileAt(l, m).FrameX / 18); n > 1; n -= 2)
					{
					}
					if (!TileAt(l, m).Active || (int)TileAt(l, m).Type != type || n != l - k || (int)TileAt(l, m).FrameY != (m - num) * 18)
					{
						flag = true;
					}
				}
				if (!TileAt(l, num + 2).Active || !Main.tileSolid[(int)TileAt(l, num + 2).Type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				int type2 = 48;
				if (TileAt(i, j).FrameX >= 216)
				{
					type2 = 348;
				}
				else if (TileAt(i, j).FrameX >= 180)
				{
					type2 = 343;
				}
				else if (TileAt(i, j).FrameX >= 108)
				{
					type2 = 328;
				}
				else if (TileAt(i, j).FrameX >= 36)
				{
					type2 = 306;
				}
				destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 3; num3++)
					{
						if ((int)TileAt(num2, num3).Type == type && TileAt(num2, num3).Active)
						{
							sandbox.DestroyChest (num2, num3);
							KillTile(num2, num3, false, false, false);
						}
					}
				}
				sandbox.NewItem(i * 16, j * 16, 32, 32, type2, 1, false);
				destroyObject = false;
			}
		}

		public void Place1xX(int x, int y, int type, int style = 0)
		{
			int num = style * 18;
			int num2 = 3;
			if (type == 92)
			{
				num2 = 6;
			}
			bool flag = true;
			for (int i = y - num2 + 1; i < y + 1; i++)
			{
				if (TileAt(x, i).Active)
				{
					flag = false;
				}
				if (type == 93 && TileAt(x, i).Liquid > 0)
				{
					flag = false;
				}
			}
			if (flag && TileAt(x, y + 1).Active && Main.tileSolid[(int)TileAt(x, y + 1).Type])
			{
				for (int j = 0; j < num2; j++)
				{
					TileAt(x, y - num2 + 1 + j).SetActive(true);
					TileAt(x, y - num2 + 1 + j).SetFrameY((short)(j * 18));
					TileAt(x, y - num2 + 1 + j).SetFrameX((short)num);
					TileAt(x, y - num2 + 1 + j).SetType((byte)type);
				}
			}
		}

		public void Place2xX(int x, int y, int type, int style = 0)
		{
			int num = style * 18;
			int num2 = 3;
			if (type == 104)
			{
				num2 = 5;
			}
			bool flag = true;
			for (int i = y - num2 + 1; i < y + 1; i++)
			{
				if (TileAt(x, i).Active)
				{
					flag = false;
				}
				if (TileAt(x + 1, i).Active)
				{
					flag = false;
				}
			}
			if (flag && TileAt(x, y + 1).Active && Main.tileSolid[(int)TileAt(x, y + 1).Type] &&
				TileAt(x + 1, y + 1).Active && Main.tileSolid[(int)TileAt(x + 1, y + 1).Type])
			{
				for (int j = 0; j < num2; j++)
				{
					TileAt(x, y - num2 + 1 + j).SetActive(true);
					TileAt(x, y - num2 + 1 + j).SetFrameY((short)(j * 18));
					TileAt(x, y - num2 + 1 + j).SetFrameX((short)num);
					TileAt(x, y - num2 + 1 + j).SetType((byte)type);
					TileAt(x + 1, y - num2 + 1 + j).SetActive(true);
					TileAt(x + 1, y - num2 + 1 + j).SetFrameY((short)(j * 18));
					TileAt(x + 1, y - num2 + 1 + j).SetFrameX((short)(num + 18));
					TileAt(x + 1, y - num2 + 1 + j).SetType((byte)type);
				}
			}
		}
		
		public void Place3x4(int x, int y, int type)
		{
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = x - 1; i < x + 2; i++)
			{
				for (int j = y - 3; j < y + 1; j++)
				{
					if (TileAt(i, j).Active)
					{
						flag = false;
					}
				}
				if (!TileAt(i, y + 1).Active || !Main.tileSolid[(int)TileAt(i, y + 1).Type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				for (int k = -3; k <= 0; k++)
				{
					short frameY = (short)((3 + k) * 18);
					TileAt(x - 1, y + k).SetActive(true);
					TileAt(x - 1, y + k).SetFrameY(frameY);
					TileAt(x - 1, y + k).SetFrameX(0);
					TileAt(x - 1, y + k).SetType((byte)type);
					TileAt(x, y + k).SetActive(true);
					TileAt(x, y + k).SetFrameY(frameY);
					TileAt(x, y + k).SetFrameX(18);
					TileAt(x, y + k).SetType((byte)type);
					TileAt(x + 1, y + k).SetActive(true);
					TileAt(x + 1, y + k).SetFrameY(frameY);
					TileAt(x + 1, y + k).SetFrameX(36);
					TileAt(x + 1, y + k).SetType((byte)type);
				}
			}
		}
		
		public bool PlaceTile(int i, int j, int type, bool mute = false, bool forced = false, int plr = -1, int style = 0)
		{
			if (type >= MAX_TILE_SETS)
			{
				return false;
			}
			bool result = false;
			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
			{
				if (forced || Collision.EmptyTile(i, j, false) || !Main.tileSolid[type] || (type == 23 && TileAt(i, j).Type == 0 && TileAt(i, j).Active) || (type == 2 && TileAt(i, j).Type == 0 && TileAt(i, j).Active) || (type == 60 && TileAt(i, j).Type == 59 && TileAt(i, j).Active) || (type == 70 && TileAt(i, j).Type == 59 && TileAt(i, j).Active))
				{
					if (type == 23 && (TileAt(i, j).Type != 0 || !TileAt(i, j).Active))
					{
						return false;
					}
					if (type == 2 && (TileAt(i, j).Type != 0 || !TileAt(i, j).Active))
					{
						return false;
					}
					if (type == 60 && (TileAt(i, j).Type != 59 || !TileAt(i, j).Active))
					{
						return false;
					}
					if (type == 81)
					{
						if (TileAt(i - 1, j).Active || TileAt(i + 1, j).Active || TileAt(i, j - 1).Active)
						{
							return false;
						}
						if (!TileAt(i, j + 1).Active || !Main.tileSolid[(int)TileAt(i, j + 1).Type])
						{
							return false;
						}
					}
					if (TileAt(i, j).Liquid > 0 && (type == 3 || type == 4 || type == 20 || type == 24 || type == 27 || type == 32 || type == 51 || type == 69 || type == 72))
					{
						return false;
					}
					TileAt(i, j).SetFrameY (0);
					TileAt(i, j).SetFrameX (0);
					if (type == 3 || type == 24)
					{
						if (j + 1 < Main.maxTilesY && TileAt(i, j + 1).Active && ((TileAt(i, j + 1).Type == 2 && type == 3) || (TileAt(i, j + 1).Type == 23 && type == 24) || (TileAt(i, j + 1).Type == 78 && type == 3)))
						{
							if (type == 24 && genRand.Next(13) == 0)
							{
								TileAt(i, j).SetActive (true);
								TileAt(i, j).SetType (32);
								SquareTileFrame(i, j, true);
							}
							else if (TileAt(i, j + 1).Type == 78)
							{
								TileAt(i, j).SetActive (true);
								TileAt(i, j).SetType ((byte)type);
								TileAt(i, j).SetFrameX ((short)(genRand.Next(2) * 18 + 108));
							}
							else if (TileAt(i, j).Wall == 0 && TileAt(i, j + 1).Wall == 0)
							{
								if (genRand.Next(50) == 0 || (type == 24 && genRand.Next(40) == 0))
								{
									TileAt(i, j).SetActive (true);
									TileAt(i, j).SetType ((byte)type);
									TileAt(i, j).SetFrameX (144);
								}
								else if (genRand.Next(35) == 0)
								{
									TileAt(i, j).SetActive (true);
									TileAt(i, j).SetType ((byte)type);
									TileAt(i, j).SetFrameX ((short)(genRand.Next(2) * 18 + 108));
								}
								else
								{
									TileAt(i, j).SetActive (true);
									TileAt(i, j).SetType ((byte)type);
									TileAt(i, j).SetFrameX ((short)(genRand.Next(6) * 18));
								}
							}
						}
					}
					else if (type == 61)
					{
						if (j + 1 < Main.maxTilesY && TileAt(i, j + 1).Active && TileAt(i, j + 1).Type == 60)
						{
							if (genRand.Next(10) == 0 && (double)j > Main.worldSurface)
							{
								TileAt(i, j).SetActive (true);
								TileAt(i, j).SetType (69);
								SquareTileFrame(i, j, true);
							}
							else if (genRand.Next(15) == 0 && (double)j > Main.worldSurface)
							{
								TileAt(i, j).SetActive (true);
								TileAt(i, j).SetType ((byte)type);
								TileAt(i, j).SetFrameX (144);
							}
							else if (genRand.Next(1000) == 0 && (double)j > Main.rockLayer)
							{
								TileAt(i, j).SetActive (true);
								TileAt(i, j).SetType ((byte)type);
								TileAt(i, j).SetFrameX (162);
							}
							else
							{
								TileAt(i, j).SetActive (true);
								TileAt(i, j).SetType ((byte)type);
								if ((double)j > Main.rockLayer)
								{
									TileAt(i, j).SetFrameX ((short)(genRand.Next(8) * 18));
								}
								else
								{
									TileAt(i, j).SetFrameX ((short)(genRand.Next(6) * 18));
								}
							}
						}
					}
					else if (type == 71)
					{
						if (j + 1 < Main.maxTilesY && TileAt(i, j + 1).Active && TileAt(i, j + 1).Type == 70)
						{
							TileAt(i, j).SetActive (true);
							TileAt(i, j).SetType ((byte)type);
							TileAt(i, j).SetFrameX ((short)(genRand.Next(5) * 18));
						}
					}
					else if (type == 4)
					{
						if ((TileAt(i - 1, j).Active && (Main.tileSolid[(int)TileAt(i - 1, j).Type] || (TileAt(i - 1, j).Type == 5 && TileAt(i - 1, j - 1).Type == 5 && TileAt(i - 1, j + 1).Type == 5))) || (TileAt(i + 1, j).Active && (Main.tileSolid[(int)TileAt(i + 1, j).Type] || (TileAt(i + 1, j).Type == 5 && TileAt(i + 1, j - 1).Type == 5 && TileAt(i + 1, j + 1).Type == 5))) || (TileAt(i, j + 1).Active && Main.tileSolid[(int)TileAt(i, j + 1).Type]))
						{
							TileAt(i, j).SetActive (true);
							TileAt(i, j).SetType ((byte)type);
							SquareTileFrame(i, j, true);
						}
					}
					else if (type == 10)
					{
						if (!TileAt(i, j - 1).Active && !TileAt(i, j - 2).Active && TileAt(i, j - 3).Active && Main.tileSolid[(int)TileAt(i, j - 3).Type])
						{
							PlaceDoor(i, j - 1, type);
							SquareTileFrame(i, j, true);
						}
						else
						{
							if (TileAt(i, j + 1).Active || TileAt(i, j + 2).Active || !TileAt(i, j + 3).Active || !Main.tileSolid[(int)TileAt(i, j + 3).Type])
							{
								return false;
							}
							PlaceDoor(i, j + 1, type);
							SquareTileFrame(i, j, true);
						}
					}
					else if (type == 34 || type == 35 || type == 36 || type == 106)
					{
						Place3x3(i, j, type);
						SquareTileFrame(i, j, true);
					}
					else if (type == 13 || type == 33 || type == 49 || type == 50 || type == 78)
					{
						PlaceOnTable1x1(i, j, type, style);
						SquareTileFrame(i, j, true);
					}
					else if (type == 14 || type == 26 || type == 86 || type == 87 || type == 88 || type == 89)
					{
						Place3x2(i, j, type);
						SquareTileFrame(i, j, true);
					}
					else if (type == 20)
					{
						if (TileAt(i, j + 1).Active && TileAt(i, j + 1).Type == 2)
						{
							Place1x2(i, j, type, style);
							SquareTileFrame(i, j, true);
						}
					}
					else if (type == 15)
					{
						Place1x2(i, j, type, style);
						SquareTileFrame(i, j, true);
					}
					else if (type == 16 || type == 18 || type == 29 || type == 103)
					{
						Place2x1(i, j, type);
						SquareTileFrame(i, j, true);
					}
					else if (type == 92 || type == 93)
					{
						Place1xX(i, j, type, 0);
						SquareTileFrame(i, j, true);
					}
					else if (type == 104 || type == 105)
					{
						Place2xX(i, j, type, 0);
						SquareTileFrame(i, j, true);
					}
					else if (type == 17 || type == 77)
					{
						Place3x2(i, j, type);
						SquareTileFrame(i, j, true);
					}
					else if (type == 21)
					{
						PlaceChest(i, j, type, false, style);
						SquareTileFrame(i, j, true);
					}
					else if (type == 91)
					{
						PlaceBanner(i, j, type, style);
						SquareTileFrame(i, j, true);
					}
					else if (type == 101 || type == 102)
					{
						Place3x4(i, j, type);
						SquareTileFrame(i, j, true);
					}
					else if (type == 27)
					{
						PlaceSunflower(i, j, 27);
						SquareTileFrame(i, j, true);
					}
					else if (type == 28)
					{
						PlacePot(i, j, 28);
						SquareTileFrame(i, j, true);
					}
					else if (type == 42)
					{
						Place1x2Top(i, j, type);
						SquareTileFrame(i, j, true);
					}
					else if (type == 55 || type == 85)
					{
						PlaceSign(i, j, type);
					}
					else if (Main.tileAlch[type])
					{
						PlaceAlch(i, j, style);
					}
					else if (type == 94 || type == 95 || type == 96 || type == 97 || type == 98 || type == 99 || type == 100)
					{
						Place2x2(i, j, type);
					}
					else if (type == 79 || type == 90)
					{
						int direction = 1;
						if (plr > -1)
						{
							direction = Main.players[plr].direction;
						}
						Place4x2(i, j, type, direction);
					}
					else if (type == 81)
					{
						TileAt(i, j).SetFrameX((short)(26 * genRand.Next(6)));
						TileAt(i, j).SetActive(true);
						TileAt(i, j).SetType((byte)type);
					}
					else
					{
						TileAt(i, j).SetActive(true);
						TileAt(i, j).SetType((byte)type);
					}
					if (TileAt(i, j).Active && !mute)
					{
						SquareTileFrame(i, j, true);
						result = true;
					}
				}
			}
			return result;
		}
		
		public void KillWall(int i, int j, bool fail = false)
		{
			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
			{
				if (TileAt(i, j).Wall > 0)
				{
					if (fail)
					{
						return;
					}
					int num2 = 0;
					if (TileAt(i, j).Wall == 1)
					{
						num2 = 26;
					}
					if (TileAt(i, j).Wall == 4)
					{
						num2 = 93;
					}
					if (TileAt(i, j).Wall == 5)
					{
						num2 = 130;
					}
					if (TileAt(i, j).Wall == 6)
					{
						num2 = 132;
					}
					if (TileAt(i, j).Wall == 7)
					{
						num2 = 135;
					}
					if (TileAt(i, j).Wall == 8)
					{
						num2 = 138;
					}
					if (TileAt(i, j).Wall == 9)
					{
						num2 = 140;
					}
					if (TileAt(i, j).Wall == 10)
					{
						num2 = 142;
					}
					if (TileAt(i, j).Wall == 11)
					{
						num2 = 144;
					}
					if (TileAt(i, j).Wall == 12)
					{
						num2 = 146;
					}
					if (TileAt(i, j).Wall == 14)
					{
						num2 = 330;
					}
					if (TileAt(i, j).Wall == 16)
					{
						num2 = 30;
					}
					if (TileAt(i, j).Wall == 17)
					{
						num2 = 135;
					}
					if (TileAt(i, j).Wall == 18)
					{
						num2 = 138;
					}
					if (TileAt(i, j).Wall == 19)
					{
						num2 = 140;
					}
					if (TileAt(i, j).Wall == 20)
					{
						num2 = 330;
					}
					if (num2 > 0)
					{
						sandbox.NewItem(i * 16, j * 16, 16, 16, num2, 1, false);
					}
					TileAt(i, j).SetWall (0);
				}
			}
		}
		
		public void KillTile(int x, int y, bool fail = false, bool effectOnly = false, bool noItem = false, Player player = null)
		{
			if (x >= 0 && y >= 0 && x < Main.maxTilesX && y < Main.maxTilesY)
			{
				var tile = TileAt(x, y);
				var type = tile.Type;

				if (tile.Active)
				{
					if (y >= 1 && TileAt(x, y - 1).Active && ((TileAt(x, y - 1).Type == 5 && tile.Type != 5) || (TileAt(x, y - 1).Type == 21 && tile.Type != 21) || (TileAt(x, y - 1).Type == 26 && tile.Type != 26) || (TileAt(x, y - 1).Type == 72 && tile.Type != 72) || (TileAt(x, y - 1).Type == 12 && tile.Type != 12)) && (TileAt(x, y - 1).Type != 5 || ((TileAt(x, y - 1).FrameX != 66 || TileAt(x, y - 1).FrameY < 0 || TileAt(x, y - 1).FrameY > 44) && (TileAt(x, y - 1).FrameX != 88 || TileAt(x, y - 1).FrameY < 66 || TileAt(x, y - 1).FrameY > 110) && TileAt(x, y - 1).FrameY < 198)))
					{
						return;
					}
					if (!effectOnly && !stopDrops)
					{
						if (tile.Type == 3)
						{
							if (tile.FrameX == 144)
							{
								sandbox.NewItem(x * 16, y * 16, 16, 16, 5, 1, false);
							}
						}
						else if (tile.Type == 24)
						{
							if (tile.FrameX == 144)
							{
								sandbox.NewItem(x * 16, y * 16, 16, 16, 60, 1, false);
							}
						}
					}
					if (effectOnly)
					{
						return;
					}
					if (fail)
					{
						if (type == 2 || type == 23)
						{
							tile.SetType (0);
						}
						else if (type == 60 || type == 70)
						{
							tile.SetType (59);
						}
						SquareTileFrame(x, y, true);
						return;
					}
					if (type == 21)
					{
						int l = (int)(tile.FrameX / 18);
						int chestY = y - (int)(tile.FrameY / 18);
						while (l > 1)
						{
							l -= 2;
						}
						l = x - l;
						sandbox.DestroyChest (l, chestY);
					}
					if (!noItem && !stopDrops)
					{
						int dropItem = 0;
						if (tile.Type == 0 || tile.Type == 2)
						{
							dropItem = 2;
						}
						else if (tile.Type == 1)
						{
							dropItem = 3;
						}
						else if (tile.Type == 3 || tile.Type == 73)
						{
							if (Main.rand.Next(2) == 0 && Main.players[(int)Player.FindClosest(new Vector2((float)(x * 16), (float)(y * 16)), 16, 16)].HasItem(281))
							{
								dropItem = 283;
							}
						}
						else if (tile.Type == 4)
						{
							dropItem = 8;
						}
						else if (tile.Type == 5)
						{
							if (tile.FrameX >= 22 && tile.FrameY >= 198)
							{
								if (genRand.Next(2) == 0)
								{
									int num5 = y;
									while ((!TileAt(x, num5).Active || !Main.tileSolid[(int)TileAt(x, num5).Type] || Main.tileSolidTop[(int)TileAt(x, num5).Type]))
									{
										num5++;
									}
									if (TileAt(x, num5).Type == 2)
									{
										dropItem = 27;
									}
									else
									{
										dropItem = 9;
									}
								}
								else
								{
									dropItem = 9;
								}
							}
							else
							{
								dropItem = 9;
							}
						}
						else if (tile.Type == 6)
						{
							dropItem = 11;
						}
						else if (tile.Type == 7)
						{
							dropItem = 12;
						}
						else if (tile.Type == 8)
						{
							dropItem = 13;
						}
						else if (tile.Type == 9)
						{
							dropItem = 14;
						}
						else if (tile.Type == 13)
						{
							if (tile.FrameX == 18)
							{
								dropItem = 28;
							}
							else if (tile.FrameX == 36)
							{
								dropItem = 110;
							}
							else if (tile.FrameX == 54)
							{
								dropItem = 350;
							}
							else if (TileAt(x, y).FrameX == 72)
							{
								dropItem = 351;
							}
							else
							{
								dropItem = 31;
							}
						}
						else if (tile.Type == 19)
						{
							dropItem = 94;
						}
						else if (tile.Type == 22)
						{
							dropItem = 56;
						}
						else if (tile.Type == 23)
						{
							dropItem = 2;
						}
						else if (tile.Type == 25)
						{
							dropItem = 61;
						}
						else if (tile.Type == 30)
						{
							dropItem = 9;
						}
						else if (tile.Type == 33)
						{
							dropItem = 105;
						}
						else if (tile.Type == 37)
						{
							dropItem = 116;
						}
						else if (tile.Type == 38)
						{
							dropItem = 129;
						}
						else if (tile.Type == 39)
						{
							dropItem = 131;
						}
						else if (tile.Type == 40)
						{
							dropItem = 133;
						}
						else if (tile.Type == 41)
						{
							dropItem = 134;
						}
						else if (tile.Type == 43)
						{
							dropItem = 137;
						}
						else if (tile.Type == 44)
						{
							dropItem = 139;
						}
						else if (tile.Type == 45)
						{
							dropItem = 141;
						}
						else if (tile.Type == 46)
						{
							dropItem = 143;
						}
						else if (tile.Type == 47)
						{
							dropItem = 145;
						}
						else if (tile.Type == 48)
						{
							dropItem = 147;
						}
						else if (tile.Type == 49)
						{
							dropItem = 148;
						}
						else if (tile.Type == 51)
						{
							dropItem = 150;
						}
						else if (tile.Type == 53)
						{
							dropItem = 169;
						}
						else if (tile.Type != 54)
						{
							if (tile.Type == 56)
							{
								dropItem = 173;
							}
							else if (tile.Type == 57)
							{
								dropItem = 172;
							}
							else if (tile.Type == 58)
							{
								dropItem = 174;
							}
							else if (tile.Type == 60)
							{
								dropItem = 176;
							}
							else if (tile.Type == 70)
							{
								dropItem = 176;
							}
							else if (tile.Type == 75)
							{
								dropItem = 192;
							}
							else if (tile.Type == 76)
							{
								dropItem = 214;
							}
							else if (tile.Type == 78)
							{
								dropItem = 222;
							}
							else if (tile.Type == 81)
							{
								dropItem = 275;
							}
							else if (tile.Type == 80)
							{
								dropItem = 276;
							}
							else if (tile.Type == 61 || tile.Type == 74)
							{
								if (tile.FrameX == 144)
								{
									sandbox.NewItem(x * 16, y * 16, 16, 16, 331, genRand.Next(1, 3), false);
								}
								if (tile.FrameX == 162)
								{
									dropItem = 223;
								}
								else if (tile.FrameX >= 108 && tile.FrameX <= 126 && genRand.Next(100) == 0)
								{
									dropItem = 208;
								}
								else if (genRand.Next(100) == 0)
								{
									dropItem = 195;
								}
							}
							else if (tile.Type == 59 || tile.Type == 60)
							{
								dropItem = 176;
							}
							else if (tile.Type == 71 || tile.Type == 72)
							{
								if (genRand.Next(50) == 0)
								{
									dropItem = 194;
								}
								else if (genRand.Next(2) == 0)
								{
									dropItem = 183;
								}
							}
							else if (tile.Type >= 63 && tile.Type <= 68)
							{
								dropItem = (int)(tile.Type - 63 + 177);
							}
							else if (tile.Type == 50)
							{
								if (tile.FrameX == 90)
								{
									dropItem = 165;
								}
								else
								{
									dropItem = 149;
								}
							}
							else if (Main.tileAlch[(int)tile.Type] && tile.Type > 82)
							{
								int num6 = (int)(tile.FrameX / 18);
								bool flag = false;
								if (tile.Type == 84)
								{
									flag = true;
								}
								if (num6 == 0 && Main.dayTime)
								{
									flag = true;
								}
								if (num6 == 1 && !Main.dayTime)
								{
									flag = true;
								}
								if (num6 == 3 && Main.bloodMoon)
								{
									flag = true;
								}
								dropItem = 313 + num6;
								if (flag)
								{
									sandbox.NewItem(x * 16, y * 16, 16, 16, 307 + num6, genRand.Next(1, 4), false);
								}
							}
						}
						if (dropItem > 0)
						{
							sandbox.NewItem(x * 16, y * 16, 16, 16, dropItem, 1, false);
						}
					}
					tile.SetActive (false);
					if (Main.tileSolid[(int)tile.Type])
					{
						tile.SetLighted (false);
					}
					tile.SetFrameX (-1);
					tile.SetFrameY (-1);
					tile.SetFrameNumber (0);
					if (tile.Type == 58 && y > Main.maxTilesY - 200)
					{
						tile.SetLava(true);
						tile.SetLiquid(128);
					}
					tile.SetType (0);
					SquareTileFrame(x, y, true);
				}
			}
		}
		
		public bool PlayerLOS(int x, int y)
		{
			Rectangle rectangle = new Rectangle(x * 16, y * 16, 16, 16);
			for (int i = 0; i < 255; i++)
			{
				if (Main.players[i].Active)
				{
					Rectangle value = new Rectangle((int)((double)Main.players[i].Position.X + (double)Main.players[i].Width * 0.5 - (double)NPC.sWidth * 0.6), (int)((double)Main.players[i].Position.Y + (double)Main.players[i].Height * 0.5 - (double)NPC.sHeight * 0.6), (int)((double)NPC.sWidth * 1.2), (int)((double)NPC.sHeight * 1.2));
					if (rectangle.Intersects(value))
					{
						return true;
					}
				}
			}
			return false;
		}
		
		public void PlaceWall(int i, int j, int type, bool mute = false)
		{
			if ((int)TileAt(i, j).Wall != type)
			{
				for (int k = i - 1; k < i + 2; k++)
				{
					for (int l = j - 1; l < j + 2; l++)
					{
						if (TileAt(k, l).Wall > 0 && (int)TileAt(k, l).Wall != type)
						{
							bool flag = false;
							if (TileAt(i, j).Wall == 0 && (type == 2 || type == 16) && (TileAt(k, l).Wall == 2 || TileAt(k, l).Wall == 16))
							{
								flag = true;
							}
							if (!flag)
							{
								return;
							}
						}
					}
				}
				TileAt(i, j).SetWall ((byte)type);
			}
		}
		
		public void SpreadGrass(int i, int j, int dirt = 0, int grass = 2, bool repeat = true)
		{
			if ((int)TileAt(i, j).Type != dirt || !TileAt(i, j).Active || ((double)j < Main.worldSurface && grass == 70) || ((double)j >= Main.worldSurface && dirt == 0))
			{
				return;
			}
			int num = i - 1;
			int num2 = i + 2;
			int num3 = j - 1;
			int num4 = j + 2;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 > Main.maxTilesX)
			{
				num2 = Main.maxTilesX;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 > Main.maxTilesY)
			{
				num4 = Main.maxTilesY;
			}
			bool flag = true;
			for (int k = num; k < num2; k++)
			{
				for (int l = num3; l < num4; l++)
				{
					if (!TileAt(k, l).Active || !Main.tileSolid[(int)TileAt(k, l).Type])
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag)
			{
				if (grass == 23 && TileAt(i, j - 1).Type == 27)
				{
					return;
				}
				TileAt(i, j).SetType ((byte)grass);
				for (int m = num; m < num2; m++)
				{
					for (int n = num3; n < num4; n++)
					{
						if (TileAt(m, n).Active && (int)TileAt(m, n).Type == dirt && repeat)
						{
							SpreadGrass(m, n, dirt, grass, true);
						}
					}
				}
			}
		}
		
		public void SquareTileFrame(int i, int j, bool resetFrame = true)
		{
			TileFrame(i - 1, j - 1, false, false);
			TileFrame(i - 1, j, false, false);
			TileFrame(i - 1, j + 1, false, false);
			TileFrame(i, j - 1, false, false);
			TileFrame(i, j, resetFrame, false);
			TileFrame(i, j + 1, false, false);
			TileFrame(i + 1, j - 1, false, false);
			TileFrame(i + 1, j, false, false);
			TileFrame(i + 1, j + 1, false, false);
		}
		
		public void SectionTileFrame(int startX, int startY, int endX, int endY)
		{
			int num = startX * 200;
			int num2 = (endX + 1) * 200;
			int num3 = startY * 150;
			int num4 = (endY + 1) * 150;
			if (num < 1)
			{
				num = 1;
			}
			if (num3 < 1)
			{
				num3 = 1;
			}
			if (num > Main.maxTilesX - 2)
			{
				num = Main.maxTilesX - 2;
			}
			if (num3 > Main.maxTilesY - 2)
			{
				num3 = Main.maxTilesY - 2;
			}
			for (int i = num - 1; i < num2 + 1; i++)
			{
				for (int j = num3 - 1; j < num4 + 1; j++)
				{
					TileFrame(i, j, true, true);
				}
			}
		}
		
		public void RangeFrame(int startX, int startY, int endX, int endY)
		{
			int num = endX + 1;
			int num2 = endY + 1;
			for (int i = startX - 1; i < num + 1; i++)
			{
				for (int j = startY - 1; j < num2 + 1; j++)
				{
					TileFrame(i, j, false, false);
				}
			}
		}
		
		public void PlantCheck(int i, int j)
		{
			int num = -1;
			int type = (int)TileAt(i, j).Type;
			if (j + 1 >= Main.maxTilesY)
			{
				num = type;
			}
			if (j + 1 < Main.maxTilesY && TileAt(i, j + 1).Active)
			{
				num = (int)TileAt(i, j + 1).Type;
			}
			if ((type == 3 && num != 2 && num != 78) || (type == 24 && num != 23) || (type == 61 && num != 60) || (type == 71 && num != 70) || (type == 73 && num != 2 && num != 78) || (type == 74 && num != 60))
			{
				KillTile(i, j, false, false, false);
			}
		}

		public void TileFrame(int i, int j, bool resetFrame = false, bool noBreak = false)
		{
			if (i > 5 && j > 5 && i < Main.maxTilesX - 5 && j < Main.maxTilesY - 5)
			{
				if (TileAt(i, j).Liquid > 0 && !noLiquidCheck)
				{
					sandbox.AddWater(i, j);
				}
				if (TileAt(i, j).Active)
				{
					if (noBreak && Main.tileFrameImportant[(int)TileAt(i, j).Type])
					{
						return;
					}
					int num = -1;
					int num2 = -1;
					int num3 = -1;
					int num4 = -1;
					int num5 = -1;
					int num6 = -1;
					int num7 = -1;
					int num8 = -1;
					int num9 = (int)TileAt(i, j).Type;
					if (Main.tileStone[num9])
					{
						num9 = 1;
					}
					int frameX = (int)TileAt(i, j).FrameX;
					int frameY = (int)TileAt(i, j).FrameY;
					Rectangle rectangle;
					rectangle.X = -1;
					rectangle.Y = -1;
					mergeUp = false;
					mergeDown = false;
					mergeLeft = false;
					mergeRight = false;
					if (TileAt(i - 1, j).Active)
					{
						num4 = (int)TileAt(i - 1, j).Type;
					}
					if (TileAt(i + 1, j).Active)
					{
						num5 = (int)TileAt(i + 1, j).Type;
					}
					if ( TileAt(i, j - 1).Active)
					{
						num2 = (int)TileAt(i, j - 1).Type;
					}
					if (TileAt(i, j + 1).Active)
					{
						num7 = (int)TileAt(i, j + 1).Type;
					}
					if (TileAt(i - 1, j - 1).Active)
					{
						num = (int)TileAt(i - 1, j - 1).Type;
					}
					if (TileAt(i + 1, j - 1).Active)
					{
						num3 = (int)TileAt(i + 1, j - 1).Type;
					}
					if (TileAt(i - 1, j + 1).Active)
					{
						num6 = (int)TileAt(i - 1, j + 1).Type;
					}
					if (TileAt(i + 1, j + 1).Active)
					{
						num8 = (int)TileAt(i + 1, j + 1).Type;
					}
					if (num4 >= 0 && Main.tileStone[num4])
					{
						num4 = 1;
					}
					if (num5 >= 0 && Main.tileStone[num5])
					{
						num5 = 1;
					}
					if (num2 >= 0 && Main.tileStone[num2])
					{
						num2 = 1;
					}
					if (num7 >= 0 && Main.tileStone[num7])
					{
						num7 = 1;
					}
					if (num >= 0 && Main.tileStone[num])
					{
						num = 1;
					}
					if (num3 >= 0 && Main.tileStone[num3])
					{
						num3 = 1;
					}
					if (num6 >= 0 && Main.tileStone[num6])
					{
						num6 = 1;
					}
					if (num8 >= 0 && Main.tileStone[num8])
					{
						num8 = 1;
					}
					if (num9 != 0 && num9 != 1)
					{
						if (num9 == 3 || num9 == 24 || num9 == 61 || num9 == 71 || num9 == 73 || num9 == 74)
						{
							PlantCheck(i, j);
							return;
						}
						if (num9 == 4)
						{
							if (num7 >= 0 && Main.tileSolid[num7] && !Main.tileNoAttach[num7])
							{
								TileAt(i, j).SetFrameX(0);
								return;
							}
							if ((num4 >= 0 && Main.tileSolid[num4] && !Main.tileNoAttach[num4]) || (num4 == 5 && num == 5 && num6 == 5))
							{
								TileAt(i, j).SetFrameX(22);
								return;
							}
							if ((num5 >= 0 && Main.tileSolid[num5] && !Main.tileNoAttach[num5]) || (num5 == 5 && num3 == 5 && num8 == 5))
							{
								TileAt(i, j).SetFrameX(44);
								return;
							}
							KillTile(i, j, false, false, false);
							return;
						}
						else
						{
							if (num9 == 80)
							{
								CactusFrame(i, j);
								return;
							}
							if (num9 == 12 || num9 == 31)
							{
								if (!destroyObject)
								{
									int num10 = i;
									int num11 = j;
									if (TileAt(i, j).FrameX == 0)
									{
										num10 = i;
									}
									else
									{
										num10 = i - 1;
									}
									if (TileAt(i, j).FrameY == 0)
									{
										num11 = j;
									}
									else
									{
										num11 = j - 1;
									}
									if ((!TileAt(num10, num11).Active || (int)TileAt(num10, num11).Type != num9 ||
										!TileAt(num10 + 1, num11).Active || (int)TileAt(num10 + 1, num11).Type != num9 ||
											!TileAt(num10, num11 + 1).Active || (int)TileAt(num10, num11 + 1).Type != num9 ||
												!TileAt(num10 + 1, num11 + 1).Active || 
													(int)TileAt(num10 + 1, num11 + 1).Type != num9))
									{
										destroyObject = true;
										if ((int)TileAt(num10, num11).Type == num9)
										{
											KillTile(num10, num11, false, false, false);
										}
										if ((int)TileAt(num10 + 1, num11).Type == num9)
										{
											KillTile(num10 + 1, num11, false, false, false);
										}
										if ((int)TileAt(num10, num11 + 1).Type == num9)
										{
											KillTile(num10, num11 + 1, false, false, false);
										}
										if ((int)TileAt(num10 + 1, num11 + 1).Type == num9)
										{
											KillTile(num10 + 1, num11 + 1, false, false, false);
										}
										if (!noTileActions)
										{
											if (num9 == 12)
											{
												sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 29, 1, false);
											}
											else
											{
												if (num9 == 31)
												{
													if (genRand.Next(2) == 0)
													{
														spawnMeteor = true;
													}
													int num12 = Main.rand.Next(5);
													if (!WorldModify.shadowOrbSmashed)
													{
														num12 = 0;
													}
													if (num12 == 0)
													{
														sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 96, 1, false);
														int stack = genRand.Next(25, 51);
														sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 97, stack, false);
													}
													else
													{
														if (num12 == 1)
														{
															sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 64, 1, false);
														}
														else
														{
															if (num12 == 2)
															{
																sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 162, 1, false);
															}
															else
															{
																if (num12 == 3)
																{
																	sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 115, 1, false);
																}
																else
																{
																	if (num12 == 4)
																	{
																		sandbox.NewItem(num10 * 16, num11 * 16, 32, 32, 111, 1, false);
																	}
																}
															}
														}
													}
													sandbox.ShadowOrbSmashed (num10, num11);
												}
											}
										}
										destroyObject = false;
									}
								}
								return;
							}
							if (num9 == 19)
							{
								if (num4 == num9 && num5 == num9)
								{
									if (TileAt(i, j).FrameNumber == 0)
									{
										rectangle.X = 0;
										rectangle.Y = 0;
									}
									if (TileAt(i, j).FrameNumber == 1)
									{
										rectangle.X = 0;
										rectangle.Y = 18;
									}
									if (TileAt(i, j).FrameNumber == 2)
									{
										rectangle.X = 0;
										rectangle.Y = 36;
									}
								}
								else
								{
									if (num4 == num9 && num5 == -1)
									{
										if (TileAt(i, j).FrameNumber == 0)
										{
											rectangle.X = 18;
											rectangle.Y = 0;
										}
										if (TileAt(i, j).FrameNumber == 1)
										{
											rectangle.X = 18;
											rectangle.Y = 18;
										}
										if (TileAt(i, j).FrameNumber == 2)
										{
											rectangle.X = 18;
											rectangle.Y = 36;
										}
									}
									else
									{
										if (num4 == -1 && num5 == num9)
										{
											if (TileAt(i, j).FrameNumber == 0)
											{
												rectangle.X = 36;
												rectangle.Y = 0;
											}
											if (TileAt(i, j).FrameNumber == 1)
											{
												rectangle.X = 36;
												rectangle.Y = 18;
											}
											if (TileAt(i, j).FrameNumber == 2)
											{
												rectangle.X = 36;
												rectangle.Y = 36;
											}
										}
										else
										{
											if (num4 != num9 && num5 == num9)
											{
												if (TileAt(i, j).FrameNumber == 0)
												{
													rectangle.X = 54;
													rectangle.Y = 0;
												}
												if (TileAt(i, j).FrameNumber == 1)
												{
													rectangle.X = 54;
													rectangle.Y = 18;
												}
												if (TileAt(i, j).FrameNumber == 2)
												{
													rectangle.X = 54;
													rectangle.Y = 36;
												}
											}
											else
											{
												if (num4 == num9 && num5 != num9)
												{
													if (TileAt(i, j).FrameNumber == 0)
													{
														rectangle.X = 72;
														rectangle.Y = 0;
													}
													if (TileAt(i, j).FrameNumber == 1)
													{
														rectangle.X = 72;
														rectangle.Y = 18;
													}
													if (TileAt(i, j).FrameNumber == 2)
													{
														rectangle.X = 72;
														rectangle.Y = 36;
													}
												}
												else
												{
													if (num4 != num9 && num4 != -1 && num5 == -1)
													{
														if (TileAt(i, j).FrameNumber == 0)
														{
															rectangle.X = 108;
															rectangle.Y = 0;
														}
														if (TileAt(i, j).FrameNumber == 1)
														{
															rectangle.X = 108;
															rectangle.Y = 18;
														}
														if (TileAt(i, j).FrameNumber == 2)
														{
															rectangle.X = 108;
															rectangle.Y = 36;
														}
													}
													else
													{
														if (num4 == -1 && num5 != num9 && num5 != -1)
														{
															if (TileAt(i, j).FrameNumber == 0)
															{
																rectangle.X = 126;
																rectangle.Y = 0;
															}
															if (TileAt(i, j).FrameNumber == 1)
															{
																rectangle.X = 126;
																rectangle.Y = 18;
															}
															if (TileAt(i, j).FrameNumber == 2)
															{
																rectangle.X = 126;
																rectangle.Y = 36;
															}
														}
														else
														{
															if (TileAt(i, j).FrameNumber == 0)
															{
																rectangle.X = 90;
																rectangle.Y = 0;
															}
															if (TileAt(i, j).FrameNumber == 1)
															{
																rectangle.X = 90;
																rectangle.Y = 18;
															}
															if (TileAt(i, j).FrameNumber == 2)
															{
																rectangle.X = 90;
																rectangle.Y = 36;
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
								if (num9 == 10)
								{
									if (!destroyObject)
									{
										int frameY2 = (int)TileAt(i, j).FrameY;
										int num17 = j;
										bool flag = false;
										if (frameY2 == 0)
										{
											num17 = j;
										}
										if (frameY2 == 18)
										{
											num17 = j - 1;
										}
										if (frameY2 == 36)
										{
											num17 = j - 2;
										}
										if (!TileAt(i, num17 - 1).Active || !Main.tileSolid[(int)TileAt(i, num17 - 1).Type])
										{
											flag = true;
										}
										if (!TileAt(i, num17 + 3).Active || !Main.tileSolid[(int)TileAt(i, num17 + 3).Type])
										{
											flag = true;
										}
										if (!TileAt(i, num17).Active || (int)TileAt(i, num17).Type != num9)
										{
											flag = true;
										}
										if (!TileAt(i, num17 + 1).Active || (int)TileAt(i, num17 + 1).Type != num9)
										{
											flag = true;
										}
										if (!TileAt(i, num17 + 2).Active || (int)TileAt(i, num17 + 2).Type != num9)
										{
											flag = true;
										}
										if (flag)
										{
											destroyObject = true;
											KillTile(i, num17, false, false, false);
											KillTile(i, num17 + 1, false, false, false);
											KillTile(i, num17 + 2, false, false, false);
											sandbox.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
										}
										destroyObject = false;
									}
									return;
								}
								if (num9 == 11)
								{
									if (!destroyObject)
									{
										int num18 = 0;
										int num19 = i;
										int num20 = j;
										int frameX2 = (int)TileAt(i, j).FrameX;
										int frameY3 = (int)TileAt(i, j).FrameY;
										bool flag2 = false;
										if (frameX2 == 0)
										{
											num19 = i;
											num18 = 1;
										}
										else
										{
											if (frameX2 == 18)
											{
												num19 = i - 1;
												num18 = 1;
											}
											else
											{
												if (frameX2 == 36)
												{
													num19 = i + 1;
													num18 = -1;
												}
												else
												{
													if (frameX2 == 54)
													{
														num19 = i;
														num18 = -1;
													}
												}
											}
										}
										if (frameY3 == 0)
										{
											num20 = j;
										}
										else
										{
											if (frameY3 == 18)
											{
												num20 = j - 1;
											}
											else
											{
												if (frameY3 == 36)
												{
													num20 = j - 2;
												}
											}
										}
										if (!TileAt(num19, num20 - 1).Active ||
											!Main.tileSolid[(int)TileAt(num19, num20 - 1).Type] ||
											!TileAt(num19, num20 + 3).Active ||
											!Main.tileSolid[(int)TileAt(num19, num20 + 3).Type])
										{
											flag2 = true;
											destroyObject = true;
											sandbox.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
										}
										int num21 = num19;
										if (num18 == -1)
										{
											num21 = num19 - 1;
										}
										for (int l = num21; l < num21 + 2; l++)
										{
											for (int m = num20; m < num20 + 3; m++)
											{
												if (!flag2 && (TileAt(l, m).Type != 11 || !TileAt(l, m).Active))
												{
													destroyObject = true;
													sandbox.NewItem(i * 16, j * 16, 16, 16, 25, 1, false);
													flag2 = true;
													l = num21;
													m = num20;
												}
												if (flag2)
												{
													KillTile(l, m, false, false, false);
												}
											}
										}
										destroyObject = false;
									}
									return;
								}
								if (num9 == 34 || num9 == 35 || num9 == 36 || num9 == 106)
								{
									Check3x3(i, j, (int)((byte)num9));
									return;
								}
								if (num9 == 15 || num9 == 20)
								{
									Check1x2(i, j, (byte)num9);
									return;
								}
								if (num9 == 14 || num9 == 17 || num9 == 26 || num9 == 77 || num9 == 86 || num9 == 87 || num9 == 88 || num9 == 89)
								{
									Check3x2(i, j, (int)((byte)num9));
									return;
								}
								if (num9 == 16 || num9 == 18 || num9 == 29 || num9 == 103)
								{
									Check2x1(i, j, (byte)num9);
									return;
								}
								if (num9 == 13 || num9 == 33 || num9 == 49 || num9 == 50 || num9 == 78)
								{
									CheckOnTable1x1(i, j, (int)((byte)num9));
									return;
								}
								if (num9 == 21)
								{
									CheckChest(i, j, (int)((byte)num9));
									return;
								}
								if (num9 == 27)
								{
									CheckSunflower(i, j, 27);
									return;
								}
								if (num9 == 28)
								{
									CheckPot(i, j, 28);
									return;
								}
								if (num9 == 91)
								{
									CheckBanner(i, j, (byte)num9);
									return;
								}
								if (num9 == 92 || num9 == 93)
								{
									Check1xX(i, j, (byte)num9);
									return;
								}
								if (num9 == 104 || num9 == 105)
								{
									Check2xX(i, j, (byte)num9);
								}
								else
								{
									if (num9 == 101 || num9 == 102)
									{
										Check3x4(i, j, (int)((byte)num9));
										return;
									}
									if (num9 == 42)
									{
										Check1x2Top(i, j, (byte)num9);
										return;
									}
									if (num9 == 55 || num9 == 85)
									{
										CheckSign(i, j, num9);
										return;
									}
									if (num9 == 79 || num9 == 90)
									{
										Check4x2(i, j, num9);
										return;
									}
									if (num9 == 85 || num9 == 94 || num9 == 95 || num9 == 96 || num9 == 97 || num9 == 98 || num9 == 99 || num9 == 100)
									{
										Check2x2(i, j, num9);
										return;
									}
									if (num9 == 81)
									{
										if (num4 != -1 || num2 != -1 || num5 != -1)
										{
											KillTile(i, j, false, false, false);
											return;
										}
										if (num7 < 0 || !Main.tileSolid[num7])
										{
											KillTile(i, j, false, false, false);
										}
										return;
									}
									else
									{
										if (Main.tileAlch[num9])
										{
											CheckAlch(i, j);
											return;
										}
										if (num9 == 72)
										{
											if (num7 != num9 && num7 != 70)
											{
												KillTile(i, j, false, false, false);
											}
											else
											{
												if (num2 != num9 && TileAt(i, j).FrameX == 0)
												{
													TileAt(i, j).SetFrameNumber((byte)genRand.Next(3));
													if (TileAt(i, j).FrameNumber == 0)
													{
														TileAt(i, j).SetFrameX(18);
														TileAt(i, j).SetFrameY(0);
													}
													if (TileAt(i, j).FrameNumber == 1)
													{
														TileAt(i, j).SetFrameX(18);
														TileAt(i, j).SetFrameY(18);
													}
													if (TileAt(i, j).FrameNumber == 2)
													{
														TileAt(i, j).SetFrameX(18);
														TileAt(i, j).SetFrameY(36);
													}
												}
											}
										}
										else
										{
											if (num9 == 5)
											{
												if (num7 == 23)
												{
													num7 = 2;
												}
												if (num7 == 60)
												{
													num7 = 2;
												}
												if (TileAt(i, j).FrameX >= 22 && TileAt(i, j).FrameX <= 44 && TileAt(i, j).FrameY >= 132 && TileAt(i, j).FrameY <= 176)
												{
													if ((num4 != num9 && num5 != num9) || num7 != 2)
													{
														KillTile(i, j, false, false, false);
													}
												}
												else
												{
													if ((TileAt(i, j).FrameX == 88 && TileAt(i, j).FrameY >= 0 && TileAt(i, j).FrameY <= 44) || (TileAt(i, j).FrameX == 66 && TileAt(i, j).FrameY >= 66 && TileAt(i, j).FrameY <= 130) || (TileAt(i, j).FrameX == 110 && TileAt(i, j).FrameY >= 66 && TileAt(i, j).FrameY <= 110) || (TileAt(i, j).FrameX == 132 && TileAt(i, j).FrameY >= 0 && TileAt(i, j).FrameY <= 176))
													{
														if (num4 == num9 && num5 == num9)
														{
															if (TileAt(i, j).FrameNumber == 0)
															{
																TileAt(i, j).SetFrameX(110);
																TileAt(i, j).SetFrameY(66);
															}
															if (TileAt(i, j).FrameNumber == 1)
															{
																TileAt(i, j).SetFrameX(110);
																TileAt(i, j).SetFrameY(88);
															}
															if (TileAt(i, j).FrameNumber == 2)
															{
																TileAt(i, j).SetFrameX(110);
																TileAt(i, j).SetFrameY(110);
															}
														}
														else
														{
															if (num4 == num9)
															{
																if (TileAt(i, j).FrameNumber == 0)
																{
																	TileAt(i, j).SetFrameX(88);
																	TileAt(i, j).SetFrameY(0);
																}
																if (TileAt(i, j).FrameNumber == 1)
																{
																	TileAt(i, j).SetFrameX(88);
																	TileAt(i, j).SetFrameY(22);
																}
																if (TileAt(i, j).FrameNumber == 2)
																{
																	TileAt(i, j).SetFrameX(88);
																	TileAt(i, j).SetFrameY(44);
																}
															}
															else
															{
																if (num5 == num9)
																{
																	if (TileAt(i, j).FrameNumber == 0)
																	{
																		TileAt(i, j).SetFrameX(66);
																		TileAt(i, j).SetFrameY(66);
																	}
																	if (TileAt(i, j).FrameNumber == 1)
																	{
																		TileAt(i, j).SetFrameX(66);
																		TileAt(i, j).SetFrameY(88);
																	}
																	if (TileAt(i, j).FrameNumber == 2)
																	{
																		TileAt(i, j).SetFrameX(66);
																		TileAt(i, j).SetFrameY(110);
																	}
																}
																else
																{
																	if (TileAt(i, j).FrameNumber == 0)
																	{
																		TileAt(i, j).SetFrameX(0);
																		TileAt(i, j).SetFrameY(0);
																	}
																	if (TileAt(i, j).FrameNumber == 1)
																	{
																		TileAt(i, j).SetFrameX(0);
																		TileAt(i, j).SetFrameY(22);
																	}
																	if (TileAt(i, j).FrameNumber == 2)
																	{
																		TileAt(i, j).SetFrameX(0);
																		TileAt(i, j).SetFrameY(44);
																	}
																}
															}
														}
													}
												}
												if (TileAt(i, j).FrameY >= 132 && TileAt(i, j).FrameY <= 176 && (TileAt(i, j).FrameX == 0 || TileAt(i, j).FrameX == 66 || TileAt(i, j).FrameX == 88))
												{
													if (num7 != 2)
													{
														KillTile(i, j, false, false, false);
													}
													if (num4 != num9 && num5 != num9)
													{
														if (TileAt(i, j).FrameNumber == 0)
														{
															TileAt(i, j).SetFrameX(0);
															TileAt(i, j).SetFrameY(0);
														}
														if (TileAt(i, j).FrameNumber == 1)
														{
															TileAt(i, j).SetFrameX(0);
															TileAt(i, j).SetFrameY(22);
														}
														if (TileAt(i, j).FrameNumber == 2)
														{
															TileAt(i, j).SetFrameX(0);
															TileAt(i, j).SetFrameY(44);
														}
													}
													else
													{
														if (num4 != num9)
														{
															if (TileAt(i, j).FrameNumber == 0)
															{
																TileAt(i, j).SetFrameX(0);
																TileAt(i, j).SetFrameY(132);
															}
															if (TileAt(i, j).FrameNumber == 1)
															{
																TileAt(i, j).SetFrameX(0);
																TileAt(i, j).SetFrameY(154);
															}
															if (TileAt(i, j).FrameNumber == 2)
															{
																TileAt(i, j).SetFrameX(0);
																TileAt(i, j).SetFrameY(176);
															}
														}
														else
														{
															if (num5 != num9)
															{
																if (TileAt(i, j).FrameNumber == 0)
																{
																	TileAt(i, j).SetFrameX(66);
																	TileAt(i, j).SetFrameY(132);
																}
																if (TileAt(i, j).FrameNumber == 1)
																{
																	TileAt(i, j).SetFrameX(66);
																	TileAt(i, j).SetFrameY(154);
																}
																if (TileAt(i, j).FrameNumber == 2)
																{
																	TileAt(i, j).SetFrameX(66);
																	TileAt(i, j).SetFrameY(176);
																}
															}
															else
															{
																if (TileAt(i, j).FrameNumber == 0)
																{
																	TileAt(i, j).SetFrameX(88);
																	TileAt(i, j).SetFrameY(132);
																}
																if (TileAt(i, j).FrameNumber == 1)
																{
																	TileAt(i, j).SetFrameX(88);
																	TileAt(i, j).SetFrameY(154);
																}
																if (TileAt(i, j).FrameNumber == 2)
																{
																	TileAt(i, j).SetFrameX(88);
																	TileAt(i, j).SetFrameY(176);
																}
															}
														}
													}
												}
												if ((TileAt(i, j).FrameX == 66 && (TileAt(i, j).FrameY == 0 || TileAt(i, j).FrameY == 22 || TileAt(i, j).FrameY == 44)) || (TileAt(i, j).FrameX == 88 && (TileAt(i, j).FrameY == 66 || TileAt(i, j).FrameY == 88 || TileAt(i, j).FrameY == 110)) || (TileAt(i, j).FrameX == 44 && (TileAt(i, j).FrameY == 198 || TileAt(i, j).FrameY == 220 || TileAt(i, j).FrameY == 242)) || (TileAt(i, j).FrameX == 66 && (TileAt(i, j).FrameY == 198 || TileAt(i, j).FrameY == 220 || TileAt(i, j).FrameY == 242)))
												{
													if (num4 != num9 && num5 != num9)
													{
														KillTile(i, j, false, false, false);
													}
												}
												else
												{
													if (num7 == -1 || num7 == 23)
													{
														KillTile(i, j, false, false, false);
													}
													else
													{
														if (num2 != num9 && TileAt(i, j).FrameY < 198 && ((TileAt(i, j).FrameX != 22 && TileAt(i, j).FrameX != 44) || TileAt(i, j).FrameY < 132))
														{
															if (num4 == num9 || num5 == num9)
															{
																if (num7 == num9)
																{
																	if (num4 == num9 && num5 == num9)
																	{
																		if (TileAt(i, j).FrameNumber == 0)
																		{
																			TileAt(i, j).SetFrameX(132);
																			TileAt(i, j).SetFrameY(132);
																		}
																		if (TileAt(i, j).FrameNumber == 1)
																		{
																			TileAt(i, j).SetFrameX(132);
																			TileAt(i, j).SetFrameY(154);
																		}
																		if (TileAt(i, j).FrameNumber == 2)
																		{
																			TileAt(i, j).SetFrameX(132);
																			TileAt(i, j).SetFrameY(176);
																		}
																	}
																	else
																	{
																		if (num4 == num9)
																		{
																			if (TileAt(i, j).FrameNumber == 0)
																			{
																				TileAt(i, j).SetFrameX(132);
																				TileAt(i, j).SetFrameY(0);
																			}
																			if (TileAt(i, j).FrameNumber == 1)
																			{
																				TileAt(i, j).SetFrameX(132);
																				TileAt(i, j).SetFrameY(22);
																			}
																			if (TileAt(i, j).FrameNumber == 2)
																			{
																				TileAt(i, j).SetFrameX(132);
																				TileAt(i, j).SetFrameY(44);
																			}
																		}
																		else
																		{
																			if (num5 == num9)
																			{
																				if (TileAt(i, j).FrameNumber == 0)
																				{
																					TileAt(i, j).SetFrameX(132);
																					TileAt(i, j).SetFrameY(66);
																				}
																				if (TileAt(i, j).FrameNumber == 1)
																				{
																					TileAt(i, j).SetFrameX(132);
																					TileAt(i, j).SetFrameY(88);
																				}
																				if (TileAt(i, j).FrameNumber == 2)
																				{
																					TileAt(i, j).SetFrameX(132);
																					TileAt(i, j).SetFrameY(110);
																				}
																			}
																		}
																	}
																}
																else
																{
																	if (num4 == num9 && num5 == num9)
																	{
																		if (TileAt(i, j).FrameNumber == 0)
																		{
																			TileAt(i, j).SetFrameX(154);
																			TileAt(i, j).SetFrameY(132);
																		}
																		if (TileAt(i, j).FrameNumber == 1)
																		{
																			TileAt(i, j).SetFrameX(154);
																			TileAt(i, j).SetFrameY(154);
																		}
																		if (TileAt(i, j).FrameNumber == 2)
																		{
																			TileAt(i, j).SetFrameX(154);
																			TileAt(i, j).SetFrameY(176);
																		}
																	}
																	else
																	{
																		if (num4 == num9)
																		{
																			if (TileAt(i, j).FrameNumber == 0)
																			{
																				TileAt(i, j).SetFrameX(154);
																				TileAt(i, j).SetFrameY(0);
																			}
																			if (TileAt(i, j).FrameNumber == 1)
																			{
																				TileAt(i, j).SetFrameX(154);
																				TileAt(i, j).SetFrameY(22);
																			}
																			if (TileAt(i, j).FrameNumber == 2)
																			{
																				TileAt(i, j).SetFrameX(154);
																				TileAt(i, j).SetFrameY(44);
																			}
																		}
																		else
																		{
																			if (num5 == num9)
																			{
																				if (TileAt(i, j).FrameNumber == 0)
																				{
																					TileAt(i, j).SetFrameX(154);
																					TileAt(i, j).SetFrameY(66);
																				}
																				if (TileAt(i, j).FrameNumber == 1)
																				{
																					TileAt(i, j).SetFrameX(154);
																					TileAt(i, j).SetFrameY(88);
																				}
																				if (TileAt(i, j).FrameNumber == 2)
																				{
																					TileAt(i, j).SetFrameX(154);
																					TileAt(i, j).SetFrameY(110);
																				}
																			}
																		}
																	}
																}
															}
															else
															{
																if (TileAt(i, j).FrameNumber == 0)
																{
																	TileAt(i, j).SetFrameX(110);
																	TileAt(i, j).SetFrameY(0);
																}
																if (TileAt(i, j).FrameNumber == 1)
																{
																	TileAt(i, j).SetFrameX(110);
																	TileAt(i, j).SetFrameY(22);
																}
																if (TileAt(i, j).FrameNumber == 2)
																{
																	TileAt(i, j).SetFrameX(110);
																	TileAt(i, j).SetFrameY(44);
																}
															}
														}
													}
												}
												rectangle.X = (int)TileAt(i, j).FrameX;
												rectangle.Y = (int)TileAt(i, j).FrameY;
											}
										}
									}
								}
							}
						}
					}
					if (Main.tileFrameImportant[(int)TileAt(i, j).Type])
					{
						return;
					}
					int num22 = 0;
					if (resetFrame)
					{
						num22 = genRand.Next(0, 3);
						TileAt(i, j).SetFrameNumber((byte)num22);
					}
					else
					{
						num22 = (int)TileAt(i, j).FrameNumber;
					}
					if (num9 == 0)
					{
						if (num2 >= 0 && Main.tileMergeDirt[num2])
						{
							TileFrame(i, j - 1, false, false);
							if (mergeDown)
							{
								num2 = num9;
							}
						}
						if (num7 >= 0 && Main.tileMergeDirt[num7])
						{
							TileFrame(i, j + 1, false, false);
							if (mergeUp)
							{
								num7 = num9;
							}
						}
						if (num4 >= 0 && Main.tileMergeDirt[num4])
						{
							TileFrame(i - 1, j, false, false);
							if (mergeRight)
							{
								num4 = num9;
							}
						}
						if (num5 >= 0 && Main.tileMergeDirt[num5])
						{
							TileFrame(i + 1, j, false, false);
							if (mergeLeft)
							{
								num5 = num9;
							}
						}
						if (num >= 0 && Main.tileMergeDirt[num])
						{
							num = num9;
						}
						if (num3 >= 0 && Main.tileMergeDirt[num3])
						{
							num3 = num9;
						}
						if (num6 >= 0 && Main.tileMergeDirt[num6])
						{
							num6 = num9;
						}
						if (num8 >= 0 && Main.tileMergeDirt[num8])
						{
							num8 = num9;
						}
						if (num2 == 2)
						{
							num2 = num9;
						}
						if (num7 == 2)
						{
							num7 = num9;
						}
						if (num4 == 2)
						{
							num4 = num9;
						}
						if (num5 == 2)
						{
							num5 = num9;
						}
						if (num == 2)
						{
							num = num9;
						}
						if (num3 == 2)
						{
							num3 = num9;
						}
						if (num6 == 2)
						{
							num6 = num9;
						}
						if (num8 == 2)
						{
							num8 = num9;
						}
						if (num2 == 23)
						{
							num2 = num9;
						}
						if (num7 == 23)
						{
							num7 = num9;
						}
						if (num4 == 23)
						{
							num4 = num9;
						}
						if (num5 == 23)
						{
							num5 = num9;
						}
						if (num == 23)
						{
							num = num9;
						}
						if (num3 == 23)
						{
							num3 = num9;
						}
						if (num6 == 23)
						{
							num6 = num9;
						}
						if (num8 == 23)
						{
							num8 = num9;
						}
					}
					else
					{
						if (num9 == 57)
						{
							if (num2 == 58)
							{
								TileFrame(i, j - 1, false, false);
								if (mergeDown)
								{
									num2 = num9;
								}
							}
							if (num7 == 58)
							{
								TileFrame(i, j + 1, false, false);
								if (mergeUp)
								{
									num7 = num9;
								}
							}
							if (num4 == 58)
							{
								TileFrame(i - 1, j, false, false);
								if (mergeRight)
								{
									num4 = num9;
								}
							}
							if (num5 == 58)
							{
								TileFrame(i + 1, j, false, false);
								if (mergeLeft)
								{
									num5 = num9;
								}
							}
							if (num == 58)
							{
								num = num9;
							}
							if (num3 == 58)
							{
								num3 = num9;
							}
							if (num6 == 58)
							{
								num6 = num9;
							}
							if (num8 == 58)
							{
								num8 = num9;
							}
						}
						else
						{
							if (num9 == 59)
							{
								if (num2 == 60)
								{
									num2 = num9;
								}
								if (num7 == 60)
								{
									num7 = num9;
								}
								if (num4 == 60)
								{
									num4 = num9;
								}
								if (num5 == 60)
								{
									num5 = num9;
								}
								if (num == 60)
								{
									num = num9;
								}
								if (num3 == 60)
								{
									num3 = num9;
								}
								if (num6 == 60)
								{
									num6 = num9;
								}
								if (num8 == 60)
								{
									num8 = num9;
								}
								if (num2 == 70)
								{
									num2 = num9;
								}
								if (num7 == 70)
								{
									num7 = num9;
								}
								if (num4 == 70)
								{
									num4 = num9;
								}
								if (num5 == 70)
								{
									num5 = num9;
								}
								if (num == 70)
								{
									num = num9;
								}
								if (num3 == 70)
								{
									num3 = num9;
								}
								if (num6 == 70)
								{
									num6 = num9;
								}
								if (num8 == 70)
								{
									num8 = num9;
								}
							}
						}
					}
					if (Main.tileMergeDirt[num9])
					{
						if (num2 == 0)
						{
							num2 = -2;
						}
						if (num7 == 0)
						{
							num7 = -2;
						}
						if (num4 == 0)
						{
							num4 = -2;
						}
						if (num5 == 0)
						{
							num5 = -2;
						}
						if (num == 0)
						{
							num = -2;
						}
						if (num3 == 0)
						{
							num3 = -2;
						}
						if (num6 == 0)
						{
							num6 = -2;
						}
						if (num8 == 0)
						{
							num8 = -2;
						}
					}
					else
					{
						if (num9 == 58)
						{
							if (num2 == 57)
							{
								num2 = -2;
							}
							if (num7 == 57)
							{
								num7 = -2;
							}
							if (num4 == 57)
							{
								num4 = -2;
							}
							if (num5 == 57)
							{
								num5 = -2;
							}
							if (num == 57)
							{
								num = -2;
							}
							if (num3 == 57)
							{
								num3 = -2;
							}
							if (num6 == 57)
							{
								num6 = -2;
							}
							if (num8 == 57)
							{
								num8 = -2;
							}
						}
						else
						{
							if (num9 == 59)
							{
								if (num2 == 1)
								{
									num2 = -2;
								}
								if (num7 == 1)
								{
									num7 = -2;
								}
								if (num4 == 1)
								{
									num4 = -2;
								}
								if (num5 == 1)
								{
									num5 = -2;
								}
								if (num == 1)
								{
									num = -2;
								}
								if (num3 == 1)
								{
									num3 = -2;
								}
								if (num6 == 1)
								{
									num6 = -2;
								}
								if (num8 == 1)
								{
									num8 = -2;
								}
							}
						}
					}
					if (num9 == 32)
					{
						if (num7 == 23)
						{
							num7 = num9;
						}
					}
					else
					{
						if (num9 == 69)
						{
							if (num7 == 60)
							{
								num7 = num9;
							}
						}
						else
						{
							if (num9 == 51)
							{
								if (num2 > -1 && !Main.tileNoAttach[num2])
								{
									num2 = num9;
								}
								if (num7 > -1 && !Main.tileNoAttach[num7])
								{
									num7 = num9;
								}
								if (num4 > -1 && !Main.tileNoAttach[num4])
								{
									num4 = num9;
								}
								if (num5 > -1 && !Main.tileNoAttach[num5])
								{
									num5 = num9;
								}
								if (num > -1 && !Main.tileNoAttach[num])
								{
									num = num9;
								}
								if (num3 > -1 && !Main.tileNoAttach[num3])
								{
									num3 = num9;
								}
								if (num6 > -1 && !Main.tileNoAttach[num6])
								{
									num6 = num9;
								}
								if (num8 > -1 && !Main.tileNoAttach[num8])
								{
									num8 = num9;
								}
							}
						}
					}
					if (num2 > -1 && !Main.tileSolid[num2] && num2 != num9)
					{
						num2 = -1;
					}
					if (num7 > -1 && !Main.tileSolid[num7] && num7 != num9)
					{
						num7 = -1;
					}
					if (num4 > -1 && !Main.tileSolid[num4] && num4 != num9)
					{
						num4 = -1;
					}
					if (num5 > -1 && !Main.tileSolid[num5] && num5 != num9)
					{
						num5 = -1;
					}
					if (num > -1 && !Main.tileSolid[num] && num != num9)
					{
						num = -1;
					}
					if (num3 > -1 && !Main.tileSolid[num3] && num3 != num9)
					{
						num3 = -1;
					}
					if (num6 > -1 && !Main.tileSolid[num6] && num6 != num9)
					{
						num6 = -1;
					}
					if (num8 > -1 && !Main.tileSolid[num8] && num8 != num9)
					{
						num8 = -1;
					}
					if (num9 == 2 || num9 == 23 || num9 == 60 || num9 == 70)
					{
						int num23 = 0;
						if (num9 == 60 || num9 == 70)
						{
							num23 = 59;
						}
						else
						{
							if (num9 == 2)
							{
								if (num2 == 23)
								{
									num2 = num23;
								}
								if (num7 == 23)
								{
									num7 = num23;
								}
								if (num4 == 23)
								{
									num4 = num23;
								}
								if (num5 == 23)
								{
									num5 = num23;
								}
								if (num == 23)
								{
									num = num23;
								}
								if (num3 == 23)
								{
									num3 = num23;
								}
								if (num6 == 23)
								{
									num6 = num23;
								}
								if (num8 == 23)
								{
									num8 = num23;
								}
							}
							else
							{
								if (num9 == 23)
								{
									if (num2 == 2)
									{
										num2 = num23;
									}
									if (num7 == 2)
									{
										num7 = num23;
									}
									if (num4 == 2)
									{
										num4 = num23;
									}
									if (num5 == 2)
									{
										num5 = num23;
									}
									if (num == 2)
									{
										num = num23;
									}
									if (num3 == 2)
									{
										num3 = num23;
									}
									if (num6 == 2)
									{
										num6 = num23;
									}
									if (num8 == 2)
									{
										num8 = num23;
									}
								}
							}
						}
						if (num2 != num9 && num2 != num23 && (num7 == num9 || num7 == num23))
						{
							if (num4 == num23 && num5 == num9)
							{
								if (num22 == 0)
								{
									rectangle.X = 0;
									rectangle.Y = 198;
								}
								if (num22 == 1)
								{
									rectangle.X = 18;
									rectangle.Y = 198;
								}
								if (num22 == 2)
								{
									rectangle.X = 36;
									rectangle.Y = 198;
								}
							}
							else
							{
								if (num4 == num9 && num5 == num23)
								{
									if (num22 == 0)
									{
										rectangle.X = 54;
										rectangle.Y = 198;
									}
									if (num22 == 1)
									{
										rectangle.X = 72;
										rectangle.Y = 198;
									}
									if (num22 == 2)
									{
										rectangle.X = 90;
										rectangle.Y = 198;
									}
								}
							}
						}
						else
						{
							if (num7 != num9 && num7 != num23 && (num2 == num9 || num2 == num23))
							{
								if (num4 == num23 && num5 == num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 0;
										rectangle.Y = 216;
									}
									if (num22 == 1)
									{
										rectangle.X = 18;
										rectangle.Y = 216;
									}
									if (num22 == 2)
									{
										rectangle.X = 36;
										rectangle.Y = 216;
									}
								}
								else
								{
									if (num4 == num9 && num5 == num23)
									{
										if (num22 == 0)
										{
											rectangle.X = 54;
											rectangle.Y = 216;
										}
										if (num22 == 1)
										{
											rectangle.X = 72;
											rectangle.Y = 216;
										}
										if (num22 == 2)
										{
											rectangle.X = 90;
											rectangle.Y = 216;
										}
									}
								}
							}
							else
							{
								if (num4 != num9 && num4 != num23 && (num5 == num9 || num5 == num23))
								{
									if (num2 == num23 && num7 == num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 72;
											rectangle.Y = 144;
										}
										if (num22 == 1)
										{
											rectangle.X = 72;
											rectangle.Y = 162;
										}
										if (num22 == 2)
										{
											rectangle.X = 72;
											rectangle.Y = 180;
										}
									}
									else
									{
										if (num7 == num9 && num5 == num2)
										{
											if (num22 == 0)
											{
												rectangle.X = 72;
												rectangle.Y = 90;
											}
											if (num22 == 1)
											{
												rectangle.X = 72;
												rectangle.Y = 108;
											}
											if (num22 == 2)
											{
												rectangle.X = 72;
												rectangle.Y = 126;
											}
										}
									}
								}
								else
								{
									if (num5 != num9 && num5 != num23 && (num4 == num9 || num4 == num23))
									{
										if (num2 == num23 && num7 == num9)
										{
											if (num22 == 0)
											{
												rectangle.X = 90;
												rectangle.Y = 144;
											}
											if (num22 == 1)
											{
												rectangle.X = 90;
												rectangle.Y = 162;
											}
											if (num22 == 2)
											{
												rectangle.X = 90;
												rectangle.Y = 180;
											}
										}
										else
										{
											if (num7 == num9 && num5 == num2)
											{
												if (num22 == 0)
												{
													rectangle.X = 90;
													rectangle.Y = 90;
												}
												if (num22 == 1)
												{
													rectangle.X = 90;
													rectangle.Y = 108;
												}
												if (num22 == 2)
												{
													rectangle.X = 90;
													rectangle.Y = 126;
												}
											}
										}
									}
									else
									{
										if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num9)
										{
											if (num != num9 && num3 != num9 && num6 != num9 && num8 != num9)
											{
												if (num8 == num23)
												{
													if (num22 == 0)
													{
														rectangle.X = 108;
														rectangle.Y = 324;
													}
													if (num22 == 1)
													{
														rectangle.X = 126;
														rectangle.Y = 324;
													}
													if (num22 == 2)
													{
														rectangle.X = 144;
														rectangle.Y = 324;
													}
												}
												else
												{
													if (num3 == num23)
													{
														if (num22 == 0)
														{
															rectangle.X = 108;
															rectangle.Y = 342;
														}
														if (num22 == 1)
														{
															rectangle.X = 126;
															rectangle.Y = 342;
														}
														if (num22 == 2)
														{
															rectangle.X = 144;
															rectangle.Y = 342;
														}
													}
													else
													{
														if (num6 == num23)
														{
															if (num22 == 0)
															{
																rectangle.X = 108;
																rectangle.Y = 360;
															}
															if (num22 == 1)
															{
																rectangle.X = 126;
																rectangle.Y = 360;
															}
															if (num22 == 2)
															{
																rectangle.X = 144;
																rectangle.Y = 360;
															}
														}
														else
														{
															if (num == num23)
															{
																if (num22 == 0)
																{
																	rectangle.X = 108;
																	rectangle.Y = 378;
																}
																if (num22 == 1)
																{
																	rectangle.X = 126;
																	rectangle.Y = 378;
																}
																if (num22 == 2)
																{
																	rectangle.X = 144;
																	rectangle.Y = 378;
																}
															}
															else
															{
																if (num22 == 0)
																{
																	rectangle.X = 144;
																	rectangle.Y = 234;
																}
																if (num22 == 1)
																{
																	rectangle.X = 198;
																	rectangle.Y = 234;
																}
																if (num22 == 2)
																{
																	rectangle.X = 252;
																	rectangle.Y = 234;
																}
															}
														}
													}
												}
											}
											else
											{
												if (num != num9 && num8 != num9)
												{
													if (num22 == 0)
													{
														rectangle.X = 36;
														rectangle.Y = 306;
													}
													if (num22 == 1)
													{
														rectangle.X = 54;
														rectangle.Y = 306;
													}
													if (num22 == 2)
													{
														rectangle.X = 72;
														rectangle.Y = 306;
													}
												}
												else
												{
													if (num3 != num9 && num6 != num9)
													{
														if (num22 == 0)
														{
															rectangle.X = 90;
															rectangle.Y = 306;
														}
														if (num22 == 1)
														{
															rectangle.X = 108;
															rectangle.Y = 306;
														}
														if (num22 == 2)
														{
															rectangle.X = 126;
															rectangle.Y = 306;
														}
													}
													else
													{
														if (num != num9 && num3 == num9 && num6 == num9 && num8 == num9)
														{
															if (num22 == 0)
															{
																rectangle.X = 54;
																rectangle.Y = 108;
															}
															if (num22 == 1)
															{
																rectangle.X = 54;
																rectangle.Y = 144;
															}
															if (num22 == 2)
															{
																rectangle.X = 54;
																rectangle.Y = 180;
															}
														}
														else
														{
															if (num == num9 && num3 != num9 && num6 == num9 && num8 == num9)
															{
																if (num22 == 0)
																{
																	rectangle.X = 36;
																	rectangle.Y = 108;
																}
																if (num22 == 1)
																{
																	rectangle.X = 36;
																	rectangle.Y = 144;
																}
																if (num22 == 2)
																{
																	rectangle.X = 36;
																	rectangle.Y = 180;
																}
															}
															else
															{
																if (num == num9 && num3 == num9 && num6 != num9 && num8 == num9)
																{
																	if (num22 == 0)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 90;
																	}
																	if (num22 == 1)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 126;
																	}
																	if (num22 == 2)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 162;
																	}
																}
																else
																{
																	if (num == num9 && num3 == num9 && num6 == num9 && num8 != num9)
																	{
																		if (num22 == 0)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 90;
																		}
																		if (num22 == 1)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 126;
																		}
																		if (num22 == 2)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 162;
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
											if (num2 == num9 && num7 == num23 && num4 == num9 && num5 == num9 && num == -1 && num3 == -1)
											{
												if (num22 == 0)
												{
													rectangle.X = 108;
													rectangle.Y = 18;
												}
												if (num22 == 1)
												{
													rectangle.X = 126;
													rectangle.Y = 18;
												}
												if (num22 == 2)
												{
													rectangle.X = 144;
													rectangle.Y = 18;
												}
											}
											else
											{
												if (num2 == num23 && num7 == num9 && num4 == num9 && num5 == num9 && num6 == -1 && num8 == -1)
												{
													if (num22 == 0)
													{
														rectangle.X = 108;
														rectangle.Y = 36;
													}
													if (num22 == 1)
													{
														rectangle.X = 126;
														rectangle.Y = 36;
													}
													if (num22 == 2)
													{
														rectangle.X = 144;
														rectangle.Y = 36;
													}
												}
												else
												{
													if (num2 == num9 && num7 == num9 && num4 == num23 && num5 == num9 && num3 == -1 && num8 == -1)
													{
														if (num22 == 0)
														{
															rectangle.X = 198;
															rectangle.Y = 0;
														}
														if (num22 == 1)
														{
															rectangle.X = 198;
															rectangle.Y = 18;
														}
														if (num22 == 2)
														{
															rectangle.X = 198;
															rectangle.Y = 36;
														}
													}
													else
													{
														if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num23 && num == -1 && num6 == -1)
														{
															if (num22 == 0)
															{
																rectangle.X = 180;
																rectangle.Y = 0;
															}
															if (num22 == 1)
															{
																rectangle.X = 180;
																rectangle.Y = 18;
															}
															if (num22 == 2)
															{
																rectangle.X = 180;
																rectangle.Y = 36;
															}
														}
														else
														{
															if (num2 == num9 && num7 == num23 && num4 == num9 && num5 == num9)
															{
																if (num3 != -1)
																{
																	if (num22 == 0)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 108;
																	}
																	if (num22 == 1)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 144;
																	}
																	if (num22 == 2)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 180;
																	}
																}
																else
																{
																	if (num != -1)
																	{
																		if (num22 == 0)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 108;
																		}
																		if (num22 == 1)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 144;
																		}
																		if (num22 == 2)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 180;
																		}
																	}
																}
															}
															else
															{
																if (num2 == num23 && num7 == num9 && num4 == num9 && num5 == num9)
																{
																	if (num8 != -1)
																	{
																		if (num22 == 0)
																		{
																			rectangle.X = 54;
																			rectangle.Y = 90;
																		}
																		if (num22 == 1)
																		{
																			rectangle.X = 54;
																			rectangle.Y = 126;
																		}
																		if (num22 == 2)
																		{
																			rectangle.X = 54;
																			rectangle.Y = 162;
																		}
																	}
																	else
																	{
																		if (num6 != -1)
																		{
																			if (num22 == 0)
																			{
																				rectangle.X = 36;
																				rectangle.Y = 90;
																			}
																			if (num22 == 1)
																			{
																				rectangle.X = 36;
																				rectangle.Y = 126;
																			}
																			if (num22 == 2)
																			{
																				rectangle.X = 36;
																				rectangle.Y = 162;
																			}
																		}
																	}
																}
																else
																{
																	if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num23)
																	{
																		if (num != -1)
																		{
																			if (num22 == 0)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 90;
																			}
																			if (num22 == 1)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 126;
																			}
																			if (num22 == 2)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 162;
																			}
																		}
																		else
																		{
																			if (num6 != -1)
																			{
																				if (num22 == 0)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 108;
																				}
																				if (num22 == 1)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 144;
																				}
																				if (num22 == 2)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 180;
																				}
																			}
																		}
																	}
																	else
																	{
																		if (num2 == num9 && num7 == num9 && num4 == num23 && num5 == num9)
																		{
																			if (num3 != -1)
																			{
																				if (num22 == 0)
																				{
																					rectangle.X = 36;
																					rectangle.Y = 90;
																				}
																				if (num22 == 1)
																				{
																					rectangle.X = 36;
																					rectangle.Y = 126;
																				}
																				if (num22 == 2)
																				{
																					rectangle.X = 36;
																					rectangle.Y = 162;
																				}
																			}
																			else
																			{
																				if (num8 != -1)
																				{
																					if (num22 == 0)
																					{
																						rectangle.X = 36;
																						rectangle.Y = 108;
																					}
																					if (num22 == 1)
																					{
																						rectangle.X = 36;
																						rectangle.Y = 144;
																					}
																					if (num22 == 2)
																					{
																						rectangle.X = 36;
																						rectangle.Y = 180;
																					}
																				}
																			}
																		}
																		else
																		{
																			if ((num2 == num23 && num7 == num9 && num4 == num9 && num5 == num9) || (num2 == num9 && num7 == num23 && num4 == num9 && num5 == num9) || (num2 == num9 && num7 == num9 && num4 == num23 && num5 == num9) || (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num23))
																			{
																				if (num22 == 0)
																				{
																					rectangle.X = 18;
																					rectangle.Y = 18;
																				}
																				if (num22 == 1)
																				{
																					rectangle.X = 36;
																					rectangle.Y = 18;
																				}
																				if (num22 == 2)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 18;
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
						if ((num2 == num9 || num2 == num23) && (num7 == num9 || num7 == num23) && (num4 == num9 || num4 == num23) && (num5 == num9 || num5 == num23))
						{
							if (num != num9 && num != num23 && (num3 == num9 || num3 == num23) && (num6 == num9 || num6 == num23) && (num8 == num9 || num8 == num23))
							{
								if (num22 == 0)
								{
									rectangle.X = 54;
									rectangle.Y = 108;
								}
								if (num22 == 1)
								{
									rectangle.X = 54;
									rectangle.Y = 144;
								}
								if (num22 == 2)
								{
									rectangle.X = 54;
									rectangle.Y = 180;
								}
							}
							else
							{
								if (num3 != num9 && num3 != num23 && (num == num9 || num == num23) && (num6 == num9 || num6 == num23) && (num8 == num9 || num8 == num23))
								{
									if (num22 == 0)
									{
										rectangle.X = 36;
										rectangle.Y = 108;
									}
									if (num22 == 1)
									{
										rectangle.X = 36;
										rectangle.Y = 144;
									}
									if (num22 == 2)
									{
										rectangle.X = 36;
										rectangle.Y = 180;
									}
								}
								else
								{
									if (num6 != num9 && num6 != num23 && (num == num9 || num == num23) && (num3 == num9 || num3 == num23) && (num8 == num9 || num8 == num23))
									{
										if (num22 == 0)
										{
											rectangle.X = 54;
											rectangle.Y = 90;
										}
										if (num22 == 1)
										{
											rectangle.X = 54;
											rectangle.Y = 126;
										}
										if (num22 == 2)
										{
											rectangle.X = 54;
											rectangle.Y = 162;
										}
									}
									else
									{
										if (num8 != num9 && num8 != num23 && (num == num9 || num == num23) && (num6 == num9 || num6 == num23) && (num3 == num9 || num3 == num23))
										{
											if (num22 == 0)
											{
												rectangle.X = 36;
												rectangle.Y = 90;
											}
											if (num22 == 1)
											{
												rectangle.X = 36;
												rectangle.Y = 126;
											}
											if (num22 == 2)
											{
												rectangle.X = 36;
												rectangle.Y = 162;
											}
										}
									}
								}
							}
						}
						if (num2 != num23 && num2 != num9 && num7 == num9 && num4 != num23 && num4 != num9 && num5 == num9 && num8 != num23 && num8 != num9)
						{
							if (num22 == 0)
							{
								rectangle.X = 90;
								rectangle.Y = 270;
							}
							if (num22 == 1)
							{
								rectangle.X = 108;
								rectangle.Y = 270;
							}
							if (num22 == 2)
							{
								rectangle.X = 126;
								rectangle.Y = 270;
							}
						}
						else
						{
							if (num2 != num23 && num2 != num9 && num7 == num9 && num4 == num9 && num5 != num23 && num5 != num9 && num6 != num23 && num6 != num9)
							{
								if (num22 == 0)
								{
									rectangle.X = 144;
									rectangle.Y = 270;
								}
								if (num22 == 1)
								{
									rectangle.X = 162;
									rectangle.Y = 270;
								}
								if (num22 == 2)
								{
									rectangle.X = 180;
									rectangle.Y = 270;
								}
							}
							else
							{
								if (num7 != num23 && num7 != num9 && num2 == num9 && num4 != num23 && num4 != num9 && num5 == num9 && num3 != num23 && num3 != num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 90;
										rectangle.Y = 288;
									}
									if (num22 == 1)
									{
										rectangle.X = 108;
										rectangle.Y = 288;
									}
									if (num22 == 2)
									{
										rectangle.X = 126;
										rectangle.Y = 288;
									}
								}
								else
								{
									if (num7 != num23 && num7 != num9 && num2 == num9 && num4 == num9 && num5 != num23 && num5 != num9 && num != num23 && num != num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 144;
											rectangle.Y = 288;
										}
										if (num22 == 1)
										{
											rectangle.X = 162;
											rectangle.Y = 288;
										}
										if (num22 == 2)
										{
											rectangle.X = 180;
											rectangle.Y = 288;
										}
									}
									else
									{
										if (num2 != num9 && num2 != num23 && num7 == num9 && num4 == num9 && num5 == num9 && num6 != num9 && num6 != num23 && num8 != num9 && num8 != num23)
										{
											if (num22 == 0)
											{
												rectangle.X = 144;
												rectangle.Y = 216;
											}
											if (num22 == 1)
											{
												rectangle.X = 198;
												rectangle.Y = 216;
											}
											if (num22 == 2)
											{
												rectangle.X = 252;
												rectangle.Y = 216;
											}
										}
										else
										{
											if (num7 != num9 && num7 != num23 && num2 == num9 && num4 == num9 && num5 == num9 && num != num9 && num != num23 && num3 != num9 && num3 != num23)
											{
												if (num22 == 0)
												{
													rectangle.X = 144;
													rectangle.Y = 252;
												}
												if (num22 == 1)
												{
													rectangle.X = 198;
													rectangle.Y = 252;
												}
												if (num22 == 2)
												{
													rectangle.X = 252;
													rectangle.Y = 252;
												}
											}
											else
											{
												if (num4 != num9 && num4 != num23 && num7 == num9 && num2 == num9 && num5 == num9 && num3 != num9 && num3 != num23 && num8 != num9 && num8 != num23)
												{
													if (num22 == 0)
													{
														rectangle.X = 126;
														rectangle.Y = 234;
													}
													if (num22 == 1)
													{
														rectangle.X = 180;
														rectangle.Y = 234;
													}
													if (num22 == 2)
													{
														rectangle.X = 234;
														rectangle.Y = 234;
													}
												}
												else
												{
													if (num5 != num9 && num5 != num23 && num7 == num9 && num2 == num9 && num4 == num9 && num != num9 && num != num23 && num6 != num9 && num6 != num23)
													{
														if (num22 == 0)
														{
															rectangle.X = 162;
															rectangle.Y = 234;
														}
														if (num22 == 1)
														{
															rectangle.X = 216;
															rectangle.Y = 234;
														}
														if (num22 == 2)
														{
															rectangle.X = 270;
															rectangle.Y = 234;
														}
													}
													else
													{
														if (num2 != num23 && num2 != num9 && (num7 == num23 || num7 == num9) && num4 == num23 && num5 == num23)
														{
															if (num22 == 0)
															{
																rectangle.X = 36;
																rectangle.Y = 270;
															}
															if (num22 == 1)
															{
																rectangle.X = 54;
																rectangle.Y = 270;
															}
															if (num22 == 2)
															{
																rectangle.X = 72;
																rectangle.Y = 270;
															}
														}
														else
														{
															if (num7 != num23 && num7 != num9 && (num2 == num23 || num2 == num9) && num4 == num23 && num5 == num23)
															{
																if (num22 == 0)
																{
																	rectangle.X = 36;
																	rectangle.Y = 288;
																}
																if (num22 == 1)
																{
																	rectangle.X = 54;
																	rectangle.Y = 288;
																}
																if (num22 == 2)
																{
																	rectangle.X = 72;
																	rectangle.Y = 288;
																}
															}
															else
															{
																if (num4 != num23 && num4 != num9 && (num5 == num23 || num5 == num9) && num2 == num23 && num7 == num23)
																{
																	if (num22 == 0)
																	{
																		rectangle.X = 0;
																		rectangle.Y = 270;
																	}
																	if (num22 == 1)
																	{
																		rectangle.X = 0;
																		rectangle.Y = 288;
																	}
																	if (num22 == 2)
																	{
																		rectangle.X = 0;
																		rectangle.Y = 306;
																	}
																}
																else
																{
																	if (num5 != num23 && num5 != num9 && (num4 == num23 || num4 == num9) && num2 == num23 && num7 == num23)
																	{
																		if (num22 == 0)
																		{
																			rectangle.X = 18;
																			rectangle.Y = 270;
																		}
																		if (num22 == 1)
																		{
																			rectangle.X = 18;
																			rectangle.Y = 288;
																		}
																		if (num22 == 2)
																		{
																			rectangle.X = 18;
																			rectangle.Y = 306;
																		}
																	}
																	else
																	{
																		if (num2 == num9 && num7 == num23 && num4 == num23 && num5 == num23)
																		{
																			if (num22 == 0)
																			{
																				rectangle.X = 198;
																				rectangle.Y = 288;
																			}
																			if (num22 == 1)
																			{
																				rectangle.X = 216;
																				rectangle.Y = 288;
																			}
																			if (num22 == 2)
																			{
																				rectangle.X = 234;
																				rectangle.Y = 288;
																			}
																		}
																		else
																		{
																			if (num2 == num23 && num7 == num9 && num4 == num23 && num5 == num23)
																			{
																				if (num22 == 0)
																				{
																					rectangle.X = 198;
																					rectangle.Y = 270;
																				}
																				if (num22 == 1)
																				{
																					rectangle.X = 216;
																					rectangle.Y = 270;
																				}
																				if (num22 == 2)
																				{
																					rectangle.X = 234;
																					rectangle.Y = 270;
																				}
																			}
																			else
																			{
																				if (num2 == num23 && num7 == num23 && num4 == num9 && num5 == num23)
																				{
																					if (num22 == 0)
																					{
																						rectangle.X = 198;
																						rectangle.Y = 306;
																					}
																					if (num22 == 1)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 306;
																					}
																					if (num22 == 2)
																					{
																						rectangle.X = 234;
																						rectangle.Y = 306;
																					}
																				}
																				else
																				{
																					if (num2 == num23 && num7 == num23 && num4 == num23 && num5 == num9)
																					{
																						if (num22 == 0)
																						{
																							rectangle.X = 144;
																							rectangle.Y = 306;
																						}
																						if (num22 == 1)
																						{
																							rectangle.X = 162;
																							rectangle.Y = 306;
																						}
																						if (num22 == 2)
																						{
																							rectangle.X = 180;
																							rectangle.Y = 306;
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
						if (num2 != num9 && num2 != num23 && num7 == num9 && num4 == num9 && num5 == num9)
						{
							if ((num6 == num23 || num6 == num9) && num8 != num23 && num8 != num9)
							{
								if (num22 == 0)
								{
									rectangle.X = 0;
									rectangle.Y = 324;
								}
								if (num22 == 1)
								{
									rectangle.X = 18;
									rectangle.Y = 324;
								}
								if (num22 == 2)
								{
									rectangle.X = 36;
									rectangle.Y = 324;
								}
							}
							else
							{
								if ((num8 == num23 || num8 == num9) && num6 != num23 && num6 != num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 54;
										rectangle.Y = 324;
									}
									if (num22 == 1)
									{
										rectangle.X = 72;
										rectangle.Y = 324;
									}
									if (num22 == 2)
									{
										rectangle.X = 90;
										rectangle.Y = 324;
									}
								}
							}
						}
						else
						{
							if (num7 != num9 && num7 != num23 && num2 == num9 && num4 == num9 && num5 == num9)
							{
								if ((num == num23 || num == num9) && num3 != num23 && num3 != num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 0;
										rectangle.Y = 342;
									}
									if (num22 == 1)
									{
										rectangle.X = 18;
										rectangle.Y = 342;
									}
									if (num22 == 2)
									{
										rectangle.X = 36;
										rectangle.Y = 342;
									}
								}
								else
								{
									if ((num3 == num23 || num3 == num9) && num != num23 && num != num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 54;
											rectangle.Y = 342;
										}
										if (num22 == 1)
										{
											rectangle.X = 72;
											rectangle.Y = 342;
										}
										if (num22 == 2)
										{
											rectangle.X = 90;
											rectangle.Y = 342;
										}
									}
								}
							}
							else
							{
								if (num4 != num9 && num4 != num23 && num2 == num9 && num7 == num9 && num5 == num9)
								{
									if ((num3 == num23 || num3 == num9) && num8 != num23 && num8 != num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 54;
											rectangle.Y = 360;
										}
										if (num22 == 1)
										{
											rectangle.X = 72;
											rectangle.Y = 360;
										}
										if (num22 == 2)
										{
											rectangle.X = 90;
											rectangle.Y = 360;
										}
									}
									else
									{
										if ((num8 == num23 || num8 == num9) && num3 != num23 && num3 != num9)
										{
											if (num22 == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 360;
											}
											if (num22 == 1)
											{
												rectangle.X = 18;
												rectangle.Y = 360;
											}
											if (num22 == 2)
											{
												rectangle.X = 36;
												rectangle.Y = 360;
											}
										}
									}
								}
								else
								{
									if (num5 != num9 && num5 != num23 && num2 == num9 && num7 == num9 && num4 == num9)
									{
										if ((num == num23 || num == num9) && num6 != num23 && num6 != num9)
										{
											if (num22 == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 378;
											}
											if (num22 == 1)
											{
												rectangle.X = 18;
												rectangle.Y = 378;
											}
											if (num22 == 2)
											{
												rectangle.X = 36;
												rectangle.Y = 378;
											}
										}
										else
										{
											if ((num6 == num23 || num6 == num9) && num != num23 && num != num9)
											{
												if (num22 == 0)
												{
													rectangle.X = 54;
													rectangle.Y = 378;
												}
												if (num22 == 1)
												{
													rectangle.X = 72;
													rectangle.Y = 378;
												}
												if (num22 == 2)
												{
													rectangle.X = 90;
													rectangle.Y = 378;
												}
											}
										}
									}
								}
							}
						}
						if ((num2 == num9 || num2 == num23) && (num7 == num9 || num7 == num23) && (num4 == num9 || num4 == num23) && (num5 == num9 || num5 == num23) && num != -1 && num3 != -1 && num6 != -1 && num8 != -1)
						{
							if (num22 == 0)
							{
								rectangle.X = 18;
								rectangle.Y = 18;
							}
							if (num22 == 1)
							{
								rectangle.X = 36;
								rectangle.Y = 18;
							}
							if (num22 == 2)
							{
								rectangle.X = 54;
								rectangle.Y = 18;
							}
						}
						if (num2 == num23)
						{
							num2 = -2;
						}
						if (num7 == num23)
						{
							num7 = -2;
						}
						if (num4 == num23)
						{
							num4 = -2;
						}
						if (num5 == num23)
						{
							num5 = -2;
						}
						if (num == num23)
						{
							num = -2;
						}
						if (num3 == num23)
						{
							num3 = -2;
						}
						if (num6 == num23)
						{
							num6 = -2;
						}
						if (num8 == num23)
						{
							num8 = -2;
						}
					}
					if ((num9 == 1 || num9 == 2 || num9 == 6 || num9 == 7 || num9 == 8 || num9 == 9 || num9 == 22 || num9 == 23 || num9 == 25 || num9 == 37 || num9 == 40 || num9 == 53 || num9 == 56 || num9 == 58 || num9 == 59 || num9 == 60 || num9 == 70) && rectangle.X == -1 && rectangle.Y == -1)
					{
						if (num2 >= 0 && num2 != num9)
						{
							num2 = -1;
						}
						if (num7 >= 0 && num7 != num9)
						{
							num7 = -1;
						}
						if (num4 >= 0 && num4 != num9)
						{
							num4 = -1;
						}
						if (num5 >= 0 && num5 != num9)
						{
							num5 = -1;
						}
						if (num2 != -1 && num7 != -1 && num4 != -1 && num5 != -1)
						{
							if (num2 == -2 && num7 == num9 && num4 == num9 && num5 == num9)
							{
								if (num22 == 0)
								{
									rectangle.X = 144;
									rectangle.Y = 108;
								}
								if (num22 == 1)
								{
									rectangle.X = 162;
									rectangle.Y = 108;
								}
								if (num22 == 2)
								{
									rectangle.X = 180;
									rectangle.Y = 108;
								}
								mergeUp = true;
							}
							else
							{
								if (num2 == num9 && num7 == -2 && num4 == num9 && num5 == num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 144;
										rectangle.Y = 90;
									}
									if (num22 == 1)
									{
										rectangle.X = 162;
										rectangle.Y = 90;
									}
									if (num22 == 2)
									{
										rectangle.X = 180;
										rectangle.Y = 90;
									}
									mergeDown = true;
								}
								else
								{
									if (num2 == num9 && num7 == num9 && num4 == -2 && num5 == num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 162;
											rectangle.Y = 126;
										}
										if (num22 == 1)
										{
											rectangle.X = 162;
											rectangle.Y = 144;
										}
										if (num22 == 2)
										{
											rectangle.X = 162;
											rectangle.Y = 162;
										}
										mergeLeft = true;
									}
									else
									{
										if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == -2)
										{
											if (num22 == 0)
											{
												rectangle.X = 144;
												rectangle.Y = 126;
											}
											if (num22 == 1)
											{
												rectangle.X = 144;
												rectangle.Y = 144;
											}
											if (num22 == 2)
											{
												rectangle.X = 144;
												rectangle.Y = 162;
											}
											mergeRight = true;
										}
										else
										{
											if (num2 == -2 && num7 == num9 && num4 == -2 && num5 == num9)
											{
												if (num22 == 0)
												{
													rectangle.X = 36;
													rectangle.Y = 90;
												}
												if (num22 == 1)
												{
													rectangle.X = 36;
													rectangle.Y = 126;
												}
												if (num22 == 2)
												{
													rectangle.X = 36;
													rectangle.Y = 162;
												}
												mergeUp = true;
												mergeLeft = true;
											}
											else
											{
												if (num2 == -2 && num7 == num9 && num4 == num9 && num5 == -2)
												{
													if (num22 == 0)
													{
														rectangle.X = 54;
														rectangle.Y = 90;
													}
													if (num22 == 1)
													{
														rectangle.X = 54;
														rectangle.Y = 126;
													}
													if (num22 == 2)
													{
														rectangle.X = 54;
														rectangle.Y = 162;
													}
													mergeUp = true;
													mergeRight = true;
												}
												else
												{
													if (num2 == num9 && num7 == -2 && num4 == -2 && num5 == num9)
													{
														if (num22 == 0)
														{
															rectangle.X = 36;
															rectangle.Y = 108;
														}
														if (num22 == 1)
														{
															rectangle.X = 36;
															rectangle.Y = 144;
														}
														if (num22 == 2)
														{
															rectangle.X = 36;
															rectangle.Y = 180;
														}
														mergeDown = true;
														mergeLeft = true;
													}
													else
													{
														if (num2 == num9 && num7 == -2 && num4 == num9 && num5 == -2)
														{
															if (num22 == 0)
															{
																rectangle.X = 54;
																rectangle.Y = 108;
															}
															if (num22 == 1)
															{
																rectangle.X = 54;
																rectangle.Y = 144;
															}
															if (num22 == 2)
															{
																rectangle.X = 54;
																rectangle.Y = 180;
															}
															mergeDown = true;
															mergeRight = true;
														}
														else
														{
															if (num2 == num9 && num7 == num9 && num4 == -2 && num5 == -2)
															{
																if (num22 == 0)
																{
																	rectangle.X = 180;
																	rectangle.Y = 126;
																}
																if (num22 == 1)
																{
																	rectangle.X = 180;
																	rectangle.Y = 144;
																}
																if (num22 == 2)
																{
																	rectangle.X = 180;
																	rectangle.Y = 162;
																}
																mergeLeft = true;
																mergeRight = true;
															}
															else
															{
																if (num2 == -2 && num7 == -2 && num4 == num9 && num5 == num9)
																{
																	if (num22 == 0)
																	{
																		rectangle.X = 144;
																		rectangle.Y = 180;
																	}
																	if (num22 == 1)
																	{
																		rectangle.X = 162;
																		rectangle.Y = 180;
																	}
																	if (num22 == 2)
																	{
																		rectangle.X = 180;
																		rectangle.Y = 180;
																	}
																	mergeUp = true;
																	mergeDown = true;
																}
																else
																{
																	if (num2 == -2 && num7 == num9 && num4 == -2 && num5 == -2)
																	{
																		if (num22 == 0)
																		{
																			rectangle.X = 198;
																			rectangle.Y = 90;
																		}
																		if (num22 == 1)
																		{
																			rectangle.X = 198;
																			rectangle.Y = 108;
																		}
																		if (num22 == 2)
																		{
																			rectangle.X = 198;
																			rectangle.Y = 126;
																		}
																		mergeUp = true;
																		mergeLeft = true;
																		mergeRight = true;
																	}
																	else
																	{
																		if (num2 == num9 && num7 == -2 && num4 == -2 && num5 == -2)
																		{
																			if (num22 == 0)
																			{
																				rectangle.X = 198;
																				rectangle.Y = 144;
																			}
																			if (num22 == 1)
																			{
																				rectangle.X = 198;
																				rectangle.Y = 162;
																			}
																			if (num22 == 2)
																			{
																				rectangle.X = 198;
																				rectangle.Y = 180;
																			}
																			mergeDown = true;
																			mergeLeft = true;
																			mergeRight = true;
																		}
																		else
																		{
																			if (num2 == -2 && num7 == -2 && num4 == num9 && num5 == -2)
																			{
																				if (num22 == 0)
																				{
																					rectangle.X = 216;
																					rectangle.Y = 144;
																				}
																				if (num22 == 1)
																				{
																					rectangle.X = 216;
																					rectangle.Y = 162;
																				}
																				if (num22 == 2)
																				{
																					rectangle.X = 216;
																					rectangle.Y = 180;
																				}
																				mergeUp = true;
																				mergeDown = true;
																				mergeRight = true;
																			}
																			else
																			{
																				if (num2 == -2 && num7 == -2 && num4 == -2 && num5 == num9)
																				{
																					if (num22 == 0)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 90;
																					}
																					if (num22 == 1)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 108;
																					}
																					if (num22 == 2)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 126;
																					}
																					mergeUp = true;
																					mergeDown = true;
																					mergeLeft = true;
																				}
																				else
																				{
																					if (num2 == -2 && num7 == -2 && num4 == -2 && num5 == -2)
																					{
																						if (num22 == 0)
																						{
																							rectangle.X = 108;
																							rectangle.Y = 198;
																						}
																						if (num22 == 1)
																						{
																							rectangle.X = 126;
																							rectangle.Y = 198;
																						}
																						if (num22 == 2)
																						{
																							rectangle.X = 144;
																							rectangle.Y = 198;
																						}
																						mergeUp = true;
																						mergeDown = true;
																						mergeLeft = true;
																						mergeRight = true;
																					}
																					else
																					{
																						if (num2 == num9 && num7 == num9 && num4 == num9 && num5 == num9)
																						{
																							if (num == -2)
																							{
																								if (num22 == 0)
																								{
																									rectangle.X = 18;
																									rectangle.Y = 108;
																								}
																								if (num22 == 1)
																								{
																									rectangle.X = 18;
																									rectangle.Y = 144;
																								}
																								if (num22 == 2)
																								{
																									rectangle.X = 18;
																									rectangle.Y = 180;
																								}
																							}
																							if (num3 == -2)
																							{
																								if (num22 == 0)
																								{
																									rectangle.X = 0;
																									rectangle.Y = 108;
																								}
																								if (num22 == 1)
																								{
																									rectangle.X = 0;
																									rectangle.Y = 144;
																								}
																								if (num22 == 2)
																								{
																									rectangle.X = 0;
																									rectangle.Y = 180;
																								}
																							}
																							if (num6 == -2)
																							{
																								if (num22 == 0)
																								{
																									rectangle.X = 18;
																									rectangle.Y = 90;
																								}
																								if (num22 == 1)
																								{
																									rectangle.X = 18;
																									rectangle.Y = 126;
																								}
																								if (num22 == 2)
																								{
																									rectangle.X = 18;
																									rectangle.Y = 162;
																								}
																							}
																							if (num8 == -2)
																							{
																								if (num22 == 0)
																								{
																									rectangle.X = 0;
																									rectangle.Y = 90;
																								}
																								if (num22 == 1)
																								{
																									rectangle.X = 0;
																									rectangle.Y = 126;
																								}
																								if (num22 == 2)
																								{
																									rectangle.X = 0;
																									rectangle.Y = 162;
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
						else
						{
							if (num9 != 2 && num9 != 23 && num9 != 60 && num9 != 70)
							{
								if (num2 == -1 && num7 == -2 && num4 == num9 && num5 == num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 234;
										rectangle.Y = 0;
									}
									if (num22 == 1)
									{
										rectangle.X = 252;
										rectangle.Y = 0;
									}
									if (num22 == 2)
									{
										rectangle.X = 270;
										rectangle.Y = 0;
									}
									mergeDown = true;
								}
								else
								{
									if (num2 == -2 && num7 == -1 && num4 == num9 && num5 == num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 234;
											rectangle.Y = 18;
										}
										if (num22 == 1)
										{
											rectangle.X = 252;
											rectangle.Y = 18;
										}
										if (num22 == 2)
										{
											rectangle.X = 270;
											rectangle.Y = 18;
										}
										mergeUp = true;
									}
									else
									{
										if (num2 == num9 && num7 == num9 && num4 == -1 && num5 == -2)
										{
											if (num22 == 0)
											{
												rectangle.X = 234;
												rectangle.Y = 36;
											}
											if (num22 == 1)
											{
												rectangle.X = 252;
												rectangle.Y = 36;
											}
											if (num22 == 2)
											{
												rectangle.X = 270;
												rectangle.Y = 36;
											}
											mergeRight = true;
										}
										else
										{
											if (num2 == num9 && num7 == num9 && num4 == -2 && num5 == -1)
											{
												if (num22 == 0)
												{
													rectangle.X = 234;
													rectangle.Y = 54;
												}
												if (num22 == 1)
												{
													rectangle.X = 252;
													rectangle.Y = 54;
												}
												if (num22 == 2)
												{
													rectangle.X = 270;
													rectangle.Y = 54;
												}
												mergeLeft = true;
											}
										}
									}
								}
							}
							if (num2 != -1 && num7 != -1 && num4 == -1 && num5 == num9)
							{
								if (num2 == -2 && num7 == num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 72;
										rectangle.Y = 144;
									}
									if (num22 == 1)
									{
										rectangle.X = 72;
										rectangle.Y = 162;
									}
									if (num22 == 2)
									{
										rectangle.X = 72;
										rectangle.Y = 180;
									}
									mergeUp = true;
								}
								else
								{
									if (num7 == -2 && num2 == num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 72;
											rectangle.Y = 90;
										}
										if (num22 == 1)
										{
											rectangle.X = 72;
											rectangle.Y = 108;
										}
										if (num22 == 2)
										{
											rectangle.X = 72;
											rectangle.Y = 126;
										}
										mergeDown = true;
									}
								}
							}
							else
							{
								if (num2 != -1 && num7 != -1 && num4 == num9 && num5 == -1)
								{
									if (num2 == -2 && num7 == num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 90;
											rectangle.Y = 144;
										}
										if (num22 == 1)
										{
											rectangle.X = 90;
											rectangle.Y = 162;
										}
										if (num22 == 2)
										{
											rectangle.X = 90;
											rectangle.Y = 180;
										}
										mergeUp = true;
									}
									else
									{
										if (num7 == -2 && num2 == num9)
										{
											if (num22 == 0)
											{
												rectangle.X = 90;
												rectangle.Y = 90;
											}
											if (num22 == 1)
											{
												rectangle.X = 90;
												rectangle.Y = 108;
											}
											if (num22 == 2)
											{
												rectangle.X = 90;
												rectangle.Y = 126;
											}
											mergeDown = true;
										}
									}
								}
								else
								{
									if (num2 == -1 && num7 == num9 && num4 != -1 && num5 != -1)
									{
										if (num4 == -2 && num5 == num9)
										{
											if (num22 == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 198;
											}
											if (num22 == 1)
											{
												rectangle.X = 18;
												rectangle.Y = 198;
											}
											if (num22 == 2)
											{
												rectangle.X = 36;
												rectangle.Y = 198;
											}
											mergeLeft = true;
										}
										else
										{
											if (num5 == -2 && num4 == num9)
											{
												if (num22 == 0)
												{
													rectangle.X = 54;
													rectangle.Y = 198;
												}
												if (num22 == 1)
												{
													rectangle.X = 72;
													rectangle.Y = 198;
												}
												if (num22 == 2)
												{
													rectangle.X = 90;
													rectangle.Y = 198;
												}
												mergeRight = true;
											}
										}
									}
									else
									{
										if (num2 == num9 && num7 == -1 && num4 != -1 && num5 != -1)
										{
											if (num4 == -2 && num5 == num9)
											{
												if (num22 == 0)
												{
													rectangle.X = 0;
													rectangle.Y = 216;
												}
												if (num22 == 1)
												{
													rectangle.X = 18;
													rectangle.Y = 216;
												}
												if (num22 == 2)
												{
													rectangle.X = 36;
													rectangle.Y = 216;
												}
												mergeLeft = true;
											}
											else
											{
												if (num5 == -2 && num4 == num9)
												{
													if (num22 == 0)
													{
														rectangle.X = 54;
														rectangle.Y = 216;
													}
													if (num22 == 1)
													{
														rectangle.X = 72;
														rectangle.Y = 216;
													}
													if (num22 == 2)
													{
														rectangle.X = 90;
														rectangle.Y = 216;
													}
													mergeRight = true;
												}
											}
										}
										else
										{
											if (num2 != -1 && num7 != -1 && num4 == -1 && num5 == -1)
											{
												if (num2 == -2 && num7 == -2)
												{
													if (num22 == 0)
													{
														rectangle.X = 108;
														rectangle.Y = 216;
													}
													if (num22 == 1)
													{
														rectangle.X = 108;
														rectangle.Y = 234;
													}
													if (num22 == 2)
													{
														rectangle.X = 108;
														rectangle.Y = 252;
													}
													mergeUp = true;
													mergeDown = true;
												}
												else
												{
													if (num2 == -2)
													{
														if (num22 == 0)
														{
															rectangle.X = 126;
															rectangle.Y = 144;
														}
														if (num22 == 1)
														{
															rectangle.X = 126;
															rectangle.Y = 162;
														}
														if (num22 == 2)
														{
															rectangle.X = 126;
															rectangle.Y = 180;
														}
														mergeUp = true;
													}
													else
													{
														if (num7 == -2)
														{
															if (num22 == 0)
															{
																rectangle.X = 126;
																rectangle.Y = 90;
															}
															if (num22 == 1)
															{
																rectangle.X = 126;
																rectangle.Y = 108;
															}
															if (num22 == 2)
															{
																rectangle.X = 126;
																rectangle.Y = 126;
															}
															mergeDown = true;
														}
													}
												}
											}
											else
											{
												if (num2 == -1 && num7 == -1 && num4 != -1 && num5 != -1)
												{
													if (num4 == -2 && num5 == -2)
													{
														if (num22 == 0)
														{
															rectangle.X = 162;
															rectangle.Y = 198;
														}
														if (num22 == 1)
														{
															rectangle.X = 180;
															rectangle.Y = 198;
														}
														if (num22 == 2)
														{
															rectangle.X = 198;
															rectangle.Y = 198;
														}
														mergeLeft = true;
														mergeRight = true;
													}
													else
													{
														if (num4 == -2)
														{
															if (num22 == 0)
															{
																rectangle.X = 0;
																rectangle.Y = 252;
															}
															if (num22 == 1)
															{
																rectangle.X = 18;
																rectangle.Y = 252;
															}
															if (num22 == 2)
															{
																rectangle.X = 36;
																rectangle.Y = 252;
															}
															mergeLeft = true;
														}
														else
														{
															if (num5 == -2)
															{
																if (num22 == 0)
																{
																	rectangle.X = 54;
																	rectangle.Y = 252;
																}
																if (num22 == 1)
																{
																	rectangle.X = 72;
																	rectangle.Y = 252;
																}
																if (num22 == 2)
																{
																	rectangle.X = 90;
																	rectangle.Y = 252;
																}
																mergeRight = true;
															}
														}
													}
												}
												else
												{
													if (num2 == -2 && num7 == -1 && num4 == -1 && num5 == -1)
													{
														if (num22 == 0)
														{
															rectangle.X = 108;
															rectangle.Y = 144;
														}
														if (num22 == 1)
														{
															rectangle.X = 108;
															rectangle.Y = 162;
														}
														if (num22 == 2)
														{
															rectangle.X = 108;
															rectangle.Y = 180;
														}
														mergeUp = true;
													}
													else
													{
														if (num2 == -1 && num7 == -2 && num4 == -1 && num5 == -1)
														{
															if (num22 == 0)
															{
																rectangle.X = 108;
																rectangle.Y = 90;
															}
															if (num22 == 1)
															{
																rectangle.X = 108;
																rectangle.Y = 108;
															}
															if (num22 == 2)
															{
																rectangle.X = 108;
																rectangle.Y = 126;
															}
															mergeDown = true;
														}
														else
														{
															if (num2 == -1 && num7 == -1 && num4 == -2 && num5 == -1)
															{
																if (num22 == 0)
																{
																	rectangle.X = 0;
																	rectangle.Y = 234;
																}
																if (num22 == 1)
																{
																	rectangle.X = 18;
																	rectangle.Y = 234;
																}
																if (num22 == 2)
																{
																	rectangle.X = 36;
																	rectangle.Y = 234;
																}
																mergeLeft = true;
															}
															else
															{
																if (num2 == -1 && num7 == -1 && num4 == -1 && num5 == -2)
																{
																	if (num22 == 0)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 234;
																	}
																	if (num22 == 1)
																	{
																		rectangle.X = 72;
																		rectangle.Y = 234;
																	}
																	if (num22 == 2)
																	{
																		rectangle.X = 90;
																		rectangle.Y = 234;
																	}
																	mergeRight = true;
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
					if (rectangle.X < 0 || rectangle.Y < 0)
					{
						if (num9 == 2 || num9 == 23 || num9 == 60 || num9 == 70)
						{
							if (num2 == -2)
							{
								num2 = num9;
							}
							if (num7 == -2)
							{
								num7 = num9;
							}
							if (num4 == -2)
							{
								num4 = num9;
							}
							if (num5 == -2)
							{
								num5 = num9;
							}
							if (num == -2)
							{
								num = num9;
							}
							if (num3 == -2)
							{
								num3 = num9;
							}
							if (num6 == -2)
							{
								num6 = num9;
							}
							if (num8 == -2)
							{
								num8 = num9;
							}
						}
						if (num2 == num9 && num7 == num9 && (num4 == num9 & num5 == num9))
						{
							if (num != num9 && num3 != num9)
							{
								if (num22 == 0)
								{
									rectangle.X = 108;
									rectangle.Y = 18;
								}
								if (num22 == 1)
								{
									rectangle.X = 126;
									rectangle.Y = 18;
								}
								if (num22 == 2)
								{
									rectangle.X = 144;
									rectangle.Y = 18;
								}
							}
							else
							{
								if (num6 != num9 && num8 != num9)
								{
									if (num22 == 0)
									{
										rectangle.X = 108;
										rectangle.Y = 36;
									}
									if (num22 == 1)
									{
										rectangle.X = 126;
										rectangle.Y = 36;
									}
									if (num22 == 2)
									{
										rectangle.X = 144;
										rectangle.Y = 36;
									}
								}
								else
								{
									if (num != num9 && num6 != num9)
									{
										if (num22 == 0)
										{
											rectangle.X = 180;
											rectangle.Y = 0;
										}
										if (num22 == 1)
										{
											rectangle.X = 180;
											rectangle.Y = 18;
										}
										if (num22 == 2)
										{
											rectangle.X = 180;
											rectangle.Y = 36;
										}
									}
									else
									{
										if (num3 != num9 && num8 != num9)
										{
											if (num22 == 0)
											{
												rectangle.X = 198;
												rectangle.Y = 0;
											}
											if (num22 == 1)
											{
												rectangle.X = 198;
												rectangle.Y = 18;
											}
											if (num22 == 2)
											{
												rectangle.X = 198;
												rectangle.Y = 36;
											}
										}
										else
										{
											if (num22 == 0)
											{
												rectangle.X = 18;
												rectangle.Y = 18;
											}
											if (num22 == 1)
											{
												rectangle.X = 36;
												rectangle.Y = 18;
											}
											if (num22 == 2)
											{
												rectangle.X = 54;
												rectangle.Y = 18;
											}
										}
									}
								}
							}
						}
						else
						{
							if (num2 != num9 && num7 == num9 && (num4 == num9 & num5 == num9))
							{
								if (num22 == 0)
								{
									rectangle.X = 18;
									rectangle.Y = 0;
								}
								if (num22 == 1)
								{
									rectangle.X = 36;
									rectangle.Y = 0;
								}
								if (num22 == 2)
								{
									rectangle.X = 54;
									rectangle.Y = 0;
								}
							}
							else
							{
								if (num2 == num9 && num7 != num9 && (num4 == num9 & num5 == num9))
								{
									if (num22 == 0)
									{
										rectangle.X = 18;
										rectangle.Y = 36;
									}
									if (num22 == 1)
									{
										rectangle.X = 36;
										rectangle.Y = 36;
									}
									if (num22 == 2)
									{
										rectangle.X = 54;
										rectangle.Y = 36;
									}
								}
								else
								{
									if (num2 == num9 && num7 == num9 && (num4 != num9 & num5 == num9))
									{
										if (num22 == 0)
										{
											rectangle.X = 0;
											rectangle.Y = 0;
										}
										if (num22 == 1)
										{
											rectangle.X = 0;
											rectangle.Y = 18;
										}
										if (num22 == 2)
										{
											rectangle.X = 0;
											rectangle.Y = 36;
										}
									}
									else
									{
										if (num2 == num9 && num7 == num9 && (num4 == num9 & num5 != num9))
										{
											if (num22 == 0)
											{
												rectangle.X = 72;
												rectangle.Y = 0;
											}
											if (num22 == 1)
											{
												rectangle.X = 72;
												rectangle.Y = 18;
											}
											if (num22 == 2)
											{
												rectangle.X = 72;
												rectangle.Y = 36;
											}
										}
										else
										{
											if (num2 != num9 && num7 == num9 && (num4 != num9 & num5 == num9))
											{
												if (num22 == 0)
												{
													rectangle.X = 0;
													rectangle.Y = 54;
												}
												if (num22 == 1)
												{
													rectangle.X = 36;
													rectangle.Y = 54;
												}
												if (num22 == 2)
												{
													rectangle.X = 72;
													rectangle.Y = 54;
												}
											}
											else
											{
												if (num2 != num9 && num7 == num9 && (num4 == num9 & num5 != num9))
												{
													if (num22 == 0)
													{
														rectangle.X = 18;
														rectangle.Y = 54;
													}
													if (num22 == 1)
													{
														rectangle.X = 54;
														rectangle.Y = 54;
													}
													if (num22 == 2)
													{
														rectangle.X = 90;
														rectangle.Y = 54;
													}
												}
												else
												{
													if (num2 == num9 && num7 != num9 && (num4 != num9 & num5 == num9))
													{
														if (num22 == 0)
														{
															rectangle.X = 0;
															rectangle.Y = 72;
														}
														if (num22 == 1)
														{
															rectangle.X = 36;
															rectangle.Y = 72;
														}
														if (num22 == 2)
														{
															rectangle.X = 72;
															rectangle.Y = 72;
														}
													}
													else
													{
														if (num2 == num9 && num7 != num9 && (num4 == num9 & num5 != num9))
														{
															if (num22 == 0)
															{
																rectangle.X = 18;
																rectangle.Y = 72;
															}
															if (num22 == 1)
															{
																rectangle.X = 54;
																rectangle.Y = 72;
															}
															if (num22 == 2)
															{
																rectangle.X = 90;
																rectangle.Y = 72;
															}
														}
														else
														{
															if (num2 == num9 && num7 == num9 && (num4 != num9 & num5 != num9))
															{
																if (num22 == 0)
																{
																	rectangle.X = 90;
																	rectangle.Y = 0;
																}
																if (num22 == 1)
																{
																	rectangle.X = 90;
																	rectangle.Y = 18;
																}
																if (num22 == 2)
																{
																	rectangle.X = 90;
																	rectangle.Y = 36;
																}
															}
															else
															{
																if (num2 != num9 && num7 != num9 && (num4 == num9 & num5 == num9))
																{
																	if (num22 == 0)
																	{
																		rectangle.X = 108;
																		rectangle.Y = 72;
																	}
																	if (num22 == 1)
																	{
																		rectangle.X = 126;
																		rectangle.Y = 72;
																	}
																	if (num22 == 2)
																	{
																		rectangle.X = 144;
																		rectangle.Y = 72;
																	}
																}
																else
																{
																	if (num2 != num9 && num7 == num9 && (num4 != num9 & num5 != num9))
																	{
																		if (num22 == 0)
																		{
																			rectangle.X = 108;
																			rectangle.Y = 0;
																		}
																		if (num22 == 1)
																		{
																			rectangle.X = 126;
																			rectangle.Y = 0;
																		}
																		if (num22 == 2)
																		{
																			rectangle.X = 144;
																			rectangle.Y = 0;
																		}
																	}
																	else
																	{
																		if (num2 == num9 && num7 != num9 && (num4 != num9 & num5 != num9))
																		{
																			if (num22 == 0)
																			{
																				rectangle.X = 108;
																				rectangle.Y = 54;
																			}
																			if (num22 == 1)
																			{
																				rectangle.X = 126;
																				rectangle.Y = 54;
																			}
																			if (num22 == 2)
																			{
																				rectangle.X = 144;
																				rectangle.Y = 54;
																			}
																		}
																		else
																		{
																			if (num2 != num9 && num7 != num9 && (num4 != num9 & num5 == num9))
																			{
																				if (num22 == 0)
																				{
																					rectangle.X = 162;
																					rectangle.Y = 0;
																				}
																				if (num22 == 1)
																				{
																					rectangle.X = 162;
																					rectangle.Y = 18;
																				}
																				if (num22 == 2)
																				{
																					rectangle.X = 162;
																					rectangle.Y = 36;
																				}
																			}
																			else
																			{
																				if (num2 != num9 && num7 != num9 && (num4 == num9 & num5 != num9))
																				{
																					if (num22 == 0)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 0;
																					}
																					if (num22 == 1)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 18;
																					}
																					if (num22 == 2)
																					{
																						rectangle.X = 216;
																						rectangle.Y = 36;
																					}
																				}
																				else
																				{
																					if (num2 != num9 && num7 != num9 && (num4 != num9 & num5 != num9))
																					{
																						if (num22 == 0)
																						{
																							rectangle.X = 162;
																							rectangle.Y = 54;
																						}
																						if (num22 == 1)
																						{
																							rectangle.X = 180;
																							rectangle.Y = 54;
																						}
																						if (num22 == 2)
																						{
																							rectangle.X = 198;
																							rectangle.Y = 54;
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
					if (rectangle.X <= -1 || rectangle.Y <= -1)
					{
						if (num22 <= 0)
						{
							rectangle.X = 18;
							rectangle.Y = 18;
						}
						if (num22 == 1)
						{
							rectangle.X = 36;
							rectangle.Y = 18;
						}
						if (num22 >= 2)
						{
							rectangle.X = 54;
							rectangle.Y = 18;
						}
					}
					TileAt(i, j).SetFrameX((short)rectangle.X);
					TileAt(i, j).SetFrameY((short)rectangle.Y);
					if (num9 == 52 || num9 == 62)
					{
						if (!TileAt(i, j - 1).Active)
						{
							num2 = -1;
						}
						else
						{
							num2 = (int)TileAt(i, j - 1).Type;
						}
						if (num2 != num9 && num2 != 2 && num2 != 60)
						{
							KillTile(i, j, false, false, false);
						}
					}
					if (!noTileActions && num9 == 53)
					{
						if (!TileAt(i, j + 1).Active)
							{
								bool flag4 = true;
								if (TileAt(i, j - 1).Active && TileAt(i, j - 1).Type == 21)
								{
									flag4 = false;
								}
								if (flag4)
								{
									int type2 = 31;
									if (num9 == 59)
									{
										type2 = 39;
									}
									if (num9 == 57)
									{
										type2 = 40;
									}
									TileAt(i, j).SetActive(false);
									sandbox.FallingBlockProjectile (i, j, type2);
//                                    int num25 = Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0.41f, type2, 10, 0f, Main.myPlayer);
//                                    Main.projectile[num25].Velocity.Y = 0.5f;
//                                    Projectile expr_6501_cp_0 = Main.projectile[num25];
//                                    expr_6501_cp_0.Position.Y = expr_6501_cp_0.Position.Y + 2f;
//                                    Main.projectile[num25].ai[0] = 1f;
									SquareTileFrame(i, j, true);
								}
							}
					}
					if (rectangle.X != frameX && rectangle.Y != frameY && frameX >= 0 && frameY >= 0)
					{
						bool flag5 = mergeUp;
						bool flag6 = mergeDown;
						bool flag7 = mergeLeft;
						bool flag8 = mergeRight;
						TileFrame(i - 1, j, false, false);
						TileFrame(i + 1, j, false, false);
						TileFrame(i, j - 1, false, false);
						TileFrame(i, j + 1, false, false);
						mergeUp = flag5;
						mergeDown = flag6;
						mergeLeft = flag7;
						mergeRight = flag8;
					}
				}
			}
		}*/
	}
}