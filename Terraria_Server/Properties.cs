using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class Properties : PropertiesFile
    {

        //PropertiesFile propFile;

        public Properties(String pFile)
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
            setOpPassword(getOpPassword());
            setSeed(getSeed());
            setMapSize(getMapSize());
            setNPCDoorOpenCancel(isNPCDoorOpenCancelled());
            setUsingCutomTiles(isUsingCutomTiles());
            setMaxTilesX(getMaxTilesX());
            setMaxTilesY(getMaxTilesY());
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
        
        public string getOpPassword()
        {
            string Password = base.getValue("op-password");
            if (Password == null || Password.Trim().Length < 0)
            {
                return ""; //None?
            }
            else
            {
                return Password;
            }
        }

        public void setOpPassword(string Password)
        {
            base.setValue("op-password", Password);
        }

        public int getSeed()
        {
            string Seed = base.getValue("optional-seed");
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
            base.setValue("optional-seed", Seed.ToString());
        }

        public int getMaxTilesX()
        {
            string MaxTilesX = base.getValue("optional-maxtilesx");
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
            base.setValue("optional-maxtilesx", MaxTilesX.ToString());
        }

        public int getMaxTilesY()
        {
            string MaxTilesY = base.getValue("optional-maxtilesy");
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
            base.setValue("optional-maxtilesy", MaxTilesY.ToString());
        }

        public bool isUsingCutomTiles()
        {
            string CustomTiles = base.getValue("usecustomtiles");
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
            base.setValue("usecustomtiles", CustomTiles.ToString());
        }

        public int[] getMapSizes()
        {
            string CustomTiles = base.getValue("optional-mapsize");
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
            string CustomTiles = base.getValue("optional-mapsize");
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
            base.setValue("optional-mapsize", MapSize);
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
            string WhiteList = base.getValue("npc-opendoor");
            if (WhiteList == null || WhiteList.Trim().Length < 0)
            {
                return false;
            }
            else
            {
                return Boolean.Parse(WhiteList);
            }
        }

        public void setNPCDoorOpenCancel(bool NPCDoorOpen)
        {
            base.setValue("npc-opendoor", NPCDoorOpen.ToString());
        }
    }
}
