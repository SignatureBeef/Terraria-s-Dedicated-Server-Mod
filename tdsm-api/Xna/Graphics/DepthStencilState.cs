

namespace Microsoft.Xna.Framework.Graphics
{
    public class DepthStencilState
    {
        public DepthStencilState() { }
        public bool DepthBufferEnable { get; set; }
        public static readonly DepthStencilState None;
        public static readonly DepthStencilState Default;
    }
}
