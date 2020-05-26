using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using dnlib.IO;
using dnlib.W32Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MemeDevirtualizer
{
    class Program
    {
        public static ModuleDefMD module;
        static void Main(string[] args)
        {
string strr =@"                     __  __                     _____             _      _   
                    |  \/  |                   |  __ \           (_)    | |  
                    | \  / | ___ _ __ ___   ___| |  | | _____   ___ _ __| |_ 
                    | |\/| |/ _ \ '_ ` _ \ / _ \ |  | |/ _ \ \ / / | '__| __|
                    | |  | |  __/ | | | | |  __/ |__| |  __/\ V /| | |  | |_ 
                    |_|  |_|\___|_| |_| |_|\___|_____/ \___| \_/ |_|_|   \__|";
            Console.WriteLine(strr);
            Console.WriteLine("                              - by MindSystem \n");
            try
            {
                module = ModuleDefMD.Load(args[0]);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Load a virtualized assembly into this exe");
                Console.ReadLine();
                return;
            }
            asmRefAdder();
            List<MethodDef> Virtualized = FindVirtualizedMethod(module);
            Dictionary<MethodDef, int> MethodIndex;
            MethodIndex = FindMethodIndex(Virtualized);
            foreach (MethodDef meth in MethodIndex.Keys)
            {
                Console.WriteLine("Find Virtualized method : {0} with index {1}", meth.Name, MethodIndex[meth]); ;
            }
            Stream str = null;
            foreach(EmbeddedResource res in module.Resources)
            {
                if(res.Name == " ")
                {
                    DataReader red = res.CreateReader();
                    str = red.AsStream();
                    break;
                }
              
            }
            Decompress(str);
            foreach(MethodDef method in MethodIndex.Keys)
            {
                RecoverMethod(method, MethodIndex[method]);
      //          method.Body.SimplifyMacros(method.Parameters);
            }

            string SavingPath = module.Kind == ModuleKind.Dll ? args[0].Replace(".dll", "-Obfuscated.dll") : args[0].Replace(".exe", "-Obfuscated.exe");
            if (module.IsILOnly)
            {
                var opts = new ModuleWriterOptions(module);
                opts.MetadataOptions.Flags = MetadataFlags.PreserveAll;
                opts.MetadataOptions.Flags = MetadataFlags.KeepOldMaxStack;
              //  opts.Logger = DummyLogger.NoThrowInstance;
               module.Write(SavingPath, opts);
            }
            else
            {
                var opts = new NativeModuleWriterOptions(module, false);
                opts.MetadataOptions.Flags = MetadataFlags.PreserveAll;
            //    opts.Logger = DummyLogger.NoThrowInstance;
                module.NativeWrite(SavingPath, opts);
            }
        }

        private static void asmRefAdder()
        {
            var asmResolver = new AssemblyResolver { EnableTypeDefCache = true };
            var modCtx = new ModuleContext(asmResolver);
            asmResolver.DefaultModuleContext = modCtx;
            var asmRefs = module.GetAssemblyRefs().ToList();
            module.Context = modCtx;
            foreach (var asmRef in asmRefs)
            {
                try
                {
                    if (asmRef == null)
                        continue;
                    var asm = asmResolver.Resolve(asmRef.FullName, module);
                    if (asm == null)
                        continue;
                    ((AssemblyResolver)module.Context.AssemblyResolver).AddToCache(asm);

                }
                catch
                {

                }
            }
        }
        public static void RecoverMethod(MethodDef method, int index)
        {
            method.Body.Instructions.Clear();
            List<Instruction> VMBody = _methods[index];
            foreach (Instruction instr in VMBody)
            {
                method.Body.Instructions.Add(CreateInstr(instr, method));
            }
            CorrectBranches(method);
        }
        public static void CorrectBranches(MethodDef method)
        {
            for(int i = 0; i < method.Body.Instructions.Count-1;i++)
            {
                if(method.Body.Instructions[i].OpCode == OpCodes.Ldstr && method.Body.Instructions[i].Operand.ToString().Contains("BrTrue|"))
                {
                    string operand = method.Body.Instructions[i].Operand.ToString();
                    int GoodInstruction = Convert.ToInt32(operand.Split('|')[1]);
                    method.Body.Instructions[i] = dnlib.DotNet.Emit.Instruction.Create(OpCodes.Brtrue, method.Body.Instructions[GoodInstruction]);
                }
                else if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr && method.Body.Instructions[i].Operand.ToString().Contains("BrFalse|"))
                {
                    string operand = method.Body.Instructions[i].Operand.ToString();
                    int GoodInstruction = Convert.ToInt32(operand.Split('|')[1]);
                    method.Body.Instructions[i] = dnlib.DotNet.Emit.Instruction.Create(OpCodes.Brfalse, method.Body.Instructions[GoodInstruction]);
                }
                else if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr && method.Body.Instructions[i].Operand.ToString().Contains("Br|"))
                {
                    string operand = method.Body.Instructions[i].Operand.ToString();
                    int GoodInstruction = Convert.ToInt32(operand.Split('|')[1]);
                    method.Body.Instructions[i] = dnlib.DotNet.Emit.Instruction.Create(OpCodes.Brfalse, method.Body.Instructions[GoodInstruction]);
                }
            }
        }
        internal static string GetReference(short index)
        {
            KeyValuePair<string, Assembly> keyValuePair = _references.ElementAt((int)index);
            bool flag = keyValuePair.Value == null;
            if (flag)
            {
                _references[keyValuePair.Key] = AppDomain.CurrentDomain.Load(new AssemblyName(keyValuePair.Key));
            }
            return _references[keyValuePair.Key].Location;
        }
        public static Stack Stack = new Stack();
        public static dnlib.DotNet.Emit.Instruction CreateInstr(Instruction instr, MethodDef meth)
        {
            switch(instr.Code)
            {
                case OpCode.Add:
                    Stack.Pop();
                    Stack.Pop();
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Add);
                case OpCode.Call:
                    Tuple<short, int, bool> tuple = (Tuple<short, int, bool>)instr.Operand;
                    ModuleDefMD str = ModuleDefMD.Load(GetReference(tuple.Item1));
                    IMethod meh = (IMethod)str.ResolveToken((uint)tuple.Item2);
                    module.Import(meh);
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Call, module.Import(meh));
                case OpCode.Cgt:
                    Stack.Pop();
                    Stack.Pop();
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Cgt);
                case OpCode.Clt:
                    Stack.Pop();
                    Stack.Pop();
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Clt);
              //  case OpCode.Cmp:
                //    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.C);
                case OpCode.Div:
                    Stack.Pop();
                    Stack.Pop();
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Div);
                case OpCode.Dup:
                    object value = Stack.Pop();
                    Stack.Push(value);
                    Stack.Push(value);
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Dup);
                case OpCode.Jf:
                    Stack.Pop();
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Ldstr, string.Format("BrFalse|{0}",meth.Body.Instructions[(int)instr.Operand]));
                case OpCode.Jmp:
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Ldstr, string.Format("Br|{0}", meth.Body.Instructions[(int)instr.Operand]));
                case OpCode.Jt:
                    Stack.Pop();
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Ldstr, string.Format("BrTrue|{0}", meth.Body.Instructions[(int)instr.Operand]));
                case OpCode.Int32:
                    Stack.Push((int)instr.Operand);
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Ldc_I4, (int)instr.Operand);
                case OpCode.Ldarg:
                    //A fix
                    Stack.Push(meth.Parameters[(int)((short)instr.Operand)]);
                    return dnlib.DotNet.Emit.Instruction.Create(ldarg(Convert.ToInt32(instr.Operand)));
                case OpCode.Ldfld:
                    Stack.Pop();
                    int item2 = ((Tuple<short, int>)instr.Operand).Item2;
                    FieldDef fld = module.ResolveField((uint)item2);
                    Stack.Push(fld.InitialValue);
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Ldfld, fld);
                case OpCode.Ldloc:
                    //A fix
                    Local l = meth.Body.Variables[(short)instr.Operand];
                    Stack.Push(l);
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Ldloc, l);
                case OpCode.Mul:
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Mul);
                case OpCode.Int64:
                    Stack.Push((int)instr.Operand);
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Ldc_I8, (long)instr.Operand);
                case OpCode.Newarr:
                    Stack.Pop();
                    Tuple<short, int> tuple3 = (Tuple<short, int>)instr.Operand;
                    Stack.Push(module.ResolveTypeDef((uint)tuple3.Item2)) ;
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Newarr, module.ResolveTypeDef((uint)tuple3.Item2));
                case OpCode.Null:
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Nop);
                case OpCode.Pop:
                    Stack.Pop();
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Pop);
                case OpCode.Ret:
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Ret);
                case OpCode.Stfld:
                    Stack.Pop();
                    int item22 = ((Tuple<short, int>)instr.Operand).Item2;
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Stfld, module.ResolveField((uint)item22));
                case OpCode.Stloc:
                    
                    var loc = Stack.Pop();
                    Local ll = meth.Body.Variables.Add(new Local(module.Import(loc.GetType()).ToTypeSig()));
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Stloc, ll);
                case OpCode.String:
                    Stack.Push((string)instr.Operand);
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Ldstr, (string)instr.Operand);
                default:
                    return dnlib.DotNet.Emit.Instruction.Create(OpCodes.Nop);
            }
        }
        public static dnlib.DotNet.Emit.OpCode ldarg(int index)
        {
            switch (index)
            {
                case 0:
                    return OpCodes.Ldarg_0;
                case 1:
                    return OpCodes.Ldarg_1;
                case 2:
                    return OpCodes.Ldarg_2;
                case 3:
                    return OpCodes.Ldarg_3;
                default:
                    return OpCodes.Ldarg_0;
            }
        }
        public static dnlib.DotNet.Emit.OpCode ldloc(int index)
        {
            switch (index)
            {
                case 0:
                    return OpCodes.Ldloc_0;
                case 1:
                    return OpCodes.Ldloc_1;
                case 2:
                    return OpCodes.Ldloc_2;
                case 3:
                    return OpCodes.Ldloc_3;
                default:
                    return OpCodes.Ldloc_0;
            }
        }
        public static dnlib.DotNet.Emit.OpCode stloc(int index)
        {
            switch(index)
            {
                case 0:
                    return OpCodes.Stloc_0;
                case 1:
                    return OpCodes.Stloc_1;
                case 2:
                    return OpCodes.Stloc_2;
                case 3:
                    return OpCodes.Stloc_3;
                default:
                    return OpCodes.Stloc_0;
            }
        }
        public static Dictionary<MethodDef, int> FindMethodIndex(List<MethodDef> Virtualized)
        {
            Dictionary<MethodDef, int> MethodIndex = new Dictionary<MethodDef, int>();
            foreach (MethodDef method in Virtualized)
            {
                for(int i = 0; i < method.Body.Instructions.Count;i++)
                {
                    if (method.Body.Instructions[i].IsLdcI4())
                    {
                        MethodIndex[method] = method.Body.Instructions[i].GetLdcI4Value();
                        break;
                    }

                }
            }
            return MethodIndex;
        }
        public static List<MethodDef> FindVirtualizedMethod(ModuleDefMD module)
        {
            List<MethodDef> VirtualizedMeth = new List<MethodDef>();
            foreach(TypeDef type in module.Types)
            {
                foreach(MethodDef method in type.Methods)
                {
                    if (!method.HasBody || !method.Body.HasInstructions)
                        continue;
                    for(int i = 0; i < method.Body.Instructions.Count-1;i++)
                    {
                        if(method.Body.Instructions[i].OpCode == OpCodes.Call && method.Body.Instructions[i].Operand.ToString().Contains("MemeVM"))
                        {
                            VirtualizedMeth.Add(method);
                            break;
                        }
                    }
                }
            }
            return VirtualizedMeth;
        }
        #region InternalCode
        public static Dictionary<string, Assembly> _references;
        public static List<List<Instruction>> _methods;
        public static void Decompress(Stream resourceStream)
        {
            _references = new Dictionary<string, Assembly>();
            _methods = new List<List<Instruction>>();
            using (DeflateStream deflateStream = new DeflateStream(resourceStream, CompressionMode.Decompress))
            {
                using (BinaryReader binaryReader = new BinaryReader(deflateStream))
                {
                    int num = binaryReader.ReadInt32();
                    for (int i = 0; i < num; i++)
                    {
                        int count = binaryReader.ReadInt32();
                        _references.Add(Encoding.UTF8.GetString(binaryReader.ReadBytes(count)), null);
                    }
                    int num2 = binaryReader.ReadInt32();
                    for (int j = 0; j < num2; j++)
                    {
                        Console.WriteLine("Working with method {0}", _methods.Count);
                        int num3 = binaryReader.ReadInt32();
                        List<Instruction> list = new List<Instruction>();
                        for (int k = 0; k < num3; k++)
                        {
                            OpCode code = (OpCode)binaryReader.ReadByte();
                            Instruction instr = Map.Lookup(code).Deserialize(binaryReader);
                            Console.WriteLine("Recovered OpCodes {0}, with operand {1}", instr.Code, instr.Operand);
                            list.Add(instr);
                        }
                        _methods.Add(list);
                    }
                }
            }
        }
        internal enum OpCode : byte
        {
            // Token: 0x04000008 RID: 8
            Int32,
            // Token: 0x04000009 RID: 9
            Int64,
            // Token: 0x0400000A RID: 10
            Float,
            // Token: 0x0400000B RID: 11
            Double,
            // Token: 0x0400000C RID: 12
            String,
            // Token: 0x0400000D RID: 13
            Null,
            // Token: 0x0400000E RID: 14
            Add,
            // Token: 0x0400000F RID: 15
            Sub,
            // Token: 0x04000010 RID: 16
            Mul,
            // Token: 0x04000011 RID: 17
            Div,
            // Token: 0x04000012 RID: 18
            Rem,
            // Token: 0x04000013 RID: 19
            Dup,
            // Token: 0x04000014 RID: 20
            Pop,
            // Token: 0x04000015 RID: 21
            Jmp,
            // Token: 0x04000016 RID: 22
            Jt,
            // Token: 0x04000017 RID: 23
            Jf,
            // Token: 0x04000018 RID: 24
            Je,
            // Token: 0x04000019 RID: 25
            Jne,
            // Token: 0x0400001A RID: 26
            Jge,
            // Token: 0x0400001B RID: 27
            Jgt,
            // Token: 0x0400001C RID: 28
            Jle,
            // Token: 0x0400001D RID: 29
            Jlt,
            // Token: 0x0400001E RID: 30
            Cmp,
            // Token: 0x0400001F RID: 31
            Cgt,
            // Token: 0x04000020 RID: 32
            Clt,
            // Token: 0x04000021 RID: 33
            Newarr,
            // Token: 0x04000022 RID: 34
            Ldarg,
            // Token: 0x04000023 RID: 35
            Ldloc,
            // Token: 0x04000024 RID: 36
            Ldfld,
            // Token: 0x04000025 RID: 37
            Ldelem,
            // Token: 0x04000026 RID: 38
            Starg,
            // Token: 0x04000027 RID: 39
            Stloc,
            // Token: 0x04000028 RID: 40
            Stfld,
            // Token: 0x04000029 RID: 41
            Stelem,
            // Token: 0x0400002A RID: 42
            Call,
            // Token: 0x0400002B RID: 43
            Ret
        }
        #endregion
    }
    internal interface IHandler
    {
        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600000A RID: 10
        MemeDevirtualizer.Program.OpCode Handles { get; }

        // Token: 0x0600000B RID: 11
        

        // Token: 0x0600000C RID: 12
        Instruction Deserialize(BinaryReader reader);
    }
    internal static class Map
    {
        // Token: 0x0600000E RID: 14 RVA: 0x00002334 File Offset: 0x00000534
        static Map()
        {
         //   Assembly Runtime = Assembly.LoadFrom(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\MemeVM.Runtime.dll");
            foreach (Type type in typeof(Map).Module.GetTypes())
            {
                bool isInterface = type.IsInterface;
                if (!isInterface)
                {
                    bool flag = !typeof(IHandler).IsAssignableFrom(type);
                    if (!flag)
                    {
                        IHandler handler = (IHandler)Activator.CreateInstance(type);
                        Map.OpCodeToHandler.Add(handler.Handles, handler);
                    }
                }
            }
        }

        // Token: 0x0600000F RID: 15 RVA: 0x000023BD File Offset: 0x000005BD
        internal static IHandler Lookup(MemeDevirtualizer.Program.OpCode code)
        {
            return Map.OpCodeToHandler[code];
        }

        // Token: 0x0400002C RID: 44
        private static readonly Dictionary<MemeDevirtualizer.Program.OpCode, IHandler> OpCodeToHandler = new Dictionary<MemeDevirtualizer.Program.OpCode, IHandler>();
    }
   
}
