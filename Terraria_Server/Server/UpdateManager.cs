using System;
using System.Diagnostics;
using System.IO;
using Terraria_Server.Definitions;
using Terraria_Server.Logging;
using Terraria_Server.Misc;

namespace Terraria_Server
{
    public class UpdateCompleted : ApplicationException
    {
    }
    
    public class UpdateManager
    {
        public static string UpdateList = "http://update.tdsm.org/updatelist.txt";
        public static string UpdateLink = "http://update.tdsm.org/Terraria_Server.exe";
        public static string UpdateInfo = "http://update.tdsm.org/buildinfo.txt";
        public static string UpdateMDBLink = "http://update.tdsm.org/Terraria_Server.exe.mdb";

        public static int MAX_UPDATES = 2;

        public static void printUpdateInfo()
        {
            try
            {
                ProgramLog.Log ("Attempting to retreive Build Info...");
                string buildInfo = new System.Net.WebClient().DownloadString(UpdateInfo).Trim();
                string toString = "comments: ";
                if (buildInfo.ToLower().Contains(toString))
                {
                    buildInfo = buildInfo.Remove(0, buildInfo.ToLower().IndexOf(toString.ToLower()) + toString.Length).Trim().Replace("<br/>", "\n"); //This is also used for the forums, so easy use here ;D
                    if (buildInfo.Length > 0)
                    {
                        ProgramLog.Log ("Build Comments: " + buildInfo);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public static string getUpdateList()
        {
            return new System.Net.WebClient().DownloadString(UpdateList).Trim();
        }

        private static string uList = "";
        public static bool isUptoDate()
        {
            string updateList = getUpdateList();
            //b-r
            if (updateList.Contains("b"))
			{
				try
				{
                    string updateBuild = "";
					for (int i = 1; i < updateList.Length; i++)
					{
						updateBuild += updateList[i];
					}
					int updateBuildNum = Int32.Parse(updateBuild);
                    string myBuild = "b" + Statics.BUILD.ToString();
					uList = updateList;
					return Statics.BUILD >= updateBuildNum;
				}
				catch
				{
				}
            }
            return false;
        }

        public static bool performUpdate(string DownloadLink, string savePath, string backupPath, string myFile, int Update, int MaxUpdates, string header = "update ")
        {
            if (File.Exists(savePath)) //No download conflict, Please :3 (Looks at Mono)
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

        public static bool performProcess()
        {
            if (!Program.properties.AutomaticUpdates)
            {
                return false;
            }
            ProgramLog.Log ("Checking for updates...");
            if (!isUptoDate())
            {
                ProgramLog.Log ("Update found, performing b{0} -> {1}", Statics.BUILD, uList);

                printUpdateInfo();

                string myFile = System.AppDomain.CurrentDomain.FriendlyName;

                performUpdate(UpdateLink, "Terraria_Server.upd", "Terraria_Server.bak", myFile, 1, MAX_UPDATES);
                performUpdate(UpdateMDBLink, "Terraria_Server.upd.mdb", "Terraria_Server.bak.mdb", myFile + ".mdb", 2, MAX_UPDATES);

                Platform.PlatformType oldPlatform = Platform.Type; //Preserve old data if command args were used
                Platform.InitPlatform(); //Reset Data of Platform for determinine exit/enter method.

                if (Platform.Type == Platform.PlatformType.WINDOWS)
                {
                    try
                    {
                        Process.Start(myFile); //Windows only?
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
                ProgramLog.Log ("TDSM Upto Date.");
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
