using System;
using OTA.Command;
using System.Linq;
using OTA.Data;
using TDSM.Core.Data;
using OTA.Logging;
using TDSM.Core.ServerCharacters;
using TDSM.Core.ServerCharacters.Tables;
using TDSM.Core.Command;
using TDSM.Core.Data.Permissions;
using TDSM.Core.Data.Models;
using OTA;
using OTA.Permissions;

namespace TDSM.Core
{
    public partial class Entry
    {
        public const String Setting_Mana = "SSC_Mana";
        public const String Setting_MaxMana = "SSC_MaxMana";
        public const String Setting_Health = "SSC_Health";
        public const String Setting_MaxHealth = "SSC_MaxHealth";

        [TDSMComponent(ComponentEvent.Enabled)]
        public static void SetupDatabase(Entry plugin)
        {
            #if ENTITY_FRAMEWORK_6
            Storage.IsAvailable = OTA.Data.EF6.OTAContext.HasConnection;

            if (Storage.IsAvailable) ProgramLog.Admin.Log("Entity framework has a registered connection.");
            else ProgramLog.Admin.Log("Entity framework has no registered connection.");

            //if (Storage.IsAvailable)
            //    using (var ctx = new TContext())
            //    {
            //        ctx.APIAccounts.Add(new TDSM.Core.Data.Models.APIAccount()
            //        {
            //            Username = "Test",
            //            Password = "Testing"
            //        });

            //        ctx.SaveChanges();
            //    }

            #elif !ENTITY_FRAMEWORK_7
            using (var ctx = new TContext())
            {
            ctx.Database.EnsureCreated();
            Storage.IsAvailable = true;
            }

            #endif
        }

        //protected override void DatabaseInitialising(System.Data.Entity.DbModelBuilder builder)
        //{
        //    base.DatabaseInitialising(builder);

        //    using (var dbc = new TDSM.Core.Data.TContext())
        //    {
        //        dbc.CreateModel(builder);
        //    }
        //}

        protected override void DatabaseCreated()
        {
            base.DatabaseCreated();

            using (var ctx = new TContext())
            {
                ProgramLog.Admin.Log("Creating default groups...");
                CreateDefaultGroups(ctx);
                ProgramLog.Admin.Log("Creating default SSC values...");
                DefaultLoadoutTable.PopulateDefaults(ctx, true, CharacterManager.StartingOutInfo);
            }
        }

        public void CreateDefaultGroups(TContext ctx)
        {
            var pc = OTA.Commands.CommandManager.Parser.GetTDSMCommandsForAccessLevel(AccessLevel.PLAYER);
            var ad = OTA.Commands.CommandManager.Parser.GetTDSMCommandsForAccessLevel(AccessLevel.OP);
            var op = OTA.Commands.CommandManager.Parser.GetTDSMCommandsForAccessLevel(AccessLevel.CONSOLE); //Funny how these have now changed

            CreateGroup("Guest", true, null, 255, 255, 255, pc
                    .Where(x => !String.IsNullOrEmpty(x.Node))
                    .Select(x => x.Node)
                    .Distinct()
                    .ToArray(), ctx, "[Guest] ");
            CreateGroup("Admin", false, "Guest", 240, 131, 77, ad
                    .Where(x => !String.IsNullOrEmpty(x.Node))
                    .Select(x => x.Node)
                    .Distinct()
                    .ToArray(), ctx, "[Admin] ");
            CreateGroup("Operator", false, "Admin", 77, 166, 240, op
                    .Where(x => !String.IsNullOrEmpty(x.Node))
                    .Select(x => x.Node)
                    .Distinct()
                    .ToArray(), ctx, "[OP] ");
        }

        static void CreateGroup(string name, bool guest, string parent, byte r, byte g, byte b, string[] nodes, TContext ctx,
                                string chatPrefix = null,
                                string chatSuffix = null)
        {
            var grp = new Group()
            {
                Name = name,
                ApplyToGuests = guest,
                Parent = parent,
                Chat_Red = r,
                Chat_Green = g,
                Chat_Blue = b,
                Chat_Prefix = chatPrefix,
                Chat_Suffix = chatSuffix
            };
            ctx.Groups.Add(grp);

            ctx.SaveChanges(); //Save to get the ID

            foreach (var nd in nodes)
            {
                var node = ctx.Nodes.SingleOrDefault(x => x.Node == nd && x.Permission == Permission.Permitted);
                if (node == null)
                {
                    ctx.Nodes.Add(node = new NodePermission()
                        {
                            Node = nd,
                            Permission = Permission.Permitted
                        });

                    ctx.SaveChanges();
                }

                ctx.GroupNodes.Add(new GroupNode()
                    {
                        GroupId = grp.Id,
                        NodeId = node.Id 
                    });
            }

            ctx.SaveChanges();
        }
    }
}