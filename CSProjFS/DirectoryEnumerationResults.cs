
using static CSProjFS.Native.ProjectedFSLib;

namespace CSProjFS
{
    public class DirectoryEnumerationResults : IDirectoryEnumerationResults
    {
        public DirectoryEnumerationResults(nint dirEntryBufferHandle)
        {
            DirEntryBufferHandle = dirEntryBufferHandle;
        }

        public IntPtr DirEntryBufferHandle { get; }

        public bool Add(string fileName, long fileSize, bool isDirectory)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName cannot be empty");
            }

            var basicInfo = new PRJ_FILE_BASIC_INFO
            {
                FileSize = fileSize,
                IsDirectory = isDirectory,
            };

            var result = PrjFillDirEntryBuffer(fileName, basicInfo, DirEntryBufferHandle);
            return result == HResult.Ok;
        }

        public bool Add(string fileName, long fileSize, bool isDirectory, FileAttributes fileAttributes, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime, DateTime changeTime)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName cannot be empty");
            }

            var basicInfo = new PRJ_FILE_BASIC_INFO
            {
                FileSize = fileSize,
                IsDirectory = isDirectory,
                FileAttributes = (uint)fileAttributes,
                CreationTime = creationTime.ToFileTime(),
                LastAccessTime = lastAccessTime.ToFileTime(),
                LastWriteTime = lastWriteTime.ToFileTime(),
                ChangeTime = changeTime.ToFileTime(),
            };

            var result = PrjFillDirEntryBuffer(fileName, basicInfo, DirEntryBufferHandle);
            return result == HResult.Ok;
        }
    }
}
