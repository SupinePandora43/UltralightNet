using System;
using System.Diagnostics;

namespace UltralightNet
{
	[Conditional("DEBUG")]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
	internal class NativeTypeNameAttribute : Attribute
	{
		public NativeTypeNameAttribute(string _) { }
	}
}
