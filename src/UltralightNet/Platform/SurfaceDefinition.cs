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
#if !NETSTANDARD
			public delegate* unmanaged[Cdecl]<uint, uint, nint> Create;
			public delegate* unmanaged[Cdecl]<nint, void> Destroy;
			public delegate* unmanaged[Cdecl]<nint, uint> GetWidth;
			public delegate* unmanaged[Cdecl]<nint, uint> GetHeight;
			public delegate* unmanaged[Cdecl]<nint, uint> GetRowBytes;
			public delegate* unmanaged[Cdecl]<nint, nuint> GetSize;
			public delegate* unmanaged[Cdecl]<nint, byte*> LockPixels;
			public delegate* unmanaged[Cdecl]<nint, void> UnlockPixels;
			public delegate* unmanaged[Cdecl]<nint, uint, uint, void> Resize;
#else
			public void* Create, Destroy, GetWidth, GetHeight, GetRowBytes, GetSize, LockPixels, UnlockPixels, Resize;
#endif
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
			delegate nint CreateCallback(uint width, uint height);
			delegate void VoidIdCallback(nint id);
			delegate uint UintIdCallback(nint id);
			delegate nuint NUintIdCallback(nint id);
			delegate byte* BytePtrIdCallback(nint id);
			delegate void ResizeCallback(nint id, uint width, uint height);

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

				handles = new GCHandle[9];

				NativeStruct = new()
				{
					Create = (delegate* unmanaged[Cdecl]<uint, uint, nint>)Helper.AllocateDelegate<CreateCallback>(instance.Create, out handles[0]),
					Destroy = (delegate* unmanaged[Cdecl]<nint, void>)Helper.AllocateDelegate<VoidIdCallback>(instance.Destroy, out handles[1]),
					GetWidth = (delegate* unmanaged[Cdecl]<nint, uint>)Helper.AllocateDelegate<UintIdCallback>(instance.GetWidth, out handles[2]),
					GetHeight = (delegate* unmanaged[Cdecl]<nint, uint>)Helper.AllocateDelegate<UintIdCallback>(instance.GetHeight, out handles[3]),
					GetRowBytes = (delegate* unmanaged[Cdecl]<nint, uint>)Helper.AllocateDelegate<UintIdCallback>(instance.GetRowBytes, out handles[4]),
					GetSize = (delegate* unmanaged[Cdecl]<nint, nuint>)Helper.AllocateDelegate<NUintIdCallback>(instance.GetSize, out handles[5]),
					LockPixels = (delegate* unmanaged[Cdecl]<nint, byte*>)Helper.AllocateDelegate<BytePtrIdCallback>(instance.LockPixels, out handles[6]),
					UnlockPixels = (delegate* unmanaged[Cdecl]<nint, void>)Helper.AllocateDelegate<VoidIdCallback>(instance.UnlockPixels, out handles[7]),
					Resize = (delegate* unmanaged[Cdecl]<nint, uint, uint, void>)Helper.AllocateDelegate<ResizeCallback>(instance.Resize, out handles[8])
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
