using System;

namespace TDSM.Core.Data.Models
{
    /// <summary>
    /// Players link to a nodes permission.
    /// </summary>
    public class PlayerNode
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Associated users identifier
        /// </summary>
        public long PlayerId { get; set; }

        /// <summary>
        /// Associated node identifier
        /// </summary>
        public long NodeId { get; set; }
    }
}

