Imports tdsm.api
Imports tdsm.api.Command
Imports tdsm.api.Plugin

Public Class YourPlugin : Inherits BasePlugin
    Public Sub New()
        MyBase.TDSMBuild = 1
        MyBase.Version = "1"
        MyBase.Author = "TDSM"
        MyBase.Name = "Simple name"
        MyBase.Description = "This plugin does these awesome things!"
    End Sub

    Protected Overrides Sub Initialized(state As Object)
        AddCommand("commandname") _
        .WithAccessLevel(AccessLevel.PLAYER) _
        .WithDescription("My command description") _
        .WithHelpText("<name>") _
        .WithHelpText("<something else> <maybe more>") _
        .WithPermissionNode("BareBones.commandname") _
        .Calls(AddressOf MyCustomCommandCallback)
    End Sub

    Sub MyCustomCommandCallback(sender As ISender, args As ArgumentList)
        'Your implementation
    End Sub

    <Hook(HookOrder.NORMAL)> _
    Sub MyFunctionNameThatDoesntMatter(ByRef ctx As HookContext, ByRef args As HookArgs.PlayerEnteredGame)
        'Your implementation
    End Sub

End Class
