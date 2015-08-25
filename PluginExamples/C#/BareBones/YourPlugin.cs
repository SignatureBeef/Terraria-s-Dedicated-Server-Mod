using TDSM.API;
using TDSM.API.Command;
using TDSM.API.Plugin;

namespace BareBones
{
    public class YourPlugin : BasePlugin
    {
        public YourPlugin()
        {
            this.TDSMBuild = 1;
            this.Version = "1";
            this.Author = "TDSM";
            this.Name = "Simple name";
            this.Description = "This plugin does these awesome things!";
        }

        protected override void Initialized(object state)
        {
            AddCommand("commandname")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("My command description")
                .WithHelpText("<name>")
                .WithHelpText("<something else> <maybe more>")
                .WithPermissionNode("BareBones.commandname")
                .Calls(MyCustomCommandCallback);
        }

        void MyCustomCommandCallback(ISender sender, ArgumentList args)
        {
            //Your implementation
        }

        [Hook(HookOrder.NORMAL)]
        void MyFunctionNameThatDoesntMatter(ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
        {
            //Your implementation
        }
    }
}

