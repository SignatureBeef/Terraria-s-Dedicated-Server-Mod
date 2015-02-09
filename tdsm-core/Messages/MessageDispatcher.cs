using System;
using System.Linq;
using tdsm.core.Logging;
using tdsm.core.ServerCore;

namespace tdsm.core.Messages
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
            var maxValue = Enum.GetValues(typeof(Packet)).Cast<Int32>()
                .OrderByDescending(x => x)
                .First();

            MessageHandler[] tempArray = new MessageHandler[maxValue];

            //Load all the Events found in the current assembly into the the message array.
            Type type = typeof(MessageHandler);
            foreach (Type messageType in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(clazz => clazz.GetTypes()).Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract))
            {
                MessageHandler message = (MessageHandler)Activator.CreateInstance(messageType);
                tempArray[(int)message.GetPacket()] = message;
            }

            return tempArray;
        }

        public static void Dispatch(ClientConnection conn, byte[] readBuffer, int start, int length)
        {
            try
            {
                int num = start + 1;
                byte pkt = readBuffer[start];

                if (conn.State == SlotState.SERVER_AUTH && pkt != 38)
                {
                    conn.Kick("Incorrect password.");
                    return;
                }

                if ((conn.State & SlotState.DISCONNECTING) == 0)
                {
                    var handler = messageArray[pkt];
                    var state = conn.State;

                    if (handler != null)
                    {
                        if ((state & handler.IgnoredStates) != 0)
                        {
                            //ProgramLog.Log("ignoring");
                        }
                        else if ((state & handler.ValidStates) != 0)
                        {
                            //These guys are per thread, so we must ensure they are set or we will end up with strange issues (trust me...)
                            if (Terraria.WorldGen.genRand == null)
                                Terraria.WorldGen.genRand = new Random((new Random()).Next(Int32.MinValue, Int32.MaxValue));
                            if (Terraria.Main.rand == null)
                                Terraria.Main.rand = new Random((new Random()).Next(Int32.MinValue, Int32.MaxValue));

                            handler.Reset(num);
                            handler.Process(conn, readBuffer, length, num);
                        }
                        else
                        {
                            ProgramLog.Log("{0}: sent message {1} in state {2}.", conn.RemoteAddress, (pkt > 0 && pkt <= 76) ? (object)(Packet)pkt : pkt, conn.State);
                            conn.Kick("Invalid operation in this state.");
                        }
                    }
                    else
                    {
                        conn.Kick(String.Format("Message not understood ({0}).", pkt));
                    }
                }
            }
            catch (Exception e)
            {
                string pkt = "invalid packet";
                if (readBuffer.Length > start)
                    pkt = String.Format("packet {0}", (Packet)readBuffer[start]);

                ProgramLog.Log("Exception handling {0} of length {1} from {2}\n{3}",
                    pkt, length, conn.RemoteAddress, e);

                conn.Kick("Server malfunction, please reconnect.");
            }
        }
    }
}
