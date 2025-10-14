using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace UltralightNet.GPUCommon;

public class ResourceList<T> : IDisposable
{
	readonly List<T> list = new(16) { default! };
	readonly Queue<int> freeIds = new(8);

	public ref T this[int id] => ref CollectionsMarshal.AsSpan(list)[id];

	public int GetNewId()
	{
		if (freeIds.TryDequeue(out var id)) return id;
		list.Add(default!);
		return list.Count - 1;
	}

	public void Remove(int id)
	{
		freeIds.Enqueue(id);
	}

	[SuppressMessage("CodeAnalysis", "CA1816")]
	public void Dispose()
	{
		Debug.Assert(list.Count - freeIds.Count is 0 or 1, "All resources must be properly disposed");

		list.Clear();
		list.TrimExcess();
		freeIds.Clear();
		freeIds.TrimExcess();
	}
}
