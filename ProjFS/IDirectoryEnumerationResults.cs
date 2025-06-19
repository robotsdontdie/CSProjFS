namespace CSProjFS
{
    //
    // Summary:
    //     Interface to allow for easier unit testing of a virtualization provider.
    //
    // Remarks:
    //     This class defines the interface implemented by the Microsoft.Windows.ProjFS.DirectoryEnumerationResults
    //     class. This interface class is provided for use by unit tests to mock up the
    //     interface to ProjFS.
    public interface IDirectoryEnumerationResults
    {
        IntPtr DirEntryBufferHandle { get; }

        //
        // Summary:
        //     When overridden in a derived class, adds one entry to a directory enumeration
        //     result.
        //
        // Parameters:
        //   fileName:
        //     The name of the file or directory.
        //
        //   fileSize:
        //     The size of the file.
        //
        //   isDirectory:
        //     true if this item is a directory, false if it is a file.
        //
        //   fileAttributes:
        //     The file attributes.
        //
        //   creationTime:
        //     Specifies the time that the file was created.
        //
        //   lastAccessTime:
        //     Specifies the time that the file was last accessed.
        //
        //   lastWriteTime:
        //     Specifies the time that the file was last written to.
        //
        //   changeTime:
        //     Specifies the last time the file was changed.
        //
        // Returns:
        //     true if the entry was successfully added to the enumeration buffer, false otherwise.
        //
        //
        // Remarks:
        //     In its implementation of a GetDirectoryEnumerationCallback delegate the provider
        //     calls this method for each matching file or directory in the enumeration.
        //
        //     If this method returns false, the provider returns HResult.Ok and waits for the
        //     next GetDirectoryEnumerationCallback. Then it resumes filling the enumeration
        //     with the entry it was trying to add when it got false.
        //
        //     If the function returns false for the first file or directory in the enumeration,
        //     the provider returns HResult.InsufficientBuffer from the GetDirectoryEnumerationCallback
        //     method.
        bool Add(string fileName, long fileSize, bool isDirectory, FileAttributes fileAttributes, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime, DateTime changeTime);

        //
        // Summary:
        //     When overridden in a derived class, adds one entry to a directory enumeration
        //     result.
        //
        // Parameters:
        //   fileName:
        //     The name of the file or directory.
        //
        //   fileSize:
        //     The size of the file.
        //
        //   isDirectory:
        //     true if this item is a directory, false if it is a file.
        //
        // Returns:
        //     true if the entry was successfully added to the enumeration buffer, false otherwise.
        //
        //
        // Remarks:
        //     In its implementation of a GetDirectoryEnumerationCallback delegate the provider
        //     calls this method for each matching file or directory in the enumeration.
        //
        //     If this method returns false, the provider returns HResult.Ok and waits for the
        //     next GetDirectoryEnumerationCallback. Then it resumes filling the enumeration
        //     with the entry it was trying to add when it got false.
        //
        //     If the function returns false for the first file or directory in the enumeration,
        //     the provider returns HResult.InsufficientBuffer from the GetDirectoryEnumerationCallback
        //     method.
        bool Add(string fileName, long fileSize, bool isDirectory);
    }
}
