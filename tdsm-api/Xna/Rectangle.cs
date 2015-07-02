
namespace Microsoft.Xna.Framework
{
    public struct Rectangle
    {
        private static Rectangle _empty = default(Rectangle);

        public static Rectangle Empty
        {
            get
            { return Rectangle._empty; }
        }

        public int Left
        {
            get
            { return this.X; }
        }

        public int Right
        {
            get
            { return this.X + this.Width; }
        }

        public int Top
        {
            get
            { return this.Y; }
        }

        public int Bottom
        {
            get
            { return this.Y + this.Height; }
        }

        public Point Location
        {
            get
            {
                return new Point(this.X, this.Y);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public Point Center
        {
            get
            {
                return new Point(this.X + this.Width / 2, this.Y + this.Height / 2);
            }
        }

        public static Rectangle[] Array;

        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rectangle(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public bool Intersects(Rectangle value)
        {
            return value.X < this.X + this.Width && this.X < value.X + value.Width && value.Y < this.Y + this.Height && this.Y < value.Y + value.Height;
        }

        public bool Contains(Point value)
        {
            return this.X <= value.X && value.X < this.X + this.Width && this.Y <= value.Y && value.Y < this.Y + this.Height;
        }

        public void Offset(Point value)
        {
            this.X += value.X;
            this.Y += value.Y;
        }

        public void Offset(int x, int y)
        {
            this.X += x;
            this.Y += y;
        }

        public void Inflate(int width, int height)
        {
            this.X -= width;
            this.Y -= height;
            this.Width += width * 2;
            this.Height += height * 2;
        }

        public static Rectangle Union(Rectangle r1, Rectangle r2)
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
    }
}
