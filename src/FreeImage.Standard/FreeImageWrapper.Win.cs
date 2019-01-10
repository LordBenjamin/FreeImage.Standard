#if NET472 // TODO: This is Windows specific (P/Invoke), not a .NET FX / Core issue
using System;
using Gdi = FreeImageAPI.NativeMethods.Gdi;
using Win32 = FreeImageAPI.NativeMethods.Win32;

namespace FreeImageAPI
{
    public static partial class FreeImage
    {
        #region Pixel access functions

        /// <summary>
        /// Retrieves an hBitmap for a FreeImage bitmap.
        /// Call FreeHbitmap(IntPtr) to free the handle.
        /// </summary>
        /// <param name="dib">Handle to a FreeImage bitmap.</param>
        /// <param name="hdc">A reference device context.
        /// Use IntPtr.Zero if no reference is available.</param>
        /// <param name="unload">When true dib will be unloaded if the function succeeded.</param>
        /// <returns>The hBitmap for the FreeImage bitmap.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dib"/> is null.</exception>
        public static unsafe IntPtr GetHbitmap(FIBITMAP dib, IntPtr hdc, bool unload)
        {
            if (dib.IsNull)
            {
                throw new ArgumentNullException("dib");
            }

            IntPtr hBitmap = IntPtr.Zero;
            bool release = false;
            IntPtr ppvBits = IntPtr.Zero;
            // Check if we have destination
            if (release = (hdc == IntPtr.Zero))
            {
                // We don't so request dc
                hdc = Gdi.GetDC(IntPtr.Zero);
            }

            if (hdc != IntPtr.Zero)
            {
                // Get pointer to the infoheader of the bitmap
                IntPtr info = GetInfo(dib);
                // Create a bitmap in the dc
                hBitmap = Gdi.CreateDIBSection(hdc, info, DIB_RGB_COLORS, out ppvBits, IntPtr.Zero, 0);
                if (hBitmap != IntPtr.Zero && ppvBits != IntPtr.Zero)
                {
                    // Copy the data into the dc
                    CopyMemory(ppvBits, GetBits(dib), (GetHeight(dib) * GetPitch(dib)));
                    // Success: we unload the bitmap
                    if (unload)
                    {
                        Unload(dib);
                    }
                }

                // We have to release the dc
                if (release)
                {
                    Gdi.ReleaseDC(IntPtr.Zero, hdc);
                }
            }

            return hBitmap;
        }

        /// <summary>
        /// Returns an HBITMAP created by the <c>CreateDIBitmap()</c> function which in turn
        /// has always the same color depth as the reference DC, which may be provided
        /// through <paramref name="hdc"/>. The desktop DC will be used,
        /// if <c>IntPtr.Zero</c> DC is specified.
        /// Call <see cref="FreeImage.FreeHbitmap(IntPtr)"/> to free the handle.
        /// </summary>
        /// <param name="dib">Handle to a FreeImage bitmap.</param>
        /// <param name="hdc">Handle to a device context.</param>
        /// <param name="unload">When true the structure will be unloaded on success.
        /// If the function failed and returned false, the bitmap was not unloaded.</param>
        /// <returns>If the function succeeds, the return value is a handle to the
        /// compatible bitmap. If the function fails, the return value is <see cref="IntPtr.Zero"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dib"/> is null.</exception>
        public static IntPtr GetBitmapForDevice(FIBITMAP dib, IntPtr hdc, bool unload)
        {
            if (dib.IsNull)
            {
                throw new ArgumentNullException("dib");
            }

            IntPtr hbitmap = IntPtr.Zero;
            bool release = false;
            if (release = (hdc == IntPtr.Zero))
            {
                hdc = Gdi.GetDC(IntPtr.Zero);
            }

            if (hdc != IntPtr.Zero)
            {
                hbitmap = Gdi.CreateDIBitmap(
                    hdc,
                    GetInfoHeader(dib),
                    CBM_INIT,
                    GetBits(dib),
                    GetInfo(dib),
                    DIB_RGB_COLORS);
                if (unload)
                {
                    Unload(dib);
                }

                if (release)
                {
                    Gdi.ReleaseDC(IntPtr.Zero, hdc);
                }
            }

            return hbitmap;
        }

