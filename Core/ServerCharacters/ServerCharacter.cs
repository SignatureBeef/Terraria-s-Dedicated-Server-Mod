using Microsoft.Xna.Framework;
using OTA.Logging;
using System;
using System.Linq;
using TDSM.Core.ServerCharacters.Models;
using Terraria;

namespace TDSM.Core.ServerCharacters
{
    public class ServerCharacter : IDisposable
    {
        public long Id { get; set; }

        public long? PlayerId { get; set; }

        public string UUID { get; set; }

        public int Health { get; set; }

        public int MaxHealth { get; set; }

        public int Mana { get; set; }

        public int MaxMana { get; set; }

        public int SpawnX { get; set; }

        public int SpawnY { get; set; }

        public int Hair { get; set; }

        public byte HairDye { get; set; }

        public bool[] HideVisual { get; set; }

        public byte Difficulty { get; set; }

        public Color HairColor { get; set; }

        public Color SkinColor { get; set; }

        public Color EyeColor { get; set; }

        public Color ShirtColor { get; set; }

        public Color UnderShirtColor { get; set; }

        public Color PantsColor { get; set; }

        public Color ShoeColor { get; set; }

        public System.Collections.Generic.List<SlotItem> Inventory { get; set; }

        public System.Collections.Generic.List<SlotItem> Dye { get; set; }

        public System.Collections.Generic.List<SlotItem> Armor { get; set; }

        public System.Collections.Generic.List<SlotItem> Equipment { get; set; }

        public System.Collections.Generic.List<SlotItem> MiscDyes { get; set; }

        public System.Collections.Generic.List<SlotItem> Bank { get; set; }

        public System.Collections.Generic.List<SlotItem> Bank2 { get; set; }

        public SlotItem Trash { get; set; }

        public int AnglerQuests { get; set; }

        public int[] Buffs { get; set; }

        public int[] BuffTime { get; set; }

        /// <summary>
        /// NEVER USE THIS - Reflection only
        /// 
        /// </summary>
        public ServerCharacter()
        {
        }

        /// <summary>
        /// Creates a new server config based off a Terrarian player instance
        /// </summary>
        /// <param name="player"></param>
        public ServerCharacter(Player player)
        {
            this.Health = player.statLife;
            this.MaxHealth = player.statLifeMax;

            this.Mana = player.statMana;
            this.MaxMana = player.statManaMax;

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

            this.AnglerQuests = player.anglerQuestsFinished;

            this.Inventory = player.inventory
                .Select((item, index) => item == null ? null : new SlotItem(item.netID, item.stack, item.prefix, item.favorited, index))
                .Where(x => x != null)
                .ToList();

            this.Dye = player.dye
                .Select((item, index) => item == null ? null : new SlotItem(item.netID, item.stack, item.prefix, item.favorited, index))
                .Where(x => x != null)
                .ToList();

            this.Armor = player.armor
                .Select((item, index) => item == null ? null : new SlotItem(item.netID, item.stack, item.prefix, item.favorited, index))
                .Where(x => x != null)
                .ToList();

            this.Equipment = player.miscEquips
                .Select((item, index) => item == null ? null : new SlotItem(item.netID, item.stack, item.prefix, item.favorited, index))
                .Where(x => x != null)
                .ToList();

            this.MiscDyes = player.miscDyes
                .Select((item, index) => item == null ? null : new SlotItem(item.netID, item.stack, item.prefix, item.favorited, index))
                .Where(x => x != null)
                .ToList();

            this.Bank = player.bank.item
                .Select((item, index) => item == null ? null : new SlotItem(item.netID, item.stack, item.prefix, item.favorited, index))
                .Where(x => x != null)
                .ToList();

            this.Bank2 = player.bank2.item
                .Select((item, index) => item == null ? null : new SlotItem(item.netID, item.stack, item.prefix, item.favorited, index))
                .Where(x => x != null)
                .ToList();

            this.Buffs = player.buffType;
            this.BuffTime = player.buffTime;

            this.Trash = new SlotItem(player.trashItem.netID, player.trashItem.stack, player.trashItem.prefix, player.trashItem.favorited, 0);

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
            this.Health = info.Health;
            this.MaxHealth = info.MaxHealth;

            this.Mana = info.Mana;
            this.MaxMana = info.MaxMana;

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

            this.AnglerQuests = player.anglerQuestsFinished;

            if (info.Inventory != null) this.Inventory = info.Inventory.ToList();
            if (info.Armor != null) this.Armor = info.Armor.ToList();
            if (info.Dye != null) this.Dye = info.Dye.ToList();
            if (info.Equipment != null) this.Equipment = info.Equipment.ToList();
            if (info.MiscDyes != null) this.MiscDyes = info.MiscDyes.ToList();
            if (info.Bank != null) this.Bank = info.Bank.ToList();
            if (info.Bank2 != null) this.Bank2 = info.Bank2.ToList();

            this.Buffs = player.buffType;
            this.BuffTime = player.buffTime;

            //            player.anglerQuestsFinished
        }

