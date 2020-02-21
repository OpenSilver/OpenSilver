#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	public partial class RichTextBox : Control
	{
		public string Xaml
		{
			get;
			set;
		}
	}
}
#endif