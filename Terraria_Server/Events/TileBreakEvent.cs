namespace Terraria_Server.Events
{
    public class TileBreakEvent : BasePlayerEvent
    {
        public Tile Tile { get; set; }

        public int Type { get; set; }

        public Vector2 Position { get; set; }
    }
}

        
