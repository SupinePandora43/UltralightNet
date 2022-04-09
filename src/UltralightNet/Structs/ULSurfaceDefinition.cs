using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULSurfaceDefinition : IDisposable
	{
		public ULSurfaceDefinitionCreateCallback? Create
		{
			set => ULPlatform.Handle(ref this, this with { _Create = value is null ? null : (delegate* unmanaged[Cdecl]<uint, uint, void*>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			get
			{
				var p = _Create;
				return p is null ? null : (width, height) => p(width, height);
			}
		}
		public ULSurfaceDefinitionDestroyCallback? Destroy
		{
			set => ULPlatform.Handle(ref this, this with { _Destroy = value is null ? null : (delegate* unmanaged[Cdecl]<void*, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			get
			{
				var p = _Destroy;
				return p is null ? null : (data) => p(data);
			}
		}
		public ULSurfaceDefinitionGetWidthCallback? GetWidth
		{
			set => ULPlatform.Handle(ref this, this with { _GetWidth = value is null ? null : (delegate* unmanaged[Cdecl]<void*, uint>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			get
			{
				var p = _GetWidth;
				return p is null ? null : (data) => p(data);
			}
		}
		public ULSurfaceDefinitionGetHeightCallback? GetHeight
		{
			set => ULPlatform.Handle(ref this, this with { _GetHeight = value is null ? null : (delegate* unmanaged[Cdecl]<void*, uint>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			get
			{
				var p = _GetHeight;
				return p is null ? null : (data) => p(data);
			}
		}
		public ULSurfaceDefinitionGetRowBytesCallback? GetRowBytes
		{
			set => ULPlatform.Handle(ref this, this with { _GetRowBytes = value is null ? null : (delegate* unmanaged[Cdecl]<void*, uint>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			get
			{
				var p = _GetRowBytes;
				return p is null ? null : (data) => p(data);
			}
		}
		public ULSurfaceDefinitionGetSizeCallback? GetSize
		{
			set => ULPlatform.Handle(ref this, this with { _GetSize = value is null ? null : (delegate* unmanaged[Cdecl]<void*, nuint>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			get
			{
				var p = _GetSize;
				return p is null ? null : (data) => p(data);
			}
		}
		public ULSurfaceDefinitionLockPixelsCallback? LockPixels
		{
			set => ULPlatform.Handle(ref this, this with { _LockPixels = value is null ? null : (delegate* unmanaged[Cdecl]<void*, void*>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			get
			{
				var p = _LockPixels;
				return p is null ? null : (data) => p(data);
			}
		}
		public ULSurfaceDefinitionUnlockPixelsCallback? UnlockPixels
		{
			set => ULPlatform.Handle(ref this, this with { _UnlockPixels = value is null ? null : (delegate* unmanaged[Cdecl]<void*, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			get
			{
				var p = _UnlockPixels;
				return p is null ? null : (data) => p(data);
			}
		}
		public ULSurfaceDefinitionResizeCallback? Resize
		{
			set => ULPlatform.Handle(ref this, this with { _Resize = value is null ? null : (delegate* unmanaged[Cdecl]<void*, uint, uint, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			get
			{
				var p = _Resize;
				return p is null ? null : (data, width, height) => p(data, width, height);
			}
		}

		public delegate* unmanaged[Cdecl]<uint, uint, void*> _Create;
		public delegate* unmanaged[Cdecl]<void*, void> _Destroy;
		public delegate* unmanaged[Cdecl]<void*, uint> _GetWidth;
		public delegate* unmanaged[Cdecl]<void*, uint> _GetHeight;
		public delegate* unmanaged[Cdecl]<void*, uint> _GetRowBytes;
		public delegate* unmanaged[Cdecl]<void*, nuint> _GetSize;
		public delegate* unmanaged[Cdecl]<void*, void*> _LockPixels;
		public delegate* unmanaged[Cdecl]<void*, void> _UnlockPixels;
		public delegate* unmanaged[Cdecl]<void*, uint, uint, void> _Resize;

		public void Dispose() => ULPlatform.Free(this);
	}
}
