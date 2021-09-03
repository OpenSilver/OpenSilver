using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Reflection.Emit
{
	//
	// Summary:
	//     Defines and represents a dynamic method that can be compiled, executed, and discarded.
	//     Discarded methods are available for garbage collection.
	[ComVisible(true)]
	[OpenSilver.NotImplemented]
	public sealed partial class DynamicMethod : MethodInfo
	{
		//
		// Summary:
		//     Creates an anonymously hosted dynamic method, specifying the method name, return
		//     type, and parameter types.
		//
		// Parameters:
		//   name:
		//     The name of the dynamic method. This can be a zero-length string, but it cannot
		//     be null.
		//
		//   returnType:
		//     A System.Type object that specifies the return type of the dynamic method, or
		//     null if the method has no return type.
		//
		//   parameterTypes:
		//     An array of System.Type objects specifying the types of the parameters of the
		//     dynamic method, or null if the method has no parameters.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     An element of parameterTypes is null or System.Void.
		//
		//   T:System.ArgumentNullException:
		//     name is null.
		//
		//   T:System.NotSupportedException:
		//     returnType is a type for which System.Type.IsByRef returns true.
		[SecuritySafeCritical]
		[OpenSilver.NotImplemented]
		public DynamicMethod(string name, Type returnType, Type[] parameterTypes)
		{
			
		}

		//
		// Summary:
		//     Gets the attributes specified when the dynamic method was created.
		//
		// Returns:
		//     A bitwise combination of the System.Reflection.MethodAttributes values representing
		//     the attributes for the method.
		[OpenSilver.NotImplemented]
		public override MethodAttributes Attributes { get; }
		//
		// Summary:
		//     Gets the calling convention specified when the dynamic method was created.
		//
		// Returns:
		//     One of the System.Reflection.CallingConventions values that indicates the calling
		//     convention of the method.
		[OpenSilver.NotImplemented]
		public override CallingConventions CallingConvention { get; }
		//
		// Summary:
		//     Gets the type that declares the method, which is always null for dynamic methods.
		//
		// Returns:
		//     Always null.
		[OpenSilver.NotImplemented]
		public override Type DeclaringType { get; }
		//
		// Summary:
		//     Gets or sets a value indicating whether the local variables in the method are
		//     zero-initialized.
		//
		// Returns:
		//     true if the local variables in the method are zero-initialized; otherwise, false.
		//     The default is true.
		[OpenSilver.NotImplemented]
		public bool InitLocals { get; set; }
		//
		// Summary:
		//     Not supported for dynamic methods.
		//
		// Returns:
		//     Not supported for dynamic methods.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     Not allowed for dynamic methods.
		[OpenSilver.NotImplemented]
		public override RuntimeMethodHandle MethodHandle { get; }
		//
		// Summary:
		//     Gets the module with which the dynamic method is logically associated.
		//
		// Returns:
		//     The System.Reflection.Module with which the current dynamic method is associated.
		[OpenSilver.NotImplemented]
		public override Module Module { get; }
		//
		// Summary:
		//     Gets the name of the dynamic method.
		//
		// Returns:
		//     The simple name of the method.
		[OpenSilver.NotImplemented]
		public override string Name { get; }
		//
		// Summary:
		//     Gets the class that was used in reflection to obtain the method.
		//
		// Returns:
		//     Always null.
		[OpenSilver.NotImplemented]
		public override Type ReflectedType { get; }
		//
		// Summary:
		//     Gets the return parameter of the dynamic method.
		//
		// Returns:
		//     Always null.
		[OpenSilver.NotImplemented]
		public override ParameterInfo ReturnParameter { get; }
		//
		// Summary:
		//     Gets the type of return value for the dynamic method.
		//
		// Returns:
		//     A System.Type representing the type of the return value of the current method;
		//     System.Void if the method has no return type.
		[OpenSilver.NotImplemented]
		public override Type ReturnType { get; }
		//
		// Summary:
		//     Gets the custom attributes of the return type for the dynamic method.
		//
		// Returns:
		//     An System.Reflection.ICustomAttributeProvider representing the custom attributes
		//     of the return type for the dynamic method.
		[OpenSilver.NotImplemented]
		public override ICustomAttributeProvider ReturnTypeCustomAttributes { get; }

		//
		// Summary:
		//     Completes the dynamic method and creates a delegate of the specified type, which
		//     can be used to execute the dynamic method.
		//
		// Parameters:
		//   delegateType:
		//     A delegate type whose signature matches that of the dynamic method.
		//
		// Returns:
		//     A delegate of the specified type, which can be used to execute the dynamic method.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     The dynamic method has no method body.
		//
		//   T:System.ArgumentException:
		//     delegateType has the wrong number of parameters or the wrong parameter types.
		[ComVisible(true)]
		[SecuritySafeCritical]
		[OpenSilver.NotImplemented]
		public sealed override Delegate CreateDelegate(Type delegateType)
		{
			return default(Delegate);
		}
		//
		// Summary:
		//     Completes the dynamic method and creates a delegate that can be used to execute
		//     it, specifying the delegate type and an object the delegate is bound to.
		//
		// Parameters:
		//   delegateType:
		//     A delegate type whose signature matches that of the dynamic method, minus the
		//     first parameter.
		//
		//   target:
		//     An object the delegate is bound to. Must be of the same type as the first parameter
		//     of the dynamic method.
		//
		// Returns:
		//     A delegate of the specified type, which can be used to execute the dynamic method
		//     with the specified target object.
		//
		// Exceptions:
		//   T:System.InvalidOperationException:
		//     The dynamic method has no method body.
		//
		//   T:System.ArgumentException:
		//     target is not the same type as the first parameter of the dynamic method, and
		//     is not assignable to that type.-or-delegateType has the wrong number of parameters
		//     or the wrong parameter types.
		[ComVisible(true)]
		[SecuritySafeCritical]
		[OpenSilver.NotImplemented]
		public sealed override Delegate CreateDelegate(Type delegateType, object target)
		{
			return default(Delegate);
		}
		//
		// Summary:
		//     Defines a parameter of the dynamic method.
		//
		// Parameters:
		//   position:
		//     The position of the parameter in the parameter list. Parameters are indexed beginning
		//     with the number 1 for the first parameter.
		//
		//   attributes:
		//     A bitwise combination of System.Reflection.ParameterAttributes values that specifies
		//     the attributes of the parameter.
		//
		//   parameterName:
		//     The name of the parameter. The name can be a zero-length string.
		//
		// Returns:
		//     Always returns null.
		//
		// Exceptions:
		//   T:System.ArgumentOutOfRangeException:
		//     The method has no parameters.-or- position is less than 0.-or- position is greater
		//     than the number of the method's parameters.
		[OpenSilver.NotImplemented]
		public ParameterBuilder DefineParameter(int position, ParameterAttributes attributes, string parameterName)
		{
			return default(ParameterBuilder);
		}
		//
		// Summary:
		//     Returns the base implementation for the method.
		//
		// Returns:
		//     The base implementation of the method.
		[OpenSilver.NotImplemented]
		public override MethodInfo GetBaseDefinition()
		{
			return default(MethodInfo);
		}
		//
		// Summary:
		//     Returns all the custom attributes defined for the method.
		//
		// Parameters:
		//   inherit:
		//     true to search the method's inheritance chain to find the custom attributes;
		//     false to check only the current method.
		//
		// Returns:
		//     An array of objects representing all the custom attributes of the method.
		[OpenSilver.NotImplemented]
		public override object[] GetCustomAttributes(bool inherit)
		{
			return default(object[]);
		}
		//
		// Summary:
		//     Returns the custom attributes of the specified type that have been applied to
		//     the method.
		//
		// Parameters:
		//   attributeType:
		//     A System.Type representing the type of custom attribute to return.
		//
		//   inherit:
		//     true to search the method's inheritance chain to find the custom attributes;
		//     false to check only the current method.
		//
		// Returns:
		//     An array of objects representing the attributes of the method that are of type
		//     attributeType or derive from type attributeType.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     attributeType is null.
		[OpenSilver.NotImplemented]
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return default(object[]);
		}
		//
		// Summary:
		//     Returns a Microsoft intermediate language (MSIL) generator for the method with
		//     a default MSIL stream size of 64 bytes.
		//
		// Returns:
		//     An System.Reflection.Emit.ILGenerator object for the method.
		[OpenSilver.NotImplemented]
		public ILGenerator GetILGenerator()
		{
			return default(ILGenerator);
		}
		//
		// Summary:
		//     Returns a Microsoft intermediate language (MSIL) generator for the method with
		//     the specified MSIL stream size.
		//
		// Parameters:
		//   streamSize:
		//     The size of the MSIL stream, in bytes.
		//
		// Returns:
		//     An System.Reflection.Emit.ILGenerator object for the method, with the specified
		//     MSIL stream size.
		[SecuritySafeCritical]
		[OpenSilver.NotImplemented]
		public ILGenerator GetILGenerator(int streamSize)
		{
			return default(ILGenerator);
		}
		//
		// Summary:
		//     Returns the implementation flags for the method.
		//
		// Returns:
		//     A bitwise combination of System.Reflection.MethodImplAttributes values representing
		//     the implementation flags for the method.
		[OpenSilver.NotImplemented]
		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return default(MethodImplAttributes);
		}
		//
		// Summary:
		//     Returns the parameters of the dynamic method.
		//
		// Returns:
		//     An array of System.Reflection.ParameterInfo objects that represent the parameters
		//     of the dynamic method.
		[OpenSilver.NotImplemented]
		public override ParameterInfo[] GetParameters()
		{
			return default(ParameterInfo[]);
		}
		//
		// Summary:
		//     Invokes the dynamic method using the specified parameters, under the constraints
		//     of the specified binder, with the specified culture information.
		//
		// Parameters:
		//   obj:
		//     This parameter is ignored for dynamic methods, because they are static. Specify
		//     null.
		//
		//   invokeAttr:
		//     A bitwise combination of System.Reflection.BindingFlags values.
		//
		//   binder:
		//     A System.Reflection.Binder object that enables the binding, coercion of argument
		//     types, invocation of members, and retrieval of System.Reflection.MemberInfo objects
		//     through reflection. If binder is null, the default binder is used. For more details,
		//     see System.Reflection.Binder.
		//
		//   parameters:
		//     An argument list. This is an array of arguments with the same number, order,
		//     and type as the parameters of the method to be invoked. If there are no parameters
		//     this parameter should be null.
		//
		//   culture:
		//     An instance of System.Globalization.CultureInfo used to govern the coercion of
		//     types. If this is null, the System.Globalization.CultureInfo for the current
		//     thread is used. For example, this information is needed to correctly convert
		//     a System.String that represents 1000 to a System.Double value, because 1000 is
		//     represented differently by different cultures.
		//
		// Returns:
		//     A System.Object containing the return value of the invoked method.
		//
		// Exceptions:
		//   T:System.NotSupportedException:
		//     The System.Reflection.CallingConventions.VarArgs calling convention is not supported.
		//
		//   T:System.Reflection.TargetParameterCountException:
		//     The number of elements in parameters does not match the number of parameters
		//     in the dynamic method.
		//
		//   T:System.ArgumentException:
		//     The type of one or more elements of parameters does not match the type of the
		//     corresponding parameter of the dynamic method.
		//
		//   T:System.Reflection.TargetInvocationException:
		//     The dynamic method is associated with a module, is not anonymously hosted, and
		//     was constructed with skipVisibility set to false, but the dynamic method accesses
		//     members that are not public or internal (Friend in Visual Basic).-or-The dynamic
		//     method is anonymously hosted and was constructed with skipVisibility set to false,
		//     but it accesses members that are not public.
		[SecuritySafeCritical]
		[OpenSilver.NotImplemented]
		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return default(object);
		}
		//
		// Summary:
		//     Indicates whether the specified custom attribute type is defined.
		//
		// Parameters:
		//   attributeType:
		//     A System.Type representing the type of custom attribute to search for.
		//
		//   inherit:
		//     true to search the method's inheritance chain to find the custom attributes;
		//     false to check only the current method.
		//
		// Returns:
		//     true if the specified custom attribute type is defined; otherwise, false.
		[OpenSilver.NotImplemented]
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return default(bool);
		}
		//
		// Summary:
		//     Returns the signature of the method, represented as a string.
		//
		// Returns:
		//     A string representing the method signature.
		[OpenSilver.NotImplemented]
		public override string ToString()
		{
			return default(string);
		}
	}
}
