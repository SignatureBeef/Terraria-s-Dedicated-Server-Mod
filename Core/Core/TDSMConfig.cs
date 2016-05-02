using OTA;
using OTA.Logging;
using OTA.Misc;
using System;
using System.Diagnostics;
using TDSM.Core.Config;
using Terraria;
using Terraria.IO;

namespace TDSM.Core
{
    public class StockConfig : ComponentConfiguration
    {
        [ConfigPrefix("motd")]
        public string MessageOfTheDay
        {
            get
            { return Main.motd; }
            set
            { Main.motd = value; }
        }

        [ConfigPrefix("world")]
        public string World
        {
            get
            { return Main.ActiveWorldFileData.Path; }
            set
            { Main.ActiveWorldFileData = WorldFile.GetAllMetadata(value, false); }
        }

        [ConfigPrefix("port")]
        public int Port
        {
            get
            { return Netplay.ListenPort; }
            set
            { Netplay.ListenPort = value; }
        }

        [ConfigPrefix("maxplayers")]
        public int MaxPlayers
        {
            get
            { return Main.maxNetPlayers; }
            set
            { Main.maxNetPlayers = value; }
        }

        [ConfigPrefix("priority")]
        public int ProcessPriority
        {
            set
            {
                if (!Program.LaunchParameters.ContainsKey("-forcepriority"))
                {
                    if (Tools.RuntimePlatform != RuntimePlatform.Mono)
                    {
                        try
                        {
                            int priority = Convert.ToInt32(value);
                            if (priority >= 0 && priority <= 5)
                            {
                                Process currentProcess = Process.GetCurrentProcess();
                                if (priority == 0)
                                    currentProcess.PriorityClass = ProcessPriorityClass.RealTime;
                                else if (priority == 1)
                                    currentProcess.PriorityClass = ProcessPriorityClass.High;
                                else if (priority == 2)
                                    currentProcess.PriorityClass = ProcessPriorityClass.AboveNormal;
                                else if (priority == 3)
                                    currentProcess.PriorityClass = ProcessPriorityClass.Normal;
                                else if (priority == 4)
                                    currentProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
                                else if (priority == 5)
                                    currentProcess.PriorityClass = ProcessPriorityClass.Idle;
                            }
                            else ProgramLog.Log("Invalid priority value: " + priority);
                        }
                        catch
                        {
                        }
                    }
                    else
                        ProgramLog.Log("Skipped setting process priority on mono");
                }
            }
        }

        [ConfigPrefix("password")]
        public string Password
        {
            get
            { return Netplay.ServerPassword; }
            set
            { Netplay.ServerPassword = value; }
        }

        [ConfigPrefix("lang")]
        public int Language
        {
            get
            { return Lang.lang; }
            set
            { Lang.lang = value; }
        }

        [ConfigPrefix("worldpath")]
        public string WorldPath
        {
            get
            { return Main.WorldPath; }
            set
            { Main.WorldPath = value; }
        }

        [ConfigPrefix("worldname")]
        public string WorldName
        {
            get
            { return Main.worldName; }
            set
            { Main.worldName = value; }
        }

        [ConfigPrefix("banlist")]
        public string BanList
        {
            get
            { return Netplay.BanFilePath; }
            set
            { Netplay.BanFilePath = value; }
        }

        [ConfigPrefix("difficulty")]
        public string ExpertMode
        {
            get
            { return Main.expertMode ? "1" : "0"; }
            set
            { Main.expertMode = value == "1"; }
        }

        [ConfigPrefix("autocreate")]
        public int AutoCreate
        {
            set
            {
                switch (value)
                {
                    case 0:
                        Terraria.Main.autoGen = false;
                        break;
                    case 1:
                        Terraria.Main.maxTilesX = 4200;
                        Terraria.Main.maxTilesY = 1200;
                        Terraria.Main.autoGen = true;
                        break;
                    case 2:
                        Terraria.Main.maxTilesX = 6300;
                        Terraria.Main.maxTilesY = 1800;
                        Terraria.Main.autoGen = true;
                        break;
                    case 3:
                        Terraria.Main.maxTilesX = 8400;
                        Terraria.Main.maxTilesY = 2400;
                        Terraria.Main.autoGen = true;
                        break;
                }
            }
        }

