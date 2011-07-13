
namespace Terraria_Server.Plugin
{
    /// <summary>
    /// All hooks currently available for processing
    /// </summary>
    public enum Hooks
    {
        CONSOLE_COMMAND,
        PLAYER_COMMAND,
        PLAYER_PRELOGIN,
        PLAYER_LOGIN,
        PLAYER_LOGOUT,
        PLAYER_PARTYCHANGE,
        PLAYER_CHAT,
        PLAYER_TILECHANGE,
        PLAYER_HURT,
        PLAYER_CHEST,
        PLAYER_STATEUPDATE,
        PLAYER_DEATH,
        DOOR_STATECHANGE,
        PLAYER_EDITSIGN,
        PLAYER_PROJECTILE,
        NPC_DEATH,
        NPC_SPAWN,
        PLAYER_TELEPORT,
        PLAYER_MOVE,
        PLAYER_KEYPRESS,
        PLAYER_PVPCHANGE,
        PLAYER_AUTH_QUERY,
        PLAYER_AUTH_REPLY,
        NPC_BOSSDEATH,
        NPC_BOSSSUMMON,
    }
}