        /// <summary>
        /// Creates a FreeImage DIB from a Device Context/Compatible Bitmap.
        /// </summary>
        /// <param name="hbitmap">Handle to the bitmap.</param>
        /// <param name="hdc">Handle to a device context.</param>
        /// <returns>Handle to a FreeImage bitmap.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="hbitmap"/> is null.</exception>
        public unsafe static FIBITMAP CreateFromHbitmap(IntPtr hbitmap, IntPtr hdc)
        {
            if (hbitmap == IntPtr.Zero)
            {
                throw new ArgumentNullException("hbitmap");
            }

            FIBITMAP dib = new FIBITMAP();
            BITMAP bm;
            uint colors;
            bool release;

            if (Gdi.GetObject(hbitmap, sizeof(BITMAP), (IntPtr)(&bm)) != 0)
            {
                dib = Allocate(bm.bmWidth, bm.bmHeight, bm.bmBitsPixel, 0, 0, 0);
                if (!dib.IsNull)
                {
                    colors = GetColorsUsed(dib);
                    if (release = (hdc == IntPtr.Zero))
                    {
                        hdc = Gdi.GetDC(IntPtr.Zero);
                    }

                    if (Gdi.GetDIBits(
                        hdc,
                        hbitmap,
                        0,
                        (uint)bm.bmHeight,
                        GetBits(dib),
                        GetInfo(dib),
                        DIB_RGB_COLORS) != 0)
                    {
                        if (colors != 0)
                        {
                            BITMAPINFOHEADER* bmih = (BITMAPINFOHEADER*)GetInfo(dib);
                            bmih[0].biClrImportant = bmih[0].biClrUsed = colors;
                        }
                    }
                    else
                    {
                        UnloadEx(ref dib);
                    }

                    if (release)
                    {
                        Gdi.ReleaseDC(IntPtr.Zero, hdc);
                    }
                }
            }

            return dib;
        }

        /// <summary>
        /// Frees a bitmap handle.
        /// </summary>
        /// <param name="hbitmap">Handle to a bitmap.</param>
        /// <returns>True on success, false on failure.</returns>
        public static bool FreeHbitmap(IntPtr hbitmap)
        {
            return Gdi.DeleteObject(hbitmap);
        }

        #endregion

        #region Memory

        /// <summary>
        /// Moves a block of memory from one location to another.
        /// </summary>
        /// <param name="dst">A pointer to the starting address of the move destination.</param>
        /// <param name="src">A pointer to the starting address of the block of memory to be moved.</param>
        /// <param name="size">The size of the block of memory to move, in bytes.</param>
        public static unsafe void MoveMemory(void* dst, void* src, long size)
        {
            if (IsWindows)
            {
                Win32.MoveMemory(dst, src, checked((uint)size));
            }
            else
            {
                throw new NotImplementedException("This method is only implemented on Windows.");
            }
        }

        /// <summary>
        /// Moves a block of memory from one location to another.
        /// </summary>
        /// <param name="dst">A pointer to the starting address of the move destination.</param>
        /// <param name="src">A pointer to the starting address of the block of memory to be moved.</param>
        /// <param name="size">The size of the block of memory to move, in bytes.</param>
        public static unsafe void MoveMemory(IntPtr dst, IntPtr src, uint size)
        {
            if (IsWindows)
            {
                Win32.MoveMemory(dst.ToPointer(), src.ToPointer(), size);
            }
            else
            {
                throw new NotImplementedException("This method is only implemented on Windows.");
            }
        }

        /// <summary>
        /// Moves a block of memory from one location to another.
        /// </summary>
        /// <param name="dst">A pointer to the starting address of the move destination.</param>
        /// <param name="src">A pointer to the starting address of the block of memory to be moved.</param>
        /// <param name="size">The size of the block of memory to move, in bytes.</param>
        public static unsafe void MoveMemory(IntPtr dst, IntPtr src, long size)
        {
            if (IsWindows)
            {
                Win32.MoveMemory(dst.ToPointer(), src.ToPointer(), checked((uint)size));
            }
            else
            {
                throw new NotImplementedException("This method is only implemented on Windows.");
            }
        }

        #endregion
    }
}
#endif