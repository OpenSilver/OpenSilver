#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	//
	// Summary:
	//     Provides static, predefined format names for data objects. Use the named constants
	//     to identify the format of the data that you request from an System.Windows.IDataObject
	//     object.
	public static partial class DataFormats
	{
		//
		// Summary:
		//     Specifies the Microsoft Windows file drop format.
		//
		// Returns:
		//     The string specifying the Microsoft Windows file drop format.
		public static readonly string FileDrop;
	}
}
#endif