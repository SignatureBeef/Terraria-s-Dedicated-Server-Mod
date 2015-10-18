using System;

namespace TDSM.Core.Data
{
    /// <summary>
    /// Default OTA user information
    /// </summary>
    public class DbPlayer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public bool Operator { get; set; }

        public DateTime DateAddedUTC { get; set; }

        public DbPlayer()
        {
        }

        public DbPlayer(string name, string rawPassword)
        {
            this.Name = name;
            this.Password = AuthenticatedUsers.Hash(name, rawPassword);
        }

        public override string ToString()
        {
            return String.Format("[UserDetails: Id {3}, Name: '{0}', Password: '{1}', Operator: {2}]", Name, Password, Operator, Id);
        }

        public void SetRawPassword(string password)
        {
            this.Password = AuthenticatedUsers.Hash(this.Name, password);
        }

        /// <summary>
        /// Compares a username & password to the current instance
        /// </summary>
        /// <returns><c>true</c>, if password was compared, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        public bool ComparePassword(string name, string password)
        {
            var hs = AuthenticatedUsers.Hash(name, password);

            return hs.Equals(Password);
        }
    }
}

