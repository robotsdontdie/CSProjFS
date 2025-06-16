using System.Runtime.CompilerServices;

namespace ProjFS
{
    //
    // Summary:
    //     HRESULT values that ProjFS may report to a provider, or that a provider may return
    //     to ProjFS.
    //
    // Remarks:
    //     .NET methods normally do not return error codes, preferring to throw exceptions.
    //     For the most part this API does not throw exceptions, preferring instead to return
    //     error codes. We do this for few reasons:
    //
    //     • This API is a relatively thin wrapper around a native API that itself returns
    //     HRESULT codes. This managed library would have to translate those error codes
    //     into exceptions to throw.
    //     • Errors that a provider returns are sent through the file system, back to the
    //     user who is performing the I/O. If the provider callbacks threw exceptions, the
    //     managed library would just have to catch them and turn them into HRESULT codes.
    //
    //     • If the API methods described here threw exceptions, either the provider would
    //     have to catch them and turn them into error codes to return from its callbacks,
    //     or it would allow those exceptions to propagate and this managed library would
    //     still have to deal with them as described in the preceding bullet.
    //
    //     So rather than deal with the overhead of exceptions just to try to conform to
    //     .NET conventions, this API largely dispenses with them and uses HRESULT codes.
    //
    //
    //     Note that for the convenience of C# developers the VirtualizationInstance::CreateWriteBuffer
    //     method does throw System::OutOfMemoryException if it cannot allocate the buffer.
    //     This makes the method convenient to use with the using keyword.
    //
    //     Note that when HRESULT codes returned from the provider are sent to the file
    //     system, the ProjFS library translates them into NTSTATUS codes. Because there
    //     is not a 1-to-1 mapping of HRESULT codes to NTSTATUS codes, the set of HRESULT
    //     codes that a provider is allowed to return is necessarily constrained.
    //
    //     A provider's IRequiredCallbacks method and On... delegate implementations may
    //     return any HResult value returned from a VirtualizationInstance, as well as the
    //     following HResult values:
    //
    //     • Microsoft.Windows.ProjFS.HResult.Ok
    //     • Microsoft.Windows.ProjFS.HResult.Pending
    //     • Microsoft.Windows.ProjFS.HResult.OutOfMemory
    //     • Microsoft.Windows.ProjFS.HResult.InsufficientBuffer
    //     • Microsoft.Windows.ProjFS.HResult.FileNotFound
    //     • Microsoft.Windows.ProjFS.HResult.VirtualizationUnavaliable
    //     • Microsoft.Windows.ProjFS.HResult.InternalError
    //
    //     The remaining values in the HResult enum may be returned to a provider from ProjFS
    //     APIs and are primarily intended to communicate information to the provider. As
    //     noted above, if such a value is returned to a provider in its implementation
    //     of a callback or delegate, it may return the value to ProjFS.
    public enum HResult : int
    {
        //
        // Summary:
        //     Success.
        Ok = 0,
        //
        // Summary:
        //     The data necessary to complete this operation is not yet available.
        Pending = -2147023899,
        //
        // Summary:
        //     Ran out of memory.
        OutOfMemory = -2147024882,
        //
        // Summary:
        //     The data area passed to a system call is too small.
        InsufficientBuffer = -2147024774,
        //
        // Summary:
        //     The system cannot find the file specified.
        FileNotFound = -2147024894,
        //
        // Summary:
        //     The provider that supports file system virtualization is temporarily unavailable.
        VirtualizationUnavaliable = -2147024527,
        //
        // Summary:
        //     The provider is in an invalid state that prevents it from servicing the callback
        //     (only use this if none of the other error codes is a better match).
        InternalError = -2147023537,
        //
        // Summary:
        //     An attempt was made to perform an initialization operation when initialization
        //     has already been completed.
        AlreadyInitialized = -2147023649,
        //
        // Summary:
        //     Access is denied.
        AccessDenied = -2147024891,
        //
        // Summary:
        //     An attempt has been made to remove a file or directory that cannot be deleted.
        CannotDelete = -805306079,
        //
        // Summary:
        //     The directory name is invalid (it may not be a directory).
        Directory = -2147024629,
        //
        // Summary:
        //     The directory is not empty.
        DirNotEmpty = -2147024751,
        //
        // Summary:
        //     Invalid handle (it may already be closed).
        Handle = -2147024890,
        //
        // Summary:
        //     One or more arguments are invalid.
        InvalidArg = -2147024809,
        //
        // Summary:
        //     The system cannot find the path specified.
        PathNotFound = -2147024893,
        //
        // Summary:
        //     The object manager encountered a reparse point while retrieving an object.
        ReparsePointEncountered = -2147020501,
        //
        // Summary:
        //     The virtualization operation is not allowed on the file in its current state.
        VirtualizationInvalidOp = -2147024511
    }
}
