
using System.Runtime.InteropServices;
namespace Microsoft.Xna.Framework.Graphics
{
    public class RenderTarget2D
    {
        public static RenderTarget2D[,] Array = new RenderTarget2D[0, 0];

        public bool IsContentLost { get { return false; } set { } }

        public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height)
        {
        }

        public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height, [MarshalAs(UnmanagedType.U1)] bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
        {

        }
        public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height, [MarshalAs(UnmanagedType.U1)] bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat) { }
    } // : Texture2D, IDynamicGraphicsResource
}
