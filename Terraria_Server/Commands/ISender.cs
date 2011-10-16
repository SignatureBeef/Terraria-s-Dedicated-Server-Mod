using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server.Misc;

namespace Terraria_Server
{
    public interface ISender
    {
        string Name { get; }
        bool Op { get; set; }
        void sendMessage(string Message, int A = 255, float R = 255f, float G = 0f, float B = 0f);
    }
    
	public static class ISenderExtensions
    {
        public static void Message(this ISender recpt, string message)
        {
            recpt.sendMessage(message);
        }

        public static void Message(this ISender recpt, string message, params object[] args)
        {
            recpt.sendMessage(message);
        }

        public static void Message(this ISender recpt, int sender, string message)
        {
            recpt.sendMessage(message, sender);
        }
		
		public static void Message (this ISender recpt, int sender, Color color, string message)
		{
			recpt.sendMessage (message, sender, color.R, color.G, color.B);
		}
		
		public static void Message (this ISender recpt, int sender, string fmt, params object[] args)
		{
			recpt.sendMessage (String.Format (fmt, args), sender);
		}
		
		public static void Message (this ISender recpt, int sender, Color color, string fmt, params object[] args)
		{
			recpt.sendMessage (String.Format (fmt, args), sender, color.R, color.G, color.B);
		}
	}
}
