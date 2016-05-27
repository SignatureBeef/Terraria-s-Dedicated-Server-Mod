using System.Collections.Generic;
using System.Linq;

namespace TDSM.Core.Mono
{
    /// <summary>
    /// This is a Mono specific implementation to check for termination events.
    /// </summary>
    public static class Sigterm
    {
        [TDSMComponent(ComponentEvent.ServerTick)]
        public static void OnServerTick(Entry plugin)
        {
            if (OTA.Tools.RuntimePlatform != OTA.Misc.RuntimePlatform.Microsoft)
            {
                MonoSigterm.OnServerTick(plugin);
            }
        }

        /// <summary>
        /// This class is used to isolate the mono specific code into it's own container.
        /// The Sigterm class is used to check what platform is running without triggering
        /// a load of the Mono.Posix dll. Calling this class will trigger the load.
        /// </summary>
        internal static class MonoSigterm
        {
            private static IEnumerable<global::Mono.Unix.UnixSignal> _signals = new[]
            {
                new global::Mono.Unix.UnixSignal(global::Mono.Unix.Native.Signum.SIGINT),
                new global::Mono.Unix.UnixSignal(global::Mono.Unix.Native.Signum.SIGUSR1),
                new global::Mono.Unix.UnixSignal(global::Mono.Unix.Native.Signum.SIGTERM)
            };

            public static void OnServerTick(Entry plugin)
            {
                if (_signals.Any(s => s.IsSet))
                {
                    OTA.Logging.ProgramLog.Log("Server received Exit Signal, auto saving...");

                    Terraria.IO.WorldFile.saveWorld(false);
                    Terraria.Netplay.disconnect = true;
                }
            }
        }
    }
}

