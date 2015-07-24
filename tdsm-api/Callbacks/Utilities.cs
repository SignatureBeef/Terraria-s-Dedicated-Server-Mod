using System;

namespace TDSM.API.Callbacks
{
    public static class Utilities
    {
        public static bool RemoveFile(string path)
        {
            if (Tools.RuntimePlatform == RuntimePlatform.Mono)
            {
                try
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            #if Full_API
            return Terraria.Utilities.FileOperationAPIWrapper.MoveToRecycleBin(path);
            #else
            return false;
            #endif
        }
    }
}

