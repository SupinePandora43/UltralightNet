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
#if !NET7_0_OR_GREATER
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	internal class GeneratedMarshallingAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Struct)]
	internal class BlittableTypeAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Delegate)]
	internal class NativeMarshallingAttribute : Attribute
	{
		public NativeMarshallingAttribute(Type nativeType)
		{
			NativeType = nativeType;
		}

		public Type NativeType { get; }
	}
#endif
#if !NET7_0_OR_GREATER
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	internal class MarshalUsingAttribute : Attribute
	{
		public MarshalUsingAttribute()
		{
			CountElementName = string.Empty;
		}
		public MarshalUsingAttribute(Type nativeType) : this()
		{
			NativeType = nativeType;
		}

		public Type? NativeType { get; }
		public string CountElementName { get; set; }
		public int ConstantElementCount { get; set; }
		public int ElementIndirectionDepth { get; set; }
		public const string ReturnsCountValue = "return-value";
	}
#endif

#if !NET7_0_OR_GREATER

	internal enum CustomTypeMarshallerKind
	{
		Value,
		LinearCollection
	}
	[Flags]
	internal enum CustomTypeMarshallerFeatures
	{
		None = 0,
		UnmanagedResources = 0x1,
		CallerAllocatedBuffer = 0x2,
		TwoStageMarshalling = 0x4
	}
	[Flags]
	internal enum CustomTypeMarshallerDirection
	{
		None = 0,
		In = 0x1,
		Out = 0x2,
		Ref = In | Out
	}

	[AttributeUsage(AttributeTargets.Struct)]
	internal sealed class CustomTypeMarshallerAttribute : Attribute
	{
		public CustomTypeMarshallerAttribute(Type managedType, CustomTypeMarshallerKind marshallerKind = CustomTypeMarshallerKind.Value)
		{
			ManagedType = managedType;
			MarshallerKind = marshallerKind;
		}

		public Type ManagedType { get; set; }
		public CustomTypeMarshallerKind MarshallerKind { get; set; }
		public int BufferSize { get; set; }
		public CustomTypeMarshallerDirection Direction { get; set; }
		public CustomTypeMarshallerFeatures Features { get; set; }
		public struct GenericPlaceholder
		{

		}
	}

	[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
	internal sealed class DisableRuntimeMarshallingAttribute : Attribute
	{
		public DisableRuntimeMarshallingAttribute() { }
	}

	internal enum StringMarshalling {
		Custom = 0,
		Utf8,
		Utf16
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	internal sealed class LibraryImportAttribute : Attribute {
		public LibraryImportAttribute(string libraryName){
			LibraryName = libraryName;
		}
		public string LibraryName {get;}
		public string? EntryPoint {get;set;}
		public StringMarshalling StringMarshalling {get;set;}
		public Type? StringMarshallingCustomType {get;set;}
		public bool SetLastError {get;set;}
	}

#endif
}

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
#nullable restore
