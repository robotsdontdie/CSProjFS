using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using ProjFS.Native;
using static ProjFS.Native.ProjectedFSLib;

namespace ProjFS
{
    public class VirtualizationInstance
    {
        private static readonly ConcurrentDictionary<int, VirtualizationInstance> instances = [];
        private static readonly Random random = new();

        private readonly string virtualizationRootPath;
        private readonly uint poolThreadCount;
        private readonly uint concurrentThreadCount;
        private readonly bool enableNegativePathCache;
        private readonly IReadOnlyCollection<NotificationMapping> notificationMappings;

        private int initialized = 0;
        private int instanceContext = 0;
        private Guid virtualizationInstanceId = default;

        private IRequiredCallbacks? requiredCallbacks = null;

        public VirtualizationInstance(
            string virtualizationRootPath,
            uint poolThreadCount,
            uint concurrentThreadCount,
            bool enableNegativePathCache,
            IReadOnlyCollection<NotificationMapping> notificationMappings)
        {
            this.virtualizationRootPath = virtualizationRootPath;
            this.poolThreadCount = poolThreadCount;
            this.concurrentThreadCount = concurrentThreadCount;
            this.notificationMappings = [.. notificationMappings];

            bool markAsRoot = false;

            var dirInfo = new DirectoryInfo(virtualizationRootPath);
            Guid virtualizationInstanceID = Guid.NewGuid();
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
                markAsRoot = true;
            }
            else
            {
                // TODO reimplement the check to see if its already a reparse point
            }

            if (markAsRoot)
            {
                var versionInfo = new PRJ_PLACEHOLDER_VERSION_INFO();
                ProjectedFSLib.PrjMarkDirectoryAsPlaceholder(virtualizationRootPath, null, versionInfo, ref virtualizationInstanceID);
            }
        }

        /// <summary>
        /// Stores the provider's implementation of <see cref="QueryFileNameCallback"/>.
        /// </summary>
        /// <seealso cref="QueryFileNameCallback"/>
        /// <remarks>The provider must set this property prior to calling <see cref="StartVirtualizing"/>.</remarks>
        public QueryFileNameCallback? OnQueryFileName { get; set; }

        /// <summary>Stores the provider's implementation of <see cref="CancelCommandCallback"/>.</summary>
        /// <seealso cref="CancelCommandCallback"/>
        /// <remarks>
        /// <para>
        /// If the provider wishes to support asynchronous processing of callbacks (that is, if it
        /// intends to return <see cref="HResult::Pending"/> from any of its callbacks), then the provider
        /// must set this property prior to calling <see cref="StartVirtualizing"/>.
        /// </para>
        /// <para>
        /// If the provider does not wish to support asynchronous processing of callbacks, then it
        /// is not required to provide an implementation of this callback.
        /// </para>
        /// <para>If the provider does implement this callback, then it must set this property prior to
        /// calling <see cref="StartVirtualizing"/>.</para>
        /// </remarks>
        public CancelCommandCallback? OnCancelCommand { get; set; }

        /// <summary>Stores the provider's implementation of <see cref="NotifyFileOpenedCallback"/>.</summary>
        /// <seealso cref="NotifyFileOpenedCallback"/>
        /// <remarks>
        /// <para>The provider is not required to provide an implementation of this callback.
        /// If it does not provide this callback, the provider will not receive notifications when
        /// a file has been opened.</para>
        /// <para>If the provider does implement this callback, then it must set this property prior to
        /// calling <see cref="StartVirtualizing"/>.</para>
        /// </remarks>
        public NotifyFileOpenedCallback? OnNotifyFileOpened { get; set; }

        /// <summary>Stores the provider's implementation of <see cref="NotifyNewFileCreatedCallback"/>.</summary>
        /// <seealso cref="NotifyNewFileCreatedCallback"/>
        /// <remarks>
        /// <para>The provider is not required to provide an implementation of this callback.
        /// If it does not provide this callback, the provider will not receive notifications when
        /// a new file has been created.</para>
        /// <para>If the provider does implement this callback, then it must set this property prior to
        /// calling <see cref="StartVirtualizing"/>.</para>
        /// </remarks>
        public NotifyNewFileCreatedCallback? OnNotifyNewFileCreated { get; set; }

