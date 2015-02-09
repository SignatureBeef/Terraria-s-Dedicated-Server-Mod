using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace tdsm.core
{
    class ServerEntry
    {
        private static Main game;
        private static void Main(string[] args)
        {
            //Process currentProcess = Process.GetCurrentProcess();
            //currentProcess.PriorityClass = ProcessPriorityClass.High;
            game = new Main();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "-config")
                {
                    i++;
                    game.LoadDedConfig(args[i]);
                }
                if (args[i].ToLower() == "-port")
                {
                    i++;
                    try
                    {
                        int serverPort = Convert.ToInt32(args[i]);
                        Netplay.serverPort = serverPort;
                    }
                    catch
                    {
                    }
                }
                if (args[i].ToLower() == "-players" || args[i].ToLower() == "-maxplayers")
                {
                    i++;
                    try
                    {
                        int netPlayers = Convert.ToInt32(args[i]);
                        game.SetNetPlayers(netPlayers);
                    }
                    catch
                    {
                    }
                }
                if (args[i].ToLower() == "-pass" || args[i].ToLower() == "-password")
                {
                    i++;
                    Netplay.password = args[i];
                }
                if (args[i].ToLower() == "-lang")
                {
                    i++;
                    Lang.lang = Convert.ToInt32(args[i]);
                }
                if (args[i].ToLower() == "-world")
                {
                    i++;
                    game.SetWorld(args[i]);
                }
                if (args[i].ToLower() == "-worldname")
                {
                    i++;
                    game.SetWorldName(args[i]);
                }
                if (args[i].ToLower() == "-motd")
                {
                    i++;
                    game.NewMOTD(args[i]);
                }
                if (args[i].ToLower() == "-banlist")
                {
                    i++;
                    Netplay.banFile = args[i];
                }
                if (args[i].ToLower() == "-autoshutdown")
                {
                    game.autoShut();
                }
                if (args[i].ToLower() == "-secure")
                {
                    Netplay.spamCheck = true;
                }
                if (args[i].ToLower() == "-autocreate")
                {
                    i++;
                    string newOpt = args[i];
                    game.autoCreate(newOpt);
                }
                if (args[i].ToLower() == "-loadlib")
                {
                    i++;
                    string path = args[i];
                    game.loadLib(path);
                }
                if (args[i].ToLower() == "-noupnp")
                {
                    Netplay.uPNP = false;
                }
            }
            game.DedServ();
        }
    }
}
