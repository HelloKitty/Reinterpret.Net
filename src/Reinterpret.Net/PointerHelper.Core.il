#define CORE_ASSEMBLY "System.Runtime"

// Metadata version: v4.0.30319
.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}

.assembly PointerHelper.Core
{
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module PointerHelper.Core.dll

.imagebase           0x00400000
.file alignment      0x00000200
.stackreserve        0x00100000
.subsystem           0x0003       
.corflags            0x00020003  

.class public auto ansi abstract sealed beforefieldinit Reinterpret.Net.PointerHelper
	extends [CORE_ASSEMBLY]System.Object
{
	.method public hidebysig static native unsigned int GetTypeHeaderValue<T> () cil managed aggressiveinlining 
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
		IL_0016: ldfld native unsigned int Reinterpret.Net.PointerHelper/ArrayHeader::'type'
		IL_001b: ret
	}

	.method public hidebysig static void SetTypeAndSize<T>(
			!!T[] values,
			native unsigned int 'type',
			native unsigned int size
		) cil managed aggressiveinlining 
	{
		.locals init (
			[0] native int& pinned p
		)

		IL_0000: ldarg.0
		IL_0001: ldc.i4.0
		IL_0002: ldelema !!T
		IL_0007: stloc.0
		IL_0008: ldloc.0
		IL_0009: conv.i
		IL_000a: sizeof Reinterpret.Net.PointerHelper/ArrayHeader
		IL_0010: sub
		IL_0011: dup
		IL_0012: ldarg.1
		IL_0013: stfld native unsigned int Reinterpret.Net.PointerHelper/ArrayHeader::'type'
		IL_0018: ldarg.2
		IL_0019: stfld native unsigned int Reinterpret.Net.PointerHelper/ArrayHeader::length
		IL_001e: ldc.i4.0
		IL_001f: conv.u
		IL_0020: stloc.0
		IL_0021: ret
	}

	.class nested assembly sequential ansi sealed beforefieldinit ArrayHeader
		extends [CORE_ASSEMBLY]System.ValueType
	{
		.pack 1
		.size 0

		.field public native unsigned int 'type'

		.field public native unsigned int length
	}
}