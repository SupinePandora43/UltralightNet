using System.Runtime.InteropServices;
using UltralightNet.Platform.HighPerformance;

namespace UltralightNet.Platform
{
	namespace HighPerformance
	{
		/// <summary>
		/// <see cref="ISurfaceDefinition" /> native definition.
		/// </summary>
		public unsafe struct ULSurfaceDefinition
		{
			public delegate* unmanaged[Cdecl]<uint, uint, nint> Create;
			public delegate* unmanaged[Cdecl]<nint, void> Destroy;
			public delegate* unmanaged[Cdecl]<nint, uint> GetWidth;
			public delegate* unmanaged[Cdecl]<nint, uint> GetHeight;
			public delegate* unmanaged[Cdecl]<nint, uint> GetRowBytes;
			public delegate* unmanaged[Cdecl]<nint, nuint> GetSize;
			public delegate* unmanaged[Cdecl]<nint, byte*> LockPixels;
			public delegate* unmanaged[Cdecl]<nint, void> UnlockPixels;
			public delegate* unmanaged[Cdecl]<nint, uint, uint, void> Resize;
		}
	}
	public interface ISurfaceDefinition : IDisposable
	{
		nint Create(uint width, uint height);
		void Destroy(nint id);
		uint GetWidth(nint id);
		uint GetHeight(nint id);
		uint GetRowBytes(nint id);
		nuint GetSize(nint id);
		unsafe byte* LockPixels(nint id);
		void UnlockPixels(nint id);
		void Resize(nint id, uint width, uint height);

#if !NETSTANDARD2_0
		virtual ULSurfaceDefinition? GetNativeStruct() => null;
#else
		ULSurfaceDefinition? GetNativeStruct();
#endif

		internal sealed unsafe class Wrapper : IDisposable
		{
			readonly ISurfaceDefinition instance;
			readonly ULSurfaceDefinition _NativeStruct;
			public ULSurfaceDefinition NativeStruct
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

			public Wrapper(ISurfaceDefinition instance)
			{
				this.instance = instance;
				var nativeStruct = instance.GetNativeStruct();
				if (nativeStruct is not null)
				{
					NativeStruct = nativeStruct.Value;
					return;
				}

				handles = new GCHandle[3];

				NativeStruct = new()
				{

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
