using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULFileSystem : IDisposable
	{
		public ULFileSystemFileExistsCallback FileExists
		{
			set
			{
				unsafe
				{
					_FileExists = (path) => value(ULString.NativeToManaged(path));
				}
			}
		}
		public ULFileSystemGetFileMimeTypeCallback GetFileMimeType
		{
			set
			{
				unsafe
				{
					_GetFileMimeType = (path, resultUlStrPtr) =>
					{
						bool ret = value(ULString.NativeToManaged(path), out string result);

						ULStringGeneratedDllImportMarshaler m = new(result);
						Methods.ulStringAssignString(resultUlStrPtr, m.Value);
						m.FreeNative();

						return ret;
					};
				}
			}
		}
		public ULFileSystemOpenFileCallback OpenFile
		{
			set
			{
				unsafe
				{
					_OpenFile = (path, open_for_writing) => value(ULString.NativeToManaged(path), open_for_writing);
				}
			}
		}
		public ULFileSystemReadFromFileCallback ReadFromFile
		{
			set
			{
				unsafe
				{
					_ReadFromFile = (handle, data_native, len) => value(handle, new Span<byte>(data_native, (int)len));
				}
			}
			get
			{
				var c = _ReadFromFile;
				return (handle, data) =>
				{
					fixed (byte* bytes = data) return c(handle, bytes, data.Length);
				};
			}
		}

		public ULFileSystemFileExistsCallback__PInvoke__ _FileExists
		{
			set
			{
				unsafe
				{
					ULFileSystemFileExistsCallback__PInvoke__ callback = value;
					ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
					__FileExists = (delegate* unmanaged[Cdecl]<ULString*, bool>)Marshal.GetFunctionPointerForDelegate(callback);
				}
			}
			get
			{
				var p = __FileExists;
				return (path) => p(path);
			}
		}
		public ULFileSystemGetFileSizeCallback GetFileSize
		{
			set
			{
				unsafe
				{
					ULFileSystemGetFileSizeCallback callback = value;
					ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
					__GetFileSize = (delegate* unmanaged[Cdecl]<nuint, long*, bool>)Marshal.GetFunctionPointerForDelegate(callback);
				}
			}
			get
			{
				var p = __GetFileSize;
				return (nuint handle, out long result) =>
				{
					fixed (long* resultPtr = &result)
						return p(handle, resultPtr);
				};
			}
		}
		public ULFileSystemGetFileMimeTypeCallback__PInvoke__ _GetFileMimeType
		{
			set
			{
				unsafe
				{
					ULFileSystemGetFileMimeTypeCallback__PInvoke__ callback = value;
					ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
					__GetFileMimeType = (delegate* unmanaged[Cdecl]<ULString*, ULString*, bool>)Marshal.GetFunctionPointerForDelegate(callback);
				}
			}
			get
			{
				var p = __GetFileMimeType;
				return (path, result) => p(path, result);
			}
		}
		public ULFileSystemOpenFileCallback__PInvoke__ _OpenFile
		{
			set
			{
				unsafe
				{
					ULFileSystemOpenFileCallback__PInvoke__ callback = value;
					ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
					__OpenFile = (delegate* unmanaged[Cdecl]<ULString*, bool, nuint>)Marshal.GetFunctionPointerForDelegate(callback);
				}
			}
			get
			{
				var p = __OpenFile;
				return (path, openForWriting) => p(path, openForWriting);
			}
		}
		public ULFileSystemCloseFileCallback CloseFile
		{
			set
			{
				unsafe
				{
					ULFileSystemCloseFileCallback callback = value;
					ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
					__CloseFile = (delegate* unmanaged[Cdecl]<nuint, void>)Marshal.GetFunctionPointerForDelegate(callback);
				}
			}
			get
			{
				var p = __CloseFile;
				return (handle) => p(handle);
			}
		}
		public ULFileSystemReadFromFileCallback__PInvoke__ _ReadFromFile
		{
			set
			{
				unsafe
				{
					ULFileSystemReadFromFileCallback__PInvoke__ callback = value;
					ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
					__ReadFromFile = (delegate* unmanaged[Cdecl]<nuint, byte*, long, long>)Marshal.GetFunctionPointerForDelegate(callback);
				}
			}
			get
			{
				var p = __ReadFromFile;
				return (handle, data, length) => p(handle, data, length);
			}
		}

		public delegate* unmanaged[Cdecl]<ULString*, bool> __FileExists;
		public delegate* unmanaged[Cdecl]<nuint, long*, bool> __GetFileSize;
		public delegate* unmanaged[Cdecl]<ULString*, ULString*, bool> __GetFileMimeType;
		public delegate* unmanaged[Cdecl]<ULString*, bool, nuint> __OpenFile;
		public delegate* unmanaged[Cdecl]<nuint, void> __CloseFile;
		public delegate* unmanaged[Cdecl]<nuint, byte*, long, long> __ReadFromFile;

		public void Dispose()
		{
			ULPlatform.Free(this);
		}
	}
}
