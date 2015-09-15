using System;
using OTA.Data;
using TDSM.Core.ServerCharacters;
using TDSM.Core.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace TDSM.Core.ServerCharacters.Tables
{
    internal class ItemTable
    {
        public static async Task<SlotItem> NewItem(CharacterManager.ItemType type, int netId, int stack, int prefix, bool favorite, int slot, int? characterId = null)
        {
            using (var ctx = new TContext())
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

                await ctx.SaveChangesAsync();

                return itm;
            }
        }

        public static SlotItem GetItem(CharacterManager.ItemType type, int slot, int? characterId = null)
        {
            using (var ctx = new TContext())
            {
                return ctx.Items.FirstOrDefault(x => x.Type == type && x.Slot == slot && x.CharacterId == characterId);
            }
        }

        public static IEnumerable<SlotItem> GetItemsForCharacter(CharacterManager.ItemType type, int? characterId = null)
        {
            using (var ctx = new TContext())
            {
                return ctx.Items.Where(x => x.Type == type && x.CharacterId == characterId);
            }
        }

        public static async Task<bool> UpdateItem(CharacterManager.ItemType type, int netId, int prefix, int stack, bool favorite, int slot, int? characterId = null)
        {
            using (var ctx = new TContext())
            {
                var existing = ctx.Items.FirstOrDefault(x => x.Type == type && x.Slot == slot && x.CharacterId == characterId);
                if (existing != null)
                {
                    existing.NetId = netId;
                    existing.Prefix = prefix;
                    existing.Stack = stack;
                    existing.Favorite = favorite;

                    await ctx.SaveChangesAsync();

                    return true;
                }
                else return false;
            }
        }
    }
}