using System;
using UltralightNet;
using UltralightNet.AppCore;

ULPlatform.FileSystem = ULPlatform.DefaultFileSystem;
using ULApp app = ULApp.Create(new(), new());
using ULWindow window = app.MainMonitor.CreateWindow(512, 512, false, ULWindowFlags.Titled | ULWindowFlags.Resizable | ULWindowFlags.Maximizable);

window.Title = "AppCore Example";

using ULOverlay overlay = window.CreateOverlay(window.Width, window.Height, 0, 0);
window.SetResizeCallback((IntPtr user_data, ULWindow window, uint width, uint height) => overlay.Resize(width, height));
window.SetCloseCallback((_, _) => app.Quit());

View view = overlay.View;
//view.URL = "https://github.com/SupinePandora43/UltralightNet";

view.OnFailLoading += (frame_id, is_main_frame, url, description, error_domain, error_code) => throw new Exception("Failed loading");

//view.HTML = "<html><body><p>123</p></body></html>";
//view.URL = "https://vk.com/supinepandora43";
//view.URL = "https://www.youtube.com/watch?v=N1v4TjntTJI";
//view.URL = "https://twitter.com/@supinepandora43";
view.URL = "https://ultralig.ht/";
//while (!l) { app.Renderer.Update(); Thread.Sleep(20); }

app.Run();
