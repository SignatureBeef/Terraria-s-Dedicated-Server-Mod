
namespace Microsoft.Xna.Framework
{
    public struct Vector4
    {
        public static Vector4[] Array;

        public float W;
        public float X;
        public float Y;
        public float Z;

        public Vector4(Vector2 value, float z, float w)
        {
            this.W = w;
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
        }
    }
}
