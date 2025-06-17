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
            public static int Size = 4 + 8 * 5 + 4;

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
        internal struct PRJ_NOTIFICATION_MAPPING
        {
            public PRJ_NOTIFY_TYPES NotificationBitMask;
            public string? NotificationRoot;
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
                public byte VariableData;
            }

            public static PRJ_PLACEHOLDER_INFO_UNMANAGED ConvertToUnmanaged(PRJ_PLACEHOLDER_INFO managed)
            {
                if (managed.EaBufferSize != 0
                    || managed.SecurityBufferSize != 0
                    || managed.StreamsInfoBufferSize != 0)
                {
                    throw new ArgumentException("Optional placeholder info not supported");
                }

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
                    VariableData = 0,
                };
            }
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_placeholder_info
        [StructLayout(LayoutKind.Sequential)]
        internal struct PRJ_PLACEHOLDER_INFO
        {
            public int Size
            {
                get
                {
                    return PRJ_FILE_BASIC_INFO.Size
                        + 8 // EA info
                        + 8 // Security info
                        + 8 // Streams info
                        + (int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH * 2 // version info
                        + 1; // we dont actually support variable data atm and just write the one byte in the struct
                }
            }

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

        [CustomMarshaller(typeof(PRJ_PLACEHOLDER_VERSION_INFO), MarshalMode.ManagedToUnmanagedIn, typeof(PrjPlaceholderVersionInfoMarshaller))]
        internal static unsafe class PrjPlaceholderVersionInfoMarshaller
        {
            private const int IdLength = (int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH;

            internal struct PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED
            {
                public fixed byte ProviderID[IdLength];
                public fixed byte ContentID[IdLength];
            }

            public static PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED ConvertToUnmanaged(PRJ_PLACEHOLDER_VERSION_INFO managed)
            {
                var unmanaged = new PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED();
                Marshal.Copy(managed.ProviderID, 0, (nint)unmanaged.ProviderID, IdLength);
                Marshal.Copy(managed.ContentID, 0, (nint)unmanaged.ContentID, IdLength);

                return unmanaged;
            }
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_placeholder_version_info
        internal struct PRJ_PLACEHOLDER_VERSION_INFO
        {
            public PRJ_PLACEHOLDER_VERSION_INFO()
            {
                ProviderID = new byte[(int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH];
                ContentID = new byte[(int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH];
            }

            // max length is PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH
            public byte[] ProviderID;
            public byte[] ContentID;
        }

        [CustomMarshaller(typeof(PRJ_STARTVIRTUALIZING_OPTIONS), MarshalMode.ManagedToUnmanagedIn, typeof(PrjStartVirtualizingMarshaller))]
        internal static unsafe class PrjStartVirtualizingMarshaller
        {
            private const int EnumSize = sizeof(PRJ_NOTIFY_TYPES);
            private static readonly int StringSize = sizeof(IntPtr);

            internal struct PRJ_STARTVIRTUALIZING_OPTIONS_UNMANAGED
            {
                public PRJ_STARTVIRTUALIZING_FLAGS Flags;
                public uint PoolThreadCount;
                public uint ConcurrentThreadCount;
                public IntPtr NotificationMappings;
                public uint NotificationMappingsCount;
            }

            internal unsafe struct PRJ_NOTIFICATION_MAPPING_UNMANAGED
            {
                public PRJ_NOTIFY_TYPES NotificationBitMask;
                public ushort* NotificationRoot;
            }

            public static PRJ_STARTVIRTUALIZING_OPTIONS_UNMANAGED ConvertToUnmanaged(PRJ_STARTVIRTUALIZING_OPTIONS managed)
            {
                var unmanaged = new PRJ_STARTVIRTUALIZING_OPTIONS_UNMANAGED()
                {
                    Flags = managed.Flags,
                    PoolThreadCount = managed.PoolThreadCount,
                    ConcurrentThreadCount = managed.ConcurrentThreadCount,
                };

                if (managed.NotificationMappings is null)
                {
                    unmanaged.NotificationMappings = IntPtr.Zero;
                    unmanaged.NotificationMappingsCount = 0;
                }
                else
                {

                    int arraySize = checked((EnumSize + StringSize) * managed.NotificationMappings.Length);
                    unmanaged.NotificationMappings = Marshal.AllocCoTaskMem(arraySize);
                    int offset = 0;

                    foreach (var mapping in managed.NotificationMappings)
                    {
                        Marshal.WriteInt32(unmanaged.NotificationMappings, offset, (int)mapping.NotificationBitMask);
                        offset += EnumSize;

                        var unmanagedString = (IntPtr)Utf16StringMarshaller.ConvertToUnmanaged(mapping.NotificationRoot ?? "");
                        Marshal.WriteIntPtr(unmanaged.NotificationMappings, offset, unmanagedString);
                        offset += StringSize;
                    }
                }

                return unmanaged;
            }

            public static void Free(PRJ_STARTVIRTUALIZING_OPTIONS_UNMANAGED unmanaged)
            {
                int offset = 0;
                for (int i = 0; i < unmanaged.NotificationMappingsCount; i++)
                {
                    offset += EnumSize;
                    Utf16StringMarshaller.Free((ushort*)(unmanaged.NotificationMappings + offset));
                    offset += StringSize;
                }

                Marshal.FreeCoTaskMem(unmanaged.NotificationMappings);
            }
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/ns-projectedfslib-prj_startvirtualizing_options
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct PRJ_STARTVIRTUALIZING_OPTIONS
        {
            public PRJ_STARTVIRTUALIZING_FLAGS Flags;
            public uint PoolThreadCount;
            public uint ConcurrentThreadCount;
            public PRJ_NOTIFICATION_MAPPING[] NotificationMappings; 
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
