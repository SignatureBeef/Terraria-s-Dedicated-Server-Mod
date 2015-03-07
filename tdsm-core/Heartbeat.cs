//#define ENABLED

using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using tdsm.api;
using tdsm.api.Misc;
using tdsm.core.Logging;

namespace tdsm.core
{
    /// <summary>
    /// This is a multi purpose class.
    /// 1) It sends non identifiable data to the heatbeat server for us to see the stats (unless allowed)
    /// 2) The heartbeat server will reply with particular case data. Responses are:
    ///     a) 1 = OK
    ///     b) 2 = Update available
    /// </summary>
    public static class Heartbeat
    {
        internal const String EndPoint = "http://localhost/tdsm/"; //"http://heartbeat.tdsm.org/";
        internal const Int32 MinuteInterval = 1;

        private static System.Timers.Timer _timer;
        private static int _coreBuild;

        public static bool Enabled { get; private set; }
        public static bool PublishToList { get; set; }
        public static string ServerName { get; set; }
        public static string ServerDescription { get; set; }

        [Flags]
        enum UpdateReady : byte
        {
            API = 1,
            Core = 2,
            NPCDefinitions = 4,
            ItemDefinitions = 8,
            PluginOutOfDate = 16
        }

        enum ResponseCode : byte
        {
            UpToDate = 0,
            UpdateReady = 1,

            //Maybe a notice (flag for server/console and players - must be allowed in config)
            StringResponse = 255
        }

        static void Beat()
        {
            try
            {
                using (var wc = new WebClient())
                {
                    var req = new System.Collections.Specialized.NameValueCollection();
                    req.Add("API", Globals.Build.ToString());
                    req.Add("Core", _coreBuild.ToString());
                    req.Add("Platform", ((int)Platform.Type).ToString());
                    req.Add("UUID", ConstructUUID());
                    req.Add("npc-def", Definitions.DefinitionManager.NPCVersion.ToString());
                    req.Add("item-def", Definitions.DefinitionManager.ItemVersion.ToString());

                    if (PublishToList)
                    {
                        req.Add("Port", Terraria.Netplay.serverPort.ToString());
                        req.Add("MaxPlayers", Terraria.Main.maxPlayers.ToString());

                        if (!String.IsNullOrEmpty(ServerName)) req.Add("Name", ServerName);
                        if (!String.IsNullOrEmpty(ServerDescription)) req.Add("Desc", ServerDescription);
                    }

                    //TODO; Maybe plugin versions
                    //TODO; think about branches, release or dev

                    var data = wc.UploadValues(EndPoint, "POST", req);
                    if (data != null && data.Length > 0)
                    {
                        switch ((ResponseCode)data[0])
                        {
                            case ResponseCode.UpToDate:
                                //We're online - all up to date
                                break;
                            case ResponseCode.UpdateReady:
                                var flag = (UpdateReady)data[1];

                                var updates = String.Empty;
                                if ((flag & UpdateReady.API) != 0) updates += "TDSM API";
                                if ((flag & UpdateReady.Core) != 0) updates += (updates.Length > 0 ? ", " : String.Empty) + "TDSM Core";
                                if ((flag & UpdateReady.NPCDefinitions) != 0) updates += (updates.Length > 0 ? ", " : String.Empty) + "NPC definitions";
                                if ((flag & UpdateReady.ItemDefinitions) != 0) updates += (updates.Length > 0 ? ", " : String.Empty) + "Item definitions";

                                if ((flag & UpdateReady.PluginOutOfDate) != 0)
                                {
                                    //Read how many
                                    //for each
                                    //  Read name
                                }

                                Tools.NotifyAllOps("New updates have been found for: " + updates);
                                //Tools.NotifyAllOps("Updates are ready for: " + updates + ", 2 plugins(s)"); Future use
                                break;
                            case ResponseCode.StringResponse:
                                try
                                {
                                    var str = Encoding.UTF8.GetString(data);
                                    if (str != null)
                                    {
                                        ProgramLog.Log("Heartbeat Sent: " + str);
                                    }
                                }
                                catch { }
                                break;
                            default:
                                ProgramLog.Log("Invalid heartbeat response.");
                                break;
                        }
                    }
                    else ProgramLog.Log("Failed get a heartbeat response.");
                }
            }
            catch
            {
                ProgramLog.Log("Heartbeat failed, are we online or is the tdsm server down?");
            }
        }

        /// <summary>
        /// Used to construct a consistent id, meerly to detect multiple servers on the same IP (e.g. different port)
        /// </summary>
        /// <returns></returns>
        static string ConstructUUID()
        {
            var str = String.Empty;

            str += Terraria.Netplay.serverPort;

            using (var hasher = new SHA256Managed())
            {
                var output = hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
                return BitConverter.ToString(output).Replace("-", String.Empty);
            }
        }

        public static void Begin(int coreBuild)
        {
#if ENABLED
            if (_timer == null)
            {
                _coreBuild = coreBuild;
                _timer = new System.Timers.Timer(10 * 1000); //1000 * 60 * MinuteInterval);
                _timer.Elapsed += (e, a) =>
                {
                    if (Enabled) Beat();
                };
                _timer.Start();
                Enabled = true;
            }
            else if (!_timer.Enabled) _timer.Enabled = true;
#endif
        }

        public static void End()
        {
#if ENABLED
            if (_timer != null) _timer.Enabled = false;
            Enabled = false;
#endif
        }
    }
}
