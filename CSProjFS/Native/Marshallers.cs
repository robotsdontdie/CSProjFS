using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using static CSProjFS.Native.ProjectedFSLib;

namespace CSProjFS.Native
{
    internal static class Marshallers
    {
        [CustomMarshaller(typeof(PRJ_CALLBACKS), MarshalMode.ManagedToUnmanagedIn, typeof(PrjCallbacksMarshaller))]
        internal unsafe static class PrjCallbacksMarshaller
        {
            internal static PRJ_CALLBACKS_UNMANAGED ConvertToUnmanaged(in PRJ_CALLBACKS managed)
            {
                PRJ_CALLBACKS_UNMANAGED unmanaged = default;
                unmanaged.StartDirectoryEnumerationCallbackPtr = (nint)managed.StartDirectoryEnumerationCallback;
                unmanaged.EndDirectoryEnumerationCallbackPtr = (nint)(managed.EndDirectoryEnumerationCallback);
                unmanaged.GetDirectoryEnumerationCallbackPtr = (nint)(managed.GetDirectoryEnumerationCallback);
                unmanaged.GetPlaceholderInfoCallbackPtr = (nint)(managed.GetPlaceholderInfoCallback);
                unmanaged.GetFileDataCallbackPtr = (nint)(managed.GetFileDataCallback);
                unmanaged.QueryFileNameCallbackPtr = (nint)(managed.QueryFileNameCallback);
                unmanaged.NotificationCallbackPtr = (nint)(managed.NotificationCallback);
                unmanaged.CancelCommandCallbackPtr = (nint)(managed.CancelCommandCallback);
                return unmanaged;
            }
        }

        [CustomMarshaller(typeof(PRJ_CALLBACK_DATA), MarshalMode.UnmanagedToManagedIn, typeof(PrjCallbackDataMarshaller))]
        internal static class PrjCallbackDataMarshaller
        {
            internal static unsafe PRJ_CALLBACK_DATA ConvertToManaged(in PRJ_CALLBACK_DATA_UNMANAGED* unmanagedPtr)
            {
                ref PRJ_CALLBACK_DATA_UNMANAGED unmanaged = ref Unsafe.AsRef<PRJ_CALLBACK_DATA_UNMANAGED>(unmanagedPtr);

                string filePathName = Utf16StringMarshaller.ConvertToManaged(unmanaged.FilePathName)
                    ?? throw new ArgumentException("FilePathName cannot be null.");

                string? triggeringProccessImageFileName = Utf16StringMarshaller.ConvertToManaged(unmanaged.TriggeringProcessImageFileName);

                PRJ_PLACEHOLDER_VERSION_INFO? versionInfo = null;
                if (unmanaged.VersionInfo != 0)
                {
                    ref PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED unmanagedVersionInfo = ref Unsafe.AsRef<PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED>((void*)unmanaged.VersionInfo);
                    versionInfo = PrjPlaceholderVersionInfoMarshaller.ConvertToManaged(in unmanagedVersionInfo);
                }

                return new()
                {
                    Size = unmanaged.Size,
                    Flags = unmanaged.Flags,
                    NamespaceVirtualizationContext = unmanaged.NamespaceVirtualizationContext,
                    CommandId = unmanaged.CommandId,
                    FileId = unmanaged.FileId,
                    DataStreamId = unmanaged.DataStreamId,
                    FilePathName = filePathName,
                    VersionInfo = versionInfo,
                    TriggeringProcessId = unmanaged.TriggeringProcessId,
                    TriggeringProcessImageFileName = triggeringProccessImageFileName,
                    InstanceContext = (nint)unmanaged.InstanceContext,
                };
            }
        }

