//using System;
//using OTA.Command;
//using Terraria;
//
//namespace TDSM.Core.Command.Commands
//{
//    public class RestartCommand : CoreCommand
//    {
//        public override void Initialise()
//        {
//            //            this.AddCommand("restart")
//            //                .WithAccessLevel(AccessLevel.OP)
//            //                .WithDescription("Restart the server.")
//            //                .WithHelpText("<no parameters>    - Restart immediately.")
//            //                .WithHelpText("--wait             - Wait for users to disconnect and then restart.")
//            //                .WithPermissionNode("tdsm.restart")
//            //                .Calls(this.Restart);
//        }
//
//        GameTask _tskWaitForPlayers;
//        private bool? _waitFPState;
//
//        /// <summary>
//        /// Restart and reload the world without reloading the application
//        /// </summary>
//        /// <param name="sender">Sending player</param>
//        /// <param name="args">Arguments sent with command</param>
//        public void Restart(ISender sender, ArgumentList args)
//        {
//#if TDSMServer
//            string cmd = null;
//            args.TryGetString(0, out cmd);
//
//            if (String.IsNullOrEmpty(cmd))
//                PerformRestart();
//            else if (cmd == "-wait")
//            {
//                RestartWhenNoPlayers = !RestartWhenNoPlayers;
//
//                if (_waitFPState == null) _waitFPState = Server.AcceptNewConnections;
//
//                if (RestartWhenNoPlayers)
//                {
//                    Server.AcceptNewConnections = false;
//                    if (ClientConnection.All.Count == 0)
//                    {
//                        PerformRestart();
//                        return;
//                    }
//
//                    if (_tskWaitForPlayers == null)
//                    {
//                        _tskWaitForPlayers = new Task()
//                        {
//                            Enabled = true,
//                            Method = (tsk) =>
//                            {
//                                Tools.NotifyAllPlayers("The server is waiting to restart.", Color.Orange, false);
//                                Tools.NotifyAllPlayers("Please finish what you are doing and disconnect.", Color.Orange, false);
//
//                                var players = from p in Terraria.Main.player where p.active orderby p.name select p.Name;
//
//                                var pn = players.Count();
//                                if (pn == 0) return;
//
//                                ProgramLog.Admin.Log("Notified player(s) of restart: " + String.Join(", ", players));
//                            },
//                            Trigger = 60
//                        };
//                        Tasks.Schedule(_tskWaitForPlayers);
//                    }
//                    else
//                    {
//                        _tskWaitForPlayers.Enabled = true;
//                    }
//                }
//                else
//                {
//                    Server.AcceptNewConnections = _waitFPState.Value; //Restore
//                    if (_tskWaitForPlayers != null && _tskWaitForPlayers.Enabled)
//                    {
//                        if (_tskWaitForPlayers.HasTriggered)
//                        {
//                            Tools.NotifyAllPlayers("Restart was terminated.", Color.Orange);
//                        }
//                        _tskWaitForPlayers.Enabled = false;
//                    }
//                }
//
//                sender.Message("The server is " + (_tskWaitForPlayers != null && _tskWaitForPlayers.Enabled ? "waiting to restart" : "not restarting"));
//            }
//            else throw new CommandError("No restart command: " + cmd);
//#endif
//        }
//    }
//}
//
