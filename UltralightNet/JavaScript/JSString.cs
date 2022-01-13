using System;
using System.Runtime.InteropServices;

namespace UltralightNet {

	unsafe partial class JavaScriptMethods {
		[DllImport("WebCore")]
		public static extern void* JSStringCreateWithCharacters(ushort* characters, nuint length);

		[DllImport("WebCore")]
		public static extern void* JSStringCreateWithUTF8CString(byte* characters);

		[DllImport("WebCore")]
		public static extern void* JSStringRetain(void* @string);

		[DllImport("WebCore")]
		public static extern void JSStringRelease(void* @string);

		[DllImport("WebCore")]
		public static extern nuint JSStringGetLength(void* @string);

		[DllImport("WebCore")]
		public static extern ushort* JSStringGetCharactersPtr(void* @string);

		[DllImport("WebCore")]
		public static extern nuint JSStringGetMaximumUTF8CStringSize(void* @string);

		[DllImport("WebCore")]
		public static extern nuint JSStringGetUTF8CString(void* @string, byte* buffer, nuint bufferSize);

		[GeneratedDllImport("WebCore")]
		public static partial bool JSStringIsEqual(void* a, void* b);

		[GeneratedDllImport("WebCore")]
		public static partial bool JSStringIsEqualToUTF8CString(void* str, byte* characters);
	}

	public unsafe class JSString: IDisposable {
		private void* handle;
		private bool isDisposed = false;

		public JSString(string str){
			fixed(char* characters = str){
				handle = JavaScriptMethods.JSStringCreateWithCharacters((ushort*) characters, (nuint) str.Length);
			}
		}

		public void* Handle => handle;

		~JSString() => Dispose();

		public void Dispose()
		{
			if(isDisposed) return;

			JavaScriptMethods.JSStringRelease(handle);

			isDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

}
