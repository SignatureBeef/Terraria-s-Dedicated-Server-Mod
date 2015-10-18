using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace TDSM.Core.Data
{
    /// <summary>
    /// Authenticated users.
    /// </summary>
    /// <remarks></remarks>
    public static class AuthenticatedUsers
    {
        public const String SQLSafeName = "tdsm";

        internal static string Hash(string username, string password)
        {
            var hash = SHA256.Create();
            var sb = new StringBuilder(64);
            var bytes = hash.ComputeHash(Encoding.ASCII.GetBytes(username + ":" + password));
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// Gets the user count.
        /// </summary>
        /// <value>The user count.</value>
        public static int UserCount
        {
            get
            {
                using (var ctx = new TContext()) return ctx.Players.Count();
            }
        }

        /// <summary>
        /// Checks if a user exists
        /// </summary>
        /// <returns><c>true</c>, if exists was usered, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        public static bool UserExists(string username)
        {
            using (var ctx = new TContext())
            {
                return ctx.Players.Any(x => x.Name == username);
            }
        }

        /// <summary>
        /// Gets the user password.
        /// </summary>
        /// <returns>The user password.</returns>
        /// <param name="username">Username.</param>
        public static string GetUserPassword(string username)
        {
            using (var ctx = new TContext())
            {
                return ctx.Players
                    .Where(x => x.Name == username)
                    .Select(x => x.Password)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the user from the database by name
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="username">Username.</param>
        public static DbPlayer GetUser(string username)
        {
            using (var ctx = new TContext())
            {
                return ctx.Players
                    .Where(x => x.Name == username)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Finds a list of users matching a prefix
        /// </summary>
        /// <returns>The users by prefix.</returns>
        /// <param name="search">Search.</param>
        public static string[] FindUsersByPrefix(string search)
        {
            using (var ctx = new TContext())
            {
                if (OTA.Data.OTAContext.IsSQLite)
                {
                    var lowered = search.ToLower();
                    return ctx.Players
                        .Where(x => x.Name.Length >= search.Length && x.Name.Substring(0, search.Length).ToLower() == lowered)
                        .Select(x => x.Name)
                        .ToArray();
                }
                else return ctx.Players
                    .Where(x => x.Name.StartsWith(search))
                    .Select(x => x.Name)
                    .ToArray();
            }
        }

        /// <summary>
        /// Removes a user from the database by name
        /// </summary>
        /// <returns><c>true</c>, if user was deleted, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        public static bool DeleteUser(string username)
        {
            using (var ctx = new TContext())
            {
                var matches = ctx.Players.Where(x => x.Name == username);
                var range = ctx.Players.RemoveRange(matches);

                ctx.SaveChanges();

                return matches.Any();
            }
        }

        /// <summary>
        /// Creates a user.
        /// </summary>
        /// <returns><c>true</c>, if user was created, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="op">If set to <c>true</c> op.</param>
        public static DbPlayer CreateUser(string username, string password, bool op = false)
        {
            using (var ctx = new TContext())
            {
                var player = ctx.Players.Add(new DbPlayer(username, password)
                    {
                        Operator = op,
                        DateAddedUTC = DateTime.UtcNow
                    });

                ctx.SaveChanges();

                return player;
            }
        }

        /// <summary>
        /// Updates a user in the database.
        /// </summary>
        /// <returns><c>true</c>, if user was updated, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="op">If set to <c>true</c> op.</param>
        public static bool UpdateUser(string username, string password, bool? op = null)
        {
            if (username == null && op == null) throw new InvalidOperationException("You have not specified anything to be updated");
            using (var ctx = new TContext())
            {
                var player = ctx.Players.SingleOrDefault(p => p.Name == username);
                if (player == null) throw new InvalidOperationException("Cannot update a non-existent player");

                if (password != null) player.SetRawPassword(password);
                if (op.HasValue) player.Operator = op.Value;

                ctx.SaveChanges();

                return true;
            }
        }
    }
}

