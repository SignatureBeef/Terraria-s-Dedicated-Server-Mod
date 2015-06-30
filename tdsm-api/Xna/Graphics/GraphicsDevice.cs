
namespace Microsoft.Xna.Framework.Graphics
{
    public class GraphicsDevice
    {
        public void Clear(Color colour) { }
        public void SetRenderTarget(RenderTarget2D renderTarget) { }
        public DepthStencilState DepthStencilState { get; set; }
        public PresentationParameters PresentationParameters { get; set; }
        public Viewport Viewport { get; set; }
    }
}
