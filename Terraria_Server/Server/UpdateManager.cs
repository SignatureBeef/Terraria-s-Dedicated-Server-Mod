
using System;
using System.Diagnostics;
using System.IO;
namespace Terraria_Server
{
    public class UpdateManager
    {
        public static String UpdateList = "http://update.tdsm.org/updatelist.txt";
        public static String UpdateLink = "http://update.tdsm.org/Terraria_Server.exe"; //Still hosted by Olympus, <3 Olympus Gaming! Check em out some time ;)
        public static String UpdateInfo = "http://update.tdsm.org/buildinfo.txt";

        public static void printUpdateInfo()
        {
            try
            {
                Program.tConsole.WriteLine("Attempting to retreive Build Info...");
                String buildInfo = new System.Net.WebClient().DownloadString(UpdateInfo).Trim();
                String toString = "comments: ";
                if (buildInfo.ToLower().Contains(toString))
                {
                    buildInfo = buildInfo.Remove(0, buildInfo.ToLower().IndexOf(toString.ToLower()) + toString.Length).Trim().Replace("<br/>", "\n"); //This is also used for the forums, so easy use here ;D
                    if (buildInfo.Length > 0)
                    {
                        Program.tConsole.WriteLine("Build Comments: " + buildInfo);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public static String getUpdateList()
        {
            return new System.Net.WebClient().DownloadString(UpdateList).Trim();
        }

        private static String uList = "";
        public static bool isUptoDate()
        {
            String updateList = getUpdateList();
            //b-r
            if (updateList.Contains("b"))
            {
                String myBuild = "b" + Statics.BUILD.ToString();
                uList = updateList;
                return updateList.Equals(myBuild);
            }
            return false;
        }

        public static bool performProcess()
        {
            if (!Program.properties.AutomaticUpdates)
            {
                return false;
            }
            Program.tConsole.Write("Checking for Updates...");
            if (!isUptoDate())
            {
                Program.tConsole.WriteLine("Update found, Performing b" + Statics.BUILD.ToString() + " -> " + uList);

                printUpdateInfo();

                String savePath = "Terraria_Server.upd";
                String backupPath = "Terraria_Server.bak";
                String myFile = System.AppDomain.CurrentDomain.FriendlyName;

                if (File.Exists(savePath)) //No download conflict, Please :3 (Looks at Mono)
                {
                    try
                    {
                        File.Delete(savePath);
                    }
                    catch (Exception e)
                    {
                        Program.tConsole.WriteLine("Error deleting old download!");
                        Program.tConsole.WriteLine(e.Message);
                        return false;
                    }
                }

                if(!MoveFile(myFile, backupPath)) {
                    Program.tConsole.WriteLine("Error moving current executable!");
                    return false;
                }

                Program.tConsole.Write("Downloading Update from Servers...");
                new System.Net.WebClient().DownloadFile(UpdateLink, savePath);
                Program.tConsole.Write("Ok");

                Program.tConsole.Write("Finishing Update...");

                if (!MoveFile(savePath, myFile))
                {
                    Program.tConsole.WriteLine("Error moving updated executable!");
                    return false;
                }

                try
                {
                    Process.Start(myFile);
                }
                catch (Exception e)
                {
                    Program.tConsole.WriteLine("Could not boot into the new Update!");
                    Program.tConsole.WriteLine(e.Message);
                    return false;
                }

                return true;
            }
            else
            {
                Program.tConsole.WriteLine("TDSM Upto Date.");
            }
            return false;
        }

        //Seems Mono had an issue when files were overwriting.
        public static bool MoveFile(String Location, String Destination)
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
