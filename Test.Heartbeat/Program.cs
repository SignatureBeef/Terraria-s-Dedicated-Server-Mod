using System;
//using System.Threading.Tasks;

namespace Test.Heartbeat
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync();//.Wait();
        }

        static /*async Task*/ void MainAsync()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Beating...");
            Console.ForegroundColor = ConsoleColor.White;
            /*await */TDSM.Core.Net.Web.Heartbeat.Beat(false);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Complete. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
