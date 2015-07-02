
namespace Microsoft.Xna.Framework.Graphics
{
    public class Effect
    {
        public EffectTechnique CurrentTechnique { get; set; }

        public EffectParameterCollection Parameters { get; set; }
    }

    public class EffectParameterCollection
    {
        public EffectParameter this [string name]
        {
            get
            {
                return null;
            }
            set{ }
        }
    }

    public class EffectParameter
    {
        public void SetValue(Vector3 value)
        {
        }
    }
}
