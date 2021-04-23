using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ULFileSystem
	{
		public ULFileSystemFileExistsCallback FileExists;
		public ULFileSystemGetFileSizeCallback GetFileSize;
		public ULFileSystemGetFileMimeTypeCallback GetFileMimeType;
		public ULFileSystemOpenFileCallback OpenFile;
		public ULFileSystemCloseFileCallback CloseFile;
		public ULFileSystemReadFromFileCallback__PInvoke__ _ReadFromFile;
		public ULFileSystemReadFromFileCallback ReadFromFile
		{
			set
			{
				unsafe
				{
					_ReadFromFile = (handle, data_native, len) =>
					{
						long retVal = value(handle, out byte[] data, len);
						// TODO: FIXME
						// at least, it works
						for (int i = 0; i < len; i++)
						{
							data_native[i] = data[i];
						}

						return retVal;
					};
					// TODO: Free
					GCHandle.Alloc(_ReadFromFile, GCHandleType.Normal);
				}
			}
		}
	}
}
