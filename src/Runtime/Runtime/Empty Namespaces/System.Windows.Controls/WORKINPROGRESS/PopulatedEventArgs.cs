#if WORKINPROGRESS

using System;
using System.Windows.Documents;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class PopulatedEventArgs
    {

    }

    public delegate void RoutedPropertyChangingEventHandler<T>(Object sender, RoutedPropertyChangingEventArgs<T> e);
}
#endif