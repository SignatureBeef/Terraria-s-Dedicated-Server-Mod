using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server.Misc
{
    /// <summary>
    /// Used to break "Console.ReadLine()" when a player or another entity stops the server
    /// Otherwise the Command Loop will hang at Console.ReadLine until data is inputted.
    /// </summary>
    public class ExitException : Exception
    {
        public ExitException() { }

        public ExitException(string Info) : base(Info) { }
    }
}
