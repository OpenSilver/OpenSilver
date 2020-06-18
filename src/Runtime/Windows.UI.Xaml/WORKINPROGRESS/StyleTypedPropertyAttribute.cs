#if WORKINPROGRESS
using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	//
	// Summary:
	//     Represents an attribute that is applied to the class definition and reports the
	//     System.Windows.Style.TargetType of the properties that are of type System.Windows.Style.
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed partial class StyleTypedPropertyAttribute : Attribute
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.StyleTypedPropertyAttribute
		//     class.
		public StyleTypedPropertyAttribute()
		{
		}

		//
		// Summary:
		//     Gets or sets the name of the property that is of type System.Windows.Style.
		//
		// Returns:
		//     The name of the property.
		public string Property
		{
			get;
			set;
		}

		//
		// Summary:
		//     Gets or sets the System.Windows.Style.TargetType of the System.Windows.StyleTypedPropertyAttribute.Property
		//     this attribute is specifying.
		//
		// Returns:
		//     The System.Windows.Style.TargetType of the System.Windows.StyleTypedPropertyAttribute.Property
		//     this attribute is specifying.
		public Type StyleTargetType
		{
			get;
			set;
		}
	}
}
#endif