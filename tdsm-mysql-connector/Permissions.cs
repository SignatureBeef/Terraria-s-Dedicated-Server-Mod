using System;
using TDSM.API.Data;
using TDSM.API;
using TDSM.Data.MySQL.Tables;
using System.IO;
using TDSM.API.Logging;

namespace TDSM.Data.MySQL
{
    public partial class MySQLConnector
    {
        private GroupTable _groups;
        private PermissionTable _nodes;
        private GroupPermissions _groupPerms;
        private UserPermissions _userPerms;
        private UserGroupsTable _users;

        Permission IPermissionHandler.IsPermitted(string node, BasePlayer player)
        {
            if (player != null)
            {
                if (player.AuthenticatedAs != null)
                    return IsPermitted(node, false, player.AuthenticatedAs);

                return IsPermitted(node, true);
            }

            return Permission.Denied;
        }

        void InitialisePermissions()
        {
            _groups = new GroupTable();
            _nodes = new PermissionTable();
            _userPerms = new UserPermissions();
            _groupPerms = new GroupPermissions();
            _users = new UserGroupsTable();

            _groups.Initialise(this);
            _nodes.Initialise(this);
            _userPerms.Initialise(this);
            _groupPerms.Initialise(this);
            _users.Initialise(this);

            if (!Procedure_IsPermitted.Exists(this))
            {
                ProgramLog.Admin.Log("Permission procedure does not exist and will now be created");
                Procedure_IsPermitted.Create(this);
            }
        }

        private Permission IsPermitted(string node, bool isGuest, string authentication = null)
        {
            using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
            {
                sb.ExecuteProcedure("SqlPermissions_IsPermitted", "prm", 
                    new DataParameter("Node", node),
                    new DataParameter("IsGuest", isGuest),
                    new DataParameter("Authentication", authentication)
                );

                return (Permission)Storage.ExecuteScalar<Int32>(sb);
            }
        }

        static class PluginContent
        {
            public static string GetResource(string name)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();

                using (var stream = assembly.GetManifestResourceStream(name))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        static class Procedure_IsPermitted
        {
            public static bool Exists(MySQLConnector conn)
            {
                using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
                {
                    sb.ProcedureExists("SqlPermissions_IsPermitted");

                    return ((IDataConnector)conn).Execute(sb);
                }
            }

            public static bool Create(MySQLConnector conn)
            {
                using (var sb = new MySQLQueryBuilder(SqlPermissions.SQLSafeName))
                {
                    var proc = PluginContent.GetResource("TDSM.Data.MySQL.IsPermitted.sql");
//                    sb.ProcedureCreate("SqlPermissions_IsPermitted", proc, 
//                        new ProcedureParameter("prmNode", typeof(String), 50),
//                        new ProcedureParameter("prmIsGuest", typeof(Boolean)),
//                        new ProcedureParameter("prmAuthentication", typeof(String), 50)
//                    );

                    sb.CommandType = System.Data.CommandType.Text;
                    sb.CommandText = proc;

                    return ((IDataConnector)conn).Execute(sb);
                }
            }
        }
    }
}

