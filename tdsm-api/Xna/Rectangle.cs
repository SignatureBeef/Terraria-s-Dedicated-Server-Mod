
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
    }
}
