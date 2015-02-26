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
                .Where(x => x.AttributeType.Name == "AssemblyFileVersionAttribute")
                .First()
                .ConstructorArguments
                .First()
                .Value as string;
        }

        public void HookDedServEnd()
        {
            var main = _asm.MainModule.Types.Where(x => x.Name == "Main").First();
            var method = main.Methods.Where(x => x.Name == "DedServ").First();
            var callback = _self.MainModule.Types.Where(x => x.Name == "MainCallback").First();
            var replacement = callback.Methods.Where(x => x.Name == "OnProgramFinished" && x.IsStatic).First();

            var imported = _asm.MainModule.Import(replacement);
            var il = method.Body.GetILProcessor();

            il.InsertBefore(method.Body.Instructions.Last(), il.Create(OpCodes.Call, imported));
        }

        public void HookConfig()
        {
            var serv = _asm.MainModule.Types.Where(x => x.Name == "ProgramServer").First();
            var main = serv.Methods.Where(x => x.Name == "Main" && x.IsStatic).First();
            var ourClass = _self.MainModule.Types.Where(x => x.Name == "Configuration").First();
            var replacement = ourClass.Methods.Where(x => x.Name == "Load" && x.IsStatic).First();

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

        public void HookWorldFile_DEBUG()
        {
            var serv = _asm.MainModule.Types.Where(x => x.Name == "WorldGen").First();
            var main = serv.Methods.Where(x => x.Name == "serverLoadWorldCallBack" && x.IsStatic).First();

            var ourClass = _self.MainModule.Types.Where(x => x.Name == "WorldFileCallback").First();
            var replacement = ourClass.Methods.Where(x => x.Name == "loadWorld" && x.IsStatic).First();

            //Grab all occurances of "LoadDedConfig" and route it to ours
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
        }

        public void HookStatusText()
        {
            var main = _asm.MainModule.Types.Where(x => x.Name == "Main").First();
            var dedServ = main.Methods.Where(x => x.Name == "DedServ").First();

            var selfType = _self.MainModule.Types.Where(x => x.Name == "MainCallback").First();
            var callback = selfType.Methods.Where(x => x.Name == "OnStatusTextChange").First();

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
            var server = _asm.MainModule.Types.Where(x => x.Name == "NetMessage").First();
            var method = server.Methods.Where(x => x.Name == "SendData").First();

            var userInputClass = _self.MainModule.Types.Where(x => x.Name == "NetMessageCallback").First();
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
            var cls = _asm.MainModule.Types.Where(x => x.Name == "Main").First();
            var method = cls.Methods.Where(x => x.Name == "DedServ").First();

            var cbc = _self.MainModule.Types.Where(x => x.Name == "GameWindow").First();
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
            var server = _asm.MainModule.Types.Where(x => x.Name == "ProgramServer").First();
            var method = server.Methods.Where(x => x.Name == "Main").First();

            var userInputClass = _self.MainModule.Types.Where(x => x.Name == "MainCallback").First();
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
            var server = _asm.MainModule.Types.Where(x => x.Name == "Main").First();
            var method = server.Methods.Where(x => x.Name == "UpdateServer").First();

            var userInputClass = _self.MainModule.Types.Where(x => x.Name == "MainCallback").First();
            var callback = userInputClass.Methods.First(m => m.Name == "UpdateServerEnd");

            var il = method.Body.GetILProcessor();
            il.InsertBefore(method.Body.Instructions.Last(), il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        }

        public void HookInitialise()
        {
            var server = _asm.MainModule.Types.Where(x => x.Name == "Netplay").First();
            var method = server.Methods.Where(x => x.Name == "Init").First();

            var userInputClass = _self.MainModule.Types.Where(x => x.Name == "MainCallback").First();
            var callback = userInputClass.Methods.First(m => m.Name == "Initialise");

            var il = method.Body.GetILProcessor();
            il.InsertBefore(method.Body.Instructions.First(), il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        }

        public void HookWorldEvents()
        {
            var worldGen = _asm.MainModule.Types.Where(x => x.Name == "WorldGen").First();
            var method = worldGen.Methods.Where(x => x.Name == "generateWorld").First();

            var userInputClass = _self.MainModule.Types.Where(x => x.Name == "MainCallback").First();
            var callbackBegin = userInputClass.Methods.First(m => m.Name == "WorldGenerateBegin");
            var callbackEnd = userInputClass.Methods.First(m => m.Name == "WorldGenerateEnd");

            var il = method.Body.GetILProcessor();
            il.InsertBefore(method.Body.Instructions.First(), il.Create(OpCodes.Call, _asm.MainModule.Import(callbackBegin)));
            il.InsertBefore(method.Body.Instructions.Last(), il.Create(OpCodes.Call, _asm.MainModule.Import(callbackEnd)));

            var worldFile = _asm.MainModule.Types.Where(x => x.Name == "WorldFile").First();
            method = worldFile.Methods.Where(x => x.Name == "loadWorld").First();

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
            var netplay = _asm.MainModule.Types.Where(x => x.Name == "Netplay").First();
            var method = netplay.Methods.Where(x => x.Name == "StartServer").First();

            var userInputClass = _self.MainModule.Types.Where(x => x.Name == "NetplayCallback").First();
            var callback = userInputClass.Methods.First(m => m.Name == "StartServer");

            var ins = method.Body.Instructions
                .Where(x => x.OpCode == OpCodes.Ldftn)
                .First();
            ins.Operand = _asm.MainModule.Import(callback);

            //Make the Player inherit our defaults
            var player = _asm.MainModule.Types.Where(x => x.Name == "Player").First();
            var baseType = _self.MainModule.Types.Where(x => x.Name == "BasePlayer").First();
            //var interfaceType = _self.MainModule.Types.Where(x => x.Name == "ISender").First();

            player.BaseType = _asm.MainModule.Import(baseType);

            //Make the UpdateServer function public
            var main = _asm.MainModule.Types.Where(x => x.Name == "Main").First();
            var us = main.Methods.Where(x => x.Name == "UpdateServer").First();
            us.IsPrivate = false;
            us.IsPublic = true;
        }

        public void FixNetplay()
        {
            const String NATGuid = "AE1E00AA-3FD5-403C-8A27-2BBDC30CD0E1";
            var netplay = _asm.MainModule.Types.Where(x => x.Name == "Netplay").First();
            var staticConstructor = netplay.Methods.Where(x => x.Name == ".cctor").First();

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

            netplay.Fields.Remove(
                netplay.Fields.Where(x => x.Name == "upnpnat").First()
            );

            //Clear open and close methods
            netplay.Methods.Where(x => x.Name == "openPort").First().Body.Instructions.Clear();
            netplay.Methods.Where(x => x.Name == "closePort").First().Body.Instructions.Clear();
            netplay.Fields.Remove(
                netplay.Fields.Where(x => x.Name == "mappings").First()
            );
        }

        public void FixEntryPoint()
        {
            var netplay = _asm.MainModule.Types.Where(x => x.Name == "ProgramServer").First();
            var staticConstructor = netplay.Methods.Where(x => x.Name == "Main").First();

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
            var netplay = _asm.MainModule.Types.Where(x => x.Name == "Main").First();
            var staticConstructor = netplay.Methods.Where(x => x.Name == ".cctor").First();

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
                    var patches = _self.MainModule.Types.Where(k => k.Name == "Patches").First();
                    var dir = _asm.MainModule.Import(patches.Methods.Where(k => k.Name == "GetCurrentDirectory").First());

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
            var main = _asm.MainModule.Types.Where(x => x.Name == "Main").First();

            var initialise = main.Methods.Where(x => x.Name == "Initialize").First();
            var loc = initialise.Body.Instructions
                .Where(x => x.OpCode == OpCodes.Ldsfld && x.Operand is FieldDefinition)
                //.Select(x => x.Operand as FieldDefinition)
                .Where(x => (x.Operand as FieldDefinition).Name == "skipMenu")
                .First();
            var il = initialise.Body.GetILProcessor();
            il.InsertBefore(loc, il.Create(OpCodes.Ret));
        }


        /// <summary>
        /// Adds our command line hook so we get input control from the admin
        /// </summary>
        public void PatchCommandLine()
        {
            var t_mainClass = _asm.MainModule.Types.Where(x => x.Name == "Main").First();

            //Simply switch to ours
            var serv = t_mainClass.Methods.Where(x => x.Name == "DedServ").First();

            var userInputClass = _self.MainModule.Types.Where(x => x.Name == "UserInput").First();
            var callback = userInputClass.Methods.First(m => m.Name == "ListenForCommands");

            var ins = serv.Body.Instructions
                .Where(x => x.OpCode == OpCodes.Call && x.Operand is MethodReference && (x.Operand as MethodReference).Name == "startDedInput")
                .First();
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

            var tools = _self.MainModule.Types.Where(x => x.Name == "Tools").First();
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
            var tileClass = _asm.MainModule.Types.Where(x => x.Name == "Tile").First();
            var refClass = _self.MainModule.Types.Where(x => x.Name == "TileData").First();

            var userInput = _self.MainModule.Types.Where(x => x.Name == "UserInput").First();
            var DefaultTile = userInput.Fields.Where(x => x.Name == "DefaultTile").First();

            //tileClass.BaseType = refClass.BaseType;
            //tileClass.IsSequentialLayout = true;

            //Update nulls to defaults
            var mainClass = _asm.MainModule.Types.Where(x => x.Name == "Main").First();

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
                    xnaFramework[x].Name = "Microsoft.Xna.Framework";
                    xnaFramework[x].PublicKey = null;
                    xnaFramework[x].PublicKeyToken = null;
                    xnaFramework[x].Version = new Version("3.1.2.0");
                }

                //Use an NSApplication entry point for MAC
            }
        }

        public void HookMessageBuffer()
        {
            var tClass = _asm.MainModule.Types.Where(x => x.Name == "MessageBuffer").First();
            var getData = tClass.Methods.Where(x => x.Name == "GetData").First();
            var whoAmI = tClass.Fields.Where(x => x.Name == "whoAmI").First();

            var insertionPoint = getData.Body.Instructions
                .Where(x => x.OpCode == OpCodes.Callvirt && x.Operand is MethodReference && (x.Operand as MethodReference).Name == "set_Position")
                .First();

            var userInputClass = _self.MainModule.Types.Where(x => x.Name == "MessageBufferCallback").First();
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
                        .Where(x => x.OpCode == OpCodes.Ldsfld
                            && x.Operand is FieldReference
                            && (x.Operand as FieldReference).Name == "netMode"
                            && x.Next.OpCode == OpCodes.Ldc_I4_1
                            && (x.Next.Next.OpCode == OpCodes.Bne_Un_S) // || x.Next.Next.OpCode == OpCodes.Bne_Un)
                            && !offsets.Contains(x)
                            && (x.Previous == null || x.Previous.OpCode != OpCodes.Bne_Un_S))
                        .FirstOrDefault();

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
            var serverClass = _self.MainModule.Types.Where(x => x.Name == "NetplayCallback").First();
            var sockClass = _self.MainModule.Types.Where(x => x.Name == "IAPISocket").First();
            var targetArray = serverClass.Fields.Where(x => x.Name == "slots").First();
            var targetField = sockClass.Fields.Where(x => x.Name == "tileSection").First();

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
                instructions[x].Operand = _asm.MainModule.Import(targetArray);
                instructions[x].Next.Next.Next.Operand = _asm.MainModule.Import(targetField);
            }

            var ourClass = _self.MainModule.Types.Where(x => x.Name == "NetplayCallback").First();
            foreach (var rep in new string[] { "SendAnglerQuest", "sendWater", "syncPlayers", "AddBan" })
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

                var replacement = ourClass.Methods.Where(x => x.Name == rep).First();
                for (var x = 0; x < toBeReplaced.Length; x++)
                {
                    toBeReplaced[x].Operand = _asm.MainModule.Import(replacement);
                }
            }
        }

        public void HookNPCSpawning()
        {
            var npc = _asm.MainModule.Types.Where(x => x.Name == "NPC").First();
            var newNPC = npc.Methods.Where(x => x.Name == "NewNPC").First();

            var callback = _self.MainModule.Types.Where(x => x.Name == "NPCCallback").First();
            var method = callback.Methods.Where(x => x.Name == "CanSpawnNPC").First();

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

        public void Save(string fileName, int apiBuild, string tdsmUID, string name)
        {
            //Ensure the name is updated to the new one
            _asm.Name = new AssemblyNameDefinition(name, new Version(0, 0, apiBuild, 0));
            _asm.MainModule.Name = fileName;

            //Change the uniqueness from what Terraria has, to something different (that way vanilla isn't picked up by assembly resolutions)
            var g = _asm.CustomAttributes.Where(x => x.AttributeType.Name == "GuidAttribute").First();

            for (var x = 0; x < _asm.CustomAttributes.Count; x++)
            {
                if (_asm.CustomAttributes[x].AttributeType.Name == "GuidAttribute")
                {
                    _asm.CustomAttributes[x].ConstructorArguments[0] =
                        new CustomAttributeArgument(_asm.CustomAttributes[x].ConstructorArguments[0].Type, tdsmUID);
                }
                //else if (_asm.CustomAttributes[x].AttributeType.Name == "AssemblyFileVersionAttribute")
                //{
                //    _asm.CustomAttributes[x].ConstructorArguments[0] =
                //        new CustomAttributeArgument(_asm.CustomAttributes[x].ConstructorArguments[0].Type, "1.0.0.0");
                //}
            }

            _asm.Write(fileName);
        }

        public void Dispose()
        {
            _self = null;
            _asm = null;
        }
    }
}
