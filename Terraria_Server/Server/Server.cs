using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

using Terraria_Server.Plugin;

namespace Terraria_Server
{
    ///<Summary>
    /// Provides access to the majority of Server Data
    ///</Summary>
    public class Server : Main
    {
        private PluginManager pluginManager = null;
        
        private World world = null;

        private DataRegister whiteList = null;
        private DataRegister banList = null;
        private DataRegister opList = null;

        public Server() { }

        public Server(World World, int PlayerCap, string WhiteList, string BanList, string OpList)
        {
            Main.maxNetplayers = PlayerCap;
            world = World;
            world.setServer(this);
            pluginManager = new PluginManager(Statics.getPluginPath, this);
            whiteList = new DataRegister(WhiteList);
            whiteList.Load();
            banList = new DataRegister(BanList);
            banList.Load();
            opList = new DataRegister(OpList);
            opList.Load();
        }
    
        ///<Summary>
        /// Gets a specified Online Player
        ///</Summary>
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
                        if (Main.player[i].isOp())
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
        public bool getGodMode()
        {
            return Main.godMode;
        }

        // Summary:
        //       Sets Terraria's God mode (Un-Usable?)
        public void setGodMode(bool Status)
        {
            Main.godMode = Status;
        }

        // Summary:
        //       Gets the Servers Player List
        public Player[] getPlayerList()
        {
            return Main.player;
        }

        // Summary:
        //      Gets the White list 
        public DataRegister getWhiteList()
        {
            return whiteList;
        }

        // Summary:
        //       Sets the White list
        public void setWhiteList(DataRegister WhiteList)
        {
            whiteList = WhiteList;
        }

        // Summary:
        //       Gets the Ban list
        public DataRegister getBanList()
        {
            return banList;
        }

        // Summary:
        //       Sets the Ban list
        public void setBanList(DataRegister BanList)
        {
            banList = BanList;
        }

        // Summary:
        //       Gets the OP list
        public DataRegister getOpList()
        {
            return opList;
        }

        // Summary:
        //       Sets the OP list
        public void setOpList(DataRegister OpList)
        {
            opList = OpList;
        }

        // Summary:
        //       Get the array of Active NPCs
        public NPC[] getActiveNPCs()
        {
            NPC[] npcs = null;

            int npcCount = 0;
            for (int i = 0; i < Main.npc.Length-1; i++)
            {
                if (Main.npc[i].active)
                {
                    npcCount++;
                }
            }

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
            for (int i = 0; i < Main.npc.Length - 1; i++)
            {
                if (Main.npc[i].active)
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
        //       Gets the max spawn rat eof NPCs
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
