using System;

namespace TDSM.Core.ServerCharacters
{
    public enum CharacterMode
    {
        /// <summary>
        /// No server characters.
        /// </summary>
        NONE,

        /// <summary>
        /// Username and password authentication.
        /// </summary>
        AUTH,

        /// <summary>
        /// Client UUID - still not all that trustworthy
        /// </summary>
        UUID
    }

    public static class CharacterModeExtensions
    {
        public static bool TryParse(string input, out CharacterMode mode)
        {
            mode = CharacterMode.NONE;
            try
            {
                mode = (CharacterMode)Enum.Parse(typeof(CharacterMode), input);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

