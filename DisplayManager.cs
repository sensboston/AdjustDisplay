using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using static AdjustDisplay.NativeMethods;

namespace AdjustDisplay
{
    public class DisplayManager
    {
        private List<Display> _displays = new List<Display>();
        public List<Display> Displays => _displays;
        private int _currentDisplayIndex { get; set; }
        public Display CurrentDisplay => _displays[_currentDisplayIndex];
        public DisplayMode CurrentDisplayMode => _displays[_currentDisplayIndex].CurrentMode;

        /// <summary>
        /// Class constructor: collects info about all connected displays
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public DisplayManager() 
        {
            var display = new DISPLAY_DEVICE();
            display.cb = Marshal.SizeOf(display);
            uint idx = 0;

            // Enum all displays
            while (EnumDisplayDevices(null, idx++, ref display, 0))
            {
                if ((display.StateFlags & DisplayDeviceStateFlags.AttachedToDesktop) > 0)
                    _displays.Add(new Display(display));
            }

            // Get scale information
            QDC flags = QDC.QDC_ONLY_ACTIVE_PATHS;
            var retCode = GetDisplayConfigBufferSizes(flags, out int numPaths, out int numModes);
            if (retCode != WIN32STATUS.ERROR_SUCCESS) throw new Win32Exception((int)retCode);

            DISPLAYCONFIG_PATH_INFO[] displayPaths = new DISPLAYCONFIG_PATH_INFO[numPaths];
            DISPLAYCONFIG_MODE_INFO[] displayModes = new DISPLAYCONFIG_MODE_INFO[numModes];

            retCode = QueryDisplayConfig(flags, ref numPaths, displayPaths, ref numModes, displayModes, IntPtr.Zero);
            if (retCode != WIN32STATUS.ERROR_SUCCESS) throw new Win32Exception((int)retCode);

            int i = 0;
            foreach (var path in displayPaths)
            {
                if (i < _displays.Count)
                {
                    if (_displays.Count < displayPaths.Length) _displays[i].IsDuplicated = true;

                    _displays[i].DisplayPathInfo = displayPaths[i];
                    _displays[i].DisplayModeInfos = new List<DISPLAYCONFIG_MODE_INFO>();
                    _displays[i].DisplayModeInfos.AddRange(displayModes);

                    var adapterLUID = path.TargetInfo.AdapterId;
                    var targetID = path.TargetInfo.Id;
                    var sourceID = path.SourceInfo.Id;
                    var targetDeviceName = new DISPLAYCONFIG_TARGET_DEVICE_NAME();
                    targetDeviceName.Header.Size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_TARGET_DEVICE_NAME));
                    targetDeviceName.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
                    targetDeviceName.Header.AdapterId = adapterLUID;
                    targetDeviceName.Header.Id = targetID;
                    retCode = DisplayConfigGetDeviceInfo(ref targetDeviceName);
                    if (retCode != WIN32STATUS.ERROR_SUCCESS) throw new Win32Exception((int)retCode);
                    _displays[i].TargetDeviceName = targetDeviceName;
                    _displays[i].ScaleInfo = GetMonitorDpiScale(displayPaths[i].SourceInfo.AdapterId, displayPaths[i].SourceInfo.Id);
                }
                
                i++;
            }

