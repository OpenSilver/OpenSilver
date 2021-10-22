#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	public delegate void DragEventHandler(object sender, DragEventArgs e);
}
