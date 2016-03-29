using System;
using TDSM.Core.Config;

namespace TDSM.Core
{
    public class TDSMConfig : ComponentConfiguration<TDSMConfig>
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

