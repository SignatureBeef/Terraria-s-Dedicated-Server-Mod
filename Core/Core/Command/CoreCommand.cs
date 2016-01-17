using System;
using OTA.Command;
using System.Collections.Generic;
using OTA;
using System.Linq;
using OTA.Extensions;
using OTA.Commands;

namespace TDSM.Core.Command
{
    public abstract class CoreCommand
    {
        public Entry Core { get; private set; }

        public abstract void Initialise();

        public TDSMCommandInfo AddCommand(string prefix)
        {
            return AddCommand(prefix);
        }

        [TDSMComponent(ComponentEvent.Initialise)]
        public static void Initialise(Entry core)
        {
            var type = typeof(CoreCommand);
            foreach (var messageType in typeof(CoreCommand).Assembly
                .GetTypesLoaded()
                .Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract))
            {
                var cmd = (CoreCommand)Activator.CreateInstance(messageType);
                cmd.Core = core;

                cmd.Initialise();
            }
        }
    }
}

