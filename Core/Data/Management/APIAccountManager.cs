using System;
using System.Threading.Tasks;
using TDSM.Core.Data.Models;
using System.Data.Entity;
using System.Linq;

namespace TDSM.Core.Data.Management
{
    public static class APIAccountManager
    {
        public static async Task<APIAccount> FindByName(string name)
        {
            using (var ctx = new TContext())
            {
                return await ctx.APIAccounts.FirstOrDefaultAsync(x => x.Username == name);
            }
        }

        public static async Task<APIAccountRole[]> GetRolesForAccount(int accountId)
        {
            using (var ctx = new TContext())
            {
                return await ctx.APIAccountsRoles.Where(x => x.AccountId == accountId).ToArrayAsync();
            }
        }
    }
}

