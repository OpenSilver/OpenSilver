#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Reflection.Emit
{
    public sealed partial class PropertyBuilder : PropertyInfo
    {
        public override PropertyAttributes Attributes => throw new NotImplementedException();

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override Type PropertyType => throw new NotImplementedException();

        public override Type DeclaringType => throw new NotImplementedException();

        public override string Name => throw new NotImplementedException();

        public override Type ReflectedType => throw new NotImplementedException();

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public void SetGetMethod(MethodBuilder mdBuilder)
        {

        }
        
        public void SetSetMethod(MethodBuilder mdBuilder)
        {

        }
    }
}

#endif