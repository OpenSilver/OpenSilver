using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace System.Reflection.Emit
{
	[OpenSilver.NotImplemented]
    public sealed partial class ConstructorBuilder : ConstructorInfo
    {
		[OpenSilver.NotImplemented]
        public override MethodAttributes Attributes => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override RuntimeMethodHandle MethodHandle => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override Type DeclaringType => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override string Name => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override Type ReflectedType => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override ParameterInfo[] GetParameters()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
    }
}
