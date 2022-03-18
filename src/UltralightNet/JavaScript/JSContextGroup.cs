// JSContextRef.h
using System;

namespace UltralightNet
{
	public unsafe class JSContextGroup : IDisposable
	{
		public JSContextGroup() {
			handle = JavaScriptMethods.JSContextGroupCreate();
		}
		public JSContextGroup(void* handle){
			this.handle = handle;
		}

		private void* handle;
		private bool isDisposed = false;
		private bool dispose = true;

		public void* Handle => handle;

		public JSContextGroup Retain() => new(JavaScriptMethods.JSContextGroupRetain(Handle));

		public void Dispose()
		{
			if (!isDisposed && dispose)
			{
				JavaScriptMethods.JSContextGroupRelease(Handle);
				isDisposed = true;
				GC.SuppressFinalize(this);
			}
		}
	}
}