        /// <summary>
        /// Applies server config data to the player
        /// </summary>
        /// <param name="player"></param>
        public void ApplyToPlayer(Player player)
        {
            try
            {
                player.statLife = this.Health;
                player.statLifeMax = this.MaxHealth;

                player.statMana = this.Mana;
                player.statManaMax = this.MaxMana;

                player.SpawnX = this.SpawnX;
                player.SpawnY = this.SpawnY;

                player.hideVisual = this.HideVisual;
                player.hairDye = this.HairDye;

                player.hair = this.Hair;
                player.difficulty = this.Difficulty;

                player.hairColor = this.HairColor;
                player.skinColor = this.SkinColor;
                player.eyeColor = this.EyeColor;
                player.shirtColor = this.ShirtColor;
                player.underShirtColor = this.UnderShirtColor;
                player.pantsColor = this.PantsColor;
                player.shoeColor = this.ShoeColor;

                player.anglerQuestsFinished = this.AnglerQuests;

                //Trash
                player.trashItem = new Item();
                player.trashItem.name = String.Empty;
                player.trashItem.SetDefaults(0);
                if (this.Trash != null)
                {
                    player.trashItem.netDefaults(this.Trash.NetId);
                    player.trashItem.stack = this.Trash.Stack;
                    player.trashItem.Prefix(this.Trash.Prefix);
                    player.trashItem.favorited = this.Trash.Favorite;
                }
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Failed to apply player settings");
            }

            try
            {
                ApplyItems(ref player.inventory, this.Inventory);
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Failed to apply player inventory");
            }
            try
            {
                ApplyItems(ref player.armor, this.Armor);
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Failed to apply player armor");
            }
            try
            {
                ApplyItems(ref player.dye, this.Dye);
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Failed to apply player dye");
            }
            try
            {
                ApplyItems(ref player.miscEquips, this.Equipment);
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Failed to apply player equipment");
            }
            try
            {
                ApplyItems(ref player.miscDyes, this.MiscDyes);
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Failed to apply player misc dyes");
            }
            try
            {
                ApplyItems(ref player.bank.item, this.Bank);
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Failed to apply player bank items");
            }
            try
            {
                ApplyItems(ref player.bank2.item, this.Bank2);
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Failed to apply player bank (2) items");
            }

            //            try
            //            {
            //                //Reset and populate inventory
            //                //                player.inventory = Enumerable.Repeat(new Item(){ name = String.Empty }, player.inventory.Length).ToArray();
            //                player.inventory = new Item[player.inventory.Length];
            //                for (var i = 0; i < player.inventory.Length; i++)
            //                {
            //                    player.inventory[i] = new Item();
            //                    player.inventory[i].name = String.Empty;
            //                    player.inventory[i].SetDefaults(0);
            //                }
            //
            //                if (this.Inventory != null)
            //                    foreach (var slotItem in this.Inventory)
            //                    {
            //                        var item = player.inventory[slotItem.Slot];
            //
            //                        item.netDefaults(slotItem.NetId);
            //                        item.stack = slotItem.Stack;
            //                        item.Prefix(slotItem.Prefix);
            //                        item.favorited = slotItem.Favorite;
            //
            //                        player.inventory[slotItem.Slot] = item;
            //                    }
            //            }
            //            catch (Exception e)
            //            {
            //                ProgramLog.Log(e, "Failed to apply player inventory");
            //            }
            //
            //            try
            //            {
            //                //Reset and populate dye
            //                //                player.dye = Enumerable.Repeat(new Item(){ name = String.Empty }, player.dye.Length).ToArray();
            //                player.dye = new Item[player.dye.Length];
            //                for (var i = 0; i < player.dye.Length; i++)
            //                {
            //                    player.dye[i] = new Item();
            //                    player.dye[i].name = String.Empty;
            //                    player.dye[i].SetDefaults(0);
            //                }
            //                if (this.Dye != null)
            //                    foreach (var slotItem in this.Dye)
            //                    {
            //                        var item = player.dye[slotItem.Slot];
            //
            //                        item.netDefaults(slotItem.NetId);
            //                        item.stack = slotItem.Stack;
            //                        item.Prefix(slotItem.Prefix);
            //                        item.favorited = slotItem.Favorite;
            //
            //                        player.dye[slotItem.Slot] = item;
            //                    }
            //            }
            //            catch (Exception e)
            //            {
            //                ProgramLog.Log(e, "Failed to apply player dye");
            //            }
            //
            //            try
            //            {
            //                //Reset and populate armor
            //                //                player.armor = Enumerable.Repeat(new Item(){ name = String.Empty }, player.armor.Length).ToArray();
            //                player.armor = new Item[player.armor.Length];
            //                for (var i = 0; i < player.armor.Length; i++)
            //                {
            //                    player.armor[i] = new Item();
            //                    player.armor[i].name = String.Empty;
            //                    player.armor[i].SetDefaults(0);
            //                }
            //                if (this.Armor != null)
            //                    foreach (var slotItem in this.Armor)
            //                    {
            //                        var item = player.armor[slotItem.Slot];
            //
            //                        item.netDefaults(slotItem.NetId);
            //                        item.stack = slotItem.Stack;
            //                        item.Prefix(slotItem.Prefix);
            //                        item.favorited = slotItem.Favorite;
            //
            //                        player.armor[slotItem.Slot] = item;
            //                    }
            //            }
            //            catch (Exception e)
            //            {
            //                ProgramLog.Log(e, "Failed to apply player armor");
            //            }

            try
            {
                //Update client
                this.Send(player);
            }
            catch (Exception e)
            {
                ProgramLog.Log(e, "Failed to send player data");
            }

            player.SetSSCReadyForSave(true);
        }

