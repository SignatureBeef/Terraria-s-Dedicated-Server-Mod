using System;
using TDSM.API.Data;

namespace TDSM.Data.MySQL.Procedures
{
    public  static class IsPermitted
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
                var proc = PluginContent.GetResource("TDSM.Data.MySQL.Procedures.Files.IsPermitted.sql");
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

