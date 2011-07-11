namespace Terraria_Server.Events
{
    public class PlayerLoginEvent : BasePlayerEvent
    {
        public ServerSlot Socket { get; set; }
    }
}
