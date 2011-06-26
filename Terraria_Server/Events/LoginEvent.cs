namespace Terraria_Server.Events
{
    public class LoginEvent : BasePlayerEvent
    {
        public ServerSock Socket { get; set; }
    }
}
