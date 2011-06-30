
using Terraria_Server.Events;
using System.Text;
using Terraria_Server.Plugin;
using Terraria_Server.Commands;
using System;
using Terraria_Server.Shops;
using Terraria_Server.Misc;

namespace Terraria_Server.Messages
{
    public class MessageBuffer
    {
        public const int BUFFER_MAX = 65535;

        private const int MAX_HAIR_ID = 17;

        public bool broadcast;
        public bool checkBytes;

        public byte[] readBuffer;
        public byte[] writeBuffer;

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

            //Need to add code to run the appropriate message handler.
        }
    }
}