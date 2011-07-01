using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Terraria_Server.Messages
{
    public class MessageBuffer
    {
        private static IMessage[] messageArray = GetMessageArray();

        private static IMessage[] GetMessageArray()
        {
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

        public bool broadcast;
        public bool checkBytes;

        public byte[] readBuffer = new byte[BUFFER_MAX];
        public byte[] writeBuffer = new byte[BUFFER_MAX];

        public int messageLength;
        public int spamCount;
        public int totalData;
        public int whoAmI;

        public void Reset()
        {
            readBuffer = new byte[BUFFER_MAX];
            writeBuffer = new byte[BUFFER_MAX];
            messageLength = 0;
            totalData = 0;
            spamCount = 0;
        }

        public void GetData(int start, int length)
        {
            if (whoAmI < 256)
            {
                Netplay.serverSock[whoAmI].timeOut = 0;
            }
            else
            {
                Netplay.clientSock.timeOut = 0;
            }

            int num = start + 1;
            byte bufferData = readBuffer[start];

            if (Main.netMode == 1 && Netplay.clientSock.statusMax > 0)
            {
                Netplay.clientSock.statusCount++;
            }

            if (Main.netMode == 2)
            {
                if (bufferData != 38)
                {
                    if (Netplay.serverSock[whoAmI].state == -1)
                    {
                        NetMessage.SendData(2, whoAmI, -1, "Incorrect password.");
                        return;
                    }

                    if (Netplay.serverSock[whoAmI].state < 10 && bufferData > 12 && bufferData != 16 && bufferData != 42 && bufferData != 50)
                    {
                        NetMessage.BootPlayer(whoAmI, "Invalid operation at this state.");
                    }
                }
            }

            IMessage message = messageArray[bufferData];
            if (message != null && (message.GetRequiredNetMode() == null || message.GetRequiredNetMode() == Main.netMode))
            {
                message.Process(start, length, num, whoAmI, readBuffer, bufferData);
            }
        }
    }
}