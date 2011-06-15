using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Events
{
    public class TileBreakEvent : Event
    {
        private Tile tile = null;
        private int type;
    	private Vector2 pos;

        public Player getPlayer()
        {
            return (Player)getSender();
        }

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

        public Vector2 getTilePos()
        {
            return pos;
		}

        public void setTilePos(Vector2 Pos)
        {
            pos = Pos;
        }
    }
}

        
