using Dapper.Contrib.Extensions;
using System;

namespace TDSM.Core.Data.Models
{
    /// <summary>
    /// Default OTA user information
    /// </summary>
    [Table("Players")]
    public class DbPlayer
    {
        [Dapper.Contrib.Extensions.Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public bool Operator { get; set; }

        public DateTime DateAdded { get; set; }

        public DbPlayer()
        {
        }

        public DbPlayer(string name, string rawPassword)
        {
            this.Name = name;
            this.Password = Authentication.Hash(name, rawPassword);
        }

        public override string ToString()
        {
            return String.Format("[UserDetails: Id {3}, Name: '{0}', Password: '{1}', Operator: {2}]", Name, Password, Operator, Id);
        }

        public void SetRawPassword(string password)
        {
            this.Password = Authentication.Hash(this.Name, password);
        }

        /// <summary>
        /// Compares a username & password to the current instance
        /// </summary>
        /// <returns><c>true</c>, if password was compared, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        public bool ComparePassword(string name, string password)
        {
            var hs = Authentication.Hash(name, password);

            return hs.Equals(Password);
        }
    }
}

