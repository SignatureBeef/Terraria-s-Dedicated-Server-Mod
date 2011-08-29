using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Terraria_Server.Logging;
using Terraria_Server.Networking;

namespace Terraria_Server.Messages
{
    public class MessageBuffer
    {
        private static MessageHandler[] messageArray = GetMessageArray();

        /// <summary>
    	/// Load all IMessage types into an indexed array at application start.
    	/// This should allow us to process Events extremely quickly while cutting down
    	/// on how much code we have to hold in our head to understand each Event.
    	/// </summary>
        private static MessageHandler[] GetMessageArray()
        {
        	//Find the highest Packet value and make an array of that size to process Events.
            int highestPacket = 0;
            foreach (Packet packet in Enum.GetValues(typeof(Packet)))
            {
                int packetValue = (int)packet;
                if (packetValue >= highestPacket)
                {
                    highestPacket = packetValue + 1;
                }
            }
            MessageHandler[] tempArray = new MessageHandler[highestPacket];
            
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

        public const int BUFFER_MAX = 65535;

        private const int MAX_HAIR_ID = 17;

//        public byte[] readBuffer;
//        public byte[] sideBuffer; // used to store player information packets before auth
//        
//        public int sideBufferBytes;  // totalData for side buffer
//        public int sideBufferMsgLen; // messageLength for side buffer
//
//        public int messageLength;
        public int spamCount;
//        public int totalData;
//        public int whoAmI;

        public void Reset()
        {
//            messageLength = 0;
//            totalData = 0;
            spamCount = 0;
//            sideBuffer = null;
//            sideBufferBytes = 0;
//            sideBufferMsgLen = 0;
        }

		public static void GetData (ClientConnection conn, byte[] readBuffer, int start, int length)
		{
			//var slot = Netplay.slots[whoAmI];
			
			try
			{
//				if (whoAmI < 256)
//				{
//					slot.timeOut = 0;
//				}
	
				int num = start + 1;
				byte bufferData = readBuffer[start];
	
//				if (bufferData != 38)
//				{
//					if (conn.State == SlotState.SERVER_AUTH)
//					{
//						conn.Kick ("Incorrect password.");
//						return;
//					}
//	
//					if (conn.State < SlotState.PLAYING && bufferData > 12 && bufferData != 16 && bufferData != 42 && bufferData != 50 && bufferData != 25)
//					{
//						ProgramLog.Debug.Log ("{0}: sent message {1} in state {2}.", conn.RemoteAddress, (bufferData > 0 && bufferData <= 51) ? (object)(Packet)bufferData : bufferData, conn.State);
//						if ((conn.State & SlotState.DISCONNECTING) == 0)
//							conn.Kick ("Invalid operation at this state.");
//						return;
//					}
//				}
				
				if ((conn.State & SlotState.DISCONNECTING) == 0 && bufferData > 0 && bufferData < messageArray.Length)
				{
					MessageHandler message = messageArray[bufferData];
					if (message != null)
					{
						//ProgramLog.Debug.Log ("{2}, packet {0}, len {1}", (Packet)readBuffer[start], length, conn.State);
						//message.Process(start, length, num, whoAmI, readBuffer, bufferData);
						//message.Process (whoAmI, readBuffer, length, num);
						var state = conn.State;
						
						if ((state & message.IgnoredStates) != 0)
						{
							//ProgramLog.Debug.Log ("ignoring");
						}
						else if ((state & message.ValidStates) != 0)
						{
							message.Process (conn, readBuffer, length, num);
						}
						else
						{
							ProgramLog.Debug.Log ("{0}: sent message {1} in state {2}.", conn.RemoteAddress, (bufferData > 0 && bufferData <= 51) ? (object)(Packet)bufferData : bufferData, conn.State);
							conn.Kick ("Invalid operation in this state.");
						}
					}
				}
			}
			catch (Exception e)
			{
				string pkt = "invalid packet";
				if (readBuffer.Length > start)
					pkt = string.Format ("packet {0}", (Packet)readBuffer[start]);

				ProgramLog.Log (e, string.Format ("Exception handling {0} of length {1} from {2}",
					pkt, length, conn.RemoteAddress));
					
				conn.Kick ("Server malfunction, please reconnect.");
			}
		}
	}
}
