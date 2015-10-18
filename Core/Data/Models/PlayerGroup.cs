using System;

namespace TDSM.Core.Data.Models
{
    /// <summary>
    /// A players link to their group
    /// </summary>
    public class PlayerGroup
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        public int GroupId { get; set; }
    }
}

