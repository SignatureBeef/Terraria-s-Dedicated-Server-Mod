using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework
{
    public class GraphicsDeviceManager
    {
        public GraphicsDeviceManager(Game game) { }

        public bool IsFullScreen { get; set; }
        public int PreferredBackBufferWidth { get; set; }
        public int PreferredBackBufferHeight { get; set; }
        public bool SynchronizeWithVerticalRetrace { get; set; }

        public void ToggleFullScreen() { }
        public void ApplyChanges() { }

        public GraphicsDevice GraphicsDevice { get; set; }
    }
}
