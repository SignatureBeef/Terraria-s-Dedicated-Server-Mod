namespace Terraria_Server.Events
{
    public class BasePlayerEvent : CancellableEvent
    {
        public Player Player
        {
            get { return (Player)base.Sender; }
        }

    }
}
