using System;


namespace Microsoft.Xna.Framework
{
    public class GameTime
    {
        public GameTime() { }
        public TimeSpan ElapsedGameTime { get; set; }
        public TimeSpan TotalGameTime { get; set; }
    }
}
