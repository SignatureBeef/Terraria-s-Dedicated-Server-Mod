using System;


namespace Microsoft.Xna.Framework.Graphics
{
    public class DisplayMode
    {
        public int Width { get; set; }

        public int Height { get; set; }
    }

    public enum BufferUsage
    {
        WriteOnly,
        None
    }

    public class DynamicVertexBuffer : VertexBuffer
    {
        public event EventHandler<EventArgs> ContentLost;

        public DynamicVertexBuffer(
            GraphicsDevice graphicsDevice,
            VertexDeclaration vertexDeclaration,
            int vertexCount,
            BufferUsage usage
        )
        {
        }

        public DynamicVertexBuffer(
            GraphicsDevice graphicsDevice,
            Type vertexType,
            int vertexCount,
            BufferUsage usage
        )
        {
        }

        public void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride, SetDataOptions options) where T : struct
        {
            
        }

        public void SetData<T>(T value)
        {
        }
    }

    public enum IndexElementSize
    {
        ThirtyTwoBits,
        SixteenBits
    }

    public class DynamicIndexBuffer: IndexBuffer
    {
        public event EventHandler<EventArgs> ContentLost;

        public DynamicIndexBuffer(
            GraphicsDevice graphicsDevice,
            IndexElementSize indexElementSize,
            int indexCount,
            BufferUsage usage
        )
        {
        }

        public DynamicIndexBuffer(
            GraphicsDevice graphicsDevice,
            Type indexType,
            int indexCount,
            BufferUsage usage
        )
        {
        }
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

    public class IndexBuffer : GraphicsResource
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
