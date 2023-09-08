using System.Collections.Generic;
using System.Runtime.InteropServices;
using UltralightNet.Platform.HighPerformance;

namespace UltralightNet.Platform
{
	namespace HighPerformance
	{
		/// <summary>
		/// <see cref="IGPUDriver" /> native definition.
		/// </summary>
		public unsafe struct ULGPUDriver
		{
#if !NETSTANDARD
			public delegate* unmanaged[Cdecl]<void> BeginSynchronize;
			public delegate* unmanaged[Cdecl]<void> EndSynchronize;
			public delegate* unmanaged[Cdecl]<uint> NextTextureId;
			public delegate* unmanaged[Cdecl]<uint, void*, void> CreateTexture;
			public delegate* unmanaged[Cdecl]<uint, void*, void> UpdateTexture;
			public delegate* unmanaged[Cdecl]<uint, void> DestroyTexture;
			public delegate* unmanaged[Cdecl]<uint> NextRenderBufferId;
			public delegate* unmanaged[Cdecl]<uint, ULRenderBuffer, void> CreateRenderBuffer;
			public delegate* unmanaged[Cdecl]<uint, void> DestroyRenderBuffer;
			public delegate* unmanaged[Cdecl]<uint> NextGeometryId;
			public delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void> CreateGeometry;
			public delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void> UpdateGeometry;
			public delegate* unmanaged[Cdecl]<uint, void> DestroyGeometry;
			public delegate* unmanaged[Cdecl]<ULCommandList, void> UpdateCommandList;
#else
			public void* BeginSynchronize, EndSynchronize, NextTextureId, CreateTexture, UpdateTexture, DestroyTexture, NextRenderBufferId, CreateRenderBuffer, DestroyRenderBuffer, NextGeometryId, CreateGeometry, UpdateGeometry, DestroyGeometry, UpdateCommandList;
#endif
		}
	}

	public interface IGPUDriver
	{
		uint NextTextureId();
		void CreateTexture(uint textureId, ULBitmap bitmap);
		void UpdateTexture(uint textureId, ULBitmap bitmap);
		void DestroyTexture(uint textureId);

		uint NextRenderBufferId();
		void CreateRenderBuffer(uint renderBufferId, ULRenderBuffer renderBuffer);
		void DestroyRenderBuffer(uint renderBufferId);

		uint NextGeometryId();
		void CreateGeometry(uint geometryId, ULVertexBuffer vertexBuffer, ULIndexBuffer indexBuffer);
		void UpdateGeometry(uint geometryId, ULVertexBuffer vertexBuffer, ULIndexBuffer indexBuffer);
		void DestroyGeometry(uint geometryId);

		void UpdateCommandList(ULCommandList commandList);

#if !NETSTANDARD2_0
		virtual ULGPUDriver? GetNativeStruct() => null;
#else
		ULGPUDriver? GetNativeStruct();
#endif

		internal sealed unsafe class Wrapper : IDisposable
		{
			delegate uint IdCallback();
			delegate void RenderBufferCallback(uint id, ULRenderBuffer renderBuffer);
			delegate void GeometryCallback(uint id, ULVertexBuffer vertexBuffer, ULIndexBuffer indexBuffer);
			delegate void DestroyIdCallback(uint id);
			delegate void CommandListCallback(ULCommandList commandList);

			readonly Dictionary<nint, WeakReference<ULBitmap>>? BitmapCache;
			uint newCachedInstanceCount = 0;
			ULBitmap BitmapFromHandleCached(void* ptr)
			{
				if (!BitmapCache!.TryGetValue((nint)ptr, out var weakBitmap) || !weakBitmap.TryGetTarget(out ULBitmap? bitmap))
				{
					bitmap = ULBitmap.FromHandle(ptr, false);
					BitmapCache[(nint)ptr] = new WeakReference<ULBitmap>(bitmap);
					newCachedInstanceCount++;
				}

				if (newCachedInstanceCount > 256)
				{
					foreach (var keyValuePair in BitmapCache)
					{
						if (!keyValuePair.Value.TryGetTarget(out _)) BitmapCache.Remove(keyValuePair.Key);
					}
					newCachedInstanceCount = 0;
				}
				return bitmap;
			}

