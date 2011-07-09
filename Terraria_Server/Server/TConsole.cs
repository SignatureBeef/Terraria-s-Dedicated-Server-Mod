
using System.IO;
using System;
namespace Terraria_Server
{
    public class TConsole
    {
        private StreamWriter streamWriter = null;
        private Platform.PlatformType platformType;

        public TConsole(String serverLog, Platform.PlatformType platformType)
        {
            streamWriter = new StreamWriter(serverLog);
            this.platformType = platformType;
        }

        public void WriteLine(String Line)
        {
            Console.WriteLine(Line);
            streamWriter.WriteLine(Line);
            streamWriter.Flush();
        }
		
		public void WriteLine (string fmt, params object[] args)
		{
            Console.WriteLine (fmt, args);
            streamWriter.WriteLine (fmt, args);
            streamWriter.Flush();
		}
		
        public void WriteLine()
        {
            //if (platformType == Platform.PlatformType.WINDOWS)
            //{
                Console.WriteLine();
            //    streamWriter.WriteLine();
            //    streamWriter.Flush();
            //}
        }

        public void Write(String Message)
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
