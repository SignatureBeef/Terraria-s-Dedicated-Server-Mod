using System;

namespace TDSM.Core.Data.Models
{
    /// <summary>
    /// Generic OTA group information
    /// </summary>
    public class Group
    {
        [Dapper.Contrib.Extensions.Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public bool ApplyToGuests { get; set; }

        public string Parent { get; set; }

        public byte Chat_Red { get; set; }

        public byte Chat_Green { get; set; }

        public byte Chat_Blue { get; set; }

        public string Chat_Prefix { get; set; }

        public string Chat_Suffix { get; set; }
    }
}

