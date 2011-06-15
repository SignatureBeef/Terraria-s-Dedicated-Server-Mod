using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2 (float x, float y)
        {
            this = default(Vector2);
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Vector2 operator +(Vector2 value1, Vector2 value2)
        {
            return new Vector2(value1.X + value2.X, value1.Y + value2.Y);
        }

        public static Vector2 operator *(Vector2 value1, Vector2 value2)
        {
            return new Vector2(value1.X * value2.X, value1.Y * value2.Y);
        }

        public static Vector2 operator *(Vector2 value, float scaleFactor)
        {
            return new Vector2(value.X * scaleFactor, value.Y * scaleFactor);
        }

        public static Vector2 operator *(float scaleFactor, Vector2 value)
        {
            return new Vector2(value.X * scaleFactor, value.Y * scaleFactor);
        }

        public static Vector2 operator -(Vector2 value1, Vector2 value2)
        {
            return new Vector2(value1.X - value2.X, value1.Y - value2.Y);
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.X != b.X && a.Y != b.Y;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

    }

}
