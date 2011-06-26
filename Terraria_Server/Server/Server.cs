
using Terraria_Server.Plugin;
using System;
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
            pluginManager = new PluginManager(Statics.getPluginPath, this);
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
            foreach (Player player in Main.player)
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
        public void notifyOps(string Message, bool writeToConsole = false)
        {
            if (Statics.cmdMessages)
            {
                for (int i = 0; i < 255; i++)
                {
                    if (Main.player[i].active)
                    {
                        if (Main.player[i].Op)
                        {
                            NetMessage.SendData((int)Packet.PLAYER_CHAT, Main.player[i].whoAmi, -1, Message, 255, 176f, 196, 222f);
                        }
                    }
                }
            }
            Program.tConsole.WriteLine(Message);
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
            return Main.player;
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

            int npcCount = getActiveNPCCount();
            if (npcCount > 0)
            {
                npcs = new NPC[npcCount];
                npcCount = 0;
                for (int i = 0; i < Main.npc.Length-1; i++)
                {
                    if (Main.npc[i].active)
                    {
                        npcs[npcCount] = Main.npc[i];
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
            foreach (NPC npc in Main.npc)
            {
                if (npc.active)
                {
                    npcCount++;
                }
            }
            return npcCount;
        }

        // Summary:
        //       Gets the maximum allowed NPCs
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
        //       Gets the max spawn rat eof NPCs
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
