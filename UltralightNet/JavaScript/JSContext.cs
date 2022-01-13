using System.Runtime.InteropServices;

namespace UltralightNet {

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
	public static unsafe partial class JavaScriptMethods {
		static JavaScriptMethods() => Methods.Preload();

		[DllImport("WebCore")]
		public static extern void* JSContextGroupCreate();

		[DllImport("WebCore")]
		public static extern void* JSContextGroupRetain(void* contextGroup);

		[DllImport("WebCore")]
		public static extern void JSContextGroupRelease(void* contextGroup);

		[DllImport("WebCore")]
		public static extern void* JSGlobalContextCreate(void* globalObjectClass);

		[DllImport("WebCore")]
		public static extern void* JSGlobalContextCreateInGroup(void* contextGroup, void* globalObjectClass);

		[DllImport("WebCore")]
		public static extern void* JSGlobalContextRetain(void* globalContext);

		[DllImport("WebCore")]
		public static extern void JSGlobalContextRelease(void* globalContext);

		[DllImport("WebCore")]
		public static extern void* JSContextGetGlobalObject(void* context);

		[DllImport("WebCore")]
		public static extern void* JSContextGetGroup(void* context);

		[DllImport("WebCore")]
		public static extern void* JSContextGetGlobalContext(void* context);

		[DllImport("WebCore")]
		public static extern void* JSGlobalContextCopyName(void* globalContext);

		[DllImport("WebCore")]
		public static extern void JSGlobalContextSetName(void* globalContext, void* name);
	}

	public unsafe class JSContext {
		public JSContext(void* ptr){
			handle = ptr;
			dispose = false;
		}
		public JSContext(){
			dispose = true;
		}

		private void* handle;
		private bool isDisposed = false;
		private bool dispose = true;

		public void* Handle => handle;

		public JSObject GlobalObject => new(Handle, JavaScriptMethods.JSContextGetGlobalObject(handle));

		internal void OnLocked(void* actualHandle){
			handle = actualHandle;
		}
	}

}
