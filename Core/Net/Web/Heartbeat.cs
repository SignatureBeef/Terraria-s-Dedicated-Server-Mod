#define ENABLED

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using OTA;
using OTA.Misc;
using OTA.Logging;

namespace TDSM.Core.Net.Web
{
    /// <summary>
    /// This is a multi purpose class.
    /// 1) It sends non identifiable data to the heatbeat server for us to see the stats (when allowed)
    /// 2) The heartbeat server will reply with particular case data. Responses are:
    ///     a) 1 = OK
    ///     b) 2 = Update available
    /// </summary>
    public static class Heartbeat
    {
        internal const String EndPoint = "https://openterraria.com/tdsm/heartbeat/beat";
        internal const Double MinuteInterval = 5;

        private static System.Timers.Timer _timer;
        private static int _coreBuild;

        public static bool Enabled { get; private set; }

        public static bool PublishToList { get; set; }

        public static string ServerName { get; set; }

        public static string ServerDescription { get; set; }

        public static string ServerDomain { get; set; }

        public static bool RequiresAuthentication { get; set; }

        public enum ResponseCode : int
        {
            UpToDate = 1,
            ServerKey = 2,

            //Maybe a notice (flag for server/console and players - must be allowed in config)
            StringResponse = 255
        }

        public class HeartbeatResponse
        {
            public ResponseCode Code { get; set; }
            public string Value { get; set; }
        }

        private static string _serverKey = GetServerKey();

        static string GetServerKey()
        {
            var file = Path.Combine(Globals.DataPath, "server.key");
            if (File.Exists(file))
            {
                var key = File.ReadAllText(file);
                if (key.Length == 36)
                    return key;
            }
            return null;
        }

        static void SetServerKey(string key)
        {
            var file = Path.Combine(Globals.DataPath, "server.key");
            if (File.Exists(file))
                File.Delete(file);

            File.WriteAllText(file, key);
            _serverKey = key;
        }

        struct BufferReader
        {
            public byte[] DataBuffer;
            public int Index;

            public BufferReader(byte[] buff)
            {
                DataBuffer = buff;
                Index = 0;
            }

            public static implicit operator BufferReader(byte[] b)
            {
                return new BufferReader(b);
            }

            public ResponseCode ReadResponseCode()
            {
                return (ResponseCode)DataBuffer[Index++];
            }

            //public UpdateReady ReadUpdateReady()
            //{
            //    return (UpdateReady)DataBuffer[Index++];
            //}

            public int ReadInt32()
            {
                Index += 4;
                return BitConverter.ToInt32(DataBuffer, Index - 4);
            }

            public string ReadString(int length = 0)
            {
                int len = length > 0 ? length : DataBuffer.Length - Index;
                if (len > 0)
                {
                    var dta = new byte[len];
                    Buffer.BlockCopy(DataBuffer, Index, dta, 0, dta.Length);
                    Index += len;
                    return Encoding.UTF8.GetString(dta);
                }
                return null;
            }

            public string ReadStringByDef()
            {
                return ReadString(ReadInt32());
            }
        }

        [TDSMComponent(ComponentEvent.ReadyForCommands)]
        public static void OnReadyForCommands(Entry core)
        {
            if (core.Config.EnableHeartbeat)
            {
                ServerName = core.Config.Heartbeat_ServerName;
                ServerDescription = core.Config.Heartbeat_ServerDescription;
                ServerDomain = core.Config.Heartbeat_ServerDomain;
                Heartbeat.Begin(Entry.CoreBuild);
            }
        }

