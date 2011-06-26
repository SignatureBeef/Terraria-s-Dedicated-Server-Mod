namespace Terraria_Server.Events
{
    public class PlayerStateUpdateEvent : BasePlayerEvent
    {
        public byte State { get; set; }
    }
}
