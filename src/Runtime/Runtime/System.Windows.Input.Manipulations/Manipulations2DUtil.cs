#if MIGRATION
namespace System.Windows.Input.Manipulations
#else
namespace Windows.UI.Xaml.Input.Manipulations
#endif
{
	internal static class Manipulations2DUtil
	{
		public static bool IsValid(this Manipulations2D value)
		{
			int num = (int)(value & ~(Manipulations2D.TranslateX | Manipulations2D.TranslateY | Manipulations2D.Scale | Manipulations2D.Rotate));
			return num == 0;
		}

		public static void CheckValue(this Manipulations2D value, string property)
		{
			if (!value.IsValid())
			{
				throw Exceptions.ArgumentOutOfRange(property, value);
			}
		}

		public static bool SupportsAny(this Manipulations2D value, Manipulations2D supported)
		{
			return (value & supported) != Manipulations2D.None;
		}
	}
}