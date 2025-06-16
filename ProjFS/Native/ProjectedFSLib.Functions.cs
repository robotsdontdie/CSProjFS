using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace ProjFS.Native
{
    internal static partial class ProjectedFSLib
    {
        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjallocatealignedbuffer
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial IntPtr PrjAllocateAlignedBuffer(IntPtr namespaceVirtualizationContext, UIntPtr size);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjclearnegativepathcache
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjClearNegativePathCache(IntPtr namespaceVirtualizationContext, out IntPtr totalEntryNumber);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjcompletecommand
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjCompleteCommand(IntPtr namespaceVirtualizationContext, int commandId, HResult completionResult, [Optional] in PRJ_COMPLETE_COMMAND_EXTENDED_PARAMETERS extendedParameters);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjdeletefile
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjDeleteFile(IntPtr namespaceVirtualizationContext, [MarshalAs(UnmanagedType.LPWStr)] string destinationFileName, [Optional] PRJ_UPDATE_TYPES updateFlags, [Optional] out PRJ_UPDATE_FAILURE_CAUSES failureReason);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjdoesnamecontainwildcards
        [LibraryImport("ProjectedFSLib.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool PrjDoesNameContainWildCards([MarshalAs(UnmanagedType.LPWStr)] string fileName);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjfilenamecompare
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial int PrjFileNameCompare([MarshalAs(UnmanagedType.LPWStr)] string fileName1, [MarshalAs(UnmanagedType.LPWStr)] string fileName2);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjfilenamematch
        [LibraryImport("ProjectedFSLib.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool PrjFileNameMatch([MarshalAs(UnmanagedType.LPWStr)] string fileNameToCheck, [MarshalAs(UnmanagedType.LPWStr)] string pattern);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjfilldirentrybuffer
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjFillDirEntryBuffer([MarshalAs(UnmanagedType.LPWStr)] string fileName, [Optional] in PRJ_FILE_BASIC_INFO fileBasicInfo, IntPtr dirEntryBufferHandle);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjfilldirentrybuffer2
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjFillDirEntryBuffer2(IntPtr dirEntryBufferHandle, [MarshalAs(UnmanagedType.LPWStr)] string fileName, [Optional] in PRJ_FILE_BASIC_INFO fileBasicInfo, [Optional, MarshalUsing(typeof(PrjExtendedInfoMarshaller))] in PRJ_EXTENDED_INFO extendedInfo);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjfreealignedbuffer
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial void PrjFreeAlignedBuffer(IntPtr buffer);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjgetondiskfilestate
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjGetOnDiskFileState([MarshalAs(UnmanagedType.LPWStr)] string destinationFileName, out PRJ_FILE_STATE fileState);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjgetvirtualizationinstanceinfo
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjGetVirtualizationInstanceInfo(IntPtr namespaceVirtualizationContext, out PRJ_VIRTUALIZATION_INSTANCE_INFO virtualizationInstanceInfo);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjmarkdirectoryasplaceholder
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjMarkDirectoryAsPlaceholder([MarshalAs(UnmanagedType.LPWStr)] string rootPathName, [MarshalAs(UnmanagedType.LPWStr)] string? targetPathName, [Optional, MarshalUsing(typeof(PrjPlaceholderVersionInfoMarshaller))] in PRJ_PLACEHOLDER_VERSION_INFO versionInfo, ref Guid virtualizationInstanceID);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjmarkdirectoryasplaceholder
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjMarkDirectoryAsPlaceholder([MarshalAs(UnmanagedType.LPWStr)] string rootPathName, [MarshalAs(UnmanagedType.LPWStr)] string? targetPathName, IntPtr versionInfo, ref Guid virtualizationInstanceID);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjmarkdirectoryasplaceholder
        [LibraryImport("ProjectedFSLib.dll")]
        internal static unsafe partial HResult PrjMarkDirectoryAsPlaceholder(ushort* rootPathName, ushort* targetPathName, IntPtr versionInfo, ref Guid virtualizationInstanceID);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjstartvirtualizing
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjStartVirtualizing([MarshalAs(UnmanagedType.LPWStr)] string virtualizationRootPath, ref PRJ_CALLBACKS callbacks, [Optional] IntPtr instanceContext, [Optional] in PRJ_STARTVIRTUALIZING_OPTIONS options, out IntPtr namespaceVirtualizationContext);

        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult MyMethod([MarshalAs(UnmanagedType.LPArray)] int[] ints);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjstartvirtualizing
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjStartVirtualizing([MarshalAs(UnmanagedType.LPWStr)] string virtualizationRootPath, ref PRJ_CALLBACKS callbacks, [Optional] IntPtr instanceContext, [Optional] IntPtr options, out IntPtr namespaceVirtualizationContext);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjstopvirtualizing
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial void PrjStopVirtualizing(IntPtr namespaceVirtualizationContext);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjupdatefileifneeded
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjUpdateFileIfNeeded(IntPtr namespaceVirtualizationContext, [MarshalAs(UnmanagedType.LPWStr)] string destinationFileName, [MarshalUsing(typeof(PrjPlaceholderInfoMarshaller))] in PRJ_PLACEHOLDER_INFO placeholderInfo, uint placeholderInfoSize, [Optional] PRJ_UPDATE_TYPES updateFlags, [Optional] out PRJ_UPDATE_FAILURE_CAUSES failureReason);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjwritefiledata
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjWriteFileData(IntPtr namespaceVirtualizationContext, ref Guid dataStreamId, IntPtr buffer, ulong byteOffset, uint length);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjwriteplaceholderinfo
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjWritePlaceholderInfo(IntPtr namespaceVirtualizationContext, [MarshalAs(UnmanagedType.LPWStr)] string destinationFileName, [MarshalUsing(typeof(PrjPlaceholderInfoMarshaller))] in PRJ_PLACEHOLDER_INFO placeholderInfo, uint placeholderInfoSize);

        // https://learn.microsoft.com/en-us/windows/win32/api/projectedfslib/nf-projectedfslib-prjwriteplaceholderinfo2
        [LibraryImport("ProjectedFSLib.dll")]
        internal static partial HResult PrjWritePlaceholderInfo2(IntPtr namespaceVirtualizationContext, [MarshalAs(UnmanagedType.LPWStr)] string destinationFileName, [MarshalUsing(typeof(PrjPlaceholderInfoMarshaller))] in PRJ_PLACEHOLDER_INFO placeholderInfo, uint placeholderInfoSize, [MarshalUsing(typeof(PrjExtendedInfoMarshaller))] in PRJ_EXTENDED_INFO ExtendedInfo);
    }
}
