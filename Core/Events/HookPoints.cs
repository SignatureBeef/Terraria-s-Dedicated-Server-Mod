using OTA.Plugin;

namespace TDSM.Core.Events
{
    public static class HookPoints
    {
        public static readonly HookPoint<HookArgs.PreApplyServerSideCharacter> PreApplyServerSideCharacter;
        public static readonly HookPoint<HookArgs.PostApplyServerSideCharacter> PostApplyServerSideCharacter;

        static HookPoints()
        {
            PreApplyServerSideCharacter = new HookPoint<HookArgs.PreApplyServerSideCharacter>("pre-apply-server-side-character");
            PostApplyServerSideCharacter = new HookPoint<HookArgs.PostApplyServerSideCharacter>("post-apply-server-side-character");
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
    }
}

