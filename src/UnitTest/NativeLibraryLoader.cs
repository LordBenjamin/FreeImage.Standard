using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FreeImageNETUnitTest
{
    internal class NativeLibraryLoader
    {
        public static void CopyFreeImageNativeDll()
        {
            string solutionFolder = GetSolutionFolder();
            string runtimesFolder = Path.Combine(solutionFolder, "runtimes");

            const string freeImageLibraryName = "FreeImage";

            string libraryPath = GetPlatformLibraryPath(runtimesFolder, freeImageLibraryName);
            string libraryFileExtension = Path.GetExtension(libraryPath);

            if (false == File.Exists(libraryPath))
            {
                throw new FileNotFoundException(libraryPath);
            }

            string executingFolder = GetExecutingFolder();
            string targetLibraryPath = Path.Combine(executingFolder, $"{freeImageLibraryName}{libraryFileExtension}");

            if (File.Exists(targetLibraryPath))
            {
                File.Delete(targetLibraryPath);
            }

            File.Copy(libraryPath, targetLibraryPath, false);
        }

        private static string GetPlatformLibraryPath(string runtimesFolder, string libraryName)
        {
            string runtimeFolderName;
            string libraryFileName;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                runtimeFolderName = GetWindowsRuntimeFolder();
                libraryFileName = $"{libraryName}.dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                runtimeFolderName = "osx.10.10-x64";
                libraryFileName = "libfreeimage.3.17.0.dylib";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                runtimeFolderName = "linux-x64";
                libraryFileName = "libfreeimage-3.18.0.so";
            }
            else
            {
                throw new Exception($"Unsupported platform");
            }

            return Path.Combine(runtimesFolder, runtimeFolderName, "native", libraryFileName);
        }

        private static string GetWindowsRuntimeFolder()
        {
            int ptrSize = Marshal.SizeOf<IntPtr>();
            return (ptrSize == 4) ? "win-x86" : "win-x64";
        }

        private static string GetExecutingFolder()
        {
            return Path.GetDirectoryName(typeof(NativeLibraryLoader).Assembly.Locati‌​on);
        }

        public static string GetSolutionFolder()
        {
            string currentFolder = GetExecutingFolder();

            while (Path.GetFileName(currentFolder) != "src")
            {
                currentFolder = Path.GetFullPath(Path.Combine(currentFolder, ".."));
            }

            return Path.GetFullPath(Path.Combine(currentFolder, ".."));
        }
    }
}
