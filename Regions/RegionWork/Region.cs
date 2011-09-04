using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;
using Terraria_Server;

namespace Regions.RegionWork
{
    public class Region
    {
        public String Name { get; set; }
        public String Description = "";

        public Boolean Restricted = false; //restrict from Ops
        public Boolean RestrictedNPCs = false;
        public List<String> UserList = new List<String>();
        public List<String> ProjectileList = new List<String>();

        /* In tile format (playerpos / 16) */
        public Vector2 Point1 { get; set; }
        public Vector2 Point2 { get; set; }

        private List<TileRef> Tiles = new List<TileRef>();

        public List<TileRef> GetTiles
        {
            get
            {
                Tiles.Clear();

                /* Get Mins/Maxs (Already in tile format, / 16)*/
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

        public Boolean ContainsProjectile(String key)
        {
            foreach (String internalKey in ProjectileList)
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

        public Vector2 GetSmallestPoint()
        {
            return (Point1 < Point2) ? Point1 : Point2;
        }

        public Vector2 GetLargestPoint()
        {
            return (Point1 > Point2) ? Point1 : Point2;
        }

        public Vector2 GetOppositePoint(Vector2 point)
        {
            return (point == Point1) ? Point2 : Point1;
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
        /// Tries to find if a tile is within the region.
        /// </summary>
        /// <param name="tile>Tile to find in the region.</param>
        /// <returns>True on find</returns>
        public Boolean HasTile(TileData tile)
        {
            foreach (TileRef tileRef in GetTiles)
            {
                if (tileRef.Data.Equals(tile))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to find if a point is within the region.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <returns>True upon success</returns>        
        public Boolean HasPoint(Vector2 point,Boolean toTile = false)
        {
            int inX = (toTile) ? (int)(point.X / 16) : (int)point.X;
            int inY = (toTile) ? (int)(point.Y / 16) : (int)point.Y;

            Vector2 left = GetSmallestPoint();
            Vector2 right = GetLargestPoint(); /* GetOppositePoint() */
            int maxY = (int)Math.Max(left.Y, right.Y);
            int minY = (int)Math.Min(left.Y, right.Y);

            return (inX >= (int)left.X && inX <= (int)right.X && inY >= minY && inY <= maxY);
        }

        public String UserListToString()
        {
            /*String users = "";
            foreach (String user in UserList)
            {
                users += user + " ";
            }
            return users.Trim();*/
            return string.Join(" ", UserList.ToArray()).Trim();
        }

        public String ProjectileListToString()
        {
            return string.Join(" ", ProjectileList.ToArray()).Trim();
        }

        public Boolean IsRestrictedForUser(Player player)
        {
            return (Restricted && player.Op || !player.Op && !ContainsUser(player.Name) && !ContainsUser(player.IPAddress));
        }

        public override String ToString()
        {
            return string.Format(
                "name: {0}\n" +
                "description: {1}\n" +
                "point1: {2},{3}\n" +
                "point2: {4},{5}\n" +
                "users: {6}\n" +
                "projectiles: {7}\n" +
                "restricted: {8}" +
                "npcrestrict: {9}",
                Name, Description, Point1.X, Point1.Y, Point2.X, Point2.Y, UserListToString(), ProjectileListToString(), Restricted, RestrictedNPCs);
        }
    }
}
