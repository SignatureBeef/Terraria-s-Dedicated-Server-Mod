#define ENABLED

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TDSM.API;
using TDSM.API.Misc;
using TDSM.API.Logging;

namespace TDSM.Core
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
        internal const String EndPoint = "http://heartbeat.tdsm.org/";
        internal const Double MinuteInterval = 5;

        private static System.Timers.Timer _timer;
        private static int _coreBuild;

        public static bool Enabled { get; private set; }

        public static bool PublishToList { get; set; }

        public static string ServerName { get; set; }

        public static string ServerDescription { get; set; }

        public static string ServerDomain { get; set; }

        private static DateTime? _lastUpdateNotice;

        [Flags]
        public enum UpdateReady : byte
        {
            API = 1,
            Core = 2,
            NPCDefinitions = 4,
            ItemDefinitions = 8,
            PluginOutOfDate = 16
        }

        public enum ResponseCode : byte
        {
            UpToDate = 0,
            UpdateReady = 1,
            ServerKey = 2,

            //Maybe a notice (flag for server/console and players - must be allowed in config)
            StringResponse = 255
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

            public UpdateReady ReadUpdateReady()
            {
                return (UpdateReady)DataBuffer[Index++];
            }

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

        static void Beat()
        {
            try
            {
                using (var wc = new WebClient())
                {
                    var req = new System.Collections.Specialized.NameValueCollection();
//                    var req = new System.Collections.Generic.Dictionary<String, String>();
                    req.Add("API", Globals.Build.ToString());
                    req.Add("Core", _coreBuild.ToString());
                    req.Add("Platform", ((int)Platform.Type).ToString());
                    req.Add("OSPlatform", ((int)Environment.OSVersion.Platform).ToString());
                    if (!String.IsNullOrEmpty(_serverKey))
                        req.Add("UUID", _serverKey);
                    req.Add("NPCDef", Definitions.DefinitionManager.NPCVersion.ToString());
                    req.Add("ItemDef", Definitions.DefinitionManager.ItemVersion.ToString());
                    req.Add("RequiresAuth", (!String.IsNullOrEmpty(Terraria.Netplay.ServerPassword)).ToString());
                    //req.Add("ServiceTo", ServerCore.Server.UniqueConnections.ToString());

                    if (PublishToList)
                    {
                        req.Add("Port", Terraria.Netplay.ListenPort.ToString());
                        req.Add("MaxPlayers", Terraria.Main.maxNetPlayers.ToString());
                        //req.Add("ConnectedPlayers", ServerCore.ClientConnection.All.Count.ToString());

                        if (!String.IsNullOrEmpty(ServerName))
                            req.Add("Name", ServerName);
                        if (!String.IsNullOrEmpty(ServerDescription))
                            req.Add("Desc", ServerDescription);
                        if (!String.IsNullOrEmpty(ServerDomain))
                            req.Add("Domain", ServerDomain);
                    }

                    //TODO; Maybe plugin versions
                    //TODO; think about branches, release or dev

                    var data = wc.UploadValues(EndPoint, "POST", req);
//                    var parameters = (from x in req select String.Concat(x.Key, "=", Uri.EscapeDataString(x.Value))).ToArray();
//                    var data = wc.DownloadData(EndPoint + "?" + String.Join("&", parameters));
                    if (data != null && data.Length > 0)
                    {
                        var reader = (BufferReader)data;
                        switch (reader.ReadResponseCode())
                        {
                            case ResponseCode.UpToDate:
                                //We're online - all up to date
                                try
                                {
                                    var str = reader.ReadString();
                                    if (!String.IsNullOrEmpty(str))
                                    {
                                        ProgramLog.Log("Heartbeat Sent: " + str);
                                    }
                                }
                                catch
                                {
                                }
                                break;
                            case ResponseCode.UpdateReady:
                                var flag = reader.ReadUpdateReady();

                                var updates = String.Empty;
                                if ((flag & UpdateReady.API) != 0)
                                    updates += "API";
                                if ((flag & UpdateReady.Core) != 0)
                                    updates += (updates.Length > 0 ? ", " : String.Empty) + "Core";
                                if ((flag & UpdateReady.NPCDefinitions) != 0)
                                    updates += (updates.Length > 0 ? ", " : String.Empty) + "NPC definitions";
                                if ((flag & UpdateReady.ItemDefinitions) != 0)
                                    updates += (updates.Length > 0 ? ", " : String.Empty) + "Item definitions";

                                if ((flag & UpdateReady.PluginOutOfDate) != 0)
                                {
                                    var len = reader.ReadInt32();
                                    //for (var x = 0; x < len; x++)
                                    //{
                                    //    var name = reader.ReadStringByDef();
                                    //    if (!String.IsNullOrEmpty(name)) updates += (updates.Length > 0 ? ", " : String.Empty) + name;
                                    //}
                                    updates += (updates.Length > 0 ? ", " : String.Empty) + len + " plugin(s)";
                                }

                                if (!String.IsNullOrEmpty(updates))
                                {
                                    if (!_lastUpdateNotice.HasValue || (DateTime.Now - _lastUpdateNotice.Value).TotalMinutes >= 60)
                                    {
                                        Tools.NotifyAllOps("Updates are ready for: " + updates);
                                        _lastUpdateNotice = DateTime.Now;
                                    }
                                }
                                break;
                            case ResponseCode.ServerKey:
                                try
                                {
                                    var str = reader.ReadString();
                                    if (!String.IsNullOrEmpty(str) && str.Length == 36)
                                    {
                                        SetServerKey(str);
                                        Beat();
                                    }
                                }
                                catch
                                {
                                }
                                break;
                            case ResponseCode.StringResponse:
                                try
                                {
                                    var str = reader.ReadString();
                                    if (!String.IsNullOrEmpty(str))
                                    {
                                        ProgramLog.Log("Heartbeat Sent: " + str);
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
