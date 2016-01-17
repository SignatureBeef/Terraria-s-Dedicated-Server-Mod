using System;
using Microsoft.Xna.Framework;
using TDSM.Core.Command;
using OTA.Command;
using TDSM.Core.RemoteConsole;
using System.Collections.Generic;
using System.Linq;

namespace OTA
{
    /// <summary>
    /// Command extensions
    /// </summary>
    public static class CommandExtensions
    {
        public static IEnumerable<TDSMCommandInfo> GetTDSMCommandsForAccessLevel(this OTA.Commands.CommandParser parser, AccessLevel accessLevel)
        {
            return OTA.Commands.CommandManager.Parser.commands
                .Where(x => x.Value is TDSMCommandInfo)
                .Select(y => y.Value as TDSMCommandInfo)
                .Where(z => z._accessLevel.HasValue && z._accessLevel == accessLevel);
        }
    }
}

