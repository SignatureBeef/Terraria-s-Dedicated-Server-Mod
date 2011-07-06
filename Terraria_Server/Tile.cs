using System.Runtime.InteropServices;

namespace Terraria_Server
{
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct TileRef
	{
		private readonly short x;
		private readonly short y;
		
		public TileRef (int X, int Y)
		{
			x = (short)X;
			y = (short)Y;
		}
  
		public TileData Data {
			get { return Main.tile.data [x, y]; }
			set { Main.tile.data [x, y] = value; }
		}
		
		public bool Exists {
			get {
				return x >= 0 && y >= 0 && x < Main.tile.data.GetLength (0) && y < Main.tile.data.GetLength (1);
			}
		}
		
		public bool Active {
			get { return Main.tile.data [x, y].active; }
			set { Main.tile.data [x, y].active = value; }
		}
		
		public bool Lighted {
			get { return Main.tile.data [x, y].lighted; }
			set { Main.tile.data [x, y].lighted = value; }
		}

		public byte Type {
			get { return Main.tile.data [x, y].type; }
			set { Main.tile.data [x, y].type = value; }
		}

		public byte Wall {
			get { return Main.tile.data [x, y].wall; }
			set { Main.tile.data [x, y].wall = value; }
		}

		public byte WallFrameX {
			get { return Main.tile.data [x, y].wallFrameX; }
			set { Main.tile.data [x, y].wallFrameX = value; }
		}

		public byte WallFrameY {
			get { return Main.tile.data [x, y].wallFrameY; }
			set { Main.tile.data [x, y].wallFrameY = value; }
		}

		public byte WallFrameNumber {
			get { return Main.tile.data [x, y].wallFrameNumber; }
			set { Main.tile.data [x, y].wallFrameNumber = value; }
		}

		public byte Liquid {
			get { return Main.tile.data [x, y].liquid; }
			set { Main.tile.data [x, y].liquid = value; }
		}

		public bool CheckingLiquid {
			get { return Main.tile.data [x, y].checkingLiquid; }
			set { Main.tile.data [x, y].checkingLiquid = value; }
		}

		public bool SkipLiquid {
			get { return Main.tile.data [x, y].skipLiquid; }
			set { Main.tile.data [x, y].skipLiquid = value; }
		}

		public bool Lava {
			get { return Main.tile.data [x, y].lava; }
			set { Main.tile.data [x, y].lava = value; }
		}

		public byte FrameNumber {
			get { return Main.tile.data [x, y].frameNumber; }
			set { Main.tile.data [x, y].frameNumber = value; }
		}

		public short FrameX {
			get { return Main.tile.data [x, y].frameX; }
			set { Main.tile.data [x, y].frameX = value; }
		}

		public short FrameY {
			get { return Main.tile.data [x, y].frameY; }
			set { Main.tile.data [x, y].frameY = value; }
		}
	}
	
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct TileData
	{
		internal short frameX;
		internal short frameY;
		internal byte type;
		internal byte wall;
		internal byte wallFrameX;
		internal byte wallFrameY;
		internal byte wallFrameNumber;
		internal byte liquid;
		internal byte frameNumber;
		internal bool active;
		internal bool lighted;
		internal bool checkingLiquid;
		internal bool skipLiquid;
		internal bool lava;

		public bool Active {
			get {
				return this.active;
			}
			set {
				active = value;
			}
		}

		public bool CheckingLiquid {
			get {
				return this.checkingLiquid;
			}
			set {
				checkingLiquid = value;
			}
		}

		public byte FrameNumber {
			get {
				return this.frameNumber;
			}
			set {
				frameNumber = value;
			}
		}

		public short FrameX {
			get {
				return this.frameX;
			}
			set {
				frameX = value;
			}
		}

		public short FrameY {
			get {
				return this.frameY;
			}
			set {
				frameY = value;
			}
		}

		public bool Lava {
			get {
				return this.lava;
			}
			set {
				lava = value;
			}
		}

		public bool Lighted {
			get {
				return this.lighted;
			}
			set {
				lighted = value;
			}
		}

		public byte Liquid {
			get {
				return this.liquid;
			}
			set {
				liquid = value;
			}
		}

		public bool SkipLiquid {
			get {
				return this.skipLiquid;
			}
			set {
				skipLiquid = value;
			}
		}

		public byte Type {
			get {
				return this.type;
			}
			set {
				type = value;
			}
		}

		public byte Wall {
			get {
				return this.wall;
			}
			set {
				wall = value;
			}
		}

		public byte WallFrameNumber {
			get {
				return this.wallFrameNumber;
			}
			set {
				wallFrameNumber = value;
			}
		}

		public byte WallFrameX {
			get {
				return this.wallFrameX;
			}
			set {
				wallFrameX = value;
			}
		}

		public byte WallFrameY {
			get {
				return this.wallFrameY;
			}
			set {
				wallFrameY = value;
			}
		}
	}
}
