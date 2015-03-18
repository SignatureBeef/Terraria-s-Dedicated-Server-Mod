
using System;
using tdsm.api;
namespace Microsoft.Xna.Framework
{
    public class GameWindow
    {
        public string Title
        {
            get
            { return Console.Title; }
            set
            {
                SetTitle(value);
            }
        }

        public static void SetTitle(string title)
        {
            Console.Title = title + " | TDSM Rebind build " + Globals.Build + Globals.PhaseToSuffix(Globals.BuildPhase);
        }

        public IntPtr Handle { get; set; }
        public bool AllowUserResizing { get; set; }
    }
}
