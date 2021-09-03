
#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	public partial class DataTemplate
	{
		[OpenSilver.NotImplemented]
		public object DataTemplateKey { get; }
		[OpenSilver.NotImplemented]
		public object DataType { get; set; }
	}
}
