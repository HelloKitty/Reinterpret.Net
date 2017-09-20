.assembly extern mscorlib
{
  .publickeytoken = (7C EC 85 D7 BE A7 79 8E)
  .ver 2:0:5:0
}
.assembly PointerHelper.Netframework
{
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module PointerHelper.Netframework.dll

.imagebase           0x00400000
.file alignment      0x00000200
.stackreserve        0x00100000
.subsystem           0x0003       
.corflags            0x00020003  

.class public auto ansi abstract sealed beforefieldinit Reinterpret.Net.PointerHelper
	extends [mscorlib]System.Object
{
		.method public hidebysig specialname rtspecialname instance void .ctor () cil managed 
	{
		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ret
	}

	.method public hidebysig static valuetype [mscorlib]System.UIntPtr GetTypeHeaderValue<T> () cil managed aggressiveinlining 
	{
		.locals init (
			[0] native int& pinned p
		)

		IL_0000: ldc.i4.1
		IL_0001: newarr !!T
		IL_0006: ldc.i4.0
		IL_0007: ldelema !!T
		IL_000c: stloc.0
		IL_000d: ldloc.0
		IL_000e: conv.i
		IL_000f: sizeof Reinterpret.Net.PointerHelper/ArrayHeader
		IL_0015: sub
		IL_0016: ldfld valuetype [mscorlib]System.UIntPtr Reinterpret.Net.PointerHelper/ArrayHeader::'type'
		IL_001b: ret
	}

	.method public hidebysig static void SetTypeAndSize (
			uint8[] bytes,
			valuetype [mscorlib]System.UIntPtr 'type',
			valuetype [mscorlib]System.UIntPtr size
		) cil managed aggressiveinlining 
	{
		.locals init (
			[0] native int& pinned p
		)

		IL_0000: ldarg.0
		IL_0001: ldc.i4.0
		IL_0002: ldelema [mscorlib]System.Byte
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: conv.i
		IL_000a: sizeof Reinterpret.Net.PointerHelper/ArrayHeader
		IL_0010: sub
		IL_0011: dup
		IL_0012: ldarg.1
		IL_0013: stfld valuetype [mscorlib]System.UIntPtr Reinterpret.Net.PointerHelper/ArrayHeader::'type'
		IL_0018: ldarg.2
		IL_0019: stfld valuetype [mscorlib]System.UIntPtr Reinterpret.Net.PointerHelper/ArrayHeader::length
		IL_001e: ldc.i4.0
		IL_001f: conv.u
		IL_0020: stloc.0
		IL_0021: ret
	}

	.class nested assembly sequential ansi sealed beforefieldinit ArrayHeader
		extends [mscorlib]System.ValueType
	{
		.pack 1
		.size 0

		.field public valuetype [mscorlib]System.UIntPtr 'type'

		.field public valuetype [mscorlib]System.UIntPtr length
	}
}