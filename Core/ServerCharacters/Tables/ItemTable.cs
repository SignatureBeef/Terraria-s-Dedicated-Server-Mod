using System.Linq;
using System.Collections.Generic;
using Dapper;
using Dapper.Contrib.Extensions;
using OTA.Data.Dapper.Extensions;
using System.Data;
using TDSM.Core.ServerCharacters.Models;

namespace TDSM.Core.ServerCharacters.Tables
{
#if DATA_CONNECTOR
    internal class ItemTable
    {
        public const String TableName = "SSC_Items";

        static class ColumnNames
        {
            public const String Id = "Id";
            public const String CharacterId = "CharacterId";
            public const String TypeId = "TypeId";
            public const String NetId = "NetId";
            public const String Stack = "Stack";
            public const String Prefix = "Prefix";
            public const String Slot = "Slot";
            public const String Favorite = "Favorite";
        }

        public static readonly TableColumn[] Columns = new TableColumn[]
        {
            new TableColumn(ColumnNames.Id, typeof(Int32), true, true),
            new TableColumn(ColumnNames.CharacterId, typeof(Int32), true),
            new TableColumn(ColumnNames.TypeId, typeof(Int32)),
            new TableColumn(ColumnNames.NetId, typeof(Int32)),
            new TableColumn(ColumnNames.Stack, typeof(Int32)),
            new TableColumn(ColumnNames.Prefix, typeof(Int32)),
            new TableColumn(ColumnNames.Slot, typeof(Int32)),
            new TableColumn(ColumnNames.Favorite, typeof(Boolean))
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

        public static int NewItem(CharacterManager.ItemType type, int netId, int stack, int prefix, bool favorite, int slot, int? characterId = null)
        {
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.InsertInto(TableName,
                    new DataParameter(ColumnNames.TypeId, (int)type),
                    new DataParameter(ColumnNames.NetId, netId),
                    new DataParameter(ColumnNames.Stack, stack),
                    new DataParameter(ColumnNames.Prefix, prefix),
                    new DataParameter(ColumnNames.Slot, slot),
                    new DataParameter(ColumnNames.CharacterId, characterId),
                    new DataParameter(ColumnNames.Favorite, favorite)
                );

                return (int)Storage.ExecuteInsert(bl); //Get the new ID
            }
        }

        public static int GetItem(CharacterManager.ItemType type, int slot, int? characterId = null)
        {
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.SelectFrom(TableName, new string[] { ColumnNames.Id },
                    new WhereFilter(ColumnNames.TypeId, (int)type),
                    new WhereFilter(ColumnNames.Slot, slot),
                    new WhereFilter(ColumnNames.CharacterId, characterId)
                );

                return (int)Storage.ExecuteScalar<Int32>(bl);
            }
        }

        public static SlotItem[] GetItemsForCharacter(CharacterManager.ItemType type, int? characterId = null)
        {
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.SelectFrom(TableName, new string[] { "*" },
                    new WhereFilter(ColumnNames.TypeId, (int)type),
                    new WhereFilter(ColumnNames.CharacterId, characterId)
                );

                return Storage.ExecuteArray<SlotItem>(bl);
            }
        }

        public static bool UpdateItem(CharacterManager.ItemType type, int netId, int prefix, int stack, bool favorite, int slot, int? characterId = null)
        {
            using (var bl = Storage.GetBuilder(CharacterManager.SQLSafeName))
            {
                bl.Update(TableName, new DataParameter[]
                    {
                        new DataParameter(ColumnNames.NetId, netId),
                        new DataParameter(ColumnNames.Prefix, prefix),
                        new DataParameter(ColumnNames.Stack, stack),
                        new DataParameter(ColumnNames.Favorite, favorite)
                    },
                    new WhereFilter(ColumnNames.TypeId, (int)type),
                    new WhereFilter(ColumnNames.Slot, slot),
                    new WhereFilter(ColumnNames.CharacterId, characterId)
                );

                return Storage.ExecuteNonQuery(bl) > 0;
            }
        }
    }
#elif ENTITY_FRAMEWORK_6 || ENTITY_FRAMEWORK_7
    internal class ItemTable
    {
        public static SlotItem NewItem(TContext ctx, bool save, CharacterManager.ItemType type, int netId, int stack, int prefix, bool favorite, int slot, int? characterId = null)
        {
            var itm = new SlotItem()
            {
                Type = type,
                NetId = netId,
                Stack = stack,
                Prefix = prefix,
                Favorite = favorite,
                Slot = slot,
                CharacterId = characterId
            };

            ctx.Items.Add(itm);

            if (save) ctx.SaveChanges();

            return itm;
        }

        public static SlotItem GetItem(TContext ctx, CharacterManager.ItemType type, int slot, int? characterId = null)
        {
            return ctx.Items.FirstOrDefault(x => x.Type == type && x.Slot == slot && x.CharacterId == characterId);
        }

        public static List<SlotItem> GetItemsForCharacter(TContext ctx, CharacterManager.ItemType type, int? characterId = null)
        {
            return ctx.Items.Where(x => x.Type == type && x.CharacterId == characterId).ToList();
        }

        public static bool UpdateItem(TContext ctx, bool save, CharacterManager.ItemType type, int netId, int prefix, int stack, bool favorite, int slot, int? characterId = null)
        {
            var existing = ctx.Items.FirstOrDefault(x => x.Type == type && x.Slot == slot && x.CharacterId == characterId);
            if (existing != null)
            {
                existing.NetId = netId;
                existing.Prefix = prefix;
                existing.Stack = stack;
                existing.Favorite = favorite;

                if (save) ctx.SaveChanges();

                return true;
            }
            else return false;
        }
    }
#elif DAPPER
    internal class ItemTable
    {
        public static SlotItem NewItem(IDbConnection ctx, IDbTransaction txn, bool save, ItemType type, int netId, int stack, int prefix, bool favorite, int slot, long? characterId = null)
        {
            var item = new SlotItem()
            {
                Type = type,
                NetId = netId,
                Stack = stack,
                Prefix = prefix,
                Favorite = favorite,
                Slot = slot,
                CharacterId = characterId
            };

            item.Id = ctx.Insert(item, transaction: txn);

            return item;
        }

        public static SlotItem GetItem(IDbConnection ctx, IDbTransaction txn, ItemType type, int slot, long? characterId = null)
        {
            return ctx.QueryFirstOrDefault<SlotItem>(new { Type = type, Slot = slot, CharacterId = characterId }, transaction: txn);
        }

        public static List<SlotItem> GetItemsForCharacter(IDbConnection ctx, IDbTransaction txn, ItemType type, long? characterId = null)
        {
            return ctx.Where<SlotItem>(new { Type = type, CharacterId = characterId }, transaction: txn).ToList();
        }

        public static bool UpdateItem(IDbConnection ctx, IDbTransaction txn, bool save, ItemType type, int netId, int prefix, int stack, bool favorite, int slot, long? characterId = null)
        {
            var existing = GetItem(ctx, txn, type, slot, characterId);
            if (existing != null)
            {
                existing.NetId = netId;
                existing.Prefix = prefix;
                existing.Stack = stack;
                existing.Favorite = favorite;

                ctx.Update(existing, transaction: txn);

                return true;
            }
            else return false;
        }
    }
#endif
}