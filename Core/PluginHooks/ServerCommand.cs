using System;
using OTA.Command;
using OTA.Plugin;

namespace TDSM.Core.Plugin.Hooks
{
    public static partial class TDSMHookArgs
    {
        /// <summary>
        /// Server command arguments
        /// </summary>
        public struct ServerCommand
        {
            /// <summary>
            /// The command prefix that triggered the command.
            /// </summary>
            /// <example>help,say,exit</example>
            public string Prefix { get; internal set; }

            /// <summary>
            /// The ArgumentList instance that has pre-tokenised the arguments.
            /// </summary>
            public ArgumentList Arguments { get; set; }

            /// <summary>
            /// The full argument string received.
            /// </summary>
            public string ArgumentString { get; set; }
        }
    }

    public static partial class TDSMHookPoints
    {
        /// <summary>
        /// Occurs when the server console receives an unprocessed (yet valid) command.
        /// </summary>
        public static readonly HookPoint<TDSMHookArgs.ServerCommand> ServerCommand = new HookPoint<TDSMHookArgs.ServerCommand>();
    }
}

