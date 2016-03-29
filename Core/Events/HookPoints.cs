using OTA.Plugin;
using System.Data;
using TDSM.Core.Data.Models;

namespace TDSM.Core.Events
{
    public static class HookPoints
    {
        public static readonly HookPoint<HookArgs.PreApplyServerSideCharacter> PreApplyServerSideCharacter;
        public static readonly HookPoint<HookArgs.PostApplyServerSideCharacter> PostApplyServerSideCharacter;
        public static readonly HookPoint<HookArgs.PlayerRegistered> PlayerRegistered;

        static HookPoints()
        {
            PreApplyServerSideCharacter = new HookPoint<HookArgs.PreApplyServerSideCharacter>("pre-apply-server-side-character");
            PostApplyServerSideCharacter = new HookPoint<HookArgs.PostApplyServerSideCharacter>("post-apply-server-side-character");
            PlayerRegistered = new HookPoint<HookArgs.PlayerRegistered>("player-registered");
        }
    }

    public static class HookArgs
    {
        public struct PreApplyServerSideCharacter
        {
            public ServerCharacters.ServerCharacter Character { get; set; }
        }

        public struct PostApplyServerSideCharacter
        {

        }

        public struct PlayerRegistered
        {
            public IDbConnection Connection { get; set; }
            public IDbTransaction Transaction { get; set; }
            public DbPlayer Player { get; set; }
        }
    }
}

