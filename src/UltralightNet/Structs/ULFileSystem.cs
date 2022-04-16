using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	// INTEROPTODO: TEST
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULFileSystem : IDisposable
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
			get
			{
				var c = _FileExists;
				return c is null ? null : (in string path) =>
				{
					using ULStringDisposableStackToNativeMarshaller marshaller = new(path);
					return Unsafe.As<byte, bool>(ref Unsafe.AsRef(c(marshaller.Native)));
				};
			}
		}
		public ULFileSystemGetFileSizeCallback? GetFileSize
		{
			set => _GetFileSize = value is null ? null : (handle, result) => Unsafe.As<bool, byte>(ref Unsafe.AsRef(value(handle, out Unsafe.AsRef<long>(result))));
			get
			{
				var c = _GetFileSize;
				return c is null ? null : (nuint fileHandle, out long size) =>
				{
					fixed (long* sizePtr = &size)
						return Unsafe.As<byte, bool>(ref Unsafe.AsRef(c(fileHandle, sizePtr)));
				};
			}
		}
		public ULFileSystemGetFileMimeTypeCallback? GetFileMimeType
		{
			set => _GetFileMimeType = value is null ? null : (path, resultUlStrPtr) =>
				{
					bool returnValue = value(ULString.NativeToManaged(path), out string result);

					using ULStringDisposableStackToNativeMarshaller marshaller = new(result);
					Methods.ulStringAssignString(resultUlStrPtr, marshaller.Native);

					return Unsafe.As<bool, byte>(ref returnValue);
				};
			get
			{
				var c = _GetFileMimeType;
				return c is null ? null : (in string path, out string result) =>
				{
					using ULStringDisposableStackToNativeMarshaller pathMarshaller = new(path);
					using ULStringDisposableStackToNativeMarshaller resultMarshaller = new(string.Empty);

					var returnValue = c(pathMarshaller.Native, resultMarshaller.Native);

					result = ULString.NativeToManaged(resultMarshaller.Native);
					return Unsafe.As<byte, bool>(ref returnValue);
				};
			}
		}
		public ULFileSystemOpenFileCallback? OpenFile
		{
			set => _OpenFile = value is null ? null : (path, open_for_writing) => value(ULString.NativeToManaged(path), Unsafe.As<byte, bool>(ref open_for_writing));
			get
			{
				var c = _OpenFile;
				return c is null ? null : (in string path, bool openForWriting) =>
				{
					using ULStringDisposableStackToNativeMarshaller pathMarshaller = new(path);
					return c(pathMarshaller.Native, Unsafe.As<bool, byte>(ref openForWriting));
				};
			}
		}
		public ULFileSystemReadFromFileCallback? ReadFromFile
		{
			set => _ReadFromFile = value is null ? null : (handle, data_native, len) => value(handle, new UnmanagedMemoryStream(data_native, len, len, FileAccess.Write));
			get
			{
				var c = _ReadFromFile;
				return c is null ? null : (nuint handle, in UnmanagedMemoryStream data) =>
				{
					return c(handle, data.PositionPointer, data.Length);
				};
			}
		}

		public ULFileSystemFileExistsCallback__PInvoke__? _FileExists
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __FileExists = null });
				else ULPlatform.Handle(ref this, this with { __FileExists = (delegate* unmanaged[Cdecl]<ULString*, byte>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __FileExists;
				return p is null ? null : (path) => p(path);
			}
		}
		public ULFileSystemGetFileSizeCallback__PInvoke__? _GetFileSize
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __GetFileSize = null });
				else ULPlatform.Handle(ref this, this with { __GetFileSize = (delegate* unmanaged[Cdecl]<nuint, long*, byte>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __GetFileSize;
				return p is null ? null : (nuint handle, long* result) => p(handle, result);
			}
		}
		public ULFileSystemGetFileMimeTypeCallback__PInvoke__? _GetFileMimeType
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __GetFileMimeType = null });
				else ULPlatform.Handle(ref this, this with { __GetFileMimeType = (delegate* unmanaged[Cdecl]<ULString*, ULString*, byte>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __GetFileMimeType;
				return p is null ? null : (path, result) => p(path, result);
			}
		}
		public ULFileSystemOpenFileCallback__PInvoke__? _OpenFile
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __OpenFile = null });
				else ULPlatform.Handle(ref this, this with { __OpenFile = (delegate* unmanaged[Cdecl]<ULString*, byte, nuint>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __OpenFile;
				return p is null ? null : (path, openForWriting) => p(path, openForWriting);
			}
		}
		public ULFileSystemCloseFileCallback? CloseFile
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __CloseFile = null });
				else ULPlatform.Handle(ref this, this with { __CloseFile = (delegate* unmanaged[Cdecl]<nuint, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __CloseFile;
				return p is null ? null : (handle) => p(handle);
			}
		}
		public ULFileSystemReadFromFileCallback__PInvoke__? _ReadFromFile
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __ReadFromFile = null });
				else ULPlatform.Handle(ref this, this with { __ReadFromFile = (delegate* unmanaged[Cdecl]<nuint, byte*, long, long>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __ReadFromFile;
				return p is null ? null : (handle, data, length) => p(handle, data, length);
			}
		}

		public delegate* unmanaged[Cdecl]<ULString*, byte> __FileExists;
		public delegate* unmanaged[Cdecl]<nuint, long*, byte> __GetFileSize;
		public delegate* unmanaged[Cdecl]<ULString*, ULString*, byte> __GetFileMimeType;
		public delegate* unmanaged[Cdecl]<ULString*, byte, nuint> __OpenFile;
		public delegate* unmanaged[Cdecl]<nuint, void> __CloseFile;
		public delegate* unmanaged[Cdecl]<nuint, byte*, long, long> __ReadFromFile;

		public void Dispose()
		{
			ULPlatform.Free(this);
		}
	}
}
