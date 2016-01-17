using System;
using System.Threading;

/// <summary>
/// Mono specific code
/// </summary>
using Mono.Unix.Native;
using Mono.Unix;
using OTA;

namespace TDSM.Core.Mono
{
    /// <summary>
    /// This is a Mono specific implementation to check for termination events
    /// </summary>
    public static class Sigterm
    {
        private static bool _attached;
        private static Thread signal_thread;

        [TDSMComponent(ComponentEvent.ReadyForCommands)]
        public static void Attach(Entry plugin)
        {
            if (Tools.RuntimePlatform != OTA.Misc.RuntimePlatform.Microsoft)
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
        }

        [TDSMComponent(ComponentEvent.ServerStopping)]
        public static void Detach(Entry plugin)
        {
            if (Tools.RuntimePlatform != OTA.Misc.RuntimePlatform.Microsoft)
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

