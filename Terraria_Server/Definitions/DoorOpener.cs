using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Plugin
{
    /// <summary>
    /// Enumerates whether a door was opened by a player, NPC, the server, or some unknown entity
    /// </summary>
    public enum DoorOpener
    {
        PLAYER,
        NPC,
        SERVER,
        UNKNOWN
    }
}
