using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.WorldMod;
using System.IO;

namespace Terraria_Server.Misc
{
    public enum BackupIOReturn : int
    {
        SUCCESS = 0,
        LOAD_FAIL = 1,
        SAVE_FAIL = 2,
        SAVE_LOCK = 3
    }

    public class BackupManager
    {
        public static BackupIOReturn LoadWorld(string Name)
        {
            WorldIO.LoadWorld(null, Statics.WorldPath + Path.DirectorySeparatorChar + Name);

            if (WorldModify.loadFailed && !WorldModify.loadSuccess)
            {
                return BackupIOReturn.LOAD_FAIL;
            }

            return BackupIOReturn.SUCCESS;
        }

        public static BackupIOReturn SaveWorld(string Name)
        {
            if (WorldModify.saveLock) return BackupIOReturn.SAVE_LOCK;
                //Please wait for the current operation to finish.

            if (WorldIO.SaveWorld(Statics.WorldPath + Path.DirectorySeparatorChar + Name))
                return BackupIOReturn.SUCCESS;
            else
                return BackupIOReturn.SAVE_FAIL;
        }

    }
}
