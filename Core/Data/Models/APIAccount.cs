using System;

namespace TDSM.Core.Data.Models
{
    /// <summary>
    /// REST/Web account
    /// </summary>
    public class APIAccount
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [Dapper.Contrib.Extensions.Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <value>The password.</value>
        [Dapper.Contrib.Extensions.Write(false)]
        public string Password
        {
            set
            {
                PasswordFormat = PasswordFormat.SHA256;
                PasswordHash = Hash_256(value);
            }
        }

        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        /// <value>The password hash.</value>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the password format.
        /// </summary>
        /// <value>The password format.</value>
        public PasswordFormat PasswordFormat { get; set; }

        /// <summary>
        /// Compares passwords using the appropriate hashing method.
        /// </summary>
        /// <returns><c>true</c>, if password was compared, <c>false</c> otherwise.</returns>
        /// <param name="password">Password.</param>
        public bool ComparePassword(string password)
        {
            if (PasswordFormat.SHA256 == PasswordFormat)
                return Hash_256(password).Equals(PasswordHash);

            return false;
        }

        /// <summary>
        /// Hashes a password using SHA-256
        /// </summary>
        /// <returns><c>true</c> if this instance hash 256 the specified password; otherwise, <c>false</c>.</returns>
        /// <param name="password">Password.</param>
        private string Hash_256(string password)
        {
            using (var hsr = System.Security.Cryptography.SHA256.Create())
            {
                var data = System.Text.Encoding.UTF8.GetBytes(password);
                var hashed = hsr.ComputeHash(data);

                var sb = new System.Text.StringBuilder();
                foreach (var b in hashed)
                    sb.AppendFormat("{0:x2}", b);

                return sb.ToString();
            }
        }
    }
}

