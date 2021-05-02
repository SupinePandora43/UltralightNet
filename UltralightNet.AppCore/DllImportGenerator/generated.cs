using System;

#if !GENERATED

#pragma warning disable IDE0059

namespace UltralightNet.AppCore
{

	public static partial class AppCoreMethods
{
	public static partial void ulSettingsSetDeveloperName(global::System.IntPtr settings, string name)
	{
		unsafe
		{
			global::System.IntPtr __name_gen_native = default;
			//
			// Setup
			//
			global::UltralightNet.ULStringGeneratedDllImportMarshaler __name_gen_native__marshaler = default;
			try
			{
				//
				// Marshal
				//
				__name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(name);
				__name_gen_native = __name_gen_native__marshaler.Value;
				//
				// Invoke
				//
				ulSettingsSetDeveloperName__PInvoke__(settings, __name_gen_native);
			}
			finally
			{
				//
				// Cleanup
				//
				__name_gen_native__marshaler.FreeNative();
			}
		}
	}

	[System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulSettingsSetDeveloperName")]
	extern private static unsafe void ulSettingsSetDeveloperName__PInvoke__(global::System.IntPtr settings, global::System.IntPtr name);
}
}
namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		public static partial void ulSettingsSetAppName(global::System.IntPtr settings, string name)
		{
			unsafe
			{
				global::System.IntPtr __name_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __name_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__name_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(name);
					__name_gen_native = __name_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulSettingsSetAppName__PInvoke__(settings, __name_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__name_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulSettingsSetAppName")]
		extern private static unsafe void ulSettingsSetAppName__PInvoke__(global::System.IntPtr settings, global::System.IntPtr name);
	}
}
namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		public static partial void ulSettingsSetFileSystemPath(global::System.IntPtr settings, string path)
		{
			unsafe
			{
				global::System.IntPtr __path_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __path_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__path_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(path);
					__path_gen_native = __path_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulSettingsSetFileSystemPath__PInvoke__(settings, __path_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__path_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulSettingsSetFileSystemPath")]
		extern private static unsafe void ulSettingsSetFileSystemPath__PInvoke__(global::System.IntPtr settings, global::System.IntPtr path);
	}
}
namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		public static partial void ulSettingsSetLoadShadersFromFileSystem(global::System.IntPtr settings, bool enabled)
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
				ulSettingsSetLoadShadersFromFileSystem__PInvoke__(settings, __enabled_gen_native);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulSettingsSetLoadShadersFromFileSystem")]
		extern private static unsafe void ulSettingsSetLoadShadersFromFileSystem__PInvoke__(global::System.IntPtr settings, byte enabled);
	}
}
namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		public static partial void ulSettingsSetForceCPURenderer(global::System.IntPtr settings, bool force_cpu)
		{
			unsafe
			{
				byte __force_cpu_gen_native = default;
				//
				// Marshal
				//
				__force_cpu_gen_native = (byte)(force_cpu ? 1 : 0);
				//
				// Invoke
				//
				ulSettingsSetForceCPURenderer__PInvoke__(settings, __force_cpu_gen_native);
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulSettingsSetForceCPURenderer")]
		extern private static unsafe void ulSettingsSetForceCPURenderer__PInvoke__(global::System.IntPtr settings, byte force_cpu);
	}
}
namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		public static partial bool ulOverlayIsHidden(global::System.IntPtr overlay)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulOverlayIsHidden__PInvoke__(overlay);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulOverlayIsHidden")]
		extern private static unsafe byte ulOverlayIsHidden__PInvoke__(global::System.IntPtr overlay);
	}
}
namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		public static partial bool ulOverlayHasFocus(global::System.IntPtr overlay)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulOverlayHasFocus__PInvoke__(overlay);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulOverlayHasFocus")]
		extern private static unsafe byte ulOverlayHasFocus__PInvoke__(global::System.IntPtr overlay);
	}
}
namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		public static partial void ulEnablePlatformFileSystem(string base_dir)
		{
			unsafe
			{
				global::System.IntPtr __base_dir_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __base_dir_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__base_dir_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(base_dir);
					__base_dir_gen_native = __base_dir_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulEnablePlatformFileSystem__PInvoke__(__base_dir_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__base_dir_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulEnablePlatformFileSystem")]
		extern private static unsafe void ulEnablePlatformFileSystem__PInvoke__(global::System.IntPtr base_dir);
	}
}
namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		public static partial void ulEnableDefaultLogger(string log_path)
		{
			unsafe
			{
				global::System.IntPtr __log_path_gen_native = default;
				//
				// Setup
				//
				global::UltralightNet.ULStringGeneratedDllImportMarshaler __log_path_gen_native__marshaler = default;
				try
				{
					//
					// Marshal
					//
					__log_path_gen_native__marshaler = new global::UltralightNet.ULStringGeneratedDllImportMarshaler(log_path);
					__log_path_gen_native = __log_path_gen_native__marshaler.Value;
					//
					// Invoke
					//
					ulEnableDefaultLogger__PInvoke__(__log_path_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					__log_path_gen_native__marshaler.FreeNative();
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulEnableDefaultLogger")]
		extern private static unsafe void ulEnableDefaultLogger__PInvoke__(global::System.IntPtr log_path);
	}
}
namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		public static partial bool ulAppIsRunning(global::System.IntPtr app)
		{
			unsafe
			{
				bool __retVal = default;
				int __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulAppIsRunning__PInvoke__(app);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulAppIsRunning")]
		extern private static unsafe int ulAppIsRunning__PInvoke__(global::System.IntPtr app);
	}
}
namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		public static partial global::System.IntPtr ulCreateWindow(global::System.IntPtr monitor, uint width, uint height, bool fullscreen, global::UltralightNet.AppCore.ULWindowFlags flags)
		{
			unsafe
			{
				byte __fullscreen_gen_native = default;
				global::System.IntPtr __retVal = default;
				//
				// Marshal
				//
				__fullscreen_gen_native = (byte)(fullscreen ? 1 : 0);
				//
				// Invoke
				//
				__retVal = ulCreateWindow__PInvoke__(monitor, width, height, __fullscreen_gen_native, flags);
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulCreateWindow")]
		extern private static unsafe global::System.IntPtr ulCreateWindow__PInvoke__(global::System.IntPtr monitor, uint width, uint height, byte fullscreen, global::UltralightNet.AppCore.ULWindowFlags flags);
	}
}
namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		public static partial bool ulWindowIsFullscreen(global::System.IntPtr window)
		{
			unsafe
			{
				bool __retVal = default;
				byte __retVal_gen_native = default;
				//
				// Invoke
				//
				__retVal_gen_native = ulWindowIsFullscreen__PInvoke__(window);
				//
				// Unmarshal
				//
				__retVal = __retVal_gen_native != 0;
				return __retVal;
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulWindowIsFullscreen")]
		extern private static unsafe byte ulWindowIsFullscreen__PInvoke__(global::System.IntPtr window);
	}
}
namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		public static partial void ulWindowSetTitle(global::System.IntPtr window, string title)
		{
#if NET5_0_OR_GREATER
			unsafe
			{
				byte* __title_gen_native = default;
				//
				// Setup
				//
				bool title__allocated = false;
				try
				{
					//
					// Marshal
					//
					if (System.OperatingSystem.IsWindows())
					{
						__title_gen_native = (byte*)System.Runtime.InteropServices.Marshal.StringToCoTaskMemAnsi(title);
						title__allocated = true;
					}
					else
					{
						if (title != null)
						{
							int title__bytelen = (title.Length + 1) * 3 + 1;
							if (title__bytelen > 260)
							{
								__title_gen_native = (byte*)System.Runtime.InteropServices.Marshal.StringToCoTaskMemUTF8(title);
								title__allocated = true;
							}
							else
							{
								byte* title__stackptr = stackalloc byte[title__bytelen];
								{
									title__bytelen = System.Text.Encoding.UTF8.GetBytes(title, new System.Span<byte>(title__stackptr, title__bytelen));
									title__stackptr[title__bytelen] = 0;
								}

								__title_gen_native = (byte*)title__stackptr;
							}
						}
					}

					//
					// Invoke
					//
					ulWindowSetTitle__PInvoke__(window, __title_gen_native);
				}
				finally
				{
					//
					// Cleanup
					//
					if (title__allocated)
					{
						System.Runtime.InteropServices.Marshal.FreeCoTaskMem((System.IntPtr)__title_gen_native);
					}
				}
			}
#else
			throw new PlatformNotSupportedException();
#endif
		}

		[System.Runtime.InteropServices.DllImportAttribute("AppCore", CharSet = System.Runtime.InteropServices.CharSet.Ansi, EntryPoint = "ulWindowSetTitle")]
		extern private static unsafe void ulWindowSetTitle__PInvoke__(global::System.IntPtr window, byte* title);
	}
}
namespace UltralightNet.AppCore
{
    public static partial class AppCoreMethods
    {
        public static partial bool ulWindowIsVisible(global::System.IntPtr window)
        {
            unsafe
            {
                bool __retVal = default;
                byte __retVal_gen_native = default;
                //
                // Invoke
                //
                __retVal_gen_native = ulWindowIsVisible__PInvoke__(window);
                //
                // Unmarshal
                //
                __retVal = __retVal_gen_native != 0;
                return __retVal;
            }
        }

        [System.Runtime.InteropServices.DllImportAttribute("AppCore", EntryPoint = "ulWindowIsVisible")]
        extern private static unsafe byte ulWindowIsVisible__PInvoke__(global::System.IntPtr window);
    }
}

#pragma warning restore IDE0059

#endif
