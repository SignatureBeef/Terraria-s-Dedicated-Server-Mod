using System;
using System.Runtime.InteropServices;

using Terraria_Server.Logging;

namespace Terraria_Server
{
	public class TileCollection
	{
		internal TileData[,] data;
     
		public TileCollection (int X, int Y)
		{
			ProgramLog.Log("{3} {0}x{1}, {2}MB", X, Y, Marshal.SizeOf(typeof(TileData)) * X * Y / 1024 / 1024, Language.Languages.CreatingTileArrayOf);
			data = new TileData [X, Y];
		}
     
		public int SizeX {
			get { return data.GetLength (0); }
		}

		public int SizeY {
			get { return data.GetLength (1); }
		}

		public TileRef At(int x, int y)
		{
			return new TileRef(x, y);
		}

		public static ITile ITileAt(int x, int y)
		{
			return new TileRef(x, y);
		}
     
		public TileRef CreateTileAt (int x, int y)
		{
			data [x, y] = default(TileData);
			return new TileRef (x, y);
		}
     
		public void RemoveTileAt (int x, int y)
		{
			data [x, y] = default(TileData);
		}
	}
}

