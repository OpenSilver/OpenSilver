using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace OpenSilver.IO.IsolatedStorage
{
    /// <summary>Represents an isolated storage area containing files and directories.</summary>
    public sealed class IsolatedStorageFile : IsolatedStorage, IDisposable
    {
        private bool closed;
        private bool disposed;
        private DirectoryInfo directory;

        /// <summary>Gets the enumerator for the <see cref="T:System.IO.IsolatedStorage.IsolatedStorageFile" /> stores within an isolated storage scope.</summary>
        /// <param name="scope">Represents the <see cref="T:System.IO.IsolatedStorage.IsolatedStorageScope" /> for which to return isolated stores. <see langword="User" /> and <see langword="User|Roaming" /> are the only <see langword="IsolatedStorageScope" /> combinations supported.</param>
        /// <returns>Enumerator for the <see cref="T:System.IO.IsolatedStorage.IsolatedStorageFile" /> stores within the specified isolated storage scope.</returns>
        public static IEnumerator GetEnumerator(IsolatedStorageScope scope)
        {
            throw new NotImplementedException();
        }

        /// <summary>Obtains the isolated storage corresponding to the given application domain and assembly evidence objects.</summary>
        /// <param name="scope">A bitwise combination of the enumeration values.</param>
        /// <param name="domainIdentity">An object that contains evidence for the application domain identity.</param>
        /// <param name="assemblyIdentity">An object that contains evidence for the code assembly identity.</param>
        /// <returns>An object that represents the parameters.</returns>
        /// <exception cref="T:System.Security.SecurityException">Sufficient isolated storage permissions have not been granted.</exception>
        /// <exception cref="T:System.ArgumentNullException">Neither <paramref name="domainIdentity" /> nor <paramref name="assemblyIdentity" /> has been passed in. This verifies that the correct constructor is being used.
        /// -or-
        /// Either <paramref name="domainIdentity" /> or <paramref name="assemblyIdentity" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="scope" /> is invalid.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">An isolated storage location cannot be initialized.
        /// -or-
        /// <paramref name="scope" /> contains the enumeration value <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Application" />, but the application identity of the caller cannot be determined, because the <see cref="P:System.AppDomain.ActivationContext" /> for  the current application domain returned <see langword="null" />.
        /// -or-
        /// <paramref name="scope" /> contains the value <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Domain" />, but the permissions for the application domain cannot be determined.
        /// -or-
        /// <paramref name="scope" /> contains the value <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Assembly" />, but the permissions for the calling assembly cannot be determined.</exception>
        public static IsolatedStorageFile GetStore(
            IsolatedStorageScope scope,
            object domainIdentity,
            object assemblyIdentity)
        {
            return new IsolatedStorageFile(scope);
        }

        /// <summary>Obtains isolated storage corresponding to the isolated storage scope given the application domain and assembly evidence types.</summary>
        /// <param name="scope">A bitwise combination of the enumeration values.</param>
        /// <param name="domainEvidenceType">The type of the <see cref="T:System.Security.Policy.Evidence" /> that you can chose from the list of <see cref="T:System.Security.Policy.Evidence" /> present in the domain of the calling application. <see langword="null" /> lets the <see cref="T:System.IO.IsolatedStorage.IsolatedStorage" /> object choose the evidence.</param>
        /// <param name="assemblyEvidenceType">The type of the <see cref="T:System.Security.Policy.Evidence" /> that you can chose from the list of <see cref="T:System.Security.Policy.Evidence" /> present in the domain of the calling application. <see langword="null" /> lets the <see cref="T:System.IO.IsolatedStorage.IsolatedStorage" /> object choose the evidence.</param>
        /// <returns>An object that represents the parameters.</returns>
        /// <exception cref="T:System.Security.SecurityException">Sufficient isolated storage permissions have not been granted.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="scope" /> is invalid.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The evidence type provided is missing in the assembly evidence list.
        /// -or-
        /// An isolated storage location cannot be initialized.
        /// -or-
        /// <paramref name="scope" /> contains the enumeration value <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Application" />, but the application identity of the caller cannot be determined, because the <see cref="P:System.AppDomain.ActivationContext" /> for  the current application domain returned <see langword="null" />.
        /// -or-
        /// <paramref name="scope" /> contains the value <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Domain" />, but the permissions for the application domain cannot be determined.
        /// -or-
        /// <paramref name="scope" /> contains <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Assembly" />, but the permissions for the calling assembly cannot be determined.</exception>
        public static IsolatedStorageFile GetStore(
            IsolatedStorageScope scope,
            Type domainEvidenceType,
            Type assemblyEvidenceType)
        {
            return new IsolatedStorageFile(scope);
        }

        /// <summary>Obtains isolated storage corresponding to the given application identity.</summary>
        /// <param name="scope">A bitwise combination of the enumeration values.</param>
        /// <param name="applicationIdentity">An object that contains evidence for the application identity.</param>
        /// <returns>An object that represents the parameters.</returns>
        /// <exception cref="T:System.Security.SecurityException">Sufficient isolated storage permissions have not been granted.</exception>
        /// <exception cref="T:System.ArgumentNullException">The  <paramref name="applicationIdentity" /> identity has not been passed in.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="scope" /> is invalid.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">An isolated storage location cannot be initialized.
        /// -or-
        /// <paramref name="scope" /> contains the enumeration value <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Application" />, but the application identity of the caller cannot be determined,because the <see cref="P:System.AppDomain.ActivationContext" /> for  the current application domain returned <see langword="null" />.
        /// -or-
        /// <paramref name="scope" /> contains the value <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Domain" />, but the permissions for the application domain cannot be determined.
        /// -or-
        /// <paramref name="scope" /> contains the value <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Assembly" />, but the permissions for the calling assembly cannot be determined.</exception>
        public static IsolatedStorageFile GetStore(
            IsolatedStorageScope scope,
            object applicationIdentity)
        {
            return new IsolatedStorageFile(scope);
        }

        /// <summary>Obtains isolated storage corresponding to the isolation scope and the application identity object.</summary>
        /// <param name="scope">A bitwise combination of the enumeration values.</param>
        /// <param name="applicationEvidenceType">An object that contains the application identity.</param>
        /// <returns>An object that represents the parameters.</returns>
        /// <exception cref="T:System.Security.SecurityException">Sufficient isolated storage permissions have not been granted.</exception>
        /// <exception cref="T:System.ArgumentNullException">The   <paramref name="applicationEvidence" /> identity has not been passed in.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="scope" /> is invalid.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">An isolated storage location cannot be initialized.
        /// -or-
        /// <paramref name="scope" /> contains the enumeration value <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Application" />, but the application identity of the caller cannot be determined, because the <see cref="P:System.AppDomain.ActivationContext" /> for  the current application domain returned <see langword="null" />.
        /// -or-
        /// <paramref name="scope" /> contains the value <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Domain" />, but the permissions for the application domain cannot be determined.
        /// -or-
        /// <paramref name="scope" /> contains the value <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Assembly" />, but the permissions for the calling assembly cannot be determined.</exception>
        public static IsolatedStorageFile GetStore(
            IsolatedStorageScope scope,
            Type applicationEvidenceType)
        {
            return new IsolatedStorageFile(scope);
        }

        /// <summary>Obtains machine-scoped isolated storage corresponding to the calling code's application identity.</summary>
        /// <returns>An object corresponding to the isolated storage scope based on the calling code's application identity.</returns>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The application identity of the caller could not be determined.
        /// -or-
        /// The granted permission set for the application domain could not be determined.
        /// -or-
        /// An isolated storage location cannot be initialized.</exception>
        /// <exception cref="T:System.Security.SecurityException">Sufficient isolated storage permissions have not been granted.</exception>
        public static IsolatedStorageFile GetMachineStoreForApplication()
        {
            IsolatedStorageScope scope = IsolatedStorageScope.Machine | IsolatedStorageScope.Application;
            return new IsolatedStorageFile(scope);
        }

        /// <summary>Obtains machine-scoped isolated storage corresponding to the calling code's assembly identity.</summary>
        /// <returns>An object corresponding to the isolated storage scope based on the calling code's assembly identity.</returns>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">An isolated storage location cannot be initialized.</exception>
        /// <exception cref="T:System.Security.SecurityException">Sufficient isolated storage permissions have not been granted.</exception>
        public static IsolatedStorageFile GetMachineStoreForAssembly()
        {
            return new IsolatedStorageFile(IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine);
        }

        /// <summary>Obtains machine-scoped isolated storage corresponding to the application domain identity and the assembly identity.</summary>
        /// <returns>An object corresponding to the <see cref="T:System.IO.IsolatedStorage.IsolatedStorageScope" />, based on a combination of the application domain identity and the assembly identity.</returns>
        /// <exception cref="T:System.Security.SecurityException">Sufficient isolated storage permissions have not been granted.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The store failed to open.
        /// -or-
        /// The assembly specified has insufficient permissions to create isolated stores.
        /// -or-
        /// The permissions for the application domain cannot be determined.
        /// -or-
        /// An isolated storage location cannot be initialized.</exception>
        public static IsolatedStorageFile GetMachineStoreForDomain()
        {
            return new IsolatedStorageFile(IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine);
        }

        /// <summary>Obtains user-scoped isolated storage corresponding to the calling code's application identity.</summary>
        /// <returns>An object corresponding to the isolated storage scope based on the calling code's assembly identity.</returns>
        /// <exception cref="T:System.Security.SecurityException">Sufficient isolated storage permissions have not been granted.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">An isolated storage location cannot be initialized.
        /// -or-
        /// The application identity of the caller cannot be determined, because the <see cref="P:System.AppDomain.ActivationContext" /> property returned <see langword="null" />.
        /// -or-
        /// The permissions for the application domain cannot be determined.</exception>
        public static IsolatedStorageFile GetUserStoreForApplication()
        {
            IsolatedStorageScope scope = IsolatedStorageScope.User | IsolatedStorageScope.Application;
            return new IsolatedStorageFile(scope);
        }

        /// <summary>Obtains user-scoped isolated storage corresponding to the calling code's assembly identity.</summary>
        /// <returns>An object corresponding to the isolated storage scope based on the calling code's assembly identity.</returns>
        /// <exception cref="T:System.Security.SecurityException">Sufficient isolated storage permissions have not been granted.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">An isolated storage location cannot be initialized.
        /// -or-
        /// The permissions for the calling assembly cannot be determined.</exception>
        public static IsolatedStorageFile GetUserStoreForAssembly()
        {
            return new IsolatedStorageFile(IsolatedStorageScope.User | IsolatedStorageScope.Assembly);
        }

        /// <summary>Obtains user-scoped isolated storage corresponding to the application domain identity and assembly identity.</summary>
        /// <returns>An object corresponding to the <see cref="T:System.IO.IsolatedStorage.IsolatedStorageScope" />, based on a combination of the application domain identity and the assembly identity.</returns>
        /// <exception cref="T:System.Security.SecurityException">Sufficient isolated storage permissions have not been granted.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The store failed to open.
        /// -or-
        /// The assembly specified has insufficient permissions to create isolated stores.
        /// -or-
        /// An isolated storage location cannot be initialized.
        /// -or-
        /// The permissions for the application domain cannot be determined.</exception>
        public static IsolatedStorageFile GetUserStoreForDomain()
        {
            return new IsolatedStorageFile(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly);
        }

        /// <summary>Obtains a user-scoped isolated store for use by applications in a virtual host domain.</summary>
        /// <returns>The isolated storage file that corresponds to the isolated storage scope based on the calling code's application identity.</returns>
        public static IsolatedStorageFile GetUserStoreForSite() => throw new NotSupportedException();

        /// <summary>Removes the specified isolated storage scope for all identities.</summary>
        /// <param name="scope">A bitwise combination of the <see cref="T:System.IO.IsolatedStorage.IsolatedStorageScope" /> values.</param>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store cannot be removed.</exception>
        public static void Remove(IsolatedStorageScope scope)
        {
        }

        internal static string GetIsolatedStorageRoot(IsolatedStorageScope scope)
        {
            return "";
        }

        internal static ulong GetDirectorySize(DirectoryInfo di)
        {
            ulong num = 0;
            return num;
        }

        private IsolatedStorageFile(IsolatedStorageScope scope)
        {
            storage_scope = scope;
        }

        internal IsolatedStorageFile(IsolatedStorageScope scope, string location)
        {
            storage_scope = scope;
            this.directory = new DirectoryInfo(location);
        }

        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~IsolatedStorageFile()
        {
        }

        private void PostInit()
        {
        }

        /// <summary>Gets the current size of the isolated storage.</summary>
        /// <returns>The total number of bytes of storage currently in use within the isolated storage scope.</returns>
        /// <exception cref="T:System.InvalidOperationException">The property is unavailable. The current store has a roaming scope or is not open.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The current object size is undefined.</exception>
        [Obsolete]
        public override ulong CurrentSize => IsolatedStorageFile.GetDirectorySize(this.directory);

        /// <summary>Gets a value representing the maximum amount of space available for isolated storage within the limits established by the quota.</summary>
        /// <returns>The limit of isolated storage space in bytes.</returns>
        /// <exception cref="T:System.InvalidOperationException">The property is unavailable. <see cref="P:System.IO.IsolatedStorage.IsolatedStorageFile.MaximumSize" /> cannot be determined without evidence from the assembly's creation. The evidence could not be determined when the object was created.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">An isolated storage error occurred.</exception>
        [Obsolete]
        public override ulong MaximumSize => long.MaxValue;

        internal string Root => directory.FullName;

        /// <summary>Gets a value that represents the amount of free space available for isolated storage.</summary>
        /// <returns>The available free space for isolated storage, in bytes.</returns>
        /// <exception cref="T:System.InvalidOperationException">The isolated store is closed.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        public override long AvailableFreeSpace => long.MaxValue;

        /// <summary>Gets a value that represents the maximum amount of space available for isolated storage.</summary>
        /// <returns>The limit of isolated storage space, in bytes.</returns>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        public override long Quota => (long)MaximumSize;

        /// <summary>Gets a value that represents the amount of the space used for isolated storage.</summary>
        /// <returns>The used isolated storage space, in bytes.</returns>
        /// <exception cref="T:System.InvalidOperationException">The isolated store has been closed.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        public override long UsedSize => (long)GetDirectorySize(directory);

        /// <summary>Gets a value that indicates whether isolated storage is enabled.</summary>
        /// <returns>
        /// <see langword="true" /> in all cases.</returns>
        public static bool IsEnabled => true;

        internal bool IsClosed => closed;

        internal bool IsDisposed => disposed;

        /// <summary>Closes a store previously opened with <see cref="M:System.IO.IsolatedStorage.IsolatedStorageFile.GetStore(System.IO.IsolatedStorage.IsolatedStorageScope,System.Type,System.Type)" />, <see cref="M:System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForAssembly" />, or <see cref="M:System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForDomain" />.</summary>
        public void Close() => closed = true;

        /// <summary>Creates a directory in the isolated storage scope.</summary>
        /// <param name="dir">The relative path of the directory to create within the isolated storage scope.</param>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The current code has insufficient permissions to create isolated storage directory.</exception>
        /// <exception cref="T:System.ArgumentNullException">The directory path is <see langword="null" />.</exception>
        public void CreateDirectory(string dir)
        {
        }

        /// <summary>Copies an existing file to a new file.</summary>
        /// <param name="sourceFileName">The name of the file to copy.</param>
        /// <param name="destinationFileName">The name of the destination file. This cannot be a directory or an existing file.</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="sourceFileName" /> or <paramref name="destinationFileName" /> is a zero-length string, contains only white space, or contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="sourceFileName" /> or <paramref name="destinationFileName" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The isolated store has been closed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        /// <paramref name="sourceFileName" /> was not found.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        /// <paramref name="sourceFileName" /> was not found.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.
        /// -or-
        /// <paramref name="destinationFileName" /> exists.
        /// -or-
        /// An I/O error has occurred.</exception>
        public void CopyFile(string sourceFileName, string destinationFileName) => CopyFile(sourceFileName, destinationFileName, false);

        /// <summary>Copies an existing file to a new file, and optionally overwrites an existing file.</summary>
        /// <param name="sourceFileName">The name of the file to copy.</param>
        /// <param name="destinationFileName">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite">
        /// <see langword="true" /> if the destination file can be overwritten; otherwise, <see langword="false" />.</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="sourceFileName" /> or <paramref name="destinationFileName" /> is a zero-length string, contains only white space, or contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="sourceFileName" /> or <paramref name="destinationFileName" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The isolated store has been closed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        /// <paramref name="sourceFileName" /> was not found.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        /// <paramref name="sourceFileName" /> was not found.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.
        /// -or-
        /// An I/O error has occurred.</exception>
        public void CopyFile(string sourceFileName, string destinationFileName, bool overwrite)
        {
        }

        /// <summary>Creates a file in the isolated store.</summary>
        /// <param name="path">The relative path of the file to create.</param>
        /// <returns>A new isolated storage file.</returns>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="path" /> is malformed.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory in <paramref name="path" /> does not exist.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        public IsolatedStorageFileStream CreateFile(string path) => new IsolatedStorageFileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, this);

        /// <summary>Deletes a directory in the isolated storage scope.</summary>
        /// <param name="dir">The relative path of the directory to delete within the isolated storage scope.</param>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The directory could not be deleted.</exception>
        /// <exception cref="T:System.ArgumentNullException">The directory path was <see langword="null" />.</exception>
        public void DeleteDirectory(string dir)
        {
        }

        /// <summary>Deletes a file in the isolated storage scope.</summary>
        /// <param name="file">The relative path of the file to delete within the isolated storage scope.</param>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The target file is open or the path is incorrect.</exception>
        /// <exception cref="T:System.ArgumentNullException">The file path is <see langword="null" />.</exception>
        public void DeleteFile(string file)
        {
        }

        /// <summary>Releases all resources used by the <see cref="T:System.IO.IsolatedStorage.IsolatedStorageFile" />.</summary>
        public void Dispose()
        {
            this.disposed = true;
        }

        /// <summary>Determines whether the specified path refers to an existing directory in the isolated store.</summary>
        /// <param name="path">The path to test.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="path" /> refers to an existing directory in the isolated store and is not <see langword="null" />; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The isolated store is closed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.</exception>
        public bool DirectoryExists(string path)
        {
            return true;
        }

        /// <summary>Determines whether the specified path refers to an existing file in the isolated store.</summary>
        /// <param name="path">The path and file name to test.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="path" /> refers to an existing file in the isolated store and is not <see langword="null" />; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The isolated store is closed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.</exception>
        public bool FileExists(string path)
        {
            return false;
        }

        /// <summary>Returns the creation date and time of a specified file or directory.</summary>
        /// <param name="path">The path to the file or directory for which to obtain creation date and time information.</param>
        /// <returns>The creation date and time for the specified file or directory. This value is expressed in local time.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The isolated store has been closed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.</exception>
        public DateTimeOffset GetCreationTime(string path)
        {
            return DateTimeOffset.Now;
        }

        /// <summary>Returns the date and time a specified file or directory was last accessed.</summary>
        /// <param name="path">The path to the file or directory for which to obtain last access date and time information.</param>
        /// <returns>The date and time that the specified file or directory was last accessed. This value is expressed in local time.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The isolated store has been closed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.</exception>
        public DateTimeOffset GetLastAccessTime(string path)
        {
            return DateTimeOffset.Now;
        }

        /// <summary>Returns the date and time a specified file or directory was last written to.</summary>
        /// <param name="path">The path to the file or directory for which to obtain last write date and time information.</param>
        /// <returns>The date and time that the specified file or directory was last written to. This value is expressed in local time.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The isolated store has been closed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.</exception>
        public DateTimeOffset GetLastWriteTime(string path)
        {
            return DateTimeOffset.Now;
        }

        /// <summary>Enumerates the directories in an isolated storage scope that match a given search pattern.</summary>
        /// <param name="searchPattern">A search pattern. Both single-character ("?") and multi-character ("*") wildcards are supported.</param>
        /// <returns>An array of the relative paths of directories in the isolated storage scope that match <paramref name="searchPattern" />. A zero-length array specifies that there are no directories that match.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The isolated store is closed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">Caller does not have permission to enumerate directories resolved from <paramref name="searchPattern" />.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory or directories specified by <paramref name="searchPattern" /> are not found.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.</exception>
        public string[] GetDirectoryNames(string searchPattern)
        {
            return new string[0];
        }

        /// <summary>Enumerates the directories at the root of an isolated store.</summary>
        /// <returns>An array of relative paths of directories at the root of the isolated store. A zero-length array specifies that there are no directories at the root.</returns>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.InvalidOperationException">The isolated store is closed.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">Caller does not have permission to enumerate directories.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">One or more directories are not found.</exception>
        [ComVisible(false)]
        public string[] GetDirectoryNames() => GetDirectoryNames("*");

        /// <summary>Gets the file names that match a search pattern.</summary>
        /// <param name="searchPattern">A search pattern. Both single-character ("?") and multi-character ("*") wildcards are supported.</param>
        /// <returns>An array of relative paths of files in the isolated storage scope that match <paramref name="searchPattern" />. A zero-length array specifies that there are no files that match.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="searchPattern" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The file path specified by <paramref name="searchPattern" /> cannot be found.</exception>
        public string[] GetFileNames(string searchPattern)
        {
            return new string[0];
        }

        /// <summary>Enumerates the file names at the root of an isolated store.</summary>
        /// <returns>An array of relative paths of files at the root of the isolated store.  A zero-length array specifies that there are no files at the root.</returns>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">File paths from the isolated store root cannot be determined.</exception>
        public string[] GetFileNames() => this.GetFileNames("*");

        /// <summary>Enables an application to explicitly request a larger quota size, in bytes.</summary>
        /// <param name="newQuotaSize">The requested size, in bytes.</param>
        /// <returns>
        /// <see langword="true" /> if the new quota is accepted; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="newQuotaSize" /> is less than current quota size.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="newQuotaSize" /> is less than zero, or less than or equal to the current quota size.</exception>
        /// <exception cref="T:System.InvalidOperationException">The isolated store has been closed.</exception>
        /// <exception cref="T:System.NotSupportedException">The current scope is not for an application user.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.</exception>
        public override bool IncreaseQuotaTo(long newQuotaSize)
        {
            return false;
        }

        /// <summary>Moves a specified directory and its contents to a new location.</summary>
        /// <param name="sourceDirectoryName">The name of the directory to move.</param>
        /// <param name="destinationDirectoryName">The path to the new location for <paramref name="sourceDirectoryName" />. This cannot be the path to an existing directory.</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="sourceFileName" /> or <paramref name="destinationFileName" /> is a zero-length string, contains only white space, or contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="sourceFileName" /> or <paramref name="destinationFileName" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The isolated store has been closed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">
        /// <paramref name="sourceDirectoryName" /> does not exist.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.
        /// -or-
        /// <paramref name="destinationDirectoryName" /> already exists.
        /// -or-
        /// <paramref name="sourceDirectoryName" /> and <paramref name="destinationDirectoryName" /> refer to the same directory.</exception>
        public void MoveDirectory(string sourceDirectoryName, string destinationDirectoryName)
        {
        }

        /// <summary>Moves a specified file to a new location, and optionally lets you specify a new file name.</summary>
        /// <param name="sourceFileName">The name of the file to move.</param>
        /// <param name="destinationFileName">The path to the new location for the file. If a file name is included, the moved file will have that name.</param>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="sourceFileName" /> or <paramref name="destinationFileName" /> is a zero-length string, contains only white space, or contains one or more invalid characters defined by the <see cref="M:System.IO.Path.GetInvalidPathChars" /> method.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="sourceFileName" /> or <paramref name="destinationFileName" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">The isolated store has been closed.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">
        /// <paramref name="sourceFileName" /> was not found.</exception>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.</exception>
        public void MoveFile(string sourceFileName, string destinationFileName)
        {
        }

        /// <summary>Opens a file in the specified mode.</summary>
        /// <param name="path">The relative path of the file within the isolated store.</param>
        /// <param name="mode">One of the enumeration values that specifies how to open the file.</param>
        /// <returns>A file that is opened in the specified mode, with read/write access, and is unshared.</returns>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="path" /> is malformed.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory in <paramref name="path" /> does not exist.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">No file was found and the <paramref name="mode" /> is set to <see cref="F:System.IO.FileMode.Open" />.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        public IsolatedStorageFileStream OpenFile(string path, FileMode mode) => new IsolatedStorageFileStream(path, mode, this);

        /// <summary>Opens a file in the specified mode with the specified read/write access.</summary>
        /// <param name="path">The relative path of the file within the isolated store.</param>
        /// <param name="mode">One of the enumeration values that specifies how to open the file.</param>
        /// <param name="access">One of the enumeration values that specifies whether the file will be opened with read, write, or read/write access.</param>
        /// <returns>A file that is opened in the specified mode and access, and is unshared.</returns>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="path" /> is malformed.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory in <paramref name="path" /> does not exist.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">No file was found and the <paramref name="mode" /> is set to <see cref="F:System.IO.FileMode.Open" />.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        public IsolatedStorageFileStream OpenFile(
            string path,
            FileMode mode,
            FileAccess access)
        {
            return new IsolatedStorageFileStream(path, mode, access, this);
        }

        /// <summary>Opens a file in the specified mode, with the specified read/write access and sharing permission.</summary>
        /// <param name="path">The relative path of the file within the isolated store.</param>
        /// <param name="mode">One of the enumeration values that specifies how to open or create the file.</param>
        /// <param name="access">One of the enumeration values that specifies whether the file will be opened with read, write, or read/write access</param>
        /// <param name="share">A bitwise combination of enumeration values that specify the type of access other <see cref="T:System.IO.IsolatedStorage.IsolatedStorageFileStream" /> objects have to this file.</param>
        /// <returns>A file that is opened in the specified mode and access, and with the specified sharing options.</returns>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store has been removed.
        /// -or-
        /// Isolated storage is disabled.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="path" /> is malformed.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="path" /> is <see langword="null" />.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The directory in <paramref name="path" /> does not exist.</exception>
        /// <exception cref="T:System.IO.FileNotFoundException">No file was found and the <paramref name="mode" /> is set to <see cref="M:System.IO.FileInfo.Open(System.IO.FileMode)" />.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The isolated store has been disposed.</exception>
        public IsolatedStorageFileStream OpenFile(
            string path,
            FileMode mode,
            FileAccess access,
            FileShare share)
        {
            return new IsolatedStorageFileStream(path, mode, access, share, this);
        }

        /// <summary>Removes the isolated storage scope and all its contents.</summary>
        /// <exception cref="T:System.IO.IsolatedStorage.IsolatedStorageException">The isolated store cannot be deleted.</exception>
        public override void Remove()
        {
        }
    }
}