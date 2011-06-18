using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Plugin
{
    public enum Hooks
    {
        CONSOLE_COMMAND,
        PLAYER_COMMAND,
        PLAYER_PRELOGIN,
        PLAYER_LOGIN,
        PLAYER_LOGOUT,
        PLAYER_PARTYCHANGE,
        PLAYER_CHAT,
        TILE_BREAK,
        PLAYER_HURT,
        PLAYER_CHEST,
        PLAYER_STATEUPDATE
    }
}
