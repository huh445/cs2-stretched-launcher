using System;
using System.Runtime.InteropServices;

namespace CS2StretchedLauncher.Interop
{
    internal static class Native
    {
        public const int ENUM_CURRENT_SETTINGS = -1;

        public const uint CDS_UPDATEREGISTRY = 0x00000001;
        public const uint CDS_FULLSCREEN      = 0x00000004;
        public const int  DISP_CHANGE_SUCCESSFUL = 0;

        public const int DM_PELSWIDTH  = 0x00080000;
        public const int DM_PELSHEIGHT = 0x00100000;
        public const int DM_BITSPERPEL = 0x00040000;
        // NOTE: We intentionally do NOT set DM_DISPLAYFREQUENCY.

        public const int DISPLAY_DEVICE_PRIMARY_DEVICE = 0x00000004;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 32;
            private const int CCHFORMNAME   = 32;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;
            public ushort dmSpecVersion;
            public ushort dmDriverVersion;
            public ushort dmSize;
            public ushort dmDriverExtra;
            public uint   dmFields;

            public int  dmPositionX;
            public int  dmPositionY;
            public uint dmDisplayOrientation;
            public uint dmDisplayFixedOutput;

            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            public ushort dmLogPixels;
            public uint   dmBitsPerPel;
            public uint   dmPelsWidth;
            public uint   dmPelsHeight;
            public uint   dmDisplayFlags;
            public uint   dmDisplayFrequency;

            public uint dmICMMethod;
            public uint dmICMIntent;
            public uint dmMediaType;
            public uint dmDitherType;
            public uint dmReserved1;
            public uint dmReserved2;
            public uint dmPanningWidth;
            public uint dmPanningHeight;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]  public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceString;
            public int  StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceKey;
        }

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern int EnumDisplaySettings(string? lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern int ChangeDisplaySettingsEx(string? lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, uint dwflags, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern bool EnumDisplayDevices(string? lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);
    }
}
