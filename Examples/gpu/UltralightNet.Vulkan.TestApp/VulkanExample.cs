//namespace VulkanExample;

using System.Diagnostics;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;

using Application app = new();
app.Run();

internal unsafe partial class Application : IDisposable
{
	readonly Stopwatch stopwatch = Stopwatch.StartNew();

	readonly IWindow window = Window.Create(WindowOptions.DefaultVulkan);
	readonly Vk vk = Vk.GetApi();
	readonly Instance instance;

	readonly KhrSurface khrSurface;
	readonly ExtDebugUtils? debugUtils;

	readonly SurfaceKHR surface;
	readonly PhysicalDevice physicalDevice;

	readonly uint graphicsQueueFamily = uint.MaxValue;
	readonly uint presentQueueFamily = uint.MaxValue;

	readonly Device device;
	readonly Queue graphicsQueue;
	readonly Queue presentQueue;

	public Application()
	{
		{ // Window
			window.Initialize();
			if (window.VkSurface is null) throw new PlatformNotSupportedException("Vulkan surface not found.");
		}
		{ // Instance
			var extensions = SilkMarshal.PtrToStringArray((nint)window.VkSurface.GetRequiredExtensions(out uint surfaceExtensionCount), (int)surfaceExtensionCount).ToList();
			extensions.Add(KhrSurface.ExtensionName);
			if (vk.IsInstanceExtensionPresent(ExtDebugUtils.ExtensionName)) extensions.Add(ExtDebugUtils.ExtensionName);

			Utils.CreateInstance(vk, extensions.ToArray(), out instance);
		}
		{ // Instance extensions
			if (!vk.TryGetInstanceExtension(instance, out khrSurface)) throw new Exception($"{KhrSurface.ExtensionName} extension not found.");
			surface = window.VkSurface.Create<AllocationCallbacks>(instance.ToHandle(), null).ToSurface();

			vk.TryGetInstanceExtension(instance, out debugUtils);
		}
		{ // Physical Device
			uint deviceCount = 0;
			vk.EnumeratePhysicalDevices(instance, ref deviceCount, null).Check();
			if (deviceCount is 0) throw new Exception("Couldn't find physical vulkan device.");
			var devices = stackalloc PhysicalDevice[(int)deviceCount];
			vk.EnumeratePhysicalDevices(instance, ref deviceCount, devices).Check();
			physicalDevice = devices[0]; // idc
			if (!vk.IsDeviceExtensionPresent(physicalDevice, KhrSwapchain.ExtensionName)) throw new Exception($"Physical device doesn't support ${KhrSwapchain.ExtensionName}");

			uint queueFamilityCount = 0;
			vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, ref queueFamilityCount, null);
			var queueFamilyProperties = stackalloc QueueFamilyProperties[(int)queueFamilityCount];
			vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, ref queueFamilityCount, queueFamilyProperties);

			for (uint i = 0; i < queueFamilityCount; i++)
			{
				if (graphicsQueueFamily is uint.MaxValue && queueFamilyProperties[i].QueueFlags.HasFlag(QueueFlags.QueueGraphicsBit)) graphicsQueueFamily = i;
				if (presentQueueFamily is uint.MaxValue)
				{
					khrSurface.GetPhysicalDeviceSurfaceSupport(physicalDevice, i, surface, out Bool32 supported).Check();
					if ((bool)supported is true) presentQueueFamily = i;
				}
				if ((graphicsQueueFamily | presentQueueFamily) is not uint.MaxValue) break;
			}
			if ((graphicsQueueFamily | presentQueueFamily) is uint.MaxValue) throw new Exception("Suitable queue families not found.");
		}
		{ // Device + Queues
			float priority = 1.0f;
			var queueCreateInfos = stackalloc DeviceQueueCreateInfo[2];
			queueCreateInfos[0] = new(queueFamilyIndex: graphicsQueueFamily, queueCount: 1, pQueuePriorities: &priority);
			queueCreateInfos[1] = new(queueFamilyIndex: presentQueueFamily, queueCount: 1, pQueuePriorities: &priority);

			PhysicalDeviceFeatures physicalDeviceFeatures = new() { SamplerAnisotropy = true };

			var extensions = new string[] { KhrSwapchain.ExtensionName };
			byte** extensionsPtr = (byte**)SilkMarshal.StringArrayToPtr(extensions);

			try
			{
				DeviceCreateInfo deviceCreateInfo = new(
					queueCreateInfoCount: graphicsQueueFamily == presentQueueFamily ? 1u : 2u, pQueueCreateInfos: queueCreateInfos,
					enabledExtensionCount: (uint)extensions.Length, ppEnabledExtensionNames: extensionsPtr,
					pEnabledFeatures: &physicalDeviceFeatures);

				vk.CreateDevice(physicalDevice, &deviceCreateInfo, null, out device).Check();

				vk.GetDeviceQueue(device, graphicsQueueFamily, 0, out graphicsQueue);
				if (graphicsQueueFamily == presentQueueFamily) presentQueue = graphicsQueue;
				else vk.GetDeviceQueue(device, presentQueueFamily, 0, out presentQueue);
			}
			finally
			{
				for (uint i = 0; i < extensions.Length; i++) SilkMarshal.FreeString((nint)extensionsPtr[i]);
				SilkMarshal.Free((nint)extensionsPtr);
			}
		}

		Console.WriteLine($"Initialized Application in {stopwatch.Elapsed}");
	}



	public void Run()
	{

	}

	void IDisposable.Dispose()
	{
		vk.DestroyDevice(device, null);
		khrSurface.DestroySurface(instance, surface, null);
		vk.DestroyInstance(instance, null);
		vk.Dispose();
		window.Dispose();
	}
}
