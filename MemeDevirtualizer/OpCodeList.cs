using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MemeDevirtualizer.Program;

namespace MemeDevirtualizer
{
    internal struct Instruction
    {
        // Token: 0x0600000D RID: 13 RVA: 0x00002320 File Offset: 0x00000520
        internal Instruction(OpCode code, object op = null)
        {
            this.Code = code;
            this.Operand = op;
        }

        // Token: 0x04000005 RID: 5
        internal OpCode Code;

        // Token: 0x04000006 RID: 6
        internal object Operand;
    }
    internal class Add : IHandler
    {
        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000010 RID: 16 RVA: 0x000023CA File Offset: 0x000005CA
        public OpCode Handles
        {
            get
            {
                return OpCode.Add;
            }
        }

  

        // Token: 0x06000012 RID: 18 RVA: 0x000024AC File Offset: 0x000006AC
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Add, null);
        }
    }
    internal class Call : IHandler
    {
        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000014 RID: 20 RVA: 0x000024BE File Offset: 0x000006BE
        public OpCode Handles
        {
            get
            {
                return OpCode.Call;
            }
        }


        // Token: 0x06000017 RID: 23 RVA: 0x00002684 File Offset: 0x00000884
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Call, new Tuple<short, int, bool>(reader.ReadInt16(), reader.ReadInt32(), reader.ReadBoolean()));
        }
    }
    internal class Cgt : IHandler
    {
        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000019 RID: 25 RVA: 0x000026AD File Offset: 0x000008AD
        public OpCode Handles
        {
            get
            {
                return OpCode.Cgt;
            }
        }
        // Token: 0x0600001B RID: 27 RVA: 0x00002791 File Offset: 0x00000991
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Cgt, null);
        }
    }
    internal class Clt : IHandler
    {
        // Token: 0x17000006 RID: 6
        // (get) Token: 0x0600001D RID: 29 RVA: 0x000027A4 File Offset: 0x000009A4
        public OpCode Handles
        {
            get
            {
                return OpCode.Clt;
            }
        }

      

        // Token: 0x0600001F RID: 31 RVA: 0x00002885 File Offset: 0x00000A85
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Clt, null);
        }
    }
    internal class Cmp : IHandler
    {
        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000021 RID: 33 RVA: 0x00002898 File Offset: 0x00000A98
        public OpCode Handles
        {
            get
            {
                return OpCode.Cmp;
            }
        }

    

        // Token: 0x06000023 RID: 35 RVA: 0x00002979 File Offset: 0x00000B79
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Cmp, null);
        }
    }
    internal class Dup : IHandler
    {
        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000025 RID: 37 RVA: 0x0000298C File Offset: 0x00000B8C
        public OpCode Handles
        {
            get
            {
                return OpCode.Dup;
            }
        }

        // Token: 0x06000027 RID: 39 RVA: 0x000029C4 File Offset: 0x00000BC4
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Dup, null);
        }
    }
    // Token: 0x02000010 RID: 16
    internal class Int : IHandler
    {
        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000029 RID: 41 RVA: 0x000029D7 File Offset: 0x00000BD7
        public OpCode Handles
        {
            get
            {
                return OpCode.Int32;
            }
        }
        // Token: 0x0600002B RID: 43 RVA: 0x000029EE File Offset: 0x00000BEE
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Int32, reader.ReadInt32());
        }
    }
    internal class Jf : IHandler
    {
        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600002D RID: 45 RVA: 0x00002A0A File Offset: 0x00000C0A
        public OpCode Handles
        {
            get
            {
                return OpCode.Jf;
            }
        }

      

        // Token: 0x0600002F RID: 47 RVA: 0x00002A4E File Offset: 0x00000C4E
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Jf, reader.ReadInt32());
        }
    }
    internal class Jmp : IHandler
    {
        // Token: 0x1700000B RID: 11
        // (get) Token: 0x06000031 RID: 49 RVA: 0x00002A6B File Offset: 0x00000C6B
        public OpCode Handles
        {
            get
            {
                return OpCode.Jmp;
            }
        }

        
        // Token: 0x06000033 RID: 51 RVA: 0x00002A82 File Offset: 0x00000C82
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Jmp, reader.ReadInt32());
        }
    }
    internal class Jt : IHandler
    {
        // Token: 0x1700000C RID: 12
        // (get) Token: 0x06000035 RID: 53 RVA: 0x00002A9F File Offset: 0x00000C9F
        public OpCode Handles
        {
            get
            {
                return OpCode.Jt;
            }
        }

      

        // Token: 0x06000037 RID: 55 RVA: 0x00002ADF File Offset: 0x00000CDF
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Jt, reader.ReadInt32());
        }
    }
    internal class Ldarg : IHandler
    {
        // Token: 0x1700000D RID: 13
        // (get) Token: 0x06000039 RID: 57 RVA: 0x00002AFC File Offset: 0x00000CFC
        public OpCode Handles
        {
            get
            {
                return OpCode.Ldarg;
            }
        }

      

        // Token: 0x0600003B RID: 59 RVA: 0x00002B20 File Offset: 0x00000D20
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Ldarg, reader.ReadInt16());
        }
    }
    internal class Ldfld : IHandler
    {
        // Token: 0x1700000E RID: 14
        // (get) Token: 0x0600003D RID: 61 RVA: 0x00002B3D File Offset: 0x00000D3D
        public OpCode Handles
        {
            get
            {
                return OpCode.Ldfld;
            }
        }

     

        // Token: 0x0600003F RID: 63 RVA: 0x00002BB5 File Offset: 0x00000DB5
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Ldfld, new Tuple<short, int>(reader.ReadInt16(), reader.ReadInt32()));
        }
    }
    internal class Ldloc : IHandler
    {
        // Token: 0x1700000F RID: 15
        // (get) Token: 0x06000041 RID: 65 RVA: 0x00002BD8 File Offset: 0x00000DD8
        public OpCode Handles
        {
            get
            {
                return OpCode.Ldloc;
            }
        }

     

        // Token: 0x06000043 RID: 67 RVA: 0x00002C00 File Offset: 0x00000E00
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Ldloc, reader.ReadInt16());
        }
    }
    internal class Long : IHandler
    {
        // Token: 0x17000010 RID: 16
        // (get) Token: 0x06000045 RID: 69 RVA: 0x00002C1D File Offset: 0x00000E1D
        public OpCode Handles
        {
            get
            {
                return OpCode.Int64;
            }
        }

     

        // Token: 0x06000047 RID: 71 RVA: 0x00002C34 File Offset: 0x00000E34
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Int64, reader.ReadInt64());
        }
    }
    internal class Newarr : IHandler
    {
        // Token: 0x17000011 RID: 17
        // (get) Token: 0x06000049 RID: 73 RVA: 0x00002C50 File Offset: 0x00000E50
        public OpCode Handles
        {
            get
            {
                return OpCode.Newarr;
            }
        }

    

        // Token: 0x0600004B RID: 75 RVA: 0x00002CB9 File Offset: 0x00000EB9
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Newarr, new Tuple<short, int>(reader.ReadInt16(), reader.ReadInt32()));
        }
    }
    internal class Null : IHandler
    {
        // Token: 0x17000012 RID: 18
        // (get) Token: 0x0600004D RID: 77 RVA: 0x00002CDC File Offset: 0x00000EDC
        public OpCode Handles
        {
            get
            {
                return OpCode.Null;
            }
        }


        // Token: 0x0600004F RID: 79 RVA: 0x00002CEE File Offset: 0x00000EEE
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Null, null);
        }
    }
    internal class Pop : IHandler
    {
        // Token: 0x17000013 RID: 19
        // (get) Token: 0x06000051 RID: 81 RVA: 0x00002D00 File Offset: 0x00000F00
        public OpCode Handles
        {
            get
            {
                return OpCode.Pop;
            }
        }

    

        // Token: 0x06000053 RID: 83 RVA: 0x00002D12 File Offset: 0x00000F12
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Pop, null);
        }
    }
    internal class Ret : IHandler
    {
        // Token: 0x17000014 RID: 20
        // (get) Token: 0x06000055 RID: 85 RVA: 0x00002D25 File Offset: 0x00000F25
        public OpCode Handles
        {
            get
            {
                return OpCode.Ret;
            }
        }

     

        // Token: 0x06000057 RID: 87 RVA: 0x00002D32 File Offset: 0x00000F32
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Ret, null);
        }
    }
    internal class Stfld : IHandler
    {
        // Token: 0x17000015 RID: 21
        // (get) Token: 0x06000059 RID: 89 RVA: 0x00002D45 File Offset: 0x00000F45
        public OpCode Handles
        {
            get
            {
                return OpCode.Stfld;
            }
        }


        // Token: 0x0600005B RID: 91 RVA: 0x00002DC8 File Offset: 0x00000FC8
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Stfld, new Tuple<short, int>(reader.ReadInt16(), reader.ReadInt32()));
        }
    }
    internal class Stloc : IHandler
    {
        // Token: 0x17000016 RID: 22
        // (get) Token: 0x0600005D RID: 93 RVA: 0x00002DEB File Offset: 0x00000FEB
        public OpCode Handles
        {
            get
            {
                return OpCode.Stloc;
            }
        }


        // Token: 0x0600005F RID: 95 RVA: 0x00002E13 File Offset: 0x00001013
        public Instruction Deserialize(BinaryReader reader)
        {
            return new Instruction(OpCode.Stloc, reader.ReadInt16());
        }
    }
    internal class String : IHandler
    {
        // Token: 0x17000017 RID: 23
        // (get) Token: 0x06000061 RID: 97 RVA: 0x00002E30 File Offset: 0x00001030
        public OpCode Handles
        {
            get
            {
                return OpCode.String;
            }
        }

  

        // Token: 0x06000063 RID: 99 RVA: 0x00002E48 File Offset: 0x00001048
        public Instruction Deserialize(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            return new Instruction(OpCode.String, Encoding.UTF8.GetString(reader.ReadBytes(count)));
        }
    }
}