        /// <summary>Stores the provider's implementation of <see cref="NotifyFileOverwrittenCallback"/>.</summary>
        /// <seealso cref="NotifyFileOverwrittenCallback"/>
        /// <remarks>
        /// <para>The provider is not required to provide an implementation of this callback.
        /// If it does not provide this callback, the provider will not receive notifications when
        /// a file has been superseded or overwritten.</para>
        /// <para>If the provider does implement this callback, then it must set this property prior to
        /// calling <see cref="StartVirtualizing"/>.</para>
        /// </remarks>
        public NotifyFileOverwrittenCallback? OnNotifyFileOverwritten { get; set; }

        /// <summary>Stores the provider's implementation of <see cref="NotifyPreDeleteCallback"/>.</summary>
        /// <seealso cref="NotifyPreDeleteCallback"/>
        /// <remarks>
        /// <para>The provider is not required to provide an implementation of this callback.
        /// If it does not provide this callback, the provider will not receive notifications when
        /// a file is about to be deleted.</para>
        /// <para>If the provider does implement this callback, then it must set this property prior to
        /// calling <see cref="StartVirtualizing"/>.</para>
        /// </remarks>
        public NotifyPreDeleteCallback? OnNotifyPreDelete { get; set; }

        /// <summary>Stores the provider's implementation of <see cref="NotifyPreRenameCallback"/>.</summary>
        /// <seealso cref="NotifyPreRenameCallback"/>
        /// <remarks>
        /// <para>The provider is not required to provide an implementation of this callback.
        /// If it does not provide this callback, the provider will not receive notifications when
        /// a file is about to be renamed.</para>
        /// <para>If the provider does implement this callback, then it must set this property prior to
        /// calling <see cref="StartVirtualizing"/>.</para>
        /// </remarks>
        public NotifyPreRenameCallback? OnNotifyPreRename { get; set; }


        /// <summary>Stores the provider's implementation of <see cref="NotifyPreCreateHardlinkCallback"/>.</summary>
        /// <seealso cref="NotifyPreCreateHardlinkCallback"/>
        /// <remarks>
        /// <para>The provider is not required to provide an implementation of this callback.
        /// If it does not provide this callback, the provider will not receive notifications when
        /// a hard link is about to be created for a file.</para>
        /// <para>If the provider does implement this callback, then it must set this property prior to
        /// calling <see cref="StartVirtualizing"/>.</para>
        /// </remarks>
        public NotifyPreCreateHardlinkCallback? OnNotifyPreCreateHardlink { get; set; }


        /// <summary>Stores the provider's implementation of <see cref="NotifyFileRenamedCallback"/>.</summary>
        /// <seealso cref="NotifyFileRenamedCallback"/>
        /// <remarks>
        /// <para>The provider is not required to provide an implementation of this callback.
        /// If it does not provide this callback, the provider will not receive notifications when
        /// a file has been renamed.</para>
        /// <para>If the provider does implement this callback, then it must set this property prior to
        /// calling <see cref="StartVirtualizing"/>.</para>
        /// </remarks>
        public NotifyFileRenamedCallback? OnNotifyFileRenamed { get; set; }


        /// <summary>Stores the provider's implementation of <see cref="NotifyHardlinkCreatedCallback"/>.</summary>
        /// <seealso cref="NotifyHardlinkCreatedCallback"/>
        /// <remarks>
        /// <para>The provider is not required to provide an implementation of this callback.
        /// If it does not provide this callback, the provider will not receive notifications when
        /// a hard link has been created for a file.</para>
        /// <para>If the provider does implement this callback, then it must set this property prior to
        /// calling <see cref="StartVirtualizing"/>.</para>
        /// </remarks>
        public NotifyHardlinkCreatedCallback? OnNotifyHardlinkCreated { get; set; }


        /// <summary>Stores the provider's implementation of <see cref="NotifyFileHandleClosedNoModificationCallback"/>.</summary>
        /// <seealso cref="NotifyFileHandleClosedNoModificationCallback"/>
        /// <remarks>
        /// <para>The provider is not required to provide an implementation of this callback.
        /// If it does not provide this callback, the provider will not receive notifications when
        /// a file handle has been closed and the file has not been modified.</para>
        /// <para>If the provider does implement this callback, then it must set this property prior to
        /// calling <see cref="StartVirtualizing"/>.</para>
        /// </remarks>
        public NotifyFileHandleClosedNoModificationCallback? OnNotifyFileHandleClosedNoModification { get; set; }


