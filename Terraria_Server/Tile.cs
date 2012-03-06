using System;
using System.Runtime.InteropServices;

namespace Terraria_Server
{
	public interface ITile
	{
		bool Active { get; }

		bool Exists { get; }

		void SetActive(bool value);

		bool Lighted { get; }

		void SetLighted(bool value);

		byte Type { get; }

		void SetType(byte value);

		byte Wall { get; }

		void SetWall(byte value);

		byte Liquid { get; }

		void SetLiquid(byte value);

		void AddLiquid(int value);

		bool CheckingLiquid { get; }

		void SetCheckingLiquid(bool value);

		bool SkipLiquid { get; }

		void SetSkipLiquid(bool value);

		bool Lava { get; }

		void SetLava(bool value);

		byte FrameNumber { get; }

		void SetFrameNumber(byte value);

		short FrameX { get; }

		void SetFrameX(short value);

		void AddFrameX(int value);

		short FrameY { get; }

		void SetFrameY(short value);

		void AddFrameY(int value);

		bool Wire { get; }

		void SetWire(bool value);

		byte WallFrameX { get; }

		void SetWallFrameX(byte value);

		byte WallFrameY { get; }

		void SetWallFrameY(byte value);

		//byte WallFrameNumber { get; }

		//void SetWallFrameNumber(byte value);
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TileRef : ITile
	{
		private readonly short x;
		private readonly short y;

		public TileRef(int X, int Y)
		{
			x = (short)X;
			y = (short)Y;
		}

		public TileData Data
		{
			get { return Main.tile.data[x, y]; }
		}

		public void SetData(TileData value)
		{
			//if (Netplay.anyClients && value.type == 53 && Main.tile.data [x, y].type != 53) Logging.ProgramLog.Debug.Log (System.Environment.StackTrace);
			Main.tile.data[x, y] = value;
		}

		public bool Exists
		{
			get
			{
				return x >= 0 && y >= 0 && x < Main.tile.data.GetLength(0) && y < Main.tile.data.GetLength(1);
			}
		}

		public bool Active
		{
			get { return Exists && Main.tile.data[x, y].Active; }
		}

		public void SetActive(bool value)
		{
			Main.tile.data[x, y].Active = value;
		}

		public bool Lighted
		{
			get { return Main.tile.data[x, y].Lighted; }
		}

		public void SetLighted(bool value)
		{
			Main.tile.data[x, y].Lighted = value;
		}

		public byte Type
		{
			get { return Main.tile.data[x, y].type; }
		}

		public void SetType(byte value)
		{
			//if (Netplay.anyClients && value == 53) Logging.ProgramLog.Debug.Log (System.Environment.StackTrace);
			Main.tile.data[x, y].type = value;
		}

		public byte Wall
		{
			get { return Main.tile.data[x, y].Wall; }
		}

		public void SetWall(byte value)
		{
			Main.tile.data[x, y].Wall = value;
		}

		public byte Liquid
		{
			get { return Main.tile.data[x, y].liquid; }
		}

		public void SetLiquid(byte value)
		{
			Main.tile.data[x, y].liquid = value;
		}

		public void AddLiquid(int value)
		{
			Main.tile.data[x, y].liquid = (byte)(Main.tile.data[x, y].liquid + value);
		}

		public bool CheckingLiquid
		{
			get { return Main.tile.data[x, y].CheckingLiquid; }
		}

		public void SetCheckingLiquid(bool value)
		{
			Main.tile.data[x, y].CheckingLiquid = value;
		}

		public bool SkipLiquid
		{
			get { return Main.tile.data[x, y].SkipLiquid; }
		}

		public void SetSkipLiquid(bool value)
		{
			Main.tile.data[x, y].SkipLiquid = value;
		}

		public bool Lava
		{
			get { return Main.tile.data[x, y].Lava; }
		}

		public void SetLava(bool value)
		{
			Main.tile.data[x, y].Lava = value;
		}

		public byte FrameNumber
		{
			get { return Main.tile.data[x, y].FrameNumber; }
		}

		public void SetFrameNumber(byte value)
		{
			Main.tile.data[x, y].FrameNumber = value;
		}

		public short FrameX
		{
			get { return Main.tile.data[x, y].FrameX; }
		}

		public void SetFrameX(short value)
		{
			Main.tile.data[x, y].FrameX = value;
		}

		public void AddFrameX(int value)
		{
			Main.tile.data[x, y].FrameX = (short)(Main.tile.data[x, y].FrameX + value);
		}

		public short FrameY
		{
			get { return Main.tile.data[x, y].FrameY; }
		}

		public void SetFrameY(short value)
		{
			Main.tile.data[x, y].FrameY = value;
		}

		public void AddFrameY(int value)
		{
			Main.tile.data[x, y].FrameY = (short)(Main.tile.data[x, y].FrameY + value);
		}

		public bool Wire
		{
			get { return Main.tile.data[x, y].Wire; }
		}

		public void SetWire(bool value)
		{
			Main.tile.data[x, y].Wire = value;
		}

		public byte WallFrameX
		{
			get { return Main.tile.data[x, y].WallFrameX; }
		}

		public void SetWallFrameX(byte value)
		{
			Main.tile.data[x, y].WallFrameX = value;
		}

		public byte WallFrameY
		{
			get { return Main.tile.data[x, y].WallFrameY; }
		}

		public void SetWallFrameY(byte value)
		{
			Main.tile.data[x, y].WallFrameY = value;
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
		Wire = 32
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TileData
	{
		//		static byte[] frameEnc = new byte [65535];
		//		static byte[] frameDec = new byte [255];
		//		static int framesEncoded;
		//		static Dictionary<int, int> frameOverflow = new Dictionary<int, int> ();


		internal TileFlags flags;
		internal byte type;
		byte wall;
		byte frameX;
		byte frameY;
		internal byte liquid;

		//internal byte wallFrameNumber;
		internal byte wallFrameX;
		internal byte wallFrameY;

		internal void SetFlag(TileFlags f, bool value)
		{
			if (value)
				flags |= f;
			else
				flags &= ~f;
		}

		public bool Active
		{
			get
			{
				return (flags & TileFlags.Active) != 0;
			}
			set
			{
				SetFlag(TileFlags.Active, value);
			}
		}

		public bool CheckingLiquid
		{
			get
			{
				return (flags & TileFlags.CheckingLiquid) != 0;
			}
			set
			{
				SetFlag(TileFlags.CheckingLiquid, value);
			}
		}

		public byte FrameNumber
		{
			get
			{
				return (byte)(wall >> 6);
			}
			set
			{
				wall = (byte)((wall & 63) | (value << 6));
			}
		}

		private byte frameX_Size;
		public short FrameX
		{
			get
			{
				if (frameX == 255)
					return -1;
				else
				{
					if (frameX_Size > 0)
						return (short)((frameX << 3) + (frameX_Size - 1));
					else
						return (short)(frameX << 1);
				}
			}
			set
			{
				frameX_Size = 0;
				if (value == -1)
					frameX = 255;
				else
				{
					if (value > 255)
					{
						frameX = (byte)(value >> 3);
						frameX_Size = (byte)(value - (value >> 3 << 3) + 1); //Get's how many left over Because frameX should be 1026 but when shifted and reverted it = 1024
					}
					else
						frameX = (byte)(value >> 1);
				}
			}
		}

		public short FrameY
		{
			get
			{
				if (frameY == 255)
					return -1;
				else
					return (short)(frameY << 1);
			}
			set
			{
				if (value == -1)
					frameY = 255;
				else
					frameY = (byte)(value >> 1);
			}
		}

		public bool Lava
		{
			get
			{
				return (flags & TileFlags.Lava) != 0;
			}
			set
			{
				SetFlag(TileFlags.Lava, value);
			}
		}

		public bool Lighted
		{
			get
			{
				return (flags & TileFlags.Lighted) != 0;
			}
			set
			{
				SetFlag(TileFlags.Lighted, value);
			}
		}

		public byte Liquid
		{
			get
			{
				return liquid;
			}
			set
			{
				liquid = value;
			}
		}

		public bool SkipLiquid
		{
			get
			{
				return (flags & TileFlags.SkipLiquid) != 0;
			}
			set
			{
				SetFlag(TileFlags.SkipLiquid, value);
			}
		}

		public byte Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public byte Wall
		{
			get
			{
				return (byte)(wall & 63);
			}
			set
			{
				wall = (byte)((wall & ~63) | (value & 63));
			}
		}

		public bool Wire
		{
			get
			{
				return (flags & TileFlags.Wire) != 0;
			}
			set
			{
				SetFlag(TileFlags.Wire, value);
			}
		}

		public byte WallFrameX
		{
			get { return wallFrameX; }
			set { wallFrameX = value; }
		}

		public byte WallFrameY
		{
			get { return wallFrameY; }
			set { wallFrameY = value; }
		}
	}
}
