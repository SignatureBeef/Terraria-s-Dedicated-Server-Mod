using System;
using System.IO;

namespace TDSM.API.Misc
{
    public static class Platform
    {
        static Platform()
        {
            InitPlatform();
        }

        public static PlatformType Type { get; set; }

        public enum PlatformType : int
        {
            UNKNOWN = 0,
            LINUX = 1,
            MAC = 2,
            WINDOWS = 3
        }

        public static void InitPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    Type = (
                        Directory.Exists("/Applications")
                        && Directory.Exists("/System")
                        && Directory.Exists("/Users")
                        && Directory.Exists("/Volumes")
                    ) ? PlatformType.MAC : PlatformType.LINUX;
                    break;
                case PlatformID.MacOSX:
                    Type = PlatformType.MAC;
                    break;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                case PlatformID.Xbox:
                    Type = PlatformType.WINDOWS;
                    break;
                default:
                    Type = PlatformType.UNKNOWN;
                    break;
            }
        }
    }
}