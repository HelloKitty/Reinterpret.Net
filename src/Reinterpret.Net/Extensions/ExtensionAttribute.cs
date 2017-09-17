//To use Extension Methods in .NET 2.0 we must use the below hack explained by Jon Skeet
//This is an attribute that is missing in net20 that must be added to be available
//http://csharpindepth.com/Articles/Chapter1/Versions.aspx
#if NET20 || NET30
namespace System.Runtime.CompilerServices
{
	[AttributeUsageAttribute(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
	public class ExtensionAttribute : Attribute
	{
	}
}
#endif