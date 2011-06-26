
using System.IO;
using System;
namespace Terraria_Server
{
    public class TConsole
    {
        private StreamWriter streamWriter = null;
        private Platform.PlatformType platformType;

        public TConsole(string serverLog, Platform.PlatformType platformType)
        {
            streamWriter = new StreamWriter(serverLog);
            this.platformType = platformType;
        }

        public void WriteLine(string Line)
        {
            Console.WriteLine(Line);
            streamWriter.WriteLine(Line);
            streamWriter.Flush();
        }

        public void WriteLine()
        {
            if (platformType == Platform.PlatformType.UNKNOWN)
            {
                Console.WriteLine();
                streamWriter.WriteLine();
                streamWriter.Flush();
            }
        }

        public void Write(string Message)
        {
            Console.Write(Message);
            streamWriter.Write(Message);
            streamWriter.Flush();
        }

        public void Close()
        {
            streamWriter.Close();
        }

    }
}
