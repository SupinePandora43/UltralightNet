// TODO: ALLOCATE ME, TEST ME, FREE ME
using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ULFileSystem
	{
		public ULFileSystemFileExistsCallback__PInvoke__ _FileExists;
		public ULFileSystemGetFileSizeCallback GetFileSize;
		public ULFileSystemGetFileMimeTypeCallback__PInvoke__ _GetFileMimeType;
		public ULFileSystemOpenFileCallback__PInvoke__ _OpenFile;
		public ULFileSystemCloseFileCallback CloseFile;
		public ULFileSystemReadFromFileCallback__PInvoke__ _ReadFromFile;


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
					// TODO: Free
					GCHandle.Alloc(_ReadFromFile, GCHandleType.Normal);
				}
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct _ULFileSystem
	{
		// return byte? :P
		public delegate* unmanaged[Cdecl]<ULString*, bool> FileExists;
		public delegate* unmanaged[Cdecl]<int, long*, bool> GetFileSize;
		public delegate* unmanaged[Cdecl]<ULString*, ULString*, bool> GetFileMimeType;
		public delegate* unmanaged[Cdecl]<ULString*, bool, int> OpenFile;
		public delegate* unmanaged[Cdecl]<int, void> CloseFile;
		public delegate* unmanaged[Cdecl]<int, byte*, long, long> ReadFromFile;
	}
}