        [CustomMarshaller(typeof(PRJ_PLACEHOLDER_VERSION_INFO), MarshalMode.ManagedToUnmanagedIn, typeof(PrjPlaceholderVersionInfoMarshaller))]
        internal static unsafe class PrjPlaceholderVersionInfoMarshaller
        {
            private const int IdLength = (int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH;

            public static PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED ConvertToUnmanaged(in PRJ_PLACEHOLDER_VERSION_INFO managed)
            {
                var unmanaged = new PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED();

                Copy(managed.ProviderID, unmanaged.ProviderID);
                Copy(managed.ContentID, unmanaged.ContentID);
                return unmanaged;
            }

            internal static PRJ_PLACEHOLDER_VERSION_INFO? ConvertToManaged(in PRJ_PLACEHOLDER_VERSION_INFO_UNMANGED unmanagedVersionInfo)
            {
                fixed(void* providerId = unmanagedVersionInfo.ProviderID)
                fixed(void* contentId = unmanagedVersionInfo.ContentID)
                {
                    return new PRJ_PLACEHOLDER_VERSION_INFO
                    {
                        ProviderID = Copy(providerId),
                        ContentID = Copy(contentId),
                    };
                }
            }

            private static void Copy(byte[]? source, void* destination)
            {
                if (source is null)
                {
                    return;
                }

                fixed (void* sourcePtr = source)
                {
                    int length = Math.Min(IdLength, source.Length);
                    NativeMemory.Copy(sourcePtr, destination, (uint)length);
                }
            }

            private static byte[] Copy(void* source)
            {
                var destination = new byte[IdLength];
                fixed(void* destinationPtr = destination)
                {
                    NativeMemory.Copy(source, destinationPtr, IdLength);
                }

                return destination;
            }
        }

        [CustomMarshaller(typeof(PRJ_PLACEHOLDER_INFO), MarshalMode.ManagedToUnmanagedIn, typeof(PrjPlaceholderInfoMarshaller))]
        internal static unsafe class PrjPlaceholderInfoMarshaller
        {
            public static PRJ_PLACEHOLDER_INFO_UNMANAGED ConvertToUnmanaged(in PRJ_PLACEHOLDER_INFO managed)
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

        [CustomMarshaller(typeof(PRJ_STARTVIRTUALIZING_OPTIONS), MarshalMode.ManagedToUnmanagedIn, typeof(PrjStartVirtualizingMarshaller))]
        internal static unsafe class PrjStartVirtualizingMarshaller
        {
            const nuint EnumSize = sizeof(PRJ_NOTIFY_TYPES);
            private static readonly nuint StringSize = (nuint) IntPtr.Size;

            public static PRJ_STARTVIRTUALIZING_OPTIONS_UNMANAGED ConvertToUnmanaged(in PRJ_STARTVIRTUALIZING_OPTIONS managed)
            {
                var unmanaged = new PRJ_STARTVIRTUALIZING_OPTIONS_UNMANAGED()
                {
                    Flags = managed.Flags,
                    PoolThreadCount = managed.PoolThreadCount,
                    ConcurrentThreadCount = managed.ConcurrentThreadCount,
                };

                if (managed.NotificationMappings is null)
                {
                    unmanaged.NotificationMappings = null;
                    unmanaged.NotificationMappingsCount = 0;
                }
                else
                {
                    checked
                    {
                        unmanaged.NotificationMappingsCount = (uint)managed.NotificationMappings.Length;

                        nuint arraySize = (EnumSize + StringSize) * (nuint)managed.NotificationMappings.Length;
                        unmanaged.NotificationMappings = (PRJ_NOTIFICATION_MAPPING_UNMANAGED*)NativeMemory.Alloc(arraySize);

                        for (int i = 0; i < managed.NotificationMappings.Length; i++)
                        {
                            unmanaged.NotificationMappings[i] = ConvertToManaged(managed.NotificationMappings[i]);
                        }
                    }
                }

                return unmanaged;
            }

            public static void Free(in PRJ_STARTVIRTUALIZING_OPTIONS_UNMANAGED unmanaged)
            {
                for (int i = 0; i < unmanaged.NotificationMappingsCount; i++)
                {
                    Free(unmanaged.NotificationMappings[i]);
                }

                NativeMemory.Free(unmanaged.NotificationMappings);
            }

            private static PRJ_NOTIFICATION_MAPPING_UNMANAGED ConvertToManaged(in PRJ_NOTIFICATION_MAPPING managed)
            {
                var unmanaged = new PRJ_NOTIFICATION_MAPPING_UNMANAGED();
                unmanaged.NotificationBitMask = managed.NotificationBitMask;
                unmanaged.NotificationRoot = Utf16StringMarshaller.ConvertToUnmanaged(managed.NotificationRoot);

                return unmanaged;
            }

            private static void Free(in PRJ_NOTIFICATION_MAPPING_UNMANAGED unmanaged)
            {
                Utf16StringMarshaller.Free(unmanaged.NotificationRoot);
            }
        }        
    }
}
