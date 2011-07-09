using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Misc;
using Terraria_Server.Definitions.Tile;

namespace Terraria_Server.Events
{
    public class PlayerTileChangeEvent : Event
    {
        public Tile Tile { get; set; }

        public int Type { get; set; }

        public Vector2 Position { get; set; }

        public TileAction Action { get; set; }

        public TileType TileType { get; set; }
    }
}

        
