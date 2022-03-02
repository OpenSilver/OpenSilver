using System;
using System.Diagnostics;
#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	internal static class Validations
	{
		public static void CheckNotNull(object value, string paramName)
		{
			Debug.Assert(paramName != null);
			if (value == null)
			{
				throw new ArgumentNullException(paramName);
			}
		}

		public static bool IsFinite(float value)
		{
			return !float.IsNaN(value) && !float.IsInfinity(value);
		}

		public static void CheckFinite(float value, string paramName)
		{
			Debug.Assert(paramName != null);
			if (!Validations.IsFinite(value))
			{
				throw Exceptions.ValueMustBeFinite(paramName, value);
			}
		}

		public static bool IsFiniteOrNaN(float value)
		{
			return float.IsNaN(value) || !float.IsInfinity(value);
		}

		public static void CheckFiniteOrNaN(float value, string paramName)
		{
			Debug.Assert(paramName != null);
			if (!Validations.IsFiniteOrNaN(value))
			{
				throw Exceptions.ValueMustBeFiniteOrNaN(paramName, value);
			}
		}

		public static bool IsFiniteNonNegative(float value)
		{
			return !float.IsInfinity(value) && !float.IsNaN(value) && value >= 0f;
		}

		public static void CheckFiniteNonNegative(float value, string paramName)
		{
			if (!Validations.IsFiniteNonNegative(value))
			{
				throw Exceptions.ValueMustBeFiniteNonNegative(paramName, value);
			}
		}
	}
}