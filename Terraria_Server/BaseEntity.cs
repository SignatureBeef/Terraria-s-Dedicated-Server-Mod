using System;
using Terraria_Server.Misc;
using Terraria_Server.Collections;

namespace Terraria_Server
{
    public abstract class BaseEntity : IRegisterableEntity
    {
		public bool Active { get; set; }
        public int Height { get; set; }
		public string Name { get; set; }
        public Vector2 Position;
        public virtual int Type { get; set; }
		public virtual int aiStyle { get; set; }
		public virtual int damage { get; set; }
        public virtual int defense { get; set; }
        public virtual int lifeMax { get; set; }
        public virtual int life { get; set; }
        public virtual float scale { get; set; }
        public virtual float knockBackResist { get; set; }
        public virtual float slots { get; set; }
        public virtual int NetID { get; set; }// why are these virtual anyway...
        public virtual int Width { get; set; }

        public abstract object Clone();

        public bool Intersects(Rectangle rectangle)
        {
            return rectangle.Intersects(new Rectangle((int)Position.X, (int)Position.Y, Width, Height));
        }
    }
}
