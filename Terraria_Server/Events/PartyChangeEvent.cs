using Terraria_Server.Plugin;
namespace Terraria_Server.Events
{
    public class PartyChangeEvent : BasePlayerEvent
    {
        public PartyChangeEvent() 
        {
            PartyType = Party.NONE;
        }

        public Party PartyType { get; set; }
    }
}
