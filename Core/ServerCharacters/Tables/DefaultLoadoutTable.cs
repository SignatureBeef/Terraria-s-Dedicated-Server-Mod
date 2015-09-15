using System;
using OTA.Data;
using TDSM.Core.ServerCharacters;
using TDSM.Core.Data;
using TDSM.Core.Data.Models;
using System.Threading.Tasks;

namespace TDSM.Core.ServerCharacters.Tables
{
    internal class DefaultLoadoutTable
    {
        public const String TableName = "SSC_Loadout";

        public const String Setting_Mana = "SSC_Mana";
        public const String Setting_MaxMana = "SSC_MaxMana";
        public const String Setting_Health = "SSC_Health";
        public const String Setting_MaxHealth = "SSC_MaxHealth";

        public static async Task<LoadoutItem> AddItem(/*CharacterManager.ItemType type,*/ int itemId)
        {
            using (var ctx = new TContext())
            {
                var li = new LoadoutItem()
                {
                    ItemId = itemId
                };
                ctx.DefaultLoadout.Add(li);

                await ctx.SaveChangesAsync();

                return li;
            }
        }

        public static async Task PopulateDefaults(NewPlayerInfo info)
        {
            SettingsStore.Set(Setting_Health, info.Health);
            SettingsStore.Set(Setting_MaxHealth, info.MaxHealth);
            SettingsStore.Set(Setting_Mana, info.Mana);
            SettingsStore.Set(Setting_MaxMana, info.MaxMana);

            if (info.Inventory != null)
            {
                foreach (var item in info.Inventory)
                {
                    var id = await ItemTable.NewItem(CharacterManager.ItemType.Inventory, item.NetId, item.Stack, item.Prefix, item.Favorite, item.Slot);
                    await AddItem(id.Id);
                }
            }

            if (info.Armor != null)
            {
                foreach (var item in info.Armor)
                {
                    var id = await ItemTable.NewItem(CharacterManager.ItemType.Armor, item.NetId, item.Stack, item.Prefix, item.Favorite, item.Slot);
                    await AddItem(id.Id);
                }
            }

            if (info.Dye != null)
            {
                foreach (var item in info.Dye)
                {
                    var id = await ItemTable.NewItem(CharacterManager.ItemType.Dye, item.NetId, item.Stack, item.Prefix, item.Favorite, item.Slot);
                    await AddItem(id.Id);
                }
            }
        }
    }
}