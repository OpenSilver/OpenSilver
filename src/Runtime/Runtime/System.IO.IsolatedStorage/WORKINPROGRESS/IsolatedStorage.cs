using System;
using System.IO;

namespace OpenSilver.IO.IsolatedStorage
{
    /// <summary>Represents the abstract base class from which all isolated storage implementations must derive.</summary>
    public abstract class IsolatedStorage : MarshalByRefObject
    {
        internal IsolatedStorageScope storage_scope;
        internal object _assemblyIdentity;
        internal object _domainIdentity;
        internal object _applicationIdentity;

        /// <summary>Gets an application identity that scopes isolated storage.</summary>
        /// <returns>An <see cref="T:System.Object" /> that represents the <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Application" /> identity.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.IO.IsolatedStorage.IsolatedStorage" /> object is not isolated by the application <see cref="T:System.IO.IsolatedStorage.IsolatedStorageScope" />.</exception>
        public object ApplicationIdentity => throw new NotImplementedException();

        /// <summary>Gets an assembly identity used to scope isolated storage.</summary>
        /// <returns>An <see cref="T:System.Object" /> that represents the <see cref="T:System.Reflection.Assembly" /> identity.</returns>
        /// <exception cref="T:System.InvalidOperationException">The assembly is not defined.</exception>
        public object AssemblyIdentity
        {
            get
            {
                if ((storage_scope & IsolatedStorageScope.Assembly) == IsolatedStorageScope.None)
                    throw new InvalidOperationException("Invalid Isolation Scope.");
                return _assemblyIdentity ?? throw new InvalidOperationException("Identity unavailable.");
            }
        }

        /// <summary>Gets a value representing the current size of isolated storage.</summary>
        /// <returns>The number of storage units currently used within the isolated storage scope.</returns>
        /// <exception cref="T:System.InvalidOperationException">The current size of the isolated store is undefined.</exception>
        [Obsolete("IsolatedStorage.CurrentSize has been deprecated because it is not CLS Compliant.  To get the current size use IsolatedStorage.UsedSize")]
        public virtual ulong CurrentSize => throw new InvalidOperationException("IsolatedStorage does not have a preset CurrentSize.");

        /// <summary>Gets a domain identity that scopes isolated storage.</summary>
        /// <returns>An <see cref="T:System.Object" /> that represents the <see cref="F:System.IO.IsolatedStorage.IsolatedStorageScope.Domain" /> identity.</returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.IO.IsolatedStorage.IsolatedStorage" /> object is not isolated by the domain <see cref="T:System.IO.IsolatedStorage.IsolatedStorageScope" />.</exception>
        public object DomainIdentity
        {
            get
            {
                if ((this.storage_scope & IsolatedStorageScope.Domain) == IsolatedStorageScope.None)
                    throw new InvalidOperationException("Invalid Isolation Scope.");
                return _domainIdentity ?? throw new InvalidOperationException("Identity unavailable.");
            }
        }

        /// <summary>Gets a value representing the maximum amount of space available for isolated storage. When overridden in a derived class, this value can take different units of measure.</summary>
        /// <returns>The maximum amount of isolated storage space in bytes. Derived classes can return different units of value.</returns>
        /// <exception cref="T:System.InvalidOperationException">The quota has not been defined.</exception>
        [Obsolete]
        public virtual ulong MaximumSize => throw new InvalidOperationException("IsolatedStorage does not have a preset MaximumSize.");

        /// <summary>Gets an <see cref="T:System.IO.IsolatedStorage.IsolatedStorageScope" /> enumeration value specifying the scope used to isolate the store.</summary>
        /// <returns>A bitwise combination of <see cref="T:System.IO.IsolatedStorage.IsolatedStorageScope" /> values specifying the scope used to isolate the store.</returns>
        public IsolatedStorageScope Scope => storage_scope;

        /// <summary>When overridden in a derived class, gets the available free space for isolated storage, in bytes.</summary>
        /// <returns>The available free space for isolated storage, in bytes.</returns>
        /// <exception cref="T:System.InvalidOperationException">An operation was performed that requires access to <see cref="P:System.IO.IsolatedStorage.IsolatedStorage.AvailableFreeSpace" />, but that property is not defined for this store. Stores that are obtained by using enumerations do not have a well-defined <see cref="P:System.IO.IsolatedStorage.IsolatedStorage.AvailableFreeSpace" /> property, because partial evidence is used to open the store.</exception>
        public virtual long AvailableFreeSpace => throw new InvalidOperationException("This property is not defined for this store.");

        /// <summary>When overridden in a derived class, gets a value that represents the maximum amount of space available for isolated storage.</summary>
        /// <returns>The limit of isolated storage space, in bytes.</returns>
        /// <exception cref="T:System.InvalidOperationException">An operation was performed that requires access to <see cref="P:System.IO.IsolatedStorage.IsolatedStorage.Quota" />, but that property is not defined for this store. Stores that are obtained by using enumerations do not have a well-defined <see cref="P:System.IO.IsolatedStorage.IsolatedStorage.Quota" /> property, because partial evidence is used to open the store.</exception>
        public virtual long Quota => throw new InvalidOperationException("This property is not defined for this store.");

        /// <summary>When overridden in a derived class, gets a value that represents the amount of the space used for isolated storage.</summary>
        /// <returns>The used amount of isolated storage space, in bytes.</returns>
        /// <exception cref="T:System.InvalidOperationException">An operation was performed that requires access to <see cref="P:System.IO.IsolatedStorage.IsolatedStorage.UsedSize" />, but that property is not defined for this store. Stores that are obtained by using enumerations do not have a well-defined <see cref="P:System.IO.IsolatedStorage.IsolatedStorage.UsedSize" /> property, because partial evidence is used to open the store.</exception>
        public virtual long UsedSize => throw new InvalidOperationException("This property is not defined for this store.");

        /// <summary>Gets a backslash character that can be used in a directory string. When overridden in a derived class, another character might be returned.</summary>
        /// <returns>The default implementation returns the '\' (backslash) character.</returns>
        protected virtual char SeparatorExternal => Path.DirectorySeparatorChar;

        /// <summary>Gets a period character that can be used in a directory string. When overridden in a derived class, another character might be returned.</summary>
        /// <returns>The default implementation returns the '.' (period) character.</returns>
        protected virtual char SeparatorInternal => '.';

        /// <summary>When overridden in a derived class, removes the individual isolated store and all contained data.</summary>
        public abstract void Remove();

        /// <summary>When overridden in a derived class, prompts a user to approve a larger quota size, in bytes, for isolated storage.</summary>
        /// <param name="newQuotaSize">The requested new quota size, in bytes, for the user to approve.</param>
        /// <returns>
        /// <see langword="false" /> in all cases.</returns>
        public virtual bool IncreaseQuotaTo(long newQuotaSize) => false;
    }
}