using System;
using TDSM.API.Plugin;
using TDSM.API.Data;
using TDSM.API.Logging;
using TDSM.API;
using System.IO;

namespace TDSM.Data.SQLite
{
    public class Plugin : BasePlugin
    {
        public const String SQLSafeName = "SqlPermissions";

        public Plugin()
        {
            this.TDSMBuild = 5;
            this.Version = "1";
            this.Author = "TDSM";
            this.Name = "SQLite Connector";
            this.Description = "Adds SQLite storage";
        }

        private SQLiteConnector _connector;

        //        static bool Is64Bit
        //        {
        //            get
        //            { return IntPtr.Size == 8; }
        //        }

        protected override void Initialized(object state)
        {
            base.Initialized(state);

//            //We need to move the appropriate interop dll
//            string pathToInterop;
//            if (Is64Bit)
//            {
//                pathToInterop = Path.Combine(Globals.LibrariesPath, "x64", "SQLite.Interop.dll");
//            }
//            else
//            {
//                pathToInterop = Path.Combine(Globals.LibrariesPath, "x86", "SQLite.Interop.dll");
//            }
//
//            if (File.Exists(pathToInterop))
//            {
//                var destination = Path.Combine(Environment.CurrentDirectory, "SQLite.Interop.dll");
//                if (File.Exists(destination))
//                    File.Delete(destination);
//
//                File.Copy(pathToInterop, destination);
//            }
//            else
//            {
//                throw new DllNotFoundException("Cannot find " + pathToInterop);
//            }
        }

        void Test(ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
        {

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
            if (args.ServerChangeState == TDSM.API.ServerState.Initialising)
            {
                ProgramLog.Plugin.Log("SQLite connector is: " + (_connector == null ? "disabled" : "enabled"));
            }
        }
    }
}

