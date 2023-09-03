using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UltralightNet.GPU.Vulkan;

public class ResourceList<T>
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
}
