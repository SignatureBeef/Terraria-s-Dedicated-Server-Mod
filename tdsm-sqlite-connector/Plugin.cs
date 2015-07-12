using System;
using tdsm.api.Plugin;
using tdsm.api.Data;
using tdsm.api.Logging;

namespace TDSM.Data.SQLite
{
    public class Plugin : BasePlugin
    {
        public Plugin()
        {
            this.TDSMBuild = 2;
            this.Version = "1";
            this.Author = "TDSM";
            this.Name = "SQLite Connector";
            this.Description = "Adds SQLite storage";
        }

        private SQLiteConnector _connector;

        protected override void Initialized(object state)
        {
            base.Initialized(state);
        }

        [Hook]
        void OnReadConfig(ref HookContext ctx, ref HookArgs.ConfigurationLine args)
        {
            switch (args.Key)
            {
                case "sqlite":
                    if (_connector == null)
                    {
                        var cn = new SQLiteConnector(args.Value);

                        cn.Open();

                        Storage.SetConnector(cn);

                        _connector = cn;
                    }
                    break;
            }
        }

        protected override void Enabled()
        {
            base.Enabled();
        }

        [Hook]
        void OnStateChange(ref HookContext ctx, ref HookArgs.ServerStateChange args)
        {
            if (args.ServerChangeState == tdsm.api.ServerState.Initialising)
            {
                ProgramLog.Plugin.Log("SQLite connector is: " + (_connector == null ? "disabled" : "enabled"));
            }
        }
    }
}

