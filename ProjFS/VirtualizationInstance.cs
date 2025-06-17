using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using ProjFS;
using static ProjFS.Native.ProjectedFSLib;

namespace ProjFS
{
    public class VirtualizationInstance
    {
        private const int IdLength = (int)PRJ_PLACEHOLDER_ID.PRJ_PLACEHOLDER_ID_LENGTH;

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
        private nint virtualizationContext;

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
                PrjMarkDirectoryAsPlaceholder(virtualizationRootPath, null, versionInfo, virtualizationInstanceID);
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

        public Guid VirtualizationInstanceId
        {
            get
            {
                ConfirmStarted();
                return virtualizationInstanceId;
            }
        }

        public int PlaceholderIdLength => IdLength;

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

            unsafe
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
            }

            var startOptions = new PRJ_STARTVIRTUALIZING_OPTIONS
            {
                Flags = enableNegativePathCache ? PRJ_STARTVIRTUALIZING_FLAGS.PRJ_FLAG_USE_NEGATIVE_PATH_CACHE : PRJ_STARTVIRTUALIZING_FLAGS.PRJ_FLAG_NONE,
                PoolThreadCount = poolThreadCount,
                ConcurrentThreadCount = concurrentThreadCount,
            };

            if (notificationMappings.Count > 0)
            {
                startOptions.NotificationMappings = notificationMappings
                    .Select(m => new PRJ_NOTIFICATION_MAPPING
                    {
                        NotificationBitMask = (PRJ_NOTIFY_TYPES)m.NotificationMask,
                        NotificationRoot = m.NotificationRoot,
                    }).ToArray();
                startOptions.NotificationMappingsCount = (uint)notificationMappings.Count;
            }

            var result = PrjStartVirtualizing(virtualizationRootPath, ref callbacks, instanceContext, startOptions, out virtualizationContext);

            if (result != HResult.Ok)
            {
                return result;
            }

            result = PrjGetVirtualizationInstanceInfo(virtualizationContext, out var virtualizationInstanceInfo);
            if (result != HResult.Ok)
            {
                StopVirtualizing();
                return result;
            }

            virtualizationInstanceId = virtualizationInstanceInfo.InstanceID;

