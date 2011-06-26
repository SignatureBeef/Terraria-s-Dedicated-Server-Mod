using Terraria_Server.Events;
namespace Terraria_Server
{
    public class PlayerCommandEvent : ConsoleCommandEvent
    {
        public Player Player
        {
            get
            {
                return (Player)base.Sender;
            }
            set
            {
                base.Sender = value;
            }
        }
    }
}
