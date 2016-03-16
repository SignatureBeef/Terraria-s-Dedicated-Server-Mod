namespace TDSM.Core.ServerCharacters.Models
{
    public class SlotItem //: PlayerItem
    {
        /// <summary>
        /// Internal database Id
        /// </summary>
        /// <value>The identifier.</value>
        public long Id { get; set; }

        public int NetId { get; set; }

        public int Stack { get; set; }

        public int Prefix { get; set; }

        public bool Favorite { get; set; }

        public long? CharacterId { get; set; } //FOR DB

        public ItemType Type { get; set; }

        public int Slot { get; set; }

        public SlotItem(int netId, int stack, byte prefix, bool favorite, int slot)
        {
            Slot = slot;

            NetId = netId;
            Stack = stack;
            Prefix = prefix;
            Favorite = favorite;
        }

        public SlotItem() //Serialisation
        {

        }
    }
}
