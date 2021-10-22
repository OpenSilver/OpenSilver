using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	//
	// Summary:
	//     Represents a custom attribute in a form that can be attached to a type or member
	//     that is being emitted.
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_CustomAttributeBuilder))]
	[ComVisible(true)]
	[OpenSilver.NotImplemented]
	public class CustomAttributeBuilder : _CustomAttributeBuilder
	{
		//
		// Summary:
		//     Initializes a new instance of the CustomAttributeBuilder class given the constructor
		//     for the custom attribute and the arguments to the constructor.
		//
		// Parameters:
		//   con:
		//     The constructor for the custom attribute.
		//
		//   constructorArgs:
		//     The arguments to the constructor of the custom attribute.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     con is static or private.-or- The number of supplied arguments does not match
		//     the number of parameters of the constructor as required by the calling convention
		//     of the constructor.-or- The type of supplied argument does not match the type
		//     of the parameter declared in the constructor. -or-A supplied argument is a reference
		//     type other than System.String or System.Type.
		//
		//   T:System.ArgumentNullException:
		//     con or constructorArgs is null.
		[OpenSilver.NotImplemented]
		public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs)
		{
			
		}
		//
		// Summary:
		//     Initializes a new instance of the CustomAttributeBuilder class, given the constructor
		//     for the custom attribute, the arguments to the constructor, and a set of named
		//     property or value pairs.
		//
		// Parameters:
		//   con:
		//     The constructor for the custom attribute.
		//
		//   constructorArgs:
		//     The arguments to the constructor of the custom attribute.
		//
		//   namedProperties:
		//     Named properties of the custom attribute.
		//
		//   propertyValues:
		//     Values for the named properties of the custom attribute.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     The lengths of the namedProperties and propertyValues arrays are different.-or-
		//     con is static or private.-or- The number of supplied arguments does not match
		//     the number of parameters of the constructor as required by the calling convention
		//     of the constructor.-or- The type of supplied argument does not match the type
		//     of the parameter declared in the constructor.-or- The types of the property values
		//     do not match the types of the named properties.-or- A property has no setter
		//     method.-or- The property does not belong to the same class or base class as the
		//     constructor. -or-A supplied argument or named property is a reference type other
		//     than System.String or System.Type.
		//
		//   T:System.ArgumentNullException:
		//     One of the parameters is null.
		[OpenSilver.NotImplemented]
		public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues)
		{
			
		}
		//
		// Summary:
		//     Initializes a new instance of the CustomAttributeBuilder class, given the constructor
		//     for the custom attribute, the arguments to the constructor, and a set of named
		//     field/value pairs.
		//
		// Parameters:
		//   con:
		//     The constructor for the custom attribute.
		//
		//   constructorArgs:
		//     The arguments to the constructor of the custom attribute.
		//
		//   namedFields:
		//     Named fields of the custom attribute.
		//
		//   fieldValues:
		//     Values for the named fields of the custom attribute.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     The lengths of the namedFields and fieldValues arrays are different.-or- con
		//     is static or private.-or- The number of supplied arguments does not match the
		//     number of parameters of the constructor as required by the calling convention
		//     of the constructor.-or- The type of supplied argument does not match the type
		//     of the parameter declared in the constructor.-or- The types of the field values
		//     do not match the types of the named fields.-or- The field does not belong to
		//     the same class or base class as the constructor. -or-A supplied argument or named
		//     field is a reference type other than System.String or System.Type.
		//
		//   T:System.ArgumentNullException:
		//     One of the parameters is null.
		[OpenSilver.NotImplemented]
		public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, FieldInfo[] namedFields, object[] fieldValues)
		{
			
		}
		//
		// Summary:
		//     Initializes a new instance of the CustomAttributeBuilder class, given the constructor
		//     for the custom attribute, the arguments to the constructor, a set of named property
		//     or value pairs, and a set of named field or value pairs.
		//
		// Parameters:
		//   con:
		//     The constructor for the custom attribute.
		//
		//   constructorArgs:
		//     The arguments to the constructor of the custom attribute.
		//
		//   namedProperties:
		//     Named properties of the custom attribute.
		//
		//   propertyValues:
		//     Values for the named properties of the custom attribute.
		//
		//   namedFields:
		//     Named fields of the custom attribute.
		//
		//   fieldValues:
		//     Values for the named fields of the custom attribute.
		//
		// Exceptions:
		//   T:System.ArgumentException:
		//     The lengths of the namedProperties and propertyValues arrays are different.-or-
		//     The lengths of the namedFields and fieldValues arrays are different.-or- con
		//     is static or private.-or- The number of supplied arguments does not match the
		//     number of parameters of the constructor as required by the calling convention
		//     of the constructor.-or- The type of supplied argument does not match the type
		//     of the parameter declared in the constructor.-or- The types of the property values
		//     do not match the types of the named properties.-or- The types of the field values
		//     do not match the types of the corresponding field types.-or- A property has no
		//     setter.-or- The property or field does not belong to the same class or base class
		//     as the constructor. -or-A supplied argument, named property, or named field is
		//     a reference type other than System.String or System.Type.
		//
		//   T:System.ArgumentNullException:
		//     One of the parameters is null.
		[OpenSilver.NotImplemented]
		public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues, FieldInfo[] namedFields, object[] fieldValues)
		{
			
		}
	}
}
