using System;
using System.Linq;
using Terraria;

namespace TDSM.Core.ServerCharacters
{
    public class NewPlayerInfo
    {
        public int Mana { get; set; }
        public int Health { get; set; }

        public PlayerItem[] Inventory { get; set; }
    }

    public class ServerCharacter : IDisposable
    {
        public bool Male { get; set; }

        public int Mana { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }

        public int SpawnX { get; set; }
        public int SpawnY { get; set; }

        public int Hair { get; set; }
        public byte HairDye { get; set; }

        public bool[] HideVisual { get; set; }
        public byte Difficulty { get; set; }

        public SimpleColor HairColor { get; set; }
        public SimpleColor SkinColor { get; set; }
        public SimpleColor EyeColor { get; set; }
        public SimpleColor ShirtColor { get; set; }
        public SimpleColor UnderShirtColor { get; set; }
        public SimpleColor PantsColor { get; set; }
        public SimpleColor ShoeColor { get; set; }

        public System.Collections.Generic.List<SlotItem> Inventory { get; set; }
        public System.Collections.Generic.List<SlotItem> Dye { get; set; }
        public System.Collections.Generic.List<SlotItem> Armor { get; set; }

        /// <summary>
        /// Creates a new server config based off a Terrarian player instance
        /// </summary>
        /// <param name="player"></param>
        public ServerCharacter(Player player)
        {
            //this.Male = player.male;

            this.Mana = player.statMana;
            this.Health = player.statLife;
            this.MaxHealth = player.statLifeMax;

            this.SpawnX = player.SpawnX;
            this.SpawnY = player.SpawnY;

            this.HideVisual = player.hideVisual;
            this.HairDye = player.hairDye;

            this.Hair = player.hair;
            this.Difficulty = player.difficulty;

            this.HairColor = player.hairColor;
            this.SkinColor = player.skinColor;
            this.EyeColor = player.eyeColor;
            this.ShirtColor = player.shirtColor;
            this.UnderShirtColor = player.underShirtColor;
            this.PantsColor = player.pantsColor;
            this.ShoeColor = player.shoeColor;

            this.Inventory = player.inventory
                .Select((item, index) => item == null ? null : new SlotItem(item.netID, item.stack, item.prefix, index))
                .Where(x => x != null)
                .ToList();

            this.Dye = player.dye
                .Select((item, index) => item == null ? null : new SlotItem(item.netID, item.stack, item.prefix, index))
                .Where(x => x != null)
                .ToList();

            this.Armor = player.armor
                .Select((item, index) => item == null ? null : new SlotItem(item.netID, item.stack, item.prefix, index))
                .Where(x => x != null)
                .ToList();

            //this.Inventory = new System.Collections.Generic.List<SlotItem>();
            //for (var x = 0; x < player.inventory.Length; x++)
            //{
            //    var item = player.inventory[x];
            //    if (item != null)
            //    {
            //        this.Inventory.Add(new SlotItem(item.netID, item.stack, item.prefix, x));
            //    }
            //}

            //this.Dye = new System.Collections.Generic.List<SlotItem>();
            //for (var x = 0; x < player.dye.Length; x++)
            //{
            //    var item = player.dye[x];
            //    if (item != null)
            //    {
            //        this.Dye.Add(new SlotItem(item.netID, item.stack, item.prefix, x));
            //    }
            //}

            //this.Armor = new System.Collections.Generic.List<SlotItem>();
            //for (var x = 0; x < player.armor.Length; x++)
            //{
            //    var item = player.armor[x];
            //    if (item != null)
            //    {
            //        this.Armor.Add(new SlotItem(item.netID, item.stack, item.prefix, x));
            //    }
            //}
        }

        public ServerCharacter(NewPlayerInfo info, Player player)
        {
            //I need to test this soon. I'm not sure that the first time a player authenticates whether we use the existing player data (ie male, colours etc)
            //At this stage, it'll clone

            //this.Male = player.male;

            this.Mana = info.Mana;
            this.Health = info.Health;
            this.MaxHealth = info.Health;

            this.SpawnX = player.SpawnX;
            this.SpawnY = player.SpawnY;

            this.HideVisual = player.hideVisual;
            this.HairDye = player.hairDye;

            this.Hair = player.hair;
            this.Difficulty = player.difficulty;

            this.HairColor = player.hairColor;
            this.SkinColor = player.skinColor;
            this.EyeColor = player.eyeColor;
            this.ShirtColor = player.shirtColor;
            this.UnderShirtColor = player.underShirtColor;
            this.PantsColor = player.pantsColor;
            this.ShoeColor = player.shoeColor;

            this.Inventory = info.Inventory
                .Select((item, index) => item == null ? null : new SlotItem(item.NetId, item.Stack, item.Prefix, index))
                .ToList();
        }

