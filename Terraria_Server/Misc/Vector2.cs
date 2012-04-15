
namespace Terraria_Server.Misc
{
    public struct Vector2
    {
        public float X;
		public float Y;

		public Vector2(float x, float y)
		{
			this = default(Vector2);
			this.X = x;
			this.Y = y;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static Vector2 operator + (Vector2 a, Vector2 b)
		{
			return new Vector2(a.X + b.X, a.Y + b.Y);
		}

		public static Vector2 operator - (Vector2 a, Vector2 b)
		{
			return new Vector2(a.X - b.X, a.Y - b.Y);
		}

        public static Vector2 operator * (Vector2 a, float b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }

        public static Vector2 operator / (Vector2 a, float b)
        {
            return new Vector2(a.X / b, a.Y / b);
        }

		public static bool operator == (Vector2 a, Vector2 b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

        public static bool operator != (Vector2 value1, Vector2 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y;
        }

        /* i only need to worry about the X in Terraria :3 */
        public static bool operator <= (Vector2 value1, Vector2 value2)
        {
            return value1.X <= value2.X; // || value1.Y <= value2.Y;
        }

        public static bool operator >= (Vector2 value1, Vector2 value2)
        {
            return value1.X >= value2.X; // || value1.Y >= value2.Y;
        }

        public static bool operator < (Vector2 value1, Vector2 value2)
        {
            return value1.X < value2.X; // || value1.Y < value2.Y;
        }

        public static bool operator > (Vector2 value1, Vector2 value2)
        {
            return value1.X > value2.X; // || value1.Y > value2.Y;
        }
	}
}
