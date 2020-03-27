#if WORKINPROGRESS

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
	public sealed partial class CompositeTransform : Transform
	{
		/// <summary>
		/// Gets or sets the x-coordinate of the center point for all transforms specified by the System.Windows.Media.CompositeTransform.
		/// </summary>
		/// <returns>The x-coordinate of the center point for all transforms specified by the System.Windows.Media.CompositeTransform.</returns>
		public double CenterX { get; set; }
		
		/// <summary>
		/// Gets or sets the y-coordinate of the center point for all transforms specified by the System.Windows.Media.CompositeTransform.
		/// </summary>
		/// <returns>The y-coordinate of the center point for all transforms specified by the System.Windows.Media.CompositeTransform.</returns>
		public double CenterY { get; set; }
	}
}
#endif