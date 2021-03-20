using System;
using System.Runtime.InteropServices;
using Xunit;

namespace UltralightNet.Test
{
	public class ULKeyCodesTest
	{
		[DllImport("Ultralight", EntryPoint = "?GetKeyIdentifierFromVirtualKeyCode@ultralight@@YAXHAEAVString@1@@Z")]
		private static extern void GetKeyIdentifierFromVirtualKeyCode_win(int i, IntPtr id);

		[DllImport("Ultralight", EntryPoint = "_ZN10ultralight34GetKeyIdentifierFromVirtualKeyCodeEiRNS_6StringE")]
		private static extern void GetKeyIdentifierFromVirtualKeyCode_linux(int i, IntPtr id);

		[DllImport("Ultralight", EntryPoint = "__ZN10ultralight34GetKeyIdentifierFromVirtualKeyCodeEiRNS_6StringE")]
		private static extern void GetKeyIdentifierFromVirtualKeyCode_osx(int i, IntPtr id);

		// https://github.com/dotnet/runtime/issues/49858
		/*
		[DllImport("Ultralight", EntryPoint = "?GetKeyIdentifierFromVirtualKeyCode@ultralight@@YAXHAEAVString@1@@Z")]
		private static extern void GetKeyOut(
			int i,
			[MarshalAs(UnmanagedType.CustomMarshaler,MarshalTypeRef = typeof(ULStringMarshaler))]
			ref string id
		);
		*/

		[Fact]
		public void GetKeyIdentifierFromVirtualKeyCodeTest()
		{
			string test = "initialValue";
			IntPtr ulStringPtr = ULStringMarshaler.ManagedToNative(test);

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				GetKeyIdentifierFromVirtualKeyCode_win(ULKeyCodes.GK_VOLUME_MUTE, ulStringPtr);
			}else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				GetKeyIdentifierFromVirtualKeyCode_linux(ULKeyCodes.GK_VOLUME_MUTE, ulStringPtr);
			}
			else
			{
				return;
				try
				{
					GetKeyIdentifierFromVirtualKeyCode_osx(ULKeyCodes.GK_VOLUME_MUTE, ulStringPtr);
				}
				catch
				{
					GetKeyIdentifierFromVirtualKeyCode_linux(ULKeyCodes.GK_VOLUME_MUTE, ulStringPtr);
				}
			}

			string result = ULStringMarshaler.NativeToManaged(ulStringPtr);

			ULStringMarshaler.CleanUpNative(ulStringPtr);

			Assert.Equal("VolumeMute", result);
		}
	}
}
