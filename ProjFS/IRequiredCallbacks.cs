using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjFS
{
    //
    // Summary:
    //     Defines callbacks that a provider is required to implement.
    //
    // Remarks:
    //     A provider must implement the methods in this class to supply basic file system
    //     functionality. The provider passes a reference to its implementation in the Microsoft.Windows.ProjFS.StartVirtualizing
    //     method.
    public interface IRequiredCallbacks
    {
        //
        // Summary:
        //     Informs the provider that a directory enumeration is starting.
        //
        // Parameters:
        //   commandId:
        //     A value that uniquely identifies an invocation of the callback.
        //
        //     If the provider returns Microsoft.Windows.ProjFS.HResult.Pending from this method,
        //     then it must pass this value to ProjFS.VirtualizationInstance.CompleteCommand
        //     to signal that it has finished processing this invocation of the callback.
        //
        //   enumerationId:
        //     Identifies this enumeration session.
        //
        //   relativePath:
        //     Identifies the directory to be enumerated. The path is specified relative to
        //     the virtualization root.
        //
        //   triggeringProcessId:
        //     The PID of the process that triggered this callback. If this information is not
        //     available, this will be 0.
        //
        //   triggeringProcessImageFileName:
        //     The image file name corresponding to triggeringProcessId. If triggeringProcessId
        //     is 0 this will be null.
        //
        // Returns:
        //     Microsoft.Windows.ProjFS.HResult.Ok if the provider successfully completes the
        //     operation.
        //
        //     Microsoft.Windows.ProjFS.HResult.Pending if the provider wishes to complete the
        //     operation at a later time.
        //
        //     An appropriate error code if the provider fails the operation.
        //
        // Remarks:
        //     ProjFS requests a directory enumeration from the provider by first invoking this
        //     callback, then the Microsoft.Windows.ProjFS.IRequiredCallbacks.GetDirectoryEnumerationCallback(System.Int32,System.Guid,System.String,System.Boolean,Microsoft.Windows.ProjFS.IDirectoryEnumerationResults)
        //     callback one or more times, then the Microsoft.Windows.ProjFS.IRequiredCallbacks.EndDirectoryEnumerationCallback(System.Guid)
        //     callback. Because multiple enumerations may occur in parallel in the same location,
        //     ProjFS uses the enumerationId argument to associate the callback invocations
        //     into a single enumeration, meaning that a given set of calls to the enumeration
        //     callbacks will use the same value for enumerationId for the same session.
        HResult StartDirectoryEnumerationCallback(int commandId, Guid enumerationId, string relativePath, uint triggeringProcessId, string? triggeringProcessImageFileName);

        //
        // Summary:
        //     Requests directory enumeration information from the provider.
        //
        // Parameters:
        //   commandId:
        //     A value that uniquely identifies an invocation of the callback.
        //
        //     If the provider returns Microsoft.Windows.ProjFS.HResult.Pending from this method,
        //     then it must pass this value to ProjFS.VirtualizationInstance.CompleteCommand
        //     to signal that it has finished processing this invocation of the callback.
        //
        //   enumerationId:
        //     Identifies this enumeration session.
        //
        //   filterFileName:
        //     An optional string specifying a search expression. This parameter may be null.
        //
        //
        //     The search expression may include wildcard characters. The provider should use
        //     the ProjFS.Utils.DoesNameContainWildCards method routine to determine whether
        //     wildcards are present in the search expression. The provider should use the ProjFS.Utils.IsFileNameMatch
        //     method to determine whether a directory entry in its store matches the search
        //     expression.
        //
        //     If this parameter is not null, only files whose names match the search expression
        //     should be included in the directory scan.
        //
        //     If this parameter is null, all entries in the directory must be included.
        //
        //   restartScan:
        //     true if the scan is to start at the first entry in the directory.
        //
        //     false if resuming the scan from a previous call.
        //
        //     On the first invocation of this callback for an enumeration session the provider
        //     must treat this as true, regardless of its value (i.e. all enumerations must
        //     start at the first entry). On subsequent invocations of this callback the provider
        //     must honor this value.
        //
        //   result:
        //     Receives the results of the enumeration from the provider.
        //
        //     The provider uses one of the Microsoft.Windows.ProjFS.IDirectoryEnumerationResults::Add
        //     methods of this object to provide the enumeration results.
        //
        //     If the provider returns Microsoft.Windows.ProjFS.HResult.Pending from this method,
        //     then it must pass this value to ProjFS.VirtualizationInstance.CompleteCommand
        //     to provide the enumeration results.
        //
        // Returns:
        //     Microsoft.Windows.ProjFS.HResult.Ok if the provider successfully completes the
        //     operation.
        //
        //     Microsoft.Windows.ProjFS.HResult.Pending if the provider wishes to complete the
        //     operation at a later time.
        //
        //     Microsoft.Windows.ProjFS.HResult.InsufficientBuffer if result.Add returned Microsoft.Windows.ProjFS.HResult.InsufficientBuffer
        //     for the first matching file or directory in the enumeration.
        //
        //     An appropriate error code if the provider fails the operation.
        //
        // Remarks:
        //     ProjFS requests a directory enumeration from the provider by first invoking Microsoft.Windows.ProjFS.IRequiredCallbacks.StartDirectoryEnumerationCallback(System.Int32,System.Guid,System.String,System.UInt32,System.String),
        //     then this callback one or more times, then the Microsoft.Windows.ProjFS.IRequiredCallbacks.EndDirectoryEnumerationCallback(System.Guid)
        //     callback. Because multiple enumerations may occur in parallel in the same location,
        //     ProjFS uses the enumerationId argument to associate the callback invocations
        //     into a single enumeration, meaning that a given set of calls to the enumeration
        //     callbacks will use the same value for enumerationId for the same session.
        //
        //     The provider must store the value of filterFileName across calls to this callback.
        //     The provider replaces the value of filterFileName if restartScan in a subsequent
        //     invocation of the callback is true
        //
        //     If no entries match the search expression specified in filterFileName, or if
        //     all the entries in the directory were added in a previous invocation of this
        //     callback, the provider must return Microsoft.Windows.ProjFS.HResult.Ok.
        HResult GetDirectoryEnumerationCallback(int commandId, Guid enumerationId, string? filterFileName, [MarshalAs(UnmanagedType.U1)] bool restartScan, IDirectoryEnumerationResults result);

        //
        // Summary:
        //     Informs the provider that a directory enumeration is over.
        //
        // Parameters:
        //   enumerationId:
        //     Identifies this enumeration session.
        //
        // Returns:
        //     Microsoft.Windows.ProjFS.HResult.Ok if the provider successfully completes the
        //     operation.
        //
        //     An appropriate error code if the provider fails the operation.
        //
        // Remarks:
        //     ProjFS requests a directory enumeration from the provider by first invoking Microsoft.Windows.ProjFS.IRequiredCallbacks.StartDirectoryEnumerationCallback(System.Int32,System.Guid,System.String,System.UInt32,System.String),
        //     then the Microsoft.Windows.ProjFS.IRequiredCallbacks.GetDirectoryEnumerationCallback(System.Int32,System.Guid,System.String,System.Boolean,Microsoft.Windows.ProjFS.IDirectoryEnumerationResults)
        //     callback one or more times, then this callback. Because multiple enumerations
        //     may occur in parallel in the same location, ProjFS uses the enumerationId argument
        //     to associate the callback invocations into a single enumeration, meaning that
        //     a given set of calls to the enumeration callbacks will use the same value for
        //     enumerationId for the same session.
        HResult EndDirectoryEnumerationCallback(Guid enumerationId);

        //
        // Summary:
        //     Requests metadata information for a file or directory from the provider.
        //
        // Parameters:
        //   commandId:
        //     A value that uniquely identifies an invocation of the callback.
        //
        //     If the provider returns Microsoft.Windows.ProjFS.HResult.Pending from this method,
        //     then it must pass this value to ProjFS.VirtualizationInstance.CompleteCommand
        //     to signal that it has finished processing this invocation of the callback.
        //
        //   relativePath:
        //     The path, relative to the virtualization root, of the file for which to return
        //     information.
        //
        //   triggeringProcessId:
        //     The PID for the process that triggered this callback. If this information is
        //     not available, this will be 0.
        //
        //   triggeringProcessImageFileName:
        //     The image file name corresponding to triggeringProcessId. If triggeringProcessId
        //     is 0 this will be null.
        //
        // Returns:
        //     Microsoft.Windows.ProjFS.HResult.Ok if the file exists in the provider's store
        //     and it successfully gave the file's information to ProjFS.
        //
        //     Microsoft.Windows.ProjFS.HResult.FileNotFound if relativePath does not exist
        //     in the provider's store.
        //
        //     Microsoft.Windows.ProjFS.HResult.Pending if the provider wishes to complete the
        //     operation at a later time.
        //
        //     An appropriate error code if the provider fails the operation.
        //
        // Remarks:
        //     ProjFS uses the information the provider provides in this callback to create
        //     a placeholder for the requested item.
        //
        //     To handle this callback, the provider typically calls ProjFS.VirtualizationInstance.WritePlaceholderInfo
        //     to give ProjFS the information for the requested file name. Then the provider
        //     completes the callback.
        HResult GetPlaceholderInfoCallback(int commandId, string relativePath, uint triggeringProcessId, string? triggeringProcessImageFileName);

        //
        // Summary:
        //     Requests the contents of a file's primary data stream.
        //
        // Parameters:
        //   commandId:
        //     A value that uniquely identifies an invocation of the callback.
        //
        //     If the provider returns Microsoft.Windows.ProjFS.HResult.Pending from this method,
        //     then it must pass this value to ProjFS.VirtualizationInstance.CompleteCommand
        //     to signal that it has finished processing this invocation of the callback.
        //
        //   relativePath:
        //     The path, relative to the virtualization root, of the file for which to provide
        //     data.
        //
        //   byteOffset:
        //     Offset in bytes from the beginning of the file at which the provider must start
        //     returning data. The provider must return file data starting at or before this
        //     offset.
        //
        //   length:
        //     Number of bytes of file data requested. The provider must return at least this
        //     many bytes of file data beginning at byteOffset.
        //
        //   dataStreamId:
        //     The unique value to associate with this file stream. The provider must pass this
        //     value to ProjFS.VirtualizationInstance.WriteFile when providing file data as
        //     part of handling this callback.
        //
        //   contentId:
        //     The contentId value specified by the provider when it created the placeholder
        //     for this file. See ProjFS.VirtualizationInstance.WritePlaceholderInfo.
        //
        //   providerId:
        //     The providerId value specified by the provider when it created the placeholder
        //     for this file. See ProjFS.VirtualizationInstance.WritePlaceholderInfo.
        //
        //   triggeringProcessId:
        //     The PID for the process that triggered this callback. If this information is
        //     not available, this will be 0.
        //
        //   triggeringProcessImageFileName:
        //     The image file name corresponding to triggeringProcessId. If triggeringProcessId
        //     is 0 this will be null.
        //
        // Returns:
        //     Microsoft.Windows.ProjFS.HResult.Ok if the provider successfully wrote all the
        //     requested data.
        //
        //     Microsoft.Windows.ProjFS.HResult.Pending if the provider wishes to complete the
        //     operation at a later time.
        //
        //     An appropriate error code if the provider fails the operation.
        //
        // Remarks:
        //     ProjFS uses the data the provider provides in this callback to convert the file
        //     into a hydrated placeholder.
        //
        //     To handle this callback, the provider issues one or more calls to ProjFS.VirtualizationInstance.WriteFile
        //     to give ProjFS the contents of the file's primary data stream. Then the provider
        //     completes the callback.
        HResult GetFileDataCallback(int commandId, string relativePath, ulong byteOffset, uint length, Guid dataStreamId, byte[] contentId, byte[] providerId, uint triggeringProcessId, string? triggeringProcessImageFileName);
    }
}
