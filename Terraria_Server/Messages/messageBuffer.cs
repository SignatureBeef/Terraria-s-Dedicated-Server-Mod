using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Terraria_Server.Messages
{
    public class MessageBuffer
    {
        private static IMessage[] messageArray = GetMessageArray();

        /// <summary>
    	/// Load all IMessage types into an indexed array at application start.
    	/// This should allow us to process events extremely quickly while cutting down
    	/// on how much code we have to hold in our head to understand each event.
    	/// </summary>
        private static IMessage[] GetMessageArray()
        {
        	//Find the highest Packet value and make an array of that size to process events.
            int highestPacket = 0;
            foreach (Packet packet in Enum.GetValues(typeof(Packet)))
            {
                int packetValue = (int)packet;
                if (packetValue > highestPacket)
                {
                    highestPacket = packetValue + 1;
                }
            }
            IMessage[] tempArray = new IMessage[highestPacket];
            
            //Load all the events found in the current assembly into the the message array.
            Type type = typeof(IMessage);
            foreach(Type messageType in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(clazz => clazz.GetTypes()).Where(x => type.IsAssignableFrom(x) && x != type))
            {
                IMessage message = (IMessage)Activator.CreateInstance(messageType);
                tempArray[(int)message.GetPacket()] = message;
            }

            return tempArray;
        }

        public const int BUFFER_MAX = 65535;

        private const int MAX_HAIR_ID = 17;

        public byte[] readBuffer;
        public byte[] sideBuffer; // used to store player information packets before auth
        
        public int sideBufferBytes;  // totalData for side buffer
        public int sideBufferMsgLen; // messageLength for side buffer

        public int messageLength;
        public int spamCount;
        public int totalData;
        public int whoAmI;

        public void Reset()
        {
            messageLength = 0;
            totalData = 0;
            spamCount = 0;
            sideBuffer = null;
            sideBufferBytes = 0;
            sideBufferMsgLen = 0;
        }

		public void ResetSideBuffer ()
		{
			sideBuffer = null;
			sideBufferBytes = 0;
			sideBufferMsgLen = 0;
		}
		
		public void GetData (byte[] readBuffer, int start, int length)
		{
			try
			{
				if (whoAmI < 256)
				{
					Netplay.slots[whoAmI].timeOut = 0;
				}
	
				int num = start + 1;
				byte bufferData = readBuffer[start];
	
				if (bufferData != 38)
				{
					if (Netplay.slots[whoAmI].state == SlotState.SERVER_AUTH)
					{
						Netplay.slots[whoAmI].Kick ("Incorrect password.");
						return;
					}
	
					if (Netplay.slots[whoAmI].state < SlotState.PLAYING && bufferData > 12 && bufferData != 16 && bufferData != 42 && bufferData != 50)
					{
						NetMessage.BootPlayer(whoAmI, "Invalid operation at this state.");
					}
				}
	
				if (bufferData > 0 && bufferData < messageArray.Length)
				{
					IMessage message = messageArray[bufferData];
					if (message != null)
					{
						//Console.WriteLine ("packet {0}, len {1}", (Packet)readBuffer[start], length);
						message.Process(start, length, num, whoAmI, readBuffer, bufferData);
					}
				}
			}
			catch (Exception e)
			{
				string pkt = "invalid packet";
				if (NetMessage.buffer[whoAmI].readBuffer.Length > start)
					pkt = string.Format ("packet {0}", (Packet)NetMessage.buffer[whoAmI].readBuffer[start]);

				Program.tConsole.WriteLine ("Exception handling {0} of length {1} from {2}@{3}",
					pkt, length, Main.players[whoAmI].Name ?? "", Netplay.slots[whoAmI].remoteAddress);
				Program.tConsole.WriteLine (e.ToString());
					
				Netplay.slots[whoAmI].Kick ("Server malfunction, please reconnect.");
			}
		}
	}
}
