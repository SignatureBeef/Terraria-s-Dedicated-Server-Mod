namespace BareBones

open System
open tdsm.api.Plugin
open tdsm.api.Command

type Class1() =
    inherit BasePlugin()
    
    do
        base.TDSMBuild <- 1
        base.Version <- "1"
        base.Author <- "TDSM"
        base.Name <- "Simple name"
        base.Description <- "This plugin does these awesome things!"
        
    override this.Initialized(state) =
        do
            this.AddCommand("commandname")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithDescription("My command description")
                .WithHelpText("<name>")
                .WithHelpText("<something else> <maybe more>")
                .WithPermissionNode("BareBones.commandname")
                .Calls(new Action<ISender, ArgumentList>(this.MyCustomCommandCallback))
                |> ignore
        |> ignore
    
    member this.MyCustomCommandCallback(sender:ISender)(args:ArgumentList) =
        sender.Message("Hello from F#")

    
    [<Hook(HookOrder.NORMAL)>]
    member this.MyFunctionNameThatDoesntMatter(ctx: HookContext byref, args: HookArgs.PlayerEnteredGame byref) =
        do
            //Your implementation
            ctx.Player.SendMessage("Hello from F#", Microsoft.Xna.Framework.Color.Green)
        |> ignore
