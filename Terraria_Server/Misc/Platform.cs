using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Misc
{
    public static class Platform
    {
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
            Type = PlatformType.UNKNOWN;

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    Type = PlatformType.WINDOWS;
                    break;
                case PlatformID.Win32S:
                    Type = PlatformType.WINDOWS;
                    break;
                case PlatformID.Win32Windows:
                    Type = PlatformType.WINDOWS;
                    break;
                case PlatformID.WinCE:
                    Type = PlatformType.WINDOWS;
                    break;
                case PlatformID.MacOSX:
                    Type = PlatformType.MAC;
                    break;
            }

            if(Type == PlatformType.UNKNOWN)
            {
                int platformCode = (int)Environment.OSVersion.Platform;
                if (platformCode == 4 || platformCode == 6 || platformCode == 128)
					Type = PlatformType.LINUX;
            }
        }
    }
}
