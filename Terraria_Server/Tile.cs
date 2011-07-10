using System;
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
			get { return Exists && Main.tile.data [x, y].Active; }
			set { Main.tile.data [x, y].Active = value; }
		}
		
		public bool Lighted {
			get { return Main.tile.data [x, y].Lighted; }
			set { Main.tile.data [x, y].Lighted = value; }
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
			get { return Main.tile.data [x, y].CheckingLiquid; }
			set { Main.tile.data [x, y].CheckingLiquid = value; }
		}

		public bool SkipLiquid {
			get { return Main.tile.data [x, y].SkipLiquid; }
			set { Main.tile.data [x, y].SkipLiquid = value; }
		}

		public bool Lava {
			get { return Main.tile.data [x, y].Lava; }
			set { Main.tile.data [x, y].Lava = value; }
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
	
	[Flags]
	internal enum TileFlags : byte
	{
		Active = 1,
		Lighted = 2,
		CheckingLiquid = 4,
		SkipLiquid = 8,
		Lava = 16,
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
		internal TileFlags flags;
		
		internal void SetFlag (TileFlags f, bool value)
		{
			if (value)
				flags |= f;
			else
				flags &= ~f;
		}
		
		public bool Active {
			get {
				return (flags & TileFlags.Active) != 0;
			}
			set {
				SetFlag (TileFlags.Active, value);
			}
		}

		public bool CheckingLiquid {
			get {
				return (flags & TileFlags.CheckingLiquid) != 0;
			}
			set {
				SetFlag (TileFlags.CheckingLiquid, value);
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
				return (flags & TileFlags.Lava) != 0;
			}
			set {
				SetFlag (TileFlags.Lava, value);
			}
		}

		public bool Lighted {
			get {
				return (flags & TileFlags.Lighted) != 0;
			}
			set {
				SetFlag (TileFlags.Lighted, value);
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
				return (flags & TileFlags.SkipLiquid) != 0;
			}
			set {
				SetFlag (TileFlags.SkipLiquid, value);
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
