using OTA.Permissions;

namespace TDSM.Core.Data.Models
{
    /// <summary>
    /// An entity's permission node
    /// </summary>
    public class NodePermission
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

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

