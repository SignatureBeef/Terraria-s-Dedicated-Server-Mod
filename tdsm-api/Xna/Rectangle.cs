
namespace Microsoft.Xna.Framework
{
	public struct Rectangle
	{
		private static Rectangle _empty = default(Rectangle);

		public static Rectangle Empty {
			get
            { return Rectangle._empty; }
		}

		public int Left {
			get
            { return this.X; }
		}

		public int Right {
			get
            { return this.X + this.Width; }
		}

		public int Top {
			get
            { return this.Y; }
		}

		public int Bottom {
			get
            { return this.Y + this.Height; }
		}

		public Point Location {
			get {
				return new Point (this.X, this.Y);
			}
			set {
				this.X = value.X;
				this.Y = value.Y;
			}
		}

		public Point Center {
			get {
				return new Point (this.X + this.Width / 2, this.Y + this.Height / 2);
			}
		}

		public static Rectangle[] Array;

		public int X;
		public int Y;
		public int Width;
		public int Height;

		public Rectangle (int x, int y, int width, int height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}

		public void Intersects (ref Rectangle value, out bool result)
		{
			result = (value.X < this.X + this.Width && this.X < value.X + value.Width && value.Y < this.Y + this.Height && this.Y < value.Y + value.Height);
		}

		public bool Intersects (Rectangle value)
		{
			return value.X < this.X + this.Width && this.X < value.X + value.Width && value.Y < this.Y + this.Height && this.Y < value.Y + value.Height;
		}

		public static void Intersect (ref Rectangle value1, ref Rectangle value2, out Rectangle result)
		{
			int r1r = value1.X + value1.Width;
			int r2r = value2.X + value2.Width;
			int r1b = value1.Y + value1.Height;
			int r2b = value2.Y + value2.Height;
			int maxX = (value1.X > value2.X) ? value1.X : value2.X;
			int maxY = (value1.Y > value2.Y) ? value1.Y : value2.Y;
			int minX = (r1r < r2r) ? r1r : r2r;
			int minY = (r1b < r2b) ? r1b : r2b;
			if (minX > maxX && minY > maxY)
			{
				result.X = maxX;
				result.Y = maxY;
				result.Width = minX - maxX;
				result.Height = minY - maxY;
				return;
			}
			result.X = 0;
			result.Y = 0;
			result.Width = 0;
			result.Height = 0;
		}

		public bool Contains (Point value)
		{
			return this.X <= value.X && value.X < this.X + this.Width && this.Y <= value.Y && value.Y < this.Y + this.Height;
		}

		public bool Contains (int x, int y)
		{
			return this.X <= x && x < this.X + this.Width && this.Y <= y && y < this.Y + this.Height;
		}

		public void Contains (ref Point value, out bool result)
		{
			result = (this.X <= value.X && value.X < this.X + this.Width && this.Y <= value.Y && value.Y < this.Y + this.Height);
		}

		public bool Contains (Rectangle value)
		{
			return this.X <= value.X && value.X + value.Width <= this.X + this.Width && this.Y <= value.Y && value.Y + value.Height <= this.Y + this.Height;
		}

		public void Contains (ref Rectangle value, out bool result)
		{
			result = (this.X <= value.X && value.X + value.Width <= this.X + this.Width && this.Y <= value.Y && value.Y + value.Height <= this.Y + this.Height);
		}

		public void Offset (Point value)
		{
			this.X += value.X;
			this.Y += value.Y;
		}

		public void Offset (int x, int y)
		{
			this.X += x;
			this.Y += y;
		}

		public void Inflate (int width, int height)
		{
			this.X -= width;
			this.Y -= height;
			this.Width += width * 2;
			this.Height += height * 2;
		}

		public static Rectangle Union (Rectangle r1, Rectangle r2)
		{
			int num = r1.X + r1.Width;
			int num2 = r2.X + r2.Width;
			int num3 = r1.Y + r1.Height;
			int num4 = r2.Y + r2.Height;
			int num5 = (r1.X < r2.X) ? r1.X : r2.X;
			int num6 = (r1.Y < r2.Y) ? r1.Y : r2.Y;
			int num7 = (num > num2) ? num : num2;
			int num8 = (num3 > num4) ? num3 : num4;
			Rectangle result;
			result.X = num5;
			result.Y = num6;
			result.Width = num7 - num5;
			result.Height = num8 - num6;
			return result;
		}

		public bool Equals (Rectangle other)
		{
			return this.X == other.X && this.Y == other.Y && this.Width == other.Width && this.Height == other.Height;
		}

		public override bool Equals (object obj)
		{
			bool result = false;
			if (obj is Rectangle)
			{
				result = this.Equals ((Rectangle)obj);
			}
			return result;
		}

		public override int GetHashCode ()
		{
			return this.X.GetHashCode () + this.Y.GetHashCode () + this.Width.GetHashCode () + this.Height.GetHashCode ();
		}

		public static bool operator == (Rectangle r1, Rectangle r2)
		{
			return r1.X == r2.X && r1.Y == r2.Y && r1.Width == r2.Width && r1.Height == r2.Height;
		}

		public static bool operator != (Rectangle r1, Rectangle r2)
		{
			return r1.X != r2.X || r1.Y != r2.Y || r1.Width != r2.Width || r1.Height != r2.Height;
		}
	}
}