			readonly IGPUDriver instance;
			readonly ULGPUDriver _NativeStruct;
			public ULGPUDriver NativeStruct
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

			public Wrapper(IGPUDriver instance)
			{
				this.instance = instance;
				var nativeStruct = instance.GetNativeStruct();
				if (nativeStruct is not null)
				{
					NativeStruct = nativeStruct.Value;
					return;
				}

				if (instance is IGPUDriverSynchronized sync)
				{
					handles = new GCHandle[14];

					NativeStruct = NativeStruct with
					{
						BeginSynchronize = (delegate* unmanaged[Cdecl]<void>)Helper.AllocateDelegate(sync.BeginSynchronize, out handles[12]),
						EndSynchronize = (delegate* unmanaged[Cdecl]<void>)Helper.AllocateDelegate(sync.EndSynchronize, out handles[13])
					};
				}
				else handles = new GCHandle[12];

				BitmapCache = new(32);

				NativeStruct = NativeStruct with
				{
					NextTextureId = (delegate* unmanaged[Cdecl]<uint>)Helper.AllocateDelegate<IdCallback>(instance.NextTextureId, out handles[0]),
					CreateTexture = (delegate* unmanaged[Cdecl]<uint, void*, void>)Helper.AllocateDelegate((uint id, void* bitmap) => instance.CreateTexture(id, BitmapFromHandleCached(bitmap)), out handles[1]),
					UpdateTexture = (delegate* unmanaged[Cdecl]<uint, void*, void>)Helper.AllocateDelegate((uint id, void* bitmap) => instance.UpdateTexture(id, BitmapFromHandleCached(bitmap)), out handles[2]),
					DestroyTexture = (delegate* unmanaged[Cdecl]<uint, void>)Helper.AllocateDelegate<DestroyIdCallback>(instance.DestroyTexture, out handles[3]),
					NextRenderBufferId = (delegate* unmanaged[Cdecl]<uint>)Helper.AllocateDelegate<IdCallback>(instance.NextRenderBufferId, out handles[4]),
					CreateRenderBuffer = (delegate* unmanaged[Cdecl]<uint, ULRenderBuffer, void>)Helper.AllocateDelegate<RenderBufferCallback>(instance.CreateRenderBuffer, out handles[5]),
					DestroyRenderBuffer = (delegate* unmanaged[Cdecl]<uint, void>)Helper.AllocateDelegate<DestroyIdCallback>(instance.DestroyRenderBuffer, out handles[6]),
					NextGeometryId = (delegate* unmanaged[Cdecl]<uint>)Helper.AllocateDelegate<IdCallback>(instance.NextGeometryId, out handles[7]),
					CreateGeometry = (delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void>)Helper.AllocateDelegate<GeometryCallback>(instance.CreateGeometry, out handles[8]),
					UpdateGeometry = (delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void>)Helper.AllocateDelegate<GeometryCallback>(instance.UpdateGeometry, out handles[9]),
					DestroyGeometry = (delegate* unmanaged[Cdecl]<uint, void>)Helper.AllocateDelegate<DestroyIdCallback>(instance.DestroyGeometry, out handles[10]),
					UpdateCommandList = (delegate* unmanaged[Cdecl]<ULCommandList, void>)Helper.AllocateDelegate<CommandListCallback>(instance.UpdateCommandList, out handles[11])
				};
			}

			public void Dispose()
			{
				if (IsDisposed) return;
				if (handles is not null)
				{
					foreach (var handle in handles) if (handle.IsAllocated) handle.Free();
				}

				GC.SuppressFinalize(this);
				IsDisposed = true;
			}
			~Wrapper() => Dispose();
		}
	}
	public interface IGPUDriverSynchronized : IGPUDriver
	{
		/// <summary>Called before any commands are dispatched during a frame.</summary>
		void BeginSynchronize();
		/// <summary>Called after any commands are dispatched during a frame.</summary>
		void EndSynchronize();
	}
}
