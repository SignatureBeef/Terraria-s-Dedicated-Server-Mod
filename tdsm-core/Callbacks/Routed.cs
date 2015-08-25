
using tdsm.core.Logging;
namespace tdsm.core.Callbacks
{
    public static class Routed //Refactor me to something appropriate
    {
        public static void WriteLine(string fmt) //, params object[] args)
        {
            ProgramLog.Log(fmt);
        }
    }
}
