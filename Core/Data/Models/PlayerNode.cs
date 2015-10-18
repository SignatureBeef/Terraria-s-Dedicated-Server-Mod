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
        public int Id { get; set; }

        /// <summary>
        /// Associated users identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Associated node identifier
        /// </summary>
        public int NodeId { get; set; }
    }
}

