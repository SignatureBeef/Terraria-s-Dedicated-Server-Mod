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
        public string Name { get; set; }
        public string Description = "";

        public bool Restricted = false; //restrict from Ops
        public bool RestrictedNPCs = false;
        public List<String> UserList = new List<String>();
        public List<String> ProjectileList = new List<String>();

        /* In tile format (playerpos / 16) */
        public Vector2 Point1 { get; set; }
        public Vector2 Point2 { get; set; }

        public bool IsValidRegion()
        {
            bool LocationCheck = Server.IsValidLocation(Point1, false) && Server.IsValidLocation(Point2, false);
            bool NameCheck = Name != null && Name.Trim().Length > 0;
            return NameCheck && LocationCheck;
        }

        public bool ContainsUser(string key)
        {
            foreach (string internalKey in UserList)
            {
                if (internalKey.Equals(key))
                    return true;
            }

            return false;
        }

        public bool ContainsProjectile(string key)
        {
            foreach (string internalKey in ProjectileList)
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
        /// Tries to find if a point is within the region.
        /// </summary>
        /// <param name="point">Point to check.</param>
        /// <returns>True upon success</returns>        
        public bool HasPoint(Vector2 point, bool toTile = false)
        {
            int inX = (toTile) ? (int)(point.X / 16) : (int)point.X;
            int inY = (toTile) ? (int)(point.Y / 16) : (int)point.Y;

            Vector2 left = GetSmallestPoint();
            Vector2 right = GetLargestPoint(); /* GetOppositePoint() */
            int maxY = (int)Math.Max(left.Y, right.Y);
            int minY = (int)Math.Min(left.Y, right.Y);

            return (inX >= (int)left.X && inX <= (int)right.X && inY >= minY && inY <= maxY);
        }

        public string UserListToString()
        {
            return String.Join(" ", UserList.ToArray()).Trim();
        }

        public string ProjectileListToString()
        {
            return String.Join(" ", ProjectileList.ToArray()).Trim();
        }

        public bool IsRestrictedForUser(Player player)
        {
            return (Restricted && player.Op || !player.Op && !ContainsUser(player.Name) && !ContainsUser(player.IPAddress));
        }

        public override string ToString()
        {
            return String.Format(
                "name: {0}\n" +
                "description: {1}\n" +
                "point1: {2},{3}\n" +
                "point2: {4},{5}\n" +
                "users: {6}\n" +
                "projectiles: {7}\n" +
                "restricted: {8}\n" +
                "npcrestrict: {9}",
                Name, Description, Point1.X, Point1.Y, Point2.X, Point2.Y, UserListToString(), ProjectileListToString(), Restricted, RestrictedNPCs);
        }
    }
}
