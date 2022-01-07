// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See THIRD-PARTY-NOTICES file in the project root for full license information.

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace Microsoft.Expression.Interactivity
{
	internal static class TypeConverterHelper
	{
		internal class ExtendedStringConverter : TypeConverter
		{
			private Type type;

			public ExtendedStringConverter(Type type)
			{
				this.type = type;
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (typeof(IConvertible).IsAssignableFrom(type) && typeof(IConvertible).IsAssignableFrom(destinationType))
				{
					return true;
				}

				return base.CanConvertTo(context, destinationType);
			}

			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				if (sourceType != typeof(string) && sourceType != typeof(uint))
				{
					return base.CanConvertFrom(context, sourceType);
				}

				return true;
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (value is IConvertible convertible && typeof(IConvertible).IsAssignableFrom(destinationType))
				{
					return convertible.ToType(destinationType, CultureInfo.InvariantCulture);
				}

				return base.ConvertTo(context, culture, value, destinationType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if (value is string text)
				{
					Type type = this.type;

					Type underlyingType = Nullable.GetUnderlyingType(type);
					
					if (underlyingType != null)
					{
						if (string.Equals(text, "null", StringComparison.OrdinalIgnoreCase))
						{
							return null;
						}

						type = underlyingType;
					}
					else if (type.IsGenericType)
					{
						return base.ConvertFrom(context, culture, value);
					}

					object obj = new object();
					object convertedObj = obj;

					if (type == typeof(bool))
					{
						convertedObj = bool.Parse(text);
					}
					else if (type.IsEnum)
					{
						convertedObj = Enum.Parse(this.type, text, ignoreCase: false);
					}
                    else if (type == typeof(int))
                    {
						convertedObj = int.Parse(text);
                    }
					else
					{
						var stringBuilder = new StringBuilder();

						var ns = "clr-namespace:" + type.Namespace + ";assembly=" + type.Assembly.FullName.Split(',')[0];

						stringBuilder.Append("<ContentControl xmlns='http://schemas.microsoft.com/client/2007' xmlns:c='" + ns + "'>\n");
						stringBuilder.Append("<c:" + type.Name + ">\n");
						stringBuilder.Append(text);
						stringBuilder.Append("</c:" + type.Name + ">\n");
						stringBuilder.Append("</ContentControl>");

						if (XamlReader.Load(stringBuilder.ToString()) is ContentControl contentControl)
						{
							convertedObj = contentControl.Content;
						}
					}

					if (convertedObj != obj)
					{
						return convertedObj;
					}
				}
				else if (value is uint)
				{
					if (this.type == typeof(bool))
					{
						return ((uint)value != 0) ? true : false;
					}

					if (this.type.IsEnum)
					{
						return Enum.Parse(this.type, Enum.GetName(this.type, value), ignoreCase: false);
					}
				}

				return base.ConvertFrom(context, culture, value);
			}
		}

		internal static object DoConversionFrom(TypeConverter converter, object value)
		{
			object result = value;

			try
			{
				if (converter != null)
				{
					if (value != null)
					{
						if (converter.CanConvertFrom(value.GetType()))
						{
							result = converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
							return result;
						}

						return result;
					}

					return result;
				}

				return result;
			}
			catch (Exception e)
			{
				if (!ShouldEatException(e))
				{
					throw;
				}

				return result;
			}
		}

		private static bool ShouldEatException(Exception e)
		{
			bool flag = false;

			if (e.InnerException != null)
			{
				flag |= ShouldEatException(e.InnerException);
			}

			return flag || e is FormatException;
		}

		internal static TypeConverter GetTypeConverter(Type type)
		{
			var typeConverterAttribute = (TypeConverterAttribute)Attribute.GetCustomAttribute(type, typeof(TypeConverterAttribute), inherit: false);

			if (typeConverterAttribute != null)
			{
				try
				{
					Type typeConverter = Type.GetType(typeConverterAttribute.ConverterTypeName, throwOnError: false);

					if (typeConverter != null)
					{
						return Activator.CreateInstance(typeConverter) as TypeConverter;
					}
				}
				catch
				{
				}
			}

			return new ExtendedStringConverter(type);
		}
	}
}
