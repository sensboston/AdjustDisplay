using System;
using System.Runtime.InteropServices;

namespace AdjustDisplay
{
    public static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            // You can define the following constant but OUTSIDE the structure because
            // that size and layout of the structure is very important
            // CCHDEVICENAME = 32 = 0x50
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;

            // After the 32-bytes array
            [MarshalAs(UnmanagedType.U2)]
            public ushort dmSpecVersion;

            [MarshalAs(UnmanagedType.U2)]
            public ushort dmDriverVersion;

            [MarshalAs(UnmanagedType.U2)]
            public ushort dmSize;

            [MarshalAs(UnmanagedType.U2)]
            public ushort dmDriverExtra;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmFields;

            public POINTL dmPosition;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmDisplayOrientation;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmDisplayFixedOutput;

            [MarshalAs(UnmanagedType.I2)]
            public short dmColor;

            [MarshalAs(UnmanagedType.I2)]
            public short dmDuplex;

            [MarshalAs(UnmanagedType.I2)]
            public short dmYResolution;

            [MarshalAs(UnmanagedType.I2)]
            public short dmTTOption;

            [MarshalAs(UnmanagedType.I2)]
            public short dmCollate;

            // CCHDEVICENAME = 32 = 0x50
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            [MarshalAs(UnmanagedType.U2)]
            public ushort dmLogPixels;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmBitsPerPel;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmPelsWidth;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmPelsHeight;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmDisplayFlags;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmDisplayFrequency;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmICMMethod;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmICMIntent;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmMediaType;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmDitherType;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmReserved1;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmReserved2;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmPanningWidth;

            [MarshalAs(UnmanagedType.U4)]
            public uint dmPanningHeight;

