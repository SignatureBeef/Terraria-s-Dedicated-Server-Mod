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

        public Server(World World, int PlayerCap, String myWhiteList, String myBanList, String myOpList)
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
        //       Input name must already be cleaned of spaces
        public Player GetPlayerByName(String name)
        {
            String lowercaseName = name.ToLower();
            foreach (Player player in Main.players)
            {
                if (player.Name.ToLower().Replace(" ", "").Equals(lowercaseName))
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
        public void notifyOps(String message, bool writeToConsole = false)
        {
            if (Statics.cmdMessages)
            {
                foreach (Player player in Main.players)
                {
                    if (player.Active && player.Op)
                    {
                        NetMessage.SendData((int)Packet.PLAYER_CHAT, player.whoAmi, -1, message, 255, 176f, 196, 222f);
                    }
                }
            }
            Program.tConsole.WriteLine(message);
        }

        // Summary:
        //       Sends a Message to all Connected Clients
        public void notifyAll(String Message)
        {
            NetMessage.SendData((int)Packet.PLAYER_CHAT, -1, -1, Message, 255, 238f, 130f, 238f);
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

        // Summary:
        //       Get the array of Active NPCs
        public NPC[] getActiveNPCs()
        {
            NPC[] npcs = null;

            int npcCount = 0;
            for (int i = 0; i < NPC.MAX_NPCS; i++)
            {
                if (Main.npcs[i].Active)
                {
                    npcCount++;
                }
            }

            if (npcCount > 0)
            {
                npcs = new NPC[npcCount];
                npcCount = 0;
                for (int i = 0; i < Main.npcs.Length-1; i++)
                {
                    if (Main.npcs[i].Active)
                    {
                        npcs[npcCount] = Main.npcs[i];
                        npcCount++;
                    }
                }
            }
            
            return npcs;
        }

        // Summary:
        //       Gets the total of all active NPCs
        public int getActiveNPCCount()
        {
            int npcCount = 0;
            for (int i = 0; i < Main.npcs.Length - 1; i++)
            {
                if (Main.npcs[i].Active)
                {
                    npcCount++;
                }
            }
            return npcCount;
        }

        // Summary:
        //       Gets the maximum allowed NPCs
        public int getMaxNPCs()
        {
            return NPC.maxSpawns;
        }

        // Summary:
        //       Sets the maximum allowed NPCs
        public void setMaxNPCs(int Max)
        {
            NPC.defaultMaxSpawns = Max;
            NPC.maxSpawns = Max;
        }

        // Summary:
        //       Gets the max spawn rate of NPCs

        public int getSpawnRate()
        {
            return NPC.spawnRate;
        }

        // Summary:
        //       Sets the max spawn rate of NPCs
        public void setSpawnRate(int Max)
        {
            NPC.defaultSpawnRate = Max;
            NPC.spawnRate = Max;
        }
        
    }
}
