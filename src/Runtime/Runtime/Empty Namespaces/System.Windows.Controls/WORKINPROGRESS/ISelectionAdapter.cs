#if WORKINPROGRESS

using System.Collections;
using System.Windows.Input;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public interface ISelectionAdapter
    {
        event SelectionChangedEventHandler SelectionChanged;
        event RoutedEventHandler Commit;
        event RoutedEventHandler Cancel;
        IEnumerable ItemsSource { get; set; }
        Object SelectedItem { get; set; }
        void HandleKeyDown(KeyEventArgs e);
    }
}
#endif