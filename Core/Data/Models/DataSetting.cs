using System;

namespace TDSM.Core.Data.Models
{
    /// <summary>
    /// A setting that can be stored in an OTA database
    /// </summary>
    public class DataSetting
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the setting key.
        /// </summary>
        public string DataKey { get; set; }

        /// <summary>
        /// Gets or sets the setting value.
        /// </summary>
        public string DataValue { get; set; }
    }
}

