using System.Collections;
using System.Reflection;

namespace System.ComponentModel.Design.Serialization
{
    public sealed class InstanceDescriptor
    {
        public InstanceDescriptor(MemberInfo member, ICollection arguments) : this(member, arguments, true)
        {
            throw new NotImplementedException();
        }

        public InstanceDescriptor(MemberInfo member, ICollection arguments, bool isComplete)
        {
            throw new NotImplementedException();
        }
    }
}
