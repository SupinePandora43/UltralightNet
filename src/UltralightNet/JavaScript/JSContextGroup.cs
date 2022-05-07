// JSContextRef.h
using System;

namespace UltralightNet
{
	public unsafe class JSContextGroup : IDisposable
	{
		public JSContextGroup() {
			handle = JavaScriptMethods.JSContextGroupCreate();
		}
		private JSContextGroup(void* handle){
			this.handle = handle;
		}

		private void* handle;
		private bool isDisposed = false;
		private bool dispose = true;

		public void* Handle => handle;

		// public JSContext CreateGlobalContext(JSObject globalObject) => new() { isGlobalContext = true, handle = JavaScriptMethods.JSGlobalContextCreateInGroup(Handle, globalObject) };

		public JSContextGroup Retain() => new(JavaScriptMethods.JSContextGroupRetain(Handle));

		public static JSContextGroup CreateFromPointer(void* ptr) => new(ptr);

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
