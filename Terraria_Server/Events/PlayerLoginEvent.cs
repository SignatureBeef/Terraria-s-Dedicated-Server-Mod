namespace Terraria_Server.Events
{
	public enum PlayerLoginAction
	{
		ACCEPT = 0,
		ASK_PASS,
		REJECT
	}
	
	public enum LoginPriority
	{
		QUEUE_LOW_PRIO = 0,
		QUEUE_MEDIUM_PRIO = 1,
		QUEUE_HIGH_PRIO = 2,
		BYPASS_QUEUE = 3,
	}

    public class PlayerLoginEvent : BasePlayerEvent
    {
        public ServerSlot Slot { get; set; }
        
        public PlayerLoginAction Action { get; set; }
        public LoginPriority     Priority { get; set; }
        
        public string Password { get; set; }
    }
}
