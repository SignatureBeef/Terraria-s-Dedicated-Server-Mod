using OTA.Plugin;
using System;
using OTA.Misc;
using TDSM.Core.Command;
using System.Collections.Generic;
using OTA;
using OTA.Command;
using OTA.Logging;
using TDSM.Core.Plugin.Hooks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Text;
using Microsoft.Xna.Framework;
using System.Linq;
using TDSM.Core.Misc;

namespace TDSM.Core
{
    [OTAVersion(1, 0)]
    public partial class Entry : BasePlugin
    {
        public const Int32 CoreBuild = 6;

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

        public static string RConHashNonce { get; set; }

        public static string RConBindAddress { get; set; }

        public bool EnableCheatProtection { get; set; }

        private bool VanillaOnly
        {
            get
            {
                return !IsEnabled;
            }
            set
            {
                if (value)
                {
                    if (IsEnabled)
                        PluginManager.DisablePlugin(this);
                }
                else
                {
                    if (!IsEnabled)
                        PluginManager.EnablePlugin(this);
                }
            }
        }

        public bool StopNPCSpawning { get; set; }

        public bool TimeFastForwarding { get; set; }

        public bool RunServerCore { get; set; }

        internal string _webServerAddress { get; set; }

        internal string _webServerProvider { get; set; }

        public bool RestartWhenNoPlayers { get; set; }

        public PairFileRegister Ops { get; private set; }

        public bool WhitelistEnabled { get; set; }

        public DataRegister Whitelist { get; private set; }

        public int ExitAccessLevel { get; set; }

        public bool EnableHeartbeat { get; set; }

        public bool AllowSSCGuestInfo { get; set; }

        public Dictionary<string, string> CommandDictionary { get; set; }

        //private Task _customInvasion;
        internal Dictionary<Int32,Int32> _invasion;
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

        /// <summary>
        /// The active server instance of the command parser
        /// </summary>
        /// <value>The command parser.</value>
        public static CommandParser CommandParser { get; } = new CommandParser();

        public Entry()
        {
            this.Author = "TDSM";
            this.Description = "TDSM Core";
            this.Name = "TDSM Core Module";
            this.Version = CoreBuild + Globals.PhaseToSuffix(ReleasePhase.LiveRelease);
        }

        protected override void Enabled()
        {
            base.Enabled();

            EnableCheatProtection = true;
            RunServerCore = true;
            ExitAccessLevel = -1;

            OTA.Web.API.PublicController.ShowPlugins = true;

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

            AddComponents<Entry>();
            RunComponent(ComponentEvent.Initialise);

            ProgramLog.Log("TDSM Rebind core enabled");
        }

        public void ListenForCommands()
        {
            RunComponent(ComponentEvent.ReadyForCommands);

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ProgramLog.Log("Ready for commands.");
            while (!Terraria.Netplay.disconnect /*|| Server.RestartInProgress*/)
            {
                try
                {
                    var ln = Console.ReadLine();
                    if (!String.IsNullOrEmpty(ln))
                        CommandParser.ParseConsoleCommand(ln);
                    else if (null == ln)
                    {
                        ProgramLog.Log("No console input available");
                        break;
                    }
                }
                catch (ExitException)
                {
                }
                catch (Exception e)
                {
                    ProgramLog.Log("Exception from command");
                    ProgramLog.Log(e);
                }
            }
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
