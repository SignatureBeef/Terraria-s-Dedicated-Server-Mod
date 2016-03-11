using System.Threading.Tasks;
using TDSM.Core.Data.Models;
using System.Linq;
using Microsoft.Data.Entity;

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
#else
            return null;
#endif
        }

        public static async Task<APIAccountRole[]> GetRolesForAccount(int accountId)
        {
#if ENTITY_FRAMEWORK_6 || ENTITY_FRAMEWORK_7
            using (var ctx = new TContext())
            {
                return await ctx.APIAccountsRoles.Where(x => x.AccountId == accountId).ToArrayAsync();
            }
#else
            return null;
#endif
        }
    }
}

