#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	public partial class DataTemplate
	{
		public object DataTemplateKey { get; }
		public object DataType { get; set; }
	}
}

#endif