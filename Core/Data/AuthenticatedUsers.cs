using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using OTA.Logging;

namespace TDSM.Core.Data
{
    #if DATA_CONNECTOR
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
        /// The default user table
        /// </summary>
        public class UserTable
        {
            public const String TableName = "users";

            public static class ColumnNames
            {
                public const String Id = "Id";
                public const String Username = "Username";
                public const String Password = "Password";
                public const String Operator = "Operator";
                public const String DateAdded = "DateAdded";
            }

            public static readonly TableColumn[] Columns = new TableColumn[]
            {
                new TableColumn(ColumnNames.Id, typeof(Int32), true, true),
                new TableColumn(ColumnNames.Username, typeof(String), 255),
                new TableColumn(ColumnNames.Password, typeof(String), 255),
                new TableColumn(ColumnNames.IsOperator, typeof(Boolean)),
                new TableColumn(ColumnNames.DateAdded, typeof(DateTime))
            };

            /// <summary>
            /// Checks if the table exists
            /// </summary>
            public static bool Exists()
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.TableExists(TableName);

                    return Storage.Execute(bl);
                }
            }

            /// <summary>
            /// Creates the table
            /// </summary>
            public static bool Create()
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.TableCreate(TableName, Columns);

                    return Storage.ExecuteNonQuery(bl) > 0;
                }
            }
        }

        internal static void Initialise()
        {
            if (!UserTable.Exists())
            {
                ProgramLog.Admin.Log("Common user table does not exist and will now be created");
                UserTable.Create();
            }
        }

        /// <summary>
        /// Gets the user count.
        /// </summary>
        /// <value>The user count.</value>
        public static int UserCount
        {
            get
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl
                        .Select()
                        .Count()
                        .From(UserTable.TableName);

                    return Storage.ExecuteScalar<Int32>(bl);
                }
            }
        }

        /// <summary>
        /// Checks if a user exists
        /// </summary>
        /// <returns><c>true</c>, if exists was usered, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        public static bool UserExists(string username)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.Select().Count().From(UserTable.TableName).Where(new WhereFilter(UserTable.ColumnNames.Username, username));

                return Storage.ExecuteScalar<Int64>(bl) > 0;
            }
        }

        /// <summary>
        /// Gets the user password.
        /// </summary>
        /// <returns>The user password.</returns>
        /// <param name="username">Username.</param>
        public static string GetUserPassword(string username)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.SelectFrom(UserTable.TableName, new string[] { UserTable.ColumnNames.Password }, new WhereFilter(UserTable.ColumnNames.Username, username));

                return Storage.ExecuteScalar<String>(bl);
            }
        }

        /// <summary>
        /// Gets the user from the database by name
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="username">Username.</param>
        public static DbPlayer GetUser(string username)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.SelectFrom(UserTable.TableName, new string[]
                    {
                        UserTable.ColumnNames.Id,
                        UserTable.ColumnNames.Username,
                        UserTable.ColumnNames.Password,
                        UserTable.ColumnNames.IsOperator
                    }, new WhereFilter(UserTable.ColumnNames.Username, username));

                var res = Storage.ExecuteArray<DbPlayer>(bl);
                if (res != null && res.Length > 0)
                    return res[0];

                return null;
            }
        }

        private struct FUBP
        {
            public string Username;
        }

        /// <summary>
        /// Finds a list of users matching a prefix
        /// </summary>
        /// <returns>The users by prefix.</returns>
        /// <param name="search">Search.</param>
        public static string[] FindUsersByPrefix(string search)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.SelectFrom(UserTable.TableName, new string[]
                    {
                        UserTable.ColumnNames.Username,
                    }, new WhereFilter(UserTable.ColumnNames.Username, search + '%', WhereExpression.Like));

                return Storage.ExecuteArray<FUBP>(bl).Select(x => x.Username).ToArray();
            }
        }

        /// <summary>
        /// Removes a user from the database by name
        /// </summary>
        /// <returns><c>true</c>, if user was deleted, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        public static bool DeleteUser(string username)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.Delete(UserTable.TableName, new WhereFilter[]
                    {
                        new WhereFilter(UserTable.ColumnNames.Username, username)
                    });
                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }

        /// <summary>
        /// Creates a user.
        /// </summary>
        /// <returns><c>true</c>, if user was created, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="op">If set to <c>true</c> op.</param>
        public static bool CreateUser(string username, string password, bool op = false)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                var hs = AuthenticatedUsers.Hash(username, password);
                bl.InsertInto(UserTable.TableName, new DataParameter[]
                    {
                        new DataParameter(UserTable.ColumnNames.Username, username),
                        new DataParameter(UserTable.ColumnNames.Password, hs),
                        new DataParameter(UserTable.ColumnNames.IsOperator, op),
                        new DataParameter(UserTable.ColumnNames.DateAdded, DateTime.Now)
                    });
                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }

        /// <summary>
        /// Updates a user in the database.
        /// </summary>
        /// <returns><c>true</c>, if user was updated, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="op">If set to <c>true</c> op.</param>
        public static bool UpdateUser(string username, string password, bool op = false)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                var hs = AuthenticatedUsers.Hash(username, password);
                bl.Update(UserTable.TableName, new DataParameter[]
                    {
                        new DataParameter(UserTable.ColumnNames.Password, hs),
                        new DataParameter(UserTable.ColumnNames.IsOperator, op)
                    },
                    new WhereFilter(UserTable.ColumnNames.Username, username)
                );
                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }

        /// <summary>
        /// Updates a user in the database.
        /// </summary>
        /// <returns><c>true</c>, if user was updated, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        public static bool UpdateUser(string username, string password)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                var hs = AuthenticatedUsers.Hash(username, password);
                bl.Update(UserTable.TableName, new DataParameter[]
                    {
                        new DataParameter(UserTable.ColumnNames.Password, hs)
                    },
                    new WhereFilter(UserTable.ColumnNames.Username, username)
                );
                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }

        /// <summary>
        /// Updates a user in the database.
        /// </summary>
        /// <returns><c>true</c>, if user was updated, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="op">If set to <c>true</c> op.</param>
        public static bool UpdateUser(string username, bool op = false)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.Update(UserTable.TableName, new DataParameter[]
                    {
                        new DataParameter(UserTable.ColumnNames.IsOperator, op)
                    },
                    new WhereFilter(UserTable.ColumnNames.Username, username)
                );
                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }
    }




#else
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
                #if ENTITY_FRAMEWORK_6
                if (OTA.Data.EF6.OTAContext.IsSQLite)
                {
                    var lowered = search.ToLower();
                    return ctx.Players
                        .Where(x => x.Name.Length >= search.Length && x.Name.Substring(0, search.Length).ToLower() == lowered)
                        .Select(x => x.Name)
                        .ToArray();
                }
                else
                {
#endif
                    return ctx.Players
                         .Where(x => x.Name.StartsWith(search))
                         .Select(x => x.Name)
                         .ToArray();
                    #if ENTITY_FRAMEWORK_6
                }
#endif
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
                ctx.Players.RemoveRange(matches);

                ctx.SaveChanges();

                return ctx.Players.Any(x => x.Name == username);
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
                DbPlayer player;
                ctx.Players.Add(player = new DbPlayer(username, password)
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
    #endif
}

