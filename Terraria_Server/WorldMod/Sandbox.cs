using System;
using System.Collections.Generic;

namespace Terraria_Server.WorldMod
{
	public struct NullSandbox : ISandbox
	{
		public void Initialize ()
		{
		}
		
		public void AddWater         (int x, int y)
		{
		}
		
		public void ShadowOrbSmashed (int x, int y)
		{
		}

		public void NewItem(int x, int y, int w, int h, int type, int stack = 1, bool noBroadcast = false, int pfix = 0, int NetID = 255)
		{
		}
		
		public void KillSign         (int x, int y)
		{
		}
		
		public void DestroyChest     (int x, int y)
		{
		}
		
		public void FallingBlockProjectile (int x, int y, int type)
		{
		}
		
		public bool ActiveAt (int x, int y)
		{
			return Main.tile.At(x, y).Active;
		}
		
		public bool LitAt    (int x, int y)
		{
			return Main.tile.At(x, y).Lighted;
		}
		
		public bool LavaAt   (int x, int y)
		{
			return Main.tile.At(x, y).Lava;
		}
		
		public byte TypeAt   (int x, int y)
		{
			return Main.tile.At(x, y).Type;
		}
		
		public byte WallAt   (int x, int y)
		{
			return Main.tile.At(x, y).Wall;
		}
		
		public byte Liquid (int x, int y)
		{
			return Main.tile.At(x, y).Liquid;
		}
		
		public byte FrameAt  (int x, int y)
		{
			return Main.tile.At(x, y).FrameNumber;
		}
		
		public short FrameXAt (int x, int y)
		{
			return Main.tile.At(x, y).FrameX;
		}

		public short FrameYAt(int x, int y)
		{
			return Main.tile.At(x, y).FrameY;
		}

		public bool Wire(int x, int y)
		{
			return Main.tile.At(x, y).Wire;
		}
		
		public void SetActiveAt (int x, int y, bool val)
		{
		}
		
		public void SetLitAt    (int x, int y, bool val)
		{
		}
		
		public void SetLavaAt   (int x, int y, bool val)
		
		{
		}
		
		public void SetTypeAt   (int x, int y, byte val)
		{
		}
		
		public void SetWallAt   (int x, int y, byte val)
		{
		}
		
		public void SetLiquid (int x, int y, byte val)
		{
		}
		
		public void SetFrameAt  (int x, int y, byte val)
		{
		}
		
		public void SetFrameXAt (int x, int y, short val)
		{
		}
		
		public void SetFrameYAt (int x, int y, short val)
		{
		}

		public void SetWire(int x, int y, bool val)
		{

		}
		
		public void SetSkipLiquid(int x, int y, bool val)
		{

		}

		public bool SkipLiquid(int x, int y)
		{
			return Main.tile.At(x, y).SkipLiquid;
		}

		public void SetCheckingLiquid(int x, int y, bool val)
		{

		}

		public bool CheckingLiquid(int x, int y)
		{
			return Main.tile.At(x, y).CheckingLiquid;
		}

		public void AddLiquid(int x, int y, int val)
		{

		}

		public void AddFrameX(int x, int y, int val)
		{

		}

		public void AddFrameY(int x, int y, int val)
		{

		}


		public bool Exists(int x, int y)
		{
			return Main.tile.At(x, y).Exists;
		}
	}
	
	public struct TestSandbox : ISandbox
	{
		class ChangedTile
		{
			public TileData data;
			
			public ChangedTile (int x, int y)
			{
				data = Main.tile.data[x, y];
			}
		}
		
		Dictionary<int, ChangedTile> changedTiles;
		
		public void Initialize ()
		{
			changedTiles = new Dictionary<int, ChangedTile> ();
		}
		
		public IEnumerable<int> ChangedTiles ()
		{
			foreach (var kv in changedTiles)
			{
				yield return kv.Key;
			}
		}
		
		public void AddWater         (int x, int y)
		{
		}
		
		public void ShadowOrbSmashed (int x, int y)
		{
		}

		public void NewItem(int x, int y, int w, int h, int type, int stack = 1, bool noBroadcast = false, int pfix = 0, int NetID = 255)
		{
		}
		
