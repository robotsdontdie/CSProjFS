﻿namespace CSProjFS
{
    /// <summary>
    /// Interface to allow for easier unit testing of a virtualization provider.
    /// </summary>
    /// <remarks>
    /// This class defines the interface implemented by the <c>Microsoft.Windows.ProjFS.WriteBuffer</c>
    /// class.  This interface class is provided for use by unit tests to mock up the interface to ProjFS.
    /// </remarks>
    public interface IWriteBuffer : IDisposable
    {

        /// <summary>
        /// When overridden in a derived class, gets the allocated length of the buffer.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// When overridden in a derived class, gets a <see cref="System::IO::UnmanagedMemoryStream"/>
        /// representing the internal buffer.
        /// </summary>
        UnmanagedMemoryStream Stream { get; }

        /// <summary>
        /// When overridden in a derived class, gets a pointer to the internal buffer.
        /// </summary>
        IntPtr Pointer { get; }
    }
}
