using System;
using TDSM.API.Logging;


namespace TDSM.API.Command
{
    /// <summary>
    /// ConsoleSender extension of Sender.  Allows for new ConsoleCommandEvents
    /// </summary>
    public class ConsoleSender : Sender
    {
        static Action<String, Byte, Byte, Byte> _consoleMethod;

        public static ConsoleColor DefaultColour = ConsoleColor.Gray;

        /// <summary>
        /// Set the Console WriteLine method
        /// </summary>
        /// <param name="method"></param>
        public static void SetMethod(Action<String, Byte, Byte, Byte> method)
        {
            _consoleMethod = method;
        }

        /// <summary>
        /// ConsoleSender constructor
        /// </summary>
        public ConsoleSender()
        {
            Op = true;
            Name = "CONSOLE";
            // I don't know what the hell was the deal with this
        }

        public static System.ConsoleColor FromColor(byte r, byte g, byte b)
        {
            int index = (r > 128 | g > 128 | b > 128) ? 8 : 0; // Bright bit
            index |= (r > 64) ? 4 : 0; // Red bit
            index |= (g > 64) ? 2 : 0; // Green bit
            index |= (b > 64) ? 1 : 0; // Blue bit
            return (System.ConsoleColor)index;
        }

        public override void SendMessage(string message, int A = 255, byte R = 255, byte G = 255, byte B = 255)
        {
            if (_consoleMethod == null)
            {
                if (R == 255 && G == 255 && B == 255)
                    ProgramLog.Console.Print(message);
                else
                {
                    //Console.ForegroundColor = FromColor(R, G, B);
                    ProgramLog.Log(message, FromColor(R, G, B));
                    Console.ForegroundColor = DefaultColour;
                }
            }
            else
                _consoleMethod(message, R, G, B);
        }
    }
}