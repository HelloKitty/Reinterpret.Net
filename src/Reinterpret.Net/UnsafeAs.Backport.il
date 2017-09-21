.assembly extern mscorlib
{
  .publickeytoken = (7C EC 85 D7 BE A7 79 8E)
  .ver 2:0:5:0
}
.assembly UnsafeAs.Backport
{
	.hash algorithm 0x00008004
	.ver 1:0:0:0

	.custom instance void [mscorlib]System.Runtime.CompilerServices.InternalsVisibleToAttribute::.ctor(string) =
	{
		string( 'Reinterpret.Net' )
	}
}

.module UnsafeAs.Backport.dll

.imagebase           0x00400000
.file alignment      0x00000200
.stackreserve        0x00100000
.subsystem           0x0003       
.corflags            0x00020003  

.class private auto ansi abstract sealed beforefieldinit Reinterpret.Net.Unsafe
	extends [mscorlib]System.Object
{
	.method public hidebysig static !!TTo& As<TFrom, TTo>(!!TFrom& source) cil managed aggressiveinlining
	{
		.maxstack 1
		ldarg.0
		ret
	} // end of method Unsafe::As

	.method public hidebysig static !!T ReadUnaligned<T>(uint8& source) cil managed aggressiveinlining
	{
		.maxstack 1
		ldarg.0        
		unaligned. 0x1
		ldobj !!T
		ret
	} // end of method Unsafe::ReadUnaligned

	.method public hidebysig static void* AsPointer<T>(!!T& 'value') cil managed aggressiveinlining
	{
		.maxstack 1
		ldarg.0
		conv.u
		ret
	} // end of method Unsafe::AsPointer
}