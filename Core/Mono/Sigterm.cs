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
        /// The Sigterm class is used to check what platform is running without triggering
        /// a load of the Mono.Posix dll. Calling this class will trigger the load.
        /// </summary>
        internal static class MonoSigterm
        {
            private static bool _attached, _exiting;
            private static Thread _signal_thread;

            public static void Attach(Entry plugin)
            {
                try
                {
                    if (!_exiting && !_attached)
                    {
                        _attached = true;
                        // Catch SIGINT, SIGUSR1 and SIGTERM
                        UnixSignal[] signals = new UnixSignal[]
                        {
                            new UnixSignal(Signum.SIGINT),
                            new UnixSignal(Signum.SIGUSR1),
                            new UnixSignal(Signum.SIGTERM)
                        };

                        (_signal_thread = new Thread(() =>
                        {
                            System.Threading.Thread.CurrentThread.Name = "SIG";
                            try
                            {
                                while (!Terraria.Netplay.disconnect && _attached)
                                {
                                    // Wait for a signal to be delivered
                                    if (UnixSignal.WaitAll(signals, 250))
                                    {
                                        if (!Terraria.Netplay.disconnect && _attached)
                                        {
                                            _attached = false;
                                            OTA.Logging.ProgramLog.Log("Server received Exit Signal");

                                            Terraria.IO.WorldFile.saveWorld(false);
                                            Terraria.Netplay.disconnect = true;
                                        }
                                    }
                                }

                                OTA.Logging.ProgramLog.Debug.Log("Sigterm thread exiting");
                            }
                            catch (System.Exception e)
                            {
                                OTA.Logging.ProgramLog.Log(e, "Sigterm exception");
                            }
                        })).Start();
                    }
                }
                catch
                {
                    OTA.Logging.ProgramLog.Log("Failed to attach Sigterm listener");
                }
            }

            public static void Detach(Entry plugin)
            {
                if (_attached && _signal_thread != null)
                {
                    _attached = false;
                    _exiting = true;

                    //Instead of killing the thread, wait for it to exit.
                    _signal_thread.Join();
                    _signal_thread = null;
                }
            }
        }
    }
}

