//using TDSM.API.Plugin;
//using TDSM.Core.Logging;
//
//namespace TDSM.Core.Events
//{
//    public static class HookPoints
//    {
//        public static readonly HookPoint<HookArgs.ConsoleMessageReceived> ConsoleMessageReceived;
//
//        static HookPoints()
//        {
//            ConsoleMessageReceived = new HookPoint<HookArgs.ConsoleMessageReceived>("console-message-received");
//        }
//    }
//
//    public static class HookArgs
//    {
//        public struct ConsoleMessageReceived
//        {
//            public string Message { get; set; }
//            public SendingLogger Logger { get; set; }
//        }
//    }
//}
//
