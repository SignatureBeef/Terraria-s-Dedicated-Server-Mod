using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Terraria_Server.Logging;
using Terraria_Server.Networking;
using Terraria_Server.Plugins;

namespace Terraria_Server.Messages
{
	public static class MessageDispatcher
	{
		private static MessageHandler[] messageArray = GetMessageArray();
		
		/// <summary>
		/// Load all IMessage types into an indexed array at application start.
		/// This should allow us to process Events extremely quickly while cutting down
		/// on how much code we have to hold in our head to understand each Event.
		/// </summary>
		private static MessageHandler[] GetMessageArray()
		{
			MessageHandler[] tempArray = new MessageHandler[255];
			
			//Load all the Events found in the current assembly into the the message array.
			Type type = typeof(MessageHandler);
			foreach(Type messageType in AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(clazz => clazz.GetTypes()).Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract))
			{
				MessageHandler message = (MessageHandler)Activator.CreateInstance(messageType);
				tempArray[(int)message.GetPacket()] = message;
			}
		
			return tempArray;
		}

		public static void Dispatch (ClientConnection conn, byte[] readBuffer, int start, int length)
		{
			try
			{
				int num = start + 1;
				byte pkt = readBuffer[start];
	
				if (conn.State == SlotState.SERVER_AUTH && pkt != 38)
				{
					conn.Kick ("Incorrect password.");
					return;
				}
				
				if ((conn.State & SlotState.DISCONNECTING) == 0)
				{
					var handler = messageArray[pkt];
					var state = conn.State;
					
					if (handler != null)
					{
						//ProgramLog.Debug.Log ("{2}, packet {0}, len {1}", (Packet)readBuffer[start], length, conn.State);
						
						if ((state & handler.IgnoredStates) != 0)
						{
							//ProgramLog.Debug.Log ("ignoring");
						}
						else if ((state & handler.ValidStates) != 0)
						{
							handler.Process (conn, readBuffer, length, num);
						}
						else
						{
							ProgramLog.Debug.Log ("{0}: sent message {1} in state {2}.", conn.RemoteAddress, (pkt > 0 && pkt <= 51) ? (object)(Packet)pkt : pkt, conn.State);
							conn.Kick ("Invalid operation in this state.");
						}
					}
                    else 
                    {
                        var ctx = new HookContext()
                        {

                        };
                        var args = new HookArgs.UnkownReceivedPacket()
                        {
                            Conn = conn,
                            Length = length,
                            Start = start,
                            ReadBuffer = readBuffer
                        };

                        //ProgramLog.Debug.Log("Received unknown packet {0}", pkt);

                        HookPoints.UnkownReceivedPacket.Invoke(ref ctx, ref args);

                        if (ctx.Result != HookResult.IGNORE && state != SlotState.PLAYING) // this is what stock would do
                            conn.Kick(String.Format("Message not understood ({0}).", pkt));
                    }
				}
			}
			catch (Exception e)
			{
				string pkt = "invalid packet";
				if (readBuffer.Length > start)
					pkt = String.Format ("packet {0}", (Packet)readBuffer[start]);

				ProgramLog.Log (e, String.Format ("Exception handling {0} of length {1} from {2}",
					pkt, length, conn.RemoteAddress));
					
				conn.Kick ("Server malfunction, please reconnect.");
			}
		}
	}
}
