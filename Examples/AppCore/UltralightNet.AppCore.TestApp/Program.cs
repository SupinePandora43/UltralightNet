using System;
using UltralightNet;
using UltralightNet.AppCore;

ULPlatform.FileSystem = ULPlatform.DefaultFileSystem;
using var app = ULApp.Create(new(), new());
using var window = app.MainMonitor.CreateWindow(512, 512);

window.Title = "AppCore Example";

using var overlay = window.CreateOverlay(window.ScreenWidth, window.ScreenHeight);
window.OnResize += (uint width, uint height) => overlay.Resize(width, height);
window.OnClose += () => app.Quit();

var view = overlay.View;
//view.URL = "https://github.com/SupinePandora43/UltralightNet";

view.OnFailLoading += (frame_id, is_main_frame, url, description, error_domain, error_code) => throw new Exception("Failed loading");

//view.HTML = "<html><body><p>123</p></body></html>";
//view.URL = "https://vk.com/supinepandora43";
//view.URL = "https://www.youtube.com/watch?v=N1v4TjntTJI";
//view.URL = "https://twitter.com/@supinepandora43";
view.URL = "https://ultralig.ht/";
//while (!l) { app.Renderer.Update(); Thread.Sleep(20); }

app.Run();
