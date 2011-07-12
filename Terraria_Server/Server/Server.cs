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
        public PluginManager PluginManager
        {
            get
            {
                return pluginManager;
            }
        }

        // Summary:
        //       Gets the World Loaded for the Server
        public World World
        {
            get
            {
                return world;
            }
        }

        // Summary:
        //       Gets/Sets the Server Password
        public String OpPassword
        {
            get 
            {
                return Netplay.password;
            } 
            set 
            {
                Netplay.password = value;
            }
        }

        // Summary:
        //       Gets/Sets the Terraria Binding Port
        public int Port
        {
            get
            {
                return Netplay.serverPort;
            }
            set
            {
                Netplay.serverPort = value;
            }
        }

        // Summary:
        //       Gets/Sets the Terraria Binding IP
        public String ServerIP
        {
            get 
            {
                return Netplay.serverSIP;
            } 
            set 
            {
                Netplay.serverSIP = value;
            }
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
        public Player[] PlayerList
        {
            get
            {
                return Main.players;
            }
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
        //       Gets/Sets the maximum allowed NPCs
        public int MaxNPCs
        {
            get
            {
                return NPC.maxSpawns;
            }
            set
            {
                NPC.defaultMaxSpawns = value;
                NPC.maxSpawns = value;
            }
        }

        // Summary:
        //       Gets/Sets the max spawn rate of NPCs\
        public int SpawnRate
        {
            get
            {
                return NPC.spawnRate;
            }
            set
            {
                NPC.defaultSpawnRate = value;
                NPC.spawnRate = value;
            }
        }        
    }
}
