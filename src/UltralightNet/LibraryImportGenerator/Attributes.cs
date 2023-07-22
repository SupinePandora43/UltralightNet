#pragma warning disable 0649

#if NETFRAMEWORK || NETSTANDARD
namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Interface, Inherited = false)]
	internal sealed class SkipLocalsInitAttribute : Attribute
	{
		public SkipLocalsInitAttribute() { }
	}
	internal sealed class IsExternalInit {}
}
#if !NETSTANDARD2_1
namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class NotNullWhenAttribute : Attribute
	{
		public NotNullWhenAttribute(bool ReturnValue) { }
	}
}
#endif
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

#if !NET7_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
	internal sealed class DisableRuntimeMarshallingAttribute : Attribute
	{
		public DisableRuntimeMarshallingAttribute() { }
	}
}
namespace System.Runtime.InteropServices
{
	namespace Marshalling
	{
		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
		internal sealed class CustomMarshallerAttribute : Attribute
		{
			public CustomMarshallerAttribute(Type managedType, MarshalMode marshalMode, Type marshallerType)
			{
				ManagedType = managedType;
				MarshalMode = marshalMode;
				MarshallerType = marshallerType;
			}
			public Type ManagedType { get; }
			public MarshalMode MarshalMode { get; }
			public Type MarshallerType { get; }
			public struct GenericPlaceholder { }
		}
		internal enum MarshalMode
		{
			Default = 0,
			ManagedToUnmanagedIn = 1,
			ManagedToUnmanagedRef = 2,
			ManagedToUnmanagedOut = 3,
			UnmanagedToManagedIn = 4,
			UnmanagedToManagedRef = 5,
			UnmanagedToManagedOut = 6,
			ElementIn = 7,
			ElementRef = 8,
			ElementOut = 9
		}
		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate)]
		internal sealed class NativeMarshallingAttribute : Attribute
		{
			public NativeMarshallingAttribute(Type nativeType) { NativeType = nativeType; }
			public Type NativeType { get; }
		}
		[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
		public sealed class MarshalUsingAttribute : Attribute
		{
			public const string ReturnsCountValue = "return-value";
			public MarshalUsingAttribute() { }
			public MarshalUsingAttribute(Type nativeType) { NativeType = nativeType; }
			public Type? NativeType { get; }
			public string CountElementName { get; set; } = ReturnsCountValue;
			public int ConstantElementCount { get; set; }
			public int ElementIndirectionDepth { get; set; }
		}
	}
	internal enum StringMarshalling
	{
		Custom = 0,
		Utf8 = 1,
		Utf16 = 2
	}
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	internal sealed class LibraryImportAttribute : Attribute
	{
		public LibraryImportAttribute(string libraryName) { LibraryName = libraryName; }
		public string LibraryName { get; }
		public string? EntryPoint { get; set; }
		public bool SetLastError { get; set; }
		public StringMarshalling StringMarshalling { get; set; }
		public Type? StringMarshallingCustomType { get; set; }
	}
}
#endif
