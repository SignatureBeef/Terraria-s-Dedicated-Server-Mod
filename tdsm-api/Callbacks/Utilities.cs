using System;

namespace tdsm.api.Callbacks
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
            return Terraria.Utilities.FileOperationAPIWrapper.MoveToRecycleBin(path);
        }
    }
}

