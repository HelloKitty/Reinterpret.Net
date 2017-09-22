.assembly extern mscorlib
{
  auto
}
.assembly UnsafeAs.BackportNoAggressive
{
	.hash algorithm 0x00008004
	.ver 1:0:0:0

	.custom instance void [mscorlib]System.Runtime.CompilerServices.InternalsVisibleToAttribute::.ctor(string) =
	{
		string( 'Reinterpret.Net' )
	}
}

.module UnsafeAs.BackportNoAggressive.dll

.imagebase           0x00400000
.file alignment      0x00000200
.stackreserve        0x00100000
.subsystem           0x0003       
.corflags 0x00000001    //  ILONLY

.class private auto ansi abstract sealed beforefieldinit Reinterpret.Net.Unsafe
	extends [mscorlib]System.Object
{
	.method public hidebysig static !!TTo& As<TFrom, TTo>(!!TFrom& source) cil managed
	{
		ldarg.0
		ret
	} // end of method Unsafe::As

	.method public hidebysig static !!T ReadUnaligned<T>(uint8& source) cil managed
	{
		ldarg.0        
		unaligned. 0x1
		ldobj !!T
		ret
	} // end of method Unsafe::ReadUnaligned

	.method public hidebysig static void* AsPointer<T>(!!T& 'value') cil managed
	{
		ldarg.0
		conv.u
		ret
	} // end of method Unsafe::AsPointer
}