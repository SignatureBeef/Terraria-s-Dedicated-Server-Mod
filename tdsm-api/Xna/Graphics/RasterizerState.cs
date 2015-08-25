
namespace Microsoft.Xna.Framework.Graphics
{
    public class RasterizerState
    {
        public static readonly RasterizerState CullClockwise = null;
        public static readonly RasterizerState CullCounterClockwise = null;
        public static readonly RasterizerState CullNone = null;

        public float SlopeScaleDepthBias
        {
            get
            { return 0f; }
            set { }
        }
        public float DepthBias
        {
            get
            { return 0f; }
            set { }
        }

        public bool MultiSampleAntiAlias
        {
            get
            { return false; }
            set { }
        }

        public bool ScissorTestEnable
        {
            get
            { return false; }
            set { }
        }

        public FillMode FillMode
        {
            get
            { return FillMode.Solid; }
            set { }
        }

        public CullMode CullMode
        {
            get
            { return CullMode.None; }
            set { }
        }
    }

    public enum CullMode
    {
        None,
        CullClockwiseFace,
        CullCounterClockwiseFace
    }

    public enum FillMode
    {
        Solid,
        WireFrame
    }
}
