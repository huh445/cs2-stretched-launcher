using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    // ===== Win32 interop =====
    private const int ENUM_CURRENT_SETTINGS = -1;
    private const uint CDS_UPDATEREGISTRY = 0x00000001;
    private const uint CDS_FULLSCREEN      = 0x00000004;
    private const int DISP_CHANGE_SUCCESSFUL = 0;
    private const int DM_PELSWIDTH  = 0x00080000;
    private const int DM_PELSHEIGHT = 0x00100000;
    private const int DM_BITSPERPEL = 0x00040000;
    private const int DISPLAY_DEVICE_PRIMARY_DEVICE = 0x00000004;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private struct DEVMODE
    {
        private const int CCHDEVICENAME = 32;
        private const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;
        public ushort dmSpecVersion;
        public ushort dmDriverVersion;
        public ushort dmSize;
        public ushort dmDriverExtra;
        public uint dmFields;

        public int dmPositionX;
        public int dmPositionY;
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
        public uint dmBitsPerPel;
        public uint dmPelsWidth;
        public uint dmPelsHeight;
        public uint dmDisplayFlags;
        public uint dmDisplayFrequency;

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
    private struct DISPLAY_DEVICE
    {
        public int cb;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]  public string DeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceString;
        public int StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceKey;
    }

    [DllImport("user32.dll", CharSet = CharSet.Ansi)]
    private static extern int EnumDisplaySettings(string? lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

    [DllImport("user32.dll", CharSet = CharSet.Ansi)]
    private static extern int ChangeDisplaySettingsEx(string? lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, uint dwflags, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Ansi)]
    private static extern bool EnumDisplayDevices(string? lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

    // ===== Config (hard-coded as requested) =====
    private static readonly uint LOW_W = 1280;
    private static readonly uint LOW_H = 960;
    private static readonly uint LOW_BPP = 32;

    private static readonly uint HIGH_W = 1920;  // restore
    private static readonly uint HIGH_H = 1080;  // restore
    private static readonly uint HIGH_BPP = 32;  // restore

    static int Main()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, __) => SafeRestore();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; SafeRestore(); Environment.Exit(0); };

        // 1) Set desktop to 1280x960 (QRes-style)
        if (!ApplyDesktopModeLikeQRes(LOW_W, LOW_H, LOW_BPP))
        {
            Log("Failed to set 1280x960. Aborting.");
            return 2;
        }

        try
        {
            // 2) Launch CS2 via Steam
            try
            {
                Process.Start(new ProcessStartInfo("steam://rungameid/730") { UseShellExecute = true });
            }
            catch (Win32Exception ex)
            {
                Log("Failed to open Steam URI: " + ex.Message);
                return 3;
            }

            // 3) Wait for cs2 to appear (up to 60s), then wait while it is running
            if (WaitForProcessToAppear("cs2", TimeSpan.FromSeconds(60)))
            {
                WaitWhileProcessExists("cs2", TimeSpan.FromMilliseconds(750));
            }
            return 0;
        }
        finally
        {
            // 4) Restore desktop to 1920x1080 (QRes-style)
            ApplyDesktopModeLikeQRes(HIGH_W, HIGH_H, HIGH_BPP);
        }
    }

    // ===== Safety restore used by event handlers =====
    private static void SafeRestore()
    {
        try { ApplyDesktopModeLikeQRes(HIGH_W, HIGH_H, HIGH_BPP); }
        catch { /* swallow */ }
    }

    // ===== “Like QRes” helpers =====
    private static string? GetPrimaryDisplayName()
    {
        uint i = 0;
        var dd = new DISPLAY_DEVICE { cb = Marshal.SizeOf<DISPLAY_DEVICE>() };
        while (EnumDisplayDevices(null, i, ref dd, 0))
        {
            if ((dd.StateFlags & DISPLAY_DEVICE_PRIMARY_DEVICE) != 0)
                return dd.DeviceName; // e.g. "\\.\DISPLAY1"
            i++;
            dd = new DISPLAY_DEVICE { cb = Marshal.SizeOf<DISPLAY_DEVICE>() };
        }
        return null;
    }

    private static bool ApplyDesktopModeLikeQRes(uint w, uint h, uint bpp)
    {
        var device = GetPrimaryDisplayName();
        var dm = CreateEmptyDevMode();
        dm.dmFields = DM_PELSWIDTH | DM_PELSHEIGHT | DM_BITSPERPEL;
        dm.dmPelsWidth = w;
        dm.dmPelsHeight = h;
        dm.dmBitsPerPel = bpp;

        // Validate/apply fullscreen semantics
        int r = ChangeDisplaySettingsEx(device, ref dm, IntPtr.Zero, CDS_FULLSCREEN, IntPtr.Zero);
        if (r != DISP_CHANGE_SUCCESSFUL) return false;

        // Commit to registry so Windows treats it as the real desktop mode
        r = ChangeDisplaySettingsEx(device, ref dm, IntPtr.Zero, CDS_UPDATEREGISTRY | CDS_FULLSCREEN, IntPtr.Zero);
        return r == DISP_CHANGE_SUCCESSFUL;
    }

    private static (bool Success, DEVMODE Mode) GetCurrentMode(string? deviceName)
    {
        var dm = CreateEmptyDevMode();
        var ok = EnumDisplaySettings(deviceName, ENUM_CURRENT_SETTINGS, ref dm) != 0;
        return (ok, dm);
    }

    private static DEVMODE CreateEmptyDevMode()
    {
        return new DEVMODE
        {
            dmDeviceName = new string('\0', 32),
            dmFormName = new string('\0', 32),
            dmSize = (ushort)Marshal.SizeOf(typeof(DEVMODE))
        };
    }

    // ===== Process helpers =====
    private static bool WaitForProcessToAppear(string name, TimeSpan timeout)
    {
        var sw = Stopwatch.StartNew();
        while (sw.Elapsed < timeout)
        {
            if (Process.GetProcessesByName(name).Length > 0) return true;
            Thread.Sleep(500);
        }
        return false;
    }

    private static void WaitWhileProcessExists(string name, TimeSpan poll)
    {
        while (Process.GetProcessesByName(name).Length > 0)
            Thread.Sleep(poll);
    }

    // ===== Minimal debug logging (stripped in Release) =====
    [Conditional("DEBUG")]
    private static void Log(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
    }
}
