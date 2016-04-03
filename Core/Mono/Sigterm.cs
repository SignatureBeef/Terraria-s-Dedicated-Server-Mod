using Mono.Unix;
using Mono.Unix.Native;
using OTA;
using System.Threading;

namespace TDSM.Core.Mono
{
    /// <summary>
    /// This is a Mono specific implementation to check for termination events.
    /// </summary>
    public static class Sigterm
    {
        [TDSMComponent(ComponentEvent.ReadyForCommands)]
        public static void Attach(Entry plugin)
        {
            if (Tools.RuntimePlatform != OTA.Misc.RuntimePlatform.Microsoft)
            {
                MonoSigterm.Attach(plugin);
            }
        }

        [TDSMComponent(ComponentEvent.ServerStopping)]
        public static void Detach(Entry plugin)
        {
            if (Tools.RuntimePlatform != OTA.Misc.RuntimePlatform.Microsoft)
            {
                MonoSigterm.Detach(plugin);
            }
        }

        /// <summary>
        /// This class is used to isolate the mono specific code into it's own container.
        /// The Sigterm class is used to check what platform is running with triggering
        /// a load of the Mono.Posix dll.
        /// </summary>
        internal static class MonoSigterm
        {
            private static bool _attached;
            private static Thread signal_thread;

            public static void Attach(Entry plugin)
            {
                try
                {
                    if (!_attached)
                    {
                        _attached = true;
                        // Catch SIGINT, SIGUSR1 and SIGTERM
                        UnixSignal[] signals = new UnixSignal[]
                        {
                            new UnixSignal(Signum.SIGINT),
                            new UnixSignal(Signum.SIGUSR1),
                            new UnixSignal(Signum.SIGTERM)
                        };

                        (signal_thread = new Thread(delegate ()
                        {
                            System.Threading.Thread.CurrentThread.Name = "SIG";
                            while (!Terraria.Netplay.disconnect && _attached)
                            {
                                // Wait for a signal to be delivered
                                var index = UnixSignal.WaitAny(signals, -1);
                                var signal = signals[index].Signum;

                                if (!Terraria.Netplay.disconnect && _attached)
                                {
                                    _attached = false;
                                    OTA.Logging.ProgramLog.Log("Server received Exit Signal");

                                    Terraria.IO.WorldFile.saveWorld(false);
                                    Terraria.Netplay.disconnect = true;
                                }
                            }
                        })).Start();
                    }

                    OTA.Logging.ProgramLog.Log("Server can accept SIGTERM");
                }
                catch
                {
                    OTA.Logging.ProgramLog.Log("Failed to attatch SIGTERM listener");
                }
            }

            public static void Detach(Entry plugin)
            {
                _attached = false;
                try
                {
                    signal_thread.Abort();
                }
                catch
                {
                }
            }
        }
    }
}

