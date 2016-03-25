using System.Threading.Tasks;
using TDSM.Core.Data.Models;
using System.Linq;
using OTA.Data;
using OTA.Data.Dapper.Extensions;

namespace TDSM.Core.Data.Management
{
    public static class APIAccountManager
    {
        public static async Task<APIAccount> FindByName(string name)
        {
#if ENTITY_FRAMEWORK_6 || ENTITY_FRAMEWORK_7
            using (var ctx = new TContext())
            {
                return await ctx.APIAccounts.FirstOrDefaultAsync(x => x.Username == name);
            }
#elif DAPPER
            using (var ctx = DatabaseFactory.CreateConnection())
            {
                using (var txn = ctx.BeginTransaction())
                    return await ctx.FirstOrDefaultAsync<APIAccount>(new { Username = name }, transaction: txn);
            }
#else
            return null;
#endif
        }

        public static async Task<APIAccountRole[]> GetRolesForAccount(long accountId)
        {
#if ENTITY_FRAMEWORK_6 || ENTITY_FRAMEWORK_7
            using (var ctx = new TContext())
            {
                return await ctx.APIAccountsRoles.Where(x => x.AccountId == accountId).ToArrayAsync();
            }
#elif DAPPER
            using (var ctx = DatabaseFactory.CreateConnection())
            {
                using (var txn = ctx.BeginTransaction())
                    return (await ctx.WhereAsync<APIAccountRole>(new { AccountId = accountId }, transaction: txn)).ToArray();
            }
#else
            return null;
#endif
        }
    }
}

