using System;
using Microsoft.Xna.Framework;
using TDSM.Core.Command;
using OTA.Command;
using TDSM.Core.RemoteConsole;

namespace OTA
{
    /// <summary>
    /// ISender extensions
    /// </summary>
    public static class ISenderExtensions
    {
        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="recpt">Recpt.</param>
        /// <param name="message">Message.</param>
        public static bool HasAccessLevel(this ISender sender, AccessLevel accessLevel)
        {
            switch (accessLevel)
            {
                case AccessLevel.CONSOLE:
                    return sender is ConsoleSender;
                case AccessLevel.OP:
                    return sender is ConsoleSender || sender is RConSender;
                case AccessLevel.PLAYER:
                    return sender is BasePlayer;
                case AccessLevel.REMOTE_CONSOLE:
                    return sender is RConSender;
            }
            return false;
        }
    }
}

