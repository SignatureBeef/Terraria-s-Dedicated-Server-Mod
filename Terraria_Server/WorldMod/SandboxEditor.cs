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
			return WorldModify.EmptyTileCheck(startX, endX, startY, endY, ignoreStyle, ITileAt);
		}

		public bool PlaceDoor(int i, int j, int type)
		{
			return WorldModify.PlaceDoor(i, j, type, ITileAt);
		}

		public bool CloseDoor(int x, int y, bool forced, ISender sender)
		{
			return WorldModify.CloseDoor(x, y, forced, sender, ITileAt);
		}

		public bool OpenDoor(int x, int y, int direction, ISender sender)
		{
			return WorldModify.OpenDoor(x, y, direction, sender, ITileAt);
		}

		public void Check1x2(int x, int j, byte type)
		{
			WorldModify.Check1x2(x, j, type, ITileAt);
		}

		public void CheckOnTable1x1(int x, int y, int type)
		{
			WorldModify.CheckOnTable1x1(x, y, type, ITileAt);
		}

		public void CheckSign(int x, int y, int type)
		{
			WorldModify.CheckSign(x, y, type, ITileAt);
		}

		public bool PlaceSign(int x, int y, int type)
		{
			return WorldModify.PlaceSign(x, y, type, ITileAt);
		}

		public void PlaceOnTable1x1(int x, int y, int type, int style = 0)
		{
			WorldModify.PlaceOnTable1x1(x, y, type, style, ITileAt);
		}

		public bool PlaceAlch(int x, int y, int style) 
		{
			return WorldModify.PlaceAlch(x, y, style, ITileAt);
		}

		public void GrowAlch(int x, int y) 
		{
			WorldModify.GrowAlch(x, y, ITileAt);
		}

		public void PlantAlch() 
		{
			WorldModify.PlantAlch(ITileAt);
		}

		public void CheckAlch(int x, int y) 
		{
			WorldModify.CheckAlch(x, y, ITileAt);
		}

		public void Place1x2(int x, int y, int type, int style) 
		{
			WorldModify.Place1x2(x, y, type, style, ITileAt);
		}

		public void PlaceBanner(int x, int y, int type, int style = 0) 
		{
			WorldModify.PlaceBanner(x, y, type, style, ITileAt);
		}

		public void CheckBanner(int x, int j, byte type) 
		{
			WorldModify.CheckBanner(x, j, type, ITileAt);
		}

		public void Place1x2Top(int x, int y, int type) 
		{
			WorldModify.Place1x2Top(x, y, type, ITileAt);
		}

		//private TRef GetTile(int x, int y)
		//{
		//    return TileAt(x, y);
		//}

		public void Check1x2Top(int x, int y, byte type)
		{
			WorldModify.Check1x2Top(x, y, type, ITileAt);
		} 

		public void Check2x1(int x, int y, byte type) 
		{
			WorldModify.Check2x1(x, y, type, ITileAt);
		}

		public void Place2x1(int x, int y, int type) 
		{
			WorldModify.Place2x1(x, y, type, ITileAt);
		}

		public void Check4x2(int i, int j, int type) 
		{
			WorldModify.Check4x2(i, j, type, ITileAt);
		}

		public void Check2x2(int i, int j, int type) 
		{
			WorldModify.Check2x2(i, j, type, ITileAt);
		}

		public void Check3x2(int i, int j, int type) 
		{
			WorldModify.Check3x3(i, j, type, ITileAt);
		}

		public void Place4x2(int x, int y, int type, int direction = -1) 
		{
			WorldModify.Place4x2(x, y, type, direction, ITileAt);
		}

		public void Place2x2(int x, int superY, int type) 
		{
			WorldModify.Place2x2(x, superY, type, ITileAt);
		}

		public void Place3x2(int x, int y, int type) 
		{
			WorldModify.Place3x2(x, y, type, ITileAt);
		}

		public void Check3x3(int i, int j, int type) 
		{
			WorldModify.Check3x3(i, j, type, ITileAt);
		}

		public void Place3x3(int x, int y, int type) 
		{
			WorldModify.Place3x3(x, y, type, ITileAt);
		}

		public void Check3x4(int i, int j, int type) 
		{
			WorldModify.Check3x4(i, j, type, ITileAt);
		}

		public void Check1xX(int x, int j, byte type) 
		{
			WorldModify.Check1xX(x, j, type, ITileAt);
		}

		public void Check2xX(int i, int j, byte type) 
		{
			WorldModify.Check2xX(i, j, type, ITileAt);
		}

		public void PlaceSunflower(int x, int y, int type = 27) 
		{
			WorldModify.PlaceSunflower(x, y, type, ITileAt);
		}

		public void CheckSunflower(int i, int j, int type = 27) 
		{
			WorldModify.CheckSunflower(i, j, type, ITileAt);
		}

		public bool PlacePot(int x, int y, int type = 28) 
		{
			return WorldModify.PlacePot(x, y, type, ITileAt);
		}

		public bool CheckCactus(int i, int j) 
		{
			return WorldModify.CheckCactus(i, j, ITileAt);
		}

		public void PlantCactus(int i, int j) 
		{
			WorldModify.PlantCactus(i, j);
		}

		public void CactusFrame(int i, int j) 
		{
			WorldModify.CactusFrame(i, j, ITileAt);
		}

		public void GrowCactus(int i, int j) 
		{
			WorldModify.GrowCactus(i, j, ITileAt);
		}

		public void CheckPot(int i, int j, int type = 28) 
		{
			WorldModify.CheckPot(i, j, type, ITileAt);
		}

		public int PlaceChest(int x, int y, int type = 21, bool notNearOtherChests = false, int style = 0) 
		{
			return WorldModify.PlaceChest(x, y, type, notNearOtherChests, style, ITileAt);
		}

		public void CheckChest(int i, int j, int type) 
		{
			WorldModify.CheckChest(i, j, type, ITileAt);
		}

		public void Place1xX(int x, int y, int type, int style = 0) 
		{
			WorldModify.Place1xX(x, y, type, style, ITileAt);
		}

		public void Place2xX(int x, int y, int type, int style = 0) 
		{
			WorldModify.Place2xX(x, y, type, style, ITileAt);
		}

		public void Place3x4(int x, int y, int type) 
		{
			WorldModify.Place3x4(x, y, type, ITileAt);
		}

		public bool PlaceTile(int i, int j, int type, bool mute = false, bool forced = false, int plr = -1, int style = 0) 
		{
			return WorldModify.PlaceTile(i, j, type, mute, forced, plr, style, ITileAt);
		}

		public void KillWall(int i, int j, bool fail = false) 
		{
			WorldModify.KillWall(i, j, ITileAt, fail);
		}

		public void KillTile(int x, int y, bool fail = false, bool effectOnly = false, bool noItem = false, Player player = null) 
		{
			WorldModify.KillTile(x, y, ITileAt, fail, effectOnly, noItem);
		}

		//Remove
		public bool PlayerLOS(int x, int y) 
		{
			return WorldModify.PlayerLOS(x, y);
		}

		public void PlaceWall(int i, int j, int type, bool mute = false) 
		{
			WorldModify.PlaceWall(i, j, type, mute, ITileAt);
		}

		public void SpreadGrass(int i, int j, int dirt = 0, int grass = 2, bool repeat = true) 
		{
			WorldModify.SpreadGrass(i, j, dirt, grass, repeat, ITileAt);
		}

		//remove
		public void SquareTileFrame(int i, int j, bool resetFrame = true) 
		{
			WorldModify.SquareTileFrame(i, j, resetFrame);
		}

		//Remove
		public void SectionTileFrame(int startX, int startY, int endX, int endY) 
		{
			WorldModify.SectionTileFrame(startX, startY, endX, endY);
		}

		//remove
		public void RangeFrame(int startX, int startY, int endX, int endY) 
		{
			WorldModify.RangeFrame(startX, startY, endX, endY);
		}

		public void PlantCheck(int i, int j) 
		{
			WorldModify.PlantCheck(i, j, ITileAt);
		}

		public void TileFrame(int i, int j, bool resetFrame = false, bool noBreak = false)
		{
			WorldModify.TileFrame(i, j, resetFrame, noBreak, ITileAt);
		}
	}
}