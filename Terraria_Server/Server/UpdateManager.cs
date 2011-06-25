using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Terraria_Server
{
    public class UpdateManager
    {
        public static string UpdateList = "http://update.tdsm.org/updatelist.txt";
        public static string UpdateLink = "http://kangaroo.olympus-gaming.net/Terraria_Server.exe"; // <3 Olympus Gaming! Check em out some time ;)

        public static string getUpdateList()
        {
            return new System.Net.WebClient().DownloadString(UpdateList).Trim();
        }

        private static string uList = "";
        public static bool isUptoDate()
        {
            string updateList = getUpdateList();
            //b-r
            if (updateList.Contains("b")) // && updateList.Contains("-") && updateList.Contains("r"))
            {
                string myBuild = "b" + Statics.build.ToString(); //+ "-r" + Statics.revision.ToString();
                uList = updateList;
                return updateList.Equals(myBuild);
            }
            return false;
        }

        public static bool performProcess()
        {
            if (!Program.properties.automaticUpdates())
            {
                return false;
            }
            Program.tConsole.Write("Checking for Updates...");
            if (!isUptoDate())
            {
                Program.tConsole.WriteLine("Update found, Performing b" + Statics.build.ToString() + " -> " + uList);
                
                string savePath = "Terraria_Server.upd";
                string backupPath = "Terraria_Server.bak";
                string myFile = System.AppDomain.CurrentDomain.FriendlyName;

                if (System.IO.File.Exists(savePath))
                {
                    try
                    {
                        System.IO.File.Delete(savePath);
                    }
                    catch (Exception e)
                    {
                        Program.tConsole.WriteLine("Error deleting old download!");
                        Program.tConsole.WriteLine(e.Message);
                        return false;
                    }
                }
                if (System.IO.File.Exists(backupPath))
                {
                    try
                    {
                        System.IO.File.Delete(backupPath);
                    }
                    catch (Exception e)
                    {
                        Program.tConsole.WriteLine("Error deleting old backup!");
                        Program.tConsole.WriteLine(e.Message);
                        return false;
                    }
                }
                try
                {
                    System.IO.File.Move(myFile, backupPath);
                }
                catch (Exception e)
                {
                    Program.tConsole.WriteLine("Error moving current executable!");
                    Program.tConsole.WriteLine(e.Message);
                    return false;
                }

                Program.tConsole.Write("Downloading Update from Servers...");
                new System.Net.WebClient().DownloadFile(UpdateLink, savePath);
                Program.tConsole.Write("Ok");


                Program.tConsole.Write("Finishing Update...");
                try
                {
                    System.IO.File.Move(savePath, myFile);
                }
                catch (Exception e)
                {
                    Program.tConsole.WriteLine("Error moving updated executable!");
                    Program.tConsole.WriteLine(e.Message);
                    return false;
                }

                Process.Start(myFile);

                return true;
            }
            else
            {
                Program.tConsole.WriteLine("TDSM Upto Date.");
            }
            return false;
        }

    }
}