            // Get current display
            _currentDisplayIndex = _displays.FindIndex(d => (d.StateFlags & DisplayDeviceStateFlags.PrimaryDevice) != 0);
        }

        /// <summary>
        /// Sets specific video mode for display
        /// </summary>
        /// <param name="dispIndex">display #</param>
        /// <param name="modeIndex">mode #</param>
        /// <exception cref="Exception"></exception>
        public void SetVideoMode(int dispIndex, int modeIndex)
        {
            if (dispIndex < 1 || dispIndex > Displays.Count) throw new Exception("Invalid display number");
            var display = _displays[dispIndex - 1];
            if (modeIndex < 1 || modeIndex > display.Modes.Count) throw new Exception("Invalid video mode");
            var devMode = display.Modes[modeIndex - 1].DevMode;
            devMode.dmFields = DM_BITSPERPEL | DM_PELSWIDTH | DM_PELSHEIGHT | DM_DISPLAYFREQUENCY | DM_POSITION | DM_DISPLAYFLAGS;
            CheckResult((DisplayChangeResult)ChangeDisplaySettingsEx(
                display.DeviceName, ref devMode, IntPtr.Zero, CDS_UPDATEREGISTRY | CDS_RESET, IntPtr.Zero));
        }

        /// <summary>
        /// Finds and sets video mode by provided parameters
        /// </summary>
        /// <param name="dispIndex"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="orient"></param>
        /// <param name="bpp"></param>
        /// <param name="freq"></param>
        /// <param name="scale"></param>
        /// <exception cref="Exception"></exception>
        public void SetVideoMode(int dispIndex, int width, int height, int orient, int bpp, int freq, int scale)
        {
            if (dispIndex < 1 || dispIndex > Displays.Count) throw new Exception("Invalid display number");
            var display = _displays[dispIndex - 1];
            // Do we need to change display mode?
            if (width > 0 || height > 0 || orient > -1 || bpp > 0 || freq > 0)
            {
                var modes = new List<DisplayMode>(display.Modes);
                if (width > 0) modes = modes.Where(m => m.Width == width).ToList();
                if (modes == null || modes.Count == 0) throw new Exception($"Can't find video mode with width={width}");

                if (height > 0) modes = modes.Where(m => m.Height == height).ToList();
                if (modes == null || modes.Count == 0) throw new Exception($"Can't find video mode with height={height}");

                if (bpp > 0) modes = modes.Where(m => m.BitsPerPel == bpp).ToList();
                if (modes == null || modes.Count == 0) throw new Exception($"Can't find video mode with bits per pel={bpp}");

                if (freq > 0) modes = modes.Where(m => m.Frequency == freq).ToList();
                if (modes == null || modes.Count == 0) throw new Exception($"Can't find video mode with frequency={freq}");

                if (orient > -1 && !(orient == 0 || orient == 90 || orient == 180 || orient == 270))
                    throw new Exception("Invalid orientation");

                // Select "higher" mode
                var mode = modes.LastOrDefault();
                if (mode == null) throw new Exception("No available video modes found");

                if (width > 0) mode.Width = (uint)width;
                if (height > 0) mode.Height = (uint)height;
                if (bpp > 0) mode.BitsPerPel = (uint)bpp;
                if (freq > 0) mode.Frequency = (uint)freq;

                switch (orient)
                {
                    case 90: mode.Orientation = Orientation.Rotate90; break;
                    case 180: mode.Orientation = Orientation.Rotate180; break;
                    case 270: mode.Orientation = Orientation.Rotate270; break;
                    default: mode.Orientation = Orientation.Default; break;
                }

                var devMode = mode.DevMode;
                devMode.dmFields = DM_BITSPERPEL | DM_PELSWIDTH | DM_PELSHEIGHT | DM_DISPLAYFREQUENCY | DM_POSITION | DM_DISPLAYFLAGS;               
                CheckResult((DisplayChangeResult)ChangeDisplaySettingsEx(
                    display.DeviceName, ref devMode, IntPtr.Zero, CDS_UPDATEREGISTRY | CDS_RESET, IntPtr.Zero));
            }

            // Process display scale change
            if (scale > 0) SetMonitorDpiScale(dispIndex, scale);
        }

        private DPIScalingInfo GetMonitorDpiScale(LUID adapterId, uint sourceId)
        {
            DISPLAYCONFIG_SOURCE_DPI_SCALE_GET getScalePacket = new DISPLAYCONFIG_SOURCE_DPI_SCALE_GET();
            getScalePacket.Header.Size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_SOURCE_DPI_SCALE_GET));
            getScalePacket.Header.AdapterId = adapterId;
            getScalePacket.Header.Id = sourceId;
            getScalePacket.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_DPI_SCALE;

            var retCode = DisplayConfigGetDeviceInfo(ref getScalePacket);
            if (retCode != WIN32STATUS.ERROR_SUCCESS) throw new Win32Exception((int)retCode);

            var scalingInfo = new DPIScalingInfo();
            var minAbs = Math.Abs(getScalePacket.MinimumScale);
            if (DefaultDpiValues.Length >= (minAbs + getScalePacket.MaximumScale+1))
            {
                scalingInfo.Current = DefaultDpiValues[minAbs + getScalePacket.CurrentScale];
                scalingInfo.Recommended = DefaultDpiValues[minAbs];
                scalingInfo.Mininum = DefaultDpiValues[minAbs + getScalePacket.MinimumScale];
                scalingInfo.Maximum = DefaultDpiValues[minAbs + getScalePacket.MaximumScale];
            }
            return scalingInfo;
        }

        private void SetMonitorDpiScale(int dispIndex, int newScale)
        {
            var display = Displays[dispIndex - 1];
            var adapterId = display.DisplayPathInfo.SourceInfo.AdapterId;
            var sourceId = display.DisplayPathInfo.SourceInfo.Id;

            if (newScale < display.ScaleInfo.Mininum || newScale > display.ScaleInfo.Maximum)
                throw new Exception($"Invalid scale, available range from {display.ScaleInfo.Mininum}% to {display.ScaleInfo.Maximum}%");

            int idx1 = -1, idx2 = -1, i = 0;
            foreach (var val in DefaultDpiValues)
            {
                if (val == newScale) idx1 = i;
                if (val == Displays[dispIndex - 1].ScaleInfo.Recommended) idx2 = i;
                i++;
            }
            if (idx1 == -1 || idx2 == -1)
                throw new Exception($"Invalid scale, available range from {display.ScaleInfo.Mininum}% to {display.ScaleInfo.Maximum}%");
            int newScaleIndex = idx1 - idx2;

            DISPLAYCONFIG_SOURCE_DPI_SCALE_SET setScalePacket = new DISPLAYCONFIG_SOURCE_DPI_SCALE_SET();
            setScalePacket.Header.Size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_SOURCE_DPI_SCALE_SET));
            setScalePacket.Header.AdapterId = adapterId;
            setScalePacket.Header.Id = sourceId;
            setScalePacket.Header.Type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_SET_DPI_SCALE;
            setScalePacket.ScaleRel = (uint)newScaleIndex;

            var retCode = DisplayConfigSetDeviceInfo(ref setScalePacket);
            if (retCode != WIN32STATUS.ERROR_SUCCESS) throw new Win32Exception((int)retCode);
        }

        private void CheckResult(DisplayChangeResult result)
        {
            var msg = string.Empty;
            switch (result)
            {
                case DisplayChangeResult.BadDualView:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadDualView;
                    break;
                case DisplayChangeResult.BadParam:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadParam;
                    break;
                case DisplayChangeResult.BadFlags:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadFlags;
                    break;
                case DisplayChangeResult.NotUpdated:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_NotUpdated;
                    break;
                case DisplayChangeResult.BadMode:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_BadMode;
                    break;
                case DisplayChangeResult.Failed:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_Failed;
                    break;
                case DisplayChangeResult.Restart:
                    msg = Properties.Resources.InvalidOperation_Disp_Change_Restart;
                    break;
            }
            if (!string.IsNullOrEmpty(msg)) throw new InvalidOperationException(msg);
        }
    }

    public class DPIScalingInfo
    {
        public int Mininum { get; set; }
        public int Maximum { get; set; }
        public int Current { get; set; }
        public int Recommended { get; set; }
    };

    public class Display
    {
        public string DeviceID { get; private set; }
        public string DeviceKey { get; private set; }
        public string DeviceName { get; private set; }
        public string DeviceString { get; private set; }
        public DisplayDeviceStateFlags StateFlags { get; private set; } 

        public List<DisplayMode> Modes = new List<DisplayMode>();

        private int _currentModeIndex = 0;
        public DisplayMode CurrentMode => Modes[_currentModeIndex];

        public bool IsDuplicated { get; set; }
        internal DPIScalingInfo ScaleInfo { get;  set; }
        internal DISPLAYCONFIG_TARGET_DEVICE_NAME TargetDeviceName { get; set; }

        internal DISPLAYCONFIG_PATH_INFO DisplayPathInfo;
        internal List<DISPLAYCONFIG_MODE_INFO> DisplayModeInfos;

        public Display(DISPLAY_DEVICE display)
        {
            DeviceID = display.DeviceID;
            DeviceKey = display.DeviceKey;
            DeviceName = display.DeviceName;
            DeviceString = display.DeviceString;
            StateFlags = display.StateFlags;
            IsDuplicated = false;
            EnumVideoModes();
            _currentModeIndex = GetCurrentModeIndex();
        }

        /// <summary>
        /// Enums all video modes for display
        /// </summary>
        public void EnumVideoModes()
        {
            Modes.Clear();
            var mode = new DEVMODE();
            mode.Initialize();
            int modeIdx = 0;
            while (EnumDisplaySettings(DeviceName, modeIdx++, ref mode))
            {
                var m = new DisplayMode(mode);
                // Add only modes with different resolutions, frequencies and bpps
                if (Modes.FirstOrDefault(dm =>
                        mode.dmPelsWidth == dm.Width &&
                        mode.dmPelsHeight == dm.Height &&
                        mode.dmDisplayFrequency == dm.Frequency &&
                        mode.dmBitsPerPel == dm.BitsPerPel) == null)
                {
                    Modes.Add(m);
                }
            }
        }

        public DEVMODE GetCurrentMode()
        {
            // Get current mode
            var currentMode = new DEVMODE();
            currentMode.Initialize();

            if (!EnumDisplaySettings(DeviceName, ENUM_CURRENT_SETTINGS, ref currentMode))
                throw new InvalidOperationException(LastError);

            return currentMode;
        }

        private int GetCurrentModeIndex()
        {
            var currentMode = GetCurrentMode();
            var cm = Modes.FirstOrDefault(m =>
                currentMode.dmPelsWidth == m.Width &&
                currentMode.dmPelsHeight == m.Height &&
                currentMode.dmDisplayFrequency == m.Frequency &&
                currentMode.dmBitsPerPel == m.BitsPerPel);

            if (cm != null) return Modes.IndexOf(cm);
            else return 0;
        }

        public override string ToString() 
        {
            var friendlyName = TargetDeviceName.MonitorFriendlyDeviceName;
            if (string.IsNullOrEmpty(friendlyName)) friendlyName = "Default";
            return $"Adapter: {DeviceString}\tDisplay: {friendlyName}\tScale: {ScaleInfo?.Current}"; 
        }
    }

    public class DisplayMode
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public Orientation Orientation { get; set; }
        public uint BitsPerPel { get; set; }
        public uint Frequency { get; set; }

        private DEVMODE devMode;
        public DEVMODE DevMode
        {
            get
            {
                devMode.dmPelsWidth = Width;
                devMode.dmPelsHeight = Height;
                devMode.dmDisplayOrientation = (uint) Orientation;
                devMode.dmBitsPerPel = BitsPerPel;
                devMode.dmDisplayFrequency = Frequency;
                return devMode;
            }
        }
        public DisplayMode(DEVMODE mode)
        {
            Width = mode.dmPelsWidth;
            Height = mode.dmPelsHeight;
            Orientation = (Orientation) mode.dmDisplayOrientation;
            BitsPerPel = mode.dmBitsPerPel;
            Frequency = mode.dmDisplayFrequency;
            devMode = mode;
        }
        public override string ToString() { return $"{Width} by {Height}, {Orientation} orientation, {BitsPerPel} bpp, {Frequency} Hz"; }
    }

    public enum Orientation
    {
        Default = 0,
        Rotate90 = 1,
        Rotate180 = 2,
        Rotate270 = 3
    }

    public enum DisplayChangeResult
    {
        /// <summary>
        /// Windows XP: The settings change was unsuccessful because system is DualView capable.
        /// </summary>
        BadDualView = -6,
        /// <summary>
        /// An invalid parameter was passed in. This can include an invalid flag or combination of flags.
        /// </summary>
        BadParam = -5,
        /// <summary>
        /// An invalid set of flags was passed in.
        /// </summary>
        BadFlags = -4,
        /// <summary>
        /// Windows NT/2000/XP: Unable to write settings to the registry.
        /// </summary>
        NotUpdated = -3,
        /// <summary>
        /// The graphics mode is not supported.
        /// </summary>
        BadMode = -2,
        /// <summary>
        /// The display driver failed the specified graphics mode.
        /// </summary>
        Failed = -1,
        /// <summary>
        /// The settings change was successful.
        /// </summary>
        Successful = 0,
        /// <summary>
        /// The computer must be restarted in order for the graphics mode to work.
        /// </summary>
        Restart = 1
    }
}
