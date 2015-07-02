using System;


namespace Microsoft.Xna.Framework.Graphics
{
    public class DisplayMode
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }

    public class DynamicVertexBuffer : VertexBuffer
    {
        public event EventHandler<EventArgs> ContentLost;

        public void SetData<T>(T value)
        {
        }
    }

    public class DynamicIndexBuffer: IndexBuffer
    {
        public event EventHandler<EventArgs> ContentLost;
    }

    public interface IVertexType
    {

    }

    public class VertexDeclaration : GraphicsResource
    {

    }

    public struct VertexPositionColorTexture : IVertexType
    {
        public Color Color;
        public Vector3 Position;
        public Vector2 TextureCoordinate;
        public static readonly VertexDeclaration VertexDeclaration;
    }

    public class IndexBuffer
    {
        public void SetData<T>(
            int offsetInBytes,
            T[] data,
            int startIndex,
            int elementCount
        )
        {
        }

        public void SetData<T>(
            T[] data
        )
        {
        }

        public void SetData<T>(
            T[] data,
            int startIndex,
            int elementCount
        )
        {

        }
    }
}
