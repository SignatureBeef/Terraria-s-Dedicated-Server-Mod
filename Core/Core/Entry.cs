using OTA;
using OTA.Config;
using OTA.Extensions;
using OTA.Logging;
using OTA.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TDSM.Core.Data;
using TDSM.Core.Data.Permissions;
using TDSM.Core.Misc;
using TDSM.Core.Plugin.Hooks;

[assembly: PluginDependency("OTA.Commands")]

namespace TDSM.Core
{
    [OTAVersion(1, 0)]
    public partial class Entry : BasePlugin
    {
        public const Int32 CoreBuild = 6;

        public static readonly string CoreVersion = CoreBuild + Globals.PhaseToSuffix(ReleasePhase.ReleaseCandiate);

        public TDSMConfig Config { get; private set; } = new TDSMConfig();

        private bool _useTimeLock;

        public bool UseTimeLock
        {
            get
            { return _useTimeLock; }
            set
            {

                _useTimeLock = value;
                TimelockTime = Terraria.Main.time;
            }
        }

        public double TimelockTime { get; set; }

        public bool TimelockRain { get; set; }

        public bool TimelockSlimeRain { get; set; }

        public bool StopNPCSpawning { get; set; }

        public bool TimeFastForwarding { get; set; }

        public bool RestartWhenNoPlayers { get; set; }

        public PairFileRegister Ops { get; private set; }

        public DataRegister Whitelist { get; private set; }

        public Dictionary<string, string> CommandDictionary { get; set; }

        //private Task _customInvasion;
        internal Dictionary<Int32, Int32> _invasion;
        internal int _assignedInvasionType = OTA.Callbacks.NPCCallback.AssignInvasionType();
        internal bool _notfInbound;

        internal bool _likeABoss;
        internal static CyclicQueue<String> _labDeathMessages = new CyclicQueue<String>((new string[]
            {
                " jumped out a window",
                " had to approve memo's",
                " failed to promote synergy",
                " cried deeply",
                " was too busy eating chicken strips",
                " forgot to approve memo's",
                " crashed into the sun",
                " blacked out in a sewer",
                " swallowed sadness"
            }).Shuffle());

        public Entry()
        {
            this.Author = "TDSM";
            this.Description = "TDSM Core";
            this.Name = "TDSM Core Module";
            this.Version = CoreVersion;
        }

        protected override void Enabled()
        {
            base.Enabled();

            RunComponent(ComponentEvent.Enabled);
        }

        protected override void Initialized(object state)
        {
            ProgramLog.Log("TDSM Rebind core build {0}", this.Version);

            //Register hook sources
            PluginManager.RegisterHookSource(typeof(TDSMHookPoints));

            CommandDictionary = new Dictionary<string, string>();

            Ops = new PairFileRegister(System.IO.Path.Combine(Globals.DataPath, "ops.txt"));
            Whitelist = new DataRegister(System.IO.Path.Combine(Globals.DataPath, "whitelist.txt"), false);

            string configFile;
            if (!String.IsNullOrEmpty(configFile = Terraria.Initializers.LaunchInitializer.TryParameter("-config")))
                Config.LoadFromFile(configFile);

            Config.LoadFromArguments();

            if (!String.IsNullOrEmpty(Config.DatabaseProvider))
            {
                OTA.Data.DatabaseFactory.Initialise(Config.DatabaseProvider, Config.DatabaseConnectionString);
                Storage.IsAvailable = true;
                OTA.Permissions.Permissions.SetHandler(new OTAPIPermissions());
                Dapper.SqlMapper.AddTypeMap(typeof(PasswordFormat), System.Data.DbType.Int32);
                Dapper.SqlMapper.AddTypeMap(typeof(Byte), System.Data.DbType.Byte);
            }

            ProgramLog.LogRotation = Config.LogRotation;

            Hook(OTA.Commands.Events.CommandEvents.Listening, OnListeningForCommands);

            AddComponents<Entry>();
            if (!RunComponent(ComponentEvent.Initialise))
            {
                this.Disable();
                ProgramLog.Log("TDSM Rebind core disabled as components are not running.");
                return;
            }

            ProgramLog.Log("TDSM Rebind core enabled");
        }

        protected override void PreEnable()
        {
            base.PreEnable();
            if (!String.IsNullOrEmpty(Config.DatabaseProvider))
                OTA.Data.DatabaseFactory.Migrate();
        }

        public void OnListeningForCommands(ref HookContext ctx, ref OTA.Commands.Events.CommandArgs.Listening args)
        {
            RunComponent(ComponentEvent.ReadyForCommands);
        }

        void ProcessPIDFile(string pidPath)
        {
            var PIDFile = pidPath.Trim();
            if (PIDFile.Length > 0)
            {
                string ProcessUID = Process.GetCurrentProcess().Id.ToString();
                bool fail = false;
                if (File.Exists(PIDFile))
                {
                    try
                    {
                        File.Delete(PIDFile);
                    }
                    catch (Exception e)
                    {
                        ProgramLog.Log(e);
                        ProgramLog.Console.Print("Issue deleting PID file, Continue? [Y/n]: ");
                        if (Console.ReadLine().ToLower() == "n")
                        {
                            ProgramLog.Console.Print("Press any key to exit...");
                            Console.ReadKey(true);
                            return;
                        }
                        fail = true;
                    }
                }
                if (!fail)
                {
                    try
                    {
                        File.WriteAllText(PIDFile, ProcessUID);
                    }
                    catch (Exception e)
                    {
                        ProgramLog.Log(e);
                        ProgramLog.Console.Print("Issue writing PID file, Continue? [Y/n]: ");
                        if (Console.ReadLine().ToLower() == "n")
                        {
                            ProgramLog.Console.Print("Press any key to exit...");
                            Console.ReadKey(true);
                            return;
                        }
                    }
                    ProgramLog.Log("PID file successfully created with: " + ProcessUID);
                }
            }
        }

        protected override void Disabled()
        {
            ProgramLog.Log(base.Name + " disabled.");
        }

        public static void Log(string fmt, params object[] args)
        {
            ProgramLog.Log("[TDSM] " + fmt, args);
        }

        void OnLogFinished()
        {
            Thread.Sleep(500);
            ProgramLog.Log("Log end.");
            ProgramLog.Close();
        }

        public void PerformRestart()
        {
#if TDSMServer
            Server.PerformRestart();
            RestartWhenNoPlayers = false;
            if (_tskWaitForPlayers != null) _tskWaitForPlayers.Enabled = false;
            Server.AcceptNewConnections = _waitFPState.HasValue ? _waitFPState.Value : true;
#endif
        }

        void OnWorldSave(ref HookContext ctx, ref HookArgs.WorldAutoSave args)
        {
            //let our backup manager do it's thing
            ctx.SetResult(HookResult.IGNORE, true);
        }
    }
}
