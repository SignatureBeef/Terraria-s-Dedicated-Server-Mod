
namespace Microsoft.Xna.Framework.Graphics
{
    public class GraphicsDevice
    {
        public void Clear(Color colour)
        {
        }

        public void SetRenderTarget(RenderTarget2D renderTarget)
        {
        }

        public DepthStencilState DepthStencilState { get; set; }

        public PresentationParameters PresentationParameters { get; set; }

        public Viewport Viewport { get; set; }

        public TextureCollection Textures { get; set; }

        public RasterizerState RasterizerState { get; set; }

        public Rectangle ScissorRectangle { get; set; }

        public IndexBuffer Indices { get; set; }

        public SamplerStateCollection SamplerStates { get; set; }

        public bool IsDisposed { get; set; }

        public void SetVertexBuffer(VertexBuffer vertexBuffer)
        {
        }

        public void SetVertexBuffer(VertexBuffer vertexBuffer, int vertexOffset)
        {
        }

        public void DrawIndexedPrimitives(
            PrimitiveType primitiveType,
            int baseVertex,
            int minVertexIndex,
            int numVertices,
            int startIndex,
            int primitiveCount
        )
        {
        }
    }

    public enum PrimitiveType
    {

    }

    public class VertexBuffer : GraphicsResource
    {
    }

    public class TextureCollection
    {
        public Texture this [int index]
        {
            get
            {
                return null;
            }
            set{ }
        }

    }

    public class SamplerStateCollection
    {
        public SamplerState this [int index]
        {
            get
            {
                return null;
            }
            set{ }
        }
    }
}
