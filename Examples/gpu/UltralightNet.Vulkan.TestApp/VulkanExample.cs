//namespace VulkanExample;

using Silk.NET.Vulkan;
using Silk.NET.Windowing;

using Application app = new();
app.Run();

internal unsafe partial class Application : IDisposable
{
	//static Application() => Window.PrioritizeSdl();
	readonly IWindow window = Window.Create(WindowOptions.DefaultVulkan);
	readonly Vk vk = Vk.GetApi();
	readonly Instance instance;

	readonly PhysicalDevice physicalDevice;

	public Application()
	{
		{ // Window
			window.Initialize();
			if (window.VkSurface is null) throw new PlatformNotSupportedException("Vulkan surface not found.");
		}
		{ // Instance
			var extensions = window.VkSurface.GetRequiredExtensions(out uint extensionCount);
			Utils.CreateInstance(vk, extensionCount, extensions, out instance);
		}
		{ // Physical Device
			uint deviceCount = 0;
			vk.EnumeratePhysicalDevices(instance, ref deviceCount, null).Check();
			if (deviceCount is 0) throw new Exception("Couldn't find physical vulkan device.");
			var devices = stackalloc PhysicalDevice[(int)deviceCount];
			vk.EnumeratePhysicalDevices(instance, ref deviceCount, devices).Check();
			physicalDevice = devices[0]; // idc
		}
		Console.WriteLine();
	}



	public void Run()
	{

	}

	void IDisposable.Dispose()
	{
		vk.DestroyInstance(instance, null);
		window.Dispose();
	}
}
