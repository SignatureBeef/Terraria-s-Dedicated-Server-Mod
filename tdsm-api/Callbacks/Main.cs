using System;
using tdsm.api.Plugin;

namespace tdsm.api.Callbacks
{
    public static class Main
    {
        public static void ProgramStart()
        {
            Tools.WriteLine("TDSM Rebind API build {0}", Globals.Build);
            Globals.Touch();

            PluginManager.SetHookSource(typeof(HookPoints));
            PluginManager.Initialize(Globals.PluginPath, Globals.LibrariesPath);
            PluginManager.LoadPlugins();
        }

        public static bool OnProgramStarted(string[] cmd)
        {
            System.Threading.Thread.CurrentThread.Name = "Run";
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ProgramStart();

            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ProgramStart()
            {
                Arguments = cmd
            };
            HookPoints.ProgramStart.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void Initialise()
        {
#if Full_API
            if (Terraria.Main.dedServ)
            {
                var ctx = new HookContext()
                {
                    Sender = HookContext.ConsoleSender
                };
                var args = new HookArgs.ServerStateChange()
                {
                    ServerChangeState = ServerState.Initialising
                };
                HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
            }
#endif
        }

        public static void UpdateServerEnd()
        {
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.UpdateServer();
            HookPoints.UpdateServer.Invoke(ref ctx, ref args);
        }

        public static void WorldLoadBegin()
        {
            //Since this is hook is at the end of the world loading then we can clear the new progress loading
#if Full_API
            Terraria.Main.statusText = String.Empty;
#endif

            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = ServerState.WorldLoading
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
        }

        public static void WorldLoadEnd()
        {
            //Since this is hook is at the end of the world loading then we can clear the new progress loading
#if Full_API
            Terraria.Main.statusText = String.Empty;
#endif

            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = ServerState.WorldLoaded
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
        }

        public static void WorldGenerateBegin()
        {
            //Since this is hook is at the end of the world loading then we can clear the new progress loading
#if Full_API
            Terraria.Main.statusText = String.Empty;
#endif

            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = ServerState.WorldGenerating
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
        }

        public static void WorldGenerateEnd()
        {
            //Since this is hook is at the end of the world loading then we can clear the new progress loading
#if Full_API
            Terraria.Main.statusText = String.Empty;
#endif

            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.ServerStateChange()
            {
                ServerChangeState = ServerState.WorldGenerated
            };
            HookPoints.ServerStateChange.Invoke(ref ctx, ref args);
        }

        //private static int _textTimeout = 0;
        public static void OnStatusTextChange()
        {
#if Full_API
            if (Terraria.Main.oldStatusText != Terraria.Main.statusText)
            {
                var ctx = new HookContext()
                {
                    Sender = HookContext.ConsoleSender
                };
                var args = new HookArgs.StatusTextChanged() { };
                HookPoints.StatusTextChanged.Invoke(ref ctx, ref args);

                if (ctx.Result != HookResult.IGNORE)
                {
                    Terraria.Main.oldStatusText = Terraria.Main.statusText;
                    Tools.WriteLine(Terraria.Main.statusText);
                }
                //_textTimeout = 0;
            }
            //else if (Terraria.Main.oldStatusText == String.Empty && Terraria.Main.statusText == String.Empty)
            //{
            //    if (_textTimeout++ > 1000)
            //    {
            //        _textTimeout = 0;
            //        Terraria.Main.statusText = String.Empty;
            //    }
            //}
#endif
        }
    }
}