            return HResult.Ok;
        }

        public void StopVirtualizing()
        {
            if (virtualizationContext == 0)
            {
                return;
            }

            PrjStopVirtualizing(virtualizationContext);
        }

        public HResult ClearNegativePathCache(out uint totalEntryNumber)
        {
            HResult result = PrjClearNegativePathCache(virtualizationContext, out totalEntryNumber);
            return result;
        }

        public HResult WriteFileData(
            Guid dataStreamId,
            IWriteBuffer buffer,
            ulong byteOffset,
            uint length)
        {
            if (buffer is null)
            {
                return HResult.InvalidArg;
            }

            unsafe
            {
                return PrjWriteFileData(virtualizationContext,
                    dataStreamId,
                    buffer.Pointer.ToPointer(),
                    byteOffset,
                    length);
            }
        }

        public HResult DeleteFile(
            string relativePath,
            UpdateType updateFlags,
            out UpdateFailureCause failureReason)
        {
            PRJ_UPDATE_FAILURE_CAUSES deleteFailureReason = PRJ_UPDATE_FAILURE_CAUSES.PRJ_UPDATE_FAILURE_CAUSE_NONE;
            HResult result = PrjDeleteFile(
                virtualizationContext,
                relativePath,
                (PRJ_UPDATE_TYPES)updateFlags,
                out deleteFailureReason);

            failureReason = (UpdateFailureCause)deleteFailureReason;
            return result;
        }

        public HResult WritePlaceholderInfo(
            string relativePath,
            DateTime creationTime,
            DateTime lastAccessTime,
            DateTime lastWriteTime,
            DateTime changeTime,
            FileAttributes fileAttributes,
            long endOfFile,
            bool isDirectory,
            byte[] contentId,
            byte[] providerId)
        {
            if (relativePath is null)
            {
                return HResult.InvalidArg;
            }

            var placeholderInfo = CreatePlaceholderInfo(
                creationTime,
                lastAccessTime,
                lastWriteTime,
                changeTime,
                fileAttributes,
                endOfFile,
                isDirectory,
                contentId,
                providerId);

            return PrjWritePlaceholderInfo(
                    virtualizationContext,
                    relativePath,
                    placeholderInfo,
                    (uint)placeholderInfo.Size);
        }

        public HResult WritePlaceholderInfo2(
            string relativePath,
            DateTime creationTime,
            DateTime lastAccessTime,
            DateTime lastWriteTime,
            DateTime changeTime,
            FileAttributes fileAttributes,
            long endOfFile,
            bool isDirectory,
            string symlinkTargetOrNull,
            byte[] contentId,
            byte[] providerId)
        {
            if (relativePath is null)
            {
                return HResult.InvalidArg;
            }

            var placeholderInfo = CreatePlaceholderInfo(
                creationTime,
                lastAccessTime,
                lastWriteTime,
                changeTime,
                fileAttributes,
                endOfFile,
                isDirectory,
                contentId,
                providerId);

            if (symlinkTargetOrNull is null)
            {
                return PrjWritePlaceholderInfo(
                    virtualizationContext,
                    relativePath,
                    placeholderInfo,
                    (uint)placeholderInfo.Size);
            }

            var extendedInfo = new PRJ_EXTENDED_INFO
            {
                InfoType = PRJ_EXT_INFO_TYPE.PRJ_EXT_INFO_TYPE_SYMLINK,
                TargetName = symlinkTargetOrNull,
            };

            return PrjWritePlaceholderInfo2(
                virtualizationContext,
                relativePath,
                placeholderInfo,
                (uint)placeholderInfo.Size,
                extendedInfo);
        }

        public HResult UpdateFileIfNeeded(
            string relativePath,
            DateTime creationTime,
            DateTime lastAccessTime,
            DateTime lastWriteTime,
            DateTime changeTime,
            FileAttributes fileAttributes,
            long endOfFile,
            byte[] contentId,
            byte[] providerId,
            UpdateType updateFlags,
            out UpdateFailureCause failureReason)
        {
            HResult result;
            
            var placeholderInfo = CreatePlaceholderInfo(creationTime,
                lastAccessTime,
                lastWriteTime,
                changeTime,
                fileAttributes,
                endOfFile,
                isDirectory: false,
                contentId,
                providerId);

            var updateFailureCause = PRJ_UPDATE_FAILURE_CAUSES.PRJ_UPDATE_FAILURE_CAUSE_NONE;
            result = PrjUpdateFileIfNeeded(
                virtualizationContext,
                relativePath,
                placeholderInfo,
                (uint)placeholderInfo.Size,
                (PRJ_UPDATE_TYPES)updateFlags,
                out updateFailureCause);

            failureReason = (UpdateFailureCause)updateFailureCause;
            return result;
        }

        public HResult CompleteCommand(int commandId)
        {
            return CompleteCommand(commandId, HResult.Ok);
        }

        public HResult CompleteCommand(
            int commandId,
            HResult completionResult)
        {
            return PrjCompleteCommand(
                virtualizationContext,
                commandId,
                completionResult,
                IntPtr.Zero);
        }

        public HResult CompleteCommand(
            int commandId,
            IDirectoryEnumerationResults results)
        {
            var extendedParams = new PRJ_COMPLETE_COMMAND_EXTENDED_PARAMETERS_ENUMERATION();

            extendedParams.CommandType = PRJ_COMPLETE_COMMAND_TYPE.PRJ_COMPLETE_COMMAND_TYPE_ENUMERATION;
            // results has to be a concrete DirectoryEnumerationResults.
            extendedParams.DirEntryBufferHandle = results.DirEntryBufferHandle;

            return PrjCompleteCommand(
                virtualizationContext,
                commandId,
                HResult.Ok,
                extendedParams);
        }

        public HResult CompleteCommand(
            int commandId,
            NotificationType newNotificationMask)
        {
            var extendedParams = new PRJ_COMPLETE_COMMAND_EXTENDED_PARAMETERS_NOTIFICATION();

            extendedParams.CommandType = PRJ_COMPLETE_COMMAND_TYPE.PRJ_COMPLETE_COMMAND_TYPE_NOTIFICATION;
            extendedParams.NotificationMask = (PRJ_NOTIFY_TYPES)newNotificationMask;

            return PrjCompleteCommand(
                virtualizationContext,
                commandId,
                HResult.Ok,
                extendedParams);
        }

        public IWriteBuffer CreateWriteBuffer(uint desiredBufferSize)
        {
            return new WriteBuffer(desiredBufferSize, virtualizationContext);
        }

        public IWriteBuffer CreateWriteBuffer(
            ulong byteOffset,
            uint length,
            out ulong alignedByteOffset,
            out uint alignedLength)
        {
            // Get the sector size so we can compute the aligned versions of byteOffset and length to return
            // to the user.  If we're on Windows 10 version 1803 the sector size is stored on the class.
            // Otherwise it's available from the namespace virtualization context.
            ulong bytesPerSector;

            var instanceInfo = new PRJ_VIRTUALIZATION_INSTANCE_INFO();
            var result = PrjGetVirtualizationInstanceInfo(virtualizationContext, out instanceInfo);

            if (result != HResult.Ok)
            {
                throw new InvalidOperationException($"Failed to retrieve virtualization instance info for directory {virtualizationRootPath}.", Marshal.GetExceptionForHR((int)result));
            }

            bytesPerSector = instanceInfo.WriteAlignment;
            
            // alignedByteOffset is byteOffset, rounded down to the nearest bytesPerSector boundary.
            alignedByteOffset = byteOffset & (0 - bytesPerSector);

            // alignedLength is the end offset of the requested range, rounded up to the nearest bytesPerSector
            // boundary.
            ulong rangeEndOffset = byteOffset + length;
            ulong alignedRangeEndOffset = (rangeEndOffset + (bytesPerSector - 1)) & (0 - bytesPerSector);
            alignedLength = (uint)(alignedRangeEndOffset - alignedByteOffset);

            // Now that we've got the adjusted length, create the buffer itself.
            return CreateWriteBuffer(alignedLength);
        }

        public HResult MarkDirectoryAsPlaceholder(
            string targetDirectoryPath,
            byte[] contentId,
            byte[] providerId)
        {
            HResult hr = PrjGetVirtualizationInstanceInfo(virtualizationContext, out var instanceInfo);

            if (hr == HResult.Ok)
            {
                var versionInfo = new PRJ_PLACEHOLDER_VERSION_INFO
                {
                    ProviderID = providerId,
                    ContentID = contentId,
                };

                hr = PrjMarkDirectoryAsPlaceholder(
                    virtualizationRootPath,
                    targetDirectoryPath,
                    versionInfo,
                    instanceInfo.InstanceID);
            }

            return hr;
        }

        public static HResult MarkDirectoryAsVirtualizationRoot(
            string rootPath,
            Guid virtualizationInstanceGuid)
        {
            var versionInfo = new PRJ_PLACEHOLDER_VERSION_INFO();
            return PrjMarkDirectoryAsPlaceholder(
                rootPath,
                null,
                versionInfo,
                virtualizationInstanceGuid);
            
        }
        
        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nc-projectedfslib-prj_start_directory_enumeration_cb
        private static HResult StartDirectoryEnumerationCallback(ref PRJ_CALLBACK_DATA callbackData, ref Guid enumerationId)
        {
            if (!TryGetInstanceCallbacks(callbackData, out _, out var callbacks))
            {
                return HResult.InternalError;
            }

            unsafe
            {
                string? filePathName = Utf16StringMarshaller.ConvertToManaged(callbackData.FilePathName);
                if (filePathName is null)
                {
                    return HResult.InvalidArg;
                }

                string? triggeringProcessFilename = Utf16StringMarshaller.ConvertToManaged(callbackData.TriggeringProcessImageFileName);

                return callbacks.StartDirectoryEnumerationCallback(
                    callbackData.CommandId,
                    enumerationId,
                    filePathName,
                    callbackData.TriggeringProcessId,
                    triggeringProcessFilename);
            }
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nc-projectedfslib-prj_get_directory_enumeration_cb
        private static unsafe HResult GetDirectoryEnumerationCallback(ref PRJ_CALLBACK_DATA callbackData, ref Guid enumerationId, ushort* searchString, IntPtr dirEntryBufferHandle)
        {

            if (!TryGetInstanceCallbacks(callbackData, out _, out var callbacks))
            {
                return HResult.InternalError;
            }

            unsafe
            {
                string? managedSearchString = Utf16StringMarshaller.ConvertToManaged(searchString);

                return callbacks.GetDirectoryEnumerationCallback(
                    callbackData.CommandId,
                    enumerationId,
                    managedSearchString,
                    callbackData.Flags.HasFlag(PRJ_CALLBACK_DATA_FLAGS.PRJ_CB_DATA_FLAG_ENUM_RESTART_SCAN),
                    new DirectoryEnumerationResults(dirEntryBufferHandle));
            }
        }

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nc-projectedfslib-prj_end_directory_enumeration_cb
        private static HResult EndDirectoryEnumerationCallback(ref PRJ_CALLBACK_DATA callbackData, ref Guid enumerationId)
        {
            if (!TryGetInstanceCallbacks(callbackData, out _, out var callbacks))
            {
                return HResult.InternalError;
            }

            return callbacks.EndDirectoryEnumerationCallback(enumerationId);
        }

        private static HResult GetPlaceholderInfoCallback(ref PRJ_CALLBACK_DATA callbackData)
        {
            if (!TryGetInstanceCallbacks(callbackData, out _, out var callbacks))
            {
                return HResult.InternalError;
            }

            unsafe
            {
                string? filePathName = Utf16StringMarshaller.ConvertToManaged(callbackData.FilePathName);
                if (filePathName is null)
                {
                    return HResult.InvalidArg;
                }

                string? triggeringProcessFilename = Utf16StringMarshaller.ConvertToManaged(callbackData.TriggeringProcessImageFileName);

                return callbacks.GetPlaceholderInfoCallback(
                    callbackData.CommandId,
                    filePathName,
                    callbackData.TriggeringProcessId,
                    triggeringProcessFilename);
            }
        }

        private static HResult GetFileDataCallback(ref PRJ_CALLBACK_DATA callbackData, ulong byteOffset, uint length)
        {
            if (!TryGetInstanceCallbacks(callbackData, out _, out var callbacks))
            {
                return HResult.InternalError;
            }

            unsafe
            {
                string? filePathName = Utf16StringMarshaller.ConvertToManaged(callbackData.FilePathName);
                if (filePathName is null)
                {
                    return HResult.InvalidArg;
                }

                string? triggeringProcessFilename = Utf16StringMarshaller.ConvertToManaged(callbackData.TriggeringProcessImageFileName);

                var providerId = new byte[IdLength];
                Marshal.Copy(callbackData.VersionInfo, providerId, 0, IdLength);

                var contentId = new byte[IdLength];
                Marshal.Copy(callbackData.VersionInfo + IdLength, contentId, 0, IdLength);



                return callbacks.GetFileDataCallback(
                    callbackData.CommandId,
                    filePathName,
                    byteOffset,
                    length,
                    callbackData.DataStreamId,
                    contentId,
                    providerId,
                    callbackData.TriggeringProcessId,
                    triggeringProcessFilename);
            }
        }

        private static HResult QueryFileNameCallback(ref PRJ_CALLBACK_DATA callbackData)
        {
            if (!TryGetInstanceCallbacks(callbackData, out var instance, out _))
            {
                return HResult.InternalError;
            }

            if (instance.OnQueryFileName is not QueryFileNameCallback callback)
            {
                return HResult.InternalError;
            }

            unsafe
            {
                string? filePathName = Utf16StringMarshaller.ConvertToManaged(callbackData.FilePathName);
                if (filePathName is null)
                {
                    return HResult.InvalidArg;
                }

                return callback(filePathName);
            }
        }

        private static void CancelCommandCallback(ref PRJ_CALLBACK_DATA callbackData)
        {
            if (!TryGetInstanceCallbacks(callbackData, out var instance, out _))
            {
                return;
            }

            if (instance.OnCancelCommand is not CancelCommandCallback callback)
            {
                return;
            }

            callback(callbackData.CommandId);
        }

        private static unsafe HResult NotificationCallback(ref PRJ_CALLBACK_DATA callbackData, int isDirectory, PRJ_NOTIFICATION notification, ushort* destinationFileName, PRJ_NOTIFICATION_PARAMETERS* notificationParameters)
        {
            if (!TryGetInstanceCallbacks(callbackData, out var instance, out _))
            {
                return HResult.InternalError;
            }

            string? filePathName = Utf16StringMarshaller.ConvertToManaged(callbackData.FilePathName);
            if (filePathName is null)
            {
                return HResult.InvalidArg;
            }

            string? destinationPath = Utf16StringMarshaller.ConvertToManaged(destinationFileName);

            string? triggeringProcessFilename = Utf16StringMarshaller.ConvertToManaged(callbackData.TriggeringProcessImageFileName);

            var result = HResult.Ok;
            NotificationType notificationMask = 0;
            bool? allow = null;

            switch (notification)
            {
                case PRJ_NOTIFICATION.PRJ_NOTIFICATION_FILE_OPENED:
                    allow = instance.OnNotifyFileOpened?.Invoke(
                        filePathName,
                        isDirectory != 0,
                        callbackData.TriggeringProcessId,
                        triggeringProcessFilename,
                        out notificationMask);

                    if (allow.HasValue)
                    {
                        if (allow.Value)
                        {
                            notificationParameters->Data = (uint)notificationMask;
                        }
                        else
                        {
                            result = HResult.AccessDenied;
                        }
                    }

                    break;
                case PRJ_NOTIFICATION.PRJ_NOTIFICATION_NEW_FILE_CREATED:
                    instance.OnNotifyNewFileCreated?.Invoke(
                        filePathName,
                        isDirectory != 0,
                        callbackData.TriggeringProcessId,
                        triggeringProcessFilename,
                        out notificationMask);
                    notificationParameters->Data = (uint)notificationMask;
                    break;
                case PRJ_NOTIFICATION.PRJ_NOTIFICATION_FILE_OVERWRITTEN:
                    instance.OnNotifyFileOverwritten?.Invoke(
                        filePathName,
                        isDirectory != 0,
                        callbackData.TriggeringProcessId,
                        triggeringProcessFilename,
                        out notificationMask);
                    notificationParameters->Data = (uint)notificationMask;
                    break;
                case PRJ_NOTIFICATION.PRJ_NOTIFICATION_PRE_DELETE:
                    allow = instance.OnNotifyPreDelete?.Invoke(
                        filePathName,
                        isDirectory != 0,
                        callbackData.TriggeringProcessId,
                        triggeringProcessFilename);
                    if (allow.HasValue && !allow.Value)
                    {
                        result = HResult.CannotDelete;
                    }
                    break;
                case PRJ_NOTIFICATION.PRJ_NOTIFICATION_PRE_RENAME:
                    if (destinationPath is null)
                    {
                        return HResult.InvalidArg;
                    }

                    allow = instance.OnNotifyPreRename?.Invoke(
                        filePathName,
                        destinationPath,
                        callbackData.TriggeringProcessId,
                        triggeringProcessFilename);
                    if (allow.HasValue && !allow.Value)
                    {
                        result = HResult.AccessDenied;
                    }
                    break;
                case PRJ_NOTIFICATION.PRJ_NOTIFICATION_PRE_SET_HARDLINK:
                    if (destinationPath is null)
                    {
                        return HResult.InvalidArg;
                    }

                    allow = instance.OnNotifyPreCreateHardlink?.Invoke(
                        filePathName,
                        destinationPath,
                        callbackData.TriggeringProcessId,
                        triggeringProcessFilename);
                    if (allow.HasValue && !allow.Value)
                    {
                        result = HResult.AccessDenied;
                    }
                    break;
                case PRJ_NOTIFICATION.PRJ_NOTIFICATION_FILE_RENAMED:
                    if (destinationPath is null)
                    {
                        return HResult.InvalidArg;
                    }

                    instance.OnNotifyFileRenamed?.Invoke(
                        filePathName,
                        destinationPath,
                        isDirectory != 0,
                        callbackData.TriggeringProcessId,
                        triggeringProcessFilename,
                        out notificationMask);
                    notificationParameters->Data = (uint)notificationMask;
                    break;
                case PRJ_NOTIFICATION.PRJ_NOTIFICATION_HARDLINK_CREATED:
                    if (destinationPath is null)
                    {
                        return HResult.InvalidArg;
                    }

                    instance.OnNotifyHardlinkCreated?.Invoke(
                        filePathName,
                        destinationPath,
                        callbackData.TriggeringProcessId,
                        triggeringProcessFilename);
                    break;
                case PRJ_NOTIFICATION.PRJ_NOTIFICATION_FILE_HANDLE_CLOSED_NO_MODIFICATION:
                    instance.OnNotifyFileHandleClosedNoModification?.Invoke(
                        filePathName,
                        isDirectory != 0,
                        callbackData.TriggeringProcessId,
                        triggeringProcessFilename);
                    break;
                case PRJ_NOTIFICATION.PRJ_NOTIFICATION_FILE_HANDLE_CLOSED_FILE_MODIFIED:
                    instance.OnNotifyFileHandleClosedFileModifiedOrDeleted?.Invoke(
                        filePathName,
                        isDirectory != 0,
                        isFileModified: true,
                        isFileDeleted: false,
                        callbackData.TriggeringProcessId,
                        triggeringProcessFilename);
                    break;
                case PRJ_NOTIFICATION.PRJ_NOTIFICATION_FILE_HANDLE_CLOSED_FILE_DELETED:
                    instance.OnNotifyFileHandleClosedFileModifiedOrDeleted?.Invoke(
                        filePathName,
                        isDirectory != 0,
                        isFileModified: notificationParameters->Data != 0,
                        isFileDeleted: true,
                        callbackData.TriggeringProcessId,
                        triggeringProcessFilename);
                    break;
                case PRJ_NOTIFICATION.PRJ_NOTIFICATION_FILE_PRE_CONVERT_TO_FULL:
                    allow = instance.OnNotifyFilePreConvertToFull?.Invoke(
                        filePathName,
                        callbackData.TriggeringProcessId,
                        triggeringProcessFilename);
                    if (allow.HasValue && !allow.Value)
                    {
                        result = HResult.AccessDenied;
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        private static bool TryGetInstanceCallbacks(
            PRJ_CALLBACK_DATA callbackData,
            [NotNullWhen(true)] out VirtualizationInstance? virtualizationInstance,
            [NotNullWhen(true)] out IRequiredCallbacks? callbacks)
        {
            int instanceContext = (int)callbackData.InstanceContext;
            if (!instances.TryGetValue(instanceContext, out virtualizationInstance))
            {
                callbacks = null;
                return false;
            }

            callbacks = virtualizationInstance.RequiredCallbacks;
            return callbacks is not null;
        }

        private static PRJ_PLACEHOLDER_INFO CreatePlaceholderInfo(
            DateTime creationTime,
            DateTime lastAccessTime,
            DateTime lastWriteTime,
            DateTime changeTime,
            FileAttributes fileAttributes,
            long endOfFile,
            bool isDirectory,
            byte[] contentId,
            byte[] providerId)
        {
            return new PRJ_PLACEHOLDER_INFO
            {
                FileBasicInfo = new PRJ_FILE_BASIC_INFO
                {
                    FileSize = isDirectory ? 0 : endOfFile,
                    IsDirectory = isDirectory,
                    FileAttributes = (uint)fileAttributes,
                    CreationTime = creationTime.ToFileTime(),
                    LastAccessTime = lastAccessTime.ToFileTime(),
                    LastWriteTime = lastWriteTime.ToFileTime(),
                    ChangeTime = changeTime.ToFileTime(),
                },
                EaBufferSize = 0,
                OffsetToFirstEa = 0,
                SecurityBufferSize = 0,
                OffsetToSecurityDescriptor = 0,
                StreamsInfoBufferSize = 0,
                OffsetToFirstStreamInfo = 0,
                VersionInfo = new PRJ_PLACEHOLDER_VERSION_INFO
                {
                    ContentID = contentId,
                    ProviderID = providerId,
                },
            };
        }

        private void ConfirmStarted()
        {
            if (virtualizationContext == 0)
            {
                throw new InvalidOperationException("Operation invalid before virtualization instance is started");
            }
        }

        private void ConfirmNotStarted()
        {
            if (virtualizationContext != 0)
            {
                throw new InvalidOperationException("Operation invalid after virtualization instance is started");
            }
        }
    }
}
