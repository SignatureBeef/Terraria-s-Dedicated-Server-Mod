using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Terraria_Server
{
    public class TConsole
    {
        StreamWriter streamWriter = null;
        int platform = 0;

        public TConsole(string serverLog, int Platform)
        {
            streamWriter = new StreamWriter(serverLog);
            platform = Platform;
        }

        public void WriteLine(string Line)
        {
            Console.WriteLine(Line);
            streamWriter.WriteLine(Line);
            streamWriter.Flush();
        }

        public void WriteLine()
        {
            if (platform == 0)
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