        /// <summary>Stores the provider's implementation of <see cref="NotifyFileHandleClosedFileModifiedOrDeletedCallback"/>.</summary>
        /// <seealso cref="NotifyFileHandleClosedFileModifiedOrDeletedCallback"/>
        /// <remarks>
        /// <para>The provider is not required to provide an implementation of this callback.
        /// If it does not provide this callback, the provider will not receive notifications when
        /// a file handle has been closed on a modified file, or the file was deleted as a result
        /// of closing the handle.</para>
        /// <para>If the provider does implement this callback, then it must set this property prior to
        /// calling <see cref="StartVirtualizing"/>.</para>
        /// </remarks>
        public NotifyFileHandleClosedFileModifiedOrDeletedCallback? OnNotifyFileHandleClosedFileModifiedOrDeleted { get; set; }


        /// <summary>Stores the provider's implementation of <see cref="NotifyFilePreConvertToFullCallback"/>.</summary>
        /// <seealso cref="NotifyFilePreConvertToFullCallback"/>
        /// <remarks>
        /// <para>The provider is not required to provide an implementation of this callback.
        /// If it does not provide this callback, the provider will not receive notifications when
        /// a file is about to be converted from a placeholder to a full file. </para>
        /// <para>If the provider does implement this callback, then it must set this property prior to
        /// calling <see cref="StartVirtualizing"/>.</para>
        /// </remarks>
        public NotifyFilePreConvertToFullCallback? OnNotifyFilePreConvertToFull { get; set; }


        /// <summary>Retrieves the <see cref="IRequiredCallbacks"/> interface.</summary>
        public IRequiredCallbacks? RequiredCallbacks => requiredCallbacks;


