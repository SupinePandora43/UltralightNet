using System;
using System.Collections.Generic;

namespace UltralightNet.GPU.Vulkan;

public class DestroyQueue : IDisposable
{
	readonly Queue<(uint frame, Action action)> toDestroy = new(64);

	public void Enqueue(uint frame, Action action) => toDestroy.Enqueue((frame, action));

	public void Execute(uint frame)
	{
		while (toDestroy.TryPeek(out (uint frame, Action action) pair) && pair.frame == frame)
		{
			pair = toDestroy.Dequeue();
			pair.action.Invoke();
		}
	}

	public void Dispose()
	{
		while (toDestroy.TryDequeue(out (uint _, Action action) pair)) pair.action.Invoke();
		GC.SuppressFinalize(this);
	}
	~DestroyQueue() => Dispose();
}
