
namespace tdsm.api.Command
{
    /// <summary>
    /// Sender class for command sending/parsing
    /// </summary>
    public abstract class Sender : ISender
    {
        /// <summary>
        /// Get/set method for Sender's Op status
        /// </summary>
        public bool Op { get; set; }

        /// <summary>
        /// Gets the name of the sending entity; defaults to Console
        /// </summary>
        /// <returns>Sending entity name</returns>
        public virtual string Name { get; protected set; }

        public string SenderName
        {
            get
            { return this.Name; }
        }

        /// <summary>
        /// Writes a message to the server console
        /// </summary>
        /// <param name="Message">Message to write to the console</param>
        /// <param name="A"></param>
        /// <param name="R">Red text color value</param>
        /// <param name="G">Green text color value</param>
        /// <param name="B">Blue text color value</param>
        public abstract void SendMessage(string Message, int A = 255, float R = 255f, float G = 0f, float B = 0f);

        //public bool Is(Type type)
        //{
        //    return this.GetType().IsAssignableFrom(type);
        //}

        public bool IsPlayer()
        {
            return this is Terraria.Player;
        }
    }
}