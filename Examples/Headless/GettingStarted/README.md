# Getting Started

## Nuget packages

you need to install at least:

* `UltralightNet`
* `UltralightNet.Binaries`
* `UltralightNet.AppCore` (because only AppCore provides font loader)

to have fully functional Ultralight renderer

## How to render a static page

1. Set Font Loader (or crash)
2. Set File System
3. Create `Renderer`
4. Create `View`
5. Load page
6. Update renderer until page is loaded
7. Render
8. Get View's Surface
9.  Get Surface's Bitmap
10. Swap Red and Blue channels
11. Save to png file

## Code / Ready to run project

[GettingStarted](https://github.com/SupinePandora43/UltralightNet/tree/master/UltralightNet.GettingStarted)

## PNG Result

![ultralight render result](./ultralight.png)

