using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class TileChangeEvent : Event
    {
        private Tile tile = null;
        private int type;

        public Tile getTile()
        {
            return tile;
        }

        public void setTile(Tile Tile)
        {
            tile = Tile;
        }

        public int getTileType()
        {
            return type;
        }

        public void setTileType(int TileType)
        {
            type = TileType;
        }

    }
}

        
