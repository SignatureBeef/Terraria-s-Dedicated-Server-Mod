using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using Terraria_Server.Plugin;

namespace Terraria_Server
{
    public class Server : Main
    {
        private PluginManager pluginManager = null;
        
        private World world = null;

        private DataRegister whiteList = null;
        private DataRegister banList = null;

        public Server() { }

        public Server(World World, int PlayerCap, string WhiteList, string BanList)
        {
            Main.maxNetplayers = PlayerCap;
            world = World;
            world.setServer(this);
            pluginManager = new PluginManager(Statics.getPluginPath, this);
            whiteList = new DataRegister(WhiteList);
            whiteList.Load();
            //joinedPlayerList = new DataRegister(JoinedPlayers);
            //joinedPlayerList.Load();
            banList = new DataRegister(BanList);
            banList.Load();
        }

        public Player GetPlayerByName(string name)
        {
            for (int i = 0; i < Main.player.Length; i++)
            {
                if (Main.player[i].name.ToLower() == name.ToLower())
                {
                    return Main.player[i];
                }
            }
            return null;
        }

        public PluginManager getPluginManager()
        {
            return pluginManager;
        }
        
        public World getWorld()
        {
            return world;
        }

        public void setOpPassword(String Password)
        {
            Netplay.password = Password;
        }

        public void setPort(int Port)
        {
            Netplay.serverPort = Port;
        }

        public void setIP(String IPAddress)
        {
            Netplay.serverSIP = IPAddress;
        }

        public void StopServer()
        {
            Netplay.StopServer();
        }

        public void StartServer()
        {
            Netplay.StartServer();
        }
        
        public void notifyOps(string Message, bool writeToConsole = false)
        {
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active)
                {
                    if (Main.player[i].isOp())
                    {
                        NetMessage.SendData((int)Packet.PLAYER_CHAT, Main.player[i].whoAmi, -1, Message, 255, 176f, 196, 222f);
                    }
                }
            }
            Console.WriteLine(Message);
        }

        public void notifyAll(string Message)
        {
            NetMessage.SendData((int)Packet.PLAYER_CHAT, -1, -1, Message, 255, 238f, 130f, 238f);
        }

        public bool getGodMode()
        {
            return Main.godMode;
        }

        public void setGodMode(bool Status)
        {
            Main.godMode = Status;
        }

        public DataRegister getWhiteList()
        {
            return whiteList;
        }

        public Player[] getPlayerList()
        {
            return Main.player;
        }

        public void setWhiteList(DataRegister WhiteList)
        {
            whiteList = WhiteList;
        }

        /*public DataRegister getJoinedPlayerList()
        {
            return joinedPlayerList;
        }

        public void setJoinedPlayerList(DataRegister JoinedPlayerList)
        {
            joinedPlayerList = JoinedPlayerList;
        }*/

        public DataRegister getBanList()
        {
            return banList;
        }

        public void setBanList(DataRegister BanList)
        {
            banList = BanList;
        }

    }
}
