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
        TILE_CHANGE,
        PLAYER_HURT,
        PLAYER_CHEST,
        PLAYER_STATEUPDATE,
        PLAYER_DEATH,
        DOOR_STATECHANGE,
        PLAYER_EDITSIGN,
        PLAYER_PROJECTILE //,
        //PLAYER_DESTROYSIGN
    }
}
