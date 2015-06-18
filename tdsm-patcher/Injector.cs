using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;

namespace tdsm.patcher
{
    public class Injector : IDisposable
    {
        private AssemblyDefinition _asm;
        private AssemblyDefinition _self;

        public AssemblyDefinition Terraria
        {
            get
            { return _asm; }
        }

        public AssemblyDefinition API
        {
            get
            { return _self; }
        }

        public Injector(string filePath, string patchFile)
        {
            Initalise(filePath, patchFile);
        }

        private void Initalise(string filePath, string patchFile)
        {
            //Load the Terraria assembly
            using (var ms = new MemoryStream())
            {
                using (var fs = File.OpenRead(filePath))
                {
                    var buff = new byte[256];
                    while (fs.Position < fs.Length)
                    {
                        var task = fs.Read(buff, 0, buff.Length);
                        ms.Write(buff, 0, task);
                    }
                }

                ms.Seek(0L, SeekOrigin.Begin);
                _asm = AssemblyDefinition.ReadAssembly(ms);
            }
            //Load the assembly to patch to
            using (var ms = new MemoryStream())
            {
                using (var fs = File.OpenRead(patchFile))
                {
                    var buff = new byte[256];
                    while (fs.Position < fs.Length)
                    {
                        var task = fs.Read(buff, 0, buff.Length);
                        ms.Write(buff, 0, task);
                    }
                    fs.Close();
                }

                ms.Seek(0L, SeekOrigin.Begin);
                _self = AssemblyDefinition.ReadAssembly(ms);
            }
        }

        /// <summary>
        /// Checks to see if the source (Terraria) binary is a supported version
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public string GetAssemblyVersion()
        {
            return _asm.CustomAttributes
                .Single(x => x.AttributeType.Name == "AssemblyFileVersionAttribute")
                .ConstructorArguments
                .First()
                .Value as string;
        }

        public void HookDedServEnd()
        {
            var main = _asm.MainModule.Types.Single(x => x.Name == "Main");
            var method = main.Methods.Single(x => x.Name == "DedServ");
            var callback = _self.MainModule.Types.Single(x => x.Name == "MainCallback");
            var replacement = callback.Methods.Single(x => x.Name == "OnProgramFinished" && x.IsStatic);

            var imported = _asm.MainModule.Import(replacement);
            var il = method.Body.GetILProcessor();

            il.InsertBefore(method.Body.Instructions.Last(), il.Create(OpCodes.Call, imported));
        }

        public void HookConfig()
        {
            var serv = _asm.MainModule.Types.Single(x => x.Name == "ProgramServer");
            var main = serv.Methods.Single(x => x.Name == "Main" && x.IsStatic);
            var ourClass = _self.MainModule.Types.Single(x => x.Name == "Configuration");
            var replacement = ourClass.Methods.Single(x => x.Name == "Load" && x.IsStatic);

            //Grab all occurances of "LoadDedConfig" and route it to ours
            var toBeReplaced = main.Body.Instructions
                .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Callvirt
                    && x.Operand is MethodReference
                    && (x.Operand as MethodReference).Name == "LoadDedConfig"
                )
                .ToArray();

            for (var x = 0; x < toBeReplaced.Length; x++)
            {
                toBeReplaced[x].OpCode = OpCodes.Call;
                toBeReplaced[x].Operand = _asm.MainModule.Import(replacement);
            }
            var il = main.Body.GetILProcessor();
            for (var x = toBeReplaced.Length - 1; x > -1; x--)
            {
                il.Remove(toBeReplaced[x].Previous.Previous.Previous.Previous);
            }
        }

        public void HookInvasions()
        {
            var serv = _asm.MainModule.Types.Single(x => x.Name == "NPC");
            var main = serv.Methods.Single(x => x.Name == "SpawnNPC" && x.IsStatic);

            var il = main.Body.GetILProcessor();
            var callback = _self.MainModule.Types
                .Single(x => x.Name == "NPCCallback")
                .Methods.Single(x => x.Name == "OnInvasionNPCSpawn");

            var ins = main.Body.Instructions.Where(x =>
                x.OpCode == OpCodes.Ldsfld
                && x.Operand is FieldReference
                && (x.Operand as FieldReference).Name == "invasionType").ToArray()[1];


            /*ldloc.2
		IL_14d1: ldc.i4.s 16
		IL_14d3: mul
		IL_14d4: ldc.i4.8
		IL_14d5: add
		IL_14d6: ldloc.3
		IL_14d7: ldc.i4.s 16
		IL_14d9: mul*/
            il.InsertBefore(ins, il.Create(OpCodes.Ldloc_2));
            il.InsertBefore(ins, il.Create(OpCodes.Ldc_I4, 16));
            il.InsertBefore(ins, il.Create(OpCodes.Mul));
            il.InsertBefore(ins, il.Create(OpCodes.Ldc_I4_8));
            il.InsertBefore(ins, il.Create(OpCodes.Add));
            il.InsertBefore(ins, il.Create(OpCodes.Ldloc_3));
            il.InsertBefore(ins, il.Create(OpCodes.Ldc_I4, 16));
            il.InsertBefore(ins, il.Create(OpCodes.Mul));
            il.InsertBefore(ins, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        }

        public void FixStatusTexts()
        {
            var serv = _asm.MainModule.Types.Single(x => x.Name == "WorldFile");
            var main = serv.Methods.Single(x => x.Name == "saveWorld" && x.IsStatic);

            var il = main.Body.GetILProcessor();
            var statusText = _asm.MainModule.Types.Single(x => x.Name == "Main").Fields.Single(x => x.Name == "statusText");

            var ins = main.Body.Instructions.Where(x => x.OpCode == OpCodes.Leave_S).Last();

            il.InsertBefore(ins, il.Create(OpCodes.Ldstr, ""));
            il.InsertBefore(ins, il.Create(OpCodes.Stsfld, statusText));
        }

        public void HookWorldFile_DEBUG()
        {
            var serv = _asm.MainModule.Types.Single(x => x.Name == "WorldGen");
            var main = serv.Methods.Single(x => x.Name == "serverLoadWorldCallBack" && x.IsStatic);

            var ourClass = _self.MainModule.Types.Single(x => x.Name == "WorldFileCallback");
            var replacement = ourClass.Methods.Single(x => x.Name == "loadWorld" && x.IsStatic);

            var toBeReplaced = main.Body.Instructions
                .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Call
                    && x.Operand is MethodReference
                    && (x.Operand as MethodReference).Name == "loadWorld"
                )
                .ToArray();

            for (var x = 0; x < toBeReplaced.Length; x++)
            {
                toBeReplaced[x].Operand = _asm.MainModule.Import(replacement);
            }

            //            lastMaxTilesX

            var fld = serv.Fields.Single(x => x.Name == "lastMaxTilesX");
            fld.IsPrivate = false;
            fld.IsFamily = false;
            fld.IsPublic = true;

            fld = serv.Fields.Single(x => x.Name == "lastMaxTilesY");
            fld.IsPrivate = false;
            fld.IsFamily = false;
            fld.IsPublic = true;
        }