        public HResult StartVirtualizing(IRequiredCallbacks requiredCallbacks)
        {
            if (Interlocked.CompareExchange(ref initialized, 1, 0) != 0)
            {
                return HResult.AlreadyInitialized;
            }

            this.requiredCallbacks = requiredCallbacks;

            do
            {
                instanceContext = random.Next();
            } while (instanceContext != 0 && instances.TryAdd(instanceContext, this));

            var callbacks = new PRJ_CALLBACKS();
            var startOptions = new PRJ_STARTVIRTUALIZING_OPTIONS();

            unsafe
            {
                var unmanagedStrings = new List<IntPtr>();
                IntPtr unmanagedMappingArray = default;

                try
                {
                    callbacks.StartDirectoryEnumerationCallback = &StartDirectoryEnumerationCallback;
                    callbacks.GetDirectoryEnumerationCallback = &GetDirectoryEnumerationCallback;
                    callbacks.EndDirectoryEnumerationCallback = &EndDirectoryEnumerationCallback;
                    callbacks.GetPlaceholderInfoCallback = &GetPlaceholderInfoCallback;
                    callbacks.GetFileDataCallback = &GetFileDataCallback;

                    if (OnQueryFileName is not null)
                    {
                        callbacks.QueryFileNameCallback = &QueryFileNameCallback;
                    }

                    if (OnCancelCommand is not null)
                    {
                        callbacks.CancelCommandCallback = &CancelCommandCallback;
                    }

                    if (OnNotifyFileOpened is not null
                        || OnNotifyNewFileCreated is not null
                        || OnNotifyFileOverwritten is not null
                        || OnNotifyPreDelete is not null
                        || OnNotifyPreRename is not null
                        || OnNotifyPreCreateHardlink is not null
                        || OnNotifyFileRenamed is not null
                        || OnNotifyHardlinkCreated is not null
                        || OnNotifyFileHandleClosedNoModification is not null
                        || OnNotifyFileHandleClosedFileModifiedOrDeleted is not null
                        || OnNotifyFilePreConvertToFull is not null)
                    {
                        callbacks.NotificationCallback = &NotificationCallback;
                    }

                    startOptions = new PRJ_STARTVIRTUALIZING_OPTIONS
                    {
                        Flags = enableNegativePathCache ? PRJ_STARTVIRTUALIZING_FLAGS.PRJ_FLAG_USE_NEGATIVE_PATH_CACHE : PRJ_STARTVIRTUALIZING_FLAGS.PRJ_FLAG_NONE,
                        PoolThreadCount = poolThreadCount,
                        ConcurrentThreadCount = concurrentThreadCount,
                    };

                    if (notificationMappings.Count > 0)
                    {
                        // allocate at least 1 byte so we get an actual pointer
                        int enumSize = sizeof(PRJ_NOTIFY_TYPES);
                        int stringSize = sizeof(IntPtr);
                        int arraySize = checked((enumSize + stringSize) * notificationMappings.Count);
                        unmanagedMappingArray = Marshal.AllocCoTaskMem(arraySize);
                        var offset = 0;

                        foreach (var mapping in notificationMappings)
                        {
                            Marshal.WriteInt32(unmanagedMappingArray, offset, (int)mapping.NotificationMask);
                            offset += enumSize;

                            var unmanagedString = (IntPtr)Utf16StringMarshaller.ConvertToUnmanaged(mapping.NotificationRoot ?? "");
                            unmanagedStrings.Add(unmanagedString);
                            Marshal.WriteIntPtr(unmanagedMappingArray, offset, unmanagedString);
                            offset += stringSize;
                        }

                        startOptions.NotificationMappings = unmanagedMappingArray;
                        startOptions.NotificationMappingsCount = (uint)notificationMappings.Count;
                    }
                    else
                    {
                        startOptions.NotificationMappingsCount = 0;
                    }

                    var result = PrjStartVirtualizing(virtualizationRootPath, ref callbacks, instanceContext, startOptions, out IntPtr namespaceVirtualizationContext);

                    if (result != HResult.Ok)
                    {
                        return result;
                    }

                    result = PrjGetVirtualizationInstanceInfo(instanceContext, out var virtualizationInstanceInfo);
                    if (result != HResult.Ok)
                    {
                        StopVirtualizing();
                        return result;
                    }

                    virtualizationInstanceId = virtualizationInstanceInfo.InstanceID;

                    return HResult.Ok;
                }
                finally
                {
                    foreach (var str in unmanagedStrings)
                    {
                        Utf16StringMarshaller.Free((ushort*)str);
                    }

                    if(unmanagedMappingArray != default)
                    {
                        Marshal.FreeCoTaskMem(unmanagedMappingArray);
                    }
                }
            }
        }

        public void StopVirtualizing()
        {
            if (instanceContext == 0)
            {
                return;
            }

            PrjStopVirtualizing(instanceContext);
        }

        private static HResult StartDirectoryEnumerationCallback(ref PRJ_CALLBACK_DATA callbackData, ref Guid guid)
        {
            return HResult.Ok;
        }

        private static unsafe HResult GetDirectoryEnumerationCallback(ref PRJ_CALLBACK_DATA callbackData, ref Guid guid, ushort* searchString, IntPtr something)
        {
            return HResult.Ok;
        }

        private static HResult EndDirectoryEnumerationCallback(ref PRJ_CALLBACK_DATA callbackData, ref Guid guid)
        {
            return HResult.Ok;
        }

        private static HResult GetPlaceholderInfoCallback(ref PRJ_CALLBACK_DATA callbackData)
        {
            return HResult.Ok;
        }

        private static HResult GetFileDataCallback(ref PRJ_CALLBACK_DATA callbackData, ulong someLong, uint someInt)
        {
            return HResult.Ok;
        }

        private static HResult QueryFileNameCallback(ref PRJ_CALLBACK_DATA callbackData)
        {
            return HResult.Ok;
        }

        private static void CancelCommandCallback(ref PRJ_CALLBACK_DATA callbackData)
        {
            
        }

        private static unsafe HResult NotificationCallback(ref PRJ_CALLBACK_DATA callbackData, int isDirectory, PRJ_NOTIFICATION notification, ushort* destinationFileName, PRJ_NOTIFICATION_PARAMETERS* operationParameters)
        {
            return HResult.Ok;
        }
    }
}
