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
        [Dapper.Contrib.Extensions.Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public long PlayerId { get; set; }

        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        public long GroupId { get; set; }
    }
}

