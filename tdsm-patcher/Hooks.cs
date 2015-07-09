using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace tdsm.patcher
{
    public static class ConsoleHelper
    {
        public static void ClearLine()
        {
            var current = System.Console.CursorTop;
            System.Console.SetCursorPosition(0, System.Console.CursorTop);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(0, current);
        }
    }

    public sealed class HookAttribute : Attribute { }

    /// <summary>
    /// This file is specifically for Vanilla hooks.
    /// When Terraria is released for multi-platforms our core server class will not be of service anymore, or atleast we can reuse the Packet code from vanilla.
    /// What this class will do is expose the hooks we need for plugins. E.g. OnPlayerJoined
    /// </summary>
    public partial class Injector
    {
        public TerrariaOrganiser Terraria;
        public APIOrganiser API;

        private void InitOrganisers()
        {
            Terraria = new TerrariaOrganiser(_asm);
            API = new APIOrganiser(_self);
        }

        public void InjectHooks()
        {
            var hooks = typeof(Injector)
                .GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(HookAttribute), false).Count() == 1)
                .ToArray();

            string line = null;
            for (var x = 0; x < hooks.Length; x++)
            {
                const String Fmt = "Patching in hooks - {0}/{1}";

                if (line != null) ConsoleHelper.ClearLine();

                line = String.Format(Fmt, x + 1, hooks.Length);
                Console.Write(line);
                hooks[x].Invoke(this, null);
            }

            //Clear ready for the Ok\n
            if (line != null) ConsoleHelper.ClearLine();
            Console.Write("Patching in hooks - ");
        }

        [Hook]
        private void OnPlayerEntering()
        {
            var getData = Terraria.MessageBuffer.Methods.Single(x => x.Name == "GetData");
            var match = getData.Body.Instructions.First(x => x.Operand is String && x.Operand.Equals("Empty name."));
            var callback = API.VanillaHooks.Methods.Single(x => x.Name == "OnPlayerEntering");

            var il = getData.Body.GetILProcessor();

            //Find the second RETURN and insert our callback just before.
            int count = 0;
            while (count < 2)
            {
                match = match.Next;

                if (match.OpCode == OpCodes.Ret)
                    count++;
            }

            var playerObject = match.Previous;
            count = 0;
            while (count < 2)
            {
                playerObject = playerObject.Previous;

                if (playerObject.OpCode == OpCodes.Ldloc_S)
                    count++;
            }

            il.InsertBefore(match, il.Create(OpCodes.Ldloc, playerObject.Operand as VariableDefinition));
            il.InsertBefore(match, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        }

        [Hook]
        private void OnGreetPlayer()
        {
            var greetPlayer = Terraria.NetMessage.Methods.Single(x => x.Name == "greetPlayer");
            var callback = API.VanillaHooks.Methods.Single(x => x.Name == "OnGreetPlayer");
            var il = greetPlayer.Body.GetILProcessor();

            var first = greetPlayer.Body.Instructions.First();

            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(first, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ret));
        }

        [Hook]
        private void OnConnectionAccepted()
        {
            var oca = Terraria.Netplay.Methods.Single(x => x.Name == "OnConnectionAccepted");
            var callback = API.NetplayCallback.Methods.Single(x => x.Name == "OnNewConnection");
            var ldsfld = oca.Body.Instructions.First(x => x.OpCode == OpCodes.Ldsfld);

            var il = oca.Body.GetILProcessor();
            il.InsertBefore(ldsfld, il.Create(OpCodes.Ldloc_0));
            il.InsertBefore(ldsfld, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        }
    }

    public class TerrariaOrganiser
    {
        private AssemblyDefinition _asm;
        public TerrariaOrganiser(AssemblyDefinition assembly)
        {
            this._asm = assembly;
        }

        public TypeDefinition MessageBuffer
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "MessageBuffer"); }
        }

        public TypeDefinition NetMessage
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NetMessage"); }
        }

        public TypeDefinition Main
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Main"); }
        }

        public TypeDefinition ProgramServer
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "ProgramServer"); }
        }

        public TypeDefinition NPC
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NPC"); }
        }

        public TypeDefinition WorldFile
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "WorldFile"); }
        }

        public TypeDefinition WorldGen
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "WorldGen"); }
        }

        public TypeDefinition Netplay
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Netplay"); }
        }

        public TypeDefinition Player
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Player"); }
        }

//        public TypeDefinition ServerSock
//        {
//            get
//            { return _asm.MainModule.Types.Single(x => x.Name == "ServerSock"); }
//        }

        public TypeDefinition RemoteClient
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "RemoteClient"); }
        }

        public TypeDefinition RemoteServer
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "RemoteServer"); }
        }
    }

    public class APIOrganiser
    {
        private AssemblyDefinition _asm;
        public APIOrganiser(AssemblyDefinition assembly)
        {
            this._asm = assembly;
        }

        #region "Callbacks"
        public TypeDefinition BasePlayer
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "BasePlayer"); }
        }
        public TypeDefinition Configuration
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Configuration"); }
        }

        public TypeDefinition GameWindow
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "GameWindow"); }
        }

        public TypeDefinition IAPISocket
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "IAPISocket"); }
        }

        public TypeDefinition MainCallback
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "MainCallback"); }
        }

        public TypeDefinition MessageBufferCallback
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "MessageBufferCallback"); }
        }

        public TypeDefinition NetMessageCallback
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NetMessageCallback"); }
        }

        public TypeDefinition NetplayCallback
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NetplayCallback"); }
        }

        public TypeDefinition NPCCallback
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NPCCallback"); }
        }

        public TypeDefinition Patches
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Patches"); }
        }

        public TypeDefinition Tools
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Tools"); }
        }

        public TypeDefinition UserInput
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "UserInput"); }
        }

        public TypeDefinition WorldFileCallback
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "WorldFileCallback"); }
        }

        public TypeDefinition VanillaHooks
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "VanillaHooks"); }
        }
        #endregion

        public TypeDefinition NAT
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NAT"); }
        }

        public TypeDefinition ClientConnection
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "ClientConnection"); }
            //{ return _asm.MainModule.Types.Single(x => x.Name == "TemporarySynchSock"); }
        }
    }
}
