using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
    public struct GameOverlayActivated_t
    {
        public byte m_bActive;
    }

    public sealed class Callback<T>
    {
        public delegate void DispatchDelegate(T param);
    }

    public static class SteamAPI
    {
        public static bool Init()
        {
            return false;
        }

        public static void Shutdown()
        { 
        }

        public static void RunCallbacks()
        { 
        }

        public static bool IsSteamRunning()
        {
            return false;
        }
    }
}

