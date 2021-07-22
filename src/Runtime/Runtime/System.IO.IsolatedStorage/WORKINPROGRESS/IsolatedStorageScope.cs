using System;

namespace OpenSilver.IO.IsolatedStorage
{
    /// <summary>Enumerates the levels of isolated storage scope that are supported by <see cref="T:System.IO.IsolatedStorage.IsolatedStorage" />.</summary>
    [Flags]
    public enum IsolatedStorageScope
    {
        /// <summary>No isolated storage usage.</summary>
        None = 0,
        /// <summary>Isolated storage scoped by user identity.</summary>
        User = 1,
        /// <summary>Isolated storage scoped to the application domain identity.</summary>
        Domain = 2,
        /// <summary>Isolated storage scoped to the identity of the assembly.</summary>
        Assembly = 4,
        /// <summary>The isolated store can be placed in a location on the file system that might roam (if roaming user data is enabled on the underlying operating system).</summary>
        Roaming = 8,
        /// <summary>Isolated storage scoped to the machine.</summary>
        Machine = 16, // 0x00000010
        /// <summary>Isolated storage scoped to the application.</summary>
        Application = 32, // 0x00000020
    }
}