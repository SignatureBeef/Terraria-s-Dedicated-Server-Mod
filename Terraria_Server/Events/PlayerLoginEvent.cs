namespace Terraria_Server.Events
{
    public class PlayerLoginEvent : BasePlayerEvent
    {
        public ServerSock Socket { get; set; }
    }
}
