namespace Terraria_Server.Events
{
    public class BasePlayerEvent : Event
    {
        public Player Player
        {
            get { return (Player)base.Sender; }
        }

    }
}
