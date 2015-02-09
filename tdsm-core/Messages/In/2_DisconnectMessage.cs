using tdsm.api.Plugin;
using tdsm.core.ServerCore;

namespace tdsm.core.Messages.In
{
    public class DisconnectMessage : MessageHandler
    {
        public DisconnectMessage()
        {
            ValidStates = SlotState.CONNECTED | SlotState.ACCEPTED | SlotState.SERVER_AUTH;
        }

        public override Packet GetPacket()
        {
            return Packet.DISCONNECT;
        }

        public override void Process(ClientConnection conn, byte[] readBuffer, int length, int num)
        {
            //var data = Encoding.ASCII.GetString (readBuffer, num, length - 1);
            //var lines = data.Split ('\n');

            //foreach (var line in lines)
            //{
            //    if (line == "tdcm1")
            //    {
            //        //player.HasClientMod = true;
            //        ProgramLog.Log ("{0} is a TDCM protocol version 1 client.", conn.RemoteAddress);
            //    }
            //    else if (line == "tdsmcomp1")
            //    {
            //        conn.CompressionVersion = 1;
            //        ProgramLog.Log ("{0} supports TDSM compression version 1.", conn.RemoteAddress);
            //    }
            //}

            ReadString(readBuffer);

            var ctx = new HookContext
            {
                Connection = conn,
            };

            var args = new HookArgs.DisconnectReceived
            {
            };

            HookPoints.DisconnectReceived.Invoke(ref ctx, ref args);

            ctx.CheckForKick();
        }
    }
}