        private void ApplyItems(ref Item[] items, System.Collections.Generic.List<SlotItem> source)
        {
            //Reset and populate
            items = new Item[items.Length];
            for (var i = 0; i < items.Length; i++)
            {
                items[i] = new Item();
                items[i].name = String.Empty;
                items[i].SetDefaults(0);
            }
            if (source != null)
                foreach (var slotItem in source)
                {
                    var item = items[slotItem.Slot];

                    item.netDefaults(slotItem.NetId);
                    item.stack = slotItem.Stack;
                    item.Prefix(slotItem.Prefix);
                    item.favorited = slotItem.Favorite;

                    items[slotItem.Slot] = item;
                }
        }

        public void Send(Player player)
        {
#if TDSMServer
            var msg = NewNetMessage.PrepareThreadInstance();
            msg.PlayerData(player.whoAmI);
            msg.BuildPlayerUpdate(player.whoAmI);
            msg.Broadcast();
#endif
            for (int i = 0; i < 59; i++)
            {

                NetMessage.SendData(5, -1, -1, player.inventory[i].name, player.whoAmI, (float)i, (float)player.inventory[i].prefix, 0, 0, 0, 0);
                NetMessage.SendData(5, player.whoAmI, -1, player.inventory[i].name, player.whoAmI, (float)i, (float)player.inventory[i].prefix, 0, 0, 0, 0);
            }
            for (int i = 0; i < player.armor.Length; i++)
            {
                NetMessage.SendData(5, -1, -1, player.armor[i].name, player.whoAmI, (float)(59 + i), (float)player.armor[i].prefix, 0, 0, 0, 0);
                NetMessage.SendData(5, player.whoAmI, -1, player.armor[i].name, player.whoAmI, (float)(59 + i), (float)player.armor[i].prefix, 0, 0, 0, 0);
            }
            for (int i = 0; i < player.dye.Length; i++)
            {
                NetMessage.SendData(5, -1, -1, player.dye[i].name, player.whoAmI, (float)(58 + player.armor.Length + 1 + i), (float)player.dye[i].prefix, 0, 0, 0, 0);
                NetMessage.SendData(5, player.whoAmI, -1, player.dye[i].name, player.whoAmI, (float)(58 + player.armor.Length + 1 + i), (float)player.dye[i].prefix, 0, 0, 0, 0);
            }
            for (int i = 0; i < player.miscEquips.Length; i++)
            {
                NetMessage.SendData(5, -1, -1, String.Empty, player.whoAmI, (float)(58 + player.armor.Length + player.dye.Length + 1 + i), (float)player.miscEquips[i].prefix, 0, 0, 0, 0);
                NetMessage.SendData(5, player.whoAmI, -1, String.Empty, player.whoAmI, (float)(58 + player.armor.Length + player.dye.Length + 1 + i), (float)player.miscEquips[i].prefix, 0, 0, 0, 0);
            }
            for (int i = 0; i < player.miscDyes.Length; i++)
            {
                NetMessage.SendData(5, -1, -1, String.Empty, player.whoAmI, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + 1 + i), (float)player.miscDyes[i].prefix, 0, 0, 0, 0);
                NetMessage.SendData(5, player.whoAmI, -1, String.Empty, player.whoAmI, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + 1 + i), (float)player.miscDyes[i].prefix, 0, 0, 0, 0);
            }
            for (int i = 0; i < player.bank.item.Length; i++)
            {
                NetMessage.SendData(5, player.whoAmI, -1, String.Empty, player.whoAmI, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + player.miscDyes.Length + 1 + i), (float)player.bank.item[i].prefix, 0, 0, 0, 0);
            }
            for (int i = 0; i < player.bank2.item.Length; i++)
            {
                NetMessage.SendData(5, -1, -1, String.Empty, player.whoAmI, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + player.miscDyes.Length + player.bank.item.Length + 1 + i), (float)player.bank2.item[i].prefix, 0, 0, 0, 0);
                NetMessage.SendData(5, player.whoAmI, -1, String.Empty, player.whoAmI, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + player.miscDyes.Length + player.bank.item.Length + 1 + i), (float)player.bank2.item[i].prefix, 0, 0, 0, 0);
            }

            //Trash
            NetMessage.SendData(5, -1, -1, String.Empty, player.whoAmI, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + player.miscDyes.Length + player.bank.item.Length + player.bank2.item.Length + 1), (float)player.trashItem.prefix, 0, 0, 0, 0);
            NetMessage.SendData(5, player.whoAmI, -1, String.Empty, player.whoAmI, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + player.miscDyes.Length + player.bank.item.Length + player.bank2.item.Length + 1), (float)player.trashItem.prefix, 0, 0, 0, 0);

            //Health
            NetMessage.SendData(16, player.whoAmI, -1, String.Empty, player.whoAmI);

            //Mana
            NetMessage.SendData(42, player.whoAmI, -1, String.Empty, player.whoAmI);

            //Quests
            NetMessage.SendData(76, -1, -1, String.Empty, player.whoAmI);

            //Update skins
            NetMessage.SendData(4, -1, player.whoAmI, player.name, player.whoAmI);

            //Buffs
            //            if (this.Buffs != null && this.BuffTime != null)
            //            {
            //                var max = Math.Min(this.Buffs.Length, this.BuffTime.Length);
            //                for (var x = 0; x < max; x++)
            //                {
            //                    if (this.Buffs[x] > 0)
            //                    {
            //                        var time = this.BuffTime[x] * 60;
            //
            //                        ProgramLog.Plugin.Log("Adding buff {0} for {1}/{2}", this.Buffs[x], time, this.BuffTime[x]);
            //
            //                        player.AddBuff(this.Buffs[x], time);
            //                        NetMessage.SendData(55, -1, -1, String.Empty, player.whoAmI, this.Buffs[x], time);
            //                        NetMessage.SendData(55, player.whoAmI, -1, String.Empty, player.whoAmI, this.Buffs[x], time);
            //                    }
            //                }
            //            }
        }

        public void Dispose()
        {
            this.Mana = 0;
            this.MaxMana = 0;

            this.Health = 0;
            this.MaxHealth = 0;

            this.SpawnX = 0;
            this.SpawnY = 0;

            this.HideVisual = null;
            this.HairDye = 0;

            this.Hair = 0;
            this.Difficulty = 0;

            this.HairColor = Color.White;
            this.SkinColor = Color.White;
            this.EyeColor = Color.White;
            this.ShirtColor = Color.White;
            this.UnderShirtColor = Color.White;
            this.PantsColor = Color.White;
            this.ShoeColor = Color.White;

            this.AnglerQuests = 0;

            if (this.Inventory != null)
            {
                this.Inventory.Clear();
                this.Inventory = null;
            }
            if (this.Dye != null)
            {
                this.Dye.Clear();
                this.Dye = null;
            }
            if (this.Armor != null)
            {
                this.Armor.Clear();
                this.Armor = null;
            }
            if (this.Equipment != null)
            {
                this.Equipment.Clear();
                this.Equipment = null;
            }
            if (this.MiscDyes != null)
            {
                this.MiscDyes.Clear();
                this.MiscDyes = null;
            }
            if (this.Bank != null)
            {
                this.Bank.Clear();
                this.Bank = null;
            }
            if (this.Bank2 != null)
            {
                this.Bank2.Clear();
                this.Bank2 = null;
            }

            this.Buffs = null;
            this.BuffTime = null;
        }
    }

    public class NewPlayerInfo
    {
        public int Mana { get; set; }

        public int MaxMana { get; set; }

        public int Health { get; set; }

        public int MaxHealth { get; set; }

        public SlotItem[] Inventory { get; set; }

        public SlotItem[] Armor { get; set; }

        public SlotItem[] Dye { get; set; }

        public SlotItem[] Equipment { get; set; }

        public SlotItem[] MiscDyes { get; set; }

        public SlotItem[] Bank { get; set; }

        public SlotItem[] Bank2 { get; set; }
    }

    public class SimpleColor
    {
        public byte R { get; set; }

        public byte G { get; set; }

        public byte B { get; set; }

        public SimpleColor()
        {
        }

        public SimpleColor(uint color)
        {
            this.R = (byte)color;
            this.G = (byte)(color >> 8);
            this.B = (byte)(color >> 16);
        }

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

        public static implicit operator SimpleColor(uint original)
        {
            return new SimpleColor(original);
        }
    }
}
