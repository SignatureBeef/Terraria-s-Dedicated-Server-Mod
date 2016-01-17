using System;
using OTA.Logging;
using Terraria;
using OTA;

namespace TDSM.Core
{
    public static class Utils
    {
        /// <summary>
        /// Notifies all ops.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="writeToConsole">If set to <c>true</c> write to console.</param>
        public static void NotifyAllOps(string message, bool writeToConsole = true) //, SendingLogger Logger = SendingLogger.CONSOLE)
        {
            foreach (var player in Main.player)
            {
                if (player != null && player.active && player.IsOp())
                    NetMessage.SendData((int)Packet.PLAYER_CHAT, player.whoAmI, -1, message, 255 /* PlayerId */, 176f, 196, 222f);
            }

            if (writeToConsole)
                ProgramLog.Log(message);
        }
    }
}

