
namespace Microsoft.Xna.Framework
{
    public struct Vector4
    {
        public static Vector4[] Array;

        public float W;
        public float X;
        public float Y;
        public float Z;

        private static readonly Vector4 _one = new Vector4(1f, 1f, 1f, 1f);

        public static Vector4 One { get { return _one; } }

        public Vector4(Vector2 value, float z, float w)
        {
            this.W = w;
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
        }

        public Vector4(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Vector4(float value)
        {
            this.W = value;
            this.Z = value;
            this.Y = value;
            this.X = value;
        }

        public static Vector4 Lerp(Vector4 value1, Vector4 value2, float amount)
        {
            Vector4 result;
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
            result.W = value1.W + (value2.W - value1.W) * amount;
            return result;
        }

        public static void Lerp(ref Vector4 value1, ref Vector4 value2, float amount, out Vector4 result)
        {
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
            result.W = value1.W + (value2.W - value1.W) * amount;
        }

        public static Vector4 operator -(Vector4 value)
        {
            Vector4 result;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = -value.W;
            return result;
        }

        public static Vector4 operator *(Vector4 value1, Vector4 value2)
        {
            Vector4 result;
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
            result.W = value1.W * value2.W;
            return result;
        }

        public static Vector4 operator *(Vector4 value1, float scaleFactor)
        {
            Vector4 result;
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
            result.W = value1.W * scaleFactor;
            return result;
        }

        public static Vector4 operator *(float scaleFactor, Vector4 value1)
        {
            Vector4 result;
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
            result.W = value1.W * scaleFactor;
            return result;
        }


        public bool Equals(Vector4 other)
        {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector4)
            {
                var vec = (Vector4)obj;
                return this == vec;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode() + this.Z.GetHashCode() + this.W.GetHashCode();
        }

        public static bool operator ==(Vector4 p1, Vector4 p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z && p1.W == p2.W;
        }

        public static bool operator !=(Vector4 p1, Vector4 p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y || p1.Z != p2.Z || p1.W != p2.W;
        }

        public static Vector4 operator +(Vector4 p1, Vector4 p2)
        {
            Vector4 result;
            result.X = p1.X + p2.X;
            result.Y = p1.Y + p2.Y;
            result.Z = p1.Z + p2.Z;
            result.W = p1.W + p2.W;
            return result;
        }

        public static Vector4 operator -(Vector4 p1, Vector4 p2)
        {
            Vector4 result;
            result.X = p1.X - p2.X;
            result.Y = p1.Y - p2.Y;
            result.Z = p1.Z - p2.Z;
            result.W = p1.W - p2.W;
            return result;
        }
    }
}