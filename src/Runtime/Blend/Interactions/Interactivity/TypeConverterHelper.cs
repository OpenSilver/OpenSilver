// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.ComponentModel;
using System;
using System.Globalization;
using OpenSilver.Internal.Xaml;

namespace Microsoft.Expression.Interactivity
{
	internal static class TypeConverterHelper
	{	
		internal static object DoConversionFrom(TypeConverter converter, object value)
		{
			object returnValue = value;

			try
			{
				if (converter != null && value != null && converter.CanConvertFrom(value.GetType()))
				{
					// This utility class is used to convert value that come from XAML, so we should use the invariant culture.
					returnValue = converter.ConvertFrom(context: null, culture: CultureInfo.InvariantCulture, value: value);
				}
			}
			catch (Exception e)
			{
				if (!TypeConverterHelper.ShouldEatException(e))
				{
					throw;
				}
			}

			return returnValue;
		}

		private static bool ShouldEatException(Exception e)
		{
			bool shouldEat = false;
			
			if (e.InnerException != null)
			{
				shouldEat |= ShouldEatException(e.InnerException);
			}

			shouldEat |= e is FormatException;
			return shouldEat;
		}

		internal static TypeConverter GetTypeConverter(Type type)
		{
			return RuntimeHelpers.GetTypeConverter(type);
		}
	}
}