		public void KillSign         (int x, int y)
		{
		}
		
		public void DestroyChest     (int x, int y)
		{
		}
		
		public void FallingBlockProjectile (int x, int y, int type)
		{
		}
		
		public bool ActiveAt (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Active;
			return Main.tile.At(x, y).Active;
		}
		
		public bool LitAt    (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Lighted;
			return Main.tile.At(x, y).Lighted;
		}
		
		public bool LavaAt   (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Lava;
			return Main.tile.At(x, y).Lava;
		}
		
		public byte TypeAt   (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Type;
			return Main.tile.At(x, y).Type;
		}
		
		public byte WallAt   (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Wall;
			return Main.tile.At(x, y).Wall;
		}
		
		public byte Liquid (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Liquid;
			return Main.tile.At(x, y).Liquid;
		}
		
		public byte FrameAt  (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.FrameNumber;
			return Main.tile.At(x, y).FrameNumber;
		}
		
		public short FrameXAt (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.FrameX;
			return Main.tile.At(x, y).FrameX;
		}
		
		public short FrameYAt (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.FrameY;
			return Main.tile.At(x, y).FrameY;
		}
		
		public void SetActiveAt (int x, int y, bool val)
		{
			ChangedTile tile;
			if (! changedTiles.TryGetValue ((x << 16) | y, out tile))
			{
				tile = new ChangedTile (x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			
			Logging.ProgramLog.Debug.Log ("{0}, {1} Active = {2}", x, y, val);
			
			tile.data.Active = val;
		}
		
		public void SetLitAt    (int x, int y, bool val)
		{
			ChangedTile tile;
			if (! changedTiles.TryGetValue ((x << 16) | y, out tile))
			{
				tile = new ChangedTile (x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			Logging.ProgramLog.Debug.Log ("{0}, {1} Lit = {2}", x, y, val);
			tile.data.Lighted = val;
		}
		
		public void SetLavaAt   (int x, int y, bool val)
		{
			ChangedTile tile;
			if (! changedTiles.TryGetValue ((x << 16) | y, out tile))
			{
				tile = new ChangedTile (x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			Logging.ProgramLog.Debug.Log ("{0}, {1} Lava = {2}", x, y, val);
			tile.data.Lava = val;
		}
		
		public void SetTypeAt   (int x, int y, byte val)
		{
			ChangedTile tile;
			if (! changedTiles.TryGetValue ((x << 16) | y, out tile))
			{
				tile = new ChangedTile (x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			Logging.ProgramLog.Debug.Log ("{0}, {1} Type = {2}", x, y, val);
			tile.data.Type = val;
		}
		
		public void SetWallAt   (int x, int y, byte val)
		{
			ChangedTile tile;
			if (! changedTiles.TryGetValue ((x << 16) | y, out tile))
			{
				tile = new ChangedTile (x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			Logging.ProgramLog.Debug.Log ("{0}, {1} Wall = {2}", x, y, val);
			tile.data.Wall = val;
		}
		
		public void SetLiquid (int x, int y, byte val)
		{
			ChangedTile tile;
			if (! changedTiles.TryGetValue ((x << 16) | y, out tile))
			{
				tile = new ChangedTile (x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			Logging.ProgramLog.Debug.Log ("{0}, {1} Liquid = {2}", x, y, val);
			tile.data.Liquid = val;
		}
		
		public void SetFrameAt  (int x, int y, byte val)
		{
			ChangedTile tile;
			if (! changedTiles.TryGetValue ((x << 16) | y, out tile))
			{
				tile = new ChangedTile (x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			Logging.ProgramLog.Debug.Log ("{0}, {1} Frame = {2}", x, y, val);
			tile.data.FrameNumber = val;
		}
		
		public void SetFrameXAt (int x, int y, short val)
		{
			ChangedTile tile;
			if (! changedTiles.TryGetValue ((x << 16) | y, out tile))
			{
				tile = new ChangedTile (x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			Logging.ProgramLog.Debug.Log ("{0}, {1} FrameX = {2}", x, y, val);
			tile.data.FrameX = val;
		}
		
		public void SetFrameYAt (int x, int y, short val)
		{
			ChangedTile tile;
			if (! changedTiles.TryGetValue ((x << 16) | y, out tile))
			{
				tile = new ChangedTile (x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			Logging.ProgramLog.Debug.Log ("{0}, {1} FrameY = {2}", x, y, val);
			tile.data.FrameY = val;
		}

		public bool Wire(int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Wire;
			return Main.tile.At(x, y).Wire;
		}

		public void SetWire(int x, int y, bool val)
		{
			ChangedTile tile;
			if (!changedTiles.TryGetValue((x << 16) | y, out tile))
			{
				tile = new ChangedTile(x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			Logging.ProgramLog.Debug.Log("{0}, {1} SetWire = {2}", x, y, val);
			tile.data.Wire = val;
		}

		public void SetSkipLiquid(int x, int y, bool val)
		{
			ChangedTile tile;
			if (!changedTiles.TryGetValue((x << 16) | y, out tile))
			{
				tile = new ChangedTile(x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			Logging.ProgramLog.Debug.Log("{0}, {1} SetSkipLiquid = {2}", x, y, val);
			tile.data.SkipLiquid = val;
		}

		public bool SkipLiquid(int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.SkipLiquid;
			return Main.tile.At(x, y).SkipLiquid;
		}

		public void SetCheckingLiquid(int x, int y, bool val)
		{
			ChangedTile tile;
			if (!changedTiles.TryGetValue((x << 16) | y, out tile))
			{
				tile = new ChangedTile(x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			Logging.ProgramLog.Debug.Log("{0}, {1} SetCheckingLiquid = {2}", x, y, val);
			tile.data.CheckingLiquid = val;
		}

		public bool CheckingLiquid(int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.CheckingLiquid;
			return Main.tile.At(x, y).CheckingLiquid;
		}

		public void AddLiquid(int x, int y, int val)
		{

		}

		public void AddFrameX(int x, int y, int val)
		{

		}

		public void AddFrameY(int x, int y, int val)
		{

		}


		public bool Exists(int x, int y)
		{
			return changedTiles.ContainsKey((x << 16) | y);
		}
	}
		
	public struct BBSandbox : ISandbox
	{
		class ChangedTile
		{
			public TileData data;
			
			public ChangedTile (int x, int y)
			{
				data = Main.tile.data[x, y];
			}
		}
		
		Dictionary<int, ChangedTile> changedTiles;
		
		int left;
		int top;
		int right;
		int bottom;
		
		public void Initialize ()
		{
			changedTiles = new Dictionary<int, ChangedTile> ();
			changedRows = new Dictionary<int, MinMax> ();
			
			left = short.MaxValue;
			top = short.MaxValue;
			right = 0;
			bottom = 0;
		}
		
		public IEnumerable<int> ChangedTiles ()
		{
			foreach (var kv in changedTiles)
			{
				yield return kv.Key;
			}
		}
		
		public void AddWater         (int x, int y)
		{
		}
		
		public void ShadowOrbSmashed (int x, int y)
		{
		}

		public void NewItem(int x, int y, int w, int h, int type, int stack = 1, bool noBroadcast = false, int pfix = 0, int NetID = 255)
		{
		}
		
		public void KillSign         (int x, int y)
		{
		}
		
		public void DestroyChest     (int x, int y)
		{
		}
		
		public void FallingBlockProjectile (int x, int y, int type)
		{
		}
		
		public bool ActiveAt (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Active;
			return Main.tile.At(x, y).Active;
		}
		
		public bool LitAt    (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Lighted;
			return Main.tile.At(x, y).Lighted;
		}
		
		public bool LavaAt   (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Lava;
			return Main.tile.At(x, y).Lava;
		}
		
		public byte TypeAt   (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Type;
			return Main.tile.At(x, y).Type;
		}
		
		public byte WallAt   (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Wall;
			return Main.tile.At(x, y).Wall;
		}
		
		public byte Liquid (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Liquid;
			return Main.tile.At(x, y).Liquid;
		}
		
		public byte FrameAt  (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.FrameNumber;
			return Main.tile.At(x, y).FrameNumber;
		}
		
		public short FrameXAt (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.FrameX;
			return Main.tile.At(x, y).FrameX;
		}
		
		public short FrameYAt (int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.FrameY;
			return Main.tile.At(x, y).FrameY;
		}
		
		ChangedTile Change (int x, int y)
		{
			ChangedTile tile;
			if (! changedTiles.TryGetValue ((x << 16) | y, out tile))
			{
				tile = new ChangedTile (x, y);
				changedTiles[(x << 16) | y] = tile;
			}
			
			if (x < left) left = x;
			if (x > right) right = x;
			if (y < top) top = y;
			if (y > bottom) bottom = y;
			
			MinMax row;
			if (! changedRows.TryGetValue (y, out row))
			{
				changedRows[y] = new MinMax { Min = x, Max = x };
			}
			else
			{
				changedRows[y] = new MinMax { Min = Math.Min(x, row.Min), Max = Math.Max (x, row.Max) };
			}
			
			if (lastX != x || lastY != y)
			{
				Logging.ProgramLog.Debug.Log ("changed");
				lastX = x;
				lastY = y;
			}
			
			return tile;
		}
		
		int lastX;
		int lastY;
		
		public int Left { get { return left; } }
		public int Top { get { return top; } }
		public int Size { get { return Math.Max (right - left + 1, bottom - top + 1); } }
		
		public struct MinMax
		{
			public int Min;
			public int Max;
		}
		
		public Dictionary<int, MinMax> changedRows;
		
		public void SetActiveAt (int x, int y, bool val)
		{
			var tile = Change (x, y);
			tile.data.Active = val;
		}
		
		public void SetLitAt    (int x, int y, bool val)
		{
			var tile = Change (x, y);
			tile.data.Lighted = val;
		}
		
		public void SetLavaAt   (int x, int y, bool val)
		{
			var tile = Change (x, y);
			tile.data.Lava = val;
		}
		
		public void SetTypeAt   (int x, int y, byte val)
		{
			var tile = Change (x, y);
			tile.data.Type = val;
		}
		
		public void SetWallAt   (int x, int y, byte val)
		{
			var tile = Change (x, y);
			tile.data.Wall = val;
		}
		
		public void SetLiquid (int x, int y, byte val)
		{
			var tile = Change (x, y);
			tile.data.Liquid = val;
		}
		
		public void SetFrameAt  (int x, int y, byte val)
		{
			var tile = Change (x, y);
			tile.data.FrameNumber = val;
		}
		
		public void SetFrameXAt (int x, int y, short val)
		{
			var tile = Change (x, y);
			tile.data.FrameX = val;
		}
		
		public void SetFrameYAt (int x, int y, short val)
		{
			var tile = Change (x, y);
			tile.data.FrameY = val;
		}


		public bool Wire(int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.Wire;
			return Main.tile.At(x, y).Wire;
		}

		public void SetWire(int x, int y, bool val)
		{
			var tile = Change(x, y);
			tile.data.Wire = val;
		}

		public void SetSkipLiquid(int x, int y, bool val)
		{
			var tile = Change(x, y);
			tile.data.SkipLiquid = val;
		}

		public bool SkipLiquid(int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.SkipLiquid;
			return Main.tile.At(x, y).SkipLiquid;
		}

		public void SetCheckingLiquid(int x, int y, bool val)
		{
			var tile = Change(x, y);
			tile.data.CheckingLiquid = val;
		}

		public bool CheckingLiquid(int x, int y)
		{
			ChangedTile tile;
			if (changedTiles.TryGetValue((x << 16) | y, out tile))
				return tile.data.CheckingLiquid;
			return Main.tile.At(x, y).CheckingLiquid;
		}

		public void AddLiquid(int x, int y, int val)
		{

		}

		public void AddFrameX(int x, int y, int val)
		{

		}

		public void AddFrameY(int x, int y, int val)
		{

		}


		public bool Exists(int x, int y)
		{
			return changedTiles.ContainsKey((x << 16) | y);
		}
	}
}

