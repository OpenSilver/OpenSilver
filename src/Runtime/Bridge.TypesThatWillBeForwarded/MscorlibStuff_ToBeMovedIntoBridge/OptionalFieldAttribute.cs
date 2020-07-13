namespace System.Runtime.Serialization
{
    using System;
    using System.Diagnostics.Contracts;

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class OptionalFieldAttribute : Attribute
    {
        int _versionAdded = 1;
        public OptionalFieldAttribute() { }

        public int VersionAdded
        {
            get
            {
                return _versionAdded;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Serialization_OptionalFieldVersionValue");
                Contract.EndContractBlock();
                _versionAdded = value;
            }
        }
    }
}