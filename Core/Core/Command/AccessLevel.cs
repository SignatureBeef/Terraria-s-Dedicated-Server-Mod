using System;

namespace TDSM.Core.Command
{
    /// <summary>
    /// Default access privileges.
    /// </summary>
    public enum AccessLevel : int
    {
        PLAYER = 1,
        OP,
        REMOTE_CONSOLE,
        CONSOLE,
    }
}

