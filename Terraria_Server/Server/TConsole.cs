
using System.IO;
using System;
using Terraria_Server.Definitions;
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

        public void WriteLine(String Line) //FIXME: replace with proper async logging
        {
            var now = DateTime.Now;
            Console.Write (now);
            Console.Write ("> ");
            Console.WriteLine(Line);
            
            streamWriter.Write (now);
            streamWriter.Write ("> ");
            streamWriter.WriteLine(Line);
            streamWriter.Flush();
        }
		
		public void WriteLine (string fmt, params object[] args)
		{
            var now = DateTime.Now;
            Console.Write (now);
            Console.Write ("> ");
            Console.WriteLine (fmt, args);
            
            streamWriter.Write (now);
            streamWriter.Write ("> ");
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
