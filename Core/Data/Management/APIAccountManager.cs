using System.Threading.Tasks;
using TDSM.Core.Data.Models;
using System.Linq;
using OTA.Data;
using OTA.Data.Dapper.Extensions;
using Dapper.Contrib.Extensions;
using OTA.Data.Dapper.Mappers;
using Dapper;
using System;

namespace TDSM.Core.Data.Management
{
    public static class APIAccountManager
    {
        public static APIAccount FindByName(string name)
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
                    return ctx.FirstOrDefault<APIAccount>(new { Username = name }, transaction: txn);
            }
#else
            return null;
#endif
        }
        public static async Task<APIAccount> FindByNameAsync(string name)
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

        public static APIAccount Create(string name, string password)
        {
            var acc = new APIAccount()
            {
                Username = name,
                Password = password
            };

            using (var ctx = DatabaseFactory.CreateConnection())
            {
                using (var txn = ctx.BeginTransaction())
                {
                    acc.Id = ctx.Insert(acc, txn);
                    txn.Commit();
                }
            }

            return acc;
        }

        /// <summary>
        /// Finds a list of accounts matching a prefix
        /// </summary>
        /// <returns>The account username prefix.</returns>
        /// <param name="search">Search prefix.</param>
        public static string[] FindAccountsByPrefix(string search)
        {
            using (var ctx = DatabaseFactory.CreateConnection())
            {
                using (var txn = ctx.BeginTransaction())
                    return ctx.Query<APIAccount>($"select * from {TableMapper.TypeToName<APIAccount>()} where {ColumnMapper.Enclose("Username")} like @Username", new { Username = search + '%' }, transaction: txn)
                    .Select(x => x.Username)
                    .ToArray();
            }
        }

        public static APIAccountRole AddType(long accountId, string type, string value)
        {
            var acc = new APIAccountRole()
            {
                AccountId = accountId,
                Type = type,
                Value = value,
                DateFrom = DateTime.UtcNow
            };

            using (var ctx = DatabaseFactory.CreateConnection())
            {
                using (var txn = ctx.BeginTransaction())
                {
                    acc.Id = ctx.Insert(acc, txn);
                    txn.Commit();
                }
            }

            return acc;
        }

        public static bool DeleteType(long accountId, string type, string value)
        {
            using (var ctx = DatabaseFactory.CreateConnection())
            {
                using (var txn = ctx.BeginTransaction())
                {
                    var res = ctx.Execute($"delete from {TableMapper.TypeToName<APIAccountRole>()} " +
                        $"where {ColumnMapper.Enclose("AccountId")} = @AccountId " +
                        $"and {ColumnMapper.Enclose("Type")} = @Type " +
                        $"and {ColumnMapper.Enclose("Value")} = @Value", new { AccountId = accountId, Type = type, Value = value }, transaction: txn);
                    txn.Commit();
                    return res > 0;
                }
            }
        }

        public static bool DeleteAccount(long accountId)
        {
            using (var ctx = DatabaseFactory.CreateConnection())
            {
                using (var txn = ctx.BeginTransaction())
                {
                    //Remove all roles
                    ctx.Execute($"delete from {TableMapper.TypeToName<APIAccountRole>()} " +
                       $"where {ColumnMapper.Enclose("AccountId")} = @AccountId ", new { AccountId = accountId }, transaction: txn);

                    //Remove the account
                    var res = ctx.Execute($"delete from {TableMapper.TypeToName<APIAccount>()} " +
                        $"where {ColumnMapper.Enclose("Id")} = @Id ", new { Id = accountId }, transaction: txn);
                    txn.Commit();
                    return res > 0;
                }
            }
        }
    }
}

