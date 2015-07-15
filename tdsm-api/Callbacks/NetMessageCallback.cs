using TDSM.API.Plugin;

#if Full_API
using Terraria;
#endif

namespace TDSM.API.Callbacks
{
    public static class NetMessageCallback
    {
        public static bool SendData(Packet msgType, int remoteClient = -1, int ignoreClient = -1, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0)
        {
            return SendData((int)msgType, remoteClient, ignoreClient, text, number, number2, number3, number4, number5);
        }

        public static bool SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0)
        {
            #if Full_API
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.SendNetData()
            {
                MsgType = msgType,
                RemoteClient = remoteClient,
                IgnoreClient = ignoreClient,
                Text = text,
                Number = number,
                Number2 = number2,
                Number3 = number3,
                Number4 = number4,
                Number5 = number5
            };
            HookPoints.SendNetData.Invoke(ref ctx, ref args);
            return ctx.Result == HookResult.DEFAULT;
            #else
            return false;
            #endif
        }
    }
}
