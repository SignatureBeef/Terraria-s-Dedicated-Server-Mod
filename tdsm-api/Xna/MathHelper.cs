
namespace Microsoft.Xna.Framework
{
    public static class MathHelper
    {
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static float SmoothStep(float value1, float value2, float amount)
        {
            float num = MathHelper.Clamp(amount, 0f, 1f);
            return MathHelper.Lerp(value1, value2, num * num * (3f - 2f * num));
        }

        public static float Clamp(float value, float min, float max)
        {
            value = ((value > max) ? max : value);
            value = ((value < min) ? min : value);
            return value;
        }
    }
}
