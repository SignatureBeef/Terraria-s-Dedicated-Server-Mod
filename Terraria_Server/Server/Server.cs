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
            //joinedPlayerList = new DataRegister(JoinedPlayers);
            //joinedPlayerList.Load();
            banList = new DataRegister(BanList);
            banList.Load();
            opList = new DataRegister(OpList);
            opList.Load();
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

        public DataRegister getOpList()
        {
            return opList;
        }

        public void setOpList(DataRegister OpList)
        {
            opList = OpList;
        }

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

        public int getMaxNPCs()
        {
            return NPC.maxSpawns;
        }

        public void setMaxNPCs(int Max)
        {
            NPC.defaultMaxSpawns = maxBackgrounds;
            NPC.maxSpawns = Max;
        }

        public int getSpawnRate()
        {
            return NPC.spawnRate;
        }

        public void setSpawnRate(int Max)
        {
            NPC.defaultSpawnRate = Max;
            NPC.spawnRate = Max;
        }


    }
}
