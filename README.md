# UltralightNet

[![NuGet](https://img.shields.io/nuget/v/UltralightNet.svg)](https://www.nuget.org/packages/UltralightNet/) [![Build & Test](https://github.com/SupinePandora43/UltralightNet/actions/workflows/build.yml/badge.svg)](https://github.com/SupinePandora43/UltralightNet/actions/workflows/build.yml)
[![codecov](https://codecov.io/gh/SupinePandora43/UltralightNet/branch/master/graph/badge.svg?token=QSP6YT7UM8)](https://codecov.io/gh/SupinePandora43/UltralightNet)

[Ultralight](https://ultralig.ht) .NET bindings

## Supported platforms

NOTE: ultralight supports only x64 processors
NOTE: At that time, ultralight doesn't support arm. (eg. Android phones or macs on M1)

- Windows
- Linux
- OSX

## UltralightSharp

UltralightNet was a fork of UltralightSharp.
But it was too complicated for me to update generated bindings.
So i decided to rewrite it from scratch.

### Differences

#### Packages

- `UltralightNet.Binaries` is `UltralightSharp.Core.LinuxX64`, `UltralightSharp.Core.OsxX64` and `UltralightSharp.Core.WinX64` at same time.
- `UltralightNet.Binaries` doesn't contain [AppCore](https://github.com/ultralight-ux/AppCore) binaries, they're in `UltralightNet.AppCore`
- `UltralightNet.Resources` contains only `resources` folder, as `UltralightSharp.Core`

#### Code

UltralightSharp uses a lot of IL injection, UltralightNet doesn't.

name conflicts like `System.String` and `ImpromptuNinjas.UltralightSharp.String`: UltralightNet just adds `UL` prefix.

Managed (`string`) versions vs Unmanaged (`ULString*`) versions: UltralightSharp has two different namespaces for that, UltralightNet just adds `_` prefix. Example: `ULFileSystem.OpenFile` and `ULFileSystem._OpenFile` (also `_ULFileSystem.OpenFile` for `[UnmanagedCallersOnlyAttribute]` scenario)

Marshaling: UltralightNet heavily relies on [DllImportGenerator](https://github.com/dotnet/runtimelab/tree/feature/DllImportGenerator) for Native interop, it lets us easily marshal values without NET's `CustomMarshaler` overhead.

## Reporting issues

* [issues page on github](https://github.com/SupinePandora43/UltralightNet/issues)
* Discord `SupinePandora43#3399`

# Getting Started

## Nuget packages

you need to install at least:

* `UltralightNet`
* `UltralightNet.Binaries`
* `UltralightNet.AppCore` because only AppCore provides font loader

to have fully functional Ultralight renderer

## How to render a static page

1. Set Logger (optional)
2. Set Font Loader (or crash)
3. Set FileSystem (used by ultralight to load "resources" folder content)
4. Create renderer (configurable, using ULConfig)
5. Create View (html page)
6. Load page: Page (raw html string) or URL (requires `UltralightNet.Resources` package, and ULConfig.ResourcePath set to `./resources`)
7. Update renderer until page is loaded
8. Render
9. Get View's Surface
10. Get Surface's Bitmap
11. Swap Red and Blue channels
12. Save bitmap to png file

## Code

```cs
using System;
using System.IO;
using System.Threading;
using UltralightNet;
using UltralightNet.AppCore;

namespace UltralightNet.GettingStarted
{
	class Program
	{
		static void Main()
		{
			// Set Logger
			ULPlatform.SetLogger(new ULLogger()
			{
				LogMessage = (level, message) =>
				{
					Console.WriteLine($"({level}): {message}");
				}
			});

			// Set Font Loader
			AppCoreMethods.ulEnablePlatformFontLoader();

			// Set filesystem (Ultralight requires "resources/icudt67l.dat", and probably cacert.pem too)
			AppCoreMethods.ulEnablePlatformFileSystem(Path.GetDirectoryName(typeof(Program).Assembly.Location));

			// Create config, used for specifying resources folder (used for URL loading)
			ULConfig config = new();

			// Create Renderer
			Renderer renderer = new(config);

			// Create View
			View view = new(renderer, 512, 512);

			// Load URL

			bool loaded = false;

			view.SetFinishLoadingCallback((user_data, caller, frame_id, is_main_frame, url) =>
			{
				loaded = true;
			});

			view.URL = "https://github.com"; // Requires "UltralightNet.Resources"

			// Update Renderer until page is loaded
			while (!loaded)
			{
				renderer.Update();
				// sleep | give ultralight time to process network etc.
				Thread.Sleep(10);
			}

			// Render
			renderer.Render();

			// Get Surface
			ULSurface surface = view.Surface;

			// Get Bitmap
			ULBitmap bitmap = surface.Bitmap;

			// Swap Red and Blue channels
			bitmap.SwapRedBlueChannels();

			// save bitmap to png file
			bitmap.WritePng("./github.png");
		}
	}
}

```

## Ready to run project

[UltralightNet.GettingStarted](https://github.com/SupinePandora43/UltralightNet/tree/master/UltralightNet.GettingStarted)

## Result

![github](https://user-images.githubusercontent.com/36124472/112279554-dc648600-8ca5-11eb-868c-f7d4441adc3b.png)

