using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Microsoft.Xna.Framework
{
    public class Game
    {
        public ContentManager Content { get; set; }
        public GameWindow Window { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public bool IsActive { get; set; }
        public bool IsFixedTimeStep { get; set; }
        public bool IsMouseVisible { get; set; }

        public Game()
        {
            Content = new ContentManager();
            Window = new GameWindow();
        }

        protected virtual void Initialize() { }
        protected virtual void LoadContent() { }
        protected virtual void Draw(GameTime gameTime) { }
        protected virtual void Update(GameTime gameTime) { }
        protected virtual void UnloadContent() { }

        public void Exit() { }
    }
}
