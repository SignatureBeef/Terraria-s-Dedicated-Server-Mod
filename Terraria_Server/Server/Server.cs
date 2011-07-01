using System;
using Terraria_Server.Misc;
using Terraria_Server.Plugin;
using System.Collections.Generic;

namespace Terraria_Server
{
    ///<Summary>
    /// Provides access to the majority of Server Data
    ///</Summary>
    public class Server : Main
    {
        private PluginManager pluginManager = null;
        
        private World world = null;

        public Server() { }

        public Server(World World, int PlayerCap, string myWhiteList, string myBanList, string myOpList)
        {
            Main.maxNetplayers = PlayerCap;
            world = World;
            world.Server = this;
            pluginManager = new PluginManager(Statics.PluginPath, this);
            WhiteList = new DataRegister(myWhiteList);
            WhiteList.Load();
            BanList = new DataRegister(myBanList);
            BanList.Load();
            OpList = new DataRegister(myOpList);
            OpList.Load();
        }

        // Summary:
        //       Gets a specified Online Player
        public Player GetPlayerByName(string name)
        {
            String lowercaseName = name.ToLower();
            foreach (Player player in Main.players)
            {
                if (player.name.ToLower().Equals(lowercaseName))
                {
                    return player;
                }
            }
            return null;
        }

        // Summary:
        //       Gets the Plugin Manager
        public PluginManager getPluginManager()
        {
            return pluginManager;
        }

        // Summary:
        //       Gets the World Loaded for the Server
        public World getWorld()
        {
            return world;
        }

        // Summary:
        //       Sets the Server Password
        public void setOpPassword(String Password)
        {
            Netplay.password = Password;
        }

        // Summary:
        //       Sets the Terraria Binding Port
        public void setPort(int Port)
        {
            Netplay.serverPort = Port;
        }

        // Summary:
        //       Sets the Terraria Binding IP
        public void setIP(String IPAddress)
        {
            Netplay.serverSIP = IPAddress;
        }

        // Summary:
        //       Stops the Terraria Server
        public void StopServer()
        {
            Netplay.StopServer();
        }

        // Summary:
        //       Starts the Terraria Server
        public void StartServer()
        {
            Netplay.StartServer();
        }

        // Summary:
        //       Send a message to all online OPs
        public void notifyOps(string message, bool writeToConsole = false)
        {
            if (Statics.cmdMessages)
            {
                foreach (Player player in Main.players)
                {
                    if (player.active && player.Op)
                    {
                        NetMessage.SendData((int)Packet.PLAYER_CHAT, player.whoAmi, -1, message, 255, 176f, 196, 222f);
                    }
                }
            }
            Program.tConsole.WriteLine(message);
        }

        // Summary:
        //       Sends a Message to all Connected Clients
        public void notifyAll(string Message)
        {
            NetMessage.SendData((int)Packet.PLAYER_CHAT, -1, -1, Message, 255, 238f, 130f, 238f);
        }

        // Summary:
        //       Gets Terraria's God mode (Un-Usable?)
        public bool GodMode
        {
            get
            {
                return Main.godMode;
            }
            set
            {
                Main.godMode = value;
            }
        }

        // Summary:
        //       Gets the Servers Player List
        public Player[] getPlayerList()
        {
            return Main.players;
        }

        // Summary:
        //      Gets the White list 
        public DataRegister WhiteList { get; set; }

        // Summary:
        //       Gets the Ban list
        public DataRegister BanList { get; set; }

        // Summary:
        //       Gets the OP list
        public DataRegister OpList { get; set; }
    }
}
