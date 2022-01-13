using System;
using System.Runtime.InteropServices;

namespace UltralightNet {

	unsafe partial class JavaScriptMethods {
		[DllImport("WebCore")]
		public static extern void* JSValueMakeString(void* context, void* jsString);
	}

}
