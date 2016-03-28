using System;

namespace TDSM.Core.Command
{
    public class TDSMCommandInfo : OTA.Commands.CommandDefinition<TDSMCommandInfo>
    {
        internal AccessLevel? _accessLevel;

        public TDSMCommandInfo(string[] aliases) : base(aliases) { }

        public TDSMCommandInfo WithAccessLevel(AccessLevel accessLevel)
        {
            _accessLevel = accessLevel;
            return this;
        }
    }
}

