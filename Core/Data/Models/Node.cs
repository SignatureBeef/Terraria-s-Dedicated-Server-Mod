using OTA.Permissions;

namespace TDSM.Core.Data.Models
{
    /// <summary>
    /// An entity's permission node
    /// </summary>
    public class PermissionNode
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Dapper.Contrib.Extensions.Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        public string Node { get; set; }

        /// <summary>
        /// Gets or sets the permission.
        /// </summary>
        public Permission Permission { get; set; }
    }
}

