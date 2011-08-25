using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;
using Terraria_Server;

namespace Regions.Region
{
    public class Region
    {
        public String Name { get; set; }
        public String Description = "";

        public List<String> UserList = new List<String>();

        /* In tile format (*16) */
        public Vector2 Point1 { get; set; }
        public Vector2 Point2 { get; set; }

        private List<TileRef> Tiles = new List<TileRef>();

        public List<TileRef> GetTiles
        {
            get
            {
                Tiles.Clear();

                /* Get Mins/Maxs and convert to tile format */
                int XHighest = (int)Math.Max(Point1.X, Point2.X);
                int XLowest = (int)Math.Min(Point1.X, Point2.X);
                int YHighest = (int)Math.Max(Point1.Y, Point2.Y);
                int YLowest = (int)Math.Min(Point1.Y, Point2.Y);

                for (int x = XLowest; x < XHighest + 1; x++)
                {
                    for (int y = YLowest; y < YHighest + 1; y++)
                    {
                        Tiles.Add(Main.tile.At(x, y));
                    }
                }

                return Tiles;
            }
        }

        public Boolean IsValidRegion()
        {
            Boolean LocationCheck = Program.server.isValidLocation(Point1) && Program.server.isValidLocation(Point2);
            Boolean NameCheck = Name != null && Name.Trim().Length > 0;
            return NameCheck && LocationCheck;
        }

        public Boolean ContainsUser(String key)
        {
            foreach (String internalKey in UserList)
            {
                if (internalKey.Equals(key))
                    return true;
            }

            return false;
        }

        public int GetWidth
        {
            get
            {
                return GetBiggestX - GetSmallestX;
            }
        }

        public int GetHeight
        {
            get
            {
                return GetBiggestY - GetSmallestY;
            }
        }

        public int GetBiggestX
        {
            get
            {
                return (int)Math.Max(Point1.X, Point2.X);
            }
        }

        public int GetSmallestX
        {
            get
            {
                return (int)Math.Min(Point1.X, Point2.X);
            }
        }

        public int GetBiggestY
        {
            get
            {
                return (int)Math.Max(Point1.Y, Point2.Y);
            }
        }

        public int GetSmallestY
        {
            get
            {
                return (int)Math.Min(Point1.Y, Point2.Y);
            }
        }

        /// <summary>
        /// Tries to find if a tile is within the region.
        /// </summary>
        /// <param name="tile>Tile to find in the region.</param>
        /// <returns>True on find</returns>
        public Boolean HasTile(TileRef tile)
        {
            return GetTiles.Contains(tile);
        }

        /// <summary>
        /// Tries to find if a point is within the region.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <returns>True upon success</returns>
        public Boolean HasPoint(Vector2 point)
        {
            Rectangle pRect = new Rectangle((int)point.X, (int)point.Y, 1, 1);
            int x = GetSmallestX;
            int y = (int)((Point1.X == x) ? Point1.Y : Point2.Y);
            int width = (int)(Point2.X - Point1.X);
            int height = (int)(Point2.Y - Point1.Y);
            return pRect.Intersects(new Rectangle(x, y, width, height));
        }

        public String UserListToString()
        {
            String users = "";
            foreach (String user in UserList)
            {
                users += user + " ";
            }
            return users.Trim();
        }

        public String ToString()
        {
            return string.Format(
                "name: {0}\n" +
                "description: {1}\n" +
                "point1: {2},{3}\n" +
                "point2: {4},{5}\n" +
                "users: {6}\n",
                Name, Description, Point1.X, Point1.Y, Point2.X, Point2.Y, UserListToString());
        }
    }
}
