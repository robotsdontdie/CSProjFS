using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace ProjFS.Native
{
    internal static partial class ProjectedFSLib
    {
        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_callback_data
        unsafe internal struct PRJ_CALLBACK_DATA
        {
            public uint Size;
            public PRJ_CALLBACK_DATA_FLAGS Flags;
            public IntPtr NamespaceVirtualizationContext;
            public int CommandId;
            public Guid FileId;
            public Guid DataStreamId;
            public ushort* FilePathName;
            public IntPtr VersionInfo; // PRJ_PLACEHOLDER_VERSION_INFO*
            public uint TriggeringProcessId;
            public ushort* TriggeringProcessImageFileName;
            public IntPtr InstanceContext;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_callbacks
        unsafe internal struct PRJ_CALLBACKS
        {
            public delegate*<ref PRJ_CALLBACK_DATA, ref Guid, HResult> StartDirectoryEnumerationCallback;
            public delegate*<ref PRJ_CALLBACK_DATA, ref Guid, HResult> EndDirectoryEnumerationCallback;
            public delegate*<ref PRJ_CALLBACK_DATA, ref Guid, ushort*, IntPtr, HResult> GetDirectoryEnumerationCallback;
            public delegate*<ref PRJ_CALLBACK_DATA, HResult> GetPlaceholderInfoCallback;
            public delegate*<ref PRJ_CALLBACK_DATA, ulong, uint, HResult> GetFileDataCallback;
            public delegate*<ref PRJ_CALLBACK_DATA, HResult> QueryFileNameCallback;
            public delegate*<ref PRJ_CALLBACK_DATA, int, PRJ_NOTIFICATION, ushort*, PRJ_NOTIFICATION_PARAMETERS*, HResult> NotificationCallback;
            public delegate*<ref PRJ_CALLBACK_DATA, void> CancelCommandCallback;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_complete_command_extended_parameters
        internal struct PRJ_COMPLETE_COMMAND_EXTENDED_PARAMETERS
        {
            public int Field1;
            public int Field2;
        }

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
        [StructLayout(LayoutKind.Sequential)]
        internal struct PRJ_FILE_BASIC_INFO
        {
            public bool IsDirectory;
            public long FileSize;
            public long CreationTime;
            public long LastAccessTime;
            public long LastWriteTime;
            public long ChangeTime;
            public uint FileAttributes;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_notification_mapping
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct PRJ_NOTIFICATION_MAPPING
        {
            public PRJ_NOTIFY_TYPES NotificationBitMask;
            public ushort* NotificationRoot;
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_notification_parameters
        internal struct PRJ_NOTIFICATION_PARAMETERS
        {
            public uint Data;
        }

        internal struct PRJ_NOTIFICATION_PARAMETERS_POSTCREATE
        {

            public PRJ_NOTIFY_TYPES NotificationMask;
        }

        internal struct PRJ_NOTIFICATION_PARAMETERS_FILERENAMED
        {
            public PRJ_NOTIFY_TYPES NotificationMask;
        }

        internal struct PRJ_NOTIFICATION_PARAMETERS_FILEDELETEDONHANDLECLOSE
        {
            public bool IsFileModified;
        }

        [CustomMarshaller(typeof(PRJ_PLACEHOLDER_INFO), MarshalMode.ManagedToUnmanagedIn, typeof(PrjPlaceholderInfoMarshaller))]
        internal static unsafe class PrjPlaceholderInfoMarshaller
        {
            private const int IdLength = (int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH;
            private const int byteSize = 8;

            internal struct PRJ_PLACEHOLDER_INFO_UNMANAGED
            {
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

                public PrjPlaceholderVersionInfoMarshaller.PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED VersionInfo;
                public IntPtr VariableData; // byte[1]
            }

            public static PRJ_PLACEHOLDER_INFO_UNMANAGED ConvertToUnmanaged(PRJ_PLACEHOLDER_INFO managed)
            {
                return new PRJ_PLACEHOLDER_INFO_UNMANAGED
                {
                    FileBasicInfo = managed.FileBasicInfo,
                    EaBufferSize = managed.EaBufferSize,
                    OffsetToFirstEa = managed.OffsetToFirstEa,
                    SecurityBufferSize = managed.SecurityBufferSize,
                    OffsetToSecurityDescriptor = managed.OffsetToSecurityDescriptor,
                    StreamsInfoBufferSize = managed.StreamsInfoBufferSize,
                    OffsetToFirstStreamInfo = managed.OffsetToFirstStreamInfo,
                    VersionInfo = PrjPlaceholderVersionInfoMarshaller.ConvertToUnmanaged(managed.VersionInfo),
                    VariableData = managed.VariableData,
                };
            }

            public static void Free(PRJ_PLACEHOLDER_INFO_UNMANAGED unmanaged)
            {
                PrjPlaceholderVersionInfoMarshaller.Free(unmanaged.VersionInfo);
            }
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_placeholder_info
        internal struct PRJ_PLACEHOLDER_INFO
        {
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
            public IntPtr VariableData; // byte[1]
        }

        [CustomMarshaller(typeof(PRJ_PLACEHOLDER_VERSION_INFO), MarshalMode.ManagedToUnmanagedIn, typeof(PrjPlaceholderVersionInfoMarshaller))]
        internal static unsafe class PrjPlaceholderVersionInfoMarshaller
        {
            private const int IdLength = (int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH;

            internal struct PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED
            {
                public IntPtr ProviderID;
                public IntPtr ContentID;
            }

            public static PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED ConvertToUnmanaged(PRJ_PLACEHOLDER_VERSION_INFO managed)
            {
                return new PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED
                {
                    ProviderID = ConvertToUnmanaged(managed.ProviderID),
                    ContentID = ConvertToUnmanaged(managed.ContentID),
                };
            }

            private static IntPtr ConvertToUnmanaged(byte[]? managed)
            {
                if (managed is null)
                {
                    return IntPtr.Zero;
                }

                var unmanaged = Marshal.AllocCoTaskMem(IdLength);
                Marshal.Copy(managed, 0, unmanaged, IdLength);
                return unmanaged;
            }

            public static void Free(PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED unmanaged)
            {
                Marshal.FreeCoTaskMem(unmanaged.ProviderID);
                Marshal.FreeCoTaskMem(unmanaged.ContentID);
            }
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_placeholder_version_info
        internal struct PRJ_PLACEHOLDER_VERSION_INFO
        {
            // max length is PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH
            public byte[]? ProviderID;
            public byte[]? ContentID;

            internal void Init()
            {
                ProviderID = new byte[(int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH];
                ContentID = new byte[(int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH];
            }
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_startvirtualizing_options
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct PRJ_STARTVIRTUALIZING_OPTIONS
        {
            public PRJ_STARTVIRTUALIZING_FLAGS Flags;
            public uint PoolThreadCount;
            public uint ConcurrentThreadCount;
            public IntPtr NotificationMappings; // PRJ_NOTIFICATION_MAPPING*
            public uint NotificationMappingsCount;
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
