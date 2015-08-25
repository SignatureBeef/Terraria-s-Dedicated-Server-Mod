
namespace Microsoft.Xna.Framework.Graphics
{
    public class GraphicsResource
    {
        public void Dispose()
        {
        }

        public GraphicsDevice GraphicsDevice { get; set; }

        public bool IsDisposed { get; set; }
    }
}
