using System;
using System.Diagnostics;
using System.IO;
using Terraria_Server.Definitions;
using Terraria_Server.Logging;
using Terraria_Server.Misc;
using System.Net;
using System.Collections.Generic;

namespace Terraria_Server
{
    public class UpdateCompleted : ApplicationException
    {
    }
    
    public static class UpdateManager
    {
        public static string UpdateList = "http://update.tdsm.org/updatelist.txt";
        public static string UpdateLink = "http://update.tdsm.org/Terraria_Server.exe";
        public static string UpdateInfo = "http://update.tdsm.org/buildinfo.txt";
        public static string UpdateMDBLink = "http://update.tdsm.org/Terraria_Server.exe.mdb";

        public const Int32 MaxUpdates = 2;
        public static readonly Dictionary<String, String> BuildInfoReplacements = new Dictionary<String, String>()
        {
            { "<i>", String.Empty },
            { "</i>", String.Empty },
            { "<br>", "\n" },
            { "<br/>", "\n" },
            { "-", String.Empty },
            { "\r", String.Empty }
        };
        
//        private static string uList = "";
        
        static UpdateManager()
        {
            if(!Program.properties.UpdateNotice) return;
            var task = new Task()
            {
                Method = CheckForUpdates,
                Trigger = 30 * 60 //30 minutes                
            };
            Tasks.Schedule (task);
        }
        
        static bool IsChecking;
        static void CheckForUpdates()
        {
            try 
            {
                if(IsChecking) return;
                IsChecking = true;
                
                int build;
                if(!TrySeeIfIsUpToDate (out build))
                    ProgramLog.Admin.Log("A TDSM update is available: b{0}", build);
                
                IsChecking = false;
            }
            catch { }
        }

        public static void PrintUpdateInfo()
        {
            try
            {
//                ProgramLog.Log ("Attempting to retreive Build Info...");
                
                var buildInfo = String.Empty;
                using(var ctx = new WebClient())
                {
                    using (var prog = new ProgressLogger(100, "Downloading build information..."))
                    {
                        var signal = new System.Threading.AutoResetEvent (false);
                        
                        ctx.DownloadProgressChanged += (sender, args) =>
                        {
                            prog.Value = args.ProgressPercentage;
                        };
                        
                        ctx.DownloadStringCompleted += (sender, args) =>
                        {
                            var arg = args as DownloadStringCompletedEventArgs;
                            buildInfo = arg.Result;
                            
                            signal.Set ();
                        };
                        
                        ctx.DownloadStringAsync (new Uri(UpdateInfo));
                        
                        signal.WaitOne ();
                    }
                }
                
                if(String.IsNullOrEmpty (buildInfo))
                {
                    ProgramLog.Log ("Failed to download build information.");
                    return;
                }
                
                var toString = "comments: ";
                //if (buildInfo.ToLower().Contains(toString))
                var index = buildInfo.ToLower ().IndexOf (toString);
                if(index != -1)
                {
                    buildInfo = buildInfo.Remove (0, index + toString.Length).Trim ();                    
                    foreach(var pair in BuildInfoReplacements)
                        buildInfo = buildInfo.Replace (pair.Key, pair.Value);
                    
                    ProgramLog.Log ("Build Comments: \n\t " + buildInfo);
                }
            }
            catch (Exception) { }
        }

        public static string GetUpdateList()
        {
            using(var ctx = new WebClient())
                return ctx.DownloadString(UpdateList).Trim();
        }
  
        public static bool IsUpToDate()
        {
            int build;
            return TrySeeIfIsUpToDate(out build);
        }
        