        /// <summary>
        /// Applies server config data to the player
        /// </summary>
        /// <param name="player"></param>
        public void ApplyToPlayer(Player player)
        {
            //player.male = this.Male;

            player.statMana = this.Mana;
            player.statLife = this.Health;
            player.statLifeMax = this.MaxHealth;

            player.SpawnX = this.SpawnX;
            player.SpawnY = this.SpawnY;

            player.hideVisual = this.HideVisual;
            player.hairDye = this.HairDye;

            player.hair = this.Hair;
            player.difficulty = this.Difficulty;

            player.hairColor = this.HairColor.ToXna();
            player.skinColor = this.SkinColor.ToXna();
            player.eyeColor = this.EyeColor.ToXna();
            player.shirtColor = this.ShirtColor.ToXna();
            player.underShirtColor = this.UnderShirtColor.ToXna();
            player.pantsColor = this.PantsColor.ToXna();
            player.shoeColor = this.ShoeColor.ToXna();

            //Reset and populate inventory
            player.inventory = Enumerable.Repeat(new Item(), player.inventory.Length).ToArray();
            foreach (var slotItem in this.Inventory)
            {
                var item = new Terraria.Item();

                item.netDefaults(slotItem.NetId);
                item.stack = slotItem.Stack;
                item.Prefix(slotItem.Prefix);

                player.inventory[slotItem.Slot] = item;
            }

            //Reset and populate dye
            player.dye = Enumerable.Repeat(new Item(), player.dye.Length).ToArray();
            foreach (var slotItem in this.Dye)
            {
                var item = new Terraria.Item();

                item.netDefaults(slotItem.NetId);
                item.Prefix(slotItem.Prefix);

                player.dye[slotItem.Slot] = item;
            }

            //Reset and populate armor
            player.armor = Enumerable.Repeat(new Item(), player.armor.Length).ToArray();
            foreach (var slotItem in this.Armor)
            {
                var item = new Terraria.Item();

                item.netDefaults(slotItem.NetId);
                item.Prefix(slotItem.Prefix);

                player.armor[slotItem.Slot] = item;
            }

            //Update client
            this.Send(player);
        }

        public void Send(Player player)
        {
#if TDSMServer
            var msg = NewNetMessage.PrepareThreadInstance();
            msg.PlayerData(player.whoAmI);
            msg.BuildPlayerUpdate(player.whoAmI);
            msg.Broadcast();
#endif
        }

        public void Dispose()
        {
            this.Male = false;

            this.Mana = 0;
            this.Health = 0;
            this.MaxHealth = 0;

            this.SpawnX = 0;
            this.SpawnY = 0;

            this.HideVisual = null;
            this.HairDye = 0;

            this.Hair = 0;
            this.Difficulty = 0;

            this.HairColor = null;
            this.SkinColor = null;
            this.EyeColor = null;
            this.ShirtColor = null;
            this.UnderShirtColor = null;
            this.PantsColor = null;
            this.ShoeColor = null;

            this.Inventory.Clear(); this.Inventory = null;
            this.Dye.Clear(); this.Dye = null;
            this.Armor.Clear(); this.Armor = null;
        }
    }

    public class SimpleColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public SimpleColor() { }

        public SimpleColor(Microsoft.Xna.Framework.Color color)
        {
            this.R = color.R;
            this.G = color.G;
            this.B = color.B;
        }

        public Microsoft.Xna.Framework.Color ToXna()
        {
            return new Microsoft.Xna.Framework.Color(R, G, B, 255);
        }

        public static implicit operator SimpleColor(Microsoft.Xna.Framework.Color original)
        {
            return new SimpleColor(original);
        }
    }

    public class PlayerItem
    {
        public int NetId { get; set; }
        public int Stack { get; set; }
        public byte Prefix { get; set; }

        public PlayerItem(int netId, int stack, byte prefix)
        {
            NetId = netId;
            Stack = stack;
            Prefix = prefix;
        }
    }

    public class SlotItem : PlayerItem
    {
        public int Slot { get; set; }

        public SlotItem(int netId, int stack, byte prefix, int slot)
            : base(netId, stack, prefix)
        {
            Slot = slot;
        }
    }
}
