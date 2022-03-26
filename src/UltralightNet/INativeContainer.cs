using System;

namespace UltralightNet;

public unsafe interface INativeContainer<TSelf> : IDisposable
{
	public abstract void* Handle { get; init; }
	public abstract bool IsDisposed { get; }

	public abstract TSelf FromPointer(void* ptr);
}