using Terraria_Server.Commands;
namespace Terraria_Server.Events
{
    public class Event
    {
        public bool Cancelled { get; set; }

        public ISender Sender { get; set; }
    }
}
