using System;
#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	internal static class DoubleUtil
	{
		public static bool IsZero(double d)
		{
			return Math.Abs(d) <= 2.220446049250313E-16;
		}

		public static double Limit(double d, double min, double max)
		{
			double result;
			if (!double.IsNaN(max) && d > max)
			{
				result = max;
			}
			else if (!double.IsNaN(min) && d < min)
			{
				result = min;
			}
			else
			{
				result = d;
			}
			return result;
		}

		private const double DBL_EPSILON = 2.220446049250313E-16;
	}
}