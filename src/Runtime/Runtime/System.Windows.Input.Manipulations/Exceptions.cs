using System.Diagnostics;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	internal static class Exceptions
	{
		public static Exception ValueMustBeFinite(string paramName, object value)
		{
			
			return Exceptions.ArgumentOutOfRange(paramName, value, Resources.ValueMustBeFinite);
		}

		public static Exception ValueMustBeFiniteOrNaN(string paramName, object value)
		{
			return Exceptions.ArgumentOutOfRange(paramName, value, Resources.ValueMustBeFiniteOrNaN);
		}

		public static Exception ValueMustBeFiniteNonNegative(string paramName, object value)
		{
			return Exceptions.ArgumentOutOfRange(paramName, value, Resources.ValueMustBeFiniteNonNegative);
		}

		public static Exception IllegalPivotRadius(string paramName, object value)
		{
			return Exceptions.ArgumentOutOfRange(paramName, value, Resources.IllegalPivotRadius);
		}

		public static Exception IllegialInertiaRadius(string paramName, object value)
		{
			return Exceptions.ArgumentOutOfRange(paramName, value, Resources.InertiaProcessorInvalidRadius);
		}

		public static Exception InvalidTimestamp(string paramName, long value)
		{
			return Exceptions.ArgumentOutOfRange(paramName, value, Resources.InvalidTimestamp);
		}

		public static Exception ArgumentOutOfRange(string paramName, object value)
		{
			return Exceptions.ArgumentOutOfRange(paramName, value, Resources.ArgumentOutOfRange);
		}

		public static Exception OnlyProportionalExpansionSupported(string paramName1, string paramName2)
		{
			Debug.Assert(paramName1 != null);
			Debug.Assert(paramName2 != null);
			return new NotSupportedException(Exceptions.Format(Resources.OnlyProportionalExpansionSupported, new object[]
			{
				paramName1,
				paramName2
			}));
		}

		public static Exception InertiaParametersUnspecified2(string paramName1, string paramName2)
		{
			Debug.Assert(paramName1 != null);
			Debug.Assert(paramName2 != null);
			return new InvalidOperationException(Exceptions.Format(Resources.InertiaParametersUnspecified2, new object[]
			{
				paramName1,
				paramName2
			}));
		}

		public static Exception InertiaParametersUnspecified1and2(string paramName1, string paramName2a, string paramName2b)
		{
			Debug.Assert(paramName1 != null);
			Debug.Assert(paramName2a != null);
			Debug.Assert(paramName2b != null);
			return new InvalidOperationException(Exceptions.Format(Resources.InertiaParametersUnspecified1and2, new object[]
			{
				paramName1,
				paramName2a,
				paramName2b
			}));
		}

		public static Exception CannotChangeParameterDuringInertia(string paramName)
		{
			return new InvalidOperationException(Exceptions.Format(Resources.CannotChangeParameterDuringInertia, new object[]
			{
				paramName
			}));
		}

		public static Exception NoInertiaVelocitiesSpecified(string linearVelocityXParamName, string linearVelocityYParamName, string angularVelocityParamName, string expansionVelocityXParamName, string expansionVelocityYParamName)
		{
			return new InvalidOperationException(Exceptions.Format(Resources.NoInertiaVelocitiesSpecified, new object[]
			{
				linearVelocityXParamName,
				linearVelocityYParamName,
				angularVelocityParamName,
				expansionVelocityXParamName,
				expansionVelocityYParamName
			}));
		}

		private static ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, object value, string messageFormat)
		{
			Debug.Assert(paramName != null);
			Debug.Assert(messageFormat != null);
			string text = Exceptions.IsPropertyName(paramName) ? "value" : paramName;
			string text2 = string.Format(CultureInfo.CurrentCulture, messageFormat, new object[]
			{
				paramName
			});
			return new ArgumentOutOfRangeException(text, text2);
		}

		private static bool IsPropertyName(string paramName)
		{
			Debug.Assert(paramName != null);
			Debug.Assert(paramName.Length > 0);
			char c = paramName.ToCharArray()[0];
			return c >= 'A' && c <= 'Z';
		}

		private static string Format(string format, params object[] args)
		{
			Debug.Assert(format != null);
			return string.Format(CultureInfo.CurrentCulture, format, args);
		}
	}
}