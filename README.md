# FreeImage.Standard

This a .NET Standard 2.0 wrapper for the FreeImage library.

Currently API-compatible with the FreeImage .NET Wrapper (http://freeimage.sourceforge.net/), however this may diverge over time as the original wrapper is no longer maintained.

## Installing
Nuget package: https://www.nuget.org/packages/FreeImage.Standard

`Install-Package FreeImage.Standard`

![](https://img.shields.io/nuget/dt/FreeImage.Standard.svg)

## Native Compatibility

FreeImage native binaries are included in the nuget package for Windows x86/x64 and Linux x64/armhf.

For other platforms they will have to be installed separately. Note that the native function calls require the library filename to be `FreeImage`, so symlinking may be required (eg. `sudo ln -s /usr/lib/x86_64-linux-gnu/libfreeimage.so /usr/lib/FreeImage`).

## FreeImage Version

This is for FreeImage version 3.18.0

Note that FreeImage 3.18.0 is not available on Raspian yet - please use the v4.3.7 package which is linked against FreeImage 3.17.0 for now.

**The version number of this package no longer matches the FreeImage native library version!**

## Dependencies

Windows build requires [Microsoft Visual C++ Runtime 2015](https://www.microsoft.com/en-gb/download/details.aspx?id=48145).

## License

The license is the same as the FreeImage license available at the link above
