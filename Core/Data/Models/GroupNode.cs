using System;

namespace TDSM.Core.Data.Models
{
    /// <summary>
    /// A group's link to the nodes table.
    /// </summary>
    public class GroupNode
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Dapper.Contrib.Extensions.Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        public long GroupId { get; set; }

        /// <summary>
        /// Gets or sets the node identifier.
        /// </summary>
        public long NodeId { get; set; }
    }
}

