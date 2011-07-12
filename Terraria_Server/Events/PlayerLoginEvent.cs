namespace Terraria_Server.Events
{
	public enum PlayerLoginAction
	{
		ACCEPT = 0,
		ASK_PASS,
		REJECT
	}

    public class PlayerLoginEvent : BasePlayerEvent
    {
        public ServerSlot Slot { get; set; }
        
        public PlayerLoginAction Action { get; set; }
    }
}
