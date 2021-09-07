using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Reflection.Emit
{
	[OpenSilver.NotImplemented]
    public sealed partial class PropertyBuilder : PropertyInfo
    {
		[OpenSilver.NotImplemented]
        public override PropertyAttributes Attributes => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override bool CanRead => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override bool CanWrite => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override Type PropertyType => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override Type DeclaringType => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override string Name => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override Type ReflectedType => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

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
        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public void SetGetMethod(MethodBuilder mdBuilder)
        {

        }
        
		[OpenSilver.NotImplemented]
        public void SetSetMethod(MethodBuilder mdBuilder)
        {

        }
    }
}

