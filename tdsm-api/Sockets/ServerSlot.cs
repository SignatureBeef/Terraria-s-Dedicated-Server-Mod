using System;

namespace TDSM.API.Sockets
{
    public enum SlotState : int
    {
        PLAYER_AUTH = -2,
        SERVER_AUTH = -1,

        NONE = 0,

        CONNECTING = 1,
        ACCEPTED = 2,
        SENDING_TILES = 3,

        PLAYING = 10
    }

    //[Flags]
    //public enum SlotState : int
    //{
    //    NONE = 0,

    //    SHUTDOWN = 1,        // the client's socket is being shut down unconditionally
    //    KICK = 2,            // the client is being kicked, disconnect him after sending him all remaining data

    //    VACANT = 4,           //                this socket has no client and is available
    //    CONNECTED = 8,        // previously 0,  the client socket has been accepted
    //    SERVER_AUTH = 16,     //           -1,  the client has been asked for a server password
    //    ACCEPTED = 32,        //            1,  the client has successfully authenticated
    //    PLAYER_AUTH = 64,     //                the client has been asked for a character password
    //    QUEUED = 128,         //                the client is waiting for a free slot
    //    ASSIGNING_SLOT = 256,     //                the client has been assigned a slot after waiting
    //    SENDING_WORLD = 512,  //            2,  the client requested world info
    //    SENDING_TILES = 1024, //            3,  the client requested tiles
    //    PLAYING = 2048,       //            10

    //    // composites
    //    DISCONNECTING = KICK | SHUTDOWN,
    //    // before a slot is assigned (whoAmI is invalid)
    //    UNASSIGNED = KICK | SHUTDOWN | VACANT | CONNECTED | SERVER_AUTH | ACCEPTED | PLAYER_AUTH | QUEUED,

    //    ALL = 4095,
    //}

    //public static class SlotStateExtensions
    //{
    //    public static bool DisconnectInProgress(this SlotState state)
    //    {
    //        return (state & SlotState.DISCONNECTING) != 0;
    //    }
    //}
}

