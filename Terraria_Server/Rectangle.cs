using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class Rectangle
    {

        public int X = 0, Y = 0, Width = 0, Height = 0;

        public Rectangle() { }

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Intersects(Rectangle value)
        {
            return ((((value.X < (this.X + this.Width)) &&
                      (this.X < (value.X + value.Width))) &&
                      (value.Y < (this.Y + this.Height))) &&
                      (this.Y < (value.Y + value.Height)));
        } 
    }
}
