#if WORKINPROGRESS
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
	public partial struct Point : IFormattable
	{
		/// <summary>
		/// Creates a System.String representation of this Windows.Foundation.Point.
		/// </summary>
		/// <param name="provider">Culture-specific formatting information.</param>
		/// <returns>
		/// A System.String containing the Windows.Foundation.Point.X and Windows.Foundation.Point.Y
		/// values of this Windows.Foundation.Point structure.
		/// </returns>
		public string ToString(IFormatProvider provider)
		{
			return default(string);
		}
	}
}
#endif