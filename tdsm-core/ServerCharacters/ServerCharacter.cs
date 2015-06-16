
namespace tdsm.core.ServerCharacters
{
    public class ServerCharacter
    {
        public int Mana { get; set; }
        public int Health { get; set; }

        public byte HairDye { get; set; }
        public byte HideVisual { get; set; }
        public byte Difficulty { get; set; }

        public SimpleColor HairColor { get; set; }
        public SimpleColor SkinColor { get; set; }
        public SimpleColor EyeColor { get; set; }
        public SimpleColor ShirtColor { get; set; }
        public SimpleColor UnderShirtColor { get; set; }
        public SimpleColor PantsColor { get; set; }
        public SimpleColor ShoeColor { get; set; }

        public InventoryItem[] Inventory { get; set; }
    }

    class SimpleColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }

    class PlayerItem
    {
        public short NetId { get; set; }
        public short Stack { get; set; }
        public byte Prefix { get; set; }

        public PlayerItem(short netId, short stack, byte prefix)
        {
            NetId = netId;
            Stack = stack;
            Stack = stack;
        }
    }

    class InventoryItem : PlayerItem
    {
        public byte Slot { get; set; }

        public InventoryItem(short netId, short stack, byte prefix, byte slot)
            : base(netId, stack, prefix)
        {
            Slot = slot;
        }
    }
}
