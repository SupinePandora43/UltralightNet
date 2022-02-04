#nullable enable
#pragma warning disable 0649

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	internal sealed class GeneratedDllImportAttribute : Attribute
	{
		public bool BestFitMapping;
		public CallingConvention CallingConvention;
		public CharSet CharSet;
		public string? EntryPoint;
		public bool ExactSpelling;
		public bool PreserveSig;
		public bool SetLastError;
		public bool ThrowOnUnmappableChar;

		public GeneratedDllImportAttribute(string dllName)
		{
			this.Value = dllName;
		}

		public string Value { get; private set; }
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	internal class GeneratedMarshallingAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Struct)]
	internal class BlittableTypeAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
	internal class NativeMarshallingAttribute : Attribute
	{
		public NativeMarshallingAttribute(Type nativeType)
		{
			NativeType = nativeType;
		}

		public Type NativeType { get; }
	}

	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Field)]
	internal class MarshalUsingAttribute : Attribute
	{
		public MarshalUsingAttribute(Type nativeType)
		{
			NativeType = nativeType;
		}

		public Type NativeType { get; }
	}
}
#if NETFRAMEWORK || NETSTANDARD
namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Interface, Inherited = false)]
	internal sealed class SkipLocalsInitAttribute : Attribute
	{
		public SkipLocalsInitAttribute() { }
	}
}
namespace System
{
	internal static class OperatingSystem {
		public static bool IsWindows() =>
#if NETFRAMEWORK
			true;
#else
			System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
#endif
	}
}
#endif
#pragma warning restore 0649
#nullable restore
