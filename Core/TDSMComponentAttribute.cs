using System;
using System.Linq.Expressions;

namespace TDSM.Core
{
    public enum ComponentEvent : byte
    {
        Initialise = 1,

        Enabled,

        ReadyForCommands,

        ServerStarting,
        ServerStopping,
        ServerInitialising,
        ServerTick
    }

    /// <summary>
    /// Used to dynamically load components
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TDSMComponentAttribute : Attribute
    {
        public ComponentEvent ComponentEvent { get; private set; }

        public TDSMComponentAttribute(ComponentEvent componentEvent)
        {
            this.ComponentEvent = componentEvent;
        }
    }
}

