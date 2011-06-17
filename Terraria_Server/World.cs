using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class World
    {

        public enum MAP_SIZE : int
        {
            SMALL_X = 4200,
            MEDIUM_X = 6300,
            LARGE_X = 8400,
            SMALL_Y = 1200,
            MEDIUM_Y = 1800,
            LARGE_Y = 2400
        }


        /*public static int[] getWorldSize(String size) {
            string nSize = size.ToLower().Trim();
            if (nSize.Equals("small"))
            {
                return new int[] { 4200, };
            }
            else if (nSize.Equals("medium"))
            {
                
                return new int[] { 6300, 1800};
            }
            else if (nSize.Equals("large"))
            {
                return new int[] { 8400, 2400};
            }
            else
            {
                return null;
            }
        }*/


        public string name = "";
        public string savePath = "";
        public Server server = null;
        public int seed = -1; //default
        public int id = 0;

        public float leftWorld = 0.0f;
        public float rightWorld = 134400f;
        public float topWorld = 0.0f;
        public float bottomWorld = 38400f;
        public int maxTilesX;
        public int maxTilesY;
        public int maxSectionsX;
        public int maxSectionsY;

        public World(int MaxTilesX, int MaxTilesY)
        {
            maxTilesY = MaxTilesY;
            maxTilesX = MaxTilesX;

            UpdateWorldCoords(false);
        }

        public World(Server Server, int MaxTilesX, int MaxTilesY)
        {
            maxTilesY = MaxTilesY;
            maxTilesX = MaxTilesX;

            server = Server;

            UpdateWorldCoords(false);
        }

        public void UpdateWorldCoords(bool useWorld) {
            if (useWorld)
            {
                maxTilesX = (int)rightWorld / 16 + 1;
                maxTilesY = (int)bottomWorld / 16 + 1;
            }
            maxSectionsX = maxTilesX / 200;
            maxSectionsY = maxTilesY / 150;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string Name)
        {
            name = Name;
        }
    
        public Server getServer()
        {
            return server;
        }

        public void setServer(Server Server)
        {
            server = Server;
        }

        public int getSeed()
        {
            return seed;
        }

        public void setSeed(int Seed)
        {
            seed = Seed;
        }

        public int getId()
        {
            return id;
        }

        public void setId(int ID)
        {
            id = ID;
        }

        public double getTime()
        {
            return Main.time;
        }

        public void setTime(double Time)
        {
            Main.time = Time;

            if (Main.time >= 13500)
            {
                Main.dayTime = true;
            }
            else
            {
                Main.dayTime = false;
            }
        }

        public string getSavePath()
        {
            return savePath;
        }

        public void setSavePath(string SavePath)
        {
            savePath = SavePath;
        }

        public int getMaxSectionsY()
        {
            return maxSectionsY;
        }

        public void setMaxSectionsY(int MaxSectionsY)
        {
            maxSectionsY = MaxSectionsY;
        }

        public int getMaxSectionsX()
        {
            return maxSectionsX;
        }

        public void setMaxSectionsX(int MaxSectionsX)
        {
            maxSectionsX = MaxSectionsX;
        }

        public int getMaxTilesY()
        {
            return maxTilesY;
        }

        public void setMaxTilesY(int MaxTilesY, bool updateSection = true)
        {
            maxTilesY = MaxTilesY;
            if (updateSection)
            {
                UpdateWorldCoords(false);
            }
        }

        public int getMaxTilesX()
        {
            return maxTilesX;
        }

        public void setMaxTilesX(int MaxTilesX, bool updateSection = true)
        {
            maxTilesX = MaxTilesX;
            if (updateSection)
            {
                UpdateWorldCoords(false);
            }
        }

        public float getBottomWorld()
        {
            return bottomWorld;
        }

        public void setBottomWorld(float BottomWorld)
        {
            bottomWorld = BottomWorld;
        }

        public float getTopWorld()
        {
            return topWorld;
        }

        public void setTopWorld(float TopWorld)
        {
            topWorld = TopWorld;
        }

        public float getRightWorld()
        {
            return rightWorld;
        }

        public void setRightWorld(float RightWorld)
        {
            rightWorld = RightWorld;
        }

        public float getLeftWorld()
        {
            return leftWorld;
        }

        public void setLeftWorld(float LeftWorld)
        {
            leftWorld = LeftWorld;
        }

        public Tile getHighestTile(int axisX)
        {
            int tallest = -1;

            for (int i = 0; i < maxTilesY; i++)
            {
                if (Server.maxTilesY > axisX && Server.tile[axisX, i] != null && Server.tile[axisX, i].active)
                {
                    if (i > tallest)
                    {
                        tallest = i;
                    }
                }
            }
            if (tallest >= 0)
            {
                return WorldGen.cloneTile(Server.tile[axisX, tallest], axisX, tallest);
            }
            return null;
        }

    }

}
