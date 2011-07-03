
namespace Terraria_Server
{
	public class Tile
	{
		public bool Active;
		public bool lighted;
		public byte type;
		public byte wall;
		public byte wallFrameX;
		public byte wallFrameY;
		public byte wallFrameNumber;
		public byte liquid;
		public bool checkingLiquid;
		public bool skipLiquid;
		public bool lava;
		public byte frameNumber;
		public short frameX;
        public short frameY;
        public int tileX;
        public int tileY;

		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
