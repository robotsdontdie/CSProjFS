using static ProjFS.Native.ProjectedFSLib;

namespace ProjFS
{
    public unsafe class WriteBuffer : IWriteBuffer
    {
        private readonly nint buffer;
        private readonly UnmanagedMemoryStream stream;

        public WriteBuffer(uint bufferSize, nint virtualizationContext)
        {
            buffer = PrjAllocateAlignedBuffer(virtualizationContext, bufferSize);
            stream = new UnmanagedMemoryStream((byte*)buffer, bufferSize, bufferSize, FileAccess.Write);
        }

        public long Length => stream.Length;

        public UnmanagedMemoryStream Stream => stream;

        public nint Pointer => buffer;

        public void Dispose()
        {
            PrjFreeAlignedBuffer(buffer);
        }
    }
}
