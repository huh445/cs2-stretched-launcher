using CS2StretchedLauncher.Interop;
using CS2StretchedLauncher.Utilities;
using System;
using System.Runtime.InteropServices;

namespace CS2StretchedLauncher.Services
{
    internal sealed class ResolutionManager
    {
        private readonly uint _lowW, _lowH, _lowBpp;
        private readonly uint _highW, _highH, _highBpp;

        public ResolutionManager(uint lowW, uint lowH, uint lowBpp, uint highW, uint highH, uint highBpp)
        {
            _lowW = lowW; _lowH = lowH; _lowBpp = lowBpp;
            _highW = highW; _highH = highH; _highBpp = highBpp;
        }

        public bool ApplyLow()  => ApplyDesktopModeLikeQRes(_lowW,  _lowH,  _lowBpp);
        public bool RestoreHigh() => ApplyDesktopModeLikeQRes(_highW, _highH, _highBpp);

        private static string? GetPrimaryDisplayName()
        {
            uint i = 0;
            var dd = new Native.DISPLAY_DEVICE { cb = Marshal.SizeOf<Native.DISPLAY_DEVICE>() };
            while (Native.EnumDisplayDevices(null, i, ref dd, 0))
            {
                if ((dd.StateFlags & Native.DISPLAY_DEVICE_PRIMARY_DEVICE) != 0)
                    return dd.DeviceName; // e.g. "\\.\DISPLAY1"
                i++;
                dd = new Native.DISPLAY_DEVICE { cb = Marshal.SizeOf<Native.DISPLAY_DEVICE>() };
            }
            return null;
        }

        // Mirror QRes: target primary device, set width/height/bpp, commit to registry, don't force Hz
        private static bool ApplyDesktopModeLikeQRes(uint w, uint h, uint bpp)
        {
            var device = GetPrimaryDisplayName();
            var dm = CreateEmptyDevMode();
            dm.dmFields     = Native.DM_PELSWIDTH | Native.DM_PELSHEIGHT | Native.DM_BITSPERPEL;
            dm.dmPelsWidth  = w;
            dm.dmPelsHeight = h;
            dm.dmBitsPerPel = bpp;

            // Validate/apply fullscreen semantics first
            int r = Native.ChangeDisplaySettingsEx(device, ref dm, IntPtr.Zero, Native.CDS_FULLSCREEN, IntPtr.Zero);
            if (r != Native.DISP_CHANGE_SUCCESSFUL)
            {
                Logger.Log($"ChangeDisplaySettingsEx validate failed: {r}");
                return false;
            }

            // Commit to registry so Windows treats it as desktop mode
            r = Native.ChangeDisplaySettingsEx(device, ref dm, IntPtr.Zero, Native.CDS_UPDATEREGISTRY | Native.CDS_FULLSCREEN, IntPtr.Zero);
            if (r != Native.DISP_CHANGE_SUCCESSFUL)
            {
                Logger.Log($"ChangeDisplaySettingsEx commit failed: {r}");
                return false;
            }

            return true;
        }

        private static Native.DEVMODE CreateEmptyDevMode()
        {
            return new Native.DEVMODE
            {
                dmDeviceName = new string('\0', 32),
                dmFormName   = new string('\0', 32),
                dmSize       = (ushort)Marshal.SizeOf(typeof(Native.DEVMODE))
            };
        }
    }
}
