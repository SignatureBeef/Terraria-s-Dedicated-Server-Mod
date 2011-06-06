using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;

namespace HomePlugin
{
    class HomeProperties : PropertiesFile
    {
        public HomeProperties(String propLoc)
        {
            base.setFile(propLoc);
        }

        public Boolean hasHome(Player p)
        {
            string save = base.getValue("homeSaveData");
            if (save == null)
            {
                return false;
            }
            else
            {
                if (save.Contains(p.name))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int getHomeX(Player p)
        {
            string save = base.getValue("homeSaveData");
            if (save == null)
            {
                return 0;
            }
            else
            {
                if (save.Contains(p.name))
                {
                    string[] saveSplit = save.Split(':');
                    foreach (string player in saveSplit)
                    {
                        if (player.Contains(p.name))
                        {
                            string[] playerSplit = player.Split('X');
                            return int.Parse(playerSplit[1].Split('!')[0]);
                        }
                    }

                }
                return 0;
            }
        }

        public int getHomeY(Player p)
        {
            string save = base.getValue("homeSaveData");
            if (save == null)
            {
                return 0;
            }
            else
            {
                if (save.Contains(p.name))
                {
                    string[] saveSplit = save.Split(':');
                    foreach (string player in saveSplit)
                    {
                        if (player.Contains(p.name))
                        {
                            string[] playerSplit = player.Split('X');
                            return int.Parse(playerSplit[1].Split('!')[1]);
                        }
                    }

                }
                return 0;
            }
        }

        public void setHome(Player p)
        {
            string save = base.getValue("homeSaveData");
            if (save == null) { }
            else
            {
                if (!save.Contains(p.name))
                {
                    base.setValue("homeSaveData", save + p.name + "X" + p.bodyPosition.X + "!" + p.bodyPosition.Y + ":");
                }
                else
                {
                    string[] saveSplit = save.Split(':');
                    foreach (string player in saveSplit)
                    {
                        if (player.Contains(p.name))
                        {
                            base.setValue("homeSaveData", save.Replace(player, p.name + "X" + p.bodyPosition.X + "!" + p.bodyPosition.Y + ":"));
                        }
                    }
                }
            }
        }
    }
}
