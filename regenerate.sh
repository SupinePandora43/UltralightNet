#!/bin/bash

dotnet build -c Release -f net7.0 src/UltralightNet
dotnet build -c Release -f net7.0 src/UltralightNet.AppCore
cp ./src/UltralightNet/obj/Release/net7.0/generated/Microsoft.Interop.LibraryImportGenerator/Microsoft.Interop.LibraryImportGenerator/LibraryImports.g.cs ./src/UltralightNet/LibraryImportGenerator/LibraryImports.g.cs
mkdir ./src/UltralightNet.AppCore/LibraryImportGenerator
cp ./src/UltralightNet.AppCore/obj/Release/net7.0/generated/Microsoft.Interop.LibraryImportGenerator/Microsoft.Interop.LibraryImportGenerator/LibraryImports.g.cs ./src/UltralightNet.AppCore/LibraryImportGenerator/LibraryImports.g.cs
