using System;
using TDSM.API.Misc;
using TDSM.API.Plugin;
using TDSM.API.Sockets;
using TDSM.API.Logging;
using TDSM.API.Command;

namespace TDSM.API.Callbacks
{
    public static class MainCallback
    {
        public static Action StatusTextChange;
        public static Action UpdateServer;

        //public static bool StartEclipse;
        //public static bool StartBloodMoon;

        public static void ProgramStart()
        {
            Console.Clear();
            if (!ProgramLog.IsOpen)
            {
                var logFile = Globals.DataPath + System.IO.Path.DirectorySeparatorChar + "server.log";
                ProgramLog.OpenLogFile(logFile);
                ConsoleSender.DefaultColour = ConsoleColor.Gray;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            ProgramLog.Log("TDSM Rebind API build {0}{1} running on {2}", 
                Globals.Build, 
                Globals.PhaseToSuffix(Globals.BuildPhase),
                Tools.RuntimePlatform.ToString()
            );
            Console.ForegroundColor = Command.ConsoleSender.DefaultColour;

            Globals.Touch();
            ID.Lookup.Initialise();

            try
            {
                var lis = new Logging.LogTraceListener();
                System.Diagnostics.Trace.Listeners.Clear();
                System.Diagnostics.Trace.Listeners.Add(lis);
                System.Diagnostics.Debug.Listeners.Clear();
                System.Diagnostics.Debug.Listeners.Add(lis);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            PluginManager.SetHookSource(typeof(HookPoints));
            PluginManager.Initialize(Globals.PluginPath);
            PluginManager.LoadPlugins();

//            if (!Permissions.PermissionsManager.IsSet)
//            {
//                var file = System.IO.Path.Combine(Globals.DataPath, "permissions.xml");
//                //if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
//                if (System.IO.File.Exists(file))
//                {
//                    var handler = new Permissions.XmlSupplier(file);
//                    if (handler.Load())
//                        Permissions.PermissionsManager.SetHandler(handler);
//                }
//            }
        }

        public static bool OnProgramStarted(string[] cmd)
        {
            System.Threading.Thread.CurrentThread.Name = "Run";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ProgramStart();
#pragma warning disable 0162
            if (!Globals.FullAPIDefined)
            {
                Console.WriteLine("Your TDSM.API.dll is incorrect, and does not expose all methods.");
                return false;
            }
#pragma warning restore 0162

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

        public static void OnProgramFinished()
        {
            PluginManager.DisablePlugins();

            //Close the logging if set
            ProgramLog.Close();
//            if (Tools.WriteClose != null)
//                Tools.WriteClose.Invoke();
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
            ///* Check tolled tasks */
            //Tasks.CheckTasks();

            if (UpdateServer != null)
                UpdateServer();

            #if Full_API
            for (var i = 0; i < Terraria.Netplay.Clients.Length; i++)
            {
                var client = Terraria.Netplay.Clients[i];
//                if (player.active)
                if (client != null && client.Socket != null && client.Socket is ClientConnection)
                {
                    var conn = (client.Socket as ClientConnection);
                    if (conn != null)
                        conn.Flush();
                }
            }
            #endif
            //var ctx = new HookContext()
            //{
            //    Sender = HookContext.ConsoleSender
            //};
            //var args = new HookArgs.UpdateServer();
            //HookPoints.UpdateServer.Invoke(ref ctx, ref args);
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
            try
            {
                /* Check tolled tasks - OnStatusTextChanged is called without clients connected */
                Tasks.CheckTasks(); //This still may not be the best place for this.

#if Full_API
                if (Terraria.Main.oldStatusText != Terraria.Main.statusText)
                {
                    if (StatusTextChange != null)
                        StatusTextChange();
                    else
                    {
                        Terraria.Main.oldStatusText = Terraria.Main.statusText;
                        ProgramLog.Log(Terraria.Main.statusText);
                    }
                    /*var ctx = new HookContext()
                {
                    Sender = HookContext.ConsoleSender
                };
                var args = new HookArgs.StatusTextChanged() { };
                HookPoints.StatusTextChanged.Invoke(ref ctx, ref args);

                if (ctx.Result == HookResult.DEFAULT)
                {
                    Terraria.Main.oldStatusText = Terraria.Main.statusText;
                    Tools.WriteLine(Terraria.Main.statusText);
                }*/
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
            catch (Exception e)
            {
                ProgramLog.Log(e, "OnStatusTextChange error");
            }
        }

        public static bool OnInvasionWarning()
        {
            var ctx = new HookContext();
            var args = new HookArgs.InvasionWarning();
            
            HookPoints.InvasionWarning.Invoke(ref ctx, ref args);
            return ctx.Result == HookResult.DEFAULT; //Continue on
        }
    }
}

