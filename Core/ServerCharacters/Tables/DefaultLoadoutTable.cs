using System;
using OTA.Data;
using TDSM.Core.ServerCharacters;
using TDSM.Core.Data;
using TDSM.Core.Data.Models;

namespace TDSM.Core.ServerCharacters.Tables
{
#if DATA_CONNECTOR
    internal class DefaultLoadoutTable
    {
        public const String TableName = "SSC_Loadout";

        public const String Setting_Mana = "SSC_Mana";
        public const String Setting_MaxMana = "SSC_MaxMana";
        public const String Setting_Health = "SSC_Health";
        public const String Setting_MaxHealth = "SSC_MaxHealth";

        static class ColumnNames
        {
            public const String Id = "Id";
            //            public const String TypeId = "TypeId";
            public const String ItemId = "ItemId";
        }

        public static readonly TableColumn[] Columns = new TableColumn[]
        {
            new TableColumn(ColumnNames.Id, typeof(Int32), true, true),
//            new TableColumn(ColumnNames.TypeId, typeof(Int32)),
            new TableColumn(ColumnNames.ItemId, typeof(Int32))
        };

        public static bool Exists()
        {
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.TableExists(TableName);

                return Storage.Execute(bl);
            }
        }

        public static bool Create()
        {
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.TableCreate(TableName, Columns);

                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }

        public static int AddItem(/*CharacterManager.ItemType type,*/ int itemId)
        {
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.InsertInto(TableName,
                    //                    new DataParameter(ColumnNames.TypeId, (int)type),
                    new DataParameter(ColumnNames.ItemId, itemId)
                );

                return (int)Storage.ExecuteInsert(bl); //Get the new ID
            }
        }

        public static void PopulateDefaults(NewPlayerInfo info)
        {
            SettingsStore.Set(Setting_Health, info.Health);
            SettingsStore.Set(Setting_MaxHealth, info.MaxHealth);
            SettingsStore.Set(Setting_Mana, info.Mana);
            SettingsStore.Set(Setting_MaxMana, info.MaxMana);

            if (info.Inventory != null)
            {
                foreach (var item in info.Inventory)
                {
                    var id = ItemTable.NewItem(CharacterManager.ItemType.Inventory, item.NetId, item.Stack, item.Prefix, item.Favorite, item.Slot);
                    AddItem(id);
                }
            }

            if (info.Armor != null)
            {
                foreach (var item in info.Armor)
                {
                    var id = ItemTable.NewItem(CharacterManager.ItemType.Armor, item.NetId, item.Stack, item.Prefix, item.Favorite, item.Slot);
                    AddItem(id);
                }
            }

            if (info.Dye != null)
            {
                foreach (var item in info.Dye)
                {
                    var id = ItemTable.NewItem(CharacterManager.ItemType.Dye, item.NetId, item.Stack, item.Prefix, item.Favorite, item.Slot);
                    AddItem(id);
                }
            }
        }
    }

#elif ENTITY_FRAMEWORK_6 || ENTITY_FRAMEWORK_7
    internal class DefaultLoadoutTable
    {
        public const String TableName = "SSC_Loadout";

        public const String Setting_Mana = "SSC_Mana";
        public const String Setting_MaxMana = "SSC_MaxMana";
        public const String Setting_Health = "SSC_Health";
        public const String Setting_MaxHealth = "SSC_MaxHealth";

        public static LoadoutItem AddItem(/*CharacterManager.ItemType type,*/ int itemId)
        {
            using (var ctx = new TContext())
            {
                var li = new LoadoutItem()
                {
                    ItemId = itemId
                };
                ctx.DefaultLoadout.Add(li);

                ctx.SaveChanges();

                return li;
            }
        }

        public static void PopulateDefaults(NewPlayerInfo info)
        {
            SettingsStore.Set(Setting_Health, info.Health);
            SettingsStore.Set(Setting_MaxHealth, info.MaxHealth);
            SettingsStore.Set(Setting_Mana, info.Mana);
            SettingsStore.Set(Setting_MaxMana, info.MaxMana);

            if (info.Inventory != null)
            {
                foreach (var item in info.Inventory)
                {
                    var id = ItemTable.NewItem(CharacterManager.ItemType.Inventory, item.NetId, item.Stack, item.Prefix, item.Favorite, item.Slot);
                    AddItem(id.Id);
                }
            }

            if (info.Armor != null)
            {
                foreach (var item in info.Armor)
                {
                    var id = ItemTable.NewItem(CharacterManager.ItemType.Armor, item.NetId, item.Stack, item.Prefix, item.Favorite, item.Slot);
                    AddItem(id.Id);
                }
            }

            if (info.Dye != null)
            {
                foreach (var item in info.Dye)
                {
                    var id = ItemTable.NewItem(CharacterManager.ItemType.Dye, item.NetId, item.Stack, item.Prefix, item.Favorite, item.Slot);
                    AddItem(id.Id);
                }
            }
        }
    }
#endif
}