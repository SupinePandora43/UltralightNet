using System;
using System.Runtime.InteropServices;
using System.Threading;
using UltralightNet.Structs;
using Xunit;

namespace UltralightNet.Test
{
	public class RendererTest
	{
		[DllImport("Ultralight", EntryPoint = "?GetKeyIdentifierFromVirtualKeyCode@ultralight@@YAXHAEAVString@1@@Z")]
		private static extern void GetKey(int i, IntPtr id);

		[DllImport("Ultralight", EntryPoint = "?GetKeyIdentifierFromVirtualKeyCode@ultralight@@YAXHAEAVString@1@@Z")]
		private static extern void GetKeyOut(
			int i,
			[MarshalAs(UnmanagedType.CustomMarshaler,MarshalTypeRef =typeof(ULStringMarshaler))]
			out string id
		);

		[Fact]
		public void TestRenderer()
		{
			AppCore.AppCore.EnablePlatformFontLoader();

			ULConfig config = new()
			{
				ResourcePath = "./resources"
			};
			Renderer renderer = new(config);

			View view = new View(renderer, 512, 512, false, Session.DefaultSession(renderer), true);

			view.URL = "https://github.com";

			uint att = 0;
			while (view.URL == "")
			{
				renderer.Update();
				Thread.Sleep(100);
				att++;
			}
			ULStringMarshaler marshaler = (ULStringMarshaler)ULStringMarshaler.GetInstance("");

			IntPtr ulStringPtr = marshaler.MarshalManagedToNative("test");

			IntPtr wCharPtr = Marshal.ReadIntPtr(ulStringPtr, 0);

			string managedString = Marshal.PtrToStringUni(wCharPtr);

			Assert.Equal("test", managedString);

			string managedByMarshalerString = (string)marshaler.MarshalNativeToManaged(ulStringPtr);

			Assert.Equal("test", managedByMarshalerString);
			ULString str = new("");
			GetKey(ULKeyCodes.GK_VOLUME_MUTE, str.Ptr);

			// error
			//GetKeyOut(ULKeyCodes.GK_VOLUME_MUTE, out string i);

			Console.WriteLine(str.ToString());

			Assert.Equal("https://github.com", view.URL);
		}
	}
}
