

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class DepthStencilState : GraphicsResource
    {
        public DepthStencilState()
        {
        }

        public bool DepthBufferEnable { get; set; }

        public static readonly DepthStencilState None;
        public static readonly DepthStencilState Default;
    }
}
