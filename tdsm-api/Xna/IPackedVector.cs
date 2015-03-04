
namespace Microsoft.Xna.Framework
{
    public interface IPackedVector
    {
        /// <summary>Expands the packed representation into a Vector4.</summary>
        Vector4 ToVector4();
        /// <summary>Sets the packed representation from a Vector4.</summary>
        /// <param name="vector">The vector to create the packed representation from.</param>
        void PackFromVector4(Vector4 vector);
    }
    public interface IPackedVector<TPacked> : IPackedVector
    {
        /// <summary>Directly gets or sets the packed representation of the value.</summary>
        TPacked PackedValue
        {
            get;
            set;
        }
    }
}
