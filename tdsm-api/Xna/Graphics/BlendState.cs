
namespace Microsoft.Xna.Framework.Graphics
{
    public partial class BlendState : GraphicsResource
    {
        public static readonly BlendState Additive;
        public static readonly BlendState AlphaBlend;
        public static readonly BlendState NonPremultiplied;
        public static readonly BlendState Opaque;
    }

    public enum SetDataOptions
    {
        None,
        Discard,
        NoOverwrite
    }
}
