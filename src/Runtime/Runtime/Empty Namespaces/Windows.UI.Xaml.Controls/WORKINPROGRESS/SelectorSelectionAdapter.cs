#if WORKINPROGRESS

using System;
using System.Collections;
#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Input;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public class SelectorSelectionAdapter : ISelectionAdapter
    {
        public SelectorSelectionAdapter(Selector s)
        {

        }
        public IEnumerable ItemsSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public object SelectedItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event SelectionChangedEventHandler SelectionChanged;
        public event RoutedEventHandler Commit;
        public event RoutedEventHandler Cancel;

        public void HandleKeyDown(KeyEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
#endif