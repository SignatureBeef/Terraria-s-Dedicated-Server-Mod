using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public enum ServerState
    {
        GENERATING,
        INITIALIZING,
        STARTING,
        LOADING,
        LOADED,
        RESTARTING,
        STOPPING
    }
}
