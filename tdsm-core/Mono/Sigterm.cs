using System;
using System.Threading;

/// <summary>
/// Mono specific code
/// </summary>
using Mono.Unix.Native;
using Mono.Unix;


namespace TDSM.Core.Mono
{
    /// <summary>
    /// This is a Mono specific implementation to check for termination events
    /// </summary>
    public static class Sigterm
    {
        private static bool _attached;
        private static Thread signal_thread;

        public static void Attach()
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
                                    TDSM.API.Logging.ProgramLog.Log("Server received Exit Signal");
                                    TDSM.API.Command.DefaultCommands.Exit(null, null);
                                }
                            }
                        })).Start();
                }

                TDSM.API.Logging.ProgramLog.Log("Server can accept SIGTERM");
            }
            catch
            {
                TDSM.API.Logging.ProgramLog.Log("Failed to attatch SIGTERM listener");
            }
        }

        public static void Detach()
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

