using System;
using TDSM.API.Logging;

namespace TDSM.API.Data
{
    public class UserDetails
    {
        public string Password;
        public string Username;
        public bool Operator;

        public override string ToString()
        {
            return String.Format("[UserDetails: Username: '{0}', Password: '{1}', Operator: {2}]", Username, Password, Operator);
        }
    }

    /// <summary>
    /// Authenticated users.
    /// </summary>
    /// <remarks></remarks>
    public static class AuthenticatedUsers
    {
        const String SQLSafeName = "tdsm";

        private class UserTable
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
                new TableColumn(ColumnNames.Operator, typeof(Boolean)),
                new TableColumn(ColumnNames.DateAdded, typeof(DateTime))
            };

            public static bool Exists()
            {
                using (var bl = Storage.GetBuilder(SQLSafeName))
                {
                    bl.TableExists(TableName);

                    return Storage.Execute(bl);
                }
            }

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

        public static bool UserExists(string username)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.Select().Count().From(UserTable.TableName).Where(new WhereFilter(UserTable.ColumnNames.Username, username));

                return Storage.ExecuteScalar<Int64>(bl) > 0;
            }
        }

        public static string GetUserPassword(string username)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.SelectFrom(UserTable.TableName, new string[] { UserTable.ColumnNames.Password }, new WhereFilter(UserTable.ColumnNames.Username, username));

                return Storage.ExecuteScalar<String>(bl);
            }
        }

        public static UserDetails GetUser(string username)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.SelectFrom(UserTable.TableName, new string[] {
                    UserTable.ColumnNames.Username,
                    UserTable.ColumnNames.Password, 
                    UserTable.ColumnNames.Operator 
                }, new WhereFilter(UserTable.ColumnNames.Username, username));

                var res = Storage.ExecuteArray<UserDetails>(bl);
                if (res != null && res.Length > 0)
                    return res[0];

                return null;
            }
        }

        public static bool CreateUser(string username, string password, bool op = false)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.InsertInto(UserTable.TableName, new DataParameter[]
                    {
                        new DataParameter(UserTable.ColumnNames.Username, username),
                        new DataParameter(UserTable.ColumnNames.Password, password),
                        new DataParameter(UserTable.ColumnNames.Operator, op),
                        new DataParameter(UserTable.ColumnNames.DateAdded, DateTime.Now)
                    });
                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }

        public static bool UpdateUser(string username, string password, bool op = false)
        {
            using (var bl = Storage.GetBuilder(SQLSafeName))
            {
                bl.Update(UserTable.TableName, new DataParameter[]
                    {
                        new DataParameter(UserTable.ColumnNames.Password, password),
                        new DataParameter(UserTable.ColumnNames.Operator, op)
                    },
                    new WhereFilter(UserTable.ColumnNames.Username, username)
                );
                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }
    }
}

