using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet.OpenGL;

public unsafe partial class OpenGLGPUDriver
{
	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
	public static ULGPUDriver UNGL_Initialize_GPUDriver(uint sampleCount){
		return default;
	}
}