            /// <summary>
            /// Initializes the structure variables.
            /// </summary>
            public void Initialize()
            {
                dmDeviceName = new string(new char[32]);
                dmFormName = new string(new char[32]);
                dmSize = (ushort)Marshal.SizeOf(this);
            }
        }

        // 8-bytes structure
        [StructLayout(LayoutKind.Sequential)]
        public struct POINTL
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumDisplaySettings(
            string lpszDeviceName,  // display device
            [param: MarshalAs(UnmanagedType.U4)]
            int iModeNum,         // graphics mode
            [In, Out]
            ref DEVMODE lpDevMode // graphics mode settings
            );

        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x10,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            [MarshalAs(UnmanagedType.U4)]
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumDisplayDevices(
            [In, Out] 
            string lpDevice, 
            uint iDevNum, 
            ref DISPLAY_DEVICE lpDisplayDevice, 
            uint dwFlags);

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int DMDO_DEFAULT = 0;
        public const int DMDO_90 = 1;
        public const int DMDO_180 = 2;
        public const int DMDO_270 = 3;

        public const int CDS_UPDATEREGISTRY = 0x01;
        public const int CDS_TEST = 0x02;
        public const int CDS_FULLSCREEN = 0x04;
        public const int CDS_GLOBAL = 0x08;
        public const int CDS_SET_PRIMARY = 0x10;
        public const int CDS_RESET = 0x40000000;
        public const int CDS_NORESET = 0x10000000;

        public const int DM_BITSPERPEL = 0x00040000;
        public const int DM_PELSWIDTH = 0x00080000;
        public const int DM_PELSHEIGHT = 0x00100000;
        public const int DM_DISPLAYFLAGS = 0x00200000;
        public const int DM_DISPLAYFREQUENCY = 0x00400000;
        public const int DM_POSITION = 0x00000020;

        [DllImport("user32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ChangeDisplaySettings(
            [In, Out]
            ref DEVMODE lpDevMode,
            [param: MarshalAs(UnmanagedType.U4)]
            uint dwflags);

        [DllImport("user32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ChangeDisplaySettingsEx(
            string lpszDeviceName, 
            ref DEVMODE lpDevMode, 
            IntPtr hwnd,
            uint dwflags, 
            IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern uint FormatMessage(
            [param: MarshalAs(UnmanagedType.U4)]
            uint dwFlags,
            [param: MarshalAs(UnmanagedType.U4)]
            uint lpSource,
            [param: MarshalAs(UnmanagedType.U4)]
            uint dwMessageId,
            [param: MarshalAs(UnmanagedType.U4)]
            uint dwLanguageId,
            [param: MarshalAs(UnmanagedType.LPTStr)]
            out string lpBuffer,
            [param: MarshalAs(UnmanagedType.U4)]
            uint nSize,
            [param: MarshalAs(UnmanagedType.U4)]
            uint Arguments);

        public const uint FORMAT_MESSAGE_FROM_HMODULE = 0x800;

        public const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
        public const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;
        public const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        public const uint FORMAT_MESSAGE_FLAGS = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_SYSTEM;

        public static readonly int[] DefaultDpiValues = { 100, 125, 150, 175, 200, 225, 250, 300, 350, 400, 450, 500, 550, 600 };

        [DllImport("user32")]
        public static extern WIN32STATUS GetDisplayConfigBufferSizes(QDC flags,
            out int numPathArrayElements, out int numModeInfoArrayElements);

        // QueryDisplayConfig
        [DllImport("user32")]
        public static extern WIN32STATUS QueryDisplayConfig(QDC flags,
            ref int numPathArrayElements, [Out] DISPLAYCONFIG_PATH_INFO[] pathArray,
            ref int numModeInfoArrayElements, [Out] DISPLAYCONFIG_MODE_INFO[] modeInfoArray,
                IntPtr currentTopologyId);

        [DllImport("user32.dll")]
        public static extern WIN32STATUS DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME requestPacket);

        [DllImport("user32.dll")]
        public static extern WIN32STATUS DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_SOURCE_DPI_SCALE_GET requestPacket);

        [DllImport("user32.dll")]
        public static extern WIN32STATUS DisplayConfigSetDeviceInfo(ref DISPLAYCONFIG_SOURCE_DPI_SCALE_SET requestPacket);

        #region CDC data structires

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_SOURCE_DPI_SCALE_GET
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER Header;
            public int MinimumScale;
            public int CurrentScale;
            public int MaximumScale;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID : IEquatable<LUID>
        {
            public uint LowPart;
            public uint HighPart;

            public ulong Value => ((ulong)HighPart << 32) | LowPart;

            public override bool Equals(object obj) => obj is LUID other && this.Equals(other);
            public bool Equals(LUID other) => LowPart == other.LowPart && HighPart == other.HighPart;

            public override int GetHashCode()
            {
                return (LowPart, HighPart).GetHashCode();
            }

            public static bool operator ==(LUID lhs, LUID rhs) => lhs.Equals(rhs);

            public static bool operator !=(LUID lhs, LUID rhs) => !(lhs == rhs);

            public override string ToString() => Value.ToString();
        }

        public enum DISPLAYCONFIG_DEVICE_INFO_TYPE : int
        {
            // MS Private API (which seems to use negative numbers)
            // Set current dpi scaling value for a display
            DISPLAYCONFIG_DEVICE_INFO_SET_DPI_SCALE = -4,
            // Returns min, max, suggested, and currently applied DPI scaling values.
            DISPLAYCONFIG_DEVICE_INFO_GET_DPI_SCALE = -3,       

            // MS Public API
            Zero = 0,
            // Specifies the source name of the display device. If the DisplayConfigGetDeviceInfo
            // function is successful, DisplayConfigGetDeviceInfo returns the source name in the DISPLAYCONFIG_SOURCE_DEVICE_NAME structure.
            DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
            // Specifies information about the monitor. If the DisplayConfigGetDeviceInfo function
            // is successful, DisplayConfigGetDeviceInfo returns info about the monitor in the DISPLAYCONFIG_TARGET_DEVICE_NAME structure.
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
            // Specifies information about the preferred mode of a monitor. If the DisplayConfigGetDeviceInfo
            // function is successful, DisplayConfigGetDeviceInfo returns info about the preferred mode of a monitor in the
            // DISPLAYCONFIG_TARGET_PREFERRED_MODE structure.
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
            // Specifies the graphics adapter name. If the DisplayConfigGetDeviceInfo function is successful,
            // DisplayConfigGetDeviceInfo returns the adapter name in the DISPLAYCONFIG_ADAPTER_NAME structure.
            DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
            // Specifies how to set the monitor. If the DisplayConfigSetDeviceInfo function is successful,
            // DisplayConfigSetDeviceInfo uses info in the DISPLAYCONFIG_SET_TARGET_PERSISTENCE structure to force the output in a boot-persistent manner.
            DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
            // Specifies how to set the base output technology for a given target ID. If the DisplayConfigGetDeviceInfo function is successful,
            // DisplayConfigGetDeviceInfo returns base output technology info in the DISPLAYCONFIG_TARGET_BASE_TYPE structure.
            // Supported by WDDM 1.3 and later user-mode display drivers running on Windows 8.1 and later.
            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
            // Specifies the state of virtual mode support. If the DisplayConfigGetDeviceInfo function is
            // successful, DisplayConfigGetDeviceInfo returns virtual mode support information in the DISPLAYCONFIG_SUPPORT_VIRTUAL_RESOLUTION structure. Supported starting in Windows 10.
            DISPLAYCONFIG_DEVICE_INFO_GET_SUPPORT_VIRTUAL_RESOLUTION = 7,
            // Specifies how to set the state of virtual mode support. If the DisplayConfigSetDeviceInfo
            // function is successful, DisplayConfigSetDeviceInfo uses info in the DISPLAYCONFIG_SUPPORT_VIRTUAL_RESOLUTION structure to change the state of virtual mode support. Supported starting in Windows 10.
            DISPLAYCONFIG_DEVICE_INFO_SET_SUPPORT_VIRTUAL_RESOLUTION = 8,
            // Specifies information about the state of the HDR Color for a display
            DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO = 9,
            // Enables or disables the HDR Color for a display
            DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE = 10,
            // Specifies the current SDR white level for an HDR monitor. If the DisplayConfigGetDeviceInfo
            // function is successful, DisplayConfigGetDeviceInfo return SDR white level info in the
            // DISPLAYCONFIG_SDR_WHITE_LEVEL structure.
            // Supported starting in Windows 10 Fall Creators Update (Version 1709).
            DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL = 11, 
            DISPLAYCONFIG_DEVICE_INFO_GET_MONITOR_SPECIALIZATION = 12,
            DISPLAYCONFIG_DEVICE_INFO_SET_MONITOR_SPECIALIZATION = 13,
            //DISPLAYCONFIG_DEVICE_INFO_FORCE_UINT32 = 0xFFFFFFFF
        }

        public struct DISPLAYCONFIG_SOURCE_DPI_SCALE_SET
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER Header;
            public uint ScaleRel;
        };

        public enum WIN32STATUS : uint
        {
            ERROR_SUCCESS = 0,
            ERROR_ACCESS_DENIED = 5,
            ERROR_NOT_SUPPORTED = 50,
            ERROR_GEN_FAILURE = 31,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INSUFFICIENT_BUFFER = 122,
            ERROR_BAD_CONFIGURATION = 1610,
        }

        [Flags]
        public enum QDC : uint
        {
            Zero = 0x0,
            // Get all paths
            QDC_ALL_PATHS = 0x00000001,
            // Get only the active paths currently in use
            QDC_ONLY_ACTIVE_PATHS = 0x00000002,
            // Get the currently active paths as stored in the display database
            QDC_DATABASE_CURRENT = 0x00000004,
            // This flag should be bitwise OR'ed with other flags to indicate that the caller is aware of
            // virtual mode support.  Supported starting in Windows 10.
            QDC_VIRTUAL_MODE_AWARE = 0x00000010,
            // This flag should be bitwise OR'ed with QDC_ONLY_ACTIVE_PATHS to indicate that the caller
            // would like to include head-mounted displays (HMDs) in the list of active paths. See Remarks
            // for more information. 
            // Supported starting in Windows 10 1703 Creators Update.
            QDC_INCLUDE_HMD = 0x00000020,
            // This flag should be bitwise OR'ed with other flags to indicate that the caller is aware of
            // virtual refresh rate support.
            // Supported starting in Windows 11.
            QDC_VIRTUAL_REFRESH_RATE_AWARE = 0x00000040,
        }

        [Flags]
        public enum DISPLAYCONFIG_TOPOLOGY_ID : uint
        {
            Zero = 0x0,
            DISPLAYCONFIG_TOPOLOGY_INTERNAL = 0x00000001,
            DISPLAYCONFIG_TOPOLOGY_CLONE = 0x00000002,
            DISPLAYCONFIG_TOPOLOGY_EXTEND = 0x00000004,
            DISPLAYCONFIG_TOPOLOGY_EXTERNAL = 0x00000008,
            DISPLAYCONFIG_TOPOLOGY_FORCEUINT32 = 0xFFFFFFFF,
        }

        [Flags]
        public enum DISPLAYCONFIG_PATH_FLAGS : uint
        {
            Zero = 0x0,
            DISPLAYCONFIG_PATH_ACTIVE = 0x00000001,
            DISPLAYCONFIG_PATH_PREFERRED_UNSCALED = 0x00000004,
            DISPLAYCONFIG_PATH_SUPPORT_VIRTUAL_MODE = 0x00000008,
        }

        [Flags]
        public enum DISPLAYCONFIG_SOURCE_FLAGS : uint
        {
            Zero = 0x0,
            DISPLAYCONFIG_SOURCE_IN_USE = 0x00000001,
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct DISPLAYCONFIG_PATH_SOURCE_INFO : IEquatable<DISPLAYCONFIG_PATH_SOURCE_INFO>
        {
            [FieldOffset(0)]
            public LUID AdapterId;
            [FieldOffset(8)]
            public uint Id;
            [FieldOffset(12)]
            public uint ModeInfoIdx;
            [FieldOffset(12)]
            public ushort cloneGroupId;
            [FieldOffset(14)]
            public ushort sourceModeInfoIdx;
            [FieldOffset(16)]
            public DISPLAYCONFIG_SOURCE_FLAGS StatusFlags;

            public override bool Equals(object obj) => obj is DISPLAYCONFIG_PATH_SOURCE_INFO other && this.Equals(other);
            public bool Equals(DISPLAYCONFIG_PATH_SOURCE_INFO other)
                => // AdapterId.Equals(other.AdapterId) && // Removed the AdapterId from the Equals, as it changes after a reboot.
                   //Id == other.Id &&  // Removed the ID from the list as the Display ID it maps to will change after a switch from surround to non-surround profile
                    ModeInfoIdx == other.ModeInfoIdx &&
                    StatusFlags.Equals(other.StatusFlags);

            public override int GetHashCode()
            {
                //return (AdapterId, Id, ModeInfoIdx, StatusFlags).GetHashCode();
                return (ModeInfoIdx, Id, StatusFlags).GetHashCode();
            }

            public static bool operator ==(DISPLAYCONFIG_PATH_SOURCE_INFO lhs, DISPLAYCONFIG_PATH_SOURCE_INFO rhs) => lhs.Equals(rhs);
            public static bool operator !=(DISPLAYCONFIG_PATH_SOURCE_INFO lhs, DISPLAYCONFIG_PATH_SOURCE_INFO rhs) => !(lhs == rhs);
        }

        [Flags]
        public enum DISPLAYCONFIG_SCALING : UInt32
        {
            Zero = 0,
            DISPLAYCONFIG_SCALING_IDENTITY = 1,
            DISPLAYCONFIG_SCALING_CENTERED = 2,
            DISPLAYCONFIG_SCALING_STRETCHED = 3,
            DISPLAYCONFIG_SCALING_ASPECTRATIOCENTEREDMAX = 4,
            DISPLAYCONFIG_SCALING_CUSTOM = 5,
            DISPLAYCONFIG_SCALING_PREFERRED = 128,
            DISPLAYCONFIG_SCALING_FORCEUINT32 = 0xFFFFFFFF,
        }

        [Flags]
        public enum DISPLAYCONFIG_ROTATION : UInt32
        {
            Zero = 0,
            DISPLAYCONFIG_ROTATION_IDENTITY = 1,
            DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
            DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
            DISPLAYCONFIG_ROTATION_ROTATE270 = 4,
            DISPLAYCONFIG_ROTATION_FORCEUINT32 = 0xFFFFFFFF,
        }

        [Flags]
        public enum DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY : UInt32
        {
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_OTHER = 4294967295, // - 1
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HD15 = 0,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SVIDEO = 1,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPOSITE_VIDEO = 2,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_COMPONENT_VIDEO = 3,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DVI = 4,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_HDMI = 5,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_LVDS = 6,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_D_JPN = 8,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDI = 9,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EXTERNAL = 10,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_DISPLAYPORT_EMBEDDED = 11,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EXTERNAL = 12,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_UDI_EMBEDDED = 13,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_SDTVDONGLE = 14,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_MIRACAST = 15,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INDIRECT_WIRED = 16,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_INDIRECT_VIRTUAL = 17,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_public = 0x80000000,
            DISPLAYCONFIG_OUTPUT_TECHNOLOGY_FORCEUINT32 = 0xFFFFFFFF,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_RATIONAL : IEquatable<DISPLAYCONFIG_RATIONAL>
        {
            public uint Numerator;
            public uint Denominator;
            public override bool Equals(object obj) => obj is DISPLAYCONFIG_RATIONAL other && this.Equals(other);
            public bool Equals(DISPLAYCONFIG_RATIONAL other) => Numerator == other.Numerator && Denominator == other.Denominator;
            public override int GetHashCode() { return (Numerator, Denominator).GetHashCode(); }
            public static bool operator ==(DISPLAYCONFIG_RATIONAL lhs, DISPLAYCONFIG_RATIONAL rhs) => lhs.Equals(rhs);
            public static bool operator !=(DISPLAYCONFIG_RATIONAL lhs, DISPLAYCONFIG_RATIONAL rhs) => !(lhs == rhs);
            public override string ToString() => Numerator + " / " + Denominator;
        }

        [Flags]
        public enum DISPLAYCONFIG_SCANLINE_ORDERING : UInt32
        {
            DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,
            DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
            DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,
            DISPLAYCONFIG_SCANLINE_ORDERING_FORCEUINT32 = 0xFFFFFFFF,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_PATH_TARGET_INFO : IEquatable<DISPLAYCONFIG_PATH_TARGET_INFO>
        {
            public LUID AdapterId;
            public uint Id;
            public uint ModeInfoIdx;
            public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY OutputTechnology;
            public DISPLAYCONFIG_ROTATION Rotation;
            public DISPLAYCONFIG_SCALING Scaling;
            public DISPLAYCONFIG_RATIONAL RefreshRate;
            public DISPLAYCONFIG_SCANLINE_ORDERING ScanLineOrdering;
            public bool TargetAvailable;
            public uint StatusFlags;

            public bool TargetInUse => (StatusFlags & 0x1) == 0x1;
            public bool TargetForcible => (StatusFlags & 0x2) == 0x2;
            public bool ForcedAvailabilityBoot => (StatusFlags & 0x4) == 0x4;
            public bool ForcedAvailabilityPath => (StatusFlags & 0x8) == 0x8;
            public bool ForcedAvailabilitySystem => (StatusFlags & 0x10) == 0x10;
            public bool IsHMD => (StatusFlags & 0x20) == 0x20;


            /* DISPLAYCONFIG_TARGET_IN_USE = 0x00000001,
            DISPLAYCONFIG_TARGET_FORCIBLE = 0x00000002,
            DISPLAYCONFIG_TARGET_FORCED_AVAILABILITY_BOOT = 0x00000004,
            DISPLAYCONFIG_TARGET_FORCED_AVAILABILITY_PATH = 0x00000008,
            DISPLAYCONFIG_TARGET_FORCED_AVAILABILITY_SYSTEM = 0x00000010,
            DISPLAYCONFIG_TARGET_IS_HMD = 0x00000020,*/
            public override bool Equals(object obj) => obj is DISPLAYCONFIG_PATH_TARGET_INFO other && this.Equals(other);

            public bool Equals(DISPLAYCONFIG_PATH_TARGET_INFO other)
                => // AdapterId.Equals(other.AdapterId) && // Removed the AdapterId from the Equals, as it changes after reboot.
                   // Id == other.Id && // Removed as ID changes after reboot when the display is a cloned copy :(
                    ModeInfoIdx == other.ModeInfoIdx &&
                    OutputTechnology.Equals(other.OutputTechnology) &&
                    Rotation.Equals(other.Rotation) &&
                    Scaling.Equals(other.Scaling) &&
                    RefreshRate.Equals(other.RefreshRate) &&
                    ScanLineOrdering.Equals(other.ScanLineOrdering) &&
                    TargetAvailable == other.TargetAvailable &&
                    StatusFlags.Equals(StatusFlags);

            public override int GetHashCode()
            {
                return (AdapterId, Id, ModeInfoIdx, OutputTechnology, Rotation, Scaling, RefreshRate, ScanLineOrdering, TargetAvailable, StatusFlags).GetHashCode();
            }

            public static bool operator ==(DISPLAYCONFIG_PATH_TARGET_INFO lhs, DISPLAYCONFIG_PATH_TARGET_INFO rhs) => lhs.Equals(rhs);
            public static bool operator !=(DISPLAYCONFIG_PATH_TARGET_INFO lhs, DISPLAYCONFIG_PATH_TARGET_INFO rhs) => !(lhs == rhs);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_PATH_INFO : IEquatable<DISPLAYCONFIG_PATH_INFO>
        {
            public DISPLAYCONFIG_PATH_SOURCE_INFO SourceInfo;
            public DISPLAYCONFIG_PATH_TARGET_INFO TargetInfo;
            public DISPLAYCONFIG_PATH_FLAGS Flags;

            public override bool Equals(object obj) => obj is DISPLAYCONFIG_PATH_INFO other && this.Equals(other);
            public bool Equals(DISPLAYCONFIG_PATH_INFO other)
                => SourceInfo.Equals(other.SourceInfo) &&
                   TargetInfo.Equals(other.TargetInfo) &&
                   Flags.Equals(other.Flags);

            public override int GetHashCode()
            {
                return (SourceInfo, TargetInfo, Flags).GetHashCode();
            }
            public static bool operator ==(DISPLAYCONFIG_PATH_INFO lhs, DISPLAYCONFIG_PATH_INFO rhs) => lhs.Equals(rhs);
            public static bool operator !=(DISPLAYCONFIG_PATH_INFO lhs, DISPLAYCONFIG_PATH_INFO rhs) => !(lhs == rhs);
        }

        [Flags]
        public enum DISPLAYCONFIG_MODE_INFO_TYPE : UInt32
        {
            Zero = 0x0,
            DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
            DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
            DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE = 3,
            DISPLAYCONFIG_MODE_INFO_TYPE_FORCEUINT32 = 0xFFFFFFFF,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_2DREGION : IEquatable<DISPLAYCONFIG_2DREGION>
        {
            public uint Cx;
            public uint Cy;

            public override bool Equals(object obj) => obj is DISPLAYCONFIG_2DREGION other && this.Equals(other);
            public bool Equals(DISPLAYCONFIG_2DREGION other) => Cx == other.Cx && Cy == other.Cy;
            public override int GetHashCode() { return (Cx, Cy).GetHashCode(); }
            public static bool operator ==(DISPLAYCONFIG_2DREGION lhs, DISPLAYCONFIG_2DREGION rhs) => lhs.Equals(rhs);
            public static bool operator !=(DISPLAYCONFIG_2DREGION lhs, DISPLAYCONFIG_2DREGION rhs) => !(lhs == rhs);
        }

        [Flags]
        public enum D3D_VIDEO_SIGNAL_STANDARD : uint
        {
            Uninitialized = 0,
            VesaDmt = 1,
            VesaGtf = 2,
            VesaCvt = 3,
            Ibm = 4,
            Apple = 5,
            NtscM = 6,
            NtscJ = 7,
            Ntsc443 = 8,
            PalB = 9,
            PalB1 = 10,
            PalG = 11,
            PalH = 12,
            PalI = 13,
            PalD = 14,
            PalN = 15,
            PalNc = 16,
            SecamB = 17,
            SecNVIDIA = 18,
            SecamG = 19,
            SecamH = 20,
            SecamK = 21,
            SecamK1 = 22,
            SecamL = 23,
            SecamL1 = 24,
            Eia861 = 25,
            Eia861A = 26,
            Eia861B = 27,
            PalK = 28,
            PalK1 = 29,
            PalL = 30,
            PalM = 31,
            Other = 255
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_VIDEO_SIGNAL_INFO : IEquatable<DISPLAYCONFIG_VIDEO_SIGNAL_INFO>
        {
            public ulong PixelRate;
            public DISPLAYCONFIG_RATIONAL HSyncFreq;
            public DISPLAYCONFIG_RATIONAL VSyncFreq;
            public DISPLAYCONFIG_2DREGION ActiveSize;
            public DISPLAYCONFIG_2DREGION TotalSize;
            public D3D_VIDEO_SIGNAL_STANDARD VideoStandard;
            public DISPLAYCONFIG_SCANLINE_ORDERING ScanLineOrdering;

            public override bool Equals(object obj) => obj is DISPLAYCONFIG_VIDEO_SIGNAL_INFO other && this.Equals(other);
            public bool Equals(DISPLAYCONFIG_VIDEO_SIGNAL_INFO other)
                => PixelRate == other.PixelRate &&
                    HSyncFreq.Equals(other.HSyncFreq) &&
                    VSyncFreq.Equals(other.VSyncFreq) &&
                    ActiveSize.Equals(other.ActiveSize) &&
                    TotalSize.Equals(other.TotalSize) &&
                    VideoStandard == other.VideoStandard &&
                    ScanLineOrdering.Equals(other.ScanLineOrdering);

            public override int GetHashCode()
            {
                return (PixelRate, HSyncFreq, VSyncFreq, ActiveSize, TotalSize, VideoStandard, ScanLineOrdering).GetHashCode();
            }
            public static bool operator ==(DISPLAYCONFIG_VIDEO_SIGNAL_INFO lhs, DISPLAYCONFIG_VIDEO_SIGNAL_INFO rhs) => lhs.Equals(rhs);
            public static bool operator !=(DISPLAYCONFIG_VIDEO_SIGNAL_INFO lhs, DISPLAYCONFIG_VIDEO_SIGNAL_INFO rhs) => !(lhs == rhs);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_TARGET_MODE : IEquatable<DISPLAYCONFIG_TARGET_MODE>
        {
            public DISPLAYCONFIG_VIDEO_SIGNAL_INFO TargetVideoSignalInfo;
            public override bool Equals(object obj) => obj is DISPLAYCONFIG_TARGET_MODE other && this.Equals(other);
            public bool Equals(DISPLAYCONFIG_TARGET_MODE other) => TargetVideoSignalInfo.Equals(other.TargetVideoSignalInfo);
            public override int GetHashCode() { return (TargetVideoSignalInfo).GetHashCode(); }
            public static bool operator ==(DISPLAYCONFIG_TARGET_MODE lhs, DISPLAYCONFIG_TARGET_MODE rhs) => lhs.Equals(rhs);
            public static bool operator !=(DISPLAYCONFIG_TARGET_MODE lhs, DISPLAYCONFIG_TARGET_MODE rhs) => !(lhs == rhs);
        }

        [Flags]
        public enum DISPLAYCONFIG_PIXELFORMAT : UInt32
        {
            Zero = 0x0,
            DISPLAYCONFIG_PIXELFORMAT_8BPP = 1,
            DISPLAYCONFIG_PIXELFORMAT_16BPP = 2,
            DISPLAYCONFIG_PIXELFORMAT_24BPP = 3,
            DISPLAYCONFIG_PIXELFORMAT_32BPP = 4,
            DISPLAYCONFIG_PIXELFORMAT_NONGDI = 5,
            DISPLAYCONFIG_PIXELFORMAT_FORCEUINT32 = 0xFFFFFFFF,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_SOURCE_MODE : IEquatable<DISPLAYCONFIG_SOURCE_MODE>
        {
            public uint Width;
            public uint Height;
            public DISPLAYCONFIG_PIXELFORMAT PixelFormat;
            public POINTL Position;
            public override bool Equals(object obj) => obj is DISPLAYCONFIG_SOURCE_MODE other && this.Equals(other);
            public bool Equals(DISPLAYCONFIG_SOURCE_MODE other)
                => Width == other.Width &&
                    Height == other.Height &&
                    PixelFormat.Equals(other.PixelFormat) &&
                    Position.Equals(other.Position);
            public override int GetHashCode() { return (Width, Height, PixelFormat, Position).GetHashCode(); }
            public static bool operator ==(DISPLAYCONFIG_SOURCE_MODE lhs, DISPLAYCONFIG_SOURCE_MODE rhs) => lhs.Equals(rhs);
            public static bool operator !=(DISPLAYCONFIG_SOURCE_MODE lhs, DISPLAYCONFIG_SOURCE_MODE rhs) => !(lhs == rhs);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECTL : IEquatable<RECTL>
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECTL(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public static RECTL FromXYWH(int x, int y, int width, int height)
            {
                return new RECTL(x, y, x + width, y + height);
            }

            public override bool Equals(object obj) => obj is RECTL other && this.Equals(other);
            public bool Equals(RECTL other)
                => Left == other.Left &&
                   Top == other.Top &&
                   Right == other.Right &&
                   Bottom == other.Bottom;

            public override int GetHashCode() { return (Left, Top, Right, Bottom).GetHashCode(); }
            public static bool operator ==(RECTL lhs, RECTL rhs) => lhs.Equals(rhs);
            public static bool operator !=(RECTL lhs, RECTL rhs) => !(lhs == rhs);
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_DESKTOP_IMAGE_INFO : IEquatable<DISPLAYCONFIG_DESKTOP_IMAGE_INFO>
        {
            public POINTL PathSourceSize;
            public RECTL DesktopImageRegion;
            public RECTL DesktopImageClip;
            public override bool Equals(object obj) => obj is DISPLAYCONFIG_DESKTOP_IMAGE_INFO other && this.Equals(other);
            public bool Equals(DISPLAYCONFIG_DESKTOP_IMAGE_INFO other)
                => PathSourceSize.Equals(other.PathSourceSize) &&
                   DesktopImageRegion.Equals(other.DesktopImageRegion) &&
                   DesktopImageClip.Equals(other.DesktopImageClip);
            public override int GetHashCode()
            {
                return (PathSourceSize, DesktopImageRegion, DesktopImageClip).GetHashCode();
            }
            public static bool operator ==(DISPLAYCONFIG_DESKTOP_IMAGE_INFO lhs, DISPLAYCONFIG_DESKTOP_IMAGE_INFO rhs) => lhs.Equals(rhs);
            public static bool operator !=(DISPLAYCONFIG_DESKTOP_IMAGE_INFO lhs, DISPLAYCONFIG_DESKTOP_IMAGE_INFO rhs) => !(lhs == rhs);
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct DISPLAYCONFIG_MODE_INFO : IEquatable<DISPLAYCONFIG_MODE_INFO>
        {
            [FieldOffset((0))]
            public DISPLAYCONFIG_MODE_INFO_TYPE InfoType;

            [FieldOffset(4)]
            public uint Id;

            [FieldOffset(8)]
            public LUID AdapterId;

            // These 3 fields are all a C union in wingdi.dll
            [FieldOffset(16)]
            public DISPLAYCONFIG_TARGET_MODE TargetMode;

            [FieldOffset(16)]
            public DISPLAYCONFIG_SOURCE_MODE SourceMode;

            [FieldOffset(16)]
            public DISPLAYCONFIG_DESKTOP_IMAGE_INFO DesktopImageInfo;

            public override bool Equals(object obj) => obj is DISPLAYCONFIG_MODE_INFO other && this.Equals(other);
            public bool Equals(DISPLAYCONFIG_MODE_INFO other)
            {
                if (InfoType != other.InfoType)
                    return false;

                // This happens when it is a target mode info block
                if (InfoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET &&
                    Id == other.Id && // Disabling this check as as the Display ID it maps to will change after a switch from clone to non-clone profile, ruining the equality match
                    TargetMode.Equals(other.TargetMode))
                    return true;

                // This happens when it is a source mode info block
                if (InfoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE &&
                    //Id == other.Id && // Disabling this check as as the Display ID it maps to will change after a switch from surround to non-surround profile, ruining the equality match
                    // Only seems to be a problem with the DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE options weirdly enough!
                    SourceMode.Equals(other.SourceMode))
                    return true;

                // This happens when it is a desktop image mode info block
                if (InfoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE &&
                    Id == other.Id &&  // Disabling this check as as the Display ID it maps to will change after a switch from clone to non-clone profile, ruining the equality match
                    DesktopImageInfo.Equals(other.DesktopImageInfo))
                    return true;

                // This happens when it is a clone - there is an extra entry with all zeros in it!
                if (InfoType == DISPLAYCONFIG_MODE_INFO_TYPE.Zero &&
                    //Id == other.Id && // Disabling this check as as the Display ID it maps to will change after a switch from clone to non-clone profile, ruining the equality match
                    DesktopImageInfo.Equals(other.DesktopImageInfo) &&
                    TargetMode.Equals(other.TargetMode) &&
                    SourceMode.Equals(other.SourceMode))
                    return true;

                return false;
            }

            public override int GetHashCode()
            {
                if (InfoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET)
                    return (InfoType, Id, TargetMode).GetHashCode();
                if (InfoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE)
                    return (InfoType, SourceMode).GetHashCode();
                if (InfoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE)
                    return (InfoType, Id, DesktopImageInfo).GetHashCode();
                // otherwise we return everything
                return (InfoType, Id, TargetMode, SourceMode, DesktopImageInfo).GetHashCode();
            }

            public static bool operator ==(DISPLAYCONFIG_MODE_INFO lhs, DISPLAYCONFIG_MODE_INFO rhs) => lhs.Equals(rhs);
            public static bool operator !=(DISPLAYCONFIG_MODE_INFO lhs, DISPLAYCONFIG_MODE_INFO rhs) => !(lhs == rhs);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_DEVICE_INFO_HEADER
        {
            public DISPLAYCONFIG_DEVICE_INFO_TYPE Type;
            public uint Size;
            public LUID AdapterId;
            public uint Id;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS
        {
            public uint Value;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DISPLAYCONFIG_TARGET_DEVICE_NAME
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER Header;
            public DISPLAYCONFIG_TARGET_DEVICE_NAME_FLAGS Flags;
            public DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY OutputTechnology;
            public ushort EdidManufactureId;
            public ushort EdidProductCodeId;
            public uint ConnectorInstance;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string MonitorFriendlyDeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string MonitorDevicePath;
        }
        #endregion

        public static string LastError
        {
            get
            {
                var err = Marshal.GetLastWin32Error();
                if (FormatMessage(FORMAT_MESSAGE_FLAGS, FORMAT_MESSAGE_FROM_HMODULE, (uint)err, 0, out string msg, 0, 0) == 0)
                    return Properties.Resources.InvalidOperation_FatalError;
                else
                    return msg;
            }
        }
    }
}