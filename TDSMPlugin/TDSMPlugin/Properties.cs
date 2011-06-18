using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;

namespace TDSMPlugin
{
    public class Properties : PropertiesFile
    {
        public Properties(String pFile)
        {
            base.setFile(pFile);
        }

        public void pushData()
        {
            setSpawningCancelled(isSpawningCancelled());
            setTileBreakage(getTileBreakage());
        }

        public bool isSpawningCancelled()
        {
            string AllowSpawns = base.getValue("allowspawns");
            if (AllowSpawns == null || AllowSpawns.Trim().Length < 0)
            {
                return true;
            }
            else
            {
                return Boolean.Parse(AllowSpawns);
            }
        }

        public void setSpawningCancelled(bool AllowSpawns)
        {
            base.setValue("allowspawns", AllowSpawns.ToString());
        }

        public bool getTileBreakage()
        {
            string TileBreakage = base.getValue("tilebreakage");
            if (TileBreakage == null || TileBreakage.Trim().Length < 0)
            {
                return true;
            }
            else
            {
                return Boolean.Parse(TileBreakage);
            }
        }

        public void setTileBreakage(bool TileBreakage)
        {
            base.setValue("tilebreakage", TileBreakage.ToString());
        }
    }
}