        public void HookStatusText()
        {
            var main = _asm.MainModule.Types.Single(x => x.Name == "Main");
            var dedServ = main.Methods.Single(x => x.Name == "DedServ");

            var selfType = _self.MainModule.Types.Single(x => x.Name == "MainCallback");
            var callback = selfType.Methods.Single(x => x.Name == "OnStatusTextChange");

            var startInstructions = dedServ.Body.Instructions
                .Where(x => x.OpCode == OpCodes.Ldsfld && x.Operand is FieldReference && (x.Operand as FieldReference).Name == "oldStatusText")
                .Reverse() //Remove desc
                .ToArray();

            var il = dedServ.Body.GetILProcessor();
            foreach (var ins in startInstructions)
            {
                var end = ins.Operand as Instruction;
                var ix = il.Body.Instructions.IndexOf(ins);

                var inLoop = il.Body.Instructions[ix].Previous.OpCode == OpCodes.Br_S;

                while (!(il.Body.Instructions[ix].OpCode == OpCodes.Call && il.Body.Instructions[ix].Operand is MethodReference && ((MethodReference)il.Body.Instructions[ix].Operand).Name == "WriteLine"))
                {
                    il.Remove(il.Body.Instructions[ix]);
                }
                il.Remove(il.Body.Instructions[ix]); //Remove the Console.WriteLine

                var insCallback = il.Create(OpCodes.Call, _asm.MainModule.Import(callback));
                il.InsertBefore(il.Body.Instructions[ix], insCallback);

                //Fix the loop back to the start
                if (inLoop && il.Body.Instructions[ix + 2].OpCode == OpCodes.Brfalse_S)
                {
                    il.Body.Instructions[ix + 2].Operand = insCallback;
                }
            }
        }

