#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    [OpenSilver.NotImplemented]
	public sealed partial class VisualTreeHelper
	{
		/// <summary>
		/// Returns an object's root object in the visual tree.
		/// </summary>
		/// <param name="reference">The object to get the root object for.</param>
		/// <returns>The root object of the reference object in the visual tree.</returns>
        [OpenSilver.NotImplemented]
		public static DependencyObject GetRoot(DependencyObject reference)
		{
			return default(DependencyObject);
		}
	}
}

#endif