
namespace Microsoft.Xna.Framework
{
    public struct Point
    {
        public static Point[] Array;

        public int X;
        public int Y;

        private static readonly Point _zero;
        public static Point Zero
        {
            get
            {
                return Point._zero;
            }
        }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public static bool operator ==(Point a, Point b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Point a, Point b)
        {
            return a.X != b.X || a.Y != b.Y;
        }
        public override bool Equals(object obj)
        {
            //bool result = false;
            //if (obj is Point)
            //{
            //    result = this.Equals((Point)obj);
            //}
            //return result;

            if (obj is Point)
            {
                var target = (Point)obj;
                return target.X == this.X && target.Y == this.Y;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode();
        }
    }
}
