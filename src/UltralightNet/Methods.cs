using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

[assembly: InternalsVisibleTo("UltralightNet.AppCore")]
[assembly: DisableRuntimeMarshallingAttribute]
[assembly: AssemblyMetadata("IsTrimmable", "True")]

#if RELEASE
[module: SkipLocalsInit]
#endif

namespace UltralightNet;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
public static partial class Methods
{
	public const string LibUltralight = "Ultralight";

	static Methods() => Preload();

	[GeneratedDllImport(LibUltralight)]
	[return: MarshalUsing(typeof(UTF8Marshaller))]
	public static partial string ulVersionString();

	[DllImport(LibUltralight)]
	public static extern uint ulVersionMajor();

	[DllImport(LibUltralight)]
	public static extern uint ulVersionMinor();

	[DllImport(LibUltralight)]
	public static extern uint ulVersionPatch();

	/// <summary>
	/// Preload Ultralight binaries on OSX/MacOS
	/// </summary>
	/// <remarks>UltralightCore, WebCore, Ultralight</remarks>
	public static void Preload()
	{
#if !NETFRAMEWORK
#if NET5_0_OR_GREATER
		bool isLinux = OperatingSystem.IsLinux();
		bool isOSX = OperatingSystem.IsMacOS();
#else
			bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
			bool isOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#endif
		if (isLinux || isOSX)
		{
			ReadOnlySpan<string> libsLinux = new[] { "libglib-2.0.so.0.6800.3", "libgthread-2.0.so.0.6800.3", "libgobject-2.0.so.0.6800.3", "libgmodule-2.0.so.0.6800.3", "libgio-2.0.so.0.6800.3", /* --- */ "libgstreamer-full-1.0.so", "libUltralightCore.so", "libWebCore.so", "libUltralight.so" };
			ReadOnlySpan<string> libsOSX = new[] { "libgstreamer-full-1.0.dylib", "libUltralightCore.dylib", "libWebCore.dylib", "libUltralight.dylib" };

			string? absoluteAssemblyLocationDir = Path.GetDirectoryName(typeof(Methods).Assembly.Location);
			if (string.IsNullOrEmpty(absoluteAssemblyLocationDir)) absoluteAssemblyLocationDir = Path.GetDirectoryName(
#if NET6_0_OR_GREATER
				Environment.ProcessPath ??
#endif
				"NonExistantFolder") ?? "AnotherNonExistantFolder";
			string absoluteRuntimeNativesDir = Path.Combine(absoluteAssemblyLocationDir, "runtimes", isLinux ? "linux-x64" : "osx-x64", "native");

#if !NETSTANDARD
			Assembly assembly = typeof(Methods).Assembly;
			DllImportSearchPath searchPath =
				DllImportSearchPath.UseDllDirectoryForDependencies |
				DllImportSearchPath.AssemblyDirectory |
				DllImportSearchPath.ApplicationDirectory;
#endif
			foreach (string lib in (isLinux ? libsLinux : libsOSX))
			{
				string absoluteRuntimeNative = Path.Combine(absoluteRuntimeNativesDir, lib);
				if (File.Exists(absoluteRuntimeNative))
				{
					NativeLibrary.Load(absoluteRuntimeNative
#if !NETSTANDARD
						, assembly, searchPath
#endif
						);
					continue;
				}
				else
				{
					string absoluteAssemblyLocation = Path.Combine(absoluteAssemblyLocationDir, lib);
					if (File.Exists(absoluteAssemblyLocation))
					{
						NativeLibrary.Load(absoluteAssemblyLocation
#if !NETSTANDARD
							, assembly, searchPath
#endif
							);
					}
					else
						try
						{
							NativeLibrary.Load(lib
#if !NETSTANDARD
								, assembly, searchPath
#endif
								); // last hope (will not work)
						}
						catch (DllNotFoundException)
						{
#if DEBUG
							Console.WriteLine($"UltralightNet: failed to load {lib}");
#endif
						} // will cause DllNotFoundException somewhere else
				}
			}
		}
#endif
	}

#if NETSTANDARD
		private static partial class NativeLibrary
		{
			public static IntPtr Load(string libraryPath) => dlopen(libraryPath, 0x002); // RTLD_NOW

			// LPUTF8Str = 48
			[DllImport("libdl")]
			private static extern IntPtr dlopen([MarshalAs(48)] string path, int mode);
		}
#endif
}
// INTEROPTODO: CUSTOMTYPEMARSHALLER
internal unsafe ref struct UTF8Marshaller
{
	public const int BufferSize = 0x100;

	public byte* bytes;
	public Span<byte> span;

	public UTF8Marshaller(string str)
	{
		span = null;
		int strLen = str.Length;
		int byteCount = (strLen + 1) * 3 + 1;
		bytes = (byte*)NativeMemory.Alloc((nuint)byteCount);

#if NETSTANDARD2_1 || NET
		int written = Encoding.UTF8.GetBytes(str.AsSpan(), new Span<byte>(bytes, byteCount));
#else
			int written;
			fixed(char* characterPtr = str){
				written = Encoding.UTF8.GetBytes(characterPtr, strLen, bytes, byteCount);
			}
#endif
		bytes[written] = 0;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UTF8Marshaller(string str, Span<byte> buffer)
	{
		int strLen = str.Length;
		int byteCount = checked((strLen + 1) * 3 + 1);

		if (buffer.Length >= byteCount)
		{
			bytes = null;
			span = buffer;
#if NETSTANDARD2_1 || NET
			int written = Encoding.UTF8.GetBytes(str.AsSpan(), span);
			span[written] = 0;
#else
				int written;
				fixed(char* characterPtr = str)
				fixed(byte* bytePtr = span){
					written = Encoding.UTF8.GetBytes(characterPtr, strLen, bytePtr, byteCount);
					bytePtr[written] = 0;
				}
#endif
		}
		else
		{
			span = null;
			bytes = (byte*)NativeMemory.Alloc((nuint)byteCount);

#if NETSTANDARD2_1 || NET
			int written = Encoding.UTF8.GetBytes(str.AsSpan(), new Span<byte>(bytes, byteCount));
#else
				int written;
				fixed(char* characterPtr = str){
					written = Encoding.UTF8.GetBytes(characterPtr, strLen, bytes, byteCount);
				}
#endif
			bytes[written] = 0;
		}
	}

	public readonly ref byte GetPinnableReference()
	{
		if (bytes is not null) return ref *bytes;
		return ref span.GetPinnableReference();
	}

	public readonly string ToManaged() =>
#if NETSTANDARD2_1 || NETCOREAPP1_1_OR_GREATER
		Marshal.PtrToStringUTF8((IntPtr)bytes)!;
#else
			new((sbyte*)bytes);
#endif

	public unsafe byte* Value
	{
		readonly get => bytes is not null ? bytes : (byte*)Unsafe.AsPointer(ref span[0]);
		set
		{
			bytes = value;
			span = default;
		}
	}

	public void FreeNative()
	{
		if (bytes is not null)
			NativeMemory.Free(bytes);
		bytes = null;
	}
}
