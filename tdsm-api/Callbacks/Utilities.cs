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

    public class Rand : Random
    {
        public override int Next(int maxValue)
        {
            if (maxValue <= 0)
                return 0;
            return base.Next(maxValue);
        }

        public override int Next(int minValue, int maxValue)
        {
            if (minValue < maxValue)
                return base.Next(minValue, maxValue);
            return 0;
        }
    }
}