        [ConfigPrefix("secure")]
        public string SpamCheck
        {
            get
            { return Netplay.spamCheck ? "1" : "0"; }
            set
            { Netplay.spamCheck = value == "1"; }
        }

        [ConfigPrefix("upnp")]
        public string UseUPNP
        {
            set
            {
                Netplay.UseUPNP = value == "1";
                if (Netplay.UseUPNP && Tools.RuntimePlatform == RuntimePlatform.Mono)
                {
                    ProgramLog.Log("[Warning] uPNP is only available on Windows platforms.");
                    Netplay.UseUPNP = false;
                }
            }
        }

        [ConfigPrefix("npcstream")]
        public int NpcStream
        {
            get
            { return Main.npcStreamSpeed; }
            set
            { Main.npcStreamSpeed = value; }
        }
    }

    public class TDSMConfig : StockConfig
    {
        [ConfigPrefix("pid-file")]
        public string ProcessPIDFile { get; set; }

        [ConfigPrefix("whitelist")]
        public bool WhitelistEnabled { get; set; }

        [ConfigPrefix("default-player-group")]
        public string DefaultPlayerGroup { get; set; }

        #region Heartbeat

        [ConfigPrefix("server-list")]
        public bool Heartbeat_PublishToList { get; set; }

        [ConfigPrefix("server-list-name")]
        public string Heartbeat_ServerName { get; set; }

        [ConfigPrefix("server-list-desc")]
        public string Heartbeat_ServerDescription { get; set; }

        [ConfigPrefix("server-list-domain")]
        public string Heartbeat_ServerDomain { get; set; }

        [ConfigPrefix("heartbeat")]
        public bool EnableHeartbeat { get; set; } = true;

        [ConfigPrefix("heartbeat-scheme")]
        public string Heartbeat_Scheme { get; set; } = "https://";

        #endregion

        #region RCON

        [ConfigPrefix("rcon-hash-nonce")]
        public string Rcon_HashNonce { get; set; }

        [ConfigPrefix("rcon-bind-address")]
        public string Rcon_BindAddress { get; set; }

        #endregion

        #region Web

        [ConfigPrefix("web-server-bind-address")]
        public string Web_BindAddress { get; set; }

        #endregion

        [ConfigPrefix("cheat-detection")]
        public bool CheatDetection { get; set; }

        [ConfigPrefix("log-rotation")]
        public bool LogRotation { get; set; }

        [ConfigPrefix("exit-access-level")]
        public int ExitAccessLevel { get; set; }

        #region SSC

        [ConfigPrefix("server-side-characters")]
        public string SSC_CharacterMode { get; set; }

        [ConfigPrefix("ssc-allow-guest-info")]
        public bool SSC_AllowGuestInfo { get; set; }

        [ConfigPrefix("ssc-save-interval")]
        public int SSC_SaveInterval { get; set; }

        #endregion

        #region WebAPI

        [ConfigPrefix("api-show-plugins")]
        public bool API_ShowPlugins { get; set; }

        #endregion

        #region Maintenance

        [ConfigPrefix("logs-to-keep")]
        public int Maintenance_LogsToKeep { get; set; } = 5;

        [ConfigPrefix("backup-interval-min")]
        public int Maintenance_BackupIntervalMinutes { get; set; } = 10;

        [ConfigPrefix("backup-expiry-min")]
        public int Maintenance_BackupExpiryMinutes { get; set; } = 40;

        [ConfigPrefix("compress-backups")]
        public bool Maintenance_CompressBackups { get; set; } = true;

        [ConfigPrefix("copy-backups")]
        public bool Maintenance_CopyBackups { get; set; } = true;

        [ConfigPrefix("save-interval-min")]
        public int Maintenance_SaveIntervalMinutes { get; set; } = 10;

        #endregion

        #region Database
        [ConfigPrefix("database-provider")]
        public string DatabaseProvider { get; set; }

        [ConfigPrefix("database-connection")]
        public string DatabaseConnectionString { get; set; }
        #endregion
    }
}

