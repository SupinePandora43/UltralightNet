namespace UltralightNet.GPU.Vulkan;

public enum Bufferization
{
	/// <summary>
	/// Do not bufferize - may lead to image tearing
	/// </summary>
	/// <remarks>Fastest one</remarks>
	None = 0,
	/// <summary>
	/// Bufferize and copy content of previous modified frame on demand - exactly what an API expects
	/// </summary>
	/// <remarks>Slowest one</remarks>
	FrameWithCopy,
	/// <summary>
	/// Bufferize, but don't copy content of previous modified frame - requires <see cref="ULConfig.ForceRepaint" />
	/// </summary>
	/// <remarks>Fast as <see cref="None"/>, but requires <see cref="ULConfig.ForceRepaint"/></remarks>
	FrameWithoutCopy
}