        public void HookNetMessage()
        {
            var server = _asm.MainModule.Types.Single(x => x.Name == "NetMessage");
            var method = server.Methods.Single(x => x.Name == "SendData");

            var userInputClass = _self.MainModule.Types.Single(x => x.Name == "NetMessageCallback");
            var callback = userInputClass.Methods.First(m => m.Name == "SendData");

            var il = method.Body.GetILProcessor();

            var ret = il.Create(OpCodes.Ret);
            var call = il.Create(OpCodes.Call, _asm.MainModule.Import(callback));
            var first = method.Body.Instructions.First();

            il.InsertBefore(first, il.Create(OpCodes.Nop));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_1));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_2));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_3));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, method.Parameters[4]));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, method.Parameters[5]));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, method.Parameters[6]));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, method.Parameters[7]));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, method.Parameters[8]));
            il.InsertBefore(first, call);
            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, ret);
        }

        public void HookConsoleTitle()
        {
            var cls = _asm.MainModule.Types.Single(x => x.Name == "Main");
            var method = cls.Methods.Single(x => x.Name == "DedServ");

            var cbc = _self.MainModule.Types.Single(x => x.Name == "GameWindow");
            var callback = cbc.Methods.First(m => m.Name == "SetTitle");

            var il = method.Body.GetILProcessor();

            var replacement = _asm.MainModule.Import(callback);
            foreach (var ins in method.Body.Instructions
                .Where(x => x.OpCode == OpCodes.Call
                    && x.Operand is MethodReference
                    && (x.Operand as MethodReference).DeclaringType.FullName == "System.Console"
                    && (x.Operand as MethodReference).Name == "set_Title"))
            {
                ins.Operand = replacement;
            }
        }

        public void HookProgramStart()
        {
            var server = _asm.MainModule.Types.Single(x => x.Name == "ProgramServer");
            var method = server.Methods.Single(x => x.Name == "Main");

            var userInputClass = _self.MainModule.Types.Single(x => x.Name == "MainCallback");
            var callback = userInputClass.Methods.First(m => m.Name == "OnProgramStarted");

            var il = method.Body.GetILProcessor();

            var ret = il.Create(OpCodes.Ret);
            var call = il.Create(OpCodes.Call, _asm.MainModule.Import(callback));
            var first = method.Body.Instructions.First();

            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(first, call);
            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, ret);
        }

        public void HookUpdateServer()
        {
            var server = _asm.MainModule.Types.Single(x => x.Name == "Main");
            var method = server.Methods.Single(x => x.Name == "UpdateServer");

            var userInputClass = _self.MainModule.Types.Single(x => x.Name == "MainCallback");
            var callback = userInputClass.Methods.First(m => m.Name == "UpdateServerEnd");

            var il = method.Body.GetILProcessor();
            il.InsertBefore(method.Body.Instructions.Last(), il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        }

        public void HookInitialise()
        {
            var server = _asm.MainModule.Types.Single(x => x.Name == "Netplay");
            var method = server.Methods.Single(x => x.Name == "Init");

            var userInputClass = _self.MainModule.Types.Single(x => x.Name == "MainCallback");
            var callback = userInputClass.Methods.First(m => m.Name == "Initialise");

            var il = method.Body.GetILProcessor();
            var first = method.Body.Instructions.First();
            //il.InsertBefore(method.Body.Instructions.First(), il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));


            il.InsertBefore(first, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));

            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ret));
        }

        public void HookWorldEvents()
        {
            var worldGen = _asm.MainModule.Types.Single(x => x.Name == "WorldGen");
            var method = worldGen.Methods.Single(x => x.Name == "generateWorld");

            var userInputClass = _self.MainModule.Types.Single(x => x.Name == "MainCallback");
            var callbackBegin = userInputClass.Methods.First(m => m.Name == "WorldGenerateBegin");
            var callbackEnd = userInputClass.Methods.First(m => m.Name == "WorldGenerateEnd");

            var il = method.Body.GetILProcessor();
            il.InsertBefore(method.Body.Instructions.First(), il.Create(OpCodes.Call, _asm.MainModule.Import(callbackBegin)));
            il.InsertBefore(method.Body.Instructions.Last(), il.Create(OpCodes.Call, _asm.MainModule.Import(callbackEnd)));

            var worldFile = _asm.MainModule.Types.Single(x => x.Name == "WorldFile");
            method = worldFile.Methods.Single(x => x.Name == "loadWorld");

            callbackBegin = userInputClass.Methods.First(m => m.Name == "WorldLoadBegin");
            callbackEnd = userInputClass.Methods.First(m => m.Name == "WorldLoadEnd");

            il = method.Body.GetILProcessor();
            il.InsertBefore(method.Body.Instructions.First(), il.Create(OpCodes.Call, _asm.MainModule.Import(callbackBegin)));

            var old = method.Body.Instructions.Last();
            var newI = il.Create(OpCodes.Call, _asm.MainModule.Import(callbackEnd));

            for (var x = 0; x < method.Body.Instructions.Count; x++)
            {
                var ins = method.Body.Instructions[x];
                if (ins.OpCode == OpCodes.Call && ins.Operand is MethodReference)
                {
                    var mref = ins.Operand as MethodReference;
                    if (mref.Name == "setFireFlyChance")
                    {
                        il.InsertAfter(ins, newI);
                        break;
                    }
                }
            }
            //TODO work out why it crashes when you replace Ret with Ret
        }

        public void PatchServer()
        {
            var netplay = _asm.MainModule.Types.Single(x => x.Name == "Netplay");
            var method = netplay.Methods.Single(x => x.Name == "StartServer");

            var userInputClass = _self.MainModule.Types.Single(x => x.Name == "NetplayCallback");
            var callback = userInputClass.Methods.First(m => m.Name == "StartServer");

            var ins = method.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldftn);
            ins.Operand = _asm.MainModule.Import(callback);

            //Make the Player inherit our defaults
            var player = _asm.MainModule.Types.Single(x => x.Name == "Player");
            var baseType = _self.MainModule.Types.Single(x => x.Name == "BasePlayer");
            //var interfaceType = _self.MainModule.Types.Single(x => x.Name == "ISender");

            player.BaseType = _asm.MainModule.Import(baseType);

            //Make the UpdateServer function public
            var main = _asm.MainModule.Types.Single(x => x.Name == "Main");
            var us = main.Methods.Single(x => x.Name == "UpdateServer");
            us.IsPrivate = false;
            us.IsPublic = true;

            //Map ServerSock.CheckSection to our own
            var repl = _asm.MainModule.Types
                .SelectMany(x => x.Methods)
                .Where(x => x.HasBody)
                .SelectMany(x => x.Body.Instructions)
                .Where(x => x.OpCode == OpCodes.Call && x.Operand is MethodReference && (x.Operand as MethodReference).Name == "CheckSection")
                .ToArray();
            callback = userInputClass.Methods.First(m => m.Name == "CheckSection");
            var mref = _asm.MainModule.Import(callback);
            foreach (var inst in repl)
            {
                inst.Operand = mref;
            }
        }

        public void FixNetplay()
        {
            const String NATGuid = "AE1E00AA-3FD5-403C-8A27-2BBDC30CD0E1";
            var netplay = _asm.MainModule.Types.Single(x => x.Name == "Netplay");
            var staticConstructor = netplay.Methods.Single(x => x.Name == ".cctor");

            var il = staticConstructor.Body.GetILProcessor();
            var counting = 0;
            for (var x = 0; x < staticConstructor.Body.Instructions.Count; x++)
            {
                var ins = staticConstructor.Body.Instructions[x];
                if (ins.OpCode == OpCodes.Ldstr && ins.Operand is String && ins.Operand as String == NATGuid)
                {
                    counting = 9;
                }

                if (counting-- > 0)
                {
                    il.Remove(ins);
                    x--;
                }
            }

            var fl = netplay.Fields.SingleOrDefault(x => x.Name == "upnpnat");
            if (fl != null)
                netplay.Fields.Remove(fl);

            //Clear open and close methods, add reference to the APIs
            var cb = netplay.Methods.Single(x => x.Name == "openPort");
            //    .Body;
            //cb.InitLocals = false;
            //cb.Variables.Clear();
            //cb.Instructions.Clear();
            netplay.Methods.Remove(cb);
            //cb.Instructions.Add(cb.GetILProcessor().Create(OpCodes.Nop));
            //cb.Instructions.Add(cb.GetILProcessor().Create(OpCodes.Ret));

            var close = netplay.Methods.Single(x => x.Name == "closePort");
            //    .Body;
            //close.InitLocals = false;
            //close.Variables.Clear();
            //close.Instructions.Clear();
            //close.Instructions.Add(cb.GetILProcessor().Create(OpCodes.Nop));
            //close.Instructions.Add(cb.GetILProcessor().Create(OpCodes.Ret));
            netplay.Methods.Remove(close);

            fl = netplay.Fields.SingleOrDefault(x => x.Name == "mappings");
            if (fl != null)
                netplay.Fields.Remove(fl);

            //use our uPNP (when using native terraria server)
            var natClass = _self.MainModule.Types.Single(x => x.Name == "NAT");
            var openCallback = natClass.Methods.First(m => m.Name == "OpenPort");
            var closeCallback = natClass.Methods.First(m => m.Name == "ClosePort");

            var serverLoop = netplay.Methods.Single(x => x.Name == "ServerLoop");

            foreach (var ins in serverLoop.Body.Instructions
                .Where(x => x.OpCode == OpCodes.Call
                    && x.Operand is MethodReference
                    && new string[] { "openPort", "closePort" }.Contains((x.Operand as MethodReference).Name))
                )
            {
                var mr = ins.Operand as MemberReference;
                if (mr.Name == "closePort")
                {
                    ins.Operand = _asm.MainModule.Import(closeCallback);
                }
                else
                {
                    ins.Operand = _asm.MainModule.Import(openCallback);
                }
            }
        }

        public void FixEntryPoint()
        {
            var netplay = _asm.MainModule.Types.Single(x => x.Name == "ProgramServer");
            var staticConstructor = netplay.Methods.Single(x => x.Name == "Main");

            var il = staticConstructor.Body.GetILProcessor();
            var counting = 0;
            for (var x = 0; x < staticConstructor.Body.Instructions.Count; x++)
            {
                var ins = staticConstructor.Body.Instructions[x];
                if (ins.OpCode == OpCodes.Call && ins.Operand is MethodReference && (ins.Operand as MethodReference).Name == "GetCurrentProcess")
                {
                    counting = 5;
                }

                if (counting-- > 0)
                {
                    il.Remove(ins);
                    x--;
                }
            }
        }

        public void FixSavePath()
        {
            var netplay = _asm.MainModule.Types.Single(x => x.Name == "Main");
            var staticConstructor = netplay.Methods.Single(x => x.Name == ".cctor");

            var il = staticConstructor.Body.GetILProcessor();
            var removing = false;
            for (var x = 0; x < staticConstructor.Body.Instructions.Count; x++)
            {
                var ins = staticConstructor.Body.Instructions[x];
                if (ins.OpCode == OpCodes.Call && ins.Operand is MethodReference && (ins.Operand as MethodReference).Name == "GetFolderPath")
                {
                    //Remove parameters for this
                    for (var y = 0; y < 8; y++)
                    {
                        il.Remove(staticConstructor.Body.Instructions[x - 1]);
                        x--;
                    }

                    removing = true;
                }

                if (ins.OpCode == OpCodes.Stsfld && ins.Operand is FieldDefinition && (ins.Operand as FieldDefinition).Name == "SavePath")
                {
                    //Insert the new value
                    var patches = _self.MainModule.Types.Single(k => k.Name == "Patches");
                    var dir = _asm.MainModule.Import(patches.Methods.Single(k => k.Name == "GetCurrentDirectory"));

                    il.InsertBefore(ins, il.Create(OpCodes.Call, dir));
                    removing = false;
                    return;
                }

                if (removing)
                {
                    il.Remove(ins);
                    x--;
                }
            }
        }

        public void SkipMenu()
        {
            var main = _asm.MainModule.Types.Single(x => x.Name == "Main");

            var initialise = main.Methods.Single(x => x.Name == "Initialize");
            var loc = initialise.Body.Instructions
                .Where(x => x.OpCode == OpCodes.Ldsfld && x.Operand is FieldDefinition)
                //.Select(x => x.Operand as FieldDefinition)
                .Single(x => (x.Operand as FieldDefinition).Name == "skipMenu");
            var il = initialise.Body.GetILProcessor();
            il.InsertBefore(loc, il.Create(OpCodes.Ret));
        }

        /// <summary>
        /// Adds our command line hook so we get input control from the admin
        /// </summary>
        public void PatchCommandLine()
        {
            var t_mainClass = _asm.MainModule.Types.Single(x => x.Name == "Main");

            //Simply switch to ours
            var serv = t_mainClass.Methods.Single(x => x.Name == "DedServ");

            var userInputClass = _self.MainModule.Types.Single(x => x.Name == "UserInput");
            var callback = userInputClass.Methods.First(m => m.Name == "ListenForCommands");

            var ins = serv.Body.Instructions
                .Single(x => x.OpCode == OpCodes.Call && x.Operand is MethodReference && (x.Operand as MethodReference).Name == "startDedInput");
            ins.Operand = _asm.MainModule.Import(callback);

            var ignore = new string[] {
				"Terraria.Main.DedServ"
			};

            //Patch Console.WriteLines
            var cwi = _asm.MainModule.Types
                .SelectMany(x => x.Methods)
                .Where(x => x.HasBody && x.Body.Instructions.Count > 0 && !ignore.Contains(x.DeclaringType.FullName + "." + x.Name))
                .SelectMany(x => x.Body.Instructions)
                .Where(x => x.OpCode == OpCodes.Call && x.Operand is MethodReference
                    && (x.Operand as MethodReference).Name == "WriteLine"
                    && (x.Operand as MethodReference).DeclaringType.FullName == "System.Console")
                .ToArray();

            var tools = _self.MainModule.Types.Single(x => x.Name == "Tools");
            foreach (var oci in cwi)
            {
                var mr = oci.Operand as MethodReference;
                var writeline = tools.Methods.First(m => m.Name == "WriteLine"
                    && CompareParameters(m.Parameters, mr.Parameters));
                oci.Operand = _asm.MainModule.Import(writeline);
            }
        }

        static bool CompareParameters(Mono.Collections.Generic.Collection<ParameterDefinition> a, Mono.Collections.Generic.Collection<ParameterDefinition> b)
        {
            if (a.Count == b.Count)
            {

                for (var x = 0; x < a.Count; x++)
                {
                    if (a[x].ParameterType.FullName != b[x].ParameterType.FullName) return false;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Makes the types public.
        /// </summary>
        /// <param name="server">If set to <c>true</c> server.</param>
        public void MakeTypesPublic(bool server)
        {
            var types = _asm.MainModule.Types
                .Where(x => x.IsPublic == false)
                .ToArray();

            for (var x = 0; x < types.Length; x++)
                types[x].IsPublic = true;

            var sd = _asm.MainModule.Types.Where(x => x.Name == "WorldGen")
                .SelectMany(x => x.Fields)
                .Where(x => x.Name == "stopDrops")
                .Select(x => x)
                .First();
            sd.IsPrivate = false;
            sd.IsPublic = true;

            if (server)
            {
                sd = _asm.MainModule.Types.Where(x => x.Name == "ProgramServer")
                    .SelectMany(x => x.Fields)
                    .Where(x => x.Name == "Game")
                    .Select(x => x)
                    .First();
                sd.IsPrivate = false;
                sd.IsPublic = true;

                var main = _asm.MainModule.Types.Where(x => x.Name == "Main")
                    .SelectMany(x => x.Methods)
                    .Where(x => x.Name == "Update")
                    .Select(x => x)
                    .First();
                main.IsFamily = false;
                main.IsPublic = true;
            }
        }

        /// <summary>
        /// Changes the tile class to a structure for less over head.
        /// </summary>
        public void ChangeTileToStruct()
        {
            var tileClass = _asm.MainModule.Types.Single(x => x.Name == "Tile");
            var refClass = _self.MainModule.Types.Single(x => x.Name == "TileData");

            var userInput = _self.MainModule.Types.Single(x => x.Name == "UserInput");
            var DefaultTile = userInput.Fields.Single(x => x.Name == "DefaultTile");

            //tileClass.BaseType = refClass.BaseType;
            //tileClass.IsSequentialLayout = true;

            //Update nulls to defaults
            var mainClass = _asm.MainModule.Types.Single(x => x.Name == "Main");

            var defaultTile = _asm.MainModule.Import(DefaultTile);

            //////Change to struct
            tileClass.BaseType = refClass.BaseType;
            tileClass.IsSequentialLayout = true;

            //Replace != null
            var mth = _asm.MainModule.Types
                .SelectMany(x => x.Methods

                    .Where(k => k.Body != null && k.Body.GetILProcessor().Body != null && k.Body.GetILProcessor().Body.Instructions.Where(i => i.OpCode == OpCodes.Ldsfld
                        && ((FieldReference)i.Operand).DeclaringType.FullName == mainClass.FullName
                        && ((FieldReference)i.Operand).Name == "tile"
                        && i.Next.Next.Next.OpCode == OpCodes.Call).Count() > 0)
                )
                .ToArray();
            foreach (var item in mth)
            {
                try
                {
                    var proc = item.Body.GetILProcessor();
                    var iuns = proc.Body.Instructions
                            .Where(k => k.OpCode == OpCodes.Ldsfld
                                && ((FieldReference)k.Operand).DeclaringType.FullName == mainClass.FullName
                                && ((FieldReference)k.Operand).Name == "tile"
                                && k.Next.Next.Next.OpCode == OpCodes.Call)
                            .ToArray();
                    var itm = iuns[0].Next.Next.Next;
                    proc.InsertAfter(itm, proc.Create(OpCodes.Ceq));
                    proc.InsertAfter(itm, proc.Create(OpCodes.Ldsfld, defaultTile));
                }
                catch { }
            }

            //Replace = null
            var setToNull = _asm.MainModule.Types
                .SelectMany(x => x.Methods

                    .Where(k => k.Body != null && k.Body.GetILProcessor().Body != null && k.Body.GetILProcessor().Body.Instructions.Where(i => i.OpCode == OpCodes.Ldsfld
                        && ((FieldReference)i.Operand).DeclaringType.FullName == mainClass.FullName
                        && ((FieldReference)i.Operand).Name == "tile"
                        && i.OpCode == OpCodes.Ldsfld
                        && i.Next.Next.Next.OpCode == OpCodes.Ldnull).Count() > 0)
                )
                .ToArray();
            foreach (var item in setToNull)
            {
                try
                {
                    var proc = item.Body.GetILProcessor();
                    var iuns = proc.Body.Instructions
                            .Where(k => k.OpCode == OpCodes.Ldsfld
                                && ((FieldReference)k.Operand).DeclaringType.FullName == mainClass.FullName
                                && ((FieldReference)k.Operand).Name == "tile"
                                && k.Next.Next.Next.OpCode == OpCodes.Ldnull)
                            .ToArray();
                    var itm = iuns[0].Next.Next.Next;
                    proc.InsertBefore(itm, proc.Create(OpCodes.Ldsfld, defaultTile));
                    proc.Remove(itm);
                }
                catch { }
            }



            var tl = _asm.MainModule.Types.Single(x => x.Name == "Tile");
            MethodDefinition opInequality, opEquality;
            //Add operators that call a static API function for comparisions


            //Do == operator
            var boolType = _asm.MainModule.Import(typeof(Boolean));
            var ui = _self.MainModule.Types.Single(x => x.Name == "UserInput");
            var method = new MethodDefinition("op_Equality",
                                                  MethodAttributes.Public |
                                                  MethodAttributes.Static |
                                                  MethodAttributes.HideBySig |
                                                  MethodAttributes.SpecialName, boolType);

            method.Parameters.Add(new ParameterDefinition("t1", ParameterAttributes.None, tl));
            method.Parameters.Add(new ParameterDefinition("t2", ParameterAttributes.None, tl));


            var callback = ui.Methods.Single(x => x.Name == "Tile_Equality");

            var il = method.Body.GetILProcessor();

            method.Body.Instructions.Add(il.Create(OpCodes.Nop));
            method.Body.Instructions.Add(il.Create(OpCodes.Ldarg_0));
            method.Body.Instructions.Add(il.Create(OpCodes.Ldarg_1));
            method.Body.Instructions.Add(il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
            method.Body.Instructions.Add(il.Create(OpCodes.Stloc_0));

            var val = il.Create(OpCodes.Ldloc_0);
            method.Body.Instructions.Add(val);
            method.Body.Instructions.Add(il.Create(OpCodes.Ret));

            var br = il.Create(OpCodes.Br, val);
            il.InsertBefore(val, br);

            //We're storing one local variable
            method.Body.Variables.Add(new VariableDefinition(boolType));

            opEquality = method;
            tl.Methods.Add(method);

            //Do != operator
            method = new MethodDefinition("op_Inequality",
                                                  MethodAttributes.Public |
                                                  MethodAttributes.Static |
                                                  MethodAttributes.HideBySig |
                                                  MethodAttributes.SpecialName, boolType);

            method.Parameters.Add(new ParameterDefinition("t1", ParameterAttributes.None, tl));
            method.Parameters.Add(new ParameterDefinition("t2", ParameterAttributes.None, tl));


            callback = ui.Methods.Single(x => x.Name == "Tile_Inequality");

            il = method.Body.GetILProcessor();

            method.Body.Instructions.Add(il.Create(OpCodes.Nop));
            method.Body.Instructions.Add(il.Create(OpCodes.Ldarg_0));
            method.Body.Instructions.Add(il.Create(OpCodes.Ldarg_1));
            method.Body.Instructions.Add(il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
            method.Body.Instructions.Add(il.Create(OpCodes.Stloc_0));

            val = il.Create(OpCodes.Ldloc_0);
            method.Body.Instructions.Add(val);
            method.Body.Instructions.Add(il.Create(OpCodes.Ret));

            br = il.Create(OpCodes.Br, val);
            il.InsertBefore(val, br);

            //We're storing one local variable
            method.Body.Variables.Add(new VariableDefinition(boolType));

            opInequality = method;
            tl.Methods.Add(method);

            //Now to change how tiles are accessed.
            //Change to by-reference when creating a new tile for the tile array
            //Replace callvirt with call for each method of tile
            //br.s should be replaced with br after instanciation
            //ceq (and other comparitors) must now be replaced with call, and to the appropriate operator
            //Add nop's

            //By ref.
            //            var byRef = new TypeReference(tl.Namespace, tl.Name, _asm.MainModule, tl.Scope, true);
            //            var byRef = new ByReferenceType(new TypeSpecification(tl)
            //                                            {
            //
            //            });

            var mda = new ArrayType(tl);
            mda.Dimensions.Clear();
            mda.Dimensions.Insert(0, new ArrayDimension(0, null));
            mda.Dimensions.Insert(0, new ArrayDimension(0, null));
            //            mda.Dimensions.Add(new ArrayDimension(0,0));
            var byRef = mda;

            foreach (var mtd in _asm.MainModule.Types
                     .SelectMany(x => x.Methods)
                     .Where(y => y.Body != null && y.Body.Instructions.Where(z =>
                                                  z.OpCode == OpCodes.Call
                                                  && z.Operand is MethodReference
                                                  && (z.Operand as MethodReference).DeclaringType.FullName.Contains("Terraria.Tile")
                                                  ).Count() > 0))
            {
                var instructions = mtd.Body.Instructions.Where(z =>
                                                             z.OpCode == OpCodes.Call
                                                             && z.Operand is MethodReference

                                                               && (z.Operand as MethodReference).DeclaringType.FullName.Contains("Terraria.Tile")).ToArray();
                var mil = mtd.Body.GetILProcessor();
                foreach (var ins in instructions)
                {
                    var mref = (ins.Operand as MethodReference);
                    mref.DeclaringType = byRef;

                    if (mref.Name == "Get" && ins.Next.Next.OpCode == OpCodes.Ceq && ins.Next.Next.Next.OpCode == OpCodes.Brtrue_S)
                    {
                        //                        ins.Next.Next = mil.Create(OpCodes.Call, opInequality);
                        //                        ins.Next.Next.Next = mil.Create(OpCodes.Brfalse, ins.Next.Next.Next.Operand as Instruction);
                        mil.Replace(ins.Next.Next, mil.Create(OpCodes.Call, opInequality));
                        mil.Replace(ins.Next.Next.Next, mil.Create(OpCodes.Brfalse, ins.Next.Next.Next.Operand as Instruction));
                    }
                }
            }

            //Section 2 for inequality
            foreach (var mtd in _asm.MainModule.Types
                     .SelectMany(x => x.Methods)
                     .Where(y => y.Body != null && y.Body.Instructions.Where(z =>
                                                                    z.OpCode == OpCodes.Newobj
                                                                    && z.Operand is MethodReference
                                                                    && (z.Operand as MethodReference).DeclaringType.FullName == ("Terraria.Tile")
                                                                    ).Count() > 0))
            {
                var instructions = mtd.Body.Instructions.Where(z =>
                                                               z.OpCode == OpCodes.Newobj
                                                               && z.Operand is MethodReference
                                                               && (z.Operand as MethodReference).DeclaringType.FullName == ("Terraria.Tile")
                                                               ).ToArray();
                var mil = mtd.Body.GetILProcessor();
                foreach (var ins in instructions)
                {

                }
            }
        }

        /// <summary>
        /// Removes the references to the XNA binaries, and replaces them with dummies.
        /// </summary>
        public void PatchXNA(bool server)
        {
            var xnaFramework = _asm.MainModule.AssemblyReferences
                .Where(x => x.Name.StartsWith("Microsoft.Xna.Framework"))
                .ToArray();

            if (server)
                for (var x = 0; x < xnaFramework.Length; x++)
                {
                    xnaFramework[x].Name = _self.Name.Name;
                    xnaFramework[x].PublicKey = _self.Name.PublicKey;
                    xnaFramework[x].PublicKeyToken = _self.Name.PublicKeyToken;
                    xnaFramework[x].Version = _self.Name.Version;
                }
            else
            {
                for (var x = 0; x < xnaFramework.Length; x++)
                {
                    xnaFramework[x].Name = "MonoGame.Framework";
                    xnaFramework[x].PublicKey = null;
                    xnaFramework[x].PublicKeyToken = null;
                    xnaFramework[x].Version = new Version("3.1.2.0");
                }

                //Use an NSApplication entry point for MAC
            }
        }

        public void HookMessageBuffer()
        {
            var tClass = _asm.MainModule.Types.Single(x => x.Name == "MessageBuffer");
            var getData = tClass.Methods.Single(x => x.Name == "GetData");
            var whoAmI = tClass.Fields.Single(x => x.Name == "whoAmI");

            var insertionPoint = getData.Body.Instructions
                .Single(x => x.OpCode == OpCodes.Callvirt
                    && x.Operand is MethodReference
                    && (x.Operand as MethodReference).Name == "set_Position");

            var userInputClass = _self.MainModule.Types.Single(x => x.Name == "MessageBufferCallback");
            var callback = userInputClass.Methods.First(m => m.Name == "ProcessPacket");

            var il = getData.Body.GetILProcessor();
            il.InsertAfter(insertionPoint, il.Create(OpCodes.Stloc_0));
            il.InsertAfter(insertionPoint, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
            il.InsertAfter(insertionPoint, il.Create(OpCodes.Ldloc_0));
            il.InsertAfter(insertionPoint, il.Create(OpCodes.Ldfld, whoAmI));
            il.InsertAfter(insertionPoint, il.Create(OpCodes.Ldarg_0));
        }

        public void RemoveClientCode()
        {
            var methods = _asm.MainModule.Types
                .SelectMany(x => x.Methods)
                .ToArray();
            var offsets = new System.Collections.Generic.List<Instruction>();

            foreach (var mth in methods)
            {
                var hasMatch = true;
                while (mth.HasBody && hasMatch)
                {
                    var match = mth.Body.Instructions
                        .SingleOrDefault(x => x.OpCode == OpCodes.Ldsfld
                            && x.Operand is FieldReference
                            && (x.Operand as FieldReference).Name == "netMode"
                            && x.Next.OpCode == OpCodes.Ldc_I4_1
                            && (x.Next.Next.OpCode == OpCodes.Bne_Un_S) // || x.Next.Next.OpCode == OpCodes.Bne_Un)
                            && !offsets.Contains(x)
                            && (x.Previous == null || x.Previous.OpCode != OpCodes.Bne_Un_S));

                    hasMatch = match != null;
                    if (hasMatch)
                    {
                        var blockEnd = match.Next.Next.Operand as Instruction;
                        var il = mth.Body.GetILProcessor();

                        var cur = il.Body.Instructions.IndexOf(match) + 3;

                        while (il.Body.Instructions[cur] != blockEnd)
                        {
                            il.Remove(il.Body.Instructions[cur]);
                        }
                        offsets.Add(match);
                        //var newIns = il.Body.Instructions[cur];
                        //for (var x = 0; x < il.Body.Instructions.Count; x++)
                        //{
                        //    if (il.Body.Instructions[x].Operand == newIns)
                        //    {
                        //        il.Replace(il.Body.Instructions[x], il.Create(il.Body.Instructions[x].OpCode, newIns));
                        //    }
                        //}
                    }
                }
            }
        }

        public void HookSockets()
        {
            var serverClass = _self.MainModule.Types.Single(x => x.Name == "NetplayCallback");
            var sockClass = _self.MainModule.Types.Single(x => x.Name == "IAPISocket");
            //var targetArray = serverClass.Fields.Single(x => x.Name == "slots");
            var targetField = sockClass.Fields.Single(x => x.Name == "tileSection");

            //Replace Terraria.Netplay.serverSock references with tdsm.core.Server.slots
            var instructions = _asm.MainModule.Types
                .SelectMany(x => x.Methods
                    .Where(y => y.HasBody && y.Body.Instructions != null)
                )
                .SelectMany(x => x.Body.Instructions)
                .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Ldsfld
                    && x.Operand is FieldReference
                    && (x.Operand as FieldReference).FieldType.FullName == "Terraria.ServerSock[]"
                    && x.Next.Next.Next.OpCode == Mono.Cecil.Cil.OpCodes.Ldfld
                    && x.Next.Next.Next.Operand is FieldReference
                    && (x.Next.Next.Next.Operand as FieldReference).Name == "tileSection"
                )
                .ToArray();

            for (var x = 0; x < instructions.Length; x++)
            {
                //instructions[x].Operand = _asm.MainModule.Import(targetArray);
                instructions[x].Next.Next.Next.Operand = _asm.MainModule.Import(targetField);
            }


            //TODO BELOW - update ServerSock::announce to IAPISocket::announce (etc)
            //Replace Terraria.Netplay.serverSock references with tdsm.core.Server.slots
            //instructions = _asm.MainModule.Types
            //   .SelectMany(x => x.Methods
            //       .Where(y => y.HasBody && y.Body.Instructions != null)
            //   )
            //   .SelectMany(x => x.Body.Instructions)
            //   .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Ldsfld
            //       && x.Operand is FieldReference
            //       && (x.Operand as FieldReference).FieldType.FullName == "Terraria.ServerSock[]"
            //   )
            //   .ToArray();

            //for (var x = 0; x < instructions.Length; x++)
            //{
            //    instructions[x].Operand = _asm.MainModule.Import(targetArray);

            //    //var var = instructions[x].Next.Next.Next;
            //    //if (var.OpCode == OpCodes.Ldfld && var.Operand is MemberReference)
            //    //{
            //    //    var mem = var.Operand as MemberReference;
            //    //    if (mem.DeclaringType.Name == "ServerSock")
            //    //    {
            //    //        var ourVar = sockClass.Fields.Where(j => j.Name == mem.Name).FirstOrDefault();
            //    //        if (ourVar != null)
            //    //        {
            //    //            var.Operand = _asm.MainModule.Import(ourVar);
            //    //        }
            //    //    }
            //    //}
            //}

            instructions = _asm.MainModule.Types
               .SelectMany(x => x.Methods
                   .Where(y => y.HasBody && y.Body.Instructions != null)
               )
               .SelectMany(x => x.Body.Instructions)
               .Where(x => (x.OpCode == Mono.Cecil.Cil.OpCodes.Callvirt)
                   &&
                   (
                        (x.Operand is MemberReference && (x.Operand as MemberReference).DeclaringType.FullName == "Terraria.ServerSock")
                        ||
                        (x.Operand is MethodDefinition && (x.Operand as MethodDefinition).DeclaringType.FullName == "Terraria.ServerSock")
                   )
               )
               .ToArray();

            instructions = _asm.MainModule.Types
               .SelectMany(x => x.Methods
                   .Where(y => y.HasBody && y.Body.Instructions != null)
               )
               .SelectMany(x => x.Body.Instructions)
               .Where(x => (x.OpCode == Mono.Cecil.Cil.OpCodes.Ldfld || x.OpCode == Mono.Cecil.Cil.OpCodes.Stfld || x.OpCode == Mono.Cecil.Cil.OpCodes.Callvirt)
                   &&
                   (
                        (x.Operand is MemberReference && (x.Operand as MemberReference).DeclaringType.FullName == "Terraria.ServerSock")
                        ||
                        (x.Operand is MethodDefinition && (x.Operand as MethodDefinition).DeclaringType.FullName == "Terraria.ServerSock")
                   )
               )
               .ToArray();

            for (var x = 0; x < instructions.Length; x++)
            {
                var var = instructions[x];
                if (var.Operand is MethodDefinition)
                {
                    var mth = var.Operand as MethodDefinition;
                    var ourVar = sockClass.Methods.SingleOrDefault(j => j.Name == mth.Name);
                    if (ourVar != null)
                    {
                        var.Operand = _asm.MainModule.Import(ourVar);
                    }
                    else
                    {

                    }
                }
                else if (var.Operand is MemberReference)
                {
                    var mem = var.Operand as MemberReference;
                    var ourVar = sockClass.Fields.SingleOrDefault(j => j.Name == mem.Name);
                    if (ourVar != null)
                    {
                        var.Operand = _asm.MainModule.Import(ourVar);
                    }
                }
            }

            var ourClass = _self.MainModule.Types.Single(x => x.Name == "NetplayCallback");
            foreach (var rep in new string[] { "SendAnglerQuest",/* "sendWater", "syncPlayers",*/ "AddBan" })
            {
                var toBeReplaced = _asm.MainModule.Types
                    .SelectMany(x => x.Methods
                        .Where(y => y.HasBody)
                    )
                    .SelectMany(x => x.Body.Instructions)
                    .Where(x => x.OpCode == Mono.Cecil.Cil.OpCodes.Call
                        && x.Operand is MethodReference
                        && (x.Operand as MethodReference).Name == rep
                    )
                    .ToArray();

                var replacement = ourClass.Methods.Single(x => x.Name == rep);
                for (var x = 0; x < toBeReplaced.Length; x++)
                {
                    toBeReplaced[x].Operand = _asm.MainModule.Import(replacement);
                }
            }

            //Change to override
            var serverSock = _asm.MainModule.Types.Single(x => x.Name == "ServerSock");
            serverSock.BaseType = _asm.MainModule.Import(sockClass);
            foreach (var rep in new string[] { "SpamUpdate", "SpamClear", "Reset", "SectionRange" })
            {
                var mth = serverSock.Methods.Single(x => x.Name == rep);
                mth.IsVirtual = true;
            }

            //Remove variables that are in the base class
            foreach (var fld in sockClass.Fields)
            {
                var rem = serverSock.Fields.SingleOrDefault(x => x.Name == fld.Name);
                if (rem != null)
                {
                    serverSock.Fields.Remove(rem);
                }
            }

            //Now change Netplay.serverSock to the IAPISocket type
            var netplay = _asm.MainModule.Types.Single(x => x.Name == "Netplay");
            var serverSockArr = netplay.Fields.Single(x => x.Name == "serverSock");
            var at = new ArrayType(_asm.MainModule.Import(sockClass));
            serverSockArr.FieldType = at;



            var sendWater = _asm.MainModule.Types.Single(x => x.Name == "NetMessage").Methods.Single(x => x.Name == "sendWater");
            {
                var ix = 0;
                var removing = false;
                while (sendWater.Body.Instructions.Count > 0 && ix < sendWater.Body.Instructions.Count)
                {
                    var first = false;
                    var ins = sendWater.Body.Instructions[ix];
                    if (ins.OpCode == OpCodes.Ldsfld && ins.Operand is FieldReference && (ins.Operand as FieldReference).Name == "buffer")
                    {
                        removing = true;
                        first = true;
                    }
                    else first = false;

                    if (ins.OpCode == OpCodes.Brfalse_S)
                    {
                        //Keep instruction, and replace the first (previous instruction)
                        var canSendWater = _self.MainModule.Types.Single(x => x.Name == "IAPISocket").Methods.Single(x => x.Name == "CanSendWater");

                        //IL_0011: nop
                        //IL_0012: ldsfld class tdsm.api.Callbacks.IAPISocket[] [tdsm]Terraria.Netplay::serverSock
                        //IL_0017: ldloc.0
                        //IL_0018: ldelem.ref
                        //IL_0019: callvirt instance bool tdsm.api.Callbacks.IAPISocket::CanSendWater()
                        //IL_001e: ldc.i4.0
                        //IL_001f: ceq
                        //IL_0021: stloc.3
                        //IL_0022: ldloc.3
                        //IL_0023: brtrue.s IL_006e

                        var il = sendWater.Body.GetILProcessor();
                        var target = ins.Previous;
                        var newTarget = il.Create(OpCodes.Nop);

                        il.Replace(target, newTarget);

                        il.InsertAfter(newTarget, il.Create(OpCodes.Callvirt, _asm.MainModule.Import(canSendWater)));
                        il.InsertAfter(newTarget, il.Create(OpCodes.Ldelem_Ref));
                        il.InsertAfter(newTarget, il.Create(OpCodes.Ldloc_0));
                        il.InsertAfter(newTarget, il.Create(OpCodes.Ldsfld, _asm.MainModule.Import(serverSockArr)));

                        removing = false;
                        break;
                    }

                    if (removing && !first)
                    {
                        sendWater.Body.Instructions.RemoveAt(ix);
                    }

                    if (!removing || first) ix++;
                }
            }

            var syncPlayers = _asm.MainModule.Types.Single(x => x.Name == "NetMessage").Methods.Single(x => x.Name == "syncPlayers");
            {
                var ix = 0;
                var removing = false;
                var isPlayingComplete = false;
                while (syncPlayers.Body.Instructions.Count > 0 && ix < syncPlayers.Body.Instructions.Count)
                {
                    var first = false;
                    var ins = syncPlayers.Body.Instructions[ix];
                    if (ins.OpCode == OpCodes.Ldsfld && ins.Operand is FieldDefinition && (ins.Operand as FieldDefinition).Name == "serverSock")
                    {
                        removing = true;
                        first = true;

                        if (isPlayingComplete)
                        {
                            //We'll use the next two instructions because im cheap.
                            ix += 2;
                        }
                    }
                    else first = false;

                    if (removing && ins.OpCode == OpCodes.Bne_Un)
                    {
                        //Keep instruction, and replace the first (previous instruction)
                        var isPlaying = _self.MainModule.Types.Single(x => x.Name == "IAPISocket").Methods.Single(x => x.Name == "IsPlaying");

                        //IL_0036: nop

                        //IL_0037: ldsfld class tdsm.api.Callbacks.IAPISocket[] [tdsm]Terraria.Netplay::serverSock
                        //IL_003c: ldloc.1
                        //IL_003d: ldelem.ref
                        //IL_003e: callvirt instance bool tdsm.api.Callbacks.IAPISocket::IsPlaying()
                        //IL_0043: ldc.i4.0
                        //IL_0044: ceq
                        //IL_0046: stloc.s CS$4$0000
                        //IL_0048: ldloc.s CS$4$0000
                        //IL_004a: brtrue IL_0874

                        var il = syncPlayers.Body.GetILProcessor();
                        var target = ins.Previous;

                        il.InsertAfter(target, il.Create(OpCodes.Callvirt, _asm.MainModule.Import(isPlaying)));
                        il.InsertAfter(target, il.Create(OpCodes.Ldelem_Ref));
                        il.InsertAfter(target, il.Create(OpCodes.Ldloc_1));

                        il.Replace(ins, il.Create(OpCodes.Brfalse, ins.Operand as Instruction));

                        isPlayingComplete = true;
                        removing = false;
                        //break;

                        ix += 3;
                    }
                    else if (removing && ins.OpCode == OpCodes.Callvirt && isPlayingComplete)
                    {
                        if (ins.Operand is MethodReference)
                        {
                            var md = ins.Operand as MethodReference;
                            if (md.DeclaringType.Name == "Object" && md.Name == "ToString")
                            {
                                var remoteAddress = _self.MainModule.Types.Single(x => x.Name == "IAPISocket").Methods.Single(x => x.Name == "RemoteAddress");
                                ins.Operand = _asm.MainModule.Import(remoteAddress);
                                break;
                            }
                        }
                    }

                    if (removing && !first)
                    {
                        syncPlayers.Body.Instructions.RemoveAt(ix);
                    }

                    if (!removing || first) ix++;
                }
            }
        }

        public void HookNPCSpawning()
        {
            var npc = _asm.MainModule.Types.Single(x => x.Name == "NPC");
            var newNPC = npc.Methods.Single(x => x.Name == "NewNPC");

            var callback = _self.MainModule.Types.Single(x => x.Name == "NPCCallback");
            var method = callback.Methods.Single(x => x.Name == "CanSpawnNPC");

            var il = newNPC.Body.GetILProcessor();
            var first = newNPC.Body.Instructions.First();

            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_1));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_2));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_3));
            il.InsertBefore(first, il.Create(OpCodes.Call, _asm.MainModule.Import(method)));

            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ldc_I4, 200));
            il.InsertBefore(first, il.Create(OpCodes.Ret));
        }

        public void HookEclipse()
        {
            var type = _asm.MainModule.Types.Single(x => x.Name == "Main");
            var mth = type.Methods.Single(x => x.Name == "UpdateTime");

            var callback = _self.MainModule.Types.Single(x => x.Name == "MainCallback");
            var field = callback.Fields.Single(x => x.Name == "StartEclipse");

            var il = mth.Body.GetILProcessor();
            var start = il.Body.Instructions.Single(x =>
                x.OpCode == OpCodes.Ldsfld
                && x.Operand is FieldReference
                && (x.Operand as FieldReference).Name == "hardMode"
                && x.Previous.OpCode == OpCodes.Call
                && x.Previous.Operand is MethodReference
                && (x.Previous.Operand as MethodReference).Name == "StartInvasion"
            );

            //Preserve
            var old = start.Operand as FieldReference;

            //Replace with ours to keep the IL inline
            start.Operand = _asm.MainModule.Import(field);
            //Readd the preserved
            il.InsertAfter(start, il.Create(OpCodes.Ldsfld, old));

            //Now find the target instruction if the value is true
            var startEclipse = il.Body.Instructions.Single(x =>
                x.OpCode == OpCodes.Stsfld
                && x.Operand is FieldReference
                && (x.Operand as FieldReference).Name == "eclipse"
                && x.Next.OpCode == OpCodes.Ldsfld
                && x.Next.Operand is FieldReference
                && (x.Next.Operand as FieldReference).Name == "eclipse"
            ).Previous;
            il.InsertAfter(start, il.Create(OpCodes.Brtrue, startEclipse));

            //Since all we care about is setting the StartEclipse to TRUE; we need to be able to disable once done.
            il.InsertAfter(startEclipse.Next, il.Create(OpCodes.Stsfld, start.Operand as FieldReference));
            il.InsertAfter(startEclipse.Next, il.Create(OpCodes.Ldc_I4_0));
        }

        public void HookBloodMoon()
        {
            var type = _asm.MainModule.Types.Single(x => x.Name == "Main");
            var mth = type.Methods.Single(x => x.Name == "UpdateTime");

            var callback = _self.MainModule.Types.Single(x => x.Name == "MainCallback");
            var field = callback.Fields.Single(x => x.Name == "StartBloodMoon");
            //return;
            var il = mth.Body.GetILProcessor();
            var start = il.Body.Instructions.Single(x =>
                x.OpCode == OpCodes.Ldsfld
                && x.Operand is FieldReference
                && (x.Operand as FieldReference).Name == "spawnEye"
                && x.Next.Next.OpCode == OpCodes.Ldsfld
                && x.Next.Next.Operand is FieldReference
                && (x.Next.Next.Operand as FieldReference).Name == "moonPhase"
            );

            //Preserve
            var old = start.Operand as FieldReference;
            var target = start.Next as Instruction;

            //Replace with ours to keep the IL inline
            start.Operand = _asm.MainModule.Import(field);
            //Readd the preserved
            il.InsertAfter(start, il.Create(OpCodes.Ldsfld, old));

            //Now find the target instruction if the value is true
            Instruction begin = start.Next;
            var i = 12;
            while (i > 0)
            {
                i--;
                begin = begin.Next;
            }
            il.InsertAfter(start, il.Create(OpCodes.Brtrue, begin));

            //Since all we care about is setting the StartBloodMoon to TRUE; we need to be able to disable once done.
            var startBloodMoon = il.Body.Instructions.Single(x =>
                x.OpCode == OpCodes.Ldsfld
                && x.Operand is FieldReference
                && (x.Operand as FieldReference).Name == "bloodMoon"
                && x.Next.Next.OpCode == OpCodes.Ldsfld
                && x.Next.Next.Operand is FieldReference
                && (x.Next.Next.Operand as FieldReference).Name == "netMode"
            );
            il.InsertAfter(startBloodMoon.Next, il.Create(OpCodes.Stsfld, start.Operand as FieldReference));
            il.InsertAfter(startBloodMoon.Next, il.Create(OpCodes.Ldc_I4_0));
        }

        public void Save(string fileName, int apiBuild, string tdsmUID, string name)
        {
            //Ensure the name is updated to the new one
            _asm.Name = new AssemblyNameDefinition(name, new Version(0, 0, apiBuild, 0));
            _asm.MainModule.Name = fileName;

            //Change the uniqueness from what Terraria has, to something different (that way vanilla isn't picked up by assembly resolutions)
            var g = _asm.CustomAttributes.Single(x => x.AttributeType.Name == "GuidAttribute");

            for (var x = 0; x < _asm.CustomAttributes.Count; x++)
            {
                if (_asm.CustomAttributes[x].AttributeType.Name == "GuidAttribute")
                {
                    _asm.CustomAttributes[x].ConstructorArguments[0] =
                        new CustomAttributeArgument(_asm.CustomAttributes[x].ConstructorArguments[0].Type, tdsmUID);
                }
                else if (_asm.CustomAttributes[x].AttributeType.Name == "AssemblyTitleAttribute")
                {
                    _asm.CustomAttributes[x].ConstructorArguments[0] =
                        new CustomAttributeArgument(_asm.CustomAttributes[x].ConstructorArguments[0].Type, name);
                }
                else if (_asm.CustomAttributes[x].AttributeType.Name == "AssemblyProductAttribute")
                {
                    _asm.CustomAttributes[x].ConstructorArguments[0] =
                        new CustomAttributeArgument(_asm.CustomAttributes[x].ConstructorArguments[0].Type, name);
                }
                //else if (_asm.CustomAttributes[x].AttributeType.Name == "AssemblyFileVersionAttribute")
                //{
                //    _asm.CustomAttributes[x].ConstructorArguments[0] =
                //        new CustomAttributeArgument(_asm.CustomAttributes[x].ConstructorArguments[0].Type, "1.0.0.0");
                //}
            }

            //_asm.Write(fileName);
            using (var fs = File.OpenWrite(fileName))
            {
                _asm.Write(fs);
                fs.Flush();
                fs.Close();
            }
        }

        public void Dispose()
        {
            _self = null;
            _asm = null;
        }
    }
}
