using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Reflection.Emit
{
	[OpenSilver.NotImplemented]
    public sealed partial class TypeBuilder : Type
    {
		[OpenSilver.NotImplemented]
        public override Assembly Assembly => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override string AssemblyQualifiedName => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override Type BaseType => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override string FullName => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override Guid GUID => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override Module Module => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override string Namespace => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override Type UnderlyingSystemType => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override string Name => throw new NotImplementedException();

		[OpenSilver.NotImplemented]
        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
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
        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override Type GetInterface(string name, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override Type[] GetInterfaces()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        protected override bool HasElementTypeImpl()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        protected override bool IsArrayImpl()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        protected override bool IsByRefImpl()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        protected override bool IsCOMObjectImpl()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        protected override bool IsPointerImpl()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        protected override bool IsPrimitiveImpl()
        {
            throw new NotImplementedException();
        }

		[OpenSilver.NotImplemented]
        public Type CreateType()
        {
            return new MethodBuilder().GetType();
        }

		[OpenSilver.NotImplemented]
        public FieldBuilder DefineField(string fieldName, Type type, FieldAttributes attributes)
        {
            return new FieldBuilder();
        }

		[OpenSilver.NotImplemented]
        public PropertyBuilder DefineProperty(string name, PropertyAttributes attributes, Type returnType, Type[] parameterTypes)
        {
            return new PropertyBuilder();
        }

		[OpenSilver.NotImplemented]
        public MethodBuilder DefineMethod(string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
        {
            return new MethodBuilder();
        }

  
		[OpenSilver.NotImplemented]
        public MethodBuilder DefineMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
        {
            return new MethodBuilder();
        }

		[OpenSilver.NotImplemented]
        public ConstructorBuilder DefineDefaultConstructor(MethodAttributes attributes)
        {
            return new ConstructorBuilder();
        }

    }
}

