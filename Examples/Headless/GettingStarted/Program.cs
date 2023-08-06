using System;
using System.IO;
using System.Threading;
using UltralightNet;
using UltralightNet.AppCore;

// Set Font Loader
AppCoreMethods.SetPlatformFontLoader();

// Create Renderer
var cfg = new ULConfig();
using Renderer renderer = ULPlatform.CreateRenderer(cfg);

// Create View
using View view = renderer.CreateView(1980, 1024);

// Load URL

bool loaded = false;

view.OnFinishLoading += (_, _, _) =>
{
	loaded = true;
};

view.URL = "https://ultralig.ht";

// Update Renderer until page is loaded
while (!loaded)
{
	renderer.Update();
	// give time to process network etc.
	Thread.Sleep(10);
}

// Render
renderer.Render();

// Get Surface
ULSurface surface = view.Surface ?? throw new Exception("Surface not found, did you perhaps set ViewConfig.IsAccelerated to true?");

// Get Bitmap
ULBitmap bitmap = surface.Bitmap;

// Save bitmap to png file
var path = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
bitmap.WritePng(Path.Combine(path, "OUTPUT.png"));
