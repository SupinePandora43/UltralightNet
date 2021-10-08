using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULFileSystem: IDisposable
	{
		public ULFileSystemFileExistsCallback FileExists { set { unsafe { _FileExists = (path) => value(ULString.NativeToManaged(path)); } } }
		public ULFileSystemGetFileMimeTypeCallback GetFileMimeType
		{
			set
			{
				unsafe
				{
					_GetFileMimeType = (path, resultPtr) =>
					{
						bool ret = value(ULString.NativeToManaged(path), out string result);

						ULStringGeneratedDllImportMarshaler marshaler = new(result);
						Methods.ulStringAssignString((IntPtr)resultPtr, (IntPtr)marshaler.Value);

						return ret;
					};
				}
			}
		}
		public ULFileSystemOpenFileCallback OpenFile { set { unsafe { _OpenFile = (path, open_for_writing) => value(ULString.NativeToManaged(path), open_for_writing); } } }
		public ULFileSystemReadFromFileCallback ReadFromFile
		{
			set
			{
				unsafe
				{
					_ReadFromFile = (handle, data_native, len) => value(handle, new Span<byte>(data_native, (int)len), len);
				}
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
		}
		public ULFileSystemGetFileSizeCallback GetFileSize
		{
			set
			{
				unsafe
				{
					ULFileSystemGetFileSizeCallback callback = value;
					ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
					__GetFileSize = (delegate* unmanaged[Cdecl]<int, long*, bool>)Marshal.GetFunctionPointerForDelegate(callback);
				}
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
		}
		public ULFileSystemOpenFileCallback__PInvoke__ _OpenFile
		{
			set
			{
				unsafe
				{
					ULFileSystemOpenFileCallback__PInvoke__ callback = value;
					ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
					__OpenFile = (delegate* unmanaged[Cdecl]<ULString*, bool, int>)Marshal.GetFunctionPointerForDelegate(callback);
				}
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
					__CloseFile = (delegate* unmanaged[Cdecl]<int, void>)Marshal.GetFunctionPointerForDelegate(callback);
				}
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
					__ReadFromFile = (delegate* unmanaged[Cdecl]<int, byte*, long, long>)Marshal.GetFunctionPointerForDelegate(callback);
				}
			}
		}

		public delegate* unmanaged[Cdecl]<ULString*, bool> __FileExists;
		public delegate* unmanaged[Cdecl]<int, long*, bool> __GetFileSize;
		public delegate* unmanaged[Cdecl]<ULString*, ULString*, bool> __GetFileMimeType;
		public delegate* unmanaged[Cdecl]<ULString*, bool, int> __OpenFile;
		public delegate* unmanaged[Cdecl]<int, void> __CloseFile;
		public delegate* unmanaged[Cdecl]<int, byte*, long, long> __ReadFromFile;

		public void Dispose()
		{
			ULPlatform.Free(this);
		}
	}
}
