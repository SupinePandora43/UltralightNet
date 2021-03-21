![UltralightSharp](https://gitlab.com/SupinePandora43/UltralightSharp/raw/master/icon.png)

[![NuGet](https://img.shields.io/nuget/v/Supine.UltralightSharp.svg)](https://www.nuget.org/packages/Supine.UltralightSharp/) ![Build & Test](https://github.com/SupinePandora43/UltralightSharp/workflows/Build%20&%20Test/badge.svg)
[![Coverage](https://raw.githubusercontent.com/SupinePandora43/UltralightSharp/gh-pages/badge_linecoverage.svg)](https://supinepandora43.github.io/UltralightSharp/)
# UltralightSharp

A multi-platform .NET binding of the **[Ultralight](https://utralig.ht)** project.

## Supported platforms:

|         | x86_64    | arm64    |
|---------|-----------|----------|
| windows | win-x64   | upstream |
| linux   | linux-x64 | upstream |
| osx     | osx-x64   | upstream |

### Known Issues:

* not all api covered
* * ping me in discord `SupinePandora43#3399` 

* Demo but no tests, no WebCore bindings yet.


Acknowlegedments
----------------

* [Ultralight](https://utralig.ht)
* [Ultralight on GitHub](https://github.com/ultralight-ux/Ultralight)

This project includes binary distributions of Ultralight SDK libraries.

Examples
--------

## .NET Core Headless / Console Demo

See the [DemoProgram](https://github.com/Supine/UltralightSharp/tree/master/UltralightSharp.Demo) and Safe [DemoProgram](https://github.com/Supine/UltralightSharp/tree/master/UltralightSharp.SafeDemo) for headless functional examples.

![Demo Screenshot](https://cdn.discordapp.com/attachments/738836157923852368/739599229709844520/unknown.png)

The demo can produce PNGs or a scaled down low resolution 24-bit ANSI image to the console.
(ANSI image on Windows console seen above.)

## Unity Demo (2018.4 LTS)

![Unity Demo](https://cdn.discordapp.com/attachments/738836157923852368/739376040970944572/unknown.png)
![Unity Tests](https://cdn.discordapp.com/attachments/738836157923852368/739376118435414096/unknown.png)

A Unity demo and test has been added to this repo.

The CI will test against LTS branches of Unity.

Currently only 2018.4 LTS is tested.

It is forward compatible up to at least 2020.1, but may require some tweaking of dependency versions.

## Silk.NET OpenGL ES 3.0 Demo

![Silk.NET Demo](https://cdn.discordapp.com/attachments/738836157923852368/744597691937325116/unknown.png)

![Silk.NET Demo](https://cdn.discordapp.com/attachments/738836157923852368/742182287134818304/unknown.png)

![Silk.NET Console](https://cdn.discordapp.com/attachments/738836157923852368/742182302599217306/unknown.png)

## Blazor Usage in Ultralight

[NUlliiON's UltralightBlazorExperiment](https://github.com/NUlliiON/UltralightBlazorExperiment)

![NUlliiON's UltralightBlazorExperiment](https://i.imgur.com/FXeTRYL.png)

