
namespace Microsoft.Xna.Framework.Input
{
    public struct KeyboardState
    {
        public static readonly Keys[] Empty = new Keys[0];

        public bool IsKeyDown(Keys key)
        {
            return false;
        }

        public Keys[] GetPressedKeys() { return Empty; }
    }
}
