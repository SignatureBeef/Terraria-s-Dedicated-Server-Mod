using System;

namespace Terraria_Server
{
	public class TileCollection
	{
		internal TileData[,] data;
     
		public TileCollection (int X, int Y)
		{
			Console.WriteLine ("Creating tile array of {0}x{1}", X, Y);
			data = new TileData [X, Y];
		}
     
		public int SizeX {
			get { return data.GetLength (0); }
		}

		public int SizeY {
			get { return data.GetLength (1); }
		}
     
		public TileRef this [int x, int y] {
			get {
				return new TileRef (x, y);
			}
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

