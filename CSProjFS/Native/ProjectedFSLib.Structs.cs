using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace CSProjFS.Native
{
    internal static partial class ProjectedFSLib
    {
        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_callback_data
        internal unsafe struct PRJ_CALLBACK_DATA_UNMANAGED
        {
            public uint Size;
            public PRJ_CALLBACK_DATA_FLAGS Flags;
            public IntPtr NamespaceVirtualizationContext;
            public int CommandId;
            public Guid FileId;
            public Guid DataStreamId;
            public ushort* FilePathName;
            public IntPtr VersionInfo;
            public uint TriggeringProcessId;
            public ushort* TriggeringProcessImageFileName;
            public IntPtr InstanceContext;
        }

        internal struct PRJ_CALLBACK_DATA
        {
            internal uint Size;
            internal PRJ_CALLBACK_DATA_FLAGS Flags;
            internal IntPtr NamespaceVirtualizationContext;
            internal int CommandId;
            internal Guid FileId;
            internal Guid DataStreamId;
            internal string FilePathName;
            internal PRJ_PLACEHOLDER_VERSION_INFO? VersionInfo;
            internal uint TriggeringProcessId;
            internal string? TriggeringProcessImageFileName;
            internal IntPtr InstanceContext;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_callbacks
        unsafe internal struct PRJ_CALLBACKS
        {
            public delegate* unmanaged[Stdcall]<PRJ_CALLBACK_DATA_UNMANAGED*, Guid*, HResult> StartDirectoryEnumerationCallback;
            public delegate* unmanaged[Stdcall]<PRJ_CALLBACK_DATA_UNMANAGED*, Guid*, HResult> EndDirectoryEnumerationCallback;
            public delegate* unmanaged[Stdcall]<PRJ_CALLBACK_DATA_UNMANAGED*, Guid*, ushort*, IntPtr, HResult> GetDirectoryEnumerationCallback;
            public delegate* unmanaged[Stdcall]<PRJ_CALLBACK_DATA_UNMANAGED*, HResult> GetPlaceholderInfoCallback;
            public delegate* unmanaged[Stdcall]<PRJ_CALLBACK_DATA_UNMANAGED*, ulong, uint, HResult> GetFileDataCallback;
            public delegate* unmanaged[Stdcall]<PRJ_CALLBACK_DATA_UNMANAGED*, HResult> QueryFileNameCallback;
            public delegate* unmanaged[Stdcall]<PRJ_CALLBACK_DATA_UNMANAGED*, int, PRJ_NOTIFICATION, ushort*, PRJ_NOTIFICATION_PARAMETERS*, HResult> NotificationCallback;
            public delegate* unmanaged[Stdcall]<PRJ_CALLBACK_DATA_UNMANAGED*, void> CancelCommandCallback;
        }

        internal struct PRJ_CALLBACKS_UNMANAGED
        {
            internal nint StartDirectoryEnumerationCallbackPtr;
            internal nint EndDirectoryEnumerationCallbackPtr;
            internal nint GetDirectoryEnumerationCallbackPtr;
            internal nint GetPlaceholderInfoCallbackPtr;
            internal nint GetFileDataCallbackPtr;
            internal nint QueryFileNameCallbackPtr;
            internal nint NotificationCallbackPtr;
            internal nint CancelCommandCallbackPtr;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_complete_command_extended_parameters
        internal struct PRJ_COMPLETE_COMMAND_EXTENDED_PARAMETERS_NOTIFICATION
        {
            public PRJ_COMPLETE_COMMAND_TYPE CommandType;
            public PRJ_NOTIFY_TYPES NotificationMask;
        }

        internal struct PRJ_COMPLETE_COMMAND_EXTENDED_PARAMETERS_ENUMERATION
        {
            public PRJ_COMPLETE_COMMAND_TYPE CommandType;
            public IntPtr DirEntryBufferHandle;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_file_basic_info
        [StructLayout(LayoutKind.Explicit, Size = 52)]
        internal struct PRJ_FILE_BASIC_INFO
        {
            [FieldOffset(0)] public bool IsDirectory;
            [FieldOffset(8)] public long FileSize;
            [FieldOffset(16)] public long CreationTime;
            [FieldOffset(24)] public long LastAccessTime;
            [FieldOffset(32)] public long LastWriteTime;
            [FieldOffset(40)] public long ChangeTime;
            [FieldOffset(48)] public uint FileAttributes;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_notification_mapping
        internal struct PRJ_NOTIFICATION_MAPPING
        {
            public PRJ_NOTIFY_TYPES NotificationBitMask;
            public string? NotificationRoot;
        }

        internal unsafe struct PRJ_NOTIFICATION_MAPPING_UNMANAGED
        {
            public PRJ_NOTIFY_TYPES NotificationBitMask;
            public ushort* NotificationRoot;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_notification_parameters
        internal struct PRJ_NOTIFICATION_PARAMETERS
        {
            public uint Data;
        }

        [StructLayout(LayoutKind.Explicit, Size = 344)]
        internal struct PRJ_PLACEHOLDER_INFO_UNMANAGED
        {
            [FieldOffset(0)] public PRJ_FILE_BASIC_INFO FileBasicInfo;
            //EaInformation
            [FieldOffset(56)] public uint EaBufferSize;
            [FieldOffset(60)] public uint OffsetToFirstEa;

            //SecurityInformation
            [FieldOffset(64)] public uint SecurityBufferSize;
            [FieldOffset(68)] public uint OffsetToSecurityDescriptor;

            //StreamsInformation
            [FieldOffset(72)] public uint StreamsInfoBufferSize;
            [FieldOffset(76)] public uint OffsetToFirstStreamInfo;

            [FieldOffset(80)] public PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED VersionInfo;
            [FieldOffset(336)] public byte VariableData;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_placeholder_info
        [StructLayout(LayoutKind.Sequential)]
        internal struct PRJ_PLACEHOLDER_INFO
        {
            public const int Size = 344;

            public PRJ_FILE_BASIC_INFO FileBasicInfo;
            //EaInformation
            public uint EaBufferSize;
            public uint OffsetToFirstEa;

            //SecurityInformation
            public uint SecurityBufferSize;
            public uint OffsetToSecurityDescriptor;

            //StreamsInformation
            public uint StreamsInfoBufferSize;
            public uint OffsetToFirstStreamInfo;

            public PRJ_PLACEHOLDER_VERSION_INFO VersionInfo;
            //public byte[] VariableData;
        }

        internal unsafe struct PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED
        {
            public fixed byte ProviderID[(int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH];
            public fixed byte ContentID[(int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH];
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_placeholder_version_info
        internal struct PRJ_PLACEHOLDER_VERSION_INFO
        {
            public const int IdLength = (int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH;

            public PRJ_PLACEHOLDER_VERSION_INFO()
            {
                ProviderID = new byte[IdLength];
                ContentID = new byte[IdLength];
            }

            // max length is PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH
            public byte[] ProviderID;
            public byte[] ContentID;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_startvirtualizing_options
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct PRJ_STARTVIRTUALIZING_OPTIONS_UNMANAGED
        {
            public PRJ_STARTVIRTUALIZING_FLAGS Flags;
            public uint PoolThreadCount;
            public uint ConcurrentThreadCount;
            public PRJ_NOTIFICATION_MAPPING_UNMANAGED* NotificationMappings;
            public uint NotificationMappingsCount;
        }

        /// <summary>
        /// Managed version of <see cref="PRJ_STARTVIRTUALIZING_OPTIONS"/>.
        /// </summary>
        internal struct PRJ_STARTVIRTUALIZING_OPTIONS
        {
            public PRJ_STARTVIRTUALIZING_FLAGS Flags;
            public uint PoolThreadCount;
            public uint ConcurrentThreadCount;
            public PRJ_NOTIFICATION_MAPPING[] NotificationMappings;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_virtualization_instance_info
        internal struct PRJ_VIRTUALIZATION_INSTANCE_INFO
        {
            public Guid InstanceID;
            public uint WriteAlignment;
        }

        [CustomMarshaller(typeof(PRJ_EXTENDED_INFO), MarshalMode.ManagedToUnmanagedIn, typeof(PrjExtendedInfoMarshaller))]
        internal static unsafe class PrjExtendedInfoMarshaller
        {
            internal struct PRJ_EXTENDED_INFO_UNMANAGED
            {
                public int InfoType;
                public uint NextInfoOffset;
                public ushort* Targetname;
            }

            public static PRJ_EXTENDED_INFO_UNMANAGED ConvertToUnmanaged(PRJ_EXTENDED_INFO managed)
            {
                return new PRJ_EXTENDED_INFO_UNMANAGED
                {
                    InfoType = (int)managed.InfoType,
                    NextInfoOffset = managed.NextInfoOffset,
                    Targetname = Utf16StringMarshaller.ConvertToUnmanaged(managed.TargetName),
                };
            }

            public static void Free(PRJ_EXTENDED_INFO_UNMANAGED unmanaged)
            {
                Utf16StringMarshaller.Free(unmanaged.Targetname);
            }
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_extended_info
        [StructLayout(LayoutKind.Sequential)]
        internal struct PRJ_EXTENDED_INFO
        {
            public PRJ_EXT_INFO_TYPE InfoType;
            public uint NextInfoOffset;
            public string? TargetName;
        }

       
    }
}
