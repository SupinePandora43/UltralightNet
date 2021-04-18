// Until https://github.com/dotnet/runtimelab/issues/925
// + (side story) https://github.com/dotnet/runtimelab/issues/938


#if !GENERATED

#pragma warning disable IDE0059

using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
    public static partial class Methods
    {
        public static partial global::System.IntPtr ulCreateKeyEvent(global::UltralightNet.ULKeyEventType type, global::UltralightNet.ULKeyEventModifiers modifiers, int virtual_key_code, int native_key_code, string text, string unmodified_text, bool is_keypad, bool is_auto_repeat, bool is_system_key)
        {
            unsafe
            {
                global::System.IntPtr __text_gen_native = default;
                global::System.IntPtr __unmodified_text_gen_native = default;
                byte __is_keypad_gen_native = default;
                byte __is_auto_repeat_gen_native = default;
                byte __is_system_key_gen_native = default;
                global::System.IntPtr __retVal = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __text_gen_native__marshaler = default;
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __unmodified_text_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __text_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(text);
                    __text_gen_native = __text_gen_native__marshaler.Value;
                    __unmodified_text_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(unmodified_text);
                    __unmodified_text_gen_native = __unmodified_text_gen_native__marshaler.Value;
                    __is_keypad_gen_native = (byte)(is_keypad ? 1 : 0);
                    __is_auto_repeat_gen_native = (byte)(is_auto_repeat ? 1 : 0);
                    __is_system_key_gen_native = (byte)(is_system_key ? 1 : 0);
                    //
                    // Invoke
                    //
                    __retVal = ulCreateKeyEvent__PInvoke__(type, modifiers, virtual_key_code, native_key_code, __text_gen_native, __unmodified_text_gen_native, __is_keypad_gen_native, __is_auto_repeat_gen_native, __is_system_key_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __text_gen_native__marshaler.FreeNative();
                    __unmodified_text_gen_native__marshaler.FreeNative();
                }

                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulCreateKeyEvent")]
        extern private static unsafe global::System.IntPtr ulCreateKeyEvent__PInvoke__(global::UltralightNet.ULKeyEventType type, global::UltralightNet.ULKeyEventModifiers modifiers, int virtual_key_code, int native_key_code, global::System.IntPtr text, global::System.IntPtr unmodified_text, byte is_keypad, byte is_auto_repeat, byte is_system_key);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        public static partial global::System.IntPtr ulCreateSession(global::System.IntPtr renderer, bool is_persistent, string name)
        {
            unsafe
            {
                byte __is_persistent_gen_native = default;
                global::System.IntPtr __name_gen_native = default;
                global::System.IntPtr __retVal = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __name_gen_native__marshaler = default;
                try
                {
                    //
                    // Marshal
                    //
                    __is_persistent_gen_native = (byte)(is_persistent ? 1 : 0);
                    __name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(name);
                    __name_gen_native = __name_gen_native__marshaler.Value;
                    //
                    // Invoke
                    //
                    __retVal = ulCreateSession__PInvoke__(renderer, __is_persistent_gen_native, __name_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    __name_gen_native__marshaler.FreeNative();
                }

                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulCreateSession")]
        extern private static unsafe global::System.IntPtr ulCreateSession__PInvoke__(global::System.IntPtr renderer, byte is_persistent, global::System.IntPtr name);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        public static partial string ulSessionGetName(global::System.IntPtr session)
        {
            unsafe
            {
                string __retVal = default;
                global::System.IntPtr __retVal_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __retVal_gen_native__marshaler = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulSessionGetName__PInvoke__(session);
                //
                // Unmarshal
                //
                __retVal_gen_native__marshaler.Value = __retVal_gen_native;
                __retVal = __retVal_gen_native__marshaler.ToManaged();
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulSessionGetName")]
        extern private static unsafe global::System.IntPtr ulSessionGetName__PInvoke__(global::System.IntPtr session);
    }
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulSessionIsPersistent(global::System.IntPtr session)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulSessionIsPersistent__PInvoke__(session);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulSessionIsPersistent")]
		extern private static unsafe byte ulSessionIsPersistent__PInvoke__(global::System.IntPtr session);
	}
}
namespace UltralightNet
{
    public static partial class Methods
    {
        public static partial string ulSessionGetDiskPath(global::System.IntPtr session)
        {
            unsafe
            {
                string __retVal = default;
                global::System.IntPtr __retVal_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __retVal_gen_native__marshaler = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulSessionGetDiskPath__PInvoke__(session);
                //
                // Unmarshal
                //
                __retVal_gen_native__marshaler.Value = __retVal_gen_native;
                __retVal = __retVal_gen_native__marshaler.ToManaged();
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulSessionGetDiskPath")]
        extern private static unsafe global::System.IntPtr ulSessionGetDiskPath__PInvoke__(global::System.IntPtr session);
    }
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial global::System.IntPtr ulCreateBitmapFromPixels(uint width, uint height, global::UltralightNet.ULBitmapFormat format, uint row_bytes, global::System.IntPtr pixels, uint size, bool should_copy)
		{
			unsafe
			{
				byte __should_copy_gen_native = default;
				global::System.IntPtr __retVal = default;
				//
				// Marshal
				//
				__should_copy_gen_native = (byte)(should_copy ? 1 : 0);
				//
				// Invoke
				//
				__retVal = ulCreateBitmapFromPixels__PInvoke__(width, height, format, row_bytes, pixels, size, __should_copy_gen_native);
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulCreateBitmapFromPixels")]
		extern private static unsafe global::System.IntPtr ulCreateBitmapFromPixels__PInvoke__(uint width, uint height, global::UltralightNet.ULBitmapFormat format, uint row_bytes, global::System.IntPtr pixels, uint size, byte should_copy);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulBitmapOwnsPixels(global::System.IntPtr bitmap)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulBitmapOwnsPixels__PInvoke__(bitmap);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulBitmapOwnsPixels")]
		extern private static unsafe byte ulBitmapOwnsPixels__PInvoke__(global::System.IntPtr bitmap);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulBitmapIsEmpty(global::System.IntPtr bitmap)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulBitmapIsEmpty__PInvoke__(bitmap);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulBitmapIsEmpty")]
		extern private static unsafe byte ulBitmapIsEmpty__PInvoke__(global::System.IntPtr bitmap);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulBitmapWritePNG(global::System.IntPtr bitmap, string path)
		{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
			unsafe
            {
                byte *__path_gen_native = default;
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Setup
                //
                bool path__allocated = false;
                try
                {
                    //
                    // Marshal
                    //
                    if (path != null)
                    {
                        int path__bytelen = (path.Length + 1) * 3 + 1;
                        if (path__bytelen > 260)
                        {
                            __path_gen_native = (byte *)System.Runtime.InteropServices.Marshal.StringToCoTaskMemUTF8(path);
                            path__allocated = true;
                        }
                        else
                        {
                            byte *path__stackptr = stackalloc byte[path__bytelen];
                            {
                                path__bytelen = System.Text.Encoding.UTF8.GetBytes(path, new System.Span<byte>(path__stackptr, path__bytelen));
                                path__stackptr[path__bytelen] = 0;
                            }

                            __path_gen_native = (byte *)path__stackptr;
                        }
                    }

                    //
                    // Invoke
                    //
                    __retVal_gen_native = ulBitmapWritePNG__PInvoke__(bitmap, __path_gen_native);
                    //
                    // Unmarshal
                    //
                    __retVal = __retVal_gen_native != 0;
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    if (path__allocated)
                    {
                        System.Runtime.InteropServices.Marshal.FreeCoTaskMem((System.IntPtr)__path_gen_native);
                    }
                }

                return __retVal;
            }
#else
			unsafe
			{
				byte* __path_gen_native = default;
				bool __retVal = default;

				__path_gen_native = (byte*)Marshal.StringToCoTaskMemAnsi(path);

				try
				{
					__retVal = ulBitmapWritePNG__PInvoke__(bitmap, __path_gen_native) != 0;
				}
				finally
				{
					Marshal.FreeCoTaskMem((IntPtr)__path_gen_native);
				}

				return __retVal;
			}
#endif
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", CharSet = System.Runtime.InteropServices.CharSet.Ansi, EntryPoint = "ulBitmapWritePNG")]
		extern private static unsafe byte ulBitmapWritePNG__PInvoke__(global::System.IntPtr bitmap, byte* path);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetCachePath(global::System.IntPtr config, string cache_path)
		{
			unsafe
			{
				global::System.IntPtr __cache_path_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __cache_path_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__cache_path_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(cache_path);
					__cache_path_gen_native = __cache_path_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulConfigSetCachePath__PInvoke__(config, __cache_path_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__cache_path_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetCachePath")]
		extern private static unsafe void ulConfigSetCachePath__PInvoke__(global::System.IntPtr config, global::System.IntPtr cache_path);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetUseGPURenderer(global::System.IntPtr config, bool use_gpu)
		{
			unsafe
			{
				byte __use_gpu_gen_native = default;
				//
				// Marshal
				//
				__use_gpu_gen_native = (byte)(use_gpu ? 1 : 0);
				//
				// Invoke
				//
				ulConfigSetUseGPURenderer__PInvoke__(config, __use_gpu_gen_native);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetUseGPURenderer")]
		extern private static unsafe void ulConfigSetUseGPURenderer__PInvoke__(global::System.IntPtr config, byte use_gpu);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetDeviceScale(global::System.IntPtr config, double value)
		{
			unsafe
			{
				//
				// Invoke
				//
				ulConfigSetDeviceScale__PInvoke__(config, value);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetDeviceScale")]
		extern private static unsafe void ulConfigSetDeviceScale__PInvoke__(global::System.IntPtr config, double value);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetFaceWinding(global::System.IntPtr config, global::UltralightNet.ULFaceWinding winding)
		{
			unsafe
			{
				//
				// Invoke
				//
				ulConfigSetFaceWinding__PInvoke__(config, winding);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetFaceWinding")]
		extern private static unsafe void ulConfigSetFaceWinding__PInvoke__(global::System.IntPtr config, global::UltralightNet.ULFaceWinding winding);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetEnableImages(global::System.IntPtr config, bool enabled)
		{
			unsafe
			{
				byte __enabled_gen_native = default;
				//
				// Marshal
				//
				__enabled_gen_native = (byte)(enabled ? 1 : 0);
				//
				// Invoke
				//
				ulConfigSetEnableImages__PInvoke__(config, __enabled_gen_native);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetEnableImages")]
		extern private static unsafe void ulConfigSetEnableImages__PInvoke__(global::System.IntPtr config, byte enabled);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetEnableJavaScript(global::System.IntPtr config, bool enabled)
		{
			unsafe
			{
				byte __enabled_gen_native = default;
				//
				// Marshal
				//
				__enabled_gen_native = (byte)(enabled ? 1 : 0);
				//
				// Invoke
				//
				ulConfigSetEnableJavaScript__PInvoke__(config, __enabled_gen_native);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetEnableJavaScript")]
		extern private static unsafe void ulConfigSetEnableJavaScript__PInvoke__(global::System.IntPtr config, byte enabled);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetFontHinting(global::System.IntPtr config, global::UltralightNet.ULFontHinting font_hinting)
		{
			unsafe
			{
				//
				// Invoke
				//
				ulConfigSetFontHinting__PInvoke__(config, font_hinting);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetFontHinting")]
		extern private static unsafe void ulConfigSetFontHinting__PInvoke__(global::System.IntPtr config, global::UltralightNet.ULFontHinting font_hinting);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetFontGamma(global::System.IntPtr config, double font_gamma)
		{
			unsafe
			{
				//
				// Invoke
				//
				ulConfigSetFontGamma__PInvoke__(config, font_gamma);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetFontGamma")]
		extern private static unsafe void ulConfigSetFontGamma__PInvoke__(global::System.IntPtr config, double font_gamma);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetFontFamilyStandard(global::System.IntPtr config, string font_name)
		{
			unsafe
			{
				global::System.IntPtr __font_name_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __font_name_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__font_name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(font_name);
					__font_name_gen_native = __font_name_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulConfigSetFontFamilyStandard__PInvoke__(config, __font_name_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__font_name_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetFontFamilyStandard")]
		extern private static unsafe void ulConfigSetFontFamilyStandard__PInvoke__(global::System.IntPtr config, global::System.IntPtr font_name);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetFontFamilyFixed(global::System.IntPtr config, string font_name)
		{
			unsafe
			{
				global::System.IntPtr __font_name_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __font_name_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__font_name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(font_name);
					__font_name_gen_native = __font_name_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulConfigSetFontFamilyFixed__PInvoke__(config, __font_name_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__font_name_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetFontFamilyFixed")]
		extern private static unsafe void ulConfigSetFontFamilyFixed__PInvoke__(global::System.IntPtr config, global::System.IntPtr font_name);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetFontFamilySerif(global::System.IntPtr config, string font_name)
		{
			unsafe
			{
				global::System.IntPtr __font_name_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __font_name_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__font_name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(font_name);
					__font_name_gen_native = __font_name_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulConfigSetFontFamilySerif__PInvoke__(config, __font_name_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__font_name_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetFontFamilySerif")]
		extern private static unsafe void ulConfigSetFontFamilySerif__PInvoke__(global::System.IntPtr config, global::System.IntPtr font_name);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetFontFamilySansSerif(global::System.IntPtr config, string font_name)
		{
			unsafe
			{
				global::System.IntPtr __font_name_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __font_name_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__font_name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(font_name);
					__font_name_gen_native = __font_name_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulConfigSetFontFamilySansSerif__PInvoke__(config, __font_name_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__font_name_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetFontFamilySansSerif")]
		extern private static unsafe void ulConfigSetFontFamilySansSerif__PInvoke__(global::System.IntPtr config, global::System.IntPtr font_name);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetUserAgent(global::System.IntPtr config, string font_name)
		{
			unsafe
			{
				global::System.IntPtr __font_name_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __font_name_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__font_name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(font_name);
					__font_name_gen_native = __font_name_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulConfigSetUserAgent__PInvoke__(config, __font_name_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__font_name_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetUserAgent")]
		extern private static unsafe void ulConfigSetUserAgent__PInvoke__(global::System.IntPtr config, global::System.IntPtr font_name);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetUserStylesheet(global::System.IntPtr config, string font_name)
		{
			unsafe
			{
				global::System.IntPtr __font_name_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __font_name_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__font_name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(font_name);
					__font_name_gen_native = __font_name_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulConfigSetUserStylesheet__PInvoke__(config, __font_name_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__font_name_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetUserStylesheet")]
		extern private static unsafe void ulConfigSetUserStylesheet__PInvoke__(global::System.IntPtr config, global::System.IntPtr font_name);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetForceRepaint(global::System.IntPtr config, bool enabled)
		{
			unsafe
			{
				byte __enabled_gen_native = default;
				//
				// Marshal
				//
				__enabled_gen_native = (byte)(enabled ? 1 : 0);
				//
				// Invoke
				//
				ulConfigSetForceRepaint__PInvoke__(config, __enabled_gen_native);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetForceRepaint")]
		extern private static unsafe void ulConfigSetForceRepaint__PInvoke__(global::System.IntPtr config, byte enabled);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetResourcePath(global::System.IntPtr config, string resource_path)
		{
			unsafe
			{
				global::System.IntPtr __resource_path_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __resource_path_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__resource_path_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(resource_path);
					__resource_path_gen_native = __resource_path_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulConfigSetResourcePath__PInvoke__(config, __resource_path_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__resource_path_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetResourcePath")]
		extern private static unsafe void ulConfigSetResourcePath__PInvoke__(global::System.IntPtr config, global::System.IntPtr resource_path);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetAnimationTimerDelay(global::System.IntPtr config, double delay)
		{
			unsafe
			{
				//
				// Invoke
				//
				ulConfigSetAnimationTimerDelay__PInvoke__(config, delay);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetAnimationTimerDelay")]
		extern private static unsafe void ulConfigSetAnimationTimerDelay__PInvoke__(global::System.IntPtr config, double delay);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetScrollTimerDelay(global::System.IntPtr config, double delay)
		{
			unsafe
			{
				//
				// Invoke
				//
				ulConfigSetScrollTimerDelay__PInvoke__(config, delay);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetScrollTimerDelay")]
		extern private static unsafe void ulConfigSetScrollTimerDelay__PInvoke__(global::System.IntPtr config, double delay);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulConfigSetRecycleDelay(global::System.IntPtr config, double delay)
		{
			unsafe
			{
				//
				// Invoke
				//
				ulConfigSetRecycleDelay__PInvoke__(config, delay);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetRecycleDelay")]
		extern private static unsafe void ulConfigSetRecycleDelay__PInvoke__(global::System.IntPtr config, double delay);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulIntRectIsEmpty(global::UltralightNet.ULIntRect rect)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulIntRectIsEmpty__PInvoke__(rect);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulIntRectIsEmpty")]
		extern private static unsafe byte ulIntRectIsEmpty__PInvoke__(global::UltralightNet.ULIntRect rect);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulRectIsEmpty(global::UltralightNet.ULRect rect)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulRectIsEmpty__PInvoke__(rect);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulRectIsEmpty")]
		extern private static unsafe byte ulRectIsEmpty__PInvoke__(global::UltralightNet.ULRect rect);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial global::UltralightNet.ULRect ulRectMakeEmpty()
		{
			unsafe
			{
				global::UltralightNet.ULRect __retVal = default;
				//
				// Invoke
				//
				__retVal = ulRectMakeEmpty__PInvoke__();
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulRectMakeEmpty")]
		extern private static unsafe global::UltralightNet.ULRect ulRectMakeEmpty__PInvoke__();
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial global::System.IntPtr ulCreateString(string str)
		{
#if NET5_0_OR_GREATER
			unsafe
            {
                byte *__str_gen_native = default;
                global::System.IntPtr __retVal = default;
                //
                // Setup
                //
                bool str__allocated = false;
                try
                {
                    //
                    // Marshal
                    //
                    if (System.OperatingSystem.IsWindows())
                    {
                        __str_gen_native = (byte *)System.Runtime.InteropServices.Marshal.StringToCoTaskMemAnsi(str);
                        str__allocated = true;
                    }
                    else
                    {
                        if (str != null)
                        {
                            int str__bytelen = (str.Length + 1) * 3 + 1;
                            if (str__bytelen > 260)
                            {
                                __str_gen_native = (byte *)System.Runtime.InteropServices.Marshal.StringToCoTaskMemUTF8(str);
                                str__allocated = true;
                            }
                            else
                            {
                                byte *str__stackptr = stackalloc byte[str__bytelen];
                                {
                                    str__bytelen = System.Text.Encoding.UTF8.GetBytes(str, new System.Span<byte>(str__stackptr, str__bytelen));
                                    str__stackptr[str__bytelen] = 0;
                                }

                                __str_gen_native = (byte *)str__stackptr;
                            }
                        }
                    }

                    //
                    // Invoke
                    //
                    __retVal = ulCreateString__PInvoke__(__str_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    if (str__allocated)
                    {
                        System.Runtime.InteropServices.Marshal.FreeCoTaskMem((System.IntPtr)__str_gen_native);
                    }
                }

                return __retVal;
            }
#else
			throw new PlatformNotSupportedException();
#endif

		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", CharSet = System.Runtime.InteropServices.CharSet.Ansi, EntryPoint = "ulCreateString")]
		extern private static unsafe global::System.IntPtr ulCreateString__PInvoke__(byte* str);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial global::System.IntPtr ulCreateStringUTF8(string str, uint len)
		{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
			unsafe
            {
                byte *__str_gen_native = default;
                global::System.IntPtr __retVal = default;
                //
                // Setup
                //
                bool str__allocated = false;
                try
                {
                    //
                    // Marshal
                    //
                    if (str != null)
                    {
                        int str__bytelen = (str.Length + 1) * 3 + 1;
                        if (str__bytelen > 260)
                        {
                            __str_gen_native = (byte *)System.Runtime.InteropServices.Marshal.StringToCoTaskMemUTF8(str);
                            str__allocated = true;
                        }
                        else
                        {
                            byte *str__stackptr = stackalloc byte[str__bytelen];
                            {
                                str__bytelen = System.Text.Encoding.UTF8.GetBytes(str, new System.Span<byte>(str__stackptr, str__bytelen));
                                str__stackptr[str__bytelen] = 0;
                            }

                            __str_gen_native = (byte *)str__stackptr;
                        }
                    }

                    //
                    // Invoke
                    //
                    __retVal = ulCreateStringUTF8__PInvoke__(__str_gen_native, len);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    if (str__allocated)
                    {
                        System.Runtime.InteropServices.Marshal.FreeCoTaskMem((System.IntPtr)__str_gen_native);
                    }
                }

                return __retVal;
            }
#else
			return ulCreateStringUTF16(str, len);
#endif
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulCreateStringUTF8")]
		extern private static unsafe global::System.IntPtr ulCreateStringUTF8__PInvoke__(byte* str, uint len);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial global::System.IntPtr ulCreateStringUTF16(string str, uint len)
		{
			unsafe
			{
				ushort* __str_gen_native = default;
				global::System.IntPtr __retVal = default;
				//
				// Invoke
				//
				fixed (char* __str_gen_native__pinned = str)
					__retVal = ulCreateStringUTF16__PInvoke__((ushort*)__str_gen_native__pinned, len);
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", CharSet = System.Runtime.InteropServices.CharSet.Unicode, EntryPoint = "ulCreateStringUTF16")]
		extern private static unsafe global::System.IntPtr ulCreateStringUTF16__PInvoke__(ushort* str, uint len);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial string ulStringGetData(global::System.IntPtr str)
		{
			unsafe
			{
				string __retVal = default;
				ushort* __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulStringGetData__PInvoke__(str);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native == null ? null : new string((char*)__retVal_gen_native);
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", CharSet = System.Runtime.InteropServices.CharSet.Unicode, EntryPoint = "ulStringGetData")]
		extern private static unsafe ushort* ulStringGetData__PInvoke__(global::System.IntPtr str);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial uint ulStringGetLength(global::System.IntPtr str)
		{
			unsafe
			{
				uint __retVal = default;
				//
				// Invoke
				//
				__retVal = ulStringGetLength__PInvoke__(str);
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulStringGetLength")]
		extern private static unsafe uint ulStringGetLength__PInvoke__(global::System.IntPtr str);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulStringIsEmpty(global::System.IntPtr str)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulStringIsEmpty__PInvoke__(str);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulStringIsEmpty")]
		extern private static unsafe byte ulStringIsEmpty__PInvoke__(global::System.IntPtr str);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulStringAssignCString(global::System.IntPtr str, string c_str)
		{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
			unsafe
            {
                byte *__c_str_gen_native = default;
                //
                // Setup
                //
                bool c_str__allocated = false;
                try
                {
                    //
                    // Marshal
                    //
                    if (c_str != null)
                    {
                        int c_str__bytelen = (c_str.Length + 1) * 3 + 1;
                        if (c_str__bytelen > 260)
                        {
                            __c_str_gen_native = (byte *)System.Runtime.InteropServices.Marshal.StringToCoTaskMemUTF8(c_str);
                            c_str__allocated = true;
                        }
                        else
                        {
                            byte *c_str__stackptr = stackalloc byte[c_str__bytelen];
                            {
                                c_str__bytelen = System.Text.Encoding.UTF8.GetBytes(c_str, new System.Span<byte>(c_str__stackptr, c_str__bytelen));
                                c_str__stackptr[c_str__bytelen] = 0;
                            }

                            __c_str_gen_native = (byte *)c_str__stackptr;
                        }
                    }

                    //
                    // Invoke
                    //
                    ulStringAssignCString__PInvoke__(str, __c_str_gen_native);
                }
                finally
                {
                    //
                    // Cleanup
                    //
                    if (c_str__allocated)
                    {
                        System.Runtime.InteropServices.Marshal.FreeCoTaskMem((System.IntPtr)__c_str_gen_native);
                    }
                }
            }
#else
			unsafe
			{
				byte* dat = default;

				try
				{
					dat = (byte*)Marshal.StringToCoTaskMemAnsi(c_str);
					ulStringAssignCString__PInvoke__(str, dat);
				}
				finally
				{
					Marshal.FreeCoTaskMem((IntPtr)dat);
				}
			}
#endif
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", CharSet = System.Runtime.InteropServices.CharSet.Ansi, EntryPoint = "ulStringAssignCString")]
		extern private static unsafe void ulStringAssignCString__PInvoke__(global::System.IntPtr str, byte* c_str);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial global::System.IntPtr ulCreateView(global::System.IntPtr renderer, uint width, uint height, bool transparent, global::System.IntPtr session, bool force_cpu_renderer)
		{
			unsafe
			{
				byte __transparent_gen_native = default;
				byte __force_cpu_renderer_gen_native = default;
				global::System.IntPtr __retVal = default;
				//
				// Marshal
				//
				__transparent_gen_native = (byte)(transparent ? 1 : 0);
				__force_cpu_renderer_gen_native = (byte)(force_cpu_renderer ? 1 : 0);
				//
				// Invoke
				//
				__retVal = ulCreateView__PInvoke__(renderer, width, height, __transparent_gen_native, session, __force_cpu_renderer_gen_native);
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulCreateView")]
		extern private static unsafe global::System.IntPtr ulCreateView__PInvoke__(global::System.IntPtr renderer, uint width, uint height, byte transparent, global::System.IntPtr session, byte force_cpu_renderer);
	}
}
namespace UltralightNet
{
    public static partial class Methods
    {
        public static partial string ulViewGetURL(global::System.IntPtr view)
        {
            unsafe
            {
                string __retVal = default;
                global::System.IntPtr __retVal_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __retVal_gen_native__marshaler = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulViewGetURL__PInvoke__(view);
                //
                // Unmarshal
                //
                __retVal_gen_native__marshaler.Value = __retVal_gen_native;
                __retVal = __retVal_gen_native__marshaler.ToManaged();
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", CharSet = System.Runtime.InteropServices.CharSet.Unicode, EntryPoint = "ulViewGetURL")]
        extern private static unsafe global::System.IntPtr ulViewGetURL__PInvoke__(global::System.IntPtr view);
    }
}
namespace UltralightNet
{
    public static partial class Methods
    {
        public static partial string ulViewGetTitle(global::System.IntPtr view)
        {
            unsafe
            {
                string __retVal = default;
                global::System.IntPtr __retVal_gen_native = default;
                //
                // Setup
                //
                global::UltralightNet.ULStringGeneratedDllImportMarshaler __retVal_gen_native__marshaler = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulViewGetTitle__PInvoke__(view);
                //
                // Unmarshal
                //
                __retVal_gen_native__marshaler.Value = __retVal_gen_native;
                __retVal = __retVal_gen_native__marshaler.ToManaged();
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewGetTitle")]
        extern private static unsafe global::System.IntPtr ulViewGetTitle__PInvoke__(global::System.IntPtr view);
    }
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulViewIsLoading(global::System.IntPtr view)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulViewIsLoading__PInvoke__(view);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewIsLoading")]
		extern private static unsafe byte ulViewIsLoading__PInvoke__(global::System.IntPtr view);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial global::UltralightNet.RenderTarget ulViewGetRenderTarget(global::System.IntPtr view)
		{
			unsafe
			{
				global::UltralightNet.RenderTarget __retVal = default;
				global::UltralightNet.RenderTargetNative __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulViewGetRenderTarget__PInvoke__(view);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native.ToManaged();
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewGetRenderTarget")]
		extern private static unsafe global::UltralightNet.RenderTargetNative ulViewGetRenderTarget__PInvoke__(global::System.IntPtr view);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulViewLoadHTML(global::System.IntPtr view, string html_string)
		{
			unsafe
			{
				global::System.IntPtr __html_string_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __html_string_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__html_string_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(html_string);
					__html_string_gen_native = __html_string_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulViewLoadHTML__PInvoke__(view, __html_string_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__html_string_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewLoadHTML")]
		extern private static unsafe void ulViewLoadHTML__PInvoke__(global::System.IntPtr view, global::System.IntPtr html_string);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulViewLoadURL(global::System.IntPtr view, string url_string)
		{
			unsafe
			{
				global::System.IntPtr __url_string_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __url_string_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__url_string_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(url_string);
					__url_string_gen_native = __url_string_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulViewLoadURL__PInvoke__(view, __url_string_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__url_string_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewLoadURL")]
		extern private static unsafe void ulViewLoadURL__PInvoke__(global::System.IntPtr view, global::System.IntPtr url_string);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial string ulViewEvaluateScript(global::System.IntPtr view, string js_string, out string exception)
		{
			unsafe
			{
				global::System.IntPtr __js_string_gen_native = default;
				exception = default;
				global::System.IntPtr __exception_gen_native = default;
				string __retVal = default;
				global::System.IntPtr __retVal_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __retVal_gen_native__marshaler = default;
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __js_string_gen_native__marshaler = default;
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __exception_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__js_string_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(js_string);
					__js_string_gen_native = __js_string_gen_native__marshaler.Value;
					//
					// Invoke
					//
					__retVal_gen_native = ulViewEvaluateScript__PInvoke__(view, __js_string_gen_native, &__exception_gen_native);
					//
					// Unmarshal
					//
					__retVal_gen_native__marshaler.Value = __retVal_gen_native;
					__retVal = __retVal_gen_native__marshaler.ToManaged();
					__exception_gen_native__marshaler.Value = __exception_gen_native;
					exception = __exception_gen_native__marshaler.ToManaged();
				}
				finally
				{
					//
					// Cleanup
					//
					__js_string_gen_native__marshaler.FreeNative();
				}

				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewEvaluateScript")]
		extern private static unsafe global::System.IntPtr ulViewEvaluateScript__PInvoke__(global::System.IntPtr view, global::System.IntPtr js_string, global::System.IntPtr* exception);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulViewCanGoBack(global::System.IntPtr view)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulViewCanGoBack__PInvoke__(view);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewCanGoBack")]
		extern private static unsafe byte ulViewCanGoBack__PInvoke__(global::System.IntPtr view);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulViewCanGoForward(global::System.IntPtr view)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulViewCanGoForward__PInvoke__(view);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewCanGoForward")]
		extern private static unsafe byte ulViewCanGoForward__PInvoke__(global::System.IntPtr view);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulViewHasFocus(global::System.IntPtr view)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulViewHasFocus__PInvoke__(view);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewHasFocus")]
		extern private static unsafe byte ulViewHasFocus__PInvoke__(global::System.IntPtr view);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulViewHasInputFocus(global::System.IntPtr view)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulViewHasInputFocus__PInvoke__(view);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewHasInputFocus")]
		extern private static unsafe byte ulViewHasInputFocus__PInvoke__(global::System.IntPtr view);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulViewFireScrollEvent(global::System.IntPtr view, global::UltralightNet.ULScrollEvent scroll_event)
		{
			unsafe
			{
				global::UltralightNet.ULScrollEventNative __scroll_event_gen_native = default;
				//
				// Marshal
				//
				__scroll_event_gen_native = new global::UltralightNet.ULScrollEventNative(scroll_event);
				//
				// Invoke
				//
				ulViewFireScrollEvent__PInvoke__(view, __scroll_event_gen_native);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewFireScrollEvent")]
		extern private static unsafe void ulViewFireScrollEvent__PInvoke__(global::System.IntPtr view, global::UltralightNet.ULScrollEventNative scroll_event);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial void ulViewSetNeedsPaint(global::System.IntPtr view, bool needs_paint)
		{
			unsafe
			{
				byte __needs_paint_gen_native = default;
				//
				// Marshal
				//
				__needs_paint_gen_native = (byte)(needs_paint ? 1 : 0);
				//
				// Invoke
				//
				ulViewSetNeedsPaint__PInvoke__(view, __needs_paint_gen_native);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewSetNeedsPaint")]
		extern private static unsafe void ulViewSetNeedsPaint__PInvoke__(global::System.IntPtr view, byte needs_paint);
	}
}
namespace UltralightNet
{
	public static partial class Methods
	{
		public static partial bool ulViewGetNeedsPaint(global::System.IntPtr view)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulViewGetNeedsPaint__PInvoke__(view);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulViewGetNeedsPaint")]
		extern private static unsafe byte ulViewGetNeedsPaint__PInvoke__(global::System.IntPtr view);
	}
}
#pragma warning restore IDE0059

#endif
