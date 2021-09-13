using System.Runtime.InteropServices;
using System.Security;

namespace System.Reflection.Emit
{
	//
	// Summary:
	//     Creates or associates parameter information.
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_ParameterBuilder))]
	[ComVisible(true)]
	[OpenSilver.NotImplemented]
	public class ParameterBuilder : _ParameterBuilder
	{
		//
		// Summary:
		//     Gets the attributes for this parameter.
		//
		// Returns:
		//     Read-only. Retrieves the attributes for this parameter.
		[OpenSilver.NotImplemented]
		public virtual int Attributes { get; }
		//
		// Summary:
		//     Gets a value that indicates whether this is an input parameter.
		//
		// Returns:
		//     Retrieves whether this is an input parameter.
		[OpenSilver.NotImplemented]
		public bool IsIn { get; }
		//
		// Summary:
		//     Gets a value that indicates whether this parameter is optional.
		//
		// Returns:
		//     A value that indicates whether this parameter is optional.
		[OpenSilver.NotImplemented]
		public bool IsOptional { get; }
		//
		// Summary:
		//     Gets a value that indicates whether this parameter is an output parameter.
		//
		// Returns:
		//     Retrieves whether this parameter is an output parameter.
		[OpenSilver.NotImplemented]
		public bool IsOut { get; }
		//
		// Summary:
		//     Gets the name of this parameter.
		//
		// Returns:
		//     The name of this parameter.
		[OpenSilver.NotImplemented]
		public virtual string Name { get; }
		//
		// Summary:
		//     Gets the signature position for this parameter.
		//
		// Returns:
		//     The signature position for this parameter.
		[OpenSilver.NotImplemented]
		public virtual int Position { get; }

		//
		// Summary:
		//     Gets the token for this parameter.
		//
		// Returns:
		//     The token for this parameter.
		[OpenSilver.NotImplemented]
		public virtual ParameterToken GetToken()
		{
			return default(ParameterToken);
		}
		//
		// Summary:
		//     Sets the default value of the parameter.
		//
		// Parameters:
		//   defaultValue:
		//     The default value of this parameter.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     The parameter is not one of the supported types.-or-The type of defaultValue
		//     does not match the type of the parameter.-or-The parameter is of type System.Object
		//     or other reference type, defaultValue is not null, and the value cannot be assigned
		//     to the reference type.
		[SecuritySafeCritical]
		[OpenSilver.NotImplemented]
		public virtual void SetConstant(object defaultValue)
		{
			
		}
		//
		// Summary:
		//     Set a custom attribute using a custom attribute builder.
		//
		// Parameters:
		//   customBuilder:
		//     An instance of a helper class to define the custom attribute.
		//
		// Exceptions:
		//   T:System.ArgumentNullException:
		//     con is null.
		[SecuritySafeCritical]
		[OpenSilver.NotImplemented]
		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			
		}
	}
}
