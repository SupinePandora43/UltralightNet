using System.Runtime.InteropServices;
using UltralightNet.Platform.HighPerformance;

namespace UltralightNet.Platform
{
	namespace HighPerformance
	{
		/// <summary>
		/// <see cref="IFileSystem" /> native definition.
		/// </summary>
		public unsafe struct ULFileSystem
		{
			public delegate* unmanaged[Cdecl]<ULString*, bool> FileExists;
			public delegate* unmanaged[Cdecl]<ULString*, ULString*> GetFileMimeType;
			public delegate* unmanaged[Cdecl]<ULString*, ULString*> GetFileCharset;
			public delegate* unmanaged[Cdecl]<ULString*, ULBuffer> OpenFile;
		}
	}
	public interface IFileSystem : IDisposable
	{
		bool FileExists(string path);
		string GetFileMimeType(string path);
		string GetFileCharset(string path);
		ULBuffer OpenFile(string path);

#if !NETSTANDARD2_0
		virtual ULFileSystem? GetNativeStruct() => null;
#else
		ULFileSystem? GetNativeStruct();
#endif

		internal sealed unsafe class Wrapper : IDisposable
		{
			readonly IFileSystem instance;
			readonly ULFileSystem _NativeStruct;
			public ULFileSystem NativeStruct
			{
				get
				{
					if (IsDisposed) throw new ObjectDisposedException(nameof(Wrapper));
					return _NativeStruct;
				}
				private init => _NativeStruct = value;
			}
			readonly GCHandle[]? handles;
			public bool IsDisposed { get; private set; }

			public Wrapper(IFileSystem instance)
			{
				this.instance = instance;
				var nativeStruct = instance.GetNativeStruct();
				if (nativeStruct is not null)
				{
					NativeStruct = nativeStruct.Value;
					return;
				}

				handles = new GCHandle[4];

				NativeStruct = new()
				{
					FileExists = (delegate* unmanaged[Cdecl]<ULString*, bool>)Helper.AllocateDelegate((ULString* path) => instance.FileExists(path->ToString()), out handles[0]),
					GetFileMimeType = (delegate* unmanaged[Cdecl]<ULString*, ULString*>)Helper.AllocateDelegate((ULString* path) => instance.GetFileMimeType(path->ToString()), out handles[1]),
					GetFileCharset = (delegate* unmanaged[Cdecl]<ULString*, ULString*>)Helper.AllocateDelegate((ULString* path) => instance.GetFileCharset(path->ToString()), out handles[2]),
					OpenFile = (delegate* unmanaged[Cdecl]<ULString*, ULBuffer>)Helper.AllocateDelegate((ULString* path) => instance.OpenFile(path->ToString()), out handles[3])
				};
			}

			public void Dispose()
			{
				if (IsDisposed) return;
				if (handles is not null)
				{
					foreach (var handle in handles) if (handle.IsAllocated) handle.Free();
				}

				try { instance.Dispose(); }
				finally
				{
					GC.SuppressFinalize(this);
					IsDisposed = true;
				}
			}
			~Wrapper() => Dispose();
		}
	}
}
