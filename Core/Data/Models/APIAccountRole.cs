using System;

namespace TDSM.Core.Data.Models
{
    /// <summary>
    /// API account role for use with the REST API
    /// </summary>
    public class APIAccountRole
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Dapper.Contrib.Extensions.Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        public long AccountId { get; set; }

        /// <summary>
        /// Gets or sets the claim type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the claims value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the valid starting date
        /// </summary>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// Gets or sets the end of the valid date range
        /// </summary>
        /// <remarks>If set to NULL the role is indefinite</remarks>
        public DateTime? DateTo { get; set; }
    }
}

