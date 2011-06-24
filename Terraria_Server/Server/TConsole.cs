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

        public TConsole(string serverLog)
        {
            streamWriter = new StreamWriter(serverLog);
        }

        public void WriteLine(string Line)
        {
            Console.WriteLine(Line);
            streamWriter.WriteLine(Line);
            streamWriter.Flush();
        }

        public void WriteLine()
        {
            Console.WriteLine();
            streamWriter.WriteLine();
            streamWriter.Flush();
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
