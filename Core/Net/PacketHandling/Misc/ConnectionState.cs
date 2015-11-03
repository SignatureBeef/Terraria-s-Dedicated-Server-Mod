using System;

namespace TDSM.Core.Net.PacketHandling.Misc
{
    /// <summary>
    /// Additional TDSM connection states
    /// </summary>
    public enum ConnectionState : int
    {
        WaitingForUserPassword = -2
    }
}

