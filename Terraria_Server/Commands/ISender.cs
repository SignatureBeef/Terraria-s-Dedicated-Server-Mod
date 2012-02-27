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
            recpt.sendMessage(String.Format(message, args));
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

		public static bool IsSender(this ISender sender, SenderType type)
		{
			switch (type)
			{
				case SenderType.CONSOLE:
					return sender is ConsoleSender && !(sender is RemoteConsole.RConSender);
				case SenderType.PLAYER:
					return sender is Player;
				case SenderType.RCON:
					return sender is RemoteConsole.RConSender;
				default:
					throw new InvalidOperationException("No such sender.");
			}
		}

		public static SenderType GetSenderType(this ISender sender)
		{
			if (sender is ConsoleSender && !(sender is RemoteConsole.RConSender))
				return SenderType.CONSOLE;
			else if (sender is Player)
				return SenderType.PLAYER;
			else if (sender is RemoteConsole.RConSender)
				return SenderType.RCON;

			return SenderType.UNKNOWN;
		}
	}

	public enum SenderType
	{
		CONSOLE,
		PLAYER,
		RCON,
		UNKNOWN
	}
}
