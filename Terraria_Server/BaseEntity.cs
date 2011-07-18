using System;
using Terraria_Server.Misc;
using Terraria_Server.Collections;

namespace Terraria_Server
{
    public abstract class BaseEntity : IRegisterableEntity
    {
        public bool Active { get; set; }
        public int Height { get; set; }
        public String Name { get; set; }
        public Vector2 Position;
        public virtual int Type { get; set; }
        public virtual int Inherits { get; set; }
        public int Width { get; set; }

        public abstract object Clone();

        public bool Intersects(Rectangle rectangle)
        {
            return rectangle.Intersects(new Rectangle((int)Position.X, (int)Position.Y, Width, Height));
        }
    }
}