        private static bool TrySeeIfIsUpToDate(out int build)
        {
            build = -1;
            var updateList = GetUpdateList();
            //b<number>
            if (updateList.StartsWith("b"))
			{
				try
				{
//                    string updateBuild = String.Empty;
                    
//                    for (int i = 1; i < updateList.Length; i++)
//                        updateBuild += updateList[i];
                    var updateBuild = updateList.Remove (0, 1);
                    
                    if(Int32.TryParse(updateBuild, out build))
                    {
//                        string myBuild = "b" + Statics.BUILD.ToString();
//                        uList = updateList;
    					return Statics.BUILD >= build;
                    }
				}
				catch { }
            }
            return false;
        }

        public static bool PerformUpdate(string DownloadLink, string savePath, string backupPath, string myFile, int Update, int MaxUpdates, string header = "update ")
        {
            if (File.Exists(savePath))
            {
                try
                {
                    File.Delete(savePath);
                }
                catch (Exception e)
                {
                    ProgramLog.Log (e, "Error deleting old file");
                    return false;
                }
            }

            if (File.Exists(myFile) && !MoveFile(myFile, backupPath))
            {
                ProgramLog.Log ("Error moving current file!");
                return false;
            }

            var download = new System.Net.WebClient();
            Exception error = null;

            string downloadText = "";
            if (MaxUpdates > 1)
                downloadText = "Downloading " + header + Update.ToString() + "/" + MaxUpdates.ToString() + " from server";
            else
                downloadText = "Downloading " + header + "from server";

            using (var prog = new ProgressLogger(100, downloadText))
            {
                var signal = new System.Threading.AutoResetEvent (false);
                
                download.DownloadProgressChanged += (sender, args) =>
                {
                    prog.Value = args.ProgressPercentage;
                };
                
                download.DownloadFileCompleted += (sender, args) =>
                {
                    error = args.Error;
                    signal.Set ();
                };
                
                download.DownloadFileAsync(new Uri (DownloadLink), savePath);
                
                signal.WaitOne ();
            }
            
            if (error != null)
            {
                ProgramLog.Log (error, "Error downloading update");
                return false;
            }

            //Program.tConsole.Write("Finishing Update...");

            if (!MoveFile(savePath, myFile))
            {
                ProgramLog.Log ("Error moving updated file!");
                return false;
            }

            return true;
        }

        public static bool PerformProcess()
        {
            if (!Program.properties.AutomaticUpdates)
            {
                return false;
            }
            ProgramLog.Log ("Checking for updates...");
            
            int build;
            if (!TrySeeIfIsUpToDate(out build))
            {
                ProgramLog.Log ("Update found, performing b{0} -> b{1}", Statics.BUILD, build);

                PrintUpdateInfo();

                string myFile = System.AppDomain.CurrentDomain.FriendlyName;

                PerformUpdate(UpdateLink, "Terraria_Server.upd", "Terraria_Server.bak", myFile, 1, MaxUpdates);
                PerformUpdate(UpdateMDBLink, "Terraria_Server.upd.mdb", "Terraria_Server.bak.mdb", myFile + ".mdb", 2, MaxUpdates);

                Platform.PlatformType oldPlatform = Platform.Type; //Preserve old data if command args were used
                Platform.InitPlatform(); //Reset Data of Platform for determinine exit/enter method.

                if (Platform.Type == Platform.PlatformType.WINDOWS)
                {
                    try
                    {
                        Process.Start(myFile);
                    }
                    catch (Exception e)
                    {
                        Platform.Type = oldPlatform;
                        ProgramLog.Log (e, "Could not boot into the new Update!");
                        return false;
                    }
                }
                else
                {
                    Platform.Type = oldPlatform;
                    ProgramLog.Log ("Exiting, please re-run the program to use your new installation.");
                    throw new UpdateCompleted ();
                }

                return true;
            }
            else
            {
                ProgramLog.Log ("TDSM is up to date.");
            }
            return false;
        }

        //Seems Mono had an issue when files were overwriting.
        public static bool MoveFile(string Location, string Destination)
        {
            if (File.Exists(Destination))
            {
                try
                {
                    File.Delete(Destination);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            try
            {
                File.Move(Location, Destination);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
