using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public partial struct C_Config
	{
	}

	public partial struct C_Renderer
	{
	}

	public partial struct C_Session
	{
	}

	public partial struct C_View
	{
	}

	public partial struct C_Bitmap
	{
	}

	public partial struct C_String
	{
	}

	public partial struct C_Buffer
	{
	}

	public partial struct C_KeyEvent
	{
	}

	public partial struct C_MouseEvent
	{
	}

	public partial struct C_ScrollEvent
	{
	}

	public partial struct C_Surface
	{
	}

	public enum ULMessageSource
	{
		kMessageSource_XML = 0,
		kMessageSource_JS,
		kMessageSource_Network,
		kMessageSource_ConsoleAPI,
		kMessageSource_Storage,
		kMessageSource_AppCache,
		kMessageSource_Rendering,
		kMessageSource_CSS,
		kMessageSource_Security,
		kMessageSource_ContentBlocker,
		kMessageSource_Other,
	}

	public enum ULMessageLevel
	{
		kMessageLevel_Log = 1,
		kMessageLevel_Warning = 2,
		kMessageLevel_Error = 3,
		kMessageLevel_Debug = 4,
		kMessageLevel_Info = 5,
	}

	public enum ULCursor
	{
		kCursor_Pointer = 0,
		kCursor_Cross,
		kCursor_Hand,
		kCursor_IBeam,
		kCursor_Wait,
		kCursor_Help,
		kCursor_EastResize,
		kCursor_NorthResize,
		kCursor_NorthEastResize,
		kCursor_NorthWestResize,
		kCursor_SouthResize,
		kCursor_SouthEastResize,
		kCursor_SouthWestResize,
		kCursor_WestResize,
		kCursor_NorthSouthResize,
		kCursor_EastWestResize,
		kCursor_NorthEastSouthWestResize,
		kCursor_NorthWestSouthEastResize,
		kCursor_ColumnResize,
		kCursor_RowResize,
		kCursor_MiddlePanning,
		kCursor_EastPanning,
		kCursor_NorthPanning,
		kCursor_NorthEastPanning,
		kCursor_NorthWestPanning,
		kCursor_SouthPanning,
		kCursor_SouthEastPanning,
		kCursor_SouthWestPanning,
		kCursor_WestPanning,
		kCursor_Move,
		kCursor_VerticalText,
		kCursor_Cell,
		kCursor_ContextMenu,
		kCursor_Alias,
		kCursor_Progress,
		kCursor_NoDrop,
		kCursor_Copy,
		kCursor_None,
		kCursor_NotAllowed,
		kCursor_ZoomIn,
		kCursor_ZoomOut,
		kCursor_Grab,
		kCursor_Grabbing,
		kCursor_Custom,
	}

	public enum ULBitmapFormat
	{
		kBitmapFormat_A8_UNORM,
		kBitmapFormat_BGRA8_UNORM_SRGB,
	}

	public enum ULKeyEventType
	{
		kKeyEventType_KeyDown,
		kKeyEventType_KeyUp,
		kKeyEventType_RawKeyDown,
		kKeyEventType_Char,
	}

	public enum ULMouseEventType
	{
		kMouseEventType_MouseMoved,
		kMouseEventType_MouseDown,
		kMouseEventType_MouseUp,
	}

	public enum ULMouseButton
	{
		kMouseButton_None = 0,
		kMouseButton_Left,
		kMouseButton_Middle,
		kMouseButton_Right,
	}

	public enum ULScrollEventType
	{
		kScrollEventType_ScrollByPixel,
		kScrollEventType_ScrollByPage,
	}

	public enum ULFaceWinding
	{
		kFaceWinding_Clockwise,
		kFaceWindow_CounterClockwise,
	}

	public enum ULFontHinting
	{
		kFontHinting_Smooth,
		kFontHinting_Normal,
		kFontHinting_Monochrome,
	}

	public partial struct ULRect
	{
		public float left;

		public float top;

		public float right;

		public float bottom;
	}

	public partial struct ULIntRect
	{
		public int left;

		public int top;

		public int right;

		public int bottom;
	}

	public partial struct ULRenderTarget
	{
		public bool is_empty;

		[NativeTypeName("unsigned int")]
		public uint width;

		[NativeTypeName("unsigned int")]
		public uint height;

		[NativeTypeName("unsigned int")]
		public uint texture_id;

		[NativeTypeName("unsigned int")]
		public uint texture_width;

		[NativeTypeName("unsigned int")]
		public uint texture_height;

		public ULBitmapFormat texture_format;

		public ULRect uv_coords;

		[NativeTypeName("unsigned int")]
		public uint render_buffer_id;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULChangeTitleCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("ULView")] C_View* caller, [NativeTypeName("ULString")] C_String* title);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULChangeURLCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("ULView")] C_View* caller, [NativeTypeName("ULString")] C_String* url);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULChangeTooltipCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("ULView")] C_View* caller, [NativeTypeName("ULString")] C_String* tooltip);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULChangeCursorCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("ULView")] C_View* caller, ULCursor cursor);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULAddConsoleMessageCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("ULView")] C_View* caller, ULMessageSource source, ULMessageLevel level, [NativeTypeName("ULString")] C_String* message, [NativeTypeName("unsigned int")] uint line_number, [NativeTypeName("unsigned int")] uint column_number, [NativeTypeName("ULString")] C_String* source_id);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: NativeTypeName("ULView")]
	public unsafe delegate C_View* ULCreateChildViewCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("ULView")] C_View* caller, [NativeTypeName("ULString")] C_String* opener_url, [NativeTypeName("ULString")] C_String* target_url, bool is_popup, ULIntRect popup_rect);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULBeginLoadingCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("ULView")] C_View* caller, [NativeTypeName("unsigned long long")] ulong frame_id, bool is_main_frame, [NativeTypeName("ULString")] C_String* url);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULFinishLoadingCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("ULView")] C_View* caller, [NativeTypeName("unsigned long long")] ulong frame_id, bool is_main_frame, [NativeTypeName("ULString")] C_String* url);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULFailLoadingCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("ULView")] C_View* caller, [NativeTypeName("unsigned long long")] ulong frame_id, bool is_main_frame, [NativeTypeName("ULString")] C_String* url, [NativeTypeName("ULString")] C_String* description, [NativeTypeName("ULString")] C_String* error_domain, int error_code);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULWindowObjectReadyCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("ULView")] C_View* caller, [NativeTypeName("unsigned long long")] ulong frame_id, bool is_main_frame, [NativeTypeName("ULString")] C_String* url);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULDOMReadyCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("ULView")] C_View* caller, [NativeTypeName("unsigned long long")] ulong frame_id, bool is_main_frame, [NativeTypeName("ULString")] C_String* url);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULUpdateHistoryCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("ULView")] C_View* caller);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: NativeTypeName("void *")]
	public unsafe delegate void* ULSurfaceDefinitionCreateCallback([NativeTypeName("unsigned int")] uint width, [NativeTypeName("unsigned int")] uint height);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULSurfaceDefinitionDestroyCallback([NativeTypeName("void *")] void* user_data);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: NativeTypeName("unsigned int")]
	public unsafe delegate uint ULSurfaceDefinitionGetWidthCallback([NativeTypeName("void *")] void* user_data);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: NativeTypeName("unsigned int")]
	public unsafe delegate uint ULSurfaceDefinitionGetHeightCallback([NativeTypeName("void *")] void* user_data);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: NativeTypeName("unsigned int")]
	public unsafe delegate uint ULSurfaceDefinitionGetRowBytesCallback([NativeTypeName("void *")] void* user_data);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: NativeTypeName("size_t")]
	public unsafe delegate UIntPtr ULSurfaceDefinitionGetSizeCallback([NativeTypeName("void *")] void* user_data);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: NativeTypeName("void *")]
	public unsafe delegate void* ULSurfaceDefinitionLockPixelsCallback([NativeTypeName("void *")] void* user_data);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULSurfaceDefinitionUnlockPixelsCallback([NativeTypeName("void *")] void* user_data);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULSurfaceDefinitionResizeCallback([NativeTypeName("void *")] void* user_data, [NativeTypeName("unsigned int")] uint width, [NativeTypeName("unsigned int")] uint height);

	public partial struct ULSurfaceDefinition
	{
		[NativeTypeName("ULSurfaceDefinitionCreateCallback")]
		public IntPtr create;

		[NativeTypeName("ULSurfaceDefinitionDestroyCallback")]
		public IntPtr destroy;

		[NativeTypeName("ULSurfaceDefinitionGetWidthCallback")]
		public IntPtr get_width;

		[NativeTypeName("ULSurfaceDefinitionGetHeightCallback")]
		public IntPtr get_height;

		[NativeTypeName("ULSurfaceDefinitionGetRowBytesCallback")]
		public IntPtr get_row_bytes;

		[NativeTypeName("ULSurfaceDefinitionGetSizeCallback")]
		public IntPtr get_size;

		[NativeTypeName("ULSurfaceDefinitionLockPixelsCallback")]
		public IntPtr lock_pixels;

		[NativeTypeName("ULSurfaceDefinitionUnlockPixelsCallback")]
		public IntPtr unlock_pixels;

		[NativeTypeName("ULSurfaceDefinitionResizeCallback")]
		public IntPtr resize;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate bool ULFileSystemFileExistsCallback([NativeTypeName("ULString")] C_String* path);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate bool ULFileSystemGetFileSizeCallback([NativeTypeName("ULFileHandle")] UIntPtr handle, [NativeTypeName("long long *")] long* result);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate bool ULFileSystemGetFileMimeTypeCallback([NativeTypeName("ULString")] C_String* path, [NativeTypeName("ULString")] C_String* result);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: NativeTypeName("ULFileHandle")]
	public unsafe delegate UIntPtr ULFileSystemOpenFileCallback([NativeTypeName("ULString")] C_String* path, bool open_for_writing);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULFileSystemCloseFileCallback([NativeTypeName("ULFileHandle")] UIntPtr handle);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: NativeTypeName("long long")]
	public unsafe delegate long ULFileSystemReadFromFileCallback([NativeTypeName("ULFileHandle")] UIntPtr handle, [NativeTypeName("char *")] sbyte* data, [NativeTypeName("long long")] long length);

	public partial struct ULFileSystem
	{
		[NativeTypeName("ULFileSystemFileExistsCallback")]
		public IntPtr file_exists;

		[NativeTypeName("ULFileSystemGetFileSizeCallback")]
		public IntPtr get_file_size;

		[NativeTypeName("ULFileSystemGetFileMimeTypeCallback")]
		public IntPtr get_file_mime_type;

		[NativeTypeName("ULFileSystemOpenFileCallback")]
		public IntPtr open_file;

		[NativeTypeName("ULFileSystemCloseFileCallback")]
		public IntPtr close_file;

		[NativeTypeName("ULFileSystemReadFromFileCallback")]
		public IntPtr read_from_file;
	}

	public enum ULLogLevel
	{
		kLogLevel_Error = 0,
		kLogLevel_Warning,
		kLogLevel_Info,
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULLoggerLogMessageCallback(ULLogLevel log_level, [NativeTypeName("ULString")] C_String* message);

	public partial struct ULLogger
	{
		[NativeTypeName("ULLoggerLogMessageCallback")]
		public IntPtr log_message;
	}

	public partial struct ULRenderBuffer
	{
		[NativeTypeName("unsigned int")]
		public uint texture_id;

		[NativeTypeName("unsigned int")]
		public uint width;

		[NativeTypeName("unsigned int")]
		public uint height;

		public bool has_stencil_buffer;

		public bool has_depth_buffer;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public unsafe partial struct ULVertex_2f_4ub_2f
	{
		[NativeTypeName("float [2]")]
		public fixed float pos[2];

		[NativeTypeName("unsigned char [4]")]
		public fixed byte color[4];

		[NativeTypeName("float [2]")]
		public fixed float obj[2];
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public unsafe partial struct ULVertex_2f_4ub_2f_2f_28f
	{
		[NativeTypeName("float [2]")]
		public fixed float pos[2];

		[NativeTypeName("unsigned char [4]")]
		public fixed byte color[4];

		[NativeTypeName("float [2]")]
		public fixed float tex[2];

		[NativeTypeName("float [2]")]
		public fixed float obj[2];

		[NativeTypeName("float [4]")]
		public fixed float data0[4];

		[NativeTypeName("float [4]")]
		public fixed float data1[4];

		[NativeTypeName("float [4]")]
		public fixed float data2[4];

		[NativeTypeName("float [4]")]
		public fixed float data3[4];

		[NativeTypeName("float [4]")]
		public fixed float data4[4];

		[NativeTypeName("float [4]")]
		public fixed float data5[4];

		[NativeTypeName("float [4]")]
		public fixed float data6[4];
	}

	public enum ULVertexBufferFormat
	{
		kVertexBufferFormat_2f_4ub_2f,
		kVertexBufferFormat_2f_4ub_2f_2f_28f,
	}

	public unsafe partial struct ULVertexBuffer
	{
		public ULVertexBufferFormat format;

		[NativeTypeName("unsigned int")]
		public uint size;

		[NativeTypeName("unsigned char *")]
		public byte* data;
	}

	public unsafe partial struct ULIndexBuffer
	{
		[NativeTypeName("unsigned int")]
		public uint size;

		[NativeTypeName("unsigned char *")]
		public byte* data;
	}

	public enum ULShaderType
	{
		kShaderType_Fill,
		kShaderType_FillPath,
	}

	public unsafe partial struct ULMatrix4x4
	{
		[NativeTypeName("float [16]")]
		public fixed float data[16];
	}

	public unsafe partial struct ULvec4
	{
		[NativeTypeName("float [4]")]
		public fixed float value[4];
	}

	public unsafe partial struct ULGPUState
	{
		[NativeTypeName("unsigned int")]
		public uint viewport_width;

		[NativeTypeName("unsigned int")]
		public uint viewport_height;

		public ULMatrix4x4 transform;

		public bool enable_texturing;

		public bool enable_blend;

		[NativeTypeName("unsigned char")]
		public byte shader_type;

		[NativeTypeName("unsigned int")]
		public uint render_buffer_id;

		[NativeTypeName("unsigned int")]
		public uint texture_1_id;

		[NativeTypeName("unsigned int")]
		public uint texture_2_id;

		[NativeTypeName("unsigned int")]
		public uint texture_3_id;

		[NativeTypeName("float [8]")]
		public fixed float uniform_scalar[8];

		[NativeTypeName("ULvec4 [8]")]
		public _uniform_vector_e__FixedBuffer uniform_vector;

		[NativeTypeName("unsigned char")]
		public byte clip_size;

		[NativeTypeName("ULMatrix4x4 [8]")]
		public _clip_e__FixedBuffer clip;

		public bool enable_scissor;

		public ULIntRect scissor_rect;

		public partial struct _uniform_vector_e__FixedBuffer
		{
			public ULvec4 e0;
			public ULvec4 e1;
			public ULvec4 e2;
			public ULvec4 e3;
			public ULvec4 e4;
			public ULvec4 e5;
			public ULvec4 e6;
			public ULvec4 e7;

			public ref ULvec4 this[int index]
			{
				get
				{
					return ref AsSpan()[index];
				}
			}

			public Span<ULvec4> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 8);
		}

		public partial struct _clip_e__FixedBuffer
		{
			public ULMatrix4x4 e0;
			public ULMatrix4x4 e1;
			public ULMatrix4x4 e2;
			public ULMatrix4x4 e3;
			public ULMatrix4x4 e4;
			public ULMatrix4x4 e5;
			public ULMatrix4x4 e6;
			public ULMatrix4x4 e7;

			public ref ULMatrix4x4 this[int index]
			{
				get
				{
					return ref AsSpan()[index];
				}
			}

			public Span<ULMatrix4x4> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 8);
		}
	}

	public enum ULCommandType
	{
		kCommandType_ClearRenderBuffer,
		kCommandType_DrawGeometry,
	}

	public partial struct ULCommand
	{
		[NativeTypeName("unsigned char")]
		public byte command_type;

		public ULGPUState gpu_state;

		[NativeTypeName("unsigned int")]
		public uint geometry_id;

		[NativeTypeName("unsigned int")]
		public uint indices_count;

		[NativeTypeName("unsigned int")]
		public uint indices_offset;
	}

	public unsafe partial struct ULCommandList
	{
		[NativeTypeName("unsigned int")]
		public uint size;

		[NativeTypeName("ULCommand *")]
		public ULCommand* commands;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverBeginSynchronizeCallback();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverEndSynchronizeCallback();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: NativeTypeName("unsigned int")]
	public delegate uint ULGPUDriverNextTextureIdCallback();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULGPUDriverCreateTextureCallback([NativeTypeName("unsigned int")] uint texture_id, [NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULGPUDriverUpdateTextureCallback([NativeTypeName("unsigned int")] uint texture_id, [NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverDestroyTextureCallback([NativeTypeName("unsigned int")] uint texture_id);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: NativeTypeName("unsigned int")]
	public delegate uint ULGPUDriverNextRenderBufferIdCallback();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverCreateRenderBufferCallback([NativeTypeName("unsigned int")] uint render_buffer_id, ULRenderBuffer buffer);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverDestroyRenderBufferCallback([NativeTypeName("unsigned int")] uint render_buffer_id);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: NativeTypeName("unsigned int")]
	public delegate uint ULGPUDriverNextGeometryIdCallback();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverCreateGeometryCallback([NativeTypeName("unsigned int")] uint geometry_id, ULVertexBuffer vertices, ULIndexBuffer indices);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverUpdateGeometryCallback([NativeTypeName("unsigned int")] uint geometry_id, ULVertexBuffer vertices, ULIndexBuffer indices);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverDestroyGeometryCallback([NativeTypeName("unsigned int")] uint geometry_id);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverUpdateCommandListCallback(ULCommandList list);

	public partial struct ULGPUDriver
	{
		[NativeTypeName("ULGPUDriverBeginSynchronizeCallback")]
		public IntPtr begin_synchronize;

		[NativeTypeName("ULGPUDriverEndSynchronizeCallback")]
		public IntPtr end_synchronize;

		[NativeTypeName("ULGPUDriverNextTextureIdCallback")]
		public IntPtr next_texture_id;

		[NativeTypeName("ULGPUDriverCreateTextureCallback")]
		public IntPtr create_texture;

		[NativeTypeName("ULGPUDriverUpdateTextureCallback")]
		public IntPtr update_texture;

		[NativeTypeName("ULGPUDriverDestroyTextureCallback")]
		public IntPtr destroy_texture;

		[NativeTypeName("ULGPUDriverNextRenderBufferIdCallback")]
		public IntPtr next_render_buffer_id;

		[NativeTypeName("ULGPUDriverCreateRenderBufferCallback")]
		public IntPtr create_render_buffer;

		[NativeTypeName("ULGPUDriverDestroyRenderBufferCallback")]
		public IntPtr destroy_render_buffer;

		[NativeTypeName("ULGPUDriverNextGeometryIdCallback")]
		public IntPtr next_geometry_id;

		[NativeTypeName("ULGPUDriverCreateGeometryCallback")]
		public IntPtr create_geometry;

		[NativeTypeName("ULGPUDriverUpdateGeometryCallback")]
		public IntPtr update_geometry;

		[NativeTypeName("ULGPUDriverDestroyGeometryCallback")]
		public IntPtr destroy_geometry;

		[NativeTypeName("ULGPUDriverUpdateCommandListCallback")]
		public IntPtr update_command_list;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULClipboardClearCallback();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULClipboardReadPlainTextCallback([NativeTypeName("ULString")] C_String* result);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULClipboardWritePlainTextCallback([NativeTypeName("ULString")] C_String* text);

	public partial struct ULClipboard
	{
		[NativeTypeName("ULClipboardClearCallback")]
		public IntPtr clear;

		[NativeTypeName("ULClipboardReadPlainTextCallback")]
		public IntPtr read_plain_text;

		[NativeTypeName("ULClipboardWritePlainTextCallback")]
		public IntPtr write_plain_text;
	}

	public static unsafe partial class Methods
	{
		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("const char *")]
		public static extern sbyte* ulVersionString();

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned int")]
		public static extern uint ulVersionMajor();

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned int")]
		public static extern uint ulVersionMinor();

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned int")]
		public static extern uint ulVersionPatch();

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULConfig")]
		public static extern C_Config* ulCreateConfig();

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulDestroyConfig([NativeTypeName("ULConfig")] C_Config* config);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetResourcePath([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("ULString")] C_String* resource_path);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetCachePath([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("ULString")] C_String* cache_path);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetUseGPURenderer([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("bool")] byte use_gpu);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetDeviceScale([NativeTypeName("ULConfig")] C_Config* config, double value);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetFaceWinding([NativeTypeName("ULConfig")] C_Config* config, ULFaceWinding winding);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetEnableImages([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("bool")] byte enabled);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetEnableJavaScript([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("bool")] byte enabled);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetFontHinting([NativeTypeName("ULConfig")] C_Config* config, ULFontHinting font_hinting);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetFontGamma([NativeTypeName("ULConfig")] C_Config* config, double font_gamma);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetFontFamilyStandard([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("ULString")] C_String* font_name);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetFontFamilyFixed([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("ULString")] C_String* font_name);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetFontFamilySerif([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("ULString")] C_String* font_name);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetFontFamilySansSerif([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("ULString")] C_String* font_name);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetUserAgent([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("ULString")] C_String* agent_string);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetUserStylesheet([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("ULString")] C_String* css_string);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetForceRepaint([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("bool")] byte enabled);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetAnimationTimerDelay([NativeTypeName("ULConfig")] C_Config* config, double delay);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetScrollTimerDelay([NativeTypeName("ULConfig")] C_Config* config, double delay);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetRecycleDelay([NativeTypeName("ULConfig")] C_Config* config, double delay);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetMemoryCacheSize([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("unsigned int")] uint size);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetPageCacheSize([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("unsigned int")] uint size);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetOverrideRAMSize([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("unsigned int")] uint size);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetMinLargeHeapSize([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("unsigned int")] uint size);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulConfigSetMinSmallHeapSize([NativeTypeName("ULConfig")] C_Config* config, [NativeTypeName("unsigned int")] uint size);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULRenderer")]
		public static extern C_Renderer* ulCreateRenderer([NativeTypeName("ULConfig")] C_Config* config);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulDestroyRenderer([NativeTypeName("ULRenderer")] C_Renderer* renderer);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulUpdate([NativeTypeName("ULRenderer")] C_Renderer* renderer);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulRender([NativeTypeName("ULRenderer")] C_Renderer* renderer);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulPurgeMemory([NativeTypeName("ULRenderer")] C_Renderer* renderer);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulLogMemoryUsage([NativeTypeName("ULRenderer")] C_Renderer* renderer);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULSession")]
		public static extern C_Session* ulCreateSession([NativeTypeName("ULRenderer")] C_Renderer* renderer, [NativeTypeName("bool")] byte is_persistent, [NativeTypeName("ULString")] C_String* name);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulDestroySession([NativeTypeName("ULSession")] C_Session* session);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULSession")]
		public static extern C_Session* ulDefaultSession([NativeTypeName("ULRenderer")] C_Renderer* renderer);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulSessionIsPersistent([NativeTypeName("ULSession")] C_Session* session);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULString")]
		public static extern C_String* ulSessionGetName([NativeTypeName("ULSession")] C_Session* session);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned long long")]
		public static extern ulong ulSessionGetId([NativeTypeName("ULSession")] C_Session* session);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULString")]
		public static extern C_String* ulSessionGetDiskPath([NativeTypeName("ULSession")] C_Session* session);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULView")]
		public static extern C_View* ulCreateView([NativeTypeName("ULRenderer")] C_Renderer* renderer, [NativeTypeName("unsigned int")] uint width, [NativeTypeName("unsigned int")] uint height, [NativeTypeName("bool")] byte transparent, [NativeTypeName("ULSession")] C_Session* session, [NativeTypeName("bool")] byte force_cpu_renderer);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulDestroyView([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULString")]
		public static extern C_String* ulViewGetURL([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULString")]
		public static extern C_String* ulViewGetTitle([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned int")]
		public static extern uint ulViewGetWidth([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned int")]
		public static extern uint ulViewGetHeight([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulViewIsLoading([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern ULRenderTarget ulViewGetRenderTarget([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULSurface")]
		public static extern C_Surface* ulViewGetSurface([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewLoadHTML([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULString")] C_String* html_string);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewLoadURL([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULString")] C_String* url_string);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewResize([NativeTypeName("ULView")] C_View* view, [NativeTypeName("unsigned int")] uint width, [NativeTypeName("unsigned int")] uint height);

		//[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		//[return: NativeTypeName("JSContextRef")]
		//public static extern OpaqueJSContext* ulViewLockJSContext([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewUnlockJSContext([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULString")]
		public static extern C_String* ulViewEvaluateScript([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULString")] C_String* js_string, [NativeTypeName("ULString *")] C_String** exception);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulViewCanGoBack([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulViewCanGoForward([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewGoBack([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewGoForward([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewGoToHistoryOffset([NativeTypeName("ULView")] C_View* view, int offset);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewReload([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewStop([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewFocus([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewUnfocus([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulViewHasFocus([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulViewHasInputFocus([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewFireKeyEvent([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULKeyEvent")] C_KeyEvent* key_event);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewFireMouseEvent([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULMouseEvent")] C_MouseEvent* mouse_event);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewFireScrollEvent([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULScrollEvent")] C_ScrollEvent* scroll_event);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetChangeTitleCallback([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULChangeTitleCallback")] IntPtr callback, [NativeTypeName("void *")] void* user_data);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetChangeURLCallback([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULChangeURLCallback")] IntPtr callback, [NativeTypeName("void *")] void* user_data);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetChangeTooltipCallback([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULChangeTooltipCallback")] IntPtr callback, [NativeTypeName("void *")] void* user_data);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetChangeCursorCallback([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULChangeCursorCallback")] IntPtr callback, [NativeTypeName("void *")] void* user_data);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetAddConsoleMessageCallback([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULAddConsoleMessageCallback")] IntPtr callback, [NativeTypeName("void *")] void* user_data);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetCreateChildViewCallback([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULCreateChildViewCallback")] IntPtr callback, [NativeTypeName("void *")] void* user_data);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetBeginLoadingCallback([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULBeginLoadingCallback")] IntPtr callback, [NativeTypeName("void *")] void* user_data);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetFinishLoadingCallback([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULFinishLoadingCallback")] IntPtr callback, [NativeTypeName("void *")] void* user_data);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetFailLoadingCallback([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULFailLoadingCallback")] IntPtr callback, [NativeTypeName("void *")] void* user_data);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetWindowObjectReadyCallback([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULWindowObjectReadyCallback")] IntPtr callback, [NativeTypeName("void *")] void* user_data);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetDOMReadyCallback([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULDOMReadyCallback")] IntPtr callback, [NativeTypeName("void *")] void* user_data);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetUpdateHistoryCallback([NativeTypeName("ULView")] C_View* view, [NativeTypeName("ULUpdateHistoryCallback")] IntPtr callback, [NativeTypeName("void *")] void* user_data);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulViewSetNeedsPaint([NativeTypeName("ULView")] C_View* view, [NativeTypeName("bool")] byte needs_paint);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulViewGetNeedsPaint([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULView")]
		public static extern C_View* ulViewCreateInspectorView([NativeTypeName("ULView")] C_View* view);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULString")]
		public static extern C_String* ulCreateString([NativeTypeName("const char *")] sbyte* str);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULString")]
		public static extern C_String* ulCreateStringUTF8([NativeTypeName("const char *")] sbyte* str, [NativeTypeName("size_t")] UIntPtr len);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULString")]
		public static extern C_String* ulCreateStringUTF16([NativeTypeName("ULChar16 *")] ushort* str, [NativeTypeName("size_t")] UIntPtr len);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULString")]
		public static extern C_String* ulCreateStringFromCopy([NativeTypeName("ULString")] C_String* str);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulDestroyString([NativeTypeName("ULString")] C_String* str);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULChar16 *")]
		public static extern ushort* ulStringGetData([NativeTypeName("ULString")] C_String* str);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("size_t")]
		public static extern UIntPtr ulStringGetLength([NativeTypeName("ULString")] C_String* str);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulStringIsEmpty([NativeTypeName("ULString")] C_String* str);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulStringAssignString([NativeTypeName("ULString")] C_String* str, [NativeTypeName("ULString")] C_String* new_str);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulStringAssignCString([NativeTypeName("ULString")] C_String* str, [NativeTypeName("const char *")] sbyte* c_str);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULBitmap")]
		public static extern C_Bitmap* ulCreateEmptyBitmap();

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULBitmap")]
		public static extern C_Bitmap* ulCreateBitmap([NativeTypeName("unsigned int")] uint width, [NativeTypeName("unsigned int")] uint height, ULBitmapFormat format);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULBitmap")]
		public static extern C_Bitmap* ulCreateBitmapFromPixels([NativeTypeName("unsigned int")] uint width, [NativeTypeName("unsigned int")] uint height, ULBitmapFormat format, [NativeTypeName("unsigned int")] uint row_bytes, [NativeTypeName("const void *")] void* pixels, [NativeTypeName("size_t")] UIntPtr size, [NativeTypeName("bool")] byte should_copy);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULBitmap")]
		public static extern C_Bitmap* ulCreateBitmapFromCopy([NativeTypeName("ULBitmap")] C_Bitmap* existing_bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulDestroyBitmap([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned int")]
		public static extern uint ulBitmapGetWidth([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned int")]
		public static extern uint ulBitmapGetHeight([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern ULBitmapFormat ulBitmapGetFormat([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned int")]
		public static extern uint ulBitmapGetBpp([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned int")]
		public static extern uint ulBitmapGetRowBytes([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("size_t")]
		public static extern UIntPtr ulBitmapGetSize([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulBitmapOwnsPixels([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("void *")]
		public static extern void* ulBitmapLockPixels([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulBitmapUnlockPixels([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("void *")]
		public static extern void* ulBitmapRawPixels([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulBitmapIsEmpty([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulBitmapErase([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulBitmapWritePNG([NativeTypeName("ULBitmap")] C_Bitmap* bitmap, [NativeTypeName("const char *")] sbyte* path);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulBitmapSwapRedBlueChannels([NativeTypeName("ULBitmap")] C_Bitmap* bitmap);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULKeyEvent")]
		public static extern C_KeyEvent* ulCreateKeyEvent(ULKeyEventType type, [NativeTypeName("unsigned int")] uint modifiers, int virtual_key_code, int native_key_code, [NativeTypeName("ULString")] C_String* text, [NativeTypeName("ULString")] C_String* unmodified_text, [NativeTypeName("bool")] byte is_keypad, [NativeTypeName("bool")] byte is_auto_repeat, [NativeTypeName("bool")] byte is_system_key);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULKeyEvent")]
		public static extern C_KeyEvent* ulCreateKeyEventWindows(ULKeyEventType type, [NativeTypeName("uintptr_t")] UIntPtr wparam, [NativeTypeName("intptr_t")] IntPtr lparam, [NativeTypeName("bool")] byte is_system_key);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulDestroyKeyEvent([NativeTypeName("ULKeyEvent")] C_KeyEvent* evt);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULMouseEvent")]
		public static extern C_MouseEvent* ulCreateMouseEvent(ULMouseEventType type, int x, int y, ULMouseButton button);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulDestroyMouseEvent([NativeTypeName("ULMouseEvent")] C_MouseEvent* evt);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULScrollEvent")]
		public static extern C_ScrollEvent* ulCreateScrollEvent(ULScrollEventType type, int delta_x, int delta_y);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulDestroyScrollEvent([NativeTypeName("ULScrollEvent")] C_ScrollEvent* evt);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulRectIsEmpty(ULRect rect);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern ULRect ulRectMakeEmpty();

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("bool")]
		public static extern byte ulIntRectIsEmpty(ULIntRect rect);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern ULIntRect ulIntRectMakeEmpty();

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned int")]
		public static extern uint ulSurfaceGetWidth([NativeTypeName("ULSurface")] C_Surface* surface);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned int")]
		public static extern uint ulSurfaceGetHeight([NativeTypeName("ULSurface")] C_Surface* surface);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("unsigned int")]
		public static extern uint ulSurfaceGetRowBytes([NativeTypeName("ULSurface")] C_Surface* surface);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("size_t")]
		public static extern UIntPtr ulSurfaceGetSize([NativeTypeName("ULSurface")] C_Surface* surface);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("void *")]
		public static extern void* ulSurfaceLockPixels([NativeTypeName("ULSurface")] C_Surface* surface);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulSurfaceUnlockPixels([NativeTypeName("ULSurface")] C_Surface* surface);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulSurfaceResize([NativeTypeName("ULSurface")] C_Surface* surface, [NativeTypeName("unsigned int")] uint width, [NativeTypeName("unsigned int")] uint height);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulSurfaceSetDirtyBounds([NativeTypeName("ULSurface")] C_Surface* surface, ULIntRect bounds);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern ULIntRect ulSurfaceGetDirtyBounds([NativeTypeName("ULSurface")] C_Surface* surface);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulSurfaceClearDirtyBounds([NativeTypeName("ULSurface")] C_Surface* surface);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("void *")]
		public static extern void* ulSurfaceGetUserData([NativeTypeName("ULSurface")] C_Surface* surface);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		[return: NativeTypeName("ULBitmap")]
		public static extern C_Bitmap* ulBitmapSurfaceGetBitmap([NativeTypeName("ULBitmapSurface")] C_Surface* surface);

		//[NativeTypeName("const ULFileHandle")]
		//public const UIntPtr ULInvalidFileHandle = (UIntPtr)(-1);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern ULMatrix4x4 ulApplyProjection(ULMatrix4x4 transform, float viewport_width, float viewport_height, [NativeTypeName("bool")] byte flip_y);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulPlatformSetLogger(ULLogger logger);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulPlatformSetFileSystem(ULFileSystem file_system);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulPlatformSetSurfaceDefinition(ULSurfaceDefinition surface_definition);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulPlatformSetGPUDriver(ULGPUDriver gpu_driver);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		public static extern void ulPlatformSetClipboard(ULClipboard clipboard);
	}
}
