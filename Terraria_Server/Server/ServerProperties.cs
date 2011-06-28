using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class ServerProperties : PropertiesFile
    {

        //PropertiesFile propFile;

        public ServerProperties(String pFile)
        {
            base.setFile(pFile);
        }
        
        public void pushData()
        {
            setMaxPlayers(getMaxPlayers());
            setServerIP(getServerIP());
            setPort(getPort());
            setGreeting(getGreeting());
            setInitialWorldPath(getInitialWorldPath());
            setServerPassword(getServerPassword());
            setUsingWhiteList(isUsingWhiteList());
            setAutomaticUpdates(automaticUpdates());
            setNPCDoorOpenCancel(isNPCDoorOpenCancelled());
            setSeed(getSeed());
            setMapSize(getMapSize());
            setUsingCutomTiles(isUsingCutomTiles());
            setMaxTilesX(getMaxTilesX());
            setMaxTilesY(getMaxTilesY());
            setUsingCustomGenOpts(getUsingCustomGenOpts());
            setDungeonAmount(getDungeonAmount());
            setFloatingIslandAmount(getFloatingIslandAmount());
            setDebugMode(debugMode());
        }

        public int getMaxPlayers()
        {
            string players = base.getValue("maxplayers");
            if (players == null || players.Trim().Length < 0 || Int32.Parse(players) <= -1)
            {
                return 8; //Default
            }
            else
            {
                return Int32.Parse(players);
            }
        }

        public void setMaxPlayers(int Max)
        {
            base.setValue("maxplayers", Max.ToString());
        }

        public int getPort()
        {
            string Port = base.getValue("port");
            if (Port == null || Port.Trim().Length < 0 || Int32.Parse(Port) <= -1)
            {
                return 7777;
            }
            else
            {
                return Int32.Parse(Port);
            }
        }

        public void setPort(int Port)
        {
            base.setValue("port", Port.ToString());
        }

        /*public string getServerName()
        {
            string Name = base.getValue("servername");
            if (Name == null || Name.Trim().Length < 0)
            {
                return "TDSM Server!";
            }
            else
            {
                return Name;
            }
        }

        public void setServerName(string Name)
        {
            base.setValue("servername", Name);
        }*/

        public string getGreeting()
        {
            string Greeting = base.getValue("greeting");
            if (Greeting == null || Greeting.Trim().Length < 0)
            {
                return "Welcome to a TDSM Server!";
            }
            else
            {
                return Greeting;
            }
        }

        public void setGreeting(string Greeting)
        {
            base.setValue("greeting", Greeting);
        }

        public string getServerIP()
        {
            string IP = base.getValue("serverip");
            if (IP == null || IP.Trim().Length < 0)
            {
                return "0.0.0.0";
            }
            else
            {
                return IP;
            }
        }

        public void setServerIP(string Greeting)
        {
            base.setValue("serverip", Greeting);
        }

        public string getInitialWorldPath()
        {
            string Path = base.getValue("worldpath");
            if (Path == null || Path.Trim().Length < 0)
            {
                return Statics.getWorldPath + Statics.systemSeperator + "world1.wld";
            }
            else
            {
                return Path;
            }
        }

        public void setInitialWorldPath(string Path)
        {
            base.setValue("worldpath", Path);
        }
        
        public string getServerPassword()
        {
            string Password = base.getValue("server-password");
            if (Password == null || Password.Trim().Length < 0)
            {
                return ""; //None?
            }
            else
            {
                return Password;
            }
        }

        public void setServerPassword(string Password)
        {
            base.setValue("server-password", Password);
        }

        public int getSeed()
        {
            string Seed = base.getValue("opt-seed");
            if (Seed == null || Seed.Trim().Length < 0)
            {
                return -1;
            }
            else
            {
                return Int32.Parse(Seed);
            }
        }

        public void setSeed(int Seed)
        {
            base.setValue("opt-seed", Seed.ToString());
        }

        public int getMaxTilesX()
        {
            string MaxTilesX = base.getValue("opt-maxtilesx");
            if (MaxTilesX == null || MaxTilesX.Trim().Length < 0 || Int32.Parse(MaxTilesX) <= -1)
            {
                return (int)World.MAP_SIZE.SMALL_X;
            }
            else
            {
                return Int32.Parse(MaxTilesX);
            }
        }

        public void setMaxTilesX(int MaxTilesX)
        {
            base.setValue("opt-maxtilesx", MaxTilesX.ToString());
        }

        public int getMaxTilesY()
        {
            string MaxTilesY = base.getValue("opt-maxtilesy");
            if (MaxTilesY == null || MaxTilesY.Trim().Length < 0 || Int32.Parse(MaxTilesY) <= -1)
            {
                return (int)World.MAP_SIZE.SMALL_Y;
            }
            else
            {
                return Int32.Parse(MaxTilesY);
            }
        }

        public void setMaxTilesY(int MaxTilesY)
        {
            base.setValue("opt-maxtilesy", MaxTilesY.ToString());
        }

        public bool isUsingCutomTiles()
        {
            string CustomTiles = base.getValue("opt-usecustomtiles");
            if (CustomTiles == null || CustomTiles.Trim().Length < 0)
            {
                return false;
            }
            else
            {
                return Boolean.Parse(CustomTiles);
            }
        }

        public void setUsingCutomTiles(bool CustomTiles)
        {
            base.setValue("opt-usecustomtiles", CustomTiles.ToString());
        }

        public int[] getMapSizes()
        {
            string CustomTiles = base.getValue("opt-mapsize");
            /*if (CustomTiles == null || CustomTiles.Trim().Length < 0)
            {
                return false;
            }
            else
            {
                return Boolean.Parse(CustomTiles);
            }*/
            if (CustomTiles == null)
            {
                return null;
            }
            switch (CustomTiles.Trim().ToLower())
            {
                case "small":
                    {
                        return new int[] { (int)World.MAP_SIZE.SMALL_X, (int)World.MAP_SIZE.SMALL_Y };
                    }
                case "medium":
                    {
                        return new int[] { (int)World.MAP_SIZE.MEDIUM_X, (int)World.MAP_SIZE.MEDIUM_Y };
                    }
                case "large":
                    {
                        return new int[] { (int)World.MAP_SIZE.LARGE_X, (int)World.MAP_SIZE.LARGE_Y };
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public string getMapSize()
        {
            string CustomTiles = base.getValue("opt-mapsize");
            if (CustomTiles == null)
            {
                return "small";
            }
            switch (CustomTiles.Trim().ToLower())
            {
                case "small":
                    {
                        return "small";
                    }
                case "medium":
                    {
                        return "medium";
                    }
                case "large":
                    {
                        return "large";
                    }
                default:
                    {
                        return "small";
                    }
            }
        }

        public void setMapSize(string MapSize)
        {
            base.setValue("opt-mapsize", MapSize);
        }

        public bool isUsingWhiteList()
        {
            string WhiteList = base.getValue("whitelist");
            if (WhiteList == null || WhiteList.Trim().Length < 0)
            {
                return false;
            }
            else
            {
                return Boolean.Parse(WhiteList);
            }
        }

        public void setUsingWhiteList(bool WhiteList)
        {
            base.setValue("whitelist", WhiteList.ToString());
        }

        public bool isNPCDoorOpenCancelled()
        {
            string NPCDoorOpen = base.getValue("npc-cancelopendoor");
            if (NPCDoorOpen == null || NPCDoorOpen.Trim().Length < 0)
            {
                return false;
            }
            else
            {
                return Boolean.Parse(NPCDoorOpen);
            }
        }

        public void setNPCDoorOpenCancel(bool NPCDoorOpen)
        {
            base.setValue("npc-cancelopendoor", NPCDoorOpen.ToString());
        }

        public void setDungeonAmount(int amount)
        {
            base.setValue("opt-numdungeons", amount.ToString());
        }

        public int getDungeonAmount()
        {
            string numDungeons = base.getValue("opt-numdungeons");
            if (numDungeons == null || numDungeons.Trim().Length < 0)
                return 1;
            else if (Int32.Parse(numDungeons) <= 0)
                return 1;
            else if (Int32.Parse(numDungeons) >= 10)
                return 10;
            else
                return Int32.Parse(numDungeons);
        }

        public void setUsingCustomGenOpts(bool customGen)
        {
            base.setValue("opt-custom-worldgen", customGen.ToString());
        }

        public bool getUsingCustomGenOpts()
        {
            string customGen = base.getValue("opt-custom-worldgen");
            if (customGen == null || customGen.Trim().Length < 0)
                return false;
            else
                return Boolean.Parse(customGen);
        }

        public void setFloatingIslandAmount(int ficount)
        {
            base.setValue("opt-num-floating-islands", ficount.ToString());
        }

        public int getFloatingIslandAmount()
        {
            string ficount = base.getValue("opt-num-floating-islands");
            if (ficount == null || ficount.Trim().Length < 0)
                return (int)((double)Main.maxTilesX * 0.0008);
            else if (Int32.Parse(ficount) > (int)((double)Main.maxTilesX * 0.0008) * 3)
                return (int)((double)Main.maxTilesX * 0.0008) * 3;
            else
                return Int32.Parse(ficount);
        }

        public bool debugMode()
        {
            string DebugMode = base.getValue("debugmode");
            if (DebugMode == null || DebugMode.Trim().Length < 0)
            {
                return false;
            }
            else
            {
                return Boolean.Parse(DebugMode);
            }
        }

        public void setDebugMode(bool DebugMode)
        {
            base.setValue("debugmode", DebugMode.ToString());
        }

        public bool automaticUpdates()
        {
            string AutomaticUpdates = base.getValue("allowupdates");
            if (AutomaticUpdates == null || AutomaticUpdates.Trim().Length < 0)
            {
                return true;
            }
            else
            {
                return Boolean.Parse(AutomaticUpdates);
            }
        }

        public void setAutomaticUpdates(bool AutomaticUpdates)
        {
            base.setValue("allowupdates", AutomaticUpdates.ToString());
        }
    }
}
