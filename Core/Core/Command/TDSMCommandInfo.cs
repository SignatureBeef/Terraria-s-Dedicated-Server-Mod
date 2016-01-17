using System;

namespace TDSM.Core.Command
{
    public class TDSMCommandInfo : OTA.Commands.CommandDefinition<TDSMCommandInfo>
    {
        private AccessLevel? _accessLevel;

        public TDSMCommandInfo(string prefix) : base(prefix) { }

        public TDSMCommandInfo WithAccessLevel(AccessLevel accessLevel)
        {
            _accessLevel = accessLevel;
            return this;
        }
    }
}

