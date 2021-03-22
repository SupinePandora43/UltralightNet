namespace UltralightNet
{
	//todo: test
	public struct ULFileSystem
	{
		public ULFileSystemFileExistsCallback file_exists;
		public ULFileSystemGetFileSizeCallback get_file_size;
		public ULFileSystemGetFileMimeTypeCallback get_file_mime_type;
		public ULFileSystemOpenFileCallback open_file;
		public ULFileSystemCloseFileCallback close_file;
		public ULFileSystemReadFromFileCallback read_from_file;
	}
}
