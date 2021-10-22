#if MIGRATION
namespace System.Windows.Printing
#else
namespace Windows.UI.Xaml.Printing
#endif
{
	[OpenSilver.NotImplemented]
	public partial class PrinterFallbackSettings
	{
		//
		// Summary:
		//     Gets or sets whether all printing is forced to print in vector format.
		//
		// Returns:
		//     true to indicate all printing is forced to print in vector format; otherwise
		//     false. The default is true.
		[OpenSilver.NotImplemented]
		public bool ForceVector
		{
			get;
			set;
		}

		//
		// Summary:
		//     Gets or sets the opacity value of visual elements at which Silverlight will round
		//     the opacity to 1.0 to support vector printing on PostScript printers or drivers.
		//
		// Returns:
		//     The opacity value of visual elements at which Silverlight rounds the opacity
		//     to 1.0 to support vector printing on Postscript printers or drivers. The default
		//     is 0.
		[OpenSilver.NotImplemented]
		public double OpacityThreshold
		{
			get;
			set;
		}
	}
}
