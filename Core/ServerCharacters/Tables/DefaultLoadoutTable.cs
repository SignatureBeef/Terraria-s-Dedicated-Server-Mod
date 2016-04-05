using System;
using OTA.Data;
using TDSM.Core.ServerCharacters;
using TDSM.Core.Data;
using TDSM.Core.Data.Models;
using Dapper.Contrib.Extensions;
using System.Data;
using TDSM.Core.ServerCharacters.Models;
using Dapper;
using OTA.Data.Dapper.Mappers;
using System.Collections.Generic;

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



#elif ENTITY_FRAMEWORK_6 || ENTITY_FRAMEWORK_7 || DAPPER
    internal class DefaultLoadoutTable
    {
        public const String TableName = "SSC_Loadout";

        public const String Setting_Mana = "SSC_Mana";
        public const String Setting_MaxMana = "SSC_MaxMana";
        public const String Setting_Health = "SSC_Health";
        public const String Setting_MaxHealth = "SSC_MaxHealth";

        public static LoadoutItem AddItem(/*CharacterManager.ItemType type,*/ long itemId, IDbConnection ctx, IDbTransaction transaction)
        {
#if DAPPER
            //using (var ctx = DatabaseFactory.CreateConnection())
            //{
            var li = new LoadoutItem()
            {
                ItemId = itemId
            };

            //using (var txn = ctx.BeginTransaction())
            //{
            li.Id = ctx.Insert(li, transaction: transaction);
            //txn.Commit();
            //}
#else
            using (var ctx = new TContext())
            {
                var li = new LoadoutItem()
                {
                    ItemId = itemId
                };
                ctx.DefaultLoadout.Add(li);

                ctx.SaveChanges();
#endif

            return li;
            //}
        }

#if DAPPER
        public static void PopulateDefaults(IDbConnection ctx, IDbTransaction txn, bool save, NewPlayerInfo info)
#else
        public static void PopulateDefaults(TContext ctx, bool save, NewPlayerInfo info)
#endif
        {
            //SettingsStore.Set(Setting_Health, info.Health);
            //SettingsStore.Set(Setting_MaxHealth, info.MaxHealth);
            //SettingsStore.Set(Setting_Mana, info.Mana);
            //SettingsStore.Set(Setting_MaxMana, info.MaxMana);

            if (info.Inventory != null)
            {
                foreach (var item in info.Inventory)
                {
                    var id = ItemTable.NewItem(ctx, txn, save, ItemType.Inventory, item.NetId, item.Stack, item.Prefix, item.Favorite, item.Slot);
                    AddItem(id.Id, ctx, txn);
                }
            }

            if (info.Armor != null)
            {
                foreach (var item in info.Armor)
                {
                    var id = ItemTable.NewItem(ctx, txn, save, ItemType.Armor, item.NetId, item.Stack, item.Prefix, item.Favorite, item.Slot);
                    AddItem(id.Id, ctx, txn);
                }
            }

            if (info.Dye != null)
            {
                foreach (var item in info.Dye)
                {
                    var id = ItemTable.NewItem(ctx, txn, save, ItemType.Dye, item.NetId, item.Stack, item.Prefix, item.Favorite, item.Slot);
                    AddItem(id.Id, ctx, txn);
                }
            }
        }

        public static NewPlayerInfo LoadNewPlayerInfo()
        {
            var inventory = new List<SlotItem>();
            var armor = new List<SlotItem>();
            var dye = new List<SlotItem>();
            using (var ctx = DatabaseFactory.CreateConnection())
            {
                using (var txn = ctx.BeginTransaction())
                {
                    var res = ctx.Query($"select si.* from LoadoutItems li left join SlotItems si on li.ItemId = si.Id order by si.Slot", transaction: txn);

                    if (res != null)
                    {
                        foreach (var row in res)
                        {
                            var type = (ItemType)row.Type;
                            switch (type)
                            {
                                case ItemType.Inventory:
                                    inventory.Add(new SlotItem()
                                    {
                                        NetId = row.NetId,
                                        Stack = row.Stack,
                                        Prefix = row.Prefix,
                                        Favorite = row.Favorite,
                                        Slot = row.Slot
                                    });
                                    break;
                                case ItemType.Armor:
                                    armor.Add(new SlotItem()
                                    {
                                        NetId = row.NetId,
                                        Stack = row.Stack,
                                        Prefix = row.Prefix,
                                        Favorite = row.Favorite,
                                        Slot = row.Slot
                                    });
                                    break;
                                case ItemType.Dye:
                                    dye.Add(new SlotItem()
                                    {
                                        NetId = row.NetId,
                                        Stack = row.Stack,
                                        Prefix = row.Prefix,
                                        Favorite = row.Favorite,
                                        Slot = row.Slot
                                    });
                                    break;
                            }
                        }
                    }
                }
            }
            return new NewPlayerInfo()
            {
                Armor = armor.ToArray(),
                Inventory = inventory.ToArray(),
                Dye = dye.ToArray()
            };
        }
    }
#endif
}