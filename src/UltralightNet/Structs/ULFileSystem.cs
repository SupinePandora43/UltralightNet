using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet;

// INTEROPTODO: TEST
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ULFileSystem : IDisposable, IEquatable<ULFileSystem>
{
	public static nuint InvalidFileHandle =>
#if NET5_0_OR_GREATER
		nuint.MaxValue; // -1 is equal to MaxValue. Don't ask me how - i don't know either.
#else
			sizeof(void*) == 8 ? unchecked((nuint)ulong.MaxValue) : uint.MaxValue;
#endif

	public ULFileSystemFileExistsCallback? FileExists
	{
		set => _FileExists = value is null ? null : (path) => Unsafe.As<bool, byte>(ref Unsafe.AsRef(value(ULString.NativeToManaged(path))));
		readonly get
		{
			var c = _FileExists;
			return c is null ? null : (in string path) =>
			{
				using ULString pathNative = new(path.AsSpan());
				return Unsafe.As<byte, bool>(ref Unsafe.AsRef(c(&pathNative)));
			};
		}
	}
	public ULFileSystemGetFileMimeTypeCallback? GetFileMimeType
	{
		set => _GetFileMimeType = value is null ? null : (path) => new ULString(value(ULString.NativeToManaged(path)).AsSpan()).Allocate();
		readonly get
		{
			var c = _GetFileMimeType;
			return c is null ? null : (in string path) =>
			{
				using ULString uPath = new ULString(path.AsSpan());
				ULString* mime = c(&uPath);
				string retVal = ULString.NativeToManaged(mime);
				mime->Deallocate();
				return retVal;
			};
		}
	}
	public ULFileSystemGetFileCharsetCallback? GetFileCharset
	{
		set => _GetFileCharset = value is null ? null : (path) => new ULString(value(ULString.NativeToManaged(path)).AsSpan()).Allocate();
		readonly get
		{
			var c = _GetFileCharset;
			return c is null ? null : (in string path) =>
			{
				using ULString uPath = new ULString(path.AsSpan());
				ULString* mime = c(&uPath);
				string retVal = ULString.NativeToManaged(mime);
				mime->Deallocate();
				return retVal;
			};
		}
	}
	public ULFileSystemOpenFileCallback? OpenFile
	{
		set => _OpenFile = value is null ? null : (path) =>
		{
			byte[]? result = value(ULString.NativeToManaged(path));
			if (result is not null) return ULBuffer.CreateFrom<byte>(result);
			else return null;
		};
		readonly get
		{
			var c = _OpenFile;
			return c is null ? null : (in string path) =>
			{
				using ULString pathNative = new(path.AsSpan());
				ULBuffer* buffer = c(&pathNative);
				try
				{
					byte[] bytes =
#if NET5_0_OR_GREATER
						GC.AllocateUninitializedArray<byte>(checked((int)buffer->Size));
#else
						new byte[checked((int)buffer->Size)];
#endif
					new ReadOnlySpan<byte>(buffer->Data, checked((int)buffer->Size)).CopyTo(bytes);
					return bytes;
				}
				finally
				{
					buffer->Destroy();
				}
			};
		}
	}
	public ULFileSystemFileExistsCallback__PInvoke__? _FileExists
	{
		set => ULPlatform.Handle(ref this, this with { __FileExists = value is null ? null : (delegate* unmanaged[Cdecl]<ULString*, byte>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __FileExists;
			return p is null ? null : (path) => p(path);
		}
	}
	public ULFileSystemGetFileMimeTypeCallback__PInvoke__? _GetFileMimeType
	{
		set => ULPlatform.Handle(ref this, this with { __GetFileMimeType = value is null ? null : (delegate* unmanaged[Cdecl]<ULString*, ULString*>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __GetFileMimeType;
			return p is null ? null : (path) => p(path);
		}
	}
	public ULFileSystemGetFileCharsetCallback__PInvoke__? _GetFileCharset
	{
		set => ULPlatform.Handle(ref this, this with { __GetFileCharset = value is null ? null : (delegate* unmanaged[Cdecl]<ULString*, ULString*>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __GetFileCharset;
			return p is null ? null : (path) => p(path);
		}
	}
	public ULFileSystemOpenFileCallback__PInvoke__? _OpenFile
	{
		set => ULPlatform.Handle(ref this, this with { __OpenFile = value is null ? null : (delegate* unmanaged[Cdecl]<ULString*, ULBuffer*>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __OpenFile;
			return p is null ? null : (path) => p(path);
		}
	}

	public delegate* unmanaged[Cdecl]<ULString*, byte> __FileExists;
	public delegate* unmanaged[Cdecl]<ULString*, ULString*> __GetFileMimeType;
	public delegate* unmanaged[Cdecl]<ULString*, ULString*> __GetFileCharset;
	public delegate* unmanaged[Cdecl]<ULString*, ULBuffer*> __OpenFile;

	public void Dispose()
	{
		ULPlatform.Free(this);
	}
#pragma warning disable CS8909
	public readonly bool Equals(ULFileSystem other) => __FileExists == other.__FileExists && __GetFileMimeType == other.__GetFileMimeType && __GetFileCharset == other.__GetFileCharset && __OpenFile == other.__OpenFile;
#pragma warning restore CS8909
	public readonly override bool Equals(object? other) => other is ULFileSystem fileSystem ? Equals(fileSystem) : false;

#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER
	public readonly override int GetHashCode() => HashCode.Combine((nuint)__FileExists, (nuint)__GetFileMimeType, (nuint)__GetFileCharset, (nuint)__OpenFile);
#endif
}