        static void Beat()
        {
            try
            {
                using (var wc = new WebClient())
                {
                    //TODO; Maybe plugin versions
                    //TODO; think about branches, release or dev

                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        API = new
                        {
                            Major = Globals.Version.Major,
                            Minor = Globals.Version.Minor
                        },
                        Core = _coreBuild,

                        Platform = (int)Platform.Type,
                        OSPlatform = (int)Environment.OSVersion.Platform,

                        ServerKey = _serverKey,

                        NpcVersion = Definitions.DefinitionManager.NPCVersion,
                        ItemVersion = Definitions.DefinitionManager.ItemVersion,

                        RequiresAuthentication = !String.IsNullOrEmpty(Terraria.Netplay.ServerPassword) || RequiresAuthentication,

                        Port = PublishToList ? Terraria.Netplay.ListenPort : 0,
                        MaxPlayers = PublishToList ? Terraria.Main.maxNetPlayers : 0,
                        Name = PublishToList ? ServerName : String.Empty,
                        Description = PublishToList ? ServerDescription : String.Empty,
                        Domain = PublishToList ? ServerDomain : String.Empty,

                        Plugins = OTA.Plugin.PluginManager.Loaded.NameAndVersions.ToArray()
                    });

                    wc.Headers.Add("Content-Type: application/json");

                    var data = wc.UploadData(EndPoint, "POST", UTF8Encoding.Default.GetBytes(json));
                    if (data != null && data.Length > 0)
                    {
                        var message = Newtonsoft.Json.JsonConvert.DeserializeObject<HeartbeatResponse>(UTF8Encoding.Default.GetString(data));
                        if (message != null)
                        {
                            switch (message.Code)
                            {
                                case ResponseCode.ServerKey:
                                    try
                                    {
                                        if (!String.IsNullOrEmpty(message.Value) && message.Value.Length == 36)
                                        {
                                            SetServerKey(message.Value);
                                            Beat(); //Rebeat to acknowledge the new UUID
                                        }
                                    }
                                    catch
                                    {
                                    }
                                    break;
                                case ResponseCode.StringResponse:
                                    try
                                    {
                                        if (!String.IsNullOrEmpty(message.Value))
                                        {
                                            ProgramLog.Log(message.Value);
                                        }
                                    }
                                    catch
                                    {
                                    }
                                    break;
                                    //default:
                                    //    ProgramLog.Log("Invalid heartbeat response.");
                                    //    try
                                    //    {
                                    //        var str = reader.ReadString();
                                    //        if (!String.IsNullOrEmpty(str))
                                    //        {
                                    //            ProgramLog.Log("Heartbeat Sent: " + str);
                                    //        }
                                    //    }
                                    //    catch { }
                                    //    break;
                            }
                        }
                        else
                        {
                            ProgramLog.Log("Invalid heartbeat response.");
                        }
                    }
                    else
                        ProgramLog.Log("Failed get a heartbeat response.");
                }
            }
#if DEBUG
            catch (Exception e)
            {
                ProgramLog.Log("Heartbeat failed, are we online or is the tdsm server down?");
                ProgramLog.Log(e);
            }
#else
            catch
            {
                ProgramLog.Log("Heartbeat failed, are we online or is the tdsm server down?");
            }
#endif
        }

        ///// <summary>
        ///// Used to construct a consistent id, meerly to detect multiple servers on the same IP (e.g. different port)
        ///// </summary>
        ///// <returns></returns>
        //static string ConstructUUID()
        //{
        //    var str = String.Empty;

        //    str += Terraria.Netplay.serverPort;
        //    str += Environment.MachineName;

        //    using (var hasher = new SHA256Managed())
        //    {
        //        var output = hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
        //        return BitConverter.ToString(output).Replace("-", String.Empty);
        //    }
        //}

        public static void Begin(int coreBuild)
        {
#if ENABLED
            if (_timer == null)
            {
                _coreBuild = coreBuild;
                _timer = new System.Timers.Timer(1000 * 60 * MinuteInterval);
                var callback = new System.Timers.ElapsedEventHandler((e, a) =>
                    {
                        if (Enabled)
                            Beat();
                    });
                _timer.Elapsed += callback;
                _timer.Start();
                callback.BeginInvoke(null, null, new AsyncCallback((result) =>
                        {
                            (result.AsyncState as System.Timers.ElapsedEventHandler).EndInvoke(result);
                        }), callback);
                Enabled = true;

                if (PublishToList && String.IsNullOrEmpty(ServerName))
                {
                    ProgramLog.Error.Log("The server was instructed to be published but no server name was specified");
                }
            }
            else if (!_timer.Enabled)
            {
                _timer.Enabled = true;
                Enabled = true;
            }
#endif
        }

        public static void End()
        {
#if ENABLED
            if (_timer != null)
                _timer.Enabled = false;
            Enabled = false;
#endif
        }
    }
}
